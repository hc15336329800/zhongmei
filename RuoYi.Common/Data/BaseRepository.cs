using Mapster;
using Microsoft.AspNetCore.Http;
using RuoYi.Common.Utils;
using RuoYi.Data.Dtos;
using RuoYi.Data.Entities;
using RuoYi.Framework.DependencyInjection;
using RuoYi.Framework.Extensions;
using RuoYi.Framework.Utils;
using SqlSugar;
using System.Linq.Expressions;

namespace RuoYi.Common.Data;
public abstract class BaseRepository<TEntity, TDto> : ITransient where TEntity : BaseEntity, new()
    where TDto : BaseDto, new()
{
    public virtual ISqlSugarRepository<TEntity> Repo { get; set; }

    public abstract ISugarQueryable<TEntity> Queryable(TDto dto);
    public abstract ISugarQueryable<TDto> DtoQueryable(TDto dto);
    protected virtual async Task FillRelatedDataAsync(IEnumerable<TDto> dtos)
    {
        await Task.FromResult(true);
    }

    public virtual void FillRelatedData(IEnumerable<TDto> entities)
    {
    }

    public DbType GetDbType()
    {
        return Repo.Context.CurrentConnectionConfig.DbType;
    }

    public IUpdateable<TEntity> Updateable()
    {
        if (typeof(TEntity) == typeof(UserBaseEntity))
        {
            return Repo.Context.Updateable<TEntity>().SetColumns("update_time", DateTime.Now).SetColumns("update_by", SecurityUtils.GetUsername());
        }
        else
        {
            return Repo.Context.Updateable<TEntity>();
        }
    }

    public IUpdateable<TEntity> Updateable(TEntity entity)
    {
        this.SetUpdateUserInfo(entity);
        return Repo.Context.Updateable<TEntity>(entity);
    }

    public IUpdateable<TEntity> Updateable(IEnumerable<TEntity> entities)
    {
        this.SetUpdateUserInfo(entities);
        return Repo.Context.Updateable<TEntity>(entities);
    }

    public IInsertable<TEntity> Insertable(TEntity entity)
    {
        this.SetCreateUserInfo(entity);
        return Repo.Context.Insertable<TEntity>(entity);
    }

    public IInsertable<TEntity> Insertable(IEnumerable<TEntity> entities)
    {
        this.SetCreateUserInfo(entities);
        return Repo.Context.Insertable<TEntity>(entities.ToList());
    }

    public ISugarQueryable<TEntity> SqlQueryable(string sql)
    {
        return Repo.SqlQueryable(sql);
    }

    public ISugarQueryable<TEntity> SqlQueryable(string sql, List<SugarParameter> parameters)
    {
        return Repo.SqlQueryable(sql, parameters);
    }

#region Select
    public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
    {
        return Repo.Context.Queryable<TEntity>().Where(predicate).First();
    }

    public int Count(Expression<Func<TEntity, bool>> predicate)
    {
        return Repo.Context.Queryable<TEntity>().Count(predicate);
    }

    public int Count(TDto dto)
    {
        return Queryable(dto).Count();
    }

    public bool Any(TDto dto)
    {
        return Queryable(dto).Any();
    }

    public TEntity GetFirst(TDto dto)
    {
        return Queryable(dto).First();
    }

    public List<TEntity> GetList(TDto dto)
    {
        return Queryable(dto).ToList();
    }

    public SqlSugarPagedList<TEntity> GetPagedList(TDto dto)
    {
        var quryable = Queryable(dto);
        return this.GetPagedList(quryable);
    }

    public SqlSugarPagedList<TEntity> GetPagedList(ISugarQueryable<TEntity> queryable)
    {
        var pageDomain = PageUtils.GetPageDomain();
        SqlSugarPagedList<TEntity> pagedInfo;
        if (!string.IsNullOrEmpty(pageDomain.PropertyName))
        {
            OrderByType? orderByType = (pageDomain.IsAsc ?? "").EqualsIgnoreCase("desc") ? OrderByType.Desc : OrderByType.Asc;
            pagedInfo = queryable.OrderByPropertyName(pageDomain.PropertyName, orderByType).ToPagedList(pageDomain.PageNum, pageDomain.PageSize);
        }
        else
        {
            pagedInfo = queryable.ToPagedList(pageDomain.PageNum, pageDomain.PageSize);
        }

        pagedInfo.Code = StatusCodes.Status200OK;
        return pagedInfo;
    }

#region 返回 Dto
    public TDto GetDtoFirst(TDto dto)
    {
        return DtoQueryable(dto).First();
    }

    public List<TDto> GetDtoList(TDto dto)
    {
        return DtoQueryable(dto).ToList();
    }

    public SqlSugarPagedList<TDto> GetDtoPagedList(TDto dto)
    {
        var queryable = DtoQueryable(dto);
        return this.GetDtoPagedList(queryable);
    }

    public SqlSugarPagedList<TDto> GetDtoPagedList(ISugarQueryable<TDto> queryable)
    {
        var pageDomain = PageUtils.GetPageDomain();
        SqlSugarPagedList<TDto> pagedInfo;
        if (!string.IsNullOrEmpty(pageDomain.PropertyName))
        {
            OrderByType? orderByType = (pageDomain.IsAsc ?? "").EqualsIgnoreCase("desc") ? OrderByType.Desc : OrderByType.Asc;
            pagedInfo = queryable.OrderByPropertyName(pageDomain.PropertyName, orderByType).ToPagedList(pageDomain.PageNum, pageDomain.PageSize);
        }
        else
        {
            pagedInfo = queryable.ToPagedList(pageDomain.PageNum, pageDomain.PageSize);
        }

        pagedInfo.Code = StatusCodes.Status200OK;
        if (pagedInfo.Rows.IsNotEmpty())
        {
            FillRelatedData(pagedInfo.Rows);
        }

        return pagedInfo;
    }

#endregion
#region Async
    public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Repo.FirstOrDefaultAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Repo.CountAsync(predicate);
    }

    public async Task<int> CountAsync(TDto dto)
    {
        return await Queryable(dto).CountAsync();
    }

    public async Task<bool> AnyAsync(TDto dto)
    {
        return await Queryable(dto).AnyAsync();
    }

    public async Task<TEntity> GetFirstAsync(TDto dto)
    {
        var entities = await GetListAsync(dto);
        return entities.FirstOrDefault()!;
    }

    public async Task<List<TEntity>> GetListAsync(TDto dto)
    {
        var entities = await Queryable(dto).ToListAsync();
        return entities;
    }

    public async Task<SqlSugarPagedList<TEntity>> GetPagedListAsync(TDto dto)
    {
        var queryable = Queryable(dto);
        return await this.GetPagedListAsync(queryable);
    }

    public async Task<SqlSugarPagedList<TEntity>> GetPagedListAsync(ISugarQueryable<TEntity> queryable)
    {
        var pageDomain = PageUtils.GetPageDomain();
        SqlSugarPagedList<TEntity> pagedInfo;
        if (!string.IsNullOrEmpty(pageDomain.PropertyName))
        {
            OrderByType? orderByType = (pageDomain.IsAsc ?? "").EqualsIgnoreCase("desc") ? OrderByType.Desc : OrderByType.Asc;
            pagedInfo = await queryable.OrderByPropertyName(pageDomain.PropertyName, orderByType).ToPagedListAsync(pageDomain.PageNum, pageDomain.PageSize);
        }
        else
        {
            pagedInfo = await queryable.ToPagedListAsync(pageDomain.PageNum, pageDomain.PageSize);
        }

        pagedInfo.Code = StatusCodes.Status200OK;
        return pagedInfo;
    }

