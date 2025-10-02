using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using System.Data.Objects;
using System.Text.RegularExpressions;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class FollowUpDataAccess : GenericRepository<FollowUp>, IFollowUpDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<FollowUp> getFollowUpProspect(int cd_prospect, int cd_escola)
        {
            try
            {
                var sql = (from followUp in db.FollowUp.Include(f => f.FollowUpUsuario)
                          .Include(f => f.Turma)
                          where followUp.cd_prospect == cd_prospect
                          && followUp.FollowUpProspect.cd_pessoa_escola == cd_escola
                          orderby followUp.dt_follow_up descending
                          select new
                          {
                              cd_follow_up = followUp.cd_follow_up,
                              cd_usuario = followUp.cd_usuario,
                              cd_prospect = followUp.cd_prospect,
                              cd_aluno = followUp.cd_aluno,
                              dt_follow_up = followUp.dt_follow_up,
                              dc_assunto = followUp.dc_assunto,
                              cd_escola = followUp.cd_escola,
                              id_follow_resolvido = followUp.id_follow_resolvido,
                              id_follow_lido = followUp.id_follow_lido,
                              id_tipo_follow = followUp.id_tipo_follow,
                              id_usuario_administrador = followUp.id_usuario_administrador,
                              cd_acao_follow_up = followUp.cd_acao_follow_up,
                              cd_usuario_destino = followUp.cd_usuario_destino,
                              dt_proximo_contato = followUp.dt_proximo_contato,
                              cd_follow_up_pai = followUp.cd_follow_up_pai,
                              cd_follow_up_origem = followUp.cd_follow_up_origem,
                              id_tipo_atendimento = followUp.id_tipo_atendimento,
                              id_email_enviado = followUp.id_email_enviado,
                              cd_turma = followUp.cd_turma,
                              no_usuario_origem = followUp.FollowUpUsuario.no_login,
                              no_usuario_destino = followUp.UsuarioDestino.no_login,
                              qtd_lido = followUp.FollowUpUsuarios.Count(),
                              FollowUpUsuario = followUp.FollowUpUsuario,
                              Turma = followUp.Turma

                          }).ToList().Select(x => new FollowUp 
                            {
                              cd_follow_up = x.cd_follow_up,
                              cd_usuario = x.cd_usuario,
                              cd_prospect = x.cd_prospect,
                              cd_aluno = x.cd_aluno,
                              dt_follow_up = x.dt_follow_up,
                              dc_assunto = x.dc_assunto,
                              cd_escola = x.cd_escola,
                              id_follow_resolvido = x.id_follow_resolvido,
                              id_follow_lido = x.id_follow_lido,
                              id_tipo_follow = x.id_tipo_follow,
                              id_usuario_administrador = x.id_usuario_administrador,
                              cd_acao_follow_up = x.cd_acao_follow_up,
                              cd_usuario_destino = x.cd_usuario_destino,
                              dt_proximo_contato = x.dt_proximo_contato,
                              cd_follow_up_pai = x.cd_follow_up_pai,
                              cd_follow_up_origem = x.cd_follow_up_origem,
                              id_tipo_atendimento = x.id_tipo_atendimento,
                              id_email_enviado = x.id_email_enviado,
                              cd_turma = x.cd_turma,
                              no_usuario_origem = x.no_usuario_origem,
                              no_usuario_destino = x.no_usuario_destino,
                              qtd_lido = x.qtd_lido,
                              FollowUpUsuario = x.FollowUpUsuario,
                              Turma = x.Turma
                          });
                
                return sql;
      
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAllFollowUp(List<FollowUp> follows)
        {
            try
            {
                string strFollowUp = "";
                if (follows != null && follows.Count > 0)
                    foreach (FollowUp e in follows)
                        strFollowUp += e.cd_follow_up + ",";
                // Remove o último ponto e virgula:
                if (strFollowUp.Length > 0)
                    strFollowUp = strFollowUp.Substring(0, strFollowUp.Length - 1);
                int retorno = db.Database.ExecuteSqlCommand("delete from t_follow_up where cd_follow_up in(" + strFollowUp + ")");
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FollowUp> getFollowUpByAlunoAllData(int cdAluno, int cd_escola)
        {
            try
            {
                var sql = from followUp in db.FollowUp
                          where followUp.cd_aluno == cdAluno &&
                                followUp.FollowUpAluno.cd_pessoa_escola == cd_escola
                          orderby followUp.dt_follow_up descending
                          select followUp;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeFollowNaoResolvido(int cd_usuario_logado, int cd_escola, bool usuario_login_master)
        {
            try
            {
                var sql = (from flw in db.FollowUp
                           where flw.id_follow_resolvido == false && flw.cd_escola == cd_escola
                            && (
                                //1° Interno
                                (flw.id_tipo_follow != (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL && 
                                        !flw.FollowUpsDestino.Any() && //Não é um follow de resposta
                                        (
                                            (flw.cd_usuario_destino == cd_usuario_logado && flw.cd_escola == cd_escola) //Enviada mensagem dentro da escola para usuário específico
                                            ||
                                            ( //Administrador enviando para outros usuários e para outros administradores
                                                flw.cd_usuario_destino == null && 
                                                flw.FollowUpUsuario.id_master &&
                                                (
                                                    !flw.id_usuario_administrador || (flw.id_usuario_administrador && usuario_login_master)
                                                ) &&
                                                //Follow-up do usuário master da escola
                                                (
                                                    flw.FollowUpEscolas.Any(x => x.cd_escola == cd_escola) 
                                                    || (!flw.FollowUpEscolas.Any() && flw.FollowUpUsuario.Empresas.Any(x => x.cd_pessoa_empresa == cd_escola))
                                                )
                                            )
                                        )
                                ) ||
                                //3° Administrador (master geral)
                                (flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL &&
                                   (!flw.FollowUpEscolas.Any() || flw.FollowUpEscolas.Any(x => x.cd_escola == cd_escola)) &&
                                   (!flw.id_usuario_administrador || (flw.id_usuario_administrador && usuario_login_master))
                                )
                           )
                           select flw).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FollowUp> getFollowUpByAluno(int cdAluno, int cd_escola)
        {
            try
            {
                var sql = (from f in db.FollowUp
                           where f.cd_aluno == cdAluno &&
                                 f.FollowUpAluno.cd_pessoa_escola == cd_escola
                           orderby f.dt_follow_up descending
                           select new
                           {
                               f.cd_follow_up,
                               f.dt_follow_up,
                               f.dt_proximo_contato,
                               f.id_follow_resolvido,
                               f.cd_acao_follow_up,
                               f.FollowUpUsuario.cd_usuario,
                               f.FollowUpUsuario.no_login,
                               f.id_tipo_atendimento,
                               f.dc_assunto,
                               cd_usuario_destino = f.cd_usuario_destino,
                               no_usuario_destino = f.UsuarioDestino.no_login,
                           }).ToList().Select(x => new FollowUp
                                            {
                                                cd_follow_up = x.cd_follow_up,
                                                dt_follow_up = x.dt_follow_up,
                                                FollowUpUsuario = new UsuarioWebSGF { cd_usuario = x.cd_usuario, no_login = x.no_login },
                                                id_tipo_atendimento = x.id_tipo_atendimento,
                                                dc_assunto = x.dc_assunto,
                                                dt_proximo_contato = x.dt_proximo_contato,
                                                id_follow_resolvido = x.id_follow_resolvido,
                                                cd_acao_follow_up = x.cd_acao_follow_up,
                                                cd_usuario_destino = x.cd_usuario_destino,
                                                no_usuario_destino = x.no_usuario_destino
                                            }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FollowUp> getFollowAluno(SearchParameters parametros, int cd_aluno, int cd_escola) {
            try {
                IEntitySorter<FollowUp> sorter = EntitySorter<FollowUp>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection) (int) parametros.sortOrder);
                IQueryable<FollowUp> sql = from f in db.FollowUp.AsNoTracking()
                          where f.cd_aluno == cd_aluno
                          && f.FollowUpAluno.cd_pessoa_escola == cd_escola
                          select f;

                sql = sorter.Sort(sql);

                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);

                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                var retorno = (from f in sql
                               select new {
                                   cd_follow_up = f.cd_follow_up,
                                   dt_follow_up = f.dt_follow_up,
                                   no_login = f.FollowUpUsuario.no_login,
                                   dc_assunto = f.dc_assunto,
                                   dt_proximo_contato = f.dt_proximo_contato,
                                   dc_acao_follow_up = f.AcaoFollowUp.dc_acao_follow_up
                               }).ToList().Select(x => new FollowUp {
                                   dt_follow_up = x.dt_follow_up,
                                   FollowUpUsuario = new UsuarioWebSGF { no_login = x.no_login },
                                   dc_assunto = x.dc_assunto,
                                   dt_proximo_contato = x.dt_proximo_contato,
                                   dc_acao = x.dc_acao_follow_up
                               });

                parametros.qtd_limite = limite;
                return retorno;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FollowUpSearchUI> getFollowUpSearch(SearchParameters parametros, int cdEscola, byte id_tipo_follow, int cd_usuario_org, int cd_usuario_destino, int cd_prospect,
                                                               int cd_acao, int resolvido, int lido, bool data, bool proximo_contato, DateTime? dt_inicial, DateTime? dt_final, bool id_usuario_adm,
                                                               int cd_usuario_logado, int cd_aluno, bool usuario_login_master) {
            try
            {
                IEntitySorter<FollowUpSearchUI> sorter = EntitySorter<FollowUpSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FollowUp> sql;
                sql = from flw in db.FollowUp.AsNoTracking()
                      select flw;
                bool lido_filtro = FollowUp.retornarStatusPesquisa(lido);
                if (id_tipo_follow > 0)
                    switch (id_tipo_follow)
                    {
                        case (int)FollowUp.TipoFollowUp.INTERNO:
                            sql = from flw in sql
                                  //1° interno
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.INTERNO && !flw.FollowUpsDestino.Any() &&
                                        (((flw.cd_usuario == cd_usuario_logado) || (flw.cd_usuario_destino == cd_usuario_logado && flw.cd_escola == cdEscola) && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                        (flw.cd_usuario_destino == null && flw.FollowUpUsuario.id_master &&
                                         ((!flw.id_usuario_administrador) || (flw.id_usuario_administrador && usuario_login_master)) &&
                                        //Follow-up do usuário master da escola
                                         ((flw.FollowUpEscolas.Any(x => x.cd_escola == cdEscola)) || (!flw.FollowUpEscolas.Any() && flw.FollowUpUsuario.Empresas.Any(x => x.cd_pessoa_empresa == cdEscola))) &&
                                          ((flw.cd_usuario == cd_usuario_logado && (lido == 0 || flw.id_follow_lido == lido_filtro)) || 
                                           (lido == 0 || (lido_filtro ? flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any() : !flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any()))))
                                        )
                                  select flw;
                            break;
                        case (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                            sql = from flw in sql
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO &&
                                        flw.cd_escola == cdEscola &&
                                        flw.cd_usuario == cd_usuario_logado || flw.cd_usuario_destino == cd_usuario_logado
                                  select flw;
                            if (lido > 0)
                            {
                                bool bLido = FollowUp.retornarStatusPesquisa(lido);
                                sql = from flw in sql
                                      where flw.id_follow_lido == bLido
                                      select flw;
                            }
                            break;
                        case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                            sql = from flw in sql
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL &&
                                        ((!flw.FollowUpEscolas.Any()) || (!flw.FollowUpEscolas.Any(x => x.cd_escola == cdEscola))) &&
                                        ((!flw.id_usuario_administrador) || (flw.id_usuario_administrador && usuario_login_master))
                                  select flw;
                            if (lido > 0)
                            {
                                bool bLido = FollowUp.retornarStatusPesquisa(lido);
                                sql = from flw in sql
                                      where ((flw.cd_usuario == cd_usuario_logado && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                          (lido_filtro ? flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any() : !flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any()))
                                      select flw;
                            }
                            break;
                    }
                else
                {
                    

                    if (usuario_login_master)
                    {
                        sql = from flw in sql
                              where
                                  (flw.cd_escola == cdEscola && (lido == 0 || flw.id_follow_lido == lido_filtro) &&
                                  ((flw.FollowUpEscolas.Any(x => x.cd_escola == cdEscola)) || (!flw.FollowUpEscolas.Any() && flw.FollowUpUsuario.Empresas.Any(x => x.cd_pessoa_empresa == cdEscola))) &&
                                  ((flw.cd_usuario == cd_usuario_logado && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                   (lido == 0 || (lido_filtro ? flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any() : !flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any()))))
                              select flw;

                    }
                    else
                    {
                        sql = from flw in sql
                              where ((flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.INTERNO && !flw.FollowUpsDestino.Any() &&
                                            (((flw.cd_usuario == cd_usuario_logado) || (flw.cd_usuario_destino == cd_usuario_logado && flw.cd_escola == cdEscola) && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                            (flw.cd_usuario_destino == null && flw.FollowUpUsuario.id_master &&
                                             ((!flw.id_usuario_administrador) || (flw.id_usuario_administrador && usuario_login_master)) &&
                                  //Follow-up do usuário master da escola
                                             ((flw.FollowUpEscolas.Any(x => x.cd_escola == cdEscola)) || (!flw.FollowUpEscolas.Any() && flw.FollowUpUsuario.Empresas.Any(x => x.cd_pessoa_empresa == cdEscola))) &&
                                              ((flw.cd_usuario == cd_usuario_logado && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                               (lido == 0 || (lido_filtro ? flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any() : !flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any()))))
                                            )) ||
                                  //2° Porpsect/Aluno
                                      (flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO && flw.cd_escola == cdEscola && flw.cd_usuario == cd_usuario_logado && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                        ((flw.cd_usuario_destino == cd_usuario_logado && flw.cd_escola == cdEscola) && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                      //3° Administrador
                                      (flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL &&
                                       ((!flw.FollowUpEscolas.Any()) || (!flw.FollowUpEscolas.Any(x => x.cd_escola == cdEscola))) &&
                                       ((!flw.id_usuario_administrador) || (flw.id_usuario_administrador && usuario_login_master)) &&
                                       ((flw.cd_usuario == cd_usuario_logado && (lido == 0 || flw.id_follow_lido == lido_filtro)) ||
                                        (lido_filtro ? flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any() : !flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any())))
                                    )
                              select flw;

                    }
                     
                    
                }
                
                if (resolvido > 0)
                {
                    bool bResolvido = FollowUp.retornarStatusPesquisa(resolvido);
                    sql = from flw in sql
                          where flw.id_follow_resolvido == bResolvido
                          select flw;
                }

                if (cd_usuario_org > 0)
                {
                    if (usuario_login_master)
                    {
                        if (cd_usuario_logado != cd_usuario_org)
                        {
                            sql = from flw in sql
                                where flw.cd_usuario == cd_usuario_org
                                select flw;
                        }
                    }
                    else
                    {
                        sql = from flw in sql
                            where flw.cd_usuario == cd_usuario_org
                            select flw;
                    }
                }


                if (cd_usuario_destino > 0)
                {
                    if (usuario_login_master)
                    {
                        if (cd_usuario_logado != cd_usuario_destino)
                        {
                            sql = from flw in sql
                                  where flw.cd_usuario_destino == cd_usuario_destino
                                  select flw;
                        }
                    }
                    else
                    {
                        sql = from flw in sql
                            where flw.cd_usuario_destino == cd_usuario_destino
                            select flw;
                    }
                }
                    
                if (cd_prospect > 0)
                    sql = from flw in sql
                          where flw.FollowUpProspect.cd_prospect == cd_prospect
                          select flw;
                if (cd_aluno > 0)
                    sql = from flw in sql
                          where flw.FollowUpAluno.cd_aluno == cd_aluno
                          select flw;
                if (cd_acao > 0)
                    sql = from flw in sql
                          where flw.cd_acao_follow_up == cd_acao
                          select flw;
              /*  if(id_usuario_adm)
                    sql = from flw in sql
                          where flw.id_usuario_administrador
                          select flw;*/
                if (dt_inicial.HasValue)
                    if (data)
                        sql = from t in sql
                              where System.Data.Entity.DbFunctions.TruncateTime(t.dt_follow_up) >= dt_inicial
                              select t;
                    else
                        sql = from t in sql
                              where t.dt_proximo_contato >= dt_inicial
                              select t;

                if (dt_final.HasValue)
                    if (data)
                        sql = from t in sql
                              where System.Data.Entity.DbFunctions.TruncateTime(t.dt_follow_up) <= dt_final
                              select t;
                    else
                        if (id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO)
                            sql = from t in sql
                                  where t.dt_proximo_contato <= dt_final
                                  select t;
                IQueryable<FollowUpSearchUI> retorno;
                if (id_tipo_follow != (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO)
                    retorno = from flw in sql
                              select new FollowUpSearchUI
                              {
                                  cd_follow_up = flw.cd_follow_up,
                                  dt_follow_up = flw.dt_follow_up,
                                  dt_proximo_contato = flw.dt_proximo_contato,
                                  id_follow_resolvido = flw.id_follow_resolvido,
                                  id_follow_lido = !flw.id_follow_lido ? flw.FollowUpUsuarios.Where(u => u.cd_usuario == cd_usuario_logado).Any() : flw.id_follow_lido,
                                  id_tipo_follow = flw.id_tipo_follow,
                                  no_usuario_origem = flw.FollowUpUsuario.no_login,
                                  no_usuario_destino = flw.UsuarioDestino.no_login,
                                  no_prospect_aluno = (flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO && flw.FollowUpAluno != null)  ? flw.FollowUpAluno.AlunoPessoaFisica.no_pessoa : (flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO && flw.FollowUpProspect != null) ? flw.FollowUpProspect.PessoaFisica.no_pessoa : null,
                                  id_tipo_follow_pesq = id_tipo_follow
                              };
                else
                    retorno = from flw in sql
                              select new FollowUpSearchUI
                              {
                                  cd_follow_up = flw.cd_follow_up,
                                  dt_follow_up = flw.dt_follow_up,
                                  dt_proximo_contato = flw.dt_proximo_contato,
                                  id_follow_resolvido = flw.id_follow_resolvido,
                                  id_follow_lido = flw.id_follow_lido,
                                  id_tipo_follow = flw.id_tipo_follow,
                                  no_usuario_origem = flw.FollowUpUsuario.no_login,
                                  no_usuario_destino = flw.UsuarioDestino.no_login,
                                  no_prospect_aluno = flw.FollowUpAluno != null ? flw.FollowUpAluno.AlunoPessoaFisica.no_pessoa : flw.FollowUpProspect.PessoaFisica.no_pessoa,
                                  id_tipo_follow_pesq = id_tipo_follow
                              };

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

        public FollowUp getFollowEditView(int cd_follow_up, int cd_escola, int id_tipo_follow)
        {
            try
            {
                FollowUp retorno = new FollowUp();
                IQueryable<FollowUp> sql = from followUp in db.FollowUp
                                           where followUp.cd_follow_up == cd_follow_up
                                           select followUp;
                switch (id_tipo_follow)
                {
                    case (int)FollowUp.TipoFollowUp.INTERNO:
                        retorno = (from followUp in sql
                                   select new
                                   {
                                       cd_follow_up = followUp.cd_follow_up,
                                       cd_follow_up_pai = followUp.cd_follow_up_pai,
                                       cd_follow_up_origem = followUp.cd_follow_up_origem,
                                       cd_usuario = followUp.cd_usuario,
                                       dt_follow_up = followUp.dt_follow_up,
                                       dc_assunto = followUp.dc_assunto,
                                       id_follow_resolvido = followUp.id_follow_resolvido,
                                       id_follow_lido = followUp.id_follow_lido,
                                       id_tipo_follow = followUp.id_tipo_follow,
                                       id_usuario_administrador = followUp.id_usuario_administrador,
                                       cd_usuario_destino = followUp.cd_usuario_destino,
                                       no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                       no_usuario_destino = followUp.UsuarioDestino.no_login,
                                       qtd_lido = followUp.FollowUpUsuarios.Count()
                                   }).ToList().Select(x => new FollowUp
                               {
                                   cd_follow_up = x.cd_follow_up,
                                   cd_follow_up_pai = x.cd_follow_up_pai,
                                   cd_follow_up_origem =x.cd_follow_up_origem,
                                   cd_usuario = x.cd_usuario,
                                   dt_follow_up = x.dt_follow_up,
                                   dc_assunto = x.dc_assunto,
                                   id_follow_resolvido = x.id_follow_resolvido,
                                   id_follow_lido = x.id_follow_lido,
                                   id_tipo_follow = x.id_tipo_follow,
                                   id_usuario_administrador = x.id_usuario_administrador,
                                   cd_usuario_destino = x.cd_usuario_destino,
                                   no_usuario_origem = x.no_usuario_origem,
                                   no_usuario_destino = x.no_usuario_destino,
                                   qtd_lido = x.qtd_lido
                               }).FirstOrDefault();
                        if (retorno != null && retorno.cd_follow_up > 0 && retorno.cd_follow_up_pai > 0)
                            retorno.followUpsTodasRepostas = (from flw in db.FollowUp
                                                              where (flw.cd_follow_up != retorno.cd_follow_up && flw.cd_follow_up_origem == retorno.cd_follow_up_origem)
                                                              select new
                                                              {
                                                                  cd_follow_up = flw.cd_follow_up,
                                                                  cd_usuario = flw.cd_usuario,
                                                                  dt_follow_up = flw.dt_follow_up,
                                                                  dc_assunto = flw.dc_assunto,
                                                                  no_usuario_origem = flw.FollowUpUsuario.no_login
                                                              }).ToList().Select(x => new FollowUp
                                                                                    {
                                                                                        cd_follow_up = x.cd_follow_up,
                                                                                        cd_usuario = x.cd_usuario,
                                                                                        dt_follow_up = x.dt_follow_up,
                                                                                        dc_assunto = x.dc_assunto,
                                                                                        no_usuario_origem = x.no_usuario_origem
                                                                                    }).ToList();
                        break;
                    case (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                        retorno = (from followUp in sql
                                   where followUp.cd_escola == cd_escola
                                   select new
                                   {
                                       cd_follow_up = followUp.cd_follow_up,
                                       cd_usuario = followUp.cd_usuario,
                                       dt_follow_up = followUp.dt_follow_up,
                                       dc_assunto = followUp.dc_assunto,
                                       id_follow_resolvido = followUp.id_follow_resolvido,
                                       id_follow_lido = followUp.id_follow_lido,
                                       id_tipo_follow = followUp.id_tipo_follow,
                                       cd_prospect = followUp.cd_prospect,
                                       cd_aluno = followUp.cd_aluno,
                                       no_aluno = followUp.FollowUpAluno.AlunoPessoaFisica.no_pessoa,
                                       no_prospect = followUp.FollowUpProspect.PessoaFisica.no_pessoa,
                                       dt_proximo_contato = followUp.dt_proximo_contato,
                                       cd_acao_follow_up = followUp.cd_acao_follow_up,
                                       id_tipo_atendimento = followUp.id_tipo_atendimento,
                                       no_acao = followUp.AcaoFollowUp.dc_acao_follow_up,
                                       dc_email = followUp.cd_aluno.HasValue ? db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpAluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail :
                                                                               db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpProspect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                                       dc_telefone = followUp.cd_aluno.HasValue ? followUp.FollowUpAluno.AlunoPessoaFisica.Telefone.dc_fone_mail : followUp.FollowUpProspect.PessoaFisica.Telefone.dc_fone_mail,
                                       cd_turma = followUp.cd_turma,
                                       no_turma = followUp.Turma.no_turma,
                                       horarios = db.Horario.Where(h => h.cd_pessoa_escola == cd_escola &&
                                                                        h.id_origem == (int)Horario.Origem.TURMA && h.cd_registro == ((followUp.cd_turma != null)? followUp.cd_turma : 0) &&
                                                                        h.id_dia_semana == SqlFunctions.DatePart("dw", followUp.dt_proximo_contato)).FirstOrDefault(),
                                       cd_usuario_destino = followUp.cd_usuario_destino,
                                       no_usuario_destino = followUp.UsuarioDestino.no_login,

                                   }).ToList().Select(x => new FollowUp
                                   {
                                       cd_follow_up = x.cd_follow_up,
                                       cd_usuario = x.cd_usuario,
                                       dt_follow_up = x.dt_follow_up,
                                       dc_assunto = x.dc_assunto,
                                       id_follow_resolvido = x.id_follow_resolvido,
                                       id_follow_lido = x.id_follow_lido,
                                       id_tipo_follow = x.id_tipo_follow,
                                       cd_prospect = x.cd_prospect,
                                       cd_aluno = x.cd_aluno,
                                       no_aluno_prospect = string.IsNullOrEmpty(x.no_aluno) ? x.no_prospect : x.no_aluno,
                                       dt_proximo_contato = x.dt_proximo_contato,
                                       cd_acao_follow_up = x.cd_acao_follow_up,
                                       id_tipo_atendimento = x.id_tipo_atendimento,
                                       dc_acao = x.no_acao,
                                       dc_mail_pessoa = x.dc_email,
                                       dc_telefone_pessoa = x.dc_telefone,
                                       cd_turma = x.cd_turma,
                                       no_turma = x.no_turma,
                                       horarios = x.horarios,
                                       no_usuario_destino = x.no_usuario_destino,
                                       cd_usuario_destino = x.cd_usuario_destino,
                                   }).FirstOrDefault();
                        break;
                    case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                        retorno = (from followUp in db.FollowUp
                                   where followUp.cd_follow_up == cd_follow_up
                                   select new
                                   {
                                       cd_follow_up = followUp.cd_follow_up,
                                       cd_usuario = followUp.cd_usuario,
                                       dt_follow_up = followUp.dt_follow_up,
                                       dc_assunto = followUp.dc_assunto,
                                       id_follow_resolvido = followUp.id_follow_resolvido,
                                       id_follow_lido = followUp.id_follow_lido,
                                       id_tipo_follow = followUp.id_tipo_follow,
                                       id_usuario_administrador = followUp.id_usuario_administrador,
                                       no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                       qtd_lido = followUp.FollowUpUsuarios.Count()
                                   }).ToList().Select(x => new FollowUp
                               {
                                   cd_follow_up = x.cd_follow_up,
                                   cd_usuario = x.cd_usuario,
                                   dt_follow_up = x.dt_follow_up,
                                   dc_assunto = x.dc_assunto,
                                   id_follow_resolvido = x.id_follow_resolvido,
                                   id_follow_lido = x.id_follow_lido,
                                   id_tipo_follow = x.id_tipo_follow,
                                   id_usuario_administrador = x.id_usuario_administrador,
                                   no_usuario_origem = x.no_usuario_origem,
                                   qtd_lido = x.qtd_lido
                               }).FirstOrDefault();
                        break;
                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public List<FollowUpRptUI> GetRtpFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola)
        {
            try
            {


                List<FollowUpRptUI> retorno = new List<FollowUpRptUI>();
                //IQueryable<FollowUp> sql = from followUp in db.FollowUp
                //                           where followUp.cd_follow_up == cd_follow_up
                //                           select followUp;
                IQueryable<FollowUp> sql;
                sql = (from flw in db.FollowUp
                        where flw.cd_escola == cd_escola
                    select flw);

                bool lido_filtro = FollowUp.retornarStatusPesquisa(lido);
                if (id_tipo_follow > 0)
                {
                    switch (id_tipo_follow)
                    {
                        case (int)FollowUp.TipoFollowUp.INTERNO:
                            sql = from flw in sql
                                      //1° interno
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.INTERNO
                                  select flw;
                            break;
                        case (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                            sql = from flw in sql
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO
                                  select flw;
                            break;
                        case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                            sql = from flw in sql
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL
                                  select flw;
                            break;
                    }
                }

                if (lido > 0)
                {
                    bool bLido = FollowUp.retornarStatusPesquisa(lido);
                    sql = from flw in sql
                        where flw.id_follow_lido == bLido
                        select flw;
                }

                if (resolvido > 0)
                {
                    bool bResolvido = FollowUp.retornarStatusPesquisa(resolvido);
                    sql = from flw in sql
                          where flw.id_follow_resolvido == bResolvido
                          select flw;
                }

                if (cd_usuario_org > 0)
                {
                    sql = from flw in sql
                          where flw.cd_usuario == cd_usuario_org
                          select flw;
                }


                
                if (dtaIni.HasValue)
                        sql = from t in sql
                              where System.Data.Entity.DbFunctions.TruncateTime(t.dt_follow_up) >= dtaIni
                              select t;
                if (dtaFinal.HasValue)
                        sql = from t in sql
                              where System.Data.Entity.DbFunctions.TruncateTime(t.dt_follow_up) <= dtaFinal
                              select t;





                if (id_tipo_follow > 0)
                {
                    switch (id_tipo_follow)
                    {
                        case (int)FollowUp.TipoFollowUp.INTERNO:
                            retorno = (from followUp in sql
                                       select new
                                       {
                                           cd_follow_up = followUp.cd_follow_up,
                                           cd_follow_up_pai = followUp.cd_follow_up_pai,
                                           cd_follow_up_origem = followUp.cd_follow_up_origem,
                                           dt_follow_up = followUp.dt_follow_up,
                                           dt_proximo_contato = followUp.dt_proximo_contato,
                                           dc_assunto = followUp.dc_assunto,
                                           id_follow_resolvido = followUp.id_follow_resolvido,
                                           id_follow_lido = followUp.id_follow_lido,
                                           id_tipo_follow = followUp.id_tipo_follow,
                                           id_usuario_administrador = followUp.id_usuario_administrador,
                                           cd_usuario_destino = followUp.cd_usuario_destino,
                                           cd_usuario_origem = followUp.cd_usuario,
                                           no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                           no_usuario_destino = followUp.UsuarioDestino.no_login,
                                           dc_acao_follow_up = followUp.AcaoFollowUp.dc_acao_follow_up
                                       }).ToList().Select(x => new FollowUpRptUI()
                                       {
                                           cd_follow_up = x.cd_follow_up,
                                           dt_follow_up = x.dt_follow_up,
                                           id_follow_resolvido = x.id_follow_resolvido,
                                           id_follow_lido = x.id_follow_lido,
                                           id_tipo_follow = x.id_tipo_follow,
                                           cd_usuario_origem = x.cd_usuario_origem,
                                           no_usuario_origem = x.no_usuario_origem,
                                           no_usuario_destino = x.no_usuario_destino,
                                           dc_assunto = !String.IsNullOrEmpty(x.dc_assunto) ? Strip(x.dc_assunto) : "",
                                       }).ToList();
                            
                            break;
                        case (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                            retorno = (from followUp in sql
                                       where followUp.cd_escola == cd_escola
                                       select new
                                       {
                                           cd_follow_up = followUp.cd_follow_up,
                                           cd_usuario_origem = followUp.cd_usuario,
                                           no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                           no_usuario_destino = followUp.UsuarioDestino.no_login,
                                           dt_follow_up = followUp.dt_follow_up,
                                           dc_assunto = followUp.dc_assunto,
                                           id_follow_resolvido = followUp.id_follow_resolvido,
                                           id_follow_lido = followUp.id_follow_lido,
                                           id_tipo_follow = followUp.id_tipo_follow,
                                           cd_prospect = followUp.cd_prospect,
                                           cd_aluno = followUp.cd_aluno,
                                           no_aluno = followUp.FollowUpAluno.AlunoPessoaFisica.no_pessoa,
                                           no_prospect = followUp.FollowUpProspect.PessoaFisica.no_pessoa,
                                           dt_proximo_contato = followUp.dt_proximo_contato,
                                           cd_acao_follow_up = followUp.cd_acao_follow_up,
                                           id_tipo_atendimento = followUp.id_tipo_atendimento,
                                           no_acao = followUp.AcaoFollowUp.dc_acao_follow_up,
                                           dc_email = followUp.cd_aluno.HasValue ? db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpAluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail :
                                                                                   db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpProspect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                                           dc_telefone = followUp.cd_aluno.HasValue ? followUp.FollowUpAluno.AlunoPessoaFisica.Telefone.dc_fone_mail : followUp.FollowUpProspect.PessoaFisica.Telefone.dc_fone_mail,
                                           cd_turma = followUp.cd_turma,
                                           no_turma = followUp.Turma.no_turma,
                                           dc_acao_follow_up = followUp.AcaoFollowUp.dc_acao_follow_up

                                       }).ToList().Select(x => new FollowUpRptUI
                                       {

                                           cd_follow_up = x.cd_follow_up,
                                           dt_follow_up = x.dt_follow_up,
                                           dt_proximo_contato = x.dt_proximo_contato,
                                           id_follow_resolvido = x.id_follow_resolvido,
                                           id_follow_lido = x.id_follow_lido,
                                           id_tipo_follow = x.id_tipo_follow,
                                           cd_usuario_origem = x.cd_usuario_origem,
                                           no_usuario_origem = x.no_usuario_origem,
                                           no_usuario_destino = x.no_usuario_destino,
                                           dc_assunto = !String.IsNullOrEmpty(x.dc_assunto) ? Strip(x.dc_assunto) : "",
                                           dc_acao_follow_up = x.dc_acao_follow_up,

                                           no_prospect_aluno = string.IsNullOrEmpty(x.no_aluno) ? x.no_prospect : x.no_aluno,
                                           email = x.dc_email,
                                           telefone = x.dc_telefone,
                                           //celular

                                           
                                       }).ToList();
                            break;
                        case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                            retorno = (from followUp in sql
                                       select new
                                       {
                                           cd_follow_up = followUp.cd_follow_up,
                                           cd_usuario_origem = followUp.cd_usuario,
                                           dt_follow_up = followUp.dt_follow_up,
                                           dc_assunto = followUp.dc_assunto,
                                           id_follow_resolvido = followUp.id_follow_resolvido,
                                           id_follow_lido = followUp.id_follow_lido,
                                           id_tipo_follow = followUp.id_tipo_follow,
                                           id_usuario_administrador = followUp.id_usuario_administrador,
                                           no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                           no_usuario_destino = followUp.UsuarioDestino.no_login,
                                       }).ToList().Select(x => new FollowUpRptUI
                                       {
                                           cd_follow_up = x.cd_follow_up,
                                           dt_follow_up = x.dt_follow_up,
                                           id_follow_resolvido = x.id_follow_resolvido,
                                           id_follow_lido = x.id_follow_lido,
                                           id_tipo_follow = x.id_tipo_follow,
                                           cd_usuario_origem = x.cd_usuario_origem,
                                           no_usuario_origem = x.no_usuario_origem,
                                           dc_assunto = !String.IsNullOrEmpty(x.dc_assunto) ? Strip(x.dc_assunto) : "",

                                       }).ToList();
                            break;
                    }
                }
                else
                {
                    retorno = (from followUp in sql
                        where followUp.cd_escola == cd_escola
                        select new
                        {
                            cd_follow_up = followUp.cd_follow_up,
                            cd_usuario_origem = followUp.cd_usuario,
                            no_usuario_origem = followUp.FollowUpUsuario.no_login,
                            no_usuario_destino = followUp.UsuarioDestino.no_login,
                            dt_follow_up = followUp.dt_follow_up,
                            dc_assunto = followUp.dc_assunto,
                            id_follow_resolvido = followUp.id_follow_resolvido,
                            id_follow_lido = followUp.id_follow_lido,
                            id_tipo_follow = followUp.id_tipo_follow,
                            cd_prospect = followUp.cd_prospect,
                            cd_aluno = followUp.cd_aluno,
                            no_aluno = followUp.FollowUpAluno.AlunoPessoaFisica.no_pessoa,
                            no_prospect = followUp.FollowUpProspect.PessoaFisica.no_pessoa,
                            dt_proximo_contato = followUp.dt_proximo_contato,
                            cd_acao_follow_up = followUp.cd_acao_follow_up,
                            id_tipo_atendimento = followUp.id_tipo_atendimento,
                            no_acao = followUp.AcaoFollowUp.dc_acao_follow_up,
                            dc_email = followUp.cd_aluno.HasValue ? db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpAluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail : db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int) TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpProspect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                            dc_telefone = followUp.cd_aluno.HasValue ? followUp.FollowUpAluno.AlunoPessoaFisica.Telefone.dc_fone_mail : followUp.FollowUpProspect.PessoaFisica.Telefone.dc_fone_mail,
                            cd_turma = followUp.cd_turma,
                            no_turma = followUp.Turma.no_turma,
                            dc_acao_follow_up = followUp.AcaoFollowUp.dc_acao_follow_up

                        }).ToList().Select(x => new FollowUpRptUI
                    {

                        cd_follow_up = x.cd_follow_up,
                        dt_follow_up = x.dt_follow_up,
                        dt_proximo_contato = x.dt_proximo_contato,
                        id_follow_resolvido = x.id_follow_resolvido,
                        id_follow_lido = x.id_follow_lido,
                        id_tipo_follow = x.id_tipo_follow,
                        cd_usuario_origem = x.cd_usuario_origem,
                        no_usuario_origem = x.no_usuario_origem,
                        no_usuario_destino = x.no_usuario_destino,
                        dc_assunto = !String.IsNullOrEmpty(x.dc_assunto) ? Strip(x.dc_assunto) : "",
                            dc_acao_follow_up = x.dc_acao_follow_up,

                        no_prospect_aluno = string.IsNullOrEmpty(x.no_aluno) ? x.no_prospect : x.no_aluno,
                        email = x.dc_email,
                        telefone = x.dc_telefone,
                        //celular


                    }).ToList();

                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        /**
         *Retira as tags Html de uma string (text não pode ser nulo)
         */
        
        public static string Strip(string text)
        {
            
            string v_sTexto;
            

            v_sTexto = text;

            if (String.IsNullOrEmpty(text))
            {
                return "";
            }else
            {
                v_sTexto = v_sTexto.Replace("<BR><BR><BR>", "<BR>");
                v_sTexto = v_sTexto.Replace("<BR><BR>", "<BR>");
                v_sTexto = v_sTexto.Replace("<BR>", ". ");

                v_sTexto = v_sTexto.Replace("<br><br><br>", "<br>");
                v_sTexto = v_sTexto.Replace("<br><br>", "<br>");
                v_sTexto = v_sTexto.Replace("<br>", ". ");

                v_sTexto = Regex.Replace(v_sTexto, @"<(.|\n)*?>", string.Empty);
                v_sTexto = Regex.Replace(v_sTexto, @" < ", string.Empty);
                v_sTexto = Regex.Replace(v_sTexto, @" > ", string.Empty);
                v_sTexto = Regex.Replace(v_sTexto, @"&gt;", string.Empty);



                v_sTexto = v_sTexto.Replace("..", ". ");
                v_sTexto = v_sTexto.Replace(",.", ", ");
                v_sTexto = v_sTexto.Replace("?.", "? ");
                v_sTexto = v_sTexto.Replace("!.", "! ");
                v_sTexto = v_sTexto.Replace(",.", ", ");
                v_sTexto = v_sTexto.Replace(".. ", ". ");
                v_sTexto = v_sTexto.Replace(",. ", ", ");
                v_sTexto = v_sTexto.Replace("?. ", "? ");
                v_sTexto = v_sTexto.Replace("!. ", "! ");
                v_sTexto = v_sTexto.Replace(",. ", ", ");
                v_sTexto = v_sTexto.Replace(".  ", ". ");
                v_sTexto = v_sTexto.Replace(",  ", ", ");
                v_sTexto = v_sTexto.Replace("?  ", "? ");
                v_sTexto = v_sTexto.Replace("!  ", "! ");
                v_sTexto = v_sTexto.Replace(" . ", ". ");
                v_sTexto = v_sTexto.Replace("&nbsp", "");
                v_sTexto = v_sTexto.TrimEnd();
                v_sTexto = v_sTexto.TrimStart();
            }

            

            return v_sTexto;



        }
        public List<FollowUpRptUI> GetSubRtpFollowUp(int cd_usuario_origem, byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola)
        {
            try
            {
                List<FollowUpRptUI> retorno = new List<FollowUpRptUI>();
                //IQueryable<FollowUp> sql = from followUp in db.FollowUp
                //                           where followUp.cd_follow_up == cd_follow_up
                //                           select followUp;
                IQueryable<FollowUp> sql;
                sql = (from flw in db.FollowUp
                       where flw.cd_escola == cd_escola && 
                             flw.cd_usuario == cd_usuario_origem

                       select flw);

                bool lido_filtro = FollowUp.retornarStatusPesquisa(lido);
                if (id_tipo_follow > 0)
                {
                    switch (id_tipo_follow)
                    {
                        case (int)FollowUp.TipoFollowUp.INTERNO:
                            sql = from flw in sql
                                      //1° interno
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.INTERNO
                                  select flw;
                            break;
                        case (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                            sql = from flw in sql
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO
                                  select flw;
                            break;
                        case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                            sql = from flw in sql
                                  where flw.id_tipo_follow == (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL
                                  select flw;
                            break;
                    }
                }

                if (lido > 0)
                {
                    bool bLido = FollowUp.retornarStatusPesquisa(lido);
                    sql = from flw in sql
                          where flw.id_follow_lido == bLido
                          select flw;
                }

                if (resolvido > 0)
                {
                    bool bResolvido = FollowUp.retornarStatusPesquisa(resolvido);
                    sql = from flw in sql
                          where flw.id_follow_resolvido == bResolvido
                          select flw;
                }

                if (cd_usuario_org > 0)
                {
                    sql = from flw in sql
                          where flw.cd_usuario == cd_usuario_org
                          select flw;
                }



                if (dtaIni.HasValue)
                    sql = from t in sql
                          where System.Data.Entity.DbFunctions.TruncateTime(t.dt_follow_up) >= dtaIni
                          select t;
                if (dtaFinal.HasValue)
                    sql = from t in sql
                          where System.Data.Entity.DbFunctions.TruncateTime(t.dt_follow_up) <= dtaFinal
                          select t;





                if (id_tipo_follow > 0)
                {
                    switch (id_tipo_follow)
                    {
                        case (int)FollowUp.TipoFollowUp.INTERNO:
                            retorno = (from followUp in sql
                                       select new
                                       {
                                           cd_follow_up = followUp.cd_follow_up,
                                           cd_follow_up_pai = followUp.cd_follow_up_pai,
                                           cd_follow_up_origem = followUp.cd_follow_up_origem,
                                           dt_follow_up = followUp.dt_follow_up,
                                           dt_proximo_contato = followUp.dt_proximo_contato,
                                           dc_assunto = followUp.dc_assunto,
                                           id_follow_resolvido = followUp.id_follow_resolvido,
                                           id_follow_lido = followUp.id_follow_lido,
                                           id_tipo_follow = followUp.id_tipo_follow,
                                           id_usuario_administrador = followUp.id_usuario_administrador,
                                           cd_usuario_destino = followUp.cd_usuario_destino,
                                           cd_usuario_origem = followUp.cd_usuario,
                                           no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                           no_usuario_destino = followUp.UsuarioDestino.no_login,
                                           dc_acao_follow_up = followUp.AcaoFollowUp.dc_acao_follow_up
                                       }).ToList().Select(x => new FollowUpRptUI()
                                       {
                                           cd_follow_up = x.cd_follow_up,
                                           dt_follow_up = x.dt_follow_up,
                                           id_follow_resolvido = x.id_follow_resolvido,
                                           id_follow_lido = x.id_follow_lido,
                                           id_tipo_follow = x.id_tipo_follow,
                                           cd_usuario_origem = x.cd_usuario_origem,
                                           no_usuario_origem = x.no_usuario_origem,
                                           no_usuario_destino = x.no_usuario_destino,
                                           dc_assunto = x.dc_assunto,
                                       }).ToList();

                            break;
                        case (int)FollowUp.TipoFollowUp.PROSPECT_ALUNO:
                            retorno = (from followUp in sql
                                       where followUp.cd_escola == cd_escola
                                       select new
                                       {
                                           cd_follow_up = followUp.cd_follow_up,
                                           cd_usuario_origem = followUp.cd_usuario,
                                           no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                           no_usuario_destino = followUp.UsuarioDestino.no_login,
                                           dt_follow_up = followUp.dt_follow_up,
                                           dc_assunto = followUp.dc_assunto,
                                           id_follow_resolvido = followUp.id_follow_resolvido,
                                           id_follow_lido = followUp.id_follow_lido,
                                           id_tipo_follow = followUp.id_tipo_follow,
                                           cd_prospect = followUp.cd_prospect,
                                           cd_aluno = followUp.cd_aluno,
                                           no_aluno = followUp.FollowUpAluno.AlunoPessoaFisica.no_pessoa,
                                           no_prospect = followUp.FollowUpProspect.PessoaFisica.no_pessoa,
                                           dt_proximo_contato = followUp.dt_proximo_contato,
                                           cd_acao_follow_up = followUp.cd_acao_follow_up,
                                           id_tipo_atendimento = followUp.id_tipo_atendimento,
                                           no_acao = followUp.AcaoFollowUp.dc_acao_follow_up,
                                           dc_email = followUp.cd_aluno.HasValue ? db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpAluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail :
                                                                                   db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpProspect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                                           dc_telefone = followUp.cd_aluno.HasValue ? followUp.FollowUpAluno.AlunoPessoaFisica.Telefone.dc_fone_mail : followUp.FollowUpProspect.PessoaFisica.Telefone.dc_fone_mail,
                                           cd_turma = followUp.cd_turma,
                                           no_turma = followUp.Turma.no_turma,
                                           dc_acao_follow_up = followUp.AcaoFollowUp.dc_acao_follow_up

                                       }).ToList().Select(x => new FollowUpRptUI
                                       {

                                           cd_follow_up = x.cd_follow_up,
                                           dt_follow_up = x.dt_follow_up,
                                           dt_proximo_contato = x.dt_proximo_contato,
                                           id_follow_resolvido = x.id_follow_resolvido,
                                           id_follow_lido = x.id_follow_lido,
                                           id_tipo_follow = x.id_tipo_follow,
                                           cd_usuario_origem = x.cd_usuario_origem,
                                           no_usuario_origem = x.no_usuario_origem,
                                           no_usuario_destino = x.no_usuario_destino,
                                           dc_assunto = x.dc_assunto,
                                           dc_acao_follow_up = x.dc_acao_follow_up,

                                           no_prospect_aluno = string.IsNullOrEmpty(x.no_aluno) ? x.no_prospect : x.no_aluno,
                                           email = x.dc_email,
                                           telefone = x.dc_telefone,
                                           //celular


                                       }).ToList();
                            break;
                        case (int)FollowUp.TipoFollowUp.ADMINISTRACAO_GERAL:
                            retorno = (from followUp in sql
                                       select new
                                       {
                                           cd_follow_up = followUp.cd_follow_up,
                                           cd_usuario_origem = followUp.cd_usuario,
                                           dt_follow_up = followUp.dt_follow_up,
                                           dc_assunto = followUp.dc_assunto,
                                           id_follow_resolvido = followUp.id_follow_resolvido,
                                           id_follow_lido = followUp.id_follow_lido,
                                           id_tipo_follow = followUp.id_tipo_follow,
                                           id_usuario_administrador = followUp.id_usuario_administrador,
                                           no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                           no_usuario_destino = followUp.UsuarioDestino.no_login,
                                       }).ToList().Select(x => new FollowUpRptUI
                                       {
                                           cd_follow_up = x.cd_follow_up,
                                           dt_follow_up = x.dt_follow_up,
                                           id_follow_resolvido = x.id_follow_resolvido,
                                           id_follow_lido = x.id_follow_lido,
                                           id_tipo_follow = x.id_tipo_follow,
                                           cd_usuario_origem = x.cd_usuario_origem,
                                           no_usuario_origem = x.no_usuario_origem,
                                           dc_assunto = x.dc_assunto,

                                       }).ToList();
                            break;
                    }
                }
                else
                {
                    retorno = (from followUp in sql
                               where followUp.cd_escola == cd_escola
                               select new
                               {
                                   cd_follow_up = followUp.cd_follow_up,
                                   cd_usuario_origem = followUp.cd_usuario,
                                   no_usuario_origem = followUp.FollowUpUsuario.no_login,
                                   no_usuario_destino = followUp.UsuarioDestino.no_login,
                                   dt_follow_up = followUp.dt_follow_up,
                                   dc_assunto = followUp.dc_assunto,
                                   id_follow_resolvido = followUp.id_follow_resolvido,
                                   id_follow_lido = followUp.id_follow_lido,
                                   id_tipo_follow = followUp.id_tipo_follow,
                                   cd_prospect = followUp.cd_prospect,
                                   cd_aluno = followUp.cd_aluno,
                                   no_aluno = followUp.FollowUpAluno.AlunoPessoaFisica.no_pessoa,
                                   no_prospect = followUp.FollowUpProspect.PessoaFisica.no_pessoa,
                                   dt_proximo_contato = followUp.dt_proximo_contato,
                                   cd_acao_follow_up = followUp.cd_acao_follow_up,
                                   id_tipo_atendimento = followUp.id_tipo_atendimento,
                                   no_acao = followUp.AcaoFollowUp.dc_acao_follow_up,
                                   dc_email = followUp.cd_aluno.HasValue ? db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpAluno.cd_pessoa_aluno).FirstOrDefault().dc_fone_mail : db.TelefoneSGF.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.EMAIL && t.cd_pessoa == followUp.FollowUpProspect.cd_pessoa_fisica).FirstOrDefault().dc_fone_mail,
                                   dc_telefone = followUp.cd_aluno.HasValue ? followUp.FollowUpAluno.AlunoPessoaFisica.Telefone.dc_fone_mail : followUp.FollowUpProspect.PessoaFisica.Telefone.dc_fone_mail,
                                   cd_turma = followUp.cd_turma,
                                   no_turma = followUp.Turma.no_turma,
                                   dc_acao_follow_up = followUp.AcaoFollowUp.dc_acao_follow_up

                               }).ToList().Select(x => new FollowUpRptUI
                               {

                                   cd_follow_up = x.cd_follow_up,
                                   dt_follow_up = x.dt_follow_up,
                                   dt_proximo_contato = x.dt_proximo_contato,
                                   id_follow_resolvido = x.id_follow_resolvido,
                                   id_follow_lido = x.id_follow_lido,
                                   id_tipo_follow = x.id_tipo_follow,
                                   cd_usuario_origem = x.cd_usuario_origem,
                                   no_usuario_origem = x.no_usuario_origem,
                                   no_usuario_destino = x.no_usuario_destino,
                                   dc_assunto = x.dc_assunto,
                                   dc_acao_follow_up = x.dc_acao_follow_up,

                                   no_prospect_aluno = string.IsNullOrEmpty(x.no_aluno) ? x.no_prospect : x.no_aluno,
                                   email = x.dc_email,
                                   telefone = x.dc_telefone,
                                   //celular


                               }).ToList();

                }

                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FollowUpSearchUI getFollowUpGrid(int? cd_pessoa_empresa, int cd_follow_up)
        {
            try
            {
                FollowUpSearchUI sql = (from flw in db.FollowUp
                               where  flw.cd_follow_up == cd_follow_up &&
                               (cd_pessoa_empresa == null || flw.cd_escola == cd_pessoa_empresa)
                              select new FollowUpSearchUI
                              {
                                  cd_follow_up = flw.cd_follow_up,
                                  dt_follow_up = flw.dt_follow_up,
                                  dt_proximo_contato = flw.dt_proximo_contato,
                                  id_follow_resolvido = flw.id_follow_resolvido,
                                  id_follow_lido = flw.id_follow_lido,
                                  id_tipo_follow = flw.id_tipo_follow,
                                  no_usuario_origem = flw.FollowUpUsuario.no_login,
                                  no_usuario_destino = flw.UsuarioDestino.no_login
                              }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FollowUp> getFollowUps(List<int> codigosFollowUps, int cd_usuario_origem)
        {
            try
            {
                var sql = from flw in db.FollowUp
                          where codigosFollowUps.Contains(flw.cd_follow_up) && flw.cd_usuario == cd_usuario_origem
                          select flw;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeRespostaFollowUp(int cd_follow_up)
        {
            try
            {
                bool retonro = (from flw in db.FollowUp
                                        where flw.cd_follow_up_origem == cd_follow_up
                                        select flw.cd_follow_up).Any();
                return retonro;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaExisteFollowUPOutroUsuario(List<int> codigosFollowUps, int cd_usuario_origem)
        {
            try
            {
                return (from flw in db.FollowUp
                          where codigosFollowUps.Contains(flw.cd_follow_up) && flw.cd_usuario != cd_usuario_origem
                          select flw.cd_follow_up).Any();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
