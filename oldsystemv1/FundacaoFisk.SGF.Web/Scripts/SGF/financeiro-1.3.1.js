//** Monta o SelectOneMenu
var GRUPO_ESTOQUE = 1;
var MOVIMENTACAO_FINANCEIRA = 2;
var TIPO_LIQUIDACAO = 3;
var TIPO_FINANCEIRO = 4;
var TIPO_DESCONTO = 5;
var BIBLIOTECA = 3;
var UMNIVEL = 1, DOISNIVEIS = 2;
var NOVO = 1,EDITAR = 2;
var GRUPO_CONTA = 7;
var SUBGRUPO_CONTA = 8;
var BANCO = 9;
var CARTEIRA = 10;
var nivel_parametro = 2;

var EnumSelectOneMenu = {
    ORGAO_FINANCEIRO: 11
}

// ** função que monta o comboBox de staus
function montarStatusSN(nomElement) {
    require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (ready, Memory, filteringSelect) {
         try{
             var statusStore = new Memory({
                 data: [
                     { name: "Todos", id: "0" },
                     { name: "Sim", id: "1" },
                     { name: "Não", id: "2" }
                 ]
             });
             ready(function () {
                 var status = new filteringSelect({
                     id: nomElement,
                     //name: "baixa",
                     value: "0",
                     store: statusStore,
                     searchAttr: "name",
                     style: "width: 75px;"
                 }, nomElement);
             })
         } catch (e) {
             postGerarLog(e);
         }
     });

};
function montarImpres(nomElement) {
    require(["dojo/store/Memory", "dijit/form/FilteringSelect"],
     function (Memory, filteringSelect) {
         try {
             var dados = [
                    { name: "Todos", id: "0" },
                    { name: "Banco", id: "1" },
                    { name: "Escola", id: "2" }
             ]
             var statusStore = new Memory({
                 data: dados
             });
             var status = new filteringSelect({
                 id: nomElement,
                 name: "tpImp",
                 store: statusStore,
                 searchAttr: "name",
                 style: "width: 40%;",
                 value: 0
             }, nomElement);
         } catch (e) {
             postGerarLog(e);
         }
     });

};

function formatCheckBoxEsc(value, rowIndex, obj) {
    try{
        var gridName = 'gridEscola';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEsc');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_pessoa", grid._by_idx[rowIndex].item.cd_pessoa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:16px'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_pessoa', 'selecionadoEsc', -1, 'selecionaTodosEsc', 'selecionaTodosEsc', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_pessoa', 'selecionadoEsc', " + rowIndex + ", '" + id + "', 'selecionaTodosEsc', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function selecionaTab(e) {
    try{
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) tab = dojo.query(e.target)[0];// Clicou na borda da aba

        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabGrupoEstoque') {
            abrirTabGrupoEstoque();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322995', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMovFinanceira') {
            abrirTabMovimentacaoFinanceira();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322996', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTpLiquidacao') {
            abrirTabTipoLiquidacao();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322997', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTipoFinanceiro') {
            abrirTabTipoFinanceiro();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322998', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabTipoDesconto') {
            abrirTabTipoDesconto();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322999', '765px', '771px');
                    });
            }
        }
        if (jQuery.parseJSON(MasterGeral()) == false)
            dijit.byId("tabContainerTpDesc_tablist_tabEscolaTpDesc").domNode.style.visibility = 'hidden';
        if (tab.getAttribute('widgetId') == "tabContainerTpDesc_tablist_tabEscolaTpDesc") {
            popularGridEscola();
            dojo.byId('abriuEscola').value = true;
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabGrupoContas') {
            abrirTabGrupoContas();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323000', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabSubgrupoConta') {
            abrirTabSubgrupoConta();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323001', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabBanco') {
            abrirTabBanco();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323002', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabCarteira') {
            abrirTabCarteira();
            if (hasValue(dijit.byId("menuManual"))) {
                var menuManual = dijit.byId("menuManual");
                if (hasValue(menuManual.handler))
                    menuManual.handler.remove();
                menuManual.handler = menuManual.on("click",
                    function(e) {
                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323003', '765px', '771px');
                    });
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabOrgaoFinanceiro') {
	        abrirTabOrgaoFinanceiro();
	        if (hasValue(dijit.byId("menuManual"))) {
		        var menuManual = dijit.byId("menuManual");
		        if (hasValue(menuManual.handler))
			        menuManual.handler.remove();
		        menuManual.handler = menuManual.on("click",
			        function (e) {
				        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322997', '765px', '771px');
			        });
	        }
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabGrupoEstoque() {
    apresentaMensagem('apresentadorMensagem', null);
}

function abrirTabMovimentacaoFinanceira() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirMovFinan').className)) {
            montarGridMovFinanceira();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabTipoLiquidacao() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTpLiquidacao').className)) {
            montarGridTipoLiquidacao();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabTipoFinanceiro() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTipoFinanceiro').className)) {
            montarGridTipoFinanceiro();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabTipoDesconto() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirTipoDesconto').className)) {
            montarGridTipoDesconto();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabItemServico() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirItemServico').className)) {
            montarGridItemServico();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabGrupoContas() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirGrupoContas').className)) {
            montarGridGrupoContas();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabSubgrupoConta() {
    try {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirSubgrupoConta').className)) {
            require(["dojo/_base/xhr"],
          function (xhr) {
              xhr.get({
                  url: Endereco() + "/api/escola/getParametrosNiveisPlanoConta",
                  preventCache: true,
                  handleAs: "json",
                  headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
              }).then(function (data) {
                  try {
                      showCarregando();
                      nivel_parametro = data.retorno;
                      montarGridSubgrupoConta(data.retorno);
                  } catch (e) {
                      showCarregando();
                      postGerarLog(e);
                  }
              },
              function (error) {
                  showCarregando();
                  montarGridSubgrupoConta(null);
                  apresentaMensagem('apresentadorMensagem', error);
              });
          })

        } else {
            showCarregando();
        }
    } catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function abrirTabBanco() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirBanco').className)) {
            montarCadastroBanco();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTabCarteira() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        if (!hasValue(document.getElementById('incluirCarteira').className)) {
            montarCadastroCarteira();
        }
    } catch (e) {
        postGerarLog(e);
    }
}


function abrirTabOrgaoFinanceiro() {
	try {
		apresentaMensagem('apresentadorMensagem', null);
		if (!hasValue(document.getElementById('incluirOrgaoFinanceiro').className)) {
			montarGridOrgaoFinanceiro();
		}
	} catch (e) {
		postGerarLog(e);
	}
}

//Pega os Antigos dados do Formulário
function keepValues(tipoForm, grid, ehLink) {
    try{
        var valorCancelamento = grid.selection.getSelected();

        verificarMasterGeral();
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
            case GRUPO_ESTOQUE: {
                getLimpar('#formGrupoEstoque');
                dojo.byId("cd_grupo_estoque").value = value.cd_grupo_estoque;
                dojo.byId("no_grupo_estoque").value = value.no_grupo_estoque;
                dijit.byId("id_categoria_grupo").set("value", value.id_categoria_grupo);
                dojo.byId("id_grupo_estoque_ativo").value = value.id_grupo_estoque_ativo == true ? dijit.byId("id_grupo_estoque_ativo").set("value", true) : dijit.byId("id_grupo_estoque_ativo").set("value", false);
                dojo.byId("id_eliminar_inventario").value = value.id_eliminar_inventario == true ? dijit.byId("id_eliminar_inventario").set("value", true) : dijit.byId("id_eliminar_inventario").set("value", false);
                break;
            };//GRUPO_ESTOQUE
            case MOVIMENTACAO_FINANCEIRA: {
                getLimpar('#formMovFinanceira');
                dojo.byId("cd_movimentacao_financeira").value = value.cd_movimentacao_financeira;
                dojo.byId("dc_movimentacao_financeira").value = value.dc_movimentacao_financeira;
                dojo.byId("id_mov_financeira_ativa").value = value.id_mov_financeira_ativa == true ? dijit.byId("id_mov_financeira_ativa").set("value", true) : dijit.byId("id_mov_financeira_ativa").set("value", false);
                break;
            };//MOVIMENTACAO_FINANCEIRA
            case TIPO_LIQUIDACAO: {
                getLimpar('#formTipoLiquidacao');
                dojo.byId("cd_tipo_liquidacao").value = value.cd_tipo_liquidacao;
                dojo.byId("dc_tipo_liquidacao").value = value.dc_tipo_liquidacao;
                dojo.byId("id_tipo_liquidacao_ativa").value = value.id_tipo_liquidacao_ativa == true ? dijit.byId("id_tipo_liquidacao_ativa").set("value", true) : dijit.byId("id_tipo_liquidacao_ativa").set("value", false);
                break;
            }//TIPO_LIQUIDACAO
            case TIPO_FINANCEIRO: {
                getLimpar('#formTipoFinanceiro');
                dojo.byId("cd_tipo_financeiro").value = value.cd_tipo_financeiro;
                dojo.byId("dc_tipo_financeiro").value = value.dc_tipo_financeiro;
                dojo.byId("id_tipo_financeiro_ativo").value = value.id_tipo_financeiro_ativo == true ? dijit.byId("id_tipo_financeiro_ativo").set("value", true) : dijit.byId("id_tipo_financeiro_ativo").set("value", false);
                break;
            };//TIPO_FINANCEIRO
            case TIPO_DESCONTO: {
                dojo.byId('abriuEscola').value = false;
                var tabContainerTpDesc = dijit.byId("tabContainerTpDesc");
                tabContainerTpDesc.selectChild(tabContainerTpDesc.getChildren()[0]);
                dijit.byId('tabPrincipalTpDesc').resize();
                getLimpar('#formTipoDesconto');
                dojo.byId("cd_tipo_desconto").value = value.cd_tipo_desconto;
                dojo.byId("dc_tipo_desconto").value = value.dc_tipo_desconto;
                dojo.byId("id_tipo_desconto_ativo").value = value.id_tipo_desconto_ativo == true ? dijit.byId("id_tipo_desconto_ativo").set("value", true) : dijit.byId("id_tipo_desconto_ativo").set("value", false);
                //dojo.byId("id_incide_parcela_1").value = value.id_incide_parcela_1 == true ? dijit.byId("id_incide_parcela_1").set("value", true) : dijit.byId("id_incide_parcela_1").set("value", false);
                dojo.byId("id_incide_baixa").value = value.id_incide_baixa == true ? dijit.byId("id_incide_baixa").set("value", true) : dijit.byId("id_incide_baixa").set("value", false);
                dijit.byId("pc_desconto").value = hasValue(value.pc_desc) ? dijit.byId('pc_desconto').set('value', value.pc_desc) : 0;
                break;
            };//TIPO_DESCONTO

            case GRUPO_CONTA: {
                getLimpar('#formGrupoContas');
                dojo.byId("cd_grupo_conta").value = value.cd_grupo_conta;
                dojo.byId("no_grupo_conta").value = value.no_grupo_conta;
                // dojo.byId("id_tipo_grupo_conta").value = dijit.byId("id_tipo_grupo_conta").set("value", value.id_tipo_grupo_conta);
                dijit.byId('ordem').set('value', value.nm_ordem_grupo);
                var idTipo = dijit.byId("id_tipo_grupo_conta");
                idTipo.set("value", value.id_tipo_grupo_conta);

                break;
            };//GRUPO_CONTA
            case SUBGRUPO_CONTA: {
                confLayoutCadSubgrupo(EDITAR);
                require([
                    "dojo/_base/xhr"
                ], function (xhr) {
                    var compNivel = dijit.byId("cadNivel");
                    xhr.get({
                        url: Endereco() + "/api/financeiro/getAllGrupoConta",
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }).then(function (dataGrupoConta) {
                        loadGrupoConta(jQuery.parseJSON(dataGrupoConta).retorno, 'cd_grupo_subgrupo');
                        dojo.byId("cd_subgrupo_conta").value = value.cd_subgrupo_conta;
                        dojo.byId("no_subgrupo_conta").value = value.no_subgrupo_conta;
                        var cdGrupo = dijit.byId("cd_grupo_subgrupo");
                        cdGrupo._onChangeActive = false;
                        cdGrupo.set("value", value.cd_grupo_conta);
                        cdGrupo._onChangeActive = true;
                        if (value.cd_subgrupo_pai != null && value.cd_subgrupo_pai > 0) {
                            dojo.byId("cd_subgrupo_pai").value = value.cd_subgrupo_pai;
                            dojo.byId("no_subgrupo_conta").value = value.subgrupo_2_conta;
                            dijit.byId("subgrupo1contas").set("value", value.cd_subgrupo_pai);
                            loadSubgrupoPorGrupoContas(dojo.xhr, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, value.cd_grupo_conta, value.cd_subgrupo_pai);
                            compNivel.set("value", 2);
                        } else {
                            compNivel.set("value", 1);
                            dojo.byId("no_subgrupo_conta").value = value.no_subgrupo_conta;
                        }
                        dijit.byId('ordem_subgrupo').set('value', value.nm_ordem_subgrupo);
                        dijit.byId('descCdIntegracaoPlano').set('value', value.dc_cod_integracao_plano);
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagemSubgrupoContas', error);
                    });
                });
            };//SUBGRUPO_CONTA
            case BANCO: {
                getLimpar('#formBanco');
                dojo.byId("cd_banco").value = value.cd_banco;
                dojo.byId("no_banco").value = value.no_banco;
                dijit.byId("nm_banco").set("value", value.nm_banco);
                break;
            };//BANCO
            case CARTEIRA: {
                populaBancoCad(value.cd_banco, 'cdBanco');
                getLimpar('#formCarteira');
                clearForm('formCarteira');
                dijit.byId("nmColuna").reset();
                dojo.byId("cd_carteira_cnab").value = value.cd_carteira_cnab;
                dojo.byId("no_carteira").value = value.no_carteira;
                dijit.byId("cdBanco").set("value", value.cd_banco);
                dojo.byId("cdBanco").value = value.bancoCarteira;
                dijit.byId("nm_carteira").set("value", value.nm_carteira);
                dijit.byId("ckRegristrada").set("checked", value.id_registrada);
                dijit.byId("ckCarteiraAtiva").set("checked", value.id_carteira_ativa);
                dijit.byId("nmColuna").set("value", value.nm_colunas);
                dijit.byId("tpImp").set("value", value.id_impressao);
                break
            }; //Carteira CNAB
            case EnumSelectOneMenu.ORGAO_FINANCEIRO: {
	            getLimpar('#formOrgaoFinanceiro');
	            dojo.byId("cd_orgao_financeiro").value = value.cd_orgao_financeiro;
	            dojo.byId("dc_orgao_financeiro").value = value.dc_orgao_financeiro;
                dojo.byId("id_orgao_financeiro_ativo").value = value.id_orgao_ativo == true ? dijit.byId("id_orgao_financeiro_ativo").set("value", true) : dijit.byId("id_orgao_financeiro_ativo").set("value", false);
	            break;
            }//TIPO_LIQUIDACAO
        }
    } catch (e) {
        postGerarLog(e);
    }
};

// inicio da formatação do grupo estoque

