var PESQTURMADIARIOAULAFILTRO = 0, PESQTURMADIARIOAULACADASTRO = 1;
var PROFESSORPRESENTE = 0, FALTAPROFESSOR = 1, FALTAJUSTIFICADAPORFESSOR = 2;
var AULAEFETIVADA = 0, AULAAGENDADA = 2, AULACANCELADA = 1;
var PROGRAMACAOSEMTURMA = -1;
var NOVO = 1, EDIT = 2;
var TIPOAULADEFAULT = 1;
//TODO Michelangelo
//Não sei pra que isso? (Deivid)
var CAD_DIARIO = 12;
function montarDiarioPartial(funcao) {
    //Criação da Grade de sala
    require([
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dijit/form/FilteringSelect",
    "dojox/json/ref",
    "dojox/grid/EnhancedGrid",
    "dijit/registry"
    ], function (xhr, Memory, Button, ready, on, FilteringSelect, ref, EnhancedGrid) {
        ready(function () {
            var gridAlunosCarga = new dojox.grid.EnhancedGrid({
                store: dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null })}),
                structure: [
                    { name: "Aluno", field: "nomeAluno", width: "87%", styles: "min-width:180px;" },
                    { name: "Carga H.", field: "nm_carga", width: "13%", styles: "min-width:50px; text-align: center;" },
                ],
                noDataMessage: msgNotRegEnc,
                canSort: true,
                selectionMode: "single"
            }, "gridAlunosCarga");
            gridAlunosCarga.startup();

            xhr.get({
                url: Endereco() + "/api/escola/getNmMaxFaltasAluno",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (data.retorno != null && data.retorno > 0)
                        dojo.byId("qtd_max_faltas_aluno").value = data.retorno;
                    //dojo.byId('tagFalta').style.display = 'none';
                    inserirIdTabsCadastroDiarioAula();
                    var statusStoreCurso = new Memory({
                        data: [
                          { name: "Efetivada", id: "0" },
                          { name: "Agendada", id: "2" },
                          { name: "Cancelada", id: "1" }
                        ]
                    });
                    dijit.byId("cbStatusCad").store = statusStoreCurso;

                    //Crud Pessoa
                    if (dijit.registry.byId("incluirDiario") == null) { new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { } }, "incluirDiario"); }

                    if (hasValue(dijit.byId("cancelarDiario"))) {
	                    dijit.byId("cancelarDiario").destroy();
                    }
                    new Button({ label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { } }, "cancelarDiario");
                    new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () { dijit.byId("cadDiarioAula").hide(); } }, "fecharDiario");
                    new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { } }, "alterarDiario");
                    new Button({
                        label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () { }
                    }, "deleteDiario");
                    new Button({
                        label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                            try {
                                destroyCreateGridAluno();
                                limparDiarioAula(false);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }
                    }, "limparDiario");
                    //Fim
                    var buttonFkArray = ['pesTurmaFKCad'];

                    for (var p = 0; p < buttonFkArray.length; p++) {
                        var buttonFk = document.getElementById(buttonFkArray[p]);

                        if (hasValue(buttonFk)) {
                            buttonFk.parentNode.style.minWidth = '18px';
                            buttonFk.parentNode.style.width = '18px';
                        }
                    }

                    dijit.byId("ckFaltaProf").on("change", function (evt) {
                        try {
                            if (evt == true) {
                                concedeFalta(xhr, ready, Memory, FilteringSelect);
                            } else {
                                dijit.byId('ProfSubst').set('required', false);
                                dijit.byId('cbProfessor').set('required', true);
                                dijit.byId('cbProfessor').set('disabled', false);
                                dojo.byId("tagFalta").style.display = 'none';
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("pesTurmaFKCad").on("click", function (evt) {
                        try {
                            if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                montarGridPesquisaTurmaFK(function () {
                                    abrirPesquisaTurmaFKCad();
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
                                abrirPesquisaTurmaFKCad();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("cbProgramacao").on("change", function (evt) {
                        try {
                            if (evt != null) {
                                limparCamposFilhosDeProgramacao();
                                if (hasValue(dijit.byId("gridAlunosDiario"))) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgInfoProgAlterada);
                                    apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                                    destroyCreateGridAluno();
                                    setarTabCadDiarioAula();
                                }
                                if (hasValue(evt, true) && evt == PROGRAMACAOSEMTURMA)
                                    configuraDiarioSemProgramacao(xhr, ready, Memory, FilteringSelect);
                                else
                                    if (hasValue(evt, true) && evt > 0)
                                        preencherComponetesCadastroByProgramacao(evt);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("timeIniDiario").on("change", function (evt) {
                        try {
                            if (!hasValue(dijit.byId("cbProfessor").store.data) && dijit.byId("cbProfessor").store.data.length == 0) {
                                loadPopulaCompProfLogado(xhr, ready, Memory, FilteringSelect);
                            }
                            validaHorariosProgramacao(xhr, ready, Memory, FilteringSelect);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("timeFimDiario").on("change", function (evt) {
                        try {
                            if (!hasValue(dijit.byId("cbProfessor").store.data) && dijit.byId("cbProfessor").store.data.length == 0) {
                                loadPopulaCompProfLogado(xhr, ready, Memory, FilteringSelect);
                            }
                            validaHorariosProgramacao(xhr, ready, Memory, FilteringSelect);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("dtaAula").on("change", function (evt) {
                        try {
                            var diaSem = dojo.byId("diaSemana");
                            if (hasValue(evt)) {
                                dataDia();
                                verificarAlunosPendenciaMaterialDidaticoCurso(evt, function () {
                                    diaSem.value = diaSemana(evt);
                                    if (hasValue(dijit.byId("gridAlunosDiario"))) {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgInfoDataAlterada);
                                        apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb, true);
                                        destroyCreateGridAluno();
                                        setarTabCadDiarioAula();
                                        return false;
                                    }
                                });
                            }
                            else
                                diaSem.value = "";
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    dijit.byId("eventos").on("change", function (evt) {
                        try {
                            if (hasValue(evt, true) && isNumeric(evt, null, null)) {
                                //Salva a pesquisa por evento anterior, para buscar as novas.

                                var primeiraPesquisa = false;
                                var gridAlunos = dijit.byId("gridAlunosDiario");
                                if (!hasValue(gridAlunos.eventosAlunos)) {
                                    primeiraPesquisa = true;
                                    gridAlunos.eventosAlunos = new Array();
                                    gridAlunos.eventoAnterior = evt;
                                }
                                var eventoAnterio = primeiraPesquisa ? 1 : gridAlunos.eventoAnterior;

                                if (!hasValue(gridAlunos.eventosAlunos[eventoAnterio]))
                                    gridAlunos.eventosAlunos[eventoAnterio] = new Array();
                                if (hasValue(gridAlunos.store.objectStore.data))
                                    gridAlunos.eventosAlunos[eventoAnterio] = gridAlunos.store.objectStore.data;

                                if (!hasValue(gridAlunos.eventosAlunos[evt]) || primeiraPesquisa) {
                                    var cd_turma = dojo.byId("cdTurmaCad").value;
                                    var dtAula = dojo.date.locale.parse(dojo.byId("dtaAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                                    var cd_diario_aula = dojo.byId("cd_diario_aula").value;
                                    loadDataAlunosByEvents(evt, cd_turma, dtAula, cd_diario_aula, xhr);
                                } else {
                                    loadDataAlunosByEventsLocal(evt);
                                }
                                gridAlunos.eventoAnterior = evt;
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                    inicial();
                    if (hasValue(funcao))
                        funcao.call();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemDiarioPartial", error);
            });
        });
    });
}

function verificarAlunosPendenciaMaterialDidaticoCurso(evt, pFuncao) {
    try {
        var cdTurma = dojo.byId("cdTurmaCad").value;
        if (cdTurma != null && cdTurma > 0 && hasValue(dojo.byId("dtaAula").value))
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getAlunosPendenciaMaterialDidaticoCurso?cd_turma=" + cdTurma + "&dataProgTurma=" + dojo.byId("dtaAula").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataProg) {
                try {
                    dataProg = jQuery.parseJSON(dataProg);
                    data = dataProg.retorno;
                    if (dataProg != null && dataProg.alunos != null && dataProg.alunos.length > 0) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, dataProg.nomeAlunosPendencia);
                        apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                    }
                    if (hasValue(pFuncao))
                        pFuncao.call();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            });
        else
            if (hasValue(pFuncao))
                pFuncao.call();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataAlunosByEventsLocal(cd_evento) {
    try {
        var gridAlunos = dijit.byId("gridAlunosDiario");
        if (hasValue(gridAlunos.eventosAlunos) && hasValue(gridAlunos.eventosAlunos[cd_evento])) {
            gridAlunos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridAlunos.eventosAlunos[cd_evento] }) }));
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaHorariosProgramacao(xhr, ready, Memory, FilteringSelect) {
    try {
        if (hasValue(dijit.byId("timeFimDiario").original_value) && hasValue(dijit.byId("timeIniDiario").original_value)
                && (dijit.byId("timeIniDiario").original_value > dijit.byId("timeIniDiario").value || dijit.byId("timeFimDiario").original_value < dijit.byId("timeFimDiario").value)) {//Ultrapassou o horário da programação
            dijit.byId("ckFaltaProf").set("checked", true);
        }
        else
            configuraDiarioSemProgramacao(xhr, ready, Memory, FilteringSelect);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function concedeFalta(xhr, ready, Memory, FilteringSelect) {
    try {
        dijit.byId('ProfSubst').set('required', true);
        dojo.byId("tagFalta").style.display = '';
        loadPopulaCompProfSubs(xhr, ready, Memory, FilteringSelect);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDiarioSemProgramacao(xhr, ready, Memory, FilteringSelect) {
    try {
        //Atualiza os professores com todos os professores ativos da turma:
        if (dojo.byId('cd_usuario_professor').value == "0")
            loadPopulaCompProf(xhr, ready, Memory, FilteringSelect);
        dojo.byId("nm_aula_turma").value = null;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPopulaCompProf(xhr, ready, Memory, FilteringSelect) {
    try {
        var compProf = dijit.byId("cbProfessor");
        if (compProf != null)
            xhr.get({
                url: Endereco() + "/api/professor/getProfessorTurma?cd_turma=" + dojo.byId("cdTurmaCad").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    (hasValue(jQuery.parseJSON(data).retorno)) ? data = jQuery.parseJSON(data).retorno : jQuery.parseJSON(data);
                    criarOuCarregarCompFiltering("cbProfessor", data, "", null, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function loadPopulaCompProfLogado(xhr, ready, Memory, FilteringSelect) {
    try {
        var compProf = dijit.byId("cbProfessor");
        if (compProf != null)
            xhr.get({
                url: Endereco() + "/api/professor/getProfessorTurmaLogado?cd_turma=" + dojo.byId("cdTurmaCad").value,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                    try {
                        (hasValue(jQuery.parseJSON(data).retorno)) ? data = jQuery.parseJSON(data).retorno : jQuery.parseJSON(data);
                        criarOuCarregarCompFiltering("cbProfessor", data, "", null, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemDiarioPartial', error);
                });
    }
    catch (e) {
        postGerarLog(e);
    }
}




function maskHourDiario(field) {
    try {
        $(field).mask("99:99:99");
    }
    catch (e) {
        postGerarLog(e);
    }
};

function inicial() {
    require([
           "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                //$("#telefone").mask("(99) 9999-9999");
                dijit.byId("timeIniDiario")._onChangeActive = false;
                dijit.byId("timeFimDiario")._onChangeActive = false;
                maskHourDiario("#timeIniDiario");
                maskHourDiario("#timeFimDiario");
                dijit.byId("timeIniDiario")._onChangeActive = true;
                dijit.byId("timeFimDiario")._onChangeActive = true;
                //maskDate("#dtaAula");
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxEv(value, rowIndex, obj) {
    try {
        var gridName = 'gridEvento';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodasEv');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_evento", grid._by_idx[rowIndex].item.cd_evento);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:9px'  id='" + id + "'/> ";

        setTimeout("configuraCheckBox(" + value + ", 'cd_evento', 'selecionadoEv', " + rowIndex + ", '" + id + "', 'selecionaTodasEv', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPesquisaTurmaFKCad() {
    try {
        dojo.byId("trAluno").style.display = "";
        var cd_prof = null;
        if (hasValue(dojo.byId('cd_usuario_professor').value))
            cd_prof = parseInt(dojo.byId('cd_usuario_professor').value);
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = DIARIOAULA;
        dojo.byId("id_origem_diarioAula").value = PESQTURMADIARIOAULACADASTRO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK')))
            dijit.byId('tipoTurmaFK').set('value', 1);
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        //pesquisarTurmaFK(parseInt(cd_prof));
        pesquisarTurmaFKComAluno();
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaFKComAluno() {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            /*--combo_escola_fk
            //Mostra a combo de escola
            dojo.byId("trEscolaTurmaFiltroFk").style.display = "";
            dojo.byId("lbEscolaTurmaFiltroFk").style.display = "";
            require(['dojo/dom-style', 'dijit/registry'],
                function (domStyle, registry) {

                    domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', '');
                });*/
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearchComAluno?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value + "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor + "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked
                                            + "&cdAluno=" + cdAluno + "&dtInicial=&dtFinal=&cd_turma_PPT=null&cdTurmaOri=0&opcao=" + CAD_DIARIO + "&cd_escola_combo_fk=0",
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];

            if (dijit.byId("tipoTurmaFK").get('value') == PPT) {
                grid.layout.setColumnVisibility(2, true)
                grid.layout.setColumnVisibility(3, false)
                grid.turmasFilhas = true;
            }
            else {
                grid.layout.setColumnVisibility(2, false)
                grid.layout.setColumnVisibility(3, true)
                grid.turmasFilhas = false;
            }
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function retornarTurmaFKDiarioAula() {
    require(["dojo/ready", "dojo/store/Memory", "dojo/_base/xhr", "dijit/form/FilteringSelect"],
        function (ready, Memory, xhr, FilteringSelect) {
            try {
                var valido = true;
                var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
                if (!hasValue(gridPesquisaTurmaFK.itensSelecionados))
                    gridPesquisaTurmaFK.itensSelecionados = [];
                if (gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
                    if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                    valido = false;
                }
                else {
                    var origemPesqTurma = dojo.byId("id_origem_diarioAula").value;
                    if (origemPesqTurma == PESQTURMADIARIOAULAFILTRO) {
                        dojo.byId("desc_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                        dojo.byId("cdTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                        dijit.byId('limparPesTurmaFK').set("disabled", false);
                    }
                    //Retorna com a turma selecionada no cadastro e pesquisa as programações livres.
                    if (origemPesqTurma == PESQTURMADIARIOAULACADASTRO && gridPesquisaTurmaFK.itensSelecionados[0].cd_turma > 0) {
                        
                        limparDiarioAula(false);
                        dojo.byId("escoladiario").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_pessoa_escola;
                        showCarregando();
                        xhr.get({
                            url: Endereco() + "/api/escola/getProgramacoesTurmasSemDiarioAula?cd_turma=" + gridPesquisaTurmaFK.itensSelecionados[0].cd_turma,
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                        }).then(function (dataProg) {
                            try {
                                dataProg = jQuery.parseJSON(dataProg);
                                dataProg = dataProg.retorno;
                                if (dataProg != null && dataProg.alunos != null && dataProg.alunos.length > 0) {
                                    var mensagensWeb = new Array();
                                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, dataProg.nomeAlunosPendencia);
                                    apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                                }

                                var programacaos = new Array();
                                var cdProg = null;
                                var compProg = dijit.byId("cbProgramacao");
                                dijit.byId('cbProfessor').set('disabled', false);
                                dijit.byId('timeIniDiario').set("disabled", false);
                                dijit.byId('timeFimDiario').set("disabled", false);
                                programacaos.push({ id: PROGRAMACAOSEMTURMA, name: labelCompProgramcaoSemTurma });
                                if (dataProg != null && hasValue(dataProg.progsTurma) && dataProg.progsTurma.length > 0) {
                                    cdProg = dataProg.progsTurma[0].cd_programacao_turma;
                                    $.each(dataProg.progsTurma, function (idx, val) {
                                        programacaos.push({
                                            id: val.cd_programacao_turma,
                                            name: val.no_programacao,
                                            nm_aula_programacao_turma: val.nm_aula_programacao_turma,
                                            dc_programacao_turma: val.dc_programacao_turma,
                                            dta_programacao_turma: val.dta_programacao_turma,
                                            hr_inicial_programacao: val.hr_inicial_programacao,
                                            hr_final_programacao: val.hr_final_programacao,
                                            cd_sala_prog: val.cd_sala_prog,
                                            aula_externa: val.aula_externa
                                        });
                                    });
                                }
                                var storeProg = new Memory({
                                    data: programacaos
                                });
                                compProg.store = storeProg;
                                compProg._onChangeActive = false;
                                if (hasValue(cdProg))
                                    compProg.set("value", cdProg);
                                compProg._onChangeActive = true;
                                //criarOuCarregarCompFiltering("cbProgramacao", programacaos, "", cdProg, ready, Memory, FilteringSelect, 'id', 'name');
                                if (hasValue(gridPesquisaTurmaFK.itensSelecionados[0])) {
                                    dojo.byId("no_turma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
                                    dojo.byId("cdTurmaCad").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
                                }
                                toogleTabEventos(false);
                                if (dataProg != null && hasValue(dataProg.professoresHorariosTurma) && dataProg.professoresHorariosTurma.length > 0) {
                                    var cdProfDefault = null;
                                    if (dataProg.professoresHorariosTurma.length == 1)
                                        cdProfDefault = dataProg.professoresHorariosTurma[0].cd_pessoa;

                                    criarOuCarregarCompFiltering("cbProfessor", dataProg.professoresHorariosTurma, "", cdProfDefault, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                                }
                                // Se o usuário que estiver cadastrando o diário de aula for um professor, seu nome abreviado deverá vir preenchido automaticamente e não poderá ser alterado.
                                //LBM não pode apagar pois elimina a mensagem gerada acima de material ou inadimplência.
                                //apresentaMensagem("apresentadorMensagemDiarioPartial", null);
                                if (dojo.byId('cd_usuario_professor').value != "0") {
                                    var professoresHorariosTurma = new Array();
                                    if (hasValue(dataProg.professoresHorariosTurma) && hasValue(dataProg.professoresHorariosTurma[0].no_fantasia)) {
                                        dijit.byId('cbProfessor').set('disabled', true);
                                        if (dojo.byId('cd_usuario_professor').value == dataProg.professoresHorariosTurma[0].cd_pessoa) {
                                            dojo.byId('nome_usuario_professor').value = dataProg.professoresHorariosTurma[0].no_fantasia;
                                            professoresHorariosTurma.push({ cd_pessoa: dojo.byId('cd_usuario_professor').value, no_fantasia: dojo.byId('nome_usuario_professor').value });
                                            criarOuCarregarCompFiltering("cbProfessor", professoresHorariosTurma, "", dojo.byId('cd_usuario_professor').value, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                                        }
                                        else {
                                            var mensagensWeb = new Array();
                                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHorarioDiferenteProfTurma);
                                            apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                                            hideCarregando();
                                            dijit.byId('timeIniDiario').set("disabled", true);//Desabilitando para travar e não deixar seguir o cadastro.
                                            dijit.byId('timeFimDiario').set("disabled", true);
                                            return false
                                        }
                                    } else {
                                        var mensagensWeb = new Array();
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgTurmaSemProgramacaoComDiarioAulaLancado);
                                        apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                                    }                                    
                                }

                                if (dataProg != null && hasValue(dataProg.avaliacoesTurma) && dataProg.avaliacoesTurma.length > 0)
                                    criarOuCarregarCompFiltering("cbAvaliacao", dataProg.avaliacoesTurma, "", null, ready, Memory, FilteringSelect, 'cd_avalicao', 'descTipoENomeAvalicao');
                                ready(function () {
                                    preencherComponetesCadastroByProgramacao(cdProg);
                                });
                                showCarregando();
                            }
                            catch (e) {
                                postGerarLog(e);
                                showCarregando();
                            }
                        },
                        function (error) {
                            apresentaMensagem("apresentadorMensagemDiarioPartial", error);
                            showCarregando();
                        });
                    }
                }
                if (!valido)
                    return false;
                dijit.byId("proTurmaFK").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

function preencherComponetesCadastroByProgramacao(cdProg) {
    try {
        var compProg = dijit.byId("cbProgramacao");
        if (compProg != null && hasValue(compProg.store.data) && compProg.store.data.length > 0) {
            $.each(compProg.store.data, function (idx, val) {
                var timeIniDiario = "";
                var timeFimDiario = "";
                if (val.id == cdProg) {

                    var horaI = 0, horaF = 0, minI = 0, minFim = 0;
                    if (hasValue(val.hr_inicial_programacao)) {
                        horaI = val.hr_inicial_programacao.substring(0, 2);
                        minI = val.hr_inicial_programacao.substring(3, 5);
                        timeIniDiario = "T" + horaI + ":" + minI + ":00";
                    }
                    if (hasValue(val.hr_final_programacao)) {
                        horaF = val.hr_final_programacao.substring(0, 2);
                        minFim = val.hr_final_programacao.substring(3, 5);
                        timeFimDiario = "T" + horaF + ":" + minFim + ":00";
                    }
                    if (horaI > 0 && horaF > 0) {
                        dijit.byId("timeIniDiario")._onChangeActive = false;
                        dijit.byId("timeFimDiario")._onChangeActive = false;
                        dijit.byId("timeIniDiario").set("value", timeIniDiario);
                        dijit.byId("timeIniDiario").original_value = dijit.byId("timeIniDiario").value;
                        dijit.byId("timeFimDiario").set("value", timeFimDiario);
                        dijit.byId("timeFimDiario").original_value = dijit.byId("timeFimDiario").value;
                        dijit.byId("timeIniDiario")._onChangeActive = true;
                        dijit.byId("timeFimDiario")._onChangeActive = true;
                    }
                    dijit.byId('dtaAula')._onChangeActive = false;
                    dijit.byId('dtaAula').set("value", val.dta_programacao_turma);
                    var diaSem = dojo.byId("diaSemana");
                    diaSem.value = diaSemana(dijit.byId('dtaAula').value);
                    dijit.byId('dtaAula')._onChangeActive = true;
                    dijit.byId('descObsCad').set("value", val.dc_programacao_turma);
                    dojo.byId("nm_aula_turma").value = val.nm_aula_programacao_turma;
                    if (hasValue(val.aula_externa))
                        dijit.byId("ckExterna").set("checked", val.aula_externa);
                    if (val.cd_sala_prog != null && val.cd_sala_prog > 0)
                        dijit.byId('cbSalaCad').set("value", val.cd_sala_prog);
                    return false;
                }
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function selecionaTabDiario(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        var cd_diario_aula = dojo.byId("cd_diario_aula").value;
        if (tab.getAttribute('widgetId') == 'tabContainerDiarioAula_tablist_tabEventos' && !hasValue(dijit.byId("gridAlunosDiario"))) {
            apresentaMensagem("apresentadorMensagemDiarioPartial", null);
            showCarregando();
            loadDataECreateTabEventos(cd_diario_aula);
        }
        if (tab.getAttribute('widgetId') == 'tabContainerDiarioAula_tablist_tabCargas') {
            apresentaMensagem("apresentadorMensagemDiarioPartial", null);
            showCarregando();
            loadDataCarga(cd_diario_aula);
        }
        else {
            var gridAlunoCarga = dijit.byId('gridAlunosCarga');
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) });
            gridAlunoCarga.setStore(dataStore);

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCadDiarioAula() {
    try {
        var tabs = dijit.byId("tabContainerDiarioAula");
        var pane = dijit.byId("tabPrincipalDiarioAula");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparDiarioAula(limparSala) {
    try {
        apresentaMensagem("apresentadorMensagemDiarioPartial", null);
        var cbStatus = dijit.byId("cbStatusCad");
        var cbProfessor = dijit.byId("cbProfessor");
        var compAvaliacao = dijit.byId("cbAvaliacao");
        var compProg = dijit.byId("cbProgramacao");
        var compSala = dijit.byId("cbSalaCad");
        setarTabCadDiarioAula();
        var statusStore = new dojo.store.Memory({
            data: null
        });
        clearForm("formDiarioPrincipal");
        toogleTabEventos(true);
        dojo.byId("cd_diario_aula").value = 0;
        dojo.byId("escoladiario").value = 0;
        dojo.byId("cdTurmaCad").value = 0;
        compProg.reset();
        compProg.store = statusStore;
        dijit.byId("dtaAula").reset();
        dijit.byId("timeIniDiario")._onChangeActive = false;
        dijit.byId("timeFimDiario")._onChangeActive = false;
        dijit.byId("timeIniDiario").original_value = null;
        dijit.byId("timeFimDiario").original_value = null;
        dijit.byId("timeIniDiario").reset();
        dijit.byId("timeFimDiario").reset();
        dijit.byId("timeIniDiario").set('value', null);
        dijit.byId("timeFimDiario").set('value', null);
        dijit.byId("timeIniDiario")._onChangeActive = true;
        dijit.byId("timeFimDiario")._onChangeActive = true;
        compSala.reset();
        if (limparSala)
            compSala.store = statusStore;
        dijit.byId("ckExterna").reset();
        cbProfessor.reset();
        cbProfessor.store = statusStore;
        dijit.byId("cbTipoAulaCad").set("value", TIPOAULADEFAULT);
        dijit.byId("ckFaltaProf").reset();
        compAvaliacao.reset();
        compAvaliacao.store = statusStore;
        dijit.byId("descObsCad").reset();
        dojo.byId("tagFalta").style.display = 'none';
        //cbStatus.reset();
        cbStatus.set("value", AULAAGENDADA)
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCamposFilhosDeProgramacao() {
    try {
        var compProf = dijit.byId("cbProfessor");
        dijit.byId("dtaAula").reset();
        dijit.byId("timeIniDiario")._onChangeActive = false;
        dijit.byId("timeFimDiario")._onChangeActive = false;
        dijit.byId("timeIniDiario").original_value = null;
        dijit.byId("timeFimDiario").original_value = null;
        dijit.byId("timeIniDiario").reset();
        dijit.byId("timeFimDiario").reset();
        dijit.byId("timeIniDiario").set('value', null);
        dijit.byId("timeFimDiario").set('value', null);
        dijit.byId("timeIniDiario")._onChangeActive = true;
        dijit.byId("timeFimDiario")._onChangeActive = true;
        dijit.byId("cbSalaCad").reset();
        dijit.byId("ckExterna").reset();
        dijit.byId("cbTipoAulaCad").set("value", TIPOAULADEFAULT);
        dijit.byId("ckFaltaProf").reset();
        dijit.byId("cbAvaliacao").reset();
        dijit.byId("descObsCad").reset();
        //cbStatus.reset();
        //cbStatus.set("value", AULAAGENDADA);

        if (!hasValue(dojo.byId('cd_usuario_professor').value)) {
            var statusProf = new dojo.store.Memory({
                data: null
            });
            compProf.store = statusProf;
            compProf.reset();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function findIsLoadComponetesNovoDiarioAula(xhr, ready, Memory, filteringSelect) {
    hideCarregando();
    showCarregando();
    xhr.get({
        url: Endereco() + "/api/coordenacao/componentesNovoDiarioAula",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (data != null && data.retorno != null) {
                data = data.retorno;
                //if (data.professores != null && data.professores.length > 0)
                //   criarOuCarregarCompFiltering("cbProfessor", data.professores, "", null, ready, Memory, filteringSelect, 'cd_pessoa', 'no_pessoa');
                if (data.salasDiario != null && data.salasDiario.length > 0)
                    criarOuCarregarCompFiltering("cbSalaCad", data.salasDiario, "", null, ready, Memory, filteringSelect, 'cd_sala', 'no_sala');
                if (data.tipoAtividadeExtra != null && data.tipoAtividadeExtra.length > 0)
                    criarOuCarregarCompFiltering("cbTipoAulaCad", data.tipoAtividadeExtra, "", TIPOAULADEFAULT, ready, Memory, filteringSelect, 'cd_tipo_atividade_extra', 'no_tipo_atividade_extra');
                if (data.mtvoFalta != null && data.mtvoFalta.length > 0)
                    criarOuCarregarCompFiltering("mtvoFalta", data.mtvoFalta, "", null, ready, Memory, filteringSelect, 'cd_motivo_falta', 'dc_motivo_falta');
            }
        }
        catch (e) {
            postGerarLog(e);
        }
        showCarregando();
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemDiarioPartial", error);
        showCarregando();
    });
}

function inserirIdTabsCadastroDiarioAula() {
    try {
        if (hasValue(dojo.byId("tabContainerDiarioAula_tablist_tabEventos")))
            dojo.byId("tabContainerDiarioAula_tablist_tabEventos").parentElement.id = "paiTabEventos";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataECreateTabEventos(cd_diario_aula) {
    dojo.ready(function () {
        try {
            var cd_turma = dojo.byId("cdTurmaCad").value;
            var dtAluna = dijit.byId("dtaAula").value;
            var dtAula = dojo.date.locale.parse(dojo.byId("dtaAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            //////// carrega os dados de Eventos
            dojo.xhr.post({   
                url: Endereco() + "/api/coordenacao/postEventosEAlunosDiario",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                preventCache: true,
                handleAs: "json",
                postData: dojox.json.ref.toJson(
                    {
                        cd_diario_aula: cd_diario_aula,
                        cd_turma: cd_turma,
                        dt_aula: dtAluna
                    })
            }).then(function (data) {
                try {
                    destroyCreateGridAluno();
                    apresentaMensagem("apresentadorMensagemDiarioPartial", null);
                    data = jQuery.parseJSON(data).retorno;
                    var cd_evento = null;
                    var dataAluno = null;
                    if (data != null) {
                        var atualizaAlunosEventos = false;
                        if (data.eventos != null && data.eventos.length > 0) {
                            if (hasValue(data) && hasValue(data.eventos) && data.eventos.length > 0 && hasValue(data.eventos[0]))
                                cd_evento = data.eventos[0].cd_evento;
                            criarOuCarregarCompFilteringChange("eventos", data.eventos, "", cd_evento, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_evento', 'no_evento');
                            atualizaAlunosEventos = parseInt(cd_diario_aula) > 0;
                        }
                        if (data.alunos != null && data.alunos.length > 0)
                            dataAluno = data.alunos;
                        var gridAluno = new dojox.grid.EnhancedGrid({
                            store: new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataAluno }) }),
                            structure: [
                                { name: "Aluno", field: "nomeAluno", width: "95%", styles: "min-width:180px;" },
                                { name: "-", field: 'selecionadoAluno', width: '5%', styles: "text-align: center;", formatter: formatCheckBoxEventoSelecionado },
                            ],
                            noDataMessage: msgNotRegEnc,
                            canSort: true,
                            selectionMode: "single"
                        }, "gridAlunosDiario");
                        gridAluno.startup();
                        gridAluno.on("StyleRow", function (row) {
                            try {
                                var item = gridAluno.getItem(row.index);
                                if (hasValue(item) && hasValue(item.nm_faltas) && item.nm_faltas == parseInt(dojo.byId("qtd_max_faltas_aluno").value))
                                    row.customClasses += " YellowRow";
                            }
                            catch (e) {
                                hideCarregando();
                                postGerarLog(e);
                            }
                        });
                        hideCarregando();
                        if (atualizaAlunosEventos)
                            dijit.byId("eventos").onChange(dijit.byId("eventos").value);
                    }
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            }, function (error) {
                hideCarregando();
                apresentaMensagem("apresentadorMensagemDiarioPartial", error);
            });
        }
        catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function loadDataCarga(cd_diario_aula) {
    dojo.ready(function () {
        try {
            var cd_turma = dojo.byId("cdTurmaCad").value;
            var dtAluna = dijit.byId("dtaAula").value;
            var dtAula = dojo.date.locale.parse(dojo.byId("dtaAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            //////// carrega os dados de Eventos
            dojo.xhr.post({
                url: Endereco() + "/api/coordenacao/postEventosEAlunosDiarioCarga",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                preventCache: true,
                handleAs: "json",
                postData: dojox.json.ref.toJson(
                    {
                        cd_diario_aula: cd_diario_aula,
                        cd_turma: cd_turma,
                        dt_aula: dtAluna
                    })
            }).then(function (data) {
                try {
                    apresentaMensagem("apresentadorMensagemDiarioPartial", null);
                    data = jQuery.parseJSON(data).retorno;
                    if (data != null) {
                        if (data.alunos != null && data.alunos.length > 0)
                            dataAluno = data.alunos;
                        var gridAlunoCarga = dijit.byId('gridAlunosCarga');
                        var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataAluno }) });
                        gridAlunoCarga.setStore(dataStore);
                        hideCarregando();
                    }
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            }, function (error) {
                hideCarregando();
                apresentaMensagem("apresentadorMensagemDiarioPartial", error);
            });
        }
        catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function formatCheckBoxEventoSelecionado(value, rowIndex, obj) {
    try {
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;

        if (rowIndex != -1)
            icon = "<input  id='" + id + "' class='formatCheckBox' /> ";

        setTimeout("configuraCheckBoxEventoSelecionado(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxEventoSelecionadoChange(rowIndex) {
    try {
        var gridGrupoUsuario = dijit.byId("gridAlunosDiario");
        gridGrupoUsuario._by_idx[rowIndex].item.selecionadoAluno = !gridGrupoUsuario._by_idx[rowIndex].item.selecionadoAluno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxEventoSelecionado(value, rowIndex, id) {
    try {
        var desabilitar = false;
        if (hasValue(id) && hasValue(dijit.byId(id)) && hasValue(dojo.byId(id)) && dojo.byId(id)["type"] != null && dojo.byId(id)["type"] != undefined && dojo.byId(id)["type"] == 'text')
            dijit.byId(id).destroy();
        var cd_diario_aula = dojo.byId("cd_diario_aula").value;
        var cdStatusDiario = dijit.byId("cbStatusCad").value;
        //Desabilitar os checkbox de alunos quando o diário de aula estiver cancelado.
        if (hasValue(cd_diario_aula) && cd_diario_aula > 0 && hasValue(cdStatusDiario) && cdStatusDiario == AULACANCELADA)
            desabilitar = true;
        if (hasValue(dojo.byId(id)) && dojo.byId(id).type == 'text')
            require(["dojo/ready", "dijit/form/CheckBox"], function (ready, CheckBox) {
                ready(function () {
                    try {
                        var checkBox = new dijit.form.CheckBox({
                            name: "checkBox",
                            disabled: desabilitar,
                            checked: value,
                            onChange: function (b) { checkBoxEventoSelecionadoChange(rowIndex); }
                        }, id);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                })
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function toogleTabEventos(bool) {
    try {
        if (bool) {
            if (hasValue(dojo.byId("paiTabEventos")))
                dojo.style("paiTabEventos", "display", "none");
        }
        else
            if (hasValue(dojo.byId("paiTabEventos")))
                dojo.style("paiTabEventos", "display", "");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadPopulaCompProfSubs(xhr, ready, Memory, FilteringSelect) {
    try {
        var compProfSubs = dijit.byId("ProfSubst");

        if (compProfSubs != null && compProfSubs.store.data.length == 0)
            xhr.get({
                url: Endereco() + "/api/professor/getAllProfessor",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    (hasValue(jQuery.parseJSON(data).retorno)) ? data = jQuery.parseJSON(data).retorno : jQuery.parseJSON(data);
                    criarOuCarregarCompFiltering("ProfSubst", data, "", null, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridAluno() {
    try {
        if (hasValue(dijit.byId("gridAlunosDiario"))) {
            dijit.byId("gridAlunosDiario").destroyRecursive();
            $('<div>').attr('id', 'gridAlunosDiario').attr('style', 'height:285px;').appendTo('#paigridAlunos');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(value, grid, ehLink, xhr, ready, Memory, FilteringSelect) {
    try {
        showCarregando();
        setarTabCadDiarioAula();
        limparDiarioAula(false);
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        if (value.cd_diario_aula > 0) {
            showEditDiarioAula(value.cd_diario_aula, xhr, ready, Memory, FilteringSelect);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showEditDiarioAula(cd_diario_aula, xhr, ready, Memory, filteringSelect) {
    try {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getDiarioAulaAndComponentes?cd_diario_aula=" + cd_diario_aula,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                apresentaMensagem("apresentadorMensagemDiarioPartial", null);
                if (data.retorno != null && data.retorno != null) {
                    var cbProgramacao = dijit.byId("cbProgramacao");
                    var dAula = data.retorno;
                    if (dAula.cd_professor != null && dAula.cd_professor > 0 && dAula.no_prof != null)
                        criarOuCarregarCompFiltering("cbProfessor", [{ id: dAula.cd_professor, name: dAula.no_prof }], "", dAula.cd_professor, ready, Memory, filteringSelect, 'id', 'name');
                    if (dAula.cd_tipo_aula != null && dAula.cd_tipo_aula > 0)
                        criarOuCarregarCompFiltering("cbTipoAulaCad", [{ id: dAula.cd_tipo_aula, name: dAula.desc_tipo_aula }], "", dAula.cd_tipo_aula, ready, Memory, filteringSelect, 'id', 'name');
                    if (dAula.salasDiario != null && dAula.salasDiario.length > 0) {
                        var cd_sala = null;
                        if (dAula.cd_sala != null && dAula.cd_sala > 0)
                            cd_sala = dAula.cd_sala;
                        criarOuCarregarCompFiltering("cbSalaCad", dAula.salasDiario, "", cd_sala, ready, Memory, filteringSelect, 'cd_sala', 'no_sala');
                    }
                    if (dAula.ProgramacaoTurma != null && hasValue(dAula.ProgramacaoTurma.no_programacao)) {
                        cbProgramacao._onChangeActive = false;
                        criarOuCarregarCompFiltering("cbProgramacao", [{ id: dAula.ProgramacaoTurma.cd_programacao_turma, name: dAula.ProgramacaoTurma.no_programacao }],
                            "", dAula.ProgramacaoTurma.cd_programacao_turma, ready, Memory, filteringSelect, 'id', 'name');
                        cbProgramacao._onChangeActive = true;
                    } else {
                        cbProgramacao._onChangeActive = false;
                        criarOuCarregarCompFiltering("cbProgramacao", [{ id: PROGRAMACAOSEMTURMA, name: labelCompProgramcaoSemTurma }],
                           "", PROGRAMACAOSEMTURMA, ready, Memory, filteringSelect, 'id', 'name');
                        cbProgramacao._onChangeActive = true;
                        //LBM liberando para edição os diários sem turma
                        dijit.byId("timeIniDiario").set("disabled", false);
                        dijit.byId("timeFimDiario").set("disabled", false);

                    }

                    if (dAula.avaliacoes != null && dAula.avaliacoes.length > 0) {
                        var cd_avaliacao = null;
                        if (dAula.cd_avaliacao != null && dAula.cd_avaliacao > 0)
                            cd_avaliacao = dAula.cd_avaliacao;
                        criarOuCarregarCompFiltering("cbAvaliacao", dAula.avaliacoes, "", cd_avaliacao, ready, Memory, filteringSelect, 'cd_avaliacao', 'descTipoENomeAvalicao');
                    }
                    loadDataDiarioAula(data.retorno, ready, Memory, filteringSelect);
                }else {
                    IncluirAlterar(0, 'divAlterarDiario', 'divIncluirDiario', 'divExcluirDiario', 'apresentadorMensagemDiarioPartial', 'divCancelarDiario', 'divClearDiario');
                    dijit.byId("cadDiarioAula").show();
                }
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemDiarioPartial", error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataDiarioAula(dAula, ready, Memory, filteringSelect) {
    try {
        dojo.byId("escoladiario").value = dAula.cd_pessoa_empresa;
        dojo.byId("cd_diario_aula").value = dAula.cd_diario_aula;
        dojo.byId("cdTurmaCad").value = dAula.cd_turma;
        dojo.byId("no_turma").value = dAula.no_turma;
        dijit.byId("ckExterna").set("checked", dAula.id_aula_externa);
        dijit.byId("cbStatusCad").set("value", dAula.id_status_aula);
        dijit.byId("descObsCad").set("value", dAula.tx_obs_aula);
        dojo.byId("nm_aula_turma").value = dAula.nm_aula_turma;
        var timeIniDiario = "";
        var timeFimDiario = "";
        //Configura o layout de acordo com status do profesor.
        if (dAula.id_falta_professor == FALTAPROFESSOR || dAula.id_falta_professor == FALTAJUSTIFICADAPORFESSOR) {
            dojo.byId("descFalta").value = dAula.dc_obs_falta;
            dijit.byId("ckFaltaProf").set("checked", true);
            if (dAula.cd_professor_substituto != null && dAula.cd_professor_substituto > 0 && dAula.no_substituto != null)
                criarOuCarregarCompFiltering("ProfSubst", [{ id: dAula.cd_professor_substituto, name: dAula.no_substituto }], "", dAula.cd_professor_substituto, ready, Memory, filteringSelect, 'id', 'name');
            //Motivo Falta
            if (dAula.mtvoFalta != null && dAula.mtvoFalta.length > 0 && dAula.cd_motivo_falta > 0)
                criarOuCarregarCompFiltering("mtvoFalta", dAula.mtvoFalta, "", dAula.cd_motivo_falta, ready, Memory, filteringSelect, 'cd_motivo_falta', 'dc_motivo_falta');
            if (dAula.id_falta_professor == FALTAJUSTIFICADAPORFESSOR)
                dijit.byId("ckJustificada").set("checked", true);
        }
        if (hasValue(dAula.dta_aula)) {
            var dtAula = dojo.date.locale.parse(dAula.dta_aula, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            dijit.byId('dtaAula')._onChangeActive = false;
            dijit.byId("dtaAula").set("value", dtAula);
            var diaSem = dojo.byId("diaSemana");
            diaSem.value = diaSemana(dtAula);
            dijit.byId('dtaAula')._onChangeActive = true;
        }


        var horaI = 0, horaF = 0, minI = 0, minFim = 0;
        if (hasValue(dAula.hr_inicial_aula)) {
            horaI = dAula.hr_inicial_aula.substring(0, 2);
            minI = dAula.hr_inicial_aula.substring(3, 5);
            timeIniDiario = "T" + horaI + ":" + minI + ":00";
        }
        if (hasValue(dAula.hr_final_aula)) {
            horaF = dAula.hr_final_aula.substring(0, 2);
            minFim = dAula.hr_final_aula.substring(3, 5);
            timeFimDiario = "T" + horaF + ":" + minFim + ":00";
        }
        //if (horaI > 0 && horaF > 0) {
            dijit.byId("timeIniDiario")._onChangeActive = false;
            dijit.byId("timeFimDiario")._onChangeActive = false;
            dijit.byId("timeIniDiario").set("value", timeIniDiario);
            dijit.byId("timeFimDiario").set("value", timeFimDiario);
            dijit.byId("timeIniDiario")._onChangeActive = true;
            dijit.byId("timeFimDiario")._onChangeActive = true;
        //}
        toogleTabEventos(false);
        IncluirAlterar(0, 'divAlterarDiario', 'divIncluirDiario', 'divExcluirDiario', 'apresentadorMensagemDiarioPartial', 'divCancelarDiario', 'divClearDiario');
        dijit.byId("cadDiarioAula").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataAlunosByEvents(cd_evento, cd_turma, dtAluna, cd_diario_aula, xhr) {
    dojo.ready(function () {
        showCarregando();
        try {
            xhr.post({
                url: Endereco() + "/api/coordenacao/postAlunosPorEventosDiario",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson(
                    {
                        cd_diario_aula: cd_diario_aula,
                        cd_turma: cd_turma,
                        dt_aula: dtAluna,
                        cd_evento_diario: cd_evento
                    })
            }).then(function (data) {
                try {
                    apresentaMensagem("apresentadorMensagemDiarioPartial", null);
                    data = jQuery.parseJSON(data).retorno;
                    if (hasValue(data) && hasValue(data.alunos) && hasValue(dijit.byId("gridAlunosDiario")))
                        dijit.byId("gridAlunosDiario").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: data.alunos }) }));
                    hideCarregando();
                }
                catch (e) {
                    hideCarregando();
                    postGerarLog(e);
                }
            });
        }
        catch (e) {
            hideCarregando();
            postGerarLog(e);
        }
    });
}

function mountDataForPostDiadrio() {
    try {
        seExistirSetarUltimoEvento();
        var eventosAlunos = null;
        var compProf = dijit.byId("cbProfessor");
        var compProfSubs = dijit.byId("ProfSubst");
        var compMtvoFalta = dijit.byId("mtvoFalta");
        var cdSala = dijit.byId("cbSalaCad");
        var cbAvaliacao = dijit.byId("cbAvaliacao");
        var gridAlunos = dijit.byId("gridAlunosDiario");
        var cdProgCad = dijit.byId("cbProgramacao").value;
        var cd_diario_aula = dojo.byId("cd_diario_aula").value;
        if (cdProgCad == PROGRAMACAOSEMTURMA)
            cdProgCad = null;
        if (hasValue(gridAlunos) && hasValue(gridAlunos.eventosAlunos))
            eventosAlunos = montarAlunosPorEvento(gridAlunos);
        var data = {
            cd_diario_aula: hasValue(cd_diario_aula) ? cd_diario_aula : 0,
            cd_pessoa_empresa: hasValue(dojo.byId("escoladiario").value) ? dojo.byId("escoladiario").value > 0 ? dojo.byId("escoladiario").value : dojo.byId('_ES0').value : dojo.byId('_ES0').value,
            cd_programacao_turma: cdProgCad,
            cd_turma: dojo.byId("cdTurmaCad").value,
            cd_professor: hasValue(compProf.value) ? compProf.value : null,
            cd_tipo_aula: dijit.byId("cbTipoAulaCad").value,
            cd_professor_substituto: hasValue(compProfSubs.value) ? compProfSubs.value : null,
            cd_motivo_falta: hasValue(compMtvoFalta.value) ? compMtvoFalta.value : null,
            cd_sala: hasValue(cdSala.value) ? cdSala.value : null,
            cd_avaliacao: hasValue(cbAvaliacao.value) ? cbAvaliacao.value : null,
            dt_aula: dojo.date.locale.parse(dojo.byId("dtaAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            hr_inicial_aula: dojo.byId("timeIniDiario").value,
            hr_final_aula: dojo.byId("timeFimDiario").value,
            id_aula_externa: dijit.byId("ckExterna").get("checked"),
            falta_professor: dijit.byId("ckFaltaProf").get("checked"),
            falta_justificada: dijit.byId("ckJustificada").get("checked"),
            dc_obs_falta: dojo.byId("descFalta").value,
            nm_aula_turma: dojo.byId("nm_aula_turma").value,
            tx_obs_aula: dojo.byId("descObsCad").value,
            eventos: eventosAlunos
        };
        return data;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abreCadastroDiarioAula(xhr, ready, Memory, FilteringSelect, turma, liberaCarregando) {
    dojo.ready(function () {
        try {
            if (liberaCarregando)
                showCarregando();
            configLayoutCadastro(NOVO);
            destroyCreateGridAluno();
            limparDiarioAula(true);
            IncluirAlterar(1, 'divAlterarDiario', 'divIncluirDiario', 'divExcluirDiario', 'apresentadorMensagemDiarioPartial', 'divCancelarDiario', 'divClearDiario');
            findIsLoadComponetesNovoDiarioAula(xhr, ready, Memory, FilteringSelect);
            if (hasValue(turma)) {
                dojo.byId('no_turma').value = turma.no_turma;
                dojo.byId('cdTurmaCad').value = turma.cd_turma;
                dojo.byId("escoladiario").value = turma.cd_pessoa_escola;
                retornarTurmaAcoesRelacionadas(ready, Memory, xhr, FilteringSelect, turma.cd_turma);
            }
            dijit.byId("cadDiarioAula").show();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function retornarTurmaAcoesRelacionadas(ready, Memory, xhr, FilteringSelect, cd_turma) {
    try {
        dijit.byId('ProfSubst').set('required', false);
        dijit.byId('cbProfessor').set('required', true);
        xhr.get({
            url: Endereco() + "/api/escola/getProgramacoesTurmasSemDiarioAula?cd_turma=" + cd_turma,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataProg) {
            try {
                showCarregando();
                dataProg = jQuery.parseJSON(dataProg);
                dataProg = dataProg.retorno;
                if (dataProg != null && dataProg.alunos != null && dataProg.alunos.length > 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, dataProg.nomeAlunosPendencia);
                    apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                }
                var programacaos = new Array();
                var cdProg = null;
                var compProg = dijit.byId("cbProgramacao");
                programacaos.push({ id: PROGRAMACAOSEMTURMA, name: "Sem Programação" });
                if (dataProg != null && hasValue(dataProg.progsTurma) && dataProg.progsTurma.length > 0) {
                    cdProg = dataProg.progsTurma[0].cd_programacao_turma;

                    $.each(dataProg.progsTurma, function (idx, val) {
                        programacaos.push({
                            id: val.cd_programacao_turma,
                            name: val.no_programacao,
                            nm_aula_programacao_turma: val.nm_aula_programacao_turma,
                            dc_programacao_turma: val.dc_programacao_turma,
                            dta_programacao_turma: val.dta_programacao_turma,
                            hr_inicial_programacao: val.hr_inicial_programacao,
                            hr_final_programacao: val.hr_final_programacao,
                            cd_sala_prog: val.cd_sala_prog,
                            aula_externa: val.aula_externa
                        });
                    });

                }
                var storeProg = new Memory({
                    data: programacaos
                });
                compProg.store = storeProg;
                compProg._onChangeActive = false;
                if (hasValue(cdProg))
                    compProg.set("value", cdProg);
                compProg._onChangeActive = true;
                dijit.byId('cbProfessor').set('disabled', false);
                dijit.byId('timeIniDiario').set("disabled", false);
                dijit.byId('timeFimDiario').set("disabled", false);
                //criarOuCarregarCompFiltering("cbProgramacao", programacaos, "", cdProg, ready, Memory, FilteringSelect, 'id', 'name');
                toogleTabEventos(false);
                if (dataProg != null && hasValue(dataProg.professoresHorariosTurma) && dataProg.professoresHorariosTurma.length > 0) {
                    var cdProfDefault = null;
                    if (dataProg.professoresHorariosTurma.length == 1)
                        cdProfDefault = dataProg.professoresHorariosTurma[0].cd_pessoa;

                    criarOuCarregarCompFiltering("cbProfessor", dataProg.professoresHorariosTurma, "", cdProfDefault, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                }
                // Se o usuário que estiver cadastrando o diário de aula for um professor, seu nome abreviado deverá vir preenchido automaticamente e não poderá ser alterado.
                //apresentaMensagem("apresentadorMensagemDiarioPartial", null);
                if (dojo.byId('cd_usuario_professor').value != "0") {
                    if (hasValue(dataProg.professoresHorariosTurma) && hasValue(dataProg.professoresHorariosTurma[0].no_fantasia)) {
                        dijit.byId('cbProfessor').set('disabled', true);
                        if (dojo.byId('cd_usuario_professor').value == dataProg.professoresHorariosTurma[0].cd_pessoa) {
                            dojo.byId('nome_usuario_professor').value = dataProg.professoresHorariosTurma[0].no_fantasia;
                            var professoresHorariosTurma = new Array();
                            professoresHorariosTurma.push({ cd_pessoa: dojo.byId('cd_usuario_professor').value, no_fantasia: dojo.byId('nome_usuario_professor').value });
                            criarOuCarregarCompFiltering("cbProfessor", professoresHorariosTurma, "", dojo.byId('cd_usuario_professor').value, ready, Memory, FilteringSelect, 'cd_pessoa', 'no_fantasia');
                        }
                        else {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHorarioDiferenteProfTurma);
                            apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                            showCarregando();
                            dijit.byId('timeIniDiario').set("disabled", true);//Desabilitando para travar e não deixar seguir o cadastro.
                            dijit.byId('timeFimDiario').set("disabled", true);
                            return false
                        }
                    } else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgTurmaSemProgramacaoComDiarioAulaLancado);
                        apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                    }
                }

                if (dataProg != null && hasValue(dataProg.avaliacoesTurma) && dataProg.avaliacoesTurma.length > 0)
                    criarOuCarregarCompFiltering("cbAvaliacao", dataProg.avaliacoesTurma, "", null, ready, Memory, FilteringSelect, 'cd_avalicao', 'descTipoENomeAvalicao');
                ready(function () {
                    preencherComponetesCadastroByProgramacao(cdProg);
                });
                showCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            hideCarregando();
            apresentaMensagem("apresentadorMensagemDiarioPartial", error);
        });
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function configLayoutCadastro(tipoCurd) {
    try {
        if (tipoCurd == EDIT) {
            dijit.byId("cbProgramacao").set("disabled", true);
            dijit.byId("dtaAula").set("disabled", true);
            dijit.byId("timeIniDiario").set("disabled", true);
            dijit.byId("timeFimDiario").set("disabled", true);
            dijit.byId("ckExterna").set("disabled", true);
            dijit.byId("cbProfessor").set("disabled", true);
            dijit.byId("cbTipoAulaCad").set("disabled", true);
            dijit.byId("ckFaltaProf").set("disabled", true);
            dijit.byId("ProfSubst").set("disabled", true);
            dijit.byId("ckJustificada").set("disabled", true);
            dijit.byId("no_turma").set("disabled", true);
            dijit.byId("pesTurmaFKCad").set("disabled", true);
        } else {
            dijit.byId("cbProgramacao").set("disabled", false);
            dijit.byId("dtaAula").set("disabled", false);
            dijit.byId("timeIniDiario").set("disabled", false);
            dijit.byId("timeFimDiario").set("disabled", false);
            dijit.byId("ckExterna").set("disabled", false);
            dijit.byId("cbProfessor").set("disabled", false);
            dijit.byId("cbTipoAulaCad").set("disabled", false);
            dijit.byId("ckFaltaProf").set("disabled", false);
            dijit.byId("descObsCad").set("disabled", false);
            dijit.byId("ProfSubst").set("disabled", false);
            dijit.byId("ckJustificada").set("disabled", false);
            dijit.byId("no_turma").set("disabled", false);
            dijit.byId("pesTurmaFKCad").set("disabled", false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function seExistirSetarUltimoEvento() {
    try {
        var compEvento = dijit.byId("eventos");
        var gridAlunos = dijit.byId("gridAlunosDiario");
        if (hasValue(compEvento) && hasValue(compEvento.value) && compEvento.value > 0) {
            if (hasValue(gridAlunos) && !hasValue(gridAlunos.eventosAlunos, true))
                gridAlunos.eventosAlunos = new Array();
            if (hasValue(gridAlunos) && hasValue(gridAlunos.store.objectStore.data))
                gridAlunos.eventosAlunos[compEvento.value] = gridAlunos.store.objectStore.data;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarAlunosPorEvento(gridAlunos) {
    try {
        var eventos = null;
        if (hasValue(gridAlunos) && hasValue(gridAlunos.eventosAlunos)) {
            eventos = new Array();
            for (var i in gridAlunos.eventosAlunos) {
                var evento = {
                    cd_evento: parseInt(i),
                    alunosEvento: gridAlunos.eventosAlunos[i]
                }
                eventos.push(evento);
            }
        }
        return eventos;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function chamarPesquisaDiarioAula() {
    try {
        var tipoOrigem = dojo.byId("id_origem_diarioAula").value;
        if (hasValue(tipoOrigem)) {
            if (tipoOrigem == PESQTURMADIARIOAULAFILTRO) {
                apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                pesquisarTurmaFK();
            }
            else if (hasValue(tipoOrigem) && tipoOrigem == PESQTURMADIARIOAULACADASTRO)
                pesquisarTurmaFKComAluno();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function dataDia() {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getData",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            var dtaHoje = dojo.date.locale.parse(data, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            var dtaDiario = dijit.byId("dtaAula").value;

            //Verifica se a data do diário de aula é maior que a data do dia:
            if (dojo.date.compare(dtaDiario, dtaHoje) > 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroLancarDiarioAposDataDia);
                apresentaMensagem("apresentadorMensagemDiarioPartial", mensagensWeb);
                dijit.byId("dtaAula")._onChangeActive = false;
                dijit.byId("dtaAula").reset();
                dijit.byId("dtaAula")._onChangeActive = true;
                return false;
            }
            else
                apresentaMensagem("apresentadorMensagemDiarioPartial", null);

        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemDiarioPartial", error);
        showCarregando();
    });
}