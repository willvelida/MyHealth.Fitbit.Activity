using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHealth.Fitbit.Activity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: FunctionsStartup(typeof(Startup))]
namespace MyHealth.Fitbit.Activity
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config);
            builder.Services.AddLogging();
            builder.Services.AddHttpClient();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
    }
}
