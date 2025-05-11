public class BookDTO
{
    public int BookId { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; }
    public string ISBN { get; set; }
}

public class UserDTO
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}


public class ClaimDTO
{
    public int Id { get; set; }
    public string ClaimCode { get; set; }
}


public class BookmarkDTO
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int UserId { get; set; }
}

public class ReviewDTO
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
    public DateTimeOffset ReviewDate { get; set; }
}

public class DiscountDTO
{
    public int Id { get; set; }
    public float DiscountPercent { get; set; }
    public bool OnSale { get; set; }
}


public class AnnouncementDTO
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public string Type { get; set; }
    public DateTimeOffset AnnouncementTime { get; set; }
}


public class CartDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTimeOffset Date { get; set; }
}


public class CartItemDTO
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }
    public int CartId { get; set; }
}


public class OrderDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset Date { get; set; }
}


public class OrderItemDTO
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int Quantity { get; set; }
}


public class BillDTO
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
}


