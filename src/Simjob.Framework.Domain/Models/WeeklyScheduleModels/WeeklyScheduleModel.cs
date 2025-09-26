using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simjob.Framework.Domain.Models.WeeklyScheduleModels
{
    public class WeeklyScheduleModel
    {
        public string? Id  { get; set; }
        public string? Codigo  { get; set; }
        public string? UsuarioId { get; set; }
        public string? UsuarioNome { get; set; }
        public string? FornecedorId { get; set; }
        public string? FornecedorNome { get; set; }
        public DateTime? Data { get; set; }
        public List<string>? Owners { get; set; }
        public bool? IsDeleted { get; set; }
    
    }
}
