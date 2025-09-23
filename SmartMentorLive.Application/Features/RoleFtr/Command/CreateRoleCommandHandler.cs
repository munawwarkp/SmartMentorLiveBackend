using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Features.RoleFtr.Dto;
using SmartMentorLive.Application.Interfaces.Repositories;
using SmartMentorLive.Domain.Entities.Users;
//using SmartMentorLive.Domain.Entities.Users;

namespace SmartMentorLive.Application.Features.RoleFtr.Command
{
    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, RoleDto>
    {
        private readonly IRoleRepository _roleRepository;
        public CreateRoleCommandHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            //check role exist ? tomorrow

            var role = new Role
            {
                Name = request.Name,
            };
          
          var roleAdded = await _roleRepository.AddAsync(role,cancellationToken);
            var roleDto = new RoleDto
            {
                Id = roleAdded.Id,
                Name = roleAdded.Name,
            };
            return roleDto;
        }
    }
}
