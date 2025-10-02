var TP_NF = 1, ICMS = 2, DADOSNF = 3, ENTRADA = 1;
var REDUZIDO = 3;
var CIDADEPES = 4, CIDADECD = 5;
var origemCidade = 0;
var REGIMEESCOLA = 0;

function mostraTabs(Permissoes) {
    require([
         "dijit/registry",
         "dojo/ready"
    ], function (registry, ready) {
        ready(function () {
            if (!possuiPermissao('tpNF', Permissoes)) {
                registry.byId('tabTpNF').set('disabled', !registry.byId('tabTpNF').get('disabled'));
                document.getElementById('tabTpNF').style.visibility = "hidden";
            }
            if (!possuiPermissao('icmsE', Permissoes)) {
                registry.byId('tabICMS').set('disabled', !registry.byId('tabICMS').get('disabled'));
                document.getElementById('tabICMS').style.visibility = "hidden";
            }
            if (!possuiPermissao('dadNF', Permissoes)) {
                registry.byId('tabDadosNFS').set('disabled', !registry.byId('tabDadosNFS').get('disabled'));
                document.getElementById('tabDadosNFS').style.visibility = "hidden";
            }
        })
    });
}

function keepValues(tipoForm, grid, ehLink) {
    try {
        var valorCancelamento = grid.selection.getSelected();

        var linkAnterior = document.getElementById('link');
        var value = null;
        if (!hasValue(ehLink, true)) {
            ehLink = eval(linkAnterior.value);
        }
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        switch (tipoForm) {
            case TP_NF: {
                limparFormTpNF(false);
                populaSituacao(value.cd_situacao_tributaria, value.id_regime_tributario);
                dojo.byId("cd_tipo_nota_fiscal").value = value.cd_tipo_nota_fiscal;
                dojo.byId("id_regime_tributario").value = value.id_regime_tributario;
                dijit.byId("dc_tp_nf").set("value", value.dc_tipo_nota_fiscal);
                dijit.byId("id_natureza_operacao").set("value", value.dc_natureza_operacao);
                dijit.byId("descCFOP").set("value", value.dc_CFOP);
                dojo.byId("cd_CFOP").value = value.cd_cfop;
                dijit.byId("cbMovto").set("value", value.id_natureza_movimento);
                dijit.byId("cbMovto").oldValue = value.id_natureza_movimento;
                dijit.byId("cd_integracao").set("value", value.nm_codigo_integracao);
                dojo.byId("id_reduzido").value = value.pc_reducao;
                dijit.byId("id_devolucao").set("checked", value.id_devolucao);
                dijit.byId("id_mov_estoque").set("checked", value.id_movimenta_estoque);
                dijit.byId("id_mov_financeiro").set("checked", value.id_movimenta_financeiro);
                dijit.byId("id_servico").set("checked", value.id_servico);
                dijit.byId("id_ativo").set("checked", value.id_tipo_ativo);
                dijit.byId("idObs").set("value", value.tx_obs_tipo_nota);
                dijit.byId("cbCadRegimeTrib")._onChangeActive = false;
                dijit.byId("cbCadRegimeTrib").set("value", value.id_regime_tributario);
                dijit.byId("cbCadRegimeTrib")._onChangeActive = true;
                break;
            };//TIPO NOTA FISCAL
            case ICMS: {
                limparFormICMS();
                dojo.byId("cd_aliquota_uf").value = value.cd_aliquota_uf;
                loadPesquisaEstado(value.cd_localidade_estado_origem, value.cd_localidade_estado_destino);
                dojo.byId("aliquotaICMS").value = value.aliquotaICMS;

                break;
            };//ICMS por Estado
            case DADOSNF: {
                limparFormDadosNF();
                dojo.byId("cd_dados_nf").value = value.cd_dados_nf;
                dojo.byId("cd_cidade_cad_nf").value = value.cd_cidade;
                dijit.byId("desCidadeCad").set("value", value.no_cidade);
                dijit.byId("natOperacaoCad").set("value", value.dc_natureza_operacao);
                dijit.byId("dc_lista_serv").set("value", value.dc_item_servico);
                dijit.byId("cd_trib").set("value", value.dc_tributacao_municipio);
                dojo.byId("aliquotaISS").value = value.aliquotaISS;
                dijit.byId("cbCadRegimeTribDadosNF").set("value", value.id_regime_tributario);
                
            };
        }
    } catch (e) {
        postGerarLog(e);
    }
};

// inicio da formatação do grupo estoque

//Grupo Estoque
function formatCheckBoxTpNF(value, rowIndex, obj) {
    try{
        var gridName = 'gridTpNF';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTpNF');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_nota_fiscal", grid._by_idx[rowIndex].item.cd_tipo_nota_fiscal);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_nota_fiscal', 'selecionadoTpNF', -1, 'selecionaTodosTpNF', 'selecionaTodosTpNF', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_nota_fiscal', 'selecionadoTpNF', " + rowIndex + ", '" + id + "', 'selecionaTodosTpNF', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}
function selecionaTab(e) {
    var tab = dojo.query(e.target).parents('.dijitTab')[0];

    if (!hasValue(tab)) // Clicou na borda da aba:
        tab = dojo.query(e.target)[0];
    if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTpNF' && !hasValue(document.getElementById('novoTpNF').className)) {
        apresentaMensagem('apresentadorMensagem', null);
        montarCadastroEscola();
    }
    if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabICMS' && !hasValue(document.getElementById('novoICMS').className)) {
        apresentaMensagem('apresentadorMensagem', null);
        montarCadastroICMS();
    }
    if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabDadosNFS' && !hasValue(document.getElementById('novoDadosNFS').className)) {
        apresentaMensagem('apresentadorMensagem', null);
        montarCadastroDadosNFS();
    }
}

//******************************************************************Criação das grades ******************************************************************

