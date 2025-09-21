using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMentorLive.Application.Features.Auth.Commands.Login;
using SmartMentorLive.Application.Features.Auth.Commands.Register;

namespace SmartMentorLive.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserCommand command)
        {
           var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
