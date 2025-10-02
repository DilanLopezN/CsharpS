using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAtividadeProspectDataAccess : IGenericRepository<AtividadeProspect>
    {
        IEnumerable<AtividadeProspectUI> searchAtividadeProspect(int cdAtividadeExtra, int cdEscola);
        long retornNumbersOfStudents(int idAtividadeExtra, int cdEscola);
        long retornAllNumbersOfStudents(int idAtividadeExtra);
        IEnumerable<AtividadeProspect> searchAtividadeProspectByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola);
        IEnumerable<AtividadeProspect> searchAtividadeProspectByCdAtividadeExtraForRecorrencia(int cdAtividadeExtra, int cdEscola);
        IEnumerable<String> searchAtividadeEmailsProspectByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola);
        IEnumerable<ContatoProspectAtividadeExtraUI> searchContatoProspectByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola);
        AtividadeProspect searchAtividadeProspectByCdAtividadeExtraAndEmailProspect(int cdAtividadeExtra, string email);
        IEnumerable<AtividadeProspectUI> searchAtividadeProspectByCdProspect(int cdProspect, int cdEscola);
    }
}