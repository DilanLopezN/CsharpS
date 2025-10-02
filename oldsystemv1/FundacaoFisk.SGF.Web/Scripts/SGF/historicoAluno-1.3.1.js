function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) tab = dojo.query(e.target)[0];// Clicou na borda da aba

        //if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabHistoricoTurmas') {

        //}
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTitulo') 
            atualizarTitulos();

        if (tab.getAttribute('widgetId') == 'InfTurmaEstagio_tablist_tabEstagio')
            atualizarTurmasEstagios();

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAvaliacao')
            atualizarTurmasAvaliacoes();

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAtividade')
            atualizarAtividadeExtra();

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabFollow')
            atualizarTabFollow();

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabItens')
            atualizarTabItens();

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabObsevacaoAluno')
            atualizarObservacaoAluno();

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMatricula' /*&& !hasValue(dijit.byId("gridTabMatriculas"))*/) {
            destroyCreateGridTabMatricula();
            if (hasValue(dojo.byId("cd_aluno_pesq").value) && dojo.byId("cd_aluno_pesq").value > 0)
                criarGridTabMatriculas(dojo.byId("cd_aluno_pesq").value);
            else
                criarGridTabMatriculas();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTabTit(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) tab = dojo.query(e.target)[0];// Clicou na borda da aba

        if (tab.getAttribute('widgetId') == 'tabTituloStatus_tablist_tabTitulo')
            dijit.byId('gridTituloHist').resize(true);
        if (tab.getAttribute('widgetId') == 'tabTituloStatus_tablist_tabTituloF')
            dijit.byId('gridTituloHistF').resize(true);
        if (tab.getAttribute('widgetId') == 'tabTituloStatus_tablist_tabTituloC')
            dijit.byId('gridTituloHistC').resize(true);
        if (tab.getAttribute('widgetId') == 'tabTituloStatus_tablist_tabTituloT')
            dijit.byId('gridTituloHistT').resize(true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTabAva(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) tab = dojo.query(e.target)[0];

        if (tab.getAttribute('widgetId') == 'tabInfAval_tablist_tabAvaliacaoConceito') {
            var gridTurmasAvaliacao = dijit.byId('gridTurmasAvaliacao');
            var turmaSelecionada = gridTurmasAvaliacao.turmaSelecionada;

			if(hasValue(turmaSelecionada))
				getConceitosAvaliacaoAluno(turmaSelecionada);
        }
        if (tab.getAttribute('widgetId') == 'tabInfAval_tablist_tabAvaliacaoEventos') {
            var gridTurmasAvaliacao = dijit.byId('gridTurmasAvaliacao');
            if(hasValue(gridTurmasAvaliacao.turmaSelecionada)){
				var turmaSelecionada = gridTurmasAvaliacao.turmaSelecionada;
				if (hasValue(turmaSelecionada))
                	getEventosAvaliacaoAluno(turmaSelecionada);
			}
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTabAval(e) {
    try {

        var tab = dojo.query(e.target)[0];

        //if (!hasValue(tab)) tab = dojo.query(e.target)[0];// Clicou na borda da aba

        if ((tab.id || tab.getAttribute('widgetId')) == 'InfTurmaEstagio_tablist_tabTurma') {
            dojo.byId('tgAvaliacao').style.display = 'block';
            dojo.byId('tgAvalEstagio').style.display = 'none';
            dijit.byId('tgAvalEstagio').set('open', false);
            dijit.byId('tgAvaliacao').set('open', true);


            atualizarTurmasAvaliacoes();
            //SetarAvaliacao(0);
        }
        if ((tab.id || tab.getAttribute('widgetId')) == 'InfTurmaEstagio_tablist_tabEstagio') {
            dojo.byId('tgAvaliacao').style.display = 'none';
            dojo.byId('tgAvalEstagio').style.display = 'block';
            dijit.byId('tgAvalEstagio').set('open', true);
            dijit.byId('tgAvaliacao').set('open', false);
        }

    } catch (e) {
        postGerarLog(e);

    }
    
}

function montarHistoricoAluno(permissoes) {
    require([
        
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dojo/on",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dojo/_base/xhr",
        "dijit/form/FilteringSelect",
        "dijit/registry",
        "dojo/data/ObjectStore",
    ], function (EnhancedGrid, Pagination, ObjectStore, Memory, Button, ready, on, DropDownButton, DropDownMenu, MenuItem, dom, xhr, FilteringSelect, registry, ObjectStore ) {
        ready(function () {
            try {
                dijit.byId("tabContainer").resize();
                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                var gridTurmasAvaliacao = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Turma", field: "no_turma", width: "25%" },
                        { name: "Produto", field: "no_produto", width: "8%" },
                        { name: "Sala", field: "no_sala", width: "10%" },
                        { name: "Professor", field: "no_professor", width: "25%" },
                        { name: "Aulas Dadas", field: "nm_aulas_dadas", width: "8%", styles: "text-align: center;" },
                        { name: "Faltas", field: "nm_faltas", width: "8%", styles: "text-align: center;" },
                        { name: "Contratadas", field: "nm_aulas_contratadas", width: "9%", styles: "text-align: center;" },
                        { name: "Início Turma", field: "dta_inicio_aula", width: "8%" },
                        { name: "Prev. Término", field: "dta_final_aula", width: "8%" },
                        { name: "Término", field: "dt_termino", width: "8%" }
                        ],
                    noDataMessage: msgNotRegEnc
                }, "gridTurmasAvaliacao");

                gridTurmasAvaliacao.canSort = function () { return true };
                gridTurmasAvaliacao.startup();

                gridTurmasAvaliacao.on("RowClick", function (evt) {
                    try{
                        var idx = evt.rowIndex, item = this.getItem(idx);
                        this.turmaSelecionada = item;

                        //Posiciona na primeira aba:
                        var tabs = dijit.byId("tabInfAval");
                        var pane = dijit.byId("tabAvaliacaoMedia");
                        tabs.selectChild(pane);

                        getMediasAvaliacaoAluno(item);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                //*** Cria a grade Média de Avaliações **\\
                var gridAvaliacao = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Avaliação", field: "dc_tipo_avaliacao", width: "55%", styles: "min-width:40px; text-align: left;" },
                        { name: "Média", field: "vlMedia", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Soma", field: "vlSoma", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Total", field: "vlTotal", width: "15%", styles: "min-width:40px; text-align: right;" }
                        ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc
                }, "gridAvaliacao");
                gridAvaliacao.startup();

                //*** Cria a grade Notas de Avaliações **\\
                var gridAvaliacaoNota = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Avaliação", field: "dc_tipo_avaliacao", width: "35%", styles: "min-width:40px; text-align: left;" },
                        { name: "Nome", field: "dc_criterio_avaliacao", width: "35%", styles: "min-width:40px; text-align: left;" },
                        { name: "Data", field: "dta_avaliacao_turma", width: "15%", styles: "min-width:40px; text-align: center;" },
                        { name: "Nota", field: "vlNotaCorrigida", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "2ª Chance", field: "vlNota2", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Valor", field: "vlNota", width: "15%", styles: "min-width:40px; text-align: right;" }
                        ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc
                }, "gridAvaliacaoNota");
                gridAvaliacaoNota.startup();
                //dijit.byId('tgAvaliacao').resize();

                //*** Cria a grade Conceitos **\\
                var gridAvaliacaoConc = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Avaliação", field: "dc_tipo_avaliacao", width: "35%", styles: "min-width:40px; text-align: left;" },
                        { name: "Nome", field: "dc_criterio_avaliacao", width: "35%", styles: "min-width:40px; text-align: left;" },
                        { name: "Data", field: "dta_avaliacao_turma", width: "15%", styles: "min-width:40px; text-align: center;" },
                        { name: "Conceito", field: "no_conceito", width: "15%", styles: "min-width:40px; text-align: center;" },
                        ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc
                }, "gridAvaliacaoConc");
                gridAvaliacaoConc.startup();

                //*** Cria a grade de Eventos **\\
                var gridEvento = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Data", field: "dta_aula", width: "15%", styles: "min-width:40px; text-align: center;" },
                        { name: "Nome", field: "no_evento", width: "35%", styles: "min-width:40px; text-align: left;" }
                        ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc
                }, "gridEvento");
                gridEvento.startup();


                //Grade dos Estágios
                //var dataEstagio = [
                //    { no_estagio: 'ATS - Aiming at the Sky', no_produto: 'Inglês', nm_aulasDadas: '20', nm_faltas: '5', nm_contratadas: '40' },
                //    { no_estagio: 'BRF - Breaking Free', no_produto: 'Inglês', nm_aulasDadas: '10', nm_faltas: '0', nm_contratadas: '20' },
                //    { no_estagio: 'EXP - Expand Horizons', no_produto: 'Inglês', nm_aulasDadas: '10', nm_faltas: '0', nm_contratadas: '20' },
                //    { no_estagio: 'ESPL3 - Espanhol com 3', no_produto: 'Espanhol', nm_aulasDadas: '8', nm_faltas: '1', nm_contratadas: '20' }
                //]
                var gridEstagio = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Estagio", field: "no_estagio", width: "35%" },
                        { name: "Produto", field: "no_produto", width: "18%" },
                        { name: "1° Aula", field: "primeira_aula", width: "8%", styles: "text-align: center;", formatter: dataAtualFormatada },
                        { name: "Ult. Aula", field: "ultima_aula", width: "8%", styles: "text-align: center;", formatter: dataAtualFormatada },
                        { name: "Aulas dadas", field: "nm_aulas_dadas", width: "8%", styles: "text-align: center;" },
                        { name: "Faltas", field: "nm_faltas", width: "8%", styles: "text-align: center;" },
                        { name: "Contratadas", field: "nm_aulas_contratadas", width: "9%", styles: "text-align: center;" }
                    ],
                    canSort: false,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridEstagio");

                gridEstagio.canSort = function () { return true };
                gridEstagio.startup();
                gridEstagio.on("RowClick", function (evt) {

                    var idx = evt.rowIndex, item = this.getItem(idx);
                    this.estagioSelecionado = item;

                    //console.log(this.estagioSelecionado);
                    

                    ////Posiciona na primeira aba:
                    //var tabs = dijit.byId("tabInfAval");
                    //var pane = dijit.byId("tabAvaliacaoMedia");
                    //tabs.selectChild(pane);
                    // SetarAvalEstagio(evt.rowIndex, ObjectStore, Memory);
                    SetarAvalEstagio(item);
                }, true);


                var gridAvalEstagioTurma = new EnhancedGrid({
                    structure:
                    [
                        { name: "Turma", field: "no_turma", width: "55%", styles: "min-width:40px; text-align: left;" },
                        { name: "Avaliação", field: "dc_tipo_avaliacao", width: "25%", styles: "min-width:40px; text-align: left;" },
                        { name: "Média", field: "vlMedia", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Soma", field: "vlSoma", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Total", field: "vlTotal", width: "15%", styles: "min-width:40px; text-align: right;" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridAvaliacaoEstagioTurma");
                gridAvalEstagioTurma.startup();

                //*** Cria a grade Média de Avaliações do Estagio por Turma e Avaliacao **\\
                var gridAvalEstagio = new EnhancedGrid({
                    structure:
                    [
                        { name: "Avaliação", field: "dc_tipo_avaliacao", width: "75%", styles: "min-width:40px; text-align: left;" },
                        { name: "Média", field: "vlMedia", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Soma", field: "vlSoma", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Total", field: "vlTotal", width: "15%", styles: "min-width:40px; text-align: right;" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridAvaliacaoEstagio");
                gridAvalEstagio.startup();

                //*** Cria a grade Notas de Avaliações do Estágio**\\
                var gridNotasEstagio = new EnhancedGrid({
                    structure:
                    [
                        { name: "Turma", field: "no_turma", width: "35%", styles: "min-width:40px; text-align: left;" },
                        { name: "Avaliação", field: "dc_tipo_avaliacao", width: "25%", styles: "min-width:40px; text-align: left;" },
                        { name: "Nome", field: "dc_criterio_avaliacao", width: "25%", styles: "min-width:40px; text-align: left;" },
                        { name: "Data", field: "dta_avaliacao_turma", width: "15%", styles: "min-width:40px; text-align: center;" },
                        { name: "Nota", field: "vlNotaCorrigida", width: "10%", styles: "min-width:40px; text-align: center;" },
                        { name: "2ª Chance", field: "vlNota2", width: "10%", styles: "min-width:40px; text-align: center;" },
                        { name: "Valor", field: "vlNota", width: "10%", styles: "min-width:40px; text-align: center;" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridNotasEstagio");
                gridNotasEstagio.startup();


                //*** Cria a grade de Titulos Abertos**\\
                var gridTituloHist = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Tipo Financeiro", field: "tipoDoc", width: "15%", styles: "min-width:40px; text-align: left;" },
                        { name: "Número", field: "nm_titulo", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:40px; text-align: center;" },
                        { name: "Vencimento", field: "dt_vcto", width: "10%", styles: "min-width:40px; text-align: left;" },
						{ name: "Atraso", field: "nm_atraso", width: "5%", styles: "min-width:40px; text-align: center;" },
                        { name: "Valor", field: "vlTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Dta.Pagto.", field: "dt_liquidacao", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Vl.Pagto.", field: "vlLiquidacaoBaixa", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo", field: "dc_tipo_titulo", width: "5%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo Baixa", field: "dc_tipo_liquidacao", width: "15%", styles: "min-width:40px; text-align: left;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["21", "42", "84", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "21",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTituloHist");
                gridTituloHist.startup();

                gridTituloHist.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 5 && Math.abs(col) != 10; };

                //*** Cria a grade de Titulos Fechados **\\
                var gridTituloHistF = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Tipo Financeiro", field: "tipoDoc", width: "15%", styles: "min-width:40px; text-align: left;" },
                        { name: "Número", field: "nm_titulo", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:40px; text-align: center;" },
                        { name: "Vencimento", field: "dt_vcto", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Atraso", field: "nm_atraso", width: "5%", styles: "min-width:40px; text-align: center;" },
	                    { name: "Valor", field: "vlTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Dta.Pagto.", field: "dt_liquidacao", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Vl.Pagto.", field: "vlLiquidacaoBaixa", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo", field: "dc_tipo_titulo", width: "5%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo Baixa", field: "dc_tipo_liquidacao", width: "15%", styles: "min-width:40px; text-align: left;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["21", "42", "84", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "21",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTituloHistF");
                gridTituloHistF.startup();

                gridTituloHistF.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 5 && Math.abs(col) != 10; };

                //*** Cria a grade de Titulos Cancelados**\\
                var gridTituloHistC = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Tipo Financeiro", field: "tipoDoc", width: "15%", styles: "min-width:40px; text-align: left;" },
                        { name: "Número", field: "nm_titulo", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:40px; text-align: center;" },
                        { name: "Vencimento", field: "dt_vcto", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Atraso", field: "nm_atraso", width: "5%", styles: "min-width:40px; text-align: center;" },
	                    { name: "Valor", field: "vlTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Dta.Pagto.", field: "dt_liquidacao", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Vl.Pagto.", field: "vlLiquidacaoBaixa", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo", field: "dc_tipo_titulo", width: "5%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo Baixa", field: "dc_tipo_liquidacao", width: "15%", styles: "min-width:40px; text-align: left;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["21", "42", "84", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "21",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTituloHistC");
                gridTituloHistC.startup();

                gridTituloHistC.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 5 && Math.abs(col) != 10; };

                //*** Cria a grade de Titulos Totais**\\
                var gridTituloHistT = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Tipo Financeiro", field: "tipoDoc", width: "15%", styles: "min-width:40px; text-align: left;" },
                        { name: "Número", field: "nm_titulo", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Parc.", field: "nm_parcela_titulo", width: "5%", styles: "min-width:40px; text-align: center;" },
                        { name: "Vencimento", field: "dt_vcto", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Atraso", field: "nm_atraso", width: "5%", styles: "min-width:40px; text-align: center;" },
	                    { name: "Valor", field: "vlTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Saldo", field: "vlSaldoTitulo", width: "10%", styles: "min-width:40px; text-align: right;" },
                        { name: "Dta.Pagto.", field: "dt_liquidacao", width: "10%", styles: "min-width:40px; text-align: left;" },
	                    { name: "Vl.Pagto.", field: "vlLiquidacaoBaixa", width: "15%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo", field: "dc_tipo_titulo", width: "5%", styles: "min-width:40px; text-align: right;" },
                        { name: "Tipo Baixa", field: "dc_tipo_liquidacao", width: "15%", styles: "min-width:40px; text-align: left;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["21", "42", "84", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "21",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridTituloHistT");
                gridTituloHistT.startup();

                gridTituloHistT.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 5 && Math.abs(col) != 10; };

                //*** Cria a grade de Atividades **\\
                var gridAtividade = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Tipo", field: "no_tipo_atividade_extra", width: "20%", styles: "min-width:40px; text-align: left;" },
                        { name: "Data", field: "dta_atividade_extra", width: "10%", styles: "min-width:40px; text-align: center;" },
                        { name: "H.Inicial", field: "hh_inicial", width: "8%", styles: "min-width:40px; text-align: center;" },
                        { name: "H.Final", field: "hh_final", width: "8%", styles: "min-width:40px; text-align: center;" },
                        { name: "Participou", field: "participou", width: "8%", styles: "min-width:40px; text-align: center;" },
                        { name: "Observação", field: "tx_obs_atividade", width: "50%", styles: "min-width:40px;" }
                        ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridAtividade");
                gridAtividade.startup();

                gridAtividade.canSort = function (col) { return Math.abs(col) != 5 && Math.abs(col) != 1; };

                var gridFollow = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Data/Hora", field: "dta_follow_up", width: "10%" },
                        { name: "Atendente", field: "no_usuario", width: "15%" },
                        { name: "Assunto", field: "_dc_assunto", width: "40%" },
                        { name: "Data Próximo Contato", field: "dta_proximo_contato", width: "10%" },
                        { name: "Ação", field: "dc_acao", width: "10%" },
                        ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridFollow");
                gridFollow.startup();
                gridFollow.canSort = function (col) { return Math.abs(col) != 2 && Math.abs(col) != 5; };
                gridFollow.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                               item = this.getItem(idx),
                               store = this.store;
                        dijit.byId("gridFollow").itemSelecionado = item;

                        if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
                            montaFollowUpFK(function () {
                                dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                                    configuraLayoutFollowUp(e);
                                });
                                populaFollowUpAluno(function () { preparaKeepValues(); });

                                montaFollowUpFKPersonalizadoAluno();
                            });
                        else
                            populaFollowUpAluno(function () { preparaKeepValues(); });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridFollow.itemSelecionado = [];
                //*** Cria a grade de Itens **\\
                var gridItens = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                        { name: "Data", field: "dta_emissao_movimento", width: "10%", styles: "min-width:40px; text-align: left;" },
                        { name: "Nr. Venda", field: "dc_nm_movimento", width: "10%", styles: "min-width:40px; text-align: center;" },
                        { name: "Itens", field: "dc_item_movimento", width: "70%", styles: "min-width:40px; text-align: center;" },
                        { name: "Valor", field: "vlr_liquido_item", width: "10%", styles: "min-width:40px; text-align: center;" }
                        ],
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridItens");
                gridItens.startup();

                gridItens.canSort = function (col) { return true; };
                
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        getHistoricoTurmas();
                    }
                }, "pesquisarEscola");
                decreaseBtn(document.getElementById("pesquisarEscola"), '32px');
                /*new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage'
                }, "relatorioHistorico");*/

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFk();
                                });
                            }
                            else
                                abrirAlunoFk();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesAluno");

                var buttonFkArray = ['pesAluno'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }

                // Realiza o tratamento do link proveniente de aluno e turma:
                var parametros = getParamterosURL();
                if (hasValue(parametros['cd_aluno']))
                    retornarAlunoHistoricoLink(parametros['cd_aluno'], parametros['cd_pessoa'], parametros['no_pessoa'], parametros['dta_cadastro']);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323043', '765px', '771px');
                        });
                }

                dijit.byId('tabContainer').resize();
				
				showCarregando();
                SetarAvalEstagio(0, ObjectStore, Memory);

                var menTabMariculas = new DropDownMenu({ style: "height: 25px" });
                var acaoEditarMatriculasTag = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarTabMatricula(xhr, ready, Memory, FilteringSelect, registry, Array, ObjectStore); }
                });
                menTabMariculas.addChild(acaoEditarMatriculasTag);

                var buttonFollow = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasTabMaricula",
                    dropDown: menTabMariculas,
                    id: "acoesRelacionadasTabMatrciula"
                });
                dom.byId("linkAcoesMatricula").appendChild(buttonFollow.domNode);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}


