var RELATORIO_DIARIO_AULA_TURMAS = 1, RELATORIO_EVENTOS = 2, RELATORIO_DIARIO_AULA_PROGRAMACOES = 3;
function montarMetodosDiarioAula(permissoes)
{
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button"
    ], function (ready, Memory, on, Button) {
        ready(function () {
            try {
                showCarregando();
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");
                //habiltiarDadosAdc(false);
                montarRelatorio(Memory, on);
                //montarTipoDiario("tipoDiario", Memory, on);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () { abrePesquisaTurmaFK(); }
                }, "pesTurmaFK");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () { limpaTurmaFK();}
                }, "limparPesTurmaFK");
                
                if (hasValue(document.getElementById("limparPesTurmaFK"))) {
                    document.getElementById("limparPesTurmaFK").parentNode.style.minWidth = '40px';
                    document.getElementById("limparPesTurmaFK").parentNode.style.width = '40px';
                }

                var pesPessoa = document.getElementById('pesTurmaFK');
                if (hasValue(pesPessoa)) {
                    pesPessoa.parentNode.style.minWidth = '18px';
                    pesPessoa.parentNode.style.width = '18px';
                }

                populaProfessor('cbProfessor');
                populaEventos('cbEvento');

                dijit.byId("falta_consecultiva").on("click", function (evt) {
                    try {
                        if(dijit.byId("falta_consecultiva").checked)
                            dijit.byId('qtd_faltas').set('required', true);
                        else
                            dijit.byId('qtd_faltas').set('required', false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });

                //Este último médodo trata o showcarregando:
                sugereDataCorrente();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function emitirRelatorio() {
    try {
        if(dijit.byId('formRptDiarioAula').validate())
            require(["dojo/date", "dojo/_base/xhr"], function (date, xhr) {
                xhr.get({
                    url: Endereco() + "/api/coordenacao/getUrlRelatorioEventos?cd_turma=" + dojo.byId('cd_turma').value + "&cd_professor=" + dijit.byId('cbProfessor').value
                        + "&cd_evento=" + dijit.byId('cbEvento').value + "&qtd_faltas=" + dijit.byId('qtd_faltas').value + "&falta_consecultiva=" + dijit.byId('falta_consecultiva').checked
                        + "&mais_turma_pagina=" + dijit.byId('mais_turma_pagina').checked + "&dt_inicial=" + dojo.byId('dtInicial').value +
                        "&dt_final=" + dojo.byId('dtFinal').value + "&tipoRelatorio=" + dijit.byId('tipoRelatorio').value +
                        "&lancada=" + dijit.byId('ckLancada').checked + "&infoPresenca=" + dijit.byId('ckInfoPresenca').checked,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try{
                        abrePopUp(Endereco() + '/Relatorio/RelatorioEvento?' + data, '1024px', '750px', 'popRelatorio');
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

function abrePesquisaTurmaFK() {
    try {
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
    } catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaFK() {
    try {
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de aluno na turma
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = REPORT_DIARIO_AULA;
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

function retornarTurmaFKReportDiarioAula() {
    try {
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados))
            gridPesquisaTurmaFK.itensSelecionados = [];
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        //else LBM Liberando novamente turmas filhas 
        //    if (dijit.byId("tipoRelatorio").value == RELATORIO_DIARIO_AULA_TURMAS && gridPesquisaTurmaFK.itensSelecionados[0].cd_turma_ppt != null) {
        //    caixaDialogo(DIALOGO_AVISO, "Turmas Personalizadas só podem ser PAI", null);
        //valido = false;
        //}
        else {
            dojo.byId("noTurmaFK").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dojo.byId("cd_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId('limparPesTurmaFK').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limpaTurmaFK() {
    dojo.byId("noTurmaFK").value = "";
    dojo.byId("cd_turma").value = "";
    dijit.byId('limparPesTurmaFK').set("disabled", true);
}

function populaEventos(field) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/coordenacao/getEventos",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                loadSelect(jQuery.parseJSON(data).retorno, field, 'cd_evento', 'no_evento');
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaProfessor(field) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/professor/getAllProfessorTurma",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                loadSelect(jQuery.parseJSON(data).retorno, field, 'cd_pessoa', 'no_fantasia');
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function habiltiarDadosAdc(bool) {
    try{
        if (bool == false) {
            dojo.byId("trDadosAdc").style.display = "none";
            dojo.byId("tdLblProf").style.display = "none";
            dojo.byId("tdProf").style.display = "none";
        } else {
            dojo.byId("trDadosAdc").style.display = "";
            dojo.byId("tdLblProf").style.display = "";
            dojo.byId("tdProf").style.display = "";
        }
        dijit.byId("somentAlunosAtivo").set("disabled", !bool);
        dijit.byId("monstrarDesc").set("disabled", !bool);
        dijit.byId("monstrarAtivi").set("disabled", !bool);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarRelatorio(Memory, on) {
    try {
        var dados = [{ name: "Diário de aulas das turmas", id: RELATORIO_DIARIO_AULA_TURMAS },
                     { name: "Eventos", id: RELATORIO_EVENTOS },
                     { name: "Programação das Aulas x Diário de Aulas", id: RELATORIO_DIARIO_AULA_PROGRAMACOES }
                     //{ name: "Turmas que encerrarão - Analítico", id: "4" },
                     //{ name: "Turmas que encerrarão - Sintético", id: "5" },
                     //{ name: "Turmas - Encerradas", id: "6" },
                     //{ name: "Turmas - Atuais", id: "4" },
                     //{ name: "Turmas iniciadas no período", id: "5" },
        ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId("tipoRelatorio").store = statusStore;
        dijit.byId("tipoRelatorio").set("value", 1);
        dijit.byId("tipoRelatorio").on("change", function (e) {
            try{
                switch(e){
                    case RELATORIO_DIARIO_AULA_TURMAS: 
                        dojo.byId("trEvento").style.display = "none";
                        dojo.byId("paMostrarDetalhes").style.display = "none";
                        dijit.byId('noTurmaFK').set('required', true);
                        dijit.byId('cbProfessor').set('required', true);
                        dojo.byId("tdProf").style.display = "";
                        dojo.byId("tdLblProf").style.display = "";
                        dojo.byId("lbLancada").style.display = "none";
                        dojo.byId("tdLancada").style.display = "none";
                        dijit.byId('ckLancada').set('checked', false);
                        dojo.byId("trInfoPresenca").style.display = "";
                        dijit.byId('ckInfoPresenca').set('checked', false);
                        break;
                    case RELATORIO_EVENTOS: 
                        dojo.byId("trEvento").style.display = "";
                        dojo.byId("paMostrarDetalhes").style.display = "";
                        dijit.byId('noTurmaFK').set('required', false);
                        dijit.byId('cbProfessor').set('required', false);
                        dojo.byId("tdProf").style.display = "";
                        dojo.byId("tdLblProf").style.display = "";
                        dojo.byId("lbLancada").style.display = "none";
                        dijit.byId('ckLancada').set('checked', false);
                        dojo.byId("tdLancada").style.display = "none";
                        dojo.byId("trInfoPresenca").style.display = "none";
                        dijit.byId('ckInfoPresenca').set('checked', false);
                        break;
                    case RELATORIO_DIARIO_AULA_PROGRAMACOES:
                        dojo.byId("trEvento").style.display = "none";
                        dojo.byId("paMostrarDetalhes").style.display = "";
                        dijit.byId('noTurmaFK').set('required', false);
                        dijit.byId('cbProfessor').set('required', false);
                        dojo.byId("tdProf").style.display = "none";
                        dojo.byId("tdLblProf").style.display = "none";
                        dojo.byId("lbLancada").style.display = "";
                        dojo.byId("tdLancada").style.display = "";
                        dojo.byId("trInfoPresenca").style.display = "none";
                        dijit.byId('ckInfoPresenca').set('checked', false);
                        break;
                }
            }
            catch (e) {
                postGerarLog(e);
            }
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
            showCarregando();
        }
    });
}