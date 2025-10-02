using System;
using System.Web.Http;
using Componentes.GenericBusiness.Comum;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness
{
    public interface IApiAreaRestritaBusiness : IGenericBusiness
    {
        string GerarToken(string userEmail);
        object SendToken(string token);
        TokenAreaRestritaUI ObterToken(string acao, string name);
        MenusAreaRestritaRetorno ListagemDosMenus(string token);
        LogoutAreaRestritaRetorno Logout(string token, string acao, string name);
        UserAreaRestritaCreateRetorno criarUsuario(string token, UserAreaRestritaUI usuarioCreate);
        UserAreaRestritaUpdateRetorno updateUsuario(string token, int id, UserAreaRestritaUI usuarioEdit);
        UserAreaRestritaDetalheRetorno getDetalhesUsuario(string token, string id);
        UserAreaRestritaUI deleteUsuarioAreaRestrita(string token, string id);
        bool aplicaApiAreaRestrita();
        int aplicaTamanhoSenhaApiAreaRestrita(string aplica);
    }
}