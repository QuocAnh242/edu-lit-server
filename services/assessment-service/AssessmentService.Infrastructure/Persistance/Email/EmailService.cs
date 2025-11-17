using AssessmentService.Application.IServices;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AssessmentService.Infrastructure.Persistance.Email
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _options;

        public EmailService(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var client = new SmtpClient(_options.Host, _options.Port)
            {
                Credentials = new NetworkCredential(_options.Username, _options.Password),
                EnableSsl = true
            };

            var mail = new MailMessage(_options.From, to, subject, body);

            await client.SendMailAsync(mail);
        }
    }
}
