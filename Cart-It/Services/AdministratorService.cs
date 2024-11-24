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
    }

    public class AdministratorService : IAdministratorService
    {
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IMapper _mapper;

        public AdministratorService(IAdministratorRepository administratorRepository, IMapper mapper)
        {
            _administratorRepository = administratorRepository;
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
    }

}
