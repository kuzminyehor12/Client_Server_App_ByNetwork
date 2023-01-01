using Lab3.Business.Enums;
using Lab3.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lab3.Business.Services
{
    public class GroupService
    {
        private readonly LogService _logService;
        public string ConnectionString { get; }
        private const int Delay = 10000;
        public IsolationLevel TransactionIsolationLevel { get; }
        public GroupService(string connectionString)
        {
            ConnectionString = connectionString;
            _logService = new LogService(ConnectionString);
            TransactionIsolationLevel = IsolationLevel.Serializable;
        }

        public async Task AddGroup(Group group)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO groups(id, name, description, creationdate) " +
                            $"VALUES(NEWID(), '{group.Name}', '{group.Description}', '{group.CreationDate}')";

                        using(var transaction = connection.BeginTransaction(TransactionIsolationLevel))
                        {
                            try
                            {
                                command.Transaction = transaction;
                                await command.ExecuteNonQueryAsync();
                                await Task.Delay(Delay);
                                await transaction.CommitAsync();
                                var log = new Log
                                {
                                    CommandText = command.CommandText,
                                    CommandResult = CommandResult.Commit.ToString(),
                                    CommandDate = DateTime.Now
                                };
                                await _logService.AddLog(log);
                            }
                            catch (Exception)
                            {
                                await transaction.RollbackAsync();
                                var log = new Log
                                {
                                    CommandText = command.CommandText,
                                    CommandResult = CommandResult.Rollback.ToString(),
                                    CommandDate = DateTime.Now
                                };
                                await _logService.AddLog(log);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateGroup(Group group)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"UPDATE groups SET name='{group.Name}', description='{group.Description}', creationdate='{group.CreationDate}'" +
                            $" WHERE id='{group.Id}'";

                        using (var transaction = connection.BeginTransaction(TransactionIsolationLevel))
                        {
                            try
                            {
                                command.Transaction = transaction;
                                await command.ExecuteNonQueryAsync();
                                await Task.Delay(Delay);
                                await transaction.CommitAsync();
                                var log = new Log
                                {
                                    CommandText = command.CommandText,
                                    CommandResult = CommandResult.Commit.ToString(),
                                    CommandDate = DateTime.Now
                                };
                                await _logService.AddLog(log);
                            }
                            catch (Exception)
                            {
                                await transaction.RollbackAsync();
                                var log = new Log
                                {
                                    CommandText = command.CommandText,
                                    CommandResult = CommandResult.Rollback.ToString(),
                                    CommandDate = DateTime.Now
                                };
                                await _logService.AddLog(log);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task DeleteGroup(Group group)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"DELETE FROM groups WHERE id='{group.Id}'";

                        using(var transaction = connection.BeginTransaction(TransactionIsolationLevel))
                        {
                            try
                            {
                                command.Transaction = transaction;
                                await command.ExecuteNonQueryAsync();
                                await Task.Delay(Delay);
                                await transaction.CommitAsync();
                                var log = new Log
                                {
                                    CommandText = command.CommandText,
                                    CommandResult = CommandResult.Commit.ToString(),
                                    CommandDate = DateTime.Now
                                };
                                await _logService.AddLog(log);
                            }
                            catch (Exception)
                            {
                                await transaction.RollbackAsync();
                                var log = new Log
                                {
                                    CommandText = command.CommandText,
                                    CommandResult = CommandResult.Rollback.ToString(),
                                    CommandDate = DateTime.Now
                                };
                                await _logService.AddLog(log);
                                throw;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BaseEntity>> GetGroups()
        {
            try
            {
                var groups = new List<Group>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM groups ORDER BY name";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var group = new Group
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreationDate = Convert.ToDateTime(reader["CreationDate"])
                            };

                            groups.Add(group);
                        }
                    }
                }

                return groups;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BaseEntity>> GetGroupsWithDetails()
        {

            try
            {
                var groups = new List<Group>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT groups.id, groups.name, groups.description, groups.creationdate, COUNT(employees.id) as participantscount" +
                            $" FROM groups LEFT JOIN employees ON employees.groupid=groups.id " +
                            $"GROUP BY groups.id, groups.name, groups.description, groups.creationdate " +
                            $"ORDER BY groups.name";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var group = new Group
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreationDate = Convert.ToDateTime(reader["CreationDate"]),
                                ParticipantsCount = Convert.ToInt32(reader["participantscount"])
                            };

                            groups.Add(group);
                        }
                    }
                }

                return groups;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseEntity> GetGroupById(Guid groupId)
        {

            try
            {
                var group = new Group();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM groups WHERE id='{groupId}'";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            group = new Group
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreationDate = Convert.ToDateTime(reader["CreationDate"])
                            };
                        }
                    }
                }

                return group;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseEntity> GetGroupByName(string name)
        {

            try
            {
                var group = new Group();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM groups WHERE name='{name}'";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            group = new Group
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString(),
                                Description = reader["Description"].ToString(),
                                CreationDate = Convert.ToDateTime(reader["CreationDate"])
                            };
                        }
                    }
                }

                return group;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
