using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;
using System.Collections.Generic;

namespace ShiftGenius.Rules
{
    public class MinEmployeesStrategy : RuleStrategy
    {
        int minEmployees;
        TimeSpan startTime;
        TimeSpan endTime;
        Schedule schedule;

        public MinEmployeesStrategy(int minEmp, Schedule s, TimeSpan start, TimeSpan end)
        {
            minEmployees = minEmp;
            schedule = s;
            startTime = start;
            endTime = end;
        }

        public bool CheckSchedule(Schedule s)
        {
            foreach (var day in s.ScheduleDays)
            {
                if (!AreMinimumEmployeesScheduled(day, minEmployees, startTime, endTime))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreMinimumEmployeesScheduled(ScheduleDay day, int minEmployees, TimeSpan start, TimeSpan end)
        {
            // Create a timeline for the day
            Dictionary<TimeSpan, int> timeline = new Dictionary<TimeSpan, int>();

            if (day.EmployeeScheduleds != null)
            {
                foreach (var shift in day.EmployeeScheduleds)
                {
                    // Only consider shifts within the operating hours
                    if (shift.StartTime.TimeOfDay >= start && shift.EndTime.TimeOfDay <= end)
                    {
                        if (!timeline.ContainsKey(shift.StartTime.TimeOfDay))
                        {
                            timeline[shift.StartTime.TimeOfDay] = 0;
                        }
                        if (!timeline.ContainsKey(shift.EndTime.TimeOfDay))
                        {
                            timeline[shift.EndTime.TimeOfDay] = 0;
                        }

                        timeline[shift.StartTime.TimeOfDay]++;
                        timeline[shift.EndTime.TimeOfDay]--;
                    }
                }
            }

            // Count employees at each time in the timeline
            int count = 0;
            foreach (var time in timeline.Keys.OrderBy(t => t))
            {
                count += timeline[time];
                if (count < minEmployees)
                {
                    return false;
                }
            }

            return true;
        }

        public Schedule EnforceRules(Schedule s)
        {
            Random random = new Random();
            foreach (var day in s.ScheduleDays)
            {
                while (!AreMinimumEmployeesScheduled(day, minEmployees, startTime, endTime))
                {
                    // Find an employee that is not scheduled for this day and add them
                    Employee availableEmployee = s.FindEmployeeNotScheduledForDay(day);
                    if (availableEmployee != null)
                    {
                        // Add a 4-6 hour shift for this employee to cover the gap
                        TimeSpan shiftLength = TimeSpan.FromHours(random.Next(4, 7));
                        EmployeeScheduled newShift = new EmployeeScheduled
                        {
                            EmployeeScheduledId = availableEmployee.EmployeeId,
                            ScheduleDayId = day.ScheduleDayId,
                            StartTime = day.Day + startTime,
                            EndTime = day.Day + startTime + shiftLength
                        };

                        day.EmployeeScheduleds.Add(newShift);
                    }
                }
            }

            return s;
        }

        public Schedule GenerateSchedule()
        {
            // Return a blank schedule for now
            schedule.Reset();
            return schedule;
        }
    }
}
