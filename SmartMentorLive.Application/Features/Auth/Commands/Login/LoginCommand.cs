using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Features.Auth.Dtos;

namespace SmartMentorLive.Application.Features.Auth.Commands.Login
{
    public record LoginCommand(
            string Email,
            string Password
        ):IRequest<LoginResultDto>;
    
}
