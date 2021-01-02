using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReservoirDevs.Logging.Extensions;
using ReservoirDevs.Logging.Models;

namespace ReservoirDevs.Logging.MessageHandlers
{
    public class LoggingMessageHandler : DelegatingHandler
    {

        private readonly ILogger<LoggingMessageHandler> _logger;

        public LoggingMessageHandler(ILogger<LoggingMessageHandler> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Http Request Information:{await MapHttpRequest(request)}");

            var response = await base.SendAsync(request, cancellationToken);

            _logger.LogInformation($"Http Response Information:{await MapHttpResponse(response)}");

            return response;
        }

        private static async Task<HttpRequestModel> MapHttpRequest(HttpRequestMessage httpContext)
        {
            var model = new HttpRequestModel
            {
                Scheme = httpContext.RequestUri.Scheme,
                Host = httpContext.RequestUri.Host,
                Path = httpContext.RequestUri.AbsolutePath,
                Querystring = httpContext.RequestUri.Query,
                Headers = httpContext.Headers.Select(header => new KeyValuePair<string, string>(header.Key, header.Value.ToString("")))
            };

            if (httpContext.Content == null)
            {
                return model;
            }

            model.Body = await httpContext.Content.ReadAsStringAsync();
            return model;
        }

        private static async Task<HttpResponseModel> MapHttpResponse(HttpResponseMessage httpContext)
        {
            var headers = httpContext.Headers.Select(header => new KeyValuePair<string, string>(header.Key, header.Value.ToString(""))).ToList();

            if (httpContext.Content != null)
            {
                headers.AddRange(httpContext.Content.Headers.Select(header => new KeyValuePair<string, string>(header.Key, header.Value.ToString(""))));
            }

            var model = new HttpResponseModel
            {
                Headers = headers
            };

            if (httpContext.Content == null)
            {
                return model;
            }

            model.Body = await httpContext.Content.ReadAsStringAsync();

            return model;
        }
    }
}