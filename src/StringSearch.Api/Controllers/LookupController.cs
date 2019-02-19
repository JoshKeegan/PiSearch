using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Contracts.Searches.Lookups;
using StringSearch.DataLayer;
using StringSearch.Models;
using StringSearch.Services;

namespace StringSearch.Api.Controllers
{
    [Route("api/v1/Lookup")]
    public class LookupController : Controller
    {
        private readonly IStringSearchServices stringSearchServices;
        private readonly IDbSearches dbSearches;
        private readonly IMapper mapper;

        public LookupController(
            IStringSearchServices stringSearchServices,
            IDbSearches dbSearches,
            IMapper mapper)
        {
            this.stringSearchServices = stringSearchServices;
            this.dbSearches = dbSearches;
            this.mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index(LookupRequestDto requestDto)
        {
            SearchSummary summary = new SearchSummary(requestDto.Find, requestDto.ResultId, requestDto.MinSuffixArrayIdx,
                requestDto.MaxSuffixArrayIdx, false, requestDto.NumSurroundingDigits,
                Request.HttpContext.Connection.RemoteIpAddress, requestDto.NamedDigits);

            // Time the requestDto being processed
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Do the search
            LookupRequest request = mapper.Map<LookupRequest>(requestDto);
            LookupResult result = stringSearchServices.Lookup(request);
            LookupResponseDto responseDto = mapper.Map<LookupResponseDto>(result);

            stopwatch.Stop();
            summary.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
            responseDto.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            // Log this search to the database. Defer until after the response is sent to the client
            Response.OnCompleted(async () =>
            {
                await dbSearches.Insert(summary);
            });

            return Ok(responseDto);
        }
    }
}
