
function formatCheckBoxVideo(value, rowIndex, obj) {
	try {
		var gridName = 'gridVideos';
		var grid = dijit.byId(gridName);
		var icon;
		var id = obj.field + '_Selected_' + rowIndex;
		var todos = dojo.byId('selecionaTodosVideos');

		if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
			var indice = binaryObjSearch(grid.itensSelecionados, "cd_video", grid._by_idx[rowIndex].item.cd_video);

			value = value || indice != null; // Item est� selecionado.
		}
		if (rowIndex != -1)
			icon = "<input style='height:16px'  id='" + id + "'/> ";

		// Configura o check de todos:
		if (hasValue(todos) && todos.type == 'text')
			setTimeout("configuraCheckBox(false, 'cd_video', 'selecionadoVideo', -1, 'selecionaTodosVideos', 'selecionaTodosVideos', '" + gridName + "')", grid.rowsPerPage * 3);

		setTimeout("configuraCheckBox(" + value + ", 'cd_video', 'selecionadoVideo', " + rowIndex + ", '" + id + "', 'selecionaTodosVideos', '" + gridName + "')", 2);

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

function montarMetodosVideo() {

	//Criação da Grade de Video
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
		'dojo/dom-style'
	], function (xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, ItemFileReadStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, parser, dom, on, has, DateTextBox, Tooltip, Uploader, FileList, HTML5, FileUploader, aspect, domStyle) {
		ready(function () {
			showCarregando();

			
			myStore = Cache(
			 JsonRest({
				 target: Endereco() + "/api/coordenacao/getVideoSearch",
				 handleAs: "json",
				 headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },


			 }), Memory({}));

			myStore.query({
				nm_video: 0,
				no_video: "",
				menu: []
			});
			
			var gridVideos = new EnhancedGrid({
			   
				store: ObjectStore({ objectStore: myStore }),
				structure:
				[
					{ name: "<input id='selecionaTodosVideos' style='display:none'/>", field: "selecionadoVideo", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxVideo },
                    { name: "Nº Videoaula", field: "nm_video", width: "10%", styles: "text-align: center; min-width:80px;" },
                    { name: "Parte", field: "nm_parte", width: "10%", styles: "text-align: center; min-width:80px;" },
					{ name: "Nome", field: "no_video", width: "40%", styles: "min-width:15px; max-width: 20px;" },
					{ name: "Menu", field: "nm_menu_video", width: "10%", styles: "min-width:15px; max-width: 20px;", formatter: tipoMenuFormatado },
					{
						name: "Video", field: "_item", width: "7%", styles: "text-align: center; min-width:15px; max-width: 20px;",
						formatter: function (item) {
							var label = "Adicionar";

							if (hasValue(dijit.byId("vd_" + item.cd_video))) {
								dijit.byId("vd_" + item.cd_video).destroy();
							}

							var btn = new dijit.form.Button({
								//label: label,
								id: "vd_" + item.cd_video,
								disabled: false,
								iconClass: 'dijitEditorIcon dijitEditorIconPlaySGF',
								onClick: function () {
									try {
										console.log(item);
										abrePopUp(Endereco() + '/Informacao/VideoDetail?cd_video=' + item.cd_video, '765px', '771px');
									} catch (e) {

									}
								}
							});
							setTimeout("alterarTamnhoBotaoDesistencia('" + btn.id + "')", 15);
							return btn;
						}
					},
					{
						name: "Download", field: "_item", width: "7%", styles: "text-align: center; min-width:15px; max-width: 20px;",
						formatter: function (item) {
							var label = "Adicionar";

							if (hasValue(dijit.byId("dw_" + item.cd_video))) {
								dijit.byId("dw_" + item.cd_video).destroy();
							}

							var btn = new dijit.form.Button({
								//label: label,
								id: "dw_" + item.cd_video,
								disabled: false,
                                iconClass: 'dijitEditorIcon dijitEditorIconDownload',
								onClick: function () {
									try {
										console.log(item);
										downloadVideo(item.cd_video);
									} catch (e) {

									}
								}
							});
							setTimeout("alterarTamnhoBotaoDesistencia('" + btn.id + "')", 15);
							return btn;
						}
					}
				],
				noDataMessage: msgNotRegEnc,
				canSort: true,
				selectionMode: "single",
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
			}, "gridVideos");
			gridVideos.pagination.plugin._paginator.plugin.connect(gridVideos.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
				verificaMostrarTodos(evt, gridVideos, 'cd_video', 'selecionaTodosVideos');
			});
			require(["dojo/aspect"], function (aspect) {
				aspect.after(gridVideos, "_onFetchComplete", function () {
					// Configura o check de todos:
					if (dojo.byId('selecionaTodosVideos').type == 'text')
						setTimeout("configuraCheckBox(false, 'cd_video', 'selecionadoVideo', -1, 'selecionaTodosVideos', 'selecionaTodosVideos', 'gridVideos')", gridVideos.rowsPerPage * 3);
				});
			});

			gridVideos.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 5 && Math.abs(col) != 6 };
			gridVideos.on("RowDblClick", function (evt) {
                if (!eval(MasterGeral())) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }

                var idx = evt.rowIndex;
                var video_selecionado = this.getItem(idx);
                obterVideoPorID(video_selecionado.cd_video);



                dijit.byId("dialogVideo").show();
                IncluirAlterar(0, 'divAlterarVideo', 'divIncluirVideo', 'divExcluirVideo', 'apresentadorMensagemVideo', 'divCancelarVideo', 'divLimparVideo');

                hideBtnUploader('uploaderVideoIncluir', "myDivIncluir");
                showBtnUploader('uploaderVideoEditar', "myDivEditar");
                
			}, true);

			gridVideos.startup();

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
					eventoEditarVideo(dijit.byId('gridVideos').itensSelecionados);
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

					eventoRemover(dijit.byId('gridVideos').itensSelecionados, 'deletarVideo(itensSelecionados)');
				}
			});
			menu.addChild(acaoExcluir);

			button = new DropDownButton({
				label: "Ações Relacionadas",
				name: "acoesRelacionadasVideo",
				dropDown: menu,
				id: "acoesRelacionadasVideo"
			});
			dom.byId("linkAcoesVideo").appendChild(button.domNode);

			// Adiciona link de selecionados:
			menu = new DropDownMenu({ style: "height: 25px" });
			var menuTodosItens = new MenuItem({
				label: "Todos Itens",
				onClick: function () { buscarTodosItens(gridVideos, 'todosItensVideos', ['pesquisarVideo']); pesquisarVideo(false); }
			});
			menu.addChild(menuTodosItens);

			var menuItensSelecionados = new MenuItem({
				label: "Itens Selecionados",
				onClick: function () { buscarItensSelecionados('gridVideos', 'selecionadoVideo', 'cd_video', 'selecionaTodosVideos', ['pesquisarVideo'], 'todosItensVideos'); }
			});
			menu.addChild(menuItensSelecionados);


			button = new DropDownButton({
				label: "Todos Itens",
				name: "todosItensVideos",
				dropDown: menu,
				id: "todosItensVideos"
			});
			dom.byId("linkSelecionadosVideo").appendChild(button.domNode);

			//*** Cria os botões de persistência **\\

			new Button({
				label: "Salvar",
				iconClass: "dijitEditorIcon dijitEditorIconNewSGF",
				onClick: function () {
					var isSave = true;
					salvarArquivo();
					require(["dojo/aspect"], function (aspect) {
						aspect.after(uploadCompleate, "someMethod", function (statusUpload) {
							if (statusUpload) {
								salvarVideo();
							}
						});

					});
				}

			}, "incluirVideo");

			new Button({
				label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
				onClick: function () {
					salvarArquivoEdicao("editar");
					require(["dojo/aspect"], function (aspect) {
						aspect.after(uploadCompleateEdicao, "someMethodEdit", function (statusUpload) {
							if (statusUpload) {
								editarVideo();
							}
						});

					});
				}
			}, "alterarVideo");

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

						deletarVideo()
					});
				}
			}, "deleteVideo");

			new Button({
				label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
				type: "reset",
				onClick: function () {
					limparCamposVideo();
				}
			}, "limparVideo");

			new Button({
				label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
				onClick: function () {
					obterVideoPorID(dom.byId("cd_video").value);
					myUploaderIncluir.reset();
					myUploaderEditar.reset();
				}
			}, "cancelarVideo");

			new Button({
				label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
				onClick: function () {
					dijit.byId("dialogVideo").hide();
					limparCamposVideo();
				}
			}, "fecharVideo");

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
				url: Endereco() + "/api/coordenacao/UploadVideo"
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
					pesquisarVideo(true);
				}
			}, "pesquisarVideo");

			decreaseBtn(document.getElementById("pesquisarVideo"), '32px');
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

					limparCamposVideo();
					IncluirAlterar(1, 'divAlterarVideo', 'divIncluirVideo', 'divExcluirVideo', 'apresentadorMensagemVideo', 'divCancelarVideo', 'divLimparVideo');
					dijit.byId("dialogVideo").show();

					hideBtnUploader('uploaderVideoEditar', "myDivEditar");
					showBtnUploader('uploaderVideoIncluir', "myDivIncluir");
				   
				}
			}, "novoVideo");

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
		apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);
	}
	return callback.retorno;
}


