using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReservoirDevs.Correlation.Models;
using ReservoirDevs.Logging.Extensions;

namespace ReservoirDevs.Correlation.Middleware
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _key;
        private readonly ILogger<CorrelationIdMiddleware> _logger;

        public CorrelationIdMiddleware(RequestDelegate next, CorrelationHeader key, ILogger<CorrelationIdMiddleware> logger)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            
            _next = next;
            _key = key;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (_logger.CreateScope(nameof(Invoke)))
            {
                if (!await HeaderExists(httpContext).ConfigureAwait(false) || !await HeaderValueExists(httpContext).ConfigureAwait(false))
                {
                    _logger.LogInformation($"{_key} header has failed validation");
                    return;
                }

                await _next(httpContext);
            }
        }

        private Task<bool> HeaderExists(HttpContext context)
        {
            using (_logger.CreateScope(nameof(HeaderExists)))
            {
                if (context.Request.Headers.ContainsKey(_key))
                {
                    _logger.LogInformation($"{_key} header exists");
                    return Task.FromResult(true);
                }

                _logger.LogInformation($"{_key} header does not exist");

                SerializeResponse(context, CreateValidationResponseForHeader($"Required header {_key} missing"));

                return Task.FromResult(false);
            }   
        }

        private Task<bool> HeaderValueExists(HttpContext context)
        {
            using (_logger.CreateScope(nameof(HeaderValueExists)))
            {
                if (!string.IsNullOrWhiteSpace(context.Request.Headers[_key].ToString()))
                {
                    _logger.LogInformation($"{_key} header has value");
                    return Task.FromResult(true);
                }

                _logger.LogInformation($"{_key} header has no value");

                SerializeResponse(context, CreateValidationResponseForHeader($"Header {_key} value is missing"));

                return Task.FromResult(false);
            }   
        }
        
        private Task SerializeResponse<T>(HttpContext context, T response) where T : ProblemDetails
        {
            using (_logger.CreateScope(nameof(SerializeResponse)))
            {
                _logger.LogInformation($"Serializing {typeof(T)} response");
                
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                context.Response.ContentType = "application/json";
                return JsonSerializer.SerializeAsync(context.Response.Body, response);
            }   
        }

        private ValidationProblemDetails CreateValidationResponseForHeader(string message) => CreateValidationResponse(_key, message);

        private static ValidationProblemDetails CreateValidationResponse(string key, string value) => CreateValidationResponse(new Dictionary<string, string[]> { { key, new[] { value } } });

        private static ValidationProblemDetails CreateValidationResponse(IDictionary<string, string[]> items) => new ValidationProblemDetails(items);

    }
}