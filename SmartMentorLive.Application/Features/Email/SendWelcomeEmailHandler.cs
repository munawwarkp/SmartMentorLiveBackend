using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using SmartMentorLive.Application.Interfaces.Services;

namespace SmartMentorLive.Application.Features.Email
{
    public class SendWelcomeEmailHandler : IRequestHandler<SendWelcomeEmailCommand>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<SendWelcomeEmailHandler> _logger;
        public SendWelcomeEmailHandler(IEmailService emailService, ILogger<SendWelcomeEmailHandler> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }
        public async Task Handle(SendWelcomeEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var subject = "Welcome to Smart Mentor Live!";
                var body = $"Hi {request.Name}, thanks for joining!";

                await _emailService.SendEmailAsync(request.RecipientEmail, subject, body);
                _logger.LogInformation("✅ Welcome email sent to {Email}", request.RecipientEmail);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Failed to send welcome email to {Email}", request.RecipientEmail);

            }

        }
    }
}
