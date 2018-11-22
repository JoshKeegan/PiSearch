using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StringSearch.Api.Mvc.ModelBinding.Providers;

namespace StringSearch.Api.Attributes.Binding
{
    /// <summary>
    /// Trims the string.
    /// Requires the <see cref="StringTrimModelBinderProvider"/> to be registered.
    /// </summary>
    public class TrimAttribute : Attribute {  }
}
