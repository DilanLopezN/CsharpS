using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class TituloAditamentoDataAccess : GenericRepository<TituloAditamento>, ITituloAditamento
    {
        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<TituloAditamento> ObterTitulosAditamentoPorId(int aditamentoId)
        {
            try
            {
                var sql = (from tit in db.TituloAditamento
                           where tit.cd_aditamento == aditamentoId
                           select tit);

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public bool ExisteTitulo(int cd_titulo)
        {
            try
            {
                var sql = (from tit in db.TituloAditamento
                           where tit.cd_titulo == cd_titulo
                           select tit).Any();

                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
