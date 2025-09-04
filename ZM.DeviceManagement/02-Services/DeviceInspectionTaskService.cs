using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using RuoYi.Common.Utils;
using Newtonsoft.Json;
using RuoYi.Framework.Interceptors;
using RuoYi.Framework;
using ZM.Device.Tool;
using ZM.Device._05_Dtos;
using UAParser.Objects;
using Newtonsoft.Json.Linq;
using RuoYi.Data.Entities;

namespace ZM.Device.Services
{
    public class DeviceInspectionTaskService : BaseService<DeviceInspectionTask, DeviceInspectionTaskDto>
    {
        private readonly ILogger<DeviceInspectionTaskService> _logger;
        private readonly DeviceInspectionTaskRepository _repo;
        private readonly DeviceInspectionRecordRepository _deviceInspectionRecordRepository;
        private readonly DeviceInspectionTicketRepository _ticketRepo;
        private readonly DeviceDefectRecordRepository _defectRepo;
        private readonly DeviceInspectionRecordRepository _recordRepo;
        private readonly DeviceManagementRepository _deviceManagementRepo;
        public DeviceInspectionTaskService(ILogger<DeviceInspectionTaskService> logger, DeviceInspectionTaskRepository repo, DeviceInspectionRecordRepository deviceInspectionRecordRepository, DeviceInspectionTicketRepository ticketRepo, DeviceDefectRecordRepository defectRepo, DeviceInspectionRecordRepository recordRepo, DeviceManagementRepository deviceManagementRepo)
        {
            _logger = logger;
            _repo = repo;
            _deviceInspectionRecordRepository = deviceInspectionRecordRepository;
            _ticketRepo = ticketRepo;
            _recordRepo = recordRepo;
            _defectRepo = defectRepo;
            _deviceManagementRepo = deviceManagementRepo;
            BaseRepo = repo;
        }

        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListDescAsync(DeviceInspectionTaskDto dto)
        {
            return await _repo.GetDtoPagedListOrderedByMaintenanceCycleAsync(dto);
        }

        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListDescAsyncApp(DeviceInspectionTaskDto dto)
        {
            dto.TaskStatus = null;
            return await _repo.GetDtoPagedListOrderedByMaintenanceCycleAppAsync(dto);
        }

        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListDescAsyncWxApp(DeviceInspectionTaskDto dto)
        {
            dto.TaskStatus = null;
            return await _repo.GetDtoPagedListOrderedByMaintenanceCycleAppWxAsync(dto);
        }

        [Transactional]
        public virtual async Task<AjaxResult> AddWithRecordAsync(DeviceInspectionTaskDto dto)
        {
            if (dto == null)
                return AjaxResult.Error("参数不可为空");
            var allowedTypes = new[]
            {
                "巡检",
                "维修",
                "抢修"
            };
            if (!allowedTypes.Contains(dto.TaskType))
                return AjaxResult.Error($"无效的任务类型：{dto.TaskType}");
            string deviceIdsJson = "[]";
            if (dto.TaskType == "巡检")
            {
                var ticket = await _ticketRepo.FirstOrDefaultAsync(t => t.Id == dto.TicketId);
                if (ticket == null)
                    return AjaxResult.Error("操作票不存在");
                deviceIdsJson = ticket.DeviceIds ?? "[]";
            }
            else
            {
                if (dto.DeviceId <= 0 || string.IsNullOrWhiteSpace(dto.DeviceName.ToString()))
                    return AjaxResult.Error("维修/抢修任务必须指定设备及名称");
                long deviceIdStr = dto.DeviceId ?? 0;
                deviceIdsJson = JsonConvert.SerializeObject(new List<long> { deviceIdStr });
            }

            dto.Id = NextId.Id13();
            dto.TaskStatus = "待办";
            dto.DeviceIds = deviceIdsJson;
            var taskInserted = await _repo.InsertAsync(dto);
            if (!taskInserted)
                return AjaxResult.Error("新增任务失败");
            if (dto.TaskType == "巡检")
            {
                var record = new DeviceInspectionRecord
                {
                    Id = NextId.Id13(),
                    TaskId = dto.Id,
                    TaskName = dto.TaskName,
                    InspectedDeviceIds = deviceIdsJson,
                    FinishedDeviceIds = "[]",
                    DeviceCount = JsonConvert.DeserializeObject<List<long>>(deviceIdsJson).Count,
                    DefectTotal = 0,
                    DefectProcessed = 0,
                    Leader = dto.Leader,
                    Executor = dto.Executor,
                    TaskType = dto.TaskType,
                    TaskStatus = "待办",
                    TaskStartTime = DateTime.Now
                };
                var recordInserted = await _deviceInspectionRecordRepository.InsertAsync(record);
                if (!recordInserted)
                    return AjaxResult.Error("新增记录失败");
            }
            else
            {
                var record = new DeviceDefectRecord
                {
                    Id = NextId.Id13(),
                    TaskId = dto.Id,
                    DeviceId = dto.DeviceId ?? 0,
                    DeviceName = dto.DeviceName.ToString(),
                    DefectName = dto.TaskName,
                    DefectDesc = dto.Remark,
                    DefectStatus = "待办",
                    DefectCategory = dto.TaskType,
                    DiscoveryTime = DateTime.Now,
                    SeverityLevel = "一般",
                    FixPerson = dto.Executor
                };
                var recordInserted = await _defectRepo.InsertAsync(record);
                if (!recordInserted)
                    return AjaxResult.Error("新增记录失败");
            }

            return AjaxResult.Success("新增成功");
        }

