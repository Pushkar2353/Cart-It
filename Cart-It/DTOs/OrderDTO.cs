using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; } 
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Item Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int ItemQuantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required")]
        [DataType(DataType.Currency)]
        public decimal? UnitPrice { get; set; }

        [Required(ErrorMessage = "Total Amount is required")]
        [DataType(DataType.Currency)]
        public decimal? TotalAmount { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(5000, ErrorMessage = "Address Line 1 name must not exceed 5000 characters.")]
        public string? ShippingAddress { get; set; } = null;

        [Required(ErrorMessage = "Order Date is required.")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = string.Empty;
    }
}
