using System;
using System.ComponentModel.DataAnnotations;

namespace ShiftGenius
{

    namespace ShiftGenius.Models
    {
        public class FutureDateAttribute : ValidationAttribute
        {
            public override bool IsValid(object value)
            {
                if (value is DateTime date)
                {
                    return date > DateTime.Now;
                }
                return false;
            }
        }
    }

}
