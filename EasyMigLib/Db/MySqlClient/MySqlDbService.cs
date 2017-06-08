using EasyMigLib.Commands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace EasyMigLib.Db.MySqlClient
{

    public class MySqlDbService : IMySqlDbService
    {
        public MySqlConnection Connection { get; protected set; }

        public IMySqlDbService CreateConnection(string connectionString)
        {
            this.Connection = new MySqlConnection(connectionString);
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

        protected void CheckAndAddParameters(MySqlCommand command, List<MySqlParameter> parameters)
        {
            if (parameters != null)
            {
                foreach (var parameter in parameters)
                {
                    command.Parameters.Add(parameter);
                }
            }
        }

        public int Execute(string sql, List<MySqlParameter> parameters = null)
        {
            int rowsAffected = 0;

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    this.CheckAndAddParameters(command, parameters);

                    rowsAffected = command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return rowsAffected;
        }

        public List<Dictionary<string, object>> ReadAll(string sql, List<MySqlParameter> parameters = null)
        {
            var result = new List<Dictionary<string, object>>();

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    this.CheckAndAddParameters(command, parameters);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var rows = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                rows[reader.GetName(i)] = value.GetType() == typeof(DBNull) ? null : value;
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

        public Dictionary<string, object> ReadOne(string sql, List<MySqlParameter> parameters = null)
        {
            var result = new Dictionary<string, object>();

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    this.CheckAndAddParameters(command, parameters);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var value = reader.GetValue(i);
                                result[reader.GetName(i)] = value.GetType() == typeof(DBNull) ? null : value;
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


        public object ExecuteScalar(string sql, List<MySqlParameter> parameters = null)
        {
            object result = null;

            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    this.CheckAndAddParameters(command, parameters);

                    result = command.ExecuteScalar();
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return result;
        }

        public async Task ExecuteAsync(string sql, List<MySqlParameter> parameters = null)
        {
            try
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = sql;
                    this.CheckAndAddParameters(command, parameters);
                    
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
