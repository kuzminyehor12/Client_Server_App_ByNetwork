using Lab3.Business.Models;
using Lab3.Business.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Client.Models
{
    public static class Router
    {
        public static string ConnectionString { get; } = ConfigurationManager.ConnectionStrings["PersonnelDb"].ConnectionString;
        public static string IdentityConnectionString { get; } = ConfigurationManager.ConnectionStrings["IdentityDb"].ConnectionString;

        public static IDictionary<string, Func<Task<IEnumerable<BaseEntity>>>> ReadRouter =
            new Dictionary<string, Func<Task<IEnumerable<BaseEntity>>>>
        {
            { "GetEmployees", new EmployeeService(ConnectionString).GetEmployees },
            { "GetEmployeesWithDetails", new EmployeeService(ConnectionString).GetEmployeesWithDetails },
            { "GetSpecializations" ,  new EmployeeService(ConnectionString).GetSpecializations },
            { "GetGroups", new GroupService(ConnectionString).GetGroups },
            { "GetGroupsWithDetails", new GroupService(ConnectionString).GetGroupsWithDetails },
            { "GetLogs", new LogService(ConnectionString).GetLogs }
        };

        public static IDictionary<string, Func<Guid, Task<BaseEntity>>> ReadSingleRouter =
           new Dictionary<string, Func<Guid, Task<BaseEntity>>>
           {
                { "GetEmployeeById",  new EmployeeService(ConnectionString).GetEmployeeById },
                { "GetGroupById", new GroupService(ConnectionString).GetGroupById },
                { "GetLogById", new LogService(ConnectionString).GetLogById }
           };

        public static IDictionary<string, Func<string, Task<BaseEntity>>> ReadSingleWithUniqueRouter =
          new Dictionary<string, Func<string, Task<BaseEntity>>>
          {
                { "GetSpecializationByName",  new EmployeeService(ConnectionString).GetSpecializationByName },
                { "GetGroupByName",  new GroupService(ConnectionString).GetGroupByName }
          };

        public static IDictionary<string, Func<Employee, Task>> WriteEmployeeRouter =
           new Dictionary<string, Func<Employee, Task>>
           {
                { "AddEmployee",  new EmployeeService(ConnectionString).AddEmployee },
                { "UpdateEmployee" ,  new EmployeeService(ConnectionString).UpdateEmployee },
                { "DeleteEmployee",  new EmployeeService(ConnectionString).DeleteEmployee }
           };

        public static IDictionary<string, Func<Group, Task>> WriteGroupRouter =
          new Dictionary<string, Func<Group, Task>>
          {
                { "AddGroup", new GroupService(ConnectionString).AddGroup },
                { "UpdateGroup" , new GroupService(ConnectionString).UpdateGroup },
                { "DeleteGroup" , new GroupService(ConnectionString).DeleteGroup }
          };

        public static IDictionary<string, Func<Log, Task>> WriteLogRouter =
            new Dictionary<string, Func<Log, Task>>
            {
                    { "AddLog", new LogService(ConnectionString).AddLog },
                    { "UpdateLog" , new LogService(ConnectionString).UpdateLog },
                    { "DeleteLog" , new LogService(ConnectionString).DeleteLog }
            };
    }
}
