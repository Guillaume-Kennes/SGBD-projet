# PadelManager

Plateforme de gestion de terrains de padel multi-sites — projet académique.

Application de gestion des réservations pour un gestionnaire exploitant plusieurs sites de terrains de padel : création et visibilité de matchs (privé/public), paiement, statistiques, gestion des membres et des administrateurs, sur plusieurs sites indépendants (horaires, terrains, jours de fermeture propres à chaque site).

## Stack technique

| Composant | Technologie |
|---|---|
| Base de données | SQL Server |
| Back-end | ASP.NET Core Web API (C#) — Controllers / Services / Repositories / Modèles, injection de dépendances |
| Front-end | Windows Forms (.NET), consomme l'API en HTTP/JSON |
| Tests back-end | xUnit |
| Versionning | Git / GitHub — issues, branche par issue, `main` protégée |
| Intégration continue | GitHub Actions |

## Architecture

Solution unique `PadelManager.sln` composée de projets séparés, respectant une architecture en couches :

```
PadelManager/
├── PadelManager.Api/            → ASP.NET Core Web API (Controllers)
├── PadelManager.WinForms/       → Client Windows Forms
├── PadelManager.Services/       → Couche métier (Business Layer)
├── PadelManager.Repositories/   → Couche accès données (Data Access Layer)
├── PadelManager.Models/         → Entités / DTOs partagés
└── PadelManager.Tests/          → Tests xUnit (Controllers, Services, Repositories)
```

Références entre projets :
- `Repositories` → `Models`
- `Services` → `Repositories`, `Models`
- `Api` → `Services`, `Models`
- `Tests` → `Services`, `Repositories`, `Models`
- `WinForms` → aucune référence back-end (communication exclusivement via l'API REST en HTTP/JSON)

## Prérequis

- [.NET SDK](https://dotnet.microsoft.com/download) (version 9.0 utilisée par la solution)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (WinForms Designer, débogage) et/ou [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB ou instance complète)
- Git

## Mise en route

1. Cloner le dépôt :
   ```bash
   git clone https://github.com/Guillaume-Kennes/SGBD-projet.git
   ```
2. Ouvrir `PadelManager.sln` dans Visual Studio.
3. Restaurer les packages NuGet (automatique à l'ouverture, ou `dotnet restore`).
4. Configurer la chaîne de connexion à la base de données dans `PadelManager.Api/appsettings.Development.json` (fichier non versionné — voir `.gitignore`).
5. Lancer l'API :
   ```bash
   dotnet run --project PadelManager.Api
   ```
6. Lancer le client WinForms depuis Visual Studio (définir `PadelManager.WinForms` comme projet de démarrage, F5).
.NET
## Tests

```bash
dotnet test PadelManager.Tests
```

## Workflow Git

- Aucune modification directe sur `main` (branche protégée).
- Une branche dédiée par issue (`issue-<numéro>-<description>`).
- Pull request avant tout merge vers `main`.
- La pipeline GitHub Actions exécute les tests et calcule la couverture de code à chaque contribution.

## Documentation

Le cahier des charges complet (exigences fonctionnelles et non fonctionnelles, schéma relationnel, analyse technique) se trouve dans le dépôt / la documentation du projet.