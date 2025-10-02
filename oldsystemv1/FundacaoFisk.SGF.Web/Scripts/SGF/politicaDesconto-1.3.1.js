var HAS_ATIVO = 0;
var HAS_POLITICADESCONTO = 1;
//Os tipos de formulário:
var POLITICADESCONTO = 12;
var POLITICADESCONTOALUNO = 18;

function setarTabCad() {
    try{
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabPolitica");
        tabs.selectChild(pane);
    } catch (e) {
        postGerarLog(e);
    }
}
function selecionaTabPolitica(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTurma')
            dijit.byId("gridTurma").update();
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabAluno')
            dijit.byId("gridAluno").update();

    } catch (e) {
        postGerarLog(e);
    }
}

//#region formatCheckBox
function formatCheckBoxPoliticaDesconto(value, rowIndex, obj) {
    try{
        var gridName = 'gridPoliticaDesconto';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPoliticaDesconto');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_politica_desconto", grid._by_idx[rowIndex].item.cd_politica_desconto);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_politica_desconto', 'selecionadoPoliticaDesconto', -1, 'selecionaTodosPoliticaDesconto', 'selecionaTodosPoliticaDesconto', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_politica_desconto', 'selecionadoPoliticaDesconto', " + rowIndex + ", '" + id + "', 'selecionaTodosPoliticaDesconto', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTurma(value, rowIndex, obj) {
    try{
        var gridName = 'gridTurma';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTurma');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionadoTurma', -1, 'selecionaTodosTurma', 'selecionaTodosTurma', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'selecionadoTurma', " + rowIndex + ", '" + id + "', 'selecionaTodosTurma', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeAluno(rowIndex, campo, obj) {
    try{
        var gridAluno = dijit.byId('gridAluno');
        gridAluno._by_idx[rowIndex].item.selecionado = !gridAluno._by_idx[rowIndex].item.selecionado;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxDias(value, rowIndex, obj) {
    try{
        var gridName = 'gridDatasLimites';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id_dias", grid._by_idx[rowIndex].item.id_dias);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'id_dias', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'id_dias', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAluno(value, rowIndex, obj) {
    try{
        var gridName = 'gridAluno';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAluno');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionadoAluno', -1, 'selecionaTodosAluno', 'selecionaTodosAluno', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'selecionadoAluno', " + rowIndex + ", '" + id + "', 'selecionaTodosAluno', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxContrato(value, rowIndex, obj) {
    var gridName = 'gridContrato';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosContrato');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_contrato", grid._by_idx[rowIndex].item.cd_contrato);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionadoContrato', -1, 'selecionaTodosContrato', 'selecionaTodosContrato', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_contrato', 'selecionadoContrato', " + rowIndex + ", '" + id + "', 'selecionaTodosContrato', '" + gridName + "')", 2);

    return icon;
}

//#endregion

//#region keepValues
function keepValues(value, grid, ehLink) {
    try {
        showCarregando();
        limparPoliticaDesconto();
        setarTabCad();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        showPoliticaDesc(value.cd_politica_desconto);

    } catch (e) {
        postGerarLog(e);
    }
}
function loadAlunoPes(xhr) {
    xhr.get({
        url: Endereco() + "/api/aluno/getAlunoPolitica",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataAlunoAtiva) {
        try {
            loadAluno(jQuery.parseJSON(dataAlunoAtiva).retorno, 'cdAluno');
            dijit.byId("cdAluno").set("value", 0);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function loadTurmaPes() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            preventCache: true,
            url: Endereco() + "/api/Turma/getTurmaPoliticaEsc",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataTurmaAtivo) {
            try {
                loadTurma(dataTurmaAtivo.retorno, 'cdTurma');
                dijit.byId("cdTurma").set("value", 0);
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadTurma(data, linkTurma) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var items = [];
                if ((linkTurma == 'cdTurma')) {
                    items.push({ id: -1, name: "Nenhuma" });
                    items.push({ id: 0, name: "Todas" });
                }
                Array.forEach(data, function (value, i) {
                    items.push({ id: value.cd_turma, name: value.no_turma });
                });
                var stateStore = new Memory({
                    data: items
                });
                dijit.byId(linkTurma).store = stateStore;
                dijit.byId("cdTurma").value = 0;
                dijit.byId("cdTurma").required = false;
            } catch (e) {
                postGerarLog(e);
            }
        })
}

function loadAluno(data, linkAluno) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var items = [];
                if ((linkAluno == 'cdAluno')) {
                    items.push({ id: -1, name: "Nenhum" });
                    items.push({ id: 0, name: "Todos" });
                }
                Array.forEach(data, function (value, i) {
                    items.push({ id: value.cd_aluno, name: value.no_pessoa });
                });
                var stateStore = new Memory({
                    data: items
                });
                dijit.byId(linkAluno).store = stateStore;
                dijit.byId("cdAluno").value = 0;
                dijit.byId("cdAluno").required = false;
            } catch (e) {
                postGerarLog(e);
            }
        })
}

function loadContrato(Memory) {
    var cnabStatusStore = new Memory({
        data: [
        { name: "Todos", id: -1 },
        { name: "730", id: 0 },
        { name: "780", id: 1 }
        ]
    });
    dijit.byId("cbContrato").store = cnabStatusStore;
    dijit.byId("cbContrato").set("value", -1);
}

//#region montarCadastroPoliticaDesconto
function montarCadastroPoliticaDesconto() {

    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/DateTextBox"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox) {
        ready(function () {
            try {
                loadAlunoPes(xhr);
                loadTurmaPes();
                var myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/financeiro/getPoliticaDescontoSearch?cdTurma=0&cdAluno=0&dtaIni=null&dtaFim=null&ativo=1",
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                   }
               ), Memory({ idProperty: "cd_politica_desconto" }));

                //*** Cria a grade de PoliticaDesconto **\\
                var gridPoliticaDesconto = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosPoliticaDesconto' style='display:none'/>", field: "selecionadoPoliticaDesconto", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPoliticaDesconto },
                       // { name: "Código", field: "cd_politica_desconto", width: "75px", styles: "width:75px; text-align: left;" },
                        { name: "Turma(s)", field: "no_turma", width: "35%", styles: "min-width:50px;" },
                        { name: "Aluno(s)", field: "no_aluno", width: "35%", styles: "min-width:50px;" },
                        { name: "Dia Limite/Percentual", field: "dias_percential", width: "25%", styles: "min-width:50px;" },
                        { name: "Data Inicial", field: "dt_inicial", width: "8%", styles: "min-width:50px;" },
                        { name: "Ativo", field: "politica_desconto_ativo", width: "10%", styles: "text-align: center;" }
                      ],
                    canSort: true,
                    selectionMode: "single",
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                }, "gridPoliticaDesconto");

                //Paginação
                gridPoliticaDesconto.pagination.plugin._paginator.plugin.connect(gridPoliticaDesconto.pagination.plugin._paginator, 'onSwitchPageSize',
                    function (evt) {
                        verificaMostrarTodos(evt, gridPoliticaDesconto, 'cd_politica_desconto', 'selecionaTodos');
                    });
                //Seleciona todos e impede que a primeira coluna seja ordenada
                gridPoliticaDesconto.canSort = function (col) { return Math.abs(col) > 3; };
                gridPoliticaDesconto.startup();
                gridPoliticaDesconto.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        // Limpar os dados do form
                        getLimpar('#formPoliticaDesconto');
                        clearForm("formPoliticaDesconto");
                        apresentaMensagem('apresentadorMensagem', '');
                        setarTabCad();

                        keepValues(item, gridPoliticaDesconto, false);
                        dijit.byId("cad").show();
                        dijit.byId('tabContainer').resize();
                        IncluirAlterar(0, 'divAlterarPoliticaDesconto', 'divIncluirPoliticaDesconto', 'divExcluirPoliticaDesconto', 'apresentadorMensagemPoliticaDesc', 'divCancelarPoliticaDesconto', 'divLimparPoliticaDesconto');

                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);


                //*** Cria a grade de Data Limites **\\

                var data = new Array();
                var gridDatasLimites = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxDias },
                        { name: "Dia Limite", field: "nm_dia_limite_politica", width: "48%", styles: "min-width:50px;" },
                        { name: "Percentual", field: "desconto", width: "47%", styles: "min-width:50px;" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridDatasLimites");
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPoliticaDesconto, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_politica_desconto', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridPoliticaDesconto')", gridPoliticaDesconto.rowsPerPage * 3);
                    });
                });

                gridDatasLimites.startup();
                gridDatasLimites.on("RowDblClick", function (evt) {
                    try {
                        dijit.byId("dialogDataLimite").show();
                        var idx = evt.rowIndex,
                           item = this.getItem(idx),
                           store = this.store;
                        document.getElementById("cd_politica_desconto_dias").value = item.cd_politica_desconto,
                        document.getElementById("cd_dias_politica").value = item.cd_dias_politica,
                        document.getElementById("dia_limite").value = item.nm_dia_limite_politica;
                        document.getElementById("percentual").value = item.desconto;
                        document.getElementById('divIncluirDiasPolitica').style.display = "none";
                        document.getElementById('divEditarDiasPolitica').style.display = "";
                        dojo.byId("cd_dias_aux").value = item.id_dias;
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                
                var data = new Array();
                var gridTurma = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosTurma' style='display:none'/>", field: "selecionadoTurma", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTurma },
                        { name: "Turma", field: "no_turma", width: "95%", styles: "min-width:50px;" }
                      ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["9", "18", "27", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "9",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridTurma");
                gridTurma.canSort = function (col) { return Math.abs(col) != 1; };
                gridTurma.startup();

                var data = new Array();
                var gridAluno = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) }),
                    structure:
                    [
                      { name: "<input id='selecionaTodosAluno' style='display:none'/>", field: "selecionadoAluno", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAluno },
                      { name: "Aluno", field: "no_aluno", width: "95%", styles: "min-width:50px;" }
                    ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["9", "18", "27", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "9",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridAluno"); // make sure you have a target HTML element with this id
                gridAluno.canSort = function (col) { return Math.abs(col) != 1; };
                gridAluno.startup();


                //*** Cria a grade de Contratos **\\
                var dataContrato = [
                    { selecionado: false, no_aluno: 'Aluno 3', nm_contrato: '730' },
                    { selecionado: false, no_aluno: 'Aluno 29', nm_contrato: '780' }
                ];
                var gridContrato = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataContrato }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosContrato' style='display:none'/>", field: "selecionadoContrato", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxContrato },
                        { name: "Aluno", field: "no_aluno", width: "60%", styles: "min-width:50px;" },
                        { name: "Contrato", field: "nm_contrato", width: "35%", styles: "min-width:50px;" }
                      ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["9", "18", "27", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "9",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridContrato");
                gridContrato.canSort = function (col) { return Math.abs(col) != 1; };
                gridContrato.startup();

                //////
                //*** Cria os botões do link de ações **\\
                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridPoliticaDesconto, 'todosItens', ['pesquisarPoliticaDesconto', 'relatorioPoliticaDesconto']);
                        PesquisarPoliticaDesconto(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridPoliticaDesconto', 'selecionadoPoliticaDesconto', 'cd_politica_desconto', 'selecionaTodos', ['pesquisarPoliticaDesconto', 'relatorioPoliticaDesconto'], 'todosItens');
                    }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionadosPoliticaDesconto").appendChild(button.domNode);

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () {  eventoRemover(gridPoliticaDesconto.itensSelecionados, 'DeletarPoliticaDesconto(itensSelecionados)'); }
                });
                menu.addChild(acaoExcluir);

                acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridPoliticaDesconto.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasPoliticaDesconto",
                    dropDown: menu,
                    id: "acoesRelacionadasPoliticaDesconto"
                });
                dom.byId("linkAcoesPoliticaDesconto").appendChild(button.domNode);


                //Ação relacionada excluir turma
                // Adiciona link de ações:
                var menuTurma = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluirTurma = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_turma', dijit.byId("gridTurma"));
                    }
                });
                menuTurma.addChild(acaoExcluirTurma);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasTurma",
                    dropDown: menuTurma,
                    id: "acoesRelacionadasTurma"
                });
                dom.byId("linkAcoesTurma").appendChild(button.domNode);
                //
                //Ação relacionada excluir aluno
                // Adiciona link de ações:
                var menuAluno = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluirAluno = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno"));
                    }
                });
                menuAluno.addChild(acaoExcluirAluno);            

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasAluno",
                    dropDown: menuAluno,
                    id: "acoesRelacionadasAluno"
                });
                dom.byId("linkAcoesAluno").appendChild(button.domNode);

                //Ação relacionada excluir aluno
                // Adiciona link de ações:
                //var menuContrato = new DropDownMenu({ style: "height: 25px" });
                //var acaoExcluirContrato = new MenuItem({
                //    label: "Excluir",
                //    onClick: function () {
                        //deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_turma', dijit.byId("gridTurma"));
                //    }
                //});
                //menuContrato.addChild(acaoExcluirContrato);

                //var button = new DropDownButton({
                //    label: "Ações Relacionadas",
                //    name: "acoesRelacionadasContrato",
                //    dropDown: menuContrato,
                //    id: "acoesRelacionadasContrato"
                //});
                //dom.byId("linkAcoesContrato").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar",
                    iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        IncluirPoliticaDesconto();
                    }
                }, "incluirPoliticaDesconto");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { AlterarPoliticaDesconto(); }
                }, "alterarPoliticaDesconto");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { DeletarPoliticaDesconto(); }
                }, "deletePoliticaDesconto");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        limparPoliticaDesconto();
                    }
                }, "limparPoliticaDesconto");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagemPoliticaDesc', null);
                        keepValues(null, gridPoliticaDesconto, false);
                    }
                }, "cancelarPoliticaDesconto");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cad").hide();
                    }
                }, "fecharPoliticaDesconto");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        PesquisarPoliticaDesconto();
                    }
                }, "pesquisarPoliticaDesconto");
                decreaseBtn(document.getElementById("pesquisarPoliticaDesconto"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            dijit.byId("cad").show();
                            dijit.byId('tabContainer').resize();
                            setarTabCad();
                            apresentaMensagem('apresentadorMensagem', null);
                            limparPoliticaDesconto();
                            dojo.byId("dt_inicial_politica").value = '';
                            dijit.byId("dt_inicial_politica").set("required", true);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoPoliticaDesconto");

                new Button({
                    label: "Excluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { deletarDiasPolitica() }
                }, "deleteDias");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try{
                            var cdTurma = dijit.byId("cdTurma").get("value") == null || dijit.byId("cdTurma").get("value") == "" ? 0 : dijit.byId("cdTurma").get("value");
                            var cdAluno = dijit.byId("cdAluno").get("value") == null || dijit.byId("cdAluno").get("value") == "" ? 0 : dijit.byId("cdAluno").get("value");
                            require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
                                xhr.get({
                                    url: Endereco() + "/api/financeiro/geturlrelatorioPoliticaDesconto?" + getStrGridParameters('gridPoliticaDesconto') + "cdTurma=" + cdTurma + "&cdAluno=" + cdAluno + "&dtaIni=" + dojo.byId("periodoInicial").value + "&dtaFim=" + dojo.byId("periodoFinal").value + "" + "&ativo=" + retornaStatus("statusPoliticaDesconto"),
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                            })
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioPoliticaDesconto");
                new Button({
                    label: "Pesquisar",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarEsc(true); }
                }, "pesquisarMatDid");
                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            dijit.byId("dialogDataLimite").show();
                            document.getElementById("dia_limite").value = "";
                            document.getElementById("percentual").value = "";
                            document.getElementById('divIncluirDiasPolitica').style.display = "";
                            document.getElementById('divEditarDiasPolitica').style.display = "none";
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirDataLimite");

                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () { IncluirDiasLimites(); }
                }, "incluirDiasPolitica");

                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () { editarDiasLimites(); }
                }, "editarDiasPolitica");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        document.getElementById("dia_limite").value = "";
                        document.getElementById("percentual").value = "";
                    }

                }, "limparDiasPolitica");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("dialogDataLimite").hide();
                    }
                }, "fecharDiasPolitica");
                
                montarStatus("statusPoliticaDesconto");
                // Adiciona link de ações:
                var menuDia = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluirDia = new MenuItem({
                    label: "Excluir",
                    onClick: function () {  eventoRemover(dijit.byId("gridDatasLimites").itensSelecionados, 'deletarDiasPolitica(itensSelecionados)'); }
                });
                menuDia.addChild(acaoExcluirDia);

                // Adiciona link de ações:
                var acaoEditarDia = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarDia(dijit.byId("gridDatasLimites").itensSelecionados); }
                });
                menuDia.addChild(acaoEditarDia);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasDia",
                    dropDown: menuDia,
                    id: "acoesRelacionadasDia"
                });
                dom.byId("linkAcoesDia").appendChild(button.domNode);

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true)) {
                                dojo.byId("idOrigemCadastro").value = POLITICADESCONTO;
                                montarGridPesquisaTurmaFK(function () {
                                    funcaoFKTurma();
                                    dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                            montarGridPesquisaAluno(false, function () {
                                                abrirAlunoFKTurmaFK(true);
                                            });
                                        }
                                        else
                                            abrirAlunoFKTurmaFK(true);
                                    });
                                });
                            } else
                                funcaoFKTurma();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnAddTurma");

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFk();
                                });
                            }
                            else
                                abrirAlunoFk();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnAddAluno");
                
                //new Button({
                //    label: "Incluir",
                //    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                //    onClick: function ()  {
                //        try {
                //            if (!hasValue(dijit.byId("gridContratoFK"))) {
                //                montarFkContrato(false, function () {
                //                    abrirContratoFk();
                //                });
                //            }
                //            else
                //                abrirContratoFk();
                //        } catch (e) {
                //            postGerarLog(e);
                //        }
                //    }
                //}, "btnAddContrato");

                adicionarAtalhoPesquisa(['cdTurma', 'cdAluno', 'statusPoliticaDesconto', 'periodoInicial', 'periodoFinal'], 'pesquisarPoliticaDesconto', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323064', '765px', '771px');
                        });
                }
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
};
    //#endregion
