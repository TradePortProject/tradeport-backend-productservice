using Microsoft.EntityFrameworkCore;
using ProductManagement.Data;

namespace ProductManagement.Tests.Helpers
{
    public static class DbContextHelper
    {
        public static AppDbContext GetInMemoryDbContext(string dbName = null)
        {
            dbName ??= Guid.NewGuid().ToString();  // Use a unique name each time
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new AppDbContext(options);
        }
    }
}



