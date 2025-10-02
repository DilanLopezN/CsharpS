var SAIDA = 2;
var origem = 0;
var MOVIMENTO_DEVOLUCAO = 15;
var ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL = 1, ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL = 2;
function montarGridPesquisaMovtoFK(funcao) {
    require([
       "dojo/_base/xhr",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/data/ObjectStore",
       "dojo/store/Memory",
       "dijit/form/Button",
       "dojo/ready",
       "dojo/on",
       "dijit/form/FilteringSelect",
       "dojox/charting/Chart",
       "dojox/charting/plot2d/Columns",
       "dojox/charting/axis2d/Default",
       "dojox/charting/widget/Legend",
       "dojox/charting/themes/MiamiNice",
       "dojo/_base/array"
    ], function (xhr, EnhancedGrid, Pagination, ObjectStore, Memory, Button, ready, on, FilteringSelect, Chart,
                 Columns, Default, Legend, MiamiNice, array) {
        ready(function () {
            try {
                apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                var store = null;
                store = new ObjectStore({ objectStore: new Memory({ data: null }) });

                if (hasValue(dijit.byId("gridPesquisaMovtoFK"), true))
                    return false;
                var gridPesquisaMovtoFK = new EnhancedGrid({
                    store: store,
                    structure: [
                        { name: "<input id='selecionaTodosMvtoFK' style='display:none'/>", field: "selecionadoMovtoFK", width: "20px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMovFK },
                        { name: "Número", field: "dc_numero_serie", width: "15%", styles: "min-width:80px;" },
                        { name: "Pessoa", field: "no_pessoa", width: "22%", styles: "min-width:80px;" },
                        { name: "Emissão", field: "dta_emissao_movimento", width: "10%", styles: "min-width:80px;" },
                        { name: "Natureza", field: "dc_tipo_movto", width: "10%", styles: "min-width:80px;" },
                        { name: "Movimento", field: "dta_mov_movimento", width: "10%", styles: "min-width:80px;" },
                        { name: "Tipo Financeiro", field: "dc_tipo_financeiro", width: "13%", styles: "min-width:80px;" },
                        { name: "Política Comercial", field: "dc_politica_comercial", width: "10%", styles: "min-width:80px;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["10", "20", "40", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "10",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridPesquisaMovtoFK");
                gridPesquisaMovtoFK.rowsPerPage = 5000;
                gridPesquisaMovtoFK.pagination.plugin._paginator.plugin.connect(gridPesquisaMovtoFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaMovtoFK, 'cd_movimento', 'selecionaTodosMvtoFK');
                });
                gridPesquisaMovtoFK.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 && Math.abs(col) != 10; };
                gridPesquisaMovtoFK.on("RowDblClick", function (evt) {
                });
                gridPesquisaMovtoFK.startup();
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            var tipo = parseInt(dojo.byId('tipoRetornoMovimentoFK').value);
                            switch (tipo) {
                                case ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL:
                                {
                                    retornaFKMovto(ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL);
                                    break;
                                }
                                case ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL:
                                {
                                    retornaFKMovto(ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL);
                                    break;
                                }

                                default: retornaFKMovto();
                                    break;
                            }

                            

                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaMovimentoFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("proMvtoFK").hide(); }
                }, "fecharMovimentoFK");
                new Button({
                    label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                        try {
                            limparFiltrosTurmaFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAlunoPesFKMovimentoFK");
                dijit.byId("pesquisarMovimentoFK").on("click", function (e) {
                    var tipoPesquisa = parseInt(dojo.byId('tipoPesquisaMovimentoFK').value);
                    apresentaMensagem('apresentadorMensagem', null);
                    if (typeof VINCULA_MATERIAL_FILTRO != 'undefined' && VINCULA_MATERIAL_FILTRO != null) {
                            pesquisarMovimentoFKVincularMaterial
                    } else if (tipoPesquisa == ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL) {
                        
                        pesquisarMovimentoFKPerdaMaterial(true, ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL, SAIDA);
                    } else if (tipoPesquisa == ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL) {
                        pesquisarMovimentoFKPerdaMaterial(true, ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL, SAIDA);
                    }else {
                        pesquisarMovimentoFK(true, false);
                    }
                   
                });
                decreaseBtn(document.getElementById("pesquisarMovimentoFK"), '32px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            origem = MOVIMENTO_DEVOLUCAO;
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    try {
                                        abrirPessoaFK(false);
                                        dojo.query("#_nomePessoaFK").on("keyup", function (e) { if (e.keyCode == 13) pesquisaPessoaFKMovimento(true); });
                                        dijit.byId("pesqPessoa").on("click", function (e) {
                                            consultarPessoaMovimento();
                                        });
                                        dijit.byId("fecharFK").on("click", function (e) {
                                            dijit.byId("fkPessoaPesq").hide();
                                        });
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                });
                            else
                                abrirPessoaFK(false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPessoaPesqFK");
                decreaseBtn(document.getElementById("pesPessoaPesqFK"), '18px');

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try {
                            dojo.byId('noPessoaPesqFK').value = '';
                            dojo.byId('cdPessoaPesqFK').value = 0;
                            dijit.byId("limparPessoaRelPosPesFK").set('disabled', true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparPessoaRelPosPesFK");

                decreaseBtn(document.getElementById("limparPessoaRelPosPesFK"), '40px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            origem = MOVIMENTO_DEVOLUCAO;
                            if (dojo.byId("tipoPesquisaFKItem") != null && dojo.byId("tipoPesquisaFKItem") != undefined) {
                                dojo.byId("tipoPesquisaFKItem").value = MOVIMENTO_DEVOLUCAO;
                            }
                            
                            if (!hasValue(dijit.byId("gridPesquisaItem")))
                                montargridPesquisaItem(function () {
                                    try {
                                        dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                        limparPesquisaCursoFK(false);
                                        dijit.byId("tipo").reset();
                                        chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready);
                                        dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready); });
                                        dijit.byId("pesquisarItemFK").on("click", function (e) {
                                            try {
                                                apresentaMensagem("apresentadorMensagemItemFK", null);
                                                var tipoPesquisaFKItem = dojo.byId("tipoPesquisaFKItem");
                                                if (tipoPesquisaFKItem != undefined && tipoPesquisaFKItem != null && hasValue(tipoPesquisaFKItem.value)) {
                                                    chamarPesquisaItemFK(tipoPesquisaFKItem.value, xhr, Memory, FilteringSelect, array, ready);
                                                } else { chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready);}
                                                    
                                            }
                                            catch (e) {
                                                postGerarLog(e);
                                            }
                                        });
                                    }
                                    catch (e) {
                                        postGerarLog(e);
                                    }
                                }, false, true);
                            else {
                                limparPesquisaCursoFK(false);
                                dijit.byId("tipo").reset();
                                chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesItemPesqFK");
                decreaseBtn(document.getElementById("pesItemPesqFK"), '18px');
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                        dojo.byId('noItemPesqFK').value = "";
                        dojo.byId('cdItemFK').value = 0;
                        dijit.byId("limparItemFK").set('disabled', true);
                    }
                }, "limparItemFK");
                decreaseBtn(document.getElementById("limparItemFK"), '40px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            TIPO_PESQUISA = MOVIMENTO_DEVOLUCAO;
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagem',
                                    MOVIMENTO_DEVOLUCAO, MOVIMENTO_DEVOLUCAO);
                            }
                            else
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, MOVIMENTO_DEVOLUCAO);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPlanoPesqFK");
                decreaseBtn(document.getElementById("pesPlanoPesqFK"), '18px');
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId('noPlanoContaPesqFK').value = "";
                        dojo.byId('cdPlanoContaPesqFK').value = 0;
                        dijit.byId("limparPlanoContaPesqFK").set('disabled', true);
                    }
                }, "limparPlanoContaPesqFK");
                decreaseBtn(document.getElementById("limparPlanoContaPesqFK"), '40px');
                adicionarAtalhoPesquisa(['numeroPesqFK', 'numeroSeriePesqFK', 'dtaInicialFK', 'dtaFinalFK'], 'pesquisarTurmaFK', ready);


                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            abrirFKAlunoMovimentoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKAlunoFKMovimento");
                new Button({
                    label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                        try {
                            dojo.byId('FKAlunoFKMovimento').value = 0;
                            dojo.byId("txAlunoFKMovimento").value = "";
                            dijit.byId('limparAlunoFKMovimento').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAlunoFKMovimento");
                if (hasValue(document.getElementById("limparAlunoFKMovimento"))) {
                    document.getElementById("limparAlunoFKMovimento").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAlunoFKMovimento").parentNode.style.width = '40px';
                }
                var buttonFkArrayAluno = ['FKAlunoFKMovimento'];

                diminuirBotoes(buttonFkArrayAluno);

                loadNatureza(Memory);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadNatureza(Memory) {
    dojo.ready(function () {
        try {
            var stateStore = new Memory({
                data: [
                    { name: "Todas", id: 0 },
                    { name: "Entrada", id: 1 },
                    { name: "Saída", id: 2 },
                    { name: "Despesa", id: 3 },
                    { name: "Serviço", id: 4 }
                ]
            });

            if (dijit.byId("naturezaNF") != null) {
                dijit.byId("naturezaNF").store = stateStore;
                dijit.byId("naturezaNF").set("value", 0);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function formatCheckBoxMovFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaMovtoFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMvtoFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_movimento", grid._by_idx[rowIndex].item.cd_movimento);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_movimento', 'selecionadoMovtoFK', -1, 'selecionaTodosMvtoFK', 'selecionaTodosMvtoFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_movimento', 'selecionadoMovtoFK', " + rowIndex + ", '" + id + "', 'selecionaTodosMvtoFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarMovimentoFK(limparItens, isMovimento) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory",
          "dojo/domReady!",
          "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            
            var anoInicial = new Date(Date.now()).getFullYear();
            var dtInicial = new Date(anoInicial, 00, 01);
            var strData = dojo.date.locale.format(dtInicial, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
            var grid = dijit.byId("gridPesquisaMovtoFK");
            //var idNatMvot = (dojo.byId('id_natureza_movto') != null && dojo.byId('id_natureza_movto') != undefined && hasValue(dojo.byId('id_natureza_movto').value)) ? dojo.byId('id_natureza_movto').value : 0;
            var idNatMvot = (dijit.byId('naturezaNF').value != null && dijit.byId('naturezaNF') != undefined && hasValue(dijit.byId('naturezaNF').value)) ? dijit.byId('naturezaNF').value : 0;
            var cdPessoaPesq = hasValue(dojo.byId("cdPessoaPesqFK").value) ? dojo.byId("cdPessoaPesqFK").value : 0;
            var cdItem = hasValue(dojo.byId("cdItemFK").value) ? dojo.byId("cdItemFK").value : 0;
            var cdPlanoContaPesq = hasValue(dojo.byId("cdPlanoContaPesqFK").value) ? dojo.byId("cdPlanoContaPesqFK").value : 0;
            var numeroPesq = hasValue(dojo.byId("numeroPesqFK").value) ? dojo.byId("numeroPesqFK").value : 0;
            var serie = hasValue(dojo.byId("numeroSeriePesqFK").value) ? dojo.byId("numeroSeriePesqFK").value : "";
            if (!dijit.byId('ckEmissaoFK').checked && !dijit.byId('ckMovimentoFK').checked)
                dijit.byId('ckMovimentoFK').set("checked", true);
            if (hasValue(dijit.byId('ckNotaFiscal'))) {
                idNf = dijit.byId('ckNotaFiscal').checked;
            } else {
                idNf = false;
            }
            isMovimento = false; //Alterado a regra pois já estão sendo passaos alguns filtros iniciais
            if (!hasValue(dojo.byId("dtaInicialFK").value))
            dojo.byId("dtaInicialFK").value = strData;

            var myStore =
                Cache(
                        JsonRest({
                            target: Endereco() + "/api/fiscal/getMovimentoSearchFK?cd_pessoa=" + parseInt(cdPessoaPesq) +
                            "&cd_item=" + parseInt(cdItem) + "&cd_plano_conta=" + parseInt(cdPlanoContaPesq) + "&numero=" + parseInt(numeroPesq) + "&serie=" + serie +
                            "&emissao=" + document.getElementById("ckEmissaoFK").checked + "&movimento=" + document.getElementById("ckMovimentoFK").checked +
                            "&dtInicial=" + dojo.byId("dtaInicialFK").value + "&dtFinal=" + dojo.byId("dtaFinalFK").value + "&natMovto=" + idNatMvot +
                            "&idNf=" + idNf,
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));
            if (isMovimento) {
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                grid.noDataMessage = "Favor selecionar um filtro para executar a consulta...";
            }
            else { 
                var dataStore = new ObjectStore({ objectStore: myStore });
                grid.noDataMessage = "Nenhum registro encontrado com os filtros selecionados...";
            }
            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarMovimentoFKPerdaMaterial(limparItens, origem, natureza) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/domReady!",
        "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            
            var anoInicial = new Date(Date.now()).getFullYear();
            var dtInicial = new Date(anoInicial, 00, 01);
            var strData = dojo.date.locale.format(dtInicial, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
            var grid = dijit.byId("gridPesquisaMovtoFK");
            //var idNatMvot = (dojo.byId('id_natureza_movto') != null && dojo.byId('id_natureza_movto') != undefined && hasValue(dojo.byId('id_natureza_movto').value)) ? dojo.byId('id_natureza_movto').value : 0;
            var idNatMvot = natureza ;
            var cdPessoaPesq = hasValue(dojo.byId("cdPessoaPesqFK").value) ? dojo.byId("cdPessoaPesqFK").value : 0;
            var cdItem = hasValue(dojo.byId("cdItemFK").value) ? dojo.byId("cdItemFK").value : 0;
            var cdPlanoContaPesq = hasValue(dojo.byId("cdPlanoContaPesqFK").value) ? dojo.byId("cdPlanoContaPesqFK").value : 0;
            var numeroPesq = hasValue(dojo.byId("numeroPesqFK").value) ? dojo.byId("numeroPesqFK").value : 0;
            var serie = hasValue(dojo.byId("numeroSeriePesqFK").value) ? dojo.byId("numeroSeriePesqFK").value : "";
            
            if (hasValue(dijit.byId('ckNotaFiscalPerdaMaterialFK'))) {
                idNf = dijit.byId('ckNotaFiscalPerdaMaterialFK').checked;
            } else {
                idNf = false;
            }
            isMovimento = false; //Alterado a regra pois já estão sendo passaos alguns filtros iniciais
            if (!hasValue(dojo.byId("dtaInicialFK").value))
                dojo.byId("dtaInicialFK").value = strData;

            strAlunoContrato = ""
            if (origem == ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL) {
                cdAluno = hasValue(dojo.byId("cdAlunoFKMovimento").value)
                    ? dojo.byId("cdAlunoFKMovimento").value
                    : 0;
                nmContrato = hasValue(dijit.byId("nmContratoFKMovimento").value)
                    ? dijit.byId("nmContratoFKMovimento").value
                    : 0;
                strAlunoContrato = "&cd_aluno=" + cdAluno + "&nm_contrato=" + nmContrato;
            } else {
                strAlunoContrato = "&cd_aluno=&nm_contrato=";
            }

            

            var myStore =
                Cache(
                    JsonRest({
                        target: Endereco() + "/api/fiscal/getMovimentoSearchFKPerdaMaterial?cd_pessoa=" + parseInt(cdPessoaPesq) +
                            "&cd_item=" + parseInt(cdItem) + "&cd_plano_conta=" + parseInt(cdPlanoContaPesq) + "&numero=" + parseInt(numeroPesq) + "&serie=" + serie +
                            "&emissao=" + document.getElementById("ckEmissaoFK").checked + "&movimento=" + document.getElementById("ckMovimentoFK").checked +
                            "&dtInicial=" + dojo.byId("dtaInicialFK").value + "&dtFinal=" + dojo.byId("dtaFinalFK").value + "&natMovto=" + idNatMvot +
                            "&idNf=" + idNf + "&origem=" + origem + strAlunoContrato,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparFiltrosTurmaFK() {
    try {
        dojo.byId('noPessoaPesqFK').value = '';
        dojo.byId('cdPessoaPesqFK').value = 0;
        dijit.byId("limparPessoaRelPosPesFK").set('disabled', true);
        dojo.byId('noItemPesqFK').value = "";
        dojo.byId('cdItemFK').value = 0;
        dijit.byId("limparItemFK").set('disabled', true);
        dojo.byId('noPlanoContaPesqFK').value = "";
        dojo.byId('cdPlanoContaPesqFK').value = 0;
        dijit.byId("limparPlanoContaPesqFK").set('disabled', true);
        dijit.byId('numeroPesqFK').set("value", "");
        dijit.byId('numeroSeriePesqFK').set("value", "");
        dijit.byId('dtaInicialFK').set("value", null);
        dijit.byId('dtaFinalFK').set("value", null);
        dijit.byId('ckEmissaoFK').set("checked", false);
        dijit.byId('ckMovimentoFK').set("checked", true);
        if (hasValue(dijit.byId("gridPesquisaMovtoFK")) && hasValue(dijit.byId("gridPesquisaMovtoFK").itensSelecionados))
            dijit.byId("gridPesquisaMovtoFK").itensSelecionados = [];

    }
    catch (e) {
        postGerarLog(e);
    }
}
