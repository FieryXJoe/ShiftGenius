using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;

namespace ShiftGenius.Rules
{
    public class EmployeeMinHoursStrategy : RuleStrategy
    {
        Employee employee;
        int minHours, organizationID;
        Schedule schedule;
        public EmployeeMinHoursStrategy(Employee e, int hrs, int orgID, Schedule s)
        {
            employee = e;
            minHours = hrs;
            organizationID = orgID;
            schedule = s;
        }
        public bool CheckSchedule(Schedule s)
        {
            return s.GetTotalHoursForEmployee(employee.EmployeeId).Hours >= minHours;
        }

        public Schedule EnforceRules(Schedule s)
        {
            Random random = new Random();
            
            int currentHours = (int)(s.GetTotalHoursForEmployee(employee.EmployeeId).TotalHours);
            int hoursNeeded = minHours - currentHours;

            while (hoursNeeded > 0)
            {
                int decision = random.Next(2);

                if (decision == 0) // Create a new shift
                {
                    // Fetch the days where the employee is not scheduled
                    List<ScheduleDay> daysNotScheduled = s.GetScheduleDaysEmployeeIsNotScheduled(employee.EmployeeId);

                    // If the employee is not scheduled on at least one day, proceed
                    if (daysNotScheduled.Count > 0)
                    {
                        // Randomly select a day from the list of days not scheduled
                        int randomDayIndex = random.Next(daysNotScheduled.Count);
                        ScheduleDay selectedDay = daysNotScheduled[randomDayIndex];
                        int dayOfWeek = (int)selectedDay.Day.DayOfWeek;

                        // Randomize start time between 7 AM and 2 PM
                        int randomStartHour = random.Next(7, 15);
                        DateTime startTime = selectedDay.Day.AddHours(randomStartHour);

                        // Randomize end time between 3 and 6 hours later
                        int randomEndHour = random.Next(3, 7);
                        DateTime endTime = startTime.AddHours(randomEndHour);

                        EmployeeScheduled newShift = new EmployeeScheduled
                        {
                            EmployeeScheduledId = employee.EmployeeId,
                            ScheduleDayId = selectedDay.ScheduleDayId,
                            StartTime = startTime,
                            EndTime = endTime
                        };

                        s.AddEmployeeToDay(newShift);
                        hoursNeeded -= (endTime - startTime).Hours;
                    }
                }

                else // Extend an existing shift
                {
                    // Find an existing shift that can be extended
                    List<ScheduleDay> shifts = s.GetScheduleDaysEmployeeIsScheduled(employee.EmployeeId);
                    if (shifts.Count > 0)
                    {
                        int randDay = random.Next(shifts.Count);
                        if (shifts[randDay].EmployeeScheduleds.Count != 0)
                        {
                            EmployeeScheduled existingShift = shifts[randDay].EmployeeScheduleds.FirstOrDefault(es => es.EmployeeScheduledId == employee.EmployeeId);

                            if (existingShift != null)
                            {
                                int decision2 = random.Next(2);

                                if (decision2 == 0)
                                {
                                    // Move the end time back by 1 hour
                                    existingShift.EndTime = existingShift.EndTime.AddHours(1);
                                }
                                else
                                {
                                    // Move the start time forward by 1 hour
                                    existingShift.StartTime = existingShift.StartTime.AddHours(-1);
                                }
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
            Random random = new Random();
            int currentHours = 0;

            foreach (var day in schedule.ScheduleDays)
            {
                if (day.EmployeeScheduleds == null)
                {
                    day.EmployeeScheduleds = new List<EmployeeScheduled>();
                }

                int remainingHoursForDay = Math.Min(8, minHours - currentHours);

                while (remainingHoursForDay > 0)
                {
                    // Decide the length of the shift to schedule
                    int shiftLength = Math.Min(remainingHoursForDay, random.Next(6, 8));

                    // Randomly decide the start time
                    //TODO: Fetch this from database
                    int startHour = random.Next(8, 20 - shiftLength);
                    DateTime shiftStart = day.Day.AddHours(startHour);
                    DateTime shiftEnd = shiftStart.AddHours(shiftLength);

                    var newShift = new EmployeeScheduled
                    {
                        EmployeeScheduledId = employee.EmployeeId,
                        ScheduleDayId = day.ScheduleDayId,
                        StartTime = shiftStart,
                        EndTime = shiftEnd
                    };

                    day.EmployeeScheduleds.Add(newShift);

                    currentHours += shiftLength;
                    remainingHoursForDay -= shiftLength;

                    // Check if minHours has been reached or exceeded
                    if (currentHours >= minHours)
                    {
                        break;
                    }
                }

                // Exit the loop if we've reached or exceeded minHours
                if (currentHours >= minHours)
                {
                    break;
                }
            }

            return schedule;
        }
    }
}
