using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Data.Dtos;
using RuoYi.System.Services;
using SqlSugar;
using System.IO;

namespace RuoYi.System.Controllers
{
    [ApiDescriptionSettings("Monitor")]
    [Route("monitor/logininfor")]
    public class SysLogininforController : ControllerBase
    {
        private readonly ILogger<SysLogininforController> _logger;
        private readonly SysLogininforService _sysLogininforService;
        public SysLogininforController(ILogger<SysLogininforController> logger, SysLogininforService sysLogininforService)
        {
            _logger = logger;
            _sysLogininforService = sysLogininforService;
        }

        [HttpGet("list")]
        [AppAuthorize("system:logininfor:list")]
        public async Task<SqlSugarPagedList<SysLogininforDto>> GetSysLogininforList([FromQuery] SysLogininforDto dto)
        {
            return await _sysLogininforService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("")]
        [HttpGet("{id}")]
        [AppAuthorize("system:logininfor:query")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _sysLogininforService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpPost("")]
        [AppAuthorize("system:logininfor:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "系统访问记录", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysLogininforDto dto)
        {
            var data = await _sysLogininforService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("system:logininfor:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "系统访问记录", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysLogininforDto dto)
        {
            var data = await _sysLogininforService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{ids}")]
        [AppAuthorize("system:logininfor:remove")]
        [RuoYi.System.Log(Title = "系统访问记录", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _sysLogininforService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        [HttpPost("import")]
        [AppAuthorize("system:logininfor:import")]
        [RuoYi.System.Log(Title = "系统访问记录", BusinessType = BusinessType.IMPORT)]
        public async Task Import([Required] IFormFile file)
        {
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var list = await ExcelUtils.ImportAsync<SysLogininforDto>(stream);
            await _sysLogininforService.ImportDtoBatchAsync(list);
        }

        [HttpPost("export")]
        [AppAuthorize("system:logininfor:export")]
        [RuoYi.System.Log(Title = "系统访问记录", BusinessType = BusinessType.EXPORT)]
        public async Task Export(SysLogininforDto dto)
        {
            var list = await _sysLogininforService.GetDtoListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }
    }
}