using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Enums;
using RuoYi.Common.Utils;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm.deviceMaint")]
    [Route("zm/deviceMaint")]
    [AllowAnonymous]
    public class DeviceMaintController : ControllerBase
    {
        private readonly ILogger<DeviceMaintController> _logger;
        private readonly DeviceMaintService _deviceMaintService;
        private readonly DeviceManagementService _deviceManagementService;
        public DeviceMaintController(ILogger<DeviceMaintController> logger, DeviceMaintService deviceMaintService, DeviceManagementService deviceManagementService)
        {
            _logger = logger;
            _deviceMaintService = deviceMaintService;
            _deviceManagementService = deviceManagementService;
        }

        [HttpGet("getDetailById")]
        public async Task<AjaxResult> GetDetailById([FromQuery] long deviceId)
        {
            if (deviceId <= 0)
                return AjaxResult.Error("参数无效");
            var device = await _deviceManagementService.FirstOrDefaultAsync(d => d.Id == deviceId);
            if (device == null)
                return AjaxResult.Error("设备不存在");
            var pageList = await _deviceMaintService.GetDeviceMaintPagedListById(deviceId);
            return AjaxResult.Success(new { DeviceInfo = device, MaintList = pageList });
        }

        [HttpGet("getById")]
        public async Task<AjaxResult> GetById([FromQuery] long id)
        {
            if (id <= 0)
                return AjaxResult.Error("参数无效");
            var result = await _deviceManagementService.FirstOrDefaultAsync(d => d.Id == id);
            return result != null ? AjaxResult.Success(result) : AjaxResult.Error("设备不存在");
        }

        [HttpGet("listbyid")]
        public async Task<SqlSugarPagedList<DeviceMaintDto>> GetDeviceMaintPagedListById(string par)
        {
            long id = long.Parse(par);
            return await _deviceMaintService.GetDeviceMaintPagedListById(id);
        }

        [HttpPost("add")]
        public async Task<AjaxResult> AddWithRelation([FromBody] DeviceMaintDto dto)
        {
            var success = await _deviceMaintService.AddWithRelationAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }

        [HttpPost("edit")]
        public async Task<AjaxResult> EditWithDateLimit([FromBody] DeviceMaintDto dto)
        {
            var result = await _deviceMaintService.UpdateWithDateLimitAsync(dto);
            return result;
        }

        [HttpGet("delete")]
        public async Task<AjaxResult> DeleteWithRelation(long id)
        {
            var success = await _deviceMaintService.DeleteWithRelationAsync(id);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }

        [HttpGet("list")]
        public async Task<SqlSugarPagedList<DeviceMaintDto>> GetDeviceMaintPagedList([FromQuery] DeviceMaintDto dto)
        {
            return await _deviceMaintService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("")]
        [HttpGet("{id}")]
        [AppAuthorize("zM.Device:deviceMaint:query")]
        public async Task<AjaxResult> Get(int id)
        {
            var data = await _deviceMaintService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpPut("")]
        [AppAuthorize("zM.Device:deviceMaint:edit")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        [RuoYi.System.Log(Title = "保养记录表", BusinessType = BusinessType.UPDATE)]
        public async Task<AjaxResult> Edit([FromBody] DeviceMaintDto dto)
        {
            var data = await _deviceMaintService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpDelete("{ids}")]
        [AppAuthorize("zM.Device:deviceMaint:remove")]
        [RuoYi.System.Log(Title = "保养记录表", BusinessType = BusinessType.DELETE)]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceMaintService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }
    }
}