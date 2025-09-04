using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RuoYi.Common.Utils;
using RuoYi.Framework;
using SqlSugar;
using ZM.Device._05_Dtos;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using ZM.Device.Services;

namespace ZM.Device.Controllers
{
    [ApiDescriptionSettings("zm/deviceInspectionTask")]
    [Route("zm/deviceInspectionTask")]
    [AllowAnonymous]
    public class DeviceInspectionTaskController : ControllerBase
    {
        private readonly ILogger<DeviceInspectionTaskController> _logger;
        private readonly DeviceInspectionTaskService _service;
        private readonly DeviceInspectionTaskRepository _deviceInspectionTaskRepository;
        private readonly DeviceManagementRepository _deviceManagementRepository;
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository;
        private readonly DeviceDefectRecordRepository _deviceDefectRecordRepository;
        public DeviceInspectionTaskController(ILogger<DeviceInspectionTaskController> logger, DeviceInspectionTaskService service, DeviceInspectionTaskRepository deviceInspectionTaskRepository, DeviceManagementRepository deviceManagementRepository, DeviceInspectionRecordRepository deviceInspectionRecordRepository, DeviceDefectRecordRepository deviceDefectRecordRepository)
        {
            _logger = logger;
            _service = service;
            _deviceInspectionTaskRepository = deviceInspectionTaskRepository;
            _deviceManagementRepository = deviceManagementRepository;
            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
            _deviceDefectRecordRepository = deviceDefectRecordRepository;
        }

 
        [HttpPost("submitDeviceWxStatus")]
        public async Task<AjaxResult> SubmitDeviceWxStatus([FromBody] SubmitDeviceStatusDto dto)
        {
            return await _service.SubmitDeviceWxStatusAsync(dto);
        }

        [HttpPost("submitDeviceQxStatus")]
        public async Task<AjaxResult> SubmitDeviceQxStatus([FromBody] SubmitDeviceStatusDto dto)
        {
            return await _service.SubmitDeviceQxStatusAsync(dto);
        }

        [HttpPost("submitDeviceStatus")]
        public async Task<AjaxResult> SubmitDeviceStatus([FromBody] SubmitDeviceStatusDto dto)
        {
            return await _service.SubmitDeviceStatusAsync(dto);
        }

        [HttpGet("getFinishedDevices")]
        public async Task<AjaxResult> GetFinishedDevices(long taskId)
        {
            if (taskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");
            var sql = @"   SELECT finished_device_ids
                                FROM device_inspection_record
                                WHERE task_id = @taskId
                                ORDER BY create_time DESC  
                                LIMIT 1 ";
            var finishedJson = await _deviceInspectionRecordRepository.Repo.Context.Ado.GetStringAsync(sql, new { taskId });
            var finishedList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(finishedJson ?? "[]");
            return AjaxResult.Success(finishedList);
        }

        [HttpGet("getDefectDetail")]
        public async Task<AjaxResult> GetDefectDetail(long taskId, long deviceId)
        {
            if (taskId <= 0 || deviceId <= 0)
                return AjaxResult.Error("参数无效");
            var data = await _deviceDefectRecordRepository.Repo.AsQueryable().Where(d => d.TaskId == taskId && d.DeviceId == deviceId).FirstAsync();
            return AjaxResult.Success(data);
        }

        [HttpGet("getDefectDetailDevice")]
        public async Task<AjaxResult> getDefectDetailDevice(long taskId, long deviceId)
        {
            if (taskId <= 0 || deviceId <= 0)
                return AjaxResult.Error("参数无效");
            var taskDto = await _deviceDefectRecordRepository.Repo.AsQueryable().Where(d => d.TaskId == taskId && d.DeviceId == deviceId).Select(t => new DeviceDefectRecordDto { Id = t.Id, DefectDesc = t.DefectDesc, DefectName = t.DefectName, DeviceName = t.DeviceName, Remark = t.Remark, ImageUrl = t.ImageUrl, }).FirstAsync();
            var deviceDto = await _deviceManagementRepository.Repo.AsQueryable().Where(d => d.Id == deviceId).Select(d => new DeviceManagementDto { Id = d.Id, Model = d.Model, Label = d.Label, RatedVoltage = d.RatedVoltage, RatedCurrent = d.RatedCurrent, }).FirstAsync();
            var result = new DeviceTaskDetailDto
            {
                TaskInfo = taskDto,
                DeviceInfo = deviceDto
            };
            return AjaxResult.Success(result);
        }

 
        [HttpPost("uploadImage")]
        [RequestSizeLimit(3 * 1024 * 1024)]
        public async Task<AjaxResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return AjaxResult.Error("上传文件不能为空");
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                return AjaxResult.Error("仅支持 JPG/PNG 格式");
            var dateDir = DateTime.Now.ToString("yyyyMMdd");
            var folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", dateDir);
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("📦 图片已保存到本地路径：{0}", fullPath);
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var accessPath = $"/uploads/{dateDir}/{fileName}";
            var url = baseUrl + accessPath;
            return AjaxResult.Success(data: url);
        }

