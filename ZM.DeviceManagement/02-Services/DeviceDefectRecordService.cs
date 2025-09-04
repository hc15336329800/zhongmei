using Mapster;
using Microsoft.Extensions.Logging;
using RuoYi.Common.Data;
using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Framework.DependencyInjection;
using ZM.Device.Dtos;
using ZM.Device.Entities;
using ZM.Device.Repositories;

namespace ZM.Device.Services
{
    public class DeviceDefectRecordService : BaseService<DeviceDefectRecord, DeviceDefectRecordDto>, ITransient
    {
        private readonly ILogger<DeviceDefectRecordService> _logger;
        private readonly DeviceDefectRecordRepository _deviceDefectRecordRepository;
        public DeviceDefectRecordService(ILogger<DeviceDefectRecordService> logger, DeviceDefectRecordRepository deviceDefectRecordRepository)
        {
            BaseRepo = deviceDefectRecordRepository;
            _logger = logger;
            _deviceDefectRecordRepository = deviceDefectRecordRepository;
        }

        public async Task<DeviceDefectRecord> GetAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            return entity;
        }

        public async Task<DeviceDefectRecordDto> GetDtoAsync(long id)
        {
            var entity = await base.FirstOrDefaultAsync(e => e.Id == id);
            var dto = entity.Adapt<DeviceDefectRecordDto>();
            return dto;
        }
    }
}