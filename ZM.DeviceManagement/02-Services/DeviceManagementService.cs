using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Data.Models;
using RuoYi.Data.Slave.Dtos;
using RuoYi.Data.Slave.Entities;
using RuoYi.Device.Entities;
using RuoYi.Framework.DependencyInjection;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Interceptors;
using SqlSugar;
using ZM.Device.Tool;
using ZM.Device.Entities;
using ZM.Device.Repositories;

namespace ZM.Device.Services
{
    public class DeviceManagementService : BaseService<DeviceManagement, DeviceManagementDto>, ITransient
    {
        private readonly ILogger<DeviceManagementService> _logger;
        private readonly DeviceManagementRepository _deviceManagementRepository;
        private readonly DeviceManagementTypeRepository _deviceManagementTypeRepository;
        public DeviceManagementService(ILogger<DeviceManagementService> logger, DeviceManagementRepository deviceManagementRepository, DeviceManagementTypeRepository deviceManagementTypeRepository)
        {
            _logger = logger;
            _deviceManagementRepository = deviceManagementRepository;
            _deviceManagementTypeRepository = deviceManagementTypeRepository;
            BaseRepo = deviceManagementRepository;
        }

        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleAsync(DeviceManagementDto dto)
        {
            return await _deviceManagementRepository.GetDtoPagedListOrderedByMaintenanceCycleAsync(dto);
        }

        public async Task<SqlSugarPagedList<DeviceManagementDto>> GetDtoPagedListOrderedByMaintenanceCycleIdAsync(DeviceManagementDto dto)
        {
            return await _deviceManagementRepository.GetDtoPagedListOrderedByMaintenanceCycleIDAsync(dto);
        }

        public async Task<SqlSugarPagedList<DeviceManagement>> GetPagedListByDeviceTypeAsync(long deviceTypeId, int pageIndex, int pageSize)
        {
            return await _deviceManagementRepository.GetPagedListByDeviceTypeAsync(deviceTypeId, pageIndex, pageSize);
        }

        public async Task<List<DeviceManagementDto>> GetListByDeviceIdsAsync(List<long> deviceIds)
        {
            if (deviceIds == null || !deviceIds.Any())
            {
                return new List<DeviceManagementDto>();
            }

            var query = BaseRepo.DtoQueryable(new DeviceManagementDto()).Where(x => deviceIds.Contains(x.Id));
            return await query.ToListAsync();
        }

        [Transactional]
        public async Task<bool> AddDeviceWithTypeAsync(DeviceManagementDto dto)
        {
            dto.LastMaintenanceTime = DateTime.Now;
            if (string.IsNullOrEmpty(dto.DeviceType))
            {
                throw new ArgumentException("设备类型不能为空");
            }

            var entity = dto.Adapt<DeviceManagement>();
            entity.Id = NextId.Id13();
            bool inserted = await _deviceManagementRepository.InsertAsync(entity);
            if (!inserted)
            {
                return false;
            }

            long newDeviceId = entity.Id;
            if (newDeviceId <= 0)
            {
                throw new Exception("新增设备记录后获取的DeviceId无效");
            }

            newDeviceId = entity.Id;
            long deviceTypeId = long.Parse(dto.DeviceType);
            if (deviceTypeId <= 0)
            {
                throw new Exception("设备类型ID无效");
            }

            bool relationInserted = await _deviceManagementTypeRepository.InsertAsync(new DeviceManagementType { DeviceId = newDeviceId, DeviceTypeId = deviceTypeId, TenantId = 0 });
            return relationInserted;
        }

        [Transactional]
        public async Task<bool> UpdateDeviceWithTypeAsync(DeviceManagementDto dto)
        {
            if (string.IsNullOrEmpty(dto.DeviceType))
            {
                throw new ArgumentException("设备类型不能为空");
            }

            var originalEntity = await _deviceManagementRepository.FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (originalEntity == null)
            {
                throw new Exception("原始设备记录不存在");
            }

            bool typeChanged = !string.Equals(originalEntity.DeviceType, dto.DeviceType, StringComparison.OrdinalIgnoreCase);
            var entity = dto.Adapt<DeviceManagement>();
            entity.Id = originalEntity.Id;
            int updateCount = await UpdateAsync(entity);
            if (updateCount <= 0)
            {
                return false;
            }

            if (!typeChanged)
            {
                return true;
            }

            long deviceId = entity.Id;
            if (deviceId <= 0)
            {
                throw new Exception("设备记录更新后获取的DeviceId无效");
            }

            int deleteCount = await _deviceManagementTypeRepository.Repo.Context.Deleteable<DeviceManagementType>().Where(x => x.DeviceId == deviceId).ExecuteCommandAsync();
            long deviceTypeId = long.Parse(dto.DeviceType);
            if (deviceTypeId <= 0)
            {
                throw new Exception("设备类型ID无效");
            }

            bool relationInserted = await _deviceManagementTypeRepository.InsertAsync(new DeviceManagementType { DeviceId = deviceId, DeviceTypeId = deviceTypeId, TenantId = 0 });
            return relationInserted;
        }

        [Transactional]
        public async Task<bool> DeleteDeviceWithTypeAsync(long deviceId)
        {
            int deleteCount = await DeleteAsync(deviceId);
            if (deleteCount <= 0)
            {
                return false;
            }

            int relationDeleteCount = await _deviceManagementTypeRepository.Repo.Context.Deleteable<DeviceManagementType>().Where(x => x.DeviceId == deviceId).ExecuteCommandAsync();
            return relationDeleteCount >= 0;
        }
    }
}