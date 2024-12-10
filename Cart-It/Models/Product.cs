using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Product
    {
        public Product()
        {
            Carts = new HashSet<Cart>();
            Reviews = new HashSet<Review>();
            ProductsInventory = new HashSet<ProductInventory>();
        }

        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ProductDescription { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public int? ProductStock { get; set; }
        public int CategoryId { get; set; }
        public int SellerId { get; set; }
        public string ProductUrl { get; set; } = null!;

        [NotMapped]
        public IFormFile? ProductImage { get; set; }
        public string ProductImagePath { get; set; } = null!;

        public virtual Category? Categories { get; set; } = null!;
        public virtual Seller? Sellers { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<ProductInventory> ProductsInventory { get; set; } = new List<ProductInventory>();
        public virtual ICollection<Cart>? Carts { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }


    }
}