#region 返回 Dto
    public async Task<TDto> GetDtoFirstAsync(TDto dto)
    {
        var entities = await GetDtoListAsync(dto);
        return entities.FirstOrDefault()!;
    }

    public async Task<List<TDto>> GetDtoListAsync(TDto dto)
    {
        var dtos = await DtoQueryable(dto).ToListAsync();
        if (dtos.IsNotEmpty())
        {
            await FillRelatedDataAsync(dtos);
        }

        return dtos;
    }

    public async Task<SqlSugarPagedList<TDto>> GetDtoPagedListAsync(TDto dto)
    {
        var queryable = DtoQueryable(dto);
        var pagedInfo = await this.GetDtoPagedListAsync(queryable);
        if (pagedInfo.Rows.IsNotEmpty())
        {
            await FillRelatedDataAsync(pagedInfo.Rows);
        }

        return pagedInfo;
    }

    public async Task<SqlSugarPagedList<TDto>> GetDtoPagedListAsync(ISugarQueryable<TDto> queryable)
    {
        var pageDomain = PageUtils.GetPageDomain();
        SqlSugarPagedList<TDto> pagedInfo;
        if (!string.IsNullOrEmpty(pageDomain.PropertyName))
        {
            OrderByType? orderByType = (pageDomain.IsAsc ?? "").EqualsIgnoreCase("desc") ? OrderByType.Desc : OrderByType.Asc;
            pagedInfo = await queryable.OrderByPropertyName(pageDomain.PropertyName, orderByType).ToPagedListAsync(pageDomain.PageNum, pageDomain.PageSize);
        }
        else
        {
            pagedInfo = await queryable.ToPagedListAsync(pageDomain.PageNum, pageDomain.PageSize);
        }

        pagedInfo.Code = StatusCodes.Status200OK;
        if (pagedInfo.Rows.IsNotEmpty())
        {
            await FillRelatedDataAsync(pagedInfo.Rows);
        }

        return pagedInfo;
    }

