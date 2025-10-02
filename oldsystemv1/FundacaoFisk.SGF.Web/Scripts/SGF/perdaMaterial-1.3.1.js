var ENTRADA = 1, SAIDA = 2, DESPESAS = 3, SERVICO = 4, DEVOLUCAO = 5;
var TIPOMOVIMENTO = SAIDA;
var PESQUISA = 1;
var CONSULTA_ITEM = false;
var ORIGEM_ITEM_FK = 0;

var ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL = 44, ORIGEM_ALUNO_FK_MOVIMENTO = 45;
var ORIGEM_ITEM_SEARCH_PERDA_MATERIAL = 44, ORIGEM_ITEM_FK_MOVIMENTO = 45, ORIGEM_ITEM_CAD_FK_MOVIMENTO = 46;
var ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL = 1, ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL = 2;

var TIPO_STATUS_PERDA_MATERIAL = {
    FECHADO: 2,
    ABERTO: 1,
}

function montarTipoStatus(Memory, id) {
    require(["dojo/store/Memory"],
        function (Memory) {
            try {
                var stateStore = new Memory({
                    data: [
                        { name: "Todos", id: 0 },
                        { name: "Fechado", id: 2 },
                        { name: "Aberto", id: 1 }
                    ]
                });
                id.store = stateStore;
                id._onChangeActive = false;
                id.set("value", 1);
                id._onChangeActive = true;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function montarTipoStatusCad(Memory, id, value) {
    require(["dojo/store/Memory"],
        function (Memory) {
            try {
                var stateStore = new Memory({
                    data: [
                        { name: "Todos", id: -1 },
                        { name: "Fechado", id: 2 },
                        { name: "Aberto", id: 1 }
                    ]
                });
                id.store = stateStore;
                id._onChangeActive = false;
                id.set("value", value);
                id._onChangeActive = true;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}
//Pega os Antigos dados do Formulário
function keepValues(tipoForm, grid, ehLink) {
    try {
        apresentaMensagem('apresentadorMensagemPerdaMaterial', null);
        var value = grid.itemSelecionado;
        var linkAnterior = document.getElementById('link');
        var gridMatDid = dijit.byId('gridItemsMovimentoMaterialDidatico');

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        if (hasValue(value)) {

            dojo.byId("cd_perda_material").value = value.cd_perda_material;
            dojo.byId("cd_nfDevCad").value = value.cd_movimento;
            dijit.byId("tpNfDevCad").set("value", value.dc_nm_movimento == "" ? "S/N" : value.dc_nm_movimento);
            dijit.byId("limparNFDevCad").set("disabled", false);

            if (value.no_aluno != null &&
                value.no_aluno.trim().length > 0) {
                dojo.byId("cd_alunoCad").value = value.cd_aluno;
                dijit.byId("nomeAlunoCad").set("value", value.no_aluno);


            } else {
                dojo.byId("cd_alunoCad").value = null;
                dijit.byId("nomeAlunoCad").set("value", "");

                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Aluno não encotrado para o movimento: " + (value.dc_numero_serie == "" ? "S/N" : value.dc_numero_serie));
                apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            }

            if (value.cd_contrato != null && value.cd_contrato > 0 &&
                value.nm_contrato != null &&
                value.nm_contrato > 0) {
                dojo.byId("cd_contratoCad").value = value.cd_contrato;
                dijit.byId("nomeIdContratoCad").set("value", ("Número:" + value.nm_contrato + "/ Id: " + value.cd_contrato));


            }
            else {
                dojo.byId("cd_contratoCad").value = null;
                dijit.byId("nomeIdContratoCad").set("value", "");

                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Contrato não encotrado para o movimento: " + (value.dc_numero_serie == "" ? "S/N" : value.dc_numero_serie));
                apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            }

            dijit.byId("dt_perda_material").set("value", value.dt_perda_material)

            dijit.byId("statusCad").set("value", value.id_status_perda);

            /*populaModalidades(value.cd_modalidade, 'cbModalidades', CONSULTA_MODALIDADE_HAS_ATIVO);
            dijit.byId('cbProximoPerdaMaterial').set('value', value.cd_proximo_curso);

            dijit.byId('ckCertificado').value = value.id_certificado == true ? dijit.byId("ckCertificado").set("value", true) : dijit.byId("ckCertificado").set("value", false);
            pesquisarMatDid();*/

            if (hasValue(value.itensPerdaMaterial)) {
                var grid = dijit.byId("gridItemsMovimentoMaterialDidatico");
                var dataStore = new dojo.data.ObjectStore({
                    objectStore: new dojo.store.Memory({
                        data: value.itensPerdaMaterial,
                        idProperty: "cd_item_movimento"
                    })
                });
                grid.setStore(dataStore);
            } else {
                var grid = dijit.byId("gridItemsMovimentoMaterialDidatico");
                var dataStore = new dojo.data.ObjectStore({
                    objectStore: new dojo.store.Memory({
                        data: [],
                        idProperty: "cd_item_movimento"
                    })
                });
                grid.setStore(dataStore);
            }

            
            
        }
        else
            // Seleciona os materiais didáticos:
            configuraMateriaisDidaticos(gridMatDid);
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatterItensPerdaMaterial(value, rowIndex, obj) {
    try {

        console.log(value);
        console.log(rowIndex);
        console.log(obj);


    } catch (e) {
        postGerarLog(e);
    }
}



function formatCheckBoxMatDid(value, rowIndex, obj) {
    try {
        var gridName = 'gridItemsMovimentoMaterialDidatico';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMatDid');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item_curso", grid._by_idx[rowIndex].item.cd_item_curso);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:10px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item_curso', 'selecionadoMatDid', -1, 'selecionaTodosMatDid', 'selecionaTodosMatDid', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item_curso', 'selecionadoMatDid', " + rowIndex + ", '" + id + "', 'selecionaTodosMatDid', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxPerdaMaterial(value, rowIndex, obj) {
    try {
        var gridName = 'gridPerdaMaterial';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPerdaMaterial');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_perda_material", grid._by_idx[rowIndex].item.cd_perda_material);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_perda_material', 'selecionadoPerdaMaterial', -1, 'selecionaTodosPerdaMaterial', 'selecionaTodosPerdaMaterial', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_perda_material', 'selecionadoPerdaMaterial', " + rowIndex + ", '" + id + "', 'selecionaTodosPerdaMaterial', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}




function montarCadastroPerdaMaterial() {
    //Criação da Grade
    require([
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
        "dojo/on",
        "dojo/_base/xhr",
        "dijit/form/FilteringSelect",
        "dojo/_base/array",
    ], function (EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, on, xhr, FilteringSelect, array) {
        ready(function () {
            try {

                montarTipoStatus(Memory, dijit.byId("_tipoStatus"));
                //Para alinhar o painel do dojo, verificar se isso ocorre no desenvolvimento:
                dojo.byId('tabContainer_tablist').children[3].children[0].style.width = '100%';
                // Corrige o tamanho do pane que o dojo cria para o dialog com scroll no ie7:
                if (/MSIE (\d+\.\d+);/.test(navigator.userAgent)) {
                    var ieversion = new Number(RegExp.$1)
                    if (ieversion == 7)
                        // Para IE7
                        dojo.byId('cad').childNodes[1].style.height = '100%';
                }

                /*var myStore = Cache(
                    JsonRest({
                        target:  Endereco() + "/api/curso/getPerdaMaterialSearch?desc=" + encodeURIComponent(document.getElementById("descPerdaMaterial").value) + "&inicio=" + document.getElementById("inicioPerdaMaterial").checked + "&status=1" + "&produto=" + dijit.byId('cbPesqProduto').value + "&estagio=" + dijit.byId('cbPesqEstagios').value + "&modalidade=" + dijit.byId('cbPesqModalidades').value + "&nivel=" + dijit.byId('cbPesqNivel').value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                    ), Memory({}));*/

                //*** Cria a grade de PerdaMaterials **\\
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                var gridPerdaMaterial = new EnhancedGrid({
                    store: dataStore,
                    structure:
                        [
                            { name: "<input id='selecionaTodosPerdaMaterial' style='display:none'/>", field: "selecionadoPerdaMaterial", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPerdaMaterial },
                            //{ name: "Código", field: "cd_perda_material", width: "75px", styles: "width:75px; text-align: left;" },
                            { name: "Aluno", field: "no_aluno", width: "25%", styles: "min-width:80px;" },
                            { name: "Contrato", field: "nm_contrato", width: "20%", styles: "min-width:80px;" },
                            { name: "Movimento", field: "dc_nm_movimento", width: "20%", styles: "min-width:80px;" },
                            { name: "Emissão", field: "dta_perda_material", width: "20%", styles: "min-width:80px;" },
                            { name: "Item", field: "items", width: "20%", styles: "min-width:80px;" },
                            { name: "Status", field: "dc_status_perda", width: "10%", styles: "text-align: center; min-width:40px; max-width: 50px;" }
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
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridPerdaMaterial");
                gridPerdaMaterial.canSort = function (col) { return Math.abs(col) != 1 &&  Math.abs(col) != 6; };
                gridPerdaMaterial.startup();
                gridPerdaMaterial.on("RowDblClick", function (evt) {
                    try {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        limparPerdaMaterial();
                        dijit.byId("material").set("open", true);
                        apresentaMensagem('apresentadorMensagem', '');
                        gridPerdaMaterial.itemSelecionado = item;
                        keepValues(null, gridPerdaMaterial, false);
                        dijit.byId("cad").show();
                        IncluirAlterar(0, 'divAlterarPerdaMaterial', 'divIncluirPerdaMaterial', 'divExcluirPerdaMaterial', 'apresentadorMensagemPerdaMaterial', 'divCancelarPerdaMaterial', 'divLimparPerdaMaterial');
                        dijit.byId('tabContainer').resize();
                        dijit.byId("gridItemsMovimentoMaterialDidatico").update();
                        
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                IncluirAlterar(1, 'divAlterarPerdaMaterial', 'divIncluirPerdaMaterial', 'divExcluirPerdaMaterial', 'apresentadorMensagemPerdaMaterial', 'divCancelarPerdaMaterial', 'divLimparPerdaMaterial');

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPerdaMaterial, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosPerdaMaterial').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_perda_material', 'selecionadoPerdaMaterial', -1, 'selecionaTodosPerdaMaterial', 'selecionaTodosPerdaMaterial', 'gridPerdaMaterial')", gridPerdaMaterial.rowsPerPage * 3);
                    });
                });

                if (!hasValue(gridPerdaMaterial.itensSelecionados))
                    gridPerdaMaterial.itensSelecionados = [];

                //myStore = Cache(
                //    JsonRest({
                //        target: "/api/financeiro/getItemPerdaMaterial?cdPerdaMaterial=" + dojo.byId("cd_perda_material").value,
                //        handleAs: "json",
                //        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                //    }
                //), Memory({}));
                var data = new Array();

                var gridItemsMovimentoMaterialDidatico = new EnhancedGrid({
                    //store: ObjectStore({ objectStore: myStore }),
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "cd_item" }) }),
                    structure:
                        [
                            { name: "<input id='selecionaTodosMatDid' style='display:none'/>", field: "selecionadoMatDid", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMatDid },
                            { name: "Itens", field: "dc_item_movimento", width: "20%" },
                            { name: "Qtde.", field: "qt_item_movimento", width: "7%", styles: "text-align:center; min-width:15px; max-width:20px;" },
                            { name: "R$ Unitário", field: "vlUnitarioItem", width: "10%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                            { name: "Desc. (%)", field: "pcDescontoItem", width: "9%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                            { name: "Desc.", field: "vlDescontoItem", width: "6%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                            { name: "Valor Total", field: "vlTotalItem", width: "10%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                            { name: "Valor Líquido", field: "vlLiquidoItem", width: "12%", styles: "text-align:right; min-width:15px; max-width:20px;" },
                            { name: "Plano de Contas", field: "dc_plano_conta", width: "15%" },
                            { name: "Grupo de Estoque", field: "cd_grupo_estoque", width: "15%", styles: "display: none;" }
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
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridItemsMovimentoMaterialDidatico");
                gridItemsMovimentoMaterialDidatico.canSort = function (col) {  };
                gridItemsMovimentoMaterialDidatico.startup();

                if (!hasValue(gridItemsMovimentoMaterialDidatico.itensSelecionados))
                    gridItemsMovimentoMaterialDidatico.itensSelecionados = [];

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridItemsMovimentoMaterialDidatico, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosMatDid').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_item', 'selecionadoMatDid', -1, 'selecionaTodosMatDid', 'selecionaTodosMatDid', 'gridItemsMovimentoMaterialDidatico')", gridItemsMovimentoMaterialDidatico.rowsPerPage * 3);
                    });
                });

                var params = getParamterosURL();

               
                //*** Cria os botões do link de ações **\\
                // Adiciona link de selecionados PerdaMaterial:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        buscarTodosItens(gridPerdaMaterial, 'todosItensPerdaMaterial', ['pesquisarPerdaMaterial', 'relatorioPerdaMaterial']);
                        pesquisarPerdaMaterial(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        buscarItensSelecionados('gridPerdaMaterial', 'selecionadoPerdaMaterial', 'cd_perda_material', 'selecionaTodosPerdaMaterial', ['pesquisarPerdaMaterial', 'relatorioPerdaMaterial'], 'todosItensPerdaMaterial');
                    }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensPerdaMaterial",
                    dropDown: menu,
                    id: "todosItensPerdaMaterial"
                });
                dom.byId("linkSelecionadosPerdaMaterial").appendChild(button.domNode);




                menu = new DropDownMenu({ style: "height: 25px" });
                

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditar(gridPerdaMaterial.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);



                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridPerdaMaterial.itensSelecionados, 'deletarPerdaMaterial(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                var acaoEditar = new MenuItem({
                    label: "Processar Perda Material",
                    onClick: function () {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        processarPerdaMaterial(gridPerdaMaterial.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);


                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasPerdaMaterial",
                    dropDown: menu,
                    id: "acoesRelacionadasPerdaMaterial"
                });
                dom.byId("linkAcoesPerdaMaterial").appendChild(button.domNode);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            dojo.byId('tipoRetornoItemFK').value = ORIGEM_ITEM_SEARCH_PERDA_MATERIAL;
                            if (!hasValue(dijit.byId("pesquisarItemFK"))) {
                                montargridPesquisaItem(function () {
                                    ORIGEM_ITEM_FK = ORIGEM_ITEM_SEARCH_PERDA_MATERIAL;
                                    abrirItemFK(xhr, Memory, FilteringSelect, array, false);
                                    dijit.byId("pesquisarItemFK").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemItemFK", null);
                                        pesquisarItemEstoqueFK(ORIGEM_ITEM_SEARCH_PERDA_MATERIAL);
                                    });
                                    dijit.byId("fecharItemFK").on("click", function (e) {
                                        dijit.byId("fkItem").hide();
                                    });
                                }, true, true);
                            } else {
                                ORIGEM_ITEM_FK = ORIGEM_ITEM_SEARCH_PERDA_MATERIAL;
                                abrirItemFK(xhr, Memory, FilteringSelect, array, false);
                                dijit.byId("fecharItemFK").on("click", function (e) {
                                    dijit.byId("fkItem").hide();
                                });
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesProItemFK");
                new Button({
                    label: "Limpar", iconClass: '', onClick: function () {
                        dojo.byId('cd_item_pesq').value = '';
                        dojo.byId("item_pesq").value = "";
                        dijit.byId('limparItemFiltroFK').set('disabled', true);
                    },
                    disabled: true
                }, "limparItemFiltroFK");
                if (hasValue(document.getElementById("limparItemFiltroFK"))) {
                    document.getElementById("limparItemFiltroFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparItemFiltroFK").parentNode.style.width = '40px';
                }

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            TIPO_PESQUISA = PESQUISA;
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagem',
                                    MOVIMENTO, TIPO_PESQUISA);
                            }
                            else
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, MOVIMENTO);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPlanoPesq");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId('noPlanoContaPesq').value = "";
                        dojo.byId('cdPlanoContaPesq').value = 0;
                        dijit.byId("limparPlanoContaPesq").set('disabled', true);
                    }
                }, "limparPlanoContaPesq");

                if (hasValue(document.getElementById("limparPlanoContaPesq"))) {
                    document.getElementById("limparPlanoContaPesq").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPlanoContaPesq").parentNode.style.width = '40px';
                }


                

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("selecionaMovimentoFK"))) {
                                montarGridPesquisaMovtoFK(function () {
                                    limparFiltrosTurmaFK();
                                    
                                    dojo.byId('tipoPesquisaMovimentoFK').value = ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL;     
                                    dojo.byId('trPesquisaPerdaMaterial').style.display = '';
                                    dojo.byId('trPesquisaPerdaMaterialCk').style.display = '';
                                    dojo.byId('tipoRetornoMovimentoFK').value = ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL;
                                    dijit.byId('ckNotaFiscalPerdaMaterialFK').set("checked", false);
                                    try {
                                        setTimeout(function () {
                                                showCarregando();
                                                dijit.byId('naturezaNF').set("value", SAIDA);
                                                dijit.byId('naturezaNF').set("disabled", true);
                                                hideCarregando();
                                            },
                                            500);
                                    } catch (e) {
                                        hideCarregando();
                                    }
                                    

                                    pesquisarMovimentoFKPerdaMaterial(true, ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL, SAIDA);
                                    
                                    dijit.byId("proMvtoFK").show();
                                    
                                });
                            }
                            else {
                                limparFiltrosTurmaFK();
                                dojo.byId('tipoPesquisaMovimentoFK').value = ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL;
                                dojo.byId('tipoRetornoMovimentoFK').value = ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL;
                                dijit.byId('ckNotaFiscalPerdaMaterialFK').set("checked", false);
                                dijit.byId('naturezaNF').set("value", 2);
                                dijit.byId('naturezaNF').set("disabled", true);

                                dojo.byId('trPesquisaPerdaMaterial').style.display = '';
                                dojo.byId('trPesquisaPerdaMaterialCk').style.display = '';
                                pesquisarMovimentoFKPerdaMaterial(true, ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL, SAIDA);
                                dijit.byId("proMvtoFK").show();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesNFDevCad");

                new Button({
                    label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                        dojo.byId("cd_nfDevCad").value = 0;
                        dojo.byId("id_tipo_movimentoCad").value = 0;
                        dijit.byId("tpNfDevCad").set("value", "");
                        dijit.byId('limparNFDevCad').set("disabled", true);
                        dojo.byId("id_tipo_movimentoCad").value = 0;


                        dojo.byId("cd_alunoCad").value = null;
                        dijit.byId("nomeAlunoCad").set("value", "");

                        dojo.byId("cd_contratoCad").value = null;
                        dijit.byId("nomeIdContratoCad").set("value", "");

                        var grid = dijit.byId("gridItemsMovimentoMaterialDidatico");
                        var dataStore = new dojo.data.ObjectStore({
                            objectStore: new dojo.store.Memory({
                                data: [],
                                idProperty: "cd_item_movimento"
                            })
                        });
                        grid.setStore(dataStore);
                    }
                }, "limparNFDevCad");
                if (hasValue(document.getElementById("limparNFDevCad"))) {
                    document.getElementById("limparNFDevCad").parentNode.style.minWidth = '40px';
                    document.getElementById("limparNFDevCad").parentNode.style.width = '40px';
                }

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            dojo.byId('tipoRetornoItemFK').value = ORIGEM_ITEM_SEARCH_PERDA_MATERIAL;
                            if (!hasValue(dijit.byId("selecionaMovimentoFK"))) {
                                montarGridPesquisaMovtoFK(function () {
                                    limparFiltrosTurmaFK();

                                    dojo.byId('trPesquisaPerdaMaterial').style.display = 'none';
                                    dojo.byId('trPesquisaPerdaMaterialCk').style.display = '';
                                    dojo.byId('tipoPesquisaMovimentoFK').value = ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL;     
                                    dojo.byId('tipoRetornoMovimentoFK').value = ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL;
                                    dijit.byId('ckNotaFiscalPerdaMaterialFK').set("checked", false);
                                    try {
                                        setTimeout(function () {
                                                showCarregando();
                                                dijit.byId('naturezaNF').set("value", SAIDA);
                                                dijit.byId('naturezaNF').set("disabled", true);
                                                hideCarregando();
                                            },
                                            500);
                                    } catch (e) {
                                        hideCarregando();
                                    } 
                                    
                                    pesquisarMovimentoFKPerdaMaterial(true, ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL, SAIDA);
                                    dijit.byId("proMvtoFK").show();
                                    
                                    
                                });
                            }
                            else {
                                limparFiltrosTurmaFK();
                                dojo.byId('tipoPesquisaMovimentoFK').value = ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL;
                                dojo.byId('tipoRetornoMovimentoFK').value = ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL;
                                dojo.byId('trPesquisaPerdaMaterial').style.display = 'none';
                                dojo.byId('trPesquisaPerdaMaterialCk').style.display = '';
                                pesquisarMovimentoFKPerdaMaterial(true, ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL, SAIDA);
                                dijit.byId('ckNotaFiscalPerdaMaterialFK').set("checked", false);
                                dijit.byId('naturezaNF').set("value", 2);
                                dijit.byId('naturezaNF').set("disabled", true);
                                dijit.byId("proMvtoFK").show();
                                
                                
                                    

                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesNFDev");

                new Button({
                    label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                        dojo.byId("cd_nfDev").value = 0;
                        dojo.byId("id_tipo_movimento").value = 0;
                        dijit.byId("tpNfDev").set("value", "");
                        dijit.byId('limparNFDev').set("disabled", true);
                        dijit.byId("cbMovtoFK").reset();
                        dijit.byId("cbMovtoFK").set("disabled", false);
                        dojo.byId("id_tipo_movimento").value = 0;
                        if (hasValue(gridItem)) {
                            gridItem.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                            gridItem.itensSelecionados = [];
                        }
                        dojo.byId("cdPessoaMvtoCad").value = 0;
                        dijit.byId("noPessoaMovto").set("value", "");
                    }
                }, "limparNFDev");
                if (hasValue(document.getElementById("limparNFDev"))) {
                    document.getElementById("limparNFDev").parentNode.style.minWidth = '40px';
                    document.getElementById("limparNFDev").parentNode.style.width = '40px';
                }

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                montarGridPesquisaAluno(false, function () {
                                    abrirAlunoFK(ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL);
                                });
                            }
                            else
                                abrirAlunoFK(ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "FKAluno");
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

                var buttonFkArray = ['FKAluno', 'pesNFDev', 'pesProItemFK', 'pesNFDevCad'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () { incluirPerdaMaterial(); }
                }, "incluirPerdaMaterial");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { alterarPerdaMaterial(); }
                }, "alterarPerdaMaterial");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            if (!eval(MasterGeral()) && !eval(Master())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            deletarPerdaMaterial();
                    }); }
                }, "deletePerdaMaterial");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () { limparPerdaMaterial(); }
                }, "limparPerdaMaterial");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () { keepValues(null, gridPerdaMaterial, null) }
                }, "cancelarPerdaMaterial");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("cad").hide(); }
                }, "fecharPerdaMaterial");
                new Button({
                    label: "",
                    onClick: function () {

                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                         apresentaMensagem('apresentadorMensagem', null); pesquisarPerdaMaterial(true);
                    },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarPerdaMaterial");
                decreaseBtn(document.getElementById("pesquisarPerdaMaterial"), '32px');

                new Button({
                    label: "",
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarMatDid(true); },
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                }, "pesquisarMat");
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral()) && !eval(Master())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }

                            
                            
                            dijit.byId("material").set("open", true);
                            
                            setTimeout('limparPerdaMaterial(true)', 10);
                            
                            dijit.byId("cad").show();
                            
                            dijit.byId('tabContainer').resize();
                            gridPerdaMaterial.itemSelecionado = null;
                            apresentaMensagem('apresentadorMensagem', null);
                            //populaProximosPerdaMaterials(null);
                            IncluirAlterar(1, 'divAlterarPerdaMaterial', 'divIncluirPerdaMaterial', 'divExcluirPerdaMaterial', 'apresentadorMensagemPerdaMaterial', 'divCancelarPerdaMaterial', 'divLimparPerdaMaterial');
                            dijit.byId("gridItemsMovimentoMaterialDidatico").update();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoPerdaMaterial");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        if (!eval(MasterGeral()) && !eval(Master())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacaoAdmin);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        require(["dojo/_base/xhr"], function (xhr) {
                            var cd_aluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
                            var nm_contrato = hasValue(dijit.byId("_contrato").value) ? dijit.byId("_contrato").value : 0;
                            var cd_movimento = hasValue(dojo.byId("cd_nfDev").value) ? dojo.byId("cd_nfDev").value : 0;
                            var cd_item = hasValue(dojo.byId("cd_item_pesq").value) ? dojo.byId("cd_item_pesq").value : 0;
                            var dt_ini = hasValue(dojo.byId("dtaIni").value) ? dojo.byId("dtaIni").value : "";
                            var dt_fim = hasValue(dojo.byId("dtaFim").value) ? dojo.byId("dtaFim").value : "";
                            var status = hasValue(dijit.byId("_tipoStatus").value) ? dijit.byId("_tipoStatus").value : -1;

                            xhr.get({
                                url: Endereco() + "/api/coordenacao/getUrlRelatorioPerdaMaterial?" + getStrGridParameters('gridPerdaMaterial') + "cd_aluno=" + cd_aluno + "&nm_contrato=" + nm_contrato + "&cd_movimento=" + cd_movimento + 
                                "&cd_item=" + cd_item + "&dtInicial=" + dt_ini + "&dtFinal=" + dt_fim + "&status=" + status,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                });
                        })
                    }
                }, "relatorioPerdaMaterial");

                

                montarStatus("statusPerdaMaterial");
                
                adicionarAtalhoPesquisa([], 'pesquisarPerdaMaterial', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try {
        if (load)
            loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno);

        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK(origem) {
    try {
        
        dojo.byId('tipoRetornoAlunoFK').value = origem ;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        apresentaMensagem("apresentadorMensagem", null);

        limparPesquisaAlunoFK();

        pesquisarAlunoFK(true, ( origem ));
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }

}


//Aluno FK
function retornarAlunoFK(origem) {
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
            if (origem == ORIGEM_ALUNO_SEARCH_PERDA_MATERIAL) {
                dojo.byId("cdAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                dojo.byId("txAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                dijit.byId('limparAluno').set("disabled", false);
                dijit.byId("proAluno").hide();
            } else if (origem == ORIGEM_ALUNO_FK_MOVIMENTO) {
                dojo.byId("cdAlunoFKMovimento").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                dojo.byId("txAlunoFKMovimento").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                dijit.byId('limparAlunoFKMovimento').set("disabled", false);
                dijit.byId("proAluno").hide();
            }

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function atualizaGrid(gridName) {
    try {
        dijit.byId(gridName).update();
    }
    catch (e) {
        postGerarLog(e);
    }
}





function loadPerdaMaterials(items, link) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var itemsCb = [];
                Array.forEach(items, function (value, i) {
                    itemsCb.push({ id: value.cd_perda_material, name: value.no_curso });
                });
                var stateStore = new Memory({
                    data: itemsCb
                });
                dijit.byId(link).store = stateStore;
            }
            catch (e) {
                postGerarLog(e);
            }
        })
}

