using System;
using System.Collections.Generic;
using Componentes.GenericBusiness.Comum;
using Componentes.Utils;
using FundacaoFisk.SGF.Services.InstituicaoEnsino.Model;
using FundacaoFisk.SGF.Web.Services.Usuario.Model;
using FundacaoFisk.SGF.GenericModel;

namespace FundacaoFisk.SGF.Web.Services.InstituicaoEnsino.Comum.IBusiness
{
    using FundacaoFisk.SGF.Web.Services.Secretaria.Model;
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using FundacaoFisk.SGF.Web.Services.Pessoa.Model;
    using FundacaoFisk.SGF.Web.Services.Coordenacao.Model;
    using Componentes.GenericBusiness;
    using System.Data;

    public interface IEscolaBusiness : IGenericBusiness
    {
        //void configuraUsuario(int cdUsuario);

        //Escola
		void verificaHorarioFuncionamentoEscola(TimeSpan hr_inicial, TimeSpan hr_final, TimeSpan hr_servidor);
        IEnumerable<EscolaUI> getDescEscola(SearchParameters parametros, string desc, bool inicio, bool? status, string cnpj, string fantasia, int cdUsuario);
        EscolaUI getEscolaForEdit(int cd_pessoa_empresa);
        EscolaUI addEscola(EscolaUI entity, List<RelacionamentoSGF> relacionamentos, int cdEscola, string fullPath);
        EscolaUI editEscola(EscolaUI entity, List<RelacionamentoSGF> relacionamentos, int cdEscola, bool isMasterGeral, string fullPath);
        Boolean deleteEscola(Escola entity);
        IEnumerable<EscolaUI> findEscolasSecionadas(int cdItem, int cd_usuario);
        IEnumerable<PessoaSearchUI> getSearchEscolas(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio);
        IEnumerable<PessoaSearchUI> getEscolaNotWithItem(SearchParameters parametros, string desc, string cnpj, string fantasia, int cd_item, bool inicio);
        IEnumerable<PessoaSearchUI> getEscolaNotWithKit(SearchParameters parametros, string desc, List<int> empresas, string cnpj, string fantasia, int cd_item, bool inicio);
        IEnumerable<PessoaSearchUI> getEscolaHasItem(int cd_item);
        IEnumerable<TurmaEscolaSearchUI> getTurmasEscolatWithTurma(int cd_turma);
        IEnumerable<AtividadeEscolaAtividadeSearchUI> getAtividadeEscolatWithAtividade(int cd_atividade_extra);
        IEnumerable<PessoaSearchUI> getEscolaHasTpDesc(int cdTpDesc);
        bool verificaHorarioOcupado(int cd_pessoa_escola, TimeSpan hr_ini, TimeSpan hr_fim);
        int getCodigoFranquia(int cd_escola, int id_aplicacao);
        string verifcaEstadoEscAluno(int cd_escola, int cd_pessoa, int tipoMovto);
        IEnumerable<PessoaSearchUI> getSearchEscolasFKFollowUp(SearchParameters parametros, string desc, string cnpj, string fantasia, bool inicio, List<int> cdsEmpresa, bool masterGeral, int? cd_estado, int? cd_cidade);
        bool getEmpresaPropria(int cd_escola);

