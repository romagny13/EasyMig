using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public class DropColumnCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }
        public string ColumnName { get; protected set; }

        public DropColumnCommand(string tableName, string columnName)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetDropColumn(this.TableName, this.ColumnName);
        }
    }
}
