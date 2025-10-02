using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IMensagemAvaliacaoAlunoDataAccess: IGenericRepository<MensagemAvaliacaoAluno>
    {
        IEnumerable<MensagemAvaliacaoAlunoUI> findMsgAlunobyTipo(int cdTipoAvaliacao, int cdAluno, int cdProduto, int cdCurso);
        List<MensagemAvaliacaoAlunoUI> getMensagensAvaliacaoAlunoByIds(List<int> cdsMensagens);
        List<MensagemAvaliacaoAlunoUI> getMensagensAvaliacaoAlunoById(int cdsMensagens);
    }
}
