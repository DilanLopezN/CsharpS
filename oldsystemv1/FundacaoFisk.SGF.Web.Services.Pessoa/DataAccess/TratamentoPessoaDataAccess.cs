using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.DataAccess
{
    public class TratamentoPessoaDataAccess: GenericRepository<TratamentoPessoa>, ITratamentoPessoaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public IEnumerable<TratamentoPessoa> getAllTratamentoPessoa()
        {
            try{
                return db.TratamentoPessoa.AsEnumerable();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
