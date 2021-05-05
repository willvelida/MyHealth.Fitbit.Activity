using System;
using System.Collections.Generic;
using System.Text;

namespace MyHealth.Fitbit.Activity.Models
{
    public class ActivityResponseObject
    {
        public List<object> activities { get; set; }
        public Goals goals { get; set; }
        public Summary summary { get; set; }
    }
}
