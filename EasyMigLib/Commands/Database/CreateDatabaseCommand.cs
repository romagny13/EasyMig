using EasyMigLib.Query;
namespace EasyMigLib.Commands
{
    public class CreateDatabaseCommand : DatabaseCommand
    {
        public string DatabaseName { get; protected set; }

        public CreateDatabaseCommand(string databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetCreateDatabase(this.DatabaseName);
        }
    }
}
