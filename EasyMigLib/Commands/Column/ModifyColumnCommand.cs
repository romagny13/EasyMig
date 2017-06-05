using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class ModifyColumnCommand
    {
        public string TableName { get; protected set; }
        public MigrationColumn Column { get; protected set; }

        public ModifyColumnCommand(string tableName, MigrationColumn column)
        {
            this.TableName = tableName;
            this.Column = column;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetModifyColumn(this.TableName, this.Column);
        }
    }
}
