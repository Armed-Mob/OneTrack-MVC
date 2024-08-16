namespace OneTrack.Email_Services
{
    public class EmailSettings
    {
        public int Id { get; set; }
        public string SmtpServer { get; set; } = string.Empty;
        public int Port { get; set; } = 0;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool EnableSsl { get; set; } = true;
        public bool IsBodyHtml { get; set; } = true;
    }
}
