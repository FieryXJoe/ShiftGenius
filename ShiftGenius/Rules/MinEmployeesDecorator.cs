using ShiftGeniusLibDB.Aggregate;
using System;
using System.Text.Json;

namespace ShiftGenius.Rules
{
    public class MinEmployeesDecorator : RuleDecorator
    {
        int minEmployees;
        TimeSpan start, end;
        public override Schedule schedule { get; set; }
        public override RuleStrategy ruleStrategy { get; set; }
        public override RuleComponent ruleComponent { get; set; }

        public MinEmployeesDecorator(int min, Schedule s, TimeSpan start, TimeSpan end)
        {
            minEmployees = min;
            this.start = start;
            this.end = end;
            ruleStrategy = new MinEmployeesStrategy(minEmployees, s, start, end );
            schedule = s;
        }

        public MinEmployeesDecorator(String json, Schedule s)
        {
            DecodeJSON(json);
            schedule = s;
            ruleStrategy = new MinEmployeesStrategy(minEmployees, s, start, end);
        }

        public MinEmployeesDecorator(Schedule s)
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

            if (root.TryGetProperty("MinEmployees", out JsonElement minEmployeesElement))
            {
                minEmployees = minEmployeesElement.GetInt32();
                return $"MinEmployees: {minEmployees}";
            }
            else
            {
                return "Invalid JSON format";
            }
        }

        public override string EncodeJSON()
        {
            //Create a JSON object containing info of the number of minimum employees
            String json = "{";
            json += "\"Type\": \"MinEmployees\",";
            json += "\"MinEmployees\": " + minEmployees;
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
