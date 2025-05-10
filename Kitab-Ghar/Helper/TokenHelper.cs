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
        IdentityUser user,
        List<string> roles,
        DatabaseHandler dbContext)
    {
        // Validate parameters
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (roles == null) throw new ArgumentNullException(nameof(roles));
        if (dbContext == null) throw new ArgumentNullException(nameof(dbContext));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Fetch user profile 
        var userProfile = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == user.Email);

        var claims = new List<Claim>
        {
            // Standard JWT claims
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            
            // ASP.NET Core Identity claims
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email)
        };

        // Add profile claims if available
        if (userProfile != null)
        {
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
        }

        // Add roles with both claim types
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
            claims.Add(new Claim("role", role));
            claims.Add(new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role));
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
            UserId = user.Id,
            Email = user.Email
        };
    }
}

public class AuthTokenResult
{
    public string Token { get; set; }
    public int ExpiresIn { get; set; }
    public string UserId { get; set; }
    public string Email { get; set; }
}