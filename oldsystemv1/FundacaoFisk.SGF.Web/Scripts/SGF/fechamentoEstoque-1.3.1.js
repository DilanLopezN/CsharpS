var saldosItens = new Array();
var CUSTOSDESPESAS = 6;
var MATERIAL_DIDATICO = 1;
var GRUPO_MATERIAL = 0;

function formatCheckBoxFechamento(value, rowIndex, obj) {
    try{
        var gridName = 'gridFechamento';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFechamento');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_fechamento", grid._by_idx[rowIndex].item.cd_fechamento);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_fechamento', 'selecionadoFechamento', -1, 'selecionaTodosFechamento', 'selecionaTodosFechamento', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_fechamento', 'selecionadoFechamento', " + rowIndex + ", '" + id + "', 'selecionaTodosFechamento', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatTextContagem(value, rowIndex, obj) {
    try{
        var gridItemFechamento = dijit.byId("gridItemFechamento");
        var icon;
        var desc = obj.field + '_input_' + gridItemFechamento._by_idx[rowIndex].item.cd_item;

        if (dijit.byId(desc) != null)
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height: 18px' /> ";

        setTimeout("configuraTextBoxContagem('" + value + "', '" + desc + "','" + gridItemFechamento._by_idx[rowIndex].item.cd_item + "','" + rowIndex  + "', " + gridItemFechamento._by_idx[rowIndex].item.id_movto_estoque + ")", 1);
        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatTextCustoCorrigido(value, rowIndex, obj) {
    try{
        var gridItemFechamento = dijit.byId("gridItemFechamento");
        var icon;
        var desc = obj.field + '_input_' + gridItemFechamento._by_idx[rowIndex].item.cd_item;
        //ar id = obj.field + '_Selected_' + rowIndex;
        if (dijit.byId(desc) != null)
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height: 18px' /> ";

        setTimeout("configuraTextBoxCustoCorrigido('" + value + "', '" + desc + "','" + gridItemFechamento._by_idx[rowIndex].item.cd_item + "'," + rowIndex + ")", 1);
        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatTextVendaCorrigido(value, rowIndex, obj) {
    try{
        var gridItemFechamento = dijit.byId("gridItemFechamento");
        var icon;
        var desc = obj.field + '_input_' + gridItemFechamento._by_idx[rowIndex].item.cd_item;

        if (dijit.byId(desc) != null)
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height: 18px' /> ";

        setTimeout("configuraTextBoxVendaCorrigido('" + value + "', '" + desc + "','" + gridItemFechamento._by_idx[rowIndex].item.cd_item + "','" + rowIndex + "', '" + gridItemFechamento._by_idx[rowIndex].item.cd_tipo_item + "')", 1);
        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxContagem(value, desc, cd_item, rowIndex, mvtoEstoque) {
    try{
        if (value == undefined || isNaN(parseInt(value))) value = 0;
        //else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            require(["dijit/form/NumberTextBox"], function (TextBox) {
                var newTextBox = new dijit.form.NumberTextBox({
                    name: "textBox" + desc,
                    value: value,
                    maxlength: 9,
                    style: "width: 100%;",
                    onBlur: function (b) {
                        $('#' + desc).focus();
                    },
                    onChange: function (b) {
                        atualizarValoresContagem(desc, this, rowIndex, cd_item);
                    },
                    smallDelta: 1
                }, desc);
                if (hasValue(dijit.byId(desc))) {
                    dijit.byId(desc).old_value = value;
                    if (!mvtoEstoque)
                        dijit.byId(desc).set("disabled", true);
                    else
                        dijit.byId(desc).set("disabled", false);
                }
            });
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraIntNegativo(document.getElementById(desc))
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxCustoCorrigido(value, desc, cd_item, rowIndex) {
    try{
        if (value == undefined || isNaN(parseInt(value))) value = 0;
        //else value = value.toString().replace('.', ',');
        if (!hasValue(dijit.byId(desc))) {
            require(["dijit/form/NumberTextBox"], function (TextBox) {
                var newTextBox = new dijit.form.NumberTextBox({
                    name: "textBox" + desc,
                    value: maskFixed(value, 2),
                    maxlength: 9,
                    style: "width: 100%;",
                    constraints: { min: 0, pattern: '##.00#' },
                    onBlur: function (b) {
                        $('#' + desc).focus();
                    },
                    onChange: function (b) {
                        atualizarCustoCorrigido(desc, this, rowIndex, cd_item);
                    },
                    onKeyPress: function (b) {
                        mascaraFloat(document.getElementById(desc));
                    },
                    smallDelta: 1
                }, desc);
                if (hasValue(dijit.byId(desc)))
                    dijit.byId(desc).old_value = value;
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxVendaCorrigido(value, desc, cd_item, rowIndex, tipo_item) {
    try{
        if (value == undefined || isNaN(parseInt(value))) value = 0;
        //    else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            require(["dijit/form/NumberTextBox"], function (TextBox) {
                var newTextBox = new dijit.form.NumberTextBox({
                    name: "textBox" + desc,
                    value:  maskFixed(value, 2),
                    maxlength: 9,
                    style: "width: 100%;",
                    constraints: { min: 0, pattern: '##.00#' },
                    onBlur: function (b) {
                        $('#' + desc).focus();
                    },
                    onChange: function (b) {
                        atualizarVendaCorrigido(desc, this, rowIndex, cd_item);
                    },
                    onKeyPress: function (b) {
                        mascaraFloat(document.getElementById(desc));
                    },
                    smallDelta: 1
                }, desc);
                if (hasValue(dijit.byId(desc))) {
                    dijit.byId(desc).old_value = value;
                    if (tipo_item == CUSTOSDESPESAS)
                        dijit.byId(desc).set("disabled", true);
                    else
                        dijit.byId(desc).set("disabled", false);
                }
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

//data-dojo-props="smallDelta:1, constraints:{min:0, pattern:'##.00#'}"
//onkeypress="mascaraFloat(document.getElementById('valorDesconto'));" />

function atualizarValoresContagem(desc, obj, rowIndex, cd_item) {
    try{
        if (dijit.byId(desc).value != dijit.byId(desc).old_value) {
            apresentaMensagem("apresentadorMensagemItemFK", '');
            apresentaMensagem("apresentadorMensagemFechamento", '', true);
            var gridItemFechamento = dijit.byId("gridItemFechamento");
            var valido = true;

            for (var i = 0; i < gridItemFechamento.store.objectStore.data.length; i++) {
                var cdItem = gridItemFechamento.store.objectStore.data[i].cd_item;
                if (cd_item == cdItem) {
                    if (!eval(MasterGeral()) && (gridItemFechamento.store.objectStore.data[i].id_material_didatico || gridItemFechamento.store.objectStore.data[i].id_voucher_carga)) {
                        gridItemFechamento.store.objectStore.data[i].editado = false;
                        dijit.byId(desc).set('value', gridItemFechamento.store.objectStore.data[i].qt_saldo_fechamento);
                        valido = false;
                    }
                    else {
                        gridItemFechamento.store.objectStore.data[i].qt_saldo_fechamento = dijit.byId(desc).get('value');
                        gridItemFechamento.store.objectStore.data[i].editado = true;
                        valido = true;
                    }
                    break;
                }
            }

            for (var i = 0; i < saldosItens.length; i++) {
                var cdItemSaldo = saldosItens[i].cd_item;
                if (cd_item == cdItemSaldo) {
                    if (!eval(MasterGeral()) && (saldosItens[i].id_material_didatico || saldosItens[i].id_voucher_carga)) {
                        saldosItens[i].editado = false;
                    }
                    else {
                        saldosItens[i].cd_fechamento = dojo.byId('cd_fechamento').value;
                        saldosItens[i].qt_saldo_fechamento = dijit.byId(desc).get('value');
                        saldosItens[i].editado = true;
                    }
                    break;
                }
            }
            if (!valido) {
                caixaDialogo(DIALOGO_AVISO, 'Itens Material didatico ou voucher não podem ser alterados.', null);
            }
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function atualizarCustoCorrigido(desc, obj, rowIndex, cd_item) {
    try{
        if (dijit.byId(desc).value != dijit.byId(desc).old_value) {
            apresentaMensagem("apresentadorMensagemItemFK", '');
            apresentaMensagem("apresentadorMensagemFechamento", '', true);
            var gridItemFechamento = dijit.byId("gridItemFechamento");

            for (var i = 0; i < gridItemFechamento.store.objectStore.data.length; i++) {
                var cdItem = gridItemFechamento.store.objectStore.data[i].cd_item;
                if (cd_item == cdItem) {
                    gridItemFechamento.store.objectStore.data[i].vl_custo_fechamento = dijit.byId(desc).get('value');
                    gridItemFechamento.store.objectStore.data[i].editado = true;
                    break;
                }
            }

            for (var i = 0; i < saldosItens.length; i++) {
                var cdItemSaldo = saldosItens[i].cd_item;
                if (cd_item == cdItemSaldo) {
                    saldosItens[i].cd_fechamento = dojo.byId('cd_fechamento').value;
                    saldosItens[i].vl_custo_fechamento = dijit.byId(desc).get('value');
                    saldosItens[i].editado = true;
                    break;
                }
            }
            //saldosItens = gridItemFechamento.store.objectStore.data;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function atualizarVendaCorrigido(desc, obj, rowIndex, cd_item) {
    try{
        if (dijit.byId(desc).value != dijit.byId(desc).old_value) {
            apresentaMensagem("apresentadorMensagemItemFK", '');
            apresentaMensagem("apresentadorMensagemFechamento", '', true);
            var gridItemFechamento = dijit.byId("gridItemFechamento");

            for (var i = 0; i < gridItemFechamento.store.objectStore.data.length; i++) {
                var cdItem = gridItemFechamento.store.objectStore.data[i].cd_item;
                if (cd_item == cdItem) {
                    gridItemFechamento.store.objectStore.data[i].vl_venda_fechamento = dijit.byId(desc).get('value');
                    gridItemFechamento.store.objectStore.data[i].editado = true;
                    break;
                }
            }

            for (var i = 0; i < saldosItens.length; i++) {
                var cdItemSaldo = saldosItens[i].cd_item;
                if (cd_item == cdItemSaldo) {
                    saldosItens[i].cd_fechamento = dojo.byId('cd_fechamento').value;
                    saldosItens[i].vl_venda_fechamento = dijit.byId(desc).get('value');
                    saldosItens[i].editado = true;
                    break;
                }
            }
            //saldosItens = gridItemFechamento.store.objectStore.data;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesFechamento(grid) {
    try{
        var value = grid.itemSelecionado;

        require(["dojo/_base/xhr", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory"], function (xhr, ObjectStore, Cache, Memory) {
            limparFechamento(ObjectStore, Memory);
            xhr.get({
                preventCache: true,
                url: Endereco() + "/api/financeiro/getFechamentoById?cd_fechamento=" + value.cd_fechamento,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                value = jQuery.parseJSON(eval(data)).retorno;
                dojo.byId('cd_fechamento').value = value.cd_fechamento;
                dijit.byId('dtaFec')._onChangeActive = false;
                //dijit.byId('nm_ano')._onChangeActive = false;
                //dijit.byId('nm_mes')._onChangeActive = false;
                //dijit.byId('dtaFec').set('value', value.dt_fechamento);
                dijit.byId("dtaFec").value = value.dtf_fechamento != null ? dojo.byId("dtaFec").value = value.dtf_fechamento : "";
                //dijit.byId('nm_ano').set('value', value.nm_ano_fechamento);
                //dijit.byId('nm_mes').set('value', value.nm_mes_fechamento);
                //dijit.byId('nm_ano')._onChangeActive = true;
                //dijit.byId('nm_mes')._onChangeActive = true;
                dijit.byId('dtaFec')._onChangeActive = true;
                dijit.byId('id_balanco').set("checked", value.id_balanco);
                dojo.byId('dtaCadastroFech').value = value.dta_fechamento;
                dojo.byId('hrCadastroFech').value = value.hr_fechamento;
                dojo.byId('cd_usuario').value = value.cd_usuario;
                dojo.byId('atendenteFech').value = value.no_usuario;
                dojo.byId('descObsFech').value = value.tx_obs_fechamento;
                dijit.byId("itensFechamento").set("disabled", false);

                var grid = dijit.byId("gridItemFechamento");
                if (hasValue(value.SaldosItens) && value.SaldosItens.length > 0) {
                    var dataStoreContrato = new ObjectStore({ objectStore: new Memory({ data: value.SaldosItens }) });
                    grid.setStore(dataStoreContrato);
                    saldosItens = value.SaldosItens;

                }
                if (value.SaldosItens.length > 0) {
                    dijit.byId("dtaFec").set("disabled", true);
                    //dijit.byId("nm_ano").set("disabled", true);
                    //dijit.byId("nm_mes").set("disabled", true);
                    dijit.byId("pesItem").set("disabled", false);
                    dijit.byId("btndelItem").set("disabled", false);
                    dijit.byId("btnAlterarValor").set("disabled", false);
                }
                else {
                    dijit.byId("dtaFec").set("disabled", false);
                    //dijit.byId("nm_ano").set("disabled", false);
                    //dijit.byId("nm_mes").set("disabled", false);
                    dijit.byId("pesItem").set("disabled", true);
                    dijit.byId("btndelItem").set("disabled", true);
                    dijit.byId("btnAlterarValor").set("disabled", true);
                }


            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function montarFechamento() {
    try{
        var dadosRetorno = {
            cd_fechamento: dojo.byId('cd_fechamento').value,
            //nm_ano_fechamento: parseInt(dojo.byId('nm_ano').value.replace('.', '')),
            //nm_mes_fechamento: dojo.byId('nm_mes').value,
            dt_fechamento: hasValue(dojo.byId("dtaFec").value) ? dojo.date.locale.parse(dojo.byId("dtaFec").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            id_balanco: dijit.byId('id_balanco').checked,
            tx_obs_fechamento: dojo.byId('descObsFech').value,
            SaldosItens: saldosItens
        }
        return dadosRetorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarMetodosFechamentoEstoque() {
    //Criação da Grade de sala
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
    "dojox/grid/enhanced/plugins/NestedSorting",
    "dojo/ready",
    "dijit/form/DateTextBox",
    "dojo/on",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, NestedSorting, ready, DateTextBox, on, DropDownButton, DropDownMenu, MenuItem) {
        try {
            ready(function () {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/financeiro/getFechamentoSearch?ano=&mes=&balanco=false&dta_ini=&dta_fim=",
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                     }
                 ), Memory({}));

                var displayNone = false;
                var gridFechamento = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodosFechamento' style='display:none'/>", field: "selecionadoFechamento", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFechamento },
                        { name: "Data", field: "dtf_fechamento", width: "10%", styles: "text-align: center;min-width:80px;", },
                        //{ name: "Ano", field: "nm_ano_fechamento", width: "10%", styles: "min-width:80px;", },
                        //{ name: "Mês", field: "nm_mes_fechamento", width: "10%", styles: "min-width:80px;" },
                        { name: "Balanço", field: "balanco", width: "5%", styles: "text-align: center;min-width:40px;" },
                        { name: "Data Hora", field: "dtaHr_fechamento", width: "10%", styles: "min-width:80px;" },
                        { name: "Usuário", field: "no_usuario", width: "25%", styles: "min-width:40px; max-width: 50px;" }
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
                            position: "button"
                        }
                    }
                }, "gridFechamento");
                gridFechamento.canSort = function (col) { return Math.abs(col) != 1 };
                gridFechamento.startup();
                gridFechamento.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        gridFechamento.itemSelecionado = item;
                        limparFechamento(ObjectStore, Memory);
                        keepValuesFechamento(gridFechamento);
                        apresentaMensagem('apresentadorMensagem', '');
                        setarTabCad();
                        dijit.byId("cadFechamento").show();
                        IncluirAlterar(0, 'divAlterarFechamento', 'divIncluirFechamento', 'divExcluirFechamento', 'apresentadorMensagemFechamento', 'divCancelarFechamento', 'divLimparFechamento');
                        dijit.byId('tabContainerFechamento').resize(true);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridFechamento, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosFechamento').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_fechamento', 'selecionadoFechamento', -1, 'selecionaTodosFechamento', 'selecionaTodosFechamento', 'gridFechamento')", gridFechamento.rowsPerPage * 3);
                    });
                });
                if (!hasValue(gridFechamento.itensSelecionados))
                    gridFechamento.itensSelecionados = [];
                // var data = new Array();
                var gridItemFechamento = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "Item", field: "no_item", width: "22%", styles: "min-width:80px;" },
                        { name: "S.Atual", field: "qt_saldo_atual", width: "8%", styles: "text-align: right;min-width:80px;" },
                        { name: "Saldo", field: "qt_saldo_data", width: "8%", styles: "text-align: right;min-width:80px;" },
                        { name: "Contagem", field: "qt_saldo_fechamento", width: "11%", styles: "text-align: right;min-width:40px;", formatter: formatTextContagem },
                        { name: "V.Atual", field: "vlVendaAtual", width: "11%", styles: "text-align: right;min-width:80px;" },
                        { name: "Venda Corr.", field: "vl_venda_fechamento", width: "11%", styles: "text-align: right;min-width:40px;"}, //formatter: formatTextVendaCorrigido },
                        { name: "C.Atual", field: "vlCustoAtual", width: "10%", styles: "text-align: right;min-width:80px;" },
                        { name: "Custo Corr.", field: "vl_custo_fechamento", width: "11%", styles: "text-align: right;min-width:80px;"} //, formatter: formatTextCustoCorrigido }
                      ],
                    //canSort: true,
                    //noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["8", "16", "24", "32", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "8",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridItemFechamento");
                gridItemFechamento.canSort = function () { return true };
                gridItemFechamento.startup();

                //*** Cria os botões do link de ações **\\
                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridFechamento, 'todosItensFechamento', ['pesquisarFechamento', 'relatorioFechamento']); pesquisarFechamento(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridFechamento', 'selecionadoFechamento', 'cd_fechamento', 'selecionaTodosFechamento', ['pesquisarFechamento', 'relatorioFechamento'], 'todosItensFechamento');
                    }
                });
                menu.addChild(menuItensSelecionados);

                buttonIC = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensFechamento",
                    dropDown: menu,
                    id: "todosItensFechamento"
                });
                dom.byId("linkSelecionadosFechamento").appendChild(buttonIC.domNode);

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemover(gridFechamento.itensSelecionados, 'deletarFechamento(itensSelecionados)'); }
                });
                menu.addChild(acaoExcluir);

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridFechamento.itensSelecionados, ObjectStore, Memory); }
                });
                menu.addChild(acaoEditar);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasFechamento",
                    dropDown: menu,
                    id: "acoesRelacionadasFechamento"
                });
                dom.byId("linkAcoesFechamento").appendChild(button.domNode);

                //Botao Incluir Itens
                var buttonFechamento = new Button({
                    label: "Incluir",
                    name: "itensFechamento",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    id: "itensFechamento"
                });
                dom.byId("btnaddItemFech").appendChild(buttonFechamento.domNode);

                //Botao Excluir Itens

                new Button({
                    label: "Excluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        excluirItens(ObjectStore, Memory);
                    }
                }, "btndelItem");

                // Adiciona link de ações:
                var menuFechamento = new DropDownMenu({ style: "height: 25px" });

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { }
                });
                menuFechamento.addChild(acaoRemover);


                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: "dijitEditorIcon dijitEditorIconSave",
                    onClick: function () {
                        incluirFechamento();
                    }
                }, "incluirFechamento");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        editarFechamento();
                    }
                }, "alterarFechamento");


                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            deletarFechamento();
                        });
                    }
                }, "deleteFechamento");


                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                    onClick: function () {
                        limparFechamento(ObjectStore, Memory);
                    }
                }, "limparFechamento");


                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValuesFechamento(gridFechamento);
                        setarTabCad();
                    }
                }, "cancelarFechamento");


                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadFechamento").hide();
                    }
                }, "fecharFechamento");


                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisarFechamento(true);
                    }
                }, "pesquisarFechamento");

                decreaseBtn(document.getElementById("pesquisarFechamento"), '32px');


                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        pesquisarItensFechamento();
                    }
                }, "pesItem");
                decreaseBtn(document.getElementById("pesItem"), '32px');

                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        if (!dijit.byId("formAlterarValorVenda").validate())
                            return false;
                        if (dijit.byId("gridItemFechamento").store.objectStore.data.length > 0)
                            ajustaValor();
                    }
                }, "btnAlterarValorVenda");

                new Button({
                    label: "Alterar Valor", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        if (dijit.byId("gridItemFechamento").store.objectStore.data.length > 0) {
                            dijit.byId("pcValorVenda").reset();
                            dijit.byId("cadAlterarValorPreco").show();
                        }
                    }
                }, "btnAlterarValor");

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {
                            limparFechamento(ObjectStore, Memory);
                            setarTabCad();
                            dojo.byId('tabContainerFechamento_tablist').children[3].children[0].style.width = '100%';
                            apresentaMensagem('apresentadorMensagem', null);
                            dijit.byId("itensFechamento").set("disabled", true);
                            dijit.byId("btndelItem").set("disabled", true);
                            dijit.byId("btnAlterarValor").set("disabled", true);
                            dijit.byId("pesItem").set("disabled", true);
                            IncluirAlterar(1, 'divAlterarFechamento', 'divIncluirFechamento', 'divExcluirFechamento', 'apresentadorMensagemFechamento', 'divCancelarFechamento', 'divLimparFechamento');
                            dijit.byId("cadFechamento").show();
                            dijit.byId('tabContainerFechamento').resize();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoFechamento");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        xhr.get({
                            url: Endereco() + "/api/financeiro/getUrlRelatorioFechamento?" + getStrGridParameters('gridFechamento') + "ano=" + dijit.byId("edAnoFechamento").value + "&mes=" + dijit.byId("edMesFechamento").value + "&balanco=" + dojo.byId("balancoFechamento").checked + '&dta_ini=' + dojo.byId('dta_ini').value + '&dta_fim=' + dojo.byId('dta_fim').value,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                }, "relatorioFechamento");
                populaTipoGrupoEstoque();
                //getParametros(xhr)

                dijit.byId("dtaFec").on("change", function (e) {
                    //if ((hasValue(dijit.byId("nm_ano").value) || hasValue(e)) && hasValue(dijit.byId("nm_mes").value))
                    if ((hasValue(dijit.byId("dtaFec").value) || hasValue(e)))
                        dijit.byId("itensFechamento").set("disabled", false);
                    else
                        dijit.byId("itensFechamento").set("disabled", true);
                });
                //dijit.byId("nm_mes").on("change", function (e) {
                //    if (hasValue(dijit.byId("nm_ano").value) && (hasValue(dijit.byId("nm_mes").value) || hasValue(e)))
                //        dijit.byId("itensFechamento").set("disabled", false);
                //    else
                //        dijit.byId("itensFechamento").set("disabled", true);
                //});


                hideCarregando();
                dijit.byId("itensFechamento").on("click", function () {
                    try {
                        dojo.byId("trAnoMes").style.display = "";
                        if (!hasValue(dijit.byId("gridPesquisaItem")))
                            montargridPesquisaItem(function () {
                                populaTipoItemMovimentaEstoque();
                                dijit.byId("fecharItemFK").on("click", function (e) { dijit.byId("fkItem").hide(); });
                                abrirItemFK(dijit.byId("descItemFech").value, dijit.byId("inicioItemFech").checked, dijit.byId("tipoItensFech").value, dijit.byId("grupoItensFech").value, true);
                                //abrirItemFKCadastro(xhr, ready, Memory, FilteringSelect,true);
                                dijit.byId("pesquisarItemFK").on("click", function (e) {
                                    apresentaMensagem("apresentadorMensagemItemFK", null);
                                    abrirItemFK(dijit.byId("descItemFech").value, dijit.byId("inicioItemFech").checked, dijit.byId("tipoItensFech").value, dijit.byId("grupoItensFech").value, false);
                                });
                                popularGrupoItem();
                            }, false, false);
                        else
                            abrirItemFK(dijit.byId("descItemFech").value, dijit.byId("inicioItemFech").checked, dijit.byId("tipoItensFech").value, dijit.byId("grupoItensFech").value, true);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323068', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['edAnoFechamento', 'edMesFechamento'], 'pesquisarFechamento', ready);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function populaTipoGrupoEstoque() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.get({
            url: Endereco() + "/api/financeiro/GetTipoGrupoItem",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataGrupoAtivo) {
            try{
                var data = jQuery.parseJSON(dataGrupoAtivo).retorno;
                loadSelect(data.grupos, "grupoItensFech", 'cd_grupo_estoque', 'no_grupo_estoque', 0);
                loadSelect(data.tipos, "tipoItensFech", 'cd_tipo_item', 'dc_tipo_item', 0);
                dijit.byId("grupoItensFech").set("required", false);
                dijit.byId("tipoItensFech").set("required", false);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemItemFK', error);
        });
    });
}

