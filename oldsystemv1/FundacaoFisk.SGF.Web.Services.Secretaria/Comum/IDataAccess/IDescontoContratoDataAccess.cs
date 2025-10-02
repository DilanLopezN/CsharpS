using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IDescontoContratoDataAccess : IGenericRepository<DescontoContrato>
    {
        IEnumerable<DescontoContrato> getDescontoContrato(int cd_contrato, int cd_pessoa_escola);
        IEnumerable<DescontoContrato> getDescontoAditamento(int cd_contrato, int cd_aditamento, int cd_pessoa_escola);
        IEnumerable<DescontoContrato> getDescontoContratoAllDados(int cd_contrato, int cd_pessoa_escola);
    }
}
