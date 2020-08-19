using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using optima_tracking.Data;

[assembly: FunctionsStartup(typeof(optima_tracking.Startup))]
namespace optima_tracking
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddSingleton<IUnitDatastore>((s) => {
                return new SqlUnitDataStore(Environment.GetEnvironmentVariable("sqldb_connection"));
            });

            builder.Services.AddSingleton<Scraper, Scraper>();
        }
    }
}
