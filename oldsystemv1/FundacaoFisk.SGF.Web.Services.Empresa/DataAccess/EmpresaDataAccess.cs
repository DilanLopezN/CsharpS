using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using System.Data;
using Componentes.GenericDataAccess.GenericException;
using Componentes.Utils.Messages;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Empresa.DataAccess
{
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;

    public class EmpresaDataAccess : GenericRepository<Escola>, IEmpresaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public Escola getHorarioFunc(int cd_empresa)
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           where empresa.cd_pessoa == cd_empresa
                           select new
                           {
                               hr_final = empresa.hr_final,
                               hr_inicial = empresa.hr_inicial
                           }).ToList().Select(x => new Escola { hr_final = x.hr_final, hr_inicial = x.hr_inicial }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Método utilizado para inserir um pessoa juridica como empresa
        public int insertPessoaWithEmpresa(int cdEmpresa, int? nmIntegracaoCliente, int? nmEmpresaIntegracao, TimeSpan? hrInicial, TimeSpan? hrFinal, DateTime? dt_abertura, DateTime? dt_inicio)
        {

            int retorno;
            try
            {
                retorno = db.Database.ExecuteSqlCommand("insert into t_empresa(cd_pessoa_empresa, nm_cliente_integracao, nm_empresa_integracao," +
                                                          "hr_inicial, hr_final, id_empresa_ativa, dt_abertura, dt_inicio) values({0},{1},{2},{3},{4},{5},{6},{7})",
                                                          +cdEmpresa, nmIntegracaoCliente, nmEmpresaIntegracao, hrInicial, hrFinal, true, dt_abertura, dt_inicio);

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

            return retorno;
        }

        public Escola existsEmpresaWithCNPJ(string cnpj)
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           where empresa.dc_num_cgc == cnpj
                           select empresa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllEmpresa(List<Escola> empresas)
        {
            try
            {
                string strEmpresa = "";
                if (empresas != null && empresas.Count > 0)
                    foreach (Escola e in empresas)
                        strEmpresa += e.cd_pessoa + ",";

                // Remove o último ponto e virgula:
                if (strEmpresa.Length > 0)
                    strEmpresa = strEmpresa.Substring(0, strEmpresa.Length - 1);

                db.Database.ExecuteSqlCommand("delete from t_horario where cd_pessoa_escola in(" + strEmpresa + ")");
                
                int retorno = db.Database.ExecuteSqlCommand("delete from t_empresa where cd_pessoa_empresa in(" + strEmpresa + ")");
                db.Database.ExecuteSqlCommand("delete from t_pessoa_juridica where cd_pessoa_juridica in(" + strEmpresa + ")");
                db.Database.ExecuteSqlCommand("delete from t_pessoa where cd_pessoa in(" + strEmpresa + ")");
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EmpresaSession> findAllEmpresaSession()
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           orderby empresa.dc_reduzido_pessoa
                           select new
                           {
                               cd_pessoa = empresa.cd_pessoa,
                               dc_reduzido_pessoa = empresa.dc_reduzido_pessoa
                           }).ToList().Select(x => new EmpresaSession { 
                               cd_pessoa = x.cd_pessoa, 
                               dc_reduzido_pessoa = x.dc_reduzido_pessoa
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Escola> findAllEmpresa()
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           orderby empresa.dc_reduzido_pessoa
                           select new
                           {
                               cd_pessoa = empresa.cd_pessoa,
                               dc_reduzido_pessoa = empresa.dc_reduzido_pessoa,
                               no_pessoa = empresa.no_pessoa
                           }).ToList().Select(x => new Escola { cd_pessoa = x.cd_pessoa, dc_reduzido_pessoa = x.dc_reduzido_pessoa, no_pessoa = x.no_pessoa });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EscolaApiCyberBdUI getEscola(int cd_escola)
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                        .Include(x => x.EnderecoPrincipal.Cidade)
                        .Include(x=> x.EnderecoPrincipal.Estado.Estado)
                    
                    where empresa.cd_pessoa == cd_escola
                           select new
                           {
                               cd_pessoa = empresa.cd_pessoa,
                               nm_cliente_integracao = (empresa.cd_empresa_coligada == null ? empresa.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == empresa.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),
                               nome_unidade = empresa.dc_reduzido_pessoa,
                               email =  ((from t in db.TelefoneSGF where t.cd_pessoa == empresa.cd_pessoa && t.id_telefone_principal == true && t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select t).FirstOrDefault() != null) ?
                                   (from t in db.TelefoneSGF where t.cd_pessoa == empresa.cd_pessoa && t.id_telefone_principal == true && t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select t).FirstOrDefault().dc_fone_mail : "",
                               cd_cidade = empresa.EnderecoPrincipal.Cidade.cd_localidade,
                               cidade = empresa.EnderecoPrincipal.Cidade.no_localidade,
                               cd_estado = empresa.EnderecoPrincipal.Estado.Estado.cd_localidade_estado,
                               estado = empresa.EnderecoPrincipal.Estado.Estado.sg_estado,
                               escola_ativa = empresa.id_escola_ativa

                           }).ToList().Select(x => new EscolaApiCyberBdUI()
                            {
                                cd_pessoa = x.cd_pessoa,
                                nm_cliente_integracao = x.nm_cliente_integracao,
                                codigo = (x.nm_cliente_integracao != null && x.nm_cliente_integracao > 0)? (int)x.nm_cliente_integracao : 0,
                                nome_unidade = x.nome_unidade,
                                email = x.email,
                                cd_cidade = x.cd_cidade,
                                cidade = x.cidade,
                                cd_estado = x.cd_estado,
                                estado = x.estado,
                                escola_ativa = x.escola_ativa


                            }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EscolaApiAreaRestritaBdUI getEscolaApiAreaRestrita(int cd_escola)
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           where empresa.cd_pessoa == cd_escola
                           select new
                           {
                               cd_pessoa = empresa.cd_pessoa,
                               nm_cliente_integracao = empresa.nm_cliente_integracao,
                               nome_unidade = empresa.dc_reduzido_pessoa,
                               email = ((from t in db.TelefoneSGF where t.cd_pessoa == empresa.cd_pessoa && t.id_telefone_principal == true && t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select t).FirstOrDefault() != null) ?
                                   (from t in db.TelefoneSGF where t.cd_pessoa == empresa.cd_pessoa && t.id_telefone_principal == true && t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL select t).FirstOrDefault().dc_fone_mail : "",

                           }).ToList().Select(x => new EscolaApiAreaRestritaBdUI()
                           {
                               cd_pessoa = x.cd_pessoa,
                               nm_cliente_integracao = x.nm_cliente_integracao,
                               codigo = (x.nm_cliente_integracao != null && x.nm_cliente_integracao > 0) ? (int)x.nm_cliente_integracao : 0,
                               nome_unidade = x.nome_unidade,
                               email = x.email


                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Escola findByIdEmpresa(int id)
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           where empresa.cd_pessoa == id
                           select empresa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EmpresaSession findSessionByIdEmpresa(int id)
        {
            try
            {
                return (from e in db.PessoaSGF.OfType<Escola>()
                        where e.cd_pessoa == id
                        orderby e.dc_reduzido_pessoa//, e.no_pessoa
                        select new { e.hr_final, e.hr_inicial, e.cd_pessoa, e.dc_reduzido_pessoa }).ToList().Select(x => new EmpresaSession { cd_pessoa = x.cd_pessoa, dc_reduzido_pessoa = x.dc_reduzido_pessoa, hr_final = x.hr_final, hr_inicial = x.hr_inicial }).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Escola> findAllEmpresaByUsuario(int codUsuario)
        {
            try
            {
                var sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(e => e.Grupos)
                          where empresa.Usuarios.Where(usu => usu.cd_usuario == codUsuario).Any()
                          orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          select empresa;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Escola> findAllEmpresaAnterior(int codUsuario, bool masterGeral, int cdEmpresaAnt)
        {
            try
            {
                IEnumerable<Escola> sql;
                if (cdEmpresaAnt > 0)
                    sql = from empresa in db.PessoaSGF.OfType<Escola>()
                          where empresa.EmpresasAntigas.Where(e => e.cd_empresa == empresa.cd_empresa).Any() ||
                                empresa.id_escola_ativa == false
                          orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          select empresa;
                else
                    sql = from empresa in db.PessoaSGF.OfType<Escola>()
                          where empresa.id_escola_ativa == false
                          orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          select empresa;

                if (!masterGeral)
                    sql = from empresa in sql
                          where empresa.Usuarios.Where(usu => usu.cd_usuario == codUsuario).Any() &&
                                empresa.id_escola_ativa == false
                          select empresa;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Escola> findAllEmpresaColigada(int cdEmpresa)
        {
            try
            {
                IEnumerable<Escola> sql;
                int? cdEmpresaAnt = (from empresa in db.PessoaSGF.OfType<Escola>() where empresa.cd_pessoa == cdEmpresa select empresa.cd_empresa_coligada).FirstOrDefault();
                sql = from e in db.PessoaSGF.OfType<Escola>()
                          where e.id_escola_ativa == true &&
                                e.cd_pessoa == cdEmpresaAnt
                          orderby e.dc_reduzido_pessoa, e.no_pessoa
                          select e;


                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Escola> findAllEmpresaByUsuario(string login, bool masterGeral)
        {
            try
            {
                IQueryable<Escola> sql;
                if (masterGeral)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          select empresa;
                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any()
                          orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          select empresa;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<Escola> findAllEmpresaByCdUsuario(int cd_usuario, bool masterGeral)
        {
            try
            {
                IQueryable<Escola> sql;
                if (masterGeral)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                            where empresa.id_escola_ativa == true 
                        orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                        select empresa;
                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.cd_usuario == cd_usuario).Any() &&
                                empresa.id_escola_ativa == true
                        orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                        select empresa;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public EmpresaSession findEmpresaSessionById(int cd_empresa, int cd_usuario, bool is_master, bool is_master_geral)
        {
            try
            {
                IQueryable<Escola> retorno;

                if (is_master == false)
                    retorno = from empresa in db.PessoaSGF.OfType<Escola>()
                              where empresa.Usuarios.Where(usu => usu.Usuario.cd_usuario == cd_usuario).Any() &&
                                empresa.cd_pessoa == cd_empresa &&
                                empresa.id_escola_ativa == true
                              select empresa;
                else if(!is_master_geral)
                    retorno = from empresa in db.PessoaSGF.OfType<Escola>()
                              where empresa.Usuarios.Where(usu => usu.Usuario.cd_usuario == cd_usuario).Any() &&
                              empresa.cd_pessoa == cd_empresa
                              select empresa;
                else
                    retorno = from empresa in db.PessoaSGF.OfType<Escola>()
                              where empresa.cd_pessoa == cd_empresa
                              select empresa;
                
                var sql = (from empresa in retorno
                           orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                           select new
                           {
                               empresa.cd_pessoa,
                               empresa.dc_reduzido_pessoa,
                               empresa.hr_inicial,
                               empresa.hr_final
                           }).ToList().Select(x => new EmpresaSession
                           {
                               cd_pessoa = x.cd_pessoa,
                               dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                               hr_inicial = x.hr_inicial,
                               hr_final = x.hr_final
                           });

                return sql.FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EmpresaSession> findEmpresaSessionByLogin(string login, bool ehMaster, bool ativos)
        {
            try
            {
                IQueryable<Escola> retorno;

                if (ehMaster == false)
                    retorno = from empresa in db.PessoaSGF.OfType<Escola>()
                              where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any() &&
                                empresa.id_escola_ativa == ativos
                              select empresa;
                //  select new { empresa.cd_pessoa, empresa.no_pessoa }).ToList().Select(x => new Empresa { cd_pessoa = x.cd_pessoa, no_pessoa = x.no_pessoa });
                else
                    retorno = from empresa in db.PessoaSGF.OfType<Escola>()
                              where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any()
                              select empresa;

                var sql = (from empresa in retorno
                           orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                           select new
                           {
                               empresa.cd_pessoa,
                               empresa.dc_reduzido_pessoa,
                               empresa.hr_inicial,
                               empresa.hr_final
                           }).ToList().Select(x => new EmpresaSession
                           {
                               cd_pessoa = x.cd_pessoa,
                               dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                               hr_inicial = x.hr_inicial,
                               hr_final = x.hr_final
                           });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        // Método usado em testes para buscar uma empresa que possua um usuário master.
        public Escola firstOrDefault()
        {
            try
            {
                var sql = (from empresa in db.PessoaSGF.OfType<Escola>()
                           where empresa.Usuarios.Where(p => p.cd_usuario == empresa.Usuarios.FirstOrDefault(u => u.Usuario.id_master == true).cd_usuario).Any()
                           select empresa).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public List<Escola> findAllEmpresaByArray(int[] cdEmpresas)
        {
            try
            {
                var sql = from c in db.PessoaSGF.OfType<Escola>()
                          where (cdEmpresas).Contains(c.cd_pessoa)
                          select c;

                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool addEmpresaPessoa(PessoaEscola pessoaEmpresa)
        {
            try
            {
                int ret = 0;
                var exist = from pe in db.PessoaEscola
                            where
                              pe.cd_pessoa == pessoaEmpresa.cd_pessoa &&
                              pe.cd_escola == pessoaEmpresa.cd_escola
                            select 1;
                if (exist.Count() <= 0)
                {
                    db.PessoaEscola.Add(pessoaEmpresa);
                    ret = db.SaveChanges();
                }

                return ret > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool addEmpresaPessoaBiblioteca(PessoaEscola pessoaEmpresa)
        {
            try
            {
                
                var exist = from pe in db.PessoaEscola
                    where
                        pe.cd_pessoa == pessoaEmpresa.cd_pessoa &&
                        pe.cd_escola == pessoaEmpresa.cd_escola
                    select 1;

                return exist.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        //Metodo que busca todas as empresas por usuario logado , trazendo os registros paginados.(so retorna 3 valores)
        public IEnumerable<EmpresaUIUsuario> findAllEmpresaByLoginPag(SearchParameters parametros, string login, bool masterGeral, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser)
        {
            try
            {
                IEntitySorter<EmpresaUIUsuario> sorter = EntitySorter<EmpresaUIUsuario>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<EmpresaUIUsuario> sqlEmpresa;
                IQueryable<Escola> sql;

                if (masterGeral == false)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any() && empresa.id_escola_ativa == true
                          select empresa;
                          //orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          //select new EmpresaUIUsuario
                          //{
                          //    cd_pessoa = empresa.cd_pessoa,
                          //    no_pessoa = empresa.no_pessoa,
                          //    dc_reduzido_pessoa = empresa.dc_reduzido_pessoa,
                          //    nm_cpf_cgc = empresa.dc_num_cgc,
                          //    dt_cadastramento = empresa.dt_cadastramento,
                          //    Grupos = empresa.Grupos
                          //};
                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.id_escola_ativa == true
                          select empresa;
                          //orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                          //select new EmpresaUIUsuario
                          //{
                          //    cd_pessoa = empresa.cd_pessoa,
                          //    no_pessoa = empresa.no_pessoa,
                          //    dc_reduzido_pessoa = empresa.dc_reduzido_pessoa,
                          //    nm_cpf_cgc = empresa.dc_num_cgc,
                          //    dt_cadastramento = empresa.dt_cadastramento,
                          //    Grupos = empresa.Grupos
                          //};
                }

                if (editUser)
                    sql = from empresa in sql
                          where !empresas.Contains(empresa.cd_pessoa)
                          select empresa;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.StartsWith(nome)
                              select empresa;
                    else
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.Contains(nome)
                              select empresa;

                if (!string.IsNullOrEmpty(cnpj))
                    sql = from empresa in sql
                          where empresa.dc_num_cgc == cnpj
                          select empresa;

                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.StartsWith(fantasia)
                              select empresa;
                    else
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.Contains(fantasia)
                              select empresa;

                sqlEmpresa = from empresa in sql
                             orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                             select new EmpresaUIUsuario
                             {
                                 cd_pessoa = empresa.cd_pessoa,
                                 no_pessoa = empresa.no_pessoa,
                                 dc_reduzido_pessoa = empresa.dc_reduzido_pessoa,
                                 nm_cpf_cgc = empresa.dc_num_cgc,
                                 dt_cadastramento = empresa.dt_cadastramento,
                                 Grupos = empresa.Grupos
                             };

                sqlEmpresa = sorter.Sort(sqlEmpresa);
               
                int limite = sqlEmpresa.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlEmpresa = sqlEmpresa.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlEmpresa;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EmpresaUIUsuario> findAllEmpresaTransferencia(SearchParameters parametros, int cd_empresa, String nome, string fantasia, string cnpj, bool inicio)
        {
            try
            {
                IEntitySorter<EmpresaUIUsuario> sorter = EntitySorter<EmpresaUIUsuario>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<EmpresaUIUsuario> sqlEmpresa;
                IQueryable<Escola> sql;

                
                
                sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                      where empresa.id_escola_ativa == true
                      select empresa;
               
                

                
                sql = from empresa in sql
                      where empresa.cd_pessoa != cd_empresa
                      select empresa;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.StartsWith(nome)
                              select empresa;
                    else
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.Contains(nome)
                              select empresa;

                if (!string.IsNullOrEmpty(cnpj))
                    sql = from empresa in sql
                          where empresa.dc_num_cgc == cnpj
                          select empresa;

                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.StartsWith(fantasia)
                              select empresa;
                    else
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.Contains(fantasia)
                              select empresa;

                sqlEmpresa = from empresa in sql
                             orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                             select new EmpresaUIUsuario
                             {
                                 cd_pessoa = empresa.cd_pessoa,
                                 no_pessoa = empresa.no_pessoa,
                                 dc_reduzido_pessoa = empresa.dc_reduzido_pessoa,
                                 nm_cpf_cgc = empresa.dc_num_cgc,
                                 dt_cadastramento = empresa.dt_cadastramento,
                                 Grupos = empresa.Grupos
                             };

                sqlEmpresa = sorter.Sort(sqlEmpresa);

                int limite = sqlEmpresa.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlEmpresa = sqlEmpresa.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlEmpresa;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<int> findAllEmpresasByUser(int cd_user)
        {
            try
            {
              var sql = (from ue in db.UsuarioEmpresaSGF
                    where ue.cd_usuario == cd_user
                                select ue.cd_pessoa_empresa).ToList();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchUI> findAllEmpresasByUsuarioPag(SearchParameters parametros, string login, bool masterGeral, List<int> empresas, String nome, string fantasia, string cnpj, bool inicio, bool editUser, int cdEscola)
        {
            try
            {
                IEntitySorter<PessoaSearchUI> sorter = EntitySorter<PessoaSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<PessoaSearchUI> sqlEmpresa;
                IQueryable<Escola> sql;

                if (masterGeral == false)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any() &&
                                empresa.id_escola_ativa == true &&
                                empresa.cd_pessoa != cdEscola
                          select empresa;
                   
                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.id_escola_ativa == true
                          select empresa;
                    
                }

                if (editUser)
                    sql = from empresa in sql
                          where !empresas.Contains(empresa.cd_pessoa)
                          select empresa;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.StartsWith(nome)
                              select empresa;
                    else
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.Contains(nome)
                              select empresa;

                if (!string.IsNullOrEmpty(cnpj))
                    sql = from empresa in sql
                          where empresa.dc_num_cgc == cnpj
                          select empresa;

                if (!string.IsNullOrEmpty(fantasia))
                    if (inicio)
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.StartsWith(fantasia)
                              select empresa;
                    else
                        sql = from empresa in sql
                              where empresa.dc_reduzido_pessoa.Contains(fantasia)
                              select empresa;

                sqlEmpresa = from empresa in sql
                             orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                            select new PessoaSearchUI
                            {
                                cd_pessoa = empresa.cd_pessoa,
                                nm_cpf_cgc = empresa.dc_num_cgc,
                                dt_cadastramento = empresa.dt_cadastramento,
                                no_pessoa = empresa.no_pessoa,
                                dc_reduzido_pessoa = empresa.dc_reduzido_pessoa
                            };

                sqlEmpresa = sorter.Sort(sqlEmpresa);

                int limite = sqlEmpresa.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sqlEmpresa = sqlEmpresa.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sqlEmpresa;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<int> findAllEmpresasByUsuario( string login, bool masterGeral, int cdEscola)
        {
            try
            {
                List<int> sql;

                if (masterGeral == false)
                {
                    sql = (from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any() &&
                                empresa.id_escola_ativa == true &&
                                empresa.cd_pessoa != cdEscola
                          select empresa.cd_pessoa).ToList();

                }
                else
                {
                    sql = (from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.id_escola_ativa == true
                          select empresa.cd_pessoa).ToList();

                }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<PessoaSearchComboUI> findAllEmpresasUsuarioComboFK(string login, bool masterGeral, int cdEscola)
        {
            try
            {
                IQueryable<PessoaSearchComboUI> sqlEmpresa;
                IQueryable<Escola> sql;

                if (masterGeral == false)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any() &&
                                empresa.id_escola_ativa == true //&&
                                //empresa.cd_pessoa != cdEscola
                                orderby empresa.dc_reduzido_pessoa
                          select empresa;

                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.id_escola_ativa == true
                          select empresa;

                }

                

                sqlEmpresa = from empresa in sql
                             orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                             select new PessoaSearchComboUI
                             {
                                 cd_pessoa = empresa.cd_pessoa,
                                 no_pessoa = empresa.no_pessoa,
                                 dc_reduzido_pessoa = empresa.dc_reduzido_pessoa
                             };

                
                return sqlEmpresa;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int findQuantidadeEmpresasVinculadasUsuario(string login, bool masterGeral, int cdEscola)
        {
            try
            {
                IQueryable<PessoaSearchUI> sqlEmpresa;
                IQueryable<Escola> sql;

                if (masterGeral == false)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any() &&
                                empresa.id_escola_ativa == true &&
                                empresa.cd_pessoa != cdEscola
                          select empresa;

                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where empresa.id_escola_ativa == true
                          select empresa;

                }

                sqlEmpresa = from empresa in sql
                             orderby empresa.dc_reduzido_pessoa, empresa.no_pessoa
                             select new PessoaSearchUI
                             {
                                 cd_pessoa = empresa.cd_pessoa,
                                 nm_cpf_cgc = empresa.dc_num_cgc,
                                 dt_cadastramento = empresa.dt_cadastramento,
                                 no_pessoa = empresa.no_pessoa,
                                 dc_reduzido_pessoa = empresa.dc_reduzido_pessoa
                             };

               
                return sqlEmpresa.Count();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string findByNomeEscolaComboReportView(int cdEscolaCombo)
        {
            try
            {
                string sql;
                
                    sql = (from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                        where 
                              empresa.id_escola_ativa == true &&
                              empresa.cd_pessoa == cdEscolaCombo
                        select empresa.dc_reduzido_pessoa).FirstOrDefault();

                    return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Método paginado para trazer todas as empresas do usuario master
        public IEnumerable<EmpresaUI> findAllEmpresaByUsuario(SearchParameters parametros, string login, bool masterGeral, string desc, int cditem)
        {
            try
            {
                IEntitySorter<EmpresaUI> sorter = EntitySorter<EmpresaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<EmpresaUI> sql;
                if (masterGeral == false)
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().AsNoTracking()
                          where empresa.Usuarios.Where(usu => usu.Usuario.no_login == login).Any()
                          && (String.IsNullOrEmpty(desc) || empresa.no_pessoa.Contains(desc))
                          orderby empresa.no_pessoa
                          select new EmpresaUI
                          {
                              cd_pessoa = empresa.cd_pessoa,
                              no_pessoa = empresa.no_pessoa
                          };
                }
                else
                {
                    sql = from empresa in db.PessoaSGF.OfType<Escola>().Include(g => g.Grupos)
                          where (String.IsNullOrEmpty(desc) || empresa.no_pessoa.Contains(desc))
                          orderby empresa.no_pessoa
                          select new EmpresaUI
                          {
                              cd_pessoa = empresa.cd_pessoa,
                              no_pessoa = empresa.no_pessoa
                          };
                }

                sql = sorter.Sort(sql);

                var retorno = from c in sql
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

        public IEnumerable<EmpresaUI> getEmpresaGrupo(int cd_grupo)
        {
            try
            {
                var sql = from empresa in db.PessoaSGF.OfType<Escola>()
                          join grupo in db.SysGrupo on empresa.cd_pessoa equals grupo.cd_pessoa_empresa
                          where grupo.cd_grupo == cd_grupo
                          orderby empresa.no_pessoa
                          select new EmpresaUI
                          {
                              cd_pessoa = empresa.cd_pessoa,
                              no_pessoa = empresa.no_pessoa
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EmpresaUI> getEmpresaHasGrupoMaster(int cd_grupo_master)
        {
            try
            {
                var sql = from empresa in db.PessoaSGF.OfType<Escola>()
                          //where empresa.Grupos.Any(g => g.cd_grupo_master == cd_grupo_master)
                           join grupo in db.SysGrupo on empresa.cd_pessoa equals grupo.cd_pessoa_empresa
                          where grupo.cd_grupo_master == cd_grupo_master
                          orderby empresa.no_pessoa
                          select new EmpresaUI
                          {
                              cd_pessoa = empresa.cd_pessoa,
                              no_pessoa = empresa.dc_reduzido_pessoa,
                              cd_grupo = grupo.cd_grupo
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<EmpresaUI> getEmpresaHasItem(int cd_grupo_master)
        {
            try
            {
                var sql = from empresa in db.PessoaSGF.OfType<Escola>()
                          //where empresa.Grupos.Any(g => g.cd_grupo_master == cd_grupo_master)
                          join grupo in db.SysGrupo on empresa.cd_pessoa equals grupo.cd_pessoa_empresa
                          where grupo.cd_grupo_master == cd_grupo_master
                          orderby empresa.no_pessoa
                          select new EmpresaUI
                          {
                              cd_pessoa = empresa.cd_pessoa,
                              no_pessoa = empresa.no_pessoa,
                              cd_grupo = grupo.cd_grupo
                          };

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getEmpresaPropria(int cd_escola){
            try
            {
                var retorno = (from escola in db.PessoaSGF.OfType<Escola>()
                               where escola.cd_pessoa == cd_escola
                               select escola.id_empresa_propria).FirstOrDefault();

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public string getEmailEscola(int cd_empresa)
        {
            try
            {
                var email = (from telefone in db.TelefoneSGF
                             where telefone.cd_pessoa == cd_empresa &&
                                   telefone.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL &&
                                   telefone.id_telefone_principal == true
                             select telefone.dc_fone_mail).FirstOrDefault();

                return email;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaCoordenadorCyberBdUI findPessoaCoordenadorCyberByCdPessoa(int cd_pessoa, int cd_empresa)
        {
            try
            {
                /*
                 * t_empresa e  
        inner join T_PESSOA p on e.cd_pessoa_empresa = p.cd_pessoa  
        inner join T_USUARIO_EMPRESA ue on e.cd_pessoa_empresa = ue.cd_pessoa_empresa  
        inner join T_SYS_USUARIO u on ue.cd_usuario = u.cd_usuario  
        inner join t_relacionamento r on r.cd_pessoa_filho = u.cd_pessoa
                 */
                var sql = (from e in db.PessoaSGF.OfType<Escola>()
                           join p in db.PessoaSGF on e.cd_pessoa equals p.cd_pessoa
                           from pp in db.PessoaSGF
                           from r in db.RelacionamentoSGF

                           where r.cd_pessoa_filho == pp.cd_pessoa &&
                                 r.cd_pessoa_pai == e.cd_pessoa &&
                                 pp.cd_pessoa == cd_pessoa &&
                                 e.cd_pessoa == cd_empresa &&
                                 r.cd_papel_filho == 14 &&
                                 e.id_escola_ativa == true
                           select new
                           {
                               codigo = pp.cd_pessoa,
                               nome = pp.no_pessoa,
                               email =
                                   ((from t in db.TelefoneSGF
                                     where t.cd_pessoa == cd_pessoa && t.cd_tipo_telefone == 4 &&
                                           t.id_telefone_principal == true
                                     select t).FirstOrDefault() != null
                                       ? (from t in db.TelefoneSGF
                                          where t.cd_pessoa == cd_pessoa && t.cd_tipo_telefone == 4 &&
                                                t.id_telefone_principal == true
                                          select t).FirstOrDefault().dc_fone_mail
                                       : ""),
                               id_unidade = (e.cd_empresa_coligada == null ? e.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == e.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),
                               funcionario_ativo = true,
                               tipoFuncionario = (byte) RelacionamentoDataAccess.TipoRelacionamento.COORDENADOR

                           }).ToList().Select(x => new PessoaCoordenadorCyberBdUI
                {
                    codigo = (x.codigo > 0 ? (int) x.codigo : 0),
                    nome = x.nome,
                    email = x.email,
                    id_unidade = x.id_unidade,
                    pessoa_ativa = x.funcionario_ativo,
                    tipo_funcionario = x.tipoFuncionario

                }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<RelacionamentoSGF> findRelacionamentosCoordenadorByEmpresa( int cd_empresa)
        {
            try
            {
                
                var sql = (from e in db.PessoaSGF.OfType<Escola>()
                           from r in db.RelacionamentoSGF 
                           where 
                                 r.cd_pessoa_pai == cd_empresa &&
                                 r.cd_pessoa_pai == e.cd_pessoa &&
                                 r.cd_papel_filho == 14 &&
                                 e.id_escola_ativa == true 
                           select r);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
