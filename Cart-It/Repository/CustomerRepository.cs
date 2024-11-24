using Cart_It.Data;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<Customer?> GetCustomerByEmailAsync(string email);
        Task AddCustomerAsync(Customer customer);
        Task UpdateCustomerPartialAsync(int customerId, Customer updatedCustomer);
        Task DeleteCustomerAsync(int customerId);
        Task<bool> CustomerExistsAsync(int customerId);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task AddCustomerAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerPartialAsync(int customerId, Customer updatedCustomer)
        {
            var existingCustomer = await _context.Customers.FindAsync(customerId);
            if (existingCustomer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {customerId} not found.");
            }

            // Update only the provided fields
            if (!string.IsNullOrEmpty(updatedCustomer.FirstName))
                existingCustomer.FirstName = updatedCustomer.FirstName;

            if (!string.IsNullOrEmpty(updatedCustomer.LastName))
                existingCustomer.LastName = updatedCustomer.LastName;

            if (!string.IsNullOrEmpty(updatedCustomer.Email))
                existingCustomer.Email = updatedCustomer.Email;

            if (!string.IsNullOrEmpty(updatedCustomer.PhoneNumber))
                existingCustomer.PhoneNumber = updatedCustomer.PhoneNumber;

            if (!string.IsNullOrEmpty(updatedCustomer.Gender))
                existingCustomer.Gender = updatedCustomer.Gender;

            if (updatedCustomer.DateOfBirth.HasValue)
                existingCustomer.DateOfBirth = updatedCustomer.DateOfBirth;

            if (!string.IsNullOrEmpty(updatedCustomer.AddressLine1))
                existingCustomer.AddressLine1 = updatedCustomer.AddressLine1;

            if (!string.IsNullOrEmpty(updatedCustomer.AddressLine2))
                existingCustomer.AddressLine2 = updatedCustomer.AddressLine2;

            if (!string.IsNullOrEmpty(updatedCustomer.City))
                existingCustomer.City = updatedCustomer.City;

            if (!string.IsNullOrEmpty(updatedCustomer.State))
                existingCustomer.State = updatedCustomer.State;

            if (!string.IsNullOrEmpty(updatedCustomer.Country))
                existingCustomer.Country = updatedCustomer.Country;

            if (!string.IsNullOrEmpty(updatedCustomer.PinCode))
                existingCustomer.PinCode = updatedCustomer.PinCode;

            // Save changes
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await GetCustomerByIdAsync(customerId);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> CustomerExistsAsync(int customerId)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerId == customerId);
        }
    }
}
