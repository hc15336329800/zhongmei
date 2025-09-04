using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Device.Entities;
using SqlSugar;

namespace ZM.Device.Repositories
{
    public class DeviceTypeRepository : BaseRepository<DeviceType, DeviceTypeDto>
    {
        public DeviceTypeRepository(ISqlSugarRepository<DeviceType> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<DeviceType> Queryable(DeviceTypeDto dto)
        {
            return Repo.AsQueryable().Where(t => t.Status == "0").Where(t => t.DelFlag == "0").WhereIF(!string.IsNullOrEmpty(dto.DeptName), t => t.DeptName == dto.DeptName).WhereIF(dto.ParentId > 0, t => t.ParentId == dto.ParentId);
            ;
        }

        public override ISugarQueryable<DeviceTypeDto> DtoQueryable(DeviceTypeDto dto)
        {
            return Repo.AsQueryable().Where(t => t.Status == "0").Where(t => t.DelFlag == "0").Select((t) => new DeviceTypeDto { Id = t.Id, ParentId = t.ParentId, DeptName = t.DeptName, OrderNum = t.OrderNum, Status = t.Status, DelFlag = t.DelFlag, Ancestors = t.Ancestors });
        }

        public async Task<int> DeleteDeptByIdAsync(long Id)
        {
            return await base.Updateable().SetColumns(col => col.DelFlag == DelFlag.Yes).Where(col => col.Id == Id).ExecuteCommandAsync();
        }

        public async Task<List<DeviceType>> GetChildrenDeptByIdAsync(long deptId)
        {
            string sql = @"
        SELECT * 
        FROM device_type 
        WHERE 
            ancestors LIKE CONCAT('%,', @Id, ',%') OR 
            ancestors LIKE CONCAT(@Id, ',%') OR 
            ancestors LIKE CONCAT('%,', @Id) OR 
            ancestors = @Id";
            var list = await Repo.Ado.SqlQueryAsync<DeviceType>(sql, new { Id = deptId });
            return list;
        }

        public async Task<bool> HasChildByDeptIdAsync(long parentDeptId)
        {
            var query = new DeviceTypeDto
            {
                DelFlag = DelFlag.No,
                ParentId = parentDeptId
            };
            return await base.AnyAsync(query);
        }

        public async Task<bool> CheckDeviceTypeExistInManagementTypeAsync(long deviceTypeId)
        {
            var sql = @"SELECT COUNT(1) 
                FROM device_management_type 
                WHERE devicetype_id = @deviceTypeId";
            var parameters = new List<SugarParameter>
            {
                new SugarParameter("@deviceTypeId", deviceTypeId)
            };
            int count = await Repo.Ado.SqlQuerySingleAsync<int>(sql, parameters.ToArray());
            return count > 0;
        }
    }
}