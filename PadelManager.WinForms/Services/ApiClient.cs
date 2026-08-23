using System.Net.Http.Json;

namespace PadelManager.WinForms.Services;

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

public class DisponibiliteResultat {
    public int SiteId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly HeureDebut { get; set; }
    public TimeOnly HeureFin { get; set; }
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

    public async Task<List<DisponibiliteResultat>?> ConsulterPlanningAsync(int siteId, DateOnly from, DateOnly to) {
        var response = await _httpClient.GetAsync(
            $"api/sites/{siteId}/disponibilites?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<DisponibiliteResultat>>();
    }
}
