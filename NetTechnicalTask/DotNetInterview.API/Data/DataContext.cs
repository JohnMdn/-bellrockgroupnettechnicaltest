using DotNetInterview.API.Domain;
using Microsoft.EntityFrameworkCore;

namespace DotNetInterview.API.Data
{
    public sealed class DataContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<Variation> Variations { get; set; } 

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (DbUpdateException ex) when (ex.InnerException != null && ex.InnerException.Message.Contains("table already exists"))
            {
                // Log exception if necessary, or handle accordingly
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData.Load(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }
    }
}
