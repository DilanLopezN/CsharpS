using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Services.Coordenacao.Model;


namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAtividadeCursoDataAccess : IGenericRepository<AtividadeCurso>
    {
        IEnumerable<AtividadeCursoUI> searchAtividadeCurso(int cdAtividadeExtra, int cdEscola);
        IEnumerable<AtividadeCurso> searchAtividadesCurso(int cdAtividadeExtra, int cdEscola);
        List<AtividadeCurso> searchAtividadesCursoBycdAtividadExtra(int cdAtividadeExtra, int cdEscola);
    }
}
