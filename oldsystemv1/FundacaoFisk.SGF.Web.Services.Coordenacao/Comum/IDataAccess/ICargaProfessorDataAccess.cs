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
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface ICargaProfessorDataAccess : IGenericRepository<CargaProfessor>
    {
        IEnumerable<CargaProfessorSearchUI> getCargaProfessorSearch(SearchParameters parametros, int qtd_minutos_duracao, int cd_escola);
        IEnumerable<CargaProfessor> getCargaProfessorByEscolaAllData(List<int> codigosCargaProfs, int cd_escola);
        IEnumerable<CargaProfessor> findCargaProfessorAll(int cd_escola);
        CargaProfessor findCargaProfessorById(int id, int cd_escola);
    }
}
