function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridCalAcademico';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCalAcademicos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_calendario_academico", grid._by_idx[rowIndex].item.cd_calendario_academico);

            value = value || indice != null; // Item est� selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_calendario_academico', 'selecionadoCalAcademico', -1, 'selecionaTodosCalAcademicos', 'selecionaTodosCalAcademicos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_calendario_academico', 'selecionadoCalAcademico', " + rowIndex + ", '" + id + "', 'selecionaTodosCalAcademicos', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarMetodosCalendarioAcademico() {

    //Criação da Grade de sala
    require([
        "dojo/_base/xhr",
        "dijit/registry",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/DataGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojox/data/JsonRestStore",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dojo/dom-attr",
        "dijit/form/Button",
        "dijit/form/TextBox",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/DateTextBox",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!"
    ], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox) {
        ready(function () {
            showCarregando();
            //*** Cria a grade de Calendário Acadêmico **\\        
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCalendarioAcademicosPorFiltros?tipo_calendario=0&status=true",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEvento) {
                var gridCalAcademico = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: dataEvento }) }),
                    structure: [
                        { name: "<input id='selecionaTodosCalAcademicos' style='display:none'/>", field: "selecionadoCalAcademico", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        { name: "Tipo", field: "dc_tipo_calendario", width: "40%", styles: "min-width:80px;" },
                        { name: "Mostrar Todos", field: "id_mostrar_todos", width: "10%", styles: "min-width:80px;text-align:center;", formatter: statusAtivoFormatado },
                        { name: "Ativo", field: "id_ativo", width: "10%", styles: "min-width:80px;text-align:center;", formatter: statusAtivoFormatado }
                    ],
                    noDataMessage: msgNotRegEnc,
                    canSort: true,
                    selectionMode: "single",
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
                }, "gridCalAcademico");
                gridCalAcademico.startup();
                gridCalAcademico.canSort = function (col) { return Math.abs(col) != 1; };
                gridCalAcademico.on("RowDblClick", function (evt) {
                    try {
                        if (!eval(MasterGeral())) {
                            dijit.byId('tipoCalAcademico').set('disabled', true);
                            dijit.byId('txt_desc_academico').set('disabled', true);
                        }
                        var idx = evt.rowIndex, calendarioAcademicoSelecionado = this.getItem(idx);
                        obterCalendarioAcademicoPorID(calendarioAcademicoSelecionado.cd_calendario_academico);

                        dijit.byId("dialogCalendarioAcademico").show();
                        IncluirAlterar(0, 'divAlterarCalAcademico', 'divIncluirCalAcademico', 'divExcluirCalAcademico', 'apresentadorMensagemCalAcademico', 'divCancelarCalAcademico', 'divLimparCalAcademico');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
            });
            showCarregando();

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridCalAcademico, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosCalAcademicos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_calendario_academico', 'selecionadoCalAcademico', -1, 'selecionaTodosCalAcademicos', 'selecionaTodosCalAcademicos', 'gridCalAcademico')", gridCalAcademico.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        dijit.byId('tipoCalAcademico').set('disabled', true);
                        dijit.byId('txt_desc_academico').set('disabled', true);
                    } 
                    eventoEditarCalAcademico(dijit.byId('gridCalAcademico').itensSelecionados);
                }
            });
            menu.addChild(acaoEditar);

            var acaoExcluir = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            "Favor entrar em contato com os Administradores do Sistema. Somente estes podem excluir registros.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    } else {
                        eventoRemover(dijit.byId('gridCalAcademico').itensSelecionados, 'deletarCalAcademico(itensSelecionados)');
                    }
                }
            });
            menu.addChild(acaoExcluir);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasCalAcademico",
                dropDown: menu,
                id: "acoesRelacionadasCalAcademico"
            });
            dom.byId("linkAcoesCalAcademico").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridCalAcademico, 'todosItensCalAcademico', ['pesquisarCalendario', 'relatorioCalAcademico']); pesquisarCalendarioAcademico(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridCalAcademico', 'selecionadoCalAcademico', 'cd_calendario_academico', 'selecionaTodosCalAcademicos', ['pesquisarCalendario', 'relatorioCalAcademico'], 'todosItensCalAcademico'); }
            });
            menu.addChild(menuItensSelecionados);


            button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensCalAcademico",
                dropDown: menu,
                id: "todosItensCalAcademico"
            });
            dom.byId("linkSelecionadosCalAcademico").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar",
                iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                onClick: function () {

                    salvarCalendarioAcademico();

                }
            }, "incluirCalAcademico");

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {

                    if (!eval(MasterGeral())) {
                       
                        editarCalendarioAcademico();
                    } else {

                        editarCalendarioAcademicoMaster();
                    }

                    
                }
            }, "alterarCalAcademico");

            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            "Favor entrar em contato com os Administradores do Sistema. Somente estes podem excluir registros.");
                        apresentaMensagem("apresentadorMensagemCalAcademico", mensagensWeb);
                        return;
                    } else {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarCalAcademico() });
                    }
                }
            }, "deleteCalAcademico");

            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    limparCamposCalAcademico();
                }
            }, "limparCalAcademico");

            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagemCalEvento', null);
                    obterCalendarioAcademicoPorID(dom.byId("cd_calendario_academico").value);
                }
            }, "cancelarCalAcademico");

            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("dialogCalendarioAcademico").hide();
                    limparCamposCalAcademico();
                }
            }, "fecharCalAcademico");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    pesquisarCalendarioAcademico(true);
                }
            }, "pesquisarCalendario");

            decreaseBtn(document.getElementById("pesquisarCalendario"), '32px');
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            "Favor entrar em contato com os Administradores do Sistema. Somente estes podem adicionar novos registros.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    } else {
                        limparCamposCalAcademico();
                        IncluirAlterar(1, 'divAlterarCalAcademico', 'divIncluirCalAcademico', 'divExcluirCalAcademico', 'apresentadorMensagemCalAcademico', 'divCancelarCalAcademico', 'divLimparCalAcademico');
                        dijit.byId("dialogCalendarioAcademico").show();
                    }

                    //dijit.byId('tabContainer').resize();
                    //Para alinhar o painel do dojo, verificar se isso ocorre no desenvolvimento:
                    //dojo.byId('tabContainer_tablist').children[3].children[0].style.width = '100%';
                }
            }, "novoCalAcademico");

            new Button({
                label: "Visualizar",
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    gerarRelatorioCalendarioAcademico();
                }
            }, "relatorioCalAcademico");

            montarStatus("statusCalendario");
            loadTipo(Memory);
            loadTipoCad(Memory);
        })
    });
};

