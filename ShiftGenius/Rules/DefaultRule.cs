namespace ShiftGenius.Rules
{
    public class DefaultRule : RuleComponent
    {
        private const int MAX_SHIFT_HOURS = 8;

        Schedule schedule = new Schedule();

        RuleStrategy ruleStrategy;

        RuleComponent ruleComponent { get; set; }

        public DefaultRule(RuleStrategy ruleStrategy)
        {
            this.ruleStrategy = ruleStrategy;
        }

        public Schedule GenerateSchedule()
        {
            return new Schedule();
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

        public string DecodeJSON()
        {
            return "DefaultRule decoded from JSON";
        }

        public Schedule generateSchedule()
        {
            throw new NotImplementedException();
        }

        public bool checkSchedule()
        {
            throw new NotImplementedException();
        }

        public Schedule enforceRules(Schedule s)
        {
            throw new NotImplementedException();
        }

        public string DecodeJSON(string json)
        {
            throw new NotImplementedException();
        }
    }
}