        [Transactional]
        public virtual async Task<AjaxResult> AddWithRecordQxAsync(DeviceInspectionTaskDto dto)
        {
            if (dto == null)
                return AjaxResult.Error("参数不可为空");
            if (string.IsNullOrWhiteSpace(dto.DeviceIds))
                return AjaxResult.Error("设备ID不能为空");
            List<long> Dids;
            try
            {
                Dids = JsonConvert.DeserializeObject<List<long>>(dto.DeviceIds);
            }
            catch (Exception ex)
            {
                return AjaxResult.Error("设备ID格式非法，需为数字数组字符串，如：[123456]");
            }

            if (Dids == null || !Dids.Any())
                return AjaxResult.Error("设备ID为空，解析失败");
            long deviceIdss = Dids.First();
            if (dto.RelatedTaskId > 0)
            {
                var existDefect = await _defectRepo.Repo.AsQueryable().Where(d => d.TaskId == dto.RelatedTaskId && d.DeviceId == deviceIdss && d.DefectCategory == "缺陷").FirstAsync();
                if (existDefect != null && !string.IsNullOrWhiteSpace(existDefect.DevicePath))
                {
                    return AjaxResult.Error("该缺陷已提交维修任务，不能重复操作");
                }
            }

            var ids = JsonConvert.DeserializeObject<List<long>>(dto.DeviceIds ?? "[]");
            long did = ids.FirstOrDefault();
            dto.Id = NextId.Id13();
            dto.TicketId = 0;
            dto.TaskType = "维修";
            dto.TaskStatus = "待办";
            dto.DeviceIds = dto.DeviceIds;
            dto.Leader = dto.Leader;
            dto.Executor = dto.Executor;
            var taskInserted = await _repo.InsertAsync(dto);
            if (!taskInserted)
                return AjaxResult.Error("新增任务失败");
            string deviceName = "未知设备";
            if (did > 0)
            {
                var device = await _deviceManagementRepo.FirstOrDefaultAsync(dm => dm.Id == did);
                if (device != null && !string.IsNullOrEmpty(device.Label))
                {
                    deviceName = device.Label;
                }
            }

            var record = new DeviceDefectRecord
            {
                Id = NextId.Id13(),
                TaskId = dto.Id,
                DeviceId = did,
                DeviceName = deviceName,
                DefectName = dto.TaskName,
                DefectDesc = dto.Remark,
                DefectStatus = "待办",
                DefectCategory = "维修",
                DiscoveryTime = DateTime.Now,
                SeverityLevel = "一般",
                ImageUrl = dto.ImageUrl,
                FixPerson = dto.Executor,
                CreateBy = SecurityUtils.GetLoginUser().User.UserName
            };
            var recordInserted = await _defectRepo.InsertAsync(record);
            if (!recordInserted)
                return AjaxResult.Error("新增记录失败");
            if (dto.RelatedTaskId > 0)
            {
                var updatedRows = await _defectRepo.Repo.Context.Updateable<DeviceDefectRecord>().SetColumns(d => new DeviceDefectRecord { DevicePath = dto.Id.ToString(), UpdateBy = SecurityUtils.GetLoginUser().User.UserName, UpdateTime = DateTime.Now }).Where(d => d.TaskId == dto.RelatedTaskId && d.DefectCategory == "缺陷").ExecuteCommandAsync();
                if (updatedRows == 0)
                {
                    _logger.LogWarning($"未找到原缺陷记录进行更新，RelatedTaskId={dto.RelatedTaskId}");
                }
            }

            return AjaxResult.Success("新增成功");
        }

