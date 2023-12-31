﻿using Microsoft.EntityFrameworkCore;
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

        public List<ScheduleDay> ScheduleDays { get; set; }

        public Schedule() { } //for Json Deserializer

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
                var daysToCreate = Enumerable.Range(0, endDate.Subtract(startDate).Days)
                             .Select(offset => startDate.AddDays(offset))
                             .Where(d => !existingScheduleDaysDates.Contains(d))
                             .ToList();



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

        public Schedule(int organizationId) : this(organizationId, GetNextSaturday(), GetNextSaturday().AddDays(6))
        { }

        private static DateTime GetNextSaturday()
        {
            DateTime today = DateTime.Today;
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)today.DayOfWeek + 7) % 7;
            return today.AddDays(daysUntilSaturday);
        }

        public IEnumerable<ScheduleDay> GetScheduledDays()
        {
            return ScheduleDays.AsReadOnly();
        }

        public void AddEmployeeToDay(EmployeeScheduled employeeScheduled)
        {
            var targetDay = ScheduleDays.FirstOrDefault(day => day.Day == employeeScheduled.StartTime.Date);

            if (targetDay == null)
            {
                throw new InvalidOperationException("The specified day is not in this schedule.");
            }

            targetDay.EmployeeScheduleds.Add(employeeScheduled);
        }

        public void RemoveEmployeeFromDay(int EmployeeId)
        {
            foreach (var day in ScheduleDays)
            {
                var toRemove = day.EmployeeScheduleds.FirstOrDefault(e => e.EmployeeId == EmployeeId);

                if (toRemove != null)
                {
                    day.EmployeeScheduleds.Remove(toRemove);
                    return;
                }
            }
        }

        public void UpdateEmployeeShift(int EmployeeId, DateTime newStartTime, DateTime newEndTime)
        {
            var employeeScheduled = ScheduleDays.SelectMany(day => day.EmployeeScheduleds)
                                                .FirstOrDefault(e => e.EmployeeId == EmployeeId);

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
            List<ScheduleDay> scheduleDays = this.ScheduleDays;
            Basic_Functions.UpdateEmployeeScheduledForScheduleDays(scheduleDays);
        }
        public void Reset()
        {
            foreach (var scheduleDay in ScheduleDays)
            {
                scheduleDay.EmployeeScheduleds.Clear();
            }
        }

        //Below this point is logic added for specific rules, especially logic that may need to be reused for other rules.

        public TimeSpan GetTotalHoursForEmployee(int employeeId)
        {
            TimeSpan totalHours = TimeSpan.Zero;

            foreach (var scheduleDay in ScheduleDays)
            {
                foreach (var employeeScheduled in scheduleDay.EmployeeScheduleds)
                {
                    if (employeeScheduled.EmployeeId == employeeId)
                    {
                        totalHours += (employeeScheduled.EndTime - employeeScheduled.StartTime);
                    }
                }
            }

            return totalHours;
        }

        public List<ScheduleDay> GetScheduleDaysEmployeeIsNotScheduled(int employeeId)
        {
            List<ScheduleDay> daysEmployeeIsNotScheduled = new List<ScheduleDay>();

            foreach (var scheduleDay in ScheduleDays)
            {
                var isEmployeeScheduledOnThisDay = scheduleDay.EmployeeScheduleds.Any(e => e.EmployeeId == employeeId);

                if (!isEmployeeScheduledOnThisDay)
                {
                    daysEmployeeIsNotScheduled.Add(scheduleDay);
                }
            }

            return daysEmployeeIsNotScheduled;
        }

        public List<ScheduleDay> GetScheduleDaysEmployeeIsScheduled(int employeeId)
        {
            List<ScheduleDay> daysEmployeeIsScheduled = new List<ScheduleDay>();

            foreach (var scheduleDay in ScheduleDays)
            {
                var isEmployeeScheduledOnThisDay = scheduleDay.EmployeeScheduleds.Any(e => e.EmployeeId == employeeId);

                if (isEmployeeScheduledOnThisDay)
                {
                    daysEmployeeIsScheduled.Add(scheduleDay);
                }
            }

            return daysEmployeeIsScheduled;
        }

        public Employee FindEmployeeNotScheduledForDay(ScheduleDay day)
        {
            List<int> scheduledEmployeeIds = day.EmployeeScheduleds.Select(e => e.EmployeeId).ToList();

            List<Employee> allEmployees = new List<Employee>();
            using (var context = new ShiftGeniusContext())
            {
                allEmployees = context.Employees.Where(e => e.OrganizationId == OrganizationId).ToList();
            }

            // Filter out employees who are already scheduled for the day
            var availableEmployees = allEmployees.Where(e => !scheduledEmployeeIds.Contains(e.EmployeeId)).ToList();

            if (availableEmployees.Count == 0)
            {
                return null;
            }

            Random rand = new Random();
            int index = rand.Next(availableEmployees.Count);
            return availableEmployees[index];
        }

    }
}