using ShiftGeniusLibDB;
using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;
using System.Text.Json;

namespace ShiftGenius.Rules
{
    public class MaxHoursDecorator : RuleDecorator
    {
        Employee employee;
        int maxHours;
        public override Schedule schedule { get; set; }
        public override RuleStrategy ruleStrategy { get; set; }
        public override RuleComponent ruleComponent { get; set; }

        public MaxHoursDecorator(Employee e, int max, Schedule s)
        {
            employee = e;
            maxHours = max;

            ruleStrategy = new EmployeeMaxHoursStrategy(employee, maxHours, employee.Organization.OrganizationId, s);
            schedule = s;
        }

        public MaxHoursDecorator(String json, Schedule s)
        {
            schedule = s;
            DecodeJSON(json);

            ruleStrategy = new EmployeeMaxHoursStrategy(employee, maxHours, employee.Organization.OrganizationId, s);
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
                root.TryGetProperty("MaxHours", out JsonElement maxHoursElement))
            {
                int employeeId = employeeIdElement.GetInt32();
                int maxHours = maxHoursElement.GetInt32();

                employee = Basic_Functions.getEmployeeByID(employeeId);

                return $"EmployeeID: {employeeId}, MaxHours: {maxHours}";
            }
            else
            {
                // JSON doesn't contain the expected properties
                return "Invalid JSON format";
            }
        }

        public override string EncodeJSON()
        {
            //Create a JSON object containing info of the ID of the employee and the number of hours maximum
            String json = "{";
            json += "\"Type\": \"MaxHours\",";
            json += "\"EmployeeID\": " + employee.EmployeeId + ",";
            json += "\"MaxHours\": " + maxHours;
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
