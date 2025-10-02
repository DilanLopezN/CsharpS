var RAF_SEM_DIARIO = 40; RAF_SEM_DIARIO_COM_ESCOLA = 41; cdProf = 0
var professor = null;
function montarConsultarRafsemDiario() {
    require([
    "dojo/ready",
    "dojo/_base/xhr",
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
    "dojo/dom"
    ], function (ready, xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, ref, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom) {
        ready(function () {
            try {
                xhr.get({
                    url: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        if (data.retorno != null && data.retorno.Professor != null) {
                            if (data.retorno.Professor.cd_pessoa > 0 && !data.retorno.Professor.id_coordenador) {
                                cdProf = data.retorno.Professor.cd_pessoa;
                                dojo.byId('nome_usuario_professor').value = data.retorno.no_fantasia;
                                professor = data.retorno.Professor;
                                if (eval(Master())) {
                                    cdProf = 0;
                                }
                            }

                        }
                        dojo.byId('cd_usuario_professor').value = cdProf;
                        apresentaMensagem("apresentadorMensagemTurma", null);

                        var myStore = [];

                        var gridRafsemDiario = new EnhancedGrid({
                            store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                            structure: [
                                {
                                    name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "15px", styles: "text-align:center; min-width:15px; max-width:10px;",
                                    formatter: formatCheckBox
                                },
                                { name: "Escola", field: "dc_reduzido_pessoa", width: "27%", styles: "min-width:80px;" },
                                { name: "Professor", field: "no_professor", width: "27%", styles: "min-width:80px;" },
                                { name: "Turma", field: "no_turma", width: "27%" },
                                { name: "Programações", field: "qtd_programacao", width: "8%", styles: "text-align: center;" },
                                { name: "Liberado", field: "liberado", width: "8%", styles: "min-width:80px; text-align: center;" }
                            ],
                            noDataMessage: msgNotRegEncFiltro,
                            canSort: true,
                            selectionMode: "single",
                            plugins: {
                                pagination: {
                                    pageSizes: ["17", "32", "64", "100", "All"],
                                    description: true,
                                    sizeSwitch: true,
                                    pageStepper: true,
                                    defaultPageSize: "17",
                                    gotoButton: true,
                                    /*page step to be displayed*/
                                    maxPageStep: 5,
                                    /*position of the pagination bar*/
                                    position: "button"
                                }
                            }
                        }, "gridRafsemDiario");
                        gridRafsemDiario.startup();
                        gridRafsemDiario.itenSelecionado = null;
                        gridRafsemDiario.pagination.plugin._paginator.plugin.connect(gridRafsemDiario.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            try {
                                verificaMostrarTodos(evt, gridRafsemDiario, 'cd_turma', 'selecionaTodos');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        });

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridRafsemDiario, "_onFetchComplete", function () {
                                try {
                                    // Configura o check de todos:
                                    if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                        setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridRafsemDiario')", gridRafsemDiario.rowsPerPage * 3);
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                        });
                        gridRafsemDiario.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 };

                        new Button({
                            label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                                apresentaMensagem('apresentadorMensagem', null);
                                pesquisarRaf(true);
                                if (cdProf > 0) populaProfessor('cbProfessorSD', cdProf)
                            }
                        }, "pesquisaRAF");
                        decreaseBtn(document.getElementById("pesquisaRAF"), '32px');

                        // Itens selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () {
                                buscarTodosItens(gridRafsemDiario, 'todosItens', ['pesquisaRAF']);
                                pesquisarRaf(false);

                            }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridRafsemDiario', 'selecionado', 'cd_turma', 'selecionaTodos', ['pesquisaRAF'], 'todosItens'); }
                        });
                        menu.addChild(menuItensSelecionados);

                        var button = new DropDownButton({
                            label: "Todos Itens",
                            name: "todosItens",
                            dropDown: menu,
                            id: "todosItens"
                        });
                        dom.byId("linkSelecionados").appendChild(button.domNode);


                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                if (!hasValue(dijit.byId("gridPesquisaTurmaFK"), true))
                                    montarGridPesquisaTurmaFK(function () {
                                        abrirTurmaFK(cdProf);
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
                                    abrirTurmaFK(cdProf);
                            }
                        }, "FKTurma");

                        new Button({
                            label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () { limpaTurmaFK(); }
                        }, "limparTurma");

                        if (hasValue(document.getElementById("limparTurma"))) {
                            document.getElementById("limparTurma").parentNode.style.minWidth = '40px';
                            document.getElementById("limparTurma").parentNode.style.width = '40px';
                        }
                        new Button({
                            label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF', onClick: function () {
                                try {
                                    if (!hasValue(dijit.byId("gridPesquisaAluno"))) {
                                        montarGridPesquisaAluno(false, function () {
                                            abrirAlunoFK();
                                        });
                                    }
                                    else
                                        abrirAlunoFK();
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "FKAluno");
                        new Button({
                            label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                                try {
                                    dojo.byId('cdAluno').value = 0;
                                    dojo.byId("txAluno").value = "";
                                    dijit.byId('limparAluno').set("disabled", true);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparAluno");
                        if (hasValue(document.getElementById("limparAluno"))) {
                            document.getElementById("limparAluno").parentNode.style.minWidth = '40px';
                            document.getElementById("limparAluno").parentNode.style.width = '40px';
                        }

                        var buttonFkArray = ['FKTurma','FKAluno'];

                        for (var p = 0; p < buttonFkArray.length; p++) {
                            var buttonFk = document.getElementById(buttonFkArray[p]);

                            if (hasValue(buttonFk)) {
                                buttonFk.parentNode.style.minWidth = '18px';
                                buttonFk.parentNode.style.width = '18px';
                            }
                        }
                        //adicionarAtalhoPesquisa(['FKaluno', 'FKitem', 'cbHistorico', 'dtInicialFat', 'dtFinalFat'], 'pesquisaAluno', ready);
                        populaProfessor('cbProfessorSD', cdProf);
                        dijit.byId('ckEscolas').set('disabled', !eval(MasterGeral()));
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                },
                    function (error) {
                        apresentaMensagem("apresentadorMensagem", error);

                    }
                )
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridRafsemDiario';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_turma', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarRaf(limparItens) {
    if (!eval(MasterGeral()) && dijit.byId('ckEscolas').checked) {
        caixaDialogo(DIALOGO_AVISO, 'A opção de todas as escolas somente está liberada para usuários Master Geral.', null);
        dijit.byId('ckEscolas').set('checked', false);
        return false;
    }
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridRafsemDiario = dijit.byId("gridRafsemDiario");
            var cdTurma = hasValue(dojo.byId("cdTurma").value) ? dojo.byId("cdTurma").value : 0;
            var cdAluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
            var cdProfessor = hasValue(dijit.byId("cbProfessorSD").value) ? dijit.byId("cbProfessorSD").value : 0;
            var myStore =
                    Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getRafsemDiario?cd_turma=" + cdTurma + "&cd_professor=" + cdProfessor +
                                "&idEscola=" + dijit.byId('ckEscolas').checked + '&cd_aluno=' + cdAluno,
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({})
                    );

            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridRafsemDiario.itensSelecionados = [];
            gridRafsemDiario.itemSelecionado = null;
            gridRafsemDiario.noDataMessage = msgNotRegEnc;
            gridRafsemDiario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function abrirTurmaFK(cdProf) {
    try {
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de aluno na turma
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = dijit.byId('ckEscolas').checked ? SEM_DIARIO : SEM_DIARIO_COM_ESCOLA;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (compProfpesquisa.disabled == true && hasValue(compProfpesquisa.value))
                compProfpesquisa.set('disabled', true);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(cdProf, SEM_DIARIO);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKSemDiario() {
    try {
        var valido = true;
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados))
            gridPesquisaTurmaFK.itensSelecionados = [];
        if (!hasValue(gridPesquisaTurmaFK.itensSelecionados) || gridPesquisaTurmaFK.itensSelecionados.length <= 0 || gridPesquisaTurmaFK.itensSelecionados.length > 1) {
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length <= 0)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            if (gridPesquisaTurmaFK.itensSelecionados != null && gridPesquisaTurmaFK.itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgMaisDeUmRegSelect, null);
            valido = false;
        }
        else {
            dojo.byId("txTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].no_turma;
            dojo.byId("cdTurma").value = gridPesquisaTurmaFK.itensSelecionados[0].cd_turma;
            dijit.byId('limparTurma').set("disabled", false);
        }
        if (!valido)
            return false;
        dijit.byId("proTurmaFK").hide();
    }
    catch (e) {
        postGerarLog(e);
    }
}

function limpaTurmaFK() {
    dojo.byId("txTurma").value = "";
    dojo.byId("cdTurma").value = "";
    dijit.byId('limparTurma').set("disabled", true);
}

function populaProfessor(field, cdProf) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/professor/getAllProfessorTurmasemDiario?idEscola=" + dijit.byId('ckEscolas').checked,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                var profs = jQuery.parseJSON(data).retorno;
                var obj = JSON.parse(data);
                if (professor != null)
                obj.retorno.push(professor);
                jsonStr = JSON.stringify(obj);

                loadSelect(obj.retorno, field, 'cd_pessoa', 'no_fantasia');
                if (cdProf != 0 && cdProf != 'undefined') {
                    dijit.byId(field).set('value', cdProf);
                    dijit.byId(field).set("disabled", true);
                }
            });
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = dijit.byId('ckEscolas').checked ? CARGA_HORARIA : CARGA_HORARIA_COM_ESCOLA;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        apresentaMensagem("apresentadorMensagem", null);

        limparPesquisaAlunoFK();

        pesquisarAlunoFK(true, (dijit.byId('ckEscolas').checked ? CARGA_HORARIA : CARGA_HORARIA_COM_ESCOLA));
        dijit.byId("proAluno").show();
        dijit.byId('gridPesquisaAluno').update();
    }
    catch (e) {
        postGerarLog(e);
    }

}

//Aluno FK
function retornarAlunoFK() {
    try {
        var gridPesquisaAluno = dijit.byId("gridPesquisaAluno");
        if (!hasValue(gridPesquisaAluno.itensSelecionados) || gridPesquisaAluno.itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else if (gridPesquisaAluno.itensSelecionados != null && gridPesquisaAluno.itensSelecionados.length > 1) {
            caixaDialogo(DIALOGO_ERRO, msgMaisDeUmRegSelect, null);
            return false;
        }
        else {
            dojo.byId("cdAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_aluno;
            dojo.byId("txAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;

            dijit.byId('limparAluno').set("disabled", false);
            dijit.byId("proAluno").hide();

        }
    }
    catch (e) {
        postGerarLog(e);
    }
}
