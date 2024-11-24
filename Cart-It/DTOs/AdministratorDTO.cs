using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class AdministratorDTO
    {
        public int AdminId { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Password is required.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The Password must be between 8 and 20 characters.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The Password must have at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
