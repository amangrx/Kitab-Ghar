using Microsoft.AspNetCore.Identity;
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

    //[HttpPost("register")]
    //public async Task<IActionResult> Register([FromBody] CredentialDto model)
    //{
    //    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
    //    var result = await _userManager.CreateAsync(user, model.Password);

    //    if (result.Succeeded)
    //    {
    //        return Ok("User registered successfully.");
    //    }

    //    return BadRequest(result.Errors);
    //}

    [HttpPost("register-member")]
    public async Task<IActionResult> RegisterAsMember([FromBody] CredentialDto model)
    {
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Member");
            return Ok("Member registered successfully.");
        }

        return BadRequest(result.Errors);
    }

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

    [HttpPost("login-token")]
    public async Task<IActionResult> LoginForToken([FromBody] CredentialDto model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roles = _userManager.GetRolesAsync(user);
            var result = TokenHelper.GenerateToken(user, roles.Result.ToList());

            return Ok(result);
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

