using System.Collections.Generic;

namespace EasyMigLib.Schema
{
    public enum StoredProcedureParameterDirection
    {
        IN,
        OUT
    }

    public class StoredProcedureParameter
    {
        public string ParameterName { get; }
        public StoredProcedureParameterDirection Direction { get; }
        public ColumnType ParameterType { get; set; }
        public object DefaultValue { get; }

        public StoredProcedureParameter(
            string parameterName, 
            ColumnType parameterType, 
            object defaultValue, 
            StoredProcedureParameterDirection direction)
        {
            this.ParameterName = parameterName;
            this.ParameterType = parameterType;
            this.DefaultValue = defaultValue;
            this.Direction = direction;
        }
    }

    public class StoredProcedureSchema
    {
        public string ProcedureName { get; protected set; }

        public Dictionary<string, StoredProcedureParameter> Parameters { get; }

        public string Body { get; protected set; }

        public StoredProcedureSchema(string procedureName)
        {
            this.ProcedureName = procedureName;
            this.Parameters = new Dictionary<string, StoredProcedureParameter>();
        }

        protected StoredProcedureSchema AddParameter(string parameterName, ColumnType parameterType, object defaultValue = null, StoredProcedureParameterDirection direction = StoredProcedureParameterDirection.IN)
        {
            this.Parameters[parameterName] = new StoredProcedureParameter(parameterName, parameterType, defaultValue, direction);
            return this;
        }

        public StoredProcedureSchema AddInParameter(string parameterName, ColumnType parameterType, object defaultValue = null)
        {
            return this.AddParameter(parameterName, parameterType, defaultValue);
        }

        public StoredProcedureSchema AddOutParameter(string parameterName, ColumnType parameterType)
        {
            return this.AddParameter(parameterName, parameterType, null, StoredProcedureParameterDirection.OUT);
        }

        public StoredProcedureSchema AddInParameter(string parameterName, ColumnType parameterType)
        {
            return this.AddParameter(parameterName, parameterType);
        }

        public StoredProcedureSchema SetBody(string body)
        {
            this.Body = body;
            return this;
        }
    }
}