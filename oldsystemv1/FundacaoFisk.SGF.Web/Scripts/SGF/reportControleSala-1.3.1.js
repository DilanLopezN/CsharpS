var DOMINGO = 1, SEGUNDA = 2, TERCA = 3, QUARTA = 4, QUINTA = 5, SEXTA = 6, SABADO = 7;

function montarMetodosRelatorioControleSala() {
    require([
"dojo/ready",
"dojo/store/Memory",
"dojo/on",
"dijit/form/Button"
    ], function (ready, Memory, on, Button) {
        ready(function () {
            try {
                maskHour('#hIni');
                maskHour('#hFim');
                findComponentesFiltros();
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioControleSala");
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
                }, "pesTurmaFK");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                        dojo.byId("noTurma").value = "";
                        dojo.byId("cd_turma").value = "";
                        dijit.byId('limparPesTurmaFK').set("disabled", true);
                    }
                }, "limparPesTurmaFK");
                decreaseBtn(document.getElementById("pesTurmaFK"), '18px');
                decreaseBtn(document.getElementById("limparPesTurmaFK"), '40px');
                //showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function emitirRelatorio() {
    try {
        apresentaMensagem('apresentadorMensagem', null);
        //if (dijit.byId('formRptDiarioAula').validate())
        require(["dojo/date", "dojo/_base/xhr"], function (date, xhr) {
            var cd_turma = hasValue(dojo.byId('cd_turma').value) ? dojo.byId('cd_turma').value : 0;
            var no_turma = hasValue(dojo.byId('cd_turma').value) ? dojo.byId('noTurma').value : "";
            var cd_professor = hasValue(dojo.byId('pesqProf').value) ? dijit.byId('pesqProf').value : 0;
            var cd_sala = hasValue(dijit.byId('pesqSala').value) ? dijit.byId('pesqSala').value : 0;
            var no_sala = hasValue(dijit.byId('pesqSala').value) ? dojo.byId('pesqSala').value : "";
            var diasSemana = montarListaDiasSemana();
            if (!verificarDiaSemanaMarcado()) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDiaSemanaSelect);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false;
            }
            xhr.get({
                url: Endereco() + "/api/turma/getUrlRelatorioControleSala?cd_pessoa_professor=" + cd_professor + "&cd_sala=" + cd_sala
                    + "&cd_turma=" + cd_turma + "&hIni=" + dojo.byId('hIni').value + "&hFim=" + dojo.byId('hFim').value + "&dias=" + diasSemana + "&no_turma=" + no_turma +
                    "&no_sala=" + no_sala,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    abrePopUp(Endereco() + '/Relatorio/RelatorioControleSala?' + data, '1024px', '750px', 'popRelatorio');
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

function findComponentesFiltros() {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/turma/componentesPesquisaControleSala",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data).retorno;
                if (data != null){
                    if (data.Professores != null && data.Professores.length > 0)
                        criarOuCarregarCompFiltering("pesqProf", data.Professores, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_fantasia', MASCULINO);
                    if (data.Salas != null && data.Salas.length > 0)
                        criarOuCarregarCompFiltering("pesqSala", data.Salas, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_sala', 'no_sala', MASCULINO);
                }
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

function montarListaDiasSemana() {
    var listaDiasSemana = "";
    if (dijit.byId("idSeg").get("checked"))
        listaDiasSemana += "," + SEGUNDA;
    if (dijit.byId("idTer").get("checked"))
        listaDiasSemana += "," + TERCA;
    if (dijit.byId("idQuarta").get("checked"))
        listaDiasSemana += "," + QUARTA;
    if (dijit.byId("idQuinta").get("checked"))
        listaDiasSemana += "," + QUINTA;
    if (dijit.byId("idSexta").get("checked"))
        listaDiasSemana += "," + SEXTA;
    if (dijit.byId("idSabado").get("checked"))
        listaDiasSemana += "," + SABADO;
    if (dijit.byId("idDomingo").get("checked"))
        listaDiasSemana += "," + DOMINGO;
    if(listaDiasSemana.length > 0)
        listaDiasSemana = listaDiasSemana.substring(1, listaDiasSemana.length);
    return listaDiasSemana;
}

function verificarDiaSemanaMarcado() {
    var marcado = false;
    if (dijit.byId("idSeg").get("checked") || dijit.byId("idTer").get("checked") || dijit.byId("idQuarta").get("checked") || dijit.byId("idQuinta").get("checked") ||
        dijit.byId("idSexta").get("checked") || dijit.byId("idSabado").get("checked") || dijit.byId("idDomingo").get("checked"))
        marcado = true;
    return marcado;
}

function funcaoFKTurma() {
    try {
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de aluno na turma
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = REPORTCONTROLESALA;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            dijit.byId('tipoTurmaFK').set('value', 1);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK();
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == REPORTCONTROLESALA) {
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
                dojo.byId("noTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                dijit.byId('limparPesTurmaFK').set("disabled", false);
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

