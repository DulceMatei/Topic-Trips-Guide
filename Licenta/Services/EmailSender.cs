using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace Licenta.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var smtp = new SmtpClient
                {
                    Host = _config["EmailSettings:SmtpServer"],
                    Port = int.Parse(_config["EmailSettings:Port"]),
                    EnableSsl = true,
                    Credentials = new NetworkCredential(
                        _config["EmailSettings:SenderEmail"],
                        _config["EmailSettings:Password"]
                    )
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_config["EmailSettings:SenderEmail"], _config["EmailSettings:SenderName"]),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                await smtp.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine("EROARE LA TRIMITEREA EMAILULUI:");
                Console.WriteLine(ex.Message);
                throw;
            }
        }

    }
}
