using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Validators;

namespace StringSearch.Di
{
    internal static class ValidatorsRegistrar
    {
        public static IServiceCollection AddValidators(this IServiceCollection services) =>
            services
                .AddSingleton<IFindValidator, FindValidator>()
                .AddSingleton<ILookupRequestValidator, LookupRequestValidator>()
                .AddSingleton<IOccurrenceLookupRequestValidator, OccurrenceLookupRequestValidator>();
    }
}