function downloadVideo(cd_video) {
	var parametrosUrl = {
		codigo_video: "cd_video=" + cd_video
	}
	try {
		window.open(Endereco() + "/Informacao/getDownloadArquivo/?" + parametrosUrl.codigo_video);
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

function pesquisarVideo(limparItens) {

	require([
	  "dojo/_base/xhr",
	  "dojo/data/ObjectStore",
	  "dojo/store/Cache",
	  "dojo/store/Memory"
	], function (xhr, ObjectStore, Cache, Memory) {
		try {
			showCarregando();

			var gridVideo = dijit.byId("gridVideos");

			var content = {};

            content = {
                nm_video: dijit.byId("_numero").value,
                no_video: dijit.byId("_nome").value,
                menu: dijit.byId("cbMenu").value
            }

			xhr.get({
				url: Endereco() + "/api/coordenacao/obterVideosPorFiltros",
				preventCache: true,
				content: content,
				handleAs: "json",
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
			}).then(function (dataEvento) {

				if (limparItens) {
					gridVideo.itensSelecionados = [];
				}
				showCarregando();
				gridVideo.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: dataEvento }) }));
			});
		} catch (e) {
			showCarregando();
			postGerarLog(e);
		}
	});
}

function salvarArquivo() {

	if (!dijit.byId("formVideo").validate())
		return false;

	var mensagensWeb = new Array();
	var dataArray = myUploaderIncluir.getFileList();
	//You can put some validations here ...

	apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, false);
	if (hasValue(dataArray) && dataArray.length > 0) {
		if (hasValue(dataArray[0]) && dataArray[0].size > 524288000) {
			mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanhoVideo);
			apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);
			myUploaderIncluir.reset();
			dijit.byId("NomeArquivo").set("value", "");
			return false;
		}
		if (!verificarExtensaoArquivo(dataArray[0].type)) {
			mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoErradaArquivoVideo);
			apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);
			myUploaderIncluir.reset();
			dijit.byId("NomeArquivo").set("value", "");
			return false;
		}
		if (ehMaiorQueTamanhoMaximo(dataArray[0].name)) {
			mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
			apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);

			myUploaderIncluir.reset();
			dijit.byId("NomeArquivo").set("value", "");
			return false;
		}
		myUploaderIncluir.submit();
	} else {
		mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
		apresentaMensagem("apresentadorMensagemVideo", "Arquivo inválido", true);
		dijit.byId("NomeArquivo").set("value", "");
	}


}


