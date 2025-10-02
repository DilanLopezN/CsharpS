var TODAS = 0, RECEBER = 1, PAGAR = 2;
var NATUREZA_LOCAL_MOVIMENTO_TODAS = 0, BAIXACANCELAMENTO = 6, BAIXAMOTIVOBOLSA = 100, DESCONTOFOLHAPAGAMENTO = 101, BAIXAMOTIVOBOLSAADITIVO = 102;
var TITULO_ABERTO = 1, TITULO_FECHADO = 2;
var CONTRATO = 1;
var INICIAL = 0, ENVIADO_GERADO = 1, BAIXA_MANUAL = 2, CONFIRMADO_ENVIO = 3, BAIXA_MANUAL_CONFIRMADO = 4, PEDIDO_BAIXA = 5, CONFIRMADO_PEDIDO_BAIXA = 6;
var ORIGEMBAIXAFINANCEIRA = 1, ORIGEMMATRICULA = 2;
var CHEQUEPREDATADO = 4, CHEQUEVISTA = 10, EVENTO_GRID_CHEQUE = 0, ISVIEW_CHEQUE_TRANSACAO = null;
var CARTAOCREDITO = 2, CARTAODEBITO = 3;
var CHEQUE = 4, CARTAO = 5; TROCA_FINANCEIRA = 110;
var LOCALCARTAOCREDITO = 4; LOCALCARTAODEBITO = 5;
function montaCadastroBaixaFinanceira(funcao, permissoes) {
    require([
        "dojo/ready",
        "dojo/_base/xhr",
        "dojo/on",
        "dijit/form/Button",
        "dojox/json/ref",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem",
        "dojo/dom",
        "dojo/date",
        "dojo/store/Memory",
        "dijit/form/FilteringSelect"
    ], function (ready, xhr, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, dom, date, Memory, FilteringSelect) {
        ready(function () {
            try {
                limparTroco();

                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;

                if (hasValue(dijit.byId('linkAcoesBxCad')) || hasValue(dijit.byId('incluirBaixa')) || !hasValue(dom.byId("linkAcoesBxCad")))
                    throw new Exception("Chamada inválida para função 'montaCadastroBaixaFinanceira' com origem da tela: " + window.location.pathname);
                //Ações Relacionadas Baixa Cadastro
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        excluirBaixaCad();
                        //eventoRemover(dijit.byId('gridBaixaCad').itensSelecionados, 'excluirBaixaCad();');
                    }
                });
                menu.addChild(acaoExcluir);

                var acaoEditar = new MenuItem({
                    label: "Observação",
                    onClick: function () { montarDialogoObservacao(); }
                });
                menu.addChild(acaoEditar);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadasBxCad"
                });
                dom.byId("linkAcoesBxCad").appendChild(button.domNode);

                //Botões Baixa Cadastro
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { }
                }, "incluirBaixa");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { }
                }, "cancelarBaixa");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { }
                }, "fecharBaixa");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { }
                }, "alterarBaixa");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { }
                }, "deleteBaixa");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        apresentaMensagem('apresentadorMensagemCadBaixa', '');
                    }
                }, "limparBaixa");

                dijit.byId('btOKObservacao').set('iconClass', 'dijitEditorIcon dijitEditorIconRedo');
                dijit.byId('btCancelarNota').set('iconClass', 'dijitEditorIcon dijitEditorIconCancel');

                dijit.byId("btOKObservacao").on("click", function (e) {
                    try {
                        var gridBaixaCad = dijit.byId('gridBaixaCad');
                        var itensSelecionados = gridBaixaCad.itensSelecionados;

                        itensSelecionados[0].tx_obs_baixa = dojo.byId('textAreaObs').value;
                        dijit.byId('dialogObservacao').hide();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("dt_baixa").on("change", function (e) {
                    try {

                        if (!ValidateRangeDate(e, date, "apresentadorMensagemCadBaixa", msgErroDtaMin, msgErroDtaMax))
                            return false;

                        if (dijit.byId("dt_baixa").validate()) {
                            var gridTitulo = dijit.byId('gridTitulo');
                            var itensSelecionados = gridTitulo.itensSelecionados;
                            showCarregando();
                            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                            simularBaixaTitulos(itensSelecionados, xhr, ref, Permissoes);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("cbLiquidacao").on("change", function (e) {
                    try {
                        if (hasValue(dijit.byId("gridBaixa").itensSelecionados)) {
                            if (e == CARTAOCREDITO || e == CARTAODEBITO) {
                                getLocalMovtoGeralOuTipoLiquidacaoCartao(e,dijit.byId('gridBaixa').itensSelecionados[0].cd_local_movto);
                                dijit.byId("cbLocalCad").set("value", "");
                                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                            } else if (e == CHEQUEVISTA || e == CHEQUEPREDATADO) {
                                //getLocalMovtoGeralOuCartao(CHEQUE);
                                getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect)
                                dijit.byId("cbLocalCad").set("value", "");
                                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                            } else {
                                getLocalBaixaGeral(dijit.byId("gridBaixa").itensSelecionados[0].cd_tran_finan, dijit.byId("gridBaixa").itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
                                dijit.byId("cbLocalCad").set("value", "");
                            }

                        } else if (hasValue(dijit.byId("gridTitulo").itensSelecionados) && dijit.byId("gridTitulo").itensSelecionados.length > 0) {

                            if (e == CARTAOCREDITO || e == CARTAODEBITO) {
                                getLocalMovtoGeralOuTipoLiquidacaoCartao(e, dijit.byId('gridTitulo').itensSelecionados[0].cd_local_movto); 
                                dijit.byId("cbLocalCad").set("value", "");
                                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                            } else if (e == CHEQUEVISTA || e == CHEQUEPREDATADO) {
                                //getLocalMovtoGeralOuCartao(CHEQUE);
                                getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect)
                                dijit.byId("cbLocalCad").set("value", "");
                                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                            } else {
                                getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect);
                            }
                        }

                        setRequiredCamposChequeBaia(false);
                        var compCbLocalCad = dijit.byId("cbLocalCad");

                        if (hasValue(dijit.byId('gridBaixaCad')))
                            dijit.byId('gridBaixaCad').update();
                        if (e == BAIXACANCELAMENTO || e == BAIXAMOTIVOBOLSA || e == BAIXAMOTIVOBOLSAADITIVO || DESCONTOFOLHAPAGAMENTO == e) {
                            compCbLocalCad.reset();
                            compCbLocalCad.set("disabled", true);
                            compCbLocalCad.set("required", false);
                        } else {
                            validaCartaoChequeETroca(e);
                            validarTipoLiqChequeTransacao(e);
                            compCbLocalCad.set("disabled", false);
                            compCbLocalCad.set("required", true);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("tagBaixa").on("show", function (e) {
                    try {
                        var gridBaixaCad = dijit.byId('gridBaixaCad');
                        if (hasValue(gridBaixaCad))
                            gridBaixaCad.update();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("vlTotalTroco").on("change", function (e) {
                    try {
                        calcularValorTroco();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                if (hasValue(funcao))
                    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}



function getLocalBaixaGeral(cd_trasacao_finan, cd_baixa_titulo, xhr, ready, Memory, FilteringSelect) {
    try {
        //showCarregando();
        xhr.get({
            url: Endereco() + "/api/financeiro/getBaixaTituloEComponentesTrasacaoFinan?cd_transacao_finan=" + cd_trasacao_finan,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                var compLocalMovto = dijit.byId("cbLocalCad");
                

                if (hasValue(data) && hasValue(data.LocaisMovimento)) {
                    compLocalMovto._onChangeActive = false;
                    loadSelect(data.LocaisMovimento, 'cbLocalCad', 'cd_local_movto', 'nomeLocal');
                    //loadSelect(data.LocaisMovimento, 'cbLocalCad', 'cd_local_movto', 'no_local_movto');
                    //criarOuCarregarCompFiltering("cbLocalCad", data.LocaisMovimento, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                    compLocalMovto._onChangeActive = true;
                }
               
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemCadBaixa", error);
            //showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect) {
    try {
        //showCarregando();
        xhr.get({
            url: Endereco() + "/api/escola/getLocaisMovimentoGeral",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
                try {
                    var result = data;
                    var compLocalMovto = dijit.byId("cbLocalCad");


                    if (hasValue(result) && hasValue(result.LocalMovto)) {
                        compLocalMovto._onChangeActive = false;
                        loadSelect(result.LocalMovto, 'cbLocalCad', 'cd_local_movto', 'nomeLocal');
                        //loadSelect(data.LocaisMovimento, 'cbLocalCad', 'cd_local_movto', 'no_local_movto');
                        //criarOuCarregarCompFiltering("cbLocalCad", data.LocaisMovimento, "", data.cd_local_movto, ready, Memory, FilteringSelect, 'cd_local_movto', 'nomeLocal');
                        compLocalMovto._onChangeActive = true;

                        //if (hasValue(data.LocalMovto) && data.LocalMovto.length > 0)
                        //    loadSelect(data.LocalMovto, 'cbLocalCad', 'cd_local_movto', 'nomeLocal');
                    }

                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemCadBaixa", error);
                //showCarregando();
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getLocalMovtoGeralOuCartao(cd_tipo_financeiro) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getLocalMovtoGeralOuCartao?cd_tipo_financeiro=" + cd_tipo_financeiro,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            response = jQuery.parseJSON(data);
            loadSelect(response.retorno, 'cbLocalCad', 'cd_local_movto', 'no_local_movto');
            //loadSelect(response.retorno, "edBanco", 'cd_local_movto', 'no_local_movto');
        } catch (e) {
            postGerarLog(e);
        }

    }, function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function getLocalMovtoGeralOuTipoLiquidacaoCartao(cd_tipo_liquidacao, cd_local_movto) {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getLocalMovtoGeralOuTipoLiquidacaoCartao?cd_tipo_liquidacao=" + cd_tipo_liquidacao + "&cd_local_movto=" + cd_local_movto,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            response = jQuery.parseJSON(data);
            loadSelect(response.retorno, 'cbLocalCad', 'cd_local_movto', 'no_local_movto');
            //loadSelect(response.retorno, "edBanco", 'cd_local_movto', 'no_local_movto');
        } catch (e) {
            postGerarLog(e);
        }

    }, function (error) {
        apresentaMensagem('apresentadorMensagem', error);
    });
}

function ValidateRangeDate(e, date, paramMensagemErro, msgDtaMin, msgDtaMax) {
    var dataMin = new Date(1899, 12, 01);
    if (date.compare(dataMin, e) > 0) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDtaMin);
        apresentaMensagem(paramMensagemErro, mensagensWeb);
        return false;
    }
    else {
        var dataMax = new Date(2079, 05, 06);
        if (date.compare(e, dataMax) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDtaMax);
            apresentaMensagem(paramMensagemErro, mensagensWeb);
            return false;
        }
    }
    return true;
}

function limparCamposBaixaCad() {
    try {
        var compLocal = dijit.byId('cbLocalCad');
        var compTipoLiq = dijit.byId('cbLiquidacao');
        dijit.byId("tagBaixa").set("open", true);
        dojo.byId("dt_baixa").value = '';
        dijit.byId("vlTotal").set("value", '');
        dijit.byId("emissorChequeBaixa").reset();
        dijit.byId("nomeAgenciaChequeBaixa").reset();
        dijit.byId("nroAgenciaChequeBaixa").reset();
        dijit.byId("dgAgenciaChequeBaixa").reset();
        dijit.byId("nroContaCorrenteChequeBaixa").reset();
        dijit.byId("dgContaCorrenteChequeBaixa").reset();
        dijit.byId("nroPrimeiroChequeChequeBaixa").reset();
        dijit.byId("bancoChequeBaixa").set("value", '');
        dijit.byId("dtChequeChequeBaixa").reset();
        compLocal._onChangeActive = false;
        compLocal.reset();
        compLocal._onChangeActive = true;
        compLocal.set('disabled', false);
        compTipoLiq._onChangeActive = false;
        compTipoLiq.set('disabled', false);
        compTipoLiq.reset();
        compTipoLiq._onChangeActive = true;
        document.getElementById("tgCheque").style.display = "none";
        //getLimpar('#formCadBaixa');
        //clearForm('formCadBaixa');
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluirBaixaCad() {
    try {
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var itensSelecionados = hasValue(dijit.byId('gridBaixaCad').itensSelecionados) ? dijit.byId('gridBaixaCad').itensSelecionados : new Array();
        var dadosStore = gridBaixaCad.store.objectStore.data;
        var novoStore = new Array();

        for (var i = dadosStore.length - 1; i >= 0; i--)
            if (!hasValue(binaryObjSearch(itensSelecionados, "nm_baixa", dadosStore[i].nm_baixa), true))
                novoStore.push(dadosStore[i]);

        gridBaixaCad.itensSelecionados = new Array();
        gridBaixaCad.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: novoStore }) }));
        gridBaixaCad.store.save();

        atualizarVlTotal(novoStore);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDialogoObservacao() {
    try {
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var itensSelecionados = gridBaixaCad.itensSelecionados;

        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            dijit.byId('textAreaObs').set('value', itensSelecionados[0].tx_obs_baixa);
            dijit.byId('dialogObservacao').show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostrarCadastroBaixaFinanceira(novo, gridTitulo, gridBaixa, xhr, ref, on) {
    showCarregando();

    var apresentadorMensagem = "apresentadorMensagem";    

    if (hasValue(dojo.byId('telaMensagem'))) {
        if (parseInt(dojo.byId('telaMensagem').value) == CONTRATO)
            apresentadorMensagem = "apresentadorMensagemMat";
    }

    apresentaMensagem(apresentadorMensagem, "");
    limparCamposBaixaCad()
    if (!hasValue(dijit.byId('incluirBaixa'))) {
        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
        montaCadastroBaixaFinanceira(function () { setarEventosBotoesPrincipaisCadTransacao(xhr, on); }, Permissoes);
    }

    if (novo) {
        var itensSelecionados = gridTitulo.itensSelecionados;

        //Verifica se existe pelo menos um título marcado para inclusão da baixa:
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
            showCarregando();
        }
        else {
            var mensagensWeb = new Array();
            if (parseInt(dojo.byId('telaMensagem').value) == CONTRATO)
                apresentadorMensagem = "apresentadorMensagemMat";

            apresentaMensagem(apresentadorMensagem, '');

            var natureza = itensSelecionados[0].id_natureza_titulo;
            for (var i = 0; i < itensSelecionados.length; i++) {
                //Verifica se são títulos da mesma natureza:
                if (itensSelecionados[i].id_natureza_titulo != natureza) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloMesmaNatureza);
                    apresentaMensagem(apresentadorMensagem, mensagensWeb);
                    showCarregando();
                    return false;
                }

                //Verifica se na lista de titulos existe algum que já está fechado:
                if (itensSelecionados[i].id_status_titulo == TITULO_FECHADO) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloFechado);
                    apresentaMensagem(apresentadorMensagem, mensagensWeb);
                    showCarregando();
                    return false;
                }
                //if ((itensSelecionados[i].id_status_cnab == ENVIADO_GERADO && itensSelecionados[i].id_carteira_registrada_localMvto) || (itensSelecionados[i].id_status_cnab == PEDIDO_BAIXA)) {
                //    if (itensSelecionados[i].id_status_cnab == PEDIDO_BAIXA)
                //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloStatusCnabPedidoBaixa);
                //    if (itensSelecionados[i].id_status_cnab == ENVIADO_GERADO)
                //        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloStatusCnabEnviadoGerado);
                //    apresentaMensagem(apresentadorMensagem, mensagensWeb);
                //    showCarregando();
                //    return false;
                //}
            }
            IncluirAlterar(1, 'divAlterarBaixa', 'divIncluirBaixa', 'divExcluirBaixa', apresentadorMensagem, 'divCancelarBaixa', 'divLimparBaixa');
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            //if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
            if (itensSelecionados[0].nm_tipo_local == 0 && hasValue(itensSelecionados[0].LocalMovto))
                itensSelecionados[0].nm_tipo_local = itensSelecionados[0].LocalMovto.nm_tipo_local;
            simularBaixaTitulos(itensSelecionados, xhr, ref, Permissoes);
            dijit.byId('dt_baixa').set("disabled", false);
            dijit.byId("cadBaixaFinanceira").show();
            dojo.byId("cadBaixaFinanceira").childNodes[3].style.width = "916px";
        }
    }
    else {
        var itensSelecionados = gridBaixa.itensSelecionados;

        //Verifica se existe pelo menos um título marcado para inclusão da baixa:
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_ERRO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            apresentaMensagem('apresentadorMensagem', '');
            IncluirAlterar(0, 'divAlterarBaixa', 'divIncluirBaixa', 'divExcluirBaixa', 'apresentadorMensagem', 'divCancelarBaixa', 'divLimparBaixa');
            dojo.byId("cadBaixaFinanceira").childNodes[3].style.width = "916px";
            dijit.byId("cadBaixaFinanceira").show();
        }
    }
}

function simularBaixaTitulos(titulos, xhr, ref, Permissoes) {
    try {
        apresentaMensagem('apresentadorMensagemCadBaixa', null);
        var dataBaixa = hasValue(dojo.byId('dt_baixa').value) ? dojo.byId('dt_baixa').value : null;
        var sugestaoCartao = {
            cd_local_movto: 0,
            nm_tipo_local: 0,
            cd_tipo_financeiro: 0
        };

        if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes)) {
            xhr.post({
                url: Endereco() + "/api/escola/postSimularBaixaTitulosGeral",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({ titulos: titulos, dataBaixa: dataBaixa })
            }).then(function (data) {
                try {
                   
                    data = jQuery.parseJSON(data);
                    if (hasValue(data)) {
                        if (hasValue(data.TipoLiquidacoes) && data.TipoLiquidacoes.length > 0)
                            loadSelect(data.TipoLiquidacoes, 'cbLiquidacao', 'cd_tipo_liquidacao', 'dc_tipo_liquidacao');
                        if (hasValue(data.LocalMovto) && data.LocalMovto.length > 0)
                            loadSelect(data.LocalMovto, 'cbLocalCad', 'cd_local_movto', 'nomeLocal');
                        if (hasValue(data.Bancos) && data.Bancos.length > 0)
                            criarOuCarregarCompFiltering("bancoChequeBaixa", data.Bancos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco');

                        //Atualiza a data da baixa:
                        if (hasValue(data.Baixas) && hasValue(data.Baixas[0]) && hasValue(data.Baixas[0].dt_baixa_titulo)) {
                            //dojo.byId('dt_baixa').value = data[0].dta_baixa;
                            dijit.byId('dt_baixa')._onChangeActive = false;
                            dijit.byId('dt_baixa').set("value", data.Baixas[0].dt_baixa_titulo);
                            dijit.byId('dt_baixa')._onChangeActive = true;
                        }

                        sugestaoCartao.cd_local_movto = data.cd_local_movto;
                        sugestaoCartao.nm_tipo_local = data.nm_tipo_local;
                        sugestaoCartao.cd_tipo_financeiro = data.cd_tipo_financeiro;
                        sugestaoCartao.cd_local_banco = data.cd_local_banco;
                    }

                    //Atualiza a grid:
                    criaAtualizaGridBaixaCad(data.Baixas, xhr, ref, sugestaoCartao);
                    limparTroco();

                }
                catch (e) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemCadBaixa', error);
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCadBaixa', error);
                showCarregando();
            });
        }
        else {
            xhr.post({
                url: Endereco() + "/api/escola/postSimularBaixaTitulos",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({ titulos: titulos, dataBaixa: dataBaixa })
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (hasValue(data)) {
                        if (hasValue(data.TipoLiquidacoes) && data.TipoLiquidacoes.length > 0)
                            loadSelect(data.TipoLiquidacoes, 'cbLiquidacao', 'cd_tipo_liquidacao', 'dc_tipo_liquidacao');
                        if (hasValue(data.LocalMovto) && data.LocalMovto.length > 0)
                            loadSelect(data.LocalMovto, 'cbLocalCad', 'cd_local_movto', 'nomeLocal');
                        if (hasValue(data.Bancos) && data.Bancos.length > 0)
                            criarOuCarregarCompFiltering("bancoChequeBaixa", data.Bancos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco');
                        //Atualiza a data da baixa:
                        if (hasValue(data.Baixas) && hasValue(data.Baixas[0]) && hasValue(data.Baixas[0].dt_baixa_titulo)) {
                            //dojo.byId('dt_baixa').value = data[0].dta_baixa;
                            dijit.byId('dt_baixa')._onChangeActive = false;
                            dijit.byId('dt_baixa').set("value", data.Baixas[0].dt_baixa_titulo);
                            dijit.byId('dt_baixa')._onChangeActive = true;
                        }
                    }

                    sugestaoCartao.cd_local_movto = data.cd_local_movto;
                    sugestaoCartao.nm_tipo_local = data.nm_tipo_local;
                    sugestaoCartao.cd_tipo_financeiro = data.cd_tipo_financeiro;
                    sugestaoCartao.cd_local_banco = data.cd_local_banco;

                    //Atualiza a grid:
                    criaAtualizaGridBaixaCad(data.Baixas, xhr, ref, sugestaoCartao);
                }
                catch (e) {
                    postGerarLog(e);
                    apresentaMensagem('apresentadorMensagemCadBaixa', error);
                    showCarregando();
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemCadBaixa', error);
                showCarregando();
            });
        }
    }
    catch (e) {
        postGerarLog(e);
        apresentaMensagem('apresentadorMensagemCadBaixa', error);
        showCarregando();
    }
}

function destroyCreateBaixaCad() {
    try {
        if (hasValue(dijit.byId("gridBaixaCad"))) {
            dijit.byId("gridBaixaCad").destroyRecursive();
            $('<div>').attr('id', 'gridBaixaCad').attr('style', 'height:310px;').appendTo('#PaigridBaixaCad');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatHintBaixa(value, rowIndex, obj) {
    var gridName = 'gridBaixaCad'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;

    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "nm_baixa", grid._by_idx[rowIndex].item.nm_baixa);

        value = value || indice != null; // Item está selecionado.
    }

    value = value.toString().replaceAll(' ', '&nbsp;');
    if (rowIndex != -1)
        icon = "<img alt='' src='" + Endereco() + "/images/help.png' id='imgObsBaixa' title=" + value + ">"

    return icon;
}

function formatValorMonetario(value, rowIndex, obj) {
    var retorno = maskFixed(value + "", 2);

    return retorno;
}

function formatDefineTipoDoc(value, titulos, rowIntex, obj) {
    var retorno = titulos.tipoDoc;
    console.log(retorno);

    return retorno;
}



function criaAtualizaGridBaixaCad(dadosBaixa, xhr, ref, sugestaoCartao) {
    require(["dojo/ready", "dojox/grid/EnhancedGrid", "dojo/data/ObjectStore", "dojo/store/Memory", "dijit/form/FilteringSelect"],
      function (ready, EnhancedGrid, ObjectStore, Memory, FilteringSelect) {
          ready(function () {
              try {
                  var dataStore = new ObjectStore({ objectStore: new Memory({ data: dadosBaixa }) });

                  destroyCreateBaixaCad();

                  var gridBaixaCad = new EnhancedGrid({
                      store: dataStore,
                      structure:
                          [
                            { name: "<input id='selecionaTodosBaixaCad' style='display:none'/>", field: "selecionadoBaixaCad", width: "4%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxBaixaCad },
                            { name: "Nro.", field: "nm_titulo", width: "7%", styles: "min-width:60px;" },
                            { name: "Parc.", field: "nm_parcela_titulo", width: "6%", styles: "min-width:60px;text-align: center;" },
                            { name: "Valor", field: "vl_liquidacao_baixa", width: "10%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberValor },
                            { name: "Desc.Juros", field: "vl_desc_juros_baixa", width: "8%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberDescJuros },
                            { name: "Desc.Multa", field: "vl_desc_multa_baixa", width: "10%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberDescMulta },
                            { name: "Juros", field: "vl_juros_baixa", width: "8%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberJuros },
                            { name: "Multa", field: "vl_multa_baixa", width: "8%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberMulta },
                            { name: "Tx. Cartão", field: "vl_taxa_cartao", width: "10%", styles: "text-align:right; min-width:60px;", formatter: formatTextVlTaxaCartaoBaixa },
                            { name: "Parcial", field: "id_baixa_parcial", width: "7%", styles: "text-align:center; min-width:60px;", formatter: formatCheckBoxParcial },
                            { name: "Desconto", field: "vl_desconto_baixa", width: "8%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberDesconto },
                            { name: "Vcto.", field: "dt_vcto_titulo", width: "9%", styles: "text-align:center;min-width:50px;" },
                            { name: "Nat.", field: "natureza_titulo", width: "5%", styles: "text-align:center;min-width:70px;" },
                            //{ name: "Vr.p/Troco ", field: "vl_valor_troco", width: "10%", styles: "text-align:right; min-width:60px;", formatter: formatTextNumberValorTroco },
                            { name: "Obs.", field: "obsBaixaTitulo", width: "5%", styles: "text-align:center;min-width:70px;", formatter: formatHintBaixa },
                            //{ name: "<input id='botaoCheque' style='display:none'/>", field: "selecionadoCheque", width: "60px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatButtonCheque },
                            {
                                name: "Cheque",
                                field: "_item",
                                width: '55px',
                                styles: "text-align: center;",
                                formatter: function (item) {
                                    var label = "Adicionar";
                                    if (hasValue(item.Titulo.Cheque)) {
                                        label = "Alterar";
                                        showViewCheque(item.Titulo, false);
                                    }

                                    var btn = dijit.form.Button({
                                        label: label,
                                        onClick: function () {
                                            try {
                                                dijit.byId('proChequeFK').set('title', 'Dados do cheque do Título: ' + item.nm_titulo + ' Parcela ' + item.nm_parcela_titulo);
                                                loadBancoViewCheque(hasValue(item.Titulo.Cheque) ? item.Titulo.Cheque.cd_banco : null, item.Titulo,true);
                                            } catch (e) {
                                            }
                                        }
                                    });
                                    setTimeout("alterarTamnhoBotao('" + btn.id + "')", 15);
                                    return btn;
                                }
                            }
                          ],
                      noDataMessage: msgNotRegEnc,
                      selectionMode: "single",
                      plugins: {
                          pagination: {
                              pageSizes: ["9", "18", "30", "100", "All"],
                              description: true,
                              sizeSwitch: true,
                              pageStepper: true,
                              defaultPageSize: "9",
                              gotoButton: true,
                              /*page step to be displayed*/
                              maxPageStep: 4,
                              /*position of the pagination bar*/
                              position: "button"
                          }
                      }
                  }, "gridBaixaCad");
                  gridBaixaCad.on("RowClick", function (evt) {
                      EVENTO_GRID_CHEQUE = evt;
                  }, true);
                  gridBaixaCad.rowsPerPage = 5000;
                  gridBaixaCad.pagination.plugin._paginator.plugin.connect(gridBaixaCad.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                      try {
                          verificaMostrarTodos(evt, gridBaixaCad, 'nm_baixa', 'selecionaTodosBaixaCad');
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  });
                  require(["dojo/aspect"], function (aspect) {
                      aspect.after(gridBaixaCad, "_onFetchComplete", function () {
                          try {
                              // Configura o check de todos:
                              if (hasValue(dojo.byId('selecionaTodosBaixaCad')) && dojo.byId('selecionaTodosBaixaCad').type == 'text')
                                  setTimeout("configuraCheckBox(false, 'nm_baixa', 'selecionadoBaixaCad', -1, 'selecionaTodosBaixaCad', 'selecionaTodosBaixaCad', 'gridBaixaCad')", gridBaixaCad.rowsPerPage * 3);
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });
                  });

                  gridBaixaCad.canSort = false;
                  gridBaixaCad.startup();
                  gridBaixaCad.itensSelecionados = new Array();
//LBM Estava aqui
                  
                  atualizarVlTotal(dadosBaixa);
                  if (dadosBaixa[0].cd_baixa_titulo == 0)
                    sugerirTipoLiquidacaoCheque();

                  if (hasValue(sugestaoCartao))
                      sugerirLiquidacaoELocalMovtoCartao(sugestaoCartao);
                  if (hasValue(dijit.byId("gridBaixa")) && dijit.byId("gridBaixa").itensSelecionados > 0) {
                      if (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value === CARTAOCREDITO || dijit.byId("cbLiquidacao").value === CARTAODEBITO)) {
                          getLocalMovtoGeralOuTipoLiquidacaoCartao(dijit.byId("cbLiquidacao").value, dijit.byId('gridBaixa').itensSelecionados[0].cd_local_movto);
                      } else if (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value === CHEQUEVISTA || dijit.byId("cbLiquidacao").value === CHEQUEPREDATADO)) {
                          getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect);
                          //getLocalMovtoGeralOuCartao(CHEQUE);
                      } else {
                          getLocalBaixaGeral(dijit.byId("gridBaixa").itensSelecionados[0].cd_tran_finan, dijit.byId("gridBaixa").itensSelecionados[0].cd_titulo, xhr, ready, Memory, FilteringSelect);
                      }

                      if (dijit.byId("gridBaixa").itensSelecionados[0].cd_tipo_liquidacao === CARTAOCREDITO || dijit.byId("gridBaixa").itensSelecionados[0].cd_tipo_liquidacao === CARTAODEBITO) {
                          dijit.byId("cbLiquidacao").set("disabled", true);
                      }
                  } else if (hasValue(dijit.byId("gridTitulo")) && dijit.byId("gridTitulo").itensSelecionados.length > 0) {
                      
                      if (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value === CARTAOCREDITO || dijit.byId("cbLiquidacao").value === CARTAODEBITO)) {
                          getLocalMovtoGeralOuTipoLiquidacaoCartao(dijit.byId("cbLiquidacao").value, dijit.byId('gridTitulo').itensSelecionados[0].cd_local_movto);
                      } else if (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value === CHEQUEVISTA || dijit.byId("cbLiquidacao").value === CHEQUEPREDATADO)) {
                          getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect);
                          //getLocalMovtoGeralOuCartao(CHEQUE);
                      } else {
                          getLocalMovimentoGeralInsercao(xhr, ready, Memory, FilteringSelect);
                      }
                  }
//LBM Veio para cá
                  var visibilidadeColunaCheque = false;
                  if (hasValue(dijit.byId("cbLiquidacao")) && hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value == CHEQUEPREDATADO || dijit.byId("cbLiquidacao").value == CHEQUEVISTA))
                      visibilidadeColunaCheque = true;
                  gridBaixaCad.layout.setColumnVisibility(14, visibilidadeColunaCheque);
                  hideCarregando();
              }
              catch (e) {
                  postGerarLog(e);
              }
          });
      });
}


/*-----------------*/
function formatTextVlTaxaCartaoBaixa(value, rowIndex, obj) {
    var gridBaixaCad = dijit.byId("gridBaixaCad");
    var icon;
    var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

    if (hasValue(dijit.byId(desc), true))
        dijit.byId(desc).destroy();
    if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

    setTimeout("configuraTextBoxVlTaxaCartaoBaixa('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + "," + gridBaixaCad._by_idx[rowIndex].item.Titulo.cd_tipo_financeiro + "," + gridBaixaCad._by_idx[rowIndex].item.vl_liquidacao_baixa + "," + gridBaixaCad._by_idx[rowIndex].item.vl_liquidacao_baixa + ")", 1);
    return icon;
}


/*-----------------*/



/*-----------------*/

function configuraTextBoxVlTaxaCartaoBaixa(value, desc, id, rowIndex, cd_tipo_financeiro, vl_liquidacao_baixa) {
    if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
    else value = value.toString().replace('.', ',');

    if (!hasValue(dijit.byId(desc))) {
        require(["dijit/form/NumberTextBox", "dojo/domReady!"], function (TextBox) {
            var newTextBox = new dijit.form.NumberTextBox({
                name: "textBox" + desc,
                old_value: unmaskFixed(value, 2),
                //diff_value: !vl_taxa_cartao ? 0 : unmaskFixed(value, 2),
                disabled:true,
                maxlength: 9,
                style: "width: 100%;",
                onBlur: function (b) {
                    $('#' + desc).focus();
                },
                onChange: function (b) {
                    atualizarValoresVlTaxaCartaoBaixa(desc, this, rowIndex, id);
                },
                smallDelta: 1,
                constraints: { min: 0, pattern: '##.00#' }
            }, desc);

            if (cd_tipo_financeiro != CARTAO && dijit.byId("cbLiquidacao").value != CARTAOCREDITO && dijit.byId("cbLiquidacao").value != CARTAODEBITO) {
                
	                apresentaMensagem("apresentadorMensagemCadBaixa", null);
	                newTextBox._onChangeActive = false;
	                newTextBox.set('value', unmaskFixed(0.0, 2));
	                newTextBox.value = unmaskFixed(0.0, 2);
	                newTextBox._onChangeActive = true;
                
	            

            } 
            else {
	            newTextBox._onChangeActive = false;
	            newTextBox.set('value', unmaskFixed(value, 2));
	            newTextBox.value = unmaskFixed(value, 2);
	            newTextBox._onChangeActive = true;
            }
            
        });
    }
    if (hasValue(dijit.byId(desc))) {
        dijit.byId(desc).on("keypress", function (e) {
            mascaraFloat(document.getElementById(desc));
        });
    }
}




/*-----------------*/



/*-----------------*/
function atualizarValoresVlTaxaCartaoBaixa(desc, obj, rowIndex, id) {
    try {

        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var item = getItemStoreTaxaCartaoBaixa(gridBaixaCad, id);
        var objDijit = dijit.byId(obj.id);

        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        consisteVlTaxaCartaoBaixa(item, objDijit.old_value, objDijit.value);
        gridBaixaCad.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}



/*-----------------*/


function getItemStoreTaxaCartaoBaixa(grid, id) {
    try {
        for (var i = 0; i < grid.store.objectStore.data.length; i++) {

            var _id = grid.store.objectStore.data[i].nm_baixa;
            if (_id == id)
                return grid.store.objectStore.data[i];
        }
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}




/*-----------------*/
function consisteVlTaxaCartaoBaixa(item, valorAntigo, valorAtual) {
    try {


        if (isNaN(valorAtual) || !hasValue(valorAtual, true)) {
            
	            item.vl_taxa_cartao = valorAntigo;
            
            if (item.vl_taxa_cartao > 0) {
	            item.pc_taxa_cartao_baixa = unmaskFixed(item.vl_taxa_cartao / (item.vl_liquidacao_baixa / 100), 2);
            } else {
	            item.pc_taxa_cartao_baixa = 0;
            }

            return;
        }

        item.vl_taxa_cartao = valorAtual;

       
        if (item.vl_taxa_cartao > 0) {
	        item.pc_taxa_cartao_baixa = unmaskFixed(item.vl_taxa_cartao / (item.vl_liquidacao_baixa / 100), 2);
        } else {
	        item.pc_taxa_cartao_baixa = 0;
        }
       
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatButtonCheque(value, rowIndex, obj) {
    try {
        var gridName = 'gridBaixaCad';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var label = "Adicionar";
        //if (hasValue(item.cheque))
        //    label = "Alterar";

        var btn = dijit.form.Button({
            label: label,
            //style: "max-width:32px;",
            onClick: function () {
                try {
                    dijit.byId('proChequeFK').set('title', 'Dados do cheque do Título: ' + item.nm_titulo + ' Parcela ' + item.nm_parcela_titulo);

                    var grid = dijit.byId("gridTitulo");
                    grid.on("RowClick", function (evt) {
                        EVENTO_GRID_CHEQUE = evt;
                    }, true);
                    loadBancoViewCheque(item.cd_banco, item, true);
                } catch (e) {

                }
            }
        });
        //"chque_" + item.nm_titulo + '_' + item.nm_parcela_titulo
        setTimeout("alterarTamnhoBotao('" + btn.id + "')", 15);
        //setTimeout("decreaseBtn(document.getElementById('" + btn.id + "', '32px'))", 100);
        //decreaseBtn(document.getElementById(id), '32px');
        return btn;
        //return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarTamnhoBotao(id) {
    decreaseBtn(document.getElementById(id), '48px');
}

function formatCheckBoxParcial(value, rowIndex, obj) {
    try {
        var gridName = 'gridBaixaCad';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_Parcial_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        setTimeout(function () {
            if (hasValue(dijit.byId(id)) && (!hasValue(dojoId) || dojoId.type == 'text'))
                dijit.byId(id).destroy();
            if (value == undefined)
                value = false;
            var dojoId = dojo.byId(id);
            if (hasValue(dojoId) && dojoId.type == 'text')
                var vl_desconto_baixa = grid._by_idx[rowIndex].item.vl_desconto_baixa;
            var compTipLiqui = dijit.byId("cbLiquidacao");
            var desabilitar = false;
            if (!hasValue(vl_desconto_baixa) || (hasValue(compTipLiqui)) && compTipLiqui.value == BAIXACANCELAMENTO) {
                desabilitar = true;
                //value = false;
            }
            //Sugerindo sempre baixa parcial quando valor da baixa for menor
            //if (vl_desconto_baixa > 0 && !value) {
            //    value = true;
            //}
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox" + id,
                checked: value,
                disabled: desabilitar,
                //value: desabilitar,
                onChange: function (b) { atualizarCkParcial(id, this, rowIndex, grid._by_idx[rowIndex].item.nm_baixa); } //grid.getItem(rowIndex).id_baixa_parcial = this.checked;
            }, id);
        }, 3);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarCkParcial(desc, obj, rowIndex, nm_baixa) {
    try {
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var item = getItemStore(gridBaixaCad, nm_baixa);
        var objDijit = dijit.byId(obj.id);
        var value = obj.checked;
        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        if (verificarRegraMaterialTitulo(gridBaixaCad._by_idx[rowIndex].item, null, obj.checked)) {
            value = gridBaixaCad._by_idx[rowIndex].item.id_baixa_parcial;
        }
        item.id_baixa_parcial = value;
        gridBaixaCad.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxBaixaCad(value, rowIndex, obj) {
    try {
        var gridName = 'gridBaixaCad';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosBaixaCad');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_baixa", grid._by_idx[rowIndex].item.nm_baixa);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_baixa', 'selecionadoBaixaCad', -1, 'selecionaTodosBaixaCad', 'selecionaTodosBaixaCad', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'nm_baixa', 'selecionadoBaixaCad', " + rowIndex + ", '" + id + "', 'selecionaTodosBaixaCad', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTrocaFinan(value, rowIndex, obj) {
    try {
        var gridName = 'gridBaixaCad';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosBaixaCad');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_parcela_titulo", grid._by_idx[rowIndex].item.nm_parcela_titulo);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_parcela_titulo', 'selecionadoBaixaCad', -1, 'selecionaTodosBaixaCad', 'selecionaTodosBaixaCad', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'nm_parcela_titulo', 'selecionadoBaixaCad', " + rowIndex + ", '" + id + "', 'selecionaTodosBaixaCad', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function formatTextNumberValor(value, rowIndex, obj) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var icon;
        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxValor('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + ")", 3);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTipodocumento(value, rowIndex, obj) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var icon;
        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nomeResponsavel;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxValor('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nomeResponsavel + "'," + rowIndex + ")", 3);
        return icon;
    }catch (e) {
        postGerarLog(e);
    }
}

function formatTipodocumento(value, rowIndex, obj) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var icon;
        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.tipoDoc;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxValor('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.tipoDoc + "'," + rowIndex + ")", 3);
        return icon;
    }catch (e) {
        postGerarLog(e);
    }
}



function configuraTextBoxValor(value, desc, nm_baixa, rowIndex) {
    try {
        var compTipLiqui = dijit.byId("cbLiquidacao");
        var desabilitar = false;
        if (hasValue(compTipLiqui) && compTipLiqui.value == BAIXACANCELAMENTO)
            desabilitar = true;
        if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
        else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            var newTextBox = new dijit.form.NumberTextBox({
                name: "textBox" + desc,
                disabled: desabilitar,
                //value: unmaskFixed(value, 2),
                old_value: unmaskFixed(value, 15),
                maxlength: 9,
                style: "width: 100%;",
                onBlur: function (b) {
                    $('#' + desc).focus();
                },
                onChange: function (b) {
                    atualizarValores(desc, this, rowIndex, nm_baixa);
                    calcularValorTroco();
                },
                smallDelta: 1,
                constraints: { min: 0, pattern: '##.00#' }
            }, desc);
            newTextBox._onChangeActive = false;
            newTextBox.set('value', unmaskFixed(value, 2));
            newTextBox.value = unmaskFixed(value, 15);
            newTextBox._onChangeActive = true;
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraFloat(document.getElementById(desc))
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarValores(desc, obj, rowIndex, nm_baixa) {
    try {
        
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var item = getItemStore(gridBaixaCad, nm_baixa);
        var objDijit = dijit.byId(obj.id);

        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        consisteJurosDesconto(item, objDijit.old_value, objDijit.value, item.vl_desc_juros_baixa, item.vl_juros_baixa, item.id_baixa_parcial);
        gridBaixaCad.update();        
    }
    catch (e) {
        postGerarLog(e);
    }
}

// VALOR TROCO
//function formatTextNumberValorTroco(value, rowIndex, obj) {
//    try {
//        var gridBaixaCad = dijit.byId("gridBaixaCad");
//        var icon;
//        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

//        if (hasValue(dijit.byId(desc), true))
//            dijit.byId(desc).destroy();
//        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

//        setTimeout("configuraTextBoxValorTroco('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + ")", 3);
//        return icon;
//    }
//    catch (e) {
//        postGerarLog(e);
//    }
//}

//function configuraTextBoxValorTroco(value, desc, nm_baixa, rowIndex) {
//    try {
//        var compTipLiqui = dijit.byId("cbLiquidacao");
//        var desabilitar = false;
//        if (hasValue(compTipLiqui) && compTipLiqui.value == BAIXACANCELAMENTO)
//            desabilitar = true;
//        if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
//        else value = value.toString().replace('.', ',');

//        if (!hasValue(dijit.byId(desc))) {
//            var newTextBox = new dijit.form.NumberTextBox({
//                name: "textBox" + desc,
//                disabled: desabilitar,
//                //value: unmaskFixed(value, 2),
//                old_value: unmaskFixed(value, 15),
//                maxlength: 9,
//                style: "width: 100%;",
//                onBlur: function (b) {
//                    $('#' + desc).focus();
//                },
//                onChange: function (vlTroco) {
//                    calcularValorTroco(vlTroco, nm_baixa);
//                },
//                smallDelta: 1,
//                constraints: { min: 0, pattern: '##.00#' }
//            }, desc);
//            newTextBox._onChangeActive = false;
//            newTextBox.set('value', unmaskFixed(value, 2));
//            newTextBox.value = unmaskFixed(value, 15);
//            newTextBox._onChangeActive = true;
//        }
//        if (hasValue(dijit.byId(desc))) {
//            dijit.byId(desc).on("keypress", function (e) {
//                mascaraFloat(document.getElementById(desc))
//            });
//        }
//    }
//    catch (e) {
//        postGerarLog(e);
//    }
//}

function configuraTextBoxDescMulta(value, desc, nm_baixa, rowIndex) {
    try {
        var compTipLiqui = dijit.byId("cbLiquidacao");
        var desabilitar = false;
        if (hasValue(compTipLiqui) && compTipLiqui.value == BAIXACANCELAMENTO)
            desabilitar = true;
        if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
        else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            require(["dijit/form/NumberTextBox", "dojo/domReady!"], function (TextBox) {
                var newTextBox = new dijit.form.NumberTextBox({
                    name: "textBox" + desc,
                    value: unmaskFixed(value, 2),
                    disabled: desabilitar,
                    old_value: unmaskFixed(value, 2),
                    maxlength: 9,
                    style: "width: 100%;",
                    onBlur: function (b) {
                        $('#' + desc).focus();
                    },
                    onChange: function (b) {
                        atualizarDescMulta(desc, this, rowIndex, nm_baixa, true);
                        calcularValorTroco();
                    },
                    maxlength: 9,
                    style: "width: 100%;",
                    smallDelta: 1,
                    constraints: { min: 0, pattern: '##.00#' }
                }, desc);

                //dijit.byId(desc).set('value', unmaskFixed(value, 2));
            });
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraFloat(document.getElementById(desc))
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consisteJurosDesconto(item, valorAntigo, valorAtual, descJuros, juros, baixaParcial) {
    try {
        var vl_desconto_baixa = item.vl_desconto_baixa;
        var vl_juros_baixa = item.vl_juros_baixa;
        var vl_multa_baixa = item.vl_multa_baixa;
        var id_baixa_parcial = item.id_baixa_parcial;

        if (isNaN(valorAtual) || !hasValue(valorAtual, true)) {
            item.vl_liquidacao_baixa = valorAntigo;
            return;
        }
        else if (valorAntigo < valorAtual) {
            id_baixa_parcial = false;
            var jurosCalc = valorAtual - valorAntigo;

            //Tira dos descontos e se não houver mais coloca como juros:
            if (vl_desconto_baixa > jurosCalc)
                vl_desconto_baixa = vl_desconto_baixa - jurosCalc;
            else {
                vl_juros_baixa = item.vl_juros_baixa + (jurosCalc - vl_desconto_baixa);
                vl_desconto_baixa = 0;
            }

        }
        else {
            var liquidarJuros = false;
            var jurosCalc = valorAntigo - valorAtual;
            //Calcula a diferença da multa e desconto da multa, pois o desconto da multa será mantido:
            if ((item.vl_multa_baixa - item.vl_desc_multa_baixa) < jurosCalc) {
                //vl_desconto_baixa += jurosCalc - (item.vl_multa_baixa - item.vl_desc_multa_baixa);
                vl_multa_baixa = item.vl_desc_multa_baixa;

                jurosCalc = jurosCalc - (item.vl_multa_baixa - item.vl_desc_multa_baixa);
                liquidarJuros = true;
            }
            else {
                vl_multa_baixa = item.vl_multa_baixa - jurosCalc;
                liquidarJuros = false;
            }

            //Calcula a diferença do juros e desconto de juros, pois o desconto de juros será mantido:
            if (liquidarJuros)
                if ((juros - descJuros) < jurosCalc) {
                    vl_desconto_baixa += jurosCalc - (juros - descJuros);
                    vl_juros_baixa = descJuros;
                }
                else
                    vl_juros_baixa = item.vl_juros_baixa - jurosCalc;
            //Se for baixa parcial, zera o desconto:
            //if (baixaParcial)
            //    vl_desconto_baixa = 0;
        }
        if (vl_desconto_baixa > 0)
            id_baixa_parcial = true;
        if (verificarRegraMaterialTitulo(item, vl_desconto_baixa, id_baixa_parcial))
            return false;
        item.id_baixa_parcial = id_baixa_parcial;
        item.vl_liquidacao_baixa = valorAtual;
        item.vl_juros_baixa = vl_juros_baixa;
        item.vl_multa_baixa = vl_multa_baixa;
        item.vl_desconto_baixa = vl_desconto_baixa;
        var total = dijit.byId("vlTotal").value - valorAntigo + valorAtual;
        unmaskFixed(dijit.byId("vlTotal").set("value", total) + "", 2);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getItemStore(grid, id) {
    try {
        for (var i = 0; i < grid.store.objectStore.data.length; i++) {

            var nm_baixa = grid.store.objectStore.data[i].nm_baixa;
            if (nm_baixa == id)
                return grid.store.objectStore.data[i];
        }
        return null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTextNumberDescJuros(value, rowIndex, obj) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var icon;
        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxDescJuros('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + ")", 3);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxDescJuros(value, desc, nm_baixa, rowIndex) {
    try {
        var compTipLiqui = dijit.byId("cbLiquidacao");
        var desabilitar = false;
        if (hasValue(compTipLiqui) && compTipLiqui.value == BAIXACANCELAMENTO)
            desabilitar = true;
        if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
        else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            require(["dijit/form/NumberTextBox", "dojo/domReady!"], function (TextBox) {
                var newTextBox = new dijit.form.NumberTextBox({
                    name: "textBox" + desc,
                    value: unmaskFixed(value, 2),
                    disabled: desabilitar,
                    old_value: unmaskFixed(value, 2),
                    maxlength: 9,
                    style: "width: 100%;",
                    onBlur: function (b) {
                        $('#' + desc).focus();
                    },
                    onChange: function (b) {
                        atualizarDescJuros(desc, this, rowIndex, nm_baixa, true);
                        calcularValorTroco();
                    },
                    smallDelta: 1,
                    constraints: { min: 0, pattern: '##.00#' }
                }, desc);

                //dijit.byId(desc).set('value', unmaskFixed(value, 2));
            });
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraFloat(document.getElementById(desc));
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarDescJuros(desc, obj, rowIndex, nm_baixa) {
    try {
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var item = getItemStore(gridBaixaCad, nm_baixa);
        var objDijit = dijit.byId(obj.id);

        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        consisteValorPeloDescJuros(item, objDijit.old_value, objDijit.value, item.vl_liquidacao_baixa);
        //calcularValorTroco(item.vl_valor_troco, nm_baixa);
        gridBaixaCad.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consisteValorPeloDescJuros(item, antigoValorDescJuros, novoValorDescJuros, valor) {
    try {
        //Verifica se o valor de desconto de juros ultrapassa o valor:
        if (isNaN(novoValorDescJuros) || !hasValue(novoValorDescJuros, true))
            item.vl_desc_juros_baixa = antigoValorDescJuros;
        else if (unmaskFixed(novoValorDescJuros, 2) > unmaskFixed(valor, 2)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroValorDescontoUltrapassaValor);
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);

            item.vl_desc_juros_baixa = antigoValorDescJuros;
        }
        else if (unmaskFixed(novoValorDescJuros, 2) > unmaskFixed(item.vl_juros_baixa, 2)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroValorDescontoUltrapassaJuros);
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);

            item.vl_desc_juros_baixa = antigoValorDescJuros;
        }
        else {
            if (antigoValorDescJuros < novoValorDescJuros) {
                item.vl_liquidacao_baixa -= (novoValorDescJuros - antigoValorDescJuros);
                var total = dijit.byId("vlTotal").value - valor + item.vl_liquidacao_baixa;
                unmaskFixed(dijit.byId("vlTotal").set("value", total) + "", 2);
            }
            else
                item.vl_juros_baixa -= (antigoValorDescJuros - novoValorDescJuros);
            item.vl_desc_juros_baixa = novoValorDescJuros;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTextNumberDescMulta(value, rowIndex, obj) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var icon;
        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxDescMulta('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + ")", 1);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarDescMulta(desc, obj, rowIndex, nm_baixa) {
    try {
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var item = getItemStore(gridBaixaCad, nm_baixa);
        var objDijit = dijit.byId(obj.id);

        apresentaMensagem("apresentadorMensagemCadBaixa", '');
        consisteValorPeloDescMulta(item, objDijit.old_value, objDijit.value, item.vl_liquidacao_baixa);
        //calcularValorTroco(item.vl_valor_troco, nm_baixa);
        gridBaixaCad.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function consisteValorPeloDescMulta(item, antigoValorDescMulta, novoValorDescMulta, valor) {
    try {
        //Verifica se o valor de desconto de multa ultrapassa o valor:
        if (isNaN(novoValorDescMulta) || !hasValue(novoValorDescMulta, true))
            item.vl_desc_multa_baixa = antigoValorDescMulta;
        else if (unmaskFixed(novoValorDescMulta, 2) > unmaskFixed(valor, 2)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroValorDescontoMultaUltrapassaValor);
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);

            item.vl_desc_multa_baixa = antigoValorDescMulta;
        }
        else if (unmaskFixed(novoValorDescMulta, 2) > unmaskFixed(item.vl_multa_baixa, 2)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroValorDescontoMultaUltrapassaMulta);
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);

            item.vl_desc_multa_baixa = antigoValorDescMulta;
        }
        else {
            if (antigoValorDescMulta < novoValorDescMulta) {
                item.vl_liquidacao_baixa -= (novoValorDescMulta - antigoValorDescMulta);

                var total = dijit.byId("vlTotal").value - valor + item.vl_liquidacao_baixa;
                unmaskFixed(dijit.byId("vlTotal").set("value", total) + "", 2);
            }
            else
                item.vl_multa_baixa -= (antigoValorDescMulta - novoValorDescMulta);
            item.vl_desc_multa_baixa = novoValorDescMulta;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTextNumberMulta(value, rowIndex, obj) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var icon;
        var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

        if (hasValue(dijit.byId(desc), true))
            dijit.byId(desc).destroy();
        if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

        setTimeout("configuraTextBoxMulta('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + ")", 1);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraTextBoxMulta(value, desc, nm_baixa, rowIndex) {
    try {
        if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
        else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            require(["dijit/form/NumberTextBox", "dojo/domReady!"], function (TextBox) {
                var newTextBox = new dijit.form.NumberTextBox({
                    name: "textBox" + desc,
                    value: unmaskFixed(value, 2),
                    disabled: true,
                    maxlength: 9,
                    style: "width: 100%;",
                    smallDelta: 1,
                    constraints: { min: 0, pattern: '##.00#' }
                }, desc);

                //dijit.byId(desc).set('value', unmaskFixed(value, 2));
            });
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraFloat(document.getElementById(desc));
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatTextNumberDesconto(value, rowIndex, obj) {
    var gridBaixaCad = dijit.byId("gridBaixaCad");
    var icon;
    var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

    if (hasValue(dijit.byId(desc), true))
        dijit.byId(desc).destroy();
    if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

    setTimeout("configuraTextBoxDesconto('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + "," + gridBaixaCad._by_idx[rowIndex].item.id_baixa_parcial + ")", 1);
    return icon;
}

function configuraTextBoxDesconto(value, desc, nm_baixa, rowIndex, id_baixa_parcial) {
    if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
    else value = value.toString().replace('.', ',');

    if (!hasValue(dijit.byId(desc))) {
        require(["dijit/form/NumberTextBox", "dojo/domReady!"], function (TextBox) {
            var newTextBox = new dijit.form.NumberTextBox({
                name: "textBox" + desc,
                value: id_baixa_parcial ? 0 : unmaskFixed(value, 2),
                diff_value: !id_baixa_parcial ? 0 : unmaskFixed(value, 2),
                disabled: true,
                maxlength: 9,
                style: "width: 100%;",
                smallDelta: 1,
                constraints: { min: 0, pattern: '##.00#' }
            }, desc);

            //dijit.byId(desc).set('value', unmaskFixed(value, 2));
        });
    }
    if (hasValue(dijit.byId(desc))) {
        dijit.byId(desc).on("keypress", function (e) {
            mascaraFloat(document.getElementById(desc));
        });
    }
}

function formatTextNumberJuros(value, rowIndex, obj) {
    var gridBaixaCad = dijit.byId("gridBaixaCad");
    var icon;
    var desc = obj.field + '_input_' + gridBaixaCad._by_idx[rowIndex].item.nm_baixa;

    if (hasValue(dijit.byId(desc), true))
        dijit.byId(desc).destroy();
    if (rowIndex != -1) icon = "<input id='" + desc + "' style='height:17px' /> ";

    setTimeout("configuraTextBoxJuros('" + value + "', '" + desc + "','" + gridBaixaCad._by_idx[rowIndex].item.nm_baixa + "'," + rowIndex + ")", 1);
    return icon;
}

function configuraTextBoxJuros(value, desc, nm_baixa, rowIndex) {
    try {
        if (value == undefined || isNaN(parseFloat(value))) value = '0,00';
        else value = value.toString().replace('.', ',');

        if (!hasValue(dijit.byId(desc))) {
            require(
                ["dijit/form/NumberTextBox"], function (TextBox) {
                    var newTextBox = new dijit.form.NumberTextBox({
                        name: "textBox" + desc,
                        value: unmaskFixed(value, 2),
                        disabled: true,
                        maxlength: 9,
                        style: "width: 100%;",
                        smallDelta: 1,
                        constraints: { min: 0, pattern: '##.00#' }
                    }, desc);

                    //dijit.byId(desc).set('value', unmaskFixed(value, 2));
                });
        }
        if (hasValue(dijit.byId(desc))) {
            dijit.byId(desc).on("keypress", function (e) {
                mascaraFloat(document.getElementById(desc));
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaListaBaixa() {
    try {
        var tipoLiqCheque = false;
        var DadosChqueBaixa = null;
        if (hasValue(ISVIEW_CHEQUE_TRANSACAO) && ISVIEW_CHEQUE_TRANSACAO && (dijit.byId("cbLiquidacao").value == CHEQUEPREDATADO || dijit.byId("cbLiquidacao").value == CHEQUEVISTA)) {
            tipoLiqCheque = true;
            DadosChqueBaixa = {
                cd_cheque : hasValue(dojo.byId("cd_cheque_trans").value) ? dojo.byId("cd_cheque_trans").value : 0,
                no_emitente_cheque: dijit.byId("emissorChequeBaixa").value,
                no_agencia_cheque: dijit.byId("nomeAgenciaChequeBaixa").value,
                nm_agencia_cheque: dijit.byId("nroAgenciaChequeBaixa").value,
                nm_digito_agencia_cheque: dijit.byId("dgAgenciaChequeBaixa").value,
                nm_conta_corrente_cheque: dijit.byId("nroContaCorrenteChequeBaixa").value,
                nm_digito_cc_cheque: dijit.byId("dgContaCorrenteChequeBaixa").value,
                nm_primeiro_cheque: dijit.byId("nroPrimeiroChequeChequeBaixa").value,
                cd_banco: dijit.byId("bancoChequeBaixa").value,
                dt_bom_para: dijit.byId("dtChequeChequeBaixa").value
            }
        }
        var gridBaixaCad = dijit.byId('gridBaixaCad');
        var storeBaixa = cloneArray(gridBaixaCad.store.objectStore.data);
        return {
            Baixas: storeBaixa,
            dt_tran_finan: dojo.date.locale.parse(dojo.byId('dt_baixa').value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            cd_local_movto: dijit.byId("cbLocalCad").value,
            cd_tipo_liquidacao: dijit.byId("cbLiquidacao").value,
            cd_tran_finan: dojo.byId("cd_tran_finan").value,

            vl_total_baixa: dijit.byId("vlTotal").get("value"),
            vl_total_troco: dijit.byId("vlTotalTroco").get("value"),
            vl_troco: dijit.byId("vlTroco").get("value"),
            cheque: DadosChqueBaixa,
            cd_tipo_liquidacao_old: dijit.byId("cbLiquidacao").value,
        };
    }
    catch (e) {
        postGerarLog(e);
    }
}

function calcularJuros(item, valorAtual) {
    try {
        var juros_Calc = 0;
        var multa_Calc = 0;
        //Calcular o juros e multa proporcional ao valor editado.
        if (item.id_baixa_parcial) {
            juros_Calc = item.pc_juros_calc * valorAtual / 100;
            multa_Calc = item.pc_multa_calc * valorAtual / 100;
        }
        else {
            juros_Calc = item.pc_juros_calc * ((valorAtual - item.vl_desconto_baixa) / 100);
            multa_Calc = item.pc_multa_calc * ((valorAtual - item.vl_desconto_baixa) / 100);
        }

        item.vl_multa_calculada = multa_Calc;
        item.vl_juros_calculado = juros_Calc;
        return item;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTagBaixa(value) {
    try {
        if (value == true) {
            dojo.byId('divBaixa').style.height = '345px';
        }
        else
            dojo.byId('divBaixa').style.height = '380px';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function confLayoutCadBaixa(bool) {
    try {
        var gridBaixaCad = dijit.byId("gridBaixaCad");
        var qtdRow = dijit.byId("gridBaixaCad").rowCount;
        for (var i = 0; i < qtdRow; i++) {
            dijit.byId(gridBaixaCad.getRowNode(i).childNodes[0].childNodes[0].childNodes[0].childNodes[3].firstChild.children[1].firstChild.id).set("disabled", true);
            dijit.byId(gridBaixaCad.getRowNode(i).childNodes[0].childNodes[0].childNodes[0].childNodes[4].firstChild.children[1].firstChild.id).set("disabled", true);
            dijit.byId(gridBaixaCad.getRowNode(i).childNodes[0].childNodes[0].childNodes[0].childNodes[5].firstChild.children[1].firstChild.id).set("disabled", true);
            dijit.byId(gridBaixaCad.getRowNode(i).childNodes[0].childNodes[0].childNodes[0].childNodes[8].firstChild.firstChild.id).set("disabled", true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarRegraMaterialTitulo(item, vl_desconto_baixa, id_baixa_parcial) {
    var retornoErro = false;
    if (!hasValue(vl_desconto_baixa))
        vl_desconto_baixa = item.vl_desconto_baixa;
    if (id_baixa_parcial == null)
        id_baixa_parcial = item.id_baixa_parcial;
    if (item != null && item.Titulo != null && hasValue(item.Titulo.vl_material_titulo) && item.Titulo.vl_material_titulo > 0) {
        var vl_material_titulo = item.Titulo.vl_material_titulo;
        if ((((item.vl_principal_baixa - vl_material_titulo) < 0) || ((item.vl_principal_baixa - vl_desconto_baixa) - vl_material_titulo < 0)) && !id_baixa_parcial) {
            caixaDialogo(DIALOGO_AVISO, msgErroValorDescontoUltrapassouValorMaterial, null);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroValorDescontoUltrapassouValorMaterial);
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
            retornoErro = true;
        }
    }
    return retornoErro;
}

function validarTipoLiqChequeTransacao(event) {
    var gridBaixaCad = dijit.byId('gridBaixaCad').store.objectStore.data;
    var mensagensWeb = new Array();

    if (!hasValue(gridBaixaCad)) {
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaSelecionado);//'É obrigatório selecionar pelo menos um título para a baixa.';
        apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
        return false;
    }
    //isViewChequeTransacao = false;

    var isCheque = [];
    var isChequeTransacao = [];

    isCheque = jQuery.grep(gridBaixaCad, function (titulo) {
        if (titulo.Titulo.cd_tipo_financeiro == CHEQUEPREDATADO)
            return true;
    });

    isChequeTransacao = jQuery.grep(gridBaixaCad, function (titulo) {
        if (titulo.Titulo.cd_tipo_financeiro != CHEQUEPREDATADO)
            return true;
    });

    if (event == CHEQUEPREDATADO || event == CHEQUEVISTA) {
        // Retorna mensagem de erro informando os titulos que não são cheque.
        if (isCheque.length > 0 && isChequeTransacao.length > 0) {
            document.getElementById("tgCheque").style.display = "none";
            dijit.byId('gridBaixaCad').layout.setColumnVisibility(13, false);
            if (isViewChequeTransacao != null) {
                if (isViewChequeTransacao)
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSelecionadoTransacao);//"Não é possível escolher um título com o tipo financeiro “Cheque Pré-Datado” ou “Cheque a Vista” quando se está liquidando vários títulos com um cheque. Caso queira liquidar o título com o cheque informado no cadastro do título, favor desselecionar os outros títulos que o tipo financeiro não é cheque.";
                else
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTituloSelecionadoCheque);//"Não é possível escolher um título com o tipo financeiro diferente de “Cheque Pré-Datado” ou “Cheque a Vista” quando se está liquidando o título com o cheque informado no cadastro do título, favor desselecionar os outros títulos que o tipo financeiro não é cheque.";
            } else
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTiposFinanceirosMistosCheque);// "Caso queira liquidar o(s) título(s) que seja(m) do tipo financeiro “Cheque”, utilizando  o cheque informado em seu cadastro, favor excluir o(s) outro(s) título(s) com o(s) tipo(s) diferente(s) de “Cheque”. Caso queira liquidar os títulos com o tipo diferente de “Cheque” ou pagar todos os títulos com um cheque, favor excluir o(s) título(s) com o tipo financeiro “Cheque”.";
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
            dijit.byId("cbLiquidacao").reset();
            return false;
        }

        // Libera o botão adicionar cheque para cada titulo selecionado do grid.
        if (isCheque.length > 0 && isChequeTransacao.length == 0) {
            apresentaMensagem("apresentadorMensagemCadBaixa", "");
            dijit.byId('gridBaixaCad').layout.setColumnVisibility(13, true);
            ISVIEW_CHEQUE_TRANSACAO = false;
            setRequiredCamposChequeBaia(false);
            return true;
        }

        // Libera a opção de inserir um unico cheque para todos os titulos.
        if (isCheque.length == 0 && isChequeTransacao.length > 0) {
            dijit.byId('gridBaixaCad').layout.setColumnVisibility(13, false);
            document.getElementById("tgCheque").style.display = "";
            ISVIEW_CHEQUE_TRANSACAO = true;
            setRequiredCamposChequeBaia(true);
            return true;
        }
    } else {
        ISVIEW_CHEQUE_TRANSACAO = null;
        //dijit.byId("tgCheque").set("open", true);
        document.getElementById("tgCheque").style.display = "none";
        dijit.byId('gridBaixaCad').layout.setColumnVisibility(13, false);
        isViewChequeTransacao = null;
        setRequiredCamposChequeBaia(false);
        if (isCheque.length > 0 && isChequeTransacao.length > 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTiposFinanceirosMistosCheque);
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
            dijit.byId("cbLiquidacao").reset();
            return false;
        }
        //Não deixar liquidar os títulos financeiros do tipo cheque com outro tipo de liquidação que não seja cheque pré-datado ou avista
        if (isCheque.length > 0 && isChequeTransacao.length == 0 && event != CHEQUEPREDATADO) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidChequeDif);//"Não se pode liquidar Cheques com tipos de liquidação diferentes de \"Cheque Pré-Datado\" ou \"Cheque a Vista\".";
            apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
            dijit.byId("cbLiquidacao").reset();
            return false;
        }
    }
}

function validaCartaoChequeETroca(event) {
    var mensagensWeb = new Array();
    if (event == TROCA_FINANCEIRA) {
        apresentaMensagem("apresentadorMensagemCadBaixa", null);
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloTrocaFinanceiraDif);
        apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
        dijit.byId("cbLiquidacao").reset();
        return false;
    } 

    if (hasValue(dijit.byId("gridBaixa").itensSelecionados)) {

        if (dijit.byId("gridBaixa").itenSelecionado.cd_tipo_financeiro === CARTAO){ 
            var mensagemerro = "";
            if (dijit.byId("gridBaixa").itenSelecionado.nm_tipo_local === LOCALCARTAOCREDITO && hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value !== CARTAOCREDITO))
                mensagemerro = msgErroBaixaTituloLiquidCartaoCDif
            else if(dijit.byId("gridBaixa").itenSelecionado.nm_tipo_local === LOCALCARTAODEBITO && dijit.byId("cbLiquidacao").value !== CARTAODEBITO)
                mensagemerro = msgErroBaixaTituloLiquidCartaoDDif
            if(mensagemerro !="") {   
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                //"Não se pode liquidar Cartões com tipos de liquidação diferentes de \"Cartão de Crédito\" ou \"Cartão de Débito\".";
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemerro);
                apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                dijit.byId("cbLiquidacao").reset();
                return false;
            }
        } else if (dijit.byId("gridBaixa").itenSelecionado.cd_tipo_financeiro === CHEQUE &&
                hasValue(dijit.byId("cbLiquidacao").value) &&
                (dijit.byId("cbLiquidacao").value !== CHEQUEVISTA &&
                    dijit.byId("cbLiquidacao").value !== CHEQUEPREDATADO)) {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidChequeDif);
                apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                dijit.byId("cbLiquidacao").reset();
                return false;
        } else if (dijit.byId("gridBaixa").itenSelecionado.cd_tipo_financeiro === CARTAO && (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value === CARTAOCREDITO || dijit.byId("cbLiquidacao").value === CARTAODEBITO))) {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
        } else if (dijit.byId("gridBaixa").itenSelecionado.cd_tipo_financeiro === CHEQUE &&
                hasValue(dijit.byId("cbLiquidacao").value) &&
                (dijit.byId("cbLiquidacao").value === CHEQUEVISTA ||
                    dijit.byId("cbLiquidacao").value === CHEQUEPREDATADO)) {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
            }
        

    } else if (hasValue(dijit.byId("gridTitulo").itensSelecionados) && dijit.byId("gridTitulo").itensSelecionados.length > 0) {

        if (dijit.byId("gridTitulo").itensSelecionados[0].cd_tipo_financeiro === CARTAO) {
            var mensagemerro = "";
            if(dijit.byId("gridTitulo").itensSelecionados[0].nm_tipo_local === LOCALCARTAOCREDITO && dijit.byId("cbLiquidacao").value !== CARTAOCREDITO) 
                mensagemerro = msgErroBaixaTituloLiquidCartaoCDif
            else if (dijit.byId("gridTitulo").itensSelecionados[0].nm_tipo_local === LOCALCARTAODEBITO && dijit.byId("cbLiquidacao").value !== CARTAODEBITO)
                mensagemerro = msgErroBaixaTituloLiquidCartaoDDif

            //(hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value !== CARTAOCREDITO && dijit.byId("cbLiquidacao").value !== CARTAODEBITO)))
            if (mensagemerro != "")
            {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemerro);
                apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                dijit.byId("cbLiquidacao").reset();
                dijit.byId("cbLocalCad").reset();
                return false;
            } 
        }else if (dijit.byId("gridTitulo").itensSelecionados[0].cd_tipo_financeiro === CHEQUE &&
                hasValue(dijit.byId("cbLiquidacao").value) &&
                (dijit.byId("cbLiquidacao").value !== CHEQUEVISTA && dijit.byId("cbLiquidacao").value !== CHEQUEPREDATADO)) {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBaixaTituloLiquidChequeDif);
                apresentaMensagem("apresentadorMensagemCadBaixa", mensagensWeb);
                dijit.byId("cbLiquidacao").reset();
                dijit.byId("cbLocalCad").reset();
                return false;
        } else if (dijit.byId("gridTitulo").itensSelecionados[0].cd_tipo_financeiro === CARTAO && (hasValue(dijit.byId("cbLiquidacao").value) && (dijit.byId("cbLiquidacao").value === CARTAOCREDITO || dijit.byId("cbLiquidacao").value === CARTAODEBITO))) {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
            } else if (dijit.byId("gridTitulo").itensSelecionados[0].cd_tipo_financeiro === CHEQUE &&
                hasValue(dijit.byId("cbLiquidacao").value) &&
                (dijit.byId("cbLiquidacao").value === CHEQUEVISTA ||
                    dijit.byId("cbLiquidacao").value === CHEQUEPREDATADO)) {
                apresentaMensagem("apresentadorMensagemCadBaixa", null);
            }

    }
}

//Funções Cheque
function loadBancoChequeViewCheque(dataTitulo) {
    // Popula os produtos:
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getAllBanco",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataBanco) {
        try {
            if (hasValue(dataBanco.retorno)) {
                criarOuCarregarCompFiltering("bancosViewCheque", dataBanco.retorno, "", hasValue(dataTitulo.Titulo.Cheque) ? dataTitulo.Titulo.Cheque.cd_banco : null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco');
            }
            showViewCheque(dataTitulo.Titulo,true);
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemMat', error);
    });
}

function setRequiredCamposChequeBaia(isRequired) {
    dijit.byId("tgCheque").set("open", false);
	dijit.byId("nomeAgenciaChequeBaixa").set("required", isRequired);
    dijit.byId("emissorChequeBaixa").set("required", isRequired);
    dijit.byId("nroAgenciaChequeBaixa").set("required", isRequired);
    dijit.byId("dgAgenciaChequeBaixa").set("required", isRequired);
    dijit.byId("dtChequeChequeBaixa").set("required", isRequired);
    dijit.byId("bancoChequeBaixa").set("required", isRequired);
    dijit.byId("nroContaCorrenteChequeBaixa").set("required", isRequired);
    dijit.byId("dgContaCorrenteChequeBaixa").set("required", isRequired);
    dijit.byId("nroPrimeiroChequeChequeBaixa").set("required", isRequired);
}

function sugerirTipoLiquidacaoCheque() {
    var gridBaixaCad = dijit.byId('gridBaixaCad').store.objectStore.data;
    var isCheque = [];
    var isChequeTransacao = [];

    isCheque = jQuery.grep(gridBaixaCad, function (titulo) {
        if (titulo.Titulo.cd_tipo_financeiro == CHEQUEPREDATADO)
            return true;
    });

    isChequeTransacao = jQuery.grep(gridBaixaCad, function (titulo) {
        if (titulo.Titulo.cd_tipo_financeiro != CHEQUEPREDATADO)
            return true;
    });

    dijit.byId("cbLiquidacao")._onChangeActive = false;
    if(isCheque.length > 0 && isChequeTransacao.length == 0)
        dijit.byId("cbLiquidacao").set("value", CHEQUEPREDATADO);
    dijit.byId("cbLiquidacao")._onChangeActive = true;
}

var TIPO_LOCAL_CARTAO_CREDITO = 4;
var TIPO_LOCAL_CARTAO_DEBITO = 5;

var TIPO_LIQUIDACAO_CARTAO_CREDITO = 2;
var TIPO_LIQUIDACAO_CARTAO_DEBITO = 3;

var TIPO_FINANCEIRO_CARTAO = 5;
var TIPO_FINANCEIRO_CHEQUE = 4;

function sugerirLiquidacaoELocalMovtoCartao(sugestaoCartao) {

    var disabled = false;
    var compLocal = dijit.byId('cbLocalCad');
    var compTipoLiq = dijit.byId('cbLiquidacao');
    compLocal._onChangeActive = false;
    compTipoLiq._onChangeActive = false;

    if (sugestaoCartao.cd_tipo_financeiro == TIPO_FINANCEIRO_CARTAO || sugestaoCartao.cd_tipo_financeiro == TIPO_FINANCEIRO_CHEQUE) {

        //Se não foi informado o banco no local de movimento do título, 
        //o Local de movimento será o próprio local de movimento do título.
        if (hasValue(sugestaoCartao.cd_local_banco))
            compLocal.set("value", sugestaoCartao.cd_local_banco);
        else
            compLocal.set("value", sugestaoCartao.cd_local_movto);

        if (sugestaoCartao.nm_tipo_local == TIPO_LOCAL_CARTAO_CREDITO) {
            compTipoLiq.set("value", TIPO_LIQUIDACAO_CARTAO_CREDITO);
            disabled = true;
        }
        if (sugestaoCartao.nm_tipo_local == TIPO_LOCAL_CARTAO_DEBITO) {
            compTipoLiq.set("value", TIPO_LIQUIDACAO_CARTAO_DEBITO);
            disabled = true;
        }

        //compTipoLiq.set("disabled", disabled);
        //compLocal.set("disabled", disabled);
    } else
        {
            compTipoLiq.set("value", null);
            compLocal.set("value", null);

            compTipoLiq.set("disabled", false);
            //compLocal.set("disabled", false);
        }
    compLocal._onChangeActive = true;
    compTipoLiq._onChangeActive = true;
}

function calcularValorTroco() {
    var vlTotal = hasValue(dijit.byId("vlTotal").get("value")) ? dijit.byId("vlTotal").get("value") : 0;
    var vlTotalTroco = hasValue(dijit.byId("vlTotalTroco").get("value")) ? dijit.byId("vlTotalTroco").get("value") : 0;
    var vlTroco = hasValue(dijit.byId("vlTroco").get("value")) ? dijit.byId("vlTroco").get("value") : 0;
    var vlAtualizado = 0;

    if (vlTotalTroco < vlTotal) {
        dijit.byId("vlTotalTroco").set("value", "");
        dijit.byId("vlTroco").set("value", "");
    } else {
        vlAtualizado = vlTotalTroco - vlTotal;
        dijit.byId("vlTroco").set("value", vlAtualizado);
    }
}

function limparTroco() {
    if (hasValue(dijit.byId("vlTotalTroco")))
        dijit.byId("vlTotalTroco").set("value", "");
    if (hasValue(dijit.byId("vlTroco")))
        dijit.byId("vlTroco").set("value", "");
}

function atualizarVlTotal(dadosBaixa) {
    var total = 0;
    if (dadosBaixa != null && dadosBaixa.length > 0)
        for (var i = 0; i < dadosBaixa.length; i++)
            total += dadosBaixa[i].vl_liquidacao_baixa;
    unmaskFixed(dijit.byId("vlTotal").set("value", total) + "", 2);
    calcularValorTroco();
}