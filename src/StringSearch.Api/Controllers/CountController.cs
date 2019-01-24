using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Contracts;
using StringSearch.Api.Contracts.Searches.Counts;
using StringSearch.DataLayer;
using StringSearch.Legacy;
using StringSearch.LegacyWrappers;

namespace StringSearch.Api.Controllers
{
    [Route("api/Count")]
    public class CountController : Controller
    {
        private readonly DigitsWrapper digitsWrapper;
        private readonly SuffixArrayWrapper suffixArrayWrapper;
        private readonly IPrecomputedSearchResults precomputedSearchResults;
        private readonly IDbSearches dbSearches;

        public CountController(DigitsWrapper digitsWrapper, SuffixArrayWrapper suffixArrayWrapper,
            IPrecomputedSearchResults precomputedSearchResults, IDbSearches dbSearches)
        {
            this.digitsWrapper = digitsWrapper;
            this.suffixArrayWrapper = suffixArrayWrapper;
            this.precomputedSearchResults = precomputedSearchResults;
            this.dbSearches = dbSearches;
        }

        public IActionResult Index(CountRequest request)
        {
            SearchSummary summary = new SearchSummary(request.Find, null, null,
                null, true, null, Request.HttpContext.Connection.RemoteIpAddress);

            // Time the request being processed
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Do the search
            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArrayWrapper.SuffixArray,
                digitsWrapper.Digits, summary.Find, precomputedSearchResults.Results);

            // If there is a result
            CountResponse response = new CountResponse();
            if (suffixArrayRange.HasResults)
            {
                response.SuffixArrayMinIdx = suffixArrayRange.Min;
                response.SuffixArrayMaxIdx = suffixArrayRange.Max;
                response.NumResults = (int)(suffixArrayRange.Max - suffixArrayRange.Min + 1);
            }
            else
            {
                response.NumResults = 0;
            }

            stopwatch.Stop();
            summary.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
            response.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            // Log this search to the database. Defer until after the response is sent to the client
            Response.OnCompleted(async () =>
            {
                await dbSearches.Insert(summary);
            });

            return Ok(response);
        }
    }
}