function pesquisarCalendarioAcademico(limparItens) {
    require([
      "dojo/_base/xhr",
      "dojo/data/ObjectStore",
      "dojo/store/Cache",
      "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        try {
            showCarregando();

            var gridCalAcademico = dijit.byId("gridCalAcademico");

            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCalendarioAcademicosPorFiltros?tipo_calendario=" + dijit.byId("tipoCalendario").value + "&status=" + status(),
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (calendarios) {

                if (limparItens) {
                    gridCalAcademico.itensSelecionados = [];
                }
                showCarregando();
                gridCalAcademico.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: calendarios }) }));
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function salvarCalendarioAcademico() {
    try {

        if (!dijit.byId("formCalendarioAcademico").validate())
            return false;
        showCarregando();

        var calendario = {
            cd_tipo_calendario: dijit.byId("tipoCalAcademico").value,
            dc_desc_calendario: dijit.byId("txt_desc_academico").value,
            id_mostrar_todos: dijit.byId("chk_mostrar_todos").checked,
            id_ativo: dijit.byId("chk_id_ativo").checked            
        };

        require([
          "dojo/_base/xhr"
        ], function (xhr) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/adicionarCalendarioAcademicoMaster",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify(calendario)
            }).then(function (data) {
                var calendarios = JSON.parse(data);

                if (!hasValue(calendarios.erro)) {
                    calendarios = calendarios.retorno;

                    var gridCalAcademico = dijit.byId("gridCalAcademico");
                    apresentaMensagem('apresentadorMensagemCalAcademico', data);
                    dijit.byId("dialogCalendarioAcademico").hide();
                    if (!hasValue(gridCalAcademico.itensSelecionados)) {
                        gridCalAcademico.itensSelecionados = [];
                    }
                    insertObjSort(gridCalAcademico.itensSelecionados, "cd_calendario_academico", calendarios);
                    buscarItensSelecionados('gridCalAcademico', 'selecionadoCalAcademico', 'cd_calendario_academico', 'selecionaTodosCalAcademicos', ['pesquisarCalendario', 'relatorioCalAcademico'], 'todosItensCalAcademico');
                    setGridPagination(gridCalAcademico, calendarios, "cd_calendario_academico");

                    showCarregando();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCalAcademico', data);
                }
            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioNovo);
                    apresentaMensagem("apresentadorMensagemCalAcademico", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagemCalAcademico", error);
            });
        });
    } catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function deletarCalAcademico(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            var cd_calendario_academico = 0;
            if (!hasValue(itensSelecionados) || itensSelecionados.length == 0) {
                if (dojo.byId('cd_calendario_academico').value > 0)
                    itensSelecionados = [{
                        cd_calendario_academico: dom.byId("cd_calendario_academico").value,
                        cd_tipo_calendario: dijit.byId("tipoCalAcademico").value
                    }];
            }
           
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/deletarCalendarioAcademicoMaster",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                var calendario = JSON.parse(data);
                calendario = JSON.parse(calendario);

                if (!hasValue(calendario.erro)) {
                    var todos = dojo.byId("todosItensCalAcademico");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('dialogCalendarioAcademico').hide();

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(itensSelecionados, "cd_calendario_academico", itensSelecionados[r].cd_calendario_academico);
                    pesquisarCalendarioAcademico(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCalendario").set('disabled', false);
                    dijit.byId("relatorioCalAcademico").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    showCarregando();
                }
                else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagem', data);
                }
            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioExcluir);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagem", error);
            });
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function eventoEditarCalAcademico(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            limparCamposCalAcademico();
            apresentaMensagem('apresentadorMensagem', '');
            var calendario = itensSelecionados[0];

            obterCalendarioAcademicoPorID(calendario.cd_calendario_academico);
            dijit.byId("dialogCalendarioAcademico").show();
            IncluirAlterar(0, 'divAlterarCalAcademico', 'divIncluirCalAcademico', 'divExcluirCalAcademico', 'apresentadorMensagemCalAcademico', 'divCancelarCalAcademico', 'divLimparCalAcademico');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarCalendarioAcademico() {

    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {

            if (!dijit.byId("formCalendarioAcademico").validate())
                return false;

            showCarregando();

            var calendario = {
                cd_calendario_academico: dojo.byId("cd_calendario_academico").value,
                cd_tipo_calendario: dijit.byId("tipoCalAcademico").value,
                dc_desc_calendario: dijit.byId("txt_desc_academico").value,
                id_ativo: dijit.byId("chk_id_ativo").checked,
                id_mostrar_todos: dijit.byId("chk_mostrar_todos").checked
            };
            xhr.post({
                url: Endereco() + "/api/coordenacao/editarCalendarioAcademico",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(calendario)
            }).then(function (data) {

                var calendario = JSON.parse(data);
                calendario = JSON.parse(calendario);
                if (!hasValue(calendario.erro)) {
                    calendario = calendario.retorno;

                    var gridCalAcademico = dijit.byId("gridCalAcademico");
                    apresentaMensagem('apresentadorMensagemCalAcademico', data);
                    dijit.byId("dialogCalendarioAcademico").hide();
                    if (!hasValue(gridCalAcademico.itensSelecionados)) {
                        gridCalAcademico.itensSelecionados = [];
                    }
                    removeObjSort(gridCalAcademico.itensSelecionados, "cd_calendario_academico", dom.byId("cd_calendario_academico").value);
                    insertObjSort(gridCalAcademico.itensSelecionados, "cd_calendario_academico", calendario);
                    buscarItensSelecionados('gridCalAcademico', 'selecionadoCalAcademico', 'cd_calendario_academico', 'selecionaTodosCalAcademicos', ['pesquisarCalendario', 'relatorioCalAcademico'], 'todosItensCalAcademico');
                    setGridPagination(gridCalAcademico, calendario, "cd_calendario_academico");

                    showCarregando();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCalAcademico', data);
                }

            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioEditar);
                    apresentaMensagem("apresentadorMensagemCalAcademico", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagemCalAcademico", error);
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}


function editarCalendarioAcademicoMaster() {

    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {

            if (!dijit.byId("formCalendarioAcademico").validate())
                return false;


            showCarregando();

            var calendario = {
                cd_calendario_academico: dojo.byId("cd_calendario_academico").value,
                cd_tipo_calendario: dijit.byId("tipoCalAcademico").value,
                dc_desc_calendario: dijit.byId("txt_desc_academico").value,
                id_ativo: dijit.byId("chk_id_ativo").checked,
                id_mostrar_todos: dijit.byId("chk_mostrar_todos").checked
            };
            xhr.post({
                url: Endereco() + "/api/coordenacao/editarCalendarioAcademicoMaster",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(calendario)
            }).then(function (data) {

                var calendario = JSON.parse(data);
                calendario = JSON.parse(calendario);
                if (!hasValue(calendario.erro)) {
                    calendario = calendario.retorno;

                    var gridCalAcademico = dijit.byId("gridCalAcademico");
                    apresentaMensagem('apresentadorMensagemCalAcademico', data);
                    dijit.byId("dialogCalendarioAcademico").hide();
                    if (!hasValue(gridCalAcademico.itensSelecionados)) {
                        gridCalAcademico.itensSelecionados = [];
                    }
                    removeObjSort(gridCalAcademico.itensSelecionados, "cd_calendario_academico", dom.byId("cd_calendario_academico").value);
                    insertObjSort(gridCalAcademico.itensSelecionados, "cd_calendario_academico", calendario);
                    buscarItensSelecionados('gridCalAcademico', 'selecionadoCalAcademico', 'cd_calendario_academico', 'selecionaTodosCalAcademicos', ['pesquisarCalendario', 'relatorioCalAcademico'], 'todosItensCalAcademico');
                    setGridPagination(gridCalAcademico, calendario, "cd_calendario_academico");

                    showCarregando();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCalAcademico', data);
                }

            }, function (error) {
                showCarregando();
                if (hasValue(error) && error.status == 401) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgPermissaoCalendarioEditar);
                    apresentaMensagem("apresentadorMensagemCalAcademico", mensagensWeb);
                    return;
                }
                apresentaMensagem("apresentadorMensagemCalAcademico", error);
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function obterCalendarioAcademicoPorID(cd_calendario_academico) {
    require(["dojo/_base/xhr"], function (xhr) {
        try {
            showCarregando();
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCalendarioAcademicoPorID?cd_calendario_academico=" + cd_calendario_academico,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                showCarregando();
                var calendario = jQuery.parseJSON(data);
                if (!hasValue(calendario.erro)) {
                    preencheInputCalendarioAcademico(calendario.retorno);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem("apresentadorMensagem", error);
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function gerarRelatorioCalendarioAcademico() {
    try {
        apresentaMensagem('apresentadorMensagem', null);

        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/GetUrlRelatorioCalendarioAcademico?" + getStrGridParameters('gridCalAcademico') + "tipo_calendario=" + dijit.byId("tipoCalendario").value + "&status=" + status(),
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
    }
    catch (e) {
        postGerarLog(e);
    }
}

function preencheInputCalendarioAcademico(calendario) {
    if (hasValue(calendario)) {
        dojo.byId("cd_calendario_academico").value = calendario.cd_calendario_academico;
        dijit.byId("tipoCalAcademico").set("value", calendario.cd_tipo_calendario);
        dijit.byId("txt_desc_academico").set("value", calendario.dc_desc_calendario);
        dijit.byId("chk_mostrar_todos").set("value", calendario.id_mostrar_todos);
        dijit.byId("chk_id_ativo").set("value", calendario.id_ativo);
    }
}

function limparCamposCalAcademico() {
    dojo.byId("cd_calendario_academico").value = 0;
    dijit.byId("tipoCalAcademico").reset();
    dijit.byId("txt_desc_academico").reset();
    dijit.byId("chk_mostrar_todos").set("value", true);
    dijit.byId("chk_id_ativo").set("value", true);
    apresentaMensagem('apresentadorMensagemCalEvento', null);
}

function status() {
    if (dijit.byId("statusCalendario").value == 1)
        return true;
    if (dijit.byId("statusCalendario").value == 2)
        return false;
    return null;
}

function loadTipo(Memory) {
    var statusStoreTipo = new Memory({
        data: [
            { name: "Todos", id: 0 },
            { name: "Início das Aulas", id: 1 },
            { name: "Atividade Extra", id: 2 },
            { name: "Programação", id: 3 }
        ]
    });
    dijit.byId("tipoCalendario").store = statusStoreTipo;
    dijit.byId("tipoCalendario").set("value", 0);
}
function loadTipoCad(Memory) {
    var statusStoreTipo = new Memory({
        data: [
            { name: "Início das Aulas", id: 1 },
            { name: "Atividade Extra", id: 2 },
            { name: "Programação", id: 3 }
        ]
    });
    dijit.byId("tipoCalAcademico").store = statusStoreTipo;
}