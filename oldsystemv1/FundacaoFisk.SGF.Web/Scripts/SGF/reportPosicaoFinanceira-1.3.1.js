var RELATORIO_POSICAO_FINANCEIRA = 3, RECEBER=0, PAGAR=1, RECEBIDAS = 2, PAGAS = 3;
var MASCULINO = 2;
function montarMetodosPosicaoFinanceira(permissoes)
{
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/_base/array",
    "dojo/_base/xhr",
    "dojo/data/ObjectStore",
    "dojo/store/Memory",
    "dijit/registry",
    "dojo/on",
    "dojo/data/ItemFileReadStore",
    "dijit/form/Button",
    "dojox/json/ref",
    "dojo/date"
    ], function (ready, array, xhr, ObjectStore, Memory, registry, on, ItemFileReadStore, Button, ref, date) {
        ready(function () {
            try {
                loadTipoFinanceiroLiq();
                montarSituacao("situacao", Memory, registry, ItemFileReadStore);
                if(hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");
                montarMovimento("tipoMovtoPosFin");
                montarOrdemPos("ordemPosFin", RECEBER);
                loadLocalMovto();
                sugereDataCorrente();

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPessoa")))
                                montargridPesquisaPessoa(function () {
                                    abrirPessoaFK();
                                    dojo.query("#_nomePessoaFK").on("keyup", function (e) { if (e.keyCode == 13) pesquisaPessoaEscolaFK(); });
                                    dijit.byId("pesqPessoa").on("click", function (e) {
                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                        //pesquisaPessoaFK(true);
                                        pesquisaPessoaEscolaFK();
                                    });
                                });
                            else {
                                limparPesquisaPessoaFK();
                                pesquisaPessoaFK(true);
                                dijit.byId("proPessoa").show();
                                apresentaMensagem('apresentadorMensagemProPessoa', null);
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPessoaRelPos");


                dijit.byId("ckCancelamento").on("change",
                    function(e) {
                        if (dijit.byId("ckCancelamento").checked &&
                            document.getElementById("lbCkCancelamento").style.display == "") {
                            dijit.byId("ckEndereco").set("checked", true);
                            dijit.byId("tpLiquidacao").reset();
                            
                            //Adiciona o tipoCancelamento no store e o seleciona e desabilita
                            dijit.byId("tpLiquidacao").store.data.push({ id: 6, name: "Cancelamento" });
                            atualizaStoreCombo(dijit.byId("tpLiquidacao"), Memory);
                            dijit.byId("tpLiquidacao").reset();
                            dijit.byId("tpLiquidacao").set("value", 6);
                            dijit.byId("tpLiquidacao").set("disabled", true);

                        } else if ((!dijit.byId("ckCancelamento").checked) &&
                            document.getElementById("lbCkCancelamento").style.display == "") {
                            dijit.byId("ckEndereco").set("checked", false);
                            dijit.byId("tpLiquidacao").set("disabled", false);

                            //Remove a opção cancelamento do store
                            var posCancelamento = getPosicaoItemArrayById(dijit.byId("tpLiquidacao").store.data, 6); //6 -> Cancelamento
                            if (posCancelamento >= 0) {
                                dijit.byId("tpLiquidacao").store.data.splice(posCancelamento, 1);
                                atualizaStoreCombo(dijit.byId("tpLiquidacao"),Memory);
                            }

                            dijit.byId("tpLiquidacao").set("value", 0);
                        }
                    });

                dijit.byId("tipoMovtoPosFin").on("change", function (e) {
                    try {
                        if (e == RECEBIDAS) {
                            document.getElementById("trLocal").style.display = "";
                            document.getElementById("lbCkCancelamento").style.display = "";
                            document.getElementById("tdCkCancelamento").style.display = "";
                            document.getElementById("lbCkColSpanCancelamento").style.display = "";
                            if (dijit.byId("ckCancelamento").checked) {
                                dijit.byId("ckEndereco").set("checked", true);
                                dijit.byId("tpLiquidacao").reset();

                                //Adiciona o tipoCancelamento no store e o seleciona e desabilita
                                dijit.byId("tpLiquidacao").store.data.push({ id: 6, name: "Cancelamento" });
                                atualizaStoreCombo(dijit.byId("tpLiquidacao"), Memory);
                                dijit.byId("tpLiquidacao").reset();
                                dijit.byId("tpLiquidacao").set("value", 6);
                                dijit.byId("tpLiquidacao").set("disabled", true);
                            }
                        } else {
                            document.getElementById("trLocal").style.display = "none";
                            document.getElementById("lbCkCancelamento").style.display = "none";
                            document.getElementById("tdCkCancelamento").style.display = "none";
                            document.getElementById("lbCkColSpanCancelamento").style.display = "none";
                            if (dijit.byId("tpLiquidacao").disabled || dijit.byId("ckEndereco").checked) {
                                dijit.byId("ckEndereco").set("checked", false);
                                dijit.byId("tpLiquidacao").set("disabled", false);

                                //Remove a opção cancelamento do store
                                var posCancelamento = getPosicaoItemArrayById(dijit.byId("tpLiquidacao").store.data, 6); //6 -> Cancelamento
                                if (posCancelamento >= 0) {
                                    dijit.byId("tpLiquidacao").store.data.splice(posCancelamento, 1);
                                    atualizaStoreCombo(dijit.byId("tpLiquidacao"), Memory);
                                }

                                dijit.byId("tpLiquidacao").set("value", 0);
                            }
                        }


                        if (e == RECEBIDAS && dijit.byId(mostraResp).checked) {
                            document.getElementById("lbCkCPFResponsavel").style.display = "";
                            document.getElementById("tdCkCPFResponsavel").style.display = "";
                            //dijit.byId("ckCPFResponsavel").set("checked", true);
                        } else {
                            document.getElementById("lbCkCPFResponsavel").style.display = "none";
                            document.getElementById("tdCkCPFResponsavel").style.display = "none";
                            dijit.byId("ckCPFResponsavel").set("checked", false);
                        }

                        if (e == RECEBIDAS || e == RECEBER) {
                            dojo.byId("paMostrarPedagogico").style.display = "block";
                        } else {
                            //reseta os campos situacao e turma
                            dijit.byId("situacao").set('value', []);
                            dijit.byId("situacao")._updateSelection();
                            dojo.byId("noTurma").value = "";
                            dojo.byId("cdTurmaPes").value = 0;
                            dijit.byId("limparTurma").set('disabled', true);
                            apresentaMensagem('apresentadorMensagem', null);

                            dojo.byId("paMostrarPedagogico").style.display = "none";
                        }
                        if (e == RECEBIDAS || e == PAGAS) {
                            document.getElementById("trLocal").style.display = "";
                            dijit.byId("mostraDesconto").set("checked", false);
                            dijit.byId("mostraPContas").set("checked", false);

                            document.getElementById("lbMostraDesconto").style.display = "none";
                            document.getElementById("tdMostraDesconto").style.display = "none";
                            //document.getElementById("lbMostraPContas").style.display = "none";
                            //document.getElementById("tdMostraPContas").style.display = "none";

                            document.getElementById("lbDtBase").style.display = "none";
                            document.getElementById("widget_dtBase").style.display = "none";
                            document.getElementById("tdMostraCCManuais").style.display = "";
                            document.getElementById("lbMostraCCManuais").style.display = "";
                            document.getElementById("lbCkEndereco").innerHTML = "Observação:";

                            document.getElementById("lbDadosPessoais").style.display = "none";
                            document.getElementById("tdDadosPessoais").style.display = "none";
                            document.getElementById("tpLiqCol").style.display = "";
                            document.getElementById("tpLiqLabel").style.display = "";

                            document.getElementById("tdMostraRecibo").style.display = "";
                            document.getElementById("lbMostraRecibo").style.display = "";
                        }
                        else
                        {
                            document.getElementById("lbMostraDesconto").style.display = "";
                            document.getElementById("tdMostraDesconto").style.display = "";
                            //document.getElementById("lbMostraPContas").style.display = "";
                            //document.getElementById("tdMostraPContas").style.display = "";

                            document.getElementById("lbDtBase").style.display = "";
                            document.getElementById("widget_dtBase").style.display = "";
                            document.getElementById("tdMostraCCManuais").style.display = "none";
                            document.getElementById("lbMostraCCManuais").style.display = "none";
                            document.getElementById("lbCkEndereco").innerHTML = "Endereço:";

                            document.getElementById("lbDadosPessoais").style.display = "";
                            document.getElementById("tdDadosPessoais").style.display = "";
                            document.getElementById("tpLiqCol").style.display = "none";
                            document.getElementById("tpLiqLabel").style.display = "none";

                            document.getElementById("tdMostraRecibo").style.display = "none";
                            document.getElementById("lbMostraRecibo").style.display = "none";
                        }
                        montarOrdemPos("ordemPosFin", eval(e));
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("dtInicialPos").on("change", function (e) {
                    try{
                        if (hasValue(e)) {
                            var dtaIniRel = hasValue(dojo.byId("dtInicialPos").value) ? dojo.date.locale.parse(dojo.byId("dtInicialPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            var dtaFimRel = hasValue(dojo.byId("dtFinalPos").value) ? dojo.date.locale.parse(dojo.byId("dtFinalPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            if (dtaFimRel != null && dtaIniRel != null && date.compare(dtaIniRel, dtaFimRel) > 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                return false;
                            }
                            else
                                apresentaMensagem('apresentadorMensagem', null);
                        }
                    }catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("mostraResp").on("change", function (e) {
                    if (dijit.byId("tipoMovtoPosFin").value == RECEBIDAS && e) {
                        document.getElementById("lbCkCPFResponsavel").style.display = "";
                        document.getElementById("tdCkCPFResponsavel").style.display = "";
                        //dijit.byId("ckCPFResponsavel").set("checked", true);
                    } else {
                        document.getElementById("lbCkCPFResponsavel").style.display = "none";
                        document.getElementById("tdCkCPFResponsavel").style.display = "none";
                        dijit.byId("ckCPFResponsavel").set("checked", false);
                    }
                });

                //dijit.byId("ordemPosFin").on("change", function (e) {
                //    try {
                //        if (dijit.byId("ordemPosFin").value == 2) {
                //            document.getElementById("lbMostraPContas").style.display = "none";
                //            document.getElementById("tdMostraPContas").style.display = "none";
                //        }
                //        else {
                //            document.getElementById("lbMostraPContas").style.display = "";
                //            document.getElementById("tdMostraPContas").style.display = "";
                //        }
                //    } catch (e) {
                //        postGerarLog(e);
                //    }
                //});

                dijit.byId("dtFinalPos").on("change", function (e) {
                    try{
                        if (hasValue(e)) {
                            var dtaIniRel = hasValue(dojo.byId("dtInicialPos").value) ? dojo.date.locale.parse(dojo.byId("dtInicialPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            var dtaFimRel = hasValue(dojo.byId("dtFinalPos").value) ? dojo.date.locale.parse(dojo.byId("dtFinalPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            if (dtaFimRel != null && dtaIniRel != null && date.compare(dtaIniRel, dtaFimRel) > 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                return false;
                            }
                            else
                                apresentaMensagem('apresentadorMensagem', null);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("dtBase").on("change", function (e) {
                    try{
                        if (hasValue(e)) {
                            var dtaIniRel = hasValue(dojo.byId("dtInicialPos").value) ? dojo.date.locale.parse(dojo.byId("dtInicialPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            var dtaFimRel = hasValue(dojo.byId("dtFinalPos").value) ? dojo.date.locale.parse(dojo.byId("dtFinalPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            var dtaBase = hasValue(dojo.byId("dtBase").value) ? dojo.date.locale.parse(dojo.byId("dtBase").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                            if (dtaBase != null && dtaIniRel != null && date.compare(dtaIniRel, dtaBase) > 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialBase);
                                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                                return false;
                            }
                            else
                                apresentaMensagem('apresentadorMensagem', null);
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            if (!hasValue(dijit.byId("gridPesquisaPlanoContas")))
                                montarFKPlanoContas(
                                    function () { funcaoFKPlanoContas(xhr, ObjectStore, Memory, false); },
                                    'apresentadorMensagem',
                                    RELATORIO_POSICAO_FINANCEIRA);
                            else {
                                funcaoFKPlanoContas(xhr, ObjectStore, Memory, true, RELATORIO_POSICAO_FINANCEIRA);
                                clearForm("formPlanoContasFK");
                            }
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesPlanoRelPos");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try{
                            dijit.byId("limparPessoaRelPosPes").set('disabled', true);
                            dojo.byId("cdPessoaPesRel").value = 0;
                            dijit.byId("noPessoaRelPos").set("value", "");
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                }, "limparPessoaRelPosPes");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        try{
                            dijit.byId("limparPlanoRelPosPes").set('disabled', true);
                            dojo.byId("cdPlanoPesRel").value = 0;
                            dijit.byId("noPlanoRelPos").set("value", "");
                        } catch (e) {
                            postGerarLog(e);
                        }
                    },
                }, "limparPlanoRelPosPes");


                if (hasValue(document.getElementById("limparPessoaRelPosPes"))) {
                    document.getElementById("limparPessoaRelPosPes").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPessoaRelPosPes").parentNode.style.width = '40px';
                }


                if (hasValue(document.getElementById("limparPlanoRelPosPes"))) {
                    document.getElementById("limparPlanoRelPosPes").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPlanoRelPosPes").parentNode.style.width = '40px';
                }
                var pesPlano = document.getElementById('pesPlanoRelPos');
                if (hasValue(pesPlano)) {
                    pesPlano.parentNode.style.minWidth = '18px';
                    pesPlano.parentNode.style.width = '18px';
                }

                var pesPessoa = document.getElementById('pesPessoaRelPos');
                if (hasValue(pesPessoa)) {
                    pesPessoa.parentNode.style.minWidth = '18px';
                    pesPessoa.parentNode.style.width = '18px';
                }
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323080', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['tipoMovtoPosFin', 'ordemPosFin', 'dtInicialPos', 'dtFinalPos', 'dtBase'], 'relatorio', ready);

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

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("noTurma").value = "";
                        dojo.byId("cdTurmaPes").value = 0;
                        dijit.byId("limparTurma").set('disabled', true);
                        apresentaMensagem('apresentadorMensagem', null);
                    }
                }, "limparTurma");

                decreaseBtn(document.getElementById("pesTurma"), '18px');
                decreaseBtn(document.getElementById("limparTurma"), '40px');

                setMenssageMultiSelectOpcao(SITUACAO, 'situacao', true, 0);
                dijit.byId("situacao").on("change", function (e) {
                    setMenssageMultiSelectOpcao(SITUACAO, 'situacao', true, 0);
                });
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadLocalMovto() {
    showCarregando();
    require(["dojo/_base/xhr"], function (xhr) {
        xhr.get({
            url: Endereco() + "/api/financeiro/getLocalMovtoWithContaCCByEscola",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataMovto) {
            try {
                //showCarregando();
                var dados = dataMovto.retorno;
                if (dados != null) {
                    apresentaMensagem("apresentadorMensagem", null);
                    loadMovto(dados.localMovimento, 'localMovto');
                }
                hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error);
            });
    });
}

function loadMovto(items, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var itemsCb = [];
                var cbMovto = dijit.byId(field);
                Array.forEach(items, function (value, i) {
                    itemsCb.push({
                        id: value.cd_local_movto,
                        name: value.nomeLocal == null ? value.no_local_movto : value.nomeLocal
                    });
                });
                var stateStore = new Memory({
                    data: itemsCb
                });
                cbMovto.store = stateStore;
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}



function atualizaStoreCombo(field, Memory) {
    var statusStore = new Memory({
        data: field.store.data
    });
    field.set("store",statusStore);
}

function getPosicaoItemArrayById(array, id) {
    try {
        //dijit.byId("tpLiquidacao").store.data
       return array.map(function (item, index, array) {
            return item.id;
       }).indexOf(id);

    } catch (e) {
        postGerarLog(e);
    } 
}

function abrirTurmaFK() {
    try {
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = RELMATRICULA;
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
        dojo.byId("idOrigemCadastro").value = RELMATRICULA;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFK();

    } catch (e) {
        postGerarLog(e);
    }
}
function retornarTurmaFKRelMatricula() {
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
            dojo.byId("noTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dojo.byId("cdTurmaPes").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId('limparTurma').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


function montarparametrosmulti(componente) {
    var produtos = "";
    if (!hasValue(dijit.byId(componente).value) || dijit.byId(componente).value.length <= 0) {
        produtos = "100";
    } else {

        if (dijit.byId(componente).value[0] == "0") {
            produtos = "0"
        } else {
            for (var i = 0; i < dijit.byId(componente).value.length; i++) {
                if (produtos == "") {
                    produtos = dijit.byId(componente).value[i];
                } else {
                    produtos = produtos + "|" + dijit.byId(componente).value[i];
                }
            }
        }
    }

    return produtos;
}


function pesquisaPessoaEscolaFK() {
    require([
     "dojo/store/JsonRest",
     "dojo/data/ObjectStore",
     "dojo/store/Cache",
     "dojo/store/Memory",
     "dojo/ready"],
 function (JsonRest, ObjectStore, Cache, Memory, ready) {
     ready(function () {
         try {
             var myStore = Cache(
                  JsonRest({
                      target: Endereco() + "/api/aluno/getAllPessoaSomenteEscola?nome=" + dojo.byId("_nomePessoaFK").value +
                                    "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked +
                                    "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                    "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                    "&sexo=" + dijit.byId("sexoPessoaFK").value + "&papel=",
                      handleAs: "json",
                      headers: { "Accept": "application/json", "Authorization": Token() }
                  }), Memory({}));

             dataStore = new ObjectStore({ objectStore: myStore });
             var grid = dijit.byId("gridPesquisaPessoa");
             grid.setStore(dataStore);
         } catch (e) {
             postGerarLog(e);
         }
     });
 });
}

function emitirRelatorio() {
    try{
        if (!dijit.byId("formRptPosicaoFinanceira").validate())
            return false;
    
        apresentaMensagem('apresentadorMensagem', null);
        var pNatureza = dijit.byId("tipoMovtoPosFin").value == 0 || dijit.byId("tipoMovtoPosFin").value == 2 ? 1 : 2;
        var fornecedor = hasValue(dojo.byId("cdPessoaPesRel").value) ? dojo.byId("cdPessoaPesRel").value : 0;
        var planoContas = hasValue(dojo.byId("cdPlanoPesRel").value) ? dojo.byId("cdPlanoPesRel").value : 0;

        var dtaIniRel = hasValue(dojo.byId("dtInicialPos").value) ? dojo.date.locale.parse(dojo.byId("dtInicialPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dtaFimRel = hasValue(dojo.byId("dtFinalPos").value) ? dojo.date.locale.parse(dojo.byId("dtFinalPos").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dtaBase = hasValue(dojo.byId("dtBase").value) ? dojo.date.locale.parse(dojo.byId("dtBase").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var situacoes = montarparametrosmulti('situacao');
        var cd_turma = (dojo.byId('cdTurmaPes').value != null) ? dojo.byId('cdTurmaPes').value : 0;
        var no_turma = (dojo.byId("noTurma").value != null) ? dojo.byId("noTurma").value : "";

        require(["dojo/date", "dojo/_base/xhr"], function (date, xhr) {
            //Verifica se a data inicial é menor ou igual a data final:
            if (dtaFimRel != null && dtaIniRel !=null && date.compare(dtaIniRel, dtaFimRel) > 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }
            else
                apresentaMensagem('apresentadorMensagem', null);

            if (dtaBase != null && dtaIniRel == null)
                dtaIniRel = new Date(new Date().getFullYear(), new Date().getMonth() -1, new Date().getDate());
            if (dtaBase != null && dtaIniRel != null && (date.compare(dtaIniRel, dtaBase) > 0 && dijit.byId("tipoMovtoPosFin").value != 3 && dijit.byId("tipoMovtoPosFin").value != 2)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialBase);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }
            else
                apresentaMensagem('apresentadorMensagem', null);
            var nomeEndereco = "/api/financeiro/getUrlRelatorioPosicaoFinanceira?pDtaI=";
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            var tipoLiquidacao = hasValue(dijit.byId('tpLiquidacao')) ? dijit.byId('tpLiquidacao').value : -1;
            var cdLocal = hasValue(dijit.byId('localMovto')) ? dijit.byId('localMovto').value : 0;
            var ckCancelamento = (document.getElementById("lbCkCancelamento").style.display == "")
                ? dijit.byId("ckCancelamento").checked
                : null;
            if (Permissoes != null && Permissoes != "" && possuiPermissao('ctsg', Permissoes))
                nomeEndereco = "/api/financeiro/getUrlRelatorioPosicaoFinanceiraGeral?pDtaI="
            xhr.get({
                url: Endereco() + nomeEndereco + dojo.byId("dtInicialPos").value + "&pDtaF=" + dojo.byId("dtFinalPos").value + "&pForn=" + fornecedor +
                                "&pDtaBase=" + dojo.byId("dtBase").value + "&pNatureza=" + pNatureza + "&pPlanoContas=" + planoContas + "&tpMovto=" + dijit.byId("tipoMovtoPosFin").value +
                                "&ordem=" + dijit.byId("ordemPosFin").value + "&pAnalitico=" + dijit.byId("tpAnalitico").checked + "&pMostraResp=" +
                                dijit.byId("mostraResp").checked + "&pMostraDados=" + dijit.byId("mostraDados").checked + "&pMostraEndereco=" + dijit.byId("ckEndereco").checked +
                                "&pMostraDesconto=" + dijit.byId("mostraDesconto").checked + "&pMostraContas=" + dijit.byId("mostraPContas").checked + "&pMostraCCManual="+dijit.byId('mostraCCManuais').checked +
                                "&cdTpLiq=" + tipoLiquidacao + "&cdTpFinan=" + dijit.byId('tpFinanceiro').value + "&pMostarRecibo=" + dijit.byId('mostraRecibo').checked +
                                "&pMostrarCpfResponsavel=" + dijit.byId('ckCPFResponsavel').checked + "&pSituacoes=" + situacoes + "&pCdTurma=" + cd_turma + "&pNoTurma=" + no_turma + "&pCancelamento=" + ckCancelamento +
                                "&cdLocal=" + cdLocal,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try{
                    abrePopUp(Endereco() + '/Relatorio/RelatorioPosicaoFinanceira?' + data, '1024px', '750px', 'popRelatorio');
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function montarMovimento(nomElement) {
    require(["dojo/store/Memory"],
     function (Memory) {
         try{
             var dados = [{ name: "Contas a Receber", id: "0" },
                          { name: "Contas a Pagar", id: "1" },
                          { name: "Contas Recebidas", id: "2" },
                          { name: "Contas Pagas", id: "3" }
             ]
             var statusStore = new Memory({
                 data: dados
             });
             dijit.byId(nomElement).store = statusStore;
             dijit.byId(nomElement).set("value", 0);
         } catch (e) {
             postGerarLog(e);
         }
     });
};

function montarOrdemPos(nomElement, tipo) {
    require(["dojo/store/Memory"],
     function (Memory) {
         try{
             var dados = [{ name: "Vencimento", id: "0" },
                          { name: "Cliente", id: "1" },
                          { name: "Plano de Contas", id: "2" }
             ];
             dojo.byId('labelPessoa').innerHTML = 'Cliente:';

             switch (tipo) {
                 case PAGAR:
                     dados = [{ name: "Vencimento", id: "0" },
                         { name: "Fornecedor", id: "1" },
                         { name: "Plano de Contas", id: "2" }
                     ];
                     dojo.byId('labelPessoa').innerHTML = 'Fornecedor:';
                     break;
                 case RECEBIDAS:
                     dados = [{ name: "Recebimento", id: "0" },
                         { name: "Cliente", id: "1" },
                         { name: "Plano de Contas", id: "2" }
                     ];
                     dojo.byId('labelPessoa').innerHTML = 'Cliente:';
                     break;
                 case PAGAS:
                     dados = [{ name: "Pagamento", id: "0" },
                         { name: "Fornecedor", id: "1" },
                         { name: "Plano de Contas", id: "2" }
                     ];
                     dojo.byId('labelPessoa').innerHTML = 'Fornecedor:';
             }

             var statusStore = new Memory({
                 data: dados
             });
             dijit.byId(nomElement).store = statusStore;
             dijit.byId(nomElement).set("value", 0);
         } catch (e) {
             postGerarLog(e);
         }
     });
};

function abrirPessoaFK() {
    try{
        limparPesquisaPessoaFK();
        //pesquisaPessoaFK(true);
        pesquisaPessoaEscolaFK();
        dijit.byId("proPessoa").show();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoa() {
    try{
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoa");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0 || gridPessoaSelec.itensSelecionados.length > 1) {
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPessoaSelec.itensSelecionados != null && gridPessoaSelec.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            var value = gridPessoaSelec.itensSelecionados;
            dojo.byId("cdPessoaPesRel").value = value[0].cd_pessoa;
            dojo.byId("noPessoaRelPos").value = value[0].no_pessoa;
            dijit.byId("limparPessoaRelPosPes").set('disabled', false);
        }

        if (!valido)
            return false;
        dijit.byId("proPessoa").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function funcaoFKPlanoContas(xhr, ObjectStore, Memory, load, tipoRetorno) {
    try{
        if (load)
            loadPlanoContas(ObjectStore, Memory, xhr, tipoRetorno);

        dijit.byId("cadPlanoContas").show();
        apresentaMensagem('apresentadorMensagemPlanoContasFK', null);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoaRel() {
    try{
        var gridPesPessoaRel = dijit.byId("gridPesquisaPessoaRel");
        if (!hasValue(gridPesPessoaRel.itensSelecionados) || gridPesPessoaRel.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (gridPesPessoaRel.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
        else {
            dojo.byId("cdPessoaPesRel").value = gridPesPessoaRel.itensSelecionados[0].cd_pessoa;
            dojo.byId("noPessoaRelPos").value = gridPesPessoaRel.itensSelecionados[0].no_pessoa;
        }
        dijit.byId("proPessoaRel").hide();
    } catch (e) {
        postGerarLog(e);
    }
}


function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try{
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicialPos').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinalPos').attr("value", date);
                //Data Base: Data do dia
                dijit.byId('dtBase').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}


function loadTipoFinanceiroLiq() {
    dojo.xhr.get({
        url: Endereco() + "/api/financeiro/getTipoLiquidacaoFinanceiro",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (data != null) {
                if (hasValue(data.tiposLiquidacao))
                    criarOuCarregarCompFiltering("tpLiquidacao", data.tiposLiquidacao, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_liquidacao', 'dc_tipo_liquidacao', MASCULINO);
                if (hasValue(data.tiposFinanceiro))
                    criarOuCarregarCompFiltering("tpFinanceiro", data.tiposFinanceiro, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_tipo_financeiro', 'dc_tipo_financeiro', MASCULINO);
                  
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
        showCarregando();
    });
}

function montarSituacao(nomelement, Memory, registry, ItemFileReadStore) {
    try {
        var w = registry.byId(nomelement);
        var dados = [
            { name: "Aguardando Matrícula", id: "9" },
            { name: "Matriculado", id: "1" },
            { name: "Rematriculado", id: "8" },
            { name: "Desistente", id: "2" },
            { name: "Encerrado", id: "4" },
            { name: "Movido", id: "0" }
        ]
        var store = new ItemFileReadStore({
            data: {
                identifier: "id",
                label: "name",
                items: dados
            }
        });
        w.setStore(store);
    } catch (e) {
        postGerarLog(e);
    }
}