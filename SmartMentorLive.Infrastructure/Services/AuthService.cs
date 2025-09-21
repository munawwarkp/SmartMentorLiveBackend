using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Entities;

namespace SmartMentorLive.Infrastructure.Services
{
    public class AuthService:IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleService;


        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher<User> passwordHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IUnitOfWork unitOfWork,
            IRoleRepository roleRepository)   
        {
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
            _roleService = roleRepository;
        }

        public async Task<RegisterResultDto> RegisterAsync(string name, string email, string password, string role, CancellationToken cancellationToken)
        {
            bool exists = await _userRepository.ExistsByEmailAsync(email);
            if (exists)
            {
                throw new Exception("User already exists");
            }

            //only allow student and mentor registration
            if (!role.Equals("Student", StringComparison.OrdinalIgnoreCase) &&
                !role.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Registration for this role is not allowed");
            }

            var roleId = await _roleService.GetRoleIdByNameAsync(role, cancellationToken);

            var user = new User
            {
                Name = name,
                Email = email,
                RoleId = roleId
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            //  Assign profile based on role
            if (role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                user.StudentProfile = new StudentProfile { User = user };
            }
            else if (role.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
            {
                user.MentorProfile = new MentorProfile { User = user };
            }

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken); //commit transaction

            return new RegisterResultDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = role,
            };
        }



        public async Task<LoginResultDto> LoginAsync(string email, string password, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetEmailAsync(email, cancellationToken);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid email or password");

            var verifyPassword = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verifyPassword == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid email or password");
            }

            var accessToken = _jwtTokenGenerator.GenerateAccessToken(user);
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

        private string HashToken(string token)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(bytes);
        }
    }
}
