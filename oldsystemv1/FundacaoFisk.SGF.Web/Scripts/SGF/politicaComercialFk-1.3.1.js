
function formatCheckBoxPolicaComercialFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPolicaComercialFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosPolicaComercialFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_politica_comercial", grid._by_idx[rowIndex].item.cd_politica_comercial);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_politica_comercial', 'selecionadoPolicaComercialFK', -1, 'selecionaTodosPolicaComercialFK', 'selecionaTodosPolicaComercialFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_politica_comercial', 'selecionadoPolicaComercialFK', " + rowIndex + ", '" + id + "', 'selecionaTodosPolicaComercialFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarPoliticaComercial(funcao) {
    require([
        "dojo/ready",
        "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
	    "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button"
    ], function (ready, EnhancedGrid, Pagination, ObjectStore, Memory, Button) {
        ready(function () {
            try{
                var myStore = null;
                var PolicaComercialFK = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: myStore }) }),
                    structure: [
                        { name: "<input id='selecionaTodosPolicaComercialFK' style='display:none'/>", field: "selecionadoPolicaComercialFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxPolicaComercialFK },
			            { name: "Descrição", field: "dc_politica_comercial", width: "70%" },
                        { name: "N° Parcelas", field: "nm_parcelas", width: "15%" },
                        { name: "Intervalo", field: "nm_intervalo_parcela", width: "15%", styles: "text-align: center;" }
                    ],
                    canSort: false,
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["15", "24", "48", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "15",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridPolicaComercialFK");

                PolicaComercialFK.startup();

                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                    onClick: function () {                    
                    }
                }, "selecionaPolicaComercialFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {} }, "fecharPolicaComercialFK");

                decreaseBtn(document.getElementById("pesquisarPolicaComercialFK"), '32px');
                adicionarAtalhoPesquisa(['desPoliticaComercialFK'], 'pesquisarPolicaComercialFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}


function pesquisaPolicaComercialFK(usarPesquisaPadrao) {
    try{
        if (usarPesquisaPadrao)
            require([
                  "dojo/ready"
            ], function (ready) {
                ready(function () {
                    find(dijit.byId('desPoliticaComercialFK').get('value'), dijit.byId('inicioPolicaComercialFK').checked);
                });
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function find(descriaco, marcado) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/financeiro/getPoliticaComercialByEmpresa?politica=" + descriaco + "&inicio=" + marcado + "&cdEscola=",
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_politica_comercial"
               }
            ), Memory({ idProperty: "cd_politica_comercial" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPolicaComercialFK");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaPoliticaComercialFK() {
    try{
        dijit.byId('desPoliticaComercialFK').set('value', '');
        dijit.byId('inicioPolicaComercialFK').set('checked', false);

        if (hasValue(dijit.byId("gridPolicaComercialFK"))) {
            dijit.byId("gridPolicaComercialFK").currentPage(1);
            if (hasValue(dijit.byId("gridPolicaComercialFK").itensSelecionados))
                dijit.byId("gridPolicaComercialFK").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}