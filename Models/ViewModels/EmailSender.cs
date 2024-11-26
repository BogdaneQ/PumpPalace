using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PumpPalace.Models.ViewModels
{
    public class EmailSender
    {
        private readonly SendGridSettings _sendGridSettings;

        public EmailSender(IOptions<SendGridSettings> sendGridSettings)
        {
            _sendGridSettings = sendGridSettings.Value;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var client = new SendGridClient(_sendGridSettings.ApiKey);
            var from = new EmailAddress(_sendGridSettings.FromEmail, _sendGridSettings.FromName);
            var to = new EmailAddress(toEmail);
            var message = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: body, htmlContent: body);
            var response = await client.SendEmailAsync(message);

            if (!response.IsSuccessStatusCode)
            {
                // Loguj błędy w razie potrzeby
                throw new System.Exception($"Wysłanie e-maila nie powiodło się: {response.StatusCode}");
            }
        }
    }
}
