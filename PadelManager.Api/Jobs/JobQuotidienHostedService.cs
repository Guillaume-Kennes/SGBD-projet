using Microsoft.EntityFrameworkCore;
using PadelManager.Repositories;
using PadelManager.Services;

namespace PadelManager.Api.Jobs;

// Job quotidien (EF-bk-008/009/010) : bascule des matchs privés de demain, puis clôture des
// matchs d'hier. ENF-009 : exécuté à heure fixe (minuit). ENF-011 : déclenchement exclusivement
// automatique — aucun endpoint ni commande ne permet de le relancer manuellement, y compris dans
// ce code (voir plan de vérification en tests/HTTP, hors périmètre du livrable).
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
