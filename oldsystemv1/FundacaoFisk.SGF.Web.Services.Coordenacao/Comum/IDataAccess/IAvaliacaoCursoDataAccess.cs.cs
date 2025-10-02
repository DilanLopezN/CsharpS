using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAvaliacaoCursoDataAccess : IGenericRepository<AvaliacaoCurso>
    {
        IEnumerable<AvaliacaoCurso> getAllAvaliacaoCursoByAvalicao(int cdAvaliacao);
    }
}
