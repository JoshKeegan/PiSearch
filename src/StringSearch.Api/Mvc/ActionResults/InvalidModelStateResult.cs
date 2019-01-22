using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using StringSearch.Api.Contracts.BadRequests;

namespace StringSearch.Api.Mvc.ActionResults
{
    public class InvalidModelStateResult : ObjectResult
    {
        public InvalidModelStateResult(ModelStateDictionary modelState)
            : base(generateBadRequest(modelState))
        {
            StatusCode = 400;
        }

        private static BadRequest generateBadRequest(ModelStateDictionary modelState)
        {
            return new BadRequest(generateValidationErrors(modelState));
        }

        private static IDictionary<string, IEnumerable<string>> generateValidationErrors(ModelStateDictionary modelState)
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            if (modelState.IsValid)
            {
                throw new ArgumentException("Model State must be invalid", nameof(modelState));
            }

            Dictionary<string, IEnumerable<string>> validationErrors = new Dictionary<string, IEnumerable<string>>();
            foreach (KeyValuePair<string, ModelStateEntry> kvp in modelState)
            {
                string fieldName = kvp.Key;
                ModelErrorCollection errors = kvp.Value.Errors;
                if (errors != null && errors.Count > 0)
                {
                    validationErrors.Add(fieldName, errors.Select(e => e.ErrorMessage));
                }
            }
            return validationErrors;
        }
    }
}
