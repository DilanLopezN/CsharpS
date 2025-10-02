var CONTA_CORRENTE = 5;
var TRANSFERENCIA = 3, ABERTURA_SALDO = 6 , TRANSFERENCIA_MATRIZ = 10;
var TIPO_PESQUISA = 1;
var PESQUISA_CONTA_CORRENTE = 0;
var PESQUISA = 1, CADASTRO = 2, SAIDA = 2;

var EnumTipoMovimentacaoFinanceiraContaCorrente = {
	ACERTO_FECHAMENTO_CAIXA: 9
}

//#region montar drops para pesquisa entradaSaida
function montarEntradaSaida(nomElement, isPesquisa) {
    try {
        var statusStore = [];
        if (isPesquisa)
            statusStore = new dojo.store.Memory({
                data: [
                       { name: "Todos", id: 0 },
                       { name: "Entrada", id: 1 },
                       { name: "Saída", id: 2 }
                ]
            });
        else
            statusStore = new dojo.store.Memory({
                data: [
                       { name: "Entrada", id: 1 },
                       { name: "Saída", id: 2 }
                ]
            });

        var status = new dijit.form.FilteringSelect({
            id: nomElement,
            name: "status",
            value: "0",
            store: statusStore,
            required: !isPesquisa,
            searchAttr: "name",
            style: "width: 75px;"
        }, nomElement);
    }
    catch (e) {
        postGerarLog(e);
    }
};

function motarDropsPesquisaContaCorrente() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/escola/getCarregarFiltrosContaCorrente",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataCorrente) {
            var dados = $.parseJSON(dataCorrente);
            if (dados.retorno != null) {
                apresentaMensagem("apresentadorMensagem", null);
                loadMovimentacao(dados.retorno.movimentacaoFinanceira, 'cbMovimentoPes');
                loadOrigem(dados.retorno.localMovimentoOrigem, 'cbOrigemPes');
                loadDestino(dados.retorno.localMovimentoDestino, 'cbDestinoPes');
                dojo.byId('planoContaObrigatorio').value = dados.retorno.parametro.id_requer_plano_contas_mov;
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadContaCorrente").style.display))
                apresentaMensagem('apresentadorMensagemContaCorrente', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function loadMovimentacao(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try{
              var itemsCb = [];
              var cbMovimentacao = dijit.byId(field);
              Array.forEach(items, function (value, i) {
                  itemsCb.push({
                      id: value.cd_movimentacao_financeira,
                      name: value.dc_movimentacao_financeira
                  });
              });
              var stateStore = new Memory({
                  data: itemsCb
              });
              cbMovimentacao.store = stateStore;
            }
            catch (e) {
                postGerarLog(e);
            }
    });
}

