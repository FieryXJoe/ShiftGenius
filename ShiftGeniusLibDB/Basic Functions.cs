using ShiftGeniusLibDB.Models;
using System;
using System.Collections.Generic;
using System.Data;
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
        public static bool employeeWithEmailExists(string email)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employee = context.Employees.Where(e => e.Email == email).FirstOrDefault();
                if (employee != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public static bool AddEmployee(string name, string email, string password)
        {
            if (employeeWithEmailExists(email))
            {
                return false;
            }
            else
            {
                using (var context = new ShiftGeniusContext())
                {
                    var employee = new Employee
                    {
                        Name = name,
                        Email = email,
                        Password = password,
                        OrganizationId = null
                    };
                    context.Employees.Add(employee);
                    context.SaveChanges();
                    return true;
                }
                return false;
            }
        }
        /**
         * Employee has multiple roles so this checks all of their roles and determines if they have manager privledge
         */
        public static bool isManager(int employeeId)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employeeRoles = context.EmployeeRoles.Where(e => e.EmployeeId == employeeId).ToList();
                foreach (EmployeeRole er in employeeRoles)
                {
                    var role = context.Roles.Where(r => r.RoleId == er.RoleId).FirstOrDefault();
                    if (role.IsManager)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static Employee getEmployeeByID(int id)
        {
            using (var context = new ShiftGeniusContext())
            {
                var employee = context.Employees.Where(e => e.EmployeeId == id).FirstOrDefault();
                if (employee != null)
                {
                    return employee;
                }
                else
                {
                    return null;
                }
            }
        }

        public static List<ScheduleRule> GetRulesForOrganization(int organizationID)
        {
            using (var context = new ShiftGeniusContext())
            {
                var rules = context.ScheduleRules.Where(r => r.OrganizationId == organizationID).ToList();
                if (rules != null)
                {
                    return rules;
                }
                else
                {
                    return null;
                }
            }
        }
        public static bool UpdateRule(int ruleID, String jsonRule)
        {
            using (var context = new ShiftGeniusContext())
            {
                var rule = context.ScheduleRules.Where(r => r.ScheduleRuleId == ruleID).FirstOrDefault();
                if (rule != null)
                {
                    rule.Rule = jsonRule;
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool AddRule(int organizationID, String jsonRule)
        {
            using (var context = new ShiftGeniusContext())
            {
                var rule = new ScheduleRule
                {
                    OrganizationId = organizationID,
                    Rule = jsonRule,
                    DateCreated = DateTime.Now,
                    Approved = true
                };
                context.ScheduleRules.Add(rule);
                context.SaveChanges();
                return true;
            }
        }
        
        public static ScheduleRule GetRuleById(int id)
        {
            using (var context = new ShiftGeniusContext())
            {
                var rule = context.ScheduleRules.Where(r => r.ScheduleRuleId == id).FirstOrDefault();
                if (rule != null)
                {
                    return rule;
                }
                else
                {
                    return null;
                }
            }
        }

        public static bool DeleteRule(int id)
        {
            using (var context = new ShiftGeniusContext())
            {
                var rule = context.ScheduleRules.Where(r => r.ScheduleRuleId == id).FirstOrDefault();
                if (rule != null)
                {
                    context.ScheduleRules.Remove(rule);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
