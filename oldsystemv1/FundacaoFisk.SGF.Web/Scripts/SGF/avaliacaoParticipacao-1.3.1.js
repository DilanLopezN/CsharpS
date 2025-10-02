var HAS_AVALIACAO_PARTICIPACAO = 12;
var MASCULINO = 2, FEMININO = 1, NAO_BINARIO = 3, PREFIRO_NAO_RESPONDER_NEUTRO = 4;
var SUBIR = 1, DESCER = 2;
var PARTICIPACAO_FREQUENCIA = 5;

function formatCheckBoxPart(value, rowIndex, obj) {
    var gridName = 'gridParticipacao';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosPart');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_participacao", grid._by_idx[rowIndex].item.cd_participacao_avaliacao);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos, para quando mudar de aba:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_participacao', 'selecionadoPart', -1, 'selecionaTodosPart', 'selecionaTodosPart', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_participacao', 'selecionadoPart', " + rowIndex + ", '" + id + "', 'selecionaTodosPart', '" + gridName + "')", 2);

    return icon;
}
function limparFormAvalPart() {
    try {
        dijit.byId("cbNomesCad").reset();
        dijit.byId("cbProdutoCad").reset();
        //dijit.byId("id_ativo_cad").set("checked", true);
        apresentaMensagem("apresentadorMensagemAvaliacaoPart", "");

        var gridParticipacaoAval = dijit.byId("gridParticipacaoAval");
        gridParticipacaoAval.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        gridParticipacaoAval.itensSelecionados = [];
        gridParticipacaoAval.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}
function formatCheckBoxParticipacao(value, rowIndex, obj) {
    var gridName = 'gridParticipacaoAval';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosParticipacao');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_participacao_avaliacao", grid._by_idx[rowIndex].item.cd_participacao_avaliacao);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos, para quando mudar de aba:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_participacao_avaliacao', 'selecionadoParticipacao', -1, 'selecionaTodosParticipacao', 'selecionaTodosParticipacao', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_participacao_avaliacao', 'selecionadoParticipacao', " + rowIndex + ", '" + id + "', 'selecionaTodosParticipacao', '" + gridName + "')", 2);

    return icon;
}

function keepValues(value, grid, ehLink) {
    try {
        limparFormAvalPart();
        if (!hasValue(value) && grid != null)
            value = grid.itemSelecionado;
        var cd_avaliacao_participacao = hasValue(value) && hasValue(value.cd_avaliacao_participacao) ? value.cd_avaliacao_participacao : dojo.byId('cd_avaliacao_participacao').value;
        if (cd_avaliacao_participacao > 0)
            populaAvaliacaoParticipacao(cd_avaliacao_participacao);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAvalPart(value, rowIndex, obj) {
    try
    {
        var gridName = 'gridAvaliacaoPart';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAvaliacaoPart');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_avaliacao_participacao_vinc", grid._by_idx[rowIndex].item.cd_avaliacao_participacao_vinc);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_avaliacao_participacao_vinc', 'selecionaAvaliacaoPart', -1, 'selecionaTodosAvaliacaoPart', 'selecionaTodosAvaliacaoPart', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_avaliacao_participacao_vinc', 'selecionaAvaliacaoPart', " + rowIndex + ", '" + id + "', 'selecionaTodosAvaliacaoPart', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// fim do procedimento para produto

function montarCadastroAvalicaoParticipacao() {
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
        "dojo/date",
        "dojo/on",
        "dojo/require",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, filteringSelect, date) {
        ready(function () {
            try {
                getReturnDadosAvalPart();
                var registroGrid = null;
                montarStatus("statusAvaliacaoPart");
                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/coordenacao/getSearchAvaliacaoParticipacao?cdCriterio=0&cdParticipacao=0&cdProduto=0&ativo=1",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({  }));

                var gridAvaliacaoPart = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodosAvaliacaoPart' style='display:none' />", field: "selecionaAvaliacaoPart", width: "15px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAvalPart },
                        { name: "Nome Avaliação", field: "dc_criterio_avaliacao", width: "45%", styles: "min-width:40px; text-align: left;" },
						{ name: "Participação", field: "no_participacao_avaliacao", width: "45%", styles: "min-width:50px; text-align: left;" },
						{ name: "Produto", field: "no_produto", width: "45%", styles: "min-width:50px; text-align: left;" },
                        { name: "Ordem", field: "nm_ordem", width: "50px", styles: "min-width:50px; text-align: left;" },
						{ name: "Ativo", field: "id_avaliacao_participacao_ativa", width: "50px", styles: "min-width:50px; text-align: center;" }
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
                }, "gridAvaliacaoPart");
                gridAvaliacaoPart.pagination.plugin._paginator.plugin.connect(gridAvaliacaoPart.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridAvaliacaoPart, 'cd_avaliacao_participacao_vinc', 'selecionaTodosAvaliacaoPart');
                });

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAvaliacaoPart, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosAvaliacaoPart').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_avaliacao_participacao_vinc', 'selecionaAvaliacaoPart', -1, 'selecionaTodosAvaliacaoPart', 'selecionaTodosAvaliacaoPart', 'gridAvaliacaoPart')", gridAvaliacaoPart.rowsPerPage * 3);
                    });
                });
                gridAvaliacaoPart.canSort = function (col) { return Math.abs(col) != 1 };
                gridAvaliacaoPart.startup();
                gridAvaliacaoPart.itensSelecionados = new Array();
                gridAvaliacaoPart.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                        registroGrid = item
                        showCarregando();
                        apresentaMensagem('apresentadorMensagem', '');
                        keepValues(item, gridAvaliacaoPart, true);
                        dijit.byId("cadAvaliacaoPart").show();
                        IncluirAlterar(0, 'divAlterarAvaliacaoPart', 'divIncluirAvaliacaoPart', 'divExcluirAvaliacaoPart', 'apresentadorMensagemAvaliacaoPart', 'divCancelarAvaliacaoPart', 'divLimparAvaliacaoPart');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                var dataPart = [];
                var gridParticipacaoAval = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: dataPart }) }),
                    structure:
                    [
                        { name: "<input id='selecionaTodosPart' style='display:none'/>", field: "selecionadoPart", width: "5%", styles: "text-align: center;min-width:15px; max-width:20px;", formatter: formatCheckBoxParticipacao },
						{ name: "Participação", field: "no_participacao_avaliacao", width: "70%", styles: "min-width:40px; text-align: left" },
                        {name: "Ordem", field: "nm_ordem", width: "10%", styles: "min-width:40px; text-align: center;" },
                        { name: "Ativa", field: "avaliacao_participacao_ativa", width: "10%", styles: "min-width:40px; text-align: center;" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridParticipacaoAval");
                gridParticipacaoAval.startup();
                gridParticipacaoAval.canSort = function (col) { return Math.abs(col) != 1; };

                var dataParticipacao = [];
                var gridParticipacao = new EnhancedGrid({
                    store: datastore = new ObjectStore({ objectStore: new Memory({ data: dataParticipacao }) }),
                    structure: [
                        { name: "<input id='selecionaTodosParticipacao' style='display:none'/>", field: "selecionadoParticipacao", width: "5%", styles: "text-align: center;min-width:15px; max-width:20px;", formatter: formatCheckBoxPart  },
                        { name: "Participação", field: "no_participacao", width: "80%", styles: "min-width:40px; text-align: left" }
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum Registro encontrado."
                }, "gridParticipacao");
                gridParticipacao.canSort = function (col) { return Math.abs(col) != 1; };
                gridParticipacao.startup();
                //***************** Adiciona link de ações:****************************\\
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditarAvaliacaoPart(dijit.byId('gridAvaliacaoPart').itensSelecionados, ready, Memory, filteringSelect, ObjectStore);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridAvaliacaoPart.itensSelecionados, 'DeletarAvaliacaoParticipacao(itensSelecionados)'); }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasAtividadeExtra",
                    dropDown: menu,
                    id: "acoesRelacionadasAtividadeExtra"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        buscarTodosItens(gridAvaliacaoPart, 'todosItensAvaliacaoPart', ['pesquisarAvaliacaoPart', 'relatorioAvaliacaoPart']);
                        pesquisarAvaliacaoParticipacao(false);                        
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        buscarItensSelecionados('gridAvaliacaoPart', 'selecionaAvaliacaoPart', 'cd_avaliacao_participacao_vinc', 'selecionaTodosAvaliacaoPart', ['pesquisarAvaliacaoPart', 'relatorioAvaliacaoPart'], 'todosItensAvaliacaoPart');

                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensAvaliacaoPart",
                    dropDown: menu,
                    id: "todosItensAvaliacaoPart"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);
                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        dijit.byId("PartFK").show();
                        populaParticipacoes();
                    }
                }, "incluirPartFK");
                //*** Cria os botões de persistência **\\

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        IncluirAvaliacaoParticipacao();
                    }
                }, "incluirAvaliacaoPart");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        AlteraAvaliacaoParticipacao();
                    }
                }, "alterarAvaliacaoPart");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarAvaliacaoParticipacao();
                        });
                    }
                }, "deleteAvaliacaoPart");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                    onClick: function () {
                        limparFormAvalPart();
                    }
                }, "limparAvaliacaoPart");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        try {
                            showCarregando();
                            var grade = hasValue(dijit.byId("gridAvaliacaoPart")) ? dijit.byId("gridAvaliacaoPart") : null;
                            keepValues(null, grade, false);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarAvaliacaoPart");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadAvaliacaoPart").hide();
                    }
                }, "fecharAvaliacaoPart");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisarAvaliacaoParticipacao(true);
                    }
                }, "pesquisarAvaliacaoPart");
                decreaseBtn(document.getElementById("pesquisarAvaliacaoPart"), '32px');


                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        dijit.byId("cadAvaliacaoPart").show();
                        limparFormAvalPart();
                        apresentaMensagem('apresentadorMensagem', null);
                        getReturnDadosCadAvalPart(null, null);
                        IncluirAlterar(1, 'divAlterarAvaliacaoPart', 'divIncluirAvaliacaoPart', 'divExcluirAvaliacaoPart', 'apresentadorMensagemAvaliacaoPart', 'divCancelarAvaliacaoPart', 'divLimparAvaliacaoPart');
                    }
                }, "novaAvaliacaoPart");

                //----Monta o botão de Relatório----
               
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            xhr.get({
                                url: Endereco() + "/api/coordenacao/GetUrlRelatorioAvaliacaoParticipacao?" + getStrGridParameters('gridParticipacao') + "&cdCriterio=" + dijit.byId("cbNomesAval").value + "&cdParticipacao=" + dijit.byId("pesParticipacao").value +
                                         "&cdProduto=" + dijit.byId("cbProduto").value + "&ativo=" + dijit.byId("statusAvaliacaoPart").value,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemAvaliacaoPart', error);
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioAvaliacaoPart");
                var menuPart = new DropDownMenu({ style: "height: 25px" });
                var acaoRemoverPart = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoParticipacao(Memory, ObjectStore, 'cd_participacao_avaliacao', dijit.byId("gridParticipacaoAval")); }
                });
                menuPart.addChild(acaoRemoverPart);

                var acaoAtivaDesativaPart = new MenuItem({
                    label: "Ativar/Desativar",
                    onClick: function () { AlterarStatusAvaliacaoParticipacao(); }
                });
                menuPart.addChild(acaoAtivaDesativaPart);

                var buttonPart = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasPart",
                    dropDown: menuPart,
                    id: "acoesRelacionadasPart"
                });
                dojo.byId("linkAcoesPart").appendChild(buttonPart.domNode);

                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                    onClick: function () { retornarParticipacoes(); }
                        
                }, "selecionaPartFK");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("PartFK").hide(); }
                }, "fecharPartFK");

                if (!hasValue(dijit.byId('subir'))) {
                    new Button({
                        label: "Subir", iconClass: 'dijitEditorIcon dijitEditorIconTabUp',
                        onClick: function () {
                            subirDescerOrdemAvaliacao(dijit.byId("gridParticipacaoAval"), SUBIR);
                        }
                    }, "subir");
                    new Button({
                        label: "Descer", iconClass: 'dijitEditorIcon dijitEditorIconTabDown',
                        onClick: function () {
                            subirDescerOrdemAvaliacao(dijit.byId("gridParticipacaoAval"), DESCER);
                        }
                    }, "descer");
                }
                dijit.byId("criterios").on("show", function (e) {
                    dijit.byId("gridParticipacaoAval").update();
                });

                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323056', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['cbNomesAval', 'pesParticipacao', 'cbProduto', 'statusAvaliacaoPart'], 'pesquisarAvaliacaoPart', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function deletarItemSelecionadoParticipacao(Memory, ObjectStore, nomeId, grid) {
    try {
        apresentaMensagem("apresentadorMensagemAvaliacaoPart", null);
        grid.store.save();
        var dados = grid.store.objectStore.data;

        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
            var cloneTitulos = grid.store.objectStore.data.slice();
            //Percorre a lista da grade para deleção (O(n)):
            for (var i = dados.length - 1; i >= 0; i--) {
                // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null) {
                    if (dados[i].cd_participacao_avaliacao == PARTICIPACAO_FREQUENCIA) {
                        dados = cloneTitulos;
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDelecaoParticipacaoFrequencia);
                        apresentaMensagem("apresentadorMensagemAvaliacaoPart", mensagensWeb);
                        return false;
                    }
                    dados.splice(i, 1); // Remove o item do array
                }
            }

            
            grid.itensSelecionados = new Array();
            
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
            grid.setStore(dataStore);
            OrdenaGridParticipacao();
            grid.update();
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}