function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formPoliticaDesconto');
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, dijit.byId('gridPoliticaDesconto'), true);

            dijit.byId("cad").show();
            dijit.byId('tabContainer').resize();
            IncluirAlterar(0, 'divAlterarPoliticaDesconto', 'divIncluirPoliticaDesconto', 'divExcluirPoliticaDesconto', 'apresentadorMensagemPoliticaDesc', 'divCancelarPoliticaDesconto', 'divLimparPoliticaDesconto');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarDia(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            dijit.byId("dialogDataLimite").show();

            document.getElementById("cd_politica_desconto_dias").value = itensSelecionados[0].cd_politica_desconto,
            document.getElementById("cd_dias_politica").value = itensSelecionados[0].cd_dias_politica,
            document.getElementById("dia_limite").value = itensSelecionados[0].nm_dia_limite_politica;
            document.getElementById("percentual").value = itensSelecionados[0].desconto;
            document.getElementById('divIncluirDiasPolitica').style.display = "none";
            document.getElementById('divEditarDiasPolitica').style.display = "";
            dojo.byId("cd_dias_aux").value = itensSelecionados[0].id_dias;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function IncluirDiasLimites() {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                if (!dijit.byId("dialogDataLimite").validate())
                    return false;
                var gridDatasLimites = dijit.byId("gridDatasLimites");
                var IdDias = geradorIdDatasLimites(gridDatasLimites);
                var mensagensWeb = new Array();
                dojo.byId("cd_dias_aux").value = IdDias;

                if (gridDatasLimites.store.objectStore.data.some(index => index.nm_dia_limite_politica == document.getElementById('dia_limite').value))
                {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Existe política de desconto para o dia informado.");
                    apresentaMensagem("apresentadorMensagemPoliticaDesc", mensagensWeb);
                    return false;
                }

                insertObjSort(gridDatasLimites.store.objectStore.data, "id_dias", {                    
                    id_dias: IdDias,
                    nm_dia_limite_politica: document.getElementById('dia_limite').value,
                    desconto: document.getElementById('percentual').value
                });
                gridDatasLimites.setStore(new ObjectStore({ objectStore: new Memory({ data: gridDatasLimites.store.objectStore.data }) }));
                dijit.byId("dialogDataLimite").hide();
                apresentaMensagem("apresentadorMensagemPoliticaDesc", null);
            } catch (e) {
                postGerarLog(e);
            }
        });
}

