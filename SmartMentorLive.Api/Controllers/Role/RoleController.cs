using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public Task<IActionResult> AddRole()
        {

        }
    }
}
