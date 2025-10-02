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
    public class AvaliacaoParticipacaoVincDataAccess : GenericRepository<AvaliacaoParticipacaoVinc>, IAvaliacaoParticipacaoVincDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public int verificaMaiorQntParticipacao(int idProduto, int cdEscola)
        {
            try
            {
                //Maior quantidade de participações por produto 
                List<int> avalPorProduto = (from p in db.AvaliacaoParticipacaoVinc
                                            where p.AvaliacaoParticipacao.cd_produto == idProduto &&
                                            p.cd_escola == cdEscola
                                            orderby p.cd_avaliacao_participacao
                                            select p.cd_avaliacao_participacao).ToList();

                int max = 0;
                int contador = 1;
                int quantidadeReg = avalPorProduto.Count();
                for (int i = 0; i < avalPorProduto.Count(); i++)
                {
                    if ((i + 1) < quantidadeReg && avalPorProduto[i] == avalPorProduto[i + 1])
                    {
                        contador++;
                    }
                    else
                    {
                        if (max < contador)
                            max = contador;
                        contador = 1;
                    }

                }
                return max;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<AvaliacaoParticipacaoVinc> getAvalPartVinc(int cdAvalPart, int cdEscola)
        {
            try
            {
                IEnumerable<AvaliacaoParticipacaoVinc> participacoes = (from p in db.AvaliacaoParticipacaoVinc
                                                                        where p.cd_escola == cdEscola &&
                                                                        p.cd_avaliacao_participacao == cdAvalPart
                                                                        orderby p.nm_ordem ascending
                                                                        select new
                                                                        {
                                                                            p.cd_avaliacao_participacao_vinc,
                                                                            p.cd_avaliacao_participacao,
                                                                            p.cd_participacao_avaliacao,
                                                                            no_participacao_avaliacao = p.ParticipacaoAvaliacao.no_participacao,
                                                                            p.id_avaliacao_participacao_ativa,
                                                                            p.nm_ordem
                                                                        }).ToList().Select(x => new AvaliacaoParticipacaoVinc
                                                                        {
                                                                            cd_avaliacao_participacao_vinc = x.cd_avaliacao_participacao_vinc,
                                                                            cd_avaliacao_participacao = x.cd_avaliacao_participacao,
                                                                            cd_participacao_avaliacao = x.cd_participacao_avaliacao,
                                                                            no_participacao_avaliacao = x.no_participacao_avaliacao,
                                                                            id_avaliacao_participacao_ativa = x.id_avaliacao_participacao_ativa,
                                                                            nm_ordem = x.nm_ordem
                                                                        });
                return participacoes;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
