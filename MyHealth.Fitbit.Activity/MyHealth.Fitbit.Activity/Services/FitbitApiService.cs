using Microsoft.Extensions.Configuration;
using MyHealth.Common;
using MyHealth.Fitbit.Activity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MyHealth.Fitbit.Activity.Services
{
    public class FitbitApiService : IFitbitApiService
    {
        private readonly IConfiguration _configuration;
        private readonly IKeyVaultHelper _keyVaultHelper;
        private readonly HttpClient _httpClient;

        public FitbitApiService(
            IConfiguration configuration,
            IKeyVaultHelper keyVaultHelper,
            HttpClient httpClient)
        {
            _configuration = configuration;
            _keyVaultHelper = keyVaultHelper;
            _httpClient = httpClient;
        }

        public async Task<ActivityResponseObject> GetActivityResponseObject(string date)
        {
            try
            {
                var fitbitAccessToken = await _keyVaultHelper.RetrieveSecretFromKeyVaultAsync(_configuration["AccessTokenName"]);
                _httpClient.DefaultRequestHeaders.Clear();
                Uri getDailyActivityLogUri = new Uri($"https://api.fitbit.com/1/user/-/activities/date/{date}.json");
                var request = new HttpRequestMessage(HttpMethod.Get, getDailyActivityLogUri);
                request.Content = new StringContent("");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", fitbitAccessToken.Value);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var activityResponse = JsonConvert.DeserializeObject<ActivityResponseObject>(responseString);

                return activityResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
