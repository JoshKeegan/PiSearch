using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using StringSearch.Api.Mvc.NewtonsoftJson.Converters;

namespace StringSearch.Api.Contracts.BadRequests
{
    public class ValidationErrors : Dictionary<string, IEnumerable<string>>
    {
    }
}
