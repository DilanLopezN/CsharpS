
function formatBtnPlus(value, rowIndex, obj) {
    try {
        return '';
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxFaq(value, rowIndex, obj) {
    try {
        var gridName = 'gridFaqs';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFaqs');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_faq", grid._by_idx[rowIndex].item.cd_faq);

            value = value || indice != null; // Item est� selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_faq', 'selecionadoFaq', -1, 'selecionaTodosFaqs', 'selecionaTodosFaqs', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_faq', 'selecionadoFaq', " + rowIndex + ", '" + id + "', 'selecionaTodosFaqs', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

var uploadCompleate = {
    someMethod: function (arg1) {
        return arg1;
    }
};

var uploadCompleateEdicao = {
    someMethodEdit: function (arg1) {
        return arg1;
    }
};


function verificarExtensaoArquivo(extensao) {
    try {
        var valido = true;
        var achou = false;
        var TamanhoString = extensao.length;
        var ext = new Array('video/mp4', 'video/ogg', 'video/webm');
        for (var i = 0; i < ext.length; i++)
            if (extensao.toLowerCase() == ext[i]) {
                achou = true;
                break;
            }

        if (!achou)
            valido = false;
        return valido;
    } catch (e) {
        postGerarLog(e);
    }
}

function ehMaiorQueTamanhoMaximo(nomeArquivo) {
    try {
        var max = false;
        var tamanhoNomeArquivo = nomeArquivo.length;
        max = tamanhoNomeArquivo > 100 ? true : false;
        return max;
    } catch (e) {
        postGerarLog(e);
    }

}

function alterarTamnhoBotaoDesistencia(id) {
    decreaseBtn(document.getElementById(id), '20px');
}

function montarMetodosFaq() {

    //Criação da Grade de Faq
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
		"dojo/parser",
		"dojo/dom",
		"dojo/on",
		"dojo/has",
		"dijit/form/DateTextBox",
		"dijit/Dialog",
		"dojo/parser",
		"dojo/domReady!",
		"dijit/Tooltip",
		"dojox/form/Uploader",
		"dojox/form/uploader/FileList",
		"dojox/form/uploader/plugins/HTML5",
		"dojox/form/FileUploader",
		"dojo/aspect",
		'dojo/dom-style',
        "dojo/data/ItemFileWriteStore",
        "dijit/tree/ForestStoreModel",
        "dojox/grid/SGFLazyTreeGrid",
        "dojox/grid/LazyTreeGridStoreModel"
    ], function (xhr, registry, EnhancedGrid, DataGrid,
        Pagination, JsonRest, JsonRestStore, ObjectStore,
        ItemFileReadStore, Cache, Memory, query, domAttr,
        Button, TextBox, ready, DropDownButton, DropDownMenu,
        MenuItem, parser, dom, on, has, DateTextBox, Tooltip,
        Uploader, FileList, HTML5, FileUploader, aspect, domStyle,
        ItemFileWriteStore, ForestStoreModel, SGFLazyTreeGrid, LazyTreeGridStoreModel) {
        ready(function () {
            showCarregando();

            myStore = Cache(
			 JsonRest({
			     target: Endereco() + "/api/coordenacao/obterFaqsPorFiltros?dc_faq_pergunta=&dc_faq_pergunta_inicio=false",
			     handleAs: "json",
			     headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },


			 }), Memory({}));

            //myStore.query({
            //    dc_faq_pergunta: "",
            //    dc_faq_pergunta_inicio: false,
            //    menu: []
            //});

            var model = new dijit.tree.ForestStoreModel({
                store: ObjectStore({ objectStore: myStore }),
                serverStore: true,
                childrenAttrs: ['children']
            });

            var layout =
                [
					{ name: "<input id='btnPlus' style='display:none'/>", field: "btnPlus", width: "2%", styles: "text-align:center; min-width:10px; max-width:10px;", formatter: formatBtnPlus },
					{ hidden: !eval(MasterGeral()), name: "<input id='selecionaTodosFaqs' style='display:none'/>", field: "selecionadoFaq", width: "3%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFaq },
					{ name: "Pergunta", field: "dc_faq_pergunta", width: "40%", styles: "min-width:15px; max-width: 20px;" },
					{ name: "Menu", field: "nm_menu_faq", width: "10%", styles: "min-width:15px; max-width: 20px;", formatter: tipoMenuFormatado },
                ];

            var gridFaqs = new dojox.grid.LazyTreeGrid({
                id: 'gridFaqs',
                treeModel: model,
                structure: layout,
                rowsPerPage: 25,
                noDataMessage: "Nenhum registro encontrado."
            }, "gridFaqs");
            
            gridFaqs.on("RowDblClick", function (evt) {
                if (!eval(MasterGeral())) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }

                var idx = evt.rowIndex;
                var faq_selecionado = this.getItem(idx);
                obterFaqPorID(faq_selecionado.cd_faq);



                dijit.byId("dialogFaq").show();
                IncluirAlterar(0, 'divAlterarFaq', 'divIncluirFaq', 'divExcluirFaq', 'apresentadorMensagemFaq', 'divCancelarFaq', 'divLimparFaq');

                hideBtnUploader('uploaderVideoIncluir', "myDivIncluir");
                showBtnUploader('uploaderVideoEditar', "myDivEditar");

            }, true);

            gridFaqs.startup();

            if (eval(MasterGeral())) {
                // Adiciona link de ações:
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

                        hideBtnUploader('uploaderVideoIncluir', "myDivIncluir");
                        showBtnUploader('uploaderVideoEditar', "myDivEditar");
                        eventoEditarVideo(dijit.byId('gridFaqs').itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () {

                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        eventoRemover(dijit.byId('gridFaqs').itensSelecionados, 'deletarFaq(itensSelecionados)');
                    }
                });
                menu.addChild(acaoExcluir);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasFaq",
                    dropDown: menu,
                    id: "acoesRelacionadasFaq"
                });
                dom.byId("linkAcoesFaq").appendChild(button.domNode);
            }

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridFaqs, 'todosItensFaqs', ['pesquisarFaq']);
                    pesquisarFaq(false);
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () {
                    if (!hasValue(gridFaqs.itensSelecionados)) {
                        gridFaqs.itensSelecionados = [];
                    }

                    buscarFaqsSelecionados();
                }
            });
            menu.addChild(menuItensSelecionados);


            button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensFaqs",
                dropDown: menu,
                id: "todosItensFaqs"
            });
            dom.byId("linkSelecionadosFaq").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\

            new Button({
                label: "Salvar",
                iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
                onClick: function () {
                    if (hasValue(dojo.byId("NomeArquivo").value)) {
                        salvarArquivo();
                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(uploadCompleate, "someMethod", function (statusUpload) {
                                if (statusUpload) {
                                    salvarFaq();
                                }
                            });

                        });
                    } else {
                        salvarFaq();
                    }
                }

            }, "incluirFaq");

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    if (hasValue(dojo.byId("NomeArquivo").value)) {
                        salvarArquivoEdicao("editar");
                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(uploadCompleateEdicao, "someMethodEdit", function (statusUpload) {
                                if (statusUpload) {
                                    editarFaq();
                                }
                            });

                        });
                    } else {
                        editarFaq();
                    }
                }
            }, "alterarFaq");

            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {

                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }

                        deletarFaq()
                    });
                }
            }, "deleteFaq");

            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    limparCamposFaq();
                }
            }, "limparFaq");

            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    obterFaqPorID(dom.byId("cd_faq").value);
                    myUploaderIncluir.reset();
                    myUploaderEditar.reset();
                }
            }, "cancelarFaq");

            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("dialogFaq").hide();
                    limparCamposFaq();
                }
            }, "fecharFaq");

            new Tooltip({
                connectId: ["uploaderVideo"],
                label: "Upload",
                position: ['above']
            });


            /***********Criação do Botão de incluir Video***********/
            myUploaderIncluir = new dojox.form.Uploader({
                accept: "image/jpg",
                class: "browseButton",
                type: "file",
                iconClass: "dijitEditorIcon dijitEditorIconUpload",
                label: "",
                multiple: false,
                uploadOnSelect: false,
                force: 'html5',
                name: "UploadedVideo",
                isDebug: true,
                url: Endereco() + "/api/coordenacao/UploadVideoFaq"
            }, "uploaderVideoIncluir");


            var list = new dojox.form.uploader.FileList({ uploaderId: "uploaderVideoIncluir", headerFilename: "Nome: ", headerFilesize: "Tamanho: ", headerType: "Tipo: " });
            dojo.byId("myDivIncluir").appendChild(list.domNode);
            //myUploaderIncluir.startup();
            list.startup();



            dojo.connect(myUploaderIncluir, "onComplete", function (retorno) {
                //Verifica se Salvou o vídeo com sucesso

                if (handlerErrorVideo(retorno)) {
                    uploadCompleate.someMethod(true);
                    //dojo.disconnect(handle);
                } else {
                    myUploaderIncluir.reset();
                    dijit.byId("NomeArquivo").set("value", "");
                    uploadCompleate.someMethod(false);
                }
            });



            dojo.connect(myUploaderIncluir, "onChange", function (evt) {

                var dataArray = myUploaderIncluir.getFileList();
                dojo.byId("NomeArquivo").value = dataArray[0].name;
            });

            myUploaderIncluir.startup();



            /***********Criação do Botão de Editar Video***********/
            myUploaderEditar = new dojox.form.Uploader({
                accept: "image/jpg",
                class: "browseButton",
                type: "file",
                iconClass: "dijitEditorIcon dijitEditorIconUpload",
                label: "",
                multiple: false,
                uploadOnSelect: false,
                force: 'html5',
                name: "UploadedVideo",
                isDebug: true,
                url: Endereco() + "/api/coordenacao/UploadVideo"
            }, "uploaderVideoEditar");


            var listEditar = new dojox.form.uploader.FileList({ uploaderId: "uploaderVideoEditar", headerFilename: "Nome: ", headerFilesize: "Tamanho: ", headerType: "Tipo: " });
            dojo.byId("myDivEditar").appendChild(listEditar.domNode);
            //myUploaderEditar.startup();
            listEditar.startup();



            dojo.connect(myUploaderEditar, "onComplete", function (retorno) {
                //Verifica se Salvou o vídeo com sucesso
                if (handlerErrorVideo(retorno)) {

                    uploadCompleateEdicao.someMethodEdit(true);
                    //dojo.disconnect(handle);
                } else {
                    myUploaderEditar.reset();
                    dijit.byId("NomeArquivo").set("value", "");
                    uploadCompleateEdicao.someMethodEdit(false);
                    //dojo.disconnect(handle);
                }

            });



            dojo.connect(myUploaderEditar, "onChange", function (evt) {

                var dataArray = myUploaderEditar.getFileList();
                dojo.byId("NomeArquivo").value = dataArray[0].name;
            });

            myUploaderEditar.startup();

            decreaseBtn(document.getElementById("uploaderVideoIncluir"), '18px');
            decreaseBtn(document.getElementById("uploaderVideoEditar"), '18px');

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    pesquisarFaq(true);
                }
            }, "pesquisarFaq");

            decreaseBtn(document.getElementById("pesquisarFaq"), '32px');            

            if (!eval(MasterGeral())) {
                dojo.byId("novoFaq").style.display = "none";
            } else {
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

                        limparCamposFaq();
                        IncluirAlterar(1, 'divAlterarFaq', 'divIncluirFaq', 'divExcluirFaq', 'apresentadorMensagemFaq', 'divCancelarFaq', 'divLimparFaq');
                        dijit.byId("dialogFaq").show();

                        hideBtnUploader('uploaderVideoEditar', "myDivEditar");
                        showBtnUploader('uploaderVideoIncluir', "myDivIncluir");

                    }
                }, "novoFaq");
                dojo.byId("novoFaq").style.display = "";
            }

            //loadTipo(Memory);
            loadTipoMenu(Memory);

            loadMenu('cbMenu', ItemFileReadStore);
            showCarregando();

        })
    });
};


