var RELACENTREPESSOAS = 2, CADASTROALUNO = 1, EDITARLUNO = 2, MATRICULADO = 2;
var DOMINGO = 1; var SEGUNDA = 2; var TERCA = 3; var QUARTA = 4; var QUINTA = 5; var SEXTA = 6; var SABADO = 7;
var abilHabilidadeProf = null;
var TIPO_TELEFONE_TELEFONE = 1;
var TIPO_TELEFONE_CELULAR = 3;
var TIPO_TELEFONE_EMAIL = 4;
var SITUACAO_AGUARDANDO = 9, ATIVO = 1, REMATRICULA = 8;
var TODOS = 0;
var idPanelMaricula;

var SITUACAO = 3;

var EnumStatusOrgaoFinanceiro =
{
    ATIVO: 1,
    INATIVO: 0
}

function mascarar() {
    require([
           "dojo/ready"
    ], function (ready) {
        ready(function () {
            try {
                maskHour('#timeIni');
                maskHour('#timeFim');
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function destroyCreateGridHorario() {
    if (hasValue(dijit.byId("gridHorarios"))) {
        dijit.byId("gridHorarios").destroyRecursive();
        $('<div>').attr('id', 'gridHorarios').appendTo('#paiGridHorarios');
    }
}

function destroyCreateGridFollowUp() {
    if (hasValue(dijit.byId("gridFollowUp"))) {
        dijit.byId("gridFollowUp").destroyRecursive();
        $('<div>').attr('id', 'gridFollowUp').appendTo('#paiGridFollowUp');
    }
}

function destroyCreateGridTabMatricula() {
    if (hasValue(dijit.byId("gridTabMatriculas"))) {
        dijit.byId("gridTabMatriculas").destroyRecursive();
        $('<div>').attr('id', 'gridTabMatriculas').appendTo('#idPanelMaricula');
    }
}

function populaDiasSemana(ready, registry, ItemFileReadStore) {
    ready(function () {
        var w = registry.byId("cbDias");
        var retorno = [];
        retorno.push({ value_dia: 0 + "", no_dia: getDiaSemana(0) });
        retorno.push({ value_dia: 1 + "", no_dia: getDiaSemana(1) });
        retorno.push({ value_dia: 2 + "", no_dia: getDiaSemana(2) });
        retorno.push({ value_dia: 3 + "", no_dia: getDiaSemana(3) });
        retorno.push({ value_dia: 4 + "", no_dia: getDiaSemana(4) });
        retorno.push({ value_dia: 5 + "", no_dia: getDiaSemana(5) });
        retorno.push({ value_dia: 6 + "", no_dia: getDiaSemana(6) });

        var store = new ItemFileReadStore({
            data: {
                identifier: "value_dia",
                label: "no_dia",
                items: retorno
            }
        });
        w.setStore(store, []);
        console.log(w.get("value_dia"));
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridAluno';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);

            value = value || indice != null; // Item está selecionado.
        }

        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionaAluno', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'selecionaAluno', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarLegenda() {
    try {
        var chart = new dojox.charting.Chart("chart");
        chart.setTheme(dojox.charting.themes.MiamiNice);
        chart.addAxis("x", { min: 0 });
        chart.addAxis("y", { vertical: true, fixLower: 0, fixUpper: 100 });
        chart.addPlot("default", { type: "Columns", gap: 5 });
        chart.render();

        var legend = new dojox.charting.widget.Legend({ chart: chart, horizontal: true }, "legend");

        chart.addSeries("Pretendido", [1], { fill: "#6ec2fd" }); //BlueRow
        chart.addSeries("Matriculado", [1], { fill: "#fc0000" }); //RedRow
        chart.addSeries("Aguardando Matrícula", [1], { fill: "#e6ad00" }); //RedRow
        chart.render();
        dijit.byId("legend").refresh();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaGridAluno(permissoes) {
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
       "dojo/query",
       "dojo/ready",
       "dijit/form/DropDownButton",
       "dijit/DropDownMenu",
       "dijit/MenuItem",
       "dojo/on",
       "dijit/form/FilteringSelect",
       "dijit/form/Button"
    ], function (xhr, dom, registry, ItemFileReadStore, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, ready, DropDownButton, DropDownMenu, MenuItem, on, FilteringSelect, Button) {
        ready(function () {
            try {

                getParametroEscolaInternacional();
                inserirIdTabsAlunoRaf();
                alterarVisibilidadeTabAlunoRaf(false);

                if (hasValue(permissoes))
                    document.getElementById("setValuePermissoes").value = permissoes;
                document.getElementById("trNomReduzido").style.display = "none";
                //  document.getElementById("trDocumentos").style.display = "none";
                document.getElementById("tdDtaEmissaoRG").style.display = "none";
                document.getElementById("tdDtaEmissaoRGLabel").style.display = "none";
                document.getElementById("trCarteiras").style.display = "none";
                document.getElementById("trTituloINSS").style.display = "none";
                document.getElementById("tdDataCasamento").style.display = "none";
                document.getElementById("tdDescDataCasamento").style.display = "none";
                document.getElementById("lblTratamento").style.display = "none";
                document.getElementById("compTratamento").style.display = "none";
                //document.getElementById("tdLocalNascimento").style.display = "none";
                document.getElementById("trdDatas").style.display = "none";
                populaDiasSemana(ready, registry, ItemFileReadStore);
                document.getElementById("panelObservacao").style.display = "none";
                document.getElementById("divPesEndResp").style.display = "";
                dijit.byId("atividadePrincipal").set("required", true);
                dijit.byId("dtaNasci").set("required", true);
                dijit.byId("celular").set("required", true);
                dojo.byId("descApresMsg").value = 'apresentadorMensagemAluno';
                dijit.byId("panelEndereco").set("open", false);
                dijit.byId("tabContainer").resize()
                toggleDisabled("dtaCadastro", false);
                dojo.byId("lblAtivo").innerHTML = "Ativo:";
                
                mascarar();
                setMenssageMultiSelect(DIA, 'cbDias');
                setMenssageMultiSelectOpcao(SITUACAO, 'situacao', true, 0);
                dijit.byId("situacao").on("change", function (e) {
                    setMenssageMultiSelectOpcao(SITUACAO, 'situacao',true, 0);
                });

                montarSituacaoAlunoTurma(Memory, registry, ItemFileReadStore);
                dijit.byId("cbDias").on("change", function (e) {S
                    setMenssageMultiSelect(DIA, 'cbDias');
                });
                var storeTipoAluno = [
                    { name: "Alunos", id: "13" },
                    { name: "Clientes", id: "12" }
                ];
                criarOuCarregarCompFiltering("alunosClientes", storeTipoAluno, "", "13", dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name', MASCULINO);
                // dijit.byId("alunosClientes").set("value", 13);
                myStore = Cache(
                   JsonRest({
                       target: Endereco() + "/api/aluno/getAlunoSearch?nome=&email=&status=" + parseInt(1) + "&cnpjCpf=" + "&inicio=false&cdSituacoes=100&sexo=0&semTurma=false&movido=false&tipoAluno=0&matriculasem=false&matricula=false",
                       handleAs: "json",
                       headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                   }
               ), Memory({}));

                /* Formatar o valor em armazenado, de modo a serem exibidos.*/
                var gridAluno = new EnhancedGrid({
                    //store: ObjectStore({ objectStore: myStore }),
                    store: dataStore = ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure:
                        [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionaAluno", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                            { name: "Nome", field: "no_pessoa", width: "25%", styles: "min-width:80px;" },
                            { name: "CPF", field: "nm_cpf_dependente", width: "16%" },
                            { name: "E-Mail", field: "email", width: "15%", styles: "min-width:80px;" },
                            { name: "Telefone", field: "telefone", width: "10%", styles: "text-align: center; min-width:80px;" },
                            { name: "Data Cadastro", field: "dta_cadastro", width: "10%", styles: "text-align: center; min-width:80px;" },
                            { name: "Ativo", field: "id_ativo", width: "7%", styles: "text-align: center; min-width:50px; max-width: 50px;" }
                        ],
                    canSort: true,
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
                            plugins: { nestedSorting: true }
                        }
                    }
                }, "gridAluno");

                gridAluno.pagination.plugin._paginator.plugin.connect(gridAluno.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridAluno, 'cd_aluno', 'selecionaTodos');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAluno, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionaAluno', -1, 'selecionaTodos', 'selecionaTodos', 'gridAluno')", gridAluno.rowsPerPage * 3);
                    });
                });

                gridAluno.canSort = function (col) { return Math.abs(col) != 1; };
                gridAluno.startup();
                gridAluno.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                            item = this.getItem(idx),
                            store = this.store;
                        if (hasValue(item)) {
                            destroyCreateGridHorario();
                            destroyCreateGridFollowUp();
                            destroyCreateGridTabMatricula();
                            setarTabCadAluno();
                            gridAluno.itemSelecionado = montaObjetoAluno(item);
                            keepValues(item, gridAluno, null);
                            IncluirAlterar(0, 'divAlterarAluno', 'divIncluirAluno', 'divExcluirAluno', 'apresentadorMensagemAluno', 'divCancelarAluno', 'divLimparAluno');
                            document.getElementById("trNomReduzido").style.display = "none";
                            dijit.byId("cad").show();
                            document.getElementById('parametroPospectValido').value = 'false';
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                new Button({
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                        pesquisaAluno(true);
                    }
                }, "pesquisarAluno");
                decreaseBtn(document.getElementById("pesquisarAluno"), '32px');
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        try {
                            var situacoes = "";
                            if (!hasValue(dijit.byId('situacao').value) || dijit.byId('situacao').value.length <= 0)
                                situacoes = "100";
                            else
                                for (var i = 0; i < dijit.byId('situacao').value.length; i++) {
                                    if (situacoes == "")
                                        situacoes = dijit.byId('situacao').value[i];
                                    else
                                        situacoes = situacoes + "|" + dijit.byId('situacao').value[i];
                                }
                            dojo.xhr.get({
                                url: Endereco() + "/api/aluno/GetUrlRelatorioAluno?" + getStrGridParameters('gridAluno') + "nome=" + dojo.byId("_nome").value + "&email=" + dojo.byId("emailPesq").value +
                                     "&status=" + dijit.byId("statusAluno").value + "&cnpjCpf=" + dojo.byId("_cnpjCpf").value + "&inicio=" + document.getElementById("inicioAluno").checked +
                                     "&cdSituacoes=" + situacoes + "&sexo=" + dijit.byId("nm_sexo").value + "&semTurma=" + dijit.byId("ckSemTurma").checked +
                                     "&movido=" + dijit.byId("ckMovidoSemContrato").checked + "&tipoAluno=" + dijit.byId("alunosClientes").value +
                                     "&matriculasem=" + dijit.byId("ckMatriculasemTurma").checked,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1000px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemAluno', error);
                            });
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorioAluno");

                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                        abrirCadAluno(TIPO_RETORNO_LINK_INCLUSAO, null, null, ObjectStore, Memory, on);
                        gridAluno.itemSelecionado = null;
                        dijit.byId('btProspect').setAttribute('disabled', false);
                        dijit.byId('excluirFoto').setAttribute('disabled', 1);
                        document.getElementById('parametroPospectValido').value = 'false';
                        alterarVisibilidadeTabAlunoRaf(false);
                    }
                }, "novoAluno");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        if (hasValue(dijit.byId("naturezaPessoa")) && hasValue(dijit.byId("naturezaPessoa").value))
                            salvarAluno();
                    }
                }, "incluirAluno");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try {
                            destroyCreateGridHorario();
                            destroyCreateGridFollowUp();
                            destroyCreateGridTabMatricula();
                            keepValues(null, gridAluno, false);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarAluno");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cad").hide(); } }, "fecharAluno");
                new Button({
                    label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () {
                        alterarAluno();
                    }
                }, "alterarAluno");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarAlunos() });
                    }
                }, "deleteAluno");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () {
                        setarTabCadAluno();
                        limparAluno();
                        document.getElementById('parametroPospectValido').value = 'false';
                    }
                }, "limparAluno");

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
                    label: "", iconClass: 'dijitEditorIcon dijitEditorIconEmail',
                    onClick: function () { enviarEmail("email"); }
                }, "enviarEmailAluno");

                var emailAluno = document.getElementById('enviarEmailAluno');
                if (hasValue(emailAluno)) {
                    emailAluno.parentNode.style.minWidth = '18px';
                    emailAluno.parentNode.style.width = '18px';
                }


                new Button({
                    label: "Enviar", iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        if (dojo.byId("email") != null &&
                            dojo.byId("email") != undefined &&
                            !hasValue(dojo.byId("email").value)) {
                            caixaDialogo(DIALOGO_ERRO, "Não foi possível carregar o email deste aluno para realizar o envio. Tente novamente o contate o adminstrador do sistema", null);
                        } else if (dojo.byId("nmRaf") != null &&
                            dojo.byId("nmRaf") != undefined &&
                            !hasValue(dijit.byId("nmRaf").value)) {
                            caixaDialogo(DIALOGO_ERRO, "Raf não encontrado para este aluno.", null);
                        } else {
                            var emailRafAluno = dojo.byId("email").value;
                            var nmRaf = dijit.byId("nmRaf").value;
                            enviarEmailRaf(emailRafAluno, nmRaf);
                        }
                        

                    }
                }, "enviarEmailRafAluno");
                // Botões de sub cadastro:
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function (e) {
                        montarDialog("dialogRua", e);
                    }
                }, "cadmBolsa");
                var cadmBolsa = document.getElementById('cadmBolsa');
                if (hasValue(cadmBolsa)) {
                    cadmBolsa.parentNode.style.minWidth = '18px';
                    cadmBolsa.parentNode.style.width = '18px';
                }
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function (e) {
                        montarDialog("dialogRua", e);
                    }
                }, "cadcBolsa");
                var cadcBolsa = document.getElementById('cadcBolsa');
                if (hasValue(cadcBolsa)) {
                    cadcBolsa.parentNode.style.minWidth = '18px';
                    cadcBolsa.parentNode.style.width = '18px';
                }
                loadNaturezaPessoaPesquisa();
                montarStatus("statusAluno");
                dojo.byId("_cnpjCpf").value = "";
                dojo.byId("_cnpjCpf").readOnly = true;

                //Metodos para criação do link
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditar(gridAluno.itensSelecionados); }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () { eventoRemoverAluno(gridAluno.itensSelecionados); }
                });
                menu.addChild(acaoRemover);

                var acaoMatricular = new MenuItem({
                    label: "Matricular",
                    onClick: function () {
                        abrirMatricula(xhr, Memory, FilteringSelect, registry, Array, ready, ObjectStore, gridAluno);
                    }
                });
                menu.addChild(acaoMatricular);

                var acaoHistorico = new MenuItem({
                    label: "Histórico do Aluno",
                    onClick: function () {
                        abrirHistoricoAluno(gridAluno);
                    }
                });
                menu.addChild(acaoHistorico);

                var acaoPortal = new MenuItem({
                    label: "Portal do Aluno",
                    onClick: function () {
                        abrirPortalAluno(gridAluno.itensSelecionados);
                    }
                });
                menu.addChild(acaoPortal);

                var acaoBaixaFinanceira = new MenuItem({
	                label: "Baixa Financeira",
	                onClick: function () {
		                abrirBaixaFinanceira(gridAluno);
	                }
                });
                menu.addChild(acaoBaixaFinanceira);

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
                        buscarTodosItens(gridAluno, 'todosItens', ['pesquisarAluno', 'relatorioAluno']);
                        pesquisaAluno(false);
                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridAluno', 'selecionaAluno', 'cd_aluno', 'selecionaTodos', ['pesquisarAluno', 'relatorioAluno'], 'todosItens'); }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dojo.byId("linkSelecionados").appendChild(button.domNode);

                //link motivo matricula
                var menuMsim = new DropDownMenu({ style: "height: 25px" });

                var acaoEditarMsim = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_motivo_matricula', dijit.byId("gridMotivoSim")); }
                });
                menuMsim.addChild(acaoEditarMsim);

                var buttonMsim = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasMsim",
                    dropDown: menuMsim,
                    id: "acoesRelacionadasMsim"
                });
                dom.byId("linkAcoesMotivoSim").appendChild(buttonMsim.domNode);

                //link Aluno Restrição
                var menuAlunoRestricao = new DropDownMenu({ style: "height: 25px" });


                var acaoEditarAlunoRestricao = new MenuItem({
	                label: "Editar",
                    onClick: function () { eventoEditarAlunoRestricao(dijit.byId("gridAlunoRestricao").itensSelecionados); }
                });
                menuAlunoRestricao.addChild(acaoEditarAlunoRestricao);

                var acaoEditarAlunoRestricao = new MenuItem({
	                label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'id_grid_aluno_restricao', dijit.byId("gridAlunoRestricao")); }
                });
                menuAlunoRestricao.addChild(acaoEditarAlunoRestricao);

                var buttonAlunoRestricao = new DropDownButton({
	                label: "Ações Relacionadas",
	                name: "acoesRelacionadasAlunoRestricao",
                    dropDown: menuAlunoRestricao,
	                id: "acoesRelacionadasAlunoRestricao"
                });

                dom.byId("linkAcoesAlunoRestricao").appendChild(buttonAlunoRestricao.domNode);

                //link FollowUp
                var menuFollow = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarFollowUp(); }
                });
                menuFollow.addChild(acaoEditar);

                var acaoExcluirFollow = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        deletarItemSelecionadoGrid(Memory, ObjectStore, 'nroOrdem', dijit.byId("gridFollowUp"));
                    }
                });
                menuFollow.addChild(acaoExcluirFollow);

                var buttonFollow = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasFollow",
                    dropDown: menuFollow,
                    id: "acoesRelacionadasFollow"
                });
                dom.byId("linkAcoesFollow").appendChild(buttonFollow.domNode);

                // Link matriculas
                var menTabMariculas = new DropDownMenu({ style: "height: 25px" });
                var acaoEditarMatriculasTag = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarTabMatricula(xhr, ready, Memory, FilteringSelect, registry, Array, ObjectStore); }
                });
                menTabMariculas.addChild(acaoEditarMatriculasTag);

                var buttonFollow = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasTabMaricula",
                    dropDown: menTabMariculas,
                    id: "acoesRelacionadasTabMatrciula"
                });
                dom.byId("linkAcoesMatricula").appendChild(buttonFollow.domNode);

                //Monta grid sub cadastros:
                montaGridSubCadastros(EnhancedGrid, ObjectStore, Memory);

                //Monta Grid AlunoRestricao
                montaGridSubCadastroAlunoRestricao(EnhancedGrid, ObjectStore, Memory);


                loadPesqSexo(Memory, dijit.byId("nm_sexo"));

                loadFormaPag(Memory, FilteringSelect);

                //if (hasValue(dijit.byId("cpf")))
                //    dijit.byId("cpf").on("blur", function (evt) {
                //        try {
                //            if (trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__" && dojo.byId("cpf").value != dojo.byId("cpf").oldValue)
                //                if (validarCPF("#cpf", "apresentadorMensagemAluno")) {
                //                    //cleanUsarCpf();
                //                    extistsPessoAlunoByCpf();
                //                }
                //        }
                //        catch (e) {
                //            postGerarLog(e);
                //        }
                //    });
                dijit.byId("cpf").on("change", function (evt) {
                    if (hasValue(evt) && evt != dojo.byId("cpf").oldValue)
                        dojo.byId("cpf").oldValue = evt;
                    try {
                        if (trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__" )
                            if (validarCPF("#cpf", "apresentadorMensagemAluno")) {
                                extistsPessoAlunoByCpf();
                            }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }

                });
                dijit.byId("email").on("change", function (evt) {
                        try {
                            if (hasValue(dijit.byId("email") && trim(dojo.byId("email").value) != ""))
                                if (!hasValue(dojo.byId("nomPessoa").value) && (!hasValue(dojo.byId('cdPessoaCpf').value) || !dojo.byId('cdPessoaCpf').value == 0))
                                    existsEmailPessoa(dojo.byId("email").value, "", 0);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }

                });
                dijit.byId("nomPessoa").on("change", function (evt) {
                        try {
                            if (trim(dojo.byId("nomPessoa").value) != "" &&
	                            trim(dojo.byId("nomPessoa").value).indexOf("*") > -1) {
	                            var mensagensWeb = new Array();
	                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido o caracter (*) no campo nome.");
                                apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
                                dijit.byId("nomPessoa").reset();
                                return;
                            } else {
                                apresentaMensagem('apresentadorMensagemAluno', null);
                            }

                            if (hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0" && trim(dojo.byId("cpf").value) != "" && dojo.byId("cpf").value != "___.___.___-__")
                                return true;
                                
                            if (trim(dojo.byId("nomPessoa").value) != "")
                                if ((!hasValue(dojo.byId("cd_pessoa").value) || !dojo.byId('cd_pessoa').value == 0) &&
                                    hasValue(parseInt(dojo.byId('cdPessoaCpf').value)) &&
                                    hasValue(dojo.byId("nomPessoa").value))
                                    getExistsCpfENome("", dojo.byId("nomPessoa").value, dojo.byId('cdPessoaCpf').value);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }

                });
                //if (hasValue(dijit.byId("email")))
                //    dijit.byId("email").on("blur", function (evt) {
                //        try {
                //            if (trim(dojo.byId("email").value) != "")
                //                if (!hasValue(dojo.byId("nomPessoa").value) && (!hasValue(dojo.byId('cdPessoaCpf').value) || !dojo.byId('cdPessoaCpf').value == 0))
                //                    existsEmailPessoa(dojo.byId("email").value, "", 0);
                //        }
                //        catch (e) {
                //            postGerarLog(e);
                //        }
                //    });

                //if (hasValue(dijit.byId("nomPessoa")))
                //    dijit.byId("nomPessoa").on("blur", function (evt) {
                //        try {
                //            if (trim(dojo.byId("nomPessoa").value) != "" &&
	               //             trim(dojo.byId("nomPessoa").value).indexOf("*") > -1) {
	               //             var mensagensWeb = new Array();
	               //             mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "Não é permitido o caracter (*) no campo nome.");
                //                apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
                //                dijit.byId("nomPessoa").reset();
                //                return;
                //            } else {
                //                apresentaMensagem('apresentadorMensagemAluno', null);
                //            }

                //            if (trim(dojo.byId("nomPessoa").value) != "")
                //                if ((!hasValue(dojo.byId("cd_pessoa").value) || !dojo.byId('cd_pessoa').value == 0) &&
                //                    hasValue(parseInt(dojo.byId('cdPessoaCpf').value)) && 
                //                    hasValue(dojo.byId("nomPessoa").value))
                //                    getExistsCpfENome("", dojo.byId("nomPessoa").value, dojo.byId('cdPessoaCpf').value);
                //        }
                //        catch (e) {
                //            postGerarLog(e);
                //        }
                //    });

                $("#dtaAdmissao").mask("99/99/9999");
                $("#dtaDemissao").mask("99/99/9999");
                maskHour("#timeIni");
                maskHour("#timeFim");
                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {
                        limparPesquisaMtvMatFK();
                        dijit.byId("proMotivoMatricula").show();
                    }
                }, "incluirMotivoMatricula");

                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
                        incluirFollowUp(Memory, ObjectStore);
                    }
                }, "btnNovoFollow");

                //Incluir AlunoRestricao
                new Button({
	                label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconInsert', onClick: function () {

		                populaOrgaoFinanceiroEUsuarioAtendente(null, EnumStatusOrgaoFinanceiro.ATIVO);

	                }
                }, "incluirAlunoRestricao");


                dijit.byId("proMotivoMatricula").on("Show", function (e) {
                    dijit.byId("gridMotivoMatricluaFK").update();
                });

                dijit.byId("tagMotivosSim").on("Show", function (e) {
                    dijit.byId("gridMotivoSim").update();
                });

                dijit.byId("tagAlunoRestricao").on("Show", function (e) {
                    dijit.byId("gridAlunoRestricao").update();
                });

                dijit.byId("alunosClientes").on("change", function (tipoA) {
                    try {
                        if (tipoA != 13) {
                            dijit.byId('situacao').setStore(dijit.byId('situacao').store, []);
                        } else {
                            dijit.byId('situacao').setStore(dijit.byId('situacao').store, [1, 8]);
                        }
                        if (!hasValue(tipoA) || tipoA < TODOS)
                            dijit.byId("alunosClientes").set("value", TODOS);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                });
                
                // Realiza o tratamento do link proveniente de prospects:
                var parametros = getParamterosURL();
                if (hasValue(parametros['cd_prospect']))
                    configuraDadosProspect(parametros['cd_prospect'], ObjectStore, Memory, null);
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323042', '765px', '771px');
                        });
                }
                montarLegenda();
                adicionarAtalhoPesquisa(['_nome', 'emailPesq', 'statusAluno', 'situacao', '_cnpjCpf', 'nm_sexo'], 'pesquisarAluno', ready);

                //Montando prospect dinamicamente para tela de aluno:
                showP('trProspect', true);
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIcon dijitEditorIconFindSGF',
                    onClick: function () {
                        try {
                            if (!hasValue(dijit.byId("fecharProspectFK"))) {
                                montarGridPesquisaProspect(true, function () {
                                    abrirFKProspectOrigemAluno();
                                    dojo.query("#nomeProspectFK").on("keyup", function (e) {
                                        if (e.keyCode == 13) pesquisarProspectFK();
                                    });
                                    dijit.byId("fecharProspectFK").on("click", function (e) {
                                        dijit.byId("cadProspectFollowUpFK").hide();
                                    });
                                }, true);
                            }
                            else
                                abrirFKProspectOrigemAluno();
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "btProspect");
                decreaseBtn(document.getElementById("btProspect"), '18px');
                //Metodos tab matrciulas
                //habilitando a pesquisa de pessoa para usar o seu CPF.
                dijit.byId("pesPessoaFK").set("disabled", true);
                dojo.byId("tdPessoaFK").style.display = "";


                // Criação de botões de persistência Aluno Restricao
                new Button({
	                label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
	                onClick: function () {
		                incluirAlunoRestricao();
	                }
                }, "adicionarAlunoRestricao");
                new Button({
	                label: "Alterar", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
	                onClick: function () {
		                AlterarAlunoRestricao();
	                }
                }, "alterarAlunoRestricao");
                new Button({
	                label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete',
	                onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() {

	                        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");

	                        var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
	                        if (hasValue(data) && hasValue(dojo.byId("id_grid_aluno_restricao").value)) {

		                        var posicaoItemDeletar = data.map(function (item, index, array) {
			                        return item.id_grid_aluno_restricao + "";
		                        }).indexOf(dojo.byId("id_grid_aluno_restricao").value);

		                        if (posicaoItemDeletar >= 0) {
                                    removeObjSort(data, "id_grid_aluno_restricao", data[posicaoItemDeletar].id_grid_aluno_restricao);

			                        gridAlunoRestricao.itensSelecionados = new Array();
			                        gridAlunoRestricao._refresh();

			                        dijit.byId("cadAlunoRestricao").hide();
		                        }

	                        }

	                        
			                DeletarAlunoRestricao();
		                });
	                }
                }, "deleteAlunoRestricao");
                new Button({
	                label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset",
	                onClick: function () {
		                limparFormAlunoRestricao(null, EnumStatusOrgaoFinanceiro.ATIVO);
	                }
                }, "limparAlunoRestricao");
                new Button({
	                label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
	                onClick: function () {
		                var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");

                        var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
                        if (hasValue(data) && hasValue(dojo.byId("id_grid_aluno_restricao").value)) {

	                        var posicaoItemCancelar = data.map(function (item, index, array) {
                                return item.id_grid_aluno_restricao + "";
                            }).indexOf(dojo.byId("id_grid_aluno_restricao").value);

                            if (posicaoItemCancelar >= 0) {
                                populaOrgaoFinanceiroEUsuarioAtendenteEdicao(data[posicaoItemCancelar].cd_orgao_financeiro, EnumStatusOrgaoFinanceiro.ATIVO, data[posicaoItemCancelar]);
	                        }
	                        
                        }
                        
	                }
                }, "cancelarAlunoRestricao");
                new Button({
	                label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
	                onClick: function () {
		                dijit.byId("cadAlunoRestricao").hide();
	                }
                }, "fecharAlunoRestricao");

                showCarregando();
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}


