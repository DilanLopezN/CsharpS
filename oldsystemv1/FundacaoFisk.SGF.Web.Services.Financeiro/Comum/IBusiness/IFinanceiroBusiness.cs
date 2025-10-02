using System;
using System.Collections.Generic;
using Componentes.Utils;

namespace FundacaoFisk.SGF.Web.Services.Financeiro.Comum.IBusiness
{

    using System.Data.Entity;
    using System.Linq;
    using Componentes.GenericBusiness.Comum;
    using FundacaoFisk.SGF.Web.Services.Empresa.Model;
    using FundacaoFisk.SGF.GenericModel;
    using FundacaoFisk.SGF.Web.Services.Financeiro.DataAccess;
    using FundacaoFisk.SGF.Web.Services.Financeiro.Model;
    using Componentes.GenericBusiness;
    using System.Data;

    public interface IFinanceiroBusiness : IGenericBusiness 
    {
        void sincronizarContextos(DbContext dbContext);
       // void configuraUsuario(int cdUsuario);

        //Grupo de Estoque
        IEnumerable<GrupoEstoque> getGrupoEstoqueSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int categoria);
        GrupoEstoque getGrupoEstoqueById(int id);
        GrupoEstoque postGrupoEstoque(GrupoEstoque grupoestoque, bool masterGeral);
        GrupoEstoque putGrupoEstoque(GrupoEstoque grupoestoque, bool masterGeral);
        Boolean deleteGrupoEstoque(GrupoEstoque grupoestoque, bool masterGeral);
        Boolean deleteAllGrupo(List<GrupoEstoque> grupos, bool masterGeral);
        List<GrupoEstoque> findAllGrupoAtivo(int cdGrupo, bool isMasterGeral);
        List<GrupoEstoque> findAllGrupoWithItem(int cd_pessoa_escola, bool isMaster);

