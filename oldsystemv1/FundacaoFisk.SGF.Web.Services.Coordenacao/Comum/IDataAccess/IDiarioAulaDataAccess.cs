using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface IDiarioAulaDataAccess : IGenericRepository<DiarioAula>
    {
        bool verificaTurmaDiarioAulaLancado(int cd_turma,int cd_escola);
        IEnumerable<vi_diario_aula> searchDiarioAula(SearchParameters parametros, int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf,
                                                     bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal, int cd_escola, int? cdProf, int cd_escola_combo = 0);
        DiarioAula getEditDiarioAula(int cd_diario_aula, int cd_escola);
        vi_diario_aula getDiarioForGridById(int cd_diario_aula, int cd_pessoa_escola);
        bool verificaIntersecaoHorarios(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data);
        bool verificaIntersecaoTurmaPersonalizada(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data);
        IEnumerable<DiarioAula> getDiarioAulas(int[] cdDiarios, int cd_escola);
        bool verificaDiarioAulaEfetivadoProf(int cd_turma, int cd_professor, int cd_escola);
        bool verificaExisteDiarioEfetivoProgramacaoTurma(int cd_turma, int cd_escola, int cd_programacao);
        int returnQuantidadeDiarioAulaByTurma(int cd_turma, int cd_escola, int cd_aluno);
        bool verificarDiarioAulaEfetivadoProfTurmasFilhasPPT(int cd_turma_ppt, int cd_professor, int cd_escola);

        DiarioAula getUltimaAulaAluno(int cd_aluno, int cd_turma, int cd_escola);
        DiarioAula getUltimaAulaTurma(int cd_turma, int cd_escola);
        int returnDiarioByDataDesistencia(DateTime? data_desistencia, int cd_pessoa_escola, int cd_aluno, int cd_turma_aluno, int tipoDesistencia);
        int quantidadeDiarioAulaTurma(int cd_turma, int cd_escola);
        int qtdDiarioAulaTurmaData(int cd_turma, DateTime dt_diario);
        string getObsDiarioAula(int cd_diario_aula, int cd_escola);
        List<ProgramacaoTurma> verificaDiarioAposFeriado(int cd_feriado, int? cd_escola);
        IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma, bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada);
    }
}
