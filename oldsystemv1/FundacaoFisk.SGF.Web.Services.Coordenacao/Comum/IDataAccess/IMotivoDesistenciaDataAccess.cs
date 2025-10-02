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

    public interface IMotivoDesistenciaDataAccess : IGenericRepository<MotivoDesistencia>
    {
        IEnumerable<MotivoDesistencia> getMotivoDesistenciaDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo, bool isCancelamento);
        Boolean deleteAllMotivoDesistencia(List<MotivoDesistencia> desitencias);
        IEnumerable<MotivoDesistencia> motivosDesistenciaWhitDesistencia(int cd_pessoa_empresa);
        IEnumerable<MotivoDesistencia> getMotivoDesistenciaByCancelamento(bool isCancelamento);
    }
}
