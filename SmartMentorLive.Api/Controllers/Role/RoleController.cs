using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartMentorLive.Api.Contracts.Common;
using SmartMentorLive.Application.Features.RoleFtr.Command;
using SmartMentorLive.Application.Features.RoleFtr.Dto;
using SmartMentorLive.Application.Features.RoleFtr.Queries;

namespace SmartMentorLive.Api.Controllers.Role
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;
        public RoleController(IMediator mediator)
        { 
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(CreateRoleCommand command)
        {
            var res = await _mediator.Send(command);
            // If exception happens, global handler catches it, formats ApiResponse, 
            // and this code never executes.
            return Ok(ApiResponse<RoleDto>.SuccessResponse(res, "Role created successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var res = await _mediator.Send(new GetAllRolesQuery());
            return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(res, "Roles fetched successfully"));
        }

    }
}
