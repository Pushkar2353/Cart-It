using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class ProductDTO
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Product Name is required")]
        [StringLength(100, ErrorMessage = "Product Name must not exceed 100 characters.")]
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Description is required")]
        [StringLength(1000, ErrorMessage = "Product Description must not exceed 1000 characters.")]
        [Column(TypeName = "TEXT")]
        public string ProductDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Product Price is required")]
        [DataType(DataType.Currency)]
        public decimal? ProductPrice { get; set; }

        [Required(ErrorMessage = "Stock Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Stock must be at least 1.")]
        public int? ProductStock { get; set; }
        public int? CategoryId { get; set; }
        public int? SellerId { get; set; }

        [Required(ErrorMessage = "Product Url is required")]
        [StringLength(200, ErrorMessage = "Product Url must not exceed 200 characters.")]
        public string ProductUrl { get; set; } = string.Empty;

        [NotMapped]
        public IFormFile? ProductImage { get; set; }

        [Required(ErrorMessage = "Product Image Path is required")]
        [StringLength(500, ErrorMessage = "Product Image Path must not exceed 500 characters.")]
        public string ProductImagePath { get; set; } = string.Empty;
    }
}
