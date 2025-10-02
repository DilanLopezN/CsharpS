using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface ITurmaEscolaDataAccess : IGenericRepository<TurmaEscola>
    {

        IEnumerable<TurmaEscolaSearchUI> getTurmasEscolatWithTurma(int cd_turma);

        IEnumerable<TurmaEscola> getTurmasEscolatByTurma(int cd_turma);

        IEnumerable<TurmaEscola> getTurmasEscolatByIdAndTurma(int cd_turma, int cd_escola, int cd_turma_escola);
    }
}