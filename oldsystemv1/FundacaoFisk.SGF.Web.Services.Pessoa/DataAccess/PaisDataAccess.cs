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

    public class PaisDataAccess : GenericRepository<PaisSGF>, IPaisDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<PaisUI> GetAllPais()
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          join e in db.PaisSGF
                          on c.cd_localidade equals e.cd_localidade_pais
                          orderby e.dc_pais
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS
                          select new PaisUI
                          {   //cd_localidade_pais = e.cd_localidade_pais,
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              dc_num_pais = e.dc_num_pais,
                              dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                              dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                              sg_pais = e.sg_pais,
                              dc_pais = e.dc_pais
                          };
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PaisUI> GetPaisSearch(SearchParameters parametros, string descricao, bool inicio)
        {
           try{
                IEntitySorter<PaisUI> sorter = EntitySorter<PaisUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PaisUI> sql;

                sql = from c in db.LocalidadeSGF.AsNoTracking()
                      join e in db.PaisSGF
                      on c.cd_localidade equals e.cd_localidade_pais
                      orderby e.dc_pais
                      select new PaisUI{
                          cd_localidade = c.cd_localidade,
                          cd_tipo_localidade = c.cd_tipo_localidade,
                          dc_num_pais= e.dc_num_pais,
                          dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                          dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                          sg_pais = e.sg_pais,
                          dc_pais = e.dc_pais
                      };

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in db.LocalidadeSGF.AsNoTracking()
                                  join e in db.PaisSGF.AsNoTracking()
                                  on c.cd_localidade equals e.cd_localidade_pais
                                  where e.dc_pais.StartsWith(descricao)
                                  orderby e.dc_pais
                                  select new PaisUI
                                  {
                                      cd_localidade = c.cd_localidade,
                                      cd_tipo_localidade = c.cd_tipo_localidade,
                                      dc_num_pais = e.dc_num_pais,
                                      dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                                      dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                                      sg_pais = e.sg_pais,
                                      dc_pais = e.dc_pais
                                  };
                    else
                        retorno = from c in db.LocalidadeSGF.AsNoTracking()
                                  join e in db.PaisSGF.AsNoTracking()
                                  on c.cd_localidade equals e.cd_localidade_pais
                                  where e.dc_pais.Contains(descricao)
                                  orderby e.dc_pais
                                  select new PaisUI
                                  {
                                      cd_localidade = c.cd_localidade,
                                      cd_tipo_localidade = c.cd_tipo_localidade,
                                      dc_num_pais = e.dc_num_pais,
                                      dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                                      dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                                      sg_pais = e.sg_pais,
                                      dc_pais = e.dc_pais
                                  };

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
        public IEnumerable<PaisUI> FindPais(string searchText)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          join e in db.PaisSGF
                          on c.cd_localidade equals e.cd_localidade_pais
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS && c.no_localidade.Contains(searchText)
                          select new PaisUI
                          {
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              dc_num_pais = e.dc_num_pais,
                              dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                              dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                              sg_pais = e.sg_pais,
                              dc_pais = e.dc_pais
                          };
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PaisUI GetPaisById (int idPais)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           join e in db.PaisSGF
                          on c.cd_localidade equals e.cd_localidade_pais
                          where 
                             c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS && 
                             c.cd_localidade == idPais
                          select new PaisUI
                          {
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              dc_num_pais = e.dc_num_pais,
                              dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                              dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                              sg_pais = e.sg_pais,
                              dc_pais = e.dc_pais
                          }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public PaisUI firstOrDefault()
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           join e in db.PaisSGF
                           on c.cd_localidade equals e.cd_localidade_pais
                           where
                              c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS
                           orderby e.dc_num_pais descending
                           select new PaisUI
                           {
                               cd_localidade = c.cd_localidade,
                               cd_tipo_localidade = c.cd_tipo_localidade,
                               dc_num_pais = e.dc_num_pais,
                               dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                               dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                               sg_pais = e.sg_pais,
                               dc_pais = e.dc_pais
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<PaisUI> paises)
        {
            try{
                string strPaises = "";
                if (paises != null && paises.Count > 0)
                    foreach (PaisUI e in paises)
                        strPaises += e.cd_localidade + ",";

                // Remove o último ponto e virgula:
                if (strPaises.Length > 0)
                    strPaises = strPaises.Substring(0, strPaises.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_pais where cd_localidade_pais in(" + strPaises + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<PaisUI> getPaisEstado()
        {
            try
            {
                var sql = from c in db.LocalidadeSGF
                          join e in db.PaisSGF
                          on c.cd_localidade equals e.cd_localidade_pais
                          orderby e.dc_pais
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS &&
                            db.LocalidadeSGF.Where(l => l.cd_loc_relacionada == c.cd_localidade && l.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO).Any()
                          orderby e.dc_num_pais descending
                          select new PaisUI
                           {
                               cd_localidade = c.cd_localidade,
                               cd_tipo_localidade = c.cd_tipo_localidade,
                               dc_num_pais = e.dc_num_pais,
                               dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                               dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                               sg_pais = e.sg_pais,
                               dc_pais = e.dc_pais
                           };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PaisUI> GetAllPaisPorSexoPessoa()
        {
            try
            {
                var sql = from c in db.LocalidadeSGF
                          join e in db.PaisSGF
                          on c.cd_localidade equals e.cd_localidade_pais
                          orderby e.dc_pais
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.PAIS
                          && e.dc_nacionalidade_masc != e.dc_nacionalidade_fem
                          select new PaisUI
                          {   //cd_localidade_pais = e.cd_localidade_pais,
                              cd_localidade = c.cd_localidade,
                              cd_tipo_localidade = c.cd_tipo_localidade,
                              dc_num_pais = e.dc_num_pais,
                              dc_nacionalidade_masc = e.dc_nacionalidade_masc,
                              dc_nacionalidade_fem = e.dc_nacionalidade_fem,
                              sg_pais = e.sg_pais,
                              dc_pais = e.dc_pais
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
