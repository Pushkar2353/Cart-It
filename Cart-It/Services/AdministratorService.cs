using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface IAdministratorService
    {
        Task<AdministratorDTO> GetAdministratorByIdAsync(int id);
        Task<IEnumerable<AdministratorDTO>> GetAllAdministratorsAsync();
        Task<AdministratorDTO> CreateAdministratorAsync(AdministratorDTO administratorDto);
        Task<AdministratorDTO> UpdateAdministratorAsync(int id, AdministratorDTO administratorDto);
        Task<bool> DeleteAdministratorAsync(int id);
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<IEnumerable<SellerDTO>> GetAllSellersAsync();
        Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync();
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<IEnumerable<OrderDTO>> GetAllOrdersAsync();
        Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync();

        Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync();


    }

    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISellerRepository _sellerRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public AdministratorService(IAdministratorRepository administratorRepository, ICustomerRepository customerRepository,
        ISellerRepository sellerRepository,
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        IReviewRepository reviewRepository, IMapper mapper)
        {
            _administratorRepository = administratorRepository;
            _customerRepository = customerRepository;
            _sellerRepository = sellerRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<AdministratorDTO> GetAdministratorByIdAsync(int id)
        {
            var admin = await _administratorRepository.GetAdministratorByIdAsync(id);
            if (admin == null)
            {
                return null;
            }
            return _mapper.Map<AdministratorDTO>(admin);
        }

        public async Task<IEnumerable<AdministratorDTO>> GetAllAdministratorsAsync()
        {
            var admins = await _administratorRepository.GetAllAdministratorsAsync();
            return _mapper.Map<IEnumerable<AdministratorDTO>>(admins);
        }

        public async Task<AdministratorDTO> CreateAdministratorAsync(AdministratorDTO administratorDto)
        {
            var administrator = _mapper.Map<Administrator>(administratorDto);
            var createdAdmin = await _administratorRepository.CreateAdministratorAsync(administrator);
            return _mapper.Map<AdministratorDTO>(createdAdmin);
        }

        public async Task<AdministratorDTO> UpdateAdministratorAsync(int id, AdministratorDTO administratorDto)
        {
            var existingAdmin = await _administratorRepository.GetAdministratorByIdAsync(id);
            if (existingAdmin == null)
            {
                return null;
            }

            // Only update fields that are provided in the DTO, leave others unchanged
            existingAdmin.Email = !string.IsNullOrEmpty(administratorDto.Email) ? administratorDto.Email : existingAdmin.Email;
            existingAdmin.Password = !string.IsNullOrEmpty(administratorDto.Password) ? administratorDto.Password : existingAdmin.Password;

            var updatedAdmin = await _administratorRepository.UpdateAdministratorAsync(existingAdmin);
            return _mapper.Map<AdministratorDTO>(updatedAdmin);
        }

        public async Task<bool> DeleteAdministratorAsync(int id)
        {
            return await _administratorRepository.DeleteAdministratorAsync(id);
        }

        // Method to fetch all customers
        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
        }

        // Method to fetch all sellers
        public async Task<IEnumerable<SellerDTO>> GetAllSellersAsync()
        {
            var sellers = await _sellerRepository.GetAllSellersAsync();
            return _mapper.Map<IEnumerable<SellerDTO>>(sellers);
        }

        // Method to fetch all products
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllProductsAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        // Method to fetch all categories
        public async Task<IEnumerable<CategoryDTO>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllCategoriesAsync();
            return _mapper.Map<IEnumerable<CategoryDTO>>(categories);
        }

        // Method to fetch all orders
        public async Task<IEnumerable<OrderDTO>> GetAllOrdersAsync()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDTO>>(orders);
        }

        // Method to fetch all payments
        public async Task<IEnumerable<PaymentDTO>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllPaymentsAsync();
            return _mapper.Map<IEnumerable<PaymentDTO>>(payments);
        }

        // Method to fetch all reviews
        public async Task<IEnumerable<ReviewDTO>> GetAllReviewsAsync()
        {
            var reviews = await _reviewRepository.GetAllReviewsAsync();
            return _mapper.Map<IEnumerable<ReviewDTO>>(reviews);
        }
    }

}
