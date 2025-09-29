using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Features.RoleFtr.Dto;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Domain.Entities.Users;
using SmartMentorLive.Infrastructure.Persistence.UOW;
//using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Features.RoleFtr.Command
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CreateRoleCommandHandler(IRoleRepository roleRepository, IUnitOfWork unitOfWork)
        {
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            //check role exist ? tomorrow
            if( await _roleRepository.ExistsByNameAsync(request.Name, cancellationToken))
            {
                throw new Exception("Role already exists");
            }

            var role = new Role
            {
                Name = request.Name,
            };
          
            await _roleRepository.AddAsync(role,cancellationToken);
            await _unitOfWork.SaveChangeAsync(cancellationToken);
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
            };
            return roleDto;
        }
    }
}
