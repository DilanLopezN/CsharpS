using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simjob.Framework.Services.Api.Attributes
{
    public class RequiresTwoStepAuthenticationAttribute : Attribute
    {
        public bool RequireState { get; set; }

        public RequiresTwoStepAuthenticationAttribute(bool requireState)
        {
            this.RequireState = requireState;
        }
    }
}
