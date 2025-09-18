using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Api.Models.Users.Responses;
using SmartMentorLive.Application.Features.Auth.Dtos;

namespace SmartMentorLive.Application.Features.Auth.Commands.Register
{
    public record  RegisterUserCommand(
        string Name,
        string Email,
        string Password,
        string Role
     ) : IRequest<RegisterResultDto>;
}
