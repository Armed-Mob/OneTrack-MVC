using Microsoft.Extensions.Options;
using OneTrack.Repostiories;
using System.Net;
using System.Net.Mail;

namespace OneTrack.Email_Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettingsRepository _emailSettingsRepository;

        public EmailSender(EmailSettingsRepository emailSettingsRepository)
        {
            _emailSettingsRepository = emailSettingsRepository;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailSettings = await _emailSettingsRepository.GetEmailSettingsAsync();

            if (emailSettings == null)
            {
                throw new InvalidOperationException("Email settings have not been configured");
            }

            var smtpClient = new SmtpClient(emailSettings.SmtpServer)
            {
                Port = emailSettings.Port,
                Credentials = new NetworkCredential(emailSettings.UserName, emailSettings.Password),
                EnableSsl = emailSettings.EnableSsl,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings.FromEmail, emailSettings.FromName),
                Subject = subject,
                IsBodyHtml = emailSettings.IsBodyHtml,
            };

            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task UpdateEmailSettings(EmailSettings settings)
        {
            await _emailSettingsRepository.UpdateEmailSettingsAsync(settings);
        }
    }
}
