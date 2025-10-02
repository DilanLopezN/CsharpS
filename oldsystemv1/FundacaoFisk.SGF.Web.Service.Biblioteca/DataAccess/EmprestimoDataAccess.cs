using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Biblioteca.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.GenericModel;

using FundacaoFisk.SGF.Web.Service.Biblioteca.Model;
using System.Data.Entity.Core.Objects;

namespace FundacaoFisk.SGF.Web.Services.Biblioteca.DataAccess
{
    public class EmprestimoDataAccess : GenericRepository<Emprestimo>, IEmprestimoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<PessoaSearchUI> getPessoaEmprestimoSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa) {
            try {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                if(tipoPessoa.Equals(1))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : (
                                p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                || p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                || p.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                            )
                          select new PessoaSearchUI {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_sexo
                          };

                else if(tipoPessoa.Equals(2))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) //&& !c.id_pessoa_empresa
                            && c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                          select new PessoaSearchUI {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              nm_sexo = null
                          };
                else
                    sql = from c in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_empresa).Any()
                                || c.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_empresa).Any()
                                || c.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                          select new PessoaSearchUI {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                      db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                         db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                              db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                 db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                              no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : "",
                              nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                          };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if (sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getPessoaBibliotecaSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            try {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                
                if(tipoPessoa.Equals(1))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                            && p.Emprestimos.Where(e => e.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any()
                          select new PessoaSearchUI {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_sexo
                          };

                else if(tipoPessoa.Equals(2))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : true)
                            && c.Emprestimos.Where(e => e.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_empresa).Any()).Any()
                          select new PessoaSearchUI {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.dc_num_cgc,
                              nm_sexo = null
                          };
                else
                    sql = from c in db.PessoaSGF.AsNoTracking()
                          where  c.Emprestimos.Where(e => e.cd_pessoa_escola == cd_empresa).Any()
                          select new PessoaSearchUI {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                     db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                             db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                              no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : "",
                              nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                          };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from c in sql
                              where c.no_pessoa.StartsWith(nome)
                              select c;
                    else
                        sql = from c in sql
                              where c.no_pessoa.Contains(nome)
                              select c;

                if (!string.IsNullOrEmpty(apelido))
                    if (inicio)
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    else
                        sql = from c in sql
                              where c.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                if(sexo > 0)
                    sql = from c in sql
                          where c.nm_sexo == sexo
                          select c;

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public Emprestimo getEmprestimo(int cd_biblioteca, int cd_escola) {
            try {
                Emprestimo emp = (from e in db.Emprestimo
                      where e.Item.ItemEscola.Where(ie => ie.cd_pessoa_escola == cd_escola).Any()
                            && e.cd_biblioteca == cd_biblioteca
                      select new {
                               cd_item = e.cd_item,
                               no_pessoa = e.Pessoa.no_pessoa,
                               dt_emprestimo = e.dt_emprestimo,
                               dt_prevista_devolucao = e.dt_prevista_devolucao,
                               no_item = e.Item.no_item,
                               cd_pessoa = e.Pessoa.cd_pessoa,
                               tx_obs_biblioteca = e.tx_obs_biblioteca,
                               dt_devolucao = e.dt_devolucao,
                               vl_multa_emprestimo = e.vl_multa_emprestimo,
                               vl_taxa_emprestimo = e.vl_taxa_emprestimo
                           }).ToList().Select(x => new Emprestimo {
                               cd_item = x.cd_item,
                               Pessoa = new PessoaSGF(){
                                   cd_pessoa = x.cd_pessoa,
                                   no_pessoa = x.no_pessoa
                               },
                               cd_pessoa = x.cd_pessoa,
                               dt_emprestimo = x.dt_emprestimo,
                               dt_prevista_devolucao = x.dt_prevista_devolucao,
                               Item = new Item(){
                                   no_item = x.no_item
                               },
                               tx_obs_biblioteca = x.tx_obs_biblioteca,
                               dt_devolucao = x.dt_devolucao,
                               vl_multa_emprestimo = x.vl_multa_emprestimo,
                               vl_taxa_emprestimo = x.vl_taxa_emprestimo
                           }).FirstOrDefault();

                return emp;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Emprestimo> getEmprestimoSearch(SearchParameters parametros, int? cd_pessoa, int? cd_item, bool? pendentes, DateTime? dt_inicial, DateTime? dt_final, bool? emprestimos, bool? devolucao, int cd_empresa) {
            try {
                IEntitySorter<EmprestimoSearch> sorter = EntitySorter<EmprestimoSearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<Emprestimo> sql;

                sql = from e in db.Emprestimo.AsNoTracking() 
                    where e.cd_pessoa_escola == cd_empresa
                    select e;

                if(cd_pessoa.HasValue)
                    sql = from e in sql
                          where e.cd_pessoa == cd_pessoa.Value
                          select e;
                if(cd_item.HasValue)
                    sql = from e in sql
                          where e.cd_item == cd_item.Value
                          select e;
                if(pendentes.HasValue){
                    if(pendentes.Value)
                        sql = from e in sql
                              where e.dt_devolucao == null
                              select e;
                }
                if(emprestimos.HasValue && emprestimos.Value){
                    if(dt_inicial.HasValue)
                        sql = from e in sql
                              where DbFunctions.TruncateTime(e.dt_emprestimo) >= dt_inicial.Value
                              select e;
                    if(dt_final.HasValue)
                        sql = from e in sql
                              where DbFunctions.TruncateTime(e.dt_emprestimo) <= dt_final.Value
                              select e;
                }
                if(devolucao.HasValue && devolucao.Value) {
                    if(dt_inicial.HasValue)
                        sql = from e in sql
                              where DbFunctions.TruncateTime(e.dt_devolucao) >= dt_inicial.Value
                              select e;
                    if(dt_final.HasValue)
                        sql = from e in sql
                              where DbFunctions.TruncateTime(e.dt_devolucao) <= dt_final.Value
                              select e;
                }

                var retorno = (from r in sql
                                select new EmprestimoSearch {
                                    cd_biblioteca = r.cd_biblioteca,
                                    dt_emprestimo = r.dt_emprestimo,
                                    dt_prevista_devolucao = r.dt_prevista_devolucao,
                                    dt_devolucao = r.dt_devolucao,
                                    vl_taxa_emprestimo = r.vl_taxa_emprestimo,
                                    vl_multa_emprestimo = r.vl_multa_emprestimo,
                                    no_pessoa = r.Pessoa.no_pessoa,
                                    no_item = r.Item.no_item
                                });

                var retorno2 = sorter.Sort(retorno);

                int limite = retorno2.Count();

                parametros.ajustaParametrosPesquisa(limite);
                var retorno3 = retorno2.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;

                List<Emprestimo> retornoFinal = new List<Emprestimo>();
                if(retorno3 != null)
                    foreach(EmprestimoSearch ret in retorno3) {
                        Emprestimo ret2 = new Emprestimo();
                        ret2.copy(ret);
                        retornoFinal.Add(ret2);
                    }

                return retornoFinal;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }
        public EmprestimoSearch getEmprestimoById(int cd_biblioteca, int cd_empresa)
        {
            try
            {
                EmprestimoSearch sql = (from r in db.Emprestimo
                                  where r.cd_pessoa_escola == cd_empresa &&
                                    r.cd_biblioteca == cd_biblioteca
                                 select new EmprestimoSearch
                               {
                                   cd_biblioteca = r.cd_biblioteca,
                                   cd_pessoa = r.cd_pessoa,
                                   vl_multa_emprestimo = r.vl_multa_emprestimo,
                                   no_pessoa = r.Pessoa.no_pessoa
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
