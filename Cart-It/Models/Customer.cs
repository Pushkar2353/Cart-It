using System.ComponentModel.DataAnnotations;

namespace Cart_It.Models
{
    public class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
            Payments = new HashSet<Payment>();
            Reviews = new HashSet<Review>();
        }
        public int CustomerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;

        public virtual Cart? Carts { get; set; } // One-to-one navigation property
        public virtual ICollection<Order>? Orders { get; set; }
        public virtual ICollection<Payment>? Payments { get; set; }
        public virtual ICollection<Review>? Reviews { get; set; }

    }

}
