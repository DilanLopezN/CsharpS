using System;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using Componentes.GenericDataAccess.GenericException;

    public class EstadoDataAccess : GenericRepository<EstadoSGF>, IEstadoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<EstadoUI> GetAllEstado()
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          join e in db.EstadoSGF
                          on c.cd_localidade equals e.cd_localidade_estado
                          orderby c.no_localidade
                          select new EstadoUI
                          {
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              no_localidade = c.no_localidade,
                              cd_loc_relacionada = c.cd_loc_relacionada,
                              sg_estado = e.sg_estado,
                              no_pais = db.LocalidadeSGF.Where(l => l.cd_localidade == c.cd_loc_relacionada).FirstOrDefault().no_localidade
                          };
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public List<int> GetEstadosLocalidades(List<EstadoUI> estados)
        {
            try{
                List<int> loc = new List<int>();
                for (int i = 0; i < estados.Count; i++)
                    loc[i] = db.LocalidadeSGF.Where(j => j.cd_localidade == estados[i].cd_localidade).Select(k => k.cd_localidade).FirstOrDefault();

                return loc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<EstadoUI> GetEstadoSearch(SearchParameters parametros, string descricao, bool inicio, int cdPais)
        {
            try{
                IEntitySorter<EstadoUI> sorter = EntitySorter<EstadoUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<EstadoUI> sql;

                sql = from c in db.LocalidadeSGF.AsNoTracking()
                      join e in db.EstadoSGF.AsNoTracking()
                      on c.cd_localidade equals e.cd_localidade_estado
                      where c.no_localidade.StartsWith(descricao)
                      orderby c.no_localidade
                      select new EstadoUI
                      {
                          cd_localidade = c.cd_localidade,
                          cd_tipo_localidade = c.cd_tipo_localidade,
                          no_localidade = c.no_localidade,
                          cd_loc_relacionada = c.cd_loc_relacionada,
                          sg_estado = e.sg_estado,
                          no_pais = (from pais in db.LocalidadeSGF where pais.cd_localidade == c.cd_loc_relacionada select pais.no_localidade).FirstOrDefault()
                      };
                
                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in db.LocalidadeSGF.AsNoTracking()
                                  join e in db.EstadoSGF.AsNoTracking()
                                  on c.cd_localidade equals e.cd_localidade_estado
                                  where c.no_localidade.StartsWith(descricao)
                                  orderby c.no_localidade
                                  select new EstadoUI
                                  {
                                      cd_localidade = c.cd_localidade,
                                      cd_tipo_localidade = c.cd_tipo_localidade,
                                      no_localidade = c.no_localidade,
                                      cd_loc_relacionada = c.cd_loc_relacionada,
                                      sg_estado = e.sg_estado,
                                      no_pais = (from pais in db.LocalidadeSGF where pais.cd_localidade == c.cd_loc_relacionada select pais.no_localidade).FirstOrDefault()
                                  };
                    else
                        retorno = from c in db.LocalidadeSGF.AsNoTracking()
                                  join e in db.EstadoSGF.AsNoTracking()
                                  on c.cd_localidade equals e.cd_localidade_estado
                                  where c.no_localidade.StartsWith(descricao)
                                  where c.no_localidade.Contains(descricao)
                                  orderby c.no_localidade
                                  select new EstadoUI
                                  {
                                      cd_localidade = c.cd_localidade,
                                      cd_tipo_localidade = c.cd_tipo_localidade,
                                      no_localidade = c.no_localidade,
                                      cd_loc_relacionada = c.cd_loc_relacionada,
                                      sg_estado = e.sg_estado,
                                      no_pais = (from pais in db.LocalidadeSGF where pais.cd_localidade == c.cd_loc_relacionada select pais.no_localidade).FirstOrDefault()
                                  };
                if (cdPais > 0)
                    retorno = from r in retorno
                              where r.cd_loc_relacionada == cdPais
                              select r;

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
        public IEnumerable<EstadoUI> FindEstado(string searchText)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          join e in db.EstadoSGF
                          on c.cd_localidade equals e.cd_localidade_estado
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO && c.no_localidade.Contains(searchText)
                          select new EstadoUI
                          {
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              no_localidade = c.no_localidade,
                              cd_loc_relacionada = c.cd_loc_relacionada,
                              sg_estado = e.sg_estado,
                              no_pais = (from pais in db.LocalidadeSGF where pais.cd_localidade == c.cd_loc_relacionada select pais.no_localidade).FirstOrDefault()
                          };
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EstadoUI GetEstadoById(int idEstado)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           join e in db.EstadoSGF
                           on c.cd_localidade equals e.cd_localidade_estado
                          where 
                             c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO &&
                             c.cd_localidade == idEstado
                          select new EstadoUI
                          {
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              no_localidade = c.no_localidade,
                              cd_loc_relacionada = c.cd_loc_relacionada,
                              sg_estado = e.sg_estado,
                              no_pais = (from pais in db.LocalidadeSGF where pais.cd_localidade == c.cd_loc_relacionada select pais.no_localidade).FirstOrDefault()
                          }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public EstadoUI firstOrDefault()
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           join e in db.EstadoSGF
                           on c.cd_localidade equals e.cd_localidade_estado
                           where
                             c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO
                           select new EstadoUI
                           {
                               cd_localidade = c.cd_localidade,
                               cd_tipo_localidade = c.cd_tipo_localidade,
                               no_localidade = c.no_localidade,
                               cd_loc_relacionada = c.cd_loc_relacionada,
                               sg_estado = e.sg_estado
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<EstadoUI> estados)
        {
            try{
                string strEstados = "";
                if (estados != null && estados.Count > 0)
                    foreach (EstadoUI e in estados)
                        strEstados += e.cd_localidade + ",";

                // Remove o último ponto e virgula:
                if (strEstados.Length > 0)
                    strEstados = strEstados.Substring(0, strEstados.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_estado where cd_localidade_estado in(" + strEstados + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EstadoUI> getEstadoEstado()
        {
            try
            {
                var sql = from c in db.LocalidadeSGF
                          join e in db.EstadoSGF
                            on c.cd_localidade equals e.cd_localidade_estado
                          orderby e.sg_estado
                          where
                            c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO &&
                            db.LocalidadeSGF.Where(l => l.cd_loc_relacionada == c.cd_localidade && l.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE).Any()
                          select new EstadoUI
                          {
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              no_localidade = c.no_localidade,
                              cd_loc_relacionada = c.cd_loc_relacionada,
                              sg_estado = e.sg_estado
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EstadoUI> getEstadoByPais(int cd_pais)
        {
            try
            {
                var sql = from c in db.LocalidadeSGF
                          join e in db.EstadoSGF
                            on c.cd_localidade equals e.cd_localidade_estado
                          orderby e.sg_estado
                          where
                            c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO &&
                            db.LocalidadeSGF.Where(l => l.cd_localidade == c.cd_loc_relacionada && l.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS && l.cd_localidade == cd_pais).Any()
                          select new EstadoUI
                          {
                              cd_localidade = c.cd_localidade,
                              no_localidade = c.no_localidade,
                              sg_estado = e.sg_estado
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalidadeSGF getEstadoBySigla(string no_sg_estado)
        {
            try
            {
                var sql = (from c in db.LocalidadeSGF
                          join e in db.EstadoSGF
                            on c.cd_localidade equals e.cd_localidade_estado
                          orderby e.sg_estado
                          where
                            c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO && e.sg_estado == no_sg_estado
                           select new
                           {
                               cd_localidade = c.cd_localidade,
                               no_localidade = c.no_localidade
                           }).ToList().Select(x => new LocalidadeSGF
                           {
                               cd_localidade = x.cd_localidade,
                               no_localidade = x.no_localidade
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
