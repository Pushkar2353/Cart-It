using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class ProductInventoryDTO
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Current stock must be a positive value.")]
        public int? CurrentStock { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Stock must be at least 1.")]
        public int? MinimumStock { get; set; }

        [Required(ErrorMessage = "Last Restock Date is required.")]
        [DataType(DataType.Date)]
        public DateTime? LastRestockDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Next Restock Date is required.")]
        [DataType(DataType.Date)]
        public DateTime? NextRestockDate { get; set; } = DateTime.Now;
    }
}