function montarCadastroFiscal() {
    require([
         "dojo/_base/xhr",
         "dojo/dom",
         "dojox/grid/EnhancedGrid",
         "dojox/grid/enhanced/plugins/Pagination",
         "dojo/store/JsonRest",
         "dojo/data/ObjectStore",
         "dojo/store/Cache",
         "dojo/store/Memory",
         "dijit/form/Button",
         "dojo/ready",
         "dijit/form/DropDownButton",
         "dijit/DropDownMenu",
         "dijit/MenuItem",
         "dijit/form/FilteringSelect",
         "dojo/domReady!",
         "dojo/parser"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, filteringSelect) {
        ready(function () {
            try {
                montarStatus("statusTpNF");
                loadTpMovto();
                dijit.byId('tabContainer').resize();
                regimeTrib("cbRegimetTrib", "cbCadRegimeTrib");
                if (REGIMEESCOLA > 0)
                    dijit.byId("cbRegimetTrib").set("value", REGIMEESCOLA);
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getTipoNotaFiscalSearch?desc=&natOp=&inicio=false&status=1&movimento=0&devolucao=null"+
                            "&escola=false&id_regime_trib=" + REGIMEESCOLA+"&id_servico=",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_tipo_nota_fiscal"
                    }
                ), Memory({ idProperty: "cd_tipo_nota_fiscal" }));
                var gridTpNF = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodosTpNF'/>", field: "selecionadoTpNF", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTpNF },
                        { name: "Descrição", field: "dc_tipo_nota_fiscal", width: "22%", styles: "min-width:80px;" },
                        { name: "Nat. Operação", field: "dc_natureza_operacao", width: "13%", styles: "min-width:40px;" },
                        { name: "CFOP", field: "dc_CFOP", width: "5%", styles: "text-align:center; min-width:40px;" },
                        { name: "Cód. Integração", field: "nm_codigo_integracao", width: "9%", styles: "text-align:center; min-width:40px;" },
                        { name: "Natureza", field: "movimento", width: "7%", styles: "text-align:center; min-width:40px;" },
                        { name: "Sit. Trib.", field: "nmSitTrib", width: "7%", styles: "text-align:center; min-width:40px;" },
                        { name: "Movimenta Est.", field: "mtvoEstoque", width: "9%", styles: "text-align:center; min-width:40px;" },
                        { name: "Movimenta Finan.", field: "mtvoFinanc", width: "10%", styles: "text-align:center; min-width:40px;" },
                        { name: "Devolução", field: "devolucao", width: "7%", styles: "text-align:center; min-width:40px;" },
                        { name: "Ativo", field: "ativo", width: "5%", styles: "text-align:center; min-width:40px;" }
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
                            position: "button"
                        }
                    }
                }, "gridTpNF"); // make sure you have a target HTML element with this id
                gridTpNF.pagination.plugin._paginator.plugin.connect(gridTpNF.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridTpNF, 'cd_tipo_nota_fiscal', 'selecionaTodosTpNF');
                });
                gridTpNF.canSort = function (col) { return Math.abs(col) != 1; };
                gridTpNF.startup();
                gridTpNF.on("RowDblClick", function (evt) {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        var idx = evt.rowIndex,
                                  item = this.getItem(idx),
                                  store = this.store;
                        apresentaMensagem('apresentadorMensagem', '');
                        keepValues(TP_NF, gridTpNF, false);
                        dijit.byId("cadTpNF").show();
                        IncluirAlterar(0, 'divAlterarTpNF', 'divIncluirTpNF', 'divExcluirTpNF', 'apresentadorMensagemTpNF', 'divCancelarTpNF', 'divLimparTpNF');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridTpNF, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosTpNF').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_tipo_nota_fiscal', 'selecionadoTpNF', -1, 'selecionaTodosTpNF', 'selecionaTodosTpNF', 'gridTpNF')", gridTpNF.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoEditarTpNF(gridTpNF.itensSelecionados);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoRemover(gridTpNF.itensSelecionados, 'DeletarTpNF(itensSelecionados)');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasTpNF",
                    dropDown: menu,
                    id: "acoesRelacionadasTpNF"
                });
                dom.byId("linkAcoesTpNF").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridTpNF, 'todosItensTpNF', ['pesquisarTpNF', 'relatorioTpNF']);
                        PesquisarTpNF(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridTpNF', 'selecionadoTpNF', 'cd_grupo_estoque', 'selecionaTodosTpNF', ['pesquisarTpNF', 'relatorioTpNF'], 'todosItensTpNF'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensTpNF",
                    dropDown: menu,
                    id: "todosItensTpNF"
                });
                dom.byId("linkSelecionadosTpNF").appendChild(button.domNode);


                //criação de botões de persistência

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        IncluirTpNF();
                    }
                }, "incluirTpNF");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        AlterarTpNF();
                    }
                }, "alterarTpNF");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarTpNF();
                        });
                    }
                }, "deleteTpNF");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    onClick: function () {
                        limparFormTpNF(true);
                    }
                }, "limparTpNF");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValues(TP_NF, gridTpNF, null);
                    }
                }, "cancelarTpNF");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadTpNF").hide();
                    }
                }, "fecharTpNF");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        PesquisarTpNF(true);
                    }
                }, "pesquisarTpNF");
                decreaseBtn(document.getElementById("pesquisarTpNF"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            limparFormTpNF(true);
                            apresentaMensagem('apresentadorMensagem', null);
                            IncluirAlterar(1, 'divAlterarTpNF', 'divIncluirTpNF', 'divExcluirTpNF', 'apresentadorMensagemTpNF', 'divCancelarTpNF', 'divLimparTpNF');
                            dijit.byId("cadTpNF").show();
                            //populaSituacao(0, function () {
                                    
                            //});
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoTpNF");
                //----Monta o botão de Relatório----
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        var id_regime = hasValue(dijit.byId("cbRegimetTrib").value) ? dijit.byId("cbRegimetTrib").value : 0;
                        xhr.get({
                            url: Endereco() + "/api/financeiro/getUrlRelatorioTipoNotaFiscal?" + getStrGridParameters('gridTpNF') + "desc=" + dijit.byId("descTpNF").value + "&natOp=" + dijit.byId("natOp").value + "&inicio=" + dijit.byId("inicioTpNF").checked + "&status=" + retornaStatus("statusTpNF") + "&movimento=0&devolucao=null&escola=false" + "&id_regime_trib=" + parseInt(id_regime),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            try {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                }, "relatorioTpNF");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("gridCFOPFK")))
                                montarCFOPFK(function () {
                                    //selecionaCFOPFK
                                    dijit.byId("pesquisarCFOPFK").on("click", function (e) {
                                        searchCFOP(dijit.byId("cbMovto").value);
                                    });
                                    dijit.byId("selecionaCFOPFK").on("click", function (e) {
                                        retornarCFOPFK();
                                    });
                                    abrirCFOPFK(false);
                                });
                            else
                                abrirCFOPFK(false);

                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cadCFOP");
                decreaseBtn(document.getElementById("cadCFOP"), '18px');
                if (hasValue(dijit.byId("menuManual"))) {
                    var menuManual = dijit.byId("menuManual");
                    if (hasValue(menuManual.handler))
                        menuManual.handler.remove();
                    menuManual.handler = menuManual.on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323005', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['descTpNF', 'natOp', 'statusTpNF', 'cbRegimetTrib'], 'pesquisarTpNF', ready);
                showCarregando();
                dijit.byId("tabContainer_tablist_menuBtn").on("click", function () {
                    try {
                        if (hasValue(dijit.byId("tabContainer_menu")) && dijit.byId("tabContainer_menu")._created) {

                            dijit.byId("tabTpNF_stcMi").on("click", function () {
                                try {
                                    abrirTabTpNF();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323005',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("cbStTrib").on("change", function () {
                    if (hasValue(dijit.byId("cbStTrib").value) && dijit.byId("cbStTrib").value > 0) {
                        quickSortObj(dijit.byId("cbStTrib").store.data, 'id');
                        var posicao = binaryObjSearch(dijit.byId("cbStTrib").store.data, 'id', dijit.byId("cbStTrib").value);
                        if (dijit.byId("cbStTrib").store.data[posicao].formaTrib == REDUZIDO)
                            document.getElementById("trReduzido").style.display = "";
                        else
                            document.getElementById("trReduzido").style.display = "none";
                    }
                });

                dijit.byId("cbMovto").on("change", function () {
                    if (!hasValue(dijit.byId("cbMovto").value))
                        dijit.byId("cadCFOP").set("disabled", true);
                    else
                        if (dijit.byId("cbMovto").value != dijit.byId("cbMovto").oldValue) {
                            dijit.byId("descCFOP").reset();
                            dojo.byId("cd_CFOP").value = 0;
                            dijit.byId("cadCFOP").set("disabled", false);
                            dijit.byId("cbMovto").oldValue = dijit.byId("cbMovto").value;
                        }
                });

                dijit.byId("cbCadRegimeTrib").on("change", function (e) {
                    if (hasValue(e)) {
                        dijit.byId("cbCadRegimeTrib").set("disabled", false);
                        dijit.byId("cbStTrib").reset();
                        populaSituacao(0,e);
                    } else
                        dijit.byId("cbCadRegimeTrib").set("disabled", true);
                });
                
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadTpMovto() {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var stMovto = dijit.byId("cbMovto");

	        var statusStore = new Memory({
	            data: [
                { name: "Entrada", id: 1 },
                { name: "Saída", id: 2 }
	            ]
	        });
	        stMovto.store = statusStore;
	        stMovto.set("value", ENTRADA);
	        stMovto.oldValue = stMovto.value;
	        stMovto.set("required", true);
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}
function populaSituacao(cdSituacao, id_regime_tributario, pFuncao) {
    try {
        // Popula os produtos:
        var id_regime = hasValue(id_regime_tributario) ? id_regime_tributario : 0;
        dojo.xhr.get({
            url: Endereco() + "/api/Financeiro/getSituacaoTributaria?id_regime_trib=" + id_regime,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            loadSituacao(data, cdSituacao);
            if (hasValue(pFuncao))
                pFuncao.call();
        },
            function (error) {
                apresentaMensagem('apresentadorMensagemCurso', error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacao(items, cdSituacao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
	function (Memory, Array) {
	    try {
	        var itemsCb = [];
	        var cbSitTrib = dijit.byId("cbStTrib");

	        Array.forEach(items, function (value, i) {
	            itemsCb.push({ id: value.cd_situacao_tributaria, name: value.dcSituacao, formaTrib: value.id_forma_tributacao });
	        });
	        var stateStore = new Memory({
	            data: itemsCb
	        });
	        cbSitTrib.store = stateStore;
	        if (hasValue(cdSituacao) && cdSituacao > 0) {
	            cbSitTrib._onChangeActive = false;
	            cbSitTrib.set("value", cdSituacao);
	            cbSitTrib._onChangeActive = true;
	            quickSortObj(dijit.byId("cbStTrib").store.data, 'id');
	            var posicao = binaryObjSearch(dijit.byId("cbStTrib").store.data, 'id', dijit.byId("cbStTrib").value);
	            if (dijit.byId("cbStTrib").store.data[posicao].formaTrib == REDUZIDO)
	                document.getElementById("trReduzido").style.display = "";
	            else
	                document.getElementById("trReduzido").style.display = "none";
	        }
	        else
	            document.getElementById("trReduzido").style.display = "none";
	        cbSitTrib.set("required", false);
	    }
	    catch (e) {
	        postGerarLog(e);
	    }
	});
}

function PesquisarTpNF(limparItens) {
    try {
        var id_regime = hasValue(dijit.byId("cbRegimetTrib").value) ? dijit.byId("cbRegimetTrib").value : 0;
        var myStore =
           dojo.store.Cache(
               dojo.store.JsonRest({
                   target: Endereco() + "/api/financeiro/getTipoNotaFiscalSearch?desc=" + dijit.byId("descTpNF").value + "&natOp=" + dijit.byId("natOp").value + "&inicio=" + dijit.byId("inicioTpNF").checked + "&status=" + retornaStatus("statusTpNF") + "&movimento=0&devolucao=null&escola=false&id_regime_trib=" + id_regime + "&id_servico=",
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
               }
        ), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridTpNF");
        if (limparItens)
            grid.itensSelecionados = [];
        grid.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function ObjItem() {
    try {
        this.cd_tipo_nota_fiscal = dojo.byId("cd_tipo_nota_fiscal").value;
        this.dc_tipo_nota_fiscal = dijit.byId("dc_tp_nf").value;
        this.dc_natureza_operacao = dijit.byId("id_natureza_operacao").value;
        this.dc_CFOP = dojo.byId("descCFOP").value;
        this.cd_cfop = dojo.byId("cd_CFOP").value;
        this.id_natureza_movimento = dijit.byId("cbMovto").value;
        this.nm_codigo_integracao = dijit.byId("cd_integracao").value;
        this.cd_situacao_tributaria = dijit.byId("cbStTrib").value;
        this.pc_reducao = dojo.byId("id_reduzido").value;
        this.id_devolucao = dijit.byId("id_devolucao").checked;
        this.id_movimenta_estoque = dijit.byId("id_mov_estoque").checked;
        this.id_movimenta_financeiro = dijit.byId("id_mov_financeiro").checked;
        this.id_tipo_ativo = dijit.byId("id_ativo").checked;
        this.tx_obs_tipo_nota = dojo.byId("idObs").value;
        this.id_regime_tributario = dijit.byId("cbCadRegimeTrib").value;
        this.id_servico = dijit.byId("id_servico").checked;

    }
    catch (e) {
        postGerarLog(e);
    }
}

function IncluirTpNF() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemTpNF', null);
    var item = new ObjItem();

    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        if (!dijit.byId("formTpNF").validate())
            return false;

        showCarregando();

        xhr.post(Endereco() + "/api/escola/postTpNF", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(item)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridTpNF';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadTpNF").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_tipo_nota_fiscal", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoTpNF', 'cd_tipo_nota_fiscal', 'selecionaTodosTpNF', ['pesquisarTpNF', 'relatorioTpNF'], 'todosItensTpNF');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_nota_fiscal");
                    showCarregando();

                } else {
                    apresentaMensagem('apresentadorMensagemTpNF', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemTpNF', error.response.data);
        });
    });
}
function AlterarTpNF() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemTpNF', null);

        var item = new ObjItem();

        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, xhr, ref, windows) {

            if (!dijit.byId("formTpNF").validate())
                return false;

            showCarregando();
            xhr.post(Endereco() + "/api/escola/postAlterarTpNF", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(item)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridTpNF';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadTpNF").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_tipo_nota_fiscal", dom.byId("cd_tipo_nota_fiscal").value);
                        insertObjSort(grid.itensSelecionados, "cd_tipo_nota_fiscal", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoTpNF', 'cd_tipo_nota_fiscal', 'selecionaTodosTpNF', ['pesquisarTpNF', 'relatorioTpNF'], 'todosItensTpNF');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_nota_fiscal");
                        showCarregando();
                    } else {
                        apresentaMensagem('apresentadorMensagemTpNF', data);
                        showCarregando();
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTpNF', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTpNF(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(TP_NF, dijit.byId('gridTpNF'), true);
            dijit.byId("cadTpNF").show();
            IncluirAlterar(0, 'divAlterarTpNF', 'divIncluirTpNF', 'divExcluirTpNF', 'apresentadorMensagemTpNF', 'divCancelarTpNF', 'divLimparTpNF');
            
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarTpNF(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        var grade = true;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            if (dojo.byId('cd_tipo_nota_fiscal').value != 0)
                grade = false;
                itensSelecionados = [{
                    cd_tipo_nota_fiscal: dojo.byId("cd_tipo_nota_fiscal").value
                }];
        }
        xhr.post({
            url: Endereco() + "/api/Financeiro/postDeleteTpNF",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensTpNF");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadTpNF").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridTpNF').itensSelecionados, "cd_tipo_nota_fiscal", itensSelecionados[r].cd_tipo_nota_fiscal);
                    PesquisarTpNF(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarTpNF").set('disabled', false);
                    dijit.byId("relatorioTpNF").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } else
                    if (grade)
                        apresentaMensagem('apresentadorMensagem', error);
                    else {
                        apresentaMensagem('apresentadorMensagemTpNF', error);
                    }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (grade)
                apresentaMensagem('apresentadorMensagem', error);
            else {
                apresentaMensagem('apresentadorMensagemTpNF', error);
            }
        });
    })
}

function limparFormTpNF(novo) {
    try {
        dojo.byId("cd_tipo_nota_fiscal").value = 0;
        dijit.byId("dc_tp_nf").set("value", "");
        dijit.byId("id_natureza_operacao").set("value", "");
        dijit.byId("descCFOP").reset();
        dojo.byId("cd_CFOP").value = 0;
        dijit.byId("cbMovto").set("value", ENTRADA);
        dijit.byId("cbMovto").oldValue = dijit.byId("cbMovto").value;
        dijit.byId("cd_integracao").set("value", "");
        dojo.byId("id_reduzido").value = 0;
        dijit.byId("id_devolucao").set("checked", false);
        dijit.byId("id_mov_estoque").set("checked", false);
        dijit.byId("id_mov_financeiro").set("checked", false);
        dijit.byId("id_servico").set("checked", false);
        dijit.byId("id_ativo").set("checked", true);
        dijit.byId("idObs").set("value", "");
        var cbSitTrib = dijit.byId("cbStTrib");
        cbSitTrib._onChangeActive = false;
        cbSitTrib.reset();
        cbSitTrib._onChangeActive = true;
        if (novo) {
            if (REGIMEESCOLA > 0)
                dijit.byId("cbCadRegimeTrib").set("value", REGIMEESCOLA);
            if (!MasterGeral())
                dijit.byId("cbCadRegimeTrib").set("disabled", true);
            else
                dijit.byId("cbCadRegimeTrib").set("disabled", false);
        } else {
            //dijit.byId("cbCadRegimeTrib").set("disabled", true);
            dijit.byId("cbCadRegimeTrib")._onChangeActive = false;
            dijit.byId("cbCadRegimeTrib").reset();
            dijit.byId("cbCadRegimeTrib")._onChangeActive = true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//ICMS por Estado
function formatCheckBoxICMS(value, rowIndex, obj) {
    var gridName = 'gridICMS';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosICMS');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_aliquota_uf", grid._by_idx[rowIndex].item.cd_aliquota_uf);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input class='formatCheckBox'  id='" + id + "'/> ";
    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_aliquota_uf', 'selecionadoICMS', -1, 'selecionaTodosICMS', 'selecionaTodosICMS', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_aliquota_uf', 'selecionadoICMS', " + rowIndex + ", '" + id + "', 'selecionaTodosICMS', '" + gridName + "')", 2);

    return icon;
}

function montarCadastroICMS() {

    //Criação da Grade de ICMS
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
            loadPesquisaEstPes();
            //*** Cria a grade de Cursos **\\
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getAliquotaUFSearch?cdEstadoOri=0&cdEstadoDest=0&aliquota=",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_aliquota_uf"
                    }
                ), Memory({ idProperty: "cd_aliquota_uf" }));
            var gridICMS = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure:
                  [
                    { name: "<input id='selecionaTodosICMS'/>", field: "selecionadoICMS", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxICMS },
                    { name: "Origem", field: "no_estado_origem", width: "40%", styles: "min-width:80px;" },
                    { name: "Destino", field: "no_estado_destino", width: "40%", styles: "min-width:40px;" },
                    { name: "ICMS(%)", field: "aliquotaICMS", width: "15%", styles: "min-width:40px;" }
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
                        maxPageStep: 5,
                        position: "button",
                        plugins: { nestedSorting: true }
                    }
                }
            }, "gridICMS");

            gridICMS.canSort = function (col) { return Math.abs(col) != 1; };
            gridICMS.startup();
            gridICMS.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(ICMS, gridICMS, false);
                    dijit.byId("cadICMS").show();
                    IncluirAlterar(0, 'divAlterarICMS', 'divIncluirICMS', 'divExcluirICMS', 'apresentadorMensagemICMS', 'divCancelarICMS', 'divLimparICMS');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridICMS, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTpNF').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_aliquota_uf', 'selecionadoICMS', -1, 'selecionaTodosICMS', 'selecionaTodosICMS', 'gridICMS')", gridICMS.rowsPerPage * 3);
                });
            });
            //*** Cria os botões do link de ações **\\

            // Adiciona link de selecionados:
            var menuICMS = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItensICMS = new MenuItem({
                label: "Todos Itens", 
                onClick: function () {
                    buscarTodosItens(gridICMS, 'todosItensICMS', ['pesquisarICMS', 'relatorioICMS']);
                    PesquisarICMS(false);
                }
            });
            menuICMS.addChild(menuTodosItensICMS);

            var menuItensSelecionadosICMS = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridICMS', 'selecionadoICMS', 'cd_aliquota_uf', 'selecionaTodosICMS', ['pesquisarICMS', 'relatorioICMS'], 'todosItensICMS'); }
            });
            menuICMS.addChild(menuItensSelecionadosICMS);

           var buttonICMS = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensICMS",
                dropDown: menuICMS,
                id: "todosItensICMS"
            });
           dom.byId("linkSelecionadosICMS").appendChild(buttonICMS.domNode);
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        loadPesquisaEstado(0, 0);
                        dijit.byId("cadICMS").show();
                        limparFormICMS();
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarICMS', 'divIncluirICMS', 'divExcluirICMS', 'apresentadorMensagemICMS', 'divCancelarICMS', 'divLimparICMS');
                    } catch (e) {
                        postGerarLog(e);
                    }
                    
                }
            }, "novoICMS");


            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: "dijitEditorIcon dijitEditorIconRedo",
                onClick: function () {
                    IncluirICMS();
                }
            }, "incluirICMS");

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    AlterarICMS();
                }
            }, "alterarICMS");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarICMS();
                    });
                }
            }, "deleteICMS");

            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    limparFormICMS();
                }
            }, "limparICMS");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(ICMS, gridICMS, null);
                }
            }, "cancelarICMS");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadICMS").hide();
                }
            }, "fecharICMS");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/financeiro/getUrlRelatorioAliquotaUF?" + getStrGridParameters('gridICMS') + "cdEstadoOri=" + dijit.byId("estadoOri").value + "&cdEstadoDest=" + dijit.byId("estadoDes").value + "&aliquota=" + dojo.byId("aliqICMS").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioICMS");
           

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoExcluirICMS = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridICMS.itensSelecionados, 'DeletarICMS(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoExcluirICMS);

            var acaoEditarICMS = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarICMS(gridICMS.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditarICMS);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasICMS",
                dropDown: menu,
                id: "acoesRelacionadasICMS"
            });
            dom.byId("linkAcoesICMS").appendChild(button.domNode);

            adicionarAtalhoPesquisa(['estadoOri', 'estadoDes', 'aliqICMS'], 'pesquisarICMS', ready);

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarICMS(true);
                }
            }, "pesquisarICMS");
            decreaseBtn(document.getElementById("pesquisarICMS"), '32px');


            if (hasValue(document.getElementById("limparDes"))) {
                document.getElementById("limparDes").parentNode.style.minWidth = '40px';
                document.getElementById("limparDes").parentNode.style.width = '40px';
            }
        })
    });

};

