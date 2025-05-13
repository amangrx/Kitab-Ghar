using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class TokenHelper
{
    public static string SecretKey { get; set; } = string.Empty;
    public static string Issuer { get; set; } = string.Empty;
    public static string Audience { get; set; } = string.Empty;
    public static int TokenExpirationMinutes { get; set; } = 30;

    public static async Task<AuthTokenResult> GenerateToken(
        IdentityUser identityUser,
        List<string> roles,
        DatabaseHandler dbContext)
    {
        // Validate parameters
        if (identityUser == null) throw new ArgumentNullException(nameof(identityUser));
        if (roles == null) throw new ArgumentNullException(nameof(roles));
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Fetch custom user profile
        var userProfile = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == identityUser.Email);

        if (userProfile == null)
        {
            throw new InvalidOperationException("Custom user profile not found");
        }

        var claims = new List<Claim>
        {
            // Standard JWT claims - using CUSTOM user ID
            new Claim(JwtRegisteredClaimNames.Sub, userProfile.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, identityUser.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            
            // Identity claims
            new Claim(ClaimTypes.NameIdentifier, userProfile.Id.ToString()), // Using custom ID here
            new Claim("identity_id", identityUser.Id), // Store Identity ID separately
            new Claim(ClaimTypes.Email, identityUser.Email),
            
            // Add custom user ID as a separate claim
            new Claim("custom_user_id", userProfile.Id.ToString())
        };

        // Add profile claims
        if (!string.IsNullOrEmpty(userProfile.Name))
        {
            claims.Add(new Claim(ClaimTypes.Name, userProfile.Name));
            claims.Add(new Claim(JwtRegisteredClaimNames.GivenName, userProfile.Name));
            claims.Add(new Claim("full_name", userProfile.Name));
        }

        if (!string.IsNullOrEmpty(userProfile.Address))
        {
            claims.Add(new Claim(ClaimTypes.StreetAddress, userProfile.Address));
        }

        if (!string.IsNullOrEmpty(userProfile.MembershipId))
        {
            claims.Add(new Claim("membership_id", userProfile.MembershipId));
        }

        // Add role from user entity if exists
        if (!string.IsNullOrEmpty(userProfile.Role))
        {
            claims.Add(new Claim(ClaimTypes.Role, userProfile.Role));
            claims.Add(new Claim("role", userProfile.Role));
        }

        // Add additional roles from Identity system
        foreach (var role in roles)
        {
            if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
                claims.Add(new Claim("role", role));
            }
        }

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(TokenExpirationMinutes),
            signingCredentials: creds
        );

        return new AuthTokenResult
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresIn = TokenExpirationMinutes * 60,
            UserId = userProfile.Id.ToString(), // Return custom user ID
            Email = identityUser.Email,
            IdentityUserId = identityUser.Id // Include Identity ID if needed
        };
    }
}

public class AuthTokenResult
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
    public string UserId { get; set; } // Now contains custom user ID (int as string)
    public string Email { get; set; }
    public string IdentityUserId { get; set; } // Optional: Identity system ID
}