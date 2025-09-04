using RuoYi.Data.Models;

namespace RuoYi.System.Services;
public class SysUserOnlineService : ITransient
{
    public SysUserOnline? GetOnlineByIpaddr(string ipaddr, LoginUser user)
    {
        if (StringUtils.Equals(ipaddr, user.IpAddr))
        {
            return LoginUserToUserOnline(user);
        }

        return null;
    }

    public SysUserOnline? GetOnlineByUserName(string userName, LoginUser user)
    {
        if (StringUtils.Equals(userName, user.UserName))
        {
            return LoginUserToUserOnline(user);
        }

        return null;
    }

    public SysUserOnline? GetOnlineByInfo(string ipaddr, string userName, LoginUser user)
    {
        if (StringUtils.Equals(ipaddr, user.IpAddr) && StringUtils.Equals(userName, user.UserName))
        {
            return LoginUserToUserOnline(user);
        }

        return null;
    }

    public SysUserOnline? LoginUserToUserOnline(LoginUser user)
    {
        if (user == null || user.User == null)
        {
            return null;
        }

        return new SysUserOnline
        {
            TokenId = user.Token,
            UserName = user.UserName,
            Ipaddr = user.IpAddr,
            LoginLocation = user.LoginLocation,
            Browser = user.Browser,
            Os = user.OS,
            LoginTime = user.LoginTime,
            DeptName = user.User.Dept?.DeptName
        };
    }
}