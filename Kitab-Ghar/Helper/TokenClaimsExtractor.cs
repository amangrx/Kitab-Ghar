using System.Security.Claims;

public static class TokenClaimsExtractor
{
    public static string GetUserId(ClaimsPrincipal user)
    {
        return user.FindFirst("sub")?.Value;
    }

    public static string GetEmail(ClaimsPrincipal user)
    {
        return user.FindFirst("email")?.Value;
    }

    public static string GetName(ClaimsPrincipal user)
    {
        return user.FindFirst("name")?.Value;
    }

    public static string GetGivenName(ClaimsPrincipal user)
    {
        return user.FindFirst("given_name")?.Value;
    }

    public static string GetAddress(ClaimsPrincipal user)
    {
        return user.FindFirst("address")?.Value;
    }

    public static string GetMembershipId(ClaimsPrincipal user)
    {
        return user.FindFirst("membership_id")?.Value;
    }

    public static string GetRole(ClaimsPrincipal user)
    {
        return user.FindFirst("role")?.Value;
    }

    public static List<string> GetRoles(ClaimsPrincipal user)
    {
        return user.FindAll("role").Select(c => c.Value).ToList();
    }

    public static UserProfileDto GetUserProfile(ClaimsPrincipal user)
    {
        return new UserProfileDto
        {
            UserId = GetUserId(user),
            Email = GetEmail(user),
            Name = GetName(user),
            Address = GetAddress(user),
            MembershipId = GetMembershipId(user),
            Roles = GetRoles(user)
        };
    }
}

// DTO to return all user profile data
public class UserProfileDto
{
    public string UserId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string MembershipId { get; set; }
    public List<string> Roles { get; set; }
}