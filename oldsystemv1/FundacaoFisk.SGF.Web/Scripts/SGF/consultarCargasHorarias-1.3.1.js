var CARGA_HORARIA = 42; CARGA_HORARIA_COM_ESCOLA = 43; cdProf = 0
var professor = null;

function montarConsultarCargasHorarias() {
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
                //var cdProf = 0;
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
                                dojo.byId('nome_usuario_professor').value = data.retorno.Professor.no_fantasia;  //Campo do TurmaFK
                                professor = data.retorno.Professor;
                                if (eval(Master())) {
                                    cdProf = 0;
                                }
                                if (cdProf > 0 && hasValue(dijit.byId('ckLiberado'), false) && dijit.byId('ckLiberado') != undefined)
                                    dijit.byId('ckLiberado').set('checked', true)
                            }

                        }
                        dojo.byId('cd_usuario_professor').value = cdProf;   //Campo do TurmaFK
                        apresentaMensagem("apresentadorMensagemTurma", null);
                        dijit.byId("nm_aulas_vencimento").set("value", 0);

                        var parametros = getParamterosURL();
                        var redirectParam = parametros['redirect'];
                        var cd_professorParam = parametros['cd_professor'];
                        debugger
                        var myStore = null;
                        var gridCargasHorarias = null;

                        if (redirectParam == "alunosCargaProximaMaxima" &&
                            cd_professorParam != null &&
                            cd_professorParam != undefined &&
                            cd_professorParam != '' &&
                            parseInt(cd_professorParam) > 0) {

                            populaProfessor('cbProfessorSD', parseInt(cd_professorParam));
                            
                            dijit.byId("nm_aulas_vencimento").set("value", 5);

                            myStore =
                                Cache(
                                    JsonRest({
                                        target: Endereco() +
                                            "/api/aluno/GetCargaHoraria?cd_aluno=0&cd_turma=0&cd_curso=0&cd_professor=" + cd_professorParam + "&todasEscolas=false&nm_aulas_vencimento=" + dijit.byId("nm_aulas_vencimento").value,
                                        handleAs: "json",
                                        preventCache: true,
                                        headers: { "Accept": "application/json", "Authorization": Token() }
                                    }),
                                    Memory({})
                                );

                            var gridCargasHorarias = new EnhancedGrid({
                                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                                    structure: [
                                        {
                                            name: "<input id='selecionaTodos' style='display:none'/>",
                                            field: "selecionado",
                                            width: "15px",
                                            styles: "text-align:center; min-width:15px; max-width:10px;",
                                            formatter: formatCheckBox
                                        },
                                        { name: "Escola", field: "no_escola", width: "17%", styles: "min-width:80px;" },
                                        { name: "RAF", field: "nm_raf", width: "10%" },
                                        { name: "Aluno", field: "no_aluno", width: "25%", styles: "min-width:80px;" },
                                        { name: "Turma", field: "no_turma", width: "37%" },
                                        { name: "Curso", field: "no_curso", width: "20%" },
                                        {
                                            name: "Carga",
                                            field: "nm_carga",
                                            width: "8%",
                                            styles: "text-align: center;"
                                        },
                                        {
                                            name: "Carga Máxima",
                                            field: "nm_carga_maxima",
                                            width: "10%",
                                            styles: "min-width:80px; text-align: center;"
                                        },
                                        {
                                            name: "Voucher",
                                            field: "qt_voucher",
                                            width: "10%",
                                            styles: "min-width:80px; text-align: center;"
                                        },
                                        {
                                            name: "Última Aula",
                                            field: "dta_ultima_aula",
                                            width: "17%",
                                            styles: "min-width:80px; text-align: center;"
                                        }
                                ],
                                    noDataMessage: msgNotRegEnc,    //msgNotRegEncFiltro, LBM aqui se não encontrar mostrar mensagem normal
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
                                },
                                "gridCargasHorarias");

                        } else {
                            myStore =
                                Cache(
                                    JsonRest({
                                        target: Endereco() +
                                            "/api/aluno/GetCargaHoraria?cd_aluno=0&cd_turma=0&cd_curso=0&cd_professor=0&todasEscolas=false&nm_aulas_vencimento=" + dijit.byId("nm_aulas_vencimento").value,
                                        handleAs: "json",
                                        preventCache: true,
                                        headers: { "Accept": "application/json", "Authorization": Token() }
                                    }),
                                    Memory({})
                                );

                             gridCargasHorarias = new EnhancedGrid({
                                //store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                                store: dataStore =
                                    dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                                structure: [
                                    {
                                        name: "<input id='selecionaTodos' style='display:none'/>",
                                        field: "selecionado",
                                        width: "15px",
                                        styles: "text-align:center; min-width:15px; max-width:10px;",
                                        formatter: formatCheckBox
                                    },
                                    { name: "Escola", field: "no_escola", width: "17%", styles: "min-width:80px;" },
                                    { name: "RAF", field: "nm_raf", width: "10%" },
                                    { name: "Aluno", field: "no_aluno", width: "25%", styles: "min-width:80px;" },
                                    { name: "Turma", field: "no_turma", width: "37%" },
                                    { name: "Curso", field: "no_curso", width: "20%" },
                                    {
                                        name: "Carga",
                                        field: "nm_carga",
                                        width: "8%",
                                        styles: "text-align: center;"
                                    },
                                    {
                                        name: "Carga Máxima",
                                        field: "nm_carga_maxima",
                                        width: "10%",
                                        styles: "min-width:80px; text-align: center;"
                                    },
                                    {
                                        name: "Voucher",
                                        field: "qt_voucher",
                                        width: "10%",
                                        styles: "min-width:80px; text-align: center;"
                                    },
                                    {
                                        name: "Última Aula",
                                        field: "dta_ultima_aula",
                                        width: "17%",
                                        styles: "min-width:80px; text-align: center;"
                                    }
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
                            },
                                "gridCargasHorarias");
                        }

                         
                        gridCargasHorarias.startup();
                        gridCargasHorarias.itenSelecionado = null;
                        gridCargasHorarias.pagination.plugin._paginator.plugin.connect(gridCargasHorarias.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            try {
                                verificaMostrarTodos(evt, gridCargasHorarias, 'cd_turma', 'selecionaTodos');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        });

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridCargasHorarias, "_onFetchComplete", function () {
                                try {
                                    // Configura o check de todos:
                                    if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                        setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridCargasHorarias')", gridCargasHorarias.rowsPerPage * 3);
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                        });
                        gridCargasHorarias.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 };

                        new Button({
                            label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                                apresentaMensagem('apresentadorMensagem', null);
                                pesquisarCargaHorarias(true);
                                if (cdProf > 0) populaProfessor('cbProfessorSD', cdProf)
                            }
                        }, "pesquisaCargasHorarias");
                        decreaseBtn(document.getElementById("pesquisaCargasHorarias"), '32px');

                        // Itens selecionados:
                        menu = new DropDownMenu({ style: "height: 25px" });
                        var menuTodosItens = new MenuItem({
                            label: "Todos Itens",
                            onClick: function () {
                                buscarTodosItens(gridCargasHorarias, 'todosItens', ['pesquisaCargasHorarias']);
                                pesquisarCargaHorarias(false);

                            }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridCargasHorarias', 'selecionado', 'cd_turma', 'selecionaTodos', ['pesquisaCargasHorarias'], 'todosItens'); }
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

                        var buttonFkArray = ['FKTurma', 'FKAluno'];

                        for (var p = 0; p < buttonFkArray.length; p++) {
                            var buttonFk = document.getElementById(buttonFkArray[p]);

                            if (hasValue(buttonFk)) {
                                buttonFk.parentNode.style.minWidth = '18px';
                                buttonFk.parentNode.style.width = '18px';
                            }
                        }
                        populaCurso('cbCurso', 0);
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
        var gridName = 'gridCargasHorarias';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_turma", grid._by_idx[rowIndex].item.cd_turma);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBoxPlano' id='" + id + "'/> ";

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

function pesquisarCargaHorarias(limparItens) {
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
            var gridCargasHorarias = dijit.byId("gridCargasHorarias");
            var cdTurma = hasValue(dojo.byId("cdTurma").value) ? dojo.byId("cdTurma").value : 0;
            var cdAluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
            var cdCurso = hasValue(dijit.byId("cbCurso").value) ? dijit.byId("cbCurso").value : 0;
            var cdProfessor = hasValue(dijit.byId("cbProfessorSD").value) ? dijit.byId("cbProfessorSD").value : 0;
            var myStore =
                Cache(
                    JsonRest({
                        target: Endereco() + "/api/aluno/GetCargaHoraria?cd_aluno=" + cdAluno + "&cd_turma=" + cdTurma + "&cd_curso=" + cdCurso + "&cd_professor=" + cdProfessor + "&todasEscolas=" + dijit.byId('ckEscolas').checked + "&nm_aulas_vencimento=" + dijit.byId("nm_aulas_vencimento").value,
                        handleAs: "json",
                        preventCache: true,
                        headers: { "Accept": "application/json", "Authorization": Token() }
                    }), Memory({})
                );

            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridCargasHorarias.itensSelecionados = [];
            gridCargasHorarias.itemSelecionado = null;
            gridCargasHorarias.noDataMessage = msgNotRegEnc;
            gridCargasHorarias.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}

function populaProfessor(field, cdProf) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/professor/getProfessorCargaHoraria?idEscola=" + dijit.byId('ckEscolas').checked,
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
            },
            function (error) {
                apresentaMensagem("apresentadorMensagem", error);
            }

            );
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function abrirTurmaFK(cdProf) {
    try {
        dojo.byId("trAluno").style.display = "";
        //Configuração retorno fk de aluno na turma
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = dijit.byId('ckEscolas').checked ? CARGA_HORARIA : CARGA_HORARIA_COM_ESCOLA;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (compProfpesquisa.disabled == true && hasValue(compProfpesquisa.value))
                compProfpesquisa.set('disabled', true);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(cdProf, dijit.byId('ckEscolas').checked ? CARGA_HORARIA : CARGA_HORARIA_COM_ESCOLA);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKCartaHoraria() {
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

function populaCurso(field, cdCurso) {
    try {
        require(["dojo/_base/xhr"], function (xhr) {
            xhr.get({
                url: Endereco() + "/api/curso/getCursosCargaHoraria?todasEscolas=" + dijit.byId('ckEscolas').checked,
                preventCache: true,
                handleAs: "json",
                headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
            }).then(function (data) {
                var obj = JSON.parse(data);
                jsonStr = JSON.stringify(obj);
                loadSelect(obj.retorno, field, 'cd_curso', 'no_curso');
                
            },
            function (error) {
                    apresentaMensagem("apresentadorMensagem", error);
                }
            );
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