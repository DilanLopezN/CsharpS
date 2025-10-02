using System;
using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.Pessoa.Comum.DataAccess
{
    public interface IPapelDataAccess : IGenericRepository<PapelSGF>
    {
        IEnumerable<PapelSGF> GetAllPapel();
        IEnumerable<PapelSGF> getPapelByTipo(int[] tipo);
        IEnumerable<QualifRelacionamento> getAllQualifRelacByPapel(int codPapel);
        PapelSGF getPapelById(int cd_papel);
    }
}
