using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Contracts.Health
{
    public class HealthStateDto
    {
        public bool Healthy { get; set; }
        public string Message { get; set; }
    }
}
