var SOBE = 1, DESCE = 2;
var alterarOrd = 0, ordemOri = 0;

// inicio da formatação da Avaliacao
function formatCheckBoxAvaliacao(value, rowIndex, obj) {
    try{
        var gridName = 'gridAvaliacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAvaliacao');t

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_avaliacao", grid._by_idx[rowIndex].item.cd_avaliacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_avaliacao', 'selecionadoAvaliacao', -1, 'selecionaTodosAvaliacao', 'selecionaTodosAvaliacao', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_avaliacao', 'selecionadoAvaliacao', " + rowIndex + ", '" + id + "', 'selecionaTodosAvaliacao', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// inicio da formatação da Avaliacao
function formatCheckBoxOrdenacao(value, rowIndex, obj) {
    try{
        var gridName = 'gridOrdenacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_ordem_avaliacao", grid._by_idx[rowIndex].item.nm_ordem_avaliacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_ordem_avaliacao', 'selecionadoOrdenacao', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'nm_ordem_avaliacao', 'selecionadoOrdenacao', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//restaura dados
function keepValues(value, grid, ehLink) {
    try{
        var valorCacelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true)) {
            ehLink = eval(linkAnterior.value);
        }
        linkAnterior.value = ehLink;

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length== 1 && ehLink)
            value = grid.itensSelecionados[0];

        //Quando for cancelamento
        if (!hasValue(value) && hasValue(valorCacelamento) && !ehLink) {
            value = valorCacelamento[0];
        }

        clearForm('#formAvaliacao');
        getLimpar('#formAvaliacao');
        dojo.byId("cd_avaliacao").value = value.cd_avaliacao;
        if (hasValue(dojo.byId('tipoAvaliacao').value, true)) {
            hasValue(value.cd_tipo_avaliacao) ? dijit.byId("tipoAvaliacao").set("value", value.cd_tipo_avaliacao) : 0;
        }
        if (hasValue(dojo.byId('criterioAvaliacao').value, true)) {
            hasValue(value.cd_criterio_avaliacao) ? dijit.byId("criterioAvaliacao").set("value", value.cd_criterio_avaliacao) : 0;
        }
        dojo.byId("id_avaliacao_ativa").value = value.id_avaliacao_ativa == true ? dijit.byId("id_avaliacao_ativa").set("value", true) : dijit.byId("id_avaliacao_ativa").set("value", false);
        dojo.byId("numOrdem").value = value.nm_ordem_avaliacao;
        dojo.byId("peso").value = value.nm_peso_avaliacao;
        dojo.byId("nota").value = value.vl_nota;
        loadGridAvaliacaoOrdem(dijit.byId("tipoAvaliacao").value, dijit.byId("criterioAvaliacao").value, document.getElementById("inicioAvaliacao").checked);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//restaura dados
function keepValuesAval(value, grid, ehLink) {
    try{
        var valorCacelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');

        if (!hasValue(ehLink, true)) {
            ehLink = eval(linkAnterior.value);
        }
        linkAnterior.value = ehLink;

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        //Quando for cancelamento
        if (!hasValue(value) && hasValue(valorCacelamento) && !ehLink) {
            value = valorCacelamento[0];
        }

        dojo.byId("cd_avaliacao").value = value.cd_avaliacao;

        dojo.byId("id_avaliacao_ativa").value = value.id_avaliacao_ativa == true ? dijit.byId("id_avaliacao_ativa").set("value", true) : dijit.byId("id_avaliacao_ativa").set("value", false);
        dojo.byId("numOrdem").value = value.nm_ordem_avaliacao;
        dojo.byId("peso").value = value.nm_peso_avaliacao;
        dojo.byId("nota").value = value.vl_nota;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Popula os fieltering
//Métodos para carregar o tipo de avaliação
function populaTipoAvaliacao(field, ativo) {
    // Popula os produtos:
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getTipoAvaliacao",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            loadTipoAvaliacao(jQuery.parseJSON(data).retorno, field);
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCurso', error);
        });
    });
}


function loadTipoAvaliacao(items, linkTipo) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbTipo = dijit.byId(linkTipo);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_tipo_avaliacao, name: value.dc_tipo_avaliacao });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipo.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}//fim Tipo de avaliação

//Métodos para carregar o Critério
function populaCriterio(field, ativo) {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getAvaliacaoCriterio",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataCriterio) {
            loadCriterio(jQuery.parseJSON(dataCriterio).retorno, field);
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCurso', error);
        });
    });
}

function loadCriterio(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbCriterio = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_criterio_avaliacao, name: value.dc_criterio_avaliacao });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbCriterio.store = stateStore;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
// Fim do criterio

