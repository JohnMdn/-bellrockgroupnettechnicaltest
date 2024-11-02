using DotNetInterview.API.Data;
using DotNetInterview.API.Domain;
using DotNetInterview.API.Utility;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore; // Required for DbContext and AddDbContext
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;


namespace DotNetInterview.Tests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            // Create a shared in-memory database connection
            _connection = new SqliteConnection("Data Source=DotNetInterview;Mode=Memory;Cache=Shared");
            _connection.Open();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add the DbContext using the shared connection
                services.AddDbContext<DataContext>(options =>
                {
                    options.UseSqlite(_connection);
                });

                // Optionally, seed data here if needed
                // Seed data here
                using var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DataContext>();
                db.Database.EnsureCreated();

                // Seed Items and Variations
                var item1 = new Item
                {
                    Id = DefaultGuid.Default,
                    Reference = "A123",
                    Name = "Shorts",
                    Price = 35,
                };
                var item1Variation1 = new Variation
                {
                    Id = Guid.NewGuid(),
                    ItemId = item1.Id,
                    Size = "Small",
                    Quantity = 7
                };
                var item1Variation2 = new Variation
                {
                    Id = Guid.NewGuid(),
                    ItemId = item1.Id,
                    Size = "Medium",
                    Quantity = 0
                };
                var item1Variation3 = new Variation
                {
                    Id = Guid.NewGuid(),
                    ItemId = item1.Id,
                    Size = "Large",
                    Quantity = 3
                };

                var item2 = new Item
                {
                    Id = Guid.NewGuid(),
                    Reference = "B456",
                    Name = "Tie",
                    Price = 15,
                };

                var item3 = new Item
                {
                    Id = Guid.NewGuid(),
                    Reference = "C789",
                    Name = "Shoes",
                    Price = 70,
                };
                var item3Variation1 = new Variation
                {
                    Id = Guid.NewGuid(),
                    ItemId = item3.Id,
                    Size = "9",
                    Quantity = 7
                };
                var item3Variation2 = new Variation
                {
                    Id = Guid.NewGuid(),
                    ItemId = item3.Id,
                    Size = "10",
                    Quantity = 8
                };

                db.Items.AddRange(item1, item2, item3);
                db.Variations.AddRange(item1Variation1, item1Variation2, item1Variation3, item3Variation1, item3Variation2);
                db.SaveChanges();
            });
        }


        // Dispose the connection when the factory is disposed
        public new void Dispose()
        {
            _connection.Dispose();
            base.Dispose();
        }
    }
}