        [Transactional]
        public virtual async Task<AjaxResult> SubmitDeviceStatusAsync(SubmitDeviceStatusDto dto)
        {
            if (dto.TaskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");
            if (dto.Id <= 0)
                return AjaxResult.Error("参数 id 不可为空或无效");
            if (string.IsNullOrWhiteSpace(dto.DevStatus))
                return AjaxResult.Error("参数 devStatus 不可为空");
            if (dto.DevStatus == "false" && string.IsNullOrWhiteSpace(dto.Remark))
                return AjaxResult.Error("参数 remark 不可为空");
            var record = await _recordRepo.Repo.Context.Queryable<DeviceInspectionRecord>().Where(r => r.TaskId == dto.TaskId).FirstAsync();
            if (record == null)
                return AjaxResult.Error("未找到对应的巡检记录");
            var finishedJson = record.FinishedDeviceIds ?? "[]";
            var finishedList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(finishedJson) ?? new List<Dictionary<string, string>>();
            var key = dto.Id.ToString();
            var entry = finishedList.FirstOrDefault(m => m.ContainsKey(key));
            if (entry != null)
                entry[key] = dto.DevStatus.ToLower();
            else
                finishedList.Add(new Dictionary<string, string> { { key, dto.DevStatus.ToLower() } });
            record.FinishedDeviceIds = JsonConvert.SerializeObject(finishedList);
            await _recordRepo.UpdateAsync(record);
            if (dto.DevStatus == "false")
            {
                string deviceName = "未知设备";
                var device = await _deviceManagementRepo.FirstOrDefaultAsync(d => d.Id == dto.Id);
                if (device != null && !string.IsNullOrWhiteSpace(device.Label))
                {
                    deviceName = device.Label;
                }

                var defect = new DeviceDefectRecord
                {
                    Id = NextId.Id13(),
                    TaskId = dto.TaskId,
                    DeviceId = dto.Id,
                    DeviceName = deviceName,
                    DefectName = record.TaskName,
                    DefectDesc = dto.Remark,
                    ImageUrl = dto.ImageUrl,
                    DiscoveryTime = DateTime.Now,
                    CreateTime = DateTime.Now,
                    CreateBy = SecurityUtils.GetUsername(),
                    DefectStatus = "待办",
                    DefectCategory = "缺陷"
                };
                await _defectRepo.InsertAsync(defect);
            }
            else
            {
                await _defectRepo.Repo.DeleteAsync(d => d.TaskId == dto.TaskId && d.DeviceId == dto.Id);
            }

            if (dto.IsCompleted)
            {
                record.TaskStatus = "办毕";
                record.TaskEndTime = DateTime.Now;
                await _deviceInspectionRecordRepository.UpdateAsync(record);
                await _repo.Repo.Context.Updateable<DeviceInspectionTask>().SetColumns(t => new DeviceInspectionTask { TaskStatus = record.TaskStatus, ActualEndTime = DateTime.Now }).Where(t => t.Id == dto.TaskId).ExecuteCommandAsync();
            }
            else
            {
                record.TaskStatus = "在办";
                await _deviceInspectionRecordRepository.UpdateAsync(record);
                await _repo.Repo.Context.Updateable<DeviceInspectionTask>().SetColumns(t => new DeviceInspectionTask { TaskStatus = record.TaskStatus }).Where(t => t.Id == dto.TaskId).ExecuteCommandAsync();
            }

            return AjaxResult.Success("设备状态更新成功");
        }

        [Transactional]
        public virtual async Task<AjaxResult> SubmitDeviceWxStatusAsync(SubmitDeviceStatusDto dto)
        {
            if (dto.TaskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");
            if (dto.Id <= 0)
                return AjaxResult.Error("参数 id 不可为空或无效");
            if (string.IsNullOrWhiteSpace(dto.DevStatus))
                return AjaxResult.Error("参数 devStatus 不可为空");
            var defect = await _defectRepo.Repo.AsQueryable().Where(d => d.TaskId == dto.TaskId && d.DefectCategory == "维修").FirstAsync();
            if (defect == null)
                return AjaxResult.Error("未找到维修记录");
            defect.Remark = dto.Remark;
            defect.ImageUrl = dto.ImageUrl;
            defect.FixTime = DateTime.Now;
            defect.DefectStatus = "办毕";
            defect.FixPerson = SecurityUtils.GetLoginUser().User.UserName;
            defect.UpdateTime = DateTime.Now;
            defect.UpdateBy = SecurityUtils.GetLoginUser().User.UserName;
            await _defectRepo.UpdateAsync(defect);
            var defectQx = await _defectRepo.Repo.AsQueryable().Where(d => d.DevicePath == dto.TaskId.ToString() && d.DefectCategory == "缺陷").FirstAsync();
            if (defectQx != null)
            {
                defectQx.DefectStatus = "办毕";
                defectQx.UpdateTime = DateTime.Now;
                defectQx.UpdateBy = SecurityUtils.GetLoginUser().User.UserName;
                defectQx.FixPerson = SecurityUtils.GetLoginUser().User.UserName;
                defectQx.FixTime = DateTime.Now;
                await _defectRepo.UpdateAsync(defectQx);
            }

            await _repo.Repo.Context.Updateable<DeviceInspectionTask>().SetColumns(t => new DeviceInspectionTask { TaskStatus = "办毕", ActualEndTime = DateTime.Now, UpdateBy = SecurityUtils.GetUsername(), UpdateTime = DateTime.Now }).Where(t => t.Id == dto.TaskId).ExecuteCommandAsync();
            return AjaxResult.Success("维修记录提交成功");
        }

        [Transactional]
        public virtual async Task<AjaxResult> SubmitDeviceQxStatusAsync(SubmitDeviceStatusDto dto)
        {
            if (dto.TaskId <= 0)
                return AjaxResult.Error("参数 taskId 不可为空或无效");
            if (dto.Id <= 0)
                return AjaxResult.Error("参数 id 不可为空或无效");
            if (string.IsNullOrWhiteSpace(dto.DevStatus))
                return AjaxResult.Error("参数 devStatus 不可为空");
            var defectQx = await _defectRepo.Repo.AsQueryable().Where(d => d.TaskId == dto.TaskId && d.DefectCategory == "抢修").FirstAsync();
            if (defectQx != null)
            {
                defectQx.ImageUrl = dto.ImageUrl;
                defectQx.Remark = dto.Remark;
                defectQx.DefectStatus = "办毕";
                defectQx.UpdateTime = DateTime.Now;
                defectQx.UpdateBy = SecurityUtils.GetLoginUser().User.UserName;
                defectQx.FixPerson = SecurityUtils.GetLoginUser().User.UserName;
                defectQx.FixTime = DateTime.Now;
                await _defectRepo.UpdateAsync(defectQx);
            }

            await _repo.Repo.Context.Updateable<DeviceInspectionTask>().SetColumns(t => new DeviceInspectionTask { TaskStatus = "办毕", ActualEndTime = DateTime.Now, UpdateBy = SecurityUtils.GetUsername(), UpdateTime = DateTime.Now }).Where(t => t.Id == dto.TaskId).ExecuteCommandAsync();
            return AjaxResult.Success("维修记录提交成功");
        }
    }
}