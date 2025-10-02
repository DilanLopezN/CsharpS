using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IFichaSaudeDataAccess : IGenericRepository<FichaSaude>
    {
        IEnumerable<int> findByAluno(int cdAluno);
    }
}