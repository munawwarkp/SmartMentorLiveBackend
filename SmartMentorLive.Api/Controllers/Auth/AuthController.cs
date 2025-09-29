using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartMentorLive.Api.Contracts.Common;
using SmartMentorLive.Application.Features.Auth.Commands.Login;
using SmartMentorLive.Application.Features.Auth.Commands.Register;
using SmartMentorLive.Application.Features.Auth.Dtos;
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
            var result = await _mediator.Send(command);

            return Ok(ApiResponse<RegisterResultDto>.SuccessResponse(result,"Registration succesfull"));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(ApiResponse<LoginResultDto>.SuccessResponse(result,"Login succesfull"));
        }
    }
}
