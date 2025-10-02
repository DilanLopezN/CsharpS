function formatCheckBoxCircular(value, rowIndex, obj) {
    try {
        var gridName = 'gridCirculares';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCirculares');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_circular", grid._by_idx[rowIndex].item.cd_circular);

            value = value || indice != null;
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_circular', 'selecionadoCircular', -1, 'selecionaTodosCirculares', 'selecionaTodosCirculares', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_circular', 'selecionadoCircular', " + rowIndex + ", '" + id + "', 'selecionaTodosCirculares', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function montarCadastroCirculares() {

    //Cria��o da Grade de sala
    require([
        "dojo/_base/xhr",
        "dijit/registry",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/DataGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojox/data/JsonRestStore",
        "dojo/data/ObjectStore",
        "dojo/data/ItemFileReadStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dojo/dom-attr",
        "dijit/form/Button",
        "dijit/form/TextBox",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dijit/form/DateTextBox",
        "dijit/Dialog",
        "dojo/parser",
        "dojo/domReady!",
        "dijit/Tooltip",
        "dojo/aspect",
        "dojox/form/Uploader",
        "dojox/form/uploader/FileList",
        "dojox/form/FileUploader"
    ], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, ItemFileReadStore, Cache,
        Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox, Tooltip, aspect,
        Uploader, FileList, FileUploader) {

            ready(function () {
                //*** Cria a grade de Cursos **\\

                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/coordenacao/obterCircularesPorFiltros",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }), Memory({}));

                myStore.query({
                    nm_ano_circular: 0,
                    nm_mes_circular: [],
                    nm_circular: 0,
                    no_circular: "",
                    nm_menu_circular: []
                });

                var gridCirculares = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure:
                        [
                            { name: "<input id='selecionaTodosCirculares' style='display:none'/>", field: "selecionadoCircular", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCircular },
                            { name: "Nº da Circular", field: "nm_circular", width: "10%", styles: "text-align: center; min-width:80px;" },
                            { name: "Ano", field: "nm_ano_circular", width: "10%", styles: "text-align: center; min-width:80px;" },
                            { name: "Mês", field: "nm_mes_circular", width: "10%", styles: "min-width:15px;", formatter: tipoMesFormatado },
                            { name: "Nome", field: "no_circular", width: "40%", styles: "min-width:15px; max-width: 20px;" },
                            { name: "Menu", field: "nm_menu_circular", width: "10%", styles: "min-width:15px; max-width: 20px;", formatter: tipoMenuFormatado },
                            {
                                name: "Download", field: "_item", width: "7%", styles: "text-align: center; min-width:15px; max-width: 20px;",
                                formatter: function (item) {
                                    if (hasValue(dijit.byId("ci_" + item.cd_circular))) {
                                        dijit.byId("ci_" + item.cd_circular).destroy();
                                    }

                                    var btn = new dijit.form.Button({
                                        id: "ci_" + item.cd_circular,
                                        disabled: false,
                                        iconClass: 'dijitEditorIcon dijitEditorIconDownload',
                                        onClick: function () {
                                            try {
                                                downloadCircular(item.cd_circular);
                                            } catch (e) {
                                                apresentaMensagem("apresentadorMensagem", e);
                                            }
                                        }
                                    });
                                    setTimeout("alterarTamnhoBotao('" + btn.id + "')", 15);
                                    return btn;
                                }
                            }
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
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridCirculares");
                gridCirculares.startup();
                gridCirculares.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 7 };
                gridCirculares.on("RowDblClick", function (evt) {

                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }

                    var idx = evt.rowIndex;
                    var circular_selecionda = this.getItem(idx);
                    obterCircularPorID(circular_selecionda.cd_circular);

                    dijit.byId("DialogCadastroCirculares").show();
                    IncluirAlterar(0, 'divAlterarCircular', 'divIncluirCircular', 'divExcluirCircular', 'apresentadorMensagemCircular', 'divCancelarCircular', 'divClearCircular');

                    hideBtnUploader('uploaderCircularIncluir');
                    showBtnUploader('uploaderCircularEditar');

                }, true);

                /***********Criação do Botão de incluir Arquivo Circular***********/
                uploaderArquivoCircular = new dojox.form.Uploader({
                    accept: "image/jpg",
                    class: "browseButton",
                    type: "file",
                    iconClass: "dijitEditorIcon dijitEditorIconUpload",
                    label: "",
                    multiple: false,
                    uploadOnSelect: false,
                    force: 'html5',
                    name: "UploadedCircular",
                    isDebug: true,
                    url: Endereco() + "/api/coordenacao/uploadCircular?editar=false"
                }, "uploaderCircularIncluir");

                dojo.connect(uploaderArquivoCircular, "onComplete", function (callback) {

                    //Verifica se Salvou o arquivo cirular com sucesso
                    if (handlerErrorCircular(callback)) {
                        uploadCompleateIncluirCircular.someMethod(true);
                    } else {
                        uploaderArquivoCircular.reset();
                        dijit.byId("NomeArquivo").set("value", "");

                        uploadCompleateIncluirCircular.someMethod(false);
                    }
                });

                dojo.connect(uploaderArquivoCircular, "onChange", function (evt) {

                    var dataArray = uploaderArquivoCircular.getFileList();
                    dojo.byId("NomeArquivo").value = dataArray[0].name;
                });

                uploaderArquivoCircular.startup();

                /***********Criação do Botão de Editar Video***********/
                uploaderArquivoEditarCircular = new dojox.form.Uploader({
                    accept: "image/jpg",
                    class: "browseButton",
                    type: "file",
                    iconClass: "dijitEditorIcon dijitEditorIconUpload",
                    label: "",
                    multiple: false,
                    uploadOnSelect: false,
                    force: 'html5',
                    name: "UploadedCircular",
                    isDebug: true,
                    url: Endereco() + "/api/coordenacao/uploadCircular?editar=true"
                }, "uploaderCircularEditar");

                dojo.connect(uploaderArquivoEditarCircular, "onComplete", function (callback) {
                    //Verifica se Salvou o vídeo com sucesso
                    if (handlerErrorCircular(callback)) {
                        uploadCompleateCircularEdicao.someMethodEdit(true);
                    } else {
                        uploaderArquivoEditarCircular.reset();
                        dijit.byId("NomeArquivo").set("value", "");

                        uploadCompleateCircularEdicao.someMethodEdit(false);
                    }
                });

                dojo.connect(uploaderArquivoEditarCircular, "onChange", function (evt) {

                    var dataArray = uploaderArquivoEditarCircular.getFileList();
                    dojo.byId("NomeArquivo").value = dataArray[0].name;
                });

                uploaderArquivoEditarCircular.startup();

                //CONFIGURAR TAMANHO DO BOTÃO.
                decreaseBtn(document.getElementById("uploaderCircularIncluir"), '18px');
                decreaseBtn(document.getElementById("uploaderCircularEditar"), '18px');

                //-----------------------------------------------------------------------

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Salvar",
                    iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                    onClick: function () {
                        var isSave = true;
                        salvarArquivoCircular();
                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(uploadCompleateIncluirCircular, "someMethod", function (statusUpload) {
                                if (statusUpload) {
                                    salvarCircular();
                                }
                            });

                        });
                    }

                }, "incluirCircular");

                new Button({
                    label: "Cancelar",
                    iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        obterCircularPorID(dom.byId("cd_circular").value);
                        uploaderArquivoCircular.reset();
                        uploaderArquivoEditarCircular.reset();
                        apresentaMensagem('apresentadorMensagemCircular', null);
                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "cancelarCircular");

                new Button({
                    label: "Fechar",
                    iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        limparCamposCircular();
                        dijit.byId("DialogCadastroCirculares").hide();
                    }
                }, "fecharCircular");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        salvarArquivoCircularEdicao("editar");
                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(uploadCompleateCircularEdicao, "someMethodEdit", function (statusUpload) {
                                if (statusUpload) {
                                    editarCircular();
                                }
                            });

                        });
                    }
                }, "alterarCircular");


                new Button({
                    label: "Excluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {

                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }

                            deletarCircular();
                        });
                    }
                }, "deleteCircular");

                new Button({
                    label: "Limpar",
                    iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        limparCamposCircular();
                    }
                }, "limparCircular");
                //Fim

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisarCircular(true);
                    }
                }, "pesquisarCircular");
                decreaseBtn(document.getElementById("pesquisarCircular"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {

                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        limparCamposCircular();
                        IncluirAlterar(1, 'divAlterarCircular', 'divIncluirCircular', 'divExcluirCircular', 'apresentadorMensagemCircular', 'divCancelarCircular', 'divClearCircular');

                        require(["dojo/dom-style"], function (domStyle) {
                            dijit.byId("DialogCadastroCirculares").show();
                        });

                        setAnoMesAtual();
                        hideBtnUploader('uploaderCircularEditar');
                        showBtnUploader('uploaderCircularIncluir');
                    }
                }, "novaCircular");

                // Adiciona link de a��es:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {

                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        hideBtnUploader('uploaderCircularIncluir');
                        showBtnUploader('uploaderCircularEditar');
                        eventoEditarCircular(dijit.byId('gridCirculares').itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        eventoRemover(dijit.byId('gridCirculares').itensSelecionados, 'deletarCircular(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });

                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridCirculares, 'todosItensCirculares', ['pesquisarCircular']); pesquisarCircular(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridCirculares', 'selecionadoCircular', 'cd_circular', 'selecionaTodosCirculares', ['pesquisarCircular'], 'todosItensCirculares'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                //*** Carrega os campos de pesquisa **\\

                loadMes('cbMes', ItemFileReadStore);
                loadMenu('cbMenu', ItemFileReadStore);
                loadMenuCad('cbMenuCad', Memory);
                loadMesCad('cbMesCad', Memory);
            })
        });
}

