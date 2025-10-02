using System;
using System.Collections.Generic;
using System.Linq;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro;
using System.Data.Entity;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ReajusteAnualDataAccess : GenericRepository<ReajusteAnual>, IReajusteAnualDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<ReajusteAnual> getReajusteAnualSearch(SearchParameters parametros, int cd_empresa, int cd_usuario, int status, DateTime? dtaInicial, DateTime? dtaFinal, bool cadastro, bool vctoInicial, int cd_reajuste_anual)
        {
            try
            {
                IEntitySorter<ReajusteAnual> sorter = EntitySorter<ReajusteAnual>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<ReajusteAnual> sql;
                sql = from r in db.ReajusteAnual.AsNoTracking()
                      select r;

                if (cd_empresa > 0)
                    sql = from r in sql
                          where r.cd_pessoa_escola == cd_empresa
                          select r;

                if (cd_reajuste_anual > 0)
                    sql = from r in sql
                          where r.cd_reajuste_anual == cd_reajuste_anual
                          select r;

                if (cd_usuario > 0)
                    sql = from r in sql
                          where r.cd_usuario == cd_usuario
                          select r;

                if (status > 0)
                    sql = from r in sql
                          where r.id_status_reajuste == status
                          select r;

                if (cadastro)
                {
                    if (dtaInicial.HasValue)
                        sql = from r in sql
                              where r.dh_cadastro_reajuste >= dtaInicial
                              select r;

                    if (dtaFinal.HasValue)
                        sql = from r in sql
                              where r.dh_cadastro_reajuste <= dtaFinal
                              select r;
                }
                if (vctoInicial)
                {
                    if (dtaInicial.HasValue)
                        sql = from r in sql
                              where r.dt_inicial_vencimento >= dtaInicial
                              select r;

                    if (dtaFinal.HasValue)
                        sql = from r in sql
                              where r.dt_final_vencimento <= dtaFinal
                              select r;
                }
                sql = sorter.Sort(sql);
                IEnumerable<ReajusteAnual> sql1 = (from s in sql
                                             select new
                                             {
                                                 cd_reajuste_anual = s.cd_reajuste_anual,
                                                 dh_cadastro_reajuste = s.dh_cadastro_reajuste,
                                                 no_login = s.SysUsuario.no_login,
                                                 pc_reajuste_anual = s.pc_reajuste_anual,
                                                 vl_reajuste_anual = s.vl_reajuste_anual,
                                                 dt_inicial_vencimento = s.dt_inicial_vencimento,
                                                 dt_final_vencimento = s.dt_final_vencimento,
                                                 qtd_turmas = s.ReajustesTurmas.Count(),
                                                 qtd_cursos = s.ReajustesCursos.Count(),
                                                 qtd_titulos = s.ReajustesTitulos.Count(),
                                                 qtd_alunos = s.ReajustesAlunos.Count(),
                                                 id_status_reajuste = s.id_status_reajuste
                                             }).ToList().Select(x => new ReajusteAnual
                                             {
                                                 cd_reajuste_anual = x.cd_reajuste_anual,
                                                 dh_cadastro_reajuste = x.dh_cadastro_reajuste,
                                                 SysUsuario = new UsuarioWebSGF{
                                                     no_login = x.no_login
                                                 },
                                                 pc_reajuste_anual = x.pc_reajuste_anual,
                                                 vl_reajuste_anual = x.vl_reajuste_anual,
                                                 dt_inicial_vencimento = x.dt_inicial_vencimento,
                                                 dt_final_vencimento = x.dt_final_vencimento,
                                                 qtd_turmas = x.qtd_turmas,
                                                 qtd_cursos = x.qtd_cursos,
                                                 qtd_alunos = x.qtd_alunos,
                                                 qtd_titulos = x.qtd_titulos,
                                                 id_status_reajuste = x.id_status_reajuste
                                             });
                int limite = sql1.Count();

                parametros.ajustaParametrosPesquisa(limite);
                sql1 = sql1.Skip(parametros.from).Take(parametros.qtd_limite);
                parametros.qtd_limite = limite;
                return sql1;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public Boolean deleteAllReajustes(List<ReajusteAnual> reajustes)
        {
            try
            {
                foreach (ReajusteAnual reajuste in reajustes)
                {
                    ReajusteAnual reajusteContext = this.findById(reajuste.cd_reajuste_anual, false);
                    this.deleteContext(reajusteContext, false);
                }
                return this.saveChanges(false) > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReajusteAnual getReajusteAnualFull(int cd_reajuste_anual, int cd_empresa)
        {
            try
            {
                ReajusteAnual sql;
                sql = (from r in db.ReajusteAnual.Include(ra => ra.ReajustesAlunos).Include(ra => ra.ReajustesCursos).Include(ra => ra.ReajustesTurmas)
                       where r.cd_reajuste_anual == cd_reajuste_anual 
                       && r.cd_pessoa_escola == cd_empresa
                       select r).FirstOrDefault();
                               
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReajusteAnual getReajusteAnualForEdit(int cd_empresa, int cd_reajuste_anual)
        {
            try
            {
                ReajusteAnual sql;
                sql = (from s in db.ReajusteAnual
                       where s.cd_reajuste_anual == cd_reajuste_anual
                       && s.cd_pessoa_escola == cd_empresa
                       select new
                                             {
                                                 cd_reajuste_anual = s.cd_reajuste_anual,
                                                 id_tipo_reajuste = s.id_tipo_reajuste,
                                                 dh_cadastro_reajuste = s.dh_cadastro_reajuste,
                                                 no_login = s.SysUsuario.no_login,
                                                 pc_reajuste_anual = s.pc_reajuste_anual,
                                                 vl_reajuste_anual = s.vl_reajuste_anual,
                                                 dt_inicial_vencimento = s.dt_inicial_vencimento,
                                                 dt_final_vencimento = s.dt_final_vencimento,
                                                 id_status_reajuste = s.id_status_reajuste,
                                                 s.cd_nome_contrato,
                                                 ReajustesAlunos = s.ReajustesAlunos,
                                                 ReajustesCursos = s.ReajustesCursos,
                                                 ReajustesTurmas = s.ReajustesTurmas,
                                                 ReajustesAlunosAluno = s.ReajustesAlunos.Select(ra => ra.Aluno),
                                                 ReajustesAlunosAlunoPessoaFisica = s.ReajustesAlunos.Select(ra => ra.Aluno.AlunoPessoaFisica),
                                                 ReajustesCursosCurso = s.ReajustesCursos.Select(rc => rc.Curso),
                                                 ReajustesTurmasTurma = s.ReajustesTurmas.Select(rt => rt.Turma)
                                             }).ToList().Select(x => new ReajusteAnual
                                             {
                                                 cd_reajuste_anual = x.cd_reajuste_anual,
                                                 id_tipo_reajuste = x.id_tipo_reajuste,
                                                 dh_cadastro_reajuste = x.dh_cadastro_reajuste,
                                                 cd_nome_contrato = x.cd_nome_contrato,
                                                 SysUsuario = new UsuarioWebSGF
                                                 {
                                                     no_login = x.no_login
                                                 },
                                                 pc_reajuste_anual = x.pc_reajuste_anual,
                                                 vl_reajuste_anual = x.vl_reajuste_anual,
                                                 dt_inicial_vencimento = x.dt_inicial_vencimento,
                                                 dt_final_vencimento = x.dt_final_vencimento,
                                                 id_status_reajuste = x.id_status_reajuste,
                                                 ReajustesAlunos = (from ra in x.ReajustesAlunos
                                                                    select new
                                                                    {
                                                                        cd_reajuste_aluno = ra.cd_reajuste_aluno,
                                                                        cd_aluno = ra.cd_aluno,
                                                                        Aluno = ra.Aluno,
                                                                        AlunoPessoaFisica = ra.Aluno.AlunoPessoaFisica
                                                                    }).ToList().Select(y => new ReajusteAluno
                                                                    {
                                                                        cd_reajuste_aluno = y.cd_reajuste_aluno,
                                                                        cd_aluno = y.cd_aluno,
                                                                        Aluno = new Aluno
                                                                        {
                                                                            AlunoPessoaFisica = new PessoaFisicaSGF{
                                                                                no_pessoa = y.AlunoPessoaFisica.no_pessoa
                                                                            }
                                                                        }
                                                                    }).ToList(),
                                                 ReajustesCursos = (from ra in x.ReajustesCursos
                                                                    select new
                                                                    {
                                                                        cd_reajuste_curso = ra.cd_reajuste_curso,
                                                                        cd_curso = ra.cd_curso,
                                                                        Curso = ra.Curso
                                                                    }).ToList().Select(y => new ReajusteCurso
                                                                    {
                                                                        cd_reajuste_curso = y.cd_reajuste_curso,
                                                                        cd_curso = y.cd_curso,
                                                                        Curso = new Curso
                                                                        {
                                                                            no_curso = y.Curso.no_curso
                                                                        }
                                                                    }).ToList(),
                                                 ReajustesTurmas = (from ra in x.ReajustesTurmas
                                                                    select new
                                                                    {
                                                                        cd_reajuste_turma = ra.cd_reajuste_turma,
                                                                        cd_turma = ra.cd_turma,
                                                                        Turma = ra.Turma
                                                                    }).ToList().Select(y => new ReajusteTurma
                                                                    {
                                                                        cd_reajuste_turma = y.cd_reajuste_turma,
                                                                        cd_turma = y.cd_turma,
                                                                        Turma = new Turma
                                                                        {
                                                                            cd_turma = y.Turma.cd_turma,
                                                                            no_turma = y.Turma.no_turma,
                                                                            no_apelido = y.Turma.no_apelido
                                                                        }
                                                                    }).ToList()
                                             }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaTitulosFechamentoReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            try
            {
                var sql = (from t in db.Titulo
                          where t.cd_pessoa_empresa == cd_empresa && t.ReajustesTitulos.Any(x=> x.cd_reajuste_anual == cd_reajuste_anual) &&
                                (t.id_status_cnab != (int)Titulo.StatusCnabTitulo.INICIAL && t.id_status_cnab != (int)Titulo.StatusCnabTitulo.CONFIRMADO_PEDIDO_BAIXA ||
                                t.BaixaTitulo.Any(x => x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA && x.cd_tipo_liquidacao != (int)TipoLiquidacao.TipoLiqui.MOTIVO_BOLSA))
                          select t.cd_titulo).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<int> getCodigoContratoTitulosReajusteAnual(int cd_empresa, int cd_reajuste_anual)
        {
            try
            {
                List<int> sql = (from t in db.Titulo
                          where t.cd_pessoa_empresa == cd_empresa && t.ReajustesTitulos.Any(x=> x.cd_reajuste_anual == cd_reajuste_anual)
                          select (int)t.cd_origem_titulo).Distinct().ToList();
                return  sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ReajusteAnual getReajusteAnualGridView(int cd_reajuste_anual, int cd_empresa)
        {
            try
            {
                ReajusteAnual retorno = (from r in db.ReajusteAnual
                                         where r.cd_reajuste_anual == cd_reajuste_anual && r.cd_pessoa_escola == cd_empresa
                                         select new
                                         {
                                             cd_reajuste_anual = r.cd_reajuste_anual,
                                             dh_cadastro_reajuste = r.dh_cadastro_reajuste,
                                             no_login = r.SysUsuario.no_login,
                                             pc_reajuste_anual = r.pc_reajuste_anual,
                                             vl_reajuste_anual = r.vl_reajuste_anual,
                                             dt_inicial_vencimento = r.dt_inicial_vencimento,
                                             dt_final_vencimento = r.dt_final_vencimento,
                                             qtd_turmas = r.ReajustesTurmas.Count(),
                                             qtd_cursos = r.ReajustesCursos.Count(),
                                             qtd_titulos = r.ReajustesTitulos.Count(),
                                             qtd_alunos = r.ReajustesAlunos.Count(),
                                             id_status_reajuste = r.id_status_reajuste
                                         }).ToList().Select(x => new ReajusteAnual
                                                   {
                                                       cd_reajuste_anual = x.cd_reajuste_anual,
                                                       dh_cadastro_reajuste = x.dh_cadastro_reajuste,
                                                       SysUsuario = new UsuarioWebSGF
                                                       {
                                                           no_login = x.no_login
                                                       },
                                                       pc_reajuste_anual = x.pc_reajuste_anual,
                                                       vl_reajuste_anual = x.vl_reajuste_anual,
                                                       dt_inicial_vencimento = x.dt_inicial_vencimento,
                                                       dt_final_vencimento = x.dt_final_vencimento,
                                                       qtd_turmas = x.qtd_turmas,
                                                       qtd_cursos = x.qtd_cursos,
                                                       qtd_alunos = x.qtd_alunos,
                                                       qtd_titulos = x.qtd_titulos,
                                                       id_status_reajuste = x.id_status_reajuste
                                                   }).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
