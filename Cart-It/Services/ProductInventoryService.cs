using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface IProductInventoryService
    {
        Task<ProductInventoryDTO> AddInventoryAsync(ProductInventoryDTO inventoryDto);
        Task<ProductInventoryDTO> UpdateInventoryAsync(ProductInventoryDTO inventoryDto);
        Task<ProductInventoryDTO> GetInventoryByIdAsync(int id);

    }

    public class ProductInventoryService : IProductInventoryService
    {
        private readonly IProductInventoryRepository _inventoryRepository;
        private readonly IMapper _mapper;

        public ProductInventoryService(IProductInventoryRepository inventoryRepository, IMapper mapper)
        {
            _inventoryRepository = inventoryRepository;
            _mapper = mapper;
        }

        public async Task<ProductInventoryDTO> AddInventoryAsync(ProductInventoryDTO inventoryDto)
        {
            // Validate ProductId
            var productExists = await _inventoryRepository.ProductExistsAsync(inventoryDto.ProductId);
            if (!productExists)
                throw new Exception($"Product with ID {inventoryDto.ProductId} does not exist.");

            // Map DTO to entity
            var inventory = _mapper.Map<ProductInventory>(inventoryDto);

            var addedInventory = await _inventoryRepository.AddInventoryAsync(inventory);

            // Map entity back to DTO
            return _mapper.Map<ProductInventoryDTO>(addedInventory);
        }

        public async Task<ProductInventoryDTO> UpdateInventoryAsync(ProductInventoryDTO inventoryDto)
        {
            // Map DTO to entity
            var inventory = _mapper.Map<ProductInventory>(inventoryDto);

            var updatedInventory = await _inventoryRepository.UpdateInventoryAsync(inventory);

            // Map entity back to DTO
            return _mapper.Map<ProductInventoryDTO>(updatedInventory);
        }

        public async Task<ProductInventoryDTO> GetInventoryByIdAsync(int id)
        {
            var inventory = await _inventoryRepository.GetInventoryByIdAsync(id);
            if (inventory == null)
                throw new Exception($"Inventory with ID {id} not found.");

            return _mapper.Map<ProductInventoryDTO>(inventory);
        }
    }
}
