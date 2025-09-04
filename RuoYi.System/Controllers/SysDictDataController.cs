using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Framework;
using RuoYi.System.Services;

namespace RuoYi.System.Controllers
{
    [ApiDescriptionSettings("System")]
    [Route("system/dict/data")]
    public class SysDictDataController : ControllerBase
    {
        private readonly ILogger<SysDictDataController> _logger;
        private readonly SysDictDataService _sysDictDataService;
        private readonly SysDictTypeService _sysDictTypeService;
        public SysDictDataController(ILogger<SysDictDataController> logger, SysDictDataService sysDictDataService, SysDictTypeService sysDictTypeService)
        {
            _logger = logger;
            _sysDictDataService = sysDictDataService;
            _sysDictTypeService = sysDictTypeService;
        }

        [HttpGet("list")]
        [AppAuthorize("system:dict:list")]
        public async Task<SqlSugarPagedList<SysDictDataDto>> GetSysDictDataList([FromQuery] SysDictDataDto dto)
        {
            return await _sysDictDataService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("{dictCode}")]
        [AppAuthorize("system:dict:query")]
        public async Task<AjaxResult> Get(long dictCode)
        {
            var data = await _sysDictDataService.GetAsync(dictCode);
            return AjaxResult.Success(data);
        }

        [HttpGet("type/{dictType}")]
        public async Task<AjaxResult> GetListByDictType(string dictType)
        {
            List<SysDictData> data = await _sysDictTypeService.SelectDictDataByTypeAsync(dictType);
            if (data == null)
            {
                data = new List<SysDictData>();
            }

            return AjaxResult.Success(data);
        }

        [HttpPost("")]
        [AppAuthorize("system:dict:add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "字典数据", BusinessType = BusinessType.INSERT)]
        public async Task<AjaxResult> Add([FromBody] SysDictDataDto dto)
        {
            var data = await _sysDictDataService.InsertDictDataAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("system:dict:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [Log(Title = "字典数据", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] SysDictDataDto dto)
        {
            var data = await _sysDictDataService.UpdateDictDataAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{dictCodes}")]
        [AppAuthorize("system:dict:remove")]
        [Log(Title = "字典数据", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(long[] dictCodes)
        {
            await _sysDictDataService.DeleteDictDataByIdsAsync(dictCodes);
            return AjaxResult.Success();
        }

        [HttpPost("export")]
        [AppAuthorize("system:dict:export")]
        [Log(Title = "字典数据", BusinessType = BusinessType.EXPORT)]
        public async Task Export(SysDictDataDto dto)
        {
            var list = await _sysDictDataService.GetDtoListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }
    }
}