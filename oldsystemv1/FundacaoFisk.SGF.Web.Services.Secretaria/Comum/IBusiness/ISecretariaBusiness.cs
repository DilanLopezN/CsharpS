using System;
using System.Collections.Generic;
using System.Data;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Secretaria.Comum
{
    using System.Data.Entity;
    using Componentes.GenericBusiness.Comum;
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using FundacaoFisk.SGF.Web.Services.Secretaria.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
    public interface ISecretariaBusiness : IGenericBusiness
    {
        void sincronizarContextos(DbContext dbContext);

        //void configuraUsuario(int cdUsuario);
        IEscolaridadeDataAccess getDataAccessEscolaridade();
        //Escolaridade
        IEnumerable<Escolaridade> GetEscolaridadeSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        Escolaridade GetEscolaridadeById(int id);
        Escolaridade PostEscolaridade(Escolaridade escolaridade);
        Escolaridade PutEscolaridade(Escolaridade escolaridade);
        bool DeleteEscolaridade(List<Escolaridade> escolaridade);
        IEnumerable<Escolaridade> getEscolaridade(bool? status);
      
        //Mídia
        IEnumerable<Midia> GetMidiaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        Midia GetMidiaById(int id);
        Midia PostMidia(Midia midia);
        Midia PutMidia(Midia midia);
        bool DeleteMidia(List<Midia> midia);
        IEnumerable<Midia> getMidia(bool? status, MidiaDataAccess.TipoConsultaMidiaEnum tipo, int? cd_empresa);
      
       //Tipo Contato
        IEnumerable<TipoContato> GetTipoContatoSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        TipoContato GetTipoContatoById(int id);
        TipoContato PostTipoContato(TipoContato tipocontato);
        TipoContato PutTipoContato(TipoContato tipocontato);
        bool DeleteTipoContato(List<TipoContato> tipocontato);

        //Motivo Matricula
        IEnumerable<MotivoMatricula> GetMotivoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        MotivoMatricula GetMotivoMatriculaById(int id);
        MotivoMatricula PostMotivoMatricula(MotivoMatricula motivomatricula);
        MotivoMatricula PutMotivoMatricula(MotivoMatricula motivomatricula);
        bool DeleteMotivoMatricula(List<MotivoMatricula> motivomatricula);

        //Motivo Não Matricula
        IEnumerable<MotivoNaoMatricula> GetMotivoNaoMatriculaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        MotivoNaoMatricula GetMotivoNaoMatriculaById(int id);
        MotivoNaoMatricula PostMotivoNaoMatricula(MotivoNaoMatricula motivonaomatricula);
        MotivoNaoMatricula PutMotivoNaoMatricula(MotivoNaoMatricula motivonaomatricula);
        bool DeleteMotivoNaoMatricula(List<MotivoNaoMatricula> motivonaomatricula);
        IEnumerable<MotivoNaoMatricula> getMotivoNaoMatriculaProspect(int cdProspect);
        IEnumerable<MotivoNaoMatricula> getMotivosNaoMatricula();

        //Motivo Bolsa
        IEnumerable<MotivoBolsa> GetMotivoBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        MotivoBolsa GetMotivoBolsaById(int id);
        MotivoBolsa PostMotivoBolsa(MotivoBolsa motivobolsa);
        MotivoBolsa PutMotivoBolsa(MotivoBolsa motivobolsa);
        bool DeleteMotivoBolsa(List<MotivoBolsa> motivobolsa);
        IEnumerable<MotivoBolsa> getMotivoBolsa(bool? status, int? cd_motivo_bolsa);

        //Motivo Cancelamento Bolsa
        IEnumerable<MotivoCancelamentoBolsa> GetMotivoCancelBolsaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        MotivoCancelamentoBolsa GetMotivoCancelBolsaById(int id);
        MotivoCancelamentoBolsa PostMotivoCancelBolsa(MotivoCancelamentoBolsa motivocancelbolsa);
        MotivoCancelamentoBolsa PutMotivoCancelBolsa(MotivoCancelamentoBolsa motivocancelbolsa);
        bool DeleteMotivoCancelBolsa(List<MotivoCancelamentoBolsa> motivocancelbolsa);
        IEnumerable<MotivoCancelamentoBolsa> getMotivoCancelamentoBolsa(bool? status, int? cd_motivo_cancelamento_bolsa);
        
        //Ação Follow-up
        IEnumerable<AcaoFollowUp> GetAcaoFollowUpSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        AcaoFollowUp GetAcaoFollowUpById(int id);
        AcaoFollowUp PostAcaoFollowUp(AcaoFollowUp acaoFollowUp);
        AcaoFollowUp PutAcaoFollowUp(AcaoFollowUp acaoFollowUp);
        bool DeleteAcaoFollowUp(List<AcaoFollowUp> acoesFollowUp);
        IEnumerable<AcaoFollowUp> getAcaoFollowUp(AcaoFollowupDataAccess.TipoPesquisaAcaoEnum tipo, int cd_acao_follow_up);
        
        //Pessoa
        PessoaFisicaSGF postInsertPessoaFisica(PessoaFisicaUI pessoaFisicaUI,List<RelacionamentoSGF> relacionamentos, int cdEscola);
        PessoaJuridicaSGF postInsertPessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI,List<RelacionamentoSGF> relacionamentos, int cdEscola);
        PessoaFisicaSGF postUpdatePessoaFisica(PessoaFisicaUI pessoaFisicaUI,List<RelacionamentoSGF> relacionamentos, int cdEscola);
        PessoaJuridicaSGF postUpdatePessoaJuridica(PessoaJuridicaUI pessoaJuridicaUI, List<RelacionamentoSGF> relacionamentos, int cdEscola);

        //Horario
        IEnumerable<Horario> getHorarioByEscolaForRegistro(int cdEscola, int cdRegistro, Horario.Origem origem);
        IEnumerable<Horario> getHorarioByEscolaForRegistroUncommited(int cdEscola, int cdRegistro, Horario.Origem origem);
        bool deleteHorario(Horario horario);
        Horario addHorario(Horario horario);
        Horario editHorarioContext(Horario horarioContext,Horario horarioView);
        IEnumerable<Horario> getHorarioOcupadosForTurma(int cdEscola, int cdRegistro, int[] cdProfessores, int cd_turma,
            int cd_duracao, int cd_curso, DateTime dt_inicio, DateTime? dt_final, HorarioDataAccess.TipoConsultaHorario tipoCons);
        IEnumerable<Horario> getHorarioOcupadosForSala(Turma turma, int cd_escola, HorarioDataAccess.TipoConsultaHorario tipoCons);
        bool getHorarioByHorario(int cdEscola, int cdRegistro, Horario.Origem origem, TimeSpan hr_servidor, int diaSemanaAtual);
        void verificaHorarioUsuario(int cdEscola, int cdRegistro, TimeSpan hr_servidor, int diaSemanaAtual);
        int countHorariosUsuario(int cd_empresa, int cd_usuario);
        String retornaDescricaoHorarioOcupado(int cd_empresa, TimeSpan hr_ini, TimeSpan hr_fim);

        //Prospect
        ProspectSearchUI insertProspect(ProspectSearchUI prospect);
        ProspectSearchUI editProspect(ProspectSearchUI prospect, int cd_usuario_atendente);
        IEnumerable<ProspectSearchUI> GetProspectSearch(SearchParameters parametros, string nome, bool inicio, string email, int cdEscola, DateTime? dataIni, DateTime? dataFim, bool? ativo, bool aluno, int testeClassificacaoMatriculaOnline);
        bool deleteAllProspect(List<Prospect> prospects, int cd_escola);
        ProspectSearchUI getExistsProspectEmail(string email, int cdEscola, int cdProspect);
        Prospect getProspectForEdit(int cdProspect, int cdEscola, string email);
        List<int> getProspectDia(int cd_prospect);
        bool deleteProspect(Prospect prospect);
        Prospect findProspectById(int cdProspect);
        Prospect getProspectAllData(int cdProspect, int cdEscola);
        List<ProspectSiteUI> getProspectSite(int cd_prospect, int tipo);
        PessoaFisicaSGF verificarPessoaFisicaEmail(string email);
        Prospect getProspectForAluno(int cdProspect, int cdEscola);
        void setProspectsConsultado(int cd_escola);
        bool existeProspectNaoConsultado(int cd_escola);
        IEnumerable<ProspectSearchUI> getProspectFKSearch(SearchParameters parametros, int cdEscola, string nome, bool inicio, string email, string celular, ProspectDataAccess.TipoConsultaEnum tipo);
        JsonTeste postJsonTeste(JsonTeste jsonTeste);
        void editJsonTeste(int idJsonTeste, byte statusProcedureErro, string msgErro);
        ProspectIntegracaoRetornoUI postProspectIntegracao(Nullable<int> nm_integracao, Nullable<byte> id_tipo, Nullable<int> id_teste, string no_pessoa, string email, string fone, string cep, string day_week, string periodo, Nullable<System.DateTime> dt_cadastro, string sexo, Nullable<double> hit, string phase, string courseId);
        Recibo getReciboByProspect(int cd_prospect, int cd_empresa);
        IEnumerable<ReportProspect> getProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos, int cd_faixa_etaria);
        IEnumerable<ReportProspect> getProspectAtendidoMatricula(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos);
        IEnumerable<ReportProspect> getComparativoProspectAtendido(int cd_escola, int cdMotivoNaoMatricula, int cFuncionario, int cdProduto, DateTime? pDtaI, DateTime? pDtaF, int cd_midia, List<int> periodos);
        Prospect getProspectPorEmail(int cdEscola, string email);
        string getNomeAtendente(int cdUsuario, int cdEscola);

        //FollowUp
        IEnumerable<FollowUpSearchUI> getFollowUpSearch(SearchParameters parametros, int cdEscola, byte id_tipo_follow, int cd_usuario_org, int cd_usuario_destino, int cd_prospect,
                                                       int cd_acao, int resolvido, int lido, bool data, bool proximo_contato, DateTime? dt_inicial, DateTime? dt_final,bool id_usuario_adm,
                                                       int cd_usuario_logado, int cd_aluno, bool usuario_login_master);
        IEnumerable<FollowUp> getFollowUpByAluno(int cdAluno, int cd_escola);
        IEnumerable<FollowUp> getFollowUpProspect(int cdProspect, int cd_escola);
        bool deleteFollowUp(FollowUp followUp);
        FollowUpSearchUI addFollowUp(FollowUp followUp);
        FollowUpSearchUI editFollowUp(FollowUp followUp,int cd_usuario, bool isMaster);
        bool deleteFollowUps(List<int> codigosFollowUps, int cd_usuario_origem);
        IEnumerable<MotivoNaoMatricula> getProspectMotivoNaoMatricula(int cdProspect, int cd_escola);
        ProspectMotivoNaoMatricula getProspectMotivoNaoMatriculaEsc(int cdProspect, int cd_escola, int cd_motivo);
        FollowUp getFollowEditView(int cd_follow_up, int cd_escola, int id_tipo_follow);
        IEnumerable<FollowUpEscola> getFollowUpEscola(int cd_follow_up);
        bool marcarFollowUpComoLido(FollowUp followUp, int cd_usuario_login);
        bool existeFollowNaoResolvido(int cd_usuario_logado, int cd_escola, bool usuario_login_master);
        bool enviarEmailProspectAndUpdIdEmail(ProspectSearchUI cdProspect, int cdEscola, ICollection<FollowUp> listaFollowUp);

        //Nome Contrato
        IEnumerable<NomeContrato> getSearchNoContrato(SearchParameters parametros, string desc, string layout, bool inicio, bool? status, int cdEscola,int cdUsuario);
        NomeContrato addNomeContrato(NomeContrato nomeCont,string pathContratos,int cdUsuario);
        NomeContrato editNomeContrato(NomeContrato nomeCont, string pathContratosEscola, int cdUsuario);
        bool deleteNomesContratos(int[] cdNomesContratos,string pathContratosEscola,int cdEscola,int cdUsuario);
        NomeContrato getNomeContratoById(int? cdEscola, int cdNomeContrato);
        string getNomeContratoDigitalizadoByCdContrato(int cdEscola, int cdContrato, string nomeArquivo);
        string getNomeContratoDigitalizadoByEscolaAndCdContrato(int cdEscola, int cdContrato);
        IEnumerable<NomeContrato> getNomesContrato(NomeContratoDataAccess.TipoConsultaNomeContratoEnum hasDependente, int? cd_nome_contrato, int? cd_escola);
        NomeContrato getNomeContratoAditamentoMatricula(int cd_contrato, int cd_escola);
        IEnumerable<NomeContrato> getNomeContratoMat(int? cdEscola);

        //Histórico  Aluno
        IEnumerable<HistoricoAluno> returnHistoricoSitacaoAlunoTurma(int cd_turma, int cd_pessoa_escola);
        void addHistoricoAluno(HistoricoAluno historicoAluno);
        int retunMaxSequenciaHistoricoAluno(int cd_produto, int cd_pessoa_escola, int cd_aluno);
		HistoricoAluno postHistoricoAluno(HistoricoAluno historico);
        HistoricoAluno editHistoricoAluno(HistoricoAluno historico);
        int saveHistoricoAluno();
        bool deleteHistoricoAluno(HistoricoAluno historico);
        int getUltimoHistoricoAluno(int cd_pessoa_escola, int cd_aluno, int cdProduto);
        HistoricoAluno getUltimoHistoricoAlunoPorCodTurma(int cd_aluno, int cd_escola, int cd_turma);
        HistoricoAluno GetHistoricoAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        IEnumerable<HistoricoAluno> GetHistoricosAlunoById(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        HistoricoAluno GetHistoricoAlunoPrimeiraAula(int cdEscola, int cdAluno, int cdTurma, int cdContrato, DateTime dataDiario);
        HistoricoAluno GetHistoricoAlunoMovido(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        List<Produto> getHistoricoTurmas(int cd_aluno, int cd_escola);
        IEnumerable<FollowUp> getFollowAluno(SearchParameters parametros, int cd_aluno, int cd_escola);
        HistoricoAluno getHistoricoAlunoByMatricula(int cdEscola, int cdAluno, int cdTurma, int cdContrato);
        HistoricoAluno GetHistoricoAlunoPorDesistencia(int cdDesistencia);
        HistoricoAluno getSituacaoAlunoCancelEncerramento(int cd_aluno, int cd_turma, DateTime dt_historico);
        DateTime? buscarDataHistoricoDesistenciaAlteriorCancelamento(int cd_aluno, int cd_turma, DateTime dt_historico, byte nm_sequencia);
        DataTable getAlunos(int cd_aluno, int Tipo, string produtos, string statustitulo);
        List<sp_RptHistoricoAlunoM_Result> getRtpHistoricoAlunoM(int cdAluno);
        List<st_RptFaixaEtaria_Result> getRtpFaixaEtaria(int cd_escola, int tipo, int idade, int idade_max, int cd_turma);
        DataTable getRtpFaixaEtariaDT(int cd_escola, int tipo, int idade, int idade_max, int cd_turma);
        List<ProdutoHistoricoSeachUI> getProdutosComHistorico(int cd_escola);

        //Desistência
        IEnumerable<DesistenciaUI> getDesistenciaSearchUI(SearchParameters parametros, int? cd_turma, int? cd_aluno, int? cd_pessoa_escola, int? cd_motivo_desistencia, int cd_tipo,
            DateTime? dta_ini, DateTime? dta_fim, int cd_produto, int cd_professor, List<int> cdsCurso);
        DesistenciaUI addDesistencia(DesistenciaUI desistencia, int cd_pessoa_escola, int qtd_diario, int qtd_faltas, bool chamaApiCyber);
        DesistenciaUI editDesistencia(DesistenciaUI desistencia, int cd_pessoa_escola, int qtd_diario, bool chamaApiCyber);
        DesistenciaUI getDesistenciaAlunoTurma(int cd_aluno_turma, int cd_pessoa_escola);
        bool getExisteDesistenciaPorAlunoTurma(int cd_aluno, int cd_turma, int cd_pessoa_escola);
        void validarInsersaoDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        DateTime? getMaiorDataDesistencia(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        int retornaQuantidadeDesistencia(int cd_turma, int cd_aluno, int cd_pessoa_escola, int cd_aluno_turma);
        Desistencia findByIdDesistencia(int cd_desistencia);
        bool deleteDesistencia(Desistencia desistencia);
        Desistencia retornaDesistenciaMax(DesistenciaUI desistenciaUI, int cd_pessoa_escola);
        bool verificarMasterGeral(string login);
        bool verificarMasterGeral(int cdUsuario);
        void alterarAlunoTurma(DesistenciaUI desistenciaUI, int cd_pessoa_escola, int qtd_diario, int qtd_faltas);

        //Aluno
        bool deletarAlunos(List<Aluno> alunos, int cd_escola);
        AlunoSearchUI addAluno(AlunoUI alunoUI);
        AlunoSearchUI editAluno(AlunoUI alunoUI);

        //Aluno Turma
        IEnumerable<AlunoTurma> findAlunosTurmaPorTurmaEscola(int cd_turma, int cd_escola);
        List<AlunoTurma> findAlunosTurmaForEncerramento(int cd_turma, int cd_escola);
        void deletarAlunoTurma(AlunoTurma alunoTurma);
        AlunoTurma addAlunoTurma(AlunoTurma alunoTurma);
        IEnumerable<AlunoTurma> findAlunosTurma(int cd_turma, int cd_escola, int[] cdAlunos);
        bool deleteAlunoAguardandoTurma(int cdProduto, int cdEscola, int cdContrato, int cdAluno, int cd_turma);
        bool existsAlunoTurmaByContratoEscola(int cd_contrato, int cd_pessoa_escola);
        List<AlunoTurma> existsAlunosTurmaInTurmaDestino(List<int> cdsAlunosTurma, int cdTurmaDestino);
        IEnumerable<AlunoTurma> findAlunosTurmaHist(int cd_turma, int cd_escola, int[] cdAlunos);
        AlunoTurma findAlunoTurma(int cd_aluno, int cd_turma, int cd_escola);
        AlunoTurma findAlunoTurmaByCdCursoContrato(int cd_curso_contrato, int cd_escola);
        AlunoTurma findAlunoTurmaById(int id);

        //AnoEscolar
        IEnumerable<AnoEscolar> GetAnoEscolarSearch(SearchParameters parametros, int? cdEscolaridade, string descricao, bool inicio, bool? ativo);
        AnoEscolar GetAnoEscolarById(int id);
        AnoEscolar PostAnoEscolar(AnoEscolar anoEscolar);
        AnoEscolar PutAnoEscolar(AnoEscolar anoEscolar);
        bool DeleteAnoEscolar(List<AnoEscolar> anoEscolar);
        IEnumerable<Escolaridade> getEscolaridadePossuiAnoEscolar();
        IEnumerable<AnoEscolar> getAnoEscolaresAtivos(int? cdAnoEscolar);

        // GeraNotasXML
        IEnumerable<ImportacaoXML> getListaXmlGerados(SearchParameters parametros, XmlSearchUI notatualizaUI);
        int abrirGerarMXL(int cd_usuario);
        int postGerarXmlProc(int cd_usuario);
        IEnumerable<ImportacaoXML> buscarGerarXML(int cd_usuario);
        List<int> setAtualizaXML(List<int> cdImportXML);

        //usuario
        IEnumerable<Escola> findAllEmpresaByCdUsuario(int codUsuario);

        // EnvioSMS
        //IEnumerable<SmsParametroUI> verificaParametosEmpresaSms(int cdEscola);
        //IEnumerable<SmsParametrosEscola> getListaEscolaComParametro(int cdEscola);
        //SmsParametrosEscola postParamSmsEscola(SmsParametrosEscola smsParametroUi);
        //bool deletarParamEscolarSms(int cdEscola);
        //SmsParametrosEscola editParamSmsEscola(SmsParametrosEscola smsParametrosEscola, int cdEscola);
        //// Compor Mensagens Padrão SMS
        //SmSComporMensagemPadrao postNovaMensagemPadrao(SmSComporMensagemPadrao smsComporMensagem);
        //IEnumerable<SmSComporMensagemPadrao> getListaMensagensPadraobyEscola(int cdEscola);
        //bool deletaMensagemPadrao(int cd_escola, int motivo);
        //SmSComporMensagemPadrao editMensagemPadraoSms(SmSComporMensagemPadrao smsComporMensagem);
        ////IEnumerable<PessoaSGF> getListaAniversariosPeriodo(int cdEscola);


        //AlunoRestricao
        IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiro(bool? status);
        IEnumerable<AlunoRestricao> getAlunoRestricaoByCdAluno(int cd_aluno);

        //FollowUp
        List<FollowUpRptUI> GetRtpFollowUp(byte id_tipo_follow, int cd_usuario_org, string no_usuario_org, int resolvido, int lido, DateTime? dtaIni, DateTime? dtaFinal, int cd_escola);
        //Motivo Transferencia
        IEnumerable<MotivoTransferenciaAluno> GetMotivoTransferenciaSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo);
        MotivoTransferenciaAluno GetMotivoTransferenciaById(int id);
        MotivoTransferenciaAluno PostMotivoTransferencia(MotivoTransferenciaAluno motivotransferencia);
        MotivoTransferenciaAluno PutMotivoTransferencia(MotivoTransferenciaAluno motivotransferencia);
        bool DeleteMotivoTransferencia(List<MotivoTransferenciaAluno> motivotransferencia);
        IEnumerable<MotivoTransferenciaAluno> getMotivosTransferencia();

        //TransferenciaAluno
        IEnumerable<TransferenciaAlunoUI> getEnviarTransferenciaAlunoSearch(SearchParameters parametros, int cd_escola_logada, int? cd_unidade_destino, int cd_aluno, string nm_raf, string cpf, int status_transferencia, DateTime? dataIni, DateTime? dataFim);
        EnviarTransferenciaComponentesCadParams getComponentesEnviarTransferenciaCad(int cdEscola);
        string getEmailUnidade(int cdEscola);
        string getRafByAluno(int cdAluno);
        TransferenciaAluno postInsertEnviarTransferenciaAluno(TransferenciaAluno transferenciaAlunoView);
        TransferenciaAluno getTransferenciaAlunoByCodForGrid(int cd_transferencia_aluno);
        TransferenciaAluno getTransferenciaAlunoById(int cd_transferencia_aluno);
        TransferenciaAluno getEnviarTransferenciaAlunoForEdit(int cd_transferencia_aluno);
        TransferenciaAluno postEditEnviarTransferenciaAluno(TransferenciaAluno transferenciaAlunoView);
        bool deletarTransferenciaAlunos(List<TransferenciaAluno> transferenciaAluno);
        List<string> sendEmailSolicitaTransferenciaAluno(TransferenciaAluno transferenciaAluno);
        List<string> transferirAluno(TransferenciaAluno transferenciaAluno, int cd_usuario);
        IEnumerable<TransferenciaAlunoUI> getReceberTransferenciaAlunoSearch(SearchParameters parametros, int cdEscola, int? cdUnidadeOrigem, string noAluno, string nmRaf, string cpf, int statusTransferencia, DateTime? dtInicial, DateTime? dtFinal);
        TransferenciaAluno postEditReceberTransferenciaAluno(TransferenciaAluno transferenciaAlunoView);
        List<string> sendEmailAprovarRecusarTransferenciaAluno(TransferenciaAluno transferenciaAluno);
        TransferenciaAluno getArquivoHistorico(TransferenciaAluno transferenciaAluno, string parametrosCript);
        void enviaPromocao(int retCdProspect);
        ProspectGeradoIntegracaoRetornoUI postGetProspectsGeradosSendPromocao();
        void addPessoaPromocao(PessoaPromocao pessoaPromocao);
    }
}
