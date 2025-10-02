var FECHCAIXA = 8, TODOS = 0, CAIXAS = 3, BANCOS = 2;
var nomeUsuarioLogado = "", cdUsuarioLogado = 0;

var EnumTipoMovimento = {
    ENTRADA: 1,
    SAIDA: 2
}

var EnumTipoMovimentacaoFinanceira = {
    ACERTO_FECHAMENTO_CAIXA: 9
}

function pesquisarFechamentoCaixaTpLiquidacao(pFuncao) {
    try {
	    showCarregando(); 
        var usuario = hasValue(dojo.byId("cdFkUsuario").value) ? dojo.byId("cdFkUsuario").value : 0;
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getFechamentoCaixaTpLiquidacao?dta_fechamento=" + dojo.byId("dataFechamento").value + "&cdUsuario=" + usuario + "&tipoLocal=" + cbCategoria.value +
                "&mostrarZerados=" + dijit.byId('ckZerados').checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
		    hideCarregando();
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null)
                destroyCreateGridSaldoTipo(data.retorno, pFuncao);
        },
            function (error) {
	            hideCarregando();
                if (hasValue(dojo.byId("cadContaCorrente")) && !hasValue(dojo.byId("cadContaCorrente").style.display))
                apresentaMensagem('apresentadorMensagemContaCorrente', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
	    hideCarregando();
        postGerarLog(e);
    }
}

function pesquisarFechamentoCaixaLocalMovto(tipoLiquidacao, IsTagSaldoCaixa) {
    try {
	    showCarregando();
        var usuario = hasValue(dojo.byId("cdFkUsuario").value) ? dojo.byId("cdFkUsuario").value : 0;
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getFechamentoCaixaLocalMovto?dta_fechamento=" + dojo.byId("dataFechamento").value + "&tipoLiquidacao=" + tipoLiquidacao + "&cdUsuario=" + usuario + "&tipoLocal=" + cbCategoria.value +
                "&mostrarZerados=" + dijit.byId('ckZerados').checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
		    hideCarregando();
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null)
                destroyCreateGridSaldoCaixa(data.retorno, IsTagSaldoCaixa);
        },
            function (error) {
	            hideCarregando();
            if (!hasValue(dojo.byId("cadContaCorrente").style.display))
                apresentaMensagem('apresentadorMensagemContaCorrente', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
	    hideCarregando();
        postGerarLog(e);
    }
}

