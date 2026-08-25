using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using PadelManager.Services;

namespace PadelManager.Api.Jobs;

// Job quotidien (EF-bk-008/009/010) : bascule des matchs privés de demain, puis clôture des
// matchs d'hier. ENF-009 : exécuté à heure fixe (minuit). ENF-011 : déclenchement exclusivement
// automatique — aucun endpoint ni commande ne permet de le relancer manuellement, y compris dans
// ce code (voir plan de vérification en tests/HTTP, hors périmètre du livrable).
//
// Rattrapage au démarrage : ce projet ne tourne pas en continu comme un vrai service hébergé
// (l'API est arrêtée puis relancée entre les sessions de travail/démo) ; un passage de minuit peut
// donc avoir été manqué pendant que le processus était éteint. Une exécution immédiate au
// démarrage, avant la boucle normale vers le minuit suivant, comble ce trou — toujours 100%
// automatique (déclenchée par le démarrage du processus, jamais par une action extérieure). Aucun
// suivi de "dernière date exécutée" n'est nécessaire : bascule et clôture sont idempotentes par
// construction (JobService), donc exécuter le job une fois de plus au démarrage — même si un vrai
// passage à minuit a déjà eu lieu aujourd'hui pendant que l'app tournait — ne rebascule ni ne
// pénalise jamais deux fois le même match.
//
// Utilise sa propre connexion (padel_job, ENF-004) plutôt que le DbContext injecté par défaut de
// l'application (padel_api, réservé aux couches déclenchées par un utilisateur) : les deux comptes
// SQL sont dédiés à des processus distincts et n'ont pas les mêmes droits — d'où la construction
// manuelle d'un DbContext/repository/service dédiés à chaque exécution, plutôt qu'une résolution
// via le conteneur DI (qui ne connaît que la connexion padel_api).
public class JobQuotidienHostedService : BackgroundService {
    private readonly IConfiguration _configuration;
    private readonly ILogger<JobQuotidienHostedService> _logger;

    public JobQuotidienHostedService(IConfiguration configuration, ILogger<JobQuotidienHostedService> logger) {
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
        if (!stoppingToken.IsCancellationRequested)
            await ExecuterAsync();

        while (!stoppingToken.IsCancellationRequested) {
            var maintenant = DateTime.Now;
            var prochainMinuit = maintenant.Date.AddDays(1);

            try {
                await Task.Delay(prochainMinuit - maintenant, stoppingToken);
            } catch (TaskCanceledException) {
                break; // arrêt de l'application pendant l'attente
            }

            if (stoppingToken.IsCancellationRequested)
                break;

            await ExecuterAsync();
        }
    }

    private async Task ExecuterAsync() {
        try {
            var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
                .UseSqlServer(_configuration.GetConnectionString("PadelDbJob"))
                .Options;
            await using var context = new PadelManagerDbContext(options);
            var repository = new JobRepository(context);
            var service = new JobService(repository);

            // R-VAL-004 : le lendemain et la veille du jour d'exécution (aujourd'hui, puisque le
            // job tourne à minuit pile).
            var aujourdHui = DateOnly.FromDateTime(DateTime.Today);
            await service.ExecuterBasculeAsync(aujourdHui.AddDays(1));
            await service.ExecuterClotureAsync(aujourdHui.AddDays(-1));

            _logger.LogInformation("Job quotidien exécuté avec succès pour le {Date}.", aujourdHui);
        } catch (Exception ex) {
            // Une exécution en échec (ex. base momentanément indisponible) ne doit jamais arrêter
            // la boucle de planification : la nuit suivante retente naturellement.
            _logger.LogError(ex, "Échec de l'exécution du job quotidien.");
        }
    }
}