function geradorIdDatasLimites(gridDatasLimites) {
    try {
        var id = gridDatasLimites.store.objectStore.data.length;
        var itensArray = gridDatasLimites.store.objectStore.data.sort(function byOrdem(a, b) { return a.id_dias - b.id_dias; });
        if (id == 0)
            id = 1;
        else if (id > 0)
            id = itensArray[id - 1].id_dias + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarDiasLimites() {
    try {
        if (!dijit.byId("dialogDataLimite").validate())
            return false;
        var grid = dijit.byId("gridDatasLimites");
        for (var i = 0; i < grid._by_idx.length; i++)
            if ((hasValue(grid._by_idx[i].item.cd_dias_politica) && grid._by_idx[i].item.cd_dias_politica == dojo.byId("cd_dias_politica").value) ||
                (hasValue(grid._by_idx[i].item.id_dias) && grid._by_idx[i].item.id_dias == parseInt(dojo.byId("cd_dias_aux").value))) {
                grid._by_idx[i].item.nm_dia_limite_politica = dojo.byId("dia_limite").value;
                grid._by_idx[i].item.desconto = dojo.byId("percentual").value;
                break;
            }
        dijit.byId("dialogDataLimite").hide();
        grid.update();
    } catch (e) {
        postGerarLog(e);
    }
}

function showPoliticaDesc(cdPolitica) {
    getLimpar('#formPoliticaDesconto');
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getPoliticaEdit?id=" + cdPolitica,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data).retorno;
            var dataStoreDias = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.DiasPolitica }) });
            var gridDatasLimites = dijit.byId("gridDatasLimites");
            gridDatasLimites.itensSelecionados = [];
            gridDatasLimites.setStore(dataStoreDias);
            if(hasValue(data.qtd_turmas) || hasValue(data.qtd_alunos)){
                if(!(hasValue(data.qtd_turmas) && hasValue(data.qtd_alunos))){
                    if(hasValue(data.qtd_turmas))
                        dijit.byId("btnAddAluno").set("disabled", true);
                    else
                        dijit.byId("btnAddTurma").set("disabled", true);
                }
            }
            dojo.byId("cd_politica_desconto").value = data.cd_politica_desconto;
            dijit.byId("dt_inicial_politica").set("value", data.dt_inicial_politica);
            dijit.byId("id_ativo").set("value", data.id_ativo);

            var dataStoreTurma = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.PoliticasTurmas }) });
            var gridTurma = dijit.byId("gridTurma");
            gridTurma.itensSelecionados = [];
            gridTurma.setStore(dataStoreTurma);

            var dataStoreAluno = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.PoliticasAlunos }) });
            var gridAluno = dijit.byId("gridAluno");
            gridAluno.itensSelecionados = [];
            gridAluno.setStore(dataStoreAluno);
            showCarregando();
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function limparPoliticaDesconto() {
    require(["dojo/data/ObjectStore",
              "dojo/store/Memory"
    ], function (ObjectStore, Memory) {
        try {
            getLimpar('#formPoliticaDesconto');
            clearForm('cad');
            IncluirAlterar(1, 'divAlterarPoliticaDesconto', 'divIncluirPoliticaDesconto', 'divExcluirPoliticaDesconto', 'apresentadorMensagemPoliticaDesc', 'divCancelarPoliticaDesconto', 'divLimparPoliticaDesconto');
            document.getElementById("cd_politica_desconto").value = null;
            document.getElementById("dt_inicial_politica").value = null;
            //Limpa as informações da grade de Turmas:
            var gridTurma = dijit.byId('gridTurma');
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
            gridTurma.setStore(dataStore);
            gridTurma.update();
            dijit.byId("btnAddTurma").set("disabled", false);

            //Limpa as informações da grade de Alunos:            
            var gridAluno = dijit.byId("gridAluno");
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
            gridAluno.setStore(dataStore);
            gridAluno.update();
            dijit.byId("btnAddAluno").set("disabled", false);

            //Limpa as informações da grade de Dias Limites :
            var gridDatasLimites = dijit.byId('gridDatasLimites');

            if (hasValue(gridDatasLimites)) {
                var data = new Array();
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) });
                gridDatasLimites.setStore(dataStore);
                gridDatasLimites.update();
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function EnviarDadosPoliticaDesconto() {
    try{
        if (document.getElementById("divAlterarPoliticaDesconto").style.display == "")
            AlterarPoliticaDesconto();
        else
            IncluirPoliticaDesconto();
    } catch (e) {
        postGerarLog(e);
    }
}

//#region crud  - métodos auxiliares
function TurmasPolitica() {
    try{
        var turmas = [];
        var gridTurma = dijit.byId("gridTurma");
        hasValue(gridTurma) ? gridTurma.store.save() : null;
        if (hasValue(gridTurma) && hasValue(gridTurma._by_idx))
            var data = gridTurma.store.objectStore.data;
        else turmas = null;
        if (hasValue(gridTurma) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                turmas.push(val);
            });
        return turmas;
    } catch (e) {
        postGerarLog(e);
    }
}

function AlunosPolitica() {
    try{
        var alunos = [];
        var gridAluno = dijit.byId("gridAluno");
        hasValue(gridAluno) ? gridAluno.store.save() : null;
        if (hasValue(gridAluno) && hasValue(gridAluno._by_idx))
            var data = gridAluno.store.objectStore.data;
        else alunos = null;
        if (hasValue(gridAluno) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                alunos.push(val);
            });
        return alunos;
    } catch (e) {
        postGerarLog(e);
    }
}

