using System.Net.Http.Json;
using System.Text.Json;

namespace PadelManager.WinForms.Admin.Services;

public class ConnexionResultat {
    public string Matricule { get; set; } = null!;
    public string TypeUtilisateur { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int? SiteId { get; set; }
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

public class HoraireSiteRequete {
    // Matricule de l'admin appelant (issue #13, contrôle de portée serveur).
    public string AdminMatricule { get; set; } = null!;
    public List<string> JoursOuverture { get; set; } = new();
    public TimeOnly HeureDebutReservation { get; set; }
    public TimeOnly HeureFinReservation { get; set; }
}

public class JourFermetureResultat {
    public int Id { get; set; }
    public int? SiteId { get; set; }
    public DateOnly Date { get; set; }
}

// SiteId == null -> fermeture ponctuelle globale (tous les sites), réservée à l'admin global.
public class JourFermetureRequete {
    // Matricule de l'admin appelant (issue #13, contrôle de portée serveur).
    public string AdminMatricule { get; set; } = null!;
    public int? SiteId { get; set; }
    public DateOnly Date { get; set; }
}

public class FermetureHebdoGlobaleResultat {
    public int Annee { get; set; }
    public List<string> JoursFermes { get; set; } = new();
}

public class FermetureHebdoGlobaleRequete {
    // Matricule de l'admin appelant (issue #13, contrôle de portée serveur).
    public string AdminMatricule { get; set; } = null!;
    public List<string> JoursFermes { get; set; } = new();
}

// État d'un match pour la vue administrateur (EF-bk-014) : contrairement aux écrans Membre, les
// identifiants (match, terrain) sont affichés pour faciliter le contrôle/debug par l'admin.
public class AdminMatchResultat {
    public int Id { get; set; }
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int TerrainId { get; set; }
    public int NumeroTerrain { get; set; }
    public DateTime DateHeure { get; set; }
    public string Visibilite { get; set; } = null!;
    public string Statut { get; set; } = null!;
}

public class ChiffreAffairesResultat {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public decimal Montant { get; set; }
}

public class TerrainRecapResultat {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public List<int> Numeros { get; set; } = new();
}

public class StatistiquesResultat {
    public int SiteId { get; set; }
    public string NomSite { get; set; } = null!;
    public int NombreMatchsPublics { get; set; }
    public int NombreMatchsPrives { get; set; }
    public decimal TauxOccupation { get; set; }
    public int MembresActifs { get; set; }
}

public class MembreAdminResultat {
    public string Matricule { get; set; } = null!;
    public string TypeMembre { get; set; } = null!;
    public int? SiteId { get; set; }
    public bool DetteActive { get; set; }
    public bool PenaliteActive { get; set; }
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

    public async Task<HoraireSiteResultat?> ObtenirHoraireAsync(int siteId, int annee, string adminMatricule) {
        var response = await _httpClient.GetAsync($"api/sites/{siteId}/horaires/{annee}?adminMatricule={Uri.EscapeDataString(adminMatricule)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<HoraireSiteResultat>();
    }

    public async Task<ApiResult<HoraireSiteResultat>> DefinirHoraireAsync(int siteId, int annee, HoraireSiteRequete requete) {
        var response = await _httpClient.PutAsJsonAsync($"api/sites/{siteId}/horaires/{annee}", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<HoraireSiteResultat> { Succes = false, Message = message };
        }

        var horaire = await response.Content.ReadFromJsonAsync<HoraireSiteResultat>();
        return new ApiResult<HoraireSiteResultat> { Succes = true, Data = horaire };
    }

    public async Task<List<JourFermetureResultat>?> ObtenirFermeturesPonctuellesAsync(int siteId, int annee, string adminMatricule) {
        var response = await _httpClient.GetAsync($"api/sites/{siteId}/fermetures-ponctuelles?annee={annee}&adminMatricule={Uri.EscapeDataString(adminMatricule)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<JourFermetureResultat>>();
    }

    public async Task<ApiResult<JourFermetureResultat>> DeclarerFermetureAsync(JourFermetureRequete requete) {
        var response = await _httpClient.PostAsJsonAsync("api/fermetures-ponctuelles", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<JourFermetureResultat> { Succes = false, Message = message };
        }

        var fermeture = await response.Content.ReadFromJsonAsync<JourFermetureResultat>();
        return new ApiResult<JourFermetureResultat> { Succes = true, Data = fermeture };
    }

    public async Task<bool> SupprimerFermetureAsync(int id, string adminMatricule) {
        var response = await _httpClient.DeleteAsync($"api/fermetures-ponctuelles/{id}?adminMatricule={Uri.EscapeDataString(adminMatricule)}");
        return response.IsSuccessStatusCode;
    }

    public async Task<FermetureHebdoGlobaleResultat?> ObtenirFermetureHebdoGlobaleAsync(int annee, string adminMatricule) {
        var response = await _httpClient.GetAsync($"api/fermetures-hebdo-globales/{annee}?adminMatricule={Uri.EscapeDataString(adminMatricule)}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<FermetureHebdoGlobaleResultat>();
    }

    public async Task<ApiResult<FermetureHebdoGlobaleResultat>> DefinirFermetureHebdoGlobaleAsync(int annee, FermetureHebdoGlobaleRequete requete) {
        var response = await _httpClient.PutAsJsonAsync($"api/fermetures-hebdo-globales/{annee}", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<FermetureHebdoGlobaleResultat> { Succes = false, Message = message };
        }

        var fermeture = await response.Content.ReadFromJsonAsync<FermetureHebdoGlobaleResultat>();
        return new ApiResult<FermetureHebdoGlobaleResultat> { Succes = true, Data = fermeture };
    }

    public async Task<bool> SupprimerFermetureHebdoGlobaleAsync(int annee, string adminMatricule) {
        var response = await _httpClient.DeleteAsync($"api/fermetures-hebdo-globales/{annee}?adminMatricule={Uri.EscapeDataString(adminMatricule)}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<AdminMatchResultat>?> ObtenirEtatMatchsAsync(int? siteId, string adminMatricule) {
        var url = siteId.HasValue
            ? $"api/matchs/etat?siteId={siteId}&adminMatricule={Uri.EscapeDataString(adminMatricule)}"
            : $"api/matchs/etat?adminMatricule={Uri.EscapeDataString(adminMatricule)}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<AdminMatchResultat>>();
    }

    public async Task<List<TerrainRecapResultat>?> ObtenirRecapitulatifTerrainsAsync(int? siteId, string adminMatricule) {
        var url = siteId.HasValue
            ? $"api/matchs/terrains?siteId={siteId}&adminMatricule={Uri.EscapeDataString(adminMatricule)}"
            : $"api/matchs/terrains?adminMatricule={Uri.EscapeDataString(adminMatricule)}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<TerrainRecapResultat>>();
    }

    public async Task<List<ChiffreAffairesResultat>?> ObtenirChiffreAffairesAsync(int? siteId, string adminMatricule) {
        var url = siteId.HasValue
            ? $"api/statistiques/chiffre-affaires?siteId={siteId}&adminMatricule={Uri.EscapeDataString(adminMatricule)}"
            : $"api/statistiques/chiffre-affaires?adminMatricule={Uri.EscapeDataString(adminMatricule)}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<ChiffreAffairesResultat>>();
    }

    public async Task<List<StatistiquesResultat>?> ObtenirStatistiquesAsync(int? siteId, string adminMatricule) {
        var url = siteId.HasValue
            ? $"api/statistiques?siteId={siteId}&adminMatricule={Uri.EscapeDataString(adminMatricule)}"
            : $"api/statistiques?adminMatricule={Uri.EscapeDataString(adminMatricule)}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<StatistiquesResultat>>();
    }

    public async Task<List<MembreAdminResultat>?> ObtenirMembresAsync(int? siteId, string adminMatricule) {
        var url = siteId.HasValue
            ? $"api/membres?siteId={siteId}&adminMatricule={Uri.EscapeDataString(adminMatricule)}"
            : $"api/membres?adminMatricule={Uri.EscapeDataString(adminMatricule)}";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<MembreAdminResultat>>();
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