function handlerErrorVideo(callback) {
    if (!callback.retorno) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, callback.MensagensWeb[0].mensagem);
        apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
    }
    return callback.retorno;
}


function downloadVideo(cd_faq) {
    var parametrosUrl = {
        codigo_faq: "cd_faq=" + cd_faq
    }
    try {
        window.open(Endereco() + "/Informacao/getDownloadArquivo/?" + parametrosUrl.codigo_faq);
    } catch (e) {
        postGerarLog(e);
    }
}

function hideBtnUploader(botao, tableDetail) {
    dojo.style(dijit.byId(botao).domNode, { visibility: 'hidden', display: 'none' });
    dojo.style(dojo.byId(tableDetail), { visibility: 'hidden', display: 'none' });
}

function showBtnUploader(botao, tableDetail) {
    dojo.style(dijit.byId(botao).domNode, { visibility: 'visible', display: 'inline-table' });
    dojo.style(dojo.byId(tableDetail), { visibility: 'visible', display: 'block' });
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

function pesquisarFaq(limparItens) {

    require([
	  "dojo/_base/xhr",
	  "dojo/data/ObjectStore",
	  "dojo/store/Cache",
	  "dojo/store/Memory",
      "dojo/store/JsonRest",
	  "dojox/data/JsonRestStore"
    ], function (xhr, ObjectStore, Cache, Memory, JsonRest, JsonRestStore) {
        try {
            showCarregando();

            var gridFaqs = dijit.byId("gridFaqs");
            dc_faq_pergunta = dijit.byId("_dcFaqPergunta").value;
            dc_faq_pergunta_inicio = dijit.byId("_dcFaqInicio").checked;
            menu = hasValue(dijit.byId("cbMenu").value) ? JSON.stringify(dijit.byId("cbMenu").value) : [];

            myStore = Cache(
			 JsonRest({
			     target: Endereco() + "/api/coordenacao/obterFaqsPorFiltros?dc_faq_pergunta=" + dc_faq_pergunta +
                     "&dc_faq_pergunta_inicio=" + dc_faq_pergunta_inicio + "&menu=" + menu,
			     handleAs: "json",
			     headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
			 }), Memory({}));

            var model = new dijit.tree.ForestStoreModel({
                store: ObjectStore({ objectStore: myStore }),
                serverStore: true,
                childrenAttrs: ['children']
            });

            if (limparItens) {
                gridFaqs.itensSelecionados = [];
            }

            gridFaqs.setModel(model);

            setTimeout(function () {
                gridFaqs.refresh();
            }, 500);

            showCarregando();


        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function salvarArquivo() {

    if (!dijit.byId("formFaq").validate())
        return false;

    var mensagensWeb = new Array();
    var dataArray = myUploaderIncluir.getFileList();
    //You can put some validations here ...

    apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, false);
    if (hasValue(dataArray) && dataArray.length > 0) {
        if (hasValue(dataArray[0]) && dataArray[0].size > 524288000) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanhoVideo);
            apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
            myUploaderIncluir.reset();
            dijit.byId("NomeArquivo").set("value", "");
            return false;
        }
        if (!verificarExtensaoArquivo(dataArray[0].type)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoErradaArquivoVideo);
            apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
            myUploaderIncluir.reset();
            dijit.byId("NomeArquivo").set("value", "");
            return false;
        }
        if (ehMaiorQueTamanhoMaximo(dataArray[0].name)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
            apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);

            myUploaderIncluir.reset();
            dijit.byId("NomeArquivo").set("value", "");
            return false;
        }
        myUploaderIncluir.submit();
    } else {
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
        apresentaMensagem("apresentadorMensagemFaq", "Arquivo inválido", true);
        dijit.byId("NomeArquivo").set("value", "");
    }


}