function incluirAlunoRestricao() {
    try {
        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");
        var valido = true;

        if (!validaOrgaoFinanceiro()) {
	        valido = false;
        } else if (!validaDataAlunoRestricao()) {
            valido = false;
        } else if (!validaDataAlunoRestricaoItemDuplicado()) {
	        valido = false;
        }
        else {
            apresentaMensagem('apresentadorMensagemAluno', null);
            //var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
            if (gridAlunoRestricao != null) {

	            var alunoRestricaoGrid = montarAlunoRestricaoInclusaoGrid();
                insertObjSort(gridAlunoRestricao.store.objectStore.data,
		            "id_grid_aluno_restricao", alunoRestricaoGrid);
		          
                gridAlunoRestricao.setStore(new dojo.data.ObjectStore({
                    objectStore: new dojo.store.Memory({ data: gridAlunoRestricao.store.objectStore.data })
                }));

                dijit.byId("cadAlunoRestricao").hide();
            }
        }
        
        if (!valido)
            return false;
    }
    catch (e) {
        postGerarLog(e);
    }
}



function AlterarAlunoRestricao() {
	try {
        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");

        var valido = true;
        if (!validaOrgaoFinanceiro()) {
	        valido = false;
        } else if (!validaDataAlunoRestricao()) {
	        valido = false;
        } else if (!validaDataAlunoRestricaoItemDuplicadoEdicao()) {
	        valido = false;
        }
        else {
	        apresentaMensagem('apresentadorMensagemAluno', null);
	        var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
	        if (data != null && data.length > 0 && hasValue(dojo.byId("id_grid_aluno_restricao").value)) {
	            //Acha a posicao do item que vai editar
	            var posItemAlterar = data.map(function (item, index, array) {
	                return item.id_grid_aluno_restricao + "";
	            }).indexOf(dojo.byId("id_grid_aluno_restricao").value);

	            if (posItemAlterar >= 0) {
	                var itemAlterar = data[posItemAlterar];

	                if (hasValue(dojo.byId("dt_inicio_restricao").value)) {
	                    this.dataInicial = dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value,
	                        { formatLength: 'short', selector: 'date', locale: 'pt-br' });
	                    this.dataInicial = dojo.date.locale.format(this.dataInicial,
	                        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
	                } else {
	                    this.dataInicial = null;
	                }

	                if (hasValue(dojo.byId("dt_final_restricao").value)) {
	                    this.dataFinal = dojo.date.locale.parse(dojo.byId("dt_final_restricao").value,
	                        { formatLength: 'short', selector: 'date', locale: 'pt-br' });
	                    this.dataFinal = dojo.date.locale.format(this.dataFinal,
	                        { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
	                } else {
	                    this.dataFinal = null;
	                }



	                this.dataCadastro = dojo.date.locale.format(new Date(),
	                    { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

	               
					itemAlterar.cd_aluno = hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0" ? dojo.byId("cdAluno").value : 0;
		            itemAlterar.dc_orgao_financeiro = hasValue(dijit.byId("cbOrgaoFinanceiro").value) ? dijit.byId("cbOrgaoFinanceiro").item.name : 0;
		            itemAlterar.no_responsavel = hasValue(dojo.byId("no_atendente_usuario_restricao").value) ? dojo.byId("no_atendente_usuario_restricao").value : "";
					itemAlterar.cd_orgao_financeiro = hasValue(dijit.byId("cbOrgaoFinanceiro").value) ? dijit.byId("cbOrgaoFinanceiro").value : 0;
					itemAlterar.cd_usuario = hasValue(dojo.byId("cd_atendente_usuario_restricao").value) ? dojo.byId("cd_atendente_usuario_restricao").value : 0;
					itemAlterar.dt_inicio_restricao = hasValue(dojo.byId("dt_inicio_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
					itemAlterar.dt_final_restricao = hasValue(dojo.byId("dt_final_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_final_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
					itemAlterar.dt_cadastro = dojo.date.locale.format(new Date(), { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
					itemAlterar.dtaCadastro = dataCadastro;
					itemAlterar.dtaInicioRestricao = dataInicial;
	                itemAlterar.dtaFinalRestricao = dataFinal;

                    removeObjSort(data, "id_grid_aluno_restricao", data[posItemAlterar].id_grid_aluno_restricao);
	                insertObjSort(data, "id_grid_aluno_restricao", itemAlterar);

	                gridAlunoRestricao.itensSelecionados = new Array();
	                gridAlunoRestricao._refresh();

	                dijit.byId("cadAlunoRestricao").hide();
	            }
	        }
        }
        if (!valido)
	        return false;
      
	}
	catch (e) {
		postGerarLog(e);
	}
}


function DeletarAlunoRestricao() {
	try {
		
	} catch (e) {
		postGerarLog(e);
	} 
}



function validaDataAlunoRestricao() {
	try {
		var retorno = true;

		if (hasValue(dojo.byId("dt_inicio_restricao").value)) {
			this.dataInicial = dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value,
				{ formatLength: 'short', selector: 'date', locale: 'pt-br' });
			this.dataInicial = dojo.date.locale.format(this.dataInicial,
				{ selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
		} else {
			this.dataInicial = null;
		}

		if (hasValue(dojo.byId("dt_final_restricao").value)) {
			this.dataFinal = dojo.date.locale.parse(dojo.byId("dt_final_restricao").value,
				{ formatLength: 'short', selector: 'date', locale: 'pt-br' });
			this.dataFinal = dojo.date.locale.format(this.dataFinal,
				{ selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
		} else {
			this.dataFinal = null;
		}


		//Valida se a data inicial e maior que a final
		if (hasValue(this.dataInicial) && hasValue(this.dataFinal)) {
			if (!testaPeriodoAlunoRestricao(this.dataInicial, this.dataFinal)) {
				retorno = false;
			}
        }

        //Valida se a datainicial e datafinal estao vazias
        if (!hasValue(this.dataInicial) && !hasValue(this.dataFinal)) {
	        var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInclusaoESaidaVazia);
	        apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
	        retorno = false;
        }

        //Valida quando somente a data de saída é preenchida 
        if (!hasValue(this.dataInicial) && hasValue(this.dataFinal)) {
	        var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInclusaoVazia);
	        apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
	        retorno = false;
        }

		return retorno;
	} catch (e) {
		postGerarLog(e);
	} 

	
}


function validaOrgaoFinanceiro() {
	try {
		var retorno = true;

		//Valida se o orgão financeiro está vazio
        if (!hasValue(dijit.byId("cbOrgaoFinanceiro").value)) {
			var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroOrgaoFinanceiroVazio);
			apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
			retorno = false;
		}

		return retorno;
	} catch (e) {
		postGerarLog(e);
	}


}


function validaDataAlunoRestricaoItemDuplicado() {

	try {

        var retorno = true;
        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");

        var cdOrgaoFinanceiro = hasValue(dijit.byId("cbOrgaoFinanceiro").value) ? dijit.byId("cbOrgaoFinanceiro").value : 0;
        var dtInicioRestricao = hasValue(dojo.byId("dt_inicio_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dtFinalRestricao = hasValue(dojo.byId("dt_final_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_final_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
        if (data != null && data.length > 0) {
            //Verifica se já existe um registro com mesmo orgao e mesma data de inclusão(dataInclusao)) no grid
            var existeRegistroDataInclusao = data.some(function (item, index) {
                if (hasValue(item.dtaInicioRestricao) && hasValue(dtInicioRestricao)) {

                    var itemDtInicioRestricaoDate = createDate(item.dtaInicioRestricao);
	                return (item.cd_orgao_financeiro == cdOrgaoFinanceiro &&
                        (dojo.date.compare(itemDtInicioRestricaoDate, dtInicioRestricao) == 0));
                } else {
                    return false;
                }
            });

            if (existeRegistroDataInclusao) {
	            var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroRegistroDuplicadoGridAlunoRestricao);
	            apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
	            retorno = false;
            }

            //Verifica se já existe um registro com mesmo orgao e que não possui data de saída no grid
            var existeRegistroDataSaidaVazia = data.some(function (item, index) {
                return (item.cd_orgao_financeiro == cdOrgaoFinanceiro && (item.dt_final_restricao == null));
            });

            if (existeRegistroDataSaidaVazia) {
	            var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroRegistroSemDataSaida);
	            apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
	            retorno = false;
            }

           

           
            //pega as datas do orgao no grid
            var listaDatasOrgao = data.map((value) => {
	            if (value.cd_orgao_financeiro == cdOrgaoFinanceiro) {
		            return value.dtaFinalRestricao;
	            } else {
		            return "";
	            }
            });

            //tiras da lista os valores sem data
            if (hasValue(listaDatasOrgao) && dtInicioRestricao) {
	             var  listaDataOrgaoComData = listaDatasOrgao.filter(function(value) {
	                    if (hasValue(value)) {
		                    return value;
	                    }
                 });

                //Se tiver itens na lista
                if (hasValue(listaDataOrgaoComData)) {
		            //formata para Date as datas do grid
		            var listaDataFormatDate = listaDataOrgaoComData.map((value) => {
			            return dojo.date.locale.parse(value,
				            { formatLength: 'short', selector: 'date', locale: 'pt-br' });
		            });

		            //traz a maxima data
		            var maxData = listaDataFormatDate.reduce(function (a, b) {
			            return dojo.date.compare(a, b) > 0 ? a : b;
		            });


		            //formata a data inicial que vai inserir no formato dd/MM/yyyy
		            var dataInicialFormat = dojo.date.locale.format(dtInicioRestricao,
			            { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });


		            if (hasValue(dataInicialFormat) && hasValue(maxData)) {

			            var dtInicial = createDate(dataInicialFormat);

			            //Compara Se a maxima data de saida for maior que a data que está inserindo 
			            if (dojo.date.compare(maxData, dtInicial) > 0 || dojo.date.compare(maxData, dtInicial) == 0) {
				            //Formata a maxima data para a mensagem
				            var maxDataFormatada = dojo.date.locale.format(maxData,
					            { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

				            var mensagensWeb = new Array();
				            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, "(A data de inclusão deve ser maior que: " + maxDataFormatada + ").");
				            apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
				            retorno = false;
			            } else
				            apresentaMensagem('apresentadorMensagemAlunoRestricao', "");

		            }
	            }
	            
            }

        }


		return retorno;
	} catch (e) {
		postGerarLog(e);
	}


}


function validaDataAlunoRestricaoItemDuplicadoEdicao() {

    try {

        var retorno = true;
        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");

        var cdOrgaoFinanceiro = hasValue(dijit.byId("cbOrgaoFinanceiro").value) ? dijit.byId("cbOrgaoFinanceiro").value : 0;
        var dtInicioRestricao = hasValue(dojo.byId("dt_inicio_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dtFinalRestricao = hasValue(dojo.byId("dt_final_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_final_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
        if (data != null && data.length > 0) {
            //Verifica se já existe um registro com mesmo orgao e mesma data de inclusão(dataInclusao)) no grid (diferente do registro que está editando)
           /* var existeRegistroDataAlteracao = data.some(function (item, index) {
                return (item.cd_orgao_financeiro == cdOrgaoFinanceiro && (dojo.date.compare(item.dt_inicio_restricao, dtInicioRestricao) == 0) && item.id_grid_aluno_restricao != dojo.byId("id_grid_aluno_restricao").value);
            });

            if (existeRegistroDataAlteracao) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroRegistroDuplicadoGridAlunoRestricao);
                apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
                retorno = false;
            }*/

            //Verifica se já existe um registro com mesmo orgao que não possui data de saída no grid e diferente do registro que está editando
            /*var existeRegistroDataSaidaVazia = data.some(function (item, index) {
                return (item.cd_orgao_financeiro == cdOrgaoFinanceiro && (item.dt_final_restricao == null) && item.id_grid_aluno_restricao != dojo.byId("id_grid_aluno_restricao").value && dtFinalRestricao == null);
            });

            if (existeRegistroDataSaidaVazia) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroRegistroSemDataSaida);
                apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
                retorno = false;
            }*/


        }


        return retorno;
    } catch (e) {
        postGerarLog(e);
    }


}

function testaPeriodoAlunoRestricao(dataInicial, dataFinal) {
	try {
		var retorno = true;

		var dtInicial = createDate(dataInicial);
		var dtFinal = createDate(dataFinal);

		if (dojo.date.compare(dtInicial, dtFinal) > 0) {
			var mensagensWeb = new Array();
			mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
			apresentaMensagem('apresentadorMensagemAlunoRestricao', mensagensWeb);
			retorno = false;
		} else
            apresentaMensagem('apresentadorMensagemAlunoRestricao', "");
		return retorno;
	} catch (e) {
		postGerarLog(e);
	}
}

function createDate(stringDate) {
	var dateParts = stringDate.split("/");

	var dateObject = new Date(+dateParts[2], dateParts[1] - 1, +dateParts[0]);

	return dateObject;
}


function montarAlunoRestricaoInclusaoGrid() {
    try {

        if (hasValue(dojo.byId("dt_inicio_restricao").value)) {
            this.dataInicial = dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value,
			    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
		    this.dataInicial = dojo.date.locale.format(this.dataInicial,
			    { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
	    } else {
		    this.dataInicial = null;
	    }

        if (hasValue(dojo.byId("dt_final_restricao").value)) {
            this.dataFinal = dojo.date.locale.parse(dojo.byId("dt_final_restricao").value,
			    { formatLength: 'short', selector: 'date', locale: 'pt-br' });
		    this.dataFinal = dojo.date.locale.format(this.dataFinal,
			    { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });
	    } else {
		    this.dataFinal = null;
        }

        
       
        this.dataCadastro = dojo.date.locale.format(new Date(),
            { selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" });

        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");
        var idAlunoRestricao = geradorIdAlunoRestricao(gridAlunoRestricao);
        var alunoRestricao = {
            id_grid_aluno_restricao: idAlunoRestricao,
            cd_aluno_resticao: 0,
            cd_aluno: hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0" ? dojo.byId("cdAluno").value : 0,
            dc_orgao_financeiro: hasValue(dijit.byId("cbOrgaoFinanceiro").value) ? dijit.byId("cbOrgaoFinanceiro").item.name : 0,
            no_responsavel: hasValue(dojo.byId("no_atendente_usuario_restricao").value) ? dojo.byId("no_atendente_usuario_restricao").value : "",
            cd_orgao_financeiro: hasValue(dijit.byId("cbOrgaoFinanceiro").value) ? dijit.byId("cbOrgaoFinanceiro").value: 0,
            cd_usuario: hasValue(dojo.byId("cd_atendente_usuario_restricao").value) ? dojo.byId("cd_atendente_usuario_restricao").value : 0,
            dt_inicio_restricao: hasValue(dojo.byId("dt_inicio_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            dt_final_restricao: hasValue(dojo.byId("dt_final_restricao").value) ? dojo.date.locale.parse(dojo.byId("dt_final_restricao").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null, 
            dt_cadastro: dojo.date.locale.format(new Date(),{ selector: "date", datePattern: "dd/MM/yyyy", formatLength: "short", locale: "pt-br" }),
            dtaCadastro: dataCadastro,
            dtaInicioRestricao: dataInicial,
            dtaFinalRestricao: dataFinal
            
        }

        return alunoRestricao;

	} catch (e) {
		postGerarLog(e);
	} 
}

function geradorIdAlunoRestricao(gridAlunoRestricao) {
	try {
		var id = gridAlunoRestricao.store.objectStore.data.length;
		var itensArray = gridAlunoRestricao.store.objectStore.data.sort(function byOrdem(a, b) { return a.id - b.id; });
		if (id == 0)
			id = 1;
		else if (id > 0)
            id = itensArray[id - 1].id_grid_aluno_restricao + 1;
		return id;
	}
	catch (e) {
		postGerarLog(e);
	}
}

function abrirFKProspectOrigemAluno() {
    try {
        limparPesquisaProspectFK();
        dojo.byId("idOrigemProspectFK").value = ORIGCADALUNO;
        pesquisarProspectFK();

        //Remove o tipo da FK:
        dojo.byId("trTipoPesquisaFKProspect").style.display = "none";
        
        dijit.byId("cadProspectFollowUpFK").show();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarAlunoFK() {
    var gridAlunos = null;
    var valido = true;
    var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");

    if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
        caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        valido = false;
    } else if (gridPesquisaAluno.itensSelecionados.length > 1) {
        caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
        valido = false;
    }
    else {
        dojo.byId('nomeAlunoMat').value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;
        dojo.byId('cd_aluno_mat').value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
    }

    if (!valido)
        return false;
    dijit.byId("proAluno").hide();
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
                                     + '&no_pessoa=' + gridAluno.itensSelecionados[0].no_pessoa + '&dta_cadastro=' + gridAluno.itensSelecionados[0].dta_cadastro;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirBaixaFinanceira(gridAluno) {
	try {
        if (!hasValue(gridAluno.itensSelecionados) || gridAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else if (gridAluno.itensSelecionados.length > 1) {
	        caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
	        return false;
        } else {
	        if (navigator.vendor != null &&
		        navigator.vendor.match(/Apple Computer, Inc./) &&
		        navigator.userAgent.indexOf('Safari') != -1) {
		        // window.open(result.meeting.start_url,'_blank');
                window.location.assign(Endereco() + '/Financeiro/BaixaFinanceira?cd_aluno=' + gridAluno.itensSelecionados[0].cd_aluno + '&cd_pessoa=' + gridAluno.itensSelecionados[0].cd_pessoa_aluno
	                + '&no_pessoa=' + gridAluno.itensSelecionados[0].no_pessoa);
	        } else {
                window.open(Endereco() + '/Financeiro/BaixaFinanceira?cd_aluno=' + gridAluno.itensSelecionados[0].cd_aluno + '&cd_pessoa=' + gridAluno.itensSelecionados[0].cd_pessoa_aluno
	                + '&no_pessoa=' + gridAluno.itensSelecionados[0].no_pessoa, '_blank');
	        }
        }
	}
	catch (e) {
		postGerarLog(e);
	}
}

function retornarProspectFKOrigemAluno() { //selecionaProspectFK
    try {
        var valido = true;
        var gridPesquisaProspect = dijit.byId("gridPesquisaProspect");

        if (!hasValue(gridPesquisaProspect.itensSelecionados) || gridPesquisaProspect.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            valido = false;
        } else
            if (gridPesquisaProspect.itensSelecionados.length > 1) {
                caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmRegistro, null);
                valido = false;
            }
            else {
                if (hasValue(gridPesquisaProspect.itensSelecionados[0].cd_prospect)) {
                    dojo.byId('cd_prospect').value = gridPesquisaProspect.itensSelecionados[0].cd_prospect;
                    //Chama função para carregar os dados do prospect para um novo aluno.
                    configuraDadosProspect(gridPesquisaProspect.itensSelecionados[0].cd_prospect, dojo.data.ObjectStore, dojo.store.Memory, function () {
                        dijit.byId("limparProspect").set("disabled", false);
                    });
                }
            }
        if (!valido)
            return false;
        dijit.byId("cadProspectFollowUpFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirMatricula(xhr, Memory, FilteringSelect, registry, Array, ready, ObjectStore, gridAluno) {
    try {
        //showCarregando();
        if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
            var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
            montarCadastroMatriculaPartial(function () {
                dijit.byId("alterarMatricula").on("click", function () {
                    //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                    alterarMatricula();
                });
                abreNovaMatricula(xhr, Memory, Array, ObjectStore, gridAluno);
            }, Permissoes);
        }
        else
            abreNovaMatricula(xhr, Memory, Array, ObjectStore, gridAluno);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abreNovaMatricula(xhr, Memory,Array, ObjectStore, gridAluno) {
    try {
        if (!hasValue(gridAluno.itensSelecionados) || gridAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAluno, null);
            return false;
        }
        else if (gridAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneApenasUmAluno, null);
            return false;
        }
        else {
            //Verifica se o aluno está ativo e copia os seus dados:
            var alunoSelecionado = gridAluno.itensSelecionados[0];

            if (!alunoSelecionado.id_aluno_ativo) {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgAlunoInativo);
                apresentaMensagem('apresentadorMensagem', mensagensWeb);
                return false
            }
            xhr.get({
                url: Endereco() + "/api/turma/getTurmaAlunoAguard?cd_pessoa_aluno=" + alunoSelecionado.cd_aluno,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dados) {
                try {
                    dados = jQuery.parseJSON(dados);
                    var dadosAluno = {
                        cd_aluno: alunoSelecionado.cd_aluno,
                        nom_aluno: alunoSelecionado.no_pessoa,
                        cd_pessoa_aluno : alunoSelecionado.cd_pessoa_aluno,
                        cd_pessoa_responsavel: alunoSelecionado.cd_pessoa_dependente,
                        no_pessoa_responsavel : alunoSelecionado.no_pessoa_dependente,
                        pc_bolsa: alunoSelecionado.pc_bolsa,
                        pc_bolsa_material : alunoSelecionado.pc_bolsa_material,
                        dt_inicio_bolsa: alunoSelecionado.dt_inicio_bolsa,
                        vl_abatimento_matricula: alunoSelecionado.vl_abatimento_matricula
                    }
                    criaNovaMatricula(xhr, Memory, dadosAluno , 1, ObjectStore,
                        function () {
                            if (dados.retorno.cd_turma > 0) {
                                gridTurmaMat = dijit.byId("gridTurmaMat");

                                var turmas = [{
                                    cd_turma: dados.retorno.cd_turma,
                                    no_turma: dados.retorno.no_turma,
                                    cd_produto: dados.retorno.Produto.cd_produto,
                                    no_produto: dados.retorno.Produto.no_produto,
                                    no_professor: dados.retorno.no_professor,
                                    cd_situacao_aluno_turma: 9,
                                    situacaoAlunoTurma: msgAguardandoMat,
                                    cd_aluno: alunoSelecionado.cd_aluno,
                                    cd_pessoa_aluno: alunoSelecionado.cd_pessoa_aluno,
                                    dt_matricula: null,
                                    dt_movimento: null,
                                    dt_inicio: null,
                                    nm_matricula_turma: null,
                                    cd_pessoa_responsavel: alunoSelecionado.cd_pessoa_aluno,
                                    no_responsavel: alunoSelecionado.no_pessoa
                                }];
                                gridTurmaMat.setStore(new ObjectStore({ objectStore: new Memory({ data: turmas }) }));
                                populaSemTurma(dados.retorno.Produto.cd_produto, dados.retorno.cd_curso, dados.retorno.cd_duracao, dados.retorno.cd_regime, 0, xhr, Memory);
                                dojo.byId("tagSemTurma").style.display = "none";
                                dijit.byId("btnAddTurma").set("disabled", true);
                                dijit.byId("notasMaterialDidatico").set("disabled", false);
                                dijit.byId("notasMaterialDidatico").set("required", false);
                                dadosAlunoTurma(xhr, alunoSelecionado.cd_aluno, dados.retorno.cd_turma);
                            }
                            else {
                                dojo.byId("tagSemTurma").style.display = "";
                                dijit.byId("btnAddTurma").set("disabled", false);
                                dijit.byId("notasMaterialDidatico").set("disabled", false);
                                dijit.byId("notasMaterialDidatico").set("required", false);
                            }
                        });
                }
            catch (e) {
                postGerarLog(e);
            }

        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error.response.data);
        });
    }
}
    catch (e) {
        postGerarLog(e);
    }
}

function configuraDadosProspect(cd_prospect, ObjectStore, Memory, funcao) {
    require(["dojo/request/xhr", "dojox/json/ref", "dojo/ready"], function (xhr, ref, ready) {
        ready(function () {
            try {
                xhr.get(Endereco() + "/api/secretaria/getProspectFull?cd_prospect=" + cd_prospect, {
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json"
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        if (data.retorno != null && (data.retorno.Aluno == null || data.retorno.Aluno.length <= 0)) {
                            //Abre a tela de inserção de aluno:
                            abrirCadAluno(TIPO_RETORNO_LINK_INCLUSAO, function executaRetorno() { populaDadosProspect(data.retorno) }, data.retorno, ObjectStore, Memory);
                            gridAluno.itemSelecionado = null;
                            dijit.byId('excluirFoto').setAttribute('disabled', 1);
                        }
                        else {
                            if (data.retorno != null) {
                                destroyCreateGridHorario();
                                destroyCreateGridFollowUp();
                                destroyCreateGridTabMatricula();
                                setarTabCadAluno();
                                gridAluno.itemSelecionado = montaObjetoAluno(data.retorno);
                                keepValues(data.retorno.Aluno[0], dijit.byId("gridAluno"), false);
                                IncluirAlterar(0, 'divAlterarAluno', 'divIncluirAluno', 'divExcluirAluno', 'apresentadorMensagemAluno', 'divCancelarAluno', 'divLimparAluno');
                                document.getElementById("trNomReduzido").style.display = "none";
                                dijit.byId("cad").show();
                                document.getElementById('parametroPospectValido').value = 'false';
                            }
                        }

                        if (hasValue(funcao))
                            funcao.call();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, function (error) {
                    apresentaMensagem('apresentadorMensagem', error.response.data);
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function populaDadosProspect(prospect) {
    //Monta os dados da tela de inserção de aluno com os dados do prospect:
    dojo.byId('nomPessoa').value = prospect.PessoaFisica.no_pessoa;
    dojo.byId('no_prospect').value = prospect.PessoaFisica.no_pessoa;
    dijit.byId('sexo').set('value', prospect.PessoaFisica.nm_sexo); //codigo

    for (var i = 0; i < prospect.PessoaFisica.TelefonePessoa.length; i++)
        if (prospect.PessoaFisica.TelefonePessoa[i].id_telefone_principal && prospect.PessoaFisica.TelefonePessoa[i].cd_tipo_telefone == TIPO_TELEFONE_TELEFONE)
            dojo.byId('telefone').value = prospect.PessoaFisica.TelefonePessoa[i].dc_fone_mail;
        else if (prospect.PessoaFisica.TelefonePessoa[i].id_telefone_principal && prospect.PessoaFisica.TelefonePessoa[i].cd_tipo_telefone == TIPO_TELEFONE_CELULAR) {
            dojo.byId('celular').value = prospect.PessoaFisica.TelefonePessoa[i].dc_fone_mail;
            dijit.byId('operadora').set('value', prospect.PessoaFisica.TelefonePessoa[i].cd_operadora);
        }
        else if (prospect.PessoaFisica.TelefonePessoa[i].id_telefone_principal && prospect.PessoaFisica.TelefonePessoa[i].cd_tipo_telefone == TIPO_TELEFONE_EMAIL)
            dojo.byId('email').value = prospect.PessoaFisica.TelefonePessoa[i].dc_fone_mail;

    dijit.byId('cbMarketing').set('value', prospect.cd_midia);
    document.getElementById('cd_pessoa').value = prospect.cd_pessoa_fisica;
    document.getElementById('cdPessoaProject').value = prospect.cd_pessoa_fisica;
    document.getElementById('cd_prospect').value = prospect.cd_prospect;
}

function atualizaGrid(gridName) {
    dijit.byId(gridName).update();
}

function abrirCadAluno(tipoRetorno, funcaoRetorno, prospect, ObjectStore, Memory, on) {
    try {
        IncluirAlterar(tipoRetorno, 'divAlterarAluno', 'divIncluirAluno', 'divExcluirAluno', 'apresentadorMensagemAluno', 'divCancelarAluno', 'divLimparAluno');
        limparAluno();
        destroyCreateGridHorario();
        destroyCreateGridFollowUp();
        destroyCreateGridTabMatricula();
        setarTabCadAluno();
        //Popula componentes do aluno:
        populaMotivoBolsa(null, STATUS_ATIVO);
        populaMotivoCancBolsa(null, STATUS_ATIVO);
        if (hasValue(prospect)) {
            dojo.byId('no_prospect').value = prospect.PessoaFisica.no_pessoa;
            populaMidia(prospect.cd_midia, STATUS_TODOS);
        } else
            populaMidia(null, STATUS_TODOS);
        populaEscolaridade(null, STATUS_TODOS);

        var papelRelac = new Array();
        papelRelac[0] = RELACENTREPESSOAS;
        showPessoaFK(PESSOAFISICA, 0, papelRelac, function () {
            showCarregando();
            dojo.xhr.get({
                url: Endereco() + "/api/escola/getnomeusuario",
                handleAs: "json",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                var retorno = jQuery.parseJSON(data).retorno;
                dojo.byId("no_atendente").value = retorno;
                dijit.byId('naturezaPessoa').set("disabled", true);

                dijit.byId("cad").show();
                apresentaMensagem('apresentadorMensagem', null);

                //Abre a grade de follow up:
                var tabs = dijit.byId("tabContainer");
                var pane = dijit.byId("tabFollow");
                tabs.selectChild(pane);

                //Preenche os dados:
                var gridFollowUp = dijit.byId("gridFollowUp");
                if (hasValue(prospect) && hasValue(prospect.ProspectFollowUp) && prospect.ProspectFollowUp.length > 0)
                    if (hasValue(gridFollowUp))
                        gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: dataProspect, idProperty: "nroOrdem" }) }));
                    else
                        criarGridFollowUp(null, prospect.ProspectFollowUp);

                //Volta para a primeira aba:
                tabs = dijit.byId("tabContainer");
                pane = dijit.byId("tabAluno");
                tabs.selectChild(pane);
                if (hasValue(funcaoRetorno, false))
                    funcaoRetorno.call();
                showCarregando();
            },
            function (error) {
                hideCarregando();
                
                if (hasValue(error.response.data)) {
                    var haErro = JSON.parse(error.response.data);
                    if (typeof haErro == "string") {
                        haErro = JSON.parse(haErro);
                    }

                    if (hasValue(haErro.erro) || !hasValue(haErro.retorno)) {
                        if (hasValue(haErro.MensagensWeb)) {
                            var mensagem = haErro.MensagensWeb[0].mensagem.indexOf('||') > 0
                                ? haErro.MensagensWeb[0].mensagem.substring(0,
                                    haErro.MensagensWeb[0].mensagem.indexOf('||'))
                                : haErro.MensagensWeb[0].mensagem;
                            caixaDialogo(DIALOGO_ERRO, mensagem, null);
                        }
                    }

                } else {
                    caixaDialogo(DIALOGO_ERRO, error.message, null);
                }
                
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

//Deprecate
function configuraCamposDependentes(codCidade, codTratamento, codEstadoCivil, codNacionalidade, descLocalNascimento, codMotivoBolsa, codMotivoCancelamentoBolsa) {
    dojo.byId('codCidade').value = codCidade;
    dijit.byId('tratamento').set('value', codTratamento);
    dijit.byId('estadoCivil').set('value', codEstadoCivil);
    dijit.byId('nacionalidade').set('value', codNacionalidade);
    dojo.byId("descLocalNascimento").value = hasValue(descLocalNascimento) ? descLocalNascimento : '';

    dijit.byId("cbMotivoBolsa").set('value', codMotivoBolsa);
    dijit.byId("cbMotivoCancBolsa").set('value', codMotivoCancelamentoBolsa);
}

function montarMotivoSim() {
    var motivoSim = [];
    var gridMotivoSim = dijit.byId("gridMotivoSim");
    var data = new Array();

    hasValue(gridMotivoSim) ? gridMotivoSim.store.save() : null;
    if (hasValue(gridMotivoSim) && hasValue(gridMotivoSim.store.objectStore.data))
        data = gridMotivoSim.store.objectStore.data;
    else
        motivoSim = null;
    if (hasValue(gridMotivoSim) && hasValue(data) && data.length > 0)
        $.each(data, function (idx, val) {
            motivoSim.push({ cd_motivo_matricula: val.cd_motivo_matricula, dc_motivo_matricula: val.dc_motivo_matricula });
        });
    return motivoSim;
}

function montarAlunosRestricao() {
	var listaAlunoRestricao = [];
	var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");
	var data = new Array();

	hasValue(gridAlunoRestricao) ? gridAlunoRestricao.store.save() : null;
    if (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) {
	    data = gridAlunoRestricao.store.objectStore.data;
    } else {
	    listaAlunoRestricao = null;
    }
		
    if (hasValue(gridAlunoRestricao) && hasValue(data) && data.length > 0) {
	    $.each(data, function (idx, val) {
		    listaAlunoRestricao.push({
			    cd_aluno_resticao: val.cd_aluno_resticao,
			    cd_aluno: val.cd_aluno,
			    cd_orgao_financeiro: val.cd_orgao_financeiro,
			    cd_usuario: val.cd_usuario,
			    dt_inicio_restricao: val.dt_inicio_restricao,
			    dt_final_restricao: val.dt_final_restricao,
			    dt_cadastro: val.dt_cadastro

		    });
	    });
    }
		
	return listaAlunoRestricao;
}

//Deprecate
function montarfollowUp() {
    var followUp = null;
    var gridFollowUp = dijit.byId("gridFollowUp");
    var data = new Array();

    if (hasValue(gridFollowUp)) {
        gridFollowUp.store.save()
        if (hasValue(gridFollowUp) && hasValue(gridFollowUp.store.objectStore.data))
            followUp = gridFollowUp.store.objectStore.data;
        else
            followUp = [];
    }
    return followUp;
}

function montaDadosAluno(dom, domAttr, indNaoPopula) {
    try {
        var outrosEnderecos = null;
        var outrosContatos = null;
        var relacionamentos = null;
        var motivosNaoMatricula = null;
        var cursosPretendidos = null;

        var gridEnderecos = dijit.byId("gridEnderecos");
        if (hasValue(gridEnderecos) && hasValue(gridEnderecos.store.objectStore) && hasValue(gridEnderecos.store.objectStore.data))
            outrosEnderecos = gridEnderecos.store.objectStore.data;

        var gridContatos = dijit.byId("gridContatos");
        if (hasValue(gridContatos) && hasValue(gridContatos.store.objectStore) && hasValue(gridContatos.store.objectStore.data))
            outrosContatos = gridContatos.store.objectStore.data;

        var descFoto = null;
        var ehImgUpload = false;
        if (hasValue(dijit.byId("uploader")) && hasValue(dijit.byId("uploader")._files) &&
            hasValue(dijit.byId("uploader")._files[0])) {
            descFoto = getNameFoto(dijit.byId("uploader")._files[0]);
            ehImgUpload = true;
        }
        else
            descFoto = dojo.byId('setValueExt_foto_Pessoa').value;

        if (hasValue(dijit.byId("gridRelacionamentos")))
            relacionamentos = montarRelacionamentosCompleto();
        var atividade = hasValue(dojo.byId('cdAtividade').value > 0) ? dojo.byId('cdAtividade').value : null;

        var horarios = null;
        var gridHorarios = dijit.byId("gridHorarios");
        if (gridHorarios && gridHorarios.items.length > 0)
            horarios = gridHorarios.params.store.data;

        var dadosRetorno = {
            aluno: {
                AlunoPessoaFisica: {
                    cd_pessoa: dom.byId("cd_pessoa").value,
                    no_pessoa: dom.byId("nomPessoa").value,
                    dc_reduzido_pessoa: dom.byId("nomFantasia").value,
                    cd_atividade_principal: atividade,
                    Atividade: {
                        no_atividade: dom.byId("atividadePrincipal").value
                    },
                    hr_cadastro: dojo.byId("horaCadastro").value,
                    cd_estado_civil: hasValue(dijit.byId("estadoCivil").get("value")) ? dijit.byId("estadoCivil").get("value") : null,
                    cd_loc_nascimento: hasValue(dojo.byId('codLocalNascimento').value) ? dojo.byId('codLocalNascimento').value : null,
                    cd_loc_nacionalidade: hasValue(dijit.byId("nacionalidade").get("value")) ? dijit.byId("nacionalidade").get("value") : null,
                    cd_tratamento: hasValue(dijit.byId("tratamento").get("value")) ? dijit.byId("tratamento").get("value") : null,
                    cd_pessoa_cpf: hasValue(dojo.byId('cdPessoaCpf').value) && dojo.byId('cdPessoaCpf').value > 0 ? dojo.byId('cdPessoaCpf').value : null,
                    nm_sexo: hasValue(dijit.byId("sexo").get("value")) ? dijit.byId("sexo").get("value") : 0,
                    dt_nascimento: hasValue(dojo.byId("dtaNasci").value) ? dojo.date.locale.parse(dojo.byId("dtaNasci").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    dt_casamento: hasValue(dojo.byId("dtaCasamento").value) ? dojo.date.locale.parse(dojo.byId("dtaCasamento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    nm_cpf: hasValue(dojo.byId("cpf").value) ? dojo.byId("cpf").value : "",
                    nm_doc_identidade: hasValue(dojo.byId("nroRg").value) ? dojo.byId("nroRg").value : "",
                    dc_carteira_trabalho: hasValue(dojo.byId("carteiraTrabalho").value) ? dojo.byId("carteiraTrabalho").value : "",
                    dc_carteira_motorista: hasValue(dojo.byId("carteiraMotorista").value) ? dojo.byId("carteiraMotorista").value : "",
                    dc_num_titulo_eleitor: hasValue(dojo.byId("tituloEleitor").value) ? dojo.byId("tituloEleitor").value : "",
                    dc_num_insc_inss: hasValue(dojo.byId("nroInss").value) ? dojo.byId("nroInss").value : "",
                    dt_cadastramento: hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    cd_orgao_expedidor: hasValue(dijit.byId("orgExp").get("value")) ? dijit.byId("orgExp").get("value") : null,
                    cd_estado_expedidor: hasValue(dijit.byId("expRG").get("value")) ? dijit.byId("expRG").get("value") : null,
                    dt_emis_expedidor: hasValue(dojo.byId("dtaEmiRg").value) ? dojo.date.locale.parse(dojo.byId("dtaEmiRg").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    txt_obs_pessoa: dom.byId('descObsAluno').value,
                    LocalidadeNascimento: {
                        no_localidade: hasValue(dojo.byId("descLocalNascimento").value) ? dojo.byId("descLocalNascimento").value : ''
                    },
                    Telefone: {
                        dc_fone_mail: hasValue(dojo.byId("telefone").value) ? dojo.byId("telefone").value : '',
                        cd_operadora: hasValue(dijit.byId("operadora").get("value")) ? dijit.byId("operadora").get('value') : null
                    },
                    site: hasValue(dojo.byId("site").value) ? dojo.byId("site").value : '',
                    email: hasValue(dojo.byId("email").value) ? dojo.byId("email").value : '',
                    celular: hasValue(dojo.byId("celular").value) ? dojo.byId("celular").value : '',
                    Endereco: {
                        cd_loc_cidade: hasValue(dojo.byId("codCidade").value) ? dojo.byId("codCidade").value : 0,
                        cd_loc_estado: hasValue(dijit.byId("estado").get("value")) ? dijit.byId("estado").get("value") : 0,
                        cd_loc_pais: 1,
                        cd_tipo_endereco: hasValue(dijit.byId("tipoEndereco").get("value")) ? dijit.byId("tipoEndereco").get("value") : 0,
                        cd_tipo_logradouro: hasValue(dijit.byId("logradouro").get("value")) ? dijit.byId("logradouro").get('value') : 0,
                        cd_loc_bairro: hasValue(dojo.byId('codBairro').value) ? dojo.byId('codBairro').value : 0,
                        cd_loc_logradouro: hasValue(dojo.byId('codRua').value) ? dojo.byId('codRua').value : 0,
                        dc_compl_endereco: hasValue(dijit.byId("complemento").get("value")) ? dijit.byId("complemento").get("value") : '',
                        num_cep: hasValue(dojo.byId("cep").value) ? dojo.byId("cep").value : '',
                        dc_num_endereco: hasValue(dojo.byId("numero").value) ? dojo.byId("numero").value : '',
                        LocalidadeDistrito: {
                            no_localidade: dojo.byId('cidade').value
                        },
                        LocalidadeLogradouro: {
                            no_localidade: dojo.byId('rua').value
                        },
                        Localidade: {
                            no_localidade: dojo.byId('bairro').value
                        }
                    },
                    ext_img_pessoa: descFoto,
                    ehImgUpload: ehImgUpload
                },
                Bolsa: {
                    pc_bolsa: hasValue(dojo.byId("pc_bolsa").value) ? dojo.byId("pc_bolsa").value : '',
                    dt_inicio_bolsa: hasValue(dojo.byId("dt_inicio_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    dc_validade_bolsa: hasValue(dojo.byId("dc_validade_bolsa").value) ? dojo.byId("dc_validade_bolsa").value : '',
                    dt_inicio_bolsa: hasValue(dojo.byId("dt_inicio_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    pc_bolsa_material: hasValue(dojo.byId("pc_bolsa_material").value) ? dojo.byId("pc_bolsa_material").value : '',
                    dt_comunicado_bolsa: hasValue(dojo.byId("dt_comunicado_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_comunicado_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,

                    pc_bolsa_material: hasValue(dojo.byId("pc_bolsa_material").value) ? dojo.byId("pc_bolsa_material").value : '',
                    dt_cancelamento_bolsa: hasValue(dojo.byId("dt_cancelamento_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_cancelamento_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                    cd_motivo_bolsa: hasValue(dijit.byId("cbMotivoBolsa").value) ? dijit.byId("cbMotivoBolsa").value : '',
                    cd_motivo_cancelamento_bolsa: hasValue(dijit.byId("cbMotivoCancBolsa").value) ? dijit.byId("cbMotivoCancBolsa").value : 0
                },
                cd_aluno: hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0" ? dojo.byId("cdAluno").value : 0,
                cd_pessoa_aluno: hasValue(dojo.byId("cd_pessoa").value) && dojo.byId("cd_pessoa").value != "0" ? dojo.byId("cd_pessoa").value : 0,
                //cd_pessoa_escola: , // TODO: ao inserir um aluno de uma outra escola, o mesmo deve ser outro aluno.
                cd_midia: hasValue(dijit.byId("cbMarketing").get("value")) ? dijit.byId("cbMarketing").get("value") : 0,
                cd_escolaridade: hasValue(dijit.byId("cbEscolaridade").get("value")) ? dijit.byId("cbEscolaridade").get("value") : 0,
                //nm_matricula: , //TODO: colocar o número da matrícula para o aluno

                //dc_local_transferencia: , //TODO: colocar o local de transferência para o aluno
                id_aluno_ativo: domAttr.get("idPessoaAtiva", "checked"),
                Atendente: {
                    cd_usuario: hasValue(dojo.byId("cd_atendente").value) ? dojo.byId("cd_atendente").value : 0,
                    PessoaFisica: {
                        no_pessoa: hasValue(dijit.byId("no_atendente").get("value")) ? dijit.byId("no_atendente").get("value") : ''
                    }
                },
                Horarios: horarios
            },
            enderecos: outrosEnderecos,
            contatosUI: {
                outrosContatos: outrosContatos
            },
            relacionamentoUI: relacionamentos,
            motivosNaoMatriculaUI: motivosNaoMatricula,
            cursosPretendidosUI: cursosPretendidos
        };

        return dadosRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montarDadosAlunoForPost(dom, domAttr, date) {
    try {
        var dadosRetorno = null;
        var motivosSimMatricula = montarMotivoSim();
        var alunosRestricaoUI = montarAlunosRestricao();
        //var followUp = montarfollowUp();
        var followUp = null;
        var pessoaFisicaUI = montarDadosPessoaFisica(date, 'apresentadorMensagemAluno');
        if (!pessoaFisicaUI) return false;
        pessoaFisicaUI.pessoaFisica.txt_obs_pessoa = dijit.byId("descObsAluno").value;
        var horarios = null;
        var atualizaHorarios = false;
        var atualizaFollowUp = false;
        var parametros = getParamterosURL();
        var linkProspect = 0;
        var gridFollowUp = dijit.byId("gridFollowUp");

        if (hasValue(gridFollowUp)) {
            gridFollowUp.store.save()
            atualizaFollowUp = true;
            if (hasValue(gridFollowUp) && hasValue(gridFollowUp.store.objectStore.data))
                followUp = gridFollowUp.store.objectStore.data;
        }
        if (hasValue(dijit.byId("gridHorarios"))) {
            atualizaHorarios = true;
            if (dijit.byId("gridHorarios").items.length > 0)
                horarios = mountHorarios(dijit.byId('gridHorarios').params.store.data);
        }
        if (hasValue(parametros['cd_prospect']) && eval(document.getElementById('parametroPospectValido').value))
            linkProspect = jQuery.parseJSON(parametros.cd_prospect);
        else
            linkProspect = dojo.byId("cd_prospect").value > 0 ? dojo.byId("cd_prospect").value : 0;

        require(["dojo/dom", "dojo/dom-attr", "dojo/request/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
            dadosRetorno = {
                aluno: {
                    cd_aluno: hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0" ? dojo.byId("cdAluno").value : 0,
                    cd_pessoa_aluno: hasValue(dojo.byId("cd_pessoa").value) && dojo.byId("cd_pessoa").value != "0" ? dojo.byId("cd_pessoa").value : 0,
                    cd_prospect: linkProspect > 0 ? linkProspect : null,
                    cd_midia: hasValue(dijit.byId("cbMarketing").get("value")) ? dijit.byId("cbMarketing").get("value") : 0,
                    cd_escolaridade: hasValue(dijit.byId("cbEscolaridade").get("value")) ? dijit.byId("cbEscolaridade").get("value") : null,
                    //nm_matricula: , //TODO: colocar o número da matrícula para o aluno
                    //dc_local_transferencia: , //TODO: colocar o local de transferência para o aluno
                    id_aluno_ativo: domAttr.get("idPessoaAtiva", "checked"),
                    Atendente: {
                        cd_usuario: hasValue(dojo.byId("cd_atendente").value) ? dojo.byId("cd_atendente").value : 0,
                        PessoaFisica: {
                            no_pessoa: hasValue(dijit.byId("no_atendente").get("value")) ? dijit.byId("no_atendente").get("value") : ''
                        }
                    },
                    Bolsa: {
                        pc_bolsa: hasValue(dijit.byId("pc_bolsa").value) ? dijit.byId("pc_bolsa").value : '',
                        dc_validade_bolsa: hasValue(dojo.byId("dc_validade_bolsa").value) ? dojo.byId("dc_validade_bolsa").value : '',
                        dt_inicio_bolsa: hasValue(dojo.byId("dt_inicio_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_inicio_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                        dt_comunicado_bolsa: hasValue(dojo.byId("dt_comunicado_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_comunicado_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,

                        pc_bolsa_material: hasValue(dijit.byId("pc_bolsa_material").value) ? dijit.byId("pc_bolsa_material").value : '',
                        dt_cancelamento_bolsa: hasValue(dojo.byId("dt_cancelamento_bolsa").value) ? dojo.date.locale.parse(dojo.byId("dt_cancelamento_bolsa").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,

                        cd_motivo_bolsa: hasValue(dijit.byId("cbMotivoBolsa").value) ? dijit.byId("cbMotivoBolsa").value : null,
                        cd_motivo_cancelamento_bolsa: hasValue(dijit.byId("cbMotivoCancBolsa").value) ? dijit.byId("cbMotivoCancBolsa").value : null
                    }
                },
                pessoaFisicaUI: pessoaFisicaUI,
                motivosMatriculaUI: motivosSimMatricula,
                horarioSearchUI: horarios,
                atualizaHorarios: atualizaHorarios,
                atualizaFollowUp: atualizaFollowUp,
                followUpUI: followUp,
                alunosRestricaoUI: alunosRestricaoUI,
                pessoaRaf: hasValue(dojo.byId("paiTabRafAluno")) && hasValue(dijit.byId("nmRaf").value) && eval(MasterGeral()) == true ? {
                    cd_pessoa_raf: dojo.byId("cd_pessoa_raf").value,
                    cd_pessoa: dojo.byId("raf_cd_pessoa").value,
                    nm_raf: dijit.byId("nmRaf").value,
                    id_raf_liberado: dijit.byId("ckLiberado").checked,
                    dt_limite_bloqueio: hasValue(dojo.byId("dt_limite_bloqueio").value) ? dojo.date.locale.parse(dojo.byId("dt_limite_bloqueio").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
                } : null,
                fichaSaude: {
                    cd_ficha_saude: hasValue(dojo.byId("cd_ficha_saude").value) && dojo.byId("cd_ficha_saude").value != "0" ? dojo.byId("cd_ficha_saude").value : 0,
                    cd_aluno: hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0" ? dojo.byId("cdAluno").value : 0,
                    id_problema_saude: hasValue(dijit.byId("id_problema_saude").get("value")) ? getValueBoolean(dijit.byId("id_problema_saude").get("value")) : null,
                    dc_problema_saude: hasValue(dijit.byId("dc_problema_saude").get("value")) ? dijit.byId("dc_problema_saude").get("value") : null,
                    id_tratamento_medico: hasValue(dijit.byId("id_tratamento_medico").get("value")) ? getValueBoolean(dijit.byId("id_tratamento_medico").get("value")) : null,
                    dc_tratamento_medico: hasValue(dijit.byId("dc_tratamento_medico").get("value")) ? dijit.byId("dc_tratamento_medico").get("value") : null,
                    id_uso_medicamento: hasValue(dijit.byId("id_uso_medicamento").get("value")) ? getValueBoolean(dijit.byId("id_uso_medicamento").get("value")) : null,
                    dc_uso_medicamento: hasValue(dijit.byId("dc_uso_medicamento").get("value")) ? dijit.byId("dc_uso_medicamento").get("value") : null,
                    id_recomendacao_medica: hasValue(dijit.byId("id_recomendacao_medica").get("value")) ? getValueBoolean(dijit.byId("id_recomendacao_medica").get("value")) : null,
                    dc_recomendacao_medica: hasValue(dijit.byId("dc_recomendacao_medica").get("value")) ? dijit.byId("dc_recomendacao_medica").get("value") : null,
                    id_alergico: hasValue(dijit.byId("id_alergico").get("value")) ? getValueBoolean(dijit.byId("id_alergico").get("value")) : null,
                    dc_alergico: hasValue(dijit.byId("dc_alergico").get("value")) ? dijit.byId("dc_alergico").get("value") : null,
                    id_alergico_alimento_material: hasValue(dijit.byId("id_alergico_alimento_material").get("value")) ? getValueBoolean(dijit.byId("id_alergico_alimento_material").get("value")) : null,
                    dc_alergico_alimento_material: hasValue(dijit.byId("dc_alergico_alimento_material").get("value")) ? dijit.byId("dc_alergico_alimento_material").get("value") : null,
                    id_epiletico: hasValue(dijit.byId("id_epiletico").get("value")) ? getValueBoolean(dijit.byId("id_epiletico").get("value")) : null,
                    id_epiletico_tratamento: hasValue(dijit.byId("id_epiletico_tratamento").get("value")) ? getValueBoolean(dijit.byId("id_epiletico_tratamento").get("value")) : null,
                    id_asmatico: hasValue(dijit.byId("id_asmatico").get("value")) ? getValueBoolean(dijit.byId("id_asmatico").get("value")) : null,
                    id_asmatico_tratamento: hasValue(dijit.byId("id_asmatico_tratamento").get("value")) ? getValueBoolean(dijit.byId("id_asmatico_tratamento").get("value")) : null,
                    id_diabetico: hasValue(dijit.byId("id_diabetico").get("value")) ? getValueBoolean(dijit.byId("id_diabetico").get("value")) : null,
                    id_depende_insulina: hasValue(dijit.byId("id_depende_insulina").get("value")) ? getValueBoolean(dijit.byId("id_depende_insulina").get("value")) : null,
                    id_medicacao_especifica: hasValue(dijit.byId("id_medicacao_especifica").get("value")) ? getValueBoolean(dijit.byId("id_medicacao_especifica").get("value")) : null,
                    dc_medicacao_especifica: hasValue(dijit.byId("dc_medicacao_especifica").get("value")) ? dijit.byId("dc_medicacao_especifica").get("value") : null,
                    dt_hora_medicacao_especifica: hasValue(dom.byId("dt_hora_medicacao_especifica").value) ? dom.byId("dt_hora_medicacao_especifica").value : null,
                    tx_informacoes_adicionais: hasValue(dijit.byId("tx_informacoes_adicionais").get("value")) ? dijit.byId("tx_informacoes_adicionais").get("value") : null,
                    id_plano_saude: hasValue(dijit.byId("id_plano_saude").get("value")) ? getValueBoolean(dijit.byId("id_plano_saude").get("value")) : null,
                    dc_plano_saude: hasValue(dijit.byId("dc_plano_saude").get("value")) ? dijit.byId("dc_plano_saude").get("value") : null,
                    dc_nm_carteirinha_plano: hasValue(dijit.byId("dc_nm_carteirinha_plano").get("value")) ? dijit.byId("dc_nm_carteirinha_plano").get("value") : null,
                    dc_categoria_plano: hasValue(dijit.byId("dc_categoria_plano").get("value")) ? dijit.byId("dc_categoria_plano").get("value") : null,
                    dc_nome_clinica_hospital: hasValue(dijit.byId("dc_nome_clinica_hospital").get("value")) ? dijit.byId("dc_nome_clinica_hospital").get("value") : null,
                    dc_endereco_hospital_clinica: hasValue(dijit.byId("dc_endereco_hospital_clinica").get("value")) ? dijit.byId("dc_endereco_hospital_clinica").get("value") : null,
                    dc_telefone_hospital_clinica: hasValue(dijit.byId("dc_telefone_hospital_clinica").get("value")) ? dijit.byId("dc_telefone_hospital_clinica").get("value") : null,
                    dc_telefone_fixo_hospital_clinica: hasValue(dijit.byId("dc_telefone_fixo_hospital_clinica").get("value")) ? dijit.byId("dc_telefone_fixo_hospital_clinica").get("value") : null,
                    id_aviso_emergencia: hasValue(dijit.byId("id_aviso_emergencia").get("value")) ? dijit.byId("id_aviso_emergencia").get("value") : null,
                    dc_nome_pessoa_aviso_emergencia: hasValue(dijit.byId("dc_nome_pessoa_aviso_emergencia").get("value")) ? dijit.byId("dc_nome_pessoa_aviso_emergencia").get("value") : null,
                    dc_parentesco_aviso_emergencia: hasValue(dijit.byId("dc_parentesco_aviso_emergencia").get("value")) ? dijit.byId("dc_parentesco_aviso_emergencia").get("value") : null,
                    dc_telefone_residencial_aviso_emergencia: hasValue(dijit.byId("dc_telefone_residencial_aviso_emergencia").get("value")) ? dijit.byId("dc_telefone_residencial_aviso_emergencia").get("value") : null,
                    dc_telefone_comercial_aviso_emergencia: hasValue(dijit.byId("dc_telefone_comercial_aviso_emergencia").get("value")) ? dijit.byId("dc_telefone_comercial_aviso_emergencia").get("value") : null,
                    dc_telefone_celular_aviso_emergencia: hasValue(dijit.byId("dc_telefone_celular_aviso_emergencia").get("value")) ? dijit.byId("dc_telefone_celular_aviso_emergencia").get("value") : null,
                }
            }
        });
        return dadosRetorno;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function getValueBoolean(value) {
    try {
        if (value === "1") {
            return 1;
        }else if (value === "0") {
            return 0;
        } else {
            return null;
        }
        
    } catch (e) {

    } 
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

/* Grid AlunoRestricao*/
function montaGridSubCadastroAlunoRestricao(EnhancedGrid, ObjectStore, Memory) {
	var dataAlunoRestricao = [];
	var gridAlunoRestricao = new EnhancedGrid({
		store: new ObjectStore({ objectStore: new Memory({ data: dataAlunoRestricao }) }),
		structure:
		[
			{ name: "<input id='selecionaTodosAlunoRestricao' style='display:none'/>", field: "selecionadoAlunoRestricao", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxAlunoRestricao },
            { name: "Orgão", field: "dc_orgao_financeiro", width: "20%" },
			{ name: "Responsável", field: "no_responsavel", width: "30%" },
            { name: "Inclusão", field: "dtaInicioRestricao", width: "20%" },
            { name: "Saída", field: "dtaFinalRestricao", width: "20%" },
            { name: "Dt.Cadastro", field: "dtaCadastro", width: "20%" }
		],
		canSort: true,
		noDataMessage: "Nenhum registro encontrado."
	}, "gridAlunoRestricao");
    gridAlunoRestricao.startup();

    gridAlunoRestricao.on("RowDblClick", function (evt) {
	    try {
		    var idx = evt.rowIndex,
			    item = this.getItem(idx),
                store = this.store;

		    if (hasValue(item)) {
                dijit.byId("gridAlunoRestricao").itemSelecionado = item;

                populaOrgaoFinanceiroEUsuarioAtendenteEdicao(item.cd_orgao_financeiro, EnumStatusOrgaoFinanceiro.ATIVO, item);

                
		    }


	    }
	    catch (e) {
		    postGerarLog(e);
	    }
    }, true);
}


function formatCheckBoxAlunoRestricao(value, rowIndex, obj) {
	try {
		var gridName = 'gridAlunoRestricao';
		var grid = dijit.byId(gridName);
		var icon;
		var id = obj.field + '_Selected_' + rowIndex;
		var todos = dojo.byId('selecionaTodosAlunoRestricao');

		if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "id_grid_aluno_restricao", grid._by_idx[rowIndex].item.id_grid_aluno_restricao);

			value = value || indice != null; // Item está selecionado.
		}
		if (rowIndex != -1)
			icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

		// Configura o check de todos:
		if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'id_grid_aluno_restricao', 'selecionadoAlunoRestricao', -1, 'selecionaTodosAlunoRestricao', 'selecionaTodosAlunoRestricao', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'id_grid_aluno_restricao', 'selecionadoAlunoRestricao', " + rowIndex + ", '" + id + "', 'selecionaTodosAlunoRestricao', '" + gridName + "')", 2);

		return icon;
	}
	catch (e) {
		postGerarLog(e);
	}
}

/* Fim Grid AlunoRestricao*/

function montaGridSubCadastros(EnhancedGrid, ObjectStore, Memory) {
    var dataMotivoSim = [];
    var gridMotivoSim = new EnhancedGrid({
        store: new ObjectStore({ objectStore: new Memory({ data: dataMotivoSim }) }),
        structure:
          [
            { name: "<input id='selecionaTodosMsim' style='display:none'/>", field: "selecionadoMsim", width: "25px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxMsim },
            { name: "Descrição", field: "dc_motivo_matricula", width: "70%" }
          ],
        canSort: true,
        noDataMessage: "Nenhum registro encontrado."
    }, "gridMotivoSim");
    gridMotivoSim.startup();
}

function formatCheckBoxMsim(value, rowIndex, obj) {
    try {
        var gridName = 'gridMotivoSim'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosMsim');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_motivo_matricula", grid._by_idx[rowIndex].item.cd_motivo_matricula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_motivo_matricula', 'selecionadoMsim', -1, 'selecionaTodosMsim', 'selecionaTodosMsim', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_motivo_matricula', 'selecionadoMsim', " + rowIndex + ", '" + id + "', 'selecionaTodosMsim', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

var STATUS_TODOS = 0;
var STATUS_ATIVO = 1;
var STATUS_INATIVO = 2;
function populaMotivoBolsa(id, status) {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMotivoBolsa?status=" + status + "&cd_motivo_bolsa=" + id,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbMotivoBolsa', 'cd_motivo_bolsa', 'dc_motivo_bolsa');
            if (hasValue(id))
                dijit.byId("cbMotivoBolsa").set("value", id);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function populaMotivoCancBolsa(id, status) {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMotivoCancelamentoBolsa?status=" + status + "&cd_motivo_cancelamento_bolsa=" + id,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbMotivoCancBolsa', 'cd_motivo_cancelamento_bolsa', 'dc_motivo_cancelamento_bolsa');
            if (hasValue(id))
                dijit.byId("cbMotivoCancBolsa").set("value", id);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function populaEscolaridade(id, status) {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getEscolaridade?status=" + status,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbEscolaridade', 'cd_escolaridade', 'no_escolaridade');
            if (hasValue(id))
                dijit.byId("cbEscolaridade").set("value", id);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function populaGridAlunoRestricao(cd_aluno, ObjectStore, Memory) {
	dojo.xhr.get({
		url: Endereco() + "/api/secretaria/getAlunoRestricaoByCdAluno?cd_aluno=" + cd_aluno,
		preventCache: true,
		handleAs: "json",
		headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
	}).then(function (dados) {
        try {
	            if (hasValue(dados)) {
		            var gridAlunoRestricao = dijit.byId('gridAlunoRestricao');
                    var listaAlunoRestricaoRetorno = jQuery.parseJSON(dados).retorno;
                    var data = (hasValue(gridAlunoRestricao) && hasValue(gridAlunoRestricao.store.objectStore.data)) ? gridAlunoRestricao.store.objectStore.data : [];
	               // dijit.byId("gridAlunoRestricao").setStore(new ObjectStore({ objectStore: new Memory({ data: [] }) }));

                    if (hasValue(listaAlunoRestricaoRetorno) && listaAlunoRestricaoRetorno.length > 0) {
                        $.each(listaAlunoRestricaoRetorno, function (index, val) {
	                        val.id_grid_aluno_restricao = geradorIdAlunoRestricao(gridAlunoRestricao);
                            insertObjSort(gridAlunoRestricao.store.objectStore.data, "id_grid_aluno_restricao", val);
	                        
                        });
                        gridAlunoRestricao.itensSelecionados = new Array();
                        gridAlunoRestricao._refresh();
	                }
	            }
	        
		}
		catch (e) {
			postGerarLog(e);
		}
	},
	function (error) {
		apresentaMensagem('apresentadorMensagemAluno', error);
	});
}

function populaFichaSaude(valueFichaSaude)
{
    try {
        dt_hora_medicacao_especifica_split = [];
        if (hasValue(valueFichaSaude.dt_hora_medicacao_especifica)) {
            dt_hora_medicacao_especifica_split = valueFichaSaude.dt_hora_medicacao_especifica.split(":");
        }

        dojo.byId("cd_ficha_saude").value = (valueFichaSaude.cd_ficha_saude != null && valueFichaSaude.cd_ficha_saude != undefined && valueFichaSaude.cd_ficha_saude > 0) ? valueFichaSaude.cd_ficha_saude : 0;
        dijit.byId("id_problema_saude").set("value", getValueComboFichaSaude(valueFichaSaude.id_problema_saude));
        dijit.byId("dc_problema_saude").set("value", valueFichaSaude.dc_problema_saude);
        dijit.byId("id_tratamento_medico").set("value", getValueComboFichaSaude(valueFichaSaude.id_tratamento_medico));
        dijit.byId("dc_tratamento_medico").set("value",valueFichaSaude.dc_tratamento_medico);
        dijit.byId("id_uso_medicamento").set("value", getValueComboFichaSaude(valueFichaSaude.id_uso_medicamento));
        dijit.byId("dc_uso_medicamento").set("value", valueFichaSaude.dc_uso_medicamento);
        dijit.byId("id_recomendacao_medica").set("value", getValueComboFichaSaude(valueFichaSaude.id_recomendacao_medica));
        dijit.byId("dc_recomendacao_medica").set("value",valueFichaSaude.dc_recomendacao_medica);
        dijit.byId("id_alergico").set("value", getValueComboFichaSaude(valueFichaSaude.id_alergico));
        dijit.byId("dc_alergico").set("value",valueFichaSaude.dc_alergico);
        dijit.byId("id_alergico_alimento_material").set("value", getValueComboFichaSaude(valueFichaSaude.id_alergico_alimento_material));
        dijit.byId("dc_alergico_alimento_material").set("value", valueFichaSaude.dc_alergico_alimento_material);
        dijit.byId("id_epiletico").set("value", getValueComboFichaSaude(valueFichaSaude.id_epiletico));
        dijit.byId("id_epiletico_tratamento").set("value", getValueComboFichaSaude(valueFichaSaude.id_epiletico_tratamento));
        dijit.byId("id_asmatico").set("value", getValueComboFichaSaude(valueFichaSaude.id_asmatico));
        dijit.byId("id_asmatico_tratamento").set("value", getValueComboFichaSaude(valueFichaSaude.id_asmatico_tratamento));
        dijit.byId("id_diabetico").set("value", getValueComboFichaSaude(valueFichaSaude.id_diabetico));
        dijit.byId("id_depende_insulina").set("value", getValueComboFichaSaude(valueFichaSaude.id_depende_insulina));
        dijit.byId("id_medicacao_especifica").set("value", getValueComboFichaSaude(valueFichaSaude.id_medicacao_especifica));
        dijit.byId("dc_medicacao_especifica").set("value",valueFichaSaude.dc_medicacao_especifica);
        dijit.byId("dt_hora_medicacao_especifica").set("value", valueFichaSaude.dt_hora_medicacao_especifica != null && dt_hora_medicacao_especifica_split != null && dt_hora_medicacao_especifica_split.length > 0 ? new Date(1970, 1, 1, dt_hora_medicacao_especifica_split[0], dt_hora_medicacao_especifica_split[1]) : null) ;
        dijit.byId("tx_informacoes_adicionais").set("value",valueFichaSaude.tx_informacoes_adicionais);
        dijit.byId("id_plano_saude").set("value", getValueComboFichaSaude(valueFichaSaude.id_plano_saude));
        dijit.byId("dc_plano_saude").set("value",valueFichaSaude.dc_plano_saude);
        dijit.byId("dc_nm_carteirinha_plano").set("value",valueFichaSaude.dc_nm_carteirinha_plano);
        dijit.byId("dc_categoria_plano").set("value",valueFichaSaude.dc_categoria_plano);
        dijit.byId("dc_nome_clinica_hospital").set("value",valueFichaSaude.dc_nome_clinica_hospital);
        dijit.byId("dc_endereco_hospital_clinica").set("value",valueFichaSaude.dc_endereco_hospital_clinica);
        dijit.byId("dc_telefone_hospital_clinica").set("value",valueFichaSaude.dc_telefone_hospital_clinica);
        dijit.byId("dc_telefone_fixo_hospital_clinica").set("value",valueFichaSaude.dc_telefone_fixo_hospital_clinica);
        dijit.byId("id_aviso_emergencia").set("value",valueFichaSaude.id_aviso_emergencia);
        dijit.byId("dc_nome_pessoa_aviso_emergencia").set("value",valueFichaSaude.dc_nome_pessoa_aviso_emergencia);
        dijit.byId("dc_parentesco_aviso_emergencia").set("value",valueFichaSaude.dc_parentesco_aviso_emergencia);
        dijit.byId("dc_telefone_residencial_aviso_emergencia").set("value",valueFichaSaude.dc_telefone_residencial_aviso_emergencia);
        dijit.byId("dc_telefone_comercial_aviso_emergencia").set("value",valueFichaSaude.dc_telefone_comercial_aviso_emergencia);
        dijit.byId("dc_telefone_celular_aviso_emergencia").set("value",valueFichaSaude.dc_telefone_celular_aviso_emergencia);

    } catch (e) {
        postGerarLog(e);
    } 
}

function getValueComboFichaSaude(value) {
    try {
        if (value === true) {
            return 1;
        } else if (value === false) {
            return 0;
        } else {
            return null;
        }

    } catch (e) {

    }
}



function eventoEditarAlunoRestricao(itensSelecionados) {
	try {
		if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
			caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
		else if (itensSelecionados.length > 1)
			caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
		else {
			var gridAlunoRestricao = dijit.byId('gridAlunoRestricao');

			apresentaMensagem('apresentadorMensagem', '');
            populaOrgaoFinanceiroEUsuarioAtendenteEdicao(itensSelecionados[0].cd_orgao_financeiro, EnumStatusOrgaoFinanceiro.ATIVO, itensSelecionados[0]);
		}
	}
	catch (e) {
		postGerarLog(e);
	}
}


function populaOrgaoFinanceiroEUsuarioAtendente(id, status) {
	dojo.xhr.get({
		url: Endereco() + "/api/secretaria/getOrgaoFinanceiro?status=" + status,
		preventCache: true,
		handleAs: "json",
		headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dadosOrgaoFinanceiro) {

		dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getNomeAndCodigoUsuarioAtendente?status=" + status,
			preventCache: true,
			handleAs: "json",
			headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
		}).then(function (dados) {
	        try {
                dijit.byId("cadAlunoRestricao").show();
                getLimpar('#formAlunoRestricao');
                clearForm('formAlunoRestricao');
	            loadSelect(jQuery.parseJSON(dadosOrgaoFinanceiro).retorno, 'cbOrgaoFinanceiro', 'cd_orgao_financeiro', 'dc_orgao_financeiro');
	            if (hasValue(id)) {
	                dijit.byId("cbOrgaoFinanceiro").set("value", id);
                }

                
                if (hasValue(dados)) {
	                var usuarioAtendente = jQuery.parseJSON(dados).retorno;
                    dojo.byId("cd_atendente_usuario_restricao").value = usuarioAtendente.cd_usuario;
                    dojo.byId("no_atendente_usuario_restricao").value
                    //dijit.byId("no_atendente_usuario_restricao").set("disabled", false);
                    dijit.byId("no_atendente_usuario_restricao").set("value", usuarioAtendente.no_login);
                    //dijit.byId("no_atendente_usuario_restricao").set("disabled", true);
                }
                    
				

				apresentaMensagem('apresentadorMensagemAlunoRestricao', null);
				IncluirAlterar(1, 'divAlterarAlunoRestricao', 'divAdicionarAlunoRestricao', 'divExcluirAlunoRestricao', 'apresentadorMensagemAlunoRestricao', 'divCancelarAlunoRestricao', 'divLimparAlunoRestricao');
			}
			catch (e) {
				postGerarLog(e);
			}
		},
		function (error) {
			apresentaMensagem('apresentadorMensagemAlunoRestricao', error);
            });
	},
	function (error) {
		apresentaMensagem('apresentadorMensagemAlunoRestricao', error);
	});
}


function populaOrgaoFinanceiroEUsuarioAtendenteEdicao(id, status, itemSelecionado) {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getOrgaoFinanceiro?status=" + status,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dadosOrgaoFinanceiro) {

        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getNomeAndCodigoUsuarioAtendente?status=" + status,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dados) {
            try {
                dijit.byId("cadAlunoRestricao").show();
                getLimpar('#formAlunoRestricao');
                clearForm('formAlunoRestricao');
                loadSelect(jQuery.parseJSON(dadosOrgaoFinanceiro).retorno, 'cbOrgaoFinanceiro', 'cd_orgao_financeiro', 'dc_orgao_financeiro');
                if (hasValue(id)) {
                    dijit.byId("cbOrgaoFinanceiro").set("value", id);
                }

                dojo.byId("cd_aluno_resticao").value = itemSelecionado.cd_aluno_resticao;
                dojo.byId("id_grid_aluno_restricao").value = itemSelecionado.id_grid_aluno_restricao;
                
                if (hasValue(dados)) {
                    var usuarioAtendente = jQuery.parseJSON(dados).retorno;
                    dojo.byId("cd_atendente_usuario_restricao").value = usuarioAtendente.cd_usuario;
                    dojo.byId("no_atendente_usuario_restricao").value
                    dijit.byId("no_atendente_usuario_restricao").set("value", usuarioAtendente.no_login);
                }

                if (hasValue(itemSelecionado.dt_inicio_restricao)) {
	                dojo.byId("dt_inicio_restricao").value = itemSelecionado.dtaInicioRestricao;
                }
                if (hasValue(itemSelecionado.dt_final_restricao)) {
	                dojo.byId("dt_final_restricao").value = itemSelecionado.dtaFinalRestricao;
                }

                apresentaMensagem('apresentadorMensagemAlunoRestricao', null);
                //IncluirAlterar(1, 'divAlterarAlunoRestricao', 'divAdicionarAlunoRestricao', 'divExcluirAlunoRestricao', 'apresentadorMensagemAlunoRestricao', 'divCancelarAlunoRestricao', 'divLimparAlunoRestricao');
                IncluirAlterar(0, 'divAlterarAlunoRestricao', 'divAdicionarAlunoRestricao', 'divExcluirAlunoRestricao', 'apresentadorMensagemAlunoRestricao', 'divCancelarAlunoRestricao', 'divLimparAlunoRestricao');
            }
            catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem('apresentadorMensagemAlunoRestricao', error);
            });
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAlunoRestricao', error);
    });
}


function limparFormAlunoRestricao(id, status) {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getOrgaoFinanceiro?status=" + status,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dadosOrgaoFinanceiro) {

        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getNomeAndCodigoUsuarioAtendente?status=" + status,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dados) {
            try {
                getLimpar('#formAlunoRestricao');
                clearForm('formAlunoRestricao');
                loadSelect(jQuery.parseJSON(dadosOrgaoFinanceiro).retorno, 'cbOrgaoFinanceiro', 'cd_orgao_financeiro', 'dc_orgao_financeiro');
                if (hasValue(id)) {
                    dijit.byId("cbOrgaoFinanceiro").set("value", id);
                }

                dojo.byId("cd_aluno_resticao").value = 0;
                dojo.byId("id_grid_aluno_restricao").value = 0;

                if (hasValue(dados)) {
                    var usuarioAtendente = jQuery.parseJSON(dados).retorno;
                    dojo.byId("cd_atendente_usuario_restricao").value = usuarioAtendente.cd_usuario;
                    dojo.byId("no_atendente_usuario_restricao").value
                    dijit.byId("no_atendente_usuario_restricao").set("value", usuarioAtendente.no_login);
                }


                apresentaMensagem('apresentadorMensagemAlunoRestricao', null);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
            function (error) {
                apresentaMensagem('apresentadorMensagemAlunoRestricao', error);
            });
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAlunoRestricao', error);
    });
}

function populaMidia(id, status) {
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getMidia?status=" + status,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
            loadSelect(jQuery.parseJSON(dados).retorno, 'cbMarketing', 'cd_midia', 'no_midia');
            if (hasValue(id))
                dijit.byId("cbMarketing").set("value", id);
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}

function selecionaTab(e) {
    try {
        var tab = dojo.query(e.target).parents('.dijitTab')[0];

        if (!hasValue(tab)) // Clicou na borda da aba:
            tab = dojo.query(e.target)[0];
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabHorarios' && !hasValue(dijit.byId("gridHorarios"))) {
            if (hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value > 0) {
                loadHorarioAluno(dojo.byId("cdAluno").value);
            } else {
                showCarregando();
                criacaoGradeHorario(null);
            }
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabFollow' && !hasValue(dijit.byId("gridFollowUp"))) {
            if (hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value > 0)
                criarGridFollowUp(dojo.byId("cdAluno").value);
            else
                criarGridFollowUp();
        }
        if (tab.getAttribute('widgetId') == 'tabContainer_tablist_tabMatricula' && !hasValue(dijit.byId("gridTabMatriculas"))) {
            if (hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value > 0)
                criarGridTabMatriculas(dojo.byId("cdAluno").value);
            else
                criarGridTabMatriculas();
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

                    //dijit.byId("timeIni").set("value", "T0" + horaI + ":" + minI + ":00");
                    //dijit.byId("timeFim").set("value", "T" + horaF + ":" + minF + ":00");

                    horaF = minF > 1 ? horaF + 1 : horaF;
                    var ECalendar = declare("extended.Calendar", Calendar, {

                        isItemResizeEnabled: function (item, rendererKind) {
                            return false || (hasValue(item) && item.cssClass == 'Calendar1');
                        },

                        isItemMoveEnabled: function (item, rendererKind) {
                            return false || (hasValue(item) && item.cssClass == 'Calendar1');
                        }
                    });
                    var dataHoje = new Date(2012, 1, 7, 00, 00, 00);
                    if (hasValue(dijit.byId("gridHorarios")))
                        destroyCreateGridHorario();
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
                    calendar.startup();
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
                        var minutosEnd = e.item.endTime.getMinutes() > 9 ? e.item.endTime.getMinutes() : "0" + e.item.endTime.getMinutes();
                        var horasEnd = e.item.endTime.getHours() > 9 ? e.item.endTime.getHours() : "0" + e.item.endTime.getHours();
                        var horarioEnd = horasEnd + ":" + minutosEnd;
                        var minutosStart = e.item.startTime.getMinutes() > 9 ? e.item.startTime.getMinutes() : "0" + e.item.startTime.getMinutes();
                        var horasStart = e.item.startTime.getHours() > 9 ? e.item.startTime.getHours() : "0" + e.item.startTime.getHours();
                        var horarioStart = horasStart + ":" + minutosStart;

                        if (e.item.calendar == "Calendar1")
                            dojo.attr(e.renderer.domNode.id, "title", "Pretendido pelo Aluno  " + horarioStart + " as " + horarioEnd);
                        if (e.item.calendar == "Calendar2") {
                            for (var i = 0; i < dataHorario.length; i++) {
                                if (dataHorario[i].id == e.item.id)
                                    dojo.attr(e.renderer.domNode.id, "title", "Matrículado na turma : " + dataHorario[i].no_registro);//+ " " + horarioStart + " as " + horarioEnd);
                            }
                        }
                        if (e.item.calendar == "Calendar5") {
                            for (var i = 0; i < dataHorario.length; i++) {
                                if (dataHorario[i].id == e.item.id)
                                    dojo.attr(e.renderer.domNode.id, "title", "Aguardando matrículado na turma : " + dataHorario[i].no_registro);//+ " " + horarioStart + " as " + horarioEnd);
                            }
                        }
                    });


                    // Configura o calendário para poder incluir um item no click da grade:
                    var createItem = function (view, d, e) {

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

                        var start, end, hIni, hFim, mFim, mIni;
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
                        if (start != null) {
                            hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                            hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                            mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                            mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                            var inicioDia = start.getDate();
                            if (itens.length == 0) {
                                item = {
                                    id: id,
                                    cd_horario: 0,
                                    summary: "",
                                    dt_hora_ini: hIni + ":" + mIni + ":00",
                                    dt_hora_fim: hFim + ":" + mFim + ":00",
                                    startTime: start,
                                    endTime: end,
                                    calendar: "Calendar1",
                                    id_dia_semana: start.getDate(),
                                    id_disponivel: true
                                };

                            } else {
                                for (var j = itens.length - 1; j >= 0; j--) {
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
                                    dt_hora_ini: hIni + ":" + mIni + ":00",
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
                        return null;
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
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemAluno', error);
                });
            }
            catch (e) {
                showCarregando();
                postGerarLog(e);
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
//Deprecate
function getPrimeiroHorarioDisponivelAluno(listHorario) {
    var retorno = null;
    $.each(listHorario, function (index, value) {
        if (value.calendar == "Calendar1") {
            retorno = value;
            return false;
        }
    });
    return retorno;
}

function mergeItemGridHorarios(id, startTime, endTime, cd_horario) {
    try {
        var calendar = dijit.byId('gridHorarios');
        var arraySemana = [];
        var itens = dijit.byId('gridHorarios').items;

        //Verificando se não está tirando horário que está ocupado
        var inicioHoras = (new Date(startTime)).getHours();
        var inicioMinutos = (new Date(startTime)).getMinutes();
        var fimHoras = (new Date(endTime)).getHours();
        var fimMinutos = (new Date(endTime)).getMinutes();
        var diaRegistro = (new Date(startTime));
        var inicioDia = diaRegistro.getDate();
        var diaRegistroFim = (new Date(endTime)).getDate();
        var ok = true;
        var endDia = new Date(endTime);

        var fimDia = endDia.getDate();
        //var diaRegistro = diaRegistro.getDate();

        if (inicioDia != fimDia) {
            calendar.store.remove(parseInt(id));
            return false;
        }
        var dataHorarioAluno = dijit.byId("gridAluno").horarioAluno;
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

        var id = gradeHorarios.items.length;

        if (id > 0) {
            quickSortObj(gradeHorarios.items, 'id');
            id = gradeHorarios.items[id - 1].id + 1;
        }

        if (!resolveuIntersecao) {
            if (start != null) {
                var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                var item = {
                    id: id,
                    cd_horario: parseInt(cd_horario),
                    summary: "",
                    startTime: start,
                    calendar: "Calendar1",
                    endTime: d,
                    dt_hora_ini: hIni + ":" + mIni + ":00",
                    dt_hora_fim: hFim + ":" + mFim + ":00",
                    id_dia_semana: start.getDate(),
                    id_disponivel: true
                };

                if (item != null)
                    calendar.store.add(item);
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function excluiItemHorario() {
    try {
        var gradeHorarios = dijit.byId('gridHorarios');
        if (hasValue(gradeHorarios)) {
            var store = gradeHorarios.items;
            if ((store.length > 0) && hasValue(gradeHorarios.selectedItems)) {
                apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
                for (var i = gradeHorarios.selectedItems.length - 1; i >= 0; i--) {

                    var itemSelected = gradeHorarios.selectedItems[i];
                    if (itemSelected.calendar != "Calendar2" && itemSelected.calendar != "Calendar5")
                        gradeHorarios.store.remove(itemSelected.id);
                }
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDeleteHorarioSeleciona);
                apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
                return false
            }
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirItemHorario() {
    try {
        apresentaMensagem('apresentadorMensagemAluno', null);
        var calendar = dijit.byId('gridHorarios');
        var timeFim = dijit.byId('timeFim');
        var timeIni = dijit.byId('timeIni');
        var arraySemana = dijit.byId('cbDias').value;
        var mensagensWeb = new Array();
        if (!hasValue(calendar)) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_AVISO, msgNaoConstruidoGradeHorario);
            apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
            return false
        }
        var itens = dijit.byId('gridHorarios').items;
        if (arraySemana.length <= 0) {
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotDiaSemanaSelect);
            apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
            return false
        }
        else
            if (dijit.byId("formSemana").validate()) {
                if ((timeFim.value.getMinutes() % 5 != 0 || timeIni.value.getMinutes() % 5 != 0) || validarItemHorarioManual(timeIni,timeFim)) {
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgNotInclItemHorario);
                    apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
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
                    if (!resolveuIntersecao) {
                        if (start != null) {
                            var hIni = start.getHours() > 9 ? start.getHours() : "0" + start.getHours();
                            var hFim = d.getHours() > 9 ? d.getHours() : "0" + d.getHours();
                            var mFim = d.getMinutes() > 9 ? d.getMinutes() : "0" + d.getMinutes();
                            var mIni = start.getMinutes() > 9 ? start.getMinutes() : "0" + start.getMinutes();

                            var item = {
                                id: id,
                                cd_horario: 0,
                                summary: "",
                                dt_hora_ini: hIni + ":" + mIni + ":00",
                                dt_hora_fim: hFim + ":" + mFim + ":00",
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
    }
    catch (e) {
        postGerarLog(e);
    }
}
//Deprecate
function blockedEndOpenHabilToglle(acao) {
    if (acao) {
        toglleTag(acao, 'tagHabilProf');
    } else {
        toglleTag(acao, 'tagHabilProf');
    }
}

function loadNaturezaPessoaPesquisa() {
    try {
        var statusStore = new dojo.store.Memory({
            data: [
            { name: "Todos", id: "0" },
            { name: "Física", id: "1" },
            { name: "Jurídica", id: "2" }
            ]
        });

        var tipoPessoa = new dijit.form.FilteringSelect({
            id: "tipoFunc",
            name: "tipoFunc",
            store: statusStore,
            value: "0",
            searchAttr: "name",
            style: "width:90px;"
        }, "tipoFunc");

        tipoPessoa.on("change", function () {
            try {
                if (this.get("value") == 0) {
                    dojo.byId("_cnpjCpf").value = "";
                    $('#_cnpjCpf').unmask();
                }
                dojo.byId("_cnpjCpf").readOnly = true;
                if (this.get("value") == 1) {
                    dojo.byId("_cnpjCpf").value = "";
                    dojo.byId("_cnpjCpf").readOnly = false;
                    $("#_cnpjCpf").mask("999.999.999-99");
                }
                if (this.get("value") == 2) {
                    dojo.byId("_cnpjCpf").value = "";
                    dojo.byId("_cnpjCpf").readOnly = false;
                    $("#_cnpjCpf").mask("99.999.999/9999-99")
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        });

        dijit.byId("tipoFunc").set("value", 1);
        toggleDisabled(dijit.byId("tipoFunc"), true);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadHorarioAluno(cdAluno) {
    try {
        showCarregando();
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getHorarioByEscolaForAluno?cdAluno=" + parseInt(cdAluno),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataHorario) {
            try {
                var menorHorario = null;
                var menorData = null;
                dataHorario = jQuery.parseJSON(dataHorario);
                if (hasValue(dataHorario.retorno) && dataHorario.retorno.length > 0) {
                    var codMaiorHorarioTurma = 0;
                    $.each(dataHorario.retorno, function (idx, val) {
                        codMaiorHorarioTurma += 1;
                        if (val.cd_registro != cdAluno) {
                            if (val.id_situacao == ATIVO || val.id_situacao == REMATRICULA )
                                val.calendar = "Calendar2";
                            if (val.id_situacao == SITUACAO_AGUARDANDO)
                                val.calendar = "Calendar5";
                        }
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
                    dijit.byId("gridAluno").horarioAluno = dataHorario.retorno.slice();

                    //Aciona uma thread para verificar se acabou de criar toda a grade horária e posicionar na primeira linha:
                    setTimeout(function () { posicionaPrimeiraLinhaHorarioAluno(dataHorario.retorno.length, menorData); }, 100);
                }
                criacaoGradeHorario(dataHorario.retorno);
            }
            catch (e) {
                postGerarLog(e);
                showCarregando();
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemAluno', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
        showCarregando();
    }
}

function posicionaPrimeiraLinhaHorarioAluno(tamanhoHorarios, menorData) {
    var gridHorarios = dijit.byId('gridHorarios');
    try {
        if (hasValue(gridHorarios) && hasValue(gridHorarios.items) && gridHorarios.items.length >= tamanhoHorarios)
            gridHorarios.columnView.set("startTimeOfDay", { hours: menorData.getHours(), duration: 500 });
        else
            setTimeout(function () { posicionaPrimeiraLinhaHorarioAluno(tamanhoHorarios, menorData); }, 100);
    }
    catch (e) {
        setTimeout(function () { posicionaPrimeiraLinhaHorarioAluno(tamanhoHorarios, menorData); }, 100);
    }
}

function formataHorarios(listHorarios) {
    $.each(listHorarios, function (idx, val) {
        //dojo.date.locale.parse("0"+val.DiaSemana +"/07/2012 " + val.fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' })
        val.endTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_fim, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' }),
        val.startTime = dojo.date.locale.parse("0" + val.id_dia_semana + "/07/2012 " + val.dt_hora_ini, { datePattern: "dd/MM/yyyy", timePattern: "HH:mm:ss", locale: 'pt-br' });
    });
}

function loadDataAluno(cdAluno) {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/aluno/getDataAluno?cdAluno=" + parseInt(cdAluno),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (dataAluno) {
            var data = jQuery.parseJSON(dataAluno);
            if (hasValue(data.retorno))
                setValueAluno(data.retorno, dojo.data.ObjectStore, dojo.store.Memory);
            else
                apresentaMensagem('apresentadorMensagemAluno', data);
        }, function (error) {
            apresentaMensagem('apresentadorMensagemAluno', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setValueAluno(aluno, ObjectStore, Memory) {
    try {
        dojo.byId("cdAluno").value = aluno.cd_aluno;
        dijit.byId("descObsAluno").set("value", aluno.txt_obs_pessoa);
        populaMidia(aluno.cd_midia, STATUS_TODOS);
        populaEscolaridade(aluno.cd_escolaridade, STATUS_TODOS);

        populaGridAlunoRestricao(aluno.cd_aluno, ObjectStore, Memory);

        if (hasValue(aluno.cd_prospect)) {
            dojo.byId('no_prospect').value = aluno.no_prospect;
            dojo.byId('cd_prospect').value = aluno.cd_prospect;
        }
        //Configura as grids:
        if (hasValue(aluno.MotivosMatricula) && aluno.MotivosMatricula.length > 0)
            dijit.byId("gridMotivoSim").setStore(new ObjectStore({ objectStore: new Memory({ data: aluno.MotivosMatricula }) }));

        dijit.byId("idPessoaAtiva").set("checked", aluno.id_aluno_ativo);
        dojo.byId("no_atendente").value = document.getElementById('nomeUsuario').innerText;

        if (hasValue(aluno.fichaSaudeAluno)) {
            populaFichaSaude(aluno.fichaSaudeAluno);
        }

        if (hasValue(aluno.Bolsa)) {
            dijit.byId("pc_bolsa").set('value', aluno.Bolsa.pc_bolsa);
            dojo.byId("dt_inicio_bolsa").value = aluno.Bolsa.dtaInicioBolsa;
            dojo.byId("dc_validade_bolsa").value = aluno.Bolsa.dc_validade_bolsa;
            dojo.byId("dt_comunicado_bolsa").value = aluno.Bolsa.dtaComunicadoBolsa;
            dijit.byId("pc_bolsa_material").set('value',aluno.Bolsa.pc_bolsa_material);
            dojo.byId("dt_cancelamento_bolsa").value = aluno.Bolsa.dtaCancelamentoBolsa;

            populaMotivoBolsa(aluno.Bolsa.cd_motivo_bolsa, STATUS_ATIVO);
            populaMotivoCancBolsa(aluno.Bolsa.cd_motivo_cancelamento_bolsa, STATUS_ATIVO);
        }
        else {
            populaMotivoBolsa(null, STATUS_ATIVO);
            populaMotivoCancBolsa(null, STATUS_ATIVO);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

// metodos crud Aluno
function pesquisaAluno(limparItens) {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            //Montando situações
            var situacoes = "";
            if (!hasValue(dijit.byId('situacao').value) || dijit.byId('situacao').value.length <= 0)
                situacoes = "100";
            else
                for (var i = 0; i < dijit.byId('situacao').value.length; i++) {
                    if (situacoes == "")
                        situacoes = dijit.byId('situacao').value[i];
                    else
                        situacoes = situacoes + "|" + dijit.byId('situacao').value[i];
                }

            myStore = Cache(
            JsonRest({
                target: Endereco() + "/api/aluno/getAlunoSearch?nome=" + encodeURIComponent(dojo.byId("_nome").value) + "&email=" + encodeURIComponent(dojo.byId("emailPesq").value) +
                                     "&status=" + dijit.byId("statusAluno").value + "&cnpjCpf=" + dojo.byId("_cnpjCpf").value + "&inicio=" + document.getElementById("inicioAluno").checked +
                                     "&cdSituacoes=" + situacoes + "&sexo=" + dijit.byId("nm_sexo").value +
                                     "&semTurma=" + dijit.byId("ckSemTurma").checked + "&movido=" + dijit.byId("ckMovidoSemContrato").checked + "&tipoAluno=" + dijit.byId("alunosClientes").value +
                                     "&matriculasem=" + dijit.byId("ckMatriculasemTurma").checked + "&matricula=false",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                idProperty: "cd_aluno"
            }), Memory({ idProperty: "cd_aluno" }));
            dataStore = new ObjectStore({ objectStore: myStore });
            var gridAluno = dijit.byId("gridAluno");
            if (limparItens) {
                gridAluno.itensSelecionados = [];
            }
            gridAluno.noDataMessage = msgNotRegEnc;
            gridAluno.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirPortalAluno(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectReg, null);
        else {
            abrePopUpURLPortal(itensSelecionados[0].cd_aluno);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            
            var gridAluno = dijit.byId('gridAluno');
            destroyCreateGridHorario();
            destroyCreateGridFollowUp();
            destroyCreateGridTabMatricula();
            setarTabCadAluno();
            //limparCadPessoaFK();
            apresentaMensagem('apresentadorMensagem', '');
            gridAluno.itemSelecionado = montaObjetoAluno(itensSelecionados[0]);
            keepValues(null, dijit.byId('gridAluno'), true);
            IncluirAlterar(0, 'divAlterarAluno', 'divIncluirAluno', 'divExcluirAluno', 'apresentadorMensagemAluno', 'divCancelarAluno', 'divLimparAluno');
            //dijit.byId("tabContainer").resize();
            dijit.byId("cad").show();
        }
        document.getElementById('parametroPospectValido').value = 'false';
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaObjetoAluno(item) {
    var objeto =
        {
            cd_aluno: item.cd_aluno,
            cd_pessoa_aluno: item.cd_pessoa_aluno,
            no_pessoa: item.no_pessoa,
            dt_cadastramento: item.dt_cadastramento,
            dc_reduzido_pessoa: item.dc_reduzido_pessoa,
            nm_cpf: item.nm_cpf,
            id_aluno_ativo: item.id_aluno_ativo,
            ext_img_pessoa: item.ext_img_pessoa,
            no_atividade: item.no_atividade,
            ativExtra: item.ativExtra,
            cd_pessoa: item.cd_pessoa,
            txt_obs_pessoa: item.txt_obs_pessoa
        };
    return objeto;
}

function eventoRemoverAluno(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para excluir.', null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarAlunos(itensSelecionados); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function keepValues(val, grid, ehLink) {
    dijit.byId('btProspect').setAttribute('disabled', true);
    var value = grid.itemSelecionado;
    var linkAnterior = document.getElementById('link');
    var retorno = true;

    if (!hasValue(ehLink, true))
        ehLink = eval(linkAnterior.value);
    linkAnterior.value = ehLink;
    if (!hasValue(value) && hasValue(val))
        value = val;

    var cdAlunoVal = hasValue(value) && value.cd_aluno > 0 ? value.cd_aluno : dojo.byId("cdAluno").value;
    var cdPessoaAlunoVal = hasValue(value) && value.cd_pessoa_aluno > 0 ? value.cd_pessoa_aluno : dojo.byId("cd_pessoa").value;

    limparAluno();
    setarTabCadAluno();

    if (hasValue(value) || (hasValue(cdAlunoVal) && hasValue(cdPessoaAlunoVal))) {
        var papelRelac = new Array();
        papelRelac[0] = RELACENTREPESSOAS;
        // Popula componentes da pessoa:
        showPessoaFK(PESSOAFISICA, cdPessoaAlunoVal, papelRelac, loadDataAluno(cdAlunoVal));
        //showFoto(value);
        //loadDataAluno(value.cd_aluno);
        retorno = false;
    }
    return retorno;
}

function salvarAluno() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window", "dojo/date"], function (dom, domAttr, xhr, ref, windowUtils, date) {
        try {
            var pessoaFisicaUI = null;
            var aluno = null;

            if (!validarAlunoCadastro(windowUtils, 1)) {
                setarTabCadAluno();
                return false;
            }
            aluno = montarDadosAlunoForPost(dom, domAttr, date);
            if (!aluno) return false;
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/aluno/PostInsertAluno",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(aluno)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data)
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var gridName = 'gridAluno';
                        var grid = dijit.byId(gridName);

                        apresentaMensagem('apresentadorMensagem', data);
                        data = data.retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        insertObjSort(grid.itensSelecionados, "cd_aluno", itemAlterado);
                        dijit.byId("cad").hide();
                        buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_aluno', 'selecionaTodos', ['pesquisarAluno', 'relatorioAluno'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        if (hasValue(grid) && hasValue(grid.store.objectStore.data))
                            setGridPagination(grid, itemAlterado, "cd_aluno");
                    } else
                        apresentaMensagem('apresentadorMensagemAluno', data);
                    showCarregando();
                }
                catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAluno', error);
            });
        }
        catch (e) {
            showCarregando();
            postGerarLog(e);
        }
    });
}

function alterarAluno() {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref", "dojo/window", "dojo/date"], function (dom, domAttr, xhr, ref, windowUtils, date) {
        try {
            dijit.byId('cbEscolaridade').required = true;
            dijit.byId('atividadePrincipal').required = true;
            dijit.byId('dtaNasci').required = true;
            dijit.byId('estadoCivil').required = true;

            //if (!validarCPF("#cpf", "apresentadorMensagemAluno"))
            //    return false;
            if (!validarAlunoCadastro(windowUtils, 2)) {
                setarTabCadAluno();
                return false;
            }

            aluno = montarDadosAlunoForPost(dom, domAttr, date);
            if (!aluno) return false;
            showCarregando();
            xhr.post({
                url: Endereco() + "/api/aluno/PostUpdateAluno",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(aluno)
            }).then(function (data) {
                try {
                    data = jQuery.parseJSON(data);
                    if (!hasValue(data.erro)) {
	                    var itemAlterado = data.retorno;
	                    var gridName = 'gridAluno';
	                    var grid = dijit.byId(gridName);
	                    apresentaMensagem('apresentadorMensagem', data);
	                    data = data.retorno;
	                    if (!hasValue(grid.itensSelecionados))
		                    grid.itensSelecionados = [];
	                    removeObjSort(grid.itensSelecionados, "cd_aluno", dojo.byId("cdAluno").value);
	                    insertObjSort(grid.itensSelecionados, "cd_aluno", itemAlterado);
	                    dijit.byId("cad").hide();
	                    buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_aluno', 'selecionaTodos', ['pesquisarAluno', 'relatorioAluno'], 'todosItens');
	                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
	                    setGridPagination(grid, itemAlterado, "cd_aluno");  
                    }
                    else
                        apresentaMensagem('apresentadorMensagemAluno', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemAluno', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
};

function deletarAlunos(itensSelecionados) {
    require(["dojo/dom", "dojo/dom-attr", "dojo/_base/xhr", "dojox/json/ref"], function (dom, domAttr, xhr, ref) {
        try {
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
                if (dojo.byId('cdAluno').value != 0)
                    itensSelecionados = [{
                        cd_aluno: dom.byId("cdAluno").value
                    }];
            xhr.post({
                url: Endereco() + "/api/aluno/PostdeleteAluno",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try {
                    var todos = dojo.byId("todosItens_label");
                    apresentaMensagem('apresentadorMensagem', data);
                    dijit.byId("cad").hide();
                    dijit.byId("_nome").set("value", '');
                    // Remove o item dos itens selecionados:
                    if (itensSelecionados != null)
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridAluno').itensSelecionados, "cd_aluno", itensSelecionados[r].cd_aluno);
                    pesquisaAluno(false);
                    // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                    dijit.byId("pesquisarAluno").set('disabled', false);
                    dijit.byId("relatorioAluno").set('disabled', false);

                    if (hasValue(todos))
                        todos.innerHTML = "Todos Itens";
                }
                catch (e) {
                    postGerarLog(e);
                }

            },
            function (error) {
                if (!hasValue(dojo.byId("cad").style.display))
                    apresentaMensagem('apresentadorMensagemAluno', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);
            });
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function validarAlunoCadastro(windowUtils, acao) {
    var telefone = dijit.byId("telefone");
    var email = dijit.byId("email");
    var pc_bolsa_material = dijit.byId("pc_bolsa_material");
    var validado = true;
    var gridMotivoSim = dijit.byId('gridMotivoSim');

    gridMotivoSim.store.save();
    var itens = gridMotivoSim.store.objectStore.data;
    if (!hasValue(itens) || itens.length <= 0) {
        var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgObrMotMatricula);
        apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
        dijit.byId("tagMotivosSim").set("open", true);

        validado = false;
    }

    if (dijit.byId("pc_bolsa").value > 100) {
	    var mensagensWeb = new Array();
        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroBolsaMaiorQueCem);
	    apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);
	    dijit.byId("tagAluno").set("open", true);

	    validado = false;
    }

    if (!dijit.byId("formAluno").validate()) {
        dijit.byId("tagAluno").set("open", true);
        validado = false;
    }

    if (!mostrarMensagemCampoValidado(windowUtils, dijit.byId("dtaNasci"))) {
        dijit.byId("panelPessoaFisica").set("open", true);
        validado = false;
    }
    if (!mostrarMensagemCampoValidado(windowUtils, dijit.byId("estadoCivil"))) {
        dijit.byId("panelPessoaFisica").set("open", true);
        validado = false;
    }

    if (!mostrarMensagemCampoValidado(windowUtils, dijit.byId("atividadePrincipal"))) {
        dijit.byId("panelPessoaFisica").set("open", true);
        validado = false;
    }
    if (!hasValue(dojo.byId("celular").value) && !hasValue(dojo.byId("telefone").value)) {
        dijit.byId("telefone").set("required", true);
        dijit.byId("celular").set("required", true);
        var celular = dijit.byId("celular");
        if (!celular.validate()) {
            dijit.byId("panelEndereco").set("open", true);
            mostrarMensagemCampoValidado(windowUtils, celular);
            validado = false;
        }
        if (!telefone.validate()) {
            dijit.byId("panelEndereco").set("open", true);
            mostrarMensagemCampoValidado(windowUtils, telefone);
            validado = false;
        }
    } else {
        dijit.byId("telefone").set("required", false);
        dijit.byId("celular").set("required", false);
    }

    //}
    if (!validateCadPessoaFK(windowUtils))
        validado = false;
    var cpfPessoaRelc = dojo.byId("cdPessoaCpf").value;
    cpfPessoaRelc = cpfPessoaRelc == 0 ? null : cpfPessoaRelc;

    if (!hasValue(cpfPessoaRelc) && hasValue(dojo.byId("cpf").value)) {
        if (!validarCPF("#cpf", "apresentadorMensagemAluno")) {
            validado = false;
            //dojo.byId("cpf").focus();
        }
    }
    if (!email.validate()) {
        mostrarMensagemCampoValidado(windowUtils, email);
        //email.focus();
        validado = false;
    }

    if (!pc_bolsa_material.validate()) {
        mostrarMensagemCampoValidado(windowUtils, pc_bolsa_material);
        //email.focus();
        validado = false;
    }

    return validado;
}

function extistsPessoAlunoByCpf() {
    var mensagensWeb = new Array();
    if ($("#cpf").val()) {
        dojo.xhr.get({
            url: Endereco() + "/api/aluno/getExistAlunoOrPessoaFisicaByCpfAluno?cpf=" + $("#cpf").val(),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                //(dojo.byId("descApresMsg").value, data);
                if (hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value != "0")
                    return true;
                else
                if (data.retorno != null && data.retorno.pessoaFisica != null) {
                    caixaDialogo(DIALOGO_CONFIRMAR, data.MensagensWeb[0].mensagem, function executaRetorno() {
                        setarValuePessoaFisica(data.retorno);
                        dijit.byId('descObsAluno').set('value', data.retorno.pessoaFisica.txt_obs_pessoa);
                    });
                }
                else
                    apresentaMensagem("apresentadorMensagemAluno", data.erro);
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            apresentaMensagem("apresentadorMensagemAluno", error);
        });
    }
}


function existsEmailPessoa(email) {
    var mensagensWeb = new Array();
    if ($("#email").val()) {
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getExistsEmailReturnPessoa?email=" + $("#email").val(),
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                // apresentaMensagem(dojo.byId("descApresMsg").value, data);
                data = jQuery.parseJSON(data);
                if (data.retorno != null && data.retorno.cd_prospect != null)
                    if (data.retorno.id_prospect_ativo) {
                        populaDadosProspect(data.retorno);
                        dojo.byId('no_prospect').value = data.retorno.PessoaFisica.no_pessoa;
                        dojo.byId('cd_prospect').value = data.retorno.cd_prospect;
                    } else {
                        var mensagensWeb = new Array();
                        mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroRegInativo + dijit.byId("email").value + ".");
                        apresentaMensagem(dojo.byId("descApresMsg").value, mensagensWeb);
                        dijit.byId("email").set("value", '');
                        return false;
                    }

                var prospect = data.retorno;
                //Abre a grade de follow up:
                var tabs = dijit.byId("tabContainer");
                var pane = dijit.byId("tabFollow");
                tabs.selectChild(pane);

                //Preenche os dados:
                var gridFollowUp = dijit.byId("gridFollowUp");
                if (hasValue(prospect) && hasValue(prospect.ProspectFollowUp) && prospect.ProspectFollowUp.length > 0)
                    if (hasValue(gridFollowUp))
                        gridFollowUp.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: prospect.ProspectFollowUp, idProperty: "nroOrdem" }) }));
                    else
                        criarGridFollowUp(null, prospect.ProspectFollowUp);
                //Volta para a primeira aba:
                tabs = dijit.byId("tabContainer");
                pane = dijit.byId("tabAluno");
                tabs.selectChild(pane);
                document.getElementById('parametroPospectValido').value = 'true';
                apresentaMensagem(dojo.byId("descApresMsg").value, null);
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

function getExistsCpfENome(email, nomPessoa, cd_pessoa_cpf) {
    var mensagensWeb = new Array();
    
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getExistsCpfENome?email=" + email + "&nome=" + nomPessoa + "&cd_pessoa_cpf=" + cd_pessoa_cpf,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);

                if (data.retorno != null && data.retorno.pessoaFisica != null) {
                    caixaDialogo(DIALOGO_CONFIRMAR,
                        data.MensagensWeb[0].mensagem,
                        function executaRetorno() {
                            setarValuePessoaFisica(data.retorno);
                            dijit.byId('descObsAluno').set('value', data.retorno.pessoaFisica.txt_obs_pessoa);
                        });
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

// fim metodo crud Aluno
function getNameFoto(files) {
    if (hasValue(files.name))
        return files.name;
    return null;
}

function limparFormAluno() {
    try {
        dijit.byId("email").reset();
        dojo.byId("cdAluno").value = 0;
        document.getElementById('cd_prospect').value = 0;
        document.getElementById('cdPessoaProject').value = 0;
        dojo.byId('no_prospect').value = "";
        clearForm("formAluno");
        dojo.byId("cd_ficha_saude").value = 0;
        clearForm("FormFichaSaude");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparAluno() {
    try {
        limparCadPessoaFK();
        limparFormAluno();
        dijit.byId('cbDias').setStore(dijit.byId('cbDias').store, []);
        dojo.byId("no_atendente").value = document.getElementById('nomeUsuario').innerText;
        dijit.byId("descObsAluno").set("value", "");
        var gridEnderecos = dijit.byId("gridEnderecos");
        if (hasValue(gridEnderecos))
            gridEnderecos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));

        var gridContatos = dijit.byId("gridContatos");
        if (hasValue(gridContatos))
            gridContatos.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));

        var gridMotivoSim = dijit.byId("gridMotivoSim");
        if (hasValue(gridMotivoSim))
            gridMotivoSim.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));

        var gridAlunoRestricao = dijit.byId("gridAlunoRestricao");
        if (hasValue(gridAlunoRestricao))
	        gridAlunoRestricao.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));

        if (hasValue(dijit.byId("gridHorarios"))) {
            if (hasValue(dojo.byId("cdAluno").value) && dojo.byId("cdAluno").value > 0)
                limparNewsHorarios();
            else
                limparTodosHorarios();
        }
        if (hasValue(dijit.byId("gridFollowUp")))
            limparGridFollowUp(dojo.byId("cdAluno").value, dojo.store.Memory, dojo.data.ObjectStore);
        dijit.byId("tagMotivosSim").set("open", false);
        dijit.byId("tagBolsa").set("open", false);
        dijit.byId("tagAlunoRestricao").set("open", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparTodosHorarios() {
    var gradeHorarios = dijit.byId('gridHorarios');
    var store = gradeHorarios.items;
    if (store.length > 0) {
        for (var i = store.length - 1; i >= 0; i--) {
            if (gradeHorarios.items[i].id >= 0)
                gradeHorarios.store.remove(store[i].id);
        }

    }
}

function limparNewsHorarios() {
    var gradeHorarios = dijit.byId('gridHorarios');
    var store = gradeHorarios.items;
    if (store.length > 0) {
        for (var i = store.length - 1; i >= 0; i--) {
            if (!hasValue(gradeHorarios.store.data[i].cd_horario) && !gradeHorarios.store.data[i].cd_horario > 0)
                gradeHorarios.store.remove(store[i].id);
        }

    }
}

function setarTabCadAluno() {
    var tabs = dijit.byId("tabContainer");
    var pane = dijit.byId("tabAluno");
    tabs.selectChild(pane);
}

function retornarMtvMatFK() {
    try {
        var gridMotivoSim = dijit.byId("gridMotivoSim");
        var gridMotivoMatricluaFK = dijit.byId("gridMotivoMatricluaFK");
        var valido = true;
        if (hasValue(jQuery.parseJSON(dojo.byId("ehSelectGradeMMatriculaFK").value)) && jQuery.parseJSON(dojo.byId("ehSelectGradeMMatriculaFK").value)) {
            var dataMotivoMatricluaFK = dijit.byId("gridMotivoMatricluaFK").selection.getSelected()[0];
            insertObjSort(gridMotivoSim.store.objectStore.data, "cd_motivo_matricula", { cd_aluno_motivo_matricula: 0, cd_tipo_avaliacao: 0, cd_motivo_matricula: dataMotivoMatricluaFK.cd_motivo_matricula, dc_motivo_matricula: dataMotivoMatricluaFK.dc_motivo_matricula });
            gridMotivoSim.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridMotivoSim.store.objectStore.data }) }));
        } else {
            if (!hasValue(gridMotivoMatricluaFK.itensSelecionados) || gridMotivoMatricluaFK.itensSelecionados.length <= 0) {
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                valido = false;
            } else {
                apresentaMensagem('apresentadorMensagemAluno', null);
                var storeGridCurso = (hasValue(gridMotivoSim) && hasValue(gridMotivoSim.store.objectStore.data)) ? gridMotivoSim.store.objectStore.data : [];
                if (storeGridCurso != null && storeGridCurso.length > 0) {
                    $.each(gridMotivoMatricluaFK.itensSelecionados, function (idx, value) {
                        //gridMotivoSim.store.newItem(value);
                        insertObjSort(gridMotivoSim.store.objectStore.data, "cd_motivo_matricula", { cd_aluno_motivo_matricula: 0, cd_tipo_avaliacao: 0, cd_motivo_matricula: value.cd_motivo_matricula, dc_motivo_matricula: value.dc_motivo_matricula });
                    });
                    gridMotivoSim.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: gridMotivoSim.store.objectStore.data }) }));
                    //gridMotivoSim.update();
                } else {
                    var dados = [];
                    $.each(gridMotivoMatricluaFK.itensSelecionados, function (index, val) {
                        insertObjSort(dados, "cd_motivo_matricula", { cd_aluno_motivo_matricula: 0, cd_tipo_avaliacao: 0, cd_motivo_matricula: val.cd_motivo_matricula, dc_motivo_matricula: val.dc_motivo_matricula });
                    });
                    //slice(0)
                    gridMotivoSim.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));
                }
            }
        }
        if (!valido)
            return false;
        dojo.byId("ehSelectGradeMMatriculaFK").value = false;
        dijit.byId("proMotivoMatricula").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}
//Deprecate
function montarElementoEmail() {
    try {
        var div = "";
        $("#email").attr("id", "emailSecund");
        $('<table>').attr('id', 'tableEmail').addClass("Table").appendTo('#tdNatureza');

        $('<tr>').attr('id', 'trEmail').appendTo('#tableEmail');
        $('<td>').attr('id', 'tdInpEmail').addClass("dojoTextBoxIECorrect").css("width", "90%").css("padding-top", "4px").appendTo('#trEmail');
        $('<td>').attr('id', 'tdLinkEmail').addClass("tdRight").css("width", "2%").appendTo('#trEmail');
        $("<input>").attr('id', 'email').attr('data-dojo-type', 'dijit/form/ValidationTextBox').attr('required', 'true').attr("onblur", "isEmail(this.value)")
               .attr('data-dojo-props', 'validator:dojox.validate.isEmailAddress, invalidMessage:"E-mail com formato inválido!"').attr('maxlength', '64').css("width", "100%").appendTo("#tdInpEmail");
        $("<input>").attr('id', 'enviarEmailAluno').appendTo("#tdLinkEmail");
        document.getElementById('labelNatureza').innerHTML = 'E-mail:';
        $("#tdNatureza").removeClass("dojoTextBoxIECorrect");
        showP("emailTagEndereco", false);
        showP("naturezaPessoa", false);
    }
    catch (e) {
        postGerarLog(e);
    }
}

function criarGridFollowUp(cdUsuario, dataProspect) {
    if (!hasValue(dijit.byId('gridFollowUp'))) {
        require(["dojox/grid/EnhancedGrid", "dojo/store/JsonRest", "dojo/data/ObjectStore", "dojo/store/Memory", "dojo/_base/xhr", "dojo/store/Cache", "dojox/grid/enhanced/plugins/NestedSorting"
        ], function (EnhancedGrid, JsonRest, ObjectStore, Memory, xhr, Cache, NestedSorting) {
            try {
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null, idProperty: "nroOrdem" }) });
                if (hasValue(dijit.byId("gridFollowUp")))
                    destroyCreateGridFollowUp();
                var gridFollowUp = new EnhancedGrid({
                    store: dataStore,
                    structure:
                      [
                        { name: "<input id='selecionaTodosFollowUp' style='display:none'/>", field: "selecionadoFollowUp", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxFollowUp },
                        { name: "Data/Hora", field: "dta_follow_up", width: "20%" },
                        { name: "Atendente", field: "no_usuario", width: "15%" },
                        { name: "Assunto", field: "_dc_assunto", width: "50%" },
                        { name: "", field: "dt_follow_up", hidden: true },
                        { name: "", field: "nroOrdem", hidden: true }
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
                }, "gridFollowUp");

                gridFollowUp.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex,
                                item = this.getItem(idx),
                                store = this.store;
                        dijit.byId("gridFollowUp").itemSelecionado = item;

                        if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
                            montaFollowUpFK(function () {
                                dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                                    configuraLayoutFollowUp(e);
                                });
                                populaFollowUpAluno(function () { preparaKeepValues(); });

                                montaFollowUpFKPersonalizadoAluno();
                            });
                        else
                            populaFollowUpAluno(function () { preparaKeepValues(); });
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridFollowUp, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        try {
                            if (hasValue(dojo.byId('selecionaTodosFollowUp')) && dojo.byId('selecionaTodosFollowUp').type == 'text')
                                setTimeout("configuraCheckBox(false, 'nroOrdem', 'selecionadoFollowUp', -1, 'selecionaTodosFollowUp', 'selecionaTodosFollowUp', 'gridFollowUp')", gridFollowUp.rowsPerPage * 3);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    });
                });

                gridFollowUp.canSort = function (col) { return Math.abs(col) != 1; };
                gridFollowUp.startup();
                if (cdUsuario != null && cdUsuario > 0)
                    xhr.get({
                        preventCache: true,
                        handleAs: "json",
                        url: Endereco() + "/api/secretaria/GetFollowUpAluno?cdAluno=" + parseInt(cdUsuario),
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }).then(function (dataFollowUp) {
                        try {
                            dataFollowUp = jQuery.parseJSON(dataFollowUp);
                            if (hasValue(dataFollowUp) &&
	                            dataFollowUp["retorno"] != undefined &&
                                dataFollowUp["retorno"] != null) {

	                            dataFollowUp = dataFollowUp.retorno;
	                            if (dataFollowUp != null && dataFollowUp.length > 0) {
		                            var dataStore = new ObjectStore({ objectStore: new Memory({ data: dataFollowUp, idProperty: "nroOrdem" }) });
		                            gridFollowUp.setStore(dataStore);
	                            }
                            }
                            
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    function (error) {
                        apresentaMensagem('apresentadorMensagemAluno', error);
                    });
                else if (hasValue(dataProspect))
                    gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: dataProspect, idProperty: "nroOrdem" }) }));
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    }
}

function configuraLayoutFollowUp(desabilitado) {
    if (desabilitado) {
        dijit.byId('cadAcaoFollowUpFK').set('disabled', true);
        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', true);
        dijit.byId('mensagemFollowUppartial').set('disabled', true);

        dijit.byId('ckLidoFollowUpFK').set('checked', true);
    }
    else {
        dijit.byId('cadAcaoFollowUpFK').set('disabled', false);
        dijit.byId('dtaProxContatoFollowUpFK').set('disabled', false);
        dijit.byId('mensagemFollowUppartial').set('disabled', false);
    }
}

function eventoEditarFollowUp() {
    try {
        var gridFollowUp = dijit.byId("gridFollowUp");
        var itensSelecionados = gridFollowUp.itensSelecionados;
        apresentaMensagem('apresentadorMensagemCadFollowUpPartial', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            gridFollowUp.itemSelecionado = gridFollowUp.itensSelecionados[0];

            if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
                montaFollowUpFK(function () {
                    dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                        configuraLayoutFollowUp(e);
                    });
                    populaFollowUpAluno(function () { preparaKeepValues(); });

                    montaFollowUpFKPersonalizadoAluno();
                });
            else
                populaFollowUpAluno(function () { preparaKeepValues(); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function montaFollowUpFKPersonalizadoAluno() {
    dijit.byId('cancelarFollowUpPartial').on('click', function () {
        populaFollowUpAluno(function () { preparaKeepValues(); });
    });
    dijit.byId('btnLimparFollowUpFK').on('click', function () {
        dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomPessoa').value;
        dijit.byId('btnAlunoProspectFK').set('disabled', true);
    });
    dijit.byId('btnFecharFollowUpPartial').on('click', function () {
        dijit.byId('cadFollowUp').hide();
    });
}

function preparaKeepValues() {
    IncluirAlterar(0, 'divAlterarFollowUpPartial', 'divIncluirFollowUpPartial', 'divExcluirFollowUpPartial', 'apresentadorMensagemCadFollowUpPartial', 'divCancelarFollowUpPartial', 'divClearFollowUpPartial');
    limparCadFollowUpPartial();
    findComponentesNovoFollowUpPartial(function () {
        dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomPessoa').value;
        dijit.byId('btnAlunoProspectFK').set('disabled', true);
        dijit.byId('ckResolvidoFollowUpFK').set('disabled', false);
        showP('lblLido', true);
        dijit.byId('ckLidoFollowUpFK').set('style', 'display:block');

        dijit.byId("cadFollowUp").show();
        keepValuesFollowUp();
    });
}

function keepValuesFollowUp() {
    dojo.ready(function () {
        var CompckLidoFollowUpFK = dijit.byId('ckLidoFollowUpFK');
        var item = dijit.byId("gridFollowUp").itemSelecionado

        if (hasValue(item.dta_follow_up))
            dojo.byId('dtaCadastroFollowUpFK').value = item.dta_follow_up;

        if (hasValue(item.cd_usuario_destino)) {
	        dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value = item.cd_usuario_destino;
	        dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").set("value", item.no_usuario_destino);

	        dijit.byId("limparCadProspectAlunoUsuarioDestinoFollowUpFK").set("disabled", false);
        }

        dijit.byId('mensagemFollowUppartial').set('value', item.dc_assunto);
        dijit.byId('cadAcaoFollowUpFK').set('value', item.cd_acao_follow_up);
        dojo.byId('dtaProxContatoFollowUpFK').value = item.dta_proximo_contato;
        dojo.byId('nroOrdem').value = item.ordem;
        dojo.byId('cd_follow_up_partial').value = item.cd_follow_up;
        if (hasValue(item.id_tipo_atendimento) && item.id_tipo_atendimento > 0)
            dijit.byId("cadTipoAtendimento").set("value", item.id_tipo_atendimento);
        CompckLidoFollowUpFK._onChangeActive = false;
        CompckLidoFollowUpFK.set("value", item.id_follow_lido);
        CompckLidoFollowUpFK._onChangeActive = true;
        if (hasValue(item.id_follow_resolvido))
            dijit.byId('ckResolvidoFollowUpFK').set('checked', item.id_follow_resolvido);
        configuraLayoutFollowUp(item.id_follow_resolvido);
    });
}

function formatCheckBoxFollowUp(value, rowIndex, obj) {
    try {
        var gridName = 'gridFollowUp';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosFollowUp');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nroOrdem", grid._by_idx[rowIndex].item.cd_follow_up);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nroOrdem', 'selecionadoFollowUp', -1, 'selecionaTodosFollowUp', 'selecionaTodosFollowUp', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'nroOrdem', 'selecionadoFollowUp', " + rowIndex + ", '" + id + "', 'selecionaTodosFollowUp', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirFKProspectFollowUpPartialPersonalizadoAluno() {
    try {
        montarFKProspect(function () {
            dijit.byId('tipoPesquisaFKProspect').set('value', PESQUISARALUNO);
            dijit.byId('tipoPesquisaFKProspect').set('disabled', true);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function populaFollowUpAluno(p_funcao) {
    try {
        //Personaliza a tela da FK de follow up para a tela de aluno:
        dijit.byId("cadTipoFollowUpFK").set("value", PROSPECTALUNO);
        showP('trTipoFollowUpFK', false);
        dojo.byId('trInternoUserFollowUpFK').style.display = 'none';
        dojo.byId('trInternoAdmFollowUpFK').style.display = 'none';
        dojo.byId('trProspectAlunoFollowUpFK').style.display = '';
        dojo.byId('trEmailTelefoneProspectAluno').style.display = 'none';
        showP('trProximoContatoFollowUpFK', true);
        dojo.byId('trMasterFollowUp').style.display = 'none';
        dojo.byId('trResolvidoLidoFollowUp').style.display = '';
        dijit.byId("cadNomeUsuarioOrigFollowUpFK").set("required", false);
        dijit.byId("descAlunoProspectFollowUpFK").set("required", true);
        dijit.byId("cadNomeUsuarioAdmOrgFollowUpFK").set("required", false);
        dijit.byId("cadNomeUsuarioDestinoFollowUpFK").set("required", false);

        dijit.byId('btnIncluirFollowUpPartial').set('label', "Incluir");
        dijit.byId('alterarFollowUpPartial').set('label', "Alterar");

        dijit.byId('panePesqGeralFollowUpFK').set('title', "Aluno");
        alterarVisibilidadeEscolas(false);

        var btnAlunoProspectFK = dijit.byId("btnAlunoProspectFK");
        if (hasValue(btnAlunoProspectFK.handler))
            btnAlunoProspectFK.handler.remove();
        btnAlunoProspectFK.handler = btnAlunoProspectFK.on("click", function (e) {
            abrirFKProspectFollowUpPartialPersonalizadoAluno();
        });

        var btnIncluirFollowUpPartial = dijit.byId("btnIncluirFollowUpPartial");
        if (hasValue(btnIncluirFollowUpPartial.handler))
            btnIncluirFollowUpPartial.handler.remove();
        btnIncluirFollowUpPartial.handler = btnIncluirFollowUpPartial.on("click", function () {
            incluirLinhaFollowUp();
        });

        var alterarFollowUpPartial = dijit.byId("alterarFollowUpPartial");
        if (hasValue(alterarFollowUpPartial.handler))
            alterarFollowUpPartial.handler.remove();
        alterarFollowUpPartial.handler = alterarFollowUpPartial.on("click", function () {
            alterarLinhaFollowUp();
        });

        var deleteFollowUpPartial = dijit.byId("deleteFollowUpPartial");
        if (hasValue(deleteFollowUpPartial.handler))
            deleteFollowUpPartial.handler.remove();
        deleteFollowUpPartial.handler = deleteFollowUpPartial.on("click", function () {
            removerLinhaFollowUp();
        });
        
        if(hasValue(p_funcao))
            p_funcao.call();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function removerLinhaFollowUp() {
    try {
        var gridFollowUp = dijit.byId("gridFollowUp");
        var storeFollowUp = gridFollowUp.store.objectStore.data;
        quickSortObj(storeFollowUp, 'nroOrdem');
        var indice = binaryObjSearch(storeFollowUp, "nroOrdem", gridFollowUp.itemSelecionado.nroOrdem);

        //Verifica se o usuário tem permissão para remover:
        if (!hasValue(gridFollowUp.itemSelecionado.no_usuario) || !hasValue(document.getElementById('nomeUsuario').innerText)
                || gridFollowUp.itemSelecionado.no_usuario.trim() != document.getElementById('nomeUsuario').innerText.trim()) {
            var mensagensWeb = new Array();

            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPermissaoAlterarFollowUp);
            
            if(dijit.byId('cadFollowUp').open)
                apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            else
                apresentaMensagem('apresentadorMensagemAluno', mensagensWeb);

            return false;
        }
        require([
           "dojo/data/ObjectStore",
           "dojo/store/Memory"
        ], function (ObjectStore, Memory) {
            //Remove o item da grade:
            storeFollowUp.splice(indice, 1);
            gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));
        },
        function (error) {
            apresentaMensagem('apresentadorMensagemAluno', error);
        });
        //dijit.byId("gridFollowUp").update();
        dijit.byId('cadFollowUp').hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarLinhaFollowUp() {
    try {
        //Verifica se o campo de mensagem está preenchido:
        if (!hasValue(dijit.byId('mensagemFollowUppartial').value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMensagemFollowUpObrig);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        //Verifica se o usuário pode alterar:
        if (!hasValue(dijit.byId("gridFollowUp").itemSelecionado.no_usuario) || !hasValue(document.getElementById('nomeUsuario').innerText)
                || dijit.byId("gridFollowUp").itemSelecionado.no_usuario.trim() != document.getElementById('nomeUsuario').innerText.trim()) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroPermissaoAlterarFollowUp);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        require([
           "dojo/data/ObjectStore",
           "dojo/store/Memory"
        ], function (ObjectStore, Memory) {
            dojo.xhr.post({
                url: Endereco() + "/util/PostDataHoraCorrente",
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: dojox.json.ref.toJson()
            }).then(function (data) {
                try {
                    if (hasValue(data) && hasValue(data.retorno)) {
                        var dataCorrente = data.retorno;
                    var gridFollowUp = dijit.byId("gridFollowUp");
                    var dataFollowup = new Date(dataCorrente.dataIngles);
                    var storeFollowUp;

                    storeFollowUp = gridFollowUp.store.objectStore.data;
                    quickSortObj(storeFollowUp, 'nroOrdem');
                    var indice = binaryObjSearch(storeFollowUp, "nroOrdem", gridFollowUp.itemSelecionado.nroOrdem);

                        storeFollowUp[indice].cd_follow_up_partial = dojo.byId('cd_follow_up_partial').value;
                        storeFollowUp[indice].no_usuario = document.getElementById('nomeUsuario').innerText;
                        storeFollowUp[indice].dta_follow_up = dataCorrente.dataPortugues;
                        storeFollowUp[indice].dt_follow_up = dataFollowup;
                    storeFollowUp[indice].dc_assunto = dijit.byId("mensagemFollowUppartial").value;
                    storeFollowUp[indice]._dc_assunto = hasValue(dijit.byId("mensagemFollowUppartial").value) && dijit.byId("mensagemFollowUppartial").value.indexOf('<') < 0
                            && dijit.byId("mensagemFollowUppartial").value.indexOf('>') < 0 ? dijit.byId("mensagemFollowUppartial").value : "...";
                    storeFollowUp[indice].cd_acao_follow_up = dijit.byId('cadAcaoFollowUpFK').value;
                    storeFollowUp[indice].dt_proximo_contato = hasValue(dojo.byId("dtaProxContatoFollowUpFK").value) ? dojo.date.locale.parse(dojo.byId("dtaProxContatoFollowUpFK").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null; //dojo.byId('dtaProxContatoFollowUpFK').value;
                    storeFollowUp[indice].dta_proximo_contato = dojo.byId('dtaProxContatoFollowUpFK').value;
                    storeFollowUp[indice].nroOrdem = storeFollowUp[storeFollowUp.length - 1].nroOrdem + 1;
                    storeFollowUp[indice].id_follow_lido = dijit.byId('ckLidoFollowUpFK').checked;
                    storeFollowUp[indice].id_follow_resolvido = dijit.byId('ckResolvidoFollowUpFK').checked;
                    storeFollowUp[indice].id_tipo_atendimento = hasValue(dijit.byId("cadTipoAtendimento").value) ? dijit.byId("cadTipoAtendimento").value : null;
                    storeFollowUp[indice].id_alterado = true;
                    storeFollowUp[indice].cd_usuario_destino = hasValue(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) && parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) > 0 ? parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) : null;
                    storeFollowUp[indice].no_usuario_destino = hasValue(dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value) ? dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value : null;

                    storeFollowUp.sort(function byOrdem(a, b) { return a.nroOrdem < b.nroOrdem; });
                    gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));

                    //dijit.byId("gridFollowUp").update();
                    dijit.byId('cadFollowUp').hide();
                }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAluno', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirLinhaFollowUp() {
    try {
        //Verifica se o campo de mensagem está preenchido:
        if (!hasValue(dijit.byId('mensagemFollowUppartial').value)) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroMensagemFollowUpObrig);
            apresentaMensagem('apresentadorMensagemCadFollowUpPartial', mensagensWeb);
            return false;
        }

        require([
           "dojo/data/ObjectStore",
           "dojo/store/Memory"
        ], function (ObjectStore, Memory) {
            dojo.xhr.post({
                url: Endereco() + "/util/PostDataHoraCorrente",
                preventCache: true,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                postData: dojox.json.ref.toJson()
            }).then(function (data) {
                try {
                    if (hasValue(data) && hasValue(data.retorno)) {
                        var dataCorrente = data.retorno;
                    var gridFollowUp = dijit.byId("gridFollowUp");
                    var ordem;
                    var dataFollowup = new Date(dataCorrente.dataIngles);
                    var storeFollowUp;

                    if (hasValue(gridFollowUp.store.objectStore.data) && gridFollowUp.store.objectStore.data.length > 0) {
                        storeFollowUp = gridFollowUp.store.objectStore.data;
                        ordem = gridFollowUp.store.objectStore.data[0].nroOrdem + 1;
                    }
                    else {
                        storeFollowUp = [];
                        ordem = 1;
                    }
                    var myNewItem = {
                        cd_follow_up_partial: dojo.byId('cd_follow_up_partial').value,
                        no_usuario: document.getElementById('nomeUsuario').innerText,
                        dta_follow_up: dataCorrente.dataPortugues,
                        dt_follow_up: dataFollowup,
                        dc_assunto: dijit.byId("mensagemFollowUppartial").value,
                        _dc_assunto: hasValue(dijit.byId("mensagemFollowUppartial").value) && dijit.byId("mensagemFollowUppartial").value.indexOf('<') < 0
                            && dijit.byId("mensagemFollowUppartial").value.indexOf('>') < 0 ? dijit.byId("mensagemFollowUppartial").value : "...",
                        cd_acao_follow_up: dijit.byId('cadAcaoFollowUpFK').value,
                        dt_proximo_contato: hasValue(dojo.byId("dtaProxContatoFollowUpFK").value) ? dojo.date.locale.parse(dojo.byId("dtaProxContatoFollowUpFK").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,//dojo.byId('dtaProxContatoFollowUpFK').value,
                        dta_proximo_contato: dojo.byId('dtaProxContatoFollowUpFK').value,
                        id_follow_lido: dijit.byId('ckLidoFollowUpFK').checked,
                        id_follow_resolvido: dijit.byId('ckResolvidoFollowUpFK').checked,
                        id_tipo_atendimento: hasValue(dijit.byId("cadTipoAtendimento").value) ? dijit.byId("cadTipoAtendimento").value: null,
                        nroOrdem: ordem,
                        cd_usuario_destino: hasValue(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) && parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) > 0 ? parseInt(dojo.byId("cdProspectAlunoUsuarioDestinoFollowUpFK").value) : null,
                        no_usuario_destino: hasValue(dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value) ? dijit.byId("cadNomeProspectAlunoUsuarioDestinoFollowUpFK").value : null
                    };
                    storeFollowUp.push(myNewItem);
                    storeFollowUp.sort(function byOrdem(a, b) { return a.nroOrdem < b.nroOrdem; });
                    gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));

                    dijit.byId('cadFollowUp').hide();
                }
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                apresentaMensagem('apresentadorMensagemAluno', error);
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function incluirFollowUp(Memory, ObjectStore) {
    showCarregando();
    try {
        if (!hasValue(dijit.byId('btnIncluirFollowUpPartial')) && !hasValue(dijit.byId('mensagemFollowUppartial')))
            montaFollowUpFK(function () {
                dijit.byId('ckResolvidoFollowUpFK').on('change', function (e) {
                    configuraLayoutFollowUp(e);
                });
                populaFollowUpAluno(novoFollowUpPartial(function () {
                    dijit.byId('ckResolvidoFollowUpFK').set('disabled', true);
                    configuraLayoutFollowUp(false);
                    showP('lblLido', false);
                    dijit.byId('ckLidoFollowUpFK').set('style', 'display:none');
                    dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomPessoa').value;
                    dijit.byId('btnAlunoProspectFK').set('disabled', true);
					showCarregando();
                }));
                montaFollowUpFKPersonalizadoAluno();
            });
        else
            populaFollowUpAluno(novoFollowUpPartial(function () {
                dijit.byId('ckResolvidoFollowUpFK').set('disabled', true);
                configuraLayoutFollowUp(false);
                showP('lblLido', false);
                dijit.byId('ckLidoFollowUpFK').set('style', 'display:none');
                dojo.byId('descAlunoProspectFollowUpFK').value = dojo.byId('nomPessoa').value;
                dijit.byId('btnAlunoProspectFK').set('disabled', true);
                showCarregando();
            }));
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function limparGridFollowUp(cdAluno, Memory, ObjectStore) {
    var gridFollowUp = dijit.byId("gridFollowUp");
    if (hasValue(cdAluno) && cdAluno > 0) {
        if (hasValue(gridFollowUp.store.objectStore.data) && gridFollowUp.store.objectStore.data.length > 0) {
            var storeFollowUp = gridFollowUp.store.objectStore.data;
            storeFollowUp = jQuery.grep(storeFollowUp, function (value) {
                return value.cd_aluno > 0;
            });
            gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: storeFollowUp, idProperty: "nroOrdem" }) }));
        }
    } else
        gridFollowUp.setStore(new ObjectStore({ objectStore: new Memory({ data: null, idProperty: "nroOrdem" }) }));

}

//Metodo criação matrículas
function criarGridTabMatriculas(cdAluno, dataTabMatriculas) {
    try {
        var dataStoreTabMat = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null, idProperty: "cd_contrato" }) });
        var gridTabMatricula = new dojox.grid.EnhancedGrid({
            store: dataStoreTabMat,
            structure:
              [
                {
                    name: "<input id='selecionaTodosTabMatriculas'  style='display:none' />", field: "selecionadoTabMatricula", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;",
                    formatter: formatCheckBoxTabMatricula
                },
                { name: "Data", field: "dtMatriculaContrato", width: "15%"},
                { name: "Contrato", field: "nm_contrato", width: "20%"},
                { name: "Curso", field: "no_curso", width: "20%"},
                { name: "Produto", field: "no_produto", width: "20%"},
                { name: "Ano Escolar", field: "dc_ano_escolar", width: "25%"}
              ],
            canSort: true,
            noDataMessage: "Nenhum registro encontrado."
        }, "gridTabMatriculas");
        gridTabMatricula.startup();
        gridTabMatricula.itensSelecionados = new Array();
        //gridTabMatricula.pagination.plugin._paginator.plugin.connect(gridTabMatricula.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
        //    verificaMostrarTodos(evt, gridTabMatricula, ' cd_contrato', 'selecionaTodosTabMatriculas');
        //});
        //require(["dojo/aspect"], function (aspect) {
        //    aspect.after(gridTabMatricula, "_onFetchComplete", function () {
        //        // Configura o check de todos:
        //        if (dojo.byId('selecionaTodosTabMatriculas').type == 'text')
        //            setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionadoTabMatricula', -1, 'selecionaTodosTabMatriculas', 'selecionaTodosTabMatriculas', 'gridTabMatriculas')",
        //                gridTabMatricula.rowsPerPage * 3);
        //    });
        //});

        gridTabMatricula.canSort = function () { return false };
        gridTabMatricula.startup();
        gridTabMatricula.rowsPerPage = 5000;
        if (cdAluno != null && cdAluno > 0)
            dojo.xhr.get({
                preventCache: true,
                handleAs: "json",
                url: Endereco() + "/api/secretaria/getMatriculasAluno?cd_aluno=" + parseInt(cdAluno),
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (dataTabMat) {
                try {
                    dataTabMat = jQuery.parseJSON(dataTabMat);
                    dataTabMat = dataTabMat.retorno;
                    var dataStoreTabMat = new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataTabMat, idProperty: "cd_contrato" }) });

                    if (hasValue(dataStoreTabMat))
                        gridTabMatricula.setStore(dataStoreTabMat);
                }
                catch (e) {
                    postGerarLog(e);
                }
            });
        else if (hasValue(dataTabMatriculas))
            gridTabMatricula.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dataProspect, idProperty: "cd_contrato" }) }));
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxTabMatricula(value, rowIndex, obj) {
    try {
        var gridName = 'gridTabMatriculas'
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosTabMatriculas');

        if (hasValue(grid.itensSelecionados) && hasValue(grid._by_idx[rowIndex].item)) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_contrato", grid._by_idx[rowIndex].item.cd_contrato);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_contrato', 'selecionadoTabMatricula', -1, 'selecionaTodosTabMatriculas', 'selecionaTodosTabMatriculas', '" + gridName + "')", 18);

        setTimeout("configuraCheckBox(" + value + ", 'cd_contrato', 'selecionadoTabMatricula', " + rowIndex + ", '" + id + "', 'selecionaTodosTabMatriculas', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

//eventoEditar(gridMatricula.itensSelecionados, xhr, ready, Memory, FilteringSelect, registry, Array, ObjectStore);
function eventoEditarTabMatricula(xhr, ready, Memory, FilteringSelect, registry, Array, ObjectStore) {
    try {
        var gridTabMatricula = dijit.byId("gridTabMatriculas");
        var itensSelecionados = gridTabMatricula.itensSelecionados;
        apresentaMensagem('apresentadorMensagemAluno', null);
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {

            var gridMatricula = dijit.byId('gridMatricula');
            apresentaMensagem('apresentadorMensagemAluno', null);
            if (!hasValue(dijit.byId('fkPlanoContasMat')) && !hasValue(dijit.byId("gridTurmaMat"))) {
                var Permissoes = hasValue(dojo.byId("setValuePermissoes").value) ? dojo.byId("setValuePermissoes").value : "";
                montarCadastroMatriculaPartial(function () {
                    dijit.byId("alterarMatricula").on("click", function () {
                        //Da um delay de 10 milisegundos para que o cálculo de descontos ocorra antes:
                        //setTimeout(function () { alterarMatriculaALuno(); }, 10);
                        alterarMatricula();
                    });
                    abrirMatriculaAluno(xhr, ready, Memory, FilteringSelect, ObjectStore, itensSelecionados);
                }, Permissoes);
            } else
                abrirMatriculaAluno(xhr, ready, Memory, FilteringSelect, ObjectStore, itensSelecionados);
        }
    }
    catch (e) {
        postGerarLog(e);
    }

}

function abrirMatriculaAluno(xhr, ready, Memory, FilteringSelect, ObjectStore, itensSelecionados) {
    showCarregando();
    limparCadMatricula(xhr, ObjectStore, Memory, false);
    setarTabCadMatricula(true);
    dijit.byId('tabContainerMatricula').resize();
    hideTagMatriculaTurma();
    dijit.byId("cadMatricula").show();
    dijit.byId('tabContainerMatricula').resize();
    IncluirAlterar(0, 'divAlterarMatricula', 'divIncluirMatricula', 'divExcluirMatricula', 'apresentadorMensagemMatricula', 'divCancelarMatricula', 'divLimparMatricula');
    var gridTabMatricula = dijit.byId("gridTabMatriculas");
    gridTabMatricula.itemSelecionado = itensSelecionados[0];
    habilitarOnChange("ckAula", false);
    keepValuesMatricula(null, dijit.byId('gridTabMatriculas'), true, xhr, ready, Memory, FilteringSelect, ObjectStore);
    habilitarOnChange("ckAula", true);
}
/*
function alterarMatriculaALuno() {
    try {
        var gridTitulo = dijit.byId('gridTitulo');
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemMat', null);
        dijit.byId("tipoContratoAdto").set("required", false);

        var dtaInicioMat = hasValue(dojo.byId("dtaInicioMat").value) ? dojo.date.locale.parse(dojo.byId("dtaInicioMat").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;
        var dtaFinalMat = hasValue(dojo.byId("dtaFinalMat").value) ? dojo.date.locale.parse(dojo.byId("dtaFinalMat").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null;

        // Verifica se existe algum título sem local de movimento:
        if (hasValue(gridTitulo)) {
            var titulos = gridTitulo.store.objectStore.data;
            for (var i = 0; i < titulos.length; i++)
                if (!hasValue(titulos[i].cd_local_movto) || titulos[i].cd_local_movto <= 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroLocalMovtoObrigatorio);
                    apresentaMensagem("apresentadorMensagemMat", mensagensWeb);
                    return false;
                }
        }

        var valorCurso = dijit.byId("valorCurso").get('value');
        //valorCurso = unmaskFixed(valorCurso, 2);

        var valorDesconto = dijit.byId("valorDesconto").get('value');
        valorDesconto = unmaskFixed(valorDesconto, 2);


        var desconto = dijit.byId('desconto').get('value');
        var nroParcelas = getNroParcelas("parcelas");

        var valorParcela = calcularRetornarValorParcela(valorCurso, nroParcelas, valorDesconto);

        var parcelas = hasValue(dijit.byId('parcelas')) ? dijit.byId('parcelas').get('value') : 1;
        var valorLiquido = dijit.byId("valorFaturar").get('value');
        var divida = dijit.byId("divida").get('value');

        valorLiquido = unmaskFixed(valorLiquido, 2);
        divida = unmaskFixed(divida, 2);

        //Verifica o percentual máximo permidito pela escola:
        getValoresParaDesconto(null, null, null, false);

        //Verifica a consistência do desconto máximo e desconto na primeira parcela.
        if (!consistirDescontoMaximo(valorDesconto, valorCurso))
            return false;

        //Calcula valor de desconto da primeira parcela:
        var valorDescontoParcela = calcularDescontoPrimeiraParcela(valorCurso);

        if (!consistirDescontosParcela(valorParcela, valorDescontoParcela))
            return false;
        else {
            var valorLiquido = calcularERetornarValorFaturar(valorCurso, nroParcelas, valorDesconto);

            dijit.byId('valorFaturar')._onChangeActive = false;
            dijit.byId('valorFaturar').set('value', unmaskFixed(valorLiquido, 2));
            dijit.byId('valorFaturar').old_value = valorFaturar;
            dijit.byId('valorFaturar')._onChangeActive = true;
        }

        if (!validarAditamentoCrud(window) || !consistirValorCurso(valorLiquido, valorCurso, divida))
            return false;

        if (!validarMatriculaCadastro()) {
            setarTabCadMatricula(true);
            return false;
        }

        //Verifica se a data inicial é menor ou igual a data final:
        showCarregando();
        require([
            "dojo/store/Memory",
            "dijit/form/FilteringSelect",
            "dojo/data/ObjectStore",
            "dojo/_base/xhr",
            "dojox/json/ref",
            "dojo/date"
        ], function (Memory, filteringSelect, ObjectStore, xhr, ref, date) {
            try {
                if (date.compare(dtaInicioMat, dtaFinalMat) > 0) {
                    var mensagensWeb = new Array();
                    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
                    apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
                    return false;
                }
                var files = dijit.byId("uploaderContratoDigitalizado")._files;
                if (ALTEROU_IMAGEM_DIGITALIZADA == true && gerar_titulo == false && hasValue(files) && files.length > 0) {
                    atualizarDocumentoDigitalizadoEdicaoMatriculaAluno(xhr, ref);
                } else {
                    criarAtualizarTitulo(Memory, ObjectStore, xhr, ref, function () {
                        salvarAlteracaoMatrciulaAluno(xhr, ref);
                    });
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


function atualizarDocumentoDigitalizadoEdicaoMatriculaAluno(xhr, ref) {
    try {
        var files = dijit.byId("uploaderContratoDigitalizado")._files;
        if (window.FormData !== undefined) {
            var data = new FormData();
            for (i = 0; i < files.length; i++) {
                data.append("file" + i, files[i]);
            }
            $.ajax({
                type: "POST",
                url: Endereco() + "/secretaria/UploadDocumentoDigitalizado",
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
                            apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
                            return false;
                        }
                        if (hasValue(results) && !hasValue(results.erro)) {


                            var contrato = new montarObjArquivoDigitalizado(results);
                            xhr.post({
                                url: Endereco() + "/api/escola/PostAtualizarDocumentoDigitalizado",
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                postData: ref.toJson(contrato)
                            }).then(function (data) {
                                try {
                                    var dataAlterado = jQuery.parseJSON(data);
                                    if (dataAlterado != null && hasValue(dataAlterado) && !hasValue(dataAlterado.erro)) {
                                        var itemAlterado = dataAlterado.retorno;
                                        var gridName = 'gridMatricula';
                                        var grid = dijit.byId(gridName);
                                        dojo.byId("descApresMsg").value = !hasValue(dojo.byId("descApresMsg").value) ? 'apresentadorMensagem' : dojo.byId("descApresMsg").value;
                                        apresentaMensagem(dojo.byId("descApresMsg").value, data);
                                        dijit.byId("cadMatricula").hide();
                                        if (hasValue(grid)) {
                                            if (!hasValue(grid.itensSelecionados))
                                                grid.itensSelecionados = [];
                                        }
                                        //    //removeObjSort(grid.itensSelecionados, "cd_contrato", dojo.byId("cd_contrato").value);
                                        //    //insertObjSort(grid.itensSelecionados, "cd_contrato", itemAlterado);
                                        //    buscarItensSelecionados(gridName, 'selecionadoMatricula', 'cd_contrato', 'selecionaTodosMatricula', ['pesquisarMatricula', 'relatorioMatricula'], 'todosItens');
                                        //    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                                        //    //setGridPagination(grid, itemAlterado, "cd_contrato");
                                        //}


                                        showCarregando();
                                    } else {
                                        apresentaMensagem('apresentadorMensagemMat', data);
                                        showCarregando();
                                    }
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagemMat', error);
                                showCarregando();
                            });

                        } else
                            apresentaMensagem('apresentadorMensagemMat', results);
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                error: function (error) {
                    apresentaMensagem('apresentadorMensagemMat', error);
                    return false;
                }
            });
        } else {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Não foi possível carregar o arquivo para fazer o upload. Tente novamente.");
            apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
        }
    } catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}
*/
function salvarAlteracaoMatrciulaAluno(xhr, ref) {
    try {
        
        var files = dijit.byId("uploaderContratoDigitalizado")._files;
        if (hasValue(files) && files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (i = 0; i < files.length; i++) {
                    data.append("file" + i, files[i]);
                }
                $.ajax({
                    type: "POST",
                    url: Endereco() + "/secretaria/UploadDocumentoDigitalizado",
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
                                apresentaMensagem('apresentadorMensagemMat', mensagensWeb);
                                return false;
                            }
                            if (hasValue(results) && !hasValue(results.erro)) {
                                console.log("Salvou arquivo dig temporario");
                                
                                /*Salva o contrato com arquivo digitalizado*/
                                var contrato = new montarContrato(results);

                                xhr.post({
                                    url: Endereco() + "/api/escola/PostAlterarMatriculaALuno",
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                                    postData: ref.toJson(contrato)
                                }).then(function (data) {
                                        try {
                                            if (!hasValue(jQuery.parseJSON(data).erro)) {
                                                var itemAlterado = jQuery.parseJSON(data).retorno;
                                                var gridName = 'gridTabMatriculas';
                                                var grid = dijit.byId(gridName);
                                                apresentaMensagem("apresentadorMensagemAluno", data);
                                                dijit.byId("cadMatricula").hide();
                                                if (!hasValue(grid.itensSelecionados) && grid._by_idx.length > 0)
                                                    grid.itensSelecionados = [];
                                                grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemAlterado, idProperty: "cd_contrato" }) }));
                                            } else
                                                apresentaMensagem('apresentadorMensagemMat', data);
                                            showCarregando();
                                        }
                                        catch (e) {
                                            postGerarLog(e);
                                        }
                                    },
                                    function (error) {
                                        showCarregando();
                                        apresentaMensagem('apresentadorMensagemMat', error);
                                    });
                                /*Fim Salvar o contrato com arquivo digitalizado*/

                            } else
                                apresentaMensagem('apresentadorMensagemMat', results);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    },
                    error: function (error) {
                        apresentaMensagem('apresentadorMensagemMat', error);
                        return false;
                    }
                });
            } else {
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Impossível fazer upload de arquivo.");
                apresentaMensagem('apresentadorMensagemNoCont', mensagensWeb);
            }
        } else {
            
            /*Salva o contrato sem arquivo digitalizado*/
            var contrato = new montarContrato(results);

            xhr.post({
                url: Endereco() + "/api/escola/PostAlterarMatriculaALuno",
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(contrato)
            }).then(function (data) {
                try {
                    if (!hasValue(jQuery.parseJSON(data).erro)) {
                        var itemAlterado = jQuery.parseJSON(data).retorno;
                        var gridName = 'gridTabMatriculas';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem("apresentadorMensagemAluno", data);
                        dijit.byId("cadMatricula").hide();
                        if (!hasValue(grid.itensSelecionados) && grid._by_idx.length > 0)
                            grid.itensSelecionados = [];
                        grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: itemAlterado, idProperty: "cd_contrato" }) }));
                    } else
                        apresentaMensagem('apresentadorMensagemMat', data);
                    showCarregando();
                }
                catch (e) {
                    postGerarLog(e);
                }
            },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemMat', error);
                });
            /*Fim Salvar o contrato sem arquivo digitalizado*/

        }

    }
    catch (e) {
        postGerarLog(e);
    }
}

function limparProspect() {
    try {
        dojo.byId('nomPessoa').value = "";
        dijit.byId('sexo').reset(); //codigo
        dijit.byId('telefone').reset();
        dijit.byId('celular').reset();
        dijit.byId('operadora').reset();
        dojo.byId('email').value = "";
        dijit.byId('cbMarketing').reset();
        document.getElementById('cd_pessoa').value = 0;
        document.getElementById('cdPessoaProject').value = 0;
        document.getElementById('cd_prospect').value = 0;
        var gridFollowUp = dijit.byId("gridFollowUp");
        dojo.byId('no_prospect').value = "";
        dojo.byId('cd_prospect').value = 0;
        //Preenche os dados:
        if (hasValue(gridFollowUp))
            gridFollowUp.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: new Array(), idProperty: "nroOrdem" }) }));
        else
            criarGridFollowUp(null, new Array());
    }
    catch (e) {
        postGerarLog(e);
    }
}

function loadFormaPag(Memory, filteringSelect) {
    var statusStore = new Memory({
        data: [
        { name: "Hora", id: 1 },
        { name: "Aula", id: 2 },
        { name: "Fixo", id: 3 }
        ]
    });
    var TipoPagto = new filteringSelect({
        id: "formaPagto",
        name: "formaPagto",
        store: statusStore,
        value: "3",
        searchAttr: "name",
        style: "max-width:182px;width: 100%;"
    }, "formaPagto");
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

function montarSituacaoAlunoTurma(Memory, registry, ItemFileReadStore) {
    try {
        var w = registry.byId('situacao');
        var dados = [
                     { name: "Aguardando Matrícula", id: "9" },
                     { name: "Matriculado", id: "1" },
                     { name: "Rematriculado", id: "8" },
                     { name: "Desistente", id: "2" },
                     { name: "Transferido", id: "3" },
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
        w.setStore(store);
    } catch (e) {
        postGerarLog(e);
    }
}


function inserirIdTabsAlunoRaf() {
    try {
        if (hasValue(dojo.byId("tabContainer_tablist_tabRafAluno")))
            dojo.byId("tabContainer_tablist_tabRafAluno").parentElement.id = "paiTabRafAluno";
    }
    catch (e) {
        postGerarLog(e);
    }
}


function alterarVisibilidadeTabAlunoRaf(bool) {
    try {
        if (bool && hasValue(dojo.byId("paiTabRafAluno")))
            dojo.style("paiTabRafAluno", "display", "");
        else if (!bool && hasValue(dojo.byId("paiTabRafAluno")))
            dojo.style("paiTabRafAluno", "display", "none");
    }
    catch (e) {
        postGerarLog(e);
    }
}

function enviarEmailRaf(emailRafAluno, nmRaf) {
    apresentaMensagem('apresentadorMensagemAlunoRAF', null);
    showCarregando();
    dojo.xhr.get({
        url: Endereco() + "/api/secretaria/getUrlEnviarEmailAlunoRaf?emailRafAluno=" + emailRafAluno + "&nmRaf=" + nmRaf,
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (dados) {
        try {
                if (hasValue(dados)) {
                    
                    try {
                        caixaDialogo(DIALOGO_AVISO, "Email enviado com sucesso.", null);
                        dijit.byId("enviarEmailRafAluno").set("disabled", true);
                        hideCarregando();
                    } catch (e) {
                        hideCarregando();
                        postGerarLog(e);
                    }
                        
                } else {
                    hideCarregando();
                    caixaDialogo(DIALOGO_AVISO, "Não foi possível configurar os parametros (email ou nmRaf) para enviar o email. Tente novamente ou contate o adminstrador do sistema.", null);
                }
                
            }
            catch (e) {
            hideCarregando();
            caixaDialogo(DIALOGO_AVISO, "Houve um problema ao realizar a consulta que configura os parametros do envio do email. Tente novamente ou contate o adminstrador do sistema.", null);
            }
        },
        function (error) {
            hideCarregando();
            apresentaMensagem('apresentadorMensagemAlunoRAF', error);
        });
}

function getParametroEscolaInternacional()
{
    dojo.xhr.get({
        url: Endereco() + "/api/aluno/getParametroEscolaInternacional",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        if (hasValue(data) && data.escolaInternacional == true) {
            dijit.byId("cpf").set("required", false);
            dojo.byId("parametroEscolaInternacional").value = 1;
        } else {
            dijit.byId("cpf").set("required", true);
            dojo.byId("parametroEscolaInternacional").value = 0;
        }
            
    },
    function (error) {
        apresentaMensagem('apresentadorMensagemAluno', error);
    });
}


function inicialAluno() {
    require([
        "dojo/ready",
        "dojo/on",
        "dojo/store/Memory",
        "dijit/form/FilteringSelect"
    ], function (ready, on, Memory, FilteringSelect) {
        ready(function () {
            try {
                $("#dt_hora_medicacao_especifica").mask("99:99");
                $('#dc_telefone_hospital_clinica').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                $('#dc_telefone_residencial_aviso_emergencia').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                $('#dc_telefone_fixo_hospital_clinica').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                $('#dc_telefone_comercial_aviso_emergencia').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                $('#dc_telefone_celular_aviso_emergencia').focusout(function () {
                    try {
                        var phone, element;
                        element = $(this);
                        element.unmask();
                        phone = element.val().replace(/\D/g, '');
                        if (phone.length > 10) {
                            element.mask("(99) 99999-999?9");
                        } else {
                            element.mask("(99) 9999-9999?9");
                        }
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                }).trigger('focusout');
                /*$("#telefone").mask("(99) 9999-9999");
                $("#telefonePessoaRelac").mask("(99) 9999-9999");
                
                maskHHMMSS("#horaCadastro");*/

                //Monta o tipo de Sociedade(Limitada/Anônima)
                var booleanValue = new Array(
                    { name: "Sim", id: 1 },
                    { name: "Não", id: 0 }
                    
                );
                criarOuCarregarCompFiltering('id_problema_saude', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_tratamento_medico', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_uso_medicamento', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_recomendacao_medica', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_alergico', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_alergico_alimento_material', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_epiletico', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_epiletico_tratamento', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_asmatico', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_asmatico_tratamento', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_diabetico', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_depende_insulina', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_medicacao_especifica', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                criarOuCarregarCompFiltering('id_plano_saude', booleanValue, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
                var pessoaAviso = new Array(
                    { name: "Pai", id: 1 },
                    { name: "Mãe", id: 2 }
                );

                criarOuCarregarCompFiltering('id_aviso_emergencia', pessoaAviso, 'width: 75px;', null, ready, Memory, FilteringSelect, 'id', 'name', null);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}