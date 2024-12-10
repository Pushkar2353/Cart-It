using AutoMapper;
using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerDTO?> GetCustomerByIdAsync(int customerId);
        Task<CustomerDTO?> GetCustomerByEmailAsync(string email);
        Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDto);
        Task UpdateCustomerPartialAsync(int customerId, CustomerDTO updatedCustomerDto);
        Task DeleteCustomerAsync(int customerId);
        Task<bool> CustomerExistsAsync(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerService(ICustomerRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return _mapper.Map<IEnumerable<CustomerDTO>>(customers);
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            return customer != null ? _mapper.Map<CustomerDTO>(customer) : null;
        }

        public async Task<CustomerDTO?> GetCustomerByEmailAsync(string email)
        {
            var customer = await _customerRepository.GetCustomerByEmailAsync(email);
            return customer != null ? _mapper.Map<CustomerDTO>(customer) : null;
        }

        public async Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDto)
        {
            // Map CustomerDTO to Customer entity
            var customer = _mapper.Map<Customer>(customerDto);

            await _customerRepository.AddCustomerAsync(customer);

            // Map the newly created Customer back to CustomerDTO for the response
            return _mapper.Map<CustomerDTO>(customer);
        }

        public async Task UpdateCustomerPartialAsync(int customerId, CustomerDTO updatedCustomerDto)
        {
            var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customerId);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            // Map only the properties that need updating (partial update)
            _mapper.Map(updatedCustomerDto, existingCustomer);

            await _customerRepository.UpdateCustomerPartialAsync(customerId, existingCustomer);
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            await _customerRepository.DeleteCustomerAsync(customerId);
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _customerRepository.CustomerExistsAsync(customerId);
        }
    }

}
