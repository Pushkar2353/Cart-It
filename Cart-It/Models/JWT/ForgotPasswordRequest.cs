namespace Cart_It.Models.JWT
{
    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class PasswordResetToken
    {
        public string Email { get; set; } = string.Empty; // User's email associated with the reset token
        public string Token { get; set; } = string.Empty; // The reset token
        public DateTime ExpirationDate { get; set; } // Expiration time for the reset token
    }

}
