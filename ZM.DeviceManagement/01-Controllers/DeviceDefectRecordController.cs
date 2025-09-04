using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Utils;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm/defectRecord")]
    [Route("zm/defectRecord")]
    [AllowAnonymous]
    public class DeviceDefectRecordController : ControllerBase
    {
        private readonly ILogger<DeviceDefectRecordController> _logger;
        private readonly DeviceDefectRecordService _deviceDefectRecordService;
        public DeviceDefectRecordController(ILogger<DeviceDefectRecordController> logger, DeviceDefectRecordService deviceDefectRecordService)
        {
            _logger = logger;
            _deviceDefectRecordService = deviceDefectRecordService;
        }

        [HttpGet("list")]
        public async Task<SqlSugarPagedList<DeviceDefectRecordDto>> GetDeviceDefectRecordPagedList([FromQuery] DeviceDefectRecordDto dto)
        {
            return await _deviceDefectRecordService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("info/{id}")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _deviceDefectRecordService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpPost("add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Add([FromBody] DeviceDefectRecordDto dto)
        {
            var data = await _deviceDefectRecordService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPost("update")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Edit([FromBody] DeviceDefectRecordDto dto)
        {
            var data = await _deviceDefectRecordService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPost("delete/{ids}")]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceDefectRecordService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }

        [HttpPost("import")]
        [AppAuthorize("device:record:import")]
        public async Task Import([Required] IFormFile file)
        {
            var stream = new MemoryStream();
            file.CopyTo(stream);
            var list = await ExcelUtils.ImportAsync<DeviceDefectRecordDto>(stream);
            await _deviceDefectRecordService.ImportDtoBatchAsync(list);
        }

        [HttpPost("export")]
        [AppAuthorize("device:record:export")]
        public async Task Export(DeviceDefectRecordDto dto)
        {
            var list = await _deviceDefectRecordService.GetDtoListAsync(dto);
            await ExcelUtils.ExportAsync(App.HttpContext.Response, list);
        }
    }
}