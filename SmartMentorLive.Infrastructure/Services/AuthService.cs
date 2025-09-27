using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Core;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using SmartMentorLive.Application.Event;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Entities;
using SmartMentorLive.Infrastructure.Persistence.Context;
using SmartMentorLive.Infrastructure.Persistence.UOW;

namespace SmartMentorLive.Infrastructure.Services
{
    public class AuthService:IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IRoleRepository _roleService;
        private readonly IMediator _mediator;
        private readonly IAuthUnitOfWork _unitOfWork;

        public AuthService(
            IUserRepository userRepository,
            IJwtTokenGenerator jwtTokenGenerator,
            IPasswordHasher<User> passwordHasher,
            IRefreshTokenRepository refreshTokenRepository,
            IRoleRepository roleRepository,
            AppDbContext context,
            IMediator mediator,
            IAuthUnitOfWork unitOfWork)   
        {
            _context = context;
            _userRepository = userRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _roleService = roleRepository;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }

        public async Task<RegisterResultDto> RegisterAsync(string name, string email, string password, string role, CancellationToken cancellationToken)
        {           

            if (await _userRepository.ExistsByEmailAsync(email,cancellationToken))
                throw new Exception("User already exists");

            //get role and validate
            var roleEntity = await _roleService.GetByNameAsync(role, cancellationToken);

            if (roleEntity == null || !roleEntity.IsRegistrable)
                throw new Exception("Invalid role specified");

            //start transaction for multi entity operation

            //await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Name = name,
                    Email = email,
                    RoleId = roleEntity.Id,
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, password);


                //assign profile based on role
                if (roleEntity.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
                    user.StudentProfile = new StudentProfile { User = user };
                else if (roleEntity.Name.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
                    user.MentorProfile = new MentorProfile { User = user };

                //track entity
                await _userRepository.AddAsync(user, cancellationToken);

                //commit changes
                await _unitOfWork.SaveChangeAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                //side effect ( welcom email )outside transaction
                await _mediator.Publish(new UserRegisteredEvent(user.Id, user.Email, user.Name),cancellationToken);

                return new RegisterResultDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = role,
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }   

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
