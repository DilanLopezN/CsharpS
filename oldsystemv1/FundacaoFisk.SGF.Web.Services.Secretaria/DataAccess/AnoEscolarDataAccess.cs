using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{

    public class AnoEscolarDataAccess : GenericRepository<AnoEscolar>, IAnoEscolarDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }


        public IEnumerable<AnoEscolar> GetAnoEscolarSearch(SearchParameters parametros, int? cdEscolaridade, string descricao, bool inicio, bool? ativo)
        {
            try
            {
                IEntitySorter<AnoEscolar> sorter = EntitySorter<AnoEscolar>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AnoEscolar> sql;

                if (ativo == null)
                {
                    sql = from c in db.AnoEscolar.Include("Escolaridade").AsNoTracking()
                          where ((cdEscolaridade == null || cdEscolaridade == 0) || c.Escolaridade.cd_escolaridade == cdEscolaridade)
                          select c;
                }
                else
                {
                    sql = from c in db.AnoEscolar.Include("Escolaridade").AsNoTracking()
                          where (c.id_ativo == ativo && ((cdEscolaridade == null || cdEscolaridade == 0) || c.Escolaridade.cd_escolaridade == cdEscolaridade))
                          select c;
                }

                var retorno = (from anesc in sql
                               select new
                               {
                                   cd_ano_escolar = anesc.cd_ano_escolar,
                                   cd_escolaridade = anesc.cd_escolaridade,
                                   Escolaridade = anesc.Escolaridade,
                                   dc_ano_escolar = anesc.dc_ano_escolar,
                                   nm_ordem = anesc.nm_ordem,
                                   id_ativo = anesc.id_ativo
                               }).ToList().Select(x => new AnoEscolar
                               {
                                   cd_ano_escolar = x.cd_ano_escolar,
                                   cd_escolaridade = x.cd_escolaridade,
                                   no_escolaridade = x.Escolaridade.no_escolaridade,
                                   dc_ano_escolar = x.dc_ano_escolar,
                                   nm_ordem = x.nm_ordem,
                                   id_ativo = x.id_ativo
                               }).AsQueryable();

                retorno = sorter.Sort(retorno);

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in retorno
                                  where c.dc_ano_escolar.ToLower().StartsWith(descricao.ToLower())
                                  select c;
                    else
                        retorno = from c in retorno
                                  where c.dc_ano_escolar.ToLower().Contains(descricao.ToLower())
                                  select c;

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);

                parametros.qtd_limite = limite;
                return OrdenarPorNmOrdem(retorno);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        private IEnumerable<AnoEscolar> OrdenarPorNmOrdem(IQueryable<AnoEscolar> retorno)
        {
            var ordenado = new List<AnoEscolar>();
            var esc = retorno.Select(x => x.cd_escolaridade).Distinct();

            foreach (var cd_esc in esc)
            {
                ordenado.AddRange(retorno.Where(x => x.cd_escolaridade == cd_esc).OrderBy(y => y.nm_ordem));
            }
            return ordenado;
        }


        public IEnumerable<Escolaridade> GetEscolaridadePossuiAnoEscolar()
        {
            try
            {
                var sql = (from esc in db.Escolaridade
                           where esc.AnoEscolar.Any()
                           select new
                           {
                               esc.cd_escolaridade,
                               esc.no_escolaridade
                           }).ToList().Select(x => new Escolaridade
                           {
                               cd_escolaridade = x.cd_escolaridade,
                               no_escolaridade = x.no_escolaridade
                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AnoEscolar GetAnoEscolarById(int id)
        {
            var retorno = (from anesc in db.AnoEscolar.Include("Escolaridade")
                           where anesc.cd_ano_escolar == id
                           select new
                           {
                               cd_ano_escolar = anesc.cd_ano_escolar,
                               cd_escolaridade = anesc.cd_escolaridade,
                               Escolaridade = anesc.Escolaridade,
                               dc_ano_escolar = anesc.dc_ano_escolar,
                               nm_ordem = anesc.nm_ordem,
                               id_ativo = anesc.id_ativo
                           }).ToList().Select(x => new AnoEscolar
                           {
                               cd_ano_escolar = x.cd_ano_escolar,
                               cd_escolaridade = x.cd_escolaridade,
                               no_escolaridade = x.Escolaridade.no_escolaridade,
                               dc_ano_escolar = x.dc_ano_escolar,
                               nm_ordem = x.nm_ordem,
                               id_ativo = x.id_ativo
                           }).FirstOrDefault();

            return retorno;
        }


        public int GetUltimoNmOrdem(int cd_escolaridade)
        {
            var nm_ordem = (from anesc in db.AnoEscolar.OrderByDescending(x => x.nm_ordem)
                            where anesc.cd_escolaridade == cd_escolaridade
                            select anesc.nm_ordem).FirstOrDefault();

            return nm_ordem;
        }

        public IEnumerable<AnoEscolar> getAnoEscolaresAtivos(int? cdAnoEscolar)
        {
            try
            {
                var sql = (from anoE in db.AnoEscolar
                           where anoE.id_ativo || (cdAnoEscolar.HasValue && anoE.cd_ano_escolar == cdAnoEscolar)
                           orderby anoE.nm_ordem ascending
                           select new { 
                               anoE.cd_ano_escolar,
                               anoE.dc_ano_escolar,
                               anoE.Escolaridade.no_escolaridade
                           }).ToList().Select(x=> new AnoEscolar{
                               cd_ano_escolar = x.cd_ano_escolar,
                               dc_ano_escolar = x.no_escolaridade + " - " + x.dc_ano_escolar
                           });

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
