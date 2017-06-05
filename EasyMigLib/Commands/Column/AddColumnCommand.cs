using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class AddColumnCommand
    {
        public string TableName { get; protected set; }
        public MigrationColumn Column { get; protected set; }

        public AddColumnCommand(string tableName, MigrationColumn column)
        {
            this.TableName = tableName;
            this.Column = column;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetAddColumn(this.TableName, this.Column);
        }
    }
}
