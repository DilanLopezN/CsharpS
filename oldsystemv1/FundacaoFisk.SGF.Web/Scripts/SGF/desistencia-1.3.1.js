//Variaveis
var DESISTENCIA_CAD = 8, TODAS = 1, CANCELAMENTO = 2;
var TODOS = 0;

var TIPO = new Array(
   { name: "Desistência", id: "1" },
   { name: "Cancelamento", id: "2" },
   { name: "Não Rematrícula", id: "3" },
   { name: "Cancelamento de Não Rematrícula", id: "4" }
);

//#region monta os drops
function getComponentesDesistencia() {
    dojo.xhr.get({
        url: Endereco() + "/api/coordenacao/getComponentesDesistencia",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        data = $.parseJSON(data);
        if (data.retorno != null && data.retorno != null) {
            loadTipoDesistencia(data.retorno.motivosDesistencia);
            if (hasValue(data.retorno.produtos))
                criarOuCarregarCompFiltering("pesProduto", data.retorno.produtos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', MASCULINO);
            if (data.retorno.usuarioSisProf == true) {
                criarOuCarregarCompFiltering("pesProfessor", data.retorno.professores, "", data.retorno.professores[0].cd_pessoa, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_pessoa');
                if (!eval(Master()))
                    dijit.byId("pesProfessor").set("disabled", true);
            } else if (hasValue(data.retorno.professores))
                criarOuCarregarCompFiltering("pesProfessor", data.retorno.professores, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_pessoa', MASCULINO);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemCurso', error);
    });
}

function loadTipoDesistencia(items) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbMotivo = dijit.byId('pesMotivo');
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_motivo_desistencia, name: value.dc_motivo_desistencia });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbMotivo.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion

//#region - montarLegenda - formatCheckBox
function montarLegenda() {
    dojo.ready(function () {
        try{
            var chart = new dojox.charting.Chart("chart");
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, "legend");
            //chart.addSeries("Aulas Efetuadas", [1], { fill: "#fefa77" }); //YellowRow
            chart.addSeries("Cancelamento", [1], { fill: "#bc4f4f" }); //RedRow
            chart.render();
            dijit.byId("legend").refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridDesistencia';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_desistencia", grid._by_idx[rowIndex].item.cd_desistencia);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_desistencia', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_desistencia', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  Montar a pesquisa
function montarDesistencia(permissoes) {
    //Criação da Grade de Desistencia
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
        "dijit/form/FilteringSelect",
        "dojo/data/ItemFileReadStore",
        "dijit/Dialog",
        "dijit/registry"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, FilteringSelect, ItemFileReadStore) {
        ready(function () {
            try {
                dijit.byId("pesCurso").set("disabled", false);
                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                getComponentesDesistencia();
                var cdProf = 0;
                xhr.get({
                    url: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    data = jQuery.parseJSON(data);
                    if (data.retorno != null && data.retorno.Professor != null && data.retorno.Professor.cd_pessoa > 0 && !data.retorno.Professor.id_coordenador) {
                        //dojo.byId("professor").value = data.retorno.no_fantasia;
                        //dijit.byId("professor").set("disabled", true);
                        cdProf = data.retorno.Professor.cd_pessoa;
                        if (eval(Master()))
                            cdProf = 0;
                        //dojo.byId('nome_usuario_professor').value = data.retorno.no_fantasia;
                    }
                    //dojo.byId('cd_usuario_professor').value = cdProf;

                    myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/Secretaria/getDesistenciaSearch?cd_turma=0&cd_aluno=0&cd_motivo_desistencia=0&cd_tipo=0&dta_ini=&dta_fim=&cd_produto=0&cd_professor=" + cdProf + "&cursos=",
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                            idProperty: "cd_desistencia"
                        }
                    ), Memory({ idProperty: "cd_desistencia" }));

                    var gridDesistencia = new EnhancedGrid({
                        //store: ObjectStore({ objectStore: myStore }),
                        store: ObjectStore({ objectStore: new Memory({ data: null }) }),
                        structure: [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                            { name: "Turma", field: "no_turma", width: "25%", styles: "min-width:80px;" },
                            { name: "Aluno", field: "no_pessoa", width: "23%", styles: "min-width:80px;" },
                            { name: "Telefone", field: "telefone", width: "10%", styles: "text-align: center; min-width:80px;" },
                            { name: "Motivo", field: "dc_motivo_desistencia", width: "15%", styles: "text-align: left;" },
                            { name: "Data", field: "dtaDesistencia", width: "10%", styles: "min-width:80px;text-align: center" },
                            { name: "Tipo", field: "dc_tipo", width: "10%", styles: "text-align: center;" }
                        ],
                        noDataMessage: msgNotRegEncFiltro,
                        plugins: {
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "17",
                                gotoButton: true,
                                maxPageStep: 4,
                                position: "button",
                                plugins: { nestedSorting: true }
                            }
                        }
                    }, "gridDesistencia");

                    gridDesistencia.on("StyleRow", function (row) {
                        var item = gridDesistencia.getItem(row.index);

                        if (hasValue(item) && item.id_tipo_desistencia == CANCELAMENTO)
                            row.customClasses += " Cancelado";
                        else row.customClasses += "";
                    });

                    // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridDesistencia.pagination.plugin._paginator.plugin.connect(gridDesistencia.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                        verificaMostrarTodos(evt, gridDesistencia, 'cd_desistencia', 'selecionaTodos');
                    });

                    gridDesistencia.canSort = function (col) { return Math.abs(col) != 1; };

                    gridDesistencia.startup();

                    gridDesistencia.on("RowDblClick", function (evt) {
                        try {
                            var idx = evt.rowIndex,
                               item = this.getItem(idx),
                               store = this.store;
                            showCarregando();
                            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                            if (!hasValue(dijit.byId('gridTituloDesistencia')))
                                montarCadastroDesistencia(function () {
                                    editarDesistenciaPartial(item, gridDesistencia, xhr, ready, Memory, FilteringSelect);
                                }, Permissoes)
                            else editarDesistenciaPartial(item, gridDesistencia, xhr, ready, Memory, FilteringSelect);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }, true);

                    montarLegenda();

                    //Botões de pesquisa da grid
                    new Button({
                        label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                try {
                                    var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                                    if (!hasValue(dijit.byId('gridTituloDesistencia')))
                                        montarCadastroDesistencia(function () {
                                            abrirDesistenciaPartial(xhr, ready, Memory, FilteringSelect);
                                        }, Permissoes)
                                    else abrirDesistenciaPartial(xhr, ready, Memory, FilteringSelect);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                    }, "novaDesistencia");

                    new Button({
                        label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function () {
                            try {
                                apresentaMensagem('apresentadorMensagem', null);
                                var cd_turma = hasValue(dojo.byId('cdTurma').value) ? parseInt(dojo.byId('cdTurma').value) : 0;
                                var cd_aluno = hasValue(dojo.byId('cdAluno').value) ? parseInt(dojo.byId('cdAluno').value) : 0;
                                var cd_motivo = hasValue(dijit.byId('pesMotivo').get('value')) ? parseInt(dijit.byId('pesMotivo').get('value')) : 0;
                                var cd_tipo = hasValue(dijit.byId('pesTipo').get('value')) ? parseInt(dijit.byId('pesTipo').get('value')) : 0;
                                var dta_ini = hasValue(dojo.byId('dtInicial').value) ? dojo.byId('dtInicial').value : '';
                                var dta_fim = hasValue(dojo.byId('dtFinal').value) ? dojo.byId('dtFinal').value : '';
                                var cd_produto = hasValue(dijit.byId('pesProduto').get('value')) ? parseInt(dijit.byId('pesProduto').get('value')) : 0;
                                var cd_professor = hasValue(dijit.byId('pesProfessor').get('value')) ? parseInt(dijit.byId('pesProfessor').get('value')) : 0;

                                xhr.get({
                                    url: Endereco() + "/api/secretaria/geturlrelatorioDesistencia?" + getStrGridParameters('gridDesistencia') + '&cd_turma=' + cd_turma +
                                        '&cd_aluno=' + cd_aluno + '&cd_motivo_desistencia=' + cd_motivo + "&cd_tipo=" + cd_tipo + '&dta_ini=' + dta_ini + '&dta_fim=' + dta_fim +
                                        "&cd_produto=" + cd_produto + "&cd_professor=" + cd_professor + "&cursos=" + dijit.byId('pesCurso').value,
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagemAluno', error);
                                });
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }
                    }, "relatorioDesistencia");

                    new Button({
                        label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                            apresentaMensagem('apresentadorMensagem', null);
                            pesquisarDesistencia(true);
                        }
                    }, "pesquisarDesistencia");
                    decreaseBtn(document.getElementById("pesquisarDesistencia"), '32px');


                    //Botão de pesquisa do aluno
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                        onClick: function () {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                                });
                            }
                            else
                                abrirAlunoFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                        }
                    }, "pesAluno");

                    new Button({
                        label: "Limpar", iconClass: '', disabled: true, type: "reset",
                        onClick: function () {
                            dojo.byId("cdAlunoPesTurma").value = 0;
                            dojo.byId("noAluno").value = "";
                            dijit.byId('limparAlunoPes').set("disabled", true);
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    }, "limparAlunoPes");

                    //Botão de pesquisa da turma
                    new Button({
                        label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                            dojo.byId("cdTurma").value = 0;
                            dojo.byId("nomeTurma").value = "";
                            dijit.byId('limparTurmaPes').set("disabled", true);
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    }, "limparTurmaPes");

                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                        onClick: function () {
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                montarGridPesquisaTurmaFK(function () {
                                    abrirTurmaFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                                });
                            else
                                abrirTurmaFK(xhr, ObjectStore, Memory, Cache, JsonRest);
                        }
                    }, "pesProTurmaFK");

                    // botões link de ações:

                    var menu = new DropDownMenu({ style: "height: 25px" });
                    var acaoEditar = new MenuItem({
                        label: "Editar",
                        onClick: function () {
                            eventoEditarDesistencia(gridDesistencia.itensSelecionados, true, xhr, ready, Memory, FilteringSelect);
                        }
                    });
                    menu.addChild(acaoEditar);

                    var acaoRemover = new MenuItem({
                        label: "Excluir",
                        onClick: function () {
                            eventoRemover(gridDesistencia.itensSelecionados, 'DeletarDesistencia(itensSelecionados)');
                        }
                    });
                    menu.addChild(acaoRemover);

                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "acoesRelacionadas",
                        dropDown: menu,
                        id: "acoesRelacionadas"
                    });
                    dom.byId("linkAcoesDesistencia").appendChild(button.domNode);

                    // Adiciona link de selecionados:
                    menu = new DropDownMenu({ style: "height: 25px" });
                    var menuTodosItens = new MenuItem({
                        label: "Todos Itens",
                        onClick: function () {
                            buscarTodosItens(gridDesistencia, 'todosItem', ['pesquisarDesistencia', 'relatorioDesistencia']); pesquisarDesistencia(false);
                        }
                    });
                    menu.addChild(menuTodosItens);

                    var menuItensSelecionados = new MenuItem({
                        label: "Itens Selecionados",
                        onClick: function () {
                            buscarItensSelecionados('gridDesistencia', 'selecionado', 'cd_desistencia', 'selecionaTodos', ['pesquisarDesistencia', 'relatorioDesistencia'], 'todosItens');
                        }
                    });
                    menu.addChild(menuItensSelecionados);

                    var button = new DropDownButton({
                        label: "Todos Itens",
                        name: "todosItens",
                        dropDown: menu,
                        id: "todosItens"
                    });
                    dom.byId("linkSelecionadosDesistencia").appendChild(button.domNode);

                    //Altera o tamanho dos botões
                    var buttonFkArray = ['pesProTurmaFK', 'pesAluno'];
                    diminuirBotoes(buttonFkArray);

                    if (hasValue(document.getElementById("limparTurmaPes"))) {
                        document.getElementById("limparTurmaPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparTurmaPes").parentNode.style.width = '40px';
                    }

                    if (hasValue(document.getElementById("limparAlunoPes"))) {
                        document.getElementById("limparAlunoPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPes").parentNode.style.width = '40px';
                    }

                    //Carregando filters
                    criarOuCarregarCompFiltering('pesTipo', TIPO, "width: 100%;", null, ready, Memory, FilteringSelect, 'id', 'name', TODAS);

                    //métodos de change
                    if (hasValue(dijit.byId("menuManual"))) {
                        dijit.byId("menuManual").on("click",
                            function(e) {
                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323058', '765px', '771px');
                            });
                    }
                    adicionarAtalhoPesquisa(['pesMotivo', 'pesTipo', 'dtInicial', 'dtFinal'], 'pesquisarDesistencia', ready);
                    dijit.byId("pesProduto").on("change", function (event) {
                        try {
                            if (!hasValue(event) || event < TODOS)
                                dijit.byId("pesProduto").set("value", TODOS);
                            configuraCursoPorProdutoTurma(event);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("pesProfessor").on("change", function (event) {
                        try {
                            if (!hasValue(event) || event < TODOS)
                                dijit.byId("pesProfessor").set("value", TODOS);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    showCarregando();
                }, function (exception) {
                    showCarregando();

                    myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                            idProperty: "cd_desistencia"
                        }
                    ), Memory({ idProperty: "cd_desistencia" }));

                    var gridDesistenciaException = new EnhancedGrid({
                        store: ObjectStore({ objectStore: myStore }),
                        structure: [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                            { name: "Turma", field: "no_turma", width: "25%", styles: "min-width:80px;" },
                            { name: "Aluno", field: "no_pessoa", width: "23%", styles: "min-width:80px;" },
                            { name: "Telefone", field: "telefone", width: "10%", styles: "text-align: center; min-width:80px;" },
                            { name: "Motivo", field: "dc_motivo_desistencia", width: "15%", styles: "text-align: left;" },
                            { name: "Data", field: "dtaDesistencia", width: "10%", styles: "min-width:80px;text-align: center" },
                            { name: "Tipo", field: "dc_tipo", width: "10%", styles: "text-align: center;" }
                        ],
                        noDataMessage: "Nenhum registro encontrado.",
                        plugins: {
                            pagination: {
                                pageSizes: ["17", "34", "68", "100", "All"],
                                description: true,
                                sizeSwitch: true,
                                pageStepper: true,
                                defaultPageSize: "17",
                                gotoButton: true,
                                maxPageStep: 4,
                                position: "button",
                                plugins: { nestedSorting: true }
                            }
                        }
                    }, "gridDesistencia");
                    gridDesistenciaException.startup();

                    //Botões de pesquisa da grid
                    new Button({
                        label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF'
                    }, "novaDesistencia");

                    new Button({
                        label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage'                        
                    }, "relatorioDesistencia");

                    new Button({
                        label: "", iconClass: 'dijitEditorIconSearchSGF'
                    }, "pesquisarDesistencia");
                    decreaseBtn(document.getElementById("pesquisarDesistencia"), '32px');


                    //Botão de pesquisa do aluno
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "pesAluno");

                    new Button({
                        label: "Limpar", iconClass: '', disabled: true, type: "reset"
                    }, "limparAlunoPes");

                    //Botão de pesquisa da turma
                    new Button({
                        label: "Limpar", iconClass: '', disabled: true, type: "reset"
                    }, "limparTurmaPes");

                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "pesProTurmaFK");

                    //Altera o tamanho dos botões
                    var buttonFkArray = ['pesProTurmaFK', 'pesAluno'];
                    diminuirBotoes(buttonFkArray);

                    if (hasValue(document.getElementById("limparTurmaPes"))) {
                        document.getElementById("limparTurmaPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparTurmaPes").parentNode.style.width = '40px';
                    }

                    if (hasValue(document.getElementById("limparAlunoPes"))) {
                        document.getElementById("limparAlunoPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPes").parentNode.style.width = '40px';
                    }

                });
            }
            catch (e) {
                postGerarLog(e);
            }

        });
    });
};

