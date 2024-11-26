namespace Cart_It.Models.JWT
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
    }

    public class CustomerRepository : IRepository<Customer>
    {
        public IEnumerable<Customer> GetAll()
        {
            // Replace with actual DB context logic
            return new List<Customer>
        {
            new Customer { CustomerId = 1, Email = "customer@example.com", Password = "customer123" }
        };
        }
    }

    public class SellerRepository : IRepository<Seller>
    {
        public IEnumerable<Seller> GetAll()
        {
            // Replace with actual DB context logic
            return new List<Seller>
        {
            new Seller { SellerId = 1, Email = "seller@example.com", Password = "seller123" }
        };
        }
    }

    public class AdminRepository : IRepository<Administrator>
    {
        public IEnumerable<Administrator> GetAll()
        {
            // Replace with actual DB context logic
            return new List<Administrator>
        {
            new Administrator { AdminId = 1, Email = "admin@example.com", Password = "admin123" }
        };
        }
    }

}
