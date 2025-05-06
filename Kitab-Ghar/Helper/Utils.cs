using Microsoft.EntityFrameworkCore;
public class Utils
{
    public async Task<string> GenerateMembershipId(DatabaseHandler context)
    {
        string membershipId;
        bool isUnique;
        do
        {
            membershipId = $"MEM-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
            isUnique = !await context.Users.AnyAsync(u => u.MembershipId == membershipId);
        }
        while (!isUnique);

        return membershipId;
    }
}

