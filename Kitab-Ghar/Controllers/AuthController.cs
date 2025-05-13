using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly DatabaseHandler _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly Utils _utils;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        DatabaseHandler context,
        Utils utils,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
        _utils = utils;
        _logger = logger;
    }

    [HttpPost("register-member")]
    public async Task<IActionResult> RegisterAsMember([FromBody] RegisterMemberModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            // Check for existing user
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Conflict("Email is already registered.");
            }

            // Create Identity user
            var identityUser = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(identityUser, model.Password);
            if (!createResult.Succeeded)
            {
                return BadRequest(createResult.Errors);
            }

            // Generate membership ID
            var membershipId = await _utils.GenerateMembershipId(_context);

            // Determine role
            var role = model.Email.Equals("kitab-ghar-admin@gmail.com", StringComparison.OrdinalIgnoreCase)
                ? "Admin"
                : "Member";

            // Create custom user profile
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

            // Assign role
            var roleResult = await _userManager.AddToRoleAsync(identityUser, role);
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Failed to add role to user: {Errors}", roleResult.Errors);
            }

            // Return success response without token
            return Ok(new
            {
                Message = $"{role} registered successfully. Please login to access your account.",
                UserId = userProfile.Id,
                MembershipId = membershipId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during member registration");
            return StatusCode(500, "An error occurred during registration");
        }
    }

    [HttpPost("login-token")]
    public async Task<IActionResult> LoginForToken([FromBody] CredentialDto model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Check password without locking out on failure
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Get roles and generate token
            var roles = await _userManager.GetRolesAsync(user);
            var tokenResult = await TokenHelper.GenerateToken(user, roles.ToList(), _context);

            // Get custom user details
            var customUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == user.Email);

            return Ok(new
            {
                tokenResult.Token,
                tokenResult.ExpiresIn,
                tokenResult.UserId,
                customUser?.MembershipId,
                customUser?.Name,
                Roles = roles
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login");
            return StatusCode(500, "An error occurred during login");
        }
    }

    [Authorize]
    [HttpGet("user")]
    public async Task<IActionResult> GetUser()
    {
        try
        {
            // Get user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User identification missing");
            }

            // Try to get the custom user ID if available
            var customUserIdClaim = User.FindFirst("custom_user_id");
            var effectiveUserId = customUserIdClaim?.Value ?? userId;

            // Get user details from database
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == effectiveUserId || u.Email == User.FindFirstValue(ClaimTypes.Email));

            if (user == null)
            {
                return NotFound("User profile not found");
            }

            // Get roles from claims
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            return Ok(new
            {
                UserId = user.Id,
                user.Name,
                user.Email,
                user.MembershipId,
                user.Address,
                Roles = roles,
                IsAdmin = roles.Contains("Admin")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user details");
            return StatusCode(500, "An error occurred while retrieving user information");
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        try
        {
            await _signInManager.SignOutAsync();
            return Ok("Logged out successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, "An error occurred during logout");
        }
    }
}