function configuraCursoPorProdutoTurma(evt) {
    try {
        var pesCurso = dijit.byId("pesCurso");
        pesCurso.set("disabled", false);
        //Limpar componente multi
        dijit.byId("pesCurso")._onChangeActive = false;
        var store = new dojo.data.ItemFileReadStore({
            data: {
                identifier: "cd_curso",
                label: "no_curso",
                items: []
            }
        });
        dijit.registry.byId('pesCurso').setStore(store, []);
        dijit.byId("pesCurso")._onChangeActive = true;
        if (hasValue(evt)) {
            carregarCursoPorProdutoTurma(evt);
        } else 
            pesCurso.set("disabled", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarCursoPorProdutoTurma(cd_produto) {
    showCarregando();
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/curso/getPesCursos?hasDependente=5&cd_curso=&cd_produto=" + cd_produto,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            //data = jQuery.parseJSON(data);
            if (hasValue(data.retorno) && data.retorno.length > 0) {
                dijit.byId("pesCurso")._onChangeActive = false;
                var cursoCb = [];
                $.each(data.retorno, function (index, val) {
                    cursoCb.push({
                        "cd_curso": val.cd_curso + "",
                        "no_curso": val.no_curso
                    });
                });
                var store = new dojo.data.ItemFileReadStore({
                    data: {
                        identifier: "cd_curso",
                        label: "no_curso",
                        items: cursoCb
                    }
                });
                dijit.byId("pesCurso").setStore(store, []);
                dijit.byId("pesCurso")._onChangeActive = true;
                showCarregando();
            }
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagemTurma', error);
    });
}

//#endregion

//#region -- FKS -abrirTurmaFK() - retornarTurmaFK - pesquisarDesistencia - pesquisaTurmaAlunoDesistente

function abrirTurmaFK(xhr, ObjectStore, Memory, Cache, JsonRest) {
    try{
        dojo.byId("trAluno").style.display = "none";
        //Configuração retorno fk dentro da tela de desistencia.
        dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value = DESISTENCIA_PESQUISA;
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = DESISTENCIA;
        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId("pesProfessorFK").value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else {
                dijit.byId('pesProfessorFK').set('disabled', false);
            }
        }
        dijit.byId('pesTurmasFilhasFK').set('checked', true);
        dijit.byId('pesTurmasFilhasFK').set('disabled', true);
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisaTurmaAlunoDesistente(xhr, ObjectStore, Memory, Cache, JsonRest);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKPesqDesistencia() {
    try{
        if (hasValue(dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value) && dojo.byId("idOrigenPesquisaTurmaFKDesistencia").value == DESISTENCIA_PESQUISA) {
            var valido = true;
            var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
            if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
                if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                valido = false;
            }
            else {
                dojo.byId("cdTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("nomeTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparTurmaPes').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK(xhr, ObjectStore, Memory, Cache, JsonRest) {
    try{
        dojo.byId('tipoRetornoAlunoFK').value = PESQUISADESISTENCIA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisaAlunoDesistente();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoFK() {
    try{
        if (hasValue(dojo.byId("tipoRetornoAlunoFK").value) && dojo.byId("tipoRetornoAlunoFK").value == PESQUISADESISTENCIA) {
            var valido = true;
            var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
            if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
                if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

                if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                valido = false;
            }
            else {
                dojo.byId("cdAlunoPesTurma").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                dijit.byId('limparAlunoPes').set("disabled", false);
            }
            if (!valido)
                return false;
            dijit.byId("proAluno").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaTurmaAlunoDesistente(xhr, ObjectStore, Memory, Cache, JsonRest) {
    try{
        var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
        var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
        var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
        var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
        var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
        var situacaoFK = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
        var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/turma/getTurmaAlunoDesistente?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value + "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + dijit.byId("tipoTurmaFK").value +
                                        "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + situacaoFK + "&cdProfessor=" + cdProfessor + "&prog=" + cdProg +
                                        "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&dtInicial=&dtFinal=",
                handleAs: "json",
                preventCache: true,
                headers: { "Accept": "application/json", "Authorization": Token() }
            }), Memory({}));
        var dataStore = new ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridPesquisaTurmaFK");
        grid.itensSelecionados = [];

        if (dijit.byId("tipoTurmaFK").get('value') == PPT) {
            grid.layout.setColumnVisibility(2, true)
            grid.layout.setColumnVisibility(3, false)
            grid.turmasFilhas = true;
        }
        else {
            grid.layout.setColumnVisibility(2, false)
            grid.layout.setColumnVisibility(3, true)
            grid.turmasFilhas = false;
        }
        grid.setStore(dataStore);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarDesistencia(limparItens) {
    try{
        var cd_turma = hasValue(dojo.byId('cdTurma').value) ? parseInt(dojo.byId('cdTurma').value) : 0;
        var cd_aluno = hasValue(dojo.byId('cdAlunoPesTurma').value) ? parseInt(dojo.byId('cdAlunoPesTurma').value) : 0;
        var cd_motivo = hasValue(dijit.byId('pesMotivo').get('value')) ? parseInt(dijit.byId('pesMotivo').get('value')) : 0;
        var cd_tipo = hasValue(dijit.byId('pesTipo').get('value')) ? parseInt(dijit.byId('pesTipo').get('value')) : 0;
        var dta_ini = hasValue(dojo.byId('dtInicial').value) ? dojo.byId('dtInicial').value : '';
        var dta_fim = hasValue(dojo.byId('dtFinal').value) ? dojo.byId('dtFinal').value : '';
        var cd_produto = hasValue(dijit.byId('pesProduto').get('value')) ? parseInt(dijit.byId('pesProduto').get('value')) : 0;
        var cd_professor = hasValue(dijit.byId('pesProfessor').get('value')) ? parseInt(dijit.byId('pesProfessor').get('value')) : 0;
        require([
                 "dojo/store/JsonRest",
                 "dojo/data/ObjectStore",
                 "dojo/store/Cache",
                 "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/Secretaria/getDesistenciaSearch?cd_turma=" + cd_turma + '&cd_aluno=' + cd_aluno + '&cd_motivo_desistencia=' + cd_motivo + "&cd_tipo=" + cd_tipo + '&dta_ini=' + dta_ini +
                        '&dta_fim=' + dta_fim + "&cd_produto=" + cd_produto + "&cd_professor=" + cd_professor + "&cursos=" + dijit.byId('pesCurso').value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_desistencia"
                }), Memory({ idProperty: "cd_desistencia" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridDesistencia = dijit.byId("gridDesistencia");
            if (limparItens) {
                gridDesistencia.itensSelecionados = [];
            }
            gridDesistencia.noDataMessage = msgNotRegEnc;
            gridDesistencia.setStore(dataStore);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaAlunoDesistente() {
    try{
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var myStore = dojo.store.Cache(
                 dojo.store.JsonRest({
                     target: Endereco() + "/api/aluno/getAlunoDesistente?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked + "&cdSituacao=0&sexo=" + sexo,
                     handleAs: "json",
                     headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                     idProperty: "cd_aluno"
                 }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region - keepValues - eventos para links


//funão para edição no link

function eventoEditarDesistencia(itensSelecionados, ehLink, xhr, ready, Memory, FilteringSelect) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            getLimpar('#formDesistencia');
            apresentaMensagem('apresentadorMensagem', '');
            var gridDesistencia = dijit.byId('gridDesistencia');
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            if (!hasValue(dijit.byId('gridTituloDesistencia')))
                montarCadastroDesistencia(function () {
                    editarDesistenciaPartial(null, gridDesistencia, xhr, ready, Memory, FilteringSelect);
                }, Permissoes)
            else editarDesistenciaPartial(null, gridDesistencia, xhr, ready, Memory, FilteringSelect);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirDesistenciaPartial() {
    try{
        limparCadastro();
        loadTipoNovo();
        dijit.byId('pesTurmaFKCadDesistencia').set('disabled', false);
        dijit.byId('pesAlunoFKCad').set("disabled", false);
        selecionaTab('span#tabContainer_tablist_tabPrincipal.tabLabel');
        abrirCadastroDesistencia();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion
