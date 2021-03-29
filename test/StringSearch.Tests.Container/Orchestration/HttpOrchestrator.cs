using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
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

            Uri uri = getUri(path);

            testOutputHelper.WriteLine($"Making HTTP {method} request to {uri}");

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
            byte[] bArrContent = await response.Content.ReadAsByteArrayAsync();
            T model = JsonSerializer.Deserialize<T>(bArrContent);
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

            string strUri = $"{EnvState.ApiBaseUri}/api/{ApiVersion}/{path}";
            return new Uri(strUri);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}
