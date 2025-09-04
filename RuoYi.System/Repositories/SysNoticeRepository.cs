namespace RuoYi.System.Repositories;
public class SysNoticeRepository : BaseRepository<SysNotice, SysNoticeDto>
{
    public SysNoticeRepository(ISqlSugarRepository<SysNotice> sqlSugarRepository)
    {
        Repo = sqlSugarRepository;
    }

    public override ISugarQueryable<SysNotice> Queryable(SysNoticeDto dto)
    {
        return Repo.AsQueryable().WhereIF(dto.NoticeId > 0, (t) => t.NoticeId == dto.NoticeId);
    }

    public override ISugarQueryable<SysNoticeDto> DtoQueryable(SysNoticeDto dto)
    {
        var dbType = Repo.Context.CurrentConnectionConfig.DbType;
        if (dbType == DbType.MySql)
        {
            return Repo.AsQueryable().WhereIF(dto.NoticeId > 0, (t) => t.NoticeId == dto.NoticeId).Select((t) => new SysNoticeDto { NoticeContent = SqlFunc.MappingColumn(t.NoticeContent, " cast(notice_content as char) ") }, true);
        }
        else
        {
            return Repo.AsQueryable().WhereIF(dto.NoticeId > 0, (t) => t.NoticeId == dto.NoticeId).Select((t) => new SysNoticeDto { NoticeContent = t.NoticeContent }, true);
        }
    }
}