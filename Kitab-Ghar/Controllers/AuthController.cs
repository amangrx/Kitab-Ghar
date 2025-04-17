﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    //[Authorize(Roles = "different")]
    //[HttpGet("users")]
    //public IActionResult GetAllUsers()
    //{
    //    var users = _userManager.Users.Select(u => new { u.Id, u.Email });
    //    return Ok(users);
    //}

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CredentialDto model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok("User registered successfully.");
        }

        return BadRequest(result.Errors);
    }

    //[HttpPost("register-different")]
    //public async Task<IActionResult> RegisterAsDifferent([FromBody] CredentialDto model)
    //{
    //    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    //    var result = await _userManager.CreateAsync(user, model.Password);

    //    if (result.Succeeded)
    //    {
    //        await _userManager.AddToRoleAsync(user, "different");
    //        return Ok("User registered successfully.");
    //    }

    //    return BadRequest(result.Errors);
    //}

    [HttpPost("register-admin")]
    public async Task<IActionResult> RegisterAsAdmin([FromBody] CredentialDto model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok("User registered successfully.");
        }

        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] CredentialDto model)
    {
        var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

        if (result.Succeeded)
        {
            return Ok("Logged in successfully.");
        }

        return Unauthorized("Invalid email or password.");
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok("Logged out successfully.");
    }
}

