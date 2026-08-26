# PadelManager

Plateforme de gestion de terrains de padel multi-sites — projet académique.

Application de gestion des réservations pour un gestionnaire exploitant plusieurs sites de terrains de padel : création et visibilité de matchs (privé/public), paiement, statistiques, gestion des membres et des administrateurs, sur plusieurs sites indépendants (horaires, terrains, jours de fermeture propres à chaque site).

## Stack technique

| Composant | Technologie |
|---|---|
| Base de données | SQL Server |
| Back-end | ASP.NET Core Web API (C#) — Controllers / Services / Interfaces / Repositories / Modèles, injection de dépendances |
| Front-end | Windows Forms (.NET) — deux applications distinctes (Membre et Administration), consommant la même API en HTTP/JSON |
| Tests back-end | xUnit |
| Versionning | Git / GitHub — issues, branche par issue, `main` protégée |
| Intégration continue | GitHub Actions |

## Architecture

Solution unique `PadelManager.sln` composée de projets séparés, respectant une architecture en couches (ENF-002) :

```
PadelManager/
├── PadelManager.Api/              → ASP.NET Core Web API (Controllers)
├── PadelManager.Services/         → Couche métier (Business Layer)
├── PadelManager.Interfaces/       → Contrats (interfaces) des Repositories et Services
├── PadelManager.Repositories/     → Couche accès données (Data Access Layer, EF Core)
├── PadelManager.Models/           → Entités EF Core + DTOs partagés
├── PadelManager.WinForms/         → Client Windows Forms « Membre » (projet PadelManager.WinForms.Membre)
├── PadelManager.WinForms.Admin/   → Client Windows Forms « Administration »
└── PadelManager.Tests/            → Tests xUnit (Controllers, Services, Repositories)
```

Références entre projets (vérifiées dans les `.csproj`) :
- `Interfaces` → `Models`
- `Repositories` → `Interfaces`, `Models`
- `Services` → `Interfaces`, `Models` — **jamais** `Repositories` directement (ENF-002)
- `Api` → `Interfaces`, `Models`, `Repositories`, `Services` (seul projet à connaître à la fois les abstractions et leurs implémentations, pour l'injection de dépendances)
- `Tests` → `Api`, `Interfaces`, `Models`, `Repositories`, `Services`
- `WinForms` / `WinForms.Admin` → aucune référence aux projets back-end : communication exclusivement via l'API REST en HTTP/JSON

## Prérequis

- [.NET SDK](https://dotnet.microsoft.com/download) (version 9.0 utilisée par la solution)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) (WinForms Designer, débogage) et/ou [VS Code](https://code.visualstudio.com/)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB ou instance complète), avec un outil pour exécuter des scripts SQL (SQL Server Management Studio ou `sqlcmd`)
- Git

## Mise en route

1. Cloner le dépôt :
   ```bash
   git clone https://github.com/Guillaume-Kennes/SGBD-projet.git
   ```
2. Ouvrir `PadelManager.sln` dans Visual Studio.
3. Restaurer les packages NuGet (automatique à l'ouverture, ou `dotnet restore`).
4. **Mettre en place la base de données** : exécuter, dans cet ordre exact, les scripts du dossier `database/` (non versionnés — voir `.gitignore`, remis séparément) contre l'instance SQL Server cible :
   1. `01_create_database.sql` — crée la base et l'ensemble des tables du schéma.
   2. `02_donnees_type_membre.sql` — données de référence (TYPE_MEMBRE).
   3. `03_comptes_sql_differents.sql` — crée les deux comptes SQL applicatifs à droits limités (ENF-004) nécessaires au fonctionnement de l'application : `padel_api` (back-end/API) et `padel_job` (job quotidien).
   4. `04_insert.sql` — données de démonstration (sites, terrains, membres, matchs...).
5. Configurer les chaînes de connexion dans `PadelManager.Api/appsettings.Development.json` (fichier non versionné — voir `.gitignore`) : les clés `PadelDb` (compte `padel_api`) et `PadelDbJob` (compte `padel_job`), créés à l'étape précédente.
6. Lancer l'API — profil `https`, nécessaire car les deux clients WinForms pointent sur `https://localhost:7033` :
   ```bash
   dotnet run --project PadelManager.Api --launch-profile https
   ```
7. Lancer les **deux** applications WinForms (Membre et Administration) — depuis Visual Studio : clic droit sur la solution → *Configurer les projets de démarrage...* → *Plusieurs projets de démarrage* → mettre `PadelManager.Api`, `PadelManager.WinForms.Membre` et `PadelManager.WinForms.Admin` sur *Démarrer*, puis F5 : les trois se lancent ensemble.

## Tests

```bash
dotnet test PadelManager.Tests
```

Quelques tests de `PadelManager.Repositories.MatchRepository` (verrouillage/concurrence, contraintes réelles) nécessitent une vraie instance SQL Server, via deux variables d'environnement (`PADEL_TEST_SQLSERVER_CONNECTION`, `PADEL_TEST_SQLSERVER_CLEANUP_CONNECTION`) — sans elles, ces tests échouent explicitement plutôt que d'être ignorés silencieusement (voir le commentaire en tête de `MatchRepositorySqlServerTests.cs`). Ils sont pour cette raison exclus de la CI GitHub Actions.

## Workflow Git

- Aucune modification directe sur `main` (branche protégée).
- Une branche dédiée par issue (`issue-<numéro>-<description>`).
- Pull request avant tout merge vers `main`.
- La pipeline GitHub Actions exécute les tests et calcule la couverture de code à chaque contribution.

## Documentation

Le cahier des charges complet (exigences fonctionnelles et non fonctionnelles, schéma relationnel, analyse technique) est remis séparément (Moodle), avec les scripts SQL du dossier `database/` — tous deux volontairement non versionnés sur ce dépôt.
