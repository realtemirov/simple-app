using Microsoft.EntityFrameworkCore;

namespace simple_app.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Person>? People { get; set; }

        // Ensure the OnConfiguring method is correctly setting the options
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // You can also use a connection string from a config file instead
            options.UseNpgsql("Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres");
        }

        // Optional: OnModelCreating can be added if you want to configure any entity-specific rules
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // Explicitly map 'Id' property to 'id' column in the database
            modelBuilder.Entity<Person>()
                .Property(p => p.Id)
                .HasColumnName("id");// Explicitly specify the table name if needed

            // Explicitly map 'Id' property to 'id' column in the database
            modelBuilder.Entity<Person>()
                .Property(p => p.Name)
                .HasColumnName("name");// Explicitly specify the table name if needed

            modelBuilder.Entity<Person>().ToTable("people");

        }
    }
}
