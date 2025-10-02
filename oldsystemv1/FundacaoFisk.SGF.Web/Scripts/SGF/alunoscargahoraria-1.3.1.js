var DESISTENCIA_CARGA = 50; 
function montarAlunosCargaHoraria() {
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
                var myStore =
                        Cache(
                                JsonRest({
                                    target: Endereco() + "/api/aluno/getAlunosCargaHoraria?cd_aluno=" + parseInt(0) + "&cd_turma=" + parseInt(0) +
                                                        "&dtInicial=&dtFinal=",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json",  "Authorization": Token() }
                                }), Memory({})
                        );

                var gridAlunoDesistencia = new EnhancedGrid({
//                    store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
                    store: dataStore = dojo.data.ObjectStore({ objectStore: new Memory({ data: null }) }),
                    structure: [
                        {
                            name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "15px", styles: "text-align:center; min-width:15px; max-width:20px;",
                            formatter: formatCheckBox
                        },
                        { name: "RAF", field: "nm_raf", width: "10%", styles: "min-width:80px;" },
                        { name: "Aluno", field: "no_aluno", width: "35%", styles: "min-width:80px;" },
                        { name: "Turma", field: "no_turma", width: "35%" },
                        { name: "Desistência", field: "dta_desistencia", width: "8%", styles: "min-width:80px; text-align: center;" },
                        { name: "Curso", field: "no_curso", width: "12%", styles: "min-width:80px;" }
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
                }, "gridAlunoDesistencia");
                gridAlunoDesistencia.startup();
                gridAlunoDesistencia.itenSelecionado = null;
                gridAlunoDesistencia.pagination.plugin._paginator.plugin.connect(gridAlunoDesistencia.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                    try{
                        verificaMostrarTodos(evt, gridAlunoDesistencia, 'cd_desistencia', 'selecionaTodos');
                    }catch (e) {
                        postGerarLog(e);
                    }
                });

                require(["dojo/aspect"], function (aspect) {
                    aspect.after(gridAlunoDesistencia, "_onFetchComplete", function () {
                        try {
                            // Configura o check de todos:
                            if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                setTimeout("configuraCheckBox(false, 'cd_desistencia', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridAlunoDesistencia')", gridAlunoDesistencia.rowsPerPage * 3);
                        } catch (e) {
                            postGerarLog(e);
                        }
                    });
                });
                gridAlunoDesistencia.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9  };

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
                    label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                        apresentaMensagem('apresentadorMensagem', null);
                            pesquisarDesistencia(true);
                    }
                }, "pesquisaDesistencia");
                decreaseBtn(document.getElementById("pesquisaDesistencia"), '32px');

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


                // Ações Relacionadas:
                var menu = new DropDownMenu({ style: "height: 25px" });
                var acaoEditar = new MenuItem({
                    label: "Gerar Nota Voucher 10",
                    onClick: function () { eventoProcessar(gridAlunoDesistencia.itensSelecionados, 10); } //, xhr, ready, Memory, FilteringSelect
                });
                menu.addChild(acaoEditar);

                var acaoEditar5 = new MenuItem({
                    label: "Gerar Nota Voucher 5",
                    onClick: function () { eventoProcessar(gridAlunoDesistencia.itensSelecionados, 5); }
                });
                menu.addChild(acaoEditar5);

                var button = new DropDownButton({
                    label: "Ações Relacionadas",
                    name: "acoesRelacionadas",
                    dropDown: menu,
                    id: "acoesRelacionadas"
                });
                dom.byId("linkAcoes").appendChild(button.domNode);

                // Itens selecionados:
                menu = new DropDownMenu({ style: "height: 25px" });
                var menuTodosItens = new MenuItem({
                    label: "Todos Itens",
                    onClick: function () {
                            buscarTodosItens(gridAlunoDesistencia, 'todosItens', ['pesquisaDesistencia']);
                            pesquisarDesistencia(false);

                    }
                });
                menu.addChild(menuTodosItens);

                var menuItensSelecionados = new MenuItem({
                    label: "Itens Selecionados",
                    onClick: function () { buscarItensSelecionados('gridAlunoDesistencia', 'selecionado', 'cd_desistencia', 'selecionaTodos', ['pesquisaDesistencia'], 'todosItens'); }
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
                                abrirTurmaFK(0);
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
                            abrirTurmaFK(0);
                    }
                }, "FKTurma");

                new Button({
                    label: "Limpar", iconClass: '', type: "reset", disabled: true, onClick: function () {
                        try {
                            dojo.byId('cdTurma').value = 0;
                            dojo.byId("txTurma").value = "";
                            dijit.byId('limparTurma').set("disabled", true);
                        }
                        catch (e) {
                            postGerarLog(e);
                        }
                    }
                }, "limparTurma");

                if (hasValue(document.getElementById("limparTurma"))) {
                    document.getElementById("limparTurma").parentNode.style.minWidth = '40px';
                    document.getElementById("limparTurma").parentNode.style.width = '40px';
                }


                var buttonFkArray = ['FKAluno', 'FKTurma'];

                for (var p = 0; p < buttonFkArray.length; p++) {
                    var buttonFk = document.getElementById(buttonFkArray[p]);

                    if (hasValue(buttonFk)) {
                        buttonFk.parentNode.style.minWidth = '18px';
                        buttonFk.parentNode.style.width = '18px';
                    }
                }
                //adicionarAtalhoPesquisa(['FKaluno', 'FKitem', 'cbHistorico', 'dtInicialFat', 'dtFinalFat'], 'pesquisaAluno', ready);
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
}