function DatasLimites() {
    try{
        var datasLimites = [];
        if (hasValue(dijit.byId("gridDatasLimites")._by_idx) && dijit.byId("gridDatasLimites")._by_idx.length > 0)
            datasLimites = dijit.byId("gridDatasLimites")._by_idx;
        var desconto = 0;
        var retornoDatas = [];
        for (var i = 0; i < datasLimites.length; i++) {
            desconto = datasLimites[i].item.desconto;
            desconto = parseFloat(desconto.toString().replace(",", "."));
            retornoDatas[i] = {
                cd_dias_politica: datasLimites[i].item.cd_dias_politica,
                nm_dia_limite_politica: datasLimites[i].item.nm_dia_limite_politica,
                pc_desconto: desconto
            };
        }
        return retornoDatas;
    } catch (e) {
        postGerarLog(e);
    }
}

function IncluirPoliticaDesconto() {
        
    require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            var turmas = TurmasPolitica();
            var alunos = AlunosPolitica();
            var datasLimites = DatasLimites();
            var mensagensWeb = new Array();
            if (!dijit.byId("formPoliticaDesconto").validate())
                return false;

            if (datasLimites.length <= 0) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDataLimite);
                apresentaMensagem("apresentadorMensagemPoliticaDesc", mensagensWeb);
            }
            else {
                var dt_politica = hasValue(dojo.byId("dt_inicial_politica").value) ? dojo.date.locale.parse(dojo.byId("dt_inicial_politica").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                apresentaMensagem('apresentadorMensagemPoliticaDesc', null);
                apresentaMensagem('apresentadorMensagem', null);
                showCarregando();
                xhr.post(Endereco() + "/api/coordenacao/postPoliticaDesconto", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({
                        dt_inicial_politica: dt_politica,//dojo.byId("dt_inicial_politica").value,
                        id_ativo: dojo.byId("id_ativo").checked,
                        PoliticasTurmas: turmas,
                        PoliticasAlunos: alunos,
                        DiasPolitica: datasLimites
                    })
                }).then(function (data) {
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridPoliticaDesconto';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();
                        var ativo = dom.byId("id_ativo").checked ? 1 : 2;
                        dijit.byId("statusPoliticaDesconto").set("value", ativo);

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_politica_desconto", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoPoliticaDesconto', 'cd_politica_desconto', 'selecionaTodos', ['pesquisarPoliticaDesconto', 'relatorioPoliticaDesconto'], 'todosItens');

                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_politica_desconto");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemPoliticaDesc', data);
                    showCarregando();
                },
                    function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemPoliticaDesc', error);
                    });
            }
        } catch (e) {
            postGerarLog(e);
        }
    });   
}