function salvarArquivoEdicao(operacao) {

    if (!dijit.byId("formFaq").validate())
        return false;

    var mensagensWeb = new Array();
    var dataArray = myUploaderEditar.getFileList();

    if (hasValue(operacao) && operacao === "editar" && !hasValue(dataArray)) {
        uploadCompleateEdicao.someMethodEdit(true); //Editar sem alterar o arquivo
    } else {
        apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, false);
        if (hasValue(dataArray) && dataArray.length > 0) {
            if (hasValue(dataArray[0]) && dataArray[0].size > 100000000) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanhoVideo);
                apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
                myUploaderEditar.reset();
                dijit.byId("NomeArquivo").set("value", "");
                return false;
            }
            if (!verificarExtensaoArquivo(dataArray[0].type)) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoErradaArquivoVideo);
                apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
                myUploaderEditar.reset();
                dijit.byId("NomeArquivo").set("value", "");
                return false;
            }
            if (ehMaiorQueTamanhoMaximo(dataArray[0].name)) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
                apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
                myUploaderEditar.reset();
                dijit.byId("NomeArquivo").set("value", "");
                return false;
            }
            myUploaderEditar.submit();
        } else {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
            apresentaMensagem("apresentadorMensagemFaq", "Arquivo inválido", true);
            dijit.byId("NomeArquivo").set("value", "");
        }

    }
}

