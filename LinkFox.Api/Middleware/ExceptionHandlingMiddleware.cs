using System.Net;

namespace LinkFox.Api.Middleware
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
                _logger.LogError(ex, "Unhandled exception occurred on {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            string errorCode;

            switch (ex)
            {
                case BadRequestException:
                    status = HttpStatusCode.BadRequest;
                    errorCode = "INVALID_REQUEST";
                    break;

                case NotFoundException:
                    status = HttpStatusCode.NotFound;
                    errorCode = "NOT_FOUND";
                    break;

                case ConflictException:
                    status = HttpStatusCode.Conflict;
                    errorCode = "ALIAS_CONFLICT";
                    break;

                default:
                    status = HttpStatusCode.InternalServerError;
                    errorCode = "INTERNAL_ERROR";
                    break;
            }

            var response = new { errorCode, errorMessage = ex.Message };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
