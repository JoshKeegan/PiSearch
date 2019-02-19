﻿using System;
using System.Collections.Generic;
using System.Text;

namespace StringSearch.Models
{
    public class LookupResult : SearchResult
    {
        public int? ResultId;
        public long? ResultStringIdx;
        public SurroundingDigits SurroundingDigits;
    }
}
