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
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;
using System.Data.Entity.Core.Objects;
using System.Data.Objects.SqlClient;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class PoliticaComercialDataAccess : GenericRepository<PoliticaComercial>, IPoliticaComercialDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public PoliticaComercial getPoliticaComercialById(int cdPoliticaComercial, int cdEscola)
        {
            try
            {
                PoliticaComercial sql = (from pt in db.PoliticaComercial.Include(i => i.ItemPolitica)
                                         where pt.cd_politica_comercial == cdPoliticaComercial &&
                                            pt.cd_pessoa_empresa == cdEscola
                                         select pt).FirstOrDefault();
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PoliticaComercial> getPoliticaComercialByEmpresa(int cd_pessoa_escola, string dc_politica, bool inicio)
        {
            try
            {
                var sql = (from politicaComercial in db.PoliticaComercial
                           where politicaComercial.cd_pessoa_empresa == cd_pessoa_escola && politicaComercial.id_politica_ativa == true
                           orderby politicaComercial.dc_politica_comercial
                           select new
                           {
                               cd_politica_comercial = politicaComercial.cd_politica_comercial,
                               dc_politica_comercial = politicaComercial.dc_politica_comercial,
                               nm_parcelas = politicaComercial.nm_parcelas,
                               nm_intervalo_parcela = politicaComercial.nm_periodo_intervalo
                           }).ToList().Select(x => new PoliticaComercial
                          {
                              cd_politica_comercial = x.cd_politica_comercial,
                              dc_politica_comercial = x.dc_politica_comercial,
                              nm_parcelas = x.nm_parcelas,
                              nm_intervalo_parcela = x.nm_intervalo_parcela ?? (byte)0
                          }).ToList();

                var retorno = from c in sql
                              select c;

                if (!String.IsNullOrEmpty(dc_politica))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_politica_comercial.StartsWith(dc_politica)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_politica_comercial.Contains(dc_politica)
                                  select c;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PoliticaComercial> getPoliticaComercialSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, bool parcIguais, bool vencFixo, int cdEscola)
        {
            try
            {
                IEntitySorter<PoliticaComercial> sorter = EntitySorter<PoliticaComercial>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PoliticaComercial> sql;
                sql = from pc in db.PoliticaComercial.AsNoTracking()
                      where pc.cd_pessoa_empresa == cdEscola
                      select pc;


                if (ativo != null)
                    sql = from c in sql
                          where (c.id_politica_ativa == ativo)
                          select c;

                if (parcIguais)
                    sql = from c in sql
                          where (c.id_parcela_igual == parcIguais)
                          select c;

                if (vencFixo)
                    sql = from c in sql
                          where (c.id_vencimento_fixo == vencFixo)
                          select c;

                

                sql = sorter.Sort(sql);

                var retorno = from c in sql
                              select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.dc_politica_comercial.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.dc_politica_comercial.Contains(descricao)
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

        public PoliticaComercial getPoliticaComercialSugeridaNF(int cdEscola)
        {
            try
            {
                PoliticaComercial sql = (from pt in db.PoliticaComercial
                                         where pt.cd_pessoa_empresa == cdEscola &&
                                               pt.Parametro.Where(p => p.cd_politica_comercial_nf != null).Any()
                                         select new {
                                             cd_politica_comercial = pt.cd_politica_comercial,
                                             dc_politica_comercial = pt.dc_politica_comercial
                                         }).ToList().Select( x => new PoliticaComercial
                                         {
                                             cd_politica_comercial = x.cd_politica_comercial,
                                             dc_politica_comercial = x.dc_politica_comercial
                                         }).FirstOrDefault();
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}