function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridAlunoDesistencia';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_desistencia", grid._by_idx[rowIndex].item.cd_desistencia);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox' id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_desistencia', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_desistencia', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function pesquisarDesistencia(limparItens) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest, ObjectStore, Cache, Memory) {
        try {
            var gridAlunoDesistencia = dijit.byId("gridAlunoDesistencia");
            var cdAluno = hasValue(dojo.byId("cdAluno").value) ? dojo.byId("cdAluno").value : 0;
            var cdTurma = hasValue(dojo.byId("cdTurma").value) ? dojo.byId("cdTurma").value : 0;
            var myStore =
                    Cache(
                        JsonRest({
                            target: Endereco() + "/api/aluno/getAlunosCargaHoraria?cd_aluno=" + cdAluno + "&cd_turma=" + cdTurma +
                                "&dtInicial=" + dojo.byId("dtInicialFat").value + "&dtFinal=" + dojo.byId("dtFinalFat").value,
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({})
                    );

            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridAlunoDesistencia.itensSelecionados = [];
            gridAlunoDesistencia.itemSelecionado = null;
            gridAlunoDesistencia.noDataMessage = msgNotRegEnc;
            gridAlunoDesistencia.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function eventoProcessar(itensSelecionados,itemVoucher) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else
            if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            else
                if (!eval(MasterGeral()) && !eval(Master()))
                    caixaDialogo(DIALOGO_AVISO, msgErroMasterAlunoCargaHoraria, null);
                else {
                caixaDialogo(DIALOGO_CONFIRMAR, msgConfirmarGeracaoNotaVoucher, function () { gerarNotaVoucher(itensSelecionados, itemVoucher); });
            }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function gerarNotaVoucher(itensSelecionados,itemVoucher) {
    dojo.xhr.post({
        url: Endereco() + "/api/aluno/postGerarNotaVoucher",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: JSON.stringify({
            cd_desistencia: itensSelecionados[0].cd_desistencia,
            itemVoucher: itemVoucher

        })
    }).then(function (data) {
        data = jQuery.parseJSON(data);
        if (hasValue(data)) {
            caixaDialogo(DIALOGO_AVISO, data, null);
            pesquisarDesistencia(true);
        }

    }, function (error) {
        haErro = jQuery.parseJSON(error.response.data);
        if (hasValue(haErro.erro) || !hasValue(haErro.retorno))
            if (hasValue(haErro.MensagensWeb)) {
                var mensagem = haErro.MensagensWeb[0].mensagem.indexOf('||') > 0 ? haErro.MensagensWeb[0].mensagem.substring(0, haErro.MensagensWeb[0].mensagem.indexOf('||')) : haErro.MensagensWeb[0].mensagem;
                caixaDialogo(DIALOGO_ERRO, mensagem, null);
            }
    })
}

function abrirAlunoFK() {
    try {
        dojo.byId('tipoRetornoAlunoFK').value = DESISTENCIA_CARGA;
        dijit.byId("gridPesquisaAluno").setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
        apresentaMensagem("apresentadorMensagem", null);

        limparPesquisaAlunoFK();

        pesquisarAlunoFK(true, DESISTENCIA_CARGA);
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
            dojo.byId("cdPessoaAluno").value = gridPesquisaAluno.itensSelecionados[0].cd_pessoa_aluno;
            dojo.byId("txAluno").value = gridPesquisaAluno.itensSelecionados[0].no_pessoa;

            dijit.byId('limparAluno').set("disabled", false);
            dijit.byId("proAluno").hide();

        }
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
        dojo.byId("idOrigemCadastro").value = DESISTENCIA_CARGA;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            dijit.byId('tipoTurmaFK').set('value', 1);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        pesquisarTurmaFK(cdProf, DESISTENCIA_CARGA);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function retornarTurmaFKDesistenciaCarga() {
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
