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

    public interface ITabelaPrecoDataAccess : IGenericRepository<TabelaPreco>
    {
        IEnumerable<TabelaPrecoUI> GetTabelaPrecoSearch(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, DateTime? dtaCad, int cdEscola, int cdProduto);
        TabelaPreco GetTabelaById(int idTabelaPreco, int cdEscola);
        TabelaPrecoUI GetTabelaPrecoById(int idTabelaPreco, int cdEscola);
        IEnumerable<TabelaPrecoUI> GetHistoricoTabelaPreco(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, int cdEscola);
        bool deleteAllTabelaPreco(List<TabelaPreco> tabelas);
        int? getNroParcelas(int cd_escola, int cd_curso, int cd_regime, int cd_duracao, DateTime data_matricula);
        TabelaPreco getValoresForMatricula(int cd_pessoa_escola, int cd_curso, int cd_duracao, int cd_regime, DateTime dta_matricula);
    }
}
