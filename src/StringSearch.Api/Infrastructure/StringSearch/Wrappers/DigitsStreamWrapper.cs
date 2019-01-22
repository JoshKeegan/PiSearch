using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace StringSearch.Api.Infrastructure.StringSearch.Wrappers
{
    public class DigitsStreamWrapper : BaseStreamWrapper
    {
        public DigitsStreamWrapper(Stream stream) : base(stream) {  }
    }
}
