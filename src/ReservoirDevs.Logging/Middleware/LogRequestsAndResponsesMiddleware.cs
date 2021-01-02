using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IO;
using ReservoirDevs.Logging.Extensions;
using ReservoirDevs.Logging.Models;

namespace ReservoirDevs.Logging.Middleware
{
    public class LogRequestsAndResponsesMiddleware
    {
        private const int ReadChunkBufferLength = 4096;

        private readonly RequestDelegate _next;
        private readonly ILogger<LogRequestsAndResponsesMiddleware> _logger;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;
        public LogRequestsAndResponsesMiddleware(RequestDelegate next, ILogger<LogRequestsAndResponsesMiddleware> logger)
        {
            _next = next;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }
        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            
            await using (var requestStream = _recyclableMemoryStreamManager.GetStream())
            {
                await context.Request.Body.CopyToAsync(requestStream);

                _logger.LogInformation($"Http Request Information:{await MapHttpRequest(context, requestStream)}");

                context.Request.Body.Position = 0;
            }   
        }

        private async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            await using (var responseBody = _recyclableMemoryStreamManager.GetStream())
            {
                context.Response.Body = responseBody;
                
                await _next(context);
                
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation($"Http Response Information:{await MapHttpResponse(context, context.Response.Body)}");

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                
                await responseBody.CopyToAsync(originalBodyStream);
            }   
        }

        private static async Task<HttpRequestModel> MapHttpRequest(HttpContext httpContext, Stream stream)
        {
            var model = new HttpRequestModel
            {
                Scheme = httpContext.Request.Scheme,
                Host = httpContext.Request.Host.ToString(),
                Path = httpContext.Request.Path.ToString(),
                Querystring = httpContext.Request.QueryString.ToString(),
                Headers = httpContext.Request.Headers.Select(header => new KeyValuePair<string, string>(header.Key, header.Value.ToString("")))
            };

            if (!httpContext.Request.ContentLength.HasValue || httpContext.Request.ContentLength.Value == 0)
            {
                return model;
            }

            model.Body = await stream.ReadInChunks(ReadChunkBufferLength);
            return model;
        }

        private static async Task<HttpResponseModel> MapHttpResponse(HttpContext httpContext, Stream stream)
        {
            var headers = httpContext.Response.Headers.Select(header => new KeyValuePair<string, string>(header.Key, header.Value.ToString(""))).ToList();

            var model = new HttpResponseModel
            {
                Headers = headers
            };

            if (httpContext.Response.Body == null || !httpContext.Response.ContentLength.HasValue || httpContext.Response.ContentLength.Value == 0)
            {
                return model;
            }
            
            model.Body = await stream.ReadInChunks(ReadChunkBufferLength);

            return model;
        }
    }
}