using Microsoft.AspNetCore.Mvc;
using ReservoirDevs.Idempotence.Filters;

namespace ReservoirDevs.Idempotence.Tests.Unit.SupportingClasses
{
    public class TestController : ControllerBase
    {
        public IActionResult TestMethodNoServiceFilter() => NoContent();

        [ServiceFilter(typeof(string))]
        public IActionResult TestMethodNoIdempotenceFilter() => NoContent();

        [ServiceFilter(typeof(IdempotenceFilter))]
        public IActionResult TestMethod() => NoContent();
    }
}