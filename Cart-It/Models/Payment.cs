using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Cart_It.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public decimal? AmountToPay { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public virtual Order? Orders { get; set; } = null!;
        public virtual Customer? Customers { get; set; } = null!;
    }
}
