using Cart_It.Models;
using Cart_It.Models.JWT;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Cart_It.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<Seller> Sellers { get; set; } = null!;
        public virtual DbSet<Administrator> Administrator { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductInventory> ProductsInventory { get; set; } = null!;
        public virtual DbSet<Cart> Carts { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            // Configure the Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                // Primary Key
                entity.HasKey(c => c.CustomerId);

                // Properties
                entity.Property(c => c.CustomerId)
                      .ValueGeneratedOnAdd(); // Auto-increment for the primary key

                // Properties
                entity.Property(c => c.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(c => c.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(c => c.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(c => c.Password)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(c => c.PhoneNumber)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(c => c.Gender)
                      .HasMaxLength(15);

                entity.Property(c => c.DateOfBirth);

                entity.Property(c => c.AddressLine1)
                      .HasMaxLength(100);

                entity.Property(c => c.AddressLine2)
                      .HasMaxLength(100);

                entity.Property(c => c.Street)
                      .HasMaxLength(50);

                entity.Property(c => c.City)
                      .HasMaxLength(50);

                entity.Property(c => c.State)
                      .HasMaxLength(50);

                entity.Property(c => c.Country)
                      .HasMaxLength(50);

                entity.Property(c => c.PinCode)
                      .HasMaxLength(10);

                // Relationships
                entity.HasOne(c => c.Carts) // Customer has one Cart
          .WithOne(cart => cart.Customers) // Cart has one Customer
          .HasForeignKey<Cart>(cart => cart.CustomerId) // Foreign key in Cart
          .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Orders)
                      .WithOne(order => order.Customers)
                      .HasForeignKey(order => order.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Payments)
                      .WithOne(payment => payment.Customers)
                      .HasForeignKey(payment => payment.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Reviews)
                      .WithOne(review => review.Customers)
                      .HasForeignKey(review => review.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            // Configure the Seller entity
            modelBuilder.Entity<Seller>(entity =>
            {
                // Primary Key
                entity.HasKey(s => s.SellerId);

                // Properties
                entity.Property(c => c.SellerId)
                      .ValueGeneratedOnAdd(); // Auto-increment for the primary key

                // Properties
                entity.Property(s => s.FirstName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(s => s.LastName)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(s => s.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(s => s.Password)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(s => s.SellerPhoneNumber)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(s => s.Gender)
                      .HasMaxLength(15);

                entity.Property(s => s.CompanyName)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(s => s.AddressLine1)
                      .HasMaxLength(100);

                entity.Property(s => s.AddressLine2)
                      .HasMaxLength(100);

                entity.Property(s => s.Street)
                      .HasMaxLength(50);

                entity.Property(s => s.City)
                      .HasMaxLength(50);

                entity.Property(s => s.State)
                      .HasMaxLength(50);

                entity.Property(s => s.Country)
                      .HasMaxLength(50);

                entity.Property(s => s.PinCode)
                      .HasMaxLength(10);

                entity.Property(s => s.GSTIN)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(s => s.BankAccountNumber)
                      .IsRequired()
                      .HasMaxLength(20);

                // Relationships
                entity.HasMany(s => s.Products)
                      .WithOne(product => product.Sellers)
                      .HasForeignKey(product => product.SellerId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure the Administrator entity
            modelBuilder.Entity<Administrator>(entity =>
            {
                // Primary Key
                entity.HasKey(a => a.AdminId);
                // Properties
                entity.Property(c => c.AdminId)
                      .ValueGeneratedOnAdd(); // Auto-increment for the primary key

                // Properties
                entity.Property(a => a.Email)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(a => a.Password)
                      .IsRequired()
                      .HasMaxLength(256); // Assumes passwords are hashed/stored securely
            });


            // Configure the Category entity
            modelBuilder.Entity<Category>(entity =>
            {
                // Primary Key
                entity.HasKey(c => c.CategoryId);

                // Properties
                entity.Property(c => c.CategoryId)
                      .ValueGeneratedOnAdd(); // Auto-increment for the primary key

                entity.Property(c => c.CategoryName)
                      .IsRequired()
                      .HasMaxLength(100); // Assuming a maximum length of 100 characters for category names

                // Relationships
                entity.HasMany(c => c.Products)
                      .WithOne(product => product.Categories)
                      .HasForeignKey(product => product.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent cascading deletes
            });


            // Configure the Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                // Primary Key
                entity.HasKey(p => p.ProductId);

                entity.Property(p => p.ProductId)
                      .ValueGeneratedOnAdd();

                // Configure properties
                entity.Property(p => p.ProductName)
                      .IsRequired() // Ensures the ProductName cannot be null
                      .HasMaxLength(200); // Optionally set a max length for ProductName

                entity.Property(p => p.ProductDescription)
                      .IsRequired() // Ensures ProductDescription cannot be null
                      .HasMaxLength(1000); // Optionally set a max length for ProductDescription

                entity.Property(p => p.ProductPrice)
                      .HasColumnType("decimal(18, 2)"); // Ensure proper decimal precision

                entity.Property(p => p.ProductImagePath)
                      .IsRequired(); // Ensure ProductImagePath is not null

                // Set up relationships
                entity.HasOne(p => p.Categories)
                      .WithMany(c => c.Products)  // One Category can have many Products
                      .HasForeignKey(p => p.CategoryId) // Foreign key for Category
                      .OnDelete(DeleteBehavior.Cascade); // Optional cascade delete if a category is deleted

                entity.HasOne(p => p.Sellers)
                      .WithMany(s => s.Products)  // One Seller can have many Products
                      .HasForeignKey(p => p.SellerId) // Foreign key for Seller
                      .OnDelete(DeleteBehavior.Cascade); // Optional cascade delete if a seller is deleted

                // Configure the many-to-many relationship between Product and Order
                entity.HasMany(p => p.Orders)
                      .WithMany(o => o.Products) // Many-to-many relationship with Orders
                      .UsingEntity<Dictionary<string, object>>(
                          "OrderProduct", // Temporary name for the join table
                          j => j.HasOne<Order>().WithMany().HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade),
                          j => j.HasOne<Product>().WithMany().HasForeignKey("ProductId").OnDelete(DeleteBehavior.Cascade)
                      );

                // Set up one-to-many relationship with ProductInventory
                entity.HasMany(p => p.ProductsInventory)
                      .WithOne(pi => pi.Products) // Each ProductInventory is related to a single Product
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Set up one-to-many relationship with Cart (Product can have many Cart entries)
                entity.HasMany(p => p.Carts)
                      .WithOne(c => c.Products)  // Each Cart item is related to one Product
                      .HasForeignKey(c => c.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete if a product is deleted

                // Set up one-to-many relationship with Review (Product can have many Reviews)
                entity.HasMany(p => p.Reviews)
                      .WithOne(r => r.Products)  // Each Review is related to one Product
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete if a product is deleted
            });


            // Configure the ProductInventory entity
            modelBuilder.Entity<ProductInventory>(entity =>
            {
                // Primary Key
                entity.HasKey(pi => pi.InventoryId);

                // Properties
                entity.Property(pi => pi.InventoryId)
                      .ValueGeneratedOnAdd(); // Auto-increment for primary key

                entity.Property(pi => pi.CurrentStock)
                      .IsRequired();

                entity.Property(pi => pi.MinimumStock)
                      .IsRequired();

                entity.Property(pi => pi.LastRestockDate)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()"); // Default value for LastRestockDate

                entity.Property(pi => pi.NextRestockDate)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()"); // Default value for NextRestockDate

                // Relationships
                entity.HasOne(pi => pi.Products)
                      .WithMany(p => p.ProductsInventory)
                      .HasForeignKey(pi => pi.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete on Product deletion
            });


            // Configure the Cart entity
            modelBuilder.Entity<Cart>(entity =>
            {
                // Primary Key
                entity.HasKey(c => c.CartId);

                // Properties
                entity.Property(c => c.CartId)
                      .ValueGeneratedOnAdd(); // Auto-increment for primary key

                entity.Property(c => c.CartQuantity)
                      .IsRequired(); // CartQuantity must be provided

                entity.Property(c => c.Amount)
                      .HasColumnType("decimal(18, 2)") // Ensures precision for monetary values
                      .IsRequired(false); // Nullable Amount field

                entity.Property(c => c.CreatedDate)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()"); // Default to current date/time

                entity.Property(c => c.UpdatedDate)
                      .IsRequired()
                      .HasDefaultValueSql("GETDATE()"); // Default to current date/time

                // Relationships
                entity.HasOne(c => c.Products)
                      .WithMany(p => p.Carts)
                      .HasForeignKey(c => c.ProductId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete if Product is deleted

                entity.HasOne(cart => cart.Customers) // Cart has one Customer
          .WithOne(customer => customer.Carts) // Customer has one Cart
          .HasForeignKey<Cart>(cart => cart.CustomerId) // Foreign key in Cart
          .OnDelete(DeleteBehavior.Cascade); // Cascade delete if Customer is deleted
            });


            // Configure the Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                // Primary Key
                entity.HasKey(o => o.OrderId);

                // Properties
                entity.Property(o => o.OrderId)
                      .ValueGeneratedOnAdd(); // Auto-increment for primary key

                entity.Property(o => o.ItemQuantity)
                      .IsRequired(false);

                entity.Property(o => o.UnitPrice)
                      .HasColumnType("decimal(18, 2)") // Precision for monetary values
                      .IsRequired(false);

                entity.Property(o => o.TotalAmount)
                      .HasColumnType("decimal(18, 2)")
                      .IsRequired(false);

                entity.Property(o => o.ShippingAddress)
                      .HasMaxLength(500) // Limiting string length
                      .IsRequired(false);

                entity.Property(o => o.OrderDate)
                      .HasDefaultValueSql("GETDATE()") // Default value for the current date
                      .IsRequired();

                entity.Property(o => o.OrderStatus)
                      .IsRequired()
                      .HasMaxLength(15);

                // Relationships
                entity.HasOne(o => o.Customers)
                      .WithMany(c => c.Orders)
                      .HasForeignKey(o => o.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete if the customer is deleted

                entity.HasMany(o => o.Products)
                      .WithMany(p => p.Orders) // Assuming a many-to-many relationship
                      .UsingEntity<Dictionary<string, object>>(
                          "OrderProduct", // Join table name
                          join => join
                              .HasOne<Product>()
                              .WithMany()
                              .HasForeignKey("ProductId")
                              .OnDelete(DeleteBehavior.Restrict), // Restrict deletion of Product
                          join => join
                              .HasOne<Order>()
                              .WithMany()
                              .HasForeignKey("OrderId")
                              .OnDelete(DeleteBehavior.Cascade)); // Cascade delete of join table records if Order is deleted

                entity.HasMany(o => o.Payments)
                      .WithOne(p => p.Orders) // Assuming Payment entity has navigation to Order
                      .HasForeignKey(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // Cascade delete Payments if Order is deleted
            });


            // Payment entity configuration
            modelBuilder.Entity<Payment>(entity =>
            {
                // Primary Key
                entity.HasKey(p => p.PaymentId);

                entity.Property(p => p.PaymentId)
                      .ValueGeneratedOnAdd();

                // Configure properties
                entity.Property(p => p.AmountToPay)
                      .HasColumnType("decimal(18,2)")
                      .IsRequired();

                entity.Property(p => p.PaymentDate)
                      .HasDefaultValueSql("GETDATE()");

                // Configure PaymentMethod and PaymentStatus enum mapping
                entity.Property(p => p.PaymentMethod)
                      .IsRequired()
                      .HasMaxLength(15);

                entity.Property(p => p.PaymentStatus)
                      .IsRequired()
                      .HasMaxLength(15);

                // Configure relationships
                // One-to-Many relationship with Order (one Payment can be for one Order)
                entity.HasOne(p => p.Orders)
                      .WithMany(o => o.Payments)
                      .HasForeignKey(p => p.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                // Many-to-One relationship with Customer (a Payment belongs to one Customer)
                entity.HasOne(p => p.Customers)
                      .WithMany(c => c.Payments)
                      .HasForeignKey(p => p.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict); // Restrict deletion if there are payments for a customer
            });


            // Review entity configuration
            modelBuilder.Entity<Review>(entity =>
            {
                // Primary Key
                entity.HasKey(r => r.ReviewId);

                entity.Property(r => r.ReviewId)
                      .ValueGeneratedOnAdd();


                // Configure properties
                entity.Property(r => r.Rating)
                      .IsRequired();
                entity.HasCheckConstraint("CK_Review_Rating", "[Rating] >= 1 AND [Rating] <= 5");

                entity.Property(r => r.ReviewText)
                      .HasMaxLength(500) // Limiting the length of review text
                      .IsRequired();

                entity.Property(r => r.ReviewDate)
                      .HasDefaultValueSql("GETDATE()");

                // Configure relationships
                // Many-to-One relationship with Product (many reviews can be for one product)
                entity.HasOne(r => r.Products)
                      .WithMany(p => p.Reviews)
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of product if reviews exist

                // Many-to-One relationship with Customer (many reviews can be written by one customer)
                entity.HasOne(r => r.Customers)
                      .WithMany(c => c.Reviews)
                      .HasForeignKey(r => r.CustomerId)
                      .OnDelete(DeleteBehavior.Restrict); // Prevent deletion of customer if reviews exist
            });
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            var configSection = configBuilder.GetSection("ConnectionStrings");
            var conStr = configSection["conStr"] ?? null;

            optionsBuilder.UseSqlServer(conStr);

        }

    }
}
