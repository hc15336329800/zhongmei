using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Logging;
using RuoYi.Data.Slave.Dtos;
using RuoYi.Framework.RateLimit;

namespace RuoYi.Admin
{
    [ApiDescriptionSettings("Sample")]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> _logger;
        private readonly SystemService _systemService;
        private readonly RuoYi.System.Slave.Services.SysUserService _slaveSysUserService;
        public SampleController(ILogger<SampleController> logger, SystemService systemService, System.Slave.Services.SysUserService slaveSysUserService)
        {
            _logger = logger;
            _systemService = systemService;
            _slaveSysUserService = slaveSysUserService;
        }

        [HttpGet("{id}")]
        public async Task<SlaveSysUserDto> Get(long? id)
        {
            return await _slaveSysUserService.GetAsync(id);
        }

        [HttpGet("getWithPerminAndRole/{id}")]
        [AppAuthorize("system:dept:query")]
        [AppRoleAuthorize("admin")]
        public async Task<SlaveSysUserDto> GetWithPerminAndRole(long? id)
        {
            return await _slaveSysUserService.GetAsync(id);
        }

        [HttpGet("rateLimit")]
        [EnableRateLimiting(LimitType.Default)]
        public string RateLimit()
        {
            return "rateLimit";
        }

        [HttpGet("ipRateLimit")]
        [EnableRateLimiting(LimitType.IP)]
        public string IpRateLimit()
        {
            return "ipRateLimit";
        }
    }
}