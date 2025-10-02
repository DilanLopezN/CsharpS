using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface ITratamentoPessoaDataAccess : IGenericRepository<TratamentoPessoa>
    {
        IEnumerable<TratamentoPessoa> getAllTratamentoPessoa();
    }
}
