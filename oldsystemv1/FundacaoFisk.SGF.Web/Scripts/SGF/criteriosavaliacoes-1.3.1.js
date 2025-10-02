//Pega os Antigos dados do Formulário
function keepValues(value, grid, ehLink) {
    try {
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

        dojo.byId("cd_criterio_avaliacao").value = value.cd_criterio_avaliacao;
        dojo.byId("dc_criterio_avaliacao").value = value.dc_criterio_avaliacao;
        dojo.byId("dc_criterio_abreviado").value = value.dc_criterio_abreviado;
        dojo.byId("id_criterio_ativo").value = value.id_criterio_ativo == true ? dijit.byId("id_criterio_ativo").set("value", true) : dijit.byId("id_criterio_ativo").set("value", false);
        dojo.byId("id_conceito").value = value.id_conceito == true ? dijit.byId("id_conceito").set("value", true) : dijit.byId("id_conceito").set("value", false);
        dojo.byId("id_participacao").value = value.id_participacao == true ? dijit.byId("id_participacao").set("value", true) : dijit.byId("id_participacao").set("value", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarTipo(Memory, filteringSelect) {
    try {
        var statusTipo = new Memory({
            data: [
            { name: "Todos", id: 0 },
            { name: "Notas", id: 2 },
            { name: "Conceitos", id: 1 }
            ]
        });

        var _tipo = new filteringSelect({
            id: "conceitoCritPes",
            name: "conceitoCritPes",
            store: statusTipo,
            searchAttr: "name",
            value: 0,
            style: "width: 50%;"
        }, "conceitoCrit");
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridCriterio';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_criterio_avaliacao", grid._by_idx[rowIndex].item.cd_criterio_avaliacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_criterio_avaliacao', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_criterio_avaliacao', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);


        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}
function montaCadastroCriterio() {
    require([
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
        "dijit/form/FilteringSelect"
    ], function (dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, filteringSelect) {
        try{
            ready(function () {
                montarStatus("statusCriterio");
                montarTipo(Memory, filteringSelect);
                $(".Dialogo").css('display', 'none');
                // dijit.byId('tabContainer').resize();
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/coordenacao/getCriterioAvaliacaoSearch?descricao=" + encodeURIComponent(document.getElementById("descCriterio").value) + "&abrev=" + encodeURIComponent(document.getElementById("abrevCriterio").value) + "&inicio=" + document.getElementById("inicioCriterio").checked
                            + "&status=" + retornaStatus("statusCriterio") + "&conceito=" + retornaStatus("conceitoCritPes") + "&IsParticipacao=" + document.getElementById("ckPart").checked,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));

                var gridCriterio = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                                //  { name: "Código", field: "cd_criterio_avaliacao", width: "5%", styles: "text-align: right; min-width:60px; max-width:75px;" },
                                { name: "Nome", field: "dc_criterio_avaliacao", width: "40%", styles: "min-width:80px;" },
                                { name: "Abreviatura", field: "dc_criterio_abreviado", width: "40%", styles: "min-width:80px;" },
                                { name: "Ativo", field: "criterio_ativo", width: "10%", styles: "text-align:center; min-width:40px; max-width: 50px;" },
                                { name: "Conceito", field: "conceito", width: "10%", styles: "text-align:center; min-width:40px; max-width: 50px;" },
                                { name: "Participação", field: "participacao_ativo", width: "8%", styles: "text-align: center; min-width:80px;" }
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
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridCriterio");
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridCriterio.pagination.plugin._paginator.plugin.connect(gridCriterio.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridCriterio, 'cd_criterio_avaliacao', 'selecionaTodos'); });
                gridCriterio.canSort = function (col) { return Math.abs(col) != 1; };
                gridCriterio.startup();
                gridCriterio.on("RowDblClick", function (evt) {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                    getLimpar('#formCriterio');
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(item, gridCriterio, false);
                    dijit.byId("cadCriterio").show();
                    IncluirAlterar(0, 'divAlterarCriterio', 'divIncluirCriterio', 'divExcluirCriterio', 'apresentadorMensagemCriterio', 'divCancelarCriterio', 'divLimparCriterio');
                }, true);
                IncluirAlterar(1, 'divAlterarCriterio', 'divIncluirCriterio', 'divExcluirCriterio', 'apresentadorMensagemCriterio', 'divCancelarCriterio', 'divLimparCriterio');


                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosCriterio(); } }, "incluirCriterio");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosCriterio(); } }, "alterarCriterio");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { DeletarCriterio() }); } }, "deleteCriterio");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparCriterio(); } }, "limparCriterio");
                new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { keepValues(null, gridCriterio, null) } }, "cancelarCriterio");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadCriterio").hide(); } }, "fecharCriterio");

                require(["dojo/aspect"], function(aspect){
                    aspect.after(gridCriterio, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_criterio_avaliacao', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridCriterio')", gridCriterio.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditar(gridCriterio.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridCriterio.itensSelecionados, 'DeletarCriterio(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridCriterio, 'todosItens', ['pesquisarCriterio', 'relatorioCriterio']);
                        PesquisarCriterio(false);

                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridCriterio', 'selecionado', 'cd_criterio_avaliacao', 'selecionaTodos', ['pesquisarCriterio', 'relatorioCriterio'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                montaBotoesCriterio();
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323022',
                                '765px',
                                '771px'); // Nome Avaliação
                        });
                }
                adicionarAtalhoPesquisa(['descCriterio', 'abrevCriterio', 'statusCriterio', 'conceitoCritPes'], 'pesquisarCriterio', ready);
                showCarregando();
            })
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            getLimpar('#formCriterio');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(null, dijit.byId('gridCriterio'), true);
            dijit.byId("cadCriterio").show();
            IncluirAlterar(0, 'divAlterarCriterio', 'divIncluirCriterio', 'divExcluirCriterio', 'apresentadorMensagemCriterio', 'divCancelarCriterio', 'divLimparCriterio');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
