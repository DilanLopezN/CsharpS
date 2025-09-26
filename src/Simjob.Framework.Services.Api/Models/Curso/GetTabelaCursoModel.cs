using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Simjob.Framework.Services.Api.Models.Curso
{
    public class GetTabelaCursoModel
    {
        [JsonProperty("page_number")]
        public int PageNumber { get; set; } = 1;
        
        [JsonProperty("page_size")]
        public int PageSize { get; set; } = 10;
        
        [JsonProperty("sort_field")]
        public string SortField { get; set; }
        
        [JsonProperty("sort_order")]
        public string SortOrder { get; set; } = "asc";

        [Required(ErrorMessage = "cd_pessoa_escola é obrigatório")]
        [JsonProperty("cd_pessoa_escola")]
        public int CdPessoaEscola { get; set; }

        [Required(ErrorMessage = "cd_curso é obrigatório")]
        [JsonProperty("cd_curso")]
        public int CdCurso { get; set; }

        [Required(ErrorMessage = "cd_duracao é obrigatório")]
        [JsonProperty("cd_duracao")]
        public int CdDuracao { get; set; }

        [Required(ErrorMessage = "cd_regime é obrigatório")]
        [JsonProperty("cd_regime")]
        public int CdRegime { get; set; }

        [JsonProperty("dt_tabela")]
        public DateTime? DtTabela { get; set; }
    }
}