function loadOrigem(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try{
                  var itemsCb = [];
                  var cbOrigem = dijit.byId(field);
                  Array.forEach(items, function (value, i) {
                      itemsCb.push({
                          id: value.cd_local_movto,
                          name: value.nomeLocal == null ? value.no_local_movto : value.nomeLocal
                      });
                  });
                  var stateStore = new Memory({
                      data: itemsCb
                  });
                  cbOrigem.store = stateStore;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function loadDestino(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
  function (Memory, Array) {
      try{
          var itemsCb = [];
          var cbTipoAtividade = dijit.byId(field);
          Array.forEach(items, function (value, i) {
              itemsCb.push({
                  id: value.cd_local_movto,
                  name: value.nomeLocal == null ? value.no_local_movto : value.nomeLocal
              });
          });
          var stateStore = new Memory({
              data: itemsCb
          });
          cbTipoAtividade.store = stateStore;
      }
      catch (e) {
          postGerarLog(e);
      }
  });
}

function motarDropsCadastroContaCorrente() {
    try{
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            xhr.get({
                url: Endereco() + "/api/financeiro/getCarregarCamposCadastroConta",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataCorrente) {
                try{
                    var dados = $.parseJSON(dataCorrente);
                    if (dados.retorno != null) {
                        apresentaMensagem("apresentadorMensagemContaCorrente", null);
                        loadMovimentacao(dados.retorno.movimentacaoFinanceira, 'idMovimentoCad');
                        loadOrigem(dados.retorno.localMovimentoOrigem, 'cbOrigemCad');
                        loadDestino(dados.retorno.localMovimentoDestino, 'cbDestinoCad');
                        criarOuCarregarCompFiltering("cbLiquidacao", dados.retorno.tiposLiquidacao, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_liquidacao', 'dc_tipo_liquidacao');
                    }
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadContaCorrente").style.display))
                    apresentaMensagem('apresentadorMensagemContaCorrente', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region montar ckecksBoxes
function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridContaCorrente';
	    var grid = dijit.byId(gridName);
	    var icon;
	    var id = obj.field + '_Selected_' + rowIndex;
	    var todos = dojo.byId('selecionaTodos');

	    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
	        var indice = binaryObjSearch(grid.itensSelecionados, "cd_conta_corrente", grid._by_idx[rowIndex].item.cd_conta_corrente);

		    value = value || indice != null; // Item está selecionado.
	    }
	    if (rowIndex != -1)
	        icon = "<input  class='formatCheckBox'  id='" + id + "'/> ";

	    // Configura o check de todos:
	    if (hasValue(todos) && todos.type == 'text')
	        setTimeout("configuraCheckBox(false, 'cd_conta_corrente', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

	    setTimeout("configuraCheckBox(" + value + ", 'cd_conta_corrente', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

	    return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region  montarCadastroContaCorrente
function montarCadastroContaCorrente() {
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
         "dojox/grid/enhanced/plugins/NestedSorting",
         "dojo/ready",
         "dojo/on",
         "dijit/form/DropDownButton",
         "dijit/DropDownMenu",
         "dijit/MenuItem",
         "dijit/Dialog"
        ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, NestedSorting, ready, on, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try{
                montarEntradaSaida('cbEntradaSaidaPes', true);
                motarDropsPesquisaContaCorrente();

                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getContaCorrenteSearch?cdOrigem=0&cdDestino=0&entraSaida=0&cdMovimento=0&cdPlanoConta=0&dta_ini=&dta_fim=",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_conta_corrente"
                    }), Memory({ idProperty: "cd_conta_corrente" }));


                var gridContaCorrente = new EnhancedGrid({
                    store: ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure: [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
					    { name: "Data", field: "dt_conta_corrente", width: "10%", styles: "min-width:80px;" },
					    { name: "Origem", field: "origem", width: "20%", styles: "min-width:80px;" },
					    { name: "Destino", field: "destino", width: "20%", styles: "min-width:80px;" },
					    { name: "E/S", field: "tipo_movimento", width: "3%", styles: "min-width:80px;text-align: center;" },
					    { name: "Movimento", field: "movimento", width: "15%", styles: "min-width:80px;" },
					    { name: "Valor", field: "vlConta_corrente", width: "7%", styles: "min-width:80px;text-align: right;" },
					    { name: "Plano", field: "planoConta", width: "15%"}

                    ],
                    noDataMessage: msgNotRegEncFiltro,
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
                }, "gridContaCorrente");

                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridContaCorrente.pagination.plugin._paginator.plugin.connect(gridContaCorrente.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridContaCorrente, 'cd_conta_corrente', 'selecionaTodos');
                });

                gridContaCorrente.canSort = function (col) { return Math.abs(col) != 1; };

                gridContaCorrente.startup();

                gridContaCorrente.on("RowDblClick", function (evt) {
                    var idx = evt.rowIndex,
                       item = this.getItem(idx),
                       store = this.store;
                    showCarregando();
                    dijit.byId("tabContainerContaCorrente_tablist_tabHistorico").domNode.style.visibility = '';
                    IncluirAlterar(0, 'divAlterarContaCorrente', 'divIncluirContaCorrente', 'divExcluirContaCorrente', 'apresentadorMensagemContaCorrente', 'divCancelarContaCorrente', 'divClearContaCorrente');
                    keepValuesContaCorrente(item, gridContaCorrente, false);
                }, true);

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                    onClick: function () {
                        incluirContaCorrente();
                    }
                }, "incluirContaCorrente");

                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        showCarregando();
                        keepValuesContaCorrente(null, dijit.byId('gridContaCorrente'), false)
                    }
                }, "cancelarContaCorrente");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                        dijit.byId("cadContaCorrente").hide();
                    }
                }, "fecharContaCorrente");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        editarContaCorrente();
                    }
                }, "alterarContaCorrente");

                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            deletarContaCorrente();
                        });
                    }
                }, "deleteContaCorrente");

                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                    onClick: function () {
                        limparFormulario();
                        getFullMovimentacao();
                    }
                }, "limparContaCorrente");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dijit.byId('dcPlanoCad').reset();
                        dojo.byId('cdFkPlanoContaCad').value = 0;
                        dijit.byId("limparPlanoContas").set('disabled', true);
                    }
                }, "limparPlanoContas");

                if (hasValue(document.getElementById("limparPlanoContas"))) {
                    document.getElementById("limparPlanoContas").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPlanoContas").parentNode.style.width = '40px';
                }

                //Fim

                //#region Botões de crud

                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {
	                        dijit.byId("dcPlanoCad").set('disabled', false);
	                        dijit.byId("pesPlanoCad").set('disabled', false);
                            limparFormulario();
                            obrigarPlanoContas();
                            var hasPlanoObrigatorio = $.parseJSON(dojo.byId('planoContaObrigatorio').value);
                            if (hasPlanoObrigatorio == true)
                                dijit.byId('dcPlanoCad').set('required', true);
                            else
                                dijit.byId('dcPlanoCad').set('required', false);

                            IncluirAlterar(1, 'divAlterarContaCorrente', 'divIncluirContaCorrente', 'divExcluirContaCorrente', 'apresentadorMensagemContaCorrente', 'divCancelarContaCorrente', 'divClearContaCorrente');
                            if (dijit.byId("idTipoCad") == undefined) {
                                montarEntradaSaida("idTipoCad", false);
                                dijit.byId('idTipoCad').set("required", true);
                            }
                            motarDropsCadastroContaCorrente();
                            dijit.byId("tabContainerContaCorrente_tablist_tabHistorico").domNode.style.visibility = 'hidden';
                            dijit.byId("cadContaCorrente").show();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novaContaCorrente");

                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        var conta = new ContaCorrenteObj(true);
                        xhr.get({
                            url: Endereco() + "/api/financeiro/getUrlRelatorioContaCorrente?" + getStrGridParameters('gridContaCorrente') + "&cdOrigem=" + conta.origem + "&cdDestino=" + conta.destino + "&entraSaida=" + conta.entraSai + "&cdMovimento=" + conta.movimento + "&cdPlanoConta=" + conta.planoConta + '&dta_ini=' + conta.dta_ini + '&dta_fim=' + conta.dta_fim,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagemContaCorrente', error);
                        });
                    }
                }, "relatorioContaCorrente");

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () {
                        pesquisarContaCorrente(true);
                    }
                }, "pesquisarContaCorrente");
                decreaseBtn(document.getElementById("pesquisarContaCorrente"), '32px');

                //#endregion

                //#region botões de FK

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            TIPO_PESQUISA = CADASTRO;
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(); },
                                    'apresentadorMensagemContaCorrente',
                                    CONTA_CORRENTE, null);
                            }
                            else {
                                clearForm("formPlanoContasFK");
                                loadPlanoContas(ObjectStore, Memory, xhr, CONTA_CORRENTE);
                                funcaoFKPlanoContas();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPlanoCad");

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            debugger
                            TIPO_PESQUISA = PESQUISA;
                            PESQUISA_CONTA_CORRENTE = 1;
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas"))) {
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(); },
                                    'apresentadorMensagem',
                                    CONTA_CORRENTE, null);
                            }
                            else {
                                clearForm("formPlanoContasFK");
                                loadPlanoContas(ObjectStore, Memory, xhr, CONTA_CORRENTE);
                                funcaoFKPlanoContas();
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "fkPlanoContasPes");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId('desPlanoContasPes').value = "";
                        dojo.byId('cdFkPlanoContaPes').value = 0;
                        dijit.byId("limparFkPlanoContasPes").set('disabled', true);
                    }
                }, "limparFkPlanoContasPes");

                if (hasValue(document.getElementById("limparFkPlanoContasPes"))) {
                    document.getElementById("limparFkPlanoContasPes").parentNode.style.minWidth = '40px';
                    document.getElementById("limparFkPlanoContasPes").parentNode.style.width = '40px';
                }

                //#endregion

                var buttonFkArray = ['fkPlanoContasPes', 'pesPlanoCad'];
                diminuirBotoes(buttonFkArray);

                //#region Links
                // Adiciona link de ações:

                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        eventoEditar(gridContaCorrente.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        eventoRemover(gridContaCorrente.itensSelecionados, ' deletarContaCorrente(itensSelecionados);');
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoesContaCorrente").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridContaCorrente, 'todosItem', ['pesquisarContaCorrente', 'relatorioContaCorrente']); pesquisarContaCorrente(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridContaCorrente', 'selecionado', 'cd_conta_corrente', 'selecionaTodos', ['pesquisarContaCorrente', 'relatorioContaCorrente'], 'todosItens');
                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionadosContaCorrente").appendChild(button.domNode);
                //#endregion

                //#region eventos para componentes

                dijit.byId("cbDestinoCad").on("change", function (e) {
                    try{
                        if (hasValue(e)) {
                            dijit.byId('dcPlanoCad').set('required', false);
                            pesquisarMovimentacaoTransferencia();
                        }
                        else {
                            dijit.byId('dcPlanoCad').set('required', true);
                            getFullMovimentacao();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("idMovimentoCad").on("change", function (e) {
                    try {
                       if (hasValue(e == TRANSFERENCIA))
                            dijit.byId("cbDestinoCad").set('required', true);
                        else
                            dijit.byId("cbDestinoCad").set('required', false);

                        if (hasValue(e == ABERTURA_SALDO) || hasValue(e == EnumTipoMovimentacaoFinanceiraContaCorrente.ACERTO_FECHAMENTO_CAIXA) || hasValue(e == TRANSFERENCIA) || hasValue(e == TRANSFERENCIA_MATRIZ)) {
                           dijit.byId("dcPlanoCad").set('required', false);
                           dijit.byId('dcPlanoCad').reset();
                           dojo.byId('cdFkPlanoContaCad').value = 0;
                           dijit.byId("pesPlanoCad").set("disabled", true);
                            if (e == ABERTURA_SALDO || e == EnumTipoMovimentacaoFinanceiraContaCorrente.ACERTO_FECHAMENTO_CAIXA) {
                                dijit.byId("dcPlanoCad").set('disabled', true);
                                dijit.byId("pesPlanoCad").set('disabled', true);
                           }
                           if (hasValue(e == TRANSFERENCIA_MATRIZ)) {
                               dijit.byId("idTipoCad").set('disabled', true);
                               dijit.byId("idTipoCad").set('value', SAIDA);
                           }
                       }
                       else {
                           dijit.byId("pesPlanoCad").set("disabled", false);
                           dijit.byId("idTipoCad").set('disabled', false);
                           dijit.byId("idTipoCad").reset();
                           if (!hasValue(dijit.byId("cbDestinoCad").get('value')))
                               dijit.byId("dcPlanoCad").set('required', true);
                       } 
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                //#endregion
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323062', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['cbOrigemPes', 'cbDestinoPes', 'cbEntradaSaidaPes', 'cbMovimentoPes', 'dtaIni', 'dtaFim'], 'pesquisarContaCorrente', ready);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    })
};

//#endregion

//#region Pesquisa para Movimentação
function getFullMovimentacao() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getCarregarCamposCadastroConta",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataCorrente) {
            try{
                var dados = $.parseJSON(dataCorrente);
                if (dados.retorno != null) {
                    apresentaMensagem("apresentadorMensagemContaCorrente", null);
                    dijit.byId('idMovimentoCad').reset();
                    loadMovimentacao(dados.retorno.movimentacaoFinanceira, 'idMovimentoCad');
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadContaCorrente").style.display))
                apresentaMensagem('apresentadorMensagemContaCorrente', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

function pesquisarMovimentacaoTransferencia() {
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getMovimentacaoTransferencia?cd_movimentacao_financeira=0",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataCorrente) {
            try{
                var dados = $.parseJSON(dataCorrente);
                if (dados.retorno != null) {
                    dijit.byId('idMovimentoCad').reset();
                    apresentaMensagem("apresentadorMensagemContaCorrente", null);
                    loadMovimentacao(dados.retorno, 'idMovimentoCad');
                    dijit.byId('idMovimentoCad').set('value', TRANSFERENCIA);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadContaCorrente").style.display))
                apresentaMensagem('apresentadorMensagemContaCorrente', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    });
}

//#endregion

//#region funcaoFKPlanoContas

function funcaoFKPlanoContas() {
    try{
        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region Métodos auxiliares editarContaCorrente

function obrigarPlanoContas() {
    try{
        var hasPlanoObrigatorio = $.parseJSON(dojo.byId('planoContaObrigatorio').value);
        if (hasPlanoObrigatorio == true)
            dijit.byId('dcPlanoCad').set('required', true);
        else
            dijit.byId('dcPlanoCad').set('required', false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarFormulario(windowUtils) {
    try{
        dijit.byId("idTipoCad").set("required", true);
        var validado = true;

        if (!dijit.byId("formContaCorrente").validate())
            validado = false;

        if (!hasValue(dijit.byId('idTipoCad'))) {
            var tipo = dijit.byId('idTipoCad');
            mostrarMensagemCampoValidado(windowUtils, tipo);
            validado = false;
        }

        if (dijit.byId("cbDestinoCad").required)
            if (!hasValue(dijit.byId('cbDestinoCad'))) {
                var destino = dijit.byId('cbDestinoCad');
                mostrarMensagemCampoValidado(windowUtils, destino);
                validado = false;
            }

        if (dijit.byId("dcPlanoCad").required)
            if (!hasValue(dijit.byId('dcPlanoCad'))) {
                var planoConta = dijit.byId('dcPlanoCad');
                mostrarMensagemCampoValidado(windowUtils, planoConta);
                validado = false;
            }

        return validado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesContaCorrente(value, grid, ehLink) {
    try{
        limparFormulario();
        obrigarPlanoContas();

        dijit.byId('cbDestinoCad')._onChangeActive = false;

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

        if (dijit.byId("idTipoCad") == undefined)
            montarEntradaSaida("idTipoCad", false);

        var ContaCorrente = new ContaCorrenteEdit(value);

        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            xhr.post(Endereco() + "/api/financeiro/getCarregarCamposEditarConta", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(ContaCorrente)
            }).then(function (dataContaC) {
                try{
                    var dados = $.parseJSON(dataContaC);

                    if (!hasValue(dataContaC.erro)) {
                        loadMovimentacao(dados.retorno.movimentacaoFinanceira, 'idMovimentoCad');

                        apresentaMensagem("apresentadorMensagemContaCorrente", null);
                        loadOrigem(dados.retorno.localMovimentoOrigem, 'cbOrigemCad');
                        loadDestino(dados.retorno.localMovimentoDestino, 'cbDestinoCad');
                        loadMovimentacao(dados.retorno.movimentacaoFinanceira, 'idMovimentoCad');
                        criarOuCarregarCompFiltering("cbLiquidacao", dados.retorno.tiposLiquidacao, "", value.cd_tipo_liquidacao, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_liquidacao', 'dc_tipo_liquidacao');

                        dojo.byId('cdContaCorrente').value = value.cd_conta_corrente;
                        dijit.byId('cbOrigemCad').set('value', value.cd_local_origem);
                        if (value.cd_local_destino != null || value.cd_local_destino > 0)
                            dijit.byId('cbDestinoCad').set('value', value.cd_local_destino);
                        dijit.byId('idTipoCad').set('value', value.id_tipo_movimento);

                        dijit.byId('idMovimentoCad')._onChangeActive = false;
                        dijit.byId('idMovimentoCad').set('value', value.cd_movimentacao_financeira);
                        dijit.byId('idMovimentoCad')._onChangeActive = true;

                        if (hasValue(value.cd_movimentacao_financeira == ABERTURA_SALDO) || hasValue(value.cd_movimentacao_financeira == EnumTipoMovimentacaoFinanceiraContaCorrente.ACERTO_FECHAMENTO_CAIXA)) {
	                        dijit.byId("dcPlanoCad").set('required', false);
	                        dijit.byId('dcPlanoCad').reset();
	                        dojo.byId('cdFkPlanoContaCad').value = 0;
	                        dijit.byId("pesPlanoCad").set("disabled", true);
                            if (value.cd_movimentacao_financeira == ABERTURA_SALDO || value.cd_movimentacao_financeira == EnumTipoMovimentacaoFinanceiraContaCorrente.ACERTO_FECHAMENTO_CAIXA) {
		                        dijit.byId("dcPlanoCad").set('disabled', true);
		                        dijit.byId("pesPlanoCad").set('disabled', true);
	                        }
                        }
                        

                        dojo.byId('cdFkPlanoContaCad').value = dados.retorno.cd_plano_conta;
                        dijit.byId('dcPlanoCad').set("value", dados.retorno.planoConta);

                        if (hasValue(value.planoConta))
                            dijit.byId("limparPlanoContas").set('disabled', false);

                        dijit.byId('valContaCorrente').set('value', value.vlConta_corrente);
                        dijit.byId('desObsCc').set('value', value.dc_obs_conta_corrente);

                        dojo.byId("dtaCc").value = value.dt_conta_corrente;
                        dijit.byId('cbDestinoCad')._onChangeActive = true;

                        dijit.byId("cadContaCorrente").show();

                    } else {
                        apresentaMensagem('apresentadorMensagemContaCorrente', dataContaC);
                        dijit.byId('cbDestinoCad')._onChangeActive = true;
                    }
                    showCarregando();
                    setarTabCadContaCorrente();
                }
                catch (er) {
                    postGerarLog(er);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemContaCorrente', error.response.data);
                dijit.byId('cbDestinoCad')._onChangeActive = true;
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFormulario() {
    try{
        clearForm('formContaCorrente');
        dijit.byId('desObsCc').reset();
        dojo.byId('cdFkPlanoContaCad').value = 0;
        dojo.byId('cdContaCorrente').value = 0;
        dijit.byId("limparFkPlanoContasPes").set('disabled', false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region métodos de crud

function ContaCorrenteEdit(value) {
    try{
        if (hasValue(value)) {
            this.cd_conta_corrente = value.cd_conta_corrente;
            this.cd_local_origem = value.cd_local_origem;
            this.cd_local_destino = value.cd_local_destino;
            this.cd_movimentacao_financeira = value.cd_movimentacao_financeira;
            this.cd_tipo_liquidacao = value.cd_tipo_liquidacao;

            this.origem = "";
            this.destino = "";
            this.movimento = "";
            this.planoConta = "";

            this.id_tipo_movimento = 0;
            this.cd_plano_conta = 0;
            this.vl_conta_corrente = 0;
            this.vlConta_corrente = 0;
            this.dc_obs_conta_corrente = "";
            this.dta_conta_corrente = null;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ContaCorrenteObj(isPesquisa) {
    try{
        if (isPesquisa) {
            this.origem = hasValue(dijit.byId('cbOrigemPes').get('value')) == "" ? 0 : dijit.byId('cbOrigemPes').get('value');
            this.destino = hasValue(dijit.byId('cbDestinoPes').get('value')) == "" ? 0 : dijit.byId('cbDestinoPes').get('value');
            this.movimento = hasValue(dijit.byId('cbMovimentoPes').get('value')) == "" ? 0 : dijit.byId('cbMovimentoPes').get('value');
            this.entraSai = hasValue(dijit.byId('cbEntradaSaidaPes').get('value')) == "" ? 0 : dijit.byId('cbEntradaSaidaPes').get('value');
            this.planoConta = hasValue(dojo.byId('cdFkPlanoContaPes').value) == "" ? 0 : dojo.byId('cdFkPlanoContaPes').value;
            this.dta_ini = hasValue(dojo.byId('dtaIni').value) ? dojo.byId('dtaIni').value : '';
            this.dta_fim = hasValue(dojo.byId('dtaFim').value) ? dojo.byId('dtaFim').value : '';
        } else {

            this.cd_conta_corrente = dojo.byId('cdContaCorrente').value;
            this.origem = dojo.byId('cbOrigemCad').value;
            this.destino = dojo.byId('cbDestinoCad').value;
            this.movimento = dojo.byId('idMovimentoCad').value;
            this.planoConta = dojo.byId('dcPlanoCad').value;

            this.cd_local_origem = dijit.byId('cbOrigemCad').get('value');
            this.cd_local_destino = hasValue(dijit.byId('cbDestinoCad').get('value')) == "" ? 0 : dijit.byId('cbDestinoCad').get('value');
            this.cd_tipo_liquidacao = dijit.byId('cbLiquidacao').get('value');
            this.id_tipo_movimento = dijit.byId('idTipoCad').get('value');
            this.cd_movimentacao_financeira = dijit.byId('idMovimentoCad').get('value');
            this.cd_plano_conta = dojo.byId('cdFkPlanoContaCad').value;
            this.vl_conta_corrente = dijit.byId('valContaCorrente').get('value');
            this.vlConta_corrente = dijit.byId('valContaCorrente').get('value');
            this.dc_obs_conta_corrente = hasValue(dijit.byId('desObsCc').get('value')) == "" ? "" : dijit.byId('desObsCc').get('value');
            this.dta_conta_corrente = dojo.date.locale.parse(dojo.byId("dtaCc").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarContaCorrente(limparItens) {
    try{
        var conta = new ContaCorrenteObj(true);

        require([
                 "dojo/store/JsonRest",
                 "dojo/data/ObjectStore",
                 "dojo/store/Cache",
                 "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getContaCorrenteSearch?cdOrigem=" + conta.origem + "&cdDestino=" + conta.destino + "&entraSaida=" + conta.entraSai + "&cdMovimento=" + conta.movimento + "&cdPlanoConta=" + conta.planoConta + '&dta_ini=' + conta.dta_ini + '&dta_fim=' + conta.dta_fim,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_conta_corrente"
                    }), Memory({ idProperty: "cd_conta_corrente" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridContaCorrente = dijit.byId("gridContaCorrente");
                if (limparItens) {
                    gridContaCorrente.itensSelecionados = [];
                }
                gridContaCorrente.noDataMessage = msgNotRegEnc;
                gridContaCorrente.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCadContaCorrente() {
	try {
		var tabs = dijit.byId("tabContainerContaCorrente");
		var pane = dijit.byId("tabPrincipalContaCorrente");
		tabs.selectChild(pane);
	}
	catch (e) {
		postGerarLog(e);
	}
}

function selecionaTabContaCorrente(e) {
	try {
		var tab = hasValue(e) && hasValue(dojo.query(e.target)) && hasValue(dojo.query(e.target).parents) && hasValue(dojo.query(e.target).parents('.dijitTab')[0])
			? dojo.query(e.target).parents('.dijitTab')[0] : null;
		if (!hasValue(tab)) // Clicou na borda da aba:
			tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainerContaCorrente_tablist_tabHistorico' ) {
            var cd_conta_corrente = dojo.byId("cdContaCorrente").value;
			montarGridHistoricoContaCorrente(parseInt(cd_conta_corrente), dojo.xhr);
		}
	}
	catch (e) {
		postGerarLog(e);
	}
}


function montarGridHistoricoContaCorrente(cd_conta_corrente, xhr) {
	showCarregando();
    xhr.get({
        url: Endereco() + "/api/escola/getLogGeralContaCorrente?cd_conta_corrente=" + cd_conta_corrente,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagemContaCorrente", null);
            var retornoData = jQuery.parseJSON(data).retorno;
            if (!hasValue(retornoData) || retornoData.length <= 0) {
                retornoData = [];
            }
            else {
                retornoData = clearChildrenLenthZeroContaCorrente(retornoData);
            }

            var dataTree = {
                identifier: 'id',
                label: 'descricao',
                items: retornoData
            };

            var store = new dojo.data.ItemFileWriteStore({ data: dataTree });

            var model = new dijit.tree.ForestStoreModel({
                store: store, childrenAttrs: ['children']
            });

            var layout = [
                { name: 'Usuário', field: 'descricao', width: '20%' },
                { name: 'Data/Hora', field: 'dta_historico', width: '20%' },
                { name: 'Vl.Antigo', field: 'dc_valor_antigo', width: '20%', styles: "text-align: center;" },
                { name: 'Vl.Novo', field: 'dc_valor_novo', width: '20%', styles: "text-align: center;" },
                { name: 'Operação', field: 'dc_tipo_log', width: '20%', styles: "text-align: center;" },
                { name: '', field: 'id', width: '0%', styles: "display: none;" }
            ];
            destroyCreateHistoricoContaCorrente();
            var gridHistoricoContaCorrente = new dojox.grid.LazyTreeGrid({
                id: 'gridHistoricoContaCorrente',
                treeModel: model,
                structure: layout,
                noDataMessage: msgNotRegEnc
            }, document.createElement('div'));

            dojo.byId("gridHistoricoContaCorrente").appendChild(gridHistoricoContaCorrente.domNode);
            gridHistoricoContaCorrente.canSort = function (col) { return false; };
            gridHistoricoContaCorrente.startup();
            showCarregando();
        }
        catch (e) {
	        showCarregando();
            postGerarLog(e);
        }
    },
        function (error) {
	        showCarregando();
            apresentaMensagem('apresentadorMensagemTitulo', error);
        });
}

function clearChildrenLenthZeroContaCorrente(dataRetorno) {
	try {
		for (var i = 0; i < dataRetorno.length; i++) {
			if (dataRetorno[i].children.length > 0) {
				for (var j = 0; j < dataRetorno[i].children.length; j++) {
					if (dataRetorno[i].children[j].children != null && dataRetorno[i].children[j].children.length > 0) {
						for (var m = 0; m < dataRetorno[i].children[j].children.length; m++)
							delete dataRetorno[i].children[j].children[m].children;
					} else delete dataRetorno[i].children[j].children;
				}
			}
		}
		return dataRetorno;
	}
	catch (e) {
		postGerarLog(e);
	}
}

function destroyCreateHistoricoContaCorrente() {
	try {
		if (hasValue(dijit.byId("gridHistoricoContaCorrente"))) {
			dijit.byId("gridHistoricoContaCorrente").destroy();
			//$('<div>').attr('id', 'gridHistoricoBaixaTitulo').attr('style', 'height:100%;').attr('style', 'min-height:200px;').appendTo('#paiGridHistBaixaTitulo');
		}
	}
	catch (e) {
		postGerarLog(e);
	}
}

function incluirContaCorrente() {
    try{
        showCarregando();
        apresentaMensagem("apresentadorMensagemCadContaCorrente", '');
        apresentaMensagem('apresentadorMensagem', null);

        var conta = new ContaCorrenteObj(false);

        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {
                if (!validarFormulario(windows)) {
                    showCarregando();
                    return false;
                }

                xhr.post(Endereco() + "/api/escola/PostIncluirContaCorrente", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson(conta)
                }).then(function (data) {
                    try {
                        if (!hasValue(data.erro)) {
                            data = $.parseJSON(data);
                            var itemAlterado = data.retorno;
                            var gridName = 'gridContaCorrente';
                            var grid = dijit.byId(gridName);

                            dijit.byId("cadContaCorrente").hide();
                            if (!hasValue(grid.itensSelecionados)) {
                                grid.itensSelecionados = [];
                            }
                            insertObjSort(grid.itensSelecionados, "cd_conta_corrente", itemAlterado);
                            buscarItensSelecionados(gridName, 'selecionado', 'cd_conta_corrente', 'selecionaTodos', ['pesquisarContaCorrente', 'relatorioContaCorrente'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_conta_corrente");
                        } else
                            apresentaMensagem('apresentadorMensagemContaCorrente', data);
                        showCarregando();
                    }
                    catch (er) {
                        postGerarLog(er);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemContaCorrente', error.response.data);
                    showCarregando();
                });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarContaCorrente(itensSelecionados) {
    showCarregando();
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom,  xhr, ref) {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cdContaCorrente').value != 0)
                itensSelecionados = [{
                    cd_conta_corrente: dom.byId("cdContaCorrente").value,
                    cd_local_origem: dijit.byId('cbOrigemCad').get('value'),
                    dta_conta_corrente:dojo.date.locale.parse(dojo.byId("dtaCc").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                }];
        xhr.post({
            url: Endereco() + "/api/escola/postdeleteContaCorrente",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try{
                var todos = dojo.byId("todosItens");
                apresentaMensagem('apresentadorMensagem', data);
                data = jQuery.parseJSON(data).retorno;
                dijit.byId("cadContaCorrente").hide();

                pesquisarContaCorrente(true);

                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridContaCorrente').itensSelecionados, "cd_conta_corrente", itensSelecionados[r].cd_conta_corrente);

                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarContaCorrente").set('disabled', false);
                dijit.byId("relatorioContaCorrente").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                showCarregando();
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadContaCorrente").style.display)) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemContaCorrente', error);
            }
            else {
                showCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            }
        });
    })
}

function editarContaCorrente() {
    showCarregando();
    apresentaMensagem("apresentadorMensagemContaCorrente", '');
    apresentaMensagem('apresentadorMensagem', null);

    var conta = new ContaCorrenteObj(false);

    require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {
        if (hasValue(dijit.byId("cbDestinoCad").get('value')) || (dijit.byId('idMovimentoCad').get('value') == 3) || (dijit.byId('idMovimentoCad').get('value') == 6))
            dijit.byId("dcPlanoCad").set('required', false);
        else
            dijit.byId("dcPlanoCad").set('required', true);

        if (!validarFormulario(windows)) return false;

        xhr.post(Endereco() + "/api/escola/postEditarContaCorrente", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(conta)
        }).then(function (data) {
            try{
                if (!hasValue(data.erro)) {
                    data = $.parseJSON(data);
                    var itemAlterado = data.retorno;
                    var gridName = 'gridContaCorrente';
                    var grid = dijit.byId(gridName);

                    dijit.byId("cadContaCorrente").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_conta_corrente", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionado', 'cd_conta_corrente', 'selecionaTodos', ['pesquisarContaCorrente', 'relatorioContaCorrente'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_conta_corrente");

                    showCarregando();
                } else {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemContaCorrente', data);
                }
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemContaCorrente', error.response.data);
        });
    });
}

function eventoEditar(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            showCarregando();
            apresentaMensagem('apresentadorMensagem', '');
            setarTabCadContaCorrente();
            keepValuesContaCorrente(null, dijit.byId('gridContaCorrente'), true);
            dijit.byId("cadContaCorrente").show();
            dijit.byId("tabContainerContaCorrente_tablist_tabHistorico").domNode.style.visibility = '';
            IncluirAlterar(0, 'divAlterarContaCorrente', 'divIncluirContaCorrente', 'divExcluirContaCorrente', 'apresentadorMensagemContaCorrente', 'divCancelarContaCorrente', 'divClearContaCorrente');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion