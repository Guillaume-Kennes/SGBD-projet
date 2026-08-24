using System.Net.Http.Json;
using System.Text.Json;

namespace PadelManager.WinForms.Services;

public class ConnexionResultat {
    public string Matricule { get; set; } = null!;
    public string TypeUtilisateur { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int? SiteId { get; set; }
    public int? AnticipationMaxJours { get; set; }
}

public class SiteResultat {
    public int Id { get; set; }
    public string Nom { get; set; } = null!;
}

public class HoraireSiteResultat {
    public int SiteId { get; set; }
    public int Annee { get; set; }
    public List<string> JoursOuverture { get; set; } = new();
    public TimeOnly HeureDebutReservation { get; set; }
    public TimeOnly HeureFinReservation { get; set; }
}

public class JourFermetureResultat {
    public int Id { get; set; }
    public int? SiteId { get; set; }
    public DateOnly Date { get; set; }
}

public class CreneauMatchResultat {
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public TimeOnly HeureDebut { get; set; }
    public TimeOnly HeureFin { get; set; }
}

public class CreerMatchPriveRequete {
    public string OrganisateurMatricule { get; set; } = null!;
    public int SiteId { get; set; }
    public int TerrainId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly HeureDebut { get; set; }
    public List<string> Joueurs { get; set; } = new();
}

public class CreerMatchPublicRequete {
    public string OrganisateurMatricule { get; set; } = null!;
    public int SiteId { get; set; }
    public int TerrainId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly HeureDebut { get; set; }
}

public class MatchPublicResultat {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public int PlacesRestantes { get; set; }
}

public class RejoindreMatchRequete {
    public string MembreMatricule { get; set; } = null!;
}

public class InscriptionResultat {
    public bool Succes { get; set; }
    public string? MessageErreur { get; set; }
    public decimal? MontantPaye { get; set; }
    public bool DetteReglee { get; set; }
}

// Montant à afficher AVANT paiement (15€, + dette active éventuelle). Toujours récupéré à
// nouveau au chargement de l'écran plutôt que mis en cache depuis ConnexionResultat : une dette
// peut être réglée par une autre action en cours de session, un montant caché deviendrait faux.
public class MontantAPayerResultat {
    public decimal MontantParticipation { get; set; }
    public decimal? MontantDette { get; set; }
    public decimal MontantTotal { get; set; }
}

public class PayerParticipationRequete {
    public string MembreMatricule { get; set; } = null!;
}

// Participation à un match privé en attente de paiement : le membre y a été ajouté par
// l'organisateur à la création, sa place n'est confirmée qu'une fois payée (EF-bk-007).
public class ParticipationEnAttenteResultat {
    public int ParticipationId { get; set; }
    public int MatchId { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public string OrganisateurMatricule { get; set; } = null!;
}

public class MatchResultat {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public int TerrainId { get; set; }
    public DateTime DateHeure { get; set; }
    public string Visibilite { get; set; } = null!;
    public string OrganisateurMatricule { get; set; } = null!;
    public string Statut { get; set; } = null!;
    public List<string> Joueurs { get; set; } = new();
}

// Résultat enrichi (succès + message d'erreur éventuel) pour les appels dont l'échec doit être
// expliqué à l'utilisateur, contrairement à ConnexionAsync qui renvoie simplement null.
public class ApiResult<T> {
    public bool Succes { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

public class ApiClient {
    private readonly HttpClient _httpClient;

    public ApiClient() {
        _httpClient = new HttpClient {
            BaseAddress = new Uri("https://localhost:7033/")
        };
    }

    public async Task<ConnexionResultat?> ConnexionAsync(string matricule) {
        var response = await _httpClient.PostAsJsonAsync("api/auth/connexion", matricule);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<ConnexionResultat>();
    }

    public async Task<List<SiteResultat>?> ObtenirSitesAsync() {
        var response = await _httpClient.GetAsync("api/sites");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<SiteResultat>>();
    }

    public async Task<HoraireSiteResultat?> ObtenirHoraireAsync(int siteId, int annee) {
        var response = await _httpClient.GetAsync($"api/sites/{siteId}/horaires/{annee}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<HoraireSiteResultat>();
    }

    public async Task<List<JourFermetureResultat>?> ObtenirFermeturesPonctuellesAsync(int siteId, int annee) {
        var response = await _httpClient.GetAsync($"api/sites/{siteId}/fermetures-ponctuelles?annee={annee}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<JourFermetureResultat>>();
    }

    public async Task<List<CreneauMatchResultat>?> ObtenirCreneauxDisponiblesAsync(int siteId, DateOnly date) {
        var response = await _httpClient.GetAsync($"api/sites/{siteId}/creneaux-disponibles?date={date:yyyy-MM-dd}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<CreneauMatchResultat>>();
    }

    public async Task<ApiResult<MatchResultat>> CreerMatchPriveAsync(CreerMatchPriveRequete requete) {
        var response = await _httpClient.PostAsJsonAsync("api/matchs", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<MatchResultat> { Succes = false, Message = message };
        }

        var match = await response.Content.ReadFromJsonAsync<MatchResultat>();
        return new ApiResult<MatchResultat> { Succes = true, Data = match };
    }

    public async Task<ApiResult<MatchResultat>> CreerMatchPublicAsync(CreerMatchPublicRequete requete) {
        var response = await _httpClient.PostAsJsonAsync("api/matchs/publics", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<MatchResultat> { Succes = false, Message = message };
        }

        var match = await response.Content.ReadFromJsonAsync<MatchResultat>();
        return new ApiResult<MatchResultat> { Succes = true, Data = match };
    }

    public async Task<List<MatchPublicResultat>?> ObtenirMatchsPublicsAsync(string membreMatricule) {
        var response = await _httpClient.GetAsync($"api/matchs/publics?membreMatricule={Uri.EscapeDataString(membreMatricule)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<MatchPublicResultat>>();
    }

    public async Task<ApiResult<InscriptionResultat>> RejoindreMatchPublicAsync(int matchId, string membreMatricule) {
        var requete = new RejoindreMatchRequete { MembreMatricule = membreMatricule };
        var response = await _httpClient.PostAsJsonAsync($"api/matchs/{matchId}/inscription", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<InscriptionResultat> { Succes = false, Message = message };
        }

        var inscription = await response.Content.ReadFromJsonAsync<InscriptionResultat>();
        return new ApiResult<InscriptionResultat> { Succes = true, Data = inscription };
    }

    public async Task<MontantAPayerResultat?> ObtenirMontantAPayerAsync(string membreMatricule) {
        var response = await _httpClient.GetAsync($"api/matchs/montant-a-payer?membreMatricule={Uri.EscapeDataString(membreMatricule)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<MontantAPayerResultat>();
    }

    public async Task<List<ParticipationEnAttenteResultat>?> ObtenirParticipationsEnAttenteAsync(string membreMatricule) {
        var response = await _httpClient.GetAsync($"api/participations/en-attente?membreMatricule={Uri.EscapeDataString(membreMatricule)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<ParticipationEnAttenteResultat>>();
    }

    public async Task<ApiResult<InscriptionResultat>> PayerParticipationAsync(int participationId, string membreMatricule) {
        var requete = new PayerParticipationRequete { MembreMatricule = membreMatricule };
        var response = await _httpClient.PostAsJsonAsync($"api/participations/{participationId}/paiement", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<InscriptionResultat> { Succes = false, Message = message };
        }

        var inscription = await response.Content.ReadFromJsonAsync<InscriptionResultat>();
        return new ApiResult<InscriptionResultat> { Succes = true, Data = inscription };
    }

    private static async Task<string?> LireMessageErreurAsync(HttpResponseMessage response) {
        try {
            using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
            return document.RootElement.TryGetProperty("message", out var message) ? message.GetString() : null;
        } catch (JsonException) {
            return null;
        }
    }
}
