using RuoYi.Common.Data;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    public class DeviceManagementMaintRepository : BaseRepository<DeviceManagementMaint, DeviceManagementMaintDto>
    {
        public DeviceManagementMaintRepository(ISqlSugarRepository<DeviceManagementMaint> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceManagementMaint> Queryable(DeviceManagementMaintDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.DeviceId > 0, (t) => t.DeviceId == dto.DeviceId);
        }

        public override ISugarQueryable<DeviceManagementMaintDto> DtoQueryable(DeviceManagementMaintDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.DeviceId > 0, (t) => t.DeviceId == dto.DeviceId).Select((t) => new DeviceManagementMaintDto { DeviceId = t.DeviceId }, true);
        }
    }
}