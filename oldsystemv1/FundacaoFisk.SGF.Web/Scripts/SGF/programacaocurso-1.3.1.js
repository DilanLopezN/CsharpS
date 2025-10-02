var alterarOrd = 0, ordemOri = 0;
var SOBE = 1, DESCE = 2;
var edicaoNovo = false;
//Pega os Antigos dados do Formulário
function keepValues(value, grid, ehLink) {
    try{
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
        if (hasValue(value)) {
            dojo.byId("cd_programacao_curso").value = value.cd_programacao_curso;

            var cursoAtivo = dijit.byId("cursoAtivo");
            cursoAtivo._onChangeActive = false;
            cursoAtivo.set('value', value.cd_curso);
            cursoAtivo._onChangeActive = true;

            var duracaoAtiva = dijit.byId("duracaoAtiva");
            duracaoAtiva._onChangeActive = false;
            duracaoAtiva.set('value', value.cd_duracao);
            duracaoAtiva._onChangeActive = true;
            retornaProgramacaoCurso(value.cd_programacao_curso);
            dijit.byId("incluirProgGrid")._onChangeActive = false;
            dijit.byId("incluirProgGrid").set("disabled", false);
            dijit.byId("incluirProgGrid")._onChangeActive = true;
            //habilita Cancela e some como limpar
            document.getElementById('divLimparProgramacao').style.display = "none";
            document.getElementById('divCancelarProgramacao').style.display = "";
        }
    } catch (e) {
        postGerarLog(e);
    }
}


