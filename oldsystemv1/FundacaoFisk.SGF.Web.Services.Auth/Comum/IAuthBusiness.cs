using System;
using System.Net;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Auth.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Auth.Comum {
    public interface IAuthBusiness : IGenericBusiness {
        void verificaHorarioLogin(TimeSpan hr_inicial, TimeSpan hr_final, TimeSpan hr_servidor);
        AuthenticationResult Authenticate(UserCredential credentials);
        AuthenticationResult RefreshAccessToken(string refreshToken);
        string GetNomeUsuario(string nome);
        UsuarioWebSGF GetNomeCodigoUsuario(string nome);
        AuthenticationResult renovarAutenticacao(UserCredential credentials);
    }
}
