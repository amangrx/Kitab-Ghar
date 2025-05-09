using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DatabaseHandler _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly Utils _utils;

    public AuthController(UserManager<IdentityUser> userManager,
                          SignInManager<IdentityUser> signInManager,
                          DatabaseHandler context,
                          Utils utils)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _utils = utils;
    }

    [HttpPost("register-member")]
    public async Task<IActionResult> RegisterAsMember([FromBody] RegisterMemberModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            return BadRequest("Email is already registered.");
        }

        var identityUser = new IdentityUser
        {
            UserName = model.Email,
            Email = model.Email
        };

        var result = await _userManager.CreateAsync(identityUser, model.Password);

        if (result.Succeeded)
        {
            var membershipId = await _utils.GenerateMembershipId(_context);

            var userProfile = new User
            {
                Name = model.Name,
                Address = model.Address,
                Email = model.Email,
                MembershipId = membershipId,
                Role = "Member"
            };

            _context.Users.Add(userProfile);
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(identityUser, "Member");

            return Ok(new
            {
                Message = "Member registered successfully.",
            });
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
            var roles = await _userManager.GetRolesAsync(user);
            var result = await TokenHelper.GenerateToken(user, roles.ToList(), _context);
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

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            var name = User.FindFirst("name")?.Value;
            var role = User.FindFirst("role")?.Value;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(role))
            {
                return Unauthorized("Required user information missing in token");
            }
            return Ok(new
            {
                Name = name,
                Role = role
            });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}