function SetarAvalEstagio(estagio) {
    try {
            var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;
            var cd_estagio = estagio.cd_estagio;

            if (hasValue(cd_aluno_pesquisado) && hasValue(cd_estagio)) {
                require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
                    function (xhr, ObjectStore, Memory) {
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/getMediasEstagioAvaliacaoAluno?cd_aluno=" + cd_aluno_pesquisado + "&cd_estagio=" + cd_estagio,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dados) {
                            try {
                                apresentaMensagem('apresentadorMensagem', dados);
                                dados = jQuery.parseJSON(dados).retorno;

                                //dijit.byId('gridEstagio').setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                                dijit.byId('gridAvaliacaoEstagio').setStore(new ObjectStore({ objectStore: new Memory({ data: dados.avaliacoesMedia }) }));
                                dijit.byId('gridAvaliacaoEstagioTurma').setStore(new ObjectStore({ objectStore: new Memory({ data: dados.avaliacoesTurma }) }));
                                dijit.byId('gridNotasEstagio').setStore(new ObjectStore({ objectStore: new Memory({ data: dados.avaliacoesNota }) }));

                                dijit.byId('vl_media_final').set('value', dados.vlMediaFinal);
                                //dijit.byId('vl_aproveitamento_total').set('value', dados.vlAproveitamentoTotal);
                                //dijit.byId('vl_media_parcial_e').set('value', dados.vlMediaParcial);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                    });
            }

    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoHistoricoLink(cd_aluno, cd_pessoa, no_pessoa, dta_cadastro) {
    try{
        dojo.byId('alunoPesq').value = decodeURIComponent(no_pessoa);
        dojo.byId('cd_aluno_pesq').value = cd_aluno;
        dojo.byId('cd_pessoa_pesq').value = cd_pessoa;

        //Pesquisa a última matrícula do aluno para preencher na tela:
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.get({
                url: Endereco() + "/api/secretaria/getMatriculaAluno?cd_aluno=" + cd_aluno,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try {
                    if (hasValue(dados) && hasValue(dados.retorno)) {
                        apresentaMensagem('apresentadorMensagem', dados);
                        dojo.byId('matricula').value = dados.retorno;
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoHistoricoFK() {
    try{
        var gridAlunos = null;
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else if (gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
            valido = false;
        }
        else {
            dojo.byId('alunoPesq').value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dojo.byId('cd_aluno_pesq').value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId('cd_pessoa_pesq').value = gridPesquisaAluno.itensSelecionados[0].cd_pessoa;
            var cd_aluno_pesq = dojo.byId('cd_aluno_pesq').value;
            //Pesquisa a última matrícula do aluno para preencher na tela:
            require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                xhr.get({
                    url: Endereco() + "/api/secretaria/getMatriculaAluno?cd_aluno=" + cd_aluno_pesq,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dados) {
                    try{
                        apresentaMensagem('apresentadorMensagem', dados);

                        dojo.byId('matricula').value = dados.retorno;
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
            });
        }

        if (!valido)
            return false;

        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limpaComponentes() {
    clearForm("formHistorico");

    require(["dojo/data/ObjectStore", "dojo/store/Memory", "dojo/ready"], function (ObjectStore, Memory, ready) {
        ready(function () {
			try{
				var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
				var gridTurmasAvaliacao = dijit.byId("gridTurmasAvaliacao");
				var gridAvaliacao = dijit.byId("gridAvaliacao");
				var gridAvaliacaoNota = dijit.byId("gridAvaliacaoNota");
				var gridAvaliacaoConc = dijit.byId("gridAvaliacaoConc");
				var gridEvento = dijit.byId("gridEvento");
				var gridTituloHist = dijit.byId("gridTituloHist");
				var gridTituloHistF = dijit.byId("gridTituloHistF");
				var gridTituloHistC = dijit.byId("gridTituloHistC");
				var gridTituloHistT = dijit.byId("gridTituloHistT");
				var gridAtividade = dijit.byId("gridAtividade");
				var gridFollow = dijit.byId("gridFollow");
				var gridItens = dijit.byId("gridItens");

				var gridEstagio = dijit.byId("gridEstagio");
				var gridAvaliacaoEstagio = dijit.byId("gridAvaliacaoEstagio");
				var gridAvaliacaoEstagioTurma = dijit.byId("gridAvaliacaoEstagioTurma");
				var gridNotasEstagio = dijit.byId("gridNotasEstagio");

				
				if(hasValue(gridPesquisaAluno))
					gridPesquisaAluno.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridTurmasAvaliacao))
					gridTurmasAvaliacao.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridAvaliacao))
					gridAvaliacao.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridAvaliacaoNota))
					gridAvaliacaoNota.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridAvaliacaoConc))
					gridAvaliacaoConc.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridEvento))
					gridEvento.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridTituloHist))
					gridTituloHist.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridTituloHistF))
					gridTituloHistF.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if(hasValue(gridTituloHistC))
					gridTituloHistC.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridTituloHistT))
				    gridTituloHistT.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridAtividade))
					gridAtividade.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridFollow))
				    gridFollow.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridItens))
				    gridItens.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));

				if (hasValue(gridEstagio))
                    gridEstagio.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridAvaliacaoEstagio))
                    gridAvaliacaoEstagio.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridAvaliacaoEstagioTurma))
                    gridAvaliacaoEstagioTurma.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
				if (hasValue(gridNotasEstagio))
                    gridNotasEstagio.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            }
			catch (e) {
				postGerarLog(e);
			}
		});
    });
}