function OrdenaGridParticipacao() {
    var items = dijit.byId("gridParticipacaoAval").store.objectStore.data;
    var cont = 0;
    for (var i = 0; i < items.length; i++) {
        if (items[i].nm_ordem > 0 && items[i].nm_ordem != 5) {
            if (cont == 4) cont++;
            items[i].nm_ordem = cont+ 1;
            cont++;
        }
    }

   
    quickSortObj(items, 'nm_ordem');
    dijit.byId("gridParticipacaoAval").setStore(dojo.data.ObjectStore({ objectStore: dojo.store.Memory({ data: items }) }));
}

function BuscaMaiorOrdemParticipacao() {
    var items = dijit.byId("gridParticipacaoAval").store.objectStore.data;
    var maiorOrdem = 0;
    for (var i = 0; i < items.length ; i++) {
        if (items[i].nm_ordem > maiorOrdem) {
            maiorOrdem = items[i].nm_ordem;
           
        }
    }
    if(maiorOrdem == 5) return  maiorOrdem + 1;
    if(maiorOrdem == 4) return maiorOrdem + 2;
    return maiorOrdem;
}

function IncluirAvaliacaoParticipacao() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAvaliacaoPart', null);
        
        var part = montarParticipacaoAval();
        if (!dijit.byId("formAvaliacaoPart").validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/coordenacao/postInsertAvaliacaoParticipacao", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_criterio_avaliacao: dijit.byId("cbNomesCad").value,
                    cd_produto: dijit.byId("cbProdutoCad").value,
                    //id_ativa: dijit.byId("id_ativo_cad").checked,
                    AvaliacaoParticipacaoVinc: part
                })
            }).then(function (data) {
                try {
                    showCarregando();
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridAvaliacaoPart';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadAvaliacaoPart").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }

                        for (var i = 0; i < itemAlterado.length; i++) {
                            removeObjSort(grid.itensSelecionados, "cd_avaliacao_participacao_vinc", itemAlterado[i].cd_avaliacao_participacao_vinc);
                            insertObjSort(grid.itensSelecionados, "cd_avaliacao_participacao_vinc", itemAlterado[i]);
                            if (hasValue(grid.store.objectStore.data))
                                setGridPagination(grid, itemAlterado[i], "cd_aula_personalizada_aluno");
                        }


                        buscarItensSelecionados(gridName, 'selecionaAvaliacaoPart', 'cd_avaliacao_participacao_vinc', 'selecionaTodosAvaliacaoPart', ['pesquisarAvaliacaoPart', 'relatorioAvaliacaoPart'], 'todosItensAvaliacaoPart');

                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    } else
                        apresentaMensagem('apresentadorMensagemAvaliacaoPart', data);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAvaliacaoPart', error.response.data);
            });
        });
    }
    catch (er) {
        postGerarLog(er);
    }
}



