using System;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IPerdaMaterialDataAccess : IGenericRepository<PerdaMaterial>
    {
        IEnumerable<PerdaMaterialUI> getPerdaMaterialSearch(SearchParameters parametros, int? cd_aluno, int? nm_contrato, int? cd_movimento, int? cd_item, DateTime? dtInicio, DateTime? dtTermino, int status, int cd_escola);
        PerdaMaterialUI getPerdaMaterialForGrid(int cdPerdaMaterial);
        int processarPerdaMaterial(PerdaMaterial perdaMaterial, int cdUsuario, int fuso);
    }
}