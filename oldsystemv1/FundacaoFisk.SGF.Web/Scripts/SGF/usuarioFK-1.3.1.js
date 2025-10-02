var PESQUSERORIGFOLLOWUP = 1, PESQUSERDESTINOFOLLOWUP = 2, PESQUSERORIGMASTERFOLLOWUP = 3, CADUSERDESTINOFOLLOWUPFK = 4, CADUSERORIGFOLLOWUPFK = 5, CADUSERORIGMASTERFOLLOWUPFK = 6, CADGRUPO = 7, FECHCAIXA = 8, RELMAT = 9, CADPROSPECTALUNOUSERDESTINOFOLLOWUPFK = 10;

function montarGridPesquisaUsuarioFK(funcao) {
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
                apresentaMensagem(dojo.byId("apresentadorMensagemProUsuario").value, null);
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/Escola/GetUsuarioSearch?descricao=&nome=&inicio=false&status=1&escola=0&pesqSysAdmin=false&pesqFK=true",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }), Memory({}));

                var gridPesquisaUsuarioFK = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoUsuarioFK", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxUsuarioFK },
                        { name: "Usuário", field: "no_login", width: "25%" },
                        { name: "Nome", field: "no_pessoa", width: "55%", styles: "min-width:10%; max-width: 30%;" },
                        { name: "Administrador", field: "Master", width: "15%", styles: "text-align: center; min-width:80px; max-width: 80px;" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
                    plugins: {
                        pagination: {
                            pageSizes: ["14", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "14",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridPesquisaUsuarioFK"); // make sure you have a target HTML element with this id
                gridPesquisaUsuarioFK.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 4; };
                //grid.canSort = function (col) { return true; };

                gridPesquisaUsuarioFK.startup();
                gridPesquisaUsuarioFK.on("RowDblClick", function (evt) {
                }, true);
                gridPesquisaUsuarioFK.pagination.plugin._paginator.plugin.connect(gridPesquisaUsuarioFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaUsuarioFK, 'no_login', 'selecionaTodos');
                });

                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            var tipoOrigem = dojo.byId("idOrigemUsuarioFK").value;
                            if (hasValue(tipoOrigem))
                                switch (parseInt(tipoOrigem)) {
                                    case CADGRUPO:
                                        retornarUsuarioFK();
                                        break;
                                    case PESQUSERORIGFOLLOWUP:
                                    case PESQUSERDESTINOFOLLOWUP:
                                    case PESQUSERORIGMASTERFOLLOWUP:
                                        retornarUsuarioFKPesqFollowUp();
                                        break;
                                    case CADUSERORIGFOLLOWUPFK:
                                    case CADUSERDESTINOFOLLOWUPFK:
                                    case CADPROSPECTALUNOUSERDESTINOFOLLOWUPFK:
                                    case CADUSERORIGMASTERFOLLOWUPFK:
                                        retornarUsuarioFKFollowUpPartial();
                                        break;
                                    case FECHCAIXA:
                                        retornarUsuarioFKFechamento();
                                        break;
                                    case RELMAT:
                                        retornarUsuarioFKRelMat();
                                        break;
                                    default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                                        return false;
                                        break;
                                }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaUsuarioFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("proUsuario").hide(); }
                }, "fecharUsuarioFK");
                dijit.byId("pesquisarUsuarioFK").on("click", function () {
                    try {
                        var tipo = parseInt(dojo.byId("idOrigemUsuarioFK").value);
                        switch (tipo) {
                            case PESQUSERORIGFOLLOWUP:
                            case PESQUSERDESTINOFOLLOWUP:
                            case CADPROSPECTALUNOUSERDESTINOFOLLOWUPFK:
                            case  CADUSERDESTINOFOLLOWUPFK:
                                pesquisarUsuarioFKFollowUpPesq(dojo.byId("idOrigemUsuarioFK").value);
                                break;
                            case CADGRUPO:
                            case PESQUSERORIGMASTERFOLLOWUP:
                            case CADUSERORIGFOLLOWUPFK:
                            case CADUSERORIGMASTERFOLLOWUPFK:                            
                                apresentaMensagem("apresentadorMensagemProUsuario", null);
                                pesquisarUsuarioFK();
                                break;
                            case FECHCAIXA:
                                apresentaMensagem("apresentadorMensagemProUsuario", null);
                                pesquisarUsuarioFKFechamentoCaixa();
                                break;
                            case RELMAT:
                                apresentaMensagem("apresentadorMensagemProUsuario", null);
                                pesquisarUsuarioFKGeral();
                                break;
                                
                            default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi informado/encontrado.");
                                return false;
                                break;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                decreaseBtn(document.getElementById("pesquisarUsuarioFK"), '32px');
                adicionarAtalhoPesquisa(['pesquisatextFK', 'nomPessoaFK'], 'pesquisarUsuarioFK', ready);
                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxUsuarioFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaUsuarioFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosUsuarioFK');

        if (hasValue(gridPesquisaUsuarioFK.itensSelecionados && hasValue(gridPesquisaUsuarioFK._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(gridPesquisaUsuarioFK.itensSelecionados, "cd_usuario", gridPesquisaUsuarioFK._by_idx[rowIndex].item.cd_usuario);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_usuario', 'selecionadoUsuarioFK', -1, 'selecionaTodosUsuarioFK', 'selecionaTodosUsuarioFK', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_usuario', 'selecionadoUsuarioFK', " + rowIndex + ", '" + id + "', 'selecionaTodosUsuarioFK', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarUsuarioFK() {
    require([
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/domReady!",
    "dojo/parser"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            myStore = Cache(
            JsonRest({
                target: Endereco() + "/Escola/GetUsuarioSearchFK?descricao=" + encodeURIComponent(document.getElementById("pesquisatextFK").value) + "&nome=" + encodeURIComponent(document.getElementById("nomPessoaFK").value) + "&inicio=" + document.getElementById("inicioUsuarioFK").checked,
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridUsuario = dijit.byId("gridPesquisaUsuarioFK");
            gridUsuario.setStore(dataStore);
            gridUsuario.itensSelecionados = [];

            gridUsuario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaUsuarioFK() {
    try {
        dojo.byId("pesquisatextFK").value = "";
        dojo.byId("nomPessoaFK").value = "";
        dijit.byId("inicioUsuarioFK").set("checked", false);
        if (hasValue(dijit.byId("gridPesquisaUsuarioFK"))) {
            dijit.byId("gridPesquisaUsuarioFK").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaUsuarioFK").itensSelecionados))
                dijit.byId("gridPesquisaUsuarioFK").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}