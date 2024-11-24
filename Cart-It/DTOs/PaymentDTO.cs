using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class PaymentDTO
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Amount To Pay is required")]
        [DataType(DataType.Currency)]
        public decimal? AmountToPay { get; set; }

        [Required(ErrorMessage = "Payment Method is required.")]
        public string PaymentMethod { get; set; } = string.Empty;

        [Required]
        public string PaymentStatus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Payment Date is required.")]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}