function salvarFaq() {
    try {
        apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, false);
        if (!dijit.byId("formFaq").validate())
            return false;

        if (!hasValue(trim(dojo.byId('faqResposta').value))) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Obrigatório preencher o campo resposta.");
            apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
            return false;
        }

        showCarregando();

        var faq = {
            dc_faq_pergunta: dijit.byId("faqPergunta").value,
            dc_faq_resposta: dijit.byId("faqResposta").value,
            nm_menu_faq: dijit.byId("cbMenuCad").value,
            no_video_faq: hasValue(dojo.byId("NomeArquivo").value) ? dojo.byId("NomeArquivo").value : null
        };

        require([
		  "dojo/_base/xhr"
        ], function (xhr) {
            xhr.post({
                url: Endereco() + "/api/coordenacao/adicionarFaq",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify(faq)
            }).then(function (data) {
                var faqs = JSON.parse(data);
                if (!hasValue(faqs.erro)) {
                    faqs = faqs.retorno;

                    var gridFaqs = dijit.byId("gridFaqs");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("dialogFaq").hide();
                    if (!hasValue(gridFaqs.itensSelecionados)) {
                        gridFaqs.itensSelecionados = [];
                    }
                    insertObjSort(gridFaqs.itensSelecionados, "cd_faq", faqs);
                    buscarFaqsSelecionados();

                } else {

                    apresentaMensagem('apresentadorMensagemFaq', data);
                }

            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemFaq', error.response.data);
            });
        });

        showCarregando();

    } catch (e) {
        showCarregando();
        postGerarLog(e);
    }

}

function deletarFaq(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            showCarregando();
            var cd_faq = 0;
            if (!hasValue(itensSelecionados) || itensSelecionados.length == 0) {
                if (dojo.byId('cd_faq').value > 0)
                    itensSelecionados = [{ cd_faq: dom.byId("cd_faq").value }];
            }

            xhr.post({
                url: Endereco() + "/api/coordenacao/deletarFaq",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {

                var faq = JSON.parse(data);

                if (!hasValue(faq.erro)) {
                    var todos = dojo.byId("todosItensFaqs");
                    apresentaMensagem('apresentadorMensagem', data);

                    dijit.byId('dialogFaq').hide();

                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(itensSelecionados, "cd_faq", itensSelecionados[r].cd_faq);

                    pesquisarFaq(true);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarFaq").set('disabled', false);
                    dijit.byId("relatorioFaq").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";

                }
                else {

                    apresentaMensagem('apresentadorMensagemFaq', data);
                }
            }, function (error) {

                if (!hasValue(dojo.byId("dialogFaq").style.display))
                    apresentaMensagem('apresentadorMensagemFaq', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });

            showCarregando();
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function eventoEditarVideo(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            limparCamposFaq();
            apresentaMensagem('apresentadorMensagem', '');
            var faq = itensSelecionados[0];

            obterFaqPorID(faq.cd_faq);
            dijit.byId("dialogFaq").show();
            IncluirAlterar(0, 'divAlterarFaq', 'divIncluirFaq', 'divExcluirFaq', 'apresentadorMensagemFaq', 'divCancelarFaq', 'divLimparFaq');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarFaq() {

    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, false);
            if (!dijit.byId("formFaq").validate())
                return false;

            if (!hasValue(trim(dojo.byId('faqResposta').value))) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Obrigatório preencher o campo resposta.");
                apresentaMensagem("apresentadorMensagemFaq", mensagensWeb, true);
                return false;
            }

            showCarregando();

            var faq = {
                cd_faq: dojo.byId("cd_faq").value,
                dc_faq_pergunta: dijit.byId("faqPergunta").value,
                dc_faq_resposta: dijit.byId("faqResposta").value,
                nm_menu_faq: dijit.byId("cbMenuCad").value,
                no_video_faq: dojo.byId("NomeArquivo").value
            };

            xhr.post({
                url: Endereco() + "/api/coordenacao/editarFaq",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(faq)
            }).then(function (data) {

                var faq = JSON.parse(data);
                faq = JSON.parse(faq);

                if (!hasValue(faq.erro)) {
                    var faq = faq.retorno;

                    var gridFaqs = dijit.byId("gridFaqs");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("dialogFaq").hide();
                    if (!hasValue(gridFaqs.itensSelecionados)) {
                        gridFaqs.itensSelecionados = [];
                    }
                    removeObjSort(gridFaqs.itensSelecionados, "cd_faq", dom.byId("cd_faq").value);
                    insertObjSort(gridFaqs.itensSelecionados, "cd_faq", faq);
                    //buscarItensSelecionados('gridFaqs', 'selecionadoFaq', 'cd_faq', 'selecionaTodosFaqs', ['pesquisarFaq'], 'todosItensFaqs');
                    buscarFaqsSelecionados();

                } else {
                    apresentaMensagem('apresentadorMensagemFaq', data);
                }

            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemFaq', error);
            });

            showCarregando();
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function obterFaqPorID(cd_faq) {
    require(["dojo/_base/xhr"], function (xhr) {
        try {
            showCarregando();
            xhr.get({
                url: Endereco() + "/api/coordenacao/obterFaqPorID?cd_faq=" + cd_faq,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                showCarregando();
                var faq = jQuery.parseJSON(data);
                if (!hasValue(faq.erro)) {
                    preencheInputFaq(faq.retorno);
                }
            }, function (e) {
                showCarregando();
            });
        } catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    })
}

function preencheInputFaq(faq) {
    if (hasValue(faq)) {
        dojo.byId("cd_faq").value = faq.cd_faq;
        dijit.byId("faqPergunta").set("value", faq.dc_faq_pergunta);
        dijit.byId("faqResposta").set("value", faq.dc_faq_resposta);

        if (hasValue(dojo.byId('cbMenuCad').value, true))
            hasValue(faq.nm_menu_faq) ? dijit.byId("cbMenuCad").set("value", faq.nm_menu_faq) : 0;

        dijit.byId("NomeArquivo").set("value", faq.no_video_faq);
    }
}

function limparCamposFaq() {

    dojo.byId("cd_faq").value = 0;
    dijit.byId("faqPergunta").set("value", "");
    dijit.byId("faqResposta").set("value", "");

    if (hasValue(dojo.byId('cbMenuCad').value, true))
        dijit.byId("cbMenuCad").reset();

    dijit.byId("NomeArquivo").set("value", "");
    apresentaMensagem("apresentadorMensagemFaq", null);
    myUploaderIncluir.reset();
    myUploaderEditar.reset();

}

function loadTipoMenu(Memory) {
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
    dijit.byId("cbMenuCad").store = stateStore;
}

function videDetail(cd_faq) {
    abrePopUp(Endereco() + '/Informacao/FaqVideoDetail?cd_faq=' + cd_faq, '765px', '771px');
}

function buscarFaqsSelecionados() {
    var gridFaqs = dijit.byId("gridFaqs");
    var data = {
        label: 'dc_faq_pergunta',
        items: gridFaqs.itensSelecionados
    };

    var store = new dojo.data.ItemFileWriteStore({ data: data });
    var model = new dijit.tree.ForestStoreModel({ store: store, childrenAttrs: ['children'] });
    var todos = dojo.byId("todosItensFaqs_label");

    if (hasValue(todos))
        todos.innerHTML = "Itens Selecionados";

    gridFaqs.setModel(model);
    gridFaqs.refresh();
    dijit.byId("pesquisarFaq").set('disabled', true);
}