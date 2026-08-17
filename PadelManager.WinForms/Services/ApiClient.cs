using System.Net.Http.Json;
using System.Text.Json;

namespace PadelManager.WinForms.Services;

public class ConnexionResultat {
    public string Matricule { get; set; } = null!;
    public string TypeUtilisateur { get; set; } = null!;
    public string Type { get; set; } = null!;
    public int? SiteId { get; set; }
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
}


