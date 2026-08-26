using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PadelManager.Interfaces;
using PadelManager.Repositories;
using Xunit;

namespace PadelManager.Tests;

// Tests contre une vraie instance SQL Server (pas InMemory) : InscrireEtPayerAsync,
// PayerParticipationAsync et VerrouillerMatchAsync reposent sur FromSqlInterpolated (WITH
// (UPDLOCK, HOLDLOCK)), non supporté par le provider InMemory ; AddAsync s'appuie sur la
// contrainte réelle UQ_MATCH_terrain_creneau, qu'InMemory n'impose pas. Cf. les commentaires dans
// MatchRepositoryTests.cs expliquant pourquoi ces méthodes n'y sont pas testées — ce fichier les
// couvre à la place.
//
// Nécessite deux variables d'environnement, non versionnées (comme
// PadelManager.Api/appsettings.Development.json, mêmes identifiants padel_api pour la première) :
// - PADEL_TEST_SQLSERVER_CONNECTION : connexion padel_api vers PadelDB — celle utilisée par le
//   repository testé, pour rester fidèle aux permissions réelles de l'API (padel_api n'a d'ailleurs
//   pas le droit DELETE sur MATCH/PARTICIPATION/PAIEMENT, d'où la connexion de nettoyage séparée).
// - PADEL_TEST_SQLSERVER_CLEANUP_CONNECTION : connexion de confiance (dbo), utilisée uniquement en
//   nettoyage de fin de test pour supprimer les matchs créés.
//
// Chaque test crée son propre MATCH (site 1 / terrain 11, données de référence réelles jamais
// modifiées par ces tests) à une date éloignée dans le futur (2099+) pour ne jamais entrer en
// collision avec les données seedées (toutes en 2026) ni avec un autre test de cette classe.
public class MatchRepositorySqlServerTests : IAsyncLifetime {
    private const string VarConnexion = "PADEL_TEST_SQLSERVER_CONNECTION";
    private const string VarConnexionNettoyage = "PADEL_TEST_SQLSERVER_CLEANUP_CONNECTION";

    private const int SiteId = 1;
    private const int TerrainId = 11;

    private PadelManagerDbContext _context = null!;
    private MatchRepository _repository = null!;
    private readonly List<int> _matchIdsACreer = new();

    public Task InitializeAsync() {
        var connexion = Environment.GetEnvironmentVariable(VarConnexion)
            ?? throw new InvalidOperationException(
                $"Variable d'environnement {VarConnexion} absente : ces tests nécessitent une vraie " +
                "connexion SQL Server (compte padel_api) — voir le commentaire en tête de cette classe.");

        var options = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseSqlServer(connexion)
            .Options;
        _context = new PadelManagerDbContext(options);
        _repository = new MatchRepository(_context);

        return Task.CompletedTask;
    }

    public async Task DisposeAsync() {
        await _context.DisposeAsync();

        if (_matchIdsACreer.Count == 0)
            return;

        var connexionNettoyage = Environment.GetEnvironmentVariable(VarConnexionNettoyage)
            ?? throw new InvalidOperationException(
                $"Variable d'environnement {VarConnexionNettoyage} absente : nécessaire pour nettoyer " +
                "les matchs de test créés (padel_api n'a pas de droit DELETE).");

        await using var connexion = new SqlConnection(connexionNettoyage);
        await connexion.OpenAsync();

        foreach (var matchId in _matchIdsACreer) {
            await using var commande = connexion.CreateCommand();
            // SET QUOTED_IDENTIFIER ON requis pour toute DML touchant PAIEMENT (colonne calculée
            // persistée montantTotal), même convention que database/04_insert.sql.
            commande.CommandText =
                "SET QUOTED_IDENTIFIER ON; " +
                "DELETE FROM PAIEMENT WHERE participationId IN (SELECT id FROM PARTICIPATION WHERE matchId = @matchId); " +
                "DELETE FROM PARTICIPATION WHERE matchId = @matchId; " +
                "DELETE FROM [MATCH] WHERE id = @matchId;";
            commande.Parameters.AddWithValue("@matchId", matchId);
            await commande.ExecuteNonQueryAsync();
        }
    }

    // Un créneau distinct par test (même terrain), loin dans le futur pour ne jamais collisionner
    // avec les données seedées (2026) ni avec les autres tests de cette classe.
    private static DateTime DateHeureLibre(int decalageMinutes) =>
        new DateTime(2099, 1, 1, 8, 0, 0).AddMinutes(decalageMinutes);

    private Match NouveauMatch(DateTime dateHeure, string visibilite = "PUBLIC") => new() {
        SiteId = SiteId, TerrainId = TerrainId, DateHeure = dateHeure,
        Visibilite = visibilite, OrganisateurMatricule = "G001", Statut = "INCOMPLET"
    };

    [Fact]
    public async Task InscrireEtPayerAsync_QuatriemeParticipation_BasculeStatutComplet() {
        // Arrange
        var match = NouveauMatch(DateHeureLibre(0));
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        _matchIdsACreer.Add(match.Id);

        foreach (var matricule in new[] { "G001", "G002", "G003" })
            await _repository.InscrireEtPayerAsync(match.Id, matricule, null);

        // Act : 4e participation.
        var resultat = await _repository.InscrireEtPayerAsync(match.Id, "G004", null);

        // Assert
        Assert.NotNull(resultat);
        Assert.NotNull(resultat.Paiement);

        var matchMisAJour = await _context.Matches.AsNoTracking().SingleAsync(m => m.Id == match.Id);
        Assert.Equal("COMPLET", matchMisAJour.Statut);

        var participations = await _context.Participations.AsNoTracking().Where(p => p.MatchId == match.Id).ToListAsync();
        Assert.Equal(4, participations.Count);
    }

