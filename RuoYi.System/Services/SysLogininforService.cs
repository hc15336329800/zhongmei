using RuoYi.Common.Utils;
using RuoYi.System.Repositories;
using System.Text;
using UAParser.Interfaces;

namespace RuoYi.System.Services;
public class SysLogininforService : BaseService<SysLogininfor, SysLogininforDto>, ITransient
{
    private readonly ILogger<SysLogininforService> _logger;
    private readonly IUserAgentParser _userAgentParser;
    private readonly SysLogininforRepository _sysLogininforRepository;
    public SysLogininforService(ILogger<SysLogininforService> logger, IUserAgentParser userAgentParser, SysLogininforRepository sysLogininforRepository)
    {
        _logger = logger;
        _userAgentParser = userAgentParser;
        _sysLogininforRepository = sysLogininforRepository;
        BaseRepo = sysLogininforRepository;
    }

    public async Task<SysLogininforDto> GetDtoAsync(long? id)
    {
        var entity = await base.FirstOrDefaultAsync(e => e.InfoId == id);
        var dto = entity.Adapt<SysLogininforDto>();
        return dto;
    }

    public async Task AddAsync(string username, string status, string message)
    {
        try
        {
            var clientInfo = this._userAgentParser.ClientInfo;
            string ip = App.HttpContext?.GetRemoteIpAddressToIPv4()!;
            string address = await AddressUtils.GetRealAddressByIPAsync(ip);
            StringBuilder s = new StringBuilder();
            s.Append(LogUtils.GetBlock(ip));
            s.Append(address);
            s.Append(LogUtils.GetBlock(username));
            s.Append(LogUtils.GetBlock(status));
            s.Append(LogUtils.GetBlock(message));
            _logger.LogInformation(s.ToString());
            string os = clientInfo?.OS?.Family?.ToString()!;
            string browser = clientInfo?.Browser?.ToString()!;
            SysLogininfor logininfor = new SysLogininfor();
            logininfor.UserName = username;
            logininfor.Ipaddr = ip;
            logininfor.LoginLocation = address;
            logininfor.Browser = browser;
            logininfor.Os = os;
            logininfor.Msg = message;
            logininfor.LoginTime = DateTime.Now;
            var statuses = new List<string>
            {
                Constants.LOGIN_SUCCESS,
                Constants.LOGOUT,
                Constants.REGISTER
            };
            if (statuses.Contains(status))
            {
                logininfor.Status = Status.Enabled;
            }
            else if (Constants.LOGIN_FAIL.Equals(status))
            {
                logininfor.Status = Status.Disabled;
            }

            await _sysLogininforRepository.InsertAsync(logininfor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "insert sysLogininfor error");
        }
    }
}