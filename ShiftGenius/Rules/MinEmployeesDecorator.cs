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

            if (root.TryGetProperty("MinEmployees", out JsonElement minEmployeesElement) &&
                root.TryGetProperty("StartTime", out JsonElement startTimeElement) &&
                root.TryGetProperty("EndTime", out JsonElement endTimeElement))
            {
                minEmployees = minEmployeesElement.GetInt32();
                start = TimeSpan.Parse(startTimeElement.GetString());
                end = TimeSpan.Parse(endTimeElement.GetString());

                return $"MinEmployees: {minEmployees}, StartTime: {start}, EndTime: {end}";
            }
            else
            {
                return "Invalid JSON format";
            }
        }

        public override string EncodeJSON()
        {
            // Create a JSON object containing info of the number of minimum employees, start time and end time
            String json = "{";
            json += "\"Type\": \"MinEmployees\",";
            json += "\"MinEmployees\": " + minEmployees + ",";
            json += "\"StartTime\": \"" + start.ToString(@"hh\:mm\:ss") + "\",";
            json += "\"EndTime\": \"" + end.ToString(@"hh\:mm\:ss") + "\"";
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
