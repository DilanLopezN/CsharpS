using Componentes.GenericDataAccess.Comum;
using FundacaoFisk.SGF.GenericModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAtividadeAlunoDataAccess : IGenericRepository<AtividadeAluno>
    {
        IEnumerable<AtividadeAlunoUI> searchAtividadeAluno(int cdAtividadeExtra, int cdEscola);
        IEnumerable<AtividadeAlunoUI> searchAtividadeAlunoReport(int cdAtividadeExtra, int cdEscola);
        long retornNumbersOfStudents(int idAtividadeExtra, int cdEscola);
        long retornAllNumbersOfStudents(int idAtividadeExtra);
        IEnumerable<AtividadeAluno> searchAtividadeAlunoByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola);
        IEnumerable<AtividadeAluno> searchAtividadeAlunoByCdAtividadeExtraForRecorrencia(int cdAtividadeExtra, int cdEscola);
        IEnumerable<String> searchEmailsAtividadeAlunoByCdAtividadeExtra(int cdAtividadeExtra, int cdEscola);
        int getNroPessoasAtividade(int cdAtividadeExtra);
    }
}
