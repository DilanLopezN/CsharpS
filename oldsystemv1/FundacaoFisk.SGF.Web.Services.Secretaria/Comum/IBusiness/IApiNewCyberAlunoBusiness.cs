
namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum 
{
    using System.Data.Entity;
    using Componentes.GenericBusiness.Comum;
    public interface IApiNewCyberAlunoBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);
        string postExecutaCyber(string url, string comando, string chave, string parametros);
        bool verificaRegistro(string url, string comando, string chave, int codigo);
        bool verificaRegistroLivroAlunos(string url, string comando, string chave, int codigo_aluno, int codigo_grupo, int codigo_livro);
        bool verificaRegistroGrupos(string url, string comando, string chave, int codigo_unidade, int codigo_grupo);
        bool aplicaApiCyber();
    }
}