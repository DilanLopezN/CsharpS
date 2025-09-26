using System;

namespace Simjob.Framework.Services.Api.Entities
{
    public class ActionSchedule : Action
    {
        public DateTime NextActionRun { get; set; }
        public bool IsExecuted { get; set; }
        public int Timer { get; set; }
        public string ActionId { get; set; }
    }
}
