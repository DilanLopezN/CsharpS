using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IDataAccess;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.DataAccess
{
    public class SysAppDataAccess : GenericRepository<SysApp>, ISysAppDataAccess
    {
        public enum TipoConsultaSysAppEnum
        {
            HAS_ATIVO = 0,
            HAS_ATIVO_MATRICULA = 1
        }

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public SysApp getConfigEmailMarketingSysApp()
        {
            try
            {
                var result = (from s in db.SysApp
                              select new
                              {
                                  s.tx_msg_email,
                                  s.tx_verso_cartao_postal
                              }).ToList().Select(x => new SysApp{ tx_msg_email = x.tx_msg_email, tx_verso_cartao_postal = x.tx_verso_cartao_postal}).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public string getVersoCartaoPostal()
        {
            try
            {
                var result = (from s in db.SysApp
                              select s.tx_verso_cartao_postal).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public String getRodapeSysApp()
        {
            try
            {
                var result = (from s in db.SysApp
                              select s.tx_msg_email).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

        public SysApp getSysApp()
        {
            try
            {
                SysApp result = (from s in db.SysApp
                                 select s).FirstOrDefault();

                return result;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }

        }

    }
}
