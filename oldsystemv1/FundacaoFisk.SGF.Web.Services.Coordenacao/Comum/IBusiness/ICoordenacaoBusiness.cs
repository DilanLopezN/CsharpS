using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using Componentes.GenericBusiness.Comum;
using Componentes.GenericModel;
using Componentes.Utils;
using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
using FundacaoFisk.SGF.GenericModel;
using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.DataAccess;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
using FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness;
using Componentes.GenericBusiness;
using FundacaoFisk.SGF.Services.Coordenacao.Model;
using System.Data.SqlClient;
using System.Data;
using FundacaoFisk.SGF.Web.Services.Empresa.Model;
using FundacaoFisk.SGF.GenericModel.Partial;

namespace FundacaoFisk.SGF.Web.Services.Coordenacao.Comum.IBusiness
{
    public interface ICoordenacaoBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);

        // void configuraUsuario(int cdUsuario);
        
        //Estagio
        IEnumerable<Estagio> findAllEstagio();
        IEnumerable<EstagioSearchUI> getDescEstagio(SearchParameters parametros, string desc, string abrev, bool inicio, bool? status, int cdProduto);
        IEnumerable<Estagio> getOrdemEstagio(int cdProduto, int? cd_estagio, EstagioDataAccess.TipoConsultaEstagioEnum? tipoConsulta);
        Estagio findByIdEstagio(int id);
        EstagioSearchUI addEstagio(Estagio entity);
        IEnumerable<Estagio> editEstagio(EstagioOrdem entity);
        //List<GenericModel.Estagio> editOrdem(List<Estagio> entity);
        Boolean deleteEstagio(Estagio entity);
        Boolean deleteAllEstagio(List<Estagio> estagios);
        IEnumerable<Estagio> getAllEstagioByProduto(int cdProduto);

        //Conceito
        IEnumerable<Conceito> findAllConceito();
        IEnumerable<ConceitoSearchUI> getDescConceito(SearchParameters parametros, string desc, bool inicio, bool? status, int cdProduto);
        Conceito findByIdConceito(int id);
        ConceitoSearchUI addConceito(ConceitoSearchUI entity, int cdEscola);
        ConceitoSearchUI editConceito(ConceitoSearchUI entity, int cdEscola);
        Boolean deleteConceito(Conceito entity);
        Boolean deleteAllConceito(List<Conceito> conceitos);
        IEnumerable<Conceito> findConceitosAtivos(int idProduto, int idConceito);
        void verificaConceitoParticipacao(int idProduto, int cdEscola, double vlParticipacao, int? cdConceito);
        List<Conceito> getConceitosDisponiveisByProdutoTurma(int cd_turma);
        
        //Duracao
        IEnumerable<Duracao> findAllDuracao();
        IEnumerable<Duracao> getDescDuracao(SearchParameters parametros, string desc, bool inicio, bool? status);
        Duracao findByIdDuracao(int id);
        Duracao addDuracao(Duracao entity);
        Duracao editDuracao(Duracao entity);
        Boolean deleteDuracao(Duracao entity);
        Boolean deleteAllDuracao(List<Duracao> duracoes);
        IEnumerable<Duracao> getDuracoes(DuracaoDataAccess.TipoConsultaDuracaoEnum hasDependente, int? cd_duracao, int? cd_escola);
        IEnumerable<Duracao> getDuracaoProgramacao();
        IEnumerable<Duracao> getDuracaoTabelaPreco();
      
        //Evento
        IEnumerable<Evento> findAllEvento();
        IEnumerable<Evento> getDescEvento(SearchParameters parametros, string desc, bool inicio, bool? status);
        Evento findByIdEvento(int id);
        Evento addEvento(Evento entity);
        Evento editEvento(Evento entity);
        Boolean deleteEvento(Evento entity);
        Boolean deleteAllEvento(List<Evento> eventos);
        IEnumerable<AlunoEventoReport> getRelatorioEventos(int cd_escola, int? cd_turma, int? cd_professor, int? cd_evento, int? qtd_faltas, bool falta_consecultiva, bool mais_turma_pagina,
                DateTime? dt_inicial, DateTime? dt_final);
        IEnumerable<DiarioAulaProgramcoesReport> getRelatorioDiarioAulaProgramacoes(int cd_escola, int? cd_turma, bool mais_turma_pagina, DateTime? dt_inicial, DateTime? dt_final, bool lancada);

        //Modalidade
        IEnumerable<Modalidade> findAllModalidade();
        IEnumerable<Modalidade> getDescModalidade(SearchParameters parametros, string desc, bool inicio, bool? status);
        Modalidade findByIdModalidade(int id);
        Modalidade addModalidade(Modalidade entity);
        Modalidade editModalidade(Modalidade entity);
        Boolean deleteModalidade(Modalidade entity);
        Boolean deleteAllModalidade(List<Modalidade> modalidades);
        IEnumerable<Modalidade> getModalidades(ModalidadeDataAccess.TipoConsultaModalidadeEnum? tipoConsulta, int? cd_modalidade);
        IEnumerable<Evento> getEventos(int cd_evento, EventoDataAccess.TipoConsultaEventoEnum tipoConsulta);

        //MotivoDesistencia
        IEnumerable<MotivoDesistencia> findAllMotivoDesistencia();
        IEnumerable<MotivoDesistencia> getDescMotivoDesistencia(SearchParameters parametros, string desc, bool inicio, bool? status, bool isCancelamento);
        MotivoDesistencia findByIdMotivoDesistencia(int id);
        MotivoDesistencia addMotivoDesistencia(MotivoDesistencia entity);
        MotivoDesistencia editMotivoDesistencia(MotivoDesistencia entity);
        Boolean deleteMotivoDesistencia(MotivoDesistencia entity);
        Boolean deleteAllMotivoDesistencia(List<MotivoDesistencia> desitencias);
        IEnumerable<MotivoDesistencia> getMotivoDesistenciaByCancelamento(bool isCancelamento);

        //MotivoFalta
        IEnumerable<MotivoFalta> findAllMotivoFalta();
        IEnumerable<MotivoFalta> getDescMotivoFalta(SearchParameters parametros, string desc, bool inicio, bool? status);
        MotivoFalta findByIdMotivoFalta(int id);
        MotivoFalta addMotivoFalta(MotivoFalta entity);
        MotivoFalta editMotivoFalta(MotivoFalta entity);
        Boolean deleteMotivoFalta(MotivoFalta entity);
        Boolean deleteAllMotivoFalta(List<MotivoFalta> motivos);
        IEnumerable<MotivoFalta> getMotivoFaltaAtivo(int cdDiario);

        //Produto
        IEnumerable<Produto> findAllProduto();
        IEnumerable<Produto> getDescProduto(SearchParameters parametros, string desc, string abrev, bool inicio, bool? status);
        Produto findByIdProduto(int id);
        Produto addProduto(Produto entity);
        Produto editProduto(Produto entity);
        Boolean deleteProduto(Produto entity);
        IEnumerable<Produto> findProduto(ProdutoDataAccess.TipoConsultaProdutoEnum hasDependente, int? cd_produto,int? cd_escola);
        IEnumerable<Produto> findProdutoAulaPersonalizada(int cd_aluno, int cd_escola);
        Boolean deleteAllProduto(List<Produto> produtos);
        IEnumerable<Produto> findProdutoTabela(int cdEscola);
        IEnumerable<Produto> getProdutosWithAtividadeExtra(int cd_pessoa_escola, bool isAtivo);

        //Regime
        IEnumerable<Regime> findAllRegime();
        IEnumerable<Regime> getDescRegime(SearchParameters parametros, string desc, string abrev, bool inicio, bool? status);
        Regime findByIdRegime(int id);
        Regime addRegime(Regime entity);
        Regime editRegime(Regime entity);
        Boolean deleteRegime(Regime entity);
        Boolean deleteAllRegime(List<Regime> regimes);
        IEnumerable<Regime> getRegimes(RegimeDataAccess.TipoConsultaRegimeEnum hasDependente, int? cd_regime);
        IEnumerable<Regime> getRegimeTabelaPreco();      

        //Sala
        IEnumerable<Sala> findAllSala();
        IEnumerable<SalaSearchUI> getDescSala(SearchParameters parametros, string desc, bool inicio, bool? status, int cd_escola, bool online);
        Sala findByIdSala(int id);
        Sala addSala(Sala entity);
        Sala editSala(Sala entity);
        Boolean deleteSala(Sala entity);
        Boolean deleteAllSala(List<Sala> salas);
        IEnumerable<Sala> findListSalasDiponiveis(TimeSpan horaIni, TimeSpan horaFim, DateTime data, bool? status, int? cdSala, int cdEscola, int? cd_atividade_extra);
        IEnumerable<Sala> findListSalasTurmas(int cdEscola);
        IEnumerable<Sala> findSalasTurmas(int cdEscola, bool online);
        IEnumerable<Sala> findListSalas(bool? status, int? cdSala, int cdEscola);
        IEnumerable<Sala> getSalasDisponiveisPorHorarios(Turma turma, int cd_escola);
        IEnumerable<Sala> getSalasDisponiveisPorHorariosByModalidadeOnline(Turma turma, int cd_escola);
        IEnumerable<Sala> getSalas(int cd_sala, int cd_escola, SalaDataAccess.TipoConsultaDuracaoEnum tipoConsulta);
        IEnumerable<Sala> getSalasAulaReposicao(int cd_escola, SalaDataAccess.TipoConsultaDuracaoEnum tipoConsulta);
        List<ReportControleSala> getHorariosRptControleSala(TimeSpan? hIni, TimeSpan? hFim, int cd_turma, int cd_professor, int cd_sala, List<int> diasSemana, int cd_escola);
        IEnumerable<Sala> findListSalasAulaPer(int cdEscola);
        IEnumerable<Sala> findListSalasDiponiveisAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, bool? status, int? cdSala, int cdEscola, int? cd_aula_reposicao, int? cd_turma);

        //Tipo Atividade Extra
        IEnumerable<TipoAtividadeExtra> findAllTipoAtv();
        IEnumerable<TipoAtividadeExtra> getDescTipoAtv(SearchParameters parametros, string desc, bool inicio, bool? status);
        TipoAtividadeExtra findByIdTipoAtv(int id);
        TipoAtividadeExtra addTipoAtv(TipoAtividadeExtra entity);
        TipoAtividadeExtra editTipoAtv(TipoAtividadeExtra entity);
        Boolean deleteTipoAtv(TipoAtividadeExtra entity);
        Boolean deleteAllTipoAtividade(List<TipoAtividadeExtra> tiposAtividades);
        IEnumerable<TipoAtividadeExtra> getTipoAtividade(bool? status, int? cdTipoAtividadeExtra, int? cd_pessoa_escola, TipoAtividadeExtraDataAccess.TipoConsultaAtivExtraEnum tipoConsulta);
        IEnumerable<TipoAtividadeExtra> getTipoAtividadeWhitAtividadeExtra(int cd_pessoa_escola);
        List<AtividadeExtra> gerarRecorrenciaAtividadeExtra(AtividadeExtraRecorrenciaUI atividade, int idEscola);

        //Feriado
        IEnumerable<Feriado> getDescFeriado(SearchParameters parametros, string desc, bool inicio, bool? status, int cd_escola, int Ano, int Mes, int Dia, int AnoFim, int MesFim, int DiaFim, bool? somenteAno, bool idFeriadoAtivo);
        Feriado findByIdFeriado(int id);
        List<Feriado> getFeriadosPorPeriodo(Feriado feriado, int? cd_escola);
        Feriado addFeriado(Feriado entity, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario);
        Feriado editFeriado(Feriado entity, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario);
        Boolean deleteAllFeriado(List<Feriado> feriados, int cd_escola, string login, ref Boolean refez_programacoes, int cd_usuario);
        OpcoesPagamentoUI calculaSugestaoOpcoesPgto(Parametro parametro, DateTime data_matricula, int cd_escola, int? cd_curso, int? cd_duracao, int? cd_regime);
        void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola, ref IEnumerable<Feriado> feriadosEscola);
        void pulaFeriadoEFinalSemana(ref DateTime data_opcao, int cd_escola, ref IEnumerable<Feriado> feriadosEscola, bool addDias); 
        
        // Critérios de Avaliações
        CriterioAvaliacao getCriterioAvaliacaoById(int id);
        CriterioAvaliacao postCriterioAvaliacao(CriterioAvaliacao criterioAvaliacao);
        CriterioAvaliacao putCriterioAvaliacao(CriterioAvaliacao criterioAvaliacao);
        bool deleteAllCriterioAvaliacao(List<CriterioAvaliacao> criteriosAvaliacao);
        IEnumerable<CriterioAvaliacao> getCriterioAvaliacaoSearch(SearchParameters parametros, string descricao, string abrev, bool inicio, bool? status, bool? conceito, bool IsParticipacao);
        IEnumerable<CriterioAvaliacao> getAllCriteriosAtivos(bool? ativo, int? cdCriterio);
        List<CriterioAvaliacao> getAvaliacaoCriterio(int? cd_tipo_avaliacao, int? cd_criterio_avaliacao);
        IEnumerable<CriterioAvaliacao> getNomesAvaliacao();
        IEnumerable<CriterioAvaliacao> getNomesAvaliacao(int cd_tipo_avaliacao);
        IEnumerable<CriterioAvaliacao> getCriteriosPorAvalPart(int cdEscola);
        IEnumerable<CriterioAvaliacao> getNomesAvaliacaoByAval(int? cdCriterio);

        // Tipos de Avaliações
        TipoAvaliacao getTipoAvaliacaoById(int id);
        TipoAvaliacao postTipoAvaliacao(TipoAvaliacao tipoAvaliacao);
        TipoAvaliacao putTipoAvaliacao(TipoAvaliacao tipoAvaliacao, int cdEscola);
        bool deleteAllTipoAvaliacao(List<TipoAvaliacao> tipoAvaliacao, int cdEscola);
        IEnumerable<TipoAvaliacao> getTipoAvaliacaoSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int? cd_tipo_avaliacao, int? cd_criterio_avaliacao, int cdCurso, int cdProduto);
        IEnumerable<AvaliacaoCurso> getCursoTipoAvaliacao(int cdTipoAvaliacao);
        IEnumerable<TipoAvaliacao> getTipoAvaliacao(bool? ativo, int idTipoAvaliacao);
        int? getTotalNotaTipoAvaliacao(int idtipoAvaliacao);
        List<TipoAvaliacao> getTipoAvaliacao();
        bool verificaNotaCurso(List<Curso> curso, int? nota);
        IEnumerable<TipoAtividadeExtra> getTipoAtividade();
        bool validaNotaAvaliacao(int? valorTotalNota, List<Avaliacao> avaliacoes);
        IEnumerable<TipoAvaliacao> getTipoAvaliacaoAvaliacaoTurma();
        List<TipoAvaliacaoTurma> tiposAvaliacao(int cd_turma, int cd_escola, bool id_conceito);

        //Avaliacao
        DataTable getRptAvaliacao(int cd_turma, int cdCurso, int cdProduto, int cdEscola, int cdFuncionario, int tipoTurma, byte sitTurma, DateTime? pDtIni, DateTime? pDtFim, bool isConceito);
        DataTable getRptAvaliacaoTurma(int cd_turma);

        DataTable getRptAvaliacaoTurmaConceito(int cd_turma);
        //Programação Curso
        ProgramacaoCursoUI postProgramacaoCurso(Programacao programacao);
        ProgramacaoCursoUI putProgramacaoCurso(Programacao programacao);
        bool deleteAllProgramacaoCurso(List<ProgramacaoCurso> programacaoCurso);
        IEnumerable<ProgramacaoCursoUI> getProgramacaoCursoSearch(SearchParameters parametros, int? cdCurso, int? cdDuracao, int? cd_escola);
        ProgramacaoCurso GetProgramacaoCursoById(int cdProgramacao);
        List<ProgramacaoTurma> criaProgramacaoTurmaCurso(ProgramacaoHorarioUI programacao, int cd_escola, bool? modelo);
        List<ProgramacaoTurma> atualizaProgramacaoTurma(List<ProgramacaoTurma> programacoes, ProgramacaoHorarioUI programacao, int cd_escola);
        List<ProgramacaoTurma> atualizaProgramacaoTurmaAposDiarioAula(List<ProgramacaoTurma> programacoes, ProgramacaoHorarioUI programacao, int cd_escola, ref Boolean refez_programacoes);
        void geraModeloProgramacoesTurma(List<int> cd_turmas, int cd_escola);
        void criaProgramacoesTurmaCurso(List<int> cd_turmas, int cd_escola);
        void criaProgramacoesTurmaCurso(List<int> cd_turmas, int cd_escola, bool modelo);
        List<ItemProgramacao> geraModeloProgramacoesTurma(ProgramacaoHorarioUI programacaoUI, int cd_escola);

        //Item programação
        IEnumerable<ItemProgramacao> getCursoProg(int cdCurso, int cdDuracao, int? cd_escola);
        IEnumerable<ItemProgramacao> getItensProgramacaoCursoById(int cdProgramacao);
        void criaProgramacoesTurmaTurma(int cd_turma_origem, int cd_turma_destino, int cd_escola, bool cancelar);
        List<ProgramacaoTurma> criaProgramacaoTurmaTurma(List<ProgramacaoTurma> programacaoTurma, ProgramacaoHorarioUI programacao, int cd_escola, bool cancelar);
        //Atividade Extra
        IEnumerable<AtividadeExtraUI> searchAtividadeExtra(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? tipoAtividade, int? curso, int? responsavel, int? produto, int? aluno, byte lancada, int cdEscola, int cd_escola_combo = 0);
        AtividadeExtraUI addAtividadadeExtra(AtividadeExtraUI atividadeExtra);
        AtividadeExtraUI editAtividadeExtra(AtividadeExtraUI atividadeExtra);
        AtividadeExtraUI editAtividadeExtraOutraEscola(AtividadeExtraUI atividadeExtra, int cdEscola);
        AtividadeExtra findAtividadeExtraById(int cd_atividade_extra);
        bool deleteAllAtividadeExtra(List<AtividadeExtra> atividadesExtras, int cd_escola);
        bool deleteRecorrencias(AtividadeExtra atividadeExtra, int cd_escola);
        List<string> enviarEmailRecorrencias(AtividadeExtra atividadeExtra, int cd_escola);
        List<string> enviarEmailProspectsAcaoRelacionada(List<AtividadeExtra> atividadeExtra, int cd_escola);
        AtividadeExtra findByIdAtividadeExtraFull(int cdAtividadeExtra);
        List<sp_RptAtividadeExtra_Result> getReportAtividadeExtra(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_produto, Nullable<int> cd_curso, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao, Nullable<byte> id_lancada);
        List<sp_RptAtividadeExtraAluno_Result> getReportAtividadeExtraAluno(Nullable<int> cd_atividade_extra, Nullable<int> cd_aluno, Nullable<byte> id_participou, Nullable<byte> id_lancada, Nullable<int> cd_escola);
        AtividadeExtraUI returnAtividadeExtraUsuarioAtendente(int cd_atividade_extra, int cd_pessoa_escola);
       
        //Atividade Aluno
        IEnumerable<AtividadeAlunoUI> searchAtividadeAluno(int cdAtividadeExtra, int cdEscola);
        IEnumerable<AtividadeAlunoUI> searchAtividadeAlunoReport(int cdAtividadeExtra, int cdEscola);
        long retornNumbersOfStudents(int idAtividadeExtra, int cdEscola);

        //Atividade Prospect
        IEnumerable<AtividadeProspectUI> searchAtividadeProspect(int cdAtividadeExtra, int cdEscola);
        long retornNumbersOfStudentsProspect(int idAtividadeExtra, int cdEscola);
        IEnumerable<AtividadeProspectUI> searchAtividadeProspectByCdProspect(int cdProspect, int cdEscola);

        //Desconto por Antecipação
        PoliticaDescontoUI postPoliticaDesconto(PoliticaDesconto politicaDesconto);

        //Turma
        string criarNomeTurma(bool idTurmaPPT, int? cdTurmaPPT, int? codRegime, int cdProduto, int? codCurso, List<Horario> listahorario, DateTime inicioAulas);
        TurmaSearch addTurma(Turma turma);
        TurmaSearch editTurma(Turma turma);
        TurmaSearch postNovaTurmaEnc(int cdTurma, int cdEscola, TurmaDataAccess.TipoConsultaTurmaEnum tipo);
        Turma GerarTurmaFilha(Contrato contrato, int cd_turma_pp, int cd_curso);

        //Plano Contas
        IEnumerable<PlanoConta> getPlanoContaByIdEscola(int cd_pessoa_escola);

        //titulo
        int? isNumber(string nmTitulo);
        List<Titulo> gerarTitulos(Contrato contrato, bool? diaUtil, byte? nmDia, bool id_alterar_venc_final_semana);
        List<Titulo> gerarTitulosAditamento(Contrato contrato, Parametro parametro);
        void simularBaixaContrato(Contrato contrato, ref BaixaTitulo baixa, Parametro parametro);
        void simularBaixaTitulo(Titulo titulo, ref BaixaTitulo baixa, Parametro parametro, int cd_escola, bool contaSegura, bool gerarMensagem);
        void calcularBaixaTituloTeste(Contrato contrato, Titulo titulo, ref BaixaTitulo baixa, Parametro parametro);
        bool existeAdtAdicionarParcelaBaixado(List<Titulo> titulos, Titulo tituloViewAdt);
        List<Titulo> alterarLocalMovtoTitulos(List<Titulo> titulosAlterarLocalMovto, int nm_parcelas_mensalidade);

        // Diário de Aula
        IEnumerable<vi_diario_aula> searchDiarioAula(SearchParameters parametros, int cd_turma, string no_professor, int cd_tipo_aula, byte status, byte presProf,
                                                       bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal, int cd_escola, int? cdProf, int cd_escola_combo = 0);
        DiarioAula getEditDiarioAula(int cd_diario_aula, int cd_escola);
        vi_diario_aula addDiarioAula(DiarioAula diario, bool aulaPersonalizada);
        bool deleteDiarios(int[] cdDiarios, int cd_escola);
        void cancelarDiarioAula(int cd_diario_aula, int cd_pessoa_escola);
        vi_diario_aula editDiarioAula(DiarioAula diario);
        int returnQuantidadeDiarioAulaByTurma(int cd_turma, int cd_escola, int cd_aluno);
        int returnDiarioByDataDesistencia(DateTime? data_desistencia, int cd_pessoa_escola, int cd_aluno, int cd_turma_aluno, int tipoDesistencia);
        string getObsDiarioAula(int cd_diario_aula, int cd_escola);

        //Mudanças Internas
        MudancasInternas postMudancaTurma(MudancasInternas mudanca);

        //Desistência
        IEnumerable<MotivoDesistencia> motivosDesistenciaWhitDesistencia(int cd_pessoa_empresa);

        //Histórico do Aluno
        IEnumerable<Turma> getTurmasAvaliacoes(int cd_aluno, int cd_escola);
        IEnumerable<Estagio> getEstagiosHistoricoAluno(int cd_aluno, int cd_escola);
        IEnumerable<AtividadeExtra> getAtividadesAluno(SearchParameters parametros, int cd_aluno, int cd_escola);
        EventoHistoricoUI getEventosAvaliacaoAluno(int cd_aluno, int cd_turma, int cd_escola);

        //Aluno evento
        IEnumerable<AlunoEvento> getEventosAlunoByDataDesistencia(int cd_aluno, int cd_turma, int cd_escola, DateTime dta_Desistencia);
        bool deleteAllAlunoEvento(List<AlunoEvento> alunoEventos);

        //Contrato
        void getVerificaReciboConfirmacao(int cd_contrato, int cd_escola);
        void getVerificaReciboConfirmacaoMovimento(int cd_movimento, int cd_escola);
        ContratoUI GetMatriculaByIdPesq(int id, int cdEscola);

        ContratoComponentesUI GetMatriculaByIdComponentesPesq(int id, int cdEscola);
        ContratoComponentesBancoUI GetMatriculaComponentBancoPesq();
        Contrato componentesNovaMatricula(int? cdDuracao, int? cdProduto, int? cdRegime, int? cd_nome_contrato,int? cd_ano_escolar, int? cd_motivo_bolsa, int cdEscola);

        //Carnê
        List<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, Parametro parametro, bool contaSegura, int parcIniCarne, int parcFimCarne);
        List<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola, Parametro parametro, bool contaSegura);

        //Aula Personalizada
        AulaPersonalizada getPesqAulaPersonalizada(int cdEscola);
        IEnumerable<AulaPersonalizadaUI> searchAulaPersonalizada(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim, TimeSpan? hrInicial, TimeSpan? hrFinal, int? cdProduto, int? cdProfessor,
                                                                      int? cdSala, int? cdAluno, bool participou, int cdEscola);
        IEnumerable<AulaPersonalizada> addAulaPersonalizada(AulaPersonalizada aulaPersonalizada);
        IEnumerable<AulaPersonalizada> searchAulaPersonalizadaPesq(int cdAulaPersonalizada, int cdEscola);
        AulaPersonalizada searchAulaPersonalizadaById(int cdAulaPersonalizada, int cdEscola, int cd_aluno);
        IEnumerable<AulaPersonalizada> editAulaPersonalizada(AulaPersonalizada aulaPersonalizada);
        bool deleteAllAulaPersonalizada(List<AulaPersonalizada> aulasPersonalizadas, int cdEscola);
        IEnumerable<AulaPersonalizadaReport> getReportAulaPersonalizada(int cd_empresa, int cd_aluno, int? cd_produto, int? cd_curso, DateTime? dt_inicial_agend, DateTime? dt_final_agend,
                DateTime? dt_inicial_lanc, DateTime? dt_final_lanc, TimeSpan? hr_inicial_agend, TimeSpan? hr_final_agend, TimeSpan? hr_inicial_lanc, TimeSpan? hr_final_lanc);

        //Avaliação Participação
        int verificaMaiorQntParticipacao(int idProduto, int cdEscola);
        IEnumerable<AvaliacaoParticipacaoUI> searchAvaliacaoParticipacao(Componentes.Utils.SearchParameters parametros, int cdCriterio, int cdParticipacao, int cdProduto, bool? ativo, int cdEscola);
        IEnumerable<AvaliacaoParticipacao> addAvaliacaoParticipacao(AvaliacaoParticipacao avalPart, int cdEscola);
        AvaliacaoParticipacao getAvaliacaoParticipacaoByEdit(int cdAvalPart, int cdEscola);
        IEnumerable<AvaliacaoParticipacao> editAvaliacaoParticipacao(AvaliacaoParticipacao avaliacaoParticipacao, int cdEscola);
        bool deleteAllAvaliacaoParticipacao(List<AvaliacaoParticipacao> avaliacoesPart, int cdEscola);

        //Participação
        List<Participacao> getParticipacaoAvaliacaoPart(int cdEscola);
        List<Participacao> getParticipacaoByAvaliacao(int cdAvalPart, int cdEscola);
        List<Participacao> getParticipacoes(string cdsPart);
        IEnumerable<Participacao> getParticipacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status);        
        IEnumerable<Participacao> findAllParticipacao();
        Participacao findByIdParticipacao(int id);
        Participacao addParticipacao(Participacao entity);
        Participacao editParticipacao(Participacao entity);
        Boolean deleteParticipacao(Participacao entity);
        Boolean deleteAllParticipacao(List<Participacao> participacoes);


        //Nivel       
        List<Nivel> getNiveis(string cdsNiv);
        IEnumerable<Nivel> getNivelSearch(SearchParameters parametros, string desc, bool inicio, bool? status);
        IEnumerable<Nivel> findNivel(NivelDataAccess.TipoConsultaNivelEnum hasDependente);
        IEnumerable<Nivel> findAllNivel();
        Nivel findByIdNivel(int id);
        int GetUltimoNmOrdem(int cd_nivel);
        Nivel addNivel(Nivel entity);
        Nivel editNivel(Nivel entity);
        Boolean deleteNivel(Nivel entity);
        Boolean deleteAllNivel(List<Nivel> niveis);

        //Carga Professor
        IEnumerable<CargaProfessorSearchUI> getCargaProfessorSearch(SearchParameters parametros, int qtd_minutos_duracao, int cd_escola);
        IEnumerable<CargaProfessor> findCargaProfessorAll(int cd_escola);
        CargaProfessor findCargaProfessorById(int id, int cd_escola);
        CargaProfessor addCargaProfessor(CargaProfessor entity, int cd_escola);
        CargaProfessor editCargaProfessor(CargaProfessor entity, int cd_escola);
        Boolean deleteCargaProfessor(CargaProfessor entity, int cd_escola);
        Boolean deleteAllCargaProfessor(List<CargaProfessor> cargaProfessores, int cd_escola);

        //Calendário Evento
        CalendarioEvento addCalendarioEvento(CalendarioEvento calendarioEvento);
        CalendarioEvento editarCalendarioEvento(CalendarioEvento calendarioEvento);
        bool deletarCalendarioEvento(List<int> cd_calendario_evento, int cd_escola);
        IEnumerable<CalendarioEvento> obterListaCalendarioEventos(int cdEscola);
        IEnumerable<CalendarioEvento> obterCalendarioEventosPorFiltros(SearchParameters parametros, int cd_escola, string dc_titulo_evento, bool inicio, bool? status,
            string dt_inicial_evento, string dt_final_evento, string hh_inicial_evento, string hh_final_evento);
        CalendarioEvento obterCalendarioEventoPorID(int cd_calendario_evento, int cd_escola);

        //Calendário Academico
        CalendarioAcademico addCalendarioAcademico(CalendarioAcademico calendarioAcademico);
        CalendarioAcademico editarCalendarioAcademico(CalendarioAcademico calendarioAcademico);
        CalendarioAcademico editarCalendarioMaster(List<int> cd_escolas, CalendarioAcademico calendarioAcademico);
        //bool deletarCalendarioAcademico(List<int> cd_calendario_academico, int cd_escola);
        bool deletarCalendarioAcademicoMaster(CalendarioAcademico calendarioAcademico);
        IEnumerable<CalendarioAcademico> obterListaCalendarioAcademicos(int cdEscola);
        IEnumerable<CalendarioAcademico> obterCalendarioAcademicosPorFiltros(SearchParameters parametros, int cd_escola, int tipo_calendario, bool? status, bool relatorio);
        CalendarioAcademico obterCalendarioAcademicoPorID(int cd_calendario_academico, int cd_escola);
        List<int> findEscolas();
        CalendarioAcademico findCalendarioAcademico(int item, byte tipo);

        //Video
        IEnumerable<Video> getVideoSearch(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu);
        Video addVideo(Video video);
        Video obterVideoPorID(int cd_video);
        Video editarVideo(Video video);
        bool deletarVideo(List<int> videos);
        IEnumerable<Video> obterVideosPorFiltros(Componentes.Utils.SearchParameters parametros, string no_video, int nm_video, List<byte> menu);
        Video obterVideoPorNumeroParte(int nm_video, int nm_parte);
        Video obterVideoPorNome(string no_video);

        //Faq
        Faq addFaq(Faq faq);
        Faq obterFaqPorID(int cd_faq);
        Faq editarFaq(Faq faq);
        bool deletarFaq(List<int> faqs);
        IEnumerable<Faq> obterFaqsPorFiltros(Componentes.Utils.SearchParameters parametros, string dc_faq_pergunta, bool dc_faq_inicio, List<byte> menu);
        Faq obterFaqPorNumeroParte(int nm_faq, int nm_parte);
        Faq obterFaqPorNome(string no_faq);

        //Circular
        Circular addCircular(Circular circular);
        Circular editarCircular(Circular circular);
        bool deletarCircular(List<int> circulares);
        IEnumerable<Circular> obterListaCirculares();
        IEnumerable<Circular> obterCircularesPorFiltros(SearchParameters parametros, short nm_ano_circular, List<byte> nm_mes_circular, int nm_circular, string no_circular, List<byte> nm_menu_circular);
        Circular obterCircularPorID(int cd_circular);

        //ControleFaltas
        IEnumerable<ControleFaltasUI> getControleFaltasSearch(Componentes.Utils.SearchParameters parametros, string desc, int cd_turma, int cd_aluno, int assinatura, DateTime? dtInicial, DateTime? dtFinal, int cdEscola);
        ControleFaltasUI addAlunoControleFaltas(ControleFaltasUI item);
        ControleFaltasUI editarAlunoControleFaltas(ControleFaltasUI item);
        bool deleteAllControleFaltas(List<int> itens);
        ControleFaltas getControleFaltasById(int cd_controle_faltas);
        List<sp_RptControleFaltas_Result> getReportControleFaltasResults(Nullable<int> cd_tipo, int cd_escola,
            Nullable<int> cd_curso, Nullable<int> cd_nivel,
            Nullable<int> cd_produto, Nullable<int> cd_professor, Nullable<int> cd_turma, Nullable<int> cd_sit_turma,
            string cd_sit_aluno, string dt_inicial,
            string dt_final, bool quebrarpagina);

        //ControleFaltasAluno
        IEnumerable<ControleFaltasAlunoUI> getAlunosTurmaControleFalta(int cd_turma, int cd_pessoa_escola, DateTime? dt_inicial, DateTime dt_final, int cd_controle_faltas);
        ControleFaltasAlunoUI getAlunoControleFalta(int cd_turma, int cd_pessoa_escola, int cd_aluno, DateTime? dt_inicial, DateTime dt_final);
        List<ControleFaltasAlunoUI> getAlunosControleFalta(int cd_controle_faltas);

        // Taxa Bancaria
        void aplicarTaxaBancaria(Titulo objTitulo, int nm_parcelas_mensalidade, ref DateTime dataVencimentoTituloCartao, List<Contrato.TituloTaxaParcela> titulosTaxaParcela);
        //AulasReposicao
        IEnumerable<AulaReposicaoUI> searchAulaReposicao(SearchParameters parametros, DateTime? dataIni, DateTime? dataFim,
            TimeSpan? hrInicial, TimeSpan? hrFinal, int? cd_turma, int? cd_aluno,
            int? cd_responsavel, int? cd_sala, int cdEscola);
        AulaReposicaoUI addAulaReposicao(AulaReposicaoUI aulaReposicaoUi);

        AulaReposicaoUI returnAulaReposicaoUsuarioAtendente(int cd_aula_reposicao, int cd_pessoa_escola);
        IEnumerable<AlunoAulaReposicaoUI> searchAlunoAulaReposicao(int cd_aula_reposicao, int cdEscola);
        AulaReposicaoUI editAulaReposicao(AulaReposicaoUI aulaReposicao);
        bool deleteAllAulaReposicao(List<AulaReposicao> aulaReposicao, int cd_escola);
        AulaReposicao findByIdAulaReposicaoViewFull(int cdAulaReposicao);
        AulaReposicao findByIdAulaReposicaoFull(int cdAulaReposicao);
        List<sp_RptAulaReposicao_Result> getReportAulaReposicao(Nullable<int> cd_escola, Nullable<System.DateTime> dta_ini, Nullable<System.DateTime> dta_fim, Nullable<int> cd_turma, Nullable<int> cd_funcionario, Nullable<int> cd_aluno, Nullable<byte> id_participacao);
        List<sp_RptAulaReposicaoAluno_Result> getReportAulaReposicaoAluno(Nullable<int> cd_aula_reposicao, Nullable<int> cd_aluno, Nullable<byte> id_participou);
        List<TimeSpan?> getHorariosDisponiveisAulaRep(DateTime data, int turma, int professor, int? cdAulaReposicao, List<AlunoAulaReposicaoUI> alunos);
        int? verificaHorarioAulaRep(TimeSpan horaIni, TimeSpan horaFim, DateTime data, int? cd_aula_reposicao, int cd_turma, int cd_professor, int cd_empresa, List<AlunoAulaReposicaoUI> alunos);
        bool verificaSalaOnline(int? cdSala, int cdEscola);
        int getNroPessoasAtividade(int CdAtividadeExtra);

        //Mensagem Avaliacao 
        IEnumerable<MensagemAvaliacaoSearchUI> getMensagemAvaliacaoSearch(SearchParameters parametros, string desc, bool inicio, bool? status, int? produto, int? curso);
        List<MensagemAvaliacaoSearchUI> addMensagemAvaliacao(MensagemAvaliacaoSearchUI mensagemAvaliacao);

        IEnumerable<Curso> findCurso(CursoDataAccess.TipoConsultaCursoEnum hasDependente, int? cd_curso, int? cd_produto, int? cd_escola);
        MensagemAvaliacaoSearchUI editMensagemAvaliacao(MensagemAvaliacao entity);
        bool deleteAllMensagemAvaliacao(List<MensagemAvaliacao> listaMensagemAvaliacao);
        IEnumerable<MensagemAvaliacaoAlunoUI> findMsgAlunobyTipo(int cdTipoAvaliacao, int cdAluno, int cdProduto, int cdCurso);
        List<MensagemAvaliacaoAlunoUI> addMensagemAvaliacaoAluno(MensagemAvaliacaoAlunoUI mensagemAvaliacao);
        MensagemAvaliacaoAlunoUI editMensagemAvaliacaoAluno(MensagemAvaliacaoAluno entity);
        bool deleteAllMensagemAvaliacaoAluno(List<MensagemAvaliacaoAluno> listaMensagemAvaliacao);
        AlunoEvento getEventosRtpDiarioAula(int cd_escola, int cd_aluno, int cd_professor, DateTime dataAula);

        Aluno getAlunoIsTurmaInDate(int cd_turma, int cd_aluno, int cd_pessoa_escola, DateTime dtAula);

        //PerdaMaterial

        IEnumerable<PerdaMaterialUI> getPerdaMaterialSearch(SearchParameters parametros, int? cd_aluno, int? nm_contrato, int? cd_movimento, int? cd_item, DateTime? dtInicio, DateTime? dtTermino, int status, int cd_escola);
        PerdaMaterialUI addPerdaMaterial(PerdaMaterial perdaMaterialUi);
        PerdaMaterialUI editPerdaMaterial(PerdaMaterial perdaMaterialUi);
        bool deletePerdaMaterial(List<PerdaMaterial> perdaMaterialList);
        int processarPerdaMaterial(PerdaMaterial perdaMaterial, int cd_usuario, int fuso );
        ProfessorCargaHorariaMaximaResultUI getExisteCargaHorariaProximaMaxima(int cdUsuario, int cdEscola);
    }
}
