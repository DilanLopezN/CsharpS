using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class DiasPoliticaDataAccess : GenericRepository<DiasPolitica>, IDiasPoliticaDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }
        public List<DiasPolitica> GetDiasPoliticaById(int idPoliticaDesconto, int cdEscola)
        {
            try
            {
                var sql = from c in db.DiasPolitica
                          where c.cd_politica_desconto == idPoliticaDesconto &&
                          c.DiasPoliticaDesc.cd_pessoa_escola == cdEscola

                          select c;
                return sql.ToList();
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
