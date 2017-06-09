using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public class CreateAndUseDatabaseCommand : DatabaseCommand
    {
        public string DatabaseName { get; protected set; }

        public CreateAndUseDatabaseCommand(string databaseName)
        {
            this.DatabaseName = databaseName;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetCreateAndUseDatabase(this.DatabaseName);
        }
    }
}
