using FundacaoFisk.SGF.Web.Services.Auth.Comum;

namespace FundacaoFisk.SGF.Web.Services.Auth.Model {
    public class AuthenticationResult {
        public string ErrorMessage { get; set; }
        public int CodPessoaUsuario { get; set; }
        public bool IdMaster { get; set; }
        public int CdUsuario { get; set; }
        public string Permissao { get; set; }
        public string loginUsuario { get; set; }
        public int IdFusoHorario { get; set; }
        public bool IdHorarioVerao { get; set; }
        public AccessTokenResponse Token { get; set; }
        public AuthenticationStatus Status { get; set; }
    }
}
