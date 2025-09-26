using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Simjob.Framework.Services.Api.Models.MovimentosFinanceiros
{
    public class EstornoContaCorrenteModel
    {
        [Required(ErrorMessage = "cd_conta_corrente é obrigatório")]
        [JsonPropertyName("cd_conta_corrente")]
        public int cd_conta_corrente { get; set; }
    }
}
