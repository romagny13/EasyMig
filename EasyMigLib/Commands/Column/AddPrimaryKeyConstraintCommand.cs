using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public class AddPrimaryKeyConstraintCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }
        public string[] PrimaryKeys { get; protected set; }

        public AddPrimaryKeyConstraintCommand(string tableName, string[] primaryKeys)
        {
            this.TableName = tableName;
            this.PrimaryKeys = primaryKeys;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetAddPrimaryKeyConstraint(this.TableName, this.PrimaryKeys);
        }
    }
}
