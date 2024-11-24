using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; } = DateTime.Now;

        public virtual Product? Products { get; set; } = null!;
        public virtual Customer? Customers { get; set; } = null!;
    }
}
