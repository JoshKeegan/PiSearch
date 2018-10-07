using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Api.Attributes.Validation;

namespace StringSearch.Api.ViewModels
{
    public abstract class VmSearchRequest
    {
        [Required]
        [RegularExpression("[0-9]+")]
        [StringLength(1000)]
        public string Find { get; set; }

        /*
         * TODO:
         * - Trim string "Find"
         */
    }
}
