using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.GenericModel.Partial;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness
{
    public interface ITurmaBusiness : IGenericBusiness
    {
        void sincronizaContexto(DbContext db);

        //Turma
        IEnumerable<TurmaSearch> searchTurma(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
                                             int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
                                             bool turmasFilhas, int cdAluno, int origemFK, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial,
                                             DateTime? dt_final, bool ProfTurmasAtuais, int cd_search_sala, int cd_search_sala_online, bool ckSearchSemSala, bool ckSearchSemAluno, List<int> cdSituacoesAlunoTurma = null, int cd_escola_combo_fk = 0, int diaSemanaTurma = 0, int ckOnLine = 0,
                                             string dias = "0000000");

        IEnumerable<TurmaSearch> getTurmaSearchAulaReposicaoDestinoFK(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
            int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
            bool turmasFilhas, int cdAluno, int origemFK, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial,
            DateTime? dt_final, bool ProfTurmasAtuais, DateTime? dt_programacao, int cd_estagio, int cd_turma_origem, List<int> cdSituacoesAlunoTurma = null, bool ckOnLine = true);

        IEnumerable<Turma> findTurma(int idTurma, int idEscola);
        Turma addTurma(Turma turma);
        Turma editTurma(Turma turma, Turma turmaContext);
        Turma buscarTurmaHorariosEdit(int cdTurma, int cdEscola,TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        IEnumerable<Turma> findTurma(int cdEscola, int? cd_turma, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        List<Turma> getTurmasByCod(int[] cdTurmas, int cd_escola);
        bool deleteTurmas(int[] turmas,int cd_escola);
        bool verificarTurmaExisteProgramacaoHorario(int? cd_turma,int? cd_turma_ppt, int cd_horario, int cdEscola);
        bool verificarProgramacaoParaListaHorarios(int cd_turma,int cd_escola,List<Horario> horarios);
        int getTumaAlunoByIdTurma(int cd_turma, int cd_escola);
        string criarNomeTurma(string inicioNome, List<Horario> listahorario, DateTime inicioAulas);
        int verificaExisteTurma(string nomeTurma, int cdEscola, int cdTurma);
        void crudHorariosTurma(List<Horario> horariosView, int cd_turma, int cd_escola,Turma.TipoTurma tipoTurma);
        void crudProfessoresTurma(List<ProfessorTurma> professorTurmaView, Turma turma, IEnumerable<Horario> horarios, bool turmaPPTFilha);
        TurmaApiCyberBdUI findTurmaApiCyber(int cd_turma, int cd_escola);
        TurmaApiCyberBdUI findTurmaByCdTurmaAndCdEscolaApiCyber(int cd_turma, int cd_escola);
        List<ProfessorTurma> findProfessorTurmaByCdTurma(int cd_turma, int cd_escola);
        List<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurma( int cd_turma);
        List<LivroAlunoApiCyberBdUI> findAlunoTurmaAtivosByCdTurmaPPTPai(int cd_turma);
        void deleteProfPPTFilhaMudancaInterna(int cd_turma, int cd_escola);
        IEnumerable<ProfessorTurma> findProfessoresTurmaPorTurmaEscola(int cdTurma, int cd_escola);
        bool crudProgramacaoTurma(List<ProgramacaoTurma> programacaosTurmaView, int cd_turma, int cd_escola);
        List<TurmaEscola> crudAlunosTurmasPPT(List<Turma> alunosTurmaPPT, Turma turmaPaiPPT, bool alteradoNome, int cd_escola, bool horarioDiferente);
        void crudAlunosTurma(List<AlunoTurma> alunoTurmaView, Turma turma);
        TurmaSearch getTurmaByCodForGrid(int cdTurma, int cdEscola, bool turmasFilha);
        TurmaSearch getTurmaByCodMudancaOuEncerramento(int cdTurma, int cdEscola);
        Turma findTurmasByIdAndCdEscola(int cdTurma, int cdEscola);
        Turma findTurmaByCdTurmaApiCyber(int cdTurma);
        void crudFeriadosDesconsiderados(List<FeriadoDesconsiderado> feriadosDesconsiderados, int cd_turma, int cd_escola);
        int getNumeroVagas(int id, int tipoConsulta, int cd_escola);
        List<TurmaSearch> editTurmaEncerramento(List<Turma> turmas, int cd_usuario, int fuso);
        IEnumerable<TurmaSearch> searchTurmaAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                                  int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, int cdAluno, int tipoConsulta, bool retorno, int cd_escola_combo_fk);
        IEnumerable<TurmaSearch> searchTurmasContrato(int cd_contrato, int cd_escola, int cd_aluno);
        bool verifTurmasFilhasDisponiveisHorariosTurmaPPTBD(int cd_turma_PPT, int cd_escola, List<Horario> horariosTurmaPPT);
        void verifTurmasFilhasDisponiveisHorariosTurmaPPT(List<Horario> horariosTurmaPPT, List<Turma> allTurmasFilhas);
        Turma getTurmaEHorarios(int cdTurma, int cdEscola);
        IEnumerable<TurmaSearch> searchTurmaComAluno(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
                                                    int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
                                                    bool turmasFilhas, int cdAluno, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, int cdTurmaOri, int opcao, int tipoConsulta, int cd_escola_combo_fk);
        Turma getTurmaOrigem(int cdEscola, int cd_turma, int cd_aluno);
        List<Turma> getTurmasOrigem(List<AlunoTurma> alunosTurma, int cdEscola);
        bool getVerificaDadosMudanca(List<AlunoTurma> alunos, int cdEscola);
        IEnumerable<TurmaSearch> searchTurmaAlunoDesistente(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma,
                                               int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola, bool turmasFilhas, DateTime? dtInicial,
                                               DateTime? dtFinal, int tipoConsulta);
        Turma getTurmaAlunoAguard(int cd_pessoa_aluno, DateTime dataHoje, int cd_empresa);
        TurmaSearch searchTurmaComAlunoDesistencia(int cdEscola, int cdAluno, int opcao);
        TurmaSearch postCancelarEncerramento(Turma turma, int cdEscola, int cdUsuario);
        int postCancelarTurmasEncerramento(List<Turma> turmas, int? cd_usuario, int? fuso);
        int getQuantidadeAulasProgramadasTurma(int cd_turma, int cd_pessoa_escola);
        Turma buscarTurmaHorariosNovaVirada(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        IEnumerable<Turma> getTurmasPersonalizadas(int cdProduto, DateTime dtAula, TimeSpan hrIni, TimeSpan hrFim, int cdEscola, int? cd_turma);
        List<ReportPercentualTerminoEstagio> getRptPercentualTerminoEstagio(int cd_professor, DateTime? dt_ini, DateTime? dt_fim, int cd_escola);
        bool verificaSeTurmaEFilhaPersonalizada(int cd_turma, int cdEscola);
        IEnumerable<NomeContrato> getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum hasDependente, int? cd_nome_contrato, int? cd_escola);
        string postGerarRematricula(string dc_turma, int cd_usuario, Nullable<System.DateTime> dt_inicial, Nullable<System.DateTime> dt_final, Nullable<bool> id_turma_nova, Nullable<int> cd_layout, Nullable<System.DateTime> dt_termino, int fusoHorario);
        string postCancelarEncerramento(Nullable<int> cd_turma, Nullable<int> cd_usuario, Nullable<int> fuso);
        string postRefazerProgramacao(int cdTurma);
        string postRefazerNumeracao(int cdTurma);

        FuncionarioCyberBdUI findFuncionarioByCdFuncionario(int cd_funcionario, int cd_empresa);

        //Programação Turma
        List<ProgramacaoTurma> criaProgramacaoTurma(ProgramacaoHorarioUI programacao, int cd_escola, ref IEnumerable<Feriado> feriadosEscola);
        List<ProgramacaoTurma> criaProgramacaoTurma(ProgramacaoHorarioUI programacao, int cd_escola, string dc_aula_programacao, ref IEnumerable<Feriado> feriadosEscola);
        ProgramacaoHorarioUI getProgramacaoHorarioTurma(int cd_turma, int cd_escola);
        ProgramacaoTurmaAbaUI getProgramacoesTurma(int cd_turma, int cd_escola);
        ProgramacaoTurmaAbaUI verificaProgramacaoTurma(int cd_turma, int cd_escola);
        IEnumerable<ProgramacaoTurma> getProgramacoesTurma(int cd_escola, int cd_turma, int cd_professor, DateTime? dt_inicial, DateTime dt_final);
        void atualizaProgramacoesTurma(TransactionScopeBuilder.TransactionType transactionType, List<ProgramacaoTurma> programacoes, int cd_turma, int? posicao);
        Boolean atualizaProgramacoesTurma(List<Feriado> feriados, int? cd_escola, bool isMaster, bool inclui_desconsidera_feriado, bool inclusao_feriado, bool delecao_feriado);
        IEnumerable<ProgramacaoTurma> getProgramacaoTurmaByTurma(Componentes.GenericBusiness.TransactionScopeBuilder.TransactionType transactionType, int cd_turma, int cd_escola, ProgramacaoTurmaDataAccess.TipoConsultaProgTurmaEnum tipoConsulta, bool idMostrarFeriado);
        IEnumerable<ReportTurmaMatriculaMaterial> getRptTurmasMatriculaMaterial(int cd_escola, int cd_turma, int cd_aluno, int cd_item, int nm_contrato, DateTime? pDtaI, DateTime? pDtaF);
        Turma buscarTurmaHorariosEditVirada(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        Turma getTurmaComCursoDuracao(int cd_turma);
        Feriado getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro, ref IEnumerable<Feriado> feriadosEscola);
        Feriado getFeriadosDentroOuAposData(int cd_escola, DateTime ultima_data, bool feriado_financeiro, ref IEnumerable<Feriado> feriadosEscola, bool addDias);
        ProgramacoesTurmaSemDiarioAula verificarAlunosPendenciaMaterialDidaticoCurso(int qtd_aulas_sem_material, int cd_turma, int cd_escola, DateTime dt_programacao_turma);
        IEnumerable<ProgramacaoTurma> getProgramacoesTurmaPorAluno(int cd_escola, int cd_aluno, DateTime dt_inicial, int cd_turma_principal, List<int> listaProgs);
        IEnumerable<Turma> getTurmaPoliticaEsc(int cdEscola);

        string existeProgInsuficiente(Turma turma);
        //Avaliação da turma
        IEnumerable<AvaliacaoTurmaUI> searchAvaliacaoTurma(SearchParameters parametros, int idTurma, bool? idTipoAvaliacao, int idEscola, int cdUsuario, int cd_tipo_avaliacao, int cd_criterio_avaliacao, int cd_curso, int cd_funcionario, DateTime? dta_inicial, DateTime? dta_final, bool isMaster, int cd_escola_combo = 0);
        List<AvaliacaoTurma> getAvaliacaoTurmaArvore(int idTurma, int idEscola, bool? idConceito, int cdUsuario, int tipoForm, int cd_tipo_avaliacao);
        AvaliacaoTurmaUI editAvaliacaoTurma(AvaliacaoTurmaUI avaliacaoAlunos, int cdUsuario, int idEscola);
        List<AvaliacaoTurmaUI> returnAvaliacoesConceitoOrNotaByTurma(int cd_turma, int cdUsuario, int idEscola);
        bool existsAvaliacaoTurmaByDesistencia(DateTime data, int cd_pessoa_escola, int cd_aluno, int cd_turma);
        bool deleteAvaliacaoTurma(List<AvaliacaoTurmaUI> avaliacaoTurma, int cd_pessoa_escola, int cd_usuario);
        List<RptBolsistasAval> getAvaliacoesBolsista(int cd_aluno, int cd_pessoa_escola, int cd_turma);
        void DeletarRelacionamentoAvalicaoTurma(IEnumerable<AvaliacaoTurma> avalTurmaDeletar, int cdEscola);
        //Avaliação Aluno
        bool verificaAvalicaoAlunoTurma(int cdTurma, int cdAluno);
        MediaAvaliacaoAlunoUI getMediasAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola);
        MediaAvaliacaoAlunoUI getMediasEstagioAvaliacaoAluno(int cd_aluno, int cd_escola, int cd_estagio);
        IEnumerable<AvaliacaoAluno> getConceitosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola);
        
        bool existeAulaEfetivadaTurma(int? cd_turma,int? cd_turma_ppt, int cdEscola);

        //Histórico Aluno
        IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola);
        IEnumerable<Estagio> getEstagiosHistoricoAluno(int cd_aluno, int cd_escola);
        IEnumerable<TurmaSearch> getRptTurmas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int prog, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int situacaoTurma, List<int> situacaoAlunoTurma, int tipoOnline, string dias);
        IEnumerable<TurmaSearch> getRptTurmasAEncerrar(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
          DateTime? pDtaI, DateTime? pDtaF, DateTime? pDtaFimI, DateTime? pDtaFimF, int cd_escola, int tipoOnline, string dias);
        IEnumerable<TurmaSearch> getRptTurmasNovas(int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int cdProfessor, int tipoProg, bool turmasFilhas,
           DateTime? pDtaI, DateTime? pDtaF, int cd_escola, int tipoOnline, string dias);

        IEnumerable<TurmaSearch> getRptTurmasProgAula(int cd_turma, int cd_escola, DateTime? pDtaI, DateTime? pDtaF);
        Turma getDadosTurmaMud(int cdEscola, int cd_turma);
        int retunMaxSequenciaHistoricoAluno(int cd_produto, int cd_pessoa_escola, int cd_aluno);
        void addHistoricoAluno(HistoricoAluno historicoAluno);
        HistoricoAluno GetHistoricoAlunoPrimeiraAula(int cdEscola, int cdAluno, int cdTurma, int cdContrato, DateTime dataDiario);
        bool deleteHistoricoAluno(HistoricoAluno historico);
        bool getExisteDesistenciaPorAlunoTurma(int cd_aluno, int cd_turma, int cd_pessoa_escola);

        //Evento
        IEnumerable<ReportDiarioAula> getRelatorioDiarioAula(int cd_escola, int cd_turma, int cd_professor, DateTime dt_inicial, DateTime dt_final);
        DiarioAula addDiarioAula(DiarioAula diarioAula);
        vi_diario_aula getDiarioForGridById(int cd_diario_aula, int cd_pessoa_escola);
        IEnumerable<DiarioAula> getDiarioAulas(int[] cdDiarios, int cd_escola);
        DiarioAula getEditDiarioAula(int cd_diario_aula, int cd_escola);
        IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma, bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada);
        string getObsDiarioAula(int cd_diario_aula, int cd_escola);
        int returnQuantidadeDiarioAulaByTurma(int cd_turma, int cd_escola, int cd_aluno);
        int returnDiarioByDataDesistencia(DateTime? data_desistencia, int cd_pessoa_escola, int cd_aluno, int cd_turma_aluno, int tipoDesistencia);
        bool verificaIntersecaoTurmaPersonalizada(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data);
        bool verificaExisteDiarioEfetivoProgramacaoTurma(int cd_turma, int cd_escola, int cd_programacao);
        bool verificaIntersecaoHorarios(int cd_turma, int? cd_programacao_turma, TimeSpan hr_inicial_aula, TimeSpan hr_final_aula, DateTime data);
        IEnumerable<vi_diario_aula> searchDiarioAula(SearchParameters parametros, int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf,
                                                     bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal, int cd_escola, int? cdProf, int cd_escola_combo = 0);
        bool deletarDiarioAula(DiarioAula diarioAula);

        DiarioAula getUltimaAulaAluno(int cd_aluno, int cd_turma, int cd_escola);
        DiarioAula getUltimaAulaTurma(int cd_turma, int cd_escola);
       
        //Diário de Aula
        ProgramacoesTurmaSemDiarioAula getProgramacoesTurmasSemDiarioAula(int cd_turma, int cd_escola, int qtd_aulas_sem_material, byte nm_dias_titulos_abertos);

        //Diário de Aula
        int getQtdDiarioTurma(int cd_turma, DateTime dt_aula);
        
        //Professor Business
        IEnumerable<FuncionarioSGF> getFuncionariosByAulaPers(int cdEscola);

        //Avaliação
        IEnumerable<AvaliacaoUI> searchAvaliacao(SearchParameters parametros, string descAbreviado, int? idTipoAvaliacao, int? idCriterio, bool inicio, bool? status);
        IEnumerable<Avaliacao> getAvaliacaoOrdem(int idTipoAvaliacao, int idCriterio, bool? status);
        IEnumerable<AvaliacaoUI> editAvaliacao(AvaliacaoOrdem entity, int cdEscola);
        bool deleteAllAvaliacao(List<Avaliacao> avaliacoes);
        //List<Avaliacao> editOrdemAvaliacao(List<Avaliacao> entity);
        byte? getSomatorio(int idTipoAvaliacao, int idCriterio);
        byte? getNotaAvaliacao(int idAvaliacao);
        bool existNotaLancadaAvaliacaoTurma(int cd_avaliacao, int cd_escola);
        IEnumerable<AvaliacaoUI> getAvaliacaoByIdTipoAvaliacao(int idTipoAvaliacao);
        int getAvaliacaoCursoExistsTurmaWithCurso(int cd_turma, int cd_escola);
        IEnumerable<Avaliacao> getAvaliacaoECriterioTurma(int cd_turma, int cd_escola);
        void persistAvaliacoes(int cd_tipo_avaliacao, List<Avaliacao> avaliacoes, int cdEscola);

        //Feriado
        IEnumerable<Feriado> getDescFeriado(SearchParameters parametros, string desc, bool inicio, bool? status, int cdEscola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, bool? somenteAno, bool idFeriadoAtivo);
        Feriado findByIdFeriado(int id);
        List<Feriado> getFeriadosPorPeriodo(Feriado feriado, int? cd_escola);
        Feriado editFeriado(Feriado entity, int cd_escola, string login, ref bool refez_programacoes, int cd_usuario);
        bool deleteAllFeriado(List<Feriado> feriados, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario);
        bool deletarFeriados(List<Feriado> feriados, ref Boolean refez_programacoes, int cd_escola);
        Feriado addFeriado(Feriado entity, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario);

        // Horário
        IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem);

        //Controle de Faltas
        IEnumerable<TurmaSearch> getTurmasComPercentualFaltaSearch(SearchParameters parametros, string descricao, string apelido, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto,
            int situacaoTurma, int cdProfessor, ProgramacaoTurma.TipoConsultaProgramacaoEnum tipoProg, int cdEscola,
            bool turmasFilhas, int cdAluno, DateTime? dtInicial, DateTime? dtFinal, int? cd_turma_PPT, bool semContrato, int tipoConsulta, DateTime? dt_inicial, DateTime? dt_final, bool ProfTurmasAtuais, bool id_percentual_faltas, List<int> cdSituacoesAlunoTurma = null);

        Turma findTurmaPercentualFaltaGrupoAvancado(int cdEscola, int cd_turma, int? cd_turma_ppt, bool id_turma_ppt);
        void editTurmaControleFalta(int cd_turma);

        List<FuncionarioSearchUI> getProfessoresByEmpresa(int cd_escola, int cd_turma);
        int incluirAvaliacoesTurma(int idTurma, FuncionarioSGF professor);
        List<Horario> verificaDiaSemanaTurmaFollowUp(int cdEscola, int cdTurma, int idDiaSemanaTurma);
        bool cancelarProgramacaoTurma(List<int> cds_programacao_turma, int cd_pessoa_escola);
        bool desfazerCancelarProgramacaoTurma(List<int> cds_programacao_turma, int cd_pessoa_escola);

        void sp_verificar_grupo_cyber(string no_turma, Nullable<int> id_unidade);
    }
}
