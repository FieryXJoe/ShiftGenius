using ShiftGeniusLibDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftGeniusLibDB
{
    public class Basic_Functions
    {
        public static void AddEmployee(string name, string email, string password, int organizationId)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employee = new Employee
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    OrganizationId = organizationId
                };
                context.Employees.Add(employee);
                context.SaveChanges();
            }
        }

        public static void AddEmployeeRole(int employeeId, int roleId)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employeeRole = new EmployeeRole
                {
                    EmployeeId = employeeId,
                    RoleId = roleId
                };
                context.EmployeeRoles.Add(employeeRole);
                context.SaveChanges();
            }
        }

        public static void AddOrganization(int ownerID)
        {
            using (var context = new ShiftGeniusContext())
            {
                var organization = new Organization
                {
                    Owner = ownerID
                };
                context.Organizations.Add(organization);
                context.SaveChanges();
            }
        }

        public static void AddRole(string name)
        {
            using (var context = new ShiftGeniusContext())
            {
                var role = new Role
                {
                    Name = name
                };
                context.Roles.Add(role);
                context.SaveChanges();
            }
        }
        public static int checkLoginCredentials(string email, string password)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employee = context.Employees.Where(e => e.Email == email && e.Password == password).FirstOrDefault();
                if (employee != null)
                {
                    return employee.EmployeeId;
                }
                else
                {
                    return -1;
                }
            }
        }
        public static string getEmployeeNameByID(int id)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employee = context.Employees.Where(e => e.EmployeeId == id).FirstOrDefault();
                if (employee != null)
                {
                    return employee.Name;
                }
                else
                {
                    return "";
                }
            }
        }
    }
}
