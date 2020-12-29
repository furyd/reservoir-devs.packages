using Microsoft.AspNetCore.Mvc;
using ReservoirDevs.Correlation.Tests.Integration.Setup.Constants;

namespace ReservoirDevs.Correlation.Tests.Integration.Setup.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        [Route("get")]
        public IActionResult Get([FromHeader(Name = Headers.CorrelationToken)] string correlationToken) => Ok(correlationToken);
    }
}