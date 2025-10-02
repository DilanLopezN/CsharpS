using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class TipoAvaliacaoDataAccess : GenericRepository<TipoAvaliacao>, ITipoAvaliacaoDataAccess
    {
     

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public IEnumerable<TipoAvaliacao> GetTipoAvaliacaoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int? cd_tipo_avaliacao, int? cd_criterio_avaliacao, int cdCurso, int cdProduto)
        {
            try{
                IEntitySorter<TipoAvaliacao> sorter = EntitySorter<TipoAvaliacao>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoAvaliacao> sql;

                cd_criterio_avaliacao = cd_criterio_avaliacao == 0 ? null : cd_criterio_avaliacao;
                cd_tipo_avaliacao = cd_tipo_avaliacao == 0 ? null : cd_tipo_avaliacao;

                sql = from c in db.TipoAvaliacao.AsNoTracking()
                      select c;

                if (ativo != null)
                    sql = from c in sql
                          where c.id_tipo_ativo == ativo
                          select c;
                
                if (cd_criterio_avaliacao.HasValue)
                    sql = from c in sql
                          join avaliacao in db.Avaliacao.AsNoTracking()
                          on c.cd_tipo_avaliacao equals avaliacao.cd_tipo_avaliacao
                          into ava
                          from avaliacao in ava.DefaultIfEmpty()
                          where avaliacao.cd_criterio_avaliacao == cd_criterio_avaliacao
                          select c;

                if(cd_tipo_avaliacao.HasValue)
                    sql = from c in sql
                          where c.cd_tipo_avaliacao == cd_tipo_avaliacao
                          select c;

                if (cdCurso > 0)
                    sql = from a in sql
                          where a.AvaliacaoCurso.Where(c => c.cd_curso == cdCurso).Any()
                          select a;

                if (cdProduto > 0)
                    sql = from a in sql
                          where a.AvaliacaoCurso.Where(c => c.Curso.cd_produto == cdProduto).Any()
                          select a; 
                
                sql = sorter.Sort(sql);

                var retorno = from c in sql
                              select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_tipo_avaliacao.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_tipo_avaliacao.Contains(descricao)
                                  select c;

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
        //retorna o primeiro registro ou o default
        public TipoAvaliacao firstOrDefault()
        {
            try{
                var sql = (from TipoAvaliacao in db.TipoAvaliacao
                           select TipoAvaliacao).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public bool deleteAllTipoAvaliacao(List<TipoAvaliacao> TiposAvaliacao)
        {
            try{
                string strTipoAvaliacao = "";
                if (TiposAvaliacao != null && TiposAvaliacao.Count > 0)
                {
                    foreach (TipoAvaliacao Tipo in TiposAvaliacao)
                    {
                        strTipoAvaliacao += Tipo.cd_tipo_avaliacao + ",";
                    }
                }

                if (strTipoAvaliacao.Length > 0)
                {
                    strTipoAvaliacao = strTipoAvaliacao.Substring(0, strTipoAvaliacao.Length - 1);
                }
                db.Database.ExecuteSqlCommand("delete from t_avaliacao where cd_tipo_avaliacao in(" + strTipoAvaliacao + ")");
                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_Avaliacao where cd_tipo_avaliacao in (" + strTipoAvaliacao + " )");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteCursoTipoAvaliacao(TipoAvaliacao tipoAvaliacao)
        {
            try{
                bool retorno = false;
                int tipo = db.Database.ExecuteSqlCommand("delete from t_avaliacao_curso where cd_tipo_avaliacao in(" + tipoAvaliacao.cd_tipo_avaliacao + ")");
                if (tipo > 0)
                    retorno = true;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? getTotalNotaTipoAvaliacao(int idtipoAvaliacao) {
            try{
                var sql = (from tipoavaliacao in db.TipoAvaliacao
                           where tipoavaliacao.cd_tipo_avaliacao == idtipoAvaliacao
                           select tipoavaliacao.vl_total_nota).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public List<TipoAvaliacao> getTipoAvaliacao()
        {
            try{
                var result = (from tipoAvaliacao in db.TipoAvaliacao
                             join avaliacao in db.Avaliacao
                             on tipoAvaliacao.cd_tipo_avaliacao equals avaliacao.cd_tipo_avaliacao
                             select tipoAvaliacao).Distinct();
                return result.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoAvaliacao> getTipoAvaliacao(bool? ativo, int idTipoAvaliacao)
        {
            var idAux = idTipoAvaliacao;
            try
            {
                var result = (from tipoAvaliacao in db.TipoAvaliacao
                              where (ativo == null || tipoAvaliacao.id_tipo_ativo == ativo)
                               || (idTipoAvaliacao == idAux && tipoAvaliacao.cd_tipo_avaliacao == idTipoAvaliacao)
                              select new
                              {
                                  cd_tipo_avaliacao = tipoAvaliacao.cd_tipo_avaliacao,
                                  dc_tipo_avaliacao = tipoAvaliacao.dc_tipo_avaliacao,
                              }).ToList().Select(x => new TipoAvaliacao { cd_tipo_avaliacao = x.cd_tipo_avaliacao, dc_tipo_avaliacao = x.dc_tipo_avaliacao });
                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<TipoAvaliacao> getTipoAvaliacaoAvaliacaoTurma()
        {
            try
            {
                var sql = (from tipoAvaliacao in db.TipoAvaliacao
                           join avaliacao in db.Avaliacao on tipoAvaliacao.cd_tipo_avaliacao equals avaliacao.cd_tipo_avaliacao
                           join avaliacaoTurma in db.AvaliacaoTurma on avaliacao.cd_avaliacao equals avaliacaoTurma.cd_avaliacao
                           select new
                           {
                               cd_tipo_avaliacao = tipoAvaliacao.cd_tipo_avaliacao,
                               dc_tipo_avaliacao = tipoAvaliacao.dc_tipo_avaliacao,
                           }).Distinct().ToList().Select(x => new TipoAvaliacao { cd_tipo_avaliacao = x.cd_tipo_avaliacao, dc_tipo_avaliacao = x.dc_tipo_avaliacao });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<TipoAvaliacaoTurma> tiposAvaliacao(int cd_turma, int cd_escola, bool id_conceito)
        {
            try
            {
                IEnumerable<TipoAvaliacaoTurma> retorno;
                if(!id_conceito)
                    retorno = (from tipos in db.TipoAvaliacao
                               join avaliacao in db.Avaliacao on tipos.cd_tipo_avaliacao equals avaliacao.cd_tipo_avaliacao
                               join avaliacaoTurma in db.AvaliacaoTurma on avaliacao.cd_avaliacao equals avaliacaoTurma.cd_avaliacao
                               join turma in db.Turma on avaliacaoTurma.cd_turma equals turma.cd_turma
                               where turma.cd_turma == cd_turma
                                    //LBM&& turma.cd_pessoa_escola == cd_escola
                                    && avaliacao.CriterioAvaliacao.id_conceito == false
                                    && (tipos.id_tipo_ativo == true || avaliacao.AvaliacaoTurma.Any(a => a.AvaliacaoAluno.Any(al => al.nm_nota_aluno != null 
                                                                    || al.nm_nota_aluno > 0 && al.cd_avaliacao_turma == a.cd_avaliacao_turma
                                                                    || al.cd_conceito.HasValue) && a.cd_avaliacao == avaliacao.cd_avaliacao))
                               select new
                               {
                                   cd_tipo_avaliacao = tipos.cd_tipo_avaliacao,
                                   dc_tipo_avaliacao = tipos.dc_tipo_avaliacao
                               }).Distinct().ToList().Select(x => new TipoAvaliacaoTurma
                               {
                                   cd_tipo_avaliacao = x.cd_tipo_avaliacao,
                                   dc_tipo_avaliacao = x.dc_tipo_avaliacao
                               });
                 else
                    retorno = (from tipos in db.TipoAvaliacao
                               join avaliacao in db.Avaliacao on tipos.cd_tipo_avaliacao equals avaliacao.cd_tipo_avaliacao
                               join avaliacaoTurma in db.AvaliacaoTurma on avaliacao.cd_avaliacao equals avaliacaoTurma.cd_avaliacao
                               join turma in db.Turma on avaliacaoTurma.cd_turma equals turma.cd_turma
                               where turma.cd_turma == cd_turma
                                    //LBM&& turma.cd_pessoa_escola == cd_escola
                                    && avaliacao.CriterioAvaliacao.id_conceito == true
                                    && (tipos.id_tipo_ativo == true || avaliacao.AvaliacaoTurma.Any(a => a.AvaliacaoAluno.Any(al => al.nm_nota_aluno != null
                                                                    || al.nm_nota_aluno > 0 && al.cd_avaliacao_turma == a.cd_avaliacao_turma
                                                                    || al.cd_conceito.HasValue) && a.cd_avaliacao == avaliacao.cd_avaliacao))
                               select new
                               {
                                   cd_tipo_avaliacao = tipos.cd_tipo_avaliacao,
                                   dc_tipo_avaliacao = tipos.dc_tipo_avaliacao
                               }).Distinct().ToList().Select(x => new TipoAvaliacaoTurma
                               {
                                   cd_tipo_avaliacao = x.cd_tipo_avaliacao,
                                   dc_tipo_avaliacao = x.dc_tipo_avaliacao
                               });

                return retorno.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
