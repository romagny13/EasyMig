using EasyMigLib.Query;
namespace EasyMigLib.Commands
{
    public class AddForeignKeyConstraintCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }
        public ForeignKeyColumn ForeignKey { get; protected set; }

        public AddForeignKeyConstraintCommand(string tableName, ForeignKeyColumn foreignKey)
        {
            this.TableName = tableName;
            this.ForeignKey = foreignKey;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetAddForeignKeyConstraint(this.TableName, this.ForeignKey);
        }
    }
}