function PesquisarPoliticaDesconto(limparItens) {
    try{
        var cdTurma = dijit.byId("cdTurma").get("value") == null || dijit.byId("cdTurma").get("value") == "" ? 0 : dijit.byId("cdTurma").get("value");
        var cdAluno = dijit.byId("cdAluno").get("value") == null || dijit.byId("cdAluno").get("value") == "" ? 0 : dijit.byId("cdAluno").get("value");
        var dtaIni = hasValue(dojo.byId("periodoInicial").value != '') ? dojo.byId("periodoInicial").value : null;
        var dtaFim = hasValue(dojo.byId("periodoFinal").value != '') ? dojo.byId("periodoFinal").value : null;
        require([
                   "dojo/store/JsonRest",
                   "dojo/data/ObjectStore",
                   "dojo/store/Cache",
                   "dojo/store/Memory",
                   "dojo/query"
        ], function (JsonRest, ObjectStore, Cache, Memory, query) {
            try{
                query("body").addClass("claro");
                var myStore = Cache(
                    JsonRest({
                        handleAs: "json",
                        target: Endereco() + "/api/financeiro/getPoliticaDescontoSearch?cdTurma=" + cdTurma + "&cdAluno=" + cdAluno + "&dtaIni=" + dtaIni + "&dtaFim=" + dtaFim + "&ativo=" + retornaStatus("statusPoliticaDesconto"),
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }), Memory({})
               );
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridPoliticaDesconto = dijit.byId("gridPoliticaDesconto");
                if (limparItens)
                    gridPoliticaDesconto.itensSelecionados = [];
                gridPoliticaDesconto.setStore(dataStore);
            } catch (e) {
                postGerarLog(e);
            }
        },
          function (error) {
              apresentaMensagem('apresentadorMensagemPoliticaDesc', error);
          });
    } catch (e) {
        postGerarLog(e);
    }
}

