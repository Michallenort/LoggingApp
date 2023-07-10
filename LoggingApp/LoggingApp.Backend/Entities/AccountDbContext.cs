using Microsoft.EntityFrameworkCore;

namespace LoggingApp.Backend.Entities;

public class AccountDbContext : DbContext
{
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options)
    {
        
    }
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
}