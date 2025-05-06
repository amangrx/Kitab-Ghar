using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class DatabaseHandler : IdentityDbContext<IdentityUser>
{
    public DbSet<Book> Books { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Claims> Claims { get; set; }
    public DbSet<Bookmark> Bookmarks { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Announcement> Announcements { get; set; }

    public DatabaseHandler(DbContextOptions<DatabaseHandler> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        #region Primary Keys
        modelBuilder.Entity<Book>().HasKey(b => b.BookId);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<Claims>().HasKey(c => c.Id);
        modelBuilder.Entity<Bookmark>().HasKey(bm => bm.Id);
        modelBuilder.Entity<Review>().HasKey(r => r.Id);
        modelBuilder.Entity<Discount>().HasKey(d => d.Id);
        modelBuilder.Entity<Announcement>().HasKey(a => a.Id);
        #endregion

        #region Relationships

        // Book - Review (1 to many)
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Reviews)
            .WithOne(r => r.Book)
            .HasForeignKey(r => r.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - Review (1 to many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reviews)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Book - Bookmark (1 to many)
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Bookmarks)
            .WithOne(bm => bm.Book)
            .HasForeignKey(bm => bm.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - Bookmark (1 to many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Bookmarks)
            .WithOne(bm => bm.User)
            .HasForeignKey(bm => bm.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Book - Discount (1 to many)
        modelBuilder.Entity<Book>()
            .HasMany(b => b.Discounts)
            .WithOne(d => d.Book)
            .HasForeignKey(d => d.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - Claim (1 to many)
        modelBuilder.Entity<User>()
            .HasMany(u => u.Claims)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        #endregion

        #region Optional: Seed Data (Example only)
        // add initial value here
        // Seed Users
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Name = "Alice Smith",
                Address = "123 Main St",
                Email = "alice@example.com",
                //Password = "hashedpassword1",
                MembershipId = "M12345",
                Role = "Reader",
                Image = ""
            },
            new User
            {
                Id = 2,
                Name = "Bob Johnson",
                Address = "456 Oak Ave",
                Email = "bob@example.com",
                //Password = "hashedpassword2",
                MembershipId = "M12346",
                Role = "Reader",
                Image = ""
            }
        );

        // Seed Books
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                BookId = 1,
                Title = "The Art of Coding",
                Author = "Ada Lovelace",
                Genre = "Technology",
                Price = 29.99m,
                Language = "English",
                Publishers = "CodePress",
                Description = "A deep dive into software engineering.",
                Availability = true,
                ISBN = "123-4567890123",
                Format = "Paperback",
                Tags = "coding,software,programming",
                Image = ""
            },
            new Book
            {
                BookId = 2,
                Title = "Mysteries of the Mind",
                Author = "Sigmund Freud",
                Genre = "Psychology",
                Price = 19.99m,
                Language = "English",
                Publishers = "MindHouse",
                Description = "Explore the human psyche.",
                Availability = true,
                ISBN = "456-7891234567",
                Format = "Hardcover",
                Tags = "psychology,mind,science",
                Image = ""
            }
        );

        // Seed Claims
        modelBuilder.Entity<Claims>().HasData(
            new Claims
            {
                Id = 1,
                UserId = 1,
                ClaimCode = "CLAIM2025"
            }
        );

        // Seed Bookmarks
        modelBuilder.Entity<Bookmark>().HasData(
            new Bookmark
            {
                Id = 1,
                UserId = 1,
                BookId = 1
            },
            new Bookmark
            {
                Id = 2,
                UserId = 2,
                BookId = 2
            }
        );

        // Seed Reviews
        modelBuilder.Entity<Review>().HasData(
            new Review
            {
                Id = 1,
                BookId = 1,
                UserId = 1,
                Comment = "Excellent read for beginners.",
                Rating = 5
            },
            new Review
            {
                Id = 2,
                BookId = 2,
                UserId = 2,
                Comment = "Thought-provoking and insightful.",
                Rating = 4
            }
        );

        // Seed Discounts
        modelBuilder.Entity<Discount>().HasData(
            new Discount
            {
                Id = 1,
                BookId = 1,
                DiscountPercent = 15.0f,
                OnSale = true
            }
        );

        // Seed Announcements
        modelBuilder.Entity<Announcement>().HasData(
            new Announcement
            {
                Id = 1,
                Message = "Welcome to Kitab Ghar!",
                Type = "Info"
            },
            new Announcement
            {
                Id = 2,
                Message = "New books have arrived!",
                Type = "Update"
            }
        );

        #endregion

        base.OnModelCreating(modelBuilder);
    }
}