using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Empresa.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Empresa.DataAccess
{
    public class FuncionarioComissaoDataAccess : GenericRepository<FuncionarioComissao>, IFuncionarioComissaoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public List<FuncionarioComissao> getFuncionarioComissao(int cd_func)
        {
            try
            {
                var sql = (from func in db.FuncionarioComissao
                           where func.cd_funcionario == cd_func
                           select func).ToList();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
