using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Entities;

namespace SmartMentorLive.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
    {
       private readonly IAuthService _authService;
        public LoginCommandHandler(IAuthService authService)
        {
             _authService = authService;
        }
         public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Handler is thin: just orchestrates via the interface
            return await _authService.LoginAsync(request.Email, request.Password,cancellationToken);
        }

       
    }
}
