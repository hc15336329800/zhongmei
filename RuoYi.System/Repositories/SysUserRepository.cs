using RuoYi.Data;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using System.Text;

namespace RuoYi.System.Repositories
{
    public class SysUserRepository : BaseRepository<SysUser, SysUserDto>
    {
        public SysUserRepository(ISqlSugarRepository<SysUser> sqlSugarRepository)
        {
            Repo = sqlSugarRepository;
        }

        public override ISugarQueryable<SysUser> Queryable(SysUserDto dto)
        {
            return Repo.AsQueryable().LeftJoin<SysDept>((u, d) => u.DeptId == d.DeptId).Where(u => u.DelFlag == DelFlag.No).WhereIF(dto.UserId > 0, u => u.UserId == dto.UserId).WhereIF(!string.IsNullOrEmpty(dto.UserName), u => u.UserName!.Contains(dto.UserName!)).WhereIF(!string.IsNullOrEmpty(dto.Status), u => u.Status == dto.Status).WhereIF(!string.IsNullOrEmpty(dto.Phonenumber), u => u.Phonenumber!.Contains(dto.Phonenumber!)).WhereIF(dto.UserId > 0, u => u.UserId == dto.UserId).WhereIF(dto.Params.BeginTime != null, (u) => u.CreateTime >= dto.Params.BeginTime).WhereIF(dto.Params.EndTime != null, (u) => u.CreateTime <= dto.Params.EndTime).WhereIF(dto.DeptId > 0, (u) => u.DeptId == dto.DeptId || u.DeptId == SqlFunc.Subqueryable<SysDept>().Where(d => SqlFunc.SplitIn(SqlFunc.MappingColumn(d.Ancestors, $"{nameof(d.Ancestors)} COLLATE utf8mb4_general_ci"), dto.DeptId.ToString())).GroupBy(d => d.DeptId).Select(d => d.DeptId)).WhereIF(!string.IsNullOrEmpty(dto.Params.DataScopeSql), dto.Params.DataScopeSql);
        }

        public override ISugarQueryable<SysUserDto> DtoQueryable(SysUserDto dto)
        {
            return this.UserDtoQueryable(dto);
        }

        public async Task<SysUser> GetUserByUserNameAsync(string userName)
        {
            return await this.FirstOrDefaultAsync(x => x.UserName == userName && x.DelFlag == DelFlag.No);
        }

        public async Task<SysUserDto> GetUserDtoByUserNameAsync(string userName)
        {
            var user = await this.GetUserByUserNameAsync(userName);
            return user.Adapt<SysUserDto>();
        }

        public async Task<SysUserDto> GetUserDtoAsync(SysUserDto dto)
        {
            var queryable = base.Repo.Context.Queryable<SysUser>().WhereIF(!string.IsNullOrEmpty(dto.DelFlag), u => u.DelFlag == dto.DelFlag).WhereIF(!string.IsNullOrEmpty(dto.UserName), u => u.UserName == dto.UserName).WhereIF(!string.IsNullOrEmpty(dto.Phonenumber), u => u.Phonenumber == dto.Phonenumber).WhereIF(!string.IsNullOrEmpty(dto.Email), u => u.Email == dto.Email).WhereIF(dto.UserId > 0, u => u.UserId == dto.UserId);
            var user = await queryable.FirstAsync();
            var userDto = user.Adapt<SysUserDto>();
            if (userDto != null)
            {
                var dept = await base.Repo.Context.Queryable<SysDept>().FirstAsync(d => d.DeptId == userDto.DeptId);
                userDto.Dept = dept.Adapt<SysDeptDto>();
                var roles = await base.Repo.Context.Queryable<SysRole>().InnerJoin<SysUserRole>((r, ur) => r.RoleId == ur.RoleId).Where((r, ur) => ur.UserId == userDto.UserId).ToListAsync();
                userDto.Roles = roles.Adapt<List<SysRoleDto>>();
            }

            return userDto!;
        }

        public async Task<int> UpdateUserAvatarAsync(string userName, string avatar)
        {
            return await base.Updateable().SetColumns(u => u.Avatar == avatar).Where(u => u.UserName == userName).ExecuteCommandAsync();
        }

        public async Task<int> UpdateUserStatusAsync(SysUserDto user)
        {
            return await base.Updateable().SetColumns(u => u.Status == user.Status).Where(u => u.UserId == user.UserId).ExecuteCommandAsync();
        }

        public async Task<int> UpdateUserLoginInfoAsync(SysUserDto user)
        {
            return await base.Updateable().SetColumns(u => u.LoginIp == user.LoginIp).SetColumns(u => u.LoginDate == user.LoginDate).Where(u => u.UserId == user.UserId).ExecuteCommandAsync();
        }

        public int DeleteUserById(long userId)
        {
            return base.Updateable().SetColumns(u => u.DelFlag == DelFlag.Yes).Where(u => u.UserId == userId).ExecuteCommand();
        }

        public int DeleteUserByIds(List<long> userIds)
        {
            return base.Updateable().SetColumns(u => u.DelFlag == DelFlag.Yes).Where(u => userIds.Contains(u.UserId)).ExecuteCommand();
        }

        public async Task<int> ResetPasswordAsync(string userName, string password)
        {
            return await base.Updateable().SetColumns(u => u.Password == password).Where(u => u.UserName == userName).ExecuteCommandAsync();
        }

