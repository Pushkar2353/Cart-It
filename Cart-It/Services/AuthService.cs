using Cart_It.Models;
using Cart_It.Models.JWT;
using System.IdentityModel.Tokens.Jwt;

namespace Cart_It.Services
{
    public class AuthService
    {
        private readonly IRepository<Customer> _customerRepo;
        private readonly IRepository<Seller> _sellerRepo;
        private readonly IRepository<Administrator> _adminRepo;
        private readonly JwtHelper _jwtHelper;

        public AuthService(
            IRepository<Customer> customerRepo,
            IRepository<Seller> sellerRepo,
            IRepository<Administrator> adminRepo,
            JwtHelper jwtHelper)
        {
            _customerRepo = customerRepo;
            _sellerRepo = sellerRepo;
            _adminRepo = adminRepo;
            _jwtHelper = jwtHelper;
        }

        public string Authenticate(string email, string password)
        {
            object? user = null;
            string role = string.Empty;

            // Check in Customer
            user = _customerRepo.GetAll().FirstOrDefault(c => c.Email == email && c.Password == password);
            if (user != null)
            {
                role = "Customer";
            }

            // Check in Seller if not found
            if (user == null)
            {
                user = _sellerRepo.GetAll().FirstOrDefault(s => s.Email == email && s.Password == password);
                role = "Seller";
            }

            // Check in Administrator if not found
            if (user == null)
            {
                user = _adminRepo.GetAll().FirstOrDefault(a => a.Email == email && a.Password == password);
                role = "Admin";
            }

            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            // Generate JWT Token
            return _jwtHelper.GenerateToken(email, role);
        }
    }
}