function AlterarStatusAvaliacaoParticipacao() {
    var gridParticipacao = dijit.byId("gridParticipacaoAval");

    if (hasValue(gridParticipacao) && hasValue(gridParticipacao.itensSelecionados)) {
        if (gridParticipacao.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else
        {
            if (gridParticipacao.itensSelecionados[0].id_avaliacao_participacao_ativa && gridParticipacao.itensSelecionados[0].nm_ordem != 5) {
                gridParticipacao.itensSelecionados[0].avaliacao_participacao_ativa = "Não";
                gridParticipacao.itensSelecionados[0].id_avaliacao_participacao_ativa = false;
                gridParticipacao.itensSelecionados[0].nm_ordem = 0;
               
            }
            else if (!gridParticipacao.itensSelecionados[0].id_avaliacao_participacao_ativa && gridParticipacao.itensSelecionados[0].nm_ordem != 5) {
                gridParticipacao.itensSelecionados[0].avaliacao_participacao_ativa = "Sim";
                gridParticipacao.itensSelecionados[0].id_avaliacao_participacao_ativa = true;
                gridParticipacao.itensSelecionados[0].nm_ordem = BuscaMaiorOrdemParticipacao() + 1;
            }

        }
    } else {
        caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
    }
    

   
   
    gridParticipacao.update();
    OrdenaGridParticipacao();
   
}

function AlteraAvaliacaoParticipacao() {
    try{        
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemAvaliacaoPart', null);

        var part = montarParticipacaoAval();
        if (!dijit.byId("formAvaliacaoPart").validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/coordenacao/postAlterarAvaliacaoParticipacao", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_avaliacao_participacao: dojo.byId("cd_avaliacao_participacao").value,
                    cd_criterio_avaliacao: dijit.byId("cbNomesCad").value,
                    cd_produto: dijit.byId("cbProdutoCad").value,
                    //id_ativa: dijit.byId("id_ativo_cad").checked,
                    AvaliacaoParticipacaoVinc: part
                })
            }).then(function (data) {
                try {
                    showCarregando();
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridAvaliacaoPart';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadAvaliacaoPart").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }

                        for (var i = 0; i < itemAlterado.length; i++) {
                            removeObjSort(grid.itensSelecionados, "cd_avaliacao_participacao_vinc", itemAlterado[i].cd_avaliacao_participacao_vinc);
                            insertObjSort(grid.itensSelecionados, "cd_avaliacao_participacao_vinc", itemAlterado[i]);
                            if (hasValue(grid.store.objectStore.data))
                                setGridPagination(grid, itemAlterado[i], "cd_aula_personalizada_aluno");
                        }

                        buscarItensSelecionados(gridName, 'selecionaAvaliacaoPart', 'cd_avaliacao_participacao_vinc', 'selecionaTodosAvaliacaoPart', ['pesquisarAvaliacaoPart', 'relatorioAvaliacaoPart'], 'todosItensAvaliacaoPart');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.


                        removeObjSort(grid.itensSelecionados, "cd_aula_personalizada", itemAlterado[0].cd_aula_personalizada);

                    } else
                        apresentaMensagem('apresentadorMensagemAvaliacaoPart', data);
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAvaliacaoPart', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarAvaliacaoParticipacao(itensSelecionados) {
    showCarregando();
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_avaliacao_participacao').value != 0)
                itensSelecionados = [{
                    cd_avaliacao_participacao: dom.byId("cd_avaliacao_participacao").value
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteAvaliacaoParticipacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try{
                var todos = dojo.byId("todosItensAvaliacaoPart");
                apresentaMensagem('apresentadorMensagem', data);
                data = jQuery.parseJSON(data).retorno;
                dijit.byId("cadAvaliacaoPart").hide();
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridAvaliacaoPart').itensSelecionados, "cd_avaliacao_participacao_vinc", itensSelecionados[r].cd_avaliacao_participacao_vinc);
                pesquisarAvaliacaoParticipacao(true);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarAvaliacaoPart").set('disabled', false);
                dijit.byId("relatorioAvaliacaoPart").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";

                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadAvaliacaoPart").style.display)) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAvaliacaoPart', error);
            }
            else {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            }
        });
    });
}



