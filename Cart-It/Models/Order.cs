using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Order
    {
        public Order()
        {
            Payments = new HashSet<Payment>();
        }

        [Key]
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }
        public int? ItemQuantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? ShippingAddress { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string OrderStatus { get; set; } = string.Empty;

        public virtual Customer? Customers { get; set; } = null!;
        public ICollection<Product>? Products { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }

    }
}
