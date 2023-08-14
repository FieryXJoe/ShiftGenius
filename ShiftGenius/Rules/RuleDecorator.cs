namespace ShiftGenius.Rules
{
    public abstract class RuleDecorator : RuleComponent
    {

        public abstract Schedule schedule { get; set; }

        public abstract RuleStrategy ruleStrategy { get; set; }

        public abstract RuleComponent ruleComponent { get; set; }


        public abstract bool checkSchedule();
        public abstract string DecodeJSON(String json);
        public abstract string EncodeJSON();
        public abstract Schedule enforceRules(Schedule s);
        public abstract Schedule generateSchedule();
    }
}
