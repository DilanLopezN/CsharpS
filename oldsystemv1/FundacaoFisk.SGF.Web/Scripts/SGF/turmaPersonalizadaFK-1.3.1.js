
function montarGridPesquisaTurmaPersonalizadaFK(funcao) {
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
       "dojo/_base/array",
       "dijit/form/FilteringSelect",
       "dojox/charting/Chart",
       "dojox/charting/plot2d/Columns",
       "dojox/charting/axis2d/Default",
       "dojox/charting/widget/Legend",
       "dojox/charting/themes/MiamiNice"
    ], function (xhr, EnhancedGrid, Pagination,JsonRest, ObjectStore,Cache, Memory, Button, ready, on,array, FilteringSelect, Chart,
                 Columns, Default, Legend, MiamiNice) {
        ready(function () {
            try {
                
                apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                var store = null;
                store = new ObjectStore({ objectStore: new Memory({ data: null }) });

                if (hasValue(dijit.byId("gridPesquisaTurmaPersonalizadaFK"), true))
                    return false;
                var gridPesquisaTurmaPersonalizadaFK = new EnhancedGrid({
                    store: store,
                    structure: [
                        { name: "<input id='selecionaTodosFKTurma' style='display:none'/>", field: "turmaSelecionadaFK", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFK },
                        { name: "Nome", field: "no_turma", width: "15%", styles: "min-width:80px;" },
                        { name: "Professor", field: "no_professor", width: "18%", styles: "min-width:10%; max-width: 10%;" },
                        { name: "Sala", field: "no_sala", width: "10%" },
                        { name: "Vagas", field: "nm_vagas", width: "10%" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["8", "16", "32", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "8",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridPesquisaTurmaPersonalizadaFK");
                gridPesquisaTurmaPersonalizadaFK.rowsPerPage = 5000;
                gridPesquisaTurmaPersonalizadaFK.pagination.plugin._paginator.plugin.connect(gridPesquisaTurmaPersonalizadaFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaTurmaPersonalizadaFK, 'cd_turma', 'selecionaTodosFKTurma');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPesquisaTurmaPersonalizadaFK, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (hasValue(dojo.byId('selecionaTodosFKTurma')) && dojo.byId('selecionaTodosFKTurma').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_turma', 'turmaSelecionadaFK', -1, 'selecionaTodosFKTurma', 'selecionaTodosFKTurma', 'gridPesquisaTurmaPersonalizadaFK')", gridPesquisaTurmaPersonalizadaFK.rowsPerPage * 3);
                    });
                });
                gridPesquisaTurmaPersonalizadaFK.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3 && Math.abs(col) != 4 && Math.abs(col) != 5; };
                gridPesquisaTurmaPersonalizadaFK.on("RowDblClick", function (evt) {
                });
                gridPesquisaTurmaPersonalizadaFK.startup();
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            retornarTurmaPersonalizadaFK();
                            
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaTurmaPersonalizadaFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("proTurmaPersonalizada").hide(); }
                }, "fecharTurmaPersonalizadaFK");

                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaTurmaPersonalizadaFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFKTurma');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_turma', 'turmaSelecionadaFK', -1, 'selecionaTodosFKTurma', 'selecionaTodosFKTurma', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'turmaSelecionadaFK', " + rowIndex + ", '" + id + "', 'selecionaTodosFKTurma', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaFK() {

    dojo.xhr.get({
        preventCache: true,
        handleAs: "json",
        url: Endereco() + "/api/turma/getTurmasPersonalizadas?cdProduto=" + dijit.byId('pesCadProduto').value + "&dtAula=" + dojo.byId("dtIniAula").value + "&hrIni=" + dojo.byId("hrInicial").value + "&hrFim=" + dojo.byId("hrFinal").value
        + "&cd_turma=" + null,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (data != null && data.length > 0) {
                var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data, idProperty: "cd_turma" }) });
                var grid = dijit.byId("gridPesquisaTurmaPersonalizadaFK");
                grid.setStore(dataStore);
                dijit.byId("proTurmaPersonalizada").show();
                showCarregando();
            }
            else {
                showCarregando();
                caixaDialogo(DIALOGO_AVISO, msgNaoEncontrouTurmaPers, null);
                return false;
            }
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagemFks', error);
    });

}