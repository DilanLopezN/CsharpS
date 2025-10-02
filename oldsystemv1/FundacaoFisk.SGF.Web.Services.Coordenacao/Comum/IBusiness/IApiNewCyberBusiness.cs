
using System.Collections.Generic;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness
{
    using System.Data.Entity;
    using Componentes.GenericBusiness.Comum;
    using FundacaoFisk.SGF.GenericModel;
    public interface IApiNewCyberBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);
        string postExecutaCyber(string url, string comando, string chave, string parametros);
        EscolaApiCyberBdUI getEscola(int cd_escola);
        bool verificaRegistro(string url, string comando, string chave, int codigo);
        bool verificaRegistroGrupos(string url, string comando, string chave, int codigo_grupo);
        bool verificaRegistroLivroAlunos(string url, string comando, string chave, int codigo_aluno, int codigo_grupo, int codigo_livro);
        bool verificaRegistroFuncionario(string url, string comando, string chave, string codigo);
        bool aplicaApiCyber();
        PessoaCoordenadorCyberBdUI findPessoaCoordenadorCyberByCdPessoa(int cd_pessoa, int cd_empresa);
        List<RelacionamentoSGF> findRelacionamentosCoordenadorByEmpresa(int cd_empresa);
    }
}