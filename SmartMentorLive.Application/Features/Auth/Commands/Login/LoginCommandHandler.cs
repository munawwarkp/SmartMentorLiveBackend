using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Features.Auth.Helpers;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Entities;
using SmartMentorLive.Infrastructure.Persistence.UOW;

namespace SmartMentorLive.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
    {
       //private readonly IAuthService _authService;
       private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IMediator _mediator;
        private readonly IAuthUnitOfWork _unitOfWork;

        public LoginCommandHandler(
             IUserRepository userRepository,
             IJwtTokenGenerator jwtTokenGenerator,
             IPasswordHasher<User> passwordHasher,
             IRefreshTokenRepository refreshTokenRepository,
             IMediator mediator,
             IAuthUnitOfWork unitOfWork)
        {
             //_authService = authService;
             _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }
         public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Handler is thin: just orchestrates via the interface
            //return await _authService.LoginAsync(request.Email, request.Password,cancellationToken);

            var user = await _userRepository.GetEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

          
            AuthHelper.VerifyPassword(user,request.Password, _passwordHasher);

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshTokenHash = AuthHelper.HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            };

            //save refreshtoken in db
            await _refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);


            var res = new LoginResultDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role.Name,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };

            return res;

        }

       
    }
}
