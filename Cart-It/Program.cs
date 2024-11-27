
using Cart_It.Data;
using Cart_It.Mapping;
using Cart_It.Models.JWT;
using Cart_It.Repository;
using Cart_It.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using log4net;
using Serilog;
using System.Collections.Generic;

namespace Cart_It
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ConStr")));


            builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();


            builder.Services.AddScoped<ICustomerRepository, Repository.CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            builder.Services.AddAutoMapper(typeof(MappingProfile)); // Register AutoMapper
            builder.Services.AddScoped<ISellerRepository, Repository.SellerRepository>();
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


            builder.Services.AddScoped<TokenService>();

            // Load configuration from appsettings.json
            builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Log the configuration values to ensure they are loaded correctly
            Console.WriteLine("Issuer: " + builder.Configuration["Jwt:Issuer"]);
            Console.WriteLine("Audience: " + builder.Configuration["Jwt:Audience"]);
            Console.WriteLine("Key: " + builder.Configuration["Jwt:Key"]);

            // Configure JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];
        var key = builder.Configuration["Jwt:Key"];

        // Check if the configuration values are null or empty
        if (string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience) || string.IsNullOrEmpty(key))
        {
            throw new ArgumentNullException("JWT configuration values are missing in appsettings.json.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            RoleClaimType = ClaimTypes.Role // Ensure the JWT token role claim is recognized correctly
        };
    });

            builder.Services.AddAuthorization(options =>
            {
                options.AddUserRoles();
            });



            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddLog4net();

            // Set up Serilog logging to write to a file
            Log.Logger = new LoggerConfiguration()
               .WriteTo.Console()  // Optionally log to the console as well
               .WriteTo.File("D:\\LogFiles\\CartItLogs.txt", rollingInterval: RollingInterval.Day)  // Write logs to a file, with daily log file rotation
               .CreateLogger();

            builder.Host.UseSerilog(); // Use Serilog for logging


            //builder.Services.AddSwaggerGen();

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "MyAPI", Version = "v1" });
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddAuthentication(); // Add authentication services
            builder.Services.AddAuthorization();  // Add authorization services


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication(); // Authentication middleware must come first
            app.UseAuthorization();  // Authorization middleware must come after authentication
            app.MapControllers();

            app.Run();
        }
    }
}
