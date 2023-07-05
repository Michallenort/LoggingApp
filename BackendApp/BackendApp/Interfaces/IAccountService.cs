using BackendApp.Models;

namespace BackendApp.Interfaces;

public interface IAccountService
{
    void registerUser(RegisterUserDto dto);
    string GenerateJwt(LoginDto dto);
}