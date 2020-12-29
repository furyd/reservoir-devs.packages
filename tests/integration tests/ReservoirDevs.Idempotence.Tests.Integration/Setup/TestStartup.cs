using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReservoirDevs.Idempotence.Filters;
using ReservoirDevs.Idempotence.Models;
using ReservoirDevs.Idempotence.Repositories.Interfaces;
using ReservoirDevs.Idempotence.Tests.Integration.Setup.Constants;
using ReservoirDevs.Idempotence.Tests.Integration.Setup.Repositories;

namespace ReservoirDevs.Idempotence.Tests.Integration.Setup
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
            const string databaseName = "idempotenceTokens";


            services.AddControllers().AddApplicationPart(typeof(TestStartup).Assembly);
            services.AddSingleton(new IdempotenceHeader(Headers.IdempotenceToken));
            services.AddScoped<IdempotenceFilter>();
            services.AddDbContext<InMemoryIdempotenceTokenRepository>(options => options.UseInMemoryDatabase(databaseName));
            services.AddTransient<IIdempotenceTokenRepository, InMemoryIdempotenceTokenRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public virtual void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
