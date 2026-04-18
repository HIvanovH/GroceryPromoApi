using GroceryPromoApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasMany(u => u.Favourites)
            .WithMany()
            .UsingEntity(j => j.ToTable("UserFavourites"));

        modelBuilder.Entity<UserSession>()
            .HasIndex(s => s.RefreshToken)
            .IsUnique();

        modelBuilder.Entity<Product>()
            .HasOne(p => p.CatalogueProduct)
            .WithMany()
            .HasForeignKey(p => p.CatalogueProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<UserSession> UserSessions => Set<UserSession>();

    public DbSet<Supermarket> Supermarkets => Set<Supermarket>();

    public DbSet<Brochure> Brochures => Set<Brochure>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<CatalogueProduct> CatalogueProducts => Set<CatalogueProduct>();

    public DbSet<CatalogueProductOffer> CatalogueProductOffers => Set<CatalogueProductOffer>();

    public DbSet<PreferredStore> PreferredStores => Set<PreferredStore>();

    public DbSet<Notification> Notifications => Set<Notification>();
}