function AlterarPoliticaDesconto() {
    try {
        var turmas = TurmasPolitica();
        var alunos = AlunosPolitica();
        var datasLimites = DatasLimites();
        if (!dijit.byId("formPoliticaDesconto").validate())
            return false;
        var dt_politica = hasValue(dojo.byId("dt_inicial_politica").value) ? dojo.date.locale.parse(dojo.byId("dt_inicial_politica").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        apresentaMensagem('apresentadorMensagemPoliticaDesc', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (datasLimites.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDataLimite);
            apresentaMensagem("apresentadorMensagemPoliticaDesc", mensagensWeb);
        }
        else {
            showCarregando();
            require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
                xhr.post({
                    url: Endereco() + "/api/financeiro/postAlterarPoliticaDesconto",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    postData: ref.toJson({
                        cd_politica_desconto: dojo.byId("cd_politica_desconto").value,
                        dt_inicial_politica: dt_politica,//dojo.byId("dt_inicial_politica").value,
                        id_ativo: dojo.byId("id_ativo").checked,
                        PoliticasTurmas: turmas,
                        PoliticasAlunos: alunos,
                        DiasPolitica: datasLimites
                    })
                }).then(function (data) {
                    try {
                        if (!hasValue(data.erro)) {
                            var itemAlterado = data.retorno;
                            var gridName = 'gridPoliticaDesconto';
                            var grid = dijit.byId(gridName);
                            var todos = dojo.byId("todosItens_label");

                            if (!hasValue(grid.itensSelecionados))
                                grid.itensSelecionados = [];
                            apresentaMensagem('apresentadorMensagem', data);
                            setarTabCad();
                            dijit.byId("cad").hide();
                            var ativo = dom.byId("id_ativo").checked ? 1 : 2;
                            dijit.byId("statusPoliticaDesconto").set("value", ativo);
                            removeObjSort(grid.itensSelecionados, "cd_politica_desconto", dom.byId("cd_politica_desconto").value);
                            insertObjSort(grid.itensSelecionados, "cd_politica_desconto", itemAlterado);

                            buscarItensSelecionados(gridName, 'selecionadoPoliticaDesconto', 'cd_politica_desconto', 'selecionaTodos', ['pesquisarPoliticaDesconto', 'relatorioPoliticaDesconto'], 'todosItens');

                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_politica_desconto");
                        }
                        else
                            apresentaMensagem('apresentadorMensagemPoliticaDesc', data);
                        showCarregando();
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemPoliticaDesc', error);
                });
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarPoliticaDesconto(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            var grade = true;
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                if (dojo.byId('cd_politica_desconto').value != 0)
                    itensSelecionados = [{
                        cd_politica_desconto: dom.byId("cd_politica_desconto").value,
                        dt_inicial_politica: dom.byId("dt_inicial_politica").value,
                        id_ativo: dom.byId("id_ativo").value
                    }];
                grade = false;
            }
            xhr.post({
                url: Endereco() + "/api/financeiro/postdeletePoliticaDesconto",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    data = $.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        PesquisarPoliticaDesconto(true);
                        var todos = dojo.byId("todosItens_label");
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridPoliticaDesconto').itensSelecionados, "cd_politica_desconto", itensSelecionados[r].cd_politica_desconto);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarPoliticaDesconto").set('disabled', false);
                        dijit.byId("relatorioPoliticaDesconto").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemPoliticaDesc', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!grade)
                    apresentaMensagem('apresentadorMensagemPoliticaDesc', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

    //#endregion
function deletarDiasPolitica(itensSelecionados) {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
     function (Memory, ObjectStore) {
         try {
             //var enderecos = dijit.byId("gridEnderecos").selection.getSelected();
             var dias = itensSelecionados;

             if (dias.length > 0) {
                 var arrayDias = dijit.byId("gridDatasLimites")._by_idx;
                 $.each(dias, function (idx, valueDias) {

                     arrayDias = jQuery.grep(arrayDias, function (value) {
                         return value.item != valueDias;
                     });
                 });
                 var dados = [];
                 $.each(arrayDias, function (index, value) {
                     dados.push(value.item);
                 });
                 var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
                 dijit.byId("gridDatasLimites").setStore(dataStore);
                 var mensagensWeb = new Array();
                 mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Dias excluídos com sucesso.");

                 dijit.byId("gridDatasLimites").itensSelecionados = [];
             }
         } catch (e) {
             postGerarLog(e);
         }
     });
}

function funcaoFKTurma() {
    try {
        //var turmaEmFormacao = 3;
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "none";
        dijit.byId("pesSituacaoFK").costumizado = false;
        dojo.byId("idOrigemCadastro").value = POLITICADESCONTO;
        abrirTurmaFK();
        dijit.byId('pesTurmasFilhasFK').set("disabled", true);
        dijit.byId('pesTurmasFilhasFK').set("checked", true);
        dojo.byId("lblTurmaFilhasFK").style.display = "none";
        dojo.byId("divPesTurmasFilhasFK").style.display = "none";
        pesquisarTurmaFK();
        //dojo.byId("legendTurmaFK").style.visibility = 'visible';

    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaFK() {
    try {
        loadSituacaoTurmaFKTodas(dojo.store.Memory);
        dijit.byId("proTurmaFK").show();
        dijit.byId('tipoTurmaFK')._onChangeActive = false;
        dijit.byId('tipoTurmaFK').set('value', 1);
        dijit.byId('tipoTurmaFK')._onChangeActive = true;
        var turmaEmFormacao = 3;
        dijit.byId("pesSituacaoFK").set("value", turmaEmFormacao);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK() {
    require(["dojo/ready",
        "dojo/store/Memory",
        "dijit/form/FilteringSelect",
        "dojo/data/ObjectStore",
        "dojo/_base/xhr",
        "dojox/json/ref"],
        function (ready, Memory, filteringSelect, ObjectStore, xhr, ref) {
            try {
                var gridTurmas = null;
                var valido = true;
                var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");

                gridTurmas = dijit.byId("gridTurma");

                if ((!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) &&
                    (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)) {
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    valido = false;
                }
                else {
                    for (var i = 0; i < gridPesquisaTurmaFK.itensSelecionados.length; i++) {
                        var storeGridTurma = (hasValue(gridTurmas) && hasValue(gridTurmas.store.objectStore.data)) ? gridTurmas.store.objectStore.data : [];
                        if (storeGridTurma != null && storeGridTurma.length > 0) {
                            insertObjSort(storeGridTurma, "cd_turma", {
                                cd_turma: gridPesquisaTurmaFK.itensSelecionados[i].cd_turma,
                                no_turma: gridPesquisaTurmaFK.itensSelecionados[i].no_turma,
                                cd_politica_desconto: hasValue(dojo.byId("cd_politica_desconto").value) ? dojo.byId("cd_politica_desconto").value : 0
                            });
                            gridTurmas.setStore(new ObjectStore({ objectStore: new Memory({ data: gridTurmas.store.objectStore.data }) }));

                        } else {
                            var dados = [];
                            insertObjSort(dados, "cd_turma", {
                                cd_turma: gridPesquisaTurmaFK.itensSelecionados[i].cd_turma,
                                no_turma: gridPesquisaTurmaFK.itensSelecionados[i].no_turma,
                                cd_politica_desconto: hasValue(dojo.byId("cd_politica_desconto").value) ? dojo.byId("cd_politica_desconto").value : 0

                            });
                            gridTurmas.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                        }
                    }

                }
                if (!valido)
                    return false;
                dijit.byId("proTurmaFK").hide();
            } catch (e) {
                postGerarLog(e);
            }
        });
}

function abrirAlunoFk() {
    dojo.byId('tipoRetornoAlunoFK').value = POLITICADESCONTOALUNO;
    dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    limparPesquisaAlunoFK();
    pesquisarAlunoFK(true);
    dijit.byId("proAluno").show();
    dijit.byId('gridPesquisaAluno').update();
}

function retornarAlunoFK() {

    try {
        var gridAluno = dijit.byId("gridAluno");
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else {
            var storeGridAluno = (hasValue(gridAluno) && hasValue(gridAluno.store.objectStore.data)) ? gridAluno.store.objectStore.data : [];

            if (storeGridAluno != null && storeGridAluno.length > 0) {
                $.each(gridPesquisaAluno.itensSelecionados, function (idx, value) {
                    insertObjSort(gridAluno.store.objectStore.data, "cd_aluno", { cd_aluno: value.cd_aluno, no_aluno: value.no_pessoa });
                });
                gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridAluno.store.objectStore.data }) }));

            } else {
                var dados = [];
                $.each(gridPesquisaAluno.itensSelecionados, function (index, val) {
                    insertObjSort(dados, "cd_aluno", { cd_aluno: val.cd_aluno, no_aluno: val.no_pessoa });
                });
                gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));

            }
            dijit.byId("proAluno").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//function abrirContratoFk() {
    //dijit.byId("gridContratoFK").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    //limparPesquisaContratoFK();
    //pesquisarContratoFK(true);
//    dijit.byId("proContrato").show();
//    dijit.byId('gridContratoFK').update();
//}