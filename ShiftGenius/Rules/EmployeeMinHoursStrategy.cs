namespace ShiftGenius.Rules
{
    public class EmployeeMinHoursStrategy : RuleStrategy
    {
        public bool checkSchedule(Schedule s)
        {
            return true;
        }

        public Schedule enforceRules(Schedule s)
        {
            return s;
        }

        public Schedule generateSchedule()
        {
            return new Schedule();
        }
    }
}
