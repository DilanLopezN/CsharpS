
using System.Collections.Generic;

namespace Simjob.Framework.Services.Api.Models.Matricula
{
    public class MatriculaTitulosModel
    {
        public int cd_contrato { get; set; }
        public bool? Desconto { get; set; }
        public bool? Bolsa { get; set; }
        public List<TituloModel>? TitulosMensalidade { get; set; }
        public List<TituloModel>? TitulosMaterial { get; set; }
    }
}
