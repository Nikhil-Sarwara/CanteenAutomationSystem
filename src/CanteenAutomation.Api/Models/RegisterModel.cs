using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty; // Initialize

    [Required]
    public string Username { get; set; } = string.Empty; // Initialize

    [Required]
    public string Password { get; set; } = string.Empty; // Initialize
}