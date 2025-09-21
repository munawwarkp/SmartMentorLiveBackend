using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Api.Models.Users.Responses;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand,RegisterResultDto >
    {
       private readonly IAuthService _authService;
        public RegisterUserCommandHandler(IAuthService authService)
        {
           _authService = authService;
        }

        public async Task<RegisterResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
           return await _authService.RegisterAsync(request.Name, request.Email, request.Password, request.Role,cancellationToken);
        }
    }
}
