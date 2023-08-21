using ShiftGeniusLibDB.Aggregate;

namespace ShiftGenius.Rules
{
    
    public interface RuleComponent
    {
        public Schedule generateSchedule();

        public bool checkSchedule();

        public Schedule enforceRules(Schedule s);

        //Class Diagram Specifies changeParent and changeChild but they shouldn't be needed.

        public String EncodeJSON();

        public String DecodeJSON(String json);
    }
}
