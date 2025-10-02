using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Componentes.GenericDataAccess.GenericException;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess
{
    public class AvaliacaoAlunoDataAccess : GenericRepository<AvaliacaoAluno>, IAvaliacaoAlunoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AvaliacaoAluno> getConceitosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola) {
            try {
                var sql = (from a in db.AvaliacaoAluno
                           where a.AvaliacaoTurma.cd_turma == cd_turma
                             /*&& a.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_escola*/
                             && a.cd_aluno == cd_aluno
                             && a.Conceito != null
                           orderby a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao, a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao, a.AvaliacaoTurma.dt_avaliacao_turma
                           select new {
                               cd_avaliacao_aluno = a.cd_avaliacao_aluno,
                               cd_avaliacao_turma = a.cd_avaliacao_turma,
                               cd_aluno = a.cd_aluno,
                               dt_avaliacao_turma = a.AvaliacaoTurma.dt_avaliacao_turma,
                               dc_criterio_avaliacao = a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                               dc_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                               cd_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao,
                               no_conceito = a.Conceito.no_conceito
                           }).ToList().Select(x => new AvaliacaoAluno {
                               cd_avaliacao_aluno = x.cd_avaliacao_aluno,
                               cd_avaliacao_turma = x.cd_avaliacao_turma,
                               cd_aluno = x.cd_aluno,
                               no_conceito = x.no_conceito,
                               dt_avaliacao_turma = x.dt_avaliacao_turma,
                               dc_criterio_avaliacao = x.dc_criterio_avaliacao,
                               dc_tipo_avaliacao = x.dc_tipo_avaliacao,
                               AvaliacaoTurma = new AvaliacaoTurma() { Avaliacao = new Avaliacao() { cd_tipo_avaliacao = x.cd_tipo_avaliacao } }
                           });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoAluno> getNotasAvaliacaoTurma(int cd_aluno, int cd_turma, int cd_escola) {
            try {
                var sql = (from a in db.AvaliacaoAluno 
                           where a.AvaliacaoTurma.cd_turma == cd_turma
                             /*&& a.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_escola*/
                             && a.cd_aluno == cd_aluno
                             && a.nm_nota_aluno.HasValue
                           orderby a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao, a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao, a.AvaliacaoTurma.dt_avaliacao_turma
                           select new {
                               cd_avaliacao_aluno = a.cd_avaliacao_aluno,
                               cd_avaliacao_turma = a.cd_avaliacao_turma,
                               cd_aluno = a.cd_aluno,
                               cd_turma = a.AvaliacaoTurma.Turma.cd_turma,
                               no_turma = a.AvaliacaoTurma.Turma.no_turma,
                               nm_nota_aluno = a.nm_nota_aluno,
                               nm_nota_aluno_2 = a.nm_nota_aluno_2,
                               dt_avaliacao_turma = a.AvaliacaoTurma.dt_avaliacao_turma,
                               dc_criterio_avaliacao = a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                               dc_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                               vl_nota = a.AvaliacaoTurma.Avaliacao.vl_nota,
                               nm_peso_avaliacao = a.AvaliacaoTurma.Avaliacao.nm_peso_avaliacao,
                               cd_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao,
                               vl_total_nota = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.vl_total_nota
                           }).ToList().Select(x => new AvaliacaoAluno {
                               cd_avaliacao_aluno = x.cd_avaliacao_aluno,
                               cd_avaliacao_turma = x.cd_avaliacao_turma,
                               cd_aluno = x.cd_aluno,
                               cd_turma = x.cd_turma,
                               no_turma = x.no_turma,
                               nm_nota_aluno = x.nm_nota_aluno,
                               nm_nota_aluno_2 = x.nm_nota_aluno_2,
                               dt_avaliacao_turma = x.dt_avaliacao_turma,
                               dc_criterio_avaliacao = x.dc_criterio_avaliacao,
                               dc_tipo_avaliacao = x.dc_tipo_avaliacao,
                               vl_nota = x.vl_nota,
                               nm_peso_avaliacao = x.nm_peso_avaliacao,
                               AvaliacaoTurma = new AvaliacaoTurma() { Avaliacao = new Avaliacao() { cd_tipo_avaliacao = x.cd_tipo_avaliacao, TipoAvaliacao = new TipoAvaliacao() { vl_total_nota = x.vl_total_nota } } }
                           });
                return sql;
            }
            catch(Exception exe) {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoAluno> getNotasAvaliacaoTurmaPorEstagio(int cd_aluno, int cd_estagio, int cd_escola)
        {
            try
            {
                var sql = (from a in db.AvaliacaoAluno
                           join h in db.HistoricoAluno on a.cd_aluno equals h.cd_aluno
                           where /*a.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_escola &&*/
                             a.AvaliacaoTurma.Turma.Curso.cd_estagio == cd_estagio
                            && a.cd_aluno == cd_aluno
                            && a.nm_nota_aluno.HasValue
                            && a.AvaliacaoTurma.Turma.cd_turma == h.cd_turma
                            && h.Turma.Curso.cd_estagio == cd_estagio
                            && h.nm_sequencia == (from hi in db.HistoricoAluno
                                                  where hi.cd_aluno == h.cd_aluno &&
                                                        hi.cd_turma == h.cd_turma &&
                                                        hi.Turma.Curso.cd_estagio == cd_estagio 
                                                  select hi).OrderByDescending(i=>i.nm_sequencia).FirstOrDefault().nm_sequencia
                             //orderby a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao, a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao, a.AvaliacaoTurma.dt_avaliacao_turma
                           orderby a.AvaliacaoTurma.Turma.no_turma, a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao, a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao, a.AvaliacaoTurma.dt_avaliacao_turma
                           select new
                           {
                               cd_estagio = a.AvaliacaoTurma.Turma.Curso.cd_estagio,
                               no_estagio = a.AvaliacaoTurma.Turma.Curso.Estagio.no_estagio,
                               cd_turma = a.AvaliacaoTurma.cd_turma,
                               no_turma = a.AvaliacaoTurma.Turma.no_turma,
                               nm_total_avaliacao_curso = (from ac in a.AvaliacaoTurma.Turma.Curso.AvaliacaoCurso
                                                           join ta in db.TipoAvaliacao on ac.cd_tipo_avaliacao equals ta.cd_tipo_avaliacao
                                                           join av in db.Avaliacao on ta.cd_tipo_avaliacao equals av.cd_tipo_avaliacao
                                                           join c in db.CriterioAvaliacao on av.cd_criterio_avaliacao equals c.cd_criterio_avaliacao
                                                           where c.id_conceito == false
                                                           orderby ac.cd_curso
                                                               select ac).Count(),

                               cd_avaliacao_aluno = a.cd_avaliacao_aluno,
                               cd_avaliacao_turma = a.cd_avaliacao_turma,
                               cd_aluno = a.cd_aluno,
                               nm_nota_aluno = a.nm_nota_aluno,
                               nm_nota_aluno_2 = a.nm_nota_aluno_2,
                               dt_avaliacao_turma = a.AvaliacaoTurma.dt_avaliacao_turma,
                               dc_criterio_avaliacao = a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.dc_criterio_avaliacao,
                               cd_criterio_avaliacao = a.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.cd_criterio_avaliacao,
                               dc_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.dc_tipo_avaliacao,
                               vl_nota = a.AvaliacaoTurma.Avaliacao.vl_nota,
                               nm_peso_avaliacao = a.AvaliacaoTurma.Avaliacao.nm_peso_avaliacao,
                               cd_tipo_avaliacao = a.AvaliacaoTurma.Avaliacao.cd_tipo_avaliacao,
                               vl_total_nota = a.AvaliacaoTurma.Avaliacao.TipoAvaliacao.vl_total_nota
                           }).ToList().Select(x => new AvaliacaoAluno
                           {
                               cd_estagio = x.cd_estagio,
                               no_estagio = x.no_estagio,
                               cd_turma = x.cd_turma,
                               no_turma = x.no_turma,
                               nm_total_avaliacao_curso = x.nm_total_avaliacao_curso,

                               cd_avaliacao_aluno = x.cd_avaliacao_aluno,
                               cd_avaliacao_turma = x.cd_avaliacao_turma,
                               cd_aluno = x.cd_aluno,
                               nm_nota_aluno = x.nm_nota_aluno,
                               nm_nota_aluno_2 = x.nm_nota_aluno_2,
                               dt_avaliacao_turma = x.dt_avaliacao_turma,
                               dc_criterio_avaliacao = x.dc_criterio_avaliacao,
                               cd_criterio_avaliacao = x.cd_criterio_avaliacao,
                               dc_tipo_avaliacao = x.dc_tipo_avaliacao,
                               vl_nota = x.vl_nota,
                               nm_peso_avaliacao = x.nm_peso_avaliacao,
                               AvaliacaoTurma = new AvaliacaoTurma() { Avaliacao = new Avaliacao() { cd_tipo_avaliacao = x.cd_tipo_avaliacao, TipoAvaliacao = new TipoAvaliacao() { vl_total_nota = x.vl_total_nota } } }
                           });
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool incluirAvaliacoesAluno(List<AvaliacaoAluno> avaliacoesAluno)
        {
            var incluido = false;
            try
            {
                if (avaliacoesAluno != null)
                {
                    foreach (var avaliacao in avaliacoesAluno)
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
        public bool verificaAvalicaoAlunoTurma(int cdTurma, int cdAluno)
        {
            try
            {
                var sql = from av in db.AvaliacaoAluno
                          join at in db.AvaliacaoTurma
                          on av.cd_avaliacao_turma equals at.cd_avaliacao_turma
                          where at.cd_turma == cdTurma &&
                            av.cd_aluno == cdAluno
                          select av;
                return sql.Count() > 0;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoAluno> getAvaliacaoesAlunoByIdTurmaEscola(int cd_turma, int cd_pessoa_escola, bool isConceito)
        {
            try
            {
                IQueryable<AvaliacaoAluno> sql;

                if(isConceito)
                 sql = from avAlunos in db.AvaliacaoAluno
                          where avAlunos.AvaliacaoTurma.cd_turma == cd_turma
                            && avAlunos.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_pessoa_escola
                            && avAlunos.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.id_conceito == true
                          select avAlunos;
                else
                    sql = from avAlunos in db.AvaliacaoAluno
                          where avAlunos.AvaliacaoTurma.cd_turma == cd_turma
                            && avAlunos.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_pessoa_escola
                            && !(avAlunos.AvaliacaoTurma.Avaliacao.CriterioAvaliacao.id_conceito)
                          select avAlunos;

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<AvaliacaoAluno> getAvaliacaoAlunoByIdAvlTurma(int cd_avaliacao_turma, int cd_pessoa_escola, int cd_turma)
        {
            try
            {
                IQueryable<AvaliacaoAluno> sql;

                    sql = (from avalAluno in db.AvaliacaoAluno                       
                       join turma in db.Turma on cd_turma equals turma.cd_turma
                       join curso in db.Curso on turma.cd_curso equals curso.cd_curso
                       where avalAluno.cd_avaliacao_turma == cd_avaliacao_turma
                          && turma.cd_pessoa_escola == cd_pessoa_escola
                       select avalAluno);
                
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool existeNotaOuConceitoAvalicoesAluno(int cd_turma, int cd_pessoa_escola)
        {
            try
            {
                    bool retorno = (from avAlunos in db.AvaliacaoAluno
                          where avAlunos.AvaliacaoTurma.cd_turma == cd_turma
                            && avAlunos.AvaliacaoTurma.Turma.cd_pessoa_escola == cd_pessoa_escola
                            && (avAlunos.nm_nota_aluno > 0 || avAlunos.cd_conceito != null ||
                                avAlunos.AvaliacaoTurma.dt_avaliacao_turma != null || avAlunos.AvaliacaoTurma.cd_funcionario != null)
                          select avAlunos.cd_avaliacao_aluno).Any();


                    return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
