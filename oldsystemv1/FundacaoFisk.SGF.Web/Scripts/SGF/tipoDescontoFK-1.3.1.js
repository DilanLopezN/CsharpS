var ORIGEMDESCMENSALMATRICULA = 1, ORIGEMDESCADITAMENTO = 2;

//#region
//Tipo Desconto
function formatCheckBoxTipoDesconto(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoDescontoFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTiposDesconto');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_desconto", grid._by_idx[rowIndex].item.cd_tipo_desconto);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_desconto', 'selecionadoTipoDesconto', -1, 'selecionaTodosTiposDesconto', 'selecionaTodosTiposDesconto', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_desconto', 'selecionadoTipoDesconto', " + rowIndex + ", '" + id + "', 'selecionaTodosTiposDesconto', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaTipoDesconto() {
    try{
        dojo.byId("percentualFK").value = "";
        dijit.byId('inicioDescTipoDescontoFK').set('checked', false);
        dijit.byId("inicioAlunoFK").reset();
        if (hasValue(dijit.byId("statusTipoDescontoFK")))
            dijit.byId("statusTipoDescontoFK").set("value", 1);
        if (hasValue(dijit.byId("incideBaixaFK")))
            dijit.byId("incideBaixaFK").set("value", 0);
        if (hasValue(dijit.byId("pparcFK")))
            dijit.byId("pparcFK").set("value", 0);
        dojo.byId('percentualFK').value = "";
        dojo.byId('pesquisaTipoDescontoFK').value = "";
        if (hasValue(dijit.byId("gridTipoDescontoFK")) && hasValue(dijit.byId("gridTipoDescontoFK").itensSelecionados))
            dijit.byId("gridTipoDescontoFK").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function montarTipoDescontoFK(especializada, pFuncao) {
    require([
       "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dijit/form/Button",
        "dojo/ready"
    ], function (EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready) {
        ready(function () {
            try{
                montarStatus("statusTipoDescontoFK");
                montarStatusSN("incideBaixaFK");
                montarStatusSN("pparcFK");
                var myStore = null;
                var storePesq = null
                if (!especializada) {
                    myStore = Cache(
                                    JsonRest({
                                        target: Endereco() + "/api/financeiro/gettipodescontosearch?descricao=&inicio=false&status=1&IncideBaixa=0&PParc=0&percentual=" + dijit.byId("percentualFK").value,
                                        handleAs: "json",
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                        idProperty: "cd_tipo_desconto"
                                    }), Memory({ idProperty: "cd_tipo_desconto" }));
                    storePesq = new ObjectStore({ objectStore: myStore });
                } else
                    storePesq = new ObjectStore({ objectStore: new Memory({ data: null }) });


                var gridTipoDescontoFK = new EnhancedGrid({
                    store: storePesq,
                    structure: [
                         { name: "<input id='selecionaTodosTiposDesconto' style='display:none'/>", field: "selecionadoTipoDesconto", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoDesconto },
                       // { name: "Código", field: "cd_tipo_desconto", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                        { name: "Tipo Desconto", field: "dc_tipo_desconto", width: "40%" },
                        { name: "Baixa", field: "incide_baixa", width: "10%", styles: "text-align: center;" },
                        //{ name: "1ºParcela", field: "incide_parcela_1", width: "10%", styles: "text-align: center;" },
                        { name: "Percentual", field: "pc_desc", width: "10%", styles: "text-align: right;" },
                        { name: "Ativo", field: "tipo_desconto_ativo", width: "10%", styles: "text-align: center;" }
                    ],
                    canSort: false,
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
                }, "gridTipoDescontoFK"); // make sure you have a target HTML element with this id
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridTipoDescontoFK.pagination.plugin._paginator.plugin.connect(gridTipoDescontoFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridTipoDescontoFK, 'cd_tipo_desconto', 'selecionaTodosTiposDesconto');
                });
                gridTipoDescontoFK.startup();
                gridTipoDescontoFK.canSort = function (col) { return Math.abs(col) != 1; };
                gridTipoDescontoFK.on("RowDblClick", function (evt) {
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    apresentaMensagem('apresentadorMensagemTipoDescontoFK', '');
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridTipoDescontoFK, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosTiposDesconto').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_tipo_desconto', 'selecionadoTipoDesconto', -1, 'selecionaTodosTiposDesconto', 'selecionaTodosTiposDesconto', 'gridTipoDescontoFK')", gridTipoDescontoFK.rowsPerPage * 3);
                    });
                });
            
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            if (dijit.byId("gridTipoDescontoFK").itensSelecionados == null)
                                caixaDialogo(DIALOGO_ERRO, msgNotSelectReg, null);
                            else
                                if (dijit.byId("gridTipoDescontoFK").itensSelecionados.length > 1)
                                    caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                                else {
                                    var tipoOrigem = dojo.byId("idOrigemTipoDescFK").value;
                                    if (hasValue(tipoOrigem))
                                        switch (parseInt(tipoOrigem)) {
                                            case ORIGEMDESCMENSALMATRICULA:
                                            case ORIGEMDESCADITAMENTO:
                                                retornarTipoDescontoFK();
                                                break;
                                            default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                                                return false;
                                                break;
                                        }
                                }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaTipoDescontoFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("proTipoDescontoFK").hide();
                    }
                }, "fecharTipoDescontoFK");

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagemTipoDescontoFK', null);
                        if (!especializada)
                            pesquisarTipoDescontoFK(true);
                        else {
                            var tipoOrigem = dojo.byId("idOrigemTipoDescFK").value;
                            if (hasValue(tipoOrigem))
                                switch (parseInt(tipoOrigem)) {
                                    case ORIGEMDESCADITAMENTO:
                                    case ORIGEMDESCMENSALMATRICULA:
                                        pesquisarTipoDescontoFK(true);
                                        break;
                                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                                        return false;
                                        break;
                                }
                        }
                    }
                }, "pesquisarTipoDescontoFK");
                decreaseBtn(document.getElementById("pesquisarTipoDescontoFK"), '32px');
                adicionarAtalhoPesquisa(['pesquisaTipoDescontoFK', 'statusTipoDescontoFK', 'incideBaixaFK', 'pparcFK', 'percentualFK'], 'pesquisarTipoDescontoFK', ready);
                if (hasValue(pFuncao))
                    pFuncao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#region montarStatusSN - retornarTipoDesconto - pesquisarTipoDescontoFK

