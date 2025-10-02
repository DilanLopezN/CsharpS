using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IContratoDataAccess : IGenericRepository<Contrato>
    {
        
    }
}
