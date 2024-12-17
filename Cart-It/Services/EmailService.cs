using System.Net.Mail;
using System.Net;

namespace Cart_It.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendPasswordResetEmail(string email, string resetToken)
        {
            var smtpSettings = _configuration.GetSection("EmailSettings");

            string smtpServer = smtpSettings["SmtpServer"];
            int smtpPort = int.Parse(smtpSettings["SmtpPort"]);
            string smtpUsername = smtpSettings["SmtpUsername"];
            string smtpPassword = smtpSettings["SmtpPassword"];
            string senderEmail = smtpSettings["SenderEmail"];
            string senderName = smtpSettings["SenderName"];

            var fromAddress = new MailAddress(senderEmail, senderName);
            var toAddress = new MailAddress(email);

            var resetLink = $"{_configuration["App:BaseUrl"]}/reset-password?token={resetToken}";

            var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Password Reset Request",
                Body = $"Click the following link to reset your password: {resetLink}",
                IsBodyHtml = false
            };

            using (var client = new SmtpClient(smtpServer, smtpPort))
            {
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;
                client.Send(message);
            }
        }
    }

}
