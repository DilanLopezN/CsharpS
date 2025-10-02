using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Business;
using FundacaoFisk.SGF.Utils.Messages;
using System.Data.Entity;
using Componentes.GenericBusiness;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Business
{
    public class CursoBusiness : ICursoBusiness
    {
        /// <summary>
        /// Declaração de Interfaces
        /// </summary>
        public ICursoDataAccess DaoCurso { get; set; }
        public IFinanceiroBusiness BusinessFinanceiro { get; set; }
        
        /// <summary>
        /// Método Construtor do Dao
        /// </summary>
        public CursoBusiness(IFinanceiroBusiness businessFinanceiro, ICursoDataAccess daoCurso)
        {
            if (daoCurso == null || businessFinanceiro == null)
            {
                throw new ArgumentNullException("CursoBusiness");
            }
            DaoCurso = daoCurso;
            BusinessFinanceiro = businessFinanceiro;
        }

        public void configuraUsuario(int cdUsuario, int cd_empresa) {
            // Configura os codigos do usuário para auditorias dos DataAccess:
            ((SGFWebContext)this.DaoCurso.DB()).IdUsuario  = cdUsuario;
            ((SGFWebContext)this.DaoCurso.DB()).cd_empresa = cd_empresa;
        }

        public void sincronizarContexto(DbContext db)
        {
            //this.DaoCurso.sincronizaContexto(db);
            //this.BusinessFinanceiro.sincronizarContextos(db);
        }

        public IEnumerable<Curso> getCursosAulaPersonalizada(int cd_aluno, int cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoCurso.getCursosAulaPersonalizada(cd_aluno, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }
        public IEnumerable<Curso> getCursosCargaHoraria(bool todasEscolas, int cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoCurso.getCursosCargaHoraria(todasEscolas, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Curso> getCursos(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoCurso.getCursos(hasDependente, cd_curso, cd_produto, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Curso> getCursosSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoCurso.getCursosSemMatriculaSimultanea(hasDependente, cd_curso, cd_produto, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Curso> getCursoByProdutoSemMatriculaSimultanea(int? cd_curso, int? cd_produto, int? cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoCurso.getCursoByProdutoSemMatriculaSimultanea(cd_curso, cd_produto, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Curso> getProximoCursoPorProdutoSemMatriculaSimultanea(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                retorno = DaoCurso.getProximoCursoPorProdutoSemMatriculaSimultanea(hasDependente, cd_curso, cd_produto, cd_escola).ToList();
                transaction.Complete();
            }
            return retorno;
        }


        public IEnumerable<Curso> getCursoSearch(SearchParameters parametros, string desc, bool inicio, bool? status, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_curso";
                parametros.sort = parametros.sort.Replace("curso_ativo", "id_curso_ativo");

                retorno = DaoCurso.getCursoSearch(parametros, desc, inicio, status, produto, estagio, modalidade, nivel, dt_inicial, dt_final);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Curso> getCursoByContratoSearch(SearchParameters parametros, int cd_contrato, string desc, bool inicio, bool? status, int? produto, int? estagio, int? modalidade, int? nivel, DateTime? dt_inicial, DateTime? dt_final)
        {
            IEnumerable<Curso> retorno = new List<Curso>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                // Reajusta os parâmetros de ordenação da pesquisa:
                if (parametros.sort == null)
                    parametros.sort = "no_curso";
                parametros.sort = parametros.sort.Replace("curso_ativo", "id_curso_ativo");

                retorno = DaoCurso.getCursoByContratoSearch(parametros, cd_contrato, desc, inicio, status, produto, estagio, modalidade, nivel, dt_inicial, dt_final);
                transaction.Complete();
            }
            return retorno;
        }

        public Curso findCursoById(int id)
        {
            return DaoCurso.findById(id, false);
        }

        public Curso addCurso(Curso entity, List<ItemCurso> materiaisDidaticos)
        {
            entity.ProximoCurso = null;
            entity.MateriaisDidaticos = materiaisDidaticos;
            Curso curso = DaoCurso.addCurso(entity);
            return curso;
        }

        public Curso editCurso(Curso entity, List<ItemCurso> materiaisDidaticos, int cd_pessoa_escola)
        {
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                //Remove os materiais didáticos:
                BusinessFinanceiro.deleteItemByCurso(entity.cd_curso);
                
                //Atualiza os dados do curso:
                Curso cursoET = DaoCurso.returExistCursoWithTurma(cd_pessoa_escola, entity.cd_curso);
                if ((cursoET != null) && (entity.cd_produto != cursoET.cd_produto))
                    throw new CursoBusinessException(string.Format(Messages.msgNotUpdateCursoTurma), null, CursoBusinessException.TipoErro.ERRO_EXISTES_CURSO_TURMA, false);
                
                Curso curso = new Curso();
                curso = DaoCurso.findById(entity.cd_curso, false);
                curso.copy(entity, true);
                //curso.nm_carga_maxima = entity.nm_carga_maxima;
                DaoCurso.saveChanges(false);
                
                curso.MateriaisDidaticos = materiaisDidaticos;
                entity = DaoCurso.editCurso(curso);
                transaction.Complete();
            }
            return entity;
        }

        public bool deleteCursos(List<Curso> cursos)
        {

            bool deleted = false;
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.COMMITED))
            {
                deleted = DaoCurso.deleteCursos(cursos);
                transaction.Complete();
                return deleted;

            } 
        }

        public IEnumerable<CursoUI> getCursoProduto(SearchParameters parametros, string desc, int? cdProd)
        {
            IEnumerable<CursoUI> retorno = new List<CursoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_curso";
                retorno = DaoCurso.getCursoProduto(parametros, desc, cdProd);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<CursoUI> getCursoProdutoPorTipoAval(SearchParameters parametros, string desc, int? produto, int cdTipoAvaliacao)
        {
            IEnumerable<CursoUI> retorno = new List<CursoUI>();
            using (var transaction = TransactionScopeBuilder.createDefaultTransaction(TransactionScopeBuilder.TransactionType.UNCOMMITED))
            {
                if (parametros.sort == null)
                    parametros.sort = "no_curso";
                retorno = DaoCurso.getCursoProdutoPorTipoAval(parametros, desc, produto, cdTipoAvaliacao);
                transaction.Complete();
            }
            return retorno;
        }

        public IEnumerable<Curso> getCursoTabelaPreco(int cdEscola)
        {
            return DaoCurso.getCursoTabelaPreco(cdEscola);
        }

        public IEnumerable<Curso> findAllCurso()
        {
            return DaoCurso.findAllCurso();
        }

        public IEnumerable<CursoUI> getCursoTipoAvaliacao(int cdTipoAvaliacao)
        {
            return DaoCurso.getCursoTipoAvaliacao(cdTipoAvaliacao);
        }

        public IEnumerable<Curso> getCursoAvaliacaoTurma(int cd_escola)
        {
            return DaoCurso.getCursoAvaliacaoTurma(cd_escola);
        }

        public IEnumerable<Curso> getCursoWithAtividadeExtra(bool isAtivo, int cd_pesso_escola)
        {
            return DaoCurso.getCursoWithAtividadeExtra(isAtivo, cd_pesso_escola);
        }

        public IEnumerable<Curso> getCursoProgramacao()
        {
            return DaoCurso.getCursoProgramacao();
        }

        public int? findProxCurso(int cdCurso)
        {
            return DaoCurso.findProxCurso(cdCurso);
        }

        public IEnumerable<Curso> getCursoProduto(List<int> cd_produtos)
        {
            return DaoCurso.getCursoProduto(cd_produtos);
        }
        public long? GetCursoOrdem(int cdCurso)
        {
            return DaoCurso.GetCursoOrdem(cdCurso);
        }
    }
}
