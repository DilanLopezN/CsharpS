var TELAMATRICULA = 1, TURMACADASTRO = 2, PESAVALIACAOTURMA = 3, DIARIOAULA = 4, CADMUDANCAINTERNAORI = 5,
    DESISTENCIA = 6, CNABBOLETO = 7, REPORT_DIARIO_AULA = 8, REPORTPROGAULASTURMA = 9, CADMUDANCAINTERNADES = 10,
    REPORTLISTAGEMANIVERSARIANTES = 11, REPORTCONTROLESALA = 12, POLITICADESCONTO = 13, REPORT_MEDIAS_ALUNOS = 14, REAJUSTE_ANUAL = 15,
    RELMATRICULA = 16, REPORT_TURMA_MATRICULA_MATERIAL = 17, BAIXAFINANCEIRAPESQ = 18, MAILMARKETING = 19, CADAVALIACAOTURMA = 20,
    MALADIRETA = 21;
var NORMAL = 1, PPT = 3, CONTROLEFALTA = 25, CONTROLEFALTACADASTRO = 26, REPORTCONTROLEFALTA = 27,
    AULAREPOSICAO = 28, AULAREPOSICAOCADASTRO = 29, TURMAFOLLOWUP = 30, REPORT_AVALIACAO = 31, REPORT_AVALIACAO_CONCEITO = 33;
var AULAREPOSICAOCADASTROTURMADESTINO = 32; SEM_DIARIO = 40; SEM_DIARIO_COM_ESCOLA = 41, DESISTENCIA_CARGA = 50, CARGA_HORARIA = 42,
    CARGA_HORARIA_COM_ESCOLA = 43;


