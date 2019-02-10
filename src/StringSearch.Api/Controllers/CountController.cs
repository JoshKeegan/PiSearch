using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StringSearch.Api.Contracts.Searches.Counts;
using StringSearch.DataLayer;
using StringSearch.Models;
using StringSearch.Services;

namespace StringSearch.Api.Controllers
{
    [Route("api/v1/Count")]
    public class CountController : Controller
    {
        private readonly IStringSearchServices stringSearchServices;
        private readonly IDbSearches dbSearches;
        private readonly IMapper mapper;

        public CountController(
            IStringSearchServices stringSearchServices,
            IDbSearches dbSearches,
            IMapper mapper)
        {
            this.stringSearchServices = stringSearchServices;
            this.dbSearches = dbSearches;
            this.mapper = mapper;
        }

        public IActionResult Index(CountRequestDto requestDto)
        {
            SearchSummary summary = new SearchSummary(requestDto.Find, null, null,
                null, true, null, Request.HttpContext.Connection.RemoteIpAddress, requestDto.NamedDigits);

            // Time the requestDto being processed
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Do the search
            SearchResult result = stringSearchServices.Count(requestDto.NamedDigits, requestDto.Find);
            CountResponseDto responseDto = mapper.Map<CountResponseDto>(result);

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
