using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using LoggingApp.Backend.Authentication;
using LoggingApp.Backend.Entities;
using LoggingApp.Backend.Exceptions;
using LoggingApp.Backend.Interfaces;
using LoggingApp.Shared.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace LoggingApp.Backend.Services;

public class AccountService : IAccountService
{
    private readonly AccountDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly IMapper _mapper;
    public AccountService(AccountDbContext dbContext, IPasswordHasher<User> passwordHasher, AuthenticationSettings authenticationSettings, IMapper mapper)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
        _authenticationSettings = authenticationSettings;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetAllUsers(string? filterPhrase)
    {
        var users = await _dbContext.Users
            .Include(x => x.Role)
            .Where(x => filterPhrase == null || filterPhrase.Equals("")
                        || x.FirstName.ToLower().Contains(filterPhrase.ToLower())
                        || x.LastName.ToLower().Contains(filterPhrase.ToLower())
                        || x.Email.ToLower().Contains(filterPhrase.ToLower()))
            .ToListAsync();

        var usersDto = _mapper.Map<List<UserDto>>(users);
        
        return usersDto;
    }

    public async Task RegisterUser(RegisterUserDto dto)
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
        await _dbContext.Users.AddAsync(newUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<string> GenerateJwt(LoginDto dto)
    {
        var user = await _dbContext.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Email == dto.Email);

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

    public async Task Delete(int id)
    {
        var user = await _dbContext.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
    }
}
