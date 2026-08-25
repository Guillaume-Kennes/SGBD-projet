using Microsoft.EntityFrameworkCore;
using PadelManager.Api.Jobs;
using PadelManager.Interfaces;
using PadelManager.Repositories;
using PadelManager.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


builder.Services.AddDbContext<PadelManagerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PadelDb")));

builder.Services.AddScoped<IMembreRepository, MembreRepository>();
builder.Services.AddScoped<IAdministrateurRepository, AdministrateurRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ISiteRepository, SiteRepository>();
builder.Services.AddScoped<IHoraireSiteRepository, HoraireSiteRepository>();
builder.Services.AddScoped<IJourFermetureRepository, JourFermetureRepository>();
builder.Services.AddScoped<IFermetureHebdoGlobaleRepository, FermetureHebdoGlobaleRepository>();
builder.Services.AddScoped<IDisponibiliteRepository, DisponibiliteRepository>();
builder.Services.AddScoped<ITerrainRepository, TerrainRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IDetteRepository, DetteRepository>();
builder.Services.AddScoped<IPenaliteRepository, PenaliteRepository>();

builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IDisponibiliteGenerationService, DisponibiliteGenerationService>();
builder.Services.AddScoped<IHoraireSiteService, HoraireSiteService>();
builder.Services.AddScoped<IJourFermetureService, JourFermetureService>();
builder.Services.AddScoped<IFermetureHebdoGlobaleService, FermetureHebdoGlobaleService>();
builder.Services.AddScoped<IMatchService, MatchService>();

// Job quotidien (EF-bk-008/009/010, ENF-009/011) : construit sa propre connexion (padel_job) à
// chaque exécution plutôt que de dépendre du DbContext ci-dessus (padel_api) — voir
// JobQuotidienHostedService.
builder.Services.AddHostedService<JobQuotidienHostedService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


