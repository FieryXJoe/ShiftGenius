using ShiftGeniusLibDB.Aggregate;

namespace ShiftGenius.Rules
{
    public class DefaultStrategy : RuleStrategy
    {
        Schedule schedule;

        public DefaultStrategy(Schedule s)
        {
            schedule = s;
        }

        public bool CheckSchedule(Schedule s)
        {
            return true;
        }

        public Schedule EnforceRules(Schedule s)
        {
            return schedule;
        }
    }
}