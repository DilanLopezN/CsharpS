using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AvaliacaoTurmaDataAccess : GenericRepository<AvaliacaoTurma>, IAvaliacaoTurmaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AvaliacaoTurmaUI> searchAvaliacaoTurma(Componentes.Utils.SearchParameters parametros, int idTurma, bool? idTipoAvaliacao, int idEscola,
      int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, DateTime? dta_inicial, DateTime? dta_final, int cd_escola_combo)
        {
            try
            {
                IEntitySorter<AvaliacaoTurmaUI> sorter = EntitySorter<AvaliacaoTurmaUI>.OrderBy(parametros.sort, (Componentes.GenericDataAccess.SortDirection)(int)parametros.sortOrder);
                IQueryable<AvaliacaoTurmaUI> retorno;
                IQueryable<AvaliacaoTurma> sql;

                sql = from avaliacaoTurma in db.AvaliacaoTurma.AsNoTracking()
                        where (avaliacaoTurma.Turma.cd_pessoa_escola == idEscola ||
                            (from te in db.TurmaEscola
                                where te.cd_turma == avaliacaoTurma.Turma.cd_turma &&
                                        te.cd_escola == idEscola
                                select te).Any())
                    select avaliacaoTurma;


                if(dta_inicial.HasValue)
                    sql = from at in sql
                          where (at.dt_avaliacao_turma >= dta_inicial)
                          select at;
                if(dta_final.HasValue)
                    sql = from at in sql
                          where (at.dt_avaliacao_turma <= dta_final)
                          select at;
                
                if (idTurma > 0)
                    sql = from turma in sql
                          where turma.cd_turma == idTurma
                          select turma;

                if (idTipoAvaliacao != null)
                    sql = from criterio in sql
                          where criterio.Avaliacao.CriterioAvaliacao.id_conceito == idTipoAvaliacao
                          select criterio;


                if (cd_tipo_avaliacao > 0)
                    sql = from tipo in sql
                          where tipo.Avaliacao.cd_tipo_avaliacao == cd_tipo_avaliacao
                          select tipo;

                if (cd_criterio_avaliacao > 0)
                    sql = from criterio in sql
                          where criterio.Avaliacao.CriterioAvaliacao.cd_criterio_avaliacao == cd_criterio_avaliacao
                          select criterio;

                if (cd_curso > 0)
                    sql = from curso in sql
                          where curso.Turma.cd_curso == cd_curso
                          select curso;

                if (cd_funcionario == 0)
                {
                    retorno = (from avaliacaoTurma in sql
                        join turma in db.Turma on avaliacaoTurma.cd_turma equals turma.cd_turma
                        join avaliacao in db.Avaliacao on avaliacaoTurma.cd_avaliacao equals avaliacao.cd_avaliacao
                        where  (turma.cd_pessoa_escola == idEscola ||
                                (from te in db.TurmaEscola
                                    where te.cd_turma == turma.cd_turma &&
                                            te.cd_escola == idEscola
                                    select te).Any()) 
                        select new AvaliacaoTurmaUI
                        {
                            cd_avaliacao_turma = turma.cd_turma,
                            cd_turma = turma.cd_turma,
                            no_turma = turma.no_turma,
                            no_tipo_criterio = (avaliacao.CriterioAvaliacao.id_conceito == true ? "Conceito" : turma.Curso.Estagio.no_estagio_abreviado == "IFC" ? "Pontos" : "Notas"),
                            isConceito = avaliacao.CriterioAvaliacao.id_conceito,
                            isModified = false,
                            cd_produto = turma.cd_produto,
                            cd_curso = turma.cd_curso,
                            no_produto = turma.Produto.no_produto,
                            no_curso = turma.Curso.no_curso,
                            isInFocus = turma.Curso.Estagio.no_estagio_abreviado == "IFC"
                        }).Distinct();
                }
                else
                {
                        retorno = (from avaliacaoTurma in sql
                            join turma in db.Turma on avaliacaoTurma.cd_turma equals turma.cd_turma
                            join avaliacao in db.Avaliacao on avaliacaoTurma.cd_avaliacao equals avaliacao.cd_avaliacao
                            join professor in db.ProfessorTurma on turma.cd_turma equals professor.cd_turma
                                   where (turma.cd_pessoa_escola == idEscola ||
                                  (from te in db.TurmaEscola
                                      where te.cd_turma == turma.cd_turma &&
                                            te.cd_escola == idEscola
                                      select te).Any()) &&
                                  professor.cd_professor == cd_funcionario
                            select new AvaliacaoTurmaUI
                            {
                                cd_avaliacao_turma = turma.cd_turma,
                                cd_turma = turma.cd_turma,
                                no_turma = turma.no_turma,
                                no_tipo_criterio = (avaliacao.CriterioAvaliacao.id_conceito == true ? "Conceito" : turma.Curso.Estagio.no_estagio_abreviado == "IFC" ? "Pontos" : "Notas"),
                                isConceito = avaliacao.CriterioAvaliacao.id_conceito,
                                isModified = false,
                                cd_produto = turma.cd_produto,
                                cd_curso = turma.cd_curso,
                                no_produto = turma.Produto.no_produto,
                                no_curso = turma.Curso.no_curso,
                                isInFocus = turma.Curso.Estagio.no_estagio_abreviado == "IFC"
                            }).Distinct();
                    
                }
                    

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

        public List<AvaliacaoTurma> getAvaliacaoTurmaArvore(int idTurma, int idEscola, bool? idConceito, int? cdFuncionario, int cd_tipo_avaliacao)
        { 
            try
            {
                IEnumerable<AvaliacaoTurma> retorno = (from avaT in db.AvaliacaoTurma
                                                       where (avaT.cd_turma == idTurma)
                                                                 //&& ((avaT.Turma.cd_pessoa_escola == idEscola) ||
                                                                 //    db.AvaliacaoAluno.Where(x => x.Aluno.cd_pessoa_escola == idEscola).Any())
                                                                 && (idConceito == null || avaT.Avaliacao.CriterioAvaliacao.id_conceito == idConceito)
                                                                 && (avaT.Avaliacao.cd_tipo_avaliacao == cd_tipo_avaliacao)
                                                       orderby avaT.Avaliacao.nm_ordem_avaliacao ascending
                                                       select new
                                                       {
                                                           nm_ordem_avaliacao = avaT.Avaliacao.nm_ordem_avaliacao,
                                                           id_conceito = avaT.Avaliacao.CriterioAvaliacao.id_conceito,
                                                           TurmaProfessorTurma = avaT.Turma.TurmaProfessorTurma,
                                                           cd_avaliacao_turma = avaT.cd_avaliacao_turma,
                                                           AvaliacaoAluno = avaT.AvaliacaoAluno,
                                                           AvaliacaoAlunoAluno = avaT.AvaliacaoAluno.Select(a => a.Aluno),
                                                           AvaliacaoAlunoAlunoHistoricoAluno = avaT.AvaliacaoAluno.Select(a => a.Aluno.HistoricoAluno),
                                                           cd_tipo_avaliacao = avaT.AvaliacaoAluno.Select(a => a.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao),
                                                           AvaliacaoAlunoAlunoTurma = avaT.AvaliacaoAluno.Select(a => a.Aluno.AlunoTurma),
                                                           //AvaliacoesAlunoParticipacao = avaT.AvaliacaoAluno.Select(a => a.AvaliacoesAlunoParticipacao.Select(b => b.Conceito)),
                                                           AvaliacaoAlunoAlunoPessoaFisica = avaT.AvaliacaoAluno.Select(a => a.Aluno.AlunoPessoaFisica),
                                                           AvaliacaoAlunoConceito = avaT.AvaliacaoAluno.Select(a => a.Conceito),
                                                           nm_peso_avaliacao = avaT.Avaliacao.nm_peso_avaliacao,
                                                           tx_obs_aval_turma = avaT.tx_obs_aval_turma,
                                                           no_pessoa = avaT.Funcionario.FuncionarioPessoaFisica.no_pessoa,
                                                           vl_nota = avaT.Avaliacao.vl_nota,
                                                           cd_funcionario = (int?)avaT.Funcionario.cd_funcionario,
                                                           dt_avaliacao_turma = avaT.dt_avaliacao_turma,
                                                           cd_avaliacao = avaT.cd_avaliacao,
                                                           cd_produto = avaT.Turma.cd_produto,
                                                           id_avaliacao_ativa = avaT.Avaliacao.id_avaliacao_ativa,
                                                           dt_termino_turma = avaT.Turma.dt_termino_turma,
                                                           dc_criterio_avaliacao = avaT.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                                                           dc_criterio_abreviado = avaT.Avaliacao.CriterioAvaliacao.dc_criterio_abreviado,
                                                           id_participacao = avaT.Avaliacao.CriterioAvaliacao.id_participacao,
                                                           AvaliacaoParticipacao = avaT.Avaliacao.CriterioAvaliacao.AvaliacaoParticipacao.Where(ap => ap.cd_produto == avaT.Turma.cd_produto),
                                                           cd_pessoa_escola = avaT.Turma.cd_pessoa_escola
                                                       }).ToList().Select(x => new AvaliacaoTurma
                                                       {
                                                           Avaliacao = new Avaliacao()
                                                           {
                                                               nm_ordem_avaliacao = x.nm_ordem_avaliacao,
                                                               CriterioAvaliacao = new CriterioAvaliacao()
                                                               {
                                                                   id_conceito = x.id_conceito,
                                                                   dc_criterio_avaliacao = x.dc_criterio_avaliacao,
                                                                   dc_criterio_abreviado = x.dc_criterio_abreviado,
                                                                   id_participacao = x.id_participacao,
                                                                   AvaliacaoParticipacao = (from avaP in x.AvaliacaoParticipacao
                                                                                            select new {
                                                                                                id_avaliacao_participacao_ativa = avaP.id_avaliacao_participacao_ativa,
                                                                                                cd_avaliacao_participacao = avaP.cd_avaliacao_participacao,
                                                                                                AvaliacaoParticipacaoVinc = avaP.AvaliacaoParticipacaoVinc
                                                                                            }).ToList().Select(y => new AvaliacaoParticipacao{
                                                                                                   id_avaliacao_participacao_ativa = y.id_avaliacao_participacao_ativa,
                                                                                                   AvaliacaoParticipacaoVinc = (from vinc in db.AvaliacaoParticipacaoVinc 
                                                                                                                                where vinc.cd_escola == idEscola
                                                                                                                                && vinc.cd_avaliacao_participacao == y.cd_avaliacao_participacao
                                                                                                                                orderby vinc.nm_ordem ascending
                                                                                                                                select new
                                                                                                                                {
                                                                                                                                    id_avaliacao_participacao_ativa = vinc.id_avaliacao_participacao_ativa,
                                                                                                                                    ParticipacaoAvaliacao = vinc.ParticipacaoAvaliacao,
                                                                                                                                    //AvaliacoesAlunoParticipacao = vinc.ParticipacaoAvaliacao.AvaliacoesAlunoParticipacao.Where(b => b.AvaliacaoAluno.cd_turma == idTurma )
                                                                                                                                }).ToList().Select(z => new AvaliacaoParticipacaoVinc
                                                                                                                                {
                                                                                                                                    id_avaliacao_participacao_ativa = z.id_avaliacao_participacao_ativa,
                                                                                                                                    ParticipacaoAvaliacao = new Participacao()
                                                                                                                                        {
                                                                                                                                            cd_participacao = z.ParticipacaoAvaliacao.cd_participacao,
                                                                                                                                            id_participacao_ativa = z.ParticipacaoAvaliacao.id_participacao_ativa,
                                                                                                                                            no_participacao = z.ParticipacaoAvaliacao.no_participacao,
                                                                                                                                           // AvaliacoesAlunoParticipacao = z.ParticipacaoAvaliacao.AvaliacoesAlunoParticipacao
                                                                                                                                        }
                                                                                                                                }).ToList()
                                                                                            }).ToList()
                                                               },
                                                               nm_peso_avaliacao = x.nm_peso_avaliacao,
                                                               vl_nota = x.vl_nota,
                                                               id_avaliacao_ativa = x.id_avaliacao_ativa,
                                                               cd_tipo_avaliacao = cd_tipo_avaliacao
                                                           },
                                                           Turma = new Turma()
                                                           {
                                                               TurmaProfessorTurma = x.TurmaProfessorTurma,
                                                               cd_produto = x.cd_produto,
                                                               cd_pessoa_escola = x.cd_pessoa_escola,
                                                               dt_termino_turma = x.dt_termino_turma
                                                           },
                                                           cd_avaliacao_turma = x.cd_avaliacao_turma,
                                                           AvaliacaoAluno = (from avaA in x.AvaliacaoAluno 
                                                                             select new
                                                                             {
                                                                                 
                                                                                 nm_nota_aluno = avaA.nm_nota_aluno,
                                                                                 id_segunda_prova = avaA.id_segunda_prova,
                                                                                 nm_nota_aluno_2 = avaA.nm_nota_aluno_2,
                                                                                 cd_avaliacao_aluno = avaA.cd_avaliacao_aluno,
                                                                                 cd_aluno = avaA.Aluno.cd_aluno,
                                                                                 cd_pessoa_escola = avaA.Aluno.cd_pessoa_escola,
                                                                                 //cd_tipo_avaliacao = avaA.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao,
                                                                                 cd_avaliacao_turma = avaA.cd_avaliacao_turma,
                                                                                 cd_conceito = avaA.cd_conceito,
                                                                                 tx_obs_nota_aluno = avaA.tx_obs_nota_aluno,
                                                                                 HistoricoAluno = avaA.Aluno.HistoricoAluno.Where(h => h.cd_turma == idTurma),
                                                                                 //AvaliacoesAlunoParticipacao = avaA.AvaliacoesAlunoParticipacao,
                                                                                 AlunoTurma = avaA.Aluno.AlunoTurma,
                                                                                 no_pessoa = avaA.Aluno.AlunoPessoaFisica.no_pessoa,
                                                                                 no_conceito = (avaA.Conceito != null) ? avaA.Conceito.no_conceito : ""
                                                                             }).ToList().Select(x2 => new AvaliacaoAluno()
                                                                             {
                                                                                 nm_nota_aluno = x2.nm_nota_aluno,
                                                                                 id_segunda_prova = x2.id_segunda_prova,
                                                                                 nm_nota_aluno_2 = x2.nm_nota_aluno_2,
                                                                                 cd_avaliacao_aluno = x2.cd_avaliacao_aluno,
                                                                                 AvaliacoesAlunoParticipacao =
                                                                                     (from avaP in db.AvaliacaoAlunoParticipacao
                                                                                          where 
                                                                                            avaP.AvaliacaoAluno.cd_aluno == x2.cd_aluno 
                                                                                            && avaP.AvaliacaoAluno.AvaliacaoTurma.cd_turma == idTurma
                                                                                            && avaP.AvaliacaoAluno.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao == cd_tipo_avaliacao
                                                                                      select new
                                                                                      {
                                                                                          cd_avaliacao_aluno = avaP.cd_avaliacao_aluno,
                                                                                          cd_avaliacao_aluno_participacao = avaP.cd_avaliacao_aluno_participacao,
                                                                                          cd_conceito_participacao = avaP.cd_conceito_participacao,
                                                                                          cd_participacao_avaliacao = avaP.cd_participacao_avaliacao,
                                                                                          vl_nota_participacao = (avaP.Conceito != null) ? avaP.Conceito.vl_nota_participacao : 0
                                                                                      }).ToList().Select(x4 => new AvaliacaoAlunoParticipacao()
                                                                                      {
                                                                                          cd_avaliacao_aluno = x4.cd_avaliacao_aluno,
                                                                                          cd_avaliacao_aluno_participacao = x4.cd_avaliacao_aluno_participacao,
                                                                                          cd_conceito_participacao = x4.cd_conceito_participacao,
                                                                                          cd_participacao_avaliacao = x4.cd_participacao_avaliacao,
                                                                                          vl_nota_participacao = x4.vl_nota_participacao
                                                                                      }).ToList(),
     //                                                                                 Union(
     //(from pv in db.AvaliacaoParticipacaoVinc
     // where pv.cd_escola == x2.cd_pessoa_escola &&
     //      //pv.AvaliacaoParticipacao.cd_produto == 1 &&
     //     pv.AvaliacaoParticipacao.CriterioAvaliacao.id_participacao &&
     //     !(from app in db.AvaliacaoAlunoParticipacao
     //       where
     //         app.AvaliacaoAluno.cd_aluno == x2.cd_aluno
     //         && app.AvaliacaoAluno.AvaliacaoTurma.cd_turma == idTurma
     //         && app.AvaliacaoAluno.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao == cd_tipo_avaliacao
     //       select app
     //     ).Any()
     // select new
     // {
     //     cd_avaliacao_aluno = 0,
     //     cd_avaliacao_aluno_participacao = 0,
     //     cd_conceito_participacao = 0,
     //     cd_participacao_avaliacao = pv.cd_participacao_avaliacao,
     //     vl_nota_participacao = 0
     // }).Select(x4 => new AvaliacaoAlunoParticipacao()
     // {
     //     cd_avaliacao_aluno = x4.cd_avaliacao_aluno,
     //     cd_avaliacao_aluno_participacao = x4.cd_avaliacao_aluno_participacao,
     //     cd_conceito_participacao = x4.cd_conceito_participacao,
     //     cd_participacao_avaliacao = x4.cd_participacao_avaliacao,
     //     vl_nota_participacao = x4.vl_nota_participacao
     // })).ToList(),

                Aluno = new Aluno()
                                                                                 {
                                                                                     cd_aluno = x2.cd_aluno,
                                                                                     cd_pessoa_escola = x2.cd_pessoa_escola,
                                                                                     AlunoPessoaFisica = new PessoaFisicaSGF()
                                                                                     {
                                                                                         no_pessoa = x2.no_pessoa
                                                                                     },
                                                                                     HistoricoAluno = (from hist in x2.HistoricoAluno
                                                                                                       select new
                                                                                                       {
                                                                                                           dt_cadastro = hist.dt_cadastro,
                                                                                                           nm_sequencia = hist.nm_sequencia
                                                                                                       }).ToList().Select(x3 => new HistoricoAluno()
                                                                                                       {
                                                                                                           dt_cadastro = x3.dt_cadastro,
                                                                                                           nm_sequencia = x3.nm_sequencia
                                                                                                       }).ToList(),
                                                                                     AlunoTurma = (from at in x2.AlunoTurma
                                                                                                   select new
                                                                                                   {
                                                                                                       dt_desistencia = at.dt_desistencia,
                                                                                                       dt_matricula = at.dt_matricula,
                                                                                                       dt_movimento = at.dt_movimento,
                                                                                                       dt_transferencia = at.dt_transferencia
                                                                                                   }).ToList().Select(x4 => new AlunoTurma()
                                                                                                   {
                                                                                                       dt_desistencia = x4.dt_desistencia,
                                                                                                       dt_matricula = x4.dt_matricula,
                                                                                                       dt_transferencia = x4.dt_transferencia
                                                                                                   }).ToList(),
                                                                                 },
                                                                                 cd_avaliacao_turma = x2.cd_avaliacao_turma,
                                                                                 cd_conceito = x2.cd_conceito,
                                                                                 tx_obs_nota_aluno = x2.tx_obs_nota_aluno,
                                                                                 no_conceito = x2.no_conceito
                                                                             }).ToList(),
                                                           tx_obs_aval_turma = x.tx_obs_aval_turma,
                                                           Funcionario = new FuncionarioSGF()
                                                           {
                                                               FuncionarioPessoaFisica = new PessoaFisicaSGF()
                                                               {
                                                                   no_pessoa = x.no_pessoa
                                                               },
                                                               cd_funcionario = x.cd_funcionario.HasValue ? x.cd_funcionario.Value : 0
                                                           },
                                                           dt_avaliacao_turma = x.dt_avaliacao_turma,
                                                           cd_avaliacao = x.cd_avaliacao
                                                      });

                return retorno.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool incluirAvaliacoesTurma(List<AvaliacaoTurma> novasAvaliacoes)
        {
            var incluido = false;
            try
            {
                if (novasAvaliacoes != null)
                {
                    foreach (var avaliacao in novasAvaliacoes)
                        base.add(avaliacao, false);
                    incluido = true;
                }
                return incluido;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public List<AvaliacaoTurmaUI> returnAvaliacoesConceitoOrNotaByTurma(int cd_turma, int cd_pessoa_escola)
        {
            try
            {
                var sql = (from avaliacaoTurma in db.AvaliacaoTurma
                           join avaliacao in db.Avaliacao on avaliacaoTurma.cd_avaliacao equals avaliacao.cd_avaliacao
                           join criterio in db.CriterioAvaliacao on avaliacao.cd_criterio_avaliacao equals criterio.cd_criterio_avaliacao
                           where avaliacaoTurma.cd_turma == cd_turma 
                              && avaliacaoTurma.Turma.cd_pessoa_escola == cd_pessoa_escola
                              && avaliacao.id_avaliacao_ativa
                              && avaliacao.TipoAvaliacao.id_tipo_ativo
                           select new AvaliacaoTurmaUI
                           {
                               cd_avaliacao = avaliacaoTurma.cd_avaliacao,
                               cd_turma = avaliacaoTurma.cd_turma,
                               isConceito = criterio.id_conceito,
                               is_conceito_nota = false,
                               cd_conceito = avaliacaoTurma.AvaliacaoAluno.Select(a => a.cd_conceito).FirstOrDefault(),
                               isInFocus = avaliacaoTurma.Turma.Curso.Estagio.no_estagio_abreviado == "IFC"
                           }).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        //Busca as avaliações da turma de uma turma que não existem para o determinado aluno:
        public IEnumerable<AvaliacaoTurma> returnAvaliacoesTurmaSemAvaliacoeAluno(int cd_turma, int cd_escola, int cd_pessoa_aluno)
        {
            try
            {
                var sql = from avalicaoTuma in db.AvaliacaoTurma
                          where avalicaoTuma.cd_turma == cd_turma
                                && !(from avaliacaoAluno in db.AvaliacaoAluno
                                     join aluno in db.Aluno on avaliacaoAluno.cd_aluno equals aluno.cd_aluno
                                     where avalicaoTuma.cd_avaliacao_turma == avaliacaoAluno.cd_avaliacao_turma
                                          && aluno.cd_pessoa_escola == cd_escola
                                          && aluno.cd_pessoa_aluno == cd_pessoa_aluno
                                     select avaliacaoAluno).Any()
                          select avalicaoTuma;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existsAvaliacaoTurmaByDesistencia(DateTime data, int cd_pessoa_escola, int cd_aluno, int cd_turma)
        {
            try
            {
                var sql = (from avaliacao in db.AvaliacaoTurma
                           where DbFunctions.TruncateTime(avaliacao.dt_avaliacao_turma) >= DbFunctions.TruncateTime(data)
                              && avaliacao.Turma.cd_pessoa_escola == cd_pessoa_escola
                              && avaliacao.Turma.cd_turma == cd_turma
                              && avaliacao.AvaliacaoAluno.Any(av => av.cd_aluno == cd_aluno && ((av.nm_nota_aluno.HasValue) || (av.nm_nota_aluno_2.HasValue)))
                           select avaliacao.cd_avaliacao).FirstOrDefault();

                return sql > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoTurma> GetAvaliacaoTurmaByIdAvaliacao(int cd_avaliacao, int cd_escola)
        {
            try
            {
                var sql = (from avalTurma in db.AvaliacaoTurma
                       join avalAluno in db.AvaliacaoAluno on avalTurma.cd_avaliacao_turma equals avalAluno.cd_avaliacao_turma
                       join turma in db.Turma on avalTurma.cd_turma equals turma.cd_turma
                       join curso in db.Curso on turma.cd_curso equals curso.cd_curso
                       where avalTurma.cd_avaliacao == cd_avaliacao
                          && turma.cd_pessoa_escola == cd_escola
                          && avalAluno.cd_conceito == null &&
                          avalTurma.cd_funcionario == null && avalTurma.dt_avaliacao_turma == null
                       select avalTurma);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public int gerarTurmasNulas(Nullable<int> cd_turma, Nullable<int> cd_tipo_avaliacao)
        {
            try
            {
                db.Database.Connection.Open();
                var command = db.Database.Connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = @"sp_ins_avaliacao_nula";

                var sqlParameters = new List<SqlParameter>();

                if (cd_turma != null)
                    sqlParameters.Add(new SqlParameter("@cd_turma", cd_turma));
                else
                    sqlParameters.Add(new SqlParameter("@cd_turma", DBNull.Value));

                if (cd_tipo_avaliacao != null)
                    sqlParameters.Add(new SqlParameter("@cd_tipo_avaliacao", cd_tipo_avaliacao));
                else
                    sqlParameters.Add(new SqlParameter("@cd_tipo_avaliacao", DBNull.Value));


                var parameter = new SqlParameter("@result", SqlDbType.Int);
                parameter.Direction = ParameterDirection.ReturnValue;
                sqlParameters.Add(parameter);

                command.Parameters.AddRange(sqlParameters.ToArray());
                command.ExecuteReader();

                //var retunvalue = command.Parameters["@retorno"].Value;
                var retunvalue = (int)command.Parameters["@result"].Value;
                db.Database.Connection.Close();
                return retunvalue == 0 ? (int)SaldoItem.StatusProcedure.SUCESSO_EXECUCAO_PROCEDURE : (int)SaldoItem.StatusProcedure.ERRO_EXECUCAO_PROCEDURE;
            }
            catch (SqlException exe)
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