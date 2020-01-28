using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace api.Model {
    public partial class ProAdvisorContext : DbContext {
        public ProAdvisorContext() { }

        public ProAdvisorContext(DbContextOptions<ProAdvisorContext> options) : base(options) { }

        public virtual DbSet<APourServiceEntr> APourServiceEntr { get; set; }
        public virtual DbSet<APourServiceSite> APourServiceSite { get; set; }
        public virtual DbSet<Auteur> Auteur { get; set; }
        public virtual DbSet<Commentaire> Commentaire { get; set; }
        public virtual DbSet<Entreprise> Entreprise { get; set; }
        public virtual DbSet<ServicePropose> ServicePropose { get; set; }
        public virtual DbSet<ServiceWeb> ServiceWeb { get; set; }
        public virtual DbSet<Source> Source { get; set; }
        public virtual DbSet<ZoneIntervention> ZoneIntervention { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<APourServiceEntr>(entity => {
                entity.HasKey(e => e.Nom)
                    .HasName("PRIMARY");

                entity.ToTable("a_pour_service_entr");

                entity.HasIndex(e => e.Siret)
                    .HasName("FK_serv_entr_idx");

                entity.Property(e => e.Nom)
                    .HasColumnName("nom")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Siret)
                    .IsRequired()
                    .HasColumnName("siret")
                    .HasColumnType("varchar(14)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.NomNavigation)
                    .WithOne(p => p.APourServiceEntr)
                    .HasForeignKey<APourServiceEntr>(d => d.Nom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_serv_serv");

                entity.HasOne(d => d.SiretNavigation)
                    .WithMany(p => p.APourServiceEntr)
                    .HasForeignKey(d => d.Siret)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_serv_entr");
            });

            modelBuilder.Entity<APourServiceSite>(entity => {
                entity.HasKey(e => e.Nom)
                    .HasName("PRIMARY");

                entity.ToTable("a_pour_service_site");

                entity.HasIndex(e => e.Url)
                    .HasName("FK_serv_site_idx");

                entity.Property(e => e.Nom)
                    .HasColumnName("nom")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasColumnName("url")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.NomNavigation)
                    .WithOne(p => p.APourServiceSite)
                    .HasForeignKey<APourServiceSite>(d => d.Nom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Fk_site");

                entity.HasOne(d => d.UrlNavigation)
                    .WithMany(p => p.APourServiceSite)
                    .HasForeignKey(d => d.Url)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_serv_site");
            });

            modelBuilder.Entity<Auteur>(entity => {
                entity.HasKey(e => new { e.Nom, e.Url })
                    .HasName("PRIMARY");

                entity.ToTable("auteur");

                entity.HasIndex(e => e.Url)
                    .HasName("FK_auteur_idx");

                entity.Property(e => e.Nom)
                    .HasColumnName("nom")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.IsAnonyme).HasColumnName("is_anonyme");

                entity.HasOne(d => d.UrlNavigation)
                    .WithMany(p => p.Auteur)
                    .HasForeignKey(d => d.Url)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_auteur");
            });

            modelBuilder.Entity<Commentaire>(entity => {
                entity.ToTable("commentaire");

                entity.HasIndex(e => e.Siret)
                    .HasName("FK_comm_entr_idx");

                entity.HasIndex(e => e.UrlService)
                    .HasName("FK_comm_serv_idx");

                entity.HasIndex(e => new { e.Auteur, e.Source })
                    .HasName("Fk_comm_aut_idx");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Auteur)
                    .IsRequired()
                    .HasColumnName("auteur")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasColumnType("date");

                entity.Property(e => e.Note)
                    .HasColumnName("note")
                    .HasColumnType("int(2)");

                entity.Property(e => e.Siret)
                    .HasColumnName("siret")
                    .HasColumnType("varchar(14)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasColumnName("source")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.UrlService)
                    .HasColumnName("url_service")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.SiretNavigation)
                    .WithMany(p => p.Commentaire)
                    .HasForeignKey(d => d.Siret)
                    .HasConstraintName("FK_comm_entre");

                entity.HasOne(d => d.UrlServiceNavigation)
                    .WithMany(p => p.Commentaire)
                    .HasForeignKey(d => d.UrlService)
                    .HasConstraintName("FK_comm_serv");

                entity.HasOne(d => d.AuteurNavigation)
                    .WithMany(p => p.Commentaire)
                    .HasForeignKey(d => new { d.Auteur, d.Source })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("Fk_comm_aut");
            });

            modelBuilder.Entity<Entreprise>(entity => {
                entity.HasKey(e => e.Siret)
                    .HasName("PRIMARY");

                entity.ToTable("entreprise");

                entity.Property(e => e.Siret)
                    .HasColumnName("siret")
                    .HasColumnType("varchar(14)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Adresse)
                    .IsRequired()
                    .HasColumnName("adresse")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("utf8")
                    .HasCollation("utf8_unicode_ci");

                entity.Property(e => e.CodePostal)
                    .IsRequired()
                    .HasColumnName("code_postal")
                    .HasColumnType("varchar(8)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(256)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasColumnName("nom")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Representant)
                    .HasColumnName("representant")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Siren)
                    .IsRequired()
                    .HasColumnName("siren")
                    .HasColumnType("varchar(8)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Telephone)
                    .HasColumnName("telephone")
                    .HasColumnType("varchar(16)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Ville)
                    .IsRequired()
                    .HasColumnName("ville")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ServicePropose>(entity => {
                entity.HasKey(e => e.NomService)
                    .HasName("PRIMARY");

                entity.ToTable("service_propose");

                entity.Property(e => e.NomService)
                    .HasColumnName("nom_service")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<ServiceWeb>(entity => {
                entity.HasKey(e => e.UrlService)
                    .HasName("PRIMARY");

                entity.ToTable("service_web");

                entity.Property(e => e.UrlService)
                    .HasColumnName("url_service")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasColumnName("nom")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");
            });

            modelBuilder.Entity<Source>(entity => {
                entity.HasKey(e => e.Url)
                    .HasName("PRIMARY");

                entity.ToTable("source");

                entity.Property(e => e.Url)
                    .HasColumnName("url")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasColumnName("nom")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.RespecteAfnor).HasColumnName("respecte_afnor");
            });

            modelBuilder.Entity<ZoneIntervention>(entity => {
                entity.HasKey(e => new { e.NomVille, e.Siret })
                    .HasName("PRIMARY");

                entity.ToTable("zone_intervention");

                entity.HasIndex(e => e.Siret)
                    .HasName("FK_zone_interv_idx");

                entity.Property(e => e.NomVille)
                    .HasColumnName("nom_ville")
                    .HasColumnType("varchar(128)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.Property(e => e.Siret)
                    .HasColumnName("siret")
                    .HasColumnType("varchar(14)")
                    .HasCharSet("latin1")
                    .HasCollation("latin1_swedish_ci");

                entity.HasOne(d => d.SiretNavigation)
                    .WithMany(p => p.ZoneIntervention)
                    .HasForeignKey(d => d.Siret)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_zone_interv");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}