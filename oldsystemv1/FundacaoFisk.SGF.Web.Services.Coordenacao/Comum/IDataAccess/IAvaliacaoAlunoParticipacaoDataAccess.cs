using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAvaliacaoAlunoParticipacaoDataAccess : IGenericRepository<AvaliacaoAlunoParticipacao>
    {
        IEnumerable<AvaliacaoAlunoParticipacao> getAvaliacaoAlunoParticipacaoByAvaliacaoAluno(int cd_avaliacao_aluno);
        bool verificaAvaliacaoAlunoParticipacaoByCriterio(int cd_criterio_avaliacao);
        bool verificaParticipacaoAvaliacao(int cd_participacao_avaliacao);
    }
}
