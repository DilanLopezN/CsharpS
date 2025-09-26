using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Simjob.Framework.Services.Api.Models.Curso
{
    public class TabelaCursoResponseModel
    {
        [JsonProperty("total_records")]
        public int TotalRecords { get; set; }
        
        [JsonProperty("page_number")]
        public int PageNumber { get; set; }
        
        [JsonProperty("page_size")]
        public int PageSize { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("succeeded")]
        public bool Succeeded { get; set; }
        
        [JsonProperty("data")]
        public List<TabelaCursoData> Data { get; set; }
    }

    public class TabelaCursoData
    {
        [JsonProperty("cd_tabela_preco")]
        public int? CdTabelaPreco { get; set; }
        
        [JsonProperty("cd_curso")]
        public int? CdCurso { get; set; }
        
        [JsonProperty("cd_duracao")]
        public int? CdDuracao { get; set; }
        
        [JsonProperty("cd_regime")]
        public int? CdRegime { get; set; }
        
        [JsonProperty("dta_tabela_preco")]
        public DateTime? DtaTabelaPreco { get; set; }
        
        [JsonProperty("nm_parcelas")]
        public int NmParcelas { get; set; }
        
        [JsonProperty("vl_parcela")]
        public decimal VlParcela { get; set; }
        
        [JsonProperty("vl_matricula")]
        public decimal VlMatricula { get; set; }
        
        [JsonProperty("cd_pessoa_escola")]
        public int? CdPessoaEscola { get; set; }
        
        [JsonProperty("vl_aula")]
        public decimal VlAula { get; set; }
        
        [JsonProperty("vl_preco_material")]
        public decimal VlPrecoMaterial { get; set; }
    }
}
