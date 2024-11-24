using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Cart_It.DTOs
{
    public class SellerDTO
    {
        public int SellerId { get; set; } 

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(100, ErrorMessage = "First Name must not exceed 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(100, ErrorMessage = "Last Name must not exceed 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Password is required.")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "The Password must be between 8 and 20 characters.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@$!%*?&])[A-Za-z\\d@$!%*?&]{8,}$", ErrorMessage = "The Password must have at least one uppercase letter, one lowercase letter, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string SellerPhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [MaxLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(100, ErrorMessage = "Company Name must not exceed 100 characters.")]
        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company Address is required.")]
        [StringLength(1000, ErrorMessage = "Address Line 1 name must not exceed 1000 characters.")]
        public string AddressLine1 { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Address Line 2 name must not exceed 1000 characters.")]
        public string AddressLine2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Street is required.")]
        [StringLength(100, ErrorMessage = "Street name must not exceed 100 characters.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [StringLength(100, ErrorMessage = "City name must not exceed 100 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [StringLength(100, ErrorMessage = "State name must not exceed 100 characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(100, ErrorMessage = "Country name must not exceed 100 characters.")]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pin Code is required.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Pin Code must be exactly 6 digits.")]
        [RegularExpression("^[0-9]{6}$", ErrorMessage = "Pin Code must contain only digits.")]
        public string PinCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "GSTIN is required.")]
        [StringLength(15, ErrorMessage = "The GSTIN must be 15 characters long.")]
        [RegularExpression("^[0-9]{2}[A-Z]{5}[0-9]{4}[A-Z]{1}[A-Z0-9]{1}[Z]{1}[A-Z0-9]{1}$", ErrorMessage = "Invalid GSTIN format.")]
        public string GSTIN { get; set; } = string.Empty;

        [Required(ErrorMessage = "The Bank Account Number is required.")]
        [StringLength(18, MinimumLength = 9, ErrorMessage = "The Bank Account Number must be between 9 and 18 digits.")]
        [RegularExpression("^[0-9]{9,18}$", ErrorMessage = "The Bank Account Number must contain only digits.")]
        public string BankAccountNumber { get; set; } = string.Empty;
    }
}
