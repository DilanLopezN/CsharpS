var ENTRADA = 0, TIPOMOVIMENTO;
var HAS_ESTAGIO = 3;
var BIBLIOTECA = 3, MATERIAL = 1, ESTOQUE = 2, IMOBILIZADO = 4, SERVICO = 5, CUSTODESPESA = 6;
var NIVEL1 = 1, NIVEL2 = 2;
var CADASTRO = 2;
var PRIVADA = 1, PUBLICA = 2;
var ITEM = 6;
var PESQUISA = 1;

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainerKit");
        var pane = dijit.byId("tabPrincipal");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}


//#endregion

//#region  métodos auxiliares
function limparFormItem() {
    try {
        getLimpar('#formKit');
        dojo.byId('cd_item').value = 0;
        
        limparGrid();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparSubGrupoTipo() {
    try {
        dijit.byId("pes_tipoKitMov").reset();
        dijit.byId("cbSubGrupo2").reset();
        dijit.byId("ckManterCad").set("checked", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try {
        var cdItem = dojo.byId('cd_item').value;
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab))
            tab = dojo.query(e.target)[0];// Clicou na borda da aba

        if (jQuery.parseJSON(MasterGeral()) == false)
            dijit.byId("tabContainerKit_tablist_escolaContentPane").domNode.style.visibility = 'hidden';

        if (tab.getAttribute('widgetId') == "tabContainerKit_tablist_escolaContentPane") {
            popularGridEscola();
            dojo.byId('abriuEscola').value = true;
        }
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

function popularGridEscola() {
    try {
        var item = hasValue(dojo.byId('cd_item').value) ? dojo.byId('cd_item').value : 0;
        var gridEscolas = dijit.byId('gridEscolas');
        var popular = !eval(dojo.byId('abriuEscola').value);
        if (gridEscolas != null && gridEscolas.store != null && gridEscolas.store.objectStore.data.length == 0 && popular) {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getEscolatWithItem?cd_item=" + item,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagemKit', data);
                    var empresas = data.retorno;
                    gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: empresas }) }));
                    gridEscolas.update();
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemKit', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


//Pega os Antigos dados do Formulário
function keepValues(pes_tipoKitForm, value, grid, ehLink) {
    try {
        getLimpar('#formKit');
        clearForm('formKit');
        limparGrid();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');
        dojo.byId('abriuEscola').value = false;


        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        var cdGrupo =  0;
        var cdTipoItem = hasValue(value.cd_tipo_item) && value.cd_tipo_item > 0 ? value.cd_tipo_item : 0;
        recoverDataPesquisa("grupoItem", "tipoItens");

        dojo.byId('abriuEscola').value = false;

        dojo.byId("cd_item").value = value.cd_item;
        dojo.byId("nome").value = value.no_item;
        if (hasValue(dojo.byId('tipoItens').value, true)) {
            hasValue(value.tipoItens) ? dijit.byId("tipoItens").set("value", value.tipoItens) : 0;
        }
        if (hasValue(dojo.byId('grupoItem').value, true)) {
            hasValue(value.cd_grupo_estoque) ? dijit.byId("grupoItem").set("value", value.cd_grupo_estoque) : 0;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}



function formatCheckBoxItemKit(value, rowIndex, obj) {
    try {
        var gridName = 'gridItemKit'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosItemKit');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id", grid._by_idx[rowIndex].item.id);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoItemKit', -1, 'selecionaTodosItemKit', 'selecionaTodosItemKit', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item', 'selecionadoItemKit', " + rowIndex + ", '" + id + "', 'selecionaTodosItemKit', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTextNumberValor(value, rowIndex, obj) {
    try {
        var gridItemKit = dijit.byId("gridItemKit");
        var icon;
        var desc = obj.field + '_input_' + gridItemKit._by_idx[rowIndex].item.cd_item;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxValor('" + value + "', '" + desc + "','" + gridItemKit._by_idx[rowIndex].item.qt_item_kit + "','" + gridItemKit._by_idx[rowIndex].item.cd_item + "'," + rowIndex + ")", 3);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }

}

function configuraTextBoxValor(value, desc, itemValue, cd_item, rowIndex) {
    try {
        var desabilitar = false;
        if (value == undefined || isNaN(parseInt(value))) value = '1';

        if (!hasValue(dijit.byId(desc))) {
            var newTextBox = new dijit.form.NumberTextBox({
                name: "textBox" + desc,
                disabled: desabilitar,
                //value: unmaskFixed(value, 2),
                old_value: value,
                maxlength: 6,
                style: "width: 100%;",
                onBlur: function (b) {
                    $('#' + desc).focus();
                },
                onChange: function (b) {
                    if (b != itemValue)
                        atualizarValores(desc, this, cd_item, b);
                },
                smallDelta: 1,
                constraints: { min:0,places:0 }
            }, desc);
            newTextBox._onChangeActive = false;
            newTextBox.set('value', value);
            newTextBox.value = value;
            newTextBox._onChangeActive = true;
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraInt(document.getElementById(desc));
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarValores(desc, obj, rowIndex, qt_item_kit) {
    try {
        var gridItemKit = dijit.byId("gridItemKit");
        var item = getItemStore(gridItemKit, rowIndex);
        var objDijit = dijit.byId(obj.id);


        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        setaValor(item, objDijit.old_value, objDijit.value, rowIndex, gridItemKit);
        gridItemKit.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}




function indexValue(arraytosearch, key, valuetosearch) {

    for (var i = 0; i < arraytosearch.length; i++) {

        if (arraytosearch[i][key] == valuetosearch) {
            return i;
        }
    }
    return null;
}

function setaValor(item, valorAntigo, valorAtual, index, grid) {
    try {
        

        if (isNaN(valorAtual) || !hasValue(valorAtual, true)) {
            var itemFiltrado = grid.store.objectStore.data.filter(function (item) {
                return item.cd_item == index;
            });
            var posicaoItemFiltrado = grid.store.objectStore.data.indexOf(itemFiltrado[0]);
            if (hasValue(grid.store.objectStore.data[posicaoItemFiltrado])) {
                grid.store.objectStore.data[posicaoItemFiltrado].qt_item_kit = valorAntigo;
            } 
            return;
        }
        else {

            var itemFiltrado = grid.store.objectStore.data.filter(function (item) {
                return item.cd_item == index;
            });
            var posicaoItemFiltrado = grid.store.objectStore.data.indexOf(itemFiltrado[0]);
            if (hasValue(grid.store.objectStore.data[posicaoItemFiltrado])) {
                grid.store.objectStore.data[posicaoItemFiltrado].qt_item_kit = valorAtual;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getItemStore(grid, id) {
    try {
        
            //var qt_item_kit = grid.store.objectStore.data[i].qt_item_kit;
        //if (qt_item_kit == id)
        if (hasValue(grid.store.objectStore.data)) {
            var item = grid.store.objectStore.data.filter(function (item) {
                return item.cd_item == id;
            });
            return item;
        }

        //if (hasValue(grid.store.objectStore.data[id])) {
        //    return grid.store.objectStore.data[id];
        //}

            return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Item servico
function formatCheckBoxKit(value, rowIndex, obj) {
    try {
        var gridName = 'gridKit';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosKit');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item", grid._by_idx[rowIndex].item.cd_item);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoKit', -1, 'selecionaTodosKit', 'selecionaTodosKit', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item', 'selecionadoKit', " + rowIndex + ", '" + id + "', 'selecionaTodosKit', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Escola
function formatCheckBoxEscola(value, rowIndex, obj) {
    try {
        var gridName = 'gridEscolas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEscola');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoEscola', -1, 'selecionaTodosEscola', 'selecionaTodosEscola', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoEscola', " + rowIndex + ", '" + id + "', 'selecionaTodosEscola', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//--**********************************--\\

function mostraTabs(Permissoes) {
    require([
            "dijit/registry",
            "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            try {
                if (!possuiPermissao('item', Permissoes)) {
                    registry.byId('tabKit').set('disabled', !registry.byId('tabKit').get('disabled'));
                    document.getElementById('tabKit').style.visibility = "hidden";
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}


//Métodos para carregar o Grupo de estoque
function recoverDataPesquisa(fieldGrupo, fieldTipo) {
    require([
            "dojo/_base/xhr"],

        function (xhr) {
            
            xhr.get({
                url: Endereco() +
                    "/api/financeiro/returnDataKit?cdGrupo=0&cdTipoItem=0&tipoMovimento=",
                preventCache: true,
                handleAs: "json",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": Token()
                }
            }).then(function(dataItem) {
                    try {
                        dataItem = jQuery.parseJSON(dataItem);
                        if (!hasValue(dataItem.erro)) {
                            loadGrupoKit(dataItem.retorno.grupos, fieldGrupo);
                            loadTipoItemKit(dataItem.retorno.tipos, fieldTipo);
                            loadCategoriaItem();
                            if (!hasValue(dijit.byId("gridKit"))) {
                                montarGridKit();
                            }
                        } else
                            apresentaMensagem('apresentadorMensagem', dataItem.erro);
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function(error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        });
}

function loadDados(fieldGrupo, fieldTipo) {
    require([
            "dojo/_base/xhr"],

        function (xhr) {

            xhr.get({
                url: Endereco() +
                    "/api/financeiro/returnDataKit?cdGrupo=0&cdTipoItem=0&tipoMovimento=",
                preventCache: true,
                handleAs: "json",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": Token()
                }
            }).then(function (dataItem) {
                    try {
                        dataItem = jQuery.parseJSON(dataItem);
                        if (!hasValue(dataItem.erro)) {
                            loadGrupoKit(dataItem.retorno.grupos, fieldGrupo);
                            loadTipoItemKit(dataItem.retorno.tipos, fieldTipo);
                            loadCategoriaItem();
                        } else
                            apresentaMensagem('apresentadorMensagem', dataItem.erro);
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        });
}
 

function retornarItemFK() {
    try {
        var valido = true;
        //var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        //var gridEscolas = dijit.byId("gridEscolas");
        var gridItemKitSelec = dijit.byId("gridPesquisaItem");
        var gridItemKit = dijit.byId("gridItemKit");
        if (!hasValue(gridItemKitSelec.itensSelecionados))
            gridItemKitSelec.itensSelecionados = [];
        if (!hasValue(gridItemKitSelec.itensSelecionados) || gridItemKitSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
        if (dijit.byId("cadKit").open) {
            var storeGridKits = (hasValue(gridItemKit) && hasValue(gridItemKit.store.objectStore.data)) ? gridItemKit.store.objectStore.data : [];
            quickSortObj(gridItemKit.store.objectStore.data, 'cd_item');
            $.each(gridItemKitSelec.itensSelecionados, function (idx, value) {
                insertObjSort(gridItemKit.store.objectStore.data, "cd_item", {
                    cd_item: value.cd_item,
                    cd_item_kit: 0,
                    qt_item_kit: 1,
                    no_item: value.no_item
                });
            });
            gridItemKit.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridItemKit.store.objectStore.data }) }));
        }

        if (!valido)
            return false;
        dijit.byId("fkItem").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadGrupoKit(items, linkGrupo) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbGrupo = dijit.byId(linkGrupo);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_grupo_estoque, name: value.no_grupo_estoque });
            });

            var stateStore = new Memory({
                data: itemsCb
            });
            cbGrupo.store = stateStore;
            var indexSelected = indexValue(itemsCb, "name", "Kit de vendas");
            dijit.byId(linkGrupo).set("value", itemsCb[indexSelected].id);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//fim Método Grupo
function loadTipoItemKit(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array", "dijit/form/FilteringSelect"],
    function (Memory, Array, filteringSelect) {
        try {
            var itemsCb = [];
            var cbTipoItem = dijit.byId(field);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_tipo_item, name: value.dc_tipo_item});
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipoItem.store = stateStore;
            var indexSelected = indexValue(itemsCb, "name", "Material Didático");
            dijit.byId(field).set("value", itemsCb[indexSelected].id);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadCategoriaItem() {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
        function(Memory, filteringSelect) {
            try {
                var items = [];

                items.push(
                    { name: "Todos", id: 0 },
                    { name: "Privada", id: 1 },
                    { name: "Pública", id: 2 });
                var categoria = new Memory({
                    data: items
                });

                dijit.byId("pes_categoriaKit").store = categoria;
                var indexSelected = indexValue(items, "name", "Privada");
                dijit.byId("pes_categoriaKit").set("value", items[indexSelected].id);
            } catch (e) {
                postGerarLog(e);
            }
        });
}


function indexValue(arraytosearch, key, valuetosearch) {

    for (var i = 0; i < arraytosearch.length; i++) {

        if (arraytosearch[i][key] == valuetosearch) {
            return i;
        }
    }
    return null;
}

//#endregion

//#region Montar Casdastro
function montarCadastroItem() {
    //Criação da Grade

    require([
        "dojo/_base/xhr",
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
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/FilteringSelect",
        "dojo/_base/array",
        "dojo/promise/all",
        "dojo/Deferred",
        'dojo/_base/lang',
        'dojox/grid/cells/dijit',
        "dijit/Dialog"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, FilteringSelect, array, all, Deferred,lang, cells) {
        ready(function() {
            try {

                montarStatus("pes_statusKit");



                //if (jQuery.parseJSON(MasterGeral()) == false)
                //    dijit.byId("tabContainerKit_tablist_escolaContentPane").domNode.style.visibility = 'hidden';

               
                recoverDataPesquisa("pes_grupoKit", "pes_tipoKit");
                

           
                
               
                
                ////Verifica se o usuario é master/ desabilita a aba de escola
                //if (jQuery.parseJSON(MasterGeral()) == false) {
                //    dijit.byId("tabContainerKit_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                //}

                //***************** Adiciona link de ações:****************************\\
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        if (jQuery.parseJSON(MasterGeral()) == false) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        } else {
                            eventoEditarKit(dijit.byId("gridKit").itensSelecionados);
                        }
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        if (jQuery.parseJSON(MasterGeral()) == false) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        } else {
                            eventoRemover(dijit.byId("gridKit").itensSelecionados, 'DeletarKit(itensSelecionados)');
                        }
                    }
                });
                menu.addChild(acaoRemover);

                var parametros = getParamterosURL();

                //if (hasValue(parametros['pes_tipoKitRetorno'])) {
                //    var acaoCurso = new MenuItem({
                //        label: "Curso",
                //        onClick: function () { redirecionaMaterialDidatico(parametros['pes_tipoKitRetorno']); }
                //    });
                //    menu.addChild(acaoCurso);

                //    // Mosta a tela de cadastro de item pois se trata de um link:
                //    //dijit.byId("cadKit").show();
                //}

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasKit",
                    dropDown: menu,
                    id: "acoesRelacionadasKit"
                });
                dom.byId("linkAcoesKit").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function() {
                        buscarTodosItens(gridKit, 'todosKit', ['pes_pesquisaKit', 'relatorioKit']);
                        PesquisarKit(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function() {
                        buscarItensSelecionados('gridKit',
                            'selecionadoKit',
                            'cd_item',
                            'selecionaTodosKit',
                            ['pes_pesquisaKit', 'relatorioKit'],
                            'todosItensKit');
                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensKit",
                    dropDown: menu,
                    id: "todosItensKit"
                });
                dom.byId("linkSelecionadosKit").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                query("#pes_pesquisaKit").on("keyup",
                    function(e) {
                        if (e.keyCode == 13) {
                            apresentaMensagem('apresentadorMensagem', null);
                            PesquisarKit(true);
                        }
                    });
                new Button({
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function () {
                            if (jQuery.parseJSON(MasterGeral()) == false) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSalvarFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            } else {
                                IncluirKit();
                            }
                        }
                    },
                    "incluirKit");
                new Button({
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function () {
                            if (jQuery.parseJSON(MasterGeral()) == false) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSalvarFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            } else {
                                AlterarKit();
                            }
                        }
                    },
                    "alterarKit");
                new Button({
                        label: "Excluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                        onClick: function () {

                            if (jQuery.parseJSON(MasterGeral()) == false) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            } else {
                                caixaDialogo(DIALOGO_CONFIRMAR,
                                    '',
                                    function executaRetorno() {
                                        DeletarKit();
                                    });
                            };
                        }
                    },
                    "deleteKit");
                new Button({
                        label: "Limpar",
                        iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                        type: "reset",
                        onClick: function() {
                            limparFormItem();
                            loadDados("grupoItem", "tipoItens");
                        }
                    },
                    "limparKit");
                new Button({
                        label: "Cancelar",
                        iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                        onClick: function() {
                            showCarregando();
                            keepValues(null, null, dijit.byId("gridKit"), null);
                            setarTabCad();
                            getItensKit(dijit.byId("gridKit").selection.getSelected()[0], xhr);
                            showCarregando();
                        }
                    },
                    "cancelarKit");
                new Button({
                        label: "Fechar",
                        iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                        onClick: function() {
                            dijit.byId("cadKit").hide();
                        }
                    },
                    "fecharKit");

                new Button({
                        label: "",
                        iconClass: 'dijitEditorIconSearchSGF',
                        onClick: function () {
                                PesquisarKit(true);
                        }
                    },
                    "pes_pesquisaKit");
                decreaseBtn(document.getElementById("pes_pesquisaKit"), '32px');

                var gridItemKit = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        {
                            name: "<input id='selecionaTodosItemKit' style='display:none'/>",
                            field: "selecionadoItemKit",
                            width: "5%",
                            styles: "text-align:center; min-width:15px; max-width:20px;",
                            formatter: formatCheckBoxItemKit
                        },
                        {
                            name: "Qtde",
                            field: "qt_item_kit",
                            width: "10%",
                            styles: "min-width:80px;text-align:center;",
                            formatter: formatTextNumberValor
                        },
                        { name: "Item", field: "no_item", width: "70%", styles: "min-width:80px;" },
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    contentEditable: function (col) { return Math.abs(col) == 2; },
                    plugins: {
                        pagination: {
                            pageSizes: ["8", "24", "40", "60", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "8",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                },
                    "gridItemKit");
                gridItemKit.canSort = function (col) { return Math.abs(col) != 1; };
                gridItemKit.startup();

                require(["dojo/aspect"],
                    function (aspect) {
                        aspect.after(gridItemKit,
                            "_onFetchComplete",
                            function () {
                                // Configura o check de todos:
                                if (dojo.byId('selecionaTodosItemKit').type == 'text')
                                    setTimeout(
                                        "configuraCheckBox(false, 'cd_item', 'selecionadoItemKit', -1, 'selecionaTodosItemKit', 'selecionaTodosItemKit', 'gridItemKit')",
                                        gridKit.rowsPerPage * 3);
                            });
                    });

                

                var menuItemKit = new DropDownMenu({ style: "height: 25px" });
                
                

                var acaoRemoverItemKit = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(Memory, ObjectStore,'cd_item', dijit.byId("gridItemKit"));
                    }
                });
                menuItemKit.addChild(acaoRemoverItemKit);

                //Botao Incluir ItemKits de Contas
                var buttonItemKit = new Button({
                    label: "Incluir",
                    name: "itensKit",
                    iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    id: "btnaddItemKit",
                    onClick: function () {
                        //limparSubGrupoTipo();

                        try {
                            TIPOMOVIMENTO = ENTRADA;
                            abrirKitFK(xhr, Memory, FilteringSelect, array, ready);

                        }
                        catch (e) {
                            postGerarLog(e);
                        }

                    }
                });
                dom.byId("btnaddItemKit").appendChild(buttonItemKit.domNode);
                var buttonEscola = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasItemKit",
                    dropDown: menuItemKit,
                    id: "acoesRelacionadasItemKit"
                });
                dom.byId("linkItemKit").appendChild(buttonEscola.domNode);

                //Botao Incluir ItemKits de Contas
                


              
                //#region grade da escola
                if (jQuery.parseJSON(MasterGeral()) == true) {
                    var gridEscolas = new EnhancedGrid({
                            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                {
                                    name: "<input id='selecionaTodosEscola' style='display:none'/>",
                                    field: "selecionadoEscola",
                                    width: "25px",
                                    styles: "text-align: center;",
                                    formatter: formatCheckBoxEscola
                                },
                                { name: "Escola", field: "dc_reduzido_pessoa", width: "98%" }
                            ],
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
                        },
                        "gridEscolas");
                    gridEscolas.canSort = function(col) { return Math.abs(col) != 1; };
                    gridEscolas.startup();
                }

                

                new Button({
                        label: "Incluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                        onClick: function() {
                            try {
                                if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                    montargridPesquisaPessoa(function() {
                                        dojo.query("#_nomePessoaFK").on("keyup",
                                            function(e) {
                                                if (e.keyCode == 13) pesquisarEscolasFK();
                                            });
                                        dijit.byId("pesqPessoa").on("click",
                                            function(e) {
                                                apresentaMensagem("apresentadorMensagemProPessoa", null);
                                                pesquisarEscolasFK();
                                            });
                                        abrirPessoaFK(false);
                                    });
                                else
                                    abrirPessoaFK(false);
                            } catch (e) {
                                postGerarLog(e);
                            }
                        }
                    },
                    "incluirEscolaFK");

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Excluir",
                    onClick: function() {
                        deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_pessoa', gridEscolas);
                    }
                });
                menu.addChild(menuTodosItens);

                if (!hasValue(dijit.byId('todosItensEscola'))) {
                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "todosItensEscola",
                        dropDown: menu,
                        id: "todosItensEscola"
                    });
                    dom.byId("linkSelecionadosEscola").appendChild(button.domNode);
                } //if

                //#endregion
                new Button({
                        label: "Novo",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            try {
                                if (jQuery.parseJSON(MasterGeral()) == false) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                } else {

                                    limparFormItem();
                                        showCarregando();
                                        //setTimeout(esconderValoresItem(0, 0), 1000);

                                        loadDados("grupoItem", "tipoItens");
                                        var tabContainerKit = dijit.byId("tabContainerKit");
                                        tabContainerKit.selectChild(tabContainerKit.getChildren()[0]);
                                        apresentaMensagem('apresentadorMensagem', null);
                                        //*** if (jQuery.parseJSON(Master()) == true) criarGridEscola();
                                        IncluirAlterar(1,
                                            'divAlterarKit',
                                            'divIncluirKit',
                                            'divExcluirKit',
                                            'apresentadorMensagemKit',
                                            'divCancelarKit',
                                            'divLimparKit');

                                        dojo.byId('abriuEscola').value = false;

                                        setarTabCad();
                                        dijit.byId("cadKit").show();
                                        dijit.byId('tabContainerKit').resize();
                                        // $("#PanelAliquotas").show();
                                        $("#valoresEscola").show();

                                        limparGrid();
                                        //montarGridItemKit();
                                        showCarregando();
                                };

                            } catch (e) {
                                postGerarLog(e);
                                showCarregando();
                            }
                        }
                    },
                    "novoKit");
                //----Monta o botão de Relatório----

                new Button({
                        label: getNomeLabelRelatorio(),
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function() {
                            require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"],
                                function(dom, domAttr, xhr, ref) {
                                    var pes_tipoKitItem = hasValue(dijit.byId("pes_tipoKit").value) ? dijit.byId("pes_tipoKit").value : 0;
                                    var grupoItem = hasValue(dijit.byId("pes_grupoKit").value)
                                        ? dijit.byId("pes_grupoKit").value
                                        : 0;
                                    xhr.get({
                                        url: Endereco() +
                                            "/api/financeiro/GetUrlRelatorioKit?" +
                                            getStrGridParameters('gridKit') +
                                            "desc=" +
                                            encodeURIComponent(document.getElementById("_pes_nome").value) +
                                            "&inicio=" +
                                            document.getElementById("pes_inicioKit").checked +
                                            "&status=" +
                                            retornaStatus("pes_statusKit") +
                                            "&tipoItemInt=" +
                                            pes_tipoKitItem +
                                            "&grupoItem=" +
                                            grupoItem +
                                            "&categoria=" +
                                            retornaStatus("pes_categoriaKit") +
                                            "&escola=false",
                                        preventCache: true,
                                        handleAs: "json",
                                        headers: {
                                            "Accept": "application/json",
                                            "Content-Type": "application/json",
                                            "Authorization": Token()
                                        }
                                    }).then(function(data) {
                                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data,
                                                '1000px',
                                                '750px',
                                                'popRelatorio');
                                        },
                                        function(error) {
                                            apresentaMensagem('apresentadorMensagem', error);
                                        });
                                });
                        }
                    },
                    "relatorioKit");


                

                var buttonFkArray = ['pesquisarKit', 'pesquisarKit'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                //montarStatus("statusItem");
                //configuraMateriaisDidaticos(gridKit);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc423101931', '765px', '771px');
                        });


                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323066', '765px', '771px');
                        });

                }
                adicionarAtalhoPesquisa(['pes_pesquisaKit', 'pes_statusKit', 'pes_grupoKit', 'pes_tipoKit', 'pes_categoriaKit'],
                    'pes_pesquisaKit',
                    ready);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#endregion
