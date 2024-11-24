using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;

        public ICollection<Product>? Products { get; set; }
    }
}