//Criação da Grade de Avaliacao
function montarCadastroAvalicao() {
    require([
          "dojo/_base/xhr",
          "dojo/dom",
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
          "dojo/domReady!",
          "dojo/parser",
          "dijit/Dialog"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button,
                 ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try{
                montarStatus("statusAvaliacao");
                populaTipoAvaliacao("cbTipo", null);
                populaCriterio("cbCriterio", null);
                myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getAvaliacaoSearch?desc=&idTipoAvaliacao=&idCriterio=&inicio=false&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_avaliacao"
                }), Memory({ idProperty: "cd_avaliacao" }));
                dijit.byId("cbTipo").set("required", false);
                dijit.byId("cbCriterio").set("required", false);
                var gridAvaliacao = new EnhancedGrid({

                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosAvaliacao' style='display:none'/>", field: "selecionadoAvaliacao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAvaliacao },
                       // { name: "Código", field: "cd_avaliacao", width: "5%", styles: "text-align: right; min-width:60px; max-width:75px;" },
                        { name: "Tipo", field: "dc_tipo_avaliacao", width: "40%", minwidth: "35%" },
                        { name: "Critério", field: "dc_criterio_avaliacao", width: "40%", minwidth: "35%" },
                        { name: "Nota", field: "vl_nota", width: "10%", minwidth: "10%" },
                        { name: "Peso", field: "peso", width: "5%", minwidth: "10%" },
                        { name: "Ordem", field: "ordem", width: "5%", styles: "text-align: center;", minwidth: "10%" }
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
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridAvaliacao");
                gridAvaliacao.pagination.plugin._paginator.plugin.connect(gridAvaliacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridAvaliacao, 'cd_avaliacao', 'selecionaTodosAvaliacao');
                });
                gridAvaliacao.canSort = function (col) { return Math.abs(col) != 1; };
                gridAvaliacao.startup();
                gridAvaliacao.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                        apresentaMensagem('apresentadorMensagem', null);
                        xhr.get({
                            url: Endereco() + "/api/coordenacao/getAllTipoAvaliacao?ativo=true&cdTipo=" + item.cd_tipo_avaliacao,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            xhr.get({
                                url: Endereco() + "/api/coordenacao/getAllCriteriosAtivos?ativo=true&cdCriterio"+ item.cd_criterio_avaliacao,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (dataCriterio) {
                                loadCriterio(jQuery.parseJSON(dataCriterio).retorno, "criterioAvaliacao");
                                loadTipoAvaliacao(jQuery.parseJSON(data).retorno, "tipoAvaliacao");
                            });
                        });

                        dijit.byId("gridOrdenacao").itensSelecionados = [];
                        keepValues(item, gridAvaliacao, false);
                        dijit.byId("tipoAvaliacao").set("disabled", true);
                        dijit.byId("criterioAvaliacao").set("disabled", true);
                        dijit.byId("cadAvaliacao").show();
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                }, true);

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        dijit.byId("tipoAvaliacao").set("disabled", true);
                        dijit.byId("criterioAvaliacao").set("disabled", true);
                        eventoEditarAvaliacao(gridAvaliacao.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        eventoRemover(gridAvaliacao.itensSelecionados, 'deletarAvaliacao(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dojo.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridAvaliacao, 'todosItens', ['pesquisarAvaliacao', 'relatorioAvaliacao']);
                        searchAvaliacao(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridAvaliacao', 'selecionadoAvaliacao', 'cd_avaliacao', 'selecionaTodosAvaliacao', ['pesquisarAvaliacao', 'relatorioAvaliacao'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            //incluiAvaliacaoGrid();
                            alterarOrd = 0;
                            dijit.byId("dialogAval").show();
                            document.getElementById("cd_avaliacao").value = '';
                            document.getElementById("nota").value = "000";
                            document.getElementById("peso").value = "1,00";
                            dijit.byId("id_avaliacao_ativa").set("value", true);
                            if (dijit.byId("gridOrdenacao")._by_idx.length > 0)
                                document.getElementById("numOrdem").value = dijit.byId("gridOrdenacao")._by_idx[0].item.nm_ordem_avaliacao + 1;
                            else
                                document.getElementById("numOrdem").value = 1;
                            document.getElementById('divCancelarAval').style.display = "none";
                            document.getElementById('divLimparAval').style.display = "";
                            dojo.byId("incluirAval_label").innerHTML = "Incluir";
                        }
                        catch (er) {
                            postGerarLog(er);
                        }
                    }
                }, "incluirAvaliacao");
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        if (alterarOrd == 1 || dojo.byId("cd_avaliacao").value > 0)
                            alterarAvaliacaoGrid(dijit.byId("gridOrdenacao"));
                        else
                            incluiAvaliacaoGrid();
                        dijit.byId("dialogAval").hide();
                    }
                }, "incluirAval");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        try{
                            document.getElementById("nota").value = "000";
                            document.getElementById("peso").value = "1,00";
                            dijit.byId("id_avaliacao_ativa").set("value", true);
                            if(dijit.byId("gridOrdenacao")._by_idx.length > 0)
                                document.getElementById("numOrdem").value = dijit.byId("gridOrdenacao")._by_idx[0].item.nm_ordem_avaliacao + 1;
                            else
                                document.getElementById("numOrdem").value = 1;
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAval");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValuesAval(null, dijit.byId("gridOrdenacao"), false);
                    }
                }, "cancelarAval");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("dialogAval").hide();
                    }
                }, "fecharAval");
                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        alterarAvaliacaoGrid(dijit.byId("gridOrdenacao"));
                    }
                }, "alterarAvaliacao");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        dijit.byId("tipoAvaliacao").reset();
                        dijit.byId("criterioAvaliacao").reset();
                        loadGridAvaliacaoOrdem(0, 0, 0);
                        document.getElementById('ordemOri').value = "";
                        getLimpar('#formAvaliacao');
                        clearForm('formAvaliacao');
                    }
                }, "limparAvaliacao");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValues(null, gridAvaliacao, false);
                    }
                }, "cancelarAvaliacao");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadAvaliacao").hide();
                    }
                }, "fecharAvaliacao");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        searchAvaliacao(true);
                    }
                }, "pesquisarAvaliacao");
                btnPesquisar(document.getElementById("pesquisarAvaliacao"));
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            dijit.byId("incluirAvaliacao").set("disabled", true);
                            document.getElementById("cd_avaliacao").value = "";
                            dijit.byId("criterioAvaliacao").reset();
                            dijit.byId("tipoAvaliacao").reset();
                            xhr.get({
                                url: Endereco() + "/api/coordenacao/getAllTipoAvaliacao?ativo=true&cdTipo=0",
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                xhr.get({
                                    url: Endereco() + "/api/coordenacao/getAllCriteriosAtivos?ativo=true&cdCriterio=",
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (dataCriterio) {
                                    loadCriterio(jQuery.parseJSON(dataCriterio).retorno, "criterioAvaliacao");
                                    loadTipoAvaliacao(jQuery.parseJSON(data).retorno, "tipoAvaliacao");
                                });
                            });
                            document.getElementById('numOrdem').value = "";
                            document.getElementById("cd_avaliacao").value = "";
                            apresentaMensagem('apresentadorMensagemAvaliacao', null);
                            dijit.byId("tipoAvaliacao").set("disabled", false);
                            dijit.byId("criterioAvaliacao").set("disabled", false);
                            dijit.byId("cadAvaliacao").show();
                            clearForm("formAvaliacao");
                            loadGridAvaliacaoOrdem(0, 0, 0);
                            alterarOrd = 0;
                        }
                        catch (er) {
                            postGerarLog(er);
                        }
                    }
                }, "novaAvaliacao");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        require(["dojo/_base/xhr"], function (xhr) {
                            try{
                                var requisicao = "/api/coordenacao/geturlrelatorioavaliacao?" + getStrGridParameters('gridAvaliacao') + "desc=";
                               // var abreviatura = document.getElementById("descAbreviatura").value;
                                var criterio = hasValue(dijit.byId("cbCriterio").value) ? dijit.byId("cbCriterio").value : 0;
                                var tipoAvl = hasValue(dijit.byId("cbTipo").value) ? dijit.byId("cbTipo").value :  0;
                                var inicio = document.getElementById("inicioAvaliacao").checked;
                                xhr.get({
                                    url: Endereco() + requisicao + "&idTipoAvaliacao=" + tipoAvl + "&idCriterio=" + criterio + "&inicio=" + inicio + "&status=" + retornaStatus("statusAvaliacao"),
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                            }
                            catch (er) {
                                postGerarLog(er);
                            }
                        })
                    }
                }, "relatorioAvaliacao");
                // Botoes auxiliares para ordenação  no cadastro de avaliação
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                         salvarAvaliacao();
                    }
                }, "gravarAvaliacao");
                //fim botões auxiliares
                btnPesquisar(dojo.byId("pesquisarAvaliacao"));
                xhr.get({
                    url: Endereco() + "/api/coordenacao/getTipoAvaliacao",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    xhr.get({
                        url: Endereco() + "/api/coordenacao/getAvaliacaoCriterio",
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (dataCriterio) {
                        loadCriterio(jQuery.parseJSON(dataCriterio).retorno, "criterioAvaliacao");
                        loadTipoAvaliacao(jQuery.parseJSON(data).retorno, "tipoAvaliacao");
                    });
                });
                montarGridOrdem();
                var t, c;
                dijit.byId("id_avaliacao_ativa").on("click", function (e) {
                    if (this.checked == true) 
                        document.getElementById('numOrdem').value = hasValue(dijit.byId("gridOrdenacao")._by_idx[0]) ? dijit.byId("gridOrdenacao")._by_idx[0].item.nm_ordem_avaliacao + 1 : 1;
                    else
                        document.getElementById('numOrdem').value = 0;
                })

                dijit.byId("tipoAvaliacao").on("change", function (t) {
                    this.t = t;
                    c = dijit.byId("criterioAvaliacao").value;
                    if (t > 0 && c > 0) {
                        loadGridAvaliacaoOrdem(t, c, document.getElementById("inicioAvaliacao").checked);
                        dijit.byId("incluirAvaliacao").set("disabled", false);
                        dijit.byId("gridOrdenacao").itensSelecionados = [];
                    }
                });

                dijit.byId("criterioAvaliacao").on("change", function (c) {
                    t = dijit.byId("tipoAvaliacao").value;
                    this.c = c;
                    if (t > 0 && c > 0) {
                        loadGridAvaliacaoOrdem(t, c, document.getElementById("inicioAvaliacao").checked);
                        dijit.byId("incluirAvaliacao").set("disabled", false);
                        dijit.byId("gridOrdenacao").itensSelecionados = [];
                    }
                });
                showCarregando();

                // Adiciona link de ações:
                var menuAval = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarOrdenacao(); }
                });
                menuAval.addChild(acaoExcluir);


                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasAval",
                    dropDown: menuAval,
                    id: "acoesRelacionadasAval"
                });

                dom.byId("linkAcoesAval").appendChild(button.domNode);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
        if (hasValue(dijit.byId("menuManual"))) {
            dijit.byId("menuManual").on("click",
                function(e) {
                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc417983826', '765px', '771px');
                });
        }
    });
};
// ** fim da grade de Avaliação **\\

function montarGridOrdem() {
    require([
        "dojox/grid/EnhancedGrid",
        "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dijit/Dialog"
    ], function (EnhancedGrid, ObjectStore, Memory, Button) {
        try{
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
            var gridOrdenacao = new EnhancedGrid({
                store: dataStore,
                structure: [
                    { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoOrdenacao", width: "9%", styles: "text-align: center;", formatter: formatCheckBoxOrdenacao },
                   // { name: "Código", field: "cd_avaliacao", width: "10%", styles: "min-width:40px; text-align: left;" },
                    { name: "Tipo", field: "dc_tipo_avaliacao", width: "23%", styles: "min-width:40px; text-align: left;" },
                    { name: "Critério", field: "dc_criterio_avaliacao", width: "23%", styles: "min-width:40px; text-align: left;" },
                  //  { name: "Abreviatura", field: "dc_criterio_abreviado", width: "15%", styles: "min-width:40px; text-align: left;"},
                    { name: "Nota", field: "vl_nota", width: "10%", styles: "min-width:40px; text-align: left;" },
                    { name: "Peso", field: "nm_peso_avaliacao", width: "10%", styles: "min-width:40px; text-align: left;" },
                    { name: "Ordem", field: "nm_ordem_avaliacao", width: "10%", styles: "min-width:50px; text-align: left;" },
                    { name: "Ativa", field: "avaliacao_ativa", width: "10%", styles: "min-width:50px; text-align: left;" }
                ],
                selectionMode: "single",
                canSort: false,
                noDataMessage: msgNotRegEnc// msgNotRegEstag

            }, "gridOrdenacao");
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridOrdenacao, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_avaliacao', 'selecionadoOrdenacao', -1, 'selecionaTodos', 'selecionaTodos', 'gridOrdenacao')", gridOrdenacao.rowsPerPage * 3);
                });
            });
            gridOrdenacao.canSort = function () { return false };
            gridOrdenacao.startup();
            gridOrdenacao.on("RowDblClick", function (evt) {
                try{
                    dijit.byId("dialogAval").show();
                    var idx = evt.rowIndex,
                       item = this.getItem(idx),
                       store = this.store;
                    document.getElementById("cd_avaliacao").value = item.cd_avaliacao,
                    document.getElementById("nota").value = item.vl_nota;
                    document.getElementById("peso").value = item.nm_peso_avaliacao;
                    dijit.byId("id_avaliacao_ativa").set("value", item.id_avaliacao_ativa);
                    document.getElementById("numOrdem").value = item.nm_ordem_avaliacao;
                    alterarOrd = 1;
                    ordemOri = item.nm_ordem_avaliacao;
                    document.getElementById('divCancelarAval').style.display = "";
                    document.getElementById('divLimparAval').style.display = "none";
                    dojo.byId("incluirAval_label").innerHTML = "Alterar";
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, true);
            gridOrdenacao.rowsPerPage = 5000;

            if (gridOrdenacao._by_idx == 0) {
                document.getElementById('divAvaliacaoCima').style.display = "none";
                document.getElementById('divAvaliacaoBaixo').style.display = "none";
                document.getElementById('divAvaliacaoGravar').style.display = "none";
                document.getElementById('divCancelarAvaliacao').style.display = "none";
            }
            else {
                document.getElementById('divAvaliacaoCima').style.display = "";
                document.getElementById('divAvaliacaoBaixo').style.display = "";
                document.getElementById('divAvaliacaoGravar').style.display = "";
                document.getElementById('divCancelarAvaliacao').style.display = "";
            }

            if (!hasValue(dijit.byId('subir'))) {
                new Button({
                    label: "Subir", iconClass: 'dijitEditorIcon dijitEditorIconTabUp',
                    onClick: function () {
                        subirDescerOrdemAvaliacao(gridOrdenacao, SOBE);
                    }
                }, "subir");
                new Button({
                    label: "Descer", iconClass: 'dijitEditorIcon dijitEditorIconTabDown',
                    onClick: function () {
                        //descerOrdemAvaliacao(gridOrdenacao);
                        subirDescerOrdemAvaliacao(gridOrdenacao, DESCE);
                    }
                }, "descer");
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}
function subirDescerOrdemAvaliacao(grid, sobeDesce) {
    try{
        var operacao = sobeDesce == SOBE ? 1 : -1;

        //var itemSelecionado = grid.selection.getSelected();
        var itemSelecionado = grid.itensSelecionados;


        if (itemSelecionado.length > 1)
            caixaDialogo(DIALOGO_AVISO, 'Selecione apenas um registro.', null);
        else
            if (itemSelecionado.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgSelectRegOrdem, null);
            else {
                for (var l = 0; l < grid._by_idx.length; l++)
                    if (grid._by_idx[l].item.nm_ordem_avaliacao == itemSelecionado[0].nm_ordem_avaliacao) {
                        if (hasValue(grid._by_idx[l - (operacao)])) {
                            var itemEncontrado = grid._by_idx[l].item;
                            var ordemEncontrada = grid._by_idx[l].item.nm_ordem_avaliacao;
                            var posicaoEncontrada = grid.selection.selectedIndex;

                            // Muda as ordens de lugares:
                            grid._by_idx[l].item.nm_ordem_avaliacao = grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao;
                            grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao = ordemEncontrada;

                            // Muda os itens de lugares:
                            grid._by_idx[l].item = grid._by_idx[l - (operacao)].item;
                            grid._by_idx[l - (operacao)].item = itemEncontrado;

                            grid.update();

                            grid.getRowNode(l).id = '';
                            grid.getRowNode(l - (operacao)).id = 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao;
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                            window.location.hash = '#' + 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao;

                            // Atualiza o item selecionado:
                            grid.selection.setSelected(posicaoEncontrada, false);
                            if (posicaoEncontrada < grid._by_idx.length)
                                grid.selection.setSelected(posicaoEncontrada - 1, true);
                            var codAlteradoSubir = grid._by_idx[l].item.nm_ordem_avaliacao + ";" + grid._by_idx[l - (operacao)].item.nm_ordem_avaliacao + ";";
                        }
                        return codAlteradoSubir;
                        break;
                    }
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Método para retornar Ordem
function loadGridAvaliacaoOrdem(idTipoAvaliacao, idCriterio, ativo) {
    ativo = true ? 1 : 0;
    apresentaMensagem('apresentadorMensagemAvaliacao', null);
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getAvaliacaoOrdem?idTipoAvaliacao=" + idTipoAvaliacao + "&idCriterio=" + idCriterio + "&status=&tipoAvaliacao=" + encodeURIComponent(dojo.byId("tipoAvaliacao").value) + "&criterio=" + encodeURIComponent(dojo.byId("criterioAvaliacao").value),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() }
        }).then(function (dataAvaliacao) {
            try{
                //dataProd = jQuery.parseJSON(dataProd);
                if (dataAvaliacao.MensagensWeb.length > 0)
                    apresentaMensagem('apresentadorMensagemAvaliacao', dataAvaliacao);
                else
                    apresentaMensagem('apresentadorMensagemAvaliacao', null);
                var gridAvaliacaoOrdem = dijit.byId("gridOrdenacao");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataAvaliacao.retorno }) });

                if (hasValue(dataStore.objectStore.data[0])) {
                    document.getElementById('ordemOri').value = dataStore.objectStore.data[0].nm_ordem_avaliacao + 1;
                    document.getElementById('numOrdem').value = dataStore.objectStore.data[0].nm_ordem_avaliacao + 1;
                }
                else
                    if (hasValue(dijit.byId("tipoAvaliacao").value) && hasValue(dijit.byId("criterioAvaliacao").value)) {
                        document.getElementById('ordemOri').value = 1;
                        document.getElementById('numOrdem').value = 1;
                    }
                for (var i = 0 ; i < dataStore.objectStore.data.length; i++)
                    if (dataStore.objectStore.data[i].cd_avaliacao == document.getElementById('cd_avaliacao').value) {
                        document.getElementById('numOrdem').value = dataStore.objectStore.data[i].nm_ordem_avaliacao;
                        break;
                    }
                gridAvaliacaoOrdem.setStore(dataStore);
                if (gridAvaliacaoOrdem._by_idx == 0) {
                    document.getElementById('divAvaliacaoCima').style.display = "none";
                    document.getElementById('divAvaliacaoBaixo').style.display = "none";
                    document.getElementById('divAvaliacaoGravar').style.display = "none";
                    document.getElementById('divCancelarAvaliacao').style.display = "none";
                }
                else {
                    document.getElementById('divAvaliacaoCima').style.display = "";
                    document.getElementById('divAvaliacaoBaixo').style.display = "";
                    document.getElementById('divAvaliacaoGravar').style.display = "";
                    document.getElementById('divCancelarAvaliacao').style.display = "";

                    for (var i = 0; i <= gridAvaliacaoOrdem._by_idx.length; i++)
                        gridAvaliacaoOrdem.selection.setSelected(i, false);

                    for (var i = 0; i <= gridAvaliacaoOrdem._by_idx.length; i++) {
                        if (gridAvaliacaoOrdem._by_idx[i].item.cd_avaliacao + "" == dojo.byId('cd_avaliacao').value) {
                            //Seleciona a linha com o item editado por default:
                            gridAvaliacaoOrdem.selection.setSelected(i, true);
                            gridAvaliacaoOrdem.render();
                            //Posiciona o foco do scroll no item editado:
                            gridAvaliacaoOrdem.getRowNode(i).id = '';
                            gridAvaliacaoOrdem.getRowNode(i).id = 'ordem_' + gridAvaliacaoOrdem.cd_avaliacao;
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                            window.location.hash = '#' + 'ordem_' + gridAvaliacaoOrdem.cd_avaliacao;
                        }
                        if ((!hasValue(dojo.byId('no_Avaliacao').value))) {
                            dijit.byId('gridAvaliacaoOrdem').ClearSelection() = null;
                            gridAvaliacaoOrdem.getRowNode(i).id = '';
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                        }
                    }
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAvaliacao', error);
            var gridAvaliacaoOrdem = dijit.byId("gridOrdenacao");
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
            gridAvaliacaoOrdem.setStore(dataStore);
        });
    });
}