//Grupo Estoque
function formatCheckBoxGrupoEstoque(value, rowIndex, obj) {
    try{
        var gridName = 'gridGrupoEst';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosGruposEstoque');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_grupo_estoque", grid._by_idx[rowIndex].item.cd_grupo_estoque);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_grupo_estoque', 'selecionadoGrupoEstoque', -1, 'selecionaTodosGruposEstoque', 'selecionaTodosGruposEstoque', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_grupo_estoque', 'selecionadoGrupoEstoque', " + rowIndex + ", '" + id + "', 'selecionaTodosGruposEstoque', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//Movimentação Financeira
function formatCheckBoxMovimentacaoFinanceira(value, rowIndex, obj) {
    try{
        var gridName = 'gridMovFinan';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasMovimentacoesFinanceiras');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_movimentacao_financeira", grid._by_idx[rowIndex].item.cd_movimentacao_financeira);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_movimentacao_financeira', 'selecionadoMovimentacaoFinanceira', -1, 'selecionaTodasMovimentacoesFinanceiras', 'selecionaTodasMovimentacoesFinanceiras', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_movimentacao_financeira', 'selecionadoMovimentacaoFinanceira', " + rowIndex + ", '" + id + "', 'selecionaTodasMovimentacoesFinanceiras', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//Tipo de liquidação
function formatCheckBoxTipoLiquiacao(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoLiquidacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTiposLiquidacao');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_liquidacao", grid._by_idx[rowIndex].item.cd_tipo_liquidacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_liquidacao', 'selecionadoTipoLiquidacao', -1, 'selecionaTodosTiposLiquidacao', 'selecionaTodosTiposLiquidacao', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_liquidacao', 'selecionadoTipoLiquidacao', " + rowIndex + ", '" + id + "', 'selecionaTodosTiposLiquidacao', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//Tipo Financeiro
function formatCheckBoxTipoFinanceiro(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoFinanceiro';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTiposFinanceiros');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_financeiro", grid._by_idx[rowIndex].item.cd_tipo_financeiro);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_financeiro', 'selecionadoTipoFinanceiro', -1, 'selecionaTodosTiposFinanceiros', 'selecionaTodosTiposFinanceiros', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_financeiro', 'selecionadoTipoFinanceiro', " + rowIndex + ", '" + id + "', 'selecionaTodosTiposFinanceiros', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//Subgrupo Conta
function formatCheckBoxSubgrupoConta(value, rowIndex, obj) {
    try{
        var gridName = 'gridSubgrupoConta';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosSubgrupoConta');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_subgrupo_conta", grid._by_idx[rowIndex].item.cd_subgrupo_conta);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_subgrupo_conta', 'selecionadoSubgrupoConta', -1, 'selecionaTodosSubgrupoConta', 'selecionaTodosSubgrupoConta', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_subgrupo_conta', 'selecionadoSubgrupoConta', " + rowIndex + ", '" + id + "', 'selecionaTodosSubgrupoConta', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//Grupo Conta
function formatCheckBoxGrupoContas(value, rowIndex, obj) {
    try{
        var gridName = 'gridGrupoContas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosGrupoContas');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_grupo_conta", grid._by_idx[rowIndex].item.cd_grupo_conta);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_grupo_conta', 'selecionadoGrupoContas', -1, 'selecionaTodosGrupoContas', 'selecionaTodosGrupoContas', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_grupo_conta', 'selecionadoGrupoContas', " + rowIndex + ", '" + id + "', 'selecionaTodosGrupoContas', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}


//Tipo Desconto
function formatCheckBoxTipoDesconto(value, rowIndex, obj) {
    try{
        var gridName = 'gridTipoDesconto';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTiposDesconto');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_tipo_desconto", grid._by_idx[rowIndex].item.cd_tipo_desconto);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_tipo_desconto', 'selecionadoTipoDesconto', -1, 'selecionaTodosTiposDesconto', 'selecionaTodosTiposDesconto', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_tipo_desconto', 'selecionadoTipoDesconto', " + rowIndex + ", '" + id + "', 'selecionaTodosTiposDesconto', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}


function formatCheckBoxBanco(value, rowIndex, obj) {
    try{
        var gridName = 'gridBanco';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosBanco');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_banco", grid._by_idx[rowIndex].item.cd_banco);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_banco', 'selecionadoBanco', -1, 'selecionaTodosBanco', 'selecionaTodosBanco', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_banco', 'selecionadoBanco', " + rowIndex + ", '" + id + "', 'selecionaTodosBanco', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxCarteira(value, rowIndex, obj) {
    try{
        var gridName = 'gridCarteira';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosCarteira');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_carteira_cnab", grid._by_idx[rowIndex].item.cd_carteira_cnab);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_carteira_cnab', 'selecionadoCarteira', -1, 'selecionaTodosCarteira', 'selecionaTodosCarteira', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_carteira_cnab', 'selecionadoCarteira', " + rowIndex + ", '" + id + "', 'selecionaTodosCarteira', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

//Orgao Financeiro
function formatCheckBoxOrgaoFinanceiro(value, rowIndex, obj) {
	try {
		var gridName = 'gridOrgaoFinanceiro';
		var grid = dijit.byId(gridName);
		var icon;
		var id = obj.field + '_Selected_' + rowIndex;
		var todos = dojo.byId('selecionaTodosOrgaoFinanceiro');

		if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
			var indice = binaryObjSearch(grid.itensSelecionados, "cd_orgao_financeiro", grid._by_idx[rowIndex].item.cd_orgao_financeiro);

			value = value || indice != null; // Item está selecionado.
		}
		if (rowIndex != -1)
			icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

		// Configura o check de todos, para quando mudar de aba:
		if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_orgao_financeiro', 'selecionadoOrgaoFinanceiro', -1, 'selecionaTodosOrgaoFinanceiro', 'selecionaTodosOrgaoFinanceiro', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_orgao_financeiro', 'selecionadoOrgaoFinanceiro', " + rowIndex + ", '" + id + "', 'selecionaTodosOrgaoFinanceiro', '" + gridName + "')", 2);

		return icon;
	} catch (e) {
		postGerarLog(e);
	}
}

//--**********************************--\\

function mostraTabs(Permissoes) {
    try{
        require([
             "dijit/registry",
             "dojo/ready"
        ], function (registry, ready) {
            ready(function () {
                if (!possuiPermissao('gest', Permissoes)) {
                    registry.byId('tabGrupoEstoque').set('disabled', !registry.byId('tabGrupoEstoque').get('disabled'));
                    document.getElementById('tabGrupoEstoque').style.visibility = "hidden";
                }
                if (!possuiPermissao('mvfin', Permissoes)) {
                    registry.byId('tabMovFinanceira').set('disabled', !registry.byId('tabMovFinanceira').get('disabled'));
                    document.getElementById('tabMovFinanceira').style.visibility = "hidden";
                }
                if (!possuiPermissao('tpliq', Permissoes)) {
                    registry.byId('tabTpLiquidacao').set('disabled', !registry.byId('tabTpLiquidacao').get('disabled'));
                    document.getElementById('tabTpLiquidacao').style.visibility = "hidden";
                }
                if (!possuiPermissao('tpfin', Permissoes)) {
                    registry.byId('tabTipoFinanceiro').set('disabled', !registry.byId('tabTipoFinanceiro').get('disabled'));
                    document.getElementById('tabTipoFinanceiro').style.visibility = "hidden";
                }
                if (!possuiPermissao('tpdes', Permissoes)) {
                    registry.byId('tabTipoDesconto').set('disabled', !registry.byId('tabTipoDesconto').get('disabled'));
                    document.getElementById('tabTipoDesconto').style.visibility = "hidden";
                }
                if (!possuiPermissao('subgr', Permissoes)) {
                    registry.byId('tabSubgrupoConta').set('disabled', !registry.byId('tabSubgrupoConta').get('disabled'));
                    document.getElementById('tabSubgrupoConta').style.visibility = "hidden";
                }
                if (!possuiPermissao('gruco', Permissoes)) {
                    registry.byId('tabGrupoContas').set('disabled', !registry.byId('tabGrupoContas').get('disabled'));
                    document.getElementById('tabGrupoContas').style.visibility = "hidden";
                }
                if (!possuiPermissao('banc', Permissoes)) {
                    registry.byId('tabBanco').set('disabled', !registry.byId('tabBanco').get('disabled'));
                    document.getElementById('tabBanco').style.visibility = "hidden";
                }

            })
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//******************************************************************Criação das grades ******************************************************************

function montarCadastroGrupoEstoque() {
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
         "dojo/domReady!",
         "dojo/parser"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest,  ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try {
                montarStatus("statusGrupoEstoque");
                montarStatus("statusMovFinan");
                montarStatus("statusTpLiquidacao");
                montarStatus("statusTipoFinanceiro");
                montarStatus("statusTipoDesconto");
                montarStatusSN("incideBaixa");
                //montarStatusSN("pparc");
                dijit.byId("statusTipoDesconto").set("value", 0);
                loadCategoriaGrupoItem("categoria_grupo_item");
                dijit.byId('tabContainer').resize();
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/financeiro/getgrupoestoquesearch?descricao=&inicio=false&status=1&categoria=0",
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                        idProperty: "cd_grupo_estoque"
                    }
                ), Memory({ idProperty: "cd_grupo_estoque" }));
                var gridGrupoEstoque = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                        { name: "<input id='selecionaTodosGruposEstoque' style='display:none'/>", field: "selecionadoGrupoEstoque", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxGrupoEstoque },
                      //  { name: "Código", field: "cd_grupo_estoque", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                        { name: "Grupo de Item", field: "no_grupo_estoque", width: "65%" },
                        { name: "Categoria", field: "categoria_grupo", width: "10%" },
                        { name: "Ativo", field: "grupo_estoque_ativo", width: "10%", styles: "text-align: center;" },
                        { name: "El. Inventário", field: "eliminar_inventario", width: "10%", styles: "text-align: center;" }
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
                }, "gridGrupoEst"); // make sure you have a target HTML element with this id
                gridGrupoEstoque.pagination.plugin._paginator.plugin.connect(gridGrupoEstoque.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridGrupoEstoque, 'cd_grupo_estoque', 'selecionaTodosGruposEstoque');
                });
                gridGrupoEstoque.canSort = function (col) { return Math.abs(col) != 1; };
                gridGrupoEstoque.startup();
                gridGrupoEstoque.on("RowDblClick", function (evt) {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            verificarMasterGeral();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        var idx = evt.rowIndex,
                                  item = this.getItem(idx),
                                  store = this.store;
                        getLimpar('#formGrupoEstoque');
                        apresentaMensagem('apresentadorMensagem', '');
                        keepValues(GRUPO_ESTOQUE, gridGrupoEstoque, false);
                        dijit.byId("cadGrupoEstoque").show();
                        IncluirAlterar(0, 'divAlterarGrupoEst', 'divIncluirGrupoEst', 'divExcluirGrupoEst', 'apresentadorMensagemGrupoEst', 'divCancelarGrupoEst', 'divLimparGrupoEst');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridGrupoEstoque, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosGruposEstoque').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_grupo_estoque', 'selecionadoGrupoEstoque', -1, 'selecionaTodosGruposEstoque', 'selecionaTodosGruposEstoque', 'gridGrupoEstoque')", gridGrupoEstoque.rowsPerPage * 3);
                    });
                });

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        try{
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoEditarGrupoEstoque(gridGrupoEstoque.itensSelecionados);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        try{
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            eventoRemover(gridGrupoEstoque.itensSelecionados, 'DeletarGrupoEstoque(itensSelecionados)');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasGrupoEstoque",
                    dropDown: menu,
                    id: "acoesRelacionadasGrupoEstoque"
                });
                dom.byId("linkAcoesGrupoEstoque").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () { buscarTodosItens(gridGrupoEst, 'todosItensGrupoEstoque', ['pesquisarGrupoEst', 'relatorioGrupoEstoque']); PesquisarGrupoEst(false); }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridGrupoEst', 'selecionadoGrupoEstoque', 'cd_grupo_estoque', 'selecionaTodosGruposEstoque', ['pesquisarGrupoEst', 'relatorioGrupoEstoque'], 'todosItensGrupoEstoque'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItensGrupoEstoque",
                    dropDown: menu,
                    id: "todosItensGrupoEstoque"
                });
                dom.byId("linkSelecionadosGruposEstoque").appendChild(button.domNode);

                //criação de botões de persistência

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        IncluirGrupoEstoque();
                    }
                }, "incluirGrupoEst");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        AlterarGrupoEstoque();
                    }
                }, "alterarGrupoEst");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                            DeletarGrupoEstoque();
                        });
                    }
                }, "deleteGrupoEst");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                    onClick: function () {
                        getLimpar('#formGrupoEstoque');
                        clearForm('formGrupoEstoque');
                    }
                }, "limparGrupoEst");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValues(GRUPO_ESTOQUE, gridGrupoEstoque, null);
                    }
                }, "cancelarGrupoEst");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("cadGrupoEstoque").hide();
                    }
                }, "fecharGrupoEst");
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarGrupoEst(true); }
                }, "pesquisarGrupoEst");
                decreaseBtn(document.getElementById("pesquisarGrupoEst"), '32px');

                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try{
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }

                            dijit.byId("cadGrupoEstoque").show();
                            getLimpar('#formGrupoEstoque');
                            clearForm('formTipoLiquidacao');
                            verificarMasterGeral();
                            apresentaMensagem('apresentadorMensagem', null);
                            IncluirAlterar(1, 'divAlterarGrupoEst', 'divIncluirGrupoEst', 'divExcluirGrupoEst', 'apresentadorMensagemGrupoEst', 'divCancelarGrupoEst', 'divLimparGrupoEst');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoGrupoEst");
                //----Monta o botão de Relatório----
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        xhr.get({
                            url: !hasValue(document.getElementById("pesquisaGrupoEst").value) ? Endereco() + "/api/financeiro/geturlrelatoriogrupoestoque?" + getStrGridParameters('gridGrupoEst') + "desc=&inicio=" + document.getElementById("inicioDescGrupoEst").checked + "&status=" + retornaStatus("statusGrupoEstoque") + "&categoria=" + retornaStatus("categoria_grupo_item") : Endereco() + "/api/financeiro/geturlrelatoriogrupoestoque?" + getStrGridParameters('gridGrupoEst') + "desc=" + encodeURIComponent(document.getElementById("pesquisaGrupoEst").value) + "&inicio=" + document.getElementById("inicioDescGrupoEst").checked + "&status=" + retornaStatus("statusGrupoEstoque") + "&categoria=" + retornaStatus("categoria_grupo_item"),
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (data) {
                            try{
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                }, "relatorioGrupoEstoque");
                if (hasValue(dijit.byId("menuManual"))) {
                    var menuManual = dijit.byId("menuManual");
                    if (hasValue(menuManual.handler))
                        menuManual.handler.remove();
                    menuManual.handler = menuManual.on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322995', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['pesquisaGrupoEst', 'statusGrupoEstoque', 'categoria_grupo_item'], 'pesquisarGrupoEst', ready);
                showCarregando();
                dijit.byId("tabContainer_tablist_menuBtn").on("click", function () {
                    try{
                        if (hasValue(dijit.byId("tabContainer_menu")) && dijit.byId("tabContainer_menu")._created) {

                            dijit.byId("tabGrupoEstoque_stcMi").on("click", function () {
                                try{
                                    abrirTabGrupoEstoque();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322995',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabMovFinanceira_stcMi").on("click", function () {
                                try{
                                    abrirTabMovimentacaoFinanceira();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322996',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabTpLiquidacao_stcMi").on("click", function () {
                                try{
                                    abrirTabTipoLiquidacao();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322997',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabTipoFinanceiro_stcMi").on("click", function () {
                                try{
                                    abrirTabTipoFinanceiro();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322998',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabTipoDesconto_stcMi").on("click", function () {
                                try{
                                    abrirTabTipoDesconto();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454322999',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabGrupoContas_stcMi").on("click", function () {
                                try{
                                    abrirTabGrupoContas();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323000',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabSubgrupoConta_stcMi").on("click", function () {
                                try{
                                    abrirTabSubgrupoConta();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323001',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabBanco_stcMi").on("click", function () {
                                try{
                                    abrirTabBanco();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323002',
                                                    '765px',
                                                    '771px');
                                            });
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                            dijit.byId("tabCarteira_stcMi").on("click", function () {
                                try{
                                    abrirTabCarteira();
                                    if (hasValue(dijit.byId("menuManual"))) {
                                        var menuManual = dijit.byId("menuManual");
                                        if (hasValue(menuManual.handler))
                                            menuManual.handler.remove();
                                        menuManual.handler = menuManual.on("click",
                                            function(e) {
                                                abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323003',
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
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
}


function verificarMasterGeral() {
    try{
        loadCategoriaGrupoItem("id_categoria_grupo");

        if (MasterGeral() == "false") {
            dijit.byId("id_categoria_grupo").set("value", 2);
            dijit.byId("id_categoria_grupo").set("disabled", true);
        }
        else {
            dijit.byId("id_categoria_grupo").set("value", 1);
            dijit.byId("id_categoria_grupo").set("disabled", false);
        }
    } catch (e) {
        postGerarLog(e);
    }
}


function montarGridMovFinanceira() {
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
         "dijit/MenuItem"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getmovimentacaofinanceirasearch?descricao=&inicio=false&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_movimentacao_financeira"
                }
            ), Memory({ idProperty: "cd_movimentacao_financeira" }));
            var gridMovFinanceira = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodasMovimentacoesFinanceiras' style='display:none'/>", field: "selecionadoMovimentacaoFinanceira", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMovimentacaoFinanceira },
                  //  { name: "Código", field: "cd_movimentacao_financeira", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Movimentação Financeira", field: "dc_movimentacao_financeira", width: "85%" },
                    { name: "Ativo", field: "mov_financeira_ativa", width: "10%", styles: "text-align: center;" }
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
                        maxPageStep: 4,
                        /*position of the pagination bar*/
                        position: "button"
                    }
                }
            }, "gridMovFinan"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridMovFinanceira.pagination.plugin._paginator.plugin.connect(gridMovFinanceira.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMovFinanceira, 'cd_movimentacao_financeira', 'selecionaTodasMovimentacoesFinanceiras');
            });
            gridMovFinanceira.canSort = function (col) { return Math.abs(col) != 1; };
            gridMovFinanceira.startup();
            gridMovFinanceira.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    getLimpar('#formMovFinanceira');
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(MOVIMENTACAO_FINANCEIRA, gridMovFinanceira, false);
                    dijit.byId("cadMovFinan").show();
                    IncluirAlterar(0, 'divAlterarMovFinan', 'divIncluirMovFinan', 'divExcluirMovFinan', 'apresentadorMensagemMovFinan', 'divCancelarMovFinan', 'divLimparMovFinan');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridMovFinanceira, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodasMovimentacoesFinanceiras').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_movimentacao_financeira', 'selecionadoMovimentacaoFinanceira', -1, 'selecionaTodasMovimentacoesFinanceiras', 'selecionaTodasMovimentacoesFinanceiras', 'gridMovFinan')", gridMovFinanceira.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarMovimentacaoFinanceira(gridMovFinanceira.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridMovFinanceira.itensSelecionados, 'DeletarMovFinan(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasMovimentacaoFinanceira",
                dropDown: menu,
                id: "acoesRelacionadasMovimentacaoFinanceira"
            });
            dom.byId("linkAcoesMovimentacaoFinanceira").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridMovFinanceira, 'todosItensMovimentacaoFinanceira', ['pesquisarMovFinan', 'relatorioMovFinan']); PesquisarMovFinan(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridMovFinan', 'selecionadoMovimentacaoFinanceira', 'cd_movimentacao_financeira', 'selecionaTodasMovimentacoesFinanceiras', ['pesquisarMovFinan', 'relatorioMovFinan'], 'todosItensMovimentacaoFinanceira'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensMovimentacaoFinanceira",
                dropDown: menu,
                id: "todosItensMovimentacaoFinanceira"
            });
            dom.byId("linkSelecionadosMovimentacoesFinanceiras").appendChild(button.domNode);

            //botões de persistência
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    IncluirMovFinan();
                }
            }, "incluirMovFinan");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    AlterarMovFinan();
                }
            }, "alterarMovFinan");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarMovFinan();
                    });
                }
            }, "deleteMovFinan");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    try{
                        getLimpar('#formMovFinanceira');
                        clearForm('formMovFinanceira');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "limparMovFinan");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(MOVIMENTACAO_FINANCEIRA, gridMovFinanceira, null);
                }
            }, "cancelarMovFinan");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadMovFinan").hide();
                }
            }, "fecharMovFinan");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarMovFinan(true); }
            }, "pesquisarMovFinan");
            decreaseBtn(document.getElementById("pesquisarMovFinan"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadMovFinan").show();
                        getLimpar('#formMovFinanceira');
                        clearForm('formTipoLiquidacao');
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarMovFinan', 'divIncluirMovFinan', 'divExcluirMovFinan', 'apresentadorMensagemMovFinan', 'divCancelarMovFinan', 'divLimparMovFinan');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoMovFinan");

            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaMovFinan").value) ? Endereco() + "/api/financeiro/geturlrelatoriomovimentacaofinanceira?" + getStrGridParameters('gridMovFinan') + "desc=&inicio=" + document.getElementById("inicioDescMovFinan").checked + "&status=" + retornaStatus("statusMovFinan") : Endereco() + "/api/financeiro/geturlrelatoriomovimentacaofinanceira?" + getStrGridParameters('gridMovFinan') + "desc=" + encodeURIComponent(document.getElementById("pesquisaMovFinan").value) + "&inicio=" + document.getElementById("inicioDescMovFinan").checked + "&status=" + retornaStatus("statusMovFinan"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioMovFinan");
            adicionarAtalhoPesquisa(['pesquisaMovFinan', 'statusMovFinan'], 'pesquisarMovFinan', ready);
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function montarCadastroBanco() {
    //Criação da Grade
    require([
        "dojo/_base/xhr",
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
        "dojo/dom"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom) {
        ready(function () {
            //*** Cria a grade de Bancos **\\
            dijit.byId("numBanco").set("required", false);
            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/financeiro/getBancoSearch?nome=&nmBanco=&inicio=false",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_banco"
            }), Memory({ idProperty: "cd_banco" }));
            var gridBanco = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure:
                  [
                    { name: "<input id='selecionaTodosBanco'/>", field: "selecionadoBanco", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxBanco },
                    { name: "Nome", field: "no_banco", width: "85%", styles: "min-width:80px;" },
                    { name: "Número", field: "nm_banco", width: "10%", styles: "min-width:40px;" }
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
            }, "gridBanco");

            gridBanco.canSort = function (col) { return Math.abs(col) != 1};
            gridBanco.startup();
            gridBanco.on("RowDblClick", function (evt) {
                if (!eval(MasterGeral())) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    return;
                }
                var idx = evt.rowIndex,
                          item = this.getItem(idx),
                          store = this.store;
                getLimpar('#formBanco');
                apresentaMensagem('apresentadorMensagem', '');
                keepValues(BANCO, gridBanco, false);
                dijit.byId("cadBanco").show();
                IncluirAlterar(0, 'divAlterarBanco', 'divIncluirBanco', 'divExcluirBanco', 'apresentadorMensagemBanco', 'divCancelarBanco', 'divLimparBanco');
            }, true);
            gridBanco.canSort = function (col) { return Math.abs(col) != 1; };
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridBanco, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodasMovimentacoesFinanceiras').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_banco', 'selecionadoBanco', -1, 'selecionaTodasBanco', 'selecionaTodasBanco', 'gridBanco')", gridBanco.rowsPerPage * 3);
                });
            });

            //*** Cria os botões do link de ações **\\
            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridBanco, 'todosItensBanco', ['pesquisarBanco', 'relatorioBanco']); PesquisarBanco(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridBanco', 'selecionadoBanco', 'cd_banco', 'selecionaTodosBanco', ['pesquisarBanco', 'relatorioBanco'], 'todosItensBanco'); }
            });
            menu.addChild(menuItensSelecionados);


            button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensBanco",
                dropDown: menu,
                id: "todosItensBanco"
            });
            dom.byId("linkSelecionadosBanco").appendChild(button.domNode);

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
                    eventoEditarBanco(gridBanco.itensSelecionados);
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
                    eventoRemover(gridBanco.itensSelecionados, 'DeletarBanco(itensSelecionados)');
                }
            });
            menu.addChild(acaoExcluir);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasBanco",
                dropDown: menu,
                id: "acoesRelacionadasBanco"
            });
            dom.byId("linkAcoesBanco").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () { IncluirBanco(); }
            }, "incluirBanco");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () { AlterarBanco(); }
            }, "alterarBanco");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarBanco();
                    });
                }
            }, "deleteBanco");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    getLimpar('#formBanco');
                    clearForm('formBanco');
                }
            }, "limparBanco");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(BANCO, gridBanco, null);
                }
            }, "cancelarBanco");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () { dijit.byId("cadBanco").hide(); }
            }, "fecharBanco");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarBanco(true);
                }
            }, "pesquisarBanco");
            decreaseBtn(document.getElementById("pesquisarBanco"), '32px');

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
                    dijit.byId("cadBanco").show();
                    getLimpar('#formBanco');
                    clearForm('formBanco');
                    apresentaMensagem('apresentadorMensagem', null);
                    IncluirAlterar(1, 'divAlterarBanco', 'divIncluirBanco', 'divExcluirBanco', 'apresentadorMensagemBanco', 'divCancelarBanco', 'divLimparBanco');
                }
            }, "novoBanco");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/financeiro/getUrlRelatorioBanco?" + getStrGridParameters('gridBanco') + "nome=" + dojo.byId("descBanco").value + "&nmBanco=" + dojo.byId("numBanco").value + "&inicio=" + dojo.byId("inicioBanco").checked,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioBanco");
            var buttonFkArray = ['cadPessoa', 'cadPessoaBco'];

            for (var p = 0; p < buttonFkArray.length; p++) {
                var buttonFk = document.getElementById(buttonFkArray[p]);

                if (hasValue(buttonFk)) {
                    buttonFk.parentNode.style.minWidth = '18px';
                    buttonFk.parentNode.style.width = '18px';
                }
            }

        })
        adicionarAtalhoPesquisa(['descBanco', 'numBanco'], 'pesquisarBanco', ready);
    });
}

