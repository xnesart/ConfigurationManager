using System.Net;
using Serilog;
using ServiceConfigManager.Core.Exceptions;
using AuthenticationException = System.Security.Authentication.AuthenticationException;

namespace ServiceConfigManager.Api.Configuration;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger = Log.ForContext<ExceptionMiddleware>();

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ValidationException ex)
        {
            _logger.Error($"Ошибка валидации {ex.Message}");
            await HandleValidationExceptionAsync(httpContext, ex);
        }
        catch (AuthenticationException ex)
        {
            _logger.Error($"Ошибка аутентификации {ex.Message}");
            await HandleAuthenticationExceptionAsync(httpContext, ex);
        }
        catch (NotFoundException ex)
        {
            _logger.Error($"Ошибка {ex.Message}");
            await HandleNotFoundExceptionAsync(httpContext, ex);
        }
        catch (RabbitMqException ex)
        {
            _logger.Error($"Ошибка RabbitMq {ex.Message}");
            await HandleNotFoundExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            _logger.Error($"Something went wrong: {ex}");
            await HandleRabbitExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, Exception exception)
    {
        await HandleExceptionAsync(HttpStatusCode.UnprocessableEntity, context, exception);
    }

    private async Task HandleRabbitExceptionAsync(HttpContext context, Exception exception)
    {
        await HandleExceptionAsync(HttpStatusCode.BadGateway, context, exception);
    }

    private async Task HandleAuthenticationExceptionAsync(HttpContext context, Exception exception)
    {
        await HandleExceptionAsync(HttpStatusCode.Unauthorized, context, exception);
    }

    private async Task HandleNotFoundExceptionAsync(HttpContext context, Exception exception)
    {
        await HandleExceptionAsync(HttpStatusCode.NotFound, context, exception);
    }

    private async Task HandleExceptionAsync(HttpStatusCode statusCode, HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    }
}