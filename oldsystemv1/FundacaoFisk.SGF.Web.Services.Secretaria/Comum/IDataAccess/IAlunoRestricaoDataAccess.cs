using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IAlunoRestricaoDataAccess : IGenericRepository<AlunoRestricao>
    {
        IEnumerable<AlunoRestricao> getAlunoRestricaoByCdAluno(int cd_aluno);
        IEnumerable<AlunoRestricao> getAlunoRestricaoEditByCdAluno(int cd_aluno);
    }
}