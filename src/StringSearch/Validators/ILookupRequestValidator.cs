using System;
using System.Collections.Generic;
using System.Text;
using StringSearch.Models;

namespace StringSearch.Validators
{
    public interface ILookupRequestValidator
    {
        void ThrowIfInvalid(LookupRequest request);
    }
}
