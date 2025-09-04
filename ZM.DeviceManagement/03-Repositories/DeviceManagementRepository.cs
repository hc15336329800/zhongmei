using Microsoft.AspNetCore.Http;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using SqlSugar;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    public class DeviceManagementRepository : BaseRepository<DeviceManagement, DeviceManagementDto>
    {
        public DeviceManagementRepository(ISqlSugarRepository<DeviceManagement> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceManagement> Queryable(DeviceManagementDto dto)
        {
            return Repo.AsQueryable();
        }

        public override ISugarQueryable<DeviceManagementDto> DtoQueryable(DeviceManagementDto dto)
        {
            return Repo.AsQueryable().WhereIF(!string.IsNullOrWhiteSpace(dto.Label), t => t.Label.Contains(dto.Label)).Select((t) => new DeviceManagementDto { Id = t.Id, Label = t.Label, DeviceType = t.DeviceType, Model = t.Model, Capacity = t.Capacity, Quantity = t.Quantity, Weight = t.Weight, Manufacturer = t.Manufacturer, InstallDate = t.InstallDate, RatedCurrent = t.RatedCurrent, RatedVoltage = t.RatedVoltage, Status = t.Status, TempControl = t.TempControl, MaintenanceCycle = t.MaintenanceCycle, WarrantyPeriod = t.WarrantyPeriod, ProcessId = t.ProcessId, LastMaintenanceTime = t.LastMaintenanceTime, Remark = t.Remark, CreateBy = t.CreateBy, CreateTime = t.CreateTime, UpdateBy = t.UpdateBy, UpdateTime = t.UpdateTime, MaintenanceCountdown = t.LastMaintenanceTime != null && t.MaintenanceCycle != null ? SqlSugar.SqlFunc.DateDiff(SqlSugar.DateType.Day, SqlSugar.SqlFunc.GetDate(), SqlSugar.SqlFunc.DateAdd(t.LastMaintenanceTime.Value, t.MaintenanceCycle.Value, SqlSugar.DateType.Day)) : 0, });
        }

        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleAsync(DeviceManagementDto dto)
        {
            var query = DtoQueryable(dto).OrderBy(t => SqlFunc.DateDiff(DateType.Day, SqlFunc.GetDate(), SqlFunc.DateAdd(t.LastMaintenanceTime.Value, t.MaintenanceCycle.Value, DateType.Day)));
            return await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
        }

        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleIDAsync(DeviceManagementDto dto)
        {
            var query = DtoQueryable(dto).OrderBy(t => t.Id, OrderByType.Desc);
            var pagedInfo = await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
            pagedInfo.Code = StatusCodes.Status200OK;
            return pagedInfo;
        }

        public async Task<SqlSugarPagedList<DeviceManagement>> GetPagedListByDeviceTypeAsync(long deviceTypeId, int pageIndex, int pageSize)
        {
            var sql = @"SELECT device_id FROM device_management_type WHERE devicetype_id = @DeviceTypeId";
            var parameters = new List<SugarParameter>
            {
                new SugarParameter("@DeviceTypeId", deviceTypeId)
            };
            List<long> deviceIds = await Repo.Ado.SqlQueryAsync<long>(sql, parameters.ToArray());
            if (deviceIds == null || !deviceIds.Any())
            {
                return new SqlSugarPagedList<DeviceManagement>()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    Total = 0,
                    Rows = new List<DeviceManagement>(),
                    Code = StatusCodes.Status200OK,
                    HasPrevPages = false,
                    HasNextPages = false
                };
            }

            var query = Repo.AsQueryable().Where(t => deviceIds.Contains(t.Id));
            var pagedResult = await query.ToPagedListAsync(pageIndex, pageSize);
            pagedResult.Code = StatusCodes.Status200OK;
            return pagedResult;
        }
    }
}