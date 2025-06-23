
namespace GKVK_Api.Services
{
    public class EmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _senderName;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _apiKey = configuration["SendGrid:ApiKey"]!;
            _fromEmail = configuration["SendGrid:FromEmail"]!;
            _senderName = configuration["SendGrid:SenderName"]!;
            _logger = logger;
        }

        public async Task SendEmail(string toEmail, string subject, string body, string html/*,string password,string dob, string mobileno*/)
        {
            try
            {
                var client = new SendGridClient(_apiKey);

                var from = new EmailAddress(_fromEmail, _senderName);
                var to = new EmailAddress(toEmail);

                //var plainTextContent = message;
                //var htmlContent = "";
                // TODO: Add dynamic content based on user details
                //$"Dear {username}, <br><br>Your account has been created successfully. " +
                //    $"<br>Your username: {username}" +
                //    $" <br>Password: {password}" +
                //    $" <br>Date of Birth: {dob}" +
                //    $" <br>Mobile Number: {mobileno}" +
                //    $" <br><br>Please login to your account using the provided credentials. <br><br>Thank you."
                var msg = MailHelper.CreateSingleEmail(from, to, subject, body, html);
                var response = await client.SendEmailAsync(msg);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email sent to {toEmail} successfully.", toEmail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Sending Email to {toEmail} failed", toEmail);
            }
        }
    }
}
