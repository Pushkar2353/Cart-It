using AutoMapper;
using Cart_It.Data;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Services
{
    public interface IProductService
    {
        Task<ProductDTO> GetProductByIdAsync(int productId);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<ProductDTO> AddProductAsync(ProductDTO productDto);
        Task<ProductDTO> UpdateProductAsync(int productId, ProductDTO productDto);
        Task DeleteProductAsync(int productId);
        Task<IEnumerable<ProductDTO>> GetProductsByCategoryIdAsync(int categoryId);
    }

    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly IMapper _mapper;

        public ProductService(IProductRepository productRepository, IMapper mapper, AppDbContext context)
        {
            _productRepository = productRepository;
            _mapper = mapper;
            _context = context;
        }

        public async Task<ProductDTO> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTO> AddProductAsync(ProductDTO productDto)
        {
            // Validate SellerId
            var sellerExists = await _context.Sellers.AnyAsync(s => s.SellerId == productDto.SellerId);
            if (!sellerExists)
            {
                throw new Exception($"Seller with ID {productDto.SellerId} does not exist.");
            }

            // Validate CategoryId
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == productDto.CategoryId);
            if (!categoryExists)
            {
                throw new Exception($"Category with ID {productDto.CategoryId} does not exist.");
            }
            var product = _mapper.Map<Product>(productDto);
            var addedProduct = await _productRepository.AddProductAsync(product);
            return _mapper.Map<ProductDTO>(addedProduct);
        }

        public async Task<ProductDTO> UpdateProductAsync(int productId, ProductDTO productDto)
        {
            var existingProduct = await _productRepository.GetProductByIdAsync(productId);

            if (existingProduct == null)
                throw new Exception("Product not found");

            // Update only provided fields, keep existing values for others
            existingProduct.ProductName = productDto.ProductName ?? existingProduct.ProductName;
            existingProduct.ProductDescription = productDto.ProductDescription ?? existingProduct.ProductDescription;

            // Check if ProductPrice is provided (not default 0) before updating
            if (productDto.ProductPrice != 0)
            {
                existingProduct.ProductPrice = productDto.ProductPrice;
            }

            existingProduct.ProductStock = productDto.ProductStock ?? existingProduct.ProductStock;
            existingProduct.ProductUrl = productDto.ProductUrl ?? existingProduct.ProductUrl;
            existingProduct.CategoryId = productDto.CategoryId ?? existingProduct.CategoryId;
            existingProduct.SellerId = productDto.SellerId ?? existingProduct.SellerId;
            existingProduct.ProductImagePath = productDto.ProductImagePath ?? existingProduct.ProductImagePath;

            var updatedProduct = await _productRepository.UpdateProductAsync(existingProduct);
            return _mapper.Map<ProductDTO>(updatedProduct);
        }

        public async Task DeleteProductAsync(int productId)
        {
            await _productRepository.DeleteProductAsync(productId);
        }

        public async Task<IEnumerable<ProductDTO>> GetProductsByCategoryIdAsync(int categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryIdAsync(categoryId);
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }
    }

}
