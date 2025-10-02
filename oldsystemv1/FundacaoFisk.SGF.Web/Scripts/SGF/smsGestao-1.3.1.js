require([
    "dijit/layout/TabContainer",
    "dijit/layout/ContentPane",
    "dojo/NodeList-traverse",
    "dijit/form/TextBox",
    "dijit/Dialog",
    "dijit/form/Select",
    "dojo/_base/array",
    "dojo/_base/event",
    "dojox/validate/web",
    "dojox/validate/check",
    "dijit/form/NumberTextBox",
    "dijit/form/ValidationTextBox",
    "dijit/form/FilteringSelect",
    "dijit/form/Form",
    "dojox/form/Manager",
    "dojox/form/manager/_Mixin",
    "dojox/form/manager/_NodeMixin",
    "dojox/form/manager/_EnableMixin",
    "dojox/form/manager/_ValueMixin",
    "dojox/form/manager/_DisplayMixin",
    "dijit/TitlePane",
    "dojox/charting/Chart",
    "dojox/charting/plot2d/Columns",
    "dojox/charting/axis2d/Default",
    "dojox/charting/widget/Legend",
    "dojox/charting/themes/MiamiNice",
    "dojo/parser",
    "dojox/grid/cells"
]);
		
function selecionaTab(e) {
			var tab = dojo.query(e.target).parents('.dijitTab')[0];

			if (!hasValue(tab)) tab = dojo.query(e.target)[0];// Clicou na borda da aba

			// Tab "Parametrização do serviço de SMS"
            if (tab.getAttribute('widgetId') == 'tabParametroSMS_tablist_tabParametrosSMS') {
                $("#caixaDePesquisa").hide();
			}

            // Tab "Histórico de Envio de SMS"
			if (tab.getAttribute('widgetId') == 'tabParametroSMS_tablist_tabHistoricoSMS') {
                $("#caixaDePesquisa").show();
			}

            // Tab "Mensagens Padrões de SMS"
			if (tab.getAttribute('widgetId') == 'tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms') {
                $("#caixaDePesquisa").hide();
                //montarCadastroComporSMSPadrao();
			}

            // Tab "Status de Créditos e validade de SMS contratados"
			if (tab.getAttribute('widgetId') == 'tabParametroSMS_tablist_tabCreditosSMS') {
                $("#caixaDePesquisa").hide();
			}

            // Tab "Envio manual de SMS para aniversariantes do dia"
            if (tab.getAttribute('widgetId') == 'tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS') {
                $("#caixaDePesquisa").hide();
                abrirTabAniversariantes();
			}

            // Tab "Envio manual de SMS para devedores"
            if (tab.getAttribute('widgetId') == 'tabParametroSMS_tablist_tabEnvioManualDevedoresSms') {
                $("#caixaDePesquisa").hide();
                abrirTabDevl();
			}

		}

        function abrirTabAniversariantes() {
            try{
                sugereDataCorrente();
                    //montarAniversariantes();
            } catch (e) {
                postGerarLog(e);
            }
        }

        function abrirTabDevl() {
            try{
                    montarDevedoresManual();
            } catch (e) {
                postGerarLog(e);
            }
        }
		
		function formatCheckBoxHistorico(value, rowIndex, obj) {
				var gridName = 'gridHistoricoSms';
				var grid = dijit.byId(gridName);
				var icon;
				var id = obj.field + '_Selected_' + rowIndex;
				var todos = dojo.byId('selecionaTodosHistorico');

				if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
					var indice = binaryObjSearch(grid.itensSelecionados, "motivo", grid._by_idx[rowIndex].item.motivo);

					value = value || indice != null; 
				}
				if (rowIndex != -1)
					icon = "<input style='height:16px'  id='" + id + "'/> ";

				// Configura o check de todos:
				if (hasValue(todos) && todos.type == 'text')
					setTimeout("configuraCheckBox(false, 'motivo', 'selecionadoHistorico', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

				setTimeout("configuraCheckBox(" + value + ", 'motivo', 'selecionadoHistorico', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

				return icon;
		}

    function formatCheckBoxAniversariante(value, rowIndex, obj) {
				var gridName = 'gridAniversarioManualSms';
				var grid = dijit.byId(gridName);
				var icon;
				var id = obj.field + '_Selected_' + rowIndex;
				var todos = dojo.byId('linkSelecionadosAniversariantes');

				if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
					var indice = binaryObjSearch(grid.itensSelecionados, "motivo", grid._by_idx[rowIndex].item.motivo);

					value = value || indice != null; 
				}
				if (rowIndex != -1)
					icon = "<input style='height:16px'  id='" + id + "'/> ";

				// Configura o check de todos:
				if (hasValue(todos) && todos.type == 'text')
					setTimeout("configuraCheckBox(false, 'motivo', 'selecionadoAniversariante', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

				setTimeout("configuraCheckBox(" + value + ", 'motivo', 'selecionadoAniversariante', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

				return icon;
		}

        function formatCheckBoxDevedores(value, rowIndex, obj) {
				var gridName = 'gridDevedoresManualSms';
				var grid = dijit.byId(gridName);
				var icon;
				var id = obj.field + '_Selected_' + rowIndex;
				var todos = dojo.byId('linkSelecionadosDev');

				if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
					var indice = binaryObjSearch(grid.itensSelecionados, "motivo", grid._by_idx[rowIndex].item.motivo);

					value = value || indice != null; 
				}
				if (rowIndex != -1)
					icon = "<input style='height:16px'  id='" + id + "'/> ";

				// Configura o check de todos:
				if (hasValue(todos) && todos.type == 'text')
					setTimeout("configuraCheckBox(false, 'motivo', 'selecionadoDev', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

				setTimeout("configuraCheckBox(" + value + ", 'motivo', 'selecionadoDev', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

				return icon;
		}


		function formatCheckBoxCompor(value, rowIndex, obj) {
		    var gridName = 'gridComporSMS';
		    var grid = dijit.byId(gridName);
		    var icon;
		    var id = obj.field + '_Selected_' + rowIndex;
		    var todos = dojo.byId('selecionaTodosCompor');

		    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
		        var indice = binaryObjSearch(grid.itensSelecionados, "tipo", grid._by_idx[rowIndex].item.cd_curso);

		        value = value || indice != null; 
		    }
		    if (rowIndex != -1)
		        icon = "<input style='height:16px'  id='" + id + "'/> ";

		    // Configura o check de todos:
		    if (hasValue(todos) && todos.type == 'text')
		        setTimeout("configuraCheckBox(false, 'tipo', 'selecionadoCompor', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

		    setTimeout("configuraCheckBox(" + value + ", 'tipo', 'selecionadoCompor', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

		    return icon;
		}

        function formatCheckBoxParametroSms(value, rowIndex, obj) {
            var gridName = 'gridParametroSMS';
            var grid = dijit.byId(gridName);
            var icon;
            var id = obj.field + '_Selected_' + rowIndex;
            var todos = dojo.byId('selecionaTodos');

            if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
                var indice = binaryObjSearch(grid.itensSelecionados, "num_usu", grid._by_idx[rowIndex].item.num_usu);

                value = value || indice != null; 
            }
            if (rowIndex != -1)
                icon = "<input style='height:16px'  id='" + id + "'/> ";

            // Configura o check de todos:
            if (hasValue(todos) && todos.type == 'text')
                setTimeout("configuraCheckBox(false, 'num_usus', 'selecionadoParamSms', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

            setTimeout("configuraCheckBox(" + value + ", 'num_usu', 'selecionadoParamSms', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

            return icon;
        }

        function formatMotivo(value, rowIndex, obj) {
                if (value == 1) {
                    var motivo = "Aniversariantes";
                }
                if (value == 2) {
                    var motivo = "Cobrança";
                }
           
            return motivo;
        }

        function formatDataCad(value, rowIndex, obj)
        {
            if (hasValue(value)) {
                var valor_data = dojo.date.locale.format(new Date(value),
                    { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "long", locale: "pt-br" });

                return valor_data
            }
        }

var TODOS = 0, ESCOLHA_UMA_OPCAO = 0;

function montarSmsGestao() {

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
            "dojo/on",
            "dojox/grid/cells",
            "dojo/data/ItemFileWriteStore",
            "dijit/tree/ForestStoreModel",
            "dojox/grid/LazyTreeGrid",
            "dijit/Dialog",
            "dojo/domReady!",
            "dijit/Editor",
            "dijit/_editor/plugins/FontChoice",
            "dijit/_editor/plugins/TextColor",
            "dijit/_editor/plugins/LinkDialog",
            "dijit/_editor/plugins/ViewSource",
            "dojox/editor/plugins/ShowBlockNodes",
            "dojox/editor/plugins/Preview",
            "dojox/editor/plugins/Blockquote",
            "dijit/_editor/plugins/NewPage",
            "dojox/editor/plugins/FindReplace",
            "dojox/editor/plugins/ToolbarLineBreak",
            "dojox/editor/plugins/CollapsibleToolbar",
            "dojo/window",
        ],
        function(xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox, on, cell, ItemFileWriteStore, ForestStoreModel, LazyTreeGrid, Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar, windowUtils) {
            ready(function() {
                verificaParamsSms();

                //*** Cria a grade de Historico de Envio de SMS **\\
                var dataHist = [
                    { motivo: 'Cobrança', dt_envio: '23/01/2020 22:00:23', status: 'Enviado', lido: 'Sim', mensagem: 'Ana, seu boleto com vencimento em 10/02/2020 se encontra em atraso, favor regularize.' },
                    { motivo: 'Aniversário', dt_envio: '23/01/2020 22:00:23', status: 'Com Problemas', lido: 'Não', mensagem: 'Alexandre, nos da Fisk Unidade Uberlândia Desejamos um feliz aniversário!' },
                    { motivo: 'Aniversário', dt_envio: '23/01/2020 22:00:23', status: 'Enviado', lido: 'Sim', mensagem: 'Frodo, nos da Fisk Unidade Uberlândia Desejamos um feliz aniversário!' },
                    { motivo: 'Cobrança', dt_envio: '23/01/2020 22:00:23', status: 'Enviado', lido: 'Sim', mensagem: 'Lorena, seu boleto com vencimento em 10/02/2020 se encontra em atraso, favor regularize.' },
                    { motivo: 'Cobrança', dt_envio: '23/01/2020 22:00:23', status: 'Enviado', lido: 'Sim', mensagem: 'Mriana, seu boleto com vencimento em 10/02/2020 se encontra em atraso, favor regularize.' }
                ];
                var gridHistoricoSms = new EnhancedGrid({
                        store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataHist }) }),
                        structure:
                        [
                            { name: "<input id='selecionaTodosHistorico' style='display:none'/>", field: "selecionadoHistorico", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxHistorico },
                            { name: "Motivo", field: "motivo", width: "8%", styles: "min-width:80px;" },
                            { name: "Data de Envio", field: "dt_envio", width: "12%", styles: "min-width:80px;" },
                            { name: "Status", field: "status", width: "12%", styles: "min-width:55px;" },
                            { name: "Lida", field: "lido", width: "9%", styles: "min-width:70px;" },
                            { name: "Mensagem", field: "mensagem", width: "59%", styles: "min-width:70px;" }
                        ],
                        selectionMode: "single",
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
                                position: "button"
                            }
                        }
                    },
                    "gridHistoricoSms");
                gridHistoricoSms.pagination.plugin._paginator.plugin.connect(
                    gridHistoricoSms.pagination.plugin._paginator,
                    'onSwitchPageSize',
                    function(evt) {
                        verificaMostrarTodos(evt, gridHistoricoSms, 'motivo', 'selecionadoHistorico');
                    });
                gridHistoricoSms.canSort = function(col) { return Math.abs(col) != 1 };

                gridHistoricoSms.startup();

                montarTipos("tipoListagem", Memory);

                //*** Cria a grade de Mensagens Padrões de SMS **\\
                var dataSms = new Array();
                var gridComporSMS = new EnhancedGrid({
                        store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataSms }) }),
                        structure:
                        [
                            { name: "<input id='selecionaTodosCompor' style='display:none'/>", field: "selecionadoCompor", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCompor },
                            { name: "Data Cadastro", field: "dt_cadastro", width: "5%", styles: "min-width:12px;", formatter: formatDataCad },
                            { name: "Tipo Mensagem", field: "motivo", width: "5%", styles: "min-width:10px;", formatter: formatMotivo },
                            { name: "Mensagem", field: "mensagem", width: "40%", styles: "min-width:85px;" }
                        ],
                        selectionMode: "single",
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
                                position: "button"
                            }
                        }
                    },
                    "gridComporSMS");
                gridComporSMS.pagination.plugin._paginator.plugin.connect(
                    gridComporSMS.pagination.plugin._paginator,
                    'onSwitchPageSize',
                    function(evt) {
                        verificaMostrarTodos(evt, gridComporSMS, 'tipo', 'selecionadoHistorico');
                    });
                gridComporSMS.canSort = function(col) { return Math.abs(col) != 1 };

                gridComporSMS.startup();
                carregaGridMensagensPadrao(xhr, Memory, Array, ObjectStore, gridComporSMS);

                loadDataFiltrosTipoFunc(Memory);
                loadDataFiltrosStatusFunc(Memory);

                //*** Cria a grade de Mensagens Padrões de SMS **\\
                var dataParametro = new Array();
                var gridParametroSMS = new EnhancedGrid({
                        store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataParametro }) }),
                        structure:
                        [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoParamSms", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxParametroSms },
                            { name: "Usuário", field: "num_usu", width: "5%", styles: "min-width:12px;" },
                            { name: "Celular Principal", field: "seu_num", width: "5%", styles: "min-width:10px;" },
                            { name: "URL Básica da API", field: "url_servico", width: "40%", styles: "min-width:85px;" }
                        ],
                        selectionMode: "single",
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
                                position: "button"
                            }
                        }
                    },
                    "gridParametroSMS");
                gridParametroSMS.pagination.plugin._paginator.plugin.connect(
                    gridParametroSMS.pagination.plugin._paginator,
                    'onSwitchPageSize',
                    function(evt) {
                        verificaMostrarTodos(evt, gridParametroSMS, 'num_usu', 'selecionaTodos');
                    });
                gridParametroSMS.canSort = function(col) { return Math.abs(col) != 1 };

                gridParametroSMS.startup();
                
                carregaGridParametroSMS(xhr, Memory, Array, ObjectStore, gridParametroSMS);

                //*** Cria os botões do link de ações e Todos os Itens para gridHistoricoSms**\\
                // Adiciona link de Todos os Itens:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function() {
                        buscarTodosItens(gridHistoricoSms, 'linkSelecionados', ['btPesquisa']);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function() {
                        buscarItensSelecionados('gridHistoricoSms', 'selecionadoHistorico', 'motivo', 'selecionaTodos', ['btPesquisa'], 'linkSelecionados');
                    }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensHistorico",
                    dropDown: menu,
                    id: "linkSelecionados"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                // Adiciona link de ações para Histórico:
                var menu = new DropDownMenu({ style: "height: 25px", id: "ActionMenu" });
                var acaoExcluir = new MenuItem({
                    label: "Reenviar SMS",
                    onClick: function() {
                        //deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno"));
                    }
                });
                menu.addChild(acaoExcluir);

                // Açóes relacionadas de "historico de Envio de SMS"
                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasCurso",
                    dropDown: menu,
                    //id: "acoesRelacionadasCurso"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                new Button({
                        label: "",
                        id: "btPesquisa",
                        disabled: false,
                        iconClass: 'dijitEditorIconSearchSGF'
                    },
                    "PesquisarHistoricoSms");
                decreaseBtn(document.getElementById("btPesquisa"), '32px');

                new Button({
                        label: "Salvar",
                        id: "btnSalvar",
                        disabled: true,
                        iconClass: 'dijitEditorIconSaveSGF'
                    },
                    "btnSalvar");

                new Button({
                        label: "Editar",
                        id: "btnEditar",
                        disabled: true,
                        iconClass: 'dijitEditorIconEditSGF'
                    },
                    "btnEditar");

                new Button({
                        label: "Cadastrar Meus parâmetros",
                        id: "meusParametros",
                        iconClass: 'dijitEditorIconEditSGF',
                        onClick: function() {
                            dijit.byId('cadParametroSMS').show();
                        }
                    },
                    "meusParametros");

                new Button({
                        label: "Contratar Serviços de SMS",
                        id: "contratarServico",
                        iconClass: 'dijitEditorIconEditSGF'
                    },
                    "contratarServico");


                new Button({
                        id: "btcomporMensagemSms",
                        label: "Compor Mensagem",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            limparComporMensagem();
                            loadDataFiltrosTipoSmsCompor(Memory);
                            dijit.byId('cad').show();
                        }
                    },
                    "btcomporMensagemSms");

                new Button({
                        id: "btnEditarMensagemSms",
                        label: "Editar Mensagem",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            EditarMensagem(gridComporSMS.itensSelecionados);
                        }
                    },
                    "btnEditarMensagemSms");

                new Button({
                        id: "btnExclirMensagemSMS",
                        label: "Excluir Mensagem",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            excluirMensagemPadrao(gridComporSMS.itensSelecionados);
                        }
                    },
                    "btnExclirMensagemSMS");

                new Button({
                        id: "btnEditarParametro",
                        label: "Editar Parametro",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            EditarParametros(gridParametroSMS.itensSelecionados);
                        }
                    }, "btnEditarParametro");

                new Button({
                        id: "btnExclirParametro",
                        label: "Excluir Parametro",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            deletaParametroEscolaSms(gridParametroSMS.itensSelecionados);
                        }
                    },
                    "btnExclirParametro");

                new Button({
                        id: "btnFechar",
                        label: "Fechar",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            dijit.byId('cad').hide();
                        }
                    },
                    "fecharDet");

                new Button({
                        id: "btnSalvarMensagemSms",
                        label: "Salvar",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                        onClick: function() {
                            cadastraMensagemPadrao(xhr, Memory,Array, ObjectStore) ;
                        }
                    },
                    "btnSalvarMensagemSms");


                new Button({
                        label: "Ver Detalhes",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function() {
                            dijit.byId('cad').show();
                        }
                    },
                    "relatorioCurso");

                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoCamposMesclagemNome = new MenuItem({
                    label: "#primeironome#",
                    onClick: function() {
                        incluiTextoEditor('dc_assunto', "#primeironome#");
                    }
                });
                menu.addChild(acaoCamposMesclagemNome);
                var acaoCamposMesclagemSobrenome = new MenuItem({
                    label: "#data_vencido#",
                    onClick: function() {
                        incluiTextoEditor('dc_assunto', "#data_vencido#");
                    }
                });
                menu.addChild(acaoCamposMesclagemNome);
                var acaoCamposMesclagemSobrenome = new MenuItem({
                    label: "#valor_debito_original#",
                    onClick: function() {
                        incluiTextoEditor('dc_assunto', "#valor_debito_original#");
                    }
                });
                menu.addChild(acaoCamposMesclagemSobrenome);
                var acaoCamposMesclagemSobrenome = new MenuItem({
                    label: "#dia_mes_aniversario#",
                    onClick: function() {
                        incluiTextoEditor('dc_assunto', "#dia_mes_aniversario#");
                    }
                });
                menu.addChild(acaoCamposMesclagemSobrenome);
                var acaoCamposMesclagemSobrenome = new MenuItem({
                    label: "#idade_aniversariante#",
                    onClick: function() {
                        incluiTextoEditor('dc_assunto', "#idade_aniversariante#");
                    }
                });
                menu.addChild(acaoCamposMesclagemSobrenome);

                var button = new DropDownButton({
                    label: "Campos de Mesclagem",
                    name: "acoesCamposMesclagem",
                    dropDown: menu,
                    id: "acoesCamposMesclagem"
                });
                dojo.byId("linkCamposMesclagem").appendChild(button.domNode);

                // formulário de parametros SMS
                new Button({
                        label: "Incluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function() {
                            SalvarParametros();
                        }
                    },
                    "incluirParametro");

                new Button({
                        label: "Alterar Parametro",
                        id: "alterarParametro",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function() {
                            editParametrosDaEscola();
                        }
                    },
                    "alterarParametro");

                new Button({
                        label: "Cancelar",
                        iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                        id: "cancelarParametro",
                        onClick: function() {
                            dijit.byId("cadParametroSMS").hide();
                        }
                    },
                    "cancelarParametro");

                new Button({
                        label: "Fechar",
                        iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                        onClick: function() {
                            dijit.byId("cadParametroSMS").hide();
                        }
                    },
                    "fecharJanela");

              new Button({
                        label: "Excluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                        onClick: function() {
                            caixaDialogo(DIALOGO_CONFIRMAR,
                                '',
                                function executaRetorno() {
                                    deletarTurmas(null, xhr, ref);
                                });
                        }
                    },
                    "deleteParametro");

                // formulário de Compor Mensagem Padrão SMS
                new Button({
                        label: "Incluir",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function() {
                            cadastraMensagemPadrao(xhr, Memory, Array, ObjectStore);
                        }
                    },
                    "incluirMensagem");

                new Button({
                        label: "Alterar",
                        id: "alterarMensagem",
                        iconClass: 'dijitEditorIcon dijitEditorIconSave',
                        onClick: function() {
                            editarMensagemPadrao(xhr, Memory, Array, ObjectStore, gridComporSMS.itensSelecionados);
                        }
                    },
                    "alterarMensagem");

                new Button({
                        label: "Cancelar",
                        iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                        onClick: function() {
                            dijit.byId("cad").hide();
                        }
                    },
                    "cancelarMensagem");

                new Button({
                        label: "Fechar",
                        iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                        onClick: function() {
                            dijit.byId("cad").hide();
                        }
                    },
                    "fecharJanelaMensagem");
            });
        });

        
};

