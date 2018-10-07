using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Attributes.Validation
{
    public class PositiveLongAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            // Allow no value. Can be validated separately with [Required]
            if (value == null)
            {
                return true;
            }

            return long.TryParse(value.ToString(), out long longVal) && longVal >= 0;
        }
    }
}
