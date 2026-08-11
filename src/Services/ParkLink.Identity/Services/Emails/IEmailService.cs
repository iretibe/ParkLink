using ParkLink.Identity.Dtos;

namespace ParkLink.Identity.Services.Emails
{
    public interface IEmailService
    {
        void SendEmailToUser(EmailDto dto);
        Task SendPasswordResetEmailAsync(string email, string firstName, string resetUrl);
    }
}
