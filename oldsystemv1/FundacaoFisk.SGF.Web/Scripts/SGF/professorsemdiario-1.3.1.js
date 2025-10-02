var SEM_DIARIO = 40; SEM_DIARIO_COM_ESCOLA = 41; cdProf = 0
var professor = null;
function montarProfessorsemDiario() {
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
                                dojo.byId('nome_usuario_professor').value = data.retorno.no_fantasia;  //Campo do TurmaFK
                                professor = data.retorno.Professor;
                                if (eval(Master())) {
                                    cdProf = 0;
                                }
                                if(cdProf > 0)
                                    dijit.byId('ckLiberado').set('checked', true)
                            }

                        }
                        dojo.byId('cd_usuario_professor').value = cdProf;  //Campo do TurmaFK
                        apresentaMensagem("apresentadorMensagemTurma", null);

                        var myStore = [];
                        //    Cache(
                        //        JsonRest({
                        //            target: Endereco() + "/api/professor/getProfessorsemDiario?cd_turma=0" + "&cd_professor=" + parseInt(cdProf) + "&idEscola=false&idLiberado=" + dijit.byId('ckLiberado').checked,
                        //            handleAs: "json",
                        //            preventCache: true,
                        //            headers: { "Accept": "application/json", "Authorization": Token() }
                        //        }), Memory({})
                        //    );

                        var gridProfsemDiario = new EnhancedGrid({
//                            store: dataStore = dojo.data.ObjectStore({ objectStore: myStore }),
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
                        }, "gridProfsemDiario");
                        gridProfsemDiario.startup();
                        gridProfsemDiario.itenSelecionado = null;
                        gridProfsemDiario.pagination.plugin._paginator.plugin.connect(gridProfsemDiario.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            try {
                                verificaMostrarTodos(evt, gridProfsemDiario, 'cd_turma', 'selecionaTodos');
                            } catch (e) {
                                postGerarLog(e);
                            }
                        });

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridProfsemDiario, "_onFetchComplete", function () {
                                try {
                                    // Configura o check de todos:
                                    if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                        setTimeout("configuraCheckBox(false, 'cd_turma', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridProfsemDiario')", gridProfsemDiario.rowsPerPage * 3);
                                } catch (e) {
                                    postGerarLog(e);
                                }
                            });
                        });
                        gridProfsemDiario.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 8 && Math.abs(col) != 9 };

                        new Button({
                            label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                                apresentaMensagem('apresentadorMensagem', null);
                                pesquisarProfessor(true);
                                if (cdProf > 0) populaProfessor('cbProfessorSD', cdProf)
                            }
                        }, "pesquisaProfessor");
                        decreaseBtn(document.getElementById("pesquisaProfessor"), '32px');


                        // Ações Relacionadas:
                        var menu = new DropDownMenu({ style: "height: 25px" });
                        var acaoLiberar = new MenuItem({
                            label: "Liberar",
                            onClick: function () { eventoLiberar(gridProfsemDiario.itensSelecionados); }
                        });
                        menu.addChild(acaoLiberar);

                        var acaoLancar = new MenuItem({
                            label: "Lançar Diário",
                            onClick: function () { eventoLancar(gridProfsemDiario.itensSelecionados, xhr, ready, Memory, FilteringSelect, on); }
                        });
                        menu.addChild(acaoLancar);

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
                                buscarTodosItens(gridProfsemDiario, 'todosItens', ['pesquisaProfessor']);
                                pesquisarProfessor(false);

                            }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridProfsemDiario', 'selecionado', 'cd_turma', 'selecionaTodos', ['pesquisaProfessor'], 'todosItens'); }
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

                        var buttonFkArray = ['FKTurma'];

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
        var gridName = 'gridProfsemDiario';
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

function pesquisarProfessor(limparItens) {
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
            var gridProfsemDiario = dijit.byId("gridProfsemDiario");
            var cdTurma = hasValue(dojo.byId("cdTurma").value) ? dojo.byId("cdTurma").value : 0;
            var cdProfessor = hasValue(dijit.byId("cbProfessorSD").value) ? dijit.byId("cbProfessorSD").value : 0;
            var id_liberado = dijit.byId('ckLiberado').checked;
            var myStore =
                    Cache(
                        JsonRest({
                            target: Endereco() + "/api/professor/getProfessorsemDiario?cd_turma=" + cdTurma + "&cd_professor=" + cdProfessor +
                                "&idEscola=" + dijit.byId('ckEscolas').checked + '&idLiberado=' + id_liberado,
                                handleAs: "json",
                                preventCache: true,
                                headers: { "Accept": "application/json", "Authorization": Token() }
                            }), Memory({})
                    );

            var dataStore = new ObjectStore({ objectStore: myStore });


            if (limparItens)
                gridProfsemDiario.itensSelecionados = [];
            gridProfsemDiario.itemSelecionado = null;
            gridProfsemDiario.noDataMessage = msgNotRegEnc;
            gridProfsemDiario.setStore(dataStore);
        }
        catch (e) {
            postGerarLog(e);
        }
    });
}


function eventoLiberar(itensSelecionados) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
        else
            if (itensSelecionados.length > 1)
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            else
                if (itensSelecionados[0].id_liberado == 1)
                    caixaDialogo(DIALOGO_AVISO, "Professor já liberado", null);
                else
                    if (!eval(Master()))
                        caixaDialogo(DIALOGO_AVISO, "Somente usuários administradores da escola podem liberar o professor", null);
                    else {
                            caixaDialogo(DIALOGO_CONFIRMAR, msgConfirmarLiberarProfessor, function () { liberarProfessor(itensSelecionados); });
                    }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoLancar(itensSelecionados, xhr, ready, Memory, FilteringSelect, on) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0) {
            caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
            return false;
        }
        else
            if (itensSelecionados.length > 1) {
                caixaDialogo(DIALOGO_AVISO, msgNotSelectReg, null);
                return false;
            }
        var turma = {
            cd_turma: itensSelecionados[0].cd_turma,
            no_turma: itensSelecionados[0].no_turma,
            cd_pessoa_escola: itensSelecionados[0].cd_pessoa_empresa
        }
        if (!hasValue(dijit.byId('incluirDiario')))
            montarDiarioPartial(function () {
                abreCadastroDiarioAula(xhr, ready, Memory, FilteringSelect, turma, false);
                setarEventosBotoesPrincipais(xhr, on);
            });
        else {
            abreCadastroDiarioAula(xhr, ready, Memory, FilteringSelect, turma, false);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function liberarProfessor(itensSelecionados) {
    dojo.xhr.post({
        url: Endereco() + "/api/professor/postLiberarProfessor",
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
        postData: JSON.stringify({
            cd_professor: itensSelecionados[0].cd_professor
        })
    }).then(function (data) {
        data = jQuery.parseJSON(data);
        if (hasValue(data)) {
            caixaDialogo(DIALOGO_AVISO, data, null);
            pesquisarProfessor(true);
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
                //obj['retorno'].push({ "cd_professor": 198, "no_pessoa": "Teste", "no_fantasia":"Teste", "id_coordenador":false });
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
        showCarregando();
        diario = mountDataForPostDiadrio();
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
                    pesquisarProfessor(true);
                } else
                    apresentaMensagem('apresentadorMensagemDiarioPartial', data);
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
