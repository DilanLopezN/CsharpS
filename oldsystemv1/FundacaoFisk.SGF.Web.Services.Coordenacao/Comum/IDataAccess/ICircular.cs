using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface ICircular : IGenericRepository<Circular>
    {
        IEnumerable<Circular> obterListaCirculares();
        Circular findCircularById(int cd_circular);
        Circular findDeletedCircularById(int cd_circular);
        IEnumerable<Circular> obterCircularesPorFiltros(SearchParameters parametros, short nm_ano_circular, List<byte> nm_mes_circular, int nm_circular, string no_circular, List<byte> nm_menu_circular);
    }
}
