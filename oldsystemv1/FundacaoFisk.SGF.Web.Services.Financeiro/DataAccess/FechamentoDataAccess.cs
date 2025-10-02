using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Utils;
using Componentes.Utils;
using Componentes.GenericDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class FechamentoDataAccess : GenericRepository<Fechamento>, IFechamentoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<Fechamento> getFechamentoSearch(SearchParameters parametros, int? ano, int? mes, bool balanco, DateTime? dta_ini, DateTime? dta_fim, int cd_escola)
        {
            try
            {
                IEntitySorter<Fechamento> sorter = EntitySorter<Fechamento>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Fechamento> sql;
                sql = from c in db.Fechamento.AsNoTracking()
                      where c.cd_pessoa_empresa == cd_escola
                            && c.dt_fechamento >= dta_ini
                            && c.dt_fechamento <= dta_fim
                      select c;

               
                if (ano > 0)
                    sql = from c in sql
                              where c.nm_ano_fechamento == (short)ano.Value
                              select c;

                if (mes > 0)
                    sql = from c in sql
                          where c.nm_mes_fechamento == (byte)mes.Value
                          select c;
                if (balanco)
                    sql = from c in sql
                          where c.id_balanco == true
                          select c;
                sql = sorter.Sort(sql);

                var retorno = (from l in sql
                               select new 
                               {
                                   cd_fechamento = l.cd_fechamento,
                                   nm_mes_fechamento = l.nm_mes_fechamento,
                                   nm_ano_fechamento = l.nm_ano_fechamento,
                                   id_balanco = l.id_balanco,
                                   dh_fechamento = l.dh_fechamento,
                                   cd_usuario = l.cd_usuario,
                                   no_usuario = l.Usuario.no_login,
                                   dt_fechamento = l.dt_fechamento
                               }).ToList().Select(x => new Fechamento
                           {
                               cd_fechamento = x.cd_fechamento,
                               nm_mes_fechamento = x.nm_mes_fechamento,
                               nm_ano_fechamento = x.nm_ano_fechamento,
                               id_balanco = x.id_balanco,
                               dh_fechamento = x.dh_fechamento,
                               cd_usuario = x.cd_usuario,
                               no_usuario = x.no_usuario,
                               dt_fechamento = x.dt_fechamento
                           });
                
                
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

        public Fechamento getFechamentoById(int cd_fechamento, int cd_escola)
        {
            try
            {
                Fechamento sql = (from f in db.Fechamento
                                  where f.cd_fechamento == cd_fechamento &&
                                        f.cd_pessoa_empresa == cd_escola
                                  select new
                                    {
                                        cd_fechamento = f.cd_fechamento,
                                        nm_mes_fechamento = f.nm_mes_fechamento,
                                        nm_ano_fechamento = f.nm_ano_fechamento,
                                        id_balanco = f.id_balanco,
                                        dh_fechamento = f.dh_fechamento,
                                        cd_usuario = f.cd_usuario,
                                        no_usuario = f.Usuario.no_login,
                                        tx_obs_fechamento = f.tx_obs_fechamento,
                                        dt_fechamento = f.dt_fechamento
                                    }).ToList().Select(x => new Fechamento
                                      {
                                          cd_fechamento = x.cd_fechamento,
                                          nm_mes_fechamento = x.nm_mes_fechamento,
                                          nm_ano_fechamento = x.nm_ano_fechamento,
                                          id_balanco = x.id_balanco,
                                          dh_fechamento = x.dh_fechamento,
                                          cd_usuario = x.cd_usuario,
                                          no_usuario = x.no_usuario,
                                          tx_obs_fechamento = x.tx_obs_fechamento,
                                          dt_fechamento = x.dt_fechamento
                                      }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public Fechamento getFechById(int cd_fechamento, int cd_escola)
        {
            try
            {
                Fechamento sql = (from f in db.Fechamento
                                  where f.cd_fechamento == cd_fechamento &&
                                        f.cd_pessoa_empresa == cd_escola
                                  select f).FirstOrDefault();


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existeFechamentoAnoMes(DateTime data, int cd_escola, int cd_fechamento)
        {
            try
            {
                int sql = (from f in db.Fechamento
                           where f.dt_fechamento == data && 
                                 //f.nm_mes_fechamento == mes &&
                                 f.cd_pessoa_empresa == cd_escola &&
                                 (f.cd_fechamento != cd_fechamento || cd_fechamento == 0)
                           select f.cd_fechamento).FirstOrDefault();

                return sql > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool existeFechamentoSuperior(DateTime data, int cd_escola, int cd_fechamento)
        {
            try
            {
                int sql = (from f in db.Fechamento
                           where f.cd_pessoa_empresa == cd_escola && f.id_balanco &&
                                 //(f.nm_ano_fechamento > ano || (f.nm_ano_fechamento == ano && f.nm_mes_fechamento > mes)) &&
                                 f.dt_fechamento > data &&
                                 (f.cd_fechamento != cd_fechamento || cd_fechamento == 0)
                           select f.cd_fechamento).FirstOrDefault();


                return sql > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<Fechamento> fechamentoAnoMes(int cd_escola)
        {
            try
            {
                var sql = (from fechamento in db.Fechamento
                           where fechamento.cd_pessoa_empresa == cd_escola
                           select new
                           {
                               cd_fechamento = fechamento.cd_fechamento,
                               nm_mes_fechamento = fechamento.nm_mes_fechamento,
                               nm_ano_fechamento = fechamento.nm_ano_fechamento,
                               dt_fechamento = fechamento.dt_fechamento
                           }).ToList().Select(x => new Fechamento
                          {
                              cd_fechamento = x.cd_fechamento,
                              nm_mes_fechamento = x.nm_mes_fechamento,
                              nm_ano_fechamento = x.nm_ano_fechamento,
                              dt_fechamento = x.dt_fechamento
                          });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Fechamento> getFechamentos(List<int> cdFechamentos, int cd_empresa)
        {
            try
            {
                var sql = from fechamento in db.Fechamento.Include(s => s.SaldosItens)
                          where fechamento.cd_pessoa_empresa == cd_empresa && cdFechamentos.Contains(fechamento.cd_fechamento)
                           select fechamento;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public Fechamento getFechamentoByDta(DateTime data, int cd_empresa)
        {
            try
            {
                int mes = data.Month;
                int ano = data.Year;
                //Ano lancado for menor que o ano do fechamento  ou anos orem iguais mas o mês for menor ou igual ao do fechamento
                var sql = (from fechamento in db.Fechamento
                           where fechamento.cd_pessoa_empresa == cd_empresa &&
                                fechamento.dt_fechamento >= data
                                //(fechamento.nm_ano_fechamento > ano || (fechamento.nm_ano_fechamento == ano && fechamento.nm_mes_fechamento >= mes) )
                          orderby fechamento.dt_fechamento descending
                          select fechamento).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Fechamento getUltimoFechamentoItem(DateTime dt_fechamento, int cd_item, int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from f in db.Fechamento
                           where f.cd_pessoa_empresa == cd_pessoa_empresa
                           && f.dt_fechamento < dt_fechamento
                           && f.SaldosItens.Any(s => s.cd_item == cd_item)
                           select new
                           {
                               f.dt_fechamento,
                               qtd_estoque = f.SaldosItens.Where(x => x.cd_item == cd_item).FirstOrDefault().qt_saldo_fechamento
                           }
                           ).ToList().Select(x => new Fechamento
                           {
                               dh_fechamento = x.dt_fechamento,
                               qtd_estoque = x.qtd_estoque
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
