using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class BaixaAutomaticaDataAccess : GenericRepository<BaixaAutomatica>, IBaixaAutomaticaDataAccess
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCheque(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            try
            {
                IEntitySorter<BaixaAutomatica> sorter = EntitySorter<BaixaAutomatica>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<BaixaAutomatica> sql;

                 sql = (from b in db.BaixaAutomatica
                           select b);

                if (baixaAutomaticaChequeUI.dt_inicial != null)
                    sql = from b in sql
                          where b.dt_final >= baixaAutomaticaChequeUI.dt_inicial
                          select b;

                if (baixaAutomaticaChequeUI.dt_final != null)
                    sql = from b in sql
                          where b.dt_final <= baixaAutomaticaChequeUI.dt_final
                          select b;

                if (baixaAutomaticaChequeUI.cd_local_movto > 0 && baixaAutomaticaChequeUI.id_trocar_local == true)
                {
                    sql = from b in sql
                          where b.cd_local_movto == baixaAutomaticaChequeUI.cd_local_movto
                          select b;
                }

                if (baixaAutomaticaChequeUI.id_tipo > 0)
                {
                    sql = from b in sql
                          where b.id_tipo == baixaAutomaticaChequeUI.id_tipo
                          select b;
                }

               

                var retorno = (from b in sql
                       join l in db.LocalMovto on b.cd_local_movto equals l.cd_local_movto
                       join u in db.UsuarioWebSGF on b.cd_usuario equals u.cd_usuario
                       //join p in db.PessoaSGF on u.cd_pessoa equals p.cd_pessoa
                       where b.cd_escola == baixaAutomaticaChequeUI.cd_escola
                       select new
                       {
                           no_usuario = u.no_login,
                           no_local_movto = l.no_local_movto,
                           cd_baixa_automatica = b.cd_baixa_automatica,
                           cd_escola = b.cd_escola,
                           cd_local_movto = b.cd_local_movto,
                           cd_cartao_credito = b.cd_cartao_credito,
                           cd_usuario = b.cd_usuario,
                           dt_inicial = b.dt_inicial,
                           dt_final = b.dt_final,
                           dh_baixa_automatica = b.dh_baixa_automatica,
                           id_tipo = b.id_tipo,
                           id_trocar_local = b.id_trocar_local

                       }).Distinct().ToList().Select(x => new BaixaAutomatica
                       {
                           no_usuario = x.no_usuario,
                           no_local_movto = x.no_local_movto,
                           cd_baixa_automatica = x.cd_baixa_automatica,
                           cd_escola = x.cd_escola,
                           cd_local_movto = x.cd_local_movto,
                           cd_cartao_credito = x.cd_cartao_credito,
                           cd_usuario = x.cd_usuario,
                           dt_inicial = x.dt_inicial,
                           dt_final = x.dt_final,
                           dh_baixa_automatica = x.dh_baixa_automatica,
                           id_tipo = x.id_tipo,
                           id_trocar_local = x.id_trocar_local
                       }).AsQueryable();

                retorno = sorter.Sort(retorno);
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

        public IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCartao(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI)
        {
            try
            {
                IEntitySorter<BaixaAutomatica> sorter = EntitySorter<BaixaAutomatica>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<BaixaAutomatica> sql;

                sql = (from b in db.BaixaAutomatica
                       select b);

                if (baixaAutomaticaChequeUI.dt_inicial != null)
                    sql = from b in sql
                          where b.dt_final >= baixaAutomaticaChequeUI.dt_inicial
                          select b;

                if (baixaAutomaticaChequeUI.dt_final != null)
                    sql = from b in sql
                          where b.dt_final <= baixaAutomaticaChequeUI.dt_final
                          select b;



                if (baixaAutomaticaChequeUI.cd_local_movto > 0 && baixaAutomaticaChequeUI.id_trocar_local == false)
                {
                    sql = from b in sql
                          where b.cd_local_movto == baixaAutomaticaChequeUI.cd_local_movto
                          select b;
                }


                if (baixaAutomaticaChequeUI.id_tipo > 0)
                {
                    sql = from b in sql
                          where b.id_tipo == baixaAutomaticaChequeUI.id_tipo
                          select b;
                }


                var retorno = (from b in sql
                               join l in db.LocalMovto on b.cd_local_movto equals l.cd_local_movto
                               join u in db.UsuarioWebSGF on b.cd_usuario equals u.cd_usuario
                               //join p in db.PessoaSGF on u.cd_pessoa equals p.cd_pessoa
                               where b.cd_escola == baixaAutomaticaChequeUI.cd_escola
                               select new
                               {
                                   no_usuario = u.no_login,
                                   no_local_movto = l.no_local_movto,
                                   cd_baixa_automatica = b.cd_baixa_automatica,
                                   cd_escola = b.cd_escola,
                                   cd_local_movto = b.cd_local_movto,
                                   dc_cartao_movto = baixaAutomaticaChequeUI.cd_cartao_credito == null ? "" : db.LocalMovto.Where(k => k.cd_local_movto == baixaAutomaticaChequeUI.cd_cartao_credito && k.cd_pessoa_empresa == baixaAutomaticaChequeUI.cd_escola).FirstOrDefault() != null ?
                                       db.LocalMovto.Where(z => z.cd_local_movto == baixaAutomaticaChequeUI.cd_cartao_credito && z.cd_pessoa_empresa == baixaAutomaticaChequeUI.cd_escola).FirstOrDefault().no_local_movto : "",
                                   cd_cartao_credito = b.cd_cartao_credito,
                                   cd_usuario = b.cd_usuario,
                                   dt_inicial = b.dt_inicial,
                                   dt_final = b.dt_final,
                                   dh_baixa_automatica = b.dh_baixa_automatica,
                                   id_tipo = b.id_tipo,
                                   id_trocar_local = b.id_trocar_local

                               }).Distinct().ToList().Select(x => new BaixaAutomatica
                               {
                                   no_usuario = x.no_usuario,
                                   no_local_movto = x.no_local_movto,
                                   cd_baixa_automatica = x.cd_baixa_automatica,
                                   cd_escola = x.cd_escola,
                                   cd_local_movto = x.cd_local_movto,
                                   dc_cartao_movto = x.dc_cartao_movto,
                                   cd_cartao_credito = x.cd_cartao_credito,
                                   cd_usuario = x.cd_usuario,
                                   dt_inicial = x.dt_inicial,
                                   dt_final = x.dt_final,
                                   dh_baixa_automatica = x.dh_baixa_automatica,
                                   id_tipo = x.id_tipo,
                                   id_trocar_local = x.id_trocar_local
                               }).AsQueryable();

                retorno = sorter.Sort(retorno);
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
    }
}