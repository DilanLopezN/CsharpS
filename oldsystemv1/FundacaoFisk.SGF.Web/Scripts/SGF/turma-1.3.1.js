var DOMINGO = 1; var SEGUNDA = 2; var TERCA = 3; var QUARTA = 4; var QUINTA = 5; var SEXTA = 6; var SABADO = 7;
var AUTOCOMPLETE_FECHADO = 0, AUTOCOMPLETE_ABERTO = 1, PESQUISAALUNOCAD = 1, PESQUISAALUNOPPT = 2, PESQUISAALUNOFILTRO = 3, MOVIDO = 0, DESISTENTE = 2, ATIVO = 1, REMATRICULADO = 8, AGUARDANDO = 9, NAOREMATRICULADO = 10;
var ENCERRADO = 4;
//Constante do campo hidden tipoTurmaCad
var TURMA = 1, TURMA_PPT = 2, TURMA_PPT_FILHA = 3, CLONARPPT = 1, CLONAREDUPLICARHORARIO = 2;
var RESOLVERMERGE = 0, ALTERARUMREGISTRO = 1, ADD_HORARIO = 0, EDIT_HORARIO = 1, DELETE_HORARIO = 3, NOVO = 1, EDIT = 2;
var PERSONALIZADO = 2, CADPRINCFILHAPPT = 1, CADPRINCFILHAPPTLINK = 2, VERIFTURMANORMAL = 0, VERIFITURMAFILHAPPT2MODAL = 1;
var MATRICULAPORTURMA = 1;
var INICIO_ENCERRAMENTO = 1, CONFIRMA_ENCERRAMENTO = 2;
var novaTurmaEnc = false, virada = false, LIBERAR_HABILITACAO_PROFESSOR = false;
var PROGRAMACAO_GERADA_MODELO = 2, PROGRAMACAO_GERADA_CURSO = 0, PROGRAMACAO_MANUAL = 1;
var TODOS = 0;
var TP_DESISTENCIA = 1, CANCELADESISTENCIA = 2;
var itensSelecionadosCheckGrid = [];
var itensSelecionadosCheckGridPPT = [];
var OUTRAESCOLA = false;

function mascarar(ready) {
    ready(function () {
        maskHour('#timeIni');
        maskHour('#timeFim');
        maskHour('#timeIniFilhaPPT');
        maskHour('#timeFimFilhaPPT');
    });
}

function populaDiasSemana(registry, ItemFileReadStore) {
    var w = registry.byId("cbDias");
    var retorno = [];
    retorno.push({ value_dia: 0 + "", no_dia: "Domingo" });
    retorno.push({ value_dia: 1 + "", no_dia: "Segunda" });
    retorno.push({ value_dia: 2 + "", no_dia: "Terça" });
    retorno.push({ value_dia: 3 + "", no_dia: "Quarta" });
    retorno.push({ value_dia: 4 + "", no_dia: "Quinta" });
    retorno.push({ value_dia: 5 + "", no_dia: "Sexta" });
    retorno.push({ value_dia: 6 + "", no_dia: "Sábado" });

    var store = new ItemFileReadStore({
        data: {
            identifier: "value_dia",
            label: "no_dia",
            items: retorno
        }
    });

    w.setStore(store, []);
    dijit.byId("cbDiaFilhaPPTs").setStore(store, []);
}

function montarLegenda() {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart("chart");
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, "legend");

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" }); //NormalRow
            chart.addSeries("Ocupado pela turma", [1], { fill: "#fc0000" }); //RedRow
            chart.addSeries("Ocupado pela sala", [1], { fill: "#FC5B00" }); //laranja
            //chart.addSeries("Disponíveis do professor ", [1], { fill: "#6ec2fd" }); //BlueRow
            chart.addSeries("Ocupados pelo professor", [1], { fill: "#84fe88" }); //GreenRow
            // chart.addSeries("Ocupados professores ", [1], { fill: "#fefa77" }); //YellowRow
            chart.render();
            dijit.byId("legend").refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function mudarLegendaPorTipoTurma(tipoTurma) {
    var ocupTurma = "Ocupado pela turma";
    var ocupSala = "Ocupado pela sala";
    var ocupProf = "Ocupados pelo professor";
    var ocupTurmaPPT = "Ocupado pela turma PPT";
    var compLegend = dijit.byId("legend");
    if (tipoTurma == TURMA_PPT_FILHA) {
        if (hasValue(compLegend) && hasValue(compLegend.chart) && hasValue(compLegend.chart.series) && compLegend.chart.series.length > 0) {
            var storeSeries = compLegend.chart.series.slice();
            $.each(storeSeries, function (index, value) {
                compLegend.chart.removeSeries(value.name);
            });
            compLegend.chart.addSeries("Ocupado pela turma", [1], { fill: "#fc0000" }); //RedRow
            compLegend.chart.addSeries("Ocupado pela turma PPT", [1], { fill: "#FC5B00" }); //laranja
            compLegend.chart.render();
            compLegend.refresh();
        }

    }
    else {
        if (hasValue(compLegend) && hasValue(compLegend.chart) && hasValue(compLegend.chart.series) && compLegend.chart.series.length > 0) {
            var storeSeries = compLegend.chart.series.slice();
            $.each(storeSeries, function (index, value) {
                compLegend.chart.removeSeries(value.name);
            });
            compLegend.chart.addSeries("Ocupado pela turma", [1], { fill: "#fc0000" }); //RedRow
            compLegend.chart.addSeries("Ocupado pela sala", [1], { fill: "#FC5B00" }); //laranja
            compLegend.chart.addSeries("Ocupados pelo professor", [1], { fill: "#84fe88" }); //GreenRow
            compLegend.chart.render();
            compLegend.refresh();
        }
    }
}

function montarLegendaTurmaFilhaPPT() {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart("chartFilhaPPT");
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, "legendFilhaPPT");

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" }); //NormalRow
            chart.addSeries("Ocupado pela turma", [1], { fill: "#fc0000" }); //RedRow
            chart.addSeries("Ocupado pela turma PPT", [1], { fill: "#FC5B00" }); //laranja
            chart.render();
            dijit.byId("legendFilhaPPT").refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarLegendaProgramacao() {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart("chartProgramacao");
            var legendName = "legendProgramacao";
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, legendName);

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" });
            chart.addSeries("Manual", [1], { fill: "#FC5B00" });
            chart.addSeries("Programada", [1], { fill: "#fff" });
            chart.addSeries("Diário de Aula", [1], { fill: "#fefa77" });
            chart.addSeries("Feriado", [1], { fill: "#84fe88" }); //GreenRow
            chart.addSeries("Cancelada", [1], { fill: "#be4848" }); //Red
            chart.render();
            dijit.byId(legendName).refresh();

            chart = new dojox.charting.Chart("chartProgramacaoPPT");
            legendName = "legendProgramacaoPPT";
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, legendName);

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" });
            chart.addSeries("Manual", [1], { fill: "#FC5B00" });
            chart.addSeries("Programada", [1], { fill: "#fff" });
            chart.addSeries("Diário de Aula", [1], { fill: "#fefa77" });
            chart.addSeries("Feriado", [1], { fill: "#84fe88" }); //GreenRow
            chart.addSeries("Cancelada", [1], { fill: "#be4848" }); //GreenRow
            chart.render();
            dijit.byId(legendName).refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarLegendaSituacaoAlunoPPT() {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart("chartAlunoPPT");
            var legendName = "legendaluno";
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, legendName);

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" });
            chart.addSeries("Desistente", [1], { fill: "#FC5B00" });
            chart.addSeries("Movido", [1], { fill: "#6495ed" });
            chart.render();
            dijit.byId(legendName).refresh();

           
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarLegendaSituacaoPaiAlunoPPTFilha() {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart("chartAlunosPPTFilha");
            var legendName = "legendPaiAlunoPPTFilha";
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, legendName);

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" });
            chart.addSeries("Desistente", [1], { fill: "#FC5B00" });
            chart.addSeries("Movido", [1], { fill: "#6495ed" });
            chart.render();
            dijit.byId(legendName).refresh();


        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarLegendaSituacaoPaiAlunosPPT() {
    dojo.ready(function () {
        try {
            var chart = new dojox.charting.Chart("chartAlunosPPT");
            var legendName = "legendPaiAlunoPPT";
            chart.setTheme(dojox.charting.themes.MiamiNice);
            chart.addAxis("x", { min: 0 });
            chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
            chart.addPlot("default", { type: "Columns", gap: 5 });
            chart.render();

            var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, legendName);

            //chart.addSeries("Matriculado", [1], { fill: "#FFF" });
            chart.addSeries("Desistente", [1], { fill: "#FC5B00" });
            chart.addSeries("Movido", [1], { fill: "#6495ed" });
            chart.render();
            dijit.byId(legendName).refresh();


        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function montarPadraoFiltroTurmaFK(Memory) {
    var tipoTurmaFK = dijit.byId("tipoTurmaFK");
    tipoTurmaFK.set("value", 3);
    tipoTurmaFK.set("disabled", true);
    dijit.byId("pesCursoFK").set("disabled", true);
    dijit.byId("pesTurmasFilhasFK").set("checked", false);
    dijit.byId("pesTurmasFilhasFK").set("disabled", true);
    dijit.byId("pesSituacaoFK").set("disabled", true);
    dijit.byId("sPogramacaoFK").set("disabled", true);
}



//Escola
function formatCheckBoxEscola(value, rowIndex, obj) {
    try {
        var gridName = 'gridEscolas';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosEscola');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_escola", grid._by_idx[rowIndex].item.cd_escola);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_escola', 'selecionadoEscola', -1, 'selecionaTodosEscola', 'selecionaTodosEscola', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_escola', 'selecionadoEscola', " + rowIndex + ", '" + id + "', 'selecionaTodosEscola', '" + gridName + "')", 2);
        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}


//function CheckBoxCalendario(value, rowIndex, obj) {
//    var gridNota = dijit.byId('gridProgramacao');
//    var icon;
//    var id = obj.field + '_Selected_' + rowIndex;

//    if (hasValue(dijit.byId(id)))
//        dijit.byId(id).destroy();

//    if (rowIndex != -1) icon = "<input id='" + id + "' /> ";
//    setTimeout("setCheckBoxId(" + value + ", " + rowIndex + ", " + id + ")", 1);
//    return icon;
//}

//function setCheckBoxId(value, rowIndex, id) {
//    require(["dijit/form/CheckBox", "dojo/domReady!"], function (CheckBox) {
//        return new CheckBox({
//            //name: "checkBox" + id,
//            value: value,
//            checked: value
//        }, id);
//    });
//}

function montarMetodosTurma(Permissoes) {
    require([
    "dojo/ready",
    "dojo/dom-construct",
    "dojo/_base/array",
    "dojo/_base/xhr",
    "dijit/registry",
    "dojo/data/ItemFileReadStore",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dojo/on",
    "dijit/form/Button",
    "dojox/json/ref",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
    "dojo/dom",
    "dijit/MenuSeparator",
    "dijit/Dialog",
    "dijit/form/DateTextBox",
    "dojo/domReady!"
    ], function (ready, domConstruct, array, xhr, registry, ItemFileReadStore, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect,dom, MenuSeparator) {
        ready(function () {
            try {
                //findAllEmpresasUsuarioComboFiltroEscolaTelaTurma();
                dojo.byId('trEscolaFiltroTelaTurma').style.display = "none";
                pesquisarEscolasVinculadasUsuario();
                pesquisarSalas();
                if (hasValue(Permissoes))
                    document.getElementById("setValuePermissoes").value = Permissoes;
                dojo.byId("descApresMsg").value = 'apresentadorMensagemTurma';
                dojo.byId("apresentadorMensagemFks").value = "apresentadorMensagemTurma";

                setMenssageMultiSelect(DIA, 'cbDias');
                dijit.byId("cbDias").on("change", function (e) {
                    setMenssageMultiSelect(DIA, 'cbDias');
                });
                var cdProf = 0;
                mascarar(ready);

                xhr.get({
                    url: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        if (data.retorno != null && data.retorno.Professor != null)
                        {
                            if (data.retorno.Professor.cd_pessoa > 0 && !data.retorno.Professor.id_coordenador) {
                                cdProf = data.retorno.Professor.cd_pessoa;
                                dijit.byId("ckIdProfTurmasAtuais").set("checked", true);
                                dojo.byId('nome_usuario_professor').value = data.retorno.no_fantasia;
                                if (eval(Master())) {
                                    cdProf = 0;
                                    dijit.byId("ckIdProfTurmasAtuais").set("checked", false);
                                }
                            }
                            
                        }
                        LIBERAR_HABILITACAO_PROFESSOR = data.retorno.id_liberar_habilitacao_professor;
                        dojo.byId('cd_usuario_professor').value = cdProf;
                        apresentaMensagem("apresentadorMensagemTurma", null);
                        populaDiasSemana(registry, ItemFileReadStore);
                        dijit.byId('tabContainerTurma').resize();
                        dijit.byId("pesCadCurso").set("required", true);
                        inserirIdTabsCadastro();
                        criarElementosTagAlunos(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest, Pagination,
                                                FilteringSelect, registry, array, ready);
                        criarElementosTagProfessores(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest, Pagination);
                        criarElementosTagAlunosPPT(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest, ready, FilteringSelect, Pagination);
                        var dtInicial = dojo.date.locale.parse(dojo.byId("dtInicial").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                        var dtFinal = dojo.date.locale.parse(dojo.byId("dtFinal").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                        var dias = '0000000';

                        var cd_search_sala = hasValue(dijit.byId('cbSearchSala').value) ? dijit.byId('cbSearchSala').value : 0;
                        var cd_search_sala_online = hasValue(dijit.byId('cbSearchSalaOnLine').value) ? dijit.byId('cbSearchSalaOnLine').value : 0;
                        var ckSearchSemSala = dijit.byId('ckSemSala').checked ? true: false;
                        var ckSearchSemAluno = dijit.byId('ckSemAluno').checked ? true : false;

                        var myStore =
                        Cache(
                                JsonRest({
                                    target: Endereco() + "/api/turma/getTurmaSearch?descricao=&apelido=&inicio=false&tipoTurma=" + parseInt(0) + "&cdCurso=" + parseInt(0) + "&cdDuracao=" + parseInt(0) +
                                        "&cdProduto=" + parseInt(0) + "&situacaoTurma=" + parseInt(1) + "&cdProfessor=" + parseInt(cdProf) + "&prog=0" + "&turmasFilhas=false&cdAluno=0&dtInicial=&dtFinal=&cd_turma_PPT=0&"+
                                        "semContrato=false&ProfTurmasAtuais=" + document.getElementById('ckIdProfTurmasAtuais').checked + "&cd_escola_combo=0&ckOnLine=0&dias=" + dias + 
                                        "&cd_search_sala=" + cd_search_sala + "&cd_search_sala_online=" + cd_search_sala_online + "&ckSearchSemSala=" + ckSearchSemSala + "&ckSearchSemAluno=" + ckSearchSemAluno,
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Authorization": Token() }
                                }), Memory({}));  // + "&cd_escola_combo=" + (dijit.byId("escolaFiltroTelaTurma").value || dojo.byId("_ES0").value)

                        var gridTurma = new EnhancedGrid({
                            //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                            store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "turmaSelecionada", width: "3%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                                { name: " ", field: "dc_escola_origem", width: "3%", styles: "text-align: center;", formatter: formatImgCompartilhada },
                                { name: "Nome", field: "no_turma", width: "20%", styles: "min-width:80px;" },
                                { name: "Apelido", field: "no_apelido", width: "14%", styles: "min-width:80px;" },
                                { name: "Aluno", field: "no_aluno", width: "14%", styles: "min-width:10%; max-width: 10%;" },
                                { name: "Curso", field: "no_curso", width: "10%" },
                                { name: "Carga Horária", field: "no_duracao", width: "6%", styles: "min-width:80px;" },
                                { name: "Produto", field: "no_produto", width: "8%", styles: "min-width:80px;" },
                                { name: "Data Início", field: "dtaIniAula", width: "8%", styles: "text-align: center;" },
                                { name: "Professor", field: "no_professor", width: "8%", styles: "text-align: center;" },
                                { name: "Tipo", field: "descTipoTurma", width: "8%" },
                                { name: "Situação", field: "situacao", width: "8%" },
                                { name: "Nro. Alunos", field: "nro_alunos", width: "6%", styles: "text-align: center;" }
                            ],
                            noDataMessage: msgNotRegEncFiltro,
                            selectionMode: "single",
                            plugins: {
                                pagination: {
                                    pageSizes: ["17", "34", "68", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "17",
                                    gotoButton: true,
                                    /*page step to be displayed*/
                                    maxPageStep: 5,
                                    /*position of the pagination bar*/
                                    position: "button",
                                    plugins: { nestedSorting: false }
                                }
                            }
                        }, "gridTurma");
                        showCarregando();
                        gridTurma.itensSelecionados = new Array();
                        gridTurma.pagination.plugin._paginator.plugin.connect(gridTurma.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            verificaMostrarTodos(evt, gridTurma, 'cd_turma', 'selecionaTodos');
                        });

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridTurma, "_onFetchComplete", function () {
                                // Configura o check de todos:
                                if (dojo.byId('selecionaTodos').type == 'text')
                                    setTimeout("configuraCheckBox(false, 'cd_turma', 'turmaSelecionada', -1, 'selecionaTodos', 'selecionaTodos', 'gridTurma')", gridTurma.rowsPerPage * 3);
                            });
                        });
                        gridTurma.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 9 && Math.abs(col) != 10 && Math.abs(col) != 11; };
                        gridTurma.on("RowDblClick", function (evt) {
                            try {
                                var idx = evt.rowIndex,
                                    item = this.getItem(idx),
                                    store = this.store;
                                pesquisarEscolasVinculadasUsuario();
                                gridTurma.itemSelecionado = item;
                                dojo.byId('abriuEscola').value = false;
                                setarTabCad();
                                destroyCreateGridHorario();
                                destroyCreateProgramacao();
                                destroyCreateGridAvaliacaoAluno();
                                virada = false;
                                keepValues(item, dijit.byId("gridTurma"), false, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton);
                                IncluirAlterar(0, 'divAlterarTurma', 'divIncluirTurma', 'divExcluirTurma', 'apresentadorMensagemTurma', 'divCancelarTurma', 'divClearTurma');
                                popularGridEscolaTurma();
                                dijit.byId("cadTurma").show();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);
                        //Desabilita a coluna de aluno
                        gridTurma.layout.setColumnVisibility(4, false)
                        gridTurma.startup();
                        //Propriedade. serve para informar se foi clicado na acão relacionada "turmas filhas"
                        gridTurma.turmasFilhas = false;
                        var gridHorarioProfessor = new EnhancedGrid({
                            store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodosHorarioProf' style='display:none'/>", field: "horarioProfSelecionado", width: "10%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxHorarioProf },
                                { name: "Hora Inicial", field: "dt_hora_ini", width: "30%", styles: "min-width:80px;" },
                                { name: "Hora Final", field: "dt_hora_fim", width: "30%", styles: "min-width:80px;" },
                                { name: "Dia da Semana", field: "dia_semana", width: "30%", styles: "min-width:80px;" }
                            ],
                            noDataMessage: msgNotRegEnc,
                            selectionMode: "single",
                            plugins: {
                                pagination: {
                                    pageSizes: ["7", "14", "30", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "7",
                                    gotoButton: true,
                                    /*page step to be displayed*/
                                    maxPageStep: 5,
                                    /*position of the pagination bar*/
                                    position: "button",
                                    plugins: { nestedSorting: false }
                                }
                            }
                        }, "gridHorarioProfessor");
                        gridHorarioProfessor.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 && Math.abs(col) != 10; };
                        gridHorarioProfessor.startup();

                        /*Grid Escola*/
                        var gridEscolas = new EnhancedGrid({
                            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodosEscola' style='display:none'/>", field: "selecionadoEscola", width: "25px", styles: "text-align: center;", formatter: formatCheckBoxEscola },
                                { name: "Escola", field: "dc_reduzido_pessoa", width: "98%" }
                            ],
                            plugins: {
                                pagination: {
                                    pageSizes: ["17", "34", "68", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "17",
                                    gotoButton: true,
                                    maxPageStep: 4,
                                    position: "button",
                                    plugins: { nestedSorting: true }
                                }
                            }
                        }, "gridEscolas");
                        gridEscolas.canSort = function (col) { return Math.abs(col) != 1; };
                        gridEscolas.startup();

                        new Button({
                                label: "Incluir",
                                iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                                onClick: function () {
                                    try {
                                        if (!hasValue(dijit.byId("gridPesquisaPessoaTurma")))
                                            montargridPesquisaPessoaTurma(function () {
                                                dojo.query("#_nomePessoaTurmaFK").on("keyup",
                                                    function (e) {
                                                        if (e.keyCode == 13) pesquisarEscolasFK();
                                                    });
                                                dijit.byId("pesqPessoaTurma").on("click",
                                                    function (e) {
                                                        apresentaMensagem("apresentadorMensagemProPessoa", null);
                                                        pesquisarEscolasFK();
                                                    });
                                                abrirPessoaEscolaFK(false);
                                            });
                                        else
                                            abrirPessoaEscolaFK(false);
                                    } catch (e) {
                                        postGerarLog(e);
                                    }
                                }
                            },
                            "incluirEscolaFK");

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Excluir",
                            onClick: function () {
                                deletarItemSelecionadoGrid(dojo.store.Memory, dojo.data.ObjectStore, 'cd_escola', gridEscolas);
                            }
                        });
                        menu.addChild(menuTodosItens);

                        if (!hasValue(dijit.byId('todosItensEscola'))) {
                            var button = new DropDownButton({
                                label: "Ações Relacionadas",
                                name: "todosItensEscola",
                                dropDown: menu,
                                id: "todosItensEscola"
                            });
                            dom.byId("linkSelecionadosEscola").appendChild(button.domNode);
                        }//if

                        //criação do botões pesquisa principal
                        new Button({
                            label: "Limpar", iconClass: '', Disabled: true, onClick: function () {
                                dojo.byId('cdAlunoPesTurma').value = 0;
                                dojo.byId("noAluno").value = "";
                                dijit.byId('limparAlunoPes').set("disabled", true);
                            }
                        }, "limparAlunoPes");
                        if (hasValue(document.getElementById("limparAlunoPes"))) {
                            document.getElementById("limparAlunoPes").parentNode.style.minWidth = '40px';
                            document.getElementById("limparAlunoPes").parentNode.style.width = '40px';
                        }
                        new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarTurma(true); } }, "pesquisarTurma");
                        decreaseBtn(document.getElementById("pesquisarTurma"), '32px');
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                        montarGridPesquisaTurmaFK(function () {
                                            abrirPesquisaTurmaFK(Memory);
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
                                        abrirPesquisaTurmaFK(Memory);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesProTurmaFK");
                        btnPesquisar(document.getElementById("pesProTurmaFK"));

                        new Button({
                            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                                salvarTurma(xhr, ref);
                            }
                        }, "incluirTurma");
                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                try {
                                    destroyCreateGridHorario();
                                    destroyCreateProgramacao();
                                    dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
                                    if (hasValue(dojo.byId("cd_turma_enc").value) && dojo.byId("cd_turma_enc").value > 0)
                                        virada = true;
                                    else
                                        virada = false;
                                    keepValues(null, gridTurma, null, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton);
                                    popularGridEscolaTurma();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "cancelarTurma");
                        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTurma").hide(); } }, "fecharTurma");
                        new Button({
                            label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                                alterarTurma(xhr, ref);
                            }
                        }, "alterarTurma");
                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarTurmas(null, xhr, ref) });
                            }
                        }, "deleteTurma");
                        new Button({
                            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                                limparCadTurma(Memory);
                            }
                        }, "limparTurma");
                        new Button({
                            label: getNomeLabelRelatorio(),
                            iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                            onClick: function () {
                                try {
                                    var gridTurma = dijit.byId("gridTurma");
                                    var cdCurso = hasValue(dijit.byId("pesCurso").value) ? dijit.byId("pesCurso").value : 0;
                                    var cdProduto = hasValue(dijit.byId("pesProduto").value) ? dijit.byId("pesProduto").value : 0;
                                    var cdDuracao = hasValue(dijit.byId("pesDuracao").value) ? dijit.byId("pesDuracao").value : 0;
                                    var cdProfessor = hasValue(dijit.byId("pesProfessor").value) ? dijit.byId("pesProfessor").value : 0;
                                    var cdProg = hasValue(dijit.byId("sProgramacao").value) ? dijit.byId("sProgramacao").value : 0;
                                    var cdAluno = hasValue(dojo.byId('cdAluno').value) ? parseInt(dojo.byId('cdAluno').value) : 0;
                                    var cd_turma_PPT = 0;
                                    var ckOnLine = dijit.byId('ckOnLine').value;


                                    var cd_search_sala = hasValue(dijit.byId('cbSearchSala').value) ? dijit.byId('cbSearchSala').value : 0;
                                    var cd_search_sala_online = hasValue(dijit.byId('cbSearchSalaOnLine').value) ? dijit.byId('cbSearchSalaOnLine').value : 0;
                                    var ckSearchSemSala = dijit.byId('ckSemSala').checked ? true : false;
                                    var ckSearchSemAluno = dijit.byId('ckSemAluno').checked ? true : false;

                                    if (gridTurma.turmasFilhas) {
                                        if (hasValue(gridTurma.itensSelecionados) && gridTurma.itensSelecionados.length > 0)
                                            var turma = gridTurma.itensSelecionados[0];

                                        if (hasValue(turma)) {
                                            if ((!hasValue(turma.cd_turma_ppt) && turma.id_turma_ppt == true) || (turma.cd_turma_ppt == 0 && turma.id_turma_ppt == true))
                                                cd_turma_PPT = turma.cd_turma;
                                        }
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
                                    target: Endereco() + "/api/turma/getTurmaSearch?descricao=&apelido=&inicio=false&tipoTurma=" + parseInt(0) + "&cdCurso=" + parseInt(0) + "&cdDuracao=" + parseInt(0) +
                                        "&cdProduto=" + parseInt(0) + "&situacaoTurma=" + parseInt(1) + "&cdProfessor=" + parseInt(cdProf) + "&prog=0" + "&turmasFilhas=false&cdAluno=0&dtInicial=&dtFinal=&cd_turma_PPT=0&" +
                                        "semContrato=false&ProfTurmasAtuais=" + document.getElementById('ckIdProfTurmasAtuais').checked + "&cd_escola_combo=0&ckOnLine=" + ckOnLine + "&dias=" + dias + 
                                        "&cd_search_sala=" + cd_search_sala + "&cd_search_sala_online=" + cd_search_sala_online + "&ckSearchSemSala=" + ckSearchSemSala + "&ckSearchSemAluno=" + ckSearchSemAluno,

                                    xhr.get({
                                        url: Endereco() + "/api/turma/GeturlrelatorioTurma?" + getStrGridParameters('gridTurma') + "descricao=" + document.getElementById("_nomTurma").value +
                                            "&apelido=" + document.getElementById("_apelidoT").value +
                                            "&inicio=" + document.getElementById("inicioTurma").checked + "&tipoTurma=" + dijit.byId("tipoTurma").value +
                                                           "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + dijit.byId("pesSituacao").value +
                                                           "&cdProfessor=" + cdProfessor + "&prog=" + cdProg +
                                                           "&turmasFilhas=" + document.getElementById("pesTurmasFilhas").checked + "&cdAluno=" + cdAluno + "&dtInicial=" + dojo.byId("dtInicial").value +
                                                           "&dtFinal=" + dojo.byId("dtFinal").value + "&cd_turma_PPT=" + cd_turma_PPT + "&semContrato=" + document.getElementById('semContrato').checked +
                                            "&ProfTurmasAtuais=" + document.getElementById('ckIdProfTurmasAtuais').checked + "&ckOnLine=" + dijit.byId('ckOnLine').value + "&dias=" + dias +
                                            "&cd_search_sala=" + cd_search_sala + "&cd_search_sala_online=" + cd_search_sala_online + "&ckSearchSemSala=" + ckSearchSemSala + "&ckSearchSemAluno=" + ckSearchSemAluno,
                                        preventCache: true,
                                        handleAs: "json",
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                    }).then(function (data) {
                                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                    },
                                    function (error) {
                                        apresentaMensagem('apresentadorMensagemTurma', error);
                                    });
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "relatorioTurma");
                        new Button({
                            label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                                try {
                                    OUTRAESCOLA = false;
                                    novaTurmaEnc = false;
                                    dijit.byId('gridTurma').itemSelecionado = null;
                                    dojo.byId('abriuEscola').value = false;
                                    dijit.byId("btnNovoProfessor").set("disabled", false);
                                    dijit.byId("btnNovoProfessorFilhaPPT").set("disabled", false);
                                    pesquisarEscolasVinculadasUsuario();
                                    setarTabCad();
                                    limparCadTurma(Memory);
                                    IncluirAlterar(1, 'divAlterarTurma', 'divIncluirTurma', 'divExcluirTurma', 'apresentadorMensagemTurma', 'divCancelarTurma', 'divClearTurma');
                                    FindIsLoadComponetesNovaTurma(xhr, ready, Memory, FilteringSelect);
                                    destroyCreateGridHorario();
                                    destroyCreateProgramacao();
                                    dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
                                    if (dijit.byId("idTurmaPPTPai").checked == false) {
                                        if (!eval($("#tabAlunosRemovida").val())) {
                                            dijit.byId('tagAlunosPPT').toggleable = false;
                                            dojo.byId("tagAlunosPPT").style.display = "none";
                                            $("#tabAlunosRemovida").val(true);
                                        }
                                    }
                                    confLayoutBotInclAluno();
                                    destroyCreateGridAvaliacaoAluno();
                                    dijit.byId("tabContainerTurma_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                                    dijit.byId("cadTurma").show();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "novaTurma");

                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                        montarGridPesquisaAluno(false, function () {
                                            abrirAlunoFKPesquisaTurma();
                                        });
                                    }
                                    else
                                        abrirAlunoFKPesquisaTurma();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesAluno");

                        dijit.byId("proAluno").on("Show", function (e) {
                            dijit.byId("gridPesquisaAluno").update();
                        });
                        //*** Cria os botões de persistência **\\
                        new Button({
                            label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconInsert",
                            onClick: function () { incluirItemHorarioTurma('gridHorarios', 'apresentadorMensagemTurma'); }
                        }, "incluirHorarioTurma");

                        new Button({
                            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                            onClick: function () { excluiItemHorarioTurma('gridHorarios', 'apresentadorMensagemTurma', VERIFTURMANORMAL, xhr); }
                        }, "excluirHorarioTurma");

                        // Botões da tela de dialogProgramacao
                        new Button({
                            label: "Incluir",
                            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                            onClick: function () {
                                incluirProgramacaoGrid();
                            }
                        }, "incluirProg");

                        new Button({
                            label: "Fechar",
                            iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                            onClick: function () {
                                dijit.byId("dialogProgramacao").hide();
                            }
                        }, "fecharProg");

                        new Button({
                            label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                alterarProgramacao(Memory);
                            }
                        }, "alterarProg");

                        new Button({
                            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                                if (tipoTurma != null && tipoTurma != TURMA_PPT) {
                                    apresentaMensagem('apresentadorMensagemTurma', '');
                                    keepValuesProgramacao(null, dijit.byId('gridProgramacao'), false);
                                } else {
                                    apresentaMensagem('apresentadorMensagemTurmaPPT', '');
                                    keepValuesProgramacao(null, dijit.byId('gridProgramacaoPPT'), false);
                                }
                            }
                        }, "cancelarProg");

                        //Lista de botões a serem diminuidos.
                        var buttonFkArray = ['proAlunoPPT', 'proCursoPPT'];
                        diminuirBotoes(buttonFkArray);
                        //link turma
                        var menu = new DropDownMenu({ id: "menuAcoesRelacionadas", style: "height: 25px" });

                        var acaoEditar = new MenuItem({
                            label: "Editar",
                            onClick: function () { eventoEditar(gridTurma.itensSelecionados, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton); }
                        });
                        menu.addChild(acaoEditar);

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () { eventoRemoverTurmas(gridTurma.itensSelecionados, xhr, ref); }
                        });
                        menu.addChild(acaoRemover);

                        menu.addChild(new MenuSeparator());


                        var acaoEncerrar = new MenuItem({
                            label: "Encerrar",
                            onClick: function () { eventoEncerrar(gridTurma.itensSelecionados, xhr, ready, Memory, FilteringSelect); }
                        });
                        menu.addChild(acaoEncerrar);
                        var acaoCancelarEncerrar = new MenuItem({
                            label: "Cancelar Encerramento",
                            onClick: function () { eventoCancelaEncerramento(gridTurma.itensSelecionados); }
                        });
                        menu.addChild(acaoCancelarEncerrar);
                        var acaoVirada = new MenuItem({
                            label: "Nova Turma",
                            onClick: function () {
                                eventoVirada(gridTurma.itensSelecionados, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton);
                            }
                        });
                        menu.addChild(acaoVirada);

                        menu.addChild(new MenuSeparator());

                        var acaoDiario = new MenuItem({
                            label: "Lançar Diário de Aula",
                            onClick: function () { incluirDiarioAula(xhr, ref, 'apresentadorMensagem', ready, Memory, FilteringSelect, on); }
                        });
                        menu.addChild(acaoDiario);

                        menu.addChild(new MenuSeparator());
                        var acaoProgramacao = new MenuItem({
                            label: "Gerar Modelo de Programação",
                            onClick: function () { gerarModeloProgramacaoAcaoRelacionada(xhr, ref, 'apresentadorMensagem'); }
                        });
                        menu.addChild(acaoProgramacao);

                        acaoProgramacao = new MenuItem({
                            label: "Programação Automática pelo Curso",
                            onClick: function () { incluirProgramacoesCurso(xhr, ref, 'apresentadorMensagem'); }
                        });
                        menu.addChild(acaoProgramacao);

                        acaoProgramacao = new MenuItem({
                            label: "Programação Automática pelo Modelo",
                            onClick: function () { incluirProgramacoesModelo(xhr, ref, 'apresentadorMensagem'); }
                        });
                        menu.addChild(acaoProgramacao);

                        acaoProgramacao = new MenuItem({
                            label: "Percentual de Faltas para Grupo Avançado",
                            onClick: function () { gerarRelatorioPercentualFaltasGrupoAvancado(xhr, ref, 'apresentadorMensagem', Memory, ObjectStore, dijit.byId("gridTurma")); }
                        });

                        menu.addChild(acaoProgramacao);

                        var button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadas",
                            dropDown: menu,
                            id: "acoesRelacionadas"
                        });
                        dojo.byId("linkAcoesTurma").appendChild(button.domNode);

                        menu = new DropDownMenu({ style: "height: 25px" });
                        acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'id', dijit.byId("gridHorarioProfessor")); }
                        });

                        menu.addChild(acaoRemover);

                        button = new DropDownButton({
                            label: "Ações Relacionadas",
                            name: "acoesRelacionadasHorProf",
                            dropDown: menu,
                            id: "acoesRelacionadasHorProf"
                        });
                        dojo.byId("linkAcoesHorarioProf").appendChild(button.domNode);

                        // Adiciona link de selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () { buscarTodosItens(gridTurma, 'todosItens', ['pesquisarTurma', 'relatorioTurma']); pesquisarTurma(false); }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridTurma', 'turmaSelecionada', 'cd_turma', 'selecionaTodos', ['pesquisarTurma', 'relatorioTurma'], 'todosItens'); }
                        });
                        menu.addChild(menuItensSelecionados);

                        var button = new DropDownButton({
                            label: "Todos Itens",
                            name: "todosItens",
                            dropDown: menu,
                            id: "todosItens"
                        });
                        dojo.byId("linkSelecionados").appendChild(button.domNode);
                        //fim
                        //link professor

                        //link Aluno
                        var buttons = new Button({
                            label: "Incluir",
                            name: "itens",
                            id: "itensAluno"
                        });
                        dijit.byId("idTurmaPPTPai").on("change", function (e) {
                            try {
                                if (this.checked) {
                                    if (hasValue(dijit.byId("pesCadRegime").value) && dijit.byId("pesCadRegime").value == PERSONALIZADO) {
                                        dojo.byId("tipoTurmaCad").value = TURMA_PPT;
                                        dojo.byId("trSalaHorario").style.display = "";
                                        mostrarMensagemDelecaoHorariosProgramacao();
                                        destroyCreateGridHorario();
                                        destroyCreateProgramacao();
                                        mudarLegendaPorTipoTurma(TURMA);
                                        dojo.byId("lblSalaPPTPai").style.display = "none";
                                        dojo.byId("tdSalaPPTPai").style.display = "none";
                                    }
                                    dijit.byId('tagAlunos').toggleable = false;
                                    dojo.byId("tagAlunos").style.display = "none";

                                    if (hasValue(dojo.byId("paiTabProgramacao")))
                                        dojo.style("paiTabProgramacao", "display", "none");
                                    if (hasValue(dojo.byId("paiTabAvaliacoes")))
                                        dojo.style("paiTabAvaliacoes", "display", "none");
                                    dijit.byId('tagAlunosPPT').toggleable = true;
                                    dojo.byId("tagAlunosPPT").style.display = "";
                                    dijit.byId("pesCadCurso").set("required", false);
                                    dijit.byId("pesCadCurso").set("disabled", true);
                                    document.getElementById("turmaAtiva").style.visibility = "";
                                    document.getElementById("labelAtiva").style.visibility = "";
                                    dojo.byId("trTurmaPPT").style.display = "none";
                                    if (dojo.byId('cd_turma').value <= 0) {
                                        dijit.byId("pesCadCurso")._onChangeActive = false;
                                        dijit.byId("pesCadCurso").reset();
                                        dijit.byId("pesCadCurso")._onChangeActive = true;
                                        //dijit.byId("pesCadProduto").reset();
                                        dijit.byId("pesCadProduto").set("disabled", false);
                                        //loadNewProdutos(xhr, ready, Memory, FilteringSelect);
                                    }
                                } else {
                                    if (hasValue(dijit.byId("pesCadRegime").value) && dijit.byId("pesCadRegime").value == PERSONALIZADO) {
                                        mostrarMensagemDelecaoHorariosProgramacao();
                                        destroyCreateGridHorario();
                                        destroyCreateProgramacao();
                                        dojo.byId("tipoTurmaCad").value = TURMA_PPT_FILHA;
                                        dojo.byId("trTurmaPPT").style.display = "";
                                        dojo.byId("trSalaHorario").style.display = "none";
                                        mudarLegendaPorTipoTurma(TURMA_PPT_FILHA);
                                        dojo.byId("lblSalaPPTPai").style.display = "";
                                        dojo.byId("tdSalaPPTPai").style.display = "";
                                    }
                                    dojo.byId("tagAlunos").style.display = "block";
                                    dijit.byId('tagAlunos').toggleable = true;
                                    document.getElementById("turmaAtiva").style.visibility = "hidden";
                                    document.getElementById("labelAtiva").style.visibility = "hidden";

                                    //if (hasValue(dojo.byId("paiTabAlunos")))
                                    //    dojo.style("paiTabAlunos", "display", "");
                                    if (hasValue(dojo.byId("paiTabProgramacao")))
                                        dojo.style("paiTabProgramacao", "display", "");
                                    if (hasValue(dojo.byId("paiTabAvaliacoes")))
                                        dojo.style("paiTabAvaliacoes", "display", "");
                                    dijit.byId('tagAlunosPPT').toggleable = false;
                                    dojo.byId("tagAlunosPPT").style.display = "none";
                                    if (dojo.byId('cd_turma').value <= 0) {
                                        dijit.byId("pesCadCurso").set("required", true);
                                        dijit.byId("pesCadCurso").set("disabled", true);
                                        dijit.byId("pesCadProduto").set("disabled", false);
                                        //dijit.byId("pesCadProduto").reset();
                                        if (hasValue(dijit.byId("pesCadProduto").value))
                                            dijit.byId("pesCadCurso").set("disabled", false);
                                        if (hasValue(dijit.byId("pesCadProduto").value)) {
                                            configuraCursoPorProdutoTurma(dijit.byId("pesCadProduto").value, xhr, ready, Memory, FilteringSelect);
                                        }
                                    }
                                }
                              
                                confLayoutBotInclAluno();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("pesCadRegime").on("change", function (e) {
                            try {
                                apresentaMensagem("apresentadorMensagemTurma", null);
                                var acao = NOVO;
                                var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
                                if (cd_turma > 0)
                                    acao = EDIT
                                if (hasValue(e) && e == PERSONALIZADO) {
                                    if (dijit.byId("idTurmaPPTPai").checked) {
                                        dojo.byId("tipoTurmaCad").value = TURMA_PPT;
                                        dijit.byId("pesProTurmaFK").set("disabled", true);
                                        dojo.byId("trTurmaPPT").style.display = "none";
                                        dojo.byId("trSalaHorario").style.display = "";
                                        mudarLegendaPorTipoTurma(TURMA);
                                        dojo.byId("lblSalaPPTPai").style.display = "none";
                                        dojo.byId("tdSalaPPTPai").style.display = "none";
                                    } else {
                                        dojo.byId("tipoTurmaCad").value = TURMA_PPT_FILHA;
                                        dijit.byId("pesProTurmaFK").set("disabled", true);
                                        dojo.byId("trTurmaPPT").style.display = "";
                                        mudarLegendaPorTipoTurma(TURMA_PPT_FILHA);
                                        mostrarMensagemDelecaoHorariosProgramacao();
                                        destroyCreateGridHorario();
                                        destroyCreateProgramacao();
                                        dojo.byId("trSalaHorario").style.display = "none";
                                        dojo.byId("lblSalaPPTPai").style.display = "";
                                        dojo.byId("tdSalaPPTPai").style.display = "";
                                    }
                                    if (acao == NOVO) {
                                        dijit.byId("idTurmaPPTPai").set("disabled", false);
                                        dijit.byId("pesProTurmaFK").set("disabled", false);
                                    }
                                } else {
                                    dojo.byId("trSalaHorario").style.display = "";
                                    mostrarMensagemDelecaoHorariosProgramacao();
                                    destroyCreateGridHorario();
                                    destroyCreateProgramacao();
                                    mudarLegendaPorTipoTurma(TURMA);
                                    dojo.byId("tipoTurmaCad").value = TURMA;
                                    dojo.byId("trTurmaPPT").style.display = "none";
                                    dijit.byId("idTurmaPPTPai").reset();
                                    dojo.byId("lblSalaPPTPai").style.display = "none";
                                    dojo.byId("tdSalaPPTPai").style.display = "none";
                                    dijit.byId("idTurmaPPTPai").set("disabled", true);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        var pesAluno = document.getElementById('pesAluno');
                        if (hasValue(pesAluno)) {
                            pesAluno.parentNode.style.minWidth = '18px';
                            pesAluno.parentNode.style.width = '18px';
                        }

                        loadDataTipoProg(Memory);
                        FindIsLoadComponetesPesquisaTurma(xhr, ready, Memory, FilteringSelect);
                        montarGridEComponetesTurmaPPT(ready, domConstruct, array, xhr, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect);
                        //Metodo temporario. Todo:
                        montarLegenda();
                        montarLegendaProgramacao();
                        montarLegendaSituacaoAlunoPPT();
                        montarLegendaSituacaoPaiAlunoPPTFilha();
                        montarLegendaSituacaoPaiAlunosPPT();


                        dijit.byId('cbSala').on("click", function () {
                            if(!hasValue(dijit.byId('cbSalaOnLine').value))
                                popularSalasPorHorarioTurma(xhr, ObjectStore, Memory, ref, Cache, JsonRest);
                        });
                        dijit.byId('cbSalaOnLine').on("click", function () {
                            popularSalasPorHorarioTurmaOnLine(xhr, ObjectStore, Memory, ref, Cache, JsonRest);
                        });
                        dijit.byId("cbSala").on("change", function (e) { 
                            if (!hasValue(dijit.byId('cbSalaOnLine').value))
                                buscaPopulaHorariosOcupadosSala(xhr, ObjectStore, Memory, ref, e, false);
                        });
                        dijit.byId("cbSalaOnLine").on("change", function (e) {
                            if (e == null || e == "")
                                dijit.byId("tabContainerTurma_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                            else
                                dijit.byId("tabContainerTurma_tablist_escolaContentPane").domNode.style.visibility = '';
                            buscaPopulaHorariosOcupadosSala(xhr, ObjectStore, Memory, ref, e, true);
                        });

                        var pesCadProduto = dijit.byId("pesCadProduto");

                        dijit.byId("pesCadProduto").on("change", function (e) {
                            try {
                                var gridTurma = dijit.byId('gridTurma');
                                //O produto não pode refazer as programações, pois o curso do produto ainda não foi selecionado, o usuário terá que selecioná-lo novamente de acordo com o novo produto.
                                if (!hasValue(gridTurma.itemSelecionado) || ((e != gridTurma.itemSelecionado.cd_produto && !hasValue(gridTurma.itemSelecionado.cd_produto_anterior))
                                        || (e != gridTurma.itemSelecionado.cd_produto_anterior && hasValue(gridTurma.itemSelecionado.cd_produto_anterior)))) {
                                    //    verificaProgramacaoTurma(pesCadProduto, xhr, 'cd_produto', TIPO_DIJIT, function () { atualizaCursoParaNovoProduto(e, xhr, ready, Memory, FilteringSelect); });
                                    if (hasValue(gridTurma.itemSelecionado))
                                        gridTurma.itemSelecionado.cd_produto_anterior = e;
                                    atualizaCursoParaNovoProduto(e, xhr, ready, Memory, FilteringSelect);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        var pesCadCurso = dijit.byId("pesCadCurso");
                        pesCadCurso.on("change", function (e) {
                            try {
                                var gridTurma = dijit.byId('gridTurma');
                                //TODO: verificar o problema do dojo de dar tabulação e acionar o onchange:
                                if ((!hasValue(gridTurma.itemSelecionado) && hasValue(dijit.byId('pesCadCurso').dta_inicio_aula_anterior) && dijit.byId('pesCadCurso').dta_inicio_aula_anterior != e)
                                        || (hasValue(gridTurma.itemSelecionado) && ((e != gridTurma.itemSelecionado.cd_curso && !hasValue(gridTurma.itemSelecionado.cd_curso_anterior))
                                            || (e != gridTurma.itemSelecionado.cd_curso_anterior && hasValue(gridTurma.itemSelecionado.cd_curso_anterior)))))
                                    verificaProgramacaoTurma(pesCadCurso, xhr, 'cd_curso', TIPO_DIJIT, function executaRetorno() { confLayoutBotInclAluno(); });

                                confLayoutBotInclAluno();

                                if (hasValue(gridTurma.itemSelecionado))
                                    gridTurma.itemSelecionado.dta_inicio_aula_anterior = e;
                                else
                                    dijit.byId('pesCadCurso').dta_inicio_aula_anterior = e;
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        var pesCadDuracao = dijit.byId("pesCadDuracao");
                        pesCadDuracao.on("change", function (e) {
                            try {
                                var gridTurma = dijit.byId('gridTurma');
                                //TODO: verificar o problema do dojo de dar tabulação e acionar o onchange:
                                if ((!hasValue(gridTurma.itemSelecionado) && hasValue(dijit.byId('pesCadDuracao').dta_inicio_aula_anterior) && dijit.byId('pesCadDuracao').dta_inicio_aula_anterior != e)
                                        || (hasValue(gridTurma.itemSelecionado) && ((e != gridTurma.itemSelecionado.cd_duracao && !hasValue(gridTurma.itemSelecionado.cd_duracao_anterior))
                                            || (e != gridTurma.itemSelecionado.cd_duracao_anterior && hasValue(gridTurma.itemSelecionado.cd_duracao_anterior)))))
                                    verificaProgramacaoTurma(pesCadDuracao, xhr, 'cd_duracao', TIPO_DIJIT, null);
                                if (hasValue(gridTurma.itemSelecionado))
                                    gridTurma.itemSelecionado.dta_inicio_aula_anterior = e;
                                else
                                    dijit.byId('pesCadDuracao').dta_inicio_aula_anterior = e;
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        dijit.byId("dtIniAula").on("change", function (e) {
                            try {
                                var gridTurma = dijit.byId('gridTurma');
                                var cd_turma_ppt = 0;
                                //TODO: verificar o problema do dojo de dar tabulação e acionar o onchange:
                                if (e != null)
                                    e = dojo.date.locale.format(e, { datePattern: "dd/MM/yyyy", selector: "date" });
                                if ((!hasValue(gridTurma.itemSelecionado) && hasValue(dijit.byId('dtIniAula').dta_inicio_aula_anterior) && dijit.byId('dtIniAula').dta_inicio_aula_anterior != e)
                                            || (hasValue(gridTurma.itemSelecionado) && ((e != gridTurma.itemSelecionado.dta_inicio_aula && !hasValue(gridTurma.itemSelecionado.dta_inicio_aula_anterior))
                                                || (e != gridTurma.itemSelecionado.dta_inicio_aula_anterior && hasValue(gridTurma.itemSelecionado.dta_inicio_aula_anterior)))))
                                    verificaProgramacaoTurma(dojo.byId("dtIniAula"), xhr, 'dta_inicio_aula', TIPO_DOJO, null);
                                if (hasValue(gridTurma.itemSelecionado)) {
                                    gridTurma.itemSelecionado.dta_inicio_aula_anterior = e;
                                    cd_turma_ppt = gridTurma.itemSelecionado.cd_turma_ppt;
                                }
                                else {
                                    dijit.byId('dtIniAula').dta_inicio_aula_anterior = e;
                                    cd_turma_ppt = 0;
                                }
                                if (!hasValue(dijit.byId('pesCadRegime').value) || cd_turma_ppt  == 0) {
                                    dijit.byId("cbSala")._onChangeActive = false;
                                    dijit.byId("cbSala").reset();
                                    dijit.byId("cbSala")._onChangeActive = true;
                                    dijit.byId("cbSala").set('disabled', true);
                                    dijit.byId("cbSalaOnLine")._onChangeActive = false;
                                    dijit.byId("cbSalaOnLine").reset();
                                    dijit.byId("cbSalaOnLine")._onChangeActive = true;
                                }
                                dijit.byId("cbSalaOnLine").set('disabled', true);
                                if (e != null) {
                                    dijit.byId("cbSala").set('disabled', false);
                                    dijit.byId("cbSalaOnLine").set('disabled', false);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        //#region Onchange para PPT

                        dijit.byId("dtIniAulaPPT").on("change", function (e) {
                            try {
                                var gridTurma = dijit.byId('gridTurma');
                                //TODO: verificar o problema de dar tabulação e acionar o onchange:
                                if (e != null)
                                    e = dojo.date.locale.format(e, { datePattern: "dd/MM/yyyy", selector: "date" });
                                if ((!hasValue(gridTurma.itemSelecionado) && hasValue(dijit.byId('dtIniAulaPPT').dta_inicio_aula_anterior) && dijit.byId('dtIniAulaPPT').dta_inicio_aula_anterior != e)
                                        || (hasValue(gridTurma.itemSelecionado) && ((e != gridTurma.itemSelecionado.dta_inicio_aula && !hasValue(gridTurma.itemSelecionado.dta_inicio_aula_anterior))
                                            || (e != gridTurma.itemSelecionado.dta_inicio_aula_anterior && hasValue(gridTurma.itemSelecionado.dta_inicio_aula_anterior)))))
                                    verificaProgramacaoTurma(dojo.byId("dtIniAulaPPT"), xhr, 'dta_inicio_aula', TIPO_DOJO, null);
                                if (hasValue(gridTurma.itemSelecionado))
                                    gridTurma.itemSelecionado.dta_inicio_aula_anterior = e;
                                else
                                    dijit.byId('dtIniAulaPPT').dta_inicio_aula_anterior = e;
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });


                        var pesCadDuracaoPPT = dijit.byId("pesCadDuracaoPPT");
                        pesCadDuracaoPPT.on("change", function (e) {
                            try {
                                var gridTurma = dijit.byId('gridTurma');
                                //TODO: verificar o problema de dar tabulação e acionar o onchange:
                                if ((!hasValue(gridTurma.itemSelecionado) && hasValue(dijit.byId('pesCadDuracaoPPT').dta_inicio_aula_anterior) && dijit.byId('pesCadDuracaoPPT').dta_inicio_aula_anterior != e)
                                        || (hasValue(gridTurma.itemSelecionado) && ((e != gridTurma.itemSelecionado.cd_duracao && !hasValue(gridTurma.itemSelecionado.cd_duracao_anterior))
                                            || (e != gridTurma.itemSelecionado.cd_duracao_anterior && hasValue(gridTurma.itemSelecionado.cd_duracao_anterior)))))
                                    verificaProgramacaoTurma(pesCadDuracaoPPT, xhr, 'cd_duracao', TIPO_DIJIT, null);
                                if (hasValue(gridTurma.itemSelecionado))
                                    gridTurma.itemSelecionado.dta_inicio_aula_anterior = e;
                                else
                                    dijit.byId('pesCadDuracaoPPT').dta_inicio_aula_anterior = e;
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });


                        dijit.byId("idTurmaAtiva").on("click", function (e) {
                            try {
                                if (this.checked)
                                    dijit.byId("btnNovoAlunoPPT").set("disabled", false);
                                else
                                    dijit.byId("btnNovoAlunoPPT").set("disabled", true);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        //#endregion

                        new Button({ label: "Anterior", iconClass: 'dijitEditorIcon dijitEditorIconRedo', disabled: true, onClick: function () { } }, "anterior");
                        new Button({
                            label: "Posterior", iconClass: 'dijitEditorIcon dijitEditorIconUndo', onClick: function ()
                            {
                                proximoWizard(xhr, ready, Memory, FilteringSelect);
                            }
                        }, "proximo");
                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                dijit.byId("dialogEncerramento").hide();
                            }
                        }, "fecharEnc");
                        new Button({ label: "Anterior", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () { wizardAnterior(); } }, "anteriorConf");
                        new Button({
                            label: "Confirmar", iconClass: 'dijitEditorIcon dijitEditorIconSelect', onClick: function () {
                                confirmar(xhr, ObjectStore, Memory, ref, Cache, JsonRest, Button, dijit.byId("gridTurma").itensSelecionados, FilteringSelect, ready);
                            }
                        }, "proximoConf");
                        new Button({
                            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                dijit.byId("dialogEncerramento2").hide();
                            }
                        }, "fecharConf");
                        // colocar aqui se é rematricula e seguir
                            new Button({
                                label: "Nova Turma", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                    // se é rematricula e seguir
                                    if (document.getElementById("ckViradaTurma").checked == false) {
                                        try {
                                            var eTurmaNova = false;
                                            var cdTurma = dijit.byId("no_turma_encerramento").store.data;
                                            if (cdTurma.length > 1) {
                                                caixaDialogo(DIALOGO_ERRO, msgErroNovaTurmaEnc, null);
                                            } else {
                                                var cdTurma = cdTurma[0].id;
                                                criaDadosNovaTurma(cdTurma, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova);
                                            }
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                                    if (document.getElementById("ckViradaTurma").checked == true) {
                                        try {
                                            var eTurmaNova = true;
                                            var cdTurma = null;
                                            criaDadosNovaTurma(cdTurma, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova);
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                                }
                            }, "novaTurmaEnc");
                            new Button({
                                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                    dijit.byId("dialogEncerramento3").hide();
                                    // botão fechar
                                    if (document.getElementById("ckViradaTurma").checked == true) {
                                        try {
                                            var eTurmaNova = false;
                                            var cdTurma = null;
                                            criaDadosNovaTurma(cdTurma, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova);
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                                }
                            }, "fecharEncNova");

                        // teste de botão
                            new Button({
                                label: "Novas Turmas", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                    // se é rematricula e seguir
                                    if (document.getElementById("ckViradaTurma").checked == false) {
                                        try {
                                            var eTurmaNova = false;
                                            var cdTurma = dijit.byId("no_turma_encerramento").store.data;
                                            if (cdTurma.length > 1) {
                                                caixaDialogo(DIALOGO_ERRO, msgErroNovaTurmaEnc, null);
                                            } else {
                                                var cdTurma = cdTurma[0].id;
                                                criaDadosNovaTurma(cdTurma, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova);
                                            }
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                                    if (document.getElementById("ckViradaTurma").checked == true) {
                                        try {
                                            var eTurmaNova = true;
                                            var cdTurma = null;
                                            criaDadosNovaTurma(cdTurma, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova);
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                                }
                            }, "novaTurmaEncCk");
                            new Button({
                                label: "Encerrar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                    dijit.byId("dialogEncerramento3").hide();
                                    // botão fechar
                                    if (document.getElementById("ckViradaTurma").checked == true) {
                                        try {
                                            var eTurmaNova = false;
                                            var cdTurma = null;
                                            criaDadosNovaTurma(cdTurma, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova);
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    }
                                }
                            }, "fecharEncNovaCk");

                            dijit.byId("dt_fim_enc").on("change", function (e) {
                            localStorage.setItem("data_termino", e);
                            apresentaMensagem('apresentadorMensagemEncerramento', null);
                            apresentaMensagem('apresentadorMensagemEncConf', null);
                        });

                        dijit.byId("proTurmaFK").on("show", function (e) {
                            var gridPesquisa = dijit.byId("gridPesquisaTurmaFK");
                            if (hasValue(gridPesquisa))
                                gridPesquisa.update();
                        });

                        dijit.byId("tagProgramaoesTurmaPPT").on("show", function (e) {
                            var gridProgFilhaPPT = dijit.byId("gridProgramacaoPPT");
                            if (hasValue(gridProgFilhaPPT))
                                dijit.byId("gridProgramacaoPPT").update();
                        });
                        dijit.byId("tagDesconsiderarFeriadosPPT").on("show", function (e) {
                            var gridDesconsideraFeriadosPPT = dijit.byId("gridDesconsideraFeriadosPPT");
                            if (hasValue(gridDesconsideraFeriadosPPT))
                                gridDesconsideraFeriadosPPT.update();
                        });

                        dijit.byId("tipoTurma").on("change", function (e) {
                            try {
                                if (isNaN(eval(e))) {
                                    dijit.byId("tipoTurma").set('value', 0);
                                } else {
                                    var acoesTurmasFilhas = dijit.byId("acoesTurmasFilhas");
                                    var menu = dijit.byId('menuAcoesRelacionadas');
                                    var gridTurma = dijit.byId("gridTurma");
                                    var pesSituacao = dijit.byId("pesSituacao");
                                    if (this.displayedValue == "Personalizada") {
                                        dojo.byId("lblTurmaFilhas").style.display = "";
                                        dojo.byId("divPesTurmasFilhas").style.display = "";
                                        loadSituacaoTurmaPPT(Memory);
                                        pesSituacao.set("value", 1);
                                        pesSituacao.set("disabled", false);
                                        if (!hasValue(acoesTurmasFilhas)) {
                                            var acaoMostrarTurmasFilhas = new MenuItem({
                                                label: "Mostrar Turmas Filhas",
                                                id: "acoesTurmasFilhas",
                                                onClick: function () {
                                                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                                    if (!hasValue(gridTurma.itensSelecionados) || gridTurma.itensSelecionados.length <= 0)
                                                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                                                    else if (gridTurma.itensSelecionados.length > 1)
                                                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                                                    else {
                                                        var turma = gridTurma.itensSelecionados[0];
                                                        if ((!hasValue(turma.cd_turma_ppt) && turma.id_turma_ppt == true) || (turma.cd_turma_ppt == 0 && turma.id_turma_ppt == true)) {
                                                            pesquisarTurma(false, turma.cd_turma, true);
                                                            var acoesTurmasFilhas = dijit.byId("acoesTurmasFilhas");
                                                            var menu = dijit.byId('menuAcoesRelacionadas');
                                                            if (hasValue(acoesTurmasFilhas))
                                                                menu.removeChild(acoesTurmasFilhas);
                                                            dijit.byId("pesTurmasFilhas").set("checked", true);
                                                            eventoClickPesquisarTurmasFilhas(dijit.byId("pesTurmasFilhas"));
                                                        }
                                                        else {
                                                            var mensagensWeb = new Array();
                                                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNecessarioTurmaPPT);
                                                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                                        }
                                                    }
                                                }
                                            });
                                            menu.addChild(acaoMostrarTurmasFilhas, 7);
                                        } else
                                            menu.addChild(acoesTurmasFilhas, 7);
                                        //dojo.style("acoesTurmasFilhas", "display", "");
                                        //acoesTurmasFilhas.set("display","");
                                    } else {
                                        dojo.byId("lblTurmaFilhas").style.display = "none";
                                        dojo.byId("divPesTurmasFilhas").style.display = "none";
                                        dijit.byId("pesTurmasFilhas").set("checked", false);
                                        eventoClickPesquisarTurmasFilhas(dijit.byId("pesTurmasFilhas"));
                                        loadSituacaoTurma(Memory);
                                        if (hasValue(acoesTurmasFilhas))
                                            menu.removeChild(acoesTurmasFilhas);
                                        pesSituacao.set("value", 1);
                                    }
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        dijit.byId("pesTurmasFilhas").on("click", function (e) {
                            try {
                                eventoClickPesquisarTurmasFilhas(this);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        dijit.byId("pesProfessor").on("change", function (cd_prof) {
                            try {
                                if (!hasValue(cd_prof) || cd_prof < 0) {
                                    dijit.byId("pesProfessor").set("value", TODOS);
                                    dijit.byId("ckIdProfTurmasAtuais").set("checked", false);
                                } else
                                    dijit.byId("ckIdProfTurmasAtuais").set("checked", true);
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        if (hasValue(dijit.byId("menuManual"))) {
                            dijit.byId("menuManual").on("click",
                                function(e) {
                                    abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323054',
                                        '765px',
                                        '771px');
                                });
                        }
                        inicial();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem("apresentadorMensagem", error);
                });

                adicionarAtalhoPesquisa(['_nomTurma', '_apelidoT', 'tipoTurma', 'pesCurso', 'pesDuracao', 'pesProduto', 'pesSituacao', 'pesProfessor', 'sProgramacao', 'dtInicial', 'dtFinal'], 'pesquisarTurma', ready);

            }
            catch (e) {
                postGerarLog(e);
            }
        });

    });
}

function getEmpresasGridEscolasTurma() {
    var listaEmpresasGrid = "";

    if (hasValue(dijit.byId("gridEscolas").store.objectStore.data))
        //for (var i = 0; dijit.byId("gridEscolas").store.objectStore.data.length - 1;  i++)
        $.each(dijit.byId("gridEscolas").store.objectStore.data, function (index, value) {
            listaEmpresasGrid += value.cd_escola + ",";
        });
    return listaEmpresasGrid;
}


//#region  eventos para a aba de escola
function pesquisarEscolasFK() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
        function (JsonRest, ObjectStore, Cache, Memory, ready) {
            ready(function() {
                try {
                    var listaEmpresasGrid = getEmpresasGridEscolasTurma();
                    var myStore = Cache(
                        JsonRest({
                            target: Endereco() + "/api/empresa/findAllEmpresasByUsuarioPag?cdEmpresas=" + listaEmpresasGrid +
                                "&nome=" + dojo.byId("_nomePessoaTurmaFK").value +
                                "&fantasia=" + dojo.byId("_apelidoTurma").value +
                                "&cnpj=" + dojo.byId("CnpjCpfTurma").value +
                                "&inicio=" + document.getElementById("inicioPessoaTurmaFK").checked,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }),
                        Memory({}));

                    dataStore = new ObjectStore({ objectStore: myStore });
                    var grid = dijit.byId("gridPesquisaPessoaTurma");
                    grid.setStore(dataStore);
                    grid.layout.setColumnVisibility(5, false);
                } catch (e) {
                    postGerarLog(e);
                }
            });
        });
}

function pesquisarEscolasVinculadasUsuario() {
    
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/empresa/findQuantidadeEmpresasVinculadasUsuario",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                if (data != null && data != undefined && data >= 0) {
                    dojo.byId("qtdEscolasVinculadasUsuario").value = data;
                } else {
                    dojo.byId("qtdEscolasVinculadasUsuario").value = 0;
                }

                if ( dojo.byId("qtdEscolasVinculadasUsuario").value == 0) {
                    dijit.byId("tabContainerTurma_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                }
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemMat', error);
        });
    
}

function pesquisarSalas() {

	dojo.xhr.get({
		preventCache: true,
        url: Endereco() + "/api/turma/findSalas",
		handleAs: "json",
		headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
	}).then(function (data) {
			try {
				data = jQuery.parseJSON(data).retorno;
				if (data != null) {
                    criarOuCarregarCompFiltering("cbSearchSala", data.Salas, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_sala', 'no_sala', MASCULINO);
                    criarOuCarregarCompFiltering("cbSearchSalaOnLine", data.SalasOnline, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_sala', 'no_sala', MASCULINO);
				} 

				
			} catch (e) {
				postGerarLog(e);
			}
		},
		function (error) {
			apresentaMensagem('apresentadorMensagemMat', error);
		});

}

function abrirPessoaEscolaFK(isPesquisa) {
    dojo.ready(function () {
        try {
            dijit.byId("fkPessoaTurmaPesq").set("title", "Pesquisar Escolas");
            dijit.byId('tipoPessoaTurmaFK').set('value', 2);
            dojo.byId('lblNomRezudioPessoaTurmaFK').innerHTML = "Fantasia";
            dijit.byId('tipoPessoaTurmaFK').set('disabled', true);
            dijit.byId("gridPesquisaPessoaTurma").getCell(3).name = "Fantasia";
            dijit.byId("gridPesquisaPessoaTurma").getCell(2).width = "15%";
            dijit.byId("gridPesquisaPessoaTurma").getCell(2).unitWidth = "15%";
            dijit.byId("gridPesquisaPessoaTurma").getCell(3).width = "20%";
            dijit.byId("gridPesquisaPessoaTurma").getCell(3).unitWidth = "20%";
            dijit.byId("gridPesquisaPessoaTurma").getCell(1).width = "25%";
            dijit.byId("gridPesquisaPessoaTurma").getCell(1).unitWidth = "25%";
            limparPesquisaEscolaFK();
            pesquisarEscolasFK();
            dijit.byId("fkPessoaTurmaPesq").show();
            apresentaMensagem('apresentadorMensagemProPessoaTurma', null);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function limparPesquisaEscolaFK() {
    try {
        dojo.byId("_nomePessoaTurmaFK").value = "";
        dojo.byId("_apelidoTurma").value = "";
        dojo.byId("CnpjCpfTurma").value = "";
        if (hasValue(dijit.byId("gridPesquisaPessoaTurma"))) {
            dijit.byId("gridPesquisaPessoaTurma").currentPage(1);
            if (hasValue(dijit.byId("gridPesquisaPessoaTurma").itensSelecionados))
                dijit.byId("gridPesquisaPessoaTurma").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGrid() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        if (hasValue(gridEscolas)) {
            gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridEscolas.update();
        }
        
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarPessoaTurma() {
    try {
        var valido = true;
        var gridPessoaSelec = dijit.byId("gridPesquisaPessoaTurma");
        var gridEscolas = dijit.byId("gridEscolas");
        if (!hasValue(gridPessoaSelec.itensSelecionados))
            gridPessoaSelec.itensSelecionados = [];
        if (!hasValue(gridPessoaSelec.itensSelecionados) || gridPessoaSelec.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            
                var storeGridEscolas = (hasValue(gridEscolas) && hasValue(gridEscolas.store.objectStore.data)) ? gridEscolas.store.objectStore.data : [];
                quickSortObj(gridEscolas.store.objectStore.data, 'cd_escola');
                $.each(gridPessoaSelec.itensSelecionados, function (idx, value) {
                    insertObjSort(gridEscolas.store.objectStore.data, "cd_escola", {
                        cd_turma_escola: 0,
                        cd_turma: dojo.byId("cd_turma").value,
                        cd_escola: value.cd_pessoa,
                        dc_reduzido_pessoa: value.dc_reduzido_pessoa
                    });
                });
                gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridEscolas.store.objectStore.data }) }));
          

        if (!valido)
            return false;
        dijit.byId("fkPessoaTurmaPesq").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}


// lista os contratos na rematricula
function componentesPesquisaMatricula() {
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/componentesPesquisaMatricula",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (hasValue(data.retorno)) {
                if (hasValue(data.retorno.nomesContrato)) {
                    data.retorno.nomesContrato.push({ cd_nome_contrato: 1000000, no_contrato: "Nenhum" });
                    criarOuCarregarCompFiltering("cd_nome_contrato_pesq", data.retorno.nomesContrato, "", 0, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_nome_contrato', 'no_contrato', MASCULINO);
                }
                if (hasValue(data.retorno.anosEscolares))
                    criarOuCarregarCompFiltering("pesqAnoEscolar", data.retorno.anosEscolares, "", 0, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'cd_ano_escolar', 'dc_ano_escolar', MASCULINO);
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemEncerramento', error);
    });
}

function formartExistAluno(value, rowIndex, obj) {
    try {
        var gridName = 'gridTurma';
        var grid = dijit.byId(gridName);
        retorno = grid._by_idx[rowIndex].item.no_aluno;

        if (document.getElementById("pesTurmasFilhas").checked)
            dojo.byId(this.id).style.display = '';
        else
            dojo.byId(this.id).style.display = 'none';

        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formartExistCurso(value, rowIndex, obj) {
    try {
        var gridName = 'gridTurma';
        var grid = dijit.byId(gridName);
        retorno = grid._by_idx[rowIndex].item.no_curso;

        if (document.getElementById("pesTurmasFilhas").checked)
            dojo.byId(this.id).style.display = 'none';
        else
            dojo.byId(this.id).style.display = '';

        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizaCursoParaNovoProduto(e, xhr, ready, Memory, FilteringSelect) {
    confLayoutBotInclAluno();
    if (!dijit.byId("idTurmaPPTPai").checked)
        configuraCursoPorProdutoTurma(e, xhr, ready, Memory, FilteringSelect);
}

function configuraCursoPorProdutoTurma(evt, xhr, ready, Memory, FilteringSelect) {
    try {
        var pesCadCurso = dijit.byId("pesCadCurso");
        pesCadCurso.set("disabled", false);
        var statusStore = new Memory({
            data: null
        });
        dijit.byId("pesCadCurso")._onChangeActive = false;
        pesCadCurso.reset();
        pesCadCurso.store = statusStore;
        dijit.byId("pesCadCurso")._onChangeActive = true;
        if (hasValue(evt)) {
            carregarCursoPorProdutoTurma(evt, xhr, ready, Memory, FilteringSelect);
        } else
            pesCadCurso.set("disabled", true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarElementosTagAlunos(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest, Pagination,
                                 FilteringSelect, registry, Array, ready) {
    try {
        new Button({
            label: "Incluir",
            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
            onClick: function () {
                try {
                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                        montarGridPesquisaAluno(false, function () {
                            abrirAlunoFKPesquisaCadTurmaNornal();
                        });
                    }
                    else
                        abrirAlunoFKPesquisaCadTurmaNornal();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "btnNovoAluno");

        var menuAluno = new DropDownMenu({ style: "height: 25px" });
        var acaoExcluirAluno = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try {
                    gridAlunos = dijit.byId("gridAlunos");
                    var possuiAlunoOutraEscola = gridAlunos.itensSelecionados.some(function (item) {
                        return (hasValue(item.cd_pessoa_escola_aluno) && (item.cd_pessoa_escola_aluno + "") !== dojo.byId("_ES0").value);
                    });
                    if (possuiAlunoOutraEscola) {
                        caixaDialogo(DIALOGO_AVISO, msgErroMatriculaAlunoOutroaEscola, null);
                        return false;
                    } else {
                        deletarItemSelecionadoAlunoGrid(Memory, ObjectStore, 'cd_aluno', "gridAlunos");
                        var tipoTurma = dojo.byId("tipoTurmaCad").value;
                        var totalAlunos = hasValue(dijit.byId("gridAlunos")) ? dijit.byId("gridAlunos").listaCompletaAlunos.length : 0;
                        habilitarCamposTurma(tipoTurma, totalAlunos);
                    }
                     
                    
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menuAluno.addChild(acaoExcluirAluno);
        var acaoMatricularAluno = new MenuItem({
            label: "Matrícula",
            onClick: function () {
                populaContrato(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, dijit.byId("gridAlunos"), false);
            }
        });
        menuAluno.addChild(acaoMatricularAluno);

        var acaoDesistenciaAluno = new MenuItem({
            label: "Desistência",
            onClick: function () {
                populaDesistencia(dijit.byId("gridAlunos"), false);
            }
        });
        menuAluno.addChild(acaoDesistenciaAluno);

        var acaoHistorico = new MenuItem({
            label: "Histórico do Aluno",
            onClick: function () {
                abrirHistoricoAluno(dijit.byId('gridAlunos'));
            }
        });
        menuAluno.addChild(acaoHistorico);

        var buttonAluno = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasAluno",
            dropDown: menuAluno,
            id: "acoesRelacionadasAluno"
        });
        dojo.byId("linkAcoesAluno").appendChild(buttonAluno.domNode);
        //Filtro grade alunos
        var menuFiltroAluno = new DropDownMenu({ style: "height: 25px", id: "menuFiltroAluno" });
        menuFiltroAluno.addChild(new MenuItem({
            id: "100",
            name: "todasFiltroAluno",
            label: "Todas",
            onClick: function (e) { montarFiltroSituacaoALunoTurma(e); }
        }));
        var buttonFiltroAluno = new DropDownButton({
            label: "Situações",
            name: "acoesRelacionadasFiltroAluno",
            dropDown: menuFiltroAluno,
            id: "acoesRelacionadasFiltroAluno"
        });
        dojo.byId("linkFiltroAcoesAluno").appendChild(buttonFiltroAluno.domNode);

        var menuAlunoPPTFilha = new DropDownMenu({ style: "height: 25px" });
        var acaoMatricularAluno = new MenuItem({
            label: "Matrícula",
            onClick: function () {
                populaContrato(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, dijit.byId("gridAlunos"), false);
            }
        });
        menuAlunoPPTFilha.addChild(acaoMatricularAluno);


        var acaoDesistenciaAlunoPPT = new MenuItem({
            label: "Desistência",
            onClick: function () {
                populaDesistencia(dijit.byId("gridAlunos"), false);
            }
        });
        menuAlunoPPTFilha.addChild(acaoDesistenciaAlunoPPT);

        var buttonAlunoPPTFilha = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasAlunoFilha",
            dropDown: menuAlunoPPTFilha,
            id: "acoesRelacionadasAlunoFilha"
        });

        dojo.byId("linkAcoesFilha").appendChild(buttonAlunoPPTFilha.domNode);



        var gridAlunos = new EnhancedGrid({
            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
              [
                { name: "<input id='selecionaTodosAluno' style='display:none'/>", field: "alunoSelecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAlunos },
                { name: "Nome", field: "no_aluno", width: "70%" },
                //{ name: "Escola", field: "dc_reduzido_pessoa_escola", width: "35%" },
                { name: "Situação", field: "situacaoAlunoTurma", width: "25%" }
              ],
            canSort: true,
            noDataMessage: msgNotRegEnc,
            selectionMode: "single",
            plugins: {
                pagination: {
                    pageSizes: ["10", "30", "60", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "10",
                    gotoButton: true,
                    /*page step to be displayed*/
                    maxPageStep: 5,
                    /*position of the pagination bar*/
                    position: "button",
                    plugins: { nestedSorting: false }
                }
            }
        }, "gridAlunos");
        gridAlunos.on("StyleRow", function (row) {
            try {
                var item = gridAlunos.getItem(row.index);
                if (hasValue(item)) {
                    if (item.cd_situacao_aluno_turma == DESISTENTE)
                        row += " RedRow";
                    if (item.cd_situacao_aluno_turma == MOVIDO)
                        row.customClasses += " BlueRow";
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        gridAlunos.canSort = function (col) { return Math.abs(col) != 1; };
        gridAlunos.startup();
        gridAlunos.listaCompletaAlunos = [];
        dijit.byId("tagAlunos").on("show", function (e) {
            dijit.byId("paiAluno").resize();
            dijit.byId("gridAlunos").update();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirHistoricoAluno(gridAluno) {
    try {
        if (!hasValue(gridAluno.itensSelecionados) || gridAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else if (gridAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
            return false;
        }
        else
            window.location = Endereco() + '/Secretaria/HistoricoAluno?cd_aluno=' + gridAluno.itensSelecionados[0].cd_aluno + '&cd_pessoa=' + gridAluno.itensSelecionados[0].cd_pessoa_aluno
                                     + '&no_pessoa=' + gridAluno.itensSelecionados[0].no_aluno + '&dta_cadastro=' + gridAluno.itensSelecionados[0].dta_cadastro
                                     + '&cd_situacao_aluno=' + gridAluno.itensSelecionados[0].cd_situacao_aluno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarItemSelecionadoAlunoGrid(Memory, ObjectStore, nomeId, id) {
    try {
        var grid = dijit.byId(id);
        grid.store.save();
        var dados = grid.listaCompletaAlunos;
        var dadosView = grid.store.objectStore.data;
        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
            //Percorre a lista da grade para deleção (O(n)):
            for (var i = dados.length - 1; i >= 0; i--)
                // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null)
                    dados.splice(i, 1); // Remove o item do array
            if (dadosView.length > 0)
                for (var i = dadosView.length - 1; i >= 0; i--)
                    // Verifica se os itens selecionados estão na lista e remove com busca binária (O(log n)):
                    if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dadosView[i].' + nomeId)) != null)
                        dadosView.splice(i, 1); // Remove o item do array

            grid.itensSelecionados = new Array();
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dadosView, idProperty: "id" }) });
            grid.setStore(dataStore);
            grid.update();
            if (dojo.byId("tipoTurmaCad").value == TURMA_PPT_FILHA)
                dijit.byId("btnNovoAluno").set("disabled", false);
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function confLayoutBotInclAluno() {
    try {
        var gridHorarios = dijit.byId("gridHorarios");
        // dijit.byId("btnNovoAlunoPPT").set("disabled", false);
        var horarios = [];
        var cdTurma = dojo.byId("cd_turma").value;
        var cdTurmaPPT = dojo.byId("cd_turma_ppt").value;
        if (hasValue(gridHorarios) && gridHorarios.params.store.data.length >= 0) {
            horarios = mountHorariosOcupadosTurma(gridHorarios.params.store.data);
        } else if ((hasValue(cdTurmaPPT) && eval(cdTurmaPPT) > 0 &&
                   hasValue(dijit.byId("gridTurma")) &&
                   hasValue(dijit.byId("gridTurma").TurmaEdit) &&
                   hasValue(dijit.byId("gridTurma").TurmaEdit.horariosTurma)) ||
            (hasValue(cdTurma) && eval(cdTurma) > 0 &&
                   hasValue(dijit.byId("gridTurma")) &&
                   hasValue(dijit.byId("gridTurma").TurmaEdit) &&
                   hasValue(dijit.byId("gridTurma").TurmaEdit.horariosTurma)))
            horarios = dijit.byId("gridTurma").TurmaEdit.horariosTurma;
        if (horarios != null && horarios.length > 0) {
            habilitarBotaoAluno();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function habilitarBotaoAluno() {
    try {
        var turmAtiva = dijit.byId("idTurmaAtiva").checked;
        var gridAlunos = dijit.byId('gridAlunos');
        var tipoTurma = dojo.byId("tipoTurmaCad").value;

        switch (parseInt(tipoTurma)) {
            case TURMA_PPT: {
                if (turmAtiva)
                    dijit.byId("btnNovoAlunoPPT").set("disabled", false);
                else
                    dijit.byId("btnNovoAlunoPPT").set("disabled", true);
                break;
            }
            case TURMA_PPT_FILHA: {
                if ((gridAlunos != null) && (gridAlunos.listaCompletaAlunos != null) && (gridAlunos.listaCompletaAlunos.length == 0))
                    dijit.byId("btnNovoAluno").set("disabled", false);
                else {
                    dijit.byId("btnNovoAluno").set("disabled", true);
                }
                break;
            }
            default: {
                dijit.byId("btnNovoAluno").set("disabled", false);
                break;
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxFeriadoDesconsiderado(value, rowIndex, obj) {
    try {
        var gridName = 'gridDesconsideraFeriados';
        var tipoTurma = dojo.byId("tipoTurmaCad").value;

        if (tipoTurma == TURMA_PPT)
            gridName = 'gridDesconsideraFeriadosPPT';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFeriadoDesconsiderado');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_feriado_desconsiderado", grid._by_idx[rowIndex].item.nm_feriado_desconsiderado);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_feriado_desconsiderado', 'selecionadoFeriadoDesconsiderado', -1, 'selecionaTodosFeriadoDesconsiderado', 'selecionaTodosFeriadoDesconsiderado', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'nm_feriado_desconsiderado', 'selecionadoFeriadoDesconsiderado', " + rowIndex + ", '" + id + "', 'selecionaTodosFeriadoDesconsiderado', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxProgramacao(value, rowIndex, obj) {
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var gridName = 'gridProgramacao'
        var grid = dijit.byId(gridName);

        if (tipoTurma == TURMA_PPT) { //Programação filha
            gridName = 'gridProgramacaoPPT';
            grid = dijit.byId(gridName);
        }

        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosProgramacao');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_aula_programacao_turma", grid._by_idx[rowIndex].item.nm_aula_programacao_turma);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_aula_programacao_turma', 'selecionadoProgramacao', -1, 'selecionaTodosProgramacao', 'selecionaTodosProgramacao', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'nm_aula_programacao_turma', 'selecionadoProgramacao', " + rowIndex + ", '" + id + "', 'selecionaTodosProgramacao', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxHorarioProf(value, rowIndex, obj) {
    try {
        var gridName = 'gridHorarioProfessor'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosHorarioProf');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id", grid._by_idx[rowIndex].item.id);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'id', 'horarioProfSelecionado', -1, 'selecionaTodosHorarioProf', 'selecionaTodosHorarioProf', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'id', 'horarioProfSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosHorarioProf', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAlunos(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunos'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAluno');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'alunoSelecionado', -1, 'selecionaTodosAluno', 'selecionaTodosAluno', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'alunoSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosAluno', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatImgCompartilhada(value, rowIndex, obj)
{
    var gridName = 'gridTurma'
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);

        value = value || indice != null; // Item está selecionado.
    }
    value = hasValue(grid._by_idx[rowIndex].item) ? grid._by_idx[rowIndex].item.dc_escola_origem : "";
    if (value != null)
        value = value.toString().replaceAll(' ', '&nbsp;');
    if (rowIndex != -1)
        if (value != null && value != "")
            icon = "<img alt='' src='" + Endereco() + "/images/compartilhada.png' id='imgObscompartilhada' title=" + value + ">"
        else
            icon = null;
    return icon
}

function criarElementosTagProfessores(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest, Pagination) {
    try {
        new Button({
            label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                var tipoVerif = dojo.byId("tipoVerif").value;
                var gridProfessor = dijit.byId('gridProfessor');
                if (hasValue(tipoVerif) && tipoVerif == VERIFITURMAFILHAPPT2MODAL) {
                    gridProfessor = dijit.byId('gridProfessorPPT');
                }
                var antigoValor = gridProfessor.itemSelecionado.id_professor_ativo;
                var gridHorarioProfessor = dijit.byId('gridHorarioProfessor');
                var horariosDeletados = [];
                var horariosTurmaPPTClone = new Array();
                var gridHorarios = dijit.byId("gridHorarios");
                //Altera os horários dos professores na grid de horarios:
                if (gridHorarioProfessor.store.objectStore.data.length <= 0) {
                    //Garante que o professor tenha um horário na turma:
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrDelHorarioProfessor);
                    apresentaMensagem("apresentadorMensagemProfessor", mensagensWeb);
                    return false;
                }

                //Altera os horarios da grid de professor conforme os novos da grid de horarios e tambem da grid de horarios da turma:
                var horariosProfessor = gridHorarioProfessor.store.objectStore.data;
                var horariosTurma = new Array();
                if (tipoVerif == VERIFTURMANORMAL) {
                    horariosTurmaPPTClone = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value); //Se o professor estiver no horário da turma e o horário não tiver na grid do horario do professor, remove da turma:
                    horariosTurma = clonarHorarios(horariosTurmaPPTClone);
                }
                else
                    horariosTurma = validaRetornaHorariosFIlhaPPT();

                for (var i = 0; i < horariosTurma.length; i++)
                    if (horariosTurma[i].calendar == "Calendar2" || horariosTurma[i].calendar == null)
                        for (var j = horariosTurma[i].HorariosProfessores.length - 1; j >= 0; j--)
                            if (horariosTurma[i].HorariosProfessores[j].cd_professor == gridProfessor.itemSelecionado.cd_professor) {
                                quickSortObj(horariosProfessor, 'id');

                                var posicao = binaryObjSearch(horariosProfessor, 'id', horariosTurma[i].id);

                                if (hasValue(horariosProfessor) && !hasValue(posicao, true)) {
                                    horariosDeletados.push({
                                        HorariosProfessores: horariosTurma[i].HorariosProfessores[j],
                                        horariosTurma: horariosTurma[i]
                                    });
                                    horariosTurma[i].HorariosProfessores.splice(j, 1);
                                }
                            }

                if (tipoVerif == VERIFTURMANORMAL) {
                    var retorno = replicarMudancasPofessorNasTurmasFilhas(horariosDeletados, dijit.byId('situacaoProfessorTurma').checked, gridProfessor.itemSelecionado.cd_professor);
                    if (retorno) {
                        gridProfessor.itemSelecionado.id_professor_ativo = dijit.byId('situacaoProfessorTurma').checked;
                        removeObjSort(gridProfessor.store.objectStore.data, "cd_professor", gridProfessor.itemSelecionado.cd_professor);
                        insertObjSort(gridProfessor.store.objectStore.data, "cd_professor", gridProfessor.itemSelecionado);
                        gridProfessor.itemSelecionado.horarios = horariosProfessor;
                        gridProfessor.update();
                        var cdTurma = dojo.byId("cd_turma").value ? dojo.byId("cd_turma").value : 0;
                        var gridTurma = dijit.byId("gridTurma");
                        if (hasValue(gridHorarios) && gridHorarios.items.length > 0) {
                            $.each(horariosTurma, function (index, value) {
                                if (value.calendar == "Calendar2") {
                                    if (value.id != null || value.id >= 0)
                                        gridHorarios.store.remove(value.id);
                                    gridHorarios.store.add(value);
                                }
                            });
                            //horarios = gridHorarios.params.store.data;
                        }
                        else if ((hasValue(cdTurma) && cdTurma > 0) || (hasValue(cdTurmaPPT) && cdTurmaPPT > 0)) {
                            if (hasValue(gridTurma) && hasValue(gridTurma.TurmaEdit) && hasValue(gridTurma.TurmaEdit.horariosTurma))
                                gridTurma.TurmaEdit.horariosTurma = horariosTurma;
                        }
                    }
                    else {
                        return false;
                    }
                } else {
                    gridProfessor.itemSelecionado.id_professor_ativo = dijit.byId('situacaoProfessorTurma').checked;
                    removeObjSort(gridProfessor.store.objectStore.data, "cd_professor", gridProfessor.itemSelecionado.cd_professor);
                    insertObjSort(gridProfessor.store.objectStore.data, "cd_professor", gridProfessor.itemSelecionado);
                    gridProfessor.update();
                }
                //Verifica se ativou/inativou o professor e atualiza a grade de horários ocupados dos professoes:
                if (hasValue(tipoVerif) && tipoVerif == VERIFTURMANORMAL && antigoValor != dijit.byId('situacaoProfessorTurma').checked)
                    if (dijit.byId('situacaoProfessorTurma').checked)
                        validaBuscaHorariosProfessor(gridProfessor.itemSelecionado.cd_professor);
                    else
                        limparHorariosProfTurma(gridProfessor.itemSelecionado.cd_professor);

                dijit.byId("dialogProfessor").hide();
            }
        }, "alterarProf");
        new Button({
            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                try {
                    var tipoVerif = dojo.byId("tipoVerif").value;
                    var gridProfessor = dijit.byId('gridProfessor');
                    if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                        gridProfessor = dijit.byId('gridProfessorPPT');
                    clearForm('formProfessor');
                    apresentaMensagem('apresentadorMensagemProfessor', null);
                    keepValuesProfessor(null, gridProfessor, null, xhr, Memory, EnhancedGrid);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "cancelarProf");
        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogProfessor").hide(); } }, "fecharProf");

        new Button({
            label: "Incluir",
            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
            onClick: function () {
                try {
                    var tipoTurma = dojo.byId("tipoTurmaCad").value;
                    if ((!LIBERAR_HABILITACAO_PROFESSOR || tipoTurma == TURMA_PPT) || (LIBERAR_HABILITACAO_PROFESSOR && hasValue(dijit.byId("pesCadCurso").value))) {
                        //getProfessoresDisponiveisFaixaHorariovar tipoTurma = dojo.byId("tipoTurmaCad").value;
                        configLayoutPesquisaProf(tipoTurma == TURMA_PPT_FILHA);
                        dojo.byId("tipoVerif").value = VERIFTURMANORMAL;
                        var cdTurma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
                        var cdTurmaPPT = hasValue(dojo.byId("cd_turma_ppt").value) ? dojo.byId("cd_turma_ppt").value : 0;
                        limparPesquisaProfessorFK();
                        //dojo.byId("setValuePesquisaAlunoFK").value = PESQUISAALUNO;
                        var horarios = [];
                        dijit.byId("gridPesquisaProfessor").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                        horarios = validaRetornaHorarios(tipoTurma);
                        if (horarios == null || horarios.length <= 0) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrInclHorarioTurma);
                            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                            return false;
                        }
                        if (tipoTurma == TURMA_PPT_FILHA)
                            pesquisaProfessorHorarioPorHorariosTurmaFilha(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horarios, cdTurmaPPT);
                        else
                            pesquisaProfessorDisponivelHorario(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horarios, cdTurma, cdTurmaPPT);

                        //Mostra a linha de escolha dos horários e os popula com relação aos horários da turma:
                        showP('trHorarios', true);

                        var horariosTurma = validaRetornaHorarios(tipoTurma);
                        horariosTurma = cloneArray(horariosTurma);
                        for (var i = horariosTurma.length - 1; i >= 0; i--)
                            if (horariosTurma[i].calendar == "Calendar2" || horariosTurma[i].calendar == null) {
                                horariosTurma[i].dia_semana = getDiaSemana(horariosTurma[i].id_dia_semana - 1);
                                horariosTurma[i].no_datas = horariosTurma[i].dt_hora_ini + ' / ' + horariosTurma[i].dt_hora_fim + ' - ' + horariosTurma[i].dia_semana;
                            }
                            else //Remove da lista os horários que não são ocupados da turma
                                horariosTurma.splice(i, 1);

                        var strCbHorarios = 'cbHorarios';
                        var cbHorarios = dijit.byId(strCbHorarios);

                        //Ordena os horários:
                        var sorto = {
                            id_dia_semana: "asc", dt_hora_ini: "asc", dt_hora_fim: "asc"
                        };
                        horariosTurma.keySort(sorto);

                        cbHorarios._onChangeActive = false;
                        loadMultiSelect(horariosTurma, strCbHorarios, 'id', 'no_datas', true);
                        cbHorarios._onChangeActive = true;

                        if (hasValue(cbHorarios.handle))
                            cbHorarios.handle.remove();

                        cbHorarios.handle = cbHorarios.on("change", function (e) {
                            var horariosSelecionados = getHorariosProfSelecionados();

                            dijit.byId("gridPesquisaProfessor").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                            if (hasValue(horariosSelecionados) && horariosSelecionados.length > 0)
                                if (tipoTurma == TURMA_PPT_FILHA)
                                    pesquisaProfessorHorarioPorHorariosTurmaFilha(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horariosSelecionados, cdTurmaPPT);
                                else
                                    pesquisaProfessorDisponivelHorario(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horariosSelecionados, cdTurma);
                        });

                        dijit.byId("proProfessor").show();
                    } else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPesquisaProfessorCurso);
                        apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                        return false;
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }

            }
        }, "btnNovoProfessor");

        var menuProfessor = new DropDownMenu({ style: "height: 25px" });
        var acaoExcluirProfessor = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try {
                    //Remove os professores dos horários da turma:
                    var gridAlunosPPT = dijit.byId("gridAlunosPPT");
                    var gridProfessor = dijit.byId("gridProfessor");
                    var tipoTurma = dojo.byId("tipoTurmaCad").value;
                    var horariosOcupadoTurma = validaRetornaHorarios(tipoTurma);
                    var horarios = new Array();
                    if (horariosOcupadoTurma.length > 0)
                        $.each(horariosOcupadoTurma, function (index, value) {
                            if (value.calendar == "Calendar2" || !hasValue(value.calendar))
                                horarios.push(value);
                        });

                    for (var k = 0; k < gridProfessor.itensSelecionados.length; k++)
                        for (var l = 0; l < horarios.length; l++)
                            if (hasValue(horarios[l]) && hasValue(horarios[l].HorariosProfessores))
                                for (var s = horarios[l].HorariosProfessores.length - 1; s >= 0; s--)
                                    if (horarios[l].HorariosProfessores[s].cd_professor == gridProfessor.itensSelecionados[k].cd_professor)
                                        horarios[l].HorariosProfessores.splice(s, 1);

                    if (tipoTurma == TURMA_PPT) {
                        if (hasValue(gridAlunosPPT) && gridAlunosPPT.listaCompletaAlunos.length > 0) {
                            //Percorre a lista de todas as turmas Filhas
                            $.each(gridAlunosPPT.listaCompletaAlunos, function (dex, v) {
                                var horariosFilha = v.horariosTurma;

                                for (var k = 0; k < gridProfessor.itensSelecionados.length; k++) {
                                    for (var l = 0; l < horariosFilha.length; l++)
                                        if (hasValue(horarios[l]) && hasValue(horarios[l].HorariosProfessores))
                                            for (var s = horariosFilha[l].HorariosProfessores.length - 1; s >= 0; s--)
                                                if (horariosFilha[l].HorariosProfessores[s].cd_professor == gridProfessor.itensSelecionados[k].cd_professor)
                                                    horariosFilha[l].HorariosProfessores.splice(s, 1);
                                    //Remove os professores da lista dos professores da(s) turma(s) filha(s):
                                    removeObjSort(v.ProfessorTurma, "cd_professor", gridProfessor.itensSelecionados[k].cd_professor);
                                }
                            });
                            //Percorrendo a lista da view
                            if (hasValue(gridAlunosPPT.store.objectStore.data))
                                $.each(gridAlunosPPT.store.objectStore.data, function (dex, v) {
                                    var horariosFilha = v.horariosTurma;

                                    for (var k = 0; k < gridProfessor.itensSelecionados.length; k++) {
                                        for (var l = 0; l < horariosFilha.length; l++)
                                            if (hasValue(horarios[l]) && hasValue(horarios[l].HorariosProfessores))
                                                for (var s = horariosFilha[l].HorariosProfessores.length - 1; s >= 0; s--)
                                                    if (horariosFilha[l].HorariosProfessores[s].cd_professor == gridProfessor.itensSelecionados[k].cd_professor)
                                                        horariosFilha[l].HorariosProfessores.splice(s, 1);
                                        //Remove os professores da lista dos professores da(s) turma(s) filha(s):
                                        removeObjSort(v.ProfessorTurma, "cd_professor", gridProfessor.itensSelecionados[k].cd_professor);
                                    }
                                });
                        }
                    }

                    //Remove os professores da lista dos professores da turma:
                    if (hasValue(gridProfessor.itensSelecionados) && gridProfessor.itensSelecionados.length > 0)
                        $.each(gridProfessor.itensSelecionados, function (x, vl) {
                            limparHorariosProfTurma(vl.cd_professor);
                        });

                    deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_professor', gridProfessor);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }

        });

        menuProfessor.addChild(acaoExcluirProfessor);

        var acaoEditarProfessor = new MenuItem({
            label: "Editar",
            onClick: function () {
                var gridProfessor = dijit.byId('gridProfessor');
                var itensSelecionados = gridProfessor.itensSelecionados;
                if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
                else if (itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
                else {
                    var dialogProfessor = dijit.byId("dialogProfessor");
                    dojo.byId("tipoVerif").value = VERIFTURMANORMAL;
                    keepValuesProfessor(null, gridProfessor, true, dojo.xhr, dojo.store.Memory, dojox.grid.EnhancedGrid);
                    //IncluirAlterar(0, 'divAlterarProf', null, null, 'apresentadorMensagemProfessor', 'divCancelarProf', null);
                    dialogProfessor.on('show', function () {
                        dijit.byId('paiGridHorarioProfessor').resize();
                        dijit.byId('gridHorarioProfessor').update();
                    });
                    dialogProfessor.show();
                }
            }
        });
        menuProfessor.addChild(acaoEditarProfessor);

        var buttonProfessor = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasProfessor",
            dropDown: menuProfessor,
            id: "acoesRelacionadasProfessor"
        });
        dojo.byId("linkAcoesProfessor").appendChild(buttonProfessor.domNode);

        var gridProfessor = new EnhancedGrid({
            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
              [
                { name: "<input id='selecionaTodosProfessor' style='display:none'/>", field: "selecionadoProfessor", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProfessor },
                { name: "Nome", field: "no_professor", width: "85%" },
                { name: "Ativo ", field: "id_professor_ativo", width: "10%", styles: "text-align: center;", formatter: formatCheckBoxProfessorAtivo }
              ],
            canSort: true,
            selectionMode: "single",
            noDataMessage: msgNotRegEnc,
            plugins: {
                pagination: {
                    pageSizes: ["10", "30", "60", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "10",
                    gotoButton: true,
                    /*page step to be displayed*/
                    maxPageStep: 5,
                    /*position of the pagination bar*/
                    position: "button",
                    plugins: { nestedSorting: false }
                }
            }
        }, "gridProfessor");
        gridProfessor.canSort = function (col) { return Math.abs(col) != 1; };
        gridProfessor.on("RowDblClick", function (evt) {
            try {
                var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                var dialogProfessor = dijit.byId("dialogProfessor");

                apresentaMensagem('apresentadorMensagemProfessor', null);
                gridProfessor.itemSelecionado = item;
                dojo.byId("tipoVerif").value = dojo.byId("tipoVerif").value = VERIFTURMANORMAL;
                keepValuesProfessor(item, gridProfessor, false, xhr, Memory, EnhancedGrid);
                //IncluirAlterar(0, 'divAlterarProf', null, null, 'apresentadorMensagemProfessor', 'divCancelarProf', null);
                dialogProfessor.on('show', function () {
                    dijit.byId('paiGridHorarioProfessor').resize();
                    dijit.byId('gridHorarioProfessor').update();
                });
                dialogProfessor.show();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, true);
        gridProfessor.startup();

        dijit.byId("tagProfessores").on("show", function (e) {
            dijit.byId("paiProfessor").resize();
            dijit.byId("gridProfessor").update();
        });
        document.getElementById('divGridPesquisaProfessor').style.height = '383px';
        //if (!hasValue(dijit.byId('situacaoProfessorTurma')))
        //montarStatusSingular("situacaoProfessorTurma", true);

        dijit.byId("pesquisarProfessorFK").on("click", function (e) {
            var horariosSelecionados = getHorariosProfSelecionados();
            var cdTurma = dojo.byId('cd_turma').value;

            desabilitaCampo(0);
            dijit.byId("gridPesquisaProfessor").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            if ((!LIBERAR_HABILITACAO_PROFESSOR || tipoTurma == TURMA_PPT) || (LIBERAR_HABILITACAO_PROFESSOR && hasValue(dijit.byId("pesCadCurso").value))) {
                pesquisaProfessorDisponivelHorario(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horariosSelecionados, cdTurma);
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPesquisaProfessorCurso);
                apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            }
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodo que replica as acções executadas na edição ao dados do professor na turma ppt.
function replicarMudancasPofessorNasTurmasFilhas(horariosProf, profAtivo, cd_professor) {
    try {
        var retorno = true;
        var cloneAlunosTurma = new Array();
        var gridAlunosPPT = dijit.byId("gridAlunosPPT");
        var gridProfessorPPT = dijit.byId("gridProfessorPPT");
        //gridAlunosPPT.store.objectStore.data
        if (hasValue(gridAlunosPPT) && gridAlunosPPT.listaCompletaAlunos.length > 0) {
            cloneAlunosTurma = gridAlunosPPT.listaCompletaAlunos;
            retorno = aplicarMudancasPofessorNaListaTurmasFilhas(gridAlunosPPT.listaCompletaAlunos, profAtivo, cd_professor, horariosProf);
            if (hasValue(gridAlunosPPT.store.objectStore.data))
                aplicarMudancasPofessorNaListaTurmasFilhas(gridAlunosPPT.store.objectStore.data, profAtivo, cd_professor, horariosProf);
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function aplicarMudancasPofessorNaListaTurmasFilhas(lista, profAtivo, cd_professor, horariosProf) {
    var horarioProfResultante = [];
    var cloneHorariosProf = new Array();
    var gerarErro = false;
    var retorno = true;
    //Percorre a lista de turmas Filhas
    $.each(lista, function (dex, v) {
        var sair = false;
        if (hasValue(v.ProfessorTurma))
            //Percorre a lista de professor da turma filha.
            $.each(v.ProfessorTurma, function (idx, val) {
                if (hasValue(val.horarios))
                    cloneHorariosProf = val.horarios;
                if (val.cd_professor == cd_professor && val.id_professor_ativo == true) {
                    val.id_professor_ativo = profAtivo;
                    //Percorre a lista de horarios deletados do professor na turma PPT.
                    $.each(horariosProf, function (x, vl) {
                        //Percorre lista de horariosProfessor para verificar se e igual ou esta contido no horário deletado na turma PPT, se for ele é removido.
                        for (var j = val.horarios.length - 1; j >= 0; j--) {
                            if (hasValue(val.horarios[j]) && hasValue(vl.horariosTurma))
                                if (vl.horariosTurma.id_dia_semana == val.horarios[j].id_dia_semana &&
                                    val.horarios[j].dt_hora_ini >= vl.horariosTurma.dt_hora_ini && val.horarios[j].dt_hora_ini < vl.horariosTurma.dt_hora_fim &&
                                    val.horarios[j].dt_hora_fim > vl.horariosTurma.dt_hora_ini && val.horarios[j].dt_hora_fim <= vl.horariosTurma.dt_hora_fim) {
                                    if (val.horarios.length == 1) {
                                        val.horarios = cloneHorariosProf;
                                        sair = true;
                                        gerarErro = true;
                                        return false;
                                    }
                                    else {
                                        val.horarios.splice(j, 1);
                                    }
                                }
                            if (sair)
                                return false;
                        }
                        if (sair)
                            return false;
                    });
                    //lista atual de horáros do professor da turma filha.
                    horarioProfResultante = val.horarios;
                }
                if (sair)
                    return false;
            });

        if (gerarErro) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroProfessorSemHorarioTurmasFilhas);
            apresentaMensagem("apresentadorMensagemProfessor", mensagensWeb);
            retorno = false;
            return false;
        }

        //Segunda parte do metodo
        //Percorre a lista de horário das turmas filhas e remove a referencia HorariosProfessores do horário.
        //Altera os horarios da grid de professor conforme os novos da grid de horarios e tambem da grid de horarios da turma:
        var horariosProfessor = horarioProfResultante.slice();
        if (hasValue(v.horariosTurma) && v.horariosTurma.length > 0) {
            var horariosTurma = v.horariosTurma; //Se o professor estiver no horário da turma e o horário não tiver na grid do horario do professor, remove da turma:
            for (var i = 0; i < horariosTurma.length; i++)
                for (var j = horariosTurma[i].HorariosProfessores.length - 1; j >= 0; j--)
                    if (horariosTurma[i].HorariosProfessores[j].cd_professor == cd_professor) {
                        quickSortObj(horariosProfessor, 'id');

                        var posicao = binaryObjSearch(horariosProfessor, 'id', horariosTurma[i].id);

                        if (hasValue(horariosProfessor) && !hasValue(posicao, true))
                            horariosTurma[i].HorariosProfessores.splice(j, 1);
                    }
        }
    });
    return retorno;
}

function getHorariosProfSelecionados() {
    try {
        var horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);
        var horariosSelecionados = new Array();
        var idHorariosSelecionados = dijit.byId('cbHorarios').value;

        //Seleciona somente os horários marcados:
        quickSortObj(horarios, 'id');

        for (var i = 0; i < idHorariosSelecionados.length; i++) {
            var posicao = binaryObjSearch(horarios, 'id', idHorariosSelecionados[i]);
            if (hasValue(posicao, true))
                horariosSelecionados.push(horarios[posicao]);
        }

        return horariosSelecionados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getHorariosProfSelecionadosFilhasPPT() {
    try {
        var horarios = validaRetornaHorariosFIlhaPPT();
        var horariosSelecionados = new Array();
        var idHorariosSelecionados = dijit.byId('cbHorarios').value;

        //Seleciona somente os horários marcados:
        quickSortObj(horarios, 'id');

        for (var i = 0; i < idHorariosSelecionados.length; i++) {
            var posicao = binaryObjSearch(horarios, 'id', idHorariosSelecionados[i]);
            if (hasValue(posicao, true))
                horariosSelecionados.push(horarios[posicao]);
        }

        return horariosSelecionados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxProfessor(value, rowIndex, obj) {
    try {
        var gridName = 'gridProfessor';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_professor", grid._by_idx[rowIndex].item.cd_professor);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_professor', 'professorSelecionado', -1, 'selecionaTodosProfessor', 'selecionaTodosProfessor', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_professor', 'professorSelecionado', " + rowIndex + ", '" + id + "', 'selecionaTodosProfessor', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxProfessorAtivo(value, rowIndex, obj) {
    try {
        var gridProfessor = dijit.byId("gridProfessor");
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();
        if (rowIndex != -1 && (hasValue(value) || value == false))
            icon = "<input class='formatCheckBox'  id='" + id + "' /> ";

        setTimeout("configuraCheckBoxProfessorAtivo(" + value + ", '" + rowIndex + "', '" + id + "')", 10);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxProfessorAtivo(value, rowIndex, id) {
    try {
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                name: "checkBox",
                checked: value,
                onChange: function (b) { checkBoxChangeProfessorAtivo(rowIndex) }
            }, id);
            checkBox.set('disabled', true);
        }
        else {
            var dijitObj = dijit.byId(id);

            dijitObj._onChangeActive = false;
            dijitObj.set('value', value);
            dijitObj._onChangeActive = true;
            dijitObj.onChange = function (b) { checkBoxChangeProfessorAtivo(rowIndex) }
            dijitObj.set('disabled', true);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeProfessorAtivo(rowIndex) {
    try {
        var gridProfessor = dijit.byId('gridProfessor');
        gridProfessor._by_idx[rowIndex].item.id_professor_ativo = !gridProfessor._by_idx[rowIndex].item.id_professor_ativo
        if (!gridProfessor._by_idx[rowIndex].item.id_professor_ativo)
            limparHorariosProfTurma(gridProfessor._by_idx[rowIndex].item.cd_professor);
        else
            validaBuscaHorariosProfessor(gridProfessor._by_idx[rowIndex].item.cd_professor);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarElementosTagAlunosPPT(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest, ready, filteringSelect, Pagination) {
    try {
        //ALUNO PPT
        var gridAlunoPPT = new EnhancedGrid({
            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
              [
                { name: "<input id='selecionaTodosAlunoPPT' style='display:none'/>", field: "selecionadoAlunoPPT", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAlunoPPT },
                { name: "Nome", field: "no_aluno", width: "35%" },
                { name: "Curso", field: "no_curso", width: "30%" },
                { name: "Situação", field: "situacaoAlunoTurmaFilhaPPT", width: "25%" }
              ],
            canSort: false,
            noDataMessage: msgNotRegEnc,
            selectionMode: "single",
            plugins: {
                pagination: {
                    pageSizes: ["10", "30", "60", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "10",
                    gotoButton: true,
                    /*page step to be displayed*/
                    maxPageStep: 5,
                    /*position of the pagination bar*/
                    position: "button",
                    plugins: { nestedSorting: false }
                }
            }
        }, "gridAlunosPPT");
        gridAlunoPPT.on("StyleRow", function (row) {
            try {
                var item = gridAlunoPPT.getItem(row.index);
                if (hasValue(item)) {
                    if (item.alunoTurma.cd_situacao_aluno_turma == DESISTENTE)
                        row.customClasses += " RedRow";
                    if (item.alunoTurma.cd_situacao_aluno_turma == MOVIDO)
                        row.customClasses += " BlueRow";
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        gridAlunoPPT.canSort = function (col) { return Math.abs(col) != 1; };
        gridAlunoPPT.startup();
        gridAlunoPPT.listaCompletaAlunos = [];
        new Button({
            label: "Incluir",
            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
            onClick: function () {
                try {
                    //limparPesquisaCursoFKT(true);
                    limparAlunoCurso();
                    //limparPesquisaAlunoFK();
                    var horarios = [];
                    horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);
                    if (horarios == null || horarios.length <= 0) {
                        var mensagensWeb = [];
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrInclHorarioTurma);
                        apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                        return false;
                    }
                    var produto = dijit.byId('pesCadProduto').get('value');
                    var duracao = dijit.byId('pesCadDuracao').get('value');
                    var dtInicio = dijit.byId('dtIniAula').get('value');

                    if ((produto == null || produto <= 0) || (duracao == null || duracao <= 0) || (dtInicio == null || dtInicio <= 0)) {
                        var mensagensWeb = [];
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgConsitirCamposPPTPai);
                        apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                        return false;
                    }
                    dijit.byId("dialogAlunoPPT").show();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "btnNovoAlunoPPT");

        var menuAlunoPPT = new DropDownMenu({ style: "height: 25px" });
        var acaoExcluirAlunoPPT = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try {
                    deletarItemSelecionadoAlunoGrid(Memory, ObjectStore, 'id', dijit.byId("gridAlunosPPT"));
                    var tipoTurma = dojo.byId("tipoTurmaCad").value;
                    var totalAlunos = hasValue(dijit.byId("gridAlunosPPT")) ? dijit.byId("gridAlunosPPT").listaCompletaAlunos.length : 0;
                    habilitarCamposTurma(tipoTurma, totalAlunos);
                    habilitarBotaoAluno();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menuAlunoPPT.addChild(acaoExcluirAlunoPPT);

        var acaoAbrirAlunoPPT = new MenuItem({
            label: "Turma do Aluno",
            onClick: function () {
                eventoEditarAlunoPPT(gridAlunoPPT.itensSelecionados, ready, Memory, filteringSelect);
            }
        });
        menuAlunoPPT.addChild(acaoAbrirAlunoPPT);

        var acaoMatriculaPPT = new MenuItem({
            label: "Matrícula",
            onClick: function () {
                populaContrato(xhr, Memory, filteringSelect, ready, ObjectStore, ref, dijit.byId("gridAlunosPPT"), true);
            }
        });
        menuAlunoPPT.addChild(acaoMatriculaPPT);

        var buttonAlunoPPT = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasAlunoPPT",
            dropDown: menuAlunoPPT,
            id: "acoesRelacionadasAlunoPPT"
        });
        dojo.byId("linkAcoesAlunoPPT").appendChild(buttonAlunoPPT.domNode);

        var menuFiltroAlunoPPT = new DropDownMenu({ style: "height: 25px", id: "menuFiltroAlunoPPT" });
        menuFiltroAlunoPPT.addChild(new MenuItem({
            id: "filtroSit" + 100,
            name: "todasFiltroAluno",
            label: "Todas",
            onClick: function (e) { montarFiltroSituacaoALunoTurmaPPT(e); }
        }));
        var buttonFiltroAluno = new DropDownButton({
            label: "Situações",
            name: "acoesRelacionadasFiltroAlunoPPT",
            dropDown: menuFiltroAlunoPPT,
            id: "acoesRelacionadasFiltroAlunoPPT"
        });
        dojo.byId("linkFiltroAcoesAlunoPPT").appendChild(buttonFiltroAluno.domNode);

        dijit.byId("tagAlunosPPT").on("show", function (e) {
            dijit.byId("paiAlunoPPT").resize();
            dijit.byId("gridAlunosPPT").update();
        });

        dijit.byId("proAlunoPPT").on("click", function (e) {
            try {
                if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                    montarGridPesquisaAluno(false, function () {
                        pesquisarAlunoNaTurmaPPT();
                    });
                }
                else {
                    limparPesquisaAlunoFK();
                    pesquisarAlunoNaTurmaPPT();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });

        dijit.byId("pesquisarCursoFK").on("click", function (e) {
            apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
            pesquisarCursoFK();
        });

        dijit.byId("proCurso").on("Show", function (e) {
            dijit.byId("cbPesqProdutoFK").set("value", dijit.byId("pesCadProduto").value);
            dijit.byId("gridPesquisaCurso").itensSelecionados = [];
            dijit.byId("gridPesquisaCurso").update();
        });

        dijit.byId("proCursoPPT").on("click", function (e) {
            try {
                dijit.byId("cbPesqProdutoFK").set("disabled", true);
                if (hasValue(dijit.byId("pesCadProduto").value)) {
                    dijit.byId("cbPesqProdutoFK").set("value", dijit.byId("pesCadProduto").value);
                    dijit.byId("cbPesqProdutoFK").produto_selecionado = dijit.byId("pesCadProduto").value;
                }
                apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                limparPesquisaCursoFKT(true);
                pesquisarCursoFK();
                dijit.byId("proCurso").show();
            }
            catch (e) {
                postGerarLog(e);
            }
        });

        new Button({
            label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                dijit.byId("dialogAlunoPPT").hide();
            }
        }, "btnFecharPPT");
        new Button({
            label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', onClick: function () {
                limparAlunoCurso();
            }
        }, "btnLimparPPT");

        new Button({
            label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                try {
                    valido = true;
                    var pesCadDuracao = dijit.byId("pesCadDuracao");
                    if (!dijit.byId("formDescAlunoPPT").validate() || dojo.byId("cdAlunoPPT").value <= 0) {
                        valido = false;
                    }
                    if (!dijit.byId("formDescCursoPPT").validate() || dojo.byId("cdCursoPPT").value <= 0)
                        valido = false;
                    if (!valido)
                        return false;
                    var professorTurma = new Array();
                    var cdTurma = dojo.byId("cd_turma").value;
                    var gridHorarios = dijit.byId("gridHorarios");
                    var gridTurma = dijit.byId("gridTurma");
                    var gridAlunoCurso = dijit.byId("gridAlunosPPT");
                    var gridProfessor = dijit.byId("gridProfessor");
                    var horarios = new Array();
                    if (hasValue(gridHorarios) && gridHorarios.items.length > 0)
                        horarios = mountHorariosOcupadosTurma(gridHorarios.params.store.data);
                    else if (hasValue(cdTurma) && cdTurma > 0) {
                        if (hasValue(gridTurma) && hasValue(gridTurma.TurmaEdit) && hasValue(gridTurma.TurmaEdit.horariosTurma))
                            horarios = gridTurma.TurmaEdit.horariosTurma;
                    }
                    if (hasValue(gridProfessor)) {
                        professorTurma = clone(gridProfessor.store.objectStore.data);
                        professorTurma = clonarProfessorTurmaEHorarios(professorTurma);
                    }
                    if (horarios.length > 0)
                        horarios = cloneHorarioPPTParaFilha(horarios, professorTurma, CLONARPPT);
                    var newAlunoCurso = {
                        id: geradorIdAlunoPPT(gridAlunoCurso),
                        cd_turma: 0,
                        cd_duracao: hasValue(dijit.byId("pesCadRegime").value) ? dijit.byId("pesCadRegime").value : 0,
                        cd_regime: dijit.byId("pesCadRegime").value,
                        cd_curso: dojo.byId("cdCursoPPT").value,
                        no_curso: dojo.byId("descCursoPPT").value,
                        cd_aluno: parseInt(dojo.byId("cdAlunoPPT").value),
                        no_aluno: dojo.byId("descAlunoPPT").value,
                        cd_sala: hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : null,
                        cd_sala_online: hasValue(dijit.byId("cbSalaOnLine").value) ? dijit.byId("cbSalaOnLine").value : null,

                        no_turma: "",
                        no_regime: "",
                        no_apelido: "",
                        dta_termino_turma: "",
                        dt_final_aula: "",
                        dta_inicio_aula: dojo.byId("dtIniAula").value,
                        dt_inicio_aula: null,
                        alterouProgramacaoOuDescFeriado: false,
                        ProgramacaoTurma: null,
                        alunoTurma: {cd_situacao_aluno_turma: AGUARDANDO_MATRICULA},
                        horariosTurma: horarios,
                        ProfessorTurma: professorTurma,
                        situacaoAlunoTurmaFilhaPPT: msgAguardandoMat,
                        alunosTurma: [{
                            cd_aluno: dojo.byId("cdAlunoPPT").value,
                            no_aluno: dojo.byId("descAlunoPPT").value,
                            cd_situacao_aluno_turma: AGUARDANDO_MATRICULA
                        }]
                    }
                    if (hasValue(newAlunoCurso.dta_inicio_aula))
                        newAlunoCurso.dt_inicio_aula = dojo.date.locale.parse(newAlunoCurso.dta_inicio_aula, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    if (hasValue(pesCadDuracao) && pesCadDuracao > 0) {
                        newAlunoCurso.cd_duracao = pesCadDuracao.value;
                        newAlunoCurso.no_regime = pesCadDuracao.displayedValue;
                    }

                    
                    var verificaAlunosCurso = jQuery.grep(gridAlunoCurso.listaCompletaAlunos, function (value) {
                        return value.cd_aluno == newAlunoCurso.cd_aluno && parseInt(value.cd_curso) == newAlunoCurso.cd_curso;
                    });
                    if (!hasValue(verificaAlunosCurso) || verificaAlunosCurso.length <= 0)
                    {
                        insertObjSort(gridAlunoCurso.listaCompletaAlunos, "id", newAlunoCurso, false);
                        insertObjSort(gridAlunoCurso.store.objectStore.data, "id", newAlunoCurso, false);
                        gridAlunoCurso.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAlunoCurso.store.objectStore.data, idProperty: "id" }) }));

                        var tipoTurma = dojo.byId("tipoTurmaCad").value;
                        var totalAlunos = gridAlunoCurso.listaCompletaAlunos.length;
                        desabilitarCamposTuma(tipoTurma, totalAlunos);
                        habilitarBotaoAluno();
                        dijit.byId("dialogAlunoPPT").hide();
                    }else{
                        dijit.byId("dialogAlunoPPT").hide();
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "btnIncluirPPT");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function geradorIdAlunoPPT(gridAlunoCurso) {
    try {
        var id = gridAlunoCurso.store.objectStore.data.length;
        var itensArray = gridAlunoCurso.store.objectStore.data.sort(function byOrdem(a, b) { return a.id - b.id; });
        if (!hasValue(id))
            id = 1;
        else 
            id = itensArray[id - 1].id + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAlunoPPT(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunosPPT'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAlunoPPT');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id", grid._by_idx[rowIndex].item.id);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:12px'   id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'id', 'selecionadoAlunoPPT', -1, 'selecionaTodosAlunoPPT', 'selecionaTodosAlunoPPT', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'id', 'selecionadoAlunoPPT', " + rowIndex + ", '" + id + "', 'selecionaTodosAlunoPPT', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaAlunoDisponivelHorario(horarios, cdTurma, cd_produto, id_turma_PPT, cd_curso, cd_duracao) {
    try {
        var horariosOcupadoTurma = [];
        if (horarios.length > 0)
            $.each(horarios, function (index, value) {
                if (value.calendar == "Calendar2")
                    horariosOcupadoTurma.push(value);
            });
        dojo.xhr.post({
            url: Endereco() + "/turma/postHorariosTurma",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(horariosOcupadoTurma)
        }).then(function (data) {
            var myStore =
                 dojo.store.Cache(
                         dojo.store.JsonRest({
                             target: Endereco() + "/aluno/getAlunosDisponiveisFaixaHorario?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value +
                                   "&status=" + dijit.byId("statusAlunoFK").value + "&cpf=" + dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked +
                                   "&sexo=" + dijit.byId("nm_sexo_fk").value + "&cdTurma=" + cdTurma + "&cd_produto=" + cd_produto + "&id_turma_PPT=" + id_turma_PPT + 
                                   "&dta_final_aula=" + dojo.byId("dtIniAula").value + "&cd_curso=" + cd_curso + "&cd_duracao=" + cd_duracao,
                             handleAs: "json",
                             preventCache: true,
                             headers: { "Accept": "application/json", "Authorization": Token() }
                         }), dojo.store.Memory({}));
            var gridAluno = dijit.byId("gridPesquisaAluno");
            gridAluno.itensSelecionados = [];
            gridAluno.setStore(new dojo.data.ObjectStore({ objectStore: myStore }));

        },
        function (error) {
            apresentaMensagem(dojo.byId("apresentadorMensagemTurma").value, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function popularSalasPorHorarioTurma(xhr, ObjectStore, Memory, ref, Cache, JsonRest) {
    try {
        var calendar = dijit.byId("gridHorarios");
        var horariosOcupadoTurma = [];
        var horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);

        if (horarios.length > 0) {
            $.each(horarios, function (idx, val) {
                if (val.calendar == "Calendar2")
                    horariosOcupadoTurma.push(val);
            });
        }
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/getSalasDisponiveisPorHorarios",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson({
                horariosTurma: horariosOcupadoTurma,
                cd_turma: hasValue(dojo.byId('cd_turma').value) ? parseInt(dojo.byId('cd_turma').value) : 0,
                dt_inicio_aula: dijit.byId('dtIniAula').get('value'),
                dt_final_aula: dijit.byId('dtFimAula').get('value'),
                cd_duracao: dijit.byId('pesCadDuracao').value,
                cd_curso: hasValue(dojo.byId('pesCadCurso').value) ? dijit.byId('pesCadCurso').value : 0
            })
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagemTurma', data);
                var cbSala = dijit.byId("cbSala");
                loadSelect(jQuery.parseJSON(eval(data)).retorno, "cbSala", 'cd_sala', 'no_sala');
                dijit.byId('cbSala').loadAndOpenDropDown();
                showCarregando();
            } catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemTurma', error);
        });

    }
    catch (e) {
        postGerarLog(e);
    }
}

function popularSalasPorHorarioTurmaOnLine(xhr, ObjectStore, Memory, ref, Cache, JsonRest) {
    try {
        var calendar = dijit.byId("gridHorarios");
        var horariosOcupadoTurma = [];
        var horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);

        if (horarios.length > 0) {
            $.each(horarios, function (idx, val) {
                if (val.calendar == "Calendar2")
                    horariosOcupadoTurma.push(val);
            });
        }
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/getSalasDisponiveisPorHorariosByModalidadeOnline",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson({
                horariosTurma: horariosOcupadoTurma,
                cd_turma: hasValue(dojo.byId('cd_turma').value) ? parseInt(dojo.byId('cd_turma').value) : 0,
                dt_inicio_aula: dijit.byId('dtIniAula').get('value'),
                dt_final_aula: dijit.byId('dtFimAula').get('value'),
                cd_duracao: dijit.byId('pesCadDuracao').value,
                cd_curso: hasValue(dojo.byId('pesCadCurso').value) ? dijit.byId('pesCadCurso').value : 0
            })
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagemTurma', data);
                var cbSala = dijit.byId("cbSalaOnLine");
                loadSelect(jQuery.parseJSON(eval(data)).retorno, "cbSalaOnLine", 'cd_sala', 'no_sala');
                dijit.byId('cbSalaOnLine').loadAndOpenDropDown();
                showCarregando();
            } catch (e) {
                showCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemTurma', error);
        });
    }
        catch (e) {
        postGerarLog(e);
    }
}

function buscaPopulaHorariosOcupadosSala(xhr, ObjectStore, Memory, ref, e, online) {
    try {
        var gridHorarios = dijit.byId('gridHorarios');
        if (hasValue(e) && hasValue(gridHorarios) && hasValue(gridHorarios.store))
            xhr.post({
                url: Endereco() + "/api/secretaria/getHorariosOcupadosSala",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_turma: hasValue(dojo.byId('cd_turma').value) ? parseInt(dojo.byId('cd_turma').value) : 0,
                    dt_inicio_aula: dijit.byId('dtIniAula').get('value'),
                    dt_final_aula: dijit.byId('dtFimAula').get('value'),
                    cd_duracao: dijit.byId('pesCadDuracao').value,
                    cd_curso: hasValue(dojo.byId('pesCadCurso').value) ? dijit.byId('pesCadCurso').value : 0,
                    cd_sala: online ? null : e,
                    cd_sala_online: online ? e : null
            })
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    apresentaMensagem("apresentadorMensagemTurma", null);
                    if (hasValue(gridHorarios)) {
                        var storeHorarios = gridHorarios.items.length;
                        for (var i = gridHorarios.items.length - 1; i >= 0 ; i--) {
                            if (gridHorarios.items[i].cssClass == "Calendar3")
                                //gridHorarios.store.remove(gridHorarios.items[i].id);
                                removeHorario(gridHorarios, gridHorarios.items[i].id);
                        }
                    }
                    var codMaiorHorarioTurma = buscarMaiorCodigoHorario();
                    if (hasValue(data.retorno) && data.retorno.length > 0)
                        $.each(data.retorno, function (idx, val) {
                            codMaiorHorarioTurma += 1;
                            val.calendar = "Calendar3";
                            //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
                            val.id = codMaiorHorarioTurma;
                            val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            gridHorarios.store.add(val);
                        });
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem("apresentadorMensagemTurma", error);
            });
        else {
            if (hasValue(gridHorarios)) {
                var storeHorarios = gridHorarios.items.length;
                for (var i = gridHorarios.items.length - 1; i >= 0 ; i--) {
                    if (gridHorarios.items[i].cssClass == "Calendar3")
                        //gridHorarios.store.remove(gridHorarios.items[i].id);
                        removeHorario(gridHorarios, gridHorarios.items[i].id);
                }
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparAlunoCurso() {
    try {
        dojo.byId("cdAlunoPPT").value = 0;
        dojo.byId("cdCursoPPT").value = 0;
        dojo.byId("descCursoPPT").value = "";
        dojo.byId("descAlunoPPT").value = "";
        clearForm("formDescAlunoPPT");
        clearForm("formDescCursoPPT");
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Função para colocar id's nos componetes de tab para poder esconder-las.
function inserirIdTabsCadastro() {
    try {
        if (hasValue(dojo.byId("tabContainerTurma_tablist_tabHorarios")))
            dojo.byId("tabContainerTurma_tablist_tabHorarios").parentElement.id = "paiTabHorarios";
        dojo.byId("tabContainerTurma_tablist_tabProgramacao").parentElement.id = "paiTabProgramacao";
        if (hasValue(dojo.byId("tabContainerTurma_tablist_tabAvaliacoes")))
            dojo.byId("tabContainerTurma_tablist_tabAvaliacoes").parentElement.id = "paiTabAvaliacoes";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataTipoProg(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
            { name: "Todas", id: 0 },
            { name: "Geradas", id: 1 },
            { name: "Não geradas", id: 2 }
            ]
        });
        dijit.byId("sProgramacao").store = statusStoreTipo;
        dijit.byId("sProgramacao").set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function FindIsLoadComponetesPesquisaTurma(xhr, ready, Memory, filteringSelect) {
    loadTipoTurma(Memory);
    loadSituacaoTurma(Memory);
    loadOnLine(Memory);
    xhr.get({
        url: Endereco() + "/api/turma/componentesPesquisaTurma",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagemTurma", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.cursos))
                    criarOuCarregarCompFiltering("pesCurso", data.retorno.cursos, "", null, ready, Memory, filteringSelect, 'cd_curso', 'no_curso', MASCULINO);
                if (hasValue(data.retorno.produtos))
                    criarOuCarregarCompFiltering("pesProduto", data.retorno.produtos, "", null, ready, Memory, filteringSelect, 'cd_produto', 'no_produto', MASCULINO);
                if (hasValue(data.retorno.duracoes))
                    criarOuCarregarCompFiltering("pesDuracao", data.retorno.duracoes, "", null, ready, Memory, filteringSelect, 'cd_duracao', 'dc_duracao', MASCULINO);
                if (data.retorno.usuarioSisProf == true) {
                    criarOuCarregarCompFiltering("pesProfessor", data.retorno.professores, "", data.retorno.professores[0].cd_pessoa, ready, Memory, filteringSelect, 'cd_pessoa', 'no_pessoa');
                    if (!eval(Master()))
                        dijit.byId("pesProfessor").set("disabled", true);
                } else if (hasValue(data.retorno.professores))
                    criarOuCarregarCompFiltering("pesProfessor", data.retorno.professores, "", null, ready, Memory, filteringSelect, 'cd_pessoa', 'no_pessoa', MASCULINO);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function FindIsLoadComponetesNovaTurma(xhr, ready, Memory, filteringSelect) {
    xhr.get({
        url: Endereco() + "/api/turma/componentesNovaTurma?cdDuracao=&cdProduto=&cdRegime=&cdCurso=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagemTurma", null);
            if (data.retorno != null && data.retorno != null) {
                if (hasValue(data.retorno.produtos)) {
                    dijit.byId("pesCadProduto")._onChangeActive = false;
                    criarOuCarregarCompFiltering("pesCadProduto", data.retorno.produtos, "", null, ready, Memory, filteringSelect, 'cd_produto', 'no_produto');
                    dijit.byId("pesCadProduto")._onChangeActive = true;
                }
                if (hasValue(data.retorno.duracoes)) {
                    dijit.byId("pesCadDuracao")._onChangeActive = false;
                    dijit.byId("pesCadDuracaoPPT")._onChangeActive = false;
                    criarOuCarregarCompFiltering("pesCadDuracao", data.retorno.duracoes, "", null, ready, Memory, filteringSelect, 'cd_duracao', 'dc_duracao');
                    criarOuCarregarCompFiltering("pesCadDuracaoPPT", data.retorno.duracoes, "", null, ready, Memory, filteringSelect, 'cd_duracao', 'dc_duracao');
                    dijit.byId("pesCadDuracao")._onChangeActive = true;
                    dijit.byId("pesCadDuracaoPPT")._onChangeActive = true;
                }
                if (hasValue(data.retorno.regimes))
                    criarOuCarregarCompFiltering("pesCadRegime", data.retorno.regimes, "", null, ready, Memory, filteringSelect, 'cd_regime', 'no_regime');
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemTurma", error);
    });
}

function montarHorariosOcupadosProfessorTurma(turma) {
    if (turma.horariosTurma != null && turma.ProfessorTurma != null)
        $.each(turma.horariosTurma, function (index, value) {
            if (hasValue(value.HorariosProfessores) && value.HorariosProfessores.length > 0)
                $.each(value.HorariosProfessores, function (idx, val) {
                    if (hasValue(turma.ProfessorTurma) && turma.ProfessorTurma.length > 0)
                        $.each(turma.ProfessorTurma, function (idxp, valp) {
                            if (hasValue(valp.HorariosProfessores) && valp.HorariosProfessores.length > 0)
                                $.each(valp.HorariosProfessores, function (idxhp, valhp) {
                                    if (valhp.cd_horario == val.cd_horario && valhp.cd_professor == val.cd_professor)
                                        valp.horarios.push(value);
                                });
                        });
                });
        });
    return turma;
}

function showEditTurma(cdTurma, xhr, ready, Memory, filteringSelect, tipoTurma, DropDownMenu, MenuItem, DropDownButton) {
    var endereco = "/api/turma/getTurmaAndComponentesByTurmaEdit?cdTurma=" + cdTurma;
    if (virada)
        endereco = "/api/turma/getTurmaAndComponentesByTurmaEditVirada?cdTurma=" + cdTurma;
    xhr.get({
        url: Endereco() + endereco,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            apresentaMensagem("apresentadorMensagemTurma", null);
            if (data.retorno != null && data.retorno.turma != null) {
                data.retorno.turma = montarHorariosOcupadosProfessorTurma(data.retorno.turma);
                dijit.byId('gridTurma').itemSelecionado = data.retorno.turma;
                var pesCadCurso = dijit.byId("pesCadCurso");
                pesCadCurso.set("disabled", false);
                mudarLegendaPorTipoTurma(tipoTurma);
                if (tipoTurma == TURMA_PPT_FILHA) {
                    if (hasValue(data.retorno.cursos))
                        loadSelect(data.retorno.cursos, "pesCadCurso", 'cd_curso', 'no_curso', data.retorno.turma.cd_curso)
                    //criarOuCarregarCompFiltering("pesCadCurso", data.retorno.cursos, "", data.retorno.turma.cd_curso, ready, Memory, filteringSelect, 'cd_curso', 'no_curso');
                    if (hasValue(data.retorno.produtos))
                        loadSelect(data.retorno.produtos, "pesCadProduto", 'cd_produto', 'no_produto', data.retorno.turma.cd_produto);
                } else {
                    if (hasValue(data.retorno.cursos)) {
                        pesCadCurso._onChangeActive = false;
                        criarOuCarregarCompFiltering("pesCadCurso", data.retorno.cursos, "", data.retorno.turma.cd_curso, ready, Memory, filteringSelect, 'cd_curso', 'no_curso');
                        pesCadCurso._onChangeActive = true;
                    }
                    if (hasValue(data.retorno.produtos))
                        loadSelect(data.retorno.produtos, "pesCadProduto", 'cd_produto', 'no_produto', data.retorno.turma.cd_produto);
                }

                //criarOuCarregarCompFiltering("pesCadProduto", data.retorno.produtos, "", data.retorno.turma.cd_produto, ready, Memory, filteringSelect, 'cd_produto', 'no_produto');
                if (hasValue(data.retorno.duracoes)) {
                    dijit.byId("pesCadDuracao")._onChangeActive = false;
                    dijit.byId("pesCadDuracaoPPT")._onChangeActive = false;
                    criarOuCarregarCompFiltering("pesCadDuracao", data.retorno.duracoes, "", data.retorno.turma.cd_duracao, ready, Memory, filteringSelect, 'cd_duracao', 'dc_duracao');
                    criarOuCarregarCompFiltering("pesCadDuracaoPPT", data.retorno.duracoes, "", null, ready, Memory, filteringSelect, 'cd_duracao', 'dc_duracao');
                    dijit.byId("pesCadDuracao")._onChangeActive = true;
                    dijit.byId("pesCadDuracaoPPT")._onChangeActive = true;
                }
                loadDataTurma(data.retorno.turma, ready, Memory, filteringSelect, tipoTurma);
                if (data.retorno.turma.cd_sala_online == null || data.retorno.turma.cd_pessoa_escola != dojo.byId('_ES0').value)
                    dijit.byId("tabContainerTurma_tablist_escolaContentPane").domNode.style.visibility = 'hidden';
                else
                    dijit.byId("tabContainerTurma_tablist_escolaContentPane").domNode.style.visibility = '';

                if (hasValue(data.retorno.regimes)) {
                    if (data.retorno.turma.cd_regime == PERSONALIZADO && data.retorno.turma.id_turma_ppt)
                        tipoTurma = TURMA_PPT;
                    criarOuCarregarCompFiltering("pesCadRegime", data.retorno.regimes, "", data.retorno.turma.cd_regime, ready, Memory, filteringSelect, 'cd_regime', 'no_regime');
                }

                desabilitarCamposTuma(tipoTurma, data.retorno.turma);

                confLayoutBotInclAluno();
            } else {
                if (virada) {
                    criaDadosNovaTurma(cdTurma, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton);
                } else {
                    apresentaMensagem("apresentadorMensagemTurma", data);
                }
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagemTurma", error);
    });
}

function showLoadTurmaPPT(cdTurma, tipoTurma) {
    require(["dojo/ready", "dojo/store/Memory", "dojo/_base/xhr", "dijit/form/FilteringSelect", "dijit/DropDownMenu", "dijit/MenuItem", "dijit/form/DropDownButton"],
  function (ready, Memory, xhr, FilteringSelect, DropDownMenu, MenuItem, DropDownButton) {
      xhr.get({
          url: Endereco() + "/api/turma/getTurmaEdit?cdTurma=" + cdTurma,
          preventCache: true,
          handleAs: "json",
          headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
      }).then(function (data) {
          try {
              data = jQuery.parseJSON(data);
              apresentaMensagem("apresentadorMensagemTurma", null);
              if (data.retorno != null && data.retorno != null) {
                  if (data.retorno.cd_pessoa_escola != dojo.byId('_ES0').value && data.retorno.id_turma_ppt) {
                      dijit.byId("pesProTurmaFK").set("disabled", false);
                      caixaDialogo(DIALOGO_AVISO, 'Esta turma pertence a outra escola. Favor definir a turma filha através da grade Alunos PPT desta Turma PPT', null);
                      return false;
                  };
                  var compRegime = dijit.byId("pesCadRegime");
                  var comppesProduto = dijit.byId("pesCadProduto");
                  hasValue(data.retorno.cd_duracao) ? dijit.byId("pesCadDuracao").set("value", data.retorno.cd_duracao) : null;
                  comppesProduto._onChangeActive = false;
                  hasValue(data.retorno.cd_duracao) ? comppesProduto.set("value", data.retorno.cd_produto) : null;
                  comppesProduto._onChangeActive = false;
                  comppesProduto.set("disabled", true);
                  if (hasValue(data.retorno.cd_regime)) {
                      compRegime._onChangeActive = false;
                      compRegime.set("value", data.retorno.cd_regime);
                      compRegime._onChangeActive = true;
                  }

                  if (data.retorno.horariosTurma != null && data.retorno.horariosTurma.length > 0)
                      loadHorariosPPTReplicacaoFilha(data.retorno.horariosTurma);
                  loadDataTurmaPPT(data.retorno, ready, Memory, FilteringSelect);
                  confLayoutBotInclAluno();
                  confLayoutTurma(TURMA_PPT_FILHA, NOVO, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton);
              }
          }
          catch (e) {
              postGerarLog(e);
          }
      },
      function (error) {
          apresentaMensagem("apresentadorMensagemTurma", error);
      });
  });
}

function loadHorariosPPTReplicacaoFilha(listaHorariosPPT) {
    try {
        limparTodosHorarios();
        var listaCompletaHorarios = cloneHorarioPPTParaFilha(listaHorariosPPT, null, CLONAREDUPLICARHORARIO);
        destroyCreateGridHorario();
        dijit.byId("gridTurma").horariosClonePPT = listaCompletaHorarios;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataTurma(turma, ready, Memory, filteringSelect, tipoTurma) {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
         function (Memory, ObjectStore) {
             try {
                 var gridAlunos = dijit.byId("gridAlunos");
                 var gridProfessores = dijit.byId("gridProfessor");
                 var gridAlunosPPT = dijit.byId("gridAlunosPPT");
                 var cbSala = dijit.byId("cbSala");
                 var cbSalaOnLine = dijit.byId("cbSalaOnLine");
                 var turmaPPTPai = dijit.byId("idTurmaPPTPai");
                 dojo.byId("escolaturma").value = turma.cd_pessoa_escola;
                 dojo.byId("cd_turma").value = turma.cd_turma;
                 dojo.byId("cd_turma_enc").value = turma.cd_turma_enc;
                 dojo.byId("cd_turma_ppt").value = turma.cd_turma_ppt;
                 dojo.byId("noTurma").value = turma.no_turma;
                 dojo.byId("noApelido").value = turma.no_apelido;
                 //cd_sala : dojo.byId("nrCadSala").value,
                 if (hasValue(turma.dt_termino_turma)) {
                     dijit.byId("dtaTermino").set("value", turma.dt_termino_turma);
                     dijit.byId("alterarTurma").set("disabled", true);
                 } else {
                     dijit.byId("alterarTurma").set("disabled", false);
                 }
                 if (hasValue(turma.dt_final_aula))
                     dijit.byId("dtFimAula").set("value", turma.dt_final_aula);
                 if (turma.cd_sala != null && turma.cd_sala > 0) {
                     cbSala._onChangeActive = false;
                     criarOuCarregarCompFiltering("cbSala", [{ id: turma.cd_sala, name: turma.no_sala }], "", turma.cd_sala, ready, Memory, filteringSelect, 'id', 'name');
                     cbSala._onChangeActive = true;
                     dojo.byId("salaPPTPai").value = cbSala.displayedValue;
                 }
                 if (turma.cd_sala_online != null) {
                     cbSalaOnLine._onChangeActive = false;
                     criarOuCarregarCompFiltering("cbSalaOnLine", [{ id: turma.cd_sala_online, name: turma.no_sala_online }], "", turma.cd_sala_online, ready, Memory, filteringSelect, 'id', 'name');
                     cbSalaOnLine._onChangeActive = true;
                     dojo.byId("salaPPTPai").value = cbSalaOnLine.displayedValue;
                 }
                 if (tipoTurma == TURMA_PPT_FILHA) {
                     dojo.byId("nomTurmaPPF").value = turma.no_turma_ppt;
                     turmaPPTPai._onChangeActive = false;
                     turmaPPTPai.set("checked", turma.id_turma_ppt);
                     turmaPPTPai._onChangeActive = true;
                     //desabilitarCamposIncluirHorariosTurma(true);
                 } else
                     turmaPPTPai.set("checked", turma.id_turma_ppt);
                 dijit.byId("dtIniAula")._onChangeActive = false;
                 dojo.byId("dtIniAula").value = turma.dta_inicio_aula;
                 dijit.byId("dtIniAula")._onChangeActive = true;
                 dijit.byId('cbSala').set('disabled', false);
                 dijit.byId('cbSalaOnLine').set('disabled', false);
                 dijit.byId("idTurmaAtiva").set("checked", turma.id_turma_ativa);
                 dijit.byId("idAulaExterna").set("checked", turma.id_aula_externa);
                 dojo.byId("nrAulasProg").value = turma.nro_aulas_programadas;
                 dojo.byId('nm_vagas').value = turma.nm_vagas;
                 if (hasValue(dijit.byId("gridTurma")) && hasValue(turma != null)) {
                     if (hasValue(turma.horariosTurma))
                         $.each(turma.horariosTurma, function (idx, val) {
                             val.calendar = "Calendar2";
                         });
                     dijit.byId("gridTurma").TurmaEdit = turma;
                     //dijit.byId("btnNovoAluno").set("disabled", false);
                 }
                 if (hasValue(turma.situacoesAluno) && turma.situacoesAluno) {
                     var menu = "";
                     var comecoID = "";
                     var qtd_alunos = 0
                     if (!turma.id_turma_ppt)
                         menu = dijit.byId('menuFiltroAluno');
                     else {
                         comecoID = "filtroSit";
                         menu = dijit.byId('menuFiltroAlunoPPT');
                     }
                     $.each(turma.situacoesAluno, function (idx, val) {
                     if (!hasValue(dijit.byId(comecoID + (val.cd_situacao_aluno_turma + 10))))
                             menu.addChild(new dijit.MenuItem({
                                 id: comecoID + (val.cd_situacao_aluno_turma + 10),
                                 label: val.situacaoAlunoTurma,
                                 onClick: function (e) { comecoID == "filtroSit" ? montarFiltroSituacaoALunoTurmaPPT(e) : montarFiltroSituacaoALunoTurma(e) }
                             }));
                     });
                     dojo.byId('nm_alunos').value = qtd_alunos;
                 }
                 var qtd_alunos = 0
                 if (turma.alunosTurmaEscola != null && turma.alunosTurmaEscola.length > 0)
                     for (var i = 0; i < turma.alunosTurmaEscola.length; i++)
                         if (turma.alunosTurmaEscola[i].cd_situacao_aluno_turma == ATIVO || turma.alunosTurmaEscola[i].cd_situacao_aluno_turma == REMATRICULADO) {
                             qtd_alunos += 1;
                         }
                 dojo.byId('nm_alunos').value = qtd_alunos;
                 if (turma.id_turma_ppt) {
                     if (hasValue(gridAlunosPPT) && (hasValue(turma.alunosTurmasPPTSearch != null) || hasValue(turma.alunosTurmaEscola != null)) && (turma.alunosTurmasPPTSearch.length > 0 || turma.alunosTurmaEscola.length > 0)) {
                
                         $.each(turma.alunosTurmasPPTSearch, function (index, val) {
                             val.id = geradorIdAlunoPPT(gridAlunosPPT);
                             val = montarHorariosOcupadosProfessorTurma(val);
                             insertObjSort(gridAlunosPPT.store.objectStore.data, "id", val);
                         });
                         
                         gridAlunosPPT.setStore(new ObjectStore({ objectStore: new Memory({ data: gridAlunosPPT.store.objectStore.data }) }));
                         var alunosTurmasPPTSearch = gridAlunosPPT.store.objectStore.data;
                         gridAlunosPPT.listaCompletaAlunos = cloneArray(alunosTurmasPPTSearch);
                         gridAlunosPPT.listaCompletaAlunosEsc = cloneArray(turma.alunosTurmaEscola);

                         dijit.byId("idTurmaAtiva").set("disabled", false);
                         if (turma.id_turma_ativa == true) {
                             // REgras desabilitadas, vai fazer tudo no controlller
                             //for (var i = 0; i < alunosTurmasPPTSearch.length; i++)
                             //    if (alunosTurmasPPTSearch[i].dta_termino_turma == null) { //&& (alunosTurmasPPTSearch[i].cd_situacao_aluno_turma == 1 || alunosTurmasPPTSearch[i].cd_situacao_aluno_turma == 8)
                             //        dijit.byId("idTurmaAtiva").set("disabled", true);
                             //        break;
                             //    }
                             //if (hasValue(turma.alunosTurmaEscola != null) && turma.alunosTurmaEscola.length > 0)
                             //    for (var i = 0; i < turma.alunosTurmaEscola.length; i++)
                             //        if (turma.alunosTurmaEscola[i].dta_termino_turma == null) { //&& (turma.alunosTurmaEscola[i].cd_situacao_aluno_turma == 1 || turma.alunosTurmaEscola[i].cd_situacao_aluno_turma == 8)
                             //        dijit.byId("idTurmaAtiva").set("disabled", true);
                             //        break;
                             //    }
                             dijit.byId("btnNovoAlunoPPT").set("disabled", false);
                         }
                         else
                             dijit.byId("btnNovoAlunoPPT").set("disabled", true);

                     }
                 } else if (hasValue(gridAlunos) && hasValue(turma.alunosTurma != null) && turma.alunosTurma.length > 0) {
                     gridAlunos.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: turma.alunosTurma, idProperty: "cd_aluno" }) }));
                     //gridAlunos.on("StyleRow", function (row) {
                     //    try {
                     //        var item = gridAlunos.getItem(row.index);
                     //        if(hasValue(item)){
                     //            if (item.cd_situacao_aluno_turma == DESISTENTE)
                     //                row.customClasses += " RedRow";
                     //            if (item.cd_situacao_aluno_turma == MOVIDO)
                     //                row.customClasses += " BlueRow";
                     //        }
                     //    }
                     //    catch (e) {
                     //        postGerarLog(e);
                     //    }
                     //});

                     gridAlunos.listaCompletaAlunos = cloneArray(turma.alunosTurma);
                     gridAlunos.listaCompletaAlunosEsc = cloneArray(turma.alunosTurmaEscola);

                 }
                 if (hasValue(gridProfessores) && hasValue(turma.ProfessorTurma != null) && turma.ProfessorTurma.length > 0)
                     gridProfessores.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: turma.ProfessorTurma }) }));
             }
             catch (e) {
                 postGerarLog(e);
             }
         });
}

function loadDataTurmaPPT(turma, ready, Memory, filteringSelect) {
    try {
        turma = montarHorariosOcupadosProfessorTurma(turma);
        var gridProfessores = dijit.byId("gridProfessor");
        var cbSala = dijit.byId("cbSala");
        var cbSalaOnLine = dijit.byId("cbSalaOnLine");
        dojo.byId("escolaturma").value = turma.cd_pessoa_escola;
        dojo.byId("cd_turma_ppt").value = turma.cd_turma;
        //dojo.byId("cd_turma_ppt").value = turma.cd_turma_ppt;
        dojo.byId("nomTurmaPPF").value = turma.no_turma;
        if (hasValue(turma.dta_termino_turma)) {
            dojo.byId("dtaTermino").value = turma.dta_termino_turma;
            dijit.byId("alterarTurma").set("disabled", true);
        } else {
            dijit.byId("alterarTurma").set("disabled", false);
        }
        if (turma.cd_sala != null && turma.cd_sala > 0) {
            cbSala._onChangeActive = false;
            criarOuCarregarCompFiltering("cbSala", [{ id: turma.cd_sala, name: turma.no_sala }], "", turma.cd_sala, ready, Memory, filteringSelect, 'id', 'name');
            cbSala._onChangeActive = true;
            dojo.byId("salaPPTPai").value = cbSala.displayedValue;
        }
        if (turma.cd_sala_online != null && turma.cd_sala_online > 0) {
            cbSalaOnLine._onChangeActive = false;
            criarOuCarregarCompFiltering("cbSalaOnLine", [{ id: turma.cd_sala_online, name: turma.no_sala_online }], "", turma.cd_sala_online, ready, Memory, filteringSelect, 'id', 'name');
            cbSalaOnLine._onChangeActive = true;
            dojo.byId("salaPPTPai").value = cbSalaOnLine.displayedValue;
        }
        dojo.byId("dtIniAula").value = turma.dta_inicio_aula;
        dojo.byId("dtFimAula").value = turma.dta_final_aula;
        dijit.byId("idTurmaAtiva").set("checked", turma.id_turma_ativa);
        dijit.byId("idAulaExterna").set("checked", turma.id_aula_externa);
        dojo.byId("nrAulasProg").value = turma.nro_aulas_programadas;
        if (hasValue(dijit.byId("gridTurma")) && hasValue(turma != null) && turma.cd_turma > 0) {
            dijit.byId("gridTurma").TurmaEdit = turma;
        }
        if (hasValue(gridProfessores) && hasValue(turma.ProfessorTurma != null) && turma.ProfessorTurma.length > 0)
            gridProfessores.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: turma.ProfessorTurma }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadDataNewTurmaCurso(data, ready, Memory, filteringSelect) {
    ready(function () {
        try {
            storeData = [];
            if (hasValue(data) && data.length > 0) {

                $.each(data, function (index, value) {
                    storeData.push({
                        id: value.cd_curso,
                        name: value.no_curso,
                        cd_produto: value.Produto.cd_produto,
                        no_produto: value.Produto.no_produto
                    });
                });
            }

            var statusStore = new Memory({
                data: storeData
            });
            dijit.byId("pesCadCurso")._onChangeActive = false;
            dijit.byId("pesCadCurso").store = statusStore;
            dijit.byId("pesCadCurso")._onChangeActive = true;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadOnLine(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
            { name: "Todas", id: 0 },
            { name: "Presenciais", id: 1 },
            { name: "On-Line", id: 2 }
            ]
        });
        dijit.byId("ckOnLine").store = statusStoreTipo;
        dijit.byId("ckOnLine").set("value", 0);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadTipoTurma(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
            { name: "Todas", id: 0 },
            { name: "Regular", id: 1 },
            { name: "Personalizada", id: 3 }
            ]
        });
        dijit.byId("tipoTurma").store = statusStoreTipo;
        dijit.byId("tipoTurma").set("value", 0);
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

function loadNewProdutos(xhr, ready, Memory, FilteringSelect) {
    xhr.get({
        preventCache: true,
        url: Endereco() + "/api/coordenacao/getAllProduto",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            (hasValue(jQuery.parseJSON(data).retorno)) ? data = jQuery.parseJSON(data).retorno : jQuery.parseJSON(data);
            if (hasValue(data)) {
                dijit.byId("pesCadProduto")._onChangeActive = false;
                criarOuCarregarCompFiltering("pesCadProduto", data, "", null, ready, Memory, FilteringSelect, 'cd_produto', 'no_produto');
                dijit.byId("pesCadProduto")._onChangeActive = true;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemTurma', error);
    });
}

function carregarCursoPorProdutoTurma(cd_produto, xhr, ready, Memory, filteringSelect, cd_curso) {
    xhr.get({
        preventCache: true,
        url: Endereco() + "/api/curso/getCursosPorProduto?cd_curso=&cd_produto=" + cd_produto,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            data = jQuery.parseJSON(data);
            if (hasValue(data.retorno) && data.retorno.length > 0) {
                dijit.byId("pesCadCurso")._onChangeActive = false;
                criarOuCarregarCompFiltering("pesCadCurso", data.retorno, "", cd_curso, ready, Memory, filteringSelect, 'cd_curso', 'no_curso');
                dijit.byId("pesCadCurso")._onChangeActive = true;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemTurma', error);
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridTurma';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBoxPlano'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_turma', 'turmaSelecionada', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'turmaSelecionada', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function carregarImagemHelp(endereco, id) {
    dojo.ready(function () {
        if (hasValue(endereco)) {
            $(id).attr({
                src: endereco
            });
            $(id)[0].src = endereco;
        }
    });
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        var cd_turmaPtt = dojo.byId("cd_turmaPPT").value;
        var cd_turma = dojo.byId("cd_turma").value;
        var cd_sala = hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : 0;
        var cd_sala_online = hasValue(dijit.byId("cbSalaOnLine").value) ? dijit.byId("cbSalaOnLine").value : 0;
        var cd_prof = returnProfessoresAtivosTurma();
        var gridTurma = dijit.byId("gridTurma");
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainerTurma_tablist_tabPrincipalTurma') {
            apresentaMensagem("apresentadorMensagemTurma", null);
            confLayoutBotInclAluno();
            atualizarGradesTabPrincipal();
        }

        if (tab.getAttribute('widgetId') == 'tabContainerTurma_tablist_tabHorarios' && !hasValue(dijit.byId("gridHorarios"))) {
            apresentaMensagem("apresentadorMensagemTurma", null);
            if (dijit.byId("pesCadRegime").validate()) {
                if (dojo.byId("tipoTurmaCad").value != TURMA_PPT_FILHA) {
                    if (hasValue(cd_turma) && cd_turma > 0) {
                        if (cd_sala_online == 0)
                            loadHorarioTurma(cd_turma, cd_sala, cd_prof, TURMA)
                        else
                            loadHorarioTurma(cd_turma, cd_sala_online, cd_prof, TURMA);
                    }
                    else {
                        criacaoGradeHorarioTurma(null);
                    }
                }
                else
                    if (dojo.byId("tipoTurmaCad").value == TURMA_PPT_FILHA)
                        if (hasValue(cd_turma) && cd_turma > 0)
                            loadHorarioTurmaPPTFilha(dojo.byId("cd_turma_ppt").value, dojo.byId("cd_turma").value, "gridHorarios", "295px", "apresentadorMensagemTurma");
                        else {
                            var listaHorariosPPT = null;
                            if (hasValue(cd_turmaPtt) && cd_turmaPtt <= 0)
                                if (hasValue(gridTurma.horariosClonePPT) && gridTurma.horariosClonePPT.length > 0)
                                    listaHorariosPPT = gridTurma.horariosClonePPT;
                            criacaoGradeHorarioTurmaPPTFilha(listaHorariosPPT, 'gridHorarios', "295px", "apresentadorMensagemTurma", "timeIni", "timeFim", "excluirHorarioTurma", CADPRINCFILHAPPT, VERIFTURMANORMAL);
                        }
            } else {
                mostrarMensagemCampoValidado(dojo.window, dijit.byId("pesCadRegime"));
                setarTabCad();
            }
        }

        if (tab.getAttribute('widgetId') == 'tabContainerTurma_tablist_tabProgramacao' && !hasValue(dijit.byId("gridProgramacao"))) {
            apresentaMensagem("apresentadorMensagemTurma", null);
            criaAtualizaTagProgramacao(false);
        }

        //Ação click turmaPPTFilha
        if (tab.getAttribute('widgetId') == 'tabContainerPPT_tablist_tabHorariosPPT' && !hasValue(dijit.byId("gridHorariosPPT"))) {
            apresentaMensagem("apresentadorMensagemTurma", null);
            loadHorarioTurmaPPTPorBD(dijit.byId("gridHorarios"));
        }

        if (tab.getAttribute('widgetId') == 'tabContainerPPT_tablist_tabProgramacaoPPT') {
            apresentaMensagem('apresentadorMensagemTurmaPPT', null);
            if (!hasValue(dijit.byId("gridProgramacaoPPT"))) {
                var cd_turm_PPTFilha = dijit.byId("gridAlunosPPT").itensSelecionados[0].cd_turma;
                var progTurmaPPTFilhaEdit = loadProgramacaoPPTEditadosGrade(dijit.byId("gridAlunosPPT"));
                if (cd_turm_PPTFilha > 0) {
                    if (progTurmaPPTFilhaEdit != null && progTurmaPPTFilhaEdit.alterouProgramacaoOuDescFeriado)
                        criacaoComponentesProgramacaoPPTFilha(progTurmaPPTFilhaEdit.ProgramacaoTurma, progTurmaPPTFilhaEdit.FeriadosDesconsiderados);
                    else
                        loadProgramacao(cd_turm_PPTFilha, TURMA_PPT, false);
                } else
                    criacaoComponentesProgramacaoPPTFilha(progTurmaPPTFilhaEdit.ProgramacaoTurma, progTurmaPPTFilhaEdit.FeriadosDesconsiderados);
            } else {
                dijit.byId("gridProgramacaoPPT").update();
                dijit.byId("gridDesconsideraFeriadosPPT").update();
            }
        }

        if (tab.getAttribute('widgetId') == 'tabContainerTurma_tablist_tabAvaliacoes' && !hasValue(dijit.byId("gridAvaliacaoAluno"))) {
            apresentaMensagem("apresentadorMensagemTurma", null);
            dojo.byId('tipoGrid').value = 'gridAvaliacaoAluno';
            var mensagem = 'apresentadorMensagemTurma';
            if (!hasValue(cd_turma))
                cd_turma = 0;
            loadAvaliacaoAluno(cd_turma, mensagem);
        }

        if (tab.getAttribute('widgetId') == 'tabContainerPPT_tablist_tabAvaliacoesPPT' && !hasValue(dijit.byId("gridAvalAlunoPPT"))) {
            dojo.byId('tipoGrid').value = 'gridAvalAlunoPPT';
            var mensagem = 'apresentadorMensagemTurma';
            if (!hasValue(cd_turmaPtt))
                cd_turmaPtt = 0;
            loadAvaliacaoAluno(cd_turmaPtt, mensagem);
        }

        pesquisarEscolasVinculadasUsuario();

        if (tab.getAttribute('widgetId') == "tabContainerTurma_tablist_escolaContentPane") {
            popularGridEscolaTurma();
            dojo.byId('abriuEscola').value = true;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criaAtualizaTagProgramacao() {
    try {
        var cd_turma = dojo.byId("cd_turma").value;
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var gridDesc = dijit.byId('gridDesconsideraFeriados');
        var gridProgramacao = dijit.byId('gridProgramacao');


        if (tipoTurma == TURMA_PPT) {
            gridDesc = dijit.byId('gridDesconsideraFeriadosPPT');
            gridProgramacao = dijit.byId('gridProgramacaoPPT');
            cd_turma = dojo.byId("cd_turmaPPT").value;
        }

        if (hasValue(gridProgramacao) || (hasValue(cd_turma) && cd_turma > 0))
            loadProgramacao(cd_turma, tipoTurma);
        else {
            var storeProg = null;
            var storeDescon = null;

            if (hasValue(gridProgramacao) && hasValue(gridDesc)) {
                storeProg = gridProgramacao.store.objectStore.data;
                storeDescon = gridDesc.store.objectStore.data;
            }
            criacaoComponentesProgramacao(storeProg, storeDescon);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criacaoComponentesProgramacao(dataProgramacao, dataDesconsideraFeriados) {
    require(["dojo/ready", "dijit/DropDownMenu", "dijit/form/Button", "dijit/MenuItem", "dijit/form/DropDownButton", "dojox/grid/EnhancedGrid", "dojo/data/ObjectStore",
        "dojo/store/Memory", "dojo/_base/xhr", "dojox/json/ref", "dijit/MenuSeparator", "dijit/form/CheckBox"],
        function (ready, DropDownMenu, Button, MenuItem, DropDownButton, EnhancedGrid, ObjectStore, Memory, xhr, ref, MenuSeparator, CheckBox) {
          ready(function () {
              try {
                  var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataProgramacao }) });

                  destroyCreateProgramacao();

                  var gridProgramacao = new EnhancedGrid({
                      store: dataStore,
                      structure:
                          [
                            { name: "<input id='selecionaTodosProgramacao' style='display:none'/>", field: "selecionadoProgramacao", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProgramacao },
                            { name: "Aula", field: "nm_programacao_real", width: "8%" },
                            { name: "Data", field: "dt_programacao_turma", width: "14%" },
                            { name: "Horário", field: "hr_aula", width: "24%" },
                            { name: "Descrição", field: "dc_programacao_turma", width: "45%" },
                            {
                                name: "Calendário", field: "_item", width: "15%", styles: "text-align: center; min-width:15px;", formatter: function (value, rowIndex, obj) {
                                    //var id = obj.field + '_Selected_' + value.nm_aula_programacao_turma;
                                    var id = "id_mostrar_calendario_Selected_" + value.nm_aula_programacao_turma;
                                    if (hasValue(dijit.byId(id)))
                                        dijit.byId(id).destroy();
                                    return new CheckBox({
                                        name: id,
                                        id: id,
                                        value: value,
                                        checked: value.id_mostrar_calendario,
                                        onChange: function (b, z) {
                                            var id = this.id;
                                            var checked = this.checked;
                                            var filteredArr = itensSelecionadosCheckGrid.filter(function (item) {
                                                return item.id == id;
                                            });
                                            if (filteredArr.length > 0) {
                                                var position = itensSelecionadosCheckGrid.indexOf(filteredArr[0]);
                                                itensSelecionadosCheckGrid[position].checked = checked;

                                            } else {

                                                itensSelecionadosCheckGrid.push({ id: this.id, rowIndex: rowIndex, checked: this.checked });
                                            }


                                            progTurma = dijit.byId("gridProgramacao").store.objectStore.data;

                                            var nm_aula = this.value.nm_aula_programacao_turma;
                                            var itensFilter = progTurma.filter(function (item) {
                                                return item.nm_aula_programacao_turma == nm_aula;
                                            });
                                            if (itensFilter.length > 0) {
                                                var position = progTurma.indexOf(itensFilter[0]);
                                                progTurma[position].id_mostrar_calendario = b;
                                                progTurma[position].programacaoTurmaEdit = true;
                                                console.log(b);
                                                console.log(progTurma[position]);
                                            }

                                            
                                        }
                                    }, id);
                                }
                            },
                            { name: "Aux", field: "nm_aula_programacao_turma", width: "8%" },
                            { name: "", field: "id_aula_dada", styles: "display:none;" },
                            { name: "", field: "cd_feriado", styles: "display:none;" },
                            { name: "", field: "cd_feriado_desconsiderado", styles: "display:none;" },
                            { name: "", field: "id_feriado_desconsiderado", styles: "display:none;" },
                            { name: "", field: "id_programacao_manual", styles: "display:none;" },
                            { name: "", field: "id_prog_cancelada", styles: "display:none;" }
                          ],
                      noDataMessage: msgNotRegEnc,
                      plugins: {
                          pagination: {
                              pageSizes: ["7", "14", "30", "100", "All"],
                              description: true,
                              sizeSwitch: true,
                              pageStepper: true,
                              defaultPageSize: "7",
                              gotoButton: true,
                              /*page step to be displayed*/
                              maxPageStep: 4,
                              /*position of the pagination bar*/
                              position: "button"
                          }
                      }
                  }, "gridProgramacao");
                  gridProgramacao.on("StyleRow", function (row) {
                      try {
                          var item = gridProgramacao.getItem(row.index);
                          if (row.node.children[0].children[0].children[0].children[7].innerHTML != '...' && eval(row.node.children[0].children[0].children[0].children[7].innerHTML))
                              row.customClasses += " YellowRow";
                          if (row.node.children[0].children[0].children[0].children[8].innerHTML != '...' && hasValue(row.node.children[0].children[0].children[0].children[8].innerHTML) //se tem codigo do feriado.
                                  && (row.node.children[0].children[0].children[0].children[9].innerHTML == '...' || !hasValue(row.node.children[0].children[0].children[0].children[9].innerHTML)) //não tem código do desconsidera feriado do banco.
                                  && row.node.children[0].children[0].children[0].children[10].innerHTML != "true")//não tem codigo do desconsidera feriado de memória.
                              row.customClasses += " GreenRow";
                          if (row.node.children[0].children[0].children[0].children[11].innerHTML != '...' && eval(row.node.children[0].children[0].children[0].children[11].innerHTML) == PROGRAMACAO_MANUAL)
                              row.customClasses += " OrangeRow";
                          if (item != null && item.id_prog_cancelada)
                              row.customClasses += " RedRow"
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  });
                  gridProgramacao.rowsPerPage = 5000;
                  gridProgramacao.pagination.plugin._paginator.plugin.connect(gridProgramacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                      verificaMostrarTodos(evt, gridProgramacao, 'nm_aula_programacao_turma', 'selecionaTodosProgramacao');
                  });
                  require(["dojo/aspect", "dojo/_base/array", "dijit/registry"], function (aspect, Array, registry) {
                     /* aspect.before(gridProgramacao, "_onFetchComplete", function () {
                          //console.log(gridProgramacao.store.objectStore.data);
                          if (hasValue(dijit.byId("id_mostrar_calendario_Selected_1"))) 
                            console.log(dijit.byId("id_mostrar_calendario_Selected_1").get("checked"));
                      });*/

                      aspect.after(gridProgramacao, "_onFetchComplete", function () {
                          // Configura o check de todos:
                          if (hasValue(dojo.byId('selecionaTodosProgramacao')) && dojo.byId('selecionaTodosProgramacao').type == 'text')
                              setTimeout("configuraCheckBox(false, 'nm_aula_programacao_turma', 'selecionadoProgramacao', -1, 'selecionaTodosProgramacao', 'selecionaTodosProgramacao', 'gridProgramacao')", gridProgramacao.rowsPerPage * 3);

                          setTimeout(function () {
                                  Array.forEach(itensSelecionadosCheckGrid, function (value, i) {
                                            if (hasValue(dijit.byId(value.id))) {registry.byId(value.id).set('checked', value.checked);}
                                  });
                              },
                              gridProgramacao.rowsPerPage );
                      });
                  });
                          

                  gridProgramacao.canSort = false;
                  gridProgramacao.startup();
                  gridProgramacao.on("RowDblClick", function (evt) {
                      try {
                          var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                          apresentaMensagem('apresentadorMensagemTurma', '');
                          apresentaMensagem('apresentadorMensagemTurmaPPT', '');
                          document.getElementById("divAlterarProg").style.display = "";
                          document.getElementById("divCancelarProg").style.display = "";
                          document.getElementById("divIncluirProg").style.display = "none";
                          keepValuesProgramacao(item, gridProgramacao, false);
                          dijit.byId("dialogProgramacao").show();
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  }, true);

                  var dataStoreDesconsidera = null;

                  destroyCreateFeriadoDesconsiderado();

                  if (dataDesconsideraFeriados != null && dataDesconsideraFeriados.length > 0) {
                      //Cria uma key para diferenciar os inclusos em memória:
                      for (var i = 0; i < dataDesconsideraFeriados.length; i++)
                          dataDesconsideraFeriados[i].nm_feriado_desconsiderado = i + 1;

                      dataStoreDesconsidera = new ObjectStore({ objectStore: new Memory({ data: dataDesconsideraFeriados }) });
                  }
                  else
                      dataStoreDesconsidera = new ObjectStore({ objectStore: new Memory({ data: null }) });

                  var gridDesconsideraFeriados = new EnhancedGrid({
                      store: dataStoreDesconsidera,
                      structure:
                          [
                            { name: "<input id='selecionaTodosFeriadoDesconsiderado' style='display:none'/>", field: "selecionadoFeriadoDesconsiderado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFeriadoDesconsiderado },
                            { name: "", field: "nm_feriado_desconsiderado", styles: "display:none;" },
                            { name: "", field: "cd_feriado_desconsiderado", styles: "display:none;" },
                            { name: "Data Inicial", field: "dta_inicial", width: "40%" },
                            { name: "Data Final", field: "dta_final", width: "40%" },
                            { name: "Programação", field: "dc_programacao_feriado", width: "20%" },
                            { name: "", field: "id_aula_dada", styles: "display:none;" }
                          ],
                      noDataMessage: msgNotRegEnc,
                      plugins: {
                          pagination: {
                              pageSizes: ["6", "12", "30", "100", "All"],
                              description: true,
                              sizeSwitch: true,
                              pageStepper: true,
                              defaultPageSize: "10",
                              gotoButton: true,
                              /*page step to be displayed*/
                              maxPageStep: 4,
                              /*position of the pagination bar*/
                              position: "button"
                          }
                      }
                  }, "gridDesconsideraFeriados");
                  gridDesconsideraFeriados.rowsPerPage = 5000;
                  gridDesconsideraFeriados.pagination.plugin._paginator.plugin.connect(gridDesconsideraFeriados.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                      verificaMostrarTodos(evt, gridDesconsideraFeriados, 'nm_feriado_desconsiderado', 'selecionaTodosFeriadoDesconsiderado');
                  });
                  require(["dojo/aspect"], function (aspect) {
                      aspect.after(gridDesconsideraFeriados, "_onFetchComplete", function () {
                          // Configura o check de todos:
                          if (hasValue(dojo.byId('selecionaTodosProgramacao')) && dojo.byId('selecionaTodosProgramacao').type == 'text')
                              setTimeout("configuraCheckBox(false, 'nm_feriado_desconsiderado', 'selecionadoFeriadoDesconsiderado', -1, 'selecionaTodosFeriadoDesconsiderado', 'selecionaTodosFeriadoDesconsiderado', 'gridDesconsideraFeriados')", gridDesconsideraFeriados.rowsPerPage * 3);
                      });
                  });

                  gridDesconsideraFeriados.canSort = false;
                  gridDesconsideraFeriados.startup();
                  gridDesconsideraFeriados.on("RowDblClick", function (evt) {
                      try {
                          var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                          apresentaMensagem('apresentadorMsgDesconsideracao', '');
                          document.getElementById("divAlterarDesc").style.display = "";
                          document.getElementById("divCancelarDesc").style.display = "";
                          document.getElementById("divIncluirDesc").style.display = "none";
                          document.getElementById("divLimparDesc").style.display = "none";
                          gridDesconsideraFeriados.itemSelecionado = item;
                          keepValuesFeriadoDesconsiderado(gridDesconsideraFeriados, false);
                          dijit.byId("dialogDesconsideracao").show();
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  }, true);

                  dijit.byId("tagDesconsiderarFeriados").on("Show", function (e) {
                      dijit.byId('paiDesconsideraFeriados').resize();
                      dijit.byId('gridDesconsideraFeriados').update();
                  });

                  if (!hasValue(dijit.byId("acoesRelacionadasProgramacao"))) {
                      var menuProgramacao = new DropDownMenu({ style: "height: 25px" });
                      var acaoExcluirProgramacao = new MenuItem({
                          label: "Excluir",
                          onClick: function () { deletarItemSelecionadoGridProgramacao(Memory, ObjectStore, 'nm_aula_programacao_turma', dijit.byId("gridProgramacao"), 'apresentadorMensagemTurma'); }
                      });
                      menuProgramacao.addChild(acaoExcluirProgramacao);

                      var acaoEditarProgramacao = new MenuItem({
                          label: "Editar",
                          onClick: function () { eventoEditaProgramacao(dijit.byId("gridProgramacao").itensSelecionados); }
                      });
                      menuProgramacao.addChild(acaoEditarProgramacao);

                      //Desabilitado Temporáriamente
                      //var acaoCancelarProgramacao = new MenuItem({
                      //    label: "Cancelar",
                      //    onClick: function () {
                      //        eventoCancelaProgramacao(dijit.byId("gridProgramacao"), dijit.byId("gridProgramacao").itensSelecionados);
                      //    }
                      //});
                      //menuProgramacao.addChild(acaoCancelarProgramacao);

                      var acaoCancelarProgramacao = new MenuItem({
	                      label: "Desfazer Cancelar",
	                      onClick: function () {
		                      eventoDesfazerCancelarProgramacao(dijit.byId("gridProgramacao"), dijit.byId("gridProgramacao").itensSelecionados);
	                      }
                      });
                      menuProgramacao.addChild(acaoCancelarProgramacao);

                      menuProgramacao.addChild(new MenuSeparator());
                      var acaoTelaProgramacao = new MenuItem({
                          label: "Gerar Modelo de Programação",
                          onClick: function () { gerarModeloProgramacao(xhr, ref, 'apresentadorMensagemTurma'); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      acaoTelaProgramacao = new MenuItem({
                          label: "Programação Automática pelo Curso",
                          onClick: function () { incluirProgramacaoCurso(xhr, 'gridProgramacao', dijit.byId('gridDesconsideraFeriados'), ref, 'apresentadorMensagemTurma', false); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      acaoTelaProgramacao = new MenuItem({
                          label: "Programação Automática pelo Modelo",
                          onClick: function () { incluirProgramacaoCurso(xhr, 'gridProgramacao', dijit.byId('gridDesconsideraFeriados'), ref, 'apresentadorMensagemTurma', true); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      acaoTelaProgramacao = new MenuItem({
                          label: "Refazer numeração das aulas",
                          onClick: function () { refazerNumeracaoProgramacao(dijit.byId("gridProgramacao"), dijit.byId("gridProgramacao").itensSelecionados); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      acaoTelaProgramacao = new MenuItem({
                          label: "Refazer numeração auxiliar das aulas",
                          onClick: function () { refazerNumeracaoAuxiliar(dijit.byId("gridProgramacao"), dijit.byId("gridProgramacao").itensSelecionados); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      var buttonProgramacao = new DropDownButton({
                          label: "Ações Relacionadas",
                          name: "acoesRelacionadasProgramacao",
                          dropDown: menuProgramacao,
                          id: "acoesRelacionadasProgramacao"
                      });
                      dojo.byId("linkAcoesProgramacao").appendChild(buttonProgramacao.domNode);

                      var menuDesconsidera = new DropDownMenu({ style: "height: 25px" });
                      var acaoExcluirDesconsidera = new MenuItem({
                          label: "Excluir",

                          onClick: function () { excluirDesconsideraFeriados(Memory, ObjectStore, xhr, 'gridProgramacao', 'gridDesconsideraFeriados', 'apresentadorMensagemTurma'); }
                      });
                      menuDesconsidera.addChild(acaoExcluirDesconsidera);

                      var acaoEditarDesconsidera = new MenuItem({
                          label: "Editar",
                          onClick: function () {
                              eventoEditaDesconsidera(dijit.byId("gridDesconsideraFeriados").itensSelecionados);
                          }
                      });
                      menuDesconsidera.addChild(acaoEditarDesconsidera);

                      var buttonDesconsidera = new DropDownButton({
                          label: "Ações Relacionadas",
                          name: "acoesRelacionadasDesconsidera",
                          dropDown: menuDesconsidera,
                          id: "acoesRelacionadasDesconsidera"
                      });
                      dojo.byId("linkAcoesDesconsidera").appendChild(buttonDesconsidera.domNode);

                      new Button({
                          label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                              try {
                                  document.getElementById("divAlterarProg").style.display = "none";
                                  document.getElementById("divCancelarProg").style.display = "none";
                                  document.getElementById("divIncluirProg").style.display = "";
                                  dijit.byId('aulaProgramacao').set('disabled', false);
                                  incluirProgramacao(xhr, dijit.byId('gridProgramacao'), dijit.byId('gridDesconsideraFeriados'), ref, 'apresentadorMensagemTurma');
                              }
                              catch (e) {
                                  postGerarLog(e);
                              }
                          }
                      }, "btnNovaProgramacao");
                      new Button({
                          label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                              incluirFeriadoDesconsiderado();
                          }
                      }, "btnNovaDesconsideracao");
                      if (!hasValue(dijit.byId('alterarDescon')))
                          new Button({
                              label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                  alterarDesconsideraFeriados(xhr);
                              }
                          }, "alterarDescon");
                      if (!hasValue(dijit.byId('incluirDescon')))
                          new Button({
                              label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                  incluirDesconsiderarFeriado(xhr);
                              }
                          }, "incluirDescon");
                      if (!hasValue(dijit.byId('limparDescon')))
                          new Button({
                              label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                  clearForm("formFeriadoDesconsiderado");
                              }
                          }, "limparDescon");
                      if (!hasValue(dijit.byId('fecharDescon')))
                          new Button({
                              label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                  dijit.byId('dialogDesconsideracao').hide();
                              }
                          }, "fecharDescon");
                  }
                  if (!hasValue(dijit.byId('cancelarDescon'))) {
                      var cancelarDescon = new Button({
                          label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent'
                      }, "cancelarDescon");
                      if (hasValue(cancelarDescon.handler))
                          cancelarDescon.handler.remove();
                      cancelarDescon.handler = cancelarDescon.on("click", function (e) {
                          keepValuesFeriadoDesconsiderado(dijit.byId('gridDesconsideraFeriados'), false);
                      });
                  }
                  else {
                      var cancelarDescon = dijit.byId("cancelarDescon");
                      if (hasValue(cancelarDescon.handler))
                          cancelarDescon.handler.remove();
                      cancelarDescon.handler = cancelarDescon.on("click", function (e) {
                          keepValuesFeriadoDesconsiderado(dijit.byId('gridDesconsideraFeriados'), false);
                      });
                  }
              }
              catch (e) {
                  postGerarLog(e);
              }
          });
      });
}

function incluirFeriadoDesconsiderado() {
    try {
        if (OUTRAESCOLA) {
            caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
            return false;
        } 
        clearForm("formFeriadoDesconsiderado");
        document.getElementById("divAlterarDesc").style.display = "none";
        document.getElementById("divCancelarDesc").style.display = "none";
        document.getElementById("divIncluirDesc").style.display = "";
        document.getElementById("divLimparDesc").style.display = "";

        dijit.byId('dialogDesconsideracao').show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluirDesconsideraFeriados(Memory, ObjectStore, xhr, idProgramacao, gridDesconsideraFeriados, apresentadorMensagemTurma) {
    try {
        if (OUTRAESCOLA) {
            caixaDialogo(DIALOGO_AVISO, msgErroMatriculaAlunoOutroaEscola, null);
            return false;
        } 
        var grid = dijit.byId(gridDesconsideraFeriados);
        if (!hasValue(grid.itensSelecionados) || grid.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else {
            // verifica se existe programação no feriado desconsiderado:
            var programacoesTurma = dijit.byId(idProgramacao).store.objectStore.data;
            var existe_diario_aula = false;
            var existe_intersecao = false;

            for (var j = 0; !existe_intersecao && j < grid.itensSelecionados.length; j++) {
                var item = grid.itensSelecionados[j];

                if (hasValue(programacoesTurma))
                    for (var i = programacoesTurma.length - 1; i >= 0; i--) {
                        var dataProg = dojo.date.locale.parse(programacoesTurma[i].dt_programacao_turma + ' ' + programacoesTurma[i].hr_final_programacao
                            , { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                        var dt_inicial = dojo.date.locale.parse(item.dta_inicial, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                        var dt_final = dojo.date.locale.parse(item.dta_final, { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                        //Verifica a intersecção das datas:
                        if ((dojo.date.compare(dt_inicial, dataProg, "date") >= 0 && dojo.date.compare(dt_inicial, dataProg, "date") <= 0)
                                || (dojo.date.compare(dt_final, dataProg, "date") >= 0 && dojo.date.compare(dt_final, dataProg, "date") <= 0)
                                || (dojo.date.compare(dt_inicial, dataProg, "date") <= 0 && dojo.date.compare(dt_final, dataProg, "date") >= 0)) {
                            item.id_programacao_feriado = 1;
                            item.dc_programacao_feriado = 'Sim';

                            programacoesTurma[i].id_feriado_desconsiderado = true;
                            existe_intersecao = true;
                        }

                        if (programacoesTurma[i].id_aula_dada) {
                            if (dojo.date.compare(dataProg, dt_inicial, "date") >= 0) {
                                existe_diario_aula = true;

                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDesconsideraFeriadoNoDiarioAula);
                                apresentaMensagem(apresentadorMensagemTurma, mensagensWeb, true);
                            }
                            break;
                        }
                    }
            }
            if (!existe_diario_aula) {
                deletarItemSelecionadoGrid(Memory, ObjectStore, 'nm_feriado_desconsiderado', grid);
                if (existe_intersecao)
                    verificaProgramacaoTurma(null, xhr, null, TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO, null);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarDesconsideraFeriados(xhr) {
    require(["dojo/date"], function (date) {
        try {
            if (!dijit.byId("formFeriadoDesconsiderado").validate())
                return false;

            var tipoProgTurma = dojo.byId("tipoTurmaCad").value;
            var tipoTurma = dojo.byId("tipoTurmaCad").value;

            apresentaMensagem('apresentadorMensagemTurma', '', false);

            var grid = dijit.byId('gridDesconsideraFeriados');
            var programacoes = dijit.byId('gridProgramacao');

            if (tipoTurma == TURMA_PPT) {
                apresentaMensagem('apresentadorMensagemTurmaPPT', '', false);
                grid = dijit.byId('gridDesconsideraFeriadosPPT');
                programacoes = dijit.byId('gridProgramacaoPPT');
            }

            var item = grid.itemSelecionado;

            var dataInicio = dojo.date.locale.parse(dojo.byId("dt_inicio_des").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            var dataFim = dojo.date.locale.parse(dojo.byId("dt_fim_des").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            var dta_inicio = dojo.date.locale.format(dataInicio, { datePattern: "dd/MM/yyyy", selector: "date" });
            var dta_final = dojo.date.locale.format(dataFim, { datePattern: "dd/MM/yyyy", selector: "date" });

            //Verifica se a data inicial é menor ou igual a data final:
            if (date.compare(dataInicio, dataFim) > 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
                apresentaMensagem('apresentadorMsgDesconsideracao', mensagensWeb);
                return false;
            }

            // verifica se existia programacao no feriado desconsiderado anterior:
            var programacoesTurma = programacoes.store.objectStore.data;
            var existe_diario_aula = false;
            var existe_intersecao = false;
            if (hasValue(programacoesTurma))
                for (var i = programacoesTurma.length - 1; i >= 0; i--) {
                    var dataProg = dojo.date.locale.parse(programacoesTurma[i].dt_programacao_turma + ' ' + programacoesTurma[i].hr_final_programacao
                        , { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                    var dt_inicial = dojo.date.locale.parse(item.dta_inicial, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    var dt_final = dojo.date.locale.parse(item.dta_final, { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                    //Verifica a intersecção das datas:
                    if ((dojo.date.compare(dt_inicial, dataProg, "date") >= 0 && dojo.date.compare(dt_inicial, dataProg, "date") <= 0)
                            || (dojo.date.compare(dt_final, dataProg, "date") >= 0 && dojo.date.compare(dt_final, dataProg, "date") <= 0)
                            || (dojo.date.compare(dt_inicial, dataProg, "date") <= 0 && dojo.date.compare(dt_final, dataProg, "date") >= 0))
                        existe_intersecao = true;

                    if (programacoesTurma[i].id_aula_dada)
                        break;
                }

            item.dta_final = dta_final;
            item.dta_inicial = dta_inicio;

            item.dt_final = dataFim;
            item.dt_inicial = dataInicio;

            item.id_programacao_feriado = 0;
            item.dc_programacao_feriado = 'Não';

            // verifica se existe programação no feriado desconsiderado:
            if (hasValue(programacoesTurma))
                for (var i = programacoesTurma.length - 1; i >= 0; i--) {
                    var dataProg = dojo.date.locale.parse(programacoesTurma[i].dt_programacao_turma + ' ' + programacoesTurma[i].hr_final_programacao
                        , { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                    var dt_inicial = dojo.date.locale.parse(item.dta_inicial, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    var dt_final = dojo.date.locale.parse(item.dta_final, { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                    //Verifica a intersecção das datas:
                    if ((dojo.date.compare(dt_inicial, dataProg, "date") >= 0 && dojo.date.compare(dt_inicial, dataProg, "date") <= 0)
                            || (dojo.date.compare(dt_final, dataProg, "date") >= 0 && dojo.date.compare(dt_final, dataProg, "date") <= 0)
                            || (dojo.date.compare(dt_inicial, dataProg, "date") <= 0 && dojo.date.compare(dt_final, dataProg, "date") >= 0)) {
                        item.id_programacao_feriado = 1;
                        item.dc_programacao_feriado = 'Sim';

                        programacoesTurma[i].id_feriado_desconsiderado = true;
                        existe_intersecao = true;
                    }

                    if (programacoesTurma[i].id_aula_dada) {
                        if (dojo.date.compare(dataProg, dt_inicial, "date") >= 0) {
                            existe_diario_aula = true;

                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDesconsideraFeriadoNoDiarioAula);
                            apresentaMensagem('apresentadorMsgDesconsideracao', mensagensWeb, true);

                        }
                        break;
                    }
                }
            if (!existe_diario_aula) {
                if (existe_intersecao)
                    verificaProgramacaoTurma(null, xhr, null, TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO, null);
                dijit.byId("dialogDesconsideracao").hide();

            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function incluirDesconsiderarFeriado(xhr) {
    require(["dojo/date"], function (date) {
        try {
            apresentaMensagem('apresentadorMensagemTurma', '', false);
            if (!dijit.byId("formFeriadoDesconsiderado").validate())
                return false;

            var tipoProgTurma = dojo.byId("tipoTurmaCad").value;
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            var dataInicio = dojo.date.locale.parse(dojo.byId("dt_inicio_des").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            var dataFim = dojo.date.locale.parse(dojo.byId("dt_fim_des").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            var dta_inicio = dojo.date.locale.format(dataInicio, { datePattern: "dd/MM/yyyy", selector: "date" });
            var dta_final = dojo.date.locale.format(dataFim, { datePattern: "dd/MM/yyyy", selector: "date" });
            var cd_turma = document.getElementById('cd_turma').value;
            var gridDesc = dijit.byId('gridDesconsideraFeriados');
            var gridProgramacao = dijit.byId('gridProgramacao');

            if (tipoTurma == TURMA_PPT) {
                gridDesc = dijit.byId('gridDesconsideraFeriadosPPT');
                gridProgramacao = dijit.byId('gridProgramacaoPPT');
            }

            var programacoesTurma = gridProgramacao.store.objectStore.data;

            //Verifica se a data inicial é menor ou igual a data final:
            if (date.compare(dataInicio, dataFim) > 0) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
                apresentaMensagem('apresentadorMsgDesconsideracao', mensagensWeb);
                return false;
            }

            // Verifica se já existe algum diário de aula efetivado durante ou após o feriado a ser desonsiderado:
            //TODO: caso existe, volta mensagem de erro.

            // Verifica se já existe programações com feriados no período que está incluindo
            //TODO: recalcula as programações sem feriado.

            //Calcula o próximo número da grid:
            var nm_feriado_desconsiderado = 1;

            if (hasValue(gridDesc.store.objectStore.data) && gridDesc.store.objectStore.data.length > 0)
                nm_feriado_desconsiderado = gridDesc.store.objectStore.data[gridDesc.store.objectStore.data.length - 1].nm_feriado_desconsiderado + 1;

            var feriado_desconsiderado = {
                nm_feriado_desconsiderado: nm_feriado_desconsiderado,
                dt_inicial: dataInicio,
                dt_final: dataFim,
                dta_inicial: dta_inicio,
                dta_final: dta_final,
                cd_turma: hasValue(cd_turma) ? parseInt(cd_turma) : 0,
                dc_programacao_feriado: 'Não',
                id_programacao_feriado: 0
            };

            // verifica se existe programação no feriado desconsiderado:
            var existe_diario_aula = false;
            var existe_intersecao = false;
            if (hasValue(programacoesTurma))
                for (var i = programacoesTurma.length - 1; i >= 0; i--) {
                    var dataProg = dojo.date.locale.parse(programacoesTurma[i].dt_programacao_turma + ' ' + programacoesTurma[i].hr_final_programacao
                        , { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                    var dt_inicial = dojo.date.locale.parse(feriado_desconsiderado.dta_inicial, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    var dt_final = dojo.date.locale.parse(feriado_desconsiderado.dta_final, { formatLength: 'short', selector: 'date', locale: 'pt-br' });

                    //Verifica a intersecção das datas:
                    if ((dojo.date.compare(feriado_desconsiderado.dt_inicial, dataProg, "date") >= 0 && dojo.date.compare(feriado_desconsiderado.dt_inicial, dataProg, "date") <= 0)
                            || (dojo.date.compare(dt_final, dataProg, "date") >= 0 && dojo.date.compare(dt_final, dataProg, "date") <= 0)
                            || (dojo.date.compare(dt_inicial, dataProg, "date") <= 0 && dojo.date.compare(dt_final, dataProg, "date") >= 0)) {
                        feriado_desconsiderado.id_programacao_feriado = 1;
                        feriado_desconsiderado.dc_programacao_feriado = 'Sim';
                        programacoesTurma[i].id_feriado_desconsiderado = true;

                        existe_intersecao = true;
                    }

                    if (programacoesTurma[i].id_aula_dada) {
                        if (dojo.date.compare(dataProg, dt_inicial, "date") >= 0) {
                            existe_diario_aula = true;

                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDesconsideraFeriadoNoDiarioAula);
                            apresentaMensagem('apresentadorMsgDesconsideracao', mensagensWeb, true);
                        }

                        break;
                    }
                }
            if (!existe_diario_aula) {
                gridDesc.store.newItem(feriado_desconsiderado);
                gridDesc.store.save();
                if (existe_intersecao)
                    verificaProgramacaoTurma(null, xhr, null, TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO, null);
                dijit.byId("dialogDesconsideracao").hide();
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadProgramacao(cd_turma, tipoTurma) {
    require(["dojo/_base/xhr", "dojox/json/ref", "dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect"], function (xhr, ref, ready, Memory, FilteringSelect) {
        try {
            var apresentadorMsg = 'apresentadorMensagemTurma';
            if (tipoTurma == TURMA_PPT)
                apresentadorMsg = 'apresentadorMensagemTurmaPPT';

            var refazer_programacao = dojo.byId('refazer_programacao');

            if (refazer_programacao.value == TIPO_REFAZER_PROGRAMACOES_CURSO || refazer_programacao.value == TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL
                    || refazer_programacao.value == TIPO_REFAZER_PROGRAMACOES_HORARIO || refazer_programacao.value == TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO) {
                var horariosOcupadoTurma = new Array();
                var horarios = new Array();
                if (tipoTurma == TURMA_PPT)
                    horarios = validaRetornaHorariosFIlhaPPT();
                else
                    horarios = validaRetornaHorarios(tipoTurma);
                if (horarios.length > 0)
                    $.each(horarios, function (index, value) {
                        if (value.calendar == "Calendar2" || !hasValue(value.calendar))
                            horariosOcupadoTurma.push(value);
                    });
                var dtIniAula = hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                var cd_curso = dijit.byId("pesCadCurso").value;
                var cd_duracao = dijit.byId("pesCadDuracao").value;

                var programacoes = null;
                var gridProgramacao = dijit.byId('gridProgramacao');

                if (tipoTurma == TURMA_PPT) {
                    gridProgramacao = dijit.byId('gridProgramacaoPPT');
                    cd_curso = dijit.byId("pesCadCursoPPT").value;
                    cd_duracao = dijit.byId("pesCadDuracaoPPT").value;

                    dijit.byId("dtIniAulaPPT")._onChangeActive = false;
                    dtIniAula = hasValue(dojo.byId("dtIniAulaPPT").value) ? dojo.date.locale.parse(dojo.byId("dtIniAulaPPT").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                    dijit.byId("dtIniAulaPPT")._onChangeActive = true;
                }

                if (hasValue(gridProgramacao)) {
                    programacoes = new Array();
                    var dataProgramacoes = gridProgramacao.store.objectStore.data;
                    for (var i = 0; i < dataProgramacoes.length; i++)
                        programacoes.push({
                            cd_programacao_turma: dataProgramacoes[i].cd_programacao_turma,
                            id_aula_dada: dataProgramacoes[i].id_aula_dada,
                            cd_feriado: dataProgramacoes[i].cd_feriado,
                            cd_feriado_desconsiderado: dataProgramacoes[i].cd_feriado_desconsiderado,
                            id_feriado_desconsiderado: dataProgramacoes[i].id_feriado_desconsiderado,
                            dc_programacao_turma: dataProgramacoes[i].dc_programacao_turma,
                            dta_programacao_turma: dojo.date.locale.parse(dataProgramacoes[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                            hr_inicial_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[0] : '',
                            hr_final_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[1] : '',
                            nm_aula_programacao_turma: dataProgramacoes[i].nm_aula_programacao_turma,
                            id_programacao_manual: dataProgramacoes[i].id_programacao_manual,
                            id_mostrar_calendario: dataProgramacoes[i].id_mostrar_calendario,
                            nm_programacao_real: dataProgramacoes[i].nm_rogramacao_real
                        });
                }

                var feriados_descosiderados = null;
                var gridFeriadosDesconsiderados = dijit.byId('gridDesconsideraFeriados');

                if (tipoTurma == TURMA_PPT)
                    gridFeriadosDesconsiderados = dijit.byId('gridDesconsideraFeriadosPPT');

                if (hasValue(gridFeriadosDesconsiderados)) {
                    feriados_descosiderados = new Array();
                    var dataFeriadosDesconsiderados = gridFeriadosDesconsiderados.store.objectStore.data;
                    for (var i = 0; i < dataFeriadosDesconsiderados.length; i++)
                        feriados_descosiderados.push({
                            cd_feriado_desconsiderado: dataFeriadosDesconsiderados[i].cd_feriado_desconsiderado,
                            dt_inicial: dataFeriadosDesconsiderados[i].dt_inicial,
                            dt_final: dataFeriadosDesconsiderados[i].dt_final,
                            id_programacao_feriado: dataFeriadosDesconsiderados[i].id_programacao_feriado
                        });
                }
                xhr.post({
                    url: Endereco() + "/api/coordenacao/postProgramacoesTurma",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    preventCache: true,
                    postData: ref.toJson({ programacoes: programacoes, horarios: horariosOcupadoTurma, dt_inicio: dtIniAula, cd_curso: cd_curso, cd_duracao: cd_duracao, cd_turma: parseInt(cd_turma), tipo: refazer_programacao.value, feriados_desconsiderados: dataFeriadosDesconsiderados })
                }).then(function (dataProgTurma) {
                    try {
                        apresentaMensagem(apresentadorMsg, dataProgTurma, true);

                        dataProgTurma = jQuery.parseJSON(dataProgTurma);

                        if (hasValue(dataProgTurma.retorno) && hasValue(dataProgTurma.retorno.Programacoes, true)) {
                            if (tipoTurma != TURMA_PPT)
                                criacaoComponentesProgramacao(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                            else
                                criacaoComponentesProgramacaoPPTFilha(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                        }
                        else if (refazer_programacao.value != TIPO_REFAZER_PROGRAMACOES_HORARIO && refazer_programacao.value != TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO) {
                            if (tipoTurma != TURMA_PPT)
                                criacaoComponentesProgramacao(null, null);
                            else
                                criacaoComponentesProgramacaoPPTFilha(null, null);
                            retornaDadosIniciaisTurma(xhr, ready, Memory, FilteringSelect, true);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem(apresentadorMsg, error, true);
                    if (refazer_programacao.value != TIPO_REFAZER_PROGRAMACOES_HORARIO && refazer_programacao.value != TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO)
                        retornaDadosIniciaisTurma(xhr, ready, Memory, FilteringSelect);
                });
            }
            else {
                xhr.get({
                    preventCache: true,
                    url: Endereco() + "/api/turma/getProgramacoesTurma?cd_turma=" + parseInt(cd_turma),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataProgTurma) {
                    try {
                        dataProgTurma = jQuery.parseJSON(dataProgTurma);
                        if (tipoTurma != TURMA_PPT)
                            criacaoComponentesProgramacao(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                        else
                            criacaoComponentesProgramacaoPPTFilha(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem(apresentadorMsg, error);

                    //Retorna o semáforo para refazer as programações na outra tag
                    dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
                });
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function retornaDadosIniciaisTurma(xhr, ready, Memory, FilteringSelect, remove_programacoes) {
    try {
        //Retorna o semáforo para refazer as programações na outra tag
        dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;

        if (remove_programacoes) {
            var gridProgramacao = dijit.byId("gridProgramacao");

            if (gridProgramacao.store.objectStore.data.length > 0) {
                dijit.byId("gridProgramacao").setStore(new dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));

                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgAvisoProgramacoesRemovidas);
                apresentaMensagem(apresMsg, mensagensWeb, true);
            }
            return false;
        }
        else if (hasValue(item)) {
            // Volta com o curso, produto e duração novamente para o valor original:
            var item = dijit.byId('gridTurma').itemSelecionado;

            dijit.byId("pesCadProduto")._onChangeActive = false;
            dijit.byId('pesCadProduto').set('value', item.cd_produto);
            dijit.byId("pesCadProduto")._onChangeActive = true;

            dijit.byId("pesCadDuracao")._onChangeActive = false;
            dijit.byId('pesCadDuracao').set('value', item.cd_duracao);
            dijit.byId("pesCadDuracao")._onChangeActive = true;

            //dijit.byId("pesCadCurso")._onChangeActive = false;
            //dijit.byId('pesCadCurso').set('value', item.cd_curso);
            //dijit.byId("pesCadCurso")._onChangeActive = true;

            carregarCursoPorProdutoTurma(item.cd_produto, xhr, ready, Memory, FilteringSelect, item.cd_curso);
            dojo.byId('dtIniAula').value = item.dta_inicio_aula;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Método que busca as programações e feriados desconsiderados da memória:
function loadProgramacaoPPTEditadosGrade(gridPrg) {
    try {
        var progTurmaPPTFilhaEdit = [];
        var itenSelecionado = gridPrg.itensSelecionados[0];
        $.each(gridPrg.store.objectStore.data, function (idx, val) {
            if (itenSelecionado.cd_aluno == val.cd_aluno && itenSelecionado.cd_curso == val.cd_curso) {
                progTurmaPPTFilhaEdit = {
                    ProgramacaoTurma: val.ProgramacaoTurma,
                    FeriadosDesconsiderados: hasValue(val.FeriadosDesconsiderados) ? val.FeriadosDesconsiderados : null,
                    alterouProgramacaoOuDescFeriado: hasValue(val.alterouProgramacaoOuDescFeriado) ? val.alterouProgramacaoOuDescFeriado : false
                };
                return false;
            }
        });
        return progTurmaPPTFilhaEdit;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region  loadAvaliacaoAluno, inverteVisaoNota
function inverteVisaoNota(data) {
    try {
        var novoDataPrimeiroNivel = [];
        var novoDataSegundoNivel = [];
        var dadosGridNota = cloneArray(data);

        //Pega os filhos do primeiro pai e transforma eles nos novos pais:
        for (var i = 0; i < dadosGridNota.length; i++) {
            novoDataPrimeiroNivel = dadosGridNota;
            for (var j = 0; j < dadosGridNota[i].children.length; j++) {
                novoDataSegundoNivel = dadosGridNota[i].children[j].children;
            }
        }

        //Remove todos os filhos dos pais do data original para transformá-lo em um vetor de novos filhos:
        var cloneData = cloneArray(dadosGridNota);
        for (var j = 0; j < cloneData.length; j++) {
            for (var n = 0; n < cloneData[j].children.length; n++)
                delete cloneData[j].children[n].children;
            cloneData[j].pai = 0;// deixa de ser pai
            if (cloneData[j].cd_tipo_avaliacao == 0) cloneData[j].isChildren = 1;
        }

        for (var r = 0; r < novoDataSegundoNivel.length; r++) {
            var novosItensSegNivel = cloneArray(cloneData);
            for (var j = 0; j < novosItensSegNivel.length; j++) {
                novosItensSegNivel[j].id = novosItensSegNivel[j].id + '' + r;
            }
            novoDataSegundoNivel[r].children = novosItensSegNivel;
            novoDataSegundoNivel[r].pai = 1;
            if (novoDataSegundoNivel[r].cd_tipo_avaliacao == 0) novoDataSegundoNivel[r].isChildren = 0;
        }

        //Corrigi o id para a visão do aluno.
        var contador = 0;
        for (var d = 0; d < novoDataSegundoNivel.length; d++) {
            novoDataSegundoNivel[d].id = novoDataSegundoNivel[d].id + '' + contador + '' + d;
            for (var e = 0; e < novoDataSegundoNivel[d].children.length; e++) {
                novoDataSegundoNivel[d].children[e].id = novoDataSegundoNivel[d].children[e].id + '' + contador;
                for (var f = 0; f < novoDataSegundoNivel[d].children[e].children.length; f++) {
                    contador++;
                    novoDataSegundoNivel[d].children[e].children[f].id = novoDataSegundoNivel[d].children[e].children[f].id + '' + contador;
                    novoDataSegundoNivel[d].children[e].children[f].pai = 0;
                }
            }
        }
        var notaTurma = 0;
        var vl_nota = ''
        for (var i = 0; i < data.length; i++)
            for (var u = 0; u < data[i].children.length; u++)
                for (var v = 0; v < data[i].children[u].children.length; v++)
                    for (var d = 0; d < novoDataSegundoNivel.length; d++)
                        for (var c = 0; c < novoDataSegundoNivel[d].children.length; c++)
                            for (var e = 0; e < novoDataSegundoNivel[d].children[c].children.length; e++)
                                if (data[i].children[u].cd_avaliacao != 0 //Significa que a visão original tem o nível pai como aluno:
                                        && data[i].children[u].cd_avaliacao == novoDataSegundoNivel[d].children[c].children[e].cd_avaliacao
                                        && data[i].children[u].children[v].cd_aluno == novoDataSegundoNivel[d].cd_aluno) {
                                    novoDataSegundoNivel[d].children[c].children[e].participacoesAluno = null;
                                    novoDataSegundoNivel[d].children[c].children[e].participacoesDisponiveis = null;

                                    novoDataSegundoNivel[d].children[c].children[e].dc_observacao_aux = data[i].children[u].dc_observacao;
                                    novoDataSegundoNivel[d].children[c].children[e].dt_termino_turma = data[i].children[u].dt_termino_turma;
                                    novoDataSegundoNivel[d].children[c].children[e].dc_observacao = data[i].children[u].children[v].dc_observacao;
                                    novoDataSegundoNivel[d].dt_matricula = data[i].children[u].children[v].dt_matricula;
                                    novoDataSegundoNivel[d].dt_movimento = data[i].children[u].children[v].dt_movimento;
                                    novoDataSegundoNivel[d].dt_desistencia = data[i].children[u].children[v].dt_desistencia;
                                    novoDataSegundoNivel[d].dt_transferencia = data[i].children[u].children[v].dt_transferencia;
                                    novoDataSegundoNivel[d].children[c].children[e].dt_avaliacao_turma = data[i].children[u].dt_avaliacao_turma;
                                    novoDataSegundoNivel[d].children[c].children[e].cd_avaliacao_turma = data[i].children[u].children[v].cd_avaliacao_turma;
                                    novoDataSegundoNivel[d].children[c].children[e].cd_avaliacao_aluno = data[i].children[u].children[v].cd_avaliacao_aluno;
                                    novoDataSegundoNivel[d].children[c].dc_nome = data[i].dc_nome;
                                    novoDataSegundoNivel[d].children[c].children[e].somaNotas = data[i].children[u].vl_nota;
                                    novoDataSegundoNivel[d].children[c].children[e].id_segunda_prova = data[i].children[u].children[v].id_segunda_prova;
                                    novoDataSegundoNivel[d].children[c].children[e].cd_conceito = data[i].children[u].children[v].cd_conceito;
                                    if (data[i].children[u].isConceito)
                                        novoDataSegundoNivel[d].children[c].children[e].vl_nota = data[i].children[u].children[v].dc_conceito;
                                    else {
                                        if (hasValue(data[i].children[u].children[v].vl_nota_2)) {
                                            vl_nota = data[i].children[u].children[v].vl_nota_2;
                                            novoDataSegundoNivel[d].children[c].children[e].dc_observacao =
                                            novoDataSegundoNivel[d].children[c].children[e].dc_observacao == null ? '2ª Chance' : novoDataSegundoNivel[d].children[c].children[e].dc_observacao + ' 2ª Chance'
                                        }
                                        else vl_nota = data[i].children[u].children[v].vl_nota
                                        if (hasValue(data[i].children[u].children[v].peso))
                                            novoDataSegundoNivel[d].children[c].children[e].vl_nota = data[i].children[u].children[v].peso * vl_nota;
                                        else
                                            novoDataSegundoNivel[d].children[c].children[e].vl_nota = vl_nota;
                                    }
                                    if (!hasValue(novoDataSegundoNivel[d].children[c].children[e].vl_nota))
                                        novoDataSegundoNivel[d].children[c].children[e].val_nota = "";
                                    else
                                        novoDataSegundoNivel[d].children[c].children[e].val_nota = maskFixed(novoDataSegundoNivel[d].children[c].children[e].vl_nota, 2);
                                }
        //Atualiza a soma das notas para mostrar na visão do aluno:
        for (var i = 0; i < novoDataSegundoNivel.length; i++) {
            novoDataSegundoNivel[i].dta_avaliacao_turma = "";
            novoDataSegundoNivel[i].vl_nota = "";
            novoDataSegundoNivel[i].val_nota = "";
            novoDataSegundoNivel[i].dc_observacao = "";
            novoDataSegundoNivel[i].participacoesAluno = null;
            novoDataSegundoNivel[i].participacoesDisponiveis = null;
        }

        return novoDataSegundoNivel;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridAvaliacaoAluno() {
    try {
        if (hasValue(dijit.byId("gridAvalAlunoPPT"))) {
            dijit.byId("gridAvalAlunoPPT").destroy();
            $('<div>').attr('id', 'gridAvalAlunoPPT').appendTo('#paiGridAvaliacaoAluno');

        }
        if (hasValue(dijit.byId("gridAvaliacaoAluno"))) {
            dijit.byId("gridAvaliacaoAluno").destroy();
            $('<div>').attr('id', 'gridAvaliacaoAluno').appendTo('#paiGridAvaliacaoAluno');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadAvaliacaoAluno(turma, mensagem) {
    try {
        var dataRetorno = []
        apresentaMensagem(mensagem, null);
        if (turma > 0) {
            dojo.xhr.get({
                preventCache: true,
                handleAs: "json",
                url: Endereco() + "/api/turma/getAvaliacaoTurmaArvore?idTurma=" + turma + "&idConceito=0" + "&tipoForm=1" + 
                    "&cd_escola_combo=0", //LBMPassando zero, No Controller temos que analisar as turmas compartilhadas
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataAvaliacaoTurma) {
                try {
                    dataAvaliacaoTurma = jQuery.parseJSON(dataAvaliacaoTurma);
                    apresentaMensagem(mensagem, dataAvaliacaoTurma);
                    if (hasValue(dataAvaliacaoTurma) && hasValue(dataAvaliacaoTurma.retorno) && hasValue(dataAvaliacaoTurma.retorno.tipoAvaliacaoTurma))
                        dataRetorno = dataAvaliacaoTurma.retorno.tipoAvaliacaoTurma;
                    dataRetorno = inverteVisaoNota(dataRetorno);

                    var data = {
                        identifier: 'id',
                        label: 'cd_avaliacao_turma',
                        items: dataRetorno
                    };

                    var store = new dojo.data.ItemFileWriteStore({ data: data });
                    var model = new dijit.tree.ForestStoreModel({ store: store, childrenAttrs: ['children'] });

                    /* set up layout */
                    var layout = [
                      { name: 'Avaliação por Alunos', field: 'dc_nome', width: '30%' },
                      { name: 'Data', field: 'dta_avaliacao_turma', width: '10%', styles: "text-align: center;" },
                      { name: 'Nota Máx.', field: 'maxNotaTurma', width: '10%', styles: "text-align: center;" },
                      { name: 'Nota Aluno', field: 'val_nota', width: '10%', styles: "text-align: center;" },
                      { name: 'Obs.', field: 'dc_observacao', width: '20%', styles: "text-align: center;" }
                    ];

                    var gridAvaliacao = hasValue(dojo.byId('tipoGrid').value != "") ? dojo.byId('tipoGrid').value : 'gridAvaliacaoAluno';
                    destroyCreateGridAvaliacaoAluno();
                    var gridAvaliacaoAluno = new dojox.grid.LazyTreeGrid({
                        id: gridAvaliacao + '',
                        treeModel: model,
                        structure: layout
                    }, document.createElement('div'));

                    dojo.byId(gridAvaliacao).appendChild(gridAvaliacaoAluno.domNode);
                    gridAvaliacaoAluno.startup();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem(mensagem, error);
                loadGridNotaVazia();
            });
        }
        else loadGridNotaVazia();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadGridNotaVazia() {
    try {
        //Avaliacao turma
        var dataRetorno = [];
        var data = {
            identifier: 'id',
            label: 'dc_avaliacao_aluno',
            items: dataRetorno
        };

        var store = new dojo.data.ItemFileWriteStore({ data: data });
        var model = new dijit.tree.ForestStoreModel({ store: store, childrenAttrs: ['children'] });

        /* set up layout */
        var layout = [
                 { name: 'Avaliação por Alunos', field: 'dc_nome', width: '30%' },
                 { name: 'Data', field: 'dta_avaliacao_turma', width: '15%', styles: "text-align: center;" },
                 { name: 'Nota Máx.', field: 'maximoNotaTurma', width: '15%', styles: "text-align: center;" },
                 { name: 'Nota Aluno', field: 'vl_nota', width: '15%', styles: "text-align: center;" },
                 { name: 'Obs.', field: 'dc_observacao', width: '25%', styles: "text-align: center;" }
        ];

        var gridAvaliacao = dojo.byId('tipoGrid').value;
        gridAvaliacao = gridAvaliacao == '' ? 'gridAvaliacaoAluno' : dojo.byId('tipoGrid').value;

        var gridAvaliacaoAluno = new dojox.grid.LazyTreeGrid({
            id: gridAvaliacao,
            treeModel: model,
            structure: layout
        }, document.createElement('div'));

        dojo.byId(gridAvaliacao).appendChild(gridAvaliacaoAluno.domNode);
        gridAvaliacaoAluno.startup();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion
function incluirProgramacoesModelo(xhr, ref, apresMsg) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    try {
        var cd_turmas = new Array();
        var turmasSelecionadas = dijit.byId('gridTurma').itensSelecionados;

        if (!hasValue(turmasSelecionadas) || turmasSelecionadas.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaNaoSelecionada);
            apresentaMensagem(apresMsg, mensagensWeb);
            return false;
        }

        for (var i = 0; i < turmasSelecionadas.length; i++)
            cd_turmas.push(turmasSelecionadas[i].cd_turma);

        xhr.post({
            url: Endereco() + "/api/coordenacao/postIncluirProgramacoesModelo",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(cd_turmas)
        }).then(function (data) {
            data = jQuery.parseJSON(data);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegIncludSucess);
            apresentaMensagem(apresMsg, mensagensWeb);
        },
        function (error) {
            apresentaMensagem(apresMsg, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}


function gerarRelatorioPercentualFaltasGrupoAvancado(xhr, ref, apresMsg, Memory, ObjectStore, grid) {
    try {

        if (!hasValue(grid.itensSelecionados) || grid.itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else if (grid.itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            console.log(grid.itensSelecionados);
            var url = Endereco() + "/relatorio/findTurmaPercentualFaltaGrupoAvancado?cd_turma=" + grid.itensSelecionados[0].cd_turma + "&cd_turma_ppt=" + grid.itensSelecionados[0].cd_turma_ppt + "&id_turma_ppt=" + grid.itensSelecionados[0].id_turma_ppt;
            //window.open(url);

            xhr.get({
                url: url,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                    try {
                        window.open(data);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        }

    }
    catch (e) {
        postGerarLog(e);
    }
}

function gerarModeloProgramacao(xhr, ref, apresMsg) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var apresentadorMsg = 'apresentadorMensagemTurma';
        if (tipoTurma == TURMA_PPT)
            apresentadorMsg = 'apresentadorMensagemTurmaPPT';

        var cd_curso = dijit.byId("pesCadCurso").value;
        var cd_duracao = dijit.byId("pesCadDuracao").value;
        var cd_turma = dojo.byId('cd_turma').value;

        if (tipoTurma == TURMA_PPT)
            cd_turma = dojo.byId('cd_turmaPPT').value

        var programacoes = null;
        var gridProgramacao = dijit.byId('gridProgramacao');

        if (cd_turma <= 0 || cd_turma == null) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgRegistroNaoSalvo);
            apresentaMensagem(apresentadorMsg, mensagensWeb);
            return false;
        }

        apresentaMensagem(apresentadorMsg, '');
        if (tipoTurma == TURMA_PPT) {
            gridProgramacao = dijit.byId('gridProgramacaoPPT');
            cd_curso = dijit.byId("pesCadCursoPPT").value;
            cd_duracao = dijit.byId("pesCadDuracaoPPT").value;
        }

        if (hasValue(gridProgramacao)) {
            programacoes = new Array();
            var dataProgramacoes = gridProgramacao.store.objectStore.data;
            for (var i = 0; i < dataProgramacoes.length; i++)
                programacoes.push({
                    cd_programacao_turma: dataProgramacoes[i].cd_programacao_turma,
                    id_aula_dada: dataProgramacoes[i].id_aula_dada,
                    cd_feriado: dataProgramacoes[i].cd_feriado,
                    cd_feriado_desconsiderado: dataProgramacoes[i].cd_feriado_desconsiderado,
                    id_feriado_desconsiderado: dataProgramacoes[i].id_feriado_desconsiderado,
                    dc_programacao_turma: dataProgramacoes[i].dc_programacao_turma,
                    dta_programacao_turma: dojo.date.locale.parse(dataProgramacoes[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                    hr_inicial_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[0] : '',
                    hr_final_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[1] : '',
                    nm_aula_programacao_turma: dataProgramacoes[i].nm_aula_programacao_turma,
                    id_programacao_manual: dataProgramacoes[i].id_programacao_manual
                });
        }

        var feriados_descosiderados = null;
        var gridFeriadosDesconsiderados = dijit.byId('gridDesconsideraFeriados');

        if (tipoTurma == TURMA_PPT)
            gridFeriadosDesconsiderados = dijit.byId('gridDesconsideraFeriadosPPT');
        if (hasValue(gridFeriadosDesconsiderados)) {
            feriados_descosiderados = new Array();
            var dataFeriadosDesconsiderados = gridFeriadosDesconsiderados.store.objectStore.data;
            for (var i = 0; i < dataFeriadosDesconsiderados.length; i++)
                feriados_descosiderados.push({
                    cd_feriado_desconsiderado: dataFeriadosDesconsiderados[i].cd_feriado_desconsiderado,
                    dt_inicial: dataFeriadosDesconsiderados[i].dt_inicial,
                    dt_final: dataFeriadosDesconsiderados[i].dt_final,
                    id_programacao_feriado: dataFeriadosDesconsiderados[i].id_programacao_feriado
                });
        }
        xhr.post({
            url: Endereco() + "/api/coordenacao/postGerarModeloProgramacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            handleAs: "json",
            preventCache: true,
            postData: ref.toJson({
                programacoes: programacoes,
                //horarios: null,
                //dt_inicio: null,
                cd_curso: cd_curso,
                cd_duracao: cd_duracao,
                cd_turma: parseInt(cd_turma),
                //tipo: null,
                feriados_desconsiderados: dataFeriadosDesconsiderados
            })
        }).then(function (dataProgTurma) {
            try {
                apresentaMensagem(apresentadorMsg, dataProgTurma, true);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(apresentadorMsg, error, true);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function gerarModeloProgramacaoAcaoRelacionada(xhr, ref, apresMsg) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    try {
        var cd_turmas = new Array();
        var turmasSelecionadas = dijit.byId('gridTurma').itensSelecionados;

        if (!hasValue(turmasSelecionadas) || turmasSelecionadas.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaNaoSelecionada);
            apresentaMensagem(apresMsg, mensagensWeb);
            return false;
        }
        for (var i = 0; i < turmasSelecionadas.length; i++)
            cd_turmas.push(turmasSelecionadas[i].cd_turma);

        xhr.post({
            url: Endereco() + "/api/coordenacao/postGerarModeloProgramacaoAcaoRelacionada",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(cd_turmas)
        }).then(function (data) {
            apresentaMensagem(apresMsg, data);
        },
        function (error) {
            apresentaMensagem(apresMsg, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirProgramacoesCurso(xhr, ref, apresMsg) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    try {
        var cd_turmas = new Array();
        var turmasSelecionadas = dijit.byId('gridTurma').itensSelecionados;

        if (!hasValue(turmasSelecionadas) || turmasSelecionadas.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaNaoSelecionada);
            apresentaMensagem(apresMsg, mensagensWeb);
            return false;
        }

        for (var i = 0; i < turmasSelecionadas.length; i++)
            cd_turmas.push(turmasSelecionadas[i].cd_turma);

        xhr.post({
            url: Endereco() + "/api/coordenacao/postIncluirProgramacoesCurso",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(cd_turmas)
        }).then(function (data) {
            apresentaMensagem(apresMsg, data);
        },
        function (error) {
            apresentaMensagem(apresMsg, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirDiarioAula(xhr, ref, apresMsg, ready, Memory, FilteringSelect, on) {
    try {
        var cd_turmas = new Array();
        var turmasSelecionadas = dijit.byId('gridTurma').itensSelecionados;

        if (!hasValue(turmasSelecionadas) || turmasSelecionadas.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTurmaNaoSelecionada);
            apresentaMensagem(apresMsg, mensagensWeb);
            return false;
        }
        else if (turmasSelecionadas.length > 1) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSelecioneApenasUmaTurma);
            apresentaMensagem(apresMsg, mensagensWeb);
            return false;
        }
        if (!hasValue(dijit.byId('incluirDiario')))
            montarDiarioPartial(function () {
                abreCadastroDiarioAula(xhr, ready, Memory, FilteringSelect, turmasSelecionadas[0], false);
                setarEventosBotoesPrincipais(xhr, on);
            });
        else {
            abreCadastroDiarioAula(xhr, ready, Memory, FilteringSelect, turmasSelecionadas[0], false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirProgramacaoCurso(xhr, nameGridProgramacao, gridFeriadosDesconsiderados, ref, apresMsg, is_modelo) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    require(["dojo/ready"], function (ready) {
        ready(function () {
            try {
                //Recupera os horários e as programações:
                //var gridHorarios = dijit.byId('gridHorarios');
                var gridProgramacao = dijit.byId(nameGridProgramacao);
                if (hasValue(gridProgramacao) && gridProgramacao["store"] != null && gridProgramacao["store"] != undefined && gridProgramacao.store.objectStore.data.length > 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorExisteProgramacao);
                    apresentaMensagem(apresMsg, mensagensWeb);
                    return false;
                }
                var horariosOcupadoTurma = new Array();
                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                var horarios = new Array();
                var programacoes = new Array();
                var feriados_descosiderados = new Array();
                var dtIniAula = null;
                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                var tipoVerif = dojo.byId("tipoVerif").value;


                if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                    horarios = validaRetornaHorariosFIlhaPPT();
                else
                    horarios = validaRetornaHorarios(tipoTurma);

                if (horarios.length > 0)
                    $.each(horarios, function (index, value) {
                        if (value.calendar == "Calendar2" || !hasValue(value.calendar))
                            horariosOcupadoTurma.push(value);
                    });

                if (tipoTurma != TURMA_PPT)
                    dtIniAula = hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                else dtIniAula = hasValue(dojo.byId("dtIniAulaPPT").value) ? dojo.date.locale.parse(dojo.byId("dtIniAulaPPT").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
                var dataProgramacoes = gridProgramacao.store.objectStore.data;
                for (var i = 0; i < dataProgramacoes.length; i++)
                    programacoes.push({
                        cd_programacao_turma: dataProgramacoes[i].cd_programacao_turma,
                        dc_programacao_turma: dataProgramacoes[i].dc_programacao_turma,
                        dta_programacao_turma: dojo.date.locale.parse(dataProgramacoes[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                        hr_inicial_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[0] : '',
                        hr_final_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[1] : '',
                        nm_aula_programacao_turma: dataProgramacoes[i].nm_aula_programacao_turma
                    });

                var dataFeriadosDesconsiderados = gridFeriadosDesconsiderados.store.objectStore.data;
                for (var i = 0; i < dataFeriadosDesconsiderados.length; i++)
                    feriados_descosiderados.push({
                        cd_feriado_desconsiderado: dataFeriadosDesconsiderados[i].cd_feriado_desconsiderado,
                        dt_inicial: dataFeriadosDesconsiderados[i].dt_inicial,
                        dt_final: dataFeriadosDesconsiderados[i].dt_final,
                        id_programcao_feriado: dataFeriadosDesconsiderados[i].id_programcao_feriado
                    });

                var cd_curso = dijit.byId("pesCadCurso").value;
                var cd_duracao = dijit.byId("pesCadDuracao").value;

                if (tipoTurma == TURMA_PPT) {
                    cd_curso = dijit.byId("pesCadCursoPPT").value;
                    cd_regime = dijit.byId("pesCadRegimePPT").value;
                    cd_duracao = dijit.byId("pesCadDuracaoPPT").value;
                }
                xhr.post({
                    url: Endereco() + "/api/coordenacao/postIncluirProgramacaoCurso",
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    postData: ref.toJson({ programacoes: programacoes, horarios: horariosOcupadoTurma, dt_inicio: dtIniAula, cd_curso: cd_curso, cd_duracao: cd_duracao, feriados_desconsiderados: dataFeriadosDesconsiderados, modelo: is_modelo })
                }).then(function (data) {
                    try {
                        apresentaMensagem(apresMsg, data);
                        var retorno = jQuery.parseJSON(data).retorno;
                        var cd_turma = document.getElementById('cd_turma').value;
                        for (var i = 0; i < retorno.length; i++) {
                            var novaProgramacao = {
                                aula_efetivada: false,
                                cd_feriado: retorno[i].cd_feriado,
                                cd_programacao_turma: 0,
                                cd_turma: hasValue(cd_turma) ? cd_turma : 0,
                                dc_programacao_turma: retorno[i].dc_programacao_turma,
                                dt_programacao_turma: retorno[i].dt_programacao_turma,
                                hr_final_programacao: retorno[i].hr_final_programacao,
                                hr_inicial_programacao: retorno[i].hr_inicial_programacao,
                                id_aula_dada: false,
                                id_programacao_manual: is_modelo ? PROGRAMACAO_GERADA_MODELO : PROGRAMACAO_GERADA_CURSO,
                                nm_aula_programacao_turma: retorno[i].nm_aula_programacao_turma,
                                hr_aula: retorno[i].hr_inicial_programacao + ' ' + retorno[i].hr_final_programacao,
                                programacaoTurmaEdit: false,
                                dta_programacao_turma: dojo.date.locale.parse(retorno[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                            };

                            gridProgramacao.store.newItem(novaProgramacao);
                        }
                        gridProgramacao.store.save();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
				function (error) {
				    apresentaMensagem(apresMsg, error);
				});
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function refazerNumeracaoProgramacao(grid, itensSelecionados) {
    apresentaMensagem('apresentadorMensagemProgramacao', '');
    apresentaMensagem('apresentadorMensagemTurma', '');
    var existeNova = false;
    var cdTurma = (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) ? dojo.byId(cd_turma).value : itensSelecionados[0].cd_turma;
    if (cdTurma == null || cdTurma == 0) {
        caixaDialogo(DIALOGO_ERRO, "A turma deve estar salva para oder realizar esta operação", null);
        return false
    }

    var msg = "Deseja refazer a numeração destas programações?";
    try {
        if (hasValue(itensSelecionados) && itensSelecionados.length >= 1)
            msg = "Apesar de ter selecionado apenas uma programação, todas as outras poderão ser renumeradas. " + msg;
        for (var i = 0; i < grid._by_idx.length; i++) {
            if (grid._by_idx[i].item.cd_programacao_turma == "0" || grid._by_idx[i].item.cd_programacao_turma == 0) {
                existeNova = true;
                break;
            }
        }
        if (existeNova) {
            caixaDialogo(DIALOGO_ERRO, "Favor Salvar a Turma primeiro pois a numeração somente é feita em programações já gravadas", null);
            return false
        }
        caixaDialogo(DIALOGO_CONFIRMAR, msg, function () { refazerNumeracao(cdTurma); });
    }
    catch (e) {
        postGerarLog(e);
    }

}

function refazerNumeracao(cdTurma) {
    try {
        showCarregando();

        dojo.xhr.post({
            url: Endereco() + "/api/turma/postRefazerProgramacao",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({ cd_turma: cdTurma })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                if (data != '0') {
                    if (hasValue(data.erro) || !hasValue(data.retorno)) {
                        hideCarregando();
                        caixaDialogo(DIALOGO_AVISO, data, null);
                        return false;
                    }
                } else {
                    hideCarregando();
                    loadProgramacao(cdTurma, tipoTurma);
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Renumeração feita com sucesso");
                    apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                }
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
                return false;
            }
        }, function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemTurma', error);
            return false;
        });

    }
    catch (e) {
        postGerarLog(e);
    }
}

function refazerNumeracaoAuxiliar(grid, itensSelecionados) {
    apresentaMensagem('apresentadorMensagemProgramacao', '');
    apresentaMensagem('apresentadorMensagemTurma', '');
    var existeNova = false;
    var cdTurma = (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) ? dojo.byId(cd_turma).value : itensSelecionados[0].cd_turma;
    if (cdTurma == null || cdTurma == 0) {
        caixaDialogo(DIALOGO_ERRO, "A turma deve estar salva para oder realizar esta operação", null);
        return false
    }

    var msg = "Deseja refazer a numeração destas programações?";
    try {
        if (hasValue(itensSelecionados) && itensSelecionados.length >= 1)
            msg = "Apesar de ter selecionado apenas uma programação, todas as outras poderão ser renumeradas. " + msg;
        for (var i = 0; i < grid._by_idx.length; i++) {
            if (grid._by_idx[i].item.cd_programacao_turma == "0" || grid._by_idx[i].item.cd_programacao_turma == 0) {
                existeNova = true;
                break;
            }
        }
        if (existeNova) {
            caixaDialogo(DIALOGO_ERRO, "Favor Salvar a Turma primeiro pois a numeração somente é feita em programações já gravadas", null);
            return false
        }
        caixaDialogo(DIALOGO_CONFIRMAR, msg, function () { refazerNumeracaoAux(cdTurma); });
    }
    catch (e) {
        postGerarLog(e);
    }


}

function refazerNumeracaoAux(cdTurma) {
    try {
        showCarregando();

        dojo.xhr.post({
            url: Endereco() + "/api/turma/postRefazerNumeracao",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({ cd_turma: cdTurma })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                if (data != '0') {
                    if (hasValue(data.erro) || !hasValue(data.retorno)) {
                        hideCarregando();
                        caixaDialogo(DIALOGO_AVISO, data, null);
                        return false;
                    }
                } else {
                    hideCarregando();
                    loadProgramacao(cdTurma, tipoTurma);
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Renumeração auxiliar feita com sucesso");
                    apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                }
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
                return false;
            }
        }, function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemTurma', error);
            return false;
        });

    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function incluirProgramacao(xhr, gridProgramacao, gridFeriadosDesconsiderados, ref, apresMsg) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    try {
        //Recupera os horários e as programações:
        //var gridHorarios = dijit.byId('gridHorarios');
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var horariosOcupadoTurma = new Array();
        var programacoes = new Array();
        var feriados_descosiderados = new Array();
        var dtIniAula = null;
        var tipoVerif = dojo.byId("tipoVerif").value;

        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
            horarios = validaRetornaHorariosFIlhaPPT();
        else
            horarios = validaRetornaHorarios(tipoTurma);

        apresentaMensagem('apresentadorMensagemProgramacao', '');
        if (hasValue(horarios) && horarios.length > 0)
            $.each(horarios, function (index, value) {
                if (value.calendar == "Calendar2" || !hasValue(value.calendar))
                    horariosOcupadoTurma.push(value);
            });

        if (tipoTurma != TURMA_PPT)
            dtIniAula = hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        else
            dtIniAula = hasValue(dojo.byId("dtIniAulaPPT").value) ? dojo.date.locale.parse(dojo.byId("dtIniAulaPPT").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

        var dataProgramacoes = gridProgramacao.store.objectStore.data;
        for (var i = 0; i < dataProgramacoes.length; i++)
            programacoes.push({
                cd_programacao_turma: dataProgramacoes[i].cd_programacao_turma,
                dc_programacao_turma: dataProgramacoes[i].dc_programacao_turma,
                cd_feriado: dataProgramacoes[i].cd_feriado,
                dta_programacao_turma: dojo.date.locale.parse(dataProgramacoes[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                hr_inicial_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[0] : '',
                hr_final_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[1] : '',
                nm_aula_programacao_turma: dataProgramacoes[i].nm_aula_programacao_turma,
                id_programacao_manual: dataProgramacoes[i].id_programacao_manual,
                nm_programacao_real: dataProgramacoes[i].nm_programacao_real
            });

        var dataFeriadosDesconsiderados = gridFeriadosDesconsiderados.store.objectStore.data;
        for (var i = 0; i < dataFeriadosDesconsiderados.length; i++)
            feriados_descosiderados.push({
                cd_feriado_desconsiderado: dataFeriadosDesconsiderados[i].cd_feriado_desconsiderado,
                dt_inicial: dataFeriadosDesconsiderados[i].dt_inicial,
                dt_final: dataFeriadosDesconsiderados[i].dt_final,
                id_programcao_feriado: dataFeriadosDesconsiderados[i].id_programcao_feriado
            });

        xhr.post({
            url: Endereco() + "/api/turma/postIncluirProgramacao",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson({ programacoes: programacoes, horarios: horariosOcupadoTurma, dt_inicio: dtIniAula, feriados_desconsiderados: dataFeriadosDesconsiderados })
        }).then(function (data) {
            try {
                apresentaMensagem(apresMsg, null);
                dijit.byId("dialogProgramacao").show();

                var retorno = jQuery.parseJSON(data).retorno;
                var cd_turma = document.getElementById('cd_turma').value;

                for (var i = 0; i < retorno.length; i++)
                    //Caso seja o horário encontrado:
                    if (i == retorno.length - 1) {
                        dojo.byId('cd_programacao_turma').value = retorno[i].cd_programacao_turma;
                        dojo.byId('horarioIniProgramacao').value = retorno[i].hr_inicial_programacao;
                        dojo.byId('horarioFimProgramacao').value = hasValue(retorno[i].hr_final_programacao) ? retorno[i].hr_final_programacao : '';
                        dojo.byId('dtaProgramacao').value = retorno[i].dt_programacao_turma;
                        dojo.byId('descricaoProgramacao').value = retorno[i].dc_programacao_turma;
                        dojo.byId('aulaProgramacao').value = retorno[i].nm_aula_programacao_turma;
                        dojo.byId('aulaProgramacao').original_value = retorno[i].nm_aula_programacao_turma;
                    }
                        //Caso seja algum feriado, já inclui na grade direto:
                    else {
                        var programacaoFeriado = {
                            aula_efetivada: false,
                            cd_feriado: retorno[i].cd_feriado,
                            cd_programacao_turma: 0,
                            cd_turma: hasValue(cd_turma) ? cd_turma : 0,
                            dc_programacao_turma: retorno[i].dc_programacao_turma,
                            dt_programacao_turma: retorno[i].dt_programacao_turma,
                            dta_programacao_turma: dojo.date.locale.parse(retorno[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                            hr_final_programacao: retorno[i].hr_final_programacao,
                            hr_inicial_programacao: retorno[i].hr_inicial_programacao,
                            id_aula_dada: false,
                            nm_aula_programacao_turma: retorno[i].nm_aula_programacao_turma,
                            id_programacao_manual: PROGRAMACAO_MANUAL
                        };

                        gridProgramacao.store.newItem(programacaoFeriado);
                    }
                gridProgramacao.store.save();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(apresMsg, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizaFeriadosDesconsideradosComProgramacao() {
    try {
        var gridDesconsideraFeriados = dijit.byId('gridDesconsideraFeriados');
        var gridProgramacao = dijit.byId('gridProgramacao');

        var tipoTurma = dojo.byId("tipoTurmaCad").value;

        if (tipoTurma == TURMA_PPT) {
            gridDesconsideraFeriados = dijit.byId('gridDesconsideraFeriadosPPT');
            gridProgramacao = dijit.byId('gridProgramacaoPPT');
        }
        var programacoesTurma = gridProgramacao.store.objectStore.data;
        var feriadosDesconsiderados = gridDesconsideraFeriados.store.objectStore.data;

        if (hasValue(feriadosDesconsiderados, true) && hasValue(programacoesTurma, true)) {
            for (var j = 0; j < feriadosDesconsiderados.length; j++) {
                var tem_programacao = false;
                for (var i = 0; i < programacoesTurma.length; i++) {
                    var dataProg = dojo.date.locale.parse(programacoesTurma[i].dt_programacao_turma + ' ' + programacoesTurma[i].hr_final_programacao
                        , { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });

                    //Verifica a intersecção das datas:
                    if ((dojo.date.compare(feriadosDesconsiderados[j].dt_inicial, dataProg, "date") >= 0 && dojo.date.compare(feriadosDesconsiderados[j].dt_inicial, dataProg, "date") <= 0)
                            || (dojo.date.compare(feriadosDesconsiderados[j].dt_final, dataProg, "date") >= 0 && dojo.date.compare(feriadosDesconsiderados[j].dt_final, dataProg, "date") <= 0)
                            || (dojo.date.compare(feriadosDesconsiderados[j].dt_inicial, dataProg, "date") <= 0 && dojo.date.compare(feriadosDesconsiderados[j].dt_final, dataProg, "date") >= 0))
                        tem_programacao = true;
                }
                if (tem_programacao) {
                    feriadosDesconsiderados[j].id_programacao_feriado = 1;
                    feriadosDesconsiderados[j].dc_programacao_feriado = 'Sim';
                }
                else {
                    feriadosDesconsiderados[j].id_programacao_feriado = 0;
                    feriadosDesconsiderados[j].dc_programacao_feriado = 'Não';
                }
            }
            gridDesconsideraFeriados.store.save();
            gridDesconsideraFeriados.update();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirProgramacaoGrid() {
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var gridProg = null;
        var apresMsg = 'apresentadorMensagemProgramacao';
        if (tipoTurma != null && tipoTurma != TURMA_PPT)
            gridProg = dijit.byId('gridProgramacao');
        else
            gridProg = dijit.byId('gridProgramacaoPPT');

        if (!dijit.byId("formProgramacao").validate())
            return false;
        var nova_programacao = montaProgramacao();

        if (dojo.byId('aulaProgramacao').original_value == dojo.byId('aulaProgramacao').value && hasValue(gridProg)) {
            gridProg.store.newItem(nova_programacao);
            gridProg.store.save();
        }
        else {
            //Verifica se a aula é igual a zero ou maior que a da última programação:
            if (dojo.byId('aulaProgramacao').value == '0') {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroProgramacaoNroZero);
                apresentaMensagem(apresMsg, mensagensWeb);

                return false;
            }

            gridProg.store.save();
            var dados = gridProg.store.objectStore.data;

            if (dados.length > 1 && nova_programacao.nm_aula_programacao_turma > dados[dados.length - 1].nm_aula_programacao_turma + 1) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroProgramacaoNroNaoPermitido);
                apresentaMensagem(apresMsg, mensagensWeb);

                return false;
            }

            //Verifica se está incluindo uma programação anterior a um diário de aula ou no lugar de um feriado:
            for (var i = dados.length - 1; i >= 0; i--) {
                if (dados[i].id_aula_dada) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroProgramacaoNroNaoPermitidoDiario);
                    apresentaMensagem(apresMsg, mensagensWeb);

                    return false;
                }
                else if (nova_programacao.nm_aula_programacao_turma == dados[i].nm_aula_programacao_turma) {
                    if ((hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroProgramacaoNroNaoPermitidoFeriado);
                        apresentaMensagem(apresMsg, mensagensWeb);

                        return false;
                    }
                    break;
                }
            }

            //Remaneja as descrições conforme a posição que se deseja incluir:
            var no_programacao = '';
            var dc_programacao_turma = '';
            var passou_reprogramacao = false;
            for (var i = 0; i < dados.length; i++) {
                if (nova_programacao.nm_aula_programacao_turma == dados[i].nm_aula_programacao_turma || passou_reprogramacao) {
                    //Pula os feriados:
                    while (hasValue(dados[i]) && hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)
                        i += 1;
                    if (hasValue(dados[i])) {

                        // Condição para primeira interação do for não alterar valor da propriedade "id_mostrar_calendario".
                        // primeira interação valor do parâmetro "passou_reprogramacao" é igual a falso.
                        if (passou_reprogramacao)
                        {
                            id_mostrar_calendario = dados[i].id_mostrar_calendario;
                            dados[i].id_mostrar_calendario = nova_programacao.id_mostrar_calendario;
                            nova_programacao.id_mostrar_calendario = id_mostrar_calendario;
                        }

                        if (nova_programacao.nm_aula_programacao_turma == dados[i].nm_aula_programacao_turma &&
                            dados[i].id_programacao_manual) {
                            dados[i].id_programacao_manual = PROGRAMACAO_MANUAL;
                        } else {
                            dados[i].id_programacao_manual = 0;
                            nova_programacao.id_programacao_manual = 0;
                        }

                        passou_reprogramacao = true;
                        no_programacao = dados[i].no_programacao;
                        dc_programacao_turma = dados[i].dc_programacao_turma;

                        dados[i].no_programacao = nova_programacao.no_programacao;
                        dados[i].dc_programacao_turma = nova_programacao.dc_programacao_turma;                        
                        dados[i].programacaoTurmaEdit = true;

                        nova_programacao.no_programacao = no_programacao;
                        nova_programacao.dc_programacao_turma = dc_programacao_turma;
                    }
                    //Pula os feriados:
                    while (hasValue(dados[i]) && hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)
                        i += 1;
                }
            }
            if (dados.length > 0)
                nova_programacao.nm_aula_programacao_turma = dados[dados.length - 1].nm_aula_programacao_turma + 1;
            gridProg.update();
            gridProg.store.newItem(nova_programacao);
            gridProg.store.save();

            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgAvisoProgramacoesRefeitasNaInclusao);
            apresentaMensagem('apresentadorMensagemTurma', mensagensWeb);
        }

        // Verifica se tem que atualizar a grid de desconsidera feriados:
        atualizaFeriadosDesconsideradosComProgramacao();

        dijit.byId("dialogProgramacao").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaProgramacao() {
    try {
        return {
            cd_programacao_turma: dojo.byId('cd_programacao_turma').value,
            nm_aula_programacao_turma: parseInt(dojo.byId('aulaProgramacao').value),
            dta_programacao_turma: dojo.date.locale.parse(dojo.byId("dtaProgramacao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
            dt_programacao_turma: dojo.byId("dtaProgramacao").value,
            hr_aula: dojo.byId('horarioIniProgramacao').value + ' ' + dojo.byId('horarioFimProgramacao').value,
            hr_inicial_programacao: dojo.byId('horarioIniProgramacao').value,
            hr_final_programacao: dojo.byId('horarioFimProgramacao').value,
            dc_programacao_turma: dojo.byId('descricaoProgramacao').value,
            programacaoTurmaEdit: false,
            id_aula_dada: false,
            id_programacao_manual: PROGRAMACAO_MANUAL,
            id_mostrar_calendario: false
        };
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditaProgramacao(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            var gridProg = null;
            document.getElementById("divAlterarProg").style.display = "";
            document.getElementById("divCancelarProg").style.display = "";
            document.getElementById("divIncluirProg").style.display = "none";
            if (tipoTurma != null && tipoTurma != TURMA_PPT) {
                apresentaMensagem('apresentadorMensagemTurma', '');
                keepValuesProgramacao(null, dijit.byId('gridProgramacao'), true);
            } else {
                apresentaMensagem('apresentadorMensagemTurmaPPT', '');
                keepValuesProgramacao(null, dijit.byId('gridProgramacaoPPT'), true);
            }
            dijit.byId("dialogProgramacao").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaProgramacoesSelecionadas(grid, itensSelecionados) {
    var retorno = null;

    for (var i = 0; i < itensSelecionados.length; i++) {
        if (itensSelecionados[i].id_aula_dada) {
            retorno = msgErroCancelarProgAulaDada + ". Programação: (" + itensSelecionados[i].dc_programacao_turma + " - " + itensSelecionados[i].dt_programacao_turma + ").";
            break;
        }

        else if (itensSelecionados[i].cd_feriado != null) {
            retorno = msgErroCancelarProgAulaDada + ". Programação: (" + itensSelecionados[i].dc_programacao_turma + " - " + itensSelecionados[i].dt_programacao_turma + ").";
            break;
        }
		    
	    else if (itensSelecionados[i].cd_programacao_turma == 0) {
            retorno = msgErroCancelarProgNova + ". Programação: (" + itensSelecionados[i].dc_programacao_turma + " - " + itensSelecionados[i].dt_programacao_turma + ").";
            break;
        }
		    
    }

    return retorno;

}

function eventoCancelaProgramacao(grid, itensSelecionados) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    var existeNova = false;
    var msg = "Deseja cancelar esta(s) programação(ões)?";
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        //else if (itensSelecionados.length > 1)
        //    caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else if (validaProgramacoesSelecionadas(grid, itensSelecionados) != null) {
	        caixaDialogo(DIALOGO_ERRO, validaProgramacoesSelecionadas(grid, itensSelecionados), null);
        }
        else {
            for (var i = 0; i < grid._by_idx.length; i++) {
                if (grid._by_idx[i].item.cd_programacao_turma == 0) {
                    existeNova = true;
                    break;
                }
            }
            if (existeNova)
                caixaDialogo(DIALOGO_CONFIRMAR, msgAvisoCancelarOutraProgNova + msg, function () { cancelarProgramacao(itensSelecionados); });
            else
                caixaDialogo(DIALOGO_CONFIRMAR, msg, function () { cancelarProgramacao(itensSelecionados); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function cancelarProgramacao(itensSelecionados) {
    try {
        var cd_turma = itensSelecionados[0].cd_turma;

        cds_programacao_turma = itensSelecionados.map(function (item) { return item.cd_programacao_turma})

        dojo.xhr.post({
            url: Endereco() + "/api/turma/postCancelarProgramacao",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(cds_programacao_turma)
        }).then(function (data) {
            try {
                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                apresentaMensagem('apresentadorMensagemTurma', data);
                if (tipoTurma != null && tipoTurma != TURMA_PPT) {
                    apresentaMensagem('apresentadorMensagemTurma', data);
                } else {
                    apresentaMensagem('apresentadorMensagemTurmaPPT', data);
                }
                dojo.xhr.get({
                    preventCache: true,
                    url: Endereco() + "/api/turma/getProgramacoesTurma?cd_turma=" + parseInt(cd_turma),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataProgTurma) {
                    try {
                        dataProgTurma = jQuery.parseJSON(dataProgTurma);
                        if (tipoTurma != TURMA_PPT)
                            criacaoComponentesProgramacao(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                        else
                            criacaoComponentesProgramacaoPPTFilha(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                    function (error) {
                        apresentaMensagem(apresentadorMsg, error);

                    });
            }
            catch (e) {
                postGerarLog(e);
                return false;
            }
        }, function (error) {
                apresentaMensagem('apresentadorMensagemTurma', error);
                return false;
        });

    }
    catch (e) {
            postGerarLog(e);
    }
}

function eventoDesfazerCancelarProgramacao(grid, itensSelecionados) {
    if (OUTRAESCOLA) {
        caixaDialogo(DIALOGO_AVISO, msgAvisoTurmaEditOutraEscola, null);
        return false;
    }
    var existeNova = false;
    var msg = "Deseja desfazer o cancelamento desta(s) programação(ões)?"
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        //else if (itensSelecionados.length > 1)
        //    caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else if (validaProgramacoesSelecionadas(grid, itensSelecionados) != null) {
	        caixaDialogo(DIALOGO_ERRO, validaProgramacoesSelecionadas(grid, itensSelecionados), null);
        }
        else {
            for (var i = 0; i < grid._by_idx.length; i++) {
                if (grid._by_idx[i].item.cd_programacao_turma == 0) {
                    existeNova = true;
                    break;
                }
            }
            if (existeNova)
                caixaDialogo(DIALOGO_CONFIRMAR, msgAvisoCancelarOutraProgNova + msg, function () { desfazerCancelarProgramacao(itensSelecionados); });
            else
                caixaDialogo(DIALOGO_CONFIRMAR, msg, function () { desfazerCancelarProgramacao(itensSelecionados); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function desfazerCancelarProgramacao(itensSelecionados) {
    try {
        var cd_turma = itensSelecionados[0].cd_turma;

        cds_programacao_turma = itensSelecionados.map(function (item) { return item.cd_programacao_turma })

        dojo.xhr.post({
            url: Endereco() + "/api/turma/postDesfazerCancelarProgramacao",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(cds_programacao_turma)
        }).then(function (data) {
            try {
                var tipoTurma = dojo.byId("tipoTurmaCad").value;
                apresentaMensagem('apresentadorMensagemTurma', data);
                if (tipoTurma != null && tipoTurma != TURMA_PPT) {
                    apresentaMensagem('apresentadorMensagemTurma', data);
                } else {
                    apresentaMensagem('apresentadorMensagemTurmaPPT', data);
                }
                dojo.xhr.get({
                    preventCache: true,
                    url: Endereco() + "/api/turma/getProgramacoesTurma?cd_turma=" + parseInt(cd_turma),
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (dataProgTurma) {
                    try {
                        dataProgTurma = jQuery.parseJSON(dataProgTurma);
                        if (tipoTurma != TURMA_PPT)
                            criacaoComponentesProgramacao(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                        else
                            criacaoComponentesProgramacaoPPTFilha(dataProgTurma.retorno.Programacoes, dataProgTurma.retorno.FeriadosDesconsiderados);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                    function (error) {
                        apresentaMensagem(apresentadorMsg, error);

                    });
            }
            catch (e) {
                postGerarLog(e);
                return false;
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemTurma', error);
            return false;
        });

    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditaDesconsidera(itensSelecionados) {
    try {
        if (OUTRAESCOLA) {
            caixaDialogo(DIALOGO_AVISO, msgErroMatriculaAlunoOutroaEscola, null);
            return false;
        } 
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            var gridProg = null;
            document.getElementById("divAlterarDesc").style.display = "";
            document.getElementById("divCancelarDesc").style.display = "";
            document.getElementById("divIncluirDesc").style.display = "none";
            if (tipoTurma != null && tipoTurma != TURMA_PPT) {
                apresentaMensagem('apresentadorMensagemTurma', '');
                var grid = dijit.byId('gridDesconsideraFeriados');
                grid.itemSelecionado = grid.itensSelecionados[0];
                keepValuesFeriadoDesconsiderado(grid, true);
            } else {
                apresentaMensagem('apresentadorMensagemTurmaPPT', '');
                var grid = dijit.byId('gridDesconsideraFeriadosPPT');
                grid.itemSelecionado = grid.itensSelecionados[0];
                keepValuesFeriadoDesconsiderado(grid, true);
            }
            dijit.byId("dialogDesconsideracao").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesProfessor(value, grid, ehLink, xhr, Memory, EnhancedGrid) {
    try {
        apresentaMensagem('apresentadorMensagemTurma', '');
        apresentaMensagem('apresentadorMensagemProfessor', null);
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('linkTagProf');
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        if (value != null) {
            grid.itemSelecionado = value;
            var valor = value;
            var cloneHorariosProf = new Array();
            dojo.byId('no_professor').value = valor.no_professor;
            if (valor.id_professor_ativo)
                dijit.byId('situacaoProfessorTurma').set('checked', true);
            else
                dijit.byId('situacaoProfessorTurma').set('checked', false);

            var sorto = {
                id_dia_semana: "asc", dt_hora_ini: "asc", dt_hora_fim: "asc"
            };
            valor.horarios.keySort(sorto);
            if (hasValue(valor.horarios))
                cloneHorariosProf = clonarHorarios(valor.horarios);
            dijit.byId("gridHorarioProfessor").setStore(new dojo.data.ObjectStore({ objectStore: new Memory({ data: cloneHorariosProf }) }));
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesProgramacao(value, grid, ehLink) {
    try {
        var valorCancelamento;
        if (hasValue(grid) && hasValue(grid.selection))
            valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('linkProg');
        //limparCadTurma(Memory);

        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        dojo.byId('cd_programacao_turma').value = value.cd_programacao_turma;
        if (hasValue(value.hr_aula) && value.hr_aula != '...') {
            dojo.byId('horarioIniProgramacao').value = value.hr_aula.split(' ')[0];
            dojo.byId('horarioFimProgramacao').value = value.hr_aula.split(' ')[1];
        }
        else {
            dojo.byId('horarioIniProgramacao').value = '';
            dojo.byId('horarioFimProgramacao').value = '';
        }
        dojo.byId('dtaProgramacao').value = value.dt_programacao_turma;
        dojo.byId('descricaoProgramacao').value = value.dc_programacao_turma
        dojo.byId('aulaProgramacao').value = value.nm_aula_programacao_turma;
        dojo.byId('aulaProgramacao').original_value = value.nm_aula_programacao_turma;
        dijit.byId('aulaProgramacao').set('disabled', true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesFeriadoDesconsiderado(grid, ehLink) {
    try {
        var value = grid.itemSelecionado;
        dojo.byId('dt_fim_des').value = value.dta_final;
        dojo.byId('dt_inicio_des').value = value.dta_inicial;
        document.getElementById('cd_feriado_desconsiderado').value = value.cd_feriado_desconsiderado;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarProgramacao(Memory) {
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var gridProg = null;
        if (tipoTurma != null && tipoTurma != TURMA_PPT)
            gridProg = dijit.byId('gridProgramacao');
        else
            gridProg = dijit.byId('gridProgramacaoPPT');
        if (!dijit.byId("formProgramacao").validate())
            return false;
        var cd_programacao_turma = dojo.byId('cd_programacao_turma').value;
        var nm_aula_programacao_turma = parseInt(dojo.byId('aulaProgramacao').value);
        if (!dijit.byId("formProgramacao").validate())
            return false;

        if (hasValue(gridProg) && hasValue(gridProg.store.objectStore.data))
            $.each(gridProg.store.objectStore.data, function (index, value) {
                if ((eval(value.cd_programacao_turma) == 0 && value.nm_aula_programacao_turma == nm_aula_programacao_turma) || (value.cd_programacao_turma > 0 && value.cd_programacao_turma == cd_programacao_turma)) {
                    value.dc_programacao_turma = dojo.byId('descricaoProgramacao').value;
                    value.programacaoTurmaEdit = value.cd_programacao_turma > 0 ? true : false;
                    return false;
                }
            });
        gridProg.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: gridProg.store.objectStore.data }) }));
        dijit.byId("dialogProgramacao").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarItemSelecionadoGridProgramacao(Memory, ObjectStore, nomeId, grid, apresMsg) {
    try {
        apresentaMensagem(apresMsg, '');
        grid.store.save();
        var dados = grid.store.objectStore.data;
        var valido = true;

        if (dados.length > 0 && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length > 0) {
            var passou_diario_aula = false;

            //Verifica se tem algum registro com diário de aula ou feriado:
            grid.itensSelecionados.sort(function byOrdem(a, b) { return a.nm_aula_programacao_turma - b.nm_aula_programacao_turma; });

            //Remove os últimos feriados antes de começar:
            for (var i = dados.length - 1; i >= 0; i--)
                if (!dados[i].id_aula_dada && (hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)) {
                    if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null)
                        removeObjSort(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId));
                    dados.splice(i, 1); // Remove o item do array
                }
                else
                    break;

            //Percorre a lista da grade para deleção (O(n)):
            for (var i = dados.length - 1; i >= 0; i--) {
                // Verifica se os itens selecionados estão na lista e se trata de um feriado ou diário de aula:
                if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null) {
                    if (dados[i].id_aula_dada || (hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)) {
                        valido = false;
                        break;
                    }
                }
                else
                    if (!dados[i].id_aula_dada && !(hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgRefazProgramacoesDasExclusoes);
                        apresentaMensagem(apresMsg, mensagensWeb);
                    }

                // Contabiliza a quantidade de itens deletados para ver se está deletando antes dos itens com diário de aula:
                if (dados[i].id_aula_dada)
                    passou_diario_aula = true;

                if (passou_diario_aula)
                    if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null) {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExcProgAnteriorDiarioAula);
                        apresentaMensagem(apresMsg, mensagensWeb);
                        return false;
                    }
            }

            if (!valido) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotExcProgTDiarioAula);
                apresentaMensagem(apresMsg, mensagensWeb);
                return false;
            }

            //Dá um shift nas descrições das programações que foram deletadas:
            for (var i = dados.length - 1; i >= 0; i--)
                // Verifica se os itens selecionados estão na lista e se trata de um feriado ou diário de aula:
                if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null)
                    for (var j = i; j < dados.length - 1; j++) {
                        //Se for o feriado, pula ele:
                        while (hasValue(dados[j].cd_feriado) && !hasValue(dados[j].cd_feriado_desconsiderado) && !dados[j].id_feriado_desconsiderado)
                            j += 1;

                        var l = j + 1;
                        //Se for feriado, pula a descrição do feriado:
                        while (hasValue(dados[l]) && hasValue(dados[l].cd_feriado) && !hasValue(dados[l].cd_feriado_desconsiderado) && !dados[l].id_feriado_desconsiderado)
                            l += 1;
                        if (l < dados.length) {
                            dados[j].no_programacao = dados[l].no_programacao;
                            dados[j].dc_programacao_turma = dados[l].dc_programacao_turma;
                            dados[j].id_mostrar_calendario = dados[l].id_mostrar_calendario;
                            dados[j].programacaoTurmaEdit = true;
                        }
                    }

            //Sempre que deletar um item na grade, deve se tratar dos últimos itens para os primeiros (similar ao refazer as programações):
            var parada = dados.length - grid.itensSelecionados.length;
            for (var i = dados.length - 1; i >= parada && i >= 0; i--)
                if (!dados[i].id_aula_dada && !(hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado))
                    dados.splice(i, 1); // Remove o item do array
                else if (!dados[i].id_aula_dada) {
                    var l = i;
                    i += 1;
                    // Enquanto for um feriado, deleta eles:
                    while (l >= 0 && !dados[l].id_aula_dada && (hasValue(dados[l].cd_feriado) && !hasValue(dados[l].cd_feriado_desconsiderado) && !dados[l].id_feriado_desconsiderado)) {
                        dados.splice(l, 1); // Remove os últimos feriados da lista
                        l -= 1;

                        i -= 1;
                        parada -= 1;
                    }
                }
			
			//Remove os últimos feriados depois que finalizou:
            for (var i = dados.length - 1; i >= 0; i--)
                if (!dados[i].id_aula_dada && (hasValue(dados[i].cd_feriado) && !hasValue(dados[i].cd_feriado_desconsiderado) && !dados[i].id_feriado_desconsiderado)) {
                    if (binaryObjSearch(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId)) != null)
                        removeObjSort(grid.itensSelecionados, nomeId, eval('dados[i].' + nomeId));
                    dados.splice(i, 1); // Remove o item do array
                }
                else
                    break;
					
            grid.itensSelecionados = new Array();
            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
            grid.setStore(dataStore);
            grid.update();
        }
        else
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);

        atualizaFeriadosDesconsideradosComProgramacao();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criacaoGradeHorarioTurma(dataHorario, height) {
    require(["dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr", "dojox/json/ref"],
      function (ready, Calendar, Observable, Memory, declare, on, xhr, ref) {
          ready(function () {
              var tipoTurma = dojo.byId("tipoTurmaCad").value;
              xhr.get({
                  preventCache: true,
                  url: Endereco() + "/api/empresa/getHorarioFuncEmpresa",
                  handleAs: "json",
                  headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
              }).then(function (getHorarioFuncEscola) {
                  try {
                      getHorarioFuncEscola = jQuery.parseJSON(getHorarioFuncEscola);
                      var horaI = 0, horaF = 24;
                      var minI = 0, minF = 0;
                      if (hasValue(getHorarioFuncEscola) && hasValue(getHorarioFuncEscola.retorno)) {
                          if (hasValue(getHorarioFuncEscola.retorno.hr_inicial)) {
                              horaI = parseInt(getHorarioFuncEscola.retorno.hr_inicial.substring(0, 2));
                              minI = parseInt(getHorarioFuncEscola.retorno.hr_inicial.substring(3, 5));
                              minI = minI == 0 ? "00" : minI;
                          }
                          if (hasValue(getHorarioFuncEscola.retorno.hr_final)) {
                              horaF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(0, 2));
                              minF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(3, 5));
                              minF = minF == 0 ? "00" : minF;
                          }
                      }
                      dijit.byId("timeIni").constraints.min.setMinutes(minI);
                      dijit.byId("timeIni").constraints.min.setHours(horaI);
                      dijit.byId("timeIni").constraints.max.setHours(horaF);
                      dijit.byId("timeFim").constraints.max.setMinutes(minF);
                      dijit.byId("timeFim").constraints.min.setHours(horaI);
                      dijit.byId("timeFim").constraints.max.setHours(horaF);

                      //dijit.byId("timeIni").set("value", "T0" + horaI + ":" + minI + ":00");
                      //dijit.byId("timeFim").set("value", "T" + horaF + ":" + minF + ":00");

                      horaF = minF > 1 ? horaF + 1 : horaF;
                      var ECalendar = declare("extended.Calendar", Calendar, {

                          isItemResizeEnabled: function (item, rendererKind) {
                              return false || (hasValue(item) && item.cssClass == 'Calendar2');
                          },

                          isItemMoveEnabled: function (item, rendererKind) {
                              return false || (hasValue(item) && item.cssClass == 'Calendar2');
                          }
                      });
                      var dataHoje = new Date(2012, 0, 1);
                      //dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                      if (hasValue(dijit.byId("gridHorarios")))
                          destroyCreateGridHorario();
                      var calendar = new ECalendar({
                          date: dojo.date.locale.parse("01/07/2012", { locale: 'pt-br', formatLength: "short", selector: "date" }),
                          cssClassFunc: function (item) {
                              return item.calendar;
                          },
                          store: store = new Observable(new Memory({ data: dataHorario })),
                          dateInterval: "week",
                          columnViewProps: { minHours: horaI, maxHours: horaF, timeSlotDuration: 5, hourSize: 50, percentOverlap: 50 },
                          style: "position:relative;max-width:650px;width:100%;height:250px"
                      }, "gridHorarios");
                      calendar.novosItensEditados = [];
                      // Configura o calendario para poder excluir o item ao selecionar:

                      calendar.on("itemClick", function (item) {
                          try {
                              var calendarName = null;

                              if (hasValue(item) && hasValue(item.item))
                                  calendarName = item.item.calendar;
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });
                      calendar.on("change", function (item) {
                          try {
                              var calendarName = null;

                              if (hasValue(item) && hasValue(item.newValue))
                                  calendarName = item.newValue.calendar;
                              else
                                  if (hasValue(item) && hasValue(item.oldValue))
                                      calendarName = item.oldValue.calendar;
                          }
                          catch (e) {
                              postGerarLog(e);
                          }

                      });
                      calendar.on("itemeditbegin", function (item) {
                          addItemHorariosEdit(item.item.id, calendar);
                      });

                      calendar.on("itemRollOver", function (e) {
                          try {
                              var gridProfessores = dijit.byId("gridProfessor");
                              if (hasValue(gridProfessores) && gridProfessores.store.objectStore.data.length > 0)
                                  var storePorf = gridProfessores.store.objectStore.data;
                              var minutosEnd = e.item.endTime.getMinutes() > 9 ? e.item.endTime.getMinutes() : "0" + e.item.endTime.getMinutes();
                              var horasEnd = e.item.endTime.getHours() > 9 ? e.item.endTime.getHours() : "0" + e.item.endTime.getHours();
                              var horarioEnd = horasEnd + ":" + minutosEnd;
                              var minutosStart = e.item.startTime.getMinutes() > 9 ? e.item.startTime.getMinutes() : "0" + e.item.startTime.getMinutes();
                              var horasStart = e.item.startTime.getHours() > 9 ? e.item.startTime.getHours() : "0" + e.item.startTime.getHours();
                              var horarioStart = horasStart + ":" + minutosStart;
                              if (e.item.calendar == "Calendar2") {
                                  //new Tooltip({
                                  //    connectId: e.renderer.domNode.id,
                                  //    label: "Ocupado pela Turma  " + horarioStart + " as " + horarioEnd
                                  //});
                                  if (hasValue(e.item) && hasValue(e.item.HorariosProfessores) && e.item.HorariosProfessores.length > 0) {
                                      var nomesProf = "";
                                      $.each(e.item.HorariosProfessores, function (index, value) {
                                          if (hasValue(storePorf)) {
                                              $.each(storePorf, function (idx, val) {
                                                  if (value.cd_professor == val.cd_professor) {
                                                      nomesProf += "\n" + val.no_professor;
                                                      return false;
                                                  }
                                              });
                                          }
                                      });
                                      dojo.attr(e.renderer.domNode.id, "title", "Ocupado pela Turma  " + horarioStart + " as " + horarioEnd + "\n" + "Professores:" + nomesProf);
                                  } else
                                      dojo.attr(e.renderer.domNode.id, "title", "Ocupado pela Turma  " + horarioStart + " as " + horarioEnd);
                              }
                              if (e.item.calendar == "Calendar3")
                                  dojo.attr(e.renderer.domNode.id, "title", "Ocupado pela Sala  " + horarioStart + " as " + horarioEnd);
                              if (e.item.calendar == "Calendar4") {
                                  //dojo.attr(e.renderer.domNode.id, "title", "Ocupado pelo(s) Professor(e)  " + horarioStart + " as " + horarioEnd);
                                  if (hasValue(e.item) && hasValue(e.item.HorariosProfessores) && e.item.HorariosProfessores.length > 0) {
                                      var nomesProf = "";
                                      $.each(e.item.HorariosProfessores, function (index, value) {
                                          if (hasValue(storePorf)) {
                                              $.each(storePorf, function (idx, val) {
                                                  if (value.cd_professor == val.cd_professor) {
                                                      nomesProf += "\n" + val.no_professor;
                                                      return false;
                                                  }
                                              });
                                          }
                                      });
                                      dojo.attr(e.renderer.domNode.id, "title", "Ocupado pelo(s) Professor(es) " + horarioStart + " as " + horarioEnd + nomesProf);
                                  } else
                                      dojo.attr(e.renderer.domNode.id, "title", "Ocupado pelo(s) Professor(e)  " + horarioStart + " as " + horarioEnd);
                              }

                              dojo.byId('mostrarLabelHorario').value = true;
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });
                      // Configura o calendário para poder incluir um item no click da grade:

                      var createItem = function (view, d, e) {
                          try {
                              apresentaMensagem("apresentadorMensagemTurma", null);
                              dojo.byId('mostrarLabelHorario').value = false;
                              // create item by maintaining control key
                              //if(!e.ctrlKey ||e.shiftKey || e.altKey){ //Usado para criar o item no componente somente com o click do mouse + a tecla <ctrl> pressionada.
                              if (e.shiftKey || e.altKey) {
                                  return;
                              }

                              var dates = {
                                  convert: function (dto) {
                                      return (
                                          dto.constructor === Date ? dto :
                                          dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                                          dto.constructor === Number ? new Date(dto) :
                                          dto.constructor === String ? new Date(dto) :
                                          typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                                          NaN
                                      );
                                  },
                                  compare: function (a, b) {
                                      return (
                                          isFinite(a = this.convert(a).valueOf()) &&
                                          isFinite(b = this.convert(b).valueOf()) ?
                                          (a > b) - (a < b) :
                                          NaN
                                      );
                                  },
                                  inRange: function (dto, start, end) {
                                      return (
                                           isFinite(dto = this.convert(dto).valueOf()) &&
                                           isFinite(start = this.convert(start).valueOf()) &&
                                           isFinite(end = this.convert(end).valueOf()) ?
                                           start <= dto && dto <= end :
                                           NaN
                                       );
                                  },
                                  interception: function (start1, end1, start2, end2) {
                                      return (
                                           isFinite(a1 = this.convert(start1).valueOf()) &&
                                           isFinite(a2 = this.convert(end1).valueOf()) &&
                                           isFinite(b1 = this.convert(start2).valueOf()) &&
                                           isFinite(b2 = this.convert(end2).valueOf()) ?
                                           ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                                           NaN
                                       );
                                  },
                                  include: function (start1, end1, start2, end2) {
                                      return (
                                           isFinite(a1 = this.convert(start1).valueOf()) &&
                                           isFinite(a2 = this.convert(end1).valueOf()) &&
                                           isFinite(b1 = this.convert(start2).valueOf()) &&
                                           isFinite(b2 = this.convert(end2).valueOf()) ?
                                           ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                                           NaN
                                       );
                                  }
                              }

                              var start, end;
                              var colView = calendar.columnView;
                              var cal = calendar.dateModule;
                              var gradeHorarios = dijit.byId('gridHorarios');
                              var itens = dijit.byId('gridHorarios').items;
                              var id = gradeHorarios.items.length;
                              var item = null;

                              id = geradorIdHorarios(gradeHorarios);

                              if (view == colView) {
                                  start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                                  end = cal.add(start, "minute", colView.timeSlotDuration);;

                                  if (itens.length == 0) {
                                      var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                                      var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                                      var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                                      var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                                      item = {
                                          id: id,
                                          cd_horario: 0,
                                          summary: "",
                                          dt_hora_ini: hIni + ":" + mIni + ":00",
                                          dt_hora_fim: hFim + ":" + mFim + ":00",
                                          startTime: start,
                                          endTime: end,
                                          calendar: "Calendar2",
                                          id_dia_semana: start.getDate(),
                                          id_disponivel: true,
                                          cd_registro: 0,
                                          HorariosProfessores: new Array(),
                                          inserido: true
                                      };

                                  } else {

                                      for (var j = itens.length - 1; j >= 0; j--) {
                                          // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                                          if (itens[j].cssClass == "Calendar2" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                                              // Verifica se um item inclui totalmente o outro item e remove o incluído:
                                              var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);

                                              if (included == 1) {
                                                  resolveuIntersecao = true;
                                                  return false;
                                              }
                                              else
                                                  if (included == 2) {
                                                      removeHorario(calendar, itens[j].id);
                                                      dojo.byId('mostrarLabelHorario').value = true;
                                                      // Caso contrário, junta um item com o outro:
                                                  } else
                                                      if (included != NaN) {
                                                          if (dates.compare(start, itens[j].startTime) == 1)
                                                              start = itens[j].startTime;
                                                          else
                                                              end = itens[j].endTime;
                                                          removeHorario(calendar, itens[j].id);
                                                      }
                                          }
                                      }
                                      var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                                      var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                                      var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                                      var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                                      item = {
                                          id: id,
                                          cd_horario: 0,
                                          summary: "",
                                          dt_hora_ini: hIni + ":" + mIni + ":00",
                                          dt_hora_fim: hFim + ":" + mFim + ":00",
                                          startTime: start,
                                          endTime: end,
                                          calendar: "Calendar2",
                                          id_dia_semana: start.getDate(),
                                          id_disponivel: true,
                                          HorariosProfessores: new Array(),
                                          inserido: true
                                      };

                                  }
                                  if (tipoTurma == TURMA_PPT_FILHA) {
                                      //verifica se não esta fazendo merge com horarios ocupados pela turma PPT.
                                      if (!verificarIntersecaoHorariosPPTFilha(item, idComp, msg, VERIFTURMANORMAL))
                                          return false;
                                  } else
                                      //verifica se não esta fazendo merge com horarios ocupados professores e salas.
                                      if (!testarHorarioGrid(item))
                                          return false;
                                  setTimeout("removeHorarioEditado(" + item.id + ",'gridHorarios')", 1500);
                                  return item;
                              }
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      }

                      calendar.set("createOnGridClick", true);
                      calendar.set("createItemFunc", createItem);
                      //calendar.set("ItemEditEnd", ItemEditEnd);

                      dijit.byId("gridHorarios").on("ItemEditEnd", function (evt) {
                          try {
                              var inserido = false;
                              if (hasValue(evt.item.inserido) && evt.item.inserido)
                                  inserido = evt.item.inserido;
                              setTimeout(function () {
                                  mergeItemGridHorariosTurma(evt.item.id, evt.item.startTime, evt.item.endTime, evt.item.cd_horario, 'gridHorarios', 'apresentadorMensagemTurma', inserido, xhr, ref);
                              }, 100);
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });

                      var toolBar = document.getElementById('dijit_Toolbar_0');
                      $(".buttonContainer").css("display", "none");
                      $(".viewContainer").css("top", "5px");
                      if (hasValue(toolBar))
                          toolBar.style.display = 'none';
                  }
                  catch (e) {
                      postGerarLog(e);
                  }
              },
              function (error) {
                  apresentaMensagem('apresentadorMensagemTurma', error);
              });
          });
      });
}

function removeHorarioEditado(id, grid) {
    try {
        var gridHorarios = dijit.byId(grid);

        if (!hasValue(gridHorarios.novosItensEditados) || !binarySearch(gridHorarios.novosItensEditados, id))
            gridHorarios.store.remove(parseInt(id));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function removeHorario(gridHorarios, id) {
    try {
        gridHorarios.store.remove(id);
        if (hasValue(gridHorarios.novosItensEditados))
            gridHorarios.novosItensEditados = removeSort(gridHorarios.novosItensEditados, id);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridHorario() {
    try {
        if (hasValue(dijit.byId("gridHorarios"))) {
            dijit.byId("gridHorarios").destroyRecursive();
            $('<div>').attr('id', 'gridHorarios').appendTo('#paiGridHorarios');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateProgramacao() {
    try {

        itensSelecionadosCheckGrid = [];
        if (hasValue(dijit.byId("gridProgramacao"))) {
            dijit.byId("gridProgramacao").destroyRecursive();
            $('<div>').attr('id', 'gridProgramacao').attr('style', 'height:235px;').appendTo('#PaigridProgramacao');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateFeriadoDesconsiderado() {
    try {
        if (hasValue(dijit.byId("gridDesconsideraFeriados"))) {
            dijit.byId("gridDesconsideraFeriados").destroyRecursive();
            $('<div>').attr('id', 'gridDesconsideraFeriados').attr('style', 'height:235px;').appendTo('#paiDesconsideraFeriados');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirItemHorarioTurma(idComp, msg) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try {
            apresentaMensagem(msg, null);
            var mensagensWeb = new Array();
            var calendar = dijit.byId(idComp);
            var timeFim = dijit.byId('timeFim');
            var timeIni = dijit.byId('timeIni');
            var arraySemana = dijit.byId('cbDias').value;
            var cd_horario = 0;
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            if (!hasValue(dijit.byId(idComp))) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNaoConstruidoGradeHorario);
                apresentaMensagem(msg, mensagensWeb);
                return false
            }
            var itens = dijit.byId(idComp).items;
            if (arraySemana.length <= 0) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDiaSemanaSelect);
                apresentaMensagem(msg, mensagensWeb);
                return false
            }
            else
                //if (timeIni.validate() && timeIni.validate()) {
                if (dijit.byId("formSemana").validate()) {
                    if ((timeFim.value.getMinutes() % 5 != 0 || timeIni.value.getMinutes() % 5 != 0) || validarItemHorarioManual(timeIni, timeFim)) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotInclItemHorario);
                        apresentaMensagem(msg, mensagensWeb);
                        return false;
                    }
                    var dates = {
                        convert: function (dto) {
                            return (
                                dto.constructor === Date ? dto :
                                dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                                dto.constructor === Number ? new Date(dto) :
                                dto.constructor === String ? new Date(dto) :
                                typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                                NaN
                            );
                        },
                        compare: function (a, b) {
                            return (
                                isFinite(a = this.convert(a).valueOf()) &&
                                isFinite(b = this.convert(b).valueOf()) ?
                                (a > b) - (a < b) :
                                NaN
                            );
                        },
                        inRange: function (dto, start, end) {
                            return (
                                 isFinite(dto = this.convert(dto).valueOf()) &&
                                 isFinite(start = this.convert(start).valueOf()) &&
                                 isFinite(end = this.convert(end).valueOf()) ?
                                 start <= dto && dto <= end :
                                 NaN
                             );
                        },
                        interception: function (start1, end1, start2, end2) {
                            return (
                                 isFinite(a1 = this.convert(start1).valueOf()) &&
                                 isFinite(a2 = this.convert(end1).valueOf()) &&
                                 isFinite(b1 = this.convert(start2).valueOf()) &&
                                 isFinite(b2 = this.convert(end2).valueOf()) ?
                                 ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                                 NaN
                             );
                        },
                        include: function (start1, end1, start2, end2) {
                            return (
                                 isFinite(a1 = this.convert(start1).valueOf()) &&
                                 isFinite(a2 = this.convert(end1).valueOf()) &&
                                 isFinite(b1 = this.convert(start2).valueOf()) &&
                                 isFinite(b2 = this.convert(end2).valueOf()) ?
                                 ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                                 NaN
                             );
                        }
                    }
                    //Faz um clone do store da grade para voltar ao estado original, em caso de ocorrer um erro ao incluir vários horários:
                    calendar.bkpStore = cloneArray(calendar.store.data);
                    for (var i = 0; i < calendar.store.data.length; i++) {
                        calendar.bkpStore[i].startTime = calendar.store.data[i].startTime;
                        calendar.bkpStore[i].endTime = calendar.store.data[i].endTime;
                    }
                    var restaurou = false;
                    if (tipoTurma == TURMA_PPT) {
                        calendar.horariosAddLenth = arraySemana.length;
                        calendar.horariosAdd = new Array();
                    }
                    for (var i = 0; i < arraySemana.length; i++) {
                        var diaSemana = parseInt(arraySemana[i]) + 1;
                        var d = new Date(2012, 6, diaSemana, timeIni.value.getHours(), timeIni.value.getMinutes());
                        var start, end;
                        var colView = calendar.columnView;
                        var cal = calendar.dateModule;
                        var gradeHorarios = dijit.byId(idComp);
                        var resolveuIntersecao = false;
                        var dataHorarios = gradeHorarios.store.data;
                        var criarItemHorario = true;
                        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
                        var quantItensMerge = 0;
                        var arraycodHorariosItensMerge = [];
                        var arrayTotalItensMerge = [];
                        var posicaoArray = 0;

                        start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                        //start = d;
                        d = new Date(2012, 6, diaSemana, timeFim.value.getHours(), timeFim.value.getMinutes());
                        id = geradorIdHorarios(gradeHorarios);
                        var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                        var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                        var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                        var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                        var dataRegistro = new Date(start);
                        var diaRegistro = dataRegistro.getDate();

                        if (!resolveuIntersecao) {
                            var item = {
                                id: id,
                                cd_horario: cd_horario,
                                summary: "",
                                startTime: start,
                                calendar: "Calendar2",
                                endTime: d,
                                dt_hora_ini: hIni + ":" + mIni + ":00",
                                dt_hora_fim: hFim + ":" + mFim + ":00",
                                id_dia_semana: start.getDate(),
                                id_disponivel: true
                            };
                            addItemHorariosEdit(item.id, calendar);
                            calendar.store.add(item);
                            colView.set("startTimeOfDay", { hours: hIni, duration: 500 });
                        }

                        //Filtra a lista de itens para trazer os horários do dia inserido.
                        if (hasValue(dataHorarios) && dataHorarios.length > 0) {
                            dataHorarios = jQuery.grep(dataHorarios, function (value) {
                                return value.startTime.getDate() == diaRegistro && value.id != parseInt(item.id);
                            });
                        }

                        /****Verificar se existe interseção com um ou mais horários na grade:****/
                        for (var j = dataHorarios.length - 1; j >= 0; j--) {
                            // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                            if (dataHorarios[j].calendar == "Calendar2" && (dates.interception(start, d, dataHorarios[j].startTime, dataHorarios[j].endTime))) {
                                quantItensMerge += 1;
                                posicaoArray = j;
                                arrayTotalItensMerge.push(dataHorarios[j]);
                                if (hasValue(dataHorarios[j].cd_horario) && dataHorarios[j].cd_horario > 0)
                                    arraycodHorariosItensMerge.push(dataHorarios[j].cd_horario);
                            }
                        }

                        if (tipoTurma == TURMA_PPT_FILHA) {
                            //verifica se não esta fazendo merge com horarios ocupados pela turma PPT.
                            if (!verificarIntersecaoHorariosPPTFilha(item, idComp, msg, VERIFTURMANORMAL)) {
                                restauraCalendar(calendar);
                                restaurou = true;
                                return false;
                            }
                        } else
                            //verifica se não esta fazendo merge com horarios ocupados professores e salas.
                            if (!testarHorarioGrid(item)) {
                                restauraCalendar(calendar);
                                restaurou = true;
                                return false;
                            }

                        if (quantItensMerge == 1) {
                            criarItemHorario = false;

                            var cd_horario_item_merge = dataHorarios[posicaoArray].cd_horario;
                            var itemMerge = dataHorarios[posicaoArray];
                            if (parseInt(cd_turma) > 0) {
                                if (cd_horario_item_merge > 0 && cd_horario > 0)
                                    verificarDoisItensHorarioDisponibilidadeTurma(xhr, ref, item, gradeHorarios, dates, resolveuIntersecao,
                                                                                                           calendar, id, itemMerge, EDIT_HORARIO, true, tipoTurma, VERIFTURMANORMAL, msg, null);
                                else
                                    if (cd_horario_item_merge > 0) {
                                        if (id != null && id > 0)
                                            //calendar.store.remove(id);
                                            removeHorario(calendar, id);
                                        item.cd_horario = cd_horario_item_merge;
                                        verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates,
                                                                                                                 calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, true, tipoTurma, VERIFTURMANORMAL, msg, null);
                                    } else
                                        if (cd_horario > 0)
                                            verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates,
                                                                                                                  calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, true, tipoTurma, VERIFTURMANORMAL, msg, null);
                                        else {
                                            if (id != null && id > 0)
                                                //calendar.store.remove(id);
                                                removeHorario(calendar, id);
                                            resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFTURMANORMAL, xhr, tipoTurma, msg, null);
                                        }
                            } else {
                                if (id != null && id > 0)
                                    removeHorario(calendar, id);
                                resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFTURMANORMAL, xhr, tipoTurma, msg, null);
                            }
                        } else if (quantItensMerge > 1) {
                            criarItemHorario = false;
                            verificarSeexisteProgranacaoParaVariosHorarios(xhr, ref, item, calendar, arrayTotalItensMerge, true, VERIFTURMANORMAL);
                        }

                        //Verificação quando é alterado um item só.
                        if (criarItemHorario) {
                            verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, null, null, calendar, null, null, null, ADD_HORARIO, true, tipoTurma, VERIFTURMANORMAL, msg, NOVO);
                        }
                    }
                }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function restauraCalendar(calendar) {
    try {
        var newStore = new Array();

        for (var i = 0; hasValue(calendar.bkpStore) && i < calendar.bkpStore.length; i++) {
            var item = {
                id: calendar.bkpStore[i].id,
                cd_horario: calendar.bkpStore[i].cd_horario,
                summary: calendar.bkpStore[i].summary,
                startTime: calendar.bkpStore[i].startTime,
                calendar: calendar.bkpStore[i].calendar,
                endTime: calendar.bkpStore[i].endTime,
                dt_hora_ini: calendar.bkpStore[i].dt_hora_ini,
                dt_hora_fim: calendar.bkpStore[i].dt_hora_fim,
                id_dia_semana: calendar.bkpStore[i].id_dia_semana,
                id_disponivel: calendar.bkpStore[i].id_disponivel,
                no_datas: calendar.bkpStore[i].no_datas,
                dia_semana: calendar.bkpStore[i].dia_semana
            };
            newStore.push(item);
        }
        require(["dojo/store/Memory", "dojo/store/Observable"],
           function (Memory, Observable) {
               calendar.set('store', new Observable(new Memory({ data: newStore })));
           });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mergeItemGridHorariosTurma(id, startTime, endTime, cd_horario, idComp, msg, inserido, xhr, ref) {
    try {
        apresentaMensagem(msg, null);
        var calendar = dijit.byId(idComp);
        var arraySemana = [];
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var diaRegistro = new Date(startTime);
        var endDia = new Date(endTime);
        var inicioDia = diaRegistro.getDate();
        var fimDia = endDia.getDate();
        var diaRegistro = diaRegistro.getDate();
        var tipoRegistroMerge = EDIT;
        //Metodo verifica se o horário editado e de origem (inserção/alteração)
        if (inserido)
            tipoRegistroMerge = NOVO;

        if (inicioDia != fimDia) {
            if (!hasValue(cd_horario))
                removeHorario(calendar, id);
            else
                getHorarioOriginalEditado(cd_horario, id);
            return false;
        }

        var dates = {
            convert: function (dto) {
                return (
                    dto.constructor === Date ? dto :
                    dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                    dto.constructor === Number ? new Date(dto) :
                    dto.constructor === String ? new Date(dto) :
                    typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                    NaN
                );
            },
            compare: function (a, b) {
                return (
                    isFinite(a = this.convert(a).valueOf()) &&
                    isFinite(b = this.convert(b).valueOf()) ?
                    (a > b) - (a < b) :
                    NaN
                );
            },
            inRange: function (dto, start, end) {
                return (
                     isFinite(dto = this.convert(dto).valueOf()) &&
                     isFinite(start = this.convert(start).valueOf()) &&
                     isFinite(end = this.convert(end).valueOf()) ?
                     start <= dto && dto <= end :
                     NaN
                 );
            },
            interception: function (start1, end1, start2, end2) {
                return (
                     isFinite(a1 = this.convert(start1).valueOf()) &&
                     isFinite(a2 = this.convert(end1).valueOf()) &&
                     isFinite(b1 = this.convert(start2).valueOf()) &&
                     isFinite(b2 = this.convert(end2).valueOf()) ?
                     ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                     NaN
                 );
            },
            include: function (start1, end1, start2, end2) {
                return (
                     isFinite(a1 = this.convert(start1).valueOf()) &&
                     isFinite(a2 = this.convert(end1).valueOf()) &&
                     isFinite(b1 = this.convert(start2).valueOf()) &&
                     isFinite(b2 = this.convert(end2).valueOf()) ?
                     ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                     NaN
                 );
            }
        }
        var d;
        var start, end;
        var colView = calendar.columnView;
        var gradeHorarios = dijit.byId(idComp);
        var dataHorarios = gradeHorarios.store.data;
        var resolveuIntersecao = false;
        var criarItemHorario = true;
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var quantItensMerge = 0;
        var arraycodHorariosItensMerge = [];
        var arrayTotalItensMerge = [];
        var posicaoArray = 0;
        start = new Date(startTime);
        end = new Date(endTime);
        d = new Date(endTime);
        //cria o item para a verificação.
        var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
        var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
        var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
        var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
        var item = {
            id: id,
            cd_horario: parseInt(cd_horario),
            summary: "",
            startTime: start,
            calendar: "Calendar2",
            endTime: d,
            dt_hora_ini: hIni + ":" + mIni + ":00",
            dt_hora_fim: hFim + ":" + mFim + ":00",
            id_dia_semana: start.getDate(),
            id_disponivel: true
        };

        if (hasValue(dataHorarios) && dataHorarios.length > 0) {
            dataHorarios = jQuery.grep(dataHorarios, function (value) {
                return value.startTime.getDate() == diaRegistro && value.id != parseInt(id);
            });

        }

        /****Verificar se existe interseção com um ou mais horários disponíveis:****/
        for (var j = dataHorarios.length - 1; j >= 0; j--) {
            // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
            if (dataHorarios[j].calendar == "Calendar2" && (dates.interception(start, d, dataHorarios[j].startTime, dataHorarios[j].endTime))) {
                quantItensMerge += 1;
                posicaoArray = j;
                arrayTotalItensMerge.push(dataHorarios[j]);
                if (hasValue(dataHorarios[j].cd_horario) && dataHorarios[j].cd_horario > 0)
                    arraycodHorariosItensMerge.push(dataHorarios[j].cd_horario);
            }
        }
        if (tipoTurma == TURMA_PPT_FILHA) {
            //verifica se não esta fazendo merge com horarios ocupados pela turma PPT.
            if (!verificarIntersecaoHorariosPPTFilha(item, idComp, msg, VERIFTURMANORMAL))
                return false;
        } else
            //verifica se não esta fazendo merge com horarios ocupados professores e salas.
            if (!testarHorarioGrid(item))
                return false;

        if (quantItensMerge == 1) {
            criarItemHorario = false;

            var cd_horario_item_merge = dataHorarios[posicaoArray].cd_horario;
            var itemMerge = dataHorarios[posicaoArray];
            if (parseInt(cd_turma) > 0) {
                if (cd_horario_item_merge > 0 && cd_horario > 0)
                    verificarDoisItensHorarioDisponibilidadeTurma(xhr, ref, item, gradeHorarios, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, false, tipoTurma, VERIFTURMANORMAL, msg, null);
                else
                    if (cd_horario_item_merge > 0) {
                        if (id != null && id > 0)
                            //calendar.store.remove(id);
                            removeHorario(calendar, id);
                        item.cd_horario = cd_horario_item_merge;
                        verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates, calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, false, tipoTurma, VERIFTURMANORMAL, msg, null);
                    } else
                        if (cd_horario > 0)
                            verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates, calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, false, tipoTurma, VERIFTURMANORMAL, msg, EDIT);
                        else {
                            if (id != null && id > 0)
                                //calendar.store.remove(id);
                                removeHorario(calendar, id);
                            resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFTURMANORMAL, xhr, tipoTurma, msg, null);
                        }
            } else {
                if (id != null && id > 0)
                    removeHorario(calendar, id);
                resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, xhr, VERIFTURMANORMAL, tipoTurma, msg, null);
            }
        } else if (quantItensMerge > 1) {
            criarItemHorario = false;
            //if (!testarHorarioGrid(item))
            //    return false;
            verificarSeexisteProgranacaoParaVariosHorarios(xhr, ref, item, calendar, arrayTotalItensMerge, VERIFTURMANORMAL, null);
        }

        //Verificação quando é alterado um intem só.
        if (criarItemHorario) {
            //if (!testarHorarioGrid(item))
            //    return false;
            verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, null, null, calendar, null, null, null, ADD_HORARIO, false, tipoTurma, VERIFTURMANORMAL, msg, tipoRegistroMerge);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verifExisteProgHorarioEAlunosDisp(xhr, ref, item, validarProgramacao, resolveuIntersecao, dates, calendar, RESOLVERMERGE, id, itemMerge, tipoCrud, variasInclusoes, tipoTurma, tipoVerif, msg, tipoRegistroMerge) {
    try {
        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
            tipoTurma = TURMA_PPT_FILHA;
        var gridHorario = hasValue(dijit.byId("gridHorarios")) ? dijit.byId("gridHorarios") : [];
        var gridTurma = hasValue(dijit.byId('gridTurma')) ? dijit.byId(dijit.byId('gridTurma')) : [];
        var alunosProfVerifDispo = new AlunoProfessorTurma(item, validarProgramacao, tipoTurma, tipoVerif);

        xhr.post({
            url: Endereco() + "/api/turma/verifProgHorariosEAlunosDispHorarios",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(alunosProfVerifDispo)
        }).then(function (data) {
            try {
                retorno = data.retorno;
                if (id != null && id > 0)
                    //calendar.store.remove(id);
                    removeHorario(calendar, id);
                resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, tipoCrud, tipoVerif, xhr, tipoTurma, msg, tipoRegistroMerge);
                return false;
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            var retornoMetode = true;
            if (variasInclusoes)
                restauraCalendar(calendar);
            else {
                if (itemMerge != null && itemMerge.cd_horario > 0)
                    //gridHorario.store.remove(itemMerge.id);
                    removeHorario(calendar, itemMerge.id);
                if (item.cd_horario == 0)
                    //gridHorario.store.remove(item.id);
                    removeHorario(calendar, item.id);
                else {
                    if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                        getHorarioOriginalEditadoFilhaPPTModal(item.cd_horario, item.id);
                    else
                        getHorarioOriginalEditado(item.cd_horario, item.id);
                }
                retornoMetode = false;
            }
            apresentaMensagem(msg, error);

            return retornoMetode;
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function verificarSeexisteProgranacaoParaVariosHorarios(xhr, ref, item, calendar, arrayTotalItensMerge, variasInclusoes, tipoVerif) {
    var gridTurma = hasValue(dijit.byId('gridTurma')) ? dijit.byId(dijit.byId('gridTurma')) : [];
    //Aluno
    var horariosProf = new HorariosProgramacao(arrayTotalItensMerge);
    xhr.post({
        url: Endereco() + "/api/turma/verifProgHorariosEAlunosDispHorarios",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: ref.toJson(horariosProf)
    }).then(function (data) {
        try {
            retorno = data.retorno;

            for (var j = 0; j < arrayTotalItensMerge.length; j++)
                calendar.store.remove(arrayTotalItensMerge[j].id);
            //Verifica se é preciso atualizar as programações da turma:
            verificaProgramacaoTurma(null, xhr, null, TIPO_REFAZER_PROGRAMACOES_HORARIO, null);

            return false;
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        try {
            var retorno = true;

            if (variasInclusoes)
                restauraCalendar(calendar);
            else {
                if (item.cd_horario == 0)
                    //calendar.store.remove(item.id);
                    removeHorario(calendar, item.id);
                else {
                    if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                        getHorarioOriginalEditadoFilhaPPTModal(item.cd_horario, item.id);
                    else
                        getHorarioOriginalEditado(item.cd_horario, item.id);
                }
                retorno = false;
            }
            apresentaMensagem("apresentadorMensagemTurma", error);

            return retorno;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, tipoCrud, tipoVerif, xhr, tipoTurma, msg, tipoRegistroMerge) {
    try {
        if (tipoCrud == EDIT_HORARIO) {
            var included = dates.include(item.startTime, item.endTime, itemMerge.startTime, itemMerge.endTime);

            item.cd_horario = item.cd_horario <= 0 ? itemMerge.cd_horario : item.cd_horario;

            if (included == 1) {
                resolveuIntersecao = true;
                removeHorario(calendar, item.id);
            } else
                if (included == 2)
                    //calendar.store.remove(itemMerge.id);
                    removeHorario(calendar, itemMerge.id);
                    // Caso contrário, junta um item com o outro:
                else
                    if (included != NaN) {
                        if (dates.compare(item.startTime, itemMerge.startTime) == 1)
                            item.startTime = itemMerge.startTime;
                        else
                            item.endTime = itemMerge.endTime;
                        //calendar.store.remove(itemMerge.id);
                        removeHorario(calendar, itemMerge.id);
                    }
            if (!resolveuIntersecao) {
                item.id = geradorIdHorarios(calendar);
                addItemHorariosEdit(item.id, calendar);
                calendar.store.add(item);
            }
        } else {
            $.each(calendar.store.data, function (index, value) {
                if (value.id == item.id) {
                    var hIni = value.startTime.getHours() > 9 ? value.startTime.getHours() : "0" + value.startTime.getHours();
                    var hFim = value.endTime.getHours() > 9 ? value.endTime.getHours() : "0" + value.endTime.getHours();
                    var mFim = value.endTime.getMinutes() > 9 ? value.endTime.getMinutes() : "0" + value.endTime.getMinutes();
                    var mIni = value.startTime.getMinutes() > 9 ? value.startTime.getMinutes() : "0" + value.startTime.getMinutes();
                    value.id_dia_semana = item.id_dia_semana;
                    value.dt_hora_ini = hIni + ":" + mIni + ":00";
                    value.dt_hora_fim = hFim + ":" + mFim + ":00";
                    value.inserido = false;
                }
            });
        }

        deletarProfessoresHorariosTurma(tipoTurma, item, tipoVerif, msg);
        if (hasValue(calendar.horariosAddLenth)) {
            calendar.horariosAddLenth -= 1;
            if (calendar.horariosAddLenth == 0) {
                if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL) {
                    calendar.horariosAdd.push(item);
                    //Verifica se é preciso atualizar as programações da turma e atualiza o horário editado ou novo. (turmas Filhas).
                    refazerProgramacaoTurmasFilhas(msg, calendar.horariosAdd, xhr, tipoCrud, true);
                }
                if (tipoTurma != TURMA_PPT && tipoRegistroMerge == NOVO)
                    verificaProgramacaoTurma(null, xhr, null, ADD_HORARIO, null);
            } else
                calendar.horariosAdd.push(item);
        } else
            if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL && tipoRegistroMerge == NOVO)
                //Verifica se é preciso atualizar as programações da turma e atualiza o horário editado ou novo (turmas Filhas).
                refazerProgramacaoTurmasFilhas(msg, item, xhr, tipoCrud, false);
        //Verifica se é preciso atualizar as programações da turma:
        if (tipoTurma != TURMA_PPT)
            verificaProgramacaoTurma(null, xhr, null, TIPO_REFAZER_PROGRAMACOES_HORARIO, null);
    }
    catch (e) {
        postGerarLog(e);
    }
}
//obsoleto
function verificarDoisItensHorarioDisponibilidadeTurma(xhr, ref, item, gridHorario, dates, resolveuIntersecao, calendar, id, itemMerge, tipoCrud, variasInclusoes, tipoTurma, msg, tipoRegistroMerge) {
    var gridHorario = hasValue(dijit.byId("gridHorarios")) ? dijit.byId("gridHorarios") : [];
    var gridTurma = hasValue(dijit.byId('gridTurma')) ? dijit.byId(dijit.byId('gridTurma')) : [];
    //Aluno
    var alunosProfVerifDispo = new AlunoProfessorTurma(item, true, tipoTurma);
    xhr.post({
        url: Endereco() + "/api/turma/verifProgHorariosEAlunosDispHorarios",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: ref.toJson(alunosProfVerifDispo)
    }).then(function (data) {
        try {
            var alunosProfVerifDispo2 = new AlunoProfessorTurma(itemMerge, true, tipoTurma);
            xhr.post({
                url: Endereco() + "/api/turma/verifProgHorariosEAlunosDispHorarios",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(alunosProfVerifDispo2)
            }).then(function (data) {
                try {
                    if (id != null && id > 0)
                        //calendar.store.remove(id);
                        removeHorario(calendar, id);
                    resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, tipoCrud, xhr, tipoTurma, msg, tipoRegistroMerge);

                    return false;
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
             function (error) {
                 try {
                     var retorno = true;

                     if (variasInclusoes)
                         restauraCalendar(calendar);
                     else {
                         if (item.cd_horario == 0)
                             //gridHorario.store.remove(item.id);
                             removeHorario(gridHorario, item.id);
                         else {
                             if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                                 getHorarioOriginalEditadoFilhaPPTModal(item.cd_horario, item.id);
                             else
                                 getHorarioOriginalEditado(item.cd_horario, item.id);
                         }
                         retorno = false;
                     }
                     apresentaMensagem("apresentadorMensagemTurma", error);

                     return retorno;
                 }
                 catch (e) {
                     postGerarLog(e);
                 }
             });
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        try {
            var retorno = true;
            if (variasInclusoes)
                restauraCalendar(calendar);
            else {
                //if (itemMerge.cd_horario > 0) gridHorario.store.remove(itemMerge.id);
                if (item.cd_horario == 0)
                    //gridHorario.store.remove(item.id);
                    removeHorario(gridHorario, item.id);
                else {
                    if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                        getHorarioOriginalEditadoFilhaPPTModal(item.cd_horario, item.id);
                    else
                        getHorarioOriginalEditado(item.cd_horario, item.id);
                }
                retorno = false;
            }
            apresentaMensagem("apresentadorMensagemTurma", error);
            return retorno;
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function excluiItemHorarioTurma(idComp, msg, tipoVerif, xhr) {
    try {
        apresentaMensagem(msg, mensagensWeb);
        var gradeHorarios = dijit.byId(idComp);
        if (hasValue(gradeHorarios)) {
            var store = gradeHorarios.items;
            var itemSelected = new Object;
            var retorno = true;
            var cd_turma = null;
            var cd_turma_ppt = null;
            var cadastroTurmaPPT = false;
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
            if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL)
                cadastroTurmaPPT = true;
            if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL) {
                var gridAlunosPPT = dijit.byId("gridAlunosPPT");
                if (hasValue(gridAlunosPPT))
                    cd_turma = gridAlunosPPT.itensSelecionados[0].cd_turma;
            }

            var mensagensWeb = new Array();
            //var verificacao = verificarHorarioAlunoProfessorTurma(tipoVerif, msg);
            //if (verificacao)
            if (hasValue(gradeHorarios.selectedItems)) {
                for (var i = gradeHorarios.selectedItems.length - 1; i >= 0; i--) {
                    itemSelected = gradeHorarios.selectedItems[i];
                    if (itemSelected.calendar == "Calendar2") {
                        if (parseInt(cd_turma) > 0) {
                            xhr.get({
                                preventCache: true,
                                url: Endereco() + "/api/turma/existeAulaEfetivadaTurma?cd_turma=" + cd_turma + "&turma_PPT=" + cadastroTurmaPPT,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                try {
                                    retorno = data.retorno;
                                    if (retorno) {
                                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDeleteUpdateHorarioTurma);
                                        apresentaMensagem(msg, mensagensWeb);
                                    } else {
                                        removeHorario(gradeHorarios, itemSelected.id);
                                        deletarProfessoresHorariosTurma(tipoTurma, itemSelected, tipoVerif, msg);
                                        //Verifica se é preciso atualizar as programações da turma:
                                        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL)
                                            //Refaz as programações das turmas filhas:
                                            refazerProgramacaoTurmasFilhas(msg, itemSelected, xhr, DELETE_HORARIO, false);
                                        else
                                            verificaProgramacaoTurma(null, xhr, null, TIPO_REFAZER_PROGRAMACOES_HORARIO, null);
                                        verificarRemoverHorarioProfessorTurma(itemSelected, tipoTurma, tipoVerif, msg);
                                    }
                                    return retorno;
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem(msg, error);
                            });
                        } else {
                            removeHorario(gradeHorarios, itemSelected.id);
                            deletarProfessoresHorariosTurma(tipoTurma, itemSelected, tipoVerif, msg);
                            verificarRemoverHorarioProfessorTurma(itemSelected, tipoTurma, tipoVerif, msg);
                        }
                    }
                }
            }//if verificacao && hasValue(gradeHorarios.selectedItems)
            else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDeleteHorarioSeleciona);
                apresentaMensagem(msg, mensagensWeb);
                return false
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//Obsolete
function verificarRemoverHorarioProfessorTurma(itemSelected, tipoTurma, tipoVerif, msg) {
    if ((tipoTurma != TURMA_PPT) || (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)) {
        var mostrarMensagem = false;
        var qtdProf = 0;
        var horariosProf = itemSelected.HorariosProfessores;
        var compProfTurma = dijit.byId("gridProfessor");
        if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
            compProfTurma = dijit.byId('gridProfessorPPT');

        if (horariosProf != null && horariosProf.length > 0 && compProfTurma.store.objectStore.data != null && compProfTurma.store.objectStore.data.length > 0) {
            var mesagemProf = "";
            quickSortObj(horariosProf, 'cd_professor');
            quickSortObj(compProfTurma.store.objectStore.data, 'cd_professor');

            $.each(horariosProf, function (index, value) {
                var posicaoP = binaryObjSearch(compProfTurma.store.objectStore.data, 'cd_professor', value.cd_professor);
                if (hasValue(posicaoP, true)) {
                    var horarios = new Array();
                    if (hasValue(compProfTurma.store.objectStore.data[posicaoP])) {
                        quickSortObj(compProfTurma.store.objectStore.data[posicaoP].horarios, 'id');
                        var posicaoH = binaryObjSearch(compProfTurma.store.objectStore.data[posicaoP].horarios, 'id', itemSelected.id);
                        if (hasValue(posicaoH, true))
                            compProfTurma.store.objectStore.data[posicaoP].horarios.splice(posicaoH, 1);
                    }
                }
            });
            var cloneProf = cloneArray(compProfTurma.store.objectStore.data);
            $.each(cloneProf, function (index, value) {
                if (!hasValue(value.horarios) || value.horarios.length <= 0) {
                    mesagemProf += value.no_professor + ", "
                    mostrarMensagem = true;
                    var posicaoP = binaryObjSearch(compProfTurma.store.objectStore.data, 'cd_professor', value.cd_professor);
                    if (hasValue(posicaoP, true))
                        compProfTurma.store.objectStore.data.splice(posicaoP, 1);
                    qtdProf += 1;
                }
            });
            if (mostrarMensagem) {
                if (qtdProf == 1) {
                    mesagemProf = mesagemProf.substring(2, mesagemProf.length - 2);
                    mesagemProf = mesagemProf + " ";
                }
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, "O(s) professor(es) " + mesagemProf + "foi(ram) excluído(s) da turma por não existir horário alocado a ele(s)");
                apresentaMensagem(msg, mensagensWeb);
            }
        }
    }
}

function addItemHorariosEdit(id, calendar) {
    try {
        var novosItensEditados = hasValue(calendar.novosItensEditados) ? calendar.novosItensEditados : new Array();

        //novosItensEditados.push(item.item.id);
        novosItensEditados = insertSort(novosItensEditados, id, false);
        calendar.novosItensEditados = novosItensEditados;
    }
    catch (e) {
        postGerarLog(e);
    }
}


function popularGridEscolaTurma() {
    try {
        var turma = hasValue(dojo.byId('cd_turma').value) ? dojo.byId('cd_turma').value : 0;
        var gridEscolas = dijit.byId('gridEscolas');
        var popular = !eval(dojo.byId('abriuEscola').value);
        if (gridEscolas != null && gridEscolas.store != null && gridEscolas.store.objectStore.data.length == 0 && popular) {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getTurmasEscolatWithTurma?cd_turma=" + turma,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    apresentaMensagem('apresentadorMensagemTurma', data);
                    var empresas = data.retorno;
                    gridEscolas.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: empresas }) }));
                    gridEscolas.update();
                    showCarregando();
                }
                catch (er) {
                    postGerarLog(er);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemTurma', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarEscolasTurmaGrid() {
    try {
        var gridEscolas = dijit.byId('gridEscolas');
        var listaEscolas = [];
        if (hasValue(gridEscolas)) {
            if (gridEscolas.store != null && gridEscolas.store.objectStore != null && gridEscolas.store.objectStore.data.length > 0)
                listaEscolas = [];
            if (hasValue(gridEscolas) && gridEscolas.store.objectStore.data.length > 0) {
                var escolas = gridEscolas.store.objectStore.data;
                for (var i = 0; i < escolas.length; i++) {
                    listaEscolas.push({
                        cd_turma_escola: escolas[i].cd_turma_escola,
                        cd_turma: escolas[i].cd_turma,
                        cd_escola: escolas[i].cd_escola,
                        dc_reduzido_pessoa: escolas[i].dc_reduzido_pessoa
                    });
                }
            }
        }
        return listaEscolas;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mountDataTurmaForPost() {
    try {
        var horarios = null;
        var alunosTurma = null;
        var alunosTurmaPPT = null;
        var alunosTurmaEscola = null;
        var professorTurma = [];
        var progTurma = null
        var feriadosDesconsiderados = null
        var atualizarHorarios = false;
        var gridHorarios = dijit.byId("gridHorarios");
        var gridAlunosComp = dijit.byId("gridAlunos");
        var gridProfessor = dijit.byId("gridProfessor");
        var gridAlunosPPT = dijit.byId("gridAlunosPPT");
        var gridProgramacaoTurma = dijit.byId("gridProgramacao");
        var gridDesconsideraFeriados = dijit.byId("gridDesconsideraFeriados");
        var cd_turma = dojo.byId("cd_turma").value;
        var turmaEscola = montarEscolasTurmaGrid();
        this.hasClickEscola = dojo.byId('abriuEscola').value;

        if (document.getElementById("idTurmaPPTPai").checked && hasValue(gridAlunosPPT) && gridAlunosPPT.listaCompletaAlunos != null) {
            for (var i = 0; i < gridAlunosPPT.listaCompletaAlunos.length; i++) {
                if (hasValue(gridAlunosPPT.listaCompletaAlunos[i].alunoTurma)) {
                    gridAlunosPPT.listaCompletaAlunos[i].cd_situacao_aluno_turma = gridAlunosPPT.listaCompletaAlunos[i].alunoTurma.cd_situacao_aluno_turma;
                }
            }
            alunosTurmaPPT = gridAlunosPPT.listaCompletaAlunos;
            if (gridAlunosPPT.listaCompletaAlunosEsc != null)
                alunosTurmaEscola = gridAlunosPPT.listaCompletaAlunosEsc;
        }
        if (!document.getElementById("idTurmaPPTPai").checked && hasValue(gridAlunosComp) ) {
            if (gridAlunosComp.listaCompletaAlunos != null)
                alunosTurma = gridAlunosComp.listaCompletaAlunos;
            if (gridAlunosComp.listaCompletaAlunosEsc != null)
                alunosTurmaEscola = gridAlunosComp.listaCompletaAlunosEsc;
        }
        if (hasValue(gridProgramacaoTurma) && gridProgramacaoTurma.store.objectStore.data != null) {
            progTurma = gridProgramacaoTurma.store.objectStore.data;
            for (var i = 0; i < progTurma.length; i++) {
                var index = "id_mostrar_calendario_Selected_" + progTurma[i].nm_aula_programacao_turma;
                progTurma[i].dta_programacao_turma = dojo.date.locale.parse(progTurma[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' });  

                if (!hasValue(progTurma[i].id_mostrar_calendario)) // id_mostrar_calendario pode vir 'undefined'
                    progTurma[i].id_mostrar_calendario = false;

                if (hasValue(dijit.byId(index))){
                    if (progTurma[i].id_mostrar_calendario != dijit.byId(index).get("checked")) {
                        progTurma[i].id_mostrar_calendario = dijit.byId(index).get("checked");
                        progTurma[i].programacaoTurmaEdit = true;
                    }
                }
            }
        }

        if (hasValue(gridDesconsideraFeriados) && gridDesconsideraFeriados.store.objectStore.data != null) {
            feriadosDesconsiderados = gridDesconsideraFeriados.store.objectStore.data;
            for (var i = 0; i < feriadosDesconsiderados.length; i++) {
                feriadosDesconsiderados[i].dt_inicial = dojo.date.locale.parse(feriadosDesconsiderados[i].dta_inicial, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                feriadosDesconsiderados[i].dt_final = dojo.date.locale.parse(feriadosDesconsiderados[i].dta_final, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            }
        }

        dojo.ready(function () {
            try {
                if (gridProfessor.store.objectStore.data != null && gridProfessor.store.objectStore.data.length > 0) {
                    professorTurma = gridProfessor.store.objectStore.data;
                }
                //horarios = validaRetornaHorarios();// getHorariosOcupadosTurma(gridHorarios.params.store.data);

                if (hasValue(gridHorarios)) {
                    //atualizarHorarios = true;
                    if (gridHorarios.items.length > 0)
                        horarios = mountHorariosOcupadosTurma(gridHorarios.params.store.data);
                }
                else {
                    if (cd_turma <= 0)
                        horarios = mountHorariosOcupadosTurma(validaRetornaHorarios(dojo.byId("tipoTurmaCad").value));
                    else
                        horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);
                    //if (horarios != null)
                    //    horarios = mountHorariosOcupadosTurma(horarios);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });
        //var alunosTurma;

        var retorno = {
            cd_pessoa_escola: dojo.byId('escolaturma').value,
            cd_turma: dojo.byId("cd_turma").value,
            cd_turma_enc: dojo.byId("cd_turma_enc").value > 0 ? dojo.byId("cd_turma_enc").value : null,
            cd_turma_ppt: hasValue(dojo.byId("cd_turma_ppt").value) && dojo.byId("cd_turma_ppt").value > 0
                ? dojo.byId("cd_turma_ppt").value
                : null,
            no_turma: dojo.byId("noTurma").value,
            no_apelido: dojo.byId("noApelido").value,
            cd_curso: hasValue(dijit.byId("pesCadCurso").value) ? dijit.byId("pesCadCurso").value : null,
            cd_sala: hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : null,
            cd_sala_online: hasValue(dijit.byId("cbSalaOnLine").value) ? dijit.byId("cbSalaOnLine").value : null,
            cd_duracao: hasValue(dijit.byId("pesCadDuracao").value) ? dijit.byId("pesCadDuracao").value : null,
            cd_regime: hasValue(dijit.byId("pesCadRegime").value) ? dijit.byId("pesCadRegime").value : null,
            dt_inicio_aula: hasValue(dojo.byId("dtIniAula").value)
                ? dojo.date.locale.parse(dojo.byId("dtIniAula").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                : null,
            dt_final_aula: hasValue(dojo.byId("dtFimAula").value)
                ? dojo.date.locale.parse(dojo.byId("dtFimAula").value,
                    { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                : null,
            id_aula_externa: dijit.byId("idAulaExterna").get("checked"),
            nro_aulas_programadas: dojo.byId("nrAulasProg").value,
            id_turma_ppt: dijit.byId("idTurmaPPTPai").get("checked"),
            cd_produto: hasValue(dijit.byId("pesCadProduto").value) ? dijit.byId("pesCadProduto").value : null,
            horariosTurma: horarios,
            alunosTurma: alunosTurma,
            alunosTurmasPPT: alunosTurmaPPT,
            alunosTurmaEscola: alunosTurmaEscola,
            ProgramacaoTurma: progTurma,
            FeriadosDesconsiderados: feriadosDesconsiderados,
            atualizaHorarios: atualizarHorarios,
            ProfessorTurma: professorTurma,
            id_turma_ativa: dijit.byId("idTurmaPPTPai").get("checked") ? dijit.byId("idTurmaAtiva").get("checked") : true,
            TurmaEscola: turmaEscola,
            hasClickEscola: hasClickEscola
    }
        //horariosTurma
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadHorarioTurma(cd_turma, cd_sala, cd_professores, tipoTurma) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try {
            xhr.post({
                url: Endereco() + "/api/escola/postTodosHorariosVinculosTurma",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson({
                    cd_turma: parseInt(cd_turma),
                    cd_sala: parseInt(cd_sala),
                    professores: cd_professores,
                    dt_inicio: dijit.byId('dtIniAula').get('value'),
                    dt_final: dijit.byId('dtFimAula').get('value'),
                    cd_duracao: dijit.byId('pesCadDuracao').value,
                    cd_curso: hasValue(dojo.byId('pesCadCurso').value) ? dijit.byId('pesCadCurso').value : 0
                })
            }).then(function (dataHorario) {
                try {
                    dataHorario = jQuery.parseJSON(dataHorario);
                    var menorHorario = null;
                    var menorData = null;
                    var turma = dijit.byId("gridTurma");
                    var listaCompeltaHorariosTurma = [];
                    var codMaiorHorarioTurma = 0;
                    if (hasValue(dataHorario.retorno) && dataHorario.retorno.horairosOcupSala != null && dataHorario.retorno.horairosOcupSala.length > 0)
                        $.each(dataHorario.retorno.horairosOcupSala, function (idx, val) {
                            codMaiorHorarioTurma += 1;
                            val.calendar = "Calendar3";
                            val.id = codMaiorHorarioTurma;
                            val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            listaCompeltaHorariosTurma.push(val);
                        });
                    if (hasValue(dataHorario.retorno) && dataHorario.retorno.horariosOcupProf != null && dataHorario.retorno.horariosOcupProf.length > 0)
                        $.each(dataHorario.retorno.horariosOcupProf, function (idx, val) {
                            codMaiorHorarioTurma += 1;
                            val.calendar = "Calendar4";
                            val.id = codMaiorHorarioTurma;
                            val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            listaCompeltaHorariosTurma.push(val);
                        });
                    if (hasValue(turma.TurmaEdit) && (turma.TurmaEdit.horariosTurma != null && turma.TurmaEdit.horariosTurma.length > 0)) {
                        $.each(turma.TurmaEdit.horariosTurma, function (idx, val) {
                            codMaiorHorarioTurma += 1;

                            // Atualiza os ids na grid de Professores da primeira aba:
                            var dadosProfessor = dijit.byId('gridProfessor').store.objectStore.data;

                            for (var i = 0; i < dadosProfessor.length; i++)
                                for (var j = 0; j < dadosProfessor[i].horarios.length; j++)
                                    if (dadosProfessor[i].horarios[j].id == val.id)
                                        dadosProfessor[i].horarios[j].id = codMaiorHorarioTurma;
                            val.id = codMaiorHorarioTurma;
                            val.calendar = "Calendar2";
                            val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                            listaCompeltaHorariosTurma.push(val);

                            //Pega o menor horário:
                            if (menorData == null || val.dt_hora_ini < menorHorario) {
                                menorData = val.startTime;
                                menorHorario = val.dt_hora_ini;
                            }
                        });

                        //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                        setTimeout(function () { posicionaPrimeiraLinhaHorarioTurma('gridHorarios', listaCompeltaHorariosTurma.length, menorData); }, 100);
                    }
                    criacaoGradeHorarioTurma(listaCompeltaHorariosTurma);
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemTurma', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function posicionaPrimeiraLinhaHorarioTurma(nameGridHorarios, tamanhoHorarios, menorData) {
    try {
        var gridHorarios = dijit.byId(nameGridHorarios);
        try {
            if (hasValue(gridHorarios) && hasValue(gridHorarios.items) && gridHorarios.items.length >= tamanhoHorarios)
                gridHorarios.columnView.set("startTimeOfDay", { hours: menorData.getHours(), duration: 500 });
            else
                setTimeout(function () { posicionaPrimeiraLinhaHorarioTurma(nameGridHorarios, tamanhoHorarios, menorData); }, 100);
        }
        catch (e) {
            setTimeout(function () { posicionaPrimeiraLinhaHorarioTurma(nameGridHorarios, tamanhoHorarios, menorData); }, 100);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getHorariosOcupadosTurma(dataHorarios) {
    try {
        var retorno = [];
        $.each(dataHorarios, function (index, value) {
            if (value.calendar == "Calendar2") {
                var item = {
                    id: value.id,
                    cd_horario: parseInt(value.cd_horario),
                    summary: "",
                    startTime: value.startTime,
                    calendar: value.calendar,
                    endTime: value.endTime,
                    dt_hora_ini: value.dt_hora_ini,
                    dt_hora_fim: value.dt_hora_fim,
                    id_dia_semana: value.id_dia_semana,
                    id_disponivel: value.id_disponivel,
                    HorariosProfessores: hasValue(value.HorariosProfessores) ? value.HorariosProfessores.slice() : new Array
                };
                retorno.push(item);
            }
        });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mountHorariosOcupadosTurma(dataHorarios) {
    try {
        var retorno = [];
        $.each(dataHorarios, function (index, value) {
            if (value.calendar == "Calendar2") {
                if (!hasValue(value.removeTimeZone)) {
                    value.endTime = removeTimeZone(value.endTime);
                    value.startTime = removeTimeZone(value.startTime);
                    value.removeTimeZone = true;
                }
                retorno.push(value);
            }
        });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainerTurma");
        var pane = dijit.byId("tabPrincipalTurma");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTodosHorarios() {
    try {
        var gradeHorarios = dijit.byId('gridHorarios');
        if (hasValue(gradeHorarios)) {
            var store = gradeHorarios.items;
            if (store.length > 0)
                for (var i = store.length - 1; i >= 0; i--)
                    if (gradeHorarios.items[i].id >= 0)
                        gradeHorarios.store.remove(store[i].id);
            gradeHorarios.novosItensEditados = new Array();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparNewsHorarios() {
    try {
        var gradeHorarios = dijit.byId('gridHorarios');
        if (hasValue(gradeHorarios)) {
            var store = gradeHorarios.items;
            if (store.length > 0)
                for (var i = store.length - 1; i >= 0; i--)
                    if (!hasValue(gradeHorarios.store.data[i].cd_horario) && !gradeHorarios.store.data[i].cd_horario > 0)
                        //gradeHorarios.store.remove(store[i].id);
                        removeHorario(gradeHorarios, store[i].id);
            gradeHorarios.novosItensEditados = new Array();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoFKTurma() {
    try {
        var gridAlunos = null;
        var valido = true;
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        //Inseri aluno em uma turma normal.
        if (dojo.byId("setValuePesquisaAlunoFK").value == PESQUISAALUNOCAD) {
            gridAlunos = dijit.byId("gridAlunos");
            if (dojo.byId("tipoTurmaCad").value != TURMA_PPT_FILHA) {
                if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    valido = false;
                } else {
                    var storeGridAlunos = (hasValue(gridAlunos) && hasValue(gridAlunos.store.objectStore.data)) ? gridAlunos.store.objectStore.data : [];
                    if (storeGridAlunos != null && storeGridAlunos.length > 0) {
                        $.each(gridPesquisaAluno.itensSelecionados, function (idx, value) {
                            var newAluno = {
                                cd_aluno_turma: 0,
                                cd_turma: 0,
                                cd_aluno: value.cd_aluno,
                                no_aluno: value.no_pessoa,
                                cd_pessoa_aluno: value.cd_pessoa_aluno,
                                cd_situacao_aluno_turma: 9,
                                situacaoAlunoTurma: msgAguardandoMat,
                                id_ativo: value.id_ativo,
                                cd_pessoa: value.cd_pessoa_dependente,
                                no_pessoa: value.no_pessoa_dependente,
                                pc_bolsa: value.pc_bolsa,
                                dt_inicio_bolsa: value.dt_inicio_bolsa,
                                vl_abatimento_matricula : value.vl_abatimento_matricula,
                                PessoaSGFQueUsoOCpf: value.PessoaSGFQueUsoOCpf,
                                dc_reduzido_pessoa_escola: value.dc_reduzido_pessoa_escola,
                                cd_pessoa_escola_aluno: value.cd_pessoa_escola_aluno
                            };
                            insertObjSort(gridAlunos.store.objectStore.data, "cd_aluno", newAluno);
                            insertObjSort(gridAlunos.listaCompletaAlunos, "cd_aluno", newAluno);
                        });
                        gridAlunos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: storeGridAlunos, idProperty: "cd_aluno" }) }));
                    } else {
                        var dados = [];
                        $.each(gridPesquisaAluno.itensSelecionados, function (index, val) {
                            var newAluno = {
                                cd_aluno_turma: 0, cd_turma: 0,
                                cd_aluno: val.cd_aluno,
                                no_aluno: val.no_pessoa,
                                cd_pessoa_aluno: val.cd_pessoa_aluno,
                                cd_situacao_aluno_turma: 9,
                                situacaoAlunoTurma: msgAguardandoMat,
                                id_ativo: val.id_ativo,
                                cd_pessoa: val.cd_pessoa_dependente,
                                no_pessoa: val.no_pessoa_dependente,
                                pc_bolsa: val.pc_bolsa,
                                dt_inicio_bolsa: val.dt_inicio_bolsa,
                                vl_abatimento_matricula: val.vl_abatimento_matricula,
                                dc_reduzido_pessoa_escola: val.dc_reduzido_pessoa_escola,
                                cd_pessoa_escola_aluno: val.cd_pessoa_escola_aluno
                            };
                            insertObjSort(dados, "cd_aluno", newAluno);
                            insertObjSort(gridAlunos.listaCompletaAlunos, "cd_aluno", newAluno);
                        });
                        gridAlunos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados, idProperty: "cd_aluno" }) }));
                    }
                    verificarESetarFiltroSituacaoaAguardando();
                }
            } else {
                //Inseri aluno na turma fuilha ppt.
                if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
                    if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                    valido = false;
                }
                else {
                    var dados = [];
                    var newAluno = {
                        cd_aluno_turma: 0, cd_turma: 0,
                        cd_aluno: gridPesquisaAluno.itensSelecionados[0].cd_aluno,
                        no_aluno: gridPesquisaAluno.itensSelecionados[0].no_pessoa,
                        cd_pessoa_aluno: gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno,
                        cd_situacao_aluno_turma: 9,
                        situacaoAlunoTurma: msgAguardandoMat,
                        id_ativo: gridPesquisaAluno.itensSelecionados[0].id_ativo,
                        cd_pessoa: gridPesquisaAluno.itensSelecionados[0].cd_pessoa_dependente,
                        no_pessoa: gridPesquisaAluno.itensSelecionados[0].no_pessoa_dependente,
                        pc_bolsa: gridPesquisaAluno.itensSelecionados[0].pc_bolsa,
                        dt_inicio_bolsa: gridPesquisaAluno.itensSelecionados[0].dt_inicio_bolsa,
                        vl_abatimento_matricula : gridPesquisaAluno.itensSelecionados[0].vl_abatimento_matricula
                    };
                    insertObjSort(dados, "cd_aluno", newAluno);
                    insertObjSort(gridAlunos.listaCompletaAlunos, "cd_aluno", newAluno);
                    gridAlunos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados, idProperty: "cd_aluno" }) }));
                    //dijit.byId("btnNovoAluno").set("disabled", true);
                }
            }
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            var totalAlunos = gridAlunos.listaCompletaAlunos.length;
            if (dojo.byId("cd_turma").value > 0)
                desabilitarCamposTuma(tipoTurma, totalAlunos);
        }
        //Retorna para o modal de aluno e curso quando a inclusão e na turma PPT.
        if (dojo.byId("setValuePesquisaAlunoFK").value == PESQUISAALUNOPPT) {
            if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
                if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                valido = false;
            } else {
                dojo.byId("cdAlunoPPT").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                dojo.byId("descAlunoPPT").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
            }
        } else
            if (dojo.byId("setValuePesquisaAlunoFK").value == PESQUISAALUNOFILTRO) {
                if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0 || gridPesquisaAluno.itensSelecionados.length > 1) {
                    if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
                    valido = false;
                } else {
                    dojo.byId("cdAlunoPesTurma").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
                    dojo.byId("noAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
                    dijit.byId('limparAlunoPes').set("disabled", false);
                }
            }
        if (!valido)
            return false;
        habilitarBotaoAluno();
        dojo.byId("ehSelectGradeAlunoFK").value = false;
        dijit.byId("proAluno").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarProfessorFK() {
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var tipoVerif = dojo.byId("tipoVerif").value;
        var gridProfessores = dijit.byId("gridProfessor");
        var valido = true;
        var listHorariosInsert = new Array();
        var gridPesquisaProfessor = dijit.byId("gridPesquisaProfessor");
        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
            gridProfessores = dijit.byId("gridProfessorPPT");

        if (!hasValue(gridPesquisaProfessor.itensSelecionados) || gridPesquisaProfessor.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        }
        else {
            // Coloca os professores selecionados dentro da grid de professores da turma (esses registros de horários são somente para mostrar na grade):
            var horariosSelecionados = new Array(); //Horários selecionados na FK de professor.
            var dados = hasValue(gridProfessores.store.objectStore.data) ? gridProfessores.store.objectStore.data.splice(0) : new Array();

            quickSortObj(dados, 'cd_professor');

            $.each(gridPesquisaProfessor.itensSelecionados, function (index, val) { //Para cada professor
                var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
                var cd_sala = hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : 0;
                var cd_sala_online = hasValue(dijit.byId("cbSalaOnLine").value) ? dijit.byId("cbSalaOnLine").value : 0;

                //Adiciona os horários antigos do professor:
                var posicao = binaryObjSearch(dados, "cd_professor", val.cd_funcionario);
                if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                    horariosSelecionados = getHorariosProfSelecionadosFilhasPPT();
                else
                    horariosSelecionados = getHorariosProfSelecionados();
                if (hasValue(posicao, true)) {
                    var horariosAdic = dados[posicao].horarios; //Horários antigos dos professores
                    if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL)
                        if (hasValue(horariosAdic))
                            listHorariosInsert = compDifEntreDoisHorarios(horariosAdic, horariosSelecionados);
                        else
                            listHorariosInsert = horariosSelecionados;
                    for (var j = 0; j < horariosAdic.length; j++) {
                        removeObjSort(horariosSelecionados, "id", horariosAdic[j].id); // Remove somente para garantir que não irá ter horários duplicados.
                        insertObjSort(horariosSelecionados, "id", horariosAdic[j]);
                    }
                } else
                    listHorariosInsert = horariosSelecionados;
                // Pega a chave do professor turma para não apago-lo.
                var cd_prof_turma = 0;
                if (hasValue(posicao, true)) {
                    if (hasValue(dados[posicao].cd_professor_turma) && dados[posicao].cd_professor_turma > 0)
                        cd_prof_turma = dados[posicao].cd_professor_turma;
                }
                removeObjSort(dados, "cd_professor", val.cd_funcionario);
                insertObjSort(dados, "cd_professor",
                    {
                        cd_professor_turma: cd_prof_turma,
                        cd_turma: 0, cd_professor: val.cd_funcionario,
                        no_professor: val.dc_reduzido_pessoa,
                        id_professor_ativo: true,
                        horarios: horariosSelecionados
                    });
                if (tipoTurma != TURMA_PPT_FILHA && tipoVerif != VERIFITURMAFILHAPPT2MODAL)
                    if (hasValue(dijit.byId("gridHorarios")))
                        buscaPopulaHorariosProfessor(dojo.xhr, val.cd_funcionario, cd_turma, cd_sala, cd_sala_online);
            });

            gridProfessores.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));
            var horariosTurma = new Array();
            //Coloca os professores dentro dos seus respectivos horários da grid de horários da turma (esses registros serão salvos):
            if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                horariosTurma = validaRetornaHorariosFIlhaPPT();
            else
                horariosTurma = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);
            for (var i = 0; i < horariosSelecionados.length; i++) {
                var posicao = binaryObjSearch(horariosTurma, 'id', horariosSelecionados[i].id);
                if (hasValue(posicao, true)) {
                    if (!hasValue(horariosTurma[posicao].HorariosProfessores, true))
                        horariosTurma[posicao].HorariosProfessores = new Array();

                    quickSortObj(horariosTurma[posicao].HorariosProfessores, 'cd_professor');

                    for (var j = 0; j < gridPesquisaProfessor.itensSelecionados.length; j++)
                        insertObjSort(horariosTurma[posicao].HorariosProfessores, "cd_professor",
                            {
                                cd_horario: horariosTurma[posicao].cd_horario,
                                cd_professor: gridPesquisaProfessor.itensSelecionados[j].cd_funcionario,
                                cd_horario_professor_turma: null
                            }, false);

                    //    horariosTurma[posicao].HorariosProfessores.push({ cd_horario: horariosTurma[posicao].cd_horario, cd_professor: gridPesquisaProfessor.itensSelecionados[j].cd_funcionario, cd_horario_professor_turma: null});
                }
            }
            if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL)
                atualizarHorariosProfessorEProfessorHorariosTurmasFilhas(listHorariosInsert, gridPesquisaProfessor.itensSelecionados);
        }
        if (!valido)
            return false;
        dijit.byId("proProfessor").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarHorariosProfessorEProfessorHorariosTurmasFilhas(horariosInsertProf, professroesSelecionados) {
    try {
        if (hasValue(horariosInsertProf)) {
            var gridAlunosPPT = dijit.byId("gridAlunosPPT");
            $.each(professroesSelecionados, function (index, val) { //Para cada professor
                atualizarListaTurmaFilhaComHorariosProfessor(gridAlunosPPT.store.objectStore.data, val);
                atualizarListaTurmaFilhaComHorariosProfessor(gridAlunosPPT.listaCompletaAlunos, val);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarListaTurmaFilhaComHorariosProfessor(listaTurmaFilha, val) {
	var alunosAtivos = listaTurmaFilha.filter(function(aluno) {
        if (aluno.alunoTurma.cd_situacao_aluno_turma == ATIVO ||
            aluno.alunoTurma.cd_situacao_aluno_turma == REMATRICULADO ||
            aluno.alunoTurma.cd_situacao_aluno_turma == AGUARDANDO) {
	        return aluno;
        }
    });
	//console.log(alunosAtivos);
	listaTurmaFilha = alunosAtivos;
    $.each(listaTurmaFilha, function (idx, value) {//Para cada turma aluno
        if (!LIBERAR_HABILITACAO_PROFESSOR || (val.cursosHabilitacao.indexOf(value.cd_curso)) > 0) {
            var cd_turma = value.cd_turma;
            var dados = (hasValue(value.ProfessorTurma.cd_professor_turma) && value.ProfessorTurma.cd_professor_turma == 0) ? value.ProfessorTurma.splice(0) : new Array();
            //Adiciona os horários antigos do professor:
            var posicao = binaryObjSearch(dados, "cd_professor", val.cd_funcionario);

            var horarios = value.horariosTurma;
            var horariosSelecionados = new Array();
            horariosSelecionadosTurmaPPT = getHorariosProfSelecionados();
            //Seleciona somente os horários marcados:
            quickSortObj(horarios, 'id');

            for (var i = 0; i < horariosSelecionadosTurmaPPT.length; i++) {
                for (var j = 0; j < horarios.length; j++) {
                    if (horariosSelecionadosTurmaPPT[i].id_dia_semana == horarios[j].id_dia_semana &&
                           horarios[j].dt_hora_ini >= horariosSelecionadosTurmaPPT[i].dt_hora_ini && horarios[j].dt_hora_ini < horariosSelecionadosTurmaPPT[i].dt_hora_fim &&
                           horarios[j].dt_hora_fim > horariosSelecionadosTurmaPPT[i].dt_hora_ini && horarios[j].dt_hora_fim <= horariosSelecionadosTurmaPPT[i].dt_hora_fim) {
                        horariosSelecionados.push(horarios[j]);
                    }
                }
            }

            if (hasValue(posicao, true)) {
                var horariosAdic = dados[posicao].horarios; //Horários antigos dos professores
                for (var j = 0; j < horariosAdic.length; j++) {
                    removeObjSort(horariosSelecionados, "id", horariosAdic[j].id); // Remove somente para garantir que não irá ter horários duplicados.
                    insertObjSort(horariosSelecionados, "id", horariosAdic[j]);
                }
            }

            //Verificação para não deixar inserir professor sem horário na turma.
            if (horariosSelecionados != null && horariosSelecionados.length > 0) {
                // Pega a chave do professor turma para não apago-lo.
                var cd_prof_turma = 0;
                if (hasValue(posicao, true)) {
                    if (hasValue(dados[posicao].cd_professor_turma) && dados[posicao].cd_professor_turma > 0)
                        cd_prof_turma = dados[posicao].cd_professor_turma;
                }
                removeObjSort(dados, "cd_professor", val.cd_funcionario);
                insertObjSort(dados, "cd_professor", {
                    cd_professor_turma: cd_prof_turma, cd_turma: 0, cd_professor: val.cd_funcionario,
                    no_professor: val.dc_reduzido_pessoa, id_professor_ativo: true, horarios: horariosSelecionados
                });


                $.each(dados,
	                function(idx, valueProfessor) {
		                value.ProfessorTurma.push(valueProfessor);
                    });

                var horariosTurma = value.horariosTurma
                //Coloca os professores dentro dos seus respectivos horários da grid de horários da turma (esses registros serão salvos):
                for (var i = 0; i < horariosSelecionados.length; i++) {
                    var posicao = binaryObjSearch(horariosTurma, 'id', horariosSelecionados[i].id);
                    if (hasValue(posicao, true)) {
                        if (!hasValue(horariosTurma[posicao].HorariosProfessores, true))
                            horariosTurma[posicao].HorariosProfessores = new Array();

                        quickSortObj(horariosTurma[posicao].HorariosProfessores, 'cd_professor');

                        //for (var j = 0; j < value.ProfessorTurma.length; j++)
                        //    insertObjSort(horariosTurma[posicao].HorariosProfessores, "cd_professor",
                        //        {
                        //            cd_horario: horariosTurma[posicao].cd_horario,
                        //            cd_professor: value.ProfessorTurma[j].cd_professor,  //val.cd_funcionario, //
                        //            cd_horario_professor_turma: null
                        //        }, false);
                        for (var j = 0; j < value.ProfessorTurma.length; j++)
                            for (var k = 0; k < value.ProfessorTurma[j].horarios.length; k++)
                                if (horariosTurma[posicao].cd_horario == value.ProfessorTurma[j].horarios[k].cd_horario) {
                                    insertObjSort(horariosTurma[posicao].HorariosProfessores, "cd_professor",
                                        {
                                            cd_horario: horariosTurma[posicao].cd_horario,
                                            cd_professor: value.ProfessorTurma[j].cd_professor,  //val.cd_funcionario, //
                                            cd_horario_professor_turma: null
                                        }, false);
                                    break;
                                }

                    }
                }
            }
        }
    });
}

function compDifEntreDoisHorarios(listHorariosProfessor, listaHorariosProfSelecionado, professroesSelecionados) {
    try {
        var horariosInsert = new Array();
        quickSortObj(listHorariosProfessor, 'id');
        quickSortObj(listaHorariosProfSelecionado, 'id');
        $.each(listaHorariosProfSelecionado, function (index, value) {
            var posicao = binaryObjSearch(listHorariosProfessor, "id", value.id);
            if (!hasValue(posicao, true))
                horariosInsert.push(value);
        });
        return horariosInsert;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarCursoFK() {
    try {
        var valido = true;
        var gridCursosFK = dijit.byId("gridPesquisaCurso");
        if (!hasValue(gridCursosFK.itensSelecionados) || gridCursosFK.itensSelecionados.length <= 0 || gridCursosFK.itensSelecionados.length > 1) {
            if (gridCursosFK.itensSelecionados == null || gridCursosFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridCursosFK.itensSelecionados != null && gridCursosFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        } else {
            dojo.byId("cdCursoPPT").value = gridCursosFK.itensSelecionados[0].cd_curso;
            dijit.byId("descCursoPPT").set("value", gridCursosFK.itensSelecionados[0].no_curso);
        }
        if (!valido)
            return false;
        dojo.byId("ehSelectGrade").value = false;
        dijit.byId("proCurso").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKTurma() {
    try {
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
            dojo.byId("cd_turma_ppt").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId("nomTurmaPPF").set("value", gridPesquisaTurmaFK.itensSelecionados[0].no_turma);
            dijit.byId("pesProTurmaFK").set("disabled", true);
            destroyCreateGridHorario();
            mudarLegendaPorTipoTurma(TURMA_PPT_FILHA);
            showLoadTurmaPPT(gridPesquisaTurmaFK.itensSelecionados[0].cd_turma, TURMA_PPT_FILHA);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaRetornaHorarios(tipoTurma) {
    try {
        var cdTurma = dojo.byId("cd_turma").value;
        var cdTurmaPPT = dojo.byId("cd_turma_ppt").value;
        var gridHorarios = dijit.byId("gridHorarios");
        var gridTurma = dijit.byId("gridTurma");
        var horarios = [];
        if (hasValue(gridHorarios) && gridHorarios.items.length >= 0)
            horarios = gridHorarios.params.store.data;
        else if ((hasValue(cdTurma) && cdTurma > 0) || (hasValue(cdTurmaPPT) && cdTurmaPPT > 0)) {
            if (hasValue(gridTurma) && hasValue(gridTurma.TurmaEdit) && hasValue(gridTurma.TurmaEdit.horariosTurma))
                horarios = gridTurma.TurmaEdit.horariosTurma;
        }

        if (tipoTurma == TURMA_PPT_FILHA)
            if (!hasValue(gridHorarios) && cdTurma <= 0 && hasValue(gridTurma.horariosClonePPT))
                horarios = gridTurma.horariosClonePPT;

        return horarios;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaRetornaHorariosFIlhaPPT() {
    try {
        var horarios = [];
        var gridHorariosPPT = dijit.byId("gridHorariosPPT");
        var gridAlunosPPT = dijit.byId("gridAlunosPPT");
        if (hasValue(gridHorariosPPT) && gridHorariosPPT.items != null && gridHorariosPPT.items.length > 0)
            horarios = mountHorariosOcupadosTurma(gridHorariosPPT.params.store.data);
        else if (hasValue(gridAlunosPPT.itensSelecionados[0] && hasValue(gridAlunosPPT.itensSelecionados[0].horariosTurma)))
            horarios = gridAlunosPPT.itensSelecionados[0].horariosTurma;
        return horarios;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridTurma = dijit.byId('gridTurma');
            apresentaMensagem('apresentadorMensagem', '');
            pesquisarEscolasVinculadasUsuario();
            dojo.byId('abriuEscola').value = false;
            gridTurma.itemSelecionado = gridTurma.itensSelecionados[0];
            setarTabCad();
            destroyCreateGridHorario();
            destroyCreateProgramacao();
            //dijit.byId("pesCadProduto").set("disabled", true);
            //dijit.byId("pesCadCurso").set("disabled", true);
            dijit.byId("pesCadRegime").set("disabled", true);
            virada = false;
            keepValues(null, gridTurma, true, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton);
            IncluirAlterar(0, 'divAlterarTurma', 'divIncluirTurma', 'divExcluirTurma', 'apresentadorMensagemATurma', 'divCancelarTurma', 'divClearTurma');
            popularGridEscolaTurma();
            dijit.byId("cadTurma").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function returnProfAtivoTurma() {
    try {
        var gridProf = dijit.byId("gridProfessor");
        var cd_prof = new Array();

        if (hasValue(gridProf) && gridProf.store.objectStore.data.length > 0)
            $.each(gridProf.store.objectStore.data, function (index, value) {
                if (value.id_professor_ativo)
                    cd_prof.push(value.cd_professor);
            });
        return cd_prof;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function returnProfessoresAtivosTurma() {
    try {
        var gridProf = dijit.byId("gridProfessor");
        var cd_professores = [];
        if (hasValue(gridProf) && gridProf.store.objectStore.data.length > 0)
            $.each(gridProf.store.objectStore.data, function (index, value) {
                if (value.id_professor_ativo) {
                    cd_professores.push(value.cd_professor);
                }
            });
        return cd_professores;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function buscaPopulaHorariosProfessor(xhr, cd_professor, cd_turma, cd_sala, cd_sala_online) {
    if (cd_sala_online > 0)
        cd_sala = cd_sala_online;
    xhr.post({
        url: Endereco() + "/api/secretaria/getHorariosProfessorTurma",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: dojox.json.ref.toJson({
            cd_turma: parseInt(cd_turma),
            cd_sala: parseInt(cd_sala),
            cd_professor: cd_professor,
            dt_inicio: dijit.byId('dtIniAula').get('value'),
            dt_final: dijit.byId('dtFimAula').get('value'),
            cd_duracao: dijit.byId('pesCadDuracao').value,
            cd_curso: hasValue(dojo.byId('pesCadCurso').value) ? dijit.byId('pesCadCurso').value : 0
        })
    }).then(function (dataHorario) {
        try {
            dataHorario = jQuery.parseJSON(dataHorario);
            apresentaMensagem("apresentadorMensagemTurma", null);
            var calendar = dijit.byId("gridHorarios");
            var codMaiorHorarioTurma = buscarMaiorCodigoHorario();
            if (hasValue(dataHorario.retorno) && dataHorario.retorno.horariosOcupProf != null && dataHorario.retorno.horariosOcupProf.length > 0)
                $.each(dataHorario.retorno.horariosOcupProf, function (idx, val) {
                    codMaiorHorarioTurma += 1;
                    val.calendar = "Calendar4";
                    //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
                    val.id = codMaiorHorarioTurma;
                    val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                    val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                    calendar.store.add(val);
                });
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem(dojo.byId("apresentadorMensagemTurma").value, error);
    });
}

function limparHorariosProfTurma(cd_professor) {
    try {
        var gridHorarios = dijit.byId('gridHorarios');

        if (hasValue(gridHorarios)) {
            var storeHorarios = gridHorarios.items.length;

            if (!hasValue(cd_professor)) {
                for (var i = gridHorarios.items.length - 1; i >= 0 ; i--)
                    if (gridHorarios.items[i].cssClass == "Calendar4")
                        //gridHorarios.store.remove(gridHorarios.items[i].id);
                        removeHorario(gridHorarios, gridHorarios.items[i].id);
            }
            else
                for (var i = gridHorarios.store.data.length - 1; i >= 0; i--)
                    if (hasValue(gridHorarios.store.data[i].HorariosProfessores))
                        for (var j = gridHorarios.store.data[i].HorariosProfessores.length - 1; gridHorarios.store.data[i].calendar == "Calendar4" && j >= 0; j--)
                            if (cd_professor == gridHorarios.store.data[i].HorariosProfessores[j].cd_professor) {
                                // Remove o professor da lista do horário:
                                gridHorarios.store.data[i].HorariosProfessores.splice(j, 1);

                                //Remove o horário alocado pelo professor:
                                if (gridHorarios.store.data[i].HorariosProfessores.length <= 0) {
                                    //gridHorarios.store.remove(gridHorarios.store.data[i].id);
                                    removeHorario(gridHorarios, gridHorarios.store.data[i].id);
                                    break;
                                }
                            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function buscarMaiorCodigoHorario() {
    try {
        var codigo = 0;
        var gridHorarios = dijit.byId('gridHorarios');
        if (hasValue(gridHorarios) && hasValue(gridHorarios.items) && gridHorarios.items.length > 0) {
            var storeHorarios = cloneArray(gridHorarios.items);
            quickSortObj(storeHorarios, "id");
            codigo = storeHorarios[storeHorarios.length - 1].id;
        }
        return codigo;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validaBuscaHorariosProfessor(cd_prof) {
    try {
        //var cd_prof = returnProfAtivoTurma();
        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var cd_sala = hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : 0;
        var cd_sala_online = hasValue(dijit.byId("cbSalaOnLine").value) ? dijit.byId("cbSalaOnLine").value : 0;
        if (hasValue(dijit.byId("gridHorarios")))
            buscaPopulaHorariosProfessor(dojo.xhr, cd_prof, cd_turma, cd_sala, cd_sala_online);
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region Metodo C.R.U.D Turma
function salvarTurma(xhr, ref) {
    showCarregando();
    try {
        var turma = null;
        if (!validarTurmaCadastro('apresentadorMensagemTurma')) {
            hideCarregando();
            return false;
        }
        turma = mountDataTurmaForPost();
        xhr.post({
            url: Endereco() + "/api/turma/postInsertTurma",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(turma)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridTurma';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_turma", itemAlterado);
                    buscarItensSelecionados(gridName, 'turmaSelecionada', 'cd_turma', 'selecionaTodos', ['pesquisarTurma', 'relatorioTurma'], 'todosItens');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_turma");
                    hideCarregando();
                    dijit.byId("cadTurma").hide();

                } else {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemTurma', data);
                }
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        }, function (error) {
                hideCarregando();
            apresentaMensagem('apresentadorMensagemTurma', error);
        });
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function alterarTurma(xhr, ref) {
    try {
        var turma = null;
        if (!validarTurmaCadastro("apresentadorMensagemTurma")) {
            return false;
        }
        if (OUTRAESCOLA)
            caixaDialogo(DIALOGO_CONFIRMAR, msgAvisoTurmaOutraEscola,
                function () { editarTurma(xhr, ref) },
                function () { return false }
            )
        else
            editarTurma(xhr, ref);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function editarTurma(xhr, ref) {
    showCarregando();
    try {
        turma = mountDataTurmaForPost();
        xhr.post({
            url: Endereco() + "/api/turma/postUpdateTurma",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(turma)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridTurma';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    removeObjSort(grid.itensSelecionados, "cd_turma", dojo.byId("cd_turma").value);
                    insertObjSort(grid.itensSelecionados, "cd_turma", itemAlterado);
                    buscarItensSelecionados(gridName, 'turmaSelecionada', 'cd_turma', 'selecionaTodos', ['pesquisarTurma', 'relatorioTurma'], 'todosItens');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_turma");
                    if (novaTurmaEnc)
                        dijit.byId('dialogEncerramento3').hide();
                    hideCarregando();
                    dijit.byId("cadTurma").hide();
                }
                else {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemTurma', data);
                }
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        }, function (error) {
                hideCarregando();
                apresentaMensagem('apresentadorMensagemTurma', error);
        });
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function deletarTurmas(itensSelecionados, xhr, ref) {
    try {
        if (OUTRAESCOLA) {
            caixaDialogo(DIALOGO_AVISO, msgErroMatriculaAlunoOutroaEscola, null);
            return false;
        }
        showCarregando();
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_turma').value != 0)
                itensSelecionados = [{
                    cd_turma: dojo.byId("cd_turma").value
                }];
        xhr.post({
            url: Endereco() + "/api/turma/postdeleteTurma",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("cadTurma").hide();
                dijit.byId("_nomTurma").set("value", '');
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridTurma').itensSelecionados, "cd_turma", itensSelecionados[r].cd_turma);
                pesquisarTurma(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarTurma").set('disabled', false);
                dijit.byId("relatorioTurma").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                hideCarregando();
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            hideCarregando();
            if (!hasValue(dojo.byId("cadTurma").style.display))
                apresentaMensagem('apresentadorMensagemTurma', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function pesquisarTurma(limparItens, cd_turma_ppt, click_mostrar_turmas_filhas) {
    var cdAluno = hasValue(dojo.byId('cdAlunoPesTurma').value) ? parseInt(dojo.byId('cdAlunoPesTurma').value) : 0;
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var grid = dijit.byId("gridTurma");
            var descricao = encodeURIComponent(document.getElementById("_nomTurma").value);
            var cdCurso = dijit.byId("pesCurso").value;
            cdCurso = hasValue(cdCurso) ? cdCurso : 0;
            var cdProduto = hasValue(dijit.byId("pesProduto").value) ? dijit.byId("pesProduto").value : 0;
            var cdDuracao = hasValue(dijit.byId("pesDuracao").value) ? dijit.byId("pesDuracao").value : 0;
            var cdProfessor = hasValue(dijit.byId("pesProfessor").value) ? dijit.byId("pesProfessor").value : 0;
            var cdProg = hasValue(dijit.byId("sProgramacao").value) ? dijit.byId("sProgramacao").value : 0;
            var cdSitTurma = hasValue(dijit.byId("pesSituacao").value) ? dijit.byId("pesSituacao").value : 0;
            var cdTipoTurma = hasValue(dijit.byId("tipoTurma").value) ? dijit.byId("tipoTurma").value : 0;

            var cd_search_sala = hasValue(dijit.byId('cbSearchSala').value) ? dijit.byId('cbSearchSala').value : 0;
            var cd_search_sala_online = hasValue(dijit.byId('cbSearchSalaOnLine').value) ? dijit.byId('cbSearchSalaOnLine').value : 0;
            var ckSearchSemSala = dijit.byId('ckSemSala').checked ? true : false;
            var ckSearchSemAluno = dijit.byId('ckSemAluno').checked ? true : false;

            //(string descricao, bool inicio, int tipoTurma, int cdCurso, int cdDuracao, int cdProduto, int situacaoTurma, int cdProfessor, bool sProg)
            if (hasValue(click_mostrar_turmas_filhas) && click_mostrar_turmas_filhas)
                descricao = "";
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

            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaSearch?descricao=" + descricao +
                            "&apelido=" + encodeURIComponent(document.getElementById("_apelidoT").value) +
                            "&inicio=" + document.getElementById("inicioTurma").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor + "&prog=" + cdProg +
                                            "&turmasFilhas=" + document.getElementById("pesTurmasFilhas").checked + "&cdAluno=" + cdAluno + "&dtInicial=" + dojo.byId("dtInicial").value + "&dtFinal=" + dojo.byId("dtFinal").value +
                            "&cd_turma_PPT=" + (cd_turma_ppt == undefined ? 0 : cd_turma_ppt) + "&semContrato=" + document.getElementById('semContrato').checked + "&ProfTurmasAtuais=" + document.getElementById('ckIdProfTurmasAtuais').checked + "&cd_escola_combo=0" +
                            "&ckOnLine=" + dijit.byId('ckOnLine').value + "&dias=" + dias +
							"&cd_search_sala=" + cd_search_sala + "&cd_search_sala_online=" + cd_search_sala_online + "&ckSearchSemSala=" + ckSearchSemSala + "&ckSearchSemAluno=" + ckSearchSemAluno,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                grid.itensSelecionados = [];
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
            if ((document.getElementById("pesTurmasFilhas").checked) || (cd_turma_ppt != null && cd_turma_ppt > 0)) {
                grid.layout.setColumnVisibility(4, true)
                grid.layout.setColumnVisibility(5, false)
                if (cd_turma_ppt > 0) {
                    grid.turmasFilhas = true;
                    removeObjSort(grid.itensSelecionados, "cd_turma", cd_turma_ppt);
                }
            }
            else {
                grid.layout.setColumnVisibility(4, false)
                grid.layout.setColumnVisibility(5, true)
                grid.turmasFilhas = false;
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//#endregion
function validarTurmaCadastro(msg) {
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var retorno = true;
        if (!dijit.byId("idTurmaPPTPai").get("checked"))
            dijit.byId("pesCadCurso").set("required", true);
        else
            dijit.byId("pesCadCurso").set("required", false);
        var retorno = true;
        if (!dijit.byId("formTurmaPrincipal").validate()) {
            setarTabCad();
            retorno = false;
        }

        //LBM Não é mais
        //if (!dijit.byId("cbSala").validate()) {
        //    var mensagensWeb = new Array();
        //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgSalaObrigatorioModalidadeOnline);
        //    apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
        //    retorno = false;
        //}

        if (!verificarHorarioAlunoProfessorTurma(VERIFTURMANORMAL, msg))
            retorno = false;
        if (tipoTurma == TURMA_PPT_FILHA) {
            var gridAlunos = dijit.byId("gridAlunos");
            if ((hasValue(gridAlunos) && gridAlunos.listaCompletaAlunos.length <= 0) && !hasValue(dojo.byId(nm_alunos).value)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgDeveSelecionarAlunoTurmaPPTFilha);
                apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                retorno = false;
            }
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCadTurma(Memory) {
    try {
        setarTabCad();
        dojo.byId("tipoTurmaCad").value = TURMA;
        var gridTurma = dijit.byId("gridTurma");
        var gridAlunos = dijit.byId("gridAlunos");
        var gridProfessor = dijit.byId("gridProfessor");
        var gridProgramacao = dijit.byId("gridProgramacao");
        var gridAlunoPPT = dijit.byId("gridAlunosPPT");
        var gridDesconsideraFeriados = dijit.byId('gridDesconsideraFeriados');
        dijit.byId("idTurmaPPTPai").reset();
        dojo.byId("cd_turma").value = 0;
        dojo.byId("cd_turma_ppt").value = 0;
		dojo.byId("cd_turmaPPT").value = 0;
        dojo.byId("escolaturma").value = 0;
        clearForm("formTurmaPrincipal");
        clearForm("formSemana");
        mudarLegendaPorTipoTurma(TURMA);
        dijit.byId("pesCadCurso")._onChangeActive = false;
        dijit.byId("pesCadCurso").reset();
        dijit.byId("pesCadCurso")._onChangeActive = true;

        dijit.byId("pesCadRegime").reset();

        dijit.byId("pesCadProduto")._onChangeActive = false;
        dijit.byId("pesCadProduto").reset();
        dijit.byId("pesCadProduto")._onChangeActive = true;

        dijit.byId("pesCadDuracao")._onChangeActive = false;
        dijit.byId("pesCadDuracao").reset();
        dijit.byId("pesCadDuracao")._onChangeActive = true;

        dijit.byId('dtIniAula').dta_inicio_aula_anterior = null;
        dijit.byId('pesCadDuracao').dta_inicio_aula_anterior = null;
        dijit.byId('pesCadCurso').dta_inicio_aula_anterior = null;

        dijit.byId("dtIniAula").reset();
        dijit.byId("dtFimAula").reset();
        dijit.byId("dtaTermino").reset();
        dijit.byId("cbSala")._onChangeActive = false;
        dijit.byId("cbSala").reset();
        dijit.byId("cbSala")._onChangeActive = true;
        dijit.byId("cbSala").set('disabled',true);
        dijit.byId("cbSalaOnLine")._onChangeActive = false;
        dijit.byId("cbSalaOnLine").reset();
        dijit.byId("cbSalaOnLine")._onChangeActive = true;
        dijit.byId("cbSalaOnLine").set('disabled',true);

        confLayoutTurma(TURMA, NOVO, null, null, null, null, null, null, null, null);

        if (hasValue(gridTurma)) {
            gridTurma.TurmaEdit = null;
            gridTurma.horariosClonePPT = null;
        }
        dijit.byId('cbDias').setStore(dijit.byId('cbDias').store, []);
        if (hasValue(dijit.byId("gridHorarios"))) {
            if (hasValue(dojo.byId("cd_turma").value) && dojo.byId("cd_turma").value > 0)
                limparNewsHorarios();
            else
                limparTodosHorarios();
        }
        if (hasValue(gridAlunos)) {
            gridAlunos.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridAlunos.itensSelecionados = [];
            gridAlunos.listaCompletaAlunos = [];
        }
        if (hasValue(gridProfessor)) {
            gridProfessor.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridProfessor.itensSelecionados = [];
        }
        if (hasValue(gridProgramacao)) {
            gridProgramacao.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridProgramacao.itensSelecionados = [];
        }
        if (hasValue(gridAlunoPPT)) {
            gridAlunoPPT.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridAlunoPPT.listaCompletaAlunos = [];
            gridAlunoPPT.itensSelecionados = [];
        }

        if (hasValue(gridDesconsideraFeriados)) {
            gridDesconsideraFeriados.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridDesconsideraFeriados.itensSelecionados = [];
        }
        if (hasValue(dijit.byId('menuFiltroAluno'))) {
            var filhosMenu = dijit.byId('menuFiltroAluno').getDescendants();
            if (hasValue(filhosMenu))
                $.each(filhosMenu, function (index, val) {
                    if (val.id != "100")//Todos
                        if (hasValue(dijit.byId(val.id + "")))
                            dijit.byId(val.id + "").destroy();
                });
        }
        if (hasValue(dijit.byId('menuFiltroAlunoPPT'))) {
            var filhosMenu = dijit.byId('menuFiltroAlunoPPT').getDescendants();
            if (hasValue(filhosMenu))
                $.each(filhosMenu, function (index, val) {
                    if (val.id != "filtroSit100") {//Todos
                        if (hasValue(dijit.byId(val.id + "")))
                            dijit.byId(val.id + "").destroy();
                    }
                });
        }
        dijit.byId("tagAlunos").set("open", false);
        dijit.byId("tagProfessores").set("open", false);
        dijit.byId("tagAlunosPPT").set("open", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(value, grid, ehLink, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton) {
    try {
        limparCadTurma(Memory);
        dijit.byId("idTurmaPPTPai").set("disabled", true);
        var valorCancelamento = grid.selection.getSelected();
        var linkAnterior = document.getElementById('link');
        dojo.byId('abriuEscola').value = false;
        limparGrid();
        setarTabCad();
        var tipoTurma = TURMA;
        if (!hasValue(ehLink, true))
            ehLink = eval(linkAnterior.value);
        linkAnterior.value = ehLink;

        // Quando for cancelamento:
        if (!hasValue(value) && hasValue(valorCancelamento) && !ehLink)
            value = valorCancelamento[0];

        // Quando value for nulo, dá prioridade para os itens selecionados, pois a ação do link e o cancelamento passa value nulo e a edição de duplo click não passa:
        if (!hasValue(value) && hasValue(grid.itensSelecionados) && grid.itensSelecionados.length == 1 && ehLink)
            value = grid.itensSelecionados[0];

        if (hasValue(value) && value.cd_turma > 0) {

            dojo.byId("cd_turma").value = value.cd_turma;

            if (hasValue(value.cd_turma_ppt) && value.cd_turma_ppt > 0) {
                confLayoutTurma(TURMA_PPT_FILHA, EDIT, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton);
                tipoTurma = TURMA_PPT_FILHA;
            } else {
                confLayoutTurma(TURMA, EDIT, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton);
                tipoTurma = TURMA;
            }
            dojo.byId("tipoTurmaCad").value = tipoTurma;
            if (virada && dojo.byId("cd_turma_enc").value > 0)
                value.cd_turma = dojo.byId("cd_turma_enc").value;

            OUTRAESCOLA = (value.cd_pessoa_escola != dojo.byId('_ES0').value) && (tipoTurma != TURMA_PPT_FILHA)
            dijit.byId("btnNovoProfessor").set("disabled", OUTRAESCOLA);

            showEditTurma(value.cd_turma, xhr, ready, Memory, filteringSelect, tipoTurma, DropDownMenu, MenuItem, DropDownButton);
            dojo.byId('abriuEscola').value = false;
        }

        dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodos TurmaPPTFilha
function montarGridEComponetesTurmaPPT(ready, domConstruct, array, xhr, registry, EnhancedGrid, Pagination, JsonRest, ObjectStore,
                                       Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect) {
    try {
        dijit.byId('tabContainerPPT').resize();
        inserirIdTabsCadastro();
        criarElementosTagAlunosFilhosPPT(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest);
        criarElementosTagProfessoresPPT(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest);

        new Button({
            label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                salvarTurmaFilhaPPT();
            }
        }, "incluirTurmaPPT");
        new Button({
            label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                try {
                    limparTurmaPPT(Memory);
                    apresentaMensagem('apresentadorMensagemTurmaPPT', null);
                    destroyCreateGridAvaliacaoAluno();
                    keepValuesTurmaPPT(dijit.byId("gridAlunosPPT").itensSelecionados, ready, Memory, FilteringSelect);
                    //keepValues(null, dijit.byId('gridUsuario'), true);
                    popularGridEscolaTurma();
                    setarTabCadFilha();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "cancelarTurmaPPT");
        new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadTurmaPPT").hide(); } }, "fecharTurmaPPT");

        //*** Cria os botões de persistência Horarios**\\
        new Button({
            label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconInsert",
            onClick: function () { incluirItemHorarioTurmaFilhaPPT('gridHorariosPPT', 'apresentadorMensagemTurmaPPT'); }
        }, "incluirHorarioTurmaFilhaPPT");
        new Button({
            label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
            onClick: function () { excluiItemHorarioTurma('gridHorariosPPT', 'apresentadorMensagemTurmaPPT', VERIFITURMAFILHAPPT2MODAL, xhr); }
        }, "excluirHorarioTurmaFilhaPPT");

        // açaões do desconsidera feriado na Filha PPT
        var menuDesconsidera = new DropDownMenu({ style: "height: 25px" });
        var acaoExcluirDesconsidera = new MenuItem({
            label: "Excluir",

            onClick: function () { excluirDesconsideraFeriados(Memory, ObjectStore, xhr, 'gridProgramacaoPPT', 'gridDesconsideraFeriadosPPT', 'apresentadorMensagemTurmaPPT'); }
        });
        menuDesconsidera.addChild(acaoExcluirDesconsidera);

        var acaoEditarDesconsidera = new MenuItem({
            label: "Editar",
            onClick: function () { eventoEditaDesconsidera(dijit.byId("gridDesconsideraFeriadosPPT").itensSelecionados); }
        });
        menuDesconsidera.addChild(acaoEditarDesconsidera);

        var buttonDesconsidera = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasDesconsidera",
            dropDown: menuDesconsidera,
            id: "acoesRelacionadasDesconsideraPPT"
        });
        dojo.byId("linkAcoesDesconsideraPPT").appendChild(buttonDesconsidera.domNode);

        montarLegendaTurmaFilhaPPT();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarElementosTagAlunosFilhosPPT(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest) {
    try {
        var menuAluno = new DropDownMenu({ style: "height: 25px" });
        var acaoHistorico = new MenuItem({
            label: "Histórico do Aluno",
            onClick: function () {
                abrirHistoricoAluno(dijit.byId('gridAlunosPPTFilha'));
            }
        });
        menuAluno.addChild(acaoHistorico);

        var acaoDesistirAluno = new MenuItem({
            label: "Desistência",
            onClick: function () {
                if (hasValue(dijit.byId("gridAlunosPPTFilha")))
                    populaDesistencia(dijit.byId("gridAlunosPPTFilha"), true);
            }
        });
        menuAluno.addChild(acaoDesistirAluno);

        var buttonAluno = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasAlunoPPTFilha",
            dropDown: menuAluno,
            id: "acoesRelacionadasAlunoPPTFilha"
        });
        dojo.byId("linkAcoesAlunoPPTFilha").appendChild(buttonAluno.domNode);

        var gridAlunosPPTFilha = new EnhancedGrid({
            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
              [
                { name: "<input id='selecionaTodosAlunoPPT' style='display:none'/>", field: "alunoSelecionadoPTT", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAlunosPPT },
                { name: "Nome", field: "no_aluno", width: "92%" }
              ],
            canSort: true,
            noDataMessage: msgNotRegEnc,
            selectionMode: "single"
        }, "gridAlunosPPTFilha");
        gridAlunosPPTFilha.startup();

        dijit.byId("tagAlunosPPTFilha").on("show", function (e) {
            dijit.byId("paiAlunoPPTFilha").resize();
            dijit.byId("gridAlunosPPTFilha").update();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarElementosTagProfessoresPPT(Button, DropDownMenu, DropDownButton, MenuItem, EnhancedGrid, ObjectStore, Memory, on, xhr, ref, Cache, JsonRest) {
    try {
        var menuProfessor = new DropDownMenu({ style: "height: 25px" });
        var acaoExcluirProfessor = new MenuItem({
            label: "Excluir",
            onClick: function () {
                try {
                    var gridProfessor = dijit.byId('gridProfessorPPT');
                    var horarios = validaRetornaHorariosFIlhaPPT();

                    for (var k = 0; k < gridProfessor.itensSelecionados.length; k++)
                        for (var l = 0; l < horarios.length; l++)
                            for (var s = horarios[l].HorariosProfessores.length - 1; s >= 0; s--)
                                if (horarios[l].HorariosProfessores[s].cd_professor == gridProfessor.itensSelecionados[k].cd_professor)
                                    horarios[l].HorariosProfessores.splice(s, 1);

                    deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_professor', gridProfessor);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        });
        menuProfessor.addChild(acaoExcluirProfessor);

        var acaoEditarProfessorPPT = new MenuItem({
            label: "Editar",
            onClick: function () {
                try {
                    var gridProfessorPPT = dijit.byId('gridProfessorPPT');
                    var itensSelecionados = gridProfessorPPT.itensSelecionados;
                    if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
                    else if (itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
                    else {
                        var dialogProfessor = dijit.byId("dialogProfessor");
                        apresentaMensagem('apresentadorMensagemProfessor', null);
                        dojo.byId("tipoVerif").value = VERIFITURMAFILHAPPT2MODAL;
                        keepValuesProfessor(null, gridProfessorPPT, true, xhr, Memory, EnhancedGrid);
                        dialogProfessor.on('show', function () {
                            dijit.byId('paiGridHorarioProfessor').resize();
                            dijit.byId('gridHorarioProfessor').update();
                        });
                        dialogProfessor.show();
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        });

        menuProfessor.addChild(acaoEditarProfessorPPT);

        var buttonProfessor = new DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasProfessor",
            dropDown: menuProfessor,
            id: "acoesRelacionadasProfessorFilhasPPT"
        });
        dojo.byId("linkAcoesProfessorFilhaPPT").appendChild(buttonProfessor.domNode);

        var gridProfessorPPT = new EnhancedGrid({
            store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
              [
                { name: "<input id='selecionaTodosProfessorFilhasPPT' style='display:none'/>", field: "selecionadoProfessorFilhasPPT", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProfessorFilhasPPT },
                { name: "Nome", field: "no_professor", width: "50%" },
                { name: "Ativo ", field: "id_professor_ativo", width: "5%", styles: "text-align: center;", formatter: formatCheckBoxProfessorAtivo }
              ],
            canSort: true,
            noDataMessage: "Nenhum registro encontrado."
        }, "gridProfessorPPT");
        gridProfessorPPT.on("RowDblClick", function (evt) {
            try {
                var idx = evt.rowIndex,
                    item = this.getItem(idx),
                    store = this.store;
                var dialogProfessor = dijit.byId("dialogProfessor");

                apresentaMensagem('apresentadorMensagemProfessor', null);
                gridProfessorPPT.itemSelecionado = item;
                dojo.byId("tipoVerif").value = VERIFITURMAFILHAPPT2MODAL;
                keepValuesProfessor(item, gridProfessorPPT, false, xhr, Memory, EnhancedGrid);
                //IncluirAlterar(0, 'divAlterarProf', null, null, 'apresentadorMensagemProfessor', 'divCancelarProf', null);
                dialogProfessor.on('show', function () {
                    dijit.byId('paiGridHorarioProfessor').resize();
                    dijit.byId('gridHorarioProfessor').update();
                });
                dialogProfessor.show();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, true);
        gridProfessorPPT.startup();
        dijit.byId("tagProfessoresPPT").on("show", function (e) {
            dijit.byId("paiProfessorPPT").resize();
            dijit.byId("gridProfessorPPT").update();
        });
        new Button({
            label: "Incluir",
            iconClass: 'dijitEditorIcon dijitEditorIconInsert',
            onClick: function () {
                try {
                    if ((!LIBERAR_HABILITACAO_PROFESSOR) || (LIBERAR_HABILITACAO_PROFESSOR && hasValue(dijit.byId("pesCadCursoPPT").value))) {
                        configLayoutPesquisaProf(true);
                        var gridAlunosPPT = dijit.byId("gridAlunosPPT");
                        dojo.byId("tipoVerif").value = VERIFITURMAFILHAPPT2MODAL;
                        var cdTurma = 0;
                        if (hasValue(gridAlunosPPT))
                            cdTurma = gridAlunosPPT.itensSelecionados[0].cd_turma;
                        var tipoTurma = dojo.byId("tipoTurmaCad").value;
                        horarios = validaRetornaHorariosFIlhaPPT();
                        horariosTurmaPPT = validaRetornaHorarios(tipoTurma);
                        limparPesquisaProfessorFK();
                        dijit.byId("gridPesquisaProfessor").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                        if (horarios == null || horarios.length <= 0) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrInclHorarioTurma);
                            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                            return false;
                        }
                        pesquisaProfessorHorarioTurmaPPT(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horarios, horariosTurmaPPT, cdTurma, gridAlunosPPT.itensSelecionados[0].cd_curso);

                        //Mostra a linha de escolha dos horários e os popula com relação aos horários da turma:
                        showP('trHorarios', true);

                        var horariosTurma = validaRetornaHorariosFIlhaPPT();
                        horariosTurma = cloneArray(horariosTurma);
                        for (var i = horariosTurma.length - 1; i >= 0; i--)
                            if (horariosTurma[i].calendar == "Calendar2" || horariosTurma[i].calendar == null) {
                                horariosTurma[i].dia_semana = getDiaSemana(horariosTurma[i].id_dia_semana - 1);
                                horariosTurma[i].no_datas = horariosTurma[i].dt_hora_ini + ' / ' + horariosTurma[i].dt_hora_fim + ' - ' + horariosTurma[i].dia_semana;
                            }
                            else //Remove da lista os horários que não são ocupados da turma
                                horariosTurma.splice(i, 1);

                        var strCbHorarios = 'cbHorarios';
                        var cbHorarios = dijit.byId(strCbHorarios);

                        //Ordena os horários:
                        var sorto = {
                            id_dia_semana: "asc", dt_hora_ini: "asc", dt_hora_fim: "asc"
                        };
                        horariosTurma.keySort(sorto);

                        cbHorarios._onChangeActive = false;
                        loadMultiSelect(horariosTurma, strCbHorarios, 'id', 'no_datas', true);
                        cbHorarios._onChangeActive = true;

                        if (hasValue(cbHorarios.handle))
                            cbHorarios.handle.remove();

                        cbHorarios.handle = cbHorarios.on("change", function (e) {
                            var horariosSelecionados = getHorariosProfSelecionadosFilhasPPT();

                            dijit.byId("gridPesquisaProfessor").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                            if (hasValue(horariosSelecionados) && horariosSelecionados.length > 0)
                                pesquisaProfessorHorarioTurmaPPT(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horariosSelecionados, horariosTurmaPPT, cdTurma, gridAlunosPPT.itensSelecionados[0].cd_curso);
                            //pesquisaProfessorDisponivelHorario(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horariosSelecionados, cdTurma);
                        });

                        dijit.byId("proProfessor").show();
                    } else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPesquisaProfessorCurso);
                        apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                        return false;
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }
        }, "btnNovoProfessorFilhaPPT");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaProfessorHorarioTurmaPPT(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horariosTurmaFilhas, horariosTurmaPPT, cdTurma, cd_curso) {

    xhr.post({
        url: Endereco() + "/api/Professor/professoresDisponiveisFaixaHorarioTurmaFilhaPPT",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: ref.toJson({
            horariosTurmaFilha: horariosTurmaFilhas,
            horariosTurmaPPT: horariosTurmaPPT,
            cd_curso: cd_curso,
            id_liberar_habilitacao_professor: LIBERAR_HABILITACAO_PROFESSOR
        })
    }).then(function (data) {
        try {
            var gridProfessor = dijit.byId("gridPesquisaProfessor");
            gridProfessor.itensSelecionados = [];
            gridProfessor.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: data.retorno }) }));
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem(dojo.byId("apresentadorMensagemTurma").value, error);
    });
}

function configLayoutPesquisaProf(habilitar) {
    try {
        dijit.byId("nomeProf").set('disabled', habilitar);
        dijit.byId("nomeRed").set('disabled', habilitar);
        dijit.byId("inicioProf").set('disabled', habilitar);
        dijit.byId("cpfProf").set('disabled', habilitar);
        dijit.byId("statusProf").set('disabled', habilitar);
        dijit.byId("nm_sexo_prof").set('disabled', habilitar);
        dijit.byId("pesquisarProfessorFK").set('disabled', habilitar);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxProfessorFilhasPPT(value, rowIndex, obj) {
    try {
        var gridName = 'gridProfessorPPT';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_professor", grid._by_idx[rowIndex].item.cd_professor);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_professor', 'professorSelecionadoFilhasPPT', -1, 'selecionaTodosProfessorFilhasPPT', 'selecionaTodosProfessorFilhasPPT', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_professor', 'professorSelecionadoFilhasPPT', " + rowIndex + ", '" + id + "', 'selecionaTodosProfessorFilhasPPT', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxAlunosPPT(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunosPPTFilha'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosAlunoPPT');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);
            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'alunoSelecionadoPPT', -1, 'selecionaTodosAlunoPPT', 'selecionaTodosAlunoPPT', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'alunoSelecionadoPPT', " + rowIndex + ", '" + id + "', 'selecionaTodosAlunoPPT', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCadFilha() {
    try {
        var tabs = dijit.byId("tabContainerPPT");
        var pane = dijit.byId("tabPrincipalPPT");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaProfessorDisponivelHorario(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horarios, cdTurma, cdTurmaPPT) {
    try {
        var cd_curso = hasValue(dijit.byId("pesCadCurso").value) ? dijit.byId("pesCadCurso").value : 0;
        var cd_produto = hasValue(dijit.byId("pesCadProduto").value) ? dijit.byId("pesCadProduto").value : 0;
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var PPT_pai = tipoTurma == TURMA_PPT ? true : false;
        var horariosOcupadoTurma = [];
        var dt_Final = hasValue(dojo.byId('dtFimAula').value) ? dojo.byId('dtFimAula').value : null;
        var cd_duracao = hasValue(dijit.byId("pesCadDuracao").value) ? dijit.byId("pesCadDuracao").value : 0;
        if (horarios.length > 0)
            $.each(horarios, function (index, value) {
                if (value.calendar == "Calendar2" || value.calendar == null)
                    horariosOcupadoTurma.push(value);
            });
        xhr.post({
            url: Endereco() + "/turma/postHorariosTurma",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(horariosOcupadoTurma)
        }).then(function (data) {
            try {
                var myStore =
                     Cache(
                        JsonRest({
                            target: Endereco() + "/funcionario/getProfessoresDisponiveisFaixaHorario?desc=" + dojo.byId("nomeProf").value + "&nomeRed=" + dojo.byId("nomeRed").value +
                                                "&inicio=" + document.getElementById("inicioProf").checked + "&status=" + dijit.byId("statusProf").value + "&cpf=" + dojo.byId("cpfProf").value +
                                "&sexo=" + dijit.byId("nm_sexo_prof").value + "&cd_turma=" + cdTurma + "&cd_curso=" + cd_curso + "&PPT_pai=" + PPT_pai + "&cd_produto=" + cd_produto +
                                "&dt_Inicio=" + dojo.byId('dtIniAula').value + "&dt_Final=" + dt_Final + "&cd_duracao=" + cd_duracao,
                            handleAs: "json",
                            preventCache: true,
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }), Memory({}));
                var gridProfessor = dijit.byId("gridPesquisaProfessor");
                gridProfessor.itensSelecionados = [];
                gridProfessor.setStore(new dojo.data.ObjectStore({ objectStore: myStore }));
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(dojo.byId("apresentadorMensagemTurma").value, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaProfessorHorarioPorHorariosTurmaFilha(xhr, ObjectStore, Memory, ref, Cache, JsonRest, horarios, cdTurmaPPT) {
    try {
        var horariosOcupadoTurma = [];
        if (horarios.length > 0)
            $.each(horarios, function (index, value) {
                if (value.calendar == "Calendar2" || value.calendar == null)
                    horariosOcupadoTurma.push(value);
            });
        xhr.post({
            url: Endereco() + "/turma/postHorariosTurma",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(horariosOcupadoTurma)
        }).then(function (data) {
            try {
                var cdProf = 0;
                xhr.get({
                    url: Endereco() + "/funcionario/pesquisaProfessorHorarioPorHorariosTurmaFilha?cd_turma_PPT=" + cdTurmaPPT,
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        var gridProfessor = dijit.byId("gridPesquisaProfessor");
                        gridProfessor.itensSelecionados = [];
                        gridProfessor.setStore(new ObjectStore({ objectStore: new Memory({ data: data.retorno }) }));
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem(dojo.byId("apresentadorMensagemTurma").value, error);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(dojo.byId("apresentadorMensagemTurma").value, error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarAlunoPPT(itensSelecionados, ready, Memory, filteringSelect) {
    try {
        //var cdTurma = dojo.byId("cd_turma").value;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
            return false;
        } else if (itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
            return false;
        } else {
            var escola = dojo.byId('escolaturma').value;
           
            dijit.byId("btnNovoProfessorFilhaPPT").set("disabled", escola != dojo.byId('_ES0').value);
            
            limparTurmaPPT(Memory);
            dojo.byId('escolaturma').value = escola;
            apresentaMensagem('apresentadorMensagemTurmaPPT', null);
            destroyCreateGridAvaliacaoAluno();
            //Configurando que se trata de uma edição de turma PPT Filha:
            dojo.byId("tipoVerif").value = VERIFITURMAFILHAPPT2MODAL;
            keepValuesTurmaPPT(itensSelecionados, ready, Memory, filteringSelect);
            setarTabCadFilha();
            popularGridEscolaTurma();
            dijit.byId("cadTurmaPPT").show();
        }
        dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverTurmas(itensSelecionados, xhr, ref) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else {
            if (itensSelecionados[0].cd_pessoa_escola != dojo.byId('_ES0').value) {
                caixaDialogo(DIALOGO_AVISO, 'Turmas de outra escola somente podem ser excluídas na escola de origem.', null);
                return false;
            }
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarTurmas(itensSelecionados, xhr, ref); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridHorarioPPT() {
    try {
        if (hasValue(dijit.byId("gridHorariosPPT"))) {
            dijit.byId("gridHorariosPPT").destroyRecursive();
            $('<div>').attr('id', 'gridHorariosPPT').appendTo('#paiGridHorariosPPT');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateGridProgramacaoPPTFilha() {
    try {

         itensSelecionadosCheckGridPPT = [];
        if (hasValue(dijit.byId("gridProgramacaoPPT"))) {
            dijit.byId("gridProgramacaoPPT").destroyRecursive();
            $('<div>').attr('id', 'gridProgramacaoPPT').attr('style', 'height:235px;').appendTo('#PaiGridProgramacaoPPT');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function destroyCreateFeriadoDesconsideradoPPTFilha() {
    try {
        if (hasValue(dijit.byId("gridDesconsideraFeriadosPPT"))) {
            dijit.byId("gridDesconsideraFeriadosPPT").destroyRecursive();
            $('<div>').attr('id', 'gridDesconsideraFeriadosPPT').attr('style', 'height:235px;').appendTo('#paiDesconsideraFeriadosPPT');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criacaoComponentesProgramacaoPPTFilha(dataProgramacao, dataFeriadosDescon) {
    require(["dojo/ready", "dijit/DropDownMenu", "dijit/form/Button", "dijit/DropDownMenu", "dijit/MenuItem", "dijit/form/DropDownButton", "dojox/grid/EnhancedGrid", "dojo/data/ObjectStore",
        "dojo/store/Memory", "dojo/_base/xhr", "dojox/json/ref", "dijit/MenuSeparator", "dijit/form/CheckBox"],
        function (ready, DropDownMenu, Button, DropDownMenu, MenuItem, DropDownButton, EnhancedGrid, ObjectStore, Memory, xhr, ref, MenuSeparator, CheckBox) {
          ready(function () {
              try {
                  var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataProgramacao }) });

                  destroyCreateGridProgramacaoPPTFilha();

                  var gridProgramacaoPPT = new EnhancedGrid({
                      store: dataStore,
                      structure:
                          [
                            { name: "<input id='selecionaTodosProgramacao' style='display:none'/>", field: "selecionadoProgramacao", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxProgramacao },
                            { name: "Aula", field: "nm_programacao_real", width: "8%" },
                            { name: "Data", field: "dt_programacao_turma", width: "14%" },
                            { name: "Horário", field: "hr_aula", width: "24%" },
                            { name: "Descrição", field: "dc_programacao_turma", width: "45%" },
                            {
                                name: "Calendário", field: "_item", width: "15%", styles: "text-align: center; min-width:15px;", formatter: function (value, rowIndex, obj) {
                                    var id = "id_mostrar_calendario_Selected_" + value.nm_aula_programacao_turma;
                                    if (hasValue(dijit.byId(id)))
                                        dijit.byId(id).destroy();
                                    return new CheckBox({
                                        name: id,
                                        id: id,
                                        value: value,
                                        checked: value.id_mostrar_calendario,
                                        onChange: function (b) {
                                            var id = this.id;
                                            var checked = this.checked;
                                            var filteredArr = itensSelecionadosCheckGridPPT.filter(function (item) {
                                                return item.id == id;
                                            });
                                            if (filteredArr.length > 0) {
                                                var position = itensSelecionadosCheckGridPPT.indexOf(filteredArr[0]);
                                                itensSelecionadosCheckGridPPT[position].checked = checked;

                                            } else {

                                                itensSelecionadosCheckGridPPT.push({ id: this.id, rowIndex: rowIndex, checked: this.checked });
                                            }


                                            progTurma = dijit.byId("gridProgramacaoPPT").store.objectStore.data;

                                            var nm_aula = this.value.nm_aula_programacao_turma;
                                            var itensFilter = progTurma.filter(function (item) {
                                                return item.nm_aula_programacao_turma == nm_aula;
                                            });
                                            if (itensFilter.length > 0) {
                                                var position = progTurma.indexOf(itensFilter[0]);
                                                progTurma[position].id_mostrar_calendario = b;
                                                progTurma[position].programacaoTurmaEdit = true;
                                            }

                                            
                                        }
                                    }, id);
                                }
                            },
                            { name: "Aux", field: "nm_aula_programacao_turma", width: "8%" },
                            { name: "", field: "id_aula_dada", styles: "display:none;" },
                            { name: "", field: "cd_feriado", styles: "display:none;" },
                            { name: "", field: "cd_feriado_desconsiderado", styles: "display:none;" },
                            { name: "", field: "id_feriado_desconsiderado", styles: "display:none;" },
                            { name: "", field: "id_programacao_manual", styles: "display:none;" },                            
                            { name: "", field: "id_prog_cancelada", styles: "display:none;" }
                          ],
                      plugins: {
                          pagination: {
                              pageSizes: ["7", "14", "30", "100", "All"],
                              description: true,
                              sizeSwitch: true,
                              pageStepper: true,
                              defaultPageSize: "7",
                              gotoButton: true,
                              /*page step to be displayed*/
                              maxPageStep: 4,
                              /*position of the pagination bar*/
                              position: "button"
                          }
                      },
                      noDataMessage: msgNotRegEnc
                  }, "gridProgramacaoPPT");
                  gridProgramacaoPPT.on("StyleRow", function (row) {
                      try {
                          var item = gridProgramacaoPPT.getItem(row.index);
                          if (row.node.children[0].children[0].children[0].children[7].innerHTML != '...' && eval(row.node.children[0].children[0].children[0].children[7].innerHTML))
                              row.customClasses += " YellowRow";
                          if (row.node.children[0].children[0].children[0].children[8].innerHTML != '...' && hasValue(row.node.children[0].children[0].children[0].children[8].innerHTML) //se tem codigo do feriado.
                                  && (row.node.children[0].children[0].children[0].children[9].innerHTML == '...' || !hasValue(row.node.children[0].children[0].children[0].children[9].innerHTML)) //não tem código do desconsidera feriado do banco.
                                  && row.node.children[0].children[0].children[0].children[10].innerHTML != "true")//não tem codigo do desconsidera feriado de memória.
                              row.customClasses += " GreenRow";
                          if (row.node.children[0].children[0].children[0].children[11].innerHTML != '...' && eval(row.node.children[0].children[0].children[0].children[11].innerHTML) == PROGRAMACAO_MANUAL)
                              row.customClasses += " OrangeRow";
                          if (item != null && item.id_prog_cancelada)
                              row.customClasses += " RedRow"
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  });
                  require(["dojo/aspect","dojo/_base/array", "dijit/registry"], function (aspect, Array, registry) {
                      aspect.after(gridProgramacaoPPT, "_onFetchComplete", function () {
                          // Configura o check de todos:
                          if (dojo.byId('selecionaTodosProgramacao').type == 'text')
                              setTimeout("configuraCheckBox(false, 'nm_aula_programacao_turma', 'selecionadoProgramacao', -1, 'selecionaTodosProgramacao', 'selecionaTodosProgramacao', 'gridProgramacaoPPT')", gridProgramacaoPPT.rowsPerPage * 3);
                          setTimeout(function () {
                              Array.forEach(itensSelecionadosCheckGridPPT, function (value, i) {
                                      if (hasValue(dijit.byId(value.id))) {registry.byId(value.id).set('checked', value.checked);}
                                  });
                              },
                              gridProgramacaoPPT.rowsPerPage );
                      });
                  });

                  gridProgramacaoPPT.canSort = false;
                  gridProgramacaoPPT.startup();
                  gridProgramacaoPPT.on("RowDblClick", function (evt) {
                      try {
                          var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                          apresentaMensagem('apresentadorMensagemTurmaPPT', '');
                          document.getElementById("divAlterarProg").style.display = "";
                          document.getElementById("divCancelarProg").style.display = "";
                          document.getElementById("divIncluirProg").style.display = "none";
                          keepValuesProgramacao(item, gridProgramacaoPPT, false);
                          dijit.byId("dialogProgramacao").show();
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  }, true);

                  var dataStoreDesconsidera = new ObjectStore({ objectStore: new Memory({ data: dataFeriadosDescon }) });

                  destroyCreateFeriadoDesconsideradoPPTFilha();

                  var gridDesconsideraFeriadosPPT = new EnhancedGrid({
                      store: dataStoreDesconsidera,
                      structure:
                          [
                            { name: "<input id='selecionaTodosFeriadoDesconsiderado' style='display:none'/>", field: "selecionadoFeriadoDesconsiderado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFeriadoDesconsiderado },
                            { name: "", field: "nm_feriado_desconsiderado", styles: "display:none;" },
                            { name: "", field: "cd_feriado_desconsiderado", styles: "display:none;" },
                            { name: "Data Inicial", field: "dta_inicial", width: "40%" },
                            { name: "Data Final", field: "dta_final", width: "40%" },
                            { name: "Programação", field: "dc_programacao_feriado", width: "20%" },
                            { name: "", field: "id_aula_dada", styles: "display:none;" }
                          ],
                      noDataMessage: msgNotRegEnc,
                      plugins: {
                          pagination: {
                              pageSizes: ["6", "12", "30", "100", "All"],
                              description: true,
                              sizeSwitch: true,
                              pageStepper: true,
                              defaultPageSize: "10",
                              gotoButton: true,
                              /*page step to be displayed*/
                              maxPageStep: 4,
                              /*position of the pagination bar*/
                              position: "button"
                          }
                      }
                  }, "gridDesconsideraFeriadosPPT");
                  gridDesconsideraFeriadosPPT.rowsPerPage = 5000;
                  gridDesconsideraFeriadosPPT.pagination.plugin._paginator.plugin.connect(gridDesconsideraFeriadosPPT.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                      verificaMostrarTodos(evt, gridDesconsideraFeriadosPPT, 'nm_feriado_desconsiderado', 'selecionaTodosFeriadoDesconsiderado');
                  });
                  require(["dojo/aspect"], function (aspect) {
                      aspect.after(gridDesconsideraFeriados, "_onFetchComplete", function () {
                          // Configura o check de todos:
                          if (dojo.byId('selecionaTodosProgramacao').type == 'text')
                              setTimeout("configuraCheckBox(false, 'nm_feriado_desconsiderado', 'selecionadoFeriadoDesconsiderado', -1, 'selecionaTodosFeriadoDesconsiderado', 'selecionaTodosFeriadoDesconsiderado', 'gridDesconsideraFeriados')", gridDesconsideraFeriados.rowsPerPage * 3);
                      });
                  });

                  gridDesconsideraFeriadosPPT.canSort = false;
                  gridDesconsideraFeriadosPPT.startup();
                  gridDesconsideraFeriadosPPT.on("RowDblClick", function (evt) {
                      try {
                          var idx = evt.rowIndex,
                              item = this.getItem(idx),
                              store = this.store;
                          apresentaMensagem('apresentadorMsgDesconsideracao', '');
                          document.getElementById("divAlterarDesc").style.display = "";
                          document.getElementById("divCancelarDesc").style.display = "";
                          document.getElementById("divIncluirDesc").style.display = "none";
                          document.getElementById("divLimparDesc").style.display = "none";
                          gridDesconsideraFeriadosPPT.itemSelecionado = item;
                          keepValuesFeriadoDesconsiderado(gridDesconsideraFeriadosPPT, false);
                          dijit.byId("dialogDesconsideracao").show();
                      }
                      catch (e) {
                          postGerarLog(e);
                      }
                  }, true);

                  dijit.byId("tagDesconsiderarFeriados").on("Show", function (e) {
                      dijit.byId('paiDesconsideraFeriados').resize();
                      dijit.byId('gridDesconsideraFeriadosPPT').update();
                  });

                  if (!hasValue(dijit.byId("acoesRelacionadasProgramacaoPPTFilha"))) {
                      var menuProgramacao = new DropDownMenu({ style: "height: 25px" });
                      var acaoExcluirProgramacao = new MenuItem({
                          label: "Excluir",
                          onClick: function () { deletarItemSelecionadoGridProgramacao(Memory, ObjectStore, 'nm_aula_programacao_turma', dijit.byId("gridProgramacaoPPT"), 'apresentadorMensagemTurmaPPT'); }
                      });
                      menuProgramacao.addChild(acaoExcluirProgramacao);

                      var acaoEditarProgramacao = new MenuItem({
                          label: "Editar",
                          onClick: function () { eventoEditaProgramacao(gridProgramacaoPPT.itensSelecionados); }
                      });
                      menuProgramacao.addChild(acaoEditarProgramacao);

                      //var acaoCancelarProgramacao = new MenuItem({
                      //    label: "Cancelar",
                      //    onClick: function () {
                      //        eventoCancelaProgramacao(dijit.byId("gridProgramacaoPPT"), dijit.byId("gridProgramacaoPPT").itensSelecionados);
                      //    }
                      //});
                      //menuProgramacao.addChild(acaoCancelarProgramacao);

                      var acaoCancelarProgramacao = new MenuItem({
                          label: "Desfazer Cancelar",
                          onClick: function () {
                              eventoDesfazerCancelarProgramacao(dijit.byId("gridProgramacaoPPT"), dijit.byId("gridProgramacaoPPT").itensSelecionados);
                          }
                      });
                      menuProgramacao.addChild(acaoCancelarProgramacao);

                      menuProgramacao.addChild(new MenuSeparator());
                      var acaoTelaProgramacao = new MenuItem({
                          label: "Gerar Modelo de Programação",
                          onClick: function () { gerarModeloProgramacao(xhr, ref, 'apresentadorMensagemTurmaPPT'); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      acaoTelaProgramacao = new MenuItem({
                          label: "Programação Automática pelo Curso",
                          onClick: function () { incluirProgramacaoCurso(xhr, 'gridProgramacaoPPT', dijit.byId('gridDesconsideraFeriadosPPT'), ref, 'apresentadorMensagemTurmaPPT', false); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      acaoTelaProgramacao = new MenuItem({
                          label: "Programação Automática pelo Modelo",
                          onClick: function () { incluirProgramacaoCurso(xhr, 'gridProgramacaoPPT', dijit.byId('gridDesconsideraFeriadosPPT'), ref, 'apresentadorMensagemTurmaPPT', true); }
                      });
                      menuProgramacao.addChild(acaoTelaProgramacao);

                      var buttonProgramacao = new DropDownButton({
                          label: "Ações Relacionadas",
                          name: "acoesRelacionadasProgramacaoPPTFilha",
                          dropDown: menuProgramacao,
                          id: "acoesRelacionadasProgramacaoPPTFilha"
                      });
                      dojo.byId("linkAcoesProgramacaoPPT").appendChild(buttonProgramacao.domNode);

                      new Button({
                          label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                              try {
                                  document.getElementById("divAlterarProg").style.display = "none";
                                  document.getElementById("divCancelarProg").style.display = "none";
                                  document.getElementById("divIncluirProg").style.display = "";
                                  dijit.byId('aulaProgramacao').set('disabled', false);
                                  incluirProgramacao(xhr, dijit.byId("gridProgramacaoPPT"), dijit.byId('gridDesconsideraFeriadosPPT'), ref, 'apresentadorMensagemTurmaPPT');
                              }
                              catch (e) {
                                  postGerarLog(e);
                              }
                          }
                      }, "btnNovaProgramacaoPPT");
                      new Button({
                          label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                              incluirFeriadoDesconsiderado();
                          }
                      }, "btnNovaDesconsideracaoPPT");
                      if (!hasValue(dijit.byId('alterarDescon')))
                          new Button({
                              label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                  alterarDesconsideraFeriados(xhr);
                              }
                          }, "alterarDescon");

                      if (!hasValue(dijit.byId('incluirDescon')))
                          new Button({
                              label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                                  incluirDesconsiderarFeriado(xhr);
                              }
                          }, "incluirDescon");

                      if (!hasValue(dijit.byId('limparDescon')))
                          new Button({
                              label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                                  clearForm("formFeriadoDesconsiderado");
                              }
                          }, "limparDescon");

                      if (!hasValue(dijit.byId('fecharDescon')))
                          new Button({
                              label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () {
                                  dijit.byId('dialogDesconsideracao').hide();
                              }
                          }, "fecharDescon");
                  }

                  if (!hasValue(dijit.byId('cancelarDescon'))) {
                      var cancelarDescon = new Button({
                          label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent'
                      }, "cancelarDescon");
                      if (hasValue(cancelarDescon.handler))
                          cancelarDescon.handler.remove();
                      cancelarDescon.handler = cancelarDescon.on("click", function (e) {
                          keepValuesFeriadoDesconsiderado(dijit.byId('gridDesconsideraFeriadosPPT'), false);
                      });
                  }
                  else {
                      var cancelarDescon = dijit.byId("cancelarDescon");
                      if (hasValue(cancelarDescon.handler))
                          cancelarDescon.handler.remove();
                      cancelarDescon.handler = cancelarDescon.on("click", function (e) {
                          keepValuesFeriadoDesconsiderado(dijit.byId('gridDesconsideraFeriadosPPT'), false);
                      });
                  }
              }
              catch (e) {
                  postGerarLog(e);
              }
          });
      });
}

//#region C.R.U.D TurmaPPT - limparTurmaPPT -  keepValuesTurmaPPT - limparTodosHorariosTurmaPPT - salvarTurmaFilhaPPT
function limparTurmaPPT(Memory) {
    try {
        var gridAlunosPPTFilha = dijit.byId("gridAlunosPPTFilha");
        var gridProfessorPPT = dijit.byId("gridProfessorPPT");
        var gridProgramacaoPPT = dijit.byId("gridProgramacaoPPT");
        //var gridProgramacao = dijit.byId("gridProgramacao");
        dojo.byId("escolaturma").value = 0;
        dojo.byId('cd_turmaPPT').value = 0;
        clearForm("formTurmaPrincipalPPT");
        dijit.byId("pesCadCursoPPT")._onChangeActive = false;
        dijit.byId("pesCadCursoPPT").reset();
        dijit.byId("pesCadCursoPPT")._onChangeActive = true;

        dijit.byId("pesCadRegimePPT").reset();

        dijit.byId("pesCadProdutoPPT")._onChangeActive = false;
        dijit.byId("pesCadProdutoPPT").reset();
        dijit.byId("pesCadProdutoPPT")._onChangeActive = true;

        dijit.byId("pesCadDuracaoPPT")._onChangeActive = false;
        dijit.byId("pesCadDuracaoPPT").reset();
        dijit.byId("pesCadDuracaoPPT")._onChangeActive = true;

        dijit.byId('dtIniAulaPPT').dta_inicio_aula_anterior = null;
        dijit.byId('pesCadDuracaoPPT').dta_inicio_aula_anterior = null;

        dijit.byId("dtIniAulaPPT").reset();
        dijit.byId("dtFimAulaPPT").reset();
        destroyCreateGridHorarioPPT();
        destroyCreateGridProgramacaoPPTFilha();
        if (hasValue(gridAlunosPPTFilha)) {
            gridAlunosPPTFilha.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridAlunosPPTFilha.itensSelecionados = [];
        }
        if (hasValue(gridProfessorPPT)) {
            gridProfessorPPT.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }));
            gridProfessorPPT.itensSelecionados = [];
        }
        dijit.byId("tagAlunosPPTFilha").set("open", false);
        dijit.byId("tagProfessoresPPT").set("open", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValuesTurmaPPT(itensSelecionados, ready, Memory, filteringSelect) {
    try {
        dijit.byId("btnNovoProfessor").set("disabled", itensSelecionados[0].cd_pessoa_escola != dojo.byId('_ES0').value);
        var cdTurma = dojo.byId("cd_turma").value;
        var pesCadProduto = dijit.byId("pesCadProduto");
        var gridProfessor = dijit.byId("gridProfessor");
        dojo.byId('cd_turmaPPT').value = itensSelecionados[0].cd_turma;
        dojo.byId("noTurmaPPT").value = itensSelecionados[0].no_turma;
        dojo.byId("noApelidoPPT").value = itensSelecionados[0].no_apelido;
        dijit.byId("nrAulasProgPPT").set("value", itensSelecionados[0].nro_aulas_programadas)
        //dojo.byId("escolaturma").value = itensSelecionados[0].cd_pessoa_escola;
        //displayedValue
        criarOuCarregarCompFiltering("pesCadCursoPPT", [{ cd_curso: itensSelecionados[0].cd_curso, no_curso: itensSelecionados[0].no_curso }], "", itensSelecionados[0].cd_curso, ready, Memory, filteringSelect, 'cd_curso', 'no_curso');
        if (hasValue(pesCadProduto.value))
            criarOuCarregarCompFiltering("pesCadProdutoPPT", [{ cd_produto: pesCadProduto.value, no_produto: pesCadProduto.displayedValue }], "", pesCadProduto.value, ready, Memory, filteringSelect, 'cd_produto', 'no_produto');
        if (hasValue(itensSelecionados[0].cd_duracao))
            dijit.byId("pesCadDuracaoPPT").set("value", itensSelecionados[0].cd_duracao);
        if (hasValue(itensSelecionados[0].cd_regime))
            criarOuCarregarCompFiltering("pesCadRegimePPT", [{ cd_regime: itensSelecionados[0].cd_regime, no_regime: itensSelecionados[0].no_regime }], "", itensSelecionados[0].cd_regime, ready, Memory, filteringSelect, 'cd_regime', 'no_regime');
        //dojo.byId("noTurmaPPT").value = dojo.byId("noTurma").value;
        //dojo.byId("nrCadSalaPPT").value = dojo.byId("nrCadSala").value;
        if (hasValue(itensSelecionados[0].dta_inicio_aula)) {
            dojo.byId("dtIniAulaPPT").value = itensSelecionados[0].dta_inicio_aula;
            dijit.byId('dtIniAulaPPT').dta_inicio_aula_anterior = itensSelecionados[0].dta_inicio_aula;
        }
        if (hasValue(itensSelecionados[0].dta_final_aula))
            dojo.byId("dtFimAulaPPT").value = itensSelecionados[0].dta_final_aula;
        if (hasValue(itensSelecionados[0].dta_termino_turma))
            dojo.byId("dtaTerminoPPT").value = itensSelecionados[0].dta_termino_turma;
        dijit.byId("idAulaExterna").set("checked", dijit.byId("idAulaExterna").get("checked"));
        dijit.byId("gridAlunosPPTFilha").setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: itensSelecionados }) }));
        if (hasValue(itensSelecionados[0].ProfessorTurma) && hasValue(itensSelecionados[0].ProfessorTurma.length > 0))
            dijit.byId("gridProfessorPPT").setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: clone(itensSelecionados[0].ProfessorTurma) }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTodosHorariosTurmaPPT() {
    try {
        var gradeHorariosPPT = dijit.byId('gridHorariosPPT');
        var store = gradeHorariosPPT.items;
        if (store.length > 0) {
            for (var i = store.length - 1; i >= 0; i--) {
                if (gradeHorariosPPT.items[i].id >= 0)
                    //gradeHorariosPPT.store.remove(store[i].id);
                    removeHorario(gradeHorariosPPT, store[i].id);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarTurmaFilhaPPT() {
    try {
        var gridProgramacaoPPT = dijit.byId("gridProgramacaoPPT");
        var gridDesconsideraFeriadosPPT = dijit.byId("gridDesconsideraFeriadosPPT");
        var gridHorariosFilhaPPT = dijit.byId("gridHorariosPPT");
        var gridAlunosPPT = dijit.byId("gridAlunosPPT");
        var gridProfessoresPPT = dijit.byId("gridProfessorPPT");
        if (hasValue(gridProgramacaoPPT)) {
            var storeGridProgPPT = gridProgramacaoPPT.store.objectStore.data;
            if (storeGridProgPPT != null) {

                for (var i = 0; i < storeGridProgPPT.length; i++) {
                    var index = "id_mostrar_calendario_Selected_" + storeGridProgPPT[i].nm_aula_programacao_turma;
                    if (!hasValue(storeGridProgPPT[i].id_mostrar_calendario)) // id_mostrar_calendario pode vir 'undefined'
                        storeGridProgPPT[i].id_mostrar_calendario = false;

                    if (hasValue(dijit.byId(index))) {
                        if (storeGridProgPPT[i].id_mostrar_calendario != dijit.byId(index).get("checked")) {
                            storeGridProgPPT[i].id_mostrar_calendario = dijit.byId(index).get("checked");
                            storeGridProgPPT[i].programacaoTurmaEdit = true;
                        }
                    }
                }

                gridAlunosPPT.itensSelecionados[0].ProgramacaoTurma = storeGridProgPPT;
                gridAlunosPPT.itensSelecionados[0].alterouProgramacaoOuDescFeriado = true;
            }
        }
        if (hasValue(gridHorariosFilhaPPT)) {
            var storeGridHorariosFilhaPPT = gridHorariosFilhaPPT.params.store.data;
            if (storeGridHorariosFilhaPPT != null)
                gridAlunosPPT.itensSelecionados[0].horariosTurma = getHorariosOcupadosTurma(storeGridHorariosFilhaPPT);
        }
        if (hasValue(gridProfessoresPPT)) {
            var storeGridProfFilhaPPT = gridProfessoresPPT.store.objectStore.data;
            if (storeGridProfFilhaPPT != null)
                gridAlunosPPT.itensSelecionados[0].ProfessorTurma = storeGridProfFilhaPPT;
        }
        //Verifica se não deletou o último registro de horário.
        if (!verificarHorarioAlunoProfessorTurma(VERIFITURMAFILHAPPT2MODAL, "apresentadorMensagemTurmaPPT"))
            return false;
        gridAlunosPPT.itensSelecionados[0].cd_duracao = dijit.byId("pesCadDuracaoPPT").value;
        gridAlunosPPT.itensSelecionados[0].dt_inicio_aula = hasValue(dojo.byId("dtIniAulaPPT").value) ? dojo.date.locale.parse(dojo.byId("dtIniAulaPPT").value,
                                                                                                                           { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        gridAlunosPPT.itensSelecionados[0].dta_inicio_aula = dojo.byId("dtIniAulaPPT").value;
        gridAlunosPPT.itensSelecionados[0].cd_sala = hasValue(dijit.byId("cbSala").value) ? dijit.byId("cbSala").value : null;
        gridAlunosPPT.itensSelecionados[0].cd_sala_online = hasValue(dijit.byId("cbSalaOnLine").value) ? dijit.byId("cbSalaOnLine").value : null;

        gridAlunosPPT.itensSelecionados[0].no_apelido = dojo.byId("noApelidoPPT").value;
        if (hasValue(gridDesconsideraFeriadosPPT)) {
            var storeGridDescPPT = gridDesconsideraFeriadosPPT.store.objectStore.data;
            if (storeGridDescPPT != null) {
                gridAlunosPPT.itensSelecionados[0].FeriadosDesconsiderados = storeGridDescPPT;
                gridAlunosPPT.itensSelecionados[0].alterouProgramacaoOuDescFeriado = true;
            }
        }
        if (hasValue(gridAlunosPPT.listaCompletaAlunos)) {
            for (var i = 0; i < gridAlunosPPT.listaCompletaAlunos.length; i++) {
                if (gridAlunosPPT.listaCompletaAlunos[i].cd_aluno == gridAlunosPPT.itensSelecionados[0].cd_aluno && 
                    (gridAlunosPPT.listaCompletaAlunos[i].cd_turma == gridAlunosPPT.itensSelecionados[0].cd_turma)) {
                    gridAlunosPPT.listaCompletaAlunos[i] = jQuery.extend({}, gridAlunosPPT.itensSelecionados[0]);
                    break;
                }
            }
        }
        dijit.byId("cadTurmaPPT").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

//#region testa se o item editado esta sobrepondo algum horário ocupado da sala ou professor.
function testarHorarioGrid(item) {
    try {
        var mensagensWeb = new Array();
        var mensagem = '';
        var mergeAceito = true;
        var gridHorario = hasValue(dijit.byId("gridHorarios")) ? dijit.byId("gridHorarios") : [];
        var gridTurma = hasValue(dijit.byId('gridTurma')) ? dijit.byId(dijit.byId('gridTurma')) : [];
        var horarioSalaInicial = null;
        var horarioSalaFinal = null;

        var hIni = (new Date(item.startTime)).getHours();
        var hFim = (new Date(item.endTime)).getHours();
        var mIni = (new Date(item.startTime)).getMinutes();
        var mFim = (new Date(item.endTime)).getMinutes();

        var totalMinitosInicial = (parseInt(hIni) * 60) + parseInt(mIni);
        var totalMinutosFinal = (parseInt(hFim) * 60) + parseInt(mFim);

        if (gridHorario != null && gridHorario.store.data.length > 0) {
            var itemMergeError = false;
            for (var i = 0; i < gridHorario.store.data.length; i++) {
                //Verifica e á interseção com os horários ocupados do professor.
                if ((gridHorario.store.data[i].calendar == "Calendar3") && (gridHorario.store.data[i].id_dia_semana == item.id_dia_semana)) {
                    horarioSalaInicial = trasformarHorasEmMinutos(gridHorario.store.data[i].dt_hora_ini);
                    horarioSalaFinal = trasformarHorasEmMinutos(gridHorario.store.data[i].dt_hora_fim);
                    if (((horarioSalaInicial <= totalMinitosInicial) && (horarioSalaFinal > totalMinitosInicial))
                        || ((horarioSalaInicial < totalMinutosFinal) && (horarioSalaFinal > totalMinutosFinal))
                        || ((horarioSalaInicial > totalMinitosInicial) && (horarioSalaFinal < totalMinutosFinal))) {

                        mensagem = gridHorario.store.data[i].calendar == "Calendar4" ? msgHorarioOcupadoProfessor : msgHorarioOcupadoSala;
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagem);
                        apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
                        mergeAceito = false;
                        itemMergeError = true;
                        if (item.cd_horario == 0)
                            //gridHorario.store.remove(item.id);
                            removeHorario(gridHorario, item.id);
                        else {
                            getHorarioOriginalEditado(item.cd_horario, item.id);
                        }
                    }//end if 2°
                }
                //else
                //    //Verifica e á interseção com os horários ocupados da sala.
                //    if ((gridHorario.store.data[i].calendar == "Calendar4") && (gridHorario.store.data[i].id_dia_semana == item.id_dia_semana)) {
                //    }
                if (itemMergeError)
                    break;
            }
        }
        return mergeAceito;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatHorarioBDParaHorariosGrid(listHorarios, calendar) {
    try {
        var retorno = [];
        $.each(listHorarios, function (idx, val) {
            //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
            val.calendar = calendar;
            val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
            val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
            retorno.push(val);
        });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function AlunoProfessorTurma(horario, validarProgramacao, tipoTurma, tipoVerif) {
    try {
        var cd_turmaNormal = dojo.byId("cd_turma").value;
        var cd_turmaPtt = dojo.byId("cd_turmaPPT").value;
        var profs = [];
        var alunos = [];
        var gridAlunos = dijit.byId("gridAlunos");
        var gridAlunosPPTFilha = dijit.byId("gridAlunosPPTFilha");
        if (tipoTurma != TURMA_PPT_FILHA) {
            var gridProf = dijit.byId("gridProfessor");
            //profs = grid.store.objectStore.data;
            if (gridProf != null && hasValue(gridProf.store.objectStore.data))
                for (var i = 0; i < gridProf.store.objectStore.data.length; i++) {
                    if (hasValue(gridProf) && hasValue(gridProf.store.objectStore.data)) {
                        profs[i] = gridProf.store.objectStore.data[i].cd_professor;
                    }
                }
        }
        if (gridAlunos != null && hasValue(gridAlunos.listaCompletaAlunos))
            for (var i = 0; i < gridAlunos.listaCompletaAlunos.length; i++) {
                if (hasValue(gridAlunos) && hasValue(gridAlunos.listaCompletaAlunos))
                    alunos[i] = gridAlunos.listaCompletaAlunos[i];
            }
        //Prever a ppt tambem
        if (tipoTurma == TURMA_PPT_FILHA) {
            if (tipoVerif == VERIFITURMAFILHAPPT2MODAL) {
                this.cd_turma = dijit.byId("gridAlunosPPT").listaCompletaAlunos[0].cd_turma;
                if (gridAlunosPPTFilha != null && hasValue(gridAlunosPPTFilha.store.objectStore.data))
                    for (var i = 0; i < gridAlunosPPTFilha.store.objectStore.data.length; i++) {
                        if (hasValue(gridAlunosPPTFilha) && hasValue(gridAlunosPPTFilha.store.objectStore.data)) {
                            alunos[i] = gridAlunosPPTFilha.store.objectStore.data[i];
                        }
                    }
            } else
                this.cd_turma = cd_turmaNormal;
        }
        switch (parseInt(tipoTurma)) {
            case TURMA_PPT_FILHA:
                this.cd_turma = cd_turmaPtt;
                break;
            case TURMA_PPT:
                this.cd_turma_ppt = cd_turmaNormal;
                this.cd_turma = cd_turmaNormal;
                break;
            case TURMA:
                this.cd_turma = cd_turmaNormal;
                break;
        }
        this.horario = horario;
        this.alunos = alunos;
        this.professores = profs;
        this.validarProgramacao = validarProgramacao;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function HorariosProgramacao(horarios) {
    try {
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var cd_turmaNormal = dojo.byId("cd_turma").value;
        var cd_turmaPtt = dojo.byId("cd_turmaPPT").value;
        if (tipoTurma == TURMA_PPT)
            this.cd_turma_ppt = cd_turmaPtt;
        else
            this.cd_turma = cd_turmaNormal;
        this.horarios = [];
        this.alunos = [];
        this.professores = [];
        this.validarProgramacao = true;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Metodo verifica a disponibilidade do item para ser deletado.(professores/Alunos)
function verificarHorarioAlunoProfessorTurma(tipoVerif, msg) {
    try {
        var mensagensWeb = new Array();
        var gridHorario = null, alunos = null, professores = null;
        var totalHorarios = 0, retorno = true;
        var itemSelected = new Object;
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        if (tipoVerif == VERIFTURMANORMAL) {
            gridHorario = validaRetornaHorarios(tipoTurma);
            if (tipoTurma != TURMA_PPT)
                alunos = dijit.byId("gridAlunos").listaCompletaAlunos.length;
            else
                alunos = dijit.byId("gridAlunosPPT").listaCompletaAlunos.length;
            professores = dijit.byId("gridProfessor").store.objectStore.data.length;
            if (hasValue(gridHorario))
                for (var i = gridHorario.length - 1; i >= 0; i--) {
                    itemSelected = gridHorario[i];
                    if (itemSelected.calendar == "Calendar2" || itemSelected.calendar == "Calendar3")
                        totalHorarios++;
                }
        } else {
            gridHorariosPPT = validaRetornaHorariosFIlhaPPT();
            alunos = dijit.byId("gridAlunosPPTFilha").store.objectStore.data.length;
            professores = dijit.byId("gridProfessorPPT").store.objectStore.data.length;
            if (hasValue(gridHorariosPPT))
                for (var i = gridHorariosPPT.length - 1; i >= 0; i--) {
                    itemSelected = gridHorariosPPT[i];
                    if (!hasValue(itemSelected.calendar) || itemSelected.calendar == "Calendar2")
                        totalHorarios++;
                }
        }

        //Verificação se e o ultimo horário da turma e se existe professores ou alunos na turma,pois não sera possivel excluir os horários.
        if ((totalHorarios == 0 && alunos > 0) || (totalHorarios == 0 && professores > 0)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDeleteHorariosComAlunoProfessorTurma);
            apresentaMensagem(msg, mensagensWeb);
            retorno = false;
        }
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function getHorarioOriginalEditado(cd_horario, id) {
    try {
        var gridHorario = hasValue(dijit.byId("gridHorarios")) ? dijit.byId("gridHorarios") : [];
        var gridTurma = hasValue(dijit.byId('gridTurma')) ? dijit.byId(dijit.byId('gridTurma')) : [];
        if (gridTurma != null && gridTurma.TurmaEdit != null && gridTurma.TurmaEdit.horariosTurma != null)
            for (var j = 0; j < gridTurma.TurmaEdit.horariosTurma.length; j++) {
                var itemEncontrado = false;
                if (cd_horario == gridTurma.TurmaEdit.horariosTurma[j].cd_horario) {
                    var addHorarios = formatHorarioBDParaHorariosGrid([gridTurma.TurmaEdit.horariosTurma[j]], 'Calendar2')
                    removeHorario(gridHorario, id);
                    addHorarios[0].id = geradorIdHorarios(gridHorario);
                    addItemHorariosEdit(addHorarios[0].id, gridHorario);
                    gridHorario.store.add(addHorarios[0]);
                    itemEncontrado = true;
                }
                if (itemEncontrado)
                    break;
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getHorarioOriginalEditadoFilhaPPTModal(cd_horario, id) {
    try {
        var gridHorario = hasValue(dijit.byId("gridHorariosPPT")) ? dijit.byId("gridHorariosPPT") : [];
        var gridTurmaPPT = hasValue(dijit.byId('gridAlunosPPT')) ? dijit.byId(dijit.byId('gridAlunosPPT')) : [];
        for (var j = 0; j < gridTurmaPPT.itensSelecionados[0].horariosTurma.length; j++) {
            var itemEncontrado = false;
            if (cd_horario == gridTurmaPPT.itensSelecionados[0].horariosTurma[j].cd_horario) {
                var addHorarios = formatHorarioBDParaHorariosGrid([gridTurmaPPT.itensSelecionados[0].horariosTurma[j]], 'Calendar2')
                removeHorario(gridHorario, id);
                addHorarios[0].id = geradorIdHorarios(gridHorario);
                addItemHorariosEdit(addHorarios[0].id, gridHorario);
                gridHorario.store.add(addHorarios[0]);
                itemEncontrado = true;
            }
            if (itemEncontrado)
                break;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function geradorIdHorarios(gradeHorarios) {
    try {
        var id = gradeHorarios.items.length;
        var itensArray = gradeHorarios.items.sort(function byOrdem(a, b) { return a.id - b.id; });
        if (id > 0)
            id = itensArray[id - 1].id + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function confLayoutTurma(tipoTurma, operacao, xhr, ready, Memory, FilteringSelect, DropDownMenu, MenuItem, DropDownButton) {
    try {
        if (tipoTurma == TURMA) {
            dojo.byId("lblSalaPPTPai").style.display = "none";
            dojo.byId("tdSalaPPTPai").style.display = "none";
            dojo.byId("trSalaHorario").style.display = "";
            dojo.byId("novoAlunoTurma").style.display = "";
            dojo.byId("acoesFilha").style.display = "none";
            dojo.byId("acoesAluno").style.display = "";
            if (operacao == NOVO) {
                dijit.byId("pesCadProduto").set("disabled", false);
                //dijit.byId("pesCadCurso").set("disabled", true);
                dijit.byId("pesCadRegime").set("disabled", false);
                dijit.byId("pesCadDuracao").set("disabled", false);
                dijit.byId("dtIniAula").set("disabled", false);
                dojo.byId("grupoAcoesProf").style.display = "";
                dojo.byId("grupoAcoesAlunos").style.display = "";
                dojo.byId("trTurmaPPT").style.display = "none";
                dojo.byId("tbCriarHorarios").style.display = "";


            } else {
                //dijit.byId("pesCadProduto").set("disabled", true);
                //dijit.byId("pesCadCurso").set("disabled", true);
                dijit.byId("pesCadRegime").set("disabled", true);
                //dijit.byId("pesCadDuracao").set("disabled", true);
                dijit.byId("idTurmaPPTPai").set("disabled", true);
                //dijit.byId("dtIniAula").set("disabled", true);
                dojo.byId("grupoAcoesProf").style.display = "";
                dojo.byId("grupoAcoesAlunos").style.display = "";
                dojo.byId("trTurmaPPT").style.display = "none";
                dojo.byId("tbCriarHorarios").style.display = "";
                confLayoutBotoesFormProfessor(false);
                dojo.byId("acoesFilha").style.display = "none";
                dojo.byId("acoesAluno").style.display = "";
            }
        } else {
            if (operacao == NOVO) {
                dojo.byId("grupoAcoesAlunos").style.display = "";
                dojo.byId("novoAlunoTurma").style.display = "";
            }
            else {
                dojo.byId("novoAlunoTurma").style.display = "none";


                dojo.byId("acoesFilha").style.display = "";
                dojo.byId("acoesAluno").style.display = "none";
            }
            //dojo.byId("grupoAcoesAlunos").style.display = "none";
            //dojo.byId("tbCriarHorarios").style.display = "none";
            dojo.byId("trSalaHorario").style.display = "none";

            //dojo.byId("tbCriarHorarios").style.display = "none";
            //dijit.byId("pesCadProduto").set("disabled", true);
            dijit.byId("pesCadRegime").set("disabled", true);
            //dijit.byId("pesCadDuracao").set("disabled", true);
            //dijit.byId("dtIniAula").set("disabled", true);
            dijit.byId("idTurmaPPTPai").set("disabled", true);
            //dijit.byId("pesCadProduto").set("disabled", true);
            //dojo.byId("grupoAcoesProf").style.display = "none";
            confLayoutBotoesFormProfessor(true);
            dojo.byId("lblSalaPPTPai").style.display = "";
            dojo.byId("tdSalaPPTPai").style.display = "";
            dojo.byId("trTurmaPPT").style.display = "";
            if (operacao == EDIT) {
                //dijit.byId("pesCadCurso").set("disabled", true);
                dijit.byId("pesProTurmaFK").set("disabled", true);
            } else {
                //dijit.byId("pesCadCurso").set("required", true);
                if (hasValue(dijit.byId("pesCadProduto").value)) {
                    configuraCursoPorProdutoTurma(dijit.byId("pesCadProduto").value, xhr, ready, Memory, FilteringSelect);
                }
            }
            confLayoutBotInclAluno();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function confLayoutBotoesFormProfessor(semafaro) {
    try {
        dijit.byId("alterarProf").set("disabled", semafaro);
        dijit.byId("cancelarProf").set("disabled", semafaro);
        dijit.byId("fecharProf").set("disabled", semafaro);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function confirmar(xhr, ObjectStore, Memory, ref, Cache, JsonRest, Button, itens, FilteringSelect, ready) {
    try {
        var turmas = [];
        for (var i = 0; i < itens.length; i++)
            turmas[i] = {
                cd_turma: itens[i].cd_turma,
                dt_termino: dojo.byId("dt_fim_enc").value
            }
        dataDiaEnc(xhr, ready, Memory, FilteringSelect, 0, 2, function () { confirmaEncTurma(xhr, ref, turmas); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function confirmaEncTurma(xhr, ref, turmas, cdTurma, value) {
    if (turmas != null && turmas.length > 0 && document.getElementById("ckViradaTurma").checked == false || document.getElementById("ckViradaTurma").checked == null) {
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/turma/postUpdateTurmaEnc",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson(turmas)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridTurma';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    for (var i = 0; i < turmas.length; i++)
                        removeObjSort(grid.itensSelecionados, "cd_turma", turmas[i].cd_turma);
                    for (var i = 0; i < itemAlterado.length; i++)
                        insertObjSort(grid.itensSelecionados, "cd_turma", itemAlterado[i]);
                    hideCarregando();
                    buscarItensSelecionados(gridName, 'turmaSelecionada', 'cd_turma', 'selecionaTodos', ['pesquisarTurma', 'relatorioTurma'], 'todosItens');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_turma");
                    dijit.byId("dialogEncerramento2").hide();
                    dijit.byId("dialogEncerramento3").show();
                    if (turmas.length > 1)
                        dijit.byId("novaTurmaEnc").set("disabled", true);
                    else
                        dijit.byId("novaTurmaEnc").set("disabled", false);
                }
                else {
                    hideCarregando();
                    apresentaMensagem('apresentadorMensagemEncConf', data);
                }
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
            }
        },
        function (error) {
            hideCarregando();
            var data = (hasValue(error) && hasValue(error.response)) ? error.response.data : '';
            var isString = (data != "" && !hasValue(data.MensagensWeb))
            if (data != "")
                data = JSON.parse(data);
            if (!hasValue(data.MensagensWeb)) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroGeralUpdateTurmaEnc);
                apresentaMensagem('apresentadorMensagemEncConf', mensagensWeb);
            } if (isString) {
                apresentaMensagem('apresentadorMensagemEncConf', data.MensagensWeb);
            }
            else {
                apresentaMensagem('apresentadorMensagemEncConf', error);
            }
            
        });
    } else {
        var cdTurma = null;
        dijit.byId("dialogEncerramentoNovaturma").show();
    }
}

function proximoWizard(xhr, ready, Memory, FilteringSelect) {
    dataDiaEnc(xhr, ready, Memory, FilteringSelect, 0, 1, function () { proximaWizardCont(); });
}

function proximaWizardCont() {
    try {
        if (!dijit.byId("dialogEncerramento").validate())
            return false;
        dijit.byId("dialogEncerramento").hide();
        dijit.byId("dialogEncerramento2").show();
        if (hasValue(dijit.byId("gridConfirmacaoAlunos")))
            dijit.byId("gridConfirmacaoAlunos").update();
        var setTurma = dijit.byId("no_turma_encerramento").store.data[0].name;
        var setIdTurma = dijit.byId("no_turma_encerramento").store.data[0].id;

        var contrato_selecionado = dijit.byId("cd_nome_contrato_pesq").value;

        if (setIdTurma.value > 0) {
            dijit.byId("no_turma_encerramento").value = setTurma;
        } else {
            dijit.byId("no_turma_encerramento").set("value", setIdTurma);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function wizardAnterior() {
    dijit.byId("dialogEncerramento2").hide();
    dijit.byId("dialogEncerramento").show();
}

function criarGradesEncerramento(dataTurmasEnc) {
    require([
   "dojox/grid/EnhancedGrid",
   "dojox/grid/enhanced/plugins/Pagination",
   "dojo/data/ObjectStore",
   "dojo/store/Memory"
    ], function (EnhancedGrid, Pagination, ObjectStore, Memory) {
        try {
            var gridEncerramentoTurmas = new EnhancedGrid({
                store: new ObjectStore({ objectStore: new Memory({ data: dataTurmasEnc }) }),
                structure:
                  [
                    { name: "Turma", field: "no_turma", width: "100%" }
                  ],
                canSort: true,
                plugins: {
                    pagination: {
                        pageSizes: ["10", "34", "100", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "10",
                        gotoButton: true,
                        /*page step to be displayed*/
                        maxPageStep: 5,
                        /*position of the pagination bar*/
                        position: "button"
                    }
                },
                noDataMessage: "Nenhum registro encontrado.",
                contentEditable: true
            }, "gridEncerramentoTurmas");
            gridEncerramentoTurmas.startup();


            //GRID CONFIRMAÇÃO ALUNO
            var dataConfirAluno = new Array();
            var gridConfirmacaoAlunos = new EnhancedGrid({
                store: new ObjectStore({ objectStore: new Memory({ data: dataConfirAluno }) }),
                structure:
                  [
                    { name: "Aluno", field: "no_aluno", width: "35%" },
                    { name: "Movimento", field: "dta_movimento", width: "15%" },
                    { name: "Aulas Dadas", field: "nm_aulas_dadas", width: "15%" },
                    { name: "Faltas", field: "nm_faltas", width: "15%" },
                    { name: "% Frequência", field: "frequencia", width: "20%" }
                  ],
                canSort: true,
                plugins: {
                    pagination: {
                        pageSizes: ["10", "34", "100", "All"],
                        description: true,
                        sizeSwitch: true,
                        pageStepper: true,
                        defaultPageSize: "10",
                        gotoButton: true,
                        /*page step to be displayed*/
                        maxPageStep: 5,
                        /*position of the pagination bar*/
                        position: "button"
                    }
                },
                noDataMessage: "Nenhum registro encontrado.",
                contentEditable: true
            }, "gridConfirmacaoAlunos");
            gridConfirmacaoAlunos.startup();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function populaGridConfirmacaoAlunos(setIdTurma) {
    if (setIdTurma == 0) {
        var setIdTurma = hasValue(dijit.byId("no_turma_encerramento").value) ? parseInt(dijit.byId("no_turma_encerramento").value) : 0;
    }
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getAlunosTurma?cd_turma=" + setIdTurma,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            var dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridConfirmacaoAlunos");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function dataDiaEnc(xhr, ready, Memory, filteringSelect, id, tela, pFuncao) {
    xhr.get({
        url: Endereco() + "/api/secretaria/getData",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            if (id == INICIO_ENCERRAMENTO)
                dojo.byId('dt_fim_enc').value = data;
            else {
                var dataFim = dojo.date.locale.parse(dojo.byId('dt_fim_enc').value, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                var dataAtual = dojo.date.locale.parse(data, { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                if (dojo.date.compare(dataFim, dataAtual, "date") == 1) {

                    var mensagensWeb = [];
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataEnc);
                    if (tela == INICIO_ENCERRAMENTO)
                        apresentaMensagem('apresentadorMensagemEncerramento', mensagensWeb);
                    else
                        apresentaMensagem('apresentadorMensagemEncConf', mensagensWeb);
                    return false;
                }
                else
                    if (hasValue(pFuncao))
                        pFuncao.call();
            }

        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        if (tela == INICIO_ENCERRAMENTO)
            apresentaMensagem('apresentadorMensagemEncerramento', error);
        else
            apresentaMensagem('apresentadorMensagemEncConf', error);
    });
}

function eventoEncerrar(itensSelecionados, xhr, ready, Memory, filteringSelect) {
    try {
        if (hasValue(itensSelecionados) && itensSelecionados.length > 0) {
            var erro = false;
            for (var i = 0 ; i < itensSelecionados.length; i++)
                if (itensSelecionados[i].dta_termino_turma != null) {
                    caixaDialogo(DIALOGO_AVISO, 'Turma ' + itensSelecionados[i].no_turma + ' já está encerrada.', null);
                    erro = true;
                    break;
                }
            for (var i = 0 ; i < itensSelecionados.length; i++)
                if (itensSelecionados[i].id_turma_ppt == true) {
                    caixaDialogo(DIALOGO_AVISO, 'Turma PPT ' + itensSelecionados[i].no_turma + ' não pode ser encerrada.', null);
                    erro = true;
                    break;
                }
            if (itensSelecionados[0].cd_pessoa_escola != dojo.byId('_ES0').value){
                    caixaDialogo(DIALOGO_AVISO, 'Turmas de outra escola somente podem ser encerradas na escola de origem.', null);
                    return false;
                }
            if (erro == false)
                require([
               "dojo/ready",
               "dojo/_base/xhr",
               "dojo/store/Memory",
               "dijit/form/FilteringSelect"
                ], function (ready, xhr, Memory, FilteringSelect) {
                    //   dijit.byId('dt_fim_enc').attr("value", new Date(ano, mes, dia));
                    dataDiaEnc(xhr, ready, Memory, FilteringSelect, 1, 0, null);
                    xhr.get({
                        url: Endereco() + "/api/secretaria/getData",
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (data) {
                        dojo.byId("dt_fim_enc").value = data;
                        // chamo aqui o carregamento?
                        componentesPesquisaMatricula();
                    },
                    function (error) {
                        apresentaMensagem("apresentadorMensagemEncerramento", error);
                    });

                    var dataTurmasEnc = new Array(itensSelecionados.length);
                    var dataTurmasEncComCod = new Array(itensSelecionados.length);
                    for (var i = 0; i < itensSelecionados.length; i++)
                        dataTurmasEnc[i] = { no_turma: itensSelecionados[i].no_turma };

                    dijit.byId("no_turma_encerramento").on("change", function (e) {
                        var setIdTurma = e;
                        if (this.value > 0) {
                            populaGridConfirmacaoAlunos(setIdTurma);
                        }
                    });

                    destroyCreateEncerramentoTurmas();
                    populaTurmasEnc(itensSelecionados);
                    criarGradesEncerramento(dataTurmasEnc);

                    dijit.byId("dialogEncerramento").show();
                });
        } else
            caixaDialogo(DIALOGO_AVISO, 'Selecione a(s) turma(s) para efetuar o encerramento.', null);
        if (hasValue(dijit.byId("gridEncerramentoTurmas")))
            dijit.byId("gridEncerramentoTurmas").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaTurmasEnc(itensSelecionados) {
    require(["dojo/store/Memory", "dojo/_base/array"],
    function (Memory, Array) {
        try {
            var cbTurmaEnc = dijit.byId("no_turma_encerramento");
            var itemsCb = [];
            Array.forEach(itensSelecionados, function (value, i) {
                itemsCb.push({ id: value.cd_turma, name: value.no_turma });
            });
            var stateStore = new Memory({
                data: itemsCb
            });
            cbTurmaEnc.store = stateStore;
            cbTurmaEnc._onChangeActive = false;
            cbTurmaEnc.set("value", 0);
            cbTurmaEnc._onChangeActive = true;
            setaTurmaDojo = JSON.stringify(stateStore.data[0].name);
            setaTurmaIDDojo = JSON.stringify(stateStore.data[0].id);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function destroyCreateEncerramentoTurmas() {
    try {
        apresentaMensagem('apresentadorMensagemEncerramento', null);
        apresentaMensagem('apresentadorMensagemEncConf', null);
        if (hasValue(dijit.byId("gridEncerramentoTurmas"))) {
            dijit.byId("gridEncerramentoTurmas").destroyRecursive();
            $('<div>').attr('id', 'gridEncerramentoTurmas').attr('style', 'height:300px;').appendTo('#PaigridEncerramentoTurmas');
        }
        if (hasValue(dijit.byId("gridConfirmacaoAlunos"))) {
            dijit.byId("gridConfirmacaoAlunos").destroyRecursive();
            $('<div>').attr('id', 'gridConfirmacaoAlunos').attr('style', 'height:300px;').appendTo('#PaigridConfirmacaoAlunos');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

var TIPO_NAO_REFAZER_PROGRAMACOES = 0;
var TIPO_REFAZER_PROGRAMACOES_CURSO = 1;
var TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL = 2;
var TIPO_REFAZER_PROGRAMACOES_HORARIO = 3;
var TIPO_REFAZER_PROGRAMACOES_FERIADO = 4;
var TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO = 5;

function verificaProgramacaoTurma(obj, xhr, nome_campo, tipo, p_funcao) {
    try {
        var gridProgramacao = dijit.byId('gridProgramacao');
        var item = dijit.byId('gridTurma').itemSelecionado;
        var turma_PPT = false;
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        if (tipoTurma == TURMA_PPT) {
            turma_PPT = true;
            gridProgramacao = dijit.byId('gridProgramacaoPPT');
            if (dojo.byId("tipoVerif").value == VERIFITURMAFILHAPPT2MODAL)
                item.cd_turma = dojo.byId("cd_turmaPPT").value;
        }
        //Verifica se realmente é troca de valores:
        /*if ((tipo == TIPO_DOJO && obj.value == eval('item.'+nome_campo))
             || (tipo == TIPO_DIJIT && dijit.byId(obj.id) ==  eval('item.'+nome_campo)))
            return false;*/

        if (hasValue(gridProgramacao) && existeAulaEfetivadaGrid(gridProgramacao) && tipo != TIPO_REFAZER_PROGRAMACOES_HORARIO && tipo != TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO) {
            dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
            retrocedeCampo(obj, item, nome_campo, tipo);
        }
        else if (tipo != TIPO_REFAZER_PROGRAMACOES_HORARIO && tipo != TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO) {

            if (hasValue(item))
                xhr.get({
                    preventCache: true,
                    url: Endereco() + "/api/turma/existeAulaEfetivadaTurma?cd_turma=" + item.cd_turma + "&turma_PPT=" + turma_PPT,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (retorno) {
                    try {
                        if (retorno.retorno) {
                            dojo.byId('refazer_programacao').value = TIPO_NAO_REFAZER_PROGRAMACOES;
                            retrocedeCampo(obj, item, nome_campo, tipo);
                        }
                        else {
                            if (hasValue(p_funcao))
                                p_funcao.call();

                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgProgramacaoRefeitas);

                            if (!hasValue(dojo.byId("cadTurmaPPT").style.display))
                                apresentaMensagem('apresentadorMensagemTurmaPPT', mensagensWeb);
                            else
                                apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);

                            dojo.byId('refazer_programacao').value = (tipo == TIPO_DOJO ? TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL : TIPO_REFAZER_PROGRAMACOES_CURSO);
                            criaAtualizaTagProgramacao();
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    apresentaMensagem('apresentadorMensagemTurma', error);
                });
            else {
                if (hasValue(p_funcao))
                    p_funcao.call();

                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgProgramacaoRefeitas);

                if (!hasValue(dojo.byId("cadTurmaPPT").style.display))
                    apresentaMensagem('apresentadorMensagemTurmaPPT', mensagensWeb);
                else
                    apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);

                dojo.byId('refazer_programacao').value = (tipo == TIPO_DOJO ? TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL : TIPO_REFAZER_PROGRAMACOES_CURSO);
                criaAtualizaTagProgramacao();
            }
        }
        else if (hasValue(p_funcao)) {
            p_funcao.call();

            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgProgramacaoRefeitas);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);

            dojo.byId('refazer_programacao').value = (tipo == TIPO_DOJO ? TIPO_REFAZER_PROGRAMACOES_DATA_INICIAL : TIPO_REFAZER_PROGRAMACOES_CURSO);
            criaAtualizaTagProgramacao();
        }
        else if (tipo == TIPO_REFAZER_PROGRAMACOES_HORARIO || tipo == TIPO_REFAZER_PROGRAMACOES_DESC_FERIADO) {
            dojo.byId('refazer_programacao').value = tipo;
            criaAtualizaTagProgramacao();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

var TIPO_DIJIT = 1;
var TIPO_DOJO = 0;

function retrocedeCampo(campo, item, nome_campo, tipo) {
    try {
        //Volta ao valor original do campo:
        var campo_dijit = campo;
        if (tipo == TIPO_DOJO) {
            campo_dijit = dijit.byId(campo.id);
            campo_dijit._onChangeActive = false;
            if (hasValue(item))
                campo.value = eval('item.' + nome_campo);
            else
                campo.value = '';
            campo_dijit._onChangeActive = true;
        }
        else {
            campo_dijit._onChangeActive = false;
            if (hasValue(item))
                campo_dijit.set('value', eval('item.' + nome_campo));
            else
                campo_dijit.set('value', '');
            campo_dijit._onChangeActive = true;
        }

        //Lança a mensagem de erro:
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgProgramacaoComAulaEfetivada);
        if (!hasValue(dojo.byId("cadTurmaPPT").style.display))
            apresentaMensagem('apresentadorMensagemTurmaPPT', mensagensWeb);
        else
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
        return false;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function existeAulaEfetivadaGrid(gridProgramacao) {
    try {
        var retorno = false;

        for (var i = 0; i < gridProgramacao.store.objectStore.data.length; i++)
            if (gridProgramacao.store.objectStore.data[i].id_aula_dada)
                retorno = true;

        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function desabilitarCamposIncluirHorariosTurma(bool) {
    try {
        if (bool) {
            dojo.byId("cbDias").style.display = "none";
            dojo.byId("timeIni").style.display = "none";
            dojo.byId("timeFim").style.display = "none";
        } else {
            dojo.byId("cbDias").style.display = "";
            dojo.byId("timeIni").style.display = "";
            dojo.byId("timeFim").style.display = "";
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region Metodos para verificação de horários Filha PPT - verificarIntersecaoHorariosPPTFilha - mergeItemGridHorariosTurmaFilhaPPT - loadHorarioTurmaPPTFilha - loadHorarioTurmaPPTPorBD - incluirItemHorarioTurmaFilhaPPT - carregarHorariosTurmaFilhaPPT
function verificarIntersecaoHorariosPPTFilha(item, idComp, msg, tipoVerif) {
    try {
        var mensagensWeb = new Array();
        var mensagem = '';
        var mergeAceito = false;
        var gridHorario = hasValue(dijit.byId(idComp)) ? dijit.byId(idComp) : [];
        var gridTurma = hasValue(dijit.byId('gridTurma')) ? dijit.byId(dijit.byId('gridTurma')) : [];
        var dataHorarios = [];

        if (item != null && hasValue(item.startTime) && hasValue(item.endTime)) {
            var hIni = (new Date(item.startTime)).getHours();
            var hFim = (new Date(item.endTime)).getHours();
            var mIni = (new Date(item.startTime)).getMinutes();
            var mFim = (new Date(item.endTime)).getMinutes();

            var totalMinFilhaPPTIni = (parseInt(hIni) * 60) + parseInt(mIni);
            var totalMinFilhaPPTFin = (parseInt(hFim) * 60) + parseInt(mFim);

            if (gridHorario != null && gridHorario.store.data.length > 0) {
                //Filtra a lista de itens para trazer os horários do dia inserido.
                $.each(gridHorario.store.data, function (index, value) {
                    if (value.startTime.getDate() == item.id_dia_semana && value.id != parseInt(item.id))
                        dataHorarios.push(value);
                });
                if (dataHorarios.length == 0) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHorarioForaDoItervalo);
                    apresentaMensagem(msg, mensagensWeb);
                    mergeAceito = false;
                    if (item.cd_horario == 0)
                        //gridHorario.store.remove(item.id);
                        removeHorario(gridHorario, item.id);
                    else {
                        if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                            getHorarioOriginalEditadoFilhaPPTModal(item.cd_horario, item.id);
                        else
                            getHorarioOriginalEditado(item.cd_horario, item.id);
                    }
                    return mergeAceito;
                }

                var itemMergeError = false;
                for (var i = 0; i < dataHorarios.length; i++) {
                    if (dataHorarios[i].calendar == "Calendar3") {
                        totalMinPaiPPTIni = trasformarHorasEmMinutos(dataHorarios[i].dt_hora_ini);
                        totalMinPaiPPTFin = trasformarHorasEmMinutos(dataHorarios[i].dt_hora_fim);

                        if (totalMinFilhaPPTIni >= totalMinPaiPPTIni && totalMinFilhaPPTIni < totalMinPaiPPTFin &&
                            totalMinFilhaPPTFin > totalMinPaiPPTIni && totalMinFilhaPPTFin <= totalMinPaiPPTFin) {
                            mergeAceito = true;
                            itemMergeError = true;
                        }//end if 2°
                    }//end if 1°
                    if (itemMergeError)
                        break;
                }
                if (!mergeAceito) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroHorarioForaDoItervalo);
                    apresentaMensagem(msg, mensagensWeb);
                    if (item.cd_horario == 0)
                        //gridHorario.store.remove(item.id);
                        removeHorario(gridHorario, item.id);
                    else {
                        if (tipoVerif == VERIFITURMAFILHAPPT2MODAL)
                            getHorarioOriginalEditadoFilhaPPTModal(item.cd_horario, item.id);
                        else
                            getHorarioOriginalEditado(item.cd_horario, item.id);
                    }
                }
            }
        }
        return mergeAceito;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mergeItemGridHorariosTurmaFilhaPPT(id, startTime, endTime, cd_horario, idComp, msg) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try {
            apresentaMensagem(msg, null);
            var calendar = dijit.byId(idComp);
            var arraySemana = [];
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            var diaRegistro = new Date(startTime);
            var endDia = new Date(endTime);
            var inicioDia = diaRegistro.getDate();
            var fimDia = endDia.getDate();
            var diaRegistro = diaRegistro.getDate();

            if (inicioDia != fimDia) {
                if (cd_horario == 0)
                    removeHorario(calendar, id);
                else
                    getHorarioOriginalEditadoFilhaPPTModal(cd_horario, id);
                return false;
            }

            var dates = {
                convert: function (dto) {
                    return (
                        dto.constructor === Date ? dto :
                        dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                        dto.constructor === Number ? new Date(dto) :
                        dto.constructor === String ? new Date(dto) :
                        typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                        NaN
                    );
                },
                compare: function (a, b) {
                    return (
                        isFinite(a = this.convert(a).valueOf()) &&
                        isFinite(b = this.convert(b).valueOf()) ?
                        (a > b) - (a < b) :
                        NaN
                    );
                },
                inRange: function (dto, start, end) {
                    return (
                         isFinite(dto = this.convert(dto).valueOf()) &&
                         isFinite(start = this.convert(start).valueOf()) &&
                         isFinite(end = this.convert(end).valueOf()) ?
                         start <= dto && dto <= end :
                         NaN
                     );
                },
                interception: function (start1, end1, start2, end2) {
                    return (
                         isFinite(a1 = this.convert(start1).valueOf()) &&
                         isFinite(a2 = this.convert(end1).valueOf()) &&
                         isFinite(b1 = this.convert(start2).valueOf()) &&
                         isFinite(b2 = this.convert(end2).valueOf()) ?
                         ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                         NaN
                     );
                },
                include: function (start1, end1, start2, end2) {
                    return (
                         isFinite(a1 = this.convert(start1).valueOf()) &&
                         isFinite(a2 = this.convert(end1).valueOf()) &&
                         isFinite(b1 = this.convert(start2).valueOf()) &&
                         isFinite(b2 = this.convert(end2).valueOf()) ?
                         ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                         NaN
                     );
                }
            }
            var d;
            var start, end;
            var colView = calendar.columnView;
            var gradeHorarios = dijit.byId(idComp);
            var dataHorarios = gradeHorarios.store.data;
            var resolveuIntersecao = false;
            var criarItemHorario = true;
            var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
            var quantItensMerge = 0;
            var arraycodHorariosItensMerge = [];
            var arrayTotalItensMerge = [];
            var posicaoArray = 0;
            start = new Date(startTime);
            end = new Date(endTime);
            d = new Date(endTime);
            //cria o item para a verificação.
            var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
            var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
            var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
            var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
            var item = {
                id: id,
                cd_horario: parseInt(cd_horario),
                summary: "",
                startTime: start,
                calendar: "Calendar2",
                endTime: d,
                dt_hora_ini: hIni + ":" + mIni + ":00",
                dt_hora_fim: hFim + ":" + mFim + ":00",
                id_dia_semana: start.getDate(),
                id_disponivel: true
            };

            if (hasValue(dataHorarios) && dataHorarios.length > 0) {
                dataHorarios = jQuery.grep(dataHorarios, function (value) {
                    return value.startTime.getDate() == diaRegistro && value.id != parseInt(id);
                });

            }

            /****Verificar se existe interseção com um ou mais horários disponíveis:****/
            for (var j = dataHorarios.length - 1; j >= 0; j--) {
                // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                if (dataHorarios[j].calendar == "Calendar2" && (dates.interception(start, d, dataHorarios[j].startTime, dataHorarios[j].endTime))) {
                    quantItensMerge += 1;
                    posicaoArray = j;
                    arrayTotalItensMerge.push(dataHorarios[j]);
                    if (hasValue(dataHorarios[j].cd_horario) && dataHorarios[j].cd_horario > 0)
                        arraycodHorariosItensMerge.push(dataHorarios[j].cd_horario);
                }
            }
            //verifica se não esta fazendo merge com horarios ocupados pela turma PPT.
            if (!verificarIntersecaoHorariosPPTFilha(item, idComp, msg, VERIFITURMAFILHAPPT2MODAL))
                return false;
            if (quantItensMerge == 1) {
                criarItemHorario = false;

                var cd_horario_item_merge = dataHorarios[posicaoArray].cd_horario;
                var itemMerge = dataHorarios[posicaoArray];
                if (parseInt(cd_turma) > 0) {
                    if (cd_horario_item_merge > 0 && cd_horario > 0)
                        verificarDoisItensHorarioDisponibilidadeTurma(xhr, ref, item, gradeHorarios, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, false, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, null);
                    else
                        if (cd_horario_item_merge > 0) {
                            if (id != null && id > 0)
                                //calendar.store.remove(id);
                                removeHorario(calendar, id);
                            item.cd_horario = cd_horario_item_merge;
                            verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates, calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, false, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, null);
                        } else
                            if (cd_horario > 0)
                                verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates, calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, false, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, null);
                            else {
                                if (id != null && id > 0)
                                    //calendar.store.remove(id);
                                    removeHorario(calendar, id);
                                resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFITURMAFILHAPPT2MODAL, xhr, tipoTurma, msg, null);
                            }
                } else {
                    if (id != null && id > 0)
                        removeHorario(calendar, id);
                    resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFITURMAFILHAPPT2MODAL, xhr, tipoTurma, msg, null);
                }
            } else if (quantItensMerge > 1) {
                criarItemHorario = false;
                //if (!testarHorarioGrid(item))
                //    return false;
                verificarSeexisteProgranacaoParaVariosHorarios(xhr, ref, item, calendar, arrayTotalItensMerge, VERIFITURMAFILHAPPT2MODAL, VERIFITURMAFILHAPPT2MODAL);
            }

            //Verificação quando é alterado um intem só.
            if (criarItemHorario)
                verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, null, null, calendar, null, null, null, ADD_HORARIO, false, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, EDIT);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function criacaoGradeHorarioTurmaPPTFilha(dataHorario, idComp, height, msg, idTimeIni, idTimeFim, idExcluirHorarioTurma, tipoOrigenMerge, tipoVerif) {
    require(["dojo/parser", "dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr",
             "dojo/data/ObjectStore", "dojox/json/ref", "dojo/store/Cache", "dojo/store/JsonRest"],
      function (parser, ready, Calendar, Observable, Memory, declare, on, xhr, ObjectStore, ref, Cache, JsonRest) {
          ready(function () {
              xhr.get({
                  preventCache: true,
                  url: Endereco() + "/api/empresa/getHorarioFuncEmpresa",
                  handleAs: "json",
                  headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
              }).then(function (getHorarioFuncEscola) {
                  try {
                      getHorarioFuncEscola = jQuery.parseJSON(getHorarioFuncEscola);
                      var horaI = 0, horaF = 24;
                      if (hasValue(getHorarioFuncEscola) && hasValue(getHorarioFuncEscola.retorno)) {
                          if (hasValue(getHorarioFuncEscola.retorno.hr_inicial))
                              horaI = parseInt(getHorarioFuncEscola.retorno.hr_inicial.substring(0, 2));
                          if (hasValue(getHorarioFuncEscola.retorno.hr_final))
                              horaF = parseInt(getHorarioFuncEscola.retorno.hr_final.substring(0, 2));
                      }
                      dijit.byId(idTimeIni).reset();
                      dijit.byId(idTimeFim).reset();
                      dijit.byId(idTimeIni).constraints.min.setHours(horaI);
                      dijit.byId(idTimeIni).constraints.max.setHours(horaF);
                      dijit.byId(idTimeFim).constraints.min.setHours(horaI);
                      dijit.byId(idTimeFim).constraints.max.setHours(horaF);
                      dijit.byId(idTimeIni).set("value", "T0" + horaI + ":00");
                      dijit.byId(idTimeFim).set("value", "T" + horaF + ":00");

                      var ECalendar = declare("extended.Calendar", Calendar, {

                          isItemResizeEnabled: function (item, rendererKind) {
                              return false || (hasValue(item) && item.cssClass == 'Calendar2');
                          },

                          isItemMoveEnabled: function (item, rendererKind) {
                              return false || (hasValue(item) && item.cssClass == 'Calendar2');
                          }
                      });

                      var dataHoje = new Date(2012, 0, 1);
                      if (hasValue(dijit.byId(idComp))) {
                          if (idComp == "gridHorariosPPT")
                              destroyCreateGridHorarioPPT();
                          else
                              destroyCreateGridHorario();
                      }
                      var ECalendar = new ECalendar({
                          date: dojo.date.locale.parse("01/07/2012", { locale: 'pt-br', formatLength: "short", selector: "date" }),
                          cssClassFunc: function (item) {
                              return item.calendar;
                          },
                          store: new Observable(new Memory({ data: dataHorario })),
                          dateInterval: "week",
                          columnViewProps: { minHours: horaI, maxHours: horaF, timeSlotDuration: 5, hourSize: 50 },
                          style: "position:relative;max-width:650px;width:100%;height:" + height + ""
                      }, idComp);
                      ECalendar.novosItensEditados = [];
                      ECalendar.on("itemClick", function (item) {
                          try {
                              var calendarName = null;

                              if (hasValue(item) && hasValue(item.item))
                                  calendarName = item.item.calendar;
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });
                      ECalendar.on("change", function (item) {
                          try {
                              var calendarName = null;

                              if (hasValue(item) && hasValue(item.newValue))
                                  calendarName = item.newValue.calendar;
                              else
                                  if (hasValue(item) && hasValue(item.oldValue))
                                      calendarName = item.oldValue.calendar;
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });

                      ECalendar.on("itemeditbegin", function (item) {
                          addItemHorariosEdit(item.item.id, ECalendar);
                      });

                      ECalendar.on("itemRollOver", function (e) {
                          try {
                              var minutosEnd = e.item.endTime.getMinutes() > 9 ? e.item.endTime.getMinutes() : "0" + e.item.endTime.getMinutes();
                              var horasEnd = e.item.endTime.getHours() > 9 ? e.item.endTime.getHours() : "0" + e.item.endTime.getHours();
                              var horarioEnd = horasEnd + ":" + minutosEnd;
                              var minutosStart = e.item.startTime.getMinutes() > 9 ? e.item.startTime.getMinutes() : "0" + e.item.startTime.getMinutes();
                              var horasStart = e.item.startTime.getHours() > 9 ? e.item.startTime.getHours() : "0" + e.item.startTime.getHours();
                              var horarioStart = horasStart + ":" + minutosStart;
                              if (e.item.calendar == "Calendar2") {
                                  dojo.attr(e.renderer.domNode.id, "title", "Ocupado pela Turma  " + horarioStart + " as " + horarioEnd);
                              }
                              if (e.item.calendar == "Calendar3")
                                  dojo.attr(e.renderer.domNode.id, "title", "Ocupado pela Turma PPT  " + horarioStart + " as " + horarioEnd);
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });

                      var createItem = function (view, d, e) {
                          try {
                              apresentaMensagem(msg, null);
                              // create item by maintaining control key
                              //if(!e.ctrlKey ||e.shiftKey || e.altKey){ //Usado para criar o item no componente somente com o click do mouse + a tecla <ctrl> pressionada.
                              if (e.shiftKey || e.altKey) {
                                  return;
                              }

                              var dates = {
                                  convert: function (dto) {
                                      return (
                                          dto.constructor === Date ? dto :
                                          dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                                          dto.constructor === Number ? new Date(dto) :
                                          dto.constructor === String ? new Date(dto) :
                                          typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                                          NaN
                                      );
                                  },
                                  compare: function (a, b) {
                                      return (
                                          isFinite(a = this.convert(a).valueOf()) &&
                                          isFinite(b = this.convert(b).valueOf()) ?
                                          (a > b) - (a < b) :
                                          NaN
                                      );
                                  },
                                  inRange: function (dto, start, end) {
                                      return (
                                           isFinite(dto = this.convert(dto).valueOf()) &&
                                           isFinite(start = this.convert(start).valueOf()) &&
                                           isFinite(end = this.convert(end).valueOf()) ?
                                           start <= dto && dto <= end :
                                           NaN
                                       );
                                  },
                                  interception: function (start1, end1, start2, end2) {
                                      return (
                                           isFinite(a1 = this.convert(start1).valueOf()) &&
                                           isFinite(a2 = this.convert(end1).valueOf()) &&
                                           isFinite(b1 = this.convert(start2).valueOf()) &&
                                           isFinite(b2 = this.convert(end2).valueOf()) ?
                                           ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                                           NaN
                                       );
                                  },
                                  include: function (start1, end1, start2, end2) {
                                      return (
                                           isFinite(a1 = this.convert(start1).valueOf()) &&
                                           isFinite(a2 = this.convert(end1).valueOf()) &&
                                           isFinite(b1 = this.convert(start2).valueOf()) &&
                                           isFinite(b2 = this.convert(end2).valueOf()) ?
                                           ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                                           NaN
                                       );
                                  }
                              }

                              //var d = new Date(2012, 6, diaSemana, timeIni.value.getHours(), timeIni.value.getMinutes());
                              var start, end;
                              var colView = ECalendar.columnView;
                              var cal = ECalendar.dateModule;
                              var gradeHorarios = dijit.byId(idComp);
                              var itens = dijit.byId(idComp).items;
                              var id = gradeHorarios.items.length;
                              var item = null;

                              id = geradorIdHorarios(gradeHorarios);

                              if (view == colView) {
                                  start = ECalendar.floorDate(d, "minute", colView.timeSlotDuration);
                                  //start = d;
                                  end = cal.add(start, "minute", colView.timeSlotDuration);
                                  //end = new Date(d.getTime() + 15*60000);
                                  var inicioDia = start.getDate();
                                  if (itens.length == 0) {
                                      var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                                      var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                                      var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                                      var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                                      item = {
                                          id: id,
                                          cd_horario: 0,
                                          summary: "",
                                          dt_hora_ini: hIni + ":" + mIni + ":00",
                                          dt_hora_fim: hFim + ":" + mFim + ":00",
                                          startTime: start,
                                          endTime: end,
                                          calendar: "Calendar2",
                                          id_dia_semana: start.getDate(),
                                          id_disponivel: true,
                                          cd_registro: 0,
                                          HorariosProfessores: new Array(),
                                          inserido: true
                                      };

                                  } else {

                                      for (var j = itens.length - 1; j >= 0; j--) {
                                          //if (itens[j].id != evento.item.id) {
                                          //calendar.store.remove(itens[j].id);

                                          // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                                          if (itens[j].cssClass == "Calendar2" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                                              // Verifica se um item inclui totalmente o outro item e remove o incluído:
                                              var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);

                                              if (included == 1) {
                                                  resolveuIntersecao = true;
                                                  return false;
                                              }
                                              else
                                                  if (included == 2) {
                                                      //calendar.store.remove(itens[j].id);
                                                      removeHorario(calendar, itens[j].id);
                                                      // Caso contrário, junta um item com o outro:
                                                  } else
                                                      if (included != NaN) {
                                                          if (dates.compare(start, itens[j].startTime) == 1)
                                                              start = itens[j].startTime;
                                                          else
                                                              end = itens[j].endTime;
                                                          //calendar.store.remove(itens[j].id);
                                                          removeHorario(calendar, itens[j].id);
                                                      }
                                          }
                                      }
                                      var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                                      var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                                      var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                                      var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                                      item = {
                                          id: id,
                                          cd_horario: 0,
                                          summary: "",
                                          dt_hora_ini: hIni + ":" + mIni + ":00",
                                          dt_hora_fim: hFim + ":" + mFim + ":00",
                                          startTime: start,
                                          endTime: end,
                                          calendar: "Calendar2",
                                          id_dia_semana: start.getDate(),
                                          id_disponivel: true,
                                          inserido: true
                                      };

                                  }
                              }
                              if (tipoOrigenMerge == CADPRINCFILHAPPT) {
                                  if (!verificarIntersecaoHorariosPPTFilha(item, idComp, msg, tipoVerif))
                                      return false;
                              }
                              var idCompReplace = idComp.replace('"', '');
                              setTimeout("removeHorarioEditado(" + item.id + ",'" + idComp + "')", 1500);
                              return item;
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      }

                      ECalendar.set("createOnGridClick", true);
                      ECalendar.set("createItemFunc", createItem);
                      //calendar.set("ItemEditEnd", ItemEditEnd);

                      dijit.byId(idComp).on("ItemEditEnd", function (evt) {
                          try {
                              var inserido = false;
                              if (hasValue(evt.item.inserido) && evt.item.inserido)
                                  inserido = evt.item.inserido;
                              if (tipoOrigenMerge == CADPRINCFILHAPPT) {
                                  setTimeout(function () {
                                      mergeItemGridHorariosTurma(evt.item.id, evt.item.startTime, evt.item.endTime, evt.item.cd_horario, 'gridHorarios', 'apresentadorMensagemTurma', inserido, xhr, ref);
                                  }, 100);
                                  //setTimeout('mergeItemGridHorariosTurma("' + evt.item.id + '", "' + evt.item.startTime + '", "' + evt.item.endTime + '", "' + evt.item.cd_horario + '", "' +
                                  //           'gridHorarios' + '", "' + 'apresentadorMensagemTurma' + '")', 100);
                              } else
                                  setTimeout('mergeItemGridHorariosTurmaFilhaPPT("' + evt.item.id + '", "' + evt.item.startTime + '", "' + evt.item.endTime + '", "' + evt.item.cd_horario + '", "' +
                                             'gridHorariosPPT' + '", "' + 'apresentadorMensagemTurmaPPT' + '")', 100);
                          }
                          catch (e) {
                              postGerarLog(e);
                          }
                      });
                      var toolBar = document.getElementById('dijit_Toolbar_0');
                      $(".buttonContainer").css("display", "none");
                      $(".viewContainer").css("top", "5px");
                      if (hasValue(toolBar))
                          toolBar.style.display = 'none';
                  }
                  catch (e) {
                      postGerarLog(e);
                  }
              },
              function (error) {
                  apresentaMensagem(msg, error);
              });
          });
      });

}

function loadHorarioTurmaPPTFilha(cd_turma_PPT, cd_turma, idComp, height, msg) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        xhr.post({
            url: Endereco() + "/api/escola/postTodosHorariosVinculosTurmaPPTFilha",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: ref.toJson({
                cd_turma_PPT: parseInt(cd_turma_PPT),
                cd_turma: parseInt(cd_turma)
            })
        }).then(function (dataHorario) {
            try {
                var turma = dijit.byId("gridTurma");
                var listaHorariosTurma = [];
                var codMaiorHorarioTurma = 0;
                var menorData = null;
                var menorHorario = null;

                if (hasValue(dataHorario.retorno) && dataHorario.retorno.horarioOcupTurmaPPT != null && dataHorario.retorno.horarioOcupTurmaPPT.length > 0)
                    $.each(dataHorario.retorno.horarioOcupTurmaPPT, function (idx, val) {
                        codMaiorHorarioTurma += 1;
                        val.calendar = "Calendar3";
                        val.id = codMaiorHorarioTurma;
                        val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                        val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                        listaHorariosTurma.push(val);
                    });
                //Horários ocupados da turma:
                if (hasValue(dataHorario.retorno) && dataHorario.retorno.horarioOcupTurma != null && dataHorario.retorno.horarioOcupTurma.length > 0)
                    $.each(dataHorario.retorno.horarioOcupTurma, function (idx, val) {
                        codMaiorHorarioTurma += 1;
                        val.calendar = "Calendar2";
                        val.id = codMaiorHorarioTurma;
                        val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                        val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                        listaHorariosTurma.push(val);

                        //Pega o menor horário:
                        if (menorData == null || val.dt_hora_ini < menorHorario) {
                            menorData = val.startTime;
                            menorHorario = val.dt_hora_ini;
                        }
                    });

                //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                setTimeout(function () { posicionaPrimeiraLinhaHorarioTurma('gridHorarios', listaHorariosTurma.length, menorData); }, 100);

                criacaoGradeHorarioTurmaPPTFilha(listaHorariosTurma, 'gridHorarios', '293px', msg, "timeIni", "timeFim", "excluirHorarioTurma", CADPRINCFILHAPPT, VERIFTURMANORMAL);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(msg, error);
        });
    })
}

function loadHorarioTurmaPPTPorBD(gridHorarios) {
    try {
        var codMaiorHorarioTurma = 0;
        var horarios = [];
        var menorData = null;
        var menorHorario = null;
        var cdTurma = dojo.byId("cd_turma").value;
        var gridHorariosFilhaPPT = dijit.byId("gridHorariosPPT");
        if (hasValue(gridHorarios) && gridHorarios.params.store.data.length >= 0) {
            var horariosFilha = formatHorarioBDParaHorariosGrid(dijit.byId("gridAlunosPPT").itensSelecionados[0].horariosTurma, 'Calendar2');
            var horariosPPT = getHorariosOcupadosTurma(gridHorarios.params.store.data);
            $.each(horariosPPT, function (idx, val) {
                val.calendar = 'Calendar3';
            });

            $.each(horariosFilha, function (idx, val) {
                codMaiorHorarioTurma += 1;
                val.id = codMaiorHorarioTurma;
                horariosPPT.push(val);
            });
            horariosFilha = horariosPPT;

            $.each(horariosFilha, function (idx, val) {
                //Pega o menor horário:
                if (menorData == null || val.dt_hora_ini < menorHorario) {
                    menorData = val.startTime;
                    menorHorario = val.dt_hora_ini;
                }
            });

            if (hasValue(gridHorariosFilhaPPT))
                carregarHorariosTurmaFilhaPPT(gridHorariosFilhaPPT, horariosFilha);
            else
                criacaoGradeHorarioTurmaPPTFilha(horariosFilha, "gridHorariosPPT", '295px',
                                                 'apresentadorMensagemTurmaPPT', "timeIniFilhaPPT", "timeFimFilhaPPT", "excluirHorarioTurmaFilhaPPT", CADPRINCFILHAPPTLINK, VERIFITURMAFILHAPPT2MODAL);

            //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
            setTimeout(function () { posicionaPrimeiraLinhaHorarioTurma("gridHorariosPPT", horariosFilha.length, menorData); }, 100);
        } else
            if (cdTurma != null && cdTurma > 0) {
                var listaCompleta = [];
                var horariosFilha = formatHorarioBDParaHorariosGrid(dijit.byId("gridAlunosPPT").itensSelecionados[0].horariosTurma, 'Calendar2');
                var horariosPPT = formatHorarioBDParaHorariosGrid(dijit.byId("gridTurma").TurmaEdit.horariosTurma, 'Calendar3');

                $.each(horariosFilha, function (idx, val) {
                    //Pega o menor horário:
                    if (menorData == null || val.dt_hora_ini < menorHorario) {
                        menorData = val.startTime;
                        menorHorario = val.dt_hora_ini;
                    }
                });
                $.each(horariosPPT, function (idx, value) {
                    codMaiorHorarioTurma += 1;
                    var id = codMaiorHorarioTurma;
                    var item = {
                        id: id,
                        cd_horario: parseInt(value.cd_horario),
                        summary: "",
                        startTime: value.startTime,
                        calendar: value.calendar,
                        endTime: value.endTime,
                        dt_hora_ini: value.dt_hora_ini,
                        dt_hora_fim: value.dt_hora_fim,
                        id_dia_semana: value.id_dia_semana,
                        id_disponivel: value.id_disponivel,
                        no_datas: value.no_datas,
                        HorariosProfessores: hasValue(value.HorariosProfessores) ? value.HorariosProfessores.slice() : new Array
                    };
                    listaCompleta.push(item);
                });
                $.each(horariosFilha, function (idx, value) {
                    codMaiorHorarioTurma += 1;
                    var id = codMaiorHorarioTurma;
                    var dadosProfessor = dijit.byId('gridProfessorPPT').store.objectStore.data;

                    for (var i = 0; i < dadosProfessor.length; i++)
                        for (var j = 0; j < dadosProfessor[i].horarios.length; j++)
                            if (dadosProfessor[i].horarios[j].id == value.id)
                                dadosProfessor[i].horarios[j].id = codMaiorHorarioTurma;

                    var item = {
                        id: id,
                        cd_horario: parseInt(value.cd_horario),
                        summary: "",
                        startTime: value.startTime,
                        calendar: value.calendar,
                        endTime: value.endTime,
                        dt_hora_ini: value.dt_hora_ini,
                        dt_hora_fim: value.dt_hora_fim,
                        id_dia_semana: value.id_dia_semana,
                        id_disponivel: value.id_disponivel,
                        no_datas: value.no_datas,
                        HorariosProfessores: hasValue(value.HorariosProfessores) ? value.HorariosProfessores.slice() : new Array
                    };
                    listaCompleta.push(item);
                });


                if (hasValue(gridHorariosFilhaPPT))
                    carregarHorariosTurmaFilhaPPT(gridHorariosFilhaPPT, listaCompleta);
                else
                    criacaoGradeHorarioTurmaPPTFilha(listaCompleta, "gridHorariosPPT", '295px', 'apresentadorMensagemTurmaPPT',
                                                     "timeIniFilhaPPT", "timeFimFilhaPPT", "excluirHorarioTurmaFilhaPPT", CADPRINCFILHAPPTLINK, VERIFITURMAFILHAPPT2MODAL);

                //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                setTimeout(function () { posicionaPrimeiraLinhaHorarioTurma("gridHorariosPPT", listaCompleta.length, menorData); }, 100);
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirItemHorarioTurmaFilhaPPT(idComp, msg) {
    require(["dojo/_base/xhr", "dojox/json/ref"], function (xhr, ref) {
        try {
            apresentaMensagem(msg, null);
            var mensagensWeb = new Array();
            var calendar = dijit.byId(idComp);
            var timeFim = dijit.byId('timeFimFilhaPPT');
            var timeIni = dijit.byId('timeIniFilhaPPT');
            var arraySemana = dijit.byId('cbDiaFilhaPPTs').value;
            var cd_horario = 0;
            var tipoTurma = dojo.byId("tipoTurmaCad").value;
            if (!hasValue(calendar)) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNaoConstruidoGradeHorario);
                apresentaMensagem(msg, mensagensWeb);
                return false
            }
            var itens = dijit.byId(idComp).items;
            if (arraySemana.length <= 0) {
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDiaSemanaSelect);
                apresentaMensagem(msg, mensagensWeb);
                return false
            }
            else
                //if (timeIni.validate() && timeIni.validate()) {
                if (dijit.byId("formSemanaFilhaPPT").validate()) {
                    if ((timeFim.value.getMinutes() % 5 != 0 || timeIni.value.getMinutes() % 5 != 0) || validarItemHorarioManual(timeIni, timeFim)) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotInclItemHorario);
                        apresentaMensagem(msg, mensagensWeb);
                        return false;
                    }
                    var dates = {
                        convert: function (dto) {
                            return (
                                dto.constructor === Date ? dto :
                                dto.constructor === Array ? new Date(dto[0], dto[1], dto[2]) :
                                dto.constructor === Number ? new Date(dto) :
                                dto.constructor === String ? new Date(dto) :
                                typeof dto === "object" ? new Date(dto.year, dto.month, dto.date) :
                                NaN
                            );
                        },
                        compare: function (a, b) {
                            return (
                                isFinite(a = this.convert(a).valueOf()) &&
                                isFinite(b = this.convert(b).valueOf()) ?
                                (a > b) - (a < b) :
                                NaN
                            );
                        },
                        inRange: function (dto, start, end) {
                            return (
                                 isFinite(dto = this.convert(dto).valueOf()) &&
                                 isFinite(start = this.convert(start).valueOf()) &&
                                 isFinite(end = this.convert(end).valueOf()) ?
                                 start <= dto && dto <= end :
                                 NaN
                             );
                        },
                        interception: function (start1, end1, start2, end2) {
                            return (
                                 isFinite(a1 = this.convert(start1).valueOf()) &&
                                 isFinite(a2 = this.convert(end1).valueOf()) &&
                                 isFinite(b1 = this.convert(start2).valueOf()) &&
                                 isFinite(b2 = this.convert(end2).valueOf()) ?
                                 ((b1 <= a1 && b2 > a1) || (b2 >= a2 && b1 < a2) || (a1 >= b1 && a2 <= b2) || (b1 >= a1 && b2 <= a2)) :
                                 NaN
                             );
                        },
                        include: function (start1, end1, start2, end2) {
                            return (
                                 isFinite(a1 = this.convert(start1).valueOf()) &&
                                 isFinite(a2 = this.convert(end1).valueOf()) &&
                                 isFinite(b1 = this.convert(start2).valueOf()) &&
                                 isFinite(b2 = this.convert(end2).valueOf()) ?
                                 ((a1 >= b1 && a2 <= b2) ? 1 : ((b1 >= a1 && b2 <= a2) ? 2 : -1)) :
                                 NaN
                             );
                        }
                    }

                    //Faz um clone do store da grade para voltar ao estado original, em caso de ocorrer um erro ao incluir vários horários:
                    calendar.bkpStore = cloneArray(calendar.store.data);
                    for (var i = 0; i < calendar.store.data.length; i++) {
                        calendar.bkpStore[i].startTime = calendar.store.data[i].startTime;
                        calendar.bkpStore[i].endTime = calendar.store.data[i].endTime;
                    }
                    var restaurou = false;

                    for (var i = 0; i < arraySemana.length; i++) {
                        var diaSemana = parseInt(arraySemana[i]) + 1;
                        var d = new Date(2012, 6, diaSemana, timeIni.value.getHours(), timeIni.value.getMinutes());
                        var start, end;
                        var colView = calendar.columnView;
                        var cal = calendar.dateModule;
                        var gradeHorarios = dijit.byId(idComp);
                        var resolveuIntersecao = false;
                        var dataHorarios = gradeHorarios.store.data;
                        var criarItemHorario = true;

                        var cd_turma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;

                        var quantItensMerge = 0;
                        var arraycodHorariosItensMerge = [];
                        var arrayTotalItensMerge = [];
                        var posicaoArray = 0;

                        start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                        //start = d;
                        d = new Date(2012, 6, diaSemana, timeFim.value.getHours(), timeFim.value.getMinutes());
                        id = geradorIdHorarios(gradeHorarios);
                        var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                        var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                        var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                        var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                        var dataRegistro = new Date(start);
                        var diaRegistro = dataRegistro.getDate();

                        if (!resolveuIntersecao) {
                            var item = {
                                id: id,
                                cd_horario: cd_horario,
                                summary: "",
                                startTime: start,
                                calendar: "Calendar2",
                                endTime: d,
                                dt_hora_ini: hIni + ":" + mIni + ":00",
                                dt_hora_fim: hFim + ":" + mFim + ":00",
                                id_dia_semana: start.getDate(),
                                id_disponivel: true
                            };
                            addItemHorariosEdit(item.id, calendar);
                            calendar.store.add(item);
                            colView.set("startTimeOfDay", { hours: hIni, duration: 500 });
                        }

                        //Filtra a lista de itens para trazer os horários do dia inserido.
                        if (hasValue(dataHorarios) && dataHorarios.length > 0) {
                            dataHorarios = jQuery.grep(dataHorarios, function (value) {
                                return value.startTime.getDate() == diaRegistro && value.id != parseInt(item.id);
                            });

                        }

                        /****Verificar se existe interseção com um ou mais horários na grade:****/
                        for (var j = dataHorarios.length - 1; j >= 0; j--) {
                            // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                            if (dataHorarios[j].calendar == "Calendar2" && (dates.interception(start, d, dataHorarios[j].startTime, dataHorarios[j].endTime))) {
                                quantItensMerge += 1;
                                posicaoArray = j;
                                arrayTotalItensMerge.push(dataHorarios[j]);
                                if (hasValue(dataHorarios[j].cd_horario) && dataHorarios[j].cd_horario > 0)
                                    arraycodHorariosItensMerge.push(dataHorarios[j].cd_horario);
                            }
                        }

                        //verifica se não esta fazendo merge com horarios ocupados pela turma PPT.
                        if (!verificarIntersecaoHorariosPPTFilha(item, idComp, msg, VERIFITURMAFILHAPPT2MODAL)) {
                            restauraCalendar(calendar);
                            restaurou = true;
                            return false;
                        }

                        if (quantItensMerge == 1) {
                            criarItemHorario = false;

                            var cd_horario_item_merge = dataHorarios[posicaoArray].cd_horario;
                            var itemMerge = dataHorarios[posicaoArray];
                            if (parseInt(cd_turma) > 0) {
                                if (cd_horario_item_merge > 0 && cd_horario > 0)
                                    restaurou = restaurou || verificarDoisItensHorarioDisponibilidadeTurma(xhr, ref, item, gradeHorarios, dates, resolveuIntersecao,
                                                                                                           calendar, id, itemMerge, EDIT_HORARIO, true, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, null);
                                else
                                    if (cd_horario_item_merge > 0) {
                                        if (id != null && id > 0)
                                            //calendar.store.remove(id);
                                            removeHorario(calendar, id);
                                        item.cd_horario = cd_horario_item_merge;
                                        restaurou = restaurou || verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates,
                                                                                                                 calendar, RESOLVERMERGE, id, itemMerge, EDIT_HORARIO, true, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, null);
                                    } else
                                        if (cd_horario > 0)
                                            restaurou = restaurou || verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, resolveuIntersecao, dates,
                                                                                                                  calendar, RESOLVERMERGE, id, itemMerge, tipoCrud, EDIT_HORARIO, true, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, null);
                                        else {
                                            if (id != null && id > 0)
                                                //calendar.store.remove(id);
                                                removeHorario(calendar, id);
                                            resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFITURMAFILHAPPT2MODAL, xhr, tipoTurma, msg, null);
                                        }
                            } else {
                                if (id != null && id > 0)
                                    removeHorario(calendar, id);
                                resolverMergeItemHorario(item, dates, resolveuIntersecao, calendar, id, itemMerge, EDIT_HORARIO, VERIFITURMAFILHAPPT2MODAL, xhr, tipoTurma, msg, null);
                            }
                        } else if (quantItensMerge > 1) {
                            criarItemHorario = false;
                            restaurou = restaurou || verificarSeexisteProgranacaoParaVariosHorarios(xhr, ref, item, calendar, arrayTotalItensMerge, true, VERIFITURMAFILHAPPT2MODAL);
                        }

                        //Verificação quando ele esta inserindo um registro
                        if (criarItemHorario) {
                            restaurou = restaurou || verifExisteProgHorarioEAlunosDisp(xhr, ref, item, true, null, null, calendar, null, null, null, ADD_HORARIO, true, tipoTurma, VERIFITURMAFILHAPPT2MODAL, msg, NOVO);
                        }
                    }
                }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function carregarHorariosTurmaFilhaPPT(calendar, horarios) {
    try {
        limparTodosHorariosTurmaPPT();
        if (horarios != null && horarios.length > 0)
            $.each(horarios, function (index, value) {
                calendar.store.add(value);
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarProfessoresHorariosTurma(tipoTurma, item, tipoVerif, msg) {
    require(["dojo/store/Memory"], function (Memory) {
        var horariosTurma = [];
        var professores = [];
        var gridProfessor = null;
        var mensagensWeb = new Array();
        if (tipoTurma == TURMA_PPT_FILHA) {
            //Deleções registros pela edição turma PPT filha, através da turma pai
            if (tipoVerif == VERIFITURMAFILHAPPT2MODAL) {
                var gridHorariosTurmaFilha = dijit.byId("gridHorariosPPT");
                var gridProfessoresFulhasPPT = dijit.byId("gridProfessorPPT");
                gridProfessor = gridProfessoresFulhasPPT;
                if (hasValue(gridHorariosTurmaFilha))
                    horariosTurma = mountHorariosOcupadosTurma(gridHorariosTurmaFilha.params.store.data);
                else
                    horariosTurma = dijit.byId("gridAlunosPPT").itensSelecionados[0].horariosTurma;
                if (hasValue(gridProfessoresFulhasPPT))
                    professores = gridProfessoresFulhasPPT.store.objectStore.data;
                //Deleções registros pela edição turma PPT filha, através da tela normal
            } else if (tipoVerif == VERIFTURMANORMAL) {
                var gridHorariosTurma = dijit.byId("gridHorarios");
                if (hasValue(gridHorariosTurma))
                    horariosTurma = mountHorariosOcupadosTurma(gridHorariosTurma.params.store.data);
                gridProfessor = dijit.byId("gridProfessor");
                if (hasValue(gridProfessor))
                    professores = gridProfessor.store.objectStore.data;
            }

        } else {
            //Deleções registros pela edição turma PPT
            if (tipoTurma == TURMA_PPT && tipoVerif == VERIFTURMANORMAL) {
                if (professores != null && professores.length > 0) {
                    mensagensWeb.push(new MensagensWeb(MENSAGEM_AVISO, msgInfProfEHorariosDeletadosTurmaFilhas));
                }
                var gridAlunosPPT = dijit.byId("gridAlunosPPT");
                if (hasValue(gridAlunosPPT)) {
                    //Retira na view
                    $.each(gridAlunosPPT.store.objectStore.data, function (index, value) {
                        if (value.horariosTurma != null) {
                            $.each(value.horariosTurma, function (idx, val) {
                                if (val.HorariosProfessores != null)
                                    val.HorariosProfessores = new Array();
                            });
                        }
                        if (value.ProfessorTurma != null) {
                            for (var i = value.ProfessorTurma.length - 1; i >= 0; i--)
                                value.ProfessorTurma.splice(i, 1); // Remove o item do array
                        }
                    });
                    //Retira na lista completa
                    $.each(gridAlunosPPT.listaCompletaAlunos, function (index, value) {
                        if (value.horariosTurma != null) {
                            $.each(value.horariosTurma, function (idx, val) {
                                if (val.HorariosProfessores != null)
                                    val.HorariosProfessores = new Array();
                            });
                        }
                        if (value.ProfessorTurma != null) {
                            for (var i = value.ProfessorTurma.length - 1; i >= 0; i--)
                                value.ProfessorTurma.splice(i, 1); // Remove o item do array
                        }
                    });
                    //Lista View
                    $.each(gridAlunosPPT.store.objectStore.data, function (index, value) {
                        if (value.horariosTurma != null) {
                            $.each(value.horariosTurma, function (idx, val) {
                                if (val.HorariosProfessores != null)
                                    val.HorariosProfessores = new Array();
                            });
                        }
                        if (value.ProfessorTurma != null) {
                            for (var i = value.ProfessorTurma.length - 1; i >= 0; i--)
                                value.ProfessorTurma.splice(i, 1); // Remove o item do array
                        }
                    });
                }
                horariosTurma = mountHorariosOcupadosTurma(validaRetornaHorarios(dojo.byId("tipoTurmaCad").value));
                gridProfessor = dijit.byId("gridProfessor");
                if (hasValue(gridProfessor))
                    professores = gridProfessor.store.objectStore.data;
                limparHorariosProfTurma();
            } else
                //Deleções registros pela edição turma normnal/regulares
                if (tipoTurma == TURMA && tipoVerif == VERIFTURMANORMAL) {
                    var gridHorariosTurma = dijit.byId("gridHorarios");
                    if (hasValue(gridHorariosTurma))
                        horariosTurma = mountHorariosOcupadosTurma(gridHorariosTurma.params.store.data);
                    gridProfessor = dijit.byId("gridProfessor");
                    if (hasValue(gridProfessor))
                        professores = gridProfessor.store.objectStore.data;
                }
        }

        if (horariosTurma.length > 0) {
            $.each(horariosTurma, function (index, value) {
                if (value.HorariosProfessores != null)
                    value.HorariosProfessores = new Array();
            });
        }
        if (professores.length > 0) {
            for (var i = professores.length - 1; i >= 0; i--)
                professores.splice(i, 1); // Remove o item do array
            gridProfessor.setStore(dojo.data.ObjectStore({ objectStore: new Memory({ data: professores }) }));
            mensagensWeb.push(new MensagensWeb(MENSAGEM_AVISO, msgInfProfEHorariosDeletados));
            apresentaMensagem(msg, mensagensWeb);
        }
    });
}

//#endregion

//#region Desbilita campos para a turma - desbilitarCamposTuma
function desabilitarCamposTuma(tipoTurma, totalAlunos) {
    try {
        var alunos = 0;
        if (parseInt(tipoTurma) == TURMA) {
            alunos = totalAlunos.alunosTurma == null ? totalAlunos : totalAlunos.alunosTurma.length;
            if (alunos != null && alunos > 0) {
                dijit.byId("pesCadProduto").set("disabled", true);
                dijit.byId("pesCadCurso").set("disabled", true);
            } else habilitarCamposTurma(tipoTurma, alunos);
        }
        if (parseInt(tipoTurma) == TURMA_PPT_FILHA) {
            alunos = totalAlunos.alunosTurma == null ? totalAlunos : totalAlunos.alunosTurma.length;
            if (alunos != null && alunos > 0) {
                dijit.byId("pesCadProduto").set("disabled", true);
                dijit.byId("pesCadCurso").set("disabled", true);
            }
        }
        if (parseInt(tipoTurma) == TURMA_PPT) {
            alunos = totalAlunos.alunosTurmasPPTSearch == null ? totalAlunos : totalAlunos.alunosTurmasPPTSearch.length;
            if (alunos != null && alunos > 0) {
                dijit.byId("pesCadProduto").set("disabled", true);
                dijit.byId("pesCadDuracao").set("disabled", true);
                dijit.byId("dtIniAula").set("disabled", true);
            } else habilitarCamposTurma(tipoTurma, alunos)
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function habilitarCamposTurma(tipoTurma, totalAlunos) {
    try {
        if (parseInt(tipoTurma) == TURMA) {
            if (totalAlunos <= 0) {
                dijit.byId("pesCadProduto").set("disabled", false);
                dijit.byId("pesCadCurso").set("disabled", false)
            }
        }
        if (parseInt(tipoTurma) == TURMA_PPT_FILHA) {
            if (totalAlunos <= 0) {
                dijit.byId("pesCadProduto").set("disabled", false);
                dijit.byId("pesCadCurso").set("disabled", false);
            }
        }
        if (parseInt(tipoTurma) == TURMA_PPT) {
            if (totalAlunos <= 0) {
                dijit.byId("pesCadProduto").set("disabled", false);
                dijit.byId("pesCadDuracao").set("disabled", false);
                dijit.byId("dtIniAula").set("disabled", false);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
//#endregion

function cloneHorarioPPTParaFilha(listaHorariosPPT, professorTurma, tipoClone) {
    try {
        var codMaiorHorarioTurma = 1000;
        var listaCompletaHorarios = new Array();

        $.each(listaHorariosPPT, function (idx, value) {
            var id = 0;
            codMaiorHorarioTurma += 1;
            if (tipoClone == CLONAREDUPLICARHORARIO) {
                var item1 = {
                    calendar: "Calendar3",
                    id: codMaiorHorarioTurma,
                    cd_horario: parseInt(value.cd_horario),
                    summary: "",
                    endTime: dojo.date.locale.parse("0" + value.id_dia_semana + "/07/2012 " + value.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                    startTime: dojo.date.locale.parse("0" + value.id_dia_semana + "/07/2012 " + value.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                    dt_hora_ini: value.dt_hora_ini,
                    dt_hora_fim: value.dt_hora_fim,
                    id_dia_semana: value.id_dia_semana,
                    id_disponivel: value.id_disponivel,
                    no_datas: value.no_datas,
                    HorariosProfessores: new Array()
                };
                if (value.HorariosProfessores != null) {
                    item1.HorariosProfessores = new Array();
                    if (value.HorariosProfessores.length > 0)
                        $.each(value.HorariosProfessores, function (idx, val) {
                            item1.HorariosProfessores.push({ cd_horario_professor_turma: 0, cd_horario: 0, cd_professor: val.cd_professor });
                        });
                }
                listaCompletaHorarios.push(item1);
                codMaiorHorarioTurma += 1;
            }
            var item2 = {
                calendar: "Calendar2",
                id: codMaiorHorarioTurma,
                cd_horario: 0,
                summary: "",
                endTime: dojo.date.locale.parse("0" + value.id_dia_semana + "/07/2012 " + value.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                startTime: dojo.date.locale.parse("0" + value.id_dia_semana + "/07/2012 " + value.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                dt_hora_ini: value.dt_hora_ini,
                dt_hora_fim: value.dt_hora_fim,
                id_dia_semana: value.id_dia_semana,
                id_disponivel: value.id_disponivel,
                no_datas: value.no_datas,
                HorariosProfessores: new Array()
            };
            if (value.HorariosProfessores != null) {
                item2.HorariosProfessores = new Array();
                if (value.HorariosProfessores.length > 0)
                    $.each(value.HorariosProfessores, function (idx, val) {
                        item2.HorariosProfessores.push({ cd_horario_professor_turma: 0, cd_horario: 0, cd_professor: val.cd_professor });
                    });
            }

            if (hasValue(professorTurma))
                for (var i = 0; i < professorTurma.length; i++)
                    for (var j = professorTurma[i].horarios.length - 1; j >= 0; j--)
                        if (professorTurma[i].horarios[j].id == value.id)
                            professorTurma[i].horarios[j].id = codMaiorHorarioTurma;
            listaCompletaHorarios.push(item2);

        });
        return listaCompletaHorarios;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clonarProfessorTurmaEHorarios(listProf) {
    try {
        var listaCompletaProfessores = new Array();
        $.each(listProf, function (idx, value) {
            var item = {
                cd_professor: value.cd_professor,
                cd_professor_turma: value.cd_professor_turma,
                cd_turma: value.cd_turma,
                horarios: new Array(),
                id_professor_ativo: value.id_professor_ativo,
                no_professor: value.no_professor
            };
            if (value.horarios != null && value.horarios.length > 0)
                $.each(value.horarios, function (idx, val) {
                    var item2 = {
                        calendar: "Calendar2",
                        id: val.id,
                        cd_horario: 0,
                        summary: "",
                        endTime: dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                        startTime: dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                        dt_hora_ini: val.dt_hora_ini,
                        dt_hora_fim: val.dt_hora_fim,
                        dia_semana: val.dia_semana,
                        id_dia_semana: val.id_dia_semana,
                        id_disponivel: val.id_disponivel,
                        no_datas: val.no_datas,
                        HorariosProfessores: new Array()
                    };
                    if (val.HorariosProfessores != null) {
                        item2.HorariosProfessores = new Array();
                        if (val.HorariosProfessores.length > 0)
                            $.each(val.HorariosProfessores, function (idx, val) {
                                item2.HorariosProfessores.push({ cd_horario_professor_turma: 0, cd_horario: 0, cd_professor: val.cd_professor });
                            });
                    }
                    item.horarios.push(item2);
                });
            listaCompletaProfessores.push(item);
        });
        return listaCompletaProfessores;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clonarHorarios(listHorarios) {
    try {
        var listaCompleta = new Array();
        $.each(listHorarios, function (idx, val) {
            var item = {
                calendar: val.calendar,
                id: val.id,
                cd_horario: (val.cd_horario == null || val.cd_horario == undefined) ? 0 : val.cd_horario,
                summary: "",
                endTime: dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                startTime: dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                dt_hora_ini: val.dt_hora_ini,
                dt_hora_fim: val.dt_hora_fim,
                id_dia_semana: val.id_dia_semana,
                dia_semana: hasValue(val.dia_semana) ? val.dia_semana : "",
                id_disponivel: val.id_disponivel,
                no_datas: val.no_datas,
                HorariosProfessores: new Array()
            };
            if (val.HorariosProfessores != null) {
                item.HorariosProfessores = new Array();
                if (val.HorariosProfessores.length > 0)
                    $.each(val.HorariosProfessores, function (idx, val) {
                        item.HorariosProfessores.push({ cd_horario_professor_turma: val.cd_horario_professor_turma, cd_horario: val.cd_horario, cd_professor: val.cd_professor });
                    });
            }
            listaCompleta.push(item);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
    return listaCompleta;
}

function refazerProgramacaoTurmasFilhas(apresentadorMsg, item, xhr, tipoCrud, maisDeUmHorario) {
    try {
        var gridAlunosPPT = dijit.byId("gridAlunosPPT");
        if (hasValue(gridAlunosPPT)) {
            if (gridAlunosPPT.listaCompletaAlunos.length > 0) {
                var mensagensWeb = new Array();
                mensagensWeb.push(new MensagensWeb(MENSAGEM_AVISO, msgInfProgramacaoRefeitaTurmaFilhas));
                apresentaMensagem(apresentadorMsg, mensagensWeb, true);
            }
            var contadorShowCarregando = 0;
            var cloneDataTurmasFilhasPPT = gridAlunosPPT.listaCompletaAlunos.slice();
            var cloneDataViewTurmasFilhasPPT = gridAlunosPPT.store.objectStore.data.slice();
            $.each(gridAlunosPPT.listaCompletaAlunos, function (index, value) {
                var horariosOcupadoTurma = new Array();
                if (tipoCrud == ADD_HORARIO || tipoCrud == EDIT_HORARIO) {
                    if (!maisDeUmHorario) {
                        //quickSortObj(cloneDataTurmasFilhasPPT, 'cd_horario');
                        //var posicao = binaryObjSearch(cloneDataTurmasFilhasPPT, 'id', item.cd_registro);
                        //if (hasValue(horariosProfessor) && !hasValue(posicao, true)) {
                        value.horariosTurma.push(clonarHorariosZerado(item, value.horariosTurma));
                    } else
                        $.each(item, function (idx, val) {
                            value.horariosTurma.push(clonarHorariosZerado(val, value.horariosTurma));
                        });
                }
                if (tipoCrud == DELETE_HORARIO && value.horariosTurma != null)
                    for (var j = value.horariosTurma.length - 1; j >= 0; j--) {
                        if (item.id_dia_semana == value.horariosTurma[j].id_dia_semana &&
                                           value.horariosTurma[j].dt_hora_ini >= item.dt_hora_ini && value.horariosTurma[j].dt_hora_ini < item.dt_hora_fim &&
                                           value.horariosTurma[j].dt_hora_fim > item.dt_hora_ini && value.horariosTurma[j].dt_hora_fim <= item.dt_hora_fim)
                            value.horariosTurma.splice(j, 1);
                    }

                if (hasValue(value.horariosTurma))
                    horariosOcupadoTurma = value.horariosTurma;
                var cd_curso = value.cd_curso;
                var cd_duracao = value.cd_duracao;
                var dtIniAula = value.dt_inicio_aula;
                var refazer_programacao = TIPO_REFAZER_PROGRAMACOES_HORARIO;
                var cd_turma = value.cd_turma;
                var programacoes = null;
                if (hasValue(value.ProgramacaoTurma)) {
                    programacoes = new Array();
                    var dataProgramacoes = value.ProgramacaoTurma;
                    for (var i = 0; i < dataProgramacoes.length; i++)
                        programacoes.push({
                            cd_programacao_turma: dataProgramacoes[i].cd_programacao_turma,
                            id_aula_dada: dataProgramacoes[i].id_aula_dada,
                            cd_feriado: dataProgramacoes[i].cd_feriado,
                            cd_feriado_desconsiderado: dataProgramacoes[i].cd_feriado_desconsiderado,
                            id_feriado_desconsiderado: dataProgramacoes[i].id_feriado_desconsiderado,
                            dc_programacao_turma: dataProgramacoes[i].dc_programacao_turma,
                            dta_programacao_turma: dojo.date.locale.parse(dataProgramacoes[i].dt_programacao_turma, { formatLength: 'short', selector: 'date', locale: 'pt-br' }),
                            hr_inicial_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[0] : '',
                            hr_final_programacao: hasValue(dataProgramacoes[i].hr_aula) ? dataProgramacoes[i].hr_aula.split(' ')[1] : '',
                            nm_aula_programacao_turma: dataProgramacoes[i].nm_aula_programacao_turma,
                            id_programacao_manual: dataProgramacoes[i].id_programacao_manual
                        });
                }

                var feriados_descosiderados = null;
                var dataFeriadosDesconsiderados = null;
                if (hasValue(value.FeriadosDesconsiderados)) {
                    feriados_descosiderados = new Array();
                    dataFeriadosDesconsiderados = value.FeriadosDesconsiderados;
                    for (var i = 0; i < dataFeriadosDesconsiderados.length; i++)
                        feriados_descosiderados.push({
                            cd_feriado_desconsiderado: dataFeriadosDesconsiderados[i].cd_feriado_desconsiderado,
                            dt_inicial: dataFeriadosDesconsiderados[i].dt_inicial,
                            dt_final: dataFeriadosDesconsiderados[i].dt_final,
                            id_programacao_feriado: dataFeriadosDesconsiderados[i].id_programacao_feriado
                        });
                }
                xhr.post({
                    url: Endereco() + "/api/coordenacao/postProgramacoesTurma",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    preventCache: true,
                    postData: dojox.json.ref.toJson({
                        programacoes: programacoes,
                        horarios: horariosOcupadoTurma,
                        dt_inicio: dtIniAula,
                        cd_curso: cd_curso,
                        cd_duracao: cd_duracao,
                        cd_turma: parseInt(cd_turma),
                        tipo: refazer_programacao,
                        feriados_desconsiderados: feriados_descosiderados
                    })
                }).then(function (dataProgTurma) {
                    try {
                        dataProgTurma = jQuery.parseJSON(dataProgTurma);
                        if (hasValue(dataProgTurma.retorno)) {
                            value.ProgramacaoTurma = dataProgTurma.retorno.Programacoes;
                            value.FeriadosDesconsiderados = dataProgTurma.retorno.FeriadosDesconsiderados;
                            value.alterouProgramacaoOuDescFeriado = true;
                        }
                        if (hasValue(gridAlunosPPT.store.objectStore.data))
                            for (var i = 0; i < gridAlunosPPT.store.objectStore.data.length; i++)
                                if (gridAlunosPPT.store.objectStore.data[i].cd_aluno == value.cd_aluno)
                                    gridAlunosPPT.store.objectStore.data[i] = jQuery.extend({}, value);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    gridAlunosPPT.listaCompletaAlunos = cloneDataTurmasFilhasPPT;
                    gridAlunosPPT.store.objectStore.data = cloneDataViewTurmasFilhasPPT;
                    apresentaMensagem(apresentadorMsg, error, true);
                });
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function geradorIdPoListaHorarios(listHorarios, horariosTurma) {
    try {
        var id = listHorarios.length;
        var itensArray = listHorarios.sort(function byOrdem(a, b) { return a.id - b.id; });
        if (id > 0)
            id = itensArray[id - 1].id + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function clonarHorariosZerado(item, horariosTurma) {
    try {
        var itemRet = {
            calendar: item.calendar,
            id: geradorIdPoListaHorarios(horariosTurma),
            cd_horario: 0,
            summary: "",
            endTime: dojo.date.locale.parse("0" + item.id_dia_semana + "/07/2012 " + item.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
            startTime: dojo.date.locale.parse("0" + item.id_dia_semana + "/07/2012 " + item.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
            dt_hora_ini: item.dt_hora_ini,
            dt_hora_fim: item.dt_hora_fim,
            id_dia_semana: item.id_dia_semana,
            dia_semana: hasValue(item.dia_semana) ? item.dia_semana : "",
            id_disponivel: item.id_disponivel,
            no_datas: hasValue(item.no_datas) ? item.no_datas : "",
            HorariosProfessores: new Array()
        }
        return itemRet;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirMatricula(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, alunoSelecionado, iPPT) {
    var id_turma_ppt = false;
    var curso = 0;
    var cdTurmaPPT = hasValue(dojo.byId("cd_turma_ppt").value) ? dojo.byId("cd_turma_ppt").value : 0;
    if (cdTurmaPPT > 0)
        id_turma_ppt = true;
    if (iPPT) {
        id_turma_ppt = true;
        curso = alunoSelecionado.cd_curso;
    }
    else {
        curso = dijit.byId("pesCadCurso").value;
    }
    ready(function () {
        xhr.get({
            url: Endereco() + "/api/secretaria/getExisteMatriculaByProduto?produto=" + dijit.byId("pesCadProduto").value + "&cdAluno=" + alunoSelecionado.cd_aluno + "&dtIniAula=" + 
                dojo.byId("dtIniAula").value + "&curso=" + curso + "&id_turma_ppt=" + id_turma_ppt + "&cd_contrato=0" + "&dtFimAula=" + dojo.byId("dtFimAula").value +
                "&cd_duracao=" + dijit.byId("pesCadDuracao").value,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                if (!data) {
                    if (hasValue(dojo.byId("dtaTermino").value)) {
                        caixaDialogo(DIALOGO_AVISO, msgMatriculaTurmaEnc, null);
                        return false;
                    }
                    else if (dojo.byId("cd_turma").value <= 0 || (dojo.byId("idTurmaPPTPai").checked && alunoSelecionado.cd_turma <= 0) ||
                        (dojo.byId("cd_turma_ppt").value <= 0 && alunoSelecionado.cd_turma <= 0)) {
                        caixaDialogo(DIALOGO_AVISO, msgExistirTurma, null);
                        return false;
                    }
                    else {
                        //Verifica se o aluno está ativo e copia os seus dados:

                        dijit.byId("notasMaterialDidatico").set("disabled", false);
                        dijit.byId("notasMaterialDidatico").set("required", false);

                        if ((hasValue(alunoSelecionado.id_aluno_ativo) && !alunoSelecionado.id_aluno_ativo) ||
                            (hasValue(alunoSelecionado.id_ativo) && !alunoSelecionado.id_ativo)) {
                            var mensagensWeb = [];
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgAlunoInativo);
                            apresentaMensagem('apresentadorMensagemTurma', mensagensWeb);
                            return false;

                        }
                        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                        if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
                            montarCadastroMatriculaPartial(function () {
                                dijit.byId("alterarMatricula").on("click", function () {
                                    //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                                    alterarMatricula();
                                });
                                montarMatricula(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, alunoSelecionado);
                                dadosAlunoTurma(xhr, alunoSelecionado.cd_aluno, alunoSelecionado.cd_turma);
                            }, Permissoes);
                        } else {
                            montarMatricula(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, alunoSelecionado);
                            dadosAlunoTurma(xhr, alunoSelecionado.cd_aluno, alunoSelecionado.cd_turma);
                        }
                    }
                } else {
                    caixaDialogo(DIALOGO_AVISO, msgAlunoMatriculadoProd, null);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemTurma', error);
        });
    });
}

function montarMatricula(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, alunoSelecionado) {
    try {
        var cd_responsavel = "";
        var no_pessoa_responsavel = "";

        if (alunoSelecionado.PessoaSGFQueUsoOCpf == null) {
            cd_responsavel = alunoSelecionado.cd_pessoa_aluno;
            no_pessoa_responsavel = alunoSelecionado.no_aluno;
        }
        else {
            cd_responsavel = alunoSelecionado.PessoaSGFQueUsoOCpf.cd_pessoa;
            no_pessoa_responsavel = alunoSelecionado.PessoaSGFQueUsoOCpf.no_pessoa;
        }
        var dadosAluno = {
            cd_aluno: alunoSelecionado.cd_aluno,
            nom_aluno: alunoSelecionado.no_aluno,
            cd_pessoa_aluno: alunoSelecionado.cd_pessoa_aluno,
            cd_pessoa_responsavel: cd_responsavel,
            no_pessoa_responsavel: no_pessoa_responsavel,
            pc_bolsa: alunoSelecionado.pc_bolsa,
            pc_bolsa_material: alunoSelecionado.pc_bolsa_material,
            dt_inicio_bolsa: alunoSelecionado.dt_inicio_bolsa,
            vl_abatimento_matricula :alunoSelecionado.vl_abatimento_matricula
        }
        criaNovaMatricula(xhr, Memory, dadosAluno, 2, ObjectStore,
            function () {
                //Professor Ativo
                var noProfessor = "";
                for (var i = 0; i < dijit.byId("gridProfessor")._by_idx.length; i++)
                    if (dijit.byId("gridProfessor")._by_idx[i].item.id_professor_ativo == true)
                        noProfessor = dijit.byId("gridProfessor")._by_idx[i].item.no_professor;

                //Colocando a turma na grade Turma Matrícula
                gridTurmaMat = dijit.byId("gridTurmaMat");
                var dados = [];
                if (!dojo.byId("idTurmaPPTPai").checked) {
                    insertObjSort(dados, "cd_turma", {
                        cd_turma: dojo.byId("cd_turma").value,
                        no_turma: dojo.byId("noTurma").value,
                        no_produto: dojo.byId("pesCadProduto").value,
                        no_professor: noProfessor,
                        cd_situacao_aluno_turma: 9,
                        situacaoAlunoTurma: alunoSelecionado.situacaoAlunoTurma,
                        cd_aluno: alunoSelecionado.cd_aluno,
                        cd_pessoa_aluno: alunoSelecionado.cd_pessoa_aluno,
                        dt_matricula: hasValue(dojo.byId("dtaMatricula").value) ? dojo.date.locale.parse(dojo.byId("dtaMatricula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        dt_movimento: new Date(),
                        dt_inicio: hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        nm_matricula_turma: dojo.byId("matriculaNro").value.replace('.', ''),
                        cd_pessoa_responsavel: alunoSelecionado.cd_pessoa_aluno,
                        no_responsavel: alunoSelecionado.no_aluno
                    });

                    populaSemTurma(dijit.byId("pesCadProduto").value, dijit.byId("pesCadCurso").value, dijit.byId("pesCadDuracao").value, dijit.byId("pesCadRegime").value, 0, xhr, Memory, null, null, true);
                    getValoresForMatricula(xhr, dijit.byId("pesCadCurso").value, dijit.byId("pesCadDuracao").value, dijit.byId("pesCadRegime").value, false);
                }
                else {
                    insertObjSort(dados, "cd_turma", {
                        cd_turma: alunoSelecionado.cd_turma,
                        no_turma: alunoSelecionado.no_turma,
                        no_produto: dojo.byId("pesCadProduto").value,
                        no_professor: noProfessor,
                        cd_situacao_aluno_turma: 9,
                        situacaoAlunoTurma: msgAguardandoMat,
                        cd_aluno: alunoSelecionado.cd_aluno,
                        cd_pessoa_aluno: alunoSelecionado.cd_pessoa_aluno,
                        dt_matricula: hasValue(dojo.byId("dtaMatricula").value) ? dojo.date.locale.parse(dojo.byId("dtaMatricula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        dt_movimento: new Date(),
                        dt_inicio: hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        nm_matricula_turma: dojo.byId("matriculaNro").value.replace('.', ''),
                        cd_pessoa_responsavel: alunoSelecionado.cd_pessoa_aluno,
                        no_responsavel: alunoSelecionado.no_aluno
                    });
                    populaSemTurma(dijit.byId("pesCadProduto").value, alunoSelecionado.cd_curso, alunoSelecionado.cd_duracao, alunoSelecionado.cd_regime, 0, xhr, Memory, null, null, true);

                    getValoresForMatricula(xhr, alunoSelecionado.cd_curso, alunoSelecionado.cd_duracao, alunoSelecionado.cd_regime, false);

                }
                //slice(0)
                gridTurmaMat.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));



                dojo.byId("tagSemTurma").style.display = "none";
            });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaContrato(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, gridAlunos, iPPT) {
    try {
        if (!hasValue(gridAlunos.itensSelecionados) || gridAlunos.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else if (gridAlunos.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
            return false;
        }
        else if (hasValue(gridAlunos.itensSelecionados[0].cd_pessoa_escola_aluno) && (gridAlunos.itensSelecionados[0].cd_pessoa_escola_aluno + "") !== dojo.byId("_ES0").value) {
            caixaDialogo(DIALOGO_AVISO, msgErroMatriculaAlunoOutroaEscola, null);
            return false;
        }
        else {
            if (gridAlunos.id == "gridAlunosPPT" && hasValue(gridAlunos.itensSelecionados[0].pc_bolsa))
                gridAlunos.itensSelecionados[0].alunoTurma.pc_bolsa = gridAlunos.itensSelecionados[0].pc_bolsa;
            var alunoSelecionado = gridAlunos.itensSelecionados[0];

            var cdTurma = !dojo.byId("idTurmaPPTPai").checked ? dojo.byId("cd_turma").value : alunoSelecionado.cd_turma;
            alunoSelecionado.cd_turma = cdTurma;
            alunoSelecionado.dt_inicio = hasValue(dojo.byId("dtIniAula").value) ? dojo.date.locale.parse(dojo.byId("dtIniAula").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
            xhr.get({
                url: Endereco() + "/api/secretaria/getMatriculaByTurmaAluno?cdTurma=" + cdTurma + "&cdAluno=" + alunoSelecionado.cd_aluno,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (hasValue(data.retorno) && data.retorno.cd_contrato > 0) {
                        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                        if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat")))
                            montarCadastroMatriculaPartial(function () {
                                dijit.byId("alterarMatricula").on("click", function () {
                                    //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                                    alterarMatricula();
                                });
                                dijit.byId("cadMatricula").show();
                                dijit.byId('tabContainerMatricula').resize();
                                IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');
                                habilitarOnChange("ckAula", false);
                                keepValuesMatricula(data.retorno, null, false, xhr, ready, Memory, FilteringSelect, ObjectStore);
                                habilitarOnChange("ckAula", true);
                                if (alunoSelecionado.cd_situacao_aluno_origem != null) {
                                    if (alunoSelecionado.cd_situacao_aluno_origem == ENCERRADO)
                                        dijit.byId("tipoMatricula").set("value", 2);
                                }
                                dijit.byId("notasMaterialDidatico").set("disabled", false);
                                dijit.byId("notasMaterialDidatico").set("required", false);
                            }, Permissoes);
                        else {
                            dijit.byId("cadMatricula").show();
                            dijit.byId('tabContainerMatricula').resize();
                            IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');
                            habilitarOnChange("ckAula", false);
                            keepValuesMatricula(data.retorno, null, false, xhr, ready, Memory, FilteringSelect, ObjectStore);
                            habilitarOnChange("ckAula", true);
                        }

                    } else
                        abrirMatricula(xhr, Memory, FilteringSelect, ready, ObjectStore, ref, alunoSelecionado, iPPT);
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagemTurma', error);
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirPesquisaTurmaFK(Memory, ready) {
    try {
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de aluno na turma
        dojo.byId('tipoRetornoAlunoFK').value = CADASTROTURMA;
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = TURMACADASTRO;
        limparFiltrosTurmaFK();
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        montarPadraoFiltroTurmaFK(Memory);
        pesquisarTurmaFK();
        dijit.byId("proTurmaFK").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function inicial() {
    try {
        dojo.ready(function () {
            //$("#telefone").mask("(99) 9999-9999");
            dijit.byId("timeIni")._onChangeActive = false;
            dijit.byId("timeFim")._onChangeActive = false;
            maskHour("#timeIni");
            maskHour("#timeFim");
            dijit.byId("timeIni")._onChangeActive = true;
            dijit.byId("timeFim")._onChangeActive = true;
            // maskDate("#dtIniAula");
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region Configura as ações do diário de aula.
function setarEventosBotoesPrincipais(xhr, on) {
    try {
        dijit.byId("incluirDiario").on("click", function () {
            apresentaMensagem('apresentadorMensagemDiarioPartial', null);
            apresentaMensagem('apresentadorMensagem', null);
            salvarDiarioAulaTurma(xhr);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarDiarioAulaTurma(xhr) {
    try {
        var diario = null;
        if (!dijit.byId("formDiarioPrincipal").validate()) {
            setarTabCadDiarioAula();
            return false;
        }
        diario = mountDataForPostDiadrio();
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/postInsertDiarioAula",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(diario)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                hideCarregando();
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cadDiarioAula").hide();
                    
                } else {
                    apresentaMensagem('apresentadorMensagemDiarioPartial', data);
                    
                }
                    
            }
            catch (e) {
                hideCarregando();
                postGerarLog(e);
                
            }
        }, function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function atualizarGradesTabPrincipal() {
    try {
        var gridProfessor = dijit.byId("gridProfessor");
        var gridAlunos = dijit.byId("gridAlunos");
        if (gridProfessor != null)
            gridProfessor.update();
        if (gridAlunos != null)
            gridAlunos.update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#endregion

//#region Métodos para a desistência

function populaDesistencia(gridAlunos, isPPT) {
    try {
        if (!hasValue(gridAlunos.itensSelecionados) || gridAlunos.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else if (gridAlunos.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
            return false;
        }
        else {
            var msg = "apresentadorMensagemTurma";
            var alunoSelecionadoDes = [];
            if (!isPPT)
                alunoSelecionadoDes = gridAlunos.itensSelecionados[0];
            else {
                alunoSelecionadoDes = gridAlunos.itensSelecionados[0].alunoTurma;
                alunoSelecionadoDes.cd_pessoa_aluno = gridAlunos.itensSelecionados[0].cd_pessoa_aluno;
                alunoSelecionadoDes.no_aluno = gridAlunos.itensSelecionados[0].no_aluno;
                msg = "apresentadorMensagemTurmaPPT";
            }

            if (alunoSelecionadoDes.cd_situacao_aluno_turma == ATIVO || alunoSelecionadoDes.cd_situacao_aluno_turma == REMATRICULADO ||
                alunoSelecionadoDes.cd_situacao_aluno_turma == DESISTENTE) {
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (dijit.byId('cbEscolhaCad').store.data.length <= 0)
                    montarCadastroDesistencia(function () { dadosDesistencia(alunoSelecionadoDes); }, Permissoes);
                else dadosDesistencia(alunoSelecionadoDes);
            }
            else {
                //msg avisando q não é possível desistir um aluno desse status
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgErroDesistirAlunoStatus);
                apresentaMensagem(msg, mensagensWeb);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function dadosDesistencia(alunoSelecionadoDes) {
    try {
        limparCadastro();
        abrirCadastroDesistencia();
        showCarregando();
        //Turma
        dojo.byId("nome_turma").value = dojo.byId("noTurma").value;
        var tipoTurma = dojo.byId("tipoTurmaCad").value;
        var tipoVerif = dojo.byId("tipoVerif").value;
        dojo.byId("cdTurmaCad").value = dojo.byId("cd_turma").value;
        if (tipoTurma == TURMA_PPT && tipoVerif == VERIFITURMAFILHAPPT2MODAL)
            dojo.byId("cdTurmaCad").value = dojo.byId("cd_turmaPPT").value;
        dijit.byId('limparTurmaCad').set("disabled", false);
        dijit.byId("pesAlunoFKCad").set("disabled", false);
        //Aluno
        dojo.byId("cdAlunoCad").value = alunoSelecionadoDes.cd_aluno_turma;
        dojo.byId('cdAlunoDesistencia').value = alunoSelecionadoDes.cd_aluno;
        dojo.byId("nome_aluno").value = alunoSelecionadoDes.no_aluno;

        var cd_aluno = alunoSelecionadoDes.cd_pessoa_aluno;
        var cd_contrato = alunoSelecionadoDes.cd_contrato;
        var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
        returnLocalMovimento(cd_aluno, cd_contrato, Permissoes);

        dijit.byId('limparAlunoCad').set("disabled", false);

        switch (alunoSelecionadoDes.cd_situacao_aluno_turma) {
            case DESISTENTE: {
                dijit.byId("cbTipoCad").set("value", CANCELADESISTENCIA);
                break;
            }
            default: {
                dijit.byId("cbTipoCad").set("value", TP_DESISTENCIA);
                break;
            }
        }

        dijit.byId("cbTipoCad").set("disabled", true);
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

//#endregion

//Pesquisa de Alunos
function pesquisarAlunoNaTurmaPPT() {
    try {
	    var mensagensWeb = new Array();
        var cdTurma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var cd_produto = hasValue(dijit.byId("pesCadProduto").value) ? dijit.byId("pesCadProduto").value : 0;
        var cd_curso = hasValue(dijit.byId("pesCadCurso").value) ? dijit.byId("pesCadCurso").value : 0;
        if (cd_curso ==0)
            var cd_curso = hasValue(dijit.byId("descCursoPPT").value) ? dijit.byId("gridPesquisaCurso").itensSelecionados[0].cd_curso : 0;
        var cd_duracao = hasValue(dijit.byId("pesCadDuracao").value) ? dijit.byId("pesCadDuracao").value : 0;
        var horarios = [];
        //Configuração retorno fk de aluno na turma
        dojo.byId('tipoRetornoAlunoFK').value = CADASTROTURMA;
        //Configuração retorno fk de aluno nas varias chamdas de aluno dentro da turma.
        dojo.byId("setValuePesquisaAlunoFK").value = PESQUISAALUNOPPT;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);
        if (!hasValue(dijit.byId("pesCadProduto").value)) {
	        
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoObrigtProdutoOrHorario);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        if (horarios == null || horarios.length <= 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrInclHorarioTurma);
            apresentaMensagem("apresentadorMensagemAlunoCursoPPT", mensagensWeb);
            return false;
        }
        if (!hasValue(dijit.byId("pesCadCurso").value) && !hasValue(dijit.byId("descCursoPPT").value)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoObrigtProdutoOrHorario);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        if (!hasValue(dijit.byId("pesCadDuracao").value)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoObrigtProdutoOrHorario);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        pesquisaAlunoDisponivelHorario(horarios, cdTurma, cd_produto, true, cd_curso, cd_duracao);
        dijit.byId("proAluno").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarAlunoDisponiveisNaTurma() {
    try {
        var id_turma_ppt = false;
        var mensagensWeb = [];
        var cdTurma = hasValue(dojo.byId("cd_turma").value) ? dojo.byId("cd_turma").value : 0;
        var cdTurmaPPT = hasValue(dojo.byId("cd_turma_ppt").value) ? dojo.byId("cd_turma_ppt").value : 0;
        var cd_produto = hasValue(dijit.byId("pesCadProduto").value) ? dijit.byId("pesCadProduto").value : 0;
        var cd_curso = hasValue(dijit.byId("pesCadCurso").value) ? dijit.byId("pesCadCurso").value : 0;
        var cd_duracao = hasValue(dijit.byId("pesCadDuracao").value) ? dijit.byId("pesCadDuracao").value : 0;
        if (cdTurmaPPT > 0)
            id_turma_ppt = true;
        //Configuração retorno fk de aluno na turma
        dojo.byId('tipoRetornoAlunoFK').value = CADASTROTURMA;
        //Configuração retorno fk de aluno nas varias chamdas de aluno dentro da turma.
        dojo.byId("setValuePesquisaAlunoFK").value = PESQUISAALUNOCAD;
        var horarios = [];
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        horarios = validaRetornaHorarios(dojo.byId("tipoTurmaCad").value);
        if (!hasValue(dijit.byId("pesCadProduto").value)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoObrigtProdutoOrHorario);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        if (horarios == null || horarios.length <= 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrInclHorarioTurma);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        if (!hasValue(dijit.byId("pesCadCurso").value)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoObrigtProdutoOrHorario);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        if (!hasValue(dijit.byId("pesCadDuracao").value)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgInfoObrigtProdutoOrHorario);
            apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
            return false;
        }
        pesquisaAlunoDisponivelHorario(horarios, cdTurma, cd_produto, id_turma_ppt, cd_curso, cd_duracao)
        dijit.byId("proAluno").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

//#region métodos para  o aluno - pesquisarAlunoFK

function abrirAlunoFKPesquisaTurma() {
    try {
        //Configuração retorno fk de aluno na turma
        dojo.byId('tipoRetornoAlunoFK').value = CADASTROTURMA;
        //Configuração retorno fk de aluno nas varias chamdas de aluno dentro da turma.
        dojo.byId("setValuePesquisaAlunoFK").value = PESQUISAALUNOFILTRO;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        limparPesquisaAlunoFK();
        pesquisaAlunoTurmaFKTurmaFK();
        dijit.byId("proAluno").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFKPesquisaCadTurmaNornal() {
    try {
        //Configuração retorno fk de aluno na turma
        dojo.byId('tipoRetornoAlunoFK').value = CADASTROTURMA;
        //Configuração retorno fk de aluno nas varias chamdas de aluno dentro da turma, esta configurado na pesquisa abaixo.
        limparPesquisaAlunoFK();
        pesquisarAlunoDisponiveisNaTurma();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function chamarPesquisaTurmaFKTurma() {
    try {
        //PESQUISAALUNOCAD = 1, PESQUISAALUNOPPT = 2, PESQUISAALUNOFILTRO = 3
        var tipoPesquisa = dojo.byId("setValuePesquisaAlunoFK").value;
        if (hasValue(tipoPesquisa)) {
            if (tipoPesquisa == PESQUISAALUNOPPT)
                pesquisarAlunoNaTurmaPPT();
            if (tipoPesquisa == PESQUISAALUNOCAD)
                pesquisarAlunoDisponiveisNaTurma();
            if (tipoPesquisa == PESQUISAALUNOFILTRO)
                pesquisaAlunoTurmaFKTurmaFK();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function validarItemHorarioManual(timeIni, timeFim) {
    try {
        var retorno = false;
        var hIni = timeIni.value.getHours();
        var hFim = timeFim.value.getHours();
        var mIni = timeIni.value.getMinutes();
        var mFim = timeFim.value.getMinutes();

        var totalMinitosInicial = (parseInt(hIni) * 60) + parseInt(mIni);
        var totalMinutosFinal = (parseInt(hFim) * 60) + parseInt(mFim);
        if (totalMinutosFinal - totalMinitosInicial <= 5)
            retorno = true;
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// Nova turma via encerramento
function criaDadosNovaTurma(cdTurma, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton, eTurmaNova) {
    if (cdTurma != null && cdTurma > 0 && cdTurma != "undefined" && document.getElementById("ckViradaTurma").checked == false) {
        try {
            novaTurmaEnc = true;
            dojo.xhr.get({
                url: Endereco() + "/api/turma/getInsertTurmaEnc?cdTurma=" + cdTurma,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var gridTurma = dijit.byId('gridTurma');
                        if (novaTurmaEnc) {
                            apresentaMensagem('apresentadorMensagemEncNovaTurma', '');
                            dijit.byId('dialogEncerramento3').hide();
                        }
                        else
                            apresentaMensagem('apresentadorMensagem', '');
                        gridTurma.itemSelecionado = gridTurma.itensSelecionados[0];
                        setarTabCad();
                        destroyCreateGridHorario();
                        destroyCreateProgramacao();
                        dijit.byId("pesCadRegime").set("disabled", true);
                        virada = false;
                        keepValues(data.retorno, gridTurma, true, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton);
                        IncluirAlterar(0, 'divAlterarTurma', 'divIncluirTurma', 'divExcluirTurma', 'apresentadorMensagemATurma', 'divCancelarTurma', 'divClearTurma');
                        popularGridEscolaTurma();
                        dijit.byId("cadTurma").show();
                    }
                    else {
                        if (novaTurmaEnc)
                            apresentaMensagem('apresentadorMensagemEncNovaTurma', data);
                        else
                            apresentaMensagem('apresentadorMensagem', data);
                    }
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                if (novaTurmaEnc)
                    apresentaMensagem('apresentadorMensagemEncNovaTurma', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    } else {
        // pegando as datas selecionadas para jogar no Json
        var data_ini = dojo.date.locale.parse(dojo.byId("dtInicialR").value,
            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        data_ini = dojo.date.locale.format(data_ini,
            { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });

        var data_termino = dojo.date.locale.parse(dojo.byId("dt_fim_enc").value,
           { formatLength: 'short', selector: 'date', locale: 'pt-br' });
        data_termino = dojo.date.locale.format(data_termino,
            { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
        
        if (hasValue(dijit.byId("dtFinalR"))) {
            var data_end = dojo.date.locale.parse(dojo.byId("dtFinalR").value,
                { formatLength: 'short', selector: 'date', locale: 'pt-br' });
            data_end = dojo.date.locale.format(data_end,
                { selector: "date", datePattern: "MM/dd/yyyy", formatLength: "short", locale: "pt-br" });
        } else {
            var data_end = null;
        }

        if (eTurmaNova === true) {
            var id_turma_nova = 1;
        }
        if (eTurmaNova === false) {
            var id_turma_nova = 0;
        }
       
        dt_inicial = data_ini;

        if (dijit.byId("cd_nome_contrato_pesq").value == 0) {
            cd_layout = null;
        } else {
            cd_layout = dijit.byId("cd_nome_contrato_pesq").value;
        }

        dt_final = data_end;
        dt_termino = data_termino;

        var teste_turma = dijit.byId("no_turma_encerramento").store.data;
        var dc_turma = "";
        teste_turma.forEach(function (value, index) {
            if (index == 0) {
                dc_turma = value.id;
            } else {
                dc_turma = dc_turma + "|" + value.id;
            }
        });
        dc_turma = dc_turma + "|";       

        try {
            dojo.xhr.post({
                url: Endereco() + "/api/turma/postGerarRematricula",
                handleAs: "json",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: JSON.stringify({
                    dc_turma: dc_turma,
                    dt_inicial: dt_inicial,
                    dt_final: dt_final,
                    id_turma_nova: id_turma_nova,
                    cd_layout: cd_layout,
                    dt_termino: dt_termino
                })
            }).then(function (data) {
                data = jQuery.parseJSON(data);
                if (hasValue(data.erro) || !hasValue(data.retorno))
                    

                // monta nova grid
                var itemAlterado = data.retorno;
                var gridName = 'gridTurma';
                var grid = dijit.byId(gridName);
                caixaDialogo(DIALOGO_AVISO, data, null);
                dijit.byId("dialogEncerramento3").hide();
                dijit.byId("dialogEncerramento2").hide();
                dijit.byId("dialogEncerramentoNovaturma").hide();
                if (!hasValue(grid.itensSelecionados)) {
                    grid.itensSelecionados = [];
                }
                for (var i = 0; i < teste_turma.length; i++)
                    removeObjSort(grid.itensSelecionados, "cd_turma", teste_turma[i].cd_turma);

                buscarItensSelecionados(gridName, 'turmaSelecionada', 'cd_turma', 'selecionaTodos', ['pesquisarTurma', 'relatorioTurma'], 'todosItens');
                grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, teste_turma, "cd_turma");
                // fim monta nova grid
            }, function (error) {
                haErro = jQuery.parseJSON(error);
                if (hasValue(haErro.erro) || !hasValue(haErro.retorno))
                // monta nova grid
                var itemAlterado = data.retorno;
                var gridName = 'gridTurma';
                var grid = dijit.byId(gridName);
                caixaDialogo(DIALOGO_AVISO, haErro, null);
                dijit.byId("dialogEncerramento3").hide();
                dijit.byId("dialogEncerramento2").hide();
                dijit.byId("dialogEncerramentoNovaturma").hide();
                if (!hasValue(grid.itensSelecionados)) {
                    grid.itensSelecionados = [];
                }
                for (var i = 0; i < teste_turma.length; i++)
                    removeObjSort(grid.itensSelecionados, "cd_turma", teste_turma[i].cd_turma);                
                buscarItensSelecionados(gridName, 'turmaSelecionada', 'cd_turma', 'selecionaTodos', ['pesquisarTurma', 'relatorioTurma'], 'todosItens');
                grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, teste_turma, "cd_turma");
                // fim monta nova grid
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    }
}

function eventoVirada(itensSelecionados, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else
            if (itensSelecionados[0].dta_termino_turma == null)
                caixaDialogo(DIALOGO_ERRO, msgViradaTurmaAnd, null);
            else {
                var gridTurma = dijit.byId('gridTurma');
                apresentaMensagem('apresentadorMensagem', '');
                gridTurma.itemSelecionado = gridTurma.itensSelecionados[0];
                setarTabCad();
                destroyCreateGridHorario();
                destroyCreateProgramacao();
                dijit.byId("pesCadRegime").set("disabled", true);
                virada = true;
                keepValues(null, gridTurma, true, xhr, ready, Memory, filteringSelect, DropDownMenu, MenuItem, DropDownButton);
                IncluirAlterar(0, 'divAlterarTurma', 'divIncluirTurma', 'divExcluirTurma', 'apresentadorMensagemATurma', 'divCancelarTurma', 'divClearTurma');
                popularGridEscolaTurma();
                dijit.byId("cadTurma").show();
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function mostrarMensagemDelecaoHorariosProgramacao() {
    
    if (hasValue(dijit.byId("gridProgramacao")) || hasValue(dijit.byId("gridHorarios"))) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgInfoHorariosDeletados);
        if (hasValue(dijit.byId("gridProgramacao")))
            mensagensWeb[1] = new MensagensWeb(MENSAGEM_AVISO, msgInfoProgramacoesDeletados);
        apresentaMensagem("apresentadorMensagemTurma", mensagensWeb);
    }
    // em meus testes aparenta erro aqui. Antes e depois. Ao mesmo tempo que abre a tela com todos os dados lá...(??)
}

function mostrarPeriodo(value) {
    if (value == true) {
        dojo.byId('tgPeriodo').style.display = 'block';
        dojo.byId('dialogEncerramento').style.height = '600px';
        // torna o campo requerido quando 'Rematricular' for ativo
        dijit.byId("dtInicialR").required = true;
    }
    else {
        dojo.byId('tgPeriodo').style.display = 'none';
        dojo.byId('dialogEncerramento').style.height = '500px';
        // REMOVE o campo requerido quando 'Rematricular' for INAtivo
        dijit.byId("dtInicialR").required = false;
        var rematriculaAtivo = false;
    }
}


function eventoCancelaEncerramento(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCancelarEncSemSelecionar);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }
        else if (itensSelecionados.length > 1) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCancelarEncMaisTurmas);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        } else if (itensSelecionados[0].dta_termino_turma == null) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroCancelarTurmaAnd);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }
        if (itensSelecionados[0].cd_pessoa_escola != dojo.byId('_ES0').value) {
            caixaDialogo(DIALOGO_AVISO, 'Somente pode cancelar encerramento de Turmas de outra escola na escola de origem.', null);
            return false;
        }
        showCarregando();
        var turma = {
            cd_turma: itensSelecionados[0].cd_turma
        };
        var cd_usuario = "0";
        var fuso = "0";
        dojo.xhr.post({
            url: Endereco() + "/api/turma/postCancelarEncerramento",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({
                cd_turma: itensSelecionados[0].cd_turma,
                cd_usuario: cd_usuario,
                fuso: fuso,
                dt_termino: itensSelecionados[0].dt_termino_turma
            })
        }).then(function (data) {
            data = jQuery.parseJSON(data);
            if (data == "0") {
                hideCarregando();
                var itemAlterado = itensSelecionados;
                var gridName = 'gridTurma';
                var grid = dijit.byId(gridName);
                caixaDialogo(DIALOGO_AVISO, msgCancelaEncerramentoTurma, null);  // "Cancelamento de encerramento de turma realizado com sucesso."
                if (!hasValue(grid.itensSelecionados)) {
                    grid.itensSelecionados = [];
                }
                for (var i = 0; i < turma.length; i++)
                    removeObjSort(grid.itensSelecionados, "cd_turma", turma[i].cd_turma);
                for (var i = 0; i < itemAlterado.length; i++)
                    insertObjSort(grid.itensSelecionados, "cd_turma", itemAlterado[i]);
                turmaSelecionada = 3;
                var pesSituacao = dijit.byId("pesSituacao");
                pesSituacao.set("value", 3);
                buscarItensSelecionados(gridName, 'turmaSelecionada', 'cd_turma', 'selecionaTodos' , ['pesquisarTurma', 'relatorioTurma'], 'todosItens');
                grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                setGridPagination(grid, itemAlterado, "cd_turma", "turmaSelecionada");
            } else if (hasValue(data.erro) || !hasValue(data.retorno)) {
                hideCarregando();
                caixaDialogo(DIALOGO_AVISO, data, null);
            }
        }, function (error) {
            hideCarregando();
            caixaDialogo(DIALOGO_AVISO, data, null);
        }
        )}
    catch (e) {
        hideCarregando();
        postGerarLog(e);
    }
}

function montarFiltroSituacaoALunoTurma(item) {
    var id_menu = "";
    var gridAlunos = dijit.byId("gridAlunos");
    if (hasValue(item) && hasValue(item.target))
        id_menu = item.target.id.substring(0, item.target.id.length - 5);
    if (hasValue(gridAlunos) && hasValue(gridAlunos.listaCompletaAlunos)) {
        var listaAlunos = cloneArray(gridAlunos.listaCompletaAlunos);
        if (id_menu != 100) {
            id_menu -= 10;
            quickSortObj(listaAlunos, 'cd_situacao_aluno_turma');
            listaAlunos = jQuery.grep(listaAlunos, function (value) {
                return value.cd_situacao_aluno_turma == id_menu;
            });
        }
        quickSortObj(listaAlunos, 'no_aluno');
        gridAlunos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: listaAlunos, idProperty: "cd_aluno" }) }));
    }
}

function montarFiltroSituacaoALunoTurmaPPT(item) {
    var id_menu = "";
    var gridAlunosPPT = dijit.byId("gridAlunosPPT");
    if (hasValue(item) && hasValue(item.target))
        id_menu = item.target.id.substring(9, item.target.id.length - 5);
    if (hasValue(gridAlunosPPT) && hasValue(gridAlunosPPT.listaCompletaAlunos)) {
        var listaAlunos = cloneArray(gridAlunosPPT.listaCompletaAlunos);
        if (id_menu != 100) {
            id_menu -= 10;
            quickSortObj(listaAlunos, 'cd_situacao_aluno_turma');
            listaAlunos = jQuery.grep(listaAlunos, function (value) {
                if (hasValue(value.alunoTurma))
                    return value.alunoTurma.cd_situacao_aluno_turma == id_menu;
            });
        }
        gridAlunosPPT.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: listaAlunos, idProperty: "cd_aluno" }) }));
    }
}

function verificarESetarFiltroSituacaoaAguardando() {
    var menu = dijit.byId('menuFiltroAluno');
    if (hasValue(menu)) {
        var filhosMenu = menu.getDescendants();
        if (hasValue(filhosMenu))
            filhosMenu = jQuery.grep(filhosMenu, function (value) {
                return value.id == 19;
            });
        if (!hasValue(filhosMenu))
            menu.addChild(new dijit.MenuItem({
                id: "19",
                label: "Aguardando Mat.",
                onClick: function (e) { montarFiltroSituacaoALunoTurma(e); }
            }, 1));
    }

}

function eventoClickPesquisarTurmasFilhas(comp) {
    var acoesTurmasFilhas = dijit.byId("acoesTurmasFilhas");
    var menu = dijit.byId('menuAcoesRelacionadas');
    var gridTurma = dijit.byId("gridTurma");
    var pesSituacao = dijit.byId("pesSituacao");
    if (dijit.byId("tipoTurma").displayedValue == "Personalizada" && comp.checked) {
        loadSituacaoTurmaPPT(dojo.store.Memory);
        loadSituacaoTurma(dojo.store.Memory);
        if (hasValue(acoesTurmasFilhas))
            menu.removeChild(acoesTurmasFilhas);
        pesSituacao.set("value", 1);
    } else if (dijit.byId("tipoTurma").displayedValue == "Personalizada" && !comp.checked) {
        loadSituacaoTurma(dojo.store.Memory);
        loadSituacaoTurmaPPT(dojo.store.Memory);
        pesSituacao.set("value", 1);
        pesSituacao.set("disabled", false);
        if (!hasValue(acoesTurmasFilhas)) {
            var acaoMostrarTurmasFilhas = new MenuItem({
                label: "Mostrar Turmas Filhas",
                id: "acoesTurmasFilhas",
                onClick: function () {
                    apresentaMensagem("apresentadorMensagem", mensagensWeb);
                    if (!hasValue(gridTurma.itensSelecionados) || gridTurma.itensSelecionados.length <= 0)
                        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                    else if (gridTurma.itensSelecionados.length > 1)
                        caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
                    else {
                        var turma = gridTurma.itensSelecionados[0];
                        if ((!hasValue(turma.cd_turma_ppt) && turma.id_turma_ppt == true) || (turma.cd_turma_ppt == 0 && turma.id_turma_ppt == true)) {
                            pesquisarTurma(false, turma.cd_turma, false);
                            if (hasValue(acoesTurmasFilhas))
                                menu.removeChild(acoesTurmasFilhas);
                            dijit.byId("pesTurmasFilhas").set("checked", true);
                            eventoClickPesquisarTurmasFilhas(dijit.byId("pesTurmasFilhas"));
                        }
                        else {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNecessarioTurmaPPT);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                        }
                    }
                }
            });
            menu.addChild(acaoMostrarTurmasFilhas, 7);
        } else
            menu.addChild(acoesTurmasFilhas, 7);
    }
}


function findAllEmpresasUsuarioComboFiltroEscolaTelaTurma() {
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
                        loadSelect(dataRetorno, "escolaFiltroTelaTurma", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
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
function limparPesquisaCursoFKT() {
    try {
        if (hasValue(dijit.byId("gridPesquisaAluno"))) {
            dojo.byId("nomeAlunoFk").value = "";
            dojo.byId("emailPesqFK").value = "";
            dijit.byId("inicioAlunoFK").reset();
            dijit.byId("statusAlunoFK").set("value", 0);
            dijit.byId("nm_sexo_fk").set("value", 0);
            dojo.byId('cpf_fk').value = "";
            if (hasValue(dijit.byId("gridPesquisaAluno").itensSelecionados))
                dijit.byId("gridPesquisaAluno").itensSelecionados = [];
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
