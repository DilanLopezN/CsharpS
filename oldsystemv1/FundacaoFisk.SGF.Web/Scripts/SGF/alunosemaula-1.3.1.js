var PRIMEIRA = true;
function selecionaTab(e) {
    try {
        if (dijit.byId('tabContainer').selectedChildWidget.id == 'tgGeradas') {
            if (PRIMEIRA) {
                dijit.byId('gridNotasDevolvidas').startup();
                PRIMEIRA = false;
            }
            dijit.byId('gridNotasDevolvidas').resize();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAlunosemAula() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojox/json/ref",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojo/dom"
    ], function (ready, xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom) {
        ready(function () {
            try {
                var myStore =
                        Cache(
                                JsonRest({
                                    target: Endereco() + "/api/aluno/getAlunosemAula?cd_aluno=" + parseInt(0) + "&cd_item=" + parseInt(0) +
                                                        "&idEscola=false&dtInicial=&dtFinal=&idMovimento=false&idHistorico=false&idSituacao=0",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json",  "Authorization": Token() }
                                }), Memory({})
                        );

                var gridAlunosemAula = new EnhancedGrid({
                    //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        {
                            name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "15px", styles: "text-align:center; min-width:15px; max-width:20px;",
                            formatter: formatCheckBox
                        },
                        { name: "Escola", field: "dc_reduzido_pessoa", width: "15%", styles: "min-width:80px;" },
                        { name: "Aluno", field: "no_pessoa", width: "16%", styles: "min-width:80px;" },
                        { name: "Movimento", field: "nm_movimento", width: "7%", styles: "text-align: center;" },
                        { name: "Emissão", field: "dta_movimento", width: "8%", styles: "min-width:80px; text-align: center;" },
                        { name: "Livro", field: "no_item", width: "13%", styles: "min-width:80px;" },
                        { name: "Turma", field: "no_turma", width: "22%" },
                        { name: "Movimentação", field: "Movimentacao", width: "9%", styles: "min-width:80px;" },
                        { name: "Historico", field: "dta_historico", width: "7%", styles: "min-width:80px;" }
                    ],
                    noDataMessage: msgNotRegEncFiltro,
                    canSort: true,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "32", "64", "100", "All"],
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
                }, "gridAlunosemAula");
                gridAlunosemAula.startup();
                gridAlunosemAula.itenSelecionado = null;
                gridAlunosemAula.pagination.plugin._paginator.plugin.connect(gridAlunosemAula.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try{
                        verificaMostrarTodos(evt, gridAlunosemAula, 'cd_item_movimento', 'selecionaTodos');
                    }catch (e) {
                        postGerarLog(e);
                    }
                });

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAlunosemAula, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_item_movimento', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridAlunosemAula')", gridAlunosemAula.rowsPerPage * 3);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridAlunosemAula.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9  };

                var myStoreN =
                    Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getNotasDevolvidas?cd_aluno=0" + "&cd_item=0" +
                                "&idEscola=false&dtInicial=&dtFinal=&idMovimento=false&idHistorico=false&idSituacao=0",
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({})
                    );

                var gridNotasDevolvidas = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStoreN }),
                    structure: [
                        //{
                        //    name: "<input id='selecionaTodosN' style='display:none'/>", field: "selecionadoN", width: "15px", styles: "text-align:center; min-width:15px; max-width:20px;",
                        //    formatter: formatCheckBoxNota
                        //},
                        { name: "Escola", field: "dc_reduzido_pessoa", width: "15%", styles: "min-width:80px;" },
                        { name: "Aluno", field: "no_pessoa", width: "27%", styles: "min-width:80px;" },
                        { name: "RAF", field: "nm_raf", width: "8%", styles: "min-width:80px;" },
                        { name: "Dt.Movimento", field: "dta_historico", width: "8%", styles: "min-width:80px;" },
                        { name: "Movimento", field: "nm_movimento", width: "8%", styles: "text-align: center;" },
                        { name: "Livro", field: "no_item", width: "24%", styles: "min-width:80px;" },
                        { name: "Dt.Devolução", field: "dta_movimento", width: "8%", styles: "min-width:80px; text-align: center;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    canSort: true,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "32", "64", "100", "All"],
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
                }, "gridNotasDevolvidas");
                //gridNotasDevolvidas.startup();
                //gridNotasDevolvidas.itenSelecionado = null;
                //gridNotasDevolvidas.pagination.plugin._paginator.plugin.connect(gridNotasDevolvidas.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                //    try {
                //        verificaMostrarTodos(evt, gridNotasDevolvidas, 'cd_item_movimento', 'selecionaTodosN');
                //    } catch (e) {
                //        postGerarLog(e);
                //    }
                //});

                //require(["dojo/aspect"], function (aspect) {
                //    aspect.after(gridNotasDevolvidas, "_onFetchComplete", function () {
                //        try {
                //            // Configura o check de todos:
                //            if (hasValue(dojo.byId('selecionaTodosN')) && dojo.byId('selecionaTodosN').type == 'text')
                //                setTimeout("configuraCheckBox(false, 'cd_item_movimento', 'selecionadoN', -1, 'selecionaTodosN', 'selecionaTodosN', 'gridNotasDevolvidas')", gridNotasDevolvidas.rowsPerPage * 3);
                //        } catch (e) {
                //            postGerarLog(e);
                //        }
                //    });
                //});
                gridNotasDevolvidas.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 };


                var statusHistorico = new Memory({
                    data: [
                        { name: "Todos", id: "0" },
                        { name: "Desistencia", id: "2" },
                        { name: "Encerramento", id: "4" }
                    ]
                });
                dijit.byId("cbHistorico").store = statusHistorico;
                dijit.byId("cbHistorico").set("value", 0);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK();
                                });
                            }
                            else
                                abrirAlunoFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKAluno");
                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        if (dijit.byId('tabContainer').selectedChildWidget.id == 'tgGerar') {
                            pesquisarAluno(true);
                        }
                        else
                            pesquisarNotas(true);
                    }
                }, "pesquisaAluno");
                decreaseBtn(document.getElementById("pesquisaAluno"), '32px');

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdAluno').value = 0;
                            dojo.byId("txAluno").value = "";
                            dijit.byId('limparAluno').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAluno");
                if (hasValue(document.getElementById("limparAluno"))) {
                    document.getElementById("limparAluno").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAluno").parentNode.style.width = '40px';
                }


                // Ações Relacionadas:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Processar",
                    onClick: function () { eventoProcessar(gridAlunosemAula.itensSelecionados, xhr, ready, Memory, FilteringSelect); }
                });
                menu.addChild(acaoEditar);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Itens selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        if (dijit.byId('tabContainer').selectedChildWidget.id == 'tgGerar') {
                            buscarTodosItens(gridAlunosemAula, 'todosItens', ['pesquisaAluno']);
                            pesquisarAluno(false);
                        }
                        else {
                            buscarTodosItens(gridNotasDevolvidas, 'todosItens', ['pesquisaAluno']);
                            pesquisarNotas(false);
                        }

                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridAlunosemAula', 'selecionado', 'cd_item_movimento', 'selecionaTodos', ['pesquisaAluno'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("pesquisarItemFK")))
                                montargridPesquisaItem(function () {
                                    abrirItemFK();
                                    dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) pesquisarItemFK(MATERIALDIDATICO); });
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        pesquisarItemFK(MATERIALDIDATICO);
                                    });
                                    dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("proItem").hide(); });
                                }, true, true);
                            else
                                abrirItemFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKItem");

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdItem').value = 0;
                            dojo.byId("txItem").value = "";
                            dijit.byId('limparItem').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparItem");

                if (hasValue(document.getElementById("limparItem"))) {
                    document.getElementById("limparItem").parentNode.style.minWidth = '40px';
                    document.getElementById("limparItem").parentNode.style.width = '40px';
                }

                var buttonFkArray = ['FKAluno', 'FKItem'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                dijit.byId('ckHistorico').set("checked", true);
                //adicionarAtalhoPesquisa(['FKaluno', 'FKitem', 'cbHistorico', 'dtInicialFat', 'dtFinalFat'], 'pesquisaAluno', ready);
                dijit.byId('ckEscolas').on('change', function (e) {
                    if (e && !eval(MasterGeral())) {
                        caixaDialogo(DIALOGO_AVISO, msgErroMasterAlunosemAulaEscola, null);
                        dijit.byId('ckEscolas').set('value',false)
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunosemAula';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item_movimento", grid._by_idx[rowIndex].item.cd_item_movimento);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item_movimento', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item_movimento', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//function formatCheckBoxNota(value, rowIndex, obj) {
//    try {
//        var gridName = 'gridNotasDevolvidas';
//        var grid = dijit.byId(gridName);
//        var icon;
//        var id = obj.field + '_Selected_' + rowIndex;
//        var todos = dojo.byId('selecionaTodosN');

//        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
//            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item_movimento", grid._by_idx[rowIndex].item.cd_item_movimento);

//            value = value || indice != null; // Item está selecionado.
//        }
//        if (rowIndex != -1)
//            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

//        // Configura o check de todos:
//        if (hasValue(todos) && todos.type == 'text')
//            setTimeout("configuraCheckBox(false, 'cd_item_movimento', 'selecionadoN', -1, 'selecionaTodosN', 'selecionaTodosN', '" + gridName + "')", grid.rowsPerPage * 3);

//        setTimeout("configuraCheckBox(" + value + ", 'cd_item_movimento', 'selecionadoN', " + rowIndex + ", '" + id + "', 'selecionaTodosN', '" + gridName + "')", 2);

//        return icon;
//    }
//    catch (e) {
//        postGerarLog(e);
//    }
//}

function pesquisarAluno(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridAlunosemAula = dijit.byId("gridAlunosemAula");
            var cdAluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
            var cdItem = hasValue(dojo.byId("cdItem").value) ? dojo.byId("cdItem").value : 0;
            var idMovimento = dijit.byId("ckMovimento").checked;
            var idHistorico = dijit.byId("ckHistorico").checked;
            var idSituacao = hasValue(dijit.byId("cbHistorico").value) ? dijit.byId("cbHistorico").value : 0;
            var myStore =
                    Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getAlunosemAula?cd_aluno=" + cdAluno + "&cd_item=" + cdItem +
                                "&idEscola=" + dijit.byId('ckEscolas').checked + "&dtInicial=" + dojo.byId("dtInicialFat").value + "&dtFinal=" + dojo.byId("dtFinalFat").value +
                                "&idMovimento=" + idMovimento + "&idHistorico=" + idHistorico + "&idSituacao=" + idSituacao,
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({})
                    );

            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridAlunosemAula.itensSelecionados = [];
            gridAlunosemAula.itemSelecionado = null;
            gridAlunosemAula.noDataMessage = msgNotRegEnc;
            gridAlunosemAula.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function eventoProcessar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else
            if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            else
                if (!eval(MasterGeral()))
                    caixaDialogo(DIALOGO_AVISO, msgErroMasterAlunosemAula, null);
                else {
                caixaDialogo(DIALOGO_CONFIRMAR, msgConfirmarGeracaoKardexEntrada, function () { processarMovimento(itensSelecionados); });
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function processarMovimento(itensSelecionados) {
    dojo.xhr.post({
        url: Endereco() + "/api/aluno/postGerarKardexEntrada",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: JSON.stringify({
            cd_item_movimento: itensSelecionados[0].cd_item_movimento
        })
    }).then(function (data) {
        data = jQuery.parseJSON(data);
        if (hasValue(data)) {
            caixaDialogo(DIALOGO_AVISO, data, null);
            pesquisarAluno(true);
        }

    }, function (error) {
        haErro = jQuery.parseJSON(error.response.data);
        if (hasValue(haErro.erro) || !hasValue(haErro.retorno))
            if (hasValue(haErro.MensagensWeb)) {
                var mensagem = haErro.MensagensWeb[0].mensagem.indexOf('||') > 0 ? haErro.MensagensWeb[0].mensagem.substring(0, haErro.MensagensWeb[0].mensagem.indexOf('||')) : haErro.MensagensWeb[0].mensagem;
                caixaDialogo(DIALOGO_ERRO, mensagem, null);
            }
    })
}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = ALUNOSEMAULA;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        apresentaMensagem("apresentadorMensagem", null);

        limparPesquisaAlunoFK();

        pesquisarAlunoFK(true, ALUNOSEMAULA);
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }

}


//Aluno FK
function retornarAlunoFK() {
    try {
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            return false;
        }
        else {
            dojo.byId("cdAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("cdPessoaAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno;
            dojo.byId("txAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;

            dijit.byId('limparAluno').set("disabled", false);
            dijit.byId("proAluno").hide();

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirItemFK() {
    try {
        dojo.byId('tipoRetornoItemFK').value = ALUNOSEMAULA;
        limparPesquisaCursoFK(false);
        pesquisarItemFK(ALUNOSEMAULA);
        dijit.byId("proItem").show();
        dijit.byId("gridPesquisaItem").update();
        dijit.byId("tipo").set("disabled", true);
        dijit.byId("tipo").set("value", 1);
        dojo.byId("cdTipoItem").value = 1;
        dijit.byId("statusItemFK").set("disabled", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarItemFK() {
    var gridPesquisaItem = dijit.byId("gridPesquisaItem");
    if (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0) {
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        return false;
    }
    else if (gridPesquisaItem.itensSelecionados.length > 1) {
        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        return false;
    }
    else {

        dojo.byId("cdItem").value = gridPesquisaItem.itensSelecionados[0].cd_item;
        dojo.byId("txItem").value = gridPesquisaItem.itensSelecionados[0].no_item;
        dijit.byId('limparItem').set("disabled", false);
        dijit.byId("proItem").hide();
    }
}

function pesquisarNotas(limparItens) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridNotasDevolvidas = dijit.byId("gridNotasDevolvidas");
            var cdAluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
            var cdItem = hasValue(dojo.byId("cdItem").value) ? dojo.byId("cdItem").value : 0;
            var idMovimento = dijit.byId("ckMovimento").checked;
            var idHistorico = dijit.byId("ckHistorico").checked;
            var idSituacao = hasValue(dijit.byId("cbHistorico").value) ? dijit.byId("cbHistorico").value : 0;
            var myStoreN =
                Cache(
                    JsonRest({
                        target: Endereco() + "/api/aluno/getNotasDevolvidas?cd_aluno=" + cdAluno + "&cd_item=" + cdItem +
                            "&idEscola=" + dijit.byId('ckEscolas').checked + "&dtInicial=" + dojo.byId("dtInicialFat").value + "&dtFinal=" + dojo.byId("dtFinalFat").value +
                            "&idMovimento=" + idMovimento + "&idHistorico=" + idHistorico + "&idSituacao=" + idSituacao,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), Memory({})
                );

            var dataStore = new ObjectStore({ objectStore: myStoreN });


            if (limparItens)
                gridNotasDevolvidas.itensSelecionados = [];
            gridNotasDevolvidas.itemSelecionado = null;
            gridNotasDevolvidas.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
