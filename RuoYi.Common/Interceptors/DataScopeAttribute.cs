using AspectCore.DynamicProxy;
using RuoYi.Common.Constants;
using RuoYi.Common.Utils;
using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Models;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Logging;
using RuoYi.Framework.Utils;
using System.Text;

namespace RuoYi.Common.Interceptors
{
    public class DataScopeAttribute : AbstractInterceptorAttribute
    {
        public string? DeptAlias { get; set; }
        public string? UserAlias { get; set; }
        public string? Permission { get; set; }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            try
            {
                Console.WriteLine("Before service call");
                LoginUser loginUser = SecurityUtils.GetLoginUser();
                if (loginUser != null)
                {
                    SysUserDto currentUser = loginUser.User;
                    if (currentUser != null && !SecurityUtils.IsAdmin(currentUser))
                    {
                        string permission = StringUtils.DefaultIfEmpty(Permission, PermissionContextHolder.GetContext());
                        this.DataScopeFilter(context, currentUser, DeptAlias, UserAlias, permission);
                    }
                }

                await next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Service threw an exception!");
                Log.Error("DataScope Error", ex);
            }
            finally
            {
                Console.WriteLine("After service call");
            }
        }

        private void DataScopeFilter(AspectContext context, SysUserDto user, string deptAlias, string userAlias, string permission)
        {
            StringBuilder sqlString = new StringBuilder();
            List<string> conditions = new List<string>();
            foreach (SysRoleDto role in user.Roles ?? new List<SysRoleDto>())
            {
                var dataScope = role.DataScope ?? DataScope.Custom;
                if (dataScope != DataScope.Custom && conditions.Contains(dataScope))
                {
                    continue;
                }

                if (StringUtils.IsNotEmpty(permission) && role.Permissions.IsNotEmpty() && !StringUtils.ContainsAny(role.Permissions, permission))
                {
                    continue;
                }

                if (dataScope == DataScope.All)
                {
                    sqlString = new StringBuilder();
                    conditions.Add(dataScope);
                    break;
                }
                else if (DataScopeConstants.DATA_SCOPE_CUSTOM.Equals(dataScope))
                {
                    sqlString.Append($" OR {deptAlias}.dept_id IN ( SELECT dept_id FROM sys_role_dept WHERE role_id = {role.RoleId} ) ");
                }
                else if (DataScopeConstants.DATA_SCOPE_DEPT.Equals(dataScope))
                {
                    sqlString.Append($" OR {deptAlias}.dept_id = {user.DeptId} ");
                }
                else if (DataScopeConstants.DATA_SCOPE_DEPT_AND_CHILD.Equals(dataScope))
                {
                    sqlString.Append($" OR {deptAlias}.dept_id IN ( SELECT dept_id FROM sys_dept WHERE dept_id = {user.DeptId} or find_in_set( {user.DeptId} , ancestors ) )");
                }
                else if (DataScopeConstants.DATA_SCOPE_SELF.Equals(dataScope))
                {
                    if (StringUtils.IsNotBlank(userAlias))
                    {
                        sqlString.Append($" OR {userAlias}.user_id = {user.UserId} ");
                    }
                    else
                    {
                        sqlString.Append(" OR {deptAlias}.dept_id = 0 ");
                    }
                }

                conditions.Add(dataScope);
            }

            if (conditions.IsEmpty())
            {
                sqlString.Append($" OR {deptAlias}.dept_id = 0 ");
            }

            if (StringUtils.IsNotBlank(sqlString.ToString()))
            {
                object parameters = context.Parameters[0];
                if (parameters != null && parameters.GetType().BaseType != null && parameters.GetType().BaseType!.Equals(typeof(BaseDto)))
                {
                    BaseDto baseEntity = (BaseDto)parameters;
                    baseEntity.Params.DataScopeSql = $" ({sqlString.ToString()[4..]})";
                }
            }
        }
    }
}