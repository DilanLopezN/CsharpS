//var CONSULTA_PRODUTO_HAS_CURSO = 4;
//var CONSULTA_MODALIDADE_HAS_CURSO = 2;
var TIPOMOVIMENTOITEMFK = null;
var TIPO_MATERIAL_DIDATICO = 1;
var SAIDA = 2;
var ALUNOSEMAULA = 33;
var IS_MATERIAL_DIDATICO = false;
var ORIGEM_ITEM_SEARCH_PERDA_MATERIAL = 44, ORIGEM_ITEM_FK_MOVIMENTO = 45, ORIGEM_ITEM_CAD_FK_MOVIMENTO = 46;

function montargridPesquisaItem(funcao, popularTipoItem, popularGrupoItem, kit) {
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
       "dojo/on",
       "dojo/_base/array"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, array) {
        ready(function () {
            try {
                

                if (popularTipoItem)
                    populaTipoItem('tipo', xhr, Memory, array, TIPOMOVIMENTOITEMFK);

                if (!hasValue(dijit.byId("statusItemFK"))) {
                    montarStatus("statusItemFK");
                }
                
                if (!hasValue(dijit.byId("mes_items"))) {
                    loadMes("mes_items");
                }

                if (popularGrupoItem)
                    populaGrupoEstoque(null, 'grupoPes', kit);

                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getItemSearch?desc=&inicio=false&status=1&tipoItemInt=1&grupoItem=null&categoria=0&escola=false",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_item"
                    }), Memory({ idProperty: "cd_item" }));

                //*** Cria a grade de Cursos **\\
                var gridPesquisaItem = new EnhancedGrid({
                    //store: ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosItemFK' style='display:none'/>", field: "selecionadoItemFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxItemFK },
                        //{ name: "Código", field: "cd_curso", width: "75px", styles: "width:75px; text-align: left;" },
                        { name: "Item", field: "no_item", width: "40%" },
                        { name: "Grupo", field: "no_grupo_estoque", width: "17%", styles: "text-align: center;" },
                        { name: "Tipo", field: "dc_tipo_item", width: "18%", styles: "text-align: center;" },
                        { name: "Qtde.", field: "qt_estoque", width: "10%", styles: "text-align: right;" },
                        { name: "Ativo", field: "item_ativo", width: "8%", styles: "text-align: center;" }

                        ],
                    noDataMessage: msgNotRegEncFiltro,
                    plugins: {
                        pagination: {
                            pageSizes: ["14", "30", "45", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "14",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridPesquisaItem");
                gridPesquisaItem.on("RowDblClick", function () {
                    retornarItemFK();
                }, true);
                gridPesquisaItem.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3 && Math.abs(col) != 4 && Math.abs(col) != 5 && Math.abs(col) != 6; };
                gridPesquisaItem.pagination.plugin._paginator.plugin.connect(gridPesquisaItem.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaItem, 'cd_item', 'selecionaTodosItemFK');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPesquisaItem, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosItemFK').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_curso', 'selecionadoItemFK', -1, 'selecionaTodosItemFK', 'selecionaTodosItemFK', 'gridPesquisaItem')", gridPesquisaItem.rowsPerPage * 3);
                    });
                });
                gridPesquisaItem.startup();

                new Button({
                    label: "",
                    onClick: function () {
                        try {
                            //CONSULTA_ITEM = true;
                            var tipo = parseInt(dojo.byId('tipoRetornoItemFK').value);
                            switch (tipo) {
                                case ALUNOSEMAULA: {
                                    if (hasValue(pesquisarItemFK)) {
                                        apresentaMensagem('apresentadorMensagem', null);

                                        pesquisarItemFK(ALUNOSEMAULA);
                                    }
                                    break;
                                }
                                case ORIGEM_ITEM_SEARCH_PERDA_MATERIAL:
                                {
                                        if (hasValue(pesquisarItemEstoqueFK)) {
                                        apresentaMensagem('apresentadorMensagem', null);

                                        pesquisarItemEstoqueFK(ORIGEM_ITEM_SEARCH_PERDA_MATERIAL);
                                    }
                                    break;
                                }
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }

                    },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarItemFK");

                decreaseBtn(document.getElementById("pesquisarItemFK"), '32px');
                new Button({ label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () { 
                    if (hasValue(retornarItemFK))
                        retornarItemFK();
                }
                }, "selecionaItemFK");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        if(hasValue(dijit.byId("proItem")))
                            dijit.byId("proItem").hide();
                    }
                }, "fecharItemFK");
                adicionarAtalhoPesquisa(['pesquisaItemServico', 'statusItemFK', 'grupoPes', 'tipo'], 'pesquisarItemFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function populaGrupoEstoque(idGrupo, field, kit) {
    // Popula os produtos:
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/GetAllGrupoEstoqueItem",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataGrupoAtivo) {
        try {
            var listaGrupos = jQuery.parseJSON(dataGrupoAtivo).retorno;
            loadGrupoEstoque(listaGrupos, field, idGrupo, kit);            
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemItemFK', error);
    });
}

