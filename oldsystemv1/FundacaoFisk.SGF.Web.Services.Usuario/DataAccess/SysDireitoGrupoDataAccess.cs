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
    public class SysDireitoGrupoDataAccess : GenericRepository<SysDireitoGrupo>, ISysDireitoGrupoDataAccess
    {

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db {
            get {
                return (SGFWebContext) base.DB();
            }
        }


        public SysDireitoGrupo findGrupoDireito(int cdGrupo, int cdMenu)
        {
            try
            {
                var sql = (from gu in db.SysDireitoGrupo
                           where gu.cd_grupo == cdGrupo &&
                             gu.cd_menu == cdMenu
                           select gu).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
        
        public IQueryable<SysDireitoGrupo> findGrupoDireito(int cdGrupo)
        {
            try
            {
                var sql = from DireitoGrupo in db.SysDireitoGrupo
                           where DireitoGrupo.cd_grupo == cdGrupo
                           select DireitoGrupo;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

        public IEnumerable<SysDireitoGrupo> findAllGrupoDireito(int[] cdGrupos)
        {
            try
            {
                var sql = from DireitoGrupo in db.SysDireitoGrupo
                          where cdGrupos.Contains(DireitoGrupo.cd_grupo)
                          select DireitoGrupo;
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }

    }
}
