using ShiftGeniusLibDB.Aggregate;

namespace ShiftGenius.Rules
{
    public interface RuleStrategy
    {
        public Schedule GenerateSchedule();
        public bool CheckSchedule(Schedule s);
        public Schedule EnforceRules(Schedule s);
    }
}
