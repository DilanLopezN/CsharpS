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
using System.Data.Entity.Core.Objects;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class GrupoContaDataAccess : GenericRepository<GrupoConta>, IGrupoContaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<GrupoConta> GetGrupoContaSearch(SearchParameters parametros, string descricao, bool inicio, int tipoGrupo)
        {
            try
            {
                IEntitySorter<GrupoConta> sorter = EntitySorter<GrupoConta>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<GrupoConta> sql;
                sql = from c in db.GrupoConta.AsNoTracking()
                      select c;

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (tipoGrupo > 0)
                    retorno = from c in sql
                              where c.id_tipo_grupo_conta == tipoGrupo
                              select c;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_grupo_conta.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_grupo_conta.Contains(descricao)
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

        public bool deleteAllGrupoConta(List<GrupoConta> grupoConta)
        {
            try
            {
                var strGrupoConta = "";
                if (grupoConta != null && grupoConta.Count > 0)
                {
                    foreach (GrupoConta tipo in grupoConta)
                    {
                        strGrupoConta += tipo.cd_grupo_conta + ",";
                    }
                }
                if (strGrupoConta.Length > 0)
                {
                    strGrupoConta = strGrupoConta.Substring(0, strGrupoConta.Length - 1);
                }
                int retorno = db.Database.ExecuteSqlCommand("delete from t_grupo_conta where cd_grupo_conta in(" + strGrupoConta + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<GrupoConta> getListaContas(int cd_grupo_conta, string no_subgrupo_conta, bool inicio, int nivel, int tipoPlanoConta, int cd_pessoa_empresa)
        {
            try
            {
                List<GrupoConta> retorno = new List<GrupoConta>();
                var subGrupos = from subGrupo in db.SubgrupoConta.Include(sg => sg.SubgrupoPai.SubgrupoContaGrupo).Include(sg => sg.SubgrupoContaGrupo)
                             where !subGrupo.SubgrupoPlanoConta.Any(s => s.cd_subgrupo_conta == s.PlanoContaSubgrupo.cd_subgrupo_conta && s.cd_pessoa_empresa == cd_pessoa_empresa)
                             select subGrupo;

                
                if (nivel == (int)SubgrupoConta.TipoNivelConsulta.UM_NIVEL)
                    subGrupos = from s in subGrupos
                                 //where EntityFunctions.Equals(s.cd_grupo_conta, teste)
                                 where s.SubgrupoContaGrupo != null
                                 && s.SubgruposFilhos.Count == 0
                                 select s;

                if (nivel == (int)SubgrupoConta.TipoNivelConsulta.DOIS_NIVEIS)
                    subGrupos = from s in subGrupos
                                 where s.SubgrupoContaGrupo == null
                                 select s;

                foreach (SubgrupoConta sbg in subGrupos)
                    incluiRecursivoGrupoContas(sbg, retorno);

                var sql = from s in retorno
                          select s;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<GrupoConta> getGrupoContasWithPlanoContas(int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from grupoConta in db.GrupoConta
                           join subGrupoConta in db.SubgrupoConta on grupoConta.cd_grupo_conta equals subGrupoConta.cd_grupo_conta
                           join planoConta in db.PlanoConta on subGrupoConta.cd_subgrupo_conta equals planoConta.cd_subgrupo_conta
                           where planoConta.cd_pessoa_empresa == cd_pessoa_empresa
                           orderby grupoConta.nm_ordem_grupo, grupoConta.id_tipo_grupo_conta, grupoConta.no_grupo_conta, subGrupoConta.nm_ordem_subgrupo, subGrupoConta.no_subgrupo_conta
                           select new
                           {
                               cd_grupo_conta = grupoConta.cd_grupo_conta,
                               no_grupo_conta = grupoConta.no_grupo_conta
                           }).ToList().Select(x => new GrupoConta
                           {
                               cd_grupo_conta = x.cd_grupo_conta,
                               no_grupo_conta = x.no_grupo_conta
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getGrupoContasWhitOutPlanoContas(byte nivel, int cd_pessoa_empresa)
        {
            try
            {
                bool existe = false;
                var subGrupos = from subGrupo in db.SubgrupoConta
                                where !subGrupo.SubgrupoPlanoConta.Any(s => s.cd_subgrupo_conta == s.PlanoContaSubgrupo.cd_subgrupo_conta && s.cd_pessoa_empresa == cd_pessoa_empresa)
                                select subGrupo;

                if (nivel == (int)SubgrupoConta.TipoNivelConsulta.UM_NIVEL)
                    existe = (from s in subGrupos
                                //where EntityFunctions.Equals(s.cd_grupo_conta, teste)
                                where s.SubgrupoContaGrupo != null
                                && s.SubgruposFilhos.Count == 0
                                select s.cd_subgrupo_conta).Any();
                else
                    existe = (from s in subGrupos
                                where s.SubgrupoContaGrupo == null
                                select s.cd_subgrupo_conta).Any();

                return existe;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Atenção: Esse método é usado para buscar e montar os grupo de contas que estão no plano de contas("Plano Contas"), se alterar deve fazer teste para ver se o plano de contas vai sofrer alguma alteração.
        public IEnumerable<GrupoConta> getPlanoContasTreeSearch(int cd_escola, bool busca_somente_ativo, bool isDireitoContaSeg, string descricao, bool inicio)
        {
            try
            {    
                IQueryable<PlanoConta> planos;
                List<GrupoConta> retorno = new List<GrupoConta>();

                if (isDireitoContaSeg)
                    planos = (from p in db.PlanoConta.Include(plano => plano.PlanoContaSubgrupo.SubgrupoPai.SubgrupoContaGrupo) //2 Niveis
                                                     .Include(plano => plano.PlanoContaSubgrupo.SubgrupoContaGrupo) // 1 Nivel
                              where p.cd_pessoa_empresa == cd_escola
                                && (p.id_ativo || !busca_somente_ativo)
                              select p);
                else
                    planos = (from p in db.PlanoConta.Include(plano => plano.PlanoContaSubgrupo.SubgrupoPai.SubgrupoContaGrupo) //2 Niveis
                                                   .Include(plano => plano.PlanoContaSubgrupo.SubgrupoContaGrupo) // 1 Nivel
                              where p.cd_pessoa_empresa == cd_escola
                                && (p.id_ativo || !busca_somente_ativo)
                                && (!p.id_conta_segura)
                              select p);

                if(!string.IsNullOrEmpty(descricao))
                    if(!inicio)
                        planos = from p in planos
                                 where p.PlanoContaSubgrupo.no_subgrupo_conta.Contains(descricao)
                                 select p;
                    else
                        planos = from p in planos
                                 where p.PlanoContaSubgrupo.no_subgrupo_conta.StartsWith(descricao)
                                 select p;
 
                //Inverte para ficar em forma de árvore:
                foreach (PlanoConta plano in planos)
                    incluiRecursivoGrupoContas(plano.PlanoContaSubgrupo, retorno);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private void incluiRecursivoGrupoContas(SubgrupoConta subGrupoContas, List<GrupoConta> gruposConta)
        {
            if (subGrupoContas.SubgrupoPai == null)
            {
                if (subGrupoContas.SubgrupoContaGrupo != null && !gruposConta.Contains(subGrupoContas.SubgrupoContaGrupo, new FundacaoFisk.SGF.GenericModel.GrupoConta.GrupoContaComparer()))
                    gruposConta.Add(subGrupoContas.SubgrupoContaGrupo);
            }
            else
                incluiRecursivoGrupoContas(subGrupoContas.SubgrupoPai, gruposConta);
        }

        public IEnumerable<GrupoConta> getPlanoContasTreeSearchWhitMovimento(int cd_escola, int tipoMovimento, string descricao, bool inicio)
        {
            try
            {
                List<GrupoConta> retorno = new List<GrupoConta>();
                var planos = (from p in db.PlanoConta.Include(plano => plano.PlanoContaSubgrupo.SubgrupoPai.SubgrupoContaGrupo) //2 Niveis
                                                  .Include(plano => plano.PlanoContaSubgrupo.SubgrupoContaGrupo) // 1 Nivel
                              where p.cd_pessoa_empresa == cd_escola
                              && (p.ItensMovimento.Any(im => im.Movimento.cd_movimento == im.cd_movimento
                                                          && im.cd_plano_conta == p.cd_plano_conta
                                                          && (im.Movimento.id_tipo_movimento == tipoMovimento || tipoMovimento == (int)Movimento.TipoMovimentoEnum.DEVOLUCAO)))
                              select p);

                if(!string.IsNullOrEmpty(descricao))
                    if(!inicio)
                        planos = from p in planos
                                 where p.PlanoContaSubgrupo.no_subgrupo_conta.Contains(descricao)
                                 select p;
                    else
                        planos = from p in planos
                                 where p.PlanoContaSubgrupo.no_subgrupo_conta.StartsWith(descricao)
                                 select p;

                //Inverte para ficar em forma de árvore:
                foreach (PlanoConta plano in planos)
                    incluiRecursivoGrupoContas(plano.PlanoContaSubgrupo, retorno);

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<GrupoConta> getPlanoContasTreeSearchWhitContaCorrente(int cd_escola, string descricao, bool inicio)
        {
            try
            {
                List<GrupoConta> retorno = new List<GrupoConta>();
                var planos = (from p in db.PlanoConta.Include(plano => plano.PlanoContaSubgrupo.SubgrupoPai.SubgrupoContaGrupo) //2 Niveis
                                                  .Include(plano => plano.PlanoContaSubgrupo.SubgrupoContaGrupo) // 1 Nivel
                              where p.cd_pessoa_empresa == cd_escola
                                 && p.ContaCorrente.Any(c => c.cd_plano_conta == p.cd_plano_conta)
                              select p);

                if(!string.IsNullOrEmpty(descricao))
                    if(!inicio)
                        planos = from p in planos
                                 where p.PlanoContaSubgrupo.no_subgrupo_conta.Contains(descricao)
                                 select p;
                    else
                        planos = from p in planos
                                 where p.PlanoContaSubgrupo.no_subgrupo_conta.StartsWith(descricao)
                                 select p;

                //Inverte para ficar em forma de árvore:
                foreach (PlanoConta plano in planos)
                    incluiRecursivoGrupoContas(plano.PlanoContaSubgrupo, retorno);

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<GrupoConta> getSubgrupoContaSearchFK(string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo, bool contaSegura, int cdEscola)
        {
            try
            {
                IQueryable<SubgrupoConta> sql;
                List<GrupoConta> retorno = new List<GrupoConta>();
                sql = from c in db.SubgrupoConta.Include(p => p.SubgrupoContaGrupo).Include(s => s.SubgrupoPai).Include(s => s.SubgrupoPai.SubgrupoContaGrupo)
                      orderby c.no_subgrupo_conta ascending
                      select c;

                if (!contaSegura)
                    sql = from t in sql
                          where
                          t.SubgrupoPlanoConta.Where(pc => t.ItemSubgrupos.Where(isg => pc.cd_subgrupo_conta == isg.cd_subgrupo_conta).Any() &&
                                                                       pc.cd_pessoa_empresa == cdEscola && !pc.id_conta_segura).Any()
                          select t;

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

                //Inverte para ficar em forma de árvore:
                List<SubgrupoConta> sql1 = sql.ToList();
                foreach (SubgrupoConta sub in sql1)
                    incluiRecursivoGrupoContas(sub, retorno);
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
