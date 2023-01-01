using Lab3.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Business.Services
{
    public class LogService
    {
        public string ConnectionString { get; }
        public LogService(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public async Task AddLog(Log log)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        var formattedCommand = log.CommandText.Replace("\'", string.Empty);
                        command.CommandText = "INSERT INTO logs(id, commandtext, commandresult, commanddate) " +
                            $"VALUES(NEWID(), '{formattedCommand}', '{log.CommandResult}', '{log.CommandDate}')";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateLog(Log log)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"UPDATE logs SET commandtext='{log.CommandText}', commandresult='{log.CommandResult}', commanddate='{log.CommandDate}'" +
                            $" WHERE id={log.Id}";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteLog(Log log)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"DELETE FROM logs WHERE id={log.Id}";
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BaseEntity>> GetLogs()
        {

            try
            {
                var logs = new List<Log>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM logs ORDER BY commanddate";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var log = new Log
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                CommandText = reader["CommandText"].ToString(),
                                CommandResult = reader["CommandResult"].ToString(),
                                CommandDate = Convert.ToDateTime(reader["CommandDate"])
                            };

                            logs.Add(log);
                        }
                    }
                }

                return logs;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseEntity> GetLogById(Guid logId)
        {

            try
            {
                var log = new Log();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM logs WHERE id={logId}";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            log = new Log
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                CommandText = reader["CommandText"].ToString(),
                                CommandResult = reader["CommandResult"].ToString(),
                                CommandDate = Convert.ToDateTime(reader["CommandDate"])
                            };
                        }
                    }
                }

                return log;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