function salvarArquivoEdicao(operacao) {

	if (!dijit.byId("formVideo").validate())
		return false;

	var mensagensWeb = new Array();
	var dataArray = myUploaderEditar.getFileList();
	
	if (hasValue(operacao) && operacao === "editar" && !hasValue(dataArray)) {
		uploadCompleateEdicao.someMethodEdit(true); //Editar sem alterar o arquivo
	} else {
		apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, false);
		if (hasValue(dataArray) && dataArray.length > 0) {
			if (hasValue(dataArray[0]) && dataArray[0].size > 100000000) {
				mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanhoVideo);
				apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);
				myUploaderEditar.reset();
				dijit.byId("NomeArquivo").set("value", "");
				return false;
			}
			if (!verificarExtensaoArquivo(dataArray[0].type)) {
				mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtensaoErradaArquivoVideo);
				apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);
				myUploaderEditar.reset();
				dijit.byId("NomeArquivo").set("value", "");
				return false;
			}
			if (ehMaiorQueTamanhoMaximo(dataArray[0].name)) {
				mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
				apresentaMensagem("apresentadorMensagemVideo", mensagensWeb, true);
				myUploaderEditar.reset();
				dijit.byId("NomeArquivo").set("value", "");
				return false;
			}
			myUploaderEditar.submit();
		} else {
			mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorNomeExcedeuTamanhoVideo);
			apresentaMensagem("apresentadorMensagemVideo", "Arquivo inválido", true);
			dijit.byId("NomeArquivo").set("value", "");
		}

	}
}

