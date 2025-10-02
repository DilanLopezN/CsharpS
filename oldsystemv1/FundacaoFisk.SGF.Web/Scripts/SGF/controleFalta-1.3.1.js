var PESQUISAALUNOFILTRO = 3;

function formatCheckBoxControleFalta() {

}

function formatCheckBoxControleFalta(value, rowIndex, obj) {
    try {
        var gridName = 'gridControleFalta';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosControleFalta');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_controle_faltas", grid._by_idx[rowIndex].item.cd_controle_faltas);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  Id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_controle_faltas', 'selecionadoControleFalta', -1, 'selecionaTodosControleFalta', 'selecionaTodosControleFalta', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_controle_faltas', 'selecionadoControleFalta', " + rowIndex + ", '" + id + "', 'selecionaTodosControleFalta', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAluno(value, rowIndex, obj) {
    var gridName = 'gridAluno'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosAluno');

    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "Id", grid._by_idx[rowIndex].item.Id);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  Id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'Id', 'selecionadoAluno', -1, 'selecionaTodosAluno', 'selecionaTodosAluno', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'Id', 'selecionadoAluno', " + rowIndex + ", '" + id + "', 'selecionaTodosAluno', '" + gridName + "')", 2);

    return icon;
}

function montarCadastroControleFalta() {
    //Criação da Grade

    require([
            "dojo/_base/xhr",
            "dojox/grid/EnhancedGrid",
            "dojox/grid/enhanced/plugins/Pagination",
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory",
            "dojo/query",
            "dijit/form/Button",
            "dojo/ready",
            "dijit/form/DropDownButton",
            "dijit/DropDownMenu",
            "dijit/MenuItem",
            "dojo/dom",
            "dijit/form/FilteringSelect",
            "dojo/_base/array",
            "dojo/promise/all",
            "dojo/Deferred",
            'dojo/_base/lang',
            'dojox/grid/cells/dijit',
            "dijit/Dialog"
        ],
        function(xhr,
            EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom, FilteringSelect, array, all, Deferred, lang, cells) {
            ready(function() {
            	try {

					/*
					 * Loaders
					 */
                        montarAssinaturas("cbAssinatura");
					/*Fim Loaders*/

                    /**
                     * Ações Relacionadas
                     */
                    var menu = new DropDownMenu({ style: "height: 25px" });
                    var acaoEditar = new MenuItem({
                        label: "Editar",
                        onClick: function () {
                            eventoEditarControleFalta(dijit.byId("gridControleFalta").itensSelecionados, xhr);
                        }
                    });
                    menu.addChild(acaoEditar);

                    var acaoRemover = new MenuItem({
                        label: "Excluir",
                        onClick: function() {
                                eventoRemover(dijit.byId("gridControleFalta").itensSelecionados,
                                    'DeletarControleFalta(itensSelecionados)');
                        }
                    });
                    menu.addChild(acaoRemover);
                    var button = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "acoesRelacionadasControleFalta",
                        dropDown: menu,
                        id: "acoesRelacionadasControleFalta"
                    });
                    dom.byId("linkAcoesControleFalta").appendChild(button.domNode);

                    // Adiciona link de selecionados:
                    menu = new DropDownMenu({ style: "height: 25px" });
                    var menuTodosItens = new MenuItem({
                        label: "Todos Itens",
                        onClick: function() {
                            buscarTodosItens(gridControleFalta,
                                'todosControleFalta',
                                ['pesquisarControleFalta', 'relatorioControleFalta']);
                            PesquisarControleFalta(false);
                        }
                    });
                    menu.addChild(menuTodosItens);

                    var menuItensSelecionados = new MenuItem({
                        label: "Itens Selecionados",
                        onClick: function() {
                            buscarItensSelecionados('gridControleFalta',
                                'selecionadoControleFalta',
                                'cd_controle_faltas',
                                'selecionaTodosControleFalta',
                                ['pesquisarControleFalta', 'relatorioControleFalta'],
                                'todosItensControleFalta');
                        }
                    });
                    menu.addChild(menuItensSelecionados);

                    var button = new DropDownButton({
                        label: "Todos Itens",
                        name: "todosItensControleFalta",
                        dropDown: menu,
                        id: "todosItensControleFalta"
                    });
                    dom.byId("linkSelecionadosControleFalta").appendChild(button.domNode);
            	    /*-------------Fim Ações relacionadas -----------*/

                    require(["dijit/registry", "dojo/on"], function (registry, on) {
                        on(registry.byId("dtIniAula"), "change", function (value) {
                            if (hasValue(dojo.byId("dtIniAula")) && hasValue(dojo.byId("cd_turma").value) && dojo.byId("cd_turma").value > 0) {
                                        getAlunosControleFalta();
                            }
                        });
                    });

                    //dojo.connect(dojo.byId("dtIniAula"), "change", function () {
                    //    if (hasValue(dojo.byId("dtIniAula")) && hasValue(dojo.byId("cd_turma").value) && dojo.byId("cd_turma").value > 0) {
                    //        getAlunosControleFalta();
                    //    }
            	    //});


                    //dijit.byId("dtIniAula").on("change", function (e) {
                    //    if (hasValue(dojo.byId("dtIniAula")) && hasValue(dojo.byId("cd_turma").value) && dojo.byId("cd_turma").value > 0) {
                    //        getAlunosControleFalta();
                    //    }
                    //});


                    /*
                     * Botões
                     */

                    new Button({
                            label: "Salvar",
                            iconClass: 'dijitEditorIcon dijitEditorIconSave',
                            onClick: function () {
                                    IncluirControleFalta();
                            }
                        },
                        "incluirControleFalta");
                    new Button({
                            label: "Salvar",
                            iconClass: 'dijitEditorIcon dijitEditorIconSave',
                            onClick: function() {
                                    AlterarControleFalta();
                            }
                        },
                        "alterarControleFalta");
                    new Button({
                            label: "Excluir",
                            iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                            onClick: function() {

                                    caixaDialogo(DIALOGO_CONFIRMAR,
                                        '',
                                        function executaRetorno() {
                                            DeletarControleFalta();
                                        });
                            }
                        },
                        "deleteControleFalta");

                    new Button({
                            label: "Limpar",
                            iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                            onClick: function() {
                                limparFormControleFaltas();
                                //dojo.byId("atendente").value = document.getElementById('nomeUsuario').innerText;
                            }
                        },
                        "limparControleFalta");

                    new Button({
                            label: "Cancelar",
                            iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                            onClick: function() {
                                showCarregando();
                                keepValues(null, null, dijit.byId("gridControleFalta"), null);
                                getAlunosCancelarControleFalta(dijit.byId("gridControleFalta").selection.getSelected()[0], xhr);
                                showCarregando();
                            }
                        },
                        "cancelarControleFalta");
                    new Button({
                            label: "Fechar",
                            iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                            onClick: function() {
                                dijit.byId("cadControleFalta").hide();
                            }
                        },
                        "fecharControleFalta");

                    

                    new Button({
                            label: "",
                            iconClass: 'dijitEditorIconSearchSGF',
                            onClick: function () {
                                    apresentaMensagem('apresentadorMensagem', null);
                                    PesquisarControleFalta(true);
                            }
                    }, "pesquisarControleFalta");

                    new Button({
                    	label: "Novo",
                    	iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    	onClick: function () {
                    	    try {

                                limparFormControleFaltas();
                                dojo.byId('dt_atual').value = dojo.date.locale.format(new Date(),
                                    { selector: "date", datePattern: "dd/MM/yyyy HH:mm", formatLength: "short", locale: "pt-br" });

                                    dojo.byId("atendente").value = document.getElementById('nomeUsuario').innerText;
                                    //dojo.byId('dt_atual').value = new Date();
                                    
                    				showCarregando();
                    				apresentaMensagem('apresentadorMensagem', null);
                    				IncluirAlterar(1, 'divAlterarControleFalta', 'divIncluirControleFalta', 'divExcluirControleFalta', 'apresentadorMensagemControleFalta', 'divCancelarControleFalta', 'divLimparControleFalta');

                    				dijit.byId("cadControleFalta").show();

                    				//limparGrid();
                    				showCarregando();
                    			

                    		} catch (e) {
                    			postGerarLog(e);
                    			showCarregando();
                    		}
                    	}
                    },
                    "novoControleFalta");

                    new Button({
                    	label: getNomeLabelRelatorio(),
                    	iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    	onClick: function () {
                            var controleFalta = montarParametrosUrlControleFalta();
                    		require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"],
                                function (dom, domAttr, xhr, ref) {
                                	xhr.get({
                                		url: Endereco() +
                                            "/api/Coordenacao/GetUrlRelatorioControleFaltas?" +
                                            getStrGridParameters('gridControleFalta') + "desc=" +
                                            encodeURIComponent(document.getElementById("no_turma").value) +
                                            "&cd_turma=" + (controleFalta.cd_turma || 0) + "&cd_aluno="+ (controleFalta.cd_aluno || 0) +"&assinatura="+ (controleFalta.assinatura || 0) + "&dataIni="+ (controleFalta.dataIni || "")+"&dataFim=" + (controleFalta.dataFim || ""),
                                		preventCache: true,
                                		handleAs: "json",
                                		headers: {
                                			"Accept": "application/json",
                                			"Content-Type": "application/json",
                                			"Authorization": Token()
                                		}
                                	}).then(function (data) {
                                		abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data,
											'1000px',
											'750px',
											'popRelatorio');
                                	},
                                        function (error) {
                                        	apresentaMensagem('apresentadorMensagem', error);
                                        });
                                });
                    	}
                    },
                    "relatorioControleFalta");

                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            try {
                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                    montarGridPesquisaAluno(false, function () {
                                        abrirAlunoFK();
                                    });
                                }
                                else
                                    abrirAlunoFK();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }
                    }, "pesAluno");

                    var pesAluno = document.getElementById('pesAluno');
                    if (hasValue(pesAluno)) {
                        pesAluno.parentNode.style.minWidth = '18px';
                        pesAluno.parentNode.style.width = '18px';
                    }

                    //criação do botões pesquisa principal
                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("cdAlunoPes").value = 0;
                            dojo.byId("noAluno").value = null;
                            dijit.byId("noAluno").value = null;
                            apresentaMensagem('apresentadorMensagem', null);
                            dijit.byId("limparAlunoPes").set('disabled', true);
                        }
                    }, "limparAlunoPes");

                    if (hasValue(document.getElementById("limparAlunoPes"))) {
                        document.getElementById("limparAlunoPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparAlunoPes").parentNode.style.width = '40px';
                    }


                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                montarGridPesquisaTurmaFK(function () {
                                    abrirTurmaFK();
                                    dijit.byId("pesAlunoTurmaFK").on("click", function (e) {
                                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                            montarGridPesquisaAluno(false, function () {
                                                abrirAlunoFKTurmaFK(true);
                                                
                                            });
                                        }
                                        else
                                            abrirAlunoFKTurmaFK(true);

                                    });
                                });
                            else
                                abrirTurmaFK();
                        }
                    }, "pesTurma");

                    var pesAluno = document.getElementById('pesTurma');
                    if (hasValue(pesAluno)) {
                        pesAluno.parentNode.style.minWidth = '18px';
                        pesAluno.parentNode.style.width = '18px';
                    }

                    new Button({
                        label: "Limpar", iconClass: '', type: "reset", disabled: true,
                        onClick: function () {
                            dojo.byId("no_turma").value = "";
                            dojo.byId("cdTurmaPesTurma").value = 0;
                            dijit.byId("limparTurmaPes").set('disabled', true);
                            apresentaMensagem('apresentadorMensagem', null);
                        }
                    }, "limparTurmaPes");
                    if (hasValue(document.getElementById("limparTurmaPes"))) {
                        document.getElementById("limparTurmaPes").parentNode.style.minWidth = '40px';
                        document.getElementById("limparTurmaPes").parentNode.style.width = '40px';
                    }

                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                            if (dojo.byId('dtIniAula').value != 'Invalid Date') {
                                apresentaMensagem("apresentadorMensagemControleFalta", "");
                                if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                    montarGridPesquisaTurmaFK(function() {
                                        abrirTurmaFKCadastro();
                                        dijit.byId("pesAlunoTurmaFK").on("click",
                                            function(e) {
                                                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                                    montarGridPesquisaAluno(false,
                                                        function() {
                                                            abrirAlunoFKTurmaFK(true);
                                                        });
                                                } else
                                                    abrirAlunoFKTurmaFK(true);
                                            });
                                    });
                                else
                                    abrirTurmaFKCadastro();
                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Selecione uma data para pesquisar uma turma.");
                                apresentaMensagem("apresentadorMensagemControleFalta", mensagensWeb);
                            }
                            
                        }
                    }, "cadProTurmaFK");
                    btnPesquisar(dojo.byId("cadProTurmaFK"));

                    new Button({
                        label: "Incluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function () {
                            if (dojo.byId('dtIniAula').value != 'Invalid Date' && dojo.byId('dtIniAula').value != "") {
                                if (dojo.byId("cd_turma").value > 0) {

                                    apresentaMensagem("apresentadorMensagemControleFalta", "");
                                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                        montarGridPesquisaAluno(false,
                                            function() {
                                                abrirAlunoGridFk();
                                            });
                                    } else {
                                        abrirAlunoGridFk();
                                    }

                                } else {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Selecione uma turma para incluir um aluno.");
                                    apresentaMensagem("apresentadorMensagemControleFalta", mensagensWeb);
                                }

                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Selecione uma data para incluir um aluno.");
                                apresentaMensagem("apresentadorMensagemControleFalta", mensagensWeb);
                            }
                        }
                    }, "btnNovoAluno");

                    dijit.byId("tipoTurmaFK").on("change", function (e) {
                        try {
                            dijit.byId("pesTurmasFilhasFK").set("checked", true);
                            dijit.byId('pesTurmasFilhasFK').set('disabled', true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });

                    /*---------Fim Botões----------------*/


                    /*
                     * Grid ControleFaltas
                     */

                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/Coordenacao/getControleFaltasSearch?desc=&cd_turma=0&cd_aluno=0&assinatura=0&dataIni=&dataFim=",
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            idProperty: "cd_turma"
                        }),
            		    Memory({ idProperty: "cd_turma" }));

                    var gridControleFalta = new EnhancedGrid({
                    	store: ObjectStore({ objectStore: myStore }),
                        //store: ObjectStore({ objectStore: new dojo.store.Memory({ data: myStore }) }),
                    	structure: [
							{
								name: "<input id='selecionaTodosControleFalta' style='display:none'/>",
								field: "selecionadoControleFalta",
								width: "5%",
								styles: "text-align:center; min-width:15px; max-width:20px;",
								formatter: formatCheckBoxControleFalta
							},
							//   { name: "Código", field: "cd_item", width: "5%", styles: "text-align: right; min-width:75px; max-width:75px;" },
							{ name: "Data", field: "dt_controle_faltas", width: "8%", formatter: formatterDate },
							{ name: "Turma", field: "no_turma", width: "50%", styles: "min-width:80px;" },
							{ name: "Usuário", field: "no_usuario", width: "10%", styles: "min-width:80px;" },
							{ name: "Data Cadastro", field: "dh_controle_faltas", width: "10%", styles: "min-width:80px;", formatter: formatterDateHours }
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
                    			position: "button",
                    			plugins: { nestedSorting: true }
                    		}
                    	}
                    },
                        "gridControleFalta"); // make sure you have a target HTML element with this id
            		// Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                    gridControleFalta.pagination.plugin._paginator.plugin.connect(gridControleFalta.pagination.plugin._paginator,
                        'onSwitchPageSize',
                        function (evt) {
                        	verificaMostrarTodos(evt, gridControleFalta, 'cd_item', 'selecionaTodosControleFalta');
                        });
                    var idGrupoItem = 0;
                    gridControleFalta.startup();
                    gridControleFalta.canSort = function (col) { return Math.abs(col) != 1; };
                    gridControleFalta.on("RowDblClick",
                        function (evt) {
                        	try {

                                //limparGridControleFaltas();
                        			var idx = evt.rowIndex,
                                        item = this.getItem(idx),
                                        store = this.store;
                        			showCarregando();
                                    limparFormControleFaltas();
                        			item.cd_controle_faltas = item.cd_controle_faltas == null ? 0 : item.cd_controle_faltas;
                                    dojo.byId("cd_controle_faltas").value = item.cd_controle_faltas;
                        				apresentaMensagem('apresentadorMensagem', '');
                        				keepValues(null, item, gridControleFalta, false);
                        				dijit.byId("cadControleFalta").show();

                        				xhr.get({
                        				    url: Endereco() + "/api/Coordenacao/getAlunosControleFalta?cd_controle_faltas=" + item.cd_controle_faltas,
                        					preventCache: true,
                        					handleAs: "json",
                        					headers: {
                        						"Accept": "application/json",
                        						"Content-Type": "application/json",
                        						"Authorization": Token()
                        					}
                        				}).then(function (dataItem) {
                        					try {

                        						console.log(dataItem);

                        						apresentaMensagem('apresentadorMensagemControleFalta', null);
                        						//var itensControleFalta = jQuery.parseJSON(dataItem);
                        						var itensControleFalta = dataItem.retorno;
                        						quickSortObj(itensControleFalta, 'cd_controle_faltas_aluno');
                        						var itens_aux = [];
                        						$.each(itensControleFalta,
													function (idx, value) {
														insertObjSort(itens_aux,
															"no_aluno",
															{
                                                                cd_controle_faltas_aluno: value.cd_controle_faltas_aluno,
                                                                cd_controle_faltas: value.cd_controle_faltas,
                                                                cd_aluno: value.cd_aluno,
                                                                cd_situacao_aluno_turma: value.cd_situacao_aluno_turma,
                                                                id_assinatura: value.id_assinatura,
                                                                no_aluno: value.no_aluno,
                                                                nm_faltas: value.nm_faltas
															});
													});
                        						var gridAluno = dijit.byId('gridAluno');
                                                gridAluno.setStore(new dojo.data.ObjectStore({
                        							objectStore: new dojo.store.Memory({ data: itens_aux })
                        						}));
                        						//gridItemControleFalta.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensControleFalta }) }));
                                                gridAluno.update();

                        						IncluirAlterar(0,
													'divAlterarControleFalta',
													'divIncluirControleFalta',
													'divExcluirControleFalta',
													'apresentadorMensagemControleFalta',
													'divCancelarControleFalta',
													'divLimparControleFalta');

                        						showCarregando();

                        					} catch (er) {
                        						postGerarLog(er);
                        					}
                        				},
											function (error) {
												apresentaMensagem('apresentadorMensagemControleFalta', error);
											});

                        				//getItensControleFalta(item, xhr);


                        			
                        			
                        	} catch (e) {
                        		postGerarLog(e);
                        	}
                        },true);

                        require(["dojo/aspect"],
                            function(aspect) {
                        	    aspect.after(gridControleFalta,
                                    "_onFetchComplete",
                                    function() {
                                        // Configura o check de todos:
                                	    if (dojo.byId('selecionaTodosControleFalta').type == 'text')
                                            setTimeout(
                                                "configuraCheckBox(false, 'cd_controle_faltas', 'selecionadoControleFalta', -1, 'selecionaTodosControleFalta', 'selecionaTodosControleFalta', 'gridControleFalta')",
                                                gridControleFalta.rowsPerPage * 3);
                                    });
                            });
            	    /*Fim Grid Controle Faltas*/

                    /**
                     * Grid Aluno
                     */
                        var gridAluno = new EnhancedGrid({
                        store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
                        structure:
                        [
                            { name: "<input id='selecionaTodosAluno' style='display:none'/>", field: "selecionadoAluno", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAluno },
                            { name: "Nome", field: "no_aluno", width: "40%" },
                            { name: "Situação", field: "cd_situacao_aluno_turma", width: "20%", formatter: situacaoFormatter },
                            { name: "Faltas", field: "nm_faltas", width: "13%", styles: "text-align: center;" },
                            { name: "Assinou", field: "id_assinatura", width: "15%", styles: "text-align: center;", formatter: assinaturaFormatter }
                        ],
                        canSort: true,
                        noDataMessage: "Nenhum registro encontrado.",
                        contentEditable: false,
                    }, "gridAluno");
                    gridAluno.startup();
                    gridAluno.canSort = function (col) { return false };

                    dijit.byId("tagAlunos").on("show", function (e) {
                        dijit.byId("gridAluno").update();
                    });

                    /*Fim Grid Aluno*/
                     

                    var menuAluno = new DropDownMenu({ style: "height: 25px" });
                    var acaoExcluirAluno = new MenuItem({
                        label: "Excluir",
                        onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno")); }
                    });

                    menuAluno.addChild(acaoExcluirAluno);

                    var acaoAssinaturaAluno = new MenuItem({
                        label: "Assinatura",
                        onClick: function () {
                            alunoAssinatura(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno"));

                        }
                    });
                    menuAluno.addChild(acaoAssinaturaAluno);

                    var buttonAluno = new DropDownButton({
                        label: "Ações Relacionadas",
                        name: "acoesRelacionadasAluno",
                        dropDown: menuAluno,
                        id: "acoesRelacionadasAluno"
                    });
                    dojo.byId("linkAcoesAluno").appendChild(buttonAluno.domNode);


                    showCarregando();

                } catch (e) {
                    postGerarLog(e);
                }

            
            });

        });
}


