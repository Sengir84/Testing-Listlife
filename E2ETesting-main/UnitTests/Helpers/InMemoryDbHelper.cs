using ListLife.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace E2ETesting.UnitTests.Helpers
{
    class InMemoryDbHelper
    {
        public static ApplicationDbContext GetContext(string dbName = "TestDb")
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureDeleted(); 
            context.Database.EnsureCreated(); 
            return context;
        }
    }
}
