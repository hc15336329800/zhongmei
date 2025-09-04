using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using RuoYi.Common.Interceptors;
using RuoYi.Common.Utils;
using RuoYi.Data.Models;
using RuoYi.Framework.Utils;
using RuoYi.System.Services;

namespace RuoYi.Admin.Authorization
{
    public class AuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private static string ALL_PERMISSION = "*:*:*";
        private static string SUPER_ADMIN = "admin";
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            var tokenService = App.GetService<TokenService>();
            LoginUser loginUser = tokenService.GetLoginUser(context.Request);
            if (loginUser != null)
            {
                var isAuthenticated = CheckAuthorize(context);
                if (isAuthenticated)
                {
                    tokenService.VerifyToken(loginUser);
                    await next(context!);
                }
                else
                {
                    var result = AjaxResult.Error(StatusCodes.Status403Forbidden, msg: "403 Forbidden");
                    await context.Response.WriteAsJsonAsync(result, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
                }
            }
            else
            {
                context?.SignoutToSwagger();
                var result = AjaxResult.Error(StatusCodes.Status401Unauthorized, msg: "401 Unauthorized");
                await context.Response.WriteAsJsonAsync(result, App.GetOptions<JsonOptions>()?.JsonSerializerOptions);
            }
        }

        private static bool CheckAuthorize(HttpContext httpContext)
        {
            var appAuthorizeAttribute = httpContext.GetMetadata<AppAuthorizeAttribute>();
            var appRoleAuthorizeAttribute = httpContext.GetMetadata<AppRoleAuthorizeAttribute>();
            if (appAuthorizeAttribute == null && appRoleAuthorizeAttribute == null)
                return true;
            if (appRoleAuthorizeAttribute != null)
            {
                if (!HasAnyRoles(appRoleAuthorizeAttribute.AppRoles))
                {
                    return false;
                }
            }

            if (appAuthorizeAttribute != null)
            {
                if (!HasAnyPermi(appAuthorizeAttribute.Policies))
                {
                    return false;
                }
            }

            return true;
        }

#region Permi
        private static bool HasPermi(string permission)
        {
            if (string.IsNullOrEmpty(permission))
                return false;
            var tokenService = App.GetService<TokenService>();
            var loginUser = tokenService.GetLoginUser(App.HttpContext.Request);
            if (loginUser == null || loginUser.Permissions.IsEmpty())
            {
                return false;
            }

            PermissionContextHolder.SetContext(permission);
            return HasPermissions(loginUser.Permissions, permission);
        }

        private static bool HasAnyPermi(string[] permissions)
        {
            if (permissions.IsEmpty())
                return false;
            var tokenService = App.GetService<TokenService>();
            var loginUser = tokenService.GetLoginUser(App.HttpContext.Request);
            if (loginUser == null || loginUser.Permissions.IsEmpty())
            {
                return false;
            }

            foreach (var permission in permissions)
            {
                if (HasPermissions(loginUser.Permissions, permission))
                {
                    PermissionContextHolder.SetContext(permission);
                    return true;
                }
            }

            return false;
        }

        private static bool HasPermissions(List<string> permissions, string permission)
        {
            return permissions.Contains(ALL_PERMISSION) || permissions.Contains(permission.Trim());
        }

#endregion
#region Role
        public static bool HasRole(string role)
        {
            if (StringUtils.IsEmpty(role))
            {
                return false;
            }

            LoginUser loginUser = SecurityUtils.GetLoginUser();
            if (loginUser == null || loginUser.User.Roles == null || loginUser.User.Roles.Count == 0)
            {
                return false;
            }

            foreach (var sysRole in loginUser.User.Roles)
            {
                string roleKey = sysRole.RoleKey;
                if (SUPER_ADMIN == roleKey || roleKey == StringUtils.TrimToEmpty(role))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool HasAnyRoles(string[] roles)
        {
            if (roles == null && roles.Length == 0)
            {
                return false;
            }

            LoginUser loginUser = SecurityUtils.GetLoginUser();
            if (loginUser == null || loginUser.User.Roles == null || loginUser.User.Roles.Count == 0)
            {
                return false;
            }

            foreach (string role in roles)
            {
                if (HasRole(role))
                {
                    return true;
                }
            }

            return false;
        }
#endregion
    }
}