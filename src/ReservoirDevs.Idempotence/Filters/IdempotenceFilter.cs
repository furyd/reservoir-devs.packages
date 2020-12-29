using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ReservoirDevs.Idempotence.Extensions;
using ReservoirDevs.Idempotence.Models;
using ReservoirDevs.Idempotence.Repositories.Interfaces;
using ReservoirDevs.Logging.Extensions;

namespace ReservoirDevs.Idempotence.Filters
{
    public class IdempotenceFilter : IAsyncActionFilter
    {
        private readonly string _tokenKey;

        private readonly IIdempotenceTokenRepository _idempotenceTokenRepository;
        private readonly ILogger<IdempotenceFilter> _logger;

        public IdempotenceFilter(IIdempotenceTokenRepository idempotenceTokenRepository, IdempotenceHeader tokenKey, ILogger<IdempotenceFilter> logger)
        {
            _idempotenceTokenRepository = idempotenceTokenRepository ?? throw new ArgumentNullException(nameof(idempotenceTokenRepository));
            _tokenKey = tokenKey?.ToString() ?? throw new ArgumentNullException(nameof(tokenKey));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (_logger.CreateScope(nameof(OnActionExecutionAsync)))
            {
                if (!HeaderExists(context) || !HeaderValueExists(context) || !await IsHeaderValueUnique(context))
                {
                    _logger.LogInformation($"{_tokenKey} header has failed validation");
                    return;
                }

                _logger.LogInformation($"{_tokenKey} header has passed validation");

                var token = context.GetHeaderValue(_tokenKey);

                _logger.LogInformation($"Add {token} to token repository");
                await _idempotenceTokenRepository.Create(token);
                await next();
            }
        }

        private async Task<bool> IsHeaderValueUnique(ActionExecutingContext context)
        {
            using (_logger.CreateScope(nameof(IsHeaderValueUnique)))
            {
                var token = context.GetHeaderValue(_tokenKey);

                var existingToken = await _idempotenceTokenRepository.Retrieve(token);

                if (existingToken == null)
                {
                    _logger.LogInformation($"{token} is unique");
                    return true;
                }

                _logger.LogInformation($"{token} is not unique");

                context.Result = new ConflictObjectResult(new ProblemDetails { Detail = "Token already used" });
                return false;
            }
        }

        private bool HeaderExists(ActionExecutingContext context) {

            using (_logger.CreateScope(nameof(HeaderExists)))
            {
                if (context.HttpContext.Request.Headers.ContainsKey(_tokenKey))
                {
                    _logger.LogInformation($"{_tokenKey} header exists");
                    return true;
                }

                _logger.LogInformation($"{_tokenKey} header does not exist");

                context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(new Dictionary<string, string[]> { { _tokenKey, new[] { $"Required header {_tokenKey} missing" } } }));
                return false;
            }   
        }

        private bool HeaderValueExists(ActionExecutingContext context)
        {
            using (_logger.CreateScope(nameof(HeaderValueExists)))
            {
                if (!string.IsNullOrWhiteSpace(context.GetHeaderValue(_tokenKey)))
                {
                    _logger.LogInformation($"{_tokenKey} header has value");
                    return true;
                }

                _logger.LogInformation($"{_tokenKey} header has no value");

                context.Result = new UnprocessableEntityObjectResult(new ValidationProblemDetails(new Dictionary<string, string[]> { { _tokenKey, new[] { $"Header {_tokenKey} value is missing" } } }));
                return false;
            }   
        }
    }
}
