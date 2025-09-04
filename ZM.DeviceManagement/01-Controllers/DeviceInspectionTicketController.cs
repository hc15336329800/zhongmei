using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ZM.Device.Services;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Tool;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm/deviceInspectionTicketController")]
    [Route("zm/ticket")]
    [AllowAnonymous]
    public class DeviceInspectionTicketController : ControllerBase
    {
        private readonly ILogger<DeviceInspectionTicketController> _logger;
        private readonly DeviceInspectionTicketService _deviceInspectionTicketService;
        public DeviceInspectionTicketController(ILogger<DeviceInspectionTicketController> logger, DeviceInspectionTicketService deviceInspectionTicketService)
        {
            _logger = logger;
            _deviceInspectionTicketService = deviceInspectionTicketService;
        }

        [HttpGet("list")]
        public async Task<SqlSugarPagedList<DeviceInspectionTicketDto>> GetDeviceInspectionTicketPagedList([FromQuery] DeviceInspectionTicketDto dto)
        {
            return await _deviceInspectionTicketService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("info/{id}")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _deviceInspectionTicketService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpPost("add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Add([FromBody] DeviceInspectionTicketDto dto)
        {
            if (dto == null)
            {
                return AjaxResult.Error("新增失败");
            }

            dto.Id = NextId.Id13();
            var data = await _deviceInspectionTicketService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPost("update")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Edit([FromBody] DeviceInspectionTicketDto dto)
        {
            var data = await _deviceInspectionTicketService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPost("delete/{ids}")]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceInspectionTicketService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }
    }
}