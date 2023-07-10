using LoggingApp.Backend.Entities;
using LoggingApp.Backend.Interfaces;
using Microsoft.AspNetCore.Authorization;
using LoggingApp.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoggingApp.Backend.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] string? filterPhrase)
    {
        var usersDto = await _accountService.GetAllUsers(filterPhrase);

        return Ok(usersDto);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto dto)
    {
        await _accountService.RegisterUser(dto);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginDto dto)
    {
        string token = await _accountService.GenerateJwt(dto);
        return Ok(token);
    }
    
    [HttpDelete("user/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete([FromRoute] int id)
    {
        await _accountService.Delete(id);
        return NoContent();
    }
}