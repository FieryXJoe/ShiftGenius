namespace ShiftGenius.Rules
{
    public interface RuleStrategy
    {
        public Schedule generateSchedule();
        public bool checkSchedule();
        public Schedule enforceRules(Schedule s);
    }
}
