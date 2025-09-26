using System.ComponentModel.DataAnnotations;

namespace Simjob.Framework.Services.Api.Models.Acao
{
    public class ImportAcaoModel
    {
        [Required]
        public string Arquivo { get; set; }
        [Required]
        public int Cd_acao { get; set; }
        [Required]
        public int Cd_pessoa_escola { get; set; }
    }
}
