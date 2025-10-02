function limparGridBaixas() {
    try {
        var gridBaixa = dijit.byId("gridBaixa");
        gridBaixa.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        gridBaixa.itensSelecionados = [];
        gridBaixa.itemSelecionado = null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarBaixaTitulo(itensSelecionados, xhr, ready, Memory, FilteringSelect, on) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else if (itensSelecionados[0].cd_tran_finan == null)
            caixaDialogo(DIALOGO_ERRO, msgErroCdTranFinaNaoEncontrado, null);
        else {
            apresentaMensagem("apresentadorMensagemCadBaixa", "");
            limparCamposBaixaCad()
            if (!hasValue(dijit.byId('incluirBaixa'))) {
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                montaCadastroBaixaFinanceira(function () { setarEventosBotoesPrincipaisCadTransacao(xhr, on); }, Permissoes);
            }
            IncluirAlterar(0, 'divAlterarBaixa', 'divIncluirBaixa', 'divExcluirBaixa', 'apresentadorMensagem', 'divCancelarBaixa', 'divLimparBaixa');
            showEditBaixaTitulo(itensSelecionados[0].cd_tran_finan, itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
            dojo.byId('telaMensagem').value = '';
            dijit.byId("cadBaixaFinanceira").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditBaixaTitulo(cd_trasacao_finan, cd_baixa_titulo, xhr, ready, Memory, FilteringSelect) {
    try {
        showCarregando();
        xhr.get({
            url: Endereco() + "/api/financeiro/getBaixaTituloEComponentesTrasacaoFinan?cd_transacao_finan=" + cd_trasacao_finan,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                var compLocalMovto = dijit.byId("cbLocalCad");
                var compLiquidacao = dijit.byId("cbLiquidacao");
                var compDtBaixa = dijit.byId("dt_baixa");

                dijit.byId("vlTotal").set("value", "");
                dijit.byId("vlTotal").set("value", data.vl_total_baixa);

                dijit.byId("vlTotalTroco").set("value", "");
                dijit.byId("vlTotalTroco").set("value", data.vl_total_troco);

                dijit.byId("vlTroco").set("value", "");
                dijit.byId("vlTroco").set("value", data.vl_troco);



                if (hasValue(data) && hasValue(data.LocaisMovimento)) {
                    compLocalMovto._onChangeActive = false;
                    criarOuCarregarCompFiltering("cbLocalCad", data.LocaisMovimento, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    compLocalMovto._onChangeActive = true;
                }
                if (hasValue(data) && hasValue(data.TiposLiquidacao)) {
                    compLiquidacao._onChangeActive = false;
                    criarOuCarregarCompFiltering("cbLiquidacao", data.TiposLiquidacao, "", data.cd_tipo_liquidacao, ready, Memory, FilteringSelect, 'cd_tipo_liquidacao', 'dc_tipo_liquidacao');
                    compLiquidacao._onChangeActive = true;
                    if (data.cd_tipo_liquidacao == BAIXACANCELAMENTO || data.cd_tipo_liquidacao == BAIXAMOTIVOBOLSA) {
                        compLiquidacao.set("disabled", true);
                        compLocalMovto.set("disabled", true);
                    }
                }
                //Atualiza a data da baixa:
                if (data.dt_tran_finan != null) {
                    compDtBaixa._onChangeActive = false;
                    dijit.byId('dt_baixa').set("value", data.dt_tran_finan);
                    compDtBaixa._onChangeActive = true;
                    compDtBaixa.set("disabled", true);
                }
                dojo.byId("cd_tran_finan").value = data.cd_tran_finan;
                //Atualiza a grid:
                criaAtualizaGridBaixaCad(data.Baixas, xhr, dojox.json.ref);

                if (data.cd_tipo_liquidacao == CHEQUEPREDATADO || data.cd_tipo_liquidacao == CHEQUEVISTA) {
                    criarOuCarregarCompFiltering("bancoChequeBaixa", data.bancos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco');
                    validarTipoLiqChequeTransacao(data.cd_tipo_liquidacao);
                    if (hasValue(data.cheque))
                        loadDataChequeTranFinan(data.cheque);
                    //else
                    //    validarTipoLiqChequeTransacao(data.cd_tipo_liquidacao);
                }
                if (data.cd_tipo_liquidacao === CARTAOCREDITO || data.cd_tipo_liquidacao === CARTAODEBITO || data.cd_tipo_liquidacao === TROCA_FINANCEIRA) {
                    compLiquidacao.set("disabled", true);
                }
                hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemCadBaixa", error);
            hideCarregando();
        });
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function loadDataChequeTranFinan(cheque) {

    if (hasValue(cheque)) {
        document.getElementById("tgCheque").style.display = "";
        dojo.byId("cd_cheque_trans").value = cheque.cd_cheque;
        dijit.byId("emissorChequeBaixa").set('value', cheque.no_emitente_cheque);
        dijit.byId("nomeAgenciaChequeBaixa").set('value', cheque.no_agencia_cheque);
        dijit.byId("nroAgenciaChequeBaixa").set('value', cheque.nm_agencia_cheque);
        dijit.byId("dgAgenciaChequeBaixa").set('value', cheque.nm_digito_agencia_cheque);
        dijit.byId("nroContaCorrenteChequeBaixa").set('value', cheque.nm_conta_corrente_cheque);
        dijit.byId("dgContaCorrenteChequeBaixa").set('value', cheque.nm_digito_cc_cheque);
        dijit.byId("bancoChequeBaixa").set('value', cheque.cd_banco);
        if (hasValue(cheque.dt_bom_para)) {
            dijit.byId("nroPrimeiroChequeChequeBaixa").set("value", cheque.nm_primeiro_cheque);
            dijit.byId("dtChequeChequeBaixa").set('value', cheque.dt_bom_para);
        }
    }
}