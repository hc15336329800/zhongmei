using Lazy.Captcha.Core;
using RuoYi.Common.Constants;
using RuoYi.Common.Enums;
using RuoYi.Data.Models;
using RuoYi.Framework.Cache;
using RuoYi.Framework.Exceptions;

namespace RuoYi.System.Services;
public class SysLoginService : ITransient
{
    private readonly ILogger<SysLoginService> _logger;
    private readonly ICaptcha _captcha;
    private readonly ICache _cache;
    private readonly TokenService _tokenService;
    private readonly SysUserService _sysUserService;
    private readonly SysConfigService _sysConfigService;
    private readonly SysLogininforService _sysLogininforService;
    private readonly SysPasswordService _sysPasswordService;
    private readonly SysPermissionService _sysPermissionService;
    public SysLoginService(ILogger<SysLoginService> logger, ICaptcha captcha, ICache cache, TokenService tokenService, SysUserService sysUserService, SysConfigService sysConfigService, SysLogininforService sysLogininforService, SysPasswordService sysPasswordService, SysPermissionService sysPermissionService)
    {
        _logger = logger;
        _captcha = captcha;
        _cache = cache;
        _tokenService = tokenService;
        _sysUserService = sysUserService;
        _sysConfigService = sysConfigService;
        _sysLogininforService = sysLogininforService;
        _sysPasswordService = sysPasswordService;
        _sysPermissionService = sysPermissionService;
    }

    public async Task<string> LoginAsync(string username, string password, string code, string uuid)
    {
        ValidateCaptcha(username, code, uuid);
        LoginPreCheck(username, password);
        var userDto = await _sysUserService.GetDtoByUsernameAsync(username);
        CheckLoginUser(username, password, userDto);
        await _sysLogininforService.AddAsync(username, Constants.LOGIN_SUCCESS, MessageConstants.User_Login_Success);
        var loginUser = CreateLoginUser(userDto);
        await RecordLoginInfoAsync(userDto.UserId ?? 0);
        return await _tokenService.CreateToken(loginUser);
    }

    private void CheckLoginUser(string username, string password, SysUserDto user)
    {
        if (user == null)
        {
            _logger.LogInformation($"登录用户：{username} 不存在.");
            throw new ServiceException(MessageConstants.User_Passwrod_Not_Match);
        }
        else if (UserStatus.DELETED.GetValue().Equals(user.DelFlag))
        {
            _logger.LogInformation($"登录用户：{username} 已被删除.");
            throw new ServiceException(MessageConstants.User_Deleted);
        }
        else if (UserStatus.DISABLE.GetValue().Equals(user.Status))
        {
            _logger.LogInformation($"登录用户：{username} 已被停用.");
            throw new ServiceException(MessageConstants.User_Blocked);
        }

        _sysPasswordService.Validate(username, password, user);
    }

    private void ValidateCaptcha(string username, string code, string uuid)
    {
        bool captchaEnabled = _sysConfigService.IsCaptchaEnabled();
        if (captchaEnabled)
        {
            var isValidCaptcha = _captcha.Validate(uuid, code, true, true);
            if (!isValidCaptcha)
            {
                Task.Factory.StartNew(async () =>
                {
                    await _sysLogininforService.AddAsync(username, Constants.LOGIN_FAIL, MessageConstants.Captcha_Invalid);
                });
                throw new ServiceException(MessageConstants.Captcha_Invalid);
            }
        }
    }

    private void LoginPreCheck(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Task.Factory.StartNew(async () =>
            {
                await _sysLogininforService.AddAsync(username, Constants.LOGIN_FAIL, MessageConstants.Required);
            });
            throw new ServiceException(MessageConstants.Required);
        }

        if (password.Length < UserConstants.PASSWORD_MIN_LENGTH || password.Length > UserConstants.PASSWORD_MAX_LENGTH)
        {
            Task.Factory.StartNew(async () =>
            {
                await _sysLogininforService.AddAsync(username, Constants.LOGIN_FAIL, MessageConstants.User_Passwrod_Not_Match);
            });
            throw new ServiceException(MessageConstants.User_Passwrod_Not_Match);
        }

        if (username.Length < UserConstants.USERNAME_MIN_LENGTH || username.Length > UserConstants.USERNAME_MAX_LENGTH)
        {
            Task.Factory.StartNew(async () =>
            {
                await _sysLogininforService.AddAsync(username, Constants.LOGIN_FAIL, MessageConstants.User_Passwrod_Not_Match);
            });
            throw new ServiceException(MessageConstants.User_Passwrod_Not_Match);
        }

        string? blackStr = _cache.GetString("sys.login.blackIPList");
        if (IpUtils.IsMatchedIp(blackStr, App.HttpContext.GetRemoteIpAddressToIPv4()))
        {
            Task.Factory.StartNew(async () =>
            {
                await _sysLogininforService.AddAsync(username, Constants.LOGIN_FAIL, MessageConstants.Login_Blocked);
            });
            throw new ServiceException(MessageConstants.Login_Blocked);
        }
    }

    private LoginUser CreateLoginUser(SysUserDto user)
    {
        var permissions = _sysPermissionService.GetMenuPermission(user);
        return new LoginUser
        {
            UserId = user.UserId ?? 0,
            DeptId = user.DeptId ?? 0,
            UserName = user.UserName ?? "",
            Password = user.Password ?? "",
            User = user,
            Permissions = permissions
        };
    }

    public async Task RecordLoginInfoAsync(long userId)
    {
        SysUserDto sysUser = new SysUserDto();
        sysUser.UserId = userId;
        sysUser.LoginIp = IpUtils.GetIpAddr();
        sysUser.LoginDate = DateTime.Now;
        await _sysUserService.UpdateUserLoginInfoAsync(sysUser);
    }
}