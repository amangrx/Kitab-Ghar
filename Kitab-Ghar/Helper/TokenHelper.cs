using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public static class TokenHelper
{
    public static string SecretKey = string.Empty;
    public static string Issuer = string.Empty;
    public static string Audience = string.Empty;

    public static async Task<object> GenerateToken(
        IdentityUser user,
        List<string> roles,
        DatabaseHandler dbContext)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Fetch user profile 
        var userProfile = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == user.Email);

        var claims = new List<Claim>
        {
            new Claim("sub", user.Id),  
            new Claim("email", user.Email),
            new Claim("jti", Guid.NewGuid().ToString())
        };

        // Add profile claims 
        if (userProfile != null)
        {
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                claims.Add(new Claim("name", userProfile.Name)); 
                claims.Add(new Claim("given_name", userProfile.Name));  
            }

            if (!string.IsNullOrEmpty(userProfile.Address))
            {
                claims.Add(new Claim("address", userProfile.Address));  
            }

            if (!string.IsNullOrEmpty(userProfile.MembershipId))
            {
                claims.Add(new Claim("membership_id", userProfile.MembershipId));
            }
        }

        // Add roles 
        claims.AddRange(roles.Select(role => new Claim("role", role)));  

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30),
            signingCredentials: creds
        );

        return new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            userId = user.Id
        };
    }
}