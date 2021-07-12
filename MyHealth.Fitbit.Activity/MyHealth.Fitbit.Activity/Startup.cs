using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyHealth.Common;
using MyHealth.Fitbit.Activity;
using MyHealth.Fitbit.Activity.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.IO;
using System.Net.Http;

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
            builder.Services.AddHttpClient<IFitbitApiService, FitbitApiService>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(15))
                .AddPolicyHandler(GetRetryPolicy());
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }
}