        //Movimentação Financeira
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoFinanceiraSearch(SearchParameters parametros, string descricao, bool inicio, bool? status);
        MovimentacaoFinanceira getMovimentacaoFinanceiraById(int id);
        MovimentacaoFinanceira postMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira);
        MovimentacaoFinanceira putMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira);
        Boolean deleteMovimentacaoFinanceira(MovimentacaoFinanceira movimentacaofinanceira);
        Boolean deleteAllMovimentacao(List<MovimentacaoFinanceira> movimentacoes);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoWithContaCorrente(int cd_pessoa_escola);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, bool isCadastrar);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoAtivaWithConta(int cd_pessoa_escola, int cd_movimentacao_financeira);
        IEnumerable<MovimentacaoFinanceira> getMovimentacaoTransferencia(int cd_pessoa_escola, int cd_movimentacao_financeira);

        //Tipo Liquidação
        IEnumerable<TipoLiquidacao> getTipoLiquidacaoSearch(SearchParameters parametros, string descricao, bool inicio, bool? status);
        TipoLiquidacao getTipoLiquidacaoById(int id);
        TipoLiquidacao postTipoLiquidacao(TipoLiquidacao tipoliquidacao);
        TipoLiquidacao putTipoLiquidacao(TipoLiquidacao tipoliquidacao);
        Boolean deleteTipoLiquidacao(TipoLiquidacao tipoliquidacao);
        Boolean deleteAllTipoLiquidacao(List<TipoLiquidacao> tiposLiquidacao);
        IEnumerable<TipoLiquidacao> getTipoLiquidacao(TipoLiquidacaoDataAccess.TipoConsultaTipoLiquidacaoEnum hasDependente, int? cd_tipo_liquidacao);
        IEnumerable<TipoLiquidacao> getTipoLiquidacaoCd(int? cdTipoLiq);


        //Orgao Financeiro
        IEnumerable<OrgaoFinanceiro> getOrgaoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? status);
        OrgaoFinanceiro postOrgaoFinanceiro(OrgaoFinanceiro orgaoFinanceiro);
        OrgaoFinanceiro putOrgaoFinanceiro(OrgaoFinanceiro orgaoFinanceiro);
        bool deleteAllOrgaoFinanceiro(List<OrgaoFinanceiro> orgaosFinanceiros);
        IEnumerable<OrgaoFinanceiro> getAllOrgaoFinanceiro();
        DataTable getRptAlunoRestricao(int? cd_escola, int? cd_orgao, DateTime? dt_inicio, DateTime? dt_final, byte? tipodata);


        //Tipo Financeiro
        IEnumerable<TipoFinanceiro> getTipoFinanceiroSearch(SearchParameters parametros, string descricao, bool inicio, bool? status);
        TipoFinanceiro getTipoFinanceiroById(int id);
        TipoFinanceiro postTipoFinanceiro(TipoFinanceiro tipofinanceiro);
        TipoFinanceiro putTipoFinanceiro(TipoFinanceiro tipofinanceiro);
        Boolean deleteTipoFinanceiro(TipoFinanceiro tipofinanceiro);
        Boolean deleteAllTipoFinanceiro(List<TipoFinanceiro> tiposFinanceiros);
        List<TipoFinanceiro> getTipoFinanceiroAtivo();
        IEnumerable<TipoFinanceiro> getTipoFinanceiroMovimento(int cd_tipo_finan, int cd_empresa, int id_tipo_movto);
        IEnumerable<TipoFinanceiro> getTipoFinanceiro(int cd_tipo_finan, TipoFinanceiroDataAccess.TipoConsultaTipoFinanEnum tipoConsulta);

        //Tipo Desconto
        IEnumerable<TipoDescontoUI> getTipoDescontoSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, bool? incideBaixa, bool? pparc, decimal? percentual, int cdEscola);
        TipoDesconto getTipoDescontoByIdComTipoDescontoEscola(int id);
        TipoDesconto getTipoDescontoComTipoDescontoEscola(int cd_escola, int cd_tipo_desconto);
        TipoDesconto getTipoDescontoByContrato(int cd_desconto_contrato);
        TipoDescontoUI postTipoDesconto(TipoDescontoUI tipoDesconto, int cdEscola, ICollection<Escola> escolasUsuario, bool isMasterGeral);
        TipoDescontoUI putTipoDesconto(TipoDescontoUI tipoDesconto, int cdEscola, ICollection<Escola> escolasUsuario, bool isMasterGeral, ICollection<EmpresaSession> listEmp);
        Boolean deleteTipoDesconto(TipoDesconto tipodesconto);
        bool deleteAllTipoDesconto(List<TipoDescontoUI> tiposDesconto, bool isMasterGeral, int cdEscola, ICollection<EmpresaSession> listEmp);

        //Grupo de Contas
        IEnumerable<GrupoConta> getGrupoContaSearch(SearchParameters parametros, string descricao, bool inicio, int tipoGrupo);
        GrupoConta getGrupoContaById(int id);
        GrupoConta postGrupoConta(GrupoConta grupoConta);
        GrupoConta putGrupoConta(GrupoConta grupoConta);
        Boolean deleteGrupoConta(GrupoConta grupoConta);
        Boolean deleteAllGrupoConta(List<GrupoConta> grupoConta);
        IEnumerable<GrupoConta> getAllGrupoConta();
        IEnumerable<GrupoConta> getListaContas(int cd_grupo_conta, string no_subgrupo_conta, bool inicio, int nivel, int tipoPlanoConta, int cd_pessoa_empresa);
        IEnumerable<GrupoConta> getGrupoContasWithPlanoContas(int cd_pessoa_empresa);
        bool getGrupoContasWhitOutPlanoContas(byte nivel, int cd_pessoa_empresa);
        IEnumerable<GrupoConta> getPlanoContasTreeSearch(int cd_escola, bool busca_somente_ativo, bool conta_segura, string descricao, bool inicio);
        IEnumerable<GrupoConta> getPlanoContasWithMovimento(int cd_pessoa_empresa, int tipoMovimento, string descricao, bool inicio);
        IEnumerable<GrupoConta> getPlanoContasTreeSearchWhitContaCorrente(int cd_escola, string descricao, bool inicio);
        IEnumerable<GrupoConta> getSubgrupoContaSearchFK(string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo, bool contaSegura, int cdEscola);

        //Subgrupo de Contas
        IEnumerable<SubGrupoSort> getSubgrupoContaSearch(SearchParameters parametros, string descricao, bool inicio, int cdGrupo, SubgrupoConta.TipoNivelConsulta tipo);
        SubgrupoConta getSubgrupoContaById(int id);
        SubGrupoSort postSubgrupoConta(SubgrupoConta subgrupoConta);
        SubGrupoSort putSubgrupoConta(SubgrupoConta subgrupoConta);
        Boolean deleteSubgrupoConta(SubgrupoConta subgrupoConta);
        Boolean deleteAllSubgrupoConta(List<SubgrupoConta> subgrupoConta);
        IEnumerable<SubgrupoConta> getAllSubgrupoConta();
        IEnumerable<SubgrupoConta> getSubgruposPorCodGrupoContas(int cdGrupoContas);

        //Item
        IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral, bool estoque,
            bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura);
        IEnumerable<ItemUI> getItemSearchAlunosemAula(SearchParameters parametros, string descricao, bool inicio, bool? status, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral, bool estoque,
            bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura);
        IEnumerable<ItemKitUI> getKitSearch(SearchParameters parametros, string descricao, bool inicio, bool? status, int? tipoItem, int? cdGrupoItem, int? cdEscola, bool isMasterGeral, bool estoque,
            bool biblioteca, bool comEstoque, int categoria, bool todas_escolas, bool contaSegura);
        IEnumerable<Item> getItensByIds(List<Item> itens);
        bool deleteItemByCurso(int cd_curso);
        ItemUI addItemEstoque(ItemUI item, int cdEscola, ICollection<Escola> escolas, bool isMasterGeral);
        bool deleteAllItem(List<Item> itens, bool isMasterGeral, int cdEscola);
        ItemUI editarItemEstoque(ItemUI item, int cdEscola, ICollection<Escola> escolas, bool isMasterGeral, ICollection<EmpresaSession> listEmp);
        ItemUI editarKitEstoque(ItemUI item, int cdEscola, ICollection<Escola> escolas, bool isMasterGeral, ICollection<EmpresaSession> listEmp);
        IEnumerable<ItemUI> getItemCurso(int cdCurso, int? cdEscola, bool isMasterGeral);
        IEnumerable<ItemUI> getItemSearch(SearchParameters parametros, String descricao, Boolean inicio, Boolean? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, int categoria);
        IEnumerable<ItemUI> getItemSearchEstoque(SearchParameters parametros, string descricao, bool inicio, bool? ativo, int tipoItem, int cdGrupoItem, int cdEscola, bool comEstoque, List<int> cdItens, bool isMaster, int ano, int mes);
        IEnumerable<RptItemFechamento> rptItemWithSaldoItem(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo, int ano, int mes, bool isContagem);
        IEnumerable<RptItemFechamento> rptItemSaldoBiblioteca(int cd_pessoa_escola, int cd_item, int cd_grupo, byte cd_tipo);
        IEnumerable<ItemUI> listItensMaterial(int cd_curso, int cd_escola);
        ItemUI getItemUIbyId(int cd_item, int cdEscola);
        int quantidadeItensMaterialCurso(int cd_turma, int cd_curso);
        List<KitUI> getItensKit(int idKit);
        IEnumerable<ItemUI> obterListaItemsKit(int cd_item_kit, int cdEscola);
        IEnumerable<ItemUI> obterListaItemMovItemsKit(ItemMovimento item, int cdEscola);
        IEnumerable<ItemUI> obterListaItemsKitMov(int cd_item_kit, int cdEscola, int id_tipo_movto, int? id_natureza_TPNF);
        Movimento calcularQuantidadeItemKit(ItemUI item, int cdEscola);
        Movimento excluirKitDoGrid(ItemUI item, int cdEscola);

        //Desconto por Antecipação
        PoliticaDesconto getPoliticaEdit(int id, int cdEscola);
        PoliticaDesconto getPoliticaDescontoById(int id, int cdEscola);
        PoliticaDescontoUI postPoliticaDesconto(PoliticaDesconto politicaDesconto);
        bool deletePoliticaDesconto(PoliticaDesconto politicaDesconto);
        IEnumerable<PoliticaDescontoUI> getPoliticaDescontoSearch(SearchParameters parametros, int cdTurma, int cdAluno, DateTime? dtaIni, DateTime? dtaFim, bool? ativo, int cdEscola);
        bool deleteAllPoliticaDesconto(List<PoliticaDesconto> politicasDesconto, int cdEscola);
        IEnumerable<DiasPolitica> GetDiasPoliticaById(int cdPolitica, int cdEscola);
        IEnumerable<AlunosSemTituloGeradoUI> GetAlunosSemTituloGerado(int vl_mes, int ano, int cd_turma, string situacoes, int cd_escola);
        PoliticaDescontoUI postAlterarPoliticaDesconto(PoliticaDesconto politica);
        PoliticaDescontoUI getPoliticaDesconto(int cdEscola, int cd_politica_desconto);
        PoliticaDesconto getPoliticaDescontoByTurmaAluno(int cd_turma, int cd_aluno, DateTime dt_vcto_titulo);
        PoliticaDesconto getPoliticaDescontoByAluno(int cd_aluno, DateTime dt_vcto_titulo);
        PoliticaDesconto getPoliticaDescontoByTurma(int cd_turma, DateTime dt_vcto_titulo);
        PoliticaDesconto getPoliticaDescontoByEscola(int cd_pessoa_escola, DateTime dt_vcto_titulo);
        int getCriarDataPoliticaContrato(int cdTurma, int cdAluno, DateTime dataVencto, int cdEscola);

        //Item escola
        ItemEscola addItemEscola(ItemEscola itemEscola);
        ItemEscola findItemEscolabyId(int cdItem, int cdPessoa);
        ICollection<ItemEscola> getItensWithEscola(int cdItem, int cdUsuario);

        //Tipo item
        IEnumerable<TipoItem> getAllTipoItem(int? tipoMovimento);
        IEnumerable<TipoItem> getTipoItemMovimento(TipoItemDataAccess.TipoConsultaTipoItemEnum tipoConsulta);
        IEnumerable<TipoItem> getTipoItemMovimentoWithItem(int cd_pessoa_escola, bool isMaster);
        IEnumerable<TipoItem> getTipoItemMovimentoEstoque();

        //Bibiblioteca
        Biblioteca addBiblioteca(Biblioteca biblioteca);
        bool findItemBibliotecaById(int cdItem);

        // Tabela de Preços (Curso)
        IEnumerable<TabelaPrecoUI> GetTabelaPrecoSearch(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, DateTime? dtaCad, int cdEscola, int cdProduto);
        TabelaPreco getTabelaPrecoById(int id);
        TabelaPrecoUI postTabelaPreco(TabelaPreco tabelaPreco);
        bool deleteTabelaPreco(TabelaPreco tabela);
        bool deleteAllTabelaPreco(List<TabelaPreco> tabelas, int cdEscola);
        TabelaPrecoUI postAlterarTabelaPreco(TabelaPreco tabela);
        IEnumerable<TabelaPrecoUI> GetHistoricoTabelaPreco(SearchParameters parametros, int cdCurso, int cdDuracao, int cdRegime, int cdEscola);
        int? getNroParcelas(int cd_escola, int cd_curso, int cd_regime, int cd_duracao, DateTime data_matricula);
        Valores getValoresForMatricula(int cd_pessoa_escola, int cd_curso, int cd_duracao, int cd_regime, DateTime dta_matricula);

        //Plano contas
        IEnumerable<PlanoConta> getPlanoContasSearch(int cd_pessoa_empresa);
        PlanoContaUI addPlanoConta(ICollection<PlanoConta> planosContas, int tipoPlano, int cd_pessoa_empresa, byte nivel, out bool was_persisted, bool isContaSegura);
        PlanoContaUI excluirPlanoConta(ICollection<PlanoConta> planosContas, int tipoPlano, int nivel, int cd_pessoa_empresa, out bool was_deleted);
        PlanoConta confirmSubGrupoHasPlanoByIdSubgrupo(int cd_sub_grupo, int cd_pessoa_empresa);
        IEnumerable<RptPlanoTitulo> getPlanosContaPosicaoFinanceira(int cd_titulo);
        String getDescPlanoContaByEscola(int cd_pessoa_empresa, int cd_plano_conta);
        
        //Cheque
        Cheque getChequeByContrato(int id);
        Cheque getChequeByContratoPesq(int id);        
        bool excluirCheque(Cheque cheque);
        Cheque addCheque(Cheque cheque);
        ChequeTransacao addChequeTransacao(ChequeTransacao chequeTran);
        Cheque getChequeById(int cd_cheque);
        bool deleteCheque(Cheque cheque);
        Cheque editCheque(Cheque cheque);
        ChequeTransacao editChequeTransacao(ChequeTransacao chequeTran);
        List<Cheque> getChequesByTitulosContrato(List<int> cdTitulos, int cd_empresa);
        Cheque getChequeTransacao(int cd_tran_finan, int cd_empresa);

        //Banco
        IEnumerable<Banco> getAllBanco();
        Banco getBancobyId(int cdBanco);
        IEnumerable<Banco> getBancoSearch(SearchParameters parametros, string nome, string nmBanco, bool inicio);
        Banco postBanco(Banco banco);
        Banco putBanco(Banco banco);
        bool deleteAllBanco(List<Banco> bancos);
        IEnumerable<Banco> getBancoCarteira();
        IEnumerable<Banco> getBancosTituloCheque(int cd_empresa);

        //Titulo Aditamento
        void adicionaTituloAditamento(List<Titulo> titulos, Contrato contratoView);

        //Título
        List<Titulo> addTitulos(List<Titulo> titulos);
        List<Titulo> getTitulosByContrato(int cdContrato, int cdEscola);
        int getQtdContratoNaoMultiploDiferenteCartaoCheque(int cdContrato, int cdEscola);
        int getQtdMovimentoDiferenteCartaoCheque(int cdMovimento, int cdEscola);
        int getQtdTitulosSemBaixaTipoCartaoOuCheque(int cdContrato, int cdEscola);
        int getQtdTitulosMovimentoSemBaixaTipoCartaoOuCheque(int cdMovimento, int cdEscola);
        Titulo getTituloByContrato(int cd_contrato, int nro_parcela);
        Titulo getTituloAbertoByAditamento(int cd_aditamento);
        List<Titulo> getTitulosByRenegociacao(int cdContrato, int cdAluno, int cdEscola, int cdProduto);
        decimal getValorParcela(bool primeira_parcela, int cd_contrato, int cd_escola);
        List<Titulo> editTitulos(List<Titulo> titulos);
        List<Titulo> editTitulosContrato(List<Titulo> titulos, Contrato contratoView, double pc_bolsa);
        void deleteAllTitulo(List<Titulo> titulos, int cd_escola);
        bool deleteAllTitulo(int cd_contrato, int cd_escola, bool deletarBaixasBolsa);
        bool deletarTitulo(Titulo titulo, int cdEscola);
        IEnumerable<Titulo> getTitulosByContratoTodosDados(int cdContrato, int cdEscola);
        bool getTituloBaixadoContrato(int cd_contrato, int cdEscola, int cdTitulo); 
        IEnumerable<Titulo> searchTitulo(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int locMov, int natureza, int status, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool emissao, bool vencimento, bool baixa, int locMovBaixa,
                                               int cdTipoLiquidacao, bool contaSegura, byte tipoTitulo, string nossoNumero, int cnabStatus, int? nro_recibo, int? cd_turma,
                                               List<int> cd_situacoes_aluno, int? cd_tipo_financeiro);
        IEnumerable<Titulo> searchTituloCnab(TituloUI titulo);
        IEnumerable<Titulo> getTitulosForBaixaAutomatica(SearchParameters parametros, TituloUI titulo);
        IEnumerable<Titulo> getTitulosForBaixaAutomaticaCheque(TituloChequeUI titulo);
        IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCheque(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI);
        IEnumerable<BaixaAutomatica> listarBaixaAutomaticasEfetuadasCartao(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaChequeUI);
        IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCheque(SearchParameters parametros, BaixaAutomaticaUI automaticaChequeUi);
        IEnumerable<BaixaEfetuadaChequeUI> getBaixasEfetuadasForBaixaAutomaticaCartao(SearchParameters parametros, BaixaAutomaticaUI baixaAutomaticaCartaoUI);
        int gerarBaixaAutomatica(BaixaAutomaticaUI baixaAutomaticaUi);
        List<TituloCnab> searchTituloCnabGrade(TituloUI titulo);
        Titulo getTituloBaixaFinan(int cd_titulo, int cd_pessoa_empresa, TituloDataAccess.TipoConsultaTituloEnum tipoConsulta);
        Titulo getTituloBaixaFinan(string dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto);
        List<Titulo> getTitulosBaixaFinan(List<string> dc_nosso_numero, int cd_pessoa_empresa, int cd_local_movto);
        List<Titulo> getTituloBaixaFinanSimulacao(List<int> cdsTitulo, int cd_pessoa_empresa, int? cd_registro_origem, TituloDataAccess.TipoConsultaTituloEnum tipoConsulta);
        IEnumerable<RptReceberPagar> receberPagarStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, DateTime pDtaBase, byte pNatureza, int pPlanoContas, int ordem, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma);
        IEnumerable<RptRecebidaPaga> recebidaPagaStoreProcedure(int cdEscola, DateTime pDtaI, DateTime pDtaF, int pForn, byte pNatureza, int pPlanoContas, bool pMostraCCManual, bool cSegura, int cdTpLiq, int cdTpFinan, int tipo, string situacoes, int cdTurma, bool? ckCancelamento, int cdLocal);
        IEnumerable<ObservacaoBaixaUI> getObservacoesBaixaCancelamento(int cdEscola, int cd_baixa_titulo);
        Titulo editTitulosBaixaFinan(Titulo titulo);
        IEnumerable<Titulo> getTituloByPessoa(SearchParameters parametros, int cd_pessoa, int cd_escola, TituloDataAccess.TipoConsultaTituloEnum tipo, bool contaSeg);
        IEnumerable<Titulo> getTituloByPessoaResponsavel(int cd_pessoa_titulo, int cd_escola, int cd_contrato, bool contaSeg);
        Titulo voltarEstadoAnteriorTitulo(BaixaTitulo baixa, int cd_tran_finan, int cd_pessoa_empresa, Titulo titulo);
        Titulo aplicarBaixaTitulo(BaixaTitulo baixa, Titulo titulo);
        Titulo aplicarTituloTaxaBancaria(Titulo titulo);
        List<Titulo> getTitulosGridByMovimento(int cd_movto, int cd_empresa, int? cd_aluno);
        IEnumerable<CarneUI> getCarnePorContrato(int cdContrato, int cdEscola, int parcIniCarne, int parcFimCarne);
        IEnumerable<CarneUI> getCarnePorMovimentos(int cdMovimento, int cdEscola);
        IEnumerable<Titulo> getTitulosByMovimento(int cd_movto, int cd_empresa);
        void deletarBaixasBolsaTituloContrato(int cd_contrato, int cd_escola);
        //List<Titulo> gerarTitulosMovimento(Titulo tituloDefault, Movimento movimento, bool? diaUtil, byte? nmDia, bool id_alterar_venc_final_semana);
        bool verificarTituloContaSegura(int cd_titulo, int cd_empresa);
        IEnumerable<Titulo> getTitulosByOrigem(int cdOrigemTitulo, int idOrigemTitulo, int cd_empresa);
        bool verificarStatusCnabTitulo(int[] cdTituls, int cd_empresa, int cd_cnab, byte id_tipo_cnab);
        IEnumerable<Titulo> getDadosAdicionaisTituloParaCnab(int[] cdTitulos, int cd_empresa);
        IEnumerable<Titulo> getTitulosByCnab(int cd_cnab, int cd_empresa);
        void trocarStatusCnabTitulos(int[] cdCnabs, int cd_empresa, Titulo.StatusCnabTitulo statusTitulo, int? cd_contrato = null);
        Decimal getSaldoTitulosMatricula(int cd_empresa, int cd_contrato);
        IEnumerable<Titulo> searchTituloFaturamento(SearchParameters parametros, int cd_pessoa_empresa, int cd_pessoa, bool responsavel, bool inicio, int? numeroTitulo,
                                               int? parcelaTitulo, string valorTitulo, DateTime? dtInicial, DateTime? dtFinal, bool contaSegura, byte tipoTitulo);
        Titulo getTituloBaixaFinanMovimentoNF(int cd_baixa_titulo, int cd_pessoa_empresa);
        List<Titulo> getTitulosByContratoLeitura(int cdContrato, int cdEscola);

        List<Titulo> getTitulosAbertosImpressaoAdt(int cdContrato, int cdEscola);
        List<Titulo> getTitulosAbertosAdicionarParcImpressaoAdt(int cdContrato, int cdEscola, int qtd_titulos_adt);
        IEnumerable<ChequeUI> getRptChequesAbertos(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
        int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza);
        IEnumerable<ChequeUI> getRptChequesLiquidados(int cd_empresa, int cd_pessoa_aluno, int cd_banco, string emitente, bool liquidados, string nm_cheques, string vl_titulo, int nm_agencia,
        int nm_ccorrente, DateTime? dt_ini_bPara, DateTime? dt_fim_bPara, DateTime? dt_ini, DateTime? dt_fim, bool emissao, bool liquidacao, byte natureza);
        bool verificaTituloOrContratoBaixaEfetuada(int cd_contrato, int cdEscola, int cdTitulo);
        IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa);
        void deletarTitulosEdicaoMatricula(Contrato contratoView, double pcBolsa);
        bool verificaTituloVencido(int cdPessoa, DateTime dataHoje, int cd_escola);
        List<Titulo> getTitulosContrato(int cd_contrato);
        IEnumerable<Titulo> getTitulosContrato(List<int> cdTitulos, int cd_escola);
        IEnumerable<ReajusteTitulo> getReajusteTitulos(int cd_empresa, int cd_reajuste_anual);
        bool verificaTitulosFechamentoReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        void reverterAlteracaoTituloReajusteAnual(int cd_pessoa_escola, int cd_reajuste_anual);
        List<int> getAlunosQuePossuemTitulosAbertoMes(List<int> cdPessoaAlunos, int cd_empresa, DateTime dt_diario);
        bool verificaTituloEnviadoCnabOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo);
        bool verificaTituloEnviadoBoletoOuContrato(int cd_contrato, int cdEscola, List<int> cdsTitulo);

        //Baixa
        TransacaoFinanceira postIncluirTransacao(TransacaoFinanceira transacao, bool baixarBolsaAutomatica);
        TransacaoFinanceira postIncluirTransacao(TransacaoFinanceira transacao, bool baixarBolsaAutomatica, bool eh_cnab, Parametro parametros = null);
        TransacaoFinanceira editTransacao(TransacaoFinanceira transacao, bool aperacao_sistema);
        TransacaoFinanceira editTransacao(TransacaoFinanceira transacao, bool aperacao_sistema, Parametro parametros);
        IEnumerable<BaixaTitulo> getBaixaTituloByIdTitulo(int cd_titulo, int cd_pessoa_empresa);
        IEnumerable<BaixaTitulo> getBaixasTransacaoFinan(int cd_transacao_finan,int cd_baixa_titulo, int cd_titulo, int cd_pessoa_empresa, BaixaTituloDataAccess.TipoConsultaBaixaEnum tipoConsulta);
        Recibo getReciboByBaixa(int cd_baixa, int cd_empresa);
        ReciboAgrupadoUI getReciboAgrupado(string cds_titulos_selecionados, int cd_empresa);
        ReciboPagamentoUI getReciboPagamentoByBaixa(int cd_baixa, int cd_empresa);
	    ReciboPagamentoUI getVerificaReciboPagamentoByBaixa(int cd_baixa, int cd_empresa);
        bool validaReciboAgrupadoAlunosDiferentes(List<int> cds_titulos_selecionados, int cd_empresa);
        bool validaReciboAgrupadoResponsaveisDiferentes(List<int> cds_titulos_selecionados, int cd_empresa);
        ReciboConfirmacaoUI getReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa);
        ReciboConfirmacaoUI getReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa);
        List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByContrato(int cd_contrato, int cd_empresa);
        List<ReciboConfirmacaoParcelasUI> getParcelasReciboConfirmacaoByMovimento(int cd_movimento, int cd_empresa);
        bool deleteBaixaTitulo(BaixaTitulo baixa);
        string getResponsavelTitulo(int cd_titulo, int cd_pessoa_empresa);
        IEnumerable<Titulo> getTitulosAbertoContrato(int cd_empresa, int cd_contrato);
        bool verificarTituloOrigemMatricula(int cd_baixa, int cd_escola);
        void gerarBaixaParcialBolsaTitulos(List<Titulo> titulos, double pc_bolsa, DateTime dt_emissao_titulo, bool nova_matricula, bool aditivoBolsa = false);
        IEnumerable<BaixaTitulo> getBaixaTitulosBolsaContrato(int cd_contrato, int cd_pessoa_empresa, List<int> cdTitulos);

        //Local Movto
        List<LocalMovto> getLocalMovtoByEscola(int cdEscola, int cd_local_movto, bool semcarteira);
        List<LocalMovto> getAllLocalMovtoByEscola(int cdEscola, int cd_local_movto);
        List<LocalMovto> getLocalMovtoCdEEsc(int cdEscola, int? cdLocalMovto);
        IEnumerable<LocalMovtoUI> getLocalMovtoSearch(SearchParameters parametros, int cdEscola, string nome, string nmBanco, bool inicio, bool? status, int tipo, string pessoa, int cd_pessoa_usuario);
        LocalMovtoUI getLocalMovtoById(int cdEscola, int cdLocalMovto);
        IEnumerable<LocalMovto> getAllLocalMovto(int cdEscola, bool isOrigem, int cd_pessoa_usuario);    
        bool deleteAllLocalMovto(List<LocalMovto> locais);
        LocalMovtoUI postLocalMovto(LocalMovto local);
        LocalMovtoUI putLocalMovto(LocalMovto local);
        List<LocalMovto> getLocalMovimentoSomenteLeitura(int cdEscola, int cd_loc_mvto, LocalMovtoDataAccess.TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario);
        List<LocalMovto> getLocalMovimentoSomenteLeituraComFiltrosTrocaFinanceira(int cdEscola, int cd_loc_mvto, int cd_tipo_financeiro, LocalMovtoDataAccess.TipoConsultaLocalMovto tipoConsulta, int cd_pessoa_usuario);
        List<LocalMovto> getLocalMovtoBaixa(int cd_escola, int? cd_loc_mvto, int natureza, int[] listPessoas, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getLocalMovtoAtivosWithConta(int cdEscola, bool isOrigem, bool isCadastrar, int cdLocalMovto);
        IEnumerable<LocalMovto> getLocalMovtoAtivosWithContaUsuario(int cdEscola, bool isOrigem, int cdLocalMovto, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getLocalMovtoAtivosWithCodigo(int cdEscola, bool isOrigem, int cd_local);
        LocalMovto findLocalMovtoById(int cdEscola, int cdLocalMovto);
        LocalMovto findLocalMovtoComCarteira(int cdEscola, int cdLocalMovto);
        IEnumerable<LocalMovimentoWithContaUI> getLocalMovtoWithContaByEscola(int cdEscola, int cd_pessoa_usuario);
        bool verificaCarteiraCnab(int cdCarteira);
        long getNossoNumeroLocalMovimento(int cd_escola, int cd_local_movto);
        LocalMovto getLocalMovimentoWithPessoaBanco(int cd_local_movto);
        LocalMovto findCodigoClienteForCnab(int cd_empresa, int cd_local_movto);
        LocalMovtoUI getLocalByTitulo(int cdEscola, int cd_local_movto);
        IEnumerable<LocalMovto> getLocalMovtoProspect(int cdEscola, int cd_loc_mvto, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getAllLocalMovtoCartao(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoCartaoSemPai(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoCartaoComPai(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoTipoCartao(int cdEscola, int cd_tipo_liquidacao, int cd_local_movto, int cd_pessoa_usuario);
        IEnumerable<LocalMovto> getAllLocalMovtoCheque(int cdEscola);
        IEnumerable<LocalMovto> getAllLocalMovtoBanco(int cdEscola); 

        // Taxa Bancaria
        TaxaBancaria getTaxaBancariaPorId(int cd_taxa_bancaria);

        //Transação Financeira
        TransacaoFinanceira getTransacaoFinanceira(int cd_tran_finan, int cd_pessoa_empresa);
        bool deleteTransFinanBaixa(TransacaoFinanceira transFinan);
        IEnumerable<TipoLiquidacao> getTipoLiquidacao();
        TransacaoFinanceira getTransacaoBaixaTitulo(int cd_titulo, int cd_pessoa_empresa);       

        //Plano Título
        PlanoTitulo getPlanoTituloByTitulo(int cdTitulo, int cdPlanoConta, int cdEscola);
        bool deletePlanoTitulo(PlanoTitulo planoTitulo);

        //Política Comercial
        PoliticaComercial getPoliticaComercialById(int cdPoliticaComercial, int cdEscola);
        bool deletePoliticaComercial(PoliticaComercial polComercial);
        PoliticaComercial addPoliticaComercial(PoliticaComercial polComercial);
        PoliticaComercial editPolCom(PoliticaComercial polComercial);
        IEnumerable<PoliticaComercial> getPoliticaComercialSearch(SearchParameters parametros, string descricao, bool inicio, bool? ativo, bool parcIguais, bool vencFixo, int cdEscola);
        IEnumerable<PoliticaComercial> getPoliticaComercialByEmpresa(int cd_pessoa_escola, string dc_politica, bool inicio);
        bool deleteAllPolCom(List<PoliticaComercial> politicas, int cd_escola);
        PoliticaComercial getPoliticaComercialSugeridaNF(int cd_escola);

        //Kardex
        Kardex addKardex(Kardex kardex);
        Kardex editKardex(Kardex kardex);
        bool deleteKardex(Kardex kardex);
        IEnumerable<Kardex> getKardexByOrigem(int cd_origem, int cd_registro_origem);
        IEnumerable<KardexUI> st_Rptkardex(int cd_pessoa_escola, int cd_item, DateTime dt_ini, DateTime dt_fim, int cd_grupo, byte tipo, bool isApenasItensMovimento);
        int getSaldoItem(int cd_item, DateTime dataLimite, int cd_escola);
        void atualizaKardex(Kardex kardex);
        IEnumerable<Kardex> getKardexItensMovimentoNF(int cd_movimento, int cd_pessoa);
        bool existeKardexItemMovimentoByOrigem(int cd_origem, int cd_registro_origem);
        //List<sp_RptInventario_Result> getRtpInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor);
        DataTable getRtpInventario(int? cd_escola, DateTime? dt_analise, byte? id_valor, string tipoItem);

        //Conta Corrente
        IEnumerable<ContaCorrenteUI> getContaCorreteSearch(SearchParameters parametros, int cd_pessoa_escola, int cd_origem, int cd_destino, byte entraSaida, int cd_movimento, int cd_plano_conta, DateTime? dta_ini, DateTime? dta_fim, int cd_pessoa_usuario, bool contaSegura);
        ContaCorrenteUI incluirContaCorrente(ContaCorrenteUI contaCorrente, int cd_pessoa_escola, bool isSupervisor, int movimentoRetroativo);
        bool deleteContaCorrente(ICollection<ContaCorrenteUI> contaCorrenteUI, bool isSupervisor, int movimentoRetroativo);
        ContaCorrenteUI editarContaCorrente(ContaCorrenteUI contaCorrenteUI, bool isSupervisor, int movimentoRetroativo);
        IEnumerable<ContaCorrenteUI> getRelatorioContaCorrente(int cd_pessoa_escola, DateTime? dta_ini, DateTime? dta_fim, int cd_local_movto, int tipoLiquidacao, bool contaSegura, bool isMaster);
        IEnumerable<RptBalanceteMensal> getBalanceteMensal(int cdEscola, int mes, int ano, int nivel, int nivel_analisar, bool mostrar_contas, bool conta_segura);
        IEnumerable<ContaCorrente> getObservacoesCCBaixa(int? cd_baixa_titulo, int? cd_conta_corrente);
        IEnumerable<SaldoFinanceiro> getRelatorioSaldoFinanceiro(int cd_pessoa_escola, DateTime dt_base, byte tipoLocal, bool liquidacao);
        ContaCorrenteUI getContaCorretePlanoConta(int cd_pessoa_escola, int cd_conta_corrente);
        IEnumerable<ContaCorrenteUI> getFechamentoCaixaTpLiquidacao(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados);
        IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovto(int cd_pessoa_escola, DateTime dta_fechamento, int tipoLiquidacao, int cdUsuario, byte tipoLocal, bool mostrarZerados);
        void postZerarSaldoFinanceiro(int cd_escola, int cd_tipo_liquidacao, Nullable<System.DateTime> dta_base, byte tipo);
        IEnumerable<ContaCorrenteUI> getFechamentoCaixaLocalMovtoRel(int cd_pessoa_escola, DateTime dta_fechamento, int cdUsuario, byte tipoLocal, bool mostrarZerados);
        ObsSaldoCaixa postObsSaldoCaixaUsuario(ObsSaldoCaixa obsSaldoCaixa, int cdEscola);
        ObsSaldoCaixa getObsSaldoCaixaConsolidado(int cdEscola, DateTime dt_saldo_caixa, int cd_usuario);

        //Fechamento de Estoque
        IEnumerable<Fechamento> getFechamentoSearch(SearchParameters parametros, int? ano, int? mes, bool balanco, DateTime? dta_ini, DateTime? dta_fim, int cd_escola);
        Fechamento getFechamentoById(int cd_fechamento, int cd_escola);
        bool existeFechamentoAnoMes(DateTime data, int cd_escola, int cd_fechamento);
        Fechamento postFechamento(Fechamento fechamento);
        IEnumerable<SaldoItem> postGerarEstoque(Fechamento fechamento, int cd_escola);
        Fechamento postAlterarFechamento(Fechamento fechamento);
        IEnumerable<Fechamento> fechamentoAnoMes(int cd_escola);
        bool existeFechamentoSuperior(DateTime data, int cd_escola, int cd_fechamento);
        SaldoItem getSaldoItemLocal(SaldoItem saldos);
        bool deleteFechamentos(List<int> cdFechamentos, int cd_empresa);
        Fechamento getFechamentoByDta(DateTime data, int cd_empresa);

        //ItemSubgrupo
        IEnumerable<ItemSubgrupo> getSubGrupoTpHasItem(int cd_item);
        ItemSubgrupo getSubGrupoPlano(int cd_item, byte tipo, int cdEscola);        
        
        //Situação Tributaria
        IEnumerable<SituacaoTributaria> getSituacaoTributaria(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int cdTpNF);
        SituacaoTributaria getSituacaoTributariaItem(int cd_grupo_estoque, int id_regime_tributario, int cdSitTrib);
        IEnumerable<SituacaoTributaria> getSituacaoTributariaTipo(SituacaoTributariaDataAccess.TipoConsultaSitTribEnum tipo, List<int> cd_situacoes, int tipoImp,
                                                                  int cd_escola, byte id_regime_trib, bool master_geral);
        SituacaoTributaria getSituacaoTributariaFormaTrib(int cd_situacao_trib);

        //Aliquota UF
        IEnumerable<AliquotaUF> getAliquotaUFSearch(SearchParameters parametros, int cdEstadoOri, int cdEstadoDest, double? aliquota);
        AliquotaUF getEstadosPesq();
        AliquotaUF postAliquotaUF(AliquotaUF aliquota);
        AliquotaUF getAliquotaUFByOriDes(int cdEstadoOri, int cdEstadoDest);
        AliquotaUF getAliquotaUFByEscDes(int cdEscola, int cdEstadoDest);
        AliquotaUF getAliquotaUFPorEstadoPessoa(int cdEscola, int cd_pessoa_cliente);
        AliquotaUF getAliquotaUFById(int cd_aliquota_uf);

        //Dados NF
        IEnumerable<DadosNF> getDadosNFSearch(SearchParameters parametros, int cdCidade, string natOp, double? aliquota, byte id_regime);
        DadosNF getDadosNFById(int cdDadoNF);
        DadosNF postDadosNF(DadosNF dado);
        bool getDadosCidade(int cdCidade);
        double? getISSEscola(int cdEscola);
        bool posDeletetAliquotaUF(AliquotaUF aliquota);
        bool postDeleteDadosNF(DadosNF dado);

        // Ajuste Anual
        IEnumerable<ReajusteAnual> getReajusteAnualSearch(SearchParameters parametros, int cd_empresa, int cd_usuario, int status, DateTime? dtaInicial, DateTime? dtaFinal, bool cadastro, bool vctoInicial, int cd_reajuste_anual);
        ReajusteAnual addReajusteAnual(ReajusteAnual reajuste);
        Boolean deleteAllReajuste(List<ReajusteAnual> grupos, int cd_empresa);
        ReajusteAnual getReajusteAnualForEdit(int cd_empresa, int cd_reajuste_anual);
        ReajusteAnual postUpdateReajusteAnual(ReajusteAnual reajuste);
        IEnumerable<Titulo> getTitulosReajusteAnual(int cd_escola, int cd_reajuste_anual, DateTime dt_ini_venc, DateTime? dt_fim_venc);
        decimal getSaldoContratoParaReajusteAnual(int cd_escola, decimal pc_bolsa, int cd_contrato, List<int> cdsTitulos);
        ReajusteAnual getReajusteAnualFull(int cd_reajuste_anual, int cd_empresa);
        List<int> getCodigoContratoTitulosReajusteAnual(int cd_empresa, int cd_reajuste_anual);
        ReajusteAnual getReajusteAnualGridView(int cd_reajuste_anual, int cd_empresa);

        //relatório Controle de Vendas de Material
        IEnumerable<RptContVendasMaterial> getRptContVendasMaterial(int cd_escola, int cd_aluno, int cd_item, DateTime dt_inicial, DateTime dt_final, int cd_turma, bool semmaterial);

        //Titulos
        List<Titulo> getTitulosByTituloAditamento(int cd_escola, int cd_aditamento);
        bool deleteTitulosByTituloAditamento(int cd_escola, string cd_aditamento);


        Cheque getChequeTransacaoTrocaFinanceira(int cd_titulo, int cdEscola);
        void alterarResponsavelTitulos(int cd_contrato, int cd_escola, List<Titulo> titulos);
        void alterarDtVctoTitulos(int cd_contrato, int cd_escola, List<Titulo> titulos);
        bool updateNossoNumeroTitulo(int numeroDocumento, int cd_pessoa_empresa, int cd_local_movto, string nossoNumero);
        int findNotaAluno(int cd_aluno, int cd_curso);
    }
}