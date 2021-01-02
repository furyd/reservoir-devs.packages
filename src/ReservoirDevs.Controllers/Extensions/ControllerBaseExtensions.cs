using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace ReservoirDevs.Controllers.Extensions
{
    public static class ControllerBaseExtensions
    {
        public static async Task PassThrough(this ControllerBase controllerBase, HttpResponseMessage message)
        {
            foreach (var (key, value) in message.Content.Headers)
            {
                controllerBase.Response.Headers.Add(key, new StringValues(value.ToArray()));
            }

            controllerBase.Response.StatusCode = (int)message.StatusCode;

            await message.Content.CopyToAsync(controllerBase.Response.Body);
        }
    }
}