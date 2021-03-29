using System;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StringSearch.Api.Di;

namespace StringSearch.Tests.Unit.Api
{
    [TestFixture]
    public class AutoMapperTests
    {
        [Test]
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
