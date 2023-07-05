using BackendApp.Interfaces;
using BackendApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackendApp.Controllers;

[Route("api/account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("register")]
    public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
    {
        _accountService.registerUser(dto);
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult<string> Login(LoginDto dto)
    {
        string token = _accountService.GenerateJwt(dto);
        return Ok(token);
    }
}