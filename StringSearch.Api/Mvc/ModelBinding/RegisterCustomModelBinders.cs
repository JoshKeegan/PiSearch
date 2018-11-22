using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;
using StringSearch.Api.Mvc.ModelBinding.Providers;

namespace StringSearch.Api.Mvc.ModelBinding
{
    public static class RegisterCustomModelBinders
    {
        public static void RegisterAllCustomModelBinders(this MvcOptions options, ILoggerFactory loggerFactory)
        {
            options.registerStringTrimModelBinder(loggerFactory);
        }

        private static void registerStringTrimModelBinder(this MvcOptions options, ILoggerFactory loggerFactory)
        {
            // Find the index of the existing Simple Type binder
            int idxSimpleTypeBinder = -1;
            for (int i = 0; i < options.ModelBinderProviders.Count; i++)
            {
                if (options.ModelBinderProviders[i].GetType() == typeof(SimpleTypeModelBinderProvider))
                {
                    idxSimpleTypeBinder = i;
                    break;
                }
            }

            // If the Simple Type binder is found, add more before it
            if (idxSimpleTypeBinder != -1)
            {
                options.ModelBinderProviders.Insert(idxSimpleTypeBinder,
                    new StringTrimModelBinderProvider(loggerFactory));
            }
        }
    }
}