function loadGrupoEstoque(items, linkGrupo, idGrupo, kit) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbGrupo = dijit.byId(linkGrupo);
            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_grupo_estoque, name: value.no_grupo_estoque });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbGrupo.store = stateStore;
            if (hasValue(idGrupo)) {
                cbGrupo._onChangeActive = false;
                cbGrupo.set("value", idGrupo);
                cbGrupo._onChangeActive = true;
            }

            if (kit) {
                var indexSelected = indexValue(items, "no_grupo_estoque", "Kit de vendas");
                dijit.byId("grupoPes").set("value", items[indexSelected].cd_grupo_estoque);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function populaTipoItem(field, xhr, Memory, array, tipoMovimentoItemFK, defaultValue, kit) {
    tipoMovimentoItemFK = tipoMovimentoItemFK == null ? 0 : tipoMovimentoItemFK;
    xhr.get({
        url: Endereco() + "/api/financeiro/getalltipoitem?tipoMovimento=" + tipoMovimentoItemFK,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataTipoItem) {
        try {
            var listaItems = jQuery.parseJSON(dataTipoItem).retorno;
            loadTipoItem(listaItems, field);
            if (hasValue(defaultValue))
                dijit.byId(field).set("defaultValue", defaultValue);

            if (kit) {
                var indexSelected = indexValue(listaItems, "dc_tipo_item", "Material Didático");
                dijit.byId("tipo").set("value", listaItems[indexSelected].cd_tipo_item);
            }

            var parametrosTela = getParamterosURL();
            if (dijit.byId("tipo") != null && dijit.byId("tipo") != undefined && (((((parametrosTela['id_material_didatico'] != null && parametrosTela['id_material_didatico'] != undefined) &&
                eval(parametrosTela['id_material_didatico']) == 1))))) {
                dijit.byId("tipo").set("value", MATERIAL_DIDATICO);
                // dijit.byId("tipo").set("disabled", true); LBM liberando para outros itens, para evitar 2 notas. 
            }

            if (typeof VINCULA_MATERIAL_FILTRO !== 'undefined' &&
                VINCULA_MATERIAL_FILTRO !== null ) {
                dijit.byId("tipo").set("value", MATERIAL_DIDATICO);
                dijit.byId("tipo").set("disabled", true);
            }

            if (IS_MATERIAL_DIDATICO == true) {
                dijit.byId("tipo").set("value", TIPO_MATERIAL_DIDATICO);
                dijit.byId("tipo").set("disabled", true);
            }

        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemItemFK', error);
    });
 }

function loadTipoItem(items, field) {
    
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var itemsCb = [];
            var cbTipoItem = dijit.byId(field);
            var comCdTipoItem = dojo.byId("cdTipoItem");

            itemsCb.push({
                id: 0,
                name: "Todos"
            });

            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_tipo_item, name: value.dc_tipo_item });
            });

            var stateStore = new Memory({
                data: itemsCb
            });
            cbTipoItem.store = stateStore;
            if (hasValue(comCdTipoItem.value) && comCdTipoItem.value > 0)
                cbTipoItem.set("value", 1);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarItemFK(tipo) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : null;
            var tipoItem = 0;
            if (tipo == ALUNOSEMAULA)
                tipoItem = -1 * TIPO_MATERIAL_DIDATICO;
            else {
                var tipoItem = hasValue(tipo) ? tipo : dijit.byId("tipo").value;
                if (tipo == TIPO_MATERIAL_DIDATICO) tipoItem = -1 * tipoItem;
            }
            if (tipo != ALUNOSEMAULA)
            myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getItemSearch?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" + retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItem + "&grupoItem=" + grupoItem + "&categoria=0&escola=false",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_item"
                }
                ), Memory({ idProperty: "cd_item" }));
            else
                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getItemSearchAlunosemAula?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" + retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItem + "&grupoItem=" + grupoItem + "&categoria=0&escola=false",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_item"
                    }
                    ), Memory({ idProperty: "cd_item" }));

                dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaItem");
            grid.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function formatCheckBoxItemFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaItem';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosItemFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item", grid._by_idx[rowIndex].item.cd_item);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBoxItemFK'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoItemFK', -1, 'selecionaTodosItemFK', 'selecionaTodosItemFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item', 'selecionadoItemFK', " + rowIndex + ", '" + id + "', 'selecionaTodosItemFK', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaCursoFK() {
    try{
        dojo.byId("pesquisaItemServico").value = "";
        dijit.byId("inicioItemServico").set("checked", false);
        dijit.byId("statusItemFK").reset();
        dijit.byId("grupoPes").reset();
        if (hasValue(dijit.byId("gridPesquisaItem")) && hasValue(dijit.byId("gridPesquisaItem").itensSelecionados))
            dijit.byId("gridPesquisaItem").itensSelecionados = [];
        dijit.byId("gridPesquisaItem").update();
        dijit.byId("comEstoque").set("checked", false);
    } catch (e) {
        postGerarLog(e);
    }
}

function limparItensSelecionados() {
    if (hasValue(dijit.byId("gridPesquisaItem")))
        dijit.byId("gridPesquisaItem").itensSelecionados = [];
}

function loadMes(nomElement) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try {
             var statusStore = null;

             statusStore = new Memory({
                 data: [
                     { name: "", id: "0" },
                     { name: "1", id: "1" },
                     { name: "2", id: "2" },
                     { name: "3", id: "3" },
                     { name: "4", id: "4" },
                     { name: "5", id: "5" },
                     { name: "6", id: "6" },
                     { name: "7", id: "7" },
                     { name: "8", id: "8" },
                     { name: "9", id: "9" },
                     { name: "10", id: "10" },
                     { name: "11", id: "11" },
                     { name: "12", id: "12" }
                 ]
             });
                 
             var status = new filteringSelect({
                 id: nomElement,
                 name: "status",
                 value: "0",
                 store: statusStore,
                 searchAttr: "name",
                 style: "width: 75px;"
             }, nomElement);
         }
         catch (e) {
             postGerarLog(e);
         }
     });
};

function indexValue(arraytosearch, key, valuetosearch) {

    for (var i = 0; i < arraytosearch.length; i++) {

        if (arraytosearch[i][key].toLowerCase() == valuetosearch.toLowerCase()) {
            return i;
        }
    }
    return null;
}