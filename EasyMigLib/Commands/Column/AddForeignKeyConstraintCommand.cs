using EasyMigLib.Services;

namespace EasyMigLib.Commands
{
    public class AddForeignKeyConstraintCommand
    {
        public string TableName { get; protected set; }
        public ForeignKeyColumn ForeignKey { get; protected set; }

        public AddForeignKeyConstraintCommand(string tableName, ForeignKeyColumn foreignKey)
        {
            this.TableName = tableName;
            this.ForeignKey = foreignKey;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetAddForeignKeyConstraint(this.TableName, this.ForeignKey);
        }
    }
}
