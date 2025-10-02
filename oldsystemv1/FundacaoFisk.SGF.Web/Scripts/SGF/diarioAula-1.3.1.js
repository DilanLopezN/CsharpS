
function montarMetodosDiarioAula() {
    //Criação da Grade de sala
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
    "dijit/form/DropDownButton",
    "dijit/DropDownMenu",
    "dijit/MenuItem",
    "dijit/form/FilteringSelect",
      "dojo/dom"
    ], function (ready, xhr, EnhancedGrid, Pagination, JsonRest, ObjectStore, Cache, Memory, on, Button, DropDownButton, DropDownMenu, MenuItem, FilteringSelect, dom) {
        ready(function () {
            try {
                //findAllEmpresasUsuarioComboFiltroEscolaTelaDiario();
                dojo.byId('trEscolaFiltroTelaDiario').style.display = "none";

                dojo.byId("apresentadorMensagemFks").value = "apresentadorMensagemTurmaFK";
                montarDiarioPartial(function () { setarEventosBotoesPrincipais(xhr, on, ready, Memory, FilteringSelect); });
                var cdProf = 0;
                xhr.get({
                    url: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                    preventCache: true,
                    handleAs: "json",
                    headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                }).then(function (data) {
                    try {
                        data = jQuery.parseJSON(data);
                        var no_professor = "";
                        if (data.retorno != null && data.retorno.Professor != null)
                        {
                            if (data.retorno.Professor.cd_pessoa > 0 && !data.retorno.Professor.id_coordenador) {
                                cdProf = data.retorno.Professor.cd_pessoa;
                                dojo.byId("professor").value = data.retorno.Professor.no_fantasia;
                                dijit.byId("professor").set("disabled", true);
                                dojo.byId('cd_usuario_professor').value = cdProf;
                                no_professor = data.retorno.Professor.no_fantasia;
                                dojo.byId('nome_usuario_professor').value = data.retorno.no_fantasia;
                                if (eval(Master())) {
                                    cdProf = 0;
                                }
                            }
                        } else {
                            cdProf = 0;
                            dojo.byId('professor').value = "";
                            no_professor = "";
                        }
                        var myStore =
                            Cache(
                                    JsonRest({
                                        target: Endereco() + "/api/coordenacao/getDiarioAulaSearch?cd_turma=" + parseInt(0) + "&no_professor=" + no_professor + "&cd_tipo_aula=" + parseInt(0) + "&status=" + parseInt(0) +
                                            "&presProf=" + parseInt(0) + " &substituto=false&inicio=false&dtInicial=&dtFinal=&cdProf=" + cdProf + "&cd_escola_combo=0", // + (dijit.byId("escolaFiltroTelaDiario").value || dojo.byId("_ES0").value),
                                        handleAs: "json",
                                        preventCache: true,
                                        headers: { "Accept": "application/json", "Authorization": Token() }
                                    }), Memory({}));
                        //presProf,bool substituto, bool inicio, DateTime? dtInicial, DateTime? dtFinal)
                        var gridDiario = new EnhancedGrid({
                            store: ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }),
                            structure: [
                                { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                                { name: "Data Aula", field: "dta_aula", width: "8%", styles: "min-width:80px;", styles: "text-align: center;" },
                                { name: "Turma", field: "no_turma", width: "26%", styles: "min-width:80px;" },
                                { name: "Professor", field: "no_professor", width: "20%", styles: "min-width:80px;" },
                                { name: "Substituto", field: "id_substituto", width: "7%", styles: "min-width:80px;", styles: "text-align: center;" },//, formatter: formatCheckProfSubstituo },
                                { name: "Aula", field: "nm_aula_turma", width: "10%", styles: "min-width:80px;", styles: "text-align: center;" },
                                { name: "Tipo Aula", field: "no_tipo_atividade_extra", width: "14%", styles: "min-width:80px;" },
                                { name: "Status Aula", field: "desc_status", width: "10%", styles: "text-align: center;" },
                                { name: "Faltas", field: "dc_eventos", width: "5%", styles: "text-align: center;" }
                            ],
                            noDataMessage: msgNotRegEncFiltro,
                            canSort: true,
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
                                    position: "button"
                                }
                            }
                        }, "gridDiario");
                        gridDiario.startup();
                        gridDiario.pagination.plugin._paginator.plugin.connect(gridDiario.pagination.plugin._paginator, 'onSwitchPageSize', function (evt) {
                            try {
                                verificaMostrarTodos(evt, gridDiario, 'cd_tipo_atividade_extra', 'selecionaTodos');
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });

                        require(["dojo/aspect"], function (aspect) {
                            aspect.after(gridDiario, "_onFetchComplete", function () {
                                try {
                                    // Configura o check de todos:
                                    if (hasValue(dojo.byId('selecionaTodos')) && dojo.byId('selecionaTodos').type == 'text')
                                        setTimeout("configuraCheckBox(false, 'cd_tipo_atividade_extra', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', 'gridDiario')", gridDiario.rowsPerPage * 3);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            });
                        });
                        gridDiario.canSort = function (col) { return Math.abs(col) != 1 && Math.abs(col) != 4 && Math.abs(col) != 5 && Math.abs(col) != 8 };
                        gridDiario.on("RowDblClick", function (evt) {
                            try {
                                var idx = evt.rowIndex,
                                   item = this.getItem(idx),
                                   store = this.store;
                                configLayoutCadastro(EDIT);
                                destroyCreateGridAluno();
                                keepValues(item, gridDiario, false, xhr, ready, Memory, FilteringSelect);
                                //Problema assincrono(foi para o keepValue) - chamado 290346 
                                //IncluirAlterar(0, 'divAlterarDiario', 'divIncluirDiario', 'divExcluirDiario', 'apresentadorMensagemDiarioPartial', 'divCancelarDiario', 'divClearDiario');
                                //dijit.byId("cadDiarioAula").show();
                                
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        }, true);
                        var compDiarioAula = dijit.byId("id_status");
                        var statusStoreCurso = new Memory({
                            data: [
                              { name: "Efetivado", id: "0" },
                              { name: "Cancelada", id: "1" }
                            ]
                        });
                        compDiarioAula.store = statusStoreCurso;
                        compDiarioAula.set("value", 0);

                        var compStatusProf = dijit.byId("id_falta_pesq");
                        var statusFalta = new Memory({
                            data: [
                              { name: "Presente", id: "0" },
                              { name: "Falta", id: "1" },
                              { name: "Justificada", id: "2" }
                            ]
                        });
                        compStatusProf.store = statusFalta;
                        compStatusProf.set("value", 0);
                        // Adiciona link de ações:

                        var menu = new DropDownMenu({ style: "height: 25px" });
                        var acaoEditar = new MenuItem({
                            label: "Editar",
                            onClick: function () { eventoEditar(gridDiario.itensSelecionados, xhr, ready, Memory, FilteringSelect); }
                        });
                        menu.addChild(acaoEditar);

                        var acaoRemover = new MenuItem({
                            label: "Excluir",
                            onClick: function () { eventoRemoverDiario(gridDiario.itensSelecionados, xhr); }
                        });
                        menu.addChild(acaoRemover);

                        var acaoCancelar = new MenuItem({
                            label: "Cancelar",
                            onClick: function () { eventoCancelarDiario(gridDiario.itensSelecionados, xhr, cdProf); }
                        });
                        menu.addChild(acaoCancelar);

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
                            onClick: function () { buscarTodosItens(gridDiario, 'todosItens', ['pesquisarDiarioAula', 'relatorioDiarioAula']); pesquisarDiarioAula(false, cdProf); }
                        });
                        menu.addChild(menuTodosItens);

                        var menuItensSelecionados = new MenuItem({
                            label: "Itens Selecionados",
                            onClick: function () { buscarItensSelecionados('gridDiario', 'selecionado', 'cd_diario_aula', 'selecionaTodos', ['pesquisarDiarioAula', 'relatorioDiarioAula'], 'todosItens'); }
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
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesProTurmaFK");

                        var buttonFkArray = ['pesProTurmaFK'];

                        new Button({
                            label: "", iconClass: 'dijitEditorIconSearchSGF', onClick: function () {
                                try {
                                    apresentaMensagem("apresentadorMensagem", null);
                                    pesquisarDiarioAula(true, cdProf);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "pesquisarDiarioAula");
                        var buttonSearch = document.getElementById('pesquisarDiarioAula');
                        if (hasValue(buttonSearch)) {
                            buttonSearch.parentNode.style.minWidth = '40px';
                            buttonSearch.parentNode.style.width = '40px';
                        }
                        new Button({
                            label: "Limpar", iconClass: '', disabled: true, onClick: function () {
                                try {
                                    dojo.byId('cdTurma').value = 0;
                                    dojo.byId("desc_turma").value = "";
                                    dijit.byId('limparPesTurmaFK').set("disabled", true);
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "limparPesTurmaFK");
                        new Button({
                            label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage', onClick: function () {
                                try {
                                    var compTipoAula = dijit.byId("desc_tipo_aula");
                                    var compIdStatus = dijit.byId("id_status");
                                    var compStatusProfAula = dijit.byId("id_falta_pesq");
                                    var compTurma = dojo.byId("cdTurma");
                                    var cdTurma = hasValue(compTurma.value) ? compTurma.value : 0;
                                    var cdTipoAula = hasValue(compTipoAula.value) ? compTipoAula.value : 0;
                                    var cdStatus = hasValue(compIdStatus.value) ? compIdStatus.value : 0;
                                    var cdStatusProfAula = hasValue(compStatusProfAula.value) ? compStatusProfAula.value : 0;
                                    xhr.get({
                                        url: Endereco() + "/api/coordenacao/GeturlrelatorioDiarioAula?" + getStrGridParameters('gridDiario') + "&cd_turma=" + parseInt(cdTurma) + "&no_professor=" +
                                            dojo.byId("professor").value + "&cd_tipo_aula=" + parseInt(cdTipoAula) + "&status=" + parseInt(cdStatus) + "&presProf=" + parseInt(cdStatusProfAula) +
                                            "&substituto=" + document.getElementById("subst").checked + "&inicio=" + document.getElementById("inicio_professor").checked + "&dtInicial=" +
                                            dojo.byId("dt_inicial").value + "&dtFinal=" + dojo.byId("dt_final").value + "&cdProf=" + cdProf + "&cd_escola_combo=0", //+ (dijit.byId("escolaFiltroTelaDiario").value || dojo.byId("_ES0").value),
                                        preventCache: true,
                                        handleAs: "json",
                                        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
                                    }).then(function (data) {
                                        abrePopUp(Endereco() + '/Relatorio/RelatorioDinamico?' + data, '1024px', '750px', 'popRelatorio');
                                    },
                                    function (error) {
                                        apresentaMensagem('apresentadorMensagem', error);
                                    });
                                }
                                catch (e) {
                                    postGerarLog(e);
                                }
                            }
                        }, "relatorioDiarioAula");
                        new Button({
                            label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF', onClick:
                                function () {
                                    ready(function () {
                                        abreCadastroDiarioAula(xhr, ready, Memory, FilteringSelect, null, true);
                                    });
                                }
                        }, "novoDiarioAula");
                        if (hasValue(document.getElementById("limparPesTurmaFK"))) {
                            document.getElementById("limparPesTurmaFK").parentNode.style.minWidth = '40px';
                            document.getElementById("limparPesTurmaFK").parentNode.style.width = '40px';
                        }
                        for (var p = 0; p < buttonFkArray.length; p++) {
                            var buttonFk = document.getElementById(buttonFkArray[p]);

                            if (hasValue(buttonFk)) {
                                buttonFk.parentNode.style.minWidth = '18px';
                                buttonFk.parentNode.style.width = '18px';
                            }
                        }
                        //Configura que quando o usuario selecionar o tipo PPT marcara automaticamente turmas filhas.
                        dijit.byId("tipoTurmaFK").on("change", function (e) {
                            try {
                                if (this.displayedValue == "Personalizada") {
                                    dijit.byId("pesTurmasFilhasFK").set("checked", true);
                                    dijit.byId('pesTurmasFilhasFK').set('disabled', true);
                                } else {
                                    dijit.byId("pesTurmasFilhasFK").set("checked", false);
                                    dijit.byId('pesTurmasFilhasFK').set('disabled', false);
                                }
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        dijit.byId("proTurmaFK").on("show", function (e) {
                            try {
                                dijit.byId("gridPesquisaTurmaFK").update();
                            }
                            catch (e) {
                                postGerarLog(e);
                            }
                        });
                        findIsLoadComponetesPesquisaDiario(xhr, ready, Memory, FilteringSelect);
                        if (hasValue(dijit.byId("menuManual"))) {
                            dijit.byId("menuManual").on("click",
                                function(e) {
                                    try {
                                        abrePopUp(Endereco() + '/Content/manual/manual.htm#_Toc454323057',
                                            '765px',
                                            '771px');
                                    } catch (e) {
                                        postGerarLog(e);
                                    }
                                });
                        }
                        adicionarAtalhoPesquisa(['professor', 'desc_tipo_aula', 'id_status', 'id_falta_pesq', 'dt_inicial', 'dt_final'], 'pesquisarDiarioAula', ready);
                        showCarregando();
                    }
                    catch (e) {
                        postGerarLog(e);
                    }
                //});
                }, function (exeception) {
                    showCarregando();

                    //var mensagensWeb = new Array();
                    //mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, exeception);
                    //apresentaMensagem("apresentadorMensagem", mensagensWeb);

                    new Button({
                        label: "", iconClass: 'dijitEditorIconSearchSGF',
                    }, "pesquisarDiarioAula");

                    var buttonSearch = document.getElementById('pesquisarDiarioAula');
                    if (hasValue(buttonSearch)) {
                        buttonSearch.parentNode.style.minWidth = '40px';
                        buttonSearch.parentNode.style.width = '40px';
                    }
                    new Button({
                        label: "Novo", iconClass: 'dijitEditorIcon dijitEditorIconNewSGF'
                    }, "novoDiarioAula");

                    new Button({
                        label: getNomeLabelRelatorio(), iconClass: 'dijitEditorIcon dijitEditorIconNewPage'
                    }, "relatorioDiarioAula");
                
                    new Button({
                        label: "", iconClass: 'dijitEditorIcon dijitEditorIconFindSGF'
                    }, "pesProTurmaFK");

                    new Button({
                        label: "Limpar", iconClass: '', disabled: true
                    }, "limparPesTurmaFK");

                    var buttonFkArray = ['pesProTurmaFK'];

                    if (hasValue(document.getElementById("limparPesTurmaFK"))) {
                        document.getElementById("limparPesTurmaFK").parentNode.style.minWidth = '40px';
                        document.getElementById("limparPesTurmaFK").parentNode.style.width = '40px';
                    }
                    for (var p = 0; p < buttonFkArray.length; p++) {
                        var buttonFk = document.getElementById(buttonFkArray[p]);

                        if (hasValue(buttonFk)) {
                            buttonFk.parentNode.style.minWidth = '18px';
                            buttonFk.parentNode.style.width = '18px';
                        }
                    }

                    var myStore =
                           Cache(
                                JsonRest({
                                    target: Endereco() + "/api/escola/verificaRetornaSeUsuarioEProfessor",
                                    handleAs: "json",
                                    preventCache: true,
                                    headers: { "Accept": "application/json", "Authorization": Token() }
                                }), Memory({}));

                    var gridDiarioAula = new EnhancedGrid({
                        store: ObjectStore({ objectStore: myStore }),
                        structure: [
                            { name: "<input id='selecionaTodos' style='display:none'/>", field: "selecionado", width: "5%", styles: "text-align:center; min-width:15px; max-width:20px;", formatter: formatCheckBox },
                            { name: "Data Aula", field: "dta_aula", width: "8%", styles: "min-width:80px;", styles: "text-align: center;" },
                            { name: "Turma", field: "no_turma", width: "26%", styles: "min-width:80px;" },
                            { name: "Professor", field: "no_professor", width: "20%", styles: "min-width:80px;" },
                            { name: "Substituto", field: "id_substituto", width: "7%", styles: "min-width:80px;", styles: "text-align: center;" },//, formatter: formatCheckProfSubstituo },
                            { name: "Aula", field: "nm_aula_turma", width: "10%", styles: "min-width:80px;", styles: "text-align: center;" },
                            { name: "Tipo Aula", field: "no_tipo_atividade_extra", width: "14%", styles: "min-width:80px;" },
                            { name: "Status Aula", field: "desc_status", width: "10%", styles: "text-align: center;" }
                        ],
                        noDataMessage: msgNotRegEnc,
                        canSort: true,
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
                                position: "button"
                            }
                        }
                    }, "gridDiario");
                    gridDiarioAula.startup();
                });
            }
            catch (e) {
                postGerarLog(e);
            }
        });
    });
};

function formatCheckBox(value, rowIndex, obj) {
    try {
        var gridName = 'gridDiario';
        var grid = dijit.byId(gridName);
        var icon;
        var id = obj.field + '_Selected_' + rowIndex;
        var todos = dojo.byId('selecionaTodos');

        if (hasValue(grid.itensSelecionados && hasValue(grid._by_idx[rowIndex].item))) {
            var indice = binaryObjSearch(grid.itensSelecionados, "cd_diario_aula", grid._by_idx[rowIndex].item.cd_diario_aula);

            value = value || indice != null; // Item está selecionado.
        }
        if (rowIndex != -1)
            icon = "<input class='formatCheckBox'  id='" + id + "'/> ";

        // Configura o check de todos:
        if (hasValue(todos) && todos.type == 'text')
            setTimeout("configuraCheckBox(false, 'cd_diario_aula', 'selecionado', -1, 'selecionaTodos', 'selecionaTodos', '" + gridName + "')", grid.rowsPerPage * 3);

        setTimeout("configuraCheckBox(" + value + ", 'cd_diario_aula', 'selecionado', " + rowIndex + ", '" + id + "', 'selecionaTodos', '" + gridName + "')", 2);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function formatCheckProfSubstituo(value, rowIndex, obj) {
    try {
        var gridDiario = dijit.byId("gridDiario");
        var icon;
        var id = obj.field + '_Selected_' + gridDiario._by_idx[rowIndex].item.cd_diario_aula + '_4';
        if (value == null || value == undefined)
            value = true;
        if (hasValue(dijit.byId(id)))
            dijit.byId(id).destroy();

        if (rowIndex != -1 && hasValue(id))
            icon = "<input class='formatCheckBox' id='" + id + "' />";
        setTimeout(function () {
            configuraCheckBoxConsultaDiarioAula(value, rowIndex, id);
        }, 1);

        return icon;
    }
    catch (e) {
        postGerarLog(e);
    }
}

function configuraCheckBoxConsultaDiarioAula(value, rowIndex, id) {
    try {
        if (!hasValue(dijit.byId(id))) {
            var checkBox = new dijit.form.CheckBox({
                disabled: true,
                name: "checkBox",
                checked: value
            }, id);
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function findIsLoadComponetesPesquisaDiario(xhr, ready, Memory, filteringSelect) {
    xhr.get({
        url: Endereco() + "/api/coordenacao/getAtividadeExtraDiarioById?id=",
        preventCache: true,
        handleAs: "json",
        headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() }
    }).then(function (data) {
        try {
            apresentaMensagem("apresentadorMensagemTurma", null);
            if (data != null && jQuery.parseJSON(data).retorno != null)
                criarOuCarregarCompFiltering("desc_tipo_aula", jQuery.parseJSON(data).retorno, "", null, ready, Memory, filteringSelect, 'cd_tipo_atividade_extra', 'no_tipo_atividade_extra');
        }
        catch (e) {
            postGerarLog(e);
        }
    },
    function (error) {
        apresentaMensagem("apresentadorMensagem", error);
    });
}

//Metodos de C.R.U.D diário de aula.

function pesquisarDiarioAula(limparItens, cdProf) {
    require([
          "dojo/store/JsonRest",
          "dojo/data/ObjectStore",
          "dojo/store/Cache",
          "dojo/store/Memory"
    ], function (JsonRest,  ObjectStore, Cache, Memory) {
        try {
            var grid = dijit.byId("gridDiario");
            var compTipoAula = dijit.byId("desc_tipo_aula");
            var compIdStatus = dijit.byId("id_status");
            var compStatusProfAula = dijit.byId("id_falta_pesq");
            var compTurma = dojo.byId("cdTurma");
            var cdTurma = hasValue(compTurma.value) ? compTurma.value : 0;
            var cdTipoAula = hasValue(compTipoAula.value) ? compTipoAula.value : 0;
            var cdStatus = hasValue(compIdStatus.value) ? compIdStatus.value : 0;
            var cdStatusProfAula = hasValue(compStatusProfAula.value) ? compStatusProfAula.value : 0;

            var myStore = Cache(
                JsonRest({
                    target: Endereco() + "/api/coordenacao/getDiarioAulaSearch?cd_turma=" + parseInt(cdTurma) + "&no_professor=" + dojo.byId("professor").value +
                                      "&cd_tipo_aula=" + parseInt(cdTipoAula) + "&status=" + parseInt(cdStatus) + "&presProf=" + parseInt(cdStatusProfAula) + "&substituto=" + document.getElementById("subst").checked +
                                      "&inicio=" + document.getElementById("inicio_professor").checked + "&dtInicial=" + dojo.byId("dt_inicial").value + "&dtFinal=" + dojo.byId("dt_final").value + "&cdProf=" + cdProf +
                                      "&cd_escola_combo=0", //+ (dijit.byId("escolaFiltroTelaDiario").value || dojo.byId("_ES0").value),
                    handleAs: "json",
                    preventCache: true,
                    headers: { "Accept": "application/json", "Authorization": Token() }
                }), Memory({}));

            var dataStore = new ObjectStore({ objectStore: myStore });
            if (limparItens)
                grid.itensSelecionados = [];
            grid.noDataMessage = msgNotRegEnc;
            grid.setStore(dataStore);
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
        dojo.byId('tipoRetornoAlunoFK').value = PESQUISADIARIOAULA;
        limparFiltrosTurmaFK();
        //Configuração retorno fk de turma
        dojo.byId("idOrigemCadastro").value = DIARIOAULA;
        dojo.byId("id_origem_diarioAula").value = PESQTURMADIARIOAULAFILTRO;
        dijit.byId("tipoTurmaFK").store.remove(0);
        if (hasValue(dijit.byId('tipoTurmaFK'))) {
            var compProfpesquisa = dijit.byId('pesProfessorFK');
            dijit.byId('tipoTurmaFK').set('value', 1);
            if (compProfpesquisa.disabled == true && hasValue(compProfpesquisa.value))
                compProfpesquisa.set('disabled', true);
        }
        var gridPesquisaTurmaFK = dijit.byId("gridPesquisaTurmaFK");
        gridPesquisaTurmaFK.setStore(null);
        /*--combo_escola_fk
        //mostra a combo de Escola
        dojo.byId("trEscolaTurmaFiltroFk").style.display = "";
        dojo.byId("lbEscolaTurmaFiltroFk").style.display = "";
        require(['dojo/dom-style', 'dijit/registry'],
            function (domStyle, registry) {

                domStyle.set(registry.byId("escolaTurmaFiltroFK").domNode, 'display', '');
            });*/

        pesquisarTurmaFK(cdProf);
        dijit.byId("proTurmaFK").show();
        dojo.byId("legendTurmaFK").style.visibility = "hidden";
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoEditar(itensSelecionados, xhr, ready, Memory, FilteringSelect) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegUpdate, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegUpdate, null);
        else {
            var gridDiario = dijit.byId('gridDiario');
            apresentaMensagem('apresentadorMensagem', null);
            destroyCreateGridAluno();
            setarTabCadDiarioAula();
            configLayoutCadastro(EDIT);
            keepValues(null, gridDiario, true, xhr, ready, Memory, FilteringSelect);
            //Problema assincrono(foi para o keepValue) - chamado 290346 
            //IncluirAlterar(0, 'divAlterarDiario', 'divIncluirDiario', 'divExcluirDiario', 'apresentadorMensagemDiarioPartial', 'divCancelarDiario', 'divClearDiario');
            //dijit.byId("cadDiarioAula").show();
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoRemoverDiario(itensSelecionados, xhr) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelectRegExcluir, null);
        else
            caixaDialogo(DIALOGO_CONFIRMAR, '', function () { deletarDiarios(itensSelecionados, xhr); });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function eventoCancelarDiario(itensSelecionados, xhr, cdProf) {
    try {
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            caixaDialogo(DIALOGO_AVISO, msgSelecioneAlgumCancelar, null);
        else if (itensSelecionados.length > 1)
            caixaDialogo(DIALOGO_ERRO, msgSelectOneRegCancelar, null);
        else {
            caixaDialogo(DIALOGO_CONFIRMAR, 'Deseja cancelar esse registro?', function () { cancelarDiario(itensSelecionados, xhr, cdProf); });
        }
    }
    catch (e) {
        postGerarLog(e);
    }
}

function cancelarDiario(itensSelecionados, xhr, cdProf) {
    try {
        apresentaMensagem("apresentadorMensagem", null);
        if (itensSelecionados[0].id_status_aula == AULACANCELADA) {
            var mensagensWeb = new Array();
            mensagensWeb[0] = new MensagensWeb(MENSAGEM_ERRO, msgErroNotCancelDiarioJaCancelado);
            apresentaMensagem("apresentadorMensagem", mensagensWeb);
            return false;
        }
        xhr.post({
            url: Endereco() + "/api/coordenacao/postCancelarDiario",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify({ cd_diario_aula: itensSelecionados[0].cd_diario_aula })
        }).then(function (data) {
            try {
                apresentaMensagem('apresentadorMensagem', data);
                pesquisarDiarioAula(false, cdProf);
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function salvarDiarioAula(xhr) {
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
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridDiario';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    data = data.retorno;

                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    if (!hasValue(grid.store.objectStore) || !hasValue(grid.store.objectStore.data))
                        grid.setStore(new dojo.data.ObjectStore({ objectStore: new dojo.store.Memory({ data: null }) }));
                    insertObjSort(grid.itensSelecionados, "cd_diario_aula", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_diario_aula', 'selecionaTodos', ['pesquisarDiarioAula', 'relatorioDiarioAula'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    setGridPagination(grid, itemAlterado, "cd_diario_aula");
                    showCarregando();
                    dijit.byId("cadDiarioAula").hide();
                } else {
                    apresentaMensagem('apresentadorMensagemDiarioPartial', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
                showCarregando();
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function alterarDiarioAula(xhr) {
    try {
        var diario = null;
        if (!dijit.byId("formDiarioPrincipal").validate()) {
            setarTabCadDiarioAula();
            return false;
        }
        diario = mountDataForPostDiadrio();
        showCarregando();
        xhr.post({
            url: Endereco() + "/api/coordenacao/postUpdateDiarioAula",
            handleAs: "json",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(diario)
        }).then(function (data) {
            try {
                data = jQuery.parseJSON(data);
                if (!hasValue(data.erro)) {
                    var itemAlterado = data.retorno;
                    var gridName = 'gridDiario';
                    var grid = dijit.byId(gridName);

                    apresentaMensagem('apresentadorMensagem', data);
                    if (!hasValue(grid.itensSelecionados))
                        grid.itensSelecionados = [];
                    removeObjSort(grid.itensSelecionados, "cd_diario_aula", dojo.byId("cd_diario_aula").value);
                    insertObjSort(grid.itensSelecionados, "cd_diario_aula", itemAlterado);
                    buscarItensSelecionados(gridName, 'selecionaTodos', 'cd_diario_aula', 'selecionaTodos', ['pesquisarDiarioAula', 'relatorioDiarioAula'], 'todosItens');
                    grid.sortInfo = 2; // Configura a segunda coluna (código) como a coluna de ordenação.
                    if (hasValue(grid.store.objectStore.data))
                        setGridPagination(grid, itemAlterado, "cd_diario_aula");
                    showCarregando();
                    dijit.byId("cadDiarioAula").hide();
                }
                else {
                    apresentaMensagem('apresentadorMensagemDiarioPartial', data);
                    showCarregando();
                }
            }
            catch (e) {
                postGerarLog(e);
            }
        }, function (error) {
            apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            showCarregando();
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function deletarDiarios(itensSelecionados, xhr) {
    try {
        var elemCodProf = dojo.byId('cd_usuario_professor');
        var cdProf = hasValue(elemCodProf.value) ? elemCodProf.value : 0;
        if (!hasValue(itensSelecionados) || itensSelecionados.length <= 0)
            if (dojo.byId('cd_diario_aula').value != 0)
                itensSelecionados = [{
                    cd_diario_aula: dojo.byId("cd_diario_aula").value
                }];
        xhr.post({
            url: Endereco() + "/api/coordenacao/postDeleteDiarios",
            headers: { "Accept": "application/json", "Content-Type": "application/json", "Authorization": Token() },
            postData: JSON.stringify(itensSelecionados)
        }).then(function (data) {
            try {
                var todos = dojo.byId("todosItens_label");
                apresentaMensagem('apresentadorMensagem', data);
                dijit.byId("desc_turma").set("value", '');
                dojo.byId("cdTurma").value = 0;
                // Remove o item dos itens selecionados:
                if (hasValue(itensSelecionados))
                    for (var r = itensSelecionados.length - 1; r >= 0; r--)
                        removeObjSort(dijit.byId('gridDiario').itensSelecionados, "cd_diario_aula", itensSelecionados[r].cd_diario_aula);
                pesquisarDiarioAula(false, cdProf);
                // Habilita os botões de pesquisar e relatório, pois estão dependentes dos filtros de pesquisa de itens não selecionados:
                dijit.byId("pesquisarDiarioAula").set('disabled', false);
                dijit.byId("relatorioDiarioAula").set('disabled', false);
                if (hasValue(todos))
                    todos.innerHTML = "Todos Itens";
                dijit.byId("cadDiarioAula").hide();
            }
            catch (e) {
                postGerarLog(e);
            }
        },
        function (error) {
            if (!hasValue(dojo.byId("cadDiarioAula").style.display))
                apresentaMensagem('apresentadorMensagemDiarioPartial', error);
            else
                apresentaMensagem('apresentadorMensagem', error);
        });
    }
    catch (e) {
        postGerarLog(e);
    }
}

function setarEventosBotoesPrincipais(xhr, on, ready, Memory, FilteringSelect) {
    try {
        dijit.byId("incluirDiario").on("click", function () {
            try {
                apresentaMensagem('apresentadorMensagemDiarioPartial', null);
                salvarDiarioAula(xhr);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
        dijit.byId("alterarDiario").on("click", function () {
            try {
                apresentaMensagem('apresentadorMensagemDiarioPartial', null);
                alterarDiarioAula(xhr);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
        dijit.byId("deleteDiario").on("click", function () {
            try {
                apresentaMensagem('apresentadorMensagemDiarioPartial', null);
                caixaDialogo(DIALOGO_CONFIRMAR, '', function executaRetorno() { deletarDiarios(null, xhr) });
            }
            catch (e) {
                postGerarLog(e);
            }
        })
        dijit.byId("cancelarDiario").on("click", function () {
            try {
                configLayoutCadastro(EDIT);
                destroyCreateGridAluno();
                keepValues(null, dijit.byId("gridDiario"), null, xhr, ready, Memory, FilteringSelect);
            }
            catch (e) {
                postGerarLog(e);
            }
        })
    }
    catch (e) {
        postGerarLog(e);
    }
}


function findAllEmpresasUsuarioComboFiltroEscolaTelaDiario() {
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
                        loadSelect(dataRetorno, "escolaFiltroTelaDiario", 'cd_pessoa', 'dc_reduzido_pessoa', dojo.byId("_ES0").value);
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
