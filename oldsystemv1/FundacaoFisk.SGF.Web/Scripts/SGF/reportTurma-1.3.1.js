var TODOS = 0;
var NORMAL = 1, PPT = 3;
var SITUACAO = 3;

//Situação da Turma
var TURMA_ANDAMENTO = 1;
var TURMA_NOVA = 5;
var TURMA_FORMACAO = 3;
var TURMA_ENCERRADA = 2;
var TURMA_A_ENCERRAR = 4;

var EnumTipoRelatorio = {
    ANALITICO: 0,
    SINTETICO: 1
}

var EnumTipoTurma = {
    TODAS: 0,
    PERSONALIZADA: 3,
    REGULAR: 1
}

var EnumSituacaoTurma = {
     TURMA_ANDAMENTO: 1,
     TURMA_NOVA: 5,
     TURMA_FORMACAO: 3,
     TURMA_ENCERRADA: 2,
     TURMA_A_ENCERRAR: 4
}

function montarTipoRelatorio(Memory, on) {
    try {
        var dados = [
            { name: "Analítico", id: "0" },
            { name: "Sintético", id: "1" }
        ];
        var statusStore = new Memory({
            data: dados
        });
        dijit.byId('tipoRelatorio').store = statusStore;
        dijit.byId('tipoRelatorio').set("value", EnumTipoRelatorio.ANALITICO);
    }
    catch (e) {
        postGerarLog(e);
    }
};

