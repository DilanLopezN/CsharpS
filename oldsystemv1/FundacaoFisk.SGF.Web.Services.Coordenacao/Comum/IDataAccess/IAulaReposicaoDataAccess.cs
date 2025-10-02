using System;
using System.Collections.Generic;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IAulaReposicaoDataAccess : IGenericRepository<AulaReposicao>
    {
        AulaReposicao findByIdAulaReposicaoViewFull(int cdAulaReposicao);

        IEnumerable<AulaReposicaoUI> searchAulaReposicao(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim,
            TimeSpan? hrInicial, TimeSpan? hrFinal, int? cd_turma, int? cd_aluno,
            int? cd_responsavel, int? cd_sala, int cdEscola);
        AulaReposicaoUI returnAulaReposicaoUsuarioAtendente(int cd_aula_reposicao, int cd_pessoa_escola);
        AulaReposicao findByIdAulaReposicaoFull(int cdAulaReposicao);
        //IEnumerable<AulaReposicao> getAtividadesAlunoR(SearchParameters parametros, int cd_aluno, int cd_escola);
        List<sp_RptAulaReposicao_Result> getReportAulaReposicao(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_turma, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao);
        List<sp_RptAulaReposicaoAluno_Result> getReportAulaReposicaoAluno(Nullable<int> cd_aula_reposicao, Nullable<int> cd_aluno, Nullable<byte> id_participou);
        List<TimeSpan?> getHorariosDisponiveisAulaRep(DateTime data, int turma, int professor, int? cdAulaReposicao, List<AlunoAulaReposicaoUI> alunos);
        int? verificaHorarioAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, int? cd_aula_reposicao, int cd_turma, int cd_professor, int cd_empresa, List<AlunoAulaReposicaoUI> alunos);
    }
}

