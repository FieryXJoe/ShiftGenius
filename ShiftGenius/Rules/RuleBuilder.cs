using ShiftGeniusLibDB.Models;
using ShiftGenius.Rules;
using Newtonsoft.Json.Linq;
using System.Data;

namespace ShiftGeniusLibDB.Aggregate
{
    public class RuleBuilder
    {
        private readonly int organizationId;
        Schedule schedule;

        public RuleBuilder(int organizationId, Schedule s)
        {
            this.organizationId = organizationId;
            schedule = s;
        }

        public RuleBuilder(int organizationId)
        {
            this.organizationId = organizationId;
            schedule = new Schedule(organizationId);
        }
        public DefaultRule BuildRules()
        {
            List<ScheduleRule> rules = Basic_Functions.getRulesForOrganization(organizationId);

            DefaultRule defaultRule = new DefaultRule(schedule);
            RuleComponent lastRule = defaultRule;

            foreach (var rule in rules)
            {
                JObject ruleJson = JObject.Parse(rule.Rule);
                string ruleType = (string)ruleJson["Type"];

                switch (ruleType)
                {
                    case "MinHours":
                        var minHoursDecorator = new MinHoursDecorator(rule.Rule, schedule);
                        lastRule.ruleComponent = minHoursDecorator;
                        lastRule = minHoursDecorator;
                        break;
                    case "MaxHours":
                        var maxHoursDecorator = new MaxHoursDecorator(rule.Rule, schedule);
                        lastRule.ruleComponent = maxHoursDecorator;
                        lastRule = maxHoursDecorator;
                        break;
                    case "OperatingHours":
                        var operatingHoursDecorator = new OperatingHoursDecorator(rule.Rule, schedule);
                        lastRule.ruleComponent = operatingHoursDecorator;
                        lastRule = operatingHoursDecorator;
                        break;
                    case "MinEmployees":
                        var minEmployeesStrategy = new MinEmployeesDecorator(rule.Rule, schedule);
                        lastRule.ruleComponent = minEmployeesStrategy;
                        lastRule = minEmployeesStrategy;
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported rule type");
                }
            }
            return defaultRule;
        }

        public RuleDecorator buildSingleRule(string ruleType)
        {
            switch (ruleType)
            {
                case "MinHours":
                    return new MinHoursDecorator(schedule);
                case "MaxHours":
                    return new MaxHoursDecorator(schedule);
                case "OperatingHours":
                    return new OperatingHoursDecorator(schedule);
                case "MinEmployees":
                    return new MinEmployeesDecorator(schedule);
                default:
                    throw new InvalidOperationException("Unsupported rule type");
            }
        }

        public RuleDecorator buildSingleRule(ScheduleRule rule)
        {
            string jsonString = rule.Rule;
            JObject ruleJson = JObject.Parse(jsonString);
            string ruleType = (string)ruleJson["Type"];

            switch (ruleType)
            {
                case "MinHours":
                    return new MinHoursDecorator(jsonString, schedule);
                case "MaxHours":
                    return new MaxHoursDecorator(jsonString, schedule);
                case "OperatingHours":
                    return new OperatingHoursDecorator(jsonString, schedule);
                case "MinEmployees":
                    return new MinEmployeesDecorator(jsonString, schedule);
                default:
                    throw new InvalidOperationException("Unsupported rule type");
            }
        }

        public void SaveRuleToDatabase(RuleDecorator ruleDecorator, int? ruleId = null)
        {
            string jsonRule = ruleDecorator.EncodeJSON();

            if (ruleId.HasValue)
            {
                Basic_Functions.UpdateRule(ruleId.Value, jsonRule);
            }
            else
            {
                Basic_Functions.AddRule(organizationId, jsonRule);
            }
        }

        public RuleDecorator LoadRuleFromDatabase(int id)
        {
            ScheduleRule rule = Basic_Functions.GetRuleById(id);
            return buildSingleRule(rule);
        }
    }
}