using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ReservoirDevs.Logging.Tests.Unit
{
    public class MockHandler : DelegatingHandler
    {
        private readonly HttpResponseMessage _responseMessage;

        public MockHandler(HttpResponseMessage responseMessage)
        {
            _responseMessage = responseMessage;
        }
        
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => Task.FromResult(_responseMessage);
    }
}