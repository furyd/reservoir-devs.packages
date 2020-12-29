using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ReservoirDevs.Correlation.Models;
using ReservoirDevs.Correlation.Tests.Integration.Setup;
using ReservoirDevs.Correlation.Tests.Integration.Setup.Constants;
using ReservoirDevs.Tests.Integration.Common.Factories;
using Xunit;

namespace ReservoirDevs.Correlation.Tests.Integration
{
    public class RequireCorrelationIdTests : IClassFixture<IntegrationTestingWebApplicationFactory<TestStartup>>
    {
        private const string Endpoint = "/test/get";

        private readonly IntegrationTestingWebApplicationFactory<TestStartup> _webApplicationFactory;

        public RequireCorrelationIdTests(IntegrationTestingWebApplicationFactory<TestStartup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Request_ReturnsUnprocessableEntity_WhenNoCorrelationIdHeaderPassed()
        {
            var client = _webApplicationFactory.CreateClient();

            var result = await client.GetAsync(Endpoint);

            result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public async Task Request_ReturnsUnprocessableEntity_WhenInvalidCorrelationIdHeaderPassed(string value)
        {
            var client = _webApplicationFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);
            
            request.Headers.Add(Headers.CorrelationToken, value);
            
            var result = await client.SendAsync(request);

            result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task Request_ReturnsOk_WhenValidCorrelationIdHeaderPassed()
        {
            var correlationToken = new CorrelationToken("A");
            
            var client = _webApplicationFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            request.Headers.Add(Headers.CorrelationToken, correlationToken);

            var result = await client.SendAsync(request);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await result.Content.ReadAsStringAsync();

            content.Should().BeEquivalentTo(correlationToken);
        }
    }
}