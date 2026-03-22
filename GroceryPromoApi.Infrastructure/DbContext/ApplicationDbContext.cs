using GroceryPromoApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GroceryPromoApi.Infrastructure.DbContext;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Supermarket> Supermarkets => Set<Supermarket>();
    public DbSet<Brochure> Brochures => Set<Brochure>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductPending> ProductsPending => Set<ProductPending>();
    public DbSet<FavouriteProduct> Favourites => Set<FavouriteProduct>();
    public DbSet<PreferredStore> PreferredStores => Set<PreferredStore>();
    public DbSet<Notification> Notifications => Set<Notification>();
}
