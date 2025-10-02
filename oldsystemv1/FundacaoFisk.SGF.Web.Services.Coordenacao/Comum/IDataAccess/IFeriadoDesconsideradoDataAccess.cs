using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IFeriadoDesconsideradoDataAccess: IGenericRepository<FeriadoDesconsiderado>
    {
        IEnumerable<FeriadoDesconsiderado> getFeriadoDesconsideradoByTurma(int cd_turma, int cd_escola);
    }
}
