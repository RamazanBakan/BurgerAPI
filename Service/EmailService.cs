using System.Net;
using System.Net.Mail;
using System;

namespace WebApplication10.Service
{
    public class EmailService
    {
        private readonly System.String _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;

        public EmailService(IConfiguration configuration)
        {
            // appsettings.json'dan değerleri okuyun
            _smtpServer = configuration["EmailSettings:SMTPServer"];
            _smtpPort = int.Parse(configuration["EmailSettings:SMTPPort"]);
            _smtpUsername = configuration["EmailSettings:SMTPUsername"];
            _smtpPassword = configuration["EmailSettings:SMTPPassword"];
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                client.EnableSsl = true; // SSL etkinleştirildi

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUsername, "Burger Sipariş Sistemi"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // HTML formatında e-posta gönderimi
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}