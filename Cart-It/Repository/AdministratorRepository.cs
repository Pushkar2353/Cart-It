using Cart_It.Data;
using Cart_It.Models;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Repository
{
    public interface IAdministratorRepository
    {
        Task<Administrator> GetAdministratorByIdAsync(int id);
        Task<IEnumerable<Administrator>> GetAllAdministratorsAsync();
        Task<Administrator> CreateAdministratorAsync(Administrator administrator);
        Task<Administrator> UpdateAdministratorAsync(Administrator administrator);
        Task<bool> DeleteAdministratorAsync(int id);
    }

    public class AdministratorRepository : IAdministratorRepository
    {
        private readonly AppDbContext _context;

        public AdministratorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Administrator> GetAdministratorByIdAsync(int id)
        {
            return await _context.Administrator.FindAsync(id);
        }

        public async Task<IEnumerable<Administrator>> GetAllAdministratorsAsync()
        {
            return await _context.Administrator.ToListAsync();
        }

        public async Task<Administrator> CreateAdministratorAsync(Administrator administrator)
        {
            await _context.Administrator.AddAsync(administrator);
            await _context.SaveChangesAsync();
            return administrator;
        }

        public async Task<Administrator> UpdateAdministratorAsync(Administrator administrator)
        {
            _context.Administrator.Update(administrator);
            await _context.SaveChangesAsync();
            return administrator;
        }

        public async Task<bool> DeleteAdministratorAsync(int id)
        {
            var admin = await _context.Administrator.FindAsync(id);
            if (admin == null)
            {
                return false;
            }

            _context.Administrator.Remove(admin);
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
