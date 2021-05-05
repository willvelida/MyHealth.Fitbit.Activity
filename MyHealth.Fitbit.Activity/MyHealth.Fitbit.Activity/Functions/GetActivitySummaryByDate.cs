using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MyHealth.Fitbit.Activity.Functions
{
    public class GetActivitySummaryByDate
    {
        private readonly IConfiguration _configuration;

        public GetActivitySummaryByDate(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [FunctionName(nameof(GetActivitySummaryByDate))]
        public void Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"{nameof(GetActivitySummaryByDate)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                log.LogInformation($"Attempting to retrieve Daily Activity Summary for {dateParameter}");

                // Call Activity Summary

                // Map API Response to Activity Object

                // Send mapped Activity Log to Service Bus
            }
            catch (Exception ex)
            {
                log.LogError($"Exception thrown in {nameof(GetActivitySummaryByDate)}: {ex.Message}");
                // Send Exception to Queue
                throw ex;
            }
            
        }
    }
}
