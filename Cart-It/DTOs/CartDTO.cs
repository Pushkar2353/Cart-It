using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class CartDTO
    {
        public int CartId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Cart Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Cart must be at least 1.")]
        public int CartQuantity { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [DataType(DataType.Currency)]
        public decimal? Amount { get; set; }

        [Required(ErrorMessage = "Created Date is required.")]
        [DataType(DataType.Date)]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Updated Date is required.")]
        [DataType(DataType.Date)]
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
