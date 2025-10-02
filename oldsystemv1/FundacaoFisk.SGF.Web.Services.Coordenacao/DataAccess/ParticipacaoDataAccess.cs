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
    public class ParticipacaoDataAccess : GenericRepository<Participacao>, IParticipacaoDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public List<Participacao> getParticipacaoAvaliacaoPart(int cdEscola)
        {
            try
            {
                //Maior quantidade de participações por produto 
                List<Participacao> listParticipacao = (from p in db.Participacao
                                            where p.id_participacao_ativa &&
                                            p.AvaliacaoParticipacaoVinc.Any(c => c.cd_escola == cdEscola)
                                            orderby p.cd_participacao
                                            select p).ToList();
                return listParticipacao;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Participacao> getParticipacaoByAvaliacao(int cdAvalPart, int cdEscola)
        {
            try
            {
                //Maior quantidade de participações por produto 
                List<Participacao> listParticipacao = (from p in db.Participacao
                                                                where p.AvaliacaoParticipacaoVinc.Any(c => c.cd_escola == cdEscola && c.cd_avaliacao_participacao == cdAvalPart)
                                                                orderby p.cd_participacao
                                                                select p).ToList();
                return listParticipacao;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Participacao> getParticipacoes(List<int> cdsPart)
        {
            try
            {
                //Maior quantidade de participações por produto 
                List<Participacao> listParticipacao = (from p in db.Participacao
                                                                where !cdsPart.Contains(p.cd_participacao) && 
                                                                p.id_participacao_ativa
                                                                orderby p.cd_participacao
                                                                select p).ToList();
                return listParticipacao;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Participacao> getParticipacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status)
        {
            try
            {
                IEntitySorter<Participacao> sorter = EntitySorter<Participacao>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Participacao> sql;

                sql = from p in db.Participacao.AsNoTracking()
                      select p;

                if (status != null)
                    sql = from p in sql
                          where p.id_participacao_ativa == status
                          select p;

                if (!String.IsNullOrEmpty(desc))
                    if (inicio)
                        sql = from p in sql
                              where p.no_participacao.StartsWith(desc)
                              select p;
                    else
                        sql = from p in sql
                              where p.no_participacao.Contains(desc)
                              select p;

                var retorno = sorter.Sort(sql);

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                var retorno2 = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return retorno2;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
