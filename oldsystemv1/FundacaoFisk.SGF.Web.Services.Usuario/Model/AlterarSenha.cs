using System;

namespace FundacaoFisk.SGF.Web.Services.Usuario.Model
{
    public class AlterarSenha
    {
        public string Login { get; set; }
        public string SenhaAtual { get; set; }
        public string NovaSenha { get; set; }
        public string ConfirmaNovaSenha { get; set; }
    }
}
