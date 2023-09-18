using ShiftGeniusLibDB.Aggregate;
using ShiftGeniusLibDB.Models;

namespace ShiftGenius.Rules
{
    public class EmployeeMaxHoursStrategy : RuleStrategy
    {
        Employee employee;
        int maxHours, organizationID;
        Schedule schedule;

        public EmployeeMaxHoursStrategy(Employee e, int hrs, int orgID, Schedule s)
        {
            employee = e;
            maxHours = hrs;
            organizationID = orgID;
            schedule = s;
        }

        public bool CheckSchedule(Schedule s)
        {
            return s.GetTotalHoursForEmployee(employee.EmployeeId).Hours <= maxHours;
        }

        public Schedule EnforceRules(Schedule s)
        {
            Random random = new Random();

            int currentHours = (int)(s.GetTotalHoursForEmployee(employee.EmployeeId).TotalHours);
            int hoursOver = currentHours - maxHours;

            while (hoursOver > 0)
            {
                // Find an existing shift to shorten
                List<ScheduleDay> shifts = s.GetScheduleDaysEmployeeIsScheduled(employee.EmployeeId);
                if (shifts.Count == 0)
                    break;

                int randDay = random.Next(shifts.Count);
                EmployeeScheduled existingShift = shifts[randDay].EmployeeScheduleds.FirstOrDefault(es => es.EmployeeId == employee.EmployeeId);

                if (existingShift != null)
                {
                    int decision = random.Next(2);

                    if (decision == 0)
                    {
                        // Move the end time back by 1 hour
                        existingShift.EndTime = existingShift.EndTime.AddHours(-1);
                    }
                    else
                    {
                        // Move the start time forward by 1 hour
                        existingShift.StartTime = existingShift.StartTime.AddHours(1);
                    }

                    hoursOver--;
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

                int remainingHoursForDay = Math.Min(8, maxHours - currentHours);

                while (remainingHoursForDay > 0)
                {
                    // Decide the length of the shift to schedule
                    int shiftLength = Math.Min(remainingHoursForDay, random.Next(6, 8));

                    // Randomly decide the start time (considering that the shift should fit within a 8 AM - 8 PM window)
                    //TODO: Fetch this from database
                    int startHour = random.Next(8, 20 - shiftLength);
                    DateTime shiftStart = day.Day.AddHours(startHour);
                    DateTime shiftEnd = shiftStart.AddHours(shiftLength);

                    var newShift = new EmployeeScheduled
                    {
                        EmployeeId = employee.EmployeeId,
                        ScheduleDayId = day.ScheduleDayId,
                        StartTime = shiftStart,
                        EndTime = shiftEnd
                    };

                    day.EmployeeScheduleds.Add(newShift);

                    // Update tracking variables
                    currentHours += shiftLength;
                    remainingHoursForDay -= shiftLength;

                    // Check if maxHours - 8 has been reached or exceeded
                    if (currentHours >= maxHours-8)
                    {
                        break;
                    }
                }

                // Exit the loop if we've reached or exceeded maxHours - 8
                if (currentHours >= maxHours-8)
                {
                    break;
                }
            }

            return schedule;
        }
    }
}