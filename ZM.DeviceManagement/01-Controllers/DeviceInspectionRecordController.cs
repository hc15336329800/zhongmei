using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuoYi.Framework;
using RuoYi.Framework.Extensions;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using ZM.Device.Services;
using ZM.Device.Tool;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm/record")]
    [Route("zm/record")]
    [AllowAnonymous]
    public class DeviceInspectionRecordController : ControllerBase
    {
        private readonly ILogger<DeviceInspectionRecordController> _logger;
        private readonly DeviceInspectionRecordService _deviceInspectionRecordService;
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository;
        private readonly DeviceInspectionTaskRepository _deviceInspectionTaskRepository;
        private readonly DeviceManagementRepository _deviceManagementRepository;
        public DeviceInspectionRecordController(ILogger<DeviceInspectionRecordController> logger, DeviceInspectionRecordService deviceInspectionRecordService, DeviceInspectionRecordRepository deviceInspectionRecordRepository, DeviceInspectionTaskRepository deviceInspectionTaskRepository, DeviceManagementRepository deviceManagementRepository)
        {
            _logger = logger;
            _deviceInspectionRecordService = deviceInspectionRecordService;
            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
            _deviceInspectionTaskRepository = deviceInspectionTaskRepository;
            _deviceManagementRepository = deviceManagementRepository;
        }

        [HttpGet("list")]
        public async Task<SqlSugarPagedList<DeviceInspectionRecordDto>> GetDeviceInspectionRecordPagedList([FromQuery] DeviceInspectionRecordDto dto)
        {
            return await _deviceInspectionRecordService.GetDtoPagedListAsync(dto);
        }

        [HttpGet("info/{id}")]
        public async Task<AjaxResult> Get(long id)
        {
            var data = await _deviceInspectionRecordService.GetDtoAsync(id);
            return AjaxResult.Success(data);
        }

        [HttpGet("getDeviceIds/{id}")]
        public async Task<AjaxResult> GetDeviceIds(long id)
        {
            var record = await _deviceInspectionRecordRepository.Repo.AsQueryable().Where(r => r.Id == id).Select(r => new { r.FinishedDeviceIds, r.InspectedDeviceIds, r.TaskId }).FirstAsync();
            if (record == null)
            {
                return AjaxResult.Error("未找到对应巡检记录");
            }

            string tataskId = record.TaskId.ToString();
            List<string> finishedDeviceIds = new();
            List<string> inspectedDeviceIds = new();
            try
            {
                if (!string.IsNullOrWhiteSpace(record.FinishedDeviceIds))
                {
                    var finishedList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(record.FinishedDeviceIds);
                    if (finishedList != null)
                    {
                        finishedDeviceIds = finishedList.SelectMany(d => d.Keys).ToList();
                    }
                }

                if (!string.IsNullOrWhiteSpace(record.InspectedDeviceIds))
                {
                    inspectedDeviceIds = JsonConvert.DeserializeObject<List<string>>(record.InspectedDeviceIds) ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                return AjaxResult.Error("设备ID解析失败: " + ex.Message);
            }

            List<DeviceManagement> devices = new();
            if (inspectedDeviceIds.Any())
            {
                devices = await _deviceManagementRepository.Repo.AsQueryable().In(d => d.Id, inspectedDeviceIds.Select(id => long.Parse(id)).ToArray()).Select(d => new DeviceManagement { Id = d.Id, Label = d.Label, }).ToListAsync();
            }

            var result = new
            {
                TataskId = tataskId,
                Devices = devices,
                FinishedDeviceIds = finishedDeviceIds
            };
            return AjaxResult.Success(result);
        }

        [HttpPost("add")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Add([FromBody] DeviceInspectionRecordDto dto)
        {
            dto.Id = NextId.Id13();
            var data = await _deviceInspectionRecordService.InsertAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPost("update")]
        [TypeFilter(typeof(RuoYi.Framework.DataValidation.DataValidationFilter))]
        public async Task<AjaxResult> Edit([FromBody] DeviceInspectionRecordDto dto)
        {
            var data = await _deviceInspectionRecordService.UpdateAsync(dto);
            return AjaxResult.Success(data);
        }

        [HttpPost("delete/{ids}")]
        public async Task<AjaxResult> Remove(string ids)
        {
            var idList = ids.SplitToList<long>();
            var data = await _deviceInspectionRecordService.DeleteAsync(idList);
            return AjaxResult.Success(data);
        }
    }
}