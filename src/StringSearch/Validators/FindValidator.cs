using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Validators
{
    public class FindValidator : IFindValidator
    {
        public void ThrowIfInvalid(string find)
        {
            if (find == null)
            {
                throw new ArgumentNullException(nameof(find));
            }

            foreach (char c in find)
            {
                if (c < '0' || c > '9')
                {
                    throw new ArgumentException("Must only be characters 0-9", nameof(find));
                }
            }
        }
    }
}
