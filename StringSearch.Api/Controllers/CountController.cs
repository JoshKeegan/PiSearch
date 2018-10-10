using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Infrastructure.DataLayer;
using StringSearch.Api.Infrastructure.StringSearch;
using StringSearch.Api.Infrastructure.StringSearch.Wrappers;
using StringSearch.Api.Search;
using StringSearch.Api.ViewModels;

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

        public async Task<IActionResult> Index(VmCountRequest request)
        {
            if (!ModelState.IsValid)
            {
                // TODO: Return more user-friendly validation errors
                return StatusCode(500, ModelState);
            }

            SearchSummary summary = new SearchSummary(request.Find, null, null,
                null, true, null, Request.HttpContext.Connection.RemoteIpAddress);

            // Time the request being processed
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Do the search
            SuffixArrayRange suffixArrayRange = SearchString.Search(suffixArrayWrapper.SuffixArray,
                digitsWrapper.Digits, summary.Find, precomputedSearchResults.Results);

            // If there is a result
            VmCountResult vmRes = new VmCountResult();
            if (suffixArrayRange.HasResults)
            {
                vmRes.SuffixArrayMinIdx = suffixArrayRange.Min;
                vmRes.SuffixArrayMaxIdx = suffixArrayRange.Max;
                vmRes.NumResults = (int)(suffixArrayRange.Max - suffixArrayRange.Min + 1);
            }
            else
            {
                vmRes.NumResults = 0;
            }

            stopwatch.Stop();
            summary.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
            vmRes.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            // Log this search to the database
            // TODO: This could happen after the response is sent to the client
            await dbSearches.Insert(summary);

            return Ok(vmRes);
        }
    }
}
