using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Administrator
    {
        public int AdminId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

    }
}
