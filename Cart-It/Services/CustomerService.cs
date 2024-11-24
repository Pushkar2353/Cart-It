using Cart_It.DTOs;
using Cart_It.Models;
using Cart_It.Repository;

namespace Cart_It.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerDTO?> GetCustomerByIdAsync(int customerId);
        Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDto);
        Task UpdateCustomerPartialAsync(int customerId, CustomerDTO updatedCustomerDto);
        Task DeleteCustomerAsync(int customerId);
    }

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<CustomerDTO>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllCustomersAsync();
            return customers.Select(c => MapToDTO(c));
        }

        public async Task<CustomerDTO?> GetCustomerByIdAsync(int customerId)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(customerId);
            return customer != null ? MapToDTO(customer) : null;
        }

        public async Task<CustomerDTO> AddCustomerAsync(CustomerDTO customerDto)
        {
            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                Password = customerDto.Password,
                PhoneNumber = customerDto.PhoneNumber,
                Gender = customerDto.Gender,
                DateOfBirth = customerDto.DateOfBirth,
                AddressLine1 = customerDto.AddressLine1,
                AddressLine2 = customerDto.AddressLine2,
                Street = customerDto.Street,
                City = customerDto.City,
                State = customerDto.State,
                Country = customerDto.Country,
                PinCode = customerDto.PinCode
            };

            await _customerRepository.AddCustomerAsync(customer);
            return new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Gender = customer.Gender,
                DateOfBirth = customer.DateOfBirth,
                AddressLine1 = customer.AddressLine1,
                AddressLine2 = customer.AddressLine2,
                Street = customer.Street,
                City = customer.City,
                State = customer.State,
                Country = customer.Country,
                PinCode = customer.PinCode
            };
        }


        public async Task UpdateCustomerPartialAsync(int customerId, CustomerDTO updatedCustomerDto)
        {
            var updatedCustomer = MapToEntity(updatedCustomerDto);

            await _customerRepository.UpdateCustomerPartialAsync(customerId, updatedCustomer);
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            await _customerRepository.DeleteCustomerAsync(customerId);
        }

        private CustomerDTO MapToDTO(Customer customer)
        {
            return new CustomerDTO
            {
                CustomerId = customer.CustomerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Gender = customer.Gender,
                DateOfBirth = customer.DateOfBirth,
                AddressLine1 = customer.AddressLine1,
                AddressLine2 = customer.AddressLine2,
                City = customer.City,
                State = customer.State,
                Country = customer.Country,
                PinCode = customer.PinCode
            };
        }

        private Customer MapToEntity(CustomerDTO customerDto)
        {
            return new Customer
            {
                CustomerId = customerDto.CustomerId,
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                Email = customerDto.Email,
                PhoneNumber = customerDto.PhoneNumber,
                Gender = customerDto.Gender,
                DateOfBirth = customerDto.DateOfBirth,
                AddressLine1 = customerDto.AddressLine1,
                AddressLine2 = customerDto.AddressLine2,
                City = customerDto.City,
                State = customerDto.State,
                Country = customerDto.Country,
                PinCode = customerDto.PinCode
            };
        }
    }

}
