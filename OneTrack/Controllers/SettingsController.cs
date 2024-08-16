using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OneTrack.Email_Services;
using OneTrack.Repostiories;

namespace OneTrack.Controllers
{
    public class SettingsController : Controller
    {
        private readonly EmailSettingsRepository _emailSettingsRepository;
        private readonly IEmailSender _emailSender;

        public SettingsController(IEmailSender emailSender, EmailSettingsRepository emailSettingsRepository)
        {
            _emailSender = emailSender;
            _emailSettingsRepository = emailSettingsRepository;
        }

        [HttpGet]
        public async Task<IActionResult> EmailSettings()
        {
            var emailSettings = await _emailSettingsRepository.GetEmailSettingsAsync();
            return View(emailSettings);
        }

        [HttpPost]
        public async Task<IActionResult> EmailSettings(EmailSettings model)
        {
            if (ModelState.IsValid)
            {
                await _emailSender.UpdateEmailSettings(model); // Update the email settings in runtime
                return RedirectToAction("EmailSettings");
            }

            return View(model);
        }
    }
}
