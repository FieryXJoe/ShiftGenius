using ShiftGeniusLibDB;
using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;
using System.Text.Json;

namespace ShiftGenius.Rules
{
    public class MinHoursDecorator : RuleDecorator
    {
        Employee employee;
        int minHours;
        public override Schedule schedule { get; set; }
        public override RuleStrategy ruleStrategy { get; set; }
        public override RuleComponent ruleComponent { get; set; }


        public MinHoursDecorator(Employee e, int min, Schedule s)
        {
            employee = e;
            minHours = min;

            ruleStrategy = new EmployeeMinHoursStrategy(employee, minHours, employee.Organization.OrganizationId, s);
            schedule = s;
        }

        public MinHoursDecorator(String json, Schedule s)
        {
            DecodeJSON(json);
            schedule = s;

            ruleStrategy = new EmployeeMinHoursStrategy(employee, minHours, employee.Organization.OrganizationId, s);
        }

        public MinHoursDecorator(Schedule s) 
        {
            schedule = s;
        }

        public override bool CheckSchedule(Schedule s)
        {
            return ruleStrategy.CheckSchedule(schedule);
        }

        public override string DecodeJSON(String json)
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("EmployeeID", out JsonElement employeeIdElement) &&
                root.TryGetProperty("MinHours", out JsonElement minHoursElement))
            {
                int employeeId = employeeIdElement.GetInt32();
                int minHours = minHoursElement.GetInt32();

                employee = Basic_Functions.getEmployeeByID(employeeId);

                return $"EmployeeID: {employeeId}, MinHours: {minHours}";
            }
            else
            {
                // JSON doesn't contain the expected properties
                return "Invalid JSON format";
            }
        }

        public override string EncodeJSON()
        {
            //Create a JSON object containing info of the ID of the employee and the number of hours minimum
            String json = "{";

            json += "\"Type\": \"MinHours\",";
            json += "\"EmployeeID\": " + employee.EmployeeId + ",";
            json += "\"MinHours\": " + minHours;
            json += "}";
            return json;
        }

        public override Schedule EnforceRules(Schedule s)
        {
            return ruleStrategy.EnforceRules(s);
        }

        public override Schedule GenerateSchedule()
        {
            return ruleStrategy.GenerateSchedule();
        }
    }
}
