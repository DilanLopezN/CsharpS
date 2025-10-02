using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
//using FundacaoFisk.SGF.Web.Service.EmailMarketing.Model;
//using FundacaoFisk.SGF.Web.Services.EmailMarketing.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.EmailMarketing.Comum.IDataAccess
{
    public interface IMalaDiretaCadastroDataAccess : IGenericRepository<MalaDiretaCadastro>
    {
    }
}
