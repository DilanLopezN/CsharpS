using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class CalendarioAcademicoDataAccess : GenericRepository<CalendarioAcademico>, ICalendarioAcademico
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<CalendarioAcademico> obterListaCalendarioAcademicos(int cd_escola)
        {
            try
            {
                var sql = from c in db.CalendarioAcademico
                          where c.cd_pessoa_escola == cd_escola
                          select c;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public CalendarioAcademico findCalendarioAcademicoById(int cd_calendario_academico, int cd_escola)
        {
            try
            {
                var sql = (from c in db.CalendarioAcademico
                           where c.cd_pessoa_escola == cd_escola && c.cd_calendario_academico == cd_calendario_academico
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public CalendarioAcademico findCalendarioAcademico(int cd_escola, byte tipo)
        {
            try
            {
                var sql = (from c in db.CalendarioAcademico
                    where c.cd_pessoa_escola == cd_escola && c.cd_tipo_calendario == tipo
                    select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CalendarioAcademico> obterCalendarioAcademicosPorFiltros(SearchParameters parametros, int cd_escola, int tipo_calendario, bool? status, bool relatorio)
        {
            IEntitySorter<CalendarioAcademico> sorter = EntitySorter<CalendarioAcademico>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<CalendarioAcademico> sql;

            sql = from evento in db.CalendarioAcademico
                  where evento.cd_pessoa_escola == cd_escola
                  select evento;

            if (tipo_calendario > 0)
            {
                sql = from evento in sql
                      where evento.cd_tipo_calendario == tipo_calendario
                      select evento;
            }

            if (status != null)
            {
                sql = from evento in sql
                      where evento.id_ativo == status
                      select evento;
            }

            sql = sorter.Sort(sql);
            var retorno = from evento in sql select evento;
            int limite = retorno.Count();

            parametros.ajustaParametrosPesquisa(limite);
            parametros.qtd_limite = limite;
            retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);

            if (relatorio && parametros.sort.Equals("cd_tipo_calendario")) 
            {
                switch ((int)parametros.sortOrder)
                {
                    case 0:
                        retorno = retorno.OrderByDescending(s => s.dc_desc_calendario);
                        break;   
                    case 1:
                        retorno = retorno.OrderBy(s => s.dc_desc_calendario);
                        break;
                }
            }
            
            return retorno;
        }

        public List<int> findEscolas()
        {
            try
            {
                var sql = (from pe in db.Parametro
                    select pe.cd_pessoa_escola);
                return sql.Distinct().ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
            //select distinct pe.cd_empresa from T_PESSOA_EMPRESA pe
            //inner join T_PESSOA p on pe.cd_pessoa = p.cd_pessoa
            //inner join t_empresa e on p.cd_pessoa = e.cd_pessoa_empresa
        }
    }
}
