using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness
{
    public interface ICursoBusiness : IGenericBusiness
    {
       // void configuraUsuario(int cdUsuario);
        void sincronizarContexto(DbContext db);
        IEnumerable<Curso> getCursoSearch(SearchParameters parametros, string desc, bool inicio, bool? status, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<Curso> getCursoByContratoSearch(SearchParameters parametros, int cd_contrato, string desc, bool inicio, bool? status, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<Curso> getCursos(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getCursosSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getCursoByProdutoSemMatriculaSimultanea(int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getProximoCursoPorProdutoSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getCursosAulaPersonalizada(int cd_aluno, int cd_escola);
        IEnumerable<Curso> getCursosCargaHoraria(bool todasEscolas, int cd_escola);
        Curso addCurso(Curso curso, List<ItemCurso> materiaisDidaticos);
        Curso editCurso(Curso curso, List<ItemCurso> materiaisDidaticos, int cd_pessoa_escola);
        Boolean deleteCursos(List<Curso> cursos);
        Curso findCursoById(int idCurso);
        IEnumerable<CursoUI> getCursoProduto(SearchParameters parametros, string desc, int? cdProd);
        IEnumerable<Curso> getCursoTabelaPreco(int cdEscola);
        IEnumerable<Curso> findAllCurso();
        IEnumerable<CursoUI> getCursoProdutoPorTipoAval(SearchParameters parametros, string desc, int? produto, int cdTipoAvaliacao);
        IEnumerable<CursoUI> getCursoTipoAvaliacao(int cdTipoAvaliacao);
        IEnumerable<Curso> getCursoAvaliacaoTurma(int cd_escola);
        IEnumerable<Curso> getCursoWithAtividadeExtra(bool isAtivo, int cd_pesso_escola);
        IEnumerable<Curso> getCursoProgramacao();
        int? findProxCurso(int cdCurso);
        IEnumerable<Curso> getCursoProduto(List<int> cd_produtos);
        long? GetCursoOrdem(int cdCurso);
    }
}
