using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class DropDatabaseCommand
    {
        public string DatabaseName { get; protected set; }

        public DropDatabaseCommand(string databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetDropDatabase(this.DatabaseName);
        }
    }
}
