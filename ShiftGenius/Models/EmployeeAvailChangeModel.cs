using System.Collections.Generic;

namespace ShiftGenius.Models
{
	public class EmployeeAvailChangeModel
	{
		public Dictionary<string, DayAvailability> Availability { get; set; }
	}

	public class DayAvailability
	{
		public bool Enabled { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
	}
}