function loadPesquisaEstPes() {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/financeiro/getEstadosPesq",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                try {
                    var retorno = jQuery.parseJSON(dataEstado).retorno;
                    //Origem
                    var itemsCbOri = [];
                    itemsCbOri.push({ id: 0, name: "Todos" });
                    var cbEstadoOri = dijit.byId("estadoOri");

                    Array.forEach(retorno.estadosOriPes, function (value, i) {
                        itemsCbOri.push({ id: value.cd_localidade_estado, name: value.Localidade.no_localidade });
                    });
                    var stateStoreOri = new Memory({
                        data: itemsCbOri
                    });
                    cbEstadoOri.store = stateStoreOri;
                    cbEstadoOri.set("value", 0);

                    //Destino
                    var itemsCbDes = [];
                    itemsCbDes.push({ id: 0, name: "Todos" });
                    var cbEstadoDes = dijit.byId("estadoDes");

                    Array.forEach(retorno.estadosOriPes, function (value, i) {
                        itemsCbDes.push({ id: value.cd_localidade_estado, name: value.Localidade.no_localidade });
                    });
                    var stateStoreDes = new Memory({
                        data: itemsCbDes
                    });
                    cbEstadoDes.store = stateStoreDes;
                    cbEstadoDes.set("value", 0);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
}

