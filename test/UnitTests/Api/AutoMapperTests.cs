using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using StringSearch.Api.Di;
using Xunit;

namespace UnitTests.Api
{
    public class AutoMapperTests
    {
        [Fact]
        public void ConfigurationIsValid()
        {
            IServiceCollection services = new ServiceCollection();
            services.RegisterApiDependencies();

            IServiceProvider provider = services.BuildServiceProvider();

            IMapper mapper = provider.GetService<IMapper>();
            mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
