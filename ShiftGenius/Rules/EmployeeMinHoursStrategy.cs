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
                    int dayOfWeek = random.Next(7);

                    // For simplicity, let's say each workday starts at 8 AM
                    //TODO: Fetch this from database
                    DateTime startTime = DateTime.Today.AddHours(8);
                    DateTime endTime = startTime.AddHours(Math.Min(hoursNeeded, 8));

                    EmployeeScheduled newShift = new EmployeeScheduled
                    {
                        EmployeeScheduledId = employee.EmployeeId,
                        ScheduleDayId = s.ScheduleDays[dayOfWeek].ScheduleDayId,
                        StartTime = startTime,
                        EndTime = endTime
                    };

                    // Add this shift to the schedule and update remaining hours needed
                    s.AddEmployeeToDay(newShift);
                    hoursNeeded -= (endTime - startTime).Hours;
                }
                else // Extend an existing shift
                {
                    // Find an existing shift that can be extended
                    List<ScheduleDay> shifts = s.GetScheduleDaysEmployeeIsScheduled(employee.EmployeeId);
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

            return s;
        }


        public Schedule GenerateSchedule()
        {
            schedule.Reset();
            Random random = new Random();
            foreach (var day in schedule.ScheduleDays)
            {
                if (day.EmployeeScheduleds == null)
                {
                    day.EmployeeScheduleds = new List<EmployeeScheduled>();
                }

                //TODO: Fetch the actual opening and closing times from the database for this step.
                int startHour = random.Next(8, 20 - minHours); // Randomly decide the start time
                DateTime shiftStart = day.Day.AddHours(startHour);
                DateTime shiftEnd = shiftStart.AddHours(minHours);

                var newShift = new EmployeeScheduled
                {
                    EmployeeScheduledId = employee.EmployeeId,
                    ScheduleDayId = day.ScheduleDayId,
                    StartTime = shiftStart,
                    EndTime = shiftEnd
                };

                day.EmployeeScheduleds.Add(newShift);
            }
            return schedule;
        }
    }
}
