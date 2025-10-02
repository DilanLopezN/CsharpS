using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using log4net;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class FeriadoDataAccess : GenericRepository<Feriado>, IFeriadoDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(FeriadoDataAccess));
        

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<Feriado> getFeriadoDesc(SearchParameters parametros, string desc, bool inicio, bool? ativo, int cdEscola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, bool? somenteAno, bool idFeriadoAtivo)
        {
            try{
                IEntitySorter<Feriado> sorter = EntitySorter<Feriado>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<Feriado> sql;

                sql = from feriado in db.Feriado.AsNoTracking()
                      where (feriado.cd_pessoa_escola == cdEscola || feriado.cd_pessoa_escola == null)
                      && feriado.id_feriado_ativo == idFeriadoAtivo
                      select feriado;

                if (somenteAno == true)
                    sql = from feriado in sql
                          where feriado.aa_feriado != null && feriado.aa_feriado_fim != null
                          select feriado;
                else
                    if (somenteAno == false)
                        sql = from feriado in sql
                              where feriado.aa_feriado == null && feriado.aa_feriado_fim == null
                              select feriado;

                if (AnoFim > 0)
                    if (Ano > 0)
                        sql = from s in sql
                              where (s.aa_feriado == null || s.aa_feriado >= Ano) && (s.aa_feriado_fim == null || s.aa_feriado_fim <= AnoFim)
                              select s;
                    else
                        sql = from s in sql
                              where (s.aa_feriado_fim == null || s.aa_feriado_fim <= AnoFim)
                              select s;
                else
                    sql = from s in sql
                          where (Ano == 0 || s.aa_feriado >= Ano)
                          select s;

                if (MesFim > 0)
                    if (Mes > 0)
                        sql = from s in sql
                              where (Mes == 0 || s.mm_feriado >= Mes) && (MesFim == 0 || s.mm_feriado_fim <= MesFim)
                              select s;
                    else
                        sql = from s in sql
                              where (MesFim == 0 || s.mm_feriado_fim <= MesFim)
                              select s;
                else
                    sql = from s in sql
                          where (Mes == 0 || s.mm_feriado >= Mes)
                          select s;
                if (DiaFim > 0)
                    if (Dia > 0)
                        sql = from s in sql
                              where (Dia == 0 || s.dd_feriado >= Dia) && (DiaFim == 0 || s.dd_feriado_fim <= DiaFim)
                              select s;
                    else
                        sql = from s in sql
                              where (DiaFim == 0 || s.dd_feriado_fim <= DiaFim)
                              select s;
                else
                    sql = from s in sql
                          where (Dia == 0 || s.dd_feriado >= Dia)
                          select s;

                if (ativo != null)
                    sql = from feriado in sql
                          where feriado.id_feriado_financeiro == ativo
                          select feriado;
                sql = sorter.Sort(sql);

                var retorno = from feriado in sql
                              select feriado;

                if (!String.IsNullOrEmpty(desc))
                {
                    if (inicio)
                    {
                        retorno = from feriado in sql
                                  where feriado.dc_feriado.StartsWith(desc)
                                  select feriado;
                    }//end if
                    else
                    {
                        retorno = from evento in sql
                                  where evento.dc_feriado.Contains(desc)
                                  select evento;
                    }// end if    
                }

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

        public bool deleteAllFeriado(List<Feriado> feriados)
        {
            try
            {
                //foreach (Feriado feriado in feriados)
                //{
                //    Feriado feriadoContext = this.findById(feriado.cod_feriado, false);
                //    this.deleteContext(feriadoContext, false);
                //}
                //return this.saveChanges(false) > 0;
                string strFeridado = "";
                if (feriados != null && feriados.Count > 0)
                    foreach (Feriado e in feriados)
                        strFeridado += e.cod_feriado + ",";

                // Remove o último ponto e virgula:
                if (strFeridado.Length > 0)
                    strFeridado = strFeridado.Substring(0, strFeridado.Length - 1);

                int retorno = db.Database.ExecuteSqlCommand("delete from t_feriado where cod_feriado in(" + strFeridado + ")");

                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Feriado> getFeriadosEscola(int cd_escola, bool feriado_financeiro)
        {
            try
            {
                var sql = from feriado in db.Feriado
                          where feriado.id_feriado_ativo == true
                          select feriado;

                if (feriado_financeiro)
                    sql = from feriado in sql
                          where feriado.id_feriado_financeiro
                          select feriado;

                IEnumerable<Feriado> sql1 = (from feriado in sql
                                             where (feriado.cd_pessoa_escola == null || feriado.cd_pessoa_escola == cd_escola)
                                             select new
                                             {
                                                 aa_feriado = feriado.aa_feriado,
                                                 aa_feriado_fim = feriado.aa_feriado_fim,
                                                 dd_feriado = feriado.dd_feriado,
                                                 dd_feriado_fim = feriado.dd_feriado_fim,
                                                 mm_feriado = feriado.mm_feriado,
                                                 mm_feriado_fim = feriado.mm_feriado_fim,
                                                 dc_feriado = feriado.dc_feriado,
                                                 cod_feriado = feriado.cod_feriado
                                             }).ToList().Select(x => new Feriado
                                             {
                                                 aa_feriado = x.aa_feriado,
                                                 aa_feriado_fim = x.aa_feriado_fim,
                                                 dd_feriado = x.dd_feriado,
                                                 dd_feriado_fim = x.dd_feriado_fim,
                                                 mm_feriado = x.mm_feriado,
                                                 mm_feriado_fim = x.mm_feriado_fim,
                                                 dc_feriado = x.dc_feriado,
                                                 cod_feriado = x.cod_feriado
                                             });
                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<Feriado> getFeriadosPorPeriodo(Feriado feriado, int? cd_escola)
        {
            try
            {
                List<Feriado> result = new List<Feriado>();
                // Verifica se o intervalo está dentro do periodo quando tem data
                if (feriado.aa_feriado.HasValue && feriado.aa_feriado_fim.HasValue)
                {
                    DateTime dataInical = new DateTime((int)feriado.aa_feriado, feriado.mm_feriado, feriado.dd_feriado);
                    DateTime dataFim = new DateTime((int)feriado.aa_feriado_fim, (int)feriado.mm_feriado_fim, (int)feriado.dd_feriado_fim);

                    List<Feriado> sql = (from f in db.Feriado
                                         where
                                              (f.cd_pessoa_escola == null || f.cd_pessoa_escola == cd_escola) &&
                                              ((!f.aa_feriado.HasValue && !f.aa_feriado_fim.HasValue) ||
                                              (f.aa_feriado >= feriado.aa_feriado &&
                                              f.aa_feriado_fim <= feriado.aa_feriado_fim)) &&
                                              f.id_feriado_ativo == true
                                         select f).ToList();

                    Feriado datasFeriado = new Feriado();
                    List<Feriado> listaDatasFeriados = new List<Feriado>();
                    foreach (Feriado fer in sql)
                        if (fer.aa_feriado.HasValue && fer.aa_feriado_fim.HasValue)
                        {
                            datasFeriado = new Feriado
                            {
                                dt_inicio = new DateTime((int)fer.aa_feriado, fer.mm_feriado, fer.dd_feriado),
                                dt_fim = new DateTime((int)fer.aa_feriado_fim, (int)fer.mm_feriado_fim, (int)fer.dd_feriado_fim),
                                cod_feriado = fer.cod_feriado
                            };
                            listaDatasFeriados.Add(datasFeriado);
                        }
                        else
                        {
                            //Colocando ano nos feriados fixos (onde o ano é nulo)
                            // Usando como base o ano de inicio do que está cadastrando para o ano inicial e final do registro existente, 
                            // pois feriado fixo não pode passar de um ano para o outro

                            //Ano 
                            datasFeriado = new Feriado
                            {
                                dt_inicio = new DateTime((int)feriado.aa_feriado, fer.mm_feriado, fer.dd_feriado),
                                dt_fim = new DateTime((int)feriado.aa_feriado, (int)fer.mm_feriado_fim, (int)fer.dd_feriado_fim),
                                cod_feriado = fer.cod_feriado
                            };
                            listaDatasFeriados.Add(datasFeriado);
                            
                            //Ano anterior 
                            datasFeriado = new Feriado
                            {
                                dt_inicio = new DateTime((int)feriado.aa_feriado - 1, fer.mm_feriado, fer.dd_feriado),
                                dt_fim = new DateTime((int)feriado.aa_feriado - 1, (int)fer.mm_feriado_fim, (int)fer.dd_feriado_fim),
                                cod_feriado = fer.cod_feriado
                            };
                            listaDatasFeriados.Add(datasFeriado);

                            //v posterior
                            datasFeriado = new Feriado
                            {
                                dt_inicio = new DateTime((int)feriado.aa_feriado + 1, fer.mm_feriado, fer.dd_feriado),
                                dt_fim = new DateTime((int)feriado.aa_feriado + 1, (int)fer.mm_feriado_fim, (int)fer.dd_feriado_fim),
                                cod_feriado = fer.cod_feriado
                            };
                            listaDatasFeriados.Add(datasFeriado);
                        }



                    result = (from s in listaDatasFeriados
                                 where
                                     //Data de inicio cadastrada menor que a data inicio existente e Data de Fim cadastrada maior que a data de inicio existente 
                                  (dataInical <= s.dt_inicio && dataFim >= s.dt_inicio) ||
                                     //Data de inicio cadastrada maior que a data inicio existente e Data de fim cadastrada menor que a data de fim existente 
                                  (dataInical >= s.dt_inicio && dataFim <= s.dt_fim) ||
                                     //Data de inicio cadastrada maior que a data inicio existente e Data de inicio cadastrada menor que a data de fim existente 
                                  (dataInical >= s.dt_inicio && dataInical <= s.dt_fim) ||
                                     //Data de inicio cadastrada menor que a data inicio existente e Data de fim cadastrada maior que a data de fim existente 
                                  (dataInical >= s.dt_inicio && dataInical <= s.dt_fim)

                                 select s).ToList();

                }
                else
                {
                    DateTime dataInical1 = new DateTime();
                    DateTime dataFim1 = new DateTime();

                    DateTime? dataInical2 = null;
                    DateTime? dataFim2 = null;

                    DateTime? dataInical3 = null;
                    DateTime? dataFim3 = null;

                    List<Feriado> sql = (from f in db.Feriado
                                         where (f.cd_pessoa_escola == null || f.cd_pessoa_escola == cd_escola) &&
                                            f.id_feriado_ativo == true
                                         select f).ToList();

                    Feriado datasFeriado = new Feriado();
                    List<Feriado> listaDatasFeriados = new List<Feriado>();
                    foreach (Feriado fer in sql)
                        if (fer.aa_feriado.HasValue && fer.aa_feriado_fim.HasValue)
                        {
                            datasFeriado = new Feriado
                            {
                                dt_inicio = new DateTime((int)fer.aa_feriado, fer.mm_feriado, fer.dd_feriado),
                                dt_fim = new DateTime((int)fer.aa_feriado_fim, (int)fer.mm_feriado_fim, (int)fer.dd_feriado_fim),
                                cod_feriado = fer.cod_feriado
                            };
                            listaDatasFeriados.Add(datasFeriado);
                            //Colocando ano no feriado cadastrado (onde o ano é nulo)
                            // Usando como base o ano de inicio do que exite no banco para o ano inicial e final do registro existente, 
                            // pois feriado fixo não pode passar de um ano para o outro
                            //Ano
                            dataInical1 = new DateTime((int)fer.aa_feriado, feriado.mm_feriado, feriado.dd_feriado);
                            dataFim1 = new DateTime((int)fer.aa_feriado, (int)feriado.mm_feriado_fim, (int)feriado.dd_feriado_fim);
                            //Ano anterior 
                            dataInical2 = new DateTime((int)fer.aa_feriado - 1, feriado.mm_feriado, feriado.dd_feriado);
                            dataFim2 = new DateTime((int)fer.aa_feriado - 1, (int)feriado.mm_feriado_fim, (int)feriado.dd_feriado_fim);
                            //Ano posterior 
                            dataInical3 = new DateTime((int)fer.aa_feriado + 1, feriado.mm_feriado, feriado.dd_feriado);
                            dataFim3 = new DateTime((int)fer.aa_feriado + 1, (int)feriado.mm_feriado_fim, (int)feriado.dd_feriado_fim);
                        }
                        else
                        {
                            //Pegar ano atual, anterior e posterior
                            int anoAtual = DateTime.UtcNow.Year;

                            //Colocando ano nos feriados fixos (onde o ano é nulo)
                            // Usando como base o ano de inicio do que está cadastrando para o ano inicial e final do registro existente, 
                            // pois feriado fixo não pode passar de um ano para o outro

                            //Ano 
                            datasFeriado = new Feriado
                            {
                                dt_inicio = new DateTime(anoAtual, fer.mm_feriado, fer.dd_feriado),
                                dt_fim = new DateTime(anoAtual, (int)fer.mm_feriado_fim, (int)fer.dd_feriado_fim),
                                cod_feriado = fer.cod_feriado
                            };
                            listaDatasFeriados.Add(datasFeriado);
                            
                            //Ano
                            dataInical1 = new DateTime(anoAtual, feriado.mm_feriado, feriado.dd_feriado);
                            dataFim1 = new DateTime(anoAtual, (int)feriado.mm_feriado_fim, (int)feriado.dd_feriado_fim);
                            
                        }




                    result = (from s in listaDatasFeriados
                              where
                              //Verificação com ano atual
                                  //Data de inicio cadastrada menor que a data inicio existente e Data de Fim cadastrada maior que a data de inicio existente 
                               (dataInical1 <= s.dt_inicio && dataFim1 >= s.dt_inicio) ||
                                  //Data de inicio cadastrada maior que a data inicio existente e Data de fim cadastrada menor que a data de fim existente 
                               (dataInical1 >= s.dt_inicio && dataFim1 <= s.dt_fim) ||
                                  //Data de inicio cadastrada maior que a data inicio existente e Data de inicio cadastrada menor que a data de fim existente 
                               (dataInical1 >= s.dt_inicio && dataInical1 <= s.dt_fim) ||
                                  //Data de inicio cadastrada menor que a data inicio existente e Data de fim cadastrada maior que a data de fim existente 
                               (dataInical1 >= s.dt_inicio && dataInical1 <= s.dt_fim) ||

                              //Verificação com ano anterior
                              ((dataInical2 != null && dataFim2 != null) &&
                                 //Data de inicio cadastrada menor que a data inicio existente e Data de Fim cadastrada maior que a data de inicio existente 
                               (dataInical2 <= s.dt_inicio && dataFim2 >= s.dt_inicio) ||
                                  //Data de inicio cadastrada maior que a data inicio existente e Data de fim cadastrada menor que a data de fim existente 
                               (dataInical2 >= s.dt_inicio && dataFim2 <= s.dt_fim) ||
                                  //Data de inicio cadastrada maior que a data inicio existente e Data de inicio cadastrada menor que a data de fim existente 
                               (dataInical2 >= s.dt_inicio && dataInical2 <= s.dt_fim) ||
                                  //Data de inicio cadastrada menor que a data inicio existente e Data de fim cadastrada maior que a data de fim existente 
                               (dataInical2 >= s.dt_inicio && dataInical2 <= s.dt_fim)) ||

                              //Verificação com ano posterior
                              ((dataInical3 != null && dataFim3 != null) &&
                                  //Data de inicio cadastrada menor que a data inicio existente e Data de Fim cadastrada maior que a data de inicio existente 
                               (dataInical3 <= s.dt_inicio && dataFim3 >= s.dt_inicio) ||
                                  //Data de inicio cadastrada maior que a data inicio existente e Data de fim cadastrada menor que a data de fim existente 
                               (dataInical3 >= s.dt_inicio && dataFim3 <= s.dt_fim) ||
                                  //Data de inicio cadastrada maior que a data inicio existente e Data de inicio cadastrada menor que a data de fim existente 
                               (dataInical3 >= s.dt_inicio && dataInical3 <= s.dt_fim) ||
                                  //Data de inicio cadastrada menor que a data inicio existente e Data de fim cadastrada maior que a data de fim existente 
                               (dataInical3 >= s.dt_inicio && dataInical3 <= s.dt_fim))

                              select s).ToList();

                }

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Feriado> getAllFeriados(List<int> feriados)
        {
            try
            {
                var sql = from feriado in db.Feriado
                          where feriados.Contains(feriado.cod_feriado)
                          select feriado;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool spRefazerProgramacoesFeriado(int cd_feriado, Feriado.TipoOperacaoSPFeriado operacao)
        {
            try
            {
                var data = context.Database.SqlQuery<int>(@"declare @ret int exec @ret = sp_analise_feriado " + @cd_feriado + "," + (int)operacao + " select @ret");
                var sql = data.First();
                return sql == 0 ? true : false;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
