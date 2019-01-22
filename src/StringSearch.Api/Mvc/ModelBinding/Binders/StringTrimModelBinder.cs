using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace StringSearch.Api.Mvc.ModelBinding.Binders
{
    public class StringTrimModelBinder : IModelBinder
    {
        private readonly IModelBinder fallbackBinder;

        public StringTrimModelBinder(IModelBinder fallbackBinder)
        {
            this.fallbackBinder = fallbackBinder ?? throw new ArgumentNullException(nameof(fallbackBinder));
        }

        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            string result = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;
            if (!string.IsNullOrEmpty(result))
            {
                bindingContext.Result = ModelBindingResult.Success(result.Trim());
            }
            else
            {
                await fallbackBinder.BindModelAsync(bindingContext);
            }
        }
    }
}
