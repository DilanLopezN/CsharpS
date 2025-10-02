using System;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Model
{
    public class PessoaRafUI
    {
        public int cd_pessoa_raf { get; set; }

        public int cd_pessoa { get; set; }

        public string nm_raf { get; set; }

        public bool id_raf_liberado { get; set; }
        public DateTime? dt_limite_bloqueio { get; set; }


    }
}