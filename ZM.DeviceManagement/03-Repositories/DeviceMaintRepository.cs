using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;

namespace ZM.Device.Repositories
{
    public class DeviceMaintRepository : BaseRepository<DeviceMaint, DeviceMaintDto>
    {
        public DeviceMaintRepository(ISqlSugarRepository<DeviceMaint> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceMaint> Queryable(DeviceMaintDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.Id > 0, (t) => t.Id == dto.Id);
        }

        public override ISugarQueryable<DeviceMaintDto> DtoQueryable(DeviceMaintDto dto)
        {
            return Repo.AsQueryable().WhereIF(dto.Id > 0, (t) => t.Id == dto.Id).Select((t) => new DeviceMaintDto { Id = t.Id }, true);
        }
    }
}