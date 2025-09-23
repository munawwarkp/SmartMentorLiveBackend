using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using SmartMentorLive.Application.Features.RoleFtr.Dto;

namespace SmartMentorLive.Application.Features.RoleFtr.Command
{
    public record CreateRoleCommand(string Name) : IRequest<RoleDto>;
}