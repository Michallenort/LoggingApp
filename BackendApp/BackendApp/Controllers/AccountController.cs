using BackendApp.Interfaces;
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
    public ActionResult RegisterUser()
    {
        return Ok();
    }

    [HttpPost("login")]
    public ActionResult Login()
    {
        return Ok();
    }
}