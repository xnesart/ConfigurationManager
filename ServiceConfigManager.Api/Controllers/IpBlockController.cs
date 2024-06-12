using Microsoft.AspNetCore.Mvc;

namespace ServiceConfigManager.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class IpBlockController:ControllerBase
{
    [HttpGet("unblocked")]
    public string Unblocked()
    {
        return "Unblocked access";
    }
    [HttpGet("blocked")]
    public string Blocked()
    {
        return "Blocked access";
    }
}