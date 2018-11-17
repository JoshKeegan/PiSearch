using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using StringSearch.Api.Mvc.ActionResults;

namespace StringSearch.Api.Mvc.ActionFilters
{
    /// <summary>
    /// Validates all models for every request.
    /// Prevents validation code from needing to exist within controllers.
    /// </summary>
    public class ValidateModelsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                context.Result = new InvalidModelStateResult(context.ModelState);
            }

            base.OnActionExecuting(context);
        }
    }
}
