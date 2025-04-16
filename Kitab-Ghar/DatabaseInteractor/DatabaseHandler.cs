using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseHandler : IdentityDbContext<IdentityUser>
{
    public DatabaseHandler(DbContextOptions<DatabaseHandler> options)
        : base(options)
    {
    }
}