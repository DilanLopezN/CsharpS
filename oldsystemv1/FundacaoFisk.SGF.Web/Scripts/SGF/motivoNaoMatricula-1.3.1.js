

function montarMtvNMat() {
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
                myStore = Cache(
               JsonRest({
                   target: Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvNMatFK").checked + "&status=1",
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                   idProperty: "cd_motivo_nao_matricula"
               }
           ), Memory({ idProperty: "cd_motivo_nao_matricula" }));

                var gridMotivoNMatricula = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                                { name: "<input id='selecionaTodosMtvNMatFK' style='display:none'/>", field: "selecionadoMtvNMatFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMtvNMatFK },
                               // { name: "Código", field: "cd_motivo_nao_matricula", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                                { name: "Motivo da Não Matrícula", field: "dc_motivo_nao_matricula", width: "75%" },
                                { name: "Ativo", field: "motivo_nao_matricula_ativo", width: "80px", styles: "text-align: center;" }
                    ],
                    noDataMessage: "Nenhum registro encontrado.",
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["15", "30", "60", "100", "All"],
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
                }, "gridMotivoNMatricluaFK");
                gridMotivoNMatricula.on("RowDblClick", function () {
                    try {
                        if (hasValue(retornarMtvNMatFK)) {
                            dojo.byId("ehSelectGradeMNMatriculaFK").value = true;
                            retornarMtvNMatFK();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridMotivoNMatricula.startup();
                gridMotivoNMatricula.canSort = function (col) { return Math.abs(col) != 1; };
                montarStatus("statusMtvNMatFK");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () { PesquisarMtvNMatFK(true); }
                }, "pesquisarMtvNMatFK");
                btnPesquisar(document.getElementById("pesquisarMtvNMatFK"));
                decreaseBtn(document.getElementById("pesquisarMtvNMatFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            if (hasValue(retornarMtvNMatFK)) {
                                dojo.byId("ehSelectGradeMNMatriculaFK").value = false;
                                retornarMtvNMatFK();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaMotivoNMatricluaFK");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("proMotivoNMatricula").hide(); } }, "fecharMotivoNMatricluaFK");
                adicionarAtalhoPesquisa(['pesquisaMtvNMatFK', 'statusMtvNMatFK'], 'pesquisarMtvNMatFK', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxMtvNMatFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridMotivoNMatricluaFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMtvNMatFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_nao_matricula", grid._by_idx[rowIndex].item.cd_motivo_nao_matricula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_nao_matricula', 'selecionadoMtvNMatFK', -1, 'selecionaTodosMtvNMatFK', 'selecionaTodosMtvNMatFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_nao_matricula', 'selecionadoMtvNMatFK', " + rowIndex + ", '" + id + "', 'selecionaTodosMtvNMatFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function PesquisarMtvNMatFK(limparItens) {
    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaMtvNMatFK").value) ? Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=&inicio=" + document.getElementById("inicioDescMtvNMatFK").checked + "&status=" + retornaStatus("statusMtvNMatFK") : Endereco() + "/api/secretaria/getmotivonmatriculasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMtvNMatFK").value) + "&inicio=" + document.getElementById("inicioDescMtvNMatFK").checked + "&status=" + retornaStatus("statusMtvNMatFK"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_motivo_nao_matricula"
                }
            ), Memory({ idProperty: "cd_motivo_nao_matricula" }));
            var gridMotivoNMatricula = dijit.byId("gridMotivoNMatricluaFK");

            dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                gridMotivoNMatricula.itensSelecionados = [];

            gridMotivoNMatricula.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaMtvNMatFK() {
    try {
        dojo.byId("pesquisaMtvNMatFK").value = "";
        dijit.byId("statusMtvNMatFK").reset();
        dijit.byId("statusMtvNMatFK").reset();
        if (hasValue(dijit.byId("gridMotivoNMatricluaFK")) && hasValue(dijit.byId("gridMotivoNMatricluaFK").itensSelecionados))
            dijit.byId("gridMotivoNMatricluaFK").itensSelecionados = [];
        // dijit.byId("gridPesquisaCurso").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}