#endregion
#endregion
#endregion
#region Insert
    public bool Insert(TEntity entity)
    {
        return Repo.Context.Insertable(entity).ExecuteCommandIdentityIntoEntity();
    }

    public bool Insert(IEnumerable<TEntity> entities)
    {
        return Repo.Context.Insertable(entities.ToArray()).ExecuteCommandIdentityIntoEntity();
    }

    public int InsertBatch(IEnumerable<TEntity> entities)
    {
        this.SetCreateUserInfo(entities);
        return Repo.Context.Fastest<TEntity>().BulkCopy(entities.ToList());
    }

    public bool Insert(TDto dto)
    {
        var entity = dto.Adapt<TEntity>();
        var success = this.Insert(entity);
        SetDtoPrimaryKeyValue(dto, entity);
        return success;
    }

    public bool Insert(IEnumerable<TDto> dtos)
    {
        var entities = dtos.Adapt<List<TEntity>>();
        return this.Insert(entities);
    }

    public async Task<bool> InsertAsync(TEntity entity)
    {
        this.SetCreateUserInfo(entity);
        return await Repo.InsertReturnIndentityIntoEntityAsync(entity);
    }

    public async Task<int> InsertAsync(IEnumerable<TEntity> entities)
    {
        this.SetCreateUserInfo(entities);
        return await Repo.InsertAsync(entities);
    }

    public async Task<int> InsertBatchAsync(IEnumerable<TEntity> entities)
    {
        this.SetCreateUserInfo(entities);
        return await Repo.Context.Fastest<TEntity>().BulkCopyAsync(entities.ToList());
    }

    public async Task<bool> InsertAsync(TDto dto)
    {
        var entity = dto.Adapt<TEntity>();
        var success = await this.InsertAsync(entity);
        SetDtoPrimaryKeyValue(dto, entity);
        return success;
    }

#endregion
#region Update
    public int Update(TEntity entity, bool ignoreAllNullColumns = false)
    {
        return Repo.Context.Updateable(entity).IgnoreColumns(ignoreAllNullColumns).ExecuteCommand();
    }

    public int Update(IEnumerable<TEntity> entities)
    {
        return Repo.Context.Updateable(entities.ToArray()).ExecuteCommand();
    }

    public async Task<int> UpdateAsync(TEntity entity, bool ignoreAllNullColumns = false)
    {
        this.SetUpdateUserInfo(entity);
        return await Repo.UpdateAsync(entity, ignoreAllNullColumns);
    }

    public async Task<int> UpdateAsync(IEnumerable<TEntity> entities)
    {
        this.SetUpdateUserInfo(entities);
        return await Repo.UpdateAsync(entities);
    }

    public async Task<int> UpdateBulkAsync(List<TEntity> entities)
    {
        this.SetUpdateUserInfo(entities);
        return await Repo.Context.Fastest<TEntity>().BulkUpdateAsync(entities);
    }

    public int Update(TDto dto, bool ignoreAllNullColumns = false)
    {
        var entity = dto.Adapt<TEntity>();
        return this.Update(entity, ignoreAllNullColumns);
    }

    public async Task<int> UpdateAsync(TDto dto, bool ignoreAllNullColumns = false)
    {
        var entity = dto.Adapt<TEntity>();
        return await this.UpdateAsync(entity, ignoreAllNullColumns);
    }

    public int Update(IEnumerable<TDto> dtos)
    {
        var entities = dtos.Adapt<List<TEntity>>();
        return this.Update(entities);
    }

    public async Task<int> UpdateAsync(IEnumerable<TDto> dtos)
    {
        var entities = dtos.Adapt<List<TEntity>>();
        return await this.UpdateAsync(entities);
    }

    public async Task<int> UpdateBulkAsync(List<TDto> dtos)
    {
        var entities = dtos.Adapt<List<TEntity>>();
        return await this.UpdateBulkAsync(entities);
    }

