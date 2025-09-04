using Lazy.Captcha.Core;
using RuoYi.Common.Constants;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.Framework.Exceptions;

namespace RuoYi.System.Services;
public class SysRegisterService : ITransient
{
    private readonly ILogger<SysRegisterService> _logger;
    private readonly ICaptcha _captcha;
    private readonly SysUserService _sysUserService;
    private readonly SysConfigService _sysConfigService;
    private readonly SysLogininforService _sysLogininforService;
    public SysRegisterService(ILogger<SysRegisterService> logger, ICaptcha captcha, SysLogininforService sysLogininforService, SysUserService sysUserService, SysConfigService sysConfigService)
    {
        _logger = logger;
        _captcha = captcha;
        _sysLogininforService = sysLogininforService;
        _sysUserService = sysUserService;
        _sysConfigService = sysConfigService;
    }

    public async Task<string> RegisterAsync(RegisterBody registerBody)
    {
        string msg = "", username = registerBody.Username, password = registerBody.Password;
        SysUserDto sysUser = new SysUserDto
        {
            UserName = username
        };
        bool captchaEnabled = _sysConfigService.IsCaptchaEnabled();
        if (captchaEnabled)
        {
            ValidateCaptcha(registerBody.Code, registerBody.Uuid);
        }

        if (StringUtils.IsEmpty(username))
        {
            msg = "用户名不能为空";
        }
        else if (StringUtils.IsEmpty(password))
        {
            msg = "用户密码不能为空";
        }
        else if (username.Length < UserConstants.USERNAME_MIN_LENGTH || username.Length > UserConstants.USERNAME_MAX_LENGTH)
        {
            msg = "账户长度必须在2到20个字符之间";
        }
        else if (password.Length < UserConstants.PASSWORD_MIN_LENGTH || password.Length > UserConstants.PASSWORD_MAX_LENGTH)
        {
            msg = "密码长度必须在5到20个字符之间";
        }
        else if (!await _sysUserService.CheckUserNameUniqueAsync(sysUser))
        {
            msg = "保存用户'" + username + "'失败，注册账号已存在";
        }
        else
        {
            sysUser.NickName = username;
            sysUser.Password = SecurityUtils.EncryptPassword(password);
            bool regFlag = await _sysUserService.RegisterUserAsync(sysUser);
            if (!regFlag)
            {
                msg = "注册失败,请联系系统管理人员";
            }
            else
            {
                _ = Task.Factory.StartNew(async () =>
                {
                    await _sysLogininforService.AddAsync(username, Constants.LOGIN_SUCCESS, MessageConstants.User_Register_Success);
                });
            }
        }

        return msg;
    }

    public void ValidateCaptcha(string code, string uuid)
    {
        var isValidCaptcha = _captcha.Validate(uuid, code, true, true);
        if (!isValidCaptcha)
        {
            throw new ServiceException(MessageConstants.Captcha_Invalid);
        }
    }
}