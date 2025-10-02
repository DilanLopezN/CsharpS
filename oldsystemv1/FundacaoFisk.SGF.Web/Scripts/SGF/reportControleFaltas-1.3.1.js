var EnumsControleFaltas = {
    TIPO: {
        TODOS: 0,
        NORMAL: 1,
        PPT: 2
    },
    SITUACAO : 3,
    SITUACAOTURMA : {
        TURMA_ANDAMENTO : 1,
        TURMA_NOVA : 5,
        TURMA_FORMACAO : 3,
        TURMA_ENCERRADA : 2,
        TURMA_A_ENCERRAR : 4
    },
    REPORTCONTROLEFALTA: 12
}



//Situação da Turma

function montarMetodosRelatorioControleFaltas() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dijit/Tooltip",
    "dojo/on",
    "dijit/form/FilteringSelect",
    "dijit/registry",
    "dojo/data/ItemFileReadStore"
    ], function (ready, xhr, Memory, on, Button, JsonRest, ObjectStore, Cache, Tooltip, on, FilteringSelect, registry, ItemFileReadStore) {
        ready(function () {
            try {
                showCarregando();

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

                        dijit.byId("tipoTurma").set("value", 0);
                        dijit.byId("tipoTurma").set("disabled", false);

                        dijit.byId("pesSituacao").set("value", 1);
                        dijit.byId("pesSituacao").set("disabled", false);

                        dijit.byId("cbCurso").set("value", 0);
                        dijit.byId("cbCurso").set("disabled", false);

                        dijit.byId("cbProduto").set("value", 0);
                        dijit.byId("cbProduto").set("disabled", false);

                        dijit.byId("cbProfessor").set("value", 0);
                        dijit.byId("cbProfessor").set("disabled", false);


                        dojo.byId("noTurma").value = "";
                        dojo.byId("cd_turma").value = "";
                        dijit.byId('limparPesTurmaFK').set("disabled", true);
                    }
                }, "limparPesTurmaFK");
                decreaseBtn(document.getElementById("pesTurmaFK"), '18px');
                decreaseBtn(document.getElementById("limparPesTurmaFK"), '40px');

                new Button({ label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { emitirRelatorio(); } }, "relatorioControleFaltas");

                findIsLoadComponetesPesquisaReportTurma();
                //loadProduto('cbProduto', ItemFileReadStore);
                loadTipoTurma();
                loadSituacaoAluno(Memory, registry, ItemFileReadStore);

                setMenssageMultiSelect(EnumsControleFaltas.SITUACAO, 'cbSituacao');
                dijit.byId("cbSituacao").on("change", function (e) {
                    setMenssageMultiSelectOpcao(EnumsControleFaltas.SITUACAO, 'cbSituacao', true, 0);
                });

                dijit.byId("tipoTurma").on("change", function (tipo) {
                    try {
                        if (!hasValue(tipo, true) || tipo < 0)
                            dijit.byId("tipoTurma").set("value", EnumsControleFaltas.TIPO.TODOS);
                        else {
                            var pesSituacao = dijit.byId("pesSituacao");
                            if (tipo == EnumsControleFaltas.TIPO.PPT) {
                                loadSituacaoTurmaPPT(Memory);
                                pesSituacao.set("value", 1);
                                pesSituacao.set("disabled", false);
                            } else {
                                loadSituacaoTurma(Memory);
                                pesSituacao.set("value", 1);
                            }
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        });
    });
}

function toglleTpAvalEstagio(value) {
    if (value == true)
        document.getElementById('trAvalEstagio').style.display = 'block';
    else
        document.getElementById('trAvalEstagio').style.display = 'none';

}

function setoptions(item, option) {
    option.selected = true;
    require(["dojo/on"], function (on) {
        if (option.value >= 0) {
            on(item, "click", function (e) {
                var id = item.parent.id.split("_");
                var checkedMultiSelect = dijit.byId(id[0]);

                var contSelected = 0;
                for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                    if (checkedMultiSelect.options[i].selected == true) {
                        contSelected++;
                    }
                }

                if (option.value == 0) {
                    for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                        checkedMultiSelect.options[i].selected = option.selected;
                    }
                } else {
                    if (checkedMultiSelect.options.length - 1 == contSelected &&
                        checkedMultiSelect.options[0].selected == false) {
                        for (var i = 0; i < checkedMultiSelect.options.length; i++) {
                            checkedMultiSelect.options[i].selected = true;
                        }
                    } else {
                        checkedMultiSelect.options[0].selected = false;
                    }
                }
            });
        }
    });
}

