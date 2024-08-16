using Microsoft.EntityFrameworkCore;
using OneTrack.Data;
using OneTrack.Email_Services;

namespace OneTrack.Repostiories
{
    public class EmailSettingsRepository
    {
        private readonly SettingsDbContext _context;

        public EmailSettingsRepository(SettingsDbContext context)
        {
            _context = context;
        }

        public async Task<EmailSettings> GetEmailSettingsAsync()
        {
            return await _context.EmailSettings.FirstOrDefaultAsync();
        }

        public async Task UpdateEmailSettingsAsync(EmailSettings settings)
        {
            var existingSettings = await GetEmailSettingsAsync();
            if (existingSettings != null)
            {
                existingSettings.SmtpServer = settings.SmtpServer;
                existingSettings.Port = settings.Port;
                existingSettings.FromEmail = settings.FromEmail;
                existingSettings.IsBodyHtml = settings.IsBodyHtml;
                existingSettings.EnableSsl = settings.EnableSsl;
                existingSettings.FromName = settings.FromName;
                existingSettings.Password = settings.Password;
                existingSettings.UserName = settings.UserName;

                _context.EmailSettings.Update(existingSettings);
            }
            else
            {
                _context.EmailSettings.Add(settings);
            }

            await _context.SaveChangesAsync();
        }
    }
}
