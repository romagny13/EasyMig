using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public class ModifyColumnCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }
        public MigrationColumn Column { get; protected set; }

        public ModifyColumnCommand(string tableName, MigrationColumn column)
        {
            this.TableName = tableName;
            this.Column = column;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetModifyColumn(this.TableName, this.Column);
        }
    }
}
