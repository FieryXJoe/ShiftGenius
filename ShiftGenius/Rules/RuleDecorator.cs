namespace ShiftGenius.Rules
{
    public abstract class RuleDecorator : RuleComponent
    {

        Schedule schedule;

        RuleStrategy ruleStrategy;

        RuleComponent ruleComponent { get; set; }


        public abstract bool checkSchedule();
        public abstract string DecodeJSON();
        public abstract string EncodeJSON();
        public abstract Schedule enforceRules(Schedule s);
        public abstract Schedule generateSchedule();
    }
}
