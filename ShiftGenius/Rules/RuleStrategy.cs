namespace ShiftGenius.Rules
{
    public interface RuleStrategy
    {
        public Schedule generateSchedule();
        public bool checkSchedule(Schedule s);
        public Schedule enforceRules(Schedule s);
    }
}
