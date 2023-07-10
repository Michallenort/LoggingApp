using LoggingApp.Backend.Entities;
using LoggingApp.Shared.Models;

namespace LoggingApp.Backend.Interfaces;

public interface IAccountService
{
    Task<IEnumerable<UserDto>> GetAllUsers(string? filterPhrase);
    Task RegisterUser(RegisterUserDto dto);
    Task<string> GenerateJwt(LoginDto dto);
    Task Delete(int id);
}