function populaAvaliacaoParticipacao(cdAvalPart) {
    
    require([
          "dojo/_base/xhr",
          "dojo/data/ObjectStore",
          "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getAvaliacaoParticipacaoByEdit?cdAvalPart=" + cdAvalPart,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_atividade_aluno"
        }).then(function (data) {
            try {
                showCarregando();
                var grid = dijit.byId("gridParticipacaoAval");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: data.AvaliacaoParticipacaoVinc, idProperty: "cd_participacao" }) });
                grid.setStore(dataStore);               

                //Popular Produto e Criterio
                getReturnDadosCadAvalPart(data.cd_produto, data.cd_criterio_avaliacao);
                
                //dijit.byId("id_ativo_cad").set("checked", data.id_ativa);
                dojo.byId("cd_avaliacao_participacao").value = data.cd_avaliacao_participacao;
                grid.update();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemAvaliacaoPart', error.response.data);
        });
    });
}
//#endregion fim dos métodos de percistência

function eventoEditarAvaliacaoPart(itensSelecionados, ready, Memory, filteringSelect, ObjectStore) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(itensSelecionados[0], dijit.byId('gridAvaliacaoPart'), true);
            dijit.byId("cadAvaliacaoPart").show();
            IncluirAlterar(0, 'divAlterarAvaliacaoPart', 'divIncluirAvaliacaoPart', 'divExcluirAvaliacaoPart', 'apresentadorMensagemAvaliacaoPart', 'divCancelarAvaliacaoPart', 'divLimparAvaliacaoPart');
        }
    }
    catch (er) {
        postGerarLog(er);
    }
}


function montarParticipacaoAval() {
    try{
        var participacoes = [];
        var gridParticipacao = dijit.byId("gridParticipacaoAval");

        hasValue(gridParticipacao) ? gridParticipacao.store.save() : null;
        if (hasValue(gridParticipacao) && hasValue(gridParticipacao._by_idx))
            var data = gridParticipacao._by_idx;
        else participacoes = null;
        if (hasValue(gridParticipacao) && hasValue(data) && data.length > 0)
            $.each(data, function (idx, val) {
                participacoes.push({
                    cd_participacao_avaliacao: val.item.cd_participacao_avaliacao,
                    cd_avaliacao_participacao_vinc: val.item.cd_avaliacao_participacao_vinc,
                    id_avaliacao_participacao_ativa: val.item.id_avaliacao_participacao_ativa,
                    nm_ordem: val.item.nm_ordem
                });
            });
        return participacoes;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function getReturnDadosAvalPart() {
    try {
        
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/coordenacao/getReturnDadosAvalPart?hasDependente=" + HAS_AVALIACAO_PARTICIPACAO,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                criarOuCarregarCompFiltering("cbNomesAval", data.criterios, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_criterio_avaliacao', 'dc_criterio_avaliacao',FEMININO);
                criarOuCarregarCompFiltering("cbProduto", data.produtos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', MASCULINO);
                criarOuCarregarCompFiltering("pesParticipacao", data.participacoes, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_participacao', 'no_participacao', FEMININO);
                //#endregion
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAvaliacaoPart', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarAvaliacaoParticipacao(limparItens) {
    try {
        
        var myStore =
             dojo.store.Cache(
                dojo.store.JsonRest({
                    target: Endereco() + "/api/coordenacao/getSearchAvaliacaoParticipacao?cdCriterio=" + dijit.byId("cbNomesAval").value + "&cdParticipacao=" + dijit.byId("pesParticipacao").value +
                                         "&cdProduto=" + dijit.byId("cbProduto").value + "&ativo=" + dijit.byId("statusAvaliacaoPart").value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }
        ), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridAvaliacaoPart");

        if (limparItens)
            grid.itensSelecionados = [];

        grid.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}


function getReturnDadosCadAvalPart(cdProduto, cdCriterio) {
    try {

        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/coordenacao/getReturnDadosCadAvalPart?cdProduto=" + cdProduto + "&cdCriterio=" + cdCriterio,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                criarOuCarregarCompFiltering("cbNomesCad", data.criterios, "", cdCriterio, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_criterio_avaliacao', 'dc_criterio_avaliacao', null);
                criarOuCarregarCompFiltering("cbProdutoCad", data.produtos, "", cdProduto, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', null);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAvaliacaoPart', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}
function populaParticipacoes() {
    try {
        var stringCds = "";
        var gridPartExiste = dijit.byId("gridParticipacaoAval")._by_idx;
        for (var i = 0; i < gridPartExiste.length; i++) {
            if (stringCds == "")
                stringCds = gridPartExiste[i].item.cd_participacao_avaliacao + "";
            else
                stringCds = stringCds + "|" + gridPartExiste[i].item.cd_participacao_avaliacao + "";
        }
        var myStore =
             dojo.store.Cache(
                dojo.store.JsonRest({
                    target: Endereco() + "/api/coordenacao/getParticipacoes?cdsPart=" + stringCds,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }
        ), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridParticipacao");
        grid.itensSelecionados = [];
        grid.setStore(dataStore);

    } catch (e) {
        postGerarLog(e);
    }
}

function retornarParticipacoes() {

    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridParticipacaoAval = dijit.byId("gridParticipacaoAval");
                var gridParticipacao = dijit.byId("gridParticipacao");

                if (!hasValue(gridParticipacao.itensSelecionados) || gridParticipacao.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridParticipacaoAval = (hasValue(gridParticipacaoAval) && hasValue(gridParticipacaoAval.store) && hasValue(gridParticipacaoAval.store.objectStore.data)) ? gridParticipacaoAval.store.objectStore.data : [];

                    var itemFrequencia = jQuery.grep(gridParticipacaoAval.store.objectStore.data, function (value) {
                        return value.cd_participacao_avaliacao == PARTICIPACAO_FREQUENCIA;
                    });
                    $.each(gridParticipacao.itensSelecionados, function (idx, value) {
                        if (value.cd_participacao != PARTICIPACAO_FREQUENCIA) {
                            var ultimaOrdem = 0;
                            var storeParticipacaoAval = cloneArray(gridParticipacaoAval.store.objectStore.data);
                            storeParticipacaoAval = jQuery.grep(storeParticipacaoAval, function (value) {
                                return value.cd_participacao_avaliacao != PARTICIPACAO_FREQUENCIA;
                            });
                            quickSortObj(storeParticipacaoAval, 'nm_ordem');
                            var ultimaOrdem = storeParticipacaoAval.length;
                            if (ultimaOrdem > 0){
                                if (ultimaOrdem == 4)
                                    ultimaOrdem = ultimaOrdem + 2;
                                else if (ultimaOrdem == 5)
                                    ultimaOrdem = ultimaOrdem + 1;
                                else
                                    ultimaOrdem = storeParticipacaoAval[ultimaOrdem - 1].nm_ordem + 1;
                            }
                            insertObjSort(gridParticipacaoAval.store.objectStore.data, "cd_participacao_avaliacao",
                                {
                                    cd_participacao_avaliacao: value.cd_participacao,
                                    no_participacao_avaliacao: value.no_participacao,
                                    id_avaliacao_participacao_ativa: true,
                                    avaliacao_participacao_ativa: "Sim",
                                    nm_ordem: ultimaOrdem == 0 ? 1 : ultimaOrdem
                                });
                        }
                    });
                    if (!hasValue(itemFrequencia) || itemFrequencia.length <= 0) {
                        insertObjSort(gridParticipacaoAval.store.objectStore.data, "cd_participacao_avaliacao", {
                            cd_participacao_avaliacao: 5,
                            no_participacao_avaliacao: "Frequência",
                            id_avaliacao_participacao_ativa: true,
                            avaliacao_participacao_ativa: "Sim",
                            nm_ordem: PARTICIPACAO_FREQUENCIA
                        });
                    }
                    quickSortObj(gridParticipacaoAval.store.objectStore.data, 'nm_ordem');
                    gridParticipacaoAval.setStore(new ObjectStore({ objectStore: new Memory({ data: gridParticipacaoAval.store.objectStore.data }) }));

                    dijit.byId("PartFK").hide();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }

function subirDescerOrdemAvaliacao(grid, sobeDesce) {
    try {
        apresentaMensagem("apresentadorMensagemAvaliacaoPart", null);
        var operacao = sobeDesce == SUBIR ? 1 : -1;
        if (grid.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            grid.itensSelecionados = [];
            grid.update();
            return false;
        }

        if (grid.itensSelecionados.length > 0 && grid.itensSelecionados[0].id_avaliacao_participacao_ativa == false) {
            caixaDialogo(DIALOGO_AVISO, msgNaoRegOrdenarInativo, null);
            grid.itensSelecionados = [];
            grid.update();
            return false;
        }

        var itemSelecionado = grid.itensSelecionados;

        if (itemSelecionado.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegOrdem, null);
        else {

            if (itemSelecionado[0].nm_ordem == 5) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPosicaoFrequenciaParticipacao);
                apresentaMensagem("apresentadorMensagemAvaliacaoPart", mensagensWeb);
                return false;
            }

            for (var l = 0; l < grid._by_idx.length; l++) {
                if (grid._by_idx[l].item.cd_participacao_avaliacao == itemSelecionado[0].cd_participacao_avaliacao) {
                    if (hasValue(grid._by_idx[l - (operacao)])) {
                        var itemEncontrado = grid._by_idx[l].item;
                        var ordemEncontrada = grid._by_idx[l].item.nm_ordem;
                        var posicaoEncontrada = grid.selection.selectedIndex;

                        // Muda as ordens de lugares:
                        if (grid._by_idx[l - (operacao)].item.nm_ordem == 0) {
                            caixaDialogo(DIALOGO_AVISO, msgItemPrimeiraPosicao, null);
                            grid.update();
                            return false;
                        } else {
                            if (grid._by_idx[l - (operacao)].item.nm_ordem == 5) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPosicaoFrequenciaParticipacao);
                                apresentaMensagem("apresentadorMensagemAvaliacaoPart", mensagensWeb);
                                return false;
                            }
                            grid._by_idx[l].item.nm_ordem = grid._by_idx[l - (operacao)].item.nm_ordem;
                            grid._by_idx[l - (operacao)].item.nm_ordem = ordemEncontrada;
                            // Muda os itens de lugares:
                            grid._by_idx[l].item = grid._by_idx[l - (operacao)].item;
                            grid._by_idx[l - (operacao)].item = itemEncontrado;
                            grid.update();
                        }
                        grid.getRowNode(l).id = '';
                        grid.getRowNode(l - (operacao)).id = 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem;
                        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                        window.location.hash = '#' + 'ordem_' + grid._by_idx[l - (operacao)].item.nm_ordem;

                        // Atualiza o item selecionado:
                        grid.selection.setSelected(posicaoEncontrada, false);
                        if (posicaoEncontrada < grid._by_idx.length)
                            grid.selection.setSelected(posicaoEncontrada - 1, true);
                        var codAlteradoSubir = grid._by_idx[l].item.nm_ordem + ";" + grid._by_idx[l - (operacao)].item.nm_ordem + ";";

                    }
                    return codAlteradoSubir;
                    break;
                }
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//Marca na grade o registro inserido
function atualizarAvaliacoes(i) {
    try {
        var grid = dijit.byId('gridParticipacaoAval');
        grid.getRowNode(i).id = '';
        grid.getRowNode(i).id = 'ordem_' + grid._by_idx[i].item.cd_avaliacao_participacao_vinc;
        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
        window.location.hash = '#ordem_' + grid._by_idx[i].item.cd_avaliacao_participacao_vinc;
    } catch (e) {
        postGerarLog(e);
    }
}