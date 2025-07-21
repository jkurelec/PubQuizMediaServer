using PubQuizMediaServer.Exceptions;
using System.Text.Json;

namespace PubQuizMediaServer.Util.Handler
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            switch (exception)
            {
                case ForbiddenException:
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    break;
                case NotFoundException:
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                    break;
                case InsufficientDataException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case UnauthorizedException:
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    break;
                case MyBadException:
                    context.Response.StatusCode = StatusCodes.Status406NotAcceptable;
                    break;
                case ConflictException:
                    context.Response.StatusCode = StatusCodes.Status409Conflict;
                    break;
                case DivineException:
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    break;
                case YourBadException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                case BadRequestException:
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    break;
                default:
                    _logger.LogCritical(exception, "Unhandled exception – crashing app");
                    throw exception;
            }

            var result = JsonSerializer.Serialize(new { message = exception.Message });
            return context.Response.WriteAsync(result);
        }
    }
}
