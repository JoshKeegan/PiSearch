using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StringSearch.Api.Mvc.NewtonsoftJson.Converters;

namespace StringSearch.Api.Contracts.BadRequests
{
    public class BadRequest
    {
        [JsonConverter(typeof(DictionaryKeysCamelCaseConverter))]
        public ValidationErrors ValidationErrors;

        public BadRequest(ValidationErrors validationErrors)
        {
            ValidationErrors = validationErrors ?? throw new ArgumentNullException(nameof(validationErrors));
        }
    }
}
