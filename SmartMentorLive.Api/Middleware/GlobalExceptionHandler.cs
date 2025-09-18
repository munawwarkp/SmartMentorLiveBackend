

using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SmartMentorLive.Api.Contracts.Common;

namespace SmartMentorLive.Api.MIddleware
{
    public class GlobalExceptionHandler:IMiddleware
    {
        private readonly ILogger _logger;
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                _logger.LogError("Unhandled Exception : {Message}",ex.Message); 

                var problem = MaptoProblemDetail(ex,context);
                var response = ApiResponse<ProblemDetails>.FailResponse(problem.Title ?? "Error",
                    new List<string> { problem.Detail ?? "Unexpected error" });
                response.Data = problem;
                response.TraceId = context.TraceIdentifier;

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;

                //var json = JsonSerializer.Serialize(problem);
                await context.Response.WriteAsJsonAsync(response);    
            }
            //throw new NotImplementedException();
        }

        //ProblmDetails is used for standardized json format

        private ProblemDetails MaptoProblemDetail(Exception ex,HttpContext context)
        {
            ProblemDetails problem;
            switch (ex)
            {

                case UnauthorizedAccessException unauthorizedAccessEx:
                    problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = "You must be logged in",
                        Instance = context.Request.Path
                    };
                    break;

                case KeyNotFoundException keyNotFoundException:
                    problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status404NotFound,
                        Title = "Not found",
                        Detail = ex.Message,
                        Instance = context.Request.Path
                    };
                    break;

                case ValidationException validationEx:
                    problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Validation Error",
                        Detail = validationEx.Message,
                        Instance = context.Request.Path
                    };
                    break;

                case BadHttpRequestException badHttpEx:
                    problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Title = "Bad Request",
                        Detail = badHttpEx.Message,
                        Instance = context.Request.Path
                    };
                    break;

                case AggregateException aggEx:
                    problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Aggregate Exception",
                        Detail = aggEx.Flatten().Message,
                        Instance = context.Request.Path
                    };
                    break;


                default:
                    problem = new ProblemDetails
                    {
                        Status = StatusCodes.Status500InternalServerError,
                        Title = "Server Error",
                        Detail = "An unexpected error occurred. Please try again later.",
                        Instance = context.Request.Path
                    };
                    break;

            }

            return problem;
        }
    }
}