function montarCadastroCarteira() {
    //Criação da Grade
    require([
        "dojo/_base/xhr",
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
        "dojo/dom"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, DropDownButton, DropDownMenu, MenuItem, dom) {
        ready(function () {
            //*** Cria a grade de Bancos **\\
            montarStatus("statusCarteira");
            montarColuna("nmColuna");
            montarImpres("tpImp");
            dijit.byId('tabContainer').resize();
            populaBancoPesq(0, 'cdBancoPesq');

            var myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/cnab/getCarteiraCnabSearch?nome=&inicio=false&banco=0&status=1",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_carteira_cnab"
            }), Memory({ idProperty: "cd_carteira_cnab" }));
            var gridCarteira = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure:
                  [
                    { name: "<input id='selecionaTodosCarteira' style='display:none'/>", field: "selecionadoCarteira", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxCarteira },
					{ name: "Banco", field: "bancoCarteira", width: "15%", styles: "min-width:80px;" },
					{ name: "Nome", field: "no_carteira", width: "50%", styles: "min-width:80px;" },
					{ name: "Número", field: "nm_carteira", width: "5%", styles: "min-width:80px;text-align:center;" },
					{ name: "Registrada", field: "dc_registro", width: "8%", styles: "min-width:80px;text-align:center;" },
					{ name: "Colunas", field: "nm_colunas", width: "8%", styles: "min-width:80px;text-align:center;" },
					{ name: "Ativa", field: "carteira_ativa", width: "8%", styles: "min-width:80px;text-align:center;" }
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
            }, "gridCarteira");

            gridCarteira.canSort = function (col) { return Math.abs(col) != 1 };
            gridCarteira.startup();
            gridCarteira.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    getLimpar('#formCarteira');
                    clearForm('formCarteira');
                    dijit.byId("nmColuna").reset();
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(CARTEIRA, gridCarteira, false);
                    dijit.byId("cadCarteira").show();
                    IncluirAlterar(0, 'divAlterarCarteira', 'divIncluirCarteira', 'divExcluirCarteira', 'apresentadorMensagemCarteira', 'divCancelarCarteira', 'divClearCarteira');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);
            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridCarteira, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodasMovimentacoesFinanceiras').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_carteira_cnab', 'selecionadoCarteira', -1, 'selecionaTodasCarteira', 'selecionaTodasCarteira', 'gridCarteira')", gridCarteira.rowsPerPage * 3);
                });
            });

            //*** Cria os botões do link de ações **\\
            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () {
                    try{
                        buscarTodosItens(gridCarteira, 'todosItensCarteira', ['pesquisarCarteira', 'relatorioCarteira']);
                        pesquisarCarteiraCNAB(false);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () {
                    buscarItensSelecionados('gridCarteira', 'selecionadoCarteira', 'cd_carteira_cnab', 'selecionaTodosCarteira', ['pesquisarCarteira', 'relatorioCarteira'], 'todosItensCarteira');
                }
            });
            menu.addChild(menuItensSelecionados);


            button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensCarteira",
                dropDown: menu,
                id: "todosItensCarteira"
            });
            dom.byId("linkSelecionadosCarteira").appendChild(button.domNode);

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
           

            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarCarteira(gridCarteira.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);
            var acaoExcluir = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridCarteira.itensSelecionados, 'DeletarCarteira(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }

            });
            menu.addChild(acaoExcluir);

            button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasCarteira",
                dropDown: menu,
                id: "acoesRelacionadasCarteira"
            });
            dom.byId("linkAcoesCarteira").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () { IncluirCarteiraCNAB(); }
            }, "incluirCarteira");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () { AlterarCarteira(); }
            }, "alterarCarteira");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarCarteira();
                    });
                }
            }, "deleteCarteira");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                type: "reset",
                onClick: function () {
                    getLimpar('#formCarteira');
                    clearForm('formCarteira');
                    dijit.byId("nmColuna").reset();
                }
            }, "limparCarteira");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(CARTEIRA, gridCarteira, null);
                }
            }, "cancelarCarteira");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () { dijit.byId("cadCarteira").hide(); }
            }, "fecharCarteira");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    pesquisarCarteiraCNAB(true);
                }
            }, "pesquisarCarteira");
            decreaseBtn(document.getElementById("pesquisarCarteira"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        populaBancoCad(0, 'cdBanco');
                        dijit.byId("cadCarteira").show();
                        getLimpar('#formCarteira');
                        clearForm('formCarteira');
                        dijit.byId("nmColuna").reset();

                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarCarteira', 'divIncluirCarteira', 'divExcluirCarteira', 'apresentadorMensagemCarteira', 'divCancelarCarteira', 'divClearCarteira');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novaCarteira");

            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    var banco = dijit.byId("cdBancoPesq").value > 0 ? dijit.byId("cdBancoPesq").value : 0;
                    xhr.get({
                        url: Endereco() + "/api/cnab/getUrlRelatorioCarteiraCnab?" + getStrGridParameters('gridCarteira') + "nome=" + dojo.byId("descCarteira").value + "&inicio=" + dojo.byId("ckInicio").checked + "&banco=" + banco + "&status=" + retornaStatus("statusCarteira"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioCarteira");
            var buttonFkArray = ['cadPessoa', 'cadPessoaBco'];

            for (var p = 0; p < buttonFkArray.length; p++) {
                var buttonFk = document.getElementById(buttonFkArray[p]);

                if (hasValue(buttonFk)) {
                    buttonFk.parentNode.style.minWidth = '18px';
                    buttonFk.parentNode.style.width = '18px';
                }
            }

        })
        adicionarAtalhoPesquisa(['descCarteira', 'cdBancoPesq', 'statusCarteira'], 'pesquisarCarteira', ready);
    });
}

function montarGridTipoLiquidacao() {
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
            "dijit/form/DropDownButton",
            "dijit/DropDownMenu",
            "dijit/MenuItem",
            "dojo/ready"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/gettipoliquidacaosearch?descricao=&inicio=false&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_liquidacao"
                }
        ), Memory({ idProperty: "cd_tipo_liquidacao" }));

            var gridTpLiquidacao = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTiposLiquidacao' style='display:none'/>", field: "selecionadoTipoLiquidacao", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoLiquiacao },
                   // { name: "Código", field: "cd_tipo_liquidacao", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Tipo Liquidação", field: "dc_tipo_liquidacao", width: "85%" },
                    { name: "Ativo", field: "tipo_liquidacao_ativa", width: "10%", styles: "text-align: center;" }
                ],
                canSort: false,
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
            }, "gridTipoLiquidacao"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridTpLiquidacao.pagination.plugin._paginator.plugin.connect(gridTpLiquidacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridMotivoNMatricula, 'cd_tipo_liquidacao', 'selecionaTodosTiposLiquidacao');
            });
            gridTpLiquidacao.startup();
            gridTpLiquidacao.canSort = function (col) { return Math.abs(col) != 1; };
            gridTpLiquidacao.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                        item = this.getItem(idx),
                        store = this.store;
                    getLimpar('#formTipoLiquidacao');
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(TIPO_LIQUIDACAO, gridTpLiquidacao, false);
                    dijit.byId("cadTipoLiquidacao").show();
                    IncluirAlterar(0, 'divAlterarTpLiquidacao', 'divIncluirTpLiquidacao', 'divExcluirTpLiquidacao', 'apresentadorMensagemTpLiquidacao', 'divCancelarTpLiquidacao', 'divLimparTpLiquidacao');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTpLiquidacao, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTiposLiquidacao').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_tipo_liquidacao', 'selecionadoTipoLiquidacao', -1, 'selecionaTodosTiposLiquidacao', 'selecionaTodosTiposLiquidacao', 'gridTipoLiquidacao')", gridTpLiquidacao.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarTipoLiquidacao(gridTpLiquidacao.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridTpLiquidacao.itensSelecionados, 'DeletarTipoLiquidacao(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTipoLiquidacao",
                dropDown: menu,
                id: "acoesRelacionadasTipoLiquidacao"
            });
            dom.byId("linkAcoesTipoLiquidacao").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTpLiquidacao, 'todosItensTipoLiquidacao', ['pesquisarTpLiquidacao', 'relatorioTpLiquidacao']); PesquisarTpLiquidacao(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTipoLiquidacao', 'selecionadoTipoLiquidacao', 'cd_tipo_liquidacao', 'selecionaTodosTiposLiquidacao', ['pesquisarTpLiquidacao', 'relatorioTpLiquidacao'], 'todosItensTipoLiquidacao'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTipoLiquidacao",
                dropDown: menu,
                id: "todosItensTipoLiquidacao"
            });
            dom.byId("linkSelecionadosTiposLiquidacoes").appendChild(button.domNode);

            // Criação de botões de persistência
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    IncluirTipoLiquidacao();
                }
            }, "incluirTpLiquidacao");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    AlterarTipoLiquidacao();
                }
            }, "alterarTpLiquidacao");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarTipoLiquidacao();
                    });
                }
            }, "deleteTpLiquidacao");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    getLimpar('#formTipoLiquidacao');
                    clearForm('formTipoLiquidacao');
                }
            }, "limparTpLiquidacao");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(TIPO_LIQUIDACAO, gridTpLiquidacao, false);
                }
            }, "cancelarTpLiquidacao");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadTipoLiquidacao").hide();
                }
            }, "fecharTpLiquidacao");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarTpLiquidacao(true);
                }
            }, "pesquisarTpLiquidacao");
            decreaseBtn(document.getElementById("pesquisarTpLiquidacao"), '32px');
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadTipoLiquidacao").show();
                        getLimpar('#formTipoLiquidacao');
                        clearForm('formTipoLiquidacao');
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTpLiquidacao', 'divIncluirTpLiquidacao', 'divExcluirTpLiquidacao', 'apresentadorMensagemTpLiquidacao', 'divCancelarTpLiquidacao', 'divLimparTpLiquidacao');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTpLiquidacao");

            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaTpLiquidacao").value) ? Endereco() + "/api/financeiro/geturlrelatoriotipoliquidacao?" + getStrGridParameters('gridTipoLiquidacao') + "desc=&inicio=" + document.getElementById("inicioDescTpLiquidacao").checked + "&status=" + retornaStatus("statusTpLiquidacao") : Endereco() + "/api/financeiro/geturlrelatoriotipoliquidacao?" + getStrGridParameters('gridTipoLiquidacao') + "desc=" + encodeURIComponent(document.getElementById("pesquisaTpLiquidacao").value) + "&inicio=" + document.getElementById("inicioDescTpLiquidacao").checked + "&status=" + retornaStatus("statusTpLiquidacao"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioTpLiquidacao");
            adicionarAtalhoPesquisa(['pesquisaTpLiquidacao', 'statusTpLiquidacao'], 'pesquisarTpLiquidacao', ready);
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function montarGridTipoFinanceiro() {
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
         "dijit/form/DropDownButton",
         "dijit/DropDownMenu",
         "dijit/MenuItem",
         "dojo/ready",
         "dojo/domReady!",
         "dojo/parser"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/gettipofinanceirosearch?descricao=&inicio=false&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_financeiro"
                }
        ), Memory({ idProperty: "cd_tipo_financeiro" }));

            var gridTipoFinanceiro = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTiposFinanceiros' style='display:none'/>", field: "selecionadoTipoFinanceiro", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoFinanceiro },
                  //  { name: "Código", field: "cd_tipo_financeiro", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Tipo Financeiro", field: "dc_tipo_financeiro", width: "85%" },
                    { name: "Ativo", field: "tipo_financeiro_ativo", width: "10%", styles: "text-align: center;" }
                ],
                canSort: false,
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
            }, "gridTipoFinanceiro"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridTipoFinanceiro.pagination.plugin._paginator.plugin.connect(gridTipoFinanceiro.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridTipoFinanceiro, 'cd_tipo_financeiro', 'selecionaTodosTiposFinanceiros');
            });
            gridTipoFinanceiro.startup();
            gridTipoFinanceiro.canSort = function (col) { return Math.abs(col) != 1; };
            gridTipoFinanceiro.on("RowDblClick", function (evt) {
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
                    getLimpar('#formTipoFinanceiro');
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(TIPO_FINANCEIRO, gridTipoFinanceiro, false);
                    dijit.byId("cadTipoFinanceiro").show();
                    IncluirAlterar(0, 'divAlterarTipoFinanceiro', 'divIncluirTipoFinanceiro', 'divExcluirTipoFinanceiro', 'apresentadorMensagemTipoFinanceiro', 'divCancelarTipoFinanceiro', 'divLimparTipoFinanceiro');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoFinanceiro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTiposFinanceiros').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_tipo_financeiro', 'selecionadoTipoFinanceiro', -1, 'selecionaTodosTiposFinanceiros', 'selecionaTodosTiposFinanceiros', 'gridTipoFinanceiro')", gridTipoFinanceiro.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarTipoFinanceiro(gridTipoFinanceiro.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridTipoFinanceiro.itensSelecionados, 'DeletarTipoFinanceiro(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTipoFinanceiro",
                dropDown: menu,
                id: "acoesRelacionadasTipoFinanceiro"
            });
            dom.byId("linkAcoesTipoFinanceiro").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTipoFinanceiro, 'todosItensTipoFinanceiro', ['pesquisarTipoFinanceiro', 'relatorioTipoFinanceiro']); PesquisarTipoFinanceiro(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTipoFinanceiro', 'selecionadoTipoFinanceiro', 'cd_tipo_financeiro', 'selecionaTodosTiposFinanceiros', ['pesquisarTipoFinanceiro', 'relatorioTipoFinanceiro'], 'todosItensTipoFinanceiro'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTipoFinanceiro",
                dropDown: menu,
                id: "todosItensTipoFinanceiro"
            });
            dom.byId("linkSelecionadosTipoFinanceiro").appendChild(button.domNode);

            // Botões para percistência
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    IncluirTipoFinanceiro('apresentadorMensagemTipoFinanceiro', null, 'formTipoFinanceiro');
                }
            }, "incluirTipoFinanceiro");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    AlterarTipoFinanceiro('apresentadorMensagemTipoFinanceiro', 'formTipoFinanceiro');
                }
            }, "alterarTipoFinanceiro");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarTipoFinanceiro();
                    });
                }
            }, "deleteTipoFinanceiro");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    getLimpar('#formTipoFinanceiro');
                    clearForm('formTipoFinanceiro');
                }
            }, "limparTipoFinanceiro");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                keepValues(TIPO_FINANCEIRO, gridTipoFinanceiro, null);
                }
            }, "cancelarTipoFinanceiro");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarTipoFinanceiro(true);
                }
            }, "pesquisarTipoFinanceiro");
            decreaseBtn(document.getElementById("pesquisarTipoFinanceiro"), '32px');
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadTipoFinanceiro").hide();
                }
            }, "fecharTipoFinanceiro");
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadTipoFinanceiro").show();
                        getLimpar('#formTipoFinanceiro');
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTipoFinanceiro', 'divIncluirTipoFinanceiro', 'divExcluirTipoFinanceiro', 'apresentadorMensagemTipoFinanceiro', 'divCancelarTipoFinanceiro', 'divLimparTipoFinanceiro');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoFinanceiro");

            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaTipoFinanceiro").value) ? Endereco() + "/api/financeiro/geturlrelatoriotipofinanceiro?" + getStrGridParameters('gridTipoFinanceiro') + "desc=&inicio=" + document.getElementById("inicioDescTipoFinanceiro").checked + "&status=" + retornaStatus("statusTipoFinanceiro") : Endereco() + "/api/financeiro/geturlrelatoriotipofinanceiro?" + getStrGridParameters('gridTipoFinanceiro') + "desc=" + encodeURIComponent(document.getElementById("pesquisaTipoFinanceiro").value) + "&inicio=" + document.getElementById("inicioDescTipoFinanceiro").checked + "&status=" + retornaStatus("statusTipoFinanceiro"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioTipoFinanceiro");
            adicionarAtalhoPesquisa(['pesquisaTipoFinanceiro', 'statusTipoFinanceiro'], 'pesquisarTipoFinanceiro', ready);
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function loadGrupoConta(data, linkGrupoConta) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkGrupoConta == 'grupo_conta'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_grupo_conta, name: value.no_grupo_conta });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkGrupoConta).store = stateStore;
		        if ((linkGrupoConta == 'grupo_conta'))
		            dijit.byId(linkGrupoConta).set("value", 0);
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}

