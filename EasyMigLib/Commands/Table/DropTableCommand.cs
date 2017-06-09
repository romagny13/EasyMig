using EasyMigLib.Query;
namespace EasyMigLib.Commands
{
    public class DropTableCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }

        public DropTableCommand(string tableName)
        {
            this.TableName = tableName;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetDropTable(this.TableName);
        }
    }
}
