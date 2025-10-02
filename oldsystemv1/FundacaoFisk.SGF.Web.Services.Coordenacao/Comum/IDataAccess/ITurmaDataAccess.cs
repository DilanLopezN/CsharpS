using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericDataAccess.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IDataAccess
{
    public interface ITurmaDataAccess : IGenericRepository<Turma>
    {
        IEnumerable<TurmaSearchUI> getTurmaDesc(Componentes.Utils.SearchParameters parametros, string desc, bool inicio, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo, int cdPolitica);      
        Turma firstOrDefault(int cdEscola);
        IEnumerable<Turma> findTurma(int cdEscola, int? cd_turma, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        IEnumerable<Turma> findTurma(int idTurma, int idEscola);

        TurmaApiCyberBdUI findTurmaApiCyber(int cd_turma, int cd_escola);
        TurmaApiCyberBdUI findTurmaByCdTurmaAndCdEscolaApiCyber(int cd_turma, int cd_escola);
        TurmaApiCyberBdUI findTurmaCancelarEncerramentoApiCyber(int cd_turma);
        IEnumerable<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurmaPPTPai(int cd_turma);
        IEnumerable<ProfessorTurma> findProfessorTurmaByCdTurma(int cd_turma, int cd_escola);
        IEnumerable<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurma( int cd_turma);
        int findNovaTurmaByCdTurmaEncerrada(int cd_turma);
        Turma findTurmasByIdAndCdEscola(int idTurma, int idEscola);
        Turma findTurmaByCdTurmaApiCyber(int cdTurma);
        IEnumerable<TurmaSearch> searchTurma(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
                                               int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas,
                                               int cdAluno, int origemFK, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial, DateTime? dt_final,
                                               bool ProfTurmasAtuais, int cd_search_sala, int cd_search_sala_online, bool ckSearchSemSala, bool ckSearchSemAluno, 
                                               List<int> cdSituacoesAlunoTurma = null, int cd_escola_combo_fk = 0, int diaSemanaTurma = 0, int ckOnLine = 0, string dias="0000000");

        IEnumerable<TurmaSearch> getTurmaSearchAulaReposicaoDestinoFK(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
            int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas,
            int cdAluno, int origemFK, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial, DateTime? dt_final,
            bool ProfTurmasAtuais, DateTime? dt_programacao, int cd_estagio, int cd_turma_origem, List<int> cdSituacoesAlunoTurma = null, bool ckOnLine = true);
        TurmaSearch getTurmaByCodMudancaOuEncerramento(int cdTurma, int cdEscola);
        List<TurmaSearch> getTurmaByCodigosEncerramento(List<int?> cdTurma, int cdEscola);
        TurmaSearch getTurmaByCodForGrid(int cdTurma, int cdEscola, bool turmasFilha);
        List<Turma> getTurmasByCod(int[] cdTurmas, int cd_escola);
        Turma buscarTurmaHorariosEdit(int cdTurma, int cdEscola,TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        bool ExisteProgramacaoTurma(int cd_turma,int cd_escola);
        bool ExisteAvaliacaoTurma(int cd_turma, int cd_escola);
        bool verificarTurmaExisteProgramacaoHorario(int cd_turma, int cd_horario, int cdEscola);
        int verificaExisteTurma(string nomeTurma, int cdEscola, int cdTurma);
        int getTumaAlunoByIdTurma(int cd_turma, int cd_escola);
        int getNumeroVagas(int id, int tipoConsulta, int cd_escola);

        bool getVerificaContrato(int cd_turma);

        Turma getTurmaComCursoDuracao(int cd_turma);
        ProgramacaoHorarioUI getProgramacaoHorarioTurma(int cd_turma, int cd_escola);
        Turma getTurmaComProgramacoes(int cd_turma);

        IEnumerable<TurmaSearch> searchTurmaAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                  int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, int tipoConsulta, bool retorno, int cd_escola_combo_fk);
        IEnumerable<TurmaSearch> searchTurmasContrato(int cd_contrato, int cd_escola, int cd_aluno);
        bool verifTurmasFilhasDisponiveisHorariosTurmaPPTBD(int cd_turma_PPT, int cd_escola, List<Horario> horariosTurmaPPT);
        IEnumerable<Turma> getTurmaPorProgramacaoTurmaComFeriado(Feriado feriado, int? cd_escola, bool delecao_feriado);
        Turma getTurmaEHorarios(int cdTurma, int cdEscola);

        IEnumerable<TurmaSearch> searchTurmaComAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                   int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, DateTime? dtInicial,
                                                   DateTime? dtFinal, int? cd_turma_PPT, int cdTurmaOrigem, int opcao, int tipoConsulta, int cd_escola_combo_fk);
        Turma findTurmaOrigem(int cdEscola, int cd_turma, int cd_aluno);

        IEnumerable<TurmaSearch> searchTurmaAlunoDesistente(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                               int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, DateTime? dtInicial,
                                               DateTime? dtFinal, int tipoConsulta);

        IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola);
        IEnumerable<Estagio> getEstagiosHistoricoAluno(int cd_aluno, int cd_escola);
        bool verificaTurmaSeExisteAlunoComSitacaoDifAguard(int cd_turma, int cd_empresa);
        Turma getTurmaAlunoAguard(int cd_pessoa_aluno, int cd_empresa);
        TurmaSearch searchTurmaComAlunoDesistencia(int cdEscola, int cdAluno, int opcao);
        IEnumerable<TurmaSearch> getRptTurmas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int situacaoTurma, List<int> situacaoAlunoTurma, int tipoOnline, string dias);
        IEnumerable<TurmaSearch> getRptTurmasAEncerrar(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
          DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoOnline, string dias);
        IEnumerable<TurmaSearch> getRptTurmasNovas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoOnline, string dias);

        IEnumerable<TurmaSearch> getRptTurmasProgAula(int cd_turma, int cd_escola, DateTime? pDtaI, DateTime? pDtaF);        
        Turma buscarTurmaHorariosEditVirada(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        IEnumerable<ReportTurmaMatriculaMaterial> getRptTurmasMatriculaMaterial(int cd_escola, int cd_turma, int cd_aluno, int cd_item, int nm_contrato, DateTime? pDtaI, DateTime? pDtaF);
        bool getTurmaNovaEnc(int cd_turma_enc);
        Turma getTurmaNovaEncSituacao(int cd_turma_enc);
        Turma buscarTurmaHorariosNovaVirada(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        Turma getDadosTurmaMud(int cdEscola, int cd_turma);
        IEnumerable<Turma> getTurmaPoliticaEsc(int cdEscola);
        List<RptBolsistasAval> getAvaliacoesBolsista(int cd_aluno, int cd_pessoa_escola, int cd_turma);
        IEnumerable<Turma> getTurmasPersonalizadas(int cdProduto, DateTime dtAula, TimeSpan hrIni, TimeSpan hrFim, int cdEscola, int? cd_turma);
        List<int> getTurmaPorAlunoProg(int cd_escola, int cd_aluno, DateTime dt_inicial);
        List<Turma> getTurmaAlunoMatRemat(int cdEscola, int cd_aluno);
        List<ReportPercentualTerminoEstagio> getRptPercentualTerminoEstagio(int cd_professor, DateTime? dt_ini, DateTime? dt_fim, int cd_escola);
        bool verificaSeTurmaEFilhaPersonalizada(int cd_turma, int cdEscola);
        List<TipoAvaliacao> getTiposAvaliacaoComQtdAvaliacao(int cd_turma, List<int> cdsTiposAvaliacao);
        //Controle de Faltas
        IEnumerable<TurmaSearch> searchTurmaComPercentualFaltas(SearchParameters parametros, string descricao,
            string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
            int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas,
            int cdAluno, DateTime? dtInicial,
            DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial,
            DateTime? dt_final, bool ProfTurmasAtuais, bool id_percentual_faltas,
            List<int> cdSituacoesAlunoTurma = null);

        Turma findTurmaPercentualFaltaGrupoAvancado(int cdEscola, int cd_turma, int? cd_turma_ppt, bool id_turma_ppt);
        string postGerarRematricula(string dc_turma, int cd_usuario, Nullable<System.DateTime> dt_inicial, Nullable<System.DateTime> dt_final, Nullable<bool> id_turma_nova, Nullable<int> cd_layout, Nullable<System.DateTime> dt_termino, int fusoHorario);
        string postCancelarEncerramento(Nullable<int> cd_turma, Nullable<int> cd_usuario, Nullable<int> fuso);
        string postRefazerProgramacao(int cdTurma);
        string postRefazerNumeracao(int cdTurma);
        List<Horario> verificaDiaSemanaTurmaFollowUp(int cdEscola, int cdTurma, int idDiaSemanaTurma);
        int getEscolaAluno(int CdAluno);

        bool HasTurmasEmAndamento(List<int> cdsTurmasPpt);

        void sp_verificar_grupo_cyber(string no_turma, Nullable<int> id_unidade);
        List<int?> TurmaEncLista(string dc_turma, Nullable<System.DateTime> dt_termino, Nullable<int> cd_usuario, Nullable<int> fuso);
        int postCancelarTurmasEncerramento(string dc_turma, int? cd_usuario, int? fuso);
    }
}