function montarStatusSN(nomElement) {
    require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (ready, Memory, filteringSelect) {
         try{
             var statusStore = new Memory({
                 data: [
                     { name: "Todos", id: "0" },
                     { name: "Sim", id: "1" },
                     { name: "Não", id: "2" }
                 ]
             });
             ready(function () {
                 var status = new filteringSelect({
                     id: nomElement,
                     value: "0",
                     store: statusStore,
                     searchAttr: "name",
                     style: "width: 75px;"
                 }, nomElement);
             });
         }
         catch (e) {
             postGerarLog(e);
         }
     });
};

function pesquisarTipoDescontoFK(limpar) {
    try{
        var per = document.getElementById("percentualFK").value;
        var inicio = document.getElementById("inicioDescTipoDescontoFK").checked;
        var status = retornaStatus("statusTipoDescontoFK");
        var baixa = retornaStatus("incideBaixaFK");
        var primeiraParc = retornaStatus("pparcFK");
        var percentual = parseFloat(document.getElementById("percentualFK").value, 10).toFixed(2);
        var desTipoDesconto = hasValue(document.getElementById("pesquisaTipoDescontoFK").value) ? document.getElementById("pesquisaTipoDescontoFK").value : "";

        require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/gettipodescontosearch?descricao=" + desTipoDesconto + "&inicio=" + inicio + "&status=" + status + "&IncideBaixa=" + baixa + "&PParc=" + primeiraParc + "&percentual=" + percentual,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_tipo_desconto"
                    }
                ), Memory({ idProperty: "cd_tipo_desconto" }));
                dataStore = new ObjectStore({ objectStore: myStore });

                if (dijit.byId("gridTipoDescontoFK") != null && dijit.byId("gridTipoDescontoFK") != undefined) {
                    var gridTipoDescontoFK = dijit.byId("gridTipoDescontoFK");

                    if (limpar)
                        gridTipoDescontoFK.itensSelecionados = [];

                    gridTipoDescontoFK.setStore(dataStore);
                }
            }
            catch (er) {
                postGerarLog(er);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion