namespace EasyMigLib.Commands
{
    public class DatabaseParameter
    {
        public string ParameterName { get; }
        public DatabaseParameterDirection Direction { get; }
        public ColumnType ParameterType { get; set; }
        public object DefaultValue { get; }

        public DatabaseParameter(string parameterName, ColumnType parameterType, object defaultValue, DatabaseParameterDirection direction)
        {
            this.ParameterName = parameterName;
            this.ParameterType = parameterType;
            this.DefaultValue = defaultValue;
            this.Direction = direction;
        }
    }
}
