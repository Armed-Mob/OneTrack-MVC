using Microsoft.EntityFrameworkCore;
using OneTrack.Email_Services;

namespace OneTrack.Data
{
    public class SettingsDbContext : DbContext
    {
        public SettingsDbContext(DbContextOptions<SettingsDbContext> options) : base(options)
        {
        }

        public DbSet<EmailSettings> EmailSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optionally Configure The EmailSettings Entity Here
            modelBuilder.Entity<EmailSettings>().HasKey(e => e.Id);

            modelBuilder.Entity<EmailSettings>().ToTable(nameof(EmailSettings));

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.SmtpServer)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.Port)
                .IsRequired();

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.UserName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.FromEmail)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.FromName)
                .IsRequired()
                .HasMaxLength(256);

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.EnableSsl)
                .IsRequired();

            modelBuilder.Entity<EmailSettings>()
                .Property(e => e.IsBodyHtml)
                .IsRequired();

            modelBuilder.Entity<EmailSettings>().HasData(
                new EmailSettings
                {
                    Id = 1000,
                    SmtpServer = "localhost",
                    Port = 587,
                    UserName = "user@example.com",
                    Password = "password",
                    FromEmail = "noreply@example.com",
                    FromName = "OneTrack App",
                    EnableSsl = true,
                    IsBodyHtml = true
                });
        }
    }
}
