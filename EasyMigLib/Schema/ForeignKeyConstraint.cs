namespace EasyMigLib.Schema
{
    public class ForeignKeyConstraint
    {
        public string TableName { get; }
        public string ColumnName { get; }
        public ColumnType ColumnType { get; }

        public string TableReferenced { get; }
        public string PrimaryKeyReferenced { get; }

        public ForeignKeyConstraint(
            string tableName,
            string columnName,
            ColumnType columnType,
            string tableReferenced,
            string primaryKeyReferenced)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.ColumnType = columnType;
            this.TableReferenced = tableReferenced;
            this.PrimaryKeyReferenced = primaryKeyReferenced;
        }
    }
}