    [Fact]
    public async Task InscrireEtPayerAsync_MembreDejaInscrit_LeveDejaInscritException() {
        // Arrange
        var match = NouveauMatch(DateHeureLibre(30));
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        _matchIdsACreer.Add(match.Id);

        await _repository.InscrireEtPayerAsync(match.Id, "G001", null);

        // Act & Assert
        await Assert.ThrowsAsync<DejaInscritException>(() => _repository.InscrireEtPayerAsync(match.Id, "G001", null));
    }

    [Fact]
    public async Task InscrireEtPayerAsync_MatchDejaComplet_LeveMatchCompletException() {
        // Arrange : 4 participations déjà en place.
        var match = NouveauMatch(DateHeureLibre(60));
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        _matchIdsACreer.Add(match.Id);

        foreach (var matricule in new[] { "G001", "G002", "G003", "S001" })
            await _repository.InscrireEtPayerAsync(match.Id, matricule, null);

        // Act & Assert : une 5e place n'existe pas.
        await Assert.ThrowsAsync<MatchCompletException>(() => _repository.InscrireEtPayerAsync(match.Id, "S002", null));
    }

    [Fact]
    public async Task PayerParticipationAsync_DerniereParticipationEnAttente_BasculeStatutComplet() {
        // Arrange : match privé à 3 participations déjà payées + 1 en attente.
        var match = NouveauMatch(DateHeureLibre(90), "PRIVE");
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        _matchIdsACreer.Add(match.Id);

        foreach (var matricule in new[] { "G001", "G002", "G003" })
            await _repository.InscrireEtPayerAsync(match.Id, matricule, null);

        var participationEnAttente = new Participation { MatchId = match.Id, MembreMatricule = "S001", DateInscription = DateTime.Now };
        _context.Participations.Add(participationEnAttente);
        await _context.SaveChangesAsync();

        // Act
        var resultat = await _repository.PayerParticipationAsync(participationEnAttente, null);

        // Assert
        Assert.NotNull(resultat.Paiement);
        var matchMisAJour = await _context.Matches.AsNoTracking().SingleAsync(m => m.Id == match.Id);
        Assert.Equal("COMPLET", matchMisAJour.Statut);
    }

    [Fact]
    public async Task InscrireEtPayerAsync_MatchInexistant_LeveInvalidOperationException() {
        // Act & Assert : VerrouillerMatchAsync ne trouve aucune ligne à verrouiller.
        await Assert.ThrowsAsync<InvalidOperationException>(() => _repository.InscrireEtPayerAsync(int.MaxValue, "G001", null));
    }

    [Fact]
    public async Task PayerParticipationAsync_DejaPayeeEntreTemps_LeveParticipationDejaPayeeException() {
        // Arrange : une participation en attente, vue depuis deux contextes indépendants — simule
        // deux requêtes concurrentes qui n'ont pas vu le paiement l'une de l'autre (PayerParticipationAsync
        // suit l'objet passé en paramètre, déjà tracké par SON propre contexte).
        var match = NouveauMatch(DateHeureLibre(150), "PRIVE");
        _context.Matches.Add(match);
        await _context.SaveChangesAsync();
        _matchIdsACreer.Add(match.Id);

        var participation = new Participation { MatchId = match.Id, MembreMatricule = "S001", DateInscription = DateTime.Now };
        _context.Participations.Add(participation);
        await _context.SaveChangesAsync();

        var optionsAutreContexte = new DbContextOptionsBuilder<PadelManagerDbContext>()
            .UseSqlServer(Environment.GetEnvironmentVariable(VarConnexion)!)
            .Options;
        await using var autreContexte = new PadelManagerDbContext(optionsAutreContexte);
        var autreRepository = new MatchRepository(autreContexte);
        var participationVueAilleurs = await autreContexte.Participations.FirstAsync(p => p.Id == participation.Id);

        // Act : la première requête paie normalement.
        await _repository.PayerParticipationAsync(participation, null);

        // Assert : la seconde, qui n'a pas vu ce paiement, se heurte à la vraie contrainte
        // UQ_PAIEMENT_participationId (pas imposée par InMemory).
        await Assert.ThrowsAsync<ParticipationDejaPayeeException>(
            () => autreRepository.PayerParticipationAsync(participationVueAilleurs, null));
    }

    [Fact]
    public async Task AddAsync_CreneauDejaPris_LeveCreneauIndisponibleException() {
        // Arrange : un premier match sur (terrain, créneau) déjà inséré et validé par la DB.
        var dateHeure = DateHeureLibre(120);
        var premier = NouveauMatch(dateHeure, "PRIVE");
        var cree = await _repository.AddAsync(premier);
        _matchIdsACreer.Add(cree.Id);

        var doublon = new Match { SiteId = SiteId, TerrainId = TerrainId, DateHeure = dateHeure, Visibilite = "PRIVE", OrganisateurMatricule = "G002", Statut = "INCOMPLET" };

        // Act & Assert : UQ_MATCH_terrain_creneau, imposée par la vraie base (pas par InMemory).
        await Assert.ThrowsAsync<CreneauIndisponibleException>(() => _repository.AddAsync(doublon));
    }
}
