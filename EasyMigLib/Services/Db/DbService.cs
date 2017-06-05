using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyMigLib.Services
{
    public class DbService : IDbService
    {
        public DbConnection Connection { get; protected set; }

        public IDbService CreateConnection(string connectionString, string providerName)
        {
            var connection = DbProviderFactories.GetFactory(providerName).CreateConnection();
            connection.ConnectionString = connectionString;
            this.Connection = connection;
            return this;
        }

        public bool IsOpen()
        {
            return this.Connection.State == ConnectionState.Open;
        }

        public void Open()
        {
            try
            {
                if (!this.IsOpen())
                {
                    this.Connection.Open();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task OpenAsync()
        {
            try
            {
                if (!this.IsOpen())
                {
                    await this.Connection.OpenAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Close()
        {
            this.Connection.Close();
        }

        public int Execute(string sql)
        {
            int rowsAffected = 0;

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return rowsAffected;
        }

        public List<Dictionary<string, object>> ReadAll(string sql)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rows = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                rows[reader.GetName(i)] = reader.GetValue(i);
                            }
                            result.Add(rows);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public Dictionary<string, object> ReadOne(string sql)
        {
            var result = new Dictionary<string, object>();

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                result[reader.GetName(i)] = reader.GetValue(i);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }


        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            object result = null;

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;

                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            var dbParameter = command.CreateParameter();
                            dbParameter.ParameterName = parameter.Key;
                            dbParameter.Value = parameter.Value;
                            command.Parameters.Add(dbParameter);

                        }
                    }

                    result = command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public async Task ExecuteAsync(string sql)
        {
            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
