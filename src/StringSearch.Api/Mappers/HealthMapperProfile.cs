using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using StringSearch.Api.Contracts.Health;
using StringSearch.Health;

namespace StringSearch.Api.Mappers
{
    public class HealthMapperProfile : Profile
    {
        public HealthMapperProfile()
        {
            CreateMap<HealthServiceSummary, HealthCheckResponseDto>();
            CreateMap<HealthResourceSummary, HealthResourceSummaryDto>();
            CreateMap<IHealthResource, HealthResourceDto>();
            CreateMap<HealthState, HealthStateDto>();
        }
    }
}