function abrirKitFK(xhr, Memory, FilteringSelect, array, ready) {
    dojo.byId("tipoPesquisaFKItem").value = PESQUISA;
    if (!hasValue(dijit.byId("gridPesquisaItem"))) {

        //Alterar caracteristicas fk item
        dijit.byId("fkItem").set('title', "Pesquisar Kit");
        dijit.byId("comEstoque").set("disabled", false);
        dojo.byId('comEstoqueTitulo').style.display = 'inline';
        dojo.byId('comEstoqueTitulo').style.paddingTop = '5px';
        dojo.byId('comEstoqueCampo').style.display = 'contents';
        dojo.byId('comEstoqueCampo').style.paddingTop = '5px';
        dijit.byId("kit").set("checked", false);
        dijit.byId("kit").set("disabled", true);

        montargridPesquisaItem(function () {
            try {
                dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                limparPesquisaCursoFK(false);
                dijit.byId("tipo").reset();
                chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready, true);
                //abrirItemFK(xhr, Memory, FilteringSelect, array)
                dojo.query("#pesquisaItemServico").on("keyup", function (e) { if (e.keyCode == 13) chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready, true); });
                dijit.byId("pesquisarItemFK").on("click", function (e) {
                    try {
                        apresentaMensagem("apresentadorMensagemItemFK", null);
                        var tipoPesquisaFKItem = dojo.byId("tipoPesquisaFKItem");
                        if (hasValue(tipoPesquisaFKItem.value))
                            chamarPesquisaItemFK(tipoPesquisaFKItem.value, xhr, Memory, FilteringSelect, array, ready, true);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        }, false, true, false);
    }
    else {
        limparPesquisaCursoFK(false);
        dijit.byId("tipo").reset();
        chamarPesquisaItemFK(PESQUISA, xhr, Memory, FilteringSelect, array, ready, false);
        populaGrupoEstoque(null, 'grupoPes', false);
    }

}

function abrirItemFKCadastro(xhr, ready, Memory, array, popularTipoItem) {
    try {
       
        if (popularTipoItem)
            populaTipoItem("tipo", xhr, Memory, array, TIPOMOVIMENTO);
        //populaTipoItemMovimento(xhr, ready, Memory, FilteringSelect)
        pesquisarItemEstoqueFKCadastro(CADASTRO);
        
        //dijit.byId("tipo").set("disabled", false);
        //dijit.byId("tipo").reset();
        dijit.byId("fkItem").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirItemFK(xhr, Memory, FilteringSelect, array, kit) {
    try {
        populaTipoItem("tipo", xhr, Memory, array, TIPOMOVIMENTO, null, kit);
        pesquisarItemEstoqueFK();
        
        dijit.byId("tipo").set("disabled", false);
        dijit.byId("fkItem").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function pesquisarItemEstoqueFK() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
            var tipoItemInt = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : null;
            var id_natureza = 0;
            
            myStore = Cache(
                JsonRest({
                        target: Endereco() + "/fiscal/getItemMovimentoSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" +
                            retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItemInt + "&grupoItem=" + grupoItem + "&id_tipo_movto=" + TIPOMOVIMENTO +
                            "&comEstoque=" + document.getElementById("comEstoque").checked + "&id_natureza_TPNF=" + parseInt(id_natureza) + "&kit=" + document.getElementById("kit").checked,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_item"
                    }
                ), Memory({ idProperty: "cd_item" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaItem");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function chamarPesquisaItemFK(tipoPesquisa, xhr, Memory, FilteringSelect, array, ready, kit) {
    try {
        if (tipoPesquisa == PESQUISA)
            abrirItemFK(xhr, Memory, FilteringSelect, array, false);
        else
            abrirItemFKCadastro(xhr, ready, Memory, array, true);
    }
    catch (e) {
        postGerarLog(e);
    }
}
function montarGridItemKit() {
    require([
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dojo/on"
    ], function (EnhancedGrid, Pagination, ObjectStore, Memory, Button, ready, on) {
        ready(function () {
            try {
                //var dataMatKit = [
                //    { nm_qtde: 1, no_item: 'PASTA CRISTAL PVC' },
                //    { nm_qtde: 1, no_item: 'CANETA' },
                //    { nm_qtde: 1, no_item: 'LÁPIS' },
                //    { nm_qtde: 1, no_item: 'BORRACHA' },
                //    { nm_qtde: 1, no_item: 'PORTA LÁPIS' },
                //    { nm_qtde: 1, no_item: 'RÉGUA FLEX 20 CM' }
                //];
                
                
            }
            catch (e) {
                postGerarLog(e);
            }
        });

    });
}

function montarGridKit() {

    require([
        "dojo/_base/xhr",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready",
        "dojo/on",
        "dojo/store/JsonRest",
        "dojo/store/Cache"
    ], function (xhr, EnhancedGrid, Pagination, ObjectStore, Memory, Button, ready, on, JsonRest, Cache) {
        ready(function () {
            try {
            var myStore = Cache(
        JsonRest({
            target: Endereco() + "/api/financeiro/getKitSearch?desc=&inicio=false&status=1&tipoItemInt=" + dijit.byId("pes_tipoKit").value + "&grupoItem=70&categoria=1&escola=false",
            handleAs: "json",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": Token()
            },
            idProperty: "cd_item"
        }
        ),
        Memory({ idProperty: "cd_item" }));
            var gridKit = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    {
                        name: "<input id='selecionaTodosKit' style='display:none'/>",
                        field: "selecionadoKit",
                        width: "5%",
                        styles: "text-align:center; min-width:15px; max-width:20px;",
                        formatter: formatCheckBoxKit
                    },
                    //   { name: "Código", field: "cd_item", width: "5%", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Nome", field: "no_item", width: "30%" },
                    { name: "Grupo", field: "no_grupo_estoque", width: "15%", styles: "text-align: center;" },
                    { name: "Tipo", field: "dc_tipo_item", width: "15%", styles: "text-align: center;" },
                    { name: "Categoria", field: "categoria_grupo", width: "10%", styles: "text-align: center;" },
                    { name: "Ativo", field: "item_ativo", width: "10%", styles: "text-align: center;" }
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
                        position: "button",
                        plugins: { nestedSorting: true }
                    }
                }
            },
                "gridKit"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridKit.pagination.plugin._paginator.plugin.connect(gridKit.pagination.plugin._paginator,
                'onSwitchPageSize',
                function (evt) {
                    verificaMostrarTodos(evt, gridKit, 'cd_item', 'selecionaTodosKit');
                });
            var idGrupoItem = 0;
            gridKit.startup();
            gridKit.canSort = function (col) { return Math.abs(col) != 1; };
            gridKit.on("RowDblClick",
                function (evt) {
                    try {

                        if (jQuery.parseJSON(MasterGeral()) == false) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        } else {
                            limparGrid();
                            var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                            showCarregando();
                            limparFormItem();
                            dojo.byId('abriuEscola').value = false;
                            item.cd_grupo_estoque = item.cd_grupo_estoque == null ? 0 : item.cd_grupo_estoque;
                            xhr.get({
                                url: Endereco() +
                                    "/api/financeiro/returnDataKit?cdGrupo=" +
                                    item.cd_grupo_estoque +
                                    "&cdTipoItem=" +
                                    item.cd_tipo_item +
                                    "&tipoMovimento=",
                                preventCache: true,
                                handleAs: "json",
                                headers: {
                                    "Accept": "application/json",
                                    "Content-Type": "application/json",
                                    "Authorization": Token()
                                }
                            }).then(function(dataItem) {
                                    try {
                                        if (jQuery.parseJSON(MasterGeral()) == false)
                                            dijit.byId("tabContainerKit_tablist_escolaContentPane").domNode.style
                                                .visibility = 'hidden';
                                        dataItem = jQuery.parseJSON(dataItem);
                                        loadGrupoKit(dataItem.retorno.grupos, "grupoItem");
                                        loadTipoItemKit(dataItem.retorno.tipos, "tipoItens");
                                        apresentaMensagem('apresentadorMensagem', '');
                                        var tabContainerKit = dijit.byId("tabContainerKit");
                                        keepValues(null, item, gridKit, false);
                                        setarTabCad();
                                        dijit.byId("cadKit").show();
                                        dijit.byId('tabContainerKit').resize();
                                        dijit.byId("ativo").set("value", item.id_item_ativo);

                                        xhr.get({
                                            url: Endereco() + "/api/escola/getItensKit?idKit=" + item.cd_item,
                                            preventCache: true,
                                            handleAs: "json",
                                            headers: {
                                                "Accept": "application/json",
                                                "Content-Type": "application/json",
                                                "Authorization": Token()
                                            }
                                        }).then(function(dataItem) {
                                                try {

                                                    console.log(dataItem);

                                                    apresentaMensagem('apresentadorMensagemKit', null);
                                                    //var itensKit = jQuery.parseJSON(dataItem);
                                                    var itensKit = dataItem.retorno;
                                                    quickSortObj(itensKit, 'cd_item');
                                                    var kits_aux = [];
                                                    $.each(itensKit,
                                                        function(idx, value) {
                                                            insertObjSort(kits_aux,
                                                                "cd_item",
                                                                {
                                                                    cd_item: value.cd_item,
                                                                    cd_item_kit: value.cd_item_kit,
                                                                    qt_item_kit: value.qt_item_kit,
                                                                    no_item: value.no_item,
                                                                    cd_iitem_kit: value.cd_iitem_kit
                                                                });
                                                        });
                                                    var gridItemKit = dijit.byId('gridItemKit');
                                                    gridItemKit.setStore(new dojo.data.ObjectStore({
                                                        objectStore: new dojo.store.Memory({ data: kits_aux })
                                                    }));
                                                    //gridItemKit.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensKit }) }));
                                                    gridItemKit.update();

                                                    IncluirAlterar(0,
                                                        'divAlterarKit',
                                                        'divIncluirKit',
                                                        'divExcluirKit',
                                                        'apresentadorMensagemKit',
                                                        'divCancelarKit',
                                                        'divLimparKit');

                                                    showCarregando();

                                                } catch (er) {
                                                    postGerarLog(er);
                                                }
                                            },
                                            function(error) {
                                                apresentaMensagem('apresentadorMensagemCurso', error);
                                            });

                                        //getItensKit(item, xhr);


                                    } catch (er) {
                                        postGerarLog(er);
                                    }
                                },
                                function(error) {
                                    apresentaMensagem('apresentadorMensagemCurso', error);
                                });
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                true);

            require(["dojo/aspect"],
                function (aspect) {
                    aspect.after(gridKit,
                        "_onFetchComplete",
                        function () {
                            // Configura o check de todos:
                            if (dojo.byId('selecionaTodosKit').type == 'text')
                                setTimeout(
                                    "configuraCheckBox(false, 'cd_item', 'selecionadoKit', -1, 'selecionaTodosKit', 'selecionaTodosKit', 'gridKit')",
                                    gridKit.rowsPerPage * 3);
                        });
                });

            }
            catch (e) {
                postGerarLog(e);
            }
        });

    });

}

function getItensKit(item, xhr)
{
    try {
        //var item = hasValue(dojo.byId('cd_item').value) ? dojo.byId('cd_item').value : 0;
        var gridItemKit = dijit.byId('gridItemKit');
        xhr.get({
            url: Endereco() + "/api/escola/getItensKit?idKit=" + item.cd_item,
            preventCache: true,
            handleAs: "json",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": Token()
            }
        }).then(function (dataItem) {
            try {

                console.log(dataItem);

                apresentaMensagem('apresentadorMensagemKit', null);
                //var itensKit = jQuery.parseJSON(dataItem);
                var itensKit = dataItem.retorno;
                quickSortObj(itensKit, 'cd_item');
                var kits_aux = [];
                $.each(itensKit, function (idx, value) {
                    insertObjSort(kits_aux, "cd_item", {
                        cd_item: value.cd_item,
                        cd_item_kit: value.cd_item_kit,
                        qt_item_kit: value.qt_item_kit,
                        no_item: value.no_item,
                        cd_iitem_kit: value.cd_iitem_kit
                    });
                });
                var gridItemKit = dijit.byId('gridItemKit');
                gridItemKit.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: kits_aux }) }));
                //gridItemKit.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensKit }) }));
                gridItemKit.update();

                IncluirAlterar(0, 'divAlterarKit', 'divIncluirKit', 'divExcluirKit', 'apresentadorMensagemKit', 'divCancelarKit', 'divLimparKit');

                

            } catch (er) {
                postGerarLog(er);
            }
        },
       function (error) {
           apresentaMensagem('apresentadorMensagemKit', error);
       });
    }
    catch (e) {
        postGerarLog(e);
    }
    
}