function salvarVideo() {
	try {

		if (!dijit.byId("formVideo").validate())
			return false;
		showCarregando();

		var video = {
			nm_video: dijit.byId("VideoNro").value,
			no_video: dijit.byId("VideoNome").value,
			nm_menu_video: dijit.byId("cbMenuCad").value,
			no_arquivo_video: dojo.byId("NomeArquivo").value,
		    nm_parte: dojo.byId("ParteNro").value
		};

		require([
		  "dojo/_base/xhr"
		], function (xhr) {
			xhr.post({
				url: Endereco() + "/api/coordenacao/adicionarVideo",
				handleAs: "json",
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
				postData: JSON.stringify(video)
			}).then(function (data) {
				var videos = JSON.parse(data);

				if (!hasValue(videos.erro)) {
					videos = videos.retorno;

					var gridVideos = dijit.byId("gridVideos");
					apresentaMensagem('apresentadorMensagem', data);
					dijit.byId("dialogVideo").hide();
					if (!hasValue(gridVideos.itensSelecionados)) {
						gridVideos.itensSelecionados = [];
					}
					insertObjSort(gridVideos.itensSelecionados, "cd_video", videos);
					buscarItensSelecionados('gridVideos', 'selecionadoVideo', 'cd_video', 'selecionaTodosVideos', ['pesquisarVideo'], 'todosItensVideos');
					setGridPagination(gridVideos, videos, "cd_video");

				} else {

					apresentaMensagem('apresentadorMensagemVideo', data);
				}

			}, function (error) {

				apresentaMensagem('apresentadorMensagemVideo', error.response.data);
			});
		});

		showCarregando();

	} catch (e) {
		showCarregando();
		postGerarLog(e);
	}

}

