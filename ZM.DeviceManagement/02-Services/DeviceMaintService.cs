using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Common.Utils;
using RuoYi.Framework.DependencyInjection;
using SqlSugar;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Interceptors;
using ZM.Device.Tool;
using RuoYi.Framework;
using Microsoft.VisualBasic;

namespace ZM.Device.Services
{
    public class DeviceMaintService : BaseService<DeviceMaint, DeviceMaintDto>, ITransient
    {
        private readonly ILogger<DeviceMaintService> _logger;
        private readonly DeviceMaintRepository _deviceMaintRepository;
        private readonly DeviceManagementMaintRepository _deviceManagementMaintRepository;
        private readonly DeviceManagementService _deviceManagementService;
        private readonly DeviceManagementRepository _deviceManagementRepository;
        public DeviceMaintService(ILogger<DeviceMaintService> logger, DeviceManagementService deviceManagementService, DeviceManagementRepository deviceManagementRepository, DeviceMaintRepository deviceMaintRepository, DeviceManagementMaintRepository deviceManagementMaintRepository)
        {
            BaseRepo = deviceMaintRepository;
            _deviceManagementService = deviceManagementService;
            _deviceManagementRepository = deviceManagementRepository;
            _logger = logger;
            _deviceMaintRepository = deviceMaintRepository;
            _deviceManagementMaintRepository = deviceManagementMaintRepository;
        }

        public async Task<bool> UpdateLastMaintTimeRawAsync(long DeviceId, DateTime lastMaintenanceTime)
        {
            var rows = await _deviceManagementService.BaseRepo.Repo.Context.Ado.ExecuteCommandAsync("UPDATE device_management SET last_maintenance_time = @lastMaintenanceTime WHERE id = @id", new List<SugarParameter> { new SugarParameter("@id", DeviceId), new SugarParameter("@lastMaintenanceTime", lastMaintenanceTime) });
            return rows > 0;
        }

        public async Task<SqlSugarPagedList<DeviceMaintDto>> GetDeviceMaintPagedListById(long deviceId)
        {
            var maintIds = await _deviceManagementMaintRepository.Repo.AsQueryable().Where(x => x.DeviceId == deviceId).Select(x => x.MaintId).ToListAsync();
            if (maintIds.IsNullOrEmpty())
            {
                return new SqlSugarPagedList<DeviceMaintDto>
                {
                    PageIndex = 1,
                    PageSize = 10,
                    Total = 0,
                    Rows = new List<DeviceMaintDto>(),
                    Code = 200,
                    HasPrevPages = false,
                    HasNextPages = false
                };
            }

            var query = _deviceMaintRepository.Repo.AsQueryable().Where(x => maintIds.Contains(x.Id)).Select(x => new DeviceMaintDto { Id = x.Id, Content = x.Content, Remark = x.Remark, CreateTime = x.CreateTime, ImageUrl = x.ImageUrl, });
            var pagedResult = await query.ToPagedListAsync(PageUtils.GetPageDomain().PageNum, PageUtils.GetPageDomain().PageSize);
            pagedResult.Code = 200;
            return pagedResult;
        }

        [Transactional]
        public virtual async Task<bool> AddWithRelationAsync(DeviceMaintDto dto)
        {
            if (dto.DeviceId <= 0)
                throw new ArgumentException("设备ID无效");
            var entity = dto.Adapt<DeviceMaint>();
            entity.Id = NextId.Id13();
            bool inserted = await _deviceMaintRepository.InsertAsync(entity);
            if (!inserted)
                return false;
            var middle = new DeviceManagementMaint
            {
                DeviceId = dto.DeviceId,
                MaintId = entity.Id,
                TenantId = 0
            };
            bool relationInserted = await _deviceManagementMaintRepository.InsertAsync(middle);
            bool updated = await UpdateLastMaintTimeRawAsync(dto.DeviceId, DateTime.Now);
            if (!updated)
            {
                _logger.LogWarning($"未能更新设备 {dto.DeviceId} 的最后保养时间");
            }

            return updated;
        }

        public virtual async Task<AjaxResult> UpdateWithDateLimitAsync(DeviceMaintDto dto)
        {
            var entity = await FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (entity == null)
            {
                return AjaxResult.Error("保养记录不存在！");
            }

            if (!entity.CreateTime.HasValue || entity.CreateTime.Value.Date != DateTime.Today)
            {
                return AjaxResult.Error("只能修改当天创建的数据！");
            }

            if (dto.Id <= 0)
            {
                dto.Id = entity.Id;
            }

            var updateCount = await UpdateAsync(dto);
            return updateCount > 0 ? AjaxResult.Success("修改成功") : AjaxResult.Error("修改失败");
        }

        [Transactional]
        public virtual async Task<bool> DeleteWithRelationAsync(long id)
        {
            if (id <= 0)
                throw new ArgumentException("主键 ID 无效");
            var entity = await FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                throw new Exception("保养记录不存在");
            await _deviceManagementMaintRepository.Repo.DeleteAsync(x => x.MaintId == entity.Id);
            var deleted = await DeleteAsync(id);
            return deleted > 0;
        }

        public async Task<DeviceMaint> GetAsync(int id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }

        public async Task<DeviceMaintDto> GetDtoAsync(int id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            var dto = entity.Adapt<DeviceMaintDto>();
            return dto;
        }
    }
}