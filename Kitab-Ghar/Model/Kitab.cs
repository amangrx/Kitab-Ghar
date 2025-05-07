// Book Entity
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class Book
{
    [Key]
    public int BookId { get; set; }

    [Required]
    public string Title { get; set; }
    public string Author { get; set; }
    public string Genre { get; set; }

    [Required]
    public decimal Price { get; set; }
    public string Language { get; set; }
    public string Publishers { get; set; }
    public string Description { get; set; }

    public bool Availability { get; set; }
    public string ISBN { get; set; }

    public DateTime PublicationDate { get; set; }
    public string Format { get; set; }

    public string Tags { get; set; } 
    public string Image { get; set; }

    public List<Review> Reviews { get; set; }
    public List<Bookmark> Bookmarks { get; set; }
    public List<Discount> Discounts { get; set; }
}

// User Entity
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }
    public string Address { get; set; }


    [Required]
    [EmailAddress]
    public string Email { get; set; }

    //[Required]
    //[DataType(DataType.Password)]
    //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    //public string Password { get; set; }

    public string MembershipId { get; set; }

    public string Role { get; set; }

    public List<Claims> Claims { get; set; }
    public List<Bookmark> Bookmarks { get; set; }
    public List<Review> Reviews { get; set; }
}

// Claim Entity
public class Claims
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [Required]
    public string ClaimCode { get; set; }

    public User User { get; set; }
}

// Bookmark Entity
public class Bookmark
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }

    public User User { get; set; }
    public Book Book { get; set; }
}

// Review Entity
public class Review
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }

    public string Comment { get; set; }
    public int Rating { get; set; }

    // Changed to DateTimeOffset to handle time zone
    public DateTimeOffset ReviewDate { get; set; }

    public Book Book { get; set; }
    public User User { get; set; }
}

// Discount Entity
public class Discount
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Book")]
    public int BookId { get; set; }

    public float DiscountPercent { get; set; }
    public bool OnSale { get; set; }

    // Changed to DateTimeOffset to handle time zone
    public DateTimeOffset DiscountStart { get; set; }
    public DateTimeOffset DiscountEnd { get; set; }

    public Book Book { get; set; }
}

// Announcement Entity
public class Announcement
{
    [Key]
    public int Id { get; set; }

    public string Message { get; set; }

    public string Type { get; set; }

    // Changed to DateTimeOffset to handle time zone
    public DateTimeOffset AnnouncementTime { get; set; }
}

