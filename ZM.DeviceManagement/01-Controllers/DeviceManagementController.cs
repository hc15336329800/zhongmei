using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Device.Entities;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device.Entities;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm.deviceManagement")]
    [Route("zm/deviceManagement")]
    public class DeviceManagementController : ControllerBase
    {
        private readonly ILogger<DeviceManagementController> _logger;
        private readonly DeviceManagementService _deviceManagementService;
        public DeviceManagementController(ILogger<DeviceManagementController> logger, DeviceManagementService deviceManagementService)
        {
            _logger = logger;
            _deviceManagementService = deviceManagementService;
        }

        [HttpGet("pagelistById")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetTypePageList([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagementService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> pagelistById([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagementService.GetDtoPagedListOrderedByMaintenanceCycleIdAsync(dto);
        }

        [HttpGet("pagelistByMaint")]
        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetMaintPageList([FromQuery] DeviceManagementDto dto)
        {
            return await _deviceManagementService.GetDtoPagedListOrderedByMaintenanceCycleAsync(dto);
        }

        [HttpGet("pagelistbytype")]
        public async Task<SqlSugarPagedList<DeviceManagement>> GetPageByDeviceType([FromQuery] long deviceTypeId, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 100)
        {
            return await _deviceManagementService.GetPagedListByDeviceTypeAsync(deviceTypeId, pageIndex, pageSize);
        }

        [HttpGet("getById")]
        public async Task<AjaxResult> GetById([FromQuery] long id)
        {
            if (id <= 0)
                return AjaxResult.Error("参数无效");
            var result = await _deviceManagementService.FirstOrDefaultAsync(d => d.Id == id);
            return result != null ? AjaxResult.Success(result) : AjaxResult.Error("设备不存在");
        }

        [HttpPost("listByIds")]
        public async Task<List<DeviceManagementDto>> GetListByDeviceIds([FromBody] List<string> Ids)
        {
            if (Ids == null || Ids.Count == 0)
            {
                return new List<DeviceManagementDto>();
            }

            List<long> longDeviceIds = Ids.Select(id => Convert.ToInt64(id)).ToList();
            return await _deviceManagementService.GetListByDeviceIdsAsync(longDeviceIds);
        }

        [HttpPost("add")]
        public async Task<AjaxResult> AddDeviceWithType([FromBody] DeviceManagementDto dto)
        {
            bool success = await _deviceManagementService.AddDeviceWithTypeAsync(dto);
            return success ? AjaxResult.Success("新增成功") : AjaxResult.Error("新增失败");
        }

        [HttpPost("update")]
        public async Task<AjaxResult> UpdateDeviceWithType([FromBody] DeviceManagementDto dto)
        {
            bool success = await _deviceManagementService.UpdateDeviceWithTypeAsync(dto);
            return success ? AjaxResult.Success("更新成功") : AjaxResult.Error("更新失败");
        }

        [HttpPost("delete")]
        public async Task<AjaxResult> DeleteDeviceWithType([FromBody] long deviceId)
        {
            bool success = await _deviceManagementService.DeleteDeviceWithTypeAsync(deviceId);
            return success ? AjaxResult.Success("删除成功") : AjaxResult.Error("删除失败");
        }
    }
}