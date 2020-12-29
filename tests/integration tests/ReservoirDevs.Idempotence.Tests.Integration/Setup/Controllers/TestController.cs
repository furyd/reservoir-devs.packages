using Microsoft.AspNetCore.Mvc;
using ReservoirDevs.Idempotence.Filters;
using ReservoirDevs.Idempotence.Tests.Integration.Setup.Constants;

namespace ReservoirDevs.Idempotence.Tests.Integration.Setup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [Route("get")]
        [ServiceFilter(typeof(IdempotenceFilter))]
        public IActionResult Get([FromHeader(Name = Headers.IdempotenceToken)] string idempotenceToken) => Ok(idempotenceToken);
    }
}