using System;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    using System.Data.Entity;
    using Componentes.GenericDataAccess.Comum;
    using FundacaoFisk.SGF.GenericModel;
    public interface IPessoaFisicaDataAccess : IGenericRepository<PessoaFisicaSGF>
    {
    }
}
