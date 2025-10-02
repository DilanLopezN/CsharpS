using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class CargaProfessorDataAccess : GenericRepository<CargaProfessor>, ICargaProfessorDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<CargaProfessorSearchUI> getCargaProfessorSearch(SearchParameters parametros, int qtd_minutos_duracao, int cd_escola)
        {
            try
            {
                IEntitySorter<CargaProfessorSearchUI> sorter = EntitySorter<CargaProfessorSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CargaProfessorSearchUI> sql;

                sql = from cargaprofessor in db.CargaProfessor.AsNoTracking()
                      where (qtd_minutos_duracao == 0 || cargaprofessor.nm_carga_horaria == qtd_minutos_duracao) &&
                            (cd_escola == 0 || cargaprofessor.cd_escola == cd_escola)
                      select new CargaProfessorSearchUI
                      {
                          cd_carga_professor = cargaprofessor.cd_carga_professor,
                          cd_escola = cargaprofessor.cd_escola,
                          nm_carga_horaria =  cargaprofessor.nm_carga_horaria,
                          nm_carga_professor = cargaprofessor.nm_carga_professor,
                      };

                sql = sorter.Sort(sql);
                var retorno = from cargaprofessor in sql
                              select cargaprofessor;

                int limite = retorno.Select(x => x.cd_carga_professor).Count();

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

        public IEnumerable<CargaProfessor> getCargaProfessorByEscolaAllData(List<int> codigosCargaProfs, int cd_escola)
        {
            try
            {
                var sql = from c in db.CargaProfessor
                          where codigosCargaProfs.Contains(c.cd_carga_professor) && c.cd_escola == cd_escola
                          select c;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CargaProfessor> findCargaProfessorAll(int cd_escola)
        {
            try
            {
                var sql = from c in db.CargaProfessor
                          where c.cd_escola == cd_escola
                          select c;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public CargaProfessor findCargaProfessorById(int id, int cd_escola)
        {
            try
            {
                var sql = (from c in db.CargaProfessor
                           where c.cd_carga_professor == id && c.cd_escola == cd_escola
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
