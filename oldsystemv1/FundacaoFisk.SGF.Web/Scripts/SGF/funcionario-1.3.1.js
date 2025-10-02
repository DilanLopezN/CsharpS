var AUTOCOMPLETE_FECHADO = 0, AUTOCOMPLETE_ABERTO = 1, TODOS = 0;
var RELACENTREPESSOAS = 5;
var DOMINGO = 1; var SEGUNDA = 2; var TERCA = 3; var QUARTA = 4; var QUINTA = 5; var SEXTA = 6; var SABADO = 7;
var abilHabilidadeProf = null, PESSOAFISICA = 1;
var DIA_FUCIONARIO = 2;
var FUNCIONARIO = 1, PROFESSOR = 2;
var HAS_ATIVO = 0;
var CADASTRO = 1, EDICAO = 2;

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

            chart.addSeries("Disponível", [1], { fill: "#6ec2fd" }); //BlueRow
            chart.addSeries("Ocupado pela turma", [1], { fill: "#fc0000" }); //RedRow
            chart.render();
            dijit.byId("legend").refresh();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function mascarar() {
    try {
        dojo.ready(function () {
            maskHour('#timeIni');
            maskHour('#timeFim');
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaDiasSemana(registry, ItemFileReadStore) {
    try {
        var w = registry.byId("cbDias");
        var retorno = [];
        retorno.push({ value_dia: 0 + "", no_dia: "Domingo" });
        retorno.push({ value_dia: 1 + "", no_dia: "Segunda" });
        retorno.push({ value_dia: 2 + "", no_dia: "Terça" });
        retorno.push({ value_dia: 3 + "", no_dia: "Quarta" });
        retorno.push({ value_dia: 4 + "", no_dia: "Quinta" });
        retorno.push({ value_dia: 5 + "", no_dia: "Sexta" });
        retorno.push({ value_dia: 6 + "", no_dia: "Sabado" });
        var store = new ItemFileReadStore({
            data: {
                identifier: "value_dia",
                label: "no_dia",
                items: retorno
            }
        });
        w.setStore(store, []);
        console.log(w.get("value_dia"));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function toglleTag(value, id) {
    try {
        if (value == true)
            dojo.byId(id).style.display = 'block';
        else
            dojo.byId(id).style.display = 'none';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function toglleTagComissao(value) {
    require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            try {
                if (value) {
                    dojo.byId('tgComissao').style.display = 'block'
                    dijit.byId('tgComissao').set('open', true);
                }
                else {
                    dojo.byId('tgComissao').style.display = 'none'
                    
                    var grid = dijit.byId("gridComissao");
                    var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: [], idProperty: "id" }) });
                    grid.setStore(dataStore);
                    grid.update();
                }
            } catch (e) {
                postGerarLog(e);
            }
        });
}

function limparValores(id){

    switch (id) {
        case "valorMatNovaComissao":
            dijit.byId("percentualMatNovaComissao").set("value", "");
            break;
        case "percentualMatNovaComissao":
            dijit.byId("valorMatNovaComissao").set("value", "");
            break;

        case "valorRmtNovaComissao":
            dijit.byId("percentualRmtNovaComissao").set("value", "");
            break;
        case "percentualRmtNovaComissao":
            dijit.byId("valorRmtNovaComissao").set("value", "");
            break;
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

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridFuncionario';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_funcionario", grid._by_idx[rowIndex].item.cd_funcionario);

            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_funcionario', 'selecionaFuncionario', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_funcionario', 'selecionaFuncionario', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxComiss(value, rowIndex, obj) {
    var gridName = 'gridComissao';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosComiss');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_funcionario_comissao", grid._by_idx[rowIndex].item.id);

        value = value || indice != null; // Item está selecionado.
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'id', 'selecionadoComiss', -1, 'selecionaTodosComiss', 'selecionaTodosComiss', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'id', 'selecionadoComiss', " + rowIndex + ", '" + id + "', 'selecionaTodosComiss', '" + gridName + "')", 2);

    return icon;
}

function formataHorarios(listHorarios) {
    try {
        $.each(listHorarios, function (idx, val) {
            //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
            val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm", locale: 'pt-br' }),
            val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm", locale: 'pt-br' })
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaGridFuncionario(permissoes) {
    require([
       "dojo/_base/xhr",
       "dojo/dom",
       "dijit/registry",
       "dojo/data/ItemFileReadStore",
       "dojox/grid/EnhancedGrid",
       "dojox/grid/enhanced/plugins/Pagination",
       "dojo/store/JsonRest",
       "dojo/data/ObjectStore",
       "dojo/store/Cache",
       "dojo/store/Memory",
       "dojo/dom-attr",
       "dijit/form/Button",
       "dojo/ready",
       "dijit/form/DropDownButton",
       "dijit/DropDownMenu",
       "dijit/MenuItem",
       "dojo/on",
       "dijit/form/FilteringSelect",
       "dijit/Tooltip",
       "dojo/window"
    ], function (xhr, dom, registry, ItemFileReadStore, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, domAttr, Button, ready, DropDownButton, DropDownMenu, MenuItem, on,
        FilteringSelect, Tooltip, windowUtils) {
        ready(function () {
            try {
                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                dijit.byId('nomFantasia').set("required", true);
                populaDiasSemana(registry, ItemFileReadStore);
                dojo.byId("descApresMsg").value = 'apresentadorMensagemFunc';
                dijit.byId("panelEndereco").set("open", false);
                dijit.byId("tabContainer").resize()
                toggleDisabled(dijit.byId("tabHorarios"), true);
                $("#_cpf").mask("999.999.999-99");
                dojo.byId("lblAtivo").innerHTML = "Ativo:";
                mascarar();

                loadProdutoAssinatura();
                dijit.byId('cbProdutoAssinatura').set('disabled', true);

                setMenssageMultiSelect(DIA, 'cbDias');
                dijit.byId("cbDias").on("change", function (e) {
                    setMenssageMultiSelect(DIA, 'cbDias');
                });
                //Limitando scroll multi select.
                dijit.byId("cbMultiHabEstagio").dropDownMenu.domNode.style.maxHeight = "300px";
                loadDataFiltrosTipoFunc(Memory);
                myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/professor/GetFuncionarioSearch?nome=&apelido=&status=1&cpf=&inicio=false&tipo=0&sexo=0&cdAtividade=0&coordenador=0&coolaborador_cyber=0",
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                   }
               ), Memory({}));

                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridFuncionario = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }), //dojo.data.ObjectStore({ objectStore: null }),
                    structure:
                    [
                        { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionaFuncionario", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                        //{ name: "Código", field: "cd_funcionario", width: "50px", styles: "min-width:50px; text-align: left;" },
                        { name: "Nome", field: "no_pessoa", width: "25%", styles: "min-width:80px;" },
                        { name: "CPF", field: "nm_cpf_dependente", width: "15%", styles: "min-width:80px;" },
                        { name: "Nome Reduzido", field: "dc_reduzido_pessoa", width: "18%" },
                        { name: "Data Cadastro", field: "dta_cadastro", width: "8%", styles: "min-width:100px;" },
                        { name: "Cargo", field: "des_cargo", width: "18%", styles: "min-width:80px;" },
                        { name: "Professor/ Instrutor", field: "professor_ativo", width: "8%", styles: "text-align: center; min-width:70px; max-width: 80px;" },
                        { name: "Ativo", field: "id_ativo", width: "8%", styles: "text-align: center; min-width:50px; max-width: 50px;" }
                    ],
                    canSort: true,
                    noDataMessage: msgNotRegEnc,
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
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridFuncionario");

                gridFuncionario.pagination.plugin._paginator.plugin.connect(gridFuncionario.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try {
                        verificaMostrarTodos(evt, gridFuncionario, 'cd_funcionario', 'selecionaTodos');
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridFuncionario, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_funcionario', 'selecionaFuncionario', -1, 'selecionaTodos', 'selecionaTodos', 'gridFuncionario')", gridFuncionario.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });

                gridFuncionario.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 3; };
                gridFuncionario.startup();
                gridFuncionario.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        limparFuncionario();
                        destroyCreateGridHorario();
                        keepValues(item, gridFuncionario, false);
                        IncluirAlterar(0, 'divAlterarFunc', 'divIncluirFunc', 'divExcluirFunc', 'apresentadorMensagemFunc', 'divCancelarFunc', 'divClearFunc');
                        dijit.byId("formFunc").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                var gridHabilProf = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                      [
                        { name: "Produto", field: "no_produto", width: "50%", styles: "text-align: left; min-width:50px;" },
                        { name: "Estágio Máximo", field: "no_estagio", width: "50%", styles: "text-align: left; min-width:50px;" }
                      ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridHabilProf");
                gridHabilProf.startup();
                gridHabilProf.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        IncluirAlterar(0, 'divAlterarHabil', 'divIncluirHabil', 'divExcluirHabil', 'apresentadorMsgHabil', 'divCancelarHabil', 'divClearHabil');
                        keepValuesHabilitacao();
                        dojo.byId("idCamboHabEstagio").style.display = "";
                        dojo.byId("idMultiHabEstagio").style.display = "none";
                        dijit.byId("EstagioHab").set("required", true);
                        dijit.byId("cbMultiHabEstagio").set("required", false);

                        //Limitando scroll multi select.
                        dijit.byId("cbMultiHabEstagio").dropDownMenu.domNode.style.maxHeight = "300px";

                        dijit.byId("dialogHabilitacao").show();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                var gridTurmaAtual = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        { name: "Curso", field: "no_curso", width: "30%", styles: "text-align: left; min-width:50px;" },
                        { name: "Turma", field: "no_turma", width: "50%", styles: "text-align: left; min-width:50px;" },
                        { name: "Sala", field: "no_sala", width: "20%", styles: "text-align: left; min-width:50px;" }
                    ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["6", "12", "18", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "6",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 5,
                            /*position of the pagination bar*/
                            position: "button",
                            plugins: { nestedSorting: false }
                        }
                    }
                }, "gridTurmasAtuais");
                gridTurmaAtual.startup();
                loadPesqSexo(Memory, dijit.byId("nm_sexo"));
                new Button({ label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () { pesquisaFuncionario(true); } }, "pesquisarFunc");
                decreaseBtn(document.getElementById("pesquisarFunc"), '32px');

                if (dijit.byId('tgComissao').open == false)
                    dijit.byId('tgComissao').set('open', true);

                var gridComissao = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: [] }) }),
                    structure:
                    [
                        [
                            { name: "<input id='selecionaTodosComiss' style='display:none'/>", field: "selecionadoComiss", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxComiss },
                            { name: "Produto", field: "no_produto", width: "35%", styles: "text-align: left; min-width:50px;" },
                            { name: "R$", field: "vl_comissao_matricula", width: "15%", styles: "text-align: center; min-width:50px;" },
                            { name: "%", field: "pc_comissao_matricula", width: "15%", styles: "text-align: center; min-width:50px;" },
                            { name: "R$", field: "vl_comissao_rematricula", width: "15%", styles: "text-align: center; min-width:50px;" },
                            { name: "%", field: "pc_comissao_rematricula", width: "15%", styles: "text-align: center; min-width:50px;" }
                        ]
                    ],
                    canSort: true,
                    noDataMessage: "Nenhum registro encontrado."
                }, "gridComissao");
                gridComissao.startup();
                var gridComissaoHeader = new EnhancedGrid({
                    store: new ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                    [
                        [
                            { name: "" ,width: "5%",styles: "text-align:center;"},
                            { name: "" ,width: "35%",styles: "text-align:center;"},
                            { name: "Matriculas", width: "30%",styles: "text-align:center;"},
                            { name: "Rematriculas", width: "30%",styles: "text-align:center;"}
                        ]
                    ]
                }, "gridComissaoHeader");
                gridComissaoHeader.startup();
                dijit.byId('tgComissao').set('open', false);

                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            var cdAtiv = hasValue(dojo.byId("pesCdAtividade").value) ? dojo.byId("pesCdAtividade").value : 0;
                            var cdTipoPesq = hasValue(dijit.byId("pesqFuncTipo").value) ? dijit.byId("pesqFuncTipo").value : 0;
                            var colaboradorCyberPesq = hasValue(dijit.byId("colaboradorCyberPesq").value) ? dijit.byId("colaboradorCyberPesq").value : 0;
                            var coordenadorPesq = hasValue(dijit.byId("coordenadorPesq").value) ? dijit.byId("coordenadorPesq").value : 0;
                            dojo.xhr.get({
                                url: Endereco() + "/api/professor/GetUrlRelatorioFuncionario?" + getStrGridParameters('gridFuncionario') + "nome=" + dojo.byId("_nome").value + "&apelido=" +
                                     dojo.byId("_reduzido").value + "&status=" + parseInt(0) + "&cpf=" + dojo.byId("_cpf").value + "&inicio=" + document.getElementById("inicioFunc").checked +
                                     "&tipo=" + cdTipoPesq + "&sexo=" + dijit.byId("nm_sexo").value + "&cdAtividade=" + cdAtiv + "&coordenador=" + coordenadorPesq + "&coolaborador_cyber=" + colaboradorCyberPesq,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                try {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data.retorno, '990px', '750px', 'popRelatorio');
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemFunc', error);
                            });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioFuncionario");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                        try {
                            IncluirAlterar(1, 'divAlterarFunc', 'divIncluirFunc', 'divExcluirFunc', 'apresentadorMensagemFunc', 'divCancelarFunc', 'divClearFunc');
                            limparFuncionario();
                            limparCamposCargo();
                            destroyCreateGridHorario();
                            setarTabCad();
                            var papelRelac = new Array();
                            papelRelac[0] = RELACENTREPESSOAS;
                            showPessoaFK(PESSOAFISICA, 0, papelRelac);
                            dijit.byId('naturezaPessoa').set("disabled", true);
                            dijit.byId("formFunc").show();
                            dijit.byId("tabContainer").resize()
                            dijit.byId('excluirFoto').setAttribute('disabled', 1);
                            dijit.byId('cbProdutoAssinatura').set('disabled', true);
                            dijit.byId("ckCoordenador").set('checked', false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoFuncionario");

                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        try {
                            if (hasValue(dijit.byId("naturezaPessoa")) && hasValue(dijit.byId("naturezaPessoa").value)) {
                                cadastrarOuAlterarFuncionario(windowUtils, CADASTRO);
                            }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirFunc");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            limparFuncionario();
                            limparCamposCargo();
                            destroyCreateGridHorario();
                            setarTabCad();
                            keepValues(null, dijit.byId("gridFuncionario"), null);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarFunc");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("formFunc").hide(); } }, "fecharFunc");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        cadastrarOuAlterarFuncionario(windowUtils, EDICAO);
                    }
                }, "alterarFunc");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarFuncionario() });
                    }
                }, "deleteFunc");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        limparFuncionario();
                    }
                }, "limparFunc");
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                        try {
                            limparFormHabilProf();
                            loadProduto(null, null);
                            IncluirAlterar(1, 'divAlterarHabil', 'divIncluirHabil', 'divExcluirHabil', 'apresentadorMsgHabil', 'divCancelarHabil', 'divClearHabil');
                            dojo.byId("idCamboHabEstagio").style.display = "none";
                            dojo.byId("idMultiHabEstagio").style.display = "";
                            dijit.byId("EstagioHab").set("required", false);
                            dijit.byId("cbMultiHabEstagio").set("required", true);
                            dijit.byId("dialogHabilitacao").show();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnNovaHabilitacao");
                // Criar botões Tag Habilitação
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        limparFormHabilProf();
                        keepValuesHabilitacao();
                    }
                }, "cancelarHabil");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("dialogHabilitacao").hide(); } }, "fecharHabil");
                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                        alterarHabilitacao();
                    }
                }, "alterarHabil");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { excluirHabilitacao() });
                    }
                }, "deleteHabil");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        limparFormHabilProf();
                    }
                }, "limparHabil");

                //Botões para cargo
                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function (e) {
                        try {
                            dojo.byId('codNaturezaAtividade').value = CARGO;
                            dojo.byId('id_natureza_atividade').value = 'Física';
                            dojo.byId("tdCnae").style.display = 'none'
                            dojo.byId("tdCdCnaeAtividade").style.display = 'none'
                            dojo.byId("isPesquisa").value = false;

                            if (!hasValue(dijit.byId("gridAuxiliaresPessoaFK")))
                                montargridPesquisaAuxiliaresPessoa(
                                    function () {
                                        abrirPesquisaAuxiliaresPessoaCargoFK(CARGO);
                                    });
                            else
                                abrirPesquisaAuxiliaresPessoaCargoFK(CARGO);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "proFkCargo");

                new Button({
                    label: "Limpar", type: "reset", disabled: true, onClick: function () {
                        try {
                            dojo.byId("cdCargo").value = 0;
                            dijit.byId('desCargo').set('value', '');
                            dijit.byId("limparFkCargo").set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparFkCargo");

                if (hasValue(document.getElementById("limparFkCargo"))) {
                    document.getElementById("limparFkCargo").parentNode.style.minWidth = '40px';
                    document.getElementById("limparFkCargo").parentNode.style.width = '40px';
                }

                new Button({
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function (e) {
                        try {
                            dojo.byId('codNaturezaAtividade').value = CARGO;
                            dojo.byId('id_natureza_atividade').value = 'Física';
                            dojo.byId("tdCnae").style.display = 'none'
                            dojo.byId("tdCdCnaeAtividade").style.display = 'none'
                            dojo.byId("isPesquisa").value = true;
                            if (!hasValue(dijit.byId("gridAuxiliaresPessoaFK")))
                                montargridPesquisaAuxiliaresPessoa(
                                    function () {
                                        abrirPesquisaAuxiliaresPessoaCargoFK(CARGO);
                                    });
                            else
                                abrirPesquisaAuxiliaresPessoaCargoFK(CARGO);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "pesFkCargo");

                new Button({
                    label: "Limpar", type: "reset", disabled: true, onClick: function () {
                        limparCamposCargo();
                    }
                }, "limparFkCargoPes");

                if (hasValue(document.getElementById("limparFkCargoPes"))) {
                    document.getElementById("limparFkCargoPes").parentNode.style.minWidth = '40px';
                    document.getElementById("limparFkCargoPes").parentNode.style.width = '40px';
                }

                // Criar botões tag 'Comissões'.
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function ()
                    {
                        try {
                            document.getElementById("divIncluirComissao").style.display = "";
                            document.getElementById("divAlterarComissao").style.display = "none";

                            loadProduto(null, true);
                            limparFormComissao();
                            dijit.byId("dialogComiss").show();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btnNovaComiss");

                new Button({ label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () { incluirNovaComissao(); } }, "btnIncluirComissao");
                new Button({
                    label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo', onClick: function () {
                        editarComissao();
                    }
                }, "alterarComissao");
                //#endregion

                var buttonFkArray = ['proFkCargo', 'pesFkCargo'];
                diminuirBotoes(buttonFkArray);

                // Adiciona link de ações Comissão:
                var menu = new DropDownMenu({ style: "height: 25px" });

                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        loadProduto(null, true);
                        abrirEditorComissao(dijit.byId("gridComissao").itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'id', dijit.byId("gridComissao")); }
                });
                menu.addChild(acaoExcluir);                

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasComiss",
                    dropDown: menu,
                    id: "acoesRelacionadasComiss"
                });
                dom.byId("linkAcoesComiss").appendChild(button.domNode);

                //*** Cria os botões de persistência **\\
                new Button({
                    label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconInsert",
                    onClick: function () { incluirItemHorario(); }
                }, "incluirHorario");

                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
                    onClick: function () { excluiItemHorario(); }
                }, "excluirHorario");

                new Button({
                    label: "Incluir", iconClass: "dijitEditorIcon dijitEditorIconInsert",
                    onClick: function () { incluirHabilitacao(); }
                }, "incluirHabil");

                montarStatus("statusFunc");
                dojo.byId("_cpf").value = "";
                //dojo.byId("_cpf").readOnly = true;

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridFuncionario.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemoverFuncionario(gridFuncionario.itensSelecionados); }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dojo.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridFuncionario, 'todosItens', ['pesquisarFunc', 'relatorioFuncionario']);
                        pesquisaFuncionario(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridFuncionario', 'selecionaFuncionario', 'cd_funcionario', 'selecionaTodos', ['pesquisarFunc', 'relatorioFuncionario'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);

                registry.byId("ProdutoHab").on("change", function (e) {
                    if (e != null && e > 0)
                        loadEstagiosByEstagio(e);
                });
                var statusStore = new Memory({
                    data: [
                    { name: "Hora", id: 1 },
                    { name: "Aula", id: 2 },
                    { name: "Fixo", id: 3 }
                    ]
                });
                var TipoPagto = new FilteringSelect({
                    id: "formaPagto",
                    name: "formaPagto",
                    store: statusStore,
                    value: "3",
                    searchAttr: "name",
                    style: "max-width:182px;width: 100%;"
                }, "formaPagto");

                if (hasValue(dijit.byId("cpf")))
                    dijit.byId("cpf").on("blur", function (evt) {
                        try {
                            if (trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__")
                                if (validarCPF("#cpf", "apresentadorMensagemFunc")) {
                                    cleanUsarCpf();
                                    ExtistsPessoByCpf();
                                    apresentaMensagem('apresentadorMensagemFunc', '');
                                }
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                if (hasValue(dijit.byId('pesPessoaFK')))
                    dijit.byId('pesPessoaFK').set("disabled", true);
                dojo.byId('TagProfessor').style.display = 'none';
                blockedEndOpenHabil();
                showCarregando();
                maskHour("#timeIni");
                maskHour("#timeFim");
                //Verificando se o usuário tem permissão para visualizar, editar, incluir ou deixar o campo "Salário" em branco
                //dijit.byId("vl_salario").setAttribute("style", "visibility: hidden");
                //dojo.byId("lbSalario").setAttribute("style", "visibility: hidden");
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                if (Permissoes == null || Permissoes == "" || !possuiPermissao('salfunc', Permissoes)) {
                    dijit.byId("vl_salario").setAttribute("style", "visibility: hidden");
                    dojo.byId("lbSalario").setAttribute("style", "visibility: hidden");
                }
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323023', '765px', '771px');
                        });
                }
                adicionarAtalhoPesquisa(['_nome', '_reduzido', 'statusFunc', 'nm_sexo', '_cpf', 'atividadeFunc'], 'pesquisarFunc', ready);
                dijit.byId("pesqFuncTipo").on("change", function (e) {
                    try {
                        if (!hasValue(e) || e < TODOS)
                            dijit.byId("pesqFuncTipo").set("value", TODOS);
                        if (e >= 0) {
                            switch (e) {
                                case FUNCIONARIO:
                                    dojo.byId("lblCkCoordenador").style.display = "none";
                                    dojo.byId("tdCkCoordenador").style.display = "none";
                                    dojo.byId("lblCkColaboradorCyber").style.display = "";
                                    dojo.byId("tdCkColaboradorCyber").style.display = "";
                                    dojo.byId("tdCampoVazio").style.display = "none";
                                    break;
                                case PROFESSOR:
                                    dojo.byId("lblCkCoordenador").style.display = "";
                                    dojo.byId("tdCkCoordenador").style.display = "";
                                    dojo.byId("lblCkColaboradorCyber").style.display = "none";
                                    dojo.byId("tdCkColaboradorCyber").style.display = "none";
                                    dojo.byId("tdCampoVazio").style.display = "none";
                                    break;
                                default:
                                    dojo.byId("lblCkCoordenador").style.display = "none";
                                    dojo.byId("tdCkCoordenador").style.display = "none";
                                    dojo.byId("lblCkColaboradorCyber").style.display = "none";
                                    dojo.byId("tdCkColaboradorCyber").style.display = "none";
                                    dojo.byId("tdCampoVazio").style.display = "";
                                    break;
                            }
                            dijit.byId("colaboradorCyberPesq").set("value", TODOS);
                            dijit.byId("coordenadorPesq").set("value", TODOS);
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("TurmaAtual").on("show", function (e) {
                    dijit.byId("paiTurmasAtuais").resize();
                    dijit.byId("gridTurmasAtuais").update();
                });

                dijit.byId("idProfessor").on("change", function (e) {
                    try {
                        toglleTag(dijit.byId("idProfessor").get("checked"), 'TagProfessor');
                        toggleDisabled(dijit.byId("tabHorarios"), !dijit.byId("idProfessor").get("checked"));
                        if (dijit.byId("idProfessor").get("checked")) {

                            dijit.byId("tagHabilProf").set("open", true);
                            dijit.byId("TurmaAtual").set("open", true);
                            dijit.byId("gridHabilProf").update();
                            dijit.byId("gridTurmasAtuais").update();
                            dijit.byId("tagHabilProf").set("open", false);
                            dijit.byId("TurmaAtual").set("open", false);
                        }
                        if (e && dijit.byId("idColaboradorCyber").checked)
                            dijit.byId("idColaboradorCyber").set("checked", false);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                dijit.byId("idColaboradorCyber").on("change", function (e) {
                    if (e && dijit.byId("idProfessor").checked)
                        dijit.byId("idProfessor").set("checked", false);
                });
                dijit.byId("colaboradorCyberPesq").on("change", function (e) {
                    if (!hasValue(e) || e < TODOS)
                        dijit.byId("colaboradorCyberPesq").set("value", TODOS);
                });
                dijit.byId("coordenadorPesq").on("change", function (e) {
                    if (!hasValue(e) || e < TODOS)
                        dijit.byId("coordenadorPesq").set("value", TODOS);
                });

                if (hasValue(dijit.byId("nomPessoa")))
                    dijit.byId("nomPessoa").on("blur", function (evt) {
		                try {
                            if (trim(dojo.byId("nomPessoa").value) != "" &&
	                            trim(dojo.byId("nomPessoa").value).indexOf("*") > -1) {
	                            var mensagensWeb = new Array();
	                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO,
		                            "Não é permitido o caracter (*) no campo nome.");
	                            apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
	                            dijit.byId("nomPessoa").reset();
	                            return;
                            } else {
	                            apresentaMensagem('apresentadorMensagemFunc', null);
                            }
		                }
		                catch (e) {
			                postGerarLog(e);
		                }
	                });

                decreaseBtn(document.getElementById("uploaderAssinatura"), '18px');
                //new Tooltip({
                //    connectId: ["uploaderAssinatura"],
                //    label: "Upload",
                //    position: ['above']
                //});
                dijit.byId('uploaderAssinatura').on("change", function (evt) {
                    try {
                        var mensagensWeb = new Array();
                        var files = dijit.byId("uploaderAssinatura")._files;
                        apresentaMensagem("apresentadorMensagemFunc", null);
                        if (hasValue(files) && files.length > 0) {
                            if (hasValue(files[0]) && files[0].size > 400000) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErrorArquivoExcedeuTamanho);
                                apresentaMensagem("apresentadorMensagemFunc", mensagensWeb);
                                return false;
                            }
                            if (!verificarExtensaoArquivo(files[0].name)) {
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroExtesaoErradaArquivo);
                                apresentaMensagem("apresentadorMensagemFunc", mensagensWeb);
                                return false;
                            }
                            dojo.byId("nomAssinaturaCertificado").value = files[0].name;
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                montarLegenda();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];
        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabHorarios') {
            if (hasValue(dojo.byId("cdFuncionario").value) && dojo.byId("cdFuncionario").value > 0) {
                loadHorarioProfessor(dojo.byId("cdFuncionario").value);
            } else {
                showCarregando();
                criacaoGradeHorario(null);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criacaoGradeHorario(dataHorario) {
    require(["dojo/ready", "dojox/calendar/Calendar", "dojo/store/Observable", "dojo/store/Memory", "dojo/_base/declare", "dojo/on", "dojo/_base/xhr"],
      function (ready, Calendar, Observable, Memory, declare, on, xhr) {
          ready(function () {
              try {
                  xhr.get({
                      preventCache: true,
                      url: Endereco() + "/api/empresa/getHorarioFuncEmpresa",
                      handleAs: "json",
                      headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                  }).then(function (getHorarioFuncEscola) {
                      try {
                          getHorarioFuncEscola = jQuery.parseJSON(getHorarioFuncEscola);
                          var horaI = 0, horaF = 24;
                          var minI = 00, minF = 00;
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

                          //dijit.byId("timeIni").set("value", "T0" + horaI + ":" + minI);
                          //dijit.byId("timeFim").set("value", "T" + horaF + ":" + minF);

                          horaF = minF > 1 ? horaF + 1 : horaF;
                          var ECalendar = declare("extended.Calendar", Calendar, {

                              isItemResizeEnabled: function (item, rendererKind) {
                                  return false || (hasValue(item) && item.cssClass == 'Calendar1');
                              },

                              isItemMoveEnabled: function (item, rendererKind) {
                                  return false || (hasValue(item) && item.cssClass == 'Calendar1');
                              }
                          });
                          var dataHoje = new Date(2012, 0, 1);
                          if (hasValue(dijit.byId("gridHorarios")))
                              destroyCreateGridHorario();
                          //dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' })
                          var calendar = new ECalendar({
                              date: dojo.date.locale.parse("01/07/2012", { locale: 'pt-br', formatLength: "short", selector: "date" }),
                              cssClassFunc: function (item) {
                                  return item.calendar;
                              },
                              store: new Observable(new Memory({ data: dataHorario })),
                              dateInterval: "week",
                              columnViewProps: { minHours: horaI, maxHours: horaF, timeSlotDuration: 5, hourSize: 50 },
                              style: "position:relative;max-width:650px;width:100%;height:265px"
                          }, "gridHorarios");
                          calendar.novosItensEditados = new Array();

                          // Configura o calendario para poder excluir o item ao selecionar:
                          //calendar.on("itemClick", function (e) {
                          //    dijit.byId('excluirHorario').setAttribute('disabled', 0);
                          //});
                          //calendar.on("change", function (e) {
                          //    dijit.byId('excluirHorario').setAttribute('disabled', 1);
                          //});

                          calendar.on("itemeditbegin", function (item) {
                              addItemHorariosEdit(item.item.id, calendar);
                          });
                          calendar.on("itemRollOver", function (e) {
                              try {
                                  var minutosEnd = e.item.endTime.getMinutes() > 9 ? e.item.endTime.getMinutes() : "0" + e.item.endTime.getMinutes();
                                  var horasEnd = e.item.endTime.getHours() > 9 ? e.item.endTime.getHours() : "0" + e.item.endTime.getHours();
                                  var horarioEnd = horasEnd + ":" + minutosEnd;
                                  var minutosStart = e.item.startTime.getMinutes() > 9 ? e.item.startTime.getMinutes() : "0" + e.item.startTime.getMinutes();
                                  var horasStart = e.item.startTime.getHours() > 9 ? e.item.startTime.getHours() : "0" + e.item.startTime.getHours();
                                  var horarioStart = horasStart + ":" + minutosStart;

                                  if (e.item.calendar == "Calendar1")
                                      dojo.attr(e.renderer.domNode.id, "title", "Disponível pelo Professor  " + horarioStart + " as " + horarioEnd);
                                  if (e.item.calendar == "Calendar2") {
                                      for (var i = 0; i < dataHorario.length; i++) {
                                          if (dataHorario[i].id == e.item.id)
                                              //new Tooltip({
                                              //    connectId: e.renderer.domNode.id,
                                              //    label: "Ocupado pela Turma  " + horarioStart + " as " + horarioEnd
                                              //});
                                              dojo.attr(e.renderer.domNode.id, "title", "Ocupado pela Turma: " + dataHorario[i].no_registro);//+ " " + horarioStart + " as " + horarioEnd);
                                      }
                                  }
                              }
                              catch (e) {
                                  postGerarLog(e);
                              }
                              //  dojo.byId('mostrarLabelHorario').value = true;
                          });

                          // Configura o calendário para poder incluir um item no click da grade:
                          var createItem = function (view, d, e) {

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
                              var colView = calendar.columnView;
                              var cal = calendar.dateModule;
                              var gradeHorarios = dijit.byId('gridHorarios');
                              var itens = dijit.byId('gridHorarios').items;
                              var id = gradeHorarios.items.length;
                              var item = null;

                              if (id > 0) {
                                  quickSortObj(gradeHorarios.items, 'id');
                                  id = gradeHorarios.items[id - 1].id + 1;
                              }
                              if (view == colView) {
                                  start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                                  //start = d;
                                  end = cal.add(start, "minute", colView.timeSlotDuration);
                                  //end = new Date(d.getTime() + 15*60000);
                              }
                              var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                              var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                              var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                              var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();
                              var inicioDia = start.getDate();
                              if (itens.length == 0) {
                                  item = {
                                      id: id,
                                      cd_horario: 0,
                                      dt_hora_ini: hIni + ":" + mIni + ":00",
                                      dt_hora_fim: hFim + ":" + mFim + ":00",
                                      summary: "",
                                      startTime: start,
                                      endTime: end,
                                      calendar: "Calendar1",
                                      id_dia_semana: start.getDate(),
                                      id_disponivel: true
                                  };

                              } else {

                                  for (var j = itens.length - 1; j >= 0; j--) {
                                      //if (itens[j].id != evento.item.id) {
                                      //calendar.store.remove(itens[j].id);

                                      // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                                      if (itens[j].cssClass == "Calendar1" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                                          // Verifica se um item inclui totalmente o outro item e remove o incluído:
                                          var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);

                                          if (included == 1) {
                                              resolveuIntersecao = true;
                                              return false;
                                          }
                                          else
                                              if (included == 2)
                                                  calendar.store.remove(itens[j].id);

                                                  // Caso contrário, junta um item com o outro:
                                              else
                                                  if (included != NaN) {
                                                      if (dates.compare(start, itens[j].startTime) == 1)
                                                          start = itens[j].startTime;
                                                      else
                                                          end = itens[j].endTime;
                                                      calendar.store.remove(itens[j].id);

                                                  }
                                      }
                                  }

                                  item = {
                                      id: id,
                                      cd_horario: 0,
                                      summary: "",
                                      t_hora_ini: hIni + ":" + mIni + ":00",
                                      dt_hora_fim: hFim + ":" + mFim + ":00",
                                      startTime: start,
                                      endTime: end,
                                      calendar: "Calendar1",
                                      id_dia_semana: start.getDate(),
                                      id_disponivel: true
                                  };

                              }
                              setTimeout("removeHorarioEditado(" + item.id + ")", 1500);
                              return item;
                          }

                          calendar.set("createOnGridClick", true);
                          calendar.set("createItemFunc", createItem);
                          //calendar.set("ItemEditEnd", ItemEditEnd);

                          dijit.byId("gridHorarios").on("ItemEditEnd", function (evt) {
                              setTimeout('mergeItemGridHorarios("' + evt.item.id + '", "' + evt.item.startTime + '", "' + evt.item.endTime + '", "' + evt.item.cd_horario + '")', 100);
                          });

                          var toolBar = document.getElementById('dijit_Toolbar_0');
                          $(".buttonContainer").css("display", "none");
                          $(".viewContainer").css("top", "5px");
                          if (hasValue(toolBar))
                              toolBar.style.display = 'none';
                          showCarregando();
                      }
                      catch (e) {
                          postGerarLog(e);
                          showCarregando();
                      }
                  },
                  function (error) {
                      apresentaMensagem('apresentadorMensagemFunc', error);
                      showCarregando();
                  });
              }
              catch (e) {
                  postGerarLog(e);
                  showCarregando();
              }
          });
      });
}

function removeHorarioEditado(id) {
    try {
        var gridHorarios = dijit.byId("gridHorarios");

        if (!hasValue(gridHorarios.novosItensEditados) || !binarySearch(gridHorarios.novosItensEditados, id))
            gridHorarios.store.remove(parseInt(id));
    }
    catch (e) {
        postGerarLog(e);
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

function mergeItemGridHorarios(id, startTime, endTime, cd_horario) {
    try {
        var calendar = dijit.byId('gridHorarios');
        var arraySemana = [];
        var itens = dijit.byId('gridHorarios').items;

        var diaRegistro = new Date(startTime);
        var endDia = new Date(endTime);
        var fimDia = endDia.getDate();

        //Verificando se não está tirando horário que está ocupado
        var inicioHoras = (new Date(startTime)).getHours();
        var inicioMinutos = (new Date(startTime)).getMinutes();
        var fimHoras = (new Date(endTime)).getHours();
        var fimMinutos = (new Date(endTime)).getMinutes();
        var diaRegistro = (new Date(startTime));
        var inicioDia = diaRegistro.getDate();
        var diaRegistroFim = (new Date(endTime)).getDate();
        var ok = true;

        var mensagensWeb = new Array();
        if (inicioDia != fimDia) {
            calendar.store.remove(parseInt(id));
            return false;
        }

        var dataHorarioFuncionario = dijit.byId("gridFuncionario").horarioFuncionario;
        var idOriginal = id;
        $.each(itens, function (idx, value) {
            if (value.id == parseInt(id)) {
                calendar.store.remove(value.id);
                return false;
            }
        });
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
        var cal = calendar.dateModule;
        var gradeHorarios = dijit.byId('gridHorarios');
        var resolveuIntersecao = false;

        start = new Date(startTime);
        end = new Date(endTime);
        d = new Date(endTime);

        /****Verificar se existe interseção com um ou mais horários disponíveis:****/
        for (var j = itens.length - 1; j >= 0; j--) {
            //if (itens[j].id != evento.item.id) {
            //calendar.store.remove(itens[j].id);

            // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
            if (itens[j].cssClass == "Calendar1" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                // Verifica se um item inclui totalmente o outro item e remove o incluído:
                var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);
                $.each(calendar.store.data, function (idx, value) {
                    if (value.id == itens[j].id)
                        cd_horario = value.cd_horario;
                });
                if (included == 1)
                    resolveuIntersecao = true;
                else
                    if (included == 2)
                        calendar.store.remove(itens[j].id);

                        // Caso contrário, junta um item com o outro:
                    else
                        if (included != NaN) {
                            if (dates.compare(start, itens[j].startTime) == 1)
                                start = itens[j].startTime;
                            else
                                d = itens[j].endTime;
                            calendar.store.remove(itens[j].id);

                        }
            }
        }
        var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
        var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
        var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
        var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

        var id = gradeHorarios.items.length;

        if (id > 0) {
            quickSortObj(gradeHorarios.items, 'id');
            id = gradeHorarios.items[id - 1].id + 1;
        }
        if (!resolveuIntersecao) {
            var item = {
                id: id,
                cd_horario: parseInt(cd_horario),
                summary: "",
                dt_hora_ini: hIni + ":" + mIni + ":00",
                dt_hora_fim: hFim + ":" + mFim + ":00",
                startTime: start,
                calendar: "Calendar1",
                endTime: d,
                id_dia_semana: start.getDate(),
                id_disponivel: true
            };

            if (item != null)
                calendar.store.add(item);
        }
    }
    catch (e) {
        postGerarLog(e);
    }

}

function excluiItemHorario() {
    try {
        var gradeHorarios = dijit.byId('gridHorarios');
        var store = gradeHorarios.items;
        var del = false;
        var cal2 = false;
        var diaSemana = [];
        var nenhumDiaIgual = 0;
        var mensagensWeb = new Array();
        if (hasValue(gradeHorarios.selectedItems)) {
            for (var i = gradeHorarios.selectedItems.length - 1; i >= 0; i--) {
                var itemSelected = gradeHorarios.selectedItems[i];
                if (itemSelected.calendar != "Calendar2") {
                    for (var j = 0; j < gradeHorarios.items.length; j++)
                        if (gradeHorarios.items[j].cssClass == "Calendar2" && new Date(gradeHorarios.items[j].startTime).getDate() == itemSelected.id_dia_semana &&
                            (
                            ((new Date(gradeHorarios.items[j].startTime)).getHours() > (new Date(itemSelected.endTime)).getHours() ||
                             ((new Date(gradeHorarios.items[j].startTime)).getHours() == (new Date(itemSelected.endTime)).getHours() &&
                             (new Date(gradeHorarios.items[j].startTime)).getMinutes() >= (new Date(itemSelected.endTime)).getMinutes())) ||

                            ((new Date(itemSelected.startTime)).getHours() > (new Date(gradeHorarios.items[j].endTime)).getHours() ||
                             ((new Date(gradeHorarios.items[j].endTime)).getHours() == (new Date(itemSelected.startTime)).getHours() &&
                             (new Date(gradeHorarios.items[j].endTime)).getMinutes() <= (new Date(itemSelected.startTime)).getMinutes()))
                            )
                           ) {
                            gradeHorarios.store.remove(itemSelected.id);
                            del = true;
                            apresentaMensagem('apresentadorMensagemFunc', null);

                        }
                        else
                            if (gradeHorarios.items[j].cssClass == "Calendar2" && new Date(gradeHorarios.items[j].startTime).getDate() == itemSelected.id_dia_semana)
                                nenhumDiaIgual = 1;
                }
                else
                    cal2 = true;

                if (nenhumDiaIgual == 0 && cal2 == false) {
                    gradeHorarios.store.remove(itemSelected.id);
                    del = true;
                    apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
                }

                if (del == false && cal2 == false) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Não é possível excluir o horário disponivel pois existe horário ocupado no período.');
                    apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
                }
                else
                    if (cal2 == true) {
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, 'Não é possível excluir o horário ocupado.');
                        apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
                    }
            }
        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDeleteHorarioSeleciona);
            apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
            return false
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function blockedEndOpenHabil() {
    try {
        dojo.xhr.get({
            preventCache: true,
            url: Endereco() + "/api/escola/getPermissaoHabilitacao",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (abilHabil) {
            abilHabil = jQuery.parseJSON(abilHabil);
            abilHabilidadeProf = abilHabil.retorno;
            blockedEndOpenHabilToglle(abilHabil.retorno)
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemFunc', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function updateTagProfessor() {
    try {
        dijit.byId("tagHabilProf").set("open", true);
        dijit.byId("TurmaAtual").set("open", true);
        dijit.byId("gridHabilProf").update();
        dijit.byId("gridTurmasAtuais").update();
        dijit.byId("tagHabilProf").set("open", false);
        dijit.byId("TurmaAtual").set("open", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirItemHorario() {
    try {
        apresentaMensagem('apresentadorMensagemFunc', null);
        var calendar = dijit.byId('gridHorarios');
        var timeFim = dijit.byId('timeFim');
        var timeIni = dijit.byId('timeIni');
        var arraySemana = dijit.byId('cbDias').value;
        if (!hasValue(calendar)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNaoConstruidoGradeHorario);
            apresentaMensagem(msg, mensagensWeb);
            return false
        }
        var itens = dijit.byId('gridHorarios').items;
        if (arraySemana.length <= 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDiaSemanaSelect);
            apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
            return false
        }
        else
            //if (timeIni.validate() && timeIni.validate()) {
            if (dijit.byId("formSemana").validate()) {
                if ((timeFim.value.getMinutes() % 5 != 0 || timeIni.value.getMinutes() % 5 != 0) || validarItemHorarioManual(timeIni, timeFim)) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotInclItemHorario);
                    apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
                    return false;
                }
                var dates = {
                    convert: function (dto) {
                        // Converts the date in dto to a date-object. The input can be:
                        //   a date object: returned without modification
                        //  an array      : Interpreted as [year,month,day]. NOTE: month is 0-11.
                        //   a number     : Interpreted as number of milliseconds
                        //                  since 1 Jan 1970 (a timestamp)
                        //   a string     : Any format supported by the javascript engine, like
                        //                  "YYYY/MM/DD", "MM/DD/YYYY", "Jan 31 2009" etc.
                        //  an object     : Interpreted as an object with year, month and date
                        //                  attributes.  **NOTE** month is 0-11.
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
                        // Compare two dates (could be of any type supported by the convert
                        // function above) and returns:
                        //  -1 : if a < b
                        //   0 : if a = b
                        //   1 : if a > b
                        // NaN : if a or b is an illegal date
                        // NOTE: The code inside isFinite does an assignment (=).
                        return (
                            isFinite(a = this.convert(a).valueOf()) &&
                            isFinite(b = this.convert(b).valueOf()) ?
                            (a > b) - (a < b) :
                            NaN
                        );
                    },
                    inRange: function (dto, start, end) {
                        // Checks if date in dto is between dates in start and end.
                        // Returns a boolean or NaN:
                        //    true  : if dto is between start and end (inclusive)
                        //    false : if dto is before start or after end
                        //    NaN   : if one or more of the dates is illegal.
                        // NOTE: The code inside isFinite does an assignment (=).
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

                for (var i = 0; i < arraySemana.length; i++) {
                    var diaSemana = parseInt(arraySemana[i]) + 1;
                    var d = new Date(2012, 6, diaSemana, timeIni.value.getHours(), timeIni.value.getMinutes());
                    var start, end;
                    var colView = calendar.columnView;
                    var cal = calendar.dateModule;
                    var gradeHorarios = dijit.byId('gridHorarios');
                    var resolveuIntersecao = false;

                    start = calendar.floorDate(d, "minute", colView.timeSlotDuration);
                    //start = d;
                    d = new Date(2012, 6, diaSemana, timeFim.value.getHours(), timeFim.value.getMinutes());
                    //end = cal.add(d, "minute", colView.timeSlotDuration);
                    //end = new Date(d.getTime() + 15*60000);

                    /****Verificar se existe interseção com um ou mais horários disponíveis:****/
                    for (var j = itens.length - 1; j >= 0; j--) {
                        // Verifica se o intervalo da hora do item tem interseção com o item selecionado:
                        if (itens[j].cssClass == "Calendar1" && (dates.interception(start, d, itens[j].startTime, itens[j].endTime))) {
                            // Verifica se um item inclui totalmente o outro item e remove o incluído:
                            var included = dates.include(start, d, itens[j].startTime, itens[j].endTime);

                            if (included == 1)
                                resolveuIntersecao = true;
                            else
                                if (included == 2)
                                    calendar.store.remove(itens[j].id);

                                    // Caso contrário, junta um item com o outro:
                                else
                                    if (included != NaN) {
                                        if (dates.compare(start, itens[j].startTime) == 1)
                                            start = itens[j].startTime;
                                        else
                                            d = itens[j].endTime;
                                        calendar.store.remove(itens[j].id);
                                    }
                        }
                    }

                    var id = gradeHorarios.items.length;

                    if (id > 0) {
                        quickSortObj(gradeHorarios.items, 'id');
                        id = gradeHorarios.items[id - 1].id + 1;
                    }
                    var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                    var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                    var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                    var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                    if (!resolveuIntersecao) {
                        var item = {
                            id: id,
                            cd_horario: 0,
                            dt_hora_ini: hIni + ":" + mIni + ":00",
                            dt_hora_fim: hFim + ":" + mFim + ":00",
                            summary: "",
                            startTime: start,
                            calendar: "Calendar1",
                            endTime: d,
                            id_dia_semana: start.getDate(),
                            id_disponivel: true
                        };

                        calendar.store.add(item);
                        colView.set("startTimeOfDay", { hours: hIni, duration: 500 });
                    }
                }
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function blockedEndOpenHabilToglle(acao) {
    try {
        if (acao) {
            toglleTag(acao, 'tagHabilProf');
        } else {
            toglleTag(acao, 'tagHabilProf');
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisaFuncionario(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var cdAtiv = hasValue(dojo.byId("pesCdAtividade").value) ? dojo.byId("pesCdAtividade").value : 0;
            var status = hasValue(dijit.byId("statusFunc").value) ? dijit.byId("statusFunc").value : 0;
            var cdTipoPesq = hasValue(dijit.byId("pesqFuncTipo").value) ? dijit.byId("pesqFuncTipo").value : 0;
            var colaboradorCyberPesq = hasValue(dijit.byId("colaboradorCyberPesq").value) ? dijit.byId("colaboradorCyberPesq").value : 0;
            var coordenadorPesq = hasValue(dijit.byId("coordenadorPesq").value) ? dijit.byId("coordenadorPesq").value : 0;
            myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/professor/GetFuncionarioSearch?nome=" + encodeURIComponent(dojo.byId("_nome").value) + "&apelido=" + encodeURIComponent(dojo.byId("_reduzido").value) +
                                     "&status=" + parseInt(status) + "&cpf=" + dojo.byId("_cpf").value + "&inicio=" + document.getElementById("inicioFunc").checked + "&tipo=" + cdTipoPesq + "&sexo=" + dijit.byId("nm_sexo").value + "&cdAtividade=" + cdAtiv + "&coordenador=" + coordenadorPesq + "&coolaborador_cyber=" + colaboradorCyberPesq,
                handleAs: "json",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_funcionario"
            }), Memory({ idProperty: "cd_funcionario" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridFuncionario = dijit.byId("gridFuncionario");
            if (limparItens) {
                gridFuncionario.itensSelecionados = [];
            }
            gridFuncionario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function eventoEditar(itensSelecionados) {
    try {
        limparCamposCargo();
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            //limparCadPessoaFK();
            apresentaMensagem('apresentadorMensagem', '');
            destroyCreateGridHorario();
            keepValues(null, dijit.byId('gridFuncionario'), true);
            IncluirAlterar(0, 'divAlterarFunc', 'divIncluirFunc', 'divExcluirFunc', 'apresentadorMensagemFunc', 'divCancelarFunc', 'divClearFunc');
            dijit.byId("formFunc").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverFuncionario(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarFuncionario(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadHorarioProfessor(cdPessoa) {
    showCarregando();
    dojo.xhr.get({
        preventCache: true,
        url: Endereco() + "/api/professor/geHorarioByEscolaForProfessor?cdFunc=" + parseInt(cdPessoa),
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataHorario) {
        try {
            dataHorario = jQuery.parseJSON(dataHorario);
            var menorHorario = null;
            var menorData = null;
            if (hasValue(dataHorario.retorno) && dataHorario.retorno.length > 0) {
                var codMaiorHorarioTurma = 0;
                $.each(dataHorario.retorno, function (idx, val) {
                    codMaiorHorarioTurma += 1;
                    //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
                    val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
                    val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
                    val.id = codMaiorHorarioTurma;
                    //Pega o menor horário:
                    if (menorData == null || val.dt_hora_ini < menorHorario) {
                        menorData = val.startTime;
                        menorHorario = val.dt_hora_ini;
                    }
                });
                dijit.byId("gridFuncionario").horarioFuncionario = dataHorario.retorno.slice();

                //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                setTimeout(function () { posicionaPrimeiraLinhaHorarioFuncionario(dataHorario.retorno.length, menorData); }, 100);
            }
            criacaoGradeHorario(dataHorario.retorno);
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    },
    function (error) {
        showCarregando();
        apresentaMensagem('apresentadorMensagemFunc', error);
    });
}

function posicionaPrimeiraLinhaHorarioFuncionario(tamanhoHorarios, menorData) {
    var gridHorarios = dijit.byId('gridHorarios');
    try {
        if (hasValue(gridHorarios) && hasValue(gridHorarios.items) && gridHorarios.items.length >= tamanhoHorarios)
            gridHorarios.columnView.set("startTimeOfDay", { hours: menorData.getHours(), duration: 500 });
        else
            setTimeout(function () { posicionaPrimeiraLinhaHorarioFuncionario(tamanhoHorarios, menorData); }, 100);
    }
    catch (e) {
        setTimeout(function () { posicionaPrimeiraLinhaHorarioFuncionario(tamanhoHorarios, menorData); }, 100);
    }
}

function loadDataFuncioario(cdFuncionario, prof) {
    dojo.xhr.get({
        url: Endereco() + "/api/professor/getDataFuncionario?cdFunc=" + parseInt(cdFuncionario) + "&prof=" + prof,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataFuncionario) {
        dataFuncionario = jQuery.parseJSON(dataFuncionario);
        setValueFuncionario(dataFuncionario.retorno);
        checkedValuesTrue(dataFuncionario.retorno.funcionarioSGF.ProdutoFuncionario);
        if (hasValue(dataFuncionario.retorno.funcionarioSGF) &&
            dataFuncionario.retorno.funcionarioSGF.id_coordenador == true) {
            dijit.byId('cbProdutoAssinatura').set('disabled', false);
            dijit.byId("ckCoordenador").set('checked', true);
        } else {
            dijit.byId('cbProdutoAssinatura').set('disabled', true);
            dijit.byId("ckCoordenador").set('checked', false);
        }

    });
}


function checkedValuesTrue(produtosAssinatura) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function (Memory, Array) {
            try {
                dijit.byId('cbProdutoAssinatura').reset();

                for (var i = 0; i < dijit.byId("cbProdutoAssinatura").options.length; i++) {
                    for (var j = 0; j < produtosAssinatura.length; j++) {
                        if (dijit.byId("cbProdutoAssinatura").options[i].item.id[0] == produtosAssinatura[j].cd_produto) {
                            console.log(dijit.byId("cbProdutoAssinatura").options[i].item.id[0] == produtosAssinatura[j].cd_produto);
                            dijit.byId("cbProdutoAssinatura").options[i].selected = true;
                            dijit.byId("cbProdutoAssinatura").onChange(true);
                        }
                    }
                }
            }
            catch (e) {
                //showCarregando();
                postGerarLog(e);
            }
        });
}

function setValueFuncionario(funcionarioUI) {
    require(["dojo/ready", "dojo/store/Memory", "dojo/data/ObjectStore"],
        function (ready, Memory, ObjectStore) {
            ready(function () {
                var funcionario = null;
                var habilProf = null;
                var turmaAtivas = null;
                if (hasValue(funcionarioUI) && hasValue(funcionarioUI.funcionarioSGF)) {
                    funcionario = funcionarioUI.funcionarioSGF;
                    if (hasValue(funcionarioUI.funcionarioSGF.turmaAtivas))
                        turmaAtivas = funcionarioUI.funcionarioSGF.turmaAtivas;
                }
                if (hasValue(funcionarioUI) && hasValue(funcionarioUI.HablitacoesProfessorUI))
                    habilProf = funcionarioUI.HablitacoesProfessorUI;
                if (hasValue(funcionario)) {

                    var count = 1;
                    for (var i = 0; i < funcionario.FuncionarioComissao.length; i++) {
                        funcionario.FuncionarioComissao[i].id = count;
                        funcionario.FuncionarioComissao[i].vl_comissao_matricula = hasValue(funcionario.FuncionarioComissao[i].vl_comissao_matricula) ? funcionario.FuncionarioComissao[i].vl_comissao_matricula : "";
                        funcionario.FuncionarioComissao[i].pc_comissao_matricula = hasValue(funcionario.FuncionarioComissao[i].pc_comissao_matricula) ? funcionario.FuncionarioComissao[i].pc_comissao_matricula : "";
                        funcionario.FuncionarioComissao[i].vl_comissao_rematricula = hasValue(funcionario.FuncionarioComissao[i].vl_comissao_rematricula) ? funcionario.FuncionarioComissao[i].vl_comissao_rematricula : "";
                        funcionario.FuncionarioComissao[i].pc_comissao_rematricula = hasValue(funcionario.FuncionarioComissao[i].pc_comissao_rematricula) ? funcionario.FuncionarioComissao[i].pc_comissao_rematricula : "";

                        count++;
                    }
                    var grid = dijit.byId("gridComissao");
                    var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: funcionario.FuncionarioComissao, idProperty: "id" }) });
                    grid.setStore(dataStore);
                    grid.update();

                    dojo.byId("cdFuncionario").value = funcionario.cd_funcionario;
                    dojo.byId("dtaAdmissao").value = funcionario.dta_admissao;
                    dojo.byId("dtaDemissao").value = funcionario.dta_demissao;
                    dojo.byId("cdCargo").value = funcionario.cd_cargo;
                    dijit.byId("idPessoaAtiva").set("checked", funcionario.id_funcionario_ativo);
                    dijit.byId("ckComissionado").set("checked", funcionario.id_comissionado);

                    if (funcionario.id_comissionado) {
                        dojo.byId('tgComissao').style.display = 'block'
                        dijit.byId('tgComissao').set('open', true);
                    } else {
                        dijit.byId('tgComissao').set('open', false);
                    }

                    dijit.byId("idProfessor")._onChangeActive = false;
                    dijit.byId("idProfessor").set("checked", funcionario.id_professor);
                    dijit.byId("idColaboradorCyber").set("checked", funcionario.id_colaborador_cyber);
                    toggleDisabled(dijit.byId("tabHorarios"), !funcionario.id_professor);
                    dijit.byId("idProfessor")._onChangeActive = true;
                    dojo.byId("vl_salario").value = funcionario.vlSalario == 0 ? null : funcionario.vlSalario;
                    dijit.byId("nomAssinaturaCertificado").set("value", funcionario.nome_assinatura_certificado);
                    if (funcionario.id_professor) {
                        toglleTag(funcionario.id_professor, 'TagProfessor');
                    }
                    if (funcionario.id_professor) {
                        dijit.byId("formaPagto").set("value", funcionario.id_forma_pagamento);
                        dijit.byId("ckContrato").set("checked", funcionario.id_contratado)
                        dojo.byId("valInterno").value = hasValue(funcionario.vlPagamentoInterno) ? funcionario.vlPagamentoInterno : 0;
                        dojo.byId("valExterno").value = hasValue(funcionario.vlPagamentoExterno) ? funcionario.vlPagamentoExterno : 0;
                        dojo.byId("dcChapa").value = hasValue(funcionario.dc_numero_chapa) ? funcionario.dc_numero_chapa : "";
                        dijit.byId("ckCoordenador").set("checked", funcionario.id_coordenador);

                        dijit.byId("pcTerminoEstagio").set("value", funcionario.pc_termino_estagio);
                        dojo.byId("vlrTerminoEstagio").value = funcionario.vlTerminoEstagio;
                        dojo.byId("vlrRematricula").value = funcionario.vlRematricula;
                    }
                    if (hasValue(habilProf) && hasValue(habilProf.length > 0)) {
                        var dataStore = new ObjectStore({ objectStore: new Memory({ data: habilProf }) });
                        dijit.byId("gridHabilProf").setStore(dataStore);
                        //criarGradesProfessor(habilProf,null);
                        //setTimeout('atualiza()', 2001);
                    }
                    if (hasValue(turmaAtivas) && hasValue(turmaAtivas.length > 0)) {
                        var dataStoreTurmas = new ObjectStore({ objectStore: new Memory({ data: turmaAtivas }) });
                        dijit.byId("gridTurmasAtuais").setStore(dataStoreTurmas);
                    }
                    dijit.byId("tagHabilProf").set("open", true);
                    dijit.byId("TurmaAtual").set("open", true);
                    dijit.byId("gridHabilProf").update();
                    dijit.byId("gridTurmasAtuais").update();
                    dijit.byId("tagHabilProf").set("open", false);
                    dijit.byId("TurmaAtual").set("open", false);
                    if (hasValue(funcionario.FuncionarioAtividade) && hasValue(funcionario.FuncionarioAtividade.no_atividade)) {
                        dijit.byId('desCargo').set("value", funcionario.FuncionarioAtividade.no_atividade);
                        dijit.byId("limparFkCargo").set("disabled", false);
                    }
                }
            });
        });
}

function keepValues(value, grid, ehLink) {
    try {
        limparFuncionario();
        setarTabCad();
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

        if (value.cd_funcionario > 0) {
            var papelRelac = new Array();
            papelRelac[0] = RELACENTREPESSOAS;
            showPessoaFK(value.nm_natureza_pessoa, value.cd_pessoa_funcionario, papelRelac);
            //ShowFoto(value);
            loadDataFuncioario(value.cd_funcionario, value.id_professor);
            return false;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarHabilitacao() {
    try {
        var dados = [];
        var storeHabil = dijit.byId("gridHabilProf");
        if (hasValue(storeHabil) && hasValue(storeHabil) && hasValue(storeHabil)) {
            if (storeHabil.store.objectStore.data.length > 0)
                $.each(storeHabil.store.objectStore.data, function (index, value) {
                    dados.push({
                        "cd_produto": value.cd_produto,
                        "cd_estagio": value.cd_estagio
                    });
                });
        }
        return dados;
    }
    catch (e) {
        postGerarLog(e);
    }
}

// metodos crud funcionario

function cadastrarOuAlterarFuncionario(windowUtils, tipoOperacao) {
    try {
        var validado = validarCadFuncionario(windowUtils);
        if (!validado) {
            setarTabCad();
            return false;
        }
        var files = dijit.byId("uploaderAssinatura")._files;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("UploadedImage", files[i]);
                }
                $.ajax({
                    type: "POST",
                    url: Endereco() + "/api/professor/uploadCertificadoProfessor",
                    ansy: false,
                    headers: { Authorization: Token() },
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (results) {
                        try {
                            if (hasValue(results) && hasValue(results.indexOf) && results.indexOf('<') >= 0) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgSessaoExpirada3);
                                apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
                                return false;
                            }
                            if (hasValue(results) && !hasValue(results.erro)  ) {
                                if (tipoOperacao == CADASTRO)
                                    salvarFuncionario(results);
                                else
                                    alterarFuncionario(results);
                            } else
                                apresentaMensagem('apresentadorMensagemFunc', results);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    error: function (error) {
                        apresentaMensagem('apresentadorMensagemFunc', error);
                        return false;
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagemFunc', mensagensWeb);
            }
        } else {
            if (tipoOperacao == CADASTRO)
                salvarFuncionario("");
            else
                alterarFuncionario("");
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarFuncionario(nomeCertificadoTemporario) {
        try {
            var habilitacoes = null;
            var horarios = null;

            if (dijit.byId("idProfessor").get("checked") && hasValue(dijit.byId("gridHorarios")) && dijit.byId("gridHorarios").items.length > 0)
                horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);

            if (hasValue(abilHabilidadeProf) && abilHabilidadeProf)
                if (hasValue(dijit.byId("gridHabilProf")) && hasValue(dijit.byId("gridHabilProf").store.objectStore) && hasValue(dijit.byId("gridHabilProf").store.objectStore.data))
                    habilitacoes = montarHabilitacao();
            var pessoaFisicaUI = montarDadosPessoaFisica(dojo.date, 'apresentadorMensagemFunc');
            if (!pessoaFisicaUI) return false;
            showCarregando();
            dojo.xhr.post({
                url: Endereco() + "/api/professor/PostInsertFuncionario",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson({
                    funcionario: {
                        dt_admissao: hasValue(dojo.byId("dtaAdmissao").value) ? dojo.date.locale.parse(dojo.byId("dtaAdmissao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        dt_demissao: hasValue(dojo.byId("dtaDemissao").value) ? dojo.date.locale.parse(dojo.byId("dtaDemissao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        id_comissionado: dijit.byId("ckComissionado").get("checked"),
                        id_professor: dijit.byId("idProfessor").get("checked"),
                        id_funcionario_ativo: dijit.byId("idPessoaAtiva").get("checked"),
                        vl_salario: hasValue(dojo.byId("vl_salario").value) ? dijit.byId("vl_salario").get('value') : 0,
                        cd_cargo: hasValue(dojo.byId("cdCargo").value) ? dojo.byId("cdCargo").value : null,
                        id_colaborador_cyber: dijit.byId("idColaboradorCyber").get("checked"),
                        nome_temp_assinatura_certificado: hasValue(nomeCertificadoTemporario) ? nomeCertificadoTemporario : null,
                        nome_assinatura_certificado: hasValue(dojo.byId("nomAssinaturaCertificado").value) ? dojo.byId("nomAssinaturaCertificado").value : null
                    },
                    professor: {
                        id_forma_pagamento: hasValue(dijit.byId("formaPagto").get("value")) ? dijit.byId("formaPagto").get("value") : null,
                        id_contratado: dijit.byId("ckContrato").get("checked"),
                        vl_pagamento_interno: hasValue(dojo.byId("valInterno").value) ? dijit.byId("valInterno").get('value') : 0,
                        vl_pagamento_externo: hasValue(dojo.byId("valExterno").value) ? dijit.byId("valExterno").get('value') : 0,
                        dc_numero_chapa: hasValue(dojo.byId("dcChapa").value) ? dojo.byId("dcChapa").value : "",
                        id_coordenador: dijit.byId("ckCoordenador").get("checked"),
                        pc_termino_estagio: hasValue(dojo.byId("pcTerminoEstagio").value) ? dijit.byId("pcTerminoEstagio").get('value') : 0,
                        vl_termino_estagio: hasValue(dojo.byId("vlrTerminoEstagio").value) ? dijit.byId("vlrTerminoEstagio").get('value') : 0,
                        vl_rematricula: hasValue(dojo.byId("vlrRematricula").value) ? dijit.byId("vlrRematricula").get('value') : 0
                    },
                    pessoaFisicaUI: pessoaFisicaUI,
                    habilitacaoProfessor: habilitacoes,
                    horarioSearchUI: horarios,
                    cd_produtos_funcionario: montarparametrosmulti('cbProdutoAssinatura')
                })
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridFuncionario';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        data = data.retorno;

                        if (!hasValue(grid.itensSelecionados)) {
                            grid.itensSelecionados = [];
                        }
                        insertObjSort(grid.itensSelecionados, "cd_funcionario", itemAlterado);
                        dijit.byId("formFunc").hide();
                        buscarItensSelecionados(gridName, 'selecionaFuncionario', 'cd_funcionario', 'selecionaTodos', ['pesquisarFunc', 'relatorioFuncionario'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_funcionario");
                    } else
                        apresentaMensagem('apresentadorMensagemFunc', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemFunc', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
}

function validarCadFuncionario(windowUtils,operacao){
    var validado = true;
    if (!hasValue(dojo.byId("cdPessoaCpf").value))
        if (!validarCPF("#cpf", "apresentadorMensagemFunc")) {
            setarTabCad();
            validado = false;
        }
    if (!validateCadPessoaFK(windowUtils)) {
        setarTabCad();
        validado = false;
    }
    if (!dijit.byId("formfuncionario").validate() || !dijit.byId("formFantasiaPessoa").validate()) {
        dijit.byId("tagFuncionario").set("open", true);
        setarTabCad();
        validado = false;
    }

    if (hasValue(dojo.byId("dtaDemissao").value)) {
        dijit.byId("dtaDemissao").set("required", true);
        if (!dijit.byId("dtaDemissao").validate()) {
            dijit.byId("tagFuncionario").set("open", true);
            validado = false;
        }
        dijit.byId("dtaDemissao").set("required", false);
    }
    if (dijit.byId("idProfessor").get("checked") || dijit.byId("idColaboradorCyber").get("checked")) {
        if (!dijit.byId("email").validate()) {
            dijit.byId("panelEndereco").set("open", true);
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("email"));
            validado = false;
        }
    }
    if(operacao == EDICAO)
        if (!dijit.byId('pcTerminoEstagio').validate())
        {
            mostrarMensagemCampoValidado(windowUtils, dijit.byId("pcTerminoEstagio"));
            validado = false;
        }
    return validado;
}

function mountHorarios(dataHorarios) {
    try {
        var retorno = [];
        $.each(dataHorarios, function (index, value) {
            if (!hasValue(value.removeTimeZone)) {
                value.endTime = removeTimeZone(value.endTime);
                value.startTime = removeTimeZone(value.startTime);
                value.removeTimeZone = true;
            }
            retorno.push(value);
        });
        return retorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarFuncionario(nomeCertificadoTemporario) {
    try {
        var habilitacoes = null;
        var horarios = null;
        var habilitacaoNula = true;
        if (hasValue(abilHabilidadeProf) && abilHabilidadeProf) {
            if (hasValue(dijit.byId("gridHabilProf")) && hasValue(dijit.byId("gridHabilProf").store.objectStore) && hasValue(dijit.byId("gridHabilProf").store.objectStore.data))
                habilitacoes = montarHabilitacao();
        }
        if (dijit.byId("idProfessor").get("checked") && hasValue(dijit.byId("gridHorarios")) && dijit.byId("gridHorarios").items.length >= 0) {
            horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
            habilitacaoNula = false;
        }
        var pessoaFisicaUI = montarDadosPessoaFisica(dojo.date, 'apresentadorMensagemFunc');
        if (!pessoaFisicaUI) return false;
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/professor/PostUpdateFuncionario",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson({
                funcionario: {
                    cd_funcionario: hasValue(dojo.byId('cdFuncionario').value) ? dojo.byId('cdFuncionario').value : 0,
                    dt_admissao: hasValue(dojo.byId("dtaAdmissao").value) ? dojo.date.locale.parse(dojo.byId("dtaAdmissao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    dt_demissao: hasValue(dojo.byId("dtaDemissao").value) ? dojo.date.locale.parse(dojo.byId("dtaDemissao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    id_comissionado: dijit.byId("ckComissionado").get("checked"),
                    id_professor: dijit.byId("idProfessor").get("checked"),
                    vl_salario: hasValue(dojo.byId("vl_salario").value) ? dijit.byId("vl_salario").get('value') : 0,
                    id_funcionario_ativo: dijit.byId("idPessoaAtiva").get("checked"),
                    cd_cargo: hasValue(dojo.byId("cdCargo").value) ? dojo.byId("cdCargo").value : null,
                    id_colaborador_cyber: dijit.byId("idColaboradorCyber").get("checked"),
                    FuncionarioComissao: dijit.byId("gridComissao").store.objectStore.data,
                    nome_temp_assinatura_certificado: hasValue(nomeCertificadoTemporario) ? nomeCertificadoTemporario : null,
                    nome_assinatura_certificado: hasValue(dojo.byId("nomAssinaturaCertificado").value) ? dojo.byId("nomAssinaturaCertificado").value : null
                    
                },
                professor: {
                    id_forma_pagamento: hasValue(dijit.byId("formaPagto").get("value")) ? dijit.byId("formaPagto").get("value") : null,
                    id_contratado: dijit.byId("ckContrato").get("checked"),
                    vl_pagamento_interno: hasValue(dojo.byId("valInterno").value) ? dijit.byId("valInterno").get('value') : 0,
                    vl_pagamento_externo: hasValue(dojo.byId("valExterno").value) ? dijit.byId("valExterno").get('value') : 0,
                    dc_numero_chapa: hasValue(dojo.byId("dcChapa").value) ? dojo.byId("dcChapa").value : "",
                    id_coordenador: dijit.byId("ckCoordenador").get("checked"),
                    pc_termino_estagio: hasValue(dojo.byId("pcTerminoEstagio").value) ? dijit.byId("pcTerminoEstagio").get('value') : 0,
                    vl_termino_estagio: hasValue(dojo.byId("vlrTerminoEstagio").value) ? dijit.byId("vlrTerminoEstagio").get('value') : 0,
                    vl_rematricula: hasValue(dojo.byId("vlrRematricula").value) ? dijit.byId("vlrRematricula").get('value') : 0
                },
                pessoaFisicaUI: pessoaFisicaUI,
                habilitacaoProfessor: habilitacoes,
                habilitacaoNula: habilitacaoNula,
                horarioSearchUI: horarios,
                cd_produtos_funcionario: montarparametrosmulti('cbProdutoAssinatura')
            })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridFuncionario';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados)) {
                        grid.itensSelecionados = [];
                    }
                    removeObjSort(grid.itensSelecionados, "cd_funcionario", dojo.byId("cdFuncionario").value);
                    insertObjSort(grid.itensSelecionados, "cd_funcionario", itemAlterado);
                    dijit.byId("formFunc").hide();
                    buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_funcionario', 'selecionaTodos', ['pesquisarFunc', 'relatorioFuncionario'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_funcionario");
                }
                else
                    apresentaMensagem('apresentadorMensagemFunc', data);
                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemFunc', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarFuncionario(itensSelecionados) {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cdFuncionario').value != 0)
                    itensSelecionados = [{
                        cd_funcionario: dom.byId("cdFuncionario").value,
                        cd_pessoa_funcionario: dom.byId("cd_pessoa").value
                    }];
            xhr.post({
                url: Endereco() + "/api/professor/postdeleteFuncionario",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    var todos = dojo.byId("todosItens_label");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("_nome").set("value", '');
                    // Remove o item dos itens selecionados:
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridFuncionario').itensSelecionados, "cd_funcionario", itensSelecionados[r].cd_funcionario);
                    pesquisaFuncionario(false);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarFunc").set('disabled', false);
                    dijit.byId("relatorioFuncionario").set('disabled', false);
                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                    dijit.byId("formFunc").hide();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("formFunc").style.display))
                    apresentaMensagem('apresentadorMensagemFunc', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

// fim metodo crud Funcionario
function getNameFoto(files) {
    if (hasValue(files.name))
        return files.name;
    return null;
}

function loadProduto(cdProduto, populaProdutoViewComissao) {
    dojo.xhr.get({
        url: Endereco() + "/api/coordenacao/getAllProduto",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataProduto) {
        try {
            (hasValue(jQuery.parseJSON(dataProduto).retorno)) ? dataProduto = jQuery.parseJSON(dataProduto).retorno : jQuery.parseJSON(dataProduto);

            var dados = [];
            $.each(dataProduto, function (index, val) {
                dados.push({
                    "name": val.no_produto,
                    "id": val.cd_produto
                });
            });

            var stateStore = new dojo.store.Memory({
                data: eval(dados)
            });

            if (hasValue(populaProdutoViewComissao) && populaProdutoViewComissao) {
                dijit.byId("produtoViewNovaComissao").store = stateStore;
                if (hasValue(cdProduto))
                    dijit.byId("produtoViewNovaComissao").set("value", cdProduto);
            } else {
                dijit.byId("ProdutoHab").store = stateStore;
                if (hasValue(cdProduto))
                    dijit.byId("ProdutoHab").set("value", cdProduto);
            }
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function loadEstagiosByEstagio(cdProduto) {
    dijit.byId("EstagioHab").reset();
    dojo.xhr.get({
        url: Endereco() + "/api/coordenacao/getAllEstagioByProduto?id=" + cdProduto,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataEstagio) {
        try {
            dataEstagio = jQuery.parseJSON(dataEstagio);
            hasValue(dataEstagio.retorno) ? dataEstagio = dataEstagio.retorno : dataEstagio;

            var dados = [];
            var estagioCb = [];
            $.each(dataEstagio, function (index, val) {
                dados.push({
                    "name": val.no_estagio,
                    "id": val.cd_estagio
                });
                estagioCb.push({
                    "cd_estagio": val.cd_estagio+"",
                    "no_estagio": val.no_estagio
                });
            });

            var stateStore = new dojo.store.Memory({
                data: eval(dados)
            });
            dijit.byId("EstagioHab").store = stateStore;

            if (hasValue(dojo.byId("setValueCdEstagio").value)) {
                dijit.byId("EstagioHab").set("value", dojo.byId("setValueCdEstagio").value);
                dojo.byId("setValueCdEstagio").value = "";
            }

            var w = dijit.byId("cbMultiHabEstagio");

            //Adiciona a opção todos no checkedmultiselect
            estagioCb.unshift({
                "cd_estagio": -1+"",
                "no_estagio": "Todos"
            });
            var storeMEstagio = new dojo.data.ItemFileReadStore({
                data: {
                    identifier: "cd_estagio",
                    label: "no_estagio",
                    items: estagioCb
                }
            });
            w.setStore(storeMEstagio, []);
            
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

//Metodos Cruds Hablilitacao.
function incluirHabilitacao() {
    require(["dojo/dom", 
        "dojo/_base/xhr", 
        "dojox/json/ref", 
        "dojo/store/Memory", 
        "dojo/data/ObjectStore"], 
        function (dom, xhr, ref, Memory, ObjectStore) {
            try {
                if (!dijit.byId("formHabilitacao").validate()) {
                    return false;
                }
                var estagios = dijit.byId('cbMultiHabEstagio').get('value');
                if(estagios[0] == "-1") { estagios.splice (estagios.indexOf('-1'), 1);}//remove o item (Todos) do array
                var domGridHabProf = dijit.byId("gridHabilProf");

                $.each(estagios, function (index, cd_estagio){
                    var dados = dijit.byId("EstagioHab").store.data;
                    quickSortObj(dados, 'id');
                    quickSortObj(domGridHabProf, "cd_estagio");

                    var posicao = binaryObjSearch(dados, 'id', cd_estagio);
                    var newHabilitacao = {
                        cd_professor: 0,
                        cd_produto: hasValue(dijit.byId("ProdutoHab").get("value")) ? dijit.byId("ProdutoHab").get("value") : 0,
                        cd_estagio: cd_estagio,
                        no_produto: hasValue(dijit.byId("ProdutoHab").displayedValue) ? dijit.byId("ProdutoHab").displayedValue : "",
                        no_estagio: dados[posicao].name
                    }

                    //dijit.byId("gridHabilProf").store.newItem(newHabilitacao);
                    insertObjSort(domGridHabProf.store.objectStore.data, "cd_estagio", newHabilitacao);
                });
                domGridHabProf.setStore(new ObjectStore({ objectStore: new Memory({ data: domGridHabProf.store.objectStore.data }) }));

                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Habilitação incluido com sucesso.");
                apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                dijit.byId("dialogHabilitacao").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
}


function keepValuesHabilitacao() {
    try {
        limparFormHabilProf();
        var value = dijit.byId("gridHabilProf").selection.getSelected();

        if (value.length > 0) {
            loadProduto(value[0].cd_produto);
            dijit.byId("ProdutoHab").set("value", value[0].cd_produto);
            dojo.byId("setValueCdEstagio").value = value[0].cd_estagio;
            dojo.byId("noProduto").value = value[0].no_produto;
            dojo.byId("noEstagio").value = value[0].no_estagio;
            dojo.byId("cdProfessor").value = value[0].cd_professor;
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFormHabilProf() {
    if (hasValue(dojo.byId('cbMultiHabEstagio').value, true)) 
        dijit.byId("cbMultiHabEstagio").reset();
    if (hasValue(dijit.byId('cbMultiHabEstagio')) && hasValue(dijit.byId('cbMultiHabEstagio').store))
        dijit.byId('cbMultiHabEstagio').setStore(dijit.byId('cbMultiHabEstagio').store, [0]);
    clearForm("formHabilitacao");
}

function limparProdutoAssinatura() {
    if (hasValue(dojo.byId('cbProdutoAssinatura').value, true)) 
        dijit.byId("cbProdutoAssinatura").reset();
    if (hasValue(dijit.byId('cbProdutoAssinatura')) && hasValue(dijit.byId('cbProdutoAssinatura').store))
        dijit.byId('cbProdutoAssinatura').setStore(dijit.byId('cbProdutoAssinatura').store, [0]);
}

function limparFormComissao() {
    
    dijit.byId("produtoViewNovaComissao").reset();

    dijit.byId("valorMatNovaComissao").set("value", "");
    dijit.byId("percentualMatNovaComissao").set("value", "");
    dijit.byId("valorRmtNovaComissao").set("value", "");
    dijit.byId("percentualRmtNovaComissao").set("value", "");
}

function alterarHabilitacao() {
    try {
        var gridHabilProf = dijit.byId("gridHabilProf").selection.getSelected();
        if (gridHabilProf.length > 0) {
            gridHabilProf[0].cd_produto = dijit.byId("ProdutoHab").get("value"),
            gridHabilProf[0].cd_estagio = dijit.byId("EstagioHab").get("value"),
            gridHabilProf[0].no_produto = dijit.byId("ProdutoHab").displayedValue,
            gridHabilProf[0].no_estagio = dijit.byId("EstagioHab").displayedValue,
            gridHabilProf[0].cd_professor = dojo.byId("cdProfessor").value
        }
        dijit.byId("gridHabilProf").update();
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Habilitação alterado com sucesso.");
        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
        dijit.byId("dialogHabilitacao").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluirHabilitacao() {
    try {
        var habilitacaoes = dijit.byId("gridHabilProf").selection.getSelected();
        if (habilitacaoes.length > 0) {
            var arrayHabilitacaoes = dijit.byId("gridHabilProf")._by_idx;
            arrayHabilitacaoes = jQuery.grep(arrayHabilitacaoes, function (value) {
                return value.item != habilitacaoes[0];
            });
            var dados = [];
            $.each(arrayHabilitacaoes, function (index, value) {
                dados.push(value.item);
            });
            var dataStore = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) });
            dijit.byId("gridHabilProf").setStore(dataStore);
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Habilitação excluido com sucesso.");
            apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
            dijit.byId("dialogHabilitacao").hide();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparGridTurmasAtuais() {
    try {
        if (hasValue(dijit.byId("gridTurmasAtuais"))) {
            require(["dojo/ready", "dojo/store/Memory", "dijit/form/FilteringSelect", "dojo/data/ObjectStore"],
            function (ready, Memory, filteringSelect, ObjectStore) {
                dijit.byId("gridTurmasAtuais").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                dijit.byId("gridHabilProf").setStore(new ObjectStore({ objectStore: new Memory({ data: null }) }));
                dijit.byId("gridTurmasAtuais").update();
                dijit.byId("gridHabilProf").update();
            });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFormFuncionario() {
    try {
        dojo.byId("cdFuncionario").value = 0;
        dijit.byId("idProfessor").reset();
        clearForm("formfuncionario");
        clearForm("formProfessor");
        limparFormHabilProf();
        limparProdutoAssinatura();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparFuncionario() {
    try {
        dojo.byId("atividadePrincipal").value = "";
        dojo.byId("pesCdAtividade").value = 0;
        limparCadPessoaFK();
        limparFormFuncionario();
        limparFormHabilProf();
        limparProdutoAssinatura();
        limparGridTurmasAtuais();
        clearForm("formSemana");
        dijit.byId("uploaderAssinatura").reset();
        dijit.byId('cbDias').setStore(dijit.byId('cbDias').store, []);
        if (hasValue(dijit.byId("gridHorarios"))) {
            if (hasValue(dojo.byId("cdFuncionario").value) && dojo.byId("cdFuncionario").value > 0) {
                limparNewsHorarios();
            } else {
                limparTodosHorarios();
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTodosHorarios() {
    try {
        var gradeHorarios = dijit.byId('gridHorarios');
        var store = gradeHorarios.items;
        if (store.length > 0) {
            for (var i = store.length - 1; i >= 0; i--) {
                if (gradeHorarios.items[i].id >= 0)
                    gradeHorarios.store.remove(store[i].id);
            }

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparNewsHorarios() {
    try {
        var gradeHorarios = dijit.byId('gridHorarios');
        var store = gradeHorarios.items;
        if (store.length > 0) {
            for (var i = store.length - 1; i >= 0; i--) {
                if (!hasValue(gradeHorarios.store.data[i].cd_horario) && !gradeHorarios.store.data[i].cd_horario > 0)
                    gradeHorarios.store.remove(store[i].id);
            }

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarTabCad() {
    try {
        var tabs = dijit.byId("tabContainer");
        var pane = dijit.byId("tabFuncionario");
        tabs.selectChild(pane);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function ExtistsPessoByCpf() {
    if ($("#cpf").val()) {
        dojo.xhr.get({
            url: Endereco() + "/api/professor/ExistFuncionarioOrPessoaFisicaByCpf?cpf=" + $("#cpf").val(),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (data.retorno != null && data.retorno.pessoaFisica != null) {
                    caixaDialogo(DIALOGO_CONFIRMAR, data.MensagensWeb[0].mensagem, function executaRetorno() {
                        limparCadPessoaFK();
                        setarValuePessoaFisica(data.retorno);
                    });
                }
                else {
                    if (hasValue(data.MensagensWeb) && hasValue(data.MensagensWeb[0]))
                        caixaDialogo(DIALOGO_AVISO, data.MensagensWeb[0].mensagem, 0, 0, 0);
                    if (data != null && hasValue(data.erro))
                        apresentaMensagem(dojo.byId("descApresMsg").value, data.erro);
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem(dojo.byId("descApresMsg").value, error);
        });
    }
}

function abrirPesquisaAuxiliaresPessoaCargoFK(cargo) {
    try {
        clearFormAtividadeFk(cargo);
        dijit.byId("proAuxiliaresPessoaFK").show();
        dijit.byId("gridAuxiliaresPessoaFK").update();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparCamposCargo() {
    try {
        dojo.byId("pesCdAtividade").value = 0;
        dijit.byId("atividadeFunc").set('value', '');
        dijit.byId("limparFkCargoPes").set("disabled", true);
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

function loadDataFiltrosTipoFunc(Memory) {
    try {
        var statusStoreTipo = new Memory({
            data: [
            { name: "Todos", id: 0 },
            { name: "Funcionário", id: 1 },
            { name: "Professor", id: 2 }
            ]
        });
        dijit.byId("pesqFuncTipo").store = statusStoreTipo;
        dijit.byId("pesqFuncTipo").set("value", TODOS);
        var statusStoreTipoFun = new Memory({
            data: [
            { name: "Todos", id: 0 },
            { name: "Sim", id: 1 },
            { name: "Não", id: 2 }
            ]
        });
        dijit.byId("coordenadorPesq").store = statusStoreTipoFun;
        dijit.byId("coordenadorPesq").set("value", TODOS);
        dijit.byId("colaboradorCyberPesq").store = statusStoreTipoFun;
        dijit.byId("colaboradorCyberPesq").set("value", TODOS);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirNovaComissao() {
    require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref", "dojo/store/Memory", "dojo/data/ObjectStore"], function (dom, xhr, ref, Memory, ObjectStore) {
        try {          
            if (dijit.byId("gridComissao").store.objectStore.data.some(x => x.cd_produto == (hasValue(dijit.byId('produtoViewNovaComissao')) && dijit.byId('produtoViewNovaComissao').item.id))) {
                caixaDialogo(DIALOGO_AVISO, 'Produto já cadastrado.', null);
                return false;
            }

            if (!dijit.byId("formComissao").validate()) {
                return false;
            }
            var mensagensWeb = new Array();

            var grid = dijit.byId("gridComissao");
            insertObjSort(grid.store.objectStore.data, "id", 
                { 
                    id: geradorIdComissao(dijit.byId("gridComissao")),
                    no_produto: dijit.byId('produtoViewNovaComissao').item.name,
                    cd_produto: dijit.byId('produtoViewNovaComissao').item.id,
                    vl_comissao_matricula: hasValue(dijit.byId('valorMatNovaComissao').get('value')) ? dijit.byId('valorMatNovaComissao').get('value') : "",
                    pc_comissao_matricula: hasValue(dijit.byId('percentualMatNovaComissao').get('value')) ? dijit.byId('percentualMatNovaComissao').get('value') : "",
                    vl_comissao_rematricula: hasValue(dijit.byId('valorRmtNovaComissao').get('value')) ? dijit.byId('valorRmtNovaComissao').get('value') : "",
                    pc_comissao_rematricula: hasValue(dijit.byId('percentualRmtNovaComissao').get('value')) ? dijit.byId('percentualRmtNovaComissao').get('value') : ""
                });

            grid.setStore(new ObjectStore({ objectStore: new Memory({ data: grid.store.objectStore.data }) }));
            dijit.byId("dialogComiss").hide();
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirEditorComissao(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para editar.', null);
        else if(!hasValue(itensSelecionados) || itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_AVISO, 'Selecione apenas um registro para alterar.', null);
        else
        {
            document.getElementById("divIncluirComissao").style.display = "none";
            document.getElementById("divAlterarComissao").style.display = "";

            var grid = dijit.byId("gridComissao");
            var index = grid.selection.selectedIndex;
            var item = grid.getItem(index);

            if(!hasValue(dijit.byId('produtoViewNovaComissao').store.data)){
                setTimeout(function(){ dijit.byId('produtoViewNovaComissao').set('value', item.cd_produto); }, 1000);
            }else
                dijit.byId('produtoViewNovaComissao').set('value', item.cd_produto);

            dijit.byId('valorMatNovaComissao').set('value', item.vl_comissao_matricula);
            dijit.byId('percentualMatNovaComissao').set('value', item.pc_comissao_matricula);
            dijit.byId('valorRmtNovaComissao').set('value', item.vl_comissao_rematricula);
            dijit.byId('percentualRmtNovaComissao').set('value', item.pc_comissao_rematricula);

            dijit.byId("dialogComiss").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function editarComissao() {
    if (!dijit.byId("formComissao").validate()) {
        return false;
    }

    var grid = dijit.byId("gridComissao");
    var index = grid.selection.selectedIndex;
    var item = grid.getItem(index);
    var store = grid.store;

    if (grid.store.objectStore.data.some(x => x.cd_produto == dijit.byId('produtoViewNovaComissao').item.id && x.id != item.id)) {
        caixaDialogo(DIALOGO_AVISO, 'Produto já cadastrado.', null);
        return false;
    }

    store.setValue(item, 'id', item.id);
    store.setValue(item, 'no_produto', dijit.byId('produtoViewNovaComissao').item.name);
    store.setValue(item, 'cd_produto', dijit.byId('produtoViewNovaComissao').item.id);
    store.setValue(item, 'vl_comissao_matricula', hasValue(dijit.byId('valorMatNovaComissao').get('value')) ? dijit.byId('valorMatNovaComissao').get('value') : "");
    store.setValue(item, 'pc_comissao_matricula', hasValue(dijit.byId('percentualMatNovaComissao').get('value')) ? dijit.byId('percentualMatNovaComissao').get('value') : "");
    store.setValue(item, 'vl_comissao_rematricula', hasValue(dijit.byId('valorRmtNovaComissao').get('value')) ? dijit.byId('valorRmtNovaComissao').get('value') : "");
    store.setValue(item, 'pc_comissao_rematricula', hasValue(dijit.byId('percentualRmtNovaComissao').get('value')) ? dijit.byId('percentualRmtNovaComissao').get('value') : "");

    grid.update();
    grid.itensSelecionados = [];
    dijit.byId("dialogComiss").hide();
}

function geradorIdComissao(gridComissao) {
    try {
        var id = gridComissao.store.objectStore.data.length;
        var itensArray = gridComissao.store.objectStore.data.sort(function byOrdem(a, b) { return a.id - b.id; });
        if (id == 0)
            id = 1;
        else if (id > 0)
            id = itensArray[id - 1].id + 1;
        return id;
    }
    catch (e) {
        postGerarLog(e);
    }
}


//variavel utilizada para diferenciar se o check de todos foi desmarcado por click ou por ter itens selecionados 
var cbMultiHabEstagioTodos = false;

function getValues() {
    
    selected = dijit.byId("cbMultiHabEstagio").get('value');

    //se o item todos foi selecionado
    if (selected.indexOf(selected, "-1") == -1 && !cbMultiHabEstagioTodos && dijit.byId("cbMultiHabEstagio").options[0].selected == true ) {
        console.log('clicou');
        for (var i=0;i<dijit.byId("cbMultiHabEstagio").options.length;i++) {
            dijit.byId("cbMultiHabEstagio").options[i].selected = true;       
        }
        cbMultiHabEstagioTodos = true; //marcou o check de todos          
    } 

    //conta a quantidade de itens selecionados
    var contSelected = 0;
    for (var i=0;i<dijit.byId("cbMultiHabEstagio").options.length;i++) {
        if(dijit.byId("cbMultiHabEstagio").options[i].selected == true) {
            contSelected ++;
        }       
    }
  
    //se já clicou em todos mas nao esta selecionado(desmarca todos)
    if ( cbMultiHabEstagioTodos && dijit.byId("cbMultiHabEstagio").options[0].selected == false ) {
        
        for (var i=0;i<dijit.byId("cbMultiHabEstagio").options.length;i++) {
            dijit.byId("cbMultiHabEstagio").options[i].selected = false;
        }
        cbMultiHabEstagioTodos = false;//desmarcou o check de todos(click) 
           
    } 
   
    //se ja clicou em todos mas desmarcou algum (desmarca o check todos)
    if(dijit.byId("cbMultiHabEstagio").options.length != contSelected && cbMultiHabEstagioTodos == true) {
        dijit.byId("cbMultiHabEstagio").options[0].selected = false;
        cbMultiHabEstagioTodos = false;//desmarcou o check de todos(tem itens selecionados ) 
    }
          
}


function loadProdutoAssinatura() {
    dojo.xhr.get({
        url: Endereco() + "/api/coordenacao/getAllProduto",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dataProduto) {
        try {
            (hasValue(jQuery.parseJSON(dataProduto).retorno)) ? dataProduto = jQuery.parseJSON(dataProduto).retorno : jQuery.parseJSON(dataProduto);

            setProdutoAssinatura(dataProduto, 'cbProdutoAssinatura');
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function setProdutoAssinatura(produtos, field) {
    require(["dojo/store/Memory", "dojo/_base/array"],
        function(Memory, Array) {
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
                var storeProduto = new dojo.data.ItemFileReadStore({
                    data: {
                        identifier: "id",
                        label: "name",
                        items: dados
                    }
                });
                if (produtos.length > 0) {


                    w.setStore(storeProduto, []);
                    setMenssageMultiSelect(PRODUTO, field);
                    dijit.byId(field).on("change",
                        function(e) {
                            setMenssageMultiSelect(PRODUTO, field, false, 20);
                        });
                };
            } catch (e) {

            }
        });
}

function toglleCordenador(value) {
    for (var i = 0; i < dijit.byId("cbProdutoAssinatura").options.length; i++) {
        dijit.byId("cbProdutoAssinatura").options[i].selected = false;
        dijit.byId("cbProdutoAssinatura").onChange(true);
    }
    dijit.byId("cbProdutoAssinatura").reset();
    if (value == true) {
        dijit.byId('cbProdutoAssinatura').set('disabled', false);
    } else {
        dijit.byId('cbProdutoAssinatura').set('disabled', true);
    }
}

function setoptions(item, option) {
    option.selected = false;
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


function montarparametrosmulti(componente) {
    var produtosFuncionario = "";
    if (!hasValue(dijit.byId(componente).value) || dijit.byId(componente).value.length <= 0) {
        produtosFuncionario = "100";
    } else {

        if (dijit.byId(componente).value[0] == "0") {
            produtosFuncionario = "0";
        } else {
            for (var i = 0; i < dijit.byId(componente).value.length; i++) {
                if (produtosFuncionario == "") {
                    produtosFuncionario = dijit.byId(componente).value[i];
                } else {
                    produtosFuncionario = produtosFuncionario + "|" + dijit.byId(componente).value[i];
                }
            }
        }
    }

    return produtosFuncionario;
}