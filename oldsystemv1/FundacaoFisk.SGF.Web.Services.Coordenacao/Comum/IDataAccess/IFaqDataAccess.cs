using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IFaqDataAccess : IGenericRepository<Faq>
    {
        IEnumerable<Faq> getFaqSearch(Componentes.Utils.SearchParameters parametros, string no_faq, int nm_faq, List<byte> menu);
        Faq findFaqById(int cd_faq);
        Faq findDeletedFaqById(int cd_faq);
        IEnumerable<Faq> obterFaqsPorFiltros(Componentes.Utils.SearchParameters parametros, string dc_faq_pergunta, bool dc_faq_inicio, List<byte> menu);
        Faq findFaqByNumeroParte(int nm_faq, int nm_parte);
        Faq findFaqByName(string no_faq);
    }
}
