var TODOS = 0;
var NORMAL = 1, PPT = 3;
var SITUACAO = 3;

function montarMetodosRelatorioMediasAlunos() {
    //Criação dos componentes
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dijit/form/FilteringSelect",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, ObjectStore, Cache, Memory, on, Button, FilteringSelect, registry, ItemFileReadStore) {
        ready(function () {
            try {
                montarTipos("pesOpcao", Memory);
                montarTiposAlunos("pesTipoAlunos", Memory);

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () { abrePesquisaTurmaFK(); }
                }, "pesTurmaFK");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () { limpaTurmaFK(); }
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

                sugereDataCorrente();
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");

                loadTipoTurma();
                dijit.byId("tipoTurma").on("change", function (tipo) {
                    try {
                        if (!hasValue(tipo,true) || tipo < 0)
                            dijit.byId("tipoTurma").set("value", TODOS);
                        else {
                            var pesSituacao = dijit.byId("pesSituacao");
                            if (tipo == PPT) {
                                dojo.byId("lblTurmaFilhas").style.display = "";
                                dojo.byId("divPesTurmasFilhas").style.display = "";
                                loadSituacaoTurmaPPT(Memory);
                                pesSituacao.set("value", 1);
                                pesSituacao.set("disabled", false);
                            }else {
                                dojo.byId("lblTurmaFilhas").style.display = "none";
                                dojo.byId("divPesTurmasFilhas").style.display = "none";
                                dijit.byId("pesTurmasFilhas").set("checked", false);
                                loadSituacaoTurma(Memory);
                                pesSituacao.set("value", 1);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesCurso").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("pesCurso").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesProduto").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("pesProduto").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesTurmasFilhas").on("click", function (e) {
                    try {
                        var gridTurma = dijit.byId("gridTurma");
                        var pesSituacao = dijit.byId("pesSituacao");
                        if (dijit.byId("tipoTurma").displayedValue == "Personalizada" && this.checked) {
                            loadSituacaoTurmaPPT(dojo.store.Memory);
                            loadSituacaoTurma(Memory);
                            pesSituacao.set("value", 1);
                        } else if (dijit.byId("tipoTurma").displayedValue == "Personalizada" && !this.checked) {
                            loadSituacaoTurma(dojo.store.Memory);
                            loadSituacaoTurmaPPT(Memory);
                            pesSituacao.set("value", 1);
                            pesSituacao.set("disabled", false);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                loadComponetesPesquisaReportMediasAlunos();

                dijit.byId("tipoTurmaFK").on("change", function (e) {
                    if (this.displayedValue == "Personalizada") {
                        dijit.byId("pesTurmasFilhasFK").set("checked", true);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', true);
                    } else {
                        dijit.byId("pesTurmasFilhasFK").set("checked", false);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', false);
                    }
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
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
        dojo.byId("idOrigemCadastro").value = REPORT_MEDIAS_ALUNOS;
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
        if (hasValue(dojo.byId("idOrigemCadastro").value) && dojo.byId("idOrigemCadastro").value == REPORT_MEDIAS_ALUNOS) {
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
                dojo.byId("noTurmaFK").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
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

function limpaTurmaFK() {
    dojo.byId("noTurmaFK").value = "";
    dojo.byId("cd_turma").value = "";
    dijit.byId('limparPesTurmaFK').set("disabled", true);
}

function montarTipos(nomElement, Memory) {
    try {
        var dados = [{ name: "Todos", id: "0" },
                     { name: "Alunos ativos(atuais)", id: "1" },
                     { name: "Ex Alunos", id: "2" }
        ]
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId(nomElement).store = statusStore;
        dijit.byId(nomElement).set("value", TODOS);
        dijit.byId(nomElement).oldValue = TODOS;

        //dijit.byId(nomElement).set('value', 1);
    } catch (e) {
        postGerarLog(e);
    }
}

function montarTiposAlunos(nomElement, Memory) {
    try {
        var dados = [
            { name: "Todos", id: "0" },
            { name: "Concluintes", id: "1" },
            { name: "Reprovados", id: "2" }
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

function loadTipoTurma() {
    var statusStoreTipo = new dojo.store.Memory({
        data: [
        { name: "Todas", id: 0 },
        { name: "Regular", id: 1 },
        { name: "Personalizada", id: 3 }
        ]
    });
    dijit.byId("tipoTurma")._onChangeActive = false;
    dijit.byId("tipoTurma").store = statusStoreTipo;
    dijit.byId("tipoTurma").set("value", 0);
    dijit.byId("tipoTurma")._onChangeActive = true;
}

function loadComponetesPesquisaReportMediasAlunos() {
    dojo.xhr.get({
        url: Endereco() + "/api/turma/componentesPesquisaMediaAlunos",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.cursos))
                    criarOuCarregarCompFiltering("pesCurso", data.retorno.cursos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_curso', 'no_curso', MASCULINO);
                if (hasValue(data.retorno.produtos))
                    criarOuCarregarCompFiltering("pesProduto", data.retorno.produtos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', MASCULINO);
            }
            showCarregando();
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem("apresentadorMensagem", error);
    });
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
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function emitirRelatorio() {
    try {
        if (!dijit.byId("formPesquisaRelatorioMediasAlunos").validate())
            return false;
        var dataIni = hasValue(dijit.byId("dtInicial").value) ? dojo.byId("dtInicial").value : "";
        var dataFim = hasValue(dijit.byId("dtFinal").value) ? dojo.byId("dtFinal").value : "";
        var cdCurso = hasValue(dijit.byId("pesCurso").value) ? dijit.byId("pesCurso").value : 0;
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var vl_media = hasValue(dijit.byId("pesMedia").value) ? dijit.byId("pesMedia").value : null;
        dojo.xhr.get({
            url: Endereco() + "/api/aluno/getUrlReporMediasAlunos?cd_turma=" + cd_turma + "&tipoTurma=" + dijit.byId("tipoTurma").value +
                "&turmasFilhas=" + document.getElementById("pesTurmasFilhas").checked + "&cdCurso=" + cdCurso +
                "&cdProduto=" + dijit.byId("pesProduto").value + "&pesOpcao=" + dijit.byId("pesOpcao").value + "&pesTipoAluno=" + dijit.byId("pesTipoAlunos").value +
                "&vl_media=" + vl_media +
                "&dtInicial=" + dataIni + "&dtFinal=" + dataFim +
                "&no_curso=" + dojo.byId("pesCurso").value + "&no_produto=" + dojo.byId("pesProduto").value + "&no_turma=" + dojo.byId("noTurmaFK").value +
                "&noTipoTurma=" + dojo.byId('tipoTurma').value + "&noPesOpcao=" + dojo.byId("pesOpcao").value +
                "&mostrarAvaliacao=" + dijit.byId('ckAvaliacao').checked,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioMediaAlunos?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoTurma(Memory) {
    try {
        var statusStore = new Memory({
            data: [
            { name: "Turmas em Andamento", id: 1 },
            { name: "Turmas em Formação", id: 3 },
            { name: "Turmas Encerradas", id: 2 }
            ]
        });

        if (hasValue(dijit.byId("pesSituacao"))) {
            dijit.byId("pesSituacao").store = statusStore;
            dijit.byId("pesSituacao").set("value", 1);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoTurmaPPT(Memory) {
    try {
        var statusStore = new Memory({
            data: [
            { name: "Turmas Ativas", id: 1 },
            { name: "Turmas Inativas", id: 2 }
            ]
        });

        dijit.byId("pesSituacao").store = statusStore;
        dijit.byId("pesSituacao").set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarSituacaoAlunoTurma(Memory, registry, ItemFileReadStore) {
    try {
        var w = registry.byId('situacaoAlunoTurma');
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
        w.setStore(store, []);
    } catch (e) {
        postGerarLog(e);
    }
}

