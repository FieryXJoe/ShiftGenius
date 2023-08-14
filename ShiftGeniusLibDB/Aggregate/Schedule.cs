using Microsoft.EntityFrameworkCore;
using ShiftGeniusLibDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftGeniusLibDB.Aggregate
{
    public class Schedule
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        public int OrganizationId { get; private set; }

        private List<ScheduleDay> ScheduleDays { get; set; }

        public Schedule(int organizationId, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
                throw new ArgumentException("End date must be after start date.");

            OrganizationId = organizationId;
            StartDate = startDate;
            EndDate = endDate;

            //fill out schedule days with existing Days in date range or create new ones if the ScheduleDay for this Day and organization doesn't exist
            ScheduleDays = new List<ScheduleDay>();

            using (var context = new ShiftGeniusContext())
            {
                var existingScheduleDays = context.ScheduleDays.Where(sd => sd.OrganizationId == organizationId && sd.Day >= startDate && sd.Day <= endDate).ToList();
                var existingScheduleDaysDates = existingScheduleDays.Select(sd => sd.Day).ToList();
                var daysToCreate = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days).Select(offset => startDate.AddDays(offset)).Where(d => !existingScheduleDaysDates.Contains(d)).ToList();

                foreach (var day in daysToCreate)
                {
                    var scheduleDay = new ScheduleDay
                    {
                        OrganizationId = organizationId,
                        Day = day
                    };
                    context.ScheduleDays.Add(scheduleDay);
                    context.SaveChanges();
                    ScheduleDays.Add(scheduleDay);
                }

                ScheduleDays.AddRange(existingScheduleDays);
            }
        }

        public IEnumerable<ScheduleDay> GetScheduledDays()
        {
            return ScheduleDays.AsReadOnly();
        }

        public void AddEmployeeToDay(EmployeeScheduled employeeScheduled)
        {
            var targetDay = ScheduleDays.FirstOrDefault(day => day.Day == employeeScheduled.StartTime.Date && day.OrganizationId == OrganizationId);

            if (targetDay == null)
            {
                throw new InvalidOperationException("The specified day is not in this schedule or doesn't match the organization.");
            }

            targetDay.EmployeeScheduleds.Add(employeeScheduled);
        }

        public void RemoveEmployeeFromDay(int employeeScheduledId)
        {
            foreach (var day in ScheduleDays)
            {
                var toRemove = day.EmployeeScheduleds.FirstOrDefault(e => e.EmployeeScheduledId == employeeScheduledId);

                if (toRemove != null)
                {
                    day.EmployeeScheduleds.Remove(toRemove);
                    return;
                }
            }
        }

        public void UpdateEmployeeShift(int employeeScheduledId, DateTime newStartTime, DateTime newEndTime)
        {
            var employeeScheduled = ScheduleDays.SelectMany(day => day.EmployeeScheduleds)
                                                .FirstOrDefault(e => e.EmployeeScheduledId == employeeScheduledId);

            if (employeeScheduled == null)
            {
                throw new InvalidOperationException("Specified employee scheduled not found.");
            }

            employeeScheduled.StartTime = newStartTime;
            employeeScheduled.EndTime = newEndTime;
        }
        //Ensures all ScheduleDays and EmployeeScheduled for those days are in the current context then save the context.
        public void SaveChanges()
        {
            using (var context = new ShiftGeniusContext())
            {
                // 1. Ensure all ScheduleDays are tracked
                foreach (var scheduleDay in ScheduleDays)
                {
                    if (context.Entry(scheduleDay).State == EntityState.Detached)
                    {
                        context.ScheduleDays.Attach(scheduleDay);
                    }

                    // 2. Ensure all EmployeeScheduled entities are tracked
                    foreach (var employeeScheduled in scheduleDay.EmployeeScheduleds)
                    {
                        if (context.Entry(employeeScheduled).State == EntityState.Detached)
                        {
                            context.EmployeeScheduleds.Attach(employeeScheduled);
                            context.Entry(employeeScheduled).State = EntityState.Added;  // Because it's a new entity
                        }
                    }

                    // 3. Sync with database: Remove any EmployeeScheduleds from DB that aren't in our list
                    var currentEmployeeScheduledIds = scheduleDay.EmployeeScheduleds.Select(es => es.EmployeeScheduledId).ToList();
                    var employeeScheduledsInDb = context.EmployeeScheduleds.Where(es => es.ScheduleDayId == scheduleDay.ScheduleDayId).ToList();

                    foreach (var employeeScheduledInDb in employeeScheduledsInDb)
                    {
                        if (!currentEmployeeScheduledIds.Contains(employeeScheduledInDb.EmployeeScheduledId))
                        {
                            context.EmployeeScheduleds.Remove(employeeScheduledInDb);  // Remove from DB
                        }
                    }
                }
                
                context.SaveChanges();
            }
        }
    }
}