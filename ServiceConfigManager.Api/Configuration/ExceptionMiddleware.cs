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
        catch (Exception ex)
        {
            _logger.Error($"Something went wrong: {ex}");
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleValidationExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    } 
    
    private async Task HandleAuthenticationExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    }   
    
    private async Task HandleNotFoundExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.NotFound;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message
        }.ToString());
    }  
    
    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        await context.Response.WriteAsync(new ErrorDetails()
        {
            StatusCode = context.Response.StatusCode,
            Message = "Internal Server Error from the custom middleware."
        }.ToString());
    }
}