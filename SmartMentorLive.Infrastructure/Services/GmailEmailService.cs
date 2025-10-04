using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Infrastructure.Options;

namespace SmartMentorLive.Infrastructure.Services
{
    public class GmailEmailService:IEmailService
    {
        private readonly ITokenManager _tokenManager;
        private readonly GmailOptions _settings;
         
        //private readonly string _fromEmail; //error

        public GmailEmailService(ITokenManager tokenManager, IOptions<GmailOptions> settings)
        {
            _tokenManager = tokenManager;
            _settings = settings.Value;

            //_fromEmail = fromEmail;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var accessToken = await _tokenManager.GetAccessTokenAsync();

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Smart Mentor",_settings.UserEmail));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            //create SMTP client

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(new SaslMechanismOAuth2(_settings.UserEmail, accessToken));
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
