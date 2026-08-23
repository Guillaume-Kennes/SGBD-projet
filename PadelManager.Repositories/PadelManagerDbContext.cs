using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PadelManager.Models;

namespace PadelManager.Repositories;

public partial class PadelManagerDbContext : DbContext
{
    public PadelManagerDbContext(DbContextOptions<PadelManagerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Administrateur> Administrateurs { get; set; }

    public virtual DbSet<Dette> Dettes { get; set; }

    public virtual DbSet<Disponibilite> Disponibilites { get; set; }

    public virtual DbSet<FermetureHebdoGlobale> FermetureHebdoGlobales { get; set; }

    public virtual DbSet<HoraireSite> HoraireSites { get; set; }

    public virtual DbSet<JourFermeture> JourFermetures { get; set; }

    public virtual DbSet<Match> Matches { get; set; }

    public virtual DbSet<Membre> Membres { get; set; }

    public virtual DbSet<Paiement> Paiements { get; set; }

    public virtual DbSet<Participation> Participations { get; set; }

    public virtual DbSet<Penalite> Penalites { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<Terrain> Terrains { get; set; }

    public virtual DbSet<TypeMembre> TypeMembres { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrateur>(entity =>
        {
            entity.HasKey(e => e.Matricule);

            entity.ToTable("ADMINISTRATEUR");

            entity.Property(e => e.Matricule)
                .HasMaxLength(10)
                .HasColumnName("matricule");
            entity.Property(e => e.SiteId).HasColumnName("siteId");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .HasColumnName("type");

            entity.HasOne(d => d.Site).WithMany(p => p.Administrateurs)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK_ADMINISTRATEUR_SITE");
        });

        modelBuilder.Entity<Dette>(entity =>
        {
            entity.ToTable("DETTE");

            entity.HasIndex(e => new { e.MembreMatricule, e.Soldee }, "IX_DETTE_membre_soldee");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateCreation)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dateCreation");
            entity.Property(e => e.DateReglement)
                .HasPrecision(0)
                .HasColumnName("dateReglement");
            entity.Property(e => e.MatchOrigineId).HasColumnName("matchOrigineId");
            entity.Property(e => e.MatchReglementId).HasColumnName("matchReglementId");
            entity.Property(e => e.MembreMatricule)
                .HasMaxLength(10)
                .HasColumnName("membreMatricule");
            entity.Property(e => e.Montant)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("montant");
            entity.Property(e => e.Soldee).HasColumnName("soldee");

            entity.HasOne(d => d.MatchOrigine).WithMany(p => p.DetteMatchOrigines)
                .HasForeignKey(d => d.MatchOrigineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DETTE_MATCH_ORIGINE");

            entity.HasOne(d => d.MatchReglement).WithMany(p => p.DetteMatchReglements)
                .HasForeignKey(d => d.MatchReglementId)
                .HasConstraintName("FK_DETTE_MATCH_REGLEMENT");

            entity.HasOne(d => d.MembreMatriculeNavigation).WithMany(p => p.Dettes)
                .HasForeignKey(d => d.MembreMatricule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DETTE_MEMBRE");
        });

        modelBuilder.Entity<Disponibilite>(entity =>
        {
            entity.ToTable("DISPONIBILITE");

            entity.HasIndex(e => new { e.SiteId, e.Date }, "IX_DISPONIBILITE_site_date");

            entity.HasIndex(e => new { e.SiteId, e.Date, e.HeureDebut }, "UQ_DISPONIBILITE_site_date_heure").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.HeureDebut)
                .HasPrecision(0)
                .HasColumnName("heureDebut");
            entity.Property(e => e.HeureFin)
                .HasPrecision(0)
                .HasColumnName("heureFin");
            entity.Property(e => e.SiteId).HasColumnName("siteId");

            entity.HasOne(d => d.Site).WithMany(p => p.Disponibilites)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DISPONIBILITE_SITE");
        });

        modelBuilder.Entity<FermetureHebdoGlobale>(entity =>
        {
            entity.HasKey(e => e.Annee);

            entity.ToTable("FERMETURE_HEBDO_GLOBALE");

            entity.Property(e => e.Annee)
                .ValueGeneratedNever()
                .HasColumnName("annee");
            entity.Property(e => e.JoursFermes)
                .HasMaxLength(50)
                .HasColumnName("joursFermes");
        });

        modelBuilder.Entity<HoraireSite>(entity =>
        {
            entity.ToTable("HORAIRE_SITE");

            entity.HasIndex(e => new { e.SiteId, e.Annee }, "UQ_HORAIRE_SITE_site_annee").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Annee).HasColumnName("annee");
            entity.Property(e => e.HeureDebutReservation)
                .HasPrecision(0)
                .HasColumnName("heureDebutReservation");
            entity.Property(e => e.HeureFinReservation)
                .HasPrecision(0)
                .HasColumnName("heureFinReservation");
            entity.Property(e => e.JoursOuverture)
                .HasMaxLength(50)
                .HasColumnName("joursOuverture");
            entity.Property(e => e.SiteId).HasColumnName("siteId");

            entity.HasOne(d => d.Site).WithMany(p => p.HoraireSites)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HORAIRE_SITE_SITE");
        });

        modelBuilder.Entity<JourFermeture>(entity =>
        {
            entity.ToTable("JOUR_FERMETURE");

            entity.HasIndex(e => new { e.SiteId, e.Date }, "UQ_JOUR_FERMETURE_site_date").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Date).HasColumnName("date");
            entity.Property(e => e.SiteId).HasColumnName("siteId");

            entity.HasOne(d => d.Site).WithMany(p => p.JourFermetures)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK_JOUR_FERMETURE_SITE");
        });

        modelBuilder.Entity<Match>(entity =>
        {
            entity.ToTable("MATCH");

            entity.HasIndex(e => new { e.SiteId, e.DateHeure }, "IX_MATCH_site_dateHeure");

            entity.HasIndex(e => new { e.Visibilite, e.DateHeure }, "IX_MATCH_visibilite_dateHeure");

            entity.HasIndex(e => new { e.TerrainId, e.DateHeure }, "UQ_MATCH_terrain_creneau").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateHeure)
                .HasPrecision(0)
                .HasColumnName("dateHeure");
            entity.Property(e => e.OrganisateurMatricule)
                .HasMaxLength(10)
                .HasColumnName("organisateurMatricule");
            entity.Property(e => e.SiteId).HasColumnName("siteId");
            entity.Property(e => e.Statut)
                .HasMaxLength(10)
                .HasDefaultValue("INCOMPLET")
                .HasColumnName("statut");
            entity.Property(e => e.TerrainId).HasColumnName("terrainId");
            entity.Property(e => e.Visibilite)
                .HasMaxLength(10)
                .HasColumnName("visibilite");

            entity.HasOne(d => d.OrganisateurMatriculeNavigation).WithMany(p => p.Matches)
                .HasForeignKey(d => d.OrganisateurMatricule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MATCH_ORGANISATEUR");

            entity.HasOne(d => d.Site).WithMany(p => p.Matches)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MATCH_SITE");

            entity.HasOne(d => d.Terrain).WithMany(p => p.Matches)
                .HasForeignKey(d => d.TerrainId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MATCH_TERRAIN");
        });

        modelBuilder.Entity<Membre>(entity =>
        {
            entity.HasKey(e => e.Matricule);

            entity.ToTable("MEMBRE");

            entity.Property(e => e.Matricule)
                .HasMaxLength(10)
                .HasColumnName("matricule");
            entity.Property(e => e.SiteId).HasColumnName("siteId");
            entity.Property(e => e.TypeMembre)
                .HasMaxLength(10)
                .HasColumnName("typeMembre");

            entity.HasOne(d => d.Site).WithMany(p => p.Membres)
                .HasForeignKey(d => d.SiteId)
                .HasConstraintName("FK_MEMBRE_SITE");

            entity.HasOne(d => d.TypeMembreNavigation).WithMany(p => p.Membres)
                .HasForeignKey(d => d.TypeMembre)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MEMBRE_TYPE_MEMBRE");
        });

        modelBuilder.Entity<Paiement>(entity =>
        {
            entity.ToTable("PAIEMENT");

            entity.HasIndex(e => e.ParticipationId, "UQ_PAIEMENT_participationId").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DatePaiement)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("datePaiement");
            entity.Property(e => e.MontantDetteReportee)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("montantDetteReportee");
            entity.Property(e => e.MontantParticipation)
                .HasDefaultValue(1500m)
                .HasColumnType("decimal(6, 2)")
                .HasColumnName("montantParticipation");
            entity.Property(e => e.MontantTotal)
                .HasComputedColumnSql("([montantParticipation]+[montantDetteReportee])", true)
                .HasColumnType("decimal(7, 2)")
                .HasColumnName("montantTotal");
            entity.Property(e => e.ParticipationId).HasColumnName("participationId");

            entity.HasOne(d => d.Participation).WithOne(p => p.Paiement)
                .HasForeignKey<Paiement>(d => d.ParticipationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PAIEMENT_PARTICIPATION");
        });

        modelBuilder.Entity<Participation>(entity =>
        {
            entity.ToTable("PARTICIPATION");

            entity.HasIndex(e => new { e.MatchId, e.MembreMatricule }, "UQ_PARTICIPATION_match_membre").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateInscription)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dateInscription");
            entity.Property(e => e.MatchId).HasColumnName("matchId");
            entity.Property(e => e.MembreMatricule)
                .HasMaxLength(10)
                .HasColumnName("membreMatricule");

            entity.HasOne(d => d.Match).WithMany(p => p.Participations)
                .HasForeignKey(d => d.MatchId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PARTICIPATION_MATCH");

            entity.HasOne(d => d.MembreMatriculeNavigation).WithMany(p => p.Participations)
                .HasForeignKey(d => d.MembreMatricule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PARTICIPATION_MEMBRE");
        });

        modelBuilder.Entity<Penalite>(entity =>
        {
            entity.ToTable("PENALITE");

            entity.HasIndex(e => new { e.MembreMatricule, e.DelaiJusquAu }, "IX_PENALITE_membre_delai");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DateApplication)
                .HasPrecision(0)
                .HasDefaultValueSql("(sysdatetime())")
                .HasColumnName("dateApplication");
            entity.Property(e => e.DelaiJusquAu).HasColumnName("delaiJusquAu");
            entity.Property(e => e.MatchOrigineId).HasColumnName("matchOrigineId");
            entity.Property(e => e.MembreMatricule)
                .HasMaxLength(10)
                .HasColumnName("membreMatricule");

            entity.HasOne(d => d.MatchOrigine).WithMany(p => p.Penalites)
                .HasForeignKey(d => d.MatchOrigineId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PENALITE_MATCH");

            entity.HasOne(d => d.MembreMatriculeNavigation).WithMany(p => p.Penalites)
                .HasForeignKey(d => d.MembreMatricule)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PENALITE_MEMBRE");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity.ToTable("SITE");

            entity.HasIndex(e => e.Nom, "UQ_SITE_nom").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .HasColumnName("nom");
        });

        modelBuilder.Entity<Terrain>(entity =>
        {
            entity.ToTable("TERRAIN");

            entity.HasIndex(e => new { e.SiteId, e.Numero }, "UQ_TERRAIN_site_numero").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Numero).HasColumnName("numero");
            entity.Property(e => e.SiteId).HasColumnName("siteId");

            entity.HasOne(d => d.Site).WithMany(p => p.Terrains)
                .HasForeignKey(d => d.SiteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TERRAIN_SITE");
        });

        modelBuilder.Entity<TypeMembre>(entity =>
        {
            entity.HasKey(e => e.Code);

            entity.ToTable("TYPE_MEMBRE");

            entity.Property(e => e.Code)
                .HasMaxLength(10)
                .HasColumnName("code");
            entity.Property(e => e.AnticipationMaxJours).HasColumnName("anticipationMaxJours");
            entity.Property(e => e.Libelle)
                .HasMaxLength(50)
                .HasColumnName("libelle");
            entity.Property(e => e.PrefixeMatricule)
                .HasMaxLength(5)
                .HasColumnName("prefixeMatricule");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