function deletarVideo(itensSelecionados) {
	require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
		try {
			showCarregando();
			var cd_video = 0;
			if (!hasValue(itensSelecionados) || itensSelecionados.length == 0) {
				if (dojo.byId('cd_video').value > 0)
					itensSelecionados = [{ cd_video: dom.byId("cd_video").value }];
			}

			xhr.post({
				url: Endereco() + "/api/coordenacao/deletarVideo",
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
				postData: ref.toJson(itensSelecionados)
			}).then(function (data) {

				var video = JSON.parse(data);

				if (!hasValue(video.erro)) {
					var todos = dojo.byId("todosItensVideos");
					apresentaMensagem('apresentadorMensagem', data);

					dijit.byId('dialogVideo').hide();

					// Remove o item dos itens selecionados:
					for (var r = itensSelecionados.length - 1; r >= 0; r--)
						removeObjSort(itensSelecionados, "cd_video", itensSelecionados[r].cd_video);

					pesquisarVideo(true);

					// Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
					dijit.byId("pesquisarVideo").set('disabled', false);
					dijit.byId("relatorioVideo").set('disabled', false);
					if (hasValue(todos))
						todos.innerHTML = "Todos Itens";

				}
				else {

					apresentaMensagem('apresentadorMensagemVideo', data);
				}
			}, function (error) {

				if (!hasValue(dojo.byId("dialogVideo").style.display))
					apresentaMensagem('apresentadorMensagemVideo', error);
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
			limparCamposVideo();
			apresentaMensagem('apresentadorMensagem', '');
			var video = itensSelecionados[0];

			obterVideoPorID(video.cd_video);
			dijit.byId("dialogVideo").show();
			IncluirAlterar(0, 'divAlterarVideo', 'divIncluirVideo', 'divExcluirVideo', 'apresentadorMensagemVideo', 'divCancelarVideo', 'divLimparVideo');
		}
	}
	catch (e) {
		postGerarLog(e);
	}
}

function editarVideo() {

	require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
		try {

			if (!dijit.byId("formVideo").validate())
				return false;

			showCarregando();

			var video = {
				cd_video: dojo.byId("cd_video").value,
				nm_video: dijit.byId("VideoNro").value,
				no_video: dijit.byId("VideoNome").value,
				nm_menu_video: dijit.byId("cbMenuCad").value,
				no_arquivo_video: dojo.byId("NomeArquivo").value,
                nm_parte: dojo.byId("ParteNro").value
			};

			xhr.post({
				url: Endereco() + "/api/coordenacao/editarVideo",
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
				postData: ref.toJson(video)
			}).then(function (data) {

				var video = JSON.parse(data);
				video = JSON.parse(video);
				if (!hasValue(video.erro)) {
					var video = video.retorno;

					var gridVideos = dijit.byId("gridVideos");
					apresentaMensagem('apresentadorMensagem', data);
					dijit.byId("dialogVideo").hide();
					if (!hasValue(gridVideos.itensSelecionados)) {
						gridVideos.itensSelecionados = [];
					}
					removeObjSort(gridVideos.itensSelecionados, "cd_video", dom.byId("cd_video").value);
					insertObjSort(gridVideos.itensSelecionados, "cd_video", video);
					buscarItensSelecionados('gridVideos', 'selecionadoVideo', 'cd_video', 'selecionaTodosVideos', ['pesquisarVideo'], 'todosItensVideos');
					setGridPagination(gridVideos, video, "cd_video");

				} else {

					apresentaMensagem('apresentadorMensagemVideo', data);
				}

			}, function (error) {

			    apresentaMensagem('apresentadorMensagemVideo', error);
			});

			showCarregando();
		} catch (e) {
			showCarregando();
			postGerarLog(e);
		}
	});
}

function obterVideoPorID(cd_video) {
	require(["dojo/_base/xhr"], function (xhr) {
		try {
			showCarregando();
			xhr.get({
				url: Endereco() + "/api/coordenacao/obterVideoPorID?cd_video=" + cd_video,
				preventCache: true,
				handleAs: "json",
				headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
			}).then(function (data) {
				showCarregando();
				var video = jQuery.parseJSON(data);
				if (!hasValue(video.erro)) {
					preencheInputVideo(video.retorno);
				}
			});
		} catch (e) {
			showCarregando();
			postGerarLog(e);
		}
	})
}

function preencheInputVideo(video) {
	if (hasValue(video)) {
		dojo.byId("cd_video").value = video.cd_video;
		dijit.byId("VideoNro").set("value", video.nm_video);
		dijit.byId("VideoNome").set("value", video.no_video);
		dijit.byId("ParteNro").set("value", video.nm_parte);

		if (hasValue(dojo.byId('cbMenuCad').value, true))
			hasValue(video.nm_menu_video) ? dijit.byId("cbMenuCad").set("value", video.nm_menu_video) : 0;

		dijit.byId("NomeArquivo").set("value", video.no_arquivo_video);
	}
}

function limparCamposVideo() {

	dojo.byId("cd_video").value = 0;
	dijit.byId("VideoNro").set("value", "");
	dijit.byId("VideoNome").set("value", "");
	dijit.byId("ParteNro").set("value", "1");

	if (hasValue(dojo.byId('cbMenuCad').value, true))
		dijit.byId("cbMenuCad").reset();

	dijit.byId("NomeArquivo").set("value", "");
	apresentaMensagem("apresentadorMensagemVideo", null);
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