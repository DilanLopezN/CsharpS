namespace Simjob.Framework.Services.Api.Models.Pessoas
{
    public class PessoaRelacionamento
    {
        public int cd_pessoa_pai { get; set; }
        public int cd_pessoa_filho { get; set; }
        public int cd_papel_filho { get; set; }
        public int? cd_qualif_relacionamento { get; set; }
        public int? cd_papel_pai { get; set; }
        public string? email_pessoa_filho { get; set; }
    }
}
