using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Features.RoleFtr.Dto;
using SmartMentorLive.Application.Interfaces.Repositories;

namespace SmartMentorLive.Application.Features.RoleFtr.Queries
{
    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, List<RoleDto>>
    {
        private readonly IRoleRepository _roleRepository;
        public GetAllRolesQueryHandler(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<RoleDto>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllRolesAsync(cancellationToken);
            return roles.Select(r => new RoleDto
            {
                Id = r.Id,
                Name = r.Name
            }).ToList();
        }
    }
}
