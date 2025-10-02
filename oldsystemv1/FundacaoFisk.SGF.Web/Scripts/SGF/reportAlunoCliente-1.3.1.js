//#region Monta relatório
var TODOS = 0, SITUACAO = 3;
function montarMetodosReportAlunoCliente() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/data/ObjectStore",
    "dojo/store/Memory",
    "dijit/registry",
    "dojo/data/ItemFileReadStore",
    "dijit/form/Button",
    "dojo/on",
    "dijit/form/FilteringSelect",
    ], function (ready, xhr, ObjectStore, Memory, registry, ItemFileReadStore, Button, on, FilteringSelect) {
        ready(function () {
            try{
                montarStatus("statusAluno");
                montarSituacaoAlunoTurma("situacao", Memory, registry, ItemFileReadStore);
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try{
                            limparPesquisaPessoaRelFK();
                            dijit.byId("proPessoaRel").show();
                            if (hasValue(dijit.byId("gridPesquisaPessoaRel")))
                                dijit.byId("gridPesquisaPessoaRel").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                            pesquisaPessoaResponsavelFK();
                            if (hasValue(dijit.byId("pesqPessoaRel")))
                                dijit.byId("pesqPessoaRel").on("click", function (e) {
                                    pesquisaPessoaResponsavelFK();
                                });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesResp");

                var pesResp = document.getElementById('pesResp');
                if (hasValue(pesResp)) {
                    pesResp.parentNode.style.minWidth = '18px';
                    pesResp.parentNode.style.width = '18px';
                }

                new Button({
                    label: "Limpar", iconClass: '', disabled: true, type: "reset", onClick: function () {
                        try{
                            dijit.byId('noResp').reset();
                            dijit.byId("noResp").value = 0;
                            dojo.byId("noResp").value = "";
                            dijit.byId('limparResp').set("disabled", true);
                            apresentaMensagem('apresentadorMensagem', null);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparResp");

                if (hasValue(document.getElementById("limparResp"))) {
                    document.getElementById("limparResp").parentNode.style.minWidth = '40px';
                    document.getElementById("limparResp").parentNode.style.width = '40px';
                }

                dijit.byId('dtInicial').on("change", function (e) {
                    try{
                        var dtFinal = dijit.byId('dtFinal').get('value');
                        var retorno = testaPeriodo(e, dtFinal);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId('dtFinal').on("change", function (e) {
                    try{
                        var dtInicial = dijit.byId('dtInicial').get('value');
                        var retorno = testaPeriodo(dtInicial, e);
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                $('#fonePesq').focusout(function () {
                    var phone, element;
                    element = $(this);
                    element.unmask();
                    phone = element.val().replace(/\D/g, '');
                    if (phone.length > 10) {
                        element.mask("(99) 99999-999?9");
                    } else {
                        element.mask("(99) 9999-9999?9");
                    }
                }).trigger('focusout');

                adicionarAtalhoPesquisa(['_nome', 'noResp', 'fonePesq', 'statusAluno', 'emailPesq', 'dtInicial', 'dtFinal'], 'relatorio', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323077', '765px', '771px');
                        });
                }
                dijit.byId("compComoConheceu").on("change", function (cd_tipo) {
                    try {
                        if (!hasValue(cd_tipo) || cd_tipo < 0)
                            dijit.byId("compComoConheceu").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                setMenssageMultiSelectOpcao(SITUACAO, 'situacao', true, 0);
                dijit.byId("situacao").on("change", function (e) {
                    setMenssageMultiSelectOpcao(SITUACAO, 'situacao', true, 0);
                });
                findIsLoadComponetesRelatorio(xhr, ready, Memory, FilteringSelect);

            } catch (e) {
                postGerarLog(e);
            }
        });
        
    });
}

function findIsLoadComponetesRelatorio(xhr, ready, Memory, FilteringSelect) {
    xhr.get({
        url: Endereco() + "/api/secretaria/getComponentesRelatorioAlunoCliente",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var data = jQuery.parseJSON(data).retorno;
            if (data != null && data.midias != null & data.midias.length > 0)
                criarOuCarregarCompFiltering("compComoConheceu", data.midias, "", null, ready, Memory, FilteringSelect, 'cd_midia', 'no_midia', MASCULINO);
            showCarregando();
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagemMovto", error);
    });
}

function pesquisaPessoaResponsavelFK() {
        require(["dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory", "dojo/domReady!", "dojo/parser"],
        function (JsonRest, ObjectStore, Cache, Memory) {
            try{
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/GetPessoaPorEscolaSearch?nome=" + encodeURIComponent(dojo.byId("_nomePessoaRelFK").value) + "&apelido=" + encodeURIComponent(dojo.byId("_apelidoRel").value) + "&tipoPessoa=" + parseInt(dijit.byId("tipoPessoaRelFK").value) + "&cnpjCpf=" + dojo.byId("CnpjCpfPessoaRel").value + "&papel=" + parseInt(dijit.byId("papelPessoaRelFK").value) + "&sexo=" + parseInt(dijit.byId("sexoPessoaRelFK").value) + "&inicio=" + dijit.byId("inicioPessoaRelFK").checked,
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                var dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoaRel");
                grid.setStore(dataStore);
            } catch (e) {
                postGerarLog(e);
            }
        })
}

function testaPeriodo(dataInicial, dataFinal) {
    try{
        var retorno = true;
        if (dojo.date.compare(dataInicial, dataFinal) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
            retorno = false;
        } else
            apresentaMensagem('apresentadorMensagem', "");
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function retornarPessoaRel() {
    try{
        var gridPesPessoaRel = dijit.byId("gridPesquisaPessoaRel");
        if (!hasValue(gridPesPessoaRel.itensSelecionados) || gridPesPessoaRel.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesPessoaRel.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro.', null);
            return false;
        }
        else {
            //TODO Melhorar esse código  HAS_RESPONSAVEL_TIT?? karol  não é muito usual setar na mesma variavél, seria bom criar uma para cada responsabilidade.
            dijit.byId("noResp").value = gridPesPessoaRel.itensSelecionados[0].cd_pessoa;
            dojo.byId("noResp").value = gridPesPessoaRel.itensSelecionados[0].no_pessoa;
            dijit.byId('limparResp').set("disabled", false);
        }
        dijit.byId("proPessoaRel").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function emitirRelatorio() {
    try{
        var dataInicial = dojo.byId("dtInicial").value;// hasValue(dojo.byId("dtInicial").value) ? dojo.date.locale.parse(dojo.byId("dtInicial").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dataFinal = dojo.byId("dtFinal").value;// hasValue(dojo.byId("dtFinal").value) ? dojo.date.locale.parse(dojo.byId("dtFinal").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
    
        if (!hasValue(dataFinal) && hasValue(dataInicial))
            dijit.byId('dtFinal').set('value', dataInicial);

        if (!testaPeriodo(dataInicial, dataFinal))
            return false;
        var cdResp = 0;
        if (dijit.byId("noResp").value > 0)
            cdResp = dijit.byId("noResp").value;
        var situacoes = "";
        if (!hasValue(dijit.byId('situacao').value) || dijit.byId('situacao').value.length <= 0)
            situacoes = "100";
        else
            for (var i = 0; i < dijit.byId('situacao').value.length; i++) {
                if (situacoes == "")
                    situacoes = dijit.byId('situacao').value[i];
                else
                    situacoes = situacoes + "|" + dijit.byId('situacao').value[i];
            }
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/aluno/GetUrlRelAlunoCliente?nome=" + dojo.byId("_nome").value + "&cdResp=" + cdResp + "&telefone=" + dojo.byId("fonePesq").value +
                                               "&email=" + dojo.byId("emailPesq").value + "&status=" + retornaStatus("statusAluno") + "&dtaIni=" + dataInicial + "&dtaFinal=" + dataFinal +
                                               "&cd_midia=" + dijit.byId("compComoConheceu").value + "&situacaoAlunoTurma=" + situacoes + "&exibirEnderecos=" + dijit.byId("ckExibirEnderecos").checked,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioAlunoCliente?' + data, '1000px', '750px', 'popRelatorio');  //Era RelatorioDinamico
            },
            function (error) {
                apresentaMensagem('apresentadorMensagem', error);
            });
        })
    } catch (e) {
        postGerarLog(e);
    }

}
function montarSituacaoAlunoTurma(nomelement, Memory, registry, ItemFileReadStore) {
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
