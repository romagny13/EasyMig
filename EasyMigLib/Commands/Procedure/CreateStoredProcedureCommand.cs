using EasyMigLib.Query;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{

    public class CreateStoredProcedureCommand : DatabaseCommand
    {
        public string ProcedureName { get; protected set; }
        public Dictionary<string, DatabaseParameter> Parameters { get; }
        public string Body { get; protected set; }

        public CreateStoredProcedureCommand(string procedureName)
        {
            this.ProcedureName = procedureName;
            this.Parameters = new Dictionary<string, DatabaseParameter>();
        }

        public CreateStoredProcedureCommand AddParameter(string parameterName, ColumnType parameterType, object defaultValue, DatabaseParameterDirection direction = DatabaseParameterDirection.IN)
        {
            this.Parameters[parameterName] = new DatabaseParameter(parameterName, parameterType, defaultValue, direction);
            return this;
        }

        public CreateStoredProcedureCommand AddParameter(string parameterName, ColumnType parameterType, DatabaseParameterDirection direction = DatabaseParameterDirection.IN)
        {
            return this.AddParameter(parameterName, parameterType, null, direction);
        }

        public CreateStoredProcedureCommand SetBody(string body)
        {
            this.Body = body;
            return this;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetCreateStoredProcedure(this.ProcedureName, this.Parameters, this.Body);
        }
    }
}
