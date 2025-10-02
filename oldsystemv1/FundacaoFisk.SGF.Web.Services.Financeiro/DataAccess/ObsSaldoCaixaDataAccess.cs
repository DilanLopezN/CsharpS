using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess
{
    public class ObsSaldoCaixaDataAccess : GenericRepository<ObsSaldoCaixa>, IObsSaldoCaixa
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }

        public ObsSaldoCaixa getObsSaldoCaixaUsuario(int cd_pessoa_escola, int cd_local_movimento, DateTime dta_fechamento, int cdUsuario)
        {
            try
            {
                ObsSaldoCaixa sql = null;
                if (cd_local_movimento == 0)
                {
                    sql = (from obs in db.ObsSaldoCaixa
                               where db.LocalMovto.Where(lm => lm.cd_pessoa_empresa == cd_pessoa_escola).Any() &&
                               obs.cd_caixa_usuario == null && obs.dt_saldo_caixa == dta_fechamento && obs.cd_usuario == cdUsuario
                               select obs).FirstOrDefault();
                }
                else {
                    sql = (from obs in db.ObsSaldoCaixa
                               where db.LocalMovto.Where(lm => lm.cd_pessoa_empresa == cd_pessoa_escola && lm.cd_local_movto == cd_local_movimento).Any() &&
                               obs.cd_caixa_usuario == cd_local_movimento && obs.dt_saldo_caixa == dta_fechamento && obs.cd_usuario == cdUsuario
                               select obs).FirstOrDefault();
                }
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
