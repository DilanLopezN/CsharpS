using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.DataAccess
{
    public class MensageAvaliacaoDataAccess : GenericRepository<MensagemAvaliacao>, IMensagemAvaliacaoDataAccess
    {
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<MensagemAvaliacaoSearchUI> getMensagemAvaliacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status, int? produto, int? curso)
        {
            try
            {
                IEntitySorter<MensagemAvaliacaoSearchUI> sorter = EntitySorter<MensagemAvaliacaoSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<MensagemAvaliacaoSearchUI> sql;

                sql = from mensagem in db.MensagemAvaliacao.AsNoTracking()
                      select new MensagemAvaliacaoSearchUI
                      {
                          cd_mensagem_avaliacao = mensagem.cd_mensagem_avaliacao,
                          tx_mensagem_avaliacao = mensagem.tx_mensagem_avaliacao,
                          id_mensagem_ativa = mensagem.id_mensagem_ativa,
                          cd_produto = mensagem.cd_produto,
                          no_produto = mensagem.Produto.no_produto,
                          cd_curso = mensagem.cd_curso,
                          no_curso = mensagem.Curso.no_curso,
                      };
                if (produto != null && produto > 0)
                {
                    sql = from mensagem in sql
                        where mensagem.cd_produto == produto
                        select mensagem;
                }

                if (curso != null && curso > 0)
                {
                    sql = from mensagem in sql
                        where mensagem.cd_curso == curso
                        select mensagem;
                }
                

                if (status == null)
                {
                    sql = from mensagem in sql
                          select mensagem;
                }
                else
                {
                    sql = from mensagem in sql
                          where mensagem.id_mensagem_ativa == status
                          select mensagem;
                }
                
                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        sql = from mensagem in sql
                                  where mensagem.tx_mensagem_avaliacao.StartsWith(desc)
                                  select mensagem;
                    }//end if
                    else
                    {
                        sql = from mensagem in sql
                                  where mensagem.tx_mensagem_avaliacao.Contains(desc)
                                  select mensagem;
                    }//end else

                }

                sql = sorter.Sort(sql);
                var retorno = (from mensagem in sql
                    select new {
                        cd_mensagem_avaliacao = mensagem.cd_mensagem_avaliacao,
                        tx_mensagem_avaliacao = mensagem.tx_mensagem_avaliacao,
                        id_mensagem_ativa = mensagem.id_mensagem_ativa,
                        cd_produto = mensagem.cd_produto,
                        no_produto = mensagem.no_produto,
                        cd_curso = mensagem.cd_curso,
                        no_curso = mensagem.no_curso,         
                    }).ToList().Select(x =>  new MensagemAvaliacaoSearchUI
                {
                    cd_mensagem_avaliacao = x.cd_mensagem_avaliacao,
                    tx_mensagem_avaliacao = x.tx_mensagem_avaliacao,
                    id_mensagem_ativa = x.id_mensagem_ativa,
                    cd_produto = x.cd_produto,
                    no_produto = x.no_produto,
                    cd_curso = x.cd_curso,
                    no_curso = x.no_curso,
                });

                int limite = retorno.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                retorno = retorno.Skip(parametros.from).Take(parametros.qtd_limite);
                
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<MensagemAvaliacaoSearchUI> getMensagensAvaliacaoByIds(List<int> cdsMensagens)
        {
            try
            {
                List<MensagemAvaliacaoSearchUI> listMensagem = (from mensagem in db.MensagemAvaliacao
                                                    where cdsMensagens.Contains(mensagem.cd_mensagem_avaliacao) &&
                                                          mensagem.id_mensagem_ativa
                                                    orderby mensagem.Produto.no_produto
                                                     select new MensagemAvaliacaoSearchUI
                                                    {
                                                        cd_mensagem_avaliacao = mensagem.cd_mensagem_avaliacao,
                                                        tx_mensagem_avaliacao = mensagem.tx_mensagem_avaliacao,
                                                        id_mensagem_ativa = mensagem.id_mensagem_ativa,
                                                        cd_produto = mensagem.cd_produto,
                                                        no_produto = mensagem.Produto.no_produto,
                                                        cd_curso = mensagem.cd_curso,
                                                        no_curso = mensagem.Curso.no_curso,
                                                    }).ToList();
                return listMensagem;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<MensagemAvaliacaoSearchUI> getMensagensAvaliacaoById(int cd_mensagem_avaliacao)
        {
            try
            {
                List<MensagemAvaliacaoSearchUI> listMensagem = (from mensagem in db.MensagemAvaliacao
                    where mensagem.cd_mensagem_avaliacao == cd_mensagem_avaliacao 
                    orderby mensagem.Produto.no_produto
                    select new MensagemAvaliacaoSearchUI
                    {
                        cd_mensagem_avaliacao = mensagem.cd_mensagem_avaliacao,
                        tx_mensagem_avaliacao = mensagem.tx_mensagem_avaliacao,
                        id_mensagem_ativa = mensagem.id_mensagem_ativa,
                        cd_produto = mensagem.cd_produto,
                        no_produto = mensagem.Produto.no_produto,
                        cd_curso = mensagem.cd_curso,
                        no_curso = mensagem.Curso.no_curso,
                    }).ToList();
                return listMensagem;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}