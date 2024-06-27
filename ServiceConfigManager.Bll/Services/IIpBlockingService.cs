using System.Net;

namespace ServiceConfigManager.Bll.Services;

public interface IIpBlockingService
{
    bool IsBlocked(IPAddress ipAddress);
}