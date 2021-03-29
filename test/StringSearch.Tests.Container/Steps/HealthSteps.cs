using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using StringSearch.Tests.Container.Orchestration;
using TechTalk.SpecFlow;

namespace StringSearch.Tests.Container.Steps
{
    [Binding]
    public class HealthSteps
    {
        private readonly HttpOrchestrator httpOrchestrator;

        public HealthSteps(HttpOrchestrator httpOrchestrator)
        {
            this.httpOrchestrator = httpOrchestrator;
        }

        [When(@"I request the system health")]
        public Task WhenIRequestTheSystemHealth() => httpOrchestrator.SendAsync("Health", HttpMethod.Get);

        [Then(@"all critical resources should be healthy")]
        // TODO: Needs contract access
        public Task ThenAllCriticalResourcesShouldBeHealthy() => throw new NotImplementedException();

        [Then(@"the HTTP Status code should be '(.*)'")]
        public void ThenTheHttpStatusCodeShouldBe(HttpStatusCode statusCode) =>
            httpOrchestrator.AssertStatusCode(statusCode);
    }
}
