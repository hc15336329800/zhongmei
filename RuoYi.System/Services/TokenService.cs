using Microsoft.IdentityModel.JsonWebTokens;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.Framework.Cache;
using RuoYi.Framework.JwtBearer;
using UAParser.Interfaces;

namespace RuoYi.System.Services;
public class TokenService : ITransient
{
    protected static long MILLIS_SECOND = 1000;
    protected static long MILLIS_MINUTE = 60 * MILLIS_SECOND;
    private static long MILLIS_MINUTE_TEN = 20 * 60 * 1000L;
    private static long DEFAULT_EXPIREDTIME = 30;
    private static long JWT_EXPIREDTIME = 60 * 24 * 7;
    private readonly IUserAgentParser _userAgentParser;
    private readonly ICache _cache;
    public TokenService(IUserAgentParser userAgentParser, ICache cache)
    {
        _userAgentParser = userAgentParser;
        _cache = cache;
    }

    public LoginUser GetLoginUser(HttpRequest request)
    {
        return SecurityUtils.GetLoginUser(request);
    }

    public void DelLoginUser(string token)
    {
        if (!string.IsNullOrEmpty(token))
        {
            string userKey = GetTokenKey(token);
            _cache.Remove(userKey);
        }
    }

    public void SetLoginUser(LoginUser loginUser)
    {
        if (loginUser != null && !string.IsNullOrEmpty(loginUser.Token))
        {
            RefreshToken(loginUser);
        }
    }

    public async Task<string> CreateToken(LoginUser loginUser)
    {
        var token = Guid.NewGuid().ToString();
        loginUser.Token = token;
        await SetUserAgent(loginUser);
        RefreshToken(loginUser);
        var accessToken = JWTEncryption.Encrypt(new Dictionary<string, object>() { { Constants.LOGIN_USER_KEY, token }, { DataConstants.USER_ID, loginUser.UserId }, { DataConstants.USER_NAME, loginUser.UserName }, { DataConstants.USER_DEPT_ID, loginUser.DeptId }, { JwtRegisteredClaimNames.Exp, DateTimeOffset.UtcNow.AddMinutes(JWT_EXPIREDTIME).ToUnixTimeSeconds() } });
        return accessToken;
    }

    public void VerifyToken(LoginUser loginUser)
    {
        long expireTime = loginUser.ExpireTime;
        long currentTime = DateTime.Now.ToUnixTimeMilliseconds();
        if (expireTime - currentTime <= MILLIS_MINUTE_TEN)
        {
            RefreshToken(loginUser);
        }
    }

    public void RefreshToken(LoginUser loginUser)
    {
        var jwtSettings = App.GetConfig<JWTSettingsOptions>("JWTSettings");
        long expireTime = jwtSettings.ExpiredTime ?? DEFAULT_EXPIREDTIME;
        loginUser.LoginTime = DateTime.Now.ToUnixTimeMilliseconds();
        loginUser.ExpireTime = loginUser.LoginTime + expireTime * MILLIS_MINUTE;
        string userKey = GetTokenKey(loginUser.Token);
        _cache.Set<LoginUser>(userKey, loginUser, expireTime);
    }

    public async Task SetUserAgent(LoginUser loginUser)
    {
        var clientInfo = this._userAgentParser.ClientInfo;
        string ip = App.HttpContext.GetRemoteIpAddressToIPv4();
        loginUser.IpAddr = ip;
        loginUser.LoginLocation = await AddressUtils.GetRealAddressByIPAsync(ip);
        loginUser.Browser = clientInfo.Browser.ToString();
        loginUser.OS = clientInfo.OS.Family.ToString();
    }

    private string GetTokenKey(string uuid)
    {
        return SecurityUtils.GetTokenKey(uuid);
    }
}