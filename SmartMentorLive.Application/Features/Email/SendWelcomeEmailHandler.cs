using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Interfaces.Services;

namespace SmartMentorLive.Application.Features.Email
{
    public class SendWelcomeEmailHandler : IRequestHandler<SendWelcomeEmailCommand>
    {
        private readonly IEmailService _emailService;
        public SendWelcomeEmailHandler(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task Handle(SendWelcomeEmailCommand request, CancellationToken cancellationToken)
        {
            var subject = "Welcome to Smart Mentor Live!";
            var body = $"Hi {request.Name}, thanks for joining!";

            await _emailService.SendEmailAsync(request.RecipientEmail, subject, body);
        }
    }
}
