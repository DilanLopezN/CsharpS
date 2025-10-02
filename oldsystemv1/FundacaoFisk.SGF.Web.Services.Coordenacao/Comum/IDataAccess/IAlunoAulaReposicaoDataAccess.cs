using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using FundacaoFisk.SGF.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAlunoAulaReposicaoDataAccess : IGenericRepository<AlunoAulaReposicao>
    {

        long retornNumbersOfStudents(int idAulaReposicao);
        IEnumerable<AlunoAulaReposicaoUI> searchAlunoAulaReposicao(int cd_aula_reposicao, int cdEscola);
    }
}