function loadMes(idPeriodo, ItemFileReadStore) {
    var dados = [
        { name: "Janeiro", id: 1 },
        { name: "Fevereiro", id: 2 },
        { name: "Março", id: 3 },
        { name: "Abril", id: 4 },
        { name: "Maio", id: 5 },
        { name: "Junho", id: 6 },
        { name: "Julho", id: 7 },
        { name: "Agosto", id: 8 },
        { name: "Setembro", id: 9 },
        { name: "Outubro", id: 10 },
        { name: "Novembro", id: 11 },
        { name: "Dezembro", id: 12 }
    ]
    var stateStore = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    dijit.byId(idPeriodo).setStore(stateStore, []);
    setMenssageMultiSelect(MES, idPeriodo);
    dijit.byId(idPeriodo).on("change", function (e) {
        setMenssageMultiSelect(MES, idPeriodo);
    });
}
function loadMenu(idPeriodo, ItemFileReadStore) {
    var dados = [
        { name: "Secretaria", id: 1 },
        { name: "Coordenação", id: 2 },
        { name: "Financeiro", id: 3 },
        { name: "Gestão", id: 4 },
        { name: "ECommerce", id: 5 },
        { name: "Configurações", id: 6 },
        { name: "Cadastros Básicos", id: 7 },
        { name: "Informativos", id: 8 },
        { name: "Portal do Professor", id: 9 },
        { name: "Portal do Aluno", id: 10 }
    ]
    var stateStore = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    dijit.byId(idPeriodo).setStore(stateStore, []);
    setMenssageMultiSelect(MENUS, idPeriodo);
    dijit.byId(idPeriodo).on("change", function (e) {
        setMenssageMultiSelect(MENUS, idPeriodo);
    });
}
function loadMesCad(idMes, Memory) {
    var stateStore = new Memory({
        data: [
            { name: "Janeiro", id: 1 },
            { name: "Fevereiro", id: 2 },
            { name: "Março", id: 3 },
            { name: "Abril", id: 4 },
            { name: "Maio", id: 5 },
            { name: "Junho", id: 6 },
            { name: "Julho", id: 7 },
            { name: "Agosto", id: 8 },
            { name: "Setembro", id: 9 },
            { name: "Outubro", id: 10 },
            { name: "Novembro", id: 11 },
            { name: "Dezembro", id: 12 }
        ]
    });
    dijit.byId(idMes).store = stateStore;
}
function loadMenuCad(idSexo, Memory) {
    var stateStore = new Memory({
        data: [
            { name: "Secretaria", id: 1 },
            { name: "Coordenação", id: 2 },
            { name: "Financeiro", id: 3 },
            { name: "Gestão", id: 4 },
            { name: "ECommerce", id: 5 },
            { name: "Configurações", id: 6 },
            { name: "Cadastros Básicos", id: 7 },
            { name: "Informativos", id: 8 },
	        { name: "Portal do Professor", id: 9 },
	        { name: "Portal do Aluno", id: 10 }
        ]
    });
    dijit.byId(idSexo).store = stateStore;
}
function incluirTipo(grid) {
    var datains =
        { no_tipo: 'Descrição', no_relatorio: 'Relatório', id_prev_dias: false, id_valor_hora: false, id_aditamento: false, id_tipo_pgto: false, id_ativo: false };
    grid.store.newItem(datains);
    grid.store.save();

}

