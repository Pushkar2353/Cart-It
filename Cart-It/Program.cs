
using Cart_It.Data;
using Cart_It.Mapping;
using Cart_It.Repository;
using Cart_It.Services;

namespace Cart_It
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>();

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile)); // Register AutoMapper
            builder.Services.AddScoped<ISellerRepository, SellerRepository>();
            builder.Services.AddScoped<ISellerService, SellerService>();

            builder.Services.AddAutoMapper(typeof(AdministratorProfile));
            builder.Services.AddScoped<IAdministratorRepository, AdministratorRepository>();
            builder.Services.AddScoped<IAdministratorService, AdministratorService>();

            builder.Services.AddAutoMapper(typeof(CategoryProfile));
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            builder.Services.AddAutoMapper(typeof(ProductProfile));
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IProductService, ProductService>();

            builder.Services.AddAutoMapper(typeof(ProductInventoryProfile));
            builder.Services.AddScoped<IProductInventoryService, ProductInventoryService>();
            builder.Services.AddScoped<IProductInventoryRepository, ProductInventoryRepository>();

            builder.Services.AddAutoMapper(typeof(CartProfile));
            builder.Services.AddScoped<ICartRepository, CartRepository>();
            builder.Services.AddScoped<ICartService, CartService>();

            builder.Services.AddAutoMapper(typeof(OrderProfile));
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService, OrderService>();

            builder.Services.AddAutoMapper(typeof(PaymentProfile));
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            builder.Services.AddAutoMapper(typeof(ReviewProfile));
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewService, ReviewService>();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