function montarAniversariantes() {

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
            "dojo/on",
            "dojox/grid/cells",
            "dojo/data/ItemFileWriteStore",
            "dijit/tree/ForestStoreModel",
            "dojox/grid/LazyTreeGrid",
            "dijit/Dialog",
            "dojo/domReady!",
            "dijit/Editor",
            "dijit/_editor/plugins/FontChoice",
            "dijit/_editor/plugins/TextColor",
            "dijit/_editor/plugins/LinkDialog",
            "dijit/_editor/plugins/ViewSource",
            "dojox/editor/plugins/ShowBlockNodes",
            "dojox/editor/plugins/Preview",
            "dojox/editor/plugins/Blockquote",
            "dijit/_editor/plugins/NewPage",
            "dojox/editor/plugins/FindReplace",
            "dojox/editor/plugins/ToolbarLineBreak",
            "dojox/editor/plugins/CollapsibleToolbar",
            "dojo/window",
        ],
        function(xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox, on, cell, ItemFileWriteStore, ForestStoreModel, LazyTreeGrid, Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar, windowUtils) {
            ready(function() {
                
                // Envio manual de SMS para aniversariantes do dia
                var dataSMSAniv = [
                    { nome: 'Eva Gina A. Berta', celular: '(99)99999-9999', dt_aniversario: '09/09/9999' },
                    { nome: 'Eduardo Costa', celular: '(99)99999-9999', dt_aniversario: '09/09/9999' },
                    { nome: 'Frodo Bolseiro', celular: '(99)99999-9999', dt_aniversario: '09/09/9999' },
                    { nome: 'Maria Angela', celular: '(99)99999-9999', dt_aniversario: '09/09/9999' },
                    { nome: 'Dalton Melo', celular: '(99)99999-9999', dt_aniversario: '09/09/9999' }
                ];
                var gridAniversarioManualSms = new EnhancedGrid({
                        store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataSMSAniv }) }),
                        structure:
                        [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoAniversariante", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAniversariante },
                            { name: "Aluno", field: "nome", width: "49%", styles: "min-width:80px;" },
                            { name: "Celular", field: "celular", width: "12%", styles: "min-width:80px;" },
                            { name: "Aniversário", field: "dt_aniversario", width: "12%", styles: "min-width:55px;" }
                        ],
                        selectionMode: "single",
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
                                position: "button"
                            }
                        }
                    },
                    "gridAniversarioManualSms");
                gridAniversarioManualSms.pagination.plugin._paginator.plugin.connect(
                    gridAniversarioManualSms.pagination.plugin._paginator,
                    'onSwitchPageSize',
                    function(evt) {
                        verificaMostrarTodos(evt, gridAniversarioManualSms, 'motivo', 'selecionaTodos');
                    });
                gridAniversarioManualSms.canSort = function(col) { return Math.abs(col) != 1 };

                gridAniversarioManualSms.startup();

                //*** Cria os botões do link de ações e Todos os Itens para gridHistoricoSms**\\
                // Adiciona link de Todos os Itens:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function() {
                        buscarTodosItens(gridAniversarioManualSms, 'linkSelecionadosAniversariantes', ['pesquisarAniversariantes']);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function() {
                        buscarItensSelecionados('gridAniversarioManualSms', 'selecionadoAniversario', 'motivo', 'selecionaTodos', ['pesquisarAniversariantes'], 'linkSelecionadosAniversariantes');
                    }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "linkSelecionadosAniversariantes",
                    dropDown: menu,
                    id: "linkSelecionadosAniversariantes"
                });
                dom.byId("linkSelecionadosAniversariantes").appendChild(button.domNode);

                // Adiciona link de ações para Histórico:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Disparar SMS",
                    onClick: function() {
                        //deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno"));
                    }
                });
                menu.addChild(acaoExcluir);

                // Açóes relacionadas de "historico de Envio de SMS"
                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "linkAcoesAniversariantes",
                    dropDown: menu,
                    //id: "acoesRelacionadasCurso"
                });
                dom.byId("linkAcoesAniversariantes").appendChild(button.domNode);

                new Button({
                        label: "Agendar Disparo dos Selecionados",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function() {
                            
                        }
                    },
                    "relatorioAniversariantes");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () { 
                        //apresentaMensagem('apresentadorMensagem', null); PesquisarMovFinan(true); 
                    }
                }, "pesquisarAniversariantes");
                decreaseBtn(document.getElementById("pesquisarAniversariantes"), '32px');
                
            });
        });

};

