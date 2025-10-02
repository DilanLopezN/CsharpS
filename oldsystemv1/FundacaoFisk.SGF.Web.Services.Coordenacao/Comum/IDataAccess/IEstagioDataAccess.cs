using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IEstagioDataAccess : IGenericRepository<Estagio>
    {
        IEnumerable<EstagioSearchUI> getEstagioDesc(SearchParameters parametros, String nome, String abrev, Boolean inicio, Boolean? ativo, int cdProduto);
        IEnumerable<Estagio> getEstagioOrdem(int CodP, int? cd_estagio, EstagioDataAccess.TipoConsultaEstagioEnum? tipoConsulta);
        byte? getOrdem(int CodP);
        void editOrdem(Estagio estagio);
        Boolean deleteAllEstagio(List<Estagio> estagio);
        IEnumerable<Estagio> getAllEstagioByProduto(int cdProduto);
    }
}
