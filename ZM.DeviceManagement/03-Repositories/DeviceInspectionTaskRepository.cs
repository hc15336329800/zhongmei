using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using RuoYi.Data.Entities;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    public class DeviceInspectionTaskRepository : BaseRepository<DeviceInspectionTask, DeviceInspectionTaskDto>
    {
        public DeviceInspectionTaskRepository(ISqlSugarRepository<DeviceInspectionTask> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceInspectionTask> Queryable(DeviceInspectionTaskDto dto)
        {
            var par = Repo.AsQueryable().WhereIF(!string.IsNullOrWhiteSpace(dto.TaskName), it => it.TaskName.Contains(dto.TaskName)).WhereIF(!string.IsNullOrWhiteSpace(dto.TaskStatus), it => it.TaskStatus.Contains(dto.TaskStatus)).WhereIF(!string.IsNullOrWhiteSpace(dto.TaskType), it => it.TaskType == dto.TaskType);
            return par;
        }

        public override ISugarQueryable<DeviceInspectionTaskDto> DtoQueryable(DeviceInspectionTaskDto dto)
        {
            var par = Repo.AsQueryable().WhereIF(!string.IsNullOrWhiteSpace(dto.TaskName), it => it.TaskName.Contains(dto.TaskName)).WhereIF(!string.IsNullOrWhiteSpace(dto.TaskType), it => it.TaskType == dto.TaskType).WhereIF(!string.IsNullOrWhiteSpace(dto.TaskStatus), it => it.TaskStatus.Contains(dto.TaskStatus)).Select(it => new DeviceInspectionTaskDto { Id = it.Id, TaskName = it.TaskName, TicketId = it.TicketId, LeaderId = it.Leader, ExecutorId = it.Executor, TaskType = it.TaskType, TaskStatus = it.TaskStatus, CheckInDeviation = it.CheckInDeviation, PlanStartTime = it.PlanStartTime, PlanEndTime = it.PlanEndTime, ActualEndTime = it.ActualEndTime, DeviceIds = it.DeviceIds, Remark = it.Remark, CreateBy = it.CreateBy, CreateTime = it.CreateTime, UpdateBy = it.UpdateBy, UpdateTime = it.UpdateTime });
            return par;
        }

        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListOrderedByMaintenanceCycleAppAsync(DeviceInspectionTaskDto dto)
        {
            var query = DtoQueryable(dto);
            var currentUser = SecurityUtils.GetLoginUser()?.User?.UserName;
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                query = query.Where(it => it.Executor == currentUser);
            }

            if (string.IsNullOrWhiteSpace(dto.TaskStatus))
            {
                query = query.Where(it => new[] { "待办", "在办" }.Contains(it.TaskStatus));
            }

            var pagedInfo = await query.OrderBy(t => t.CreateTime, OrderByType.Desc).ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
            return await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
        }

        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListOrderedByMaintenanceCycleAppWxAsync(DeviceInspectionTaskDto dto)
        {
            var query = DtoQueryable(dto);
            var currentUser = SecurityUtils.GetLoginUser()?.User?.UserName;
            if (!string.IsNullOrWhiteSpace(currentUser))
            {
                query = query.Where(it => it.Executor == currentUser);
            }

            if (string.IsNullOrWhiteSpace(dto.TaskStatus))
            {
                query = query.Where(it => new[] { "待办", "在办" }.Contains(it.TaskStatus));
            }

            var pagedList = await query.OrderBy(t => t.CreateTime, OrderByType.Desc).ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
            var firstDeviceIdList = new List<long>();
            foreach (var item in pagedList.Rows)
            {
                try
                {
                    var ids = JsonConvert.DeserializeObject<List<long>>(item.DeviceIds ?? "[]");
                    if (ids != null && ids.Count > 0)
                    {
                        item.DevId = ids[0];
                        firstDeviceIdList.Add(ids[0]);
                    }
                }
                catch
                {
                    item.DevId = 0;
                }
            }

            if (firstDeviceIdList.Count > 0)
            {
                var deviceDict = await Repo.Context.Queryable<DeviceManagement>().Where(d => firstDeviceIdList.Contains(d.Id)).ToDictionaryAsync(d => d.Id, d => d.Label ?? "");
                foreach (var item in pagedList.Rows)
                {
                    if (deviceDict.TryGetValue(item.DevId.ToString(), out var name))
                    {
                        item.DevName = name.ToString();
                    }
                    else
                    {
                        item.DevName = "未知设备";
                    }
                }
            }

            return pagedList;
        }

        public async Task<SqlSugarPagedList<DeviceInspectionTaskDto>> GetDtoPagedListOrderedByMaintenanceCycleAsync(DeviceInspectionTaskDto dto)
        {
            var pagedInfo = await DtoQueryable(dto).OrderBy(t => t.CreateTime, OrderByType.Desc).ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
            pagedInfo.Code = StatusCodes.Status200OK;
            var userList = (await Repo.Context.Queryable<SysUser>().Where(u => u.DelFlag == "0").ToDictionaryAsync(u => u.UserId.ToString(), u => u.NickName)).ToDictionary(k => k.Key, v => v.Value?.ToString() ?? "");
            foreach (var item in pagedInfo.Rows)
            {
                if (!string.IsNullOrEmpty(item.LeaderId) && userList.ContainsKey(item.LeaderId))
                {
                    item.Leader = userList[item.LeaderId];
                }
                else
                {
                    item.Leader = item.LeaderId;
                }

                if (!string.IsNullOrEmpty(item.ExecutorId) && userList.ContainsKey(item.ExecutorId))
                {
                    item.Executor = userList[item.ExecutorId];
                }
                else
                {
                    item.Executor = item.ExecutorId;
                }
            }

            return pagedInfo;
        }
    }
}