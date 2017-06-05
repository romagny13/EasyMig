using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class DropTableCommand
    {
        public string TableName { get; protected set; }

        public DropTableCommand(string tableName)
        {
            this.TableName = tableName;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetDropTable(this.TableName);
        }
    }
}