function montarGridPesquisaTurmaFK(funcao) {
    require([
       "dojo/_base/xhr",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/data/ObjectStore",
       "dojo/store/Memory",
       "dijit/form/Button",
       "dojo/ready",
       "dojo/on",
       "dijit/form/FilteringSelect",
       "dojox/charting/Chart",
       "dojox/charting/plot2d/Columns",
       "dojox/charting/axis2d/Default",
       "dojox/charting/widget/Legend",
       "dojox/charting/themes/MiamiNice"
    ], function (xhr, EnhancedGrid, Pagination, ObjectStore, Memory, Button, ready, on, FilteringSelect, Chart,
                 Columns, Default, Legend, MiamiNice) {
        ready(function () {
            try {
                //findAllEmpresasUsuarioComboTurmaFK();
                /*--combo_escola_fk
                dojo.byId("trEscolaTurmaFiltroFk").style.display = "none";
                dojo.byId("lbEscolaTurmaFiltroFk").style.display = "none";
                require(['dojo/dom-style', 'dijit/registry'],
                    function (domStyle, registry) {

                        domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', 'none');
                    });*/

                apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                var store = null;
                store = new ObjectStore({ objectStore: new Memory({ data: null }) });

                if (hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                    return false;
                var gridPesquisaTurmaFK = new EnhancedGrid({
                    store: store,
                    structure: [
                        { name: "<input id='selecionaTodosFKTurma' style='display:none'/>", field: "turmaSelecionadaFK", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFK },
                        { name: "Nome", field: "no_turma", width: "15%", styles: "min-width:80px;" },
                        { name: "Aluno", field: "no_aluno", width: "17%", styles: "min-width:10%; max-width: 10%;" },
                        { name: "Curso", field: "no_curso", width: "10%" },
                        { name: "Carga Horária", field: "no_duracao", width: "7%", styles: "min-width:80px;" },
                        { name: "Produto", field: "no_produto", width: "8%", styles: "min-width:80px;" },
                        { name: "Data Início", field: "dtaIniAula", width: "9%", styles: "text-align: center;" },
                        { name: "Tipo", field: "descTipoTurma", width: "7%" },
                        { name: "Situação", field: "situacao", width: "8%" },
                        { name: "Nro. Alunos", field: "nro_alunos", width: "7%", styles: "text-align: center;" },
                        { name: "Vagas", field: "nm_vagas", width: "7%", styles: "text-align: center;" },
                        { name: "", field: "alunoAguardando", styles: "display:none;" }
                    ],
                    noDataMessage: msgNotRegEnc,
                    selectionMode: "single",
                    plugins: {
                        pagination: {
                            pageSizes: ["8", "16", "32", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "8",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridPesquisaTurmaFK");
                gridPesquisaTurmaFK.rowsPerPage = 5000;
                gridPesquisaTurmaFK.pagination.plugin._paginator.plugin.connect(gridPesquisaTurmaFK.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridPesquisaTurmaFK, 'cd_turma', 'selecionaTodosFKTurma');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridPesquisaTurmaFK, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (hasValue(dojo.byId('selecionaTodosFKTurma')) && dojo.byId('selecionaTodosFKTurma').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_turma', 'turmaSelecionadaFK', -1, 'selecionaTodosFKTurma', 'selecionaTodosFKTurma', 'gridPesquisaTurmaFK')", gridPesquisaTurmaFK.rowsPerPage * 3);
                    });
                });
                gridPesquisaTurmaFK.on("StyleRow", function (row) {
                    var tipoOrigem = dojo.byId("idOrigemCadastro").value;
                    if (hasValue(tipoOrigem) && tipoOrigem == TELAMATRICULA) {
                        var item = gridPesquisaTurmaFK.getItem(row.index);
                        //if (row.node.children[0].children[0].children[0].children[9].innerHTML == "true")
                        if (item != null && hasValue(item.alunoAguardando) && item.alunoAguardando)
                            row.customClasses += " GreenRow";
                    }
                });
                gridPesquisaTurmaFK.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 && Math.abs(col) != 10; };
                gridPesquisaTurmaFK.on("RowDblClick", function (evt) {
                });
                gridPesquisaTurmaFK.startup();
                dijit.byId("pesSituacaoFK").costumizado = false;
                new Button({
                    label: "Selecionar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                        try {
                            var tipoOrigem = parseInt(dojo.byId("idOrigemCadastro").value);
                            if (hasValue(tipoOrigem))
                                switch (tipoOrigem) {
                                    case TURMACADASTRO:
                                        retornarTurmaFKTurma();
                                        break;
                                    case REPORT_MEDIAS_ALUNOS:
                                    case REPORT_AVALIACAO:
                                    case REPORT_AVALIACAO_CONCEITO:
                                    case PESAVALIACAOTURMA:
                                    case CADAVALIACAOTURMA:
                                    case REPORTLISTAGEMANIVERSARIANTES:
                                    case REPORTCONTROLEFALTA:
                                    case REPORTCONTROLESALA:
                                    case POLITICADESCONTO:
                                    case REAJUSTE_ANUAL:
                                    case MAILMARKETING:
                                    case TURMAFOLLOWUP:
                                        var selecionaVariosRegistros = tipoOrigem == REAJUSTE_ANUAL ? true : false;
                                        retornarTurmaFK(selecionaVariosRegistros);
                                        break;
                                    case MALADIRETA:
                                        var selecionaVariosRegistros = tipoOrigem == REAJUSTE_ANUAL ? true : false;
                                        retornarTurmaFKMalaDireta(selecionaVariosRegistros);
                                        break;
                                    case DIARIOAULA:
                                        retornarTurmaFKDiarioAula();
                                        break;
                                    case CADMUDANCAINTERNADES:
                                    case CADMUDANCAINTERNAORI:
                                    case REPORT_TURMA_MATRICULA_MATERIAL:
                                        retornarTurmaFK();
                                        break;
                                    case DESISTENCIA:
                                        retornarTurmaFKDesistencia();
                                        break;
                                    case CNABBOLETO:
                                        retornarTurmaFKCnabBol();
                                        break;
                                    case REPORT_DIARIO_AULA:
                                        retornarTurmaFKReportDiarioAula();
                                        break;
                                    case RELMATRICULA:
                                        retornarTurmaFKRelMatricula();
                                        break;
                                    case CONTROLEFALTA:
                                        retornarTurmaFKControleFalta();
                                        break;
                                    case CONTROLEFALTACADASTRO:
                                        retornarTurmaFKControleFaltaCadastro();
                                        break;
                                    case REPORTPROGAULASTURMA:
                                        retornarTurmaFKReportProgAulasTurma();
                                        break;
                                    case BAIXAFINANCEIRAPESQ:
                                        retornarTurmaFKBaixaFinan();
                                        break;
                                    case AULAREPOSICAO:
                                        retornarTurmaFKAulaReposicao();
                                        break;
                                    case AULAREPOSICAOCADASTRO:
                                        retornarTurmaFKAulaReposicaoCadastro();
                                        break;
                                    case AULAREPOSICAOCADASTROTURMADESTINO:
	                                    retornarTurmaDestFKAulaReposicaoCadastro();
	                                    break;    
                                    case TELAMATRICULA:
                                        retornarTurmaFK();
                                        break;
                                    case SEM_DIARIO:
                                    case SEM_DIARIO_COM_ESCOLA:
                                        retornarTurmaFKSemDiario();
                                        break;
                                    case CARGA_HORARIA:
                                    case CARGA_HORARIA_COM_ESCOLA:
                                        retornarTurmaFKCartaHoraria();
                                        break;
                                    case DESISTENCIA_CARGA:
                                        retornarTurmaFKDesistenciaCarga();
                                        break;
                                }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "selecionaTurmaFK");

                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () { dijit.byId("proTurmaFK").hide(); }
                }, "fecharTurmaFK");
                new Button({
                    label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdAlunoFKTurmaFK').value = 0;
                            dojo.byId("noAlunoFKTurmaFK").value = "";
                            dijit.byId('limparAlunoPesFKTurmaFK').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparAlunoPesFKTurmaFK");
                decreaseBtn(document.getElementById("limparAlunoPesFKTurmaFK"), '40px');
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {

                    }
                }, "pesAlunoTurmaFK");
                decreaseBtn(document.getElementById("pesAlunoTurmaFK"), '18px');
                decreaseBtn(document.getElementById("pesquisarTurmaFK"), '32px');
                dijit.byId("tipoTurmaFK").on("change", function (e) {
                    try {
                        if (isNaN(eval(e))) {
                            dijit.byId("tipoTurmaFK").set('value', 0);
                        } else {
                            var pesSituacaoFK = dijit.byId("pesSituacaoFK");
                            var pesTurmasFilhasFK = dijit.byId("pesTurmasFilhasFK");
                            if (parseInt(dojo.byId("idOrigemCadastro").value) == AULAREPOSICAOCADASTRO)
                                dijit.byId('cbSalaFK').set("disabled", e != PPT)
                            if (e == PPT) {
                                
                                dojo.byId("lblTurmaFilhasFK").style.display = "";
                                dojo.byId("divPesTurmasFilhasFK").style.display = "";
                                pesSituacaoFK.set("value", 1);
                                if (dojo.byId("idOrigemCadastro").value == CADMUDANCAINTERNAORI)
                                    dijit.byId("pesTurmasFilhasFK").set("checked", true);
                                if (dojo.byId("idOrigemCadastro").value == CADMUDANCAINTERNADES)
                                    dijit.byId("pesTurmasFilhasFK").set("checked", false);
                                if (pesTurmasFilhasFK.checked) {
                                    if (!dijit.byId("pesSituacaoFK").costumizado)
                                        loadSituacaoTurmaFKTodas(dojo.store.Memory);
                                    else
                                        loadSituacaoTurmaFKAbertas(dojo.store.Memory);
                                }
                                else {
                                    
                                    loadSituacaoTurmaPPTFK(Memory);
                                }
                            } else {
                                if (parseInt(dojo.byId("idOrigemCadastro").value) == AULAREPOSICAOCADASTRO)
                                    dijit.byId('cbSalaFK').set("value", 2)
                                dojo.byId("lblTurmaFilhasFK").style.display = "none";
                                dojo.byId("divPesTurmasFilhasFK").style.display = "none";
                                if (!dijit.byId("pesSituacaoFK").costumizado)
                                    loadSituacaoTurmaFKTodas(dojo.store.Memory);
                                else
                                    loadSituacaoTurmaFKAbertas(dojo.store.Memory);
                                pesSituacaoFK.set("value", 1);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesTurmasFilhasFK").on("click", function (e) {
                    try {
                        if (dijit.byId("tipoTurmaFK").value == PPT && this.checked)
                            if (!dijit.byId("pesSituacaoFK").costumizado)
                                loadSituacaoTurmaFKTodas(dojo.store.Memory);
                            else
                                loadSituacaoTurmaFKAbertas(dojo.store.Memory);
                        else
                            if (dijit.byId("tipoTurmaFK").value == PPT && !this.checked)
                                loadSituacaoTurmaPPTFK(Memory);

                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("pesquisarTurmaFK").on("click", function () {
                    try {
                        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                        var tipo = parseInt(dojo.byId("idOrigemCadastro").value);
                        switch (tipo) {
                            case PESAVALIACAOTURMA: 
                                if (hasValue(pesquisarTurmaFK)) {
                                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                                    pesquisarTurmaFK();
                                }
                                break;
                            case CADAVALIACAOTURMA:
                                if (hasValue(pesquisarTurmaFKCadastroAvalicao)) {
                                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                                    pesquisarTurmaFKCadastroAvalicao();
                                }
                                break;
                            case DESISTENCIA: 
                                if (hasValue(chamarPesquisasDesistencia))
                                    chamarPesquisasDesistencia();
                                break;
                            case TELAMATRICULA:
                                if (dijit.byId('tipoTurmaFK').value == 3 && !dijit.byId("pesTurmasFilhasFK").checked) {
                                    if (dijit.byId('pesCursoFK').value == 0) {
                                        caixaDialogo(DIALOGO_AVISO, msgCursoObrigatorio, null);
                                        return false;
                                    }
                                }
                                if (hasValue(pesquisaTurmaDisponivelAluno))
                                    pesquisaTurmaDisponivelAluno();
                                break;
                            case DIARIOAULA: 
                                if (hasValue(chamarPesquisaDiarioAula))
                                    chamarPesquisaDiarioAula();
                                break;
                            case CADMUDANCAINTERNADES:
                            case CADMUDANCAINTERNAORI:
                                if (hasValue(chamarPesquisaMudancaInterna))
                                    chamarPesquisaMudancaInterna();
                                break;
                            case TURMACADASTRO:
                            case POLITICADESCONTO:
                                if (hasValue(pesquisarTurmaFK)) {
                                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                                    pesquisarTurmaFK();
                                }
                                break;
                            case REAJUSTE_ANUAL:
                                if (hasValue(pesquisarTurmaReajusteAnualFK))
                                    pesquisarTurmaReajusteAnualFK();
                                break;
                            case REPORT_TURMA_MATRICULA_MATERIAL:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFKRelTurmaMatriculaMaterial();
                                break;
                            case CNABBOLETO:
                            case REPORT_DIARIO_AULA:
                            case RELMATRICULA:
                            case REPORTPROGAULASTURMA:
                            case REPORTLISTAGEMANIVERSARIANTES:
                            case REPORT_MEDIAS_ALUNOS:
                            case REPORTCONTROLEFALTA:
                            case REPORTCONTROLESALA:
                            case BAIXAFINANCEIRAPESQ:
                            case MAILMARKETING:
                            case MALADIRETA:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK();
                                break;
                            case AULAREPOSICAOCADASTRO:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, AULAREPOSICAOCADASTRO);
                                break;  
                            case AULAREPOSICAOCADASTROTURMADESTINO:
	                            if (hasValue(pesquisarTurmaAulaReposicaoDestinoFK))
		                            pesquisarTurmaAulaReposicaoDestinoFK(-1, AULAREPOSICAOCADASTROTURMADESTINO);
                            case AULAREPOSICAO:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, AULAREPOSICAO);
                                break;
                            case CONTROLEFALTA:
                                if (hasValue(pesquisarTurmaControleFaltaFK))
                                    pesquisarTurmaControleFaltaFK();
                                break;
                            case CONTROLEFALTACADASTRO:
                                if (hasValue(pesquisarTurmaControleFaltaCadastroFK))
                                    pesquisarTurmaControleFaltaCadastroFK();
                                break;
                            case TURMAFOLLOWUP:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(0, TURMAFOLLOWUP, getDiaSemanaTurma());
                                break;
                            case REPORT_AVALIACAO:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, REPORT_AVALIACAO);
                                break;
                            case REPORT_AVALIACAO_CONCEITO:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, REPORT_AVALIACAO_CONCEITO);
                                break;
                            case SEM_DIARIO:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, SEM_DIARIO);
                                break;
                            case SEM_DIARIO_COM_ESCOLA:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, SEM_DIARIO_COM_ESCOLA);
                                break;
                            case CARGA_HORARIA:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, CARGA_HORARIA);
                                break;
                            case CARGA_HORARIA_COM_ESCOLA:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, CARGA_HORARIA_COM_ESCOLA);
                                break;
                            case DESISTENCIA_CARGA:
                                if (hasValue(pesquisarTurmaFK))
                                    pesquisarTurmaFK(-1, DESISTENCIA_CARGA);
                                break;
                            default: caixaDialogo(DIALOGO_INFORMACAO, "Nenhum tipo de retorno foi selecionado/encontrado.");
                                return false;
                                break;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                loadDataTipoProgFK(Memory);
                loadDataSala(Memory);
                FindIsLoadComponetesPesquisaTurmaFK(xhr, ready, Memory, FilteringSelect, funcao);
                loadTipoTurmaFK(Memory);
                montarLegendaTurmaFK(Chart, MiamiNice);
                adicionarAtalhoPesquisa(['nomTurmaFK', '_apelidoFK', 'tipoTurmaFK', 'pesCursoFK', 'pesDuracaoFK', 'pesProdutoFK', 'pesSituacaoFK', 'pesProfessorFK', 'sPogramacaoFK', 'cbSalaFK'], 'pesquisarTurmaFK', ready);
                //loadSituacaoTurmaFK(Memory);
                //if(hasValue(funcao))
                //    funcao.call();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBoxFK(value, rowIndex, obj) {
    try {
        var gridName = 'gridPesquisaTurmaFK';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFKTurma');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_turma', 'turmaSelecionadaFK', -1, 'selecionaTodosFKTurma', 'selecionaTodosFKTurma', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'turmaSelecionadaFK', " + rowIndex + ", '" + id + "', 'selecionaTodosFKTurma', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function FindIsLoadComponetesPesquisaTurmaFK(xhr, ready, Memory, filteringSelect, funcao) {
    //loadTipoTurmaFK(Memory);
    //loadSituacaoTurmaFK(Memory);
    xhr.get({
        url: Endereco() + "/api/turma/componentesPesquisaTurma",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.cursos))
                    criarOuCarregarCompFiltering("pesCursoFK", data.retorno.cursos, "", null, ready, Memory, filteringSelect, 'cd_curso', 'no_curso', MASCULINO);
                if (hasValue(data.retorno.produtos))
                    criarOuCarregarCompFiltering("pesProdutoFK", data.retorno.produtos, "", null, ready, Memory, filteringSelect, 'cd_produto', 'no_produto', MASCULINO);
                if (hasValue(data.retorno.duracoes))
                    criarOuCarregarCompFiltering("pesDuracaoFK", data.retorno.duracoes, "", null, ready, Memory, filteringSelect, 'cd_duracao', 'dc_duracao', MASCULINO);
                if (data.retorno.usuarioSisProf && !eval(Master())) {
                    criarOuCarregarCompFiltering("pesProfessorFK", data.retorno.professores, "", data.retorno.professores[0].cd_pessoa, ready, Memory, filteringSelect, 'cd_pessoa', 'no_pessoa');
                    dijit.byId("pesProfessorFK").set("disabled", true);
                } else if (hasValue(data.retorno.professores)) {
                    dijit.byId("pesProfessorFK").set("disabled", false);
                    criarOuCarregarCompFiltering("pesProfessorFK", data.retorno.professores, "", null, ready, Memory, filteringSelect, 'cd_pessoa', 'no_pessoa', MASCULINO);
                }
                if (hasValue(funcao))
                    funcao.call();
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, error);
    });
}

function pesquisarTurmaFK(cdProfDefault,origem, diaSemanaTurmaParam) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var dtInicial = "";
            var dtFinal = "";
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var ckOnLine = hasValue(dijit.byId("cbSalaFK").value) ? dijit.byId("cbSalaFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            var diaSemanaTurma = 0;
            if (hasValue(cdProfDefault) && cdProfDefault > 0)
                cdProfessor = cdProfDefault;
            var origemFK = hasValue(origem) ? origem : 0;
            if (hasValue(diaSemanaTurmaParam))
                diaSemanaTurma = diaSemanaTurmaParam;
            if (origemFK == REPORT_AVALIACAO || origemFK == REPORT_AVALIACAO_CONCEITO) {
                if (hasValue(dijit.byId('dtInicialAv')))
                    dtInicial = hasValue(dijit.byId('dtInicialAv').value) ? dojo.byId('dtInicialAv').value : "";
                if (hasValue(dijit.byId('dtFinalAv')))
                    dtFinal = hasValue(dijit.byId('dtFinalAv').value) ? dojo.byId('dtFinalAv').value : "";
            }
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearchFK?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                                             "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                                            "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno + "&origemFK=" + origemFK +
                                            "&dtInicial=" + dtInicial + "&dtFinal=" + dtFinal + "&cd_turma_PPT=null&semContrato=false&dataInicial=&dataFinal=" + "&cd_escola_combo_fk=0" + "&diaSemanaTurma=" + diaSemanaTurma +
                                            "&ckOnLine=" + ckOnLine,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];
            grid.setStore(dataStore);

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
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisarTurmaAulaReposicaoDestinoFK(cdProfDefault, origem) {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var dtInicial = "";
            var dtFinal = "";
            var cdCurso = hasValue(dijit.byId("pesCursoFK").value) ? dijit.byId("pesCursoFK").value : 0;
            var cdProduto = hasValue(dijit.byId("pesProdutoFK").value) ? dijit.byId("pesProdutoFK").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracaoFK").value) ? dijit.byId("pesDuracaoFK").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessorFK").value) ? dijit.byId("pesProfessorFK").value : 0;
            var cdProg = hasValue(dijit.byId("sPogramacaoFK").value) ? dijit.byId("sPogramacaoFK").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacaoFK").value) ? dijit.byId("pesSituacaoFK").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurmaFK").value) ? dijit.byId("tipoTurmaFK").value : 0;
            var cdAluno = dojo.byId("cdAlunoFKTurmaFK").value > 0 ? dojo.byId("cdAlunoFKTurmaFK").value : 0;
            var cdEstagioParam = dojo.byId("cdEstagioTurmaCad").value > 0 ? parseInt(dojo.byId("cdEstagioTurmaCad").value) : 0;
            var dt_programacao = hasValue(dijit.byId("dataAtividade").value) ? dojo.byId('dataAtividade').value : "";
            var cd_turma_origem = hasValue(dojo.byId("cdTurmaCad").value) ? dojo.byId("cdTurmaCad").value : 0;
            if (hasValue(cdProfDefault) && cdProfDefault > 0)
                cdProfessor = cdProfDefault;
            var origemFK = hasValue(origem) ? origem : 0;
            if (origemFK == REPORT_AVALIACAO) {
                if (hasValue(dijit.byId('dtInicialAv')))
                    dtInicial = hasValue(dijit.byId('dtInicialAv').value) ? dojo.byId('dtInicialAv').value : "";
                if (hasValue(dijit.byId('dtFinalAv')))
                    dtFinal = hasValue(dijit.byId('dtFinalAv').value) ? dojo.byId('dtFinalAv').value : "";
            }
            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/turma/getTurmaSearchAulaReposicaoDestinoFK?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                        "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                        "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                        "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno + "&origemFK=" + origemFK +
                        "&dtInicial=" + dtInicial + "&dtFinal=" + dtFinal + "&cd_turma_PPT=null&semContrato=false&dataInicial=&dataFinal=" +
                        "&ckOnLine=true" + "&dt_programacao=" + dt_programacao + "&cd_estagio=" + cdEstagioParam + "&cd_turma_origem=" + cd_turma_origem,
                    handleAs: "json",
                    preventCache: true,
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }
                ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaTurmaFK");
            grid.itensSelecionados = [];
            grid.setStore(dataStore);

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
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadTipoTurmaFK(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
            { name: "Todas", id: 0 },
            { name: "Regular", id: 1 },
            { name: "Personalizada", id: 3 }
            ]
        });
        dijit.byId("tipoTurmaFK").store = statusStoreTipo;
        dijit.byId("tipoTurmaFK").set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoTurmaFKTodas(Memory) {
    try {
        var statusStoreT = new Memory({
            data: [
            { name: "Turmas em Andamento", id: 1 },
            { name: "Turmas em Formação", id: 3 },
            { name: "Turmas Encerradas", id: 2 }
            ]
        });
        dijit.byId("pesSituacaoFK").store = statusStoreT;
        dijit.byId("pesSituacaoFK").set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoTurmaFKAbertas(Memory) {
    try {
        var statusStoreT = new Memory({
            data: [
            { name: "Turmas em Andamento", id: 1 },
            { name: "Turmas em Formação", id: 3 }
            ]
        });
        dijit.byId("pesSituacaoFK").store = statusStoreT;
        dijit.byId("pesSituacaoFK").set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadSituacaoTurmaPPTFK(Memory) {
    try {
        var statusStoreS = new Memory({
            data: [
            { name: "Turmas Ativas", id: 1 },
            { name: "Turmas Inativas", id: 2 }
            ]
        });

        dijit.byId("pesSituacaoFK").store = statusStoreS;
        dijit.byId("pesSituacaoFK").set("value", 1);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataTipoProgFK(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
            { name: "Todas", id: 0 },
            { name: "Geradas", id: 1 },
            { name: "Não geradas", id: 2 }
            ]
        });
        dijit.byId("sPogramacaoFK").store = statusStoreTipo;
        dijit.byId("sPogramacaoFK").set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataSala(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
                { name: "Todas", id: 0 },
                { name: "Presencial", id: 1 },
                { name: "OnLine", id: 2 }
            ]
        });
        dijit.byId("cbSalaFK").store = statusStoreTipo;
        dijit.byId("cbSalaFK").set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFiltrosTurmaFK() {
    try {
        if (hasValue(dijit.byId("gridPesquisaAluno")))
            limparPesquisaAlunoFK();
        var compProf = dijit.byId("pesProfessorFK");
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        dojo.byId("nomTurmaFK").value = "";
        dojo.byId("_apelidoFK").value = "";
        if (hasValue(dijit.byId("statusAluno")))
            dijit.byId("statusAluno").set("value", 0);
        dijit.byId("pesCursoFK").set("value", 0);
        dijit.byId("pesDuracaoFK").set("value", 0);
        dijit.byId("pesProdutoFK").set("value", 0);
        if(eval(Master()))
            dijit.byId("pesProfessorFK").set("value", 0);
        dijit.byId("pesSituacaoFK").set("value", 1);

        dijit.byId("tipoTurmaFK").set("disabled", false);
        dijit.byId("pesSituacaoFK").set("disabled", false);
        dijit.byId("pesProdutoFK").set("disabled", false);
        dijit.byId("pesCursoFK").set("disabled", false);
        dijit.byId("pesDuracaoFK").set("disabled", false);
        dojo.byId("noAlunoFKTurmaFK").value = "";
        dojo.byId("cdAlunoFKTurmaFK").value = "0";
        if (!(hasValue(compProf.value) || !compProf.disabled))
            compProf.set("value", 0);

        dijit.byId("sPogramacaoFK").set("value", 0);
        dijit.byId("cbSalaFK").set("value", 0);
        if (hasValue(gridPesquisaTurmaFK) && hasValue(gridPesquisaTurmaFK.itensSelecionados))
            gridPesquisaTurmaFK.itensSelecionados = [];
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarLegendaTurmaFK(Chart, MiamiNice) {
    dojo.ready(function () {
        try {
            var chart = new Chart("chartTurmaFK");
            var legendName = "legendTurmaFK";
            chart.setTheme(MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, legendName);

            chart.addSeries("Não está na turma", [1], { fill: "#fff" });
            chart.addSeries("Aguardando", [1], { fill: "#84fe88" }); //GreenRow
            chart.render();
            dijit.byId(legendName).refresh();
            if (dojo.byId("idOrigemCadastro").value == TELAMATRICULA) {
                dojo.byId("legendTurmaFK").style.visibility = "visible";
                dojo.byId("legendTurmaFK").style.display = "block";
            } else {
                dojo.byId("legendTurmaFK").style.visibility = "hidden";
                dojo.byId("legendTurmaFK").style.display = "none";
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirAlunoFKTurmaFK(setarTipoTurmaFK,origem) {
    try {
        dojo.byId("trAluno").style.display = "";
        limparPesquisaAlunoFK();
        //Configuração retorno fk de aluno na turma
        if (setarTipoTurmaFK)
            dojo.byId('tipoRetornoAlunoFK').value = TURMAFK;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
        pesquisaAlunoTurmaFKTurmaFK(origem);
        dijit.byId("proAluno").show();

    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornoPadraoAlunoFKParaTurmaFK(gridPesquisaAluno) {
    try {
        dojo.byId("cdAlunoFKTurmaFK").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
        dojo.byId("noAlunoFKTurmaFK").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
        dijit.byId('limparAlunoPesFKTurmaFK').set("disabled", false);
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarAlunoFKTurmaFK(pesquisaHabilitada) {
    var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
    if (pesquisaHabilitada)
        require([
                "dojo/store/JsonRest",
                "dojo/data/ObjectStore",
                "dojo/store/Cache",
                "dojo/store/Memory"
        ], function (JsonRest, ObjectStore, Cache, Memory) {
            try {
                myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/aluno/getAlunoSearchFKPesquisas?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" +
                        dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" +
                        dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked + "&cdSituacao=100&sexo=" +
                        sexo + "&semTurma=false&movido=false&tipoAluno=0&cd_pessoa_responsavel=0",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    idProperty: "cd_aluno"
                }), Memory({ idProperty: "cd_aluno" }));
                dataStore = new ObjectStore({ objectStore: myStore });
                var gridAluno = dijit.byId("gridPesquisaAluno");
                gridAluno.itensSelecionados = [];
                gridAluno.setStore(dataStore);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}

/*--combo_escola_fk
function findAllEmpresasUsuarioComboTurmaFK() {
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
                        loadSelect(dataRetorno, "escolaTurmaFiltroFK", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
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
}*/