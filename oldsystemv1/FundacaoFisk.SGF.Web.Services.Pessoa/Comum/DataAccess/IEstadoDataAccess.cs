using System.Collections.Generic;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using Componentes.Utils;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IEstadoDataAccess : IGenericRepository<EstadoSGF>
    {
        IEnumerable<EstadoUI> GetAllEstado();
        IEnumerable<EstadoUI> GetEstadoSearch(SearchParameters parametros, string descricao, bool inicio, int cdPais);
        IEnumerable<EstadoUI> FindEstado(string searchText);
        EstadoUI GetEstadoById(int idEstado);
        EstadoUI firstOrDefault();
        List<int> GetEstadosLocalidades(List<EstadoUI> estados);
        bool deleteAll(List<EstadoUI> estados);
        IEnumerable<EstadoUI> getEstadoEstado();
        IEnumerable<EstadoUI> getEstadoByPais(int cd_pais);
        LocalidadeSGF getEstadoBySigla(string no_sg_estado);
    }
}
