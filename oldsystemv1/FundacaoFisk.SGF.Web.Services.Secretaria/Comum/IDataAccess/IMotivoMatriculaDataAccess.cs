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
    public interface IMotivoMatriculaDataAccess : IGenericRepository<MotivoMatricula>
    {
        //Motivo Matricula
        IEnumerable<MotivoMatricula> GetMotivoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        bool deleteAll(List<MotivoMatricula> motivosMatriculas);
    }
}