        [HttpGet("getDevicesByTaskId")]
        public async Task<AjaxResult> GetDevicesByTaskId(long taskId)
        {
            var sql = @"SELECT device_ids FROM device_inspection_task WHERE id = @taskId";
            var deviceIdJson = await _deviceInspectionTaskRepository.Repo.Context.Ado.GetStringAsync(sql, new { taskId });
            var idList = JsonConvert.DeserializeObject<List<long>>(deviceIdJson ?? "[]");
            var deviceList = await _deviceManagementRepository.Repo.Context.Queryable<DeviceManagement>().Where(it => idList.Contains(it.Id)).Select(it => new DeviceManagementDto { Id = it.Id, Label = it.Label, Model = it.Model, RatedCurrent = it.RatedCurrent, RatedVoltage = it.RatedVoltage }).ToListAsync();
            return AjaxResult.Success(deviceList);
        }

        [HttpGet("pagelist")]
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetPageList([FromQuery] DeviceInspectionTaskDto dto)
        {
            return await _service.GetDtoPagedListDescAsync(dto);
        }

        [HttpGet("pageListApp")]
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetPageListApp([FromQuery] DeviceInspectionTaskDto dto)
        {
            return await _service.GetDtoPagedListDescAsyncApp(dto);
        }

        [HttpGet("pageListWxApp")]
        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetPageListWxApp([FromQuery] DeviceInspectionTaskDto dto)
        {
            return await _service.GetDtoPagedListDescAsyncWxApp(dto);
        }

        [HttpGet("pageListAppWx")]
        public async Task<SqlSugarPagedList<DeviceDefectRecordDto>> GetPageListAppWx([FromQuery] DeviceDefectRecordDto dto)
        {
            var query = _deviceDefectRecordRepository.Repo.AsQueryable().Where(d => new[] { "待办", "在办" }.Contains(d.DefectStatus));
            if (!string.IsNullOrWhiteSpace(dto.DefectCategory))
            {
                query = query.Where(d => d.DefectCategory == dto.DefectCategory);
            }

            return await query.OrderBy(d => d.CreateTime, OrderByType.Desc).Select(d => new DeviceDefectRecordDto { Id = d.Id, TaskId = d.TaskId, DeviceId = d.DeviceId, DeviceName = d.DeviceName, DefectName = d.DefectName, DefectStatus = d.DefectStatus, DefectCategory = d.DefectCategory, Remark = d.Remark, CreateTime = d.CreateTime }).ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
        }

        [HttpPost("add")]
        public async Task<AjaxResult> Add([FromBody] DeviceInspectionTaskDto dto)
        {
            return await _service.AddWithRecordAsync(dto);
        }

        [HttpPost("addQx")]
        public async Task<AjaxResult> AddQx([FromBody] DeviceInspectionTaskDto dto)
        {
            return await _service.AddWithRecordQxAsync(dto);
        }

        [HttpPost("add1")]
        public async Task<IActionResult> Add()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();
            Console.WriteLine("【原始请求体】：" + rawBody);
            return Ok(new { msg = "收到原始数据", body = rawBody });
        }

        [HttpPost("update")]
        public async Task<AjaxResult> Update([FromBody] DeviceInspectionTaskDto dto)
        {
            var rows = await _service.UpdateAsync(dto);
            return rows > 0 ? AjaxResult.Success("更新成功") : AjaxResult.Error("更新失败");
        }

        [HttpPost("delete")]
        public async Task<AjaxResult> Delete([FromBody] List<long> ids)
        {
            if (ids == null || !ids.Any())
                return AjaxResult.Error("未指定删除数据");
            await _service.DeleteAsync(ids);
            return AjaxResult.Success("删除成功");
        }
 
    }
}

public class DeviceTaskDetailDto
{
    public DeviceDefectRecordDto? TaskInfo { get; set; }
    public DeviceManagementDto? DeviceInfo { get; set; }
}