function loadTipoGrupo(link) {
    require(["dojo/store/Memory"],
		function (Memory) {
		    try{
		        var items = [];
		        if ((link == 'tipo_grupo_conta'))
		            items.push({ id: 0, name: "Todos" });
		        items.push(
                    { name: "Ativo", id: "1" },
                  { name: "Passivo", id: "2" },
                  { name: "Receita", id: "3" },
                  { name: "Custo", id: "4" },
                  { name: "Despesa", id: "5" });
		        var statusStoreTipoConta = new Memory({
		            data: items
		        });

		        dijit.byId(link).store = statusStoreTipoConta;
		        if ((link == 'tipo_grupo_conta')) {
		            dijit.byId(link).set("value", 0);
		            dijit.byId(link).set("required", false);
		        }
		        else
		            dijit.byId(link).set("required", true);
		    } catch (e) {
		        postGerarLog(e);
		    }
		});
}


function loadCategoriaGrupoItem(link) {
    require([ "dojo/store/Memory"],
		function (Memory) {
		    try{
		        var items = [];
		        if ((link == 'categoria_grupo_item'))
		            items.push({ id: 0, name: "Todos" });
		        items.push(
                  { name: "Privada", id: "1" },
                  { name: "Pública", id: "2" });
		        var statusStoreTipoConta = new Memory({
		            data: items
		        });

		        dijit.byId(link).store = statusStoreTipoConta;
		        if ((link == 'categoria_grupo_item')) {
		            dijit.byId(link).set("value", 0);
		            dijit.byId(link).set("required", false);
		        }
		        else {
		            dijit.byId(link).set("required", true);
		        }
		    } catch (e) {
		        postGerarLog(e);
		    }
		});
}

function montarGridGrupoContas() {
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
         "dijit/form/DropDownButton",
         "dijit/DropDownMenu",
         "dijit/MenuItem"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getgrupocontasearch?descricao=&inicio=false&tipoGrupo=0",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_grupo_conta"
                }
        ), Memory({ idProperty: "cd_grupo_conta" }));
            //var data = new Array();
            var gridGrupoContas = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                //store: dataStore = new ObjectStore({ objectStore: new Memory({ data: data}) }),
                structure: [
                    { name: "<input id='selecionaTodosGrupoContas' style='display:none'/>", field: "selecionadoGrupoContas", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxGrupoContas },
                  //  { name: "Código", field: "cd_tipo_financeiro", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Grupo de Contas", field: "no_grupo_conta", width: "48%", styles: "min-width:80px;" },
                    { name: "Tipo", field: "tipo", width: "32%", styles: "min-width:80px;" },
                    { name: "Ordem", field: "nm_ordem_grupo", width: "15%", styles: "min-width:80px;" }
                ],
                canSort: false,
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
            }, "gridGrupoContas"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridGrupoContas.pagination.plugin._paginator.plugin.connect(gridGrupoContas.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridGrupoContas, 'cd_grupo_conta', 'selecionaTodosGrupoContas');
            });
            gridGrupoContas.startup();
            gridGrupoContas.canSort = function (col) { return Math.abs(col) != 1; };
            gridGrupoContas.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    getLimpar('#formGrupoContas');
                    apresentaMensagem('apresentadorMensagem', '');
                    loadTipoGrupo("id_tipo_grupo_conta");
                    keepValues(GRUPO_CONTA, gridGrupoContas, false);
                    dijit.byId("cadGrupoContas").show();

                    IncluirAlterar(0, 'divAlterarGrupoContas', 'divIncluirGrupoContas', 'divExcluirGrupoContas', 'apresentadorMensagemGrupoContas', 'divCancelarGrupoContas', 'divLimparGrupoContas');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoFinanceiro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosGrupoContas').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_grupo_conta', 'selecionadoGrupoContas', -1, 'selecionaTodosGrupoContas', 'selecionaTodosGrupoContas', 'gridGrupoContas')", gridGrupoContas.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        loadTipoGrupo("id_tipo_grupo_conta"); eventoEditarGrupoContas(gridGrupoContas.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridGrupoContas.itensSelecionados, 'DeletarGrupoContas(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasGrupoContas",
                dropDown: menu,
                id: "acoesRelacionadasGrupoContas"
            });
            dom.byId("linkAcoesGrupoContas").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridGrupoContas, 'todosItensGrupoContas', ['pesquisarGrupoContas', 'relatorioGrupoContas']); PesquisarGrupoContas(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridGrupoContas', 'selecionadoGrupoContas', 'cd_grupo_conta', 'selecionaTodosGrupoContas', ['pesquisarGrupoContas', 'relatorioGrupoContas'], 'todosItensGrupoContas'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensGrupoContas",
                dropDown: menu,
                id: "todosItensGrupoContas"
            });
            dom.byId("linkSelecionadosGrupoContas").appendChild(button.domNode);

            // Botões para percistência

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    IncluirGrupoContas('apresentadorMensagemGrupoContas', null, 'formGrupoContas');
                }
            }, "incluirGrupoContas");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    AlterarGrupoContas('apresentadorMensagemGrupoContas', 'formGrupoContas');
                }
            }, "alterarGrupoContas");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarGrupoContas();
                    });
                }
            }, "deleteGrupoContas");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    getLimpar('#formGrupoContas');
                    clearForm('formGrupoContas');
                }
            }, "limparGrupoContas");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                keepValues(GRUPO_CONTA, dijit.byId('gridGrupoContas'), null);
                }
            }, "cancelarGrupoContas");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarGrupoContas(true);
                }
            }, "pesquisarGrupoContas");
            decreaseBtn(document.getElementById("pesquisarGrupoContas"), '32px');
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadGrupoContas").hide();
                }
            }, "fecharGrupoContas");
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("cadGrupoContas").show();
                        loadTipoGrupo("id_tipo_grupo_conta");
                        getLimpar('#formGrupoContas');
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarGrupoContas', 'divIncluirGrupoContas', 'divExcluirGrupoContas', 'apresentadorMensagemGrupoContas', 'divCancelarGrupoContas', 'divLimparGrupoContas');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoGrupoContas");

            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/financeiro/GetUrlRelatorioGrupoConta?" + getStrGridParameters('gridGrupoContas') + "desc=" + encodeURIComponent(document.getElementById("pesquisaGrupoContas").value) + "&inicio=" + document.getElementById("inicioDescGrupoContas").checked + "&tipoGrupo=" + retornaStatus("tipo_grupo_conta"),
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioGrupoContas");
            adicionarAtalhoPesquisa(['pesquisaGrupoContas', 'tipo_grupo_conta'], 'pesquisarGrupoContas', dojo.ready);
            loadTipoGrupo("tipo_grupo_conta");
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function montarGridSubgrupoConta(retNivel) {
    require([
        "dojo/ready",
         "dojo/_base/xhr",
         "dojo/dom",
         "dojox/grid/EnhancedGrid",
         "dojox/grid/enhanced/plugins/Pagination",
         "dojo/store/JsonRest",
         "dojo/data/ObjectStore",
         "dojo/store/Cache",
         "dojo/store/Memory",
         "dijit/form/Button",
         "dijit/form/DropDownButton",
         "dijit/DropDownMenu",
         "dijit/MenuItem",
         "dijit/form/FilteringSelect",
         "dojo/on",
         "dojo/domReady!",
         "dojo/parser"
    ], function (ready, xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, on) {
        try{
            var nivelParametro = hasValue(retNivel) && retNivel > 0 ? retNivel : 2;

            var myStoreSubG = Cache(
                       JsonRest({
                           target: Endereco() + "/api/financeiro/getsubgrupocontasearch?descricao=&inicio=false&cdGrupo=0&tipoNivel=" + nivelParametro,
                           handleAs: "json",
                           preventCache: true,
                           headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                           idProperty: "cd_subgrupo_conta"
                       }
               ), Memory({}));
            var gridSubgrupoConta = new EnhancedGrid({
                store: new ObjectStore({ objectStore: myStoreSubG }),
                structure: [
                  { name: "<input id='selecionaTodosSubgrupoConta' style='display:none'/>", field: "selecionadoSubgrupoConta", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxSubgrupoConta },
                  { name: "Grupo de Contas", field: "grupo_conta", width: "25%" },
                  { name: "Subgrupo 1 de Contas", field: "no_subgrupo_conta", width: "30%", styles: "min-width:80px;" },
                  { name: "Subgrupo 2 de Contas", field: "subgrupo_2_conta", width: "30%", styles: "min-width:80px;" },
                  { name: "Ordem", field: "nm_ordem_subgrupo", width: "10%", styles: "min-width:80px;" }
                ],
                canSort: false,
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
            }, "gridSubgrupoConta"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridSubgrupoConta.pagination.plugin._paginator.plugin.connect(gridSubgrupoConta.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridSubgrupoConta, 'cd_subgrupo_conta', 'selecionaTodosSubgrupoConta');
            });
            gridSubgrupoConta.startup();
            gridSubgrupoConta.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3 && Math.abs(col) != 4; };
            gridSubgrupoConta.on("RowDblClick", function (evt) {
                try{
                    if (!eval(MasterGeral())) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        return;
                    }
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    limparCadSubgrupoContas();
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(SUBGRUPO_CONTA, gridSubgrupoConta, false);
                    dijit.byId("cadSubgrupoConta").show();
                    IncluirAlterar(0, 'divAlterarSubgrupoConta', 'divIncluirSubgrupoConta', 'divExcluirSubgrupoConta', 'apresentadorMensagemSubgrupoConta', 'divCancelarSubgrupoConta', 'divClearSubgrupoConta');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridSubgrupoConta, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosSubgrupoConta').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_subgrupo_conta', 'selecionadoSubgrupoConta', -1, 'selecionaTodosSubgrupoConta', 'selecionaTodosSubgrupoConta', 'gridSubgrupoConta')", gridSubgrupoConta.rowsPerPage * 3);
                });
            });

            // Adiciona link de ações:
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditarSubgrupoConta(gridSubgrupoConta.itensSelecionados);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridSubgrupoConta.itensSelecionados, 'DeletarSubgrupoContas(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasSubgrupoConta",
                dropDown: menu,
                id: "acoesRelacionadasSubgrupoConta"
            });
            dom.byId("linkAcoesSubgrupoConta").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridSubgrupoConta, 'todosItensSubgrupoConta', ['pesquisarSubgrupoConta', 'relatorioSubgrupoConta']); PesquisarSubgrupoContas(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridSubgrupoConta', 'selecionadoSubgrupoConta', 'cd_subgrupo_conta', 'selecionaTodosSubgrupoConta', ['pesquisarSubgrupoConta', 'relatorioSubgrupoConta'], 'todosItensSubgrupoConta'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensSubgrupoConta",
                dropDown: menu,
                id: "todosItensSubgrupoConta"
            });
            dom.byId("linkSelecionadosSubgrupoConta").appendChild(button.domNode);

            var storeSubG = [
                       { name: "1 Nível", id: 1 },
                       { name: "2 Níveis", id: 2 }
            ];
            criarOuCarregarCompFiltering("nivel", storeSubG, "", nivelParametro, ready, Memory, FilteringSelect, 'id', 'name', null);
            criarOuCarregarCompFiltering("cadNivel", storeSubG, "", 1, ready, Memory, FilteringSelect, 'id', 'name', null);

            // Botões para percistência

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    IncluirSubgrupoContas('apresentadorMensagemSubgrupoContas', null, 'formSubgrupoContas');
                }
            }, "incluirSubgrupoConta");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    AlterarSubgrupoContas('apresentadorMensagemSubgrupoContas', 'formSubgrupoContas');
                }
            }, "alterarSubgrupoConta");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarSubgrupoContas();
                    });
                }
            }, "deleteSubgrupoConta");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    limparCadSubgrupoContas();
                }
            }, "limparSubgrupoConta");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                keepValues(SUBGRUPO_CONTA, dijit.byId('gridSubgrupoConta'), null);
                }
            }, "cancelarSubgrupoConta");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarSubgrupoContas(true);
                }
            }, "pesquisarSubgrupoConta");
            decreaseBtn(document.getElementById("pesquisarSubgrupoConta"), '32px');

            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadSubgrupoConta").hide();
                }
            }, "fecharSubgrupoConta");
            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        confLayoutCadSubgrupo(NOVO);
                        limparCadSubgrupoContas();
                        xhr.get({
                            url: Endereco() + "/api/financeiro/getAllGrupoConta",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataGrupoConta) {
                            try{
                                loadGrupoConta(jQuery.parseJSON(dataGrupoConta).retorno, 'cd_grupo_subgrupo');
                                dijit.byId("cadSubgrupoConta").show();
                                dijit.byId("cadNivel").set("value", nivel_parametro);
                                apresentaMensagem('apresentadorMensagem', null);
                                IncluirAlterar(1, 'divAlterarSubgrupoConta', 'divIncluirSubgrupoConta', 'divExcluirSubgrupoConta', 'apresentadorMensagemSubgrupoConta', 'divCancelarSubgrupoConta', 'divClearSubgrupoConta');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagemSubgrupoContas', error);
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoSubgrupoConta");

            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: Endereco() + "/api/financeiro/getUrlRelatorioSubgrupoConta?" + getStrGridParameters('gridSubgrupoConta') + "desc=" + encodeURIComponent(document.getElementById("pesquisaSubgrupoConta").value) + "&inicio=" +
                            document.getElementById("inicioDescSubgrupoConta").checked + "&cdGrupo=" + retornaStatus("grupo_conta") + "&tipoNivel=" + dijit.byId("nivel").value,
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    })
                }
            }, "relatorioSubgrupoConta");
            dijit.byId("cadNivel").on("change", function (evento) {
                try {
                    if (evento == UMNIVEL) {
                        dojo.byId("trSuggrupo1").style.display = "none";
                        dojo.byId("lblSubgrupo").innerHTML = "Subgrupo 1 de Contas:";
                        dijit.byId("subgrupo1contas").set("required", false);
                    } else {
                        dojo.byId("trSuggrupo1").style.display = "";
                        dojo.byId("lblSubgrupo").innerHTML = "Subgrupo 2 de Contas:";
                        dijit.byId("subgrupo1contas").set("required", true);
                        if (hasValue(dijit.byId("cd_grupo_subgrupo").value))
                            loadSubgrupoPorGrupoContas(xhr, ready, Memory, FilteringSelect, dijit.byId("cd_grupo_subgrupo").value, null);
                    }
                } catch (e) {
                    postGerarLog(e);
                }
            });
            dijit.byId("cd_grupo_subgrupo").on("change", function (evento) {
                try{
                    if (evento > 0 && dijit.byId("cadNivel").value == DOISNIVEIS)
                        loadSubgrupoPorGrupoContas(xhr, ready, Memory, FilteringSelect, evento, null);
                } catch (e) {
                    postGerarLog(e);
                }
            });
            adicionarAtalhoPesquisa(['grupo_conta', 'pesquisaSubgrupoConta', 'nivel'], 'pesquisarSubgrupoConta', ready);
            xhr.get({
                url: Endereco() + "/api/financeiro/getAllGrupoConta",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Authorization": Token() }
            }).then(function (dataGrupoConta) {
                try{
                    loadGrupoConta(jQuery.parseJSON(dataGrupoConta).retorno, 'grupo_conta');
                    //dijit.byId("codProduto").set("value", 0);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            })
        } catch (e) {
            postGerarLog(e);
        }
    })
}

