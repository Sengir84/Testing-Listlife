using ListLife.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ListLife.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        // Skapa tabell ShoppingLists i Databasen DbSet
        public DbSet<ShoppingList> ShoppingLists { get; set; }

        // Skapa tabell för Categories i Databasen DbSet
        public DbSet<Category> Categories { get; set; }


    }
}
