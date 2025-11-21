using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Services.CustomExceptions;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TrainerHubAPI.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        public async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            string traceId = context.Items["CorrelationId"]?.ToString() ?? Guid.NewGuid().ToString();
             (HttpStatusCode status, string title) errorInfo = ex switch
            {
                NotFoundException => (HttpStatusCode.NotFound, "Not Found"),
                ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
                ArgumentException => (HttpStatusCode.BadRequest, "Invalid Argument"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "User Not Authenticated"),
                InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid Operation"),
                FormatException => (HttpStatusCode.BadRequest, "Invalid Format"),
                _ => (HttpStatusCode.InternalServerError, "Server Error")
            };

            Log.Error(ex, "Error occurred. TraceId: {TraceId}, Title: {Title}", traceId, errorInfo.title);

            ApiProblemDetails problemDetails = new ApiProblemDetails()
            {
                Status = (int)errorInfo.status,
                Title = errorInfo.title,
                Detail = ex.Message,
                Instance = context.Request.Path,
                TraceId = traceId
            };
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = problemDetails.Status.Value;

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, options));
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