function montarDevedoresManual() {

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
            "dojo/on",
            "dojox/grid/cells",
            "dojo/data/ItemFileWriteStore",
            "dijit/tree/ForestStoreModel",
            "dojox/grid/LazyTreeGrid",
            "dijit/Dialog",
            "dojo/domReady!",
            "dijit/Editor",
            "dijit/_editor/plugins/FontChoice",
            "dijit/_editor/plugins/TextColor",
            "dijit/_editor/plugins/LinkDialog",
            "dijit/_editor/plugins/ViewSource",
            "dojox/editor/plugins/ShowBlockNodes",
            "dojox/editor/plugins/Preview",
            "dojox/editor/plugins/Blockquote",
            "dijit/_editor/plugins/NewPage",
            "dojox/editor/plugins/FindReplace",
            "dojox/editor/plugins/ToolbarLineBreak",
            "dojox/editor/plugins/CollapsibleToolbar",
            "dojo/window",
        ],
        function(xhr, registry, EnhancedGrid, DataGrid, Pagination, JsonRest, JsonRestStore, ObjectStore, Cache, Memory, query, domAttr, Button, TextBox, ready, DropDownButton, DropDownMenu, MenuItem, dom, DateTextBox, on, cell, ItemFileWriteStore, ForestStoreModel, LazyTreeGrid, Editor, FontChoice, TextColor, LinkDialog, ViewSource, ShowBlockNodes, Preview, Blockquote, NewPage, FindReplace, ToolbarLineBreak, CollapsibleToolbar, windowUtils) {
            ready(function() {

              

                // Envio manual de SMS para aniversariantes do dia
                var dataSMSDev = [
                    { nome: 'Eva Gina A. Berta', celular: '(99)99999-9999', status: 'Matriculado', valor_debito: '150,00', dt_vencimento: '09/09/9999' },
                    { nome: 'Eduardo Costa', celular: '(99)99999-9999', status: 'Matriculado', valor_debito: '150,00', dt_vencimento: '09/09/9999' },
                    { nome: 'Frodo Bolseiro', celular: '(99)99999-9999', status: 'Matriculado', valor_debito: '300,00', dt_vencimento: '09/09/9999' },
                    { nome: 'Maria Angela', celular: '(99)99999-9999', status: 'Remariculado', valor_debito: '150,00', dt_vencimento: '09/09/9999' },
                    { nome: 'Dalton Melo', celular: '(99)99999-9999', status: 'Ex-Aluno', valor_debito: '450,00', dt_vencimento: '09/09/9999' }
                ];
                var gridDevedoresManualSms = new EnhancedGrid({
                        store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataSMSDev }) }),
                        structure:
                        [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionadoDev", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxDevedores },
                            { name: "Aluno", field: "nome", width: "40%", styles: "min-width:80px;" },
                            { name: "Celular", field: "celular", width: "12%", styles: "min-width:80px;" },
                            { name: "Status", field: "status", width: "12%", styles: "min-width:55px;" },
                            { name: "Valor", field: "valor_debito", width: "9%", styles: "min-width:70px;" },
                            { name: "Vencimento", field: "dt_vencimento", width: "8%", styles: "min-width:70px;" }
                        ],
                        selectionMode: "single",
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
                                position: "button"
                            }
                        }
                    },
                    "gridDevedoresManualSms");
                gridDevedoresManualSms.pagination.plugin._paginator.plugin.connect(
                    gridDevedoresManualSms.pagination.plugin._paginator,
                    'onSwitchPageSize',
                    function(evt) {
                        verificaMostrarTodos(evt, gridDevedoresManualSms, 'motivo', 'selecionaTodos');
                    });
                gridDevedoresManualSms.canSort = function(col) { return Math.abs(col) != 1 };

                gridDevedoresManualSms.startup();

                //*** Cria os botões do link de ações e Todos os Itens para gridHistoricoSms**\\
                // Adiciona link de Todos os Itens:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function() {
                        buscarTodosItens(gridAniversarioManualSms, 'linkSelecionadosDev', ['pesquisarDevil']);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function() {
                        buscarItensSelecionados('gridDevedoresManualSms', 'selecionadoDev', 'motivo', 'selecionaTodos', ['pesquisarDevil'], 'linkSelecionadosDev');
                    }
                });
                menu.addChild(menuItensSelecionados);

                button = new DropDownButton({
                    label: "Todos Itens",
                    name: "linkSelecionadosDev",
                    dropDown: menu,
                    id: "linkSelecionadosDev"
                });
                dom.byId("linkSelecionadosDev").appendChild(button.domNode);

                // Adiciona link de ações para Histórico:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Disparar SMS",
                    onClick: function() {
                        //deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno"));
                    }
                });
                menu.addChild(acaoExcluir);

                // Açóes relacionadas de "historico de Envio de SMS"
                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "linkAcoesDev",
                    dropDown: menu,
                    //id: "acoesRelacionadasCurso"
                });
                dom.byId("linkAcoesDev").appendChild(button.domNode);

                new Button({
                        label: "Compor Mensagem e Disparar",
                        iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                        onClick: function() {
                            
                        }
                    },
                    "relatorioDevil");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () { 
                        //apresentaMensagem('apresentadorMensagem', null); PesquisarMovFinan(true); 
                    }
                }, "pesquisarDevil");
                decreaseBtn(document.getElementById("pesquisarDevil"), '32px');

                
            });
        });

};


            function montarTipos(nomElement, Memory) {
                try {
                    var dados = [{ name: "Todos", id: "0" },
                                 { name: "Alunos ativos(atuais)", id: "1" },
                                 { name: "Alunos Desistentes", id: "2" },
                                 { name: "Ex Alunos", id: "3" },
                                 { name: "Clientes", id: "4" },
                                 { name: "Funcionários/Professor", id: "5" },
                    ]
                    var statusStore = new Memory({
                        data: dados
                    });
                    dijit.byId(nomElement).store = statusStore;
                    dijit.byId(nomElement).set("value", TODOS);
                    dijit.byId(nomElement).oldValue = TODOS;
                } catch (e) {
                    postGerarLog(e);
                }
            }

            function incluiTextoEditor(idEditor, texto) {
                var campo_selecao = dijit.byId(idEditor).value;
                var tag = texto;
                if (hasValue(texto))
                    dijit.byId(idEditor).set("value", campo_selecao + texto);
                else
                    caixaDialogo(DIALOGO_AVISO, msgErroApontamentoEditor, null);
            }

            function AbrirFiltros(value) {
                dijit.byId('panePesqGeral').set('open', value);
                dijit.byId('panePesqGeral').set('disabled', !value);
            }

            function loadDataFiltrosTipoSmsCompor(Memory) {
                var statusStoreSmsCompor = new Memory({
                    data: [
                        { name: "Escolha uma Opção", id: 0 },
                        { name: "Cobrança", id: 1 },
                        { name: "Aniversariantes", id: 2 }
                    ]
                });
                dijit.byId("tipoSmsCompor").store = statusStoreSmsCompor;
                dijit.byId("tipoSmsCompor").set("value", ESCOLHA_UMA_OPCAO);
            };

            function loadDataFiltrosTipoFunc(Memory) {
                var statusStoreMotivo = new Memory({
                    data: [
                        { name: "Todos", id: 0 },
                        { name: "Cobrança", id: 1 },
                        { name: "Aniversariantes", id: 2 }
                    ]
                });
                dijit.byId("pesqFuncMotivo").store = statusStoreMotivo;
                dijit.byId("pesqFuncMotivo").set("value", TODOS);
            };

            function loadDataFiltrosStatusFunc(Memory) {
                var statusStoreStatus = new Memory({
                    data: [
                        { name: "Todos", id: 0 },
                        { name: "Enviado", id: 1 },
                        { name: "Com Problemas", id: 2 }
                    ]
                });
                dijit.byId("pesqFuncStatus").store = statusStoreStatus;
                dijit.byId("pesqFuncStatus").set("value", TODOS);
                var statusStoreLida = new Memory({
                    data: [
                        { name: "Todos", id: 0 },
                        { name: "Sim", id: 1 },
                        { name: "Não", id: 2 }
                    ]
                });
                dijit.byId("pesqFuncLido").store = statusStoreLida;
                dijit.byId("pesqFuncLido").set("value", TODOS);
            };

