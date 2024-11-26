using Cart_It.Data;
using Microsoft.EntityFrameworkCore;

namespace Cart_It.Models.JWT
{
    public class TokenService
    {
        private readonly AppDbContext _context;
        // Constructor that initializes the TokenService with an instance of the database context.
        public TokenService(AppDbContext context)
        {
            _context = context;
        }
        // Asynchronously saves a new refresh token to the database.
        public async Task SaveRefreshToken(string username, string token)
        {
            // Create a new RefreshToken object.
            var refreshToken = new RefreshToken
            {
                Email = username,  // Set the username associated with the token.
                Token = token,  // Set the token value.
                Expiration = DateTime.UtcNow.AddDays(7)  // Set the expiration date to 7 days from the current UTC date/time.
            };
            // Add the new refresh token to the corresponding DbSet in the database context.
            _context.RefreshTokens.Add(refreshToken);
            // Asynchronously save changes to the database, which includes inserting the new refresh token.
            await _context.SaveChangesAsync();
        }
        // Asynchronously retrieves the username associated with a specific refresh token.
        public async Task<string> RetrieveUsernameByRefreshToken(string refreshToken)
        {
            // Asynchronously find a refresh token that matches the provided token and has not yet expired.
            var tokenRecord = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.Expiration > DateTime.UtcNow);
            // Return the username if the token is found and valid, otherwise null.
            return tokenRecord?.Email;
        }
        // Asynchronously revokes (deletes) a refresh token from the database.
        public async Task<bool> RevokeRefreshToken(string refreshToken)
        {
            // Asynchronously find the refresh token in the database.
            var tokenRecord = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == refreshToken);
            // If the token is found, remove it from the DbSet.
            if (tokenRecord != null)
            {
                _context.RefreshTokens.Remove(tokenRecord);
                // Save changes to the database asynchronously to reflect the token removal.
                await _context.SaveChangesAsync();
                return true;  // Return true to indicate successful revocation.
            }
            // Return false if no matching token was found, indicating no revocation was performed.
            return false;
        }
    }
}
