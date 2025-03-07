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

       


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<UserList>()
        //        .HasMany(u => u.ShoppingLists)
        //        .WithOne(s => s.UserList)
        //        .HasForeignKey(s => s.UserId);
        //}

    }
}