function pesquisaAlunoFKRel() {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;

        var myStore = dojo.store.Cache(
            dojo.store.JsonRest({
                target: Endereco() + "/api/aluno/getAlunoPorTurmaSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&inicio=" +
                        document.getElementById("inicioAlunoFK").checked + "&origemFK=0" + "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value +
                        "&cdSituacao=0&sexo=" + sexo + "&cdTurma=0&opcao=0",
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
            dijit.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
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


function montarparametrosmulti(componente) {
    var produtos = "";
    if (!hasValue(dijit.byId(componente).value) || dijit.byId(componente).value.length <= 0) {
        produtos = "100";
    } else {

        if (dijit.byId(componente).value[0] == "0") {
            produtos = "0"
        } else {
            for (var i = 0; i < dijit.byId(componente).value.length; i++) {
                if (produtos == "") {
                    produtos = dijit.byId(componente).value[i];
                } else {
                    produtos = produtos + "|" + dijit.byId(componente).value[i];
                }
            }
        }
    }

    return produtos;
}



function emitirRelatorio() {
    try {

        this.tipoTurma = (dijit.byId("tipoTurma").value != null && dijit.byId("tipoTurma").value != undefined) ? dijit.byId("tipoTurma").value : null;
        this.cd_curso = (dijit.byId("cbCurso").value != null && dijit.byId("cbCurso").value != undefined) ? dijit.byId("cbCurso").value : null;
        this.cd_nivel = (dijit.byId("cbNivel").value != null && dijit.byId("cbNivel").value != undefined) ? dijit.byId("cbNivel").value : null;
        this.cd_produto = (dijit.byId("cbProduto").value != null && dijit.byId("cbProduto").value != undefined) ? dijit.byId("cbProduto").value : null;
        this.cd_professor = (dijit.byId("cbProfessor").value != null && dijit.byId("cbProfessor").value != undefined) ? dijit.byId("cbProfessor").value : null;

        this.cd_turma = (dojo.byId('cd_turma').value != null) ? dojo.byId('cd_turma').value : 0;
            //this.no_turma = hasValue(dojo.byId('cd_turma').value) ? dojo.byId('noTurma').value : "";
        this.cd_situacao_turma = (dijit.byId("pesSituacao").value != null && dijit.byId("pesSituacao").value != undefined) ? dijit.byId("pesSituacao").value : null;
        this.situacoes = montarparametrosmulti('cbSituacao');

            if (hasValue(dojo.byId("dtInicial").value)) {
                this.dataInicial = dojo.date.locale.parse(dojo.byId("dtInicial").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                this.dataInicial = dojo.date.locale.format(this.dataInicial,
                    { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
                this.dataInicialFormat = dojo.date.locale.format(dojo.date.locale.parse(dojo.byId("dtInicial").value,
		                { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
	                { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
            } else {
                this.dataInicial = null;
                this.dataInicialFormat = null;
            }

            if (hasValue(dojo.byId("dtFinal").value)) {
                this.dataFinal = dojo.date.locale.parse(dojo.byId("dtFinal").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                this.dataFinal = dojo.date.locale.format(this.dataFinal,
                    { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
                this.dataFinalFormat = dojo.date.locale.format(dojo.date.locale.parse(dojo.byId("dtFinal").value,
		                { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
	                { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
            } else {
                this.dataFinal = null;
                this.dataFinalFormat = null;
            }
            this.quebrarpagina = dojo.byId("ckPagina").checked ? true : false;

            var ControleFaltas = function (tipoTurma, cd_curso, cd_nivel, cd_produto, cd_professor, cd_turma, cd_situacao_turma, situacoes, dataInicial, dataFinal, quebrarpagina) {
                this.tipoTurma = tipoTurma;
                this.cd_curso = cd_curso;
                this.cd_nivel = cd_nivel;
                this.cd_produto = cd_produto;
                this.cd_professor = cd_professor;
                this.cd_turma = cd_turma;
                this.cd_situacao_turma = cd_situacao_turma;
                this.situacoes = situacoes;
                this.dataInicial = dataInicial;
                this.dataFinal = dataFinal;
                this.quebrarpagina = quebrarpagina;
            };

            /*Controle Faltas*/
            controleFaltas = new ControleFaltas(tipoTurma, cd_curso, cd_nivel, cd_produto, cd_professor, cd_turma, cd_situacao_turma, situacoes, dataInicial, dataFinal, quebrarpagina);

        //Valida se a data final e maior que a inicial
            if (hasValue(controleFaltas.dataInicial) && hasValue(controleFaltas.dataFinal)) {
                if (!testaPeriodoControleFaltas(dataInicialFormat, dataFinalFormat)) {
                    return false;
                } 
            }

            dojo.xhr.get({
                url: Endereco() + "/api/Coordenacao/GetUrlRptControleFaltas",
                preventCache: true,
                handleAs: "json",
                content: {
                    sort: "",
                    cd_tipo: controleFaltas.tipoTurma,
                    cd_curso: controleFaltas.cd_curso,
                    cd_nivel: controleFaltas.cd_nivel,
                    cd_produto: controleFaltas.cd_produto,
                    cd_professor: controleFaltas.cd_professor,
                    cd_turma: controleFaltas.cd_turma,
                    no_turma: controleFaltas.no_turma,
                    cd_sit_turma: controleFaltas.cd_situacao_turma,
                    cd_sit_aluno: controleFaltas.situacoes,
                    dt_inicial: controleFaltas.dataInicial,
                    dt_final: controleFaltas.dataFinal,
                    quebrarpagina: controleFaltas.quebrarpagina
                },
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                abrePopUp(Endereco() + '/Relatorio/RelatorioControleFaltas?' + data,
                    '1024px',
                    '750px',
                    'popRelatorio');
            },
            function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function showMessage(el, tipoErro, msg) {
    var mensagensWeb = new Array();
    apresentaMensagem(el, null);
    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msg);
    apresentaMensagem(el, mensagensWeb);
}


function findIsLoadComponetesPesquisaReportTurma() {
    dojo.xhr.get({
        url: Endereco() + "/api/Coordenacao/componentesPesquisaTurmaControleFalta",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
            showCarregando();
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagem", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.cursos))
                    criarOuCarregarCompFiltering("cbCurso", data.retorno.cursos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_curso', 'no_curso', MASCULINO);
                if (hasValue(data.retorno.produtos))
                    criarOuCarregarCompFiltering("cbProduto", data.retorno.produtos, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_produto', 'no_produto', MASCULINO);
                if (hasValue(data.retorno.niveis))
                    criarOuCarregarCompFiltering("cbNivel", data.retorno.niveis, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_nivel', 'dc_nivel', MASCULINO);
                if (data.retorno.usuarioSisProf == true) {
                    criarOuCarregarCompFiltering("cbProfessor", data.retorno.professores, "", data.retorno.professores[0].cd_pessoa, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_pessoa');
                    if (!eval(Master()))
                        dijit.byId("cbProfessor").set("disabled", true);
                } else if (hasValue(data.retorno.professores))
                    criarOuCarregarCompFiltering("cbProfessor", data.retorno.professores, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_pessoa', 'no_pessoa', MASCULINO);
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

function setProduto(produtos, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                var dados = [];
                $.each(produtos, function (index, val) {
                    dados.push({
                        "name": val.no_produto,
                        "id": val.cd_produto
                    });

                });

                var w = dijit.byId(field);
                //Adiciona a opção todos no checkedmultiselect
                dados.unshift({
                    "name": "Todos",
                    "id": 0
                });
                
                var statusStore = new Memory({
                    data: dados
                });
                dijit.byId(field).store = statusStore;
                dijit.byId(field).set("value", 0);
            } catch (e) {

            }
        });
};


function loadTipoTurma() {
    var statusStoreTipo = new dojo.store.Memory({
        data: [
            { name: "Todas", id: 0 },
            { name: "Regular", id: 1 },
            { name: "Personalizada", id: 2 }
        ]
    });
    dijit.byId("tipoTurma").store = statusStoreTipo;
    dijit.byId("tipoTurma").set("value", 0);
}

function loadSituacaoTurma(Memory) {
    try {
        var statusStore = new Memory({
            data: [
                { name: "Turmas em Andamento", id: 1 },
                { name: "Turmas Encerradas", id: 2 },
                //{ name: "Turmas em Formação", id: 3 }
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

function loadSituacaoAluno(Memory, registry, ItemFileReadStore) {
    var w = registry.byId('cbSituacao');
    var dados = [
        { name: "Matriculado", id: "1" },
        { name: "Rematriculado", id: "8" },
        { name: "Desistente", id: "2" },
        { name: "Encerrado", id: "4" },
        { name: "Movido", id: "0" }
    ];
    var storeTipoAluno = new ItemFileReadStore({
        data: {
            identifier: "id",
            label: "name",
            items: dados
        }
    });
    w.setStore(storeTipoAluno, []);
    w.options[0].selected = true;
    w.options[1].selected = true;
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

        this.tipoTurmaFiltro  =  dojo.byId("tipoTurma").value ;
        this.pesSituacaoFiltro = dojo.byId("pesSituacao").value ;
        this.cbCursoFiltro = dojo.byId("cbCurso").value;
        this.cbProdutoFiltro = dojo.byId("cbProduto").value;
        this.cbProfessorFiltro = dojo.byId("cbProfessor").value;

       var FiltrosTurma = function(tipoTurmaFiltro,
           pesSituacaoFiltro,
           cbCursoFiltro,
           cbProdutoFiltro,
           cbProfessorFiltro) {
           this.tipoTurmaFiltro = tipoTurmaFiltro;
           this.pesSituacaoFiltro = pesSituacaoFiltro;
           this.cbCursoFiltro = cbCursoFiltro;
           this.cbProdutoFiltro = cbProdutoFiltro;
           this.cbProfessorFiltro = cbProfessorFiltro;
       };
       filtrosTurma = new FiltrosTurma(tipoTurmaFiltro,
           pesSituacaoFiltro,
           cbCursoFiltro,
           cbProdutoFiltro,
           cbProfessorFiltro);

      
       if(hasValue(filtrarCombo("tipoTurmaFK", filtrosTurma.tipoTurmaFiltro))) {
           dijit.byId("tipoTurmaFK").set("value", filtrarCombo("tipoTurmaFK", filtrosTurma.tipoTurmaFiltro)[0].id);
       }
       if (hasValue(filtrarCombo("pesSituacaoFK", filtrosTurma.pesSituacaoFiltro))) {
           dijit.byId("pesSituacaoFK").set("value", filtrarCombo("pesSituacaoFK", filtrosTurma.pesSituacaoFiltro)[0].id);
       }
       if (hasValue(filtrarCombo("pesCursoFK", filtrosTurma.cbCursoFiltro))) {
           dijit.byId("pesCursoFK").set("value", filtrarCombo("pesCursoFK", filtrosTurma.cbCursoFiltro)[0].id);
       }
       if (hasValue(filtrarCombo("pesProdutoFK", filtrosTurma.cbProdutoFiltro))) {
           dijit.byId("pesProdutoFK").set("value", filtrarCombo("pesProdutoFK", filtrosTurma.cbProdutoFiltro)[0].id);
       }
       if (hasValue(filtrarCombo("pesProfessorFK", filtrosTurma.cbProfessorFiltro))) {
           dijit.byId("pesProfessorFK").set("value", filtrarCombo("pesProfessorFK", filtrosTurma.cbProfessorFiltro)[0].id);
       }
       
    }
    catch (e) {
        postGerarLog(e);
    }
}

function filtrarCombo(el, valueFilter) {
    var value = dijit.byId(el).store.data.filter(function (item) {
        return item.name === valueFilter;
    });
    return value;
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
                dijit.byId("tipoTurma").set("value", 0);
                dijit.byId("tipoTurma").set("disabled", true);

                dijit.byId("pesSituacao").set("value", 1);
                dijit.byId("pesSituacao").set("disabled", false);

                dijit.byId("cbCurso").set("value", 0);
                dijit.byId("cbCurso").set("disabled", true);

                dijit.byId("cbProduto").set("value", 0);
                dijit.byId("cbProduto").set("disabled", true);

                dijit.byId("cbProfessor").set("value", 0);
                dijit.byId("cbProfessor").set("disabled", true);

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

function createDate(stringDate) {
    var dateParts = stringDate.split("/");

    var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);

    return dateObject;
}

function testaPeriodoControleFaltas(dataInicial, dataFinal) {
    try {
        var retorno = true;

        var dtInicial = createDate(dataInicial);
        var dtFinal = createDate(dataFinal);

        if (dojo.date.compare(dtInicial, dtFinal) > 0) {
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