function sugereDataCorrente() {
    try {
        dojo.xhr.post({
            url: Endereco() + "/util/PostDataHoraCorrente",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson()
        }).then(function (data) {
            if (data.indexOf("<!DOCTYPE html>") < 0) {
                var dataCorrente = jQuery.parseJSON(data).retorno;
                var dataSugerida = dataCorrente.dataPortugues.split(" ");
                if (dataSugerida.length > 0) {
                    var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    dijit.byId('dt_inicial_aniver').set("value", date);
                    dijit.byId('dt_inicial_devil').set("value", date);
                    montarAniversariantes();
                }
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemPessoa', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


//metodos C.R.U.D

function SalvarParametros(xhr, Memory,Array, ObjectStore, gridParametroSMS) 
{
    try {
        if (dijit.byId("senha").value !== dijit.byId("confirmeSenha").value) {
            caixaDialogo(DIALOGO_AVISO, 'Senha de confirmação não é igual a senha digitada. É peciso confirmar.', null);
        } else {
            var dataParametro = {
                num_usu: dijit.byId("numUsu").value,
                senha: dijit.byId("senha").value,
                seu_num: dijit.byId("seuNum").value,
                url_servico: dijit.byId("urlServico").value,
                id_automatico_devedores: dijit.byId("dispartoAutomaticoDevedoresCk").checked,
                id_automatico_aniversario: dijit.byId("dispartoAutomaticoAniversariantesCk").checked,
            };

            require([
                    "dojo/_base/xhr"
                ],
                function(xhr) {
                    xhr.post({
                        url: Endereco() + "/api/secretaria/insereParamsSms",
                        handleAs: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        },
                        postData: JSON.stringify(dataParametro)
                    }).then(function(data) {
                        var dados = JSON.parse(data);
                        MontaTela(dados);
                        montarSmsGestao();
                    });
                });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function MontaTela(dados) {
    $("#tabContainer").show();
    dijit.byId("dialogParametrize").hide();
    dijit.byId('cadParametroSMS').hide();
    var gridParametroSMS = dijit.byId("gridParametroSMS");
    gridParametroSMS.setStore(new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
    gridParametroSMS.update();
}

function EditarParametros(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length == 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione um registro para Editar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para Editar.', null);
        else {
            dijit.byId("numUsu").set("value", itensSelecionados[0].num_usu);
            dijit.byId("senha").set("value", itensSelecionados[0].senha);
            dijit.byId("senha").set("value", itensSelecionados[0].senha);
            dijit.byId("confirmeSenha").set("value", itensSelecionados[0].senha);
            dijit.byId("seuNum").set("value", itensSelecionados[0].seu_num);
            dijit.byId("urlServico").set("value", itensSelecionados[0].url_servico);
            dijit.byId("dispartoAutomaticoDevedoresCk").set("value", itensSelecionados[0].id_automatico_devedores);
            dijit.byId("dispartoAutomaticoAniversariantesCk").set("value", itensSelecionados[0].id_automatico_aniversario);

            dijit.byId("cadParametroSMS").show();
            IncluirAlterar(0, 'divAlterarParam', 'divIncluirTurma', 'divExcluirParam', '', 'divCancelarParam', 'divClearParam');
        }

    } catch (e) {
        postGerarLog(e);
    }
}

function editParametrosDaEscola(xhr, Memory, Array, ObjectStore) {
    require(["dojo/store/Memory",  "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
        try {
            var dataParametro = {
                num_usu: dijit.byId("numUsu").value,
                senha: dijit.byId("senha").value,
                seu_num: dijit.byId("seuNum").value,
                url_servico: dijit.byId("urlServico").value,
                id_automatico_devedores: dijit.byId("dispartoAutomaticoDevedoresCk").checked,
                id_automatico_aniversario: dijit.byId("dispartoAutomaticoAniversariantesCk").checked,
            };

            require([
                    "dojo/_base/xhr"
                ],
                function(xhr) {
                    xhr.post({
                        url: Endereco() + "/api/secretaria/atualizaParamsSms",
                        handleAs: "json",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json",
                            "Authorization": Token()
                        },
                        postData: JSON.stringify(dataParametro)
                    }).then(function(data) {
                        var dados = JSON.parse(data);
                        var itemAlterado = dados.retorno;

                        if (dados.retorno[0].id_automatico_aniversario == true) 
                        {
                            dijit.byId("tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").set("disabled", true);
                            //$("#tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").hide();
                        
                        } 
                        if (dados.retorno[0].id_automatico_devedores == true) 
                        {
                            dijit.byId("tabParametroSMS_tablist_tabEnvioManualDevedoresSms").set("disabled", true);
                            //$("#tabParametroSMS_tablist_tabEnvioManualDevedoresSms").hide();
                        } 
                        if (dados.retorno[0].id_automatico_devedores == false && dados.retorno[0].id_automatico_aniversario == false) 
                        {
                            dijit.byId("tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").set("disabled", true);
                            //$("#tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").hide();
                             dijit.byId("tabParametroSMS_tablist_tabEnvioManualDevedoresSms").set("disabled", false);
                            //$("#tabParametroSMS_tablist_tabEnvioManualDevedoresSms").show();
                            dijit.byId("tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").set("disabled", false);
                            //$("#tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").show();
                        
                        } 
                        if (dados.retorno[0].id_automatico_aniversario == false && dados.retorno[0].id_automatico_devedores == true) 
                        {
                            dijit.byId("tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").set("disabled", true);
                            //$("#tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").hide();
                            dijit.byId("tabParametroSMS_tablist_tabEnvioManualDevedoresSms").set("disabled", true);
                            //$("#tabParametroSMS_tablist_tabEnvioManualDevedoresSms").hide();
                        
                        } 

                        dijit.byId("cadParametroSMS").hide();
                        var gridParametroSMS = dijit.byId("gridComporSMS");
                        gridParametroSMS.setStore( new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
                        gridParametroSMS.update();

                       dijit.byId("cadParametroSMS").hide();
                    });
                });
        } catch (e) {
            postGerarLog(e);
        }
        });
    }

function limparCadParametro (Memory)
    {
        try {
            dijit.byId("numUsu").reset();
            dijit.byId("seuNum").reset();
            dijit.byId("senha").reset();
            dijit.byId("confirmeSenha").reset();
            dijit.byId("urlServico").reset();
        }
        catch (e) {
            postGerarLog(e);
        }
    }


    function verificaParamsSms() 
    {
        try {
            require(["dojo/_base/xhr", "dijit/registry", "dojo/_base/array",],
                function(xhr, registry, Array) {
                    xhr.get({
                        url: Endereco() + "/api/secretaria/VerificaParamsSms",
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function(data) {
                        dados = jQuery.parseJSON(data);
                        console.log(dados.retorno.length);
                        if (dados.retorno[0] == "undefined" || dados.retorno.length == 0) {
                            $("#content-table").hide();
                            setTimeout(function(){ showDialog(); }, 2000);
                        } 
                        
                    });
                });
        }
        catch (e)
        {
            postGerarLog(e);
        }
    }

    function showDialog() {
        dijit.byId("dialogParametrize").show();
    }

    function carregaGridParametroSMS(xhr, Memory, Array, ObjectStore, gridParametroSMS) {
        try
        {
            xhr.get({
                url: Endereco() + "/api/secretaria/listaParamsSmsEscola",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function(dados) {
                    dados = jQuery.parseJSON(dados);
                    if (dados.retorno[0].id_automatico_aniversario == true) 
                    {
                        dijit.byId("tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").set("disabled", true);
                        //$("#tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").hide();
                        
                    } 
                    if (dados.retorno[0].id_automatico_devedores == true) 
                    {
                        dijit.byId("tabParametroSMS_tablist_tabEnvioManualDevedoresSms").set("disabled", true);
                        //$("#tabParametroSMS_tablist_tabEnvioManualDevedoresSms").hide();
                    } 
                    if (dados.retorno[0].id_automatico_devedores == false && dados.retorno[0].id_automatico_aniversario == false) 
                    {
                        dijit.byId("tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").set("disabled", true);
                        //$("#tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").hide();
                         dijit.byId("tabParametroSMS_tablist_tabEnvioManualDevedoresSms").set("disabled", false);
                        //$("#tabParametroSMS_tablist_tabEnvioManualDevedoresSms").show();
                        dijit.byId("tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").set("disabled", false);
                        //$("#tabParametroSMS_tablist_tabEnvioManualAniversarintesSMS").show();
                        
                    } 
                    if (dados.retorno[0].id_automatico_aniversario == false && dados.retorno[0].id_automatico_devedores == true) 
                    {
                        dijit.byId("tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").set("disabled", true);
                        //$("#tabParametroSMS_tablist_tabMensagemPadraoAutomaticoSms").hide();
                        dijit.byId("tabParametroSMS_tablist_tabEnvioManualDevedoresSms").set("disabled", true);
                        //$("#tabParametroSMS_tablist_tabEnvioManualDevedoresSms").hide();
                        
                    } 

                    gridParametroSMS.setStore(new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
                    gridParametroSMS.update();
            });
        }
        catch (e) {
            postGerarLog(e);
        }
           
    }

    function carregaGridMensagensPadrao(xhr, Memory,Array, ObjectStore, gridComporSMS) 
    {
        try {
            xhr.get({
                url: Endereco() + "/api/secretaria/listaMensagensPadrao",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function(dados) {
                dados = jQuery.parseJSON(dados);
                gridComporSMS.setStore(new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
                gridComporSMS.update();
            });
        }
        catch (e) {
            postGerarLog(e);
        }

    }

    function deletaParametroEscolaSms(itensSelecionados) {
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"],
            function(dom, domAttr, xhr, ref) {
                try {
                    if (!hasValue(itensSelecionados) || itensSelecionados.length == 0)
                        caixaDialogo(DIALOGO_AVISO, 'Selecione um registro para Excluir.', null);
                    else if (itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para Excluir.', null);
                    else {
                        var postDataSMS = gridParametroSMS.itensSelecionados;
                        xhr.post({
                            url: Endereco() + "/api/secretaria/PostdeleteParamescolaSms",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            postData: ref.toJson(postDataSMS)
                        }).then(function(data) {
                            verificaParamsSms();
                        });

                    }
                } catch
                (e) {
                    postGerarLog(e);
                }
            });
    }


    // CRUD MENSAGENS PADRÃO DE SMS
    function cadastraMensagemPadrao(xhr, Memory, Array, ObjectStore) 
    {   
        try {

            if (dijit.byId("tipoSmsCompor").value === 0 && dijit.byId("dc_assunto").value !== " ") {
                caixaDialogo(DIALOGO_AVISO, 'Todos os campos são Obrigatórios.', null);
            } else {
                var dataParametro = {
                    motivo: dijit.byId("tipoSmsCompor").value,
                    mensagem: dijit.byId("dc_assunto").value,
                };

                require([
                        "dojo/_base/xhr"
                    ],
                    function(xhr) {
                        xhr.post({
                            url: Endereco() + "/api/secretaria/insereNovaMensagemPadrao",
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            postData: JSON.stringify(dataParametro)
                        }).then(function(data) {
                            var dados = JSON.parse(data);

                             dijit.byId("cad").hide();

                            var gridComporSMS =  dijit.byId("gridComporSMS");
                            gridComporSMS.setStore(new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
                            gridComporSMS.update();
                            
                        });
                    });
            }
        } catch (e) {
            postGerarLog(e);
        }

    }

function limparComporMensagem() {
    try {
        dojo.byId("tipoSmsCompor").value = "";
        dojo.byId("dc_assunto").value = "";
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

function EditarMensagem(itensSelecionados) {
    try {
        require([
            'dojo/store/Memory',
            'dojo/data/ObjectStore',
            'dijit/form/FilteringSelect',
            'dijit/registry',
            'dojo/domReady!'
        ], function(Memory, ObjectStore, Select, Registry) {

        if (!hasValue(itensSelecionados) || itensSelecionados.length == 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione um registro para Editar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para Editar.', null);
        else {

            //loadDataFiltrosTipoSmsCompor(itensSelecionados[0].motivo);
            //var teste = dijit.byId("tipoSmsCompor").set.store = "Cobrança";
            //dijit.byId("tipoSmsCompor").set("value", teste );

            loadSelect(itensSelecionados[0].motivo, 'tipoSmsCompor', 'motivo');

            //dijit.byId("tipoSmsCompor").set("value", itensSelecionados[0].motivo);
            dijit.byId("dc_assunto").set("value", itensSelecionados[0].mensagem);
            dijit.byId("cad").show();
            IncluirAlterar(0, 'divAlterarMensagem', 'divIncluirMensagem', 'divExcluirMensagem', '', 'divCancelarMensagem', 'divClearMensagem');
        }
        });

    } catch (e) {
        postGerarLog(e);
    }
    
}


    function editarMensagemPadrao(xhr, Memory, Array, ObjectStore, gridComporSMS) {
        try {
            require([
                'dojo/store/Memory',
                'dojo/data/ObjectStore',
                'dijit/form/FilteringSelect',
                'dijit/registry',
                'dojo/domReady!'
            ], function(Memory, ObjectStore, Select, Registry) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length == 0)
                caixaDialogo(DIALOGO_AVISO, 'Selecione um registro para Editar.', null);
            else if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para Editar.', null);
            else {

                var dataParametro = {
                    cd_escola: itensSelecionados[0].cd_escola,
                    motivo: dijit.byId("tipoSmsCompor").value,
                    mensagem: dijit.byId("dc_assunto").value,
                };

                require(["dojo/_base/xhr"],
                    function(xhr) {
                        xhr.post({
                            url: Endereco() + "/api/secretaria/atualizaMensagemPadrao",
                            preventCache: true,
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            postData: JSON.stringify(dataParametro)
                        }).then(function(data) {
                            var dados = dataMensagem;
                            if (dados != null) {
                                var dados = JSON.parse(data);

                                dijit.byId("tipoSmsCompor").set("value", dados.retorno.motivo);

                                dijit.byId("cad").hide();

                                gridComporSMS.setStore(new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
                                gridComporSMS.update();

                                dijit.byId("cad").hide();
                                //montarSmsGestao();
                            }
                        });
                    });
            }
            });
        } catch (e) {
            postGerarLog(e);
        }
    }

    function excluirMensagemPadrao(itensSelecionados) {
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/store/Memory", "dojo/data/ObjectStore"],
            function(dom, domAttr, xhr, ref, Memory, ObjectStore) {
                try {
                    if (!hasValue(itensSelecionados) || itensSelecionados.length == 0)
                        caixaDialogo(DIALOGO_AVISO, 'Selecione um registro para Excluir.', null);
                    else {
                        var dataSms = gridComporSMS.itensSelecionados;
                        xhr.post({
                            url: Endereco() + "/api/secretaria/deletaMensagemSms?motivo=" + itensSelecionados[0].motivo,
                            preventCache: true,
                            handleAs: "json",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json",
                                "Authorization": Token()
                            },
                            postData: ref.toJson(dataSms)
                        }).then(function(data) {
                            var dados = JSON.parse(data);

                            var gridComporSMS =  dijit.byId("gridComporSMS");
                            gridComporSMS.itensSelecionados = [];
                            gridComporSMS.setStore(new ObjectStore({ objectStore: new Memory({ data: dados.retorno }) }));
                            gridComporSMS.update();

                        });
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
    }
    
    
       