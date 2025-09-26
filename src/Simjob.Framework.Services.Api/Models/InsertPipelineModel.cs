namespace Simjob.Framework.Services.Api.Models
{
    public class InsertPipelineModel
    {
        public int cd_contato { get; set; }
        public int cd_empresa { get; set; }
        public string? Curso { get; set; }
        public int? cd_usuario { get; set; }
        public int? nm_resultado_teste { get; set; }
        public bool? manual { get; set; }

    }
}
