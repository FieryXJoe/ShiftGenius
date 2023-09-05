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
    }
}