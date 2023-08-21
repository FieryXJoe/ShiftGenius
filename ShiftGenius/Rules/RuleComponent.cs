using ShiftGeniusLibDB.Aggregate;

namespace ShiftGenius.Rules
{
    
    public interface RuleComponent
    {
        public Schedule GenerateSchedule();

        public bool CheckSchedule(Schedule s);

        public Schedule EnforceRules(Schedule s);

        //Class Diagram Specifies changeParent and changeChild but they shouldn't be needed.

        public String EncodeJSON();

        public String DecodeJSON(String json);
    }
}