function PesquisarICMS(limparItens) {
    try {
        var myStore =
           dojo.store.Cache(
               dojo.store.JsonRest({
                   target: Endereco() + "/api/financeiro/getAliquotaUFSearch?cdEstadoOri=" + dijit.byId("estadoOri").value + "&cdEstadoDest=" + dijit.byId("estadoDes").value + "&aliquota=" + dojo.byId("aliqICMS").value,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
               }
        ), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridICMS");
        if (limparItens)
            grid.itensSelecionados = [];
        grid.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function limparFormICMS() {
    try {
        dojo.byId("cd_aliquota_uf").value = 0;
        dijit.byId("desEstOri").reset();
        dijit.byId("desEstDest").reset();
        dojo.byId("aliquotaICMS").value = 0;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPesquisaEstado(cdOri, cdDes) {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/GetAllEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                try {
                    var itemsCb = [];
                    var cbEstadoOri = dijit.byId("desEstOri");
                    var cbEstadoDes = dijit.byId("desEstDest");

                    Array.forEach(dataEstado.retorno, function (value, i) {
                        itemsCb.push({ id: value.cd_localidade, name: value.no_localidade });
                    });
                    var stateStore = new Memory({
                        data: itemsCb
                    });
                    cbEstadoOri.store = stateStore;
                    cbEstadoDes.store = stateStore;
                    if (hasValue(cdOri) && cdOri > 0)
                        cbEstadoOri.set("value", cdOri);
                    if (hasValue(cdDes) && cdDes > 0)
                        cbEstadoDes.set("value", cdDes);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
}

function eventoEditarICMS(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(ICMS, dijit.byId('gridICMS'), true);
            dijit.byId("cadICMS").show();
            IncluirAlterar(0, 'divAlterarICMS', 'divIncluirICMS', 'divExcluirICMS', 'apresentadorMensagemICMS', 'divCancelarICMS', 'divLimparICMS');

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function IncluirICMS() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemICMS', null);
    var item = new ObjItemICMS();

    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        if (!dijit.byId("formICMS").validate())
            return false;

        showCarregando();

        xhr.post(Endereco() + "/api/financeiro/postAliquotaUF", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(item)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridICMS';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadICMS").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_aliquota_uf", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoICMS', 'cd_aliquota_uf', 'selecionaTodosICMS', ['pesquisarICMS', 'relatorioICMS'], 'todosItensICMS');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_aliquota_uf");
                    showCarregando();

                } else {
                    apresentaMensagem('apresentadorMensagemICMS', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemICMS', error.response.data);
        });
    });
}

function ObjItemICMS() {
    try {
        this.cd_aliquota_uf = dojo.byId("cd_aliquota_uf").value;
        this.cd_localidade_estado_origem = dijit.byId("desEstOri").value;
        this.cd_localidade_estado_destino = dijit.byId("desEstDest").value;
        this.pc_aliq_icms_padrao = dijit.byId("aliquotaICMS").value;

    }
    catch (e) {
        postGerarLog(e);
    }
}

function AlterarICMS() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemICMS', null);

        var item = new ObjItemICMS();

        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, xhr, ref, windows) {

            if (!dijit.byId("formICMS").validate())
                return false;

            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postAlterarAliquotaUF", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(item)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridICMS';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadICMS").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_aliquota_uf", dom.byId("cd_aliquota_uf").value);
                        insertObjSort(grid.itensSelecionados, "cd_aliquota_uf", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoICMS', 'cd_aliquota_uf', 'selecionaTodosICMS', ['pesquisarICMS', 'relatorioICMS'], 'todosItensICMS');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_aliquota_uf");
                        showCarregando();
                    } else {
                        apresentaMensagem('apresentadorMensagemICMS', data);
                        showCarregando();
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemICMS', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarICMS(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        var grade = true;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            grade = false;
            if (dojo.byId('cd_aliquota_uf').value != 0)
                itensSelecionados = [{
                    cd_aliquota_uf: dojo.byId("cd_aliquota_uf").value
                }];
        }
        xhr.post({
            url: Endereco() + "/api/Financeiro/postDeleteAliquotaUF",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensICMS");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadICMS").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridICMS').itensSelecionados, "cd_aliquota_uf", itensSelecionados[r].cd_aliquota_uf);
                    PesquisarICMS(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarICMS").set('disabled', false);
                    dijit.byId("relatorioICMS").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } else
                    if (grade)
                        apresentaMensagem('apresentadorMensagem', error);
                    else {
                        apresentaMensagem('apresentadorMensagemICMS', error);
                    }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (grade)
                apresentaMensagem('apresentadorMensagem', error);
            else {
                apresentaMensagem('apresentadorMensagemICMS', error);
            }
        });
    })
}
// Dados UF
function formatCheckBoxDadosNFS(value, rowIndex, obj) {
    var gridName = 'gridDadosNFS';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosDadosNFS');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_dados_nf", grid._by_idx[rowIndex].item.cd_dados_nf);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_dados_nf', 'selecionadoDadosNFS', -1, 'selecionaTodosDadosNFS', 'selecionaTodosDadosNFS', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_dados_nf', 'selecionadoDadosNFS', " + rowIndex + ", '" + id + "', 'selecionaTodosDadosNFS', '" + gridName + "')", 2);

    return icon;
}


