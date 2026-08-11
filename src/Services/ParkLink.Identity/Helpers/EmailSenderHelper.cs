namespace ParkLink.Identity.Helpers
{
    public class EmailSenderHelper
    {
        private readonly IConfiguration _configuration;

        public EmailSenderHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public EmailSettings GetEmailSettings()
        {
            return new EmailSettings
            {
                From = _configuration["EmailConfiguration:From"]!,
                Host = _configuration["EmailConfiguration:Host"]!,
                Port = Convert.ToInt32(_configuration["EmailConfiguration:Port"]),
                Username = _configuration["EmailConfiguration:Username"]!,
                Password = _configuration["EmailConfiguration:Password"]!,
                UseSSL = Convert.ToBoolean(_configuration["EmailConfiguration:UseSSL"]),
                UseStartTls = Convert.ToBoolean(_configuration["EmailConfiguration:UseStartTls"])
            };
        }
    }

    public class EmailSettings
    {
        public string From { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool UseSSL { get; set; }
        public bool UseStartTls { get; set; }
        public string ConfirmationLink { get; set; } = string.Empty;
    }
}
