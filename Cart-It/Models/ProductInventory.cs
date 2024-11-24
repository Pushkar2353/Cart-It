using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class ProductInventory
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int? CurrentStock { get; set; }
        public int? MinimumStock { get; set; }
        public DateTime? LastRestockDate { get; set; } = DateTime.Now;
        public DateTime? NextRestockDate { get; set; } = DateTime.Now;
        public virtual Product? Products { get; set; }

    }
}
