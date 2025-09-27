using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace SmartMentorLive.Application.Features.Auth.Event.Event
{
    public record UserRegisteredEvent(int UserId, string Email, string Name) : INotification;

}
