using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models.WeeklyScheduleModels
{
    public class WeeklyScheduleReturnModel
    {
        public string? UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public List<WeeklyScheduleModel> Schedules { get; set; }
    }
}
