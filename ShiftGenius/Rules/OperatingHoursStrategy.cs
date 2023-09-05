using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;
using System;

namespace ShiftGenius.Rules
{
    public class OperatingHoursStrategy : RuleStrategy
    {
        int organizationID;
        Schedule schedule;
        TimeSpan startTime;
        TimeSpan endTime;

        public OperatingHoursStrategy(int orgID, Schedule s, TimeSpan start, TimeSpan end)
        {
            organizationID = orgID;
            schedule = s;
            startTime = start;
            endTime = end;
        }

        public bool CheckSchedule(Schedule s)
        {
            foreach (var day in s.ScheduleDays)
            {
                if (day.EmployeeScheduleds != null)
                {
                    foreach (var shift in day.EmployeeScheduleds)
                    {
                        if (!IsWithinOperatingHours(shift.StartTime.TimeOfDay, shift.EndTime.TimeOfDay))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool IsWithinOperatingHours(TimeSpan start, TimeSpan end)
        {
            if (startTime <= endTime)
            {
                return start >= startTime && end <= endTime;
            }
            else
            {
                // If hours span midnight (e.g., 8 PM - 1 AM)
                return (start >= startTime && end >= startTime) || (start <= endTime && end <= endTime);
            }
        }

        public Schedule EnforceRules(Schedule s)
        {
            foreach (var day in s.ScheduleDays)
            {
                if (day.EmployeeScheduleds != null)
                {
                    for (int i = day.EmployeeScheduleds.Count - 1; i >= 0; i--)
                    {
                        EmployeeScheduled shift = day.EmployeeScheduleds.ToList()[i];

                        if (!IsWithinOperatingHours(shift.StartTime.TimeOfDay, shift.EndTime.TimeOfDay))
                        {
                            // Calculate how far the shift start and end times are from the operating hours
                            TimeSpan distanceFromOpening = shift.StartTime.TimeOfDay - startTime;
                            TimeSpan distanceFromClosing = endTime - shift.EndTime.TimeOfDay;

                            // Compare to see which end of the shift is closer to the operating hours
                            if (distanceFromOpening.TotalMinutes < distanceFromClosing.TotalMinutes)
                            {
                                // Adjust shift to match starttime
                                shift.StartTime = shift.StartTime - distanceFromOpening;
                                shift.EndTime = shift.EndTime - distanceFromOpening;
                            }
                            else
                            {
                                // Adjust shift to match endtime
                                shift.StartTime = shift.StartTime + distanceFromClosing;
                                shift.EndTime = shift.EndTime + distanceFromClosing;
                            }
                        }
                    }
                }
            }
            return s;
        }

        public Schedule GenerateSchedule()
        {
            schedule.Reset();
            return schedule;
        }
    }
}