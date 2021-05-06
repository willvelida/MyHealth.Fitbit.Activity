using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace MyHealth.Fitbit.Activity.Models
{
    [ExcludeFromCodeCoverage]
    public class ActivityResponseObject
    {
        public List<object> activities { get; set; }
        public Goals goals { get; set; }
        public Summary summary { get; set; }
    }
}
