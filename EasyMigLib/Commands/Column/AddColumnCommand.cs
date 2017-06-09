using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public class AddColumnCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }
        public MigrationColumn Column { get; protected set; }

        public AddColumnCommand(string tableName, MigrationColumn column)
        {
            this.TableName = tableName;
            this.Column = column;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetAddColumn(this.TableName, this.Column);
        }
    }
}
