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
    public class SubgrupoContaDataAccess : GenericRepository<SubgrupoConta>, ISubgrupoContaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<SubGrupoSort> GetSubgrupoContaSearch(SearchParameters parametros, string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo)
        {
            try
            {
                IEntitySorter<SubGrupoSort> sorter = EntitySorter<SubGrupoSort>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<SubgrupoConta> sql;
                IQueryable<SubGrupoSort> retorno;
                sql = from c in db.SubgrupoConta.AsNoTracking()
                      orderby c.no_subgrupo_conta ascending
                      select c;

                if (tipo == SubgrupoConta.TipoNivelConsulta.UM_NIVEL)
                {
                    sql = from s in sql
                          where s.cd_subgrupo_pai == null && !s.SubgruposFilhos.Any()
                          select s;

                    if (cdGrupo > 0)
                        sql = from c in sql
                              where c.cd_grupo_conta == cdGrupo
                              select c;
                }
                else
                {
                    if (tipo == SubgrupoConta.TipoNivelConsulta.DOIS_NIVEIS)
                    {
                        sql = from s in sql
                              where s.cd_subgrupo_pai != null
                              select s;

                        if (cdGrupo > 0)
                            sql = from c in sql
                                  where c.SubgrupoPai.cd_grupo_conta == cdGrupo
                                  select c;
                    }
                }

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from s in sql
                              where (s.no_subgrupo_conta.StartsWith(descricao) || s.SubgrupoPai.no_subgrupo_conta.Contains(descricao))// || s.SubgruposFilhos.Where(sub => sub.no_subgrupo_conta.StartsWith(descricao)).Any())
                              select s;
                    else
                        sql = from s in sql
                              where s.no_subgrupo_conta.Contains(descricao) || s.SubgrupoPai.no_subgrupo_conta.Contains(descricao) //|| s.SubgruposFilhos.Where(sub => sub.no_subgrupo_conta.Contains(descricao)).Any()
                              select s;
                retorno = from s in sql
                          orderby s.no_subgrupo_conta ascending
                          select new SubGrupoSort{
                              no_subgrupo_conta = s.no_subgrupo_conta,
                              cd_grupo_conta = s.cd_grupo_conta != null ? s.cd_grupo_conta : s.SubgrupoPai.cd_grupo_conta,
                              cd_subgrupo_conta = s.cd_subgrupo_conta,
                              cd_subgrupo_pai = s.cd_subgrupo_pai,
                              nm_ordem_subgrupo = s.nm_ordem_subgrupo,
                              SubgrupoContaGrupo = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo : s.SubgrupoPai.SubgrupoContaGrupo,
                              SubgrupoPai = s.SubgrupoPai,
                              SubgruposFilhos = s.SubgruposFilhos,
                              grupo_conta = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo.no_grupo_conta: s.SubgrupoPai.SubgrupoContaGrupo.no_grupo_conta,
                              id_tipo_grupo_conta = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo.id_tipo_grupo_conta : s.SubgrupoPai.SubgrupoContaGrupo.id_tipo_grupo_conta,
                              dc_cod_integracao_plano = s.dc_cod_integracao_plano
                          };
                retorno = sorter.Sort(retorno);
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
       
        public bool deleteAllSubgrupoConta(List<SubgrupoConta> SubgrupoConta)
        {
            try{
                var strSubgrupoConta = "";
                if (SubgrupoConta != null && SubgrupoConta.Count > 0)
                {
                    foreach (SubgrupoConta tipo in SubgrupoConta)
                    {
                        strSubgrupoConta += tipo.cd_subgrupo_conta + ","; 
                    }                
                }
                if (strSubgrupoConta.Length > 0)
                {
                   strSubgrupoConta = strSubgrupoConta.Substring(0, strSubgrupoConta.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_subgrupo_conta where cd_subgrupo_conta in(" + strSubgrupoConta + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SubgrupoConta> getSubgruposPorCodGrupoContas(int cdGrupoContas)
        {
            try
            {
                var sql = from sg in db.SubgrupoConta
                          where sg.cd_grupo_conta == cdGrupoContas &&
                                sg.cd_subgrupo_pai == null
                                orderby sg.no_subgrupo_conta
                          select sg;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public SubGrupoSort getSubgrupoPorNivelEId(SubgrupoConta.TipoNivelConsulta nivel, int cdSubgrupo)
        {
            try
            {
                SubGrupoSort sql = null;
                if (nivel > 0)
                    switch (nivel)
                    {
                        case SubgrupoConta.TipoNivelConsulta.UM_NIVEL:
                            sql = (from s in db.SubgrupoConta
                                   where s.cd_subgrupo_pai == null && s.cd_subgrupo_conta == cdSubgrupo //&&
                                   //!s.SubgruposFilhos.Any()
                                   select new SubGrupoSort
                                   {
                                       no_subgrupo_conta = s.no_subgrupo_conta,
                                       cd_grupo_conta = s.cd_grupo_conta != null ? s.cd_grupo_conta : s.SubgrupoPai.cd_grupo_conta,
                                       cd_subgrupo_conta = s.cd_subgrupo_conta,
                                       cd_subgrupo_pai = s.cd_subgrupo_pai,
                                       nm_ordem_subgrupo = s.nm_ordem_subgrupo,
                                       SubgrupoContaGrupo = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo : s.SubgrupoPai.SubgrupoContaGrupo,
                                       SubgrupoPai = s.SubgrupoPai,
                                       SubgruposFilhos = s.SubgruposFilhos,
                                       grupo_conta = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo.no_grupo_conta : s.SubgrupoPai.SubgrupoContaGrupo.no_grupo_conta,
                                       id_tipo_grupo_conta = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo.id_tipo_grupo_conta : s.SubgrupoPai.SubgrupoContaGrupo.id_tipo_grupo_conta,
                                       dc_cod_integracao_plano = s.dc_cod_integracao_plano
                                   }).FirstOrDefault();
                            break;
                        case SubgrupoConta.TipoNivelConsulta.DOIS_NIVEIS:
                            sql = (from s in db.SubgrupoConta
                                   where s.cd_subgrupo_pai != null && s.cd_subgrupo_conta == cdSubgrupo
                                   select new SubGrupoSort
                                    {
                                        no_subgrupo_conta = s.no_subgrupo_conta,
                                        cd_grupo_conta = s.cd_grupo_conta != null ? s.cd_grupo_conta : s.SubgrupoPai.cd_grupo_conta,
                                        cd_subgrupo_conta = s.cd_subgrupo_conta,
                                        cd_subgrupo_pai = s.cd_subgrupo_pai,
                                        nm_ordem_subgrupo = s.nm_ordem_subgrupo,
                                        SubgrupoContaGrupo = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo : s.SubgrupoPai.SubgrupoContaGrupo,
                                        SubgrupoPai = s.SubgrupoPai,
                                        SubgruposFilhos = s.SubgruposFilhos,
                                        grupo_conta = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo.no_grupo_conta : s.SubgrupoPai.SubgrupoContaGrupo.no_grupo_conta,
                                        id_tipo_grupo_conta = s.SubgrupoContaGrupo != null ? s.SubgrupoContaGrupo.id_tipo_grupo_conta : s.SubgrupoPai.SubgrupoContaGrupo.id_tipo_grupo_conta,
                                        dc_cod_integracao_plano = s.dc_cod_integracao_plano
                                    }).FirstOrDefault();
                            break;
                    }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SubgrupoConta> getSubgruposContaAll(List<int> codigosSubGrupos)
        {
            try
            {
                var sql = from sg in db.SubgrupoConta
                          where codigosSubGrupos.Contains(sg.cd_subgrupo_conta)
                          select sg;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}


