using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface ICursoContratoDataAccess : IGenericRepository<CursoContrato>
    {
        CursoContrato getCursoContratoById(int cd_curso_contrato);
        List<CursoContrato> getCursosContratoByCdContrato(int cd_contrato);
    }
}