var TODOS = 0;
var NORMAL = 1, PPT = 3;
var NOTAS = 1, COBCEITO = 2;
function montarMetodosRelatorioAvaliacao() {
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
                loadSituacaoTurma();
                loadTipoTurma();

                sugereDataCorrente();
                loadTipoAval();
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");

                dijit.byId("tipoTurma").on("change", function (tipo) {
                    try {
                        if (!hasValue(tipo,true) || tipo < 0)
                            dijit.byId("tipoTurma").set("value", TODOS);
                        else {
                            var pesSituacao = dijit.byId("cbSituacao");
                            if (tipo == PPT) {
                                dojo.byId("lblTurmaFilhas").style.display = "";
                                dojo.byId("divPesTurmasFilhas").style.display = "";
                                dijit.byId("pesTurmasFilhas").set("checked", true);
                                //loadSituacaoTurmaPPT(Memory);
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
                        var pesSituacao = dijit.byId("cbSituacao");
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
                loadComponetesPesquisaReportAvaliacoes();

                dijit.byId("tipoTurmaFK").on("change", function (e) {
                    if (this.displayedValue == "Personalizada") {
                        dijit.byId("pesTurmasFilhasFK").set("checked", true);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', true);
                    } else {
                        dijit.byId("pesTurmasFilhasFK").set("checked", false);
                        dijit.byId('pesTurmasFilhasFK').set('disabled', false);
                    }
                });

                dijit.byId("cbTipoAval").on("change", function (e) {
                    limpaTurmaFK()
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
        if(dijit.byId('cbTipoAval').value == NOTAS)
            dojo.byId("idOrigemCadastro").value = REPORT_AVALIACAO;
        else
            dojo.byId("idOrigemCadastro").value = REPORT_AVALIACAO_CONCEITO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        var cdRegime = hasValue(dijit.byId("tipoTurma").value) ? dijit.byId("tipoTurma").value : 0;
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            dijit.byId('tipoTurmaFK').set('value', cdRegime);

        }
        var sitTurma = hasValue(dijit.byId("cbSituacao").value) ? dijit.byId("cbSituacao").value : 0;
        if (hasValue(dijit.byId('pesSituacaoFK'))) {
            dijit.byId('pesSituacaoFK').set('value', sitTurma);
            dijit.byId('pesSituacaoFK').set('disabled', sitTurma != 0);
        }

        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(-1, dojo.byId("idOrigemCadastro").value);
        var titulo = "Pesquisar Turma com avaliação " + (dijit.byId('cbTipoAval').value == NOTAS? " notas ":" conceito ");
        if (hasValue(dijit.byId('dtInicialAv'))) {
            dtInicial = hasValue(dijit.byId('dtInicialAv').value) ? dojo.byId('dtInicialAv').value : "";
            titulo = titulo + "no período " + dojo.byId('dtInicialAv').value;
        }
        if (hasValue(dijit.byId('dtFinalAv'))) {
            dtFinal = hasValue(dijit.byId('dtFinalAv').value) ? dojo.byId('dtFinalAv').value : "";
            titulo = titulo + " até " + dojo.byId('dtFinalAv').value;
        }

        dijit.byId("proTurmaFK").set('title', titulo)
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFK() {
    try {
        if (hasValue(dojo.byId("idOrigemCadastro").value) && (dojo.byId("idOrigemCadastro").value == REPORT_AVALIACAO || dojo.byId("idOrigemCadastro").value == REPORT_AVALIACAO_CONCEITO)) {
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

function loadTipoAval() {
    var statusStoreTipo = new dojo.store.Memory({
        data: [
            { name: "Nota", id: 1 },
            { name: "Conceito", id: 2 }
        ]
    });
    dijit.byId("cbTipoAval")._onChangeActive = false;
    dijit.byId("cbTipoAval").store = statusStoreTipo;
    dijit.byId("cbTipoAval").set("value", 1);
    dijit.byId("cbTipoAval")._onChangeActive = true;
}

function loadComponetesPesquisaReportAvaliacoes() {
    dojo.xhr.get({
        url: Endereco() + "/api/turma/componentesPesquisaAvaliacoes",
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
                if (hasValue(data.retorno.professores))
                    criarOuCarregarCompFiltering("cbAvaliador", data.retorno.professores, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_fantasia', MASCULINO);
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
                dijit.byId('dtInicialAv').attr("value", new Date(date.getFullYear(), 0, 1));
                //Data Final: Data do dia
                dijit.byId('dtFinalAv').attr("value", date);
            }
        } catch (e) {
            postGerarLog(e);
        }
    });
}

function emitirRelatorio() {
    try {
        if (!dijit.byId("formPesquisaRelatorioAvaliacoes").validate())
            return false;
        var dataIni = hasValue(dijit.byId("dtInicialAv").value) ? dojo.byId("dtInicialAv").value : "";
        var dataFim = hasValue(dijit.byId("dtFinalAv").value) ? dojo.byId("dtFinalAv").value : "";
        var cdCurso = hasValue(dijit.byId("pesCurso").value) ? dijit.byId("pesCurso").value : 0;
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var cdProduto = hasValue(dijit.byId("pesProduto").value) ? dijit.byId("pesProduto").value : 0;
        var cdRegime = hasValue(dijit.byId("tipoTurma").value) ? dijit.byId("tipoTurma").value : 0;
        var cdFuncionario = hasValue(dijit.byId("cbAvaliador").value) ? dijit.byId("cbAvaliador").value : 0;
        var sitTurma = hasValue(dijit.byId("cbSituacao").value) ? dijit.byId("cbSituacao").value : 0;
        var isConceito = dijit.byId("cbTipoAval").value == 2? true: false;
        dojo.xhr.get({
            url: Endereco() + "/api/coordenacao/getUrlReportAvaliacao?cd_turma=" + cd_turma + "&tipoTurma=" + cdRegime +
                "&cdCurso=" + cdCurso + "&cdProduto=" + cdProduto + "&dtInicial=" + dataIni + "&dtFinal=" + dataFim +
                "&cdFuncionario=" + cdFuncionario + "&sitTurma=" + sitTurma + "&isConceito=" + isConceito,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioAvaliacao?' + data, '1024px', '750px', 'popRelatorio');
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoTurma() {
    try {
        var statusStore = dojo.store.Memory({
            data: [
            { name: "Turmas em Andamento", id: 1 },
            { name: "Turmas em Formação", id: 3 },
            { name: "Turmas Encerradas", id: 2 }
            ]
        });

        dijit.byId("cbSituacao").store = statusStore;
        dijit.byId("cbSituacao").set("value", 1);
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

        dijit.byId("cbSituacao").store = statusStore;
        dijit.byId("cbSituacao").set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

