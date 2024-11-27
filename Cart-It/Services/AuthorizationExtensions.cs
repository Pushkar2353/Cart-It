using Microsoft.AspNetCore.Authorization;

namespace Cart_It.Services
{
    public static class AuthorizationExtensions
    {
        public static void AddUserRoles(this AuthorizationOptions options)
        {
            options.AddPolicy("CustomerPolicy", policy =>
                policy.RequireRole("Customer"));

            options.AddPolicy("SellerPolicy", policy =>
                policy.RequireRole("Seller"));

            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy("MultiRolePolicy", policy =>
                policy.RequireRole("Customer", "Seller", "Administrator"));
        }
    }
}
