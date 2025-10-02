using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System.Data.Entity;
using Componentes.Utils;
using Componentes.GenericDataAccess;
using FundacaoFisk.SGF.Utils;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class ProfessorDataAccess : GenericRepository<Professor>, IProfessorDataAccess
    {
        public enum TipoConsultaProfessorEnum
        {
            HAS_ATIVO = 0,
            HAS_TURMA = 1,
            DADOS_FUNC_EDIT = 2,
            LOAD_PROFE = 3,
            LOAD_PROF_HABILPROF = 4,
            DADOS_PROF_EDIT = 5,
            PROF_COMISSIONADOS = 6,
            HAS_DESISTENCIA = 7,
            HAS_AVALIACAO = 8,
            SEM_DIARIO = 9,
            CARGA_HORARIA = 10
        }


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<ProfessorUI> getProfessorReturnProfUI(TipoConsultaProfessorEnum hasDependente, int cdEscola, int? cdProfessor)
        {
            try
            {
                IQueryable<ProfessorUI> sql = null;
                switch (hasDependente)
                {
                    case TipoConsultaProfessorEnum.HAS_ATIVO:
                        sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                              join pessoa in db.PessoaSGF
                              on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                              where professor.id_professor == true && professor.cd_pessoa_empresa == cdEscola && professor.id_funcionario_ativo == true
                              orderby professor.FuncionarioPessoaFisica.no_pessoa
                              select new ProfessorUI
                              {
                                  cd_pessoa = professor.cd_funcionario,
                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa != null ? professor.FuncionarioPessoaFisica.dc_reduzido_pessoa : professor.FuncionarioPessoaFisica.no_pessoa
                              };
                        break;
                    case TipoConsultaProfessorEnum.HAS_TURMA:
                        sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                              //join pessoa in db.PessoaSGF
                             // on professor.cd_funcionario equals pessoa.cd_pessoa
                              where professor.id_professor == true && professor.cd_pessoa_empresa == cdEscola &&
                              professor.ProfessorTurma.Where(pt => pt.cd_professor == professor.cd_funcionario).Any()
                              //(from t in professor.ProfessorTurma select t.cd_professor).Contains(professor.cd_funcionario)
                              orderby professor.FuncionarioPessoaFisica.no_pessoa
                              select new ProfessorUI
                              {
                                  cd_pessoa = professor.cd_funcionario,
                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa != null ? professor.FuncionarioPessoaFisica.dc_reduzido_pessoa : professor.FuncionarioPessoaFisica.no_pessoa
                              };
                        break;
                    case TipoConsultaProfessorEnum.HAS_DESISTENCIA:
                        sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                              where professor.cd_pessoa_empresa == cdEscola &&
                              professor.ProfessorTurma.Where(pt => pt.cd_professor == professor.cd_funcionario && pt.Turma.TurmaAluno.Any(at=> at.Desistencia.Any())).Any()
                              orderby professor.FuncionarioPessoaFisica.no_pessoa
                              select new ProfessorUI
                              {
                                  cd_pessoa = professor.cd_funcionario,
                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa != null ? professor.FuncionarioPessoaFisica.dc_reduzido_pessoa : professor.FuncionarioPessoaFisica.no_pessoa
                              };
                        break;
                    case TipoConsultaProfessorEnum.HAS_AVALIACAO:
                        sql = from professor in db.FuncionarioSGF
                              where professor.cd_pessoa_empresa == cdEscola &&
                              professor.AvaliacaoTurma.Where(pt => pt.cd_funcionario == professor.cd_funcionario).Any()
                              orderby professor.FuncionarioPessoaFisica.no_pessoa
                              select new ProfessorUI
                              {
                                  cd_pessoa = professor.cd_funcionario,
                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa != null ? professor.FuncionarioPessoaFisica.dc_reduzido_pessoa : professor.FuncionarioPessoaFisica.no_pessoa
                              };
                        break;
                    case TipoConsultaProfessorEnum.SEM_DIARIO:
                        sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                              where professor.id_professor == true && (professor.cd_pessoa_empresa == cdEscola || cdEscola == 0) &&
                              db.vi_raf_sem_diario.Where(v => v.cd_professor == professor.cd_funcionario && (v.cd_pessoa_escola == cdEscola || cdEscola == 0)).Any()
                              orderby professor.FuncionarioPessoaFisica.no_pessoa
                              select new ProfessorUI
                              {
                                  cd_pessoa = professor.cd_funcionario,
                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa != null ? professor.FuncionarioPessoaFisica.dc_reduzido_pessoa : professor.FuncionarioPessoaFisica.no_pessoa
                              };
                        break;
                    case TipoConsultaProfessorEnum.CARGA_HORARIA:
                        sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                              where professor.id_professor == true && (professor.cd_pessoa_empresa == cdEscola || cdEscola == 0) &&
                              db.vi_carga_horaria.Where(v => db.DiarioAula.Where(d=> d.cd_turma == v.cd_turma && d.cd_professor == professor.cd_funcionario).Any() && (v.cd_escola == cdEscola || cdEscola == 0)).Any() 
                              orderby professor.FuncionarioPessoaFisica.no_pessoa
                              select new ProfessorUI
                              {
                                  cd_pessoa = professor.cd_funcionario,
                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa != null ? professor.FuncionarioPessoaFisica.dc_reduzido_pessoa : professor.FuncionarioPessoaFisica.no_pessoa
                              };
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProfessorUI> getProfessorTurma(int cd_escola, int cd_turma) {
            try {
                IQueryable<ProfessorUI> sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                                              where professor.id_professor == true && //LBMprofessor.cd_pessoa_empresa == cd_escola &&
                                              //professor.id_funcionario_ativo &&
                                             professor.ProfessorTurma.Where(pt => pt.cd_turma == cd_turma && pt.id_professor_ativo).Any()
                                              select new ProfessorUI {
                                                  cd_pessoa = professor.cd_funcionario,
                                                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                                                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa
                                              };
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProfessorUI> getProfessorTurmaLogado(int cd_escola, int cd_turma, int cd_usuario)
        {
            try
            {
                IQueryable<ProfessorUI> sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                    join u in db.UsuarioWebSGF on professor.cd_pessoa_funcionario equals u.cd_pessoa 
                    where professor.id_professor == true && professor.cd_pessoa_empresa == cd_escola &&
                          u.cd_usuario == cd_usuario &&
                          //professor.id_funcionario_ativo &&
                          professor.ProfessorTurma.Where(pt => pt.cd_turma == cd_turma && pt.id_professor_ativo).Any()
                    select new ProfessorUI
                    {
                        cd_pessoa = professor.cd_funcionario,
                        no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                        no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa
                    };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscola(int cdEscola, int? cdProfessor, bool? status) {
            try
            {
                var idProfessor = cdProfessor;
                var sql = from professor in db.FuncionarioSGF
                          join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                          where  (professor.cd_pessoa_empresa == cdEscola) 
                                && ((professor.id_funcionario_ativo == status)
                                    || (cdProfessor == idProfessor && professor.cd_funcionario == cdProfessor))
                          orderby professor.FuncionarioPessoaFisica.no_pessoa
                          select new ProfessorUI
                          {
                              cd_pessoa = professor.cd_funcionario,
                              no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscolaAtividade(int cdEscola, int? cdProfessor, int? cd_atividade_extra, bool? status)
        {
            try
            {
                int cdEscolaAtividade = (from a in db.AtividadeExtra where a.cd_atividade_extra == cd_atividade_extra select a.cd_pessoa_escola).FirstOrDefault();
                if (cdEscola != cdEscolaAtividade && cdProfessor != null) cdEscola = cdEscolaAtividade;
                var idProfessor = cdProfessor;
                var sql = from professor in db.FuncionarioSGF
                          join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                          where (professor.cd_pessoa_empresa == cdEscola)
                                && ((professor.id_funcionario_ativo == status)
                                    || (cdProfessor == idProfessor && professor.cd_funcionario == cdProfessor))
                          orderby professor.FuncionarioPessoaFisica.no_pessoa
                          select new ProfessorUI
                          {
                              cd_pessoa = professor.cd_funcionario,
                              no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscolaAulaReposicao(int cdEscola, int? cdProfessor, bool? status)
        {
            try
            {
                var idProfessor = cdProfessor;
                var sql = from professor in  db.FuncionarioSGF.OfType<Professor>()
                    join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                    where (professor.cd_pessoa_empresa == cdEscola) //&&
                        //professor.ProfessorTurma.Where(pt => pt.cd_professor == professor.cd_funcionario && pt.id_professor_ativo == true).Any() 
                          && ((professor.id_funcionario_ativo == status)
                              || (cdProfessor == idProfessor && professor.cd_funcionario == cdProfessor))
                    orderby professor.FuncionarioPessoaFisica.no_pessoa
                    select new ProfessorUI
                    {
                        cd_pessoa = professor.cd_funcionario,
                        no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                    };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public Professor getFuncionarioEditById(int cdFuncionario, int cdEmpresa,TipoConsultaProfessorEnum tipo)
        {
            try
            {
                Professor sql = null;
                switch (tipo)
                {
                        //Include(p => p.HabilitacaoProfessor).Include(p => p.HabilitacaoProfessor.Select(h => h.Produto))
                        //       .Include(p => p.HabilitacaoProfessor.Select(h => h.Estagio))
                    case TipoConsultaProfessorEnum.DADOS_PROF_EDIT:
                        sql = (from func in db.FuncionarioSGF.OfType<Professor>()
                               where func.cd_funcionario == cdFuncionario && func.cd_pessoa_empresa == cdEmpresa
                               select new
                               {
                                   cd_funcionario = func.cd_funcionario,
                                   cd_pessoa_funcionario = func.cd_pessoa_funcionario,
                                   dt_admissao = func.dt_admissao,
                                   dt_demissao = func.dt_demissao,
                                   id_comissionado = func.id_comissionado,
                                   id_professor = func.id_professor,
                                   id_funcionario_ativo = func.id_funcionario_ativo,
                                   vl_salario = func.vl_salario,
                                   cd_cargo = func.cd_cargo,
                                   id_colaborador_cyber = func.id_colaborador_cyber,
                                   no_atividade = func.FuncionarioAtividade.no_atividade,
                                   habilitacaoProfessor = func.HabilitacaoProfessor,
                                   id_forma_pagamento = func.id_forma_pagamento,
                                   id_contratado = func.id_contratado,
                                   vl_pagamento_interno = func.vl_pagamento_interno,
                                   vl_pagamento_externo = func.vl_pagamento_externo,
                                   dc_numero_chapa = func.dc_numero_chapa,
                                   id_coordenador = func.id_coordenador,
                                   pc_termino_estagio = func.pc_termino_estagio,
                                   vl_termino_estagio = func.vl_termino_estagio,
                                   vl_rematricula = func.vl_rematricula,
                                   FuncionarioComissao = func.FuncionarioComissao,
                                   func.nome_assinatura_certificado,
                               }).ToList().Select(x => new Professor
                                           {
                                               cd_funcionario = x.cd_funcionario,
                                               cd_pessoa_funcionario = x.cd_pessoa_funcionario,
                                               dt_admissao = x.dt_admissao,
                                               dt_demissao = x.dt_demissao,
                                               id_comissionado = x.id_comissionado,
                                               id_professor = x.id_professor,
                                               id_funcionario_ativo = x.id_funcionario_ativo,
                                               vl_salario = x.vl_salario,
                                               cd_cargo = x.cd_cargo,
                                               id_colaborador_cyber = x.id_colaborador_cyber,
                                               id_forma_pagamento = x.id_forma_pagamento,
                                               id_contratado = x.id_contratado,
                                               vl_pagamento_interno = x.vl_pagamento_interno,
                                               vl_pagamento_externo = x.vl_pagamento_externo,
                                               dc_numero_chapa = x.dc_numero_chapa,
                                               id_coordenador = x.id_coordenador,
                                               pc_termino_estagio = x.pc_termino_estagio,
                                               vl_termino_estagio = x.vl_termino_estagio,
                                               vl_rematricula = x.vl_rematricula,
                                               nome_assinatura_certificado = x.nome_assinatura_certificado,
                                               FuncionarioAtividade = new Atividade
                                               {
                                                   no_atividade = x.no_atividade
                                               },
                                               FuncionarioComissao = x.FuncionarioComissao.Select(fc => new FuncionarioComissao 
                                               {
                                                    cd_funcionario = fc.cd_funcionario,
                                                    cd_funcionario_comissao = fc.cd_funcionario_comissao,
                                                    cd_produto = fc.cd_produto,
                                                    no_produto = fc.cd_produto > 0 ? db.Produto.Where(y => y.cd_produto == fc.cd_produto).FirstOrDefault().no_produto : "",
                                                    pc_comissao_matricula = fc.pc_comissao_matricula,
                                                    pc_comissao_rematricula = fc.pc_comissao_rematricula,
                                                    vl_comissao_matricula = fc.vl_comissao_matricula,
                                                    vl_comissao_rematricula = fc.vl_comissao_rematricula
                                               }).ToList()
                                           }).FirstOrDefault();
                        if (sql != null && sql.cd_funcionario > 0)
                            sql.turmaAtivas = (from t in db.Turma
                                               where t.cd_pessoa_escola == cdEmpresa && t.cd_turma_ppt == null && 
                                                    ((!t.id_turma_ppt && t.dt_termino_turma == null) || (t.id_turma_ppt && t.id_turma_ativa)) && 
                                                     t.TurmaProfessorTurma.Where(pt => pt.Professor.cd_funcionario == cdFuncionario && pt.id_professor_ativo).Any()
                                               select new { 
                                                   cd_turma = t.cd_turma,
                                                   no_turma =  t.no_turma,
                                                   no_curso = t.Curso.no_curso,
                                                   no_sala = t.Sala.no_sala
                                               }).ToList().Select(x => new Turma{
                                                   cd_turma = x.cd_turma,
                                                   no_turma = x.no_turma,
                                                   no_curso = x.no_curso,
                                                   no_sala = x.no_sala
                                               }).ToList();
                        sql.HabilitacaoProfessor = (from hp in db.HabilitacaoProfessor
                                                    where hp.cd_professor == sql.cd_funcionario && hp.Professor.cd_pessoa_empresa == cdEmpresa
                                                    select new
                                                    {
                                                        cd_produto = hp.cd_produto,
                                                        cd_estagio = hp.cd_estagio,
                                                        cd_habilitacao_professor = hp.cd_habilitacao_professor,
                                                        cd_professor = hp.cd_professor,
                                                        no_produto = hp.Produto.no_produto,
                                                        no_estagio = hp.Estagio.no_estagio
                                                    }).ToList().Select(x => new HabilitacaoProfessor
                                                    {
                                                        cd_produto = x.cd_produto,
                                                        cd_estagio = x.cd_estagio,
                                                        cd_habilitacao_professor = x.cd_habilitacao_professor,
                                                        cd_professor = x.cd_professor,
                                                        Estagio = new Estagio { no_estagio = x.no_estagio },
                                                        Produto = new Produto { no_produto = x.no_produto }
                                                    }).ToList();
                        break;
                    case TipoConsultaProfessorEnum.DADOS_FUNC_EDIT:
                        var funcionario = (from func in db.FuncionarioSGF
                                           where func.cd_funcionario == cdFuncionario && func.cd_pessoa_empresa == cdEmpresa
                                           select new { 
                                                    cd_funcionario = func.cd_funcionario,
                                                    cd_pessoa_funcionario = func.cd_pessoa_funcionario,
                                                    dt_admissao = func.dt_admissao,
                                                    dt_demissao = func.dt_demissao,
                                                    id_comissionado = func.id_comissionado,
                                                    id_professor = func.id_professor,
                                                    id_funcionario_ativo = func.id_funcionario_ativo,
                                                    vl_salario = func.vl_salario,
                                                    cd_cargo = func.cd_cargo,
                                                    id_colaborador_cyber = func.id_colaborador_cyber,
                                                    no_atividade = func.FuncionarioAtividade.no_atividade,
                                                    FuncionarioComissao = func.FuncionarioComissao,
                                                    func.nome_assinatura_certificado
                                           }).ToList().Select(x => new FuncionarioSGF
                                           {
                                               cd_funcionario = x.cd_funcionario,
                                               cd_pessoa_funcionario = x.cd_pessoa_funcionario,
                                               dt_admissao = x.dt_admissao,
                                               dt_demissao = x.dt_demissao,
                                               id_comissionado = x.id_comissionado,
                                               id_professor = x.id_professor,
                                               id_funcionario_ativo = x.id_funcionario_ativo,
                                               vl_salario = x.vl_salario,
                                               cd_cargo = x.cd_cargo,
                                               id_colaborador_cyber = x.id_colaborador_cyber,
                                               nome_assinatura_certificado = x.nome_assinatura_certificado,
                                               FuncionarioAtividade = new Atividade
                                               {
                                                   no_atividade = x.no_atividade
                                               },
                                               FuncionarioComissao = x.FuncionarioComissao.Select(fc => new FuncionarioComissao
                                               {
                                                   cd_funcionario = fc.cd_funcionario,
                                                   cd_funcionario_comissao = fc.cd_funcionario_comissao,
                                                   cd_produto = fc.cd_produto,
                                                   no_produto = fc.cd_produto > 0 ? db.Produto.Where(y => y.cd_produto == fc.cd_produto).FirstOrDefault().no_produto : "",
                                                   pc_comissao_matricula = fc.pc_comissao_matricula,
                                                   pc_comissao_rematricula = fc.pc_comissao_rematricula,
                                                   vl_comissao_matricula = fc.vl_comissao_matricula,
                                                   vl_comissao_rematricula = fc.vl_comissao_rematricula
                                               }).ToList()
                                           }).FirstOrDefault();
                        if (funcionario != null && funcionario.cd_funcionario > 0){
                            sql = new Professor();
                            sql.copy(funcionario);
                        }
                        break;
                    case TipoConsultaProfessorEnum.LOAD_PROFE:
                        sql = (from func in db.FuncionarioSGF.OfType<Professor>()
                               where func.cd_funcionario == cdFuncionario && func.cd_pessoa_empresa == cdEmpresa
                               select func).FirstOrDefault();
                              
                        break;
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<HabilitacaoProfessor> getAllHabilitacaoProfessorByCdProfessor(int cdProfessor)
        {
            try
            {
                var sql = from habilitacao in db.HabilitacaoProfessor
                          where habilitacao.cd_professor == cdProfessor
                          select habilitacao;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public void deletarHabilitacaoProfessor(HabilitacaoProfessor habilitacaoProfessor)
        {
            try
            {
                db.HabilitacaoProfessor.Remove(habilitacaoProfessor);
                db.SaveChanges();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void addHabilitacaoProfessor(HabilitacaoProfessor habilitacaoProfessor)
        {
            try
            {
                db.HabilitacaoProfessor.Add(habilitacaoProfessor);
                db.SaveChanges();

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

          //Método responsavel por buscar todos alunos que estejam disponíveis nos horários de uma determinada turma (e não estão alocados no mesmos horários para outra turma).
        public IEnumerable<FuncionarioSearchUI> getProfessoresDisponiveisFaixaHorario(SearchParameters parametros, string desc, string nomeRed, bool inicio, bool? status,
                                                                           string cpf, int sexo, List<Horario> horariosTurma, int cd_escola, int cd_turma, int cd_curso, 
                                                                           bool PPT_pai, int cd_produto, DateTime dtInicio, DateTime? dtFinal, int cd_duracao )
        {
            IEntitySorter<FuncionarioSearchUI> sorter = EntitySorter<FuncionarioSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            //IEnumerable<FuncionarioSearchUI> sql;
            try
            {

                int cd_escola_turma = (from t in db.Turma where t.cd_turma == cd_turma select t.cd_pessoa_escola).FirstOrDefault();

                bool habilProf = (bool)(from p in db.Parametro
                                        where p.cd_pessoa_escola == ((cd_escola == cd_escola_turma && cd_escola_turma > 0) || cd_escola_turma == 0 ? cd_escola : cd_escola_turma)
                                        select p.id_liberar_habilitacao_professor).FirstOrDefault();
                decimal nm_duracao = (from d in db.Duracao
                                      where d.cd_duracao == cd_duracao
                                      select d.nm_duracao).FirstOrDefault();
                short? nm_carga_horaria = (from c in db.Curso
                                           where c.cd_curso == cd_curso
                                           select c.nm_carga_horaria).FirstOrDefault();

                DateTime? dt_final_carga = dtFinal == null ? dtInicio.AddDays(nm_duracao == 0 ? 0 : (int)(nm_carga_horaria == null ? 120 : nm_carga_horaria / (double)nm_duracao * 7.0)) : dtFinal;
                //if (PPT_pai)
                //    habilProf = false;
                /*
                if (sexo > 0)
                    sql1 = from c in sql1
                           where c.FuncionarioPessoaFisica.nm_sexo == sexo
                           select c;*/
                //if (habilProf)
                //    sql1 = from c in sql1
                //           where c.HabilitacaoProfessor.an
                //           select c;

                var sql1 = (from c in db.FuncionarioSGF
                           where
                               c.cd_pessoa_empresa == ((cd_escola == cd_escola_turma && cd_escola_turma > 0) || cd_escola_turma == 0 ? cd_escola : cd_escola_turma) &&
                               c.id_funcionario_ativo == true &&
                               c.dt_demissao == null &&
                               (!habilProf || db.HabilitacaoProfessor.Where(pp => pp.cd_professor == c.cd_funcionario).Any(x => ((PPT_pai && x.cd_produto == cd_produto) ||
                                                                                                                                 db.Curso.Any(z => z.cd_curso == cd_curso &&
                                                                                                                                                   z.cd_estagio == x.cd_estagio))
                               )) &&
                               c.cd_pessoa_empresa == ((cd_escola == cd_escola_turma && cd_escola_turma > 0) || cd_escola_turma == 0 ? cd_escola : cd_escola_turma) &&
                               c.id_funcionario_ativo == true &&
                               c.dt_demissao == null
                           select c);

                if (!string.IsNullOrEmpty(desc))
                {
                    if (inicio)
                        sql1 = from func in sql1
                              where func.FuncionarioPessoaFisica.no_pessoa.StartsWith(desc)
                              select func;
                    else
                        sql1 = from c in sql1
                              where c.FuncionarioPessoaFisica.no_pessoa.Contains(desc)
                              select c;
                }

                if (!string.IsNullOrEmpty(nomeRed))
                {
                    if (inicio)
                        sql1 = from func in sql1
                              where func.FuncionarioPessoaFisica.dc_reduzido_pessoa.StartsWith(nomeRed)
                               select func;
                    else
                        sql1 = from c in sql1
                              where c.FuncionarioPessoaFisica.dc_reduzido_pessoa.Contains(nomeRed)
                               select c;
                }

                var sql = (from c in sql1
                      select new FuncionarioSearchUI
                      {
                          cd_funcionario = c.cd_funcionario,
                          cd_pessoa_funcionario = c.cd_pessoa_funcionario,
                          no_pessoa = c.FuncionarioPessoaFisica.no_pessoa,
                          dc_num_pessoa = c.FuncionarioPessoaFisica.dc_num_pessoa,
                          nm_natureza_pessoa = c.FuncionarioPessoaFisica.nm_natureza_pessoa,
                          dc_reduzido_pessoa = c.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                          dt_cadastramento = c.dt_admissao,
                          id_funcionario_ativo = c.id_funcionario_ativo,
                          nm_cpf_cgc = c.FuncionarioPessoaFisica.nm_cpf,
                          ext_img_pessoa = c.FuncionarioPessoaFisica.ext_img_pessoa,
                          no_atividade = c.FuncionarioPessoaFisica.Atividade.no_atividade,
                          id_professor = c.id_professor,
                          nm_atividade_principal = c.FuncionarioPessoaFisica.cd_atividade_principal,
                          nm_endereco_principal = c.FuncionarioPessoaFisica.cd_endereco_principal,
                          nm_telefone_principal = c.FuncionarioPessoaFisica.cd_telefone_principal,
                          des_cargo = c.FuncionarioAtividade.no_atividade,
                          cursosHabilitacao = db.Curso.Where(cs => db.HabilitacaoProfessor.Any(hp => hp.cd_professor == c.cd_funcionario && hp.cd_produto == cs.cd_produto && hp.cd_estagio == cs.cd_estagio)).Select(cs => cs.cd_curso),
                          cd_pessoa_escola = c.cd_pessoa_empresa,
                          nm_sexo = c.FuncionarioPessoaFisica.nm_sexo

                      }).AsEnumerable();



                 sql = sorter.Sort(sql.AsQueryable());

                 if (horariosTurma != null && horariosTurma.Count() > 0)
                 {
                     var em = horariosTurma.GetEnumerator();
                     while (em.MoveNext())
                     {
                         var h = em.Current;
                        sql = (from professor in sql
                               where
                               !(from ht in db.Horario
                                 join turmas in db.Turma on ht.cd_registro equals turmas.cd_turma
                                 where turmas.cd_turma != cd_turma &&
                                 ((turmas.cd_turma_ppt == null && !turmas.id_turma_ppt && turmas.dt_termino_turma == null) ||
                                  (turmas.id_turma_ppt && turmas.id_turma_ativa && !turmas.TurmaFilhasPPT.Any())) &&
                                 turmas.TurmaProfessorTurma.Where(t => t.cd_professor == professor.cd_funcionario && t.id_professor_ativo == true).Any()
                                 && ht.HorariosProfessores.Where(hp => hp.cd_horario == ht.cd_horario && hp.cd_professor == professor.cd_funcionario).Any()
                                 && ht.id_origem == (int)Horario.Origem.TURMA
                                 &&  ((turmas.dt_final_aula == null && DbFunctions.AddDays(turmas.dt_inicio_aula, turmas.Curso == null ? 120 : turmas.Duracao.nm_duracao == 0 ? 0 : (int)(turmas.Curso.nm_carga_horaria / (double)turmas.Duracao.nm_duracao * 7.0)) >= dtInicio) ||
                                      (turmas.dt_final_aula != null && turmas.dt_final_aula >= dtInicio)) &&
                                      turmas.dt_inicio_aula <= dt_final_carga
                                 && ht.id_dia_semana == h.id_dia_semana
                                 && ((ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_ini < ht.dt_hora_fim)
                                                || (ht.dt_hora_ini < h.dt_hora_fim && h.dt_hora_fim <= ht.dt_hora_fim)
                                                || (ht.dt_hora_ini <= h.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim)
                                                || (ht.dt_hora_ini >= h.dt_hora_ini && h.dt_hora_fim >= ht.dt_hora_fim)
                                             )
                                 select ht.cd_horario).Any()
                                 &&
                                (from ht in db.Horario
                                 where ht.cd_registro == professor.cd_funcionario && ht.id_origem == (int)Horario.Origem.PROFESSOR &&
                                      ht.cd_pessoa_escola == ((cd_escola == cd_escola_turma && cd_escola_turma > 0) || cd_escola_turma == 0 ? cd_escola : cd_escola_turma) && ht.id_dia_semana == h.id_dia_semana &&
                                      h.dt_hora_ini >= ht.dt_hora_ini && h.dt_hora_fim <= ht.dt_hora_fim
                                 select ht.cd_horario).Any()
                               select professor).ToList();
                    }
                }
                    


                if (sexo > 0)
                    sql = from c in sql
                        where c.nm_sexo == sexo
                        select c;

                if (!string.IsNullOrEmpty(cpf))
                {
                    sql = from c in sql
                          where c.nm_cpf_cgc == cpf
                          select c;
                }

                if (status != null)
                {
                    sql = from c in sql
                          where c.id_funcionario_ativo == status
                          select c;
                }
               

               

               
               


               int limite = sql.Select(x => x.cd_funcionario).ToList().Count();

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


        public IEnumerable<Professor> getProfessoresDisponiveisFaixaHorarioTurma(TurmaAlunoProfessorHorario turmaProfessorHorario, int cdEscola)
        {
            try
            {

                 int cd_escola_turma = (from t in db.Turma
                                       where ((turmaProfessorHorario.cd_turma > 0 && turmaProfessorHorario.cd_turma == t.cd_turma) ||
                           (turmaProfessorHorario.cd_turma_ppt > 0 && turmaProfessorHorario.cd_turma_ppt == t.cd_turma_ppt))
                    select t.cd_pessoa_escola).FirstOrDefault();


                var professoresT = from prof in db.FuncionarioSGF.OfType<Professor>()
                                   where (prof.cd_pessoa_empresa == cdEscola || (cd_escola_turma > 0 && cdEscola != cd_escola_turma && prof.cd_pessoa_empresa == cd_escola_turma))
                                   && prof.id_funcionario_ativo == true
                                   //?????????????????? && prof.ProfessorTurma.Any(a => a.cd_professor == prof.cd_funcionario)
                                   && turmaProfessorHorario.professores.Contains(prof.cd_funcionario)
                                    select prof;

                professoresT = from prof in professoresT
                          where
                              // Não exista horário alocado em outras turmas.
                          !(from ht in db.Horario
                            join turmas in db.Turma on ht.cd_registro equals turmas.cd_turma
                            join profTurma in db.ProfessorTurma on turmas.cd_turma equals profTurma.cd_turma
                            where ((turmas.cd_turma_ppt == null && !turmas.id_turma_ppt && turmas.dt_termino_turma == null) || 
                                   (turmas.id_turma_ppt && turmas.id_turma_ativa && !turmas.TurmaFilhasPPT.Any())) &&
                                    ((turmas.dt_final_aula == null && DbFunctions.AddDays(turmas.dt_inicio_aula, turmas.Curso == null ? 120 : turmas.Duracao.nm_duracao == 0 ? 0 : (int)(turmas.Curso.nm_carga_horaria == null ? 0 : turmas.Curso.nm_carga_horaria / (double)turmas.Duracao.nm_duracao * 7.0)) >= turmaProfessorHorario.dt_inicio) ||
                                        turmas.dt_final_aula >= turmaProfessorHorario.dt_inicio)
                                    && turmas.dt_inicio_aula <= turmaProfessorHorario.dt_final &&
                            (turmaProfessorHorario.cd_turma == 0 || turmas.cd_turma != turmaProfessorHorario.cd_turma)
                            && turmaProfessorHorario.professores.Contains(profTurma.cd_professor)
                            && profTurma.id_professor_ativo
                            && ht.HorariosProfessores.Where(hp => hp.cd_horario == ht.cd_horario && hp.cd_professor == prof.cd_funcionario).Any()
                            //turmas.ProfessorTurma.Where(t => t.cd_professor == prof.cd_funcionario).Any()
                            && ht.id_origem == (int)Horario.Origem.TURMA
                            && ht.id_dia_semana == turmaProfessorHorario.horario.id_dia_semana
                            && ((ht.dt_hora_ini <= turmaProfessorHorario.horario.dt_hora_ini && turmaProfessorHorario.horario.dt_hora_ini < ht.dt_hora_fim)
                                        || (ht.dt_hora_ini < turmaProfessorHorario.horario.dt_hora_fim && turmaProfessorHorario.horario.dt_hora_fim <= ht.dt_hora_fim)
                                        || (ht.dt_hora_ini <= turmaProfessorHorario.horario.dt_hora_ini && turmaProfessorHorario.horario.dt_hora_fim <= ht.dt_hora_fim)
                                        || (ht.dt_hora_ini >= turmaProfessorHorario.horario.dt_hora_ini && turmaProfessorHorario.horario.dt_hora_fim >= ht.dt_hora_fim)
                                        )
                            select ht.cd_horario).Any()
                            &&
                              //Que exista horários disponiveis.
                           (from ht in db.Horario
                            where (ht.cd_registro == prof.cd_funcionario && ht.id_origem == (int)Horario.Origem.PROFESSOR &&
                                   /*ht.cd_pessoa_escola == cdEscola &&*/ ht.id_dia_semana == turmaProfessorHorario.horario.id_dia_semana &&
                                  turmaProfessorHorario.horario.dt_hora_ini >= ht.dt_hora_ini && turmaProfessorHorario.horario.dt_hora_fim <= ht.dt_hora_fim)
                             select ht.cd_horario).Any()

                                select prof;
                return professoresT;

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSGF getFuncionarioById(int cd_funcionario,int cd_escola)
        {
            try
            {
                var sql = (from funcionarioSGF in db.FuncionarioSGF
                          where funcionarioSGF.cd_funcionario == cd_funcionario && funcionarioSGF.cd_pessoa_empresa == cd_escola
                          select funcionarioSGF).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSGF getProfLogByCodPessoaUsuario(int cd_pessoa_usuario, int cd_pessoa_empresa)
        {
            try
            {
                FuncionarioSGF sql = (from professor in db.FuncionarioSGF.OfType<Professor>()
                                      where professor.cd_pessoa_funcionario == cd_pessoa_usuario
                                        && professor.cd_pessoa_empresa == cd_pessoa_empresa
                                      select professor).FirstOrDefault();
                if (sql == null)
                    sql = (from professor in db.FuncionarioSGF.OfType<FuncionarioSGF>()
                           where professor.cd_pessoa_funcionario == cd_pessoa_usuario
                             && professor.cd_pessoa_empresa == cd_pessoa_empresa
                           select professor).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public ProfessorUI verificaRetornaSeUsuarioLogadoEProfessor(int cd_pessoa_usuario, int cd_pessoa_empresa)
        {
            try
            {
                var sql = (from professor in db.FuncionarioSGF.OfType<Professor>()
                           where professor.cd_pessoa_funcionario == cd_pessoa_usuario
                             && professor.cd_pessoa_empresa == cd_pessoa_empresa
                           select new ProfessorUI
              {
                  cd_pessoa = professor.cd_funcionario,
                  no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                  no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                  id_coordenador = professor.id_coordenador
                  
              }).FirstOrDefault();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteProfessor(int cd_professor, int cd_escola)
        {
            try
            {
                int retorno = db.Database.ExecuteSqlCommand("delete p from T_PROFESSOR p inner join  T_FUNCIONARIO f on p.cd_professor = f.cd_funcionario "+ 
                                                            "and f.cd_pessoa_empresa ="+ cd_escola + "and p.cd_professor =" + cd_professor);
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int addProfExistFunc(Professor prof)
        {
            int retorno;
            try
            {
                retorno = db.Database.ExecuteSqlCommand("insert into t_professor(cd_professor,id_forma_pagamento, id_contratado, vl_pagamento_interno," +
                                                        "vl_pagamento_externo, dc_numero_chapa, id_coordenador) values({0},{1},{2},{3},{4},{5},{6})",
                                                        +prof.cd_funcionario, prof.id_forma_pagamento, prof.id_contratado, prof.vl_pagamento_interno, prof.vl_pagamento_externo, prof.dc_numero_chapa, prof.id_coordenador);

            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

            return retorno;
        }

        public IEnumerable<ProfessorUI> getProfessorAvaliacaoTurma(int cd_pessoa_empresa) {
            try
            {
                var sql = (from funcionario in db.FuncionarioSGF
                           join avaliacaoTurma in db.AvaliacaoTurma on funcionario.cd_funcionario equals avaliacaoTurma.cd_funcionario
                           join pessoa in db.PessoaSGF on funcionario.cd_pessoa_funcionario equals pessoa.cd_pessoa
                           where funcionario.cd_pessoa_empresa == cd_pessoa_empresa
                           select new ProfessorUI
                           {
                               cd_pessoa = funcionario.cd_funcionario,
                               no_pessoa = pessoa.no_pessoa
                           }).Distinct().OrderBy(p => p.no_pessoa);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProfessorUI> getFuncionariosByEscolaWithAtividadeExtra(int cd_pessoa_escola, bool status)
        {
            try
            {
                var sql = (from professor in db.FuncionarioSGF
                          join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                          join atividadeExtra in db.AtividadeExtra on professor.cd_funcionario equals atividadeExtra.cd_funcionario
                          where (professor.cd_pessoa_empresa == cd_pessoa_escola)
                                && (professor.id_funcionario_ativo == status)
                          orderby professor.FuncionarioPessoaFisica.no_pessoa
                          select new ProfessorUI
                          {
                              cd_pessoa = professor.cd_funcionario,
                              no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                          }).Distinct().OrderBy(p => p.no_pessoa);
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresByCodigos(int[] cdProfs, int cd_escola)
        {
            try
            {
                var sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                          join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                          where (professor.id_professor == true)
                                && cdProfs.Contains(professor.cd_funcionario)
                                && (professor.cd_pessoa_empresa == cd_escola)
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = professor.cd_funcionario,
                              no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa,
                              dc_reduzido_pessoa = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              nm_cpf_cgc = professor.FuncionarioPessoaFisica.nm_cpf,
                              dt_cadastramento = professor.FuncionarioPessoaFisica.dt_cadastramento,
                              id_funcionario_ativo = professor.id_funcionario_ativo,
                              des_cargo = professor.FuncionarioAtividade.no_atividade
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresTurmaPPTPorFaixaHorariosTurmaFilha(List<Horario> horariosTurmaFilha, int cd_escola, int cd_turma_PPT)
        {
            try
            {
                int cd_escola_turma = (from t in db.Turma where t.cd_turma == cd_turma_PPT select t.cd_pessoa_escola).FirstOrDefault();

                IEnumerable<FuncionarioSearchUI> listaFuncionarios = new List<FuncionarioSearchUI>();
                var sql = from c in db.FuncionarioSGF.OfType<Professor>()
                          where c.cd_pessoa_empresa == (cd_escola == cd_escola_turma ? cd_escola : cd_escola_turma) &&
                                c.id_funcionario_ativo == true &&
                                c.dt_demissao == null
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = c.cd_funcionario,
                              cd_pessoa_funcionario = c.cd_pessoa_funcionario,
                              no_pessoa = c.FuncionarioPessoaFisica.no_pessoa,
                              nm_natureza_pessoa = c.FuncionarioPessoaFisica.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              dt_cadastramento = c.dt_admissao,
                              id_funcionario_ativo = c.id_funcionario_ativo,
                              nm_cpf_cgc = c.FuncionarioPessoaFisica.nm_cpf,
                              id_professor = c.id_professor,
                              des_cargo = c.FuncionarioAtividade.no_atividade
                          };

                if (horariosTurmaFilha != null && horariosTurmaFilha.Count() > 0)
                    foreach (var h in horariosTurmaFilha)
                    {
                        sql = from prof in sql
                              where
                         (from ht in db.Horario
                          where
                          ht.HorariosProfessores.Where(hp => hp.cd_horario == ht.cd_horario && hp.cd_professor == prof.cd_funcionario).Any()
                          && ht.cd_registro == cd_turma_PPT && ht.id_origem == (int)Horario.Origem.TURMA
                          && ht.cd_pessoa_escola == (cd_escola == cd_escola_turma ? cd_escola : cd_escola_turma)
                          && ht.id_dia_semana == h.id_dia_semana
                          && h.dt_hora_ini >= ht.dt_hora_ini
                          && h.dt_hora_fim <= ht.dt_hora_fim
                          select ht.cd_horario).Any()
                              select prof;
                    }

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public IEnumerable<ProfessorUI> getProfHorariosProgTurma(int cd_turma, int cd_escola,byte diaSemana,TimeSpan horaIni,TimeSpan horaFim)
        {
            try
            {
                int cd_escola_turma = (from t in db.Turma where t.cd_turma == cd_turma select t.cd_pessoa_escola).FirstOrDefault();
                var sql = from professor in db.FuncionarioSGF.OfType<Professor>()
                          where professor.id_professor == true /*&& professor.cd_pessoa_empresa == (cd_escola == cd_escola_turma ? cd_escola : cd_escola_turma)*/ &&
                      professor.ProfessorTurma.Where(pt => pt.cd_professor == professor.cd_funcionario && pt.cd_turma == cd_turma && pt.id_professor_ativo == true).Any() &&
                      professor.HorariosProfessores.Where(hp => hp.Horario.id_origem == (int)Horario.Origem.TURMA && 
                                                                hp.Horario.cd_registro == cd_turma  &&
                                                                /*hp.Horario.cd_pessoa_escola == (cd_escola == cd_escola_turma ? cd_escola : cd_escola_turma) &&*/
                                                                hp.cd_professor == professor.cd_funcionario &&
                                                                hp.Horario.id_dia_semana == diaSemana &&
                                                                hp.Horario.dt_hora_ini == horaIni &&
                                                                hp.Horario.dt_hora_fim == horaFim ).Any()
                      select new ProfessorUI
                      {
                          cd_pessoa = professor.cd_funcionario,
                          no_fantasia = professor.FuncionarioPessoaFisica.dc_reduzido_pessoa
                      };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresByEmpresa(int cd_pessoa_empresa, int? cd_turma)
        {
            try
            {
                var sql = from f in db.FuncionarioSGF
                          join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on f.cd_pessoa_funcionario equals pessoa.cd_pessoa
                          where f.cd_pessoa_empresa == cd_pessoa_empresa
                            && f.id_funcionario_ativo
                            && (f.id_professor || f.id_colaborador_cyber)
                            && f.dt_demissao == null 
                            || (cd_turma.HasValue && f.AvaliacaoTurma.Where(at => at.cd_turma == cd_turma).Any())
                          orderby f.FuncionarioPessoaFisica.no_pessoa
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = f.cd_funcionario,
                              no_pessoa = f.FuncionarioPessoaFisica.no_pessoa,
                              dc_reduzido_pessoa = f.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              nm_cpf_cgc = f.FuncionarioPessoaFisica.nm_cpf
                          };
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSGF findByIdFuncionario(int cd_funcionario, int cd_pessoa_escola)
        {
            try
            {
                FuncionarioSGF sql = (from funcionario in db.FuncionarioSGF
                          where funcionario.cd_funcionario == cd_funcionario && funcionario.cd_pessoa_empresa == cd_pessoa_escola
                          select funcionario).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FuncionarioSearchUI getProfessoresById(int cdProf, int cd_escola)
        {
            try
            {
                var sql = (from professor in db.FuncionarioSGF.OfType<Professor>()
                          join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                          where (professor.id_professor == true)
                                && professor.cd_funcionario == cdProf
                                && (professor.cd_pessoa_empresa == cd_escola)
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = professor.cd_funcionario,
                              no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                          }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FuncionarioSearchUI> getProfessoresAulaReposicao(int cd_escola)
        {
            try
            {
                var sql = (from professor in db.FuncionarioSGF.OfType<Professor>()
                    join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                           //join aulareposicao in db.AulaReposicao on professor.cd_funcionario equals aulareposicao.cd_professor
                           where (professor.id_professor == true) &&
                            professor.ProfessorTurma.Where(pt => pt.cd_professor == professor.cd_funcionario && pt.id_professor_ativo == true).Any() 
                            && (professor.cd_pessoa_empresa == cd_escola)
                            && professor.AulaReposicao.Any()
                    orderby professor.FuncionarioPessoaFisica.no_pessoa
                    select new FuncionarioSearchUI
                    {
                        cd_funcionario = professor.cd_funcionario,
                        cd_pessoa = professor.cd_funcionario,
                        no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                    });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public void deleteProfessorContexto(FuncionarioSGF funcionario)
        {
            try
            {
                db.FuncionarioSGF.Remove(funcionario);
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FuncionarioSearchUI> getSearchFuncionario(SearchParameters parametros, string nome, string apelido, bool? status, string cpf, bool inicio, byte tipo, int cdEscola, int sexo,
    int cdAtividade, int coordenador, int colaborador_cyber)
        {
            try
            {
                IEntitySorter<FuncionarioSearchUI> sorter = EntitySorter<FuncionarioSearchUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<FuncionarioSearchUI> retorno;

                var sql = from f in db.FuncionarioSGF.AsNoTracking()
                          where f.cd_pessoa_empresa == cdEscola
                          select f;

                if (!string.IsNullOrEmpty(nome))
                {
                    if (inicio)
                    {
                        sql = from func in sql
                              where func.FuncionarioPessoaFisica.no_pessoa.StartsWith(nome)
                              select func;
                    }//end if
                    else
                    {
                        sql = from c in sql
                              where c.FuncionarioPessoaFisica.no_pessoa.Contains(nome)
                              select c;
                    }//end else


                }
                if (!string.IsNullOrEmpty(cpf))
                {
                    sql = from c in sql
                          where c.FuncionarioPessoaFisica.nm_cpf == cpf
                          select c;
                }

                if (!string.IsNullOrEmpty(apelido))
                {
                    if (inicio)
                    {
                        sql = from c in sql
                              where c.FuncionarioPessoaFisica.dc_reduzido_pessoa.StartsWith(apelido)
                              select c;
                    }
                    else
                    {
                        sql = from c in sql
                              where c.FuncionarioPessoaFisica.dc_reduzido_pessoa.Contains(apelido)
                              select c;
                    }
                }

                if (status != null)
                    sql = from c in sql
                          where c.id_funcionario_ativo == status
                          select c;
                if (tipo > (byte)FuncionarioSGF.TipoFuncionario.TODOS)
                {
                    if (tipo == (byte)FuncionarioSGF.TipoFuncionario.FUNCIONARIO)
                    {
                        sql = from c in sql
                              where !c.id_professor
                              select c;
                        if (colaborador_cyber > 0)
                            sql = from c in sql
                                  where c.id_colaborador_cyber == (colaborador_cyber == 1 ? true : false)
                                  select c;
                    }
                    else
                    {
                        sql = from c in sql
                              where c.id_professor
                              select c;
                        if (coordenador > 0)
                            sql = from c in sql
                                  where db.FuncionarioSGF.OfType<Professor>().Any(x => x.cd_funcionario == c.cd_funcionario && x.id_coordenador == (coordenador == 1 ? true : false))
                                  select c;
                    }
                }

                if (cdAtividade > 0)
                    sql = from c in sql
                          where c.cd_cargo == cdAtividade
                          select c;

                retorno = from c in sql
                          where c.cd_pessoa_empresa == cdEscola //&&
                          //c.PessoaFisica.nm_sexo == sexo
                          select new FuncionarioSearchUI
                          {
                              cd_funcionario = c.cd_funcionario,
                              cd_pessoa_funcionario = c.FuncionarioPessoaFisica.cd_pessoa,
                              no_pessoa = c.FuncionarioPessoaFisica.no_pessoa,
                              dc_num_pessoa = c.FuncionarioPessoaFisica.dc_num_pessoa,
                              nm_natureza_pessoa = c.FuncionarioPessoaFisica.nm_natureza_pessoa,
                              dc_reduzido_pessoa = c.FuncionarioPessoaFisica.dc_reduzido_pessoa,
                              dt_cadastramento = c.FuncionarioPessoaFisica.dt_cadastramento,
                              id_funcionario_ativo = c.id_funcionario_ativo,
                              nm_cpf_cgc = c.FuncionarioPessoaFisica.cd_pessoa_cpf > 0 ? c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.nm_cpf : c.FuncionarioPessoaFisica.nm_cpf,
                              no_pessoa_dependente = c.FuncionarioPessoaFisica.PessoaSGFQueUsoOCpf.no_pessoa,
                              ext_img_pessoa = c.FuncionarioPessoaFisica.ext_img_pessoa,
                              //no_atividade = c.PessoaFisica.AtividadePessoa.no_atividade,
                              //cd_atividade = c.PessoaFisica.cd_atividade_principal,
                              id_professor = c.id_professor,
                              nm_atividade_principal = c.FuncionarioPessoaFisica.cd_atividade_principal,
                              nm_endereco_principal = c.FuncionarioPessoaFisica.cd_endereco_principal,
                              nm_telefone_principal = c.FuncionarioPessoaFisica.cd_telefone_principal,
                              cd_cargo = c.cd_cargo,
                              des_cargo = c.FuncionarioAtividade.no_atividade
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

        public IEnumerable<FuncionarioSGF> getFuncionariosByAulaPers(int cdEscola)
        {
            try
            {
                var sql = (from professor in db.FuncionarioSGF.OfType<Professor>()
                           join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                           where professor.cd_pessoa_empresa == cdEscola &&
                            professor.ProfessorTurma.Any(p => p.Turma.AulaPersonalizada.Any())
                           select new
                                    {
                                        cd_funcionario = professor.cd_funcionario,
                                        no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                                    }).ToList().Select(x => new FuncionarioSGF
                           {
                               cd_funcionario = x.cd_funcionario,
                               no_pessoa = x.no_pessoa
                           });

                //var sql = (from professor in db.FuncionarioSGF.OfType<Professor>()
                //           join pessoa in db.PessoaSGF.OfType<PessoaFisicaSGF>() on professor.cd_pessoa_funcionario equals pessoa.cd_pessoa
                //           where professor.cd_pessoa_empresa == cdEscola // &&
                //          // professor.ProfessorTurma.Any(p => p.Turma.AulaPersonalizada.Any())
                //           select new FuncionarioSGF
                //           {
                //               cd_funcionario = professor.cd_funcionario //,                               no_pessoa = professor.FuncionarioPessoaFisica.no_pessoa
                //           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessores(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            try
            {
                var sql = db.st_RptPgtoProfessor(cd_empresa, cd_professor, (byte?)cd_tipo_relatorio, dt_ini, dt_fim)
                    .ToList().Select(s => new ReportPagamentoProfessor 
                    {
                        cd_professor = s.cd_professor,
                        cd_tipo_relatorio = cd_tipo_relatorio,
                        cd_turma = s.cd_turma,
                        dc_horario = s.dc_horario,
                        dc_horarios = s.dc_horarios,
                        dc_total_hora_sabado = s.dc_total_hora_sabado,
                        dc_total_horas_semana = s.dc_total_horas_semana,
                        dias_horarios = s.dias_horarios,
                        dt_dia_falta = s.dt_dia_falta,
                        dt_inicio_aula = s.dt_inicio_aula,
                        id_dia_semana = s.id_dia_semana,
                        no_estagio = s.no_estagio,
                        no_professor = s.no_professor,
                        no_professor_substituto = s.no_professor_substituto,
                        no_turma = s.no_turma,
                        qtd_alunos = s.qtd_alunos,
                        qtd_total_horas_prof = s.qtd_total_horas_prof,
                        tx_obs = s.tx_obs,
                        vl_hora_interna = s.vl_hora_interna,
                        vl_hora_externa = s.vl_hora_externa,
                        id_aula_externa = s.id_aula_externa
                    });

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresFaltas(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            try
            {
                var sql = db.st_RptPgtoProfessor_faltas(cd_empresa, cd_professor, (byte?)cd_tipo_relatorio, dt_ini, dt_fim).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ReportPagamentoProfessor> getRptPagamentoProfessoresObs(int cd_tipo_relatorio, int cd_empresa, int cd_professor, DateTime? dt_ini, DateTime? dt_fim)
        {
            try
            {
                var sql = db.st_RptPgtoProfessor_obs(cd_empresa, cd_professor, (byte?)cd_tipo_relatorio, dt_ini, dt_fim).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<FuncionarioComissao> getRptComissaoSecretarias(int cd_funcionario, int cd_produto, int cd_empresa, DateTime? dt_ini, DateTime? dt_fim)
        {
            try
            {
                var sql = (from fc in db.FuncionarioComissao
                           where fc.Funcionario.cd_pessoa_empresa == cd_empresa &&
                                 (cd_produto == 0 || fc.cd_produto == cd_produto) &&
                                 (cd_funcionario == 0 || fc.cd_funcionario == cd_funcionario)
                           select new
                           {
                               fc.cd_funcionario,
                               fc.cd_produto,
                               fc.Funcionario.FuncionarioPessoaFisica.no_pessoa,
                               fc.Produto.no_produto,
                               fc.vl_comissao_matricula,
                               fc.pc_comissao_matricula,
                               fc.vl_comissao_rematricula,
                               fc.pc_comissao_rematricula,
                               qtd_matriculas_por_pc = db.Contrato.Count(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                      ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                       (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                       (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                       c.id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA && c.vl_matricula_contrato > 0),
                               qtd_matriculas_por_vl = db.Contrato.Count(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                      ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                       (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                       (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                       c.id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA),
                               vl_total_matriculas_por_pc =  db.Contrato.Any(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                                                    ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                                                    (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                                                    (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                                                    c.id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA) ?
                                                                                                    db.Contrato.Where(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                                                    ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto)  || c.cd_produto_atual == cd_produto) &&
                                                                                                    (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                                                    (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                                                    c.id_tipo_matricula == (int)Contrato.TipoMatricula.MATRICULA).Sum(x => x.vl_matricula_contrato) : 0,
                               vl_total_rematriculas_por_pc = db.Contrato.Any(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                      ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                      (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                      (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                    c.id_tipo_matricula == (int)Contrato.TipoMatricula.REMATRICULA) ? db.Contrato.Where(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                      ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                      (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                      (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                    c.id_tipo_matricula == (int)Contrato.TipoMatricula.REMATRICULA).Sum(s => s.vl_matricula_contrato) : 0,
                               qtd_rematriculas_por_pc = db.Contrato.Count(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                          ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                           (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                           (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                           c.id_tipo_matricula == (int)Contrato.TipoMatricula.REMATRICULA && c.vl_matricula_contrato > 0),
                               qtd_rematriculas_por_vl = db.Contrato.Count(c => c.cd_pessoa_escola == cd_empresa && c.SysUsuario.cd_pessoa == fc.Funcionario.cd_pessoa_funcionario &&
                                                                      ((cd_produto == 0 && c.cd_produto_atual == fc.cd_produto) || c.cd_produto_atual == cd_produto) &&
                                                                      (!dt_ini.HasValue || c.dt_matricula_contrato >= dt_ini.Value) &&
                                                                      (!dt_fim.HasValue || c.dt_matricula_contrato <= dt_fim.Value) &&
                                                                           c.id_tipo_matricula == (int)Contrato.TipoMatricula.REMATRICULA),

                           }).ToList().Select(x => new FuncionarioComissao
                           {
                               cd_funcionario = x.cd_funcionario,
                               cd_produto = x.cd_produto,
                               no_professor = x.no_pessoa,
                               no_produto = x.no_produto,


                               vl_comissao_matricula = x.vl_comissao_matricula > 0 ? x.vl_comissao_matricula :
                                                           x.vl_total_matriculas_por_pc > 0 && x.pc_comissao_matricula > 0 ?
                                                             Decimal.Round((x.vl_total_matriculas_por_pc / x.qtd_matriculas_por_pc) * (decimal)(x.pc_comissao_matricula / 100), 2) : 0,
                               pc_comissao_matricula = x.pc_comissao_matricula,
                               vl_comissao_rematricula = x.vl_comissao_rematricula > 0 ? x.vl_comissao_rematricula : 
                                                           x.vl_total_rematriculas_por_pc > 0 && x.pc_comissao_rematricula > 0 ?
                                                             Decimal.Round((x.vl_total_rematriculas_por_pc / x.qtd_rematriculas_por_pc) * (x.pc_comissao_rematricula / 100), 2) : 0,
                               pc_comissao_rematricula = x.pc_comissao_rematricula,
                               qtd_matriculas = x.pc_comissao_matricula > 0 ? x.qtd_matriculas_por_pc : x.qtd_matriculas_por_vl,
                               qtd_rematriculas = x.pc_comissao_rematricula > 0 ? x.qtd_rematriculas_por_pc : x.qtd_rematriculas_por_vl
                           });
                               
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<int> getVerificaProfessorHabilitacaoCursos(List<int>cdProfs, int cd_curso, int cd_escola)
        {
            try
            {

                var sql = (from f in db.FuncionarioSGF.OfType<Professor>().AsNoTracking()
                                                  where //f.cd_pessoa_empresa == cd_escola &&
                                                        cdProfs.Contains(f.cd_funcionario) &&
                                                        f.HabilitacaoProfessor.Any(x => db.Curso.Any(c => c.cd_curso == cd_curso && c.cd_produto == x.cd_produto &&
                                                                                                                       c.cd_estagio == x.cd_estagio))
                                                  select f.cd_funcionario).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<ProfsemDiarioUI> getProfessorsemDiario(SearchParameters parametros, int cd_turma, int cd_professor, int cdEscola, bool idLiberado)
        {
            IEntitySorter<ProfsemDiarioUI> sorter = EntitySorter<ProfsemDiarioUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
            IQueryable<ProfsemDiarioUI> sql;
            List<ProfsemDiarioUI> retorno = new List<ProfsemDiarioUI>();
            try
            {
                sql = from a in db.vi_raf_sem_diario.AsNoTracking()
                      where a.id_liberado == idLiberado
                      select new ProfsemDiarioUI
                      {
                          cd_professor = a.cd_professor,
                          cd_turma = a.cd_turma,
                          dc_reduzido_pessoa = a.dc_reduzido_pessoa,
                          no_professor = a.no_professor,
                          cd_pessoa_empresa = a.cd_pessoa_escola,
                          id_liberado = a.id_liberado,
                          no_turma = a.no_turma,
                          qtd_programacao = (int)a.qtd_programacao
                      };
                if (cdEscola != 0)
                    sql = from a in sql
                          where a.cd_pessoa_empresa == cdEscola
                          select a;
                if (cd_turma != 0)
                    sql = from a in sql
                          where a.cd_turma == cd_turma
                          select a;
                if (cd_professor != 0)
                    sql = from a in sql
                          where a.cd_professor == cd_professor
                          select a;
                
                sql = sql.Distinct();

                sql = sorter.Sort(sql);


                int limite = sql.Count();

                parametros.ajustaParametrosPesquisa(limite);
                parametros.qtd_limite = limite;
                sql = sql.Skip(parametros.from).Take(parametros.qtd_limite);

                return sql.Distinct();

            }
            catch (Exception exe)
            {

                throw new DataAccessException(exe);
            }
        }

        public string postLiberarProfessor(int cd_professor, int cd_usuario, int fuso)
        {
            try
            {

                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = System.Data.CommandType.StoredProcedure;
                command.CommandText = @"sp_liberar_professor";
                command.CommandTimeout = 180;

                var sqlParameters = new List<System.Data.SqlClient.SqlParameter>();

                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_professor", cd_professor));

                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@cd_usuario", cd_usuario));

                    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", fuso));
                //else
                //    sqlParameters.Add(new System.Data.SqlClient.SqlParameter("@fuso", DBNull.Value));

                var parameterm = new System.Data.SqlClient.SqlParameter("@mensagem", System.Data.SqlDbType.VarChar, 1024);
                parameterm.Direction = System.Data.ParameterDirection.Output;

                sqlParameters.Add(parameterm);

                var parameter = new System.Data.SqlClient.SqlParameter("@result", System.Data.SqlDbType.Int);
                parameter.Direction = System.Data.ParameterDirection.ReturnValue;

                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                //var retunvalue = Convert.ToString((int)command.Parameters["@result"].Value);
                string retunvalue = null;
                retunvalue = (string)command.Parameters["@mensagem"].Value;

                db.Database.Connection.Close();
                return retunvalue;
            }
            catch (System.Data.SqlClient.SqlException exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
            catch (Exception exe)
            {
                db.Database.Connection.Close();
                throw new DataAccessException(exe);
            }
        }
    }
}
