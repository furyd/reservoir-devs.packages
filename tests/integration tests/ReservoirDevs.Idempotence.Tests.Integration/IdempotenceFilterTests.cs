using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using ReservoirDevs.Idempotence.Models;
using ReservoirDevs.Idempotence.Tests.Integration.Setup;
using ReservoirDevs.Idempotence.Tests.Integration.Setup.Constants;
using ReservoirDevs.Idempotence.Tests.Integration.Setup.Repositories;
using ReservoirDevs.Tests.Integration.Common.Factories;
using Xunit;

namespace ReservoirDevs.Idempotence.Tests.Integration
{
    public class IdempotenceFilterTests : IClassFixture<IntegrationTestingWebApplicationFactory<TestStartup>>
    {
        private const string Endpoint = "/test/get";

        private readonly IntegrationTestingWebApplicationFactory<TestStartup> _webApplicationFactory;

        public IdempotenceFilterTests(IntegrationTestingWebApplicationFactory<TestStartup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
        }

        [Fact]
        public async Task Request_ReturnsUnprocessableEntity_WhenNoIdempotenceIdHeaderPassed()
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
        public async Task Request_ReturnsUnprocessableEntity_WhenInvalidIdempotenceIdHeaderPassed(string value)
        {
            var client = _webApplicationFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            request.Headers.Add(Headers.IdempotenceToken, value);

            var result = await client.SendAsync(request);

            result.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task Request_ReturnsOk_WhenValidIdempotenceIdHeaderPassed()
        {
            await ClearData();
            
            var idempotenceToken = new IdempotenceToken("A");

            var client = _webApplicationFactory.CreateClient();

            var request = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            request.Headers.Add(Headers.IdempotenceToken, idempotenceToken);

            var result = await client.SendAsync(request);

            result.StatusCode.Should().Be(HttpStatusCode.OK);

            var content = await result.Content.ReadAsStringAsync();

            content.Should().BeEquivalentTo(idempotenceToken);
        }

        [Fact]
        public async Task Request_ReturnsConflict_WhenValidIdempotenceIdHeaderPassedTwice()
        {
            await ClearData();

            var idempotenceToken = new IdempotenceToken("A");

            var client = _webApplicationFactory.CreateClient();

            var firstRequest = new HttpRequestMessage(HttpMethod.Get, Endpoint);
            var secondRequest = new HttpRequestMessage(HttpMethod.Get, Endpoint);

            firstRequest.Headers.Add(Headers.IdempotenceToken, idempotenceToken);
            secondRequest.Headers.Add(Headers.IdempotenceToken, idempotenceToken);

            var firstResult = await client.SendAsync(firstRequest);
            var secondResult = await client.SendAsync(secondRequest);

            firstResult.StatusCode.Should().Be(HttpStatusCode.OK);
            secondResult.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        private async Task ClearData()
        {
            var repo = _webApplicationFactory.Services.GetService(typeof(InMemoryIdempotenceTokenRepository)) as InMemoryIdempotenceTokenRepository;

            if (repo == null)
            {
                throw new Exception("Repository is null");
            }
            
            repo.Tokens.RemoveRange(repo.Tokens);
            await repo.SaveChangesAsync();
        }
    }
}
