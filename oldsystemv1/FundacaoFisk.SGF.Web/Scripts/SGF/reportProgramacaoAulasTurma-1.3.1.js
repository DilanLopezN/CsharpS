
function montarMetodosRelatorioProgramacaoAulasTurma() {
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/on",
    "dijit/form/Button"
    ], function (ready, xhr, on, Button) {
        ready(function () {
            sugereDataCorrente();
            new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");
            new Button({
                label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                    try {
                        if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                            montarGridPesquisaTurmaFK(function () {
                                funcaoFKTurma();
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
                            funcaoFKTurma();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "pesProTurma");
            new Button({
                label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                    try {
                        limparTurmaFKPesquisa();
                        dijit.byId('dtInicial').set("disabled", false);
                        dijit.byId('dtFinal').set("disabled", false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }
            }, "limparProTurma");
            decreaseBtn(document.getElementById("limparProTurma"), '40px');
            decreaseBtn(document.getElementById("pesProTurma"), '18px');

            dijit.byId('dtInicial').on("change", function (e) {
                if (hasValue(e)) {
                    limparTurmaFKPesquisa();
                }
            });
            dijit.byId('dtFinal').on("change", function (e) {
                if (hasValue(e)) {
                    limparTurmaFKPesquisa();
                }
            });
        });
    });
}

function emitirRelatorio() {
    try {
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        if (!dijit.byId("formRelatorioProgAulasTurma").validate())
            return false;
        dojo.xhr.get({
            url: Endereco() + "/api/turma/getUrlReporProgramacaoAulasTurma?cdTurma=" + cd_turma + "&dtInicial=" + dojo.byId("dtInicial").value + "&dtFinal=" + dojo.byId("dtFinal").value +
                              "&umaTurmaPorPagina=" + document.getElementById("cKUmaTurmaPagina").checked + "&mostrarFerias=" + document.getElementById("cKMostrarFerias").checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioProgramacaoAulasTurma?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
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
                dijit.byId('dtInicial').attr("value", new Date(date.getFullYear(), (date.getMonth() - 1), date.getDate()));
                //Data Final: Data do dia
                dijit.byId('dtFinal').attr("value", date);
                showCarregando();
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagem', error);
    });
}

//Turma
function funcaoFKTurma() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = REPORTPROGAULASTURMA;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKReportProgAulasTurma() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == REPORTPROGAULASTURMA) {
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
                dojo.byId("cd_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                dojo.byId("noTurmaPesq").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparProTurma').set("disabled", false);
                dijit.byId('dtInicial').reset();
                dijit.byId('dtFinal').reset();
                dijit.byId('dtInicial').set("disabled",true);
                dijit.byId('dtFinal').set("disabled", true);
            }
            if (!valido)
                return false;
            dijit.byId("proTurmaFK").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTurmaFKPesquisa() {
    dojo.byId("cd_turma").value = 0;
    dojo.byId("noTurmaPesq").value = "";
    dijit.byId("limparProTurma").set('disabled', true);
}