        public int UpdateUser(SysUserDto userDto)
        {
            var user = userDto.Adapt<SysUser>();
            return base.Updateable(user).IgnoreColumns(u => u.Password).Where(u => u.UserId == userDto.UserId).ExecuteCommand();
        }

        public async Task<bool> CheckDeptExistUserAsync(long deptId)
        {
            var count = await base.CountAsync(d => d.DelFlag == DelFlag.No && d.DeptId == deptId);
            return count > 0;
        }

#region private methods
        private ISugarQueryable<SysUser> UserQueryable(SysUserDto dto)
        {
            var parameters = this.GetSugarParameters(dto);
            var sb = new StringBuilder();
            sb.AppendLine(@"
            select u.user_id, u.dept_id, u.nick_name, u.user_name, u.email, u.avatar, u.phonenumber, u.sex, u.status, u.del_flag, u.login_ip, u.login_date, u.create_by, u.create_time, u.remark, d.dept_name, d.leader from sys_user u
		    left join sys_dept d on u.dept_id = d.dept_id
            ");
            var where = GetDtoWhere(dto);
            sb.AppendLine(where);
            return base.SqlQueryable(sb.ToString(), parameters);
        }

        private ISugarQueryable<SysUserDto> UserDtoQueryable(SysUserDto dto)
        {
            var parameters = this.GetSugarParameters(dto);
            var sb = new StringBuilder();
            sb.AppendLine(GetDtoTable());
            sb.AppendLine(GetDtoWhere(dto));
            return base.SqlQueryable(sb.ToString(), parameters).Select(u => new SysUserDto { UserId = u.UserId, UserName = u.UserName, Password = u.Password, DeptId = u.DeptId, NickName = u.NickName, Email = u.Email, Status = u.Status, CreateTime = u.CreateTime, Remark = u.Remark });
        }

        private string GetDtoTable()
        {
            return @"
            select distinct u.*
            from sys_user u
		    left join sys_dept d on u.dept_id = d.dept_id
		    left join sys_user_role ur on u.user_id = ur.user_id
		    left join sys_role r on r.role_id = ur.role_id
            ";
        }

        private string GetDtoWhere(SysUserDto dto)
        {
            var sb = new StringBuilder();
            sb.AppendLine("where u.del_flag = '0'");
            if (dto.UserId.HasValue && dto.UserId > 0)
            {
                sb.AppendLine("AND u.user_id = @UserId");
            }

            if (!string.IsNullOrEmpty(dto.UserName))
            {
                sb.AppendLine("AND u.user_name like concat('%', @UserName, '%')");
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                sb.AppendLine("AND u.user_name = @Status");
            }

            if (!string.IsNullOrEmpty(dto.Phonenumber))
            {
                sb.AppendLine("AND u.phonenumber like concat('%', @PhoneNumber, '%')");
            }

            if (dto.Params.BeginTime != null)
            {
                sb.AppendLine("AND date_format(u.create_time,'%y%m%d') >= date_format(@BeginTime,'%y%m%d')");
            }

            if (dto.Params.BeginTime != null)
            {
                sb.AppendLine("AND date_format(u.create_time,'%y%m%d') <= date_format(@EndTime,'%y%m%d')");
            }

            if (dto.DeptId.HasValue && dto.DeptId > 0)
            {
                sb.AppendLine("AND (u.dept_id = @DeptId OR u.dept_id IN ( SELECT t.dept_id FROM sys_dept t WHERE find_in_set(@DeptId, ancestors) ))");
            }

            if (dto.IsAllocated.HasValue)
            {
                if (dto.IsAllocated.Value)
                {
                    sb.AppendLine("AND r.role_id = @RoleId");
                }
                else
                {
                    sb.AppendLine("AND (r.role_id != @RoleId OR r.role_id IS NULL)");
                    sb.AppendLine("AND u.user_id NOT IN (select u.user_id from sys_user u inner join sys_user_role ur on u.user_id = ur.user_id and ur.role_id = @RoleId)");
                }
            }

            if (!string.IsNullOrEmpty(dto.Params.DataScopeSql))
            {
                sb.AppendLine($"AND {dto.Params.DataScopeSql}");
            }

            return sb.ToString();
        }

        private List<SugarParameter> GetSugarParameters(SysUserDto dto)
        {
            var parameters = new List<SugarParameter>();
            if (dto.UserId > 0)
            {
                parameters.Add(new SugarParameter("@UserId", dto.UserId));
            }

            if (!string.IsNullOrEmpty(dto.UserName))
            {
                parameters.Add(new SugarParameter("@UserName", dto.UserName));
            }

            if (!string.IsNullOrEmpty(dto.DelFlag))
            {
                parameters.Add(new SugarParameter("@DelFlag", dto.DelFlag));
            }

            if (!string.IsNullOrEmpty(dto.Status))
            {
                parameters.Add(new SugarParameter("@Status", dto.Status));
            }

            if (!string.IsNullOrEmpty(dto.Phonenumber))
            {
                parameters.Add(new SugarParameter("@PhoneNumber", dto.Phonenumber));
            }

            if (dto.Params.BeginTime != null)
            {
                parameters.Add(new SugarParameter("@BeginTime", dto.Params.BeginTime));
            }

            if (dto.Params.BeginTime != null)
            {
                parameters.Add(new SugarParameter("@EndTime", dto.Params.EndTime));
            }

            if (dto.DeptId > 0)
            {
                parameters.Add(new SugarParameter("@DeptId", dto.DeptId));
            }

            parameters.Add(new SugarParameter("@RoleId", dto.RoleId));
            return parameters;
        }
#endregion
    }
}