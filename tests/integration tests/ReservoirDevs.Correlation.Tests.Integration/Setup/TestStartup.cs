using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservoirDevs.Correlation.Extensions;
using ReservoirDevs.Correlation.Models;
using ReservoirDevs.Correlation.Tests.Integration.Setup.Constants;

namespace ReservoirDevs.Correlation.Tests.Integration.Setup
{
    public class TestStartup
    {
        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddApplicationPart(typeof(TestStartup).Assembly);
            services.AddSingleton(new CorrelationHeader(Headers.CorrelationToken));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.RequireCorrelationId();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
