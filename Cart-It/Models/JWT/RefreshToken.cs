namespace Cart_It.Models.JWT
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // Link to user
        public DateTime Expiration { get; set; }

    }
}