function eventoEditar(itensSelecionados, ObjectStore, Memory) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            var gridFechamento = dijit.byId('gridFechamento');

            limparFechamento(ObjectStore, Memory);
            apresentaMensagem('apresentadorMensagem', '');
            gridFechamento.itemSelecionado = itensSelecionados[0];
            keepValuesFechamento(gridFechamento);
            setarTabCad();
            dijit.byId("cadFechamento").show();
            IncluirAlterar(0, 'divAlterarFechamento', 'divIncluirFechamento', 'divExcluirFechamento', 'apresentadorMensagemFechamento', 'divCancelarFechamento', 'divLimparFechamento');
            dijit.byId('tabContainerFechamento').resize(true);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainerFechamento");
        var pane = dijit.byId("tabPrincipalTurma");
        tabs.selectChild(pane);
    } catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
    } catch (e) {
        postGerarLog(e);
    }
}

function editarFechamento() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemFechamento', null);
        if (!dijit.byId("formFechamento").validate())
            return false;
        var fechamento = montarFechamento();
        showCarregando();
        require(["dojo/_base/xhr", "dojox/json/ref", "dojo/dom"], function (xhr, ref, dom) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postAlterarFechamento",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(fechamento)
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridFechamento';
                    var todos = dojo.byId("todosItensFechamento_label");
                    var grid = dijit.byId(gridName);

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadFechamento").hide();
                    removeObjSort(grid.itensSelecionados, "cd_fechamento", dom.byId("cd_fechamento").value);
                    insertObjSort(grid.itensSelecionados, "cd_fechamento", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoFechamento', 'cd_fechamento', 'selecionaTodosFechamento', ['pesquisarFechamento', 'relatorioFechamento'], 'todosItensFechamento', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_fechamento");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemFechamento', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function incluirFechamento() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemFechamento', null);
        if (!dijit.byId("formFechamento").validate())
            return false;
        var fechamento = montarFechamento();
        showCarregando();
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postFechamento",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(fechamento)
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridFechamento';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadFechamento").hide();

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];

                    insertObjSort(grid.itensSelecionados, "cd_fechamento", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoFechamento', 'cd_fechamento', 'selecionaTodosFechamento', ['pesquisarFechamento', 'relatorioFechamento'], 'todosItensFechamento', 2);
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_fechamento");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemFechamento', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function limparFechamento(ObjectStore, Memory) {
    try{
        apresentaMensagem('apresentadorMensagemFechamento', null);
        getLimpar('#formFechamento');
        clearForm('formFechamento');
        getLimpar('#formItensFechamento');
        clearForm('formItensFechamento');
        getLimpar('#formObsFechamento');
        clearForm('formObsFechamento');
        dijit.byId("dtaFec").set("value", null);
        dijit.byId("dtaFec").set("disabled", false);
        //dijit.byId("nm_ano").set("disabled", false);
        //dijit.byId("nm_mes").set("disabled", false);
        dijit.byId("itensFechamento").set("disabled", true);
        dijit.byId("btndelItem").set("disabled", true);
        dijit.byId("btnAlterarValor").set("disabled", true);
        saldosItens = [];

        var gridItemFechamento = dijit.byId("gridItemFechamento");
        if (hasValue(gridItemFechamento)) {
            gridItemFechamento.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridItemFechamento.itenSelecionado = [];
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarFechamento(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getFechamentoSearch?ano=" + dijit.byId("edAnoFechamento").value + "&mes=" + dijit.byId("edMesFechamento").value + "&balanco=" + dojo.byId("balancoFechamento").checked + '&dta_ini=' + dojo.byId('dtaIni').value + '&dta_fim=' + dojo.byId('dtaFim').value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridFechamento");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function deletarFechamento(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            var grade = true;
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                if (dojo.byId('cd_fechamento').value != 0)
                    itensSelecionados = [{
                        cd_fechamento: parseInt(dojo.byId("cd_fechamento").value)
                    }];
                grade = false;
            }
            showCarregando();

            xhr.post({
                url: Endereco() + "/api/financeiro/postDeleteFechamentos",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                var gridName = 'gridFechamento';
                var todos = dojo.byId("todosItensFechamento_label");
                var grid = dijit.byId(gridName);

                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cadFechamento").hide();

                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(grid.itensSelecionados, "cd_fechamento", itensSelecionados[r].cd_fechamento);

                pesquisarFechamento(false);

                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarFechamento").set('disabled', false);
                dijit.byId("relatorioFechamento").set('disabled', false);

                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";

                showCarregando();
            },
            function (error) {
                showCarregando();
                if (!grade)
                    apresentaMensagem('apresentadorMensagemFechamento', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

// Metodos FK de Itens
function abrirItemFK(desc, inicio, tipo, grupo, pesq) {
    try{
        if (pesq) {
            dijit.byId("tipo").set("disabled", false);
            dijit.byId("tipo").reset();
            dojo.byId("pesquisaItemServico").value = desc;
            dijit.byId("inicioItemServico").set("value", inicio);
            dijit.byId("tipo").set("value", tipo);
            dijit.byId("grupoPes").set("value", grupo);

            dijit.byId("anoItemFk").set("value", "");
            dijit.byId("mes_items").set("value", "0");
        }
        if (hasValue(dijit.byId("gridPesquisaItem")) && hasValue(dijit.byId("gridPesquisaItem").itensSelecionados))
            dijit.byId("gridPesquisaItem").itensSelecionados = [];
        dijit.byId("fkItem").show();
        dijit.byId("gridPesquisaItem").update();
        pesquisarItemEstoqueFK();
    } catch (e) {
        postGerarLog(e);
    }
}

function populaTipoItemMovimentaEstoque() {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getTipoItemMovimentaEstoque",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataTipoItem) {
        try{
            criarOuCarregarCompFiltering("tipo", jQuery.parseJSON(dataTipoItem).retorno, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_item', 'dc_tipo_item');
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemItemFK', error);
    });
}

function popularGrupoItem() {
    try{
        if (hasValue(dijit.byId("grupoItensFech"),true))
            if (hasValue(dijit.byId("grupoPes"),true))
                dijit.byId("grupoPes").store = new dojo.store.Memory({
                    data: dijit.byId("grupoItensFech").store.data
                });
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItemEstoqueFK() {
    require([
          "dojo/store/JsonRest",
          "dojox/data/JsonRestStore",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory",
          "dojo/domReady!",
          "dojo/parser"
    ], function (JsonRest, JsonRestStore, ObjectStore, Cache, Memory) {
        try {

            if (!validarAnoMes())
                return false;

            var grupoItem = hasValue(dijit.byId("grupoPes").value) ? dijit.byId("grupoPes").value : 0;
            var tipoItemInt = hasValue(dijit.byId("tipo").value) ? dijit.byId("tipo").value : -1;
            var ano = hasValue($('#anoItemFk').val()) ? $('#anoItemFk').val() : 0;
            var mes = hasValue($('#mes_items').val()) ? $('#mes_items').val() : 0;
            var listaItensFech = [];
            var gridItem = dijit.byId("gridItemFechamento");
            if (saldosItens != null && saldosItens.length > 0)
                for (var i = 0; i < saldosItens.length; i++)
                    listaItensFech.push(saldosItens[i].cd_item);//gridItem.store.objectStore.data;

            dojo.xhr.post({
                url: Endereco() + "/financeiro/postItensFechamento",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson(listaItensFech)
            }).then(function (data) {
                var myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/financeiro/getItensSearchEstoque?desc=" + encodeURIComponent(document.getElementById("pesquisaItemServico").value) + "&inicio=" + document.getElementById("inicioItemServico").checked + "&status=" +
                                            retornaStatus("statusItemFK") + "&tipoItemInt=" + tipoItemInt + "&grupoItem=" + grupoItem + "&ano=" + ano + "&mes=" + mes,
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                       idProperty: "cd_item"
                   }
                ), Memory({ idProperty: "cd_item" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaItem");
                grid.setStore(dataStore);
                
            },
            function (error) {
                apresentaMensagem(dojo.byId("apresentadorMensagemItemFK").value, error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function validarAnoMes() {
    valid = true;
    if ($('#anoItemFk').val() && !$('#mes_items').val()) {
        dijit.byId("mes_items").set("required", true);
        valid = false;
    }
    else
        dijit.byId("mes_items").set("required", false);

    if (!$('#anoItemFk').val() && $('#mes_items').val()) {
        dijit.byId("anoItemFk").set("required", true);
        valid = false;
    }
    else
        dijit.byId("anoItemFk").set("required", false);

    if (!dijit.byId("mes_items").validate())
        valid = false;

    return valid;
}

function validaEntradaMasterMaterialDidatico(item) {
    try {
        var retorno = { valid: true, message: '' };

        if (item.cd_tipo_item == MATERIAL_DIDATICO && item.id_material_didatico && eval(MasterGeral()) == false) {

            retorno = { valid: false, message: msgErroMasterCrudItemMaterialDidatico };
        }
        if (item.id_voucher_carga && eval(MasterGeral()) == false) {

            retorno = { valid: false, message: msgErroMasterCrudItemVoucher };
        }

        return retorno;
    } catch (e) {
        postGerarLog(e);
    }

}

function retornarItemFK() {
    try{
        var gridPesquisaItem = dijit.byId("gridPesquisaItem");
        var i_ok = 1;
        apresentaMensagem('apresentadorMensagemFechamento', null);
        if (!hasValue(gridPesquisaItem.itensSelecionados) || gridPesquisaItem.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else {
            for (var i = 0; i < gridPesquisaItem.itensSelecionados.length; i++) {

                if (validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[i]).valid == false) {
                    var retornoValidaCompra = validaEntradaMasterMaterialDidatico(gridPesquisaItem.itensSelecionados[i]);
                    if (retornoValidaCompra.valid == false) {
                        caixaDialogo(DIALOGO_ERRO, retornoValidaCompra.message, null);
                        i_ok = 0;
                        break;
                    }
                }
            }
            if (i_ok == 0) return false
        }
        var gridItemFechamento = dijit.byId("gridItemFechamento");
        var dados = gridItemFechamento.store.objectStore.data;
        var fechamento = {
            itensFechamento: gridPesquisaItem.itensSelecionados,
            dt_fechamento: hasValue(dojo.byId("dtaFec").value) ? dojo.date.locale.parse(dojo.byId("dtaFec").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            //nm_ano_fechamento: dojo.byId('nm_ano').value.replace('.', ''),
            //nm_mes_fechamento: dijit.byId('nm_mes').value,
            SaldosItens: dados
        };
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postGerarEstoque",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(fechamento)
            }).then(function (data) {
                var itens = jQuery.parseJSON(data).retorno;

                gridItemFechamento.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itens }) }));
                if (hasValue(itens) && itens.length > 0) {
                    quickSortObj(itens, 'cd_item');
                    $.each(itens, function (index, value) {
                        insertObjSort(saldosItens, "cd_item", value, false);
                    });
                }
                for (var i = 0; i < saldosItens.length; i++)
                    saldosItens[i].cd_fechamento = dojo.byId('cd_fechamento').value;
                dijit.byId("fkItem").hide();
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemItemFK', error);
            });
        });

        dijit.byId("dtaFec").set("disabled", true);
        //dijit.byId("nm_ano").set("disabled", true);
        //dijit.byId("nm_mes").set("disabled", true);
        dijit.byId("pesItem").set("disabled", false);
        dijit.byId("btndelItem").set("disabled", false);
        dijit.byId("btnAlterarValor").set("disabled", false);

    } catch (e) {
        postGerarLog(e);
    }
}

function excluirItens(ObjectStore, Memory) {
    try{
        var gridItemFechamento = dijit.byId("gridItemFechamento");
        var dados = gridItemFechamento.store.objectStore.data;
        var dadosContinua = saldosItens;
        var removerItem = [];
        quickSortObj(dadosContinua, 'cd_item');
        $.each(dados, function (index, value) {
            var qtd_saldo_fechamento = hasValue(value.qt_saldo_fechamento) ? value.qt_saldo_fechamento : 0;
            var qtd_saldo_data = hasValue(value.qt_saldo_data) ? value.qt_saldo_data : 0;

            var valorCustoAtual = hasValue(value.vlCustoAtual) ? parseFloat(value.vlCustoAtual.replace('.', '')) : 0;
            var valorCustoFechamento = hasValue(value.vl_custo_fechamento) ? value.vl_custo_fechamento : 0;
            var valorVendaAtual = hasValue(value.vlVendaAtual) ? parseFloat(value.vlVendaAtual.replace('.', '')) : 0;
            var valorVendaFechamento = hasValue(value.vl_venda_fechamento) ? value.vl_venda_fechamento : 0;
            if ((qtd_saldo_fechamento == qtd_saldo_data) || (valorCustoAtual == valorCustoFechamento) || (valorVendaAtual == valorVendaFechamento))
                removerItem.push(value);
        });
        if (removerItem != null && removerItem.length > 0)
            for (var j = 0; j < removerItem.length; j++) {
                if (eval(MasterGeral()) || (!removerItem[j].id_material_didatico && !removerItem[j].id_voucher_carga))
                removeObjSort(dadosContinua, "cd_item", removerItem[j].cd_item);
            }
        var dataStore = new ObjectStore({ objectStore: new Memory({ data: dadosContinua }) });
        gridItemFechamento.setStore(dataStore);
        saldosItens = dadosContinua;
        if (dadosContinua.length > 0) {
            dijit.byId("dtaFec").set("disabled", true);
            //dijit.byId("nm_ano").set("disabled", true);
            //dijit.byId("nm_mes").set("disabled", true);
            dijit.byId("pesItem").set("disabled", false);
            dijit.byId("btndelItem").set("disabled", false);
            dijit.byId("btnAlterarValor").set("disabled", false);
        }
        else {
            dijit.byId("dtaFec").set("disabled", false);
            //dijit.byId("nm_ano").set("disabled", false);
            //dijit.byId("nm_mes").set("disabled", false);
            dijit.byId("pesItem").set("disabled", true);
            dijit.byId("btndelItem").set("disabled", true);
            dijit.byId("btnAlterarValor").set("disabled", true);
        }
        limparPesquisaCad();
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarItensFechamento() {
    //Pesquisar somente quando tiver itens retornados
    if (saldosItens != null && saldosItens.length > 0)
        require(["dojo/_base/xhr", "dojox/json/ref", "dojo/data/ObjectStore", "dojo/store/Memory"], function (xhr, ref, ObjectStore, Memory) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postSaldoItemLocal",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    listaSaldo: saldosItens,
                    no_item: dijit.byId("descItemFech").value,
                    inicio: dojo.byId("inicioItemFech").checked,
                    cd_grupo_estoque: dijit.byId("grupoItensFech").value,
                    cd_tipo_item: dijit.byId("tipoItensFech").value
                })
            }).then(function (data) {
                try{
                    data = jQuery.parseJSON(data).retorno.listaSaldo;
                    var dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) });
                    var grid = dijit.byId("gridItemFechamento");
                    grid.setStore(dataStore);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        });
}

function ajustaValor() {
    try{
        var itens = dijit.byId("gridItemFechamento").store.objectStore.data;
        if(hasValue(itens)) {
            for (var i = 0; i < itens.length; i++)
                if (itens[i].cd_tipo_item != CUSTOSDESPESAS) {
                    itens[i].vl_venda_fechamento = itens[i].vl_venda_fechamento + ((itens[i].vl_venda_fechamento * parseFloat(dijit.byId("pcValorVenda").value)) / 100);
                    quickSortObj(saldosItens, 'cd_item');

                    var posicao = binaryObjSearch(saldosItens, 'cd_item', itens[i].cd_item);
                    saldosItens[posicao].cd_fechamento = dojo.byId('cd_fechamento').value;
                    saldosItens[posicao].vl_venda_fechamento = itens[i].vl_venda_fechamento;
                    saldosItens[posicao].editado = true;

                }
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itens }) });
            var grid = dijit.byId("gridItemFechamento");
            grid.setStore(dataStore);
            dijit.byId("gridItemFechamento").value = "";
            dijit.byId("cadAlterarValorPreco").hide();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaCad() {
    dijit.byId("descItemFech").set("Value", "");
    dijit.byId("inicioItemFech").set("checked", false);
    dijit.byId("grupoItensFech").reset();
    dijit.byId("tipoItensFech").reset();

}