using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendApp.Authentication;
using BackendApp.Entities;
using BackendApp.Exceptions;
using BackendApp.Interfaces;
using BackendApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace BackendApp.Services;

public class AccountService : IAccountService
{
    private readonly AccountDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;

    public AccountService(AccountDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
    }

    public void registerUser(RegisterUserDto dto)
    {
        var newUser = new User()
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Nationality = dto.Nationality,
            RoleId = dto.RoleId
        };

        var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);

        newUser.PasswordHash = hashedPassword;
        _dbContext.Users.Add(newUser);
        _dbContext.SaveChanges();
    }

    public string GenerateJwt(LoginDto dto)
    {
        var user = _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefault(u => u.Email == dto.Email);

        if (user is null)
        {
            throw new NotFoundException("User not found!");
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            throw new NotFoundException("User not found!");
        }

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
            new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd")),
            new Claim("Nationality", user.Nationality)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

        var token = new JwtSecurityToken(
            _authenticationSettings.JwtIssuer,
            _authenticationSettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: cred
        );

        var tokenHandler = new JwtSecurityTokenHandler();

        return tokenHandler.WriteToken(token);
    }
}
