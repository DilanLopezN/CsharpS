using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
    
    public interface INivelDataAccess : IGenericRepository<Nivel>
    {
        IEnumerable<Nivel> getNivelSearch(SearchParameters parametros, string desc, bool inicio, bool? status);
        IEnumerable<Nivel> findNivel(NivelDataAccess.TipoConsultaNivelEnum hasDependente);
        List<Nivel> getNiveis(List<int> cdsNiv);
        int GetUltimoNmOrdem(int cd_nivel);
    }
}
