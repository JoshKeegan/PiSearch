using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using StringSearch.Api.Di;

namespace UnitTests.Api
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
