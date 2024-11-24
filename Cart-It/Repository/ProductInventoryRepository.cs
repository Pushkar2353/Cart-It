using Cart_It.Data;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface IProductInventoryRepository
    {
        Task<ProductInventory> AddInventoryAsync(ProductInventory inventory);
        Task<ProductInventory> UpdateInventoryAsync(ProductInventory inventory);
        Task<bool> ProductExistsAsync(int productId);
        Task<ProductInventory?> GetInventoryByIdAsync(int id);
    }

    public class ProductInventoryRepository : IProductInventoryRepository
    {
        private readonly AppDbContext _context;

        public ProductInventoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProductInventory> AddInventoryAsync(ProductInventory inventory)
        {
            await _context.ProductsInventory.AddAsync(inventory);
            await _context.SaveChangesAsync();
            return inventory;
        }

        public async Task<ProductInventory> UpdateInventoryAsync(ProductInventory inventory)
        {
            var existingInventory = await _context.ProductsInventory.FindAsync(inventory.InventoryId);

            if (existingInventory == null)
                throw new Exception($"Inventory with ID {inventory.InventoryId} does not exist.");

            // Update only the provided fields
            existingInventory.CurrentStock = inventory.CurrentStock ?? existingInventory.CurrentStock;
            existingInventory.MinimumStock = inventory.MinimumStock ?? existingInventory.MinimumStock;
            existingInventory.LastRestockDate = inventory.LastRestockDate ?? existingInventory.LastRestockDate;
            existingInventory.NextRestockDate = inventory.NextRestockDate ?? existingInventory.NextRestockDate;

            _context.ProductsInventory.Update(existingInventory);
            await _context.SaveChangesAsync();
            return existingInventory;
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            return await _context.Products.AnyAsync(p => p.ProductId == productId);
        }

        public async Task<ProductInventory?> GetInventoryByIdAsync(int id)
        {
            return await _context.ProductsInventory.FirstOrDefaultAsync(p => p.InventoryId == id);
        }
    }

}
