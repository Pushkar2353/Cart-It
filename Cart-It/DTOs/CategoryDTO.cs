using System.ComponentModel.DataAnnotations;

namespace Cart_It.DTOs
{
    public class CategoryDTO
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category Name is required")]
        [StringLength(100, ErrorMessage = "Category Name must not exceed 100 characters.")]
        public string CategoryName { get; set; } = string.Empty;
    }
}
