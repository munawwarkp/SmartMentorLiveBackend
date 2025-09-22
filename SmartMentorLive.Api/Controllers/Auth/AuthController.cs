using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartMentorLive.Application.Features.Auth.Commands.Login;
using SmartMentorLive.Application.Features.Auth.Commands.Register;
using SmartMentorLive.Application.Features.Email;
using SmartMentorLive.Domain.Entities.Users;

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
            try
            {
                var result = await _mediator.Send(command);
                await _mediator.Send(new SendWelcomeEmailCommand
                {
                    RecipientEmail = result.Email,
                    Name = result.Name
                });



            }
            catch (Exception ex)
            {

            }

            if (result.Success) // or check null depending on your DTO
            {
                await _mediator.Send(new SendWelcomeEmailCommand
                {
                    RecipientEmail = result.Email,
                    Name = result.Name
                });
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
