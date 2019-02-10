using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Validators
{
    public interface IFindValidator
    {
        void ThrowIfInvalid(string find);
    }
}
