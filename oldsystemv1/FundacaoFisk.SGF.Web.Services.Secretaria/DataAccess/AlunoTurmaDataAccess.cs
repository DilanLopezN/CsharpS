using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class AlunoTurmaDataAccess : GenericRepository<AlunoTurma>, IAlunoTurmaDataAccess
    {


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AlunoTurma> findAlunosTurmaPorTurmaEscola(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = from alunoT in db.AlunoTurma
                          where alunoT.cd_turma == cd_turma && alunoT.Aluno.cd_pessoa_escola == cd_escola
                          select alunoT;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                          where alunosT.cd_turma == cd_turma && alunosT.Aluno.cd_pessoa_escola == cd_escola
                          select new
                                           {
                                               cd_aluno_turma = alunosT.cd_aluno_turma,
                                               cd_turma = alunosT.cd_turma,
                                               cd_aluno = alunosT.cd_aluno,
                                               no_aluno = alunosT.Aluno.AlunoPessoaFisica.no_pessoa,
                                               dt_movimento = alunosT.dt_movimento,
                                               nm_aulas_dadas = alunosT.nm_aulas_dadas,
                                               nm_faltas = alunosT.nm_faltas,
                                               frequencia = alunosT.nm_aulas_dadas != 0 && alunosT.nm_aulas_dadas > alunosT.nm_faltas ? (alunosT.nm_aulas_dadas - alunosT.nm_faltas) * 100 / alunosT.nm_aulas_dadas : 0
                                           }).ToList().Select(x => new AlunoTurma
                                           {
                                           cd_aluno_turma = x.cd_aluno_turma,
                                           cd_turma = x.cd_turma,
                                           cd_aluno = x.cd_aluno,
                                           no_aluno = x.no_aluno,
                                           dt_movimento = x.dt_movimento,
                                           nm_aulas_dadas = x.nm_aulas_dadas,
                                           nm_faltas = x.nm_faltas,
                                           frequencia = x.frequencia
                                       });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> findAlunosTurmaHist(int cd_turma, int cd_escola,int[] cdAlunos)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                          where alunosT.cd_turma == cd_turma && alunosT.Aluno.cd_pessoa_escola == cd_escola &&
                                cdAlunos.Contains(alunosT.cd_aluno)
                           select new
                           {
                               alunosT.cd_turma,
                               alunosT.cd_aluno,
                               alunosT.cd_aluno_turma,
                               alunosT.dt_matricula,
                               alunosT.dt_desistencia,
                               alunosT.dt_movimento,
                               alunosT.id_tipo_movimento,
                               alunosT.id_reprovado,
                               alunosT.nm_aulas_dadas,
                               alunosT.dt_transferencia,
                               alunosT.cd_situacao_aluno_turma,
                               alunosT.dt_inicio,
                               alunosT.nm_faltas,
                               alunosT.cd_contrato,
                               alunosT.nm_matricula_turma,
                               alunosT.id_manter_contrato,
                               alunosT.id_renegociacao,
                               alunosT.cd_turma_origem,
                               alunosT.cd_situacao_aluno_origem,
                               alunosT.Turma.dt_inicio_aula
                           }).ToList().Select(x => new AlunoTurma
                           {
                               cd_turma = x.cd_turma,
                               cd_aluno = x.cd_aluno,
                               cd_aluno_turma = x.cd_aluno_turma,
                               dt_matricula = x.dt_matricula,
                               dt_desistencia = x.dt_desistencia,
                               dt_movimento = x.dt_movimento,
                               id_tipo_movimento = x.id_tipo_movimento,
                               id_reprovado = x.id_reprovado,
                               nm_aulas_dadas = x.nm_aulas_dadas,
                               dt_transferencia = x.dt_transferencia,
                               cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                               dt_inicio = x.dt_inicio,
                               nm_faltas = x.nm_faltas,
                               cd_contrato = x.cd_contrato,
                               nm_matricula_turma = x.nm_matricula_turma,
                               id_manter_contrato = x.id_manter_contrato,
                               id_renegociacao = x.id_renegociacao,
                               cd_turma_origem = x.cd_turma_origem,
                               cd_situacao_aluno_origem = x.cd_situacao_aluno_origem,
                               Turma = new Turma
                               {
                                   dt_inicio_aula = x.dt_inicio_aula
                               }

                           }
                          );

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola, int[] cdAlunos)
        {
            try
            {
                var sql = from alunosT in db.AlunoTurma.Include("Turma").Include("Turma.Curso")
                          where alunosT.cd_turma == cd_turma && alunosT.Aluno.cd_pessoa_escola == cd_escola &&
                                cdAlunos.Contains(alunosT.cd_aluno)
                          select alunosT;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool getVerificaAlunosTurma(List<int> listaAlunos, int cd_turma)
        {
            try
            {

                var sql = from alunosT in db.AlunoTurma
                          where alunosT.cd_turma == cd_turma &&
                                (listaAlunos.Contains(alunosT.cd_aluno) && alunosT.cd_situacao_aluno_turma != (int)AlunoTurma.SituacaoAlunoTurma.Movido)
                          select alunosT;

                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoTurma findAlunoTurma(int cd_aluno, int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                           where
                             alunosT.cd_turma == cd_turma &&
                             alunosT.Aluno.cd_pessoa_escola == cd_escola &&
                             alunosT.cd_aluno == cd_aluno
                           select new
                           {
                               alunosT.cd_turma,
                               alunosT.cd_aluno,
                               alunosT.cd_aluno_turma,
                               alunosT.dt_matricula,
                               alunosT.dt_desistencia,
                               alunosT.dt_movimento,
                               alunosT.id_tipo_movimento,
                               alunosT.id_reprovado,
                               alunosT.nm_aulas_dadas,
                               alunosT.dt_transferencia,
                               alunosT.cd_situacao_aluno_turma,
                               alunosT.dt_inicio,
                               alunosT.nm_faltas,
                               alunosT.cd_contrato,
                               alunosT.nm_matricula_turma,
                               alunosT.id_manter_contrato,
                               alunosT.id_renegociacao,
                               alunosT.cd_turma_origem,
                               alunosT.cd_situacao_aluno_origem,
                               alunosT.Turma.dt_inicio_aula,
                               alunosT.Turma.dt_termino_turma,
                               alunosT.Turma.Curso.id_permitir_matricula,
                               alunosT.Turma.Curso.cd_curso,
                               alunosT.Turma.cd_regime
                           }).ToList().Select(x => new AlunoTurma
                           {
                               cd_turma = x.cd_turma,
                               cd_aluno = x.cd_aluno,
                               cd_aluno_turma = x.cd_aluno_turma,
                               dt_matricula = x.dt_matricula,
                               dt_desistencia = x.dt_desistencia,
                               dt_movimento = x.dt_movimento,
                               id_tipo_movimento = x.id_tipo_movimento,
                               id_reprovado = x.id_reprovado,
                               nm_aulas_dadas = x.nm_aulas_dadas,
                               dt_transferencia = x.dt_transferencia,
                               cd_situacao_aluno_turma = x.cd_situacao_aluno_turma,
                               dt_inicio = x.dt_inicio,
                               nm_faltas = x.nm_faltas,
                               cd_contrato = x.cd_contrato,
                               nm_matricula_turma = x.nm_matricula_turma,
                               id_manter_contrato = x.id_manter_contrato,
                               id_renegociacao = x.id_renegociacao,
                               cd_turma_origem = x.cd_turma_origem,
                               cd_situacao_aluno_origem = x.cd_situacao_aluno_origem,
                               Turma = new Turma
                               {
                                   dt_inicio_aula = x.dt_inicio_aula,
                                   dt_termino_turma = x.dt_termino_turma,
                                   Curso = new Curso { 
                                       id_permitir_matricula = x.id_permitir_matricula,
                                       cd_curso = x.cd_curso
                                   },
                                   cd_regime = x.cd_regime
                               }

                           }).FirstOrDefault();

         
    
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoTurma findAlunoTurmaByCdCursoContrato(int cd_curso_contrato, int cd_escola)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                           where
                             alunosT.cd_curso_contrato == cd_curso_contrato &&
                             alunosT.Turma.cd_pessoa_escola == cd_escola 
                           select alunosT).FirstOrDefault();



                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<AlunoTurma> existsAlunosTurmaInTurmaDestino(List<int> cdsAlunosTurma, int cdTurmaDestino)
        {
            try
            {
                var sql = (from alunosTurma in db.AlunoTurma
                    where cdsAlunosTurma.Contains(alunosTurma.cd_aluno) &&
                          alunosTurma.cd_turma == cdTurmaDestino

                           select new
                    {
                        cd_aluno_turma = alunosTurma.cd_aluno_turma,
                        cd_turma = alunosTurma.cd_turma,
                        cd_aluno = alunosTurma.cd_aluno,
                        no_aluno = alunosTurma.Aluno.AlunoPessoaFisica.no_pessoa,
                    }).ToList().Select(x => new AlunoTurma
                    {
                        cd_aluno_turma = x.cd_aluno_turma,
                        cd_turma = x.cd_turma,
                        cd_aluno = x.cd_aluno,
                        no_aluno = x.no_aluno,
                    }).ToList();



                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int findAlunoTurmaProduto(int cd_aluno, int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                           where
                             alunosT.cd_turma == cd_turma &&
                             alunosT.Aluno.cd_pessoa_escola == cd_escola &&
                             alunosT.cd_aluno == cd_aluno
                           select alunosT.Turma.cd_produto).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool deleteAlunoAguardandoTurma(int cdProduto, int cdEscola, int cdContrato, int cdAluno, int cd_turma)
        {
            try
            {
                int retorno = 0;
                var sql = (from alunosT in db.AlunoTurma
                          where
                            alunosT.cd_situacao_aluno_turma == 9 &&
                            alunosT.Aluno.cd_pessoa_escola == cdEscola &&
                            alunosT.Turma.cd_produto == cdProduto &&
                            alunosT.Aluno.cd_aluno == cdAluno &&
                            (alunosT.cd_contrato != cdContrato || alunosT.cd_contrato == null) &&
                            (alunosT.Turma.cd_turma_ppt == null || (alunosT.Turma.cd_turma_ppt != null && alunosT.cd_turma == cd_turma))
                          select alunosT).ToList();
                if (sql != null && sql.Count() > 0)
                    db.AlunoTurma.RemoveRange(sql);
                //string strAlunoTurma = "";
                //if (sql != null && sql.Count() > 0)
                //    foreach (AlunoTurma e in sql.ToList())
                //        strAlunoTurma += e.cd_aluno_turma + ",";

                //// Remove o último ponto e virgula:
                //if (strAlunoTurma.Length > 0)
                //{
                //    strAlunoTurma = strAlunoTurma.Substring(0, strAlunoTurma.Length - 1);

                //    retorno = db.Database.ExecuteSqlCommand("delete from t_aluno_turma where cd_aluno_turma in(" + strAlunoTurma + ")");
                //}
                return retorno > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //verifica se existe aluno na turma com cortrato
        public bool existsAlunoTurmaByContratoEscola(int cd_contrato, int cd_pessoa_escola)
        {
            try
            {
                var sql = (from alunoTurma in db.AlunoTurma
                           where alunoTurma.cd_contrato == cd_contrato
                              && alunoTurma.Turma.cd_pessoa_escola == cd_pessoa_escola
                           select alunoTurma.cd_aluno_turma).Any();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<AlunoTurma> findAlunoTurmasByContratoEscola(int cd_contrato, int cd_pessoa_escola)
        {
            try
            {
                var sql = (from alunoTurma in db.AlunoTurma//.Include(al => al.Turma)
                           where alunoTurma.cd_contrato == cd_contrato
                              && alunoTurma.Turma.cd_pessoa_escola == cd_pessoa_escola
                           select alunoTurma).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int existAlunoMatriculadoOuRematriculado(int cd_aluno,int cd_escola,int cd_turma)
        {
            try
            {
                var sql = (from alunoTurma in db.AlunoTurma
                           where alunoTurma.Turma.cd_pessoa_escola == cd_escola && alunoTurma.cd_aluno == cd_aluno &&
                                 alunoTurma.Turma.cd_turma != cd_turma &&
                                 alunoTurma.Turma.dt_termino_turma == null && alunoTurma.Turma.id_turma_ppt == false &&
                                 (alunoTurma.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                  alunoTurma.cd_situacao_aluno_turma == (int)  AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                           select alunoTurma.cd_aluno_turma).Count();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<AlunoTurma> findAlunosTurmaForEncerramento(int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from alunoT in db.AlunoTurma.Include(x=> x.Aluno)
                          where alunoT.cd_turma == cd_turma && alunoT.Turma.cd_pessoa_escola == cd_escola 
                               //&& (alunoT.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo || alunoT.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                          select alunoT).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> getAlunosTurmaAtivosDiarioAula(int cd_turma, int cd_pessoa_escola, DateTime dtAula)
        {
            try
            {
                var sql = from at in db.AlunoTurma.Include(a => a.Turma)
                          where at.Turma.cd_pessoa_escola == cd_pessoa_escola &&
                                at.cd_turma == cd_turma &&
                                at.Aluno.HistoricoAluno.Where(
                                    ha => ha.cd_aluno == at.cd_aluno &&
                                     ha.cd_turma == cd_turma &&
                                     (DbFunctions.TruncateTime(ha.dt_historico) <= dtAula.Date &&
                                      (ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                       ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado ||
                                       ha.id_situacao_historico == (int)AlunoTurma.SituacaoAlunoTurma.Encerrado)
                                        && ha.nm_sequencia == at.Aluno.HistoricoAluno.Where(han => han.cd_turma == cd_turma
                                                                                           && DbFunctions.TruncateTime(han.dt_historico) <= dtAula.Date).Max(x=> x.nm_sequencia)
                                      )
                                    ).Any()
                          select at;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public int? getStatusAlunoTurma(int cd_aluno, int cd_escola, int cd_turma)
        {
            try
            {
                var sql = (from alunoTurma in db.AlunoTurma
                           where //alunoTurma.Turma.cd_pessoa_escola == cd_escola
                               alunoTurma.cd_aluno == cd_aluno
                               && alunoTurma.Turma.cd_turma == cd_turma
                           select alunoTurma.cd_situacao_aluno_turma).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoTurma findAlunoTurmaContrato(int cd_aluno, int cd_turma, int cd_escola, int cd_contrato)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                        .Include("CursoContrato")
                           where
                             alunosT.cd_turma == cd_turma &&
                             //alunosT.Turma.cd_pessoa_escola == cd_escola &&
                             alunosT.cd_aluno == cd_aluno &&
                             alunosT.cd_contrato == cd_contrato
                           select alunosT).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        public AlunoTurma findAlunoTurmaMovido(int cd_aluno, int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from alunosT in db.AlunoTurma
                           where
                             alunosT.cd_turma == cd_turma &&
                             //alunosT.Turma.cd_pessoa_escola == cd_escola &&
                             alunosT.cd_aluno == cd_aluno &&
                             alunosT.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Movido
                           select alunosT).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> getAlunoTurmaByCdContrato(int cd_contrato)
        {
            try
            {
                var sql = from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                    where at.cd_contrato == cd_contrato &&
                          //at.Turma.dt_termino_turma == null && at.Turma.id_turma_ppt == false &&
                          (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                          select at;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AlunoTurma> getAlunoTurmaByCdContratoAndCdAluno(int cd_contrato, int cd_aluno)
        {
            try
            {
                var sql = from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                          where at.cd_contrato == cd_contrato && at.cd_aluno == cd_aluno &&
                                //at.Turma.dt_termino_turma == null && at.Turma.id_turma_ppt == false &&
                                (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                                 at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                          select at;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public AlunoApiCyberBdUI findAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato)
        {
            try
            {
                var sql = (from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                    where at.cd_contrato == cd_contrato && at.cd_turma == cd_turma && at.cd_aluno == cd_aluno &&
                          //at.Turma.dt_termino_turma == null && at.Turma.id_turma_ppt == false &&
                          (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                    select new
                    {
                        nome = at.Aluno.AlunoPessoaFisica.no_pessoa,
                        codigo = at.cd_aluno,
                        id_unidade = (at.Aluno.EscolaAluno.cd_empresa_coligada == null ? at.Aluno.EscolaAluno.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where  e.cd_pessoa == at.Aluno.EscolaAluno.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()) ,
                        id_aluno_ativo = at.Aluno.id_aluno_ativo,
                        email = ((from t in db.TelefoneSGF where t.cd_pessoa == at.Aluno.cd_pessoa_aluno && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault() != null ?
                            (from t in db.TelefoneSGF where t.cd_pessoa == at.Aluno.cd_pessoa_aluno && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault().dc_fone_mail : ""),

                    }).ToList().Select(x => new AlunoApiCyberBdUI()
                    {
                        nome = x.nome,
                        codigo = x.codigo,
                        id_unidade = x.id_unidade,
                        aluno_ativo = x.id_aluno_ativo,
                        email = x.email

                    }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public PromocaoIntercambioParams findAlunoApiPromocaoIntercambio(int cd_aluno, int id_tipo_matricula)
        {
            try
            {
                
                var sql = (from a in db.Aluno
                           join pf in db.PessoaSGF.OfType<PessoaFisicaSGF>().AsNoTracking() on a.cd_pessoa_aluno equals pf.cd_pessoa
                           where a.id_aluno_ativo == true && a.cd_aluno == cd_aluno
                           select new
                           {
                               cpf = pf.nm_cpf != null ? pf.nm_cpf : pf.PessoaSGFQueUsoOCpf.nm_cpf,
                               email = ((from t in db.TelefoneSGF where t.cd_pessoa == a.cd_pessoa_aluno && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault() != null ?
                                   (from t in db.TelefoneSGF where t.cd_pessoa == a.cd_pessoa_aluno && t.cd_tipo_telefone == 4 && t.id_telefone_principal == true select t).FirstOrDefault().dc_fone_mail : ""),
                               telefone = pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).Any() ? pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.TELEFONE).FirstOrDefault().dc_fone_mail : pf.TelefonePessoa.Where(t => t.cd_tipo_telefone == (int)TipoTelefoneSGF.TipoTelefoneSGFEnum.CELULAR).FirstOrDefault().dc_fone_mail,
                               tipo = id_tipo_matricula.ToString(),
                               id_unidade = (a.EscolaAluno.cd_empresa_coligada == null ? a.EscolaAluno.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == a.EscolaAluno.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()),

                           }).ToList().Select(x => new PromocaoIntercambioParams()
                           {
                               cpf = x.cpf,
                               email = x.email,
                               telefone = x.telefone,
                               tipo = x.tipo,
                               unidade = x.id_unidade != null ? x.id_unidade.ToString() : ""

                           }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }


        public LivroAlunoApiCyberBdUI findLivroAlunoApiCyber(int cd_aluno, int cd_turma, int cd_contrato)
        {
            try
            {
                var sql = (from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                    where at.cd_contrato == cd_contrato && at.cd_turma == cd_turma && at.cd_aluno == cd_aluno &&
                          //at.Turma.dt_termino_turma == null && at.Turma.id_turma_ppt == false &&
                          (at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Ativo ||
                           at.cd_situacao_aluno_turma == (int)AlunoTurma.SituacaoAlunoTurma.Rematriculado)
                    select new
                    {
                        codigo_aluno = at.cd_aluno,
                        codigo_grupo = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.cd_turma : at.Turma.cd_turma,
                        nome_turma = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.no_turma : at.Turma.no_turma,
                        codigo_livro = (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).Any()?(from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).FirstOrDefault().cd_livro_cyber: 0,
                        codigo_unidade = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ?
                            (at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada == null ?  at.Turma.TurmaPai.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa ==  at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()):
                            (at.Turma.EscolaTurma.cd_empresa_coligada == null ?  at.Turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa ==  at.Turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault())


                    }).ToList().Select(x => new LivroAlunoApiCyberBdUI()
                        {
                            codigo_aluno = x.codigo_aluno,
                            codigo_grupo = x.codigo_grupo,
                            codigo_livro = x.codigo_livro,
                            codigo_unidade = x.codigo_unidade,
                            nome_turma = x.nome_turma

                    }).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public LivroAlunoApiCyberBdUI findLivroAlunoTurmaApiCyber(int cd_aluno, int cd_turma, int cd_escola)
        {
            try
            {
                var sql = (from at in db.AlunoTurma.Include(a => a.Turma).Include(x => x.Aluno)
                    where at.Aluno.cd_pessoa_escola  == cd_escola && at.cd_turma == cd_turma && at.cd_aluno == cd_aluno 
                           select new
                    {
                        codigo_aluno = at.cd_aluno,
                        codigo_grupo = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.cd_turma : at.Turma.cd_turma,
                        nome_turma = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ? at.Turma.TurmaPai.no_turma : at.Turma.no_turma,
                        codigo_livro = (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).Any() ? (from lc in db.LivroCyber where lc.cd_estagio == at.Turma.Curso.Estagio.cd_estagio select lc).FirstOrDefault().cd_livro_cyber : 0,
                        codigo_unidade = (at.Turma.cd_turma_ppt != null && at.Turma.cd_turma_ppt > 0 && at.Turma.id_turma_ppt == false) ?
                            (at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada == null ? at.Turma.TurmaPai.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == at.Turma.TurmaPai.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault()):
                            (at.Turma.EscolaTurma.cd_empresa_coligada == null ? at.Turma.EscolaTurma.nm_cliente_integracao : (from e in db.PessoaSGF.OfType<Escola>() where e.cd_pessoa == at.Turma.EscolaTurma.cd_empresa_coligada select e.nm_cliente_integracao).FirstOrDefault())


                    }).ToList().Select(x => new LivroAlunoApiCyberBdUI()
                {
                    codigo_aluno = x.codigo_aluno,
                    codigo_grupo = x.codigo_grupo,
                    codigo_livro = x.codigo_livro,
                    codigo_unidade = x.codigo_unidade,
                    nome_turma = x.nome_turma

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