function keepValuesProg(value, grid, ehLink) {
    try{
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
        if (hasValue(value)) {
            dojo.byId("cd_programacao_curso").value = value.cd_programacao_curso;
            dojo.byId("nm_aula_programacao").value = value.nm_aula_programacao;
            dojo.byId("dc_aula_programacao").value = value.dc_aula_programacao;
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBox(value, rowIndex, obj) {
    try{
        var gridName = 'gridProgramacao';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_programacao_curso", grid._by_idx[rowIndex].item.cd_programacao_curso);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='height:14px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_programacao_curso', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_programacao_curso', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);


        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function formatCheckBoxProg(value, rowIndex, obj) {
    try{
        var gridName = 'gridProg';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosProg');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "nm_aula_programacao", grid._by_idx[rowIndex].item.nm_aula_programacao);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input style='height:19px'  id='" + id + "'/> ";

        // Configura o check de todos, para quando mudar de aba:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'nm_aula_programacao', 'selecionadoProg', -1, 'selecionaTodosProg', 'selecionaTodosProg', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'nm_aula_programacao', 'selecionadoProg', " + rowIndex + ", '" + id + "', 'selecionaTodosProg', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function checkBoxChangeProg(rowIndex) {
    try{
        var gridProg = dijit.byId('gridProg');
        if(hasValue(gridProg._by_idx))
            gridProg._by_idx[rowIndex].item.selecionadoProg = !gridProg._by_idx[rowIndex].item.selecionadoProg;
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarDia(itensSelecionados) {
    try{
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, 'Selecione algum registro para alterar.', null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, 'Selecione apenas um registro para alterar.', null);
        else {
            dijit.byId("dialogGridProg").show();
            document.getElementById("cd_item_programacao").value = itensSelecionados[0].cd_item_programacao,
            document.getElementById("dc_aula_programacao").value = itensSelecionados[0].dc_aula_programacao;
            document.getElementById("nm_aula_programacao").value = itensSelecionados[0].nm_aula_programacao;
            alterarOrd = 1;
            ordemOri = itensSelecionados[0].nm_aula_programacao;
            document.getElementById('divCancelarProg').style.display = "";
            document.getElementById('divLimparProg').style.display = "none";
            dojo.byId("incluirLinhaProg_label").innerHTML = "Alterar";
        }
    } catch (e) {
        postGerarLog(e);
    }
}
function montaCadastroProgramacao() {
    require([
        "dojo/_base/xhr",
        "dojo/dom",
        "dojox/grid/EnhancedGrid",
        "dojox/grid/enhanced/plugins/Pagination",
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/query",
        "dijit/form/Button",
        "dojo/ready",
        "dijit/form/DropDownButton",
        "dijit/DropDownMenu",
        "dijit/MenuItem"
    ], function (xhr, dom, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, query, Button, ready, DropDownButton, DropDownMenu, MenuItem) {
        ready(function () {
            try {
                // dijit.byId('tabContainer').resize();
                dijit.byId("cbCurso").set("required", false);
                dijit.byId("cbDuracao").set("required", false);
                var cdCurso = dijit.byId("cbCurso").get("value") == null || dijit.byId("cbCurso").get("value") == "" ? 0 : dijit.byId("cbCurso").get("value");
                var cdDuracao = dijit.byId("cbDuracao").get("value") == null || dijit.byId("cbDuracao").get("value") == "" ? 0 : dijit.byId("cbDuracao").get("value");
                var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/coordenacao/getProgramacaoCursoSearch?cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
                ), Memory({}));

                var gridProgramacao = new EnhancedGrid({
                    store: ObjectStore({ objectStore: myStore }),
                    structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                              //  { name: "Código", field: "cd_programacao_curso", width:"50px", styles: "text-align: right; min-width:60px; max-width:75px;"},
                                { name: "Curso", field: "no_curso", width: "50%", styles: "min-width:80px;" },
                                { name: "Carga Horária", field: "dc_duracao", width: "33%", styles: "min-width:80px;" },
                                { name: "Nr. Aulas", field: "nm_aula_programacao", width: "10%", styles: "text-align:center; min-width:80px;" }

                    ],
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            /*page step to be displayed*/
                            maxPageStep: 4,
                            /*position of the pagination bar*/
                            position: "button"
                        }
                    }
                }, "gridProgramacao");
                // Remove o marcador de todos quando selecionado todos os itens, pois o dojo pagina de 25 a 25 itens:
                gridProgramacao.pagination.plugin._paginator.plugin.connect(gridProgramacao.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) { verificaMostrarTodos(evt, gridProgramacao, 'cd_programacao_curso', 'selecionaTodos'); });
                gridProgramacao.canSort = function (col) { return Math.abs(col) != 1; };
                gridProgramacao.startup();
                gridProgramacao.on("RowDblClick", function (evt) {
                    try {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        dijit.byId("incluirProgGrid")._onChangeActive = false;
                        dijit.byId("incluirProgGrid").set("disabled", false);
                        dijit.byId("incluirProgGrid")._onChangeActive = true;
                        setTimeout("montarProgOrdem()", 200);
                        dijit.byId("cursoAtivo").set("disabled", true);
                        dijit.byId("duracaoAtiva").set("disabled", true);
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        getLimpar('#formProgramacao');
                        apresentaMensagem('apresentadorMensagem', '');
                        dijit.byId("gridProg").itensSelecionados = [];
                        xhr.get({
                            url: Endereco() + "/api/Curso/getCursos?cd_curso=",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataCursoAtivo) {
                            xhr.get({
                                url: Endereco() + "/api/Coordenacao/GetDuracaoProgramacao",
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }).then(function (dataDuracoesAtivo) {
                                try{
                                    loadCursoAtivo(dataCursoAtivo.retorno, 'cursoAtivo');
                                    loadDuracoesAtivo(jQuery.parseJSON(dataDuracoesAtivo).retorno, 'duracaoAtiva');
                                    keepValues(item, gridProgramacao, false);
                                    edicaoNovo = false;
                                    dijit.byId("cad").show();
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                //var data = new Array();
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) });
                var gridProg = new EnhancedGrid({
                    store: dataStore,
                    structure:
                      [
                        { name: "<input id='selecionaTodosProg' style='display:none'/>", field: "selecionadoProg", width: "9%", styles: "text-align: center;", formatter: formatCheckBoxProg },
                        { name: "Cd Item", field: "cd_item_programacao", styles: "display: none;" },
                        { name: "Aula Nro.", field: "nm_aula_programacao", width: "11%", styles: "min-width:50px; text-align: center;" },
                        { name: "Programação", field: "dc_aula_programacao", width: "77%", styles: "min-width:80px;" }
                      ],
                    selectionMode: "single",
                    canSort: false,
                    noDataMessage: msgNotRegEnc// "Nenhum registro encontrado."
                }, "gridProg");
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridProg, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosProg').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_item_programacao', 'selecionadoProg', -1, 'selecionaTodosProg', 'selecionaTodosProg', 'gridProg')", gridProg.rowsPerPage * 3);
                    });
                });
                gridProg.canSort = function () { return false };
                gridProg.startup();
                gridProg.on("RowDblClick", function (evt) {
                    try{
                        dijit.byId("dialogGridProg").show();
                        var idx = evt.rowIndex,
                           item = this.getItem(idx),
                           store = this.store;
                        document.getElementById("cd_item_programacao").value = item.cd_item_programacao,
                        document.getElementById("dc_aula_programacao").value = item.dc_aula_programacao;
                        document.getElementById("nm_aula_programacao").value = item.nm_aula_programacao;
                        alterarOrd = 1;
                        ordemOri = item.nm_aula_programacao;
                        document.getElementById('divCancelarProg').style.display = "";
                        document.getElementById('divLimparProg').style.display = "none";
                        dojo.byId("incluirLinhaProg_label").innerHTML = "Alterar";
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);
                gridProg.rowsPerPage = 5000;
                query("#descProgramacao").on("keyup", function (e) { if (e.keyCode == 13) PesquisarProgramacao(true); });

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosProgramacao(); } }, "incluirProgramacao");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { EnviarDadosProgramacao(); } }, "alterarProgramacao");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparProgramacao(); } }, "limparProgramacao");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        try{
                            apresentaMensagem('apresentadorMensagemProgramacao', null);
                            if (!edicaoNovo)
                                keepValues(null, gridProgramacao, null);
                            loadGridProgramacao(dijit.byId("cursoAtivo").value, dijit.byId("duracaoAtiva").value);
                            dijit.byId("gridProg").itensSelecionados = [];
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "cancelarProgramacao");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cad").hide(); } }, "fecharProgramacao");

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconInsert',
                    onClick: function () {
                        try{
                            dijit.byId("dialogGridProg").show();
                            alterarOrd = 0;
                            document.getElementById("cd_item_programacao").value = '';
                            document.getElementById("dc_aula_programacao").value = '';
                            var grid = dijit.byId("gridProg")._by_idx;
                            if (grid.length > 0) {
                                document.getElementById("nm_aula_programacao").value = grid[grid.length - 1].item.nm_aula_programacao + 1;
                                document.getElementById("dc_aula_programacao").value = "Aula " + (grid[grid.length - 1].item.nm_aula_programacao + 1);
                            }
                            else {
                                document.getElementById("nm_aula_programacao").value = 1;
                                document.getElementById("dc_aula_programacao").value = "Aula " + 1;
                            }

                            document.getElementById('divCancelarProg').style.display = "none";
                            document.getElementById('divLimparProg').style.display = "";
                            dojo.byId("incluirLinhaProg_label").innerHTML = "Incluir";
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirProgGrid");

                new Button({
                    label: "Incluir", iconClass: 'dijitEditorIcon dijitEditorIconRedo',
                    onClick: function () {
                        try{
                            if (alterarOrd == 1 || dojo.byId("cd_item_programacao").value > 0)
                                alterarProgGrid(dijit.byId("gridProg"));
                            else
                                incluiProgGrid();
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "incluirLinhaProg");
                new Button({
                    label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat',
                    type: "reset",
                    onClick: function () {
                        try{
                            alterarOrd = 0;
                            document.getElementById("cd_item_programacao").value = '';
                            document.getElementById("dc_aula_programacao").value = '';
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }

                }, "limparProg");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent',
                    onClick: function () {
                        keepValuesProg(null, dijit.byId("gridProg"), false);
                    }
                }, "cancelarProg");
                new Button({
                    label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel',
                    onClick: function () {
                        dijit.byId("dialogGridProg").hide();
                    }
                }, "fecharLinhaProg");

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridProgramacao, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodos').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_programacao_curso', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridProgramacao')", gridProgramacao.rowsPerPage * 3);
                    });
                });

                montarProgOrdem();

                // Adiciona link de ações:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Editar",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroEdicaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoEditar(gridProgramacao.itensSelecionados);
                    }
                });
                menu.addChild(acaoEditar);

                var acaoRemover = new MenuItem({
                    label: "Excluir",
                    onClick: function () {
                        if (!eval(MasterGeral())) {
                            var mensagensWeb = new Array();
                            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgExclusaoFundacao);
                            apresentaMensagem("apresentadorMensagem", mensagensWeb);
                            return;
                        }
                        eventoRemover(gridProgramacao.itensSelecionados, 'DeletarProgramacao(itensSelecionados)');
                    }
                });
                menu.addChild(acaoRemover);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Adiciona link de selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                        buscarTodosItens(gridProgramacao, 'todosItens', ['pesquisarProgramacao', 'relatorioProgramacao']);
                        PesquisarProgramacao(false);

                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () {
                        buscarItensSelecionados('gridProgramacao', 'selecionado', 'cd_programacao_curso', 'selecionaTodos', ['pesquisarProgramacao', 'relatorioProgramacao'], 'todosItens');
                    }
                });
                menu.addChild(menuItensSelecionados);

                var button = new DropDownButton({
                    label: "Todos Itens",
                    name: "todosItens",
                    dropDown: menu,
                    id: "todosItens"
                });
                dom.byId("linkSelecionados").appendChild(button.domNode);

                montaBotoesProgramacao();
                showCarregando();

                // Adiciona link de ações:
                var menuProg = new DropDownMenu({ style: "height: 25px" });

                var acaoEditarProg = new MenuItem({
                    label: "Editar",
                    onClick: function () { eventoEditarDia(dijit.byId("gridProg").itensSelecionados); }
                });
                menuProg.addChild(acaoEditarProg);

                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarLinhaProg(); }
                });
                menuProg.addChild(acaoExcluir);



                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasProg",
                    dropDown: menuProg,
                    id: "acoesRelacionadasProg"
                });
                dom.byId("linkAcoesProg").appendChild(button.domNode);

                dijit.byId("cursoAtivo").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            showCarregando();
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dijit.byId("cursoAtivo").value) && hasValue(dijit.byId("duracaoAtiva").value)) {
                                loadGridProgramacao(dijit.byId("cursoAtivo").value, dijit.byId("duracaoAtiva").value, function () { showCarregando(); });
                                dijit.byId("incluirProgGrid").set("disabled", false);
                            }
                            else {
                                dijit.byId("incluirProgGrid").set("disabled", true);
                                showCarregando();
                            }
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                dijit.byId("duracaoAtiva").on("change", function (e) {
                    try{
                        if (e >= 0) {
                            showCarregando();
                            e == null || e == "" ? e = 0 : e;
                            if (hasValue(dijit.byId("cursoAtivo").value) && hasValue(dijit.byId("duracaoAtiva").value)) {
                                loadGridProgramacao(dijit.byId("cursoAtivo").value, dijit.byId("duracaoAtiva").value, function () { showCarregando(); });
                                dijit.byId("incluirProgGrid").set("disabled", false);
                            }
                            else {
                                dijit.byId("incluirProgGrid").set("disabled", true);
                                showCarregando();
                            }
                        }
                    } catch (e) {
                        postGerarLog(e);
                    }
                });

                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323019', '765px', '771px');
                        });
                }
                xhr.get({
                    url: Endereco() + "/api/Curso/GetCursoProgramacao",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataCursoProg) {
                    try{
                        loadCurso(jQuery.parseJSON(dataCursoProg).retorno, 'cbCurso');
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                xhr.get({
                    url: Endereco() + "/api/Coordenacao/GetDuracaoProgramacao",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataDuracaoProg) {
                    try{
                        loadDuracao(jQuery.parseJSON(dataDuracaoProg).retorno, 'cbDuracao');
                    } catch (e) {
                        postGerarLog(e);
                    }
                });
                adicionarAtalhoPesquisa(['cbCurso', 'cbDuracao'], 'pesquisarProgramacao', ready);
            }catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function montarProgOrdem() {
    // ** Inicio da criação da grade de Estagio para Ordenar**\\
    try{

        var grid = dijit.byId("gridProg");

        if (grid._by_idx == 0) {
            document.getElementById('divProgCima').style.display = "none";
            document.getElementById('divProgBaixo').style.display = "none";
            document.getElementById('divIncluirProgramacao').style.display = "none";
            document.getElementById('divCancelarProgramacao').style.display = "none";
            document.getElementById('divLimparProgramacao').style.display = "";

        }
        else {
            document.getElementById('divProgCima').style.display = "";
            document.getElementById('divProgBaixo').style.display = "";
            document.getElementById('divIncluirProgramacao').style.display = "";
            document.getElementById('divCancelarProgramacao').style.display = "";
            document.getElementById('divLimparProgramacao').style.display = "none";
        }

        if (!hasValue(dijit.byId('subir'))) {
            new dijit.form.Button({
                label: "Subir", iconClass: 'dijitEditorIcon dijitEditorIconTabUp',
                onClick: function () {
                    subirDescerOrdemProg(grid, SOBE);
                }
            }, "subir");
        }
        if (!hasValue(dijit.byId('descer'))) {
            new dijit.form.Button({
                label: "Descer", iconClass: 'dijitEditorIcon dijitEditorIconTabDown',
                onClick: function () {
                    subirDescerOrdemProg(grid, DESCE);
                }
            }, "descer");
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAlgumAlterar, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
        else {
            dijit.byId("incluirProgGrid")._onChangeActive = false;
            dijit.byId("incluirProgGrid").set("disabled", false);
            dijit.byId("incluirProgGrid")._onChangeActive = true;
            setTimeout("montarProgOrdem()", 200);
            dijit.byId("cursoAtivo").set("disabled", true);
            dijit.byId("duracaoAtiva").set("disabled", true);

            getLimpar('#formProgramacao');
            apresentaMensagem('apresentadorMensagem', '');
            dijit.byId("gridProg").itensSelecionados = [];
            require([
             "dojo/_base/xhr"
            ], function (xhr) {
                xhr.get({
                    url: Endereco() + "/api/Curso/getCursos?cd_curso=",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }).then(function (dataCursoAtivo) {
                    xhr.get({
                        url: Endereco() + "/api/curso/GetCursoProgramacao",
                        preventCache: true,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }).then(function (dataCursoAtivo) {
                        xhr.get({
                            url: Endereco() + "/api/Coordenacao/GetDuracaoProgramacao",
                            preventCache: true,
                            handleAs: "json",
                            headers: { "Accept": "application/json", "Authorization": Token() }
                        }).then(function (dataDuracoesAtivo) {
                            try {
                                loadCursoAtivo(jQuery.parseJSON(dataCursoAtivo).retorno, 'cursoAtivo');
                                loadDuracoesAtivo(jQuery.parseJSON(dataDuracoesAtivo).retorno, 'duracaoAtiva');
                                keepValues(null, dijit.byId('gridProgramacao'), true);
                                edicaoNovo = false;
                                dijit.byId("cad").show();
                            } catch (e) {
                                postGerarLog(e);
                            }
                        },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    },
                function (error) {
                    apresentaMensagem('apresentadorMensagem', error);
                });
        },
        function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function loadGridProgramacao(cdCurso, cdDuracao, pFuncao) {
    require([
		"dojo/_base/xhr",
		"dojo/data/ObjectStore",
		"dojo/store/Memory"
    ], function (xhr, ObjectStore, Memory) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getCursoProgramacao?cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao,
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Authorization": Token() },
            idProperty: "nm_aula_programacao"
        }).then(function (dataProd) {
            try{
                var gridProg = dijit.byId("gridProg");

                var dataStore = new ObjectStore({ objectStore: new Memory({ data: jQuery.parseJSON(dataProd).retorno, idProperty: "nm_aula_programacao" }) });

                gridProg.setStore(dataStore);

                if (gridProg._by_idx == 0) {
                    document.getElementById('divProgCima').style.display = "none";
                    document.getElementById('divProgBaixo').style.display = "none";
                    document.getElementById('divIncluirProgramacao').style.display = "none";
                    document.getElementById('divCancelarProgramacao').style.display = "none";
                    document.getElementById('divLimparProgramacao').style.display = "";
                }
                else {
                    document.getElementById('divProgCima').style.display = "";
                    document.getElementById('divProgBaixo').style.display = "";
                    document.getElementById('divIncluirProgramacao').style.display = "";
                    document.getElementById('divCancelarProgramacao').style.display = "";
                    document.getElementById('divLimparProgramacao').style.display = "none";
                }
                if (hasValue(pFuncao))
                    pFuncao.call();
            } catch (e) {
                postGerarLog(e);
            }
        },
		function (error) {
		    apresentaMensagem('apresentadorMensagemProgramacao', error);
		});
    });
}

function loadCurso(data, linkCurso) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkCurso == 'cbCurso')) {
		            items.push({ id: 0, name: "Todos" });

		        }
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_curso, name: value.no_curso });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkCurso).store = stateStore;
		        dijit.byId(linkCurso).set("value", 0);
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function loadCursoAtivo(data, linkCurso) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_curso, name: value.no_curso });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkCurso).store = stateStore;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function loadDuracao(data, linkDuracao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        if ((linkDuracao == 'cbDuracao'))
		            items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_duracao, name: value.dc_duracao });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkDuracao).store = stateStore;
		        dijit.byId(linkDuracao).set("value", 0);
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function loadDuracoesAtivo(data, linkDuracao) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try{
		        var items = [];
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_duracao, name: value.dc_duracao });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(linkDuracao).store = stateStore;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}
function IncluirLinhaProg() {
    try{
        var gridProg = dijit.byId("gridProg");

        var myNewItem = {
            nm_aula_programacao: document.getElementById('nm_aula_programacao').value,
            dc_aula_programacao: document.getElementById('dc_aula_programacao').value,
            selecionadoProg: false
        };

        gridProg.store.newItem(myNewItem);
        gridProg.store.save();

        dijit.byId("dialogGridProg").hide();
    } catch (e) {
        postGerarLog(e);
    }
}


