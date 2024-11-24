using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int CartQuantity { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public virtual Product? Products { get; set; } = null!;
        public virtual Customer? Customers { get; set; } = null!; 
    }
}
