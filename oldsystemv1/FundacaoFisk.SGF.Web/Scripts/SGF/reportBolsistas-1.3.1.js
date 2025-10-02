//#region Monta relatório
function montarMetodosReportBolsistas() {
    require([
    "dojo/ready",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/on"
    ], function (ready, Memory, Button, JsonRest, ObjectStore, Cache, on) {
        ready(function () {
            try {
                sugereDataCorrente();
                populaMotivoBolsa();
                populaMotivoCancBolsa();
                dojo.byId("tdLabelMotvoBolsa").style.display = "";
                dojo.byId("tdMotvoBolsa").style.display = "";

                dojo.byId("tdLabelMotvoCancelBolsa").style.display = "none";
                dojo.byId("tdMotvoCancelBolsa").style.display = "none";
                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        emitirRelatorio();
                    }
                }, "relatorio");

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
                var pesAluno = document.getElementById('pesAluno');
                if (hasValue(pesAluno)) {
                    pesAluno.parentNode.style.minWidth = '18px';
                    pesAluno.parentNode.style.width = '18px';
                }
                var pesTurma = document.getElementById('pesTurma');
                if (hasValue(pesTurma)) {
                    pesTurma.parentNode.style.minWidth = '18px';
                    pesTurma.parentNode.style.width = '18px';
                }
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("cdAlunoPes").value = 0;
                        dojo.byId("noAluno").value = "";
                        apresentaMensagem('apresentadorMensagem', null);
                        dijit.byId("limparAluno").set('disabled', true);
                    }
                }, "limparAluno");
                if (hasValue(document.getElementById("limparAluno"))) {
                    document.getElementById("limparAluno").parentNode.style.minWidth = '40px';
                    document.getElementById("limparAluno").parentNode.style.width = '40px';
                }
                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true,
                    onClick: function () {
                        dojo.byId("noTurma").value = "";
                        dojo.byId("cdTurmaPes").value = 0;
                        dijit.byId("limparTurma").set('disabled', true);
                    }
                }, "limparTurma");
                if (hasValue(document.getElementById("limparTurma"))) {
                    document.getElementById("limparTurma").parentNode.style.minWidth = '40px';
                    document.getElementById("limparTurma").parentNode.style.width = '40px';
                }

                dijit.byId("ckCancelamento").on("click", function (e) {
                    if (!dijit.byId("ckCancelamento").checked) {
                        dojo.byId("tdLabelMotvoBolsa").style.display = "";
                        dojo.byId("tdMotvoBolsa").style.display = "";
                        dojo.byId("tdLabelMotvoCancelBolsa").style.display = "none";
                        dojo.byId("tdMotvoCancelBolsa").style.display = "none";
                        dijit.byId("ckDtaIni").set("checked", true);
                        dijit.byId("ckCancel").set("checked", false);
                    }
                    else {
                        dojo.byId("tdLabelMotvoBolsa").style.display = "none";
                        dojo.byId("tdMotvoBolsa").style.display = "none";
                        dojo.byId("tdLabelMotvoCancelBolsa").style.display = "";
                        dojo.byId("tdMotvoCancelBolsa").style.display = "";
                        dijit.byId("ckDtaIni").set("checked", false);
                        dijit.byId("ckCancel").set("checked", true);
                    }
                });
                adicionarAtalhoPesquisa(['percentual', 'cbMotivoBolsa', 'cbMotivoCancBolsa', 'dtInicialComunicado', 'dtFinalComunicado', 'dtInicial', 'dtFinal'], 'relatorio', ready);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323078', '765px', '771px');
                        });
                }
            } catch (e) {
                postGerarLog(e);
            }
        });
        
    });
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


function emitirRelatorio() {
    try {
        var cdMtvoBolsa = dijit.byId("cbMotivoBolsa").value > 0 ? dijit.byId("cbMotivoBolsa").value : 0;
        var cbMotivoCancBolsa = dijit.byId("cbMotivoCancBolsa").value > 0 ? dijit.byId("cbMotivoCancBolsa").value : 0;
        var cdMotivo = dojo.byId("ckCancelamento").checked ? cbMotivoCancBolsa : cdMtvoBolsa;
        var dataInicialComunicado = dojo.byId("dtInicialComunicado").value;
        var dataFinalComunicado = dojo.byId("dtFinalComunicado").value;
    
        if (!hasValue(dataFinalComunicado) && hasValue(dataInicialComunicado))
            dijit.byId('dtFinalComunicado').set('value', dataInicialComunicado);

        if (!testaPeriodo(dataInicialComunicado, dataFinalComunicado))
            return false;

        var dataInicial = dojo.byId("dtInicial").value;
        var dataFinal = dojo.byId("dtFinal").value;
    
        if (!hasValue(dataFinal) && hasValue(dataInicial))
            dijit.byId('dtFinalComunicado').set('value', dataInicial);

        if (!testaPeriodo(dataInicial, dataFinal))
            return false;
        var cdResp = 0;
        dojo.xhr.get({
            url: Endereco() + "/api/aluno/getUrlRelatorioBolsistas?cd_aluno=" + dojo.byId("cdAlunoPes").value + "&cd_turma=" + dojo.byId("cdTurmaPes").value + "&cancelamento=" + dojo.byId("ckCancelamento").checked
                            + "&per_bolsa=" + dijit.byId("percentual").value + "&cd_motivo_bolsa=" + cdMotivo
                            + "&dtIniComunicado=" + dataInicialComunicado + "&dtFimComunicado=" + dataFinalComunicado + "&dtIni=" + dataInicial + "&dtFim=" + dataFinal
                            + "&periodo_ini=" + dojo.byId("ckDtaIni").checked + "&periodo_cancel=" + dojo.byId("ckCancel").checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try{
                abrePopUp(Endereco() + '/Relatorio/RelatorioBolsistas?' + data, '1024px', '750px', 'popRelatorio');
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        postGerarLog(e);
    }

}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = RELMATRICULA;
        dijit.byId("proAluno").show();
        limparPesquisaAlunoFK();
        pesquisaAlunoFKRel();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaAlunoFKRel() {
    try{
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        var turma = hasValue(dojo.byId("noTurma").value) && dojo.byId("noTurma").value > 0 ? dojo.byId("noTurma").value : 0;

        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&origemFK=0" + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=" + turma + "&opcao=0",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), dojo.store.Memory({ idProperty: "cd_aluno" }));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });
        var gridAluno = dijit.byId("gridPesquisaAluno");
        gridAluno.itensSelecionados = [];
        gridAluno.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoRelFK() {
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
            dojo.byId("cdAlunoPes").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            dijit.byId('limparAluno').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
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
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                dijit.byId('dtInicialComunicado').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
                dijit.byId('dtFinalComunicado').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function populaMotivoBolsa() {
    var ativo = 1;
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMotivoBolsa?status=" + ativo + "&cd_motivo_bolsa=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbMotivoBolsa', 'cd_motivo_bolsa', 'dc_motivo_bolsa');
            dijit.byId("cbMotivoBolsa").set("required", false);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function populaMotivoCancBolsa() {
    var ativo = 1;
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMotivoCancelamentoBolsa?status=" + ativo + "&cd_motivo_cancelamento_bolsa=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbMotivoCancBolsa', 'cd_motivo_cancelamento_bolsa', 'dc_motivo_cancelamento_bolsa');
            dijit.byId("cbMotivoBolsa").set("required", false);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}