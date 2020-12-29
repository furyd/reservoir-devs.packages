using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace ReservoirDevs.Tests.Integration.Common.Factories
{
    public class IntegrationTestingWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override IHostBuilder CreateHostBuilder() => Host.CreateDefaultBuilder();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseTestServer();
            builder.UseStartup<TStartup>();
            builder.UseContentRoot(Directory.GetCurrentDirectory());
            base.ConfigureWebHost(builder);
        }
    }
}
