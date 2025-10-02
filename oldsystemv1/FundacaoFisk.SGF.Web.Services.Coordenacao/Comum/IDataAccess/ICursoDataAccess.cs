using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface ICursoDataAccess : IGenericRepository<Curso>
    {
        IEnumerable<Curso> getCursoSearch(SearchParameters parametros, String nome, Boolean inicio, Boolean? ativo, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<Curso> getCursoByContratoSearch(SearchParameters parametros, int cd_contrato, String nome, Boolean inicio, Boolean? ativo, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<CursoUI> getCursoProduto(SearchParameters parametros, String desc, int? produto);
        IEnumerable<Curso> getCursos(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto,int? cd_escola);
        IEnumerable<Curso> getCursosSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getCursoByProdutoSemMatriculaSimultanea(int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getProximoCursoPorProdutoSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola);
        IEnumerable<Curso> getCursosAulaPersonalizada(int cd_aluno, int cd_escola);
        IEnumerable<Curso> getCursosCargaHoraria(bool todasEscolas, int cd_escola); 
        Boolean deleteCursos(List<Curso> cursos);
        Curso addCurso(Curso curso);
        Curso editCurso(Curso curso);
        IEnumerable<Curso> getCursoTabelaPreco(int cdEscola);
        Curso firstOrDefault();
        IEnumerable<Curso> findAllCurso();
        IEnumerable<Curso> getCursoProgramacao();
        IEnumerable<CursoUI> getCursoTipoAvaliacao(int cdTipoAvaliacao);
        List<Curso> getCursosByCod(int[] cdCursos);
        IEnumerable<CursoUI> getCursoProdutoPorTipoAval(SearchParameters parametros, string desc, int? produto, int cdTipoAvaliacao);
        IEnumerable<Curso> getCursoAvaliacaoTurma(int cd_escola);
        IEnumerable<Curso> getCursoWithAtividadeExtra(bool isAtivo, int cd_pesso_escola);
        Curso returExistCursoWithTurma(int cd_pesso_escola, int cd_curso);
        int? findProxCurso(int cdCurso);
        IEnumerable<Curso> getCursoProduto(List<int> cd_produtos);
        long? GetCursoOrdem(int cdCurso);
    }
}
