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
using FundacaoFisk.SGF.Services.Coordenacao.Model;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AvaliacaoParticipacaoDataAccess : GenericRepository<AvaliacaoParticipacao>, IAvaliacaoParticipacaoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<AvaliacaoParticipacaoUI> searchAvaliacaoParticipacao(Componentes.Utils.SearchParameters parametros, int cdCriterio, int cdParticipacao, int cdProduto, bool? ativo, int cdEscola)
        {
            try
            {   IEntitySorter<AvaliacaoParticipacaoUI> sorter = null;
                if (parametros.sort != null)
                {
                    sorter = EntitySorter<AvaliacaoParticipacaoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                }
                //IEnumerable<AvaliacaoParticipacao> retorno;
                IQueryable<AvaliacaoParticipacao> sql;
                IQueryable<AvaliacaoParticipacaoUI> sql0;

                sql = from avaliacaoParticipacao in db.AvaliacaoParticipacao.AsNoTracking()                    
                      where avaliacaoParticipacao.AvaliacaoParticipacaoVinc.Where(v => v.cd_escola == cdEscola).Any()
                      select avaliacaoParticipacao;

                if (cdCriterio > 0)
                    sql = from s in sql
                          where s.cd_criterio_avaliacao == cdCriterio
                          select s;
                if (cdParticipacao > 0)
                    sql = from s in sql
                          where s.AvaliacaoParticipacaoVinc.Where(v => v.cd_participacao_avaliacao == cdParticipacao && v.cd_escola == cdEscola).Any()
                          select s;
                if (cdProduto > 0)
                    sql = from s in sql
                          where s.cd_produto == cdProduto
                          select s;

                if (ativo != null)
                    sql = from s in sql
                          where s.AvaliacaoParticipacaoVinc.Where(v => v.id_avaliacao_participacao_ativa == ativo && v.cd_escola == cdEscola).Any()
                          select s;

                sql0 = from avaliacaoParticipacao in sql
                       join p in db.AvaliacaoParticipacaoVinc                     
                       on avaliacaoParticipacao.cd_avaliacao_participacao equals p.cd_avaliacao_participacao
                       where p.cd_escola == cdEscola && (cdParticipacao == 0 || p.cd_participacao_avaliacao == cdParticipacao) && (ativo == null || p.id_avaliacao_participacao_ativa == ativo)
                       orderby avaliacaoParticipacao.Produto.cd_produto, avaliacaoParticipacao.CriterioAvaliacao.dc_criterio_avaliacao ascending, p.nm_ordem ascending
                       select new AvaliacaoParticipacaoUI
                       {
                           cd_avaliacao_participacao_vinc = p.cd_avaliacao_participacao_vinc,
                           cd_avaliacao_participacao = avaliacaoParticipacao.cd_avaliacao_participacao,
                           cd_produto = avaliacaoParticipacao.cd_produto,
                           no_produto = avaliacaoParticipacao.Produto.no_produto,
                           cd_criterio_avaliacao = avaliacaoParticipacao.cd_criterio_avaliacao,
                           dc_criterio_avaliacao = avaliacaoParticipacao.CriterioAvaliacao.dc_criterio_avaliacao,
                           no_participacao_avaliacao = p.ParticipacaoAvaliacao.no_participacao,
                           id_avaliacao_participacao_ativa = p.id_avaliacao_participacao_ativa ? "Sim" : "Não",
                           nm_ordem = p.nm_ordem
                       };


                if (sorter != null)
                {
                    sql0 = sorter.Sort(sql0);
                }
                

                var retorno = from AvaliacaoParticipacao in sql0
                              select AvaliacaoParticipacao;

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

        public IEnumerable<AvaliacaoParticipacao> getAvaliacaoParticipacaoById(int cdAvalPart, int cdEscola)
        {

            try
            {
                IEnumerable<AvaliacaoParticipacao> avaliacaoPart = (from avaliacaoParticipacao in db.AvaliacaoParticipacao
                                                                    join p in db.AvaliacaoParticipacaoVinc
                                                                    on avaliacaoParticipacao.cd_avaliacao_participacao equals p.cd_avaliacao_participacao
                                                                    where p.cd_escola == cdEscola &&
                                                                    avaliacaoParticipacao.cd_avaliacao_participacao == cdAvalPart
                                                                    select new

                                                                    {
                                                                        cd_avaliacao_participacao_vinc = p.cd_avaliacao_participacao_vinc,
                                                                        avaliacaoParticipacao.cd_avaliacao_participacao,
                                                                        avaliacaoParticipacao.cd_produto,
                                                                        no_produto = avaliacaoParticipacao.Produto.no_produto,
                                                                        avaliacaoParticipacao.cd_criterio_avaliacao,
                                                                        dc_criterio_avaliacao = avaliacaoParticipacao.CriterioAvaliacao.dc_criterio_avaliacao,
                                                                        no_participacao_avaliacao = p.ParticipacaoAvaliacao.no_participacao,
                                                                        id_avaliacao_participacao_ativa = p.id_avaliacao_participacao_ativa ? "Sim" : "Não"

                                                                    }).ToList().Select(x => new AvaliacaoParticipacao
                                                       {
                                                           cd_avaliacao_participacao_vinc = x.cd_avaliacao_participacao_vinc,
                                                           cd_avaliacao_participacao = x.cd_avaliacao_participacao,
                                                           cd_produto = x.cd_produto,
                                                           no_produto = x.no_produto,
                                                           cd_criterio_avaliacao = x.cd_criterio_avaliacao,
                                                           dc_criterio_avaliacao = x.dc_criterio_avaliacao,
                                                           no_participacao_avaliacao = x.no_participacao_avaliacao,
                                                           id_avaliacao_participacao_ativa = x.id_avaliacao_participacao_ativa
                                                       });
                return avaliacaoPart;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
            
        public AvaliacaoParticipacao getAvaliacaoParticipacaoByEditar(int cdAvalPart, int cdEscola)
        {

            try
            {
                AvaliacaoParticipacao avaliacaoPart = (from avaliacaoParticipacao in db.AvaliacaoParticipacao
                                                       where avaliacaoParticipacao.AvaliacaoParticipacaoVinc.Where(av => av.cd_escola == cdEscola).Any() &&
                                                       avaliacaoParticipacao.cd_avaliacao_participacao == cdAvalPart
                                                       select new

                                                       {
                                                           avaliacaoParticipacao.cd_avaliacao_participacao,
                                                           avaliacaoParticipacao.cd_produto,
                                                           no_produto = avaliacaoParticipacao.Produto.no_produto,
                                                           avaliacaoParticipacao.cd_criterio_avaliacao,
                                                           id_ativa = avaliacaoParticipacao.AvaliacaoParticipacaoVinc.Select(a => a.id_avaliacao_participacao_ativa).FirstOrDefault()
                                                       }).ToList().Select(x => new AvaliacaoParticipacao
                                                       {
                                                           cd_avaliacao_participacao = x.cd_avaliacao_participacao,
                                                           cd_produto = x.cd_produto,
                                                           no_produto = x.no_produto,
                                                           cd_criterio_avaliacao = x.cd_criterio_avaliacao,
                                                           id_ativa = x.id_ativa
                                                       }).FirstOrDefault();
                return avaliacaoPart;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
                
        }

        public bool verificaAvaliacaoParticipacaoByCriterio(int cd_criterio_avaliacao)
        {
            try
            {
                return db.AvaliacaoParticipacao.Any(x => x.cd_criterio_avaliacao.Equals(cd_criterio_avaliacao));
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
