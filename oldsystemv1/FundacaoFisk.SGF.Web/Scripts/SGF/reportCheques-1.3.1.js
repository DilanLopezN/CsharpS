var TODOS = 0;
function montarMetodosRelatorioCheques() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dijit/form/FilteringSelect",
    ], function (ready, xhr, Memory, on, Button, FilteringSelect) {
        ready(function () {
            try {
                findComponentesFiltroCheques();
                sugereDataCorrente();
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioCheques");
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
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                        try {
                            dojo.byId("cd_aluno").value = 0;
                            dojo.byId("noAluno").value = "";
                            apresentaMensagem('apresentadorMensagem', null);
                            dijit.byId("limparAlunoFK").set('disabled', true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAlunoFK");
                decreaseBtn(document.getElementById("limparAlunoFK"), '40px');
                decreaseBtn(document.getElementById("pesAluno"), '18px');
                dijit.byId("ckLiquidadados").on("change", function (e) {
                    if (!e)
                        dijit.byId("ckLiquidacao").reset();
                    else {
                        dijit.byId("ckLiquidacao").set("checked", true);
                        dijit.byId("ckEmissao").set("checked", false);
                    }
                    dijit.byId("ckLiquidacao").set("disabled", !e);
                    if (!e)
                        dijit.byId("ckEmissao").set("checked", true)
                });
                loadNatureza(Memory);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadNatureza(Memory) {
    var statusStoreTipo = new Memory({
        data: [
        { name: "Receber", id: 1 },
        { name: "Pagar", id: 2 }
        ]
    });
    dijit.byId("cbNatureza").store = statusStoreTipo;
    dijit.byId("cbNatureza").set("value", 1);
    dijit.byId("cbNatureza").on("change", function (e) {
        switch (e) {
            case 1:
                dojo.byId('lblAluno').innerHTML = 'Aluno:';
                break
            case 2:
                dojo.byId('lblAluno').innerHTML = 'Fornecedor:';
                break
            default:
                dijit.byId("cbNatureza").set("value", 1);
        }
    });
}

function emitirRelatorio() {
    try {
        var banco = hasValue(dijit.byId("bancos").value) ? dijit.byId("bancos").value : 0;
        var emitente = hasValue(dijit.byId("emitente").value) ? dijit.byId("emitente").value : "";
        var nm_cheque = hasValue(dijit.byId("nm_cheque").value) ? dijit.byId("nm_cheque").value : "";
        var vl_cheque = hasValue(dijit.byId("vl_cheque").value) ? maskFixed(dijit.byId("vl_cheque").value,2) : 0;
        var nm_agencia = hasValue(dijit.byId("nmAgencia").value) ? dijit.byId("nmAgencia").value : 0;
        var nm_ccorrente = hasValue(dijit.byId("nmContaCorrente").value) ? dijit.byId("nmContaCorrente").value : 0;
        var cd_pessoa_aluno = hasValue(dojo.byId("cd_aluno").value) ? dojo.byId("cd_aluno").value : 0;
        //if (!dijit.byId("formPesquisaRelatorioProspect").validate())
        //    return false;
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getUrlRelatorioCheques?cd_pessoa_aluno=" + cd_pessoa_aluno + "&cd_banco=" + banco + "&emitente=" + emitente + "&liquidados=" +
                document.getElementById("ckLiquidadados").checked + "&nm_cheques=" + nm_cheque + "&vl_titulo=" + vl_cheque + "&nm_agencia=" + nm_agencia + "&nm_ccorrente=" + nm_ccorrente +
                "&dt_ini_bPara=" +  dojo.byId("dtInicialBP").value + "&dt_fim_bPara=" +  dojo.byId("dtFinalBP").value + 
                "&dt_ini=" + dojo.byId("dtInicialDatas").value + "&dt_fim=" + dojo.byId("dtFinalDatas").value + "&emissao=" + document.getElementById("ckEmissao").checked + 
                "&liquidacao=" + document.getElementById("ckLiquidacao").checked + "&natureza=" + dijit.byId("cbNatureza").value + "&displayBanco=" + dijit.byId("bancos").displayedValue,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioCheques?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = RELCHEQUES;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisarAlunoFK(true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoFK() {
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
            dojo.byId("cd_aluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAlunoFK').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function findComponentesFiltroCheques() {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/financeiro/getBancosTituloCheque",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                if (data != null)
                    criarOuCarregarCompFiltering("bancos", data, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_banco', 'no_banco', MASCULINO);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function sugereDataCorrente() {
    dojo.xhr.post({
        url: Endereco() + "/util/PostDataHoraCorrente",
        preventCache: true,
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dataCorrente = jQuery.parseJSON(data).retorno;
            var dataSugerida = dataCorrente.dataPortugues.split(" ");
            if (dataSugerida.length > 0) {
                var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                //Data Inicial: Um mês antes à data do dia
                dijit.byId('dtInicialDatas').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinalDatas').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}