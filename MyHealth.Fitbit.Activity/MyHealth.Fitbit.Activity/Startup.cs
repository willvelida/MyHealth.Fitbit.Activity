using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHealth.Common;
using MyHealth.Fitbit.Activity;
using MyHealth.Fitbit.Activity.Services;
using System;
using System.IO;

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

            builder.Services.AddSingleton<IServiceBusHelpers>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                return new ServiceBusHelpers(configuration["ServiceBusConnectionString"]);
            });
            builder.Services.AddSingleton<IKeyVaultHelper>(sp =>
            {
                IConfiguration configuration = sp.GetService<IConfiguration>();
                return new KeyVaultHelper(configuration["KeyVaultName"]);
            });
            builder.Services.AddScoped<IFitbitApiService, FitbitApiService>();
        }
    }
}
