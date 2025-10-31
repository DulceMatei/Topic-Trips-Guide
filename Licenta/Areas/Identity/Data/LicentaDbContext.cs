using System; // Added for DateTime
using Licenta.Areas.Identity.Data;
using Licenta.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;

public class LicentaDbContext : IdentityDbContext<LicentaUser>
{
    public LicentaDbContext(DbContextOptions<LicentaDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tematica> Tematici { get; set; }
    public DbSet<Traseu> Trasee { get; set; }
    public DbSet<Tara> Tari { get; set; }
    public DbSet<Oras> Orase { get; set; }
    public DbSet<Locatie> Locatii { get; set; }
    public DbSet<Imagine> Imagini { get; set; }
    public DbSet<Recenzie> Recenzii { get; set; }
    public DbSet<LocatieTematica> LocatiiTematice { get; set; }
    public DbSet<Itinerar> Itinerarii { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configurarile existente pentru Identity
        builder.Entity<LicentaUser>().ToTable("Utilizator");
        builder.Entity<LicentaUser>().Property(u => u.PasswordHash).HasColumnName("Parola");
        builder.Entity<LicentaUser>().Property(u => u.Id).HasColumnName("id_utilizator");
        builder.Entity<LicentaUser>().Property(u => u.Email).HasMaxLength(50);

        builder.Entity<IdentityRole>().ToTable("Roluri");
        builder.Entity<IdentityRole>().Property(r => r.Name).HasColumnName("Denumire");
        builder.Entity<IdentityRole>().Property(r => r.NormalizedName).HasColumnName("DenumireNormalizata");
        builder.Entity<IdentityRole>().Property(r => r.Name).HasMaxLength(50);
        builder.Entity<IdentityRole>().HasIndex(r => r.Name).IsUnique();

        builder.Entity<IdentityUserRole<string>>().ToTable("UtilizatoriRoluri");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UtilizatoriClaimuri");
        builder.Entity<IdentityUserLogin<string>>().ToTable("Loginuri");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoluriClaimuri");
        builder.Entity<IdentityUserToken<string>>().ToTable("TokenuriUtilizatori");

        builder.Entity<Tematica>(entity =>
        {
            entity.HasKey(e => e.IdTematica);
            entity.HasIndex(e => e.Denumire).IsUnique();
            entity.Property(e => e.Denumire).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Tip).IsRequired().HasMaxLength(30);
        });

        builder.Entity<Traseu>(entity =>
        {
            entity.HasKey(e => e.IdTraseu);
            entity.HasIndex(e => new { e.DenumireTraseu, e.UtilizatorId }).IsUnique();
            entity.Property(e => e.DenumireTraseu).IsRequired().HasMaxLength(100);
            entity.Property(e => e.DurataEstimata).IsRequired();
            entity.Property(e => e.DataCreare).IsRequired();
            entity.Property(e => e.Descriere).HasColumnType("TEXT");

            entity.HasOne(d => d.Utilizator)
                  .WithMany(p => p.Trasee)
                  .HasForeignKey(d => d.UtilizatorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Tematica)
                  .WithMany(p => p.Trasee)
                  .HasForeignKey(d => d.TematicaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Tara>(entity =>
        {
            entity.HasKey(e => e.IdTara);
            entity.HasIndex(e => e.Denumire).IsUnique();
            entity.Property(e => e.Denumire).IsRequired().HasMaxLength(50);
        });

        builder.Entity<Oras>(entity =>
        {
            entity.HasKey(e => e.IdOras);
            entity.HasIndex(e => new { e.Denumire, e.TaraId }).IsUnique();
            entity.Property(e => e.Denumire).IsRequired().HasMaxLength(50);

            entity.HasOne(d => d.Tara)
                  .WithMany(p => p.Orase)
                  .HasForeignKey(d => d.TaraId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Locatie>(entity =>
        {
            entity.HasKey(e => e.IdLocatie);
            entity.HasIndex(e => new { e.Denumire, e.Geolocatie }).IsUnique();
            entity.Property(e => e.Denumire).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TipLocatie).IsRequired().HasMaxLength(30);
            entity.Property(e => e.TimpEstimativ).IsRequired();
            entity.Property(e => e.Geolocatie).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Strada).HasMaxLength(100);
            entity.Property(e => e.NumarStrada).HasMaxLength(100);
            entity.Property(e => e.Descriere).HasColumnType("TEXT");

            entity.HasOne(d => d.Oras)
                  .WithMany(p => p.Locatii)
                  .HasForeignKey(d => d.OrasId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Imagine>(entity =>
        {
            entity.HasKey(e => e.IdImagine);
            entity.HasIndex(e => new { e.LocatieId, e.NumeImagine }).IsUnique();
            entity.Property(e => e.NumeImagine).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cale).IsRequired().HasMaxLength(255);

            entity.HasOne(d => d.Locatie)
                  .WithMany(p => p.Imagini)
                  .HasForeignKey(d => d.LocatieId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Recenzie>(entity =>
        {
            entity.HasKey(e => e.IdRecenzie);
            entity.HasIndex(e => e.CodRecenzie).IsUnique();
            entity.HasIndex(e => new { e.UtilizatorId, e.LocatieId, e.DataRecenzie }).IsUnique();
            entity.Property(e => e.CodRecenzie).HasMaxLength(20);
            entity.Property(e => e.DataRecenzie).IsRequired();
            entity.Property(e => e.Rating).IsRequired();
            entity.Property(e => e.Comentariu).HasColumnType("TEXT");

            entity.HasOne(d => d.Utilizator)
                  .WithMany(p => p.Recenzii)
                  .HasForeignKey(d => d.UtilizatorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Locatie)
                  .WithMany(p => p.Recenzii)
                  .HasForeignKey(d => d.LocatieId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<LocatieTematica>(entity =>
        {
            entity.HasKey(e => e.IdLocatieTematica);
            entity.HasIndex(e => new { e.LocatieId, e.TematicaId }).IsUnique();

            entity.HasOne(d => d.Locatie)
                  .WithMany(p => p.TematiciLocatie)
                  .HasForeignKey(d => d.LocatieId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Tematica)
                  .WithMany(p => p.LocatiiTematice)
                  .HasForeignKey(d => d.TematicaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Itinerar>(entity =>
        {
            entity.HasKey(e => e.IdItinerar);
            entity.HasIndex(e => new { e.TraseuId, e.LocatieTematicaId, e.NrOrdine }).IsUnique();
            entity.Property(e => e.NrOrdine).IsRequired();

            entity.HasOne(d => d.LocatieTematica)
                  .WithMany(p => p.Itinerarii)
                  .HasForeignKey(d => d.LocatieTematicaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.Traseu)
                  .WithMany(p => p.Itinerarii)
                  .HasForeignKey(d => d.TraseuId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }

    public override int SaveChanges()
    {
        ValidateTraseuDataCreare();
        ValidateItinerarTematica();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ValidateTraseuDataCreare();
        ValidateItinerarTematica();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ValidateTraseuDataCreare()
    {
        var traseuEntries = ChangeTracker.Entries<Traseu>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in traseuEntries)
        {
            var traseu = entry.Entity;
            var utilizator = Users.Find(traseu.UtilizatorId);
            if (utilizator == null)
            {
                throw new InvalidOperationException($"Utilizator with ID {traseu.UtilizatorId} not found.");
            }

            if (traseu.DataCreare < utilizator.DataInregistrării)
            {
                throw new InvalidOperationException(
                    $"Traseu.DataCreare ({traseu.DataCreare}) must be greater than or equal to Utilizator.DataInregistrarii ({utilizator.DataInregistrării}) for UtilizatorId {traseu.UtilizatorId}.");
            }
        }
    }


    private void ValidateItinerarTematica()
    {
        var itinerarEntries = ChangeTracker.Entries<Itinerar>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in itinerarEntries)
        {
            var itinerar = entry.Entity;
            var locatieTematica = LocatiiTematice.Find(itinerar.LocatieTematicaId);
            var traseu = Trasee.Find(itinerar.TraseuId);
            
            if (locatieTematica == null || traseu == null)
            {
                throw new InvalidOperationException("LocatieTematica or Traseu not found.");
            }

            if (locatieTematica.TematicaId != traseu.TematicaId)
            {
                throw new InvalidOperationException(
                    $"Itinerar TematicaId ({locatieTematica.TematicaId}) must match Traseu TematicaId ({traseu.TematicaId}).");
            }
        }
    }
}