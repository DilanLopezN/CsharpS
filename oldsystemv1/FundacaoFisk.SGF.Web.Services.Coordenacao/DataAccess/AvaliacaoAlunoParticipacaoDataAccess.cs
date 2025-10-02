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
    public class AvaliacaoAlunoParticipacaoDataAccess : GenericRepository<AvaliacaoAlunoParticipacao>, IAvaliacaoAlunoParticipacaoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<AvaliacaoAlunoParticipacao> getAvaliacaoAlunoParticipacaoByAvaliacaoAluno(int cd_avaliacao_aluno)
        {
            try
            {
                return from aap in db.AvaliacaoAlunoParticipacao
                       where aap.cd_avaliacao_aluno == cd_avaliacao_aluno
                       select aap;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool verificaParticipacaoAvaliacao(int cd_participacao_avaliacao)
        {
            try 
	        {
                return db.AvaliacaoAlunoParticipacao.Any(aap => aap.cd_participacao_avaliacao == cd_participacao_avaliacao);
	        }
            catch (Exception exe)
	        {
                throw new DataAccessException(exe);
	        }
        }

        public bool verificaAvaliacaoAlunoParticipacaoByCriterio(int cd_criterio_avaliacao) 
        {
            try
            {
                return db.CriterioAvaliacao.Where(c => c.cd_criterio_avaliacao == cd_criterio_avaliacao && c.Avaliacao.Any(x => x.AvaliacaoTurma.Where(y => y.cd_avaliacao == x.cd_avaliacao).Any())).Any();
            }
            catch (Exception ex)
            {
                throw new DataAccessException(ex);
            }
        }
    }
}