function limparPerdaMaterial(preencheData) {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                var gridMatDid = dijit.byId('gridItemsMovimentoMaterialDidatico');
                gridMatDid.setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                //dijit.byId("gridPerdaMaterialAval")._by_idx = [];
                apresentaMensagem('apresentadorMensagemPerdaMaterial', null);
                getLimpar('#formPerdaMaterial');
                clearForm('formPerdaMaterial');
                montarTipoStatusCad(Memory, dijit.byId("statusCad"), 1)
                if (preencheData != null && preencheData != undefined && preencheData == true) {
                    var anoInicial = new Date(Date.now()).getFullYear();
                    var dtInicial = new Date(anoInicial, mes, dia);
                    var strData = dojo.date.locale.format(dtInicial, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
                    dojo.byId("dt_perda_material").value = strData;
                }
                //IncluirAlterar(1, 'divAlterarPerdaMaterial', 'divIncluirPerdaMaterial', 'divExcluirPerdaMaterial', 'apresentadorMensagemPerdaMaterial', 'divCancelarPerdaMaterial', 'divLimparPerdaMaterial');
                gridMatDid.update();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function alterarPerdaMaterial() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemPerdaMaterial', null);
        if (!dijit.byId("formPerdaMaterial").validate())
            return false;

        if (!hasValue(dojo.byId("cd_perda_material").value) || (hasValue(dojo.byId("cd_perda_material").value) && dojo.byId("cd_perda_material").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Formulário inválido.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("cd_nfDevCad").value) || (hasValue(dojo.byId("cd_nfDevCad").value) && dojo.byId("cd_nfDevCad").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O campo Movimento é obrigatório.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("cd_contratoCad").value) || (hasValue(dojo.byId("cd_contratoCad").value) && dojo.byId("cd_contratoCad").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O campo Contrato é obrigatório.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("dt_perda_material")) || (hasValue(dojo.byId("dt_perda_material").value) && dojo.byId("dt_perda_material").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O campo Data é obrigatório.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }
        

        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/postEditPerdaMaterial",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_perda_material: hasValue(dojo.byId("cd_perda_material").value) ? dojo.byId("cd_perda_material").value : null, 
                    dt_perda_material: hasValue(dojo.byId("dt_perda_material").value) ? dojo.date.locale.parse(dojo.byId("dt_perda_material").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    cd_movimento: hasValue(dojo.byId("cd_nfDevCad").value) ? dojo.byId("cd_nfDevCad").value : null,
                    cd_contrato: hasValue(dojo.byId("cd_contratoCad").value) ? dojo.byId("cd_contratoCad").value : null,
                    id_status_perda: hasValue(dijit.byId("statusCad").value) ? dijit.byId("statusCad").value : null,
                })
            }).then(function (data) {
                try {
                    var itemAlterado = data.retorno;
                    var todos = dojo.byId("todosItensPerdaMaterial_label");
                    var gridName = 'gridPerdaMaterial';
                    var grid = dijit.byId(gridName);

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();
                    removeObjSort(grid.itensSelecionados, "cd_perda_material", dom.byId("cd_perda_material").value);
                    insertObjSort(grid.itensSelecionados, "cd_perda_material", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoPerdaMaterial', 'cd_perda_material', 'selecionaTodosPerdaMaterial', ['pesquisarPerdaMaterial', 'relatorioPerdaMaterial'], 'todosItensPerdaMaterial', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_perda_material");
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemPerdaMaterial', error);
                });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirPerdaMaterial() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemPerdaMaterial', null);
        if (!dijit.byId("formPerdaMaterial").validate())
            return false;

        if (!hasValue(dojo.byId("cd_nfDevCad").value) || (hasValue(dojo.byId("cd_nfDevCad").value) && dojo.byId("cd_nfDevCad").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O campo Movimento é obrigatório.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("cd_contratoCad").value) || (hasValue(dojo.byId("cd_contratoCad").value) && dojo.byId("cd_contratoCad").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O campo Contrato é obrigatório.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }

        if (!hasValue(dojo.byId("dt_perda_material")) || (hasValue(dojo.byId("dt_perda_material").value) && dojo.byId("dt_perda_material").value <= 0)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O campo Data é obrigatório.");
            apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
            return false;
        }

        

        

        
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/postPerdaMaterial",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_perda_material: 0,
                    dt_perda_material: hasValue(dojo.byId("dt_perda_material").value) ? dojo.date.locale.parse(dojo.byId("dt_perda_material").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    cd_movimento: hasValue(dojo.byId("cd_nfDevCad").value) ? dojo.byId("cd_nfDevCad").value : null,
                    cd_contrato: hasValue(dojo.byId("cd_contratoCad").value) ? dojo.byId("cd_contratoCad").value : null,
                    id_status_perda: TIPO_STATUS_PERDA_MATERIAL.ABERTO
                })
            }).then(function (data) {
                try {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridPerdaMaterial';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    insertObjSort(grid.itensSelecionados, "cd_perda_material", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoPerdaMaterial', 'cd_perda_material', 'selecionaTodosPerdaMaterial', ['pesquisarPerdaMaterial', 'relatorioPerdaMaterial'], 'todosItensPerdaMaterial', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_perda_material");
                    hideCarregando();
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            },
                function (error) {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemPerdaMaterial', error);
                });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarPerdaMaterial(limparItens) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cd_aluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
            var nm_contrato = hasValue(dijit.byId("_contrato").value) ? dijit.byId("_contrato").value : 0;
            var cd_movimento = hasValue(dojo.byId("cd_nfDev").value) ? dojo.byId("cd_nfDev").value : 0;
            var cd_item = hasValue(dojo.byId("cd_item_pesq").value) ? dojo.byId("cd_item_pesq").value : 0;
            var dt_ini = hasValue(dojo.byId("dtaIni").value) ? dojo.byId("dtaIni").value : "";
            var dt_fim = hasValue(dojo.byId("dtaFim").value) ? dojo.byId("dtaFim").value : "";
            var status = hasValue(dijit.byId("_tipoStatus").value) ? dijit.byId("_tipoStatus").value : -1;
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getPerdaMaterialSearch?cd_aluno=" + cd_aluno + "&nm_contrato=" + nm_contrato + "&cd_movimento=" + cd_movimento + 
                        "&cd_item=" + cd_item + "&dtInicial=" + dt_ini + "&dtFinal=" + dt_fim + "&status=" + status,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }
                ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPerdaMaterial");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarMatDid(limparItens) {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getItemPerdaMaterial?cdPerdaMaterial=" + dojo.byId("cd_perda_material").value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_item_curso"
        }).then(function (data) {
            try {
                //dataProd = jQuery.parseJSON(dataProd);
                var grid = dijit.byId("gridItemsMovimentoMaterialDidatico");
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "cd_item_curso" }) });
                grid.setStore(dataStore);
                if (limparItens)
                    grid.itensSelecionados = [];
                //else
                //    grid.itensSelecionados = dijit.byId("gridItemsMovimentoMaterialDidatico").store.objectStore.data;

                quickSortObj(grid.itensSelecionados, 'cd_item_curso');
                dijit.byId('material').set('open', true);
                dijit.byId('gridItemsMovimentoMaterialDidatico').update();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function deletarPerdaMaterial(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_perda_material').value != 0)
                    itensSelecionados = [{
                        cd_perda_material: hasValue(dojo.byId("cd_perda_material").value) ? dojo.byId("cd_perda_material").value : null,
                        dt_perda_material: hasValue(dojo.byId("dt_perda_material").value) ? dojo.date.locale.parse(dojo.byId("dt_perda_material").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        cd_movimento: hasValue(dojo.byId("cd_nfDevCad").value) ? dojo.byId("cd_nfDevCad").value : null,
                        cd_contrato: hasValue(dojo.byId("cd_contratoCad").value) ? dojo.byId("cd_contratoCad").value : null,
                        id_status_perda: hasValue(dijit.byId("statusCad").value) ? dijit.byId("statusCad").value : null,
                    }];

             if (itensSelecionados[0].id_status_perda == TIPO_STATUS_PERDA_MATERIAL.FECHADO) {
                 caixaDialogo(DIALOGO_ERRO, 'Perda de material com status fechado não pode ser excluido.', null);
                 return;
            }

            
            
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeletePerdaMaterial",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItensPerdaMaterial_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridPerdaMaterial').itensSelecionados, "cd_perda_material", itensSelecionados[r].cd_perda_material);

                    pesquisarPerdaMaterial(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarPerdaMaterial").set('disabled', false);
                    dijit.byId("relatorioPerdaMaterial").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
                function (error) {
                    if (!hasValue(dojo.byId("cad").style.display))
                        apresentaMensagem('apresentadorMensagemPerdaMaterial', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        }
        catch (e) {
            postGerarLog(e);
        }
    })
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else if (itensSelecionados[0].id_status_perda == TIPO_STATUS_PERDA_MATERIAL.FECHADO)
            caixaDialogo(DIALOGO_ERRO, 'Perda de material com status fechado não pode ser editado.', null);
        else {

            
            var gridPerdaMaterial = dijit.byId('gridPerdaMaterial');

            limparPerdaMaterial();
            apresentaMensagem('apresentadorMensagem', '');
            gridPerdaMaterial.itemSelecionado = itensSelecionados[0];
            keepValues(null, gridPerdaMaterial, true);
            dijit.byId("cad").show();
            IncluirAlterar(0, 'divAlterarPerdaMaterial', 'divIncluirPerdaMaterial', 'divExcluirPerdaMaterial', 'apresentadorMensagemPerdaMaterial', 'divCancelarPerdaMaterial', 'divLimparPerdaMaterial');
            dijit.byId('tabContainer').resize();
            dijit.byId("gridItemsMovimentoMaterialDidatico").update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function processarPerdaMaterial(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"],
        function(dom, domAttr, xhr, ref) {
            try {
                if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para processar.', null);
                else if (itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para processar.', null);
                else if (itensSelecionados[0].id_status_perda == TIPO_STATUS_PERDA_MATERIAL.FECHADO)
                    caixaDialogo(DIALOGO_ERRO, 'Perda de material com status fechado não pode ser editado.', null);
                else {

                    showCarregando();
                    xhr.post({
                        url: Endereco() + "/api/coordenacao/postProcessarPerdaMaterial",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        },
                        postData: ref.toJson(itensSelecionados[0])
                    }).then(function(data) {
                        try {
                            hideCarregando();
                                var todos = dojo.byId("todosItensPerdaMaterial_label");

                                apresentaMensagem('apresentadorMensagem', data);

                                pesquisarPerdaMaterial(false);

                                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                                dijit.byId("pesquisarPerdaMaterial").set('disabled', false);
                                dijit.byId("relatorioPerdaMaterial").set('disabled', false);

                                if (hasValue(todos))
                                    todos.innerHTML = "Todos Itens";
                        } catch (er) {
                            hideCarregando();
                            postGerarLog(er);
                        }
                    },
                    function (error) {
                            hideCarregando();
                        apresentaMensagem('apresentadorMensagem', error);
                    });

                }
            } catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        });
}


/*function pesquisarMovimentoFKPerdaMaterial(limparItens, origem) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/domReady!",
        "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var anoInicial = new Date(Date.now()).getFullYear();
            var dtInicial = new Date(anoInicial, 00, 01);
            var strData = dojo.date.locale.format(dtInicial, { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
            var grid = dijit.byId("gridPesquisaMovtoFK");
            //var idNatMvot = (dojo.byId('id_natureza_movto') != null && dojo.byId('id_natureza_movto') != undefined && hasValue(dojo.byId('id_natureza_movto').value)) ? dojo.byId('id_natureza_movto').value : 0;
            var idNatMvot = (dijit.byId('naturezaNF').value != null && dijit.byId('naturezaNF') != undefined && hasValue(dijit.byId('naturezaNF').value)) ? dijit.byId('naturezaNF').value : 0;
            var cdPessoaPesq = hasValue(dojo.byId("cdPessoaPesqFK").value) ? dojo.byId("cdPessoaPesqFK").value : 0;
            var cdItem = hasValue(dojo.byId("cdItemFK").value) ? dojo.byId("cdItemFK").value : 0;
            var cdPlanoContaPesq = hasValue(dojo.byId("cdPlanoContaPesqFK").value) ? dojo.byId("cdPlanoContaPesqFK").value : 0;
            var numeroPesq = hasValue(dojo.byId("numeroPesqFK").value) ? dojo.byId("numeroPesqFK").value : 0;
            var serie = hasValue(dojo.byId("numeroSeriePesqFK").value) ? dojo.byId("numeroSeriePesqFK").value : "";
            if (!dijit.byId('ckEmissaoFK').checked && !dijit.byId('ckMovimentoFK').checked)
                dijit.byId('ckMovimentoFK').set("checked", true);
            if (hasValue(dijit.byId('ckNotaFiscal'))) {
                idNf = dijit.byId('ckNotaFiscal').checked;
            } else {
                idNf = false;
            }
            isMovimento = false; //Alterado a regra pois já estão sendo passaos alguns filtros iniciais
            if (!hasValue(dojo.byId("dtaInicialFK").value))
                dojo.byId("dtaInicialFK").value = strData;

            

            var myStore =
                Cache(
                    JsonRest({
                        target: Endereco() + "/api/fiscal/getMovimentoSearchFKPerdaMaterial?cd_pessoa=" + parseInt(cdPessoaPesq) +
                            "&cd_item=" + parseInt(cdItem) + "&cd_plano_conta=" + parseInt(cdPlanoContaPesq) + "&numero=" + parseInt(numeroPesq) + "&serie=" + serie +
                            "&emissao=" + document.getElementById("ckEmissaoFK").checked + "&movimento=" + document.getElementById("ckMovimentoFK").checked +
                            "&dtInicial=" + dojo.byId("dtaInicialFK").value + "&dtFinal=" + dojo.byId("dtaFinalFK").value + "&natMovto=" + idNatMvot +
                            "&idNf=" + idNf + "&origem=" + origem + strAlunoContrato,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}*/




function retornaFKMovto(origem) {
    try {
        var valido = true;
        var gridMovtoFK = dijit.byId("gridPesquisaMovtoFK");
        if (!hasValue(gridMovtoFK.itensSelecionados))
            gridMovtoFK.itensSelecionados = [];
        if (!hasValue(gridMovtoFK.itensSelecionados) || gridMovtoFK.itensSelecionados.length <= 0 || gridMovtoFK.itensSelecionados.length > 1) {
            if (gridMovtoFK.itensSelecionados != null && gridMovtoFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridMovtoFK.itensSelecionados != null && gridMovtoFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            if (origem == ORIGEM_MOVIMENTO_SEARCH_PERDA_MATERIAL) {
                dojo.byId("cd_nfDev").value = gridMovtoFK.itensSelecionados[0].cd_movimento;
                dijit.byId("tpNfDev").set("value", gridMovtoFK.itensSelecionados[0].dc_numero_serie == "" ? "S/N" : gridMovtoFK.itensSelecionados[0].dc_numero_serie);
                $("#id_tipo_movimento").val(gridMovtoFK.itensSelecionados[0].id_tipo_movimento);
                dijit.byId("limparNFDev").set("disabled", false);
            } else if (origem == ORIGEM_MOVIMENTO_CAD_PERDA_MATERIAL) {
                dojo.byId("cd_nfDevCad").value = gridMovtoFK.itensSelecionados[0].cd_movimento;
                dijit.byId("tpNfDevCad").set("value", gridMovtoFK.itensSelecionados[0].dc_numero_serie == "" ? "S/N" : gridMovtoFK.itensSelecionados[0].dc_numero_serie);
                $("#id_tipo_movimentoCad").val(gridMovtoFK.itensSelecionados[0].id_tipo_movimento);
                dijit.byId("limparNFDevCad").set("disabled", false);

                if (gridMovtoFK.itensSelecionados[0].cd_aluno != null &&
                    gridMovtoFK.itensSelecionados[0].cd_aluno > 0 &&
                    gridMovtoFK.itensSelecionados[0].no_aluno != null &&
                    gridMovtoFK.itensSelecionados[0].no_aluno.trim().length > 0) {
                    dojo.byId("cd_alunoCad").value = gridMovtoFK.itensSelecionados[0].cd_aluno;
                    dijit.byId("nomeAlunoCad").set("value", gridMovtoFK.itensSelecionados[0].no_aluno);


                } else {
                    dojo.byId("cd_alunoCad").value = null;
                    dijit.byId("nomeAlunoCad").set("value", "");

                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Aluno não encotrado para o movimento: " + (gridMovtoFK.itensSelecionados[0].dc_numero_serie == "" ? "S/N" : gridMovtoFK.itensSelecionados[0].dc_numero_serie));
                    apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
                }

                if (gridMovtoFK.itensSelecionados[0].cd_contrato != null && gridMovtoFK.itensSelecionados[0].cd_contrato > 0 &&
                    gridMovtoFK.itensSelecionados[0].nm_contrato != null &&
                    gridMovtoFK.itensSelecionados[0].nm_contrato > 0) {
                    dojo.byId("cd_contratoCad").value = gridMovtoFK.itensSelecionados[0].cd_contrato;
                    dijit.byId("nomeIdContratoCad").set("value", ("Número:" + gridMovtoFK.itensSelecionados[0].nm_contrato + "/ Id: " + gridMovtoFK.itensSelecionados[0].cd_contrato));


                }
                else {
                    dojo.byId("cd_contratoCad").value = null;
                    dijit.byId("nomeIdContratoCad").set("value", "");

                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Contrato não encotrado para o movimento: " + (gridMovtoFK.itensSelecionados[0].dc_numero_serie == "" ? "S/N" : gridMovtoFK.itensSelecionados[0].dc_numero_serie));
                    apresentaMensagem("apresentadorMensagemPerdaMaterial", mensagensWeb);
                }
                try {

                    require(["dojo/_base/xhr"],
                        function (xhr) {
                            showCarregando();
                            xhr.get({
                                url:
                                    Endereco() +
                                        "/api/fiscal/getItensMovimentoByCdMovimentoPerdaMaterial?cd_movimento=" + gridMovtoFK.itensSelecionados[0].cd_movimento,
                                preventCache: true,
                                handleAs: "json",
                                headers: {
                                    "Accept": "application/json",
                                    "Content-Type": "application/json",
                                    "Authorization": Token()
                                }
                            }).then(function (data) {
                                hideCarregando();
                                if (hasValue(data)) {
                                    var grid = dijit.byId("gridItemsMovimentoMaterialDidatico");
                                    var dataStore = new dojo.data.ObjectStore({
                                        objectStore: new dojo.store.Memory({
                                            data: data,
                                            idProperty: "cd_item_movimento"
                                        })
                                    });
                                    grid.setStore(dataStore);
                                } else {
                                    var grid = dijit.byId("gridItemsMovimentoMaterialDidatico");
                                    var dataStore = new dojo.data.ObjectStore({
                                        objectStore: new dojo.store.Memory({
                                            data: [],
                                            idProperty: "cd_item_movimento"
                                        })
                                    });
                                    grid.setStore(dataStore);
                                }

                            },
                            function (error) {
                                hideCarregando();
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        });
                } catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                } 
                
            }
            

        }
        if (!valido)
            return false;
        dijit.byId("proMvtoFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaCadFK() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready",
        "dojo/domReady!",
        "dojo/parser"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function () {
                try {
                    if (TIPOMOVIMENTO == SAIDA ) {
                        var myStore = null;
                            var myStore = dojo.store.Cache(
                                dojo.store.JsonRest({
                                    target: Endereco() + "/api/Pessoa/GetPessoaResponsavelSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                        "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                        "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                        "&cnpjCpf=" + dojo.byId("CnpjCpf").value + "&cdPai=0" +
                                        "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=0",
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Authorization": Token() }
                                }), dojo.store.Memory({}));
                            dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
                        var grid = dijit.byId("gridPesquisaPessoa");
                        grid.noDataMessage = msgNotRegEnc;
                        grid.setStore(dataStore);
                    } 
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        });
}

function abrirPessoaFK(isPesquisa) {
    try {
        limparPesquisaPessoaFK();
        if (isPesquisa) pesquisaPessoaFKMovimento(isPesquisa);
        else {
            //Não mais necessário pois a consulta inicial não vai ser executada.
            //dojo.byId("_nomePessoaFK").value = 'a';
            //dijit.byId("inicioPessoaFK").set('checked', true);
            pesquisaPessoaCadFK();
        }
        dijit.byId("fkPessoaPesq").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            /*if (!dijit.byId("cadMovimento").open) {
                $("#cdPessoaPesq").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                dijit.byId("noPessoaPesq").set("value", gridPessoaSelec.itensSelecionados[0].no_pessoa);
                dijit.byId("limparPessoaRelPosPes").set("disabled", false);

            } else { */
                //if (hasValue(origem) && origem == MOVIMENTO_DEVOLUCAO) {
                    $("#cdPessoaPesqFK").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                    dijit.byId("noPessoaPesqFK").set("value", gridPessoaSelec.itensSelecionados[0].no_pessoa);
                    dijit.byId("limparPessoaRelPosPesFK").set("disabled", false);
                //}
                /*else {
                    $("#cdPessoaMvtoCad").val(gridPessoaSelec.itensSelecionados[0].cd_pessoa);
                    dijit.byId("noPessoaMovto").set("value", gridPessoaSelec.itensSelecionados[0].no_pessoa);
                    dijit.byId('limparPessoaFKMovimento').set("disabled", false);
                    if ((TIPOMOVIMENTO == SAIDA || TIPOMOVIMENTO == SERVICO) && hasValue(gridPessoaSelec.itensSelecionados[0].existeAluno) && gridPessoaSelec.itensSelecionados[0].existeAluno) {
                        dojo.byId("cdAlunoFKMovimento").value = gridPessoaSelec.itensSelecionados[0].cd_aluno;
                        dojo.byId("cdPessoaAlunoFKMovimento").value = gridPessoaSelec.itensSelecionados[0].cd_pessoa;
                        dojo.byId("noAlunoFKMovimento").value = gridPessoaSelec.itensSelecionados[0].no_pessoa;
                        dijit.byId('limparAlunoFKMovimento').set("disabled", false);

                        var parametros = getParamterosURL();
                        if (hasValue(parametros['id_material_didatico']) &&
                            eval(parametros['id_material_didatico']) == 1) {

                            getContratosSemTurmaByAluno();

                        }
                    }
                    gerar_titulo = true;
                    if (dijit.byId("ckNotaFiscal").checked)
                        verificaOperacaoCFOP();
                }
            }*/
        }

        if (!valido)
            return false;
        dijit.byId("fkPessoaPesq").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consultarPessoaMovimento() {
    try {
        TIPOMOVIMENTO = SAIDA;
            apresentaMensagem("apresentadorMensagemProPessoa", null);
            pesquisaPessoaFKMovimento(true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKMovimento(isPesquisa) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory",
            "dojo/ready"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function () {
                try {
                    var myStore = null;
                    if (isPesquisa) {
                        var myStore = Cache(
                            JsonRest({
                                target: Endereco() + "/api/aluno/getPessoaMovimentoSearch?nome=" + encodeURIComponent(dojo.byId("_nomePessoaFK").value) +
                                    "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                    "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                    "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                    "&sexo=" + dijit.byId("sexoPessoaFK").value + "&tipoMovimento=" + TIPOMOVIMENTO,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({}));
                    }
                    
                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoa");
                    grid.noDataMessage = msgNotRegEnc;
                    grid.setStore(dataStore);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        });
}

function chamarPesquisaItemFK(tipoPesquisa, xhr, Memory, FilteringSelect, array, ready, kit) {
    try {
        ORIGEM_ITEM_FK = ORIGEM_ITEM_FK_MOVIMENTO;
        CONSULTA_ITEM = false;
        if (!kit)
            convertDialogItemKit("Pesquisar Item", true, false, false);

        if (tipoPesquisa == PESQUISA)
            abrirItemFK(xhr, Memory, FilteringSelect, array, kit);
        else
            abrirItemFKCadastro(xhr, ready, Memory, array, SETAR_TIPO);
    }
    catch (e) {
        postGerarLog(e);
    }
}


function convertDialogItemKit(title, disabledKit, checkedKit, disabledEstoque) {
    dijit.byId("fkItem").set('title', title);
    dijit.byId("comEstoque").set("disabled", disabledEstoque);
    dijit.byId("kit").set("checked", checkedKit);
    dijit.byId("kit").set("disabled", disabledKit);
}

function abrirItemFK(xhr, Memory, FilteringSelect, array, kit) {
    try {
        populaTipoItem("tipo", xhr, Memory, array, TIPOMOVIMENTO, null, kit);
        IS_MATERIAL_DIDATICO = true;
        pesquisarItemEstoqueFK(ORIGEM_ITEM_FK);
        if (TIPOMOVIMENTO != DESPESAS && TIPOMOVIMENTO != SERVICO) {
            showP('comEstoqueTitulo', true);
            showP('comEstoqueCampo', true);
        }
        dijit.byId("tipo").set("disabled", true);
        dijit.byId("fkItem").show();
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItemEstoqueFK(origem) {
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
            if (TIPOMOVIMENTO == DEVOLUCAO || TIPOMOVIMENTO == SERVICO)
                id_natureza = dojo.byId("id_natureza_movto").value;
            //if (CONSULTA_ITEM) {
            myStore = Cache(
                JsonRest({
                    target: Endereco() + "/fiscal/getItemMovimentoSearchPerdaMaterial?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" +
                        retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItemInt + "&grupoItem=" + grupoItem + "&id_tipo_movto=" + TIPOMOVIMENTO +
                        "&comEstoque=" + document.getElementById("comEstoque").checked + "&id_natureza_TPNF=" + parseInt(id_natureza) + "&kit=" + document.getElementById("kit").checked + "&origem=" + origem,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_item"
                }
                ), Memory({ idProperty: "cd_item" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            //}
            //else
            //    dataStore = dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) })
            //CONSULTA_ITEM = true;
            var grid = dijit.byId("gridPesquisaItem");
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function retornarItemFK() {
    try {
        var parametrosTela = getParamterosURL();

        var gridPesquisaItem = dijit.byId("gridPesquisaItem");
        if (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (gridPesquisaItem.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
        else {
            if (ORIGEM_ITEM_FK == ORIGEM_ITEM_FK_MOVIMENTO) {
                dojo.byId("cdItemFK").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                dojo.byId("noItemPesqFK").value = gridPesquisaItem.itensSelecionados[0].no_item;
                dijit.byId("limparItemFK").set("disabled", false);
            } else if (ORIGEM_ITEM_FK == ORIGEM_ITEM_SEARCH_PERDA_MATERIAL) {
           
                dojo.byId("cd_item_pesq").value = gridPesquisaItem.itensSelecionados[0].cd_item;
                dojo.byId("item_pesq").value = gridPesquisaItem.itensSelecionados[0].no_item;
                dijit.byId("limparItemFiltroFK").set("disabled", false);
            
            }
            
        }
        gridPesquisaItem.itensSelecionados = [];
        ORIGEM_ITEM_FK = 0;
        dijit.byId("fkItem").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirFKAlunoMovimentoFK()
{
    try {
        
        
        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
            montarGridPesquisaAluno(false, function () {
                abrirAlunoFK(ORIGEM_ALUNO_FK_MOVIMENTO);
            });
        }
        else
            abrirAlunoFK(ORIGEM_ALUNO_FK_MOVIMENTO);
    } catch (e) {
        postGerarLog(e);
    } 
}
