using System;
using System.Linq;
using System.Collections.Generic;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Usuario.Comum;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericRepository;
using System.Data.Entity;
using Componentes.GenericDataAccess.GenericException;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Usuario.DataAccess
{
    public class SysGrupoUsuarioDataAccess : GenericRepository<SysGrupoUsuario>, ISysGrupoUsuarioDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public SysGrupoUsuario findGrupoUsuario(int cdGrupo, int cdUsuario)
        {
            try
            {
                var sql = (from gu in db.SysGrupoUsuario
                          where gu.cd_grupo == cdGrupo &&
                            gu.cd_usuario == cdUsuario
                          select gu).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SysGrupoUsuario> findAllGrupoUsuarioByUsuario(int cd_usuario)
        {
            try
            {
                var sql = from ue in db.SysGrupoUsuario
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