function montarCadastroDadosNFS() {

    //Criação da Grade de DadosNFS
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
            //*** Cria a grade de Cursos **\\
            regimeTrib("cbRegimetTribDadosNF", "cbCadRegimeTribDadosNF");

            decreaseBtn(document.getElementById("cadCid"), '18px');
            decreaseBtn(document.getElementById("cadCidade"), '18px');
            
            decreaseBtn(document.getElementById("limparCid"), '40px');
            var myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/financeiro/getDadosNFSearch?cdCidade=0&natOp=&aliquota=&id_regime=0",
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                       idProperty: "cd_dados_uf"
                   }
               ), Memory({ idProperty: "cd_dados_uf" }));
            var gridDadosNFS = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure:
                  [
                    { name: "<input id='selecionaTodosDadosNFS'/>", field: "selecionadoDadosNFS", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxDadosNFS },
                    { name: "Origem", field: "no_cidade", width: "30%", styles: "min-width:80px;" },
                    { name: "Nat. Operação", field: "dc_natureza_operacao", width: "19%", styles: "min-width:40px;" },
                    { name: "Item Lista", field: "dc_item_servico", width: "18%", styles: "min-width:40px;" },
                    { name: "Alíquota(%)", field: "aliquotaISS", width: "10%", styles: "min-width:40px;" },
                    { name: "Cód. Tributação", field: "dc_tributacao_municipio", width: "18%", styles: "min-width:40px;" }
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
                        maxPageStep: 5,
                        position: "button",
                        plugins: { nestedSorting: true }
                    }
                }
            }, "gridDadosNFS");

            gridDadosNFS.canSort = function (col) { return Math.abs(col) != 1; };
            gridDadosNFS.startup();
            gridDadosNFS.on("RowDblClick", function (evt) {
                try {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(DADOSNF, gridDadosNFS, false);
                    dijit.byId("cadDadosNFS").show();
                    IncluirAlterar(0, 'divAlterarDadosNF', 'divIncluirDadosNF', 'divExcluirDadosNF', 'apresentadorMensagemDadosNFS', 'divCancelarDadosNF', 'divLimparDadosNF');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridDadosNFS, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTpNF').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_aliquota_uf', 'selecionadoDadosNFS', -1, 'selecionaTodosDadosNFS', 'selecionaTodosDadosNFS', 'gridDadosNFS')", gridDadosNFS.rowsPerPage * 3);
                });
            });
            //*** Cria os botões do link de ações **\\

            // Adiciona link de selecionados:
            var menuDadosNFS = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItensDadosNFS = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    buscarTodosItens(gridDadosNFS, 'todosItensDadosNFS', ['pesquisarDadosNFS', 'relatorioDadosNFS']);
                    PesquisarDadosNF(false);
                }
            });
            menuDadosNFS.addChild(menuTodosItensDadosNFS);

            var menuItensSelecionadosDadosNFS = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridDadosNFS', 'selecionadoDadosNFS', 'cd_aliquota_uf', 'selecionaTodosDadosNFS', ['pesquisarDadosNFS', 'relatorioDadosNFS'], 'todosItensDadosNFS'); }
            });
            menuDadosNFS.addChild(menuItensSelecionadosDadosNFS);

            var buttonDadosNFS = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensDadosNFS",
                dropDown: menuDadosNFS,
                id: "todosItensDadosNFS"
            });
            dom.byId("linkSelecionadosDadosNFS").appendChild(buttonDadosNFS.domNode);
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadDadosNFS").show();
                        limparFormDadosNF();
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarDadosNF', 'divIncluirDadosNF', 'divExcluirDadosNF', 'apresentadorMensagemDadosNFS', 'divCancelarDadosNF', 'divLimparDadosNF');
                    } catch (e) {
                        postGerarLog(e);
                    }

                }
            }, "novoDadosNFS");


            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: "dijitEditorIcon dijitEditorIconRedo",
                onClick: function () {
                    IncluirDadosNF();
                }
            }, "incluirDadosNF");

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    AlterarDadosNF();
                }
            }, "alterarDadosNF");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarDadosNF();
                    });
                }
            }, "deleteDadosNF");

            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    limparFormDadosNF();
                }
            }, "limparDadosNF");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(DADOSNF, gridDadosNFS, null);
                }
            }, "cancelarDadosNF");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadDadosNFS").hide();
                }
            }, "fecharDadosNF");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    var id_regime = hasValue(dijit.byId("cbRegimetTribDadosNF").value) ? dijit.byId("cbRegimetTribDadosNF").value : 0;
                    xhr.get({
                        url: Endereco() + "/api/financeiro/getUrlRelatorioDadosNF?" + getStrGridParameters('gridDadosNFS') + "cdCidade=" + dojo.byId("cd_cidade_pesq_nf").value + "&natOp=" + dijit.byId("pesNatOp").value + "&aliquota=" + dojo.byId("aliqISS").value + "&id_regime=" + id_regime,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioDadosNFS");


            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoExcluirDadosNFS = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridDadosNFS.itensSelecionados, 'DeletarDadosNF(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoExcluirDadosNFS);

            var acaoEditarDadosNFS = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarDadosNF(gridDadosNFS.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditarDadosNFS);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasDadosNFS",
                dropDown: menu,
                id: "acoesRelacionadasDadosNFS"
            });
            dom.byId("linkAcoesDadosNFS").appendChild(button.domNode);

            adicionarAtalhoPesquisa(['desc_cidade', 'pesNatOp', 'aliqISS', 'cbRegimetTribDadosNF'], 'pesquisarDadosNFS', ready);

            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarDadosNF(true);
                }
            }, "pesquisarDadosNFS");
            decreaseBtn(document.getElementById("pesquisarDadosNFS"), '32px');

            dijit.byId("cadCid").on("click", function (e) {
                if (!hasValue(dijit.byId("gridPesquisaCidade")))
                    montargridPesquisaCidade(function () {
                        origemCidade = CIDADEPES;
                        abrirPesquisaCidadeFKDadosNF();
                        dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                        dijit.byId("pesquisar").on("click", function (e) {
                            pesquisaCidadeFK();
                        });
                        
                    });
                else {
                    origemCidade = CIDADEPES;
                    abrirPesquisaCidadeFKDadosNF();
                }
            });
            dijit.byId("cadCidade").on("click", function (e) {
                if (!hasValue(dijit.byId("gridPesquisaCidade")))
                    montargridPesquisaCidade(function () {
                        origemCidade = CIDADECD;
                        abrirPesquisaCidadeFKDadosNF();
                        dojo.query("#nomeCidade").on("keyup", function (e) { if (e.keyCode == 13) pesquisaCidadeFK(); });
                        dijit.byId("pesquisar").on("click", function (e) {
                            pesquisaCidadeFK();
                        });
                        
                    });
                else {
                    origemCidade = CIDADECD;
                    abrirPesquisaCidadeFKDadosNF();
                    
                }
            });
            
            dijit.byId("limparCid").on("click", function (e) {
                dojo.byId('cd_cidade_pesq_nf').value = 0;
                dojo.byId("desc_cidade").value = "";
                dijit.byId('limparCid').set("disabled", true);
            });
        })
    });
};