function alterarTamnhoBotaoDwCircular(id) {
    decreaseBtn(document.getElementById(id), '18px');
}

function limparCamposCircular() {
    dojo.byId("cd_circular").value = 0;
    dijit.byId("anoCircular").reset();
    dijit.byId("cbMesCad").reset();
    dijit.byId("circularNro").reset();
    dijit.byId("circularNome").reset();
    dijit.byId("cbMenuCad").reset();

    dijit.byId("NomeArquivo").set("value", "");
    apresentaMensagem("apresentadorMensagemCircular", null);
    uploaderArquivoCircular.reset();
}

function salvarArquivoCircular() {
    if (!dijit.byId("formCiruclar").validate())
        return false;

    var mensagensWeb = new Array();
    var dataArray = uploaderArquivoCircular.getFileList();
    showCarregando();

    apresentaMensagem("apresentadorMensagemCircular", mensagensWeb, false);
    if (hasValue(dataArray) && dataArray.length > 0) {

        uploaderArquivoCircular.submit();
    } else {
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "");
        apresentaMensagem("apresentadorMensagemCircular", mensagensWeb, true);
        dijit.byId("NomeArquivo").set("value", "");
    }
    hideCarregando();
}

function salvarArquivoCircularEdicao(operacao) {

    if (!dijit.byId("formCiruclar").validate())
        return false;

    var mensagensWeb = new Array();
    var dataArray = uploaderArquivoEditarCircular.getFileList();

    if (hasValue(operacao) && operacao === "editar" && !hasValue(dataArray)) {
        uploadCompleateCircularEdicao.someMethodEdit(true); //Editar sem alterar o arquivo
    } else {
        apresentaMensagem("apresentadorMensagemCircular", mensagensWeb, false);
        if (hasValue(dataArray) && dataArray.length > 0) {
            uploaderArquivoEditarCircular.submit();
        } else {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Arquivo inválido");
            apresentaMensagem("apresentadorMensagemCircular", mensagensWeb, true);
            dijit.byId("NomeArquivo").set("value", "");
        }

    }
}

