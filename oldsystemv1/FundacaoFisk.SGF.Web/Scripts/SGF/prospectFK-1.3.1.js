var PESQUISARPROSPECT = 1, PESQUISARALUNO = 2;
var ORIGCADALUNO = 1, ORIGPESQFOLLOWUP = 2, ORIGCADFOLLOWUP = 3;

//#region criando grade da fk - montarGridPesquisaProspect() - formatCheckBoxProspectFK
function formatCheckBoxProspectFK(value, rowIndex, obj) {
    try{
        var gridName = 'gridPesquisaProspect';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosProspectFK');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_prospect", grid._by_idx[rowIndex].item.cd_prospect);

                value = value || indice != null; // Item está selecionado.
            }
            if (rowIndex != -1)
                icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_prospect', 'selecionadoProspectFK', -1, 'selecionaTodosProspectFK', 'selecionaTodosProspectFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_prospect', 'selecionadoProspectFK', " + rowIndex + ", '" + id + "', 'selecionaTodosProspectFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarGridPesquisaProspect(especializada, funcao, filtro_especial) {
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
                if (filtro_especial)
                    loadTipoFKProspect(on);
                var myStore = null;
                var store = null
                if (!especializada) {
                    myStore = Cache(
                      JsonRest({
                          target: Endereco() + "/api/secretaria/getProspectAlunoFKSearch?nome=&inicio=false&email=&telefone=&tipoPesquisa=" + PESQUISARPROSPECT + "&vinculoFollowUp=false",
                          handleAs: "json",
                          headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                      }
                  ), Memory({}));
                    store = new ObjectStore({ objectStore: myStore });
                } else
                    store = new ObjectStore({ objectStore: new Memory({ data: null }) });
            
                //*** Cria a grade de Prospect **\\
                var gridPesquisaProspect = new EnhancedGrid({
                    store: store,
                    structure:
                            [
                                { name: "<input id='selecionaTodosProspectFK' style='display:none'/>", field: "selecionadoProspectFK", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProspectFK },
                                { name: "Nome", field: "no_pessoa", width: "31%", styles: "min-width:80px;" },
                                { name: "Telefone", field: "telefone", width: "17%", styles: "min-width:80px;" },
                                { name: "Celular", field: "celular", width: "17%", styles: "min-width:80px;" },
                                { name: "E-mail", field: "email", width: "30%", styles: "min-width:80px;" },
                            ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["15", "30", "45", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "15",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button",
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridPesquisaProspect");

                gridPesquisaProspect.canSort = function (col) { return Math.abs(col) == 2 };
                gridPesquisaProspect.pagination.plugin._paginator.plugin.connect(gridPesquisaProspect.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaProspect, 'cd_prospect', 'selecionaTodosProspectFK');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPesquisaProspect, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosProspectFK').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_prospect', 'selecionadoProspectFK', -1, 'selecionaTodosProspectFK', 'selecionaTodosProspectFK', 'gridPesquisaProspect')", gridPesquisaProspect.rowsPerPage * 3);
                    });
                });
                gridPesquisaProspect.startup();

                new Button({
                    label: "",
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        if (!especializada)
                            pesquisarProspectFK();
                        else {
                            var tipoOrigem = dojo.byId("idOrigemProspectFK").value;
                            if (hasValue(tipoOrigem))
                                switch (parseInt(tipoOrigem)) {
                                    case ORIGCADALUNO:
                                        pesquisarProspectFK();
                                        break;
                                    case ORIGPESQFOLLOWUP:
                                        pesquisarProspectFKOrigemFollowUp(true);
                                        break;
                                    case ORIGCADFOLLOWUP:
                                        pesquisarProspectFKOrigemFollowUp(false);
                                        break;
                                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                                        return false;
                                        break;
                                }
                        }

                    },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "prospectSearchFK");
                decreaseBtn(document.getElementById("prospectSearchFK"), '32px');
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        var tipoOrigem = dojo.byId("idOrigemProspectFK").value;
                        if (hasValue(tipoOrigem))
                            switch (parseInt(tipoOrigem)) {
                                case ORIGCADALUNO:
                                    retornarProspectFKOrigemAluno();
                                    break;
                                case ORIGPESQFOLLOWUP:
                                    retornarProspectFKOrigemFollowUp();
                                    break;
                                case ORIGCADFOLLOWUP:
                                    retornarProspectFKFollowUpPartial();
                                    break;
                                default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                                    return false;
                                    break;
                            }
                    }
                }, "selecionaProspectFK");
            
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { }
                }, "fecharProspectFK");
                adicionarAtalhoPesquisa(['nomeProspectFK', 'emailPesqProspFK', 'inicioProspectFK', 'telefoneProspectFK'], 'prospectSearchFK', ready);
                if(hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}
//#endregion

function pesquisarProspectFK() {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/secretaria/getProspectAlunoFKSearch?nome=" + dojo.byId('nomeProspectFK').value + "&inicio=" + dijit.byId("inicioProspectFK").checked + "&email="
                                   + dojo.byId('emailPesqProspFK').value + "&telefone=" + dojo.byId('telefoneProspectFK').value + "&tipoPesquisa=" + PESQUISARPROSPECT + "&vinculoFollowUp=false",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_prospect"
            }), Memory({ idProperty: "cd_prospect" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridProspect = dijit.byId("gridPesquisaProspect");
            gridProspect.itensSelecionados = [];
            gridProspect.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadTipoFKProspect(on) {
    try {
        var tipoPesquisaFKProspect = dijit.byId("tipoPesquisaFKProspect");
        var storeTipo = new dojo.store.Memory({
            data: [
              { name: "Prospect", id: "1" },
              { name: "Aluno", id: "2" }
            ]
        });
        tipoPesquisaFKProspect.store = storeTipo;
        tipoPesquisaFKProspect.set("value", PESQUISARPROSPECT);
        tipoPesquisaFKProspect.on("change", function (e) {
            if (!hasValue(e))
                tipoPesquisaFKProspect.set("value", PROSPECT);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparPesquisaProspectFK() {
    try{
        dijit.byId('nomeProspectFK').set('value', '');
        dijit.byId('emailPesqProspFK').set('value', '');
        dijit.byId('telefoneProspectFK').set('value', '');
        dijit.byId('inicioProspectFK').set('checked', false);
        if (hasValue(dijit.byId("gridPesquisaProspect"))  && hasValue(dijit.byId("gridPesquisaProspect").itensSelecionados))
            dijit.byId("gridPesquisaProspect").itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}