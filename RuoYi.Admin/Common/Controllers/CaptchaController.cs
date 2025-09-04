using Lazy.Captcha.Core;
using RuoYi.Framework.Cache;
using RuoYi.System.Services;

namespace RuoYi.Admin
{
    [ApiDescriptionSettings("Common")]
    [AllowAnonymous]
    public class CaptchaController : Controller
    {
        private readonly ILogger<CaptchaController> _logger;
        private readonly SysConfigService _sysConfigService;
        private readonly ICaptcha _captcha;
        private readonly ICache _cache;
        public CaptchaController(ILogger<CaptchaController> logger, ICache cache, ICaptcha captcha, SysConfigService sysConfigService)
        {
            _logger = logger;
            _cache = cache;
            _captcha = captcha;
            _sysConfigService = sysConfigService;
        }

        [HttpGet("/captchaImage")]
        public object GetCaptchaImage()
        {
            bool captchaEnabled = _sysConfigService.IsCaptchaEnabled();
            if (!captchaEnabled)
            {
                return new
                {
                    CaptchaEnabled = captchaEnabled
                };
            }

            string uuid = Guid.NewGuid().ToString();
            var info = _captcha.Generate(uuid);
            return new
            {
                Uuid = uuid,
                Img = info.Base64
            };
        }
    }
}