using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum.IDataAccess
{
    public interface IProspectMotivoNaoMatriculaDataAccess : IGenericRepository<ProspectMotivoNaoMatricula>
    {
        ProspectMotivoNaoMatricula getProspectMotivoNaoMatriculaEsc(int cdProspect, int cdEscola, int cd_motivo);
    }
}
