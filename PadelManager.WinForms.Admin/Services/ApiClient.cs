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
    public List<string> JoursOuverture { get; set; } = new();
    public TimeOnly HeureDebutReservation { get; set; }
    public TimeOnly HeureFinReservation { get; set; }
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

    public async Task<ApiResult<HoraireSiteResultat>> DefinirHoraireAsync(int siteId, int annee, HoraireSiteRequete requete) {
        var response = await _httpClient.PutAsJsonAsync($"api/sites/{siteId}/horaires/{annee}", requete);

        if (!response.IsSuccessStatusCode) {
            var message = await LireMessageErreurAsync(response);
            return new ApiResult<HoraireSiteResultat> { Succes = false, Message = message };
        }

        var horaire = await response.Content.ReadFromJsonAsync<HoraireSiteResultat>();
        return new ApiResult<HoraireSiteResultat> { Succes = true, Data = horaire };
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
