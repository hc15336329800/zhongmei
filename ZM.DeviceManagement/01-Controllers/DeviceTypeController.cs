using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RuoYi.Device.Entities;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm/devicetype")]
    [Route("zm/devicetype")]
    [AllowAnonymous]
    public class DeviceTypeController : ControllerBase
    {
        private readonly ILogger<DeviceTypeController> _logger;
        private readonly DeviceTypeService _deviceTypeService;
        public DeviceTypeController(ILogger<DeviceTypeController> logger, DeviceTypeService deviceTypeService)
        {
            _logger = logger;
            _deviceTypeService = deviceTypeService;
        }

        [HttpGet("info/{deptId}")]
        public async Task<AjaxResult> Get(long deptId)
        {
            var data = await _deviceTypeService.GetDtoAsync(deptId);
            return AjaxResult.Success(data);
        }

        [HttpGet("list")]
        public async Task<AjaxResult> GetSysDictTypeList([FromQuery] DeviceTypeDto dto)
        {
            List<DeviceTypeDto> data = await _deviceTypeService.GetDtoListAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<DeviceTypeDto>> GetSysDictTypePageList([FromQuery] DeviceTypeDto dto)
        {
            return await _deviceTypeService.GetDtoPagedListAsync(dto);
        }

        [HttpPost("add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Add([FromBody] DeviceTypeDto tenant)
        {
            if (!await _deviceTypeService.CheckDeptNameUniqueAsync(tenant))
            {
                return AjaxResult.Error($"新增租户'{tenant.DeptName} '失败，租户名称已存在");
            }

            var data = await _deviceTypeService.InsertDeptAsync(tenant);
            return AjaxResult.Success(data);
        }

        [HttpGet("delete/{deptId}")]
        public async Task<AjaxResult> Remove(long deptId)
        {
            if (await _deviceTypeService.HasChildByDeptIdAsync(deptId))
            {
                return AjaxResult.Error("存在下级租户,不允许删除");
            }

            if (await _deviceTypeService.CheckDeptExistUserAsync(deptId))
            {
                return AjaxResult.Error("租户存在用户,不允许删除");
            }

            var data = await _deviceTypeService.DeleteDeptByIdAsync(deptId);
            return AjaxResult.Success(data);
        }

        [HttpPost("update")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Edit([FromBody] DeviceTypeDto dt)
        {
            long deptId = dt.Id;
            if (!await _deviceTypeService.CheckDeptNameUniqueAsync(dt))
            {
                return AjaxResult.Error("修改租户'" + dt.DeptName + "'失败，租户名称已存在");
            }
            else if (dt.ParentId.Equals(deptId))
            {
                return AjaxResult.Error("修改租户'" + dt.DeptName + "'失败，上级租户不能是自己");
            }

            var data = await _deviceTypeService.UpdateDeptAsync(dt);
            return AjaxResult.Success(data);
        }

        [HttpGet("list/exclude/{deptId}")]
        public async Task<AjaxResult> ExcludeChildList(long? deptId)
        {
            var list = await _deviceTypeService.GetDtoListAsync(new DeviceTypeDto());
            var id = deptId ?? 0;
            var data = list.Where(d => d.Id != id || (!d.Ancestors?.Split(",").Contains(id.ToString()) ?? false)).ToList();
            return AjaxResult.Success(data);
        }

        [HttpGet("getTreeNode")]
        public async Task<AjaxResult> getTreeNode([FromQuery] DeviceTypeDto dto)
        {
            var data = await _deviceTypeService.GetTreeNodeAsync(dto);
            return AjaxResult.Success(data);
        }
    }
}