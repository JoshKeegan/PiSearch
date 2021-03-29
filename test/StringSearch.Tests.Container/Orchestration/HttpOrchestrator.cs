using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Infrastructure;

namespace StringSearch.Tests.Container.Orchestration
{
    public class HttpOrchestrator : IDisposable
    {
        private readonly ISpecFlowOutputHelper testOutputHelper;
        private readonly HttpClient httpClient;
        private HttpResponseMessage response;

        public string ApiVersion;

        public HttpOrchestrator(ISpecFlowOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
            httpClient = new HttpClient();
        }

        public async Task SendAsync(string path, HttpMethod method, HttpContent content = null)
        {
            if (ApiVersion == null)
            {
                throw new InvalidOperationException(nameof(ApiVersion) + " must be set");
            }

            path = $"/{ApiVersion}/{path}";

            Uri uri = getUri(path);

            HttpRequestMessage request = new HttpRequestMessage(method, uri)
            {
                Content = content
            };

            response = await httpClient.SendAsync(request);

            await logResponse();
        }

        public void AssertStatusCode(HttpStatusCode statusCode) => response.StatusCode.Should().Be(statusCode);

        public async Task AssertContent<T>(Action<T> assertion)
        {
            string strContent = await response.Content.ReadAsStringAsync();
            T model = JsonConvert.DeserializeObject<T>(strContent);
            assertion(model);
        }

        private async Task logResponse()
        {
            string strBody = await response.Content.ReadAsStringAsync();
            testOutputHelper.WriteLine($"Received HTTP Status Code {response.StatusCode}, with body:\n{strBody}");
        }

        private Uri getUri(string path)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!path.StartsWith('/'))
            {
                path = '/' + path;
            }

            string strUri = $"{EnvState.ApiBaseUri}/api/{ApiVersion}/{path}";
            return new Uri(strUri);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
