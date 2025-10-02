using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum;
using Componentes.GenericModel;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AcaoFollowupDataAccess : GenericRepository<AcaoFollowUp>, IAcaoFollowupDataAccess
    {
        public enum TipoPesquisaAcaoEnum
        {
            HAS_ATIVO = 1,
            HAS_PESQ_FOLLOW_UP = 2,
            HAS_CAD_FOLLOW_UP = 3
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<AcaoFollowUp> GetAcaoFollowUpSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo)
        {
            try{
                IEntitySorter<AcaoFollowUp> sorter = EntitySorter<AcaoFollowUp>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AcaoFollowUp> sql;

                if (ativo == null)
                {
                    sql = from c in db.AcaoFollowUp.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.AcaoFollowUp.AsNoTracking()
                          where (c.id_acao_ativa == ativo)
                          select c;
                }
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_acao_follow_up.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_acao_follow_up.Contains(descricao)
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

        public IEnumerable<AcaoFollowUp> getAcaoFollowUp(TipoPesquisaAcaoEnum tipo, int cd_acao_follow_up)
        {
            try{
                IEnumerable<AcaoFollowUp> sql;
                sql = from c in db.AcaoFollowUp
                      select c;
                switch (tipo)
                {
                    case TipoPesquisaAcaoEnum.HAS_ATIVO:
                        sql = from c in db.AcaoFollowUp
                              where c.id_acao_ativa
                              select c;
                        break;
                    case TipoPesquisaAcaoEnum.HAS_CAD_FOLLOW_UP:
                        sql = from c in db.AcaoFollowUp
                              where c.id_acao_ativa || (cd_acao_follow_up > 0 && c.cd_acao_follow_up == cd_acao_follow_up)
                              select c;
                        break;
                }
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