function destroyCreateGridSaldoCaixa(dados, IsTagSaldoCaixa) {
    require([
			'dojo/_base/declare',
			'dojo/_base/lang',
			'dojo/dom',
			'dojo/on',
			'dstore/Memory',
			'dgrid/OnDemandGrid',
			'dgrid/ColumnSet',
			'my/SummaryRow',
			'dgrid/extensions/DijitRegistry',
			'dgrid/Editor',
            'dgrid/Selector',
            'dijit/Tooltip'
    ], function (declare, lang, dom, on, Memory, OnDemandGrid, ColumnSet, SummaryRow, DijitRegistry, Editor, Selector, Tooltip) {
        try {
            dijit.byId('tagSaldoCaixa').set('open', true);
            // Deve-se colocar um identificador hardcode nos registros, para o dgrid separar as linhas no clique simples.
            if (hasValue(dados))
                for (var i = 0; i < dados.length; i++)
                    dados[i].id = i;

            var store = new Memory({
                data: dados
            });

            store.getTotals = function () {
                var totals = {};
                totals.saldo_inicial = totals.vl_entrada = totals.vl_saida = totals.saldo_final = 0;

                for (var i = this.data.length; i--;)
                    for (var k in totals)
                        totals[k] += this.data[i][k];

                totals.des_local_movto = 'Total';
                totals.saldo_inicial = maskFixed(totals.saldo_inicial + "", 2);
                totals.vl_entrada = maskFixed(totals.vl_entrada + "", 2);
                totals.vl_saida = maskFixed(totals.vl_saida + "", 2);
                totals.saldo_final = maskFixed(totals.saldo_final + "", 2);
                return totals;
            }

            // Create an instance of OnDemandGrid referencing the store
            var commonArgs = {
                //className: 'dgrid-autoheight',
                collection: store,
                sort: 'des_local_movto'
            };

            var gridSaldoCaixa = dijit.byId('registryGridSaldoCaixa').registry;

            if (hasValue(gridSaldoCaixa)) {
                gridSaldoCaixa.destroy();
                $('<div style="height: 100%;width:100%">').attr('id', 'gridSaldoCaixa').appendTo('#divSaldoCaixa');
            }

            gridSaldoCaixa = new (declare([OnDemandGrid, SummaryRow, DijitRegistry, Editor, Selector]))(lang.mixin({
                noDataMessage: msgNotRegEnc,
                columns: [
                    { label: 'Local', field: 'des_local_movto', sortable: true },
                    { label: 'Saldo Anterior', field: 'saldo_inicial', className: "dgrid_rigth", formatter: formatValorMonetario },
                    { label: 'Entrada', field: 'vl_entrada', className: "dgrid_rigth", formatter: formatValorMonetario },
                    { label: 'Saída', field: 'vl_saida', className: "dgrid_rigth", formatter: formatValorMonetario },
                    {
                        label: 'Saldo Atual', field: 'saldo_final', className: "dgrid_rigth", formatter: formatValorMonetario, editor: 'text',
                        editOn: 'dblclick'}
                ]
            }, commonArgs), 'gridSaldoCaixa');

            gridSaldoCaixa.on("dgrid-datachange", lang.hitch(this, function (evt) {
                var row = gridSaldoCaixa.row(event);
                var item = row.data;
                
                apresentaMensagem("apresentadorMensagemAjusteContaCorrente", null);
                //se tem valor e não é um numero
                if (hasValue(evt.value) && isNaN(unmaskFixed(evt.value, 2))) {
                    //evt.value = unmaskFixed(evt.oldValue, 2);
                    evt.returnValue = false;
                }
                //se nao tem valor
                else if (!hasValue(evt.value)) {
	                evt.returnValue = false;
                    //evt.value = unmaskFixed(evt.oldValue, 2);
                } else if (/[a-z/~!@#$%&?\[\]\{\}=°º£¢¬_A-Z§´`'"*<>;:ª~^()¨|\u00C0-\u00FF ]+/i.test(evt.value)) {

	                evt.returnValue = false;
                    caixaDialogo(DIALOGO_AVISO, 'O valor inserido não pode conter letras, espaços e caracteres especiais.(Ex:/,~!@#$%&?[]{} ...)', null);
                }
                else if (hasValue(evt.value) && ((evt.value.split(",").length - 1) > 0)) {
	                evt.returnValue = false;
                    caixaDialogo(DIALOGO_AVISO, 'Use o caracter ponto (.) para separar as casas decimais do valor ajustado.', null);
	                
                }
                else if (!hasValue(dojo.byId("dataFechamento").value)) {
	                evt.returnValue = false;
	                caixaDialogo(DIALOGO_AVISO, 'Preencha o campo (Data) para poder ajustar os valores', null);
	                
                }
                else {

                    dijit.byId("cadAjusteContaCorrente").show();

	                var valor_atual = unmaskFixed(evt.value, 2);
	                var valor_antigo = unmaskFixed(evt.oldValue, 2);

	                var contaCorrenteSave = null;

                    if (valor_atual > valor_antigo) {
                        //calcula o valor do ajuste
                        var valor_ajuste_entrada = 0;
                        valor_ajuste_entrada = unmaskFixed(valor_atual - valor_antigo, 2);
                        var tipo = "";
                        if (dijit.byId('categoria').value == 3)
                            tipo = 'de Caixa'
                        else
                            tipo = 'de Banco'
                        //Cria uma instancia do objeto de conta corrente a ser salvo
                        var cd_tipo_liquidacao = hasValue(dijit.byId("gridSaldoTipo").itemSelecionado) ? dijit.byId("gridSaldoTipo").itemSelecionado.cd_tipo_liquidacao : dijit.byId("gridSaldoTipo").collection.data[0].cd_tipo_liquidacao;
                        var dc_tipo_liquidacao = hasValue(dijit.byId("gridSaldoTipo").itemSelecionado) ? dijit.byId("gridSaldoTipo").itemSelecionado.dc_tipo_liquidacao : dijit.byId("gridSaldoTipo").collection.data[0].dc_tipo_liquidacao;
                        contaCorrenteSave = new ContaCorrenteObjeto(0, row.data.des_local_movto, "", "Acerto Fechamento " + tipo, "", row.data.cd_local_movimento,
                            0, cd_tipo_liquidacao, EnumTipoMovimento.ENTRADA, EnumTipoMovimentacaoFinanceira.ACERTO_FECHAMENTO_CAIXA,
                            0, valor_ajuste_entrada, valor_ajuste_entrada, "", dojo.date.locale.parse(dojo.byId("dataFechamento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                            dc_tipo_liquidacao, evt.oldValue, row.data.id);

                        //reseta os campos do popup de ajuste
                        resetCamposAjusteFechamentoCaixa();
                        
                        dojo.byId("objAjuste").value = JSON.stringify(contaCorrenteSave);
                        dijit.byId("cbOrigemAjusteCad").set("disabled", false);
                        dijit.byId("cbDestinoAjusteCad").set("disabled", false);
                        dijit.byId("cbAjusteLiquidacao").set("disabled", false);
                        dijit.byId("idMovimentoAjusteCad").set("disabled", false);
                        dijit.byId("idTipoCad").set("disabled", false);
                        dijit.byId("dtaAjusteCc").set("disabled", false);
                        dijit.byId("valContaAjusteCorrente").set("disabled", false);

                        //preenche os campos do popup de ajuste
                        montarCombo(Memory, "cbOrigemAjusteCad", [{ name: contaCorrenteSave.origem, id: contaCorrenteSave.cd_local_origem }], contaCorrenteSave.cd_local_origem);
						montarCombo(Memory, "cbAjusteLiquidacao", [{ name: contaCorrenteSave.dc_tipo_liquidacao, id: contaCorrenteSave.cd_tipo_liquidacao }], contaCorrenteSave.cd_tipo_liquidacao);
						montarCombo(Memory, "idMovimentoAjusteCad", [{ name: contaCorrenteSave.movimento, id: contaCorrenteSave.cd_movimentacao_financeira }], contaCorrenteSave.cd_movimentacao_financeira);
						montarCombo(Memory, "idTipoCad", [{ name: "Entrada", id: contaCorrenteSave.id_tipo_movimento }], contaCorrenteSave.id_tipo_movimento);
						dijit.byId("dtaAjusteCc").set("value", contaCorrenteSave.dta_conta_corrente);
                        dijit.byId("valContaAjusteCorrente").set("value", contaCorrenteSave.vl_conta_corrente);

                        //desabilita os campos que não podem ser alterados
                        dijit.byId("cbOrigemAjusteCad").set("disabled", true);
                        dijit.byId("cbDestinoAjusteCad").set("disabled", true);
                        dijit.byId("cbAjusteLiquidacao").set("disabled", true);
                        dijit.byId("idMovimentoAjusteCad").set("disabled", true);
                        dijit.byId("idTipoCad").set("disabled", true);
                        dijit.byId("dtaAjusteCc").set("disabled", true);
                        dijit.byId("valContaAjusteCorrente").set("disabled", true);
                        dojo.byId("oldValueAjuste").value = contaCorrenteSave.oldValue;
                        dojo.byId("dataId").value = contaCorrenteSave.dataId;
                        //dijit.byId('desObsAjusteCc').set('value', value.dc_obs_conta_corrente);


                        //console.log("Entrada:", contaCorrenteSave);

                    } else if (valor_atual < valor_antigo) {

	                    //calcula o valor do ajuste
                        var valor_ajuste_saida = 0;
                        valor_ajuste_saida = unmaskFixed(valor_antigo - valor_atual, 2);

                        //Cria uma instancia do objeto de conta corrente a ser salvo
                        var cd_tipo_liquidacao = hasValue(dijit.byId("gridSaldoTipo").itemSelecionado) ? dijit.byId("gridSaldoTipo").itemSelecionado.cd_tipo_liquidacao : dijit.byId("gridSaldoTipo").collection.data[0].cd_tipo_liquidacao;
                        var dc_tipo_liquidacao = hasValue(dijit.byId("gridSaldoTipo").itemSelecionado) ? dijit.byId("gridSaldoTipo").itemSelecionado.dc_tipo_liquidacao : dijit.byId("gridSaldoTipo").collection.data[0].dc_tipo_liquidacao;

                        contaCorrenteSave = new ContaCorrenteObjeto(0, row.data.des_local_movto, "", "Acerto Fechamento de Caixa", "", row.data.cd_local_movimento,
                            0, cd_tipo_liquidacao, EnumTipoMovimento.SAIDA, EnumTipoMovimentacaoFinanceira.ACERTO_FECHAMENTO_CAIXA,
                            0, valor_ajuste_saida, valor_ajuste_saida, "", dojo.date.locale.parse(dojo.byId("dataFechamento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                            dc_tipo_liquidacao, evt.oldValue, row.data.id);

                        //reseta os campos do popup de ajuste
                        resetCamposAjusteFechamentoCaixa();

                        dojo.byId("objAjuste").value = JSON.stringify(contaCorrenteSave);

                        dijit.byId("cbOrigemAjusteCad").set("disabled", false);
                        dijit.byId("cbDestinoAjusteCad").set("disabled", false);
                        dijit.byId("cbAjusteLiquidacao").set("disabled", false);
                        dijit.byId("idMovimentoAjusteCad").set("disabled", false);
                        dijit.byId("idTipoCad").set("disabled", false);
                        dijit.byId("dtaAjusteCc").set("disabled", false);
                        dijit.byId("valContaAjusteCorrente").set("disabled", false);

                        //preenche os campos do popup de ajuste
                        montarCombo(Memory, "cbOrigemAjusteCad", [{ name: contaCorrenteSave.origem, id: contaCorrenteSave.cd_local_origem }], contaCorrenteSave.cd_local_origem);
                        montarCombo(Memory, "cbAjusteLiquidacao", [{ name: contaCorrenteSave.dc_tipo_liquidacao, id: contaCorrenteSave.cd_tipo_liquidacao }], contaCorrenteSave.cd_tipo_liquidacao);
                        montarCombo(Memory, "idMovimentoAjusteCad", [{ name: contaCorrenteSave.movimento, id: contaCorrenteSave.cd_movimentacao_financeira }], contaCorrenteSave.cd_movimentacao_financeira);
                        montarCombo(Memory, "idTipoCad", [{ name: "Saída", id: contaCorrenteSave.id_tipo_movimento }], contaCorrenteSave.id_tipo_movimento);
                        dijit.byId("dtaAjusteCc").set("value", contaCorrenteSave.dta_conta_corrente);
                        dijit.byId("valContaAjusteCorrente").set("value", contaCorrenteSave.vl_conta_corrente);

                        //desabilita os campos que não podem ser alterados
                        dijit.byId("cbOrigemAjusteCad").set("disabled", true);
                        dijit.byId("cbDestinoAjusteCad").set("disabled", true);
                        dijit.byId("cbAjusteLiquidacao").set("disabled", true);
                        dijit.byId("idMovimentoAjusteCad").set("disabled", true);
                        dijit.byId("idTipoCad").set("disabled", true);
                        dijit.byId("dtaAjusteCc").set("disabled", true);
                        dijit.byId("valContaAjusteCorrente").set("disabled", true);
                        dojo.byId("oldValueAjuste").value = contaCorrenteSave.oldValue;
                        dojo.byId("dataId").value = contaCorrenteSave.dataId;

                        //console.log("Saida:", contaCorrenteSave);

                    } else { evt.returnValue = false;}

                }

            }));

            gridSaldoCaixa.itemSelecionado = [];
            var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
            if (IsStatusFiltro == 'Saldo Individual') {
                var SelecaoUsuarioPadrao = hasValue(gridSaldoCaixa.collection.data[0]) ? gridSaldoCaixa.collection.data[0] : null;
                if (hasValue(SelecaoUsuarioPadrao)) {
                    gridSaldoCaixa.itemSelecionado = SelecaoUsuarioPadrao;
                    dijit.byId('tgObsCaixa').set('title', 'Observação de detalhamento ' + SelecaoUsuarioPadrao.des_local_movto);
                    dijit.byId('descObsCaixa').set('value', hasValue(SelecaoUsuarioPadrao.obsSaldoCaixa) ? SelecaoUsuarioPadrao.obsSaldoCaixa.tx_obs_saldo_caixa : "");
                }
                gridSaldoCaixa.on('.dgrid-cell:click', function (event) {
                    var row = gridSaldoCaixa.row(event);
                    if (hasValue(row)) {
                        var item = row.data;
                        dijit.byId('tgObsCaixa').set('title', 'Observação de detalhamento ' + item.des_local_movto);
                        dijit.byId('tgObsCaixa').set('open', true);
                        if (hasValue(item.obsSaldoCaixa))
                            dijit.byId('descObsCaixa').set('value', item.obsSaldoCaixa.tx_obs_saldo_caixa);
                        else
                            dijit.byId('descObsCaixa').set('value', "");
                        gridSaldoCaixa.itemSelecionado = item;
                    }
                });
            }

            var totals = store.getTotals();
            gridSaldoCaixa.set('summary', totals);
            dijit.byId('registryGridSaldoCaixa').registry = gridSaldoCaixa;

            if (IsStatusFiltro == 'Saldo Consolidado') {
                obsConsolidado = montarObsSaldoCaixa(dijit.byId('gridSaldoCaixa'));
                obsConsolidado.cd_usuario = cdUsuarioLogado;
                getObsSaldoCaixaConsolidado(obsConsolidado);
            }
            new Tooltip({
                connectId: gridSaldoCaixa.domNode,
                selector: "td",
                getContent: function (matchedNode) {
                    if (matchedNode.cellIndex == 4)
                        return "Duplo Clique para editar valor do Saldo Atual para ajustar valor."
                }
            });

            dijit.byId('tagSaldoCaixa').set('open', false);
            if(hasValue(IsTagSaldoCaixa) && IsTagSaldoCaixa)
                dijit.byId('tagSaldoCaixa').set('open', true);

        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadCategoria(categoria)
{
    var statusStore = new dojo.store.Memory({
        data: [
            { name: "Todos", id: 0 },
            { name: "Caixas", id: 3 },
            { name: "Bancos", id: 2 }
        ]
    });
    cbCategoria = dijit.byId('categoria');
    cbCategoria.store = statusStore;
    cbCategoria._onChangeActive = false;
    cbCategoria.set("value", CAIXAS);
    cbCategoria._onChangeActive = true;
    cbCategoria.on("change", function (e) {
        switch (e) {
            case CAIXAS:
                if (!hasValue(dojo.byId("cdFkUsuario").value) || parseInt(dojo.byId("cdFkUsuario").value) == 0) {
                    dijit.byId("no_usuario").set("value", nomeUsuarioLogado);
                    dojo.byId("cdFkUsuario").value = cdUsuarioLogado;
                    dijit.byId('tagSaldoTipo').set('title', 'Resumo das Movimentações de Caixas');
                    dijit.byId('visoes').set('disabled', false);
                    populaSaldoIndividual()
                }
                dijit.byId('pesUsuarioFK').set('disabled', false);
                break;
            case BANCOS:
                dijit.byId('no_usuario').set('value', '');
                dojo.byId("cdFkUsuario").value = 0;
                dijit.byId('pesUsuarioFK').set('disabled', true);
                dijit.byId('tagSaldoTipo').set('title', 'Resumo das Movimentações de Bancos');
                dijit.byId('visoes').set('disabled', true);
                populaSaldoConsolidado()
                break;
            case TODOS:
                dijit.byId('no_usuario').set('value', '');
                dojo.byId("cdFkUsuario").value = 0;
                dijit.byId('pesUsuarioFK').set('disabled', true);
                dijit.byId('tagSaldoTipo').set('title', 'Resumo das Movimentações de Caixas/Bancos');
                dijit.byId('visoes').set('disabled', true);
                populaSaldoConsolidado()
                break;
            default:
        }
    });
}

function resetCamposAjusteFechamentoCaixa() {
	try {
		//dijit.byId("cbOrigemAjusteCad").reset();
		//dijit.byId("cbDestinoAjusteCad").reset();
		//dijit.byId("cbAjusteLiquidacao").reset();
		//dijit.byId("idMovimentoAjusteCad").reset();
        //dijit.byId("idTipoCad").reset();
        dijit.byId("dtaAjusteCc").reset();
        dijit.byId("valContaAjusteCorrente").reset();
		dojo.byId("oldValueAjuste").value = null;
        dojo.byId("dataId").value = null;
        dijit.byId('desObsAjusteCc').reset();
	} catch (e) {
		postGerarLog(e);
	} 
}

function ContaCorrenteObjeto(cd_conta_corrente, origem, destino, movimento, planoConta, cd_local_origem,
    cd_local_destino, cd_tipo_liquidacao, id_tipo_movimento, cd_movimentacao_financeira, cd_plano_conta,
    vl_conta_corrente, vlConta_corrente, dc_obs_conta_corrente, dta_conta_corrente, dc_tipo_liquidacao, oldValue, dataId) {
    try {
       

        this.cd_conta_corrente = cd_conta_corrente;
        this.origem = origem;
        this.destino = destino;
        this.movimento = movimento;
        this.planoConta = planoConta;
        this.cd_local_origem = cd_local_origem;
        this.cd_local_destino = cd_local_destino;
        this.cd_tipo_liquidacao = cd_tipo_liquidacao;
        this.id_tipo_movimento = id_tipo_movimento;
        this.cd_movimentacao_financeira = cd_movimentacao_financeira;
        this.cd_plano_conta = cd_plano_conta;
        this.vl_conta_corrente = vl_conta_corrente;
        this.vlConta_corrente = vlConta_corrente;
        this.dc_obs_conta_corrente = dc_obs_conta_corrente;
        this.dta_conta_corrente = dta_conta_corrente;
        this.dc_tipo_liquidacao = dc_tipo_liquidacao;
        this.oldValue = oldValue;
        this.dataId = dataId;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ObsSaldoCaixa() {
    var gridSaldoCaixa = dijit.byId('gridSaldoCaixa');
    var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
    if (hasValue(dijit.byId('descObsCaixa').value) || 
        (!hasValue(dijit.byId('descObsCaixa').value) && hasValue(gridSaldoCaixa.itemSelecionado) &&
        hasValue(gridSaldoCaixa.itemSelecionado.obsSaldoCaixa) && hasValue(gridSaldoCaixa.itemSelecionado.obsSaldoCaixa.cd_obs_saldo_caixa)) ||
        !hasValue(dijit.byId('descObsCaixa').value) && hasValue(gridSaldoCaixa.obsSaldoCaixaConsolidado)) {
        var obsSaldoCaixa = montarObsSaldoCaixa(gridSaldoCaixa);

        if (hasValue(obsSaldoCaixa)) {
            if (IsStatusFiltro == 'Saldo Individual' && (hasValue(gridSaldoCaixa) && hasValue(gridSaldoCaixa.itemSelecionado))) {

                postObsSaldoCaixa(obsSaldoCaixa, gridSaldoCaixa);
            } else if (IsStatusFiltro == 'Saldo Consolidado') {
                obsSaldoCaixa.cd_usuario = cdUsuarioLogado;
                var gridSaldoTipo = dijit.byId('registryGridSaldoTipo').registry;
                obsSaldoCaixa.dt_saldo_caixa = dojo.date.locale.parse(gridSaldoTipo.dtFechamentoAtual, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                postObsSaldoCaixa(obsSaldoCaixa, gridSaldoCaixa);
            }
        }
    }
}

function postObsSaldoCaixa(obsSaldoCaixa, gridSaldoCaixa) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        xhr.post(Endereco() + "/api/financeiro/postObsSaldoCaixaUsuario", {
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            data: ref.toJson(obsSaldoCaixa)
        }).then(function (data) {
            try {
                var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
                var data = jQuery.parseJSON(data);
                if (IsStatusFiltro == 'Saldo Individual' && hasValue(gridSaldoCaixa.itemSelecionado)) {                    
                    gridSaldoCaixa.itemSelecionado.obsSaldoCaixa = data.retorno;
                } else if (IsStatusFiltro == 'Saldo Consolidado') {
                    gridSaldoCaixa.obsSaldoCaixaConsolidado = data.retorno
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            postGerarLog(error);
        });
    });
}

function getObsSaldoCaixaConsolidado(obsConsolidado) {
    try {
        if (hasValue(obsConsolidado)) { 
            dojo.xhr.get({
                url: Endereco() + "/api/financeiro/getObsSaldoCaixaConsolidado?dt_saldo_caixa=" + obsConsolidado.dt_saldo_caixa + "&cd_usuario=" + obsConsolidado.cd_usuario,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                sync : true
            }).then(function (data) {
                try {
                    var data = jQuery.parseJSON(data);
                    var gridSaldoCaixa = dijit.byId('gridSaldoCaixa');
                    gridSaldoCaixa.obsSaldoCaixaConsolidado = data.retorno;
                    if (hasValue(data.retorno))
                        dijit.byId('descObsCaixa').set('value', data.retorno.tx_obs_saldo_caixa);
                    else
                        dijit.byId('descObsCaixa').set('value', "");

                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                postGerarLog(error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}


function montarObsSaldoCaixa(gridSaldoCaixa) {
    try {
        var obs = null;
        if (hasValue(gridSaldoCaixa.itemSelecionado) && hasValue(gridSaldoCaixa.itemSelecionado.obsSaldoCaixa)) {
            gridSaldoCaixa.itemSelecionado.obsSaldoCaixa.tx_obs_saldo_caixa = dijit.byId('descObsCaixa').value;
            obs = gridSaldoCaixa.itemSelecionado.obsSaldoCaixa;
        } else if (hasValue(gridSaldoCaixa.obsSaldoCaixaConsolidado)) {
            gridSaldoCaixa.obsSaldoCaixaConsolidado.tx_obs_saldo_caixa = dijit.byId('descObsCaixa').value;
            obs = gridSaldoCaixa.obsSaldoCaixaConsolidado;
        }
        else {
            var cdUsuario = hasValue(dojo.byId("cdFkUsuario").value) ? dojo.byId("cdFkUsuario").value : 0;
            var gridSaldoTipo = dijit.byId('registryGridSaldoTipo').registry;
            var dtFechamentoAtual = gridSaldoTipo.dtFechamentoAtual;
            if (cdUsuario > 0)
                dtFechamentoAtual = dojo.date.locale.parse(gridSaldoTipo.dtFechamentoAtual, { formatLength: 'short', selector: 'date', locale: 'pt-br' });

            obs = {
                cd_usuario: cdUsuario,
                cd_caixa_usuario: hasValue(gridSaldoCaixa.itemSelecionado) ? gridSaldoCaixa.itemSelecionado.cd_local_movimento : null,
                dt_saldo_caixa: dtFechamentoAtual,
                tx_obs_saldo_caixa: dijit.byId('descObsCaixa').value
            }
        }
        return obs;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatValorMonetario(value, rowIndex, obj) {
    var retorno = maskFixed(value + "", 2);

    return retorno;
}

function changeValorMonetario(value, evt) {
	console.log(value);
	if (grid.get('currentCell')) {
		var currentRow = grid.get('currentCell').row.data;
		console.log(grid.get('currentValue'));
	}
}

function destroyCreateGridSaldoTipo(dados, pFuncao) {
    require([
			'dojo/_base/declare',
			'dojo/_base/lang',
			'dojo/dom',
			'dojo/on',
			'dstore/Memory',
			'dgrid/OnDemandGrid',
			'dgrid/ColumnSet',
			'my/SummaryRow',
            'dgrid/extensions/DijitRegistry'
    ], function (declare, lang, dom, on, Memory, OnDemandGrid, ColumnSet, SummaryRow, DijitRegistry) {
        try {
            // Deve-se colocar um identificador hardcode nos registros, para o dgrid separar as linhas no clique simples.
            if (hasValue(dados))
                for (var i = 0; i < dados.length; i++)
                    dados[i].id = i + 1;

            var store = new Memory({
                data: dados
            });

            store.getTotals = function () {
                var totals = {};
                totals.saldo_inicial = totals.vl_entrada = totals.vl_saida = totals.saldo_final = 0;

                for (var i = this.data.length; i--;)
                    for (var k in totals)
                        totals[k] += this.data[i][k];

                totals.dc_tipo_liquidacao = 'Total';
                totals.saldo_inicial = maskFixed(totals.saldo_inicial + "", 2);
                totals.vl_entrada = maskFixed(totals.vl_entrada + "", 2);
                totals.vl_saida = maskFixed(totals.vl_saida + "", 2);
                totals.saldo_final = maskFixed(totals.saldo_final + "", 2);

                return totals;
            }

            // Create an instance of OnDemandGrid referencing the store
            var commonArgs = {
                //className: 'dgrid-autoheight',
                collection: store,
                sort: 'dc_tipo_liquidacao'
            };

            var gridSaldoTipo = dijit.byId('registryGridSaldoTipo').registry;

            if (hasValue(gridSaldoTipo)) {
                gridSaldoTipo.destroy();
                $('<div style="height: 100%;width:100%">').attr('id', 'gridSaldoTipo').appendTo('#divSaldoTipo');
            }

            gridSaldoTipo = new (declare([OnDemandGrid, SummaryRow, DijitRegistry]))(lang.mixin({
                noDataMessage: msgNotRegEnc,
                columns: [
                    { label: 'Tipo Liquidação', field: 'dc_tipo_liquidacao', sortable: true },
                    { label: 'Saldo Anterior', field: 'saldo_inicial', className: "dgrid_rigth", formatter: formatValorMonetario },
                    { label: 'Entrada', field: 'vl_entrada', className: "dgrid_rigth", formatter: formatValorMonetario },
                    { label: 'Saída', field: 'vl_saida', className: "dgrid_rigth", formatter: formatValorMonetario },
                    { label: 'Saldo Atual', field: 'saldo_final', className: "dgrid_rigth", formatter: formatValorMonetario }
                ]
            }, commonArgs), 'gridSaldoTipo');
            
            gridSaldoTipo.dtFechamentoAtual = dojo.byId("dataFechamento").value;

            gridSaldoTipo.on('.dgrid-cell:click', function (event) {
                var row = gridSaldoTipo.row(event);
                if (hasValue(row)) {
                    var item = row.data;
                    var IsTagSaldoCaixa = true;
                    if (dijit.byId('categoria').value == 3)
                        dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + item.dc_tipo_liquidacao + ' por Caixa');
                    else
                        if (dijit.byId('categoria').value == 2)
                            dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + item.dc_tipo_liquidacao + ' por Banco');
                        else
                            dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + item.dc_tipo_liquidacao + ' por Caixa/Banco');
                    gridSaldoTipo.itemSelecionado = item;

                    pesquisarFechamentoCaixaLocalMovto(item.cd_tipo_liquidacao, IsTagSaldoCaixa);
                }
            });

            var totals = store.getTotals();
            gridSaldoTipo.set('summary', totals);
            dijit.byId('registryGridSaldoTipo').registry = gridSaldoTipo;

            var gridSaldoCaixa = dijit.byId('registryGridSaldoCaixa').registry;

            if (hasValue(gridSaldoCaixa)) {
                gridSaldoCaixa.destroy();
                $('<div style="height: 100%;width:100%">').attr('id', 'gridSaldoCaixa').appendTo('#divSaldoCaixa');
            }
            if (hasValue(pFuncao))
                pFuncao.call();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarFechamentoCaixa(permissoes) {
    //Criação da Grade de sala
    require([
        "dojo/ready",
        "dojo/_base/xhr",
        'dojo/_base/declare',
		'dojo/_base/lang',
		"dojo/on",
        "dijit/form/Button",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dijit/form/FilteringSelect",
        "dojo/dom"
    ], function (ready, xhr, declare, lang, on, Button, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom) {
        ready(function () {
            try {
                //showCarregando();
                montarEntradaSaida('idTipoCad', false);
                getUsuarioLogado(function () { dataDia(); });
                loadCategoria("categoria");

                // Ações Relacionadas:
                menu = new DropDownMenu({ style: "height: 25px" });

                var menuSaldoIndividual = new MenuItem({
                    label: "Saldo Individual",
                    onClick: function () {
                        populaSaldoIndividual();
                    }
                });
                menu.addChild(menuSaldoIndividual);

                var menuSaldoConsolidado = new MenuItem({
                    label: "Saldo Consolidado",
                    onClick: function () {
                        populaSaldoConsolidado();
                    }
                });
                menu.addChild(menuSaldoConsolidado);

                var button = new DropDownButton({
                    label: "Visões",
                    name: "visoes",
                    dropDown: menu,
                    id: "visoes"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                var menuT = new DropDownMenu({ style: "height: 25px" });

                var acaoRemover = new MenuItem({
                    label: "Zerar Saldo Financeiro",
                    onClick: function () {
                        zerarSaldoFinanceiro(dijit.byId('registryGridSaldoTipo').registry.itemSelecionado, dojo.byId("dataFechamento").value)
                        
                    }
                });
                menuT.addChild(acaoRemover);

                var buttonR = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acaoZerarCaixa",
                    dropDown: menuT,
                    id: "acaoZerarCaixa"
                });
                dom.byId("linkAcoes").appendChild(buttonR.domNode);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try {

                            if (!hasValue(dijit.byId("gridPesquisaUsuarioFK")))
                                montarGridPesquisaUsuarioFK(function () {
                                    dojo.query("#nomPessoaFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarUsuarioFK();
                                    });
                                    abrirUsuarioFK(false);
                                });
                            else
                                abrirUsuarioFK(false);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesUsuarioFK");

                if (Master() == "true")
                    dijit.byId("pesUsuarioFK").set("disabled", false);
                else
                    dijit.byId("pesUsuarioFK").set("disabled", true);
                new Button({
                    label: "Relatório Analítico", iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick:
                        function () {
                            redirecionarRelAnalitico();
                        }
                }, "relatorioA");
                new Button({
                    label: "Relatório Sintético", iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick:
                        function () {
                            try {
                                emitirRelatorioSintetico(xhr);
                            } catch (e) {
                                postGerarLog(e);
                            }
                        }
                }, "relatorioS");

                var buttonFkArray = ['pesUsuarioFK'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                dijit.byId('relatorioS').set('disabled', true);
                dijit.byId('relatorioA').set('disabled', true);

                new Button({
	                label: "Ajustar", iconClass: 'dijitEditorIcon dijitEditorIconSave',
	                onClick: function () {
		                incluirContaCorrente();
	                }
                }, "incluirAjusteContaCorrente");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {

                        if (hasValue(dojo.byId("oldValueAjuste").value) && hasValue(dojo.byId("dataId").value)) {
                            //var dataId = unmaskFixed(dojo.byId("dataId").value, 0);
                            //var oldValue = unmaskFixed(dojo.byId("oldValueAjuste").value, 2);
                            var gridSaldoCaixa = dijit.byId('registryGridSaldoCaixa').registry;
                            //gridSaldoCaixa.collection.data[dataId].saldo_final = oldValue;
                            //gridSaldoCaixa.refresh();
                            gridSaldoCaixa.revert();
                        }
                        dijit.byId("cadAjusteContaCorrente").hide();
	                }
                }, "fecharAjusteContaCorrente");

                dijit.byId('desObsAjusteCc').on("change", function(evt) {
                    if (hasValue(evt) && hasValue(dojo.byId("objAjuste").value)) {
                        var obj = JSON.parse(dojo.byId("objAjuste").value);
                        if (hasValue(obj)) {
                            obj.dc_obs_conta_corrente = hasValue(dijit.byId('desObsAjusteCc').get('value')) == "" ? "" : dijit.byId('desObsAjusteCc').get('value');
                            dojo.byId("objAjuste").value = JSON.stringify(obj);
                        }
                    } else if (!hasValue(evt) && hasValue(dojo.byId("objAjuste").value)) {
	                    var obj = JSON.parse(dojo.byId("objAjuste").value);
	                    if (hasValue(obj)) {
		                    obj.dc_obs_conta_corrente = "";
		                    dojo.byId("objAjuste").value = JSON.stringify(obj);
	                    }
                    }
                });

                dijit.byId("dataFechamento").on("change",
	                function(evt) {
                        if (hasValue(evt)) {
	                        var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
	                        if (IsStatusFiltro == 'Saldo Individual') {
		                        populaSaldoIndividual();
	                        } else if (IsStatusFiltro == 'Saldo Consolidado') {
		                        populaSaldoConsolidado();
	                        }
                        }
                    });
                dijit.byId("ckZerados").onClick = function () { 
                    var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
                    if (IsStatusFiltro == 'Saldo Individual') {
                        populaSaldoIndividual();
                    } else if (IsStatusFiltro == 'Saldo Consolidado') {
                        populaSaldoConsolidado();
                    }
                }

            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function zerarSaldoFinanceiro(itemSelecionado, dataFechamento) {
    try {
        if (itemSelecionado == null || itemSelecionado == undefined) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Para zerar o saldo financeiro é necessário ter um tipo de liquidação selecionado, clique no tipo de liquidação que deseja zerar no grid da tag Resumo das movimentação para em seguida acionar a ação relacionada zerar saldo financeiro.");
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            retorno = false;
        } else if (!hasValue(dataFechamento)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO,
                "Para zerar o saldo financeiro é necessário que o campo data esteja preenchido.");
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            retorno = false;
        } else {

            require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {


                xhr.post(Endereco() + "/api/Financeiro/postZerarSaldoFinanceiro", {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    data: ref.toJson({ cd_escola:0, cd_tipo_liquidacao: itemSelecionado.cd_tipo_liquidacao, dta_base: dataFechamento, tipo: dijit.byId('categoria').value })
                }).then(function (data) {
                        try {
                            if (hasValue(data.retorno.status)) {

                                var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
                                if (IsStatusFiltro == 'Saldo Individual') {
                                    populaSaldoIndividual();
                                } else if (IsStatusFiltro == 'Saldo Consolidado') {
                                    populaSaldoConsolidado();
                                }

                                pesquisarFechamentoCaixaLocalMovto(itemSelecionado.cd_tipo_liquidacao, IsTagSaldoCaixa);

                                var mensagensWeb = new Array();
                                mensagensWeb[0] =
                                    new MensagensWeb(MENSAGEM_INFORMACAO, "Registro alterado com sucesso.");
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);


                            } else {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                                    "Não foi possível alterar o registro.");
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            }
                            hideCarregando();
                        }
                        catch (er) {
                            hideCarregando();
                            postGerarLog(er);
                        }
                    },
                    function (error) {
                        hideCarregando();
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
                            error.response.data.MensagensWeb[0].mensagem);
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        
                    });
            });
        }
    } catch (e) {

    } 
}


function incluirContaCorrente() {
    try {
        showCarregando();
        apresentaMensagem("apresentadorMensagemAjusteContaCorrente", '');
        apresentaMensagem('apresentadorMensagem', null);

        var conta = hasValue(dojo.byId("objAjuste").value) ? JSON.parse(dojo.byId("objAjuste").value) : null;

        if (!hasValue(conta)) {
	        hideCarregando();
	        return false;
        }

        require(["dojo/request/xhr", "dojox/json/ref", "dojo/window"], function (xhr, ref, windows) {
            

            xhr.post(Endereco() + "/api/escola/PostIncluirContaCorrente", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson(conta)
            }).then(function (data) {
                try {
                    if (!hasValue(data.erro)) {
                        data = $.parseJSON(data);

                        var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
                        if (IsStatusFiltro == 'Saldo Individual') {
	                        populaSaldoIndividual();
                        } else if (IsStatusFiltro == 'Saldo Consolidado') {
	                        populaSaldoConsolidado();
                        }
                        pesquisarFechamentoCaixaLocalMovto(conta.cd_tipo_liquidacao);

                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Registro salvo com sucesso.");
                        apresentaMensagem("apresentadorMensagem", mensagensWeb);

                        dijit.byId("cadAjusteContaCorrente").hide();

                    } else
                        apresentaMensagem('apresentadorMensagemAjusteContaCorrente', data);
                    hideCarregando();
                }
                catch (er) {
                    hideCarregando();
                    postGerarLog(er);
                }
            },
                function (error) {
                    apresentaMensagem('apresentadorMensagemAjusteContaCorrente', error.response.data);
                    hideCarregando();
                });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function fecharDialogo(evt) {
	var gridSaldoCaixa = dijit.byId('registryGridSaldoCaixa').registry;
	//gridSaldoCaixa.collection.data[dataId].saldo_final = oldValue;
	//gridSaldoCaixa.refresh();
	gridSaldoCaixa.revert();
}

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

function montarCombo(Memory,nomElement, data, value) {
	try {
		var itemsCb = [];
        var cbField = dijit.byId(nomElement);
        
		var stateStore = new Memory({
            data: data
		});
        cbField.store = stateStore;

        cbField.set("value", value);

	}
	catch (e) {
		postGerarLog(e);
	}
};

function abrirUsuarioFK(isPesquisa) {
    try {
        limparPesquisaUsuarioFK();
        dojo.byId("idOrigemUsuarioFK").value = FECHCAIXA;
        pesquisarUsuarioFKFechamentoCaixa();
        dijit.byId("proUsuario").show();
        apresentaMensagem('apresentadorMensagemProUsuario', null);
    } catch (e) {
        postGerarLog(e);
    }
}
function retornarUsuarioFKFechamento() {
    try {
        var valido = true;
        var gridUsuarioSelec = dijit.byId("gridPesquisaUsuarioFK");
        if (!hasValue(gridUsuarioSelec.itensSelecionados))
            gridUsuarioSelec.itensSelecionados = [];
        if (!hasValue(gridUsuarioSelec.itensSelecionados) || gridUsuarioSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else {
            dijit.byId("no_usuario").set("value", gridUsuarioSelec.itensSelecionados[0].no_login);
            dojo.byId("cdFkUsuario").value = gridUsuarioSelec.itensSelecionados[0].cd_usuario;
        }

        if (!valido)
            return false;
        var IsStatusFiltro = dojo.byId("visoes_label").innerHTML;
        if (IsStatusFiltro == 'Saldo Individual') {
            populaSaldoIndividual();
        } else if (IsStatusFiltro == 'Saldo Consolidado') {
            populaSaldoConsolidado();
        }
        dijit.byId("proUsuario").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function getUsuarioLogado(pFuncao) {
    try {
        dojo.xhr.get({
            url: Endereco() + "/auth/GetNomeCodigoUsuario",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                var retorno = jQuery.parseJSON(data).retorno;
                nomeUsuarioLogado = retorno.no_login;
                cdUsuarioLogado = retorno.cd_usuario;
                dijit.byId("no_usuario").set("value", nomeUsuarioLogado);
                dojo.byId("cdFkUsuario").value = cdUsuarioLogado;
                showCarregando();
                if (hasValue(pFuncao))
                    pFuncao.call();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
            hideCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}
function pesquisarUsuarioFKFechamentoCaixa() {
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
                target: Endereco() + "/Escola/getUsuarioSearchGeralFK?descricao=" + encodeURIComponent(document.getElementById("pesquisatextFK").value) + "&nome=" + encodeURIComponent(document.getElementById("nomPessoaFK").value) + "&inicio=" + document.getElementById("inicioUsuarioFK").checked,
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

function dataDia() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getData",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            dijit.byId('dataFechamento').original_value = data;
            dijit.byId('dataFechamento').set('value', dojo.date.locale.parse(data, { formatLength: 'short', selector: 'date', locale: 'pt-br' }));
            populaSaldoIndividual();
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemMat", error);
        hideCarregando();
    });
}

function redirecionarRelAnalitico() {
    try {
        var tipoLiq = 0;
        var localMovto = 0;
        var gridSaldoTipo = dijit.byId('registryGridSaldoTipo').registry;
        var gridSaldoCaixa = dijit.byId('registryGridSaldoCaixa').registry;

        //showCarregando();
        if (hasValue(gridSaldoTipo) && hasValue(gridSaldoTipo.itemSelecionado) && gridSaldoTipo.itemSelecionado.cd_tipo_liquidacao > 0)
            tipoLiq = gridSaldoTipo.itemSelecionado.cd_tipo_liquidacao;

        if (hasValue(gridSaldoCaixa) && hasValue(gridSaldoCaixa.itemSelecionado) && gridSaldoCaixa.itemSelecionado.cd_local_movimento > 0)
            localMovto = gridSaldoCaixa.itemSelecionado.cd_local_movimento;

        window.location = Endereco() + '/Relatorio/ReportContaCorrente?idEmitir=true&localMovto=' + localMovto + '&tipoLiquidacao=' + tipoLiq + '&data=' + dojo.byId("dataFechamento").value;
    } catch (e) {
        //hideCarregando();
        postGerarLog(e);
    }
}
function populaSaldoIndividual() {
    try {
        if (!hasValue(dojo.byId("cdFkUsuario").value) || parseInt(dojo.byId("cdFkUsuario").value) == 0) {
            dijit.byId("no_usuario").set("value", nomeUsuarioLogado);
            dojo.byId("cdFkUsuario").value = cdUsuarioLogado;
        }
        else {
            if (!hasValue(dojo.byId("dataFechamento").value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroDataFechamentoCaixa);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }
            else
                apresentaMensagem('apresentadorMensagem', null);
        }
        pesquisarFechamentoCaixaTpLiquidacao(function () {
            var gridSaldoTipo = dijit.byId('registryGridSaldoTipo').registry;

            if (hasValue(gridSaldoTipo) && hasValue(gridSaldoTipo._total) && gridSaldoTipo._total > 0) {
                quickSortObj(gridSaldoTipo.collection.data, "dc_tipo_liquidacao");
                var tipoLiquidacao = hasValue(gridSaldoTipo.collection.data[0]) ? gridSaldoTipo.collection.data[0].cd_tipo_liquidacao : 0;
                pesquisarFechamentoCaixaLocalMovto(tipoLiquidacao);
                dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + gridSaldoTipo.collection.data[0].dc_tipo_liquidacao + ' por Caixa');
            }
            dijit.byId('relatorioS').set('disabled', false);
            dijit.byId('relatorioA').set('disabled', false);
            dojo.byId("visoes_label").innerHTML = "Saldo Individual";
        });

        dijit.byId('tgObsCaixa').set('open', true);        
        dijit.byId('tgObsCaixa').set('title', 'Observação de detalhamento');
        dijit.byId('descObsCaixa').set('value', "");

        dijit.byId('acaoZerarCaixa').set('disabled', true);
    }
    catch (e) {
        postGerarLog(e);
    }
}
function populaSaldoConsolidado() {
    try {
        if(FechConsolidado() == "true" || Master() == "true"){
            if (!hasValue(dojo.byId("dataFechamento").value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroDataFechamentoCaixa);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
            }
            else
                apresentaMensagem('apresentadorMensagem', null);

            dijit.byId('no_usuario').set('value', '');
            dojo.byId("cdFkUsuario").value = 0;
            pesquisarFechamentoCaixaTpLiquidacao(function () {
                var gridSaldoTipo = dijit.byId('registryGridSaldoTipo').registry;
                if (hasValue(gridSaldoTipo) && hasValue(gridSaldoTipo._total) && gridSaldoTipo._total > 0) {
                    quickSortObj(gridSaldoTipo.collection.data, "dc_tipo_liquidacao");
                    var tipoLiquidacao = hasValue(gridSaldoTipo.collection.data[0]) ? gridSaldoTipo.collection.data[0].cd_tipo_liquidacao : 0;
                    pesquisarFechamentoCaixaLocalMovto(tipoLiquidacao);
                    if (dijit.byId('categoria').value == 3)
                        dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + gridSaldoTipo.collection.data[0].dc_tipo_liquidacao + ' por Caixa');
                    else
                        if (dijit.byId('categoria').value == 2)
                            dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + gridSaldoTipo.collection.data[0].dc_tipo_liquidacao + ' por Banco');
                        else
                            dijit.byId('tagSaldoCaixa').set('title', 'Detalhe do ' + gridSaldoTipo.collection.data[0].dc_tipo_liquidacao + ' por Caixa/Banco');
                }
            });
            dijit.byId('relatorioS').set('disabled', false);
            dijit.byId('relatorioA').set('disabled', false);
            dojo.byId("visoes_label").innerHTML = "Saldo Consolidado";
            dijit.byId('tgObsCaixa').set('open', true);
            dijit.byId('tgObsCaixa').set('title', 'Observação de detalhamento Caixa Consolidado');
        } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroVisaoAdmFechamentoCaixa);
                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                return false;
        }
        dijit.byId('acaoZerarCaixa').set('disabled', false);
    }
    catch (e) {
        postGerarLog(e);
    }
}


function emitirRelatorioSintetico(xhr) {
    var usuario = hasValue(dojo.byId("cdFkUsuario").value) ? dojo.byId("cdFkUsuario").value : 0;
    xhr.get({
        url: Endereco() + "/api/financeiro/getUrlFechamentoCaixaSint?dta_fechamento=" + dojo.byId("dataFechamento").value + "&cdUsuario=" + usuario + "&tipoLocal=" + cbCategoria.value +
            "&mostrarZerados=" + dijit.byId('ckZerados').checked,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            abrePopUp(Endereco() + '/Relatorio/RelatorioFechamentoCaixaSint?' + data, '1024px', '750px', 'popRelatorio');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}