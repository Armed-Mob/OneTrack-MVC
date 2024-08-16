namespace OneTrack.Email_Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
        Task UpdateEmailSettings(EmailSettings settings);
    }
}
