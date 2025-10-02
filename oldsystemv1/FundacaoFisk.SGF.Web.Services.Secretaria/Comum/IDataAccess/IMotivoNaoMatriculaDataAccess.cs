using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericModel;
using Componentes.GenericDataAccess.Comum;
using System.Data.Entity;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using FundacaoFisk.SGF.GenericModel;
    public interface IMotivoNaoMatriculaDataAccess : IGenericRepository<MotivoNaoMatricula>
    {
        //Motivo Não Matricula
        IEnumerable<MotivoNaoMatricula> GetMotivoNaoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        bool deleteAll(List<MotivoNaoMatricula> motivosNaoMatricula);
        IEnumerable<MotivoNaoMatricula> getMotivoNaoMatriculaProspect(int cdProspect);
        IEnumerable<MotivoNaoMatricula> getMotivosNaoMatricula();
        IEnumerable<MotivoNaoMatricula> getProspectMotivoNaoMatricula(int cdProspect, int cdEscola);
    }
}
