using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public interface IDatabaseCommand
    {
        string GetQuery(IQueryService queryService);
    }

    public abstract class DatabaseCommand : IDatabaseCommand
    {
        public abstract string GetQuery(IQueryService queryService);
    }
}
