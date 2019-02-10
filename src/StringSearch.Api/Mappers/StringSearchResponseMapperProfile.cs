﻿using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using StringSearch.Api.Contracts.Searches.Counts;
using StringSearch.Api.Contracts.Searches.Lookups;
using StringSearch.Models;

namespace StringSearch.Api.Mappers
{
    public class StringSearchResponseMapperProfile : Profile
    {
        public StringSearchResponseMapperProfile()
        {
            CreateMap<SearchResult, CountResponseDto>().ForMember(dest => dest.ProcessingTimeMs, opt => opt.Ignore());
            CreateMap<LookupResult, LookupResponseDto>().ForMember(dest => dest.ProcessingTimeMs, opt => opt.Ignore());
        }
    }
}
