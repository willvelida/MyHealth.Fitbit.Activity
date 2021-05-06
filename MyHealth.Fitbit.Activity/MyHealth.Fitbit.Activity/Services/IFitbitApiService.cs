using MyHealth.Fitbit.Activity.Models;
using System.Threading.Tasks;

namespace MyHealth.Fitbit.Activity.Services
{
    public interface IFitbitApiService
    {
        Task<ActivityResponseObject> GetActivityResponseObject(string date);
    }
}
