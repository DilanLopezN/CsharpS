var PERCENTUAL = 1, VALOR = 2;

function montarCadastroReajusteAnual() {
    //Criação da Grade de sala
    require([
    "dojo/_base/xhr",
    "dojox/grid/EnhancedGrid",
    "dojox/grid/enhanced/plugins/Pagination",
    "dojo/store/JsonRest",
    "dojo/data/ObjectStore",
    "dojo/store/Cache",
    "dojo/store/Memory",
    "dijit/form/Button",
    "dojo/ready",
    "dojo/on",
    "dojo/_base/array",
    "dijit/Tooltip",
    "dojox/form/Uploader",
    "dojox/json/ref",
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dojo/dom"
    ], function (xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, Button, ready, on, array, Tooltip, Uploader, ref, DropDownButton, DropDownMenu, MenuItem, dom) {
        ready(function () {
            try {
                var myStore =
                    Cache(
                            JsonRest({
                                target: Endereco() + "/api/financeiro/getReajusteAnualSearch?cd_usuario=0&status=0&dtInicial=&dtFinal=&cadastro=false&vctoInicial=false&cd_reajuste_anual=0",
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({}));
                var gridReajusteAnual = new EnhancedGrid({
                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    //store: new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                    structure:
                    [
						{ name: "<input id='selecionaTodosReajusteAnual' style='display:none'/>", field: "selecionadoReajusteAnual", width: "15px", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBoxReajusteAnual },
                        { name: "Número", field: "cd_reajuste_anual", width: "45px", styles: "min-width:80px;text-align:center;" },
                        { name: "Data Cadastro", field: "dh_cadastro", width: "100px", styles: "min-width:80px;" },
						{ name: "Usuário", field: "no_login", width: "15%", styles: "min-width:80px;" },
						{ name: "Percentual", field: "pc_reajuste", width: "55px", styles: "min-width:70px;text-align:right;" },
						{ name: "Valor", field: "vl_reajuste", width: "50px", styles: "min-width:70px;text-align:right;" },
						{ name: "Vcto.Inicial", field: "dt_inicial_vcto", width: "70px", styles: "min-width:70px;" },
						{ name: "Vcto.Final", field: "dt_final_vcto", width: "70px", styles: "min-width:70px;" },
						{ name: "Turmas", field: "qtd_turmas", width: "50px", styles: "min-width:70px;text-align:center;" },
						{ name: "Cursos", field: "qtd_cursos", width: "50px", styles: "min-width:70px;text-align:center;" },
						{ name: "Alunos", field: "qtd_alunos", width: "50px", styles: "min-width:70px;text-align:center;" },
						{ name: "Títulos", field: "qtd_titulos", width: "70px", styles: "min-width:70px;text-align:center;" },
						{ name: "Status", field: "dc_status", width: "50px", styles: "min-width:70px;" }
                    ],
                    canSort: true,
                    selectionMode: "single",
                    noDataMessage: "Nenhum registro encontrado.",
                    plugins: {
                        pagination: {
                            pageSizes: ["17", "34", "68", "100", "All"],
                            description: true,
                            sizeSwitch: true,
                            pageStepper: true,
                            defaultPageSize: "17",
                            gotoButton: true,
                            maxPageStep: 5,
                            position: "button"
                        }
                    }
                }, "gridReajusteAnual");
                gridReajusteAnual.startup();
                gridReajusteAnual.itensSelecionados = [];
                gridReajusteAnual.pagination.plugin._paginator.plugin.connect(gridReajusteAnual.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    verificaMostrarTodos(evt, gridReajusteAnual, 'cd_reajuste_anual', 'selecionaTodosReajusteAnual');
                });
                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridReajusteAnual, "_onFetchComplete", function () {
                        // Configura o check de todos:
                        if (dojo.byId('selecionaTodosReajusteAnual').type == 'text')
                            setTimeout("configuraCheckBox(false, 'cd_reajuste_anual', 'selecionadoReajusteAnual', -1, 'selecionaTodosReajusteAnual', 'selecionaTodosReajusteAnual', 'gridReajusteAnual')", gridReajusteAnual.rowsPerPage * 3);
                    });
                });
                gridReajusteAnual.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 && Math.abs(col) != 10 && Math.abs(col) != 11 };
                gridReajusteAnual.on("RowDblClick", function (evt) {
                    try {
                        var idx = evt.rowIndex, item = this.getItem(idx), store = this.store;
                        gridReajusteAnual.itemSelecionado = item;
                        limparReajusteAnual(false);
                        keepValuesReajusteAnual(gridReajusteAnual);
                        apresentaMensagem('apresentadorMensagem', '');
                        IncluirAlterar(0, 'divAlterarReajusteAnual', 'divIncluirReajusteAnual', 'divExcluirReajusteAnual', 'apresentadorMensagemReajusteAnual', 'divCancelarReajusteAnual', 'divClearReajusteAnual');
                        dijit.byId("cadReajusteAnual").show();
                    } catch (e) {
                        postGerarLog(e);
                    }
                }, true);

                new Button({
                    label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                        try {
                            var grid = dijit.byId("gridReajusteAnual");
                            var cd_usuario = hasValue(dijit.byId("cbUsuario").value) ? dijit.byId("cbUsuario").value : 0;
                            var id_status = hasValue(dijit.byId("cbStatus").value) ? dijit.byId("cbStatus").value : 0;
                            var cd_reajuste_anual = hasValue(dijit.byId("cdReajusteAnual").value) ? dijit.byId("cdReajusteAnual").value : 0;
                            xhr.get({
                                url: Endereco() + "/api/financeiro/getUrlRelatorioReajusteAnual?" + getStrGridParameters('gridReajusteAnual') + "cd_usuario=" + parseInt(cd_usuario) + "&status=" + parseInt(id_status) + "&dtInicial=" + dojo.byId("dtaInicial").value +
                                                       "&dtFinal=" + dojo.byId('dtaFinal').value + "&cadastro=" + dijit.byId("id_cadastro").checked + "&vctoInicial=" + dijit.byId("id_vcto_inicial").checked + "&cd_reajuste_anual=" + cd_reajuste_anual,
                                preventCache: true,
                                handleAs: "json",
                                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                            }).then(function (data) {
                                abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                            },
                            function (error) {
                                apresentaMensagem('apresentadorMensagem', error);
                            });
                        } catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "relatorio");
                new Button({
                    label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                            function () {
                                try {
                                    showCarregando();
                                    limparReajusteAnual();
                                    IncluirAlterar(1, 'divAlterarReajusteAnual', 'divIncluirReajusteAnual', 'divExcluirReajusteAnual', 'apresentadorMensagemReajusteAnual', 'divCancelarReajusteAnual', 'divClearReajusteAnual');
                                    loadComponentesNovo(EnhancedGrid, ObjectStore, Memory, Pagination, on);
                                    dijit.byId("cadReajusteAnual").show();
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            }
                }, "novo");

                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { incluirReajusteAnual(); } }, "incluirReajusteAnual");
                new Button({
                    label: "Cancelar", iconClass: 'dijitEditorIcon dijitEditorIconTabIndent', onClick: function () {
                        keepValuesReajusteAnual(dijit.byId('gridReajusteAnual'));
                    }
                }, "cancelarReajusteAnual");
                new Button({ label: "Fechar", iconClass: 'dijitEditorIcon dijitEditorIconCancel', onClick: function () { dijit.byId("cadReajusteAnual").hide(); } }, "fecharReajusteAnual");
                new Button({ label: "Salvar", iconClass: 'dijitEditorIcon dijitEditorIconSave', onClick: function () { alterarReajusteAnual(); } }, "alterarReajusteAnual");
                new Button({
                    label: "Excluir", iconClass: 'dijitEditorIcon dijitEditorIconDelete', onClick: function () {
                        caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarReajustesAnuais(gridReajusteAnual.itensSelecionados); });
                    }
                }, "deleteReajusteAnual");
                new Button({ label: "Limpar", iconClass: 'dijitEditorIcon dijitEditorIconRemoveFormat', type: "reset", onClick: function () { limparReajusteAnual(); } }, "limparReajusteAnual");

                criarAcaoRelacionadaPesquisa(gridReajusteAnual);

                new Button({
                    label: "",
                    onClick: function () { apresentaMensagem('apresentadorMensagem', null); pesquisarReajusteAnual(true); },
                    iconClass: 'dijitEditorIconSearchSGF'
                }, "pesquisarReajusteAnual");
                decreaseBtn(document.getElementById("pesquisarReajusteAnual"), '32px');

                var dadosPesqStatus = [
                    { name: "Todos", id: "0" },
                    { name: "Aberto", id: "1" },
                    { name: "Fechado", id: "2" }
                ];
                criarOuCarregarCompFiltering("cbStatus", dadosPesqStatus, "", null, dojo.ready, dojo.store.Memory, dijit.form.FilteringSelect, 'id', 'name');
                if (hasValue(dijit.byId("menuManual"))) {
                    dijit.byId("menuManual").on("click",
                        function(e) {
                            abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323071', '765px', '771px');
                        });
                }

                loadUsuarios('cbUsuario');

                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                            montarGridPesquisaAluno(false, function () {
                                abrirAlunoFk();
                            });
                        }
                        else
                            abrirAlunoFk();
                    }
                }, "btIncluirAluno");
                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF',
                    onClick: function () {
                        limparPesquisaCursoFK(true);
                        dijit.byId("proCurso").show();
                        pesquisarCursoReajusteAnualFK();
                    }
                }, "btIncluirCurso");
                new Button({
                    label: "Incluir",
                    iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick: function () {
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
                    }
                }, "btIncluirTurma");

                // Adiciona link de ações Alunos
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_aluno', dijit.byId("gridAluno")); }
                });
                menu.addChild(acaoExcluir);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasAluno",
                    dropDown: menu,
                    id: "acoesRelacionadasAluno"
                });
                dom.byId("linkAcoesAluno").appendChild(button.domNode);

                menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_turma', dijit.byId("gridTurma")); }
                });
                menu.addChild(acaoExcluir);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasTurma",
                    dropDown: menu,
                    id: "acoesRelacionadasTurma"
                });
                dom.byId("linkAcoesTurma").appendChild(button.domNode);

                menu = new DropDownMenu({ style: "height: 25px" });
                var acaoExcluir = new MenuItem({
                    label: "Excluir",
                    onClick: function () { deletarItemSelecionadoGrid(Memory, ObjectStore, 'cd_curso', dijit.byId("gridCurso")); }
                });
                menu.addChild(acaoExcluir);

                button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadasCurso",
                    dropDown: menu,
                    id: "acoesRelacionadasCurso"
                });
                dom.byId("linkAcoesCurso").appendChild(button.domNode);

                dijit.byId("pesquisarCursoFK").on("click", function (e) {
                    apresentaMensagem(dojo.byId("apresentadorMensagemFks").value, null);
                    pesquisarCursoFK();
                });
                adicionarAtalhoPesquisa(['cbUsuario', 'cbStatus', 'dtaInicial', 'dtaFinal', 'id_cadastro', 'id_vcto_inicial'], 'pesquisarReajusteAnual', ready);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function pesquisarCursoReajusteAnualFK() {
    require([
            "dojo/store/JsonRest",
            "dojo/data/ObjectStore",
            "dojo/store/Cache",
            "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            if (!hasValue(dijit.byId("cbPesqProdutoFK").value) && hasValue(dijit.byId("cbPesqProdutoFK").produto_selecionado))
                dijit.byId("cbPesqProdutoFK").set("value", dijit.byId("cbPesqProdutoFK").produto_selecionado);
            var dataInicial = dojo.byId('dt_inicial_vencimento').value;
            var dataFinal = dojo.byId('dt_final_vencimento').value;
            var myStore = Cache(
                    JsonRest({
                        target: !hasValue(document.getElementById("descCursoFK").value) ? Endereco() + "/api/curso/getCursoFKSearch?desc=&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=" + retornaStatus("statusCursoFK") + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&dataInicial=" + dataInicial + "&dataFinal=" + dataFinal : Endereco() + "/api/curso/getCursoFKSearch?desc=" + encodeURIComponent(document.getElementById("descCursoFK").value) + "&inicio=" + document.getElementById("inicioCursoFK").checked + "&status=" + retornaStatus("statusCursoFK") + "&produto=" + dijit.byId('cbPesqProdutoFK').value + "&estagio=" + dijit.byId('cbPesqEstagiosFK').value + "&modalidade=" + dijit.byId('cbPesqModalidadesFK').value + "&dataInicial=" + dataInicial + "&dataFinal=" + dataFinal,
                        handleAs: "json",
                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                    }
            ), Memory({}));
            dataStore = new ObjectStore({ objectStore: myStore });
            var grid = dijit.byId("gridPesquisaCurso");
            grid.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function pesquisaAlunoReajusteAnual(pesquisaHabilitada) {
    try {
        var sexo = hasValue(dijit.byId("nm_sexo_fk").value) ? dijit.byId("nm_sexo_fk").value : 0;
        if (pesquisaHabilitada)
            require([
                    "dojo/store/JsonRest",
                    "dojo/data/ObjectStore",
                    "dojo/store/Cache",
                    "dojo/store/Memory"
            ], function (JsonRest, ObjectStore, Cache, Memory) {
                try {
                    var dataInicial = dojo.byId('dt_inicial_vencimento').value;
                    var dataFinal = dojo.byId('dt_final_vencimento').value;
                    myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/aluno/getAlunoFKReajusteSearch?nome=" + dojo.byId("nomeAlunoFk").value + "&email=" + dojo.byId("emailPesqFK").value + "&status=" + dijit.byId("statusAlunoFK").value + "&cnpjCpf=" + dojo.byId("cpf_fk").value + "&inicio=" + document.getElementById("inicioAlunoFK").checked + "&cdSituacoes=100&sexo=" + sexo + "&semTurma=false&movido=false&tipoAluno=0&dataInicial=" + dataInicial + "&dataFinal=" + dataFinal,
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
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFk() {
    dojo.byId('tipoRetornoAlunoFK').value = REAJUSTE_ANUAL_ALUNO;
    dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
    limparPesquisaAlunoFK();
    pesquisaAlunoReajusteAnual(true);
    dijit.byId("proAluno").show();
    dijit.byId('gridPesquisaAluno').update();
}

function abrirTurmaFK() {
    try {
        limparFiltrosTurmaFK();
        dojo.byId("idOrigemCadastro").value = REAJUSTE_ANUAL;
        dijit.byId("proTurmaFK").show();
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            if (compProfpesquisa.disabled == true && hasValue(compProfpesquisa.value))
                compProfpesquisa.set('disabled', true);
        }
        dijit.byId("tipoTurmaFK").on("change", function (e) {
            if (this.displayedValue == "Personalizada") {
                dijit.byId("pesTurmasFilhasFK").set("checked", true);
                dijit.byId('pesTurmasFilhasFK').set('disabled', true);
                loadSituacaoTurmaFKAbertas(dojo.store.Memory);
            } else {
                dijit.byId("pesTurmasFilhasFK").set("checked", false);
                dijit.byId('pesTurmasFilhasFK').set('disabled', false);
            }
        });
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
        pesquisarTurmaReajusteAnualFK();
        dojo.byId("trAluno").style.display = "none";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarTurmaReajusteAnualFK() {
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
            var dataInicial = dojo.byId('dt_inicial_vencimento').value;
            var dataFinal = dojo.byId('dt_final_vencimento').value;
            var myStore = Cache(
                    JsonRest({
                        target: Endereco() + "/api/turma/getTurmaReajusteSearchFK?descricao=" + document.getElementById("nomTurmaFK").value + "&apelido=" + document.getElementById("_apelidoFK").value +
                                             "&inicio=" + document.getElementById("inicioTrumaFK").checked + "&tipoTurma=" + cdTipoTurma +
                                            "&cdCurso=" + cdCurso + "&cdDuracao=" + cdDuracao + "&cdProduto=" + cdProduto + "&situacaoTurma=" + cdSitTurma + "&cdProfessor=" + cdProfessor +
                                            "&prog=" + cdProg + "&turmasFilhas=" + document.getElementById("pesTurmasFilhasFK").checked + "&cdAluno=" + cdAluno +
                                            "&dtInicial=&dtFinal=&cd_turma_PPT=null&semContrato=false&dataInicial=" + dataInicial + "&dataFinal=" + dataFinal,
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

function loadComponentesNovo(EnhancedGrid, ObjectStore, Memory, Pagination, on) {
    try {
        dojo.xhr.get({
            url: Endereco() + "/api/secretaria/getComponentesReajusteAnual?cd_nome_contrato=null",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
        }).then(function (data) {
            if (data.indexOf("<!DOCTYPE html>") < 0) {
                var data = jQuery.parseJSON(data).retorno;
                var dataSugerida = data.dataCorrente.dataPortugues.split(" ");
                if (dataSugerida.length > 0) {
                    var date = dojo.date.locale.parse(dataSugerida[0], { formatLength: 'short', selector: 'date', locale: 'pt-br' });
                    dijit.byId('dtaCadastro').set("value", date);
                    var hora = dataSugerida[1].split(":");
                    if (hasValue(hora) && hora[0].length < 2)
                        hora[0] = "0" + hora[0];
                    var horasTimeSpin = "T" + hora[0] + ":" + hora[1] + ":" + hora[2];
                    dijit.byId('horaCadastro').set("value", horasTimeSpin);
                }
            }
            if (hasValue(data.nomesContrato) && data.nomesContrato.length > 0) {
                var cd_nome = null;
                if (data.nomesContrato.length == 1)
                    cd_nome = data.nomesContrato[0].cd_nome_contrato;
                criarOuCarregarCompFiltering("cd_nome_contrato", data.nomesContrato, "", cd_nome, dojo.ready, dojo.store.Memory,
                                                  dijit.form.FilteringSelect, 'cd_nome_contrato', 'no_contrato');
            }
            dojo.byId('no_usuario').value = dojo.byId('nomeUsuario').innerHTML.replace("&nbsp;", "");
            var dados = [
                            { name: "Todos", id: "0" },
                            { name: "Aberto", id: "1" },
                            { name: "Fechado", id: "2" }
            ];
            criarOuCarregarCompFiltering("id_status_reajuste", dados, "", 1, dojo.ready, dojo.store.Memory,
                                                  dijit.form.FilteringSelect, 'id', 'name');

            var dadosTipo = [
                                { name: "Percentual", id: "1" },
                                { name: "Valor", id: "2" }
            ]
            criarOuCarregarCompFiltering("cbTipoCad", dadosTipo, "", 1, dojo.ready, dojo.store.Memory,
                                                  dijit.form.FilteringSelect, 'id', 'name');
            dijit.byId("cbTipoCad").on("change", function (event) {
                if (hasValue(event)) {
                    if (event == PERCENTUAL) {
                        var compPcReajuste = dijit.byId("pc_reajuste_anual");
                        var compVlReajuste = dijit.byId("vl_reajuste_anual");
                        compPcReajuste.set("required", true);
                        compVlReajuste.set("required", false);
                        compPcReajuste.set("disabled", false);
                        compVlReajuste.set("disabled", true);
                        compVlReajuste.reset();
                    }
                    else {
                        var compPcReajuste = dijit.byId("pc_reajuste_anual");
                        var compVlReajuste = dijit.byId("vl_reajuste_anual");
                        compPcReajuste.set("required", false);
                        compVlReajuste.set("required", true);
                        compPcReajuste.set("disabled", true);
                        compVlReajuste.set("disabled", false);
                        compPcReajuste.reset();
                    }
                }
            });
            criaTabelas(EnhancedGrid, ObjectStore, Memory, Pagination);
            showCarregando();
        },
        function (error) {
            showCarregando();
            apresentaMensagem('apresentadorMensagemPessoa', error);
        });
    }
    catch (e) {
        showCarregando();
        postGerarLog(e);
    }
}

function criaTabelas(EnhancedGrid, ObjectStore, Memory, Pagination) {
    if (!hasValue(dijit.byId('gridTurma'))) {
        var gridTurma = new EnhancedGrid({
            store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
            [
                { name: "<input id='selecionaTodosTurma' style='display:none'/>", field: "selecionadoTurma", width: "5%", styles: "text-align:center; min-width:15px; max-width:10px;", formatter: formatCheckBoxTurma },
                { name: "Turma", field: "no_turma", width: "25%", styles: "min-width:80px;" },
                { name: "Apelido", field: "no_apelido", width: "25%", styles: "min-width:80px;" }
            ],
            selectionMode: "single",
            noDataMessage: msgNotRegEnc,
            plugins: {
                pagination: {
                    pageSizes: ["10", "20", "50", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "10",
                    gotoButton: true,
                    maxPageStep: 5,
                    position: "button"
                }
            }
        }, "gridTurma");
        gridTurma.canSort = function (col) { return Math.abs(col) != 1 };
        gridTurma.startup();
    }

    //*** Cria a grade de Cursos **\\
    if (!hasValue(dijit.byId('gridCurso'))) {
        var gridCurso = new EnhancedGrid({
            store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
            [
                { name: "<input id='selecionaTodosCurso' style='display:none'/>", field: "selecionadoCurso", width: "5%", styles: "text-align:center; min-width:15px; max-width:10px;", formatter: formatCheckBoxCurso },
                { name: "Curso", field: "no_curso", width: "80%", styles: "min-width:80px;" }
            ],
            selectionMode: "single",
            noDataMessage: msgNotRegEnc,
            plugins: {
                pagination: {
                    pageSizes: ["10", "20", "50", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "10",
                    gotoButton: true,
                    maxPageStep: 5,
                    position: "button"
                }
            }
        }, "gridCurso");
        gridCurso.canSort = function (col) { return Math.abs(col) != 1 };
        gridCurso.startup();
    }

    if (!hasValue(dijit.byId('gridAluno'))) {

        var gridAluno = new EnhancedGrid({
            store: dataStore = new ObjectStore({ objectStore: new Memory({ data: null }) }),
            structure:
            [
                { name: "<input id='selecionaTodosAluno' style='display:none'/>", field: "selecionadoAluno", width: "5%", styles: "text-align:center; min-width:15px; max-width:10px;", formatter: formatCheckBoxAluno },
                { name: "Aluno", field: "no_pessoa", width: "80%", styles: "min-width:80px;" }
            ],
            selectionMode: "single",
            noDataMessage: msgNotRegEnc,
            plugins: {
                pagination: {
                    pageSizes: ["10", "20", "50", "100", "All"],
                    description: true,
                    sizeSwitch: true,
                    pageStepper: true,
                    defaultPageSize: "10",
                    gotoButton: true,
                    maxPageStep: 5,
                    position: "button"
                }
            }
        }, "gridAluno");
        gridAluno.canSort = function (col) { return Math.abs(col) != 1 };
        gridAluno.startup();
    }
}

function formatCheckBoxTurma(value, rowIndex, obj) {
    var gridName = 'gridTurma';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosTurma');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);

        value = value || indice != null;
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px;'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionadoTurma', -1, 'selecionaTodosTurma', 'selecionaTodosTurma', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'selecionadoTurma', " + rowIndex + ", '" + id + "', 'selecionaTodosTurma', '" + gridName + "')", 2);

    return icon;
}

function formatCheckBoxCurso(value, rowIndex, obj) {
    var gridName = 'gridCurso';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosCurso');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_curso", grid._by_idx[rowIndex].item.cd_curso);

        value = value || indice != null;
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px;'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_curso', 'selecionadoCurso', -1, 'selecionaTodosCurso', 'selecionaTodosCurso', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_curso', 'selecionadoCurso', " + rowIndex + ", '" + id + "', 'selecionaTodosCurso', '" + gridName + "')", 2);

    return icon;
}

function formatCheckBoxAluno(value, rowIndex, obj) {
    var gridName = 'gridAluno';
    var grid = dijit.byId(gridName);
    var icon;
    var id = obj.field + '_Selected_' + rowIndex;
    var todos = dojo.byId('selecionaTodosAluno');

    if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
        var indice = binaryObjSearch(grid.itensSelecionados, "cd_aluno", grid._by_idx[rowIndex].item.cd_aluno);

        value = value || indice != null;
    }
    if (rowIndex != -1)
        icon = "<input style='height:16px;'  id='" + id + "'/> ";

    // Configura o check de todos:
    if (hasValue(todos) && todos.type == 'text')
        setTimeout("configuraCheckBox(false, 'cd_aluno', 'selecionadoAluno', -1, 'selecionaTodosAluno', 'selecionaTodosAluno', '" + gridName + "')", grid.rowsPerPage * 3);

    setTimeout("configuraCheckBox(" + value + ", 'cd_aluno', 'selecionadoAluno', " + rowIndex + ", '" + id + "', 'selecionaTodosAluno', '" + gridName + "')", 2);

    return icon;
}

function atualizaTgAlunos() {
    if (hasValue(dijit.byId('gridAluno')))
        dijit.byId('gridAluno').update();
}

function atualizaTgTurmas() {
    if (hasValue(dijit.byId('gridTurma')))
        dijit.byId('gridTurma').update();
}

function atualizaTgCursos() {
    if (hasValue(dijit.byId('gridCurso')))
        dijit.byId('gridCurso').update();
}

function limparReajusteAnual() {
    try {
        clearForm("formReajusteAnual");
        getLimpar("#formReajusteAnual");
        apresentaMensagem("apresentadorMensagemReajusteAnual", null);
        dojo.byId("cd_reajuste_anual").value = 0;

        dijit.byId('tgAlunos').set('open', false)
        dijit.byId('tgTurmas').set('open', false)
        dijit.byId('tgCursos').set('open', false)

        //Grades
        var gridTurma = dijit.byId("gridTurma");
        if (hasValue(gridTurma)) {
            gridTurma.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridTurma.itensSelecionados = [];
        }
        var gridCurso = dijit.byId("gridCurso");
        if (hasValue(gridCurso)) {
            gridCurso.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridCurso.itensSelecionados = [];
        }
        var gridAluno = dijit.byId("gridAluno");
        if (hasValue(gridAluno)) {
            gridAluno.setStore(dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
            gridAluno.itensSelecionados = [];
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function loadUsuarios(fieldName) {
    dojo.xhr.get({
        url: Endereco() + "/api/usuario/getUsuarios",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagem", null);
            var cdusuario = null;
            if (data != null && data.retorno != null) {
                populaUsuarios(fieldName, data.retorno);
            }
        } catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
    });
}

function populaUsuarios(fieldName, data) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var items = [];

		        items.push({ id: 0, name: "Todos" });
		        Array.forEach(data, function (value, i) {
		            items.push({ id: value.cd_usuario, name: value.no_login });
		        });
		        var stateStore = new Memory({
		            data: items
		        });
		        dijit.byId(fieldName).store = stateStore;
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}

function criarAcaoRelacionadaPesquisa(gridReajusteAnual) {
    //*** Cria os botões do link de ações  e Todos os Itens**\\
    // Adiciona link de Todos os Itens:
    try {
        menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var menuTodosItens = new dijit.MenuItem({
            label: "Todos Itens",
            onClick: function () { buscarTodosItens(gridReajusteAnual, 'todosItensReajusteAnual', ['pesquisarReajusteAnual', 'relatorio']); pesquisarReajusteAnual(false); }
        });
        menu.addChild(menuTodosItens);

        var menuItensSelecionados = new dijit.MenuItem({
            label: "Itens Selecionados",
            onClick: function () { buscarItensSelecionados('gridReajusteAnual', 'selecionadoReajusteAnual', 'cd_reajuste_anual', 'selecionaTodosReajusteAnual', ['pesquisarReajusteAnual', 'relatorio'], 'todosItensReajusteAnual'); }
        });
        menu.addChild(menuItensSelecionados);

        button = new dijit.form.DropDownButton({
            label: "Todos Itens",
            name: "todosItensReajusteAnual",
            dropDown: menu,
            id: "todosItensReajusteAnual"
        });
        dojo.byId("linkSelecionados").appendChild(button.domNode);
        //Fim

        // Adiciona link de ações para ReajusteAnual:
        var menu = new dijit.DropDownMenu({ style: "height: 25px" });
        var acaoExcluir = new dijit.MenuItem({
            label: "Excluir",
            onClick: function () { eventoRemoverReajustesAnuais(dijit.byId("gridReajusteAnual").itensSelecionados); }
        });
        menu.addChild(acaoExcluir);

        var acaoEditar = new dijit.MenuItem({
            label: "Editar",
            onClick: function () { eventoEditarReajusteAnual(dijit.byId("gridReajusteAnual").itensSelecionados); }
        });
        menu.addChild(acaoEditar);

        var acaoEditar = new dijit.MenuItem({
            label: "Abrir/Fechar",
            onClick: function () {
                if (!hasValue(dijit.byId("gridReajusteAnual").itensSelecionados) || dijit.byId("gridReajusteAnual").itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
                else if (dijit.byId("gridReajusteAnual").itensSelecionados.length > 1)
                    caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
                else {
                    var mensagem = "Deseja processar esse registro?";
                    if (dijit.byId("gridReajusteAnual").itensSelecionados[0].id_status_reajuste == 2)
                        mensagem = "Deseja desprocessar esse registro?";
                    caixaDialogo(DIALOGO_CONFIRMAR, mensagem, function executaRetorno() {
                        eventoAbrirFecharReajusteAnual(dijit.byId("gridReajusteAnual").itensSelecionados);
                    });
                }
            }
        });
        menu.addChild(acaoEditar);

        button = new dijit.form.DropDownButton({
            label: "Ações Relacionadas",
            name: "acoesRelacionadasReajusteAnual",
            dropDown: menu,
            id: "acoesRelacionadasReajusteAnual"
        });
        dojo.byId("linkAcoes").appendChild(button.domNode);
    } catch (e) {
        postGerarLog(e);
    }
}

//Configurar Fks
function abrirPessoaFK() {
    try {
        limparPesquisaPessoaFK();
        //dijit.byId("tipoPessoaFK").set("value", 1);
        //dijit.byId("tipoPessoaFK").set("disabled", true);
        pesquisaPessoaFKTitulo();
        apresentaMensagem('apresentadorMensagemProPessoa', null);
        dijit.byId("proPessoa").show();
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisaPessoaFKTitulo() {
    require([
        "dojo/store/JsonRest",
        "dojo/data/ObjectStore",
        "dojo/store/Cache",
        "dojo/store/Memory",
        "dojo/ready"],
    function (JsonRest, ObjectStore, Cache, Memory, ready) {
        ready(function () {
            try {
                var myStore = Cache(
                     JsonRest({
                         target: Endereco() + "/api/aluno/getPessoaTituloSearch?nome=" + dojo.byId("_nomePessoaFK").value +
                                       "&apelido=" + dojo.byId("_apelido").value + "&inicio=" + document.getElementById("inicioPessoaFK").checked + "&tipoPessoa=" + dijit.byId("tipoPessoaFK").value +
                                       "&cnpjCpf=" + dojo.byId("CnpjCpf").value +
                                       "&sexo=" + dijit.byId("sexoPessoaFK").value + "&responsavel=false",
                         handleAs: "json",
                         headers: { "Accept": "application/json", "Authorization": Token() }
                     }), Memory({}));

                dataStore = new ObjectStore({ objectStore: myStore });
                var grid = dijit.byId("gridPesquisaPessoa");
                grid.setStore(dataStore);
            } catch (e) {
                postGerarLog(e);
            }
        })
    });
}

function retornarCursoFK() {
    require(["dojo/ready", "dojo/store/Memory", "dojo/data/ObjectStore"],
        function (ready, Memory, ObjectStore) {
            try {
                var gridCurso = dijit.byId("gridCurso");
                var gridCursosFK = dijit.byId("gridPesquisaCurso");
                if (!hasValue(gridCursosFK.itensSelecionados) || gridCursosFK.itensSelecionados.length <= 0)
                    caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                else {
                    var storeGridCurso = (hasValue(gridCurso) && hasValue(gridCurso.store.objectStore.data)) ? gridCurso.store.objectStore.data : [];
                    if (storeGridCurso != null && storeGridCurso.length > 0) {
                        $.each(gridCursosFK.itensSelecionados, function (idx, value) {
                            insertObjSort(gridCurso.store.objectStore.data, "cd_curso", { cod_tipo_avaliacao_curso: 0, cd_tipo_avaliacao: 0, cd_curso: value.cd_curso, no_produto: value.no_produto, no_curso: value.no_curso });
                        });
                        gridCurso.setStore(new ObjectStore({ objectStore: new Memory({ data: gridCurso.store.objectStore.data }) }));
                    } else {
                        var dados = [];
                        $.each(gridCursosFK.itensSelecionados, function (index, val) {
                            insertObjSort(dados, "cd_curso", { cod_tipo_avaliacao_curso: 0, cd_tipo_avaliacao: 0, cd_curso: val.cd_curso, no_produto: val.no_produto, no_curso: val.no_curso });
                        });
                        gridCurso.setStore(new ObjectStore({ objectStore: new Memory({ data: dados }) }));
                    }
                }
                dojo.byId("ehSelectGrade").value = false;
                dijit.byId("proCurso").hide();
            } catch (e) {
                postGerarLog(e);
            }
        });
}

function getAllLocais(data) {
    try {
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_local_movto: data[i].value });
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function getTitulosGrade(data) {
    try {
        var retorno = new Array();
        for (var i = 0; i < data.length; i++)
            retorno.push({ cd_titulo: data[i].cd_titulo });
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

//Aluno FK
function retornarAlunoFK(selecionaVariosRegistros) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
		        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
		            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
		            return false;
		        }
		        else if ((!hasValue(selecionaVariosRegistros) || !selecionaVariosRegistros) && gridPesquisaAluno.itensSelecionados.length > 1) {
		            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
		            return false;
		        }
		        else {
		            var novoAluno = {};
		            var dados = dijit.byId("gridAluno").store.objectStore.data;
		            quickSortObj(dados, 'cd_aluno');

		            Array.forEach(gridPesquisaAluno.itensSelecionados, function (value, i) {
		                novoAluno = { cd_aluno: value.cd_aluno, no_pessoa: value.no_pessoa };
		                insertObjSort(dados, "cd_aluno", novoAluno, false);
		            });

		            dijit.byId("gridAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));
		            var mensagensWeb = [];
		            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegIncludSucess);
		            apresentaMensagem('apresentadorMensagemReajusteAnual', mensagensWeb);
		            dijit.byId("proAluno").hide();
		        }
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}

function retornarTurmaFK(selecionaVariosRegistros) {
    require(["dojo/store/Memory", "dojo/_base/array"],
		function (Memory, Array) {
		    try {
		        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
		        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0) {
		            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
		            return false;
		        }
		        else if ((!hasValue(selecionaVariosRegistros) || !selecionaVariosRegistros) && gridPesquisaTurmaFK.itensSelecionados.length > 1) {
		            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
		            return false;
		        }
		        else {
		            var novaTurma = {};
		            var dados = dijit.byId("gridTurma").store.objectStore.data;
		            quickSortObj(dados, 'cd_turma');

		            Array.forEach(gridPesquisaTurmaFK.itensSelecionados, function (value, i) {
		                novaTurma = { cd_turma: value.cd_turma, no_turma: value.no_turma, no_apelido: value.no_apelido };
		                insertObjSort(dados, "cd_turma", novaTurma, false);
		            });
		            
		            dijit.byId("gridTurma").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: dados }) }));
		            var mensagensWeb = [];
		            mensagensWeb[0] = new MensagensWeb(MENSAGEM_INFORMACAO, msgRegIncludSucess);
		            apresentaMensagem('apresentadorMensagemReajusteAnual', mensagensWeb);
		            dijit.byId("proTurmaFK").hide();
		        }
		    } catch (e) {
		        postGerarLog(e);
		    }
		})
}

//Fim
function formatCheckBoxReajusteAnual(value, rowIndex, obj) {
    try {
        var gridName = 'gridReajusteAnual';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodosReajusteAnual');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_reajuste_anual", grid._by_idx[rowIndex].item.cd_reajuste_anual);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_reajuste_anual', 'selecionadoReajusteAnual', -1, 'selecionaTodosReajusteAnual', 'selecionaTodosReajusteAnual', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_reajuste_anual', 'selecionadoReajusteAnual', " + rowIndex + ", '" + id + "', 'selecionaTodosReajusteAnual', '" + gridName + "')", 2);

        return icon;
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoEditarReajusteAnual(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridReajusteAnual = dijit.byId('gridReajusteAnual');

            limparReajusteAnual(false);
            apresentaMensagem('apresentadorMensagem', '');
            gridReajusteAnual.itemSelecionado = itensSelecionados[0];
            keepValuesReajusteAnual(gridReajusteAnual);
            IncluirAlterar(0, 'divAlterarReajusteAnual', 'divIncluirReajusteAnual', 'divExcluirReajusteAnual', 'apresentadorMensagemReajusteAnual', 'divCancelarReajusteAnual', 'divClearReajusteAnual');
            dijit.byId("cadReajusteAnual").show();
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function keepValuesReajusteAnual(gridReajusteAnual) {
    try {
        var value = gridReajusteAnual.itemSelecionado;

        dojo.byId('cd_reajuste_anual').value = value.cd_reajuste_anual;
        showCarregando();
        limparReajusteAnual();

        require(["dojo/_base/xhr", "dojo/dom", "dijit/registry", "dojox/grid/EnhancedGrid",
                   "dojo/data/ObjectStore", "dojo/store/Cache", "dojo/store/Memory", "dojo/query", "dojo/dom-attr",
                   "dijit/Dialog", "dojo/domReady!", "dojox/grid/enhanced/plugins/Pagination"
        ], function (xhr, dom, registry, EnhancedGrid, ObjectStore, Cache, Memory, query, domAttr, Pagination) {
            xhr.get({
                preventCache: true,
                url: Endereco() + "/api/secretaria/getReajusteAnualForEdit?cd_reajuste_anual=" + value.cd_reajuste_anual,
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                try {
                    value = jQuery.parseJSON(data).retorno;

                    dojo.byId('cd_reajuste_anual').value = value.cd_reajuste_anual;
                    var dadosTipo = [
								{ name: "Percentual", id: "1" },
								{ name: "Valor", id: "2" }
                    ];
                    criarOuCarregarCompFiltering("cbTipoCad", dadosTipo, "", value.id_tipo_reajuste, dojo.ready, dojo.store.Memory,
                                                          dijit.form.FilteringSelect, 'id', 'name');
                    if (hasValue(value.nomesContrato) && value.nomesContrato.length > 0)
                        criarOuCarregarCompFiltering("cd_nome_contrato", value.nomesContrato, "", value.cd_nome_contrato, dojo.ready, dojo.store.Memory,
                                                          dijit.form.FilteringSelect, 'cd_nome_contrato', 'no_contrato');
                    if (value.id_tipo_reajuste == 1) {
                        dijit.byId('pc_reajuste_anual').set('value', value.pc_reajuste_anual);
                        dijit.byId('pc_reajuste_anual').set('disabled', false);
                        dijit.byId('vl_reajuste_anual').set('disabled', true);
                    } else {
                        dijit.byId('vl_reajuste_anual').set('value', value.vl_reajuste_anual);
                        dijit.byId('pc_reajuste_anual').set('disabled', true);
                        dijit.byId('vl_reajuste_anual').set('disabled', false);
                    }
                    dojo.byId('dtaCadastro').value = value.dta_cadastro;
                    dojo.byId('horaCadastro').value = value.hh_cadastro;
                    dojo.byId('no_usuario').value = value.SysUsuario.no_login;

                    var dados = [
                    { name: "Todos", id: "0" },
                    { name: "Aberto", id: "1" },
                    { name: "Fechado", id: "2" }
                    ];
                    criarOuCarregarCompFiltering("id_status_reajuste", dados, "", value.id_status_reajuste, dojo.ready, dojo.store.Memory,
                                                          dijit.form.FilteringSelect, 'id', 'name');

                    dojo.byId('dt_inicial_vencimento').value = value.dt_inicial_vcto;
                    dojo.byId('dt_final_vencimento').value = value.dt_final_vcto;

                    criaTabelas(EnhancedGrid, ObjectStore, Memory, Pagination);

                    if (hasValue(value.ReajustesAlunos) && value.ReajustesAlunos.length > 0)
                        dijit.byId('gridAluno').setStore(new ObjectStore({ objectStore: new Memory({ data: value.ReajustesAlunos }) }));
                    if (hasValue(value.ReajustesTurmas) && value.ReajustesTurmas.length > 0)
                        dijit.byId('gridTurma').setStore(new ObjectStore({ objectStore: new Memory({ data: value.ReajustesTurmas }) }));
                    if (hasValue(value.ReajustesCursos) && value.ReajustesCursos.length > 0)
                        dijit.byId('gridCurso').setStore(new ObjectStore({ objectStore: new Memory({ data: value.ReajustesCursos }) }));
                    showCarregando();
                } catch (e) {
                    postGerarLog(e);
                }
            }, function (error) {
                apresentaMensagem('apresentadorMensagemReajusteAnual', error);
                showCarregando();
            });
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function pesquisarReajusteAnual(limparItens) {
    try {
        var dtaInicial = dijit.byId("dtaInicial");
        var dtaFinal = dijit.byId("dtaFinal");

        if (!dijit.byId("id_cadastro").checked && !dijit.byId('id_vcto_inicial').checked) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroTipoPeriodoNaoInformado);
            apresentaMensagem('apresentadorMensagem', mensagensWeb);
        }

        //if (hasValue(dojo.byId("dtaInicial").value) && hasValue(dojo.byId("dtaFinal").value) && dojo.date.compare(dtaInicial.get("value"), dtaFinal.value) > 0) {
        //    var mensagensWeb = new Array();
        //    var mensagemErro = "";
        //    mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, mensagemErro);
        //    apresentaMensagem('apresentadorMensagem', mensagensWeb);
        //    dtaFinal._onChangeActive = false;
        //    dtaFinal.reset();
        //    dtaFinal._onChangeActive = true;
        //    return false;
        //}
        var grid = dijit.byId("gridReajusteAnual");
        var cd_usuario = hasValue(dijit.byId("cbUsuario").value) ? dijit.byId("cbUsuario").value : 0;
        var id_status = hasValue(dijit.byId("cbStatus").value) ? dijit.byId("cbStatus").value : 0;
        var cd_reajuste_anual = hasValue(dijit.byId("cdReajusteAnual").value) ? dijit.byId("cdReajusteAnual").value : 0;
        var myStore =
            dojo.store.Cache(
                    dojo.store.JsonRest({
                        target: Endereco() + "/api/financeiro/getReajusteAnualSearch?cd_usuario=" + parseInt(cd_usuario) + "&status=" + parseInt(id_status) + "&dtInicial=" + dojo.byId("dtaInicial").value +
                                   "&dtFinal=" + dojo.byId('dtaFinal').value + "&cadastro=" + dijit.byId("id_cadastro").checked + "&vctoInicial=" + dijit.byId("id_vcto_inicial").checked + "&cd_reajuste_anual=" + cd_reajuste_anual,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), dojo.store.Memory({}));

        var dataStore = new dojo.data.ObjectStore({ objectStore: myStore });

        if (limparItens)
            grid.itensSelecionados = [];
        grid.setStore(dataStore);
    } catch (e) {
        postGerarLog(e);
    }
}

function validarReajusteAnual() {
    try {
        var validado = true;
        if (!dijit.byId("formReajusteAnual").validate()) {
            validado = false;
        }
        if (dojo.date.compare(dijit.byId('dt_inicial_vencimento').value, dijit.byId('dt_final_vencimento').value) > 0) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroDataInicialFinal);
            apresentaMensagem('apresentadorMensagemReajusteAnual', mensagensWeb);
            return false;
        }
        return validado;
    } catch (e) {
        postGerarLog(e);
    }
}

function incluirReajusteAnual() {
    try {
        if (!validarReajusteAnual())
            return false;

        showCarregando();
        var reajusteAnual = mountDataReajustesAnuaisForPost();
        dojo.xhr.post({
            url: Endereco() + "/api/financeiro/postInsertReajusteAnual",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(reajusteAnual)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridReajusteAnual';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    insertObjSort(grid.itensSelecionados, "cd_reajuste_anual", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoReajusteAnual', 'cd_reajuste_anual', 'selecionaTodosReajusteAnual', ['pesquisarReajusteAnual', 'relatorio'], 'todosItensReajusteAnual');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_reajuste_anual");

                    loadUsuarios('cbUsuario');
                    showCarregando();
                    dijit.byId("cadReajusteAnual").hide();
                } else {
                    apresentaMensagem('apresentadorMensagemReajusteAnual', data);
                    showCarregando();
                }
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemReajusteAnual', error);
            showCarregando();
        });
    } catch (e) {
        postGerarLog(e);
    }
}

function alterarReajusteAnual(retornoTemporario) {
    try {
        var reajuste = null;
        if (!validarReajusteAnual())
            return false;
        showCarregando();
        reajuste = mountDataReajustesAnuaisForPost(retornoTemporario);
        dojo.xhr.post({
            url: Endereco() + "/api/financeiro/postUpdateReajusteAnual",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(reajuste)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridReajusteAnual';
                    var grid = dijit.byId(gridName);
                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_reajuste_anual", dojo.byId("cd_reajuste_anual").value);
                    insertObjSort(grid.itensSelecionados, "cd_reajuste_anual", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoReajusteAnual', 'cd_reajuste_anual', 'selecionaTodosReajusteAnual', ['pesquisarReajusteAnual', 'relatorio'], 'todosItensReajusteAnual');
                    grid.sortInfo = 3; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_reajuste_anual");
                    dijit.byId("cadReajusteAnual").hide();
                }
                else
                    apresentaMensagem('apresentadorMensagemReajusteAnual', data);
                showCarregando();
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemReajusteAnual', error);
            showCarregando();
        });
    } catch (e) {
        postGerarLog(e);
    }
};

function mountDataReajustesAnuaisForPost() {
    try {
        var cd_reajuste_anual = parseInt(dojo.byId('cd_reajuste_anual').value);
        var reajustesAlunos = dijit.byId('gridAluno').store.objectStore.data;
        var reajustesCursos = dijit.byId('gridCurso').store.objectStore.data;
        var reajustesTurmas = dijit.byId('gridTurma').store.objectStore.data;

        var retorno = {
            cd_reajuste_anual: cd_reajuste_anual,
            id_tipo_reajuste: dijit.byId('cbTipoCad').value,
            pc_reajuste_anual: unmaskFixed(dojo.byId('pc_reajuste_anual').value, 4),
            vl_reajuste_anual: unmaskFixed(dojo.byId('vl_reajuste_anual').value, 4),
            dt_inicial_vencimento: hasValue(dojo.byId("dt_inicial_vencimento").value) ? dojo.date.locale.parse(dojo.byId("dt_inicial_vencimento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            dt_final_vencimento: hasValue(dojo.byId("dt_final_vencimento").value) ? dojo.date.locale.parse(dojo.byId("dt_final_vencimento").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            id_status_reajuste: dijit.byId('id_status_reajuste').value,
            cd_usuario: dijit.byId('cbUsuario').value,
            //dh_cadastro_reajuste: dojo.byId('dtaCadastro').value + horaCadastro,
            dh_cadastro_reajuste: hasValue(dojo.byId("dtaCadastro").value) ? dojo.date.locale.parse(dojo.byId("dtaCadastro").value, { formatLength: 'short', selector: 'date', locale: 'pt-br' }) : null,
            hr_cadastro: dojo.byId("horaCadastro").value,
            cd_nome_contrato : hasValue(dijit.byId('cd_nome_contrato').value) ? dijit.byId('cd_nome_contrato').value : null,
            ReajustesAlunos: reajustesAlunos,
            ReajustesCursos: reajustesCursos,
            ReajustesTurmas: reajustesTurmas
        }
        return retorno;
    } catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverReajustesAnuais(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else {
            /*if (itensSelecionados.length > 1) 
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            else*/
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () {
                deletarReajustesAnuais(itensSelecionados);
            });
        }
    } catch (e) {
        postGerarLog(e);
    }
}

function deletarReajustesAnuais(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId("cd_reajuste_anual").value != 0)
                itensSelecionados = [{
                    cd_reajuste_anual: dojo.byId("cd_reajuste_anual").value
                }];
        dojo.xhr.post({
            url: Endereco() + "/api/financeiro/postDeleteReajusteAnual",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: dojox.json.ref.toJson(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                // Remove o item dos itens selecionados:
                for (var r = itensSelecionados.length - 1; r >= 0; r--)
                    removeObjSort(dijit.byId('gridReajusteAnual').itensSelecionados, "cd_reajuste_anual", itensSelecionados[r].cd_reajuste_anual);
                pesquisarReajusteAnual(false);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarReajusteAnual").set('disabled', false);
                dijit.byId("relatorio").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("cadReajusteAnual").hide();
            } catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadReajusteAnual").style.display))
                apresentaMensagem('apresentadorMensagemReajusteAnual', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    } catch (e) {
        apresentaMensagem('apresentadorMensagem', error);
        postGerarLog(e);
    }
}

function eventoAbrirFecharReajusteAnual(itensSelecionados) {
    try {
        showCarregando();
        dojo.xhr.post({
            url: Endereco() + "/api/escola/postAbrirFecharReajusteAnual",
            handleAs: "json",
            preventCache: true,
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({ cd_reajuste_anual: itensSelecionados[0].cd_reajuste_anual })
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridReajusteAnual';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_reajuste_anual", itemAlterado.cd_reajuste_anual);
                    insertObjSort(grid.itensSelecionados, "cd_reajuste_anual", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionadoReajusteAnual', 'cd_reajuste_anual', 'selecionaTodosReajusteAnual', ['pesquisarReajusteAnual', 'relatorio'], 'todosItensReajusteAnual');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_reajuste_anual");
                    showCarregando();
                    dijit.byId("cadReajusteAnual").hide();
                } else {
                    apresentaMensagem('apresentadorMensagem', data);
                    showCarregando();
                }
            } catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
            showCarregando();
        });
    } catch (e) {
        postGerarLog(e);
    }
}
