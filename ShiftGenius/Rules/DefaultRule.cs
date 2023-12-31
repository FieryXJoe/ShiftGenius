﻿using ShiftGeniusLibDB.Aggregate;

namespace ShiftGenius.Rules
{
    public class DefaultRule : RuleComponent
    {

        Schedule schedule;

        RuleStrategy ruleStrategy;

        public RuleComponent ruleComponent { get; set; }

        public DefaultRule(Schedule s)
        {
            schedule = s;
            ruleStrategy = new DefaultStrategy(s);
        }

        public Schedule GenerateSchedule()
        {
            schedule.Reset();
            return schedule;
        }

        public bool CheckSchedule(Schedule s)
        {
            return true;
        }

        public Schedule EnforceRules(Schedule s)
        {
            return schedule;
        }

        public string EncodeJSON()
        {
            return "{}";
        }

        public string DecodeJSON(String s)
        {
            return "DefaultRule decoded from JSON";
        }
    }
}
