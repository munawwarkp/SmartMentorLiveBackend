using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SmartMentorLive.Api.Models.Users.Responses;
using SmartMentorLive.Application.Event;
using SmartMentorLive.Application.Features.Auth.Dtos;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Application.Interfaces.Services;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Persistence.UOW;

namespace SmartMentorLive.Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand,RegisterResultDto >
    {
        //private readonly IAuthService _authService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IAuthUnitOfWork _authUnitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMediator _mediator;

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IAuthUnitOfWork authUnitOfWork,
            IPasswordHasher<User> password,
            IMediator mediator)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _authUnitOfWork = authUnitOfWork;
            _passwordHasher = password;
            _mediator = mediator;
        }

        public async Task<RegisterResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            //return await _authService.RegisterAsync(request.Name, request.Email, request.Password, request.Role,cancellationToken);
            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                throw new Exception("User already exists");

            //get role and validate
            var roleEntity = await _roleRepository.GetByNameAsync(request.Role, cancellationToken);

            if (roleEntity == null || !roleEntity.IsRegistrable)
                throw new Exception("Invalid role specified");

            //start transaction for multi entity operation

            //await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            await _authUnitOfWork.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Name = request.Name,
                    Email = request.Email,
                    RoleId = roleEntity.Id,
                };
                user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);


                //assign profile based on role
                if (roleEntity.Name.Equals("Student", StringComparison.OrdinalIgnoreCase))
                    user.StudentProfile = new StudentProfile { User = user };
                else if (roleEntity.Name.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
                    user.MentorProfile = new MentorProfile { User = user };

                //track entity
                await _userRepository.AddAsync(user, cancellationToken);

                //commit changes
                await _authUnitOfWork.SaveChangeAsync(cancellationToken);
                await _authUnitOfWork.CommitTransactionAsync(cancellationToken);

                //side effect ( welcom email )outside transaction
                await _mediator.Publish(new UserRegisteredEvent(user.Id, user.Email, user.Name), cancellationToken);

                return new RegisterResultDto
                {
                    UserId = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role.Name,
                };
            }
            catch (Exception ex)
            {
                await _authUnitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }

        }
    }
}
