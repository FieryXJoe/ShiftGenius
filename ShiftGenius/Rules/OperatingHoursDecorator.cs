using ShiftGeniusLibDB;
using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;
using System;
using System.Text.Json;

namespace ShiftGenius.Rules
{
    public class OperatingHoursDecorator : RuleDecorator
    {
        TimeSpan startTime;
        TimeSpan endTime;
        public override Schedule schedule { get; set; }
        public override RuleStrategy ruleStrategy { get; set; }
        public override RuleComponent ruleComponent { get; set; }

        int organizationID;

        public OperatingHoursDecorator(TimeSpan start, TimeSpan end, Schedule s, int orgId)
        {
            startTime = start;
            endTime = end;
            organizationID = orgId;
            schedule = s;

            ruleStrategy = new OperatingHoursStrategy(orgId, s, startTime, endTime);
        }

        public OperatingHoursDecorator(String json, Schedule s)
        {
            DecodeJSON(json);
            schedule = s;

            ruleStrategy = new OperatingHoursStrategy(organizationID, s, startTime, endTime);
        }

        public OperatingHoursDecorator(Schedule s) 
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

            if (root.TryGetProperty("StartTime", out JsonElement startTimeElement) &&
                root.TryGetProperty("EndTime", out JsonElement endTimeElement))
            {
                startTime = TimeSpan.Parse(startTimeElement.GetString());
                endTime = TimeSpan.Parse(endTimeElement.GetString());

                return $"StartTime: {startTime}, EndTime: {endTime}";
            }
            else
            {
                // JSON doesn't contain the expected properties
                return "Invalid JSON format";
            }
        }

        public override string EncodeJSON()
        {
            String json = "{";
            json += "\"Type\": \"OperatingHours\",";
            json += "\"StartTime\": \"" + startTime.ToString() + "\",";
            json += "\"EndTime\": \"" + endTime.ToString() + "\"";
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