using EasyMigLib.Query;
namespace EasyMigLib.Commands
{
    public class DropTableCommand
    {
        public string TableName { get; protected set; }

        public DropTableCommand(string tableName)
        {
            this.TableName = tableName;
        }

        public string GetQuery(IQueryService queryService)
        {
            return queryService.GetDropTable(this.TableName);
        }
    }
}
