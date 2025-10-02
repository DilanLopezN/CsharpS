using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess;
using Componentes.GenericDataAccess.GenericException;
using Componentes.GenericDataAccess.GenericRepository;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess
{
    public class ProspectMotivoNaoMatriculaDataAccess : GenericRepository<ProspectMotivoNaoMatricula>, IProspectMotivoNaoMatriculaDataAccess
    {
       

        // Propriedade privada de acesso do DataAcess
        private SGFWebContext db
        {
            get
            {
                return (SGFWebContext)base.DB();
            }
        }

        public ProspectMotivoNaoMatricula getProspectMotivoNaoMatriculaEsc(int cdProspect, int cdEscola, int cd_motivo)
        {
            try
            {
                ProspectMotivoNaoMatricula sql = (from motivo in db.ProspectMotivoNaoMatricula
                                                  where motivo.cd_prospect == cdProspect &&
                                                  motivo.Prospect.cd_pessoa_escola == cdEscola &&
                                                  motivo.cd_motivo_nao_matricula == cd_motivo
                                                  select motivo).FirstOrDefault();
                return sql;
            }
            catch (Exception exe)
            {
                throw new DataAccessException(exe);
            }
        }
    }
}