// ******************* Início da persistência da Avaliacao *******************\\

// Procura
function searchAvaliacao(limparItens) {
    var requisicao = "/api/coordenacao/getAvaliacaoSearch?desc=";
    //var abreviatura = document.getElementById("descAbreviatura").value;
    var criterio = dijit.byId("cbCriterio").value;
    var tipoAvl = dijit.byId("cbTipo").value;
    var inicio = document.getElementById("inicioAvaliacao").checked;
    require([
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            myStore = Cache(
            JsonRest({
                target: Endereco() + requisicao + "&idTipoAvaliacao=" + tipoAvl + "&idCriterio=" + criterio + "&inicio=" + inicio + "&status=" + retornaStatus("statusAvaliacao"),
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_avaliacao"
            }), Memory({ idProperty: "cd_avaliacao" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAvaliacao = dijit.byId("gridAvaliacao");
            if (limparItens) {
                gridAvaliacao.itensSelecionados = [];
            }
            gridAvaliacao.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};

// Método para incluir na grade
function incluiAvaliacaoGrid() {
    try{
        apresentaMensagem('apresentadorMensagemAvaliacao', null);
        if (!dijit.byId("dialogAval").validate())
            return false;
        var gridAvaliacaoOrdem = dijit.byId("gridOrdenacao");
        var cdAval = dojo.byId("cd_avaliacao").value == "" ? 0 : dojo.byId("cd_avaliacao").value;
        if (document.getElementById("cd_avaliacao").value <= 0) {
            if (hasValue(dijit.byId("tipoAvaliacao").value) && hasValue(dijit.byId("criterioAvaliacao").value)) {
            
                require([
                        "dojo/_base/xhr"
                ], function (xhr) {
                    showCarregando();
                    xhr.get({
                        url: Endereco() + "/api/coordenacao/getverificaTotalNota?idTipoAvaliacao=" + dijit.byId("tipoAvaliacao").value + "&idCriterio=" + dijit.byId("criterioAvaliacao").value + "&idAvaliacao=" + cdAval + "&nota=" + document.getElementById("nota").value + "&peso=" + dojo.number.parse(dom.byId("peso").value),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }).then(function (dataAvaliacao) {
                        try{
                            if (dataAvaliacao.MensagensWeb.length > 0)
                                apresentaMensagem('apresentadorMensagemAvaliacao', dataAvaliacao);
                            else {
                                apresentaMensagem('apresentadorMensagemAvaliacao', null);
                                document.getElementById('divAvaliacaoGravar').style.display = "";
                                var myNewStore = setStoreGrid();
                                if (myNewStore.id_avaliacao_ativa == false)
                                    myNewStore.nm_ordem_avaliacao = null;
                                gridAvaliacaoOrdem.store.newItem(myNewStore);
                                gridAvaliacaoOrdem.store.save();

                                for (var l = 0; l < gridAvaliacaoOrdem._by_idx.length ; l++)
                                    gridAvaliacaoOrdem._by_idx[l].nm_ordem_avaliacao = gridAvaliacaoOrdem._by_idx[l].item.nm_ordem_avaliacao;

                                quickSortObj(gridAvaliacaoOrdem._by_idx, 'nm_ordem_avaliacao');
                                gridAvaliacaoOrdem._by_idx.reverse();

                                for (var l = gridAvaliacaoOrdem._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                                    if (gridAvaliacaoOrdem._by_idx[l].item.nm_ordem_avaliacao > 0)
                                        gridAvaliacaoOrdem._by_idx[l].item.nm_ordem_avaliacao = j;
                                    else
                                        j = j - 1;

                                for (var i = 0; i < gridAvaliacaoOrdem._by_idx.length ; i++) {
                                    gridAvaliacaoOrdem.selection.setSelected(i, false);
                                    if (gridAvaliacaoOrdem._by_idx[i].item.nm_ordem_avaliacao == myNewStore.nm_ordem_avaliacao) {
                                        //Seleciona a linha com o item editado por default:
                                        gridAvaliacaoOrdem.selection.setSelected(i, true);
                                        //Posiciona o foco do scroll no item editado:
                                        setTimeout('atualiza(' + i + ')', 10);
                                    }
                                }
                                if (gridAvaliacaoOrdem._by_idx == 0) {
                                    document.getElementById('divAvaliacaoCima').style.display = "none";
                                    document.getElementById('divAvaliacaoBaixo').style.display = "none";
                                    document.getElementById('divAvaliacaoGravar').style.display = "none";
                                    document.getElementById('divCancelarAvaliacao').style.display = "none";
                                }
                                else {
                                    document.getElementById('divAvaliacaoCima').style.display = "";
                                    document.getElementById('divAvaliacaoBaixo').style.display = "";
                                    document.getElementById('divAvaliacaoGravar').style.display = "";
                                    document.getElementById('divCancelarAvaliacao').style.display = "";
                                }
						        showCarregando();
                            }
                        }
                        catch (er) {
                            postGerarLog(er);
                        }
                    }, function (error) {
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemAvaliacao', error);
                    });
                });

            } else {
                caixaDialogo(DIALOGO_AVISO, 'Selecione o Tipo e o Critério de Avaliação', null);
            }
        }
        else {
            for (var i = 0; i < gridAvaliacaoOrdem._by_idx.length; i++)
                if (gridAvaliacaoOrdem._by_idx[i].item.cd_avaliacao == dojo.byId("cd_avaliacao").value) {
                    gridAvaliacaoOrdem._by_idx[i].item.vl_nota = dojo.byId("nota").value;
                    gridAvaliacaoOrdem._by_idx[i].item.nm_peso_avaliacao = dojo.byId("peso").value;
                    gridAvaliacaoOrdem._by_idx[i].item.id_avaliacao_ativa = dojo.byId("id_avaliacao_ativa").checked;
                    gridAvaliacaoOrdem._by_idx[i].item.nm_ordem_avaliacao = parseInt(dojo.byId("numOrdem").value);
                    gridAvaliacaoOrdem._by_idx[i].item.ordem = dojo.byId("numOrdem").value;
                    break;
                }
            gridAvaliacaoOrdem.update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Marca na grade o registro inserido
function atualiza(i) {
    try{
        var grid = dijit.byId('gridOrdenacao');
        grid.getRowNode(i).id = '';
        grid.getRowNode(i).id = 'ordem_' + grid._by_idx[i].item.cd_avaliacao;
        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
        window.location.hash = '#ordem_' + grid._by_idx[i].item.cd_avaliacao;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setStoreGrid() {
    try{
        var myNewStore = {
            cd_avaliacao: hasValue(dojo.byId('cd_avaliacao').value) ? dojo.byId('cd_avaliacao').value : "",
            cd_tipo_avaliacao: dijit.byId("tipoAvaliacao").value,
            cd_criterio_avaliacao: dijit.byId("criterioAvaliacao").value,
            //dc_criterio_abreviado: dijit.byId("criterioAvaliacao").item.abr,
            dc_criterio_avaliacao: dojo.byId("criterioAvaliacao").value,
            dc_tipo_avaliacao: dojo.byId("tipoAvaliacao").value,
            avaliacao_ativa: document.getElementById('id_avaliacao_ativa').checked == true ? "Sim" : "Não",
            id_avaliacao_ativa: document.getElementById('id_avaliacao_ativa').checked,
            nm_ordem_avaliacao: parseInt(document.getElementById('numOrdem').value) > 0 ? parseInt(document.getElementById('numOrdem').value) - 0.5 : parseInt(document.getElementById('numOrdem').value),
            vl_nota: dojo.byId("nota").value,
            nm_peso_avaliacao: dojo.byId("peso").value
        };
        return myNewStore;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Edita um registro na grid
function alterarAvaliacaoGrid(grid) {
    apresentaMensagem('apresentadorMensagemAvaliacao', null);
    var cdAval = dojo.byId("cd_avaliacao").value == "" ? 0 : dojo.byId("cd_avaliacao").value;
    require([
            "dojo/_base/xhr"
    ], function (xhr) {
        showCarregando();
        xhr.get({
            url: Endereco() + "/api/coordenacao/getverificaTotalNota?idTipoAvaliacao=" + dijit.byId("tipoAvaliacao").value + "&idCriterio=" + dijit.byId("criterioAvaliacao").value + "&idAvaliacao=" + cdAval + "&nota=" + document.getElementById("nota").value + "&peso=" + dojo.number.parse(dom.byId("peso").value),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() }
        }).then(function (dataAvaliacao) {
            try {
                if (dataAvaliacao.MensagensWeb.length > 0)
                    apresentaMensagem('apresentadorMensagemAvaliacao', dataAvaliacao);
                else {
                    apresentaMensagem('apresentadorMensagemAvaliacao', null);

                    for (var l = 0; l < grid._by_idx.length ; l++) {
                        if (grid._by_idx[l].item.nm_ordem_avaliacao == ordemOri) {
                            // Muda os itens de lugares e altera o registro na grade:
                            grid._by_idx[l].item.nm_peso_avaliacao = dojo.byId("peso").value;
                            grid._by_idx[l].item.vl_nota = dojo.byId("nota").value;
                            grid._by_idx[l].item.id_avaliacao_ativa = document.getElementById('id_avaliacao_ativa').checked;

                            if (grid._by_idx[l].item.nm_ordem_avaliacao < parseInt(document.getElementById('numOrdem').value))
                                grid._by_idx[l].item.nm_ordem_avaliacao = parseInt(document.getElementById('numOrdem').value) + 0.5;
                            else
                                grid._by_idx[l].item.nm_ordem_avaliacao = parseInt(document.getElementById('numOrdem').value) - 0.5;

                            if (document.getElementById('id_avaliacao_ativa').checked == false) {
                                // grid._by_idx[l].item.dc_criterio_abreviado = "-";
                                grid._by_idx[l].item.dc_tipo_avaliacao = "-";
                            }
                            else {
                                grid._by_idx[l].item.dc_criterio_avaliacao = document.getElementById('criterioAvaliacao').value;
                                grid._by_idx[l].item.dc_tipo_avaliacao = document.getElementById('tipoAvaliacao').value;
                            }
                            //break;
                        }
                        grid._by_idx[l].nm_ordem_avaliacao = grid._by_idx[l].item.nm_ordem_avaliacao;
                    }
                    quickSortObj(grid._by_idx, 'nm_ordem_avaliacao');
                    grid._by_idx.reverse();

                    for (var l = grid._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                        grid._by_idx[l].item.nm_ordem_avaliacao = j;
                    grid.update();

                    for (var i = 0; i <= grid._by_idx.length - 1; i++) {
                        grid.selection.setSelected(i, false);
                        if (grid._by_idx[i].item.cd_avaliacao == parseInt(document.getElementById('cd_avaliacao').value)) {
                            //Seleciona a linha com o item editado por default:
                            grid.selection.setSelected(i, true);
                            ////Posiciona o foco do scroll no item editado:
                            grid.getRowNode(i).id = '';
                            grid.getRowNode(i).id = 'ordem_' + parseInt(document.getElementById('cd_avaliacao').value);
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                            window.location.hash = '#ordem_' + parseInt(document.getElementById('cd_avaliacao').value);
                        }
                    }
                }
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemAvaliacao', error);
        });
    });
}

function salvarAvaliacao() {
    try{
        apresentaMensagem('apresentadorMensagemAvaliacao', null);
        if (!dijit.byId("formAvaliacao").validate())
            return false;
        var listaAvaliacaoOrdem = [];
        var avaliacaoOrdem = null;
        var grid = dijit.byId("gridOrdenacao");

        for (var i = 0; i < grid._by_idx.length; i++) {
            if (hasValue(grid) && hasValue(grid._by_idx)) {
                listaAvaliacaoOrdem[i] = grid._by_idx[i].item;
                listaAvaliacaoOrdem[i].nm_peso_avaliacao = dojo.number.parse(grid._by_idx[i].item.nm_peso_avaliacao);
            }
            if (dojo.byId("cd_avaliacao").value == grid._by_idx[i].item.cd_avaliacao) {
                var nrOrdem = grid._by_idx[i].item.nm_ordem_avaliacao;
            }
        }
    
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/coordenacao/postEditAvaliacao", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    avaliacao: {
                        cd_avaliacao: dom.byId("cd_avaliacao").value,
                        cd_tipo_avaliacao: dijit.byId("tipoAvaliacao").value,
                        cd_criterio_avaliacao: dijit.byId("criterioAvaliacao").value,
                        vl_nota: dojo.number.parse(dom.byId("nota").value),
                        nm_ordem_avaliacao: nrOrdem,
                        nm_peso_avaliacao: dojo.number.parse(dom.byId("peso").value),
                        id_avaliacao_ativa: domAttr.get("id_avaliacao_ativa", "checked")
                    },
                    avaliacoes: listaAvaliacaoOrdem,
                    criterio: dom.byId("criterioAvaliacao").value,
                    tipoAvaliacao: dom.byId("tipoAvaliacao").value
                })
            }).then(function (data) {
                try{
                    data = jQuery.parseJSON(eval(data));
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var item = [];
                        var gridName = 'gridAvaliacao';
                        var grid = dijit.byId(gridName);
                        if (hasValue(dijit.byId("gridAvaliacao").itensSelecionados)) {
                            for (var l = 0; l < dijit.byId("gridAvaliacao").itensSelecionados.length; l++)
                                if (dijit.byId("gridAvaliacao").itensSelecionados[l].cd_tipo_avaliacao == dijit.byId("tipoAvaliacao").value &&
                                   dijit.byId("gridAvaliacao").itensSelecionados[l].cd_criterio_avaliacao == dijit.byId("criterioAvaliacao").value)
                                    item[l] = grid.itensSelecionados[l].cd_avaliacao;
                        }
                        if (item.length > 0)
                            for (var i = 0; i < item.length; i++)
                                removeObjSort(grid.itensSelecionados, "cd_avaliacao", item[i]);
                        //grid.update();
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId('cadAvaliacao').hide();
                        var ativo = dom.byId("id_avaliacao_ativa").checked ? 1 : 2;
                        dijit.byId("statusAvaliacao").set("value", ativo);

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        removeObjSort(grid.itensSelecionados, "cd_avaliacao", dom.byId("cd_avaliacao").value);
                        for (var i = 0; i < itemAlterado.length; i++)
                            insertObjSort(grid.itensSelecionados, "cd_avaliacao", itemAlterado[i]);

                        buscarItensSelecionados(gridName, 'selecionadoAvaliacao', 'cd_avaliacao', 'selecionaTodosAvaliacao', ['pesquisarAvaliacao', 'relatorioAvaliacao'], 'todosItensAvaliacao');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_avaliacao");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAvaliacao', data);
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAvaliacao', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Deletar avaliacao
function deletarAvaliacao(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr"], function (dom, domAttr) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_avaliacao').value != 0)
                itensSelecionados = [{
                    cd_avaliacao: dom.byId("cd_avaliacao").value,
                    cd_tipo_avaliacao: dijit.byId("tipoAvaliacao").value,
                    cd_criterio_avaliacao: dijit.byId("criterioAvaliacao").value,
                    id_avaliacao_ativa: domAttr.get("id_avaliacao_ativa", "checked")
                }];
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteAvaliacao",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItens");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId('cadAvaliacao').hide();
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridAvaliacao').itensSelecionados, "cd_avaliacao", itensSelecionados[r].cd_avaliacao);
                        searchAvaliacao(true);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarAvaliacao").set('disabled', false);
                        dijit.byId("relatorioAvaliacao").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAvaliacao', data);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                if (!hasValue(dojo.byId("cadAvaliacao").style.display))
                    apresentaMensagem('apresentadorMensagemAvaliacao', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        })
    });
};
//** fim das funções de percistência
//Função para link
//Evento do Link
function eventoEditarAvaliacao(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, dijit.byId('gridAvaliacao'), true);
            dijit.byId("cadAvaliacao").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAvaliacoesSelecionadas() {
    try{
        var dados = [];
        if (hasValue(dijit.byId("gridOrdenacao")) && hasValue(dijit.byId("gridOrdenacao").store.objectStore.data)) {
            var storeAval = dijit.byId("gridOrdenacao").itensSelecionados;
            $.each(storeAval, function (index, val) {
                    dados.push(val);
            });
            return dados;
        }
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarOrdenacao() {
     require(["dojo/store/Memory", "dojo/data/ObjectStore"],
            function (Memory, ObjectStore) {
         try{
             var aval = montarAvaliacoesSelecionadas();

             if (aval.length > 0) {
                 var arrayAval = dijit.byId("gridOrdenacao")._by_idx;
                 $.each(aval, function (idx, valueAval) {

                     arrayAval = jQuery.grep(arrayAval, function (value) {
                         return value.item != valueAval;
                     });
                 });
                 var dados = [];
                 $.each(arrayAval, function (index, value) {
                     dados.push(value.item);
                 });
                 var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
                 dijit.byId("gridOrdenacao").setStore(dataStore);
                 var mensagensWeb = new Array();
                 mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Avaliações excluídas com sucesso.");
             }

             for (var l = dijit.byId("gridOrdenacao")._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                 dijit.byId("gridOrdenacao")._by_idx[l].item.nm_ordem_avaliacao = j;
             dijit.byId("gridOrdenacao").itensSelecionados = [];
             dijit.byId("gridOrdenacao").update();
         }
         catch (e) {
             postGerarLog(e);
         }
     });
}