function limparItem() {
    try {
        apresentaMensagem('apresentadorMensagemItem', null);
        getLimpar('#formItem');
        clearForm('formItem');
        IncluirAlterar(1, 'divAlterarItem', 'divIncluirItem', 'divExcluirItem', 'apresentadorMensagemItem', 'divCancelarItem', 'divLimparItem');
        document.getElementById("cd_item").value = '';
        dojo.byId("pes_categoriaKitGrupoCad").value = '';
    }
    catch (e) {
        postGerarLog(e);
    }
}



// #region Métodos de persistencia para Item Servico
function montarEscolasGrid() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        var listaEscolas = [];
        if (hasValue(gridEscolas)) {
            if (gridEscolas.store != null && gridEscolas.store.objectStore != null && gridEscolas.store.objectStore.data.length > 0)
                listaEscolas = [];
            if (hasValue(gridEscolas) && gridEscolas.store.objectStore.data.length > 0) {
                var escolas = gridEscolas.store.objectStore.data;
                for (var i = 0; i < escolas.length; i++) {
                    listaEscolas.push({
                        cd_pessoa: escolas[i].cd_pessoa,
                        no_pessoa: escolas[i].no_pessoa
                    });
                }
            }
        }
        return listaEscolas;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarItemSubgrupoGrid() {
    try {
        var gridItemKit = dijit.byId('gridItemKit');
        var listaItemKits = [];
        if (hasValue(gridItemKit)) {
            if (gridItemKit.store != null && gridItemKit.store.objectStore != null && gridItemKit.store.objectStore.data.length > 0)
                listaItemKits = [];
            if (hasValue(gridItemKit) && gridItemKit.store.objectStore.data.length > 0) {
                var planos = gridItemKit.store.objectStore.data;
                for (var i = 0; i < planos.length; i++) {
                    listaItemKits.push({
                        id_pes_tipoKit_movimento: planos[i].id_pes_tipoKit_movimento,
                        cd_item: dojo.byId("cd_item").value,
                        cd_subgrupo_conta: planos[i].cd_subgrupo_conta,
                        cd_item_subgrupo: planos[i].cd_item_subgrupo
                    });
                }
            }
        }
        return listaItemKits;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Item Servico
function PesquisarKit(limparItens) {
    var pes_tipoKitItem = hasValue(dijit.byId("pes_tipoKit").value) ? dijit.byId("pes_tipoKit").value : null;
    var grupoItem = hasValue(dijit.byId("pes_grupoKit").value) ? dijit.byId("pes_grupoKit").value : null;
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getKitSearch?desc=" + encodeURIComponent(dijit.byId("_pes_nome").value) + "&inicio=" + dijit.byId("pes_inicioKit").checked + "&status=" + retornaStatus("pes_statusKit") + "&tipoItemInt=" + pes_tipoKitItem + "&grupoItem=" + grupoItem + "&categoria=" + retornaStatus("pes_categoriaKit") + "&escola=false",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_item"
                }
                    ), Memory({ idProperty: "cd_item" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridKit = dijit.byId("gridKit");
            if (limparItens) {
                gridKit.itensSelecionados = [];
            }
            gridKit.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function ObjItem() {
    try {
        var quantidade = 0;

        var valorItem = 0;
        var valorCusto = 0;

        var titulo = null;
        var autor = "";
        var assunto = "";
        var local =  "";

        this.cd_item = null;
        this.no_item = dojo.byId("nome").value;
        this.dc_sgl_item = null;
        this.dc_classificacao_fiscal = null;
        this.dc_classificacao =  null;
        this.cd_integracao = "";
        this.pc_aliquota_icms =  0;
        this.pc_aliquota_iss =  0;
        this.dc_codigo_barra =  null;
        this.id_item_ativo = dijit.byId("ativo").checked;
        this.cd_tipo_item = dijit.byId("tipoItens").get("value");
        this.cd_grupo_estoque = dijit.byId("grupoItem").get("value");
        this.dc_tipo_item = dojo.byId("tipoItens").value;
        this.no_grupo_estoque = dojo.byId("grupoItem").value;
        this.cd_origem_fiscal = null;
        this.qt_estoque = quantidade;
        this.vl_item = valorItem;
        this.vl_custo = valorCusto;
        this.dc_titulo = titulo;
        this.no_autor = autor;
        this.dc_assunto = assunto;
        this.dc_local = local;
        this.cd_cest = null;
        this.id_kit = true;
        this.hasClickEscola = dojo.byId('abriuEscola').value;
        this.itemKit = hasValue(dijit.byId("gridItemKit").store.objectStore.data) ? dijit.byId("gridItemKit").store.objectStore.data : null;
        this.escolas = montarEscolasGrid();
        this.itemSubgrupo = null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//IncluirKit
function IncluirKit() {
    var masterGeral = jQuery.parseJSON(MasterGeral()) == true ? true : false;

    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemKit', null);
    var item = new ObjItem();

    if (hasValue(item.itemKit) && item.itemKit.length > 0) {
        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
            function(xhr, ref, windows) {
                if (!validarCamposKit(windows))
                    return false;

                showCarregando();

                xhr.post(Endereco() + "/api/escola/postInsertItemServico",
                    {
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        },
                        handleAs: "json",
                        data: ref.toJson(item)
                    }).then(function(data) {
                        try {
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridName = 'gridKit';
                                var grid = dijit.byId(gridName);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId("cadKit").hide();
                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                }
                                insertObjSort(grid.itensSelecionados, "cd_item", itemAlterado);
                                buscarItensSelecionados(gridName,
                                    'selecionadoKit',
                                    'cd_item',
                                    'selecionaTodosKit',
                                    ['pes_pesquisaKit', 'relatorioKit'],
                                    'todosItensKit');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_item");
                                showCarregando();

                            } else {
                                apresentaMensagem('apresentadorMensagemKit', data);
                                showCarregando();
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function(error) {
                        $('#aguardar').css('display', "none");
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemKit', error.response.data);
                    });
            });
    } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O kit dever ter pelo menos 1 item.");
            apresentaMensagem("apresentadorMensagemKit", mensagensWeb);
    }
}

// Deletar  item servico
function DeletarKit(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
        function (dom, xhr, ref) {
            showCarregando();
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_item').value != 0)
                    itensSelecionados = [
                        {
                            cd_item: dom.byId("cd_item").value,
                            no_item: dojo.byId("nome").value,
                            cd_grupo_estoque: dijit.byId("grupoItem").value
                        }
                    ];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeleteitemservico",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function(data) {
                    try {
                        data = jQuery.parseJSON(data);
                        if (!hasValue(data.erro)) {
                            var todos = dojo.byId("todosItensKit");
                            apresentaMensagem('apresentadorMensagem', data);
                            data = jQuery.parseJSON(data).retorno;
                            dijit.byId("cadKit").hide();
                            // Remove o item dos itens selecionados:
                            for (var r = itensSelecionados.length - 1; r >= 0; r--)
                                removeObjSort(dijit.byId('gridKit').itensSelecionados,
                                    "cd_item",
                                    itensSelecionados[r].cd_item);
                            PesquisarKit(true);
                            // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                            dijit.byId("pes_pesquisaKit").set('disabled', false);
                            dijit.byId("relatorioKit").set('disabled', false);
                            if (hasValue(todos))
                                todos.innerHTML = "Todos Itens";
                        } else {
                            if (!hasValue(dojo.byId("cadKit").style.display))
                                apresentaMensagem('apresentadorMensagemKit', error);
                            else
                                apresentaMensagem('apresentadorMensagem', error);
                        }
                        showCarregando();
                    } catch (e) {
                        showCarregando();
                        postGerarLog(e);
                    }
                
                },
                function (error) {
                    showCarregando();
                    if (!hasValue(dojo.byId("cadKit").style.display))
                        apresentaMensagem('apresentadorMensagemKit', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        });
}

function AlterarKit() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemKit', null);

        var item = new ObjItem();
        item.cd_item = dojo.byId("cd_item").value;

        if (hasValue(item.itemKit) && item.itemKit.length > 0) {

            if (jQuery.parseJSON(MasterGeral()) == true) {
                var gridEscola = dijit.byId('gridEscolas');
                var hasClickEscola = eval(dojo.byId('abriuEscola').value);

                if ((gridEscola.store == null ||
                        gridEscola.store.objectStore == null ||
                        gridEscola.store.objectStore.data.length <= 0) &&
                    (hasClickEscola)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgEscolaDeverSerInserida);
                    apresentaMensagem("apresentadorMensagemKit", mensagensWeb);
                    return false;
                }
            }
            // $('#aguardar').append('<img style="position: relative;left: 158px; top: 79px" src="/images/carregando.gif"/>').css('display', "");

            require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function(dom, xhr, ref, windows) {

                    if (!validarCamposKit(windows))
                        return false;

                    showCarregando();

                    xhr.post(Endereco() + "/api/escola/postEditKit",
                        {
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            handleAs: "json",
                            data: ref.toJson(item)
                        }).then(function(data) {
                            try {
                                $('#aguardar').css('display', "none");
                                data = jQuery.parseJSON(data);
                                if (!hasValue(data.erro)) {
                                    var itemAlterado = data.retorno;
                                    var gridName = 'gridKit';
                                    var grid = dijit.byId(gridName);
                                    apresentaMensagem('apresentadorMensagem', data);
                                    dijit.byId("cadKit").hide();
                                    if (!hasValue(grid.itensSelecionados)) {
                                        grid.itensSelecionados = [];
                                    }
                                    removeObjSort(grid.itensSelecionados, "cd_item", dom.byId("cd_item").value);
                                    insertObjSort(grid.itensSelecionados, "cd_item", itemAlterado);
                                    buscarItensSelecionados(gridName,
                                        'selecionadoKit',
                                        'cd_item',
                                        'selecionaTodosKit',
                                        ['pes_pesquisaKit', 'relatorioKit'],
                                        'todosItensKit');
                                    grid.sortInfo =
                                        2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                    setGridPagination(grid, itemAlterado, "cd_item");
                                    showCarregando();
                                } else {
                                    apresentaMensagem('apresentadorMensagemKit', data);
                                    showCarregando();
                                }
                            } catch (er) {
                                postGerarLog(er);
                            }
                        },
                        function(error) {
                            showCarregando();
                            apresentaMensagem('apresentadorMensagemKit', error.response.data);
                        });
                });

        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O kit dever ter pelo menos 1 item.");
            apresentaMensagem("apresentadorMensagemKit", mensagensWeb);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarCamposKit(windowUtils) {
    try {
        var validado = true;
        var tabs = dijit.byId("tabContainerKit");
        var pane = dijit.byId("tabPrincipal");

        if (!dijit.byId("formKit").validate()) {
            validado = false;
            tabs.selectChild(pane);
        }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion
function eventoEditarKit(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            dojo.byId('abriuEscola').value = false;
            itensSelecionados[0].cd_grupo_estoque = itensSelecionados[0].cd_grupo_estoque == null ? 0 : itensSelecionados[0].cd_grupo_estoque;
            limparFormItem();
            dojo.xhr.get({
                url: Endereco() + "/api/financeiro/returnDataKit?cdGrupo=" + itensSelecionados[0].cd_grupo_estoque + "&cdTipoItem=" + itensSelecionados[0].cd_tipo_item + "&tipoMovimento=",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataItem) {
                if (jQuery.parseJSON(MasterGeral()) == false)
                    dijit.byId("tabContainerKit_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                dataItem = jQuery.parseJSON(dataItem);
                loadGrupoKit(dataItem.retorno.grupos, "grupoItem");
                loadTipoItemKit(dataItem.retorno.tipos, "tipoItens");
                apresentaMensagem('apresentadorMensagem', '');
                var tabContainerKit = dijit.byId("tabContainerKit");
                keepValues(null, null, dijit.byId('gridKit'), true);
                setarTabCad();
                dijit.byId("cadKit").show();
                dijit.byId('tabContainerKit').resize();
                dijit.byId("ativo").set("value", itensSelecionados[0].id_item_ativo);
                IncluirAlterar(0, 'divAlterarKit', 'divIncluirKit', 'divExcluirKit', 'apresentadorMensagemKit', 'divCancelarKit', 'divLimparKit');
                limparGrid();
                getItensKit(itensSelecionados[0], dojo.xhr);
                showCarregando();
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion


function getEmpresasGridEscolas() {
    var listaEmpresasGrid = "";

    if (hasValue(dijit.byId("gridEscolas").store.objectStore.data))
        $.each(dijit.byId("gridEscolas").store.objectStore.data, function (index, value) {
            listaEmpresasGrid += value.cd_pessoa + ",";
        });
    return listaEmpresasGrid;
}

//#region  eventos para a aba de escola
function pesquisarEscolasFK() {
    var item = hasValue(dojo.byId('cd_item').value) ? dojo.byId('cd_item').value : 0;
    var listaEmpresasGrid = getEmpresasGridEscolas();
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function() {
                try {
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/escola/getEscolaNotWithKit?nome=" + dojo.byId("_nomePessoaFK").value + "&cdEmpresas=" + listaEmpresasGrid + "&fantasia=" + dojo.byId("_apelido").value + "&cnpj=" + dojo.byId("CnpjCpf").value + "&cd_item=" + item + "&inicio=" + document.getElementById("inicioPessoaFK").checked,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }),
                        Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoa");
                    grid.setStore(dataStore);
                    grid.layout.setColumnVisibility(5, false)
                } catch (e) {
                    postGerarLog(e);
                }
            });
        });
}

function abrirPessoaFK(isPesquisa) {
    dojo.ready(function () {
        try {
            dijit.byId("fkPessoaPesq").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("fkPessoaPesq").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaEscolaFK() {
    try {
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGrid() {
    try {
        var gridItemKit = dijit.byId('gridItemKit');
        if (hasValue(gridItemKit)) {
            gridItemKit.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridItemKit.update();
        }

        var gridEscolas = dijit.byId('gridEscolas');
        if (hasValue(gridEscolas)) {
            gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridEscolas.update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        var gridEscolas = dijit.byId("gridEscolas");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("cadKit").open) {
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'cd_pessoa');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "cd_pessoa", {
                        cd_pessoa: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa
                    });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
            }

        if (!valido)
            return false;
        dijit.byId("fkPessoaPesq").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}