function limparCriterio() {
    try{
        getLimpar('#formCriterio');
        clearForm('formCriterio');
        IncluirAlterar(1, 'divAlterarCriterio', 'divIncluirCriterio', 'divExcluirCriterio', 'apresentadorMensagemCriterio', 'divCancelarCriterio', 'divLimparCriterio');
        document.getElementById("cd_criterio_avaliacao").value = '';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function EnviarDadosCriterio() {
    try{
        if (document.getElementById("divAlterarCriterio").style.display == "")
            AlterarCriterio();
        else
            IncluirCriterio();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function montaBotoesCriterio() {
    try{
        require([
              "dojo/_base/xhr",
              "dojo/store/Cache",
              "dijit/form/Button",
              "dijit/form/TextBox"
        ], function (xhr, Cache, Button) {
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarCriterio(true); }
            }, "pesquisarCriterio");
            decreaseBtn(document.getElementById("pesquisarCriterio"), '32px');

            // Criterio
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    dijit.byId("cadCriterio").show();
                    getLimpar('#formCriterio');
                    clearForm("formCriterio");
                    apresentaMensagem('apresentadorMensagem', null);
                    IncluirAlterar(1, 'divAlterarCriterio', 'divIncluirCriterio', 'divExcluirCriterio', 'apresentadorMensagemCriterio', 'divCancelarCriterio', 'divLimparCriterio');
                }
            }, "novoCriterio");
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/coordenacao/GetUrlRelatorioCriterioAvaliacao?" + getStrGridParameters('gridCriterio') + "descricao=" + encodeURIComponent(document.getElementById("descCriterio").value) + "&abrev=" + encodeURIComponent(document.getElementById("abrevCriterio").value)
                            + "&inicio=" + document.getElementById("inicioCriterio").checked + "&status=" + retornaStatus("statusCriterio") + "&conceito=" + retornaStatus("conceitoCritPes") + "&IsParticipacao=" + document.getElementById("ckPart").checked,
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
            }, "relatorioCriterio");
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function IncluirCriterio() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemCriterio', null);
        if (!dijit.byId("formCriterio").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/coordenacao/postCriterioAvaliacao", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_criterio_avaliacao: dom.byId("dc_criterio_avaliacao").value,
                    dc_criterio_abreviado: dom.byId("dc_criterio_abreviado").value,
                    id_conceito: domAttr.get("id_conceito", "checked"),
                    id_criterio_ativo: domAttr.get("id_criterio_ativo", "checked"),
                    id_participacao: domAttr.get("id_participacao", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridCriterio';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadCriterio").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    dijit.byId("statusCriterio").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_criterio_avaliacao", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCriterio', 'cd_criterio_avaliacao', 'selecionaTodosCriterio', ['pesquisarCriterio', 'relatorioCriterio'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_criterio_avaliacao");
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCriterio', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function PesquisarCriterio(limparItens) {

    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory",
              "dijit/form/TextBox"
    ], function (JsonRest, ObjectStore, Cache, Memory, TextBox) {
        try {
            var myStore = Cache(
                    JsonRest({
                        handleAs: "json",
                        target: Endereco() + "/api/coordenacao/getCriterioAvaliacaoSearch?descricao=" + encodeURIComponent(document.getElementById("descCriterio").value) + "&abrev=" + encodeURIComponent(document.getElementById("abrevCriterio").value)
                            + "&inicio=" + document.getElementById("inicioCriterio").checked + "&status=" + retornaStatus("statusCriterio") + "&conceito=" + retornaStatus("conceitoCritPes") + "&IsParticipacao=" + document.getElementById("ckPart").checked,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });

            var gridCriterio = dijit.byId("gridCriterio");

            if (limparItens)
                gridCriterio.itensSelecionados = [];

            gridCriterio.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarCriterio() {
    try{
        var gridName = 'gridCriterio';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formCriterio").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/postalterarcriterioavaliacao",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_criterio_avaliacao: dom.byId("cd_criterio_avaliacao").value,
                    dc_criterio_avaliacao: dom.byId("dc_criterio_avaliacao").value,
                    dc_criterio_abreviado: dom.byId("dc_criterio_abreviado").value,
                    id_conceito: domAttr.get("id_conceito", "checked"),
                    id_criterio_ativo: domAttr.get("id_criterio_ativo", "checked"),
                    id_participacao: domAttr.get("id_participacao", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    dijit.byId("statusCriterio").set("value", 0);
                    dijit.byId("id_participacao").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadCriterio").hide();
                    removeObjSort(grid.itensSelecionados, "cd_criterio_avaliacao", dom.byId("cd_criterio_avaliacao").value);
                    insertObjSort(grid.itensSelecionados, "cd_criterio_avaliacao", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCriterio', 'cd_criterio_avaliacao', 'selecionaTodosCriterio', ['pesquisarCriterio', 'relatorioCriterio'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_criterio_avaliacao");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCriterio', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarCriterio(itensSelecionados) {
    try{
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_criterio_avaliacao').value != 0)
                    itensSelecionados = [{
                        cd_criterio_avaliacao: dom.byId("cd_criterio_avaliacao").value,
                        dc_criterio_avaliacao: dom.byId("dc_criterio_avaliacao").value,
                        dc_criterio_abreviado: dom.byId("dc_criterio_abreviado").value,
                        id_conceito: domAttr.get("id_conceito", "checked"),
                        id_criterio_ativo: domAttr.get("id_criterio_ativo", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteCriterioAvaliacao",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItens_label");
                    PesquisarCriterio(false);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadCriterio").hide();
                    dijit.byId("pesquisarCriterio").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridCriterio').itensSelecionados, "cd_criterio_avaliacao", itensSelecionados[r].cd_criterio_avaliacao);



                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCriterio").set('disabled', false);
                    dijit.byId("relatorioCriterio").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadCriterio").style.display))
                    apresentaMensagem('apresentadorMensagemCriterio', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        })
    } catch (e) {
        postGerarLog(e);
    }
}