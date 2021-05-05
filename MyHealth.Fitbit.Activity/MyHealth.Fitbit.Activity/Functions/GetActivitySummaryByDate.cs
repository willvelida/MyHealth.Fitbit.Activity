using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyHealth.Common;
using MyHealth.Fitbit.Activity.Services;
using mdl = MyHealth.Common.Models;

namespace MyHealth.Fitbit.Activity.Functions
{
    public class GetActivitySummaryByDate
    {
        private readonly IConfiguration _configuration;
        private readonly IFitbitApiService _fitbitApiService;
        private readonly IMapper _mapper;
        private readonly IServiceBusHelpers _serviceBusHelpers;

        public GetActivitySummaryByDate(
            IConfiguration configuration,
            IFitbitApiService fitbitApiService,
            IMapper mapper,
            IServiceBusHelpers serviceBusHelpers)
        {
            _configuration = configuration;
            _fitbitApiService = fitbitApiService;
            _mapper = mapper;
            _serviceBusHelpers = serviceBusHelpers;
        }

        [FunctionName(nameof(GetActivitySummaryByDate))]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            try
            {
                log.LogInformation($"{nameof(GetActivitySummaryByDate)} executed at: {DateTime.Now}");
                var dateParameter = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
                log.LogInformation($"Attempting to retrieve Daily Activity Summary for {dateParameter}");
                var activityResponse = await _fitbitApiService.GetActivityResponseObject(dateParameter);

                log.LogInformation("Mapping API response to Activity object");
                var activity = new mdl.Activity();
                _mapper.Map(activityResponse, activity);

                // Send mapped Activity Log to Service Bus
                log.LogInformation("Sending Activity log to service bus");
                await _serviceBusHelpers.SendMessageToTopic(_configuration["ActivityTopic"], activity);
            }
            catch (Exception ex)
            {
                log.LogError($"Exception thrown in {nameof(GetActivitySummaryByDate)}: {ex.Message}");
                // Send Exception to Queue
                await _serviceBusHelpers.SendMessageToQueue(_configuration["ExceptionQueue"], ex);
                throw ex;
            }
            
        }
    }
}