function montarGridTipoDesconto() {
    require([
            "dojo/_base/xhr",
            "dojo/dom",
            "dojox/grid/EnhancedGrid",
            "dojox/grid/enhanced/plugins/Pagination",
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory",
            "dojo/query",
            "dijit/form/Button",
            "dijit/form/DropDownButton",
            "dijit/DropDownMenu",
            "dijit/MenuItem",
            "dojo/ready",
            "dojox/json/ref",
            "dojo/domReady!",
            "dojo/parser"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, DropDownButton, DropDownMenu, MenuItem, ready,ref) {
        try{
            if (jQuery.parseJSON(MasterGeral()) == false)
                dijit.byId("tabContainerTpDesc_tablist_tabEscolaTpDesc").domNode.style.visibility = 'hidden';
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/gettipodescontosearch?descricao=&inicio=false&status=0&IncideBaixa=0&PParc=0&percentual=" + dijit.byId("percentual").value,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_desconto"
                }), Memory({ idProperty: "cd_tipo_desconto" }));
            dijit.byId("percentual").set("required", false);
            var gridTipoDesconto = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosTiposDesconto' style='display:none'/>", field: "selecionadoTipoDesconto", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxTipoDesconto },
                   // { name: "Código", field: "cd_tipo_desconto", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Tipo Desconto", field: "dc_tipo_desconto", width: "65%" },
                    { name: "Baixa", field: "incide_baixa", width: "12%", styles: "text-align: center;" },
                    //{ name: "1ºParcela", field: "incide_parcela_1", width: "10%", styles: "text-align: center;" },
                    { name: "Percentual", field: "pc_desc", width: "8%", styles: "text-align: right;" },
                    { name: "Ativo", field: "tipo_desconto_ativo", width: "12%", styles: "text-align: center;" }
                ],
                canSort: false,
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
            }, "gridTipoDesconto"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridTipoDesconto.pagination.plugin._paginator.plugin.connect(gridTipoDesconto.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridTipoDesconto, 'cd_tipo_desconto', 'selecionaTodosTiposDesconto');
            });
            gridTipoDesconto.startup();
            gridTipoDesconto.canSort = function (col) { return Math.abs(col) != 1; };
            gridTipoDesconto.on("RowDblClick", function (evt) {
                try{
                    var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                    getLimpar('#formTipoDesconto');
                    apresentaMensagem('apresentadorMensagem', '');
                    if (jQuery.parseJSON(MasterGeral()) == false)
                        dijit.byId("tabContainerTpDesc_tablist_tabEscolaTpDesc").domNode.style.visibility = 'hidden';
                    keepValues(TIPO_DESCONTO, gridTipoDesconto, false);
                    dijit.byId("cadTipoDesconto").show();
                    IncluirAlterar(0, 'divAlterarTipoDesconto', 'divIncluirTipoDesconto', 'divExcluirTipoDesconto', 'apresentadorMensagemTipoDesconto', 'divCancelarTipoDesconto', 'divLimparTipoDesconto');
                    limparGridEscola();
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridTipoDesconto, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosTiposDesconto').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_tipo_desconto', 'selecionadoTipoDesconto', -1, 'selecionaTodosTiposDesconto', 'selecionaTodosTiposDesconto', 'gridTipoDesconto')", gridTipoDesconto.rowsPerPage * 3);
                });
            });
            var dataMD = new Array();
            var gridEscola = new EnhancedGrid({
                store: dataStore = new ObjectStore({ objectStore: new Memory({ data: dataMD }) }),
                structure:
                  [
                    { name: "<input id='selecionaTodosEsc' style='display:none'/>", field: "selecionadoEsc", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxEsc },
                    { name: "Nome", field: "dc_reduzido_pessoa", width: "95%" }
                  ],
                canSort: true,
                noDataMessage: "Nenhum registro encontrado.",
                plugins: {
                    pagination: {
                        pageSizes: ["12", "24", "36", "50", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "12",
                        gotoButton: true,
                        maxPageStep: 5,
                        position: "button",
                        plugins: { nestedSorting: true }
                    }
                }
            }, "gridEscola");

            gridEscola.canSort = function (col) { return Math.abs(col) != 1; };
            gridEscola.startup();


            //***************** Adiciona link de ações:****************************\\
            var menu = new DropDownMenu({ style: "height: 25px" });
            var acaoEditar = new MenuItem({
                label: "Editar",
                onClick: function () { eventoEditarTipoDesconto(gridTipoDesconto.itensSelecionados); }
            });
            menu.addChild(acaoEditar);

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () { eventoRemover(gridTipoDesconto.itensSelecionados, 'DeletarTipoDesconto(itensSelecionados)'); }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasTipoDesconto",
                dropDown: menu,
                id: "acoesRelacionadasTipoDesconto"
            });
            dom.byId("linkAcoesTipoDesconto").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridTipoDesconto, 'todosItensTipoDesconto', ['pesquisarTipoDesconto', 'relatorioTipoDesconto']); PesquisarTipoDesconto(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridTipoDesconto', 'selecionadoTipoDesconto', 'cd_tipo_desconto', 'selecionaTodosTiposDesconto', ['pesquisarTipoDesconto', 'relatorioTipoDesconto'], 'todosItensTipoDesconto'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensTipoDesconto",
                dropDown: menu,
                id: "todosItensTipoDesconto"
            });
            dom.byId("linkSelecionadosTiposDesconto").appendChild(button.domNode);

            //*** Cria os botões de persistência **\\

            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    IncluirTipoDesconto();
                }
            }, "incluirTipoDesconto");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
                onClick: function () {
                    AlterarTipoDesconto();
                }
            }, "alterarTipoDesconto");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarTipoDesconto()
                    });
                }
            }, "deleteTipoDesconto");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    try{
                        getLimpar('#formTipoDesconto');
                        clearForm('formTipoDesconto');
                        dojo.byId('cd_tipo_desconto').value = 0;
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "limparTipoDesconto");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                keepValues(TIPO_DESCONTO, gridTipoDesconto, null);
                }
            }, "cancelarTipoDesconto");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadTipoDesconto").hide();
                }
            }, "fecharTipoDesconto");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarTipoDesconto(true);
                }
            }, "pesquisarTipoDesconto");
            decreaseBtn(document.getElementById("pesquisarTipoDesconto"), '32px');

            new Button({
                label: "Novo",
                iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                onClick: function () {
                    try{
                        dojo.byId('abriuEscola').value = false;
                        var tabContainerTpDesc = dijit.byId("tabContainerTpDesc");
                        tabContainerTpDesc.selectChild(tabContainerTpDesc.getChildren()[0]);
                        dijit.byId('tabPrincipalTpDesc').resize();
                        dijit.byId("cadTipoDesconto").show();

                        getLimpar('#formTipoDesconto');
                        dojo.byId('cd_tipo_desconto').value = 0;
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarTipoDesconto', 'divIncluirTipoDesconto', 'divExcluirTipoDesconto', 'apresentadorMensagemTipoDesconto', 'divCancelarTipoDesconto', 'divLimparTipoDesconto');
                        limparGridEscola();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoTipoDesconto");

            new Button({
                label: "Incluir",
                iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                onClick: function () {
                    try{
                        if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                            montargridPesquisaPessoa(function () {
                                dojo.query("#_nomePessoaFK").on("keyup", function (e) {
                                    if (e.keyCode == 13) pesquisarEscolasFK();
                                });
                                dijit.byId("pesqPessoa").on("click", function (e) {
                                    apresentaMensagem("apresentadorMensagemProPessoa", null);
                                    pesquisarEscolasFK();
                                });
                                abrirPessoaFK(false);
                            });
                        else
                            abrirPessoaFK(false);
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "btnaddEscola");


            // Adiciona link de ações:
            var menuEscola = new DropDownMenu({ style: "height: 25px" });

            var acaoRemover = new MenuItem({
                label: "Excluir",
                onClick: function () {
                    deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_pessoa', gridEscola);
                }
            });
            menuEscola.addChild(acaoRemover);

            var buttonEscola = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasEscola",
                dropDown: menuEscola,
                id: "acoesRelacionadasEscola"
            });
            dom.byId("linkEscola").appendChild(buttonEscola.domNode);
            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        preventCache: true,
                        url: !hasValue(document.getElementById("pesquisaTipoDesconto").value) ? Endereco() + "/api/financeiro/geturlrelatoriotipodesconto?" + getStrGridParameters('gridTipoDesconto') + "desc=&inicio=" + document.getElementById("inicioDescTipoDesconto").checked + "&status=" + retornaStatus("statusTipoDesconto") + "&IncideBaixa=" + retornaStatus("incideBaixa") + "&PParc=0&percentual=" + dijit.byId("percentual").value : Endereco() + "/api/financeiro/geturlrelatoriotipodesconto?" + getStrGridParameters('gridTipoDesconto') + "desc=" + encodeURIComponent(document.getElementById("pesquisaTipoDesconto").value) + "&inicio=" + document.getElementById("inicioDescTipoDesconto").checked + "&status=" + retornaStatus("statusTipoDesconto") + "&IncideBaixa=" + retornaStatus("incideBaixa") + "&PParc=false&percentual=" + dijit.byId("percentual").value,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        try{
                            abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagem', error);
                    });
                }
            }, "relatorioTipoDesconto");
            adicionarAtalhoPesquisa(['pesquisaTipoDesconto', 'statusTipoDesconto', 'incideBaixa', 'percentual'], 'pesquisarTipoDesconto', ready);
        } catch (e) {
            postGerarLog(e);
        }
    })
}



function montarGridOrgaoFinanceiro() {
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
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/ready"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, DropDownButton, DropDownMenu, MenuItem, ready) {
            try {
                montarStatus("statusOrgaoFinanceiro");
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getOrgaoFinanceiroSearch?descricao=&inicio=false&status=1",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_orgao_financeiro"
                }
                ), Memory({ idProperty: "cd_orgao_financeiro" }));

            var gridOrgaoFinanceiro = new EnhancedGrid({
                store: ObjectStore({ objectStore: myStore }),
                structure: [
                    { name: "<input id='selecionaTodosOrgaoFinanceiro' style='display:none'/>", field: "selecionadoOrgaoFinanceiro", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxOrgaoFinanceiro },
                    // { name: "Código", field: "cd_tipo_liquidacao", width: "75px", styles: "text-align: right; min-width:75px; max-width:75px;" },
                    { name: "Orgão Financeiro", field: "dc_orgao_financeiro", width: "85%" },
                    { name: "Ativo", field: "orgao_financeiro_ativo", width: "10%", styles: "text-align: center;" }
                ],
                canSort: false,
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
            }, "gridOrgaoFinanceiro"); // make sure you have a target HTML element with this id
            // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
            gridOrgaoFinanceiro.pagination.plugin._paginator.plugin.connect(gridOrgaoFinanceiro.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                verificaMostrarTodos(evt, gridOrgaoFinanceiro, 'cd_orgao_financeiro', 'selecionaTodosOrgaoFinanceiro');
            });
            gridOrgaoFinanceiro.startup();
            gridOrgaoFinanceiro.canSort = function (col) { return Math.abs(col) != 1; };
            gridOrgaoFinanceiro.on("RowDblClick", function (evt) {
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
                    getLimpar('#formOrgaoFinanceiro');
                    apresentaMensagem('apresentadorMensagem', '');
                    keepValues(EnumSelectOneMenu.ORGAO_FINANCEIRO, gridOrgaoFinanceiro, false);
                    dijit.byId("cadOrgaoFinanceiro").show();
                    IncluirAlterar(0, 'divAlterarOrgaoFinanceiro', 'divIncluirOrgaoFinanceiro', 'divExcluirOrgaoFinanceiro', 'apresentadorMensagemOrgaoFinanceiro', 'divCancelarOrgaoFinanceiro', 'divLimparOrgaoFinanceiro');
                } catch (e) {
                    postGerarLog(e);
                }
            }, true);

            require(["dojo/aspect"], function (aspect) {
                aspect.after(gridOrgaoFinanceiro, "_onFetchComplete", function () {
                    // Configura o check de todos:
                    if (dojo.byId('selecionaTodosOrgaoFinanceiro').type == 'text')
                        setTimeout("configuraCheckBox(false, 'cd_orgao_financeiro', 'selecionadoOrgaoFinanceiro', -1, 'selecionaTodosOrgaoFinanceiro', 'selecionaTodosOrgaoFinanceiro', 'gridOrgaoFinanceiro')", gridOrgaoFinanceiro.rowsPerPage * 3);
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
                        eventoEditarOrgaoFinanceiro(gridOrgaoFinanceiro.itensSelecionados);
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
                        eventoRemover(gridOrgaoFinanceiro.itensSelecionados, 'DeletarOrgaoFinanceiro(itensSelecionados)');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            });
            menu.addChild(acaoRemover);

            var button = new DropDownButton({
                label: "Ações Relacionadas",
                name: "acoesRelacionadasOrgaoFinanceiro",
                dropDown: menu,
                id: "acoesRelacionadasOrgaoFinanceiro"
            });
            dom.byId("linkAcoesOrgaoFinanceiro").appendChild(button.domNode);

            // Adiciona link de selecionados:
            menu = new DropDownMenu({ style: "height: 25px" });
            var menuTodosItens = new MenuItem({
                label: "Todos Itens",
                onClick: function () { buscarTodosItens(gridOrgaoFinanceiro, 'todosItensOrgaoFinanceiro', ['pesquisarOrgaoFinanceiro', 'relatorioOrgaoFinanceiro']); PesquisarOrgaoFinanceiro(false); }
            });
            menu.addChild(menuTodosItens);

            var menuItensSelecionados = new MenuItem({
                label: "Itens Selecionados",
                onClick: function () { buscarItensSelecionados('gridOrgaoFinanceiro', 'selecionadoOrgaoFinanceiro', 'cd_orgao_financeiro', 'selecionaTodosOrgaoFinanceiro', ['pesquisarOrgaoFinanceiro', 'relatorioOrgaoFinanceiro'], 'todosItensOrgaoFinanceiro'); }
            });
            menu.addChild(menuItensSelecionados);

            var button = new DropDownButton({
                label: "Todos Itens",
                name: "todosItensOrgaoFinanceiro",
                dropDown: menu,
                id: "todosItensOrgaoFinanceiro"
            });
            dom.byId("linkSelecionadosOrgaoFinanceiro").appendChild(button.domNode);

            // Criação de botões de persistência
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    IncluirOrgaoFinanceiro();
                }
            }, "incluirOrgaoFinanceiro");
            new Button({
                label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                onClick: function () {
                    AlterarOrgaoFinanceiro();
                }
            }, "alterarOrgaoFinanceiro");
            new Button({
                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                onClick: function () {
                    caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {
                        DeletarOrgaoFinanceiro();
                    });
                }
            }, "deleteOrgaoFinanceiro");
            new Button({
                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
                onClick: function () {
                    getLimpar('#formOrgaoFinanceiro');
                    clearForm('formOrgaoFinanceiro');
                }
            }, "limparOrgaoFinanceiro");
            new Button({
                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                onClick: function () {
                    keepValues(EnumSelectOneMenu.ORGAO_FINANCEIRO, gridOrgaoFinanceiro, false);
                }
            }, "cancelarOrgaoFinanceiro");
            new Button({
                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                onClick: function () {
                    dijit.byId("cadOrgaoFinanceiro").hide();
                }
            }, "fecharOrgaoFinanceiro");
            new Button({
                label: "",
                iconClass: 'dijitEditorIconSearchSGF',
                onClick: function () {
                    apresentaMensagem('apresentadorMensagem', null);
                    PesquisarOrgaoFinanceiro(true);
                }
            }, "pesquisarOrgaoFinanceiro");
            decreaseBtn(document.getElementById("pesquisarOrgaoFinanceiro"), '32px');
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
                        dijit.byId("cadOrgaoFinanceiro").show();
                        getLimpar('#formOrgaoFinanceiro');
                        clearForm('formOrgaoFinanceiro');
                        apresentaMensagem('apresentadorMensagem', null);
                        IncluirAlterar(1, 'divAlterarOrgaoFinanceiro', 'divIncluirOrgaoFinanceiro', 'divExcluirOrgaoFinanceiro', 'apresentadorMensagemOrgaoFinanceiro', 'divCancelarOrgaoFinanceiro', 'divLimparOrgaoFinanceiro');
                    } catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "novoOrgaoFinanceiro");

            //----Monta o botão de Relatório----
            new Button({
                label: getNomeLabelRelatorio(),
                iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                onClick: function () {
                    xhr.get({
                        url: !hasValue(document.getElementById("pesquisaOrgaoFinanceiro").value) ? Endereco() + "/api/financeiro/GetUrlRelatorioOrgaoFinanceiro?" + getStrGridParameters('gridOrgaoFinanceiro') + "desc=&inicio=" + document.getElementById("inicioDescOrgaoFinanceiro").checked + "&status=" + retornaStatus("statusOrgaoFinanceiro") : Endereco() + "/api/financeiro/geturlrelatorioorgaofinanceiro?" + getStrGridParameters('gridOrgaoFinanceiro') + "desc=" + encodeURIComponent(document.getElementById("pesquisaOrgaoFinanceiro").value) + "&inicio=" + document.getElementById("inicioDescOrgaoFinanceiro").checked + "&status=" + retornaStatus("statusOrgaoFinanceiro"),
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
            }, "relatorioOrgaoFinanceiro");
            adicionarAtalhoPesquisa(['pesquisaOrgaoFinanceiro', 'statusOrgaoFinanceiro'], 'pesquisarOrgaoFinanceiro', ready);
        } catch (e) {
            postGerarLog(e);
        }
    })
}


