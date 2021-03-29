using StringSearch.Tests.Container.Orchestration;
using TechTalk.SpecFlow;

namespace StringSearch.Tests.Container.Steps
{
    [Binding]
    public class HttpSteps
    {
        private readonly HttpOrchestrator httpOrchestrator;

        public HttpSteps(HttpOrchestrator httpOrchestrator)
        {
            this.httpOrchestrator = httpOrchestrator;
        }

        [Given(@"the API version is '(.*)'")]
        public void GivenTheApiVersionIs(string version) => httpOrchestrator.ApiVersion = version;
    }
}
