using Microsoft.Extensions.Logging;
using RuoYi.Framework;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers
{
    [Route("monitor/server")]
    [ApiDescriptionSettings("Monitor")]
    public class ServerController : ControllerBase
    {
        private readonly ILogger<SysOperLogController> _logger;
        private readonly ServerService _serverService;
        public ServerController(ILogger<SysOperLogController> logger, ServerService serverService)
        {
            _logger = logger;
            _serverService = serverService;
        }

        [HttpGet("")]
        [AppAuthorize("monitor:server:list")]
        public AjaxResult GetServerInfo()
        {
            var server = _serverService.GetServerInfo();
            return AjaxResult.Success(server);
        }
    }
}