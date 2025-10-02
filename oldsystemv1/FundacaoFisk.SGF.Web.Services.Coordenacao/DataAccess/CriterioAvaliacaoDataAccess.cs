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


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class CriterioAvaliacaoDataAccess : GenericRepository<CriterioAvaliacao>, ICriterioAvaliacaoDataAccess
    {        

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public IEnumerable<CriterioAvaliacao> GetCriterioAvaliacaoSearch(SearchParameters parametros, string descricao, string abrev, bool inicio, bool? ativo, bool? conceito, bool IsParticipacao)
        {
            try{
                IEntitySorter<CriterioAvaliacao> sorter = EntitySorter<CriterioAvaliacao>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CriterioAvaliacao> sql;                

                if (ativo == null)
                {
                    sql = from c in db.CriterioAvaliacao.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.CriterioAvaliacao.AsNoTracking()
                          where c.id_criterio_ativo == ativo 
                          select c;
                }
                if (IsParticipacao)
                    sql = from s in sql
                          where s.id_participacao == IsParticipacao
                          select s;
                if (conceito != null)
                    sql = from s in sql
                          where s.id_conceito == conceito
                          select s;
                
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_criterio_avaliacao.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_criterio_avaliacao.Contains(descricao)
                                  select c;
                if (!String.IsNullOrEmpty(abrev))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_criterio_abreviado.StartsWith(abrev)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_criterio_abreviado.Contains(abrev)
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
        public CriterioAvaliacao firstOrDefault()
        {
            try{
                var sql = (from criterioAvaliacao in db.CriterioAvaliacao
                           select criterioAvaliacao).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public bool deleteAllCriterioAvaliacao(List<CriterioAvaliacao> criteriosAvaliacao)
        {
            try{
                string strCriterioAvaliacao = "";
                if (criteriosAvaliacao != null && criteriosAvaliacao.Count > 0)
                {
                    foreach (CriterioAvaliacao criterio in criteriosAvaliacao)
                    {
                        strCriterioAvaliacao += criterio.cd_criterio_avaliacao + ",";
                    }
                }

                if (strCriterioAvaliacao.Length > 0)
                {
                    strCriterioAvaliacao = strCriterioAvaliacao.Substring(0, strCriterioAvaliacao.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_criterio_Avaliacao where cd_criterio_avaliacao in (" + strCriterioAvaliacao + " )");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CriterioAvaliacao> getAllCriteriosAtivos() {
            try{
                var sql = from criterio in db.CriterioAvaliacao
                          orderby criterio.cd_criterio_avaliacao
                          where criterio.id_criterio_ativo == true
                          select criterio;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<CriterioAvaliacao> getAvaliacaoCriterio(int? cd_tipo_avaliacao, int? cd_criterio_avaliacao)
        {
            cd_tipo_avaliacao = cd_tipo_avaliacao == null ? 0 : cd_tipo_avaliacao;
            IQueryable<CriterioAvaliacao> sql;
            try
            {
                  sql = (from criterio in db.CriterioAvaliacao
                           where (criterio.id_criterio_ativo == true)
                              || (cd_criterio_avaliacao.HasValue && criterio.cd_criterio_avaliacao == cd_criterio_avaliacao)
                           select criterio).Distinct();
                  if (cd_tipo_avaliacao > 0)
                      sql = (from criterio in sql
                             where !(from avaliacao in db.Avaliacao
                                     where avaliacao.cd_criterio_avaliacao == criterio.cd_criterio_avaliacao
                                     && (avaliacao.cd_tipo_avaliacao == cd_tipo_avaliacao)
                                     select avaliacao).Any()
                             select criterio).Distinct();
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Retorna os critérios que existem com tipo e avaliação - na estória foi mudado o nome de critério para nome da avaliação
        public IEnumerable<CriterioAvaliacao> getNomesAvaliacao()
        {
            try
            {
                var result = (from criterio in db.CriterioAvaliacao
                              join avaliacao in db.Avaliacao
                              on criterio.cd_criterio_avaliacao equals avaliacao.cd_criterio_avaliacao
                              join tipoAva in db.TipoAvaliacao
                              on avaliacao.cd_tipo_avaliacao equals tipoAva.cd_tipo_avaliacao
                              select criterio).ToList().OrderBy(c => c.dc_criterio_avaliacao).Distinct().Select(x => new CriterioAvaliacao { cd_criterio_avaliacao = x.cd_criterio_avaliacao, dc_criterio_avaliacao = x.dc_criterio_avaliacao });
                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CriterioAvaliacao> getNomesAvaliacao(int cd_tipo_avaliacao)
        {
            try
            {
                var result = (from criterio in db.CriterioAvaliacao
                              join avaliacao in db.Avaliacao
                              on criterio.cd_criterio_avaliacao equals avaliacao.cd_criterio_avaliacao
                              join tipoAva in db.TipoAvaliacao
                              on avaliacao.cd_tipo_avaliacao equals tipoAva.cd_tipo_avaliacao
                              where tipoAva.cd_tipo_avaliacao == cd_tipo_avaliacao
                              select criterio).ToList().OrderBy(c => c.dc_criterio_avaliacao).Distinct().Select(x => new CriterioAvaliacao { cd_criterio_avaliacao = x.cd_criterio_avaliacao, dc_criterio_avaliacao = x.dc_criterio_avaliacao });
                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CriterioAvaliacao> getCriteriosPorAvalPart(int cdEscola)
        {
            try
            {
                var sql = from criterio in db.CriterioAvaliacao
                          orderby criterio.cd_criterio_avaliacao
                          where criterio.AvaliacaoParticipacao.Any(a => a.AvaliacaoParticipacaoVinc.Where(av => av.cd_escola == cdEscola).Any())
                          select criterio;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<CriterioAvaliacao> getNomesAvaliacaoByAval(int? cdCriterio)
        {
            try
            {
                var result = (from criterio in db.CriterioAvaliacao
                              where ((cdCriterio.HasValue && criterio.cd_criterio_avaliacao == cdCriterio) || criterio.id_criterio_ativo == true) && criterio.id_participacao == true
                              select criterio).ToList().OrderBy(c => c.dc_criterio_avaliacao).Distinct().Select(x => new CriterioAvaliacao {
                                  cd_criterio_avaliacao = x.cd_criterio_avaliacao, 
                                  dc_criterio_avaliacao = x.dc_criterio_avaliacao });
                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
