using Cart_It.Models;

namespace Cart_It.Mapping
{
    public static class CustomerStore
    {
        public static List<Customer> Customers = new List<Customer>
    {
        new Customer { Email = "customer@example.com", Password = "customer123" }
    };
    }

    public static class SellerStore
    {
        public static List<Seller> Sellers = new List<Seller>
    {
        new Seller { Email = "seller@example.com", Password = "seller123" }
    };
    }

    public static class AdminStore
    {
        public static List<Administrator> Administrators = new List<Administrator>
    {
        new Administrator { Email = "admin@example.com", Password = "admin123" }
    };
    }
}
