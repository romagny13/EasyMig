using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class CreateDatabaseCommand
    {
        public string DatabaseName { get; protected set; }

        public CreateDatabaseCommand(string databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetCreateDatabase(this.DatabaseName);
        }
    }
}