function alunoAssinatura(Memory, ObjectStore, nomeId, grid) {
    try {
        grid.store.save();
        var dados = grid.store.objectStore.data;

        if (grid.itensSelecionados != null && grid.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
        if (grid.itensSelecionados != null && grid.itensSelecionados.length == 0)
            caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);

        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {

            grid.itensSelecionados[0].id_assinatura = grid.itensSelecionados[0].id_assinatura == false ? true : false;

            grid.itensSelecionados = new Array();
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
            grid.setStore(dataStore);
            grid.update();
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro.', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAssinaturas(nomElement) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
        function (Memory, filteringSelect) {
            try {
                var statusStore = null;

                    statusStore = new Memory({
                        data: [
                            { name: "Todos", id: "0" },
                            { name: "Sim", id: "1" },
                            { name: "Não", id: "2" }
                        ]
                    });
                var status = new filteringSelect({
                    id: nomElement,
                    name: "Assinatura",
                    value: "0",
                    store: statusStore,
                    searchAttr: "name",
                    style: "width: 75px;"
                }, nomElement);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
};

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = CONTROLEFALTA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisaAlunoFKControleFalta();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaAlunoFKControleFalta() {
    try {
        var listaAlunosGrid = getAlunosGrid();
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var dataFinalHistorico = dojo.date.locale.parse(dojo.byId("dtIniAula").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaControleFaltaSearch?cdAlunos=" + listaAlunosGrid + "&nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=" + (hasValue(dojo.byId("cdTurmaPesTurma").value) ? dojo.byId("cdTurmaPesTurma").value : "0") + "&opcao=0" + "&dataFinalHistorico=" + (hasValue(dataFinalHistorico) ? dataFinalHistorico.toISOString(): null),
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoControleFaltaFK() {
    try {
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);

            if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("cdAlunoPes").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAlunoPes').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//function riqueridDatas(bool) {
//    dijit.byId("dtInicialComp").set("required", bool);
//    dijit.byId("dtFinalComp").set("required", bool);
//}







function abrirTurmaFK() {
    try {
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = CONTROLEFALTA;
        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId("pesProfessorFK").value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else {
                dijit.byId('pesProfessorFK').set('disabled', false);
            }
        }
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = CONTROLEFALTA;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaControleFaltaFK();

    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaControleFaltaFK(cdProfDefault) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            if (hasValue(cdProfDefault) && cdProfDefault > 0)
                cdProfessor = cdProfDefault;
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmasComPercentualFaltaSearch?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                                             "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                                            "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno +
                                            "&dtInicial=&dtFinal=&cd_turma_PPT=null&semContrato=false&dataInicial=&dataFinal=&id_percentual_faltas=true",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];
            grid.setStore(dataStore);

            if (dijit.byId("tipoTurmaFK").get('value') == PPT) {
                grid.layout.setColumnVisibility(2, true);
                grid.layout.setColumnVisibility(3, false);
                grid.turmasFilhas = true;
            }
            else {
                grid.layout.setColumnVisibility(2, false);
                grid.layout.setColumnVisibility(3, true);
                grid.turmasFilhas = false;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function retornarTurmaFKControleFalta() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("no_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dojo.byId("cdTurmaPesTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId('limparTurmaPes').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}



function abrirTurmaFKCadastro() {
    try {
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = CONTROLEFALTACADASTRO;
        dijit.byId("proTurmaFK").show();
        limparFiltrosTurmaFK();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (hasValue(dijit.byId("pesProfessorFK").value)) {
                dijit.byId('pesProfessorFK').set('disabled', true);
            } else {
                dijit.byId('pesProfessorFK').set('disabled', false);
            }
        }
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = CONTROLEFALTACADASTRO;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaControleFaltaCadastroFK();

    } catch (e) {
        postGerarLog(e);
    }
}


/**
 * /Botão turmaFK
 * @param {} cdProfDefault 
 * @returns {} 
 */
function pesquisarTurmaControleFaltaCadastroFK(cdProfDefault) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            if (hasValue(cdProfDefault) && cdProfDefault > 0)
                cdProfessor = cdProfDefault;
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmasComPercentualFaltaSearch?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                                             "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                                            "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno +
                                            "&dtInicial=&dtFinal=&cd_turma_PPT=null&semContrato=false&dataInicial=&dataFinal=&id_percentual_faltas=true",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];
            grid.setStore(dataStore);

            if (dijit.byId("tipoTurmaFK").get('value') == PPT) {
                grid.layout.setColumnVisibility(2, true);
                grid.layout.setColumnVisibility(3, false);
                grid.turmasFilhas = true;
            }
            else {
                grid.layout.setColumnVisibility(2, false);
                grid.layout.setColumnVisibility(3, true);
                grid.turmasFilhas = false;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function retornarTurmaFKControleFaltaCadastro(fieldID, fieldNome) {
    try {
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
                dojo.byId("cd_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("turma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;

                getAlunosControleFalta();
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function getAlunosControleFalta() {

    if (hasValue(dojo.byId("dtIniAula"))) {

    
    var dataControle = dojo.date.locale.parse(dojo.byId("dtIniAula").value,
         { formatLength: 'short', selector: 'date', locale: 'pt-br' });
    dataControle = dojo.date.locale.format(dataControle,
      { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
           function (dom, xhr, ref) {
               dojo.xhr.get({
                   url: Endereco() + "/api/Coordenacao/getAlunosTurmaControleFalta?cd_turma=" + dojo.byId("cd_turma").value + "&dt_inicial=&dt_final=" + dataControle + "&cd_controle_faltas=" + ((hasValue(dojo.byId("cd_controle_faltas").value) && dojo.byId("cd_controle_faltas").value > 0) ? dojo.byId("cd_controle_faltas").value: 0),
                   preventCache: true,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
               }).then(function (dataItem) {
                   try {
                       data = jQuery.parseJSON(dataItem);

                       apresentaMensagem('apresentadorMensagemControleFalta', null);
                       //var itensControleFaltas = jQuery.parseJSON(dataItem);
                       var alunosTurma = dataItem;
                       quickSortObj(alunosTurma, 'cd_aluno');
                       var alunosTurma_aux = [];
                       $.each(alunosTurma,
                           function (idx, value) {
                               insertObjSort(alunosTurma_aux,
                                   "no_aluno",
                                   {
                                       cd_controle_faltas_aluno: value.cd_controle_faltas_aluno,
                                       cd_controle_faltas: value.cd_controle_faltas,
                                       cd_aluno: value.cd_aluno,
                                       cd_situacao_aluno_turma: value.cd_situacao_aluno_turma,
                                       id_assinatura: value.id_assinatura,
                                       no_aluno: value.no_aluno,
                                       nm_faltas: value.nm_faltas
                                   });
                           });
                       var gridAluno = dijit.byId('gridAluno');
                       gridAluno.setStore(new dojo.data.ObjectStore({
                           objectStore: new dojo.store.Memory({ data: alunosTurma_aux })
                       }));
                       //gridItemControleFalta.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensControleFalta }) }));
                       gridAluno.update();

                   } catch (e) {
                       postGerarLog(e);
                   }
               },
                   function (error) {

                   });
           });

    }
}



function getAlunosCancelarControleFalta() {

    var dataControle = dojo.date.locale.parse(dojo.byId("dtIniAula").value,
         { formatLength: 'short', selector: 'date', locale: 'pt-br' });
    dataControle = dojo.date.locale.format(dataControle,
      { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
           function (dom, xhr, ref) {
               dojo.xhr.get({
                   url: Endereco() + "/api/Coordenacao/getAlunosControleFalta?cd_controle_faltas=" + dojo.byId("cd_controle_faltas").value,
                   preventCache: true,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
               }).then(function (dataItem) {
                   try {
                       //data = jQuery.parseJSON(dataItem);
                       data = dataItem.retorno;
                       apresentaMensagem('apresentadorMensagemControleFalta', null);
                       //var itensControleFaltas = jQuery.parseJSON(dataItem);
                       var alunosTurma = data;
                       quickSortObj(alunosTurma, 'no_aluno');
                       var alunosTurma_aux = [];
                       $.each(alunosTurma,
                           function (idx, value) {
                               insertObjSort(alunosTurma_aux,
                                   "no_aluno",
                                   {
                                       cd_controle_faltas_aluno: value.cd_controle_faltas_aluno,
                                       cd_controle_faltas: value.cd_controle_faltas,
                                       cd_aluno: value.cd_aluno,
                                       cd_situacao_aluno_turma: value.cd_situacao_aluno_turma,
                                       id_assinatura: value.id_assinatura,
                                       no_aluno: value.no_aluno,
                                       nm_faltas: value.nm_faltas
                                   });
                           });
                       var gridAluno = dijit.byId('gridAluno');
                       gridAluno.setStore(new dojo.data.ObjectStore({
                           objectStore: new dojo.store.Memory({ data: alunosTurma_aux })
                       }));
                       //gridItemControleFalta.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensControleFalta }) }));
                       gridAluno.update();

                   } catch (e) {
                       postGerarLog(e);
                   }
               },
                   function (error) {

                   });
           });
}



function getAlunos(item, xhr) {

   var dataControle = dojo.date.locale.parse(dojo.byId("dtIniAula").value,
        { formatLength: 'short', selector: 'date', locale: 'pt-br' });
      dataControle = dojo.date.locale.format(dataControle,
        { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
     require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
            function (dom, xhr, ref) {
                dojo.xhr.get({
                    //url: Endereco() + "/api/Coordenacao/getAlunosTurmaControleFalta?cd_turma=" + dojo.byId("cd_turma").value + "&dt_inicial=&dt_final=" + dataControle,
                    url: Endereco() + "/api/Coordenacao/getAlunosControleFalta?cd_controle_faltas=" + item.cd_controle_faltas,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataItem) {
                    try {
                        data = jQuery.parseJSON(dataItem);
                        
                            apresentaMensagem('apresentadorMensagemControleFalta', null);
                            //var itensControleFaltas = jQuery.parseJSON(dataItem);
                            var alunosTurma = dataItem;
                            quickSortObj(alunosTurma, 'cd_aluno');
                            var alunosTurma_aux = [];
                            $.each(alunosTurma,
                                function (idx, value) {
                                    insertObjSort(alunosTurma_aux,
                                        "cd_aluno",
                                        {
                                            cd_controle_faltas_aluno: value.cd_controle_faltas_aluno,
                                            cd_controle_faltas: value.cd_controle_faltas,
                                            cd_aluno: value.cd_aluno,
                                            cd_situacao_aluno_turma: value.cd_situacao_aluno_turma,
                                            id_assinatura: value.id_assinatura,
                                            no_aluno: value.no_aluno,
                                            nm_faltas: value.nm_faltas
                                        });
                                });
                            var gridAluno = dijit.byId('gridAluno');
                            gridAluno.setStore(new dojo.data.ObjectStore({
                                objectStore: new dojo.store.Memory({ data: alunosTurma_aux })
                            }));
                            //gridItemControleFalta.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensControleFalta }) }));
                            gridAluno.update();
                        
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                    function (error) {
                        
                    });
            });
}

/*Fim Botão TurmaFK */

function abrirAlunoGridFk() {
    dojo.byId('tipoRetornoAlunoFK').value = CONTROLEFALTAGRIDINCLUIR;
    dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    limparPesquisaAlunoFK();
    pesquisaAlunoFKControleFaltaIncluir();
    dijit.byId("proAluno").show();
    dijit.byId('gridPesquisaAluno').update();
}

function getAlunosGrid() {
    var listaAlunosGrid = "";

    if (hasValue(dijit.byId("gridAluno").store.objectStore.data))
        $.each(dijit.byId("gridAluno").store.objectStore.data, function (index, value) {
            listaAlunosGrid += value.cd_aluno + ",";
        });
    return listaAlunosGrid;
}

function pesquisaAlunoFKControleFaltaIncluir() {
    try {

        var listaAlunosGrid = getAlunosGrid();
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var dataFinalHistorico = dojo.date.locale.parse(dojo.byId("dtIniAula").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaControleFaltaSearch?cdAlunos=" + listaAlunosGrid + "&nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                    document.getElementById("inicioAlunoFK").checked + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                    "&cdSituacao=0&sexo=" + sexo + "&cdTurma=" + dojo.byId("cd_turma").value + "&opcao=0" + "&dataFinalHistorico=" + (hasValue(dataFinalHistorico) ? dataFinalHistorico.toISOString(): null),
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoFKGridIncluirControleFalta() {
    try {
        var valido = true;
        var gridAluno = dijit.byId("gridAluno");
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");


        if (!hasValue(gridPesquisaAluno.itensSelecionados))
            gridPesquisaAluno.itensSelecionados = [];
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("cadControleFalta").open) {
                //var storeGridControleFalta = (hasValue(gridAluno) && hasValue(gridAluno.store.objectStore.data)) ? gridAluno.store.objectStore.data : [];

                var dataControle = dojo.date.locale.parse(dojo.byId("dtIniAula").value,
                   { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                //var dataControle = dojo.byId("dtIniAula").value;
                //dataControle = dojo.date.locale.format(dataControle,
                //    { selector: "date",  formatLength: "short", locale: "pt-br" });
                require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
                    function (dom, xhr, ref) {
                        dojo.xhr.get({
                            url: Endereco() + "/api/Coordenacao/getAlunoControleFalta?cd_turma=" + dojo.byId("cd_turma").value + "&cd_aluno=" + gridPesquisaAluno.itensSelecionados[0].cd_aluno + "&dt_inicial=&dt_final=" + (hasValue(dataControle)? dataControle.toISOString(): null),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataItem) {
                            try {
                                //data = jQuery.parseJSON(dataItem);

                                //quickSortObj(gridAluno.store.objectStore.data, 'cd_aluno');
                                if (hasValue(dataItem)) {

                                    insertObjSort(gridAluno.store.objectStore.data, "cd_aluno", {

                                        cd_controle_faltas_aluno: dataItem.cd_controle_faltas_aluno,
                                        cd_controle_faltas: dataItem.cd_controle_faltas,
                                        cd_aluno: dataItem.cd_aluno,
                                        cd_situacao_aluno_turma: dataItem.cd_situacao_aluno_turma,
                                        id_assinatura: dataItem.id_assinatura,
                                        no_aluno: dataItem.no_aluno,
                                        nm_faltas: dataItem.nm_faltas
                                    });

                                    gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridAluno.store.objectStore.data }) }));

                                    return dataItem;
                                }

                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                            function (error) {

                            });
                    });

            }

        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}
function limparFormControleFaltas()
{
    try {
        var gridAluno = dijit.byId('gridAluno');
        if (hasValue(gridAluno)) {
            gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAluno.update();
        }

        dojo.byId('dtIniAula').value == null;

        dojo.byId("cd_turma").value = 0;
        dojo.byId("turma").value = "";
        dojo.byId("cd_controle_faltas").value = 0;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGridControleFaltas() {
    try {
        var gridAluno = dijit.byId('gridAluno');
        if (hasValue(gridAluno)) {
            gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAluno.update();
        }

        //var gridControleFalta = dijit.byId('gridControleFalta');
        //if (hasValue(gridControleFalta)) {
        //    gridControleFalta.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        //    gridControleFalta.update();
        //}
    }
    catch (e) {
        postGerarLog(e);
    }
}

function situacaoFormatter(id) {
    var data = [
        { name: "Movido", id: 0 },
        { name: "Ativo", id: 1 },
        { name: "Desistente", id: 2 },
        { name: "Transferido", id: 3 },
        { name: "Encerrado", id: 4 },
        { name: "Dependente", id: 5 },
        { name: "Reprovado", id: 6 },
        { name: "Remanejado", id: 7 },
        { name: "Rematriculado", id: 8 },
        { name: "Aguardando", id: 9 }
    ];

    var item = data.filter(function (item) {
        return item.id === id;
    });

    return hasValue(item) ? item[0].name : "" ;
}

function assinaturaFormatter(id) {
    var data = [
        { name: "Não", id: false },
        { name: "Sim", id: true }
        
    ];

    var item = data.filter(function (item) {
        return item.id === id;
    });

    return hasValue(item) ? item[0].name : "";
}

function formatterDateHours(data) {
    
    var data_formatada = dojo.date.locale.format(new Date(data),
        { selector: "date", datePattern: "dd/MM/yyyy HH:mm", formatLength: "short", locale: "pt-br" });
    return data_formatada;
}

function formatterDate(data) {

    var data_formatada = dojo.date.locale.format(new Date(data),
        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
    return data_formatada;
}


function IncluirControleFalta() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemControleFalta', null);
    var item = new ObjItem();

    if (hasValue(item.ControleFaltasAluno) && item.ControleFaltasAluno.length > 0) {
        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"],
            function(xhr, ref, windows) {
                if (!validarCamposControleFaltas(windows))
                    return false;

                showCarregando();

                xhr.post(Endereco() + "/api/Coordenacao/postInsertControleFaltas",
                    {
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        },
                        handleAs: "json",
                        data: ref.toJson(item)
                    }).then(function(data) {
                        try {
                            data = jQuery.parseJSON(data);
                            if (!hasValue(data.erro)) {
                                var itemAlterado = data.retorno;
                                var gridControleFalta = 'gridControleFalta';
                                var grid = dijit.byId(gridControleFalta);
                                apresentaMensagem('apresentadorMensagem', data);
                                dijit.byId("cadControleFalta").hide();
                                if (!hasValue(grid.itensSelecionados)) {
                                    grid.itensSelecionados = [];
                                }
                                insertObjSort(grid.itensSelecionados, "cd_controle_faltas", itemAlterado);
                                buscarItensSelecionados(gridControleFalta,
                                    'selecionadoControleFalta',
                                    'cd_controle_faltas',
                                    'selecionaTodosControleFalta',
                                    ['pesquisarControleFalta', 'relatorioControleFalta'],
                                    'todosItensControleFalta');
                                grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                setGridPagination(grid, itemAlterado, "cd_controle_faltas");
                                showCarregando();

                            } else {
                                apresentaMensagem('apresentadorMensagemControleFalta', data);
                                showCarregando();
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function(error) {
                        $('#aguardar').css('display', "none");
                        showCarregando();
                        apresentaMensagem('apresentadorMensagemControleFalta', error.response.data);
                    });
            });
    } else {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O Controle de Falta dever ter pelo menos 1 aluno.");
        apresentaMensagem("apresentadorMensagemControleFalta", mensagensWeb);
    }
}

function validarCamposControleFaltas(windowUtils)
{
    try {
        var validado = true;

        if (!dijit.byId("formControleFaltas").validate()) {
            validado = false;
        }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function AlterarControleFalta() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemControleFalta', null);

        var item = new ObjItem();
        item.cd_controle_faltas = dojo.byId("cd_controle_faltas").value;

        if (hasValue(item.ControleFaltasAluno) && item.ControleFaltasAluno.length > 0) {


            require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"],
                function (dom, xhr, ref, windows) {

                    if (!validarCamposControleFaltas(windows))
                        return false;

                    showCarregando();

                    xhr.post(Endereco() + "/api/Coordenacao/postEditControleFaltas",
                        {
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            handleAs: "json",
                            data: ref.toJson(item)
                        }).then(function (data) {
                            try {
                                $('#aguardar').css('display', "none");
                                data = jQuery.parseJSON(data);
                                if (!hasValue(data.erro)) {
                                    var itemAlterado = data.retorno;
                                    var gridName = 'gridControleFalta';
                                    var grid = dijit.byId(gridName);
                                    apresentaMensagem('apresentadorMensagem', data);
                                    dijit.byId("cadControleFalta").hide();
                                    if (!hasValue(grid.itensSelecionados)) {
                                        grid.itensSelecionados = [];
                                    }
                                    removeObjSort(grid.itensSelecionados, "cd_controle_faltas", dom.byId("cd_controle_faltas").value);
                                    insertObjSort(grid.itensSelecionados, "cd_controle_faltas", itemAlterado);
                                    buscarItensSelecionados(gridName,
                                        'selecionadoControleFalta',
                                        'cd_controle_faltas',
                                        'selecionaTodosControleFalta',
                                        ['pesquisarControleFalta', 'relatorioControleFalta'],
                                        'todosItensControleFalta');
                                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                    setGridPagination(grid, itemAlterado, "cd_controle_faltas");
                                    showCarregando();
                                } else {
                                    apresentaMensagem('apresentadorMensagemControleFalta', data);
                                    showCarregando();
                                }
                            } catch (er) {
                                postGerarLog(er);
                            }
                        },
                        function (error) {
                            showCarregando();
                            apresentaMensagem('apresentadorMensagemControleFalta', error.response.data);
                        });
                });

        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "O Controle de falta dever ter pelo menos 1 aluno.");
            apresentaMensagem("apresentadorMensagemControleFalta", mensagensWeb);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ObjItem() {
    try {
        this.cd_controle_faltas = null;
        this.cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;

        this.dt_controle_faltas = dojo.date.locale.parse(dojo.byId('dtIniAula').value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dt_controle_faltas = dojo.date.locale.format(this.dt_controle_faltas,
            { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });

        this.dh_controle_faltas = dojo.date.locale.format(new Date(),
            { selector: "date", datePattern: "MM/dd/yyyy HH:mm", formatLength: "short", locale: "pt-br" });

        this.cd_usuario = 0;
        this.ControleFaltasAluno = hasValue(dijit.byId("gridAluno").store.objectStore.data) ? dijit.byId("gridAluno").store.objectStore.data : null;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function eventoEditarControleFalta(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            limparFormControleFaltas();
            var gridControleFalta = dijit.byId("gridControleFalta");


            dojo.byId("cd_controle_faltas").value = itensSelecionados[0].cd_controle_faltas;
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(null, itensSelecionados[0], gridControleFalta, false);
            dijit.byId("cadControleFalta").show();

            xhr.get({
                url: Endereco() + "/api/Coordenacao/getAlunosControleFalta?cd_controle_faltas=" + itensSelecionados[0].cd_controle_faltas,
                preventCache: true,
                handleAs: "json",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json",
                    "Authorization": Token()
                }
            }).then(function (dataItem) {
                try {

                    console.log(dataItem);

                    apresentaMensagem('apresentadorMensagemControleFalta', null);
                    //var itensControleFalta = jQuery.parseJSON(dataItem);
                    var itensControleFalta = dataItem.retorno;
                    quickSortObj(itensControleFalta, 'cd_controle_faltas_aluno');
                    var itens_aux = [];
                    $.each(itensControleFalta,
                        function (idx, value) {
                            insertObjSort(itens_aux,
                                "cd_controle_faltas_aluno",
                                {
                                    cd_controle_faltas_aluno: value.cd_controle_faltas_aluno,
                                    cd_controle_faltas: value.cd_controle_faltas,
                                    cd_aluno: value.cd_aluno,
                                    cd_situacao_aluno_turma: value.cd_situacao_aluno_turma,
                                    id_assinatura: value.id_assinatura,
                                    no_aluno: value.no_aluno,
                                    nm_faltas: value.nm_faltas
                                });
                        });
                    var gridAluno = dijit.byId('gridAluno');
                    gridAluno.setStore(new dojo.data.ObjectStore({
                        objectStore: new dojo.store.Memory({ data: itens_aux })
                    }));
                    //gridItemControleFalta.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itensControleFalta }) }));
                    gridAluno.update();

                    IncluirAlterar(0,
                        'divAlterarControleFalta',
                        'divIncluirControleFalta',
                        'divExcluirControleFalta',
                        'apresentadorMensagemControleFalta',
                        'divCancelarControleFalta',
                        'divLimparControleFalta');

                    showCarregando();

                } catch (er) {
                    postGerarLog(er);
                }
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagemControleFalta', error);
                });

            
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}



function getItensControleFaltas(item, xhr) {
    try {
        var gridAluno = dijit.byId('gridAluno');
        xhr.get({
            url: Endereco() + "/api/Coordenacao/getAlunosControleFalta?cd_controle_faltas=" + item.cd_controle_faltas,
            preventCache: true,
            handleAs: "json",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json",
                "Authorization": Token()
            }
        }).then(function (dataItem) {
            try {

                console.log(dataItem);
                apresentaMensagem('apresentadorMensagemControleFalta', null);
                var itensAluno = dataItem.retorno;
                quickSortObj(itensAluno, 'cd_controle_faltas_aluno');
                var alunos_aux = [];
                $.each(itensAluno, function (idx, value) {
                    insertObjSort(alunos_aux, "cd_controle_faltas_aluno", {
                        cd_controle_faltas_aluno: value.cd_controle_faltas_aluno,
                        cd_controle_faltas: value.cd_controle_faltas,
                        cd_aluno: value.cd_aluno,
                        cd_situacao_aluno_turma: value.cd_situacao_aluno_turma,
                        id_assinatura: value.id_assinatura,
                        no_aluno: value.no_aluno,
                        nm_faltas: value.nm_faltas
                    });
                });
                var gridAluno = dijit.byId('gridAluno');
                gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: alunos_aux }) }));
                gridAluno.update();

                IncluirAlterar(0, 'divAlterarControleFalta', 'divIncluirControleFalta', 'divExcluirControleFalta', 'apresentadorMensagemControleFalta', 'divCancelarControleFalta', 'divLimparControleFalta');

            } catch (er) {
                postGerarLog(er);
            }
        },
       function (error) {
           apresentaMensagem('apresentadorMensagemControleFalta', error);
       });
    }
    catch (e) {
        postGerarLog(e);
    }

}



function keepValues(Form, value, grid, ehLink) {
    try {
        getLimpar('#formControleFaltas');
        clearForm('formControleFaltas');
        limparGridControleFaltas();
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');


        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        dojo.byId('dtIniAula').value = formatterDate(value.dt_controle_faltas);
        dijit.byId('dt_atual').set("value", formatterDateHours(value.dh_controle_faltas));
        dojo.byId("cd_turma").value = value.cd_turma;
        dojo.byId("turma").value = value.no_turma;

        dojo.byId("atendente").value = value.no_usuario;
        dojo.byId("cd_usuario").value = value.cd_usuario;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function PesquisarControleFalta(limparItens) {

   var controleFalta = montarParametrosUrlControleFalta();
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/Coordenacao/getControleFaltasSearch?desc=&cd_turma=" + (controleFalta.cd_turma || 0) + "&cd_aluno="+ (controleFalta.cd_aluno || 0) +"&assinatura="+ (controleFalta.assinatura || 0) + "&dataIni="+ (controleFalta.dataIni || "")+"&dataFim=" + (controleFalta.dataFim || ""),
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_controle_faltas"
                    }
                ), Memory({ idProperty: "cd_controle_faltas" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridControleFalta = dijit.byId("gridControleFalta");
            if (limparItens) {
                gridControleFalta.itensSelecionados = [];
            }
            gridControleFalta.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarParametrosUrlControleFalta() {
    this.cd_turma = hasValue(dojo.byId("cdTurmaPesTurma").value) ? dojo.byId("cdTurmaPesTurma").value : null;
    this.cd_aluno = hasValue(dojo.byId("cdAlunoPes").value) ? dojo.byId("cdAlunoPes").value : null;
    this.assinatura = hasValue(dijit.byId("cbAssinatura").value) ? dijit.byId("cbAssinatura").value : null;

    if (hasValue(dojo.byId("dtInicial").value)) {
        this.dataIni = dojo.date.locale.parse(dojo.byId("dtInicial").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dataIni = dojo.date.locale.format(this.dataIni,
            { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
    } else {
        this.dataIni = null;
    }

    if (hasValue(dojo.byId("dtFinal").value)) {
        this.dataFim = dojo.date.locale.parse(dojo.byId("dtFinal").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        this.dataFim = dojo.date.locale.format(this.dataFim,
            { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
    } else {
        this.dataFim = null;
    }

    var ControleFaltas = function (cd_turma, cd_aluno, assinatura, dataIni, dataFim) {
        this.cd_turma = cd_turma;
        this.cd_aluno = cd_aluno;
        this.assinatura = assinatura;
        this.dataIni = dataIni;
        this.dataFim = dataFim;
    }

    /*Controle de Faltas*/
    objControleFaltas = new ControleFaltas(cd_turma, cd_aluno, assinatura, dataIni, dataFim);
    return objControleFaltas;
}


function DeletarControleFalta(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"],
        function (dom, xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                if (dojo.byId('cd_controle_faltas').value != 0) {
                    itensSelecionados = [dom.byId("cd_controle_faltas").value];

                }

            } else {

                itensSelecionados = itensSelecionados.map((function (a) { return a.cd_controle_faltas; }));
            }

            xhr.post({
                url: Endereco() + "/api/Coordenacao/PostDeleteControleFaltas",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItensControleFalta");
                        apresentaMensagem('apresentadorMensagem', data);
                        data = jQuery.parseJSON(data).retorno;
                        dijit.byId("cadControleFalta").hide();
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridControleFalta').itensSelecionados,
                                "cd_controle_faltas",
                                itensSelecionados[r].cd_controle_faltas);
                        PesquisarControleFalta(true);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarControleFalta").set('disabled', false);
                        dijit.byId("relatorioControleFalta").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    } else {
                        if (!hasValue(dojo.byId("cadControleFalta").style.display))
                            apresentaMensagem('apresentadorMensagemControleFalta', error);
                        else
                            apresentaMensagem('apresentadorMensagem', error);
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    if (!hasValue(dojo.byId("cadControleFalta").style.display))
                        apresentaMensagem('apresentadorMensagemControleFalta', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        });
}