using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;

    public interface IPoliticaDescontoDataAccess : IGenericRepository<PoliticaDesconto>
    {
        IEnumerable<PoliticaDesconto> GetAllPoliticaDesconto();
        IEnumerable<PoliticaDescontoUI> GetPoliticaDescontoSearch(SearchParameters parametros, int cdTurma, int cdAluno, DateTime? dtaIni, DateTime? dtaFim, bool? ativo, int cdEscola);
        PoliticaDesconto GetPoliticaDescontoById(int idPoliticaDesconto, int cdEscola);
        bool deleteAllPoliticaDesconto(List<PoliticaDesconto> politicasDesconto);       
        PoliticaDescontoUI getPoliticaDesconto(int cdEscola, int cd_politica_desconto);

        PoliticaDesconto getPoliticaDescontoByTurmaAluno(int cd_turma, int cd_aluno, DateTime dt_vcto_titulo);
        PoliticaDesconto getPoliticaDescontoByAluno(int cd_aluno, DateTime dt_vcto_titulo);
        PoliticaDesconto getPoliticaDescontoByTurma(int cd_turma, DateTime dt_vcto_titulo);
        PoliticaDesconto getPoliticaDescontoByEscola(int cd_pessoa_escola, DateTime dt_vcto_titulo);
        bool existPolIgual(PoliticaDesconto politica);

        bool verificaBaixaTituloByPolitica(int cd_politica_desconto, int cd_escola);

        #region DiasPolitica
        bool addDiasPolitica(DiasPolitica dias);
        #endregion
    }
}
