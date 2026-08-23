using Microsoft.EntityFrameworkCore;
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

builder.Services.AddScoped<ISiteService, SiteService>();
builder.Services.AddScoped<IDisponibiliteGenerationService, DisponibiliteGenerationService>();
builder.Services.AddScoped<IHoraireSiteService, HoraireSiteService>();
builder.Services.AddScoped<IDisponibiliteService, DisponibiliteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