var uploadCompleateIncluirCircular = {
    someMethod: function (arg1) {
        return arg1;
    }
};

var uploadCompleateCircularEdicao = {
    someMethodEdit: function (arg1) {
        return arg1;
    }
};

function pesquisarCircular(limparItens) {

    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Cache, Memory) {
        try {

            var gridCirculares = dijit.byId("gridCirculares");
            var content = {};

            content = {
                nm_ano_circular: dijit.byId("nm_ano").value,
                nm_mes_circular: dijit.byId("cbMes").value,
                nm_circular: dijit.byId("numero").value,
                no_circular: dijit.byId("nome").value,
                nm_menu_circular: dijit.byId("cbMenu").value
            };

            showCarregando();
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCircularesPorFiltros",
                preventCache: true,
                content: content,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataCircular) {

                if (limparItens) {
                    gridCirculares.itensSelecionados = [];
                }
                gridCirculares.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: dataCircular }) }));
                hideCarregando();
            }, function (e) {
                hideCarregando();
                postGerarLog(e);
            });
        } catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function downloadCircular(cd_circular) {
    var parametrosUrl = {
        codigo_circular: "cd_circular=" + cd_circular
    };
    try {
        window.open(Endereco() + "/Informacao/getDownloadCircular/?" + parametrosUrl.codigo_circular);
    } catch (e) {
        postGerarLog(e);
    }
}

function handlerErrorCircular(callback) {
    if (!callback.retorno) {
        var mensagensWeb = new Array();
        apresentaMensagem("apresentadorMensagemCircular", callback, true);
    }
    return callback.retorno;
}

function alterarTamnhoBotao(id) {
    decreaseBtn(document.getElementById(id), '20px');
}

function setAnoMesAtual() {
    var date = new Date();

    dijit.byId("anoCircular").set("value", date.getFullYear());
    dijit.byId("cbMesCad").set("value", date.getMonth() + 1);
}

