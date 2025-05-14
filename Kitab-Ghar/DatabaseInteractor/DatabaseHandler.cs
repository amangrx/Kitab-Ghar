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

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Bill> Bills { get; set; }

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
            modelBuilder.Entity<Cart>().HasKey(c => c.Id);
            modelBuilder.Entity<CartItem>().HasKey(ci => ci.Id);
            modelBuilder.Entity<Order>().HasKey(o => o.Id);
            modelBuilder.Entity<OrderItem>().HasKey(oi => oi.Id);
            modelBuilder.Entity<Bill>().HasKey(b => b.Id);
            #endregion

            #region Relationships
            // Book - Review
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Reviews)
                .WithOne(r => r.Book)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Review
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reviews)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book - Bookmark
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Bookmarks)
                .WithOne(bm => bm.Book)
                .HasForeignKey(bm => bm.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Bookmark
            modelBuilder.Entity<User>()
                .HasMany(u => u.Bookmarks)
                .WithOne(bm => bm.User)
                .HasForeignKey(bm => bm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book - Discount
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Discounts)
                .WithOne(d => d.Book)
                .HasForeignKey(d => d.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Claim
            modelBuilder.Entity<User>()
                .HasMany(u => u.Claims)
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Cart
            modelBuilder.Entity<User>()
                .HasMany<Cart>()
                .WithOne(c => c.User)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Cart - CartItem
            modelBuilder.Entity<Cart>()
                .HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book - CartItem
            modelBuilder.Entity<Book>()
                .HasMany<CartItem>()
                .WithOne(ci => ci.Book)
                .HasForeignKey(ci => ci.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Order
            modelBuilder.Entity<User>()
                .HasMany<Order>()
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - OrderItem
            modelBuilder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Book - OrderItem
            modelBuilder.Entity<Book>()
                .HasMany<OrderItem>()
                .WithOne(oi => oi.Book)
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            // User - Bill
            modelBuilder.Entity<User>()
                .HasMany<Bill>()
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Order - Bill
            modelBuilder.Entity<Order>()
                .HasOne<Bill>()
                .WithOne(b => b.Order)
                .HasForeignKey<Bill>(b => b.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            #endregion

            #region Seed Data 

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
                    Role = "Reader"
                },
                new User
                {
                    Id = 2,
                    Name = "Bob Johnson",
                    Address = "456 Oak Ave",
                    Email = "bob@example.com",
                    //Password = "hashedpassword2",
                    MembershipId = "M12346",
                    Role = "Reader"
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
                    Image = "",
                    DiscountedPrice = 0
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
                    Image = "",
                    DiscountedPrice = 0
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
                    Title = "mega",
                    Type = "Info"
                },
                new Announcement
                {
                    Id = 2,
                    Message = "New books have arrived!",
                    Title = "mega",
                    Type = "Update"
                }
            );

            // Seed Carts
            modelBuilder.Entity<Cart>().HasData(
                new Cart
                {
                    Id = 1,
                    UserId = 1
                },
                new Cart
                {
                    Id = 2,
                    UserId = 2
                }
            );

            // Seed CartItems
            modelBuilder.Entity<CartItem>().HasData(
                new CartItem
                {
                    Id = 1,
                    CartId = 1,
                    BookId = 1,
                    Quantity = 1
                },
                new CartItem
                {
                    Id = 2,
                    CartId = 2,
                    BookId = 2,
                    Quantity = 2
                }
            );

            modelBuilder.Entity<Order>().HasData(
                new Order
                {
                    Id = 1,
                    UserId = 1,
                    Date = new DateTime(2025, 5, 1),
                    TotalAmount = 25.49m,
                    Status = "Processing"
                }
            );

            modelBuilder.Entity<OrderItem>().HasData(
                new OrderItem
                {
                    Id = 1,
                    OrderId = 1,
                    BookId = 1,
                    Quantity = 1
                }
            );

            modelBuilder.Entity<Bill>().HasData(
                new Bill
                {
                    Id = 1,
                    UserId = 1,
                    OrderId = 1,
                    Amount = 25.49m
                }
            );


            #endregion

            base.OnModelCreating(modelBuilder);
        }
    }
