using ListLife.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace ListLife.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserList>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // Skapa tabell ShoppingLists i Databasen DbSet
        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<SharedList> SharedLists { get; set; }

        public DbSet<Product> Products { get; set; }

        // Decimal för Amount
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Konfigurera 'Amount' som decimal med precision 18 och skala 2
            modelBuilder.Entity<Product>()
                .Property(p => p.Amount)
                .HasColumnType("decimal(18,2)"); // precision = 18, skala = 2
        }
    }
}
