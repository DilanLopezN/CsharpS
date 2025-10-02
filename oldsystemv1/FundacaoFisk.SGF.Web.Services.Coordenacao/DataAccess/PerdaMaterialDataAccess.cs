using System;
using System.Collections.Generic;
using Componentes.GenericDataAccess;
using System.Linq;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class PerdaMaterialDataAccess: GenericRepository<PerdaMaterial>, IPerdaMaterialDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }

        }

        public enum TipoPerdaMaterialStatus
        {
            ABERTO = 1,
            FECHADO = 2,
        }

        public IEnumerable<PerdaMaterialUI> getPerdaMaterialSearch(SearchParameters parametros, int? cd_aluno, int? nm_contrato, int? cd_movimento, int? cd_item, DateTime? dtInicio, DateTime? dtTermino, int status, int cd_escola)
        {
            try
            {
                IEntitySorter<PerdaMaterialUI> sorter = EntitySorter<PerdaMaterialUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PerdaMaterial> sql;

                sql = (from pm in db.PerdaMaterial
                       where pm.Movimento.cd_pessoa_empresa == cd_escola
                       select pm);

                if (cd_aluno > 0)
                {
                    sql = from pm in sql
                          where pm.Movimento.cd_aluno == cd_aluno
                          select pm;
                }

                if (nm_contrato > 0)
                {
                    sql = from pm in sql 
                          where  pm.Contrato.nm_contrato == nm_contrato
                              select pm;
                }

                if (cd_movimento > 0)
                {
                    sql = from pm in sql
                          where pm.cd_movimento == cd_movimento &&
                                pm.Movimento.id_material_didatico == true
                          select pm;
                }

                if (cd_item > 0)
                {
                    sql = from pm in sql
                          from im in db.ItemMovimento
                          where pm.cd_movimento == im.cd_movimento &&
                                im.cd_item == cd_item &&
                                pm.Movimento.id_material_didatico == true
                          select pm;
                }

                if (dtInicio.HasValue)
                {
                    sql = from pm in sql
                          where pm.dt_perda_material >= dtInicio
                          select pm;
                }

                if (dtTermino.HasValue)
                {
                    sql = from pm in sql
                          where pm.dt_perda_material <= dtTermino
                          select pm;
                }

                if (status >= 0)
                {
                    sql = from pm in sql
                          where pm.id_status_perda == (byte)status
                              select pm;
                }

                var retorno = (from pm in sql
                               select new PerdaMaterialUI
                               {
                                   cd_perda_material = pm.cd_perda_material,
                                   dt_perda_material = pm.dt_perda_material,
                                   cd_movimento = pm.cd_movimento,
                                   nm_movimento = pm.Movimento.nm_movimento ?? pm.Movimento.nm_movimento,
                                   dc_serie_movimento = pm.Movimento.dc_serie_movimento,
                                   cd_contrato = pm.cd_contrato,
                                   nm_contrato = pm.Contrato.nm_contrato,
                                   id_status_perda = pm.id_status_perda,
                                   no_aluno = (from al in db.Aluno
                                               from m in db.Movimento
                                               from p in db.PessoaSGF
                                               where pm.cd_movimento == m.cd_movimento &&
                                                     m.cd_aluno == al.cd_aluno &&
                                                     al.cd_pessoa_aluno == p.cd_pessoa
                                               select p.no_pessoa).FirstOrDefault(),
                                   itensPerdaMaterial = (from it in db.ItemMovimento
                                                         where it.cd_movimento == pm.cd_movimento 
                                                         select it).ToList()
                                   
                               }).AsEnumerable();

                retorno = sorter.Sort(retorno.AsQueryable());

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return retorno.ToList();

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe); 
            }
            
        }

        public PerdaMaterialUI getPerdaMaterialForGrid(int cd_perda_material)
        {
            try
            {
                PerdaMaterialUI sql = (from pm in db.PerdaMaterial
                                     where pm.cd_perda_material == cd_perda_material
                                     select new PerdaMaterialUI
                                     {
                                         cd_perda_material = pm.cd_perda_material,
                                         dt_perda_material = pm.dt_perda_material,
                                         cd_movimento = pm.cd_movimento,
                                         nm_movimento = pm.Movimento.nm_movimento ?? pm.Movimento.nm_movimento,
                                         dc_serie_movimento = pm.Movimento.dc_serie_movimento,
                                         cd_contrato = pm.cd_contrato,
                                         nm_contrato = pm.Contrato.nm_contrato,
                                         id_status_perda = pm.id_status_perda,
                                         no_aluno = (from al in db.Aluno
                                                     from m in db.Movimento
                                                     from p in db.PessoaSGF
                                                     where pm.cd_movimento == m.cd_movimento &&
                                                           m.cd_aluno == al.cd_aluno &&
                                                           al.cd_pessoa_aluno == p.cd_pessoa
                                                     select p.no_pessoa).FirstOrDefault(),
                                         itensPerdaMaterial = (from it in db.ItemMovimento
                                                               where it.cd_movimento == pm.cd_movimento
                                                               select it).ToList()

                                     }).FirstOrDefault();
               return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public int processarPerdaMaterial(PerdaMaterial perdaMaterial, int cdUsuario, int fuso)
        {
            try
            {
                return db.sp_processar_perda_material(perdaMaterial.cd_perda_material, cdUsuario, (byte)fuso);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}