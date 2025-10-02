function montarIntervalo(mostraTodos) {
    require(["dojo/store/Memory"],
     function (Memory) {
         try{
             var dados = [
                     { name: "Dia", id: "1" },
                     { name: "Mês", id: "2" }
             ]
             var statusStore = new Memory({
                 data: dados
             });
             dijit.byId("nm_periodo_intervalo").store = statusStore;
             dijit.byId("nm_periodo_intervalo").set("value", 1);
         } catch (e) {
             postGerarLog(e);
         }
     });
}

function formatCheckBoxPoliticaComercial(value, rowIndex, obj) {
    try{
        var gridName = 'gridPoliticaComercial';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPoliticaComercial');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_politica_comercial", grid._by_idx[rowIndex].item.cd_politica_comercial);

            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_politica_comercial', 'selecionadoPoliticaComercial', -1, 'selecionaTodosPoliticaComercial', 'selecionaTodosPoliticaComercial', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_politica_comercial', 'selecionadoPoliticaComercial', " + rowIndex + ", '" + id + "', 'selecionaTodosPoliticaComercial', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxParcPol(value, rowIndex, obj) {
    try{
        var gridName = 'gridParcPol';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosParcPol');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_item_politica", grid._by_idx[rowIndex].item.cd_item_politica);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_item_politica', 'selecionadoParcPol', -1, 'selecionaTodosParcPol', 'selecionaTodosParcPol', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_item_politica', 'selecionadoParcPol', " + rowIndex + ", '" + id + "', 'selecionaTodosParcPol', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroPoliticaComercial() {

    //Criação da Grade de sala
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
        "dijit/form/DateTextBox"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox) {
        ready(function () {
            try{
                montarIntervalo(true);
                var myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/financeiro/getPoliticaComercialSearch?descricao=&inicio=false&ativo=1&parcIguais=false&vencFixo=false",
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
               }), Memory({}));

                var gridPoliticaComercial = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosPoliticaComercial'/>", field: "selecionadoPoliticaComercial", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPoliticaComercial },
                    	{ name: "Descrição", field: "dc_politica_comercial", width: "47%", styles: "min-width:80px;" },
                    	{ name: "Vencimento Fixo", field: "venc_fixo", width: "8%", styles: "text-align: center; min-width:80px;" },
                    	{ name: "Parcelas Iguais", field: "parc_iguais", width: "8%", styles: "text-align: center; min-width:80px;" },
                    	{ name: "Nro Parcelas", field: "nm_parcelas", width: "8%", styles: "text-align: center; min-width:80px;" },
                    	{ name: "Intervalo", field: "nm_intervalo_parcela", width: "8%", styles: "text-align: center; min-width:80px;" },
                    	{ name: "Período", field: "periodo_intervalo", width: "8%", styles: "text-align: center; min-width:80px;" },
                    	{ name: "Ativo", field: "pol_ativa", width: "8%", styles: "text-align: center; min-width:40px; max-width: 50px;" }
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
                }, "gridPoliticaComercial");
                gridPoliticaComercial.canSort = function (col) { return Math.abs(col) != 1 };
                gridPoliticaComercial.pagination.plugin._paginator.plugin.connect(gridPoliticaComercial.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPoliticaComercial, 'cd_politica_comercial', 'selecionaTodosPoliticaComercial');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPoliticaComercial, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosPoliticaComercial').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_politica_comercial', 'selecionaPoliticaComercial', -1, 'selecionaTodosPoliticaComercial', 'selecionaTodosPoliticaComercial', 'gridPoliticaComercial')", gridPoliticaComercial.rowsPerPage * 3);
                    });
                });


                gridPoliticaComercial.startup();
                gridPoliticaComercial.on("RowDblClick", function (evt) {
                    try{
                        apresentaMensagem('apresentadorMensagemPolCom', null);
                        showCarregando();

                        IncluirAlterar(0, 'divAlterarPolCom', 'divIncluirPolCom', 'divExcluirPolCom', 'apresentadorMensagemPolCom', 'divCancelarPolCom', 'divLimparPolCom');
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                        gridPoliticaComercial.itemSelecionado = item;
                        keepValuesPolCom(item, gridPoliticaComercial, true, xhr, Memory, ObjectStore);
                        dijit.byId("cad").show();
                        dijit.byId("tabContainerGridParcPol").resize();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                var gridParcPol = new EnhancedGrid({
                    store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "<input id='selecionaTodosParcPol' style='display:none' />", field: "selecionadoParcPol", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxParcPol },
                        { name: "Número de Dias", field: "nro_dia", width: "35%", styles: "min-width:40px; text-align: left;" },
                        { name: "% Parcela", field: "pc_politica", width: "60%", styles: "min-width:80px; text-align: left;" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["5", "10", "20", "40", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "5",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridParcPol");


                gridParcPol.canSort = function (col) { return Math.abs(col) != 1 };
                gridParcPol.startup();
                gridParcPol.on("RowDblClick", function (evt) {
                    try{
                        var idx = evt.rowIndex,
                           item = this.getItem(idx),
                           store = this.store;
                        dijit.byId("dialogParc").show();
                        document.getElementById("cd_item").value = item.cd_item;
                        dojo.byId("cd_parc_politica").value = item.cd_item_politica,
                        dijit.byId("perParc").set("value", item.pc_politica),
                        dojo.byId("nroDias").value = item.nro_dia > 0 ? item.nro_dia : '';
                        document.getElementById('divIncluirParcela').style.display = "none";
                        document.getElementById('divAlterarParcela').style.display = "";
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);


                //*** Cria os botões do link de ações **\\
                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens('gridPoliticaComercial', 'todosItensPolCom', ['pesquisarPolCom', 'relatorioPolCom']);
                        pesquisarPoliticaComercial(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridPoliticaComercial', 'selecionadoPoliticaComercial', 'cd_politica_comercial', 'selecionaTodosPoliticaComercial', ['pesquisarPolCom', 'relatorioPolCom'], 'todosItensPolCom');
                    }
                });
                menu.addChild(menuItensSelecionados);

                //Botao Incluir
                var button = new Button({
                    label: "Incluir",
                    name: "parcCom",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    id: "parcCom",
                    onClick: function () {
                        try{
                            $("#formParcela").get(0).reset();
                            dojo.byId("cd_parc_politica").value = 0;
                            document.getElementById('divIncluirParcela').style.display = "";
                            document.getElementById('divAlterarParcela').style.display = "none";

                            dijit.byId("dialogParc").show();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                dom.byId("btnaddParcCom").appendChild(button.domNode);
                // Adiciona link de ações:
                var menuT = new DropDownMenu({ style: "height: 25px" });

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        eventoRemover(dijit.byId("gridParcPol").itensSelecionados, 'deletaParcPol(itensSelecionados)');
                    }
                });
                menuT.addChild(acaoRemover);

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditarParcela(dijit.byId("gridParcPol").itensSelecionados);
                    }
                });
                menuT.addChild(acaoEditar);

                var buttonR = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasT",
                    dropDown: menuT,
                    id: "acoesRelacionadasT"
                });
                dom.byId("linkTurma").appendChild(buttonR.domNode);


                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensPolCom",
                    dropDown: menu,
                    id: "todosItensPolCom"
                });
                dom.byId("linkSelecionadosPolCom").appendChild(button.domNode);

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemoverPolCom(gridPoliticaComercial.itensSelecionados); }
                });
                menu.addChild(acaoExcluir);

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditar(gridPoliticaComercial.itensSelecionados, xhr, Memory, ObjectStore);
                    }
                });
                menu.addChild(acaoEditar);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasPolCom",
                    dropDown: menu,
                    id: "acoesRelacionadasPolCom"
                });
                dom.byId("linkAcoesPolCom").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar", iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () {
                        IncluirPoliticaComercial();
                    }
                }, "incluirPolCom");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () { AlterarPoliticaComercial(); }
                }, "alterarPolCom");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarPolCom(); });
                    }
                }, "deletePolCom");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        getLimparPolCom(Memory, ObjectStore);
                    }
                }, "limparPolCom");

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        showCarregando();
                        keepValuesPolCom(null, dijit.byId("gridPoliticaComercial"), false, xhr, Memory, ObjectStore);
                    }
                }, "cancelarPolCom");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cad").hide();
                    }
                }, "fecharPolCom");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null); pesquisarPoliticaComercial(true);
                    }
                }, "pesquisarPolCom");

                decreaseBtn(document.getElementById("pesquisarPolCom"), '32px');
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            IncluirAlterar(1, 'divAlterarPolCom', 'divIncluirPolCom', 'divExcluirPolCom', 'apresentadorMensagemPolCom', 'divCancelarPolCom', 'divLimparPolCom');
                            getLimparPolCom(Memory, ObjectStore);
                            dijit.byId("cad").show();
                          //  dijit.byId("gridParcPol").update();
                            dijit.byId("tabContainerGridParcPol").resize();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoPolCom");

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        xhr.get({
                            url: Endereco() + "/api/financeiro/getUrlRelatorioPoliticaComercial?" + getStrGridParameters('gridPoliticaComercial') + "descricao=" + dijit.byId("descPoliticaComercial").value +
                      "&inicio=" + document.getElementById("inicioPoliticaComercial").checked + "&ativo=" + retornaStatus("statusPoliticaComercial") +
                      "&parcIguais=" + dijit.byId("ckParcIg").checked + "&vencFixo=" + dijit.byId("ckVencFixo").checked,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            try{
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagemPolCom', error);
                        });
                    }
                }, "relatorioPolCom");

                montarStatus("statusPoliticaComercial");
                dojo.byId('parcDiferente').style.display = 'block';

                dijit.byId("id_vencimento_fixo").on("change", function (e) {
                    try{
                        if (!dijit.byId("id_vencimento_fixo").checked) {
                            dijit.byId("nroDias").set("required", true);
                            dijit.byId("nroDias").set("disabled", false);
                        } else {
                            dijit.byId("nroDias").set("required", false);
                            dijit.byId("nroDias").set("disabled", true);
                        }

                        if (dijit.byId("id_parcela_igual").checked && dijit.byId("id_vencimento_fixo").checked) {
                            dijit.byId("nm_periodo_intervalo").set("disabled", true);
                            dijit.byId("nm_periodo_intervalo").set("value", 2);
                        }
                        else
                            if (!dijit.byId("id_parcela_igual").checked && dijit.byId("id_vencimento_fixo").checked) {
                                dijit.byId("nroDias").set("disabled", true);
                                if (dijit.byId("gridParcPol")._by_idx.length > 0) {
                                    for (var i = 0; i < dijit.byId("gridParcPol")._by_idx.length; i++)
                                        dijit.byId("gridParcPol")._by_idx[i].item.nro_dia = '';
                                    dijit.byId("gridParcPol").update();
                                }
                            }
                            else {
                                dijit.byId("nroDias").set("disabled", false);
                                if (dijit.byId("gridParcPol")._by_idx.length > 0) {
                                    for (var i = 0; i < dijit.byId("gridParcPol")._by_idx.length; i++)
                                        if (dijit.byId("gridParcPol")._by_idx[i].item.nro_dia == '' && dijit.byId("gridParcPol")._by_idx[i].item.nm_dias_politica != null)
                                            dijit.byId("gridParcPol")._by_idx[i].item.nro_dia = dijit.byId("gridParcPol")._by_idx[i].item.nm_dias_politica;
                                    dijit.byId("gridParcPol").update();
                                }
                                dijit.byId("nm_periodo_intervalo").set("disabled", false);
                            }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("id_parcela_igual").on("click", function (e) {
                    try{
                        if (dijit.byId("id_parcela_igual").checked) {
                            if (dijit.byId("id_vencimento_fixo").checked) {
                                dijit.byId("nm_periodo_intervalo").set("disabled", true);
                                dijit.byId("nm_periodo_intervalo").set("value", 2);
                            }
                            else
                                if (dijit.byId("id_parcela_igual").checked) {
                                    dijit.byId("nm_periodo_intervalo").set("disabled", false);
                                    dijit.byId("nm_periodo_intervalo").set("value", 1);
                                }
                        }
                        else {
                            dijit.byId("nm_parcelas").set("value", "");
                            dijit.byId("nm_intervalo_parcela").set("value", "");
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("id_parcela_igual").on("change", function (e) {
                    try{
                        if (dijit.byId("id_parcela_igual").checked) {
                            dijit.byId("parcDiferente").set("open", true);
                            dojo.byId('parcDiferente').style.display = 'none';
                            dijit.byId("nm_parcelas").set("disabled", false);
                            dijit.byId("nm_intervalo_parcela").set("disabled", false);

                            dijit.byId("nm_parcelas").set("required", true);
                            dijit.byId("nm_intervalo_parcela").set("required", true);
                            dijit.byId("nm_periodo_intervalo").set("required", true);
                            dojo.byId("infParcela").style.display = "";
                        }
                        else {
                            dijit.byId("parcDiferente").set("open", true);
                            dojo.byId('parcDiferente').style.display = 'block';
                            dijit.byId("gridParcPol").update();
                            dijit.byId("nm_parcelas").set("disabled", true);
                            dijit.byId("nm_intervalo_parcela").set("disabled", true);

                            dijit.byId("nm_parcelas").set("required", false);
                            dijit.byId("nm_intervalo_parcela").set("required", false);
                            dijit.byId("nm_periodo_intervalo").set("required", false);
                            dojo.byId("infParcela").style.display = "none";
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['descPoliticaComercial', 'statusPoliticaComercial'], 'pesquisarPolCom', ready);
                //MODAL TITULO
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { IncluirParcelas(); }
                }, "incluirParcela");
                new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { } }, "cancelarParcela");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogParc").hide(); } }, "fecharParcela");
                new Button({ label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { editarParcela();  } }, "alterarParcela");
                new Button({ label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { } }, "deleteParcela");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        $("#formParcela").get(0).reset();
                        dojo.byId("cd_parc_politica").value = 0;
                    }
                }, "limparParcela");
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323065', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['descPoliticaComercial', 'statusPoliticaComercial'], 'pesquisarPolCom', ready);

                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function IncluirParcelas() {
    try{
        if (!dijit.byId("formParcela").validate())
            return false;
        var cd = dijit.byId("gridParcPol")._by_idx.length;
        dojo.byId("cd_item").value = cd;

        var gridParcPol = dijit.byId("gridParcPol");
        var myNewItem = {
            cd_item: cd,
            nro_dia: document.getElementById('nroDias').value == "" ? "-" : document.getElementById('nroDias').value,
            pc_politica: document.getElementById('perParc').value
        };
        gridParcPol.store.newItem(myNewItem);
        dijit.byId("gridParcPol").store.save();
        dijit.byId("dialogParc").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function getLimparPolCom(Memory, ObjectStore) {
    try{
        $("#formCadPoliticaCom").get(0).reset();
        dojo.byId("cd_politica_comercial").value = 0;
        dijit.byId("id_politica_ativa").set("value", true);
        dijit.byId("id_vencimento_fixo").set("value", false);
        dijit.byId("id_parcela_igual").set("value", false);
        dijit.byId("nm_periodo_intervalo").set("value", 1);
        var griParc = dijit.byId('gridParcPol');
        if (hasValue(griParc)) {
            griParc.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            griParc.itensSelecionados = [];
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesPolCom(value, grid, ehLink, xhr, Memory, ObjectStore) {
    try{
        if (!hasValue(value) && grid != null)
            value = grid.itemSelecionado;

        getLimparPolCom(Memory, ObjectStore);
        if (value.cd_politica_comercial > 0)
            showEditPolDesc(value.cd_politica_comercial, xhr, Memory, ObjectStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function showEditPolDesc(cdPolCom, xhr, Memory, ObjectStore) {
    xhr.get({
        url: Endereco() + "/api/financeiro/getPoliticaComercialById?cdPoliticaCom=" + cdPolCom,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagemPolCom", null);
            loadDataPolCom(data.retorno, Memory, ObjectStore);

            showCarregando();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemPolCom", error);
        showCarregando();
    });

}

function loadDataPolCom(politica, Memory, ObjectStore) {
    try{
        //dijit.byId('id_vencimento_fixo')._onChangeActive = false;
        //dijit.byId('id_parcela_igual')._onChangeActive = false;

        dojo.byId("cd_politica_comercial").value = politica.cd_politica_comercial;
        dijit.byId("dc_politica_comercial").set("value", politica.dc_politica_comercial);
        dijit.byId("id_politica_ativa").set("checked", politica.id_politica_ativa);
        dijit.byId("id_parcela_igual").set("checked", politica.id_parcela_igual);
        dijit.byId("id_vencimento_fixo").set("checked", politica.id_vencimento_fixo);
        dijit.byId("nm_parcelas").set("value", politica.nm_parcelas);
        dijit.byId("nm_intervalo_parcela").set("value", politica.nm_intervalo_parcela);
        //dijit.byId('id_vencimento_fixo')._onChangeActive = true;
        //dijit.byId('id_parcela_igual')._onChangeActive = true;

        dijit.byId("nm_periodo_intervalo").set("value", politica.nm_periodo_intervalo);

        //Preencher a grid de Parcelas, quando não for parcelas iguais
        if (!politica.id_parcela_igual && politica.ItemPolitica) {
            //dijit.byId("gridParcPol").store.objectStore.data = politica.ItemPolitica;
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: politica.ItemPolitica }) });
            var gridParcPol = dijit.byId("gridParcPol");

            gridParcPol.setStore(dataStore);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados, xhr, Memory, ObjectStore) {
    try{
        apresentaMensagem('apresentadorMensagemPolCom', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridPoliticaComercial = dijit.byId('gridPoliticaComercial');

            if (!hasValue(dijit.byId('gridParcPol')))
                montarCadastroPoliticaComercial();
            apresentaMensagem('apresentadorMensagem', '');
            gridPoliticaComercial.itemSelecionado = itensSelecionados[0];
            showCarregando();

            keepValuesPolCom(null, gridPoliticaComercial, true, xhr, Memory, ObjectStore);
            // VERIFICAR DEPOIS
            IncluirAlterar(0, 'divAlterarPolCom', 'divIncluirPolCom', 'divExcluirPolCom', 'apresentadorMensagemPolCom', 'divCancelarPolCom', 'divLimparPolCom');
            dijit.byId("cad").show();
            dijit.byId("tabContainerGridParcPol").resize();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function editarParcela() {
    try{
        if (!dijit.byId("formParcela").validate())
            return false;
        var grid = dijit.byId("gridParcPol");
        for (var i = 0; i < grid._by_idx.length; i++)
            if (grid._by_idx[i].item.cd_item_politica == dojo.byId("cd_parc_politica").value ||
                grid._by_idx[i].item.cd_item == parseInt(dojo.byId("cd_item").value)) {
                grid._by_idx[i].item.nro_dia = dojo.byId("nroDias").value;
                grid._by_idx[i].item.pc_politica = dojo.byId("perParc").value;
                break;
            }
        dijit.byId("dialogParc").hide();
        grid.update();
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarParcela(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            dijit.byId("dialogParc").show();
            document.getElementById("cd_item").value = itensSelecionados[0].cd_item;
            document.getElementById("cd_parc_politica").value = itensSelecionados[0].cd_item_politica,
            document.getElementById("perParc").value = itensSelecionados[0].pc_politica,
            document.getElementById("nroDias").value = itensSelecionados[0].nro_dia > 0 ? itensSelecionados[0].nro_dia : '';
            document.getElementById('divIncluirParcela').style.display = "none";
            document.getElementById('divAlterarParcela').style.display = "";
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function deletaParcPol(itensSelecionados) {

    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
     function (Memory, ObjectStore) {
         try{
             var parc = itensSelecionados;

             if (parc.length > 0) {
                 var arrayParc = dijit.byId("gridParcPol")._by_idx;
                 $.each(parc, function (idx, valueDias) {

                     arrayParc = jQuery.grep(arrayParc, function (value) {
                         return value.item != valueDias;
                     });
                 });
                 var dados = [];
                 $.each(arrayParc, function (index, value) {
                     dados.push(value.item);
                 });
                 var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
                 dijit.byId("gridParcPol").setStore(dataStore);
                 var mensagensWeb = new Array();
                 mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Parcelas excluídas com sucesso.");
                 dijit.byId("gridParcPol").itensSelecionados = null;
             }
         } catch (e) {
             postGerarLog(e);
         }
     });
}

function montarPolCom() {
    try{
        var listaItem = [];
        var grid = dijit.byId("gridParcPol")._by_idx;
        if(!dijit.byId("id_parcela_igual").checked)
            for (var i = 0; i < grid.length; i++) {
                grid[i].item.pc_politica = unmaskFixed(grid[i].item.pc_politica, 2);
                grid[i].item.nm_dias_politica = isNaN(parseInt(grid[i].item.nro_dia)) ? null : parseInt(grid[i].item.nro_dia);
                listaItem[i] = grid[i].item;
            }

        var dataPolCom = {
            cd_politica_comercial : dojo.byId("cd_politica_comercial").value,
            dc_politica_comercial: dijit.byId("dc_politica_comercial").value,
            id_politica_ativa: dijit.byId("id_politica_ativa").checked,
            id_parcela_igual: dijit.byId("id_parcela_igual").checked,
            id_vencimento_fixo: dijit.byId("id_vencimento_fixo").checked,
            nm_parcelas: dijit.byId("nm_parcelas").value,
            nm_intervalo_parcela: dijit.byId("nm_intervalo_parcela").value,
            nm_periodo_intervalo: dijit.byId("nm_periodo_intervalo").value,
            ItemPolitica: listaItem
        }

        return dataPolCom;
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarPoliticaComercial(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
              JsonRest({
                  target: Endereco() + "/api/financeiro/getPoliticaComercialSearch?descricao=" + dojo.byId("descPoliticaComercial").value +
                      "&inicio=" + document.getElementById("inicioPoliticaComercial").checked + "&ativo=" + retornaStatus("statusPoliticaComercial") +
                      "&parcIguais=" + dijit.byId("ckParcIg").checked + "&vencFixo=" + dijit.byId("ckVencFixo").checked,
                  handleAs: "json",
                  headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
              }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPoliticaComercial");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}


function IncluirPoliticaComercial() {
    var polCom = montarPolCom();
    require(["dojo/request/xhr", "dojo/dom", "dojox/json/ref"], function (xhr, dom, ref) {
        try{
            if (!dijit.byId("formCadPoliticaCom").validate())
                return false;
            apresentaMensagem('apresentadorMensagemPolCom', null);
            apresentaMensagem('apresentadorMensagem', null);
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postPoliticaComercial", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(polCom)
            }).then(function (data) {
                if (!hasValue(data.erro)) {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridPoliticaComercial';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();
                    var ativo = dom.byId("id_politica_ativa").checked ? 1 : 2;
                    dijit.byId("statusPoliticaComercial").set("value", ativo);

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];


                    insertObjSort(grid.itensSelecionados, "cd_politica_comercial", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoPoliticaComercial', 'cd_politica_comercial', 'selecionaTodosPoliticaComercial', ['pesquisarPolCom', 'relatorioPolCom'], 'todosItensPolCom');

                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_politica_comercial");
                }
                else
                    apresentaMensagem('apresentadorMensagemPolCom', data);
                showCarregando();
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPolCom', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function AlterarPoliticaComercial() {
    try{
        if (!dijit.byId("formCadPoliticaCom").validate()) 
            return false;
        var polCom = montarPolCom();
        apresentaMensagem('apresentadorMensagemPolCom', null);
        apresentaMensagem('apresentadorMensagem', null);
        showCarregando();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            xhr.post({
                url: Endereco() + "/api/financeiro/postAlterarPoliticaComercial",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(polCom)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridPoliticaComercial';
                        var grid = dijit.byId(gridName);
                        var todos = dojo.byId("todosItensPolCom_label");

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();
                        dijit.byId("statusPoliticaComercial").set("value", 0);
                        removeObjSort(grid.itensSelecionados, "cd_politica_comercial", dom.byId("cd_politica_comercial").value);
                        insertObjSort(grid.itensSelecionados, "cd_politica_comercial", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoPoliticaComercial', 'cd_politica_comercial', 'selecionaTodosPoliticaComercial', ['pesquisarPolCom', 'relatorioPolCom'], 'todosItensPolCom');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_politica_comercial");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemPolCom', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemPolCom', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function deletarPolCom(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_politica_comercial').value != 0)
                    itensSelecionados = [{
                        cd_politica_comercial: dojo.byId("cd_politica_comercial").value
                    }];
            xhr.post({
                url: Endereco() + "/api/financeiro/postDeletePoliticaComercial",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensPolCom_label");

                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();
                    dijit.byId("descPoliticaComercial").set("value", '');

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridPoliticaComercial').itensSelecionados, "cd_politica_comercial", itensSelecionados[r].cd_politica_comercial);

                    pesquisarPoliticaComercial(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarPolCom").set('disabled', false);
                    dijit.byId("relatorioPolCom").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }

            },
            function (error) {
                if (!hasValue(dojo.byId("cad").style.display))
                    apresentaMensagem('apresentadorMensagemPolCom', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function eventoRemoverPolCom(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarPolCom(itensSelecionados); });
    } catch (e) {
        postGerarLog(e);
    }
}