using System;
using System.Collections.Generic;
using System.Linq;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using System.Data;
using Componentes.GenericModel;
using FundacaoFisk.SGF.Web.Services.CNAB.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.Utils.Messages;

namespace FundacaoFisk.SGF.Web.Services.CNAB.DataAccess
{
    public class CarteiraCnabDataAccess : GenericRepository<CarteiraCnab>, ICarteiraCnabDataAccess
    {


        public enum TipoConsultaCarteiraCnab
        {
            HAS_ATIVO = 0,
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public IEnumerable<CarteiraCnab> getCarteiraCnabSearch(SearchParameters parametros, string nome, bool inicio, int banco, bool? status)
        {
            try
            {
                IEntitySorter<CarteiraCnab> sorter = EntitySorter<CarteiraCnab>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<CarteiraCnab> sql;

                if (status == null)
                {
                    sql = from cc in db.CarteiraCnab.AsNoTracking()
                          select cc;
                }
                else
                {
                    sql = from cc in db.CarteiraCnab.AsNoTracking()
                          where cc.id_carteira_ativa == status
                          select cc;
                }

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_carteira.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_carteira.Contains(nome)
                              select c;
                if (banco > 0)
                    sql = from c in sql
                          where c.cd_banco == banco
                          select c;
                IEnumerable<CarteiraCnab> sql1 = (from c in sql
                                                  select new
                                                         {
                                                             c.cd_carteira_cnab,
                                                             c.cd_banco,
                                                             c.id_carteira_ativa,
                                                             c.id_homologado,
                                                             c.id_impressao,
                                                             c.id_registrada,
                                                             c.nm_carteira,
                                                             c.nm_colunas,
                                                             c.no_carteira,
                                                             c.Banco.no_banco
                                                         }).ToList().Select(x => new CarteiraCnab
                                                           {
                                                               cd_carteira_cnab = x.cd_carteira_cnab,
                                                               cd_banco = x.cd_banco,
                                                               id_carteira_ativa = x.id_carteira_ativa,
                                                               id_homologado = x.id_homologado,
                                                               id_impressao = x.id_impressao,
                                                               id_registrada = x.id_registrada,
                                                               nm_carteira = x.nm_carteira,
                                                               nm_colunas = x.nm_colunas,
                                                               no_carteira = x.no_carteira,
                                                               Banco = new Banco
                                                               {
                                                                   no_banco = x.no_banco
                                                               }

                                                           });

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<CarteiraCnab> getCarteiraByBanco(int? localMovto, int banco)
        {
            try
            {
                //Pesquisa todas carteiras ativas, ou desse local de movimento que possui e banco escolhido
                IQueryable<CarteiraCnab> sql = from cc in db.CarteiraCnab
                                               where (cc.id_carteira_ativa == true || cc.LocaisMovimento.Where(l => l.cd_local_movto == localMovto).Any()) &&
                                               cc.id_homologado &&
                                               cc.Banco.cd_banco == banco
                                               select cc;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public CarteiraCnab getCarteiraByCarteira(int cd_carteira_cnab)
        {
            try
            {
                //Pesquisa todas carteiras ativas, ou desse local de movimento que possui e banco escolhido
                var sql = from cc in db.CarteiraCnab
                          where cc.cd_carteira_cnab == cd_carteira_cnab
                          select cc;
                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<CarteiraCnab> getCarteirasCnab(int cdEscola, int cd_carteira_cnab, TipoConsultaCarteiraCnab tipoConsulta)
        {
            try
            {
                IEnumerable<CarteiraCnab> sql = null;
                switch (tipoConsulta)
                {
                    case TipoConsultaCarteiraCnab.HAS_ATIVO:
                        sql = (from l in db.LocalMovto
                               join c in db.CarteiraCnab on l.cd_carteira_cnab equals c.cd_carteira_cnab
                               where l.cd_pessoa_empresa == cdEscola && (l.id_local_ativo) ||
                                         (cd_carteira_cnab == 0 && l.cd_carteira_cnab == cd_carteira_cnab)
                                   select new
                                   {
                                       cd_carteira_cnab = c.cd_carteira_cnab,
                                       no_carteira = c.no_carteira,
                                       cd_local_movto = l.cd_local_movto,
                                       no_local_movto = l.no_local_movto,
                                       nm_agencia = l.nm_agencia,
                                       nm_conta_corrente = l.nm_conta_corrente,
                                       nm_digito_conta_corrente = l.nm_digito_conta_corrente != null ? l.nm_digito_conta_corrente : null,
                                       nm_tipo_local = l.nm_tipo_local,
                                       nm_banco = l.nm_banco
                                   }).ToList().Select(x => new CarteiraCnab
                                   {
                                       cd_carteira_cnab = x.cd_carteira_cnab,
                                       no_carteira = x.no_carteira,
                                       cd_localMvto = x.cd_local_movto,
                                       localMovtoCateiraCnab = new LocalMovto
                                       {
                                           cd_local_movto = x.cd_local_movto,
                                           no_local_movto = x.no_local_movto,
                                           nm_agencia = x.nm_agencia,
                                           nm_conta_corrente = x.nm_conta_corrente,
                                           nm_digito_conta_corrente = x.nm_digito_conta_corrente,
                                           nm_banco = x.nm_banco
                                       }
                                   });
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
