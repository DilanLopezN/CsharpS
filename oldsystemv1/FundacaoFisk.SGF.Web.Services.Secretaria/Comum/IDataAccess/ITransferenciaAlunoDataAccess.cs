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
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Business;
    using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
    public interface ITransferenciaAlunoDataAccess : IGenericRepository<TransferenciaAluno>
    {
        IEnumerable<TransferenciaAlunoUI> getEnviarTransferenciaAlunoSearch(SearchParameters parametros, int cd_escola_logada, int? cd_unidade_destino, int cd_aluno, string nm_raf, string cpf, int status_transferencia, DateTime? dataIni, DateTime? dataFim);

        EnviarTransferenciaComponentesCadParams getComponentesEnviarTransferenciaCad(int cdEscola);
        string getEmailUnidade(int cdEscola);
        string getRafByAluno(int cdAluno);
        TransferenciaAluno getTransferenciaAlunoByCodForGrid(int cd_transferencia_aluno);
        TransferenciaAluno getEnviarTransferenciaAlunoForEdit(int cd_transferencia_aluno);
        List<AlunoTurma> getAlunoTurmasBdTransferenciaAluno(int cd_aluno, int cd_escola);
        AlunoTurma getAlunoTurmaBdEditTransferenciaAluno(int cd_escola, int cd_aluno_turma);
        PessoaRaf getRafBdEditByAluno(int cdAluno);
        Aluno getAlunoBdEditByAluno(int cdAluno);
        IEnumerable<TransferenciaAlunoUI> getReceberTransferenciaAlunoSearch(SearchParameters parametros, int cdEscola, int? cdUnidadeOrigem, string noAluno, string nmRaf, string cpf, int statusTransferencia, DateTime? dtInicial, DateTime? dtFinal);
    }
}