#endregion
#region Delete
    public int Delete(TEntity entity)
    {
        return Repo.Context.Deleteable(entity).ExecuteCommand();
    }

    public int Delete(TDto dto)
    {
        var entity = dto.Adapt<TEntity>();
        return this.Delete(entity);
    }

    public int DeleteByKey<TKey>(TKey key)
    {
        return Repo.Context.Deleteable<TEntity>().In(key).ExecuteCommand();
    }

    public int DeleteByKeys<TKey>(TKey[] keys)
    {
        return Repo.Context.Deleteable<TEntity>().In(keys).ExecuteCommand();
    }

    public int DeleteByKeys<TKey>(List<TKey> keys)
    {
        return Repo.Context.Deleteable<TEntity>().In(keys).ExecuteCommand();
    }

    public int Delete(Expression<Func<TEntity, bool>> whereExpression)
    {
        return Repo.Context.Deleteable<TEntity>().Where(whereExpression).ExecuteCommand();
    }

    public async Task<int> DeleteAsync(TEntity entity)
    {
        return await Repo.DeleteAsync(entity);
    }

    public async Task<int> DeleteAsync(TDto dto)
    {
        var entity = dto.Adapt<TEntity>();
        return await Repo.DeleteAsync(entity);
    }

    public async Task<int> DeleteAsync<TKey>(TKey key)
    {
        return await Repo.DeleteAsync(key);
    }

    public async Task<int> DeleteAsync<TKey>(TKey[] keys)
    {
        return await Repo.DeleteAsync(keys);
    }

    public async Task<int> DeleteAsync<TKey>(List<TKey> keys)
    {
        return await Repo.DeleteAsync(keys);
    }

    public async Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
    {
        return await Repo.DeleteAsync(whereExpression);
    }

#endregion
    private void SetDtoPrimaryKeyValue(TDto dto, TEntity entity)
    {
        object? id = null;
        string keyName = "";
        Type type;
        var props = typeof(TEntity).GetProperties();
        foreach (var prop in props)
        {
            var attrSugarColumn = prop.CustomAttributes.Where(a => a.AttributeType == typeof(SugarColumn)).FirstOrDefault();
            if (attrSugarColumn != null)
            {
                var primaryKey = attrSugarColumn.NamedArguments.Where(arg => arg.MemberInfo.Name.EqualsIgnoreCase("IsPrimaryKey")).FirstOrDefault();
                if (primaryKey.MemberInfo != null)
                {
                    keyName = prop.Name;
                    type = prop.PropertyType;
                    id = prop.GetValue(entity);
                    ReflectUtils.SetPropertyValue(dto, keyName, id);
                }
            }
        }
    }

    private void SetCreateUserInfo(TEntity entity)
    {
        var baseType = typeof(TEntity).BaseType;
        if (baseType != typeof(UserBaseEntity) && baseType != typeof(CreateUserBaseEntity))
            return;
        ReflectUtils.SetPropertyValue(entity, "CreateBy", SecurityUtils.GetUsername()!);
        ReflectUtils.SetPropertyValue(entity, "CreateTime", DateTime.Now);
    }

    private void SetCreateUserInfo(IEnumerable<TEntity> entities)
    {
        var baseType = typeof(TEntity).BaseType;
        if (baseType != typeof(UserBaseEntity) && baseType != typeof(CreateUserBaseEntity))
            return;
        foreach (TEntity entity in entities)
        {
            this.SetCreateUserInfo(entity);
        }
    }

    private void SetUpdateUserInfo(TEntity entity)
    {
        if (typeof(TEntity).BaseType != typeof(UserBaseEntity))
            return;
        ReflectUtils.SetPropertyValue(entity, "UpdateBy", SecurityUtils.GetUsername()!);
        ReflectUtils.SetPropertyValue(entity, "UpdateTime", DateTime.Now);
    }

    private void SetUpdateUserInfo(IEnumerable<TEntity> entities)
    {
        if (typeof(TEntity).BaseType != typeof(UserBaseEntity))
            return;
        foreach (TEntity entity in entities)
        {
            this.SetUpdateUserInfo(entity);
        }
    }
}