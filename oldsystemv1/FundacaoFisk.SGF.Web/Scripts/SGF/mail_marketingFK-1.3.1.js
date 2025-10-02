function montargridPesquisaMailMarketingFK(funcao) {
    require([
       "dojo/_base/xhr",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
       "dojo/dom-attr",
       "dijit/form/Button",
       "dojo/ready",
       "dojo/on"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, domAttr, Button, ready, on) {
        ready(function () {
            try {
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/emailMarketing/getMalaDiretaPorAluno?cd_pessoa=" + dojo.byId("cd_pessoa_aluno_mat").value + "&assunto=" + dijit.byId("assuntoEmailMktFK").value +
                                            "&dtaIni=" + dojo.byId("dtIni").value + "&dtaFim=" + dojo.byId("dtFim").value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));
                var store = new ObjectStore({ objectStore: myStore });

                //*** Cria a grade de Email Marketing **\\
                var gridMailMarketingFK = new EnhancedGrid({
                    store: store,
                    structure:
                      [
                        { name: "<input id='selecionaTodosMailMarketingFK' style='display:none'/>", field: "selecionadoMailMarketingFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMailMarketingFK },
                        { name: "Assunto", field: "dc_assunto", width: "75%", styles: "min-width:80px;" },
                        { name: "Data", field: "dta_mala_direta", width: "20%", styles: "text-align: center; min-width:80px;" }
                      ],
                    noDataMessage: "Nenhum registro encontrado.",
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["13", "26", "52", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "13",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridPesquisaMailMarketing");
                gridMailMarketingFK.on("RowDblClick", function () {
                    //if (hasValue(retornarMailMarketingFK)) {
                    //    dojo.byId("ehSelectGrade").value = true;
                    //    retornarMailMarketingFK();
                    //}
                }, true);
                gridMailMarketingFK.canSort = function (col) { return Math.abs(col) != 1  };
                gridMailMarketingFK.pagination.plugin._paginator.plugin.connect(gridMailMarketingFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try {
                        verificaMostrarTodos(evt, gridMailMarketingFK, 'cd_mala_direta', 'selecionaTodosMailMarketingFK');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridMailMarketingFK, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodosMailMarketingFK')) && dojo.byId('selecionaTodosMailMarketingFK').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_mala_direta', 'selecionadoMailMarketingFK', -1, 'selecionaTodosMailMarketingFK', 'selecionaTodosMailMarketingFK', 'gridPesquisaMailMarketingFK')", gridMailMarketingFK.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridMailMarketingFK.startup();

                decreaseBtn(document.getElementById("pesquisarMailMarketingFK"), '32px');
                adicionarAtalhoPesquisa(['assuntoEmailMktFK', 'dtIni', 'dtFim'], 'pesquisarMailMarketingFK', ready);
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarMailMarketingFK");
                decreaseBtn(document.getElementById("pesquisarMailMarketingFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            retornarMailMarketingFK();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaMailMarketingFK");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("proMailMarketing").hide(); } }, "fecharMailMarketingFK");

                //adicionarAtalhoPesquisa(['descMailMarketingFK', 'cbPesqProdutoFK', 'cbPesqEstagiosFK', 'cbPesqModalidadesFK', 'statusMailMarketingFK'], 'pesquisarMailMarketingFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function pesquisarMailMarketingFK() {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/emailMarketing/getMalaDiretaPorAluno?cd_pessoa=" + dojo.byId("cd_pessoa_aluno_mat").value + "&assunto=" + dijit.byId("assuntoEmailMktFK").value +
                                            "&dtaIni=" + dojo.byId("dtIni").value + "&dtaFim=" + dojo.byId("dtFim").value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaMailMarketing");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function formatCheckBoxMailMarketingFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaMailMarketing';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMailMarketingFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_mala_direta", grid._by_idx[rowIndex].item.cd_mala_direta);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBoxMailMarketingFK'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_mala_direta', 'selecionadoMailMarketingFK', -1, 'selecionaTodosMailMarketingFK', 'selecionaTodosMailMarketingFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_mala_direta', 'selecionadoMailMarketingFK', " + rowIndex + ", '" + id + "', 'selecionaTodosMailMarketingFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaMailMarketingFK() {
    
    try {
        var gridPesquisaMailMarketing = dijit.byId("gridPesquisaMailMarketing");
        if (hasValue(gridPesquisaMailMarketing))
            gridPesquisaMailMarketing.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));

        dojo.byId("assuntoEmailMktFK").value = "";
        dijit.byId("dtIni").reset();
        dijit.byId("dtFim").reset();
        if (hasValue(gridPesquisaMailMarketing) && hasValue(gridPesquisaMailMarketing.itensSelecionados))
            gridPesquisaMailMarketing.itensSelecionados = [];
        
    }
    catch (e) {
        postGerarLog(e);
    }
}