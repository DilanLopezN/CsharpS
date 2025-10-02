using Componentes.GenericBusiness.Comum;

namespace FundacaoFisk.SGF.Web.Services.Empresa.Comum.IBusiness
{
    public interface IApiNewCyberFuncionarioBusiness : IGenericBusiness
    {
        string postExecutaCyber(string url, string comando, string chave, string parametros);
        bool verificaRegistro(string url, string comando, string chave, int codigo);
        bool verificaRegistroFuncionario(string url, string comando, string chave, string codigo);
        bool aplicaApiCyber();
    }
}