function abrirPesquisaCidadeFKDadosNF() {
    try {
        limparFiltrosCidaddeFK();
        if (hasValue(dojo.byId("id_origem_cidade")))
            dojo.byId("id_origem_cidade").value = origemCidade;
        dijit.byId("paisFk").set("value", 1);
        pesquisaCidadeFK(true);
        dijit.byId("dialogConsultaCidade").show();
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarCidadeDadosNF() {
    try {
        var valido = true;
        var gridPesquisaCidade = dijit.byId("gridPesquisaCidade");
        if (!hasValue(gridPesquisaCidade.itensSelecionados) || gridPesquisaCidade.itensSelecionados.length <= 0 || gridPesquisaCidade.itensSelecionados.length > 1) {
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaCidade.itensSelecionados != null && gridPesquisaCidade.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var value = gridPesquisaCidade.itensSelecionados;
            if (value.length > 0) {
                switch (origemCidade) {
                    case CIDADEPES:
                        dijit.byId("limparCid").set("disabled", false);
                        $("#cd_cidade_pesq_nf").val(value[0].cd_cidade);
                        $("#desc_cidade").val(value[0].no_cidade);
                        break;
                    case CIDADECD:
                        $("#cd_cidade_cad_nf").val(value[0].cd_cidade);
                        $("#desCidadeCad").val(value[0].no_cidade);
                        break;
                }
            }
        }
        if (!valido)
            return false;
        dijit.byId("dialogConsultaCidade").hide();
    } catch (e) {
        postGerarLog(e);
    }
}
////////////
function PesquisarDadosNF(limparItens) {
    try {
        var id_regime = hasValue(dijit.byId("cbRegimetTribDadosNF").value) ? dijit.byId("cbRegimetTribDadosNF").value : 0;
        var myStore =
           dojo.store.Cache(
               dojo.store.JsonRest({
                   target: Endereco() + "/api/financeiro/getDadosNFSearch?cdCidade=" + dojo.byId("cd_cidade_pesq_nf").value + "&natOp=" + dijit.byId("pesNatOp").value + "&aliquota=" + dojo.byId("aliqISS").value + "&id_regime=" + id_regime,
                   handleAs: "json",
                   headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
               }
        ), dojo.store.Memory({}));
        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var grid = dijit.byId("gridDadosNFS");
        if (limparItens)
            grid.itensSelecionados = [];
        grid.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function limparFormDadosNF() {
    try {
        dojo.byId("cd_cidade_cad_nf").value = 0;
        dijit.byId("desCidadeCad").set("value", "");
        dijit.byId("natOperacaoCad").set("value", "");
        dijit.byId("dc_lista_serv").set("value", "");
        dijit.byId("cd_trib").set("value", "");
        dojo.byId("aliquotaISS").value = 0;
        if (REGIMEESCOLA > 0)
            dijit.byId("cbCadRegimeTribDadosNF").set("value", REGIMEESCOLA);
        else
            dijit.byId("cbCadRegimeTribDadosNF").reset();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPesquisaEstado(cdOri, cdDes) {
    // Popula os produtos:
    require(["dojo/_base/xhr", "dojo/store/Memory", "dojo/_base/array"],
        function (xhr, Memory, Array) {
            xhr.get({
                url: Endereco() + "/api/localidade/GetAllEstado",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataEstado) {
                try {
                    var itemsCb = [];
                    var cbEstadoOri = dijit.byId("desEstOri");
                    var cbEstadoDes = dijit.byId("desEstDest");

                    Array.forEach(dataEstado.retorno, function (value, i) {
                        itemsCb.push({ id: value.cd_localidade, name: value.no_localidade });
                    });
                    var stateStore = new Memory({
                        data: itemsCb
                    });
                    cbEstadoOri.store = stateStore;
                    cbEstadoDes.store = stateStore;
                    if (hasValue(cdOri) && cdOri > 0)
                        cbEstadoOri.set("value", cdOri);
                    if (hasValue(cdDes) && cdDes > 0)
                        cbEstadoDes.set("value", cdDes);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCidade', error);
            });
        });
}

function eventoEditarDadosNF(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(DADOSNF, dijit.byId('gridDadosNFS'), true);
            dijit.byId("cadDadosNFS").show();
            IncluirAlterar(0, 'divAlterarDadosNF', 'divIncluirDadosNF', 'divExcluirDadosNF', 'apresentadorMensagemDadosNFS', 'divCancelarDadosNF', 'divLimparDadosNF');

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function IncluirDadosNF() {
    apresentaMensagem('apresentadorMensagem', null);
    apresentaMensagem('apresentadorMensagemDadosNFS', null);
    var item = new ObjItemDadosNF();

    require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
        if (!dijit.byId("formDadosNF").validate())
            return false;

        showCarregando();

        xhr.post(Endereco() + "/api/escola/postDadosNF", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(item)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridDadosNFS';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadDadosNFS").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_dados_nf", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoDadosNFS', 'cd_dados_nf', 'selecionaTodosDadosNFS', ['pesquisarDadosNFS', 'relatorioDadosNFS'], 'todosItensDadosNFS');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_dados_nf");
                    showCarregando();

                } else {
                    apresentaMensagem('apresentadorMensagemDadosNFS', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemDadosNFS', error.response.data);
        });
    });
}

function ObjItemDadosNF() {
    try {
        this.cd_dados_nf = dojo.byId("cd_dados_nf").value;
        this.cd_cidade = dojo.byId("cd_cidade_cad_nf").value;
        this.no_cidade = dijit.byId("desCidadeCad").value;
        this.dc_natureza_operacao = dijit.byId("natOperacaoCad").value;
        this.dc_item_servico = dijit.byId("dc_lista_serv").value;
        this.dc_tributacao_municipio = dijit.byId("cd_trib").value;
        this.pc_aliquota_iss = dijit.byId("aliquotaISS").value;
        this.id_regime_tributario = dijit.byId("cbCadRegimeTribDadosNF").value;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function AlterarDadosNF() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemDadosNFS', null);

        var item = new ObjItemDadosNF();

        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (dom, xhr, ref, windows) {

            if (!dijit.byId("formDadosNF").validate())
                return false;

            showCarregando();
            xhr.post(Endereco() + "/api/escola/postAlterarDadosNF", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(item)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridDadosNFS';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadDadosNFS").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        removeObjSort(grid.itensSelecionados, "cd_dados_nf", dom.byId("cd_dados_nf").value);
                        insertObjSort(grid.itensSelecionados, "cd_dados_nf", itemAlterado);
                        buscarItensSelecionados(gridName, 'selecionadoDadosNFS', 'cd_dados_nf', 'selecionaTodosDadosNFS', ['pesquisarDadosNFS', 'relatorioDadosNFS'], 'todosItensDadosNFS');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_dados_nf");
                        showCarregando();
                    } else {
                        apresentaMensagem('apresentadorMensagemDadosNFS', data);
                        showCarregando();
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemDadosNFS', error.response.data);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function DeletarDadosNF(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        var grade = true;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            grade = false;
            if (dojo.byId('cd_dados_nf').value != 0)
                itensSelecionados = [{
                    cd_dados_nf: dojo.byId("cd_dados_nf").value
                }];
        }
        xhr.post({
            url: Endereco() + "/api/Financeiro/postDeleteDadosNF",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var todos = dojo.byId("todosItensDadosNF");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadDadosNFS").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridDadosNFS').itensSelecionados, "cd_aliquota_uf", itensSelecionados[r].cd_aliquota_uf);
                    PesquisarDadosNF(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarDadosNFS").set('disabled', false);
                    dijit.byId("relatorioDadosNFS").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } else
                    if (grade)
                        apresentaMensagem('apresentadorMensagem', error);
                    else {
                        apresentaMensagem('apresentadorMensagemDadosNFS', error);
                    }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (grade)
                apresentaMensagem('apresentadorMensagem', error);
            else {
                apresentaMensagem('apresentadorMensagemDadosNFS', error);
            }
        });
    })
}

//FK CFOP
function abrirCFOPFK() {
    try {
        apresentaMensagem('apresentadorMensagemCFOPFK', null);
        limparPesquisaCFOPFK();
        searchCFOP(dijit.byId("cbMovto").value);
        dijit.byId("fkCFOP").show();
    }
    catch (e) {
        postGerarLog(e);
    }

}

function retornarCFOPFK() {
    try {
        var valido = true;
        var gridCFOPFK = dijit.byId("gridCFOPFK");
        if (!hasValue(gridCFOPFK.itensSelecionados))
            gridCFOPFK.itensSelecionados = [];
        if (!hasValue(gridCFOPFK.itensSelecionados) || gridCFOPFK.itensSelecionados.length <= 0 || gridCFOPFK.itensSelecionados.length > 1) {
            if (gridCFOPFK.itensSelecionados != null && gridCFOPFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridCFOPFK.itensSelecionados != null && gridCFOPFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            $("#cd_CFOP").val(gridCFOPFK.itensSelecionados[0].cd_cfop);
            dijit.byId("descCFOP").set("value", gridCFOPFK.itensSelecionados[0].nm_cfop);
        }

        if (!valido)
            return false;
        dijit.byId("fkCFOP").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function regimeTrib(campoPesq, campoCad) {
    dojo.xhr.get({
        url: Endereco() + "/api/escola/getParametroRegimeTrib",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        if (hasValue(data) && hasValue(data.retorno) && data.retorno > 0)
            REGIMEESCOLA = data.retorno;

        var storeRegimeTipo = new dojo.store.Memory({
            data: [
              { name: "Simples Nacional", id: "1" },
              { name: "Simples Nacional - excesso de sublimite da receita bruta", id: "2" },
              { name: "Regime Normal", id: "3" }
            ]
        });
        criarOuCarregarCompFiltering(campoPesq, storeRegimeTipo.data, "", REGIMEESCOLA, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect,
            'id', 'name', MASCULINO);
        dijit.byId(campoCad).store = storeRegimeTipo;
        dijit.byId(campoCad).set("required", false);
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}