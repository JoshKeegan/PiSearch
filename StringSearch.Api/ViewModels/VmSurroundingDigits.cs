using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.ViewModels
{
    public class VmSurroundingDigits
    {
        public string Before { get; }
        public string After { get; }

        public VmSurroundingDigits(string before, string after)
        {
            Before = before;
            After = after;
        }
    }
}
