namespace ShiftGenius.Models
{
	public class EmployeeAvailChangeModel
	{
		public Dictionary<string, Availability>? Availability { get; set; }
	}

	public class Availability
	{
		public bool? Enabled { get; set; }
		public string? StartTime { get; set; }
		public string? EndTime { get; set; }
	}
}
