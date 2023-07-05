using Microsoft.EntityFrameworkCore;

namespace BackendApp.Entities;

public class AccountDbContext : DbContext
{
    private string connectionString =
        "Server=(localdb)\\mssqllocaldb;Database=AccountDb;Trusted_Connection=True;";
    
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(u =>
        {
            u.Property(x => x.FirstName).IsRequired().HasMaxLength(25);
            u.Property(x => x.LastName).IsRequired().HasMaxLength(25);
            u.Property(x => x.Email).IsRequired().HasMaxLength(50);
        });

        modelBuilder.Entity<Role>()
            .Property(r => r.Name)
            .IsRequired();

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString);
    }
}