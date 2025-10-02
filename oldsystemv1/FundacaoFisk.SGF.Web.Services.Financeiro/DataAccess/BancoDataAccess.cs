using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System.Data.SqlClient;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class BancoDataAccess : GenericRepository<Banco>, IBancoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Banco> getBancoSearch(SearchParameters parametros, string nome, string nmBanco, bool inicio )
        {
            try
            {
                IEntitySorter<Banco> sorter = EntitySorter<Banco>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Banco> sql;
                sql = from banco in db.Banco.AsNoTracking()
                      select  banco;

                sql = sorter.Sort(sql);

                var retorno = from c in sql
                              select c;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        retorno = from c in sql
                                  where c.no_banco.StartsWith(nome)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.no_banco.Contains(nome)
                                  select c;

                if (!String.IsNullOrEmpty(nmBanco))
                    if (inicio)
                        retorno = from c in sql
                                  where c.nm_banco.StartsWith(nmBanco)
                                  select c;
                    else
                        retorno = from c in sql
                                  where c.nm_banco.Contains(nmBanco)
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
        public IEnumerable<Banco> getBancoCarteira()
        {
            try
            {
                IQueryable<Banco> sql;
                sql = from banco in db.Banco
                      where banco.CarteirasCnab.Any()
                      select banco;


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Banco> getBancosTituloCheque(int cd_empresa)
        {
            try
            {
                IQueryable<Banco> sql;
                sql = from banco in db.Banco
                      where banco.Cheques.Any(x => x.Contrato.cd_pessoa_escola == cd_empresa  || x.Movimento.cd_pessoa_empresa == cd_empresa)
                      select banco;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}