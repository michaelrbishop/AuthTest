using AuthTest_2025_03.Models;
using AuthTest_2025_03.Services.Interfaces;
using System.Net;
using System.Net.Mail;


namespace AuthTest_2025_03.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public async Task SendEmailAsync(EmailSenderModel model)
        {
            // TODO : MRB Test this
            var senderEmail = _config.GetSection("Gmail").GetValue<string>("Sender");
            var pwd = _config.GetSection("Gmail").GetValue<string>("Password");

            var fromAddress = new MailAddress(senderEmail!, "Test Email");
            var toAddress = new MailAddress(model.ToEmails.First());

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, pwd)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = model.Subject,
                Body = model.HTMLContent,
                IsBodyHtml = true,
            };

            await smtp.SendMailAsync(message);

        }
    }
}
