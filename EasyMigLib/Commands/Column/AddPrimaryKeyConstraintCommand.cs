using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class AddPrimaryKeyConstraintCommand
    {
        public string TableName { get; protected set; }
        public string[] PrimaryKeys { get; protected set; }

        public AddPrimaryKeyConstraintCommand(string tableName, string[] primaryKeys)
        {
            this.TableName = tableName;
            this.PrimaryKeys = primaryKeys;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetAddPrimaryKeyConstraint(this.TableName, this.PrimaryKeys);
        }
    }
}
