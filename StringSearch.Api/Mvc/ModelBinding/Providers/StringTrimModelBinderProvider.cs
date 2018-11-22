using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using StringSearch.Api.Attributes.Binding;
using StringSearch.Api.Mvc.ModelBinding.Binders;

namespace StringSearch.Api.Mvc.ModelBinding.Providers
{
    /// <summary>
    /// Trims any string with a <see cref="TrimAttribute"/>
    /// </summary>
    public class StringTrimModelBinderProvider : IModelBinderProvider
    {
        private readonly ILoggerFactory loggerFactory;

        public StringTrimModelBinderProvider(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
        }

        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            // Cast to default implementation so that we can access attributes - see https://github.com/aspnet/Mvc/issues/6545#issuecomment-319150631
            DefaultModelMetadata defaultModelMetadata = (DefaultModelMetadata) context.Metadata;

            if (!context.Metadata.IsComplexType && 
                context.Metadata.ModelType == typeof(string) &&
                defaultModelMetadata.Attributes.Attributes.Any(a => a is TrimAttribute))
            {
                return new StringTrimModelBinder(new SimpleTypeModelBinder(context.Metadata.ModelType, loggerFactory));
            }
            return null;
        }
    }
}
