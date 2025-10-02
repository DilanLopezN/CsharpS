var TODOS = 0;
var CLIENTES = 4, FUNCIONARIOS = 5;
function montarMetodosRelatorioListagemAniversariantes() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojo/window"
    ], function (ready, xhr, Memory, on, Button, window) {
        ready(function () {
            try {
                montarTipos("tipoListagem", Memory);
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(window); } }, "relatorioListagem");
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
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparProTurma");
                decreaseBtn(document.getElementById("limparProTurma"), '40px');
                decreaseBtn(document.getElementById("pesProTurma"), '18px');
                //if (hasValue(dijit.byId("menuManual"))) {
                //dijit.byId("menuManual").on("click", function (e) {
                //    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323076', '765px', '771px');
                //});}
                dijit.byId("tipoListagem").on("change", function (cd_tipo) {
                    try {
                        if (!hasValue(cd_tipo) || cd_tipo < 0)
                            dijit.byId("tipoListagem").set("value", TODOS);
                        else {
                            if (hasValue(dijit.byId("tipoListagem").oldValue) && (dijit.byId("tipoListagem").oldValue != CLIENTES && dijit.byId("tipoListagem").oldValue != FUNCIONARIOS))
                                apresentaMensagem('apresentadorMensagem', null);
                            dijit.byId("tipoListagem").oldValue = cd_tipo;
                            verificarTipoFiltros();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function emitirRelatorio(windowUtils) {
    try {
        var compMes = dijit.byId("nmMes");
        var compDia = dijit.byId("nmDia");
        var cd_tipo = hasValue(dijit.byId("tipoListagem").value) ? dijit.byId("tipoListagem").value : 0;
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var mes = hasValue(compMes.value) ? compMes.value : 0;
        var dia = hasValue(compDia.value) ? compDia.value : 0;
        if (mes > 0 && !compMes.validate()) {
            mostrarMensagemCampoValidado(windowUtils, compMes);
            return false;
        }
        if (dia > 0 && !compDia.validate()) {
            mostrarMensagemCampoValidado(windowUtils, compDia);
            return false;
        }
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getUrlRelatorioListagemAniversariantes?tipo=" + cd_tipo + "&cd_turma=" + cd_turma + "&mes=" + mes +
                               "&dia=" + dia,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioListagemAniversariantes?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Turma
function funcaoFKTurma() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("trAluno").style.display = "";
        dojo.byId("idOrigemCadastro").value = REPORTLISTAGEMANIVERSARIANTES;
        dijit.byId("proTurmaFK").show();
        pesquisarTurmaFK();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == REPORTLISTAGEMANIVERSARIANTES) {
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
                verificarTipoFiltros();
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

function montarTipos(nomElement, Memory) {
    try {
        var dados = [{ name: "Todos", id: "0" },
                     { name: "Alunos ativos(atuais)", id: "1" },
                     { name: "Alunos Desistentes", id: "2" },
                     { name: "Ex Alunos", id: "3" },
                     { name: "Clientes", id: "4" },
                     { name: "Funcionários/Professor", id: "5" },
        ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId(nomElement).store = statusStore;
        dijit.byId(nomElement).set("value", TODOS);
        dijit.byId(nomElement).oldValue = TODOS;
    } catch (e) {
        postGerarLog(e);
    }
}

function verificarTipoFiltros() {
    if (hasValue(dijit.byId("tipoListagem")) && dijit.byId("tipoListagem").value > 0 && hasValue(dojo.byId("cd_turma").value) && dojo.byId("cd_turma").value > 0) {
        var tipo = parseInt(dijit.byId("tipoListagem").value);
        switch (tipo) {
            case CLIENTES:
            case FUNCIONARIOS:
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroFiltroReportListAnivers);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                dijit.byId("tipoListagem").reset();
                break;
        }
    }
}