using AutoMapper;
using MyHealth.Fitbit.Activity.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using mdl = MyHealth.Common.Models;

namespace MyHealth.Fitbit.Activity.Profiles
{
    [ExcludeFromCodeCoverage]
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<ActivityResponseObject, mdl.Activity>();
        }
    }
}
