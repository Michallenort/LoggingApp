using LoggingApp.Backend.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LoggingApp.Backend;

public class Seeder
{
    private readonly AccountDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public Seeder(AccountDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task Seed()
    {
        if (await _dbContext.Database.CanConnectAsync())
        {
            if (_dbContext.Database.IsRelational())
            {
                var migrations = await _dbContext.Database.GetPendingMigrationsAsync();
                if (migrations != null && migrations.Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }
            }
            
            if (!await _dbContext.Roles.AnyAsync())
            {
                var roles = GetRoles();
                await _dbContext.Roles.AddRangeAsync(roles);
                await _dbContext.SaveChangesAsync();
            }

            if (!await _dbContext.Users.AnyAsync())
            {
                var users = GetUsers();
                await _dbContext.Users.AddRangeAsync(users);
                await _dbContext.SaveChangesAsync();
            }
        }
    }

    private IEnumerable<Role> GetRoles()
    {
        var roles = new List<Role>()
        {
            new Role()
            {
                Name = "User"
            },
            new Role()
            {
                Name = "Admin"
            }
        };

        return roles;
    }

    private IEnumerable<User> GetUsers()
    {
        var users = new List<User>()
        {
            new User()
            {
                Email = "user1@gmail.com",
                FirstName = "First",
                LastName = "User",
                DateOfBirth = new DateTime(2000, 1, 1),
                Nationality = "Poland",
                RoleId = 1
            },
            new User()
            {
                Email = "user2@gmail.com",
                FirstName = "Second",
                LastName = "User",
                DateOfBirth = new DateTime(2001, 2, 10),
                Nationality = "Poland",
                RoleId = 1
            },
            new User()
            {
                Email = "admin.user@gmail.com",
                FirstName = "Admin",
                LastName = "User",
                DateOfBirth = new DateTime(2002, 3, 13),
                Nationality = "USA",
                RoleId = 2
            }
        };

        users[0].PasswordHash = _passwordHasher.HashPassword(users[0], "password1");
        users[1].PasswordHash = _passwordHasher.HashPassword(users[1], "password2");
        users[2].PasswordHash = _passwordHasher.HashPassword(users[2], "adminpassword");

        return users;
    }
}