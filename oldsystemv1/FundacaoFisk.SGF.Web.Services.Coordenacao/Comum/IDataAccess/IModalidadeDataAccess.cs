using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IModalidadeDataAccess : IGenericRepository<Modalidade>
    {
        IEnumerable<Modalidade> getModalidadeDesc(SearchParameters parametros, String desc, Boolean inicio, Boolean? ativo);
        IEnumerable<Modalidade> getModalidades(ModalidadeDataAccess.TipoConsultaModalidadeEnum? tipoConsulta, int? cd_modalidade);
        Boolean deleteAllModalidade(List<Modalidade> modalidades);
    }
}
