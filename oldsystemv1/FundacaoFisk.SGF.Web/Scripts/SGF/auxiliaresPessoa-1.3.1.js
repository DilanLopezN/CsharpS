//Os tipos de formulário:
var FORM_ATIVIDADE = 11;

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, value, grid) {
    try {
        if (!hasValue(value))
            value = grid.itemSelecionado;
        switch (tipoForm) {
            case FORM_ATIVIDADE: {
                getLimpar('#formAtividade');
                dojo.byId("cd_atividade").value = value.cd_atividade;
                dojo.byId("no_atividade").value = value.no_atividade;
                dojo.byId("atividade_ativa").value = value.id_atividade_ativa == true ? dijit.byId("atividade_ativa").set("value", true) : dijit.byId("atividade_ativa").set("value", false);
                dojo.byId("cd_cnae_atividade").value = value.cd_cnae_atividade;
                var natureza_atividade = dijit.byId("id_natureza_atividade");
                natureza_atividade._onChangeActive = false;
                natureza_atividade.set('value', value.id_natureza_atividade);
                if (value.id_natureza_atividade == 2)
                    toggleDisabled(dijit.byId("cd_cnae_atividade"), false);
                else {
                    toggleDisabled(dijit.byId("cd_cnae_atividade"), true);
                    dijit.byId("cd_cnae_atividade").reset();
                }
                natureza_atividade._onChangeActive = true;
                return false;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadNatureza() {
    try {
        require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
            function (ready, Memory, filteringSelect) {
                try {
                    var stateStore = new Memory({
                        data: [
                        { name: "Todos", id: "0" },
                        { name: "Física", id: "1" },
                        { name: "Jurídica", id: "2" },
                        { name: "Cargo", id: "3" }
                        ]
                    });

                    ready(function () {
                        var natureza = new filteringSelect({
                            id: "natureza",
                            name: "natureza",
                            value: "1",
                            store: stateStore,
                            searchAttr: "name",
                            style: "width: 75px;"
                        }, "naturezaAtividade");
                    })

                    var stateStoreCad = new Memory({
                        data: [
                        { name: "Física", id: "1" },
                        { name: "Jurídica", id: "2" },
                        { name: "Cargo", id: "3" }
                        ]
                    });
                    dijit.byId("id_natureza_atividade").store = stateStoreCad;
                } catch (e) {
                    postGerarLog(e);
                }
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

////$("#labelTitulos").val("Secretaria");
function mostraTabs(Permissoes) {
    require([
         "dijit/registry",
         "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            try{
                if (!possuiPermissao('ativ', Permissoes)) {
                    registry.byId('tabAtividade').set('disabled', !registry.byId('tabAtividade').get('disabled'));
                    document.getElementById('tabAtividade').style.visibility = "hidden";
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function formatCheckBoxAtividade(value, rowIndex, obj) {
    try{
        var gridName = 'gridAtividade';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAtividade');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_atividade", grid._by_idx[rowIndex].item.cd_atividade);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_atividade', 'selecionadoAtividade', -1, 'selecionaTodosAtividade', 'selecionaTodosAtividade', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_atividade', 'selecionadoAtividade', " + rowIndex + ", '" + id + "', 'selecionaTodosAtividade', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaAuxiliarPessoa() {
    require([
        "dojo/dom",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try{
                montarStatus("statusAtividade");
                query("body").addClass("claro");
                dijit.byId("tabContainer").resize();
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/localidade/getAtividadesearch?descricao=&inicio=false&status=1&natureza=1&cnae=",
                         headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                         idProperty: "cd_atividade"
                     }), Memory({ idProperty: "cd_atividade" }));

                var gridAtividade = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodosAtividade' style='display:none'/>", field: "selecionadoAtividade", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAtividade },
                       // { name: "Código", field: "cd_atividade", width: "5%" },
                        { name: "Atividade/Profissão", field: "no_atividade", width: "55%" },
                        { name: "Natureza", field: "natureza_atividade", width: "15%" },
                        { name: "CNAE", field: "cd_cnae_atividade", width: "15%" },
                        { name: "Ativa", field: "atividade_ativa", width: "10%" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridAtividade"); // make sure you have a target HTML element with this id
                gridAtividade.startup();
                gridAtividade.pagination.plugin._paginator.plugin.connect(gridAtividade.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridAtividade, 'cd_atividade', 'selecionaTodosAtividade'); });
                gridAtividade.canSort = function (col) { return Math.abs(col) != 1; };
                gridAtividade.on("RowDblClick", function (evt) {
                    try{
                        getLimpar('#formAtividade');
                        apresentaMensagem('apresentadorMensagem', '');
                        var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                        gridAtividade.itemSelecionado = item;
                        keepValues(FORM_ATIVIDADE, null, gridAtividade);
                        dijit.byId("formularioAtividade").show();
                        IncluirAlterar(0, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                IncluirAlterar(1, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');


                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosAtividade(); } }, "incluirAtividade");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosAtividade(); } }, "alterarAtividade");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarAtividade() }); } }, "deleteAtividade");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        limparAtividade();
                    }
                }, "limparAtividade");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        keepValues(FORM_ATIVIDADE, null, gridAtividade);
                    }
                }, "cancelarAtividade");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        dijit.byId("formularioAtividade").hide();
                    }
                }, "fecharAtividade");

                loadNatureza();
                ////////////////////////////
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAtividade, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosAtividade').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_atividade', 'selecionadoAtividade', -1, 'selecionaTodosAtividade', 'selecionaTodosAtividade', 'gridAtividade')", gridAtividade.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarAtividade(gridAtividade.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridAtividade.itensSelecionados, 'DeletarAtividade(itensSelecionados)'); }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasAtividade",
                    dropDown: menu,
                    id: "acoesRelacionadasAtividade"
                });
                dom.byId("linkAcoesAtividade").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridAtividade, 'todosItensAtividade', ['pesquisarAtividade', 'relatorioAtividade']); PesquisarAtividade(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridAtividade', 'selecionadoAtividade', 'cd_atividade', 'selecionaTodosAtividade', ['pesquisarAtividade', 'relatorioAtividade'], 'todosItensAtividade'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensAtividade",
                    dropDown: menu,
                    id: "todosItensAtividade"
                });
                dom.byId("linkSelecionadosAtividade").appendChild(button.domNode);
                ///////////////////////////////////////
                montaBotoesAuxiliarPessoa();

                toggleDisabled(dijit.byId("cd_cnae_atividade"), true);
                dijit.byId("id_natureza_atividade").on("change", function (e) {
                    if (dijit.byId("id_natureza_atividade").value == 2)
                        toggleDisabled(dijit.byId("cd_cnae_atividade"), false);
                    else {
                        toggleDisabled(dijit.byId("cd_cnae_atividade"), true);
                        dijit.byId("cd_cnae_atividade").reset();
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323025', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['pesquisaAtividade', 'statusAtividade', 'natureza', 'cnae'], 'pesquisarAtividade', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formPais');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(FORM_PAIS, null, dijit.byId('gridPais'));
            dijit.byId("formularioPais").show();
            IncluirAlterar(0, 'divAlterarPais', 'divIncluirPais', 'divExcluirPais', 'apresentadorMensagemPais', 'divCancelarPais', 'divLimparPais');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarAtividade(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            var grid = dijit.byId('gridAtividade');
            getLimpar('#formAtividade');
            apresentaMensagem('apresentadorMensagem', '');
            grid.itemSelecionado = itensSelecionados[0];
            keepValues(FORM_ATIVIDADE, null, grid);
            dijit.byId("formularioAtividade").show();
            IncluirAlterar(0, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaBotoesAuxiliarPessoa() {
    require([
          "dojo/_base/xhr",
          "dojo/store/Cache",
          "dijit/form/Button"
    ], function (xhr, Cache, Button) {
        try {
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        dijit.byId("formularioAtividade").show();
                        getLimpar('#formAtividade');
                        clearForm("formAtividade");
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoAtividade");

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarAtividade(true); }
            }, "pesquisarAtividade");
            decreaseBtn(document.getElementById("pesquisarAtividade"), '32px');

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/localidade/geturlrelatorioAtividade?" + getStrGridParameters('gridAtividade') + "descricao=" + encodeURIComponent(document.getElementById("pesquisaAtividade").value) + "&inicio=" + document.getElementById("inicioDescAtividade").checked + "&status=" + retornaStatus("statusAtividade") + "&natureza=" + retornaStatus("natureza") + "&cnae=" + encodeURIComponent(document.getElementById("cnae").value),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
                }
            }, "relatorioAtividade");
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparAtividade() {
    try {
        getLimpar('#formAtividade');
        clearForm('formAtividade');
        IncluirAlterar(1, 'divAlterarAtividade', 'divIncluirAtividade', 'divExcluirAtividade', 'apresentadorMensagemAtividade', 'divCancelarAtividade', 'divLimparAtividade');
        document.getElementById("cd_atividade").value = '';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function EnviarDadosAtividade() {
    try {
        if (document.getElementById("divAlterarAtividade").style.display == "")
            AlterarAtividade();
        else
            IncluirAtividade();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function IncluirAtividade() {
    try {
        if (!dijit.byId("formAtividade").validate())
            return false;
        apresentaMensagem('apresentadorMensagemAtividade', null);
        apresentaMensagem('apresentadorMensagem', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/localidade/postAtividade", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                data: ref.toJson({
                    cd_atividade: dom.byId("cd_atividade").value,
                    no_atividade: dom.byId("no_atividade").value,
                    cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                    id_natureza_atividade: dijit.byId("id_natureza_atividade").value,
                    id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                })
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;
                        var gridName = 'gridAtividade';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioAtividade").hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        insertObjSort(grid.itensSelecionados, "cd_atividade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoAtividade', 'cd_atividade', 'selecionaTodosAtividade', ['pesquisarAtividade', 'relatorioAtividade'], 'todosItensAtividade');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_atividade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAtividade', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAtividade', error);
                });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function PesquisarAtividade(limparItens) {
    try{
        require([
                   "dojo/store/JsonRest",
                   "dojo/data/ObjectStore",
                   "dojo/store/Cache",
                   "dojo/store/Memory",
                   "dojo/query"
        ], function (JsonRest, ObjectStore, Cache, Memory, query) {
            query("body").addClass("claro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/localidade/getAtividadesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaAtividade").value) + "&inicio=" + document.getElementById("inicioDescAtividade").checked + "&status=" + retornaStatus("statusAtividade") + "&natureza=" + retornaStatus("natureza") + "&cnae=" + encodeURIComponent(document.getElementById("cnae").value),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }), Memory({})
           );
            try{
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridAtividade = dijit.byId("gridAtividade");
                if (limparItens)
                    gridAtividade.itensSelecionados = [];
                gridAtividade.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function AlterarAtividade() {
    try {
        var gridName = 'gridAtividade';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formAtividade").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/localidade/postalterarAtividade",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_atividade: dom.byId("cd_atividade").value,
                    no_atividade: dom.byId("no_atividade").value,
                    cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                    id_natureza_atividade: dijit.byId("id_natureza_atividade").value,
                    id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                })
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(eval(data)).retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioAtividade").hide();
                        removeObjSort(grid.itensSelecionados, "cd_atividade", dom.byId("cd_atividade").value);
                        insertObjSort(grid.itensSelecionados, "cd_atividade", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoAtividade', 'cd_Atividade', 'selecionaTodosAtividade', ['pesquisarAtividade', 'relatorioAtividade'], 'todosItensAtividade');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_atividade");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAtividade', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAtividade', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarAtividade(itensSelecionados) {
    try {
        if (!dijit.byId("formAtividade").validate()) {
            //        return false;
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe Atividade selecionado.");
            apresentaMensagem('apresentadorMensagemAtividade', mensagensWeb);
        }

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_atividade').value != 0)
                    itensSelecionados = [{
                        cd_atividade: dom.byId("cd_atividade").value,
                        no_atividade: dom.byId("no_atividade").value,
                        cd_cnae_atividade: dom.byId("cd_cnae_atividade").value,
                        id_natureza_atividade: dijit.byId("id_natureza_atividade").value,
                        id_atividade_ativa: domAttr.get("atividade_ativa", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/localidade/postdeleteAtividade",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensAtividade_label");

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("formularioAtividade").hide();
                        dijit.byId("pesquisaAtividade").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridAtividade').itensSelecionados, "cd_atividade", itensSelecionados[r].cd_atividade);

                        PesquisarAtividade(false);

                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarAtividade").set('disabled', false);
                        dijit.byId("relatorioAtividade").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        if (!hasValue(dojo.byId("formularioAtividade").style.display))
                            apresentaMensagem('apresentadorMensagemAtividade', error);
                        else
                            apresentaMensagem('apresentadorMensagem', error);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
        function (error) {
            if (!hasValue(dojo.byId("formularioAtividade").style.display))
                apresentaMensagem('apresentadorMensagemAtividade', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
        })
    }
    catch (e) {
        postGerarLog(e);
    }
}