function getHistoricoTurmas() {
    if (dijit.byId('formHistAlunoPesq').validate()) {
        require(["dojo/_base/xhr", "dijit/layout/ContentPane", "dojo/data/ItemFileWriteStore",
        "dijit/tree/ForestStoreModel",
        "dojox/grid/LazyTreeGrid"], function (xhr, ContentPane, ItemFileWriteStore, ForestStoreModel, LazyTreeGrid) {
            xhr.get({
                url: Endereco() + "/api/secretaria/getHistoricoTurmas?cd_aluno=" + dojo.byId('cd_aluno_pesq').value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try{
                    apresentaMensagem('apresentadorMensagem', dados);

                    limpaComponentes();

                    //Marca que já foi feito uma pesquisa:
                    dojo.byId('cd_aluno_pesquisado').value = dojo.byId('cd_aluno_pesq').value;
                    dojo.byId('cd_pessoa_pesquisada').value = dojo.byId('cd_pessoa_pesq').value;

                    dados = jQuery.parseJSON(dados).retorno;
                    //Seta a primeira aba como aberta:
                    var tabs = dijit.byId("tabContainer");
                    var pane = dijit.byId("tabHistoricoTurmas");
                    tabs.selectChild(pane);

                    if (hasValue(dados) && dados.length > 0) {
                        var tabProdutos = dijit.byId('tabProdutos');

                        //Remove todas as abas:
                        var childrens = tabProdutos.getChildren();
                        for (var i = 0; i < childrens.length; i++)
                            tabProdutos.removeChild(childrens[i]);

                        //Cria as abas dinamicamente:
                        for (var i = 0; i < dados.length; i++) {
                            if (hasValue(dijit.byId('tabHistorico' + i)))
                                dijit.byId('tabHistorico' + i).destroy();

                            var cp1 = new ContentPane({
                                id: 'tabHistorico'+i,
                                title: dados[i].no_produto,
                                content: '<div class="gridCadBasic" style="height:95%";>'+
                                         '  <div id="gridHistorico' + i + '" style="height: 100%; min-height: 200px"></div>' +
				                         '</div>'
                            });
                            tabProdutos.addChild(cp1);
                        }
                        setTimeout(function () { criaTreeViewHistoricoTurma(dados, ItemFileWriteStore, ForestStoreModel, LazyTreeGrid); }, 50);
                        //tabProdutos.startup();
                    }
                    else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgInfoNaoExisteHistorico);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
}

function montaObjetoHistoricoTurma(dados) {
    try{
        var retorno = new Array();
        var id = 0;
        dados = dados.Turma;
        for (var i = 0; i < dados.length; i++) {
            var ultimo_hist = dados[i].HistoricoAluno[dados[i].HistoricoAluno.length - 1];
            var turma = {
                no_turma: dados[i].no_turma, nm_seq: ultimo_hist.nm_sequencia, dt_historico: ultimo_hist.dtaHistorico, no_contrato: ultimo_hist.Contrato.nm_contrato,
                dc_situacao: ultimo_hist.situacao_historico, dc_tipo: ultimo_hist.tipo_movimento, dt_cadastro: ultimo_hist.dtaCadastro, no_usuario: ultimo_hist.SysUsuario.no_login, id: id, pai: null
            };
            id += 1;

            turma.children = new Array();
            for (var j = 0; j < dados[i].HistoricoAluno.length; j++){
                var hist = dados[i].HistoricoAluno[j];
                var historico = {
                    no_turma: '', nm_seq: hist.nm_sequencia, dt_historico: hist.dtaHistorico, no_contrato: hist.Contrato.nm_contrato,
                                  dc_situacao: hist.situacao_historico, dc_tipo: hist.tipo_movimento, dt_cadastro: hist.dtaCadastro, no_usuario: hist.SysUsuario.no_login, id: id, pai: turma.id};
                id += 1;
                turma.children.push(historico);
            }

            retorno.push(turma);
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criaTreeViewHistoricoTurma(dados, ItemFileWriteStore, ForestStoreModel, LazyTreeGrid) {
    try{
        var layout = [
            { name: 'Turma', field: 'no_turma', width: '45%' },
            { name: 'Seq.', field: 'nm_seq', width: '10%', styles: "text-align: center;" },
            { name: 'Data', field: 'dt_historico', width: '10%' },
            { name: 'Contrato', field: 'no_contrato', width: '10%' },
            { name: 'Situação', field: 'dc_situacao', width: '20%' },
            { name: 'Tipo', field: 'dc_tipo', width: '18%' },
            { name: 'Data/Hora', field: 'dt_cadastro', width: '17%' },
            { name: 'Usuário', field: 'no_usuario', width: '15%' },
            { name: '', field: 'id', width: '5%', styles: "display:none;" },
            { name: '', field: 'pai', width: '5%', styles: "display:none;" }
        ];

        var tabs = dijit.byId("tabProdutos");
        for (var i = 0; i < 1000 && hasValue(dados[i]); i++) {
            var dataHistoricoTurma = montaObjetoHistoricoTurma(dados[i]);
            var data = {
                identifier: 'id',
                label: 'no_Turma',
                items: dataHistoricoTurma
            };
            var store = new ItemFileWriteStore({ data: data });
            var model = new ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            //Cria e mostra a grade:
            if(!hasValue(dojo.byId('gridHistorico'+i)))
                break;
            if (hasValue(dijit.byId('gridHistorico' + i)))
                dijit.byId('gridHistorico' + i).destroy();

            var gridHistorico = new LazyTreeGrid({
                id: 'gridHistorico' + i,
                treeModel: model,
                structure: layout
            }, document.createElement('div'));
            dojo.byId("gridHistorico" + i).appendChild(gridHistorico.domNode);
            gridHistorico.on("StyleRow", function (row) {
                var item = gridHistorico.getItem(row.index);
                //if (eval(row.node.children[0].children[0].children[0].children[8].innerHTML == 1))
                //    row.customClasses += " YellowRow";
                if (row.node.children[0].children[0].children[0].children[9].innerHTML == "")
                    row.customClasses += " YellowRow";
            });
            gridHistorico.startup();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function atualizarTurmasAvaliacoes() {
    try {
        var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;
        
        if (hasValue(cd_aluno_pesquisado)) {
        showCarregando();
           require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
                    function (xhr, ObjectStore, Memory) {
                xhr.get({
                    url: Endereco() + "/api/coordenacao/getTurmasAvaliacoes?cd_aluno=" + cd_aluno_pesquisado + "&cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dados) {
                    try {
                        showCarregando();

                        apresentaMensagem('apresentadorMensagem', dados);
                        dados = jQuery.parseJSON(dados).retorno;
                        dijit.byId("gridTurmasAvaliacao").setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                    }
                    catch (er) {
                        

                        postGerarLog(er);
                    }
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', error);
                });
            });
        }
        
    }
    catch (e) {
        
        postGerarLog(e);
    }
}


function atualizarTurmasEstagios() {
    try {
        var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;

        if (hasValue(cd_aluno_pesquisado)) {
            showCarregando();
            require(["dojo/date", "dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
                function (date, xhr, ObjectStore, Memory) {
                    xhr.get({
                        url: Endereco() + "/api/coordenacao/getEstagiosHistoricoAluno?cd_aluno=" + cd_aluno_pesquisado,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (dados) {
                        try {
                                showCarregando();
                                apresentaMensagem('apresentadorMensagem', dados);
                                dados = jQuery.parseJSON(dados).retorno;
                                if (hasValue(dados)) {
                                    dados = verificaDatas(dados, date);
                                }
                                dijit.byId("gridEstagio").setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                                dijit.byId("gridEstagio").startup();
                            }
                            catch (er) {
                                postGerarLog(er);
                            }
                        },
                        function (error) {
                            showCarregando();
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificaDatas(dados, date){
    try {
        var dataDefault = new Date('0001-01-01T00:00:00');
        dados.forEach(function(value, index) {
            if (hasValue(value.primeira_aula) && date.compare(new Date(value.primeira_aula), dataDefault) == 0) {
                value.primeira_aula = null;
            }
            if (hasValue(value.ultima_aula) && date.compare(new Date(value.ultima_aula), dataDefault) == 0) {
                value.ultima_aula = null;
            }
        });
        return dados;

    } catch (e) {
        postGerarLog(e);
    }
}

function atualizarTitulos() {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;

    if (hasValue(cd_aluno_pesquisado)) {
        require(["dojo/store/JsonRest","dojo/data/ObjectStore","dojo/store/Cache","dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
                    var myStore = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/escola/getTitulosHistoricoAlunoGeral?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=3",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }
                            ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHist").setStore(dataStore);

                    var myStore = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/financeiro/getTitulosHistoricoAlunoGeral?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=4",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }
                            ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHistF").setStore(dataStore);

                    var myStore = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/escola/getTitulosHistoricoAlunoGeral?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=5",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }
                            ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHistC").setStore(dataStore);

                    var myStore = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/escola/getTitulosHistoricoAlunoGeral?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=0",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }
                            ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHistT").setStore(dataStore);
                }
                else {
                    var myStore = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/escola/getTitulosHistoricoAluno?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=3",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }
                            ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHist").setStore(dataStore);

                    var myStore = Cache(
                                JsonRest({
                                    target: Endereco() + "/api/financeiro/getTitulosHistoricoAluno?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=4",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }
                            ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHistF").setStore(dataStore);

                    var myStore = Cache(
                               JsonRest({
                                   target: Endereco() + "/api/escola/getTitulosHistoricoAluno?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=5",
                                   handleAs: "json",
                                   preventCache: true,
                                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                               }
                           ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHistC").setStore(dataStore);

                    var myStore = Cache(
                               JsonRest({
                                   target: Endereco() + "/api/escola/getTitulosHistoricoAluno?cd_pessoa=" + dojo.byId('cd_pessoa_pesquisada').value + "&cd_tipo=0",
                                   handleAs: "json",
                                   preventCache: true,
                                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                               }
                           ), Memory({}));
                    var dataStore = new ObjectStore({ objectStore: myStore });
                    dijit.byId("gridTituloHistT").setStore(dataStore);
                }
                dijit.byId('gridTituloHist').resize(true);
                dijit.byId('gridTituloHistF').resize(true);
                dijit.byId('gridTituloHistC').resize(true);
                dijit.byId('gridTituloHistT').resize(true);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function atualizarAtividadeExtra() {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;

    if (hasValue(cd_aluno_pesquisado)) {
        require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/coordenacao/getAtividadesAluno?cd_aluno=" + dojo.byId('cd_aluno_pesquisado').value,
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }
                        ), Memory({}));
                var dataStore = new ObjectStore({ objectStore: myStore });

                dijit.byId("gridAtividade").setStore(dataStore);
                dijit.byId('gridAtividade').resize(true);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function getMediasAvaliacaoAluno(turma) {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;
    var cd_turma = turma.cd_turma;

    require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
        function (xhr, ObjectStore, Memory) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getMediasAvaliacaoAluno?cd_aluno=" + cd_aluno_pesquisado + "&cd_turma=" + cd_turma,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try{
                    apresentaMensagem('apresentadorMensagem', dados);
                    dados = jQuery.parseJSON(dados).retorno;
                    dijit.byId("gridAvaliacaoNota").setStore(new ObjectStore({ objectStore: new Memory({ data: dados.avaliacoesNota }) }));
                    dijit.byId("gridAvaliacao").setStore(new ObjectStore({ objectStore: new Memory({ data: dados.avaliacoesMedia }) }));

                    dijit.byId('vl_media').set('value', dados.vlMediaFinal);
                    //dijit.byId('vl_total').set('value', dados.vlAproveitamentoTotal);
                    //dijit.byId('vl_media_parcial').set('value', dados.vlMediaParcial);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
}

function getConceitosAvaliacaoAluno(turma) {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;
    var cd_turma = turma.cd_turma;

    require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
        function (xhr, ObjectStore, Memory) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getConceitosAvaliacaoAluno?cd_aluno=" + cd_aluno_pesquisado + "&cd_turma=" + cd_turma,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try{
                    apresentaMensagem('apresentadorMensagem', dados);
                    dados = jQuery.parseJSON(dados).retorno;
                    dijit.byId("gridAvaliacaoConc").setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
}

function getEventosAvaliacaoAluno(turma) {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;
    var cd_turma = turma.cd_turma;

    require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
        function (xhr, ObjectStore, Memory) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getEventosAvaliacaoAluno?cd_aluno=" + cd_aluno_pesquisado + "&cd_turma=" + cd_turma,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try{
                    apresentaMensagem('apresentadorMensagem', dados);
                    dados = jQuery.parseJSON(dados).retorno;
                    dijit.byId("gridEvento").setStore(new ObjectStore({ objectStore: new Memory({ data: dados.listaAlunoEvento }) }));

                    dojo.byId('DescAulaAluno').value = hasValue(dados.ultimaAulaAluno) ? dados.ultimaAulaAluno.tx_obs_aula : '';
                    dojo.byId('DescAulaTurma').value = hasValue(dados.ultimaAulaTurma) ? dados.ultimaAulaTurma.tx_obs_aula : '';

                    dojo.byId('dt_Aluno').value = hasValue(dados.ultimaAulaAluno) ? dados.ultimaAulaAluno.dta_aula : '';
                    dojo.byId('hr_ultima_aula_aluno').value = hasValue(dados.ultimaAulaAluno) ? dados.ultimaAulaAluno.hr_cadastro_aula : '';
                    dojo.byId('dt_Turma').value = hasValue(dados.ultimaAulaTurma) ? dados.ultimaAulaTurma.dta_aula : '';
                    dojo.byId('hr_ultima_aula_turma').value = hasValue(dados.ultimaAulaTurma) ? dados.ultimaAulaTurma.hr_cadastro_aula : '';
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
}

function atualizarTabFollow() {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;
    if (hasValue(cd_aluno_pesquisado)) {
        require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/secretaria/getFollowAluno?cd_aluno=" + dojo.byId('cd_aluno_pesquisado').value,
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }
                        ), Memory({}));
                var dataStore = new ObjectStore({ objectStore: myStore });
                //storeFollowUp[indice]._dc_assunto = hasValue(dijit.byId("mensagemFollowUppartial").value) && dijit.byId("mensagemFollowUppartial").value.indexOf('<') < 0
                //&& dijit.byId("mensagemFollowUppartial").value.indexOf('>') < 0 ? dijit.byId("mensagemFollowUppartial").value : "...";
                dijit.byId("gridFollow").setStore(dataStore);
                dijit.byId('gridFollow').resize(true);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function atualizarTabItens() {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;

    if (hasValue(cd_aluno_pesquisado)) {
        require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try {
                var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/fiscal/getItensAluno?cd_pessoa=" + dojo.byId('cd_pessoa_pesq').value + "&cd_aluno=" + dojo.byId('cd_aluno_pesq').value,
                                            handleAs: "json",
                                            preventCache: true,
                                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                        }
                                    ), Memory({}));
                var dataStore = new ObjectStore({ objectStore: myStore });

                dijit.byId("gridItens").setStore(dataStore);
                dijit.byId('gridItens').resize(true);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function atualizarObservacaoAluno() {
    var cd_aluno_pesquisado = dojo.byId('cd_aluno_pesquisado').value;

    if (hasValue(cd_aluno_pesquisado)) {
        require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Memory"],
        function (xhr, ObjectStore, Memory) {
            xhr.get({
                url: Endereco() + "/api/secretaria/getObservacaoAluno?cd_aluno=" + cd_aluno_pesquisado,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    var response = JSON.parse(data);
                    dijit.byId('descObsAluno').set('value', response.retorno);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
}

function abrirAlunoFk() {
    try{
        limparPesquisaAlunoFK();
        dojo.byId('tipoRetornoAlunoFK').value = HISTORICOALUNO;
        //dijit.byId('gridPesquisaAluno').update();
        apresentaMensagem('apresentadorMensagem', null);
        pesquisarAlunoFK(true);
        dijit.byId("proAluno").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaFollowUpAluno(p_funcao) {
    try {
        //Personaliza a tela da FK de follow up para a tela de aluno:
        dijit.byId("cadTipoFollowUpFK").set("value", PROSPECTALUNO);
        showP('trTipoFollowUpFK', false);
        dojo.byId('trInternoUserFollowUpFK').style.display = 'none';
        dojo.byId('trInternoAdmFollowUpFK').style.display = 'none';
        dojo.byId('trProspectAlunoFollowUpFK').style.display = '';
        dojo.byId('trEmailTelefoneProspectAluno').style.display = '';
        showP('trProximoContatoFollowUpFK', true);
        dojo.byId('trMasterFollowUp').style.display = 'none';
        dojo.byId('trResolvidoLidoFollowUp').style.display = '';
        dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("required", false);
        dijit.byId("descAlunoProspectFollowUpFK").set("required", true);
        dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("required", false);
        dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", false);

        dijit.byId('btnIncluirFollowUpPartial').set('label', "Incluir");
        dijit.byId('alterarFollowUpPartial').set('label', "Alterar");

        dijit.byId('panePesqGeralFollowUpFK').set('title', "Aluno");
        alterarVisibilidadeEscolas(false);

        var btnAlunoProspectFK = dijit.byId("btnAlunoProspectFK");
        if (hasValue(btnAlunoProspectFK.handler))
            btnAlunoProspectFK.handler.remove();
        btnAlunoProspectFK.handler = btnAlunoProspectFK.on("click", function (e) {
            abrirFKProspectFollowUpPartialPersonalizadoAluno();
        });

        var btnIncluirFollowUpPartial = dijit.byId("btnIncluirFollowUpPartial");
        if (hasValue(btnIncluirFollowUpPartial.handler))
            btnIncluirFollowUpPartial.handler.remove();
        btnIncluirFollowUpPartial.handler = btnIncluirFollowUpPartial.on("click", function () {
            incluirLinhaFollowUp();
        });

        var alterarFollowUpPartial = dijit.byId("alterarFollowUpPartial");
        if (hasValue(alterarFollowUpPartial.handler))
            alterarFollowUpPartial.handler.remove();
        alterarFollowUpPartial.handler = alterarFollowUpPartial.on("click", function () {
            alterarLinhaFollowUp();
        });

        var deleteFollowUpPartial = dijit.byId("deleteFollowUpPartial");
        if (hasValue(deleteFollowUpPartial.handler))
            deleteFollowUpPartial.handler.remove();
        deleteFollowUpPartial.handler = deleteFollowUpPartial.on("click", function () {
            removerLinhaFollowUp();
        });

        if (hasValue(p_funcao))
            p_funcao.call();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function preparaKeepValues() {
    IncluirAlterar(0, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial', 'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
    dojo.byId("divAlterarFollowUpPartial").style.display = 'none';
    dojo.byId("divIncluirFollowUpPartial").style.display = 'none';
    dojo.byId("divExcluirFollowUpPartial").style.display = 'none';
    dojo.byId("divCancelarFollowUpPartial").style.display = 'none';
    dijit.byId("alterarFollowUpPartial").set("disabled", true);
    dijit.byId("btnIncluirFollowUpPartial").set("disabled",true);
    dijit.byId("deleteFollowUpPartial").set("disabled", true);
    dijit.byId("cancelarFollowUpPartial").set("disabled", true);
    limparCadFollowUpPartial();
    findComponentesNovoFollowUpPartial(function () {
        dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('alunoPesq').value;
        dijit.byId('btnAlunoProspectFK').set('disabled', true);
        dijit.byId('ckResolvidoFollowUpFK').set('disabled', false);
        showP('lblLido', true);
        dijit.byId('ckLidoFollowUpFK').set('style', 'display:block');

        dijit.byId("cadFollowUp").show();
        keepValuesFollowUp();
    });
}

function keepValuesFollowUp() {
    var CompckLidoFollowUpFK = dijit.byId('ckLidoFollowUpFK');
    var item = dijit.byId("gridFollow").itemSelecionado

    if (hasValue(item.dta_follow_up))
        dojo.byId('dtaCadastroFollowUpFK').value = item.dta_follow_up;

    dijit.byId('mensagemFollowUppartial').set('value', item.dc_assunto);
    dijit.byId('cadAcaoFollowUpFK').set('value', item.cd_acao_follow_up);
    dojo.byId('dtaProxContatoFollowUpFK').value = item.dta_proximo_contato;
    dojo.byId('nroOrdem').value = item.ordem;
    dojo.byId('cd_follow_up_partial').value = item.cd_follow_up;

    CompckLidoFollowUpFK._onChangeActive = false;
    CompckLidoFollowUpFK.set("value", item.id_follow_lido);
    CompckLidoFollowUpFK._onChangeActive = true;
    dijit.byId('ckResolvidoFollowUpFK').set('checked', item.id_follow_resolvido);
    configuraLayoutFollowUp(item.id_follow_resolvido);
}

function configuraLayoutFollowUp(desabilitado) {
    if (desabilitado) {
        dijit.byId('cadAcaoFollowUpFK').set('disabled', true);
        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', true);
        dijit.byId('mensagemFollowUppartial').set('disabled', true);

        dijit.byId('ckLidoFollowUpFK').set('checked', true);
    }
    else {
        dijit.byId('cadAcaoFollowUpFK').set('disabled', false);
        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', false);
        dijit.byId('mensagemFollowUppartial').set('disabled', false);
    }
}

function montaFollowUpFKPersonalizadoAluno() {
    dijit.byId('cancelarFollowUpPartial').on('click', function () {
        populaFollowUpAluno(function () { preparaKeepValues(); });
    });
    dijit.byId('btnLimparFollowUpFK').on('click', function () {
        dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomPessoa').value;
        dijit.byId('btnAlunoProspectFK').set('disabled', true);
    });
    dijit.byId('btnFecharFollowUpPartial').on('click', function () {
        dijit.byId('cadFollowUp').hide();
    });
}


function criarGridTabMatriculas(cdAluno, dataTabMatriculas) {
    try {
        var dataStoreTabMat = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null, idProperty: "cd_contrato" }) });
        var gridTabMatricula = new dojox.grid.EnhancedGrid({
            store: dataStoreTabMat,
            structure:
                [
                    {
                        name: "<input id='selecionaTodosTabMatriculas'  style='display:none' />", field: "selecionadoTabMatricula", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;",
                        formatter: formatCheckBoxTabMatricula
                    },
                    { name: "Data", field: "dtMatriculaContrato", width: "15%" },
                    { name: "Contrato", field: "nm_contrato", width: "20%" },
                    { name: "Curso", field: "no_curso", width: "20%" },
                    { name: "Produto", field: "no_produto", width: "20%" },
                    { name: "Ano Escolar", field: "dc_ano_escolar", width: "25%" }
                ],
            canSort: true,
            noDataMessage: "Nenhum registro encontrado."
        }, "gridTabMatriculas");
        gridTabMatricula.startup();
        gridTabMatricula.itensSelecionados = new Array();
        //gridTabMatricula.pagination.plugin._paginator.plugin.connect(gridTabMatricula.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
        //    verificaMostrarTodos(evt, gridTabMatricula, ' cd_contrato', 'selecionaTodosTabMatriculas');
        //});
        //require(["dojo/aspect"], function (aspect) {
        //    aspect.after(gridTabMatricula, "_onFetchComplete", function () {
        //        // Configura o check de todos:
        //        if (dojo.byId('selecionaTodosTabMatriculas').type == 'text')
        //            setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionadoTabMatricula', -1, 'selecionaTodosTabMatriculas', 'selecionaTodosTabMatriculas', 'gridTabMatriculas')",
        //                gridTabMatricula.rowsPerPage * 3);
        //    });
        //});

        gridTabMatricula.canSort = function () { return false };
        gridTabMatricula.startup();
        gridTabMatricula.rowsPerPage = 5000;
        if (cdAluno != null && cdAluno > 0)
            dojo.xhr.get({
                preventCache: true,
                handleAs: "json",
                url: Endereco() + "/api/secretaria/getMatriculasAluno?cd_aluno=" + parseInt(cdAluno),
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataTabMat) {
                try {
                    dataTabMat = jQuery.parseJSON(dataTabMat);
                    dataTabMat = dataTabMat.retorno;
                    var dataStoreTabMat = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataTabMat, idProperty: "cd_contrato" }) });

                    if (hasValue(dataStoreTabMat))
                        gridTabMatricula.setStore(dataStoreTabMat);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        else if (hasValue(dataTabMatriculas))
            gridTabMatricula.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataProspect, idProperty: "cd_contrato" }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTabMatricula(value, rowIndex, obj) {
    try {
        var gridName = 'gridTabMatriculas'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTabMatriculas');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_contrato", grid._by_idx[rowIndex].item.cd_contrato);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionadoTabMatricula', -1, 'selecionaTodosTabMatriculas', 'selecionaTodosTabMatriculas', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_contrato', 'selecionadoTabMatricula', " + rowIndex + ", '" + id + "', 'selecionaTodosTabMatriculas', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function eventoEditarTabMatricula(xhr, ready, Memory, FilteringSelect, registry, Array, ObjectStore) {
    try {
        var gridTabMatricula = dijit.byId("gridTabMatriculas");
        var itensSelecionados = gridTabMatricula.itensSelecionados;
        apresentaMensagem('apresentadorMensagemAluno', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {

            var gridMatricula = dijit.byId('gridMatricula');
            apresentaMensagem('apresentadorMensagemAluno', null);
            if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                montarCadastroMatriculaPartial(function () {
                    dijit.byId("alterarMatricula").on("click", function () {
                        //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                        //setTimeout(function () { alterarMatriculaALuno(); }, 10);
                        alterarMatricula();
                    });
                    abrirMatriculaAluno(xhr, ready, Memory, FilteringSelect, ObjectStore, itensSelecionados);
                }, Permissoes);
            } else
                abrirMatriculaAluno(xhr, ready, Memory, FilteringSelect, ObjectStore, itensSelecionados);
        }
    }
    catch (e) {
        postGerarLog(e);
    }

}


function abrirMatriculaAluno(xhr, ready, Memory, FilteringSelect, ObjectStore, itensSelecionados) {
    showCarregando();
    limparCadMatricula(xhr, ObjectStore, Memory, false);
    setarTabCadMatricula(true);
    dijit.byId('tabContainerMatricula').resize();
    hideTagMatriculaTurma();
    dijit.byId("cadMatricula").show();
    dijit.byId('tabContainerMatricula').resize();
    IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');
    var gridTabMatricula = dijit.byId("gridTabMatriculas");
    gridTabMatricula.itemSelecionado = itensSelecionados[0];
    habilitarOnChange("ckAula", false);
    keepValuesMatricula(null, dijit.byId('gridTabMatriculas'), true, xhr, ready, Memory, FilteringSelect, ObjectStore);
    habilitarOnChange("ckAula", true);
}


function salvarAlteracaoMatrciulaAluno(xhr, ref) {
    try {

        var files = dijit.byId("uploaderContratoDigitalizado")._files;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("file" + i, files[i]);
                }
                $.ajax({
                    type: "POST",
                    url: Endereco() + "/secretaria/UploadDocumentoDigitalizado",
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (results) {
                        try {
                            if (hasValue(results) && hasValue(results.indexOf) && results.indexOf('<') >= 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSessaoExpirada3);
                                apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
                                return false;
                            }
                            if (hasValue(results) && !hasValue(results.erro)) {
                                console.log("Salvou arquivo dig temporario");

                                /*Salva o contrato com arquivo digitalizado*/
                                var contrato = new montarContrato(results);

                                xhr.post({
                                    url: Endereco() + "/api/escola/PostAlterarMatriculaALuno",
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                    postData: ref.toJson(contrato)
                                }).then(function (data) {
                                    try {
                                        if (!hasValue(jQuery.parseJSON(data).erro)) {
                                            var itemAlterado = jQuery.parseJSON(data).retorno;
                                            var gridName = 'gridTabMatriculas';
                                            var grid = dijit.byId(gridName);
                                            apresentaMensagem("apresentadorMensagemAluno", data);
                                            dijit.byId("cadMatricula").hide();
                                            if (!hasValue(grid.itensSelecionados) && grid._by_idx.length > 0)
                                                grid.itensSelecionados = [];
                                            grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemAlterado, idProperty: "cd_contrato" }) }));
                                        } else
                                            apresentaMensagem('apresentadorMensagemMat', data);
                                        showCarregando();
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                },
                                    function (error) {
                                        showCarregando();
                                        apresentaMensagem('apresentadorMensagemMat', error);
                                    });
                                /*Fim Salvar o contrato com arquivo digitalizado*/

                            } else
                                apresentaMensagem('apresentadorMensagemMat', results);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    error: function (error) {
                        apresentaMensagem('apresentadorMensagemMat', error);
                        return false;
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
            }
        } else {

            /*Salva o contrato sem arquivo digitalizado*/
            var contrato = new montarContrato(results);

            xhr.post({
                url: Endereco() + "/api/escola/PostAlterarMatriculaALuno",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(contrato)
            }).then(function (data) {
                try {
                    if (!hasValue(jQuery.parseJSON(data).erro)) {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridTabMatriculas';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem("apresentadorMensagemAluno", data);
                        dijit.byId("cadMatricula").hide();
                        if (!hasValue(grid.itensSelecionados) && grid._by_idx.length > 0)
                            grid.itensSelecionados = [];
                        grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemAlterado, idProperty: "cd_contrato" }) }));
                    } else
                        apresentaMensagem('apresentadorMensagemMat', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemMat', error);
                });
            /*Fim Salvar o contrato sem arquivo digitalizado*/

        }

    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridTabMatricula() {
    if (hasValue(dijit.byId("gridTabMatriculas"))) {
        dijit.byId("gridTabMatriculas").destroyRecursive();
        $('<div>').attr('id', 'gridTabMatriculas').appendTo('#idPanelMaricula');
    }
}