using Lab3.Business.Enums;
using Lab3.Business.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Business.Services
{
    public class EmployeeService
    {
        private readonly LogService _logService;
        private const int Delay = 10000;
        public string ConnectionString { get; }
        public IsolationLevel TransactionIsolationLevel { get; }
        public EmployeeService(string connectionString)
        {
            ConnectionString = connectionString;
            _logService = new LogService(ConnectionString);
            TransactionIsolationLevel = IsolationLevel.Serializable;
        }

        public async Task AddEmployee(Employee employee)
        {
            try
            {
                using(SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = "INSERT INTO employees(id, firstname, lastname, address, dateofbirth, groupId, specializationid) " +
                            $"VALUES(NEWID(), '{employee.FirstName}', '{employee.LastName}', '{employee.Address}', " +
                            $"'{employee.DateOfBirth}', '{employee.GroupId}', '{employee.SpecializationId}')";

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

        public async Task UpdateEmployee(Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"UPDATE employees SET firstname='{employee.FirstName}', lastname='{employee.LastName}'," +
                            $"dateofbirth='{employee.DateOfBirth}', address='{employee.Address}', groupid='{employee.GroupId}'," +
                            $"specializationid='{employee.SpecializationId}' WHERE id='{employee.Id}'";

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

        public async Task DeleteEmployee(Employee employee)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"DELETE FROM employees WHERE id='{employee.Id}'";

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

        public async Task<IEnumerable<BaseEntity>> GetEmployees()
        {

            try
            {
                var employees = new List<Employee>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM employees ORDER BY firstname, lastname";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var employee = new Employee
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Address = reader["Address"].ToString(),
                                GroupId = Guid.Parse(reader["GroupId"].ToString()),
                                SpecializationId = Guid.Parse(reader["SpecializationId"].ToString())
                            };

                            employees.Add(employee);
                        }
                    }
                }

                return employees;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BaseEntity>> GetEmployeesWithDetails()
        {

            try
            {
                var employees = new List<Employee>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT employees.id, employees.firstname, employees.lastname, employees.DateOfBirth, employees.Address, " +
                            $"employees.specializationid, employees.groupid, specializations.name AS specializationname, groups.name AS groupname FROM employees " +
                            $"LEFT JOIN specializations ON employees.specializationid=specializations.id " +
                            $"LEFT JOIN groups ON employees.groupid=groups.id ORDER BY employees.firstname, employees.lastname";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            bool isSpecializationNull = string.IsNullOrEmpty(reader["specializationname"].ToString());
                            bool isGroupNull = string.IsNullOrEmpty(reader["groupname"].ToString());

                            var employee = new Employee
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Address = reader["Address"].ToString(),
                                GroupId = Guid.Parse(reader["GroupId"].ToString()),
                                SpecializationId = Guid.Parse(reader["SpecializationId"].ToString()),
                                GroupName = isGroupNull ? "Undefined" : reader["groupname"].ToString(),
                                SpecalizationName = isSpecializationNull ? "Undefined" : reader["specializationname"].ToString()
                            };

                            employees.Add(employee);
                        }
                    }
                }

                return employees;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseEntity> GetEmployeeById(Guid employeeId)
        {

            try
            {
                var employee = new Employee();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM employees WHERE id='{employeeId}'";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            employee = new Employee
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                FirstName = reader["FirstName"].ToString(),
                                LastName = reader["LastName"].ToString(),
                                DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                                Address = reader["Address"].ToString(),
                                GroupId = Guid.Parse(reader["GroupId"].ToString()),
                                SpecializationId = Guid.Parse(reader["SpecializationId"].ToString())
                            };
                        }
                    }
                }

                return employee;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<BaseEntity>> GetSpecializations()
        {
            try
            {
                var specializations = new List<Specialization>();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM specializations";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            var specialization = new Specialization
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString()
                            };

                            specializations.Add(specialization);
                        }
                    }
                }

                return specializations;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<BaseEntity> GetSpecializationByName(string name)
        {
            try
            {
                var specialization = new Specialization();
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand command = new SqlCommand())
                    {
                        await connection.OpenAsync();
                        command.Connection = connection;
                        command.CommandText = $"SELECT * FROM specializations WHERE name='{name}'";
                        var reader = await command.ExecuteReaderAsync();

                        while (reader.Read())
                        {
                            specialization = new Specialization
                            {
                                Id = Guid.Parse(reader["Id"].ToString()),
                                Name = reader["Name"].ToString()
                            };
                        }
                    }
                }

                return specialization;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
