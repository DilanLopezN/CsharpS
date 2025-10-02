using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface ITaxaMatriculaDataAccess : IGenericRepository<TaxaMatricula>
    {
        TaxaMatricula getTaxaMatriculaByIdContrato(int cd_contrato, int cd_pessoa_escola);
        TaxaMatriculaSearchUI searchTaxaMatricula(int cd_contrato, int cd_pessoa_escola);
    }
}
