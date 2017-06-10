namespace EasyMigLib.Schema
{
    public class PrimaryKeyConstraint
    {
        public string TableName { get; protected set; }
        public string[] PrimaryKeys { get; protected set; }

        public PrimaryKeyConstraint(string tableName, string[] primaryKeys)
        {
            this.TableName = tableName;
            this.PrimaryKeys = primaryKeys;
        }
    }
}
