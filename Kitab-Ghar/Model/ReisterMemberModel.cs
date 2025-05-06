using System.ComponentModel.DataAnnotations;

public class RegisterMemberModel
{
    [Required]
    public string Name { get; set; }

    public string Address { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    public string Password { get; set; }
}