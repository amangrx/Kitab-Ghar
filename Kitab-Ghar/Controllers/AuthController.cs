using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


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

            // Determine the role based on the email
            var role = model.Email.Equals("kitab-ghar-admin@gmail.com", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "Member";

            var userProfile = new User
            {
                Name = model.Name,
                Address = model.Address,
                Email = model.Email,
                MembershipId = membershipId,
                Role = role
            };

            _context.Users.Add(userProfile);
            await _context.SaveChangesAsync();

            await _userManager.AddToRoleAsync(identityUser, role);

            return Ok(new
            {
                Message = $"{role} registered successfully.",
            });
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
    public IActionResult GetUser()
    {
        try
        {
            // Get all claims - store as list of key-value pairs instead of dictionary
            var allClaims = User.Claims
                .Select(c => new { c.Type, c.Value })
                .ToList();

            // Get user ID - checking multiple claim types
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)  // System standard
                      ?? User.FindFirstValue("sub")                     // JWT standard
                      ?? User.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"); // Legacy

            // Get name - checking multiple claim types
            var name = User.FindFirstValue(ClaimTypes.Name)            // System standard
                    ?? User.FindFirstValue("name")                     // Common alternative
                    ?? User.FindFirstValue("given_name");              // JWT standard

            // Get role - checking multiple claim types
            var role = User.FindFirstValue(ClaimTypes.Role)            // System standard
                    ?? User.FindFirstValue("role")                     // Common alternative
                    ?? User.FindFirstValue("http://schemas.microsoft.com/ws/2008/06/identity/claims/role"); // Legacy

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new
                {
                    Message = "User identification missing",
                    AvailableClaims = allClaims
                });
            }

            if (string.IsNullOrEmpty(role))
            {
                return Unauthorized(new
                {
                    Message = "Role information missing",
                    AvailableClaims = allClaims
                });
            }

            return Ok(new
            {
                Id = userId,
                Name = name?.Trim(),
                Role = role,
                Email = User.FindFirstValue(ClaimTypes.Email),
                MembershipId = User.FindFirstValue("membership_id")
            });
        }
        catch (Exception ex)
        {
            // Log the error here if you have logging configured
            return StatusCode(500, new
            {
                Message = "An error occurred while processing user information",
                Error = ex.Message
                // Note: Don't include stack trace in production
            });
        }
    }
}