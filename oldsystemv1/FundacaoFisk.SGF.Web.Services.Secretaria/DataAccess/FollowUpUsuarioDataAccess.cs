using System.Data.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class FollowUpUsuarioDataAccess : GenericRepository<FollowUpUsuario>, IFollowUpUsuarioDataAccess
    {


        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public bool usuarioLeuFollowUp(int cd_follow_up, int cd_usuario)
        {
            try
            {
                bool existe = (from flwe in db.FollowUpUsuario
                               where flwe.cd_follow_up == cd_follow_up && flwe.cd_usuario == cd_usuario
                          select flwe.cd_usuario).Any();
                return existe;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public FollowUpUsuario getFollowUpUsuario(int cd_follow_up, int cd_usuario)
        {
            try
            {
                FollowUpUsuario retorno = (from flwe in db.FollowUpUsuario
                               where flwe.cd_follow_up == cd_follow_up && flwe.cd_usuario == cd_usuario
                               select flwe).FirstOrDefault();
                return retorno;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        
    }
}
