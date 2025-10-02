using System;
using System.Linq;
using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Usuario.DataAccess
{
    using Componentes.GenericDataAccess.GenericException;
    using log4net;
    public class UsuarioDataAccess : GenericRepository<UsuarioWebSGF>, IUsuarioDataAccess
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(UsuarioDataAccess));


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<UsuarioWebSGF> GetUsuarioByLogin(string login) {
            try{
                return from usuario in db.UsuarioWebSGF
                       //join pessoa in db.Pessoa
                       //on usuario.cd_pessoa equals pessoa.cd_pessoa
                       where (usuario.no_login == login)
                       select usuario;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> GetUsuarioAuthenticateByLogin(string login) {
            try{
                return (from u in db.UsuarioWebSGF
                       //join pessoa in db.Pessoa
                       //on u.cd_pessoa equals pessoa.cd_pessoa
                       where (u.no_login == login)
                       && u.id_usuario_ativo
                       select new { u.dc_senha_usuario, u.cd_pessoa, u.no_login, u.id_master, u.cd_usuario, u.id_bloqueado, u.id_trocar_senha, u.dt_expiracao_senha, u.id_admin, u.id_administrador }).ToList()
                       .Select(x => new UsuarioWebSGF { dc_senha_usuario = x.dc_senha_usuario,
                                                        cd_pessoa = x.cd_pessoa,
                                                        no_login = x.no_login,
                                                        id_master = x.id_master,
                                                        cd_usuario = x.cd_usuario,
                                                        id_bloqueado = x.id_bloqueado,
                                                        id_trocar_senha = x.id_trocar_senha,
                                                        dt_expiracao_senha = x.dt_expiracao_senha,
                                                        id_admin = x.id_admin,
                                                        id_administrador = x.id_administrador
                       });
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> getUsuarios(int cd_escola)
        {
            try
            {
                return (from ra in db.ReajusteAnual
                       where ra.cd_pessoa_escola == cd_escola
                       select ra.SysUsuario).Distinct();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> GetUsuario()
        {
            try{
                return from usuario in db.UsuarioWebSGF
                       join pessoa in db.PessoaSGF
                       on usuario.cd_pessoa equals pessoa.cd_pessoa
                       //join relacionamento in db.Relacionamento
                       //on pessoa.cd_pessoa equals relacionamento.cd_pessoa_filho
                       select usuario;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verifUsuarioAdmin(int cd_usuario) {
            try {
                bool admin = (from u in db.UsuarioWebSGF
                             where u.cd_usuario == cd_usuario
                             select u.id_admin).FirstOrDefault();
                return admin;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public UsuarioWebSGF getusuarioForEdit(int idUser)
        {
            try
            {
                var sql = (from user in db.UsuarioWebSGF
                           where user.cd_usuario == idUser
                           select user).FirstOrDefault();

                if (sql != null && sql.cd_usuario > 0)
                {
                    sql.Empresas = (from ue in db.UsuarioEmpresaSGF
                                    where ue.cd_usuario == idUser
                                    select new
                                    {
                                        cd_pessoa = ue.cd_pessoa_empresa,
                                        no_pessoa = ue.Escola.no_pessoa,
                                        dc_reduzido_pessoa = ue.Escola.dc_reduzido_pessoa,
                                        cd_usuario_empresa = ue.cd_usuario_empresa,
                                        Grupos = ue.Escola.Grupos
                                    }).ToList().Select(x => new UsuarioEmpresaSGF
                                    {
                                        cd_pessoa = x.cd_pessoa,
                                        no_pessoa = x.no_pessoa,
                                        dc_reduzido_pessoa  = x.dc_reduzido_pessoa,
                                        cd_usuario_empresa = x.cd_usuario_empresa,
                                        Grupos = x.Grupos.ToList()
                                    }).ToList();
                    sql.SysGrupo = (from sgu in db.SysGrupoUsuario
                                  where sgu.cd_usuario == idUser
                                  select new
                                  {
                                      cd_grupo = sgu.cd_grupo,
                                      cd_usuario = sgu.cd_usuario,
                                      cd_grupo_usuario = sgu.cd_grupo_usuario,
                                      no_grupo = sgu.Grupo.no_grupo
                                  }).ToList().Select(x => new SysGrupoUsuario
                                  {
                                      cd_grupo = x.cd_grupo,
                                      cd_usuario = x.cd_usuario,
                                      cd_grupo_usuario = x.cd_grupo_usuario,
                                      no_grupo = x.no_grupo
                                  }).ToList();
                    sql.PessoaFisica = (from p in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                                        where p.cd_pessoa == sql.cd_pessoa
                                        select new
                                        {
                                            cd_pessoa = p.cd_pessoa,
                                            no_pessoa = p.no_pessoa,
                                            nm_cpf = p.nm_cpf,
                                            nm_sexo = p.nm_sexo,
                                            email = p.TelefonePessoa.Where(tel => tel.cd_pessoa == p.cd_pessoa && tel.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).FirstOrDefault().dc_fone_mail
                                        }).ToList().Select(x => new PessoaFisicaSGF
                                        {
                                            cd_pessoa = x.cd_pessoa,
                                            no_pessoa = x.no_pessoa,
                                            nm_cpf = x.nm_cpf,
                                            nm_sexo = x.nm_sexo,
                                            email = x.email
                                        }).FirstOrDefault();
                    sql.qtdPermissao = (from p in db.SysMenu where p.Direitos.Where(x => x.cd_usuario == idUser).Any() select p).Count();
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool DeleteUsuario(int id)
        {
            try{
                bool retorno = false;
                var usuario = db.UsuarioWebSGF.SingleOrDefault(u => u.cd_usuario == id);
                if (usuario != null)
                {
                    db.UsuarioWebSGF.Remove(usuario);
                    retorno = db.SaveChanges() > 0;
                }
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
      
        public UsuarioWebSGF PutUsuarioSenha(UsuarioWebSGF usuario)
        {
            try{
                var entries = db.ChangeTracker.Entries<UsuarioWebSGF>();
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                return usuario;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PessoaFisicaSGF findIdPessoa(int? codPessoa)
        {
            try{
                return (from c in db.PessoaSGF.OfType<PessoaFisicaSGF>()
                        where c.cd_pessoa == codPessoa
                        select c).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool isValidLogin(string login)
        {
            try{
                var retorno = true;
                var usuario = (from user in db.UsuarioWebSGF
                        where user.no_login == login
                        select user).FirstOrDefault();
                if (usuario != null && usuario.cd_usuario > 0)
                    retorno = false;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Boolean DeleteUsuario(List<UsuarioWebSGF> usuariosWebSGF) {
            try {
                bool retorno = true;
                foreach(UsuarioWebSGF usu in usuariosWebSGF) {
                    var usuario = base.findById(usu.cd_usuario, false);
                    retorno = retorno && base.delete(usuario, false);
                }
                //string strUsuarios = "";
                //if (usuariosWebSGF != null && usuariosWebSGF.Count > 0)
                //    foreach(UsuarioWebSGF e in usuariosWebSGF)
                //        strUsuarios += e.cd_usuario + ",";

                //// Remove o último ponto e virgula:
                //if (strUsuarios.Length > 0)
                //    strUsuarios = strUsuarios.Substring(0, strUsuarios.Length - 1);

                //int retorno = db.Database.ExecuteSqlCommand("delete from t_sys_usuario where cd_usuario in(" + strUsuarios + ")");

                //return retorno > 0;
                return retorno;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioUISearch> GetUsuarioSearch(SearchParameters parametros, string descricao, string nome, bool inicio, bool? ativo, bool masterGeral, Int32[] cdEmpresas, int empresa, bool master, bool sysAdmin, bool filtroSysAdmin)
        {
            try {
                IEntitySorter<UsuarioUISearch> sorter = EntitySorter<UsuarioUISearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<UsuarioUISearch> sql;
                IQueryable<UsuarioWebSGF> sqlInicial;

                sqlInicial = from usuario in db.UsuarioWebSGF.AsNoTracking()
                             orderby usuario.no_login
                             select usuario;

                if (!masterGeral)
                {
                    if (master)
                        sqlInicial = from s in sqlInicial
                                     where s.Empresas.Any() && !s.id_admin
                                     select s;
                    else
                    {
                        if (sysAdmin)
                            sqlInicial = from s in sqlInicial
                                         where s.Empresas.Any() && (!s.id_admin && s.id_administrador)
                                         select s;
                        else
                            sqlInicial = from s in sqlInicial
                                         where (s.id_master == false) && (!s.id_admin)
                                         select s;

                    }

                    if (cdEmpresas != null && cdEmpresas.Count() > 0)
                    {
                        sqlInicial = from s in sqlInicial
                                     where
                                     !(from empresas in s.Empresas where !cdEmpresas.Contains(empresas.cd_pessoa_empresa) select empresas.cd_pessoa_empresa).Any()
                                     select s;
                    }
                }
                else
                {
                    sqlInicial = from s in sqlInicial
                                 select s;
                    sqlInicial = from s in sqlInicial
                                 where s.id_admin == filtroSysAdmin
                                 select s;
                }

                if(!String.IsNullOrEmpty(nome))
                    if(inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.StartsWith(nome)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.Contains(nome)
                                     select usuarioWeb;

                if(!String.IsNullOrEmpty(descricao))
                    if(inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.StartsWith(descricao)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.Contains(descricao)
                                     select usuarioWeb;
                if(empresa > 0)
                    sqlInicial = from usuario in sqlInicial
                                 where usuario.Empresas.Where(p => p.cd_pessoa_empresa == empresa).Any()
                                 select usuario;
                if (ativo != null)
                {
                    sqlInicial = from user in sqlInicial
                          where user.id_usuario_ativo == ativo
                          select user;
                }

                var possui_varias_escolas = masterGeral || (cdEmpresas != null && cdEmpresas.Count() > 1);
                //TODO: Verificar performance Deivid
                sql = from usuario in sqlInicial
                      select new UsuarioUISearch {
                          cd_pessoa = usuario.cd_pessoa,
                          cd_usuario = usuario.cd_usuario,
                          id_master = usuario.id_master,
                          no_login = usuario.no_login,
                          id_usuario_ativo = usuario.id_usuario_ativo,
                          no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == usuario.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                          //Empresas = (from e in db.Pessoa.OfType<Empresa>() where e.Usuarios.Where( u => u.cd_usuario == usuario.cd_usuario).Any() select e).ToList(),
                          possui_varias_escolas = possui_varias_escolas,
                          id_admin = usuario.id_admin,
                          id_master_geral = masterGeral,
                          id_administrador = usuario.id_administrador
                      };

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                var retorno = sql.ToList();
                parametros.qtd_limite = limite;
                if (retorno.Count() > 0)
                    foreach (UsuarioUISearch u in retorno)
                        u.Empresas = (from emp in db.PessoaSGF.OfType<Escola>()
                                      where emp.Usuarios.Where(usu => usu.cd_usuario == u.cd_usuario).Any()
                                      select new {
                                          cd_pessoa = emp.cd_pessoa,
                                          cd_empresa = emp.cd_pessoa,
                                          dc_reduzido_pessoa = emp.dc_reduzido_pessoa,
                                          no_pessoa = emp.no_pessoa
                                      }).ToList().Select(x => new Escola{
                                          cd_pessoa = x.cd_pessoa,
                                          cd_empresa = x.cd_empresa,
                                          dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                                          no_pessoa = x.no_pessoa
                                      }).ToList();
                return retorno;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<UsuarioUISearch> getUsuarioSearchFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa)
        {
            try
            {
                IEntitySorter<UsuarioUISearch> sorter = EntitySorter<UsuarioUISearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<UsuarioUISearch> sql;
                IQueryable<UsuarioWebSGF> sqlInicial;

                sqlInicial = from usuario in db.UsuarioWebSGF.AsNoTracking()
                             where !usuario.id_admin && !usuario.id_master && usuario.id_usuario_ativo &&
                                    usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any() 
                             orderby usuario.no_login
                             select usuario;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.StartsWith(nome)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.Contains(nome)
                                     select usuarioWeb;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.StartsWith(descricao)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.Contains(descricao)
                                     select usuarioWeb;

                //TODO: Verificar performance Deivid
                sql = from usuario in sqlInicial
                      select new UsuarioUISearch
                      {
                          cd_pessoa = usuario.cd_pessoa,
                          cd_usuario = usuario.cd_usuario,
                          id_master = usuario.id_master,
                          no_login = usuario.no_login,
                          id_usuario_ativo = usuario.id_usuario_ativo,
                          no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == usuario.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                          //Empresas = (from e in db.Pessoa.OfType<Empresa>() where e.Usuarios.Where( u => u.cd_usuario == usuario.cd_usuario).Any() select e).ToList(),
                          id_admin = usuario.id_admin,
                          id_administrador = usuario.id_administrador
                      };

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                var retorno = sql.ToList();
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public void postUsuarioEmpresa(UsuarioWebSGF usuarioEmpresa) {
            db.SaveChanges();
        }

        public IEnumerable<UsuarioWebSGF> findUsuarioByEmpresaLogin(int cdEmpresa, int codUsuario, bool admGeral, bool? ativo, int? cdGrupo) {
            try {
                var sql = from usuario in db.UsuarioWebSGF
                          where 
                            usuario.Empresas.Where(u => u.cd_pessoa_empresa == cdEmpresa).Any()
                          select usuario;
                if(ativo == true)
                    sql = from s in sql
                          where s.id_usuario_ativo == true
                          select s;
                else
                    sql = from s in sql
                          where s.id_usuario_ativo == true ||
                                (s.id_usuario_ativo == false && (cdGrupo.HasValue && s.Grupos.Where(p => p.cd_grupo == cdGrupo.Value).Any()))
                          select s;

                if(admGeral == false)
                    sql = from s in sql
                          where s.id_master == false || s.cd_usuario == codUsuario
                          select s;
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool VerificarMasterGeral(string login) {
            try {
                return (from usuario in db.UsuarioWebSGF
                        where !usuario.Empresas.Any() && usuario.no_login == login && usuario.id_master
                        select usuario.id_master).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool VerificarMasterGeral(int cdUsuario)
        {
            try
            {
                return (from usuario in db.UsuarioWebSGF
                        where !usuario.Empresas.Any() && usuario.cd_usuario == cdUsuario && usuario.id_master
                        select usuario.id_master).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool VerificarSuperfisor(string login) {
            try {
                return (from usuario in db.UsuarioWebSGF
                        where usuario.Empresas.Any() && usuario.no_login == login && usuario.id_master
                        select usuario.id_master).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public bool verificarSysAdmin(string login)
        {
            try
            {
                return (from usuario in db.UsuarioWebSGF
                        where usuario.no_login == login && usuario.id_admin
                        select usuario.id_admin).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int getIdUsuario(string login) {
            try {
                return (from usuario in db.UsuarioWebSGF
                        where usuario.Empresas.Any() && usuario.no_login == login
                        select usuario.cd_usuario).FirstOrDefault();
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public UsuarioWebSGF firstOrDefault() {
            try {
                var sql = (from usuario in db.UsuarioWebSGF
                           where usuario.Empresas.Any()
                           select usuario).FirstOrDefault();
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }

        }

        public UsuarioWebSGF existsUsuarioByLoginEmail(string login, string email)
        {
            try
            {
                var sql = (from usuario in db.UsuarioWebSGF
                           where usuario.no_login == login
                              && usuario.PessoaFisica.TelefonePessoa.Where(u => u.dc_fone_mail == email && u.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL).Any()
                           select usuario).FirstOrDefault();
                if (sql != null && sql.cd_usuario > 0)
                    sql.EmpresasUsuario = (from emp in db.PessoaSGF.OfType<Escola>()
                                           where emp.Usuarios.Where(usu => usu.cd_usuario == sql.cd_usuario).Any()
                                           select new
                                           {
                                               cd_pessoa = emp.cd_pessoa,
                                               cd_empresa = emp.cd_pessoa,
                                               dc_reduzido_pessoa = emp.dc_reduzido_pessoa,
                                               no_pessoa = emp.no_pessoa
                                           }).ToList().Select(x => new Escola
                                                {
                                                    cd_pessoa = x.cd_pessoa,
                                                    cd_empresa = x.cd_empresa,
                                                    dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                                                    no_pessoa = x.no_pessoa
                                                }).ToList();

                return sql;
            }
            catch (Exception exe)
            {   
                throw new DataAccessException(exe);
            }
        }

        public UsuarioUISearch getUsuarioFromViewGrid(int cd_usuario, int countCdEmpresas, bool masterGeral)
        {
            try
            {
                UsuarioUISearch sql;
                var possui_varias_escolas = masterGeral || (countCdEmpresas > 0);
                var sqlInicial = from usuario in db.UsuarioWebSGF
                                 where usuario.cd_usuario == cd_usuario
                                 select usuario;
                if (countCdEmpresas > 1)
                    sqlInicial = from s in sqlInicial
                                 select s;
                sql = (from usuario in sqlInicial
                       select new UsuarioUISearch
                       {
                           cd_pessoa = usuario.cd_pessoa,
                           cd_usuario = usuario.cd_usuario,
                           id_master = usuario.id_master,
                           no_login = usuario.no_login,
                           id_usuario_ativo = usuario.id_usuario_ativo,
                           no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == usuario.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                           //Empresas = (from e in db.Pessoa.OfType<Empresa>() where e.Usuarios.Where( u => u.cd_usuario == usuario.cd_usuario).Any() select e).ToList(),
                           possui_varias_escolas = possui_varias_escolas,
                           id_admin = usuario.id_admin,
                           id_administrador = usuario.id_administrador
                       }).FirstOrDefault();
                if(sql != null && sql.cd_usuario > 0)
                    sql.Empresas = (from emp in db.PessoaSGF.OfType<Escola>()
                                    where emp.Usuarios.Where(usu => usu.cd_usuario == sql.cd_usuario).Any()
                                    select new
                                    {
                                        cd_pessoa = emp.cd_pessoa,
                                        cd_empresa = emp.cd_empresa,
                                        dc_reduzido_pessoa = emp.dc_reduzido_pessoa,
                                        no_pessoa = emp.no_pessoa
                                    }).ToList().Select(x => new Escola
                                    {
                                        cd_pessoa = x.cd_pessoa,
                                        cd_empresa = x.cd_empresa,
                                        dc_reduzido_pessoa = x.dc_reduzido_pessoa,
                                        no_pessoa = x.no_pessoa
                                    }).ToList();
                return sql;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public bool VerificarSupervisorByEscola(int cd_login, int cd_pessoa_empresa)
        {
            try
            {
                return (from usuario in db.UsuarioWebSGF
                        where usuario.cd_usuario == cd_login
                           && usuario.id_master
                           && usuario.Empresas.Any(e => e.cd_pessoa_empresa == cd_pessoa_empresa)
                        select usuario.id_master).FirstOrDefault();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<UsuarioWebSGF> findUsuarioByGrupo(int cdEmpresa, int cdGrupo)
        {
            try
            {
                var sql = (from usuario in db.UsuarioWebSGF
                           join grupoUsuario in db.SysGrupoUsuario
                           on usuario.cd_usuario equals grupoUsuario.cd_usuario
                           where
                             grupoUsuario.Grupo.cd_pessoa_empresa == cdEmpresa &&
                             grupoUsuario.cd_grupo == cdGrupo
                           select new
                                     {
                                         cd_usuario = usuario.cd_usuario,
                                         no_login = usuario.no_login
                                     }).ToList().Select(x => new UsuarioWebSGF
                                    {
                                        cd_usuario = x.cd_usuario,
                                        no_login = x.no_login
                                    }).ToList();
               
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaExisteSysAdminAtivosEscolas(int[] cdEscolas, string no_login)
        {
            try
            {
                return db.UsuarioEmpresaSGF.Where(u => u.Usuario.id_admin && cdEscolas.Contains(u.cd_pessoa_empresa) && u.Usuario.no_login != no_login && u.Usuario.id_usuario_ativo).Any();
                //return (from ue in db.UsuarioEmpresa.Where(u => u.Usuario.id_admin && cdEscolas.Contains(u.cd_pessoa_empresa)) select ue.cd_usuario).Any();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

         public bool verificaExisteSysAdminAtivosEscolas(string no_login)
        {
            try
            {
                var sql = (from u in db.UsuarioEmpresaSGF
                          where  u.Usuario.id_admin && u.Usuario.no_login != no_login && u.Usuario.id_usuario_ativo &&
                                 db.UsuarioWebSGF.Where(x => x.no_login == no_login && x.Empresas.Where(e => e.cd_pessoa_empresa == u.cd_pessoa_empresa).Any()).Any()
                               select u.cd_usuario).Any();
                //return (from ue in db.UsuarioEmpresa.Where(u => u.Usuario.id_admin && cdEscolas.Contains(u.cd_pessoa_empresa)) select ue.cd_usuario).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<UsuarioWebSGF> getUsuarios(int[] cdUsers)
        {
            try
            {
                var sql = from u in db.UsuarioWebSGF
                          where cdUsers.Contains(u.cd_usuario)
                          select u;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<UsuarioUISearch> getUsuarioSearchGeralFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa)
        {
            try
            {
                IEntitySorter<UsuarioUISearch> sorter = EntitySorter<UsuarioUISearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<UsuarioUISearch> sql;
                IQueryable<UsuarioWebSGF> sqlInicial;

                sqlInicial = from usuario in db.UsuarioWebSGF.AsNoTracking()
                             where  usuario.id_usuario_ativo &&
                                    !usuario.id_admin &&
                                    usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any()
                             orderby usuario.no_login
                             select usuario;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.StartsWith(nome)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.Contains(nome)
                                     select usuarioWeb;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.StartsWith(descricao)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.Contains(descricao)
                                     select usuarioWeb;

                //TODO: Verificar performance Deivid
                sql = from usuario in sqlInicial
                      select new UsuarioUISearch
                      {
                          cd_pessoa = usuario.cd_pessoa,
                          cd_usuario = usuario.cd_usuario,
                          id_master = usuario.id_master,
                          no_login = usuario.no_login,
                          id_usuario_ativo = usuario.id_usuario_ativo,
                          no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == usuario.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                          //Empresas = (from e in db.Pessoa.OfType<Empresa>() where e.Usuarios.Where( u => u.cd_usuario == usuario.cd_usuario).Any() select e).ToList(),
                          id_admin = usuario.id_admin
                      };

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                var retorno = sql.ToList();
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public IEnumerable<UsuarioUISearch> getUsuarioSearchAtendenteFK(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa)
        {
            try
            {
                IEntitySorter<UsuarioUISearch> sorter = EntitySorter<UsuarioUISearch>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<UsuarioUISearch> sql;
                IQueryable<UsuarioWebSGF> sqlInicial;

                sqlInicial = from usuario in db.UsuarioWebSGF.AsNoTracking()
                             where usuario.id_usuario_ativo &&
                                    !usuario.id_admin &&
                                    (usuario.Empresas.Where(ep => ep.cd_pessoa_empresa == cd_empresa).Any() ||
                                    // administrador geral
                                    usuario.id_master && !usuario.Empresas.Any()) &&
                                    usuario.Contratos.Where(c => c.cd_pessoa_escola == cd_empresa).Any()

                             orderby usuario.no_login
                             select usuario;

                if (!String.IsNullOrEmpty(nome))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.StartsWith(nome)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.PessoaFisica.no_pessoa.Contains(nome)
                                     select usuarioWeb;

                if (!String.IsNullOrEmpty(descricao))
                    if (inicio)
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.StartsWith(descricao)
                                     select usuarioWeb;
                    else
                        sqlInicial = from usuarioWeb in sqlInicial
                                     where usuarioWeb.no_login.Contains(descricao)
                                     select usuarioWeb;

                //TODO: Verificar performance Deivid
                sql = from usuario in sqlInicial
                      select new UsuarioUISearch
                      {
                          cd_pessoa = usuario.cd_pessoa,
                          cd_usuario = usuario.cd_usuario,
                          id_master = usuario.id_master,
                          no_login = usuario.no_login,
                          id_usuario_ativo = usuario.id_usuario_ativo,
                          no_pessoa = db.PessoaSGF.OfType<PessoaFisicaSGF>().Where(pf => pf.cd_pessoa == usuario.cd_pessoa).FirstOrDefault<PessoaFisicaSGF>().no_pessoa,
                          id_admin = usuario.id_admin
                      };

                sql = sorter.Sort(sql);
                int limite = sql.Count();
                parametros.ajustaParametrosPesquisa(limite);
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);
                var retorno = sql.ToList();
                parametros.qtd_limite = limite;
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public bool verificarTravaProfessor(int cdPessoa, int cdEscola)
        {
            try
            {
                bool prof = true;
                // Tem que testar se existe pois se não existe retorna false
                bool? profe = (from f in db.FuncionarioSGF.OfType<Professor>()
                              where f.cd_pessoa_funcionario == cdPessoa &&
                              f.cd_pessoa_empresa == cdEscola
                              select f).Any();
                if (!(bool)profe) prof = true;
                if ((bool)profe)
                    prof =    (from f in db.FuncionarioSGF.OfType<Professor>()
                              where f.cd_pessoa_funcionario == cdPessoa &&
                              f.cd_pessoa_empresa == cdEscola
                              select f.id_liberado).FirstOrDefault();

                return prof;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }


        public UsuarioAreaRestritaUI GetEmailUsuario(int usuario)
        {
            try
            {
                var sql = (from u in db.UsuarioWebSGF
                           from t in db.TelefoneSGF
                           where
                               u.cd_pessoa == t.cd_pessoa &&
                               t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL &&
                               u.cd_usuario == usuario
                           select new UsuarioAreaRestritaUI()
                           {
                               email = t.dc_fone_mail,
                               cd_usuario = u.cd_usuario,
                               cd_pessoa = u.cd_pessoa
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
