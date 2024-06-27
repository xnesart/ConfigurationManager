using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ServiceConfigManager.Bll.Services;

namespace ServiceConfigManager.Api.Configuration.Filters;

public class IpBlockActionFilter : ActionFilterAttribute
{
    private readonly IIpBlockingService _blockingService;

    public IpBlockActionFilter(IIpBlockingService blockingService)
    {
        _blockingService = blockingService;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
        var isBlocked = _blockingService.IsBlocked(remoteIp!);
        if (isBlocked)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
            return;
        }

        base.OnActionExecuting(context);
    }
}