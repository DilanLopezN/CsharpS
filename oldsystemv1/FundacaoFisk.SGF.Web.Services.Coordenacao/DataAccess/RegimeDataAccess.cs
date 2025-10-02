using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess {
    public class RegimeDataAccess : GenericRepository<Regime>, IRegimeDataAccess
    {
        public enum TipoConsultaRegimeEnum
        {
            HAS_ATIVO = 0,
            HAS_TURMA = 1
        }


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Regime> getRegimeDesc(Componentes.Utils.SearchParameters parametros, string desc, string abrev, bool inicio, bool? ativo)
        {
            try{
                IQueryable<Regime> sql;
                IEntitySorter<Regime> sorter = EntitySorter<Regime>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);

                if (ativo == null)
                {
                    sql = from regime in db.Regime.AsNoTracking()
                          orderby regime.no_regime
                          select regime;
                }// end if
                else
                {
                    sql = from regime in db.Regime.AsNoTracking()
                          where regime.id_regime_ativo == ativo
                          orderby regime.no_regime
                          select regime;
                }// end else

                sql = sorter.Sort(sql);           
                var retorno = from regime in sql
                              select regime;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from regime in sql
                                  where regime.no_regime.StartsWith(desc)
                                  select regime;
                    }// end if
                    else
                    {
                        retorno = from regime in sql
                                  where regime.no_regime.Contains(desc)
                                  select regime;
                    }

                }
                if (!String.IsNullOrEmpty(abrev))
                {
                    if (inicio)
                    {
                        retorno = from regime in retorno
                                  where regime.no_regime_abreviado.StartsWith(abrev)
                                  select regime;
                    }// end if
                    else
                    {
                        retorno = from regime in retorno
                                  where regime.no_regime_abreviado.Contains(abrev)
                                  select regime;
                    }

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

        public bool deleteAllRegime(List<Regime> regimes)
        {
            try{
                string strRegime = "";
                if (regimes != null && regimes.Count > 0)
                    foreach (Regime e in regimes)
                        strRegime += e.cd_regime + ",";

                // Remove o último ponto e virgula:
                if (strRegime.Length > 0)
                    strRegime = strRegime.Substring(0, strRegime.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_regime where cd_regime in(" + strRegime + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public IEnumerable<Regime> getRegimes(TipoConsultaRegimeEnum hasDependente, int? cd_regime)
        {
            try{
                IQueryable<Regime> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaRegimeEnum.HAS_ATIVO:
                        sql = from regime in db.Regime
                              where regime.id_regime_ativo == true || (cd_regime.HasValue && regime.cd_regime == cd_regime.Value)
                              orderby regime.no_regime
                              select regime;
                        break;
                    case TipoConsultaRegimeEnum.HAS_TURMA:
                        sql = from regime in db.Regime
                              where regime.Turma.Any()
                              orderby regime.no_regime
                              select regime;
                        break;
                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<Regime> getRegimeTabelaPreco()
        {
            try{
                IQueryable<Regime> sql;

                sql = (from regime in db.Regime
                      join tabela in db.TabelaPreco
                      on regime.cd_regime equals tabela.cd_regime
                      select regime).Distinct().OrderBy(r => r.no_regime);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