function limparProgramacao() {
    apresentaMensagem('apresentadorMensagemProgramacao', null);
    require(["dojo/data/ObjectStore",
              "dojo/store/Memory"
    ], function (ObjectStore, Memory) {
        try{
            getLimpar('#formProgramacao');
            clearForm('formProgramacao');
            document.getElementById("cd_programacao_curso").value = '';
            document.getElementById("cursoAtivo").value = '';
            document.getElementById("duracaoAtiva").value = '';
            //retornaProgramacaoCurso(0);
            if (hasValue(gridProg)) {
                var data = new Array();
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: data }) });
                dijit.byId("gridProg").setStore(dataStore);
                dijit.byId("gridProg").update();
            }
        } catch (e) {
            postGerarLog(e);
        }
    });

}

function EnviarDadosProgramacao() {
        IncluirProgramacao();
}


function montaBotoesProgramacao() {
    require([
          "dojo/_base/xhr",
          "dojo/store/Cache",
          "dijit/form/Button",
          "dojo/ready"
    ], function (xhr, Cache, Button, ready) {
        ready(function () {
            try{
                new Button({
                    label: "",
                    iconClass: 'dijitEditorIconSearchSGF',
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); PesquisarProgramacao(true); }
                }, "pesquisarProgramacao");
                decreaseBtn(document.getElementById("pesquisarProgramacao"), '32px');

                // Programacao
                new Button({
                    label: "Novo",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        try {
                            if (!eval(MasterGeral())) {
                                var mensagensWeb = new Array();
                                mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroInclusaoFundacao);
                                apresentaMensagem("apresentadorMensagem", mensagensWeb);
                                return;
                            }
                            showCarregando();
                            //habilita Cancela e some como limpar
                            document.getElementById('divLimparProgramacao').style.display = "";
                            document.getElementById('divCancelarProgramacao').style.display = "none";

                            dijit.byId("gridProg").itensSelecionados = [];
                            dijit.byId("cursoAtivo").set("disabled", false);
                            dijit.byId("duracaoAtiva").set("disabled", false);

                            dijit.byId("incluirProgGrid").set("disabled", true);
                            loadGridProgramacao(0, 0, 0);
                            xhr.get({
                                url: Endereco() + "/api/Curso/getCursos?cd_curso=",
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }).then(function (dataCursoAtivo) {
                                xhr.get({
                                    url: Endereco() + "/api/Coordenacao/getDuracoes?cd_duracao=",
                                    preventCache: true,
                                    handleAs: "json",
                                    headers: { "Accept": "application/json", "Authorization": Token() }
                                }).then(function (dataDuracoesAtivo) {
                                    try{
                                        loadCursoAtivo(dataCursoAtivo.retorno, 'cursoAtivo');
                                        loadDuracoesAtivo(jQuery.parseJSON(dataDuracoesAtivo).retorno, 'duracaoAtiva');
                                        limparProgramacao();
                                        edicaoNovo = true;
                                        dijit.byId("cad").show();
                                        showCarregando();
                                    } catch (e) {
                                        postGerarLog(e);
                                    }
                                },
                                function (error) {
                                    apresentaMensagem('apresentadorMensagem', error);
                                    showCarregando();
                                });
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                                showCarregando();
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "novoProgramacao");
                new Button({
                    label: getNomeLabelRelatorio(),
                    iconClass: 'dijitEditorIcon dijitEditorIconNewPage',
                    onClick: function () {
                        var cdCurso = dijit.byId("cbCurso").get("value") == null || dijit.byId("cbCurso").get("value") == "" ? 0 : dijit.byId("cbCurso").get("value");
                        var cdDuracao = dijit.byId("cbDuracao").get("value") == null || dijit.byId("cbDuracao").get("value") == "" ? 0 : dijit.byId("cbDuracao").get("value");
                            xhr.get({
                                url: Endereco() + "/api/coordenacao/GetUrlRelatorioProgramacaoCursoSearch?" + getStrGridParameters('gridProgramacao') + "cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                try{
                                    abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '765px', '750px', 'popRelatorio');
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            },
                        function (error) {
                            apresentaMensagem('apresentadorMensagem', error);
                        });
                    }
                }, "relatorioProgramacao");
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function IncluirProgramacao() {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemProgramacao', null);
        if (!dijit.byId("formProgramacao").validate())
            return false;
        var cdProgramacaoCurso = hasValue(dojo.byId("cd_programacao_curso").value) ? dojo.byId("cd_programacao_curso").value : 0;
        require(["dojo/dom",  "dojo/request/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            var itensProg = [];
            $.each(dijit.byId("gridProg")._by_idx, function (index, value) {
                itensProg.push({
                    "cd_item_programacao" : value.item.cd_item_programacao,
                    "dc_aula_programacao" : value.item.dc_aula_programacao,
                    "nm_aula_programacao" : value.item.nm_aula_programacao
                });
            });
            showCarregando();
            xhr.post(Endereco() + "/api/coordenacao/PostProgramacaoCurso", {
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                handleAs: "json",
                data: ref.toJson({
                    cd_programacao_curso: cdProgramacaoCurso,//dojo.byId("cd_programacao_curso").value,
                    cd_curso: dijit.byId("cursoAtivo").value,
                    cd_duracao: dijit.byId("duracaoAtiva").value,
                    itens: itensProg
                })
            }).then(function (data) {
                try{
                    //  data = jQuery.parseJSON(eval(data));
                    if (!hasValue(data.erro)) {
                        var itemAlterado = data.retorno;
                        var item = [];
                        var gridName = 'gridProgramacao';
                        var grid = dijit.byId(gridName);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId('cad').hide();

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];
                        removeObjSort(grid.itensSelecionados, "cd_programacao_curso", dom.byId("cd_programacao_curso").value);
                        if (hasValue(itemAlterado) && itemAlterado.cd_programacao_curso > 0) {
                            insertObjSort(grid.itensSelecionados, "cd_programacao_curso", itemAlterado);

            		        buscarItensSelecionados(gridName, 'selecionado', 'cd_programacao_curso', 'selecionaTodos', ['pesquisarProgramacao', 'relatorioProgramacao'], 'todosItens');
                            grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                            setGridPagination(grid, itemAlterado, "cd_programacao_curso");
                        }
                        else
                            PesquisarProgramacao(false);
                    }
                    else
                        apresentaMensagem('apresentadorMensagemProgramacao', data);
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                showCarregando();
                apresentaMensagem('apresentadorMensagemProgramacao', error);
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function retornaProgramacaoCurso(cdProgramacao) {
    showCarregando();
    require([
               "dojo/_base/xhr",
               "dojo/data/ObjectStore",
               "dojo/store/Cache",
               "dojo/store/Memory",
               "dojo/ready"
    ], function (xhr, ObjectStore, Cache, Memory, ready) {
        xhr.get({
            url: Endereco() + "/api/coordenacao/getItensProgramacaoCursoById?cdProgramacao=" + cdProgramacao,
            preventCache: true,
            handleAs: "json",
            headers: { "Accept": "application/json", "Authorization": Token() },
            idProperty: "nm_aula_programacao"
        }).then(function (data) {
            ready(function () {
                try {
                    data = jQuery.parseJSON(data).retorno;
                    var dataStore = new ObjectStore({ objectStore: new Memory({ data: data, idProperty: "nm_aula_programacao" }) });
                    var gridProg = dijit.byId("gridProg");

                    gridProg.setStore(dataStore);

                    if (gridProg._by_idx == 0) {
                        document.getElementById('divProgCima').style.display = "none";
                        document.getElementById('divProgBaixo').style.display = "none";
                        document.getElementById('divIncluirProgramacao').style.display = "none";
                        document.getElementById('divCancelarProgramacao').style.display = "none";
                        document.getElementById('divLimparProgramacao').style.display = "";

                    }
                    else {
                        document.getElementById('divProgCima').style.display = "";
                        document.getElementById('divProgBaixo').style.display = "";
                        document.getElementById('divIncluirProgramacao').style.display = "";
                        document.getElementById('divCancelarProgramacao').style.display = "";
                        document.getElementById('divLimparProgramacao').style.display = "none";
                    }
                    showCarregando();
                } catch (e) {
                    showCarregando();
                    postGerarLog(e);
                }
            });
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemProgramacao', error);
        });
    });
}
function PesquisarProgramacao(limparItens) {

    require([
              "dojo/store/JsonRest",
              "dojo/data/ObjectStore",
              "dojo/store/Cache",
              "dojo/store/Memory"
    ], function ( JsonRest, ObjectStore, Cache, Memory) {
        try{
            var cdCurso = dijit.byId("cbCurso").get("value") == null || dijit.byId("cbCurso").get("value") == "" ? 0 : dijit.byId("cbCurso").get("value");
            var cdDuracao = dijit.byId("cbDuracao").get("value") == null || dijit.byId("cbDuracao").get("value") == "" ? 0 : dijit.byId("cbDuracao").get("value");

            var myStore = Cache(
                    JsonRest({
                        handleAs: "json",
                        target: Endereco() + "/api/coordenacao/getProgramacaoCursoSearch?cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao,
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({  }));
            dataStore = new ObjectStore({ objectStore: myStore });

            var gridProgramacao = dijit.byId("gridProgramacao");

            if (limparItens)
                gridProgramacao.itensSelecionados = [];

            dijit.byId("gridProgramacao").setStore(dataStore);
            dijit.byId("gridProgramacao").update();
        } catch (e) {
            postGerarLog(e);
        }
    });
}
function AlterarProgramacao() {
    try{
        var gridName = 'gridProgramacao';
        var grid = dijit.byId(gridName);
        if (!dijit.byId("formProgramacao").validate())
            return false;
        var itensProg = [];
        $.each(dijit.byId("gridProg")._by_idx, function (index, value) {
            itensProg.push({
                "dc_aula_programacao": value.item.dc_aula_programacao,
                "nm_aula_programacao": value.item.nm_aula_programacao
            });
        });
        var descBranco = 0;
        for (var i = 0; i < itensProg.length; i++)
            if (itensProg[i].dc_aula_programacao == "" || itensProg[i].dc_aula_programacao == null) {
                descBranco = 1;
                break;
            }
        if (descBranco <= 0) {
            var cdDuracao = hasValue(dijit.byId("duracaoAtiva").value) ? dijit.byId("duracaoAtiva").value : 0;
            showCarregando();
            require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
                xhr.post({
                    url: Endereco() + "/api/coordenacao/PostAlterarProgramacaoCurso",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                    handleAs: "json",
                    postData: ref.toJson({
                        cd_programacao_curso: dojo.byId("cd_programacao_curso").value,
                        cd_curso: dijit.byId("cursoAtivo").value,
                        cd_duracao: dijit.byId("duracaoAtiva").value,
                        itens: itensProg
                    })
                }).then(function (data) {
                    try{
                        var itemAlterado = data.retorno;

                        if (!hasValue(grid.itensSelecionados))
                            grid.itensSelecionados = [];

                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();
                        removeObjSort(grid.itensSelecionados, "cd_programacao_curso", dom.byId("cd_programacao_curso").value);
                        insertObjSort(grid.itensSelecionados, "cd_programacao_curso", itemAlterado);

                        buscarItensSelecionados(gridName, 'selecionado', 'cd_programacao_curso', 'selecionaTodos', ['pesquisarProgramacao', 'relatorioProgramacao'], 'todosItens');
                        grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                        setGridPagination(grid, itemAlterado, "cd_programacao_curso");
                        showCarregando();
                    } catch (e) {
                        postGerarLog(e);
                    }
                },
                function (error) {
                    showCarregando();
                    apresentaMensagem('apresentadorMensagemProgramacao', error);
                });
            });
        }
        else
            alert("Não é possivel alterar item sem descrição");
    } catch (e) {
        postGerarLog(e);
    }
}

function DeletarProgramacao(itensSelecionados) {
    try{
        apresentaMensagem('apresentadorMensagem', null);
        apresentaMensagem('apresentadorMensagemProgramacao', null);
        require(["dojo/dom", "dojo/_base/xhr", "dojox/json/ref"], function (dom, xhr, ref) {
            var nrsProg = [];
            $.each(dijit.byId("gridProg")._by_idx, function (index, value) {
                nrsProg.push({
                    "dc_aula_programacao" : value.dc_aula_programacao,
                    "nm_aula_programacao" : value.nm_aula_programacao
                });
            });
            if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
                if (dojo.byId('cd_programacao_curso').value != 0)
                    itensSelecionados = [{
                        cd_programacao_curso: dom.byId("cd_programacao_curso").value,
                        cd_curso: dom.byId("cursoAtivo").value,
                        cd_duracao: dom.byId("duracaoAtiva").value,
                        dcProgs: nrsProg
                    }];
            }
            xhr.post({
                url: Endereco() + "/api/coordenacao/postDeleteProgramacaoCurso",
                headers: { "Content-Type": "application/json", "Content-Type": "application/json", "Authorization": Token() },
                postData: ref.toJson(itensSelecionados)
            }).then(function (data) {
                try{
                    if (!hasValue(data.erro)) {
                        var todos = dojo.byId("todosItens_label");
                        PesquisarProgramacao(false);
                        apresentaMensagem('apresentadorMensagem', data);
                        dijit.byId("cad").hide();
                        dijit.byId("pesquisarProgramacao").set("value", '');

                        // Remove o item dos itens selecionados:
                        for (var r = itensSelecionados.length - 1; r >= 0; r--)
                            removeObjSort(dijit.byId('gridProgramacao').itensSelecionados, "cd_programacao_curso", itensSelecionados[r].cd_programacao_curso);



                        // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                        dijit.byId("pesquisarProgramacao").set('disabled', false);
                        dijit.byId("relatorioProgramacao").set('disabled', false);

                        if (hasValue(todos))
                            todos.innerHTML = "Todos Itens";
                    }
                    else
                        apresentaMensagem('apresentadorMensagemProgramacao', data);
                } catch (e) {
                    postGerarLog(e);
                }
            },
            function (error) {
                if (!hasValue(dojo.byId("cad").style.display))
                    apresentaMensagem('apresentadorMensagemProgramacao', error);
                else
                    apresentaMensagem('apresentadorMensagem', error);

            });
        })
    } catch (e) {
        postGerarLog(e);
    }
}

function montarProgSelecionados() {
    try{
        var dados = [];
        if (hasValue(dijit.byId("gridProg")) && hasValue(dijit.byId("gridProg").store.objectStore.data)) {
            var storeDias = dijit.byId("gridProg").store.objectStore.data;
            $.each(storeDias, function (index, val) {
                if (val.selecionadoProg == true)
                    dados.push(val);
            });
            return dados;
        }
        return null;
    } catch (e) {
        postGerarLog(e);
    }
}


function montarProgSelecionadas() {
    try{
        var dados = [];
        if (hasValue(dijit.byId("gridProg")) && hasValue(dijit.byId("gridProg").store.objectStore.data)) {
            var storeAval = dijit.byId("gridProg").itensSelecionados;
            $.each(storeAval, function (index, val) {
                dados.push(val);
            });
            return dados;
        }
        return null;
    } catch (e) {
        postGerarLog(e);
    }
}

function deletarLinhaProg() {
    try{
        var gridProg = dijit.byId("gridProg");
        if (gridProg.itensSelecionados == null || gridProg.itensSelecionados.length == 0) {
            caixaDialogo(DIALOGO_AVISO, msgSelecioneRegExcluir, null);
            return null;
        }
        require(["dojo/store/Memory", "dojo/data/ObjectStore"],
        function (Memory, ObjectStore) {
            var aval = montarProgSelecionadas();

            if (aval.length > 0) {
                var arrayAval = dijit.byId("gridProg")._by_idx;
                $.each(aval, function (idx, valueAval) {

                    arrayAval = jQuery.grep(arrayAval, function (value) {
                        return value.item != valueAval;
                    });
                });
                var dados = [];
                $.each(arrayAval, function (index, value) {
                    dados.push(value.item);
                });
                var dataStore = new ObjectStore({ objectStore: new Memory({ data: dados }) });
                dijit.byId("gridProg").setStore(dataStore);
                var mensagensWeb = new Array();
                mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, "Programações excluídas com sucesso.");
            }
            for (var l = dijit.byId("gridProg")._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                dijit.byId("gridProg")._by_idx[l].item.nm_aula_programacao = l+1;
            dijit.byId("gridProg").itensSelecionados = [];
            dijit.byId("gridProg").update();

        });
    } catch (e) {
        postGerarLog(e);
    }
}
function atualiza(i) {
    try{
        var gridProg = dijit.byId('gridProg');
        gridProg.getRowNode(i).id = '';
        gridProg.getRowNode(i).id = 'ordem_' + gridProg._by_idx[i].item.cd_item_programacao;
        window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
        window.location.hash = '#ordem_' + gridProg._by_idx[i].item.gridProg;
    } catch (e) {
        postGerarLog(e);
    }
}

function alterarProgGrid(grid) {
    try{
        if (!dijit.byId("formAlteraProg").validate())
            return false;
        for (var l = 0; l < grid._by_idx.length ; l++) {
            if (grid._by_idx[l].item.nm_aula_programacao == ordemOri) {
                // Muda os itens de lugares e altera o registro na grade:
                grid._by_idx[l].item.dc_aula_programacao = dojo.byId("dc_aula_programacao").value;

                if (grid._by_idx[l].item.nm_aula_programacao < parseInt(document.getElementById('nm_aula_programacao').value))
                    grid._by_idx[l].item.nm_aula_programacao = parseInt(document.getElementById('nm_aula_programacao').value) + 0.5;
                else
                    grid._by_idx[l].item.nm_aula_programacao = parseInt(document.getElementById('nm_aula_programacao').value) - 0.5;
                //break;
            }
            grid._by_idx[l].nm_aula_programacao = grid._by_idx[l].item.nm_aula_programacao;
        }
        quickSortObj(grid._by_idx, 'nm_aula_programacao');

        for (var l = grid._by_idx.length - 1, j = 1; l >= 0; l--, j++)
            grid._by_idx[l].item.nm_aula_programacao = l+1;
        grid.update();

        for (var i = 0; i <= grid._by_idx.length - 1; i++) {
            grid.selection.setSelected(i, false);
            if (grid._by_idx[i].item.cd_item_programacao == parseInt(document.getElementById('cd_item_programacao').value)) {
                //Seleciona a linha com o item editado por default:
                grid.selection.setSelected(i, true);
                ////Posiciona o foco do scroll no item editado:
                grid.getRowNode(i).id = '';
                grid.getRowNode(i).id = 'ordem_' + parseInt(document.getElementById('cd_item_programacao').value);
                window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                window.location.hash = '#ordem_' + parseInt(document.getElementById('cd_item_programacao').value);
                //break;
            }
        }
        dijit.byId("dialogGridProg").hide();
    } catch (e) {
        postGerarLog(e);
    }
}

function incluiProgGrid(myNewItem) {
    try{
        var gridProg = dijit.byId("gridProg");
        if (!dijit.byId("formAlteraProg").validate())
            return false;
        if (hasValue(document.getElementById('cursoAtivo').value) && hasValue(document.getElementById('duracaoAtiva').value)) {
            if (!hasValue(myNewItem))
                myNewItem = {
                    cd_programacao_curso: dojo.byId("cd_programacao_curso").value,
                    cd_item_programacao: dojo.byId("cd_item_programacao").value,
                    dc_aula_programacao: dojo.byId("dc_aula_programacao").value,
                    nm_aula_programacao: dojo.byId("nm_aula_programacao").value
                };

            gridProg.store.newItem(myNewItem);
            gridProg.store.save();

            for (var l = 0; l < gridProg._by_idx.length ; l++)
                gridProg._by_idx[l].nm_aula_programacao = gridProg._by_idx[l].item.nm_aula_programacao;
            quickSortObj(gridProg._by_idx, 'nm_aula_programacao');

            for (var l = gridProg._by_idx.length - 1, j = 1; l >= 0; l--, j++)
                gridProg._by_idx[l].item.nm_aula_programacao = l+1;

            //gridEstagioOrdem.update();
            for (var i = 0; i <= gridProg._by_idx.length - 1; i++) {
                gridProg.selection.setSelected(i, false);
                if (gridProg._by_idx[i].item.nm_aula_programacao == myNewItem.nm_aula_programacao) {
                    //Seleciona a linha com o item editado por default:
                    gridProg.selection.setSelected(i, true);
                    //Posiciona o foco do scroll no item editado:
                    setTimeout('atualiza(' + i + ')', 10);
                }
            }
            if (gridProg._by_idx == 0) {
                document.getElementById('divProgCima').style.display = "none";
                document.getElementById('divProgBaixo').style.display = "none";
                document.getElementById('divIncluirProgramacao').style.display = "none";

            }
            else {
                document.getElementById('divProgCima').style.display = "";
                document.getElementById('divProgBaixo').style.display = "";
                document.getElementById('divIncluirProgramacao').style.display = "";

            }
            dijit.byId("dialogGridProg").hide();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function subirDescerOrdemProg(grid, sobeDesce) {
    try{
        var operacao = sobeDesce == SOBE ? 1 : -1;
        var itemSelecionado = grid.itensSelecionados;

        if (itemSelecionado.length > 1)
            caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
        else
            if (itemSelecionado.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgSelectRegOrdem, null);
            else {
                for (var l = 0; l < grid._by_idx.length; l++)
                    if (grid._by_idx[l].item.nm_aula_programacao == itemSelecionado[0].nm_aula_programacao) {
                        if (hasValue(grid._by_idx[l - (operacao)])) {
                            var itemEncontrado = grid._by_idx[l].item;
                            var ordemEncontrada = grid._by_idx[l].item.nm_aula_programacao;
                            var posicaoEncontrada = grid.selection.selectedIndex;

                            // Muda as ordens de lugares:
                            grid._by_idx[l].item.nm_aula_programacao = grid._by_idx[l - (operacao)].item.nm_aula_programacao;
                            grid._by_idx[l - (operacao)].item.nm_aula_programacao = ordemEncontrada;

                            // Muda os itens de lugares:
                            grid._by_idx[l].item = grid._by_idx[l - (operacao)].item;
                            grid._by_idx[l - (operacao)].item = itemEncontrado;

                            grid.update();

                            grid.getRowNode(l).id = '';
                            grid.getRowNode(l - (operacao)).id = 'ordem_' + grid._by_idx[l - (operacao)].item.nm_aula_programacao;
                            window.location.hash = '#algumHashQueNaoExiste'; // Limpa o hash para funcionar nos browsers Chrome e Safari.
                            window.location.hash = '#' + 'ordem_' + grid._by_idx[l - (operacao)].item.nm_aula_programacao;

                            // Atualiza o item selecionado:
                            grid.selection.setSelected(posicaoEncontrada, false);
                            if (posicaoEncontrada < grid._by_idx.length)
                                grid.selection.setSelected(posicaoEncontrada - 1, true);
                            var codAlteradoSubir = grid._by_idx[l].item.nm_aula_programacao + ";" + grid._by_idx[l - (operacao)].item.nm_aula_programacao + ";";
                        }
                        return codAlteradoSubir;
                        break;
                    }
            }
    } catch (e) {
        postGerarLog(e);
    }
}