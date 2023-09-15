using ShiftGenius.ShiftGenius.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftGenius.Models
{
    public class EmployeeTimeOffRequestModel
    {
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [Required(ErrorMessage = "Start Date is required.")]
        [FutureDate(ErrorMessage = "Start Date must be in the future.")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required.")]
        [FutureDate(ErrorMessage = "End Date must be in the future.")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Type of Time Off")]
        [Required(ErrorMessage = "Type of Time Off is required.")]
        public string Type { get; set; }

        [Display(Name = "Employee ID")]
        [Required(ErrorMessage = "Employee ID is required.")]
        public int EmployeeID { get; set; }


    }
}
