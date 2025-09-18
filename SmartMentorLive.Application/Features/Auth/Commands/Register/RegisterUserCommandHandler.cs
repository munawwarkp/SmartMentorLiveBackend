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
using SmartMentorLive.Application.Features.Auth.Interfaces;
using SmartMentorLive.Application.Interfaces;
using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Features.Auth.Commands.Register
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand,RegisterResultDto >
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRoleRepository _roleService;

        public RegisterUserCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, IUnitOfWork unitOfWork,IRoleRepository roleService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _unitOfWork = unitOfWork;
            _roleService = roleService;
        }

        public async Task<RegisterResultDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
           

            bool exists = await _userRepository.ExistsByEmailAsync(request.Email);
            if (exists)
            {
                throw new Exception("User already exists");
            }

            //only allow student and mentor registration
            if(!request.Role.Equals("Student",StringComparison.OrdinalIgnoreCase) &&
                !request.Role.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception("Registration for this role is not allowed");
            }

            var roleId = await _roleService.GetRoleIdByNameAsync(request.Role, cancellationToken);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email,
                RoleId = roleId
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

            //  Assign profile based on role
            if (request.Role.Equals("Student", StringComparison.OrdinalIgnoreCase))
            {
                user.StudentProfile = new StudentProfile { User = user };
            }
            else if (request.Role.Equals("Mentor", StringComparison.OrdinalIgnoreCase))
            {
                user.MentorProfile = new MentorProfile { User = user };
            }

            await _userRepository.AddAsync(user,cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken); //commit transaction

            return new RegisterResultDto
            {
                UserId = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = request.Role,              
            };

        }
    }
}
