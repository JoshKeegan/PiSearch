using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Contracts.Health
{
    public class HealthResourceDto
    {
        public string Name { get; set; }
        public bool Critical { get; set; }
    }
}
