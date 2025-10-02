function formatCheckBoxMtvMatFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridMotivoMatricluaFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvMatFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_matricula", grid._by_idx[rowIndex].item.cd_motivo_matricula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_matricula', 'selecionadoMtvMatFK', -1, 'selecionaTodosMtvMatFK', 'selecionaTodosMtvMatFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_matricula', 'selecionadoMtvMatFK', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvMatFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarMtvMat() {
    require([
      "dojox/grid/EnhancedGrid",
      "dojox/grid/enhanced/plugins/Pagination",
      "dojo/store/JsonRest",
      "dojo/data/ObjectStore",
      "dojo/store/Cache",
      "dojo/store/Memory",
      "dijit/form/Button",
      "dojo/ready",
      "dojo/on"
    ], function (EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on) {
        ready(function () {
            try {
                var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/secretaria/getmotivomatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvMatFK").checked + "&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_matricula"
                }), Memory({ idProperty: "cd_motivo_matricula" }));

                var gridMotivoMatriculaFK = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                                { name: "<input id='selecionaTodosMtvMatFK' style='display:none'/>", field: "selecionadoMtvMatFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvMatFK },
                                { name: "Motivo da Matrícula", field: "dc_motivo_matricula", width: "75%" },
                                { name: "Ativo", field: "motivo_matricula_ativo", width: "80px", styles: "text-align: center;" }
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
                            position: "button"
                        }
                    }
                }, "gridMotivoMatricluaFK");
                gridMotivoMatriculaFK.on("RowDblClick", function () {
                    try {
                        if (hasValue(retornarMtvMatFK)) {
                            dojo.byId("ehSelectGradeMMatriculaFK").value = true;
                            retornarMtvMatFK();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridMotivoMatriculaFK.startup();
                gridMotivoMatriculaFK.canSort = function (col) { return Math.abs(col) != 1; };
                montarStatus("statusMtvMatFK");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconSearchSGF',
                    onClick: function () { PesquisarMtvMatFK(true); }
                }, "pesquisarMtvMatFK");
                decreaseBtn(document.getElementById("pesquisarMtvMatFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            if (hasValue(retornarMtvMatFK)) {
                                dojo.byId("ehSelectGradeMMatriculaFK").value = false;
                                retornarMtvMatFK();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaMotivoMatricluaFK");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("proMotivoMatricula").hide(); } }, "fecharMotivoMatricluaFK");
                adicionarAtalhoPesquisa(['pesquisaMtvMatFK', 'statusMtvMatFK'], 'pesquisarMtvMatFK', ready);
                dijit.byId("statusMtvMatFK").set("disabled", true);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function PesquisarMtvMatFK(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaMtvMatFK").value) ? Endereco() + "/api/secretaria/getmotivomatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvMatFK").checked + "&status=" + retornaStatus("statusMtvMatFK") : Endereco() + "/api/secretaria/getmotivomatriculasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvMatFK").value) + "&inicio=" + document.getElementById("inicioDescMtvMatFK").checked + "&status=" + retornaStatus("statusMtvMatFK"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_matricula"
                }
        ), Memory({ idProperty: "cd_motivo_matricula" }));
        dataStore = new ObjectStore({ objectStore: myStore });
        var gridMotivoMatricluaFK = dijit.byId("gridMotivoMatricluaFK");

        if (limparItens)
            gridMotivoMatricluaFK.itensSelecionados = [];

        gridMotivoMatricluaFK.setStore(dataStore);
    });
}

function limparPesquisaMtvMatFK() {
    try {
        dojo.byId("pesquisaMtvMatFK").value = "";
        dijit.byId("statusMtvMatFK").reset();
        if (hasValue(dijit.byId("gridMotivoMatricluaFK")) && hasValue(dijit.byId("gridMotivoMatricluaFK").itensSelecionados))
            dijit.byId("gridMotivoMatricluaFK").itensSelecionados = [];
        // dijit.byId("gridPesquisaCurso").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}