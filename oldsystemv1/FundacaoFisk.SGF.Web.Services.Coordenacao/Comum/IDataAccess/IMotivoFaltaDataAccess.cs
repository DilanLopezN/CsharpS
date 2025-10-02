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
    public interface IMotivoFaltaDataAccess :  IGenericRepository<MotivoFalta>
    {
        IEnumerable<MotivoFalta> getMotivoFaltaDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo);
        Boolean deleteAllMotivoFalta(List<MotivoFalta> motivos);
        IEnumerable<MotivoFalta> getMotivoFaltaAtivo(int cdDiario);
    }
}
