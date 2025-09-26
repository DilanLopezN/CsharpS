using System;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class MatriculaUpdateInfoModel
    {
        public int cd_contrato { get; set; }
        public int? id_tipo_matricula { get; set; }
        public DateTime? dt_inicial_contrato { get; set; }
        public string? id_retorno { get; set; }
        public DateTime? dt_final_contrato { get; set; }
        public int? id_transferencia { get; set; } 
    }
}
