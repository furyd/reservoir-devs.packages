using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Moq;
using ReservoirDevs.Idempotence.DataTransferObjects;
using ReservoirDevs.Idempotence.Filters;
using ReservoirDevs.Idempotence.Models;
using ReservoirDevs.Idempotence.Repositories.Interfaces;
using Xunit;

namespace ReservoirDevs.Idempotence.Tests.Unit
{
    public class IdempotenceFilterTests
    {
        private readonly IdempotenceHeader _idempotenceHeader;
        private readonly Mock<ILogger<IdempotenceFilter>> _logger;

        public IdempotenceFilterTests()
        {
            _idempotenceHeader = new IdempotenceHeader("A");
            _logger = new Mock<ILogger<IdempotenceFilter>>();
        }

        private static ActionContext SetupActionContext(HttpContext httpContext)
        {
            return new ActionContext(
                httpContext,
                Mock.Of<RouteData>(),
                Mock.Of<ActionDescriptor>(),
                new ModelStateDictionary()
            );
        }

        private static ActionExecutingContext SetupActionExecutingContext(ActionContext actionContext)
        {
            return new ActionExecutingContext(
                actionContext,
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                Mock.Of<Controller>()
            )
            {
                Result = new OkResult() // It will return ok unless during code execution you change this when by condition
            };
        }

        [Fact]
        public async Task OnActionExecutionAsync_Returns_UnprocessableEntity_WhenIdempotenceHeaderMissing()
        {
            var httpContext = new DefaultHttpContext();

            var actionContext = SetupActionContext(httpContext);

            var actionExecutingContext = SetupActionExecutingContext(actionContext);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var sut = new IdempotenceFilter(new Mock<IIdempotenceTokenRepository>().Object, _idempotenceHeader, _logger.Object);

            await sut.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            actionExecutingContext.Result.Should().BeAssignableTo<UnprocessableEntityObjectResult>();
        }

        [Fact]
        public async Task OnActionExecutionAsync_Returns_UnprocessableEntity_WhenIdempotenceHeaderIsEmpty()
        {
            var httpContext = new DefaultHttpContext();
            
            httpContext.Request.Headers.Add(_idempotenceHeader.ToString(), StringValues.Empty);

            var actionContext = SetupActionContext(httpContext);

            var actionExecutingContext = SetupActionExecutingContext(actionContext);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var sut = new IdempotenceFilter(new Mock<IIdempotenceTokenRepository>().Object, _idempotenceHeader, _logger.Object);

            await sut.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            actionExecutingContext.Result.Should().BeAssignableTo<UnprocessableEntityObjectResult>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("    ")]
        public async Task OnActionExecutionAsync_Returns_UnprocessableEntity_WhenIdempotenceHeaderIsEmptyString(string value)
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers.Add(_idempotenceHeader.ToString(), new StringValues(value));

            var actionContext = SetupActionContext(httpContext);

            var actionExecutingContext = SetupActionExecutingContext(actionContext);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var sut = new IdempotenceFilter(new Mock<IIdempotenceTokenRepository>().Object, _idempotenceHeader, _logger.Object);

            await sut.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            actionExecutingContext.Result.Should().BeAssignableTo<UnprocessableEntityObjectResult>();
        }

        [Fact]
        public async Task OnActionExecutionAsync_Returns_Ok_WhenIdempotenceHeaderIsNonEmptyString()
        {
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Headers.Add(_idempotenceHeader.ToString(), new StringValues("abc"));

            var actionContext = SetupActionContext(httpContext);

            var actionExecutingContext = SetupActionExecutingContext(actionContext);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var sut = new IdempotenceFilter(new Mock<IIdempotenceTokenRepository>().Object, _idempotenceHeader, _logger.Object);

            await sut.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            actionExecutingContext.Result.Should().BeAssignableTo<OkResult>();
        }

        [Fact]
        public async Task OnActionExecutionAsync_Returns_Ok_WhenIdempotenceHeaderIsNonEmptyString2()
        {
            var httpContext = new DefaultHttpContext();

            var repository = new Mock<IIdempotenceTokenRepository>();

            repository.Setup(repo => repo.Retrieve(It.IsAny<string>())).ReturnsAsync(new IdempotenceTokenDTO());

            httpContext.Request.Headers.Add(_idempotenceHeader.ToString(), new StringValues("abc"));

            var actionContext = SetupActionContext(httpContext);

            var actionExecutingContext = SetupActionExecutingContext(actionContext);

            var context = new ActionExecutedContext(actionContext, new List<IFilterMetadata>(), Mock.Of<Controller>());

            var bob = new IdempotenceFilter(repository.Object, _idempotenceHeader, _logger.Object);

            await bob.OnActionExecutionAsync(actionExecutingContext, async () => await Task.FromResult(context));

            actionExecutingContext.Result.Should().BeAssignableTo<ConflictObjectResult>();
        }
    }
}