//**************************************************************** Persistência******************************************************************************

// Grupo de Item
function PesquisarGrupoEst(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaGrupoEst").value) ? Endereco() + "/api/financeiro/getgrupoestoquesearch?descricao=&inicio=" + document.getElementById("inicioDescGrupoEst").checked + "&status=" + retornaStatus("statusGrupoEstoque") + "&categoria=" + retornaStatus("categoria_grupo_item") : Endereco() + "/api/financeiro/getgrupoestoquesearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaGrupoEst").value) + "&inicio=" + document.getElementById("inicioDescGrupoEst").checked + "&status=" + retornaStatus("statusGrupoEstoque") + "&categoria=" + retornaStatus("categoria_grupo_item"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_grupo_estoque"
                }
                    ), Memory({ idProperty: "cd_grupo_estoque" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridGrupoEstoque = dijit.byId("gridGrupoEst");
            if (limparItens) {
                gridGrupoEstoque.itensSelecionados = [];
            }
            gridGrupoEstoque.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}
//Incluir Grupo Estoque
function IncluirGrupoEstoque() {
    try{
        apresentaMensagem('apresentadorMensagemGrupoEst', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formGrupoEstoque").validate())
            return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postgrupoestoque", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    no_grupo_estoque: dom.byId("no_grupo_estoque").value,
                    id_grupo_estoque_ativo: domAttr.get("id_grupo_estoque_ativo", "checked"),
                    id_categoria_grupo: dijit.byId("id_categoria_grupo").value,
                    id_eliminar_inventario: domAttr.get("id_eliminar_inventario", "checked")

                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridGrupoEst';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadGrupoEstoque').hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusGrupoEstoque").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_grupo_estoque", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoGrupoEstoque', 'cd_grupo_estoque', 'selecionaTodosGruposEstoque', ['pesquisarGrupoEst', 'relatorioGrupoEstoque'], 'todosItensGrupoEstoque');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_grupo_estoque");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemGrupoEst', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar GrupoEstoque
function AlterarGrupoEstoque() {
    try{
        var gridName = 'gridGrupoEst';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formGrupoEstoque").validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postalterargrupoestoque",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_grupo_estoque: dom.byId("cd_grupo_estoque").value,
                    no_grupo_estoque: dom.byId("no_grupo_estoque").value,
                    id_grupo_estoque_ativo: domAttr.get("id_grupo_estoque_ativo", "checked"),
                    id_categoria_grupo: dijit.byId("id_categoria_grupo").value,
                    id_eliminar_inventario: domAttr.get("id_eliminar_inventario", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensGrupoEstoque");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusGrupoEstoque").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadGrupoEstoque").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_grupo_estoque", dom.byId("cd_grupo_estoque").value);
                    insertObjSort(grid.itensSelecionados, "cd_grupo_estoque", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoGrupoEstoque', 'cd_grupo_estoque', 'selecionaTodosGruposEstoque', ['pesquisarGrupoEst', 'relatorioGrupoEstoque'], 'todosItensGrupoEstoque');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_grupo_estoque");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemGrupoEst', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar Grupo Estoque
function DeletarGrupoEstoque(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_grupo_estoque').value != 0)
                    itensSelecionados = [{
                        cd_grupo_estoque: dom.byId("cd_grupo_estoque").value,
                        no_grupo_estoque: dom.byId("no_grupo_estoque").value,
                        id_grupo_estoque_ativo: domAttr.get("id_grupo_estoque_ativo", "checked"),
                        id_categoria_grupo: dijit.byId("id_categoria_grupo").value,
                        id_eliminar_inventario: domAttr.get("id_eliminar_inventario", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeleteGrupoEstoque",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!jQuery.parseJSON(data).error) {
                        var todos = dojo.byId("todosItensGrupoEstoque");
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadGrupoEstoque").hide();
                        data = jQuery.parseJSON(data).retorno;
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridGrupoEst').itensSelecionados, "cd_grupo_estoque", itensSelecionados[r].cd_grupo_estoque);
                        PesquisarGrupoEst(true);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarGrupoEst").set('disabled', false);
                        dijit.byId("relatorioGrupoEstoque").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadGrupoEstoque").style.display))
                    apresentaMensagem('apresentadorMensagemGrupoEst', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

//****************************************************************
// Movimentação Financeira

//Pesquisar Movimentação Financeira
function PesquisarMovFinan(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaMovFinan").value) ? Endereco() + "/api/financeiro/getmovimentacaofinanceirasearch?descricao=&inicio=" + document.getElementById("inicioDescMovFinan").checked + "&status=" + retornaStatus("statusMovFinan") : Endereco() + "/api/financeiro/getmovimentacaofinanceirasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaMovFinan").value) + "&inicio=" + document.getElementById("inicioDescMovFinan").checked + "&status=" + retornaStatus("statusMovFinan"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_movimentacao_financeira"
                }
                        ), Memory({ idProperty: "cd_movimentacao_financeira" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridMovfinanceira = dijit.byId("gridMovFinan");
            if (limparItens) {
                gridMovfinanceira.itensSelecionados = [];
            }
            gridMovfinanceira.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

//Incluir Movimentação Financeira
function IncluirMovFinan() {
    try{
        if (!dijit.byId("formMovFinanceira").validate()) return false;
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemMovFinan', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postmovimentacaofinanceira", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_movimentacao_financeira: dom.byId("dc_movimentacao_financeira").value,
                    id_mov_financeira_ativa: domAttr.get("id_mov_financeira_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridMovFinan';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadMovFinan").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusMovFinan").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_movimentacao_financeira", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMovimentacaoFinanceira', 'cd_movimentacao_financeira', 'selecionaTodasMovimentacoes', ['pesquisarMovFinan', 'relatorioMovFinan'], 'todosItensMovimentacaoFinanceira');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_movimentacao_financeira");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMovFinan', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

// Alterar  Movimentação Financeira
function AlterarMovFinan() {
    try{
        var gridName = 'gridMovFinan';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formMovFinanceira').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postalterarmovimentacaofinanceira",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_movimentacao_financeira: dom.byId("cd_movimentacao_financeira").value,
                    dc_movimentacao_financeira: dom.byId("dc_movimentacao_financeira").value,
                    id_mov_financeira_ativa: domAttr.get("id_mov_financeira_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensMovimentacaoFinanceira");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusMovFinan").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadMovFinan").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_movimentacao_financeira", dom.byId("cd_movimentacao_financeira").value);
                    insertObjSort(grid.itensSelecionados, "cd_movimentacao_financeira", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoMovimentacaoFinanceira', 'cd_movimentacao_financeira', 'selecionaTodasMovimentacoes', ['pesquisarMovFinan', 'relatorioMovFinan'], 'todosItensMovimentacaoFinanceira');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_movimentacao_financeira");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemMovFinan', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}


// Deletar  Movimentação Financeira
function DeletarMovFinan(itensSelecionados) {
    
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_movimentacao_financeira').value != 0)
                    itensSelecionados = [{
                        cd_movimentacao_financeira: dom.byId("cd_movimentacao_financeira").value,
                        dc_movimentacao_financeira: dom.byId("dc_movimentacao_financeira").value,
                        id_mov_financeira_ativa: domAttr.get("id_mov_financeira_ativa", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeletemovimentacaofinanceira",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensMovimentacaoFinanceira");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadMovFinan").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridMovFinan').itensSelecionados, "cd_movimentacao_financeira", itensSelecionados[r].cd_movimentacao_financeira);
                    PesquisarMovFinan(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarMovFinan").set('disabled', false);
                    dijit.byId("relatorioMovFinan").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }

            },
            function (error) {
                apresentaMensagem(msg, error);

                if (!hasValue(dojo.byId("cadMovFinan").style.display))
                    apresentaMensagem('apresentadorMensagemMovFinan', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

//****************************************************************
//Tipo Liquidação
//Pesquisar  Tipo Liquidação
function PesquisarTpLiquidacao(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaTpLiquidacao").value) ? Endereco() + "/api/financeiro/gettipoliquidacaosearch?descricao=&inicio=" + document.getElementById("inicioDescTpLiquidacao").checked + "&status=" + retornaStatus("statusTpLiquidacao") : Endereco() + "/api/financeiro/gettipoliquidacaosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTpLiquidacao").value) + "&inicio=" + document.getElementById("inicioDescTpLiquidacao").checked + "&status=" + retornaStatus("statusTpLiquidacao"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_liquidacao"
                }
                        ), Memory({ idProperty: "cd_tipo_liquidacao" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTpLiquidacao = dijit.byId("gridTipoLiquidacao");
            if (limparItens) {
                gridTpLiquidacao.itensSelecionados = [];
            }
            gridTpLiquidacao.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}


//Incluir Tipo Liquidação
function IncluirTipoLiquidacao() {
    try{
        if (!dijit.byId("formTipoLiquidacao").validate()) { return false; }
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemTpLiquidacao', null);
        if (!dijit.byId('formTipoLiquidacao').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/posttipoliquidacao", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_tipo_liquidacao: dom.byId("dc_tipo_liquidacao").value,
                    id_tipo_liquidacao_ativa: domAttr.get("id_tipo_liquidacao_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadTipoLiquidacao").hide();
                    var gridName = 'gridTipoLiquidacao';
                    var grid = dijit.byId(gridName);
                    var todos = dojo.byId("todosItensTiposLiquidacao");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusTpLiquidacao").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_liquidacao", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoTipoLiquidacao', 'cd_tipo_liquidacao', 'selecionaTodosTiposLiquidacao', ['pesquisarTpLiquidacao', 'relatorioTpLiquidacao'], 'todosItensTipoLiquidacao');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_liquidacao");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTpLiquidacao', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar Tipo Liquidação
function AlterarTipoLiquidacao() {
    try{
        var gridName = 'gridTipoLiquidacao';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formTipoLiquidacao').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postalterartipoliquidacao",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_tipo_liquidacao: dom.byId("cd_tipo_liquidacao").value,
                    dc_tipo_liquidacao: dom.byId("dc_tipo_liquidacao").value,
                    id_tipo_liquidacao_ativa: domAttr.get("id_tipo_liquidacao_ativa", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensTipoLiquidacao");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusTpLiquidacao").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadTipoLiquidacao").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_tipo_liquidacao", dom.byId("cd_tipo_liquidacao").value);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_liquidacao", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoTipoLiquidacao', 'cd_tipo_liquidacao', 'selecionaTodosTiposLiquidacao', ['pesquisarTpLiquidacao', 'relatorioTpLiquidacao'], 'todosItensTipoLiquidacao');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_liquidacao");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTpLiquidacao', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar Tipo Liquidação
function DeletarTipoLiquidacao(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_liquidacao').value != 0)
                    itensSelecionados = [{
                        cd_tipo_liquidacao: dom.byId("cd_tipo_liquidacao").value,
                        dc_tipo_liquidacao: dom.byId("dc_tipo_liquidacao").value,
                        id_tipo_liquidacao_ativa: domAttr.get("id_tipo_liquidacao_ativa", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeletetipoliquidacao",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensTipoLiquidacao");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadTipoLiquidacao").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridTipoLiquidacao').itensSelecionados, "cd_tipo_liquidacao", itensSelecionados[r].cd_tipo_liquidacao);

                    PesquisarTpLiquidacao(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarTpLiquidacao").set('disabled', false);
                    dijit.byId("relatorioTpLiquidacao").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadTipoLiquidacao").style.display))
                    apresentaMensagem('apresentadorMensagemTpLiquidacao', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })

}

//****************************************************************
//Tipo Financeiro
function PesquisarTipoFinanceiro(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaTipoFinanceiro").value) ? Endereco() + "/api/financeiro/gettipofinanceirosearch?descricao=&inicio=" + document.getElementById("inicioDescTipoFinanceiro").checked + "&status=" + retornaStatus("statusTipoFinanceiro") : Endereco() + "/api/financeiro/gettipofinanceirosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoFinanceiro").value) + "&inicio=" + document.getElementById("inicioDescTipoFinanceiro").checked + "&status=" + retornaStatus("statusTipoFinanceiro"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_financeiro"
                }
                        ), Memory({ idProperty: "cd_tipo_financeiro" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridPesTipoFinanceiro = dijit.byId("gridTipoFinanceiro");
            if (limparItens) {
                gridPesTipoFinanceiro.itensSelecionados = [];
            }
            gridPesTipoFinanceiro.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });

}

//Tipo Financeiro
function IncluirTipoFinanceiro() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemTipoFinanceiro', null);
        if (!dijit.byId('formTipoFinanceiro').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/posttipofinanceiro", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_tipo_financeiro: dom.byId("dc_tipo_financeiro").value,
                    id_tipo_financeiro_ativo: domAttr.get("id_tipo_financeiro_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridTipoFinanceiro';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadTipoFinanceiro").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusTipoFinanceiro").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_financeiro", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoTipoFinanceiro', 'cd_tipo_financeiro', 'selecionaTodosTiposFinanceiros', ['pesquisarTipoFinanceiro', 'relatorioTipoFinanceiro'], 'todosItensTipoFinanceiro');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_financeiro");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoFinanceiro', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar Tipo Financeiro
function AlterarTipoFinanceiro() {
    try{
        var gridName = 'gridTipoFinanceiro';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formTipoFinanceiro').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postalterartipofinanceiro",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_tipo_financeiro: dom.byId("cd_tipo_financeiro").value,
                    dc_tipo_financeiro: dom.byId("dc_tipo_financeiro").value,
                    id_tipo_financeiro_ativo: domAttr.get("id_tipo_financeiro_ativo", "checked")
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensTipoFinanceiro");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusTipoFinanceiro").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadTipoFinanceiro").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_tipo_financeiro", dom.byId("cd_tipo_financeiro").value);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_financeiro", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoTipoFinanceiro', 'cd_tipo_financeiro', 'selecionaTodosTiposFinanceiros', ['pesquisarTipoFinanceiro', 'relatorioTipoFinanceiro'], 'todosItensTipoFinanceiro');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_financeiro");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoFinanceiro', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar Tipo Financeiro
function DeletarTipoFinanceiro(itensSelecionados) {
    try{
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (document.getElementById("cd_tipo_financeiro").value != 0)
                    itensSelecionados = [{
                        cd_tipo_financeiro: document.getElementById("cd_tipo_financeiro").value,
                        dc_tipo_financeiro: dijit.byId("dc_tipo_financeiro").value,
                        id_tipo_financeiro_ativo: dijit.byId("id_tipo_financeiro_ativo").checked
                    }];

            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeletetipofinanceiro",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItensTipoFinanceiro");

                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadTipoFinanceiro").hide();
                    dijit.byId("pesquisaTipoFinanceiro").set("value", '');
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridTipoFinanceiro').itensSelecionados, "cd_tipo_financeiro", itensSelecionados[r].cd_tipo_financeiro);

                    PesquisarTipoFinanceiro(false);

                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarTipoFinanceiro").set('disabled', false);
                    dijit.byId("relatorioTipoFinanceiro").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {

                if (!hasValue(dojo.byId("cadTipoFinanceiro").style.display))
                    apresentaMensagem('apresentadorMensagemTipoFinanceiro', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

//****************************************************************
//Tipo de Desconto
function PesquisarTipoDesconto(limparItens) {
    var per = document.getElementById("percentual").value;
    require([
                         "dojo/store/JsonRest",
                         "dojo/data/ObjectStore",
                         "dojo/store/Cache",
                         "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaTipoDesconto").value) ? Endereco() + "/api/financeiro/gettipodescontosearch?descricao=&inicio=" + document.getElementById("inicioDescTipoDesconto").checked + "&status=" + retornaStatus("statusTipoDesconto") + "&IncideBaixa=" + retornaStatus("incideBaixa") + "&PParc=0&percentual=" + parseFloat(document.getElementById("percentual").value, 10).toFixed(2) : Endereco() + "/api/financeiro/gettipodescontosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaTipoDesconto").value) + "&inicio=" + document.getElementById("inicioDescTipoDesconto").checked + "&status=" + retornaStatus("statusTipoDesconto") + "&IncideBaixa=" + retornaStatus("incideBaixa") + "&PParc=0&percentual=" + parseFloat(document.getElementById("percentual").value, 10).toFixed(2),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_tipo_desconto"
                }
            ), Memory({ idProperty: "cd_tipo_desconto" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridTipoDesconto = dijit.byId("gridTipoDesconto");

            if (limparItens) {
                gridTipoDesconto.itensSelecionados = [];
            }
            gridTipoDesconto.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

//Tipo Desconto
function IncluirTipoDesconto() {
    try{
        if (!dijit.byId('formTipoDesconto').validate()) return false;
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemTipoDesconto', null);
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            var escolas = montarEscolasGrid();
            showCarregando();
            xhr.post(Endereco() + "/api/escola/posttipodesconto", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_tipo_desconto: dom.byId("dc_tipo_desconto").value,
                    id_tipo_desconto_ativo: domAttr.get("id_tipo_desconto_ativo", "checked"),
                    //id_incide_parcela_1: domAttr.get("id_incide_parcela_1", "checked"),
                    id_incide_baixa: domAttr.get("id_incide_baixa", "checked"),
                    pc_desconto: dijit.byId("pc_desconto").value,
                    hasClickEscola: dojo.byId('abriuEscola').value,
                    escolas: escolas
                })
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridTipoDesconto';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        data = data.retorno;
                        dijit.byId("cadTipoDesconto").hide();
                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        dijit.byId("statusTipoDesconto").set("value", 0);
                        insertObjSort(grid.itensSelecionados, "cd_tipo_desconto", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionadoTipoDesconto', 'cd_tipo_desconto', 'selecionaTodosTiposDesconto', ['pesquisarTipoDesconto', 'relatorioTipoDesconto'], 'todosItensTipoDesconto');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_tipo_desconto");
                    }
                    else
                        apresentaMensagem('apresentadorMensagemTipoDesconto', data.erro);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoDesconto', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar Tipo de Desconto
function AlterarTipoDesconto() {
    try{
        var gridName = 'gridTipoDesconto';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formTipoDesconto').validate())
            return false;
        var escolas = montarEscolasGrid();
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/escola/postAlterarTipoDesconto",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            postData: dojox.json.ref.toJson({
                cd_tipo_desconto: dojo.byId("cd_tipo_desconto").value,
                dc_tipo_desconto: dojo.byId("dc_tipo_desconto").value,
                id_tipo_desconto_ativo: dijit.byId("id_tipo_desconto_ativo").get("checked"),
                //id_incide_parcela_1: dijit.byId("id_incide_parcela_1").get("checked"),
                id_incide_baixa: dijit.byId("id_incide_baixa").get("checked"),
                pc_desconto: dijit.byId("pc_desconto").get("value"),
                hasClickEscola: dojo.byId('abriuEscola').value,
                escolas: escolas
            })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var todos = dojo.byId("todosItensTipoDesconto");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusTipoDesconto").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadTipoDesconto").hide();
                    removeObjSort(grid.itensSelecionados, "cd_tipo_desconto", dojo.byId("cd_tipo_desconto").value);
                    insertObjSort(grid.itensSelecionados, "cd_tipo_desconto", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoTipoDesconto', 'cd_tipo_desconto', 'selecionaTodosTiposDesconto', ['pesquisarTipoDesconto', 'relatorioTipoDesconto'], 'todosItensTipoDesconto');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_tipo_desconto");

                }
                else
                    apresentaMensagem('apresentadorMensagemTipoDesconto', data.erro);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        },

        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemTipoDesconto', error);
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar tipo de Desconto
function DeletarTipoDesconto(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            var escolas = montarEscolasGrid();
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_tipo_desconto').value != 0)
                    itensSelecionados = [{
                        cd_tipo_desconto: dojo.byId("cd_tipo_desconto").value,
                        dc_tipo_desconto: dojo.byId("dc_tipo_desconto").value,
                        id_tipo_desconto_ativo: dijit.byId("id_tipo_desconto_ativo").get("checked"),
                        //id_incide_parcela_1: dijit.byId("id_incide_parcela_1").get("checked"),
                        id_incide_baixa: dijit.byId("id_incide_baixa").get("checked"),
                        pc_desconto: dijit.byId("pc_desconto").get("value"),
                        hasClickEscola: dojo.byId('abriuEscola').value,
                        escolas: escolas
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeletetipodesconto",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensTipoDesconto");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadTipoDesconto").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridTipoDesconto').itensSelecionados, "cd_tipo_desconto", itensSelecionados[r].cd_tipo_desconto);
                    PesquisarTipoDesconto(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarTipoDesconto").set('disabled', false);
                    dijit.byId("relatorioTipoDesconto").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadTipoDesconto").style.display))
                    apresentaMensagem('apresentadorMensagemTipoDesconto', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })

}

function montarEscolasGrid() {
    try{
        var gridEscolas = dijit.byId('gridEscola');
        var listaEscolas = [];
        if (hasValue(gridEscolas)) {
            if (gridEscolas.store != null && gridEscolas.store.objectStore != null && gridEscolas.store.objectStore.data.length > 0)
                listaEscolas = [];
            if (hasValue(gridEscolas) && gridEscolas.store.objectStore.data.length > 0) {
                var escolas = gridEscolas.store.objectStore.data;
                for (var i = 0; i < escolas.length; i++) {

                    listaEscolas.push({
                        cd_pessoa: escolas[i].cd_pessoa,
                        no_pessoa: escolas[i].no_pessoa
                    });
                }
            }
        }
        return listaEscolas;
    } catch (e) {
        postGerarLog(e);
    }
}

function popularGridEscola() {
    try{
        var cdTpDesconto = hasValue(dojo.byId('cd_tipo_desconto').value) ? dojo.byId('cd_tipo_desconto').value : 0;
        var gridEscolas = dijit.byId('gridEscola');
        var popular = dojo.byId('abriuEscola').value == false;
        if (gridEscolas != null && gridEscolas.store != null && gridEscolas.store.objectStore.data.length == 0 && popular == false) {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getEscolatWithTpDesc?cdTpDesc=" + cdTpDesconto,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                apresentaMensagem('apresentadorMensagemTipoDesconto', data);
                var empresas = data.retorno;
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: empresas }) }));
                gridEscolas.update();
                showCarregando();
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTipoDesconto', error);
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function limparGridEscola() {
    try{
        var gridEscolas = dijit.byId('gridEscola');
        if (hasValue(gridEscolas)) {
            gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridEscolas.update();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

// Métodos de persistencia para Item Servico
// Banco
function PesquisarBanco(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/getBancoSearch?nome=" + dojo.byId("descBanco").value + "&nmBanco=" + dojo.byId("numBanco").value + "&inicio=" + dojo.byId("inicioBanco").checked,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_banco"
                }
                    ), Memory({ idProperty: "cd_banco" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridBanco = dijit.byId("gridBanco");
            if (limparItens) {
                gridBanco.itensSelecionados = [];
            }
            gridBanco.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}


//Tipo Desconto
function IncluirBanco() {
    try {
        if (!dijit.byId('formBanco').validate()) return false;
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemBanco', null);
        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postBanco", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    no_banco: dijit.byId("no_banco").value,
                    nm_banco: dijit.byId("nm_banco").value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridBanco';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadBanco").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_banco", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoBanco', 'cd_banco', 'selecionaTodosBanco', ['pesquisarBanco', 'relatorioBanco'], 'todosItensBanco');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_banco");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemBanco', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar Tipo de Desconto
function AlterarBanco() {
    try{
        var gridName = 'gridBanco';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formBanco').validate()) return false;
        require(["dojo/_base/xhr", "dojox/json/ref", "dojo/dom"], function ( xhr, ref, dom) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postalterarBanco",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_banco: dojo.byId("cd_banco").value,
                    no_banco: dijit.byId("no_banco").value,
                    nm_banco: dojo.byId("nm_banco").value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensBanco");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadBanco").hide();
                    removeObjSort(grid.itensSelecionados, "cd_banco", dom.byId("cd_banco").value);
                    insertObjSort(grid.itensSelecionados, "cd_banco", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoBanco', 'cd_banco', 'selecionaTodosBanco', ['pesquisarBanco', 'relatorioBanco'], 'todosItensBanco');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_banco");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemBanco', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar tipo de Desconto
function DeletarBanco(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_banco').value != 0)
                    itensSelecionados = [{
                        cd_banco: dojo.byId("cd_banco").value,
                        no_banco: dijit.byId("no_banco").value,
                        nm_banco: dojo.byId("nm_banco").value
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeletebanco",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensBanco");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadBanco").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridBanco').itensSelecionados, "cd_banco", itensSelecionados[r].cd_banco);
                    PesquisarBanco(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarBanco").set('disabled', false);
                    dijit.byId("relatorioBanco").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadBanco").style.display))
                    apresentaMensagem('apresentadorMensagemBanco', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })

}
// Métodos de persistencia para Item Servico

// Funções para Link
function eventoEditarGrupoEstoque(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formGrupoEstoque');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(GRUPO_ESTOQUE, dijit.byId('gridGrupoEst'), true);
            dijit.byId("cadGrupoEstoque").show();
            IncluirAlterar(0, 'divAlterarGrupoEst', 'divIncluirGrupoEst', 'divExcluirGrupoEst', 'apresentadorMensagemGrupoEst', 'divCancelarGrupoEst', 'divLimparGrupoEst');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarMovimentacaoFinanceira(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formMovFinanceira');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(MOVIMENTACAO_FINANCEIRA, dijit.byId('gridMovFinan'), true);
            dijit.byId("cadMovFinan").show();
            IncluirAlterar(0, 'divAlterarMovFinan', 'divIncluirMovFinan', 'divExcluirMovFinan', 'apresentadorMensagemMovFinan', 'divCancelarMovFinan', 'divLimparMovFinan');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTipoLiquidacao(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formTipoLiquidacao');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(TIPO_LIQUIDACAO, dijit.byId('gridTipoLiquidacao'), true);
            dijit.byId("cadTipoLiquidacao").show();
            IncluirAlterar(0, 'divAlterarTpLiquidacao', 'divIncluirTpLiquidacao', 'divExcluirTpLiquidacao', 'apresentadorMensagemTpLiquidacao', 'divCancelarTpLiquidacao', 'divLimparTpLiquidacao');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTipoFinanceiro(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
        if (!eval(MasterGeral())) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return;
        }
            getLimpar('#formTipoFinanceiro');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(TIPO_FINANCEIRO, dijit.byId('gridTipoFinanceiro'), true);
            dijit.byId("cadTipoFinanceiro").show();
            IncluirAlterar(0, 'divAlterarTipoFinanceiro', 'divIncluirTipoFinanceiro', 'divExcluirTipoFinanceiro', 'apresentadorMensagemTipoFinanceiro', 'divCancelarTipoFinanceiro', 'divLimparTipoFinanceiro');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarTipoDesconto(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formTipoDesconto');
            apresentaMensagem('apresentadorMensagem', '');
            dojo.byId('abriuEscola').value = false;

            keepValues(TIPO_DESCONTO, dijit.byId('gridTipoDesconto'), true);
            dijit.byId("cadTipoDesconto").show();
            IncluirAlterar(0, 'divAlterarTipoDesconto', 'divIncluirTipoDesconto', 'divExcluirTipoDesconto', 'apresentadorMensagemTipoDesconto', 'divCancelarTipoDesconto', 'divLimparTipoDesconto');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarSubgrupoConta(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            limparCadSubgrupoContas();
            getLimpar('#formSubgrupoContas');
            IncluirAlterar(0, 'divAlterarSubgrupoConta', 'divIncluirSubgrupoConta', 'divExcluirSubgrupoConta', 'apresentadorMensagemSubgrupoConta', 'divCancelarSubgrupoConta', 'divClearSubgrupoConta');
            apresentaMensagem('apresentadorMensagem', '');
            keepValues(SUBGRUPO_CONTA, dijit.byId('gridSubgrupoConta'), true);
            dijit.byId("cadSubgrupoConta").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarGrupoContas(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formGrupoContas');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(GRUPO_CONTA, dijit.byId('gridGrupoContas'), true);
            dijit.byId("cadGrupoContas").show();
            IncluirAlterar(0, 'divAlterarGrupoContas', 'divIncluirGrupoContas', 'divExcluirGrupoContas', 'apresentadorMensagemGrupoContas', 'divCancelarGrupoContas', 'divLimparGrupoContas');
        }
    } catch (e) {
        postGerarLog(e);
    }
}


function eventoEditarBanco(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formBanco');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(BANCO, dijit.byId('gridBanco'), true);
            dijit.byId("cadBanco").show();
            IncluirAlterar(0, 'divAlterarBanco', 'divIncluirBanco', 'divExcluirBanco', 'apresentadorMensagemBanco', 'divCancelarBanco', 'divLimparBanco');
        }
    } catch (e) {
        postGerarLog(e);
    }
}
// Grupo de Contas
function PesquisarGrupoContas(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/financeiro/GetGrupoContaSearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaGrupoContas").value) + "&inicio=" + document.getElementById("inicioDescGrupoContas").checked + "&tipoGrupo=" + retornaStatus("tipo_grupo_conta"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_grupo_conta"
                }
                    ), Memory({ idProperty: "cd_grupo_conta" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridGrupoContas = dijit.byId("gridGrupoContas");
            if (limparItens) {
                gridGrupoContas.itensSelecionados = [];
            }
            gridGrupoContas.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function eventoEditarOrgaoFinanceiro(itensSelecionados) {
	try {
		if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
			caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
		else if (itensSelecionados.length > 1)
			caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
		else {
			getLimpar('#formOrgaoFinanceiro');
			apresentaMensagem('apresentadorMensagem', '');

			keepValues(EnumSelectOneMenu.ORGAO_FINANCEIRO, dijit.byId('gridOrgaoFinanceiro'), true);
			dijit.byId("cadOrgaoFinanceiro").show();
			IncluirAlterar(0, 'divAlterarOrgaoFinanceiro', 'divIncluirOrgaoFinanceiro', 'divExcluirOrgaoFinanceiro', 'apresentadorMensagemOrgaoFinanceiro', 'divCancelarOrgaoFinanceiro', 'divLimparOrgaoFinanceiro');
		}
	} catch (e) {
		postGerarLog(e);
	}
}

//Incluir Grupo Contas
function IncluirGrupoContas() {
    try{
        apresentaMensagem('apresentadorMensagemGrupoContas', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formGrupoContas").validate())
            return false;
        require(["dojo/dom", "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postgrupoconta", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    no_grupo_conta: dom.byId("no_grupo_conta").value,
                    id_tipo_grupo_conta: dijit.byId("id_tipo_grupo_conta").value,
                    nm_ordem_grupo: dijit.byId('ordem').value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridGrupoContas';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadGrupoContas').hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("tipo_grupo_conta").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_grupo_conta", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoGrupoContas', 'cd_grupo_conta', 'selecionaTodosGrupoContas', ['pesquisarGrupoContas', 'relatorioGrupoContas'], 'todosItensGrupoContas');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_grupo_conta");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemGrupoContas', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}


//Alterar GrupoContas
function AlterarGrupoContas() {
    try{
        var gridName = 'gridGrupoContas';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formGrupoContas").validate()) return false;
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postalterargrupoconta",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_grupo_conta: dom.byId("cd_grupo_conta").value,
                    no_grupo_conta: dom.byId("no_grupo_conta").value,
                    id_tipo_grupo_conta: dijit.byId("id_tipo_grupo_conta").value,
                    nm_ordem_grupo: dijit.byId('ordem').value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensGrupoContas");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("tipo_grupo_conta").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadGrupoContas").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_grupo_conta", dom.byId("cd_grupo_conta").value);
                    insertObjSort(grid.itensSelecionados, "cd_grupo_conta", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoGrupoContas', 'cd_grupo_conta', 'selecionaTodosGrupoContas', ['pesquisarGrupoContas', 'relatorioGrupoContas'], 'todosItensGrupoContas');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_grupo_conta");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemGrupoContas', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}


//Deletar GrupoContas
function DeletarGrupoContas(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_grupo_conta').value != 0)
                    itensSelecionados = [{
                        cd_grupo_conta: dom.byId("cd_grupo_conta").value,
                        no_grupo_conta: dom.byId("no_grupo_conta").value,
                        id_tipo_grupo_conta: dijit.byId("id_tipo_grupo_conta").value
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeletegrupoconta",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!jQuery.parseJSON(data).error) {
                        var todos = dojo.byId("todosItensGrupoContas");
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadGrupoContas").hide();
                        data = jQuery.parseJSON(data).retorno;
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridGrupoContas').itensSelecionados, "cd_grupo_conta", itensSelecionados[r].cd_grupo_conta);
                        PesquisarGrupoContas(true);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarGrupoContas").set('disabled', false);
                        dijit.byId("relatorioGrupoContas").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadGrupoContas").style.display))
                    apresentaMensagem('apresentadorMensagemGrupoContas', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}

// Subgrupo de Contas
function PesquisarSubgrupoContas(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStoreSg = Cache(
                 JsonRest({
                     target: Endereco() + "/api/financeiro/getSubgrupoContasearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaSubgrupoConta").value) + "&inicio=" + document.getElementById("inicioDescSubgrupoConta").checked +
                                          "&cdGrupo=" + retornaStatus("grupo_conta") + "&tipoNivel=" + retornaStatus("nivel"),
                     handleAs: "json",
                     preventCache: true,
                     headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                     idProperty: "cd_subgrupo_conta"
                 }
                     ), Memory({}));
            var dataStoreSg = new ObjectStore({ objectStore: myStoreSg });
            var gridSubgrupoContas = dijit.byId("gridSubgrupoConta");
            if (limparItens) {
                gridSubgrupoContas.itensSelecionados = [];
            }
            gridSubgrupoContas.setStore(dataStoreSg);
        } catch (e) {
            postGerarLog(e);
        }
    });

}

//Incluir Subgrupo Contas

function montarDataForPostSubGrupo() {
    try{
        var cdSubGrupoPai = 0;
        if (hasValue(dijit.byId("cadNivel").value) && dijit.byId("cadNivel").value == DOISNIVEIS)
            cdSubGrupoPai = hasValue(dijit.byId("subgrupo1contas").value) ? dijit.byId("subgrupo1contas").value : null;
        var dados = {
            cd_subgrupo_conta: dojo.byId("cd_subgrupo_conta").value,
            no_subgrupo_conta: dojo.byId("no_subgrupo_conta").value,
            cd_grupo_conta: dijit.byId("cd_grupo_subgrupo").value,
            nm_ordem_subgrupo: dijit.byId('ordem_subgrupo').value,
            dc_cod_integracao_plano: dojo.byId("descCdIntegracaoPlano").value,
            cd_subgrupo_pai: cdSubGrupoPai
        }
        return dados;
    } catch (e) {
        postGerarLog(e);
    }
}

function loadSubgrupoPorGrupoContas(xhr, ready, Memory, FilteringSelect, cdGrupoContas, cd_subgrupo_pai) {
    xhr.get({
        url: Endereco() + "/api/financeiro/GetSubgrupoContaByGrupoConta?cdGrupoConta=" + cdGrupoContas,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Authorization": Token() }
    }).then(function (dataSubgrupoConta) {
        try{
            //var cd_subgrupo_pai = dojo.byId("cd_subgrupo_pai").value;
            //var cdSubgrupoPai = hasValue(cd_subgrupo_pai) && cd_subgrupo_pai > 0 ? cd_subgrupo_pai : null;
            criarOuCarregarCompFiltering("subgrupo1contas", dataSubgrupoConta, "", cd_subgrupo_pai, ready, Memory, FilteringSelect, 'cd_subgrupo_conta', 'no_subgrupo_conta', null);
            if (!hasValue(dijit.byId("subgrupo1contas").value) || dijit.byId("subgrupo1contas").value <= 0)
                dijit.byId("subgrupo1contas").set("disabled", false);
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function confLayoutCadSubgrupo(evento) {
    try{
        var compNivel = dijit.byId("cadNivel");
        var compSubgrupo = dijit.byId("cd_grupo_subgrupo");
        var compSubgrupoPai = dijit.byId("subgrupo1contas");
        if (evento == NOVO) {
            compNivel.set("disabled", false);
            compSubgrupo.set("disabled", false);
            compSubgrupoPai.set("disabled", false);
        } else {
            compNivel.set("disabled", true);
            compSubgrupo.set("disabled", true);
            compSubgrupoPai.set("disabled", true);
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function limparCadSubgrupoContas() {
    try{
        getLimpar("#formSubgrupoContas");
        clearForm('formSubgrupoContas');
        dijit.byId("cadNivel").set("value", DOISNIVEIS);
        dijit.byId("subgrupo1contas").reset();
        dojo.byId("cd_subgrupo_conta").value = 0;
        dojo.byId("cd_subgrupo_pai").value = 0;
    } catch (e) {
        postGerarLog(e);
    }
}

function IncluirSubgrupoContas() {
    try{
        apresentaMensagem('apresentadorMensagemSubgrupoContas', null);
        apresentaMensagem('apresentadorMensagem', null);
        if (!dijit.byId("formSubgrupoContas").validate())
            return false;
        var dados = montarDataForPostSubGrupo();
        require(["dojo/request/xhr", "dojox/json/ref"], function ( xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postsubgrupoconta", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(dados)
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridSubgrupoConta';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId('cadSubgrupoConta').hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("grupo_conta").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_subgrupo_conta", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoSubgrupoConta', 'cd_subgrupo_conta', 'selecionaTodosSubgrupoConta', ['pesquisarSubgrupoConta', 'relatorioSubgrupoConta'], 'todosItensSubgrupoConta');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_subgrupo_conta");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemSubgrupoContas', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar SubgrupoContas
function AlterarSubgrupoContas() {
    try{
        var gridName = 'gridSubgrupoConta';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formSubgrupoContas").validate()) return false;
        var dados = montarDataForPostSubGrupo();
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postAlterarSubgrupoConta",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson(dados)
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensSubgrupoConta");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("grupo_conta").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadSubgrupoConta").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_subgrupo_conta", dom.byId("cd_subgrupo_conta").value);
                    insertObjSort(grid.itensSelecionados, "cd_subgrupo_conta", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoSubgrupoConta', 'cd_subgrupo_conta', 'selecionaTodosSubgrupoConta', ['pesquisarSubgrupoConta', 'relatorioSubgrupoConta'], 'todosItensSubgrupoConta');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_subgrupo_conta");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemSubgrupoContas', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar SubgrupoContas
function DeletarSubgrupoContas(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_subgrupo_conta').value != 0)
                    itensSelecionados = [{
                        cd_subgrupo_conta: dom.byId("cd_subgrupo_conta").value,
                        no_subgrupo_conta: dom.byId("no_subgrupo_conta").value,
                        cd_grupo_conta: dijit.byId("cd_grupo_subgrupo").value
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postDeleteSubgrupoConta",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!jQuery.parseJSON(data).error) {
                        var todos = dojo.byId("todosItensSubgrupoConta");
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cadSubgrupoConta").hide();
                        data = jQuery.parseJSON(data).retorno;
                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridSubgrupoConta').itensSelecionados, "cd_subgrupo_conta", itensSelecionados[r].cd_subgrupo_conta);
                        PesquisarSubgrupoContas(true);
                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarSubgrupoConta").set('disabled', false);
                        dijit.byId("relatorioSubgrupoConta").set('disabled', false);
                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadSubgrupoConta").style.display))
                    apresentaMensagem('apresentadorMensagemSubgrupoContas', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })
}



//****************************************************************
//Orgao Finaceiro
//Pesquisar  Orgao Financeiro
function PesquisarOrgaoFinanceiro(limparItens) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                JsonRest({
                    target: !hasValue(document.getElementById("pesquisaOrgaoFinanceiro").value) ? Endereco() + "/api/financeiro/getOrgaoFinanceiroSearch?descricao=&inicio=" + document.getElementById("inicioDescOrgaoFinanceiro").checked + "&status=" + retornaStatus("statusOrgaoFinanceiro") : Endereco() + "/api/financeiro/getorgaofinanceirosearch?descricao=" + encodeURIComponent(document.getElementById("pesquisaOrgaoFinanceiro").value) + "&inicio=" + document.getElementById("inicioDescOrgaoFinanceiro").checked + "&status=" + retornaStatus("statusOrgaoFinanceiro"),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_orgao_financeiro"
                }
                ), Memory({ idProperty: "cd_orgao_financeiro" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridOrgaoFinanceiro = dijit.byId("gridOrgaoFinanceiro");
            if (limparItens) {
                gridOrgaoFinanceiro.itensSelecionados = [];
            }
            gridOrgaoFinanceiro.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}


//Incluir Orgao Financeiro
function IncluirOrgaoFinanceiro() {
    try {
        if (!dijit.byId("formOrgaoFinanceiro").validate()) { return false; }
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemOrgaoFinanceiro', null);
        if (!dijit.byId('formOrgaoFinanceiro').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/financeiro/postOrgaoFinanceiro", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    dc_orgao_financeiro: dom.byId("dc_orgao_financeiro").value,
                    id_orgao_ativo: domAttr.get("id_orgao_financeiro_ativo", "checked")
                })
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadOrgaoFinanceiro").hide();
                    var gridName = 'gridOrgaoFinanceiro';
                    var grid = dijit.byId(gridName);
                    var todos = dojo.byId("todosItensOrgaoFinanceiro");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusOrgaoFinanceiro").set("value", 0);
                    insertObjSort(grid.itensSelecionados, "cd_orgao_financeiro", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoOrgaoFinanceiro', 'cd_orgao_financeiro', 'selecionaTodosOrgaoFinanceiro', ['pesquisarOrgaoFinanceiro', 'relatorioOrgaoFinanceiro'], 'todosItensOrgaoFinanceiro');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_orgao_financeiro");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemOrgaoFinanceiro', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Alterar Orgao Financeiro
function AlterarOrgaoFinanceiro() {
    try {
        var gridName = 'gridOrgaoFinanceiro';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formOrgaoFinanceiro').validate()) return false;
        require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/Financeiro/postAlterarOrgaoFinanceiro",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_orgao_financeiro: dom.byId("cd_orgao_financeiro").value,
                    dc_orgao_financeiro: dom.byId("dc_orgao_financeiro").value,
                    id_orgao_ativo: domAttr.get("id_orgao_financeiro_ativo", "checked")
                })
            }).then(function (data) {
                try {
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensOrgaoFinanceiro");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    dijit.byId("statusOrgaoFinanceiro").set("value", 0);
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadOrgaoFinanceiro").hide();
                    data = jQuery.parseJSON(data).retorno;
                    removeObjSort(grid.itensSelecionados, "cd_orgao_financeiro", dom.byId("cd_orgao_financeiro").value);
                    insertObjSort(grid.itensSelecionados, "cd_orgao_financeiro", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoOrgaoFinanceiro', 'cd_orgao_financeiro', 'selecionaTodosOrgaoFinanceiro', ['pesquisarOrgaoFinanceiro', 'relatorioOrgaoFinanceiro'], 'todosItensOrgaoFinanceiro');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_orgao_financeiro");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemOrgaoFinanceiro', error);
                });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

//Deletar Orgao Financeiro
function DeletarOrgaoFinanceiro(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_orgao_financeiro').value != 0)
                    itensSelecionados = [{
                        cd_orgao_financeiro: dom.byId("cd_orgao_financeiro").value,
                        dc_orgao_financeiro: dom.byId("dc_orgao_financeiro").value,
                        id_orgao_ativo: domAttr.get("id_orgao_financeiro_ativo", "checked")
                    }];
            xhr.post({
                url: Endereco() + "/api/Financeiro/postdeleteorgaofinanceiro",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItensOrgaoFinanceiro");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadOrgaoFinanceiro").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridOrgaoFinanceiro').itensSelecionados, "cd_orgao_financeiro", itensSelecionados[r].cd_orgao_financeiro);

                    PesquisarOrgaoFinanceiro(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarOrgaoFinanceiro").set('disabled', false);
                    dijit.byId("relatorioOrgaoFinanceiro").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    if (!hasValue(dojo.byId("cadOrgaoFinanceiro").style.display))
                        apresentaMensagem('apresentadorMensagemOrgaoFinanceiro', error);
                    else
                        apresentaMensagem('apresentadorMensagem', error);
                });
        } catch (e) {
            postGerarLog(e);
        }
    })

}

function abrirPessoaFK(isPesquisa) {
    dojo.ready(function () {
        try{
            dijit.byId("fkPessoaEsc").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoa").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoa").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoa").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoa").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoa").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("fkPessoaEsc").show();
            apresentaMensagem('apresentadorMensagemProPessoa', null);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaEscolaFK() {
    try{
        dojo.byId("_nomePessoaFK").value = "";
        dojo.byId("_apelido").value = "";
        dojo.byId("CnpjCpf").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoa"))) {
            dijit.byId("gridPesquisaPessoa").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoa").itensSelecionados))
                dijit.byId("gridPesquisaPessoa").itensSelecionados = [];
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function pesquisarEscolasFK() {
    var item = 0;

    require([
     "dojo/store/JsonRest",
     "dojo/data/ObjectStore",
     "dojo/store/Cache",
     "dojo/store/Memory",
     "dojo/ready",
     "dojo/domReady!",
     "dojo/parser"],
    function (JsonRest, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/escola/getEscolaNotWithItem?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&fantasia=" + dojo.byId("_apelido").value +
                                       "&cnpj=" + dojo.byId("CnpjCpf").value +
                                       "&cd_item=" + item +
                                        "&inicio=" + document.getElementById("inicioPessoaFK").checked,
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
                grid.layout.setColumnVisibility(5, false)
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function retornarPessoa() {
    try{
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        var gridEscolas = dijit.byId("gridEscola");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (dijit.byId("cadTipoDesconto").open) {
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'cd_pessoa');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "cd_pessoa", {
                        cd_pessoa: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa
                    });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
            }

        if (!valido)
            return false;
        dijit.byId("fkPessoaEsc").hide();
    } catch (e) {
        postGerarLog(e);
    }
}
//Carteira CNAB
function populaBancoCad(idBanco, field) {
    // Popula os produtos:
    require([ "dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getAllBanco",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataBanco) {
            try{
                loadBanco(dataBanco.retorno, field, idBanco);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemMat', error);
        });
    });
}
function populaBancoPesq(idBanco, field) {
    // Popula os produtos:
    require([ "dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getBancoCarteira",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataBanco) {
            try{
                loadBanco(dataBanco.retorno, field, idBanco);
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemCarteira', error);
        });
    });
}

function loadBanco(items, linkBanco, idBanco) {
    require(["dojo/store/Memory",  "dojo/_base/array"],
    function (Memory, Array) {
        try{
            var itemsCb = [];
            var cbBanco = dijit.byId(linkBanco);
            if ((linkBanco == 'cdBancoPesq'))
                itemsCb.push({ id: 0, name: "Todos" });


            Array.forEach(items, function (value, i) {
                itemsCb.push({ id: value.cd_banco, name: value.no_banco });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbBanco.store = stateStore;
            if ((linkBanco == 'cdBancoPesq'))
                cbBanco.set("value", 0);
            if (hasValue(idBanco)) {
                cbBanco._onChangeActive = false;
                cbBanco.set("value", idBanco);
                cbBanco._onChangeActive = true;
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function montarColuna(nomElement) {
    require(["dojo/store/Memory"],
     function (Memory) {
         try{
             var dados = [
                    { name: "240", id: "240" },
                    { name: "400", id: "400" }
             ]
             var statusStore = new Memory({
                 data: dados
             });
             dijit.byId(nomElement).store = statusStore;

             dijit.byId(nomElement).set("required", true);
         } catch (e) {
             postGerarLog(e);
         }
     });
}
function pesquisarCarteiraCNAB(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try{
            var banco = dijit.byId("cdBancoPesq").value > 0 ? dijit.byId("cdBancoPesq").value : 0;
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/cnab/getCarteiraCnabSearch?nome=" + dojo.byId("descCarteira").value + "&inicio=" + dojo.byId("ckInicio").checked + "&banco=" + banco + "&status=" + retornaStatus("statusCarteira"),
                    handleAs: "json",
                    headers: { "cd_carteira_cnab": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_banco"
                }
                    ), Memory({ idProperty: "cd_carteira_cnab" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridCarteira = dijit.byId("gridCarteira");
            if (limparItens) {
                gridCarteira.itensSelecionados = [];
            }
            gridCarteira.setStore(dataStore);
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function IncluirCarteiraCNAB() {
    try{
        if (!dijit.byId('formCarteira').validate()) return false;
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemCarteira', null);
        require(["dojo/request/xhr", "dojox/json/ref"], function (xhr, ref) {
            showCarregando();
            xhr.post(Endereco() + "/api/cnab/postInsertCarteira", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    no_carteira: dijit.byId("no_carteira").value,
                    cd_banco: dijit.byId("cdBanco").value,
                    bancoCarteira: dojo.byId("cdBanco").value,
                    nm_carteira: dijit.byId("nm_carteira").value,
                    id_registrada: dijit.byId("ckRegristrada").checked,
                    id_carteira_ativa: dijit.byId("ckCarteiraAtiva").checked,
                    nm_colunas: dijit.byId("nmColuna").value,
                    id_impressao: dijit.byId("tpImp").value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var gridName = 'gridCarteira';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadCarteira").hide();
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    insertObjSort(grid.itensSelecionados, "cd_carteira_cnab", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCarteira', 'cd_carteira_cnab', 'selecionaTodosCarteira', ['pesquisarCarteira', 'relatorioCarteira'], 'todosItensCarteira');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_carteira_cnab");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCarteira', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarCarteira(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else {
            getLimpar('#formCarteira');
            apresentaMensagem('apresentadorMensagem', '');

            keepValues(CARTEIRA, dijit.byId('gridCarteira'), true);
            dijit.byId("cadCarteira").show();
            IncluirAlterar(0, 'divAlterarCarteira', 'divIncluirCarteira', 'divExcluirCarteira', 'apresentadorMensagemCarteira', 'divCancelarCarteira', 'divClearCarteira');
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function AlterarCarteira() {
    try{
        var gridName = 'gridCarteira';
        var grid = dijit.byId(gridName);
        if (!dijit.byId('formCarteira').validate()) return false;
        require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/cnab/postAlterarCarteira",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: ref.toJson({
                    cd_carteira_cnab: dojo.byId("cd_carteira_cnab").value,
                    no_carteira: dojo.byId("no_carteira").value,
                    cd_banco: dijit.byId("cdBanco").value,
                    bancoCarteira: dojo.byId("cdBanco").value,
                    nm_carteira: dijit.byId("nm_carteira").value,
                    id_registrada: dijit.byId("ckRegristrada").checked,
                    id_carteira_ativa: dijit.byId("ckCarteiraAtiva").checked,
                    nm_colunas: dijit.byId("nmColuna").value,
                    id_impressao: dijit.byId("tpImp").value
                })
            }).then(function (data) {
                try{
                    var itemAlterado = jQuery.parseJSON(data).retorno;
                    var todos = dojo.byId("todosItensCarteira");
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadCarteira").hide();
                    removeObjSort(grid.itensSelecionados, "cd_carteira_cnab", dojo.byId("cd_carteira_cnab").value);
                    insertObjSort(grid.itensSelecionados, "cd_carteira_cnab", itemAlterado);

                    buscarItensSelecionados(gridName, 'selecionadoCarteira', 'cd_carteira_cnab', 'selecionaTodosCarteira', ['pesquisarCarteira', 'relatorioCarteira'], 'todosItensCarteira');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_carteira_cnab");
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemCarteira', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarCarteira(itensSelecionados) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try{
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cd_carteira_cnab').value != 0)
                    itensSelecionados = [{
                        cd_carteira_cnab: dojo.byId("cd_carteira_cnab").value,
                        no_carteira: dojo.byId("no_carteira").value,
                        cd_banco: dijit.byId("cdBanco").value
                    }];
            xhr.post({
                url: Endereco() + "/api/cnab/postDeleteCarteira",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    var todos = dojo.byId("todosItensCarteira");
                    apresentaMensagem('apresentadorMensagem', data);
                    data = jQuery.parseJSON(data).retorno;
                    dijit.byId("cadCarteira").hide();
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridCarteira').itensSelecionados, "cd_carteira_cnab", itensSelecionados[r].cd_carteira_cnab);
                    pesquisarCarteiraCNAB(true);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarCarteira").set('disabled', false);
                    dijit.byId("relatorioCarteira").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cadCarteira").style.display))
                    apresentaMensagem('apresentadorMensagemCarteira', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        } catch (e) {
            postGerarLog(e);
        }
    })

}