using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.DataAccess
{
    public class DireitoUsuarioDataAccess : GenericRepository<SysDireitoUsuario>, IDireitoUsuarioDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public IEnumerable<SysDireitoUsuario> findAllDireitosUsuarioByUsuario(int cd_usuario)
        {
            try
            {
                var sql = from ue in db.SysDireitoUsuario
                          where ue.cd_usuario == cd_usuario
                          select ue;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
