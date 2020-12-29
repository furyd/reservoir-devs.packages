using Microsoft.AspNetCore.Mvc;

namespace ReservoirDevs.Idempotence.Extensions
{
    public static class ActionContextExtensions
    {
        public static string GetHeaderValue(this ActionContext context, string key)
        {
            return context.HttpContext.Request.Headers[key].ToString();
        }
    }
}
