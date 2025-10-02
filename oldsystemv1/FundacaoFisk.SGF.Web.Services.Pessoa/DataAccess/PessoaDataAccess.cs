using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    using Componentes.GenericDataAccess;
    using Componentes.GenericDataAccess.GenericRepository;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
    using log4net;
    using Componentes.GenericDataAccess.GenericException;
    using FundacaoFisk.SGF.GenericModel;

    public class PessoaDataAccess : GenericRepository<PessoaSGF>, IPessoaDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(PessoaDataAccess));
        
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

//        private SGFWebContext db = new SGFWebContext();

        public IEnumerable<PessoaSearchUI> GetPessoaSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, byte tipo_pesquisa)
        {
            try{
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                sql = from c in db.PessoaSGF.AsNoTracking()
                      //where //c.id_pessoa_empresa == false
                      select new PessoaSearchUI
                        {
                            cd_pessoa = c.cd_pessoa,
                            no_pessoa = c.no_pessoa,
                            dc_num_pessoa = c.dc_num_pessoa,
                            nm_natureza_pessoa = c.nm_natureza_pessoa,
                            dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                            dt_cadastramento = c.dt_cadastramento,
                            id_pessoa_empresa = c.id_pessoa_empresa,
                            nm_cpf_cgc = c.nm_natureza_pessoa == (int)PessoaFisicaSGF.TipoPessoa.FISICA ?
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                    db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                            //img_pessoa = c.img_pessoa,
                            no_pessoa_dependente = c.nm_natureza_pessoa == (int)PessoaFisicaSGF.TipoPessoa.FISICA ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : "",
                            ext_img_pessoa = c.ext_img_pessoa,
                            txt_obs_pessoa = c.txt_obs_pessoa,
                            relacionamentos = c.PessoaFilhoRelacionamento,
                            nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                        };

                if (tipoPessoa == (int)PessoaFisicaSGF.TipoPessoa.FISICA)
                {
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              //img_pessoa = p.img_pessoa,
                              ext_img_pessoa = p.ext_img_pessoa,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_sexo,
                              email = p.TelefonePessoa.Where(tel => tel.cd_pessoa == p.cd_pessoa && tel.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail,
                          };
                }

                else
                    if (tipoPessoa == (int)PessoaFisicaSGF.TipoPessoa.JURIDICA)
                    {
                        if (tipo_pesquisa == (byte)PessoaFisicaSGF.TipoPesquisa.FILTRAR_EMPRESA)
                            sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                                  where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf)
                                    //&& c.id_pessoa_empresa == false
                                    && !(from p in db.PessoaSGF.OfType<Escola>()
                                         where p.cd_pessoa == c.cd_pessoa
                                         select p.cd_pessoa).Any()
                                  select new PessoaSearchUI
                                  {
                                      cd_pessoa = c.cd_pessoa,
                                      no_pessoa = c.no_pessoa,
                                      dc_num_pessoa = c.dc_num_pessoa,
                                      nm_natureza_pessoa = c.nm_natureza_pessoa,
                                      dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                                      dt_cadastramento = c.dt_cadastramento,
                                      id_pessoa_empresa = c.id_pessoa_empresa,
                                      nm_cpf_cgc = c.dc_num_cgc,
                                      //img_pessoa = c.img_pessoa,
                                      ext_img_pessoa = c.ext_img_pessoa
                                  };
                        else
                            sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                                  where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf)
                                    //&& c.id_pessoa_empresa == false
                                  select new PessoaSearchUI
                                  {
                                      cd_pessoa = c.cd_pessoa,
                                      no_pessoa = c.no_pessoa,
                                      dc_num_pessoa = c.dc_num_pessoa,
                                      nm_natureza_pessoa = c.nm_natureza_pessoa,
                                      dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                                      dt_cadastramento = c.dt_cadastramento,
                                      id_pessoa_empresa = c.id_pessoa_empresa,
                                      nm_cpf_cgc = c.dc_num_cgc,
                                      //img_pessoa = c.img_pessoa,
                                      ext_img_pessoa = c.ext_img_pessoa
                                  };
                    }
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
                    sql = from p in sql
                          where p.nm_sexo == sexo
                          select p;


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

        public IEnumerable<PessoaSearchUI> GetPessoaResponsavelCPFSearch(SearchParameters parametros, string nome, string apelido, bool inicio,  string cnpjCpf, int sexo)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where (!string.IsNullOrEmpty(cnpjCpf) ?  p.nm_cpf == cnpjCpf : true) && p.cd_pessoa_cpf == null && p.nm_cpf != null
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.nm_cpf,
                              //img_pessoa = p.img_pessoa,
                              ext_img_pessoa = p.ext_img_pessoa,
                              nm_sexo = p.nm_sexo
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
                    sql = from p in sql
                          where p.nm_sexo == sexo
                          select p;


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

        public IEnumerable<PessoaSearchUI> GetPessoaResponsavelSearch(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int cdPai, int sexo, int papel)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                //Apenas responsável, pais e empresa

                if (cdPai > 0)
                    sql = from c in db.PessoaSGF.AsNoTracking()
                          where //c.id_pessoa_empresa == false &&
                           (c.cd_pessoa == cdPai || c.PessoaFilhoRelacionamento.Where(r => (r.cd_papel_filho == 1 ||
                                                                r.cd_papel_filho == 3 ||
                                                                r.cd_papel_filho == 7) &&
                                                                r.cd_pessoa_pai == cdPai && 
                                                                (r.cd_papel_filho == papel || papel == 0)).Any())
                          select new PessoaSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              id_pessoa_empresa = c.id_pessoa_empresa,
                              nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                            db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                              //img_pessoa = c.img_pessoa,
                              ext_img_pessoa = c.ext_img_pessoa,
                              txt_obs_pessoa = c.txt_obs_pessoa,
                              nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null,
                              no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                          db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : ""
                          };
                else
                    sql = from c in db.PessoaSGF.AsNoTracking()
                          where //c.id_pessoa_empresa == false &&
                           (c.PessoaFilhoRelacionamento.Where(r => (r.cd_papel_filho == 1 || r.cd_papel_filho == 3 || r.cd_papel_filho == 7) && 
                                                                     (r.cd_papel_filho == papel || papel == 0)).Any())
                          select new PessoaSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              id_pessoa_empresa = c.id_pessoa_empresa,
                              nm_cpf_cgc = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                        db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                              //img_pessoa = c.img_pessoa,
                              ext_img_pessoa = c.ext_img_pessoa,
                              txt_obs_pessoa = c.txt_obs_pessoa,
                              nm_sexo = c.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null,
                              no_pessoa_dependente = c.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                         db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : ""
                          };

                if (tipoPessoa.Equals(1))
                {
                    if (cdPai > 0)
                        sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                                  where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                                        //&& p.id_pessoa_empresa == false
                                        && (p.cd_pessoa == cdPai || p.PessoaFilhoRelacionamento.Where(r => ((r.cd_papel_filho == 1 ||
                                                                        r.cd_papel_filho == 3 ||
                                                                        r.cd_papel_filho == 7 &&
                                                                        (r.cd_papel_filho == papel || papel == 0)) &&
                                                                        r.cd_pessoa_pai == cdPai)).Any())
                                       && (p.nm_sexo == sexo || sexo == 0)
                                  select new PessoaSearchUI
                                  {
                                      cd_pessoa = p.cd_pessoa,
                                      no_pessoa = p.no_pessoa,
                                      dc_num_pessoa = p.dc_num_pessoa,
                                      nm_natureza_pessoa = p.nm_natureza_pessoa,
                                      dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                                      dt_cadastramento = p.dt_cadastramento,
                                      id_pessoa_empresa = p.id_pessoa_empresa,
                                      nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                                      //img_pessoa = p.img_pessoa,
                                      ext_img_pessoa = p.ext_img_pessoa,
                                      no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                                      nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                                  };
                    else
                        sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                              where (!string.IsNullOrEmpty(cnpjCpf) ? (p.PessoaSGFQueUsoOCpf.nm_cpf == cnpjCpf || p.nm_cpf == cnpjCpf) : true)
                                    //&& p.id_pessoa_empresa == false
                                    && (p.PessoaFilhoRelacionamento.Where(r => (r.cd_papel_filho == 1 ||
                                                                    r.cd_papel_filho == 3 ||
                                                                    r.cd_papel_filho == 7) && 
                                                                    (r.cd_papel_filho == papel || papel == 0)).Any())
                                    && (p.nm_sexo == sexo || sexo == 0)
                              select new PessoaSearchUI
                              {
                                  cd_pessoa = p.cd_pessoa,
                                  no_pessoa = p.no_pessoa,
                                  dc_num_pessoa = p.dc_num_pessoa,
                                  nm_natureza_pessoa = p.nm_natureza_pessoa,
                                  dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                                  dt_cadastramento = p.dt_cadastramento,
                                  id_pessoa_empresa = p.id_pessoa_empresa,
                                  nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                                  //img_pessoa = p.img_pessoa,
                                  ext_img_pessoa = p.ext_img_pessoa,
                                  no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                                  nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                              };
                }

                else if (tipoPessoa.Equals(2))
                {
                    if (cdPai > 0)
                        sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                              where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) //&& c.id_pessoa_empresa == false
                                    //&& c.id_pessoa_empresa == false
                                    && (c.cd_pessoa == cdPai || c.PessoaFilhoRelacionamento.Where(r => (r.cd_papel_filho == 1 ||
                                                                    r.cd_papel_filho == 3 ||
                                                                    r.cd_papel_filho == 7 &&
                                                                    (r.cd_papel_filho == papel || papel == 0)) &&
                                                                    r.cd_pessoa_pai == cdPai).Any())
                              select new PessoaSearchUI
                              {
                                  cd_pessoa = c.cd_pessoa,
                                  no_pessoa = c.no_pessoa,
                                  dc_num_pessoa = c.dc_num_pessoa,
                                  nm_natureza_pessoa = c.nm_natureza_pessoa,
                                  dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                                  dt_cadastramento = c.dt_cadastramento,
                                  id_pessoa_empresa = c.id_pessoa_empresa,
                                  nm_cpf_cgc = c.dc_num_cgc,
                                  img_pessoa = c.img_pessoa,
                                  ext_img_pessoa = c.ext_img_pessoa
                              };
                    else
                        sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                              where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) //&& c.id_pessoa_empresa == false
                                    //&& c.id_pessoa_empresa == false
                                    && (c.PessoaFilhoRelacionamento.Where(r => (r.cd_papel_filho == 1 ||
                                                                    r.cd_papel_filho == 3 ||
                                                                    r.cd_papel_filho == 7 && 
                                                                    (r.cd_papel_filho == papel || papel == 0))).Any())
                              select new PessoaSearchUI
                              {
                                  cd_pessoa = c.cd_pessoa,
                                  no_pessoa = c.no_pessoa,
                                  dc_num_pessoa = c.dc_num_pessoa,
                                  nm_natureza_pessoa = c.nm_natureza_pessoa,
                                  dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                                  dt_cadastramento = c.dt_cadastramento,
                                  id_pessoa_empresa = c.id_pessoa_empresa,
                                  nm_cpf_cgc = c.dc_num_cgc,
                                  img_pessoa = c.img_pessoa,
                                  ext_img_pessoa = c.ext_img_pessoa
                              };
                }
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

        public IEnumerable<PessoaSearchUI> GetPessoaSearchRelac(SearchParameters parametros, string nome, string apelido, int tipoPessoa, string cnpjCpf,int cd_pessoa_empresa)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;

                sql = from p in db.PessoaSGF.AsNoTracking()
                      where //p.id_pessoa_empresa == false &&
                            p.PessoaEmpresa.Where(pe => pe.cd_pessoa == cd_pessoa_empresa).Any()

                      select new PessoaSearchUI
                      {
                          cd_pessoa = p.cd_pessoa,
                          no_pessoa = p.no_pessoa,
                          dc_num_pessoa = p.dc_num_pessoa,
                          nm_natureza_pessoa = p.nm_natureza_pessoa,
                          dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                          dt_cadastramento = p.dt_cadastramento,
                          id_pessoa_empresa = p.id_pessoa_empresa,
                          nm_cpf_cgc = p.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                        db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().cd_pessoa_cpf != null ?
                                           db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.nm_cpf :
                                                db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                                                    db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                          img_pessoa = p.img_pessoa,
                          ext_img_pessoa = p.ext_img_pessoa,
                          txt_obs_pessoa = p.txt_obs_pessoa,
                          relacionamentos = p.PessoaFilhoRelacionamento,
                          nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null,
                          no_pessoa_dependente = p.nm_natureza_pessoa == 1 ?//Pessoa Fisica
                                         db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().PessoaSGFQueUsoOCpf.no_pessoa : ""
                      };

                if (tipoPessoa.Equals(1))
                {
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? p.nm_cpf == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) && //p.id_pessoa_empresa == false &&
                          p.PessoaEmpresa.Where(pe => pe.cd_pessoa == cd_pessoa_empresa).Any()
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              img_pessoa = p.img_pessoa,
                              ext_img_pessoa = p.ext_img_pessoa,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null
                          };
                }

                else if (tipoPessoa.Equals(2))
                    sql = from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? c.dc_num_cgc == cnpjCpf : string.IsNullOrEmpty(cnpjCpf) && //c.id_pessoa_empresa == false &&
                          c.PessoaEmpresa.Where(pe => pe.cd_pessoa == cd_pessoa_empresa).Any()
                          select new PessoaSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                              dc_num_pessoa = c.dc_num_pessoa,
                              nm_natureza_pessoa = c.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_cadastramento,
                              id_pessoa_empresa = c.id_pessoa_empresa,
                              nm_cpf_cgc = c.dc_num_cgc,
                              img_pessoa = c.img_pessoa,
                              ext_img_pessoa = c.ext_img_pessoa
                          };

                sql = sorter.Sort(sql);

                if (!string.IsNullOrEmpty(nome))
                    sql = from c in sql
                          where c.no_pessoa.StartsWith(nome)
                          select c;

                if (!string.IsNullOrEmpty(apelido))
                    sql = from c in sql
                          where c.dc_reduzido_pessoa.Contains(apelido)
                          select c;

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

        public IEnumerable<PessoaSearchUI> getTdsPessoaSearchEscolaCadMovimento(SearchParameters parametros, string nome, string apelido, bool inicio, int tipoPessoa, string cnpjCpf, int sexo, int cd_empresa)
        {
            try
            {
                int? cdEmpresaLig = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_empresa_coligada == cd_empresa select empresa.cd_pessoa).FirstOrDefault();
                int cd_escola = cd_empresa;
                cd_empresa = (int)(cdEmpresaLig == null ? cd_empresa : cdEmpresaLig);
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sql;
                //Trazer somente as pessoas que tenham CPF ou CNPJ (donas).
                sql = from c in db.PessoaSGF.AsNoTracking()
                      where (c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ?
                                (from p in db.FuncionarioSGF where p.cd_pessoa_funcionario == c.cd_pessoa && p.cd_pessoa_empresa == cd_escola select p.cd_pessoa_funcionario).Any() ||
                                (from a in db.Aluno where a.cd_pessoa_aluno == c.cd_pessoa && a.cd_pessoa_escola == cd_empresa select a.cd_pessoa_aluno).Any() ||
                                 c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_escola).Any()
                            : c.PessoaEmpresa.Where(pe => pe.cd_escola == cd_escola).Any())

                      select new PessoaSearchUI
                      {
                          cd_pessoa = c.cd_pessoa,
                          no_pessoa = c.no_pessoa,
                          dc_num_pessoa = c.dc_num_pessoa,
                          nm_natureza_pessoa = c.nm_natureza_pessoa,
                          dc_reduzido_pessoa = c.dc_reduzido_pessoa,
                          dt_cadastramento = c.dt_cadastramento,
                          nm_cpf_cgc = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_cpf :
                          db.PessoaSGF.OfType<PessoaJuridicaSGF>().Where(pj => pj.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaJuridicaSGF>().dc_num_cgc,
                          nm_sexo = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == c.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null,
                          existeAluno = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.Aluno.Any(x => x.cd_pessoa_aluno == c.cd_pessoa && x.cd_pessoa_escola == cd_empresa) : false,
                          cd_aluno = c.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.Aluno.Where(x => x.cd_pessoa_aluno == c.cd_pessoa && x.cd_pessoa_escola == cd_empresa).FirstOrDefault().cd_aluno : 0,
                      };

                if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.FISICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? p.nm_cpf == cnpjCpf : 
                            (
                                p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_escola).Any()
                                || p.PessoaFisicaFuncioanrio.Where(pff => pff.cd_pessoa_empresa == cd_escola).Any()
                                || p.Alunos.Where(a => a.cd_pessoa_escola == cd_empresa).Any()
                            )
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.cd_pessoa_cpf != null ? p.PessoaSGFQueUsoOCpf.nm_cpf : p.nm_cpf,
                              no_pessoa_dependente = p.PessoaSGFQueUsoOCpf.no_pessoa,
                              nm_sexo = p.nm_natureza_pessoa == 1 ? db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == p.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().nm_sexo : null,
                              existeAluno = p.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.Aluno.Any(x => x.cd_pessoa_aluno == p.cd_pessoa && x.cd_pessoa_escola == cd_empresa) : false,
                              cd_aluno = p.nm_natureza_pessoa == (int)PessoaSGF.TipoPessoa.FISICA ? db.Aluno.Where(x => x.cd_pessoa_aluno == p.cd_pessoa && x.cd_pessoa_escola == cd_empresa).FirstOrDefault().cd_aluno : 0
                          };

                if (tipoPessoa.Equals((int)PessoaSGF.TipoPessoa.JURIDICA))
                    sql = from p in db.PessoaSGF.OfType<PessoaJuridicaSGF>().AsNoTracking()
                          where !string.IsNullOrEmpty(cnpjCpf) ? p.dc_num_cgc == cnpjCpf : p.dc_num_cgc != null && //p.id_pessoa_empresa == false &&
                          p.PessoaEmpresa.Where(pe => pe.cd_escola == cd_escola).Any()
                          select new PessoaSearchUI
                          {
                              cd_pessoa = p.cd_pessoa,
                              no_pessoa = p.no_pessoa,
                              dc_num_pessoa = p.dc_num_pessoa,
                              nm_natureza_pessoa = p.nm_natureza_pessoa,
                              dc_reduzido_pessoa = p.dc_reduzido_pessoa,
                              dt_cadastramento = p.dt_cadastramento,
                              id_pessoa_empresa = p.id_pessoa_empresa,
                              nm_cpf_cgc = p.dc_num_cgc
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
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RelacionamentoSGF> GetRelacionamentoPorCodPessoaPai(int codPessoaPai) {
            try{
                return (from r in db.RelacionamentoSGF.Include(r =>r.RelacionamentoPaiPapel)
                        where r.cd_pessoa_pai == codPessoaPai
                        select r);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<RelacionamentoSGF> GetRelacionamentoPorCodPessoaFilho(int codPessoaFilho) {
            try{
                return from r in db.RelacionamentoSGF.Include(r => r.RelacionamentoFilhoPapel)
                      where r.cd_pessoa_filho == codPessoaFilho
                     select r;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF FindIdPessoa(int codPessoa)
        {
            try{
                var c = (from r in db.PessoaSGF
                         where r.cd_pessoa.Equals(codPessoa)
                         select r).FirstOrDefault();
                return c;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF FindIdPessoaFisica(int codPessoaFisica)
        {
            try{
                
                var c = (from r in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                        .Include("EnderecoPrincipal")
                         where r.cd_pessoa.Equals(codPessoaFisica) 
                         select r).FirstOrDefault();
                return c;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF UpdateValuesEnderecoPrincipal(PessoaFisicaSGF pessoaFisicaContext, PessoaFisicaSGF pessoaFisicaView)
        {
            try
            {
                db.Entry(pessoaFisicaContext.EnderecoPrincipal).CurrentValues.SetValues(pessoaFisicaView.EnderecoPrincipal);
                return pessoaFisicaContext;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaJuridicaSGF FindIdPessoaJuridica(int codPessoa)
        {
            try{
                var c = (from r in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                         where r.cd_pessoa.Equals(codPessoa)
                         select r).FirstOrDefault();
                return c;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void PostNovaPessoa(PessoaSGF pessoa)
        {
            try{
                db.PessoaSGF.Add(pessoa);
                db.SaveChanges();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public void PostNovaPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF)
        {
            try{
                db.PessoaSGF.Add(pessoaFisicaSGF);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public TelefoneSGF PostNovoContato(TelefoneSGF contato)
        {
            try{
                var telefone = db.TelefoneSGF.Add(contato);
                db.SaveChanges();
                return telefone;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void PostNovoEndereco(EnderecoSGF endereco)
        {
            try{
                db.EnderecoSGF.Add(endereco);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void PutPessoa(PessoaSGF pessoa)
        {
            try{
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public void PutPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF)
        {
            try{
                //pessoaFisica.tb_pessoa = null;
                pessoaFisicaSGF.PessoaSGFQueUsoOCpf = null;
                //DB1.Entry(pessoaFisica).State = System.Data.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void PutEndereco(EnderecoSGF endereco)
        {
            try{
                //DB1.Entry(endereco).State = System.Data.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void PutTelefone(TelefoneSGF telefone)
        {
            try{
                // DB1.Entry(telefone).State = System.Data.EntityState.Modified;
                 db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void DeletePessoa(PessoaSGF pessoa)
        {
            try{
                db.PessoaSGF.Remove(pessoa);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void DeletePessoaFisica(PessoaFisicaSGF pessoaFisicaSGF)
        {
            try{
                db.PessoaSGF.Remove(pessoaFisicaSGF);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void DeleteTelefone(TelefoneSGF telefone)
        {
            try{
                db.TelefoneSGF.Remove(telefone);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void DeleteEndereco(EnderecoSGF endereco)
        {
            try{
                db.EnderecoSGF.Remove(endereco);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSearchUI VerificarExisitsPessoByCpfOrCdPessoa(string cpf, string email, string nome, int cd_pessoa_cpf, int? cdPessoa, int cd_escola)
        {
            try
            {
                PessoaFisicaSGF sql = new PessoaFisicaSGF();
                var retorno = from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                              select c;
                if (!string.IsNullOrEmpty(cpf))
                    retorno = from pf in retorno
                              where pf.nm_cpf == cpf
                              select pf;

                if (!string.IsNullOrEmpty(email))
                    retorno = (from pf in retorno
                        join contato in db.TelefoneSGF on pf.cd_pessoa equals contato.cd_pessoa
                        where contato.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL &&
                              contato.dc_fone_mail == email
                        select pf);

                if (!string.IsNullOrEmpty(nome) && cd_pessoa_cpf > 0)
                    retorno = from pf in retorno
                              where pf.no_pessoa == nome && pf.cd_pessoa_cpf == cd_pessoa_cpf
                        select pf;

                if (cdPessoa != null && cdPessoa > 0)
                    retorno = from pf in retorno
                              where pf.cd_pessoa == cdPessoa
                              select pf;

                sql = (from p in retorno
                       select new
                       {
                           p.cd_pessoa,
                           p.no_pessoa,
                           p.dt_cadastramento,
                           p.dc_reduzido_pessoa,
                           p.id_pessoa_empresa,
                           p.nm_natureza_pessoa,
                           p.dc_num_pessoa,
                           p.id_exportado,
                           p.txt_obs_pessoa,
                           p.cd_atividade_principal,
                           p.cd_endereco_principal,
                           p.cd_telefone_principal,
                           p.ext_img_pessoa,
                           p.cd_estado_civil,
                           p.cd_loc_nascimento,
                           p.cd_loc_nacionalidade,
                           p.cd_tratamento,
                           p.cd_pessoa_cpf,
                           p.nm_sexo,
                           p.dt_nascimento,
                           p.dt_casamento,
                           p.nm_cpf,
                           p.nm_doc_identidade,
                           p.dc_carteira_trabalho,
                           p.dc_carteira_motorista,
                           p.dc_num_titulo_eleitor,
                           p.dc_num_insc_inss,
                           p.dc_num_crc,
                           p.dt_venc_habilitacao,
                           p.cd_integracao_folha,
                           p.cd_orgao_expedidor,
                           p.cd_estado_expedidor,
                           p.dt_emis_expedidor,
                           p.dc_num_certidao_nascimento,
                           p.Atividade.no_atividade,
                           no_local_nascimento = p.LocalNascimento.no_localidade,
                           p.PessoaRaf
                       }).ToList().Select(x => new PessoaFisicaSGF
                       {
                           cd_pessoa = x.cd_pessoa,
                           no_pessoa = x.no_pessoa,
                           dt_cadastramento = x.dt_cadastramento,
                           dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                           id_pessoa_empresa = x.id_pessoa_empresa,
                           nm_natureza_pessoa = x.nm_natureza_pessoa,
                           dc_num_pessoa = x.dc_num_pessoa,
                           id_exportado = x.id_exportado,
                           //cd_papel_principal = x.cd_papel_principal,
                           txt_obs_pessoa = x.txt_obs_pessoa,
                           cd_atividade_principal = x.cd_atividade_principal,
                           cd_endereco_principal = x.cd_endereco_principal,
                           cd_telefone_principal = x.cd_telefone_principal,
                           ext_img_pessoa = x.ext_img_pessoa,
                           cd_estado_civil = x.cd_estado_civil,
                           cd_loc_nascimento = x.cd_loc_nascimento,
                           cd_loc_nacionalidade = x.cd_loc_nacionalidade,
                           cd_tratamento = x.cd_tratamento,
                           cd_pessoa_cpf = x.cd_pessoa_cpf,
                           nm_sexo = x.nm_sexo,
                           dt_nascimento = x.dt_nascimento,
                           dt_casamento = x.dt_casamento,
                           nm_cpf = x.nm_cpf,
                           nm_doc_identidade = x.nm_doc_identidade,
                           dc_carteira_trabalho = x.dc_carteira_trabalho,
                           dc_carteira_motorista = x.dc_carteira_motorista,
                           dc_num_titulo_eleitor = x.dc_num_titulo_eleitor,
                           dc_num_insc_inss = x.dc_num_insc_inss,
                           dc_num_crc = x.dc_num_crc,
                           dt_venc_habilitacao = x.dt_venc_habilitacao,
                           cd_integracao_folha = x.cd_integracao_folha,
                           cd_orgao_expedidor = x.cd_orgao_expedidor,
                           cd_estado_expedidor = x.cd_estado_expedidor,
                           dt_emis_expedidor = x.dt_emis_expedidor,
                           dc_num_certidao_nascimento = x.dc_num_certidao_nascimento,
                           Atividade = new Atividade
                           {
                               no_atividade = x.no_atividade,
                               cd_atividade = x.cd_atividade_principal.HasValue ? (int)x.cd_atividade_principal : 0
                           },
                           LocalNascimento = new LocalidadeSGF { no_localidade = x.no_local_nascimento },
                           PessoaRaf = (x.PessoaRaf != null && x.PessoaRaf.Count > 0) ? new List<PessoaRaf>() {new PessoaRaf()
                           {
                               cd_pessoa_raf = x.PessoaRaf.First().cd_pessoa_raf,
                               cd_pessoa = x.PessoaRaf.First().cd_pessoa,
                               nm_raf = x.PessoaRaf.First().nm_raf,
                               id_raf_liberado = x.PessoaRaf.First().id_raf_liberado,
                               nm_tentativa = x.PessoaRaf.First().nm_tentativa,
                               id_bloqueado = x.PessoaRaf.First().id_bloqueado,
                               id_trocar_senha = x.PessoaRaf.First().id_trocar_senha,
                               dt_expiracao_senha = x.PessoaRaf.First().dt_expiracao_senha,
                               dt_limite_bloqueio = x.PessoaRaf.First().dt_limite_bloqueio,
                               hasPassCreated = x.PessoaRaf.First().dc_senha_raf == null || x.PessoaRaf.First().dc_senha_raf == String.Empty ? false : true

                           }} : new List<PessoaRaf>() {}
                           }).FirstOrDefault();

                if (sql != null && sql.cd_pessoa > 0)
                {


                    sql.TelefonePessoa = (from telefone in db.TelefoneSGF
                                          where telefone.cd_pessoa == sql.cd_pessoa
                                          select new
                                          {
                                              cd_telefone = telefone.cd_telefone,
                                              cd_operadora = telefone.cd_operadora,
                                              cd_classe_telefone = telefone.cd_classe_telefone,
                                              cd_endereco = telefone.cd_endereco,
                                              cd_tipo_telefone = telefone.cd_tipo_telefone,
                                              dc_fone_mail = telefone.dc_fone_mail,
                                              des_tipo_contato = telefone.TelefoneTipo.no_tipo_telefone,
                                              no_classe = telefone.ClasseTelefone.dc_classe_telefone,
                                              no_pessoa = telefone.PessoaTelefone.no_pessoa,
                                              id_telefone_principal = telefone.id_telefone_principal
                                          }).ToList().Select(x => new TelefoneSGF
                                          {
                                              cd_telefone = x.cd_telefone,
                                              cd_operadora = x.cd_operadora,
                                              cd_classe_telefone = x.cd_classe_telefone,
                                              cd_endereco = x.cd_endereco,
                                              cd_tipo_telefone = x.cd_tipo_telefone,
                                              dc_fone_mail = x.dc_fone_mail,
                                              TelefoneTipo = new TipoTelefoneSGF
                                              {
                                                  no_tipo_telefone = x.des_tipo_contato
                                              },
                                              ClasseTelefone = new ClasseTelefoneSGF
                                              {
                                                  dc_classe_telefone = x.no_classe
                                              },
                                              PessoaTelefone = new PessoaSGF
                                              {
                                                  no_pessoa = x.no_pessoa
                                              },
                                              id_telefone_principal = x.id_telefone_principal
                                          }).ToList();

                    sql.EnderecoPrincipal = (from e in db.EnderecoSGF
                                                  where e.cd_pessoa == sql.cd_pessoa && sql.cd_endereco_principal == e.cd_endereco
                                                  join p in db.PessoaSGF on e.cd_pessoa equals p.cd_pessoa
                                                  select new
                                                  {
                                                      cd_endereco = e.cd_endereco,
                                                      cd_loc_estado = e.cd_loc_estado,
                                                      cd_loc_cidade = e.cd_loc_cidade,
                                                      cd_loc_bairro = e.cd_loc_bairro,
                                                      cd_loc_logradouro = e.cd_loc_logradouro,
                                                      cd_tipo_endereco = e.cd_tipo_endereco,
                                                      cd_tipo_logradouro = e.cd_tipo_logradouro,
                                                      noLocCidade = e.Cidade.no_localidade,
                                                      noLocBairro = e.Bairro.no_localidade,
                                                      noLocRua = e.Logradouro.no_localidade,
                                                      dc_num_cep = e.Logradouro.dc_num_cep,
                                                      numero = e.dc_num_endereco,
                                                      dc_complemento = e.dc_compl_endereco,
                                                      cd_loc_pais = e.cd_loc_pais
                                                  }).ToList().Select(x => new EnderecoSGF
                                                  {
                                                      cd_endereco = x.cd_endereco,
                                                      cd_loc_estado = x.cd_loc_estado,
                                                      cd_loc_cidade = x.cd_loc_cidade,
                                                      cd_loc_bairro = x.cd_loc_bairro,
                                                      cd_loc_logradouro = x.cd_loc_logradouro,
                                                      cd_tipo_endereco = x.cd_tipo_endereco,
                                                      cd_tipo_logradouro = x.cd_tipo_logradouro,
                                                      noLocCidade = x.noLocCidade,
                                                      noLocBairro = x.noLocBairro,
                                                      noLocRua = x.noLocRua,
                                                      num_cep = x.dc_num_cep,
                                                      dc_num_endereco = x.numero,
                                                      cd_loc_pais = x.cd_loc_pais,
                                                      dc_compl_endereco = x.dc_complemento,
                                                  }).FirstOrDefault();

                    sql.PessoaPaiRelacionamento = (from rp in db.RelacionamentoSGF
                                                   where rp.cd_pessoa_pai == sql.cd_pessoa
                                                   //&& rp.PessoaFilho.nm_natureza_pessoa == (int)Pessoa.TipoPessoa.FISICA
                                                   select new
                                                   {
                                                       cd_pessoa_pai = rp.cd_pessoa_pai,
                                                       cd_pessoa_filho = rp.cd_pessoa_filho,
                                                       cd_papel_filho = rp.cd_papel_filho,
                                                       cd_papel_pai = rp.cd_papel_pai,
                                                       cd_relacionamento = rp.cd_relacionamento,
                                                       nm_tipo_papel_pai = rp.RelacionamentoPaiPapel.nm_tipo_papel,
                                                       no_papel_pai = rp.RelacionamentoPaiPapel.no_papel,
                                                       nm_tipo_papel_filho = rp.RelacionamentoFilhoPapel.nm_tipo_papel,
                                                       no_papel_filho = rp.RelacionamentoFilhoPapel.no_papel,
                                                       no_pessoa_filho = rp.PessoaFilho.no_pessoa,
                                                       nm_natureza_filho = rp.PessoaFilho.nm_natureza_pessoa,
                                                       cd_qualif_relacionamento = rp.cd_qualif_relacionamento,
                                                       desc_qualif_relacionamento = rp.RelacionamentoQualif.no_qualif_relacionamento,
                                                       no_pessoa_Pai = rp.PessoaPai.no_pessoa,
                                                       nm_natureza_Pai = rp.PessoaPai.nm_natureza_pessoa,
                                                       dc_telefone = rp.PessoaFilho.Telefone.dc_fone_mail
                                                   }).ToList().Select(x => new RelacionamentoSGF
                                                   {
                                                       cd_pessoa_pai = x.cd_pessoa_pai,
                                                       cd_pessoa_filho = x.cd_pessoa_filho,
                                                       cd_papel_filho = x.cd_papel_filho,
                                                       cd_papel_pai = x.cd_papel_pai,
                                                       cd_relacionamento = x.cd_relacionamento,
                                                       cd_qualif_relacionamento = x.cd_qualif_relacionamento,
                                                       desc_qualif_relacionamento = x.desc_qualif_relacionamento,
                                                       RelacionamentoPaiPapel = new PapelSGF { no_papel = x.no_papel_pai, nm_tipo_papel = x.nm_tipo_papel_pai },
                                                       RelacionamentoFilhoPapel = new PapelSGF { no_papel = x.no_papel_filho, nm_tipo_papel = x.nm_tipo_papel_filho },
                                                       PessoaFilho = new PessoaSGF { cd_pessoa = x.cd_pessoa_filho, no_pessoa = x.no_pessoa_filho, nm_natureza_pessoa = x.nm_natureza_filho, Telefone = 
                                                            new TelefoneSGF { dc_fone_mail = x.dc_telefone, cd_pessoa = x.cd_pessoa_filho } 
                                                       },
                                                       PessoaPai = new PessoaSGF { cd_pessoa = x.cd_pessoa_pai, no_pessoa = x.no_pessoa_Pai, nm_natureza_pessoa = x.nm_natureza_Pai }
                                                   }).ToList();

                    sql.PessoaFilhoRelacionamento = (from rp in db.RelacionamentoSGF
                                                where rp.cd_pessoa_filho == sql.cd_pessoa
                                                select new
                                                {
                                                    cd_pessoa_pai = rp.cd_pessoa_pai,
                                                    cd_pessoa_filho = rp.cd_pessoa_filho,
                                                    cd_papel_filho = rp.cd_papel_filho,
                                                    cd_papel_pai = rp.cd_papel_pai,
                                                    cd_relacionamento = rp.cd_relacionamento,
                                                    nm_tipo_papel_pai = rp.RelacionamentoPaiPapel.nm_tipo_papel,
                                                    no_papel_pai = rp.RelacionamentoPaiPapel.no_papel,
                                                    nm_tipo_papel_filho = rp.RelacionamentoFilhoPapel.nm_tipo_papel,
                                                    no_papel_filho = rp.RelacionamentoFilhoPapel.no_papel,
                                                    no_pessoa_filho = rp.PessoaFilho.no_pessoa,
                                                    nm_natureza_filho = rp.PessoaFilho.nm_natureza_pessoa,
                                                    no_pessoa_Pai = rp.PessoaPai.no_pessoa,
                                                    nm_natureza_Pai = rp.PessoaPai.nm_natureza_pessoa,
                                                    cd_qualif_relacionamento = rp.cd_qualif_relacionamento,
                                                    desc_qualif_relacionamento = rp.RelacionamentoQualif.no_qualif_relacionamento,
                                                    dc_telefone = rp.PessoaPai.Telefone.dc_fone_mail
                                                }).ToList().Select(x => new RelacionamentoSGF
                                                {
                                                    cd_pessoa_pai = x.cd_pessoa_pai,
                                                    cd_pessoa_filho = x.cd_pessoa_filho,
                                                    cd_papel_filho = x.cd_papel_filho,
                                                    cd_papel_pai = x.cd_papel_pai,
                                                    cd_relacionamento = x.cd_relacionamento,
                                                    RelacionamentoPaiPapel = new PapelSGF { no_papel = x.no_papel_pai, nm_tipo_papel = x.nm_tipo_papel_pai },
                                                    RelacionamentoFilhoPapel = new PapelSGF { no_papel = x.no_papel_filho, nm_tipo_papel = x.nm_tipo_papel_filho },
                                                    PessoaFilho = new PessoaSGF { cd_pessoa = x.cd_pessoa_filho, no_pessoa = x.no_pessoa_filho, nm_natureza_pessoa = x.nm_natureza_filho},
                                                    PessoaPai = new PessoaSGF { cd_pessoa = x.cd_pessoa_pai, no_pessoa = x.no_pessoa_Pai, nm_natureza_pessoa = x.nm_natureza_Pai, Telefone = 
                                                        new TelefoneSGF { dc_fone_mail = x.dc_telefone, cd_pessoa = x.cd_pessoa_pai } 
                                                    },
                                                    cd_qualif_relacionamento = x.cd_qualif_relacionamento,
                                                    desc_qualif_relacionamento = x.desc_qualif_relacionamento,
                                                }).ToList();
                    sql.Enderecos = (from e in db.EnderecoSGF
                                          where e.cd_pessoa == sql.cd_pessoa &&
                                          (sql.cd_endereco_principal == null || e.cd_endereco != sql.cd_endereco_principal)
                                          select new
                                          {
                                              cd_endereco = e.cd_endereco,
                                              cd_loc_estado = e.cd_loc_estado,
                                              cd_loc_cidade = e.cd_loc_cidade,
                                              cd_loc_bairro = e.cd_loc_bairro,
                                              cd_loc_logradouro = e.cd_loc_logradouro,
                                              cd_tipo_endereco = e.cd_tipo_endereco,
                                              cd_tipo_logradouro = e.cd_tipo_logradouro,
                                              noLocCidade = e.Cidade.no_localidade,
                                              noLocBairro = e.Bairro.no_localidade,
                                              noLocRua = e.Logradouro.no_localidade,
                                              dc_num_cep = e.Logradouro.dc_num_cep,
                                              numero = e.dc_num_endereco,
                                              dc_complemento = e.dc_compl_endereco,
                                              cd_loc_pais = e.cd_loc_pais
                                          }).ToList().Select(x => new EnderecoSGF
                                          {
                                              cd_endereco = x.cd_endereco,
                                              cd_loc_estado = x.cd_loc_estado,
                                              cd_loc_cidade = x.cd_loc_cidade,
                                              cd_loc_bairro = x.cd_loc_bairro,
                                              cd_loc_logradouro = x.cd_loc_logradouro,
                                              cd_tipo_endereco = x.cd_tipo_endereco,
                                              cd_tipo_logradouro = x.cd_tipo_logradouro,
                                              noLocCidade = x.noLocCidade,
                                              noLocBairro = x.noLocBairro,
                                              noLocRua = x.noLocRua,
                                              num_cep = x.dc_num_cep,
                                              dc_num_endereco = x.numero,
                                              cd_loc_pais = x.cd_loc_pais,
                                              dc_compl_endereco = x.dc_complemento,
                                          }).ToList();

                }
                PessoaFisicaSearchUI pessoaFisicaUI = new PessoaFisicaSearchUI();
                pessoaFisicaUI.pessoaFisica = sql;
                if (sql != null && sql.cd_pessoa_cpf != null && sql.cd_pessoa_cpf > 0)
                {
                    pessoaFisicaUI.no_pessoa_cpf = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>() where p.cd_pessoa == sql.cd_pessoa_cpf select p).FirstOrDefault().no_pessoa;
                }
                   

                return pessoaFisicaUI;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public PessoaFisicaSearchUI VerificaExisitsAlunoRafMatricula(int? cdPessoa, int cd_escola)
        {
            try
            {
                PessoaFisicaSGF sql = new PessoaFisicaSGF();
                var retorno = from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                              select c;
                

                if (cdPessoa != null && cdPessoa > 0)
                    retorno = from pf in retorno
                              where pf.cd_pessoa == cdPessoa
                              select pf;

                sql = (from p in retorno
                       select new
                       {
                           p.cd_pessoa,
                           p.no_pessoa,
                           p.dc_reduzido_pessoa,
                           p.id_pessoa_empresa,
                           p.PessoaRaf
                       }).ToList().Select(x => new PessoaFisicaSGF
                       {
                           cd_pessoa = x.cd_pessoa,
                           no_pessoa = x.no_pessoa,
                           dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                           id_pessoa_empresa = x.id_pessoa_empresa,
                           PessoaRaf = (x.PessoaRaf != null && x.PessoaRaf.Count > 0) ? new List<PessoaRaf>() {new PessoaRaf()
                           {
                               cd_pessoa_raf = x.PessoaRaf.First().cd_pessoa_raf,
                               cd_pessoa = x.PessoaRaf.First().cd_pessoa,
                               nm_raf = x.PessoaRaf.First().nm_raf,
                               id_raf_liberado = x.PessoaRaf.First().id_raf_liberado,
                               nm_tentativa = x.PessoaRaf.First().nm_tentativa,
                               id_bloqueado = x.PessoaRaf.First().id_bloqueado,
                               id_trocar_senha = x.PessoaRaf.First().id_trocar_senha,
                               dt_expiracao_senha = x.PessoaRaf.First().dt_expiracao_senha,
                               dt_limite_bloqueio = x.PessoaRaf.First().dt_limite_bloqueio,
                               hasPassCreated = x.PessoaRaf.First().dc_senha_raf == null || x.PessoaRaf.First().dc_senha_raf == String.Empty ? false : true

                           }} : new List<PessoaRaf>() { }
                       }).FirstOrDefault();

                if (sql != null && sql.cd_pessoa > 0)
                {


                    sql.TelefonePessoa = (from telefone in db.TelefoneSGF
                                          where telefone.cd_pessoa == sql.cd_pessoa &&
                                                telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL
                                          select new
                                          {
                                              cd_telefone = telefone.cd_telefone,
                                              cd_operadora = telefone.cd_operadora,
                                              cd_classe_telefone = telefone.cd_classe_telefone,
                                              cd_endereco = telefone.cd_endereco,
                                              cd_tipo_telefone = telefone.cd_tipo_telefone,
                                              dc_fone_mail = telefone.dc_fone_mail,
                                              des_tipo_contato = telefone.TelefoneTipo.no_tipo_telefone,
                                              no_classe = telefone.ClasseTelefone.dc_classe_telefone,
                                              no_pessoa = telefone.PessoaTelefone.no_pessoa,
                                              id_telefone_principal = telefone.id_telefone_principal
                                          }).ToList().Select(x => new TelefoneSGF
                                          {
                                              cd_telefone = x.cd_telefone,
                                              cd_operadora = x.cd_operadora,
                                              cd_classe_telefone = x.cd_classe_telefone,
                                              cd_endereco = x.cd_endereco,
                                              cd_tipo_telefone = x.cd_tipo_telefone,
                                              dc_fone_mail = x.dc_fone_mail,
                                              TelefoneTipo = new TipoTelefoneSGF
                                              {
                                                  no_tipo_telefone = x.des_tipo_contato
                                              },
                                              ClasseTelefone = new ClasseTelefoneSGF
                                              {
                                                  dc_classe_telefone = x.no_classe
                                              },
                                              PessoaTelefone = new PessoaSGF
                                              {
                                                  no_pessoa = x.no_pessoa
                                              },
                                              id_telefone_principal = x.id_telefone_principal
                                          }).ToList();

                    

                }
                PessoaFisicaSearchUI pessoaFisicaUI = new PessoaFisicaSearchUI();
                pessoaFisicaUI.pessoaFisica = sql;
                if (sql != null && sql.cd_pessoa_cpf != null && sql.cd_pessoa_cpf > 0)
                    pessoaFisicaUI.no_pessoa_cpf = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>() where p.cd_pessoa == sql.cd_pessoa_cpf select p).FirstOrDefault().no_pessoa;

                return pessoaFisicaUI;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //  metodo utilizado para buscar pessoa e endereço principal
        public PessoaFisicaSGF VerificarPessoByCpf(string cpfCgc)
        {
            try{
                var sql = (from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where c.nm_cpf == cpfCgc
                           select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        //verifica pessoa juridica pelo cnpj
        public PessoaJurdicaSearchUI VerificarExisitsEmpresaByCnpjOrcdEmpresa(string cnpj, int? cdEmpresa)
        {
            try
            {
                PessoaJuridicaSGF sql = new PessoaJuridicaSGF();
                var retorno = from p in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                              select p;
                //retorno = Mapper.
                if (!string.IsNullOrEmpty(cnpj))
                    retorno = from pj in retorno
                              where pj.dc_num_cgc == cnpj
                              select pj;
                if (cdEmpresa != null && cdEmpresa > 0)
                    retorno = from p in retorno
                              where p.cd_pessoa == cdEmpresa
                              select p;

                sql = (from p in retorno
                       select new
                       {
                           p.cd_pessoa,
                           p.no_pessoa,
                           p.dt_cadastramento,
                           p.dc_reduzido_pessoa,
                           p.id_pessoa_empresa,
                           p.nm_natureza_pessoa,
                           p.dc_num_pessoa,
                           p.id_exportado,
                           p.txt_obs_pessoa,
                           p.cd_atividade_principal,
                           p.cd_endereco_principal,
                           p.cd_telefone_principal,
                           p.ext_img_pessoa,
                           p.cd_tipo_sociedade,
                           p.dc_num_cgc,
                           p.dt_registro_junta_comercial,
                           p.dc_num_insc_estadual,
                           p.dc_num_insc_municipal,
                           p.dc_num_cnpj_cnab,
                           p.dc_registro_junta_comercial,
                           p.dt_baixa,
                           p.dc_nom_presidente,
                           p.Atividade.no_atividade
                       }).ToList().Select(x => new PessoaJuridicaSGF
                       {
                           cd_pessoa = x.cd_pessoa,
                           no_pessoa = x.no_pessoa,
                           dt_cadastramento = x.dt_cadastramento,
                           dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                           id_pessoa_empresa = x.id_pessoa_empresa,
                           nm_natureza_pessoa = x.nm_natureza_pessoa,
                           dc_num_pessoa = x.dc_num_pessoa,
                           id_exportado = x.id_exportado,
                           txt_obs_pessoa = x.txt_obs_pessoa,
                           cd_atividade_principal = x.cd_atividade_principal,
                           cd_endereco_principal = x.cd_endereco_principal,
                           cd_telefone_principal = x.cd_telefone_principal,
                           ext_img_pessoa = x.ext_img_pessoa,
                           cd_tipo_sociedade = x.cd_tipo_sociedade,
                           dc_num_cgc = x.dc_num_cgc,
                           dt_registro_junta_comercial = x.dt_registro_junta_comercial,
                           dc_num_insc_estadual = x.dc_num_insc_estadual,
                           dc_num_insc_municipal = x.dc_num_insc_municipal,
                           dc_num_cnpj_cnab = x.dc_num_cnpj_cnab,
                           dc_registro_junta_comercial = x.dc_registro_junta_comercial,
                           dt_baixa = x.dt_baixa,
                           dc_nom_presidente = x.dc_nom_presidente,
                           Atividade = new Atividade
                           {
                               no_atividade = x.no_atividade,
                               cd_atividade = x.cd_atividade_principal.HasValue ? (int)x.cd_atividade_principal : 0
                           }
                       }).FirstOrDefault();
                if (sql != null)
                {
                    sql.EnderecoPrincipal = (from e in db.EnderecoSGF
                                                  where e.cd_pessoa == sql.cd_pessoa && sql.cd_endereco_principal == e.cd_endereco
                                                  join p in db.PessoaSGF on e.cd_pessoa equals p.cd_pessoa
                                                  select new
                                                  {
                                                      cd_endereco = e.cd_endereco,
                                                      cd_loc_estado = e.cd_loc_estado,
                                                      cd_loc_cidade = e.cd_loc_cidade,
                                                      cd_loc_bairro = e.cd_loc_bairro,
                                                      cd_loc_logradouro = e.cd_loc_logradouro,
                                                      cd_tipo_endereco = e.cd_tipo_endereco,
                                                      cd_tipo_logradouro = e.cd_tipo_logradouro,
                                                      noLocCidade = e.Cidade.no_localidade,
                                                      noLocBairro = e.Bairro.no_localidade,
                                                      noLocRua = e.Logradouro.no_localidade,
                                                      dc_num_cep = e.Logradouro.dc_num_cep,
                                                      numero = e.dc_num_endereco,
                                                      dc_complemento = e.dc_compl_endereco,
                                                      cd_loc_pais = e.cd_loc_pais
                                                  }).ToList().Select(x => new EnderecoSGF
                                                  {
                                                      cd_endereco = x.cd_endereco,
                                                      cd_loc_estado = x.cd_loc_estado,
                                                      cd_loc_cidade = x.cd_loc_cidade,
                                                      cd_loc_bairro = x.cd_loc_bairro,
                                                      cd_loc_logradouro = x.cd_loc_logradouro,
                                                      cd_tipo_endereco = x.cd_tipo_endereco,
                                                      cd_tipo_logradouro = x.cd_tipo_logradouro,
                                                      noLocCidade = x.noLocCidade,
                                                      noLocBairro = x.noLocBairro,
                                                      noLocRua = x.noLocRua,
                                                      num_cep = x.dc_num_cep,
                                                      dc_num_endereco = x.numero,
                                                      cd_loc_pais = x.cd_loc_pais,
                                                      dc_compl_endereco = x.dc_complemento,
                                                  }).FirstOrDefault();

                    sql.TelefonePessoa = (from telefone in db.TelefoneSGF
                                          where telefone.cd_pessoa == sql.cd_pessoa
                                          select new
                                          {
                                              cd_telefone = telefone.cd_telefone,
                                              cd_operadora = telefone.cd_operadora,
                                              cd_classe_telefone = telefone.cd_classe_telefone,
                                              cd_endereco = telefone.cd_endereco,
                                              cd_tipo_telefone = telefone.cd_tipo_telefone,
                                              dc_fone_mail = telefone.dc_fone_mail,
                                              des_tipo_contato = telefone.TelefoneTipo.no_tipo_telefone,
                                              no_classe = telefone.ClasseTelefone.dc_classe_telefone,
                                              no_pessoa = telefone.PessoaTelefone.no_pessoa,
                                              id_telefone_principal = telefone.id_telefone_principal
                                          }).ToList().Select(x => new TelefoneSGF
                                          {
                                              cd_telefone = x.cd_telefone,
                                              cd_operadora = x.cd_operadora,
                                              cd_classe_telefone = x.cd_classe_telefone,
                                              cd_endereco = x.cd_endereco,
                                              cd_tipo_telefone = x.cd_tipo_telefone,
                                              dc_fone_mail = x.dc_fone_mail,
                                              TelefoneTipo = new TipoTelefoneSGF
                                              {
                                                  no_tipo_telefone = x.des_tipo_contato
                                              },
                                              ClasseTelefone = new ClasseTelefoneSGF
                                              {
                                                  dc_classe_telefone = x.no_classe
                                              },
                                              PessoaTelefone = new PessoaSGF
                                              {
                                                  no_pessoa = x.no_pessoa
                                              },
                                              id_telefone_principal = x.id_telefone_principal
                                          }).ToList();

                    sql.PessoaPaiRelacionamento = (from rp in db.RelacionamentoSGF
                                                   where rp.cd_pessoa_pai == sql.cd_pessoa
                                                   //&& rp.PessoaFilho.nm_natureza_pessoa == (int)Pessoa.TipoPessoa.FISICA
                                                   select new
                                                   {
                                                       cd_pessoa_pai = rp.cd_pessoa_pai,
                                                       cd_pessoa_filho = rp.cd_pessoa_filho,
                                                       cd_papel_filho = rp.cd_papel_filho,
                                                       cd_papel_pai = rp.cd_papel_pai,
                                                       cd_relacionamento = rp.cd_relacionamento,
                                                       nm_tipo_papel_pai = rp.RelacionamentoPaiPapel.nm_tipo_papel,
                                                       no_papel_pai = rp.RelacionamentoPaiPapel.no_papel,
                                                       nm_tipo_papel_filho = rp.RelacionamentoFilhoPapel.nm_tipo_papel,
                                                       no_papel_filho = rp.RelacionamentoFilhoPapel.no_papel,
                                                       no_pessoa_filho = rp.PessoaFilho.no_pessoa,
                                                       nm_natureza_filho = rp.PessoaFilho.nm_natureza_pessoa,
                                                       cd_qualif_relacionamento = rp.cd_qualif_relacionamento,
                                                       desc_qualif_relacionamento = rp.RelacionamentoQualif.no_qualif_relacionamento,
                                                       no_pessoa_Pai = rp.PessoaPai.no_pessoa,
                                                       nm_natureza_Pai = rp.PessoaPai.nm_natureza_pessoa,
                                                       dc_telefone = rp.PessoaPai.Telefone.dc_fone_mail
                                                   }).ToList().Select(x => new RelacionamentoSGF
                                                   {
                                                       cd_pessoa_pai = x.cd_pessoa_pai,
                                                       cd_pessoa_filho = x.cd_pessoa_filho,
                                                       cd_papel_filho = x.cd_papel_filho,
                                                       cd_papel_pai = x.cd_papel_pai,
                                                       cd_relacionamento = x.cd_relacionamento,
                                                       cd_qualif_relacionamento = x.cd_qualif_relacionamento,
                                                       desc_qualif_relacionamento = x.desc_qualif_relacionamento,
                                                       RelacionamentoPaiPapel = new PapelSGF { no_papel = x.no_papel_pai, nm_tipo_papel = x.nm_tipo_papel_pai },
                                                       RelacionamentoFilhoPapel = new PapelSGF { no_papel = x.no_papel_filho, nm_tipo_papel = x.nm_tipo_papel_filho },
                                                       PessoaFilho = new PessoaSGF { cd_pessoa = x.cd_pessoa_filho, no_pessoa = x.no_pessoa_filho, nm_natureza_pessoa = x.nm_natureza_filho },
                                                       PessoaPai = new PessoaSGF { cd_pessoa = x.cd_pessoa_pai, no_pessoa = x.no_pessoa_Pai, nm_natureza_pessoa = x.nm_natureza_Pai, Telefone = new TelefoneSGF { dc_fone_mail = x.dc_telefone } }
                                                   }).ToList();

                    sql.PessoaFilhoRelacionamento = (from rp in db.RelacionamentoSGF
                                                     where rp.cd_pessoa_filho == sql.cd_pessoa
                                                     select new
                                                     {
                                                         cd_pessoa_pai = rp.cd_pessoa_pai,
                                                         cd_pessoa_filho = rp.cd_pessoa_filho,
                                                         cd_papel_filho = rp.cd_papel_filho,
                                                         cd_papel_pai = rp.cd_papel_pai,
                                                         cd_relacionamento = rp.cd_relacionamento,
                                                         nm_tipo_papel_pai = rp.RelacionamentoPaiPapel.nm_tipo_papel,
                                                         no_papel_pai = rp.RelacionamentoPaiPapel.no_papel,
                                                         nm_tipo_papel_filho = rp.RelacionamentoFilhoPapel.nm_tipo_papel,
                                                         no_papel_filho = rp.RelacionamentoFilhoPapel.no_papel,
                                                         no_pessoa_filho = rp.PessoaFilho.no_pessoa,
                                                         nm_natureza_filho = rp.PessoaFilho.nm_natureza_pessoa,
                                                         no_pessoa_Pai = rp.PessoaPai.no_pessoa,
                                                         nm_natureza_Pai = rp.PessoaPai.nm_natureza_pessoa,
                                                         cd_qualif_relacionamento = rp.cd_qualif_relacionamento,
                                                         desc_qualif_relacionamento = rp.RelacionamentoQualif.no_qualif_relacionamento,
                                                         dc_telefone = rp.PessoaFilho.Telefone.dc_fone_mail
                                                     }).ToList().Select(x => new RelacionamentoSGF
                                                     {
                                                         cd_pessoa_pai = x.cd_pessoa_pai,
                                                         cd_pessoa_filho = x.cd_pessoa_filho,
                                                         cd_papel_filho = x.cd_papel_filho,
                                                         cd_papel_pai = x.cd_papel_pai,
                                                         cd_relacionamento = x.cd_relacionamento,
                                                         RelacionamentoPaiPapel = new PapelSGF { no_papel = x.no_papel_pai, nm_tipo_papel = x.nm_tipo_papel_pai },
                                                         RelacionamentoFilhoPapel = new PapelSGF { no_papel = x.no_papel_filho, nm_tipo_papel = x.nm_tipo_papel_filho },
                                                         PessoaFilho = new PessoaSGF { cd_pessoa = x.cd_pessoa_filho, no_pessoa = x.no_pessoa_filho, nm_natureza_pessoa = x.nm_natureza_filho, Telefone = new TelefoneSGF { dc_fone_mail = x.dc_telefone } },
                                                         PessoaPai = new PessoaSGF { cd_pessoa = x.cd_pessoa_pai, no_pessoa = x.no_pessoa_Pai, nm_natureza_pessoa = x.nm_natureza_Pai },
                                                         cd_qualif_relacionamento = x.cd_qualif_relacionamento,
                                                         desc_qualif_relacionamento = x.desc_qualif_relacionamento
                                                     }).ToList();

                    sql.Enderecos = (from e in db.EnderecoSGF
                                          where e.cd_pessoa == sql.cd_pessoa && (sql.cd_endereco_principal == null || e.cd_endereco != sql.cd_endereco_principal)
                                          select new
                                          {
                                              cd_endereco = e.cd_endereco,
                                              cd_loc_estado = e.cd_loc_estado,
                                              cd_loc_cidade = e.cd_loc_cidade,
                                              cd_loc_bairro = e.cd_loc_bairro,
                                              cd_loc_logradouro = e.cd_loc_logradouro,
                                              cd_tipo_endereco = e.cd_tipo_endereco,
                                              cd_tipo_logradouro = e.cd_tipo_logradouro,
                                              noLocCidade = e.Cidade.no_localidade,
                                              noLocBairro = e.Bairro.no_localidade,
                                              noLocRua = e.Logradouro.no_localidade,
                                              dc_num_cep = e.Logradouro.dc_num_cep,
                                              numero = e.dc_num_endereco,
                                              dc_complemento = e.dc_compl_endereco,
                                              cd_loc_pais = e.cd_loc_pais
                                          }).ToList().Select(x => new EnderecoSGF
                                          {
                                              cd_endereco = x.cd_endereco,
                                              cd_loc_estado = x.cd_loc_estado,
                                              cd_loc_cidade = x.cd_loc_cidade,
                                              cd_loc_bairro = x.cd_loc_bairro,
                                              cd_loc_logradouro = x.cd_loc_logradouro,
                                              cd_tipo_endereco = x.cd_tipo_endereco,
                                              cd_tipo_logradouro = x.cd_tipo_logradouro,
                                              noLocCidade = x.noLocCidade,
                                              noLocBairro = x.noLocBairro,
                                              noLocRua = x.noLocRua,
                                              num_cep = x.dc_num_cep,
                                              dc_num_endereco = x.numero,
                                              cd_loc_pais = x.cd_loc_pais,
                                              dc_compl_endereco = x.dc_complemento,
                                          }).ToList();
                }

                PessoaJurdicaSearchUI pessoaJuridicaUI = new PessoaJurdicaSearchUI();
                pessoaJuridicaUI.pessoaJuridica = sql;

                return pessoaJuridicaUI;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        //  metodo utilizado para buscar pessoa Juridica e endereço principal
        public PessoaJuridicaSGF VerificarEmpresaByCnpj(string cnpj)
        {
            try{
                var sql = (from pessoaJuridica in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                           where pessoaJuridica.dc_num_cgc == cnpj
                           select pessoaJuridica).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        // busca pessoa pelo cpf simples.
        public PessoaFisicaSGF ExistsPessoFisicaByCpf(string cpf)
        {
            try{
                var sql = (from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where c.nm_cpf == cpf
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF ExistsPessoByCpf(string cpf)
        {
            try
            {
                var sql = (from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where c.nm_cpf == cpf && c.cd_pessoa_cpf == null
                           select new
                           {
                               c.cd_pessoa,
                               c.no_pessoa
                           }).ToList().Select(x => new PessoaSGF { no_pessoa = x.no_pessoa, cd_pessoa = x.cd_pessoa}).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // busca pessoa juridica simples pelo cnpj.
        public PessoaJuridicaSGF ExistsPessoaJuridicaByCnpj(string Cgc)
        {
            try{
                var sql = (from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                           where c.dc_num_cgc == Cgc
                           select c).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF ExistsPessoaByCNPJ(string Cgc)
        {
            try
            {
                var sql = (from c in db.PessoaSGF.OfType<PessoaJuridicaSGF>()
                           where c.dc_num_cgc == Cgc
                           select new
                           {
                               c.cd_pessoa,
                               c.no_pessoa
                           }).ToList().Select(x => new PessoaSGF { no_pessoa = x.no_pessoa, cd_pessoa = x.cd_pessoa }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // retorna uma pessoa da base
        public PessoaSGF retonaPessoaFirstOrDefault() {
            try{
                var sql = (from pessoa in db.PessoaSGF
                           select pessoa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Boolean PostdeletePessoa(List<PessoaSGF> pessoas)
        {
            try{
                bool retorno = true;
                foreach (PessoaSGF pessoa in pessoas)
                {
                    var pessoaContext = base.findById(pessoa.cd_pessoa, false);
                    retorno = retorno && base.delete(pessoaContext, false);
                }

                //string strPessoas = "";
                //if (pessoas != null && pessoas.Count > 0)
                //    foreach (Pessoa e in pessoas)
                //        strPessoas += e.cd_pessoa + ",";

                //// Remove o último ponto e virgula:
                //if (strPessoas.Length > 0)
                //    strPessoas = strPessoas.Substring(0, strPessoas.Length - 1);

                //int retorno = db.Database.ExecuteSqlCommand("delete from t_pessoa where cd_pessoa in(" + strPessoas + ")");

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaJuridicaSGF postPessoaJuridica(PessoaJuridicaSGF pessoaJuridicaSGF)
        {
            try{
                db.PessoaSGF.Add(pessoaJuridicaSGF);
                db.SaveChanges();
                return pessoaJuridicaSGF;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF postPessoaFisica(PessoaFisicaSGF pessoaFisicaSGF)
        {
            try{
                db.PessoaSGF.Add(pessoaFisicaSGF);
                db.SaveChanges();
                return pessoaFisicaSGF;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaSGF getPessoaImage(int codPessoa)
        {
            try{
                var sql = (from pessoa in db.PessoaSGF
                          where pessoa.cd_pessoa == codPessoa
                          select pessoa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF VerificarPessoByCdPessoa(int cdPessoa)
        {
            try{
                var sql = (from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           where c.cd_pessoa == cdPessoa
                           select c).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF verificarPessoaByEmail(string email)
        {
            try
            {
                var sql = (from pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           join contato in db.TelefoneSGF on pessoa.cd_pessoa equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                           select pessoa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSGF> getPessoasByCds(int[] cdPessoas, int cd_empresa)
        {
            try
            {
                var sql = from p in db.PessoaSGF
                          where cdPessoas.Contains(p.cd_pessoa) && p.PessoaEmpresa.Any(x => x.cd_escola == cd_empresa)
                          select p;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF getPessoaQueUsoCpf(int codPessoa)
        {
            try
            {
                PessoaFisicaSGF sql = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                         where p.cd_pessoa.Equals(codPessoa)
                         select new { 
                             cd_pessoa = p.cd_pessoa,
                             no_pessoa = p.no_pessoa,
                             nm_cpf = p.nm_cpf
                         }).ToList().Select( x => new PessoaFisicaSGF{
                             cd_pessoa = x.cd_pessoa,
                             no_pessoa = x.no_pessoa,
                             nm_cpf = x.nm_cpf
                         }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF verificarPessoaFisicaEmail(string email)
        {
            try
            {
                var sql = (from pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                           join contato in db.TelefoneSGF on pessoa.cd_pessoa equals contato.cd_pessoa
                           where contato.dc_fone_mail == email
                           select pessoa).FirstOrDefault();
                if (sql != null)
                    sql.TelefonePessoa = (from telefone in db.TelefoneSGF
                                          where telefone.cd_pessoa == sql.cd_pessoa
                                          select telefone).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> getPessoasResponsavel(int papel, int cd_escola)
        {
            try
            {
                var sql = from c in db.PessoaSGF.AsNoTracking()
                          where c.PessoaEmpresa.Any(e => e.cd_escola == cd_escola) //&& c.id_pessoa_empresa == false
                          && (c.PessoaFilhoRelacionamento.Where(r => (r.cd_papel_filho == 1 || r.cd_papel_filho == 3 || r.cd_papel_filho == 7) && 
                                                                     (r.cd_papel_filho == papel || papel == 0)).Any())
                          select new PessoaSearchUI
                          {
                              cd_pessoa = c.cd_pessoa,
                              no_pessoa = c.no_pessoa,
                          };

                return sql.OrderBy(x => x.no_pessoa);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<GrupoEstoque> findAllGrupoAtivo(int cd_grupo, bool isMasterGeral)
        {
            try
            {
                IEnumerable<GrupoEstoque> sql = new List<GrupoEstoque>();
                if (isMasterGeral)
                    sql = (from grupos in db.GrupoEstoque
                           where (grupos.id_grupo_estoque_ativo == true
                                  && grupos.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PRIVADA)
                                 || (cd_grupo == 0 || grupos.cd_grupo_estoque == cd_grupo)
                           //orderby grupos.no_grupo_estoque
                           select grupos).Distinct();
                else
                if (cd_grupo == 0)
                    sql = (from grupos in db.GrupoEstoque
                           where grupos.id_grupo_estoque_ativo == true
                                 && grupos.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA
                           //orderby grupos.no_grupo_estoque
                           select grupos).Distinct();
                else
                    sql = (from grupos in db.GrupoEstoque
                           where (grupos.id_grupo_estoque_ativo == true
                                  && grupos.id_categoria_grupo == (int)GrupoEstoque.CategoriaEnum.PUBLICA)
                                 || (cd_grupo == 0 || grupos.cd_grupo_estoque == cd_grupo)
                           //orderby grupos.no_grupo_estoque
                           select grupos).Distinct();

                return sql.OrderBy(x => x.no_grupo_estoque).ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
