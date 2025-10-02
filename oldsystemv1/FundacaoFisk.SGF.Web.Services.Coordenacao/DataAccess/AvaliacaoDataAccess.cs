using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using DALC4NET;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AvaliacaoDataAccess : GenericRepository<Avaliacao>, IAvaliacaoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public IEnumerable<AvaliacaoUI> searchAvaliacao(SearchParameters parametros, string descAbreviado, int? idTipoAvaliacao, int? idCriterio, bool inicio, bool? status)
        {
            try{
                IEntitySorter<AvaliacaoUI> sorter = EntitySorter<AvaliacaoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AvaliacaoUI> sql;
                // As duas linha a baixo server para anular os paramentros, caso estes vierem do relatório que são passados com default 0
                idTipoAvaliacao = idTipoAvaliacao == 0 ? null : idTipoAvaliacao;
                idCriterio = idCriterio == 0 ? null : idCriterio;

                sql = from avaliacao in db.Avaliacao.AsNoTracking()
                      orderby avaliacao.nm_ordem_avaliacao ascending
                      where (idTipoAvaliacao == null || avaliacao.cd_tipo_avaliacao == idTipoAvaliacao)
                         && (idCriterio == null || avaliacao.cd_criterio_avaliacao == idCriterio)
                         && (status == null || avaliacao.id_avaliacao_ativa == status)
                      select new AvaliacaoUI
                      {
                          cd_avaliacao = avaliacao.cd_avaliacao,
                          cd_criterio_avaliacao = avaliacao.cd_criterio_avaliacao,
                          cd_tipo_avaliacao = avaliacao.cd_tipo_avaliacao,
                          dc_criterio_abreviado = avaliacao.CriterioAvaliacao.dc_criterio_abreviado,
                          dc_criterio_avaliacao = avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                          dc_tipo_avaliacao = avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                          id_avaliacao_ativa = avaliacao.id_avaliacao_ativa,
                          nm_ordem_avaliacao = avaliacao.nm_ordem_avaliacao,
                          nm_peso_avaliacao = avaliacao.nm_peso_avaliacao,
                          vl_nota = avaliacao.vl_nota 
                      };

                sql = sorter.Sort(sql);

                var retorno = from conceito in sql
                              select conceito;

                if (!String.IsNullOrEmpty(descAbreviado))
                {
                    if (inicio)
                    {
                        retorno = from avaliavao in sql
                                  where avaliavao.dc_criterio_abreviado.StartsWith(descAbreviado)
                                  select avaliavao;
                    }//end if
                    else
                    {
                        retorno = from avaliavao in sql
                                  where avaliavao.dc_criterio_abreviado.Contains(descAbreviado)
                                  select avaliavao;
                    }//end else

                }

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Avaliacao> getAvaliacaoOrdem(int idTipoAvaliacao, int idCriterio, bool? status)
        {
            try{

                IQueryable<Avaliacao> sql = from avaliacao in db.Avaliacao
                                            where avaliacao.cd_criterio_avaliacao == idCriterio
                                            && avaliacao.cd_tipo_avaliacao == idTipoAvaliacao
                //                            && avaliacao.id_avaliacao_ativa == status
                                            orderby avaliacao.nm_ordem_avaliacao descending
                                            select avaliacao;
                if (status != null)
                    sql = from s in sql
                          where s.id_avaliacao_ativa == status
                          select s;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoUI> getAvaliacaoByIdTipoAvaliacao(int idTipoAvaliacao)
        {
            try{
                var sql = from avaliacao in db.Avaliacao
                          where avaliacao.cd_tipo_avaliacao == idTipoAvaliacao
                          orderby avaliacao.nm_ordem_avaliacao ascending
                          select new AvaliacaoUI
                          {
                              cd_avaliacao = avaliacao.cd_avaliacao,
                              dc_tipo_avaliacao = avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                              cd_criterio_avaliacao = avaliacao.cd_criterio_avaliacao,
                              dc_criterio_avaliacao = avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                              vl_nota = avaliacao.vl_nota,
                              nm_ordem_avaliacao = avaliacao.nm_ordem_avaliacao,
                              nm_peso_avaliacao = avaliacao.nm_peso_avaliacao,
                              id_avaliacao_ativa = avaliacao.id_avaliacao_ativa
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //retorna o total da nota que esta no banco
        public byte? getSomatorio(int idTipoAvaliacao, int idCriterio)
        {
            try{

                int soma = (from avaliacao in db.Avaliacao
                                              where avaliacao.cd_criterio_avaliacao == idCriterio
                                              && avaliacao.cd_tipo_avaliacao == idTipoAvaliacao
                                              orderby avaliacao.nm_ordem_avaliacao descending
                            select avaliacao.vl_nota).Sum(x => x != null ? (int)x : 0);

                
                return (Byte) soma;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Retorna o valor da nota
        public byte? getNotaAvaliacao(int idAvaliacao) {
            try{
                var sql = (from avaliacao in db.Avaliacao
                           where avaliacao.cd_avaliacao == idAvaliacao
                           select avaliacao.vl_nota).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Retorna a ordem máxima
        public byte? getOrdem(int tipoAvaliacao, int criterio)
        {
            try{
                return base.findAll(false).Where(e => e.cd_tipo_avaliacao == tipoAvaliacao && e.cd_criterio_avaliacao == criterio).Max(e => e.nm_ordem_avaliacao);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        //Atualiza a ordem da Entidade
        public void editOrdemAvaliacao(Avaliacao avaliacao)
        {
            try
            {            
                db.Avaliacao.Attach(avaliacao);
                db.Entry(avaliacao).Property(a => a.nm_ordem_avaliacao).IsModified = true;
                db.SaveChanges();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllAvaliacao(List<Avaliacao> avaliacoes)
        {
            try{
                string strAvaliacao = "";
                if (avaliacoes != null && avaliacoes.Count > 0)
                    foreach (Avaliacao e in avaliacoes)
                        strAvaliacao += e.cd_avaliacao + ",";
                // Remove o último ponto e virgula:
                if (strAvaliacao.Length > 0)
                    strAvaliacao = strAvaliacao.Substring(0, strAvaliacao.Length - 1);
                int retorno = db.Database.ExecuteSqlCommand("delete from t_avaliacao where cd_avaliacao in(" + strAvaliacao + ")");
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Avaliacao> getAvaliacoesCursoSemAvaliacaoTurma(int idTurma) {
            try {
                var sql = (from avaliacao in db.Avaliacao
                           join criterio in db.CriterioAvaliacao on avaliacao.cd_criterio_avaliacao equals criterio.cd_criterio_avaliacao
                           join tipo in db.TipoAvaliacao on avaliacao.cd_tipo_avaliacao equals tipo.cd_tipo_avaliacao
                           join avaliacaoCurso in db.AvaliacaoCurso on tipo.cd_tipo_avaliacao equals avaliacaoCurso.cd_tipo_avaliacao
                           join curso in db.Curso on avaliacaoCurso.cd_curso equals curso.cd_curso
                           join turma in db.Turma on curso.cd_curso equals turma.cd_curso
                           where turma.cd_turma == idTurma
                               //Não existe avaliação da turma com essa avaliação
                               && !(from avaliacaoTurma in db.AvaliacaoTurma
                                    join turma2 in db.Turma on avaliacaoTurma.cd_turma equals turma2.cd_turma
                                    where avaliacaoTurma.cd_avaliacao == avaliacao.cd_avaliacao 
                                    && turma2.cd_turma == idTurma
                                    select avaliacaoTurma.cd_avaliacao).Any()
                               && criterio.id_criterio_ativo
                               && tipo.id_tipo_ativo
                               && avaliacao.id_avaliacao_ativa
                           select avaliacao);
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool existNotaLancadaAvaliacaoTurma(int cd_avaliacao, int cd_escola)
        {
            try
            {
                var sql = (from avaliacao in db.Avaliacao
                           join avalTurma in db.AvaliacaoTurma on avaliacao.cd_avaliacao equals avalTurma.cd_avaliacao
                           join avalAluno in db.AvaliacaoAluno on avalTurma.cd_avaliacao_turma equals avalAluno.cd_avaliacao_turma
                           join turma in db.Turma on avalTurma.cd_turma equals turma.cd_turma
                           join curso in db.Curso on  turma.cd_curso equals curso.cd_curso
                           where avaliacao.cd_avaliacao == cd_avaliacao
                              && turma.cd_pessoa_escola == cd_escola
                              && (avalAluno.nm_nota_aluno >= 0 || avalAluno.cd_conceito != null || 
                              avalTurma.cd_funcionario != null || avalTurma.dt_avaliacao_turma != null)
                           select avaliacao).Any();
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existNotaLancadaAvaliacaoTurma(List<int> cdsAvaliacoes)
        {
            try
            {
                var sql = (from avaliacao in db.Avaliacao
                           join avalTurma in db.AvaliacaoTurma on avaliacao.cd_avaliacao equals avalTurma.cd_avaliacao
                           join avalAluno in db.AvaliacaoAluno on avalTurma.cd_avaliacao_turma equals avalAluno.cd_avaliacao_turma
                           join turma in db.Turma on avalTurma.cd_turma equals turma.cd_turma
                           join curso in db.Curso on turma.cd_curso equals curso.cd_curso
                           where cdsAvaliacoes.Contains(avaliacao.cd_avaliacao)
                              && (avalAluno.nm_nota_aluno >= 0 || avalAluno.cd_conceito != null ||
                              avalTurma.cd_funcionario != null || avalTurma.dt_avaliacao_turma != null)
                           select avaliacao).Any();
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Avaliacao> findByIdTipoAvaliacao(int cd_tipo_avaliacao)
        {
            try
            {
                var sql = from avaliacao in db.Avaliacao
                          where avaliacao.cd_tipo_avaliacao == cd_tipo_avaliacao
                          select avaliacao;

                return sql;
            }
            catch (Exception exe)
            {
                 throw new DataAccessException(exe);
            }
        }       

        public int getAvaliacaoCursoExistsTurmaWithCurso(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from curso in db.Curso
                           join turma in db.Turma on curso.cd_curso equals turma.cd_curso 
                           join avalicaoCurso in db.AvaliacaoCurso on curso.cd_curso equals avalicaoCurso.cd_curso
                           where turma.cd_turma == cd_turma
                               && turma.cd_pessoa_escola == cd_escola                               
                           select turma).Count();

                return sql;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }

        public IEnumerable<Avaliacao> getAvaliacaoECriterioTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from a in db.Avaliacao
                           where a.AvaliacaoTurma.Where(at => at.Turma.cd_turma == cd_turma).Any()  //&& at.Turma.cd_pessoa_escola == cd_escola
                           orderby a.TipoAvaliacao.dc_tipo_avaliacao, a.CriterioAvaliacao.dc_criterio_avaliacao
                           select new
                           {
                               cd_avaliacao = a.cd_avaliacao,
                               dc_tipoAvalicao = a.TipoAvaliacao.dc_tipo_avaliacao,
                               dc_criterio = a.CriterioAvaliacao.dc_criterio_avaliacao
                           }).ToList().Select(x => new Avaliacao { 
                               cd_avaliacao = x.cd_avaliacao,
                               descTipoENomeAvalicao = x.dc_tipoAvalicao + " - " + x.dc_criterio
                           });

                return sql;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
        public DataTable getRptAvaliacao(int cd_turma, int cdCurso, int cdProduto, int cdEscola, int cdFuncionario, int tipoTurma, byte sitTurma, DateTime? pDtIni, DateTime? pDtFim, bool isConceito)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;

                DataTable dtReportData = new DataTable();

                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);
                DBParameter param2 = new DBParameter("@cd_curso", cdCurso, DbType.Int32);
                DBParameter param3 = new DBParameter("@cd_produto", cdProduto, DbType.Int32);
                DBParameter param4 = new DBParameter("@cd_funcionario", cdFuncionario, DbType.Int32);
                DBParameter param5 = new DBParameter("@cd_escola", cdEscola, DbType.Int32);
                DBParameter param6 = new DBParameter("@cd_regime", tipoTurma, DbType.Int32);
                DBParameter param7 = new DBParameter("@dt_ini", pDtIni, DbType.DateTime);
                DBParameter param8 = new DBParameter("@dt_fim", pDtFim, DbType.DateTime);
                DBParameter param9 = new DBParameter("@situacao", sitTurma, DbType.Byte);
                DBParameter param10 = new DBParameter("@isConceito", isConceito, DbType.Boolean);

                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);
                paramCollection.Add(param2);
                paramCollection.Add(param3);
                paramCollection.Add(param4);
                paramCollection.Add(param5);
                paramCollection.Add(param6);
                paramCollection.Add(param7);
                paramCollection.Add(param8);
                paramCollection.Add(param9);
                paramCollection.Add(param10);

                dtReportData = dbSql.ExecuteDataTable("sp_RptAvaliacao", paramCollection, CommandType.StoredProcedure);
                return dtReportData;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
        public DataTable getRptAvaliacaoTurma(int cd_turma)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
                DataTable dtReportData = new DataTable();
                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);

                dtReportData = dbSql.ExecuteDataTable("sp_RptAvaliacaoSub", paramCollection, CommandType.StoredProcedure);
                return dtReportData;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
        public DataTable getRptAvaliacaoTurmaConceito(int cd_turma)
        {
            try
            {
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ConnectionString;
                string providerName = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionStringReportProcedure"].ProviderName;
                DataTable dtReportData = new DataTable();
                DBHelper dbSql = new DBHelper(connectionString, providerName);

                DBParameter param1 = new DBParameter("@cd_turma", cd_turma, DbType.Int32);
                DBParameterCollection paramCollection = new DBParameterCollection();
                paramCollection.Add(param1);

                dtReportData = dbSql.ExecuteDataTable("sp_RptAvaliacaoTurmaConceito", paramCollection, CommandType.StoredProcedure);
                return dtReportData;
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
    }
}
