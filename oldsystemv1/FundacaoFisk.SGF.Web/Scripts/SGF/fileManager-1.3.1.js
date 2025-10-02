
function formatCheckBoxArquivo(value, rowIndex, obj) {
    var gridName = 'gridArquivo';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodos');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_arquivo", grid._by_idx[rowIndex].item.cd_arquivo);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

    // Configura o check de todos, para quando mudar de aba:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_arquivo', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_arquivo', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

    return icon;
}

function montarCadastroArquivo() {
    //Criação da Grade
    require([
        "dojo/_base/xhr",
        "dijit/registry",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/DataGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojox/data/JsonRestStore",
        "dojo/data/ObjectStore",
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
        "dojo/domReady!"
    ], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox) {
        ready(function () {

            //*** Cria a grade de Arquivo **\\
            var gridArquivo = new EnhancedGrid({
                //store: ObjectStore({ objectStore: myStore }),
                store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                structure:
                  [
                    { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxArquivo },
                    { name: "Nome", field: "no_arquivo", width: "40%", styles: "min-width:80px;" },
                    { name: "Tamanho", field: "tamanho", width: "20%", styles: "min-width:80px;" },
                    { name: "Tipo", field: "no_tipo", width: "15%", styles: "min-width:80px;" },
                    { name: "Última Modificação", field: "dt_ultima_modificacao", width: "20%", styles: "min-width:80px;" },
                    { name: "RO", field: "somente_leitura", width: "5%", styles: "min-width:80px;" }
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
                        maxPageStep: 5,
                        position: "button",
                        plugins: { nestedSorting: true }
                    }
                }
            }, "gridArquivo");
            gridArquivo.canSort = function (col) { return Math.abs(col) != 1 };
            gridArquivo.startup();

            gridArquivo.on("RowDblClick", function (evt) {
                apresentaMensagem('apresentadorMensagem', '');

                var idx = evt.rowIndex,
                item = this.getItem(idx),
                store = this.store;
                var valor = item.no_caminho;
                if (item.no_tipo == "Diretório") {
                    dijit.byId('caminho').set('value', valor);
                    pesquisarArquivo(true, valor);
                }
                else {
                    var mensagensWeb = new Array();
                    apresentaMensagem('apresentadorMensagem', null);
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNotDiretorio);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                }

            }, true);

            dojo.query("#caminho").on("keyup", function (e) {
                if (e.keyCode == 13) {
                    apresentaMensagem('apresentadorMensagem', '');
                    var valor = dojo.byId('caminho').value;
                    pesquisarArquivo(true, valor);
                }
            });


            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridArquivo, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodos').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_arquivo', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridArquivo')", gridArquivo.rowsPerPage * 3);
                });
            });

            if (!hasValue(gridArquivo.itensSelecionados))
                gridArquivo.itensSelecionados = [];

            //*** Cria os botões do link de ações **\\
            // Adiciona link de selecionados Curso:
            var menu = new DropDownMenu({ style: "height: 25px" });

            var menuItensDownload = new MenuItem({
                label: "Download",
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    var gridArquivo = dijit.byId("gridArquivo");
                    var itensSelecionados = gridArquivo.itensSelecionados;
                    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    else if (itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                    else {
                        if (hasValue(itensSelecionados[0].no_tipo) && itensSelecionados[0].no_tipo == "Diretório") {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não é possível baixar um diretório.");
                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                            return false;
                        }
                        if (hasValue(itensSelecionados[0].no_caminho)) {
                            var url = Endereco() + "/filemanager/getArquivoDiretorio?urlPath=" + itensSelecionados[0].no_caminho + "&senha=" + dojo.byId('senha').value + "&no_arquivo=" +
                                itensSelecionados[0].no_arquivo;
                            window.open(url);
                        } else {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "Não existe caminho para p arquivo" + itensSelecionados[0].no_arquivo);
                            apresentaMensagem('apresentadorMensagem', mensagensWeb);
                        }
                    }
                }
            });
            menu.addChild(menuItensDownload);

            var menuItensUpload = new MenuItem({
                label: "Upload",
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    var gridArquivo = dijit.byId("gridArquivo");
                    var itensSelecionados = gridArquivo.itensSelecionados;
                    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    else if (itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                    else {

                        dijit.byId("uploader").onClick();
                        $("#uploader").click();
                    };
                }
            });
            menu.addChild(menuItensUpload);

			var menuItensUpload = new MenuItem({
                label: "Upload",
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    var gridArquivo = dijit.byId("gridArquivo");
                    var itensSelecionados = gridArquivo.itensSelecionados;
                    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    else if (itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                    else {

                        dijit.byId("uploader").onClick();
                        $("#uploader").click();
                    };
                }
            });
            menu.addChild(menuItensUpload);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasCurso",
                dropDown: menu,
                id: "acoesRelacionadasCurso"
            });
            dom.byId("linkAcoesCurso").appendChild(button.domNode);

            new Button({
                label: "",
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    var valor = dojo.byId('caminho').value;
                    pesquisarArquivo(true, valor);
                },
                iconClass: 'dijitEditorIconSearchSGF'
            }, "pesquisarArquivo");
            decreaseBtn(document.getElementById("pesquisarArquivo"), '32px');

            new Button({
                label: "",
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    var url = Endereco() + "/filemanager/getArquivoDiretorio?urlPath=" + dojo.byId('arquivo').value + "&senha=" + dojo.byId('senha').value + "&no_arquivo=arquivo.txt";
                    window.open(url);
                },
                iconClass: 'dijitEditorIcon dijitEditorIconDownload'
            }, "downloadArquivo");
            
            var buttonFkArray = ['downloadArquivo'];

            for (var p = 0; p < buttonFkArray.length; p++) {
                var buttonFk = document.getElementById(buttonFkArray[p]);

                if (hasValue(buttonFk)) {
                    buttonFk.parentNode.style.minWidth = '18px';
                    buttonFk.parentNode.style.width = '18px';
                }
            }

            new Button({
                label: "",
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarMatDid(true); },
                iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
            }, "pesquisarMat");
            $("#uploader").change(function (e) {
                var mensagensWeb = new Array();
                if (e.target.files[0].name.length > 128) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Nome do arquivo e muito grande, favor escolher um com apenas 128 caracateres.");
                    apresentaMensagem('apresentadorMensagem', mensagensWeb);
                } else {
                    if (window.FormData !== undefined) {
                        var data = new FormData();
                        data.urlArquivo = dijit.byId("gridArquivo").itensSelecionados[0].no_caminho;
                        for (i = 0; i < e.target.files.length; i++) {
                            data.append("file" + i, e.target.files[i]);
                        }
                        $.ajax({
                            type: "POST",
                            url: Endereco() + "/filemanager/UploadArquivo",
                            ansy: false,
                            headers: { Authorization: Token() },
                            contentType: false,
                            processData: false,
                            data: data,
                            success: function (results) {
                                alert("Sucesso");
                            },
                            error: function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            }
                        });
                    } else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                        apresentaMensagem('apresentadorMensagem', mensagensWeb);
                    }
                }
            });
            xhr.get({
                preventCache: true,
                url: Endereco() + "/filemanager/getArquivosSearch?caminho=" + dojo.byId('caminho').value + "&senha=" + dojo.byId('senha').value,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                apresentaMensagem('apresentadorMensagem', data);
                data = data.retorno;
                gridArquivo.setStore(new ObjectStore({ objectStore: new Memory({ data: data }) }));
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
            showCarregando();
        });
    });
}

function atualizaGrid(gridName) {
    dijit.byId(gridName).update();
}

function pesquisarArquivo(limparItens, valor) {
    
    if (!hasValue(valor)) {
        alert(this);
        return false;
    }

    require([
          "dojo/_base/xhr",
          "dijit/registry",
          "dojox/grid/EnhancedGrid",
          "dojox/grid/enhanced/plugins/Pagination",
          "dojo/store/JsonRest",
          "dojox/data/JsonRestStore",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory",
          "dijit/form/TextBox",
          "dojo/domReady!",
          "dojo/parser"
    ], function (xhr, _registry, EnhancedGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, TextBox) {
        xhr.get({
            preventCache: true,
            url: Endereco() + "/filemanager/getArquivosSearch?caminho=" + dojo.byId('caminho').value + "&senha=" + dojo.byId('senha').value,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            apresentaMensagem('apresentadorMensagem', data);
            data = data.retorno;

            var dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) });
            var grid = dijit.byId("gridArquivo");

            if (limparItens)
                grid.itensSelecionados = [];

            grid.setStore(dataStore);
        });
    });
}

function getSession() {
    dojo.xhr.get({
        url: Endereco() + "/FileManager/getMostrarCacheAplicacao",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var mostrar = false;
            if (mostrar)
                alert(data);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemFunc', error);
    });
}

