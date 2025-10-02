using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TipoDescontoDataAccess : GenericRepository<TipoDesconto>, ITipoDescontoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<TipoDescontoUI> GetTipoDescontoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, bool? incideBaixa, bool? pparc, decimal? percentual, int cdEscola)
        {
            try
            {
                IEntitySorter<TipoDescontoUI> sorter = EntitySorter<TipoDescontoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<TipoDesconto> sql;


                if (ativo == null)
                {
                    sql = from c in db.TipoDesconto.AsNoTracking()
                          select c;
                }
                else
                {
                    sql = from c in db.TipoDesconto.AsNoTracking()
                          where c.TiposDescontoEscola.Where(p => p.cd_pessoa_escola == cdEscola && p.id_tipo_desconto_ativo == ativo).Any()
                          select c;
                }

                if (incideBaixa != null)
                    sql = from c in sql
                          where c.TiposDescontoEscola.Where(p => p.cd_pessoa_escola == cdEscola && p.id_incide_baixa == incideBaixa).Any()
                          select c;

                if (pparc != null)
                    sql = from c in sql
                          where c.TiposDescontoEscola.Where(p => p.cd_pessoa_escola == cdEscola && p.id_incide_parcela_1 == pparc).Any()
                          select c;
                if (percentual != null)
                    sql = from c in sql
                          where c.TiposDescontoEscola.Where(p => p.cd_pessoa_escola == cdEscola && p.pc_desconto == percentual).Any()
                          select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_tipo_desconto.StartsWith(descricao)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_tipo_desconto.Contains(descricao)
                              select c;


                IQueryable<TipoDescontoUI> sql1 = from c in sql
                                                  //join td in db.TipoDescontoEscola on c.cd_tipo_desconto equals td.cd_tipo_desconto into tiposDescontos
                                                  join i in db.TipoDescontoEscola on 
                                                  new { c.cd_tipo_desconto, cd_pessoa_escola = cdEscola } equals 
                                                  new { i.cd_tipo_desconto, i.cd_pessoa_escola } into tiposDescontos
                                                  from tpd in tiposDescontos.DefaultIfEmpty()
                                                  select new TipoDescontoUI
                                                  {
                                                      cd_tipo_desconto = c.cd_tipo_desconto,
                                                      dc_tipo_desconto = c.dc_tipo_desconto,
                                                      cd_pessoa_escola = tpd != null ? tpd.cd_pessoa_escola : 0,
                                                      cd_tipo_desconto_escola = tpd != null ? tpd.cd_tipo_desconto_escola : 0,
                                                      id_tipo_desconto_ativo = tpd != null ? tpd.id_tipo_desconto_ativo : false,
                                                      pc_desconto = tpd.pc_desconto,
                                                      id_incide_parcela_1 = tpd != null ? tpd.id_incide_parcela_1 : false,
                                                      id_incide_baixa = tpd != null ? tpd.id_incide_baixa : false
                                                  };

                sql1 = sorter.Sort(sql1);

                if (ativo != null)
                {
                    sql1 = sql1.Where(x => x.id_tipo_desconto_ativo == ativo);
                }

                var retorno = from c in sql1
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


        public bool deleteAllTipoDesconto(List<TipoDesconto> tiposDesconto)
        {
            try
            {
                var strTipoDesconto = "";
                if (tiposDesconto != null && tiposDesconto.Count > 0)
                {
                    foreach (TipoDesconto tipo in tiposDesconto)
                    {
                        strTipoDesconto += tipo.cd_tipo_desconto + ",";
                    }
                }
                if (strTipoDesconto.Length > 0)
                {
                    strTipoDesconto = strTipoDesconto.Substring(0, strTipoDesconto.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_tipo_desconto where cd_tipo_desconto in(" + strTipoDesconto + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoDesconto getTipoDescontoByContrato(int cd_desconto_contrato)
        {
            try
            {
                return (from tf in db.TipoDesconto
                        //where tf.DescontoContrato.Where(dc => dc.cd_desconto_contrato == cd_desconto_contrato).Any()
                        select tf).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoDesconto getTipoDescontoByIdComTipoDescontoEscola(int cdTipoDesconto)
        {
            try
            {
                TipoDesconto tpDesc = (from tp in db.TipoDesconto
                                       where tp.cd_tipo_desconto == cdTipoDesconto
                                       select new
                                         {
                                             cd_tipo_desconto = tp.cd_tipo_desconto,
                                             dc_tipo_desconto = tp.dc_tipo_desconto,
                                             TiposDescontoEscola = tp.TiposDescontoEscola
                                         }).ToList().Select(x => new TipoDesconto
                                         {
                                             cd_tipo_desconto = x.cd_tipo_desconto,
                                             dc_tipo_desconto = x.dc_tipo_desconto,
                                             TiposDescontoEscola = x.TiposDescontoEscola
                                         }).FirstOrDefault();

                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoDesconto getTipoDescontoComTipoDescontoEscola(int cd_escola, int cd_tipo_desconto)
        {
            try
            {
                TipoDesconto tpDesc = (from tp in db.TipoDesconto
                                       where tp.cd_tipo_desconto == cd_tipo_desconto
                                       select new
                                       {
                                           cd_tipo_desconto = tp.cd_tipo_desconto,
                                           dc_tipo_desconto = tp.dc_tipo_desconto,
                                           TiposDescontoEscola = tp.TiposDescontoEscola.Where(tde => tde.cd_pessoa_escola == cd_escola)
                                       }).ToList().Select(x => new TipoDesconto
                                       {
                                           cd_tipo_desconto = x.cd_tipo_desconto,
                                           dc_tipo_desconto = x.dc_tipo_desconto,
                                           TiposDescontoEscola = x.TiposDescontoEscola.ToList()
                                       }).FirstOrDefault();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TipoDescontoUI getTipoDescontoUIById(int cdTipoDesconto, int cdEscola, bool masterGeral)
        {
            try
            {
                TipoDescontoUI tpDesc = new TipoDescontoUI();
                if (!masterGeral)
                    tpDesc = (from c in db.TipoDesconto
                              where c.cd_tipo_desconto == cdTipoDesconto &&
                                    c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).Any()
                              select new TipoDescontoUI
                              {
                                  cd_tipo_desconto = c.cd_tipo_desconto,
                                  dc_tipo_desconto = c.dc_tipo_desconto,
                                  cd_pessoa_escola = cdEscola,
                                  cd_tipo_desconto_escola = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().cd_tipo_desconto_escola,
                                  id_tipo_desconto_ativo = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().id_tipo_desconto_ativo,
                                  pc_desconto = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().pc_desconto,
                                  id_incide_parcela_1 = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().id_incide_parcela_1,
                                  id_incide_baixa = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().id_incide_baixa
                              }).FirstOrDefault();
                else
                    tpDesc = (from c in db.TipoDesconto
                              where c.cd_tipo_desconto == cdTipoDesconto
                              select new TipoDescontoUI
                              {
                                  cd_tipo_desconto = c.cd_tipo_desconto,
                                  dc_tipo_desconto = c.dc_tipo_desconto,
                                  cd_pessoa_escola = cdEscola,
                                  cd_tipo_desconto_escola = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault() != null && c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().cd_tipo_desconto_escola > 0 ? c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().cd_tipo_desconto_escola : 0,
                                  id_tipo_desconto_ativo = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault() != null ? c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().id_tipo_desconto_ativo : true,
                                  pc_desconto = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault() != null && c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().pc_desconto > 0 ? c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().pc_desconto : null,
                                  id_incide_parcela_1 = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault() != null ? c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().id_incide_parcela_1 : false,
                                  id_incide_baixa = c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault() != null ? c.TiposDescontoEscola.Where(e => e.cd_pessoa_escola == cdEscola).FirstOrDefault().id_incide_baixa : false

                              }).FirstOrDefault();
                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getTipoDescontoNome(string dcTpDesc)
        {
            try
            {
                bool tpDesc = (from tp in db.TipoDesconto
                               where tp.dc_tipo_desconto == dcTpDesc
                               select tp.cd_tipo_desconto).Any();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getTipoDescontoNomeEsc(string dcTpDesc, int cdEscola)
        {
            try
            {
                bool tpDesc = (from tp in db.TipoDesconto
                               where tp.dc_tipo_desconto == dcTpDesc &&
                               tp.TiposDescontoEscola.Where(t => t.cd_pessoa_escola == cdEscola).Any()
                               select tp.cd_tipo_desconto).Any();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool getTipoDescontoMaster(int cdTpDesc)
        {
            try
            {
                bool tpDesc = (from tp in db.TipoDesconto
                               where tp.cd_tipo_desconto == cdTpDesc
                               select tp.id_master).FirstOrDefault();


                return tpDesc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
