using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models.JWT
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;   
    }
}
