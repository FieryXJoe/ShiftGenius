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

            ruleStrategy = new EmployeeMinHoursStrategy();
            schedule = s;
        }

        public MinHoursDecorator(String json, Schedule s)
        {
            DecodeJSON(json);
        }

        public override bool checkSchedule()
        {
            return ruleStrategy.checkSchedule(schedule);
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
            json += "\"EmployeeID\": " + employee.EmployeeId + ",";
            json += "\"MinHours\": " + minHours;
            json += "}";
            return json;
        }

        public override Schedule enforceRules(Schedule s)
        {
            return ruleStrategy.enforceRules(s);
        }

        public override Schedule generateSchedule()
        {
            return ruleStrategy.generateSchedule();
        }
    }
}
