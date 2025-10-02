
var ENTRADACFOP = 1, SAIDACFOP = 2; SERVICOCFOP = 9;

function formatCheckBoxCFOPFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridCFOPFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCFOPFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_cfop", grid._by_idx[rowIndex].item.cd_cfop);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_cfop', 'selecionadoCFOPFK', -1, 'selecionaTodosCFOPFK', 'selecionaTodosCFOPFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_cfop', 'selecionadoCFOPFK', " + rowIndex + ", '" + id + "', 'selecionaTodosCFOPFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarCFOPFK(funcao) {
    require([
        "dojo/ready",
        "dojox/grid/EnhancedGrid",
	    "dojox/grid/enhanced/plugins/Pagination",
	    "dojo/data/ObjectStore",
        "dojo/store/Memory",
        "dijit/form/Button"
    ], function (ready, EnhancedGrid, Pagination, ObjectStore, Memory, Button) {
        ready(function () {
            try {
                var myStore = null;
                var gridCFOPFK = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: myStore }) }),
                    structure: [
                        { name: "<input id='selecionaTodosCFOPFK' style='display:none'/>", field: "selecionadoCFOPFK", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCFOPFK },
			            { name: "Descrição", field: "dc_cfop", width: "70%", styles: "min-width:80px;" },
						{ name: "Número CFOP", field: "nm_cfop", width: "20%", styles: "text-align:center; min-width:40px;" }
                    ],
                    canSort: false,
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["13", "23", "39", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "13",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridCFOPFK");
                gridCFOPFK.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 2; };
                gridCFOPFK.startup();
                gridCFOPFK.itensSelecionados = new Array();
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect',
                    onClick: function () {

                    }
                }, "selecionaCFOPFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("fkCFOP").hide();
                    }
                }, "fecharCFOPFK");

                decreaseBtn(document.getElementById("pesquisarCFOPFK"), '32px');
                adicionarAtalhoPesquisa(['descCFOPFK', 'nm_CFOPFK'], 'pesquisarCFOPFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function limparPesquisaCFOPFK() {
    try {
        var compGridCFOPFK = dijit.byId("gridCFOPFK");
        dijit.byId('descCFOPFK').set('value', '');
        dijit.byId('nm_CFOPFK').set('value', '');
        dijit.byId('inicioCFOPFK').set('checked', false);
        if (hasValue(compGridCFOPFK)) {
            compGridCFOPFK.currentPage(1);
            if (hasValue(compGridCFOPFK.itensSelecionados))
                compGridCFOPFK.itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function searchCFOP(id_natureza) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var nm_CFOP = hasValue(dijit.byId("nm_CFOPFK").value) ? dijit.byId("nm_CFOPFK").value : 0;
            if (!hasValue(id_natureza))
                id_natureza = 0;
            var myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/fiscal/SearchCFOP?descricao=" + dojo.byId("descCFOPFK").value + "&inicio=" + document.getElementById("inicioCFOPFK").checked +
                                        "&nm_CFOP=" + nm_CFOP + "&id_natureza_CFOP=" + id_natureza,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_cfop"
               }
            ), Memory({ idProperty: "cd_cfop" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridCFOPFK");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