        //Parametro
        Parametro insertParametro(EscolaUI entity, int cdEscola);
        Parametro editParametro(EscolaUI entity, Parametro parametroExists, bool isMasterGeral);
        Parametro getParametrosByEscola(int cdEscola);
        Parametro getParametrosMatricula(int cdEscola);
        OpcoesPagamentoUI getSugestaoDiaOpcoesPgto(int cd_escola, DateTime data_matricula, int? cd_curso, int? cd_duracao, int? cd_regime);
        Parametro getParametrosBaixa(int cd_pessoa_escola);
        int? getLocalMovto(int cd_escola);
        Parametro getParametrosMovimento(int cd_escola);
        Parametro getParametros(int cdEscola);
        int getParametroNiveisPlanoContas(int cd_escola);
        bool getIdBloquearVendasSemEstoque(int cd_empresa);
        Parametro insertParametro(Parametro parametro);
        bool getIdBloquearliqTituloAnteriorAberto(int cd_empresa);
        byte? getParametroNiviesPlanoConta(int cdEscola);
        int getParametroMovimentoRetroativo(int cd_escola);
        DateTime getParametrosPrevDevolucao(int cd_escola, DateTime dataEmprestimo);
        Parametro getParametrosPlanoTxMatricula(int cdEscola);
        bool verificarGeracaoNFSBaixa(int cd_baixa, int cd_escola);
        TipoNotaFiscal postTpNF(TipoNotaFiscal tipo, int cd_escola);
        TipoNotaFiscal putTpNF(TipoNotaFiscal tipo, int cdEscola);
        DadosNF postDadosNF(DadosNF dado, int cd_escola);
        DadosNF putDadosNF(DadosNF dado, int cdEscola);
        byte getParametroRegimeTrib(int cd_escola);
        int getParametroNmFaltasAluno(int cd_escola);
        bool getImprimir3BoletosPagina(int cd_escola);
        byte? getTipoNumeroContrato(int cd_empresa);
        bool getParametroHabilitacaoProfessor(int cd_escola);
        void realizaMovimento(Movimento movimento);
        Movimento gerarTitulosMovimentoPostNf(int cdEscola, int cd_movimento, int cd_tipo_movimento);
        //Sala
        List<Sala> verficarSalasDisponiveisPorEscola(int cd_escola);
        
        //Matricula
        Contrato PostMatricula(Contrato contrato, string pathContratosEscola, int cdUsuario, int fusoHorario);
        List<Titulo> gerarTitulosGrid(Contrato titulos);
        List<Titulo> gerarTitulosAditamento(Contrato contrato);
        Contrato postAlterarMatricula(Contrato contrato, bool castMatricula, string pathContratosEscola, int cdUsuario, int fusoHorario);
        DocumentoDigitalizadoEditUI postAtualizarDocumentoDigitalizado(DocumentoDigitalizadoEditUI contrato, string pathContratosEscola);
        PacoteCertificadoUI postAtualizarPacoteCertificado(PacoteCertificadoUI contrato);
        void simularBaixaContrato(Contrato contrato, ref BaixaTitulo baixa, int cd_escola);
        List<BaixaTitulo> simularBaixaTituloLeitura(List<Titulo> titulos, DateTime data_baixa, int cd_escola, bool contaSegura);
        List<TituloCnab> simularBaixaTituloCnab(List<TituloCnab> titulos, DateTime data_baixa, int cd_escola, bool contaSegura);
        bool existeAdtAdicionarParcelaBaixado(List<Titulo> titulos, Titulo tituloViewAdt);
        List<Titulo> alterarLocalMovtoTitulos(List<Titulo> titulosAlterarLocalMovto, int nm_parcelas_mensalidade, int cd_politica_comercial, int cd_pessoa_empresa);

        //Desistência
        DesistenciaUI baixarTitulosInserirDesistencia(DesistenciaUI desistenciaUI, List<Titulo> list, DateTime data_baixa, int cd_escola, int cd_tipo_liquidacao, int cd_local_movto, bool contaSeg);
        DesistenciaUI baixarTitulosEditarDesistencia(DesistenciaUI desistenciaUI, List<Titulo> list, DateTime data_baixa, int cd_escola, int cd_liquidacao, int cd_local_movto, bool contaSeg);
        bool deletarDesistencia(List<DesistenciaUI> desistencias, int cd_escola, int cd_usuario);

        //Histórico do aluno:
        List<Titulo> getTituloByPessoa(SearchParameters parametros, int cd_pessoa, int cd_escola, TituloDataAccess.TipoConsultaTituloEnum tipo, bool contaSeg);

        //Biblioteca
        Emprestimo getEmprestimo(int cd_biblioteca, int cd_escola);
        Emprestimo addEmprestimo(Emprestimo emprestimo, int cd_escola);
        Emprestimo postEditEmprestimo(Emprestimo emprestimo, int cd_escola);
        bool deleteEmprestimos(List<Emprestimo> emprestimos, int cd_escola);
        Emprestimo postRenovarEmprestimo(Emprestimo emprestimo, int cd_escola);

        //Movimento
        List<Titulo> gerarTitulosMovimento(Titulo tituloDefault, Movimento movimento);
        MovimentoUI addMovimento(Movimento movimento);
        MovimentoUI editMovimento(Movimento movimento);
        bool deleteMovimentos(int[] cdMovimentos, int cd_empresa);
        Movimento getMontaNFMaterial(int cd_contrato, int cd_escola);
        Movimento getMontaNFBiblioteca(int cd_biblioteca, int cd_escola);
        Movimento getMontaNFBaixaFinanceira(int cd_baixa_titulo, int cd_escola);
        List<Movimento> getGerarNFFaturamento(List<Titulo> titulos, int cd_escola, bool empresaPropria);
        void cancelarNFServico(Movimento movimento, bool id_empresa_propria);
        bool processarNF(int cdEscola, int cd_movimento, bool empresaPropria);
        MovimentoUI processarNFMovimento(int cdEscola, int cd_movimento, bool empresaPropria, Movimento movimento);
        bool postAlterarLocalMovtoTitulosNFFechada(int cd_escola, List<Titulo> titulos);

        //Financeiro:
        IEnumerable<RptReceberPagar> receberPagarStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNatureza, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma);
        List<DescontoTituloCarne> getTituloCarnePorContratoSubReport(int cdTitulo, int cdEscola, bool contaSegura);

        //contrato
        ContratoUI getContratoTurma(int cd_pessoa_escola, int cd_curso, int cd_duracao, int cd_produto, int cd_regime, DateTime dta_matricula);

        //Transação Finanaceira
        TransacaoFinanceira postIncluirTransacao(TransacaoFinanceira transacao);
        TransacaoFinanceira editTransacao(TransacaoFinanceira transacao);

        //Usúario
        UsuarioUISearch PostInsertUsuario(UsuarioWebSGF usuario, PessoaFisicaSGF pessoaFisica, int cdEmp,List<Horario> horarios);
        UsuarioUISearch PostEditUsuario(UsuarioWebSGF usuario, int cdEmp, List<Horario> horarios);
        bool DeleteUsuario(List<UsuarioWebSGF> listUsuario, int cd_empresa);
        IEnumerable<UsuarioUISearch> getUsuarioSearchFKFollowUp(SearchParameters parametros, string descricao, string nome, bool inicio, int cd_empresa,
                                                               int cd_usuario_logado, int tipoPesq, string usuariologado, Int32[] codEscolas);

        //Diário de Aula
        ProgramacoesTurmaSemDiarioAula getProgramacoesTurmasSemDiarioAula(int cd_turma, int cd_escola);
        ProgramacoesTurmaSemDiarioAula verificarAlunosPendenciaMaterialDidaticoCurso(int cd_turma, int cd_escola, DateTime dt_programacao_turma);


        //Follow-Up
        IEnumerable<PessoaSearchUI> getEscolasFollowUp(int cd_follow_up, int cd_escola);

        //Mail Marketing
        String getRodapeSysApp();
        string getVersoCartaoPostal();
        SysApp getConfigEmailMarketingSysApp();
        SysApp putConfigEmailMarketingSysApp(SysApp sysApp);

        //Reajuste Anual
        ReajusteAnual abrirFecharReajusteAnual(ReajusteAnual reajuste, int cd_usuario);

        //Mudança Interna
        MudancasInternas postMudancaTurma(MudancasInternas mudanca, bool contaSeg);
        bool postMontaNFMaterial(Contrato contrato);
        List<Movimento> getMovimentosbyOrigem(int cd_contrato, int cdEscola);
        void enviaPromocaoAlunoMatricula(int cdAluno, int cdContrato, int id_tipo_matricula);
        DataTable getLoginEscola(DateTime dt_analise, bool id_login, byte id_matricula);
        int getQtdDiarioTurma(int cd_turma, DateTime dt_aula);
    }
}