function salvarCircular() {
    try {
        if (!dijit.byId("formCiruclar").validate())
            return false;

        var circular = {
            nm_ano_circular: dijit.byId("anoCircular").value,
            nm_mes_circular: dijit.byId("cbMesCad").value,
            nm_circular: dijit.byId("circularNro").value,
            no_circular: dijit.byId("circularNome").value,
            nm_menu_circular: dijit.byId("cbMenuCad").value,
            no_arquivo_circular: dojo.byId("NomeArquivo").value
        };

        showCarregando();
        require([
            "dojo/_base/xhr"
        ], function (xhr) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/adicionarCircular",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify(circular)
            }).then(function (data) {
                var circulares = JSON.parse(data);

                if (!hasValue(circulares.erro)) {
                    circulares = circulares.retorno;

                    var gridCirculares = dijit.byId("gridCirculares");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("DialogCadastroCirculares").hide();

                    if (!hasValue(gridCirculares.itensSelecionados)) {
                        gridCirculares.itensSelecionados = [];
                    }
                    insertObjSort(gridCirculares.itensSelecionados, "cd_circular", circulares);
                    buscarItensSelecionados('gridCirculares', 'selecionadoCircular', 'cd_circular', 'selecionaTodosCirculares', ['pesquisarCircular'], 'todosItensCirculares');

                    setGridPagination(gridCirculares, circulares, "cd_circular");
                    hideCarregando();
                } else {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemCircular', data);
                }

            }, function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    } catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function editarCircular() {

    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {

            if (!dijit.byId("formCiruclar").validate())
                return false;


            var circular = {
                cd_circular: dom.byId("cd_circular").value,
                nm_ano_circular: dijit.byId("anoCircular").value,
                nm_mes_circular: dijit.byId("cbMesCad").value,
                nm_circular: dijit.byId("circularNro").value,
                no_circular: dijit.byId("circularNome").value,
                nm_menu_circular: dijit.byId("cbMenuCad").value,
                no_arquivo_circular: dojo.byId("NomeArquivo").value
            };

            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/editarCircular",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(circular)
            }).then(function (data) {

                var circular = JSON.parse(data);
                circular = JSON.parse(circular);
                if (!hasValue(circular.erro)) {
                    circular = circular.retorno;

                    var gridCirculares = dijit.byId("gridCirculares");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("DialogCadastroCirculares").hide();

                    if (!hasValue(gridCirculares.itensSelecionados)) {
                        gridCirculares.itensSelecionados = [];
                    }
                    removeObjSort(gridCirculares.itensSelecionados, "cd_circular", dom.byId("cd_circular").value);
                    insertObjSort(gridCirculares.itensSelecionados, "cd_circular", circular);
                    buscarItensSelecionados('gridCirculares', 'selecionadoCircular', 'cd_circular', 'selecionaTodosCirculares', ['pesquisarCircular'], 'todosItensCirculares');
                    setGridPagination(gridCirculares, circular, "cd_circular");
                    hideCarregando();
                } else {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemCircular', data);
                }
            }, function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });

        } catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function deletarCircular(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            var cd_circular = 0;
            if (!hasValue(itensSelecionados) || itensSelecionados.length == 0) {
                if (dojo.byId('cd_circular').value > 0)
                    itensSelecionados = [{ cd_circular: dom.byId("cd_circular").value }];
            }

            showCarregando();
            xhr.post({
                url: Endereco() + "/api/coordenacao/deletarCircular",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                var circular = JSON.parse(data);

                if (!hasValue(circular.erro)) {
                    var todos = dojo.byId("todosItensCirculares");
                    apresentaMensagem('apresentadorMensagem', data);

                    dijit.byId('DialogCadastroCirculares').hide();

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(itensSelecionados, "cd_circular", itensSelecionados[r].cd_circular);

                    pesquisarCircular(true);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCircular").set('disabled', false);
                    //dijit.byId("relatorioVideo").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    hideCarregando();
                }
                else {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemCircular', data);
                }
            }, function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagemCircular', error);
            });
        }
        catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function obterCircularPorID(cd_circular) {
    require(["dojo/_base/xhr"], function (xhr) {
        try {
            showCarregando();
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterCircularPorID?cd_circular=" + cd_circular,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                var circular = jQuery.parseJSON(data);
                if (!hasValue(circular.erro)) {
                    preencheInputCircular(circular.retorno);
                }
                hideCarregando();
            });
        } catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function preencheInputCircular(circular) {
    if (hasValue(circular)) {
        dojo.byId("cd_circular").value = circular.cd_circular;
        dijit.byId("anoCircular").set("value", circular.nm_ano_circular);
        dijit.byId("cbMesCad").set("value", circular.nm_mes_circular);
        dijit.byId("circularNro").set("value", circular.nm_circular);
        dijit.byId("circularNome").set("value", circular.no_circular);
        dijit.byId("cbMenuCad").set("value", circular.nm_menu_circular);
        dijit.byId("NomeArquivo").set("value", circular.no_arquivo_circular);
    }
}

function eventoEditarCircular(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            limparCamposCircular();
            apresentaMensagem('apresentadorMensagem', '');
            var circular = itensSelecionados[0];

            obterCircularPorID(circular.cd_circular);
            dijit.byId("DialogCadastroCirculares").show();

            IncluirAlterar(0, 'divAlterarCircular', 'divIncluirCircular', 'divExcluirCircular', 'apresentadorMensagemCircular', 'divCancelarCircular', 'divClearCircular');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function hideBtnUploader(botao) {
    dojo.style(dijit.byId(botao).domNode, { visibility: 'hidden', display: 'none' });
}

function showBtnUploader(botao) {
    dojo.style(dijit.byId(botao).domNode, { visibility: 'visible', display: 'inline-table' });
}