using Microsoft.Extensions.Logging;
using RuoYi.Framework;

namespace RuoYi.System.Controllers
{
    [Route("monitor/druid")]
    [ApiDescriptionSettings("Monitor")]
    public class DruidController : ControllerBase
    {
        private readonly ILogger<SysOperLogController> _logger;
        public DruidController(ILogger<SysOperLogController> logger)
        {
            _logger = logger;
        }

        [HttpGet("")]
        [AppAuthorize("monitor:druid:list")]
        public AjaxResult GetDruidInfo()
        {
            return AjaxResult.Success();
        }
    }
}