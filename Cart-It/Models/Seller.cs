using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Seller
    {
        public Seller()
        {
            Products = new HashSet<Product>();
        }
        public int SellerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SellerPhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string GSTIN { get; set; } = string.Empty;
        public string BankAccountNumber { get; set; } = string.Empty;

        public virtual ICollection<Product>? Products { get; set; }

    }
}
