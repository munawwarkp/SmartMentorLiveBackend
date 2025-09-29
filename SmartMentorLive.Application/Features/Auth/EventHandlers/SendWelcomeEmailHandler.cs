using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartMentorLive.Application.Features.Auth.Event.Event;
using SmartMentorLive.Application.Interfaces.Services;

namespace SmartMentorLive.Application.Features.Auth.EventHandlers
{
    public class SendWelcomeEmailHandler : INotificationHandler<UserRegisteredEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<SendWelcomeEmailHandler> _logger;
        public SendWelcomeEmailHandler(IEmailService emailService, ILogger<SendWelcomeEmailHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }
        public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var subject = "Welcome to Smart Mentor Live!";
                var body = $"Hi {notification.Name}, thanks for joining!";

                await _emailService.SendEmailAsync(notification.Email, subject, body);
                _logger.LogInformation("✅ Welcome email sent to {Email}", notification.Email);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send welcome email to {Email}", notification.Email);

            }

        }
    }
}
