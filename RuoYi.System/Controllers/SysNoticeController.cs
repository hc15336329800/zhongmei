using RuoYi.Common.Enums;
using RuoYi.Data.Dtos;
using RuoYi.Framework;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers
{
    [ApiDescriptionSettings("System")]
    [Route("system/notice")]
    public class SysNoticeController : ControllerBase
    {
        private readonly ILogger<SysNoticeController> _logger;
        private readonly SysNoticeService _sysNoticeService;
        public SysNoticeController(ILogger<SysNoticeController> logger, SysNoticeService sysNoticeService)
        {
            _logger = logger;
            _sysNoticeService = sysNoticeService;
        }

        [HttpGet("list")]
        [AppAuthorize("system:notice:list")]
        public async Task<SqlSugarPagedList<SysNoticeDto>> GetSysNoticeList([FromQuery] SysNoticeDto dto)
        {
            return await _sysNoticeService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("{id}")]
        [AppAuthorize("system:notice:query")]
        public async Task<AjaxResult> Get(int id)
        {
            var data = await _sysNoticeService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpPost("")]
        [AppAuthorize("system:notice:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "通知公告", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysNoticeDto dto)
        {
            var data = await _sysNoticeService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("system:notice:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "通知公告", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysNoticeDto dto)
        {
            var data = await _sysNoticeService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{ids}")]
        [AppAuthorize("system:notice:remove")]
        [Log(Title = "通知公告", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(long[] ids)
        {
            var data = await _sysNoticeService.DeleteAsync(ids);
            return AjaxResult.Success(data);
        }
    }
}