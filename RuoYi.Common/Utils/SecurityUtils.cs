using Microsoft.AspNetCore.Http;
using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Models;
using RuoYi.Framework;
using RuoYi.Framework.Cache;
using RuoYi.Framework.DataEncryption;
using RuoYi.Framework.Exceptions;
using RuoYi.Framework.JwtBearer;
using RuoYi.Framework.Logging;
using System.Security.Claims;

namespace RuoYi.Common.Utils
{
    public static class SecurityUtils
    {
        private static ICache _cache = App.GetService<ICache>();
        public static long GetUserId()
        {
            var user = GetCurrentUser();
            return user?.UserId ?? 0;
        }

        public static long? GetDeptId()
        {
            var user = GetCurrentUser();
            return user?.DeptId ?? null;
        }

        public static string? GetUsername()
        {
            var user = GetCurrentUser();
            return user?.UserName;
        }

        public static LoginUser GetLoginUser()
        {
            LoginUser user = GetCurrentUser();
            return user == null ? throw new ServiceException("获取用户信息异常", StatusCodes.Status401Unauthorized) : user;
        }

        public static LoginUser GetLoginUser(HttpRequest request)
        {
            string token = GetToken(request);
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var claims = ParseToken(token);
                    string uuid = claims.Where(c => c.Type.Equals(RuoYi.Data.Constants.LOGIN_USER_KEY)).First().Value;
                    string userKey = GetTokenKey(uuid);
                    LoginUser user = _cache.Get<LoginUser>(userKey);
                    return user;
                }
                catch (Exception e)
                {
                    Log.Error("获取用户信息异常'{}'", e.Message);
                }
            }

            return null;
        }

        private static LoginUser GetCurrentUser()
        {
            return GetLoginUser(App.HttpContext.Request);
        }

#region Token
        public static IEnumerable<Claim> ParseToken(string token)
        {
            var jwtSecurityToken = JWTEncryption.SecurityReadJwtToken(token);
            return jwtSecurityToken.Claims;
        }

        public static string GetToken(HttpRequest request)
        {
            string token = request.Headers["Authorization"]!;
            if (!string.IsNullOrEmpty(token) && token.StartsWith(RuoYi.Data.Constants.TOKEN_PREFIX))
            {
                token = token.Replace(RuoYi.Data.Constants.TOKEN_PREFIX, "");
            }

            return token;
        }

#endregion
        public static string EncryptPassword(string password)
        {
            return MD5Encryption.Encrypt(password);
        }

        public static bool MatchesPassword(string rawPassword, string encodedPassword)
        {
            return MD5Encryption.Compare(rawPassword, encodedPassword);
        }

#region 是否为管理员
        public static bool IsAdmin(long? userId)
        {
            return userId != null && 1L == userId;
        }

        public static bool IsAdmin()
        {
            var userId = GetUserId();
            return IsAdmin(userId);
        }

        public static bool IsAdmin(SysUserDto user)
        {
            return IsAdmin(user?.UserId);
        }

#endregion
        public static bool IsAdminRole(long? roleId)
        {
            return roleId != null && 1L == roleId;
        }

        public static string GetTokenKey(string uuid)
        {
            return CacheConstants.LOGIN_TOKEN_KEY + uuid;
        }
    }
}