function validaShowTipoRelatorio() {
    if (hasValue(dijit.byId("tipoTurma").item) && dijit.byId("tipoTurma").item.id === EnumTipoTurma.PERSONALIZADA) {
        dojo.byId('trTipoRelatorioTurma').style.display = "";
        dijit.byId('tipoRelatorio').set("value", EnumTipoRelatorio.ANALITICO);

    } else if (hasValue(dijit.byId("tipoTurma").item) &&
        (dijit.byId("tipoTurma").item.id === EnumTipoTurma.REGULAR || 
        dijit.byId("tipoTurma").item.id === EnumTipoTurma.TODAS) ) {
        if (hasValue(dijit.byId("pesSituacao").item) &&
        (dijit.byId("pesSituacao").item.id === EnumSituacaoTurma.TURMA_ANDAMENTO ||
            dijit.byId("pesSituacao").item.id === EnumSituacaoTurma.TURMA_FORMACAO ||
            dijit.byId("pesSituacao").item.id === EnumSituacaoTurma.TURMA_ENCERRADA)) {
            dojo.byId('trTipoRelatorioTurma').style.display = "";
            dijit.byId('tipoRelatorio').set("value", EnumTipoRelatorio.ANALITICO);
        } else {
            dojo.byId('trTipoRelatorioTurma').style.display = "none";
        }
    } else {
        if (hasValue(dijit.byId("pesSituacao").item) &&
        (dijit.byId("pesSituacao").item.id === EnumSituacaoTurma.TURMA_ANDAMENTO ||
            dijit.byId("pesSituacao").item.id === EnumSituacaoTurma.TURMA_FORMACAO ||
            dijit.byId("pesSituacao").item.id === EnumSituacaoTurma.TURMA_ENCERRADA)) {
            dojo.byId('trTipoRelatorioTurma').style.display = "";
            dijit.byId('tipoRelatorio').set("value", EnumTipoRelatorio.ANALITICO);
        } else {
            dojo.byId('trTipoRelatorioTurma').style.display = "none";
        }
    }
}
function montarMetodosRelatorioTurma() {
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

                sugereDataCorrente();
                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorio");
                loadTipoTurma();
                loadDataTipoProg();
                loadTipoOnline();
                //findAllEmpresasUsuarioComboFiltroRelatorioTurma();
                setMenssageMultiSelect(SITUACAO, 'situacaoAlunoTurma');
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

                        validaShowTipoRelatorio();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });


                montarTipoRelatorio(Memory, on);
                validaShowTipoRelatorio();

                dijit.byId("sProgramacao").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("sProgramacao").set("value", TODOS);
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
                dijit.byId("pesDuracao").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("pesDuracao").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesProfessor").on("change", function (cdProg) {
                    try {
                        if (!hasValue(cdProg) || cdProg < 0)
                            dijit.byId("pesProfessor").set("value", TODOS);
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
                dijit.byId("pesSituacao").on("change", function (sit) {
                    try {
                        switch (sit) {
                            case !hasValue(sit) || sit < 0:
                                dijit.byId("pesSituacao").set("value", 1);

                            case TURMA_ANDAMENTO:
                                optSituacaoAluno(true);
                                break;

                            case TURMA_NOVA:
                                optSituacaoAluno(false, TURMA_NOVA);
                                break;

                            case TURMA_FORMACAO:
                                optSituacaoAluno(true);
                                break;

                            case TURMA_ENCERRADA:
                                optSituacaoAluno(true);
                                break;

                            case TURMA_A_ENCERRAR:
                                optSituacaoAluno(false, TURMA_A_ENCERRAR);
                                break;

                            default:

                        }
                        validaShowTipoRelatorio();
                        //if (!hasValue(sit) || sit < 0)
                        //    dijit.byId("pesSituacao").set("value", 1);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                montarSituacaoAlunoTurma(Memory, registry, ItemFileReadStore);
                findIsLoadComponetesPesquisaReportTurma();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function loadTipoTurma() {
    var statusStoreTipo = new dojo.store.Memory({
        data: [
        { name: "Todas", id: 0 },
        { name: "Regular", id: 1 },
        { name: "Personalizada", id: 3 }
        ]
    });
    dijit.byId("tipoTurma").store = statusStoreTipo;
    dijit.byId("tipoTurma").set("value", 0);
}

function loadTipoOnline() {
    var statusStoreTipo = new dojo.store.Memory({
        data: [
        { name: "Todas", id: 0 },
        { name: "Presencial", id: 1 },
        { name: "Online", id: 2 }
        ]
    });
    dijit.byId("tipoOnline").store = statusStoreTipo;
    dijit.byId("tipoOnline").set("value", 0);
}

function loadDataTipoProg() {

    var statusStoreTipo = new dojo.store.Memory({
        data: [
        { name: "Todas", id: 0 },
        { name: "Geradas", id: 1 },
        { name: "Não geradas", id: 2 }
        ]
    });
    dijit.byId("sProgramacao").store = statusStoreTipo;
    dijit.byId("sProgramacao").set("value", 0);
}

function findIsLoadComponetesPesquisaReportTurma() {
    dojo.xhr.get({
        url: Endereco() + "/api/turma/componentesPesquisaTurma",
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
                if (hasValue(data.retorno.duracoes))
                    criarOuCarregarCompFiltering("pesDuracao", data.retorno.duracoes, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_duracao', 'dc_duracao', MASCULINO);
                if (data.retorno.usuarioSisProf == true) {
                    criarOuCarregarCompFiltering("pesProfessor", data.retorno.professores, "", data.retorno.professores[0].cd_pessoa, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_pessoa');
                    if (!eval(Master()))
                        dijit.byId("pesProfessor").set("disabled", true);
                } else if (hasValue(data.retorno.professores))
                    criarOuCarregarCompFiltering("pesProfessor", data.retorno.professores, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_pessoa', MASCULINO);
            }
            showCarregando();
        }
        catch (e) {
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
        if (!dijit.byId("formPesquisaRelatorioTurma").validate())
            return false;
        if (!dijit.byId("formPeriodoDataInicial").validate())
            return false;
        if (!dijit.byId("formPeriodoDataFinal").validate())
            return false;
        var dataIni = hasValue(dijit.byId("dtInicial").value) ? dojo.byId("dtInicial").value : "";
        var dataFim = hasValue(dijit.byId("dtFinal").value) ? dojo.byId("dtFinal").value : "";

        if (dijit.byId("pesSituacao").value == TURMA_A_ENCERRAR)
        {
            dataIni = "";
            dataFim = "";
            dijit.byId("mostrarTel").set("checked", false);
            dijit.byId("monstrarRespons").set("checked", false);
        }

        var dataIniFim = hasValue(dijit.byId("dtInicialFim").value) ? dojo.byId("dtInicialFim").value : "";
        var dataFimFim = hasValue(dijit.byId("dtFinalFim").value) ? dojo.byId("dtFinalFim").value : "";

        if (dijit.byId("pesSituacao").value == TURMA_NOVA)
        {
            dataIniFim = "";
            dataFimFim = "";
            dijit.byId("mostrarTel").set("checked", false);
            dijit.byId("monstrarRespons").set("checked", false);
        }
        var cdCurso = hasValue(dijit.byId("pesCurso").value) ? dijit.byId("pesCurso").value : 0;
        var professor = hasValue(dijit.byId("pesProfessor").value) ? dijit.byId("pesProfessor").value : 0;
        //Montando situações
        var situacoes = "";
        if (!hasValue(dijit.byId('situacaoAlunoTurma').value) || dijit.byId('situacaoAlunoTurma').value.length <= 0)
            situacoes = "100";
        else
            for (var i = 0; i < dijit.byId('situacaoAlunoTurma').value.length; i++) {
                if (situacoes == "")
                    situacoes = dijit.byId('situacaoAlunoTurma').value[i];
                else
                    situacoes = situacoes + "|" + dijit.byId('situacaoAlunoTurma').value[i];
            }
        var dias = "";
        if (!dijit.byId('ckDomingo').checked) 
            dias = '0'
        else
            dias = '1'
        if (!dijit.byId('ckSegunda').checked) 
            dias = dias + '0'
        else
            dias = dias + '1'
        if (!dijit.byId('ckTerca').checked) 
            dias = dias + '0'
        else
            dias = dias + '1'
        if (!dijit.byId('ckQuarta').checked) 
            dias = dias + '0'
        else
            dias = dias + '1'
        if (!dijit.byId('ckQuinta').checked) 
            dias = dias + '0'
        else
            dias = dias + '1'
        if (!dijit.byId('ckSexta').checked) 
            dias = dias + '0'
        else
            dias = dias + '1'
        if (!dijit.byId('ckSabado').checked) 
            dias = dias + '0'
        else
            dias = dias + '1'

        dojo.xhr.get({
            url: Endereco() + "/api/turma/getUrlReporTurma?&tipoTurma=" + dijit.byId("tipoTurma").value + "&cdCurso=" + cdCurso + "&cdDuracao=" + dijit.byId("pesDuracao").value +
                "&cdProduto=" + dijit.byId("pesProduto").value + "&cdProfessor=" + professor + "&prog=" + dijit.byId("sProgramacao").value +
                "&turmasFilhas=" + document.getElementById("pesTurmasFilhas").checked + "&dtInicial=" + dataIni + "&dtFinal=" + dataFim + "&dtInicialFim=" + dataIniFim + "&dtFinalFim=" + dataFimFim +
                "&mostrarTelefone=" + document.getElementById("mostrarTel").checked + "&mostrarResp=" + document.getElementById("monstrarRespons").checked + "&situacaoTurma=" + dijit.byId("pesSituacao").value +
                "&situacaoAlunoTurma=" + situacoes + "&tipoOnline=" + dijit.byId("tipoOnline").value + "&tipoRelatorio=" + dijit.byId("tipoRelatorio").value + "&dias=" + dias, 
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            abrePopUp(Endereco() + '/Relatorio/RelatorioTurma?' + data, '1024px', '750px', 'popRelatorio');
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
                { name: "Turmas Novas",        id: 5 },
                { name: "Turmas em Formação",  id: 3 },
                { name: "Turmas Encerradas",   id: 2 },
                { name: "Turmas a Encerrar",   id: 4 }
            ]
        });

        dijit.byId("pesSituacao").store = statusStore;
        dijit.byId("pesSituacao").set("value", 1);
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

function optSituacaoAluno(show, situacao_turma) {
    isDisplay = 'none';
    dijit.byId("dtInicialFim").set("required", false); 
    dijit.byId("dtInicial").set("required", false);
    dijit.byId("dtFinalFim").set("required", false);

    if (show) {
        isDisplay = '';
        dijit.byId("sProgramacao").set("value", 0);
        dijit.byId("sProgramacao").set("disabled", false);
        dijit.byId("situacaoAlunoTurma").set("disabled", false);
        dojo.byId("paneDataInicial").style.display = isDisplay;
        dojo.byId("paneDataFinal").style.display = isDisplay;
        dojo.byId("paMostrarDetalhes").style.display = isDisplay;

    } else {
        if (situacao_turma == TURMA_A_ENCERRAR) {
            dijit.byId("dtInicialFim").set("required", true);
            dijit.byId("dtFinalFim").set("required", true);
            dijit.byId("sProgramacao").set("value", 1);
            dijit.byId("sProgramacao").set("disabled", true);
            dijit.byId("situacaoAlunoTurma").set("disabled", true);
            dojo.byId("paneDataInicial").style.display = isDisplay;
            dojo.byId("paneDataFinal").style.display = '';
            dojo.byId("paMostrarDetalhes").style.display = isDisplay;
        }
        if (situacao_turma == TURMA_NOVA)
        {
            dijit.byId("dtInicial").set("required", true);
            dijit.byId("sProgramacao").set("value", 0);
            dijit.byId("sProgramacao").set("disabled", false);
            dijit.byId("situacaoAlunoTurma").set("disabled", true);
            dojo.byId("paneDataInicial").style.display = '';
            dojo.byId("paneDataFinal").style.display = isDisplay;
            dojo.byId("paMostrarDetalhes").style.display = isDisplay;
        }
    }

    
}
/*
function findAllEmpresasUsuarioComboFiltroRelatorioTurma() {
    require([
        "dojo/_base/xhr",
        "dojo/data/ObjectStore",
        "dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/empresa/findAllEmpresasUsuarioComboFK",
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            idProperty: "cd_pessoa"
        }).then(function (data) {
            try {
                showCarregando();

                var dataRetorno = jQuery.parseJSON(data);
                if (hasValue(dataRetorno)) {
                    var dataCombo = dataRetorno.map(function (item, index, array) {
                        return { name: item.dc_reduzido_pessoa, id: item.cd_pessoa + "" };
                    });
                    loadSelect(dataRetorno, "escolaFiltroRelatorioTurma", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
                }
                showCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
            function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagem', error.response.data);
            });
    });
}
*/