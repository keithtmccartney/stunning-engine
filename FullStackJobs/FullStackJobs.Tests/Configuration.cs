using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FullStackJobs.Tests
{
    public static class Configuration
    {
        public static string InMemoryDatabase = "InMemoryDbForTesting";

        public static void UseInMemoryDatabase(this DbContextOptionsBuilder ctxBuilder)
        {
            // To allow the 'InMemoryDatabase' string to pass through as a parameter the Microsoft.EntityFrameworkCore.InMemory package needed to be applied (https://stackoverflow.com/a/48062124/7857102).
            ctxBuilder.UseInMemoryDatabase(InMemoryDatabase);
        }

        public static void AddInMemoryDataAccessServices<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<TDbContext>));

            if ( descriptor != null)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<TDbContext>(options =>
            {
                options.UseInMemoryDatabase();
            });

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;

                var db = scopedServices.GetRequiredService<TDbContext>();

                db.Database.EnsureCreated();
            }
        }
    }
}
