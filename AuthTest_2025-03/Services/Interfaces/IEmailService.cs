using AuthTest_2025_03.Models;

namespace AuthTest_2025_03.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailSenderModel model);
    }
}
