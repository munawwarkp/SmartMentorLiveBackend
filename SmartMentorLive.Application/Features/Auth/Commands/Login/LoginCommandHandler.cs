using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Features.Auth.Interfaces;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Entities;

namespace SmartMentorLive.Application.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResultDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        public LoginCommandHandler(IUserRepository userRepository,IJwtTokenGenerator jwtTokenGenerator,IPasswordHasher<User> passwordHasher,IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;   
        }
         public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {

            var user = await _userRepository.GetEmailAsync(request.Email, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var verifyPassword= _passwordHasher.VerifyHashedPassword(user,user.PasswordHash,request.Password);

            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken =  _jwtTokenGenerator.GenerateAccessToken(user);
            var refreshToken = _jwtTokenGenerator.GenerateRefreshToken();

            var refreshTokenHash = HashToken(refreshToken);

            var refreshTokenEntity = new RefreshToken
            {
                TokenHash = refreshTokenHash,
                UserId = user.Id.ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(7),
            };

            //save refreshtoken in db
            await _refreshTokenRepository.AddAsync(refreshTokenEntity,cancellationToken);


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

        private string HashToken(string token)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
