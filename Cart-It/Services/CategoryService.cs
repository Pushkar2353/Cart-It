using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface ICategoryService
    {
        Task<CategoryDTO> GetCategoryByIdAsync(int categoryId);
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDto);
        Task<CategoryDTO> UpdateCategoryAsync(int categoryId, CategoryDTO categoryDto);
        Task DeleteCategoryAsync(int categoryId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<CategoryDTO> GetCategoryByIdAsync(int categoryId)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(categoryId);
            return _mapper.Map<CategoryDTO>(category);
        }

        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        public async Task<CategoryDTO> AddCategoryAsync(CategoryDTO categoryDto)
        {
            var category = _mapper.Map<Category>(categoryDto);
            var addedCategory = await _categoryRepository.AddCategoryAsync(category);
            return _mapper.Map<CategoryDTO>(addedCategory);
        }

        public async Task<CategoryDTO> UpdateCategoryAsync(int categoryId, CategoryDTO categoryDto)
        {
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(categoryId);

            if (existingCategory == null)
            {
                throw new Exception("Category not found");
            }

            // Update only the provided fields and keep the existing ones
            existingCategory.CategoryName = categoryDto.CategoryName ?? existingCategory.CategoryName;
            // Add logic for other properties as needed

            var updatedCategory = await _categoryRepository.UpdateCategoryAsync(existingCategory);
            return _mapper.Map<CategoryDTO>(updatedCategory);
        }

        public async Task DeleteCategoryAsync(int categoryId)
        {
            await _categoryRepository.DeleteCategoryAsync(categoryId);
        }
    }

}
