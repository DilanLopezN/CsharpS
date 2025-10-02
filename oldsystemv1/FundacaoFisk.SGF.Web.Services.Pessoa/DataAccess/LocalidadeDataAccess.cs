using System;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using Componentes.GenericDataAccess.GenericException;

    public class LocalidadeDataAccess : GenericRepository<LocalidadeSGF>, ILocalidadeDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        //retorna o primeiro registro ou o default
        public LocalidadeSGF firstOrDefault(byte tipo)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where
                             c.cd_tipo_localidade == tipo
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
       
        #region Endereço
        public IEnumerable<LocalidadeSGF> GetAllEndereco()
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<LocalidadeSGF> GetAllEndereco(string searchText)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO && c.no_localidade.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public LocalidadeSGF GetEnderecoDesc(string descricao)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO
                           && c.no_localidade.Equals(descricao)
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public LocalidadeSGF GetLocalidade(int cd)
        {
            try{
                LocalidadeSGF loc = new LocalidadeSGF();
                //for(int i = 0; i < cd.Count; i ++)
                loc = (from c in db.LocalidadeSGF
                            where c.cd_localidade == cd
                            select c).FirstOrDefault();
                return loc;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool deleteAll(List<LocalidadeSGF> localidades)
        {
            try{
                string strPaises = "";
                if (localidades != null && localidades.Count > 0)
                    foreach (LocalidadeSGF e in localidades)
                        strPaises += e.cd_localidade + ",";

                // Remove o último ponto e virgula:
                if (strPaises.Length > 0)
                    strPaises = strPaises.Substring(0, strPaises.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_localidade where cd_localidade in(" + strPaises + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        #endregion
       
        #region Bairro

        public IEnumerable<LocalidadeSGF> GetAllBairro()
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO
                          orderby c.no_localidade
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> GetBairroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade)
        {
            try{
                IEnumerable<LocalidadeSGF> retorno;
                IEntitySorter<LocalidadeSGF> sorter = EntitySorter<LocalidadeSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                var sql = from c in db.LocalidadeSGF.AsNoTracking()
                           where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO &&
                            (cd_cidade == 0 || c.cd_loc_relacionada == cd_cidade)
                           select c;
                 if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                                  where c.no_localidade.StartsWith(descricao)
                                  select c;
                    else
                        sql = from c in sql
                                  where c.no_localidade.Contains(descricao)
                                  select c;

                sql = sorter.Sort(sql.AsQueryable());
               
                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                parametros.qtd_limite = limite;


                retorno = (from c in sql
                           select new
                            {
                                cd_localidade = c.cd_localidade,
                                cd_tipo_localidade = c.cd_tipo_localidade,
                                cd_loc_relacionada = c.cd_loc_relacionada,
                                no_localidade = c.no_localidade,
                                no_localidade_cidade = c.LocalidadeRelacionada.no_localidade
                            }).ToList().Select(x => new LocalidadeSGF
                             {
                                 cd_localidade = x.cd_localidade,
                                 no_localidade = x.no_localidade,
                                 cd_tipo_localidade = x.cd_tipo_localidade,
                                 cd_loc_relacionada = x.cd_loc_relacionada,
                                 cd_localidade_cidade = x.cd_loc_relacionada,
                                 no_localidade_cidade = x.no_localidade_cidade
                             });


                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        /// <summary>
        /// Retorna um bairro apartir de uma descrição
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public LocalidadeSGF GetBairroDesc(string descricao)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO
                           && c.no_localidade.Equals(descricao)
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<LocalidadeSGF> FindBairro(string searchText)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO && c.no_localidade.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalidadeSGF GetBairroById(int idBairro)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where
                              c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO &&
                              c.cd_localidade == idBairro
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> getBairroPorCidade(int cd_cidade, int cd_bairro)
        {
            try
            {
                var sql = (from b in db.LocalidadeSGF
                           where (cd_bairro > 0 && b.cd_localidade == cd_bairro) ||b.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO && b.cd_loc_relacionada == cd_cidade
                           orderby b.no_localidade
                           select new
                           {
                               cd_localidade = b.cd_localidade,
                               no_localidade = b.no_localidade
                           }).ToList().Select(x => new LocalidadeSGF { 
                               cd_localidade = x.cd_localidade,
                               no_localidade = x.no_localidade
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalidadeSGF getBairroByNome(string no_loc_bairro, int cd_cidade, int cd_estado)
        {
            try
            {
                var sql = (from c in db.LocalidadeSGF
                           where c.no_localidade == no_loc_bairro && c.cd_loc_relacionada == cd_cidade
                           && c.LocalidadeRelacionada.cd_loc_relacionada == cd_estado
                           && c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.BAIRRO
                           orderby c.no_localidade
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

        #endregion

        #region Cidade

        public IEnumerable<LocalidadeSGF> GetAllCidade()
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE
                          orderby c.no_localidade
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CidadeUI> GetCidadeSearch(SearchParameters parametros, string descricao, bool inicio, int nmMunicipio, int cdEstado)
        {
            try{
                IEntitySorter<CidadeUI> sorter = EntitySorter<CidadeUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CidadeUI> sql;

                sql = from c in db.LocalidadeSGF.AsNoTracking()
                      where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE
                      select new CidadeUI { 
                          cd_localidade = c.cd_localidade,
                          cd_tipo_localidade = c.cd_tipo_localidade,
                          no_localidade = c.no_localidade,
                          cd_loc_relacionada = c.cd_loc_relacionada,
                          nm_municipio =  c.nm_municipio,
                          sg_estado = db.EstadoSGF.Where(e => e.cd_localidade_estado == c.cd_loc_relacionada).Select(e => e.sg_estado).FirstOrDefault()
                      };

                sql = sorter.Sort(sql);
                var retorno = from c in sql
                              select c;
                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_localidade.StartsWith(descricao)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_localidade.Contains(descricao)
                                  select c;

                if (nmMunicipio > 0)
                    retorno = from c in retorno
                              where c.nm_municipio == nmMunicipio
                              select c;

                if (cdEstado > 0)
                    retorno = from c in retorno
                              where c.cd_loc_relacionada == cdEstado
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

        public IEnumerable<LocalidadeSGF> FindCidade(string searchText)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE && c.no_localidade.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public LocalidadeSGF GetCidadeById(int idCidade)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where
                              c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE &&
                              c.cd_localidade == idCidade
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> GetAllCidade(int idEstado)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_loc_relacionada == idEstado
                          && c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE
                          orderby c.no_localidade
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // Retorna um tipo de Localidade(cidade, distrito)
        public LocalidadeSGF findFistOrDefaultLocalidade(byte tipoLoc, int? cdLocRel)
        {
            try{
                var sql = (from localidade in db.LocalidadeSGF
                           where (localidade.cd_tipo_localidade == tipoLoc)
                          && (localidade.cd_loc_relacionada == cdLocRel || localidade.cd_loc_relacionada == null)
                           select localidade).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        //Retona um estado que tem uma cidade relaciona

        public LocalidadeSGF findFirtsEstadoWithCidade()
        {
            try{
                var sql = (from localidade in db.LocalidadeSGF
                           where (localidade.FilhasLocalidade.Where(cidade => cidade.cd_loc_relacionada == localidade.cd_localidade && localidade.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.ESTADO).Any())
                           select localidade).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        /// <summary>
        ///  retorna lista de cidades
        /// </summary>
        /// <param name="pais"></param>
        /// <param name="estado"></param>
        /// <param name="numMunicipio"></param>
        /// <param name="desCidade"></param>
        /// <returns></returns>
        public IEnumerable<LocalidadeUI> GetCidadePaisEstado(SearchParameters parametros, int pais, int estado, int numMunicipio, string desCidade)
        {
            try{
                IEntitySorter<LocalidadeUI> sorter = EntitySorter<LocalidadeUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<LocalidadeUI> sql;

                sql = from localidade in db.LocalidadeSGF.AsNoTracking()
                      join localidadeRelacionada in db.LocalidadeSGF on localidade.cd_loc_relacionada equals localidadeRelacionada.cd_localidade
                      join localidadeEstado in db.EstadoSGF on localidadeRelacionada.cd_localidade equals localidadeEstado.cd_localidade_estado
                      join localidadePais in db.PaisSGF on localidadeRelacionada.cd_loc_relacionada equals localidadePais.cd_localidade_pais
                      where  localidade.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE
                      && (pais == 0 || localidadePais.cd_localidade_pais == pais)
                      && (estado == 0 || localidadeEstado.cd_localidade_estado == estado)
                      && (numMunicipio == 0 || localidade.nm_municipio == numMunicipio)
                      && (localidade.no_localidade.Contains(desCidade))
                      select new LocalidadeUI
                      {
                          cd_cidade = localidade.cd_localidade,
                          cd_estado = localidadeEstado.cd_localidade_estado,
                          cd_pais = localidadePais.cd_localidade_pais,
                          no_cidade = localidade.no_localidade,
                          nm_cidade = localidade.nm_municipio,
                          pais = localidadePais.dc_pais,
                          estado = localidadeEstado.sg_estado
                      };

                sql = sorter.Sort(sql);
                var retorno = from localidade in sql
                              select localidade;

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
        public IEnumerable<LocalidadeSGF> GetAllCidade(string search, int idEstado)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.no_localidade.Contains(search) && c.cd_loc_relacionada == idEstado
                          && c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE
                          orderby c.no_localidade
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> getAllLogradouroPorBairro(int cd_bairro)
        {
            try
            {
                var sql = from c in db.LocalidadeSGF
                          where c.cd_loc_relacionada == cd_bairro
                          && c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO
                          orderby c.no_localidade
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalidadeSGF getCidadeByNomePorEstado(string no_loc_cidade, int cd_estado)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where c.no_localidade == no_loc_cidade && c.cd_loc_relacionada == cd_estado
                           && c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.CIDADE
                           select new {
                               cd_localidade = c.cd_localidade,
                               no_localidade = c.no_localidade
                           }).ToList().Select(x => new LocalidadeSGF{
                               cd_localidade= x.cd_localidade,
                               no_localidade = x.no_localidade
                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        #endregion

        #region Distrito

        public IEnumerable<LocalidadeSGF> GetAllDistrito()
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.DISTRITO
                          orderby c.no_localidade
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> GetDistritoSearch(SearchParameters parametros, string descricao, bool inicio, int cd_cidade)
        {
            try{
                IEntitySorter<LocalidadeSGF> sorter = EntitySorter<LocalidadeSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IEnumerable<LocalidadeSGF> retorno;
                var sql = from t in db.LocalidadeSGF.AsNoTracking()
                      where t.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.DISTRITO &&
                            (cd_cidade == 0 || t.cd_loc_relacionada == cd_cidade)
                             select t;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                                  where c.no_localidade.StartsWith(descricao)
                                  select c;
                    else
                        sql = from c in sql
                                  where c.no_localidade.Contains(descricao)
                                  select c;

                sql = sorter.Sort(sql.AsQueryable());
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                retorno = (from t in sql
                           select new
                              {
                                  cd_localidade = t.cd_localidade,
                                  cd_tipo_localidade = t.cd_tipo_localidade,
                                  cd_loc_relacionada = t.cd_loc_relacionada,
                                  no_localidade = t.no_localidade,
                                  no_localidade_cidade = t.LocalidadeRelacionada.no_localidade
                              }).ToList().Select(x => new LocalidadeSGF
                              {
                                  cd_localidade = x.cd_localidade,
                                  no_localidade = x.no_localidade,
                                  cd_tipo_localidade = x.cd_tipo_localidade,
                                  cd_loc_relacionada = x.cd_loc_relacionada,
                                  cd_localidade_cidade = x.cd_loc_relacionada,
                                  no_localidade_cidade = x.no_localidade_cidade
                              });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> FindDistrito(string searchText)
        {
            try{
                var sql = from c in db.LocalidadeSGF
                          where c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.DISTRITO && c.no_localidade.Contains(searchText)
                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public LocalidadeSGF GetDistritoById(int idDistrito)
        {
            try{
                var sql = (from c in db.LocalidadeSGF
                           where
                              c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.DISTRITO &&
                              c.cd_localidade == idDistrito
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        #endregion

        #region Logradouro
        // verifica se existe a descrição da rua conforme o código.
        public bool existsLocalidade(int cdLoc, String no_rua)
        {
            try{
                var sql = (from rua in db.LocalidadeSGF
                           where rua.cd_localidade == cdLoc && rua.no_localidade == no_rua
                           select rua);
                int? result = sql.ToArray().Length;
                return result == null || result <= 0 ? false : true;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> getLogradouroSearch(SearchParameters parametros, string descricao, bool inicio, int cd_estado, int cd_cidade, int cd_bairro, string cep)
        {
            try
            {
                IEntitySorter<LocalidadeSGF> sorter = EntitySorter<LocalidadeSGF>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<LocalidadeSGF> sql;

                sql = from l in db.LocalidadeSGF.AsNoTracking()
                      where l.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO
                      select l;
                if (cd_estado > 0)
                    sql = from l in sql
                          where
                          l.LocalidadeRelacionada.LocalidadeRelacionada.cd_loc_relacionada == cd_estado
                          //l.LocalidadeRelacionada.cd_loc_relacionada == cd_cidade && l.LocalidadeRelacionada.cd_tipo_localidade == (int)TipoLocalidade.TipoLocalidadeEnum.CIDADE
                          select l;
                if (cd_cidade > 0)
                    sql = from l in sql
                          where
                          l.LocalidadeRelacionada.LocalidadeRelacionada.cd_localidade == cd_cidade
                          //l.LocalidadeRelacionada.cd_loc_relacionada == cd_cidade && l.LocalidadeRelacionada.cd_tipo_localidade == (int)TipoLocalidade.TipoLocalidadeEnum.CIDADE
                          select l;
                if (cd_bairro > 0)
                    sql = from l in sql
                          where l.cd_loc_relacionada == cd_bairro
                          select l;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sql = from c in sql
                                  where c.no_localidade.StartsWith(descricao)
                                  select c;
                    else
                        sql = from c in sql
                                  where c.no_localidade.Contains(descricao)
                                  select c;
                if (cep != null && cep != "")
                    sql = from c in sql
                          where c.dc_num_cep == cep
                          select c;
                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                parametros.qtd_limite = limite;
                var retorno = (from t in sql
                               select new
                               {
                                   cd_localidade = t.cd_localidade,
                                   cd_tipo_localidade = t.cd_tipo_localidade,
                                   cd_loc_relacionada = t.cd_loc_relacionada,
                                   no_localidade = t.no_localidade,
                                   //no_localidade_bairro = (from b in db.Localidade where b.cd_localidade == t.cd_loc_relacionada select b.no_localidade).FirstOrDefault(),
                                   no_localidade_bairro = t.LocalidadeRelacionada.no_localidade,
                                   cep = t.dc_num_cep,
                                   no_localidade_cidade = t.LocalidadeRelacionada.LocalidadeRelacionada.no_localidade,
                                   cd_localidade_cidade = t.LocalidadeRelacionada != null ? t.LocalidadeRelacionada.cd_loc_relacionada : 0,
                                   cd_localidade_estado = t.LocalidadeRelacionada.LocalidadeRelacionada != null ? t.LocalidadeRelacionada.LocalidadeRelacionada.cd_loc_relacionada : 0,
                                   no_localidade_estado = t.LocalidadeRelacionada.LocalidadeRelacionada.LocalidadeRelacionada.no_localidade
                               }).ToList().Select(x => new LocalidadeSGF
                               {
                                   cd_localidade = x.cd_localidade,
                                   no_localidade = x.no_localidade,
                                   cd_tipo_localidade = x.cd_tipo_localidade,
                                   cd_loc_relacionada = x.cd_loc_relacionada,
                                   no_localidade_bairro = x.no_localidade_bairro,
                                   dc_num_cep = x.cep != null ? x.cep : "",
                                   nm_cep = x.cep != null ? x.cep : "",
                                   cd_localidade_cidade = x.cd_localidade_cidade,
                                   no_localidade_cidade = x.no_localidade_cidade,
                                   no_localidade_estado = x.no_localidade_estado,
                                   cd_localidade_estado = x.cd_localidade_estado
                               });
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LogradouroCEP> getLogradouroCorreio(string descricao, string estado, string cidade, string bairro, string cep, int? numero)
        {
            try
            {
                var sql = db.SPConsultaCEP(cep, estado, cidade, descricao, bairro, numero);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<LocalidadeSGF> getLogradouros(int[] cdLogradouros)
        {
            try
            {
                var sql = from l in db.LocalidadeSGF
                      where l.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO && cdLogradouros.Contains(l.cd_localidade)
                      select l;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LocalidadeSGF getRuaByNome(string no_loc_rua, int cd_bairro, int cd_cidade, int cd_estado)
        {
            try
            {
                var sql = (from c in db.LocalidadeSGF
                           where c.no_localidade == no_loc_rua &&  c.cd_loc_relacionada == cd_bairro
                           && c.LocalidadeRelacionada.cd_loc_relacionada == cd_cidade
                           && c.LocalidadeRelacionada.LocalidadeRelacionada.cd_loc_relacionada == cd_estado
                           && c.cd_tipo_localidade == (int)TipoLocalidadeSGF.TipoLocalidadeSGFEnum.LOGRADOURO
